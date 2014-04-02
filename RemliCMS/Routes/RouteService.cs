using System;
using System.Net.Mime;
using System.Web.Mvc;
//using RemliCMS.WebData.Services;

namespace RemliCMS.Routes
{
    public class RouteService : IRouteService
    {

        public RouteValues GetRouteValues(ControllerContext controllerContext)
        {
            //var translation = new TranslationService();

            var translation = "not found";
            try
            {
                translation = controllerContext.RouteData.Values["translation"].ToString();
            }
            catch
            {
            }

            var controller = "not found";
            try
            {
                controller = controllerContext.RouteData.Values["controller"].ToString();
            }
            catch
            {
            }
            
            var action = "not found";
            try
            {
                action = controllerContext.RouteData.Values["action"].ToString();
            }
            catch
            {
            }

            var permalink = "not found";
            
            //try
            //{
            //    permalink = controllerContext.RouteData.Values["permalink"].ToString();
            //}
            //catch
            //{
            //}


            return new RouteValues
                {
                    Translation = translation,
                    Controller = controller,
                    Action = action,
                    //Permalink = permalink
                };

        }

        //public string GetTranslation(ControllerContext controllerContext)
        //{
        //    var translation = "not found";
        //    try
        //    {
        //        translation = controllerContext.RouteData.Values["translation"].ToString();
        //    }
        //    catch
        //    {
        //    }
        //    return translation;
            
        //}


    }
}