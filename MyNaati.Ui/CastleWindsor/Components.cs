//using System.Configuration;
//using System.ServiceModel;
//using System.Web.Mvc;
//using AutoMapper;
//using Castle.Core;
//using Castle.MicroKernel.Registration;
//using Castle.Windsor;
//using F1Solutions.Naati.Common.Azure.SAS;
//using F1Solutions.Naati.Common.Bl.Security;
//using F1Solutions.Naati.Common.Contracts.Dal.Portal;
//using F1Solutions.Naati.Common.Contracts.Dal.Portal.Common;
//using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
//using F1Solutions.Naati.Common.Contracts.Security;
//using F1Solutions.Naati.Common.Dal;
//using F1Solutions.Naati.Common.Dal.Portal;
//using F1Solutions.Naati.Common.Dal.Portal.Repositories;
//using F1Solutions.Naati.Common.Dal.Portal.Repositories.PractitionersDirectory;
//using F1Solutions.NAATI.ePortal.SAMIntegration.Data;
//using F1Solutions.NAATI.ePortal.SAMIntegration.Data.Repositories.PractitionersDirectory;
//using MyNaati.Bl.Portal.AutoMapperProfiles;
//using MyNaati.Bl.Portal.Security;
//using MyNaati.Bl.Portal.SercurePay;
//using MyNaati.Ui.Common;
//using MyNaati.Ui.Controllers;
//using MyNaati.Ui.Controllers.API;
//using MyNaati.Ui.Helpers;
//using MyNaati.Ui.Models;
//using MyNaati.Ui.Reports.OnlineOrderDetails;
//using MyNaati.Ui.Reports.PDFTokenisation;

//namespace MyNaati.Ui.CastleWindsor
//{
//    public class Components
//    {
//        private const string SecretsSourceAppConfig = "AppConfig";
//        private const string SecretsSourceAzureKeyVault = "AzureKeyVault";
//        public const string WEB_CONFIG_CONNECTION_STRING = "ConnectionString";
//        public static void Register(IWindsorContainer container)
//        {
//            container.Register(Component.For<SystemValuesTranslator>());

//            RegisterRepositories(container);
//            RegisterServices(container);
//            RegisterLinqToSql(container);


//            container
//                .Register(AllTypes.FromAssembly(typeof(HomeController).Assembly)
//                    .BasedOn<Profile>()
//                    .Configure(c => c.LifeStyle.Is(LifestyleType.Singleton)));
            
//            container
//                .Register(AllTypes.FromAssembly(typeof(MembershipProfile).Assembly)
//                    .BasedOn<Profile>()
//                    .Configure(c => c.LifeStyle.Is(LifestyleType.Singleton)));

//            container
//                .Register(AllTypes.FromAssembly(typeof(HomeController).Assembly)
//                    .BasedOn<Controller>()
//                    .Configure(c => c.LifeStyle.Is(LifestyleType.Transient)));

//            container
//                .Register(AllTypes.FromAssembly(typeof(HomeController).Assembly)
//                    .BasedOn<BaseApiController>()
//                    .Configure(c => c.LifeStyle.Is(LifestyleType.Transient)));

//            //container.Register(Component.For<PublicController>().i);

//            container.Register(Component.For<IFormsAuthenticationService>().ImplementedBy<FormsAuthenticationService>().LifeStyle.Transient);

//            //container
//            //    .Register(AllTypes.FromThisAssembly()
//            //    .BasedOn(typeof(ClientBase<>))
//            //    .WithService.DefaultInterfaces()
//            //    .Configure(c => c
//            //        .LifeStyle.Is(LifestyleType.Transient)
//            //        .ExtendedProperties(new { ManageWcfSessions = true }))
//            //    );

//            container
//                .Register(Component.For<ILookupProvider>()
//                    .ImplementedBy<CachedLookupProvider>()
//                    .LifeStyle.Singleton);

//            container
//                .Register(Component.For<IOrderCsvGenerator>()
//                    .ImplementedBy<OrderCsvGenerator>());

