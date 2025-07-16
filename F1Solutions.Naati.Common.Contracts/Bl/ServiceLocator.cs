using System;

namespace F1Solutions.Naati.Common.Contracts.Bl
{
    public class ServiceLocator
    {
        private static IServiceLocator _mServiceLocator;

        public static void Configure(IServiceLocator serviceLocator)
        {
            _mServiceLocator = serviceLocator;
        }

        public static IServiceLocator GetInstance()
        {
            return _mServiceLocator;
        }

        public static T Resolve<T>()
        {
            return _mServiceLocator.Resolve<T>();
        }

        public static object GetService(Type type)
        {
            return _mServiceLocator.GetService(type);
        }
    }
}
