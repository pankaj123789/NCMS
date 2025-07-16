using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;

namespace MyNaati.Ui.Ioc
{
    public class NinjectDependecyResolver : IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectDependecyResolver(IKernel kernel)
        {
            _kernel = kernel;
        }
        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }
    }
}