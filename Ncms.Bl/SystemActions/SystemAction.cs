using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Bl.Payment;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using F1Solutions.Naati.Common.Contracts.Security;
using F1Solutions.Naati.Common.Dal.Finance.PayPal;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.System;
using Ncms.Contracts.Models.User;
using IApplicationService = Ncms.Contracts.IApplicationService;
using IFileService = Ncms.Contracts.IFileService;
using IPersonService = Ncms.Contracts.IPersonService;
using ISystemService = Ncms.Contracts.ISystemService;
using ITestMaterialService = Ncms.Contracts.ITestMaterialService;
using IUserService = Ncms.Contracts.IUserService;


namespace Ncms.Bl.SystemActions
{
    public abstract class SystemAction<TActionModelType, TWizardModelType, TOutputModelType, TUpsertType, TEmailMessageModelType>
        where TActionModelType : SystemActionModel
        where TWizardModelType : SystemActionWizardModel
        where TOutputModelType : SytemActionOutput<TUpsertType, TEmailMessageModelType>
        where TEmailMessageModelType : EmailMessageModel, new()
        where TUpsertType : new()
    {
        internal TActionModelType ActionModel { get; set; }
        internal TWizardModelType WizardModel { get; set; }
        protected TOutputModelType Output { get; set; }


        private UserModel _currentUser = null;
        protected IServiceLocator ServiceLocatorInstance { get; set; }
        protected virtual SecurityNounName? RequiredSecurityNoun => null;
        protected virtual SecurityVerbName? RequiredSecurityVerb => null;
        public IList<ValidationResultModel> ValidationErrors { get; } = new List<ValidationResultModel>();
        protected virtual List<EmailMessageAttachmentModel> Attachments { get; } = new List<EmailMessageAttachmentModel>();
        private IEnumerable<ActionEventLevel> _mActionEvents;
        protected virtual IList<Action> Preconditions => new List<Action>();
        protected virtual IList<Action> SystemActions => new List<Action>();

        public IEnumerable<ActionEventLevel> ActionEvents => (_mActionEvents ?? (_mActionEvents = GetActionEvents()));
        protected UserModel CurrentUser => _currentUser ?? (_currentUser = UserService.Get());

        protected virtual IList<ActionEventLevel> GetActionEvents()
        {
            var events = new List<ActionEventLevel>
            {
                new ActionEventLevel {Level = -1, Event = SystemActionEventTypeName.None,}
            };

            return events;
        }

        protected IApplicationService ApplicationService => ServiceLocatorInstance.Resolve<IApplicationService>();
        protected IActivityPointsCalculatorService ActivityPointsCalculatorService => ServiceLocatorInstance.Resolve<IActivityPointsCalculatorService>();
        protected ICredentialPointsCalculatorService CredentialPointsCalculatorService => ServiceLocatorInstance.Resolve<ICredentialPointsCalculatorService>();
        protected ITestResultService TestResultService => ServiceLocatorInstance.Resolve<ITestResultService>();
        protected IAccountingService AccountingService => ServiceLocatorInstance.Resolve<IAccountingService>();
        protected ITestService TestService => ServiceLocatorInstance.Resolve<ITestService>();
        protected IExaminerService ExaminerService => ServiceLocatorInstance.Resolve<IExaminerService>();
        protected ITestSessionService TestSessionService => ServiceLocatorInstance.Resolve<ITestSessionService>();
        protected ITestMaterialService TestMaterialService => ServiceLocatorInstance.Resolve<ITestMaterialService>();
        protected IAutoMapperHelper AutoMapperHelper => ServiceLocatorInstance.Resolve<IAutoMapperHelper>();
        protected IPanelService PanelService => ServiceLocatorInstance.Resolve<IPanelService>();
        protected IActivityPointsCalculatorService ActivityPointsService => ServiceLocatorInstance
            .Resolve<IActivityPointsCalculatorService>();

        protected ICredentialPointsCalculatorService CredentialPointsService => ServiceLocatorInstance
            .Resolve<ICredentialPointsCalculatorService>();

        protected IFileService FileService => ServiceLocatorInstance.Resolve<IFileService>();
        protected IEmailMessageService EmailMessageService => ServiceLocatorInstance.Resolve<IEmailMessageService>();
        protected IEmailTemplateQueryService EmailTemplateQueryService => ServiceLocatorInstance.Resolve<IEmailTemplateQueryService>();


        protected ITokenReplacementService TokenReplacementService => ServiceLocatorInstance
            .Resolve<ITokenReplacementService>();

        protected ISystemService SystemService => ServiceLocatorInstance.Resolve<ISystemService>();
        protected IPersonService PersonService => ServiceLocatorInstance.Resolve<IPersonService>();

        protected IUserService UserService => ServiceLocatorInstance.Resolve<IUserService>();
        protected IMaterialRequestService MaterialRequestService => ServiceLocatorInstance.Resolve<IMaterialRequestService>();
        protected IFinanceService FinanceService => ServiceLocatorInstance.Resolve<IFinanceService>();
        protected IPaymentClient PaymentClient => ServiceLocatorInstance.Resolve<IPaymentClient>();
        protected IPayPalService PayPalService => ServiceLocatorInstance.Resolve<IPayPalService>();
        protected SystemAction(IServiceLocator serviceLocator = null)
        {
            if (serviceLocator == null)
            {
                serviceLocator = ServiceLocator.GetInstance();
            }

            ServiceLocatorInstance = serviceLocator;
        }

        public bool ArePreconditionsMet()
        {
            ValidatePreconditions();
            return ValidationErrors == null || !ValidationErrors.Any();
        }

        public virtual void ValidatePreconditions()
        {
            ValidationErrors.Clear();
            Preconditions.ForEach(x => x());
        }

        protected virtual void ValidateUserPermissions()
        {
            if (RequiredSecurityNoun == null)
            {
                throw new Exception("Missing RequiredSecurityNoun");
            }
            if (RequiredSecurityVerb == null)
            {
                throw new Exception("Missing RequiredSecurityVerb");
            };

            if (!UserService.HasPermission(RequiredSecurityNoun.Value, RequiredSecurityVerb.Value))
            {
                LoggingHelper.LogWarning("Workflow action access denied: {RequiredRights}, action {WorkflowAction}", $"{RequiredSecurityNoun.Value}.{RequiredSecurityVerb.Value}", GetType().Name);
                throw new UserFriendlySamException(Naati.Resources.Server.UnauthorisedAccess);
            }
        }

        public virtual void Perform()
        {
            // last chance validation
            if (!ArePreconditionsMet())
            {
                throw new UserFriendlySamException("One or more preconditions for this action have not been met. " +
                                                   String.Join("; ", ValidationErrors.Select(x => x.Message)));
            }

            Log();

            SystemActions.ForEach(x => x());
        }


        protected virtual void ConfigureInstance(
            TActionModelType actionModel,
            TWizardModelType wizardModel,
            TOutputModelType outputModel)
        {
            ActionModel = actionModel;
            WizardModel = wizardModel;
            Output = outputModel;
        }

        protected abstract void CreateEmailAttachmentsIfApplicable();


        public virtual IEnumerable<TEmailMessageModelType> CreatePendingEmails()
        {
            CreatePendingEmailIfApplicable();
            return Output.PendingEmails ?? Enumerable.Empty<TEmailMessageModelType>();
        }

        public virtual void SaveChanges()
        {
            var response = SaveActionData();

            CreateEmailAttachmentsIfApplicable();

            var pendingEmails = CreatePendingEmails().ToList();

            Output.PendingEmails = new List<TEmailMessageModelType>();

            foreach (var emailModel in pendingEmails)
            {
                var emailCreateResponse = SaveEmailMessage(emailModel);

                if (!emailCreateResponse.Success)
                {
                    throw new Exception(emailCreateResponse.Errors.FirstOrDefault());
                }
                var email = emailCreateResponse.Data;
                Output.PendingEmails.Add(email);
            }


            Output.UpsertResults = response;
        }

        protected abstract GenericResponse<TEmailMessageModelType> SaveEmailMessage(TEmailMessageModelType message);
       


        protected virtual bool CanSendEmail()
        {
            return WizardModel.SendEmail;
        }

        protected abstract GenericResponse<TUpsertType> SaveActionData();

        protected virtual IEnumerable<BusinessServiceResponse> SendEmails()
        {
            var responses = new List<BusinessServiceResponse>();
            foreach (var emailModel in Output.PendingEmails)
            {
                var emailSendResponse = EmailMessageService.SendEmailMessageById(emailModel.EmailMessageId);

                responses.Add(emailSendResponse);
            }
            return responses;
        }

        public virtual TOutputModelType GetOutput()
        {
            return Output;
        }

        protected virtual void CreatePendingEmailIfApplicable()
        {
            if (EmailTemplateExists())
            {
                Output.PendingEmails = GetEmailPreviews();
            }
        }

        protected bool EmailTemplateExists()
        {
            return GetEmailTemplates().Any();
        }

        public abstract IList<EmailTemplateModel> GetEmailTemplates();

        protected virtual bool OverrideTemplates(IList<EmailTemplateModel> templates) 
        {
            return false;
        }

        public virtual IList<TEmailMessageModelType> GetEmailPreviews()
        {
            var templates = GetEmailTemplates();

            if (!templates.Any())
            {
                return new List<TEmailMessageModelType>();
            }

            OverrideTemplates(templates);

            var tokenDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (WizardModel.PublicNotes != null)
            {
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.ActionPublicNote),
                    (WizardModel.PublicNotes ?? String.Empty).Replace("\n", "<br />"));
            }

