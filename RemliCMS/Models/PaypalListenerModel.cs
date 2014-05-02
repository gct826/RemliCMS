using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using RemliCMS.RegSystem.Services;

namespace RemliCMS.Models
{
    public class PaypalListenerModel
    {
        public PayPalModel _PaypalModel { get; set; }
          
          public void GetStatus(byte[] parameters)         
          {
          
            //verify the transaction             
            string status = Verify(true, parameters);

            var regHistoryService = new RegHistoryService();

            if (status == "VERIFIED")            
            {

            regHistoryService.AddHistory(1, "Paypal Notification", "VERIFIED", 1);

              //check that the payment_status is Completed                 
              if (_PaypalModel.payment_status.ToLower() == "completed")                 
              { 
              

                //check that txn_id has not been previously processed to prevent duplicates                      
         
                //check that receiver_email is your Primary PayPal email                                          
         
                //check that payment_amount/payment_currency are correct                       
         
                 //process payment/refund/etc                     
         
              }                 
              else if (status == "INVALID")             
              {
                  regHistoryService.AddHistory(1, "Paypal Notification", "INVALID", 1);
                //log for manual investigation             
              }             
              else             
              {                 
                //log response/ipn data for manual investigation             
              }

            }
            
          }           
          
          private string Verify(bool isSandbox, byte[] parameters)         
          {               
          
            string response = "";               
            try
            {

                string url = "https://www.sandbox.paypal.com/cgi-bin/webscr";
                           
              HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);                 
              webRequest.Method = "POST";                 
              webRequest.ContentType = "application/x-www-form-urlencoded";
                             
              //must keep the original intact and pass back to PayPal with a _notify-validate command
              string data = Encoding.ASCII.GetString(parameters);                 
              data += "&cmd=_notify-validate";   
                                  
              webRequest.ContentLength = data.Length;                   
          
              //Send the request to PayPal and get the response                 
              using (StreamWriter streamOut = new StreamWriter(webRequest.GetRequestStream(),System.Text.Encoding.ASCII))                 
              {                     
                streamOut.Write(data);                     
                streamOut.Close();                 
              }                   
          
              using (StreamReader streamIn = new StreamReader(webRequest.GetResponse().GetResponseStream()))                 
              {                     
                response = streamIn.ReadToEnd();                     
                streamIn.Close();                 
              }             
          
            }             
            catch {}               
          
            return response;    
                 
          }       
    }
}