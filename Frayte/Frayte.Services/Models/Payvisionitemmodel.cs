using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
namespace Frayte.Services.Models
{
    public class Payvisionitemmodel
    {

        public string CreateTokenForPayment(string url, string postdata)
        {
            string result = string.Empty;
            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] postBytes = ascii.GetBytes(postdata);

            // set up request object
            HttpWebRequest request;
            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(url);
            }
            catch (UriFormatException)
            {
                request = null;
            }
            if (request == null)
                throw new ApplicationException("Invalid URL: " + url);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postBytes.Length;
            Stream postStream = request.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            long length = 0;

            try
            {
                using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    length = response.ContentLength;

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            catch 
            {                
            }

            return result;
        }
    }
}