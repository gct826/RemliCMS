using System.Web.Mvc;

namespace RemliCMS.Routes
{
    public class RouteBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return new RouteValues
                       {
                           Translation = controllerContext.RouteData.Values["translation"].ToString(),
                           Controller = controllerContext.RouteData.Values["controller"].ToString(),
                           Action = controllerContext.RouteData.Values["action"].ToString(),
                           Permalink = controllerContext.RouteData.Values["permalink"].ToString()
                       };
        }
    }
}