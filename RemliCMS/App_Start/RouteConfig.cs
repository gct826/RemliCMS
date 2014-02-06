using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RemliCMS
{
    public class RouteConfig
    {
        public class CmsUrlConstraint : IRouteConstraint
        {
            public bool Match(HttpContextBase httpContext, Route route, string parameterName,
                              RouteValueDictionary values, RouteDirection routeDirection)
            {
                //var pageHeaderService = new PageHeaderService();

                //var permalink = values[parameterName].ToString().ToLower();

                //bool foundLink;

                //try
                //{
                //    foundLink = pageHeaderService.IsExistPermalink(permalink);
                //}
                //catch
                //{
                //    foundLink = false;
                //}

                return true;
            }
        }

        public class TranslationConstraint : IRouteConstraint
        {
            public bool Match(HttpContextBase httpContext, Route route, string parameterName,
                              RouteValueDictionary values, RouteDirection routeDirection)
            {
                //var translationService = new TranslationService();
                //var code = values[parameterName].ToString().ToLower();
                //var foundTranslation = translationService.IsExistCode(code);

                //if (code == "admin")
                //{
                //    foundTranslation = true;
                //}


                return true;
            }
        }


        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Ping",
                url: "ping",
                defaults: new { controller = "Shared", action = "Pong" }
            );


            routes.MapRoute(
                name: "Error",
                url: "Error/{errorCode}",
                defaults: new { controller = "Shared", action = "Error", errorCode = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "CMCSRoute",
                url: "{translation}/{controller}/{permalink}",
                defaults: new { translation = "en", controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { translation = new TranslationConstraint(), permalink = new CmsUrlConstraint() }
            );
            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );




        }
    }
}