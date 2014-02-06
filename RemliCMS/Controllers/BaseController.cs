using System.Web.Mvc;
using RemliCMS.Routes;

namespace RemliCMS.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IRouteService RouteService;

        public BaseController(IRouteService routeService)
        {
            RouteService = routeService;
        }

        protected RouteValues RouteValue
        {
            get { return RouteService.GetRouteValues(ControllerContext); }
        }

    }
}