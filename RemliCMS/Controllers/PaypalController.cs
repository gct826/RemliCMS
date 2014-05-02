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




    }
}
