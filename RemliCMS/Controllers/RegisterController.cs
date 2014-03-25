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
    public class RegisterController : BaseController
    {
        public RegisterController(IRouteService routeService) : base(routeService)
        {
        }

        //
        // GET: /Register/
        public ActionResult Index()
        {
            var registrationService = new RegistrationService();

            ViewBag.RegIsAllowed = registrationService.AllowRegistration();
            
            if (Equals(ViewBag.RegIsAllowed, false))
            {
                ViewBag.MessageEn = "Registration is currently Closed. Registration starts on Sunday 4/28th";
                ViewBag.MessageCh = "目前注册关闭. 四月二十八日（星期日）开始登记.";
            }

            var test = HttpContext.User.Identity.IsAuthenticated;

            if (test)
            {
                ViewBag.RegIsAllowed = true;
            }
    
            return View();
        }

        //
        // POST: /Register/Index
        [HttpPost]
        public ActionResult Index(FormCollection values)
        {
            var registrationService = new RegistrationService();
            var order = new Registration();

            TryUpdateModel(order);

            try
            {
                order.RegEmail = order.RegEmail.ToLower();
                order.RegPhone = new string(order.RegPhone.Where(c => char.IsDigit(c)).ToArray());

                if (registrationService.IsExistEmail(order.RegEmail))
                {
                    ViewBag.MessageEn = "Email or Phone Number already exist";
                    ViewBag.MessageCh = "电子邮件或电话号码已经存在";
                    ViewBag.RegIsAllowed = true;
                    return View(order);
                }
                else if (registrationService.IsExistPhone(order.RegPhone))
                {
                    ViewBag.MessageEn = "Email or Phone Number already exist";
                    ViewBag.MessageCh = "电子邮件或电话号码已经存在";
                    ViewBag.RegIsAllowed = true;
                    return View(order);
                }
                else
                {
                    ViewBag.Message = null;

                    order.RegId = registrationService.GetLastId() + 1;
                    order.DateCreated = DateTime.Now;
                    order.IsConfirmed = false;

                    registrationService.Update(order);

                    //EventHistory NewEvent = new EventHistory();
                    //NewEvent.AddHistory(order.RegUIDtoID(order.RegistrationUID), "New Registration Created", 0);


                    return RedirectToAction("PartEntry", "Register", new { translation = "en", regObjectId = order.Id, isPage2 = false, id = 0 });
                    //return RedirectToAction("Index");
                }
            }
            catch
            {
                return View(order);
            }

            return View(order);
        }

        //
        // GET: /Register/Modify/RegUID
        //public ActionResult Modify(string RegUID)
        //{
        //    if (RegUID == null)
        //    {
        //        ViewBag.Found = false;
        //        ViewBag.Message = RegUID;
        //        return RedirectToAction("Index", "Register");
        //    }
        //    else
        //    {
        //        RegistrationEntry FoundEntry = new RegistrationEntry();
        //        int FoundRegID = FoundEntry.RegUIDtoID(RegUID);

        //        if (FoundRegID != 0)
        //        {
        //            EventHistory NewEvent = new EventHistory();
        //            NewEvent.AddHistory(FoundRegID, "General Registration Opened", 0);

        //            ViewBag.Found = true;
        //            ViewBag.RegID = FoundRegID;
        //            return View();
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
        // GET: /Register/Summary?RegID
        //[ChildActionOnly]
        //public ActionResult Summary(int Id = 0)
        //{
        //    if (Id == 0)
        //    {
        //        ViewBag.Found = false;
        //        ViewBag.MessageEn = "Participant not found";
        //        ViewBag.MessageCh = "没有找到登记";
        //        return PartialView();
        //    }
        //    else
        //    {
        //        var registrationentry = from m in _db.RegEntries.Where(p => p.RegistrationID.Equals(Id))
        //                                select m;

        //        RegistrationEntry FoundEntry = registrationentry.FirstOrDefault();

        //        if (FoundEntry != null)
        //        {
        //            ViewBag.Found = true;
        //            ViewBag.RegUID = FoundEntry.RegistrationUID;
        //            return PartialView(registrationentry.ToList());
        //        }
        //        else
        //        {
        //            ViewBag.Found = false;
        //            ViewBag.MessageEn = "Participant not found";
        //            ViewBag.MessageCh = "没有找到登记";
        //            return PartialView();
        //        }
        //    }
        //}


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
//        // GET: /Register/Complete/RegUID
//        public ActionResult Complete(string RegUID, FormCollection values)
//        {
//            if (RegUID == null)
//            {
//                ViewBag.Found = false;
//                return View();
//            }

//            if (values.Count == 0)
//            {
//                RegistrationEntry FoundEntry = new RegistrationEntry();
//                int FoundRegID = FoundEntry.RegUIDtoID(RegUID);

//                if (FoundRegID != 0)
//                {
//                    ViewBag.Found = true;
//                    ViewBag.Scholarship = false;
//                    ViewBag.TotalPrice = FoundEntry.RegTotalPrice(FoundRegID);
//                    ViewBag.RegID = FoundRegID;
//                    ViewBag.RegUID = RegUID;

//                    return View();
//                }
//                else
//                {
//                    ViewBag.Found = false;
//                    ViewBag.MessageEn = "Invalid Registration Key";
//                    ViewBag.MessageCh = "没有找到登记";
//                    return View();
//                }
//            }

//            ViewBag.Found = false;
//            return View();
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

        //
        // GET: /Register/PartEntry/RegUID
        public ActionResult PartEntry(string regObjectId, bool isPage2 = false, bool? isAdmin = false, int partId = 0)
        {
            RouteValues routeValues = RouteValue;
            var translationService = new TranslationService();
            var translationObjectId = translationService.GetTranslationObjectId(routeValues.Translation);

            if (isAdmin == null)
            {
                isAdmin = false;
            }

            if (regObjectId == null)
            {
                ViewBag.Found = false;
                ViewBag.Message = "Invalid Registration Key";
                return RedirectToAction("Index");
            }

            var registrationService = new RegistrationService();
            var participantService = new ParticipantService();
            var regValueService = new RegValueService();

            Registration foundRegistration = registrationService.GetById(regObjectId);

            
            //RegistrationEntry FoundEntry = new RegistrationEntry();
            //int RegID = FoundEntry.RegUIDtoID(RegUID);

            if (foundRegistration == null)
            {
                ViewBag.Found = false;
                ViewBag.Message = "Invalid Registration Key";
                return RedirectToAction("Index", "Register");
            }

            if (foundRegistration.RegId != 0 && partId == 0)
            {
                ViewBag.Found = true;
                ViewBag.isNew = true;
                ViewBag.isPage2 = isPage2;
                ViewBag.RegObjectId = foundRegistration.Id;
                ViewBag.RegId = foundRegistration.RegId;
                ViewBag.SessionId = new SelectList(regValueService.GetValueTextList("sessions", translationObjectId), "Value", "Text");
                ViewBag.AgeRangeId = new SelectList(regValueService.GetValueTextList("agerange", translationObjectId), "Value", "Text");
                ViewBag.GenderId = new SelectList(regValueService.GetValueTextList("gender", translationObjectId), "Value","Text");
                ViewBag.RoomTypeID = new SelectList(regValueService.GetValueTextList("roomtype", translationObjectId), "Value", "Text");

                return View();
            }

            if (foundRegistration.RegId != 0 && partId != 0)
            {
                var foundParticipant = participantService.GetByPartId(partId);

                if (foundParticipant == null || foundParticipant.RegId != foundRegistration.RegId)
                {
                    ViewBag.Found = false;
                    ViewBag.Message = "Participant not Found!";
                    return View();
                }

                ViewBag.Found = true;
                ViewBag.isNew = false;
                ViewBag.isPage2 = isPage2;
                ViewBag.isAdmin = isAdmin;
                ViewBag.RegObjectId = foundRegistration.Id;
                ViewBag.RegId = foundRegistration.RegId;
                ViewBag.ParticipantID = foundParticipant.PartId;

                if (isPage2)
                {
                    //ViewBag.ServiceID = new SelectList(db.Services.Where(p => p.ServiceID.Equals(participantentry.FirstOrDefault().ServiceID)), "ServiceID", "Name", participantentry.FirstOrDefault().ServiceID);
                    //ViewBag.AgeRangeID = new SelectList(db.AgeRanges.Where(p => p.AgeRangeID.Equals(participantentry.FirstOrDefault().AgeRangeID)), "AgeRangeID", "Name", participantentry.FirstOrDefault().AgeRangeID);
                    //ViewBag.GenderID = new SelectList(db.Genders.Where(p => p.GenderID.Equals(participantentry.FirstOrDefault().GenderID)), "GenderID", "Name", participantentry.FirstOrDefault().GenderID);
                    //ViewBag.RegTypeID = new SelectList(db.RegTypes.Where(p => p.RegTypeID.Equals(participantentry.FirstOrDefault().RegTypeID)), "RegTypeID", "Name", participantentry.FirstOrDefault().RegTypeID);
                    //ViewBag.FellowshipID = new SelectList(db.Fellowships.Where(p => p.ServiceID.Equals(participantentry.FirstOrDefault().ServiceID)), "FellowshipID", "Name", participantentry.FirstOrDefault().FellowshipID);
                    //ViewBag.RoomTypeID = new SelectList(db.RoomTypes.Where(p => p.RegTypeID.Equals(participantentry.FirstOrDefault().RegTypeID)), "RoomTypeID", "Name", participantentry.FirstOrDefault().RoomTypeID);
                    //ViewBag.PartPrice = participantentry.FirstOrDefault().PartPrice;
                }
                else
                {
                    //ViewBag.ServiceID = new SelectList(db.Services, "ServiceID", "Name", participantentry.FirstOrDefault().ServiceID);
                    //ViewBag.AgeRangeID = new SelectList(db.AgeRanges, "AgeRangeID", "Name", participantentry.FirstOrDefault().AgeRangeID);
                    //ViewBag.GenderID = new SelectList(db.Genders, "GenderID", "Name", participantentry.FirstOrDefault().GenderID);
                    //ViewBag.RegTypeID = new SelectList(db.RegTypes, "RegTypeID", "Name", participantentry.FirstOrDefault().RegTypeID);
                }

                return View(foundParticipant);
            }

            ViewBag.Found = false;
            ViewBag.Message = "Catchall Error";
            return View();
        }

        //
        // POST: /Register/PartEntry/RegUID
        [HttpPost]
        [MultiButton(MatchFormKey = "roomnote", MatchFormValue1 = "Next", MatchFormValue2 = "下页")]
        public ActionResult PartEntry(string regObjectId, bool isPage2, bool? isAdmin, int partId, Participant participantEntry)
        {
            if (isAdmin == null)
            {
                isAdmin = false;
            }

            if (regObjectId == null)
            {
                ViewBag.Found = false;
                ViewBag.Message = "Invalid Registration Key";
                return View();
            }

            var registrationService = new RegistrationService();
            var participantService = new ParticipantService();

            Registration foundRegistration = registrationService.GetById(regObjectId);

            if (foundRegistration == null)
            {
                ViewBag.Found = false;
                ViewBag.Message = "Invalid Registration Key";
                return View();
            }

            if (foundRegistration.RegId != 0 && partId == 0)
            {
                if (ModelState.IsValid)
                {

                    participantEntry.RegId = foundRegistration.RegId;
                    participantEntry.StatusId = (int)1;
                    participantEntry.SessionId = (int)1;
                    participantEntry.RoomTypeId = (int)1;
                    participantEntry.PartPrice = (decimal)99.99;

                    participantService.Update(participantEntry);

                    //EventHistory NewEvent = new EventHistory();
                    //NewEvent.AddHistory(RegID, "New Participant Created", participantentry.ParticipantID);

                    return RedirectToAction("PartEntry", new { regObjectId = regObjectId, isPage2 = true, id = participantEntry.PartId });
                }

                ViewBag.Found = true;
                ViewBag.isNew = true;
                ViewBag.isPage2 = isPage2;
                ViewBag.RegObjectId = foundRegistration.Id;
                ViewBag.RegId = foundRegistration.RegId;

                //ViewBag.ServiceID = new SelectList(db.Services, "ServiceID", "Name");
                //ViewBag.AgeRangeID = new SelectList(db.AgeRanges, "AgeRangeID", "Name");
                //ViewBag.GenderID = new SelectList(db.Genders, "GenderID", "Name");
                //ViewBag.RegTypeID = new SelectList(db.RegTypes, "RegTypeID", "Name");

                return View();
            }

            //if (RegID != 0 && Id != 0)
            //{


            //    if (isPage2)
            //    {
            //        if (ModelState.IsValid && RegID != 0)
            //        {
            //            participantentry.RegistrationID = RegID;
            //            participantentry.ParticipantID = Id;
            //            participantentry.StatusID = (int)2;

            //            RegPrice FoundPrice = new RegPrice();
            //            participantentry.PartPrice = FoundPrice.PriceReturn(participantentry.AgeRangeID, participantentry.RegTypeID);


            //            db.Entry(participantentry).State = EntityState.Modified;
            //            db.SaveChanges();


            //            EventHistory NewEvent = new EventHistory();
            //            NewEvent.AddHistory(RegID, "Participant Confirmed", participantentry.ParticipantID);

            //            //return RedirectToAction("Modify", "Register", new { RegUID = RegUID });

            //            if (participantentry.ServiceID == (int)2 || participantentry.ServiceID == (int)4)
            //            {
            //                return RedirectToAction("HeadsetRequest", "Participant",
            //                                 new { RegUID = RegUID, id = participantentry.ParticipantID });
            //            }
            //            else
            //            {
            //                return RedirectToAction("Modify", "Register", new { RegUID = RegUID });
            //            }
            //        }

            //        ViewBag.ServiceID = new SelectList(db.Services.Where(p => p.ServiceID.Equals(participantentry.ServiceID)), "ServiceID", "Name", participantentry.ServiceID);
            //        ViewBag.AgeRangeID = new SelectList(db.AgeRanges.Where(p => p.AgeRangeID.Equals(participantentry.AgeRangeID)), "AgeRangeID", "Name", participantentry.AgeRangeID);
            //        ViewBag.GenderID = new SelectList(db.Genders.Where(p => p.GenderID.Equals(participantentry.GenderID)), "GenderID", "Name", participantentry.GenderID);
            //        ViewBag.RegTypeID = new SelectList(db.RegTypes.Where(p => p.RegTypeID.Equals(participantentry.RegTypeID)), "RegTypeID", "Name", participantentry.RegTypeID);
            //        ViewBag.FellowshipID = new SelectList(db.Fellowships.Where(p => p.ServiceID.Equals(participantentry.ServiceID)), "FellowshipID", "Name", participantentry.FellowshipID);
            //        ViewBag.RoomTypeID = new SelectList(db.RoomTypes.Where(p => p.RegTypeID.Equals(participantentry.RegTypeID)), "RoomTypeID", "Name", participantentry.RoomTypeID);
            //        ViewBag.PartPrice = participantentry.PartPrice;
            //    }
            //    else
            //    {
            //        if (ModelState.IsValid)
            //        {
            //            participantentry.RegistrationID = RegID;
            //            participantentry.ParticipantID = Id;
            //            participantentry.StatusID = (int)1;

            //            var partFellowshipList = from m in db.Fellowships.Where(p => p.FellowshipID.Equals(participantentry.FellowshipID))
            //                                     select m;
            //            Fellowship partFellowship = partFellowshipList.FirstOrDefault();

            //            try
            //            {
            //                if (partFellowship.ServiceID != participantentry.ServiceID)
            //                {
            //                    participantentry.FellowshipID = participantentry.ServiceID;
            //                }
            //            }
            //            catch
            //            {
            //            }

            //            var partRoomTypeList = from m in db.RoomTypes.Where(p => p.RoomTypeID.Equals(participantentry.RoomTypeID))
            //                                   select m;
            //            RoomType partRoomType = partRoomTypeList.FirstOrDefault();

            //            try
            //            {
            //                if (partRoomType.RegTypeID != participantentry.RegTypeID)
            //                {
            //                    participantentry.RoomTypeID = participantentry.RegTypeID;
            //                }
            //            }
            //            catch
            //            {
            //            }

            //            RegPrice FoundPrice = new RegPrice();
            //            participantentry.PartPrice = FoundPrice.PriceReturn(participantentry.AgeRangeID, participantentry.RegTypeID);

            //            db.Entry(participantentry).State = EntityState.Modified;
            //            db.SaveChanges();


            //            EventHistory NewEvent = new EventHistory();
            //            NewEvent.AddHistory(RegID, "Participant Edited", participantentry.ParticipantID);

            //            return RedirectToAction("Modify", new { RegUID = RegUID, isPage2 = true, id = participantentry.ParticipantID });
            //        }

            //        ViewBag.ServiceID = new SelectList(db.Services, "ServiceID", "Name", participantentry.ServiceID);
            //        ViewBag.AgeRangeID = new SelectList(db.AgeRanges, "AgeRangeID", "Name", participantentry.AgeRangeID);
            //        ViewBag.GenderID = new SelectList(db.Genders, "GenderID", "Name", participantentry.GenderID);
            //        ViewBag.RegTypeID = new SelectList(db.RegTypes, "RegTypeID", "Name", participantentry.RegTypeID);
            //    }

            //    ViewBag.Found = true;
            //    ViewBag.isNew = false;
            //    ViewBag.isPage2 = isPage2;
            //    ViewBag.isAdmin = isAdmin;
            //    ViewBag.RegUID = RegUID;
            //    ViewBag.RegistrationID = RegID;
            //    ViewBag.ParticipantID = participantentry.ParticipantID;

            //    return View(participantentry);
            //}

            ViewBag.Found = false;
            ViewBag.Message = "Catchall Error";
            return View();
        
        }



    }
}

    

