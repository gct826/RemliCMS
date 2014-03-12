using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MongoDB.Bson;
using RemliCMS.Models;
using RemliCMS.Routes;
using RemliCMS.Helpers;
using RemliCMS.WebData.Entities;
using RemliCMS.WebData.Services;

namespace RemliCMS.Controllers
{
    public class PageContentController : BaseController
    {
        public PageContentController(IRouteService routeService) : base(routeService)
        {
        }

        //
        //GET: /Admin/PageContent?pagePermalink&translationCode
        public ActionResult Index(string pagePermalink, string translationCode)
        {
            var pageHeaderService = new PageHeaderService();
            var translationService = new TranslationService();
            var pageIndexService = new PageIndexService();

            if (pagePermalink == null)
            {
                return RedirectToAction("Index", "PageHeader");
            }

            if (translationCode == null)
            {
                translationCode = translationService.GetDefaultUrl();
            }
            
            if (pageHeaderService.IsExistPermalink(pagePermalink) == false)
            {
                return RedirectToAction("Index", "PageHeader");
            }

            var foundPageHeader = pageHeaderService.Details(pagePermalink);
            var foundTranslation = translationService.Details(translationCode);

            ViewBag.pageHeaderId = foundPageHeader.Id;
            ViewBag.translationId = foundTranslation.Id;

            var pageIndexList = pageIndexService.ListIndexs(foundPageHeader.Id);

            var pageTitle = pageHeaderService.ReturnPageTitle(foundPageHeader.Id, foundTranslation.Id);

            ViewBag.Title = "Admin - PageContent";

            if (pageTitle == null)
            {
                ViewBag.Title = "No Title Set - page inactive";
            }
            else if (pageTitle.IsActive == false)
            {
                ViewBag.Title = pageTitle.Title + " - page inactive";
            }
            else
            {
                ViewBag.Title = pageTitle.Title;
            }

            return View(pageIndexList);

        }
        
        //
        //GET: /Admin/PageContent/IndexId?pageHeaderId&translationId
        public ActionResult IndexId(string pageHeaderId, string translationId)
        {
            //RouteValues routeValues = RouteValue;
            //if (routeValues.Translation != "admin")
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            if (pageHeaderId == null || translationId == null)
            {
                return RedirectToAction("Index", "PageHeader");
            }

            var pageHeaderService = new PageHeaderService();
            var translationService = new TranslationService();

            var foundPageHeader = pageHeaderService.GetById(pageHeaderId);
            var foundTranslation = translationService.GetById(translationId);

            if (foundPageHeader == null || foundTranslation == null)
            {
                return RedirectToAction("Index", "PageHeader");
            }

            return RedirectToAction("Index", "PageContent", new { pagePermalink = foundPageHeader.Permalink, translationCode = foundTranslation.Code });
        }

        //
        //GET: /Admin/PageContent/AddIndex?pageHeaderId&translationId
        public ActionResult AddIndex(string pageHeaderId, string translationId)
        {
            if (pageHeaderId == null)
            {
                return RedirectToAction("Index", "PageHeader");
            }

            var pageHeaderService = new PageHeaderService();
            var pageIndexService = new PageIndexService();

            var foundPageHeader = pageHeaderService.GetById(pageHeaderId);

            if (foundPageHeader != null)
            {
                var lastOrderNum = pageIndexService.GetLastOrderNum(foundPageHeader.Id);

                var newPageIndex = new PageIndex
                {
                    PageHeaderId = foundPageHeader.Id,
                    CreatedDate = DateTime.Now,
                    SmWidth = 12,
                    LgWidth = 12,
                    Order = lastOrderNum + 1
                };

                pageIndexService.Create(newPageIndex);

            }

            return RedirectToAction("IndexId", "PageContent", new {pageHeaderId, translationId});
        }

        //
        //GET: /Admin/PageContent/Create?indexId&translationId
        public ActionResult Create(string indexId, string translationId)
        {
            ViewBag.Title = "Content Edit";
            
            //RouteValues routeValues = RouteValue;
            //if (routeValues.Translation != "admin")
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            if (indexId == null || translationId == null)
            {
                return RedirectToAction("Index", "PageHeader");
            }

            var pageIndexService = new PageIndexService();
            var pageHeaderService = new PageHeaderService();
            var translationService = new TranslationService();

            var foundPageIndex = pageIndexService.GetById(indexId);
            var foundPageHeader = pageHeaderService.GetById(foundPageIndex.PageHeaderId.ToString());
            var foundTranslation = translationService.GetById(translationId);

            ViewBag.pageHeader = foundPageHeader.Name;
            ViewBag.Location = pageHeaderService.ReturnPath(foundPageHeader.Id);
            ViewBag.translation = foundTranslation.Name;
            ViewBag.indexId = foundPageIndex.Id;
            ViewBag.translationId = foundTranslation.Id;
            ViewBag.smwidth = foundPageIndex.SmWidth;
            ViewBag.lgwidth = foundPageIndex.LgWidth;
            
            ViewBag.contentClass = "small-" + foundPageIndex.SmWidth + " large-" + foundPageIndex.LgWidth + " columns";

            var foundContent = pageIndexService.GetLastContent(foundPageIndex.Id, foundTranslation.Id);

            if (foundContent != null)
            {
                return View(foundContent);
            }
            return View();
        }

