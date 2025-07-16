using System.Configuration;
using System.Reflection;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Azure.Storage;
using F1Solutions.Naati.Common.Bl.AddressParser;
using F1Solutions.Naati.Common.Bl.MaterialRequest;
using F1Solutions.Naati.Common.Bl.ProfessionalDevelopment;
using F1Solutions.Naati.Common.Bl.Security;
using F1Solutions.Naati.Common.Bl.Services;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.AddressParser;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using F1Solutions.Naati.Common.Contracts.Security;
using F1Solutions.Naati.Common.Dal;
using F1Solutions.Naati.Common.Migrations.Updater;
using Ncms.Bl;
using Ncms.Bl.BackgroundTasks;
using Ncms.Bl.TestSpecifications;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;
using Ninject.Modules;
using Ninject.Extensions.Conventions;
using IApplicationService = Ncms.Contracts.IApplicationService;
using IAuditService = Ncms.Contracts.IAuditService;
using IEmailTemplateService = Ncms.Contracts.IEmailTemplateService;
using IFileService = Ncms.Contracts.IFileService;
using ILanguageService = Ncms.Contracts.ILanguageService;
using IPersonService = Ncms.Contracts.IPersonService;
using ISecurityService = Ncms.Contracts.ISecurityService;
using ISystemService = Ncms.Contracts.ISystemService;
using ITestMaterialService = Ncms.Contracts.ITestMaterialService;
using IUserService = Ncms.Contracts.IUserService;
using IMaterialRequestWizardLogicService = Ncms.Contracts.IMaterialRequestWizardLogicService;
using IMaterialRequestService = Ncms.Contracts.IMaterialRequestService;
using F1Solutions.Naati.Common.Bl;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Dal.CacheQuery;
using Ninject.Extensions.Interception.Infrastructure.Language;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Bl.Refund;
using F1Solutions.Naati.Common.Contracts.Bl.Payment;
using F1Solutions.Naati.Common.Bl.Payment;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR;
//using F1Solutions.Naati.Common.Contracts.Bl.Message;
//using F1Solutions.Naati.Common.Bl.Message;
using F1Solutions.Naati.Common.Contracts.Bl.NotificationScheduler;
//using F1Solutions.Naati.Common.Contracts.Dal.Services.Televic;
//using F1Solutions.Naati.Common.Televic;
using F1Solutions.Naati.Common.Dal.Finance;
using F1Solutions.Naati.Common.Dal.Finance.PayPal;
using F1Solutions.Naati.Common.Dal.Finance.Wiise;
using F1Solutions.Naati.Common.Wiise;
using F1Solutions.Naati.Common.Dal.Portal.Repositories;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.NHibernate;
using F1Solutions.Naati.Common.Dal.Portal;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;
using F1Solutions.Naati.Common.Dal.Portal.CacheQuery;
using Ncms.Bl.Security;
using System.Web;
using F1Solutions.Naati.Common.Dal.FileDeletion;
using F1Solutions.Naati.Common.Bl.Refund;

namespace Ncms.Ui.Ioc
{
    public class Service : NinjectModule
    {
        private const string SecretsSourceAzureKeyVault = "AzureKeyVault";

