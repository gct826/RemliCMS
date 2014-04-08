using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MongoDB.Bson;
using RemliCMS.Routes;
using RemliCMS.Models;
using RemliCMS.RegSystem.Entities;
using RemliCMS.RegSystem.Services;
using RemliCMS.WebData.Services;

namespace RemliCMS.Controllers
{
    public class RegFieldController : Controller
    {
        //
        // GET: /Admin/RegField/Index
        public ActionResult Index()
        {
            ViewBag.Title = "Manage Registration Field";

            var regFieldService = new RegFieldService();
            var allValues = regFieldService.ListAll();

            return View(allValues);
        }


        //
        // GET: /Admin/RegField/IndexHelper
        [ChildActionOnly]
        public ActionResult IndexHelper(string regFieldId)
        {
            var regFieldService = new RegFieldService();
            var regField = regFieldService.GetById(regFieldId);

            if (regField == null)
            {
                return RedirectToAction("Index");
            }

            var regValueService = new RegValueService();

            var foundValues = regValueService.GetAllValues(regField.Id);

            ViewBag.regFieldId = regFieldId;

            return PartialView(foundValues);
        }

        // GET:/Admin/RegField/TranslationHelper
        [ChildActionOnly]
        public ActionResult TranslationHelper(string regFieldId, int value = 0)
        {
            var translationService = new TranslationService();
            var translationList = translationService.ListAll();

            var valueList = new List<RegTranslationModel>();

            if (value == 0)
            {
                foreach (var translation in translationList)
                {
                    valueList.Add(new RegTranslationModel() { translationId = translation.Id.ToString(), text = translation.Name });
                }

                ViewBag.headerView = true;
                return PartialView(valueList);
            }

            ViewBag.headerView = false;

            var regFieldObjectId = new ObjectId(regFieldId);

            var regValueService = new RegValueService();
            var textList = regValueService.ListValueText(regFieldObjectId, value);

            foreach (var translation in translationList)
            {
                var translationText = new RegTranslationModel();
                translationText.translationId = translation.Id.ToString();

                var text = textList.FindLast(pt => pt.TranslationId == translation.Id);
                translationText.text = text == null ? "" : text.Text;

                //try
                //{
                //    translationText.text = textList.FindLast(pt => pt.TranslationId == translation.Id).Text;
                //}
                //catch
                //{
                //    translationText.text = "";
                //}

                valueList.Add(translationText);
            }

            return PartialView(valueList);
        }

        // GET: /Admin/RegField/TranslationText
        [ChildActionOnly]
        public ActionResult TranslationText(string translationId)
        {
            var translationService = new TranslationService();
            var translationObjectId = new ObjectId(translationId);

            ViewBag.translation = translationService.GetName(translationObjectId);
            return PartialView();
        }

        //
        // GET: /Admin/RegField/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Create New Registration Field";
            return View();
        }

        //
        // POST: /Admin/RegField/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegField submitRegField)
        {
            ViewBag.Title = "Create New Registration Field";

            try
            {
                if (ModelState.IsValid)
                {
                    var regFieldService = new RegFieldService();

                    if (regFieldService.IsExistKey(submitRegField.Key))
                    {
                        ViewBag.Message = "Field Key already exist.";
                        return View(submitRegField);
                    }

                    regFieldService.Create(submitRegField);

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                ViewBag.Message = "Field Save Error";
                return View(submitRegField);
            }

            return View(submitRegField);
        }

        //
        // GET:/Admin/RegField/AddValue
        public ActionResult AddValue(string regFieldId)
        {
            if (regFieldId == null)
            {
                return RedirectToAction("Index");
            }


            var regFieldObjectId = new ObjectId(regFieldId);

            var regFieldService = new RegFieldService();
            var regField = regFieldService.GetById(regFieldId);

            if (regField == null)
            {
                return RedirectToAction("Index");
            }

            var regValueService = new RegValueService();

            var lastValue = regValueService.GetLastValue(regFieldObjectId);

            var newValue = new RegValue();

            newValue.Value = lastValue + 1;
            newValue.RegFieldObjectId = regFieldObjectId;
            newValue.IsDeleted = false;
            newValue.IsActive = true;

            regValueService.Update(newValue);

            return RedirectToAction("AddText", new {regFieldId,newValue.Value});

        }

        //
        // GET:/Admin/RegField/AddText
        public ActionResult AddText(string regFieldId, int value = 0)
        {
            ViewBag.Title = "Create - Reg Field Value";

            if (regFieldId == null)
            {
                return RedirectToAction("Index");
            }

            if (value == 0)
            {
                return RedirectToAction("Index");
            }

            var regFieldService = new RegFieldService();
            var regField = regFieldService.GetById(regFieldId);

            var translationService = new TranslationService();
            var translationList = translationService.ListAll();

            var regText = new List<RegText>();
            
            var regValueService = new RegValueService();
            var foundValue = regValueService.ListValueText(regField.Id, value);

            if (foundValue.Count == 0)
            {
                foreach (var translation in translationList)
                {
                    var addRegText = new RegText();
                    addRegText.TranslationId = translation.Id;
                    regText.Add(addRegText);
                }
            }
            else
            {
                foreach (var translation in translationList)
                {
                    var addRegText = new RegText();
                    addRegText.TranslationId = translation.Id;
                    
                    var text = foundValue.FindLast(pt => pt.TranslationId == translation.Id);
                    addRegText.Text = text == null ? "" : text.Text;
                    
                    //addRegText.Text = foundValue.FindLast(pt => pt.TranslationId == translation.Id).Text;
                    
                    regText.Add(addRegText);
                }
            }

            ViewBag.regField = regField.Name;
            ViewBag.value = value;

            return View(regText);
        }

        //
        // POST:/Admin/RegField/AddText
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddText(string regFieldId, int value, FormCollection submittal)
        {
            ViewBag.Title = "Create - Reg Field Value";

            if (regFieldId == null)
            {
                return RedirectToAction("Index");
            }

            if (value == 0)
            {
                return RedirectToAction("Index");
            }

            var regFieldService = new RegFieldService();
            var regField = regFieldService.GetById(regFieldId);

            ViewBag.regField = regField.Name;
            ViewBag.value = value;

            var translationService = new TranslationService();
            var translationList = translationService.ListAll();

            var regText = new List<RegText>();



            bool allFilled = true;

            foreach (var translation in translationList)
            {
                var addRegText = new RegText();
                addRegText.TranslationId = translation.Id;
                addRegText.Text = submittal[translation.Id.ToString()];
                if (addRegText.Text == "")
                {
                    allFilled = false;
                }
                regText.Add(addRegText);
            }
                
            if (!allFilled)
                {
                    ViewBag.Message = "Text for all translation required.";
                    return View(regText);
                }

            var regValueService = new RegValueService();

            foreach (var updateText in regText)
            {
                regValueService.AddText(regField.Id, value,updateText);
            }

            return RedirectToAction("Index");
        }    
    }
}
