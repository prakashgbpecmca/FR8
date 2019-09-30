using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Frayte.Services.Models;
using System.Web;
using System.Xml.Serialization;
using Frayte.Services.Models.Aftership;
using Frayte.Services.Business;
using Frayte.Services.Utility;
using RazorEngine.Templating;

namespace Frayte.WebApi.MessageHandler
{
    public class APIKeyMessageHandler : DelegatingHandler
    {
        public const string PrivateKey = "IRAS-Soft-Techno";
        public class Security
        {
            public string APIKey { get; set; }
            public string AccountNumber { get; set; }
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            Security frayteSecurity = new Security();
            string accountNumber = string.Empty;
            var content = await httpRequestMessage.Content.ReadAsStringAsync();
            try
            {
                if (httpRequestMessage.RequestUri.ToString().Contains("AftershipTracking/ProcessWebHook"))
                {
                    if (!string.IsNullOrEmpty(content))
                    {
                        // send mail to developer 
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("AftershipTracking's ProcessWebHook hit."));
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(content));
                        try
                        {
                            AftershipWebhookObject webHookObj = JsonConvert.DeserializeObject<AftershipWebhookObject>(content);
                            if (webHookObj == null)
                            {
                                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Aftership's obj  not parsed."));

                                DynamicViewBag viewBag = new DynamicViewBag();
                                viewBag.AddValue("Value", content);
                                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/" + "aftershipError.cshtml");
                                var templateService = new TemplateService();
                                var EmailBody = templateService.Parse(template, "", viewBag, null);
                                FrayteEmail.SendMail("prakash.pant@irasys.biz", "", "Aftership's obj  not parsed.", "", "", "");

                            }
                        }
                        catch (Exception ex)
                        {
                            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Aftership's obj  not parsed."));
                        }
                    }
                    else
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Aftership's obj  is empty."));
                    } 
                } 
                return await base.SendAsync(httpRequestMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                return httpRequestMessage.CreateResponse(HttpStatusCode.Forbidden, "Json is not valid");
            }
        }

        private Security MapSecurity(string packagejson)
        {
            var security = new Security();
            security = JsonConvert.DeserializeObject<Security>(packagejson);
            return security;
        }
    }
}