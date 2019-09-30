using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Frayte.Api.Models;
using Frayte.Services.Models.ApiModal;
using Frayte.Services.DataAccess;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Frayte.Api
{
    public class APIKeyMessageHandler : DelegatingHandler
    {
        public const string PrivateKey = "IRAS-Soft-Techno";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            Security frayteSecurity = new Security();
            string accountNumber = string.Empty;
            var content = await httpRequestMessage.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(content))
            {
                try
                {
                    if (content.Contains("xml"))
                    {
                        XmlSerializer x = new XmlSerializer(typeof(Security));
                        frayteSecurity = (Security)x.Deserialize(new StringReader(content));
                        accountNumber = frayteSecurity.AccountNumber.Contains("-") ? (frayteSecurity.AccountNumber).Replace("-", "") : frayteSecurity.AccountNumber;
                    }
                    else
                    {
                        dynamic Responseresult = JsonConvert.DeserializeObject<object>(content);
                        frayteSecurity = MapSecurity(JsonConvert.SerializeObject(Responseresult.Security));
                        accountNumber = frayteSecurity.AccountNumber.Contains("-") ? (frayteSecurity.AccountNumber).Replace("-", "") : frayteSecurity.AccountNumber;
                    }

                    using (var dbContext = new FrayteEntities())
                    {
                        var decriptKey = CryptoEngine.Decrypt(frayteSecurity.APIKey, PrivateKey);
                        IList<string> key = decriptKey.Split(',');
                        var Account = key[0].ToString();
                        var accountEmail = key[1].ToString();
                        if (Account == accountNumber)
                        {
                            var user = (from U in dbContext.Users
                                        join UA in dbContext.UserAdditionals on U.UserId equals UA.UserId
                                        where
                                            UA.AccountNo == accountNumber &&
                                            UA.APIKey == frayteSecurity.APIKey
                                        select new
                                        {
                                            CompanyName = U.CompanyName,
                                            CustomerId = U.UserId,
                                            FirstName = U.ContactName,
                                            Phone = U.TelephoneNo,
                                            CurrencyCode = UA.CreditLimitCurrencyCode,
                                            OperationZoneId = U.OperationZoneId,
                                            IsShipperTaxAndDuty = UA.IsShipperTaxAndDuty.HasValue == true ? true : false,
                                            IsRateShow = UA.IsAllowRate.Value == true ? true : false,
                                            IsApiAllow = UA.IsApiAllow.HasValue ? UA.IsApiAllow.Value : false
                                        }).FirstOrDefault();

                            if (user != null)
                            {
                                if (user.IsApiAllow)
                                {
                                    return await base.SendAsync(httpRequestMessage, cancellationToken);
                                }
                                else
                                {
                                    return httpRequestMessage.CreateResponse(HttpStatusCode.Forbidden, "Invalid API Access");
                                }
                            }
                            else
                            {
                                return httpRequestMessage.CreateResponse(HttpStatusCode.Forbidden, "Invalid API Key");
                            }
                        }
                        else
                        {
                            return httpRequestMessage.CreateResponse(HttpStatusCode.Forbidden, "Json is not valid");
                        }
                    }

                }
                catch (Exception ex)
                {
                    return httpRequestMessage.CreateResponse(HttpStatusCode.Forbidden, "Json is not valid");
                }
            }
            else
            {
                return null;
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