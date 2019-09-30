using Frayte.Services.Models;
using Frayte.Services.Models.ApiModal;
using System;
using System.Net;
using Frayte.Services.DataAccess;
using System.Security.Authentication;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using Frayte.Services.Utility;
using System.Web.Hosting;
using System.Drawing;
using Frayte.Services.Models.UPS;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Frayte.Services.Business
{
    public class UPSRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public UPSShipmentResponseDto CreateShipment(UPSShipmentRequestDto upshipmentRequestDto, ReferenceDetail referenceDetail)
        {
            var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.UPS);
            var upsResult = new UPSShipmentResponseDto();
            var upsError = new UPSErrorDto();

            var package = upshipmentRequestDto.ShipmentRequest.Shipment.Package.Count();

            //Set UPS API credentials
            upshipmentRequestDto.UPSSecurity.ServiceAccessToken = new ServiceAccessTokenDto()
            {
                AccessLicenseNumber = logisticIntegration.InetgrationKey,
            };
            upshipmentRequestDto.UPSSecurity.UsernameToken = new UsernameTokenDto()
            {
                Password = logisticIntegration.Password,
                Username = logisticIntegration.UserName
            };

            var shipmentRequestjson = JsonConvert.SerializeObject(upshipmentRequestDto);
            //Calling UPS Service
            try
            {
                WebClient client = new WebClient();
                client.UseDefaultCredentials = true;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                ServicePointManager.SecurityProtocol = Tls12;

                string url = logisticIntegration.ServiceUrl;

                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string response = client.UploadString(url, "POST", shipmentRequestjson);
                var result = response;

                string pickupResposne = string.Empty;
                UPSPickupDto upsPickup = MappingUPSPickupDto(logisticIntegration, upshipmentRequestDto, referenceDetail);
                //Reponse Object

                if (package == 1)
                {
                    #region Package

                    var upsResult1 = Newtonsoft.Json.JsonConvert.DeserializeObject<UPSShipmentResponseTempDto>(result);
                    if (upsResult1.ShipmentResponse != null)
                    {
                        if (referenceDetail.CollectionDate != null && !string.IsNullOrWhiteSpace(referenceDetail.CollectionTime))
                        {
                            #region pickup

                            try
                            {
                                client.UseDefaultCredentials = true;
                                var pickupJson = JsonConvert.SerializeObject(upsPickup);
                                string pickupUrl = logisticIntegration.PickupApiUrl;

                                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                                pickupResposne = client.UploadString(pickupUrl, "POST", pickupJson);
                            }
                            catch (Exception ex)
                            {
                                upsResult.Error.Miscellaneous = new List<string>();
                                upsResult.Error.Miscellaneous.Add(ex.Message);
                                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                                upsResult.Error.Status = false;
                                return null;
                            }
                            dynamic pickupResposnes = JsonConvert.DeserializeObject<object>(pickupResposne);

                            #endregion

                            if (pickupResposnes.Fault == null)
                            {

                                var data = JObject.Parse(pickupResposne);
                                string refNo = (string)data["PickupCreationResponse"]["PRN"];

                                upsResult = MappingupsResult1ToupsResult(upsResult1);
                                upsResult.ShipmentResponse.ShipmentResults.PickupRef = refNo;
                            }
                            else
                            {
                                if (pickupResposnes.Fault != null)
                                {
                                    var upsVoidShipDto = MappingUPSVoidShipDto(logisticIntegration, upsResult1.ShipmentResponse.ShipmentResults.ShipmentIdentificationNumber);

                                    WebClient clients = new WebClient();
                                    clients.UseDefaultCredentials = true;
                                    var voidApiJson = JsonConvert.SerializeObject(upsVoidShipDto);
                                    string voidApiUrl = logisticIntegration.VoidApiUrl;

                                    clients.Headers[HttpRequestHeader.ContentType] = "application/json";
                                    string voidApiResponse = clients.UploadString(voidApiUrl, "POST", voidApiJson);

                                    var voidApiResponseDto = JsonConvert.DeserializeObject<VoidShipmentResponseDto>(voidApiResponse);

                                    if (voidApiResponseDto.Response.ResponseStatus.Description == "Success")
                                    {
                                        var error1 = "Shipment Pickup is not Created Please Try later.";

                                        upsResult.Error = new FratyteError();
                                        upsResult.Error.Service = new List<string>();
                                        upsResult.Error.Service.Add(error1);

                                        upsResult.Error.Status = false;
                                        return upsResult;
                                    }
                                }
                                upsError = JsonConvert.DeserializeObject<UPSErrorDto>(result);
                                var error = upsError.Fault.detail.Errors.ErrorDetail.PrimaryErrorCode.Description;
                                upsResult.Error = new FratyteError();
                                upsResult.Error.Service = new List<string>();
                                upsResult.Error.Service.Add(error);

                                upsResult.Error.Status = false;
                                //Error Recorded
                                SaveDirectShipmentObject(shipmentRequestjson, result, upshipmentRequestDto.DraftShipmentId);
                            }
                        }
                        else
                        {
                            upsResult = MappingupsResult1ToupsResult(upsResult1);
                        }
                    }

                    #endregion
                }
                else
                {
                    upsResult = Newtonsoft.Json.JsonConvert.DeserializeObject<UPSShipmentResponseDto>(result);

                    if (upsResult.ShipmentResponse != null)
                    {
                        upsResult.Error = new FratyteError()
                        {
                            Status = true,
                        };
                        if (referenceDetail.CollectionDate != null && !string.IsNullOrWhiteSpace(referenceDetail.CollectionTime))
                        {
                            #region pickup
                            try
                            {
                                client.UseDefaultCredentials = true;
                                var pickupJson = JsonConvert.SerializeObject(upsPickup);
                                string pickupUrl = logisticIntegration.PickupApiUrl;
                                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                                pickupResposne = client.UploadString(pickupUrl, "POST", pickupJson);

                            }
                            catch (Exception ex)
                            {
                                upsResult.Error.Miscellaneous = new List<string>();
                                upsResult.Error.Miscellaneous.Add(ex.Message);
                                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                                upsResult.Error.Status = false;
                                return null;
                            }

                            dynamic pickupResposnes = JsonConvert.DeserializeObject<object>(pickupResposne);
                            if (pickupResposnes.Fault == null)
                            {


                                var data = JObject.Parse(pickupResposne);
                                string refNo = (string)data["PickupCreationResponse"]["PRN"];

                                upsResult.ShipmentResponse.ShipmentResults.PickupRef = refNo;

                            }
                            if (pickupResposnes.Fault != null)
                            {
                                var upsVoidShipDto = MappingUPSVoidShipDto(logisticIntegration, upsResult.ShipmentResponse.ShipmentResults.ShipmentIdentificationNumber);

                                WebClient clients = new WebClient();
                                clients.UseDefaultCredentials = true;
                                var voidApiJson = JsonConvert.SerializeObject(upsVoidShipDto);
                                string voidApiUrl = logisticIntegration.VoidApiUrl;

                                clients.Headers[HttpRequestHeader.ContentType] = "application/json";
                                string voidApiResponse = clients.UploadString(voidApiUrl, "POST", voidApiJson);

                                var voidApiResponseDto = JsonConvert.DeserializeObject<VoidShipmentResponseDto>(voidApiResponse);

                                var error1 = "Shipment Pickup is not Created Please Try later.";

                                upsResult.Error = new FratyteError();
                                upsResult.Error.Service = new List<string>();
                                upsResult.Error.Service.Add(error1);

                                upsResult.Error.Status = false;
                                return upsResult;
                            }

                            #endregion
                        }
                    }
                }
                if (upsResult.ShipmentResponse == null)
                {
                    upsError = JsonConvert.DeserializeObject<UPSErrorDto>(result);
                    var error = upsError.Fault.detail.Errors.ErrorDetail.PrimaryErrorCode.Description;
                    upsResult.Error = new FratyteError();
                    upsResult.Error.Service = new List<string>();
                    upsResult.Error.Service.Add(error);

                    upsResult.Error.Status = false;
                    //Error Recorded
                    SaveDirectShipmentObject(shipmentRequestjson, result, upshipmentRequestDto.DraftShipmentId);
                }
                return upsResult;
            }
            catch (Exception ex)
            {
                upsResult.Error.Miscellaneous = new List<string>();
                upsResult.Error.Miscellaneous.Add(ex.Message);
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                upsResult.Error.Status = false;
                return null;
            }
        }

        public UPSShipmentRequestDto MapDirectBookingDetailToShipmentRequestDto(DirectBookingShipmentDraftDetail directBookingDetail)
        {
            try
            {
                //package Mapping
                var packages = new List<PackageDto>();
                mapPackages(directBookingDetail, packages);

                //Shipper Mapping           
                var shipper = new ShipperDto();
                mapShipper(directBookingDetail, shipper);

                //ShipToAddress
                var shipToAddress = new ShipToAddressDto();
                mapShipToAddress(directBookingDetail, shipToAddress);

                //ShipFromAddress
                var shipFromAddress = new ShipFromAddressDto();
                mapShipFromAddress(directBookingDetail, shipFromAddress);


                UPSShipmentRequestDto upsShipmentRequest = new UPSShipmentRequestDto();
                upsShipmentRequest.UPSSecurity = new UPSSecurityDto()
                {
                    UsernameToken = new UsernameTokenDto
                    {
                        Username = "",
                        Password = "",
                    },
                    ServiceAccessToken = new ServiceAccessTokenDto
                    {
                        AccessLicenseNumber = "",
                    },
                };
                upsShipmentRequest.DraftShipmentId = directBookingDetail.DirectShipmentDraftId;
                upsShipmentRequest.ShipmentRequest = new ShipmentRequestDto();

                upsShipmentRequest.ShipmentRequest.Request = new RequestDto
                {
                    RequestOption = "validate",
                    TransactionReference = new TransactionReferenceDto
                    {
                        CustomerContext = "UPSIntegration",
                    },
                };
                upsShipmentRequest.ShipmentRequest.Shipment = new ShipmentDto();

                upsShipmentRequest.ShipmentRequest.Shipment.Description = directBookingDetail.FrayteNumber + "-" + directBookingDetail.ReferenceDetail.Reference1;
                upsShipmentRequest.ShipmentRequest.Shipment.Shipper = shipper;

                upsShipmentRequest.ShipmentRequest.Shipment.ShipTo = shipToAddress;

                upsShipmentRequest.ShipmentRequest.Shipment.ShipFrom = shipFromAddress;
                if (directBookingDetail.CustomerRateCard.LogisticType.Contains("Import"))
                {
                    upsShipmentRequest.ShipmentRequest.Shipment.ShipmentServiceOptions = new ShipmentServiceOptions()
                    {
                        ImportControlIndicator = "0",
                        LabelMethod = new LabelMethod()
                        {
                            Code = "05",
                            Description = "ImportControl - Print Label",
                            CommerialInvoiceRemovalIndicator = "",
                        }
                    };
                }
                upsShipmentRequest.ShipmentRequest.Shipment.Service = new ServiceDto
                {
                    Code = "65",
                    Description = "UPS Express Saver",

                };
                upsShipmentRequest.ShipmentRequest.Shipment.Package = packages;

                upsShipmentRequest.ShipmentRequest.Shipment.PaymentInformation = new PaymentInformationDto
                {
                    ShipmentCharge = new ShipmentChargeDto()
                    {
                        BillShipper = new BillShipperDto()
                        {
                            AccountNumber = directBookingDetail.CustomerRateCard.CourierAccountNo
                        },
                        Type = "01"
                    },
                };
                upsShipmentRequest.ShipmentRequest.LabelSpecification = new LabelSpecificationDto
                {
                    LabelImageFormat = new LabelImageFormatDto
                    {
                        Code = "GIF",
                        Description = "GIF",
                    },
                    HTTPUserAgent = "Mozilla/4.5",
                };

                return upsShipmentRequest;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Shipment is not created " + ex.Message));
                return null;
            }
        }

        private void mapPackages(DirectBookingShipmentDraftDetail directBookingDetail, List<PackageDto> packageDto)
        {
            PackageDto package;
            foreach (var data in directBookingDetail.Packages)
            {
                if (data.CartoonValue > 0)
                {
                    for (int i = 0; i < data.CartoonValue; i++)
                    {
                        package = new PackageDto();
                        package.Description = data.Content;
                        if (directBookingDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms)
                        {
                            package.Dimensions = new DimensionsDto()
                            {
                                Height = data.Height.ToString(),
                                Width = data.Width.ToString(),
                                Length = data.Length.ToString(),
                            };
                        }
                        else if (directBookingDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs)
                        {
                            package.Dimensions = new DimensionsDto()
                            {
                                Height = (data.Height * 2.54m).ToString(),
                                Width = (data.Width * 2.54m).ToString(),
                                Length = (data.Length * 2.54m).ToString(),
                            };
                        }
                        package.Dimensions.UnitOfMeasurement = new UnitOfMeasurementDto()
                        {
                            Code = "CM",
                            Description = "centimeter",
                        };

                        var weight = string.Empty;

                        if (FraytePakageCalculationType.LbToInchs == directBookingDetail.PakageCalculatonType)
                        {
                            weight = (data.Weight * 0.5m).ToString();
                        }
                        else
                        {
                            weight = data.Weight.ToString();
                        }
                        package.PackageWeight = new PackageWeightDto()
                        {
                            Weight = directBookingDetail.CustomerRateCard.Weight.ToString("0.##"),
                            UnitOfMeasurement = new UnitOfMeasurementDto()
                            {
                                Code = "KGS",
                                Description = "KGS",
                            },
                        };
                        package.Packaging = new PackagingDto()
                        {
                            Code = "02",
                            Description = "Customer Supplied Package",
                        };
                        packageDto.Add(package);
                    }
                }
            }
        }

        private void mapShipper(DirectBookingShipmentDraftDetail directBookingDetail, ShipperDto shipperDto)
        {
            shipperDto.Name = "WHYTECLIFF GROUP LTD"; /*string.IsNullOrWhiteSpace(directBookingDetail.ShipFrom.CompanyName) ? directBookingDetail.ShipFrom.FirstName + " " + directBookingDetail.ShipFrom.LastName : directBookingDetail.ShipFrom.CompanyName;*/
            shipperDto.AttentionName = "MR ALEX"; /*directBookingDetail.ShipFrom.FirstName + " " + directBookingDetail.ShipFrom.LastName;*/
            shipperDto.FaxNumber = "";
            shipperDto.Phone = new PhoneDto()
            {
                Number = "852-21484881",
                Extension = "",
            };
            shipperDto.ShipperNumber = "W91865";
            shipperDto.TaxIdentificationNumber = "123456";

            var postalcode = !string.IsNullOrWhiteSpace(directBookingDetail.ShipFrom.PostCode) ? (directBookingDetail.ShipFrom.PostCode).Replace(" ", "") : null;
            shipperDto.Address = new AddressDto()
            {
                AddressLine = "501 KWONG LOONG TAI BUILDING", /*directBookingDetail.ShipFrom.Address + "," + directBookingDetail.ShipFrom.Area,*/
                AddressLine2 = "1016 - 1018 TAI NAN WEST STREET",
                City = "CHEUNG SHA WAN"/*directBookingDetail.ShipFrom.City*/,
                CountryCode = "HK" /*directBookingDetail.ShipFrom.Country.Code2*/,
                PostalCode = "",
                StateProvinceCode = ""
            };
        }

        private void mapShipToAddress(DirectBookingShipmentDraftDetail directBookingDetail, ShipToAddressDto shipToAddressDto)
        {
            shipToAddressDto.Name = string.IsNullOrWhiteSpace(directBookingDetail.ShipTo.CompanyName) ? directBookingDetail.ShipTo.FirstName + " " + directBookingDetail.ShipTo.LastName : directBookingDetail.ShipTo.CompanyName;
            shipToAddressDto.AttentionName = directBookingDetail.ShipTo.FirstName + " " + directBookingDetail.ShipTo.LastName;

            shipToAddressDto.Phone = new PhoneDto()
            {
                Number = directBookingDetail.ShipTo.Phone,
                Extension = "",
            };
            var postalcode = !string.IsNullOrWhiteSpace(directBookingDetail.ShipTo.PostCode) ? (directBookingDetail.ShipTo.PostCode).Replace(" ", "") : null;
            shipToAddressDto.Address = new AddressDto()
            {
                AddressLine = directBookingDetail.ShipTo.Address + " ," + directBookingDetail.ShipTo.Area,
                City = directBookingDetail.ShipTo.City,
                CountryCode = directBookingDetail.ShipTo.Country.Code2,
                PostalCode = postalcode,
                StateProvinceCode = ""
            };
        }

        private void mapShipFromAddress(DirectBookingShipmentDraftDetail directBookingDetail, ShipFromAddressDto shipFromAddressDto)
        {
            shipFromAddressDto.Name = string.IsNullOrWhiteSpace(directBookingDetail.ShipFrom.CompanyName) ? directBookingDetail.ShipFrom.FirstName + " " + directBookingDetail.ShipFrom.FirstName : directBookingDetail.ShipFrom.CompanyName;
            shipFromAddressDto.AttentionName = directBookingDetail.ShipFrom.FirstName + " " + directBookingDetail.ShipFrom.FirstName;
            shipFromAddressDto.FaxNumber = "";
            shipFromAddressDto.Phone = new PhoneDto()
            {
                Number = directBookingDetail.ShipFrom.Phone,
                Extension = "",
            };
            shipFromAddressDto.TaxIdentificationNumber = "123456";
            var postalcode = !string.IsNullOrWhiteSpace(directBookingDetail.ShipFrom.PostCode) ? (directBookingDetail.ShipFrom.PostCode).Replace(" ", "") : null;
            shipFromAddressDto.Address = new AddressDto()
            {
                AddressLine = directBookingDetail.ShipFrom.Address + "," + directBookingDetail.ShipFrom.Area,
                City = directBookingDetail.ShipFrom.City,
                CountryCode = directBookingDetail.ShipFrom.Country.Code2,
                PostalCode = postalcode,
                StateProvinceCode = ""
            };
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

        public string DownloadUPSImage(CourierPieceDetail pieceDetails, int totalPiece, int count, int DirectShipmentid)
        {
            string Image = string.Empty;

            byte[] image = Convert.FromBase64String(pieceDetails.ImageByte);
            System.Drawing.Image labelimage;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(image))
            {
                labelimage = System.Drawing.Image.FromStream(ms);
                string labelName = string.Empty;
                labelName = FrayteShortName.UPS;
                Image = labelName + "_" + pieceDetails.PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".jpg";

                if (AppSettings.LabelSave == "")
                {
                    if (System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/"))
                    {
                        labelimage.Save(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/" + Image);
                    }
                    else
                    {
                        labelimage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/");
                        labelimage.Save(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/" + Image);
                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(AppSettings.LabelFolder + "/" + DirectShipmentid + "/"))
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            labelimage.Save(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image);
                        }
                        else
                        {
                            labelimage.Save(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image));
                        }

                    }
                    else
                    {
                        labelimage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "/" + DirectShipmentid);
                            labelimage.Save(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image);
                        }
                        else
                        {

                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid));
                            labelimage.Save(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image));
                        }
                    }
                }
            }
            return Image;
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

        //UPS Tracking
        public FrayteShipmentTracking GetUPSTrackingInfo(string CarrierName, string TrackingNumberCode)
        {
            var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.UPS);

            //Request
            var UPSTrackRequestDto = new UPSTrackRequestDto()
            {
                Security = new SecurityDto()
                {
                    UsernameToken = new UsernameTokenDto()
                    {
                        Username = logisticIntegration.UserName,
                        Password = logisticIntegration.Password,
                    },
                    UPSServiceAccessToken = new ServiceAccessTokenDto()
                    {
                        AccessLicenseNumber = logisticIntegration.InetgrationKey,
                    },
                },
                TrackRequest = new TrackRequestDto()
                {
                    Request = new RequestDto()
                    {
                        RequestOption = "15",
                        TransactionReference = new TransactionReferenceDto()
                        {
                            CustomerContext = "UPSTracking"
                        },
                    },
                    InquiryNumber = TrackingNumberCode,  /*TrackingNumberCode,/* "*/
                    TrackingOption = "02"
                }
            };

            var UPSTrackRequestJson = JsonConvert.SerializeObject(UPSTrackRequestDto);

            WebClient client = new WebClient();
            client.UseDefaultCredentials = true;


            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            SslProtocols _Tls12 = (SslProtocols)0x00000C00;
            SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
            ServicePointManager.SecurityProtocol = Tls12;

            string url = logisticIntegration.AppId;

            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            string response = client.UploadString(url, "POST", UPSTrackRequestJson);
            var result = response;

            dynamic Responseresult = JsonConvert.DeserializeObject<object>(result);
            TrackResponseDto trackResponse = MapTrackResponse(JsonConvert.SerializeObject(Responseresult.TrackResponse));
            if (trackResponse != null)
            {
                var shimpent = Responseresult.TrackResponse.Shipment;
                List<TrackPackageDto> package = MapTrackPackage(JsonConvert.SerializeObject(Responseresult.TrackResponse.Shipment.Package));
                TrackServiceDto service = MapTrackService(JsonConvert.SerializeObject(Responseresult.TrackResponse.Shipment.Service));
                TrackShipmentWeightDto ShipmentWeight = MapShipmentWeight(JsonConvert.SerializeObject(Responseresult.TrackResponse.Shipment.ShipmentWeight));

                string pickupdate = string.Empty;
                if (shimpent.PickupDate == null)
                    pickupdate = "";
                else
                    pickupdate = shimpent.PickupDate.Value;

                if (ShipmentWeight == null)
                {
                    ShipmentWeight = new TrackShipmentWeightDto();
                    ShipmentWeight.Weight = package.FirstOrDefault().PackageWeight.Weight;
                    ShipmentWeight.UnitOfMeasurement = package.FirstOrDefault().PackageWeight.UnitOfMeasurement;
                }

                var shipmentResposneResult = new UPSTrackResponseDto()
                {

                    Shipment = new TrackingShipmentDto
                    {
                        InquiryNumber = new InquiryNumberDto()
                        {
                            Code = shimpent.InquiryNumber.Code.Value,
                            Description = shimpent.InquiryNumber.Description.Value,
                            Value = shimpent.InquiryNumber.Value.Value,
                        },
                        Package = package,
                        Service = service,
                        ShipperNumber = shimpent.ShipperNumber == null ? "" : shimpent.ShipperNumber.Value,
                        ShipmentWeight = ShipmentWeight,
                        PickupDate = pickupdate,

                    },
                    TrackResponse = trackResponse,
                };
                var upsInfo = MapUPSTrackingToFrayte(shipmentResposneResult);
                return upsInfo;
            }
            else
            {
                return null;
            }

        }

        private List<TrackPackageDto> MapTrackPackage(string packagejson)
        {
            var TrackPackage = new List<TrackPackageDto>();
            TrackPackageDto trackPackage = new TrackPackageDto();
            var result1 = packagejson.Contains("\"Activity\":[{");
            // var result2= packagejson.Contains("Activity:[{");
            if (!result1)
            {
                var Activity = new ActivityDto();
                dynamic trackPackageDto = JsonConvert.DeserializeObject<object>(packagejson);
                var Activityjson = JsonConvert.SerializeObject(trackPackageDto.Activity);

                Activity = JsonConvert.DeserializeObject<ActivityDto>(Activityjson);

                dynamic isDelted = trackPackageDto.Remove("Activity");
                if (isDelted)
                {
                    var json = JsonConvert.SerializeObject(trackPackageDto);

                    trackPackage = JsonConvert.DeserializeObject<TrackPackageDto>(json);
                    trackPackage.Activity = new List<ActivityDto>();
                    trackPackage.Activity.Add(Activity);
                }

            }
            else
            {
                trackPackage = JsonConvert.DeserializeObject<TrackPackageDto>(packagejson);
            }

            TrackPackage.Add(trackPackage);
            return TrackPackage;
        }

        private TrackServiceDto MapTrackService(string servicejson)
        {
            var service = JsonConvert.DeserializeObject<TrackServiceDto>(servicejson);
            return service;
        }

        private TrackResponseDto MapTrackResponse(string trackRequestjson)
        {
            var trackResponse = JsonConvert.DeserializeObject<TrackResponseDto>(trackRequestjson);
            return trackResponse;
        }

        private TrackShipmentWeightDto MapShipmentWeight(string shipmentWeightjson)
        {
            var shipmentWeight = JsonConvert.DeserializeObject<TrackShipmentWeightDto>(shipmentWeightjson);
            return shipmentWeight;
        }

        private FrayteShipmentTracking MapUPSTrackingToFrayte(UPSTrackResponseDto upsResposneDto)
        {
            FrayteShipmentTracking trckInfo = new FrayteShipmentTracking();
            trckInfo.Status = true;
            trckInfo.Tracking = new List<ShipmentTracking>();

            ShipmentTracking tracking = new ShipmentTracking();

            tracking.TrackingDetails = new List<ShipmentTrackingDetail>();
            ShipmentTrackingDetail detail;
            if (upsResposneDto != null)
            {
                var minDate = upsResposneDto.Shipment.Package[0].Activity.Min(s => s.Date);
                var minTime = upsResposneDto.Shipment.Package[0].Activity.Where(k => k.Date == minDate).Min(s => s.Time);
                var maxDate = upsResposneDto.Shipment.Package[0].Activity.Max(s => s.Date);
                var maxTime = upsResposneDto.Shipment.Package[0].Activity.Where(k => k.Date == maxDate).Max(s => s.Time);
                var status = upsResposneDto.Shipment.Package[0].Activity.FirstOrDefault(c => c.Date == maxDate && c.Time == maxTime);
                tracking.Status = status.Status.Description;
                tracking.CarriertrackingId = upsResposneDto.Shipment.InquiryNumber.Value;
                tracking.Carrier = upsResposneDto.Shipment.Service.Description;
                tracking.Courier = "UPS";
                tracking.TrackingNumber = upsResposneDto.Shipment.InquiryNumber.Value;
                tracking.ShowHideValue = "";
                tracking.NoOfPieces = upsResposneDto.Shipment.Package.Count();
                tracking.SignedBy = status.ActivityLocation.SignedForByName;
                tracking.EstimatedDeliveryTime = upsResposneDto.Shipment.Package[0].Activity[0].Time;
                tracking.EstimatedDeliveryDate = UtilityRepository.GetFormattedDateInMMDDYYYY(upsResposneDto.Shipment.Package[0].Activity[0].Date);
                tracking.CreatedAtDate = UtilityRepository.GetFormattedDateInMMDDYYYY(minDate);
                tracking.CreatedAtTime = minTime;
                tracking.UpdatedAtDate = UtilityRepository.GetFormattedDateInMMDDYYYY(maxDate).ToString();
                tracking.UpdatedAtTime = maxTime;
                tracking.EstimatedWeight = Convert.ToDouble(upsResposneDto.Shipment.ShipmentWeight == null ? "0.00" : upsResposneDto.Shipment.ShipmentWeight.Weight);
                tracking.IsHeaderShow = true;

                // Tracking Detail
                foreach (var data in upsResposneDto.Shipment.Package)
                {
                    foreach (var activity in data.Activity)
                    {
                        var time = UtilityRepository.GetTimeFromString(activity.Time);
                        var date = UtilityRepository.GetFormattedDateInMMDDYYYY(activity.Date);
                        detail = new ShipmentTrackingDetail();
                        detail.Activity = activity.Status.Description;
                        detail.Location = activity.ActivityLocation.Address == null ? null : activity.ActivityLocation.Address.CountryCode;
                        detail.Time = time.Value.ToString();
                        detail.Date = Frayte.Services.CommonConversion.ConvertToDateTime(date);
                        detail.Pieces = new List<string>();
                        detail.Pieces.Add(data.TrackingNumber);
                        tracking.TrackingDetails.Add(detail);
                    }
                }
                trckInfo.Tracking.Add(tracking);
            }
            return trckInfo;
        }

        public IntegrtaionResult MapUPSIntegrationResponse(UPSShipmentResponseDto upsShipmentResposnDto)
        {
            IntegrtaionResult integrtaionResult = new IntegrtaionResult();

            if (upsShipmentResposnDto.Error.Status)
            {
                integrtaionResult.Status = true;
                integrtaionResult.CourierName = FrayteCourierCompany.UPS;
                integrtaionResult.TrackingNumber = upsShipmentResposnDto.ShipmentResponse.ShipmentResults.ShipmentIdentificationNumber;
                integrtaionResult.PickupRef = upsShipmentResposnDto.ShipmentResponse.ShipmentResults.PickupRef.ToString();
                integrtaionResult.PieceTrackingDetails = new List<CourierPieceDetail>();
                foreach (var data in upsShipmentResposnDto.ShipmentResponse.ShipmentResults.PackageResults)
                {
                    CourierPieceDetail obj = new CourierPieceDetail();
                    obj.DirectShipmentDetailId = 0;
                    obj.PieceTrackingNumber = data.TrackingNumber;
                    obj.ImageByte = data.ShippingLabel.GraphicImage;
                    integrtaionResult.PieceTrackingDetails.Add(obj);
                }
            }
            else
            {
                integrtaionResult.Error = upsShipmentResposnDto.Error;
                integrtaionResult.ErrorCode = new List<FrayteApiError>();
                {
                    List<FrayteApiError> _api = new FrayteApiErrorCodeRepository().SaveFinilizeApiError(integrtaionResult.Error, FrayteLogisticServiceType.UPS, null);
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

        private UPSPickupDto MappingUPSPickupDto(FrayteLogisticIntegration FrayteLogisticIntegration, UPSShipmentRequestDto UPSShipmentRequest, ReferenceDetail referenceDetail)
        {
            DateTime mindatetime = referenceDetail.CollectionDate.Value;
            DateTime maxdatetime = referenceDetail.CollectionDate.Value.AddDays(1);

            if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Sunday)
            {
                mindatetime = referenceDetail.CollectionDate.Value.AddDays(1);
            }
            else if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Saturday)
            {

                mindatetime = referenceDetail.CollectionDate.Value.AddDays(2);
            }
            var UPSPickupDto = new UPSPickupDto();
            UPSPickupDto.UPSSecurity = new UPSSecurityDto()
            {
                UsernameToken = new UsernameTokenDto()
                {
                    Password = FrayteLogisticIntegration.Password,
                    Username = FrayteLogisticIntegration.UserName,
                },
                ServiceAccessToken = new ServiceAccessTokenDto()
                {
                    AccessLicenseNumber = FrayteLogisticIntegration.InetgrationKey,
                }
            };
            UPSPickupDto.PickupCreationRequest = new PickupCreationRequestDto()
            {
                Request = new Request()
                {
                    TransactionReference = new TransactionReferenceDto()
                    {
                        CustomerContext = "UPSPickupApiCalling"
                    }
                },
                RatePickupIndicator = "Y",
                TaxInformationIndicator = "Y",
                PickupDateInfo = new PickupDateInfoDto()
                {
                    CloseTime = "1700",
                    ReadyTime = "1200",
                    PickupDate = mindatetime.ToString("yyyyMMdd")
                },
                PickupAddress = new PickupAddressDto()
                {
                    CompanyName = UPSShipmentRequest.ShipmentRequest.Shipment.ShipFrom.Name,
                    ContactName = UPSShipmentRequest.ShipmentRequest.Shipment.ShipFrom.AttentionName,
                    AddressLine = UPSShipmentRequest.ShipmentRequest.Shipment.ShipFrom.Address.AddressLine,
                    City = UPSShipmentRequest.ShipmentRequest.Shipment.ShipFrom.Address.City,
                    CountryCode = UPSShipmentRequest.ShipmentRequest.Shipment.ShipFrom.Address.CountryCode,
                    StateProvince = UPSShipmentRequest.ShipmentRequest.Shipment.ShipFrom.Address.CountryCode == "HK" ? UPSShipmentRequest.ShipmentRequest.Shipment.ShipFrom.Address.City : UPSShipmentRequest.ShipmentRequest.Shipment.ShipFrom.Address.StateProvinceCode,
                    Phone = new PhoneDto()
                    {
                        Number = UPSShipmentRequest.ShipmentRequest.Shipment.ShipFrom.Phone.Number,
                    },
                    PostalCode = UPSShipmentRequest.ShipmentRequest.Shipment.ShipFrom.Address.PostalCode,
                    ResidentialIndicator = "N",

                },
                AlternateAddressIndicator = "N",
                OverweightIndicator = "N",
                PaymentMethod = "05",
                PickupPiece = new PickupPieceDto()
                {
                    ContainerCode = "01",
                    DestinationCountryCode = UPSShipmentRequest.ShipmentRequest.Shipment.ShipTo.Address.CountryCode,
                    Quantity = "1",
                    ServiceCode = "065",
                },
            };
            return UPSPickupDto;
        }

        private VoidShipDto MappingUPSVoidShipDto(FrayteLogisticIntegration FrayteLogisticIntegration, string shipmentIdentificationNumber)
        {
            VoidShipDto VoidShip = new VoidShipDto();
            VoidShip.UPSSecurity = new SecurityDto()
            {

                UsernameToken = new UsernameTokenDto()
                {
                    Username = FrayteLogisticIntegration.UserName,
                    Password = FrayteLogisticIntegration.Password,
                },
                UPSServiceAccessToken = new ServiceAccessTokenDto()
                {
                    AccessLicenseNumber = FrayteLogisticIntegration.InetgrationKey,
                },
            };
            VoidShip.VoidShipmentRequest = new VoidShipmentRequestDto()
            {
                Request = new Request()
                {
                    TransactionReference = new TransactionReferenceDto()
                    {
                        CustomerContext = "ups"
                    },
                },
                VoidShipment = new VoidShipmentDto()
                {
                    ShipmentIdentificationNumber = shipmentIdentificationNumber,
                }

            };
            return VoidShip;
        }

        static string ConvertObjectToXMLString(object classObject)
        {
            string xmlString = null;
            XmlSerializer xmlSerializer = new XmlSerializer(classObject.GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, classObject);
                memoryStream.Position = 0;
                xmlString = new StreamReader(memoryStream).ReadToEnd();
            }
            return xmlString;
        }

        private UPSShipmentResponseDto MappingupsResult1ToupsResult(UPSShipmentResponseTempDto upsResult1)
        {
            var upsResult = new UPSShipmentResponseDto()
            {
                Error = upsResult1.Error,
                ShipmentResponse = new ShipmentResponseDto()
                {
                    Response = new ResponseDto()
                    {
                        ResponseStatus = new ResponseStatusDto()
                        {
                            Code = upsResult1.ShipmentResponse.Response.ResponseStatus.Code,
                            Description = upsResult1.ShipmentResponse.Response.ResponseStatus.Description,

                        },
                        TransactionReference = new TransactionReferenceDto()
                        {
                            CustomerContext = upsResult1.ShipmentResponse.Response.TransactionReference.CustomerContext,
                        },

                    },
                    ShipmentResults = new ShipmentResultsDto()
                    {
                        BillingWeight = new BillingWeightDto()
                        {
                            UnitOfMeasurement = new UnitOfMeasurementDto()
                            {
                                Code = upsResult1.ShipmentResponse.ShipmentResults.BillingWeight.UnitOfMeasurement.Code,
                                Description = upsResult1.ShipmentResponse.ShipmentResults.BillingWeight.UnitOfMeasurement.Description,
                            }
                        },
                        NegotiatedRateCharges = new NegotiatedRateChargesDto()
                        {
                            TotalCharge = new TotalChargeDto()
                            {
                                CurrencyCode = upsResult1.ShipmentResponse.ShipmentResults.NegotiatedRateCharges == null ? null : upsResult1.ShipmentResponse.ShipmentResults.NegotiatedRateCharges.TotalCharge.CurrencyCode,
                                MonetaryValue = upsResult1.ShipmentResponse.ShipmentResults.NegotiatedRateCharges == null ? null : upsResult1.ShipmentResponse.ShipmentResults.NegotiatedRateCharges.TotalCharge.MonetaryValue,
                            }
                        },
                        PackageResults = new List<PackageResultsDto>()
                        {
                           new PackageResultsDto
                           {
                              ServiceOptionsCharges= new ServiceOptionsChargesDto()
                               {
                                      CurrencyCode=upsResult1.ShipmentResponse.ShipmentResults.PackageResults.ServiceOptionsCharges.CurrencyCode,
                                      MonetaryValue=upsResult1.ShipmentResponse.ShipmentResults.PackageResults.ServiceOptionsCharges.MonetaryValue,

                                },
                                ShippingLabel= new ShippingLabelDto()
                                 {
                                            GraphicImage= upsResult1.ShipmentResponse.ShipmentResults.PackageResults.ShippingLabel.GraphicImage,
                                            HTMLImage= upsResult1.ShipmentResponse.ShipmentResults.PackageResults.ShippingLabel.HTMLImage,
                                            ImageFormat= new ImageFormatDto
                                            {
                                                Code= upsResult1.ShipmentResponse.ShipmentResults.PackageResults.ShippingLabel.ImageFormat.Code,
                                                Description= upsResult1.ShipmentResponse.ShipmentResults.PackageResults.ShippingLabel.ImageFormat.Description,
                                            },
                                        },
                                        TrackingNumber= upsResult1.ShipmentResponse.ShipmentResults.PackageResults.TrackingNumber
                                    },
                                },
                        ShipmentCharges = new ShipmentChargesDto()
                        {
                            ServiceOptionsCharges = new ServiceOptionsChargesDto()
                            {
                                CurrencyCode = upsResult1.ShipmentResponse.ShipmentResults.ShipmentCharges.ServiceOptionsCharges.CurrencyCode,
                                MonetaryValue = upsResult1.ShipmentResponse.ShipmentResults.ShipmentCharges.ServiceOptionsCharges.MonetaryValue
                            },
                            TotalCharges = new TotalChargesDto()
                            {
                                CurrencyCode = upsResult1.ShipmentResponse.ShipmentResults.ShipmentCharges.TotalCharges.CurrencyCode,
                                MonetaryValue = upsResult1.ShipmentResponse.ShipmentResults.ShipmentCharges.TotalCharges.MonetaryValue,
                            },
                            TransportationCharges = new TransportationChargesDto()
                            {
                                CurrencyCode = upsResult1.ShipmentResponse.ShipmentResults.ShipmentCharges.TransportationCharges.CurrencyCode,
                                MonetaryValue = upsResult1.ShipmentResponse.ShipmentResults.ShipmentCharges.TransportationCharges.MonetaryValue,
                            }

                        },
                        ShipmentIdentificationNumber = upsResult1.ShipmentResponse.ShipmentResults.ShipmentIdentificationNumber,
                    },
                },
            };
            upsResult.Error = new FratyteError()
            {
                Status = true,
            };
            return upsResult;
        }
    }
}