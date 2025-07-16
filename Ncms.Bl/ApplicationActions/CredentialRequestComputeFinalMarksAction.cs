using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Naati.Resources;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.Examiner;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestComputeFinalMarksAction : CredentialRequestStateAction
    {
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.TestResult;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Update;

        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.TestSat };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.TestSat;

        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateUserPermissions,
            ValidateMinimumCredentialRequests,
            ValidateMandatoryFields,
            ValidateMandatoryPersonFields,
            ValidateMandatoryDocuments,
            ValidateStandardMarks,
            ValidateRequiresThirdmarking
        };

        private void ValidateRequiresThirdmarking()
        {
            var testOverallMark = GetTestOverallMark();

            if (testOverallMark.passed)
            {
                var examiners = ExaminerService.GetTestExaminers(new GetJobExaminersRequestModel
                {
                    TestAttendanceIds = new[] { TestSessionModel.CredentialTestSessionId },
                    IncludeExaminerMarkings = true
                }).Data.ToList();

                var examinerCount = examiners.Count;
                LoggingHelper.LogInfo($"ValidateRequiresThirdExaminer {examinerCount} examiners found for Test Result {TestSessionModel.TestResultId.GetValueOrDefault()}");

                if (examinerCount == 1) return;

                var marks = new List<GetMarksResponseModel>();
                foreach (var examiner in examiners)
                {
                    marks.Add(ExaminerService.GetMarks(new GetExaminerMarksRequestModel()
                    {
                        JobExaminerId = examiner.JobExaminerId,
                        TestResultId = TestSessionModel.TestResultId.GetValueOrDefault()
                    }));
                }
                var componentCount = marks.First().Components.Count();
                var requiresThirdExaminer = false;
                //if total marks for a component differ between examiners then need a third person
                for (var i = 0; i < componentCount; i++)
                {
                    if (!requiresThirdExaminer)
                    {
                        if (DidPass(marks.ElementAt(0).Components.ElementAt(i)) != DidPass(marks.ElementAt(1).Components.ElementAt(i)))
                        {
                            requiresThirdExaminer = true;
                        }
                    }
                }

                if (requiresThirdExaminer)
                {
                    ValidationErrors.Add(new ValidationResultModel { Field = "Marks", Message = Test.RequiredThirdMarking });
                }
            }
        }

        private bool DidPass(Contracts.Models.TestComponentModel mark)
        {
            return (mark.PassMark <= mark.Mark);
        }

        protected override IList<Action> SystemActions => new List<Action>
        {
            CalculateMarks,
            CreateNote,
            SetExitState
        };


        protected override void ValidateStandardMarks()
        {
            if (!TestSpecification.AutomaticIssuing || !TestSpecification.MaxScoreDifference.HasValue)
            {
                throw new Exception("Action disabled for current Test Specification");
            }

            if (TestSpecification.MarkingSchemaTypeId != (int)MarkingSchemaTypeName.Standard)
            {
                throw new Exception("Action valid for standard marking schema only");
            }

            var test = TestService.GetTestSummary(TestSessionModel.CredentialTestSessionId).Data;
            if (test.LastReviewTestResultId.HasValue)
            {
                ValidationErrors.Add(new ValidationResultModel { Field = "Marks", Message = Naati.Resources.Application.ActionInvalidForPaidReview });
                return;
            }

            var testResult = TestResultService.GetTestResult(TestSessionModel.TestResultId.GetValueOrDefault()).Data;

            if (!testResult.AllowCalculate)
            {
                ValidationErrors.Add(new ValidationResultModel { Field = "Marks", Message = Naati.Resources.Application.TestDoesntAllowAutoCalculate });
                return;
            }

            if (testResult.ResultChecked)
            {
                ValidationErrors.Add(new ValidationResultModel { Field = "Marks", Message = Naati.Resources.Application.ResultsAlreadyChecked });
                return;
            }

            if (!testResult.AllowIssue)
            {
                ValidationErrors.Add(new ValidationResultModel { Field = "Marks", Message = Naati.Resources.Application.IssueResultsNotAllowed });
                return;
            }

            var examinersQuantity = 2;
            var response = ExaminerService.GetTestExaminers(new GetJobExaminersRequestModel
            {
                TestAttendanceIds = new[] { TestSessionModel.CredentialTestSessionId },
                IncludeExaminerMarkings = true
            });
           
            var examiners = response.Data.ToList();
            
            if (examiners.Count >= 2)
            {
                if (examiners.Count != examinersQuantity)
                {
                    ValidationErrors.Add(new ValidationResultModel { Field = "Marks", Message = string.Format(Naati.Resources.Application.InvalidExaminerQuantity, examinersQuantity) });
                    return;
                }
            }
            

            if (examiners.Any(x => !x.ExaminerReceivedDate.HasValue))
            {
                throw new UserFriendlySamException(Naati.Resources.Application.MarksNotSubmitted);
            }

            var examinerOverallMarks = examiners.SelectMany(x => x.ExaminerMarkings.Select(em => em.TestComponentResults.Sum(tcr => tcr.Mark))).ToList();

            var maxMark = examinerOverallMarks.Max();
            var minMark = examinerOverallMarks.Min();
            var maxDifference = (TestSpecification.MaxScoreDifference ?? -1);
            if (maxMark - minMark > maxDifference)
            {
                ValidationErrors.Add(new ValidationResultModel { Field = "Marks", Message = string.Format(Naati.Resources.Application.ExaminerMarkDifferenceToHigh, maxDifference) });
                return;
            }

            ExaminerService.UpdateCountMarks(new UpdateCountMarksRequestModel { IncludePreviousMarks = false, JobExaminersId = examiners.Select(x => x.JobExaminerId).ToArray(), TestResultId = TestSessionModel.TestResultId.GetValueOrDefault() });

            var testOverallMark = GetTestOverallMark();
            foreach (var examinerOverallMark in examinerOverallMarks)
            {
                var passed = examinerOverallMark >= testOverallMark.overallPassMark;
                if (testOverallMark.passed != passed)
                {
                    ValidationErrors.Add(new ValidationResultModel { Field = "Marks", Message = Naati.Resources.Application.ExaminerResultIsDifferentToFinalResult });
                    return;
                }
            }
        }

      
        protected override string GetNote()
        {
            return string.Format(Naati.Resources.Application.TestResultsComputed, TestSessionModel.CredentialTestSessionId);
        }

        private (double overallPassMark, bool passed) GetTestOverallMark()
        {
            var marks = TestResultService.GetMarks(new GetMarksRequestModel { TestResultId = TestSessionModel.TestResultId.GetValueOrDefault() });
            var passed = marks.Components.All(x => x.Mark >= x.PassMark) && marks.Components.Sum(x => x.Mark.GetValueOrDefault()) >= marks.OverAllPassMark.OverAllPassMark;
            return (marks.OverAllPassMark.OverAllPassMark,  passed);
        }

        private void CalculateMarks()
        {
            var response = ExaminerService.GetTestExaminers(new GetJobExaminersRequestModel
            {
                TestAttendanceIds = new[] { TestSessionModel.CredentialTestSessionId },
                IncludeExaminerMarkings = true
            });
            var examiners = response.Data.ToList();
          

            var passed = GetTestOverallMark().passed;

            var joinedReasons = new HashSet<string>();

            var poorPerformanceReasons = examiners.SelectMany(x => x.ExaminerMarkings.Select(em=> em.ReasonsForPoorPerformance ?? String.Empty ));
            foreach (var poorPerformanceReason in poorPerformanceReasons)
            {
                var reasons = poorPerformanceReason.Split(',');
                reasons.ForEach(r=> joinedReasons.Add(r.ToUpper()));
            }

            var testResult = TestResultService.GetTestResult(TestSessionModel.TestResultId.GetValueOrDefault()).Data;
            var resultType = passed ? TestResultStatusTypeName.Passed : TestResultStatusTypeName.Failed;
            TestResultService.UpdateTestResult(new TestResultModel
            {
                AllowCalculate = true,
                ResultTypeId = (int)resultType,
                CurrentJobId = testResult.CurrentJobId,
                ProcessedDate = DateTime.Now,
                TestResultId = TestSessionModel.TestResultId.GetValueOrDefault(),
                ResultChecked = true,
                CommentsGeneral = string.Join(",", joinedReasons.OrderBy(x=> x)),
                DueDate = testResult.DueDate,
                AllowIssue = testResult.AllowIssue
            });
        }

        protected override GetMarksResponseModel GetStandardMarks()
        {
            throw new Exception($"Method {nameof(GetStandardMarks)} not supported for action {nameof(CredentialRequestComputeFinalMarksAction)}");
        }
    }
}
