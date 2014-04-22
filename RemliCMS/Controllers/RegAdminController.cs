using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using RemliCMS.RegSystem.Entities;
using RemliCMS.RegSystem.Services;
using RemliCMS.Routes;
using RemliCMS.WebData.Services;

namespace RemliCMS.Controllers
{
    [Authorize(Roles = "admin")]
    public class RegAdminController : BaseController
    {
        public RegAdminController(IRouteService routeService) : base(routeService)
        {
        }


        //
        // GET: /RegAdmin/
        public ActionResult Index()
        {
            ViewBag.Title = "Registration Admin";

            var registrationService = new RegistrationService();

            var registrationList = registrationService.ListAllRegistrations();

            return View(registrationList);
        }

        //
        // GET: /RegAdmin/Participant
        public ActionResult Participant(int numResult=0)
        {
            ViewBag.Title = "Participant Search";

            var translationService = new TranslationService();
            var transObjectId = translationService.GetTranslationObjectId(translationService.GetDefaultUrl());

            var participantService = new ParticipantService();

            var participantList = participantService.ListAllParticipants();

            var regValueService = new RegValueService();

            var statusIdList = regValueService.GetValueTextList("status", transObjectId);
            ViewBag.StatusId = new string[statusIdList.Count + 1];
            foreach (var item in statusIdList)
            {
                ViewBag.StatusId[item.Value] = item.Text;
            }

            var genderIdList = regValueService.GetValueTextList("gender", transObjectId);
            ViewBag.GenderId = new string[genderIdList.Count + 1];
            foreach (var item in genderIdList)
            {
                ViewBag.GenderId[item.Value] = item.Text;
            }
            var ageRangeIdList = regValueService.GetValueTextList("agerange", transObjectId);
            ViewBag.AgeRangeId = new string[ageRangeIdList.Count + 1];
            foreach (var item in ageRangeIdList)
            {
                ViewBag.AgeRangeId[item.Value] = item.Text;
            }


            return View(participantList);
        }

        //
        // POST: /RegAdmin/Participant
        [HttpPost]
        public ActionResult Participant(FormCollection searchfield)
        {
            ViewBag.Title = "Participant Search";

            return View();
        }

        //
        // GET: /RegAdmin/ParticipantHelper
        public ActionResult ParticipantHelper(string partId)
        {
            var participantService = new ParticipantService();
            var registrationService = new RegistrationService();

            var partRegId = participantService.GetById(partId).RegId;
            var regObjectId = registrationService.GetByRegId(partRegId).Id;



            return RedirectToAction("Registration", "Register", new {regObjectId});

        }

        //
        // GET: /RegAdmin/PaymentManagement
        public ActionResult PaymentManagement()
        {
            var ledgerService = new LedgerService();

            var ledgerList = ledgerService.GetAllLedgerList();

            var translationService = new TranslationService();
            ViewBag.translationObjectId = translationService.GetTranslationObjectId(translationService.GetDefaultUrl());

            return View(ledgerList);
        }

        //
        // GET: /RegAdmin/RegHistory
        public ActionResult RegHistory(int regId)
        {

            var regHistoryService = new RegHistoryService();
            var regHistoryList = regHistoryService.ListRegHistory(regId);

            var translationService = new TranslationService();
            ViewBag.translationObjectId = translationService.GetTranslationObjectId(translationService.GetDefaultUrl());

            return View(regHistoryList);


        }

    }
}
