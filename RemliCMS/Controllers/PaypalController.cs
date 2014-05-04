using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using RemliCMS.Models;
using RemliCMS.RegSystem.Services;
using RemliCMS.Routes;

namespace RemliCMS.Controllers
{
    public class PaypalController : BaseController
    {
        public PaypalController(IRouteService routeService) : base(routeService)
        {
        }

        //
        // GET: /Paypal/ValidateCommand
        public ActionResult ValidateCommand(string itemName, decimal amount, string regObjectId)
        {
            bool useSandbox = Convert.ToBoolean(ConfigurationManager.AppSettings["PayPalSandbox"]);
            var paypal = new PayPalModel(useSandbox);
            paypal.item_name = "2014 Summer Conference";

            string returnUrl = ConfigurationManager.AppSettings["PayPalReturnUrl"];
            string cancelUrl = ConfigurationManager.AppSettings["PayPalCancelUrl"];
            string notifyUrl = ConfigurationManager.AppSettings["PayPalNotifyUrl"] + "?regObjectId=" + regObjectId;

            paypal.item_name = itemName;
            paypal.amount = amount;
            paypal.@return = returnUrl;
            paypal.cancel_return = cancelUrl;
            paypal.notify_url = notifyUrl;

            ViewBag.RegObjectId = regObjectId;
            return View(paypal);
        }


        //
        // POST:/Paypal/PaymentNotification
        public EmptyResult PaymentNotification(string regObjectId)
        {
            string strLog = "";
            string currentTime = "";

            var regHistoryService = new RegHistoryService();

            currentTime = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year + "|" + DateTime.Now.TimeOfDay.Hours.ToString() + ":" + DateTime.Now.TimeOfDay.Minutes.ToString() + ":" + DateTime.Now.TimeOfDay.Seconds.ToString();

            strLog = "Start IPN request " + currentTime;

            regHistoryService.AddHistory(0, "Paypal Notification", strLog, 1);

            string strLive = "https://www.sandbox.paypal.com/cgi-bin/webscr";

            bool useSandbox = Convert.ToBoolean(ConfigurationManager.AppSettings["PayPalSandbox"]);
            
            if (useSandbox)
            {
                strLive = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            }
            else
            {
                strLive  = ConfigurationManager.AppSettings["PayPalActionUrl"];
            }

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strLive);

            //Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] Param = Request.BinaryRead(HttpContext.Request.ContentLength);
            string strRequest = Encoding.ASCII.GetString(Param);
            strRequest = strRequest + "&cmd=_notify-validate";
            req.ContentLength = strRequest.Length;

            //for proxy
            //Dim proxy As New WebProxy(New System.Uri("http://url:port#"))
            //req.Proxy = proxy

            //Send the request to PayPal and get the response
            StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), Encoding.ASCII);
            streamOut.Write(strRequest);
            streamOut.Close();
            StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
            string strResponse = streamIn.ReadToEnd();
            streamIn.Close();

            if (strResponse == "VERIFIED")
            {
                //check the payment_status is Completed
                //check that txn_id has not been previously processed
                //check that receiver_email is your Primary PayPal email
                //check that payment_amount/payment_currency are correct
                //process payment

                var registrationService = new RegistrationService();
                var foundRegEntry = registrationService.GetById(regObjectId);
                var success = new bool();
                if (foundRegEntry == null)
                {
                    success = false;
                }
                else
                {
                    var ledgerService = new LedgerService();
                    success = ledgerService.ConfirmPayPal(foundRegEntry.RegId);
                    regHistoryService.AddHistory(foundRegEntry.RegId,"Payment Verified","",1);
                }
                
                currentTime = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year + "|" + DateTime.Now.TimeOfDay.Hours.ToString() + ":" + DateTime.Now.TimeOfDay.Minutes.ToString() + ":" + DateTime.Now.TimeOfDay.Seconds.ToString();

                if (success)
                {
                    strLog = "IPN Request VERIFIED - Reg " + foundRegEntry.RegId +" "+ currentTime;                    
                }
                else
                {
                    strLog = "IPN Request VERIFIED - Reg Not Found " + currentTime;
                }
                
                regHistoryService.AddHistory(0, "Paypal Notification", strLog, 1);

            }
            else if (strResponse == "INVALID")
            {
                //log for manual investigation
                currentTime = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year + "|" + DateTime.Now.TimeOfDay.Hours.ToString() + ":" + DateTime.Now.TimeOfDay.Minutes.ToString() + ":" + DateTime.Now.TimeOfDay.Seconds.ToString();

                strLog = "IPN Request INVALID " + currentTime;
                
                regHistoryService.AddHistory(0, "Paypal Notification", strLog, 1);

            }
            else
            {
                //Response wasn't VERIFIED or INVALID, log for manual investigation
            }
            currentTime = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year + "|" + DateTime.Now.TimeOfDay.Hours.ToString() + ":" + DateTime.Now.TimeOfDay.Minutes.ToString() + ":" + DateTime.Now.TimeOfDay.Seconds.ToString();

            strLog = "Finish IPN Requets " + currentTime;

            regHistoryService.AddHistory(0, "Paypal Notification", strLog, 1);

            return new EmptyResult();
        }

        //
        //GET:/Paypal/ButtonTest
        public ActionResult ButtonTest()
        {
            return View();
        }
    }
}
