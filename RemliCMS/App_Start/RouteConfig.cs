using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RemliCMS.WebData.Services;

namespace RemliCMS
{
    public class RouteConfig
    {
        public class CmsUrlConstraint : IRouteConstraint
        {
            public bool Match(HttpContextBase httpContext, Route route, string parameterName,
                              RouteValueDictionary values, RouteDirection routeDirection)
            {
                var pageHeaderService = new PageHeaderService();

                var permalink = values[parameterName].ToString().ToLower();

                bool foundLink;

                try
                {
                    foundLink = pageHeaderService.IsExistPermalink(permalink);
                }
                catch
                {
                    foundLink = false;
                }

                return true;
            }
        }

        public class TranslationConstraint : IRouteConstraint
        {
            public bool Match(HttpContextBase httpContext, Route route, string parameterName,
                              RouteValueDictionary values, RouteDirection routeDirection)
            {
                var translationService = new TranslationService();
                var url = values[parameterName].ToString().ToLower();
                var foundTranslation = translationService.IsActiveUrl(url);

                return foundTranslation;
            }
        }

        //public class TranslationDefault
        //{
        //    public string Default()
        //    {
        //        var translationService = new TranslationService();
        //        return translationService.GetDefaultUrl();
        //    }
        //}


        public static void RegisterRoutes(RouteCollection routes)
        {
            var translationService = new TranslationService();
            var pageHeaderService = new PageHeaderService();

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Index",
                url: "",
                defaults: new {
                    translation = translationService.GetDefaultUrl(), 
                    controller = "Home", 
                    action = "Index",
                    permalink = pageHeaderService.GetDefaultPermalink()
                }
            );

            routes.MapRoute(
                name: "Ping",
                url: "ping",
                defaults: new { controller = "Shared", action = "Pong" }
            );


            routes.MapRoute(
                name: "Error",
                url: "error/{errorCode}",
                defaults: new { controller = "Shared", action = "Error", errorCode = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Admin",
                url: "admin/{controller}/{action}/{id}",
                defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{translation}/{permalink}",
                defaults: new { translation = translationService.GetDefaultUrl(),
                    controller = "Home", 
                    action = "Index",
                    permalink = pageHeaderService.GetDefaultPermalink()
                },
                constraints: new { translation = new TranslationConstraint(), permalink = new CmsUrlConstraint() }
            );
            
            //routes.MapRoute(
            //    name: "Default",
            //    url: "admin/{controller}/{action}/{id}",
            //    defaults: new { controller = "PageHeader", action = "Index", id = UrlParameter.Optional }
            //);




        }
    }
}