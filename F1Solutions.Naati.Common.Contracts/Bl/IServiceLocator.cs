using System;

namespace F1Solutions.Naati.Common.Contracts.Bl
{
    public interface IServiceLocator
    {
        T Resolve<T>();
        object GetService(Type serviceType);
    }
}
