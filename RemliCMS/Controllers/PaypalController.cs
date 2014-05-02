using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RemliCMS.Models;
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
        public ActionResult ValidateCommand(string itemName, decimal amount, string returnUrl, string cancelUrl)
        {
            bool useSandbox = Convert.ToBoolean(ConfigurationManager.AppSettings["PayPalSandbox"]);
            var paypal = new PayPalModel(useSandbox);
            paypal.item_name = "2014 Summer Conference";

            paypal.item_name = itemName;
            paypal.amount = amount;
            paypal.@return = returnUrl;
            paypal.cancel_return = cancelUrl;


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
        public ActionResult Notify()
        {
            return View();
        }



    }
}
