using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace frayteWinApp.Utility
{
    public static class ProcessWebApi
    {
        public static string ExecuteGetRequest(string actionName, Object jsonObj)
        {
            string url = AppSettings.WebApiURL + actionName;
            string jsonresult = string.Empty;

            try
            {
                var request = WebRequest.Create(url);

                if (request != null)
                {
                    using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
                    {

                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            jsonresult = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                jsonresult = "An error occurred while executing web request:"  + e.Message;
            }
            return jsonresult;
        }

        public static string ExecuteRateCardRequest(string actionName, Object jsonObj)
        {
            string url = AppSettings.WebApiRateCardURL + actionName;
            string jsonresult = string.Empty;

            try
            {
                var request = WebRequest.Create(url);

                if (request != null)
                {
                    using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
                    {

                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            jsonresult = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                jsonresult = "An error occurred while executing web request:" + e.Message;
            }
            return jsonresult;
        }        
    }
}
