using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.BackgroundTasks
{
    public class IssueResultsAndCredentialsBackgroundTask : ApplicationBackgroundTask,
        IIssueResultsAndCredentialsBackgroundTask
    {
        private readonly IApplicationQueryService _applicationQueryService;
        private readonly ITestResultQueryService _testResultQueryService;

        public IssueResultsAndCredentialsBackgroundTask(
            ISystemQueryService systemQueryService,
            ITestResultQueryService testResultQueryService,
            IApplicationQueryService applicationQueryService, IBackgroundTaskLogger backgroundTaskLogger, IUtilityQueryService utilityQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _testResultQueryService = testResultQueryService;
            _applicationQueryService = applicationQueryService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            IssueTestResults();
            IssueRecertificationCredentials();
        }

        private void IssueTestResults()
        {
            TaskLogger.WriteInfo("Getting test Test results to issue...");
            var pendingTestResults = _testResultQueryService.GetPendingTestsToIssueResult().Data;

            TaskLogger.WriteInfo("Processing test results...");
            pendingTestResults.ForEach(ProcessTestResults);
        }

        private void IssueRecertificationCredentials()
        {
            TaskLogger.WriteInfo("Getting Recertification applications to issue...");
            var pendingRecertifications = GetAvailableRecertificationApplicationsToIssue();

            TaskLogger.WriteInfo("Processing recertification application...");
            ExecuteIfRunning(
                pendingRecertifications,
                x => ExecuteAction(SystemActionTypeName.IssueRecertificationCredentials, x));

            TaskLogger.WriteInfo("Getting Recertification applications to assess...");
            pendingRecertifications = GetAvailableToAssess();

            TaskLogger.WriteInfo("Assessing credentials...");
            ExecuteIfRunning(pendingRecertifications, x => ExecuteAction(SystemActionTypeName.AssessCredentials, x));
        }

        private bool ExecuteAction(SystemActionTypeName action, ApplicationSearchDto application)
        {
            TaskLogger.WriteInfo("Executing action: {Action}, ApplicationId: {ApplicationId}", action, application.Id);
            var applicationId = application.Id;
            var model = new ApplicationActionWizardModel
            {
                ApplicationId = applicationId,
                ActionType = (int)action
            };
            model.SetBackGroundAction(true);
            try
            {
                var response = ServiceLocator.Resolve<IApplicationService>().PerformAction(model);
                LogResponse(response, "Recertification", applicationId);

                TaskLogger.AdProcessedApplication($"ApplicationId : {applicationId}");

                return true;
            }

            catch (UserFriendlySamException ex)
            {
                TaskLogger.WriteApplicationError(ex, applicationId, "Recertification", string.Empty, true);
            }
            catch (Exception ex)
            {
                TaskLogger.WriteApplicationError(ex, applicationId, "Recertification", string.Empty, false);
            }

            return false;
        }

        private void ProcessTestResults(TestResultInfoDto testResult)
        {
            if (testResult.TotalOriginalExaminers > 2)
            {
                LoggingHelper.LogError($"ApplicationId: {testResult.CredentialApplicationId}, CredentialRequestId: {testResult.CredentialRequestId}, " +
                    $"TestResultId: {testResult.TestResultId}. 'There are more than 2 original examiner assigned. No action carried out.'");
                return;
            }

            var action = SystemActionTypeName.ComputeFinalMarks;

            if (ValidateAction(action, testResult))
            {
                if (ExecuteAction(action, testResult))
                {
                    // check if practice ccl. If yes then issue practice test results action.
                    var isCclPracticeTestResponse = _testResultQueryService.IsCclPracticeTest(testResult.CredentialRequestId);

                    if (!isCclPracticeTestResponse.Success)
                    {
                        throw new Exception(string.Join(".", isCclPracticeTestResponse.Errors));
                    }

                    if (isCclPracticeTestResponse.Data)
                    {
                        action = SystemActionTypeName.IssuePracticeTestResults;

                        var changeTestResultStatusResponse = _testResultQueryService.UpdateTestResultToIssuePracticeResult(testResult.TestResultId);

                        if (!changeTestResultStatusResponse.Success)
                        {
                            throw new Exception(string.Join(".", changeTestResultStatusResponse.Errors));
                        }

                        if (ValidateAction(action, testResult))
                        {
                            ExecuteAction(action, testResult);
                        }

                        return;
                    }

                    var testaResult = _testResultQueryService.GetTestResultById(testResult.TestResultId);

                    if (testaResult.Result.ResultTypeId == (int)TestResultStatusTypeName.Passed)
                    {
                        action = SystemActionTypeName.IssuePass;
                        if (ExecuteAction(action, testResult))
                        {
                            action = SystemActionTypeName.IssueCredential;
                            ExecuteAction(action, testResult);
                        }
                    }

                    if (testaResult.Result.ResultTypeId == (int)TestResultStatusTypeName.Failed)
                    {
                        action = SystemActionTypeName.IssueFail;
                        ExecuteAction(action, testResult);
                    }
                }
            }
        }

        private bool ExecuteAction(SystemActionTypeName action, TestResultInfoDto testResult)
        {
            TaskLogger.WriteInfo(
                "Executing action: {Action}. ApplicationId: {ApplicationId}. CredentialRequestId: {CredentialRequestId}",
                action,
                testResult.CredentialApplicationId,
                testResult.CredentialRequestId);
            var applicationId = testResult.CredentialApplicationId;
            var model = new ApplicationActionWizardModel
            {
                ApplicationId = applicationId,
                CredentialRequestId = testResult.CredentialRequestId,
                ActionType = (int)action
            };
            model.SetBackGroundAction(true);
            try
            {
                ServiceLocator.Resolve<IApplicationService>().PerformAction(model);
                TaskLogger.AdProcessedApplication(
                    $"ApplicationId : {testResult.CredentialApplicationId}, CredentialRequestId: {testResult.CredentialRequestId}");
                return true;
            }
            catch (UserFriendlySamException ex)
            {
                TaskLogger.WriteApplicationError(ex, applicationId, "TestResult", string.Empty, true);
            }
            catch (Exception ex)
            {
                TaskLogger.WriteApplicationError(ex, applicationId, "TestResult", string.Empty, false);
            }

            return false;
        }

        private bool ValidateAction(SystemActionTypeName action, TestResultInfoDto testResult)
        {
            TaskLogger.WriteInfo(
                "Validating action: {Action}. ApplicationId: {ApplicationId}. CredentialRequestId: {CredentialRequestId}",
                action,
                testResult.CredentialApplicationId,
                testResult.CredentialRequestId);
            var applicationId = testResult.CredentialApplicationId;
            var model = new ApplicationActionWizardModel
            {
                ApplicationId = applicationId,
                CredentialRequestId = testResult.CredentialRequestId,
                ActionType = (int)action
            };
            model.SetBackGroundAction(true);
            try
            {
                var errors = ServiceLocator.Resolve<IApplicationService>().ValidateActionPreconditions(model).Data
                    .ToList();
                foreach (var error in errors)
                {
                    TaskLogger.WriteWarning(
                        $"ApplicationId: {{ApplicationId}}, CredentialRequestId: {{CredentialRequestId}}, TestResultId:{{TestResultId}}. {error.Message}",
                        testResult.CredentialApplicationId,
                        testResult.CredentialRequestId,
                        testResult.TestResultId);
                }

                return !errors.Any();
            }

            catch (UserFriendlySamException ex)
            {
                TaskLogger.WriteApplicationError(ex, applicationId, "TestResult", string.Empty, true);
                return false;
            }
            catch (Exception ex)
            {
                TaskLogger.WriteApplicationError(ex, applicationId, "TestResult", string.Empty, false);
                return false;
            }
        }

        private IEnumerable<ApplicationSearchDto> GetAvailableRecertificationApplicationsToIssue()
        {
            var receritificationBatchPaymentWaitDays =
                Convert.ToInt32(GetSystemValue("AutoReceritificationBatchPaymentWaitDays"));
            var filters = new[]
            {
                new ApplicationSearchCriteria
                {
                    Filter = ApplicationFilterType.ApplicationStatusIntList,
                    Values = new[] { ((int)CredentialApplicationStatusTypeName.InProgress).ToString() }
                },
                new ApplicationSearchCriteria
                {
                    Filter = ApplicationFilterType.CredentialRequestStatusIntList,
                    Values = new[] { ((int)CredentialRequestStatusTypeName.ToBeIssued).ToString() }
                },
                new ApplicationSearchCriteria
                {
                    Filter = ApplicationFilterType.StatusModifiedDateTo,
                    Values = new[]
                        { DateTime.Now.Date.AddDays(-receritificationBatchPaymentWaitDays).Date.ToFilterString() }
                }
            };

            var response =
                _applicationQueryService.SearchApplication(new GetApplicationSearchRequest { Filters = filters });

            return response.Results;
        }

        private IEnumerable<ApplicationSearchDto> GetAvailableToAssess()
        {
            var filters = new[]
            {
                new ApplicationSearchCriteria
                {
                    Filter = ApplicationFilterType.ApplicationStatusIntList,
                    Values = new[] { ((int)CredentialApplicationStatusTypeName.InProgress).ToString() }
                },
                new ApplicationSearchCriteria
                {
                    Filter = ApplicationFilterType.CredentialRequestStatusIntList,
                    Values = new[] { ((int)CredentialRequestStatusTypeName.ProcessingRequest).ToString() }
                }
            };

            var response =
                _applicationQueryService.SearchApplication(new GetApplicationSearchRequest { Filters = filters });

            return response.Results;
        }
    }
}