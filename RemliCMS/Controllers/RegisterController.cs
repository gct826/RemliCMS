using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MongoDB.Bson;
using RemliCMS.RegSystem.Entities;
using RemliCMS.RegSystem.Services;
using RemliCMS.Routes;
using RemliCMS.WebData.Services;

namespace RemliCMS.Controllers
{
    public class RegisterController : BaseController
    {
        public RegisterController(IRouteService routeService) : base(routeService)
        {
        }

        public int IsAdmin()
        {
            RouteValues routeValues = RouteValue;
            var translationService = new TranslationService();
            var translationObjectId = translationService.GetTranslationObjectId(routeValues.Translation);


            if (routeValues.Translation == "admin")
            {
                if (User.IsInRole("admin"))
                {
                    ViewBag.isAdmin = true;
                    ViewBag.translationObjectId =
                        translationService.GetTranslationObjectId(translationService.GetDefaultUrl());
                    return 1;
                }
                else
                {
                    ViewBag.isAdmin = false;
                    ViewBag.translationObjectId = translationService.GetTranslationObjectId(translationService.GetDefaultUrl());
                    return -1;
                }
            }

            ViewBag.isAdmin = false;
            ViewBag.translationObjectId = translationObjectId;
            return 0;
        }

        //
        // GET: /Register/
        public ActionResult Index()
        {
            ViewBag.Title = "Registration";

            var isAdmin = IsAdmin();
            if (isAdmin == -1)
            {
                return RedirectToAction("Index", "Register", new { translation = "en" });
            }

            var registrationService = new RegistrationService();

            var RegOn = ConfigurationManager.AppSettings["Registration"];

            if (RegOn == "on")
            {
                ViewBag.RegIsAllowed = registrationService.AllowRegistration();
            }
            else
            {
                ViewBag.RegIsAllowed = false;
            }

            if (Equals(ViewBag.RegIsAllowed, false))
            {
                ViewBag.MessageEn = "Registration is currently Closed. Registration starts on Sunday 4/28th";
                ViewBag.MessageCh = "目前注册关闭. 四月二十八日（星期日）开始登记.";
            }

            //var test = HttpContext.User.Identity.IsAuthenticated;

            //if (test)
            //{
            //    ViewBag.RegIsAllowed = true;
            //}

            return View();
        }

        //
        // POST: /Register/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection values)
        {
            ViewBag.Title = "Registration";

            var isAdmin = IsAdmin();
            if (isAdmin == -1)
            {
                return RedirectToAction("Index", "Register", new { translation = "en" });
            }

            var registrationService = new RegistrationService();
            var order = new Registration();

            TryUpdateModel(order);
            var submitType = values["action"];

            var RegOn = ConfigurationManager.AppSettings["Registration"];

            if (RegOn == "on")
            {
                ViewBag.RegIsAllowed = registrationService.AllowRegistration();
            }
            else
            {
                ViewBag.RegIsAllowed = false;
            }

            ViewBag.RegIsAllowed = registrationService.AllowRegistration();
            ViewBag.Message = submitType;

            if (submitType == "New Registration" || submitType == "创建新注册")
            {
                var newReg = new Registration();

                newReg.RegEmail = order.RegEmail.ToLower();
                newReg.RegPhone = new string(order.RegPhone.Where(c => char.IsDigit(c)).ToArray());

                if (registrationService.IsExistEmail(newReg.RegEmail))
                {
                    ViewBag.Message = "Email or Phone Number already exist.";
                    ViewBag.RegIsAllowed = true;
                    return View(order);
                }
                else if (registrationService.IsExistPhone(newReg.RegPhone))
                {
                    ViewBag.Message = "Email or Phone Number already exist.";
                    ViewBag.RegIsAllowed = true;
                    return View(order);
                }
                else
                {
                    ViewBag.Message = null;

                    newReg.RegId = registrationService.GetLastId() + 1;
                    newReg.IsConfirmed = false;
                    newReg.IsDeleted = false;

                    registrationService.Update(newReg);

                    var regHistoryService = new RegHistoryService();
                    regHistoryService.AddHistory(newReg.RegId, "Registration - Create New","", isAdmin);

                    return RedirectToAction("Participant", "Register", new {partId = 0, regObjectId = newReg.Id});
                }
            }
            if (submitType == "Open Registration" || submitType == "修改注册")
            {
                var openReg = new Registration();

                openReg.RegPhone = new string(order.RegPhone.Where(c => char.IsDigit(c)).ToArray());
                openReg.RegEmail = order.RegEmail.ToLower();

                var foundReg = registrationService.OpenReg(openReg.RegEmail, openReg.RegPhone);

                if (foundReg == null)
                {
                    ViewBag.Message = "Email or Phone Number not found.";
                    return View(order);
                }

                return RedirectToAction("Registration", "Register", new {regObjectId = foundReg.Id});

            }

            ViewBag.Message = "Error";
            return View(order);
        }

