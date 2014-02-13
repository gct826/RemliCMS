﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

            var translationService = new TranslationService();
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
            return View();
        }
    }
}
