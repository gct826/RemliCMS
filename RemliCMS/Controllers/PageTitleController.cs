using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MongoDB.Bson;
using RemliCMS.Routes;
using RemliCMS.Models;
using RemliCMS.WebData.Entities;
using RemliCMS.WebData.Services;

namespace RemliCMS.Controllers
{
    [Authorize(Roles = "admin")]
    public class PageTitleController : BaseController
    {
        public PageTitleController(IRouteService routeService) : base(routeService)
        {
        }

        //
        // GET: /Admin/PageTitle?pageHeaderId
        [ChildActionOnly]
        public ActionResult Index(string pageHeaderId = null)
        {
            var translationService = new TranslationService();
            var translationList = translationService.ListAll();

            var pageTitle = new List<TitleModel>();

            if (pageHeaderId == null)
            {
                foreach (var translation in translationList)
                {
                    pageTitle.Add(new TitleModel() { Name = translation.Name });
                }

                ViewBag.headerView = true;
                return PartialView(pageTitle);
            }


            ViewBag.headerView = false;
            ViewBag.pageHeaderId = pageHeaderId;

            var pageHeaderService = new PageHeaderService();
            var pageHeaderObjectId = new ObjectId(pageHeaderId);

            var pageHeaderTitleList = pageHeaderService.ListPageTitles(pageHeaderObjectId);

            foreach (var translation in translationList)
            {
                var pageHeaderTitle = pageHeaderTitleList.FindLast(pt => pt.TranslationId == translation.Id);

                if (pageHeaderTitle != null)
                {
                    pageTitle.Add(new TitleModel() { Name = translation.Name, TranslationId = translation.Id.ToString(), Title = pageHeaderTitle.Title, isActive = pageHeaderTitle.IsActive });
                }
                else
                {
                    pageTitle.Add(new TitleModel() { Name = translation.Name, TranslationId = translation.Id.ToString(), Title = "", isActive = false });
                }
            }

            return PartialView(pageTitle);
        }

        //
        // GET: /Admin/PageTitle/Create?pageHeaderId&translationId
        public ActionResult Create(string pageHeaderId, string translationId)
        {
            ViewBag.Title = "Create - Page Title";
            //RouteValues routeValues = RouteValue;
            //if (routeValues.Translation != "admin")
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            if (pageHeaderId == null)
            {
                return RedirectToAction("Index", "PageHeader"); 
            }
            var pageHeaderService = new PageHeaderService();
            var pageHeader = pageHeaderService.GetById(pageHeaderId);

            if (translationId == null)
            {
                return RedirectToAction("Index", "PageHeader");
            }
            var translationService = new TranslationService();
            var translation = translationService.GetById(translationId);

            ViewBag.pageHeader = pageHeader.Name;
            ViewBag.translation = translation.Name;
            ViewBag.translationId = translation.Id;

            var translationObjectId = new ObjectId(translationId);
            
            var title = pageHeader.PageTitles.FindLast(q => q.TranslationId == translationObjectId);
            
            if (title == null)
            {
                return View();
            }

            return View(title);
        }

        //
        // POST: /Admin/PageTitle/Create&pageHeaderId&translationId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string pageHeaderId, string translationId, PageTitle pageTitle)
        {
            ViewBag.Title = "Create - Page Title";

            try
            {
                if (pageHeaderId == null)
                {
                    return RedirectToAction("Index", "PageHeader");
                }
                var pageHeaderService = new PageHeaderService();
                var pageHeader = pageHeaderService.GetById(pageHeaderId);
                var pageHeaderObjectId = new ObjectId(pageHeaderId);

                if (translationId == null)
                {
                    return RedirectToAction("Index", "PageHeader");
                }
                var translationService = new TranslationService();
                var translation = translationService.GetById(translationId);

                ViewBag.pageHeader = pageHeader.Name;
                ViewBag.translation = translation.Name;

                var title = pageHeader.PageTitles.FindLast(q => q.TranslationId == translation.Id);

                if (title != null)
                {
                    title.TranslationId = translation.Id;
                    title.Title = pageTitle.Title;
                    title.IsActive = pageTitle.IsActive;
                    title.CreatedDate = DateTime.Now;
                    
                    pageHeaderService.AddTitle(pageHeaderObjectId, title);
                    return RedirectToAction("Index", "PageHeader");
                }

                pageTitle.TranslationId = translation.Id;
                pageTitle.CreatedDate = DateTime.Now;

                pageHeaderService.AddTitle(pageHeaderObjectId, pageTitle);

                return RedirectToAction("Index", "PageHeader");

            }
            catch
            {
                return View(pageTitle);
            }
            
        }

    }
}
