using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.System;
using Ncms.Contracts.Models.Test;
using IApplicationService = Ncms.Contracts.IApplicationService;
using F1Solutions.Naati.Common.Dal.Domain;
using System.Text;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;

namespace Ncms.Bl
{
    public class ApplicationWizardLogicService : IApplicationWizardLogicService
    {
        private readonly IEnumerable<SystemActionTypeName> RequiresPublicNote = new[] {
                SystemActionTypeName.RejectApplication,
                SystemActionTypeName.RejectApplicationAfterInvoice,
                SystemActionTypeName.FailAssessment,
                SystemActionTypeName.FailPendingAssessment,
                SystemActionTypeName.PendAssessment,
                SystemActionTypeName.CreatePaidReview,
                SystemActionTypeName.FailReview,
                SystemActionTypeName.CancelRequest,
                SystemActionTypeName.CancelTestInvitation
        };
        private readonly IEnumerable<SystemActionTypeName> RequiresPrivateNote = new[] {
                SystemActionTypeName.ReactivateApplication,
                SystemActionTypeName.Reassess,
                SystemActionTypeName.CloseApplication
        };
        private readonly IApplicationQueryService _applicationQueryService;
        private readonly ITokenReplacementService _tokenReplacementService;
        private readonly ITestQueryService _testServiceQueryService;
        private readonly ITestMaterialQueryService _testMaterialQueryService;

        private readonly IUserQueryService _userQueryService;
        private readonly ICredentialQueryService _credentialQueryService;

        private readonly ISystemQueryService _systemQueryService;

        private readonly ICredentialPrerequisiteService _credentialPrerequisiteService;

        private bool IsRolePlayerEnabled;


        public ApplicationWizardLogicService(IApplicationQueryService applicationQueryService, ITokenReplacementService tokenReplacementService, 
            ITestQueryService testServiceQueryService, ITestMaterialQueryService testMaterialQueryService, IUserQueryService userQueryService, 
            ICredentialQueryService credentialQueryService,ISystemQueryService systemQueryService, ICredentialPrerequisiteService credentialPrerequisiteService)
        {
            _applicationQueryService = applicationQueryService;
            _tokenReplacementService = tokenReplacementService;
            _testServiceQueryService = testServiceQueryService;
            _testMaterialQueryService = testMaterialQueryService;
            _userQueryService = userQueryService;
            _credentialQueryService = credentialQueryService;
            _systemQueryService = systemQueryService;
            _credentialPrerequisiteService = credentialPrerequisiteService;

            IsRolePlayerEnabled = _systemQueryService.GetSystemValue(new GetSystemValueRequest() { ValueKey = "RolePlayerAvailable" }).Value == "true" ? true : false;

        }

        public GenericResponse<NoteFieldRules> GetNoteFieldRulesForAllocateTestSessionAction(int applicationTypeId)
        {
            return GetNoteFieldRulesByApplicationTypeId((int)SystemActionTypeName.AllocateTestSession,
                applicationTypeId);
        }

        public GenericResponse<IEnumerable<SystemActionNameModel>> GetValidActions(int applicationStatusTypeId)
        {
            var actions = new SystemActionTypeName[] { };
            var actionDisplayNames = new Dictionary<SystemActionTypeName, string>
            {
                [SystemActionTypeName.SubmitApplication] = Naati.Resources.Application.SubmitActionLabel,
                [SystemActionTypeName.StartChecking] = Naati.Resources.Application.StartCheckingActionLabel,
                [SystemActionTypeName.CancelChecking] = Naati.Resources.Application.CancelCheckingActionLabel,
                [SystemActionTypeName.FinishChecking] = Naati.Resources.Application.FinishCheckingActionLabel,
                [SystemActionTypeName.RejectApplication] = Naati.Resources.Application.RejectActionLabel,
                [SystemActionTypeName.RejectApplicationAfterInvoice] = Naati.Resources.Application.RejectActionLabel,
                [SystemActionTypeName.DeleteApplication] = Naati.Resources.Application.DeleteApplicationActionLabel,
                [SystemActionTypeName.AssessmentInvoicePaid] = Naati.Resources.Application.ApplicationInvoicePaidActionLabel,
                [SystemActionTypeName.ApplicationInvoicePaid] = Naati.Resources.Application.ApplicationInvoicePaidActionLabel,
                [SystemActionTypeName.ApplicationSubmissionProcessed] = Naati.Resources.Application.ApplicationSubmissionProcessedActionLabel,
                [SystemActionTypeName.ReactivateApplication] = Naati.Resources.Application.ReactivateApplicationActionLabel,
                [SystemActionTypeName.CloseApplication] = Naati.Resources.Application.CloseApplicationActionLabel,
                [SystemActionTypeName.SendRecertificationReminder] = Naati.Resources.Application.SendRecertificationReminderLabel,
                [SystemActionTypeName.SendEmail] = Naati.Resources.Application.SendEmailActionLabel,
            };

            var status = (CredentialApplicationStatusTypeName)applicationStatusTypeId;

            switch (status)
            {
                case CredentialApplicationStatusTypeName.None:
                    actions = new[] { SystemActionTypeName.CreateApplication };
                    break;
                case CredentialApplicationStatusTypeName.Draft:
                    actions = new[] { SystemActionTypeName.SubmitApplication, SystemActionTypeName.DeleteApplication };
                    break;
                case CredentialApplicationStatusTypeName.Entered:
                    actions = new[] { SystemActionTypeName.StartChecking };
                    break;
                case CredentialApplicationStatusTypeName.ProcessingSubmission:
                    actions = new[] { SystemActionTypeName.ApplicationSubmissionProcessed };
                    break;
                case CredentialApplicationStatusTypeName.BeingChecked:
                    actions = new[] { SystemActionTypeName.CancelChecking, SystemActionTypeName.FinishChecking, SystemActionTypeName.RejectApplication };
                    break;
                case CredentialApplicationStatusTypeName.AwaitingAssessmentPayment:
                    actions = new[] { SystemActionTypeName.RejectApplicationAfterInvoice };
                    break;
                case CredentialApplicationStatusTypeName.AwaitingApplicationPayment:
                    actions = new[] { SystemActionTypeName.RejectApplicationAfterInvoice };
                    break;
                case CredentialApplicationStatusTypeName.Completed:
                    actions = new[] { SystemActionTypeName.ReactivateApplication, SystemActionTypeName.SendRecertificationReminder };
                    break;
                case CredentialApplicationStatusTypeName.InProgress:
                    actions = new[] { SystemActionTypeName.CloseApplication };
                    break;
                case CredentialApplicationStatusTypeName.ProcessingApplicationInvoice:
                case CredentialApplicationStatusTypeName.Rejected:
                case CredentialApplicationStatusTypeName.Deleted:
                    break;
                default:
                    throw new Exception($"Unexpected {nameof(CredentialApplicationStatusTypeName)}: {status}.");
            }

            return new GenericResponse<IEnumerable<SystemActionNameModel>>(
                actions
                // actions for all states
                .Union(new[] { SystemActionTypeName.SendEmail }) 
                .Select(x => new SystemActionNameModel
                {
                    Id = (int)x,
                    Name = actionDisplayNames.ContainsKey(x) ? actionDisplayNames[x] : x.ToString()
                }));
        }

