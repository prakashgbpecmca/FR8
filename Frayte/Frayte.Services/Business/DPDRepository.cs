using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Models.ApiModal;
using Frayte.Services.Models.DPD;
using Frayte.Services.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Web.Hosting;
using Frayte.Services.Models.Express;

namespace Frayte.Services.Business
{
    public class DPDRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public DpdShipmentResponse CreateShipment(DPDRequestModel dpdRequestObj)
        {
            DpdShipmentResponse respone = new DpdShipmentResponse();
            var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.DPD);
            string result = string.Empty;

            var abc = Newtonsoft.Json.JsonConvert.SerializeObject(dpdRequestObj);

            #region DPD API Login

            string usernamePasswordBase64String = Base64Encode(logisticIntegration.UserName + ":" + logisticIntegration.Password);
            //Api Login
            string loginRespone = LogindpdWebApi(logisticIntegration, usernamePasswordBase64String);
            var DPDLogin = Newtonsoft.Json.JsonConvert.DeserializeObject<DPDLoginResponeModel>(loginRespone);
            if (DPDLogin.error == null)
            {

                var shipmentRequestjson = JsonConvert.SerializeObject(dpdRequestObj);
                //Insert Shipment
                string shipmentResponsejson = CallDpdWebApi(logisticIntegration, shipmentRequestjson, DPDLogin.data.geoSession);

                respone.DPDResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<DPDResponseModel>(shipmentResponsejson);
                if (respone.DPDResponse.error == null)
                {
                    //Get Label
                    respone.LabelHTMLString = CallDpdLabelApi(logisticIntegration, respone.DPDResponse.data.shipmentId, DPDLogin.data.geoSession, "text/html");
                    respone.LabeleplString = CallDpdLabelApi(logisticIntegration, respone.DPDResponse.data.shipmentId, DPDLogin.data.geoSession, "text/vnd.eltron-epl");
                    //Collection Pickup

                    return respone;
                }
                else
                {
                    respone.Error = new FratyteError();
                    respone.Error.Service = new List<string>();
                    string err = string.Empty;
                    foreach (var error in respone.DPDResponse.error)
                    {
                        err += error.Obj + " - " + error.errorMessage + ",";

                    }

                    respone.Error.Service.Add(err);

                    respone.Error.Status = false;
                    //Error Recorded
                    SaveDirectShipmentObject(shipmentRequestjson, respone.DPDResponse.error.ToString(), dpdRequestObj.DraftShipmentId);
                }

            }
            else
            {
                respone.Error = new FratyteError();
                respone.Error.Service = new List<string>();
                string err = string.Empty;
                foreach (var error in respone.DPDResponse.error)
                {
                    err += error.Obj + " - " + error.errorMessage + ",";

                }
                respone.Error.Service.Add(err);

                respone.Error.Status = false;
                //Error Recorded
                SaveDirectShipmentObject(usernamePasswordBase64String, respone.DPDResponse.error.ToString(), dpdRequestObj.DraftShipmentId);
            }
            #endregion

