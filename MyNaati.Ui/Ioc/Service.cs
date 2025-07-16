
using System.Configuration;
using System.Web;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Azure.Storage;
using F1Solutions.Naati.Common.Bl;
using F1Solutions.Naati.Common.Bl.AddressParser;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Bl.Payment;
using F1Solutions.Naati.Common.Bl.ProfessionalDevelopment;
using F1Solutions.Naati.Common.Bl.Refund;
using F1Solutions.Naati.Common.Bl.Security;
using F1Solutions.Naati.Common.Bl.Services;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.AddressParser;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.Payment;
using F1Solutions.Naati.Common.Contracts.Bl.Refund;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using F1Solutions.Naati.Common.Contracts.Security;
using F1Solutions.Naati.Common.Dal;
using F1Solutions.Naati.Common.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Dal.CacheQuery;
using F1Solutions.Naati.Common.Dal.NHibernate;
using F1Solutions.Naati.Common.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Portal.CacheQuery;
using F1Solutions.Naati.Common.Dal.Portal.Repositories;
//using F1Solutions.Naati.Common.Dal.Portal.Repositories.PractitionersDirectory;
//using F1Solutions.NAATI.ePortal.SAMIntegration.Data;
//using F1Solutions.NAATI.ePortal.SAMIntegration.Data.Repositories.PractitionersDirectory;
using MyNaati.Bl;
using MyNaati.Bl.BackgroundTasks;
using MyNaati.Bl.BackOffice;
//using MyNaati.Bl.Portal.AutoMapperProfiles;
using MyNaati.Bl.Portal.Security;
using MyNaati.Contracts;
using MyNaati.Contracts.BackgroundTask;

//using MyNaati.Contracts.BackOffice.PortalUser;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;
using MyNaati.Ui.Controllers;
using MyNaati.Ui.Helpers;
using MyNaati.Ui.Models;
using Ninject.Modules;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.Interception.Infrastructure.Language;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using MyNaati.Bl.AutoMappingProfiles;
using F1Solutions.Naati.Common.Dal.Finance;
using F1Solutions.Naati.Common.Dal.Finance.PayPal;
using F1Solutions.Naati.Common.Dal.Finance.Wiise;
using F1Solutions.Naati.Common.Wiise;
using MyNaati.Contracts.BackOffice;
using F1Solutions.Naati.Common.Contracts.Bl.Credentials;
using F1Solutions.Naati.Common.Bl.Credentials;

namespace MyNaati.Ui.Ioc
{
    public class Service : NinjectModule
    {
        private const string SecretsSourceAppConfig = "AppConfig";
        private const string SecretsSourceAzureKeyVault = "AzureKeyVault";
        public const string WEB_CONFIG_CONNECTION_STRING = "ConnectionString";
        public override void Load()
        {
            Bind<IAutoMapperHelper>().To<AutoMapperHelper>().InSingletonScope();
            RegisterCacheQueryServices();
            RegisterCloudServices();
            RegisterQueryServices();
            RegisterBackgroundTasks();
            Register();
        }

        private void RegisterServices()
        {
            Kernel.Bind(
                x =>
                {
                    x.From(typeof(PersonalDetailsService).Assembly)
                        .Select(t => typeof(IInterceptableservice).IsAssignableFrom(t))
                        .BindDefaultInterfaces()
                        .Configure(b => b.InThreadScope().Intercept().With<ServiceInterceptor>());
                });

            Bind<ICustomSessionManager>().To<CustomSessionManager>().InThreadScope();
            Bind<IAddressParserHelper>().To<AddressParserHelper>().InThreadScope();
            Bind<IActivityPointsCalculatorService>().To<MyNaatiActivityPointsCalculatorHelper>().InThreadScope();
            Bind<ICredentialPointsCalculatorService>().To<MyNaatiCredentialPointsCalculatorHelper>().InThreadScope();
            Bind<IApplicationBusinessLogicService>().To<ApplicationBusinessLogicService>().InThreadScope();
            Bind<IApplicationFormsService>().To<ApplicationFormHelper>().InThreadScope();
            Bind<ITokenReplacementService>().To<TokenReplacementService>().InThreadScope();
            Bind<IRefundCalculator>().To<RefundCalculator>().InThreadScope();
            Bind<ISecurePayAuthorisationService>().To<SecurePayAuthorisationService>().InThreadScope();
            Bind<IPayPalService>().To<PayPalService>().InThreadScope();

            Bind<IWiiseIntegrationService>().To<WiiseIntegrationService>().InThreadScope();
            Bind<IWiiseTokenProvider>().To<WiiseIntegrationService>().InThreadScope();
            Bind<IWiiseAuthorisationService>().To<WiiseAuthorisationService>().InThreadScope();
            Bind<IWiiseAccountingApi>().To<WiiseAccountingApi>().InThreadScope();

            Bind<ICredentialQrCodeService>().To<CredentialQrCodeService>().InThreadScope();
            Bind<ICredentialApplicationRefundService>().To<CredentialApplicationRefundService>().InThreadScope();

            Bind<IUnraisedInvoiceService>().To<UnraisedInvoiceService>().InThreadScope();
        }

