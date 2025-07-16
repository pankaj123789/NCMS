using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl;
using Ncms.Bl;
using NSubstitute;

namespace Ncms.Test.Utils
{
    internal class ServiceLocatorTestHelper
    {
        public static ITestServiceLocator  GetInstance()
        {
            return new TestServiceLocator();
        }

        private class TestServiceLocator : ITestServiceLocator
        {
            private readonly IDictionary<Type, object> mServiceRepository;

            public TestServiceLocator()
            {
                mServiceRepository = new Dictionary<Type, object>();
            }
            public T Resolve<T>()
            {
                return (T)mServiceRepository[typeof(T)];
            }

            public T MockService<T>() where T : class
            {
                object service;
                if (!mServiceRepository.TryGetValue(typeof(T), out service))
                {
                    service = Substitute.For<T>();
                    mServiceRepository[typeof(T)] = service;
                }

                return (T)service;
            }

            public T MockService<T>(T service) where T : class
            {
                mServiceRepository[typeof(T)] = service;
                return (T)service;
            }

            public object GetService(Type serviceType)
            {
                return mServiceRepository[serviceType];
            }
        }
    }


    internal interface ITestServiceLocator : IServiceLocator
    {
        T MockService<T>() where T : class;
        T MockService<T>(T mock) where T : class;
    }
}
