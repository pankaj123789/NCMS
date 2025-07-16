using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Bl.AutoMappingProfiles;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Bl;
using Ncms.Bl.ApplicationActions;
using Ncms.Bl.AutoMappingProfiles;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.Common;
using Ncms.Contracts.Models.File;
using Ncms.Contracts.Models.User;
using Ncms.Test.AutoMappingProfiles;
using Ncms.Test.Utils;
using NSubstitute;
using Xunit;

namespace Ncms.Test.BusinessServices
{
    public class ApplicationStateActionTest 
    {
       

        [Theory]
        [InlineData(SystemActionTypeName.CreateApplication, typeof(ApplicationCreateAction))]
        [InlineData(SystemActionTypeName.NewCredentialRequest, typeof(CredentialRequestCreateAction))]
        [InlineData(SystemActionTypeName.SubmitApplication, typeof(ApplicationSubmitAction))]
        [InlineData(SystemActionTypeName.StartChecking, typeof(ApplicationStartCheckingAction))]
        [InlineData(SystemActionTypeName.CancelChecking, typeof(ApplicationCancelCheckingAction))]
        [InlineData(SystemActionTypeName.FinishChecking, typeof(ApplicationFinishCheckingAction))]
        [InlineData(SystemActionTypeName.RejectApplication, typeof(ApplicationRejectAction))]
        [InlineData(SystemActionTypeName.RejectApplicationAfterInvoice, typeof(ApplicationRejectAction))]
        [InlineData(SystemActionTypeName.FinaliseApplication, typeof(ApplicationFinaliseAction))]
        [InlineData(SystemActionTypeName.StartAssessing, typeof(CredentialRequestStartAssessingAction))]
        [InlineData(SystemActionTypeName.CancelAssessment, typeof(CredentialRequestCancelAssessmentAction))]
        [InlineData(SystemActionTypeName.FailAssessment, typeof(CredentialRequestFailAssessmentAction))]
        [InlineData(SystemActionTypeName.FailPendingAssessment, typeof(CredentialRequestFailAssessmentAction))]
        [InlineData(SystemActionTypeName.PendAssessment, typeof(CredentialRequestPendAssessmentAction))]
        [InlineData(SystemActionTypeName.PassAssessment, typeof(CredentialRequestPassAssessmentAction))]
        [InlineData(SystemActionTypeName.PassReview, typeof(CredentialRequestPassReviewAction))]
        [InlineData(SystemActionTypeName.CreatePaidReview, typeof(CredentialRequestCreateReviewAction))]
        [InlineData(SystemActionTypeName.FailReview, typeof(CredentialRequestFailReviewAction))]
        [InlineData(SystemActionTypeName.IssueCredential, typeof(CredentialRequestIssueCredentialAction))]
        [InlineData(SystemActionTypeName.CancelRequest, typeof(CredentialRequestCancelAction))]
        [InlineData(SystemActionTypeName.DeleteRequest, typeof(CredentialRequestDeleteAction))]
        [InlineData(SystemActionTypeName.DeleteApplication, typeof(ApplicationDeleteAction))]
        [InlineData(SystemActionTypeName.ReissueCredential, typeof(CredentialRequestReissueCredentialAction))]
        [InlineData(SystemActionTypeName.AssessmentInvoicePaid, typeof(ApplicationAssessmentInvoicePaidAction))]
        [InlineData(SystemActionTypeName.ApplicationInvoicePaid, typeof(ApplicationInvoicePaidAction))]
        [InlineData(SystemActionTypeName.WithdrawRequest, typeof(CredentialRequestWithdrawAction))]
        [InlineData(SystemActionTypeName.WithdrawSupplementaryTest, typeof(CredentialRequestWithdrawSupplementaryTestAction))]
        [InlineData(SystemActionTypeName.TestInvoicePaid, typeof(CredentialRequestTestInvoicePaidAction))]
        [InlineData(SystemActionTypeName.CancelTestInvitation, typeof(CredentialRequestCancelTestInvitationAction))]
        [InlineData(SystemActionTypeName.SupplementaryTestInvoicePaid, typeof(CredentialRequestSupplementaryTestInvoicePaidAction))]
        [InlineData(SystemActionTypeName.AllocateTestSession, typeof(CredentialRequestAllocateTestSessionAction))]