        public void Register()
        {
            
            Kernel.Bind(
                x =>
                {
                    x.From(typeof(HomeController).Assembly, typeof(MembershipProfile).Assembly)
                        .Select(t => typeof(Profile).IsAssignableFrom(t))
                        .BindToSelf()
                        .Configure(b => b.InSingletonScope());
                });
            //Kernel.Bind(
            //    x =>
            //    {
            //        x.From(typeof(HomeController).Assembly)
            //            .Select(t => typeof(Controller).IsAssignableFrom(t))
            //            .BindToSelf()
            //            .Configure(b => b.InTransientScope());
            //    });

            //Kernel.Bind(
            //    x =>
            //    {
            //        x.From(typeof(HomeController).Assembly)
            //            .Select(t => typeof(BaseApiController).IsAssignableFrom(t))
            //            .BindToSelf()
            //            .Configure(b => b.InTransientScope());
            //    });

            RegisterRepositories();
            RegisterServices();
            RegisterLinqToSql();

            Bind<IFormsAuthenticationService>().To<FormsAuthenticationService>().InThreadScope();
            Bind<ILookupProvider>().To<CachedLookupProvider>().InSingletonScope();
           // Bind<IOrderCsvGenerator>().To<OrderCsvGenerator>().InThreadScope();
           // Bind<IUserCsvGenerator>().To<UserCsvGenerator>().InSingletonScope();
            Bind<OnlineDirectorySearch>().ToSelf().InThreadScope();
            //Bind<IRegistrationUserRequestCsvGenerator>().To<RegistrationUserRequestCsvGenerator>().InSingletonScope();
           // Bind<IPdfRenderer>().To<PdfRenderer>().InSingletonScope();
            Bind<IRegisterHelper>().To<RegisterHelper>().InSingletonScope();
            Bind<IExaminerHelper>().To<ExaminerHelper>().InSingletonScope();
            Bind<IRecaptchaValidatorHelper>().To<RecaptchaValidatorHelper>().InSingletonScope();
            Bind<IMenuHelper>().To<MenuHelper>().InThreadScope();
            Bind<ILifecycleService>().To<LifecycleService>().InThreadScope();
            Bind<IMyNaatiPodsIntegrationService>().To<MyNaatiPodsIntegrationService>().InThreadScope();
            //Bind(typeof(SessionStorage<,>)
            Bind<IPaymentClient>().To<PaymentClient>().InThreadScope();
            var dataSecurityProvider = new DataSecurityProvider(() => HttpContext.Current?.User?.Identity?.Name);
            Bind<IDataSecurityProvider>().ToConstant(dataSecurityProvider).InSingletonScope();
            Bind<PasswordGenerator>().ToSelf().InSingletonScope();
            Bind<IFileCompressionHelper>().To<FileCompressionHelper>().InThreadScope();

        }

        private void RegisterBackgroundTasks()
        {
            Bind<IBackgroundTaskLogger>().To<BackgroundTaskLogger>().InThreadScope();
            Bind<IBackgroundTaskService<MyNaatiJobTypeName>>().To<MyNaatiBackgroundTaskService>().InThreadScope();
            Kernel.Bind(
                x =>
                {
                    x.From(typeof(MyNaatiRefreshPendingUsersTask).Assembly)
                        .Select(t => typeof(IBackgroundTask).IsAssignableFrom(t))
                        .BindDefaultInterfaces()
                        .Configure(b => b.InThreadScope());
                });
        }

        private void RegisterLinqToSql()
        {
           // var secretsProvider = DependencyResolver.Current.GetService<ISecretsCacheQueryService>();
           // var connectionString = secretsProvider.Get(WEB_CONFIG_CONNECTION_STRING);
            //Bind<PractitionerRepositoryDataContext>().ToMethod(ctx => new PractitionerRepositoryDataContext(connectionString)).InThreadScope();
           // Bind<SamLinqRepositoryDataContext>().ToMethod(ctx => new SamLinqRepositoryDataContext(connectionString)).InThreadScope();
           // Bind<SamKeyGenerationDataContext>().ToMethod(ctx => new SamKeyGenerationDataContext(connectionString)).InThreadScope();
        }