            GetEmailTokens(tokenDictionary);
            return CreateEmails(templates, tokenDictionary).ToList();
        }

        protected virtual void GetEmailTokens(Dictionary<string, string> tokenDictionary) { }

        protected virtual IEnumerable<TEmailMessageModelType> CreateEmails(IEnumerable<EmailTemplateModel> templates, Dictionary<string, string> tokenDictionary)
        {
            var emails = new List<TEmailMessageModelType>();
            foreach (var template in templates)
            {
                var baseEmail = CreateEmail(template, tokenDictionary);
                var templateEmails = GetEmails(template, baseEmail);
                emails.AddRange(templateEmails);
            }

            return emails;
        }

        protected abstract IEnumerable<TEmailMessageModelType> GetEmails(EmailTemplateModel template, TEmailMessageModelType baseEmail);
        protected abstract TEmailMessageModelType CreateEmail(EmailTemplateModel template, IDictionary<string, string> tokenDictionary);

        protected void SetAttachments(IEnumerable<TEmailMessageModelType> emails, AttachmentType attachmentType)
        {
            foreach (var emailMessageModel in emails)
            {
                emailMessageModel.Attachments = Attachments.Where(x => x.AttachmentType == attachmentType).ToList();
            }
        }


       
        public static TSystemActionType CreateSystemAction<TSystemActionType>
        (IReadOnlyDictionary<SystemActionTypeName, Type> dictionary,
            SystemActionTypeName actionType,
            TActionModelType actionModel,
            TWizardModelType wizardModel,
            TOutputModelType outputModel)
            where TSystemActionType : SystemAction<TActionModelType, TWizardModelType, TOutputModelType, TUpsertType, TEmailMessageModelType>
        {
            TSystemActionType action;

            if (dictionary.TryGetValue(actionType, out var actionClassType))
            {
                action = Activator.CreateInstance(actionClassType) as TSystemActionType;

                if (action == null)
                {
                    throw new Exception($"Failed to get an action instance for action {actionType}.");
                }

            }
            else
            {
                throw new Exception($"Unexpected {nameof(SystemActionTypeName)} value: {actionType}.");
            }

            action.ConfigureInstance(actionModel, wizardModel, outputModel);

            return action;
        }

        protected virtual void LogAction() { }

        private void Log()
        {
            try
            {
                LogAction();
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "An error occurred while attemping to log a workflow action.");
            }
        }
    }
}
