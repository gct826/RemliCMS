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
            ViewBag.translationObjectId = translationService.GetTranslationObjectId(translationService.GetDefaultUrl());

            var participantService = new ParticipantService();

            var participantList = participantService.ListAllParticipants();
            
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

    }
}