        public override void Load()
        {
            // Business Services
            //Bind<Logic.MainService.Main>().ToMethod(ctx => (Logic.MainService.Main)ClientGlobal.GetService(ClientGlobal.WebServices.Main)).InThreadScope();
            Bind<IResourceService>().To<ResourceService>().InThreadScope().WithConstructorArgument(Assembly.GetAssembly(typeof(Naati.Resources.ResourceDummy)));

            Bind<IAutoMapperHelper>().To<AutoMapperHelper>().InSingletonScope();
            Bind<IUserService>().To<UserService>().InThreadScope();
            Bind<IPersonService>().To<PersonService>().InThreadScope();
            Bind<IMyNaatiIntegrationService>().To<MyNaatiIntegrationService>().InThreadScope().Intercept().With<QueryServiceInterceptor>();
            Bind<IApplicationService>().To<ApplicationService>().InThreadScope();
            Bind<IMaterialRequestWizardLogicService>().To<MaterialRequestWizardLogicService>().InThreadScope();
            Bind<IMaterialRequestService>().To<MaterialRequestService>().InThreadScope();
            Bind<IMaterialRequestPayRollHelper>().To<MaterialRequestPayRollHelper>().InThreadScope();
            Bind<IEntityService>().To<EntityService>().InThreadScope();
            Bind<IUserSearchService>().To<UserSearchService>().InThreadScope();
            Bind<ITestService>().To<TestService>().InThreadScope();
            Bind<ITestSessionService>().To<TestSessionService>().InThreadScope();
            Bind<ITestResultService>().To<TestResultService>().InThreadScope();
            Bind<ITestMaterialService>().To<TestMaterialService>().InThreadScope();
            Bind<IFileService>().To<FileService>().InThreadScope();
            Bind<IAssetService>().To<AssetService>().InThreadScope();
            //Bind<IDataSetService>().To<DataSetService>().InThreadScope();
            // Bind<IJobService>().To<JobService>().InThreadScope();
            Bind<IPanelService>().To<PanelService>().InThreadScope();
            Bind<IAuditService>().To<AuditService>().InThreadScope();
            Bind<ILanguageService>().To<LanguageService>().InThreadScope();
            Bind<INoteService>().To<NoteService>().InThreadScope();
            Bind<ISystemService>().To<SystemService>().InThreadScope();
            Bind<IPersonImageService>().To<PersonImageService>().InThreadScope();
            Bind<IExaminerService>().To<ExaminerService>().InThreadScope();
            //  Bind<ILetterService>().To<LetterService>().InThreadScope();
            Bind<ISecurityService>().To<SecurityService>().InThreadScope();
            Bind<IAddressService>().To<AddressService>().InThreadScope();
            Bind<IPayrollService>().To<PayrollService>().InThreadScope();
            Bind<IAccountingService>().To<AccountingService>().InThreadScope();
            Bind<IEmailMessageService>().To<EmailMessageService>().InThreadScope();
            Bind<ITokenReplacementService>().To<TokenReplacementService>().InThreadScope();
            Bind<IInstitutionService>().To<InstitutionService>().InThreadScope();
            Bind<IContactPersonService>().To<ContactPersonService>().InThreadScope();
            Bind<IApplicationWizardLogicService>().To<ApplicationWizardLogicService>().InThreadScope();
            Bind<ITestSpecificationService>().To<TestSpecificationService>().InThreadScope();
            Bind<IBackgroundTaskLogger>().To<BackgroundTaskLogger>().InThreadScope();
            Bind<IBackgroundTaskService<NcmsJobTypeName>>().To<NcmsBackgroundTaskService>().InThreadScope();
            Bind<IBackgroundTaskService<NcmsBackgoundOperationTypeName>>().To<NcmsBackgroundOperationService>().InThreadScope();
            Bind<IBackgroundOperationScheduler>().To<BackgroundOperationScheduler>().InThreadScope();
            Bind<INotificationScheduler>().To<BackgroundOperationScheduler>().InThreadScope();
            Bind<IUtilityBackgroundTask>().To<UtilityBackGroundTask>().InThreadScope();

            Bind<IEmailTemplateService>().To<EmailTemplateService>().InThreadScope();
            Bind<IApplicationBusinessLogicService>().To<ApplicationBusinessLogicService>().InThreadScope();
            Bind<ILogbookService>().To<LogbookService>().InThreadScope();
            Bind<IActivityPointsCalculatorService>().To<NcmsActivityPointsCalculatorHelper>().InThreadScope();
            Bind<ICredentialPointsCalculatorService>().To<NcmsCredentialPointsCalculatorHelper>().InThreadScope();
            Bind<ICredentialService>().To<CredentialService>().InThreadScope();
            Bind<IAddressParserHelper>().To<AddressParserHelper>().InThreadScope();
            Bind<IFileCompressionHelper>().To<FileCompressionHelper>().InThreadScope();
            Bind<ILifecycleService>().To<LifecycleService>().InThreadScope();
            Bind<INcmsPodsIntegrationService>().To<NcmsPodsIntegrationService>().InThreadScope();
            Bind<IPaymentClient>().To<PaymentClient>().InThreadScope();
            Bind<IPayPalService>().To<PayPalService>().InThreadScope();
            Bind<ITestSpecificationSpreadsheetService>().To<TestSpecificationSpreadsheetService>().InThreadScope();
            Bind<ICredentialPrerequisiteService>().To<CredentialPrerequisiteService>().InThreadScope();
            Bind<IFileDeletionDalService>().To<FileDeletionDalService>().InThreadScope();
            Bind<ICredentialPrerequisiteDalService>().To<CredentialPrerequisiteDalService>().InThreadScope();
            Bind<ICredentialApplicationRefundService>().To<CredentialApplicationRefundService>().InThreadScope();
            // CacheQueryServices

            Bind<INcmsUserCacheQueryService>().To<NcmsUserCacheQueryService>().InSingletonScope();
            Bind<INcmsUserPermissionQueryService>().To<NcmsUserPermissionCacheQueryService>().InSingletonScope();
            Bind<ICookieQueryService>().To<NcmsCookieCacheQueryService>().InSingletonScope();
            Bind<ILookupTypeConverterHelper>().To<LookupTypeCacheQueryService>().InSingletonScope();
            Bind<ISecretsCacheQueryService>().To<SecretsCacheQueryService>().InSingletonScope();
            Bind<INcmsUserRefreshCacheQueryService>().To<NcmsUserRefreshCacheQueryService>().InSingletonScope();

            // Finance
            //Bind<ITelevicTokenAuthorisationService>().To<TelevicAuthorizationService>().InThreadScope();
            //Bind<ITelevicIntegrationService>().To<TelevicIntegrationService>().InThreadScope();
            Bind<ISecurePayAuthorisationService>().To<SecurePayAuthorisationService>().InThreadScope();

            Bind<IWiiseIntegrationService>().To<WiiseIntegrationService>().InThreadScope();
            Bind<IWiiseTokenProvider>().To<WiiseIntegrationService>().InThreadScope();
            Bind<IWiiseAuthorisationService>().To<WiiseAuthorisationService>().InThreadScope();
            Bind<IWiiseAccountingApi>().To<WiiseAccountingApi>().InThreadScope().Intercept().With<WiiseAccountingApiInterceptor>(); ;

            // NCMS Services

            Kernel.Bind(
                x =>
                {
                    x.From(typeof(PersonQueryService).Assembly)
                        .Select(t => typeof(IQueryService).IsAssignableFrom(t))
                        .BindDefaultInterfaces()
                        .Configure(b => b.InThreadScope().Intercept().With<QueryServiceInterceptor>());
                });

            Bind<IDatabaseMigrationService>().To<DatabaseMigrationService>().InThreadScope();
            Kernel.Bind(
                x =>
                {
                    x.From(typeof(ApplicationProcessBackgroundTask).Assembly)
                        .Select(t => typeof(IBackgroundTask).IsAssignableFrom(t))
                        .BindDefaultInterfaces()
                        .Configure(b => b.InThreadScope());
                });

            var dataSecurityProvider = new DataSecurityProvider(() => HttpContext.Current?.User?.Identity?.Name);
            Bind<IDataSecurityProvider>().ToConstant(dataSecurityProvider).InSingletonScope();
            Bind<ISystemValuesCacheQueryService>().To<SystemValuesCacheQueryService>().InSingletonScope();
            Bind<ICustomSessionManager>().To<CustomSessionManager>().InThreadScope();
            Bind<ISystemValuesTranslator>().To<SystemValuesTranslator>().InSingletonScope();
            Bind<IPostcodesCacheQueryService>().To<PostcodesCacheQueryService>().InSingletonScope();

            Kernel.Bind(
                x =>
                {
                    x.From(typeof(AddressRepository).Assembly)
                        .Select(t => typeof(IRepository).IsAssignableFrom(t))
                        .BindDefaultInterfaces()
                        .Configure(b => b.InThreadScope());
                });

            // Common Service
            var secretsSource = ConfigurationManager.AppSettings["secrets:Source"];
            var useKeyVault = secretsSource == SecretsSourceAzureKeyVault;
            Bind<F1Solutions.Naati.Common.Contracts.Security.ISecretsProvider>().To(useKeyVault ? typeof(F1Solutions.Naati.Common.Azure.KeyVault.SecretsProvider) : typeof(SecretsProvider)).InThreadScope();
            Bind<ICertificateProvider>().To(useKeyVault ? typeof(F1Solutions.Naati.Common.Azure.KeyVault.CertificateProvider) : typeof(CertificateProvider)).InThreadScope();
            Bind<ISharedAccessSignature>().To(useKeyVault ? typeof(AzureFileStorageManager) : typeof(GenericFileStorageManager)).InThreadScope();
            Bind<IRefundCalculator>().To<NcmsRefundCalculator>().InThreadScope();
            //Bind<IMessenger>().To<Messenger>().InThreadScope();
            //// MyNAATI Services
            //Bind<IMembershipProviderService>().To<MembershipProviderService>().InThreadScope(); 
            GlobalHost.DependencyResolver.Register(typeof(IHubActivator), () => new HubActivator(Kernel));
        }
    }

}
