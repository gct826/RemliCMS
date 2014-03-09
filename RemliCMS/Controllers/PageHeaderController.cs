using System.Collections.Generic;
using System.Web.Mvc;
using MongoDB.Bson;
using RemliCMS.Routes;
using RemliCMS.WebData.Entities;
using RemliCMS.WebData.Services;

namespace RemliCMS.Controllers
{
    public class PageHeaderController : BaseController
    {
        public PageHeaderController(IRouteService routeService) : base(routeService)
        {
        }

        //
        // GET: /Admin/PageHeader/Index
        public ActionResult Index()
        {
            ViewBag.Title = "Manage Pages";
      
            return View();
        }
        
        //
        // GET: /Admin/PageHeader/IndexHelper?nodeLevel&parentId
        [ChildActionOnly]
        public ActionResult IndexHelper(string useCase, int nodeLevel, string parentId = null)
        {
            var queryParentId = new ObjectId();
            
            if (parentId == null)
            {
                queryParentId = ObjectId.Empty;
            }
            else
            {
                queryParentId = new ObjectId(parentId);
            }

            ViewBag.UseCase = useCase;
            
            ViewBag.SetParent = useCase[0] != '!';

            var pageHeaderService = new PageHeaderService();
            var pageHeadersChildren = pageHeaderService.ListAllChildren(queryParentId);

            ViewBag.hasChildren = true;

            if (pageHeadersChildren == null)
            {
                ViewBag.hasChildren = false;
            }

            string levelPrefix = "/";

            for (int i = 0; i < nodeLevel; i++)
            {
                levelPrefix = " ⇒ " + levelPrefix;
            }

            nodeLevel = nodeLevel + 1;
            ViewBag.nodeLevel = nodeLevel;
            ViewBag.levelPrefix = levelPrefix;

            return PartialView(pageHeadersChildren);
        }

        //
        // GET: /Admin/PageHeader/Create?parentPermalink
        public ActionResult Create(string parentPermalink)
        {
            ViewBag.Title = "Create New Page";

            var pageHeaderService = new PageHeaderService();
            
            if (parentPermalink == null)
            {
                return RedirectToAction("Index", "PageHeader");
            }
            
            if (parentPermalink != "Root")
            {
                var defaultParent = pageHeaderService.Details(parentPermalink);

                if (defaultParent == null)
                {
                    return RedirectToAction("Index", "PageHeader");
                }
                ViewBag.ParentName = pageHeaderService.ReturnPath(defaultParent.Id);
            }
            else
            {
                ViewBag.ParentName = "Root";
            }

            return View();
        }

        //
        // POST: /Admin/PageHeader/Create?parentPermalink
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string parentPermalink, PageHeader pageHeader)
        {
            ViewBag.Title = "Create New Page";
            try
            {
                if (ModelState.IsValid)
                {
                    var pageHeaderService = new PageHeaderService();
                    
                    if (parentPermalink == null)
                    {
                        return RedirectToAction("Index", "PageHeader");
                    }

                    if (parentPermalink != "Root")
                    {
                        var defaultParent = pageHeaderService.Details(parentPermalink);
                        if (defaultParent == null)
                        {
                            return RedirectToAction("Index", "PageHeader");
                        }
                        pageHeader.ParentId = defaultParent.Id;
                        //ViewBag.ParentId = defaultParent.Id;
                        ViewBag.ParentName = defaultParent.Name;
                    }
                    else
                    {
                        pageHeader.ParentId = ObjectId.Empty;
                        ViewBag.ParentName = "Root";
                    }

                    if (pageHeaderService.IsExistPermalink(pageHeader.Permalink))
                    {
                        ViewBag.Message = "PageHeader permalink already exist.";
                        return View(pageHeader);
                    }

                    if (pageHeaderService.GetDefaultPermalink() == "")
                    {
                        pageHeader.IsDefault = true;
                    }

                    pageHeader.Permalink = pageHeader.Permalink.ToLower();

                    pageHeaderService.Create(pageHeader);

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return View(pageHeader);
            }
            return View(pageHeader);
        }

        //
        // GET: /Admin/PageHeader/Edit?permalink
        public ActionResult Edit(string pagePermalink)
        {
            if (pagePermalink == null)
            {
                System.Diagnostics.Debug.WriteLine("Illegal Edit PageHeader url");
                return RedirectToAction("Index", "PageHeader");
            }

            var pageHeaderService = new PageHeaderService();
            var foundPageHeader = pageHeaderService.Details(pagePermalink);

            if (foundPageHeader == null)
            {
                System.Diagnostics.Debug.WriteLine("Edit PageHeader not found");
                return RedirectToAction("Index", "PageHeader");
            }

            ViewBag.Title = "Edit Page - " + pagePermalink;
            ViewBag.ObjectId = foundPageHeader.Id;
            ViewBag.PagePath = pageHeaderService.ReturnPath(foundPageHeader.Id);

            return View(foundPageHeader);
        }

        //
        // POST: /Admin/PageHeader/Edit?permalink
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string pagePermalink, PageHeader submittedPageHeader)
        {
            if (pagePermalink == null)
            {
                System.Diagnostics.Debug.WriteLine("Illegal Edit PageHeader url");
                return RedirectToAction("Index", "PageHeader");
            }

            var pageHeaderService = new PageHeaderService();
            var foundPageHeader = pageHeaderService.Details(pagePermalink);

            if (foundPageHeader == null)
            {
                System.Diagnostics.Debug.WriteLine("Edit PageHeader not found");
                return RedirectToAction("Index", "PageHeader");
            }

            ViewBag.Title = "Edit Page - " + pagePermalink;
            ViewBag.ObjectId = foundPageHeader.Id;
            ViewBag.PagePath = pageHeaderService.ReturnPath(foundPageHeader.Id);

            if (pageHeaderService.IsExistPermalink(submittedPageHeader.Permalink))
            {
                ViewBag.Message = "PageHeader permalink already exist.";
                return View(submittedPageHeader);
            }

            foundPageHeader.Name = submittedPageHeader.Name;
            foundPageHeader.Permalink = submittedPageHeader.Permalink;

            if (foundPageHeader.IsDefault != submittedPageHeader.IsDefault)
            {
                var defaultPageHeader = pageHeaderService.Details(pageHeaderService.GetDefaultPermalink());
                
                if (defaultPageHeader != null)
                {
                    defaultPageHeader.IsDefault = false;
                    pageHeaderService.UpdateDetails(defaultPageHeader);
                }

                foundPageHeader.IsDefault = true;

            }

            pageHeaderService.UpdateDetails(foundPageHeader);

            return RedirectToAction("Index","PageHeader");
        }

