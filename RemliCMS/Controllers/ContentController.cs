using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RemliCMS.Routes;
using RemliCMS.WebData.Entities;
using RemliCMS.WebData.Services;

namespace RemliCMS.Controllers
{
    public class ContentController : BaseController
    {
        public ContentController(IRouteService routeService) : base(routeService)
        {
        }

        //
        // GET: /Content/Permalink

        public ActionResult Index(string permalink)
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



            ViewBag.LangDir = "ltr";
            ViewBag.LangCode = "en";

            return View();
        }

        //
        // GET: /Content/RouteDebug
        [ChildActionOnly]
        public ActionResult RouteDebug()
        {
            return View();
        }
    }
}
