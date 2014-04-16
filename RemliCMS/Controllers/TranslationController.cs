using System.Web.Mvc;
using RemliCMS.Routes;
using RemliCMS.WebData.Entities;
using RemliCMS.WebData.Services;

namespace RemliCMS.Controllers
{
    [Authorize(Roles = "admin")]
    public class TranslationController : BaseController
    {
        public TranslationController(IRouteService routeService) : base(routeService)
        {
        }

        //
        // GET: /Admin/Translation/
        public ActionResult Index()
        {            
            ViewBag.Title = "Manage Translations";

            var translationService = new TranslationService();
            var translations = translationService.ListAll();

            return View(translations);
        }

        //
        // GET: /Admin/Translation/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Create New Translation";
            return View();
        }

        //
        // POST: /Admin/Translation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Translation submitTranslation)
        {
            ViewBag.Title = "Create New Translation";
            try
            {
                if (ModelState.IsValid)
                {
                    var translationService = new TranslationService();

                    if (translationService.IsExistUrl(submitTranslation.Url))
                    {
                        ViewBag.Message = "Translation URL already exist.";
                        return View(submitTranslation);
                    }

                    if (translationService.GetDefaultUrl() == "")
                    {
                        submitTranslation.IsDefault = true;
                        submitTranslation.IsActive = true;
                    }

                    submitTranslation.Url = submitTranslation.Url.ToLower();
                    submitTranslation.Code = submitTranslation.Code.ToLower();

                    translationService.Create(submitTranslation);

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                ViewBag.Message = "Translation Save Error";
                return View(submitTranslation); 
            }
            return View(submitTranslation); 
        }

        //
        // GET: /Admin/Translation/Edit?url
        public ActionResult Edit(string url)
        {
            ViewBag.Title = "Edit Translation - " + url;

            var translationService = new TranslationService();
            var isExist = translationService.IsExistUrl(url);

            if (isExist == false)
            {
                System.Diagnostics.Debug.WriteLine("Edit Translation not found");
                return RedirectToAction("Index", "Translation");
            }

            var foundTranslation = translationService.Details(url);

            ViewBag.ObjectId = foundTranslation.Id;

            return View(foundTranslation);
        }

        //
        // POST: /Admin/Translation/Edit?url
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string url, Translation submitTranslation)
        {
            var translationService = new TranslationService();
            var foundTranslation = translationService.Details(url);

            submitTranslation.Id = foundTranslation.Id;

            if (submitTranslation.IsDefault && submitTranslation.IsActive == false)
            {
                ViewBag.Message = "Can Active flag must be set for Default Translation.";
                ViewBag.ObjectId = foundTranslation.Id;
                return View(foundTranslation);
            }

            if (foundTranslation.IsDefault != submitTranslation.IsDefault)
            {
                var defaultTranslation = translationService.Details(translationService.GetDefaultUrl());
                
                if (defaultTranslation != null)
                {
                    defaultTranslation.IsDefault = false;
                    translationService.Update(defaultTranslation);
                }
            }

            translationService.Update(submitTranslation);

            return RedirectToAction("Index","Translation");
        }

        //
        // POST: /Admin/Translation/SetDefault?url
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetDefault(string url, Translation submitTranslation)
        {
            var translationService = new TranslationService();
            var foundTranslation = translationService.Details(url);

            submitTranslation.Id = foundTranslation.Id;

            var defaultTranslation = translationService.Details(translationService.GetDefaultUrl());
            defaultTranslation.IsDefault = false;
            translationService.Update(defaultTranslation);

            submitTranslation.IsActive = true;
            submitTranslation.IsDefault = true;
            translationService.Update(submitTranslation);

            return RedirectToAction("Edit", "Translation", new { url = submitTranslation.Url });
        }

        //
        // GET: /Admin/Translation/Delete?url
        public ActionResult Delete(string url)
        {
            ViewBag.Title = "Delete Translation - " + url;

            var translationService = new TranslationService();
            var isExist = translationService.IsExistUrl(url);

            if (isExist == false)
            {
                System.Diagnostics.Debug.WriteLine("Delete Translation not found");
                return RedirectToAction("Index", "Translation");
            }

            var foundTranslation = translationService.Details(url);

            ViewBag.allowDelete = true;

            if (foundTranslation.IsDefault || foundTranslation.IsActive)
            {
                ViewBag.Message = "Can not delete Default or Active Translation";
                ViewBag.allowDelete = false;
            }

            ViewBag.ObjectId = foundTranslation.Id;

            return View(foundTranslation);
        }

        //
        // POST: /Admin/Translation/Delete?url
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string url, Translation submitTranslation)
        {
            var translationService = new TranslationService();
            var foundTranslation = translationService.Details(url);

            if (foundTranslation == null)
            {
                System.Diagnostics.Debug.WriteLine("Delete Translation not found");
                return RedirectToAction("Index", "Translation");
            }

            submitTranslation.Id = foundTranslation.Id;
            submitTranslation.Name = foundTranslation.Name;
            submitTranslation.Code = foundTranslation.Code;

            if (foundTranslation.IsDefault || foundTranslation.IsActive)
            {
                ViewBag.Message = "Can not delete Translation.";
                ViewBag.ObjectId = foundTranslation.Id;
                return View(foundTranslation);
            }

            submitTranslation.Url = "";
            submitTranslation.IsDeleted = true;
            submitTranslation.IsActive = false;
            submitTranslation.IsDefault = false;

            translationService.Update(submitTranslation);

            return RedirectToAction("Index", "Translation");
        }


    }
}