        public GenericResponse<string> GetConfirmationMessage(ApplicationWizardSteps step, int applicationId, int credentialRequestId)
        {
            string message;
            switch (step)
            {
                case ApplicationWizardSteps.DeleteConfirmation:
                    message = string.Format(Naati.Resources.Shared.DeleteConfirmation, $"APP {applicationId}");
                    break;
                case ApplicationWizardSteps.ExistingConcededCredential:
                    var downgradeCredentialRequest = _applicationQueryService.GetDowngradedCredentialRequest(credentialRequestId);
                    message = string.Format(Naati.Resources.Shared.ConcededPassAlreadyExists, downgradeCredentialRequest.CredentialTypeInternalName, downgradeCredentialRequest.Skill);
                    break;
                case ApplicationWizardSteps.NotFoundConcededCredential:
                    var request = _applicationQueryService.GetCredentialRequest(credentialRequestId);
                    message = string.Format(Naati.Resources.Shared.DowngradePathNotFound, request.CredentialRequest.CredentialType.InternalName, request.CredentialRequest.Skill.DisplayName);
                    break;
                case ApplicationWizardSteps.IssueConcededPass:
                    var downgradeCredential = _applicationQueryService.GetDowngradedCredentialRequest(credentialRequestId);
                    message = string.Format(Naati.Resources.Shared.IssueConcededPassMessage, downgradeCredential.CredentialTypeInternalName, downgradeCredential.Skill);
                    break;

                default:
                    throw new Exception($"No message has been configured for step {step}");
            }

            return new GenericResponse<string>(message);
        }

        public GenericResponse<string> GetStepMessage(SystemActionTypeName action)
        {
            string message;
            switch (action)
            {
                case SystemActionTypeName.CreateSupplementaryTest:
                    message = string.Format(Naati.Resources.Application.CreateSupplementaryTestWizardMessage);
                    break;
                case SystemActionTypeName.CreatePaidTestReview:
                    message = string.Format(Naati.Resources.Application.CreatePaidTestReviewWizardMessage);
                    break;
                default:
                    throw new Exception($"No message has been configured for action {action}");
            }
            return new GenericResponse<string>(message);
        }

        public GenericResponse<bool> CanShowSessionStep(SessionStepValidationModel request)
        {
            if (request.StepId != (int)TestSessionWizardSteps.RehearsalDetails)
            {
                return new GenericResponse<bool>(true);
            }

            var rolePlayersRequired = _credentialQueryService.GetCredentialType(request.CredentialTypeId).Data.RolePlayersRequired;

            return new GenericResponse<bool>(rolePlayersRequired);
        }

        private bool CanOverrideEmail()
        {
            return ServiceLocator.Resolve<IUserService>().HasPermission(SecurityNounName.Email, SecurityVerbName.Override);
        }
        public GenericResponse<IEnumerable<CredentialRequestWizardSteps>> GetCredentialRequestWizardSteps(int actionId, int applicationTypeId, int credentialRequestStatusTypeId)
        {
            var action = (SystemActionTypeName)actionId;
            var steps = new List<CredentialRequestWizardSteps>();

            switch (action)
            {
                case SystemActionTypeName.AllocateTestSession:
                    if (credentialRequestStatusTypeId == (int)CredentialRequestStatusTypeName.EligibleForTesting)
                    {
                        steps.AddRange(new[] { CredentialRequestWizardSteps.TestSession, CredentialRequestWizardSteps.ExistingApplicants, CredentialRequestWizardSteps.NewApplicants, CredentialRequestWizardSteps.Notes });
                    }
                    else if (credentialRequestStatusTypeId == (int)CredentialRequestStatusTypeName.TestAccepted)
                    {
                        steps.AddRange(new[] { CredentialRequestWizardSteps.TestSession, CredentialRequestWizardSteps.ExistingApplicants, CredentialRequestWizardSteps.NewApplicants, CredentialRequestWizardSteps.Notes });
                    }

                    break;

                default:
                    throw new Exception($"Unexpected {nameof(SystemActionTypeName)}: {action}.");
            }

            var emailTemplateReponse = _applicationQueryService.GetEmailTemplate(new GetEmailTemplateRequest
            {
                ApplicationType = (CredentialApplicationTypeName)applicationTypeId,
                Action = action

            });

            if (emailTemplateReponse.Data != null && emailTemplateReponse.Data.Any(x => x.Active))
            {
                if (CanOverrideEmail())
                {
                    steps.Add(CredentialRequestWizardSteps.CheckOption);
                }
                steps.Add(CredentialRequestWizardSteps.ViewEmailAttachments);
            }

            steps.Add(CredentialRequestWizardSteps.ViewMessage);

            return new GenericResponse<IEnumerable<CredentialRequestWizardSteps>>(steps);
        }

        public GenericResponse<IEnumerable<TestSessionWizardSteps>> GetTestSessionWizardSteps(int? testSessionId)
        {
            var firstSteps = new[] { TestSessionWizardSteps.Details, TestSessionWizardSteps.RehearsalDetails, TestSessionWizardSteps.Skills };
            if (!IsRolePlayerEnabled)
            {
                firstSteps = new[] { TestSessionWizardSteps.Details, TestSessionWizardSteps.Skills };
            }
            var steps = new List<TestSessionWizardSteps>();
            steps.AddRange(firstSteps);
            if (!testSessionId.HasValue)
            {
                steps.AddRange(new[]
                {
                    TestSessionWizardSteps.MatchingApplicants
                });
            }

            steps.AddRange(new[]
            {
                TestSessionWizardSteps.Notes,
                TestSessionWizardSteps.CheckOption,
                TestSessionWizardSteps.PreviewEmail,
            });

            return new GenericResponse<IEnumerable<TestSessionWizardSteps>>(steps);
        }
        public GenericResponse<IEnumerable<AllocateRolePlayersWizardSteps>> GetAllocateRolePlayersSteps(int? testSessionId)
        {
            var steps = new List<AllocateRolePlayersWizardSteps>();
            steps.AddRange(new[]
            {
                AllocateRolePlayersWizardSteps.TestSpecification,
                AllocateRolePlayersWizardSteps.Skill,
                AllocateRolePlayersWizardSteps.AllocateRolePlayers,
                AllocateRolePlayersWizardSteps.SendEmail,//TODO: Just for administrators
				AllocateRolePlayersWizardSteps.EmailPreview
            });


            return new GenericResponse<IEnumerable<AllocateRolePlayersWizardSteps>>(steps);
        }

