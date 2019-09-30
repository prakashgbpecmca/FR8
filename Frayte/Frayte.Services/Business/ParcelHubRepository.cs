using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Models.ApiModal;
using Frayte.Services.Models.ParcelHub;
using Frayte.Services.DataAccess;
using Parcelhub.API.InterfaceNet2;
using Parcelhub.API.InterfaceNet2.Client;
using Parcelhub.API.InterfaceNet2.ResponseObject;
using Parcelhub.API.InterfaceNet2.RequestObject;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Frayte.Services.Utility;
using System.Text.RegularExpressions;
using Frayte.Services;
using System.Text;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Net.Http.Headers;
using System.Web.Hosting;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using Newtonsoft.Json.Linq;
using Frayte.Services.Models.Express;

namespace Frayte.Services.Business
{
    public class ParcelHubRepository
    {
        #region MappingIntoParcelHubRequestObject

        public ParcelHubShipmentRequest MapDirectBookingDetailToShipmentRequest(DirectBookingShipmentDraftDetail directBookingDetail)
        {
            try
            {
                //Step 1: Map ShipFrom Address
                ParcelHubShippingAddress FromAddress = new ParcelHubShippingAddress();
                MapParcelHubShipFrom(directBookingDetail, FromAddress);

                //Step 2: Map ShipTo Address
                ParcelHubShippingAddress ToAddress = new ParcelHubShippingAddress();
                MapParcelHubShipTo(directBookingDetail, ToAddress);

                //Step 3: Map PackageDetail
                List<ParcelHubPackageDetail> _package = new List<ParcelHubPackageDetail>();
                MapParcelHubPackageDetail(directBookingDetail, _package);

                //Step 4: Map CustomInfo
                ParcelHubShipmentCustomeInfo CustomInfo = new ParcelHubShipmentCustomeInfo();
                MapParcelHubCustomInfo(directBookingDetail, CustomInfo);

                //Step 5: Get ParcelHub UserName and Password From DB
                var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.ParcelHub);

                ParcelHubShipmentRequest parcelhubShipmentRequest = new ParcelHubShipmentRequest();
                if (logisticIntegration != null)
                {
                    parcelhubShipmentRequest.Security = new ParcelHubSecurity();
                    parcelhubShipmentRequest.Security.UserName = logisticIntegration.UserName;
                    parcelhubShipmentRequest.Security.Password = logisticIntegration.Password;
                    parcelhubShipmentRequest.Security.UserAgent = logisticIntegration.UserAgent;
                    parcelhubShipmentRequest.Security.ServiceUrl = logisticIntegration.ServiceUrl;
                    parcelhubShipmentRequest.DraftShipmentId = directBookingDetail.DirectShipmentDraftId;
                    parcelhubShipmentRequest.ContentsDescription = directBookingDetail.ReferenceDetail.ContentDescription;
                    parcelhubShipmentRequest.Reference1 = directBookingDetail.FrayteNumber + "-" + directBookingDetail.ReferenceDetail.Reference1;
                    parcelhubShipmentRequest.Reference2 = directBookingDetail.ReferenceDetail.Reference2;
                    parcelhubShipmentRequest.PackageCalculationType = directBookingDetail.PakageCalculatonType;
                    parcelhubShipmentRequest.CollectionAddress = new ParcelHubShippingAddress();
                    parcelhubShipmentRequest.CollectionAddress = FromAddress;
                    parcelhubShipmentRequest.DeliveryAddress = new ParcelHubShippingAddress();
                    parcelhubShipmentRequest.DeliveryAddress = ToAddress;
                    parcelhubShipmentRequest.Package = new List<ParcelHubPackageDetail>();
                    parcelhubShipmentRequest.Package = _package;
                    parcelhubShipmentRequest.CustomInfo = new ParcelHubShipmentCustomeInfo();
                    parcelhubShipmentRequest.CustomInfo = CustomInfo;
                    parcelhubShipmentRequest.CourierAccountCountryCode = directBookingDetail.CustomerRateCard.CourierAccountCountryCode;
                    parcelhubShipmentRequest.CourierAccountNo = directBookingDetail.CustomerRateCard.CourierAccountNo;
                    parcelhubShipmentRequest.CourierName = directBookingDetail.CustomerRateCard.CourierName;
                    parcelhubShipmentRequest.CourierDescription = directBookingDetail.CustomerRateCard.CourierDescription;
                    parcelhubShipmentRequest.CustomerId = directBookingDetail.CustomerId;
                }

                return parcelhubShipmentRequest;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        #endregion

        #region Integration

        public ParcelHubResponse CreateShipment(ParcelHubShipmentRequest request)
        {
            ParcelHubResponse response = new ParcelHubResponse();
            response.Error = new FratyteError();
            response.Error.Custom = new List<string>();
            response.Error.Package = new List<string>();
            response.Error.Address = new List<string>();
            response.Error.Service = new List<string>();
            response.Error.ServiceError = new List<string>();
            response.Error.Miscellaneous = new List<string>();
            response.Error.MiscErrors = new List<FrayteKeyValue>();

            GetXMLFromRequestObject(request);

            try
            {
                string folderpath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");

                //Step1: Create Initial Nodes
                string shipmentXML = CreateShipmentRequestNodes(request, folderpath);

                //Step2: Read all data from XML file
                string xml_in = File.ReadAllText(@shipmentXML);

                //Step3: Authenticate to user
                string authenticate = Authentication(request.Security.UserName, request.Security.Password);

                //Step4: Get services for initgial xml file
                response.Request = xml_in;
                var services = GetServices(request.Security.UserName, authenticate, xml_in);

                //Step5: Deseriliaze get services object
                XDocument xml = XDocument.Parse(services);
                string json = JsonConvert.SerializeXNode(xml);
                GetServices serviceresponse = JsonConvert.DeserializeObject<GetServices>(json);

                try
                {
                    if (serviceresponse != null)
                    {
                        //Step6: Create final nodes with apply service
                        string FinalshipmentXML = CreateFinalShipmentRequestNodes(request, serviceresponse, folderpath, ref response);
                        if (response.Error.Miscellaneous.Count > 0)
                        {
                            response.Error.IsMailSend = true;
                            response.Error.Status = false;
                            return response;
                        }
                        else
                        {
                            //Ste7: Read All data from final XMl file
                            string Final_xml_in = File.ReadAllText(@FinalshipmentXML);

                            //Step8: Create finally shipment in parcel hub
                            var shipmentResult = CreateShipmentInParcelHub(Final_xml_in, authenticate, request.Security.UserName);
                            response.Response = shipmentResult;
                            //Step9: Save Parcel Hub Response                        
                            if (System.IO.Directory.Exists(folderpath))
                            {
                                if (File.Exists(folderpath + "/ParcelHubResponse.xml"))
                                {
                                    File.Delete(folderpath + "/ParcelHubResponse.xml");
                                    File.Create(folderpath + "/ParcelHubResponse.xml").Close();
                                    File.WriteAllText(folderpath + "/ParcelHubResponse.xml", shipmentResult);
                                }
                                else
                                {
                                    File.Create(folderpath + "/ParcelHubResponse.xml").Close();
                                    File.WriteAllText(folderpath + "/ParcelHubResponse.xml", shipmentResult);
                                }
                            }
                            else
                            {
                                System.IO.Directory.CreateDirectory(folderpath);
                                File.Create(folderpath + "/ParcelHubResponse.xml").Close();
                                File.WriteAllText(folderpath + "/ParcelHubResponse.xml", shipmentResult);
                            }

                            XmlDataDocument data = new XmlDataDocument();
                            FileStream fs = new FileStream(folderpath + "/ParcelHubResponse.xml", FileMode.Open, FileAccess.Read);
                            data.Load(fs);

                            List<ParcelResponse> _res = new List<ParcelResponse>();
                            ParcelResponse res = new ParcelResponse();

                            List<ImageData> _da = new List<ImageData>();
                            ImageData da;
                            XmlNodeList xmlNodeList = data.GetElementsByTagName("LabelData");

                            if (xmlNodeList.Count > 0)
                            {
                                for (int i = 0; i < xmlNodeList.Count; i++)
                                {
                                    foreach (XmlNode node in xmlNodeList[i].ChildNodes)
                                    {
                                        da = new ImageData();
                                        da.ImageBytes = node.InnerText;
                                        _da.Add(da);
                                    }
                                }

                                res.Bytes = _da;

                                CourierTrackingNumber cour;
                                XmlNodeList xmlNodeList1 = data.GetElementsByTagName("ShippingInfo");
                                for (int i = 0; i < xmlNodeList1.Count; i++)
                                {
                                    foreach (XmlNode node in xmlNodeList1[i].ChildNodes)
                                    {
                                        if (node.Name == "CourierTrackingNumber")
                                        {
                                            cour = new CourierTrackingNumber();
                                            cour.TrackingNumber = node.InnerText;
                                            res.CourierNumber = cour;

                                            //Main Tracking No
                                            response.ShippingInfo = new Models.ParcelHub.ShippingInfo();
                                            response.ShippingInfo.CourierTrackingNumber = cour.TrackingNumber;
                                        }
                                    }
                                }

                                List<PackageTrackingNumber> _pack = new List<PackageTrackingNumber>();
                                PackageTrackingNumber pack;
                                XmlNodeList xmlNodeList2 = data.GetElementsByTagName("CourierTrackingNumber");
                                for (int i = 0; i < xmlNodeList2.Count; i++)
                                {
                                    foreach (XmlNode node in xmlNodeList2[i].ChildNodes)
                                    {
                                        pack = new PackageTrackingNumber();
                                        pack.CourierTrackingNumber = node.InnerText;
                                        _pack.Add(pack);
                                    }
                                }

                                _pack.RemoveAt(0);
                                res.PackageNumber = _pack;
                                _res.Add(res);

                                response.Parcel = new List<ParcelResponse>();
                                response.Parcel = _res;
                                response.Error.IsMailSend = false;
                                response.Error.Status = true;
                            }
                            else
                            {
                                XmlNodeList errorNodeList = data.GetElementsByTagName("Message");
                                if (errorNodeList.Count > 0)
                                {
                                    for (int j = 0; j < errorNodeList.Count; j++)
                                    {
                                        foreach (XmlNode node in errorNodeList[j].ChildNodes)
                                        {
                                            response.Error.Miscellaneous.Add(node.InnerText);
                                            response.Error.IsMailSend = true;
                                            response.Error.Status = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        response.Error.Miscellaneous.Add("No services found from parcel hub");
                        response.Error.IsMailSend = true;
                        response.Error.Status = false;
                    }
                }
                catch (Exception ex)
                {
                    if (serviceresponse.GetServicesResponse.UnsuitableServiceProviders.UnsuitableServiceProvider.Count > 0)
                    {
                        for (int i = 0; i < serviceresponse.GetServicesResponse.UnsuitableServiceProviders.UnsuitableServiceProvider.Count; i++)
                        {
                            response.Error.Miscellaneous.Add(serviceresponse.GetServicesResponse.UnsuitableServiceProviders.UnsuitableServiceProvider[i].UnsuitableServiceProviderMessage);
                        }
                    }
                    if (serviceresponse.GetServicesResponse.UnsuitableServiceProviders.UnsuitableServiceProvider.Count == 0)
                    {
                        response.Error.Miscellaneous.Add("No service found and due to that could not create shipment");
                    }
                    response.Error.IsMailSend = true;
                    response.Error.Status = false;
                }
            }
            catch (Exception ex)
            {
                response.Error.Miscellaneous.Add(ex.Message);
            }

            return response;
        }

        internal string CreateShipmentRequestNodes(ParcelHubShipmentRequest request, string folderpath)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Encoding = new UTF8Encoding(true);
            xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;

            xmlWriterSettings.Indent = true;

            if (System.IO.Directory.Exists(folderpath))
            {
                if (File.Exists(folderpath + "/ParcelHub.xml"))
                {
                    File.Delete(folderpath + "/ParcelHub.xml");
                }
            }
            else
            {
                System.IO.Directory.CreateDirectory(folderpath);
            }

            using (var xmlWriter = XmlWriter.Create(folderpath + "/ParcelHub.xml"))
            {
                if (Frayte.Services.Utility.AppSettings.ApplicationMode == FrayteApplicationMode.Test)
                {
                    xmlWriter.WriteStartElement("Shipment", ParcelHubTestRequestUrl.TestStartXmlTag);
                }
                else
                {
                    xmlWriter.WriteStartElement("Shipment", ParcelHubLiveRequestUrl.LiveStartXmlTag);
                }

                CreateRequestNode(xmlWriter, request);

                xmlWriter.WriteEndElement();
            }

            return folderpath + "/ParcelHub.xml";
        }

        internal void CreateRequestNode(XmlWriter xmlWriter, ParcelHubShipmentRequest request)
        {
            xmlWriter.WriteStartElement("Account");
            xmlWriter.WriteString(request.Security.UserName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CollectionDetails");

            TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            DateTimeOffset localServerTime = DateTimeOffset.Now;
            DateTimeOffset localTime = TimeZoneInfo.ConvertTime(localServerTime, info);
            string date = localTime.Date.ToString("yyyy-MM-dd");
            string time = localTime.TimeOfDay.Hours.ToString() + ":" + (localTime.TimeOfDay.Minutes.ToString().Length == 1 ? "0" + localTime.TimeOfDay.Minutes.ToString() : localTime.TimeOfDay.Minutes.ToString()) + ":" + localTime.TimeOfDay.Seconds.ToString();

            if (TimeSpan.Parse(time) > TimeSpan.Parse("14:00:00"))
            {
                if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(localTime.Date.DayOfWeek) == FraytePickUpDay.Saturday)
                {
                    xmlWriter.WriteStartElement("CollectionDate");
                    xmlWriter.WriteString(DateTime.Parse(date).AddDays(2).ToString("yyyy-MM-dd"));
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("CollectionReadyTime");
                    xmlWriter.WriteString("14:00:00");
                    xmlWriter.WriteEndElement();
                }
                else
                {
                    xmlWriter.WriteStartElement("CollectionDate");
                    xmlWriter.WriteString(DateTime.Parse(date).AddDays(1).ToString("yyyy-MM-dd"));
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("CollectionReadyTime");
                    xmlWriter.WriteString("14:00:00");
                    xmlWriter.WriteEndElement();
                }
            }
            else if (TimeSpan.Parse(time) < TimeSpan.Parse("14:00:00"))
            {
                if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(localTime.Date.DayOfWeek) == FraytePickUpDay.Saturday)
                {
                    xmlWriter.WriteStartElement("CollectionDate");
                    xmlWriter.WriteString(DateTime.Parse(date).AddDays(2).ToString("yyyy-MM-dd"));
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("CollectionReadyTime");
                    xmlWriter.WriteString("14:00:00");
                    xmlWriter.WriteEndElement();
                }
                else
                {
                    xmlWriter.WriteStartElement("CollectionDate");
                    xmlWriter.WriteString(DateTime.Parse(date).AddDays(1).ToString("yyyy-MM-dd"));
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("CollectionReadyTime");
                    xmlWriter.WriteString("14:00:00");
                    xmlWriter.WriteEndElement();
                }
            }
            else
            {
                xmlWriter.WriteStartElement("CollectionDate");
                xmlWriter.WriteString(date);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("CollectionReadyTime");
                xmlWriter.WriteString(time);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteStartElement("LocationCloseTime");
            xmlWriter.WriteString("17:00:00");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CollectionAddress");
            xmlWriter.WriteStartElement("ContactName");
            xmlWriter.WriteString(request.CollectionAddress.FirstName + " " + request.CollectionAddress.LastName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CompanyName");
            xmlWriter.WriteString(!string.IsNullOrWhiteSpace(request.CollectionAddress.CompanyName) ? request.CollectionAddress.CompanyName : (request.CollectionAddress.FirstName + " " + request.CollectionAddress.LastName));
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Email");
            if (!string.IsNullOrWhiteSpace(request.CollectionAddress.Email))
            {
                xmlWriter.WriteString(request.CollectionAddress.Email);
                xmlWriter.WriteEndElement();
            }
            else
            {
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteStartElement("Phone");
            xmlWriter.WriteString(!string.IsNullOrWhiteSpace(request.CollectionAddress.Phone) ? request.CollectionAddress.Phone : "");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Address1");
            xmlWriter.WriteString(request.CollectionAddress.Address1);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Address2");
            if (!string.IsNullOrWhiteSpace(request.CollectionAddress.Address2))
            {
                xmlWriter.WriteString(request.CollectionAddress.Address2);
                xmlWriter.WriteEndElement();
            }
            else
            {
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteStartElement("City");
            xmlWriter.WriteString(!string.IsNullOrWhiteSpace(request.CollectionAddress.City) ? request.CollectionAddress.City : "");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Postcode");
            xmlWriter.WriteString(request.CollectionAddress.PostCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Country");
            xmlWriter.WriteString(request.CollectionAddress.Country);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("AddressType");
            xmlWriter.WriteString(ParcelHubAddressType.Residential);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("DeliveryAddress");

            xmlWriter.WriteStartElement("ContactName");
            xmlWriter.WriteString(request.DeliveryAddress.FirstName + " " + request.DeliveryAddress.LastName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CompanyName");
            xmlWriter.WriteString(!string.IsNullOrWhiteSpace(request.DeliveryAddress.CompanyName) ? request.DeliveryAddress.CompanyName : (request.DeliveryAddress.FirstName + " " + request.DeliveryAddress.LastName));
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Email");
            if (!string.IsNullOrWhiteSpace(request.DeliveryAddress.Email))
            {
                xmlWriter.WriteString(request.DeliveryAddress.Email);
                xmlWriter.WriteEndElement();
            }
            else
            {
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteStartElement("Phone");
            xmlWriter.WriteString(!string.IsNullOrWhiteSpace(request.DeliveryAddress.Phone) ? request.DeliveryAddress.Phone : "");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Address1");
            xmlWriter.WriteString(request.DeliveryAddress.Address1);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Address2");
            if (!string.IsNullOrWhiteSpace(request.DeliveryAddress.Address2))
            {
                xmlWriter.WriteString(request.DeliveryAddress.Address2);
                xmlWriter.WriteEndElement();
            }
            else
            {
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteStartElement("City");
            xmlWriter.WriteString(request.DeliveryAddress.City);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Postcode");
            xmlWriter.WriteString(request.DeliveryAddress.PostCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Country");
            xmlWriter.WriteString(request.DeliveryAddress.Country);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("AddressType");
            xmlWriter.WriteString(ParcelHubAddressType.Residential);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Reference1");
            xmlWriter.WriteString(request.Reference1);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Reference2");
            if (!string.IsNullOrWhiteSpace(request.Reference2))
            {
                xmlWriter.WriteString(request.Reference2);
                xmlWriter.WriteEndElement();
            }
            else
            {
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteStartElement("ContentsDescription");
            xmlWriter.WriteString(!string.IsNullOrWhiteSpace(request.ContentsDescription) ? request.ContentsDescription : "");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Packages");

            foreach (var package in request.Package)
            {
                xmlWriter.WriteStartElement("Package");
                xmlWriter.WriteStartElement("PackageType");
                xmlWriter.WriteString("Parcel");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Dimensions");

                if (request.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
                {
                    xmlWriter.WriteStartElement("Length");
                    xmlWriter.WriteString(package.ParcelDimensions.Length.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Width");
                    xmlWriter.WriteString(package.ParcelDimensions.Width.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Height");
                    xmlWriter.WriteString(package.ParcelDimensions.Height.ToString());
                    xmlWriter.WriteEndElement();
                }
                else if (request.PackageCalculationType == FraytePakageCalculationType.LbToInchs)
                {
                    xmlWriter.WriteStartElement("Length");
                    xmlWriter.WriteString((package.ParcelDimensions.Length * 2.54m).ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Width");
                    xmlWriter.WriteString((package.ParcelDimensions.Width * 2.54m).ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Height");
                    xmlWriter.WriteString((package.ParcelDimensions.Height * 2.54m).ToString());
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement(); //Dimension Tag Close

                xmlWriter.WriteStartElement("Weight");
                xmlWriter.WriteString(package.Weight.HasValue ? (package.Weight.Value / request.Package.Count).ToString() : "0.00");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Value");
                xmlWriter.WriteAttributeString("Currency", package.Value.ParcelCurrency);
                xmlWriter.WriteString(package.Value.Amount.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Contents");
                xmlWriter.WriteString(package.ParcelDimensions.Contents);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement(); //Package Tag Close
            }

            xmlWriter.WriteEndElement(); //Packages Tag Close

            xmlWriter.WriteStartElement("Enhancements");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("ShipmentTags");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("ProductTypeCode");
            xmlWriter.WriteEndElement();
        }

        internal string Authentication(string UserName, string Password)
        {
            string response = string.Empty;
            try
            {
                string credentials = String.Format("{0}", "grant_type=password&username=" + UserName + "&password=" + Password + "");
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(credentials);
                string base64 = Convert.ToBase64String(bytes);
                string authorization = base64;

                string url = string.Empty;

                try
                {
                    if (AppSettings.ApplicationMode == FrayteApplicationMode.Test)
                    {
                        url = ParcelHubTestRequestUrl.TestAuthenticationUrl;
                    }
                    else
                    {
                        url = ParcelHubLiveRequestUrl.LiveAuthenticationUrl;
                    }

                    HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);
                    webReq.Method = "POST";
                    webReq.ContentLength = bytes.Length;
                    webReq.ContentType = "application/x-www-form-urlencoded";
                    webReq.Headers.Add("Authorization", authorization);

                    System.IO.Stream os = webReq.GetRequestStream();
                    os.Write(bytes, 0, bytes.Length);
                    os.Close();

                    HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse();
                    Stream answer = webResp.GetResponseStream();
                    StreamReader _answer = new StreamReader(answer);
                    return _answer.ReadToEnd();
                }
                catch (Exception ex)
                {
                    return response;
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        internal string GetServices(string UserName, string authenticate, string xml_in)
        {
            string response = string.Empty;
            string url = string.Empty;

            try
            {
                var token = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenValue>(authenticate);
                byte[] bytes = System.Text.ASCIIEncoding.UTF8.GetBytes(xml_in);

                if (AppSettings.ApplicationMode == FrayteApplicationMode.Test)
                {
                    url = ParcelHubTestRequestUrl.TestGetServicesUrl + "=" + UserName;
                }
                else
                {
                    url = ParcelHubLiveRequestUrl.LiveGetServicesUrl + "=" + UserName;
                }

                WebRequest webReq = HttpWebRequest.Create(url);
                webReq.Method = "POST";
                webReq.ContentType = "application/xml; charset=utf-8";
                webReq.ContentLength = bytes.Length;
                webReq.Headers.Add("Accept-Encoding", "gzip");
                webReq.Headers.Add("username", UserName);
                webReq.Headers.Add("Authorization", "bearer " + token.access_token);

                System.IO.Stream os = webReq.GetRequestStream();
                os.Write(bytes, 0, bytes.Length);
                os.Close();

                WebResponse webResp = webReq.GetResponse();
                Stream answer = webResp.GetResponseStream();
                StreamReader _answer = new StreamReader(answer, System.Text.Encoding.UTF8);
                var strResponse = _answer.ReadToEnd();
                answer.Close();
                return strResponse;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        internal string CreateFinalShipmentRequestNodes(ParcelHubShipmentRequest request, GetServices services, string folderpath, ref ParcelHubResponse response)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Encoding = new UTF8Encoding(true);
            xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;

            xmlWriterSettings.Indent = true;

            if (System.IO.Directory.Exists(folderpath))
            {
                if (File.Exists(folderpath + "/ParcelHubFinal.xml"))
                {
                    File.Delete(folderpath + "/ParcelHubFinal.xml");
                }
            }
            else
            {
                System.IO.Directory.CreateDirectory(folderpath);
            }

            using (var xmlWriter = XmlWriter.Create(folderpath + "/ParcelHubFinal.xml"))
            {
                if (AppSettings.ApplicationMode == FrayteApplicationMode.Test)
                {
                    xmlWriter.WriteStartElement("Shipment", ParcelHubTestRequestUrl.TestStartXmlTag);
                }
                else
                {
                    xmlWriter.WriteStartElement("Shipment", ParcelHubLiveRequestUrl.LiveStartXmlTag);
                }

                CreateFinalRequestNode(xmlWriter, services, request, ref response);

                xmlWriter.WriteEndElement();
            }

            return folderpath + "/ParcelHubFinal.xml";
        }

        internal void CreateFinalRequestNode(XmlWriter xmlWriter, GetServices services, ParcelHubShipmentRequest request, ref ParcelHubResponse response)
        {
            xmlWriter.WriteStartElement("Account");
            xmlWriter.WriteString(request.Security.UserName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CollectionDetails");

            TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            DateTimeOffset localServerTime = DateTimeOffset.Now;
            DateTimeOffset localTime = TimeZoneInfo.ConvertTime(localServerTime, info);
            string date = localTime.Date.ToString("yyyy-MM-dd");
            string time = localTime.TimeOfDay.Hours.ToString() + ":" + (localTime.TimeOfDay.Minutes.ToString().Length == 1 ? "0" + localTime.TimeOfDay.Minutes.ToString() : localTime.TimeOfDay.Minutes.ToString()) + ":" + localTime.TimeOfDay.Seconds.ToString();

            if (TimeSpan.Parse(time) > TimeSpan.Parse("14:00:00"))
            {
                if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(localTime.Date.DayOfWeek) == FraytePickUpDay.Saturday)
                {
                    xmlWriter.WriteStartElement("CollectionDate");
                    xmlWriter.WriteString(DateTime.Parse(date).AddDays(2).ToString("yyyy-MM-dd"));
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("CollectionReadyTime");
                    xmlWriter.WriteString("14:00:00");
                    xmlWriter.WriteEndElement();
                }
                else
                {
                    xmlWriter.WriteStartElement("CollectionDate");
                    xmlWriter.WriteString(DateTime.Parse(date).AddDays(1).ToString("yyyy-MM-dd"));
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("CollectionReadyTime");
                    xmlWriter.WriteString("14:00:00");
                    xmlWriter.WriteEndElement();
                }
            }
            else if (TimeSpan.Parse(time) < TimeSpan.Parse("14:00:00"))
            {
                if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(localTime.Date.DayOfWeek) == FraytePickUpDay.Saturday)
                {
                    xmlWriter.WriteStartElement("CollectionDate");
                    xmlWriter.WriteString(DateTime.Parse(date).AddDays(2).ToString("yyyy-MM-dd"));
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("CollectionReadyTime");
                    xmlWriter.WriteString("14:00:00");
                    xmlWriter.WriteEndElement();
                }
                else
                {
                    xmlWriter.WriteStartElement("CollectionDate");
                    xmlWriter.WriteString(DateTime.Parse(date).AddDays(1).ToString("yyyy-MM-dd"));
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("CollectionReadyTime");
                    xmlWriter.WriteString("14:00:00");
                    xmlWriter.WriteEndElement();
                }
            }
            else
            {
                xmlWriter.WriteStartElement("CollectionDate");
                xmlWriter.WriteString(date);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("CollectionReadyTime");
                xmlWriter.WriteString(time);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteStartElement("LocationCloseTime");
            xmlWriter.WriteString("17:00:00");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement(); // Close Collection Detail

            xmlWriter.WriteStartElement("CollectionAddress");
            xmlWriter.WriteStartElement("ContactName");
            xmlWriter.WriteString(request.CollectionAddress.FirstName + " " + request.CollectionAddress.LastName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CompanyName");
            xmlWriter.WriteString(!string.IsNullOrWhiteSpace(request.CollectionAddress.CompanyName) ? request.CollectionAddress.CompanyName : (request.CollectionAddress.FirstName + " " + request.CollectionAddress.LastName));
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Email");
            if (!string.IsNullOrWhiteSpace(request.CollectionAddress.Email))
            {
                xmlWriter.WriteString(request.CollectionAddress.Email);
                xmlWriter.WriteEndElement();
            }
            else
            {
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteStartElement("Phone");
            xmlWriter.WriteString(!string.IsNullOrWhiteSpace(request.CollectionAddress.Phone) ? request.CollectionAddress.Phone : "");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Address1");
            xmlWriter.WriteString(request.CollectionAddress.Address1);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Address2");
            if (!string.IsNullOrWhiteSpace(request.CollectionAddress.Address2))
            {
                xmlWriter.WriteString(request.CollectionAddress.Address2);
                xmlWriter.WriteEndElement();
            }
            else
            {
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteStartElement("City");
            xmlWriter.WriteString(request.CollectionAddress.City);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Postcode");
            xmlWriter.WriteString(request.CollectionAddress.PostCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Country");
            xmlWriter.WriteString(request.CollectionAddress.Country);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("AddressType");
            xmlWriter.WriteString(ParcelHubAddressType.Residential);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement(); // Collection Address Close

            xmlWriter.WriteStartElement("DeliveryAddress");

            xmlWriter.WriteStartElement("ContactName");
            xmlWriter.WriteString(request.DeliveryAddress.FirstName + " " + request.DeliveryAddress.LastName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CompanyName");
            xmlWriter.WriteString(!string.IsNullOrWhiteSpace(request.DeliveryAddress.CompanyName) ? request.DeliveryAddress.CompanyName : (request.DeliveryAddress.FirstName + " " + request.DeliveryAddress.LastName));
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Email");
            if (!string.IsNullOrWhiteSpace(request.DeliveryAddress.Email))
            {
                xmlWriter.WriteString(request.DeliveryAddress.Email);
                xmlWriter.WriteEndElement();
            }
            else
            {
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteStartElement("Phone");
            xmlWriter.WriteString(!string.IsNullOrWhiteSpace(request.DeliveryAddress.Phone) ? request.DeliveryAddress.Phone : "");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Address1");
            xmlWriter.WriteString(request.DeliveryAddress.Address1);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Address2");
            if (!string.IsNullOrWhiteSpace(request.DeliveryAddress.Address2))
            {
                xmlWriter.WriteString(request.DeliveryAddress.Address2);
                xmlWriter.WriteEndElement();
            }
            else
            {
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteStartElement("City");
            xmlWriter.WriteString(request.DeliveryAddress.City);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Postcode");
            xmlWriter.WriteString(request.DeliveryAddress.PostCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Country");
            xmlWriter.WriteString(request.DeliveryAddress.Country);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("AddressType");
            xmlWriter.WriteString(ParcelHubAddressType.Residential);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement(); // Delivery address close

            xmlWriter.WriteStartElement("Reference1");
            xmlWriter.WriteString(request.Reference1);
            xmlWriter.WriteEndElement(); // Reference1 close

            xmlWriter.WriteStartElement("Reference2");
            if (!string.IsNullOrWhiteSpace(request.Reference2))
            {
                xmlWriter.WriteString(request.Reference2);
                xmlWriter.WriteEndElement(); // Reference2 close    
            }
            else
            {
                xmlWriter.WriteEndElement(); // Reference2 close
            }

            xmlWriter.WriteStartElement("ContentsDescription");
            xmlWriter.WriteString(!string.IsNullOrWhiteSpace(request.ContentsDescription) ? request.ContentsDescription : "");
            xmlWriter.WriteEndElement(); // Contents Description close

            xmlWriter.WriteStartElement("Packages");

            foreach (var package in request.Package)
            {
                xmlWriter.WriteStartElement("Package");
                xmlWriter.WriteStartElement("PackageType");
                xmlWriter.WriteString("Parcel");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Dimensions");

                if (request.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
                {
                    xmlWriter.WriteStartElement("Length");
                    xmlWriter.WriteString(package.ParcelDimensions.Length.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Width");
                    xmlWriter.WriteString(package.ParcelDimensions.Width.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Height");
                    xmlWriter.WriteString(package.ParcelDimensions.Height.ToString());
                    xmlWriter.WriteEndElement();
                }
                else if (request.PackageCalculationType == FraytePakageCalculationType.LbToInchs)
                {
                    xmlWriter.WriteStartElement("Length");
                    xmlWriter.WriteString((package.ParcelDimensions.Length * 2.54m).ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Width");
                    xmlWriter.WriteString((package.ParcelDimensions.Width * 2.54m).ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Height");
                    xmlWriter.WriteString((package.ParcelDimensions.Height * 2.54m).ToString());
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement(); //Dimension Tag Close

                xmlWriter.WriteStartElement("Weight");
                xmlWriter.WriteString(package.Weight.HasValue ? (package.Weight.Value / request.Package.Count).ToString() : "0.00");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Value");
                xmlWriter.WriteAttributeString("Currency", package.Value.ParcelCurrency);
                xmlWriter.WriteString(package.Value.Amount.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Contents");
                xmlWriter.WriteString(package.ParcelDimensions.Contents);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement(); //Package Tag Close
            }
            xmlWriter.WriteEndElement(); //Packages Tag Close

            xmlWriter.WriteStartElement("Enhancements");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("ShipmentTags");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("ServiceInfo");

            int j = 0;
            foreach (var gs in services.GetServicesResponse.Services.Service)
            {
                if (gs.ServiceProviderName.Trim() == FrayteCourierAccountCode.Yodel.ToUpper())
                {
                    string YodelService = CommonConversion.ConvertFirstLetterCaps(gs.ServiceProviderName.Trim());
                    if (YodelService == request.CourierName && gs.ServiceIds.ServiceId == request.CourierAccountNo.Trim())
                    {
                        xmlWriter.WriteStartElement("ServiceId");
                        xmlWriter.WriteString(gs.ServiceIds.ServiceId);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("ServiceProviderId");
                        xmlWriter.WriteString(gs.ServiceIds.ServiceProviderId);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("ServiceCustomerUID");
                        xmlWriter.WriteString(gs.ServiceIds.ServiceCustomerUID);
                        xmlWriter.WriteEndElement();
                        j++;
                        break;
                    }
                }
                if (gs.ServiceProviderName.Trim() == request.CourierAccountCountryCode && gs.ServiceIds.ServiceId == request.CourierAccountNo.Trim())
                {
                    xmlWriter.WriteStartElement("ServiceId");
                    xmlWriter.WriteString(gs.ServiceIds.ServiceId);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("ServiceProviderId");
                    xmlWriter.WriteString(gs.ServiceIds.ServiceProviderId);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("ServiceCustomerUID");
                    xmlWriter.WriteString(gs.ServiceIds.ServiceCustomerUID);
                    xmlWriter.WriteEndElement();
                    j++;
                    break;
                }
            }
            if (j == 0)
            {
                var desc = services.GetServicesResponse.Services.Service.Where(p => p.ServiceProviderName.Trim() == request.CourierAccountCountryCode ||
                                                                               p.ServiceProviderName.Trim() == request.CourierAccountCountryCode.ToUpper()).FirstOrDefault();
                if (desc != null)
                {
                    xmlWriter.WriteStartElement("ServiceId");
                    xmlWriter.WriteString(desc.ServiceIds.ServiceId);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("ServiceProviderId");
                    xmlWriter.WriteString(desc.ServiceIds.ServiceProviderId);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("ServiceCustomerUID");
                    xmlWriter.WriteString(desc.ServiceIds.ServiceCustomerUID);
                    xmlWriter.WriteEndElement();
                }
                else
                {
                    response.Error.Miscellaneous.Add("No services available. Please contact to sales@frayte.co.uk for query");
                }
            }

            xmlWriter.WriteEndElement(); // Close Service Info

            xmlWriter.WriteStartElement("ProductTypeCode");
            xmlWriter.WriteEndElement();
        }

        internal string CreateShipmentInParcelHub(string body, string authenticate, string UserName)
        {
            string serverUrl = string.Empty;

            try
            {
                var token = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenValue>(authenticate);
                byte[] bytes = System.Text.ASCIIEncoding.UTF8.GetBytes(body);

                if (AppSettings.ApplicationMode == FrayteApplicationMode.Test)
                {
                    serverUrl = ParcelHubTestRequestUrl.TestFinalUrl;
                }
                else
                {
                    serverUrl = ParcelHubLiveRequestUrl.LiveFinalUrl;
                }

                WebRequest requestRate = HttpWebRequest.Create(serverUrl);
                requestRate.Method = "POST";
                requestRate.ContentType = "application/xml; charset=utf-8";
                requestRate.ContentLength = bytes.Length;
                requestRate.Headers.Add("Accept-Encoding", "gzip");
                requestRate.Headers.Add("username", UserName);
                requestRate.Headers.Add("Authorization", "bearer " + token.access_token);

                using (var stream = requestRate.GetRequestStream())
                {
                    var arrBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(body);
                    stream.Write(arrBytes, 0, arrBytes.Length);
                    stream.Close();
                }

                WebResponse responseRate = requestRate.GetResponse();
                var respStream = responseRate.GetResponseStream();
                var reader = new StreamReader(respStream, System.Text.Encoding.ASCII);
                string strResponse = reader.ReadToEnd();
                respStream.Close();
                return strResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region MappingObjects

        internal void MapParcelHubShipFrom(DirectBookingShipmentDraftDetail directBookingDetail, ParcelHubShippingAddress FromAddress)
        {
            FromAddress.FirstName = directBookingDetail.ShipFrom.FirstName;
            FromAddress.LastName = directBookingDetail.ShipFrom.LastName;
            FromAddress.CompanyName = ((directBookingDetail.ShipFrom.CompanyName == "" || directBookingDetail.ShipFrom.CompanyName == null) ? (directBookingDetail.ShipFrom.FirstName + " " + directBookingDetail.ShipFrom.LastName) : directBookingDetail.ShipFrom.CompanyName);
            FromAddress.Email = directBookingDetail.ShipFrom.Email;
            FromAddress.Phone = directBookingDetail.ShipFrom.Phone;
            FromAddress.Address1 = directBookingDetail.ShipFrom.Address;
            FromAddress.Address2 = directBookingDetail.ShipFrom.Address2;
            FromAddress.City = directBookingDetail.ShipFrom.City;
            FromAddress.PostCode = directBookingDetail.ShipFrom.PostCode;
            FromAddress.Country = directBookingDetail.ShipFrom.Country.Code2;
        }


        internal void MapParcelHubShipTo(DirectBookingShipmentDraftDetail directBookingDetail, ParcelHubShippingAddress ToAddress)
        {
            ToAddress.FirstName = directBookingDetail.ShipTo.FirstName;
            ToAddress.LastName = directBookingDetail.ShipTo.LastName;
            ToAddress.CompanyName = ((directBookingDetail.ShipTo.CompanyName == "" || directBookingDetail.ShipTo.CompanyName == null) ? (directBookingDetail.ShipTo.FirstName + " " + directBookingDetail.ShipTo.LastName) : directBookingDetail.ShipTo.CompanyName);
            ToAddress.Address1 = directBookingDetail.ShipTo.Address;
            ToAddress.Address2 = directBookingDetail.ShipTo.Address2;
            ToAddress.Area = directBookingDetail.ShipTo.Area;
            ToAddress.City = directBookingDetail.ShipTo.City;
            ToAddress.PostCode = directBookingDetail.ShipTo.PostCode;
            ToAddress.Country = directBookingDetail.ShipTo.Country.Code2;
            ToAddress.Phone = directBookingDetail.ShipTo.Phone;
        }

        internal void MapParcelHubPackageDetail(DirectBookingShipmentDraftDetail directBookingDetail, List<ParcelHubPackageDetail> _package)
        {
            if (directBookingDetail.Packages != null)
            {
                if (directBookingDetail.Packages.Count > 0)
                {
                    ParcelHubPackageDetail package;
                    foreach (var mp in directBookingDetail.Packages)
                    {
                        package = new ParcelHubPackageDetail();
                        for (int i = 0; i < mp.CartoonValue; i++)
                        {
                            package.ParcelDimensions = new ParcelHubDimensions();
                            if (directBookingDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms)
                            {
                                package.ParcelDimensions.Height = Convert.ToInt32(mp.Height);
                                package.ParcelDimensions.Length = Convert.ToInt32(mp.Length);
                                package.ParcelDimensions.Width = Convert.ToInt32(mp.Width);
                                package.ParcelDimensions.Contents = mp.Content;
                                package.Weight = directBookingDetail.CustomerRateCard.Weight;
                                package.ParcelDimensions.Cartonvalue = mp.CartoonValue / mp.CartoonValue;
                                package.Value = new ParcelHubAmountOfMoney() { Amount = mp.Value, ParcelCurrency = directBookingDetail.Currency.CurrencyCode };
                                package.Contents = mp.Content;
                            }
                            else if (directBookingDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs)
                            {
                                package.ParcelDimensions.Height = Convert.ToInt32(mp.Height * 2.54m);
                                package.ParcelDimensions.Length = Convert.ToInt32(mp.Length * 2.54m);
                                package.ParcelDimensions.Width = Convert.ToInt32(mp.Width * 2.54m);
                                package.ParcelDimensions.Contents = mp.Content;
                                package.Weight = directBookingDetail.CustomerRateCard.Weight * 0.453592m;
                                package.ParcelDimensions.Cartonvalue = mp.CartoonValue / mp.CartoonValue;
                                package.Value = new ParcelHubAmountOfMoney() { Amount = mp.Value, ParcelCurrency = directBookingDetail.Currency.CurrencyCode };
                                package.Contents = mp.Content;
                            }

                            //Step9: Package Custom Declaration for international shipment or loacl shipment
                            package.PackageCustomsDeclaration = new ParcelHubPackageCustomsDeclaration();
                            if (directBookingDetail.ShipFrom.Country.Code != directBookingDetail.ShipTo.Country.Code)
                            {
                                package.PackageCustomsDeclaration.ContentsDescription = directBookingDetail.CustomInfo.ContentsType;
                                package.PackageCustomsDeclaration.CountryOfOrigin = directBookingDetail.ShipFrom.Country.Code2;
                                package.PackageCustomsDeclaration.HSTariffNumber = "Test";
                                package.PackageCustomsDeclaration.Quantity = "1";
                                package.PackageCustomsDeclaration.Value = new ParcelHubAmountOfMoney() { Amount = mp.Value, ParcelCurrency = directBookingDetail.Currency.CurrencyCode };
                                package.PackageCustomsDeclaration.Weight = directBookingDetail.CustomerRateCard.Weight.ToString("0.##");
                            }
                            else
                            {
                                package.PackageCustomsDeclaration.ContentsDescription = "";
                                package.PackageCustomsDeclaration.CountryOfOrigin = "";
                                package.PackageCustomsDeclaration.HSTariffNumber = "";
                                package.PackageCustomsDeclaration.Quantity = "1";
                                package.PackageCustomsDeclaration.Value = new ParcelHubAmountOfMoney() { Amount = mp.Value, ParcelCurrency = directBookingDetail.Currency.CurrencyCode };
                                package.PackageCustomsDeclaration.Weight = directBookingDetail.CustomerRateCard.Weight.ToString("0.##");
                            }
                            _package.Add(package);
                        }
                    }
                }
            }
        }

        internal void MapParcelHubCustomInfo(DirectBookingShipmentDraftDetail directBookingDetail, ParcelHubShipmentCustomeInfo CustomInfo)
        {
            CustomInfo.CategoryOfItem = directBookingDetail.CustomInfo.CatagoryOfItem;
            CustomInfo.SendersCustomsReference = "";
            if (directBookingDetail.CustomInfo.TermOfTrade == FrayteTermsOfTrade.Receiver)
            {
                CustomInfo.TermsOfTrade = "DutiesAndTaxesUnpaid";
                CustomInfo.CategoryOfItemExplanation = directBookingDetail.CustomInfo.CatagoryOfItemExplanation;
                CustomInfo.CertificateNumber = "Test";
                CustomInfo.Comments = "Test";
                CustomInfo.ImportersContactDetails = "Test";
                CustomInfo.ImportersReference = "Test";
                CustomInfo.InvoiceNumber = "Test";
                CustomInfo.LicenseNumber = "Test";
                CustomInfo.OfficeOfOrigin = "Test";
                CustomInfo.PostalCharges = new ParcelHubAmountOfMoney() { Amount = 0, ParcelCurrency = directBookingDetail.ShipTo.Country.Code };
            }
            else if (directBookingDetail.CustomInfo.TermOfTrade == FrayteTermsOfTrade.Shipper || directBookingDetail.CustomInfo.TermOfTrade == FrayteTermsOfTrade.ThirdParty)
            {
                CustomInfo.TermsOfTrade = "DutiesAndTaxesPaid";
                CustomInfo.CategoryOfItemExplanation = directBookingDetail.CustomInfo.CatagoryOfItemExplanation;
                CustomInfo.CertificateNumber = "Test";
                CustomInfo.Comments = "Test";
                CustomInfo.ImportersContactDetails = "Test";
                CustomInfo.ImportersReference = "Test";
                CustomInfo.InvoiceNumber = "Test";
                CustomInfo.LicenseNumber = "Test";
                CustomInfo.OfficeOfOrigin = "Test";
                CustomInfo.PostalCharges = new ParcelHubAmountOfMoney() { Amount = 0, ParcelCurrency = directBookingDetail.ShipFrom.Country.Code };
            }
        }

        #endregion

        #region ExpressMapping

        /// <summary>
        /// Avinash
        /// 17-Apr-2019
        /// </summary>        

        internal void MapExpressParcelHubShipFrom(ExpressShipmentModel expressBookingDetail, ParcelHubShippingAddress FromAddress)
        {
            FromAddress.FirstName = expressBookingDetail.ShipFrom.FirstName;
            FromAddress.LastName = expressBookingDetail.ShipFrom.LastName;
            FromAddress.CompanyName = ((expressBookingDetail.ShipFrom.CompanyName == "" || expressBookingDetail.ShipFrom.CompanyName == null) ? (expressBookingDetail.ShipFrom.FirstName + " " + expressBookingDetail.ShipFrom.LastName) : expressBookingDetail.ShipFrom.CompanyName);
            FromAddress.Email = expressBookingDetail.ShipFrom.Email;
            FromAddress.Phone = expressBookingDetail.ShipFrom.Phone;
            FromAddress.Address1 = expressBookingDetail.ShipFrom.Address;
            FromAddress.Address2 = expressBookingDetail.ShipFrom.Address2;
            FromAddress.City = expressBookingDetail.ShipFrom.City;
            FromAddress.PostCode = expressBookingDetail.ShipFrom.PostCode;
            FromAddress.Country = expressBookingDetail.ShipFrom.Country.Code2;
        }

        internal void MapExpressParcelHubShipTo(ExpressShipmentModel expressBookingDetail, ParcelHubShippingAddress ToAddress)
        {
            ToAddress.FirstName = expressBookingDetail.ShipTo.FirstName;
            ToAddress.LastName = expressBookingDetail.ShipTo.LastName;
            ToAddress.CompanyName = ((expressBookingDetail.ShipTo.CompanyName == "" || expressBookingDetail.ShipTo.CompanyName == null) ? (expressBookingDetail.ShipTo.FirstName + " " + expressBookingDetail.ShipTo.LastName) : expressBookingDetail.ShipTo.CompanyName);
            ToAddress.Address1 = expressBookingDetail.ShipTo.Address;
            ToAddress.Address2 = expressBookingDetail.ShipTo.Address2;
            ToAddress.Area = expressBookingDetail.ShipTo.Area;
            ToAddress.City = expressBookingDetail.ShipTo.City;
            ToAddress.PostCode = expressBookingDetail.ShipTo.PostCode;
            ToAddress.Country = expressBookingDetail.ShipTo.Country.Code2;
            ToAddress.Phone = expressBookingDetail.ShipTo.Phone;
        }

        internal void MapExpressParcelHubPackageDetail(ExpressShipmentModel expressBookingDetail, List<ParcelHubPackageDetail> _package)
        {
            if (expressBookingDetail.Packages != null)
            {
                if (expressBookingDetail.Packages.Count > 0)
                {
                    ParcelHubPackageDetail package;
                    foreach (var mp in expressBookingDetail.Packages)
                    {
                        package = new ParcelHubPackageDetail();
                        for (int i = 0; i < mp.CartonValue; i++)
                        {
                            package.ParcelDimensions = new ParcelHubDimensions();
                            if (expressBookingDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms)
                            {
                                package.ParcelDimensions.Height = Convert.ToInt32(mp.Height);
                                package.ParcelDimensions.Length = Convert.ToInt32(mp.Length);
                                package.ParcelDimensions.Width = Convert.ToInt32(mp.Width);
                                package.ParcelDimensions.Contents = mp.Content;
                                package.Weight = expressBookingDetail.ActualWeight;
                                package.ParcelDimensions.Cartonvalue = mp.CartonValue / mp.CartonValue;
                                package.Value = new ParcelHubAmountOfMoney() { Amount = mp.Value, ParcelCurrency = expressBookingDetail.DeclaredCurrency.CurrencyCode };
                                package.Contents = mp.Content;
                            }
                            else if (expressBookingDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs)
                            {
                                package.ParcelDimensions.Height = Convert.ToInt32(mp.Height * 2.54m);
                                package.ParcelDimensions.Length = Convert.ToInt32(mp.Length * 2.54m);
                                package.ParcelDimensions.Width = Convert.ToInt32(mp.Width * 2.54m);
                                package.ParcelDimensions.Contents = mp.Content;
                                package.Weight = expressBookingDetail.ActualWeight * 0.453592m;
                                package.ParcelDimensions.Cartonvalue = mp.CartonValue / mp.CartonValue;
                                package.Value = new ParcelHubAmountOfMoney() { Amount = mp.Value, ParcelCurrency = expressBookingDetail.DeclaredCurrency.CurrencyCode };
                                package.Contents = mp.Content;
                            }

                            //Step9: Package Custom Declaration for international shipment or loacl shipment
                            package.PackageCustomsDeclaration = new ParcelHubPackageCustomsDeclaration();
                            if (expressBookingDetail.ShipFrom.Country.Code != expressBookingDetail.ShipTo.Country.Code)
                            {
                                package.PackageCustomsDeclaration.ContentsDescription = expressBookingDetail.CustomInformation.ContentsType;
                                package.PackageCustomsDeclaration.CountryOfOrigin = expressBookingDetail.ShipFrom.Country.Code2;
                                package.PackageCustomsDeclaration.HSTariffNumber = "Test";
                                package.PackageCustomsDeclaration.Quantity = "1";
                                package.PackageCustomsDeclaration.Value = new ParcelHubAmountOfMoney() { Amount = mp.Value, ParcelCurrency = expressBookingDetail.DeclaredCurrency.CurrencyCode };
                                package.PackageCustomsDeclaration.Weight = expressBookingDetail.ActualWeight.ToString("0.##");
                            }
                            else
                            {
                                package.PackageCustomsDeclaration.ContentsDescription = "";
                                package.PackageCustomsDeclaration.CountryOfOrigin = "";
                                package.PackageCustomsDeclaration.HSTariffNumber = "";
                                package.PackageCustomsDeclaration.Quantity = "1";
                                package.PackageCustomsDeclaration.Value = new ParcelHubAmountOfMoney() { Amount = mp.Value, ParcelCurrency = expressBookingDetail.DeclaredCurrency.CurrencyCode };
                                package.PackageCustomsDeclaration.Weight = expressBookingDetail.ActualWeight.ToString("0.##");
                            }
                            _package.Add(package);
                        }
                    }
                }
            }
        }

        internal void MapExpressParcelHubCustomInfo(ExpressShipmentModel expressBookingDetail, ParcelHubShipmentCustomeInfo CustomInfo)
        {
            CustomInfo.CategoryOfItem = expressBookingDetail.CustomInformation.CatagoryOfItem;
            expressBookingDetail.CustomInformation.TermOfTrade = expressBookingDetail.PayTaxAndDuties;
            CustomInfo.SendersCustomsReference = "";
            if (expressBookingDetail.CustomInformation.TermOfTrade == FrayteTermsOfTrade.Receiver)
            {
                CustomInfo.TermsOfTrade = "DutiesAndTaxesUnpaid";
                CustomInfo.CategoryOfItemExplanation = expressBookingDetail.CustomInformation.CatagoryOfItemExplanation;
                CustomInfo.CertificateNumber = "Test";
                CustomInfo.Comments = "Test";
                CustomInfo.ImportersContactDetails = "Test";
                CustomInfo.ImportersReference = "Test";
                CustomInfo.InvoiceNumber = "Test";
                CustomInfo.LicenseNumber = "Test";
                CustomInfo.OfficeOfOrigin = "Test";
                CustomInfo.PostalCharges = new ParcelHubAmountOfMoney() { Amount = 0, ParcelCurrency = expressBookingDetail.ShipTo.Country.Code };
            }
            else if (expressBookingDetail.CustomInformation.TermOfTrade == FrayteTermsOfTrade.Shipper || expressBookingDetail.CustomInformation.TermOfTrade == FrayteTermsOfTrade.ThirdParty)
            {
                CustomInfo.TermsOfTrade = "DutiesAndTaxesPaid";
                CustomInfo.CategoryOfItemExplanation = expressBookingDetail.CustomInformation.CatagoryOfItemExplanation;
                CustomInfo.CertificateNumber = "Test";
                CustomInfo.Comments = "Test";
                CustomInfo.ImportersContactDetails = "Test";
                CustomInfo.ImportersReference = "Test";
                CustomInfo.InvoiceNumber = "Test";
                CustomInfo.LicenseNumber = "Test";
                CustomInfo.OfficeOfOrigin = "Test";
                CustomInfo.PostalCharges = new ParcelHubAmountOfMoney() { Amount = 0, ParcelCurrency = expressBookingDetail.ShipFrom.Country.Code };
            }
        }

        #endregion

        #region SaveDetail

        public IntegrtaionResult MappingParcelHubToIntegrationResult(DirectBookingShipmentDraftDetail dbDetail, ParcelHubResponse response, string Error)
        {
            IntegrtaionResult integrtaionResult = new IntegrtaionResult();
            if (response.Error.Status)
            {
                if (dbDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Yodel)
                {
                    integrtaionResult.CourierName = FrayteCourierCompany.Yodel;
                }
                else if (dbDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Hermes)
                {
                    integrtaionResult.CourierName = FrayteCourierCompany.Hermes;
                }
                else if (dbDetail.CustomerRateCard.CourierName == FrayteCourierCompany.UKMail)
                {
                    integrtaionResult.CourierName = FrayteCourierCompany.UKMail;
                }
                integrtaionResult.TrackingNumber = response.ShippingInfo.CourierTrackingNumber;
                integrtaionResult.PieceTrackingDetails = new List<CourierPieceDetail>();
                foreach (var item in response.Parcel)
                {
                    for (int i = 0; i < item.Bytes.Count; i++)
                    {
                        var PieceTrackingDetails = new CourierPieceDetail();
                        PieceTrackingDetails.ImageByte = item.Bytes[i].ImageBytes;
                        PieceTrackingDetails.PieceTrackingNumber = item.PackageNumber[i].CourierTrackingNumber;
                        integrtaionResult.PieceTrackingDetails.Add(PieceTrackingDetails);
                    }
                }
                integrtaionResult.Status = true;
            }
            else
            {
                integrtaionResult.Error = new FratyteError();
                {
                    integrtaionResult.Error = response.Error;
                }
                integrtaionResult.ErrorCode = new List<FrayteApiError>();
                {
                    List<FrayteApiError> _api = new FrayteApiErrorCodeRepository().SaveFinilizeApiError(integrtaionResult.Error, dbDetail.CustomerRateCard.CourierName, Error);
                    integrtaionResult.ErrorCode = _api;
                }
                integrtaionResult.Status = false;
            }

            return integrtaionResult;
        }

        public void MappingCourierPieceDetail(IntegrtaionResult integrtaionResult, DirectBookingShipmentDraftDetail directBookingDetail, int DirectShipmentid)
        {
            try
            {
                if (DirectShipmentid > 0)
                {
                    int k = 0, i = 0;
                    List<int> _shiId = new List<int>();
                    foreach (var Obj in directBookingDetail.Packages)
                    {
                        _shiId = new DirectShipmentRepository().GetDirectShipmentDetailID(DirectShipmentid);
                        for (int j = 1; j <= Obj.CartoonValue; j++)
                        {
                            integrtaionResult.PieceTrackingDetails[k].DirectShipmentDetailId = _shiId[i];
                            k++;
                        }
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool SaveTrackingDetail(DirectBookingShipmentDraftDetail directBookingDetail, IntegrtaionResult result, int DirectShipmentid)
        {
            try
            {
                var count = 1;
                foreach (var Obj in result.PieceTrackingDetails)
                {
                    Frayte.Services.Models.Package package = new Frayte.Services.Models.Package();
                    package.DirectShipmentDetailId = Obj.DirectShipmentDetailId;
                    package.LabelName = Obj.PieceTrackingNumber;
                    new DirectShipmentRepository().SavePackageDetail(package, "", Obj.DirectShipmentDetailId, directBookingDetail.CustomerRateCard.CourierName, count);
                    count++;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public FratyteError DownloadParcelHubPackageImage(DirectBookingShipmentDraftDetail dbDetail, IntegrtaionResult result, int DirectShipmentid)
        {
            FratyteError error = new FratyteError();
            try
            {
                int k = 0, i = 0;
                List<int> _shiId = new List<int>();
                error.Miscellaneous = new List<string>();
                int totalCount = dbDetail.Packages.Sum(p => p.CartoonValue);
                int n = 0;
                foreach (var Obj in dbDetail.Packages)
                {
                    _shiId = new DirectShipmentRepository().GetDirectShipmentDetailID(DirectShipmentid);
                    for (int j = 0; j < Obj.CartoonValue; j++)
                    {
                        n++;
                        string Image = string.Empty;
                        if (result.PieceTrackingDetails == null)
                        {
                            error.Miscellaneous.Add("Shipment can't create due to incorrect address format or wrong post code");
                            error.IsMailSend = true;
                        }
                        else
                        {
                            if (AppSettings.LabelSave == "")
                            {
                                byte[] image = Convert.FromBase64String(result.PieceTrackingDetails[k].ImageByte);

                                System.Drawing.Image labelimage;
                                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(image))
                                {
                                    labelimage = System.Drawing.Image.FromStream(ms);
                                    string labelName = string.Empty;

                                    if (dbDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Yodel)
                                    {
                                        labelName = FrayteShortName.Yodel;
                                    }
                                    else if (dbDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Hermes)
                                    {
                                        labelName = FrayteShortName.Hermes;
                                    }
                                    else if (dbDetail.CustomerRateCard.CourierName == FrayteCourierCompany.UKMail)
                                    {
                                        labelName = FrayteShortName.UKMail;
                                    }
                                    //Step16.1: Create direcory for save package label
                                    if (System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/"))
                                    {
                                        Image = labelName + "_" + result.PieceTrackingDetails[k].PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + n + " of " + totalCount + ")" + ".jpg";
                                        System.Drawing.Bitmap bmpUploadedImage = new System.Drawing.Bitmap(labelimage);
                                        System.Drawing.Image thumbnailImage = bmpUploadedImage.GetThumbnailImage(700, 900, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
                                        string extension = Path.GetExtension(Image);
                                        string FileName = Path.GetFileName(Image);
                                        string[] ff = FileName.ToString().Split('.');
                                        string name = ff[0].ToString();
                                        thumbnailImage.Save(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/" + name + extension);
                                    }
                                    else
                                    {
                                        Image = labelName + "_" + result.PieceTrackingDetails[k].PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + n + " of " + totalCount + ")" + ".jpg";
                                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/");
                                        System.Drawing.Bitmap bmpUploadedImage = new System.Drawing.Bitmap(labelimage);
                                        System.Drawing.Image thumbnailImage = bmpUploadedImage.GetThumbnailImage(700, 900, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
                                        string extension = Path.GetExtension(Image);
                                        string FileName = Path.GetFileName(Image);
                                        string[] ff = FileName.ToString().Split('.');
                                        string name = ff[0].ToString();
                                        thumbnailImage.Save(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/" + name + extension);
                                    }
                                }

                                Frayte.Services.Models.Package package = new Frayte.Services.Models.Package();
                                package.LabelName = result.PieceTrackingDetails[k].PieceTrackingNumber;

                                //Update PackageImage Name In PackageTrackingDetail Table
                                int m = 0;
                                new DirectShipmentRepository().SavePackageDetail(package, Image, _shiId[i], dbDetail.CustomerRateCard.CourierName, m);
                                k++;
                            }
                            else
                            {
                                byte[] image = Convert.FromBase64String(result.PieceTrackingDetails[k].ImageByte);

                                System.Drawing.Image labelimage;
                                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(image))
                                {
                                    labelimage = System.Drawing.Image.FromStream(ms);
                                    string labelName = string.Empty;

                                    if (dbDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Yodel)
                                    {
                                        labelName = FrayteShortName.Yodel;
                                    }
                                    else if (dbDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Hermes)
                                    {
                                        labelName = FrayteShortName.Hermes;
                                    }
                                    else if (dbDetail.CustomerRateCard.CourierName == FrayteCourierCompany.UKMail)
                                    {
                                        labelName = FrayteShortName.UKMail;
                                    }
                                    //Step16.1: Create direcory for save package label
                                    if (System.IO.Directory.Exists(AppSettings.LabelFolder + "/" + DirectShipmentid + "/"))
                                    {
                                        Image = labelName + "_" + result.PieceTrackingDetails[k].PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + n + " of " + totalCount + ")" + ".jpg";
                                        System.Drawing.Bitmap bmpUploadedImage = new System.Drawing.Bitmap(labelimage);
                                        System.Drawing.Image thumbnailImage = bmpUploadedImage.GetThumbnailImage(700, 1200, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
                                        string extension = Path.GetExtension(Image);
                                        string FileName = Path.GetFileName(Image);
                                        string[] ff = FileName.ToString().Split('.');
                                        string name = ff[0].ToString();
                                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                                        {
                                            thumbnailImage.Save(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + name + extension);
                                        }
                                        else
                                        {
                                            thumbnailImage.Save(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + name + extension));
                                        }

                                    }
                                    else
                                    {
                                        Image = labelName + "_" + result.PieceTrackingDetails[k].PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + n + " of " + totalCount + ")" + ".jpg";
                                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                                        {
                                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "/" + DirectShipmentid + "/");
                                        }
                                        else
                                        {
                                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/"));
                                        }
                                        System.Drawing.Bitmap bmpUploadedImage = new System.Drawing.Bitmap(labelimage);
                                        System.Drawing.Image thumbnailImage = bmpUploadedImage.GetThumbnailImage(700, 1200, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
                                        string extension = Path.GetExtension(Image);
                                        string FileName = Path.GetFileName(Image);
                                        string[] ff = FileName.ToString().Split('.');
                                        string name = ff[0].ToString();
                                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                                        {
                                            thumbnailImage.Save(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + name + extension);
                                        }
                                        else
                                        {
                                            thumbnailImage.Save(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + name + extension));
                                        }
                                    }
                                }

                                Frayte.Services.Models.Package package = new Frayte.Services.Models.Package();
                                package.LabelName = result.PieceTrackingDetails[k].PieceTrackingNumber;

                                //Update PackageImage Name In PackageTrackingDetail Table
                                int m = 0;
                                new DirectShipmentRepository().SavePackageDetail(package, Image, _shiId[i], dbDetail.CustomerRateCard.CourierName, m);
                                k++;
                            }
                        }
                    }
                    i++;
                }
                return error;
            }
            catch (Exception ex)
            {
                error.Status = false;
                throw;
            }
        }

        internal bool ThumbnailCallback()
        {
            return true;
        }

        internal void GetXMLFromRequestObject(object o)
        {
            dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(o);
        }

        #endregion

        /// <summary>
        /// Modification Done By Avinash
        /// </summary>
        /// <param name="expressBookingDetail"></param>
        /// <param name="result"></param>
        /// <param name="ExpressId"></param>
        /// <returns></returns>

        #region Avinash 17-Apr-2019 & 18-Apr-2019

        public FratyteError DownloadExpressParcelHubPackageImage(ExpressShipmentModel expressBookingDetail, IntegrtaionResult result, int ExpressId)
        {
            FratyteError error = new FratyteError();
            try
            {
                int k = 0, i = 0;
                List<int> _shiId = new List<int>();
                error.Miscellaneous = new List<string>();
                int totalCount = expressBookingDetail.Packages.Sum(p => p.CartonValue);
                int n = 0;
                foreach (var Obj in expressBookingDetail.Packages)
                {
                    _shiId = new DirectShipmentRepository().GetExpressDetailID(ExpressId);
                    for (int j = 0; j < Obj.CartonValue; j++)
                    {
                        n++;
                        string Image = string.Empty;
                        if (result.PieceTrackingDetails == null)
                        {
                            error.Miscellaneous.Add("Shipment can't create due to incorrect address format or wrong post code");
                            error.IsMailSend = true;
                        }
                        else
                        {
                            if (AppSettings.LabelSave == "")
                            {
                                byte[] image = Convert.FromBase64String(result.PieceTrackingDetails[k].ImageByte);

                                System.Drawing.Image labelimage;
                                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(image))
                                {
                                    labelimage = System.Drawing.Image.FromStream(ms);
                                    string labelName = string.Empty;

                                    if (expressBookingDetail.Service.HubCarrier == FrayteCourierCompany.Yodel)
                                    {
                                        labelName = FrayteShortName.Yodel;
                                    }
                                    else if (expressBookingDetail.Service.HubCarrier == FrayteCourierCompany.Hermes)
                                    {
                                        labelName = FrayteShortName.Hermes;
                                    }
                                    else if (expressBookingDetail.Service.HubCarrier == FrayteCourierCompany.UKMail)
                                    {
                                        labelName = FrayteShortName.UKMail;
                                    }
                                    //Step16.1: Create direcory for save package label
                                    if (System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressId + "/"))
                                    {
                                        Image = labelName + "_" + result.PieceTrackingDetails[k].PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + n + " of " + totalCount + ")" + ".jpg";
                                        System.Drawing.Bitmap bmpUploadedImage = new System.Drawing.Bitmap(labelimage);
                                        System.Drawing.Image thumbnailImage = bmpUploadedImage.GetThumbnailImage(700, 900, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
                                        string extension = Path.GetExtension(Image);
                                        string FileName = Path.GetFileName(Image);
                                        string[] ff = FileName.ToString().Split('.');
                                        string name = ff[0].ToString();
                                        thumbnailImage.Save(AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressId + "/" + name + extension);
                                    }
                                    else
                                    {
                                        Image = labelName + "_" + result.PieceTrackingDetails[k].PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + n + " of " + totalCount + ")" + ".jpg";
                                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressId + "/");
                                        System.Drawing.Bitmap bmpUploadedImage = new System.Drawing.Bitmap(labelimage);
                                        System.Drawing.Image thumbnailImage = bmpUploadedImage.GetThumbnailImage(700, 900, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
                                        string extension = Path.GetExtension(Image);
                                        string FileName = Path.GetFileName(Image);
                                        string[] ff = FileName.ToString().Split('.');
                                        string name = ff[0].ToString();
                                        thumbnailImage.Save(AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressId + "/" + name + extension);
                                    }
                                }

                                Frayte.Services.Models.Package package = new Frayte.Services.Models.Package();
                                package.LabelName = result.PieceTrackingDetails[k].PieceTrackingNumber;

                                //Update PackageImage Name In PackageTrackingDetail Table
                                int m = 0;
                                new DirectShipmentRepository().SaveExpressPackageDetail(package, Image, _shiId[i], expressBookingDetail.Service.HubCarrier, m);
                                k++;
                            }
                            else
                            {
                                byte[] image = Convert.FromBase64String(result.PieceTrackingDetails[k].ImageByte);

                                System.Drawing.Image labelimage;
                                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(image))
                                {
                                    labelimage = System.Drawing.Image.FromStream(ms);
                                    string labelName = string.Empty;

                                    if (expressBookingDetail.Service.HubCarrier == FrayteCourierCompany.Yodel)
                                    {
                                        labelName = FrayteShortName.Yodel;
                                    }
                                    else if (expressBookingDetail.Service.HubCarrier == FrayteCourierCompany.Hermes)
                                    {
                                        labelName = FrayteShortName.Hermes;
                                    }
                                    else if (expressBookingDetail.Service.HubCarrier == FrayteCourierCompany.UKMail)
                                    {
                                        labelName = FrayteShortName.UKMail;
                                    }
                                    //Step16.1: Create direcory for save package label
                                    if (System.IO.Directory.Exists(AppSettings.LabelFolder + "/Express/" + ExpressId + "/"))
                                    {
                                        Image = labelName + "_" + result.PieceTrackingDetails[k].PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + n + " of " + totalCount + ")" + ".jpg";
                                        System.Drawing.Bitmap bmpUploadedImage = new System.Drawing.Bitmap(labelimage);
                                        System.Drawing.Image thumbnailImage = bmpUploadedImage.GetThumbnailImage(700, 1200, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
                                        string extension = Path.GetExtension(Image);
                                        string FileName = Path.GetFileName(Image);
                                        string[] ff = FileName.ToString().Split('.');
                                        string name = ff[0].ToString();
                                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                                        {
                                            thumbnailImage.Save(AppSettings.LabelFolder + "/Express/" + ExpressId + "/" + name + extension);
                                        }
                                        else
                                        {
                                            thumbnailImage.Save(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + ExpressId + "/" + name + extension));
                                        }

                                    }
                                    else
                                    {
                                        Image = labelName + "_" + result.PieceTrackingDetails[k].PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + n + " of " + totalCount + ")" + ".jpg";
                                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                                        {
                                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "/Express/" + ExpressId + "/");
                                        }
                                        else
                                        {
                                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + ExpressId + "/"));
                                        }
                                        System.Drawing.Bitmap bmpUploadedImage = new System.Drawing.Bitmap(labelimage);
                                        System.Drawing.Image thumbnailImage = bmpUploadedImage.GetThumbnailImage(700, 1200, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
                                        string extension = Path.GetExtension(Image);
                                        string FileName = Path.GetFileName(Image);
                                        string[] ff = FileName.ToString().Split('.');
                                        string name = ff[0].ToString();
                                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                                        {
                                            thumbnailImage.Save(AppSettings.LabelFolder + "/Express/" + ExpressId + "/" + name + extension);
                                        }
                                        else
                                        {
                                            thumbnailImage.Save(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + ExpressId + "/" + name + extension));
                                        }
                                    }
                                }

                                Frayte.Services.Models.Package package = new Frayte.Services.Models.Package();
                                package.LabelName = result.PieceTrackingDetails[k].PieceTrackingNumber;

                                //Update PackageImage Name In PackageTrackingDetail Table
                                int m = 0;
                                new DirectShipmentRepository().SaveExpressPackageDetail(package, Image, _shiId[i], expressBookingDetail.Service.HubCarrier, m);
                                k++;
                            }
                        }
                    }
                    i++;
                }
                return error;
            }
            catch (Exception ex)
            {
                error.Status = false;
                throw;
            }
        }

        public bool SaveExpressTrackingDetail(ExpressShipmentModel expressBookingDetail, IntegrtaionResult result, int ExpressId)
        {
            try
            {
                var count = 1;
                foreach (var Obj in result.PieceTrackingDetails)
                {
                    Frayte.Services.Models.Package package = new Frayte.Services.Models.Package();
                    package.DirectShipmentDetailId = Obj.DirectShipmentDetailId;
                    package.LabelName = Obj.PieceTrackingNumber;
                    new DirectShipmentRepository().SaveExpressPackageDetail(package, "", Obj.DirectShipmentDetailId, expressBookingDetail.CarrierService, count);
                    count++;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void MappingExpressCourierPieceDetail(IntegrtaionResult integrtaionResult, ExpressShipmentModel expressBookingDetail, int ExpressId)
        {
            try
            {
                if (ExpressId > 0)
                {
                    int k = 0, i = 0;
                    List<int> _shiId = new List<int>();
                    foreach (var Obj in expressBookingDetail.Packages)
                    {
                        _shiId = new DirectShipmentRepository().GetExpressDetailID(ExpressId);
                        for (int j = 1; j <= Obj.CartonValue; j++)
                        {
                            integrtaionResult.PieceTrackingDetails[k].DirectShipmentDetailId = _shiId[i];
                            k++;
                        }
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IntegrtaionResult MappingExpressParcelHubToIntegrationResult(ExpressShipmentModel dbDetail, ParcelHubResponse response, string Error)
        {
            IntegrtaionResult integrtaionResult = new IntegrtaionResult();
            if (response.Error.Status)
            {
                if (dbDetail.Service.HubCarrier == FrayteCourierCompany.Yodel)
                {
                    integrtaionResult.CourierName = FrayteCourierCompany.Yodel;
                }
                else if (dbDetail.Service.HubCarrier == FrayteCourierCompany.Hermes)
                {
                    integrtaionResult.CourierName = FrayteCourierCompany.Hermes;
                }
                else if (dbDetail.Service.HubCarrier == FrayteCourierCompany.UKMail)
                {
                    integrtaionResult.CourierName = FrayteCourierCompany.UKMail;
                }
                integrtaionResult.TrackingNumber = response.ShippingInfo.CourierTrackingNumber;

                integrtaionResult.PieceTrackingDetails = new List<CourierPieceDetail>();
                foreach (var item in response.Parcel)
                {
                    for (int i = 0; i < item.Bytes.Count; i++)
                    {
                        var PieceTrackingDetails = new CourierPieceDetail();
                        PieceTrackingDetails.ImageByte = item.Bytes[i].ImageBytes;
                        PieceTrackingDetails.PieceTrackingNumber = item.PackageNumber[i].CourierTrackingNumber;
                        integrtaionResult.PieceTrackingDetails.Add(PieceTrackingDetails);
                    }
                }
                integrtaionResult.Status = true;
            }
            else
            {
                integrtaionResult.Error = new FratyteError();
                {
                    integrtaionResult.Error = response.Error;
                }
                integrtaionResult.ErrorCode = new List<FrayteApiError>();
                {
                    List<FrayteApiError> _api = new FrayteApiErrorCodeRepository().SaveFinilizeApiError(integrtaionResult.Error, dbDetail.CarrierService, Error);
                    integrtaionResult.ErrorCode = _api;
                }
                integrtaionResult.Status = false;
            }

            return integrtaionResult;
        }

        public ParcelHubShipmentRequest MapExpressBookingDetailToShipmentRequest(ExpressShipmentModel expressBookingDetail)
        {
            try
            {
                //Step 1: Map ShipFrom Address
                ParcelHubShippingAddress FromAddress = new ParcelHubShippingAddress();
                MapExpressParcelHubShipFrom(expressBookingDetail, FromAddress);

                //Step 2: Map ShipTo Address
                ParcelHubShippingAddress ToAddress = new ParcelHubShippingAddress();
                MapExpressParcelHubShipTo(expressBookingDetail, ToAddress);

                //Step 3: Map PackageDetail
                List<ParcelHubPackageDetail> _package = new List<ParcelHubPackageDetail>();
                MapExpressParcelHubPackageDetail(expressBookingDetail, _package);

                //Step 4: Map CustomInfo
                ParcelHubShipmentCustomeInfo CustomInfo = new ParcelHubShipmentCustomeInfo();
                MapExpressParcelHubCustomInfo(expressBookingDetail, CustomInfo);

                //Step 5: Get ParcelHub UserName and Password From DB
                FrayteLogisticIntegration logisticIntegration;
                if (expressBookingDetail.Service.HubCarrier == "Hermes")
                {
                    logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.HERMES);
                }
                else
                {
                    logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.ParcelHub);
                }

                ParcelHubShipmentRequest parcelhubShipmentRequest = new ParcelHubShipmentRequest();
                if (logisticIntegration != null)
                {
                    parcelhubShipmentRequest.Security = new ParcelHubSecurity();
                    parcelhubShipmentRequest.Security.UserName = logisticIntegration.UserName;
                    parcelhubShipmentRequest.Security.Password = logisticIntegration.Password;
                    parcelhubShipmentRequest.Security.UserAgent = logisticIntegration.UserAgent;
                    parcelhubShipmentRequest.Security.ServiceUrl = logisticIntegration.ServiceUrl;
                    parcelhubShipmentRequest.DraftShipmentId = expressBookingDetail.ExpressId;
                    parcelhubShipmentRequest.ContentsDescription = expressBookingDetail.AWBNumber;
                    parcelhubShipmentRequest.Reference1 = expressBookingDetail.AWBNumber.Replace(" ", "").Substring(0, 5) + "-" + expressBookingDetail.ShipmentReference;
                    parcelhubShipmentRequest.Reference2 = expressBookingDetail.ShipmentReference;
                    parcelhubShipmentRequest.PackageCalculationType = expressBookingDetail.PakageCalculatonType;
                    parcelhubShipmentRequest.CollectionAddress = new ParcelHubShippingAddress();
                    parcelhubShipmentRequest.CollectionAddress = FromAddress;
                    parcelhubShipmentRequest.DeliveryAddress = new ParcelHubShippingAddress();
                    parcelhubShipmentRequest.DeliveryAddress = ToAddress;
                    parcelhubShipmentRequest.Package = new List<ParcelHubPackageDetail>();
                    parcelhubShipmentRequest.Package = _package;
                    parcelhubShipmentRequest.CustomInfo = new ParcelHubShipmentCustomeInfo();
                    parcelhubShipmentRequest.CustomInfo = CustomInfo;

                    //here we have to add CourierAccountCountryCode 
                    parcelhubShipmentRequest.CourierAccountCountryCode = expressBookingDetail.ShipTo.Country.Code;
                    parcelhubShipmentRequest.CourierAccountNo = expressBookingDetail.Service.CourierAccountNo;
                    parcelhubShipmentRequest.CourierName = expressBookingDetail.Service.HubCarrier;
                    parcelhubShipmentRequest.CourierDescription = expressBookingDetail.ShipmentDescription;
                    parcelhubShipmentRequest.CustomerId = expressBookingDetail.CustomerId;
                }

                return parcelhubShipmentRequest;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region  Hermes Integration

        public ParcelHubResponse CreateHermesShipment(ParcelHubShipmentRequest request)
        {
            ParcelHubResponse response = new ParcelHubResponse();
            response.Error = new FratyteError();
            response.Error.Custom = new List<string>();
            response.Error.Package = new List<string>();
            response.Error.Address = new List<string>();
            response.Error.Service = new List<string>();
            response.Error.ServiceError = new List<string>();
            response.Error.Miscellaneous = new List<string>();
            response.Error.MiscErrors = new List<FrayteKeyValue>();

            try
            {
                string folderpath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");

                string shipmentXML = CreateHermesShipmentRequestNodes(request, folderpath);

                string xml_in = File.ReadAllText(@shipmentXML);

                var shipmentResult = CreateHermesShipment(xml_in, request.Security.ServiceUrl, request.Security.UserName, request.Security.Password);

                try
                {
                    //Step9: Save Parcel Hub Response                        
                    if (System.IO.Directory.Exists(folderpath))
                    {
                        if (File.Exists(folderpath + "/HermesResponse.xml"))
                        {
                            System.GC.Collect();
                            System.GC.WaitForPendingFinalizers();
                            File.Delete(folderpath + "/HermesResponse.xml");
                            File.Create(folderpath + "/HermesResponse.xml").Close();
                            File.WriteAllText(folderpath + "/HermesResponse.xml", shipmentResult);
                        }
                        else
                        {
                            File.Create(folderpath + "/HermesResponse.xml").Close();
                            File.WriteAllText(folderpath + "/HermesResponse.xml", shipmentResult);
                        }
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(folderpath);
                        File.Create(folderpath + "/HermesResponse.xml").Close();
                        File.WriteAllText(folderpath + "/HermesResponse.xml", shipmentResult);
                    }

                    XmlDataDocument data = new XmlDataDocument();
                    FileStream fs = new FileStream(folderpath + "/HermesResponse.xml", FileMode.Open, FileAccess.Read);
                    data.Load(fs);

                    List<ParcelResponse> _res = new List<ParcelResponse>();
                    ParcelResponse res = new ParcelResponse();

                    List<PackageTrackingNumber> _bar = new List<PackageTrackingNumber>();
                    PackageTrackingNumber bar;

                    XmlNodeList xmlNodeList = data.GetElementsByTagName("barcodeNumber");

                    if (xmlNodeList.Count > 0)
                    {
                        for (int i = 0; i < xmlNodeList.Count; i++)
                        {
                            foreach (XmlNode node in xmlNodeList[i].ChildNodes)
                            {
                                bar = new PackageTrackingNumber();
                                bar.CourierTrackingNumber = node.InnerText;
                                _bar.Add(bar);
                            }
                        }

                        if (_bar.Count > 1)
                        {
                            for (int i = _bar.Count - 1; i >= 1; i--)
                            {
                                _bar.RemoveAt(i);
                            }
                        }
                        res.PackageNumber = _bar;

                        List<ImageData> _ll = new List<ImageData>();
                        ImageData ll;
                        XmlNodeList xmlNodeList1 = data.GetElementsByTagName("labelImage");

                        for (int i = 0; i < xmlNodeList1.Count; i++)
                        {
                            foreach (XmlNode node in xmlNodeList1[i].ChildNodes)
                            {
                                ll = new ImageData();
                                ll.ImageBytes = node.InnerText;
                                _ll.Add(ll);
                            }
                        }

                        if (_ll.Count > 1)
                        {
                            for (int i = _ll.Count - 1; i >= 1; i--)
                            {
                                _ll.RemoveAt(i);
                            }
                        }
                        res.Bytes = _ll;
                        _res.Add(res);

                        response.ShippingInfo = new Models.ParcelHub.ShippingInfo();
                        response.ShippingInfo.CourierTrackingNumber = _res[0].PackageNumber[0].CourierTrackingNumber;

                        response.Parcel = new List<ParcelResponse>();
                        response.Parcel = _res;
                        response.Error.IsMailSend = false;
                        response.Error.Status = true;

                        ConvertbyteArrayToPdf(_res, request.DraftShipmentId);
                    }
                    else
                    {
                        XmlNodeList errorNodeList = data.GetElementsByTagName("errorDescription");
                        if (errorNodeList.Count > 0)
                        {
                            for (int j = 0; j < errorNodeList.Count; j++)
                            {
                                foreach (XmlNode node in errorNodeList[j].ChildNodes)
                                {
                                    response.Error.IsMailSend = true;
                                    response.Error.Status = false;
                                    response.Error.Miscellaneous.Add(node.InnerText);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.Error.IsMailSend = true;
                    response.Error.Status = false;
                    response.Error.Miscellaneous.Add(ex.Message);
                }
            }
            catch (Exception ex)
            {
                response.Error.Miscellaneous.Add(ex.Message);
            }

            return response;
        }

        public string CreateHermesShipmentRequestNodes(ParcelHubShipmentRequest request, string folderpath)
        {
            if (System.IO.Directory.Exists(folderpath))
            {
                if (File.Exists(folderpath + "/HermesRequest.xml"))
                {
                    File.Delete(folderpath + "/HermesRequest.xml");
                }
            }
            else
            {
                System.IO.Directory.CreateDirectory(folderpath);
            }

            using (var xmlWriter = XmlWriter.Create(folderpath + "/HermesRequest.xml"))
            {
                CreateHermesRequestNode(xmlWriter, request);
            }

            return folderpath + "/HermesRequest.xml";
        }

        public void CreateHermesRequestNode(XmlWriter xmlWriter, ParcelHubShipmentRequest request)
        {

            xmlWriter.WriteStartElement("deliveryRoutingRequest");//deliveryRoutingRequest tag

            xmlWriter.WriteStartElement("clientId");
            xmlWriter.WriteString("798");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("clientName");
            xmlWriter.WriteString(request.Security.UserAgent);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("sourceOfRequest");
            xmlWriter.WriteString("Hermes");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("deliveryRoutingRequestEntries");//deliveryRoutingRequestEntries tag
            xmlWriter.WriteStartElement("deliveryRoutingRequestEntry");//deliveryRoutingRequestEntry tag
            xmlWriter.WriteStartElement("customer");//customer tag
            xmlWriter.WriteStartElement("address");//address tag

            xmlWriter.WriteStartElement("title");
            xmlWriter.WriteString("");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("firstName");
            xmlWriter.WriteString(!string.IsNullOrWhiteSpace(request.DeliveryAddress.CompanyName) ? request.DeliveryAddress.CompanyName : request.DeliveryAddress.FirstName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("lastName");
            xmlWriter.WriteString(!string.IsNullOrWhiteSpace(request.DeliveryAddress.CompanyName) ? (request.DeliveryAddress.FirstName + " " + request.DeliveryAddress.LastName) : request.DeliveryAddress.LastName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("houseNo");
            xmlWriter.WriteString("");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("streetName");
            xmlWriter.WriteString(request.DeliveryAddress.Address1 + "" + request.DeliveryAddress.Address2 + "" + request.DeliveryAddress.Area);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("countryCode");
            xmlWriter.WriteString(request.DeliveryAddress.Country);// country code
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("postCode");
            xmlWriter.WriteString(request.DeliveryAddress.PostCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("city");
            xmlWriter.WriteString(!string.IsNullOrWhiteSpace(request.DeliveryAddress.City) ? request.DeliveryAddress.City : "");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();//address tag Close

            xmlWriter.WriteStartElement("mobilePhoneNo");
            xmlWriter.WriteString(!string.IsNullOrWhiteSpace(request.DeliveryAddress.Phone) ? request.DeliveryAddress.Phone : "");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("email");
            if (!string.IsNullOrWhiteSpace(request.CollectionAddress.Email))
            {
                xmlWriter.WriteString(request.CollectionAddress.Email);
                xmlWriter.WriteEndElement();
            }
            else
            {
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteStartElement("customerReference1");
            xmlWriter.WriteString(request.Reference1);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("customerReference2");
            xmlWriter.WriteString(request.Reference2);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();//customer tag Close

            foreach (var package in request.Package)
            {
                xmlWriter.WriteStartElement("parcel");//parcel tag

                if (request.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
                {
                    xmlWriter.WriteStartElement("weight");//grams
                    xmlWriter.WriteString(package.Weight.HasValue ? (package.Weight.Value * 1000).ToString() : "0.00");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("length");//cm
                    xmlWriter.WriteString(package.ParcelDimensions.Length.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("width");//cm
                    xmlWriter.WriteString(package.ParcelDimensions.Width.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("depth");//cm
                    xmlWriter.WriteString(package.ParcelDimensions.Height.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("girth");
                    xmlWriter.WriteString("0");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("combinedDimension");
                    xmlWriter.WriteString("0");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("volume");
                    xmlWriter.WriteString("0");//length * width * depth
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("value");
                    xmlWriter.WriteString(package.Value.Amount.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("numberOfParts");
                    xmlWriter.WriteString(package.ParcelDimensions.Cartonvalue.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("numberOfItems");
                    xmlWriter.WriteString(package.ParcelDimensions.Cartonvalue.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("description");
                    xmlWriter.WriteString(!string.IsNullOrWhiteSpace(package.ParcelDimensions.Contents) ? package.ParcelDimensions.Contents : "");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("originOfParcel");
                    xmlWriter.WriteString(request.CollectionAddress.Country);
                    xmlWriter.WriteEndElement();
                }
                else if (request.PackageCalculationType == FraytePakageCalculationType.LbToInchs)
                {
                    xmlWriter.WriteStartElement("weight");//grams
                    xmlWriter.WriteString(package.Weight.HasValue ? (package.Weight.Value / 453.592m).ToString() : "0.00");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("length");//cm
                    xmlWriter.WriteString(((package.ParcelDimensions.Length) * 2.54m).ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("width");//cm
                    xmlWriter.WriteString(((package.ParcelDimensions.Width) * 2.54m).ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("depth");//cm
                    xmlWriter.WriteString(((package.ParcelDimensions.Height) * 2.54m).ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("girth");
                    xmlWriter.WriteString("0");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("combinedDimension");
                    xmlWriter.WriteString("0");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("volume");
                    xmlWriter.WriteString("0");//length * width * depth
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("value");
                    xmlWriter.WriteString(package.Value.Amount.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("numberOfParts");
                    xmlWriter.WriteString(package.ParcelDimensions.Cartonvalue.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("numberOfItems");
                    xmlWriter.WriteString(package.ParcelDimensions.Cartonvalue.ToString());
                    xmlWriter.WriteEndElement();


                    xmlWriter.WriteStartElement("description");
                    xmlWriter.WriteString(!string.IsNullOrWhiteSpace(package.ParcelDimensions.Contents) ? package.ParcelDimensions.Contents : "");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("originOfParcel");
                    xmlWriter.WriteString(request.CollectionAddress.Country);
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();//parcel tag 
            }

            if (request.CollectionAddress.Country == "GB" && request.DeliveryAddress.Country == "GB")
            {
                xmlWriter.WriteStartElement("services");// service

                xmlWriter.WriteStartElement("nextDay");//next day service
                xmlWriter.WriteString("true");
                xmlWriter.WriteEndElement();//next day service tag close

                xmlWriter.WriteStartElement("signature");// signature
                xmlWriter.WriteString("true");
                xmlWriter.WriteEndElement();//signature tag close

                xmlWriter.WriteEndElement();//service tag close
            }

            else
            {
                xmlWriter.WriteStartElement("services");//service

                xmlWriter.WriteStartElement("signature");// signature
                xmlWriter.WriteString("true");
                xmlWriter.WriteEndElement();//signature tag close

                xmlWriter.WriteEndElement();//service
            }

            xmlWriter.WriteStartElement("senderAddress");//senderAddress tag

            xmlWriter.WriteStartElement("addressLine1");
            xmlWriter.WriteString(request.CollectionAddress.FirstName + " " + request.CollectionAddress.LastName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("addressLine2");
            xmlWriter.WriteString(request.CollectionAddress.Address1);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("addressLine3");
            xmlWriter.WriteString(request.CollectionAddress.Address2);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("addressLine4");
            xmlWriter.WriteString(request.CollectionAddress.City + ", " + request.CollectionAddress.PostCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();//senderAddress tag Close

            xmlWriter.WriteStartElement("expectedDespatchDate");

            DateTime date1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
            xmlWriter.WriteString(date1.ToString("yyyy-MM-ddT00:00:00"));
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("countryOfOrigin");
            xmlWriter.WriteString(request.CollectionAddress.Country);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();//deliveryRoutingRequestEntry tag Close

            xmlWriter.WriteEndElement();//deliveryRoutingRequestEntries tag Close

            xmlWriter.WriteEndElement();//deliveryRoutingRequest tag Close
        }

        private string CreateHermesShipment(string body, string serverUrl, string username, string password)
        {
            try
            {
                byte[] bytes = System.Text.ASCIIEncoding.UTF8.GetBytes(body);
                WebRequest request = HttpWebRequest.Create(serverUrl);
                request.Method = "POST";
                request.ContentType = "text/xml";
                request.ContentLength = bytes.Length;
                request.Headers.Add("Accept-Encoding", "gzip");
                using (var client = ClientHelper.GetClient(username, password))
                {
                    request.Headers.Add(client.DefaultRequestHeaders.ToString());
                }
                using (var stream = request.GetRequestStream())
                {
                    var arrBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(body);
                    stream.Write(arrBytes, 0, arrBytes.Length);
                    stream.Close();
                }

                WebResponse response = request.GetResponse();
                var respStream = response.GetResponseStream();
                var reader = new StreamReader(respStream, System.Text.Encoding.ASCII);
                string strResponse = reader.ReadToEnd();
                respStream.Close();
                return strResponse;
            }
            catch
            {
                throw;
            }
        }

        public void ConvertbyteArrayToPdf(List<ParcelResponse> _res, int ExpressId)
        {
            IntegrtaionResult result = new IntegrtaionResult();

            if (_res != null && _res.Count > 0)
            {
                List<int> _shiId = new List<int>();

                string labelName = FrayteShortName.Hermes;
                string Image = string.Empty;

                foreach (ParcelResponse hr in _res)
                {
                    _shiId = new DirectShipmentRepository().GetExpressDetailID(ExpressId);
                    for (int i = 0; i < hr.Bytes.Count; i++)
                    {
                        if (AppSettings.LabelSave == "")
                        {
                            if (System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressId + "/"))
                            {
                                Image = labelName + "_" + hr.PackageNumber[i].CourierTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + 1 + " of " + 1 + ")" + ".pdf";
                                byte[] bytes = Convert.FromBase64String(hr.Bytes[i].ImageBytes);
                                FileStream stream = new FileStream(@"" + AppSettings.FilePath + "Express/" + ExpressId + "/" + Image, FileMode.CreateNew);
                                BinaryWriter writer = new BinaryWriter(stream);
                                writer.Write(bytes, 0, bytes.Length);
                                writer.Close();
                            }
                            else
                            {
                                Image = labelName + "_" + hr.PackageNumber[i].CourierTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + 1 + " of " + 1 + ")" + ".pdf";
                                System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressId + "/");
                                byte[] bytes = Convert.FromBase64String(hr.Bytes[i].ImageBytes);
                                FileStream stream = new FileStream(@"" + AppSettings.FilePath + "Express/" + ExpressId + "/" + Image, FileMode.CreateNew);
                                BinaryWriter writer = new BinaryWriter(stream);
                                writer.Write(bytes, 0, bytes.Length);
                                writer.Close();
                            }
                        }
                        else
                        {
                            if (System.IO.Directory.Exists(AppSettings.LabelFolder + "/Express/" + ExpressId + "/"))
                            {
                                if (AppSettings.ShipmentCreatedFrom == "BATCH")
                                {
                                    Image = labelName + "_" + hr.PackageNumber[i].CourierTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + 1 + " of " + 1 + ")" + ".pdf";
                                    byte[] bytes = Convert.FromBase64String(hr.Bytes[i].ImageBytes);
                                    FileStream stream = new FileStream(@"" + AppSettings.FilePath + "Express/" + ExpressId + "/" + Image, FileMode.CreateNew);
                                    BinaryWriter writer = new BinaryWriter(stream);
                                    writer.Write(bytes, 0, bytes.Length);
                                    writer.Close();
                                }
                                else
                                {
                                    System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + ExpressId + "/"));
                                    Image = labelName + "_" + hr.PackageNumber[i].CourierTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + 1 + " of " + 1 + ")" + ".pdf";
                                    byte[] bytes = Convert.FromBase64String(hr.Bytes[i].ImageBytes);
                                    FileStream stream = new FileStream(@"" + AppSettings.FilePath + "Express/" + ExpressId + "/" + Image, FileMode.CreateNew);
                                    BinaryWriter writer = new BinaryWriter(stream);
                                    writer.Write(bytes, 0, bytes.Length);
                                    writer.Close();
                                }
                            }
                            else
                            {
                                if (AppSettings.ShipmentCreatedFrom == "BATCH")
                                {
                                    System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "/Express/" + ExpressId + "/");
                                    Image = labelName + "_" + hr.PackageNumber[i].CourierTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + 1 + " of " + 1 + ")" + ".pdf";
                                    byte[] bytes = Convert.FromBase64String(hr.Bytes[i].ImageBytes);
                                    FileStream stream = new FileStream(@"" + AppSettings.FilePath + "Express/" + ExpressId + "/" + Image, FileMode.CreateNew);
                                    BinaryWriter writer = new BinaryWriter(stream);
                                    writer.Write(bytes, 0, bytes.Length);
                                    writer.Close();
                                }
                                else
                                {
                                    System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + ExpressId + "/"));
                                    Image = labelName + "_" + hr.PackageNumber[i].CourierTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + 1 + " of " + 1 + ")" + ".pdf";
                                    byte[] bytes = Convert.FromBase64String(hr.Bytes[i].ImageBytes);
                                    FileStream stream = new FileStream(@"" + AppSettings.FilePath + "Express/" + ExpressId + "/" + Image, FileMode.CreateNew);
                                    BinaryWriter writer = new BinaryWriter(stream);
                                    writer.Write(bytes, 0, bytes.Length);
                                    writer.Close();
                                }
                            }
                        }
                        Frayte.Services.Models.Package package = new Frayte.Services.Models.Package();
                        package.LabelName = hr.PackageNumber[i].CourierTrackingNumber;
                        //Update PackageImage Name In PackageTrackingDetail Table
                        int m = 0;
                        new DirectShipmentRepository().SaveExpressPackageDetail(package, Image, _shiId[i], "Hermes", m);
                    }
                }

                //For All pdf
                if (AppSettings.LabelSave == "")
                {
                    if (System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressId + "/"))
                    {
                        Image = labelName + "_" + _res[0].PackageNumber[0].CourierTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                        byte[] allbytes = Convert.FromBase64String(_res[0].Bytes[0].ImageBytes);
                        FileStream allstream = new FileStream(@"" + AppSettings.FilePath + "Express/" + ExpressId + "/" + Image, FileMode.CreateNew);
                        BinaryWriter allwriter = new BinaryWriter(allstream);
                        allwriter.Write(allbytes, 0, allbytes.Length);
                        allwriter.Close();
                    }
                    else
                    {
                        Image = labelName + "_" + _res[0].PackageNumber[0].CourierTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                        byte[] allbytes = Convert.FromBase64String(_res[0].Bytes[0].ImageBytes);
                        FileStream allstream = new FileStream(@"" + AppSettings.FilePath + "Express/" + ExpressId + "/" + Image, FileMode.CreateNew);
                        BinaryWriter allwriter = new BinaryWriter(allstream);
                        allwriter.Write(allbytes, 0, allbytes.Length);
                        allwriter.Close();
                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(AppSettings.LabelFolder + "/Express/" + ExpressId + "/"))
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            Image = labelName + "_" + _res[0].PackageNumber[0].CourierTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                            byte[] allbytes = Convert.FromBase64String(_res[0].Bytes[0].ImageBytes);
                            FileStream allstream = new FileStream(@"" + AppSettings.FilePath + "Express/" + ExpressId + "/" + Image, FileMode.CreateNew);
                            BinaryWriter allwriter = new BinaryWriter(allstream);
                            allwriter.Write(allbytes, 0, allbytes.Length);
                            allwriter.Close();
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + ExpressId + "/"));
                            Image = labelName + "_" + _res[0].PackageNumber[0].CourierTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                            byte[] allbytes = Convert.FromBase64String(_res[0].Bytes[0].ImageBytes);
                            FileStream allstream = new FileStream(@"" + AppSettings.FilePath + "Express/" + ExpressId + "/" + Image, FileMode.CreateNew);
                            BinaryWriter allwriter = new BinaryWriter(allstream);
                            allwriter.Write(allbytes, 0, allbytes.Length);
                            allwriter.Close();
                        }
                    }
                    else
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "/Express/" + ExpressId + "/");
                            Image = labelName + "_" + _res[0].PackageNumber[0].CourierTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                            byte[] allbytes = Convert.FromBase64String(_res[0].Bytes[0].ImageBytes);
                            FileStream allstream = new FileStream(@"" + AppSettings.FilePath + "Express/" + ExpressId + "/" + Image, FileMode.CreateNew);
                            BinaryWriter allwriter = new BinaryWriter(allstream);
                            allwriter.Write(allbytes, 0, allbytes.Length);
                            allwriter.Close();
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + ExpressId + "/"));
                            Image = labelName + "_" + _res[0].PackageNumber[0].CourierTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                            byte[] allbytes = Convert.FromBase64String(_res[0].Bytes[0].ImageBytes);
                            FileStream allstream = new FileStream(@"" + AppSettings.FilePath + "Express/" + ExpressId + "/" + Image, FileMode.CreateNew);
                            BinaryWriter allwriter = new BinaryWriter(allstream);
                            allwriter.Write(allbytes, 0, allbytes.Length);
                            allwriter.Close();
                        }
                    }
                }
            }
        }

        #endregion
    }

    public static class ClientHelper
    {
        // Basic auth
        public static HttpClient GetClient(string username, string password)
        {
            var authValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{username}:{password}")));

            var client = new HttpClient()
            {
                DefaultRequestHeaders = { Authorization = authValue }
            };
            return client;
        }
    }
}
