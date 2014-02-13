using System.Web.Mvc;
using RemliCMS.Routes;
using RemliCMS.WebData.Entities;
using RemliCMS.WebData.Services;

namespace RemliCMS.Controllers
{
    public class TranslationController : BaseController
    {
        public TranslationController(IRouteService routeService) : base(routeService)
        {
        }

        //
        // GET: /Admin/Translation/
        public ActionResult Index()
        {
            RouteValues routeValues = RouteValue;
            if (routeValues.Translation != "admin")
            {
                return RedirectToAction("Index", "Home");
            }
            
            ViewBag.Title = "Admin - Translations";

            var translationService = new TranslationService();
            var translations = translationService.ListAll();

            return View(translations);
        }

        //
        // GET: /Admin/Translation/Create
        public ActionResult Create()
        {
            RouteValues routeValues = RouteValue;
            if (routeValues.Translation != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Title = "Create New Translation";
            return View();
        }

        //
        // POST: /Admin/Translation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Translation submitTranslation)
        {
            ViewBag.Title = "Translation Create New";
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
                return View(submitTranslation); 
            }
            return View(submitTranslation); 
        }

        //
        // GET: /Admin/Translation/Edit?url
        public ActionResult Edit(string url)
        {
            RouteValues routeValues = RouteValue;
            if (routeValues.Translation != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Title = "Translation Edit Existing";

            var translationService = new TranslationService();
            var isExist = translationService.IsExistUrl(url);

            if (isExist == false)
            {
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
        [MultiButton(MatchFormKey = "translation update", MatchFormValue1 = "Save", MatchFormValue2 = "Save As")]
        public ActionResult Edit(string url, Translation submitTranslation)
        {
            var translationService = new TranslationService();
            var foundTranslation = translationService.Details(url);

            submitTranslation.Id = foundTranslation.Id;

            if (foundTranslation.IsDefault && submitTranslation.IsActive == false)
            {
                ViewBag.Message = "Can not turn off Active flag for Default Translation.";
                ViewBag.ObjectId = foundTranslation.Id;
                return View(foundTranslation);
            }

            translationService.Update(submitTranslation);

            return RedirectToAction("Index","Translation");
        }

        //
        // POST: /Admin/Translation/SetDefault?url
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultiButton(MatchFormKey = "translation update", MatchFormValue1 = "Set as Default", MatchFormValue2 = "SetDefault")]
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

    }
}
