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
        // GET: /Home/Page?permalink
        public ActionResult Page(string permalink)
        {
            RouteValues routeValues = RouteValue;

            //System debug mode
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

            var pageHeaderService = new PageHeaderService();
            var translationService = new TranslationService();

            var pageHeader = pageHeaderService.Details(permalink);
            var translation = translationService.Details(routeValues.Translation);

            if (pageHeader == null)
            {
                return RedirectToAction("Error", "Shared", new { errorCode = 404 });
            }

            var pageTitle = pageHeaderService.ReturnPageTitle(pageHeader.Id, translation.Id);

            if (pageTitle == null || pageTitle.IsActive == false)
            {
                return RedirectToAction("Error", "Shared", new { errorCode = 404 });
            }

            ViewBag.Title = pageTitle.Title;
            ViewBag.TranslationId = translation.Id;

            var pageIndexService = new PageIndexService();
            var pageIndexList = pageIndexService.ListIndexs(pageHeader.Id);

            return View(pageIndexList);
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
        public ActionResult PageContent(string pageIndexId, string translationId, bool isAdmin = false)
        {
            var pageIndexService = new PageIndexService();

            var pageIndexObjectId = new ObjectId(pageIndexId);
            var translationObjectId = new ObjectId(translationId);
         
            ViewBag.contentClass = pageIndexService.GetContentClass(pageIndexObjectId, translationObjectId);
            ViewBag.Content = pageIndexService.GetContentString(pageIndexObjectId, translationObjectId);
            ViewBag.IsAdmin = false;

            ViewBag.RowHead = pageIndexService.GetRowHead(pageIndexObjectId, translationObjectId);
            ViewBag.RowTail = pageIndexService.GetRowTail(pageIndexObjectId, translationObjectId);

            if (isAdmin == true)
            {
                ViewBag.contentClass = ViewBag.contentClass + " edit-panel";
                ViewBag.IsAdmin = true;
                ViewBag.TranslationId = translationId;
                ViewBag.pageIndexObjectId = pageIndexId;
            }

            return PartialView();
        }

    }
}