        [InlineData(SystemActionTypeName.RejectTestSession, typeof(CredentialRequestRejectTestSessionAction))]
        [InlineData(SystemActionTypeName.RejectTestSessionFromMyNaati, typeof(CredentialRequestRejectTestSessionAction))]
        [InlineData(SystemActionTypeName.CheckIn, typeof(CredentialRequestCheckInAction))]
        [InlineData(SystemActionTypeName.MarkAsSat, typeof(CredentialRequestMarkAsSatAction))]
        [InlineData(SystemActionTypeName.UndoCheckIn, typeof(CredentialRequestUndoCheckInAction))]
        [InlineData(SystemActionTypeName.UndoMarkAsSat, typeof(CredentialRequestUndoMarkAsSatAction))]
        [InlineData(SystemActionTypeName.IssueFail, typeof(CredentialRequestIssueFailAction))]
        [InlineData(SystemActionTypeName.IssuePass, typeof(CredentialRequestIssuePassAction))]
        [InlineData(SystemActionTypeName.CreatePaidTestReview, typeof(CredentialRequestCreatePaidTestReviewAction))]

        [InlineData(SystemActionTypeName.IssueFailAfterReview, typeof(CredentialRequestIssueFailAfterReviewAction))]
        [InlineData(SystemActionTypeName.IssuePassAfterReview, typeof(CredentialRequestIssuePassAfterReviewAction))]
        [InlineData(SystemActionTypeName.CreateSupplementaryTest, typeof(CredentialRequestCreateSupplementaryTestAction))]

        [InlineData(SystemActionTypeName.AllocateTestSessionFromMyNaati, typeof(CredentialRequestAllocateAndAcceptTestSesssionAction))]

        [InlineData(SystemActionTypeName.ApplicationSubmissionProcessed, typeof(ApplicationSubmissionProcessedAction))]
        [InlineData(SystemActionTypeName.AssignTestMaterial, typeof(CredentialRequestAssignTestMaterialAction))]
        [InlineData(SystemActionTypeName.Reassess, typeof(CredentialRequestReassessAction))]
        [InlineData(SystemActionTypeName.PaidReviewInvoiceProcessed, typeof(CredentialRequestPaidReviewInvoiceProcessedAction))]
        [InlineData(SystemActionTypeName.WithdrawPaidReview, typeof(CredentialRequestWithdrawPaidReviewAction))]
        [InlineData(SystemActionTypeName.PaidReviewInvoicePaid, typeof(CredentialRequestPaidReviewInvoicePaidAction))]
        public void CreateAction_WhenMethodIsCalled_ConfiguredActionIsCreated(SystemActionTypeName actionTypeName, Type actionType)
        {
            // Arrange
            var application = new CredentialApplicationDetailedModel();
            var applicationModel = new ApplicationActionWizardModel();


            // Act
            var action = ApplicationStateAction.CreateAction(actionTypeName, application, applicationModel);

            // Assert
            Assert.Equal(actionType, action.GetType());
        }

        [Fact]
        public void CreateAction_WhenMethodIsCalled_InstanceIsProperlyConfigured()
        {
            // Arrange
            var application = new CredentialApplicationDetailedModel();
            var wizardModel = new ApplicationActionWizardModel();

            // Act
            var action = ApplicationStateAction.CreateAction(SystemActionTypeName.CreateApplication, application, wizardModel);

            // Assert
            Assert.Equal(action.ActionModel, application);
            Assert.Equal(wizardModel, action.WizardModel);

        }

        private class ApplicationStateActionStub : ApplicationStateAction
        {
            internal List<Action> ProtectedPreconditions { get; set; } = new List<Action>();
            internal List<Action> ProtectedSystemActions { get; set; } = new List<Action>();
            internal IList<ValidationResultModel> ProtectedValidationErrors => ValidationErrors;
            internal IList<EmailMessageAttachmentModel> ProtectedAttachemnts => Attachments;
            internal ApplicationActionOutput ProtectedOutput => Output;

            internal void ProtectedValidateUserPermissions()
            {
                this.ValidateUserPermissions();
            }

            internal void ProtectedMandatoryFields()
            {
                this.ValidateMandatoryFields();
            }

            internal void ProtectedValidateMandatoryDocuments()
            {
                this.ValidateMandatoryDocuments();
            }

            internal void ProtectedValidateMinimumCredentialRequests()
            {
                this.ValidateMinimumCredentialRequests();
            }

            internal void ProtectedValidateEntryState()
            {
                this.ValidateEntryState();
            }
            internal CredentialApplicationStatusTypeName[] ProtectedApplicationEntryStates { get; set; } = new CredentialApplicationStatusTypeName[] { };


            protected override CredentialApplicationStatusTypeName[] ApplicationEntryStates =>
                ProtectedApplicationEntryStates;
            protected override IList<Action> Preconditions => ProtectedPreconditions;
            protected override IList<Action> SystemActions => ProtectedSystemActions;

            public ApplicationStateActionStub(IServiceLocator serviceLocator) : base(serviceLocator)
            {

            }