        public IList<SystemActionNameModel> GetValidCredentialRequestActions(CredentialRequestStatusTypeName credentialRequestStatus, int credentialRequestId)
        {
            var actions = new SystemActionTypeName[] { };
            var actionDisplayNames = new Dictionary<SystemActionTypeName, string> // todo: awkward. if/when actions are in the DB, get display names from there
            {
                [SystemActionTypeName.StartAssessing] = Naati.Resources.Application.StartAssessingActionLabel,
                [SystemActionTypeName.CancelAssessment] = Naati.Resources.Application.CancelAssessmentActionLabel,
                [SystemActionTypeName.FailAssessment] = Naati.Resources.Application.FailAssessmentActionLabel,
                [SystemActionTypeName.FailPendingAssessment] = Naati.Resources.Application.FailAssessmentActionLabel,
                [SystemActionTypeName.PassAssessment] = Naati.Resources.Application.AssessmentSuccessfulActionLabel,
                [SystemActionTypeName.PendAssessment] = Naati.Resources.Application.AssessmentPendingActionLabel,
                [SystemActionTypeName.CreatePaidReview] = Naati.Resources.Application.CreatePaidReviewActionLabel,
                [SystemActionTypeName.CreatePaidTestReview] = Naati.Resources.Application.CreatePaidTestReviewActionLabel,
                [SystemActionTypeName.FailReview] = Naati.Resources.Application.FailReviewActionLabel,
                [SystemActionTypeName.PassReview] = Naati.Resources.Application.PassReviewActionLabel,
                [SystemActionTypeName.IssueCredential] = Naati.Resources.Application.IssueCredentialActionLabel,
                [SystemActionTypeName.CancelRequest] = Naati.Resources.Application.CancelRequestActionLabel,
                [SystemActionTypeName.DeleteRequest] = Naati.Resources.Application.DeleteRequestActionLabel,
                [SystemActionTypeName.ReissueCredential] = Naati.Resources.Application.ReissueCredentialActionLabel,
                [SystemActionTypeName.CancelTestInvitation] = Naati.Resources.Application.CancelTestInvitationLabel,
                [SystemActionTypeName.WithdrawRequest] = Naati.Resources.Application.WithdrawRequestActionLabel,
                [SystemActionTypeName.TestInvoicePaid] = Naati.Resources.Application.TestInvoicePaidActionLabel,
                [SystemActionTypeName.AllocateTestSession] = Naati.Resources.Application.AllocateTestSessionActionLabel,
                [SystemActionTypeName.RejectTestSession] = Naati.Resources.Application.RejectTestSessionActionLabel,
                [SystemActionTypeName.CheckIn] = Naati.Resources.Application.CheckInActionLabel,
                [SystemActionTypeName.MarkAsSat] = Naati.Resources.Application.MarkAsSatActionLabel,
                [SystemActionTypeName.UndoCheckIn] = Naati.Resources.Application.UndoCheckInActionLabel,
                [SystemActionTypeName.UndoMarkAsSat] = Naati.Resources.Application.UndoMarkAsSatActionLabel,
                [SystemActionTypeName.IssueFail] = Naati.Resources.Application.IssueFailActionLabel,
                [SystemActionTypeName.IssuePass] = Naati.Resources.Application.IssuePassActionLabel,
                [SystemActionTypeName.IssueFailAfterReview] = Naati.Resources.Application.IssuePaidTestReviewFailActionLabel,
                [SystemActionTypeName.IssuePassAfterReview] = Naati.Resources.Application.IssuePaidTestReviewPassActionLabel,
                [SystemActionTypeName.CertificationOnHold] = Naati.Resources.Application.CertificationOnHoldLabel,
                [SystemActionTypeName.CreatePreRequisiteApplications] = Naati.Resources.Application.CreatePreRequisiteApplicationsLabel,
                [SystemActionTypeName.CreateSupplementaryTest] = Naati.Resources.Application.CreateSupplementaryTestActionLabel,
                [SystemActionTypeName.SupplementaryTestInvoicePaid] = Naati.Resources.Application.SupplementaryTestInvoicePaidActionLabel,
                [SystemActionTypeName.WithdrawSupplementaryTest] = Naati.Resources.Application.WithdrawSupplementaryTestActionLabel,
                [SystemActionTypeName.PaidReviewInvoiceProcessed] = Naati.Resources.Application.PaidReviewInvoiceProcessedActionLabel,
                [SystemActionTypeName.WithdrawPaidReview] = Naati.Resources.Application.WithdrawPaidReviewLabel,
                [SystemActionTypeName.PaidReviewInvoicePaid] = Naati.Resources.Application.PaidReviewInvoicePaidLabel,
                [SystemActionTypeName.SupplementaryTestInvoiceProcessed] = Naati.Resources.Application.SupplementaryTestInvoiceProcessedLabel,
                [SystemActionTypeName.SendCandidateBrief] = Naati.Resources.Application.SendCandidateBriefLabel,
                [SystemActionTypeName.RevertResults] = Naati.Resources.Application.RevertResultsLabel,
                [SystemActionTypeName.InvalidateTest] = Naati.Resources.Application.InvalidateTestLabel,
                [SystemActionTypeName.MarkForAssessment] = Naati.Resources.Application.MarkAsReadyForAssessmentLabel,
                [SystemActionTypeName.TestInvoiceProcessed] = Naati.Resources.Application.TestInvoiceProcessedLabel,
                [SystemActionTypeName.SendTestSessionReminder] = Naati.Resources.Application.SendTestSessionReminder,
                [SystemActionTypeName.SendSessionAvailabilityNotice] = Naati.Resources.Application.SendSessionAvailabilityReminderLabel,
                [SystemActionTypeName.RequestRefund] = Naati.Resources.Application.RequestRefundLabel,
                [SystemActionTypeName.ApproveRefund] = Naati.Resources.Application.ApproveRefundLabel,
                [SystemActionTypeName.RejectRefund] = Naati.Resources.Application.RejectRefundLabel,
                [SystemActionTypeName.ProcessRefund] = Naati.Resources.Application.ProcessRefundLabel,
                [SystemActionTypeName.CreditNotePaid] = Naati.Resources.Application.CreditNotePaidLabel,
                [SystemActionTypeName.CreditNoteProcessed] = Naati.Resources.Application.CreditNoteProcessedLabel,
                [SystemActionTypeName.IssuePracticeTestResults] = Naati.Resources.Application.IssuePracticeTestResultsLabel,
                [SystemActionTypeName.SendEmail] = Naati.Resources.Application.SendEmailActionLabel,
            };

            var credentialRequest = _applicationQueryService.GetCredentialRequest(credentialRequestId).CredentialRequest;
            var credentialApplicationTypeId = _applicationQueryService.GetApplicationByCredentialRequestId(credentialRequestId).Result.ApplicationTypeId;
            var credentialPrerequisites = _credentialPrerequisiteService.GetCredentialPrerequisites().ToList();

            switch (credentialRequestStatus)
            {
                case CredentialRequestStatusTypeName.ReadyForAssessment:
                    actions = new[] { SystemActionTypeName.StartAssessing, SystemActionTypeName.DeleteRequest, SystemActionTypeName.WithdrawRequest, };
                    break;
                case CredentialRequestStatusTypeName.BeingAssessed:
                    if (credentialPrerequisites.Select(x => x.CredentialType.Id).Contains(credentialRequest.CredentialTypeId) &&
                        credentialPrerequisites.Select(x => x.CredentialApplicationType.Id).Contains(credentialApplicationTypeId))
                    {
                        actions = new[]
                              {
                                  SystemActionTypeName.CancelAssessment,
                                  SystemActionTypeName.DeleteRequest,
                                  SystemActionTypeName.FailAssessment,
                                  SystemActionTypeName.PassAssessment,
                                  SystemActionTypeName.PendAssessment,
                                  SystemActionTypeName.WithdrawRequest,
                                  SystemActionTypeName.CreatePreRequisiteApplications
                              };
                        break;
                    }
                    actions = new[]
                              {
                                  SystemActionTypeName.CancelAssessment,
                                  SystemActionTypeName.DeleteRequest,
                                  SystemActionTypeName.FailAssessment,
                                  SystemActionTypeName.PassAssessment,
                                  SystemActionTypeName.PendAssessment,
                                  SystemActionTypeName.WithdrawRequest,
                              };
                    break;
                case CredentialRequestStatusTypeName.Pending:
                    if(credentialPrerequisites.Select(x => x.CredentialType.Id).Contains(credentialRequest.CredentialTypeId) &&
                        credentialPrerequisites.Select(x => x.CredentialApplicationType.Id).Contains(credentialApplicationTypeId))
                    {
                        actions = new[] { SystemActionTypeName.FailPendingAssessment, SystemActionTypeName.PassAssessment, SystemActionTypeName.CreatePreRequisiteApplications };
                        break;
                    }
                    actions = new[] { SystemActionTypeName.FailPendingAssessment, SystemActionTypeName.PassAssessment };
                    break;
                case CredentialRequestStatusTypeName.AssessmentFailed:
                    actions = new[] { SystemActionTypeName.Reassess };
                    break;
                case CredentialRequestStatusTypeName.AssessmentPaidReview:
                    actions = new[] { SystemActionTypeName.FailReview, SystemActionTypeName.PassReview };
                    break;
                case CredentialRequestStatusTypeName.AssessmentComplete:
                    actions = new[] { SystemActionTypeName.IssueCredential };
                    break;
                case CredentialRequestStatusTypeName.Draft:
                    actions = new[] { SystemActionTypeName.DeleteRequest };
                    break;
                case CredentialRequestStatusTypeName.RequestEntered:
                    actions = new[] { SystemActionTypeName.CancelRequest };
                    break;
                case CredentialRequestStatusTypeName.CertificationIssued:
                    actions = new[] { SystemActionTypeName.ReissueCredential, SystemActionTypeName.RevertResults };
                    break;
                case CredentialRequestStatusTypeName.EligibleForTesting:
                    if (credentialPrerequisites.Select(x => x.CredentialType.Id).Contains(credentialRequest.CredentialTypeId) &&
                        credentialPrerequisites.Select(x => x.CredentialApplicationType.Id).Contains(credentialApplicationTypeId))
                    {
                        actions = new[] { SystemActionTypeName.WithdrawRequest, SystemActionTypeName.CreatePreRequisiteApplications };
                        break;
                    }
                    actions = new[] { SystemActionTypeName.WithdrawRequest/*, SystemActionTypeName.SendSessionAvailabilityNotice */};
                    break;
                case CredentialRequestStatusTypeName.AwaitingTestPayment:
                    actions = new[] { SystemActionTypeName.CancelTestInvitation, SystemActionTypeName.WithdrawRequest };
                    break;
                case CredentialRequestStatusTypeName.AwaitingSupplementaryTestPayment:
                    actions = new[] { SystemActionTypeName.WithdrawSupplementaryTest };
                    break;
                case CredentialRequestStatusTypeName.AwaitingPaidReviewPayment:
                    actions = new[] { SystemActionTypeName.WithdrawPaidReview };
                    break;
                case CredentialRequestStatusTypeName.TestAccepted:
                    actions = new[] { SystemActionTypeName.WithdrawRequest, SystemActionTypeName.RequestRefund };
                    if (credentialPrerequisites.Select(x => x.CredentialType.Id).Contains(credentialRequest.CredentialTypeId) &&
                        credentialPrerequisites.Select(x => x.CredentialApplicationType.Id).Contains(credentialApplicationTypeId))
                    {
                        actions = actions.Concat(new[] {SystemActionTypeName.CreatePreRequisiteApplications}).ToArray();
                    }
                    if (credentialRequest.Supplementary)
                    {
                        actions = actions.Concat(new[] { SystemActionTypeName.WithdrawSupplementaryTest }).ToArray();
                    }
                    break;
                case CredentialRequestStatusTypeName.RefundRequested:
                    actions = new[] { SystemActionTypeName.ApproveRefund, SystemActionTypeName.RejectRefund };
                    break;
                case CredentialRequestStatusTypeName.RefundRequestApproved:
                    actions = new[] { SystemActionTypeName.ProcessRefund };
                    break;
                case CredentialRequestStatusTypeName.AwaitingCreditNotePayment:
                    actions = new[] { SystemActionTypeName.CreditNotePaid };
                    break;
                case CredentialRequestStatusTypeName.ProcessingCreditNote:
                    actions = new[] { SystemActionTypeName.CreditNoteProcessed };
                    break;
                case CredentialRequestStatusTypeName.RefundFailed:
                    actions = new[] { SystemActionTypeName.RejectRefund };
                    break;
                case CredentialRequestStatusTypeName.TestSessionAccepted:
                    var testSittingId = _applicationQueryService.GetCredentialRequest(credentialRequestId)
                        .CredentialRequest.TestSessions.First(x => !x.Rejected && !x.Sat).CredentialTestSessionId;
                    var hasPendingBriefs = _testMaterialQueryService.GetPendingCandidateBriefsToSend(
                            new PendingBriefRequest()
                            {
                                TestSittingId = testSittingId,
                                CredentialRequestStatus = CredentialRequestStatusTypeName.TestSessionAccepted,
                                SendDate = DateTime.Now.Date
                            })
                        .Any();
                    actions = new[] { SystemActionTypeName.WithdrawRequest, SystemActionTypeName.RejectTestSession, SystemActionTypeName.CheckIn, SystemActionTypeName.SendTestSessionReminder };
                    if (credentialPrerequisites.Select(x => x.CredentialType.Id).Contains(credentialRequest.CredentialTypeId) &&
                        credentialPrerequisites.Select(x => x.CredentialApplicationType.Id).Contains(credentialApplicationTypeId))
                    {
                        actions = actions.Concat(new[] { SystemActionTypeName.CreatePreRequisiteApplications }).ToArray();
                    }
                    if (hasPendingBriefs)
                    {
                        actions = actions.Concat(new[] { SystemActionTypeName.SendCandidateBrief }).ToArray();
                    }
                    
                    break;
                case CredentialRequestStatusTypeName.CheckedIn:
                    actions = new[] { SystemActionTypeName.WithdrawRequest, SystemActionTypeName.RejectTestSession, SystemActionTypeName.MarkAsSat, SystemActionTypeName.UndoCheckIn };
                    break;
                case CredentialRequestStatusTypeName.TestSat:
                    if (credentialRequest.ExternalCredentialName.Contains("Practice Test"))
                    {
                        actions = new[] { SystemActionTypeName.WithdrawRequest, SystemActionTypeName.RejectTestSession, SystemActionTypeName.UndoMarkAsSat, SystemActionTypeName.IssuePracticeTestResults, SystemActionTypeName.InvalidateTest };
                        break;
                    }
                    actions = new[] { SystemActionTypeName.WithdrawRequest, SystemActionTypeName.RejectTestSession, SystemActionTypeName.UndoMarkAsSat, SystemActionTypeName.IssueFail, SystemActionTypeName.IssuePass, SystemActionTypeName.InvalidateTest };
                    break;
                case CredentialRequestStatusTypeName.IssuedPassResult:
                    actions = new[] { SystemActionTypeName.IssueCredential, SystemActionTypeName.RevertResults };
                    break;
                case CredentialRequestStatusTypeName.CredentialOnHold:
                    actions = new[] { SystemActionTypeName.IssueCredential};
                    break;
                case CredentialRequestStatusTypeName.TestFailed:
                    actions = new[] { SystemActionTypeName.CreatePaidTestReview, SystemActionTypeName.CreateSupplementaryTest, SystemActionTypeName.RevertResults };
                    break;
                case CredentialRequestStatusTypeName.UnderPaidTestReview:
                    actions = new[] { SystemActionTypeName.IssueFailAfterReview, SystemActionTypeName.IssuePassAfterReview, SystemActionTypeName.WithdrawPaidReview };
                    break;
                case CredentialRequestStatusTypeName.ProcessingPaidReviewInvoice:
                    actions = new[] { SystemActionTypeName.PaidReviewInvoiceProcessed };
                    break;
                case CredentialRequestStatusTypeName.ProcessingSupplementaryTestInvoice:
                    actions = new[] { SystemActionTypeName.SupplementaryTestInvoiceProcessed };
                    break;
                case CredentialRequestStatusTypeName.TestInvalidated:
                    actions = new[] { SystemActionTypeName.RevertResults };
                    break;
                case CredentialRequestStatusTypeName.ToBeIssued:
                    actions = new[] { SystemActionTypeName.MarkForAssessment, SystemActionTypeName.IssueCredential };
                    break;
                case CredentialRequestStatusTypeName.ProcessingTestInvoice:
                    actions = new[] { SystemActionTypeName.TestInvoiceProcessed };
                    break;
                case CredentialRequestStatusTypeName.ProcessingRequest:
                    actions = new[] { SystemActionTypeName.MarkForAssessment };
                    break;
                case CredentialRequestStatusTypeName.OnHoldToBeIssued:
                    actions = new[] { SystemActionTypeName.IssueCredential };
                    break;
            }

            return new List<SystemActionNameModel>(
                actions
                // actions for all states
                .Union(new[] { SystemActionTypeName.SendEmail })
                .Select(x => new SystemActionNameModel
                {
                    Id = (int)x,
                    Name = actionDisplayNames.ContainsKey(x) ? actionDisplayNames[x] : x.ToString()
                }));
        }

