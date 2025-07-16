using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Ninject;

namespace MyNaati.Ui.Ioc
{
    public class NinjectHttpControllerActivator : IHttpControllerActivator
    {
        private readonly IKernel _kernel;
        public NinjectHttpControllerActivator(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controller = (IHttpController)_kernel.Get(controllerType);

            request.RegisterForDispose(new Release(() => _kernel.Release(controller)));

            return controller;
        }

        private class Release : IDisposable
        {
            private readonly Action _release;

            public Release(Action release)
            {
                _release = release;
            }

            public void Dispose()
            {
                _release();
            }
        }
    }
}