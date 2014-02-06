using Ninject.Modules;
using System.Web.Mvc;

namespace RemliCMS.Routes
{
    public class BaseModule : NinjectModule 
    {
        public override void Load()
        {
            Bind<IController>().To<Controller>();
            Bind<IRouteService>().To<RouteService>();
        }
    }
}