        //
        // GET: /Register/Participant?regObjectId&partId
        public ActionResult Participant(string regObjectId, int partId)
        {
            ViewBag.Title = "Participant";

            var isAdmin = IsAdmin();
            if (isAdmin == -1)
            {
                return RedirectToAction("Index", "Register", new { translation = "en" });
            }

            
            if (regObjectId == null || partId == null)
            {
                ViewBag.Found = false;
                return RedirectToAction("Index");
            }

            var registrationService = new RegistrationService();
            var participantService = new ParticipantService();
            var regValueService = new RegValueService();

            Registration foundRegistration = registrationService.GetById(regObjectId);

            if (foundRegistration == null)
            {
                ViewBag.Found = false;
                return RedirectToAction("Index");
            }

            var partFieldNameIdList = regValueService.GetValueTextList("partfieldname", ViewBag.translationObjectId);
            ViewBag.PartFieldNameId = new string[partFieldNameIdList.Count + 1];
            foreach (var item in partFieldNameIdList)
            {
                ViewBag.PartFieldNameId[item.Value] = item.Text;
            }

            if (foundRegistration.RegId != 0 && partId == 0)
            {
                ViewBag.RegObjectId = foundRegistration.Id;
                ViewBag.RegId = foundRegistration.RegId;
                ViewBag.ParticipantId = 0;
                ViewBag.SessionId = new SelectList(regValueService.GetValueTextList("sessions", ViewBag.translationObjectId),
                                                   "Value", "Text");
                ViewBag.AgeRangeId = new SelectList(regValueService.GetValueTextList("agerange", ViewBag.translationObjectId),
                                                    "Value", "Text");
                ViewBag.GenderId = new SelectList(regValueService.GetValueTextList("gender", ViewBag.translationObjectId),
                                                  "Value", "Text");
                ViewBag.RoomTypeID = new SelectList(regValueService.GetValueTextList("roomtype", ViewBag.translationObjectId),
                                                    "Value", "Text");

                return View();
            }

            var foundParticipant = participantService.GetByPartId(partId);

            if (foundParticipant == null || foundParticipant.RegId != foundRegistration.RegId)
            {
                ViewBag.Found = false;
                return RedirectToAction("Index");
            }

            ViewBag.RegObjectId = foundRegistration.Id;
            ViewBag.RegId = foundRegistration.RegId;
            ViewBag.ParticipantId = foundParticipant.PartId;
            ViewBag.SessionId = new SelectList(regValueService.GetValueTextList("sessions", ViewBag.translationObjectId),
                                               "Value", "Text");
            ViewBag.AgeRangeId = new SelectList(regValueService.GetValueTextList("agerange", ViewBag.translationObjectId),
                                                "Value", "Text");
            ViewBag.GenderId = new SelectList(regValueService.GetValueTextList("gender", ViewBag.translationObjectId), "Value",
                                              "Text");
            ViewBag.RoomTypeID = new SelectList(regValueService.GetValueTextList("roomtype", ViewBag.translationObjectId),
                                                "Value", "Text");

            var regHistoryService = new RegHistoryService();
            regHistoryService.AddHistory(foundRegistration.RegId, "Participant - Opened", foundParticipant.PartId.ToString(), isAdmin);

            return View(foundParticipant);
        }

        //
        // POST: /Register/Participant?regObjectId&partId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Participant(string regObjectId, int partId, FormCollection collection)
        {
            ViewBag.Title = "Participant";

            var isAdmin = IsAdmin();
            if (isAdmin == -1)
            {
                return RedirectToAction("Index", "Register", new { translation = "en" });
            }

            if (regObjectId == null)
            {
                ViewBag.Found = false;
                ViewBag.Message = "Invalid Registration Key";
                return View();
            }

            var registrationService = new RegistrationService();
            var participantService = new ParticipantService();
            var regValueService = new RegValueService();
            var regPriceService = new RegPriceService();
            var regHistoryService = new RegHistoryService();

            Registration foundRegistration = registrationService.GetById(regObjectId);

            if (foundRegistration == null)
            {
                ViewBag.Found = false;
                ViewBag.Message = "Invalid Registration Key";
                return View();
            }

            var partFieldNameIdList = regValueService.GetValueTextList("partfieldname", ViewBag.translationObjectId);
            ViewBag.PartFieldNameId = new string[partFieldNameIdList.Count + 1];
            foreach (var item in partFieldNameIdList)
            {
                ViewBag.PartFieldNameId[item.Value] = item.Text;
            }

            var saveParticipant = new Participant();
            TryUpdateModel(saveParticipant);

            if (foundRegistration.RegId != 0 && partId == 0)
            {
                if (ModelState.IsValid)
                {
                    saveParticipant.PartId = participantService.GetLastId() + 1;
                    saveParticipant.RegId = foundRegistration.RegId;
                    saveParticipant.StatusId = (int) 1;
                    saveParticipant.PartPrice = regPriceService.GetPrice(saveParticipant.RoomTypeId,
                                                                         saveParticipant.AgeRangeId);

                    participantService.Update(saveParticipant);

                    regHistoryService.AddHistory(foundRegistration.RegId, "Participant - Create New", saveParticipant.PartId.ToString(), isAdmin);

                    ViewBag.Message = "Saved";
                    return RedirectToAction("Registration", new {regObjectId = foundRegistration.Id});

                }

                ViewBag.RegObjectId = foundRegistration.Id;
                ViewBag.RegId = foundRegistration.RegId;

                ViewBag.SessionId = new SelectList(regValueService.GetValueTextList("sessions", ViewBag.translationObjectId),
                                                   "Value", "Text");
                ViewBag.AgeRangeId = new SelectList(regValueService.GetValueTextList("agerange", ViewBag.translationObjectId),
                                                    "Value", "Text");
                ViewBag.GenderId = new SelectList(regValueService.GetValueTextList("gender", ViewBag.translationObjectId),
                                                  "Value", "Text");
                ViewBag.RoomTypeID = new SelectList(regValueService.GetValueTextList("roomtype", ViewBag.translationObjectId),
                                                    "Value", "Text");

                ViewBag.Message = "Save Error";

                return View(saveParticipant);
            }

            var foundParticipant = participantService.GetByPartId(saveParticipant.PartId);

            if (foundParticipant.RegId == saveParticipant.RegId)
            {
                saveParticipant.Id = foundParticipant.Id;
                saveParticipant.StatusId = (int) 1;
                saveParticipant.PartPrice = regPriceService.GetPrice(saveParticipant.RoomTypeId,
                                                                     saveParticipant.AgeRangeId);

                regHistoryService.AddHistory(foundRegistration.RegId, "Participant - Update", saveParticipant.PartId.ToString(), isAdmin);

                participantService.Update(saveParticipant);
                return RedirectToAction("Registration", new {regObjectId = foundRegistration.Id});
            }

            ViewBag.Found = false;
            ViewBag.Message = "Catchall Error";

            regHistoryService.AddHistory(foundRegistration.RegId, "Participant - Post Error", saveParticipant.PartId.ToString(), isAdmin);

            return View(saveParticipant);

        }

