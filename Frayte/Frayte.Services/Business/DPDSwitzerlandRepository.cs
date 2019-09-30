using Frayte.Services.Models;
using Frayte.Services.Models.ApiModal;
using Frayte.Services.Models.DPD_CH;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Linq;
using Frayte.Services.Models.Express;

namespace Frayte.Services.Business
{
    public class DPDSwitzerlandRepository
    {
        public DPDChResponseModel CreateShipment(DPDChRequestModel dpdChRequest, string ShipmentType)
        {
            DPDChResponseModel response = new DPDChResponseModel();
            var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.DPDCH);
            //API Login
            string ApiShipment = string.Empty;
            string xml_ship = string.Empty;
            string xml_in = string.Empty;
            string ApiLogin = string.Empty;

            try
            {
                #region API Login

                string XMLDPDCHLogin = CreateXMLDPDCHLogin();
                xml_in = File.ReadAllText(@XMLDPDCHLogin);
                var kk = "xmlns:ns=\"ns\"";
                xml_in = xml_in.Replace(kk, "");

                ApiLogin = CallWebservice(xml_in, logisticIntegration.LabelApiUrl);

                var result = XDocument.Parse(ApiLogin);
                if (!string.IsNullOrWhiteSpace(ApiLogin) && !ApiLogin.Contains("soap:Fault"))
                {
                    var loginResult = (from r in result.Descendants("return")
                                       select new
                                       {
                                           delisId = r.Element("delisId") != null ? r.Element("delisId").Value : "",
                                           authToken = r.Element("authToken") != null ? r.Element("authToken").Value : "",
                                           depot = r.Element("depot") != null ? r.Element("depot").Value : ""
                                       }).FirstOrDefault();
                    dpdChRequest.AuthToken = loginResult.authToken;
                    dpdChRequest.UserName = loginResult.delisId;
                    dpdChRequest.SendingDepot = loginResult.depot;
                }
                else
                {
                    var xml = XDocument.Parse(@ApiLogin);
                    var Error = (from r in xml.Descendants("detail")
                                 select new
                                 {
                                     code = r.Element("errorCode") != null ? r.Element("errorCode").Value : "",
                                     message = r.Element("ConditionData") != null ? r.Element("ConditionData").Value : "",
                                 }).ToList();

                    response.Status = false;
                    response.Error = new FratyteError();
                    response.Error.Service = new List<string>();

                    foreach (var i in Error)
                    {
                        string err = string.Empty;
                        err = i.code + "-" + i.message;
                        response.Error.ServiceError.Add(err);

                    }
                    if (ShipmentType == FrayteShipmentServiceType.DirectBooking)
                    {
                        new DirectShipmentRepository().SaveEasyPostErrorObject(@ApiLogin, xml_in, dpdChRequest.DraftShipmentId);
                    }
                }

                #endregion

                #region Order Insert

                if (logisticIntegration != null && !string.IsNullOrWhiteSpace(dpdChRequest.AuthToken))
                {
                    string xml_path = CreateXMLDPDCH(dpdChRequest);
                    xml_ship = File.ReadAllText(@xml_path);
                    var ns = "xmlns:ns=\"ns\"";
                    xml_ship = xml_ship.Replace(ns, "");
                    var ns1 = "xmlns:ns1=\"ns1\"";
                    xml_ship = xml_ship.Replace(ns1, "");

                    response.Request = xml_ship;
                    ApiShipment = CallWebservice(xml_ship, logisticIntegration.ServiceUrl);
                    response.Response = @ApiShipment;
                    var ApiShipmentresult = XDocument.Parse(ApiShipment);
                    if (!string.IsNullOrWhiteSpace(ApiShipment) && !ApiLogin.Contains("soap:Fault") && !ApiShipment.Contains("faults"))
                    {
                        var parcellabelsPDF = (from r in ApiShipmentresult.Descendants("orderResult")
                                               select new
                                               {
                                                   parcellabelsPDF = r.Element("parcellabelsPDF") != null ? r.Element("parcellabelsPDF").Value : "",

                                               }).FirstOrDefault();

                        var shipmentResponses = (from r in ApiShipmentresult.Descendants("shipmentResponses")
                                                 select new
                                                 {
                                                     mpsId = r.Element("mpsId") != null ? r.Element("mpsId").Value : "",
                                                 }).FirstOrDefault();

                        var parcelInformation = (from r in ApiShipmentresult.Descendants("parcelInformation")
                                                 select new
                                                 {
                                                     parcelLabelNumber = r.Element("parcelLabelNumber") != null ? r.Element("parcelLabelNumber").Value : "",
                                                 }).ToList();

                        response.ParcelLabelPDF = parcellabelsPDF.parcellabelsPDF;
                        response.ShipmentResponses = new ShipmentResponse();
                        response.ShipmentResponses.mpsId = shipmentResponses.mpsId;
                        response.ShipmentResponses.ParcelLabelNumber = new List<string>();
                        foreach (var Label in parcelInformation)
                        {
                            response.ShipmentResponses.ParcelLabelNumber.Add(Label.parcelLabelNumber);
                        }
                    }
                    else
                    {
                        var xml = XDocument.Parse(@ApiShipment);
                        var Error = (from r in xml.Descendants("faults")
                                     select new
                                     {
                                         code = r.Element("faultCode") != null ? r.Element("faultCode").Value : "",
                                         message = r.Element("message") != null ? r.Element("message").Value : "",
                                     }).ToList();

                        response.Status = false;
                        response.Error = new FratyteError();
                        response.Error.Service = new List<string>();

                        foreach (var i in Error)
                        {
                            string err = string.Empty;
                            err = i.code + "-" + i.message;
                            response.Error.Service.Add(err);
                        }
                        if (ShipmentType == FrayteShipmentServiceType.DirectBooking)
                        {
                            new DirectShipmentRepository().SaveEasyPostErrorObject(@ApiShipment, xml_ship, dpdChRequest.DraftShipmentId);
                        }
                    }
                }
                else
                {
                    var xml = XDocument.Parse(@ApiLogin);
                    var Error = (from r in xml.Descendants("soap:Fault")
                                 select new
                                 {
                                     code = r.Element("faultcode") != null ? r.Element("faultcode").Value : "",
                                     message = r.Element("faultstring") != null ? r.Element("faultstring").Value : "",
                                 }).ToList();

                    response.Status = false;
                    response.Error = new FratyteError();
                    response.Error.Service = new List<string>();

                    foreach (var i in Error)
                    {
                        string err = string.Empty;
                        err = i.code + "-" + i.message;
                        response.Error.Service.Add(err);
                    }
                    if (ShipmentType == FrayteShipmentServiceType.DirectBooking)
                    {
                        new DirectShipmentRepository().SaveEasyPostErrorObject(@ApiShipment, xml_ship, dpdChRequest.DraftShipmentId);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Error = new FratyteError();
                response.Error.Miscellaneous = new List<string>();
                response.Error.Miscellaneous.Add((ex.InnerException != null ? ex.InnerException.ToString() : ex.Message.ToString()));
                new DirectShipmentRepository().SaveEasyPostErrorObject("DPD-CHShipmentResult:-" + @ApiShipment + "DPD-CH API LoginResult:-" + @ApiLogin, "DPDCH-ShipXML:-" + xml_ship + "DPDCH-LoginXMl:-" + xml_in, dpdChRequest.DraftShipmentId);
            }
            return response;
        }

        public DPDChRequestModel MapDirectBookingDetailToShipmentRequestDto(DirectBookingShipmentDraftDetail directBookingDetail)
        {
            var ShipFrom = new ExpressRepository().getHubAddress(directBookingDetail.ShipTo.Country.CountryId, directBookingDetail.ShipTo.PostCode, directBookingDetail.ShipTo.State);
            DPDChRequestModel dpdChRequest = new DPDChRequestModel()
            {
                UserName = "",
                Password = "",
                AuthToken = "",
                Shipper = new ContactDetail()
                {
                    //Company = ShipFrom.CompanyName,
                    //Contact = string.IsNullOrWhiteSpace(ShipFrom.FirstName + " " + ShipFrom.LastName) ? ShipFrom.CompanyName : ShipFrom.FirstName + " " + ShipFrom.LastName,
                    //Address1 = ShipFrom.Address,
                    //Address2 = ShipFrom.Address2,
                    //Address3 = "",                   
                    //Town = ShipFrom.City,
                    //Country = ShipFrom.Country.Code2,
                    //Postcode = ShipFrom.PostCode,
                    //Telephone = ShipFrom.Phone

                    Company = "Total Freight Management GmbH",
                    Contact = "",
                    Address1 = "Fracht West",
                    Address2 = "Entrance 1, 2nd Fl., Office 2-327",
                    Address3 = "",
                    Town = "Zurich Airport",
                    Country = "CH",
                    Postcode = "8058",
                    Telephone = "+41 44 816 40 50"
                },
                Recipient = new ContactDetail()
                {
                    Company = directBookingDetail.ShipTo.CompanyName,
                    Contact = directBookingDetail.ShipTo.FirstName.Trim() + " " + directBookingDetail.ShipTo.LastName,
                    Address1 = directBookingDetail.ShipTo.Address,
                    Address2 = directBookingDetail.ShipTo.Address2,
                    Address3 = "",
                    Town = directBookingDetail.ShipTo.City,
                    Country = directBookingDetail.ShipTo.Country.Code2,
                    Postcode = directBookingDetail.ShipTo.PostCode,
                    Telephone = "+41" + " " + directBookingDetail.ShipTo.Phone,
                },
                ReferenceNumber1 = directBookingDetail.FrayteNumber + "-" + directBookingDetail.ReferenceDetail.Reference1,
                ReferenceNumber2 = directBookingDetail.ReferenceDetail.Reference2,
                Channel = 1,
                OrderType = "consignment",
                Product = "CL",
                SendingDepot = "",
                Value = "technicaltest@frayte.com"
            };
            dpdChRequest.Package = new List<DPDPackage>();
            for (int i = 0; i < directBookingDetail.Packages.Count; i++)
            {
                for (int j = 0; j < directBookingDetail.Packages[i].CartoonValue; j++)
                {
                    //Convert weight gm to 10 unit
                    DPDPackage parcel = new DPDPackage();
                    if (directBookingDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms)
                    {
                        parcel.ReferenceNumber1 = directBookingDetail.ReferenceDetail.Reference1;
                        parcel.ReferenceNumber2 = directBookingDetail.ReferenceDetail.Reference1;
                        parcel.Volume = "";
                        parcel.Weight = ((directBookingDetail.Packages[i].Weight * 1000) / 10).ToString("0.##");
                    }
                    else if (directBookingDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs)
                    {
                        parcel.ReferenceNumber1 = directBookingDetail.ReferenceDetail.Reference1;
                        parcel.ReferenceNumber2 = directBookingDetail.ReferenceDetail.Reference1;
                        parcel.Volume = "";
                        parcel.Weight = ((directBookingDetail.Packages[i].Weight * 453.592m) / 10).ToString("0.##");
                    }
                    dpdChRequest.Package.Add(parcel);
                }
            }
            return dpdChRequest;
        }

        public string CreateXMLDPDCH(DPDChRequestModel dPdChRequestModel)
        {
            string xmlPath = string.Empty;

            try
            {
                var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.DHL);

                if (AppSettings.LabelSave == "")
                {
                    xmlPath = AppSettings.WebApiPath + "/UploadFiles/PDFGenerator/HTMLFile";
                    // _log.Error("if section" + xmlPath);
                }
                else
                {
                    if (AppSettings.ShipmentCreatedFrom == "BATCH")
                    {
                        xmlPath = AppSettings.WebApiPath + "/UploadFiles/PDFGenerator/HTMLFile";
                        //_log.Error("else section BATCH" + xmlPath);
                    }
                    else
                    {
                        // _log.Error("else section BATCH");
                        xmlPath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");
                    }
                }

                if (!Directory.Exists(xmlPath))
                {
                    Directory.CreateDirectory(xmlPath);
                }

                xmlPath = xmlPath + "/DPDCHShipment.xml";
                //_log.Error(xmlPath);
                if (File.Exists(xmlPath))
                {
                    File.Delete(xmlPath);
                }

                XmlDocument xmlDoc = new XmlDocument();

                // XML Declaration
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                string soapNSURI = "http://schemas.xmlsoap.org/soap/envelope/";
                // Create the root element soapenv:Envelope 
                XmlElement rootEnvelope = xmlDoc.CreateElement("soapenv:Envelope", soapNSURI);
                rootEnvelope.SetAttribute("xmlns:soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
                rootEnvelope.SetAttribute("xmlns:ns", "http://dpd.com/common/service/types/Authentication/2.0");
                rootEnvelope.SetAttribute("xmlns:ns1", "http://dpd.com/common/service/types/ShipmentService/3.2");

                xmlDoc.AppendChild(rootEnvelope);

                //#region soapenv:Header
                XmlElement headerNode = xmlDoc.CreateElement("soapenv:Header", soapNSURI);

                // Append soapenv:Header
                XmlElement authentication = xmlDoc.CreateElement("ns:authentication", "ns");

                XmlElement usernameNode = xmlDoc.CreateElement("delisId");
                usernameNode.AppendChild(xmlDoc.CreateTextNode(dPdChRequestModel.UserName));

                XmlElement authToken = xmlDoc.CreateElement("authToken");
                authToken.AppendChild(xmlDoc.CreateTextNode(dPdChRequestModel.AuthToken));

                XmlElement messageLanguage = xmlDoc.CreateElement("messageLanguage");
                messageLanguage.AppendChild(xmlDoc.CreateTextNode("de_EN"));

                authentication.AppendChild(usernameNode);
                authentication.AppendChild(authToken);
                authentication.AppendChild(messageLanguage);

                headerNode.AppendChild(authentication);

                rootEnvelope.AppendChild(headerNode);

                XmlElement bodyNode = xmlDoc.CreateElement("soapenv:Body", soapNSURI);

                XmlElement storeOrders = xmlDoc.CreateElement("ns1:storeOrders", "ns1");

                XmlElement printOptions = xmlDoc.CreateElement("printOptions");

                XmlElement printerLanguage = xmlDoc.CreateElement("printerLanguage");

                printerLanguage.AppendChild(xmlDoc.CreateTextNode("PDF"));

                XmlElement paperFormat = xmlDoc.CreateElement("paperFormat");
                paperFormat.AppendChild(xmlDoc.CreateTextNode("A6"));

                printOptions.AppendChild(printerLanguage);
                printOptions.AppendChild(paperFormat);

                XmlElement order = xmlDoc.CreateElement("order");

                XmlElement generalShipmentData = xmlDoc.CreateElement("generalShipmentData");

                XmlElement sendingDepot = xmlDoc.CreateElement("sendingDepot");
                sendingDepot.AppendChild(xmlDoc.CreateTextNode(dPdChRequestModel.SendingDepot));

                XmlElement product = xmlDoc.CreateElement("product");
                product.AppendChild(xmlDoc.CreateTextNode("CL"));

                XmlElement sender = xmlDoc.CreateElement("sender");

                generalShipmentData.AppendChild(sendingDepot);
                generalShipmentData.AppendChild(product);

                XmlElement name1 = xmlDoc.CreateElement("name1");
                name1.AppendChild(xmlDoc.CreateTextNode((dPdChRequestModel.Shipper.Company == null || dPdChRequestModel.Shipper.Company == "") ? dPdChRequestModel.Shipper.Contact : dPdChRequestModel.Shipper.Company));

                XmlElement name2 = xmlDoc.CreateElement("name2");
                name2.AppendChild(xmlDoc.CreateTextNode(dPdChRequestModel.Shipper.Address1));

                XmlElement street = xmlDoc.CreateElement("street");
                street.AppendChild(xmlDoc.CreateTextNode(dPdChRequestModel.Shipper.Address2));

                XmlElement country = xmlDoc.CreateElement("country");
                country.AppendChild(xmlDoc.CreateTextNode(dPdChRequestModel.Shipper.Country));

                XmlElement zipCode = xmlDoc.CreateElement("zipCode");
                zipCode.AppendChild(xmlDoc.CreateTextNode(dPdChRequestModel.Shipper.Postcode));

                XmlElement city = xmlDoc.CreateElement("city");
                city.AppendChild(xmlDoc.CreateTextNode(dPdChRequestModel.Shipper.Town));

                XmlElement phone = xmlDoc.CreateElement("phone");
                phone.AppendChild(xmlDoc.CreateTextNode(dPdChRequestModel.Shipper.Telephone));

                sender.AppendChild(name1);
                sender.AppendChild(name2);
                sender.AppendChild(street);
                sender.AppendChild(country);
                sender.AppendChild(zipCode);
                sender.AppendChild(city);
                sender.AppendChild(phone);

                XmlElement recipient = xmlDoc.CreateElement("recipient");

                XmlElement recipientname = xmlDoc.CreateElement("name1");
                recipientname.AppendChild(xmlDoc.CreateTextNode((dPdChRequestModel.Recipient.Company == null || dPdChRequestModel.Recipient.Company == "") ? dPdChRequestModel.Recipient.Contact : dPdChRequestModel.Recipient.Company));

                XmlElement recipientname2 = xmlDoc.CreateElement("name2");
                recipientname2.AppendChild(xmlDoc.CreateTextNode(dPdChRequestModel.Recipient.Address1));

                XmlElement recipientstreet = xmlDoc.CreateElement("street");
                recipientstreet.AppendChild(xmlDoc.CreateTextNode((dPdChRequestModel.Recipient.Address2 == null || dPdChRequestModel.Recipient.Address2 == "") ? dPdChRequestModel.Recipient.Address1 : dPdChRequestModel.Recipient.Address2));

                XmlElement recipientcountry = xmlDoc.CreateElement("country");
                recipientcountry.AppendChild(xmlDoc.CreateTextNode(dPdChRequestModel.Recipient.Country));

                XmlElement recipientzipCode = xmlDoc.CreateElement("zipCode");
                recipientzipCode.AppendChild(xmlDoc.CreateTextNode(dPdChRequestModel.Recipient.Postcode));

                XmlElement recipientcity = xmlDoc.CreateElement("city");
                recipientcity.AppendChild(xmlDoc.CreateTextNode(dPdChRequestModel.Recipient.Town));

                XmlElement recipientphone = xmlDoc.CreateElement("phone");
                recipientphone.AppendChild(xmlDoc.CreateTextNode(dPdChRequestModel.Recipient.Telephone));
                recipient.AppendChild(recipientname);
                recipient.AppendChild(recipientname2);
                recipient.AppendChild(recipientstreet);
                recipient.AppendChild(recipientcountry);
                recipient.AppendChild(recipientzipCode);
                recipient.AppendChild(recipientcity);
                recipient.AppendChild(recipientphone);

                generalShipmentData.AppendChild(sender);
                generalShipmentData.AppendChild(recipient);

                order.AppendChild(generalShipmentData);

                for (int i = 0; i < dPdChRequestModel.Package.Count; i++)
                {

                    XmlElement parcels = xmlDoc.CreateElement("parcels");

                    XmlElement customerReferenceNumber1 = xmlDoc.CreateElement("customerReferenceNumber1");
                    customerReferenceNumber1.AppendChild(xmlDoc.CreateTextNode(dPdChRequestModel.ReferenceNumber1));

                    XmlElement weight = xmlDoc.CreateElement("weight");
                    weight.AppendChild(xmlDoc.CreateTextNode(dPdChRequestModel.Package[i].Weight));

                    parcels.AppendChild(customerReferenceNumber1);
                    parcels.AppendChild(weight);

                    order.AppendChild(parcels);
                }

                XmlElement productAndServiceData = xmlDoc.CreateElement("productAndServiceData");

                XmlElement orderType = xmlDoc.CreateElement("orderType");
                orderType.AppendChild(xmlDoc.CreateTextNode("consignment"));

                productAndServiceData.AppendChild(orderType);

                XmlElement predict = xmlDoc.CreateElement("predict");

                XmlElement channel = xmlDoc.CreateElement("channel");
                channel.AppendChild(xmlDoc.CreateTextNode("1"));

                XmlElement value = xmlDoc.CreateElement("value");
                value.AppendChild(xmlDoc.CreateTextNode("technicaltest@frayte.com"));

                XmlElement language = xmlDoc.CreateElement("language");
                language.AppendChild(xmlDoc.CreateTextNode("EN"));

                predict.AppendChild(channel);
                predict.AppendChild(value);
                predict.AppendChild(language);

                productAndServiceData.AppendChild(predict);

                order.AppendChild(productAndServiceData);

                storeOrders.AppendChild(printOptions);
                storeOrders.AppendChild(order);

                bodyNode.AppendChild(storeOrders);
                rootEnvelope.AppendChild(bodyNode);

                xmlDoc.Save(xmlPath);
                return xmlPath;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string CreateXMLDPDCHLogin()
        {
            string xmlPath = string.Empty;

            try
            {
                var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.DPDCH);
                if (AppSettings.LabelSave == "")
                {
                    xmlPath = AppSettings.WebApiPath + "/UploadFiles/PDFGenerator/HTMLFile";
                    // _log.Error("if section" + xmlPath);
                }
                else
                {
                    if (AppSettings.ShipmentCreatedFrom == "BATCH")
                    {
                        xmlPath = AppSettings.WebApiPath + "/UploadFiles/PDFGenerator/HTMLFile";
                        //_log.Error("else section BATCH" + xmlPath);
                    }
                    else
                    {
                        // _log.Error("else section BATCH");
                        xmlPath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");
                    }
                }

                if (!Directory.Exists(xmlPath))
                {
                    Directory.CreateDirectory(xmlPath);
                }

                xmlPath = xmlPath + "/DPDCHLogin.xml";
                //_log.Error(xmlPath);
                if (File.Exists(xmlPath))
                {
                    File.Delete(xmlPath);
                }
                XmlDocument xmlDoc = new XmlDocument();

                // XML Declaration
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                string soapNSURI = "http://schemas.xmlsoap.org/soap/envelope/";
                // Create the root element soapenv:Envelope 
                XmlElement rootEnvelope = xmlDoc.CreateElement("soapenv:Envelope", soapNSURI);
                rootEnvelope.SetAttribute("xmlns:soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
                rootEnvelope.SetAttribute("xmlns:ns", "http://dpd.com/common/service/types/LoginService/2.0");

                xmlDoc.AppendChild(rootEnvelope);

                // Create soapenv:Header 
                XmlElement headerNode = xmlDoc.CreateElement("soapenv:Header", soapNSURI);

                // Append soapenv:Header 
                rootEnvelope.AppendChild(headerNode);
                XmlElement bodyNode = xmlDoc.CreateElement("soapenv:Body", soapNSURI);

                XmlElement getAuth = xmlDoc.CreateElement("ns:getAuth", "ns");

                XmlElement usernameNode = xmlDoc.CreateElement("delisId");
                usernameNode.AppendChild(xmlDoc.CreateTextNode(logisticIntegration.UserName.ToString()));

                XmlElement password = xmlDoc.CreateElement("password");
                password.AppendChild(xmlDoc.CreateTextNode(logisticIntegration.Password.ToString()));

                XmlElement messageLanguage = xmlDoc.CreateElement("messageLanguage");
                messageLanguage.AppendChild(xmlDoc.CreateTextNode("de_EN"));

                getAuth.AppendChild(usernameNode);
                getAuth.AppendChild(password);
                getAuth.AppendChild(messageLanguage);
                bodyNode.AppendChild(getAuth);

                rootEnvelope.AppendChild(bodyNode);
                xmlDoc.Save(xmlPath);
                return xmlPath;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static string CallWebservice(string body, string serverUrl)
        {
            try
            {
                WebRequest requestRate = HttpWebRequest.Create(serverUrl);
                requestRate.ContentType = "application/x-www-form-urlencoded";
                requestRate.Method = "POST";

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
            catch (System.Exception ex)
            {
                throw;
            }
        }

        public IntegrtaionResult MapDPDCHIntegrationResponse(DPDChResponseModel dpdchShipmentResponse)
        {
            IntegrtaionResult integrtaionResult = new IntegrtaionResult();

            if (dpdchShipmentResponse.Error == null)
            {
                integrtaionResult.Status = true;
                integrtaionResult.CourierName = FrayteCourierCompany.DPDCH;
                integrtaionResult.TrackingNumber = dpdchShipmentResponse.ShipmentResponses.ParcelLabelNumber[0].ToString();
                integrtaionResult.PickupRef = null;
                integrtaionResult.PieceTrackingDetails = new List<CourierPieceDetail>();
                foreach (var data in dpdchShipmentResponse.ShipmentResponses.ParcelLabelNumber)
                {
                    CourierPieceDetail obj = new CourierPieceDetail();
                    obj.DirectShipmentDetailId = 0;
                    obj.PieceTrackingNumber = data;
                    obj.ImageByte = dpdchShipmentResponse.ParcelLabelPDF;
                    integrtaionResult.PieceTrackingDetails.Add(obj);
                }
            }
            else
            {
                integrtaionResult.Error = dpdchShipmentResponse.Error;
                integrtaionResult.ErrorCode = new List<FrayteApiError>();
                {
                    List<FrayteApiError> _api = new FrayteApiErrorCodeRepository().SaveFinilizeApiError(integrtaionResult.Error, FrayteLogisticServiceType.DPDCH, null);
                    integrtaionResult.ErrorCode = _api;
                }
                integrtaionResult.Status = false;
            }

            return integrtaionResult;
        }

        public void MappingCourierPieceDetail(IntegrtaionResult integrtaionResult, DirectBookingShipmentDraftDetail directBookingDetail, int DirectShipmentid)
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

        public bool SaveTrackingDetail(DirectBookingShipmentDraftDetail directBookingDetail, IntegrtaionResult result, int DirectShipmentid)
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

        public string DownloadDPDCHBytetoImage(CourierPieceDetail pieceDetails, int totalPiece, int count, int DirectShipmentid)
        {
            string Image = string.Empty;
            if (Image != null)
            {
                string labelName = string.Empty;
                labelName = FrayteCourierCompany.DPDCH;

                // Create a file to write to.
                Image = labelName + "_" + pieceDetails.PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".pdf";

                if (AppSettings.LabelSave == "")
                {
                    if (System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/"))
                    {
                        string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image);
                        try
                        {
                            byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);
                            File.WriteAllBytes(path, sPDFDecoded);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/");
                        string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image);
                        try
                        {
                            byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);

                            File.WriteAllBytes(path, sPDFDecoded);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            Console.ReadLine();
                        }
                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(AppSettings.LabelFolder + "/" + DirectShipmentid + "/"))
                    {
                        string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image);
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);

                            File.WriteAllBytes(path, sPDFDecoded);
                        }
                        else
                        {
                            try
                            {
                                byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);
                                File.WriteAllBytes(path, sPDFDecoded);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                                Console.ReadLine();
                            }
                        }
                    }
                    else
                    {
                        string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image);
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "/" + DirectShipmentid);
                            try
                            {
                                byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);
                                File.WriteAllBytes(path, sPDFDecoded);
                            }
                            catch (Exception e)
                            {

                            }
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid));

