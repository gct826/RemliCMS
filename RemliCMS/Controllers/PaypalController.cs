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
        public ActionResult ValidateCommand(string itemName, decimal amount, string returnUrl, string cancelUrl, string notifyUrl)
        {
            bool useSandbox = Convert.ToBoolean(ConfigurationManager.AppSettings["PayPalSandbox"]);
            var paypal = new PayPalModel(useSandbox);
            paypal.item_name = "2014 Summer Conference";

            paypal.item_name = itemName;
            paypal.amount = amount;
            paypal.@return = returnUrl;
            paypal.cancel_return = cancelUrl;
            paypal.notify_url = notifyUrl;

            return View(paypal);
        }

        //
        // GET: /Paypal/Redirect
        public ActionResult Redirect()
        {
            return View();
        }

        //
        // GET: /Paypal/Cancel
        public ActionResult Cancel()
        {
            return View();
        }

        //
        // GET: /Paypal/Notify
        public EmptyResult Notify(PayPalModel payPalModel)
        {
            PaypalListenerModel model = new PaypalListenerModel();
            model._PaypalModel = payPalModel;
            byte[] parameters = Request.BinaryRead(Request.ContentLength);

            if (parameters != null)
            {
                model.GetStatus(parameters);
            }

            return new EmptyResult();
        }

        public EmptyResult PaymentNotification()
        {
            string strLog = "";
            string currentTime = "";

            var regHistoryService = new RegHistoryService();

            currentTime = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year + "|" + DateTime.Now.TimeOfDay.Hours.ToString() + ":" + DateTime.Now.TimeOfDay.Minutes.ToString() + ":" + DateTime.Now.TimeOfDay.Seconds.ToString();

            strLog = "Insert into CPLog(Log,LogTime) values('Start IPN request','" + currentTime + "')";

            regHistoryService.AddHistory(1, "Paypal Notification", strLog, 1);

            string strLive = "https://www.sandbox.paypal.com/cgi-bin/webscr";
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
                currentTime = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year + "|" + DateTime.Now.TimeOfDay.Hours.ToString() + ":" + DateTime.Now.TimeOfDay.Minutes.ToString() + ":" + DateTime.Now.TimeOfDay.Seconds.ToString();

                strLog = "Insert into CPLog(Log,LogTime) values('Status - Verified','" + currentTime + "')";
                
                regHistoryService.AddHistory(1, "Paypal Notification", strLog, 1);

            }
            else if (strResponse == "INVALID")
            {
                //log for manual investigation
                currentTime = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year + "|" + DateTime.Now.TimeOfDay.Hours.ToString() + ":" + DateTime.Now.TimeOfDay.Minutes.ToString() + ":" + DateTime.Now.TimeOfDay.Seconds.ToString();

                strLog = "Insert into CPLog(Log,LogTime) values('Status - Invalid','" + currentTime + "')";
                
                regHistoryService.AddHistory(1, "Paypal Notification", strLog, 1);

            }
            else
            {
                //Response wasn't VERIFIED or INVALID, log for manual investigation
            }
            currentTime = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year + "|" + DateTime.Now.TimeOfDay.Hours.ToString() + ":" + DateTime.Now.TimeOfDay.Minutes.ToString() + ":" + DateTime.Now.TimeOfDay.Seconds.ToString();

            strLog = "Insert into CPLog(Log,LogTime) values('Finish IPN Request','" + currentTime + "')";

            regHistoryService.AddHistory(1, "Paypal Notification", strLog, 1);

            return new EmptyResult();
        }


    }
}