        //
        // GET: /Admin/PageHeader/SetParent?
        public ActionResult SetParent(string pagePermalink)
        {
            if (pagePermalink == null)
            {
                System.Diagnostics.Debug.WriteLine("Illegal Set Page Parent url"); 
                return RedirectToAction("Index", "PageHeader");
            }

            ViewBag.Title = "Set Page Parent - " + pagePermalink;

            var pageHeaderService = new PageHeaderService();
            var isExist = pageHeaderService.IsExistPermalink(pagePermalink);

            if (isExist == false)
            {
                System.Diagnostics.Debug.WriteLine("Set Page Parent not found");
                return RedirectToAction("Index", "PageHeader");
            }

            ViewBag.CurrentPermalink = pagePermalink;

            return View();
        }

        //
        // POST: /Admin/PageHeader/SetParent?
        [HttpPost]
        public ActionResult SetParent(string pagePermalink, FormCollection submittal)
        {
            if (pagePermalink == null)
            {
                System.Diagnostics.Debug.WriteLine("Illegal Set Page url");
                return RedirectToAction("Index", "PageHeader");
            }

            ViewBag.Title = "Set Page Parent - " + pagePermalink;

            var pageHeaderService = new PageHeaderService();
            var modifiedPageHeader = pageHeaderService.Details(pagePermalink);

            if (modifiedPageHeader == null)
            {
                System.Diagnostics.Debug.WriteLine("Set Page Parent not found");
                return RedirectToAction("Index", "PageHeader");
            }

            string newParentId = submittal["item.Id"];

            if (newParentId == null)
            {
                modifiedPageHeader.ParentId = ObjectId.Empty;
            }
            else
            {
                modifiedPageHeader.ParentId = new ObjectId(newParentId);              
            }

            modifiedPageHeader.Order = pageHeaderService.GetLastOrder(modifiedPageHeader.ParentId) + 1;
            
            pageHeaderService.UpdateParentId(modifiedPageHeader);

            pageHeaderService.ReOrderChildren(modifiedPageHeader.ParentId);

            return RedirectToAction("SetOrder", "PageHeader", new {parentId = modifiedPageHeader.ParentId});
        }

        //
        // GET: /Admin/PageHeader/SetOrder?parentId
        public ActionResult SetOrder(string parentId)
        {
            if (parentId == null)
            {
                System.Diagnostics.Debug.WriteLine("Illegal Set Order parent url");
                return RedirectToAction("Index", "PageHeader");
            }

            var pageHeaderService = new PageHeaderService();
            var pageHeader = pageHeaderService.GetById(parentId);

            var pagePermalink = "";

            if (parentId == "000000000000000000000000")
            {
                pagePermalink = "Root";
            }
            else if(pageHeader != null)
            {
                pagePermalink = pageHeader.Permalink;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Set Order Parent not found");
                return RedirectToAction("Index", "PageHeader");
            }
            
            ViewBag.Title = "Set Children Order - " + pagePermalink;
            ViewBag.CurrentId = parentId;

            return View();
        }

        //
        // POST: /Admin/PageHeader/SetOrder?parentPermalink
        [HttpPost]
        public ActionResult SetOrder(string parentId, FormCollection submittal)
        {
            if (parentId == null)
            {
                System.Diagnostics.Debug.WriteLine("Illegal Set Order parent url");
                return RedirectToAction("Index", "PageHeader");
            }

            var pageHeaderService = new PageHeaderService();
            var pageHeader = pageHeaderService.GetById(parentId);

            var pagePermalink = "";

            if (parentId == "000000000000000000000000")
            {
                pagePermalink = "Root";
            }
            else if (pageHeader != null)
            {
                pagePermalink = pageHeader.Permalink;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Set Order Parent not found");
                return RedirectToAction("Index", "PageHeader");
            }

            ViewBag.Title = "Set Children Order - " + pagePermalink;            
            ViewBag.CurrentId = parentId;

            string pageHeaderId = submittal["item.Id"];
            string direction = submittal["dir"];

            if (direction == "up")
            {
                var pageHeaderObjectId = new ObjectId(pageHeaderId);
                pageHeaderService.MoveOrderUp(pageHeaderObjectId);
            }

            if (direction == "down")
            {
                var pageHeaderObjectId = new ObjectId(pageHeaderId);
                pageHeaderService.MoveOrderDown(pageHeaderObjectId);
            }

            return View();
        }

    }
}

