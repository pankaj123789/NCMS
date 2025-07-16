using System;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using F1Solutions.Naati.Common.Contracts.Bl;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using MyNaati.Ui.Ioc;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NinjectWebCommon), nameof(NinjectWebCommon.Start))]
[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(NinjectWebCommon), nameof(NinjectWebCommon.RegisterMembership))]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(NinjectWebCommon), nameof(NinjectWebCommon.Stop))]

namespace MyNaati.Ui.Ioc
{
    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();
        private static readonly Service serviceBindings = new Service();
     
        public static IKernel Kernel;

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            Kernel = new StandardKernel();
            try
            {
                Kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                Kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                // ServiceLocator.Configure(new CustomServiceLocator(Kernel));
                var dependencyResolver = new NinjectDependecyResolver(Kernel);

                DependencyResolver.SetResolver(dependencyResolver);
                GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new NinjectHttpControllerActivator(Kernel));
                ServiceLocator.Configure(new CustomServiceLocator(Kernel));
                Register(Kernel);
              
                return Kernel;
            }
            catch
            {
                Kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void Register(IKernel kernel)
        {
            //kernel.Load(repositoryBindings);
            kernel.Load(serviceBindings);
        }

        public static void RegisterMembership()
        {
            //TODO: REQUIRED?
           // bootstrapper.Kernel.Inject(Roles.Provider);
        }
    }
}
