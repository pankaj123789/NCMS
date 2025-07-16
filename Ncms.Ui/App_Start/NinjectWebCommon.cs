using System;
using System.Web;
using System.Web.Security;
using F1Solutions.Naati.Common.Contracts.Bl;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ncms.Bl;
using Ncms.Ui;
using Ncms.Ui.Ioc;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NinjectWebCommon), nameof(NinjectWebCommon.Start))]
[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(NinjectWebCommon), nameof(NinjectWebCommon.RegisterMembership))]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(NinjectWebCommon), nameof(NinjectWebCommon.Stop))]

namespace Ncms.Ui
{
    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();
        private static readonly Service serviceBindings = new Service();
        private static readonly Repository repositoryBindings = new Repository();
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

                Register(Kernel);
                ServiceLocator.Configure(new NinjectServiceLocator(Kernel));
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
            bootstrapper.Kernel.Inject(Roles.Provider);
        }
    }
}
