using System.Web.Mvc;

namespace RemliCMS.Routes
{
    public interface IRouteService
    {
        RouteValues GetRouteValues(ControllerContext controllerContext);
    }
}