        private void RegisterRepositories()
        {
            Kernel.Bind(
                x =>
                {
                    x.From(typeof(AddressRepository).Assembly)
                        .Select(t => typeof(IRepository).IsAssignableFrom(t))
                        .BindDefaultInterfaces()
                        .Configure(b => b.InThreadScope());
                });

           // Bind<IPractitionersRepository>().To<LinqPractitionerRepository>().InThreadScope();
           // Bind<IPractitionerSearchRepository>().To<PractitionerSearchRepository>().InThreadScope();
        }

        private void RegisterCacheQueryServices()
        {
            // CacheQueryServices

            Bind<INcmsUserPermissionQueryService>().To<NcmsUserPermissionCacheQueryService>().InSingletonScope();
            Bind<INcmsUserCacheQueryService>().To<NcmsUserCacheQueryService>().InSingletonScope();
            Bind<ICountriesCacheQueryService>().To<CountriesCacheQueryService>().InSingletonScope();
            Bind<ICredentialLanguage1CacheQueryService>().To<CredentialLanguage1CacheQueryService>().InSingletonScope();
            Bind<ICredentialLanguage2CacheQueryService>().To<CredentialLanguage2CacheQueryService>().InSingletonScope();
            Bind<ILanguagesCacheQueryService>().To<LanguagesCacheQueryService>().InSingletonScope();
            Bind<IOdAddressVisibilityTypesCacheQueryService>().To<OdAddressVisibilityTypesCacheQueryService>().InSingletonScope();
            Bind<IPersonTitlesCacheQueryService>().To<PersonTitlesCacheQueryService>().InSingletonScope();
            Bind<IPostcodesCacheQueryService>().To<PostcodesCacheQueryService>().InSingletonScope();
            Bind<ISystemValuesCacheQueryService>().To<SystemValuesCacheQueryService>().InSingletonScope();
            Bind<ITestLocationsCacheQueryService>().To<TestLocationsCacheQueryService>().InSingletonScope();
            Bind<IDisplayBillsCacheQueryService>().To<DisplayBillsCacheQueryService>().InSingletonScope();
            Bind<IDisplayRolePlayerCacheQueryService>().To<DisplayRolePlayerCacheQueryService>().InSingletonScope();
            Bind<ICookieQueryService>().To<MyNaatiCookieCacheQueryService>().InSingletonScope();
            Bind<ISystemValuesTranslator>().To<SystemValuesTranslator>().InSingletonScope();
            Bind<ISecretsCacheQueryService>().To<SecretsCacheQueryService>().InSingletonScope();
            Bind<IMyNaatiUserRefreshCacheQueryService>().To<MyNaatiUserRefreshCacheQueryService>().InSingletonScope();
        }

        private void RegisterQueryServices()
        {
            Kernel.Bind(
                x =>
                {
                    x.From(typeof(PersonQueryService).Assembly)
                        .Select(t => typeof(IQueryService).IsAssignableFrom(t))
                        .BindDefaultInterfaces()
                        .Configure(b => b.InThreadScope().Intercept().With<QueryServiceInterceptor>());
                });

           

        }

        private void RegisterCloudServices()
        {
            var secretsSource = ConfigurationManager.AppSettings["secrets:Source"];
            var useKeyVault = secretsSource == SecretsSourceAzureKeyVault;

            if (useKeyVault)
            {
                Bind<F1Solutions.Naati.Common.Contracts.Security.ISecretsProvider>().To<F1Solutions.Naati.Common.Azure.KeyVault.SecretsProvider>().InThreadScope();
                Bind<ICertificateProvider>().To<F1Solutions.Naati.Common.Azure.KeyVault.CertificateProvider>().InThreadScope();
                Bind<ISharedAccessSignature>().To<AzureFileStorageManager>().InThreadScope();
            }
            else
            {
                Bind<F1Solutions.Naati.Common.Contracts.Security.ISecretsProvider>().To<SecretsProvider>().InThreadScope();
                Bind<ICertificateProvider>().To<CertificateProvider>().InThreadScope();
                
                Bind<ISharedAccessSignature>().To<GenericFileStorageManager>().InTransientScope();
            }
        }
    }
}