using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Models.ApiModal;
using Frayte.Services.Models.DPD;
using Frayte.Services.Models.Express;
using Frayte.Services.Models.SKYPOSTAL;
using Frayte.Services.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Web.Hosting;
using System.Web;
using DevExpress.Pdf;
using System.Text;

namespace Frayte.Services.Business
{
    public class SkyPostalRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        //public SkyPostalResponseModel CreateShipment(SkyPostalRequestModel skyPostalRequest, int DirectShipmentDraftId)
        //{
        //    SkyPostalResponseModel respone = new SkyPostalResponseModel();
        //    var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.SKYPOSTAL);
        //    string result = string.Empty;

        //    #region SKYPOSTaL API Login

        //    skyPostalRequest.Key = logisticIntegration.InetgrationKey;

        //    var skyPostalJson = Newtonsoft.Json.JsonConvert.SerializeObject(skyPostalRequest);

        //    result = SkyPostalWebApi(logisticIntegration, skyPostalJson);

        //    respone = Newtonsoft.Json.JsonConvert.DeserializeObject<SkyPostalResponseModel>(result);
        //    respone.Rawrequest = skyPostalJson;
        //    respone.Rawresponse = result;
        //    if (respone.success > 0)
        //    {
        //        return respone;
        //    }
        //    else
        //    {
        //        var j1 = JObject.Parse(result);

        //        var diff = j1["response"].ToList();

        //        respone.Error = new FratyteError();
        //        respone.Error.Service = new List<string>();
        //        string err = string.Empty;
        //        foreach (var error1 in diff)
        //        {
        //            err = error1.ToString();
        //            var err1 = err.Replace("{", "");
        //            var err2 = err1.Replace("}", "");

        //            respone.Error.Service.Add(err2);
        //        }

        //        respone.Error.Status = false;
        //        //Error Recorded
        //        SaveDirectShipmentObject(skyPostalJson, respone.Error.Service.ToString(), DirectShipmentDraftId);
        //        return respone;
        //    }
        //    #endregion            
        //}