        public IList<SystemActionNameModel> GetValidRolePlayerActions(RolePlayerStatusTypeName roleplayerStatus)
        {
            var actions = new SystemActionTypeName[] { };
            var actionDisplayNames = new Dictionary<SystemActionTypeName, string>
            {
                [SystemActionTypeName.RolePlayerMarkAsAccepted] = Naati.Resources.RolePlayer.RolePlayerMarkAsAccepted,
                [SystemActionTypeName.RolePlayerMarkAsRejected] = Naati.Resources.RolePlayer.RolePlayerMarkAsRejected,
                [SystemActionTypeName.RolePlayerMarkAsAttendedRehearsal] = Naati.Resources.RolePlayer.RolePlayerMarkAsAttendedRehearsal,
                [SystemActionTypeName.RolePlayerMarkAsAttendedTest] = Naati.Resources.RolePlayer.RolePlayerMarkAsAttendedTest,
                [SystemActionTypeName.RolePlayerMarkAsNoShow] = Naati.Resources.RolePlayer.RolePlayerMarkAsNoShow,
                [SystemActionTypeName.RolePlayerMarkAsPending] = Naati.Resources.RolePlayer.RolePlayerMarkAsPending,
                [SystemActionTypeName.RolePlayerMarkAsRemoved] = Naati.Resources.RolePlayer.RolePlayerMarkAsRemoved,

            };

            switch (roleplayerStatus)
            {
                case RolePlayerStatusTypeName.Pending:
                    actions = new[] { SystemActionTypeName.RolePlayerMarkAsAccepted, SystemActionTypeName.RolePlayerMarkAsRejected };
                    break;
                case RolePlayerStatusTypeName.Accepted:
                    actions = new[] {
                            SystemActionTypeName.RolePlayerMarkAsRejected,
                            SystemActionTypeName.RolePlayerMarkAsAttendedRehearsal,
                            SystemActionTypeName.RolePlayerMarkAsNoShow,
                            SystemActionTypeName.RolePlayerMarkAsPending
                    };
                    break;

                case RolePlayerStatusTypeName.Rejected:
                    actions = new[] {
                    SystemActionTypeName.RolePlayerMarkAsRemoved,
                 };
                    break;
                case RolePlayerStatusTypeName.NoShow:
                    actions = new[] {
                        SystemActionTypeName.RolePlayerMarkAsAccepted,
                        SystemActionTypeName.RolePlayerMarkAsAttendedRehearsal,
                    };
                    break;
                case RolePlayerStatusTypeName.Rehearsed:
                    actions = new[] {
                        SystemActionTypeName.RolePlayerMarkAsAccepted,
                        SystemActionTypeName.RolePlayerMarkAsAttendedTest,
                        SystemActionTypeName.RolePlayerMarkAsNoShow
                    };
                    break;
                case RolePlayerStatusTypeName.Attended:
                    actions = new[] {
                        SystemActionTypeName.RolePlayerMarkAsAttendedRehearsal,
                    };
                    break;
            }

            return new List<SystemActionNameModel>(
                actions.Select(x => new SystemActionNameModel
                {
                    Id = (int)x,
                    Name = actionDisplayNames.ContainsKey(x) ? actionDisplayNames[x] : x.ToString()
                }));
        }