            return respone;


        }

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

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private string LogindpdWebApi(FrayteLogisticIntegration frayteLogisticIntegration, string json)
        {
            try
            {
                string url = string.Empty;
                string response = string.Empty;

                WebClient client = new WebClient();
                client.UseDefaultCredentials = true;
                client.Credentials = CredentialCache.DefaultCredentials;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                ServicePointManager.SecurityProtocol = Tls12;

                url = frayteLogisticIntegration.ServiceUrl + "/user/?action=login";

                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                client.Headers[HttpRequestHeader.Authorization] = "Basic" + " " + json;

                response = client.UploadString(url, "POST", "");
                return response;
            }
            catch (Exception ex)
            {
                throw;

            }

        }

        private string CallDpdWebApi(FrayteLogisticIntegration frayteLogisticIntegration, string shipmentRequestjson, string geosession)
        {
            try
            {
                string url = string.Empty;
                string response = string.Empty;

                WebClient client = new WebClient();
                client.UseDefaultCredentials = true;
                client.Credentials = CredentialCache.DefaultCredentials;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                ServicePointManager.SecurityProtocol = Tls12;

                url = frayteLogisticIntegration.ServiceUrl + "/shipping/shipment";

                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                client.Headers["geoclient"] = "account/380995";
                client.Headers["geosession"] = geosession;

                response = client.UploadString(url, "POST", shipmentRequestjson);


                return response;
            }
            catch (Exception ex)
            {
                throw ex;

            }

        }

        private string CallDpdLabelApi(FrayteLogisticIntegration frayteLogisticIntegration, string Shipmentid, string geosession, string format)
        {
            try
            {



                string url = string.Empty;
                string response = string.Empty;

                WebClient client = new WebClient();
                client.UseDefaultCredentials = true;
                client.Credentials = CredentialCache.DefaultCredentials;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                ServicePointManager.SecurityProtocol = Tls12;

                url = frayteLogisticIntegration.ServiceUrl + "/shipping/shipment/" + Shipmentid + "/label/";


                client.Headers[HttpRequestHeader.Accept] = format;
                client.Headers["geoclient"] = "account/380995";
                client.Headers["geosession"] = geosession;

                response = client.DownloadString(url);


                return response;
            }
            catch (Exception ex)
            {
                throw ex;

            }

        }

        public DPDRequestModel MapDirectBookingDetailToShipmentRequestDto(DirectBookingShipmentDraftDetail directBookingDetail)
        {
            try
            {

                //ShipFromAddress
                var collectionDetails = new CollectionDetails();
                mapCallectionAddressDetail(directBookingDetail, collectionDetails);

                //ShipToAddress
                var deliveryDetails = new DeliveryDetails();
                mapDeliveryAddressDetail(directBookingDetail, deliveryDetails);


                DPDRequestModel dpdShipmentRequest = new DPDRequestModel();
                dpdShipmentRequest.DraftShipmentId = directBookingDetail.DirectShipmentDraftId;
                dpdShipmentRequest.collectionDate = directBookingDetail.ReferenceDetail.CollectionDate.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                dpdShipmentRequest.collectionOnDelivery = false;
                dpdShipmentRequest.invoice = null;
                dpdShipmentRequest.consolidate = false;
                dpdShipmentRequest.job_id = null;
                dpdShipmentRequest.consignment = new List<Consignment>();
                int TotalNumberParcel = 0;
                foreach (var item in directBookingDetail.Packages)
                {
                    TotalNumberParcel += item.CartoonValue;
                }
                dpdShipmentRequest.consignment.Add(new Consignment
                {
                    consignmentNumber = null,
                    consignmentRef = null,
                    parcels = new List<Parcels>(),
                    collectionDetails = collectionDetails,
                    deliveryDetails = deliveryDetails,
                    networkCode = directBookingDetail.CustomerRateCard.NetworkCode,
                    numberOfParcels = TotalNumberParcel,
                    totalWeight = directBookingDetail.CustomerRateCard.Weight,
                    customsValue = null,
                    deliveryInstructions = "Please deliver with shipment owner",
                    liability = false,
                    liabilityValue = null,
                    parcelDescription = string.Join("-", directBookingDetail.Packages.Select(x => x.Content.ToString()).ToArray()),
                    shippingRef1 = directBookingDetail.FrayteNumber + "-" + directBookingDetail.ReferenceDetail.Reference1,
                    shippingRef2 = directBookingDetail.ReferenceDetail.Reference2,
                    shippingRef3 = "",


                });


                return dpdShipmentRequest;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Shipment is not created " + ex.Message));
                return null;
            }
        }
        
        private void mapCallectionAddressDetail(DirectBookingShipmentDraftDetail directBookingDetail, CollectionDetails collectionDetails)
        {
            collectionDetails.contactDetails = new ContactDetails()
            {
                contactName = directBookingDetail.ShipFrom.FirstName + " " + directBookingDetail.ShipFrom.LastName,
                telephone = directBookingDetail.ShipFrom.Phone
            };
            collectionDetails.address = new Address()
            {

                organisation = directBookingDetail.ShipFrom.CompanyName,
                street = directBookingDetail.ShipFrom.Address,
                town = directBookingDetail.ShipFrom.City,
                locality = directBookingDetail.ShipFrom.Address2,
                countryCode = directBookingDetail.ShipFrom.Country.Code2,
                county = directBookingDetail.ShipFrom.Country.Name,
                postcode = directBookingDetail.ShipFrom.PostCode,
            };
            if (collectionDetails.address.street.Length > 35)
            {
                collectionDetails.address.street = collectionDetails.address.street.Substring(0, 35);
            }
            if (collectionDetails.address.locality.Length > 35)
            {
                collectionDetails.address.locality = collectionDetails.address.locality.Substring(0, 35);
            }
        }

        private void mapDeliveryAddressDetail(DirectBookingShipmentDraftDetail directBookingDetail, DeliveryDetails deliveryDetail)
        {
            deliveryDetail.contactDetails = new ContactDetails()
            {
                contactName = directBookingDetail.ShipTo.FirstName + " " + directBookingDetail.ShipTo.LastName,
                telephone = directBookingDetail.ShipTo.Phone
            };
            deliveryDetail.address = new Address()
            {
                organisation = directBookingDetail.ShipTo.CompanyName,
                street = directBookingDetail.ShipTo.Address,
                town = directBookingDetail.ShipTo.City,
                locality = directBookingDetail.ShipTo.Address2,
                countryCode = directBookingDetail.ShipTo.Country.Code2,
                county = directBookingDetail.ShipTo.Country.Name,
                postcode = directBookingDetail.ShipTo.PostCode,
            };
            deliveryDetail.notificationDetails = new NotificationDetails()
            {
                email = directBookingDetail.ShipTo.Email,
                mobile = directBookingDetail.ShipTo.Phone,
            };

            if (deliveryDetail.address.street.Length > 35)
            {
                deliveryDetail.address.street = deliveryDetail.address.street.Substring(0, 35);
            }
            if (deliveryDetail.address.locality.Length > 35)
            {
                deliveryDetail.address.locality = deliveryDetail.address.locality.Substring(0, 35);
            }
        }

        public IntegrtaionResult MapDPDIntegrationResponse(DpdShipmentResponse dpdShipmentResponse)
        {
            IntegrtaionResult integrtaionResult = new IntegrtaionResult();

            if (dpdShipmentResponse.Error == null)
            {
                integrtaionResult.Status = true;
                integrtaionResult.CourierName = FrayteCourierCompany.DPD;
                integrtaionResult.TrackingNumber = dpdShipmentResponse.DPDResponse.data.consignmentDetail[0].consignmentNumber;
                integrtaionResult.PickupRef = null;
                integrtaionResult.PieceTrackingDetails = new List<CourierPieceDetail>();
                foreach (var data in dpdShipmentResponse.DPDResponse.data.consignmentDetail[0].parcelNumbers)
                {
                    CourierPieceDetail obj = new CourierPieceDetail();
                    obj.DirectShipmentDetailId = 0;
                    obj.PieceTrackingNumber = data;
                    obj.ImageByte = dpdShipmentResponse.LabelHTMLString;
                    integrtaionResult.PieceTrackingDetails.Add(obj);
                }
            }
            else
            {
                integrtaionResult.Error = dpdShipmentResponse.Error;
                integrtaionResult.ErrorCode = new List<FrayteApiError>();
                {
                    List<FrayteApiError> _api = new FrayteApiErrorCodeRepository().SaveFinilizeApiError(integrtaionResult.Error, FrayteLogisticServiceType.DPD, null);
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

        public string DownloadDPDHtmlImage(CourierPieceDetail pieceDetails, int totalPiece, int count, int DirectShipmentid, DirectBookingService Carrier)
        {
            string Image = string.Empty;
            if (Image != null)
            {
                string labelName = string.Empty;
                labelName = Carrier.DisplayName;

                // Create a file to write to.
                Image = labelName + "_" + pieceDetails.PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".html";

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
                            File.WriteAllText(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image), pieceDetails.ImageByte);
                        }
                    }
                }
            }
            return Image;
        }
        
        #region Express

        public DPDRequestModel MapExpressBookingDetailToShipmentRequestDto(ExpressShipmentModel shipment)
        {
            try
            {

                //ShipFromAddress
                var collectionDetails = new CollectionDetails();
                mapCallectionAddressDetail(shipment, collectionDetails);

                //ShipToAddress
                var deliveryDetails = new DeliveryDetails();
                mapDeliveryAddressDetail(shipment, deliveryDetails);


                DPDRequestModel dpdShipmentRequest = new DPDRequestModel();
                dpdShipmentRequest.DraftShipmentId = shipment.ExpressId;
                dpdShipmentRequest.collectionDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
                dpdShipmentRequest.collectionOnDelivery = false;
                dpdShipmentRequest.invoice = null;
                dpdShipmentRequest.consolidate = false;
                dpdShipmentRequest.job_id = null;
                dpdShipmentRequest.consignment = new List<Consignment>();
                int TotalNumberParcel = 0;
                foreach (var item in shipment.Packages)
                {
                    TotalNumberParcel += item.CartonValue;
                }
                dpdShipmentRequest.consignment.Add(new Consignment
                {
                    consignmentNumber = null,
                    consignmentRef = null,
                    parcels = new List<Parcels>(),
                    collectionDetails = collectionDetails,
                    deliveryDetails = deliveryDetails,
                    networkCode = shipment.Service.NetworkCode,
                    numberOfParcels = TotalNumberParcel,
                    totalWeight = shipment.Service.ActualWeight,
                    customsValue = null,
                    deliveryInstructions = "Please deliver with shipment owner",
                    liability = false,
                    liabilityValue = null,
                    parcelDescription = string.Join("-", shipment.Packages.Select(x => x.Content.ToString()).ToArray()),
                    shippingRef1 = shipment.FrayteNumber + "-" + shipment.ShipmentReference,
                    shippingRef2 = "",
                    shippingRef3 = "",


                });


                return dpdShipmentRequest;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Shipment is not created " + ex.Message));
                return null;
            }
        }

        public string ExpressDownloadDPDHtmlImage(CourierPieceDetail pieceDetails, int totalPiece, int count, int DirectShipmentid, string Carrier)
        {
            string Image = string.Empty;
            if (Image != null)
            {
                string labelName = string.Empty;
                labelName = Carrier;

                // Create a file to write to.
                Image = labelName + "_" + pieceDetails.PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".html";

                if (AppSettings.LabelSave == "")
                {
                    if (System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/Express/" + DirectShipmentid + "/"))
                    {
                        File.WriteAllText(AppSettings.WebApiPath + "/PackageLabel/Express/" + DirectShipmentid + "/" + Image, pieceDetails.ImageByte);
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/Express/" + DirectShipmentid + "/");
                        File.WriteAllText(AppSettings.WebApiPath + "/PackageLabel/Express/" + DirectShipmentid + "/" + Image, pieceDetails.ImageByte);
                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(AppSettings.LabelFolder + "/Express/" + DirectShipmentid + "/"))
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            File.WriteAllText(AppSettings.LabelFolder + "/Express/" + DirectShipmentid + "/" + Image, pieceDetails.ImageByte);
                        }
                        else
                        {
                            string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + DirectShipmentid + "/" + Image);
                            File.WriteAllText(path, pieceDetails.ImageByte);
                        }
                    }
                    else
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "/Express/" + DirectShipmentid);
                            File.WriteAllText(AppSettings.LabelFolder + "/Express/" + DirectShipmentid + "/" + Image, pieceDetails.ImageByte);
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + DirectShipmentid));
                            File.WriteAllText(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + DirectShipmentid + "/" + Image), pieceDetails.ImageByte);
                        }
                    }
                }
            }
            return Image;
        }

        private void mapDeliveryAddressDetail(ExpressShipmentModel shipment, DeliveryDetails deliveryDetail)
        {
            deliveryDetail.contactDetails = new ContactDetails()
            {
                contactName = shipment.ShipTo.FirstName + " " + shipment.ShipTo.LastName,
                telephone = shipment.ShipTo.Phone
            };
            deliveryDetail.address = new Address()
            {
                organisation = shipment.ShipTo.CompanyName,
                street = shipment.ShipTo.Address,
                town = shipment.ShipTo.City,
                locality = shipment.ShipTo.Address2,
                countryCode = shipment.ShipTo.Country.Code2,
                county = shipment.ShipTo.Country.Name,
                postcode = shipment.ShipTo.PostCode,
            };
            deliveryDetail.notificationDetails = new NotificationDetails()
            {
                email = shipment.ShipTo.Email,
                mobile = shipment.ShipTo.Phone,
            };

            if (deliveryDetail.address.street.Length > 35)
            {
                deliveryDetail.address.street = deliveryDetail.address.street.Substring(0, 35);
            }
            if (deliveryDetail.address.locality.Length > 35)
            {
                deliveryDetail.address.locality = deliveryDetail.address.locality.Substring(0, 35);
            }
        }

        private void mapCallectionAddressDetail(ExpressShipmentModel shipmment, CollectionDetails collectionDetails)
        {
            collectionDetails.contactDetails = new ContactDetails()
            {
                contactName = shipmment.ShipFrom.FirstName + " " + shipmment.ShipFrom.LastName,
                telephone = shipmment.ShipFrom.Phone
            };
            collectionDetails.address = new Address()
            {

                organisation = shipmment.ShipFrom.CompanyName,
                street = shipmment.ShipFrom.Address,
                town = shipmment.ShipFrom.City,
                locality = shipmment.ShipFrom.Address2,
                countryCode = shipmment.ShipFrom.Country.Code2,
                county = shipmment.ShipFrom.Country.Name,
                postcode = shipmment.ShipFrom.PostCode,
            };
            if (collectionDetails.address.street.Length > 35)
            {
                collectionDetails.address.street = collectionDetails.address.street.Substring(0, 35);
            }
            if (collectionDetails.address.locality.Length > 35)
            {
                collectionDetails.address.locality = collectionDetails.address.locality.Substring(0, 35);
            }
        }
        #endregion
    }
}
