using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RemliCMS.Routes;
using RemliCMS.WebData.Entities;

namespace RemliCMS.Controllers
{
    public class SharedController : BaseController
    {
        public SharedController(IRouteService routeService) : base(routeService)
        {
        }

        //
        // GET: /Shared/Pong

        public ActionResult Pong()
        {
            return View();
        }

        //
        // GET: /Shared/Error
        public ActionResult Error(int errorCode=400)
        {

            ViewBag.ErrorCode = errorCode;
            Response.StatusCode = errorCode;

            return View();
        }

        //
        // GET: /Shared/AdminHeader
        [ChildActionOnly]
        public ActionResult AdminHeader(string currentItem = "none")
        {
            if (currentItem != "none")
            {
                ViewBag.CurrentItem = currentItem;
            }
            return PartialView();
        }

        //
        // GET: /Shared/RouteDebug
        [ChildActionOnly]
        public ActionResult RouteDebug()
        {
            RouteValues routeValues = RouteValue;

            var mongoConfig = new MongoDbConfig
            {
                DbLocation = System.Configuration.ConfigurationManager.AppSettings["MongoDbLocation"],
                DbName = System.Configuration.ConfigurationManager.AppSettings["MongoDbName"]
            };

            ViewBag.Debug = true;
            ViewBag.Translation = routeValues.Translation;
            ViewBag.Controller = routeValues.Controller;
            ViewBag.Action = routeValues.Action;
            ViewBag.Permalink = routeValues.Permalink;
            ViewBag.DbLocation = mongoConfig.DbLocation;
            ViewBag.DbName = mongoConfig.DbName;


            return PartialView();
        }
    }
}