        //
        // GET: /Register/Delete?regObjectId&partId
        public ActionResult Delete(string regObjectId, int partId)
        {
            ViewBag.Title = "Remove Participant";

            var isAdmin = IsAdmin();
            if (isAdmin == -1)
            {
                return RedirectToAction("Index", "Register", new { translation = "en" });
            }

            if (regObjectId == null || partId == null)
            {
                ViewBag.Found = false;
                return RedirectToAction("Index");
            }

            var registrationService = new RegistrationService();
            var participantService = new ParticipantService();
            var regValueService = new RegValueService();

            Registration foundRegistration = registrationService.GetById(regObjectId);

            if (foundRegistration == null)
            {
                ViewBag.Found = false;
                return RedirectToAction("Index");
            }

            var foundParticipant = participantService.GetByPartId(partId);

            if (foundParticipant == null || foundParticipant.RegId != foundRegistration.RegId)
            {
                ViewBag.Found = false;
                return RedirectToAction("Index");
            }

            ViewBag.RegObjectId = foundRegistration.Id;
            ViewBag.RegId = foundRegistration.RegId;
            ViewBag.ParticipantId = foundParticipant.PartId;

            return View(foundParticipant);
        }

        //
        // POST: /Register/Delete?regObjectId&partId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string regObjectId, int partId, FormCollection collection)
        {
            ViewBag.Title = "Remove Participant";

            var isAdmin = IsAdmin();
            if (isAdmin == -1)
            {
                return RedirectToAction("Index", "Register", new { translation = "en" });
            }

            if (regObjectId == null || partId == null)
            {
                ViewBag.Found = false;
                return RedirectToAction("Index");
            }

            var registrationService = new RegistrationService();
            var participantService = new ParticipantService();
            var regValueService = new RegValueService();

            Registration foundRegistration = registrationService.GetById(regObjectId);

            if (foundRegistration == null)
            {
                ViewBag.Found = false;
                return RedirectToAction("Index");
            }

            var foundParticipant = participantService.GetByPartId(partId);

            if (foundParticipant == null || foundParticipant.RegId != foundRegistration.RegId)
            {
                ViewBag.Found = false;
                return RedirectToAction("Index");
            }

            ViewBag.RegObjectId = foundRegistration.Id;
            ViewBag.RegId = foundRegistration.RegId;
            ViewBag.ParticipantId = foundParticipant.PartId;

            var saveParticipant = new Participant();
            TryUpdateModel(saveParticipant);

            if (foundParticipant.PartId == saveParticipant.PartId)
            {
                foundParticipant.StatusId = 4;
                participantService.Update(foundParticipant);

                var regHistoryService = new RegHistoryService();
                regHistoryService.AddHistory(foundRegistration.RegId, "Participant - Deleted", foundParticipant.PartId.ToString(), isAdmin);


                return RedirectToAction("Registration", new { regObjectId = foundRegistration.Id });
            }

            return View(foundParticipant);
        }


        //
        // GET: /Register/Registration?regObjectId&confirmation
        public ActionResult Registration(string regObjectId, bool confirmation = false)
        {
            ViewBag.Title = "Registration";

            var isAdmin = IsAdmin();
            if (isAdmin == -1)
                {
                    return RedirectToAction("Index", "Register", new {translation = "en"});                
                }
            
            if (regObjectId == null)
            {
                return RedirectToAction("Index", "Register");
            }
            ViewBag.Title = "Registration";

            var registrationService = new RegistrationService();
            var participantService = new ParticipantService();
            var foundRegistration = registrationService.GetById(regObjectId);

            if (foundRegistration != null)
            {
                registrationService.UpdateLastOpened(regObjectId);
                var regHistoryService = new RegHistoryService();
                regHistoryService.AddHistory(foundRegistration.RegId, "Registration - Opened", "", isAdmin);

                if (foundRegistration.IsConfirmed)
                {
                    confirmation = false;

                }

                var incomplete = false;

                if (confirmation)
                {
                    if (!participantService.CompleteRegistration(foundRegistration.RegId))
                    {
                        incomplete = true;
                        confirmation = false;
                    }
                }

                ViewBag.incomplete = incomplete;
                ViewBag.confirmation = confirmation;
                ViewBag.RegId = foundRegistration.RegId;
                ViewBag.RegObjectId = foundRegistration.Id;
                ViewBag.IsConfirmed = foundRegistration.IsConfirmed;
                return View();
            }
                    
            return RedirectToAction("Index", "Register");
        }

        //
        // POST: /Register/Registration?regObjectId&confirmation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(string regObjectId, bool confirmation, FormCollection collection)
        {
            ViewBag.Title = "Registration";

            var isAdmin = IsAdmin();
            if (isAdmin == -1)
            {
                return RedirectToAction("Index", "Register", new { translation = "en" });
            }

            if (regObjectId == null)
            {
                return RedirectToAction("Index", "Register");
            }

            var registrationService = new RegistrationService();
            var foundRegistration = registrationService.GetById(regObjectId);
            var regHistoryService = new RegHistoryService();

            var saveRegistration = new Registration();
            TryUpdateModel(saveRegistration);

            if (foundRegistration.RegId == saveRegistration.RegId)
            {
                var totalPrice = registrationService.getTotalPrice(foundRegistration.RegId);
                
                foundRegistration.IsConfirmed = true;
                registrationService.Update(foundRegistration);

                regHistoryService.AddHistory(foundRegistration.RegId, "Registration - Confirmed", "", isAdmin);

                var ledgerService = new LedgerService();
                var newLedger = new Ledger
                    {
                        RegId = foundRegistration.RegId,
                        LedgerTypeId = (int) 1,
                        LedgerAmount = totalPrice,
                        LedgerDate = DateTime.Now,
                        IsConfirmed = true
                    };

                ledgerService.Update(newLedger);

                regHistoryService.AddHistory(foundRegistration.RegId, "Payment Entry - " + newLedger.LedgerTypeId, String.Format("{0:C}", newLedger.LedgerAmount), isAdmin);

                return RedirectToAction("Registration", new { regObjectId = foundRegistration.Id });
            }

            return RedirectToAction("Registration", new { regObjectId = foundRegistration.Id });

        }