            protected override string CreateInvoiceFile(FileModel invoicePdf)
            {
                return "TESTPATH";
            }
        }

        [Fact]
        public void Perform_WhenMethodWithValidPrecondition_ThenAllPreconditionsAreValidated()
        {
            // Arrange
            var serviceLocator = ServiceLocatorTestHelper.GetInstance();
            var applicationStateStub = new ApplicationStateActionStub(serviceLocator);

            var preconditionsCalled = new List<int>();

            applicationStateStub.ProtectedPreconditions = new List<Action>
                {
                    () => { preconditionsCalled.Add(1); },
                    () => { preconditionsCalled.Add(2); },
                    () => { preconditionsCalled.Add(3); },
                };

            // Act
            applicationStateStub.Perform();

            // Assert
            Assert.Equal(3, preconditionsCalled.Count);
            Assert.Equal(1, preconditionsCalled.SingleOrDefault(x => x == 1));
            Assert.Equal(2, preconditionsCalled.SingleOrDefault(x => x == 2));
            Assert.Equal(3, preconditionsCalled.SingleOrDefault(x => x == 3));

        }


        [Fact]
        public void Perform_WhenMethodWithInvalidValidPrecondition_ThenuUserFriendlyExceptionIsThrownAndRestOfPreconditionsAreNotValidated()
        {
            // Arrange
            var serviceLocator = ServiceLocatorTestHelper.GetInstance();
            var applicationStateStub = new ApplicationStateActionStub(serviceLocator);
            var calledPreconditions = new List<int>();
            var exceptionMessage = "TEST";

            applicationStateStub.ProtectedPreconditions = new List<Action>
                {
                    () => { calledPreconditions.Add(1); },
                    () =>
                    {
                        applicationStateStub.ProtectedValidationErrors.Add(new ValidationResultModel {Field = "TestFied", Message = exceptionMessage});
                        calledPreconditions.Add(2);
                    },
                    () => { calledPreconditions.Add(3); },
                };


            // Act
            Assert.Throws<UserFriendlySamException>(() => applicationStateStub.Perform());


            // Assert
            Assert.Equal(3, calledPreconditions.Count);
            Assert.Equal(1, calledPreconditions.SingleOrDefault(x => x == 1));
            Assert.Equal(2, calledPreconditions.SingleOrDefault(x => x == 2));
            Assert.Equal(3, calledPreconditions.SingleOrDefault(x => x == 3));

        }

        [Fact]
        public void Perfom_WhenCalledAndEntryStateIsNotMet_ThenExceptionIsThrown()
        {

            // Arrange
            var assemblies = new[]
            {
                typeof(AccountingProfile).Assembly,
                typeof(PodHistoryProfile).Assembly,
            };

           
            var serviceLocator = ServiceLocatorTestHelper.GetInstance();

            var autoMapperHelper = new AutoMapperHelper();
            autoMapperHelper.Configure(assemblies, true);
            serviceLocator.MockService<IAutoMapperHelper>(autoMapperHelper);
            var validEntryState = CredentialApplicationStatusTypeName.AwaitingApplicationPayment;
            var applicationStateStub = new ApplicationStateActionStub(serviceLocator)
            {
                ActionModel = new CredentialApplicationDetailedModel
                {
                    ApplicationStatus =
                        new LookupTypeDetailedModel() { Id = (int)CredentialApplicationStatusTypeName.Entered }
                },
                ProtectedApplicationEntryStates = new[] { validEntryState }
            };

            applicationStateStub.ProtectedPreconditions = new List<Action>
            {
                () => { applicationStateStub.ProtectedValidateEntryState(); },
            };

            var applicationService = serviceLocator.MockService<IApplicationService>();

            applicationService.GetLookupType("CredentialApplicationStatusType")
                .Returns(new GenericResponse<IEnumerable<LookupTypeModel>>(new List<LookupTypeModel>
                {
                    new LookupTypeModel
                    {
                        Id = (int) validEntryState,
                        DisplayName = string.Empty
                    }

                }));

            // Act
            Assert.Throws<UserFriendlySamException>(() => applicationStateStub.Perform());
        }

        [Fact]
        public void Perfom_WhenCalledAndUserPermissionsAreNotMet_ThenExceptionIsThrown()
        {
            // Arrange
            var serviceLocator = ServiceLocatorTestHelper.GetInstance();
            var applicationStateStub = new ApplicationStateActionStub(serviceLocator);
            applicationStateStub.ProtectedPreconditions = new List<Action>
            {
                () => { applicationStateStub.ProtectedValidateUserPermissions(); },
            };


            var userService = serviceLocator.MockService<IUserService>();
            userService.Get().Returns(new UserModel { Id = 0 }); ;


            // Act
            Assert.Throws<UserFriendlySamException>(() => applicationStateStub.Perform());
        }

