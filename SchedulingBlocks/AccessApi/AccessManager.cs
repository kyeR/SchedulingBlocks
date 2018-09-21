using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace SchedulingBlocks.AccessApi
{
    public class AccessManager
    {
        public void Test()
        {
            byte[] requestData = Encoding.ASCII.GetBytes("<?xml version=\"1.0\"?><methodCall><methodName>Bugzilla.version</methodName></methodCall>");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://bugzilla.mozilla.org/xmlrpc.cgi");
            request.Method = "POST";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729;)";
            request.ContentType = "text/xml";
            request.ContentLength = requestData.Length;

            using (Stream requestStream = request.GetRequestStream())
                requestStream.Write(requestData, 0, requestData.Length);

            string result = null;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.ASCII))
                        result = reader.ReadToEnd();
                }
            }
        }
    }
}