        //
        // GET: /Register/Summary?RegID
        [ChildActionOnly]
        public ActionResult Summary(string transObjectId, int regId = 0, bool isAdmin = false)
        {

            if (regId == 0)
            {
                ViewBag.Found = false;
                ViewBag.Message = "Participant not found";
                ViewBag.MessageCh = "没有找到登记";
                return PartialView();
            }

            var registrationService = new RegistrationService();
            var regValueService = new RegValueService();
            var foundRegEntry = registrationService.GetByRegId(regId);

            if (foundRegEntry != null)
            {
                var regFieldNameIdList = regValueService.GetValueTextList("regfieldname", new ObjectId(transObjectId));
                ViewBag.regFieldNameId = new string[regFieldNameIdList.Count + 1];
                foreach (var item in regFieldNameIdList)
                {
                    ViewBag.regFieldNameId[item.Value] = item.Text;
                }


                ViewBag.Found = true;
                ViewBag.isAdmin = isAdmin;
                ViewBag.RegUID = foundRegEntry.Id;
                return PartialView(foundRegEntry);
            }

            ViewBag.Found = false;
            ViewBag.Message = "Participant not found";
            ViewBag.MessageCh = "没有找到登记";
            return PartialView();
        }

        //
        // GET: /Register/SummaryHelper?RegID&isAdmin
        [ChildActionOnly]
        public ActionResult SummaryHelper(string translationObjectId, int regId, bool isAdmin, int partId)
        {
            var transObjectId = new ObjectId(translationObjectId);

            if (regId == 0)
            {
                return RedirectToAction("Index", "Register");
            }

            var registrationService = new RegistrationService();
            var foundRegEntry = registrationService.GetByRegId(regId);

            if (foundRegEntry == null)
            {
                return RedirectToAction("Index", "Register");
            }

            var participantService = new ParticipantService();
            var participantList = participantService.GetParticipantList(regId);

            var totalCost = registrationService.getTotalPrice(regId);

            ViewBag.TranslationObjectId = translationObjectId;
            ViewBag.isAdmin = isAdmin;
            ViewBag.RegID = foundRegEntry.RegId;
            ViewBag.RegObjectID = foundRegEntry.Id;
            ViewBag.RegIsConfirm = foundRegEntry.IsConfirmed;
            ViewBag.TotalCost = totalCost;


            var regValueService = new RegValueService();

            var partFieldNameIdList = regValueService.GetValueTextList("partfieldname", transObjectId);
            ViewBag.PartFieldNameId = new string[partFieldNameIdList.Count + 1];
            foreach (var item in partFieldNameIdList)
            {
                ViewBag.PartFieldNameId[item.Value] = item.Text;
            }

            var statusIdList = regValueService.GetValueTextList("status", transObjectId);
            ViewBag.StatusId = new string[statusIdList.Count+1];
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

            var sessionIdList = regValueService.GetValueTextList("sessions", transObjectId);
            ViewBag.SessionId = new string[sessionIdList.Count + 1];
            foreach (var item in sessionIdList)
            {
                ViewBag.SessionId[item.Value] = item.Text;
            }

            var roomTypeIdList = regValueService.GetValueTextList("roomtype", transObjectId);
            ViewBag.RoomTypeId = new string[roomTypeIdList.Count + 1];
            foreach (var item in roomTypeIdList)
            {
                ViewBag.RoomTypeId[item.Value] = item.Text;
            }

            if (partId != 0)
            {
                var returnList = participantList.Where(p => p.PartId.Equals(partId));
                if (returnList.Count() == 0)
                {
                    return RedirectToAction("Index", "Register");
                }
                else
                {
                    ViewBag.RegIsConfirm = true; //hides the buttons
                    return PartialView(returnList);
                }
            }

            if (isAdmin)
            {
                return PartialView(participantList);
            }
            else
            {
                return PartialView(participantList.Where(p => p.StatusId != 4));
            }
        }

        //
        // GET: /Register/LedgerHelper?RegId&isAdmin
        [ChildActionOnly]
        public ActionResult LedgerHelper(string translationObjectId, int regId = 0)
        {
            if (regId == 0)
            {
                return RedirectToAction("Index", "Register");
            }
            
            var registrationService = new RegistrationService();
            var foundRegEntry = registrationService.GetByRegId(regId);

            if (foundRegEntry == null)
            {
                return RedirectToAction("Index", "Register");
            }

            var ledgerService = new LedgerService();
            var ledgerList = ledgerService.GetLedgerList(regId);

            ViewBag.TranslationObjectId = translationObjectId;
            ViewBag.RegObjectId = foundRegEntry.Id;
            ViewBag.RemainingPrice = ledgerService.GetRemaining(regId);

            return PartialView(ledgerList);
        }

        //
        // GET: /Register/RegValueText
        [ChildActionOnly]
        public ActionResult RegValueText(string translationObjectId, string regValueKey, int regValue)
        {
            var regValueService = new RegValueService();

            var transObjectId = new ObjectId(translationObjectId);
            
            var foundtext = regValueService.GetValueText(transObjectId,regValueKey,regValue);

            if (foundtext == null)
            {
                ViewBag.Text = "";
                return PartialView();
            }

            ViewBag.Text = foundtext;
            return PartialView();

        }

        //
        // GET: /Register/Scholarship/RegUID
        public ActionResult Scholarship(string regObjectId)
        {
            ViewBag.Title = "Scholorship";

            var isAdmin = IsAdmin();
            if (isAdmin == -1)
            {
                return RedirectToAction("Index", "Register", new { translation = "en" });
            }

            if (regObjectId == null)
            {
                return RedirectToAction("Index", "Register");
            }

            var registrationService = new RegistrationService();
            var ledgerService = new LedgerService();

            var foundRegEntry = registrationService.GetById(regObjectId);

            if (foundRegEntry == null)
            {
                return RedirectToAction("Index", "Register");
            }

            if (foundRegEntry.RegId != 0)
            {
                ViewBag.TotalPrice = registrationService.getTotalPrice(foundRegEntry.RegId);
                ViewBag.RemainingPrice = ledgerService.GetRemaining(foundRegEntry.RegId);
                ViewBag.RegId = foundRegEntry.RegId;
                ViewBag.RegObjectId = foundRegEntry.Id;

                return View();
            }

            return RedirectToAction("Index", "Register");

        }