//            container
//                .Register(Component.For<IUserCsvGenerator>()
//                    .ImplementedBy<UserCsvGenerator>());

//           // container
//            //    .Register(Component.For<PractitionerDataService>()
//            //        .ImplementedBy<PractitionerDataService>().LifeStyle.Transient);

//            //container
//            //    .Register(Component.For<LegacyPractitionerDataService>()
//            //        .ImplementedBy<LegacyPractitionerDataService>().LifeStyle.Transient);

//            container
//                .Register(Component.For<OnlineDirectorySearch>()
//                    .ImplementedBy<OnlineDirectorySearch>().LifeStyle.Transient);

//            container
//                .Register(Component.For<IRegistrationUserRequestCsvGenerator>()
//                    .ImplementedBy<RegistrationUserRequestCsvGenerator>());

//            container
//                .Register(Component.For<IPdfRenderer>().ImplementedBy<PdfRenderer>());

//            container.Register(Component.For<IRegisterHelper>().ImplementedBy<RegisterHelper>());
//            container.Register(Component.For<IExaminerHelper>().ImplementedBy<ExaminerHelper>());
//            container.Register(Component.For<IMenuHelper>().ImplementedBy<MenuHelper>().LifeStyle.Transient);


//            container.Register(Component.For(typeof(SessionStorage<,>)));

           
//            //  container.Register(Component.For<IValidator>().ImplementedBy<Validator>());

//            container.Register(Component.For<IPaymentClient>().ImplementedBy<PaymentClient>());

//            var dataSecurityProvider = new DataSecurityProvider(() => (NaatiWebUser)OperationContext.Current.IncomingMessageProperties[WcfHeaderKeys.CONTEXT_USER_KEY]);
//            container.Register(Component
//                .For<IDataSecurityProvider>()
//                .Instance(dataSecurityProvider));

//            container.Register(Component.For<PasswordGenerator>()
//                .ImplementedBy<PasswordGenerator>()
//                .LifeStyle.Singleton);

           

//        }

//        private static void RegisterLinqToSql(IWindsorContainer container)
//        {
//            var secretsProvider = DependencyResolver.Current.GetService<ISecretsProvider>();
//            var connectionString = secretsProvider.Get(WEB_CONFIG_CONNECTION_STRING);
//            container.Register(Component.For<PractitionerRepositoryDataContext>().UsingFactoryMethod(() => new PractitionerRepositoryDataContext(connectionString)).LifeStyle.Transient);
//            container.Register(Component.For<SamLinqRepositoryDataContext>().UsingFactoryMethod(() => new SamLinqRepositoryDataContext(connectionString)).LifeStyle.Transient);
//            container.Register(Component.For<SamKeyGenerationDataContext>().UsingFactoryMethod(() => new SamKeyGenerationDataContext(connectionString)).LifeStyle.Transient);
//        }

//        private static void RegisterRepositories(IWindsorContainer container)
//        {
//            // Default Repositories
//            container.Register(Component.For(typeof(IRepository<,>)).ImplementedBy(typeof(Repository<,>)).LifeStyle.Transient);

//            // Custom IRepositoriesp
//            container.Register(AllTypes.FromAssemblyContaining<IRegistrationRequestRepository>()
//                                   .BasedOn(typeof(IRepository<,>)).LifestyleTransient()
//                                   .WithService.DefaultInterfaces());

//            container.Register(Component.For<IPractitionerSearchRepository>().ImplementedBy<PractitionerSearchRepository>().LifeStyle.Transient);
//            container.Register(Component.For<IPractitionersRepository>().ImplementedBy<LinqPractitionerRepository>().LifeStyle.Transient);
//            container.Register(Component.For<IAccreditationResultRepository>().ImplementedBy<AccreditationResultRepository>().LifeStyle.Transient);

            

//        }



//        private static void RegisterServices(IWindsorContainer container)
//        {