        //
        //POST: /Admin/PageContent/Create?indexId&translationId
        [ValidateInput(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string indexId, string translationId, PageContent pageContent)
        {
            ViewBag.Title = "Content Edit";

            try
            {
                if (indexId == null || translationId == null)
                {
                    return RedirectToAction("Index");
                }

                var pageHeaderService = new PageHeaderService();
                var translationService = new TranslationService();
                var pageIndexService = new PageIndexService();

                var foundPageIndex = pageIndexService.GetById(indexId);
                var foundPageHeader = pageHeaderService.GetById(foundPageIndex.PageHeaderId.ToString());
                var foundTranslation = translationService.GetById(translationId);

                ViewBag.pageHeader = foundPageHeader.Name;
                ViewBag.Location = pageHeaderService.ReturnPath(foundPageHeader.Id);
                ViewBag.translation = foundTranslation.Name;
                ViewBag.translationId = foundTranslation.Id;

                //var foundContent = pageIndexService.GetContent(foundPageIndex.Id, foundTranslation.Id);

                pageContent.TranslationId = foundTranslation.Id;
                pageContent.CreatedDate = DateTime.Now;

                pageIndexService.AddContent(foundPageIndex.Id, pageContent);
                return RedirectToAction("IndexId", "PageContent", new { foundPageHeader.Id, translationId });

            }
            catch (Exception)
            {
                ViewBag.Message = "Try Failed";
                return View(pageContent);
            }


        }

        //
        //GET: /Admin/PageContent/MoveDown?indexId&translationId
        public ActionResult MoveDown(string indexId, string translationId)
        {
            //RouteValues routeValues = RouteValue;
            //if (routeValues.Language != "admin")
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            if (indexId == null || translationId == null)
            {
                return RedirectToAction("Index", "PageHeader");
            }

            var pageIndexService = new PageIndexService();

            var foundPageIndex = pageIndexService.GetById(indexId);

            if (foundPageIndex == null)
            {
                return RedirectToAction("Index", "PageHeader");
            }
           
            pageIndexService.MoveOrderDown(foundPageIndex.Id);

            return RedirectToAction("IndexId", "PageContent", new { foundPageIndex.PageHeaderId, translationId });
        }

        //
        //GET: /Admin/PageContent/MoveUp?indexId&translationId
        public ActionResult MoveUp(string indexId, string translationId)
        {
            //RouteValues routeValues = RouteValue;
            //if (routeValues.Language != "admin")
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            if (indexId == null || translationId == null)
            {
                return RedirectToAction("Index", "PageHeader");
            }

            var pageIndexService = new PageIndexService();

            var currentPageIndex = pageIndexService.GetById(indexId);

            if (currentPageIndex == null)
            {
                return RedirectToAction("Index", "PageHeader");
            }

            pageIndexService.MoveOrderUp(currentPageIndex.Id);

            return RedirectToAction("IndexId", "PageContent", new { currentPageIndex.PageHeaderId, translationId });
        }
        
        //
        //GET: /Admin/PageContent/SetWidth?indexId
        public ActionResult SetWidth(string indexId, string translationId)
        {
            //RouteValues routeValues = RouteValue;
            //if (routeValues.Language != "admin")
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            if (indexId == null || translationId == null)
            {
                return RedirectToAction("Index", "PageHeader");
            }

            var pageIndexService = new PageIndexService();
            var foundPageIndex = pageIndexService.GetById(indexId);

            if (foundPageIndex == null)
            {
                return RedirectToAction("Index", "PageHeader");
            }

            ViewBag.indexId = indexId;
            ViewBag.translationId = translationId;

            List<SelectListItem> columnDD = new List<SelectListItem>();
            columnDD.Add(new SelectListItem { Value = "1", Text = "1"});
            columnDD.Add(new SelectListItem { Value = "2", Text = "2" });
            columnDD.Add(new SelectListItem { Value = "3", Text = "3" });
            columnDD.Add(new SelectListItem { Value = "4", Text = "4" });
            columnDD.Add(new SelectListItem { Value = "5", Text = "5" });
            columnDD.Add(new SelectListItem { Value = "6", Text = "6" });
            columnDD.Add(new SelectListItem { Value = "7", Text = "7" });
            columnDD.Add(new SelectListItem { Value = "8", Text = "8" });
            columnDD.Add(new SelectListItem { Value = "9", Text = "9" });
            columnDD.Add(new SelectListItem { Value = "10", Text = "10" });
            columnDD.Add(new SelectListItem { Value = "11", Text = "11" });
            columnDD.Add(new SelectListItem { Value = "12", Text = "12" });

            ViewBag.columnWidthDD = columnDD;


            
            return PartialView(foundPageIndex);
        }

        //
        // POST: /Admin/PageContent/SetWidth?indexId&translationId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetWidth(string indexId, string translationId, PageIndex submitIndex)
        {
            //RouteValues routeValues = RouteValue;
            //if (routeValues.Language != "admin")
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            if (indexId == null || translationId == null)
            {
                return RedirectToAction("Index", "PageHeader");
            }

            var pageIndexService = new PageIndexService();
            var foundPageIndex = pageIndexService.GetById(indexId);

            if (foundPageIndex == null)
            {
                return RedirectToAction("Index", "PageHeader");
            }

            ViewBag.indexId = indexId;
            ViewBag.translationId = translationId;

            foundPageIndex.SmWidth = submitIndex.SmWidth;
            foundPageIndex.LgWidth = submitIndex.LgWidth;

            pageIndexService.Update(foundPageIndex);

            return RedirectToAction("Create", "PageContent", new { indexId, translationId });

        }

    }
}