        //
        // POST: /Register/Scholorship/RegUID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Scholarship(string regObjectId, FormCollection submitLedger)
        {
            ViewBag.Title = "Scholorship";

            var isAdmin = IsAdmin();
            if (isAdmin == -1)
            {
                return RedirectToAction("Index", "Register", new { translation = "en" });
            }
            if (regObjectId == null)
            {
                return RedirectToAction("Index", "Register");
            }

            var registrationService = new RegistrationService();
            var foundRegEntry = registrationService.GetById(regObjectId);

            if (foundRegEntry == null)
            {
                return RedirectToAction("Index", "Register");
            }

            var saveLedger = new Ledger();
            TryUpdateModel(saveLedger);

            var ledgerService = new LedgerService();
            var newLedger = new Ledger
            {
                RegId = foundRegEntry.RegId,
                LedgerTypeId = (int)2,
                LedgerAmount = saveLedger.LedgerAmount,
                LedgerDate = DateTime.Now,
                LedgerNote = "",
                IsConfirmed = false,
                IsCancelled = false
            };

            ledgerService.Update(newLedger);

            var regHistoryService = new RegHistoryService();
            regHistoryService.AddHistory(foundRegEntry.RegId, "Payment Entry - " + newLedger.LedgerTypeId, String.Format("{0:C}", newLedger.LedgerAmount), isAdmin);

            return RedirectToAction("Registration", new { regObjectId = foundRegEntry.Id });
            
        }

        //
        // GET: /Register/Payment/RegUID
        public ActionResult Payment(string regObjectId)
        {
            ViewBag.Title = "Payment Entry";

            var isAdmin = IsAdmin();
            if (isAdmin == -1)
            {
                return RedirectToAction("Index", "Register", new { translation = "en" });
            }

            if (regObjectId == null)
            {
                return RedirectToAction("Index", "Register");
            }

            var registrationService = new RegistrationService();
            var ledgerService = new LedgerService();
            var regValueService = new RegValueService();

            var foundRegEntry = registrationService.GetById(regObjectId);

            if (foundRegEntry == null)
            {
                return RedirectToAction("Index", "Register");
            }

            if (isAdmin == 0)
            {
                return RedirectToAction("Registration", new { regObjectId = foundRegEntry.Id });
            }

            if (foundRegEntry.RegId != 0)
            {
                ViewBag.TotalPrice = registrationService.getTotalPrice(foundRegEntry.RegId);
                ViewBag.RemainingPrice = ledgerService.GetRemaining(foundRegEntry.RegId);
                ViewBag.RegId = foundRegEntry.RegId;
                ViewBag.RegObjectId = foundRegEntry.Id;
                ViewBag.LedgerId = new SelectList(regValueService.GetValueTextList("ledger", ViewBag.translationObjectId),
                                   "Value", "Text");

                return View();
            }

            return RedirectToAction("Index", "Register");

        }

        //
        // POST: /Register/Payment/RegUID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(string regObjectId, FormCollection submitLedger)
        {
            ViewBag.Title = "Payment Entry";

            var isAdmin = IsAdmin();
            if (isAdmin == -1)
            {
                return RedirectToAction("Index", "Register", new { translation = "en" });
            }
            if (regObjectId == null)
            {
                return RedirectToAction("Index", "Register");
            }

            var registrationService = new RegistrationService();
            var foundRegEntry = registrationService.GetById(regObjectId);

            if (foundRegEntry == null)
            {
                return RedirectToAction("Index", "Register");
            }

            if (isAdmin == 0)
            {
                return RedirectToAction("Registration", new { regObjectId = foundRegEntry.Id });
            }

            var saveLedger = new Ledger();
            TryUpdateModel(saveLedger);

            var ledgerService = new LedgerService();
            var newLedger = new Ledger
            {
                RegId = foundRegEntry.RegId,
                LedgerTypeId = saveLedger.LedgerTypeId,
                LedgerAmount = saveLedger.LedgerAmount,
                LedgerDate = DateTime.Now,
                LedgerNote = saveLedger.LedgerNote,
                IsConfirmed = false,
                IsCancelled = false
            };

            ledgerService.Update(newLedger);

            var regHistoryService = new RegHistoryService();
            regHistoryService.AddHistory(foundRegEntry.RegId, "Payment Entry - " + newLedger.LedgerTypeId, String.Format("{0:C}", newLedger.LedgerAmount), isAdmin);

            return RedirectToAction("Registration", new { regObjectId = foundRegEntry.Id });

        }

        //
        // GET: /Register/EnterPayment/RegUID
        public ActionResult EnterPayment(string regObjectId)
        {
            ViewBag.Title = "Enter Payment";

            var isAdmin = IsAdmin();
            if (isAdmin == -1)
            {
                return RedirectToAction("Index", "Register", new { translation = "en" });
            }

            if (regObjectId == null)
            {
                return RedirectToAction("Index", "Register");
            }

            var registrationService = new RegistrationService();
            var ledgerService = new LedgerService();

            var foundRegEntry = registrationService.GetById(regObjectId);

            if (foundRegEntry == null)
            {
                return RedirectToAction("Index", "Register");
            }

            if (foundRegEntry.RegId != 0)
            {
                ViewBag.TotalPrice = registrationService.getTotalPrice(foundRegEntry.RegId);
                ViewBag.RemainingPrice = ledgerService.GetRemaining(foundRegEntry.RegId);
                ViewBag.RegId = foundRegEntry.RegId;
                ViewBag.RegObjectId = foundRegEntry.Id;

                return View();
            }

            return RedirectToAction("Index", "Register");

        }