        public IList<SystemActionNameModel> GetValidCredentialRequestActions(int credentialRequestStatus, int credentialRequestId)
        {
            return GetValidCredentialRequestActions((CredentialRequestStatusTypeName)credentialRequestStatus, credentialRequestId);
        }
        public IList<SystemActionNameModel> GetValidTestSittingActions(int testSittingId)
        {
            var response = _testServiceQueryService.GetTestSitting(new GetTestSittingRequest { TestSittingId = testSittingId });
            if (response.TestSitting.Rejected)
            {
                return new SystemActionNameModel[] { };
            }
            return GetValidCredentialRequestActions((CredentialRequestStatusTypeName)response.TestSitting.CredentialRequestStatusTypeId, response.TestSitting.CredentialRequestId);
        }

        public IList<SystemActionNameModel> GetValidCredentialRequestSummaryActions(int credentialRequestStatus)
        {
            var status = (CredentialRequestStatusTypeName)credentialRequestStatus;
            var actions = new SystemActionTypeName[] { };
            var actionDisplayNames = new Dictionary<SystemActionTypeName, string> // todo: awkward. if/when actions are in the DB, get display names from there
            {
                [SystemActionTypeName.AllocateTestSession] = Naati.Resources.Application.AllocateTestSessionActionLabel
            };

            switch (status)
            {
                case CredentialRequestStatusTypeName.TestAccepted:
                    actions = new[] { SystemActionTypeName.AllocateTestSession };
                    break;

                case CredentialRequestStatusTypeName.EligibleForTesting:
                    actions = new[] { SystemActionTypeName.AllocateTestSession };
                    break;
            }

            return new List<SystemActionNameModel>(
                actions.Select(x => new SystemActionNameModel
                {
                    Id = (int)x,
                    Name = actionDisplayNames.ContainsKey(x) ? actionDisplayNames[x] : x.ToString()
                }));
        }