        private void SaveDirectShipmentObject(string ShipmetObject, string ShipmetErrorObject, int ShipmentDraftId)
        {
            try
            {
                var detail = dbContext.DirectShipmentDrafts.Find(ShipmentDraftId);
                if (detail != null)
                {
                    detail.EasyPostOrderObject = ShipmetObject;
                    detail.EasyPostErrorObject = ShipmetErrorObject;
                    detail.LastUpdated = DateTime.UtcNow;
                    dbContext.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(ShipmetObject));
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        private string SkyPostalWebApi(FrayteLogisticIntegration frayteLogisticIntegration, string json)
        {
            try
            {
                string url = string.Empty;
                string response = string.Empty;

                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                client.UseDefaultCredentials = true;
                client.Credentials = CredentialCache.DefaultCredentials;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                ServicePointManager.SecurityProtocol = Tls12;
                url = frayteLogisticIntegration.ServiceUrl;
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                response = client.UploadString(url, "POST", json);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SkyPostalRequestModel MapDirectBookingDetailToShipmentRequestDto(DirectBookingShipmentDraftDetail directBookingDetail)
        {
            SkyPostalRequestModel skyPostalRequestModel = new SkyPostalRequestModel();
            skyPostalRequestModel.Key = "";
            skyPostalRequestModel.method = "insert_order";
            skyPostalRequestModel.include_label_data = "0";
            skyPostalRequestModel.include_label_zpl = "1";
            skyPostalRequestModel.zpl_encode_base64 = "1";
            skyPostalRequestModel.include_label_base64_image = "0";
            skyPostalRequestModel.label_pdf_rotate = "0";
            skyPostalRequestModel.header = new Header();
            skyPostalRequestModel.header.EXTR_TRACKING = directBookingDetail.FrayteNumber + "-" + directBookingDetail.ReferenceDetail.Reference1;
            skyPostalRequestModel.header.COUNTRY_CODE = directBookingDetail.ShipTo.Country.Code2;
            skyPostalRequestModel.header.STATE_CODE = 0;
            skyPostalRequestModel.header.CITY_CODE = 0;
            skyPostalRequestModel.header.STATE_NAME = directBookingDetail.ShipTo.State;
            skyPostalRequestModel.header.CITY_NAME = directBookingDetail.ShipTo.City;
            skyPostalRequestModel.header.FIRST_NAME = directBookingDetail.ShipTo.FirstName;
            skyPostalRequestModel.header.LAST_NAME = directBookingDetail.ShipTo.LastName;
            skyPostalRequestModel.header.ADDRESS_CONSIGNEE = directBookingDetail.ShipTo.Address;
            skyPostalRequestModel.header.ADDRESS2 = directBookingDetail.ShipTo.Address2;
            skyPostalRequestModel.header.ZIPCODE = directBookingDetail.ShipTo.PostCode;
            skyPostalRequestModel.header.PHONE = directBookingDetail.ShipTo.Phone;
            skyPostalRequestModel.header.MOBILE_PHONE = directBookingDetail.ShipTo.Phone;
            skyPostalRequestModel.header.EMAIL = directBookingDetail.ShipTo.Email;
            skyPostalRequestModel.header.ID_NUMBER = directBookingDetail.DirectShipmentDraftId.ToString();
            skyPostalRequestModel.header.MERCHANT_NAME = "FRAYTE LOGISTICS LTD";
            skyPostalRequestModel.header.MERCHANT_NUMBER = 665;
            skyPostalRequestModel.header.MERCHANT_BOX = directBookingDetail.CustomerRateCard.CourierAccountNo;
            if (directBookingDetail.CustomerRateCard.LogisticDescription == SKYPostalLogisticType.SkyPostalMexico)
            {
                skyPostalRequestModel.header.MERCHANT_CS_EMAIL = SKYPostalMerchantEmail.MexicoMerchantEmail;
            }
            if (directBookingDetail.CustomerRateCard.LogisticDescription == SKYPostalLogisticType.SkyPostalBrazil)
            {
                skyPostalRequestModel.header.MERCHANT_CS_EMAIL = SKYPostalMerchantEmail.BrazilMerchantEmail;
            }
            if (directBookingDetail.CustomerRateCard.LogisticDescription == SKYPostalLogisticType.SkyPostalChile)
            {
                skyPostalRequestModel.header.MERCHANT_CS_EMAIL = SKYPostalMerchantEmail.ChileMerchantEmail;
            }

            skyPostalRequestModel.header.MERCHANT_RETURN_ADDRESS = null;
            skyPostalRequestModel.header.MERCHANT_CS_NAME = "FRAYTE Logistics Ltd";
            skyPostalRequestModel.header.ORDER_NUMBER = directBookingDetail.ReferenceDetail.Reference1 + "-" + directBookingDetail.FrayteNumber;
            skyPostalRequestModel.header.ORDER_AMOUNT = directBookingDetail.Packages.Sum(k => k.Value);
            skyPostalRequestModel.header.ORDER_DATE = directBookingDetail.ReferenceDetail.CollectionDate.Value.ToString("dd/MM/yyyy");
            skyPostalRequestModel.header.INTERNAL_NUMBER = directBookingDetail.FrayteNumber;
            skyPostalRequestModel.header.MANIFEST_TYPE = "DDP";
            skyPostalRequestModel.header.CONSOLIDATED = 0;
            skyPostalRequestModel.header.CURRENCY_ISO_CODE = directBookingDetail.Currency.CurrencyCode;
            skyPostalRequestModel.header.SHIPMENT_FREIGHT = 0;
            skyPostalRequestModel.header.SHIPMENT_INSURANCE = 0;
            skyPostalRequestModel.header.SHIPMENT_DISCOUNT = 0;

            skyPostalRequestModel.detail = new List<PackageDetail>();

            foreach (var item in directBookingDetail.Packages)
            {

                PackageDetail package = new PackageDetail();

                package.HSC = "";
                package.FMPR_CDG = "";
                package.CONTENT_OF_PRODUCT = item.Content;
                package.PHYSICAL_WEIGHT = item.Weight;
                package.WEIGHT_TYPE = directBookingDetail.PakageCalculatonType == "kgToCms" ? "KG" : "LB";
                package.DIMEN_LENGTH = item.Length;
                package.DIMEN_HEIGHT = item.Height;
                package.DIMEN_WIDTH = item.Width;
                package.DIMEN_UNIT = directBookingDetail.PakageCalculatonType == "kgToCms" ? "CM" : "IN";
                package.MERCHANDISE_VALUE = item.Value;
                package.QUANTITY = item.CartoonValue;
                skyPostalRequestModel.detail.Add(package);
            }

            return skyPostalRequestModel;

        }

        //public SkyPostalRequestModel MapExpressBookingDetailToShipmentRequestDto(ExpressShipmentModel ExpressBookingDetail)
        //{
        //    SkyPostalRequestModel skyPostalRequestModel = new SkyPostalRequestModel();
        //    skyPostalRequestModel.Key = "";
        //    skyPostalRequestModel.method = "insert_order";
        //    skyPostalRequestModel.include_label_data = "0";
        //    skyPostalRequestModel.include_label_zpl = "1";
        //    skyPostalRequestModel.zpl_encode_base64 = "1";
        //    skyPostalRequestModel.include_label_base64_image = "0";
        //    skyPostalRequestModel.label_pdf_rotate = "0";
        //    skyPostalRequestModel.header = new Header();
        //    skyPostalRequestModel.header.EXTR_TRACKING = ExpressBookingDetail.AWBNumber.Replace(" ", "") + "-" + ExpressBookingDetail.ShipmentReference;
        //    skyPostalRequestModel.header.COUNTRY_CODE = ExpressBookingDetail.ShipTo.Country.Code2;
        //    skyPostalRequestModel.header.STATE_CODE = 0;
        //    skyPostalRequestModel.header.CITY_CODE = 0;
        //    skyPostalRequestModel.header.STATE_NAME = ExpressBookingDetail.ShipTo.State;
        //    skyPostalRequestModel.header.CITY_NAME = ExpressBookingDetail.ShipTo.City;
        //    skyPostalRequestModel.header.FIRST_NAME = ExpressBookingDetail.ShipTo.FirstName;
        //    skyPostalRequestModel.header.LAST_NAME = ExpressBookingDetail.ShipTo.LastName;
        //    skyPostalRequestModel.header.ADDRESS_CONSIGNEE = ExpressBookingDetail.ShipTo.Address;
        //    skyPostalRequestModel.header.ADDRESS2 = ExpressBookingDetail.ShipTo.Address2;
        //    skyPostalRequestModel.header.ZIPCODE = ExpressBookingDetail.ShipTo.PostCode;
        //    skyPostalRequestModel.header.PHONE = ExpressBookingDetail.ShipTo.Phone;
        //    skyPostalRequestModel.header.MOBILE_PHONE = ExpressBookingDetail.ShipTo.Phone;
        //    skyPostalRequestModel.header.EMAIL = ExpressBookingDetail.ShipTo.Email;
        //    skyPostalRequestModel.header.ID_NUMBER = ExpressBookingDetail.ExpressId.ToString();
        //    skyPostalRequestModel.header.MERCHANT_NAME = "FRAYTE LOGISTICS LTD";
        //    skyPostalRequestModel.header.MERCHANT_NUMBER = 665;
        //    skyPostalRequestModel.header.MERCHANT_BOX = ExpressBookingDetail.Service.CourierAccountNo;
        //    //skyPostalRequestModel.header.MERCHANT_CS_EMAIL = SKYPostalMerchantEmail.MexicoMerchantEmail;
        //    skyPostalRequestModel.header.MERCHANT_CS_EMAIL = "";

        //    skyPostalRequestModel.header.MERCHANT_RETURN_ADDRESS = null;
        //    skyPostalRequestModel.header.MERCHANT_CS_NAME = "FRAYTE Logistics Ltd";
        //    skyPostalRequestModel.header.ORDER_NUMBER = ExpressBookingDetail.ShipmentReference + "-" + ExpressBookingDetail.AWBNumber.Replace(" ", "");
        //    skyPostalRequestModel.header.ORDER_AMOUNT = ExpressBookingDetail.Packages.Sum(k => k.Value);
        //    skyPostalRequestModel.header.ORDER_DATE = DateTime.UtcNow.ToString("dd/MM/yyyy");
        //    skyPostalRequestModel.header.INTERNAL_NUMBER = ExpressBookingDetail.AWBNumber.Replace(" ", "");
        //    skyPostalRequestModel.header.MANIFEST_TYPE = "DDP";
        //    skyPostalRequestModel.header.CONSOLIDATED = 0;
        //    skyPostalRequestModel.header.CURRENCY_ISO_CODE = ExpressBookingDetail.DeclaredCurrency.CurrencyCode;
        //    skyPostalRequestModel.header.SHIPMENT_FREIGHT = 0;
        //    skyPostalRequestModel.header.SHIPMENT_INSURANCE = 0;
        //    skyPostalRequestModel.header.SHIPMENT_DISCOUNT = 0;

        //    skyPostalRequestModel.detail = new List<PackageDetail>();

        //    foreach (var item in ExpressBookingDetail.Packages)
        //    {

        //        PackageDetail package = new PackageDetail();

        //        package.HSC = "";
        //        package.FMPR_CDG = "";
        //        package.CONTENT_OF_PRODUCT = item.Content;
        //        package.PHYSICAL_WEIGHT = item.Weight;
        //        package.WEIGHT_TYPE = ExpressBookingDetail.PakageCalculatonType == "kgToCms" ? "KG" : "LB";
        //        package.DIMEN_LENGTH = item.Length;
        //        package.DIMEN_HEIGHT = item.Height;
        //        package.DIMEN_WIDTH = item.Width;
        //        package.DIMEN_UNIT = ExpressBookingDetail.PakageCalculatonType == "kgToCms" ? "CM" : "IN";
        //        package.MERCHANDISE_VALUE = item.Value;
        //        package.QUANTITY = item.CartonValue;
        //        skyPostalRequestModel.detail.Add(package);
        //    }

        //    return skyPostalRequestModel;
        //}

        //public IntegrtaionResult MapSkyPostalIntegrationResponse(SkyPostalResponseModel skyPostalResponse, List<PackageDetail> package)
        //{
        //    IntegrtaionResult integrtaionResult = new IntegrtaionResult();

        //    if (skyPostalResponse.Error == null)
        //    {
        //        integrtaionResult.Status = true;
        //        integrtaionResult.CourierName = FrayteCourierCompany.SKYPOSTAL;
        //        integrtaionResult.TrackingNumber = skyPostalResponse.response[0].TRCK_NMR_FOL;
        //        integrtaionResult.PickupRef = null;
        //        integrtaionResult.PieceTrackingDetails = new List<CourierPieceDetail>();
        //        for (int i = 0; i < package.Count(); i++)
        //        {
        //            for (int j = 0; j < package[i].QUANTITY; j++)
        //            {
        //                CourierPieceDetail obj = new CourierPieceDetail();
        //                obj.DirectShipmentDetailId = 0;
        //                obj.PieceTrackingNumber = string.Concat(skyPostalResponse.response[0].TRCK_NMR_FOL, j);
        //                obj.ImageUrl = skyPostalResponse.response[0].LABEL_URL_PDF;
        //                integrtaionResult.PieceTrackingDetails.Add(obj);
        //            }

        //        }
        //    }
        //    else
        //    {
        //        integrtaionResult.Error = skyPostalResponse.Error;
        //        integrtaionResult.ErrorCode = new List<FrayteApiError>();
        //        {
        //            List<FrayteApiError> _api = new FrayteApiErrorCodeRepository().SaveFinilizeApiError(integrtaionResult.Error, FrayteLogisticServiceType.DPD, null);
        //            integrtaionResult.ErrorCode = _api;
        //        }
        //        integrtaionResult.Status = false;
        //    }

        //    return integrtaionResult;
        //}

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

        public void MappingExpressCourierPieceDetail(IntegrtaionResult integrtaionResult, ExpressShipmentModel expessBookingDetail, int ExpressShipmentid)
        {
            if (ExpressShipmentid > 0)
            {
                int k = 0, i = 0;
                List<int> _shiId = new List<int>();
                foreach (var Obj in expessBookingDetail.Packages)
                {
                    _shiId = new DirectShipmentRepository().GetExpressDirectShipmentDetailID(ExpressShipmentid);
                    for (int j = 1; j <= Obj.CartonValue; j++)
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

        public string DownloadSkyPostalImage(CourierPieceDetail pieceDetails, int totalPiece, int count, int DirectShipmentid)
        {
            string Image = string.Empty;


            if (pieceDetails.ImageUrl != null)
            {

                string labelName = string.Empty;
                labelName = FrayteShortName.SKYPOSTAL;

                // Create a file to write to.



                Image = labelName + "_" + pieceDetails.PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".pdf";

                if (AppSettings.LabelSave == "")
                {
                    if (System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/"))
                    {

                        File.WriteAllText(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/" + Image, pieceDetails.ImageByte);
                    }
                    else
                    {

                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/");

                        File.WriteAllText(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/" + Image, pieceDetails.ImageByte);
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

                            File.WriteAllText(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image, pieceDetails.ImageByte);
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

        public string DownloadExpressSkyPostalImage(CourierPieceDetail pieceDetails, int totalPiece, int count, int ExpressShipmentid)
        {
            string Image = string.Empty;
            if (pieceDetails.ImageUrl != null)
            {
                string labelName = string.Empty;
                labelName = FrayteShortName.SKYPOSTAL;

                // Create a file to write to.
                Image = labelName + "_" + pieceDetails.PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".pdf";
                if (AppSettings.LabelSave == "")
                {
                    if (System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressShipmentid + "/"))
                    {
                        File.WriteAllText(AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressShipmentid + "/" + Image, pieceDetails.ImageByte);
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressShipmentid + "/");
                        File.WriteAllText(AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressShipmentid + "/" + Image, pieceDetails.ImageByte);
                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(AppSettings.LabelFolder + "/Express/" + ExpressShipmentid + "/"))
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            File.WriteAllText(AppSettings.LabelFolder + "/Express/" + ExpressShipmentid + "/" + Image, pieceDetails.ImageByte);
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
                            File.WriteAllText(AppSettings.LabelFolder + "/Express/" + ExpressShipmentid + "/" + Image, pieceDetails.ImageByte);
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + ExpressShipmentid));
                            string path = HttpContext.Current.Server.MapPath("~/PackageLabel/Express/" + ExpressShipmentid + "/" + Image);
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

                                string resultPath = @"" + path + "";

                                using (PdfDocumentProcessor pdfDocumentProcessor = new PdfDocumentProcessor())
                                {
                                    pdfDocumentProcessor.LoadDocument(resultPath);
                                    int angle = 0;
                                    foreach (PdfPage page in pdfDocumentProcessor.Document.Pages)
                                    {
                                        angle = (angle + 180) % 360;
                                        page.Rotate = angle;
                                    }
                                    pdfDocumentProcessor.SaveDocument(resultPath);
                                }

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

        public SkyPostalTrackingModel MappingTracking(string trackingNumber)
        {
            SkyPostalTrackingModel track = new SkyPostalTrackingModel()
            {
                copa_id = 665,
                extr_nmr = trackingNumber,
                Key = "ok",
                lang_cdg = "ENG",
                method = "search_tracking_info_by_external_tracking"
            };
            return track;

        }

        public SkyPostalTrackingResponseModel SkyPostalTrackWebApiCalling(SkyPostalTrackingModel trackModel)
        {
            SkyPostalTrackingResponseModel result = new SkyPostalTrackingResponseModel();
            string response = string.Empty;
            try
            {
                var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.SKYPOSTAL);

                var skyPostalJson = Newtonsoft.Json.JsonConvert.SerializeObject(trackModel);

                WebClient client = new WebClient();
                client.UseDefaultCredentials = true;
                client.Credentials = CredentialCache.DefaultCredentials;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                ServicePointManager.SecurityProtocol = Tls12;

                response = client.UploadString(logisticIntegration.ServiceUrl, "POST", skyPostalJson);
                if (!string.IsNullOrWhiteSpace(response))
                {
                    result = Newtonsoft.Json.JsonConvert.DeserializeObject<SkyPostalTrackingResponseModel>(response);
                }
                else
                {
                    result = null;
                }
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result;
        }

        // New API Integration

        public SkyPostalRequest MapExpressBookingDetailToShipmentRequestDto(ExpressShipmentModel ExpressBookingDetail)
        {
            SkyPostalRequest request = new SkyPostalRequest();

            request.user_info = new UserInfo();
            request.user_info.user_code = 441;
            request.user_info.user_key = "209ff2c85bc69dbdee8af67c244f8728";
            request.user_info.app_key = "0zqZl26OtlLK4oFeWh6QO890yN9ltP";

            request.shipment_info = new ShipmentInfo();

            request.shipment_info.copa_id = 665;
            request.shipment_info.ssa_copa_id = 0;
            request.shipment_info.box_id = ExpressBookingDetail.Service.CourierAccountNo;

            // Merchant info

            request.shipment_info.merchant = new Merchant();

            request.shipment_info.merchant.name = "FRAYTE LOGISTICS LTD";
            request.shipment_info.merchant.email = "";

            request.shipment_info.merchant.address = null;

            // Merchant Return Addrress
            request.shipment_info.merchant.return_address = null;

            // Merchant Phone
            request.shipment_info.merchant.phone = new List<MerchantPhone>();

            MerchantPhone mp = new MerchantPhone();
            mp.phone_type = 1;
            mp.phone_number = ExpressBookingDetail.ShipFrom.Phone;

            request.shipment_info.merchant.phone.Add(mp);


            //Shipper Info
            request.shipment_info.shipper = new Shipper();

            request.shipment_info.shipper.name = ExpressBookingDetail.ShipFrom.FirstName + " " + ExpressBookingDetail.ShipFrom.LastName;
            request.shipment_info.shipper.email = ExpressBookingDetail.ShipFrom.Email;

            request.shipment_info.shipper.shipperaddress = new ShipperAddress();

            request.shipment_info.shipper.shipperaddress.address_01 = ExpressBookingDetail.ShipFrom.Address;
            request.shipment_info.shipper.shipperaddress.address_02 = ExpressBookingDetail.ShipFrom.Address2;
            request.shipment_info.shipper.shipperaddress.address_03 = null;
            request.shipment_info.shipper.shipperaddress.city_code = 0;
            request.shipment_info.shipper.shipperaddress.city_name = ExpressBookingDetail.ShipFrom.City;
            request.shipment_info.shipper.shipperaddress.country_code = 0;
            request.shipment_info.shipper.shipperaddress.county_code = 0;
            request.shipment_info.shipper.shipperaddress.county_name = ExpressBookingDetail.ShipFrom.Country.Name;
            request.shipment_info.shipper.shipperaddress.state_code = 0;
            request.shipment_info.shipper.shipperaddress.zip_code = ExpressBookingDetail.ShipFrom.PostCode;
            request.shipment_info.shipper.shipperaddress.country_iso_code = ExpressBookingDetail.ShipFrom.Country.Code2;
            request.shipment_info.shipper.shipperaddress.country_name = ExpressBookingDetail.ShipFrom.Country.Name;
            request.shipment_info.shipper.shipperaddress.neighborhood = null;
            request.shipment_info.shipper.shipperaddress.state_name = ExpressBookingDetail.ShipFrom.State;

            // Shipper Return Address
            request.shipment_info.shipper.return_address = null;

            request.shipment_info.shipper.phone = new List<ShipperPhone>();

            ShipperPhone sp = new ShipperPhone();
            sp.phone_type = 1;
            sp.phone_number = ExpressBookingDetail.ShipFrom.Phone;

            request.shipment_info.shipper.phone.Add(sp);


            // Sender Address

            request.shipment_info.sender = new Sender();

            request.shipment_info.sender.name = ExpressBookingDetail.ShipFrom.FirstName + " " + ExpressBookingDetail.ShipFrom.LastName;
            request.shipment_info.sender.email = ExpressBookingDetail.ShipFrom.Email;

            request.shipment_info.sender.SenderAddress = new SenderAddress();

            request.shipment_info.sender.SenderAddress.address_01 = ExpressBookingDetail.ShipFrom.Address;
            request.shipment_info.sender.SenderAddress.address_02 = ExpressBookingDetail.ShipFrom.Address2;
            request.shipment_info.sender.SenderAddress.address_03 = null;
            request.shipment_info.sender.SenderAddress.city_code = 0;
            request.shipment_info.sender.SenderAddress.city_name = ExpressBookingDetail.ShipFrom.City;
            request.shipment_info.sender.SenderAddress.country_code = 0;
            request.shipment_info.sender.SenderAddress.county_code = 0;
            request.shipment_info.sender.SenderAddress.county_name = ExpressBookingDetail.ShipFrom.Country.Name;
            request.shipment_info.sender.SenderAddress.state_code = 0;
            request.shipment_info.sender.SenderAddress.zip_code = ExpressBookingDetail.ShipFrom.PostCode;
            request.shipment_info.sender.SenderAddress.country_iso_code = ExpressBookingDetail.ShipFrom.Country.Code2;
            request.shipment_info.sender.SenderAddress.country_name = ExpressBookingDetail.ShipFrom.Country.Name;
            request.shipment_info.sender.SenderAddress.neighborhood = null;
            request.shipment_info.sender.SenderAddress.state_name = ExpressBookingDetail.ShipFrom.State;


            // Shipper Return Address

            request.shipment_info.sender.return_address = new ReturnAddress();

            request.shipment_info.sender.return_address = null;


            request.shipment_info.sender.phone = new List<ShipperPhone>();

            ShipperPhone ph = new ShipperPhone();
            ph.phone_type = 1;
            ph.phone_number = ExpressBookingDetail.ShipFrom.Phone;

            request.shipment_info.sender.phone.Add(ph);

            //Consignee Address

            request.shipment_info.consignee = new Consignee();

            request.shipment_info.consignee.first_name = ExpressBookingDetail.ShipTo.FirstName;
            request.shipment_info.consignee.last_name = ExpressBookingDetail.ShipTo.LastName;
            request.shipment_info.consignee.email = ExpressBookingDetail.ShipTo.Email;
            request.shipment_info.consignee.id_number = ExpressBookingDetail.ExpressId.ToString();

            //Consignee Address
            request.shipment_info.consignee.address = new ConsigneeAddress();

            request.shipment_info.consignee.address.address_01 = ExpressBookingDetail.ShipTo.Address;
            request.shipment_info.consignee.address.address_02 = ExpressBookingDetail.ShipTo.Address2;
            request.shipment_info.consignee.address.address_03 = null;
            request.shipment_info.consignee.address.city_code = 0;
            request.shipment_info.consignee.address.city_name = ExpressBookingDetail.ShipTo.City;
            request.shipment_info.consignee.address.country_code = 0;
            request.shipment_info.consignee.address.country_iso_code = ExpressBookingDetail.ShipTo.Country.Code2;
            request.shipment_info.consignee.address.country_name = ExpressBookingDetail.ShipTo.Country.Name;
            request.shipment_info.consignee.address.county_code = 0;
            request.shipment_info.consignee.address.county_name = ExpressBookingDetail.ShipTo.Country.Name;
            request.shipment_info.consignee.address.neighborhood = null;
            request.shipment_info.consignee.address.state_code = 0;
            request.shipment_info.consignee.address.state_name = ExpressBookingDetail.ShipTo.State;
            request.shipment_info.consignee.address.zip_code = ExpressBookingDetail.ShipTo.PostCode;

            //Consignee Phone
            request.shipment_info.consignee.phone = new List<ConsigneePhone>();

            ConsigneePhone phn = new ConsigneePhone();

            phn.phone_type = 1;
            phn.phone_number = ExpressBookingDetail.ShipTo.Phone;

            request.shipment_info.consignee.phone.Add(phn);


            request.shipment_info.options = new Options();

            request.shipment_info.options.generate_label_default = false;
            request.shipment_info.options.include_label_data = false;
            request.shipment_info.options.include_label_image = false;
            request.shipment_info.options.insurance_code = 0;
            request.shipment_info.options.manifest_type = "DDP";
            request.shipment_info.options.rate_service_code = 0;
            request.shipment_info.options.zpl_encode_base64 = true;
            request.shipment_info.options.include_label_zpl = false;

            request.shipment_info.data = new Data();

            request.shipment_info.data.currency_iso_code = ExpressBookingDetail.DeclaredCurrency.CurrencyCode;
            request.shipment_info.data.dimension_01 = ExpressBookingDetail.Packages[0].Length;
            request.shipment_info.data.dimension_02 = ExpressBookingDetail.Packages[0].Height;
            request.shipment_info.data.dimension_03 = ExpressBookingDetail.Packages[0].Width;
            request.shipment_info.data.dimension_unit = ExpressBookingDetail.PakageCalculatonType == "kgToCms" ? "CM" : "IN";
            request.shipment_info.data.discount = 0;

            request.shipment_info.data.external_tracking = ExpressBookingDetail.AWBNumber.Replace(" ", "") + "-" + ExpressBookingDetail.ShipmentReference;
            request.shipment_info.data.freight = 0;
            request.shipment_info.data.insurance = 0;
            request.shipment_info.data.reference_date = DateTime.UtcNow.ToString("yyyy-MM-dd");
            request.shipment_info.data.reference_number_01 = "";
            request.shipment_info.data.reference_number_02 = "";
            request.shipment_info.data.reference_number_03 = "";
            request.shipment_info.data.tax = 0;
            request.shipment_info.data.weight = ExpressBookingDetail.Service.ActualWeight;
            request.shipment_info.data.weight_unit = ExpressBookingDetail.PakageCalculatonType == "kgToCms" ? "KG" : "LB";
            request.shipment_info.data.value = ExpressBookingDetail.DeclaredValue;

            request.shipment_info.data.items = new List<Item>();

            Item my = new Item();

            my.description = ExpressBookingDetail.Packages[0].Content;
            my.family_product = "";
            my.hs_code = "";
            my.imei_number = "";
            my.quantity = ExpressBookingDetail.Packages[0].CartonValue;
            my.serial_number = "";
            my.tax = 0;
            my.value = ExpressBookingDetail.Packages[0].Value;
            my.weight = ExpressBookingDetail.Packages[0].Weight;

            request.shipment_info.data.items.Add(my);

            return request;
        }

        public SkyPostalResponse CreateShipment(SkyPostalRequest skyPostalRequest, int ExpressId)
        {
            SkyPostalResponse respone = new SkyPostalResponse();
            var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.SKYPOSTAL);
            string result = string.Empty;

            #region SKYPOSTaL API Login

            skyPostalRequest.user_info.user_code = Convert.ToInt32(logisticIntegration.AppId);

            skyPostalRequest.user_info.app_key = logisticIntegration.InetgrationKey;

            skyPostalRequest.user_info.user_key = logisticIntegration.UserName;

            var skyPostalJson = Newtonsoft.Json.JsonConvert.SerializeObject(skyPostalRequest);

            result = SkyPostalWebApi(logisticIntegration, skyPostalJson);

            try
            {
                respone = Newtonsoft.Json.JsonConvert.DeserializeObject<SkyPostalResponse>(result);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return respone;
        }

        public IntegrtaionResult MapSkyPostalIntegrationResponse(SkyPostalResponse response)
        {
            IntegrtaionResult integrtaionResult = new IntegrtaionResult();

            if (response.data[0].label_tracking_number_01 != null)
            {
                integrtaionResult.Status = true;
                integrtaionResult.CourierName = FrayteCourierCompany.SKYPOSTAL;
                integrtaionResult.TrackingNumber = response.data[0].label_tracking_number_01;
                integrtaionResult.PickupRef = null;
                integrtaionResult.PieceTrackingDetails = new List<CourierPieceDetail>();

                CourierPieceDetail obj = new CourierPieceDetail();
                obj.DirectShipmentDetailId = 0;
                obj.PieceTrackingNumber = response.data[0].label_tracking_number_01;
                obj.ImageUrl = response.data[0].label_url_pdf;
                integrtaionResult.PieceTrackingDetails.Add(obj);
                return integrtaionResult;
            }
            else
            {
                integrtaionResult.Status = false;
                integrtaionResult.Error = new FratyteError();
                integrtaionResult.Error.IsMailSend = true;
                integrtaionResult.Error.Custom = new List<string>();
                integrtaionResult.Error.Address = new List<string>();
                integrtaionResult.Error.Package = new List<string>();
                integrtaionResult.Error.Service = new List<string>();
                integrtaionResult.Error.ServiceError = new List<string>();
                integrtaionResult.Error.Miscellaneous = new List<string>();

                for (int i = 0; i < response.data.Count; i++)
                {
                    for (int j = 0; j < response.data[i].error.Count; j++)
                    {
                        integrtaionResult.Error.Miscellaneous.Add(response.data[i].error[j].error_description);
                    }
                }
            }
            return integrtaionResult;
        }
    }

    #endregion
}




