using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RemliCMS.WebData.Entities;
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
                return pageHeaderService.IsExistPermalink(permalink);

                //var url = values["translation"].ToString().ToLower();
                //return pageHeaderService.IsActivePermalink(permalink, url);
            }
        }

        public class TranslationConstraint : IRouteConstraint
        {
            public bool Match(HttpContextBase httpContext, Route route, string parameterName,
                              RouteValueDictionary values, RouteDirection routeDirection)
            {
                
                var translationService = new TranslationService();
                var url = values[parameterName].ToString().ToLower();
                
                if (url == "admin")
                {
                    return true;
                }

                var foundTranslation = translationService.IsActiveUrl(url);

                return foundTranslation;
            }
        }

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
                    action = "Page",
                    permalink = pageHeaderService.GetDefaultPermalink()
                }
            );

            routes.MapRoute(
                name: "Ping",
                url: "ping",
                defaults: new { controller = "Shared", action = "Pong" }
            );

            //routes.MapRoute(
            //    name: "PayPal",
            //    url: "paypal",
            //    defaults: new {controller = "Paypal", action = "Index"}
            //    );

            routes.MapRoute(
                name: "AdminIndex",
                url: "admin",
                defaults: new { translation = "admin", controller = "Admin", action = "Index" }
            );


            routes.MapRoute(
                name: "Error",
                url: "error/{errorCode}",
                defaults: new { controller = "Shared", action = "Error", errorCode = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Permalink",
                url: "{translation}/{permalink}",
                defaults: new
                {
                    translation = translationService.GetDefaultUrl(),
                    controller = "Home",
                    action = "Page",
                    permalink = pageHeaderService.GetDefaultPermalink()
                },
                constraints: new { translation = new TranslationConstraint(), permalink = new CmsUrlConstraint() }
            );

            routes.MapRoute(
                name: "Default",
                url: "{translation}/{controller}/{action}/{id}",
                defaults: new { translation = translationService.GetDefaultUrl(), controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


            //routes.MapRoute(
            //    name: "Admin",
            //    url: "admin/{controller}/{action}/{id}",
            //    defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
            //);

            routes.MapRoute(
                name: "Registration",
                url: "{translation}/register/{action}/{id}",
                defaults: new {
                        translation = translationService.GetDefaultUrl(),
                        controller = "Register", 
                        action = "Index",
                        id = UrlParameter.Optional
                    }
            );


            
            //routes.MapRoute(
            //    name: "Default",
            //    url: "admin/{controller}/{action}/{id}",
            //    defaults: new { controller = "PageHeader", action = "Index", id = UrlParameter.Optional }
            //);




        }
    }
}