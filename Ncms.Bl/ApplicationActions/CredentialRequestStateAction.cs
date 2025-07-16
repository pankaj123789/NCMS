using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.System;
using Ncms.Contracts.Models.Test;
using TestTaskModel = Ncms.Contracts.TestTaskModel;

namespace Ncms.Bl.ApplicationActions
{
    public abstract class CredentialRequestStateAction : ApplicationStateAction
    {
        protected virtual CredentialRequestStatusTypeName[] CredentialRequestEntryStates { get; }
        protected virtual CredentialRequestStatusTypeName CredentialRequestExitState { get; }

        protected virtual CredentialRequestStatusTypeName CurrentRequestEntryState { get; private set; }

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.CredentialRequest;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Update;

        protected virtual TestResultStatusTypeName RequiredTestResultStatus => TestResultStatusTypeName.TestInvalidated;
        protected CredentialRequestModel CredentialRequestModel { get; set; }

        private CredentialRequestTestSessionModel _TestSessionModel;
        private GetMarksResponseModel _StandardMarks;
        private TestRubricModel _RubricMarks;
        private IEnumerable<TestTaskModel> _TestSpecificationComponents;
        private TestSpecificationInfoModel _TestSpecification;
        private ApplicationBriefModel _applicationBriefModel;
        private RefundModel _RefundRequest; 

        protected CredentialRequestTestSessionModel TestSessionModel => _TestSessionModel ?? (_TestSessionModel = GetTestSessionModel());
        protected GetMarksResponseModel StandardMarks => _StandardMarks ?? (_StandardMarks = GetStandardMarks());
        protected ApplicationBriefModel ApplicationBriefsModel => _applicationBriefModel ?? (_applicationBriefModel = GetPendingBriefDocuments());
        protected TestSpecificationInfoModel TestSpecification => _TestSpecification ?? (_TestSpecification = GetTestSpecification());
        protected TestRubricModel RubricMarks => _RubricMarks ?? (_RubricMarks = GetRubricMarks());
        protected RefundModel RefundRequest => _RefundRequest ?? (_RefundRequest = GetRefundRequest());

        protected virtual RefundModel GetRefundRequest()
        {
            return CredentialRequestModel.RefundRequests
                .OrderByDescending(refundRequest => refundRequest.Id)
                .FirstOrDefault();
        }

        private bool? _allowSupplementary;
        public bool AllowSupplementary => (_allowSupplementary ?? (_allowSupplementary = GetAllowSupplementaryFlag())).Value;

        private bool? _allConcededPass;
        public bool AllowConcededPass => (_allConcededPass ?? (_allConcededPass = GetAllowConcededPassFlag())).Value;


        private bool? _allowPaidReview;
        public bool AllowPaidReview => _allowPaidReview ?? (_allowPaidReview = GetAllowPaidReviewFlag()).Value;

        protected IEnumerable<TestTaskModel> TestSpecificationComponents => _TestSpecificationComponents ?? (_TestSpecificationComponents = GetTestSpecificationComponents());



        private IList<LookupTypeModel> _skillTypes;

        protected IList<LookupTypeModel> SKillTypes => _skillTypes ?? (_skillTypes = ApplicationService.GetSkillsForCredentialType(new[] { CredentialRequestModel.CredentialTypeId }, null).Data
                                                           .Select(AutoMapperHelper.Mapper.Map<LookupTypeModel>)
                                                           .ToList());

        protected override void ConfigureInstance(CredentialApplicationDetailedModel application, ApplicationActionWizardModel wizardModel, ApplicationActionOutput output)
        {
            base.ConfigureInstance(application, wizardModel, output);
            if (wizardModel.CredentialRequestId > 0)
            {
                CredentialRequestModel = application.CredentialRequests.SingleOrDefault(x => x.Id == wizardModel.CredentialRequestId);
                if (CredentialRequestModel == null)
                {
                    throw new Exception($"Credential Request {wizardModel.CredentialRequestId} not found on Application model.");
                }

                CurrentRequestEntryState = (CredentialRequestStatusTypeName) CredentialRequestModel.StatusTypeId;
            }
        }

