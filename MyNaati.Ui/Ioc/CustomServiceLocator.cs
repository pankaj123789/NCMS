using System;
using F1Solutions.Naati.Common.Contracts.Bl;
using Ninject;


namespace MyNaati.Ui.Ioc
{
    public class CustomServiceLocator : IServiceLocator
    {
        private readonly IKernel _kernel;

        public CustomServiceLocator(IKernel kernel)
        {
            _kernel = kernel;
        }
        public T Resolve<T>()
        {
            return _kernel.Get<T>();
        }

        public object GetService(Type serviceType)
        {
            return _kernel.GetService(serviceType);
        }
    }
}