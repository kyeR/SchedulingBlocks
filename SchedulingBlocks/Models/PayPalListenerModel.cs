using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace SchedulingBlocks.Models
{
    public class PayPalListenerModel
    {
        public PayPalCheckoutInfo PayPalCheckoutInfo { get; set; }
        public bool IsVerified { get; set; }
        public bool IsPaymentCompleted { get; set; }

        public void ProcessParameters(byte[] parameters)
        {
            //verify the transaction             
            var status = Verify(false, parameters);

            if (status == "VERIFIED")
            {
                IsVerified = true;

                //check that the payment_status is Completed                 
                if (PayPalCheckoutInfo.payment_status.ToLower() == "completed")
                {
                    IsPaymentCompleted = true;

                }
                else
                {
                    IsPaymentCompleted = false;
                }

            }
            else
            {
                IsVerified = false;
                IsPaymentCompleted = false;
            }

        }

        private string Verify(bool isSandbox, byte[] parameters)
        {

            string response = "";
            try
            {

                string url = isSandbox ?
                  "https://www.sandbox.paypal.com/cgi-bin/webscr" : "https://www.paypal.com/cgi-bin/webscr";

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";

                //must keep the original intact and pass back to PayPal with a _notify-validate command
                string data = Encoding.ASCII.GetString(parameters);
                data += "&cmd=_notify-validate";

                webRequest.ContentLength = data.Length;

                //Send the request to PayPal and get the response                 
                using (StreamWriter streamOut = new StreamWriter(webRequest.GetRequestStream(), System.Text.Encoding.ASCII))
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
            catch { }

            return response;

        }
    }
}