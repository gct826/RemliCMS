using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using RemliCMS.Routes;
using RemliCMS.WebData.Entities;
using RemliCMS.WebData.Services;

namespace RemliCMS.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IRouteService routeService) : base(routeService)
        {
        }

        //
        // GET: /Home/Permalink

        public ActionResult Index(string permalink)
        {
            RouteValues routeValues = RouteValue;

            if (System.Configuration.ConfigurationManager.AppSettings["Mode"] == "debug")
            {
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
            }


            var translationService = new TranslationService();

            ViewBag.TranslationDefault = translationService.GetDefaultUrl();

            var pageHeaderService = new PageHeaderService();

            ViewBag.PermalinkDefault = pageHeaderService.GetDefaultPermalink();

            var translation = translationService.Details(routeValues.Translation);
            
            if (translation == null || translation.IsActive == false)
            {
                return RedirectToAction("Error", "Shared", new {errorCode = 404});
            }

            if (translation.IsRtl)
            {
                ViewBag.LangDir = "rtl";
            }
            else
            {
                ViewBag.LangDir = "ltr";
            }
            ViewBag.LangCode = translation.Code;

            return View();
        }

        //
        // GET: /Home/RouteDebug
        [ChildActionOnly]
        public ActionResult RouteDebug()
        {
            return PartialView();
        }

        // GET: /Home/PageContent?pageindexId&translationId
        [ChildActionOnly]
        public ActionResult PageContent(string pageIndexId, string translationId)
        {
            var pageIndexService = new PageIndexService();

            var pageIndexObjectId = new ObjectId(pageIndexId);
            var translationObjectId = new ObjectId(translationId);

            ViewBag.Content = pageIndexService.GetContentString(pageIndexObjectId, translationObjectId);

            return PartialView();
        }

    }
}
