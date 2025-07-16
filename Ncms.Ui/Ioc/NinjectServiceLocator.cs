using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using F1Solutions.Naati.Common.Contracts.Bl;
using Ninject;

namespace Ncms.Ui.Ioc
{
    internal class NinjectServiceLocator : IServiceLocator
    {
        private readonly IKernel mNinjectKernel;

        public NinjectServiceLocator(IKernel kernel)
        {
            mNinjectKernel = kernel;
        }

        public object GetService(Type serviceType)
        {
            return mNinjectKernel.GetService(serviceType);
        }

        /// <summary>
        /// NOTE: use constructor injection whenever possible. Use ServiceLocator as a last resort.
        /// </summary>
        public T Resolve<T>()
        {
            return mNinjectKernel.Get<T>();
        }
    }
}