        [Fact]
        public void Perfom_WhenCalledAndMinimuCredentialRequestAreNotMet_ThenExceptionIsThrown()
        {
            // Arrange
            var serviceLocator = ServiceLocatorTestHelper.GetInstance();
            var applicationStateStub = new ApplicationStateActionStub(serviceLocator)
            {
                ActionModel = new CredentialApplicationDetailedModel
                {
                    CredentialRequests = new List<CredentialRequestModel>()
                }
            };
            applicationStateStub.ProtectedPreconditions = new List<Action>
            {
                () => { applicationStateStub.ProtectedValidateMinimumCredentialRequests(); },
            };

            // Act
            Assert.Throws<UserFriendlySamException>(() => applicationStateStub.Perform());
        }


        [Fact]
        public void Perfom_WhenCalledAndMandatoryDocumentsAreNotMet_ThenExceptionIsThrown()
        {


            // Arrange
            var serviceLocator = ServiceLocatorTestHelper.GetInstance();

            var applicationStateStub = new ApplicationStateActionStub(serviceLocator)
            {
                ActionModel = new CredentialApplicationDetailedModel
                {
                    CredentialRequests = new List<CredentialRequestModel>(),
                    ApplicationInfo = new CredentialApplicationInfoModel()
                    {
                        ReceivingOfficeId = 0,
                    },
                    ApplicationType = new CredentialApplicationTypeModel()
                    {
                        CredentialApplicationFields = new List<CredentialApplicationFieldModel>(),
                        CredentialApplicationTypeDocumentTypes = new List<CredentialApplicationTypeDocumentTypeModel>
                        {
                            new CredentialApplicationTypeDocumentTypeModel
                            {
                                DocumentType = new DocumentTypeModel
                                {
                                    DisplayName = string.Empty,
                                },
                                Mandatory = true
                            }
                        }
                    },
                    Fields = new List<CredentialApplicationFieldModel>()
                }
            };
            applicationStateStub.ProtectedPreconditions = new List<Action>
            {
                () => { applicationStateStub.ProtectedValidateMandatoryDocuments(); },
            };
            var applicationService = serviceLocator.MockService<IApplicationService>();
            applicationService.ListAttachments(Arg.Any<int>())
                .Returns(Enumerable.Empty<CredentialApplicationAttachmentModel>());

            // Act
            Assert.Throws<UserFriendlySamException>(() => applicationStateStub.Perform());
        }

        [Fact]
        public void Perfom_WhenCalledAndReceivingOficceIdIsLessOrEqualsToCero_ThenExceptionIsThrown()
        {
            // Arrange
            var serviceLocator = ServiceLocatorTestHelper.GetInstance();
            var applicationStateStub = new ApplicationStateActionStub(serviceLocator)
            {
                ActionModel = new CredentialApplicationDetailedModel
                {
                    CredentialRequests = new List<CredentialRequestModel>(),
                    ApplicationInfo = new CredentialApplicationInfoModel()
                    {
                        ReceivingOfficeId = 0,
                    },
                    ApplicationType = new CredentialApplicationTypeModel()
                    {
                        CredentialApplicationFields = new List<CredentialApplicationFieldModel>()
                    },
                    Fields = new List<CredentialApplicationFieldModel>()
                }
            };
            applicationStateStub.ProtectedPreconditions = new List<Action>
            {
                () => { applicationStateStub.ProtectedMandatoryFields(); },
            };

            // Act
            Assert.Throws<UserFriendlySamException>(() => applicationStateStub.Perform());
        }


        [Fact]
        public void Perform_WhenMethodWithInvalidValidPrecondition_ThenCustomExceptionIsThrownAndRestOfPreconditionsAreNotValidated()
        {
            // Arrange
            var serviceLocator = ServiceLocatorTestHelper.GetInstance();
            var applicationStateStub = new ApplicationStateActionStub(serviceLocator);
            var preconditionsCalled = new List<int>();
            var exceptionMessage = "TEST";

            applicationStateStub.ProtectedPreconditions = new List<Action>
                {
                    () => { preconditionsCalled.Add(1); },
                    () => { throw new Exception(exceptionMessage); },
                    () => { preconditionsCalled.Add(3); },
                };


            // Act
            var exception = Assert.Throws<Exception>(() => applicationStateStub.Perform());


            // Assert
            Assert.Equal(exceptionMessage, exception.Message);
            Assert.Single(preconditionsCalled);
            Assert.Equal(1, preconditionsCalled.SingleOrDefault(x => x == 1));

        }


    }
}
