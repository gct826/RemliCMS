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
    public class AdminController : BaseController
    {
        public AdminController(IRouteService routeService) : base(routeService)
        {
        }

        //
        // GET: /Admin/Index
        [Authorize(Roles="admin")]
        public ActionResult Index()
        {
            ViewBag.Title = "Site Administration";

            var translationService = new TranslationService();
            var defaultTranslation = translationService.Details(translationService.GetDefaultUrl());

            if (defaultTranslation == null)
            {
                ViewBag.DefaultTranslation = "no default";
            }
            else
            {
                ViewBag.DefaultTranslation = defaultTranslation.Url;
                ViewBag.DefaultTranslationName = defaultTranslation.Name;                
            }

            var pageHeaderService = new PageHeaderService();
            var defaultPageHeader = pageHeaderService.Details(pageHeaderService.GetDefaultPermalink());

            if (defaultPageHeader == null)
            {
                ViewBag.DefaultPage = "no default";
            }
            else
            {
                ViewBag.DefaultPage = defaultPageHeader.Permalink;
                ViewBag.DefaultPageName = defaultPageHeader.Name;
            }

            return View();
        }

    }
}
