using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Models.ApiModal;
using Frayte.Services.Models.TNT;
using Frayte.Services.Utility;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace Frayte.Services.Business
{
    public class TNTRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        private static string uniqueCONREF = "";
        string accessCode;
        string resultReply;
        string connoteReply;

        #region  TNT Inegration

        public TNTResponseDto CreateShipment(string shipmentXML, int DraftShipmentId)
        {
            FratyteError error = new FratyteError();
            TNTResponseDto TNTResponse = new TNTResponseDto();
            error.MiscErrors = new List<FrayteKeyValue>();
            var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.TNT);
            FrayteKeyValue er;
            if (logisticIntegration != null)
            {
                string trackingCode = string.Empty;
                string bookRefCode = string.Empty;
                var shipmentPackageTrackingDetail = new List<FraytePackageTrackingDetail>();

                TNTResponse.TNTShipmentResponse = new TNTShipmentResponseDto()
                {
                    XmlIn = shipmentXML
                };

                var accessCodeReply = CallWebservice(shipmentXML);
                if (accessCodeReply.StartsWith("COMPLETE:"))
                {
                    accessCode = accessCodeReply.Substring(9);
                }
                else
                {
                    error.Status = false;
                    er = new FrayteKeyValue();
                    er.Key = "Misscllaneous Errors";
                    er.Value = new List<string>() { "Something bad happen please try again later." };
                    error.MiscErrors.Add(er);
                }
                resultReply = CallWebservice(string.Format("GET_RESULT:{0}", accessCode));
                TNTResponse.TNTShipmentResponse.ResultReply = resultReply;
                trackingCode = ReadXMLDocument(resultReply);
                bookRefCode = BookXMLDocument(resultReply);
                if (!string.IsNullOrEmpty(trackingCode))
                {
                    //call get label
                    // connoteReply = CallWebservice(string.Format("GET_LABEL:{0}", accessCode));
                }
                else
                {
                    // _log.Error("enter in else section");
                    FrayteKeyValue er1;

                    // Read error messages from xml  
                    XDocument xml = XDocument.Parse(@resultReply);

                    var list1 = (from r in xml.Descendants("parse_error")
                                 select new
                                 {
                                     LineNumber = r.Element("error_line") != null ? r.Element("error_line").Value : "",
                                     Reason = r.Element("error_reason") != null ? r.Element("error_reason").Value : "",
                                     Source = r.Element("error_srcText") != null ? r.Element("error_srcText").Value : ""
                                 }).ToList();

                    // Log to elmah
                    if (list1.Count > 0)
                    {
                        //_log.Error(list1[0].Source);
                        dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(list1);
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));

                        er1 = new FrayteKeyValue();
                        er1.Value = new List<string>();
                        er1.Key = "Miscellaneous";
                        er1.Value.Add("TNT server could not parse the request. Please contact the administrator.");
                    }
                    else
                    {
                        // There are validations errors
                        var list = (from r in xml.Descendants("ERROR")
                                    select new
                                    {
                                        ErrorCode = r.Element("CODE") != null ? r.Element("CODE").Value : "",
                                        Description = r.Element("DESCRIPTION") != null ? r.Element("DESCRIPTION").Value : "",
                                        Source = r.Element("SOURCE") != null ? r.Element("SOURCE").Value : ""
                                    }).ToList();
                        //_log.Error(list[0].Description);
                        // Log to elmah
                        if (list.Count > 0)
                        {
                            dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                            if (AppSettings.ShipmentCreatedFrom != "BATCH")
                            {
                                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));
                            }
                        }
                        if (list.Count > 0)
                        {

                            //_log.Error(list[0].Description);
                            // Step 1 : Get all address erros
                            var addressErrors = list.Where(p => p.Description.Contains("address") || p.Description.Contains("town")).ToList();

                            er1 = new FrayteKeyValue();
                            if (addressErrors.Count > 0)
                            {
                                er1.Value = new List<string>();
                                er1.Key = "Address";
                                er1.Value = new List<string>();
                                foreach (var data in addressErrors)
                                {
                                    er1.Value.Add(data.Description);
                                }
                                if (er1.Value.Count > 0)
                                    error.MiscErrors.Add(er1);

                                // remove from main list
                                foreach (var data in addressErrors)
                                {
                                    list.Remove(data);
                                }
                            }


                            // Step 2 : Get all address erros
                            var packageErrors = list.Where(p => p.Description.Contains("Length") ||
                            p.Description.Contains("Weight") ||
                            p.Description.Contains("Width") ||
                            p.Description.Contains("Height")).ToList();

                            if (packageErrors.Count > 0)
                            {
                                er1 = new FrayteKeyValue();
                                er1.Key = "Package";
                                er1.Value = new List<string>();
                                foreach (var data in packageErrors)
                                {
                                    er1.Value.Add(data.Description);
                                }
                                if (er1.Value.Count > 0)
                                    error.MiscErrors.Add(er1);
                                // remove from main list
                                foreach (var data in packageErrors)
                                {
                                    list.Remove(data);
                                }
                            }

                            // remaining errors  are off type miscellaneous
                            if (list.Count > 0)
                            {
                                er1 = new FrayteKeyValue();
                                er1.Key = "Miscellaneous";
                                er1.Value = new List<string>();
                                foreach (var data in list)
                                {
                                    er1.Value.Add(data.Description);
                                }
                                if (er1.Value.Count > 0)
                                    error.MiscErrors.Add(er1);
                            }
                        }
                    }
                    error.Status = false;
                }

                //  Create Html from label xml to create devexpress label image
                if (!string.IsNullOrEmpty(trackingCode))
                {
                    TNTResponse.TNTShipmentResponse.TrackingCode = trackingCode;
                    TNTResponse.TNTShipmentResponse.ConnoteReply = connoteReply;
                    TNTResponse.TNTShipmentResponse.shipmentXML = shipmentXML;
                    TNTResponse.TNTShipmentResponse.BookingRefNo = bookRefCode;
                    error.Status = true;
                    TNTResponse.Error = error;
                }
                else
                {
                    error.Status = false;
                    TNTResponse.Error = error;
                    new DirectShipmentRepository().SaveEasyPosyPickUpObject(@resultReply, shipmentXML, DraftShipmentId);
                    //_log.Error(TNTResponse.Error.ToString());
                    return TNTResponse;
                }
            }
            else
            {
                error.Status = false;
                er = new FrayteKeyValue();
                er.Key = "Misscllaneous Errors";
                er.Value = new List<string>() { "Something bad happen please try again later." };
                error.MiscErrors.Add(er);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Fill the record in LogisticIntegration Table"));
                TNTResponse.Error = error;
                new DirectShipmentRepository().SaveEasyPosyPickUpObject(Newtonsoft.Json.JsonConvert.SerializeObject(error).ToString(), shipmentXML, DraftShipmentId);
                return TNTResponse;
            }
            return TNTResponse;
        }

        #endregion 

        #region  TNT XML

        public string CreateTNTXMl(TNTShipmentDetail shipmentDetail)
        {
            try
            {
                return File.ReadAllText(CreateDirectBookingXMLForTNT(shipmentDetail));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return "";
            }
        }

        private static string CreateDirectBookingXMLForTNT(TNTShipmentDetail directBookingDetail)
        {
            try
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = new UTF8Encoding(true);
                xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
                xmlWriterSettings.Indent = true;
                var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.TNT);

                //xmlPath = It will be the path where xml will saved  

                string xmlPath = string.Empty;
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
                        xmlPath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");
                    }

                }

                if (!Directory.Exists(xmlPath))
                {
                    Directory.CreateDirectory(xmlPath);
                }

                xmlPath = xmlPath + "/tempTNTShipment.xml";

                if (File.Exists(xmlPath))
                {
                    File.Delete(xmlPath);
                }
                using (var xmlWriter = XmlWriter.Create(xmlPath))
                {
                    xmlWriter.WriteStartDocument();
                    if (logisticIntegration != null)
                    {
                        xmlWriter.WriteStartElement("ESHIPPER");

                        //Step 1. Create login
                        CreateLoginNode(xmlWriter, logisticIntegration);

                        //Step 2. Create Consignment Batch
                        CreateConsignmentBatch(xmlWriter, directBookingDetail);

                        //Step 3. Create Activity
                        CreateActivity(xmlWriter);

                        //Step 4. End of ESHIPPER
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndDocument();
                }
                return xmlPath;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string CreateLabelXML(TNTShipmentDetail directBookingDetail, string sequenceNumbers, string totalNumberOfPieces, TNTPackage Package, string consignmentNumber)
        {
            try
            {
                return File.ReadAllText(CreateLabelXMLForTNT(directBookingDetail, sequenceNumbers, totalNumberOfPieces, Package, consignmentNumber));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return "";
            }
        }

        private static string CreateLabelXMLForTNT(TNTShipmentDetail directBookingDetail, string sequenceNumbers, string totalNumberOfPieces, TNTPackage Package, string consignmentNumber)
        {
            try
            {
                var OperationZone = UtilityRepository.GetOperationZone().OperationZoneId;
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = new UTF8Encoding(true);
                xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
                xmlWriterSettings.Indent = true;
                //xmlPath = It will be the path where xml will saved  

                string xmlPath = string.Empty;
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
                        xmlPath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");
                    }

                }

                if (!Directory.Exists(xmlPath))
                {
                    Directory.CreateDirectory(xmlPath);
                }

                xmlPath = xmlPath + "/tempTNTLabel.xml";

                if (File.Exists(xmlPath))
                {
                    File.Delete(xmlPath);
                }
                using (var xmlWriter = XmlWriter.Create(xmlPath))
                {
                    //Regex.Replace(input, "[A-Za-z]", "");
                    consignmentNumber = Regex.Replace(consignmentNumber, "[A-Za-z]", "");
                    xmlWriter.WriteStartDocument();
                    //start labelRequest
                    xmlWriter.WriteStartElement("labelRequest");

                    //start consignment
                    xmlWriter.WriteStartElement("consignment");
                    xmlWriter.WriteAttributeString("key", "CON1");

                    xmlWriter.WriteStartElement("consignmentIdentity");

                    CreateElement(xmlWriter, "consignmentNumber", consignmentNumber);
                    CreateElement(xmlWriter, "customerReference", directBookingDetail.FrayteNumber);

                    xmlWriter.WriteEndElement();

                    var data = directBookingDetail.ReferenceDetail.CollectionDate.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                    CreateElement(xmlWriter, "collectionDateTime", data);
                    //sender
                    xmlWriter.WriteStartElement("sender");
                    CreateSenderorDeliveryDetail(xmlWriter, directBookingDetail.ShipFrom, "sender");
                    xmlWriter.WriteEndElement();

                    //Delivery
                    xmlWriter.WriteStartElement("delivery");
                    CreateSenderorDeliveryDetail(xmlWriter, directBookingDetail.ShipTo, "delivery");
                    xmlWriter.WriteEndElement();

                    //  product                 
                    xmlWriter.WriteStartElement("product");
                    CreateElement(xmlWriter, "lineOfBusiness", "2");
                    CreateElement(xmlWriter, "groupId", "0");
                    CreateElement(xmlWriter, "subGroupId", "0");
                    if (directBookingDetail.TNTService.RateType == "Economy" && directBookingDetail.ParcelType.ParcelType == "Letter")
                    {

                        CreateElement(xmlWriter, "id", "EC");
                        CreateElement(xmlWriter, "type", "D");
                    }
                    if (directBookingDetail.TNTService.RateType == "Economy" && directBookingDetail.ParcelType.ParcelType == "Parcel")
                    {

                        CreateElement(xmlWriter, "id", "EC");
                        CreateElement(xmlWriter, "type", "N");
                    }
                    if (directBookingDetail.TNTService.RateType == "Express" && directBookingDetail.ParcelType.ParcelType == "Parcel")
                    {
                        CreateElement(xmlWriter, "id", "EX");
                        CreateElement(xmlWriter, "type", "N");
                    }

                    if (directBookingDetail.TNTService.RateType == "Express" && directBookingDetail.ParcelType.ParcelType == "Letter")
                    {

                        CreateElement(xmlWriter, "id", "EX");
                        CreateElement(xmlWriter, "type", "D");
                    }

                    // CreateElement(xmlWriter, "option", "PR");
                    //end product
                    xmlWriter.WriteEndElement();
                    //account
                    xmlWriter.WriteStartElement("account");
                    CreateElement(xmlWriter, "accountNumber", directBookingDetail.TNTService.CourierAccountNumber);
                    if (OperationZone == 1)
                    {
                        CreateElement(xmlWriter, "accountCountry", "HK");
                    }
                    else if (OperationZone == 2)
                    {
                        CreateElement(xmlWriter, "accountCountry", "GB");
                    }
                    //end account
                    xmlWriter.WriteEndElement();

                    CreateElement(xmlWriter, "totalNumberOfPieces", totalNumberOfPieces);
                    //pieceLine
                    xmlWriter.WriteStartElement("pieceLine");
                    CreateElement(xmlWriter, "identifier", "1");
                    CreateElement(xmlWriter, "goodsDescription", directBookingDetail.ReferenceDetail.Reference1);

                    xmlWriter.WriteStartElement("pieceMeasurements");
                    if (directBookingDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms)
                    {
                        CreateElement(xmlWriter, "length", ((Package.Length) / 100).ToString("0.##"));
                        CreateElement(xmlWriter, "width", ((Package.Weight) / 100).ToString("0.##"));
                        CreateElement(xmlWriter, "height", ((Package.Height) / 100).ToString("0.##"));
                        CreateElement(xmlWriter, "weight", Package.Weight.ToString());
                    }
                    else
                    {
                        CreateElement(xmlWriter, "length", ((Package.Length) * 2.54m).ToString("0.##"));
                        CreateElement(xmlWriter, "width", ((Package.Weight) * 2.54m).ToString("0.##"));
                        CreateElement(xmlWriter, "height", ((Package.Height) * 2.54m).ToString("0.##"));
                        CreateElement(xmlWriter, "weight", ((Package.Weight) * 0.5m).ToString("0.##"));
                    }

                    //End pieceMeasurements
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("pieces");
                    CreateElement(xmlWriter, "sequenceNumbers", sequenceNumbers);
                    CreateElement(xmlWriter, "pieceReference", directBookingDetail.FrayteNumber + "-" + directBookingDetail.ReferenceDetail.Reference1);
                    //end pieces
                    xmlWriter.WriteEndElement();
                    //end pieceLine
                    xmlWriter.WriteEndElement();

                    //End consignment
                    xmlWriter.WriteEndElement();

                    //End labelRequest
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndDocument();
                }
                return xmlPath;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #region Label XML 

        public string GetLabelxml(string body)
        {
            var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.TNT);
            try
            {
                //Call Express Connect
                using (var client = new HttpClient())
                {
                    var httpContent = new StringContent(body, Encoding.UTF8, "text/xml");
                    var testUri = new Uri(logisticIntegration.LabelApiUrl);
                    var httpResponseMessage = client.PostAsync(testUri, httpContent).Result;
                    if (httpResponseMessage.StatusCode.ToString() == "OK")
                    {
                        return httpResponseMessage.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        private static void CreateSenderorDeliveryDetail(XmlWriter xmlWriter, TNTAddress collectionAddress, string Element)
        {
            if (!string.IsNullOrWhiteSpace(collectionAddress.Address))
            {

                if (collectionAddress.Address.Length > 30)
                {
                    collectionAddress.Address = collectionAddress.Address.Substring(0, 30);
                }

            }
            if (!string.IsNullOrWhiteSpace(collectionAddress.Address2))
            {
                if (collectionAddress.Address2.Length > 30)
                {
                    collectionAddress.Address2 = collectionAddress.Address2.Substring(0, 30);
                }
            }


            if (Element == "sender")
            {

                CreateElement(xmlWriter, "name", !string.IsNullOrEmpty(collectionAddress.CompanyName) ? collectionAddress.CompanyName : collectionAddress.FirstName + " " + collectionAddress.LastName);
                CreateElement(xmlWriter, "addressLine1", collectionAddress.Address);
                CreateElement(xmlWriter, "addressLine2", collectionAddress.Address2);
                CreateElement(xmlWriter, "addressLine3", "");
                CreateElement(xmlWriter, "town", collectionAddress.City);
                CreateElement(xmlWriter, "exactMatch", "Y");
                CreateElement(xmlWriter, "province", collectionAddress.State);
                CreateElement(xmlWriter, "postcode", collectionAddress.PostCode);
                CreateElement(xmlWriter, "country", collectionAddress.Country.Code2);
            }
            else if (Element == "delivery")
            {
                CreateElement(xmlWriter, "name", !string.IsNullOrEmpty(collectionAddress.CompanyName) ? collectionAddress.CompanyName : collectionAddress.FirstName + " " + collectionAddress.LastName);
                CreateElement(xmlWriter, "addressLine1", collectionAddress.Address);
                CreateElement(xmlWriter, "addressLine2", collectionAddress.Address2);
                CreateElement(xmlWriter, "addressLine3", "");
                CreateElement(xmlWriter, "town", collectionAddress.City);
                CreateElement(xmlWriter, "exactMatch", "Y");
                CreateElement(xmlWriter, "province", collectionAddress.State);
                CreateElement(xmlWriter, "postcode", collectionAddress.PostCode);
                CreateElement(xmlWriter, "country", collectionAddress.Country.Code2);
            }
        }

        #endregion

        private static void CreateConsignmentBatch(XmlWriter xmlWriter, TNTShipmentDetail directBookingDetail)
        {
            xmlWriter.WriteStartElement("CONSIGNMENTBATCH");

            //Create SENDER
            CreateSender(xmlWriter, directBookingDetail);

            //Create CONSIGNMENT
            CreateConsignment(xmlWriter, directBookingDetail);

            // End of CONSIGNMENTBATCH
            xmlWriter.WriteEndElement();

            // End of CONSIGNMENTBATCH
            // xmlWriter.WriteEndElement();
        }

        private static void CreateActivity(XmlWriter xmlWriter)
        {
            // var uniqueCONREF = "qaas";
            xmlWriter.WriteStartElement("ACTIVITY");

            xmlWriter.WriteStartElement("CREATE");
            CreateElement(xmlWriter, "CONREF", uniqueCONREF);
            // End of CREATE
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("RATE");
            CreateElement(xmlWriter, "CONREF", uniqueCONREF);
            // End of RATE
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("BOOK");
            xmlWriter.WriteAttributeString("ShowBookingRef", "Y");
            //CreateElement(xmlWriter, "ShowBookingRef", "Y");

            CreateElement(xmlWriter, "CONREF", uniqueCONREF);
            // End of BOOK
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("PRINT");

            xmlWriter.WriteStartElement("CONNOTE");
            CreateElement(xmlWriter, "CONREF", uniqueCONREF);
            // End of CONNOTE
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("LABEL");
            CreateElement(xmlWriter, "CONREF", uniqueCONREF);
            // End of LABEL
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("MANIFEST");
            CreateElement(xmlWriter, "CONREF", uniqueCONREF);
            // End of MANIFEST
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("INVOICE");
            CreateElement(xmlWriter, "CONREF", uniqueCONREF);
            // End of INVOICE
            xmlWriter.WriteEndElement();

            // End of PRINT
            xmlWriter.WriteEndElement();

            // End of ACTIVITY
            xmlWriter.WriteEndElement();
        }

        private static void CreateConsignment(XmlWriter xmlWriter, TNTShipmentDetail directBookingDetail)
        {
            xmlWriter.WriteStartElement("CONSIGNMENT");

            //var uniqueCONREF = directBookingDetail.FrayteNumber;
            uniqueCONREF = directBookingDetail.ReferenceDetail.Reference1;
            CreateElement(xmlWriter, "CONREF", uniqueCONREF);

            xmlWriter.WriteStartElement("DETAILS");

            xmlWriter.WriteStartElement("RECEIVER");
            if (!string.IsNullOrWhiteSpace(directBookingDetail.TNTService.LogisticType) && directBookingDetail.TNTService.LogisticType == FrayteExportType.Import)
            {
                CreateImportRecevierContactDetail(xmlWriter, directBookingDetail.ShipTo, "RECEIVER", directBookingDetail.TNTService.CourierAccountNumber, directBookingDetail.TNTService.LogisticType, directBookingDetail.PayTaxAndDuties);
            }
            else
            {
                //Export Recevier==Delivery and Import Delivery=Export Delivery
                CreateImportExportRecevierorDiliveryContactDetail(xmlWriter, directBookingDetail.ShipTo, "RECEIVER", directBookingDetail.TNTService.CourierAccountNumber, directBookingDetail.TNTService.LogisticType, directBookingDetail.PayTaxAndDuties);
            }
            // End of RECEIVER
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("DELIVERY");

            if (directBookingDetail.TNTService.LogisticType == FrayteExportType.Export)
            {
                // Export Recevier == Delivery
                CreateImportExportRecevierorDiliveryContactDetail(xmlWriter, directBookingDetail.ShipTo, "DELIVERY", directBookingDetail.TNTService.CourierAccountNumber, directBookingDetail.TNTService.LogisticType, directBookingDetail.PayTaxAndDuties);
            }
            if (directBookingDetail.TNTService.LogisticType == FrayteExportType.Import)
            {
                // Import Delivery = Export Delivery
                CreateImportExportRecevierorDiliveryContactDetail(xmlWriter, directBookingDetail.ShipTo, "DELIVERY", directBookingDetail.TNTService.CourierAccountNumber, directBookingDetail.TNTService.LogisticType, directBookingDetail.PayTaxAndDuties);
            }
            // End of DELIVERY
            xmlWriter.WriteEndElement();

            CreateElement(xmlWriter, "CUSTOMERREF", directBookingDetail.FrayteNumber);   // 
            CreateElement(xmlWriter, "CONTYPE", directBookingDetail.ParcelType.ParcelType == "Parcel" ? "N" : "D");
            if (directBookingDetail.TNTService.LogisticType == FrayteExportType.Import)
            {
                if (directBookingDetail.PayTaxAndDuties == FrayteTermsOfTrade.Shipper)
                {
                    CreateElement(xmlWriter, "PAYMENTIND", "S");
                }
                if (directBookingDetail.PayTaxAndDuties == FrayteTermsOfTrade.Receiver)
                {
                    CreateElement(xmlWriter, "PAYMENTIND", "R");
                }                
            }
            else
            {
                if (directBookingDetail.PayTaxAndDuties == FrayteTermsOfTrade.Shipper)
                {
                    CreateElement(xmlWriter, "PAYMENTIND", "S");
                }
                if (directBookingDetail.PayTaxAndDuties == FrayteTermsOfTrade.Receiver)
                {
                    CreateElement(xmlWriter, "PAYMENTIND", "R");
                }
            }

            CreateElement(xmlWriter, "ITEMS", getDirectBookingtotalItems(directBookingDetail)); //
            CreateElement(xmlWriter, "TOTALWEIGHT", /*getShipmentTotalWeight(directBookingDetail)*/directBookingDetail.TotalWeight);
            CreateElement(xmlWriter, "TOTALVOLUME", directBookingTotalVolume(directBookingDetail)); // cubic meter 
            CreateElement(xmlWriter, "CURRENCY", directBookingDetail.Currency.CurrencyCode);
            CreateElement(xmlWriter, "GOODSVALUE", getShipmentTotalvalue(directBookingDetail));
            //CreateElement(xmlWriter, "INSURANCEVALUE", "");  To Do remove  Enhanced Liability surcharges//
            //CreateElement(xmlWriter, "INSURANCECURRENCY", ""); To Do remove  Enhanced Liability surcharges //
            if (directBookingDetail.TNTService.RateType == "Economy" && directBookingDetail.ParcelType.ParcelType == "Letter")
            {
                CreateElement(xmlWriter, "SERVICE", "48N");
            }
            if (directBookingDetail.TNTService.RateType == "Economy" && directBookingDetail.ParcelType.ParcelType == "Parcel")
            {
                CreateElement(xmlWriter, "SERVICE", "48N");
            }
            if (directBookingDetail.TNTService.RateType == "Express" && directBookingDetail.ParcelType.ParcelType == "Parcel")
            {
                CreateElement(xmlWriter, "SERVICE", "15N");
            }
            if (directBookingDetail.TNTService.RateType == "Express" && directBookingDetail.ParcelType.ParcelType == "Letter")
            {
                CreateElement(xmlWriter, "SERVICE", "15D");
            }

            //CreateElement(xmlWriter, "OPTION", " "); To Do remove option like Priority
            CreateElement(xmlWriter, "DESCRIPTION", ShipmentContentDescription(directBookingDetail));
            CreateElement(xmlWriter, "DELIVERYINST", directBookingDetail.CustomInfo.RestrictionComments);
            foreach (var data in directBookingDetail.Packages)
            {
                xmlWriter.WriteStartElement("PACKAGE");
                //Craete Package 
                CreatePackage(xmlWriter, data, directBookingDetail.PakageCalculatonType, directBookingDetail.ShipTo.Country);
                // End of PACKAGE
                xmlWriter.WriteEndElement();
            }

            // End of DETAILS
            xmlWriter.WriteEndElement();

            //Create DETAILS
            // CreateDetails(xmlWriter, directBookingDetail);

            // End of CONSIGNMENT
            xmlWriter.WriteEndElement();
        }

        private static string ShipmentContentDescription(TNTShipmentDetail directBookingDetail)
        {
            string description = string.Empty;
            if (directBookingDetail != null && directBookingDetail.Packages.Count > 0)
            {
                foreach (var data in directBookingDetail.Packages)
                {
                    description += data.Content;
                }
            }
            return description;
        }

        private static string directBookingTotalVolume(TNTShipmentDetail directBookingDetail)
        {
            Decimal sum = 0.00M;
            if (directBookingDetail != null && directBookingDetail.Packages.Count > 0)
            {
                foreach (var data in directBookingDetail.Packages)
                {
                    for (int i = 0; i < data.CartoonValue; i++)
                    {
                        var width = 0M;
                        var height = 0M;
                        var length = 0M;
                        if (directBookingDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms)
                        {
                            width = data.Width / 100;
                            height = data.Height / 100;
                            length = data.Length / 100;
                        }
                        else if (directBookingDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs)
                        {
                            width = data.Width / 39.37M;
                            height = data.Height / 39.37M;
                            length = data.Length / 39.37M;
                        }
                        else
                        {
                            width = data.Width / 100;
                            height = data.Height / 100;
                            length = data.Length / 100;
                        }
                        sum = sum + width * height * length;
                    }
                }
            }
            return sum.ToString();
        }

        private static string getDirectBookingtotalItems(TNTShipmentDetail directBookingDetail)
        {
            if (directBookingDetail != null && directBookingDetail.Packages != null && directBookingDetail.Packages.Count > 0)
            {
                var sum = 0;
                foreach (var data in directBookingDetail.Packages)
                {
                    sum += data.CartoonValue;
                }
                return sum.ToString();
            }
            else
            {
                return "";
            }
        }

        private static string getShipmentTotalvalue(TNTShipmentDetail directBookingDetail)
        {
            Decimal sum = 0.00M;
            if (directBookingDetail != null && directBookingDetail.Packages.Count > 0)
            {
                foreach (var data in directBookingDetail.Packages)
                {
                    sum = sum + data.Value;
                }
            }
            return sum.ToString();
        }

        private static string getShipmentTotalWeight(TNTShipmentDetail directBookingDetail)
        {
            Decimal sum = 0.00M;
            if (directBookingDetail != null && directBookingDetail.Packages.Count > 0)
            {
                foreach (var data in directBookingDetail.Packages)
                {
                    var weight = 0M;
                    if (directBookingDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs)
                    {
                        weight = data.Weight / 2.20M;
                    }
                    else
                    {
                        weight = data.Weight;
                    }
                    sum = sum + data.CartoonValue * weight;
                }
            }
            return sum.ToString();
        }

        private static void CreatePackage(XmlWriter xmlWriter, TNTPackage directBookingDetail, string CalculationType, TNTCountry Country)
        {
            CreateElement(xmlWriter, "ITEMS", directBookingDetail.CartoonValue.ToString());
            CreateElement(xmlWriter, "DESCRIPTION", directBookingDetail.Content);
            CreateElement(xmlWriter, "LENGTH", CalculationType == FraytePakageCalculationType.LbToInchs ? (directBookingDetail.Length / 39.37M).ToString() : (directBookingDetail.Length / 100).ToString());
            CreateElement(xmlWriter, "HEIGHT", CalculationType == FraytePakageCalculationType.LbToInchs ? (directBookingDetail.Height / 39.37M).ToString() : (directBookingDetail.Height / 100).ToString());
            CreateElement(xmlWriter, "WIDTH", CalculationType == FraytePakageCalculationType.LbToInchs ? (directBookingDetail.Width / 39.37M).ToString() : (directBookingDetail.Width / 100).ToString());
            CreateElement(xmlWriter, "WEIGHT", CalculationType == FraytePakageCalculationType.LbToInchs ? (directBookingDetail.Weight / 2.20M).ToString() : directBookingDetail.Weight.ToString());

            xmlWriter.WriteStartElement("ARTICLE");

            CreateElement(xmlWriter, "ITEMS", directBookingDetail.CartoonValue.ToString());
            CreateElement(xmlWriter, "DESCRIPTION", directBookingDetail.Content);
            CreateElement(xmlWriter, "WEIGHT", CalculationType == FraytePakageCalculationType.LbToInchs ? (directBookingDetail.Weight / 2.20M).ToString() : directBookingDetail.Weight.ToString());
            CreateElement(xmlWriter, "INVOICEVALUE", directBookingDetail.Value.ToString()); // get service final rate 
            CreateElement(xmlWriter, "INVOICEDESC", directBookingDetail.Content);
            CreateElement(xmlWriter, "HTS", "ABC"); // 
            CreateElement(xmlWriter, "COUNTRY", Country.Code2);
            // End of ARTICLE
            xmlWriter.WriteEndElement();

            //Create ARTICLE
            //CreateCollection(xmlWriter, directBookingDetail);
        }

        private static void CreateSender(XmlWriter xmlWriter, TNTShipmentDetail directBookingDetail)
        {
            xmlWriter.WriteStartElement("SENDER");
            if (!string.IsNullOrWhiteSpace(directBookingDetail.TNTService.LogisticType) && directBookingDetail.TNTService.LogisticType == FrayteExportType.Import)
            {
                CreateImportSenderContactDetail(xmlWriter, directBookingDetail.ShipFrom, "Sender", directBookingDetail.TNTService.CourierAccountNumber, directBookingDetail.TNTService.LogisticType, directBookingDetail.PayTaxAndDuties);
                // CreateImportSenderOrReceverContactDetail(xmlWriter, directBookingDetail.ShipFrom, "Sender", directBookingDetail.TNTService.CourierAccountNumber, directBookingDetail.TNTService.LogisticType, directBookingDetail.PayTaxAndDuties);
            }
            else
            {
                CreateExportSenderContactDetail(xmlWriter, directBookingDetail.ShipFrom, "Sender", directBookingDetail.TNTService.CourierAccountNumber, directBookingDetail.TNTService.LogisticType, directBookingDetail.PayTaxAndDuties);
                // CreateCompanyAddressAndContactDetail(xmlWriter, directBookingDetail.ShipFrom, "Sender", directBookingDetail.TNTService.CourierAccountNumber, directBookingDetail.TNTService.LogisticType, directBookingDetail.PayTaxAndDuties);
            }
            //Create COLLECTION
            CreateCollection(xmlWriter, directBookingDetail);

            // End of SENDER
            xmlWriter.WriteEndElement();
        }

        private static void CreateCollection(XmlWriter xmlWriter, TNTShipmentDetail directBookingDetail)
        {
            xmlWriter.WriteStartElement("COLLECTION");
            xmlWriter.WriteStartElement("COLLECTIONADDRESS");

            CreateCompanyAddressAndContactDetail(xmlWriter, directBookingDetail.ShipFrom, "", directBookingDetail.TNTService.CourierAccountNumber, directBookingDetail.TNTService.LogisticType, directBookingDetail.PayTaxAndDuties);

            // End of COLLECTIONADDRESS
            xmlWriter.WriteEndElement();

            CreateElement(xmlWriter, "SHIPDATE", directBookingDetail.ReferenceDetail.CollectionDate.Value.ToString("dd/MM/yyyy"));

            xmlWriter.WriteStartElement("PREFCOLLECTTIME");

            CreateElement(xmlWriter, "FROM", getFormattedTimeFromString(directBookingDetail.ReferenceDetail.CollectionTime, "From")); // 
            CreateElement(xmlWriter, "TO", getFormattedTimeFromString(directBookingDetail.ReferenceDetail.CollectionTime, "To")); // need to add 1 hour

            // End of PREFCOLLECTTIME
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("ALTCOLLECTTIME");

            CreateElement(xmlWriter, "FROM", "");
            CreateElement(xmlWriter, "TO", "");

            // End of ALTCOLLECTTIME
            xmlWriter.WriteEndElement();

            CreateElement(xmlWriter, "COLLINSTRUCTIONS", "");

            // End of COLLECTION
            xmlWriter.WriteEndElement();
        }

        private static string getFormattedTimeFromString(string collectionTime, string type)
        {
            string str = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(collectionTime))
                {
                    string myStr = string.Empty;
                    if (type == "To")
                    {
                        string str1 = collectionTime.Substring(0, 2);
                        string df = collectionTime.Substring(collectionTime.Length - 2, 2);
                        int x = Convert.ToInt32(str1);
                        x = x + 1;
                        string str2 = x.ToString().Count() == 1 ? "0" + x : x + collectionTime.Substring(collectionTime.Length - 2, 2);
                        myStr = str2;
                    }
                    else
                    {
                        myStr = collectionTime;
                    }
                    if (myStr.Count() == 4)
                    {
                        foreach (var chr in myStr)
                        {
                            str += chr;
                            if (str.Count() == 2)
                            {
                                str += ":";
                            }
                        }
                    }
                    else
                    {
                        str = collectionTime;
                    }
                }
                return str;
            }
            catch (Exception ex)
            {
                return str;
            }
        }

        private static void CreateCompanyAddressAndContactDetail(XmlWriter xmlWriter, TNTAddress collectionAddress, string elementType, string CourierAccountNumber, string LogisticType, string PayTaxAndDuties)
        {
            var CountryPhoneCode = new CountryRepository().GetCountryByNameAndContryCode(collectionAddress.Country.Name, collectionAddress.Country.Code).CountryPhoneCode;
            //string elementType = string.Empty;
            CreateElement(xmlWriter, "COMPANYNAME", !string.IsNullOrEmpty(collectionAddress.CompanyName) ? collectionAddress.CompanyName : collectionAddress.FirstName + " " + collectionAddress.LastName);
            CreateElement(xmlWriter, "STREETADDRESS1", collectionAddress.Address);
            CreateElement(xmlWriter, "STREETADDRESS2", collectionAddress.Address2);
            CreateElement(xmlWriter, "STREETADDRESS3", "");
            CreateElement(xmlWriter, "CITY", collectionAddress.City);
            CreateElement(xmlWriter, "PROVINCE", collectionAddress.State);
            CreateElement(xmlWriter, "POSTCODE", collectionAddress.PostCode);
            CreateElement(xmlWriter, "COUNTRY", collectionAddress.Country.Code2);
            if (elementType == "Sender")
            {
                // new to get this from web.config
                CreateElement(xmlWriter, "ACCOUNT", CourierAccountNumber); // Don't have this
            }

            CreateElement(xmlWriter, "VAT", "");
            CreateElement(xmlWriter, "CONTACTNAME", collectionAddress.FirstName + " " + collectionAddress.LastName);

            CreateElement(xmlWriter, "CONTACTDIALCODE", string.IsNullOrWhiteSpace(CountryPhoneCode) ? CountryPhoneCode : "(+" + CountryPhoneCode + ")");
            CreateElement(xmlWriter, "CONTACTTELEPHONE", "  " + collectionAddress.Phone);
            CreateElement(xmlWriter, "CONTACTEMAIL", collectionAddress.Email);
        }

        private static void CreateImportExportRecevierorDiliveryContactDetail(XmlWriter xmlWriter, TNTAddress collectionAddress, string elementType, string CourierAccountNumber, string LogisticType, string PayTaxAndDuties)
        {
            var OperationZone = UtilityRepository.GetOperationZone().OperationZoneId;
            if (OperationZone == 1)
            {
                var CountryPhoneCode = new CountryRepository().GetCountryByNameAndContryCode(collectionAddress.Country.Name, collectionAddress.Country.Code).CountryPhoneCode;
                //string elementType = string.Empty;
                CreateElement(xmlWriter, "COMPANYNAME", !string.IsNullOrEmpty(collectionAddress.CompanyName) ? collectionAddress.CompanyName : collectionAddress.FirstName + " " + collectionAddress.LastName);
                CreateElement(xmlWriter, "STREETADDRESS1", collectionAddress.Address);
                CreateElement(xmlWriter, "STREETADDRESS2", collectionAddress.Address2);
                CreateElement(xmlWriter, "STREETADDRESS3", "");
                CreateElement(xmlWriter, "CITY", collectionAddress.City);
                CreateElement(xmlWriter, "PROVINCE", collectionAddress.State);
                CreateElement(xmlWriter, "POSTCODE", collectionAddress.PostCode);
                CreateElement(xmlWriter, "COUNTRY", collectionAddress.Country.Code2);
                CreateElement(xmlWriter, "VAT", "");
                CreateElement(xmlWriter, "CONTACTNAME", collectionAddress.FirstName + " " + collectionAddress.LastName);
                CreateElement(xmlWriter, "CONTACTDIALCODE", string.IsNullOrWhiteSpace(CountryPhoneCode) ? CountryPhoneCode : "(+" + CountryPhoneCode + ")");
                CreateElement(xmlWriter, "CONTACTTELEPHONE", "  " + collectionAddress.Phone);
                CreateElement(xmlWriter, "CONTACTEMAIL", collectionAddress.Email);
                if (PayTaxAndDuties == "Receiver" && elementType == "RECEIVER")
                {
                    // new to get this from web.config
                    CreateElement(xmlWriter, "ACCOUNT", CourierAccountNumber); // Don't have this
                }
            }
            if (OperationZone == 2)
            {
                var CountryPhoneCode = new CountryRepository().GetCountryByNameAndContryCode(collectionAddress.Country.Name, collectionAddress.Country.Code).CountryPhoneCode;
                //string elementType = string.Empty;
                CreateElement(xmlWriter, "COMPANYNAME", !string.IsNullOrEmpty(collectionAddress.CompanyName) ? collectionAddress.CompanyName : collectionAddress.FirstName + " " + collectionAddress.LastName);
                CreateElement(xmlWriter, "STREETADDRESS1", collectionAddress.Address);
                CreateElement(xmlWriter, "STREETADDRESS2", collectionAddress.Address2);
                CreateElement(xmlWriter, "STREETADDRESS3", "");
                CreateElement(xmlWriter, "CITY", collectionAddress.City);
                CreateElement(xmlWriter, "PROVINCE", collectionAddress.State);
                CreateElement(xmlWriter, "POSTCODE", collectionAddress.PostCode);
                CreateElement(xmlWriter, "COUNTRY", collectionAddress.Country.Code2);
                CreateElement(xmlWriter, "VAT", "");
                CreateElement(xmlWriter, "CONTACTNAME", collectionAddress.FirstName + " " + collectionAddress.LastName);
                CreateElement(xmlWriter, "CONTACTDIALCODE", string.IsNullOrWhiteSpace(CountryPhoneCode) ? CountryPhoneCode : "(+" + CountryPhoneCode + ")");
                CreateElement(xmlWriter, "CONTACTTELEPHONE", "  " + collectionAddress.Phone);
                CreateElement(xmlWriter, "CONTACTEMAIL", collectionAddress.Email);
                if (PayTaxAndDuties == "Receiver" && elementType == "RECEIVER")
                {
                    // new to get this from web.config
                    CreateElement(xmlWriter, "ACCOUNT", CourierAccountNumber); // Don't have this
                }
            }
        }

        private static void CreateImportRecevierContactDetail(XmlWriter xmlWriter, TNTAddress collectionAddress, string elementType, string CourierAccountNumber, string LogisticType, string PayTaxAndDuties)
        {
            var OperationZone = UtilityRepository.GetOperationZone().OperationZoneId;
            if (OperationZone == 1)
            {
                CreateElement(xmlWriter, "COMPANYNAME", "Frayte Logistics Ltd");
                CreateElement(xmlWriter, "STREETADDRESS1", "Unit 501 Kwong Loong Tai Building");
                CreateElement(xmlWriter, "STREETADDRESS2", "1016-1018 Tai Nan West Street");
                CreateElement(xmlWriter, "STREETADDRESS3", "Kowloon");
                CreateElement(xmlWriter, "CITY", "Kowloon");
                CreateElement(xmlWriter, "PROVINCE", "Cheung Sha Wan");
                CreateElement(xmlWriter, "POSTCODE", "");
                CreateElement(xmlWriter, "COUNTRY", "HK");
                CreateElement(xmlWriter, "VAT", "");
                CreateElement(xmlWriter, "CONTACTNAME", "Mr Alex");
                CreateElement(xmlWriter, "CONTACTDIALCODE", "00000");
                CreateElement(xmlWriter, "CONTACTTELEPHONE", "852 2148 4881");
                CreateElement(xmlWriter, "CONTACTEMAIL", "sales@whytecliff.com");
                CreateElement(xmlWriter, "ACCOUNT", CourierAccountNumber);
            }
            if (OperationZone == 2)
            {
                CreateElement(xmlWriter, "COMPANYNAME", "Frayte Logistics Ltd");
                CreateElement(xmlWriter, "STREETADDRESS1", "Unit 11");
                CreateElement(xmlWriter, "STREETADDRESS2", "SA1 Business Park");
                CreateElement(xmlWriter, "STREETADDRESS3", "Langdon Road");
                CreateElement(xmlWriter, "CITY", "Swansea");
                CreateElement(xmlWriter, "PROVINCE", "WEST GLAMORGAN");
                CreateElement(xmlWriter, "POSTCODE", "SA1 8DB");
                CreateElement(xmlWriter, "COUNTRY", "GB");
                CreateElement(xmlWriter, "VAT", "");
                CreateElement(xmlWriter, "CONTACTNAME", "Nathan Phipps");
                CreateElement(xmlWriter, "CONTACTDIALCODE", "00000");
                CreateElement(xmlWriter, "CONTACTTELEPHONE", "07930547106");
                CreateElement(xmlWriter, "CONTACTEMAIL", "Nathan@frayte.co.uk");
                CreateElement(xmlWriter, "ACCOUNT", CourierAccountNumber);
            }
        }

        private static void CreateImportSenderContactDetail(XmlWriter xmlWriter, TNTAddress collectionAddress, string elementType, string CourierAccountNumber, string LogisticType, string PayTaxAndDuties)
        {
            var OperationZone = UtilityRepository.GetOperationZone().OperationZoneId;
            if (OperationZone == 1)
            {
                var CountryPhoneCode = new CountryRepository().GetCountryByNameAndContryCode(collectionAddress.Country.Name, collectionAddress.Country.Code).CountryPhoneCode;
                //string elementType = string.Empty;
                CreateElement(xmlWriter, "COMPANYNAME", !string.IsNullOrEmpty(collectionAddress.CompanyName) ? collectionAddress.CompanyName : collectionAddress.FirstName + " " + collectionAddress.LastName);
                CreateElement(xmlWriter, "STREETADDRESS1", collectionAddress.Address);
                CreateElement(xmlWriter, "STREETADDRESS2", collectionAddress.Address2);
                CreateElement(xmlWriter, "STREETADDRESS3", "");
                CreateElement(xmlWriter, "CITY", collectionAddress.City);
                CreateElement(xmlWriter, "PROVINCE", collectionAddress.State);
                CreateElement(xmlWriter, "POSTCODE", collectionAddress.PostCode);
                CreateElement(xmlWriter, "COUNTRY", collectionAddress.Country.Code2);

                // new to get this from web.config
                CreateElement(xmlWriter, "ACCOUNT", ""); // Don't have this

                CreateElement(xmlWriter, "VAT", "");
                CreateElement(xmlWriter, "CONTACTNAME", collectionAddress.FirstName + " " + collectionAddress.LastName);

                CreateElement(xmlWriter, "CONTACTDIALCODE", string.IsNullOrWhiteSpace(CountryPhoneCode) ? CountryPhoneCode : "(+" + CountryPhoneCode + ")");
                CreateElement(xmlWriter, "CONTACTTELEPHONE", "  " + collectionAddress.Phone);
                CreateElement(xmlWriter, "CONTACTEMAIL", collectionAddress.Email);
            }
            else if (OperationZone == 2)
            {
                var CountryPhoneCode = new CountryRepository().GetCountryByNameAndContryCode(collectionAddress.Country.Name, collectionAddress.Country.Code).CountryPhoneCode;
                //string elementType = string.Empty;
                CreateElement(xmlWriter, "COMPANYNAME", !string.IsNullOrEmpty(collectionAddress.CompanyName) ? collectionAddress.CompanyName : collectionAddress.FirstName + " " + collectionAddress.LastName);
                CreateElement(xmlWriter, "STREETADDRESS1", collectionAddress.Address);
                CreateElement(xmlWriter, "STREETADDRESS2", collectionAddress.Address2);
                CreateElement(xmlWriter, "STREETADDRESS3", "");
                CreateElement(xmlWriter, "CITY", collectionAddress.City);
                CreateElement(xmlWriter, "PROVINCE", collectionAddress.State);
                CreateElement(xmlWriter, "POSTCODE", collectionAddress.PostCode);
                CreateElement(xmlWriter, "COUNTRY", collectionAddress.Country.Code2);
                // new to get this from web.config
                CreateElement(xmlWriter, "ACCOUNT", ""); // Don't have this

                CreateElement(xmlWriter, "VAT", "");
                CreateElement(xmlWriter, "CONTACTNAME", collectionAddress.FirstName + " " + collectionAddress.LastName);

                CreateElement(xmlWriter, "CONTACTDIALCODE", string.IsNullOrWhiteSpace(CountryPhoneCode) ? CountryPhoneCode : "(+" + CountryPhoneCode + ")");
                CreateElement(xmlWriter, "CONTACTTELEPHONE", "  " + collectionAddress.Phone);
                CreateElement(xmlWriter, "CONTACTEMAIL", collectionAddress.Email);
            }
        }

        private static void CreateExportSenderContactDetail(XmlWriter xmlWriter, TNTAddress collectionAddress, string elementType, string CourierAccountNumber, string LogisticType, string PayTaxAndDuties)
        {
            var OperationZone = UtilityRepository.GetOperationZone().OperationZoneId;
            if (OperationZone == 1)
            {
                //var CountryPhoneCode = new CountryRepository().GetCountryByNameAndContryCode(collectionAddress.Country.Name, collectionAddress.Country.Code).CountryPhoneCode;
                ////string elementType = string.Empty;
                //CreateElement(xmlWriter, "COMPANYNAME", !string.IsNullOrEmpty(collectionAddress.CompanyName) ? collectionAddress.CompanyName : collectionAddress.FirstName + " " + collectionAddress.LastName);
                //CreateElement(xmlWriter, "STREETADDRESS1", collectionAddress.Address);
                //CreateElement(xmlWriter, "STREETADDRESS2", collectionAddress.Address2);
                //CreateElement(xmlWriter, "STREETADDRESS3", "");
                //CreateElement(xmlWriter, "CITY", collectionAddress.City);
                //CreateElement(xmlWriter, "PROVINCE", collectionAddress.State);
                //CreateElement(xmlWriter, "POSTCODE", collectionAddress.PostCode);
                //CreateElement(xmlWriter, "COUNTRY", collectionAddress.Country.Code2);
                //if (PayTaxAndDuties == "Sender")
                //{
                //    // new to get this from web.config
                //    CreateElement(xmlWriter, "ACCOUNT", CourierAccountNumber); // Don't have this
                //}

                //CreateElement(xmlWriter, "VAT", "");
                //CreateElement(xmlWriter, "CONTACTNAME", collectionAddress.FirstName + " " + collectionAddress.LastName);

                //CreateElement(xmlWriter, "CONTACTDIALCODE", string.IsNullOrWhiteSpace(CountryPhoneCode) ? CountryPhoneCode : "(+" + CountryPhoneCode + ")");
                //CreateElement(xmlWriter, "CONTACTTELEPHONE", "  " + collectionAddress.Phone);
                //CreateElement(xmlWriter, "CONTACTEMAIL", collectionAddress.Email);

            }
            else if (OperationZone == 2)
            {

                CreateElement(xmlWriter, "COMPANYNAME", "Frayte Logistics Ltd");
                CreateElement(xmlWriter, "STREETADDRESS1", "Unit 11");
                CreateElement(xmlWriter, "STREETADDRESS2", "SA1 Business Park");
                CreateElement(xmlWriter, "STREETADDRESS3", "");
                CreateElement(xmlWriter, "CITY", "Swansea");
                CreateElement(xmlWriter, "PROVINCE", "");
                CreateElement(xmlWriter, "POSTCODE", "SA1 8DB");
                CreateElement(xmlWriter, "COUNTRY", "GB");

                CreateElement(xmlWriter, "ACCOUNT", CourierAccountNumber); // Don't have this

                CreateElement(xmlWriter, "VAT", "");
                CreateElement(xmlWriter, "CONTACTNAME", "Nathan Phipps");
                CreateElement(xmlWriter, "CONTACTDIALCODE", "00000");
                CreateElement(xmlWriter, "CONTACTTELEPHONE", "07930547106");
                CreateElement(xmlWriter, "CONTACTEMAIL", "Nathan@frayte.co.uk");

                //var CountryPhoneCode = new CountryRepository().GetCountryByNameAndContryCode(collectionAddress.Country.Name, collectionAddress.Country.Code).CountryPhoneCode;
                ////string elementType = string.Empty;
                //CreateElement(xmlWriter, "COMPANYNAME", !string.IsNullOrEmpty(collectionAddress.CompanyName) ? collectionAddress.CompanyName : collectionAddress.FirstName + " " + collectionAddress.LastName);
                //CreateElement(xmlWriter, "STREETADDRESS1", collectionAddress.Address);
                //CreateElement(xmlWriter, "STREETADDRESS2", collectionAddress.Address2);
                //CreateElement(xmlWriter, "STREETADDRESS3", "");
                //CreateElement(xmlWriter, "CITY", collectionAddress.City);
                //CreateElement(xmlWriter, "PROVINCE", collectionAddress.State);
                //CreateElement(xmlWriter, "POSTCODE", collectionAddress.PostCode);
                //CreateElement(xmlWriter, "COUNTRY", collectionAddress.Country.Code2);
                //if (PayTaxAndDuties == "Sender")
                //{
                //    // new to get this from web.config
                //    CreateElement(xmlWriter, "ACCOUNT", CourierAccountNumber); // Don't have this
                //}

                //CreateElement(xmlWriter, "VAT", "");
                //CreateElement(xmlWriter, "CONTACTNAME", collectionAddress.FirstName + " " + collectionAddress.LastName);

                //CreateElement(xmlWriter, "CONTACTDIALCODE", string.IsNullOrWhiteSpace(CountryPhoneCode) ? CountryPhoneCode : "(+" + CountryPhoneCode + ")");
                //CreateElement(xmlWriter, "CONTACTTELEPHONE", "  " + collectionAddress.Phone);
                //CreateElement(xmlWriter, "CONTACTEMAIL", collectionAddress.Email);
            }
        }

        private static void CreateElement(XmlWriter xmlWriter, string elementName, string elementValue)
        {
            xmlWriter.WriteStartElement(elementName);
            xmlWriter.WriteString(elementValue);
            xmlWriter.WriteEndElement();
        }

        private static void CreateLoginNode(XmlWriter xmlWriter, FrayteLogisticIntegration logisticIntegration)
        {
            xmlWriter.WriteStartElement("LOGIN");

            xmlWriter.WriteStartElement("COMPANY");
            xmlWriter.WriteString(logisticIntegration.UserName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("PASSWORD");
            xmlWriter.WriteString(logisticIntegration.Password);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("APPID");
            xmlWriter.WriteString(logisticIntegration.AppId);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("APPVERSION");
            xmlWriter.WriteString(logisticIntegration.AppVersion);
            xmlWriter.WriteEndElement();

            // End of LOGIN
            xmlWriter.WriteEndElement();
        }

        #endregion

        #region Map to TNT Object

        public TNTShipmentDetail MapDirectBookingObjToTNTobj(DirectBookingShipmentDraftDetail shipmentDetail)
        {
            try
            {
                TNTShipmentDetail shipment = new TNTShipmentDetail();

                shipment.DraftShipmentId = shipmentDetail.DirectShipmentDraftId;

                // Step 1: Map ShipFrom
                shipment.ShipFrom = new TNTAddress();
                mapDirectBookingAddress(shipmentDetail.ShipFrom.Country.CountryId, shipmentDetail.ShipFrom, shipment.ShipFrom, TNTAddressType.ShipFrom);

                // Step 2: Map ShipFrom
                shipment.ShipTo = new TNTAddress();
                mapDirectBookingAddress(shipmentDetail.ShipTo.Country.CountryId, shipmentDetail.ShipTo, shipment.ShipTo, TNTAddressType.ShipTo);

                // Step 3: Map Packages
                shipment.Packages = new List<TNTPackage>();
                mapDirectBookingPackages(shipmentDetail.Packages, shipment.Packages);
                shipment.TotalWeight = shipmentDetail.CustomerRateCard.Weight.ToString("0.##");

                // Step 4 : map custom detail ( No Custom info for UK To UK in eCommerce )
                shipment.CustomInfo = new TNTCustomInformation();
                mapDirectBookingCustomInfo(shipmentDetail.CustomInfo, shipment.CustomInfo);

                // Step 5: map easypost service
                shipment.TNTService = new TNTServicerService();
                mapDirectBookingService(shipmentDetail, shipment.TNTService);

                // Step 6: map other shipment detail
                mapOtherDirectBookingInfo(shipmentDetail, shipment);
                return shipment;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Could not map eCommerc shipment to easypost shipment : " + ex.Message));
                return null;
            }
        }

        #region Map DirectBooking to easyost and integrate in easy post

        internal protected void mapDirectBookingAddress(int ShipToCountrryId, DirectBookingDraftCollection address, TNTAddress easyPostAddress, string addressType)
        {
            var Country = dbContext.Countries.Where(p => p.CountryId == ShipToCountrryId).FirstOrDefault();

            string Company = string.Empty;
            if (string.IsNullOrEmpty(address.CompanyName))
            {
                Company = address.FirstName + " " + address.LastName;
                easyPostAddress.CompanyName = Company;
            }
            else
            {
                easyPostAddress.CompanyName = address.CompanyName;
            }

            if (addressType == TNTAddressType.ShipFrom)
            {
                easyPostAddress.FirstName = address.FirstName;
                easyPostAddress.LastName = address.LastName;
                easyPostAddress.CompanyName = address.CompanyName;
                easyPostAddress.Country = new TNTCountry();
                easyPostAddress.Country.Name = address.Country.Name;
                easyPostAddress.Address = address.Address;
                easyPostAddress.Address2 = address.Address2;
                easyPostAddress.City = address.City;
                easyPostAddress.State = address.State;
                easyPostAddress.Phone = address.Phone;
                easyPostAddress.PostCode = address.PostCode;
                easyPostAddress.Country.CountryId = Country.CountryId;
                easyPostAddress.Country.Code = Country.CountryCode;
                easyPostAddress.Country.Code2 = Country.CountryCode2;
            }
            else
            {
                easyPostAddress.FirstName = address.FirstName;
                easyPostAddress.LastName = address.LastName;
                easyPostAddress.CompanyName = address.CompanyName;
                easyPostAddress.Country = new TNTCountry();
                easyPostAddress.Country.CountryId = address.Country.CountryId;
                easyPostAddress.Country.Code = address.Country.Code;
                easyPostAddress.Country.Code2 = address.Country.Code2;
                easyPostAddress.Country.Name = address.Country.Name;
                easyPostAddress.Address = address.Address;
                easyPostAddress.Address2 = address.Address2;
                easyPostAddress.Area = address.Area;
                easyPostAddress.City = address.City;
                easyPostAddress.PostCode = address.PostCode;
                easyPostAddress.State = address.State;
                easyPostAddress.Phone = address.Phone;
            }
        }

        internal protected void mapDirectBookingPackages(List<PackageDraft> packages, List<TNTPackage> easyPostPackages)
        {
            TNTPackage package;
            foreach (var data in packages)
            {
                package = new TNTPackage();
                package.CartoonValue = data.CartoonValue;
                package.Content = data.Content;
                package.Height = data.Height;
                package.Length = data.Length;
                package.Value = data.Value;
                package.Weight = data.Weight;
                package.Width = data.Width;
                easyPostPackages.Add(package);
            }
        }

        internal protected void mapDirectBookingService(DirectBookingShipmentDraftDetail shipmentDetail, TNTServicerService easyPostService)
        {
            if (shipmentDetail.CustomerRateCard != null)
            {
                easyPostService.Courier = shipmentDetail.CustomerRateCard.CourierName;
                easyPostService.CourierDisplay = shipmentDetail.CustomerRateCard.DisplayName;
                easyPostService.CourierAccountId = shipmentDetail.CustomerRateCard.IntegrationAccountId;
                easyPostService.CourierAccountNumber = shipmentDetail.CustomerRateCard.CourierAccountNo;
                easyPostService.RateType = shipmentDetail.CustomerRateCard.RateType;
                easyPostService.LogisticType = shipmentDetail.CustomerRateCard.LogisticType;
            }
        }

        internal protected void mapDirectBookingCustomInfo(CustomInformation customInfo, TNTCustomInformation easyPostCustomInfo)
        {
            easyPostCustomInfo.ContentsType = customInfo.ContentsType;
            easyPostCustomInfo.ContentsExplanation = customInfo.ContentsExplanation;
            easyPostCustomInfo.RestrictionType = customInfo.RestrictionType;
            easyPostCustomInfo.RestrictionComments = customInfo.RestrictionComments;
            easyPostCustomInfo.CustomsCertify = true;
            easyPostCustomInfo.EelPfc = customInfo.EelPfc;
            easyPostCustomInfo.CustomsSigner = customInfo.CustomsSigner;
            easyPostCustomInfo.NonDeliveryOption = customInfo.NonDeliveryOption;
        }

        internal protected void mapOtherDirectBookingInfo(DirectBookingShipmentDraftDetail shipmentDetail, TNTShipmentDetail shipment)
        {
            shipment.BookingApp = shipmentDetail.BookingStatusType;
            shipment.ModuleType = FrayteShipmentServiceType.DirectBooking;
            shipment.Currency = new TNTCurrency();
            shipment.Currency.CurrencyCode = shipmentDetail.Currency.CurrencyCode;
            shipment.Currency.CurrencyDescription = shipmentDetail.Currency.CurrencyDescription;

            shipment.FrayteNumber = shipmentDetail.FrayteNumber;
            shipment.PakageCalculatonType = shipmentDetail.PakageCalculatonType;
            shipment.ParcelType = new TNTParcelType();
            shipment.ParcelType.ParcelType = shipmentDetail.ParcelType.ParcelType;
            shipment.ParcelType.ParcelDescription = shipmentDetail.ParcelType.ParcelDescription;
            shipment.PaymentPartyAccountNumber = shipmentDetail.PaymentPartyAccountNumber;
            shipment.PayTaxAndDuties = shipmentDetail.PayTaxAndDuties;
            shipment.TaxAndDutiesAcceptedBy = shipmentDetail.TaxAndDutiesAcceptedBy;
            shipment.CreatedOn = DateTime.UtcNow;
            shipment.ReferenceDetail = new TNTReferenceDetail();
            shipment.ReferenceDetail.Reference1 = shipmentDetail.ReferenceDetail.Reference1;
            shipment.ReferenceDetail.CollectionDate = shipmentDetail.ReferenceDetail.CollectionDate;
            shipment.ReferenceDetail.CollectionTime = shipmentDetail.ReferenceDetail.CollectionTime;
            shipment.ReferenceDetail.ContentDescription = shipmentDetail.ReferenceDetail.ContentDescription;
            shipment.ReferenceDetail.SpecialInstruction = shipmentDetail.ReferenceDetail.SpecialInstruction;
        }

        #endregion

        #endregion

        private static string ReadXMLDocument(string xml_in)
        {
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(xml_in));
            XElement xmlFile = XElement.Load(ms);
            var status = xmlFile.Descendants("CREATE").Elements("CONNUMBER");
            string val = GetElementValue(status);
            return val;
        }

        private static string BookXMLDocument(string xml_in)
        {
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(xml_in));
            XElement xmlFile = XElement.Load(ms);
            var status = xmlFile.Descendants("CONSIGNMENT").Elements("BOOKINGREF");
            string val = GetElementValue(status);
            return val;
        }

        private static string GetElementValue(IEnumerable<XElement> elements)
        {
            foreach (var sat in elements)
            {
                return sat.Value;
            }

            return "";
        }

        private static string CallWebservice(string body)
        {
            var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.TNT);
            try
            {
                //Call Express Connect
                using (var client = new HttpClient())
                {
                    //define dictionary to send body "xml_in=<..."
                    var values = new Dictionary<string, string>
                        {
                        {"xml_in", body}
                        };
                    //urlencode content
                    var content = new FormUrlEncodedContent(values);
                    //call webservice
                    var result = client.PostAsync(logisticIntegration.ServiceUrl, content).Result;
                    return result.Content.ReadAsStringAsync().Result;
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        public Bitmap CropImage(Bitmap source, Rectangle section)
        {
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(section.Width, section.Height);

            Graphics g = Graphics.FromImage(bmp);

            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

            return bmp;
        }

        public void CropImage(int Width, int Height, int LocationY, string sourceFilePath, string saveFilePath, int Count)
        {
            // variable for percentage resize 
            float percentageResize = 0;
            float percentageResizeW = 0;
            float percentageResizeH = 0;

            // variables for the dimension of source and cropped image  
            int destX = 0;
            int destY = 0;

            // Create a bitmap object file from source file 
            using (Bitmap sourceImage = new Bitmap(sourceFilePath))
            {
                // Set the source dimension to the variables 
                int sourceWidth = sourceImage.Width;
                int sourceHeight = sourceImage.Height;

                float HorizontalResolution = sourceImage.HorizontalResolution;
                float VerticalResolution = sourceImage.VerticalResolution;

                // Calculate the percentage resize 
                percentageResizeW = ((float)Width / (float)sourceWidth);
                percentageResizeH = ((float)Height / (float)sourceHeight);

                // Checking the resize percentage 
                if (percentageResizeH < percentageResizeW)
                {
                    percentageResize = percentageResizeW;
                    destY = System.Convert.ToInt16((Height - (sourceHeight * percentageResize)) / 2);
                }
                else
                {
                    percentageResize = percentageResizeH;
                    destX = System.Convert.ToInt16((Width - (sourceWidth * percentageResize)) / 2);
                }

                // Set the new cropped percentage image
                int destWidth = (int)Math.Round(sourceWidth * percentageResize);
                int destHeight = (int)Math.Round(sourceHeight * percentageResize);

                // Create the image object
                if (Count > 1)
                {
                    using (Bitmap objBitmap = new Bitmap(1580, 1075))
                    {
                        objBitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                        using (Graphics objGraphics = Graphics.FromImage(objBitmap))
                        {
                            // Set the graphic format for better result cropping 
                            objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                            if (destX < 0)
                                destX = 0;

                            objGraphics.DrawImage(sourceImage, 0, 0, new Rectangle(0, LocationY, 1585, 1180), GraphicsUnit.Pixel);
                            objBitmap.Save(saveFilePath, ImageFormat.Jpeg);
                        }
                    }

                    //Again rotate image
                    FlipTNTImage(saveFilePath, 0);
                }
                else
                {
                    using (Bitmap objBitmap = new Bitmap(1100, 1600))
                    {
                        objBitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                        using (Graphics objGraphics = Graphics.FromImage(objBitmap))
                        {
                            // Set the graphic format for better result cropping 
                            objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                            if (destX < 0)
                                destX = 0;
                            objGraphics.DrawImage(sourceImage, 0, 0, new Rectangle(48, 0, 1180, 1650), GraphicsUnit.Pixel);
                            objBitmap.Save(saveFilePath, ImageFormat.Jpeg);
                        }
                    }
                }
            }
        }

        public void FlipTNTImage(string ImagePath, int Count)
        {
            string resultPath = @"" + ImagePath + "";
            using (Image img = Image.FromFile(resultPath))
            {
                if (Count > 1)
                {

                }
                else
                {
                    //rotate the picture by 90 degrees and re-save the picture as a Jpeg
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    resultPath = @"" + ImagePath + "";
                    img.Save(resultPath, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }

        public string GenerateLabelHtml(TNTResponseDto TNTResponse)
        {
            string labelHtml = string.Empty;
            string dataSetHtml = string.Empty;
            StringBuilder sbXslOutput = new StringBuilder();
            using (XmlWriter xslWriter = XmlWriter.Create(sbXslOutput))
            {
                XslCompiledTransform transformer = new XslCompiledTransform();
                if (AppSettings.LabelSave == "")
                {
                    transformer.Load(AppSettings.WebApiPath + "/PackageLabel/" + "TNTLabel.xsl"); //
                }
                else
                {
                    if (AppSettings.ShipmentCreatedFrom == "BATCH")
                    {
                        transformer.Load(AppSettings.LabelFolder + "/TNTLabel.xsl"); //
                    }
                    else
                    {
                        transformer.Load(HttpContext.Current.Server.MapPath(AppSettings.LabelFolder) + "TNTLabel.xsl"); //
                    }
                }

                XsltArgumentList args = new XsltArgumentList();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(TNTResponse.TNTShipmentResponse.ConnoteReply);
                transformer.Transform(doc, args, xslWriter);
            }
            dataSetHtml = sbXslOutput.ToString();
            dataSetHtml = dataSetHtml.Replace("</table><br /><br /><DIV><FONT SIZE=\"1\" COLOR=\"#5FFFFF\">.</FONT></DIV><table", "</table><br/><table");
            labelHtml = dataSetHtml.Replace("</table><table", "</table><br/><table");
            return labelHtml;
        }

        public string GenerateLabelHtmlNew(TNTResponseDto TNTResponse)
        {
            string labelHtml = string.Empty;
            string dataSetHtml = string.Empty;
            StringWriter sbXslOutput = new StringWriter();

            using (XmlWriter xslWriter = XmlWriter.Create(sbXslOutput))
            {
                XslCompiledTransform transformer = new XslCompiledTransform();
                if (AppSettings.LabelSave == "")
                {
                    transformer.Load(AppSettings.WebApiPath + "/UploadFiles/XSLT/HTMLRoutingLabelRenderer.xsl");
                }
                else
                {
                    if (AppSettings.ShipmentCreatedFrom == "BATCH")
                    {
                        transformer.Load(AppSettings.WebApiPath + "/UploadFiles/XSLT/HTMLRoutingLabelRenderer.xsl");
                    }
                    else
                    {
                        transformer.Load(AppSettings.WebApiPath + "/UploadFiles/XSLT/HTMLRoutingLabelRenderer.xsl");
                    }
                }

                XsltArgumentList args = new XsltArgumentList();
                XmlDocument doc = new XmlDocument();
                //System.IO.StringWriter writer = new System.IO.StringWriter();

                doc.LoadXml(TNTResponse.TNTShipmentResponse.ConnoteReply);
                XmlReader xmlReadB = new XmlTextReader(new StringReader(doc.DocumentElement.OuterXml));
                transformer.Transform(xmlReadB, args, sbXslOutput);

                dataSetHtml = sbXslOutput.ToString();
            }

            dataSetHtml = dataSetHtml.Replace("/label-rendering-style.css", AppSettings.LabelVirtualPath + "/UploadFiles/XSLT/label-rendering-style.css");
            dataSetHtml = dataSetHtml.Replace("/label-rendering-style-fr.css", "https://express.tnt.com/expresswebservices-website/rendering/css/label-rendering-style-fr.css");
            dataSetHtml = dataSetHtml.Replace("/label-rendering-style-it.css", "https://express.tnt.com/expresswebservices-website/rendering/css/label-rendering-style-it.css");
            dataSetHtml = dataSetHtml.Replace("/label-rendering-exception-style.css", "https://express.tnt.com/expresswebservices-website/rendering/css/label-rendering-exception-style.css");
            dataSetHtml = dataSetHtml.Replace("/logo_orig.jpg", "https://express.tnt.com/expresswebservices-website/rendering/images/logo_orig.jpg");


            #region BarCode Update
            // From String
            var doc1 = new HtmlDocument();
            doc1.LoadHtml(dataSetHtml);

            HtmlNode htmlNode = doc1.DocumentNode.SelectNodes("//div[@id='barcodeLabel']").First();
            if (htmlNode != null)
            {
                var barcode = htmlNode.InnerText;

                string findAttribut = "img src=" + "\"" + barcode + "\"";

                dataSetHtml = dataSetHtml.Replace(findAttribut, "img src=" + "https://express.tnt.com/barbecue/barcode?type=Code128C&amp;height=140&amp;width=2&amp;data=" + barcode);


            }
            #endregion

            //labelHtml = dataSetHtml.Replace("</table><table", "</table><br/><table");
            return dataSetHtml;
        }

        public bool SaveTempShipmentXML(TNTResponseDto TNTResponse, int DirectShipmentid)
        {
            // after label is generated save tnt response files for reference 

            if (AppSettings.LabelSave == "")
            {
                if (!System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/"))
                    System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/");
                // shipment xml
                string path = AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "//" + "TNT_Shipment_" + TNTResponse.TNTShipmentResponse.TrackingCode + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xml";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
                {
                    file.Write(TNTResponse.TNTShipmentResponse.XmlIn);
                }
                // label xml
                string labelPath = AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "\\" + "TNT_Label_" + TNTResponse.TNTShipmentResponse.TrackingCode + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xml";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(labelPath))
                {
                    file.Write(TNTResponse.TNTShipmentResponse.ConnoteReply);
                }
                // result xml
                string resultPath = AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "//" + "TNT_Result_" + TNTResponse.TNTShipmentResponse.TrackingCode + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xml";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(resultPath))
                {
                    file.Write(TNTResponse.TNTShipmentResponse.ResultReply);
                }
                // delete teml tnt shipment xml
                if (File.Exists(TNTResponse.TNTShipmentResponse.shipmentXML))
                {
                    File.Delete(TNTResponse.TNTShipmentResponse.shipmentXML);
                }
            }
            else
            {
                if (AppSettings.ShipmentCreatedFrom == "BATCH")
                {
                    if (!System.IO.Directory.Exists(AppSettings.LabelFolder + DirectShipmentid + "/"))
                        System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + DirectShipmentid + "/");
                    // shipment xml
                    string path = AppSettings.LabelFolder + DirectShipmentid + "//" + "TNT_Shipment_" + TNTResponse.TNTShipmentResponse.TrackingCode + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xml";
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
                    {
                        file.Write(TNTResponse.TNTShipmentResponse.XmlIn);
                    }
                    // label xml
                    string labelPath = AppSettings.LabelFolder + DirectShipmentid + "\\" + "TNT_Label_" + TNTResponse.TNTShipmentResponse.TrackingCode + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xml";
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(labelPath))
                    {
                        file.Write(TNTResponse.TNTShipmentResponse.ConnoteReply);
                    }
                    // result xml
                    string resultPath = AppSettings.LabelFolder + DirectShipmentid + "//" + "TNT_Result_" + TNTResponse.TNTShipmentResponse.TrackingCode + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xml";
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(resultPath))
                    {
                        file.Write(TNTResponse.TNTShipmentResponse.ResultReply);
                    }
                    // delete teml tnt shipment xml
                    if (File.Exists(TNTResponse.TNTShipmentResponse.shipmentXML))
                    {
                        File.Delete(TNTResponse.TNTShipmentResponse.shipmentXML);
                    }
                }
                else
                {
                    if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(AppSettings.LabelFolder + DirectShipmentid + "/")))
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.LabelFolder + DirectShipmentid + "/"));
                    // shipment xml
                    string path = HttpContext.Current.Server.MapPath("~/PackageLabel/" + DirectShipmentid + "//" + "TNT_Shipment_" + TNTResponse.TNTShipmentResponse.TrackingCode + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xml");
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
                    {
                        file.Write(TNTResponse.TNTShipmentResponse.XmlIn);
                    }
                    // label xml
                    string labelPath = HttpContext.Current.Server.MapPath("~/PackageLabel/" + DirectShipmentid + "\\" + "TNT_Label_" + TNTResponse.TNTShipmentResponse.TrackingCode + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xml");
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(labelPath))
                    {
                        file.Write(TNTResponse.TNTShipmentResponse.ConnoteReply);
                    }
                    // result xml
                    string resultPath = HttpContext.Current.Server.MapPath("~/PackageLabel/" + DirectShipmentid + "//" + "TNT_Result_" + TNTResponse.TNTShipmentResponse.TrackingCode + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xml");
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(resultPath))
                    {
                        file.Write(TNTResponse.TNTShipmentResponse.ResultReply);
                    }
                    // delete teml tnt shipment xml
                    if (File.Exists(TNTResponse.TNTShipmentResponse.shipmentXML))
                    {
                        File.Delete(TNTResponse.TNTShipmentResponse.shipmentXML);
                    }
                }
            }

            return true;
        }

        public IntegrtaionResult MapIntegrationErrorResponse(TNTResponseDto TNTResponse)
        {
            IntegrtaionResult integrtaionResult = new IntegrtaionResult();
            if (TNTResponse.Error.Status == false)
            {
                integrtaionResult.Error = new FratyteError();
                {
                    integrtaionResult.Error = TNTResponse.Error;
                }
                integrtaionResult.ErrorCode = new List<FrayteApiError>();
                {
                    List<FrayteApiError> _api = new FrayteApiErrorCodeRepository().SaveFinilizeApiError(integrtaionResult.Error, FrayteLogisticServiceType.TNT, null);
                    integrtaionResult.ErrorCode = _api;
                }
                integrtaionResult.Status = false;
            }
            return integrtaionResult;
        }

        public IntegrtaionResult MapTNTIntegrationResponse(TNTResponseDto TNTResponse, List<FraytePackageTrackingDetail> PackageResults, string ShipmentIdentificationNumber)
        {
            IntegrtaionResult integrtaionResult = new IntegrtaionResult();
            if (TNTResponse.Error.Status)
            {
                integrtaionResult.Status = true;
                integrtaionResult.CourierName = FrayteCourierCompany.TNT;
                integrtaionResult.TrackingNumber = ShipmentIdentificationNumber;
                integrtaionResult.PieceTrackingDetails = new List<CourierPieceDetail>();
                foreach (var data in PackageResults)
                {
                    CourierPieceDetail obj = new CourierPieceDetail();
                    obj.DirectShipmentDetailId = 0;
                    obj.PieceTrackingNumber = data.TrackingNo;
                    obj.ImageByte = data.PackageImage;

                    integrtaionResult.PieceTrackingDetails.Add(obj);
                }
            }
            else
            {
                integrtaionResult.Error = new FratyteError();
                {
                    integrtaionResult.Error = TNTResponse.Error;
                }
                integrtaionResult.ErrorCode = new List<FrayteApiError>();
                {
                    List<FrayteApiError> _api = new FrayteApiErrorCodeRepository().SaveFinilizeApiError(integrtaionResult.Error, FrayteLogisticServiceType.TNT, null);
                    integrtaionResult.ErrorCode = _api;
                }
                integrtaionResult.Status = false;
            }
            return integrtaionResult;
        }

        public string DownloadTNTHtmlImage(string LabelHtml, string PieceTrackingNumber, int totalPiece, int count, int DirectShipmentid)
        {
            string Image = string.Empty;
            if (Image != null)
            {
                string labelName = string.Empty;
                labelName = FrayteShortName.TNT;

                // Create a file to write to.
                Image = labelName + "_" + PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".html";

                if (AppSettings.LabelSave == "")
                {
                    if (System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/"))
                    {
                        // labelimage.Save(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/" + Image);
                        File.WriteAllText(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/" + Image, LabelHtml);
                    }
                    else
                    {
                        // labelimage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/");
                        //labelimage.Save(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/" + Image);
                        File.WriteAllText(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/" + Image, LabelHtml);
                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(AppSettings.LabelFolder + "/" + DirectShipmentid + "/"))
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            // labelimage.Save(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image);
                            File.WriteAllText(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image, LabelHtml);
                        }
                        else
                        {
                            //labelimage.Save(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image));
                            string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image);
                            File.WriteAllText(path, LabelHtml);
                        }
                    }
                    else
                    {
                        // labelimage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "/" + DirectShipmentid);
                            // labelimage.Save(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image);
                            File.WriteAllText(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image, LabelHtml);
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid));
                            //labelimage.Save(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image));
                            File.WriteAllText(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image), LabelHtml);
                        }
                    }
                }
                //Step16.1: Create direcory for save package label
            }
            return Image;
        }
    }
}
