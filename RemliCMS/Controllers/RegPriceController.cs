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
    [Authorize(Roles = "admin")]
    public class RegPriceController : Controller
    {
        //
        // GET: /RegPrice/

        public ActionResult Index()
        {
            ViewBag.Title = "Registration Price Admin";

            var regValueService = new RegValueService();

            var translationService = new TranslationService();
            var defaultTranslation = translationService.GetDefaultUrl();
            var defaultTranslationObjectId = translationService.GetTranslationObjectId(defaultTranslation);

            var roomTypeTextList = regValueService.GetValueTextList("roomtype", defaultTranslationObjectId);

            return View(roomTypeTextList);
        }

        //
        // GET: /RegPrice/IndexHelper
        [ChildActionOnly]
        public ActionResult IndexHelper(int roomTypeId = 0)
        {
            var regValueService = new RegValueService();
            var regPriceService = new RegPriceService();

            var translationService = new TranslationService();
            var defaultTranslation = translationService.GetDefaultUrl();
            var defaultTranslationObjectId = translationService.GetTranslationObjectId(defaultTranslation);

            var ageRangeTextList = regValueService.GetValueTextList("agerange", defaultTranslationObjectId);

            ViewBag.Header = true;

            if (roomTypeId != 0)
            {
                foreach (var item in ageRangeTextList)
                {
                    ViewBag.Header = false;
                    item.Text = regPriceService.GetPrice(roomTypeId,item.Value).ToString();
                    
                }
            }
            ViewBag.RoomTypeId = roomTypeId;
            return View(ageRangeTextList);
        }

        //
        // GET: /RegPrice/PriceEdit
        public ActionResult PriceEdit(int ageRangeId, int roomTypeId)
        {
            var regPriceService = new RegPriceService();

            var foundRegPrice = regPriceService.GetRegPrice(roomTypeId, ageRangeId);
   
            if (foundRegPrice == null)
            {
                var newRegPrice = new RegPrice();
                newRegPrice.AgeRangeId = ageRangeId;
                newRegPrice.RoomTypeId = roomTypeId;
                newRegPrice.Price = (decimal) 0.00;

                return View(newRegPrice);
            }

            return View(foundRegPrice);
        }

        //
        // POST: /RegPrice/PriceEdit
        [HttpPost]
        public ActionResult PriceEdit(int ageRangeId, int roomTypeId, RegPrice submitPrice)
        {
            var regPriceService = new RegPriceService();

            var foundRegPrice = regPriceService.GetRegPrice(roomTypeId, ageRangeId);

            if (foundRegPrice != null)
            {
                submitPrice.Id = foundRegPrice.Id;      
            }

            regPriceService.Update(submitPrice);

            return RedirectToAction("Index");

        }
    
    }
}