        protected virtual CredentialRequestTestSessionModel GetTestSessionModel()
        {
            return CredentialRequestModel.TestSessions.OrderByDescending(x => x.CredentialTestSessionId).FirstOrDefault(x => !x.Rejected && x.Supplementary == CredentialRequestModel.Supplementary);
        }

        protected virtual GetMarksResponseModel GetStandardMarks()
        {
            if (TestSessionModel != null && TestSessionModel.MarkingSchemaTypeId != (int)MarkingSchemaTypeName.Standard)
            {
                throw new Exception("Test does not support standard Marking");
            }

            if (TestSessionModel?.TestResultId != null)
            {
                var testResultId = TestSessionModel.TestResultId;
                return TestResultService.GetMarks(new GetMarksRequestModel { TestResultId = testResultId.GetValueOrDefault() });
            }

            return null;
        }

        protected virtual IEnumerable<TestTaskModel> GetTestSpecificationComponents()
        {
            var testTestSession = TestSessionModel;

            return TestMaterialService.GetTestSpecificationComponents(testTestSession.TestSpecificationId).Results;
        }

        protected virtual ApplicationBriefModel GetPendingBriefDocuments()
        {
            var pendingBriefs = TestMaterialService.GetPendingCandidateBriefsToSend(new PendingBriefRequest()
            {
                TestSittingId = TestSessionModel.CredentialTestSessionId,
                CredentialRequestStatus = CredentialRequestStatusTypeName.TestSessionAccepted,
                SendDate = DateTime.Now.Date
            }).Data.FirstOrDefault();

            return pendingBriefs;
        }

        protected virtual TestRubricModel GetRubricMarks()
        {
            if (TestSessionModel != null && TestSessionModel.MarkingSchemaTypeId != (int)MarkingSchemaTypeName.Rubric)
            {
                throw new Exception("Test does not support Rubric Marking");
            }

            if (TestSessionModel?.TestResultId != null)
            {
                var testResultId = TestSessionModel.TestResultId.GetValueOrDefault();
                return TestService.GetTestResultRubricMarking(testResultId).Data;
            }

            return null;
        }

        protected override ProductSpecificationModel GetFee(FeeTypeName feeTypeName)
        {
            if (WizardModel.DoNotInvoice)
            {
                return null;
            }

            switch (feeTypeName)
            {
                case FeeTypeName.SupplementaryTest:
                    return ApplicationService.GetSupplementaryTestFee(ApplicationModel.ApplicationType.Id, CredentialRequestModel.CredentialType.Id);
                case FeeTypeName.Test:
                    return ApplicationService.GetTestFee(ApplicationModel.ApplicationType.Id, CredentialRequestModel.CredentialType.Id);
                case FeeTypeName.PaidTestReview:
                    return ApplicationService.GetPaidReviewFee(ApplicationModel.ApplicationType.Id, CredentialRequestModel.CredentialType.Id);
                default:
                    throw new UserFriendlySamException("No fee operation found with given fee type in the system.");
            }
        }

        /*** preconditions ***/

        protected override void ValidateEntryState()
        {
            if (!CredentialRequestEntryStates.Contains((CredentialRequestStatusTypeName)CredentialRequestModel.StatusTypeId))
            {
                var entryStateNames = CredentialRequestEntryStates.Select(x => CredentialRequestStatusTypes.SingleOrDefault(y => y.Id == (int)x)?.DisplayName);
                throw new UserFriendlySamException(
                   String.Format(Naati.Resources.Application.WrongCredentialRequestStatusErrorMessage,
                       String.Join(", ", entryStateNames)));
            }
        }

        protected virtual void ValidateApplicationState()
        {
            if (ApplicationModel.ApplicationStatus.Id != (int)CredentialApplicationStatusTypeName.InProgress)
            {
                throw new UserFriendlySamException(String.Format(Naati.Resources.Application.WrongApplicationStatusErrorMessage,
                    CredentialApplicationStatusTypes.Single(x => x.Id == (int)CredentialApplicationStatusTypeName.InProgress).DisplayName));
            }
        }

        protected virtual void ValidateCredentialRequestInvoices()
        {
            var response = ApplicationService.GetWorkflowCredentialRequestActionInvoices(WizardModel.ActionType, WizardModel.CredentialRequestId);
            Validate(response);
        }


        /*** system actions ***/