        public GenericResponse<IEnumerable<TestMaterialWizardSteps>> GetTestMaterialWizardSteps(TestMaterialBulkAssignmentRequest request)
        {
            var steps = new List<TestMaterialWizardSteps>
            {
                TestMaterialWizardSteps.TestSpecification,
                TestMaterialWizardSteps.Skills,
                TestMaterialWizardSteps.TestMaterials,
                TestMaterialWizardSteps.Applicants,
                TestMaterialWizardSteps.ExaminersAndRolePlayers
            };

            var supplemenataryApplicants = _testMaterialQueryService.GetSupplementaryTestApplicants(
                new SupplementaryTestRequest
                {
                    TestSessionIds = request.TestSessionIds
                });
            if (supplemenataryApplicants.Any())
            {
                steps.Insert(0, TestMaterialWizardSteps.SupplementaryTestApplicants);
            }
            return new GenericResponse<IEnumerable<TestMaterialWizardSteps>>(steps);
        }

        public GenericResponse<IEnumerable<ApplicationWizardSteps>> GetWizardSteps(int applicationStatusTypeId, int actionId, int applicationId, int applicationTypeId, int credentialTypeId, int credentialRequestId)
        {
            var action = (SystemActionTypeName)actionId;
            var steps = new List<ApplicationWizardSteps>();
            var credentialPrerequisites = GetCredentialPrerequisites().Data;
            var credentialPrerequisiteIds = GetCredentialPrerequisiteIds(credentialPrerequisites).Data.ToList();
            var credentialPrerequisiteApplicationTypeIds = credentialPrerequisites.Select(x => x.CredentialApplicationType.Id).Distinct().ToList();
            var hasRelatedCredentialIdsOnHold = GetHasRelatedCredentialIdsOnHold(credentialRequestId);

            switch (action)
            {
                case SystemActionTypeName.SubmitApplication:
                    steps.AddRange(new[] { ApplicationWizardSteps.Notes });
                    break;
                case SystemActionTypeName.RejectApplication:
                case SystemActionTypeName.RejectApplicationAfterInvoice:
                    steps.Add(ApplicationWizardSteps.Notes);
                    break;
                case SystemActionTypeName.FinishChecking:
                    steps.AddRange(new[] { ApplicationWizardSteps.Notes });
                    break;
                case SystemActionTypeName.NewCredentialRequest:
                    steps.AddRange((CredentialApplicationStatusTypeName)applicationStatusTypeId == CredentialApplicationStatusTypeName.Draft
                        ? new[] { ApplicationWizardSteps.SelectCredential }
                        : new[] { ApplicationWizardSteps.SelectCredential, ApplicationWizardSteps.Notes });
                    break;
                case SystemActionTypeName.IssuePass:
                    steps.AddRange(new[] { ApplicationWizardSteps.Notes });
                    if (credentialPrerequisiteIds.IndexOf(credentialTypeId) != -1 &&
                        credentialPrerequisiteApplicationTypeIds.IndexOf(applicationTypeId) != -1)
                    {
                        steps.AddRange(new[] { ApplicationWizardSteps.PrerequisiteExemptions });
                        steps.AddRange(new[] { ApplicationWizardSteps.PrerequisiteSummary });
                    }
                    break;
                case SystemActionTypeName.IssuePracticeTestResults:
                    steps.AddRange(new[] { ApplicationWizardSteps.Notes });
                    break;
                case SystemActionTypeName.FailAssessment:
                case SystemActionTypeName.FailPendingAssessment:
                case SystemActionTypeName.PendAssessment:
                case SystemActionTypeName.CreatePaidReview:
                case SystemActionTypeName.CreatePaidTestReview:
                case SystemActionTypeName.PassReview:
                case SystemActionTypeName.FailReview:
                case SystemActionTypeName.CancelRequest:
                case SystemActionTypeName.WithdrawRequest:
                case SystemActionTypeName.CancelTestInvitation:
                case SystemActionTypeName.TestInvoicePaid:
                case SystemActionTypeName.SupplementaryTestInvoicePaid:
                case SystemActionTypeName.RejectTestSession:
                case SystemActionTypeName.IssueFail:
                case SystemActionTypeName.IssueFailAfterReview:
                case SystemActionTypeName.IssuePassAfterReview:
                case SystemActionTypeName.WithdrawSupplementaryTest:
                case SystemActionTypeName.WithdrawPaidReview:
                case SystemActionTypeName.CreateSupplementaryTest:
                case SystemActionTypeName.Reassess:
                case SystemActionTypeName.PassAssessment:
                case SystemActionTypeName.DeleteRequest:
                case SystemActionTypeName.ReactivateApplication:
                case SystemActionTypeName.CloseApplication:
                case SystemActionTypeName.InvalidateTest:
                case SystemActionTypeName.MarkForAssessment:
                case SystemActionTypeName.RejectRefund:
                case SystemActionTypeName.ProcessRefund:
                case SystemActionTypeName.CreditNoteProcessed:
                case SystemActionTypeName.CreditNotePaid:
                    steps.Add(ApplicationWizardSteps.Notes);
                    break;
                case SystemActionTypeName.IssueCredential:
                    steps.AddRange(new[] { ApplicationWizardSteps.Notes });
                    if (credentialPrerequisiteIds.IndexOf(credentialTypeId) != -1 &&
                        credentialPrerequisiteApplicationTypeIds.IndexOf(applicationTypeId) != -1)
                    {
                        steps.AddRange(new[] { ApplicationWizardSteps.PrerequisiteExemptions });
                        steps.AddRange(new[] { ApplicationWizardSteps.PrerequisiteSummary });
                        steps.AddRange(new[] { ApplicationWizardSteps.IncompletePrerequisiteCredentials });
                    }
                    if (hasRelatedCredentialIdsOnHold)
                    {
                        steps.AddRange(new[] { ApplicationWizardSteps.PrerequisiteIssueOnHoldCredentials });
                    }
                    steps.AddRange(new[] { ApplicationWizardSteps.IssueCredential, ApplicationWizardSteps.DocumentsPreview });
                    break;
                case SystemActionTypeName.CreatePreRequisiteApplications:
                        steps.AddRange(new[] { ApplicationWizardSteps.PrerequisiteSummary, ApplicationWizardSteps.PrerequisiteExemptions, ApplicationWizardSteps.PrerequisiteApplications,
                        ApplicationWizardSteps.PrerequisiteMandatoryFields,ApplicationWizardSteps.PrerequisiteMandatoryDocumentTypes,ApplicationWizardSteps.PrerequisiteConfirmApplicationCreation });                   
                    break;
                case SystemActionTypeName.RevertResults:
                    steps.AddRange(new[] { ApplicationWizardSteps.Notes });
                    break;
                case SystemActionTypeName.SendCandidateBrief:
                    steps.AddRange(new[] { ApplicationWizardSteps.Notes, ApplicationWizardSteps.DocumentsPreview });
                    break;
                case SystemActionTypeName.DeleteApplication:
                    steps.AddRange(new[] { ApplicationWizardSteps.Notes, ApplicationWizardSteps.DeleteConfirmation });
                    break;
                case SystemActionTypeName.ReissueCredential:
                    steps.AddRange(new[] { ApplicationWizardSteps.Notes, ApplicationWizardSteps.IssueCredential, ApplicationWizardSteps.DocumentsPreview });
                    break;
                case SystemActionTypeName.RequestRefund:
                    steps.AddRange(new[] { ApplicationWizardSteps.ConfigureRefund, ApplicationWizardSteps.Notes });
                    break;
                case SystemActionTypeName.ApproveRefund:
                    steps.AddRange(new[] { ApplicationWizardSteps.ApproveRefund, ApplicationWizardSteps.Notes });
                    break;
                case SystemActionTypeName.AssessmentInvoicePaid:
                case SystemActionTypeName.ApplicationInvoicePaid:
                case SystemActionTypeName.PaidReviewInvoicePaid:
                    steps.Add(ApplicationWizardSteps.Notes);
                    break;
                case SystemActionTypeName.CancelAssessment:
                case SystemActionTypeName.CancelChecking:
                case SystemActionTypeName.CreateApplication:
                case SystemActionTypeName.FinaliseApplication:
                case SystemActionTypeName.StartAssessing:
                case SystemActionTypeName.StartChecking:
                case SystemActionTypeName.UndoCheckIn:
                case SystemActionTypeName.UndoMarkAsSat:
                case SystemActionTypeName.ApplicationSubmissionProcessed:
                case SystemActionTypeName.PaidReviewInvoiceProcessed:
                case SystemActionTypeName.SupplementaryTestInvoiceProcessed:
                case SystemActionTypeName.SendRecertificationReminder:
                case SystemActionTypeName.SendTestSessionReminder:
                case SystemActionTypeName.SendSessionAvailabilityNotice:
                    steps.AddRange(new[] { ApplicationWizardSteps.Notes });
                    break;
                case SystemActionTypeName.CheckIn:
                case SystemActionTypeName.MarkAsSat:
                case SystemActionTypeName.TestInvoiceProcessed:
                    break;
                case SystemActionTypeName.SendEmail:
                    steps.AddRange(new[] { ApplicationWizardSteps.ComposeEmail, ApplicationWizardSteps.EmailPreview });
                    break;
                default:
                    throw new Exception($"Unexpected {nameof(SystemActionTypeName)}: {action}.");
            }

            // show invoice preview if application requires it. 

            if (action == SystemActionTypeName.SubmitApplication)
            {
                var applicationService = ServiceLocator.Resolve<IApplicationService>();
                var fee = applicationService.GetApplicationFee(applicationId);
                if (fee != null)
                {
                    steps.Add(ApplicationWizardSteps.ViewInvoice);
                }
            }

            if (action == SystemActionTypeName.IssueFail || action == SystemActionTypeName.IssueFailAfterReview)
            {
                var testService = ServiceLocator.Resolve<ITestService>();
                var allowSupplementary = testService.CheckIfAllowSupplementaryTest(credentialRequestId).Data;
                if (allowSupplementary)
                {
                    steps.Add(ApplicationWizardSteps.SupplementaryTest);
                }
            }

            if (action == SystemActionTypeName.IssueFail || action == SystemActionTypeName.IssueFailAfterReview)
            {
                var testService = ServiceLocator.Resolve<ITestService>();
                var allowConcededPass = testService.CheckIfAllowConcededPass(credentialRequestId).Data;

                if (allowConcededPass)
                {
                    var applicationService = ServiceLocator.Resolve<IApplicationService>();
                    var downgradeCredential = applicationService.GetDowngradedCredentialRequest(credentialRequestId);
                    if (downgradeCredential == null)
                    {
                        steps.Add(ApplicationWizardSteps.NotFoundConcededCredential);

                    }
                    else if (downgradeCredential.HasCredential)
                    {
                        steps.Add(ApplicationWizardSteps.ExistingConcededCredential);
                    }
                    else
                    {
                        steps.Add(ApplicationWizardSteps.IssueConcededPass);
                        steps.Add(ApplicationWizardSteps.IssueCredential);
                        steps.Add(ApplicationWizardSteps.DocumentsPreview);
                    }
                }
            }

            var emailTemplateReponse = _applicationQueryService.GetEmailTemplate(new GetEmailTemplateRequest
            {
                ApplicationType = (CredentialApplicationTypeName)applicationTypeId,
                Action = action
            });

            if (action != SystemActionTypeName.CreatePreRequisiteApplications && 
                action != SystemActionTypeName.SendEmail) 
            {
                if (emailTemplateReponse.Data != null && emailTemplateReponse.Data.Any(x => x.Active))
                {
                    if (CanOverrideEmail())
                    {
                        steps.Add(ApplicationWizardSteps.CheckOption);
                    }
                    steps.Add(ApplicationWizardSteps.EmailPreview);
                }
            }

            if (action == SystemActionTypeName.CreateSupplementaryTest || action == SystemActionTypeName.CreatePaidTestReview)
            {
                steps.Add(ApplicationWizardSteps.ViewMessage);
            }

            return new GenericResponse<IEnumerable<ApplicationWizardSteps>>(steps);
        }

