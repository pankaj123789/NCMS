//using System;
//using System.Collections.Generic;
//using Castle.Windsor;
//using Microsoft.Practices.ServiceLocation;

//namespace NAATI.WebService.NHibernate
//{
//    public class InversionOfControl
//    {
//        public static IWindsorContainer ConfigureForWeb()
//        {
//            IWindsorContainer container = new WindsorContainer();

//            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));

//            return container;
//        }
//    }

//    /// <summary>
//    /// Adapts the behavior of the Windsor container to the common IServiceLocator.
//    /// Taken from http://code.google.com/p/sutekishop/source/browse/trunk/Suteki.Shop/Suteki.Common/Windsor/WindsorServiceLocator.cs
//    ///  </summary>
//    public class WindsorServiceLocator : ServiceLocatorImplBase
//    {
//        private readonly IWindsorContainer mContainer;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="WindsorServiceLocator"/> class.
//        /// </summary>
//        /// <param name="container">The container.</param>
//        public WindsorServiceLocator(IWindsorContainer container)
//        {
//            mContainer = container;
//        }

//        /// <summary>
//        ///             When implemented by inheriting classes, this method will do the actual work of resolving
//        ///             the requested service instance.
//        /// </summary>
//        /// <param name="serviceType">Type of instance requested.</param>
//        /// <param name="key">Name of registered service you want. May be null.</param>
//        /// <returns>
//        /// The requested service instance.
//        /// </returns>
//        protected override object DoGetInstance(Type serviceType, string key)
//        {
//            if (key != null)
//                return mContainer.Resolve(key, serviceType);
//            return mContainer.Resolve(serviceType);
//        }

//        /// <summary>
//        ///             When implemented by inheriting classes, this method will do the actual work of
//        ///             resolving all the requested service instances.
//        /// </summary>
//        /// <param name="serviceType">Type of service requested.</param>
//        /// <returns>
//        /// Sequence of service instance objects.
//        /// </returns>
//        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
//        {
//            return (object[])mContainer.ResolveAll(serviceType);
//        }
//    }
//}