                            try
                            {
                                byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);
                                File.WriteAllBytes(path, sPDFDecoded);
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }
                }
            }
            return Image;
        }

        #region Express

        public DPDChRequestModel MapExpressBookingDetailToShipmentRequestDto(ExpressShipmentModel shipment)
        {

            var ShipFrom = new ExpressRepository().getHubAddress(shipment.ShipTo.Country.CountryId, shipment.ShipTo.PostCode, shipment.ShipTo.State);
            DPDChRequestModel dpdChRequest = new DPDChRequestModel()
            {
                UserName = "",
                Password = "",
                AuthToken = "",
                Shipper = new ContactDetail()
                {
                    Company = ShipFrom.CompanyName,
                    Contact = string.IsNullOrWhiteSpace(ShipFrom.FirstName + " " + ShipFrom.LastName) ? ShipFrom.CompanyName : ShipFrom.FirstName + " " + ShipFrom.LastName,
                    Address1 = ShipFrom.Address,
                    Address2 = ShipFrom.Address2,
                    Address3 = "",
                    Town = ShipFrom.City,
                    Country = ShipFrom.Country.Code2,
                    Postcode = ShipFrom.PostCode,
                    Telephone = ShipFrom.Phone

                    //Company = "Total Freight Management GmbH",
                    //Contact = "",
                    //Address1 = "Fracht West",
                    //Address2 = "Entrance 1, 2nd Fl., Office 2-327",
                    //Address3 = "",
                    //Town = "Zurich Airport",
                    //Country = "CH",
                    //Postcode = "8058",
                    //Telephone = "+41 44 816 40 50"
                },
                Recipient = new ContactDetail()
                {
                    Company = shipment.ShipTo.CompanyName,
                    Contact = shipment.ShipTo.FirstName.Trim() + " " + shipment.ShipTo.LastName,
                    Address1 = shipment.ShipTo.Address,
                    Address2 = shipment.ShipTo.Address2,
                    Address3 = "",
                    Town = shipment.ShipTo.City,
                    Country = shipment.ShipTo.Country.Code2,
                    Postcode = shipment.ShipTo.PostCode,
                    Telephone = "+41" + " " + shipment.ShipTo.Phone,
                },
                ReferenceNumber1 = shipment.FrayteNumber + "-" + shipment.ShipmentReference,
                ReferenceNumber2 = "",
                Channel = 1,
                OrderType = "consignment",
                Product = "CL",
                SendingDepot = "",
                Value = "technical1@frayte.com"
            };
            dpdChRequest.Package = new List<DPDPackage>();
            for (int i = 0; i < shipment.Packages.Count; i++)
            {
                for (int j = 0; j < shipment.Packages[i].CartonValue; j++)
                {
                    //Convert weight gm to 10 unit
                    DPDPackage parcel = new DPDPackage();
                    if (shipment.PakageCalculatonType == FraytePakageCalculationType.kgtoCms)
                    {
                        parcel.ReferenceNumber1 = shipment.ShipmentReference;
                        parcel.ReferenceNumber2 = "";
                        parcel.Volume = "";
                        parcel.Weight = ((shipment.Packages[i].Weight * 1000) / 10).ToString("0.##");
                    }
                    else if (shipment.PakageCalculatonType == FraytePakageCalculationType.LbToInchs)
                    {
                        parcel.ReferenceNumber1 = shipment.ShipmentReference;
                        parcel.ReferenceNumber2 = "";
                        parcel.Volume = "";
                        parcel.Weight = ((shipment.Packages[i].Weight * 453.592m) / 10).ToString("0.##");
                    }
                    dpdChRequest.Package.Add(parcel);
                }
            }
            return dpdChRequest;
        }

        public string DownloadExpressDPDCHBytetoImage(CourierPieceDetail pieceDetails, int totalPiece, int count, int shipmentId)
        {
            string Image = string.Empty;
            if (Image != null)
            {
                string labelName = string.Empty;
                labelName = FrayteCourierCompany.DPDCH;

                // Create a file to write to.
                Image = labelName + "_" + pieceDetails.PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".pdf";

                if (AppSettings.LabelSave == "")
                {
                    if (System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/Express/" + shipmentId + "/"))
                    {
                        string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + shipmentId + "/" + Image);
                        try
                        {
                            byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);
                            File.WriteAllBytes(path, sPDFDecoded);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/Express/" + shipmentId + "/");
                        string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + shipmentId + "/" + Image);
                        try
                        {
                            byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);

                            File.WriteAllBytes(path, sPDFDecoded);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            Console.ReadLine();
                        }
                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(AppSettings.LabelFolder + "/Express/" + shipmentId + "/"))
                    {
                        string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + shipmentId + "/" + Image);
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);

                            File.WriteAllBytes(path, sPDFDecoded);
                        }
                        else
                        {
                            try
                            {
                                byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);
                                File.WriteAllBytes(path, sPDFDecoded);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                                Console.ReadLine();
                            }
                        }
                    }
                    else
                    {
                        string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + shipmentId + "/" + Image);
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "/Express/" + shipmentId);
                            try
                            {
                                byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);
                                File.WriteAllBytes(path, sPDFDecoded);
                            }
                            catch (Exception e)
                            {

                            }
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + shipmentId));

                            try
                            {
                                byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);
                                File.WriteAllBytes(path, sPDFDecoded);
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }
                }
            }
            return Image;
        }

        #endregion
    }
}