        //
        // POST: /Register/EnterPayment/RegUID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EnterPayment(string regObjectId, FormCollection submitLedger)
        {
            ViewBag.Title = "Enter Payment";

            var isAdmin = IsAdmin();
            if (isAdmin == -1)
            {
                return RedirectToAction("Index", "Register", new { translation = "en" });
            }
            if (regObjectId == null)
            {
                return RedirectToAction("Index", "Register");
            }

            var registrationService = new RegistrationService();
            var foundRegEntry = registrationService.GetById(regObjectId);

            if (foundRegEntry == null)
            {
                return RedirectToAction("Index", "Register");
            }

            var saveLedger = new Ledger();
            TryUpdateModel(saveLedger);

            var ledgerService = new LedgerService();
            var newLedger = new Ledger
            {
                RegId = foundRegEntry.RegId,
                LedgerTypeId = (int)8,
                LedgerAmount = saveLedger.LedgerAmount,
                LedgerDate = DateTime.Now,
                LedgerNote = "",
                IsConfirmed = false,
                IsCancelled = true
            };

            ledgerService.Update(newLedger);

            var regHistoryService = new RegHistoryService();
            regHistoryService.AddHistory(foundRegEntry.RegId, "PayPal Entry - Entered", String.Format("{0:C}", newLedger.LedgerAmount), isAdmin);

            RouteValues routeValues = RouteValue;


            string itemName = "2014 Summer Conference";

            return RedirectToAction("ValidateCommand","Paypal", new {itemName = itemName, amount = newLedger.LedgerAmount, regObjectId = regObjectId });

        }

        //
        // POST: /Register/Unlock/RegUID
        //public ActionResult Unlock(string RegUID, bool isAdmin = false)
        //{
        //    if (RegUID == null)
        //    {
        //        ViewBag.Found = false;
        //        ViewBag.Message = RegUID;
        //        return View();
        //    }
        //    else
        //    {
        //        RegistrationEntry FoundEntry = new RegistrationEntry();
        //        int FoundRegID = FoundEntry.RegUIDtoID(RegUID);

        //        if (FoundRegID != 0)
        //        {
        //            var registrationentry = from m in _db.RegEntries.Where(p => p.RegistrationID.Equals(FoundRegID))
        //                                    select m;

        //            ViewBag.Found = true;

        //            FoundEntry = registrationentry.SingleOrDefault();
        //            FoundEntry.IsConfirmed = false;

        //            _db.Entry(FoundEntry).State = EntityState.Modified;

        //            var PartEntry = from m in _db.ParticipantEntries.Include(p => p.RegistrationEntries).
        //                           Include(p => p.Statuses).Include(p => p.Services).Include(p => p.AgeRanges).
        //                           Include(p => p.Genders).Include(p => p.RegTypes).Include(p => p.Fellowships).Include(p => p.RoomTypes).
        //                           Where(p => p.RegistrationID.Equals(FoundEntry.RegistrationID)).Where(p => !p.StatusID.Equals((int)4))
        //                            select m;

        //            foreach (ParticipantEntry FoundPart in PartEntry)
        //            {
        //                FoundPart.StatusID = (int)2;
        //                _db.Entry(FoundPart).State = EntityState.Modified;
        //            }
        //            _db.SaveChanges();

        //            if (isAdmin)
        //            {
        //                EventHistory NewEvent = new EventHistory();
        //                NewEvent.AddHistory(FoundRegID, "Admin Registration Unlocked", 0);

        //                return RedirectToAction("Detail", "SearchRegistration", new { Id = FoundRegID });
        //            }
        //            else
        //            {
        //                EventHistory NewEvent = new EventHistory();
        //                NewEvent.AddHistory(FoundRegID, "General Registration Unlocked", 0);

        //                return RedirectToAction("Modify", "Register", new { RegUID = RegUID });
        //            }
        //        }
        //        else
        //        {
        //            ViewBag.Found = false;
        //            ViewBag.MessageEn = "Invalid Registration Key";
        //            ViewBag.MessageCh = "没有找到登记";
        //            return View();
        //        }
        //    }
        //}

