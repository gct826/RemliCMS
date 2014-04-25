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

        //
        // GET: /RegAdmin/CheckIn
        public ActionResult CheckIn(int regId = 0, int partId = 0)
        {
        
            if (regId == 0)
            {
                return RedirectToAction("Index");
            }

            var registrationService = new RegistrationService();
            var regHistoryService = new RegHistoryService();
            var participantService = new ParticipantService();            
            
            var foundRegistration = registrationService.GetByRegId(regId);
            
            if (foundRegistration == null)
            {
                return RedirectToAction("Index");
            }

            var participantList = participantService.GetParticipantList(regId);

            var isValid = false;

            if (partId != 0)
            {
                foreach (var participant in participantList)
                {
                    if (partId == participant.PartId && participant.StatusId != 4)
                    {
                        participant.StatusId = 2;
                        participantService.Update(participant);
                        regHistoryService.AddHistory(participant.RegId, "Participant Check In", participant.PartId.ToString(), 1);
                    }
                }
            }

            if (partId == 0)
            {
                foreach (var participant in participantList)
                {
                    if (participant.StatusId == 1)
                    {
                        participant.StatusId = 2;
                        participantService.Update(participant);
                        regHistoryService.AddHistory(participant.RegId, "Participant Check In", participant.PartId.ToString(), 1);
                    }
                }
            }


        return RedirectToAction("Registration", "Register", new { regObjectId = foundRegistration.Id });

        }

        //
        // GET: /RegAdmin/CheckOut
        public ActionResult CheckOut(int regId = 0, int partId = 0)
        {

            if (regId == 0)
            {
                return RedirectToAction("Index");
            }

            var registrationService = new RegistrationService();
            var regHistoryService = new RegHistoryService();
            var participantService = new ParticipantService();

            var foundRegistration = registrationService.GetByRegId(regId);

            if (foundRegistration == null)
            {
                return RedirectToAction("Index");
            }

            var participantList = participantService.GetParticipantList(regId);

            var isValid = false;

            if (partId != 0)
            {
                foreach (var participant in participantList)
                {
                    if (partId == participant.PartId && participant.StatusId != 4)
                    {
                        participant.StatusId = 3;
                        participantService.Update(participant);
                        regHistoryService.AddHistory(participant.RegId, "Participant Check Out", participant.PartId.ToString(), 1);
                    }
                }
            }

            if (partId == 0)
            {
                foreach (var participant in participantList)
                {
                    if (participant.StatusId == 2)
                    {
                        participant.StatusId = 3;
                        participantService.Update(participant);
                        regHistoryService.AddHistory(participant.RegId, "Participant Check Out", participant.PartId.ToString(), 1);
                    }
                }
            }

            return RedirectToAction("Registration", "Register", new { regObjectId = foundRegistration.Id });
        }

        //
        // GET: /RegAdmin/UndoCheckIn
        public ActionResult UndoCheckIn(int regId = 0, int partId = 0)
        {

            if (regId == 0)
            {
                return RedirectToAction("Index");
            }

            var registrationService = new RegistrationService();
            var regHistoryService = new RegHistoryService();
            var participantService = new ParticipantService();

            var foundRegistration = registrationService.GetByRegId(regId);

            if (foundRegistration == null)
            {
                return RedirectToAction("Index");
            }

            var participantList = participantService.GetParticipantList(regId);

            var isValid = false;

            if (partId != 0)
            {
                foreach (var participant in participantList)
                {
                    if (partId == participant.PartId && participant.StatusId != 4)
                    {
                        participant.StatusId = 1;
                        participantService.Update(participant);
                        regHistoryService.AddHistory(participant.RegId, "Participant Undo Check In", participant.PartId.ToString(), 1);
                    }
                }
            }

            if (partId == 0)
            {
                foreach (var participant in participantList)
                {
                    if (participant.StatusId == 2)
                    {
                        participant.StatusId = 1;
                        participantService.Update(participant);
                        regHistoryService.AddHistory(participant.RegId, "Participant Undo Check In", participant.PartId.ToString(), 1);
                    }
                }
            }

            return RedirectToAction("Registration", "Register", new { regObjectId = foundRegistration.Id });
        }

        //
        // GET: /RegAdmin/UndoCheckOut
        public ActionResult UndoCheckOut(int regId = 0, int partId = 0)
        {

            if (regId == 0)
            {
                return RedirectToAction("Index");
            }

            var registrationService = new RegistrationService();
            var regHistoryService = new RegHistoryService();
            var participantService = new ParticipantService();

            var foundRegistration = registrationService.GetByRegId(regId);

            if (foundRegistration == null)
            {
                return RedirectToAction("Index");
            }

            var participantList = participantService.GetParticipantList(regId);

            var isValid = false;

            if (partId != 0)
            {
                foreach (var participant in participantList)
                {
                    if (partId == participant.PartId && participant.StatusId != 4)
                    {
                        participant.StatusId = 2;
                        participantService.Update(participant);
                        regHistoryService.AddHistory(participant.RegId, "Participant Undo Check Out", participant.PartId.ToString(), 1);
                    }
                }
            }

            if (partId == 0)
            {
                foreach (var participant in participantList)
                {
                    if (participant.StatusId == 3)
                    {
                        participant.StatusId = 2;
                        participantService.Update(participant);
                        regHistoryService.AddHistory(participant.RegId, "Participant Undo Check Out", participant.PartId.ToString(), 1);
                    }
                }
            }

            return RedirectToAction("Registration", "Register", new { regObjectId = foundRegistration.Id });
        }

        //
        // GET: /RegAdmin/InitalizeRegFields
        public ActionResult InitalizeRegFields()
        {
            var regFieldService = new RegFieldService();
            var regValueService = new RegValueService();
            var translationService = new TranslationService();

            var enTransObjectId = translationService.GetTranslationObjectId("en");
            var zhTransObjectId = translationService.GetTranslationObjectId("zh");


            var newRegField = new RegField();
            var newRegValue = new RegValue();
            var newText = new RegText();
            
            //Gender
            if (regFieldService.IsExistKey("gender") == false)
            {
                newRegField = new RegField()
                    {
                        Key = "gender",
                        Name = "Gender"
                    };
                regFieldService.Update(newRegField);
                newRegField = regFieldService.GetById(regFieldService.FindRegFieldObjectId(newRegField.Key).ToString());

                newRegValue = new RegValue()
                    {
                        Value = 1,
                        RegFieldObjectId = newRegField.Id,
                        IsActive = true,
                        IsDeleted = false
                    };
                regValueService.Update(newRegValue);

                newText = new RegText() {TranslationId = enTransObjectId, Text = "Male"};
                regValueService.AddText(newRegField.Id,newRegValue.Value,newText);

                newText = new RegText() { TranslationId = zhTransObjectId, Text = "男" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                {
                    Value = 2,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Female" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "女" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
            }

            //AgeRange
            if (regFieldService.IsExistKey("agerange") == false)
            {
                newRegField = new RegField()
                {
                    Key = "agerange",
                    Name = "Age Range"
                };
                regFieldService.Update(newRegField);
                newRegField = regFieldService.GetById(regFieldService.FindRegFieldObjectId(newRegField.Key).ToString());

                newRegValue = new RegValue()
                {
                    Value = 1,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Infant (<1) no bed provided" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "婴儿(<1岁)(不提供婴儿床)" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                {
                    Value = 2,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Toddler (1-5)" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "儿童(1岁至5岁)" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                {
                    Value = 3,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Children (6-12)" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "儿童(6岁至12岁)" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                {
                    Value = 4,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Youth (13-17)" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "青年(13岁至17岁)" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                
                newRegValue = new RegValue()
                {
                    Value = 5,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Adult (18+)" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "成人(18岁或以上)" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
            }

            //Sessions
            if (regFieldService.IsExistKey("sessions") == false)
            {
                newRegField = new RegField()
                {
                    Key = "sessions",
                    Name = "Sessions"
                };
                regFieldService.Update(newRegField);
                newRegField = regFieldService.GetById(regFieldService.FindRegFieldObjectId(newRegField.Key).ToString());

                newRegValue = new RegValue()
                {
                    Value = 1,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Nursery" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "幼兒園" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                {
                    Value = 2,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Children's Program" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "儿童节目" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                {
                    Value = 3,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Youth Group" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "青年組" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                {
                    Value = 4,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Adult - Mandarin" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "成人 - 普通話" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                {
                    Value = 5,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Adult - Cantonese" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "成人 - 廣東話" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                {
                    Value = 6,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Adult - English" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "成人 - 英語" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
            }

            //RoomType
            if (regFieldService.IsExistKey("roomtype") == false)
            {
                newRegField = new RegField()
                    {
                        Key = "roomtype",
                        Name = "Room Types"
                    };
                regFieldService.Update(newRegField);
                newRegField = regFieldService.GetById(regFieldService.FindRegFieldObjectId(newRegField.Key).ToString());

                newRegValue = new RegValue()
                    {
                        Value = 1,
                        RegFieldObjectId = newRegField.Id,
                        IsActive = true,
                        IsDeleted = false
                    };
                regValueService.Update(newRegValue);
                newText = new RegText() {TranslationId = enTransObjectId, Text = "No Room - Part Time only"};
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() {TranslationId = zhTransObjectId, Text = "無房 - 部分時間"};
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                    {
                        Value = 2,
                        RegFieldObjectId = newRegField.Id,
                        IsActive = true,
                        IsDeleted = false
                    };
                regValueService.Update(newRegValue);
                newText = new RegText() {TranslationId = enTransObjectId, Text = "Singles Doritory"};
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() {TranslationId = zhTransObjectId, Text = "单身宿舍 - 全時間"};
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                    {
                        Value = 3,
                        RegFieldObjectId = newRegField.Id,
                        IsActive = true,
                        IsDeleted = false
                    };
                regValueService.Update(newRegValue);
                newText = new RegText() {TranslationId = enTransObjectId, Text = "Couple/Family - Full Time"};
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() {TranslationId = zhTransObjectId, Text = "夫妇/家庭 - 全時間"};
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                    {
                        Value = 4,
                        RegFieldObjectId = newRegField.Id,
                        IsActive = true,
                        IsDeleted = false
                    };
                regValueService.Update(newRegValue);
                newText = new RegText() {TranslationId = enTransObjectId, Text = "Elderly/Handicap - Full Time"};
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() {TranslationId = zhTransObjectId, Text = "長者/殘障室 - 全時間"};
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
            }

            //Status
            if (regFieldService.IsExistKey("status") == false)
            {
                newRegField = new RegField()
                {
                    Key = "status",
                    Name = "Reg Status"
                };
                regFieldService.Update(newRegField);
                newRegField = regFieldService.GetById(regFieldService.FindRegFieldObjectId(newRegField.Key).ToString());

                newRegValue = new RegValue()
                {
                    Value = 1,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Registered" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "注册" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                {
                    Value = 2,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Checked In" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "报到" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                {
                    Value = 3,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Checked Out" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "退房" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                {
                    Value = 4,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Removed" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "去掉" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
            }

            //Ledger
            if (regFieldService.IsExistKey("ledger") == false)
            {
                newRegField = new RegField()
                {
                    Key = "ledger",
                    Name = "Ledger Entry"
                };
                regFieldService.Update(newRegField);
                newRegField = regFieldService.GetById(regFieldService.FindRegFieldObjectId(newRegField.Key).ToString());

                newRegValue = new RegValue()
                {
                    Value = 1,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Registration Price" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "登记价格" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                {
                    Value = 2,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Scholarship Request" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "学术申请" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                {
                    Value = 3,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Cash Payment" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "现金支付" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                {
                    Value = 4,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Credit Card" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "信用卡支付" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                {
                    Value = 5,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Check Payment" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "支票支付" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                {
                    Value = 6,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Adjustment" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "调整量" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

                newRegValue = new RegValue()
                {
                    Value = 7,
                    RegFieldObjectId = newRegField.Id,
                    IsActive = true,
                    IsDeleted = false
                };
                regValueService.Update(newRegValue);
                newText = new RegText() { TranslationId = enTransObjectId, Text = "Refund" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);
                newText = new RegText() { TranslationId = zhTransObjectId, Text = "退款" };
                regValueService.AddText(newRegField.Id, newRegValue.Value, newText);

            }

            return RedirectToAction("Index");

        }

        //
        // GET: /RegAdmin/InitalizeRegPrice
        public ActionResult InitalizeRegPrice()
        {
            var regPriceService = new RegPriceService();
            var regValueService = new RegValueService();

            var translationService = new TranslationService();

            var TransObjectId = translationService.GetTranslationObjectId("en");

            var ageRangeList = regValueService.GetValueTextList("agerange", TransObjectId);
            var roomTypeList = regValueService.GetValueTextList("roomtype", TransObjectId);

            var newRegPrice = new RegPrice();

            foreach (var ageRange in ageRangeList)
            {
                if (ageRange.Value == 1)
                {
                    foreach (var roomType in roomTypeList)
                    {
                        if (roomType.Value == 1)
                        {
                            newRegPrice = new RegPrice()
                                {
                                    RoomTypeId = roomType.Value,
                                    AgeRangeId = ageRange.Value,
                                    Price = 5
                                };
                            regPriceService.Update(newRegPrice);
                        }
                        else
                        {
                            newRegPrice = new RegPrice()
                            {
                                RoomTypeId = roomType.Value,
                                AgeRangeId = ageRange.Value,
                                Price = 5
                            };
                            regPriceService.Update(newRegPrice);
                        }
                    }
                }

                if (ageRange.Value == 2)
                {
                    foreach (var roomType in roomTypeList)
                    {
                        if (roomType.Value == 1)
                        {
                            newRegPrice = new RegPrice()
                            {
                                RoomTypeId = roomType.Value,
                                AgeRangeId = ageRange.Value,
                                Price = 15
                            };
                            regPriceService.Update(newRegPrice);
                        }
                        else
                        {
                            newRegPrice = new RegPrice()
                            {
                                RoomTypeId = roomType.Value,
                                AgeRangeId = ageRange.Value,
                                Price = 15
                            };
                            regPriceService.Update(newRegPrice);
                        }
                    }
                }

                if (ageRange.Value == 3)
                {
                    foreach (var roomType in roomTypeList)
                    {
                        if (roomType.Value == 1)
                        {
                            newRegPrice = new RegPrice()
                            {
                                RoomTypeId = roomType.Value,
                                AgeRangeId = ageRange.Value,
                                Price = 40
                            };
                            regPriceService.Update(newRegPrice);
                        }
                        else
                        {
                            newRegPrice = new RegPrice()
                            {
                                RoomTypeId = roomType.Value,
                                AgeRangeId = ageRange.Value,
                                Price = 80
                            };
                            regPriceService.Update(newRegPrice);
                        }
                    }
                }

                if (ageRange.Value == 4)
                {
                    foreach (var roomType in roomTypeList)
                    {
                        if (roomType.Value == 1)
                        {
                            newRegPrice = new RegPrice()
                            {
                                RoomTypeId = roomType.Value,
                                AgeRangeId = ageRange.Value,
                                Price = 60
                            };
                            regPriceService.Update(newRegPrice);
                        }
                        else
                        {
                            newRegPrice = new RegPrice()
                            {
                                RoomTypeId = roomType.Value,
                                AgeRangeId = ageRange.Value,
                                Price = 100
                            };
                            regPriceService.Update(newRegPrice);
                        }
                    }
                }

                if (ageRange.Value == 5)
                {
                    foreach (var roomType in roomTypeList)
                    {
                        if (roomType.Value == 1)
                        {
                            newRegPrice = new RegPrice()
                            {
                                RoomTypeId = roomType.Value,
                                AgeRangeId = ageRange.Value,
                                Price = 60
                            };
                            regPriceService.Update(newRegPrice);
                        }
                        else
                        {
                            newRegPrice = new RegPrice()
                            {
                                RoomTypeId = roomType.Value,
                                AgeRangeId = ageRange.Value,
                                Price = 100
                            };
                            regPriceService.Update(newRegPrice);
                        }
                    }
                }
            }


            return RedirectToAction("Index");

        }            


    }
}