        public GenericResponse<NoteFieldRules> GetNoteFieldRules(int actionId, int applicationId)
        {
            var applicationResponse = _applicationQueryService.GetApplication(applicationId);
            var applicationType = applicationResponse.Result.ApplicationTypeId;

            return GetNoteFieldRulesByApplicationTypeId(actionId, applicationType);
        }

        public GenericResponse<NoteFieldRules> GetNoteFieldRulesByApplicationTypeId(int actionId, int applicationTypeId)
        {
            var action = (SystemActionTypeName)actionId;
            var rules = new NoteFieldRules
            {
                ShowPrivateNote = true,
                RequirePublicNote = RequiresPublicNote.Contains(action),
                RequirePrivateNote = RequiresPrivateNote.Contains(action)
            };

            if (applicationTypeId == 0)
                return new GenericResponse<NoteFieldRules>(rules);

            var emailTemplateReponse = _applicationQueryService.GetEmailTemplate(new GetEmailTemplateRequest
            {
                ApplicationType = (CredentialApplicationTypeName)applicationTypeId,
                Action = action
            });

            var actionPublicNoteName = _tokenReplacementService.GetTokenNameFor(TokenReplacementField.ActionPublicNote);
            rules.ShowPublicNote =
                emailTemplateReponse.Data != null &&
                emailTemplateReponse.Data.Any(x => x.Active &&
                                                   (
                                                       x.Content.IndexOf($"[[{actionPublicNoteName}]]") != -1 ||
                                                       x.Subject.IndexOf($"[[{actionPublicNoteName}]]") != -1
                                                   ));

            return new GenericResponse<NoteFieldRules>(rules);
        }

