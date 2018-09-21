using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using SchedulingBlocks.Models;

namespace SchedulingBlocks.ContactsApi
{
    public class ContactManager
    {
        public string ApiUrl
        {
            get { return "https://api.ontraport.com/cdata.php"; }
        }

        public string AppId
        {
            get { return ConfigurationManager.AppSettings["OntraportAppId"]; }
        }

        public string ApiKey
        {
            get { return ConfigurationManager.AppSettings["OntraportApiKey"]; }
        }

        public bool AddContact(Customer contact)
        {
            var postArgs = GetPostData(contact);
            return PostData(postArgs);
        }

        private string GetPostData(Customer contact)
        {
            var sb = new StringBuilder();
            sb.Append("appid=");
            sb.Append(AppId);
            sb.Append("&key=");
            sb.Append(ApiKey);
            sb.Append("&return_id=1&reqType=add&f_add=false&data=");
            sb.Append(GetXmlPostArgs(contact));
            return sb.ToString();

        }

        private string GetXmlPostArgs(Customer contact)
        {
            var sb = new StringBuilder();
            sb.Append("<contact>");
            sb.Append("<Group_Tag name=\"Contact Information\">");
            sb.Append("<field name=\"First Name\">");
            sb.Append(contact.FirstName);
            sb.Append("</field>");
            sb.Append("<field name=\"Last Name\">");
            sb.Append(contact.LastName);
            sb.Append("</field>");
            sb.Append("<field name=\"Email\">");
            sb.Append(contact.Email);
            sb.Append("</field>");
            sb.Append("<field name=\"Phone\">");
            sb.Append(contact.Phone);
            sb.Append("</field>");
            sb.Append("</Group_Tag>");
            sb.Append("<Group_Tag name=\"Sequences and Tags\">");
            sb.Append("<field name=\"Contact Tags\">");
            sb.Append(contact.Sport);
            sb.Append("</field>");
            //sb.Append("<field name=\"Sequences\">*/*3*/*8*/*</field>");
            sb.Append("</Group_Tag>");
            sb.Append("</contact>");

            return HttpUtility.UrlEncode(sb.ToString());
        }



        private bool PostData(string postData)
        {
            var request = WebRequest.Create(ApiUrl);
            request.Method = "POST";
            var byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            var dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            // Get the response.
            // Should contain one of the following 
            // <result><status>Success</status></result>
            // <result><status>Failed</status></result>
            var response = request.GetResponse();
            // Display the status.
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            dataStream = response.GetResponseStream();
            var reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            //Console.WriteLine(responseFromServer);

            reader.Close();
            dataStream.Close();
            response.Close();

            if (responseFromServer.Contains("Success"))
            {
                return true;
            }

            return false;
        }
    }
}