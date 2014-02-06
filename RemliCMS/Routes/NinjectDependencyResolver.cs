using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ninject;

namespace RemliCMS.Routes
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        public NinjectDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        private readonly IKernel _kernel;
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