        protected override string GetNote()
        {
            return String.Format(Naati.Resources.Application.CredentialRequestStateChangeNote,
                CredentialRequestModel.CredentialType.InternalName,
                CredentialRequestModel.Direction,
                CredentialRequestModel.Status,
                CredentialRequestStatusTypes.Single(x => x.Id == (int)CredentialRequestExitState).DisplayName);
        }

        protected override void SetExitState()
        {
            CredentialRequestModel.StatusTypeId = (int)CredentialRequestExitState;
            CredentialRequestModel.StatusChangeUserId = CurrentUser.Id;
            CredentialRequestModel.StatusChangeDate = DateTime.Now;
        }

        protected virtual void SetApplicationStatus()
        {
            // UC 5000 BR15
            var finalStates = new[]
                              {
                                  CredentialRequestStatusTypeName.Rejected,
                                  CredentialRequestStatusTypeName.AssessmentFailed,
                                  CredentialRequestStatusTypeName.TestFailed,
                                  CredentialRequestStatusTypeName.CertificationIssued,
                                  CredentialRequestStatusTypeName.Cancelled,
                                  CredentialRequestStatusTypeName.Deleted,
                                  CredentialRequestStatusTypeName.Withdrawn,
                                  CredentialRequestStatusTypeName.TestInvalidated,
                                  CredentialRequestStatusTypeName.IssuePracticeTestResults
            };

            var allOtherCredentialRequests = ApplicationModel.CredentialRequests.Where(x => x.Id != CredentialRequestModel.Id);

            if(!allOtherCredentialRequests.Any())
            {
                var action = CreateAction(SystemActionTypeName.FinaliseApplication, ApplicationModel, WizardModel);
                action.Perform();

                return;
            }

            if (allOtherCredentialRequests.All(x => finalStates.Contains((CredentialRequestStatusTypeName)x.StatusTypeId)))
            {
                var action = CreateAction(SystemActionTypeName.FinaliseApplication, ApplicationModel, WizardModel);
                action.Perform();

                return;
            }
        }

        public virtual void RejectTestSession()
        {
            if (TestSessionModel != null)
            {
                TestSessionModel.Rejected = true;
                TestSessionModel.RejectedDate = DateTime.Now; 
                CreateTestSessionRejectedNote(TestSessionModel);

            }
        }

        protected virtual void CreateTestSessionRejectedNote(CredentialRequestTestSessionModel testSessionModel)
        {
            var noteModel = new ApplicationNoteModel
            {
                ApplicationId = ApplicationModel.ApplicationInfo.ApplicationId,
                CreatedDate = DateTime.Now,
                Note = string.Empty,
                UserId = CurrentUser.Id,
                ReadOnly = true
            };

            var credentialTypeName = CredentialRequestModel.CredentialType.InternalName;
            var skill = CredentialRequestModel.Skill.DisplayName;
            var testSessionId = testSessionModel.TestSessionId;
            var testSessionDate = testSessionModel.TestDate.ToShortDateString();
            noteModel.Note = string.Format(Naati.Resources.Application.TestSessionRemovedNote, credentialTypeName, skill, testSessionId, testSessionDate);

            ApplicationModel.Notes.Add(noteModel);
        }
        public virtual void ValidateTestSessionDate()
        {
            if (TestSessionModel == null)
            {
                throw new UserFriendlySamException(Naati.Resources.Application.TestSessionNotFound);
            }

            if (TestSessionModel.TestDate.Date > DateTime.Now.Date)
            {
                throw new UserFriendlySamException(Naati.Resources.Application.TestSessionIsInFuture);
            }
        }

