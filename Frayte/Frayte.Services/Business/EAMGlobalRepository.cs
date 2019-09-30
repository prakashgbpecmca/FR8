using Frayte.Services.Models;
using Frayte.Services.Models.EAM;
using Frayte.Services.Models.Express;
using Frayte.Services.Models.SKYPOSTAL;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Linq;

namespace Frayte.Services.Business
{
    public class EAMGlobalRepository
    {
        public EAMRequestModel MapDirectBookingDetailToShipmentRequestDto(DirectBookingShipmentDraftDetail directBookingDetail)
        {
            EAMRequestModel eamRequest = new EAMRequestModel()
            {
                ShipperAddress1 = directBookingDetail.ShipFrom.Address,
                ShipperAddress2 = directBookingDetail.ShipFrom.Address2,
                ShipperAddress3 = "",
                ShipperContact = string.Concat(directBookingDetail.ShipFrom.FirstName + " ", directBookingDetail.ShipFrom.LastName),
                ShipperCompany = string.IsNullOrWhiteSpace(directBookingDetail.ShipFrom.CompanyName) ? string.Concat(directBookingDetail.ShipFrom.FirstName + " ", directBookingDetail.ShipFrom.LastName) : directBookingDetail.ShipFrom.CompanyName,
                ShipperCountry = directBookingDetail.ShipFrom.Country.Code2,
                ShipperPostcode = string.IsNullOrEmpty(directBookingDetail.ShipFrom.PostCode) ? "" : directBookingDetail.ShipFrom.PostCode,
                ShipperTelephone = directBookingDetail.ShipFrom.Phone,
                ShipperTown = string.IsNullOrEmpty(directBookingDetail.ShipFrom.City) ? "" : directBookingDetail.ShipFrom.City,
                Account = "",
                Password = "",
                CustomerTransactionId = directBookingDetail.FrayteNumber,
                Hawb = directBookingDetail.FrayteNumber,
                Service = directBookingDetail.CustomerRateCard.NetworkCode,
                Mawb = "",
                Date = directBookingDetail.ReferenceDetail.CollectionDate.Value.ToString("dd/MM/yyyy"),
                Company = string.IsNullOrWhiteSpace(directBookingDetail.ShipTo.CompanyName) ? string.Concat(directBookingDetail.ShipTo.FirstName + " ", directBookingDetail.ShipTo.LastName) : directBookingDetail.ShipTo.CompanyName,
                Contact = string.Concat(directBookingDetail.ShipTo.FirstName + " ", directBookingDetail.ShipTo.LastName),
                Address1 = directBookingDetail.ShipTo.Address,
                Address2 = directBookingDetail.ShipTo.Address2,
                Address3 = "",
                Town = string.IsNullOrEmpty(directBookingDetail.ShipTo.City) ? "" : directBookingDetail.ShipTo.City,
                Country = directBookingDetail.ShipTo.Country.Code2,
                Postcode = string.IsNullOrEmpty(directBookingDetail.ShipTo.PostCode) ? "" : directBookingDetail.ShipTo.PostCode,
                telephone = directBookingDetail.ShipTo.Phone,
                noOfPieces = directBookingDetail.Packages.Sum(t => t.CartoonValue).ToString(),
                Weight = directBookingDetail.CustomerRateCard.Weight,
                DoxNonDox = directBookingDetail.ParcelType.ParcelType == "Parcel" ? "NDX" : "DOX",
                Description = string.Join("-", directBookingDetail.Packages.Select(x => x.Content.ToString()).ToArray()) + "-" + directBookingDetail.ReferenceDetail.Reference1,
                Value = directBookingDetail.Packages.Sum(t => t.Value).ToString(),
                Currency = directBookingDetail.Currency.CurrencyCode,
                Agent = "",
                Notes = "JUST A TEST"
            };
            if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.EAMTNT)
            {
                eamRequest.Agent = "TNT economy service";
            }
            else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.EAMDHL)
            {
                eamRequest.Agent = "DHL express service";
            }
            else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.EAMFedEx)
            {
                eamRequest.Agent = "Fedex Economy service";
            }
            return eamRequest;
        }

        public EAMRequestModel MapExpressBookingDetailToShipmentRequestDto(ExpressShipmentModel expressBookingDetail)
        {
            EAMRequestModel eamRequest = new EAMRequestModel()
            {
                ShipperAddress1 = expressBookingDetail.ShipFrom.Address,
                ShipperAddress2 = expressBookingDetail.ShipFrom.Address2,
                ShipperAddress3 = "",
                ShipperContact = string.Concat(expressBookingDetail.ShipFrom.FirstName + " ", expressBookingDetail.ShipFrom.LastName),
                ShipperCompany = string.IsNullOrWhiteSpace(expressBookingDetail.ShipFrom.CompanyName) ? string.Concat(expressBookingDetail.ShipFrom.FirstName + " ", expressBookingDetail.ShipFrom.LastName) : expressBookingDetail.ShipFrom.CompanyName,
                ShipperCountry = expressBookingDetail.ShipFrom.Country.Code2,
                ShipperPostcode = string.IsNullOrEmpty(expressBookingDetail.ShipFrom.PostCode) ? "" : expressBookingDetail.ShipFrom.PostCode,
                ShipperTelephone = expressBookingDetail.ShipFrom.Phone,
                ShipperTown = string.IsNullOrEmpty(expressBookingDetail.ShipFrom.City) ? "" : expressBookingDetail.ShipFrom.City,
                Account = "",
                Password = "",
                CustomerTransactionId = expressBookingDetail.AWBNumber.Replace(" ", ""),
                Hawb = expressBookingDetail.AWBNumber.Replace(" ", ""),
                Service = expressBookingDetail.Service.NetworkCode,
                Mawb = "",
                Date = DateTime.UtcNow.ToString("dd/MM/yyyy"),
                Company = string.IsNullOrWhiteSpace(expressBookingDetail.ShipTo.CompanyName) ? string.Concat(expressBookingDetail.ShipTo.FirstName + " ", expressBookingDetail.ShipTo.LastName) : expressBookingDetail.ShipTo.CompanyName,
                Contact = string.Concat(expressBookingDetail.ShipTo.FirstName + " ", expressBookingDetail.ShipTo.LastName),
                Address1 = expressBookingDetail.ShipTo.Address,
                Address2 = expressBookingDetail.ShipTo.Address2,
                Address3 = "",
                Town = string.IsNullOrEmpty(expressBookingDetail.ShipTo.City) ? "" : expressBookingDetail.ShipTo.City,
                Country = expressBookingDetail.ShipTo.Country.Code2,
                Postcode = string.IsNullOrEmpty(expressBookingDetail.ShipTo.PostCode) ? "" : expressBookingDetail.ShipTo.PostCode,
                telephone = expressBookingDetail.ShipTo.Phone,
                noOfPieces = expressBookingDetail.Packages.Sum(t => t.CartonValue).ToString(),
                Weight = expressBookingDetail.Service.ActualWeight,
                DoxNonDox = expressBookingDetail.ParcelType.ParcelType == "Parcel" ? "NDX" : "DOX",
                Description = string.Join("-", expressBookingDetail.Packages.Select(x => x.Content.ToString()).ToArray()) + "-" + expressBookingDetail.ShipmentReference,
                Value = expressBookingDetail.Packages.Sum(t => t.Value).ToString(),
                Currency = expressBookingDetail.DeclaredCurrency.CurrencyCode,
                Agent = "",
                Notes = "JUST A TEST"
            };
            if (expressBookingDetail.Service.RateType == FrayteCourierCompany.EAMTNT)
            {
                eamRequest.Agent = "TNT economy service";
            }
            else if (expressBookingDetail.Service.RateType == FrayteCourierCompany.EAMDHL)
            {
                eamRequest.Agent = "DHL express service";
            }
            else if (expressBookingDetail.Service.RateType == FrayteCourierCompany.EAMFedEx)
            {
                eamRequest.Agent = "Fedex Economy service";
            }
            return eamRequest;
        }

        public string CreateXMLForEAM(EAMRequestModel eamRequest)
        {
            string xmlPath = string.Empty;

            try
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();

                xmlWriterSettings.Encoding = new UTF8Encoding(true);

                xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;

                xmlWriterSettings.Indent = true;
                var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.EAM);

                eamRequest.Account = logisticIntegration.UserName;
                eamRequest.Password = logisticIntegration.Password;

                //xmlPath = It will be the path where xml will saved
                if (AppSettings.LabelSave == "")
                {
                    xmlPath = AppSettings.WebApiPath + "/UploadFiles/PDFGenerator/HTMLFile";
                }
                else
                {
                    if (AppSettings.ShipmentCreatedFrom == "BATCH")
                    {
                        xmlPath = AppSettings.WebApiPath + "/UploadFiles/PDFGenerator/HTMLFile";
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

                xmlPath = xmlPath + "/EAMShipment.xml";
                if (File.Exists(xmlPath))
                {
                    File.Delete(xmlPath);
                }

                using (var xmlWriter = XmlWriter.Create(xmlPath))
                {
                    xmlWriter.WriteStartDocument();
                    if (logisticIntegration != null)
                    {
                        xmlWriter.WriteStartElement("ProcessShipmentRequest");

                        xmlWriter.WriteStartElement("WebAuthenticationDetail");

                        xmlWriter.WriteStartElement("UserCredential");

                        xmlWriter.WriteStartElement("Account");
                        xmlWriter.WriteString(eamRequest.Account);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Password");
                        xmlWriter.WriteString(eamRequest.Password);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("TransactionDetail");

                        xmlWriter.WriteStartElement("CustomerTransactionId");
                        xmlWriter.WriteString(eamRequest.CustomerTransactionId);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("RequestedShipment");

                        xmlWriter.WriteStartElement("ShipperCompany");
                        xmlWriter.WriteString(eamRequest.ShipperCompany);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("ShipperContact");
                        xmlWriter.WriteString(eamRequest.ShipperContact);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("ShipperAddress1");
                        xmlWriter.WriteString(eamRequest.ShipperAddress1);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("ShipperAddress2");
                        xmlWriter.WriteString(string.IsNullOrEmpty(eamRequest.ShipperAddress2) ? "." : eamRequest.ShipperAddress2);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("ShipperAddress3");
                        xmlWriter.WriteString(eamRequest.ShipperAddress3);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("ShipperTown");
                        xmlWriter.WriteString(eamRequest.ShipperTown);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("ShipperCountry");
                        xmlWriter.WriteString(eamRequest.ShipperCountry);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("ShipperPostcode");
                        xmlWriter.WriteString(string.IsNullOrEmpty(eamRequest.ShipperPostcode) ? "" : eamRequest.ShipperPostcode.ToString());
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("ShipperTelephone");
                        xmlWriter.WriteString(eamRequest.ShipperTelephone);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Hawb");
                        xmlWriter.WriteString(eamRequest.Hawb);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Service");
                        xmlWriter.WriteString(eamRequest.Service);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Mawb");
                        xmlWriter.WriteString(eamRequest.Mawb);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Date");
                        xmlWriter.WriteString(eamRequest.Date.ToString());
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Company");
                        xmlWriter.WriteString(eamRequest.Company);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Contact");
                        xmlWriter.WriteString(eamRequest.Contact);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Address1");
                        xmlWriter.WriteString(eamRequest.Address1);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Address2");
                        xmlWriter.WriteString(string.IsNullOrEmpty(eamRequest.Address2) ? "'.'" : eamRequest.Address2);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Address3");
                        xmlWriter.WriteString(eamRequest.Address3);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Town");
                        xmlWriter.WriteString(eamRequest.Town);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Country");
                        xmlWriter.WriteString(eamRequest.Country);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Postcode");
                        xmlWriter.WriteString(string.IsNullOrEmpty(eamRequest.Postcode) ? "" : eamRequest.Postcode);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("telephone");
                        xmlWriter.WriteString(eamRequest.telephone.ToString());
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("noOfPieces");
                        xmlWriter.WriteString(eamRequest.noOfPieces);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Weight");
                        xmlWriter.WriteString(eamRequest.Weight.ToString());
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("DoxNonDox");
                        xmlWriter.WriteString(eamRequest.DoxNonDox.ToString());
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Description");
                        xmlWriter.WriteString(eamRequest.Description);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Value");
                        xmlWriter.WriteString(eamRequest.Value);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Weight");
                        xmlWriter.WriteString(eamRequest.Weight.ToString());
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Currency");
                        xmlWriter.WriteString(eamRequest.Currency);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Agent");
                        xmlWriter.WriteString(eamRequest.Agent);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Notes");
                        xmlWriter.WriteString(eamRequest.Notes);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();
                    }
                    return xmlPath;
                }
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

        public EAMGlobalResponse CreateShipment(string shipmentXml, int DraftShipmentId)
        {
            EAMGlobalResponse response = new EAMGlobalResponse();
            var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.EAM);
            if (logisticIntegration != null)
            {
                string serverUrl = logisticIntegration.ServiceUrl;
                string trackingCode = string.Empty;
                var shipmentPackageTrackingDetail = new List<FraytePackageTrackingDetail>();
                //Send XML
                var shipmentResult = CallWebservice(shipmentXml, serverUrl); //utf-8 only
                try
                {
                    var xmltag = "<?xml version=" + "'1.0'" + " encoding=" + "'UTF-8'" + " ?>";
                    shipmentResult = xmltag + @shipmentResult;

                    var result = XDocument.Parse(shipmentResult);

                    if (!string.IsNullOrWhiteSpace(shipmentResult) && !shipmentResult.Contains("ERROR"))
                    {
                        var label = (from r in result.Descendants("LABEL")
                                     select new
                                     {
                                         Lable = r.Element("LINK") != null ? r.Element("LINK").Value : "",
                                         AWB = r.Element("AWB") != null ? r.Element("AWB").Value : ""
                                     }).FirstOrDefault();
                        response.LabelUrl = label.Lable;
                        response.Status = true;
                        response.AWB = label.AWB;
                    }
                    else
                    {
                        var pickupxml = XDocument.Parse(shipmentResult);

                        var ElementsList = pickupxml.Descendants("MESSAGE")
                                                        .Select(x => new { ElementContent = x.Value })
                                                        .ToList();
                        response.ERROR = new List<string>();
                        foreach (var message in ElementsList)
                        {
                            response.ERROR.Add(message.ElementContent);
                        }

                        new DirectShipmentRepository().SaveEasyPosyPickUpObject(@shipmentResult, shipmentXml, DraftShipmentId);
                    }
                }
                catch (Exception ex)
                {

                    var pickupxml = XDocument.Parse(shipmentResult);
                    var Error = (from r in pickupxml.Descendants("Condition")
                                 select new
                                 {
                                     ErrorCode = r.Element("ConditionCode") != null ? r.Element("ConditionCode").Value : "",
                                     ErrorDescription = r.Element("ConditionData") != null ? r.Element("ConditionData").Value : "",
                                 }).ToList();


                    new DirectShipmentRepository().SaveEasyPosyPickUpObject(@shipmentResult, shipmentXml, DraftShipmentId);
                }
            }
            return response;
        }

        public IntegrtaionResult MapEAMGlobalIntegrationResponse(EAMGlobalResponse eamPostalResponse, List<PackageDraft> Packages)
        {
            IntegrtaionResult integrtaionResult = new IntegrtaionResult();

            if (eamPostalResponse.Status)
            {
                integrtaionResult.Status = true;
                integrtaionResult.CourierName = FrayteCourierCompany.EAM;
                integrtaionResult.TrackingNumber = eamPostalResponse.AWB;
                integrtaionResult.PickupRef = null;
                integrtaionResult.PieceTrackingDetails = new List<CourierPieceDetail>();
                int counter = 0;
                for (int i = 0; i < Packages.Count(); i++)
                {
                    for (int j = 0; j < Packages[i].CartoonValue; j++)
                    {
                        CourierPieceDetail obj = new CourierPieceDetail();
                        obj.DirectShipmentDetailId = 0;
                        obj.PieceTrackingNumber = string.Concat(eamPostalResponse.AWB, counter);
                        obj.ImageUrl = eamPostalResponse.LabelUrl;
                        integrtaionResult.PieceTrackingDetails.Add(obj);
                        counter++;
                    }
                }
            }
            else
            {
                integrtaionResult.Error = new FratyteError();
                integrtaionResult.Error.Service = new List<string>();
                integrtaionResult.Error.Service.AddRange(eamPostalResponse.ERROR);
                integrtaionResult.Status = false;
            }
            return integrtaionResult;
        }

        public IntegrtaionResult MapExpressEAMGlobalIntegrationResponse(EAMGlobalResponse eamPostalResponse, List<ExpressPackageModel> Packages, HubService carrier)
        {
            IntegrtaionResult integrtaionResult = new IntegrtaionResult();

            if (eamPostalResponse.Status)
            {
                integrtaionResult.Status = true;
                if (carrier.HubCarrier == FrayteCourierCompany.DomesticA)
                {
                    integrtaionResult.CourierName = FrayteCourierCompany.DomesticA;
                }
                else if (carrier.HubCarrier == FrayteCourierCompany.EAMExpress)
                {
                    integrtaionResult.CourierName = FrayteCourierCompany.EAMExpress;
                }
                integrtaionResult.TrackingNumber = eamPostalResponse.AWB;
                integrtaionResult.PickupRef = null;
                integrtaionResult.PieceTrackingDetails = new List<CourierPieceDetail>();
                int counter = 0;
                for (int i = 0; i < Packages.Count(); i++)
                {
                    for (int j = 0; j < Packages[i].CartonValue; j++)
                    {
                        CourierPieceDetail obj = new CourierPieceDetail();
                        obj.DirectShipmentDetailId = 0;
                        obj.PieceTrackingNumber = string.Concat(eamPostalResponse.AWB, counter);
                        obj.ImageUrl = eamPostalResponse.LabelUrl;
                        integrtaionResult.PieceTrackingDetails.Add(obj);
                        counter++;
                    }
                }
            }
            else
            {
                integrtaionResult.Error = new FratyteError();
                integrtaionResult.Error.Service = new List<string>();
                integrtaionResult.Error.Service.AddRange(eamPostalResponse.ERROR);
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

        public string DownloadEAMImageTOPDF(CourierPieceDetail pieceDetails, int totalPiece, int count, int DirectShipmentid, DirectBookingService Carrier)
        {
            string Image = string.Empty;
            if (pieceDetails.ImageUrl != null)
            {
                string labelName = string.Empty;
                labelName = Carrier.DisplayName;

                // Create a file to write to.
                if (pieceDetails.ImageUrl.Contains(".html"))
                {
                    Image = labelName + "_" + pieceDetails.PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".html";
                }
                else
                {
                    Image = labelName + "_" + pieceDetails.PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".pdf";
                }

                if (AppSettings.LabelSave == "")
                {
                    if (System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/"))
                    {

                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/");
                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(AppSettings.LabelFolder + "/" + DirectShipmentid + "/"))
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            File.WriteAllText(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image, pieceDetails.ImageByte);
                        }
                        else
                        {
                            string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image);
                            File.WriteAllText(path, pieceDetails.ImageByte);
                        }
                    }
                    else
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "/" + DirectShipmentid);
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid));
                            string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image);
                            try
                            {
                                bool status = false;
                                WebClient client = new WebClient();
                                client.UseDefaultCredentials = true;
                                client.Credentials = CredentialCache.DefaultCredentials;
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                                SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                                SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                                ServicePointManager.SecurityProtocol = Tls12;
                                client.DownloadFile(pieceDetails.ImageUrl, path);
                                status = File.Exists(path);

                                if (status)
                                {
                                    return Image;
                                }
                                else
                                {
                                    Image = string.Empty;
                                }
                            }
                            catch (Exception ex)
                            {
                                Image = string.Empty;
                            }
                        }
                    }
                }
            }
            return Image;
        }

        public string DownloadExpressEAMImageTOPDF(CourierPieceDetail pieceDetails, int totalPiece, int count, int ExpressShipmentid, HubService Carrier)
        {
            string Image = string.Empty;
            if (pieceDetails.ImageUrl != null)
            {
                string labelName = string.Empty;
                labelName = Carrier.HubCarrier;

                // Create a file to write to.
                if (pieceDetails.ImageUrl.Contains(".html"))
                {
                    Image = labelName + "_" + pieceDetails.PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".html";
                }
                else
                {
                    Image = labelName + "_" + pieceDetails.PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".pdf";
                }

                if (AppSettings.LabelSave == "")
                {
                    if (System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressShipmentid + "/"))
                    {
                        var path1 = AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressShipmentid + "/" + Image;
                        try
                        {
                            bool status = false;
                            WebClient client = new WebClient();
                            client.UseDefaultCredentials = true;
                            client.Credentials = CredentialCache.DefaultCredentials;
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                            SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                            SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                            ServicePointManager.SecurityProtocol = Tls12;
                            client.DownloadFile(pieceDetails.ImageUrl, path1);
                            status = File.Exists(path1);

                            if (status)
                            {
                                return Image;
                            }
                            else
                            {
                                Image = string.Empty;
                            }
                        }
                        catch (Exception ex)
                        {
                            Image = string.Empty;
                        }
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressShipmentid + "/");
                        var path1 = AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressShipmentid + "/" + Image;
                        try
                        {
                            bool status = false;
                            WebClient client = new WebClient();
                            client.UseDefaultCredentials = true;
                            client.Credentials = CredentialCache.DefaultCredentials;
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                            SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                            SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                            ServicePointManager.SecurityProtocol = Tls12;
                            client.DownloadFile(pieceDetails.ImageUrl, path1);
                            status = File.Exists(path1);

                            if (status)
                            {
                                return Image;
                            }
                            else
                            {
                                Image = string.Empty;
                            }
                        }
                        catch (Exception ex)
                        {
                            Image = string.Empty;
                        }
                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(AppSettings.LabelFolder + "/Express/" + ExpressShipmentid + "/"))
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            File.WriteAllText(AppSettings.LabelFolder + "/Express/" + ExpressShipmentid + "/" + Image, pieceDetails.ImageByte);
                            var path1 = AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressShipmentid + "/" + Image;
                            try
                            {
                                bool status = false;
                                WebClient client = new WebClient();
                                client.UseDefaultCredentials = true;
                                client.Credentials = CredentialCache.DefaultCredentials;
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                                SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                                SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                                ServicePointManager.SecurityProtocol = Tls12;
                                client.DownloadFile(pieceDetails.ImageUrl, path1);
                                status = File.Exists(path1);

                                if (status)
                                {
                                    return Image;
                                }
                                else
                                {
                                    Image = string.Empty;
                                }
                            }
                            catch (Exception ex)
                            {
                                Image = string.Empty;
                            }
                        }
                        else
                        {
                            string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + ExpressShipmentid + "/" + Image);
                            File.WriteAllText(path, pieceDetails.ImageByte);
                        }
                    }
                    else
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "/Express/" + ExpressShipmentid);
                            var path1 = AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressShipmentid + "/" + Image;
                            try
                            {
                                bool status = false;
                                WebClient client = new WebClient();
                                client.UseDefaultCredentials = true;
                                client.Credentials = CredentialCache.DefaultCredentials;
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                                SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                                SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                                ServicePointManager.SecurityProtocol = Tls12;
                                client.DownloadFile(pieceDetails.ImageUrl, path1);
                                status = File.Exists(path1);

                                if (status)
                                {
                                    return Image;
                                }
                                else
                                {
                                    Image = string.Empty;
                                }
                            }
                            catch (Exception ex)
                            {
                                Image = string.Empty;
                            }
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + ExpressShipmentid));
                            string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + ExpressShipmentid + "/" + Image);
                            try
                            {
                                bool status = false;
                                WebClient client = new WebClient();
                                client.UseDefaultCredentials = true;
                                client.Credentials = CredentialCache.DefaultCredentials;
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                                SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                                SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                                ServicePointManager.SecurityProtocol = Tls12;
                                client.DownloadFile(pieceDetails.ImageUrl, path);
                                status = File.Exists(path);

                                if (status)
                                {
                                    return Image;
                                }
                                else
                                {
                                    Image = string.Empty;
                                }
                            }
                            catch (Exception ex)
                            {
                                Image = string.Empty;
                            }
                        }
                    }
                }
            }
            return Image;
        }
    }
}