//            container.Register(Component.For<IPersonQueryService>().ImplementedBy<PersonQueryService>().LifeStyle.Transient);
//            container.Register(Component.For<IInstitutionQueryService>().ImplementedBy<InstitutionQueryService>().LifeStyle.Transient);
//            container.Register(Component.For<IEmailMessageQueryService>().ImplementedBy<EmailMessageQueryService>().LifeStyle.Transient);
//            container.Register(Component.For<IApplicationQueryService>().ImplementedBy<ApplicationQueryService>().LifeStyle.Transient);
//            container.Register(Component.For<INoteQueryService>().ImplementedBy<NoteQueryService>().LifeStyle.Transient);
//            container.Register(Component.For<IExaminerQueryService>().ImplementedBy<ExaminerQueryService>().LifeStyle.Transient);
//            container.Register(Component.For<IContactPersonQueryService>().ImplementedBy<ContactPersonQueryService>().LifeStyle.Transient);
//            container.Register(Component.For<IFileStorageService>().ImplementedBy<FileSystemFileStorageService>().LifeStyle.Transient);
//            container.Register(Component.For<ITestQueryService>().ImplementedBy<TestQueryService>().LifeStyle.Transient);
//            container.Register(Component.For<ITestSpecificationQueryService>().ImplementedBy<TestSpecificationQueryService>().LifeStyle.Transient);
//            container.Register(Component.For<ITestSessionQueryService>().ImplementedBy<TestSessionQueryService>().LifeStyle.Transient);
//            container.Register(Component.For<ITestMaterialQueryService>().ImplementedBy<TestMaterialQueryService>().LifeStyle.Transient);
//            container.Register(Component.For<ITestResultQueryService>().ImplementedBy<TestResultQueryService>().LifeStyle.Transient);
//            container.Register(Component.For<IAuditLogQueryService>().ImplementedBy<AuditLogQueryService>().LifeStyle.Transient);
//            container.Register(Component.For<IPanelQueryService>().ImplementedBy<PanelQueryService>().LifeStyle.Transient);
//            container.Register(Component.For<ISystemQueryService>().ImplementedBy<SystemQueryService>().LifeStyle.Transient);
//            container.Register(Component.For<IEmailTemplateQueryService>().ImplementedBy<EmailTemplateQueryService>().LifeStyle.Transient);
//            container.Register(Component.For<IPayrollQueryService>().ImplementedBy<PayrollQueryService>().LifeStyle.Transient);
//            container.Register(Component.For<IExaminerToolsService>().ImplementedBy<ExaminerToolsService>().LifeStyle.Transient);
//            container.Register(Component.For<IMaterialRequestQueryService>().ImplementedBy<MaterialRequestQueryService>().LifeStyle.Transient);
//            container.Register(Component.For<IUserQueryService>().ImplementedBy<UserQueryService>().LifeStyle.Transient);
//            container.Register(Component.For<IPractitionerQueryService>().ImplementedBy<PractitionerQueryService>().LifeStyle.Transient);
//            //container.Register(Component.For<ISamCreditCardEncryptionService>().ImplementedBy<SamCreditCardEncryptionService>().LifeStyle.Transient);
//            var secretsSource = ConfigurationManager.AppSettings["secrets:Source"];
//            var useKeyVault = secretsSource == SecretsSourceAzureKeyVault;
//            container.Register(Component.For<ISecretsProvider>().ImplementedBy(useKeyVault ? typeof(F1Solutions.Naati.Common.Azure.KeyVault.SecretsProvider) : typeof(SecretsProvider)).LifeStyle.Transient);
//            container.Register(Component.For<ICertificateProvider>().ImplementedBy(useKeyVault ? typeof(F1Solutions.Naati.Common.Azure.KeyVault.CertificateProvider) : typeof(CertificateProvider)).LifeStyle.Transient);
//            container.Register(Component.For<ISharedAccessSignature>().ImplementedBy<SharedAccessSignature>().LifeStyle.Transient);
//        }
//    }
//}