        //
        // POST: /Register/Lock/RegUID
//        public ActionResult Lock(string RegUID, bool isAdmin = false)
//        {
//            if (RegUID == null)
//            {
//                ViewBag.Found = false;
//                ViewBag.Message = RegUID;
//                return View();
//            }
//            else
//            {
//                RegistrationEntry FoundEntry = new RegistrationEntry();
//                int FoundRegID = FoundEntry.RegUIDtoID(RegUID);

//                if (FoundRegID != 0)
//                {
//                    var registrationentry = from m in _db.RegEntries.Where(p => p.RegistrationID.Equals(FoundRegID))
//                                            select m;

//                    ViewBag.Found = true;

//                    FoundEntry = registrationentry.SingleOrDefault();

//                    var PartEntry = from m in _db.ParticipantEntries.Include(p => p.RegistrationEntries).
//                                   Include(p => p.Statuses).Include(p => p.Services).Include(p => p.AgeRanges).
//                                   Include(p => p.Genders).Include(p => p.RegTypes).Include(p => p.Fellowships).Include(p => p.RoomTypes).
//                                   Where(p => p.RegistrationID.Equals(FoundEntry.RegistrationID)).Where(p => !p.StatusID.Equals((int)4))
//                                    select m;

//                    foreach (ParticipantEntry FoundPart in PartEntry)
//                    {
//                        FoundPart.StatusID = (int)3;
//                        _db.Entry(FoundPart).State = EntityState.Modified;
//                    }

//                    FoundEntry.IsConfirmed = true;

//                    _db.Entry(FoundEntry).State = EntityState.Modified;
//                    _db.SaveChanges();

//                    if (isAdmin)
//                    {
//                        EventHistory NewEvent = new EventHistory();
//                        NewEvent.AddHistory(FoundRegID, "Admin Registration Locked", 0);

//                        return RedirectToAction("Detail", "SearchRegistration", new { Id = FoundRegID });
//                    }
//                    else
//                    {
//                        EventHistory NewEvent = new EventHistory();
//                        NewEvent.AddHistory(FoundRegID, "General Registration Locked", 0);

//                        return RedirectToAction("Complete", "Register", new { RegUID = RegUID });
//                    }
//                }
//                else
//                {
//                    ViewBag.Found = false;
//                    ViewBag.MessageEn = "Invalid Registration Key";
//                    ViewBag.MessageCh = "没有找到登记";
//                    return View();
//                }
//            }
//        }

//        //
//        // POST: /Register/Search
//        [HttpPost]
//        [MultiButton(MatchFormKey = "action", MatchFormValue1 = "Open Registration", MatchFormValue2 = "修改注册")]
//        public ActionResult Search(FormCollection values)
//        {
//            var order = new RegistrationEntry();

//            TryUpdateModel(order);

//            try
//            {
//                order.Email = order.Email.ToLower();
//                order.Phone = new string(order.Phone.Where(c => char.IsDigit(c)).ToArray());

//                if (order.Email != null && order.Phone != null)
//                {
//                    var registers = from m in _db.RegEntries
//                                    select m;
                    
//                    registers = registers.Where(s => s.Email.Equals(order.Email) && s.Phone.Equals(order.Phone));

//                    RegistrationEntry FoundReg = registers.FirstOrDefault();
//                    if (FoundReg == null)
//                    {
//                        ViewBag.MessageEn = "Registration Not Found";
//                        ViewBag.MessageCh = "没有找到登记";
//                        ViewBag.RegIsAllowed = true;
//                        return View();
//                    }
//                    else
//                    {
//                        string RegKey = FoundReg.RegistrationUID;

//                        return RedirectToAction("Modify", "Register", new { RegUID = RegKey });
//                    }
//                }

//                else
//                {
//                    return View();
//                }
//            }
//            catch
//            {
//                return View(order);
//            }

//        }


//        //
//        // POST: /Register/Complete/RegUID
//        [HttpPost]
//        public ActionResult Complete(string RegUID, PaymentEntry values)
//        {
//            if (RegUID == null)
//            {
//                ViewBag.Found = false;
//                return View();
//            }

//            RegistrationEntry FoundEntry = new RegistrationEntry();
//            int FoundRegID = FoundEntry.RegUIDtoID(RegUID);

//            if (values.RegID == 0 && values.PaymentAmt == (decimal)0)
//            {
//                ViewBag.Found = true;
//                ViewBag.Scholarship = true;
//                ViewBag.TotalPrice = FoundEntry.RegTotalPrice(FoundRegID);
//                ViewBag.RegID = FoundRegID;
//                ViewBag.RegUID = RegUID;
//                values.RegID = (int)FoundRegID;
//                values.PaymentDate = DateTime.Now;
//                values.PmtTypeID = (int)1;
//                values.PmtStatusID = (int)1;

//                return View(values);
//            }

//            if (values.RegID == FoundRegID && values.PaymentAmt <= (decimal)0)
//            {
//                ViewBag.Found = true;
//                ViewBag.Scholarship = true;
//                ViewBag.TotalPrice = FoundEntry.RegTotalPrice(FoundRegID);
//                ViewBag.RegID = FoundRegID;
//                ViewBag.RegUID = RegUID;
//                ViewBag.MessageEn = "Please enter an Amount greater then 0";
//                ViewBag.MessageCh = "请输入一个数目大于0";
//                values.RegID = (int)FoundRegID;
//                values.PaymentDate = DateTime.Now;
//                values.PmtTypeID = (int)1;
//                values.PmtStatusID = (int)1;

//                return View(values);
//            }

//            if (values.RegID == FoundRegID && values.PaymentAmt > (decimal)0)
//            {
//                values.RegID = (int)FoundRegID;
//                values.PaymentDate = DateTime.Now;
//                values.PmtTypeID = (int)1;
//                values.PmtStatusID = (int)1;

//                _db.PaymentEntries.Add(values);
//                _db.SaveChanges();

//                EventHistory NewEvent = new EventHistory();
//                NewEvent.AddHistory(values.RegID, "Scholarship Request Entered", values.PaymentID);

//                return RedirectToAction("Modify", "Register", new { RegUID = RegUID });
//            }

//            ViewBag.Found = true;
//            ViewBag.Scholarship = false;
//            ViewBag.TotalPrice = FoundEntry.RegTotalPrice(FoundRegID);
//            ViewBag.RegID = FoundRegID;

//            return View();

//        }


//        //
//        // GET: /Register/Scholarship/RegUID

//        //public ActionResult Scholarship(string RegUID)
//        //{
//        //    {
//        //        if (RegUID == null)
//        //        {
//        //            ViewBag.Found = false;
//        //            return View();
//        //        }
//        //        else
//        //        {
//        //            RegistrationEntry FoundEntry = new RegistrationEntry();
//        //            int FoundRegID = FoundEntry.RegUIDtoID(RegUID);

//        //            if (FoundRegID != 0)
//        //            {
//        //                ViewBag.Found = true;
//        //                ViewBag.TotalPrice = FoundEntry.RegTotalPrice(FoundRegID);
//        //                ViewBag.RegID = FoundRegID;

//        //                return View();
//        //            }
//        //            else
//        //            {
//        //                ViewBag.Found = false;
//        //                ViewBag.MessageEn = "Invalid Registration Key";
//        //                ViewBag.MessageCh = "没有找到登记";
//        //                return View();
//        //            }
//        //        }
//        //    }
//        //}

//        //
//        // GET: /Register/PaymentSummary/ID
//        [ChildActionOnly]
//        public ActionResult PaymentSummary(int ID = 0, bool isSummary=false)
//        {
//            if (ID == 0)
//            {
//                ViewBag.Found = false;
//                return PartialView();
//            }

//            var RegEntry = _db.RegEntries.Where(s => s.RegistrationID.Equals(ID));

//            if (RegEntry.FirstOrDefault() != null)
//            {
//                RegistrationEntry FoundEntries = RegEntry.FirstOrDefault();

//                decimal totalRegPrice = FoundEntries.RegTotalPrice(ID);

//                var PaymentEntries = from m in _db.PaymentEntries.Where(p => p.RegID.Equals(ID))
//                                     select m;

//                decimal totalScholarshipPending = (decimal)0;
//                decimal totalScholarshipApproved = (decimal)0;
//                decimal totalCashRecieved = (decimal)0;
//                decimal totalCheckPending = (decimal)0;
//                decimal totalCheckApproved = (decimal)0;
//                decimal totalRefundPending = (decimal)0;
//                decimal totalRefundApproved = (decimal)0;
//                decimal totalAdjustment = (decimal)0;
//                decimal totalCreditCard = (decimal)0;

//                foreach (var item in PaymentEntries)
//                {
//                    if (item.PmtTypeID == 1 && item.PmtStatusID == 1)
//                    {
//                        totalScholarshipPending = totalScholarshipPending + item.PaymentAmt;
//                    }

//                    if (item.PmtTypeID == 1 && item.PmtStatusID == 2)
//                    {
//                        totalScholarshipApproved = totalScholarshipApproved + item.PaymentAmt;
//                    }

//                    if (item.PmtTypeID == 2 && item.PmtStatusID != 3)
//                    {
//                        totalCashRecieved = totalCashRecieved + item.PaymentAmt;
//                    }

//                    if (item.PmtTypeID == 3 && item.PmtStatusID == 1)
//                    {
//                        totalCheckPending = totalCheckPending + item.PaymentAmt;
//                    }

//                    if (item.PmtTypeID == 3 && item.PmtStatusID == 2)
//                    {
//                        totalCheckApproved = totalCheckApproved + item.PaymentAmt;
//                    }

//                    if (item.PmtTypeID == 4 && item.PmtStatusID == 1)
//                    {
//                        totalRefundPending = totalRefundPending + item.PaymentAmt;
//                    }

//                    if (item.PmtTypeID == 4 && item.PmtStatusID == 2)
//                    {
//                        totalRefundApproved = totalRefundApproved + item.PaymentAmt;
//                    }

//                    if (item.PmtTypeID == 5 && item.PmtStatusID != 3)
//                    {
//                        totalAdjustment = totalAdjustment + item.PaymentAmt;
//                    }

//                    if (item.PmtTypeID == 6 && item.PmtStatusID != 3)
//                    {
//                        totalCreditCard = totalCreditCard + item.PaymentAmt;
//                    }
//                }

//                ViewBag.Found = true;
//                ViewBag.isSummary = isSummary;
//                ViewBag.totalRegPrice = totalRegPrice;
//                ViewBag.totalScholarshipPending = totalScholarshipPending;
//                ViewBag.totalScholarshipApproved = totalScholarshipApproved;
//                ViewBag.totalCashRecieved = totalCashRecieved;
//                ViewBag.totalCheckPending = totalCheckPending;
//                ViewBag.totalCheckApproved = totalCheckApproved;
//                ViewBag.totalRefundPending = totalRefundPending;
//                ViewBag.totalRefundApproved = totalRefundApproved;
//                ViewBag.totalAdjustment = totalAdjustment;
//                ViewBag.totalCreditCard = totalCreditCard;
//                ViewBag.totalRemaining = totalRegPrice - totalScholarshipPending - totalScholarshipApproved - totalCashRecieved - totalCheckPending - totalCheckApproved - totalRefundPending - totalRefundApproved - totalAdjustment - totalCreditCard;
//                return PartialView(PaymentEntries);

//            }

//            ViewBag.Found = false;
//            return PartialView();

//        }

//        //
//        // GET: /Register/Edit/RegUID
//        public ActionResult Edit(string RegUID)
//        {
//            if (RegUID == null)
//            {
//                ViewBag.Found = false;
//                ViewBag.Message = RegUID;
//                return View();
//            }
//            else
//            {
//                var registrationentry = from m in _db.RegEntries.Where(p => p.RegistrationUID.Equals(RegUID))
//                                        select m;

//                RegistrationEntry FoundEntry = registrationentry.FirstOrDefault();

//                ViewBag.Found = true;
//                ViewBag.RegUID = FoundEntry.RegistrationUID;
//                return View(FoundEntry);

//            }
//        }

//        //
//        // POST: /Register/Edit/RegUID
//        [HttpPost]
//        public ActionResult Edit(string RegUID, RegistrationEntry RegEntry)
//        {
//            if (RegUID == null)
//            {
//                ViewBag.Found = false;
//                ViewBag.Message = RegUID;
//                return View();
//            }
//            else
//            {

//                RegEntry.Email = RegEntry.Email.ToLower();
//                RegEntry.Phone = new string(RegEntry.Phone.Where(c => char.IsDigit(c)).ToArray());

//                var registrationentry = from m in _db.RegEntries.Where(p => p.RegistrationUID.Equals(RegUID))
//                                        select m;

//                RegistrationEntry FoundEntry = registrationentry.FirstOrDefault();

//                var searchemail = from m in _db.RegEntries.Where(p => p.Email.Equals(RegEntry.Email))
//                                  select m;

//                RegistrationEntry SearchEmail = searchemail.FirstOrDefault();

//                var searchphone = from m in _db.RegEntries.Where(p => p.Phone.Equals(RegEntry.Phone))
//                                  select m;

//                RegistrationEntry SearchPhone = searchphone.FirstOrDefault();

//                if (FoundEntry != null && SearchEmail != null)
//                {
//                    if (FoundEntry.RegistrationID != SearchEmail.RegistrationID)
//                    {
//                        ViewBag.Found = true;
//                        ViewBag.RegUID = FoundEntry.RegistrationUID;
//                        ViewBag.MessageEn = "Email already exist";
//                        ViewBag.MessageCh = "电子邮件已经存在";
//                        return View(RegEntry);
//                    }
//                }

//                if (FoundEntry != null && SearchPhone != null)
//                {
//                    if (FoundEntry.RegistrationID != SearchPhone.RegistrationID)
//                    {
//                        ViewBag.Found = true;
//                        ViewBag.RegUID = FoundEntry.RegistrationUID;
//                        ViewBag.MessageEn = "Phone Number already exist";
//                        ViewBag.MessageCh = "电话号码已经存在";
//                        return View(RegEntry);
//                    }
//                }

//                FoundEntry.Email = RegEntry.Email;
//                FoundEntry.Phone = RegEntry.Phone;

//                _db.Entry(FoundEntry).State = EntityState.Modified;
//                _db.SaveChanges();

//                EventHistory NewEvent = new EventHistory();
//                NewEvent.AddHistory(FoundEntry.RegistrationID, "Registration LogIn Changed", 0);

//                return RedirectToAction("Modify", "Register", new { RegUID = FoundEntry.RegistrationUID });

//            }
//        }

    }
}

    