        protected virtual void GetStandardTestResultEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            if (TestSpecification.MarkingSchemaTypeId != (int)MarkingSchemaTypeName.Standard)
            {
                return;
            }
            var componentsTable = GetStandardTestResultsTableString();
            var generalComments = StandardMarks.CommentsGeneral ?? string.Empty;
            generalComments = generalComments.Replace("\n", "<br/>");
            var testDate = TestSessionModel.TestDate;
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestResultsStandardMarkingScheme), componentsTable);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestComments), generalComments);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionDate), testDate.ToString("dd MMMM yyyy"));
        }

        public virtual void GetRubricTestResultEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            if (TestSpecification.MarkingSchemaTypeId != (int)MarkingSchemaTypeName.Rubric)
            {
                return;
            }

            var componentsTable = GetRubricTestResultsTableString();
            var testDate = TestSessionModel.TestDate;
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestResultsStandardMarkingScheme), componentsTable);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.TestSessionDate), testDate.ToString("dd MMMM yyyy"));
        }


        protected virtual string GetStandardTestResultsTableString()
        {
            if (StandardMarks == null || StandardMarks.Components.Count == 0)
            {
                return string.Empty;
            }

            var components = StandardMarks.Components;
            var overallPassMark = StandardMarks.OverAllPassMark;

            var builder = new StringBuilder();
            builder.Append("<div class=\"test-results\">");
            builder.Append("<table class=\"results-table\">");
            builder.Append("<tbody>");

            foreach (var component in components)
            {
                var componentName = component.Name;
                var componentPassMark = component.PassMark;
                var compomentResult = component.Mark;
                var compomentTotalMark = component.TotalMarks;
                builder.Append("<tr>");
                builder.Append("<td>");
                builder.Append($"{componentName}: (minimum {componentPassMark})");
                builder.Append("</td>");
                builder.Append("<td>");
                builder.Append($"{compomentResult} / {compomentTotalMark}");
                builder.Append("</td>");
                builder.Append("</tr>");
            }

            builder.Append("<tr>");
            builder.Append("<td>");
            builder.Append(string.Empty);
            builder.Append("</td>");
            builder.Append("<td>");
            builder.Append(string.Empty);
            builder.Append("</td>");
            builder.Append("</tr>");

            builder.Append("</tbody>");

            builder.Append("<tfoot>");
            builder.Append("<tr>");
            builder.Append("<th>");
            builder.Append($"Overall Mark (minimum {overallPassMark.OverAllPassMark})");
            builder.Append("</th>");
            builder.Append("<th>");
            builder.Append($"{components.Sum(x => x.Mark)} / {components.Sum(x => x.TotalMarks)}");
            builder.Append("</th>");
            builder.Append("</tr>");
            builder.Append("</tfoot>");

            builder.Append("</table>");
            builder.Append("</div>");

            return builder.ToString();
        }

        protected virtual string GetRubricTestResultsTableString()
        {
            if (RubricMarks == null || RubricMarks.TestComponents.Count == 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();

            var taskTypes = RubricMarks.TestComponents.GroupBy(c => new { TypeLabel = c.TypeLabel, TypeDescription = c.TypeDescription });

            foreach (var taskType in taskTypes)
            {
                builder.Append($"<p><strong>Task Type {taskType.Key.TypeLabel}: {taskType.Key.TypeDescription}</strong></p>");

                foreach (var component in taskType)
                {
                    var result = "<strong>Not Attempted</strong>";
                    var wasAttempted = component.WasAttempted.GetValueOrDefault();

                    if (wasAttempted)
                    {
                        result = component.Successful.GetValueOrDefault() ? "<strong style='color: green'>Successful</strong>" : "<strong style='color: red'>Unsuccessful</strong>";
                    }

                    builder.Append("<p>");
                    builder.Append($"Task {component.TypeLabel}{component.Label}: {result}");
                    builder.Append("</p>");

                    if (!wasAttempted)
                    {
                        continue;
                    }


                    builder.Append("<table class=\"table\">");
                    builder.Append("<thead>");
                    builder.Append("<tr>");
                    builder.Append("<th class=\"table-cell\">Criterion</th>");
                    builder.Append("<th class=\"table-cell\">Band Level</th>");
                    builder.Append("<th class=\"table-cell\">Feedback comments</th>");
                    builder.Append("</tr>");
                    builder.Append("</thead>");
                    builder.Append("<tbody>");

                    var assessments = component.Competencies.SelectMany(c => c.Assessments).ToList();
                    for (int i = 0; i < assessments.Count; i++)
                    {
                        var rowClass = (i % 2) == 0 ? "table-row-odd" : String.Empty;
                        var assessment = assessments[i];
                        var bands = assessment.Bands.ToList();
                        var bandId = assessment.SelectedBand.GetValueOrDefault();
                        var band = bands.First(x=> x.Id == bandId);

                        builder.Append($"<tr class=\"{rowClass}\">");
                        builder.Append($"<td class=\"table-cell\">{assessment.Name}</td>");
                        builder.Append($"<td class=\"table-cell\"><strong>{band?.Label}</strong><br>{band?.Description}</td>");
                        builder.Append($"<td class=\"table-cell\">{assessment.Comment}</td>");
                        builder.Append("</tr>");
                    }

                    builder.Append("</tbody>");
                    builder.Append("</table>");
                }
            }

            return builder.ToString();
        }

        protected virtual void ValidateStandardSupplementaryComponentResults()
        {

            if (TestSpecification.MarkingSchemaTypeId != (int)MarkingSchemaTypeName.Standard)
            {
                return;
            }

            if (!StandardMarks.Components.Any(IsTaskSelected))
            {
                throw new UserFriendlySamException(Naati.Resources.Test.NoResultsSelected);
            }

            if (StandardMarks.Components.All(IsTaskSelected))
            {
                throw new UserFriendlySamException(Naati.Resources.Test.AllResultsSelected);
            }
        }

        protected virtual void ValidateRubricSupplementaryComponentResults()
        {
            if (TestSpecification.MarkingSchemaTypeId != (int)MarkingSchemaTypeName.Rubric)
            {
                return;
            }

            if (!RubricMarks.TestComponents.Any(IsTaskSelected))
            {
                throw new UserFriendlySamException(Naati.Resources.Test.NoResultsSelected);
            }

            if (RubricMarks.TestComponents.All(IsTaskSelected))
            {
                throw new UserFriendlySamException(Naati.Resources.Test.AllResultsSelected);
            }
        }

        protected virtual void ValidateAllowIssue()
        {
            if (TestSessionModel.AllowIssue.HasValue && !TestSessionModel.AllowIssue.Value)
            {
                throw new UserFriendlySamException(Naati.Resources.Application.IssuingNotAllowed);
            }
        }

        protected virtual void ValidateStandardMarks()
        {
            if (TestSpecification.MarkingSchemaTypeId != (int)MarkingSchemaTypeName.Standard)
            {
                return;
            }

            var passed = StandardMarks.Components.All(x => x.Mark >= x.PassMark) && StandardMarks.Components.Sum(x => x.Mark.GetValueOrDefault()) >= StandardMarks.OverAllPassMark.OverAllPassMark; 
            if (RequiredTestResultStatus == TestResultStatusTypeName.Failed && passed)
            {
                throw new UserFriendlySamException(Naati.Resources.Application.InvalidFailMarks);
            }


            if (RequiredTestResultStatus == TestResultStatusTypeName.Passed && !passed)
            {
                throw new UserFriendlySamException(Naati.Resources.Application.InvalidPassMarks);
            }

        }

        protected virtual bool IsTaskSelected(ITestComponentModel component)
        {
            return component.MarkingResultTypeId == (int)MarkingResultTypeName.Original;
        }


        protected virtual void ValidateTestResultStatus()
        {
            if (!TestSessionModel.TestResultChecked.GetValueOrDefault() || TestSessionModel.TestResultStatusId != (int)RequiredTestResultStatus)
            {
                throw new UserFriendlySamException(string.Format(Naati.Resources.Test.InvalidTestResultStatus, RequiredTestResultStatus.ToString().ToSentence()));
            }
        }

        protected virtual void ValidateExaminers()
        {
            if (TestSessionModel.TestStatusTypeId == (int)TestStatusTypeName.UnderPaidReview && !TestSessionModel.HasPaidReviewExaminers)
            {
                throw new UserFriendlySamException(string.Format(Naati.Resources.Test.RequiredExaminer));
            }

            if (!TestSessionModel.HasExaminers)
            {
                throw new UserFriendlySamException(string.Format(Naati.Resources.Test.RequiredExaminer));
            }
        }

        protected virtual void UpdateApplicationStatus()
        {
            if (ApplicationModel.ApplicationStatus.Id == (int)CredentialApplicationStatusTypeName.Completed)
            {
                ApplicationModel.ApplicationInfo.ApplicationStatusTypeId = (int)CredentialApplicationStatusTypeName.InProgress;
                ApplicationModel.ApplicationInfo.StatusChangeUserId = CurrentUser.Id;
                ApplicationModel.ApplicationInfo.StatusChangeDate = DateTime.Now;
            }
        }

        protected virtual void AllocateTestSession()
        {

            var testSessionModel = TestSessionModel;
            var testSpecificationId = testSessionModel.TestSpecificationId;
            if (testSpecificationId == 0)
            {
                throw new UserFriendlySamException($"There is no Test Specification configured for Credential type {CredentialRequestModel.CredentialType.ExternalName}.");
            }
            var credentialTestSession = new CredentialRequestTestSessionModel
            {
                Name = testSessionModel.Name,
                TestDate = testSessionModel.TestDate,
                TestSessionId = testSessionModel.TestSessionId,
                Supplementary = CredentialRequestModel.Supplementary,
                TestSpecificationId = testSpecificationId,
                MarkingSchemaTypeId = testSessionModel.MarkingSchemaTypeId,
                AllocatedDate = DateTime.Now
            };
            CredentialRequestModel.TestSessions.Add(credentialTestSession);
        }

        protected virtual TestSpecificationInfoModel GetTestSpecification()
        {
            var orderedTestSessions = CredentialRequestModel.TestSessions.OrderByDescending(x => x.CredentialTestSessionId);
            var testSession = orderedTestSessions.FirstOrDefault(x => !x.Rejected && x.Supplementary) ?? orderedTestSessions.FirstOrDefault(x => !x.Rejected);
            if (testSession == null)
            {
                throw new Exception($"No valid test sitting found for credential requestid :{CredentialRequestModel.Id}");
            }
            return new TestSpecificationInfoModel
            {
                TestSpecificationId = testSession.TestSpecificationId,
                MarkingSchemaTypeId = testSession.MarkingSchemaTypeId,
                AutomaticIssuing = testSession.AutomaticIssuing,
                MaxScoreDifference = testSession.MaxScoreDifference
            };
        }

        protected virtual void UnSetStandardSupplementaryComponentResults()
        {
            if (TestSpecification.MarkingSchemaTypeId != (int)MarkingSchemaTypeName.Standard)
            {
                return;
            }
            foreach (var component in StandardMarks.Components)
            {
                if (component.MarkingResultTypeId == (int)MarkingResultTypeName.EligableForSupplementary)
                {
                    component.MarkingResultTypeId = (int)MarkingResultTypeName.Original;
                }

                ApplicationModel.StandardTestComponentModelsToUpdate.Add(component);
            }
        }
        protected virtual void UnSetRubricSupplementaryComponentResults()
        {
            if (TestSpecification.MarkingSchemaTypeId != (int)MarkingSchemaTypeName.Rubric)
            {
                return;
            }
            foreach (var component in RubricMarks.TestComponents)
            {
                if (component.MarkingResultTypeId == (int)MarkingResultTypeName.EligableForSupplementary)
                {
                    component.MarkingResultTypeId = (int)MarkingResultTypeName.Original;
                }

                ApplicationModel.RubricTestComponentModelsToUpdate.Add(component);
            }
        }

        protected virtual bool GetAllowSupplementaryFlag()
        {
            var testService = ServiceLocator.Resolve<ITestService>();
            var allowSupplementary = testService.CheckIfAllowSupplementaryTest(CredentialRequestModel.Id).Data;
            return allowSupplementary;
        }
        protected virtual bool GetAllowConcededPassFlag()
        {
            var testService = ServiceLocator.Resolve<ITestService>();
            var allowSupplementary = testService.CheckIfAllowConcededPass(CredentialRequestModel.Id).Data;
            return allowSupplementary;
        }

        protected virtual bool GetAllowPaidReviewFlag()
        {
            var testService = ServiceLocator.Resolve<ITestService>();
            var allowSupplementary = testService.CheckIfAllowPaidTestReview(CredentialRequestModel.Id).Data;
            return allowSupplementary;
        }

        protected override void LogAction()
        {
            LoggingHelper.LogInfo("Performing workflow action {Action} for CR{CredentialRequestId} of APP{ApplicationId}",
                (SystemActionTypeName)WizardModel.ActionType, WizardModel.CredentialRequestId, WizardModel.ApplicationId);
        }

        protected override ApplicationInvoiceCreateRequestModel GetApplicationInvoiceCreateRequest(ProductSpecificationModel feeProductSpec)
        {
            var invoiceModel = base.GetApplicationInvoiceCreateRequest(feeProductSpec);
            invoiceModel.CredentialRequestId = CredentialRequestModel.Id;
            return invoiceModel;
        }

        protected override ApplicationRefundCreateRequestModel GetApplicationRefundCreateRequest(RefundModel refundRequest)
        {
            var invoiceModel = base.GetApplicationRefundCreateRequest(refundRequest);
            //invoiceModel.CredentialRequestId = CredentialRequestModel.Id;
            return invoiceModel;
        }

        protected override string GetInvoiceItemDescription(ProductSpecificationModel feeProductSpec, bool isSponsored)
        {
            var description = base.GetInvoiceItemDescription(feeProductSpec, isSponsored);

            return $"{description} \n {CredentialRequestModel.Skill.DisplayName}";
        }

        protected override string GetRefundItemDescription(ProductSpecificationModel feeProductSpec, bool isSponsored)
        {
            var description = base.GetRefundItemDescription(feeProductSpec, isSponsored);

            return $"{description} \n {CredentialRequestModel.Skill.DisplayName}";
        }

        protected virtual void ValidateTestSessionRejectionDate()
        {
            if (WizardModel.Source == SystemActionSource.MyNaati)
            {
                var days =  CredentialRequestModel.CredentialType.TestSessionBookingRejectHours / 24;

                var sessionModel = TestSessionModel;
                if (sessionModel.TestDate.AddDays(-days).Date < DateTime.Now.Date)
                {
                    throw  new Exception("Person not allowed to rejected this session.");
                }
            }
        }

        protected virtual void ValidateTestSessionAvailabilityDate()
        {
            if (WizardModel.Source == SystemActionSource.MyNaati)
            {
                var days = CredentialRequestModel.CredentialType.TestSessionBookingAvailabilityWeeks * 7;

                var sessionModel = TestSessionModel;
                if (sessionModel.TestDate.AddDays(-days).Date > DateTime.Now.Date)
                {
                    throw new Exception("Test session is not available");
                }
            }
        }
       
        protected virtual void ValidateTestSessionClosedDate()
        {
            if (WizardModel.Source == SystemActionSource.MyNaati)
            {
                var days = CredentialRequestModel.CredentialType.TestSessionBookingClosedWeeks * 7;

                var sessionModel = TestSessionModel;
                if (sessionModel.TestDate.AddDays(-days).Date < DateTime.Now.Date)
                {
                    throw new Exception("Test session is already closed");
                }
            }
        }

        protected virtual void ValidateRefundRequest(
            double? refundPercentage, int? refundMethodTypeId, int? credentialWorkflowFeeId)
        {
            if (refundPercentage == null || refundPercentage <= 0)
            {
                throw new Exception("Invalid refund percentage");
            }
            if (refundMethodTypeId == null ||
                (refundMethodTypeId != (int)RefundMethodTypeName.CreditCard &&
                 refundMethodTypeId != (int)RefundMethodTypeName.DirectDeposit &&
                 refundMethodTypeId != (int)RefundMethodTypeName.PayPal))
            {
                throw new Exception("Invalid refund method type");
            }
            if (credentialWorkflowFeeId == null)
            {
                throw new Exception("Invalid credential workflow fee id");
            }
        }

        protected virtual void ValidateRefundApprovalRequest(
            double? refundPercentage, 
            int? refundMethodTypeId, 
            int? credentialWorkflowFeeId, 
            decimal? refundAmount)
        {
            ValidateRefundRequest(refundPercentage, refundMethodTypeId, credentialWorkflowFeeId);
            if (refundAmount.GetValueOrDefault() <= 0)
            {
                throw new Exception("Invalid refund amount");
            }

            if (refundAmount.GetValueOrDefault() > RefundRequest?.InitialPaidAmount.GetValueOrDefault())
            {
                throw new Exception("Refund amount can not be higher that initial paid amount");
            }
        }


        protected virtual void ProcessRefundRequest()
        {
            var refundRequest = RefundRequest;
            refundRequest.ObjectStatusId = (int)ObjectStatusTypeName.Updated;
        }
    }
}