        private GenericResponse<IEnumerable<CredentialPrerequisite>> GetCredentialPrerequisites()
        {
            var credentialPrerequisites = _credentialPrerequisiteService.GetCredentialPrerequisites();

            var response = new GenericResponse<IEnumerable<CredentialPrerequisite>>
            {
                Data = credentialPrerequisites
            };

            return response;
        }

        private GenericResponse<IEnumerable<int>> GetCredentialPrerequisiteIds(IEnumerable<CredentialPrerequisite> credentialPrerequisites)
        {
            var credentialPrerequisiteIds = _credentialPrerequisiteService.GetCredentialPrerequisiteIds(credentialPrerequisites);

            return new GenericResponse<IEnumerable<int>>
            {
                Data = credentialPrerequisiteIds
            };
        }

        private bool GetHasRelatedCredentialIdsOnHold(int credentialRequestId)
        {
            var relatedCredentialIdsOnHoldResponse = _credentialPrerequisiteService.GetRelatedCredentialIdsOnHold(credentialRequestId);

            if (!relatedCredentialIdsOnHoldResponse.Success)
            {
                var errors = new StringBuilder();

                foreach(var error in relatedCredentialIdsOnHoldResponse.Errors)
                {
                    errors.AppendLine(error);
                }

                throw new Exception(errors.ToString());
            }

            var relatedCredentialIdsOnHold = relatedCredentialIdsOnHoldResponse.Data;

            if (relatedCredentialIdsOnHold.IsNull() || !relatedCredentialIdsOnHold.Any())
            {
                return false;
            }

            return true;
        }
    }
}