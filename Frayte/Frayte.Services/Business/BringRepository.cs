using Frayte.Services.Models;
using Frayte.Services.Models.Bring;
using Frayte.Services.Models.Express;
using Frayte.Services.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Frayte.Services.Business
{
    public class BringRepository
    {
        public BringResponseModel CreateShipment(BringRequestModel bringRequest, int DraftShipmentId, string ShipmentType)
        {
            BringResponseModel response = new BringResponseModel();
            var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.BRING);
            //API Login
            var shipmentRequestjson = JsonConvert.SerializeObject(bringRequest);
            string result = string.Empty;
            try
            {
                result = CallBringApi(logisticIntegration, shipmentRequestjson);
                
                if (!string.IsNullOrWhiteSpace(result))
                {
                    response = Newtonsoft.Json.JsonConvert.DeserializeObject<BringResponseModel>(result);
                }
                else
                {
                    var error = new Models.Bring.Error();
                    error.code = "There is something is error please contact to admin";
                    response.errors = new List<Models.Bring.Error>();
                    response.errors.Add(error);
                    if (ShipmentType == FrayteShipmentServiceType.DirectBooking)
                    {
                        new DirectShipmentRepository().SaveEasyPostErrorObject("Bring-result:-" + @result, "Bring-ShipJSON:-" + shipmentRequestjson, DraftShipmentId);
                    }
                }
                response.request = shipmentRequestjson;
                response.response = result;
            }
            catch (Exception ex)
            {
                var error = new Models.Bring.Error();
                error.code = ex.InnerException.ToString();
                response.errors.Add(error);
                if (ShipmentType == FrayteShipmentServiceType.DirectBooking)
                {
                    new DirectShipmentRepository().SaveEasyPostErrorObject("Bring-result:-" + @result, "Bring-ShipJSON:-" + shipmentRequestjson, DraftShipmentId);
                }
            }
            return response;
        }

        public BringRequestModel MapDirectBookingDetailToShipmentRequestDto(DirectBookingShipmentDraftDetail directBookingDetail)
        {
            BringRequestModel bringRequest = new BringRequestModel();
            if (AppSettings.ApplicationMode == FrayteApplicationMode.Test)
            {
                bringRequest.testIndicator = true;
            }
            else
            {
                bringRequest.testIndicator = false;
            }

            bringRequest.schemaVersion = 1;
            bringRequest.consignments = new List<ConsignmentModel>();
            var packages = new List<Models.Bring.Package>();

            Models.Bring.Package package = new Models.Bring.Package();
            for (int i = 0; i < directBookingDetail.Packages.Count; i++)
            {
                for (int j = 0; j < directBookingDetail.Packages[i].CartoonValue; j++)
                {
                    package.weightInKg = directBookingDetail.Packages[i].Weight.ToString("0.##");
                    package.goodsDescription = directBookingDetail.Packages[i].Content;
                    package.dimensions = new Dimensions()
                    {
                        heightInCm = directBookingDetail.Packages[i].Height,
                        lengthInCm = directBookingDetail.Packages[i].Length,
                        widthInCm = directBookingDetail.Packages[i].Width,
                    };
                    package.containerId = directBookingDetail.FrayteNumber;
                    package.packageType = "";
                    package.numberOfItems = "";
                    package.correlationId = "PACKAGE-" + directBookingDetail.FrayteNumber;
                    packages.Add(package);
                }
            }

            var ShipFrom = new ExpressRepository().getHubAddress(directBookingDetail.ShipTo.Country.CountryId, directBookingDetail.ShipTo.PostCode, directBookingDetail.ShipTo.State);
            ConsignmentModel consign = new ConsignmentModel()
            {
                shippingDateTime = DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss"),

                parties = new Parties()
                {
                    sender = new Sender()
                    {
                        //name = string.IsNullOrWhiteSpace(ShipFrom.CompanyName) ? ShipFrom.FirstName + " " + ShipFrom.LastName : ShipFrom.CompanyName,
                        //addressLine = ShipFrom.Address,
                        //addressLine2 = ShipFrom.Address2,
                        //additionalAddressInfo = "",
                        //postalCode = ShipFrom.PostCode,
                        //city = ShipFrom.City,
                        //countryCode = ShipFrom.Country.Code2,
                        //reference = directBookingDetail.FrayteNumber + "-" + directBookingDetail.ReferenceDetail.Reference1,
                        //contact = new Contact
                        //{
                        //    name = string.IsNullOrEmpty(ShipFrom.FirstName + " " + ShipFrom.LastName) ? ShipFrom.CompanyName : ShipFrom.FirstName + " " + ShipFrom.LastName,
                        //    email = string.IsNullOrEmpty(ShipFrom.Email) ? "technicaltest@frayte.com" : ShipFrom.Email,
                        //    phoneNumber = ShipFrom.Phone
                        //}

                        name = "Priority Cargo AS",
                        addressLine = "Skur 97,Kongshavnveien 29,",
                        addressLine2 = "Kongshavnveien 29,",
                        additionalAddressInfo = "",
                        postalCode = "0193",
                        city = "Oslo",
                        countryCode = "NO",
                        reference = directBookingDetail.FrayteNumber + "-" + directBookingDetail.ReferenceDetail.Reference1,
                        contact = new Contact
                        {
                            name = "Priority Cargo AS",
                            email = "technicaltest@frayte.com",
                            phoneNumber = "+47 9760 0402"
                        }
                    },
                    recipient = new Recipient()
                    {
                        name = string.IsNullOrWhiteSpace(directBookingDetail.ShipTo.CompanyName) ? directBookingDetail.ShipTo.FirstName + " " + directBookingDetail.ShipTo.LastName : directBookingDetail.ShipTo.CompanyName,
                        addressLine = directBookingDetail.ShipTo.Address,
                        addressLine2 = directBookingDetail.ShipTo.Address2,
                        additionalAddressInfo = "",
                        postalCode = directBookingDetail.ShipTo.PostCode,
                        city = directBookingDetail.ShipTo.City,
                        countryCode = directBookingDetail.ShipTo.Country.Code2,
                        //reference = directBookingDetail.FrayteNumber + "-" + directBookingDetail.ReferenceDetail.Reference1,
                        reference = directBookingDetail.FrayteNumber + "-" + directBookingDetail.ReferenceDetail.Reference1,
                        contact = new Contact
                        {
                            name = directBookingDetail.ShipTo.FirstName + " " + directBookingDetail.ShipTo.LastName,
                            email = string.IsNullOrEmpty(directBookingDetail.ShipTo.Email) ? "technicaltest@frayte.com" : directBookingDetail.ShipTo.Email,
                            phoneNumber = directBookingDetail.ShipTo.Phone
                        }
                    },
                    pickupPoint = null
                },
                product = new Product()
                {
                    id = "PA_DOREN",
                    customerNumber = directBookingDetail.CustomerRateCard.NetworkCode,
                    services = null,
                    customsDeclaration = directBookingDetail.CustomInfo.CatagoryOfItemExplanation,
                },
                purchaseOrder = null,
                correlationId = null,
                packages = packages
            };
            bringRequest.consignments.Add(consign);
            GetXMLFromRequestObject(bringRequest);
            return bringRequest;
        }

        internal void GetXMLFromRequestObject(object o)
        {
            dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(o);
        }

        public IntegrtaionResult MapBringIntegrationResponse(BringResponseModel bringResponse)
        {
            IntegrtaionResult integrtaionResult = new IntegrtaionResult();

            if (bringResponse.errors == null)
            {
                integrtaionResult.Status = true;
                integrtaionResult.CourierName = FrayteCourierCompany.BRING;
                integrtaionResult.TrackingNumber = bringResponse.consignments[0].confirmation.consignmentNumber;
                integrtaionResult.PickupRef = null;
                integrtaionResult.PieceTrackingDetails = new List<CourierPieceDetail>();
                foreach (var data in bringResponse.consignments[0].confirmation.packages)
                {
                    CourierPieceDetail obj = new CourierPieceDetail();
                    obj.DirectShipmentDetailId = 0;
                    obj.PieceTrackingNumber = data.packageNumber;
                    obj.ImageUrl = bringResponse.consignments[0].confirmation.links.labels;
                    integrtaionResult.PieceTrackingDetails.Add(obj);
                }
            }
            else
            {
                integrtaionResult.Error = new FratyteError();
                integrtaionResult.Error.Service = new List<string>();
                foreach (var item in bringResponse.errors)
                {
                    string error = string.Empty;
                    error = item.code + "-" + "" + item.messages;
                    integrtaionResult.Error.Service.Add(error);
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

        public string DownloadBringlabelPDF(CourierPieceDetail pieceDetails, int totalPiece, int count, int DirectShipmentid)
        {
            string Image = string.Empty;
            if (pieceDetails.ImageUrl != null)
            {
                string labelName = string.Empty;
                labelName = FrayteCourierCompany.BRING;
                Image = labelName + "_" + pieceDetails.PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".pdf";

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

        public string CallBringApi(FrayteLogisticIntegration frayteLogisticIntegration, string shipmentRequestjson)
        {
            string response = string.Empty;
            string url = string.Empty;
            try
            {
                url = frayteLogisticIntegration.ServiceUrl.Trim();
                var Uid = frayteLogisticIntegration.UserName.Trim();
                var key = frayteLogisticIntegration.InetgrationKey.Trim();
                var APIURL = frayteLogisticIntegration.AppId.Trim();

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Headers.Add("X-MyBring-API-Uid", Uid);
                httpWebRequest.Headers.Add("X-MyBring-API-Key", key);
                httpWebRequest.Headers.Add("X-Bring-Client-URL", APIURL);
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(shipmentRequestjson);
                    streamWriter.Flush();
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {

            }

            return response;
        }

        #region Express 

        public BringRequestModel MapExpressBookingDetailToShipmentRequestDto(ExpressShipmentModel shipment)
        {
            BringRequestModel bringRequest = new BringRequestModel();
            if (AppSettings.ApplicationMode == FrayteApplicationMode.Test)
            {
                bringRequest.testIndicator = true;
            }
            else
            {
                bringRequest.testIndicator = false;
            }

            bringRequest.schemaVersion = 1;
            bringRequest.consignments = new List<ConsignmentModel>();
            var packages = new List<Models.Bring.Package>();

            Models.Bring.Package package = new Models.Bring.Package();
            for (int i = 0; i < shipment.Packages.Count; i++)
            {
                for (int j = 0; j < shipment.Packages[i].CartonValue; j++)
                {
                    package.weightInKg = shipment.Packages[i].Weight.ToString("0.##");
                    package.goodsDescription = shipment.Packages[i].Content;
                    package.dimensions = new Dimensions()
                    {
                        heightInCm = shipment.Packages[i].Height,
                        lengthInCm = shipment.Packages[i].Length,
                        widthInCm = shipment.Packages[i].Width,
                    };
                    //package.containerId = shipment.FrayteNumber;
                    package.containerId = shipment.AWBNumber.Replace(" ", "").Substring(0, 8);
                    package.packageType = "";
                    package.numberOfItems = "";
                    //package.correlationId = "PACKAGE-" + shipment.FrayteNumber;
                    package.correlationId = "PACKAGE-" + shipment.AWBNumber.Replace(" ", "").Substring(0, 8);
                    packages.Add(package);
                }
            }

            var ShipFrom = new ExpressRepository().getHubAddress(shipment.ShipTo.Country.CountryId, shipment.ShipTo.PostCode, shipment.ShipTo.State);
            ConsignmentModel consign = new ConsignmentModel()
            {
                //shippingDateTime = DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss"),
                shippingDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 13, 10, 10).AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss"),

                parties = new Parties()
                {
                    sender = new Sender()
                    {
                        name = string.IsNullOrWhiteSpace(ShipFrom.CompanyName) ? ShipFrom.FirstName + " " + ShipFrom.LastName : ShipFrom.CompanyName,
                        addressLine = !string.IsNullOrEmpty(ShipFrom.Address) ? ShipFrom.Address.Trim() : "",
                        addressLine2 = !string.IsNullOrEmpty(ShipFrom.Address2) ? ShipFrom.Address2.Trim() : "",
                        additionalAddressInfo = "",
                        postalCode = !string.IsNullOrEmpty(ShipFrom.PostCode) ? ShipFrom.PostCode.Trim() : "",
                        city = !string.IsNullOrEmpty(ShipFrom.City) ? ShipFrom.City : "",
                        countryCode = !string.IsNullOrEmpty(ShipFrom.Country.Code2) ? ShipFrom.Country.Code2 : "",
                        //reference = shipment.FrayteNumber + "-" + shipment.ShipmentReference,
                        reference = shipment.AWBNumber.Replace(" ", "") + "-" + shipment.ShipmentReference,
                        contact = new Contact
                        {
                            name = string.IsNullOrEmpty(ShipFrom.FirstName + " " + ShipFrom.LastName) ? ShipFrom.CompanyName : ShipFrom.FirstName + " " + ShipFrom.LastName,
                            email = string.IsNullOrEmpty(ShipFrom.Email) ? "technicaltest@frayte.com" : ShipFrom.Email,
                            phoneNumber = ShipFrom.Phone
                        }

                        // name = "Priority Cargo AS",
                        //addressLine = "Skur 97,Kongshavnveien 29,",
                        //addressLine2 = "Kongshavnveien 29,",
                        //additionalAddressInfo = "",
                        //postalCode = "0193",
                        //city = "Oslo",
                        //countryCode = "NO",
                        //reference = shipment.AWBNumber.Replace(" ", "").Substring(0, 8) + "-" + shipment.ShipmentReference,
                        //contact = new Contact
                        //{
                        //    name = "Priority Cargo AS",
                        //    email = "technicaltest@frayte.com",
                        //    phoneNumber = "+47 9760 0402"
                        //}
                    },
                    recipient = new Recipient()
                    {
                        name = string.IsNullOrWhiteSpace(shipment.ShipTo.CompanyName) ? shipment.ShipTo.FirstName + " " + shipment.ShipTo.LastName : shipment.ShipTo.CompanyName,
                        addressLine = shipment.ShipTo.Address,
                        addressLine2 = shipment.ShipTo.Address2,
                        additionalAddressInfo = "",
                        postalCode = shipment.ShipTo.PostCode,
                        city = shipment.ShipTo.City,
                        countryCode = shipment.ShipTo.Country.Code2,
                        reference = shipment.AWBNumber.Replace(" ", "").Substring(0, 8) + "-" + shipment.ShipmentReference,
                        contact = new Contact
                        {
                            name = shipment.ShipTo.FirstName + " " + shipment.ShipTo.LastName,
                            email = string.IsNullOrEmpty(shipment.ShipTo.Email) ? "technicaltest@frayte.com" : shipment.ShipTo.Email,
                            phoneNumber = shipment.ShipTo.Phone
                        }
                    },
                    pickupPoint = null
                },
                product = new Product()
                {
                    id = "PA_DOREN",
                    //customerNumber = shipment.Service.NetworkCode,
                    customerNumber = shipment.Service.NetworkCode,
                    services = null,
                    customsDeclaration = shipment.CustomInformation.CatagoryOfItemExplanation,
                },
                purchaseOrder = null,
                correlationId = null,
                packages = packages
            };
            bringRequest.consignments.Add(consign);
            GetXMLFromRequestObject(bringRequest);
            return bringRequest;
        }

        public string ExpressDownloadBringlabelPDF(CourierPieceDetail pieceDetails, int totalPiece, int count, int DirectShipmentid)
        {
            string Image = string.Empty;
            if (pieceDetails.ImageUrl != null)
            {
                string labelName = string.Empty;
                labelName = FrayteCourierCompany.BRING;
                Image = labelName + "_" + pieceDetails.PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".pdf";

                if (AppSettings.LabelSave == "")
                {
                    if (System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/Express/" + DirectShipmentid + "/"))
                    {

                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/Express/" + DirectShipmentid + "/");
                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(AppSettings.LabelFolder + "Express/" + DirectShipmentid + "/"))
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            File.WriteAllText(AppSettings.LabelFolder + "Express/" + DirectShipmentid + "/" + Image, pieceDetails.ImageByte);
                        }
                        else
                        {
                            string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "Express/" + DirectShipmentid + "/" + Image);
                            File.WriteAllText(path, pieceDetails.ImageByte);
                        }
                    }
                    else
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "Express/" + DirectShipmentid);
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "Express/" + DirectShipmentid));
                            string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "Express/" + DirectShipmentid + "/" + Image);
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
        #endregion
    }
}
