using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RemliCMS.Models
{
    public class PayPalModel
    {
        public string cmd { get; set; }
        public string business { get; set; }
        public string no_shipping { get; set; }
        public string @return { get; set; }
        public string cancel_return { get; set; }
        public string notify_url { get; set; }
        public string currency_code { get; set; }
        public string item_name { get; set; }
        public string payment_status { get; set; }

        [Required(ErrorMessage = "Required")]
        [DisplayName("Amount")]
        [DataType(DataType.Currency)]
        public decimal amount { get; set; }

        public string action_url { get; set; }

        public PayPalModel(bool useSandbox)
        {
            if (useSandbox)
            {
                action_url = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            }
            else
            {
                action_url = ConfigurationManager.AppSettings["PayPalActionUrl"];
            }

            cmd = "_xclick";
            business = ConfigurationManager.AppSettings["PayPalBusiness"];
            cancel_return = ConfigurationManager.AppSettings["PayPalCancelUrl"];
            @return = ConfigurationManager.AppSettings["PayPalReturnUrl"];
            notify_url = ConfigurationManager.AppSettings["PayPalNotifyUrl"];
            // We can add parameters here, for example OrderId, CustomerId, etc….
            currency_code = "USD";
        }
    }
}