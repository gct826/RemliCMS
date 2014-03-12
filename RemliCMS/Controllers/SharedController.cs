using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using RemliCMS.Models;
using RemliCMS.Routes;
using RemliCMS.WebData.Entities;
using RemliCMS.WebData.Services;

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

        //
        // GET: /Shared/NavBar
        [ChildActionOnly]
        public ActionResult NavBar()
        {
            RouteValues routeValues = RouteValue;

            var menuItem = new List<MenuModel>();

            //if (routeValues.Translation.ToLower() == "admin")
            //{
            //    ViewBag.isAdmin = true;

            //    menuItem.Add(new MenuModel() { Title = "Home", Permalink = "Home", isActive = false });
            //    return PartialView(menuItem);
            //}

            var pageHeaderService = new PageHeaderService();
            var pageHeaders = pageHeaderService.ListAllChildren(ObjectId.Empty);

            var translationService = new TranslationService();
            var translation = translationService.Details(routeValues.Translation);

            foreach (var item in pageHeaders)
            {
                var title = pageHeaderService.ReturnPageTitle(item.Id, translation.Id);

                bool isCurrent = false || item.Permalink == routeValues.Permalink;

                if (title != null)
                {
                    menuItem.Add(new MenuModel() { Title = title.Title, Permalink = item.Permalink, IsActive = title.IsActive, IsCurrent = isCurrent });
                }

            }

            return PartialView(menuItem);
        }

        // GET: /Home/TranslationSwitcher
        [ChildActionOnly]
        public ActionResult TranslationSwitcher()
        {
            RouteValues routeValues = RouteValue;
            var translationService = new TranslationService();
            var translationList = translationService.ListAll();

            ViewBag.currentCode = routeValues.Translation.ToLower();
            ViewBag.Seperator = " · ";

            return PartialView(translationList);

        }

    }
}
