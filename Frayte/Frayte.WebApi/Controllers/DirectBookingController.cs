using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Models.Aftership;
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
using System.Web.Http;
using Frayte.Services.Utility;
using System.Text.RegularExpressions;
using Frayte.Services;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using Report.Generator.ManifestReport;
using System.Net.Http.Headers;
using System.Web.Hosting;
using System.Text;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using Frayte.Services.Models.DHL;
using Frayte.Services.Models.AU;
using Frayte.Services.Models.EAM;
using Frayte.Services.Models.ETower;

namespace Frayte.WebApi.Controllers
{
    // [Authorize]
    [AllowAnonymous]

    public class DirectBookingController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GetInitials(int customerId)
        {
            //Step 1: Get Country list
            List<FrayteCountryCode> lstCountry = new List<FrayteCountryCode>();

            lstCountry = new CountryRepository().lstCountry();

            //Step 1.1: Get To Country list For eCommerce
            List<FrayteCountryCode> lstToCountry = new List<FrayteCountryCode>();

            lstToCountry = new CountryRepository().ListToCountry();

            MasterDataRepository mdRepository = new MasterDataRepository();
            //Step 2: Get Parcel Type
            var lstParcelTypes = mdRepository.GetParcelType();

            //Step 3: Get Currency
            var lstCurrencyTypes = mdRepository.GetCurrencyType();

            //Step 4: Get Shipment Method Type
            List<FrayteShipmentMethod> lstShipmentMethods = new CourierRepository().GetShipmentMethods("Courier");

            //Step4: Get Customer Address
            FrayteUserDefaultAddresses customerDetail = new DirectShipmentRepository().UserDefaultAddress(customerId);

            var frayteCustomerDetail = new DirectShipmentRepository().GetCustomerDetail(customerId);

            // Step 5 : Get Country PhoneCode
            List<FrayteCountryPhoneCode> lstCountryPhones = new List<FrayteCountryPhoneCode>();

            lstCountryPhones = new CountryRepository().GetCountryPhoneCodeList();

            // step:6 get all operation zones
            var OperaionZones = new OperationZoneRepository().GetOperationZone();
            return this.Request.CreateResponse(
                HttpStatusCode.OK,
                new
                {
                    Countries = lstCountry,
                    ToCountries = lstToCountry,
                    ParcelTypes = lstParcelTypes,
                    CurrencyTypes = lstCurrencyTypes,
                    ShipmentMethods = lstShipmentMethods,
                    CustomerDetail = customerDetail,
                    FrayteCustomerDetail = frayteCustomerDetail,
                    CountryPhoneCodes = lstCountryPhones,
                    OperaionZones = OperaionZones
                });
        }

        [HttpGet]
        public DirectBookingCollection GetCustomerDetail(int CustomerId)
        {
            DirectBookingCollection customerDetail = new DirectShipmentRepository().GetCustomerDetail(CustomerId);
            return customerDetail;
        }

        [HttpGet]
        public FrayteUserDefaultAddresses UserDefaultAddress(int CustomerId)
        {
            return new DirectShipmentRepository().UserDefaultAddress(CustomerId);
        }

        [HttpPost]
        public IHttpActionResult GetServices(DirectBookingFindService serviceRequest)
        {
            try
            {
                List<DirectBookingService> lstDirectBookingServices = new List<DirectBookingService>();
                lstDirectBookingServices = new DirectShipmentRepository().GetServices(serviceRequest);
                return Ok(lstDirectBookingServices);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public DirectBookingShipmentDetail GetDirectBookingDetail(int directShipmentId, string CallingType)
        {
            return new DirectShipmentRepository().GetDirectBookingDetail(directShipmentId, CallingType);
        }

        [HttpPost]
        public IHttpActionResult SetPrintPackageStatus(Frayte.Services.Models.Package package)
        {
            FrayteResult result = new FrayteResult();
            result = new DirectShipmentRepository().SetPrintPackageStatus(package);
            return Ok(result);
        }

        [HttpPost]
        public List<FrayteDirectBookingAddressBook> CustomerAddressAdvanceSearch(FrayteCustomerAddressSearch filter)
        {
            var addresDetails = new DirectShipmentRepository().GetCustomerAddressAdvanceSearch(filter);
            return addresDetails;
        }

        [HttpGet]
        public FrayteResult CustomerDefaultAddress(int addressBookId, int countryId, int userId, string addressType)
        {
            FrayteResult result = new DirectShipmentRepository().SetCustomerDefaultAddress(addressBookId, countryId, userId, addressType);
            return result;
        }

        [HttpGet]
        public FrayteUserDefaultAddresses GetCountryDefaultAddress(int countryId, int userId, string addressType)
        {
            try
            {
                FrayteUserDefaultAddresses userAddress = new DirectShipmentRepository().getUserDefaultAddress(userId, countryId, addressType);
                return userAddress;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public FrayteResult CustomerDefaultAddressBook(int addressBookId, int countryId, int userId, bool value, string addressType)
        {
            FrayteResult result = new DirectShipmentRepository().SetCustomerDefaultAddress(addressBookId, countryId, userId, value, addressType);
            return result;
        }

        [HttpPost]
        public FrayteResult EditCustomerAddress(FrayteDirectBookingAddressBook FrayteAddressBook)
        {
            var result = new DirectShipmentRepository().AddEditCustomerAddressBook(FrayteAddressBook);
            return result;
        }

        [HttpGet]
        public IHttpActionResult SetAddressFavourite(int AddressBookId, bool IsFavorites)
        {
            new DirectShipmentRepository().MarkAsIsFavorites(AddressBookId, IsFavorites);
            return Ok();
        }

        [HttpGet]
        public FrayteResult EraseAllCustomerAddress(int CustomerId, string AddressType)
        {
            FrayteResult result = new DirectShipmentRepository().EraseCustomerAddress(CustomerId, AddressType);
            return result;
        }

        [HttpGet]
        public FrayteResult DeleteCustomerAddress(int AddressId, string TableType)
        {
            FrayteResult result = new DirectShipmentRepository().DeleteCustomerAddress(AddressId, TableType);
            return result;
        }

        [HttpGet]
        public List<DirectBookingCustomer> GetDirectBookingCustomers(int userId, string moduleType)
        {
            var customers = new DirectShipmentRepository().GetDirectBookingCustomers(userId, moduleType);
            return customers;
        }

        [HttpGet]
        public IHttpActionResult GenerateDHLLabel(int DirectShipmentId)
        {
            List<string> list = new List<string>();
            List<string> list1 = new List<string>();

            list1.Add("3623752646_01_08_2017_53_195.jpg");
            list1.Add("3623752646_01_08_2017_53_945.jpg");
            list1.Add("3623752646_01_08_2017_54_770.jpg");
            list1.Add("3623752646_01_08_2017_58_614.jpg");
            list1.Add("3623752646_01_08_2017_58_688.jpg");
            list1.Add("3623752646_01_08_2017_58_757.jpg");
            list1.Add("3623752646_01_08_2017_58_852.jpg");


            list.Add("3623752646_01_08_2017_53_195.jpg");
            list.Add("3623752646_01_08_2017_53_945.jpg");
            list.Add("3623752646_01_08_2017_54_770.jpg");
            list.Add("3623752646_01_08_2017_58_614.jpg");
            list.Add("3623752646_01_08_2017_58_688.jpg");
            list.Add("3623752646_01_08_2017_58_757.jpg");
            list.Add("3623752646_01_08_2017_58_852.jpg");

            List<string> li = new List<string>();
            // Generate seperate label
            foreach (var data in list1)
            {
                li.Add(data);
                var resultReportsa = new Report.Generator.ManifestReport.PackageLabelReport().GenerateAllLabelReport(DirectShipmentId, null, li, "DHL", "");
                li.Remove(data);
            }

            DirectBookingShipmentDraftDetail draft = new DirectBookingShipmentDraftDetail();

            draft.CustomerRateCard = new DirectBookingService();
            draft.CustomerRateCard.DisplayName = "DHL";
            draft.CustomerRateCard.RateType = "Express";

            // Generate All Label
            var resultReport = new Report.Generator.ManifestReport.PackageLabelReport().GenerateAllLabelReport(DirectShipmentId, draft, list, "DHL", "");
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult SaveDirectBooking(DirectBookingShipmentDraftDetail directBookingDetail)
        {
            int DirectShipmentid = 0;

            #region COLLECTION DATE 

            if (directBookingDetail.ReferenceDetail.CollectionDate != null)
            {
                DateTime mindatetime = directBookingDetail.ReferenceDetail.CollectionDate.Value;
                DateTime maxdatetime = directBookingDetail.ReferenceDetail.CollectionDate.Value.AddDays(1);

                if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Sunday)
                {
                    mindatetime = directBookingDetail.ReferenceDetail.CollectionDate.Value.AddDays(1);
                }

                directBookingDetail.ReferenceDetail.CollectionDate = mindatetime;
            }

            #endregion

            //Save the information in database
            DirectShipmentRepository directBooking = new DirectShipmentRepository();
            directBookingDetail = directBooking.SaveDirectBooking(directBookingDetail);
            if (directBookingDetail.Error.Status && directBookingDetail.BookingStatusType == FrayteBookingStatusType.Current)
            {
                //Update date time in UI object
                if (directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.UKMail &&
                directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.Yodel &&
                directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.Hermes)
                {
                    var Result = directBooking.GetDBDateTime(directBookingDetail.DirectShipmentDraftId);
                    if (Result != null)
                    {
                        // directBookingDetail.ReferenceDetail = new ReferenceDetail();
                        var TZ = directBooking.TimeZoneDetail(directBookingDetail.ShipFrom.Country.CountryId);
                        var TimezoneINformation = TimeZoneInfo.FindSystemTimeZoneById(TZ.Name);
                        directBookingDetail.ReferenceDetail.CollectionDate = Result.CollectionDate.HasValue ? UtilityRepository.UtcDateToOtherTimezone(Result.CollectionDate.Value, Result.CollectionTime.Value, TimezoneINformation).Item1 : DateTime.UtcNow;
                        directBookingDetail.ReferenceDetail.CollectionTime = UtilityRepository.ConvertToCustomerTimeZone(Result.CollectionTime.HasValue ? Result.CollectionTime.Value : directBookingDetail.ReferenceDetail.CollectionDate.Value.TimeOfDay, TZ);
                    }
                }

                //Shipment Integrations
                #region Parcel Hub Integration

                if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.UKMail ||
                    directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Yodel ||
                    directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Hermes)
                {
                    //Map Direct Booking object with parcel hub objects
                    Frayte.Services.Models.ParcelHub.ParcelHubShipmentRequest request = new ParcelHubRepository().MapDirectBookingDetailToShipmentRequest(directBookingDetail);

                    //Create shipment in Parcel hub
                    Frayte.Services.Models.ParcelHub.ParcelHubResponse response = new ParcelHubRepository().CreateShipment(request);

                    if (response.Error.IsMailSend)
                    {
                        //Send error mail to developer
                        directBookingDetail.Error = new FratyteError();
                        directBookingDetail.Error.Custom = new List<string>();
                        directBookingDetail.Error.Package = new List<string>();
                        directBookingDetail.Error.Address = new List<string>();
                        directBookingDetail.Error.Service = new List<string>();
                        directBookingDetail.Error.ServiceError = new List<string>();
                        directBookingDetail.Error.Miscellaneous = new List<string>();
                        directBookingDetail.Error.MiscErrors = new List<FrayteKeyValue>();
                        directBookingDetail.Error = response.Error;
                        try
                        {
                            new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, response.Error);
                        }
                        catch (Exception ex)
                        {

                        }
                        //Error Record
                        new DirectShipmentRepository().SaveEasyPosyPickUpObject(Newtonsoft.Json.JsonConvert.SerializeObject(response.Error).ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(directBookingDetail).ToString(), directBookingDetail.DirectShipmentDraftId);
                        var error = new DirectBookingUploadShipmentRepository().GetFrayteError(directBookingDetail.DirectShipmentDraftId);
                    }
                    else
                    {
                        //Mapping ShipmentResonse to IntegrtaionResult
                        var result = new ParcelHubRepository().MappingParcelHubToIntegrationResult(directBookingDetail, response, null);

                        if (result.Status)
                        {
                            //Save Shipment Detail Into Our DB
                            DirectShipmentid = new DirectShipmentRepository().SaveShipment(directBookingDetail, result);

                            //Mapping ShipmentResult to IntegrtaionResult
                            new ParcelHubRepository().MappingCourierPieceDetail(result, directBookingDetail, DirectShipmentid);

                            //Save Package Label Tracking detail
                            new ParcelHubRepository().SaveTrackingDetail(directBookingDetail, result, DirectShipmentid);

                            //Start downloading the images from UPS server and making PDF
                            FratyteError Error = new ParcelHubRepository().DownloadParcelHubPackageImage(directBookingDetail, result, DirectShipmentid);

                            //Start Making Final One pdf file for all package label
                            Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, result);
                        }
                        else
                        {
                            if (result.Error.IsMailSend)
                            {
                                //Send error mail to developer
                                directBookingDetail.Error = new FratyteError();
                                directBookingDetail.Error.Custom = new List<string>();
                                directBookingDetail.Error.Package = new List<string>();
                                directBookingDetail.Error.Address = new List<string>();
                                directBookingDetail.Error.Service = new List<string>();
                                directBookingDetail.Error.ServiceError = new List<string>();
                                directBookingDetail.Error.Miscellaneous = new List<string>();
                                directBookingDetail.Error.MiscErrors = new List<FrayteKeyValue>();
                                directBookingDetail.Error = result.Error;
                                try
                                {
                                    new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, result.Error);
                                }
                                catch (Exception ex)
                                {

                                }
                                new DirectShipmentRepository().SaveEasyPosyPickUpObject(Newtonsoft.Json.JsonConvert.SerializeObject(response.Error).ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(directBookingDetail).ToString(), directBookingDetail.DirectShipmentDraftId);
                            }
                        }
                    }
                }

                #endregion

                #region TNT Integration

                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.TNT)
                {
                    var shipmentPackageTrackingDetail = new List<FraytePackageTrackingDetail>();

                    // TNT Integration is for HK as we have account detail for HK only
                    var TNTObj = new TNTRepository().MapDirectBookingObjToTNTobj(directBookingDetail);
                    var TNTXml = new TNTRepository().CreateTNTXMl(TNTObj);

                    var TNTResponse = new TNTRepository().CreateShipment(TNTXml, directBookingDetail.DirectShipmentDraftId);
                    directBookingDetail.Error = TNTResponse.Error;

                    if (directBookingDetail.Error != null && directBookingDetail.Error.Status == true)
                    {
                        //Finally Save the Order Id too to get the information of multiple shpment in Tracking Detail page.
                        DirectShipmentid = new DirectShipmentRepository().SaveShipment(directBookingDetail, TNTResponse.TNTShipmentResponse.TrackingCode, directBookingDetail.CustomerId, TNTResponse.TNTShipmentResponse.BookingRefNo, null);

                        //Save tracking no 
                        int increment = 0;
                        foreach (var package in directBookingDetail.Packages)
                        {
                            var trackingDetail = new DirectShipmentRepository().SaveTNTTrackingDeatil(TNTResponse.TNTShipmentResponse.TrackingCode, DirectShipmentid, increment);
                            if (trackingDetail != null)
                            {
                                foreach (var data in trackingDetail)
                                {
                                    shipmentPackageTrackingDetail.Add(data);
                                }
                            }
                            increment++;
                        }

                        // check f label is generated
                        if (!string.IsNullOrEmpty(TNTResponse.TNTShipmentResponse.TrackingCode))
                        {
                            //Generate LableHtml
                            int counter = 1;
                            int totalNumberOfPieces = shipmentPackageTrackingDetail.Count;

                            #region TNT Label Generate

                            for (int i = 0; i < shipmentPackageTrackingDetail.Count; i++)
                            {
                                string labelHtml = string.Empty;
                                string lableOutputxml = string.Empty;
                                labelHtml = new TNTRepository().CreateLabelXML(TNTObj, counter.ToString(), totalNumberOfPieces.ToString(), TNTObj.Packages[0], TNTResponse.TNTShipmentResponse.TrackingCode);
                                lableOutputxml = new TNTRepository().GetLabelxml(labelHtml);
                                if (!string.IsNullOrWhiteSpace(lableOutputxml))
                                {
                                    TNTResponse.TNTShipmentResponse.ConnoteReply = lableOutputxml;
                                    labelHtml = new TNTRepository().GenerateLabelHtmlNew(TNTResponse);
                                    string LabelName = new TNTRepository().DownloadTNTHtmlImage(labelHtml, TNTResponse.TNTShipmentResponse.TrackingCode, shipmentPackageTrackingDetail.Count(), i + 1, DirectShipmentid);

                                    //save image name in package tracking detail table
                                    try
                                    {
                                        new DirectShipmentRepository().SaveImage(shipmentPackageTrackingDetail[i], LabelName);
                                        shipmentPackageTrackingDetail[i].PackageImage = LabelName;
                                        TNTResponse.Error.Status = true;
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                counter++;
                            }

                            #endregion
                        }
                        else
                        {
                            TNTResponse.Error.Status = false;
                            var er = new FrayteKeyValue();
                            er.Key = "Misscllaneous Errors";
                            er.Value = new List<string>() { "Something bad happen please try again later." };
                            TNTResponse.Error.MiscErrors.Add(er);
                        }
                        //Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail);
                        new TNTRepository().SaveTempShipmentXML(TNTResponse, DirectShipmentid);
                    }
                    //generate label pdf
                    if (directBookingDetail.Error.Status)
                    {
                        Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, null);
                    }
                }

                #endregion

                #region UPS Integration

                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.UPS)
                {
                    IntegrtaionResult result = new IntegrtaionResult();

                    if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.UPS)
                    {
                        //Mapping With directBookingDetail to ShipmentRequestDto
                        var shipmentRequestDto = new UPSRepository().MapDirectBookingDetailToShipmentRequestDto(directBookingDetail);

                        //Create Shipment
                        var shipmentResult = new UPSRepository().CreateShipment(shipmentRequestDto, directBookingDetail.ReferenceDetail);

                        //Mapping ShipmentResonse to IntegrtaionResult
                        result = new UPSRepository().MapUPSIntegrationResponse(shipmentResult);
                    }

                    if (result.Status)
                    {
                        //After Shipment Create need to save the information in database 
                        DirectShipmentid = new DirectShipmentRepository().SaveShipment(directBookingDetail, result);

                        if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.UPS)
                        {
                            //Mapping ShipmentResult to IntegrtaionResult
                            new UPSRepository().MappingCourierPieceDetail(result, directBookingDetail, DirectShipmentid);
                        }

                        //Save Package Label Tracking detail
                        new UPSRepository().SaveTrackingDetail(directBookingDetail, result, DirectShipmentid);

                        //Start downloading the images from UPS server and making PDF
                        var count = 1;
                        foreach (var data in result.PieceTrackingDetails)
                        {
                            if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.UPS)
                            {
                                data.LabelName = new UPSRepository().DownloadUPSImage(data, result.PieceTrackingDetails.Count(), count, DirectShipmentid);
                            }
                            new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                            count++;
                        }

                        //Generate PDF
                        Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, result);

                        directBookingDetail.Error = new FratyteError()
                        {
                            Status = result.Status,
                        };
                    }
                    else
                    {
                        directBookingDetail.Error = result.Error;
                        try
                        {
                            new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, result.Error);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                #endregion

                #region DHL Integration

                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DHL)
                {
                    IntegrtaionResult result = new IntegrtaionResult();
                    //Mapping With directBookingDetail to ShipmentRequestDto
                    var shipmentRequestDto = new DHLRepository().MapDirectBookingDetailToDHLShipmentRequestDto(directBookingDetail);

                    //Create Xml 
                    string shipmentXML = string.Empty;
                    //Version5.0
                    shipmentXML = new DHLRepository().CreateXMLForDHL(shipmentRequestDto);

                    string xml_in = File.ReadAllText(@shipmentXML);
                    //Create Shipment
                    var DHLResponse = new DHLRepository().CreateShipment(xml_in, shipmentRequestDto);

                    //Mapping ShipmentResonse to IntegrtaionResult
                    result = new DHLRepository().MapDHLIntegrationResponse(DHLResponse);

                    if (result.Status)
                    {
                        //After Shipment Create need to save the information in database 
                        DirectShipmentid = new DirectShipmentRepository().SaveShipment(directBookingDetail, result);

                        if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DHL)
                        {
                            //Mapping ShipmentResult to IntegrtaionResult
                            new DHLRepository().MappingCourierPieceDetail(result, directBookingDetail, DirectShipmentid);
                        }

                        //Save Package Label Tracking detail
                        new DHLRepository().SaveTrackingDetail(directBookingDetail, result, DirectShipmentid);

                        //Start downloading the images from DHL server and making PDF
                        var count = 1;
                        var totalpiece = result.PieceTrackingDetails.Count();
                        totalpiece = totalpiece - 1;

                        foreach (var data in result.PieceTrackingDetails.Take(totalpiece))
                        {
                            if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DHL)
                            {
                                data.LabelName = new DHLRepository().DownloadDHLImage(data, totalpiece, count, DirectShipmentid);
                            }
                            if (!data.PieceTrackingNumber.Contains("AirwayBillNumber_"))
                            {
                                new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                            }
                            count++;
                        }
                        var CourierPieceDetail = result.PieceTrackingDetails.Where(t => t.PieceTrackingNumber.Contains("AirwayBillNumber_")).FirstOrDefault();
                        if (CourierPieceDetail != null)
                        {
                            var data1 = new DHLRepository().DownloadDHLImage(CourierPieceDetail, totalpiece, 0, DirectShipmentid);
                        }

                        if (directBookingDetail.Error.Status)
                        {
                            Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, result);
                        }
                    }
                    else
                    {
                        directBookingDetail.Error = result.Error;
                        try
                        {
                            new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, result.Error);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                #endregion

                #region DPD Integration

                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DPD)
                {
                    IntegrtaionResult result = new IntegrtaionResult();

                    if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DPD)
                    {
                        //Mapping With directBookingDetail to ShipmentRequestDto
                        var shipmentRequestDto = new DPDRepository().MapDirectBookingDetailToShipmentRequestDto(directBookingDetail);

                        //Create Shipment
                        var shipmentResult = new DPDRepository().CreateShipment(shipmentRequestDto);

                        //Mapping ShipmentResonse to IntegrtaionResult
                        result = new DPDRepository().MapDPDIntegrationResponse(shipmentResult);
                    }

                    if (result.Status)
                    {
                        //After Shipment Create need to save the information in database 
                        DirectShipmentid = new DirectShipmentRepository().SaveShipment(directBookingDetail, result);

                        if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DPD)
                        {
                            //Mapping ShipmentResult to IntegrtaionResult
                            new DPDRepository().MappingCourierPieceDetail(result, directBookingDetail, DirectShipmentid);
                        }

                        //Save Package Label Tracking detail
                        new DPDRepository().SaveTrackingDetail(directBookingDetail, result, DirectShipmentid);

                        //Start downloading the images from DPD server and making PDF
                        var count = 1;
                        foreach (var data in result.PieceTrackingDetails)
                        {
                            if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DPD)
                            {
                                data.LabelName = new DPDRepository().DownloadDPDHtmlImage(data, result.PieceTrackingDetails.Count(), count, DirectShipmentid, directBookingDetail.CustomerRateCard);
                            }
                            new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                            count++;
                        }
                        //Generate PDF
                        Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, result);
                        directBookingDetail.Error = new FratyteError()
                        {
                            Status = result.Status,
                        };
                    }
                    else
                    {
                        directBookingDetail.Error = result.Error;
                        try
                        {
                            //Mapping With directBookingDetail to ShipmentRequestDto
                            new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, result.Error);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                #endregion

                #region AU Integration

                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.AU)
                {
                    IntegrtaionResult result = new IntegrtaionResult();
                    if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.AU)
                    {
                        var shipmentRequestDto = new AURepository().MapDirectBookingDetailToShipmentRequestDto(directBookingDetail);
                        //Create Shipment
                        var shipmentResult = new AURepository().CreateShipment(shipmentRequestDto);
                        result = new AURepository().MapAUIntegrationResponse(shipmentResult);
                    }

                    if (result.Status)
                    {
                        //After Shipment Create need to save the information in database 
                        DirectShipmentid = new DirectShipmentRepository().SaveShipment(directBookingDetail, result);

                        if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.AU)
                        {
                            //Mapping ShipmentResult to IntegrtaionResult
                            new AURepository().MappingCourierPieceDetail(result, directBookingDetail, DirectShipmentid);
                        }

                        //Save Package Label Tracking detail
                        new AURepository().SaveTrackingDetail(directBookingDetail, result, DirectShipmentid);

                        //Start downloading the images from DPD server and making PDF
                        var count = 1;
                        foreach (var data in result.PieceTrackingDetails)
                        {
                            if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.AU)
                            {
                                data.LabelName = new AURepository().DownloadAUPDF(data, result.PieceTrackingDetails.Count(), count, DirectShipmentid);
                            }
                            new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                            count++;
                        }

                        //Generate PDF
                        Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, result);
                        directBookingDetail.Error = new FratyteError()
                        {
                            Status = result.Status,
                        };
                    }
                    else
                    {
                        directBookingDetail.Error = result.Error;
                        try
                        {
                            //Mapping With directBookingDetail to ShipmentRequestDto
                            new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, result.Error);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                #endregion

                #region SKYPOSTAL Integration

                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.SKYPOSTAL)
                {
                    IntegrtaionResult result = new IntegrtaionResult();
                    if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.SKYPOSTAL)
                    {
                        var shipmentRequestDto = new SkyPostalRepository().MapDirectBookingDetailToShipmentRequestDto(directBookingDetail);
                        //Create Shipment
                        //  var shipmentResult = new SkyPostalRepository().CreateShipment(shipmentRequestDto, directBookingDetail.DirectShipmentDraftId);
                        //result = new SkyPostalRepository().MapSkyPostalIntegrationResponse(shipmentResult, shipmentRequestDto.detail);
                    }

                    if (result.Status)
                    {
                        //After Shipment Create need to save the information in database 
                        DirectShipmentid = new DirectShipmentRepository().SaveShipment(directBookingDetail, result);

                        if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.SKYPOSTAL)
                        {
                            //Mapping ShipmentResult to IntegrtaionResult
                            new SkyPostalRepository().MappingCourierPieceDetail(result, directBookingDetail, DirectShipmentid);
                        }

                        //Save Package Label Tracking detail
                        new SkyPostalRepository().SaveTrackingDetail(directBookingDetail, result, DirectShipmentid);

                        //Start downloading the images from DPD server and making PDF
                        var count = 1;
                        foreach (var data in result.PieceTrackingDetails)
                        {
                            if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.SKYPOSTAL)
                            {
                                data.LabelName = new SkyPostalRepository().DownloadSkyPostalImage(data, result.PieceTrackingDetails.Count(), count, DirectShipmentid);
                            }
                            new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                            count++;
                        }
                        //Generate PDF
                        Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, result);

                        directBookingDetail.Error = new FratyteError()
                        {
                            Status = result.Status,
                        };
                    }
                    else
                    {
                        directBookingDetail.Error = result.Error;
                        try
                        {
                            //Mapping With directBookingDetail to ShipmentRequestDto
                            new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, result.Error);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                #endregion

                #region EAM Global Integration

                else if (directBookingDetail.CustomerRateCard.CourierName.Contains(FrayteCourierCompany.EAM))
                {
                    IntegrtaionResult result = new IntegrtaionResult();
                    if (directBookingDetail.CustomerRateCard.CourierName.Contains(FrayteCourierCompany.EAM))
                    {
                        var shipmentRequestDto = new EAMGlobalRepository().MapDirectBookingDetailToShipmentRequestDto(directBookingDetail);
                        string shipmentXML = new EAMGlobalRepository().CreateXMLForEAM(shipmentRequestDto);
                        string xml_in = File.ReadAllText(@shipmentXML);

                        //Create Shipment
                        var shipmentResult = new EAMGlobalRepository().CreateShipment(xml_in, directBookingDetail.DirectShipmentDraftId);
                        result = new EAMGlobalRepository().MapEAMGlobalIntegrationResponse(shipmentResult, directBookingDetail.Packages);
                    }

                    if (result.Status)
                    {
                        //After Shipment Create need to save the information in database 
                        DirectShipmentid = new DirectShipmentRepository().SaveShipment(directBookingDetail, result);

                        if (directBookingDetail.CustomerRateCard.CourierName.Contains(FrayteCourierCompany.EAM))
                        {
                            //Mapping ShipmentResult to IntegrtaionResult
                            new EAMGlobalRepository().MappingCourierPieceDetail(result, directBookingDetail, DirectShipmentid);
                        }

                        //Save Package Label Tracking detail
                        new EAMGlobalRepository().SaveTrackingDetail(directBookingDetail, result, DirectShipmentid);

                        //Start downloading the images from DPD server and making PDF
                        var count = 1;
                        foreach (var data in result.PieceTrackingDetails)
                        {
                            if (directBookingDetail.CustomerRateCard.CourierName.Contains(FrayteCourierCompany.EAM))
                            {
                                data.LabelName = new EAMGlobalRepository().DownloadEAMImageTOPDF(data, result.PieceTrackingDetails.Count(), count, DirectShipmentid, directBookingDetail.CustomerRateCard);
                            }
                            new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                            count++;
                        }
                        //Generate PDF
                        Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, result);

                        directBookingDetail.Error = new FratyteError()
                        {
                            Status = result.Status,
                        };
                    }
                    else
                    {
                        directBookingDetail.Error = result.Error;
                        try
                        {
                            //Mapping With directBookingDetail to ShipmentRequestDto
                            new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, result.Error);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                #endregion

                #region DPD-CH Integration

                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DPDCH)
                {
                    IntegrtaionResult result = new IntegrtaionResult();

                    if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DPDCH)
                    {
                        //Mapping With directBookingDetail to ShipmentRequestDto
                        var shipmentRequestDto = new DPDSwitzerlandRepository().MapDirectBookingDetailToShipmentRequestDto(directBookingDetail);
                        //Create Shipment
                        var shipmentResult = new DPDSwitzerlandRepository().CreateShipment(shipmentRequestDto, FrayteShipmentServiceType.DirectBooking);
                        //Mapping ShipmentResonse to IntegrtaionResult
                        result = new DPDSwitzerlandRepository().MapDPDCHIntegrationResponse(shipmentResult);
                    }

                    if (result.Status)
                    {
                        //After Shipment Create need to save the information in database 
                        DirectShipmentid = new DirectShipmentRepository().SaveShipment(directBookingDetail, result);

                        if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DPDCH)
                        {
                            //Mapping ShipmentResult to IntegrtaionResult
                            new DPDSwitzerlandRepository().MappingCourierPieceDetail(result, directBookingDetail, DirectShipmentid);
                        }

                        //Save Package Label Tracking detail
                        new DPDSwitzerlandRepository().SaveTrackingDetail(directBookingDetail, result, DirectShipmentid);

                        //Start downloading the images from DPD server and making PDF
                        var count = 1;
                        foreach (var data in result.PieceTrackingDetails)
                        {
                            if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DPDCH)
                            {
                                data.LabelName = new DPDSwitzerlandRepository().DownloadDPDCHBytetoImage(data, result.PieceTrackingDetails.Count(), count, DirectShipmentid);
                            }
                            new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                            count++;
                        }
                        //Generate PDF
                        Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, result);
                        directBookingDetail.Error = new FratyteError()
                        {
                            Status = result.Status,
                        };
                    }

                    else
                    {
                        directBookingDetail.Error = result.Error;
                        try
                        {
                            //Mapping With directBookingDetail to ShipmentRequestDto
                            new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, result.Error);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                #endregion

                #region Bring Integration

                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.BRING)
                {
                    IntegrtaionResult result = new IntegrtaionResult();
                    if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.BRING)
                    {
                        var shipmentRequestDto = new BringRepository().MapDirectBookingDetailToShipmentRequestDto(directBookingDetail);
                        //Create Shipment
                        var shipmentResult = new BringRepository().CreateShipment(shipmentRequestDto, directBookingDetail.DirectShipmentDraftId, FrayteShipmentServiceType.DirectBooking);
                        result = new BringRepository().MapBringIntegrationResponse(shipmentResult);
                    }

                    if (result.Status)
                    {
                        //After Shipment Create need to save the information in database 
                        DirectShipmentid = new DirectShipmentRepository().SaveShipment(directBookingDetail, result);

                        if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.BRING)
                        {
                            //Mapping ShipmentResult to IntegrtaionResult
                            new BringRepository().MappingCourierPieceDetail(result, directBookingDetail, DirectShipmentid);
                        }

                        //Save Package Label Tracking detail
                        new BringRepository().SaveTrackingDetail(directBookingDetail, result, DirectShipmentid);

                        //Start downloading the images from DPD server and making PDF
                        var count = 1;
                        foreach (var data in result.PieceTrackingDetails)
                        {
                            if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.BRING)
                            {
                                data.LabelName = new BringRepository().DownloadBringlabelPDF(data, result.PieceTrackingDetails.Count(), count, DirectShipmentid);
                            }
                            new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                            count++;
                        }
                        //Generate PDF
                        Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, result);
                        directBookingDetail.Error = new FratyteError()
                        {
                            Status = result.Status,
                        };
                    }
                    else
                    {
                        directBookingDetail.Error = result.Error;
                        try
                        {
                            //Mapping With directBookingDetail to ShipmentRequestDto
                            new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, result.Error);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                #endregion

                #region Canada Post Integration

                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.CANADAPOST)
                {
                    IntegrtaionResult result = new IntegrtaionResult();
                    if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.CANADAPOST)
                    {
                        var shipmentRequestDto = new ETowerRepository().MapDirectBookingDetailToShipmentRequestDto(directBookingDetail);
                        //Create Shipment
                        var shipmentResult = new ETowerRepository().CreateShipment(shipmentRequestDto, directBookingDetail.DirectShipmentDraftId);
                        result = new ETowerRepository().MapCanadaPostIntegrationResponse(shipmentResult, shipmentRequestDto[0].orderItems);
                    }

                    if (result.Status)
                    {
                        //After Shipment Create need to save the information in database 
                        DirectShipmentid = new DirectShipmentRepository().SaveShipment(directBookingDetail, result);

                        if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.CANADAPOST)
                        {
                            //Mapping ShipmentResult to IntegrtaionResult
                            new ETowerRepository().MappingCourierPieceDetail(result, DirectShipmentid);
                        }

                        //Save Package Label Tracking detail
                        new ETowerRepository().SaveTrackingDetail(directBookingDetail, result, DirectShipmentid);

                        //Start downloading the images from DPD server and making PDF
                        var count = 1;
                        foreach (var data in result.PieceTrackingDetails)
                        {
                            if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.CANADAPOST)
                            {
                                data.LabelName = new ETowerRepository().DownloadCanadaPostBytetoImage(data, result.PieceTrackingDetails.Count(), count, DirectShipmentid);
                            }
                            new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                            count++;
                        }
                        //Generate PDF
                        Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, result);
                        directBookingDetail.Error = new FratyteError()
                        {
                            Status = result.Status,
                        };
                    }
                    else
                    {
                        directBookingDetail.Error = result.Error;
                        try
                        {
                            // Mapping With directBookingDetail to ShipmentRequestDto
                            new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, result.Error);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                #endregion

                if (directBookingDetail.Error.Status)
                {
                    FrayteAfterShipTracking aftershipTracking = new AftershipTrackingRepository().MapDirectShipmentObjToAfterShip(DirectShipmentid, FrayteShipmentServiceType.DirectBooking);
                    if (aftershipTracking != null && AppSettings.ApplicationMode == FrayteApplicationMode.Live)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Integration in aftership"));
                        new AftershipTrackingRepository().CreateTracking(aftershipTracking);
                    }

                    // Offline tracking
                    List<FrayteOfflineTracking> frayteOfflineTracking = new TrackingRepository().MapDirectShipmentObjTacking(DirectShipmentid, directBookingDetail);

                    new TrackingRepository().UpdateOfflineTracking(frayteOfflineTracking);

                    //SendDirectBookingConfirmationMailAsync(directBookingDetail);
                    new ShipmentEmailRepository().SendDirectBookingConfirmationMail(directBookingDetail, DirectShipmentid);
                }

                FrayteDirectShipment shipmentdetail = new DirectShipmentRepository().GetShipmentDetail(DirectShipmentid);
                if (shipmentdetail != null)
                {
                    directBookingDetail.DirectShipmentDraftId = shipmentdetail.DirectShipmentId;
                    directBookingDetail.ShipFrom.AddressBookId = shipmentdetail.FromAddressId;
                    directBookingDetail.ShipTo.AddressBookId = shipmentdetail.ToAddressId;
                    directBookingDetail.ShipmentStatusId = shipmentdetail.ShipmentStatusId;
                }
            }

            return Ok(directBookingDetail);
        }

        public FrayteResult Generate_Seperate_PackageLabelPDF(int DirectShipmentId, DirectBookingShipmentDraftDetail directBookingDetail, IntegrtaionResult integrtaionResult)
        {
            FrayteResult result = new FrayteResult();
            result.Errors = new List<string>();

            List<string> list = new List<string>();
            List<string> list1 = new List<string>();

            var Packages = new DirectShipmentRepository().GetPackageDetails(DirectShipmentId);
            if (Packages != null)
            {
                if (directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.AU && directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.SKYPOSTAL && !directBookingDetail.CustomerRateCard.CourierName.Contains(FrayteCourierCompany.EAM) && directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.DPDCH && directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.BRING && directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.CANADAPOST)
                {
                    foreach (var pack in Packages)
                    {
                        var packageTracking = getPackageImamagePath(pack.DirectShipmentDetailId);
                        if (packageTracking != null && packageTracking.Count > 0)
                        {
                            foreach (var data in packageTracking)
                            {
                                list.Add(data.PackageImage);
                                list1.Add(data.PackageImage);
                                var resultReport = new Report.Generator.ManifestReport.PackageLabelReport().GenerateAllLabelReport(DirectShipmentId, null, list1, directBookingDetail.CustomerRateCard.CourierName, "");
                                if (!resultReport.Status)
                                {
                                    result.Status = false;
                                    result.Errors.Add("All the labels Pdf are not generated for " + DirectShipmentId.ToString());
                                }
                                list1.Remove(data.PackageImage);
                            }
                        }
                        else
                        {
                            result.Status = false;
                            result.Errors.Add("All the labels Pdf are not generated for " + DirectShipmentId.ToString());
                        }
                    }
                }

                string RateType = directBookingDetail.CustomerRateCard.RateType;
                string CourierCompany = directBookingDetail.CustomerRateCard.DisplayName;
                string TrackingNo = new DirectShipmentRepository().GetTrackingNo(DirectShipmentId);
                string labelName = string.Empty;
                if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Yodel)
                {
                    labelName = FrayteShortName.Yodel;
                }
                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Hermes)
                {
                    labelName = FrayteShortName.Hermes;
                }
                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.UKMail)
                {
                    labelName = FrayteShortName.UKMail;
                }
                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DHL)
                {
                    labelName = FrayteShortName.DHL;
                }
                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.TNT)
                {
                    labelName = FrayteShortName.TNT;
                }
                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.UPS)
                {
                    labelName = FrayteShortName.UPS;
                }
                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DPD)
                {
                    labelName = directBookingDetail.CustomerRateCard.DisplayName;
                }
                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.AU)
                {
                    labelName = FrayteShortName.AU;
                }
                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.SKYPOSTAL)
                {
                    labelName = FrayteShortName.SKYPOSTAL;
                }
                else if (directBookingDetail.CustomerRateCard.CourierName.Contains(FrayteCourierCompany.EAM))
                {
                    labelName = directBookingDetail.CustomerRateCard.DisplayName;
                }
                else if (directBookingDetail.CustomerRateCard.CourierName.Contains(FrayteCourierCompany.DPDCH))
                {
                    labelName = FrayteShortName.DPDCH;
                }
                else if (directBookingDetail.CustomerRateCard.CourierName.Contains(FrayteCourierCompany.BRING))
                {
                    labelName = FrayteShortName.BRING;
                }
                else if (directBookingDetail.CustomerRateCard.CourierName.Contains(FrayteCourierCompany.CANADAPOST))
                {
                    labelName = FrayteShortName.CANADAPOST;
                }
                string LogisticLabel = string.Empty;

                int totalCartoonValue = directBookingDetail.Packages.Sum(k => k.CartoonValue);
                if (!string.IsNullOrEmpty(directBookingDetail.CustomerRateCard.RateType))
                {
                    if (TrackingNo.Contains("Order_"))
                    {
                        var Trackinginfo = TrackingNo.Replace("Order_", "");
                        LogisticLabel = labelName + "_" + Trackinginfo + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                    }
                    else
                    {
                        if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DPD || directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.TNT || directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.EAMTNT)
                        {
                            if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DPD)
                            {
                                for (int i = 0; i < integrtaionResult.PieceTrackingDetails.Count; i++)
                                {
                                    int numberofpiece = i + 1;
                                    string Image = labelName + "_" + integrtaionResult.PieceTrackingDetails[i].PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + numberofpiece + " of " + totalCartoonValue + ")" + ".html";
                                    LogisticLabel += Image + ";";
                                }
                            }
                            else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.TNT || directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.EAMTNT)
                            {
                                for (int i = 1; i <= totalCartoonValue; i++)
                                {
                                    string Image = labelName + "_" + TrackingNo + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + i + " of " + totalCartoonValue + ")" + ".html";
                                    LogisticLabel += Image + ";";
                                }
                            }
                        }
                        else
                        {
                            if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.AU || directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.SKYPOSTAL || directBookingDetail.CustomerRateCard.CourierName.Contains(FrayteCourierCompany.EAM) || directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DPDCH || directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.BRING || directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.CANADAPOST)
                            {
                                LogisticLabel = integrtaionResult.PieceTrackingDetails[0].LabelName;
                            }
                            else
                            {
                                LogisticLabel = labelName + "_" + TrackingNo + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                            }
                        }
                    }
                }
                else
                {
                    if (TrackingNo.Contains("Order_"))
                    {
                        var Trackinginfo = TrackingNo.Replace("Order_", "");
                        LogisticLabel = labelName + "_" + Trackinginfo + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                    }
                    else
                    {
                        if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DPD || directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.TNT)
                        {
                            if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DPD)
                            {
                                for (int i = 1; i <= integrtaionResult.PieceTrackingDetails.Count; i++)
                                {

                                    int numberofpiece = i + 1;
                                    string Image = labelName + "_" + integrtaionResult.PieceTrackingDetails[i].PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + numberofpiece + " of " + totalCartoonValue + ")" + ".html";
                                    LogisticLabel += Image + ";";
                                }
                            }
                            else
                            {
                                for (int i = 1; i <= totalCartoonValue; i++)
                                {
                                    string Image = labelName + "_" + TrackingNo + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + i + " of " + totalCartoonValue + ")" + ".html";
                                    LogisticLabel += Image + ";";
                                }
                            }
                        }
                        else
                        {
                            if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.AU || directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.SKYPOSTAL || directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.EAM || directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DPDCH || directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.BRING || directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.CANADAPOST)
                            {
                                LogisticLabel = integrtaionResult.PieceTrackingDetails[0].LabelName;
                            }
                            else
                            {
                                LogisticLabel = labelName + "_" + TrackingNo + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                            }
                        }
                    }
                }
                if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DHL)
                {
                    var shipmentImange = new DirectShipmentRepository().GetShipmentImage(DirectShipmentId);
                    list.Add(shipmentImange.ShipmentImage);
                }
                // After Creting label  save in DirectShipmentTable
                if (directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.DPD && directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.TNT && directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.AU && directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.SKYPOSTAL && !directBookingDetail.CustomerRateCard.CourierName.Contains(FrayteCourierCompany.EAM) && directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.DPDCH && directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.BRING && directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.CANADAPOST)
                {
                    var Result = new Report.Generator.ManifestReport.PackageLabelReport().GenerateAllLabelReport(DirectShipmentId, directBookingDetail, list, directBookingDetail.CustomerRateCard.CourierName, LogisticLabel);
                    if (Result.Status)
                    {
                        new DirectShipmentRepository().SaveLogisticLabel(DirectShipmentId, LogisticLabel);
                    }
                }
                else
                {
                    new DirectShipmentRepository().SaveLogisticLabel(DirectShipmentId, LogisticLabel);
                }
            }
            else
            {
                result.Status = false;
                result.Errors.Add("All the labels Pdf are not generated for " + DirectShipmentId.ToString());
            }
            return result;
        }

        public List<PackageTrackingDetail> getPackageImamagePath(int DirectShipmentDetailId)
        {
            return new DirectShipmentRepository().GetPackageTracking(DirectShipmentDetailId);
        }

        [HttpGet]
        public IHttpActionResult GenerateAllLabelReport()
        {
            //var result = new Report.Generator.ManifestReport.eCommerceShipmentLabelReport().GenerateDirectShipemntReportPdf();
            //var result = new Report.Generator.ManifestReport.PackageLabelReport().GenerateAllLabelReport();
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult GenerateLabelPDFForShipments()
        {
            FrayteResult result = new FrayteResult();

            result.Errors = new List<string>();

            var data = new DirectShipmentRepository().GetAllDirectShipments();
            if (data != null)
            {
                foreach (var Id in data)
                {
                    var resultReport = Generate_Seperate_PackageLabelPDF(Id, null, null);
                    if (!resultReport.Status)
                    {
                        result.Status = false;
                        if (resultReport.Errors.Count > 0)
                        {
                            foreach (var str in resultReport.Errors)
                            {
                                result.Errors.Add(str);
                            }
                        }
                    }
                }
            }
            else
            {
                result.Status = true;
                result.Errors.Add("No Shipment found.");
            }
            return Ok(result);
        }

        [HttpGet]
        public DirectBookingShipmentDraftDetail GetShipmentDraftDetail(int DirectShipmentDraftId, string CallingType)
        {
            return new DirectShipmentRepository().GetDirectShipmentDraftDetail(DirectShipmentDraftId, CallingType);
        }

        private async Task SendDirectBookingConfirmationMailAsync(DirectBookingShipmentDetail directBookingDetail, int abc)
        {
            var result = await new ShipmentEmailRepository().SendDirectBookingConfirmationMail(directBookingDetail, abc);
            if (!result)
            {
                // log to elmah
            }
        }

        [HttpPost]
        public FraytePackageDetailExcel GetPiecesDetailFromExcel()
        {
            FraytePackageDetailExcel frayteShipmentDetailexcel = new Services.Models.FraytePackageDetailExcel();

            var httpRequest = HttpContext.Current.Request;

            List<Frayte.Services.Models.Package> _shipmentdetail = new List<Frayte.Services.Models.Package>();
            Frayte.Services.Models.Package frayteshipment;

            if (httpRequest.Files.Count > 0)
            {
                HttpFileCollection files = httpRequest.Files;

                HttpPostedFile file = files[0];

                if (!string.IsNullOrEmpty(file.FileName))
                {
                    //int ShipmentId = Convert.ToInt32(httpRequest.Form["ShipmentId"].ToString());
                    string connString = "";
                    string filename = DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss_") + file.FileName;
                    string filepath = HttpContext.Current.Server.MapPath("~/UploadFiles/Shipments/" + filename);
                    file.SaveAs(filepath);
                    connString = new DirectShipmentRepository().getExcelConnectionString(filename, filepath);
                    string fileExtension = "";
                    fileExtension = new DirectShipmentRepository().getFileExtensionString(filename);
                    try
                    {
                        if (!string.IsNullOrEmpty(fileExtension))
                        {
                            var ds = new DataSet();
                            if (fileExtension == FrayteFileExtension.CSV)
                            {
                                using (var conn = new OleDbConnection(connString))
                                {
                                    conn.Open();
                                    var query = "SELECT * FROM [" + Path.GetFileName(filename) + "]";
                                    using (var adapter = new OleDbDataAdapter(query, conn))
                                    {
                                        adapter.Fill(ds, "Pieces");
                                    }
                                }
                            }
                            else
                            {
                                using (var conn = new OleDbConnection(connString))
                                {
                                    conn.Open();
                                    DataTable dbSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                    string firstSheetName = dbSchema.Rows[0]["TABLE_NAME"].ToString();
                                    var query = "SELECT * FROM " + "[" + firstSheetName + "]";
                                    using (var adapter = new OleDbDataAdapter(query, conn))
                                    {
                                        adapter.Fill(ds, "Pieces");
                                    }
                                }
                            }

                            var exceldata = ds.Tables[0];

                            string PiecesColumnList = "CartonQTY,Length,Width,Height,Weight,DeclaredValue,ShipmentContents";
                            bool IsExcelValid = UtilityRepository.CheckUploadExcelFormat(PiecesColumnList, exceldata);
                            if (!IsExcelValid)
                            {
                                frayteShipmentDetailexcel.Message = "Columns are not matching with provided template columns. Please check the column names.";
                            }
                            else
                            {
                                if (exceldata.Rows.Count > 0)
                                {
                                    _shipmentdetail = new DirectShipmentRepository().GetPiecesDetail(exceldata);
                                    frayteShipmentDetailexcel.FrayteShipmentDetail = new List<Frayte.Services.Models.Package>();
                                    frayteShipmentDetailexcel.FrayteShipmentDetail = _shipmentdetail;
                                    frayteShipmentDetailexcel.Message = "OK";
                                }

                                else
                                {
                                    frayteShipmentDetailexcel.Message = "No records found.";
                                }
                            }


                            if ((System.IO.File.Exists(filepath)))
                            {
                                System.IO.File.Delete(filepath);
                            }
                        }
                        else
                        {
                            frayteShipmentDetailexcel.Message = "Excel file not valid";
                        }

                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                        if (ex != null && !string.IsNullOrEmpty(ex.Message) && ex.Message.Contains("Sheet1$"))
                        {
                            frayteShipmentDetailexcel.Message = "Sheet name is invalid.";
                        }
                        else
                        {
                            frayteShipmentDetailexcel.Message = "Error while uploading the excel.";
                        }
                        return frayteShipmentDetailexcel;

                    }
                }
            }
            return frayteShipmentDetailexcel;
        }

        public void GetXMLFromRequestObject(object o)
        {
            dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(o);
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));
        }

        [HttpPost]
        public List<FrayteUserDirectShipment> GetDirectShipmentsList(FrayteTrackDirectBooking track)
        {
            var DirectShipList = new DirectShipmentRepository().GetDirectShipment(track);
            return DirectShipList;
        }

        [HttpPost]
        public IHttpActionResult GenerateTrackAndTraceExcel(FrayteTrackDirectBooking trackdetail)
        {
            FrayteManifestName result = new FrayteManifestName();
            result = new FrayteTrackAndTraceReport().GenerateTrackAndTraceExcel(trackdetail);
            return Ok(result);
        }

        [HttpPost]
        public HttpResponseMessage DownloadTrackAndTraceReport(ReportFile fileName)
        {
            try
            {
                if (fileName != null && !string.IsNullOrEmpty(fileName.FileName))
                {
                    return DownloadTrackAndTraceReport(fileName.FileName);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        [HttpGet]
        public List<ShipmentStatu> GetDirectShipmentStatusList(string BookingType, int UserId)
        {
            var statuslist = new DirectShipmentRepository().GetDirectShipmentStatusList(BookingType, UserId);
            return statuslist;
        }

        [HttpGet]
        public FrayteResult DeleteDirectBooking(int DirectBookingId)
        {
            FrayteResult frayteResult = new FrayteResult();
            frayteResult = new DirectShipmentRepository().DeleteDirectBooking(DirectBookingId);
            return frayteResult;
        }

        [HttpGet]
        public IHttpActionResult CreateZoneRateCard(int OperationZoneId, string CourierCompany, string LogisticType, string RateType, string ModuleType)
        {
            if (LogisticType == FrayteLogisticType.UKShipment)
            {
                new DirectShipmentRepository().CreateUKZoneRateCard(OperationZoneId, CourierCompany, LogisticType, ModuleType);
            }
            else if (LogisticType == FrayteLogisticType.EUExport)
            {
                new DirectShipmentRepository().CreateEUExportImportZoneRateCard(OperationZoneId, CourierCompany, LogisticType, RateType, ModuleType);
            }
            else if (LogisticType == FrayteLogisticType.Import ||
                     LogisticType == FrayteLogisticType.Export ||
                     LogisticType == FrayteLogisticType.ThirdParty)
            {
                new DirectShipmentRepository().CreateZoneRateCard(OperationZoneId, CourierCompany, LogisticType, RateType, ModuleType);
            }


            return Ok();
        }

        [HttpGet]
        public FraytePackageResult PrintLabelAsPDF(int DirectShipmentId, string CourierCompany, string RateType)
        {
            FraytePackageResult result = new FraytePackageResult();
            if (CourierCompany != FrayteCourierCompany.DPD && CourierCompany != FrayteCourierCompany.TNT)
            {
                string TrackingNo = new DirectShipmentRepository().GetTrackingNo(DirectShipmentId);
                var LabelName = new DirectShipmentRepository().GetLogisticLabel(DirectShipmentId);
                string pdfFileName = string.Empty;
                string physycalpath = string.Empty;
                string FileName = string.Empty;

                FrayteLogicalPhysicalPath path = new DirectShipmentRepository().GetShipmentLogisticlabelPath(DirectShipmentId);

                if (path != null)
                {
                    if (path.PhysicalPath.Contains("~"))
                    {
                        // For developement
                        pdfFileName = AppSettings.WebApiPath + "PackageLabel/" + DirectShipmentId + "/" + LabelName;
                        physycalpath = HttpContext.Current.Server.MapPath(path.PhysicalPath + "PackageLabel/" + DirectShipmentId + "/" + LabelName);
                    }
                    else
                    {
                        pdfFileName = path.LogicalPath + "PackageLabel/" + DirectShipmentId + "/" + LabelName;
                        physycalpath = path.PhysicalPath + "/PackageLabel/" + DirectShipmentId + "/" + LabelName;
                    }
                }
                else
                {
                    pdfFileName = AppSettings.WebApiPath + "PackageLabel/" + DirectShipmentId + "/" + LabelName;
                    physycalpath = HttpContext.Current.Server.MapPath("~/PackageLabel/" + DirectShipmentId + "/" + LabelName);
                }
                FileName = LabelName;
                if (File.Exists(physycalpath))
                {
                    result.PackagePath = pdfFileName;
                    result.FileName = FileName;
                    result.IsDownloaded = true;
                }
                else
                {
                    result.PackagePath = "";
                    result.FileName = FileName;
                    result.IsDownloaded = false;
                }
            }
            return result;
        }

        [HttpGet]
        public IHttpActionResult DirectBookingCancel(int DirectShipmentId)
        {
            new ShipmentEmailRepository().SendDirectBookingCancelMail(DirectShipmentId);
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult DirectBookingDelete(int DirectShipmentId)
        {
            FrayteResult result = new DirectShipmentRepository().DeleteDirectBookingInformation(DirectShipmentId);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult DirectBookingReject(string CustomerAction, int DirectShipmentId)
        {
            new ShipmentEmailRepository().SendDirectBookingRejectMail(CustomerAction, DirectShipmentId);
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult DirectBookingDetailWithLabel(string ToMail, int DirectShipmentId, string CourierName, int RoleId, int LogInUserId)
        {
            new ShipmentEmailRepository().SendDirectBookingLabel(ToMail, DirectShipmentId, CourierName, "", RoleId, LogInUserId);
            return Ok();
        }

        [HttpGet]
        public FrayteResult DeletePcsDetail(int DirectShipmentDetailId)
        {
            FrayteResult frayteResult = new DirectShipmentRepository().DeleteDirectShipmetPcsDetail(DirectShipmentDetailId);
            return frayteResult;
        }

        [HttpGet]
        public List<FraytePostCodeAddress> GetPostCodeAddress(string PostCode, string CountryCode)
        {
            var address = new DirectShipmentRepository().PostCodeAddress(PostCode, CountryCode);
            return address;
        }

        [HttpGet]
        public IHttpActionResult CreateCommercialInvoice(int DirectShipmentId, string CustomerName)
        {
            FrayteManifestName result = new FrayteManifestName();
            result = new CommercialInvoice().CreateCommercialInvoce(DirectShipmentId, CustomerName);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult CustomerLogisticService(int CustomerId)
        {
            var service = new DirectShipmentRepository().GetCustomerLogisticService(CustomerId);
            return Ok(service);
        }

        [HttpPost]
        public HttpResponseMessage DownloadrateDirectBookinCommercialInvoice(FrayteCommercialInvoiceFileName FileName)
        {
            try
            {
                if (FileName != null && !string.IsNullOrEmpty(FileName.FileName))
                {
                    return DownloadCommercialInvoice(FileName.FileName);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public IHttpActionResult GenerateSupplementoryCharges(int DirectShipmentId)
        {
            FrayteManifestName result = new FrayteManifestName();
            result = new DirectShipmentRepository().DownloadSupplemantoryChargePdf(DirectShipmentId);
            return Ok(result);
        }

        [HttpPost]
        public HttpResponseMessage DownloadSupplemantoryPdf(FrayteCommercialInvoiceFileName file)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/SupplemantoryChargeFile/" + file.FileName);
            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream filestrm = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[filestrm.Length];
                    filestrm.Read(bytes, 0, (int)filestrm.Length);
                    ms.Write(bytes, 0, (int)filestrm.Length);
                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                    httpResponseMessage.Content = new ByteArrayContent(bytes);
                    httpResponseMessage.Content.Headers.Add("download-status", "downloaded");
                    httpResponseMessage.Content.Headers.Add("x-filename", file.FileName);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = file.FileName;
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    return httpResponseMessage;
                }
            }
        }

        [HttpGet]
        public IHttpActionResult GetCountryStateCode(int CountryId)
        {
            var code = new DirectShipmentRepository().CountryStateCode(CountryId);
            return Ok(code);
        }

        [HttpGet]
        public IHttpActionResult MarkDraftShipmentAsPublic(int DirectShipmentDraftId, bool IsPublic)
        {
            bool value = new DirectShipmentRepository().MarkDraftShipmentAsPublic(DirectShipmentDraftId, IsPublic);
            return Ok(value);
        }

        [HttpGet]
        public FrayteCountryCurrentDateTime CountryCurrentDateTime(int CountryId)
        {
            FrayteCountryCurrentDateTime datetime = new DirectShipmentRepository().GetCurrentDateTime(CountryId);
            return datetime;
        }

        #region -- Get Tracking Detail of a single tracking number  --

        public IHttpActionResult GetTracking(string CarrierName, string TrackingNumber)
        {
            try
            {
                var tracker = new DirectShipmentRepository().GetTracking(CarrierName, TrackingNumber);
                return Ok(tracker);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return null;
        }

        #endregion

        #region -- Get tracking detail for per customer --

        [HttpGet]
        public IHttpActionResult GetMultipleTrackings(int userId)
        {
            try
            {
                var tracker = new DirectShipmentRepository().GetMultipleTrackings(userId);
                return Ok(tracker);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return null;
        }

        #endregion

        #region -- Get track and trace dashboard for per customer --

        [HttpPost]
        public IHttpActionResult TrackAndTraceDashboard(TrackAfterShipTracking track)
        {
            try
            {
                var tracker = new DirectShipmentRepository().GetMultipleTrackings(track);
                return Ok(tracker);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return null;
        }

        #endregion

        #region -- WebHook --

        [HttpPost]
        public IHttpActionResult ProcessWebHook(AftershipWebhookObject webHookDetail)
        {
            dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(webHookDetail);
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));

            FrayteResult result = new DirectShipmentRepository().SendTrackingStatusEmail(webHookDetail);
            if (result.Status)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }
        }

        #endregion

        #region -- Private Method --

        private HttpResponseMessage DownloadCommercialInvoice(string FileName)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/ReportFiles/" + FileName);
            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    ms.Write(bytes, 0, (int)file.Length);
                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                    httpResponseMessage.Content = new ByteArrayContent(bytes);
                    httpResponseMessage.Content.Headers.Add("download-status", "downloaded");
                    httpResponseMessage.Content.Headers.Add("x-filename", FileName);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = FileName;
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    return httpResponseMessage;
                }
            }
        }

        private HttpResponseMessage DownloadTrackAndTraceReport(string fileName)
        {

            string filePath = HttpContext.Current.Server.MapPath("~/ReportFiles/Track&Trace/" + fileName);
            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    ms.Write(bytes, 0, (int)file.Length);
                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                    httpResponseMessage.Content = new ByteArrayContent(bytes);
                    httpResponseMessage.Content.Headers.Add("download-status", "downloaded");
                    httpResponseMessage.Content.Headers.Add("x-filename", fileName);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = fileName;
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    return httpResponseMessage;
                }
            }

        }

        private string Generate_PackageLabelPDF(int DirectShipmentId, string CourierCompany, string RateType)
        {
            string pageStyleSheet = HttpContext.Current.Server.MapPath("~/Content/") + "print.css";
            string bootStrapStyleSheet = HttpContext.Current.Server.MapPath("~/Content/") + "bootstrap.min.css";
            string pdfLogo = HttpContext.Current.Server.MapPath("~/Content/") + "FrayteLogo.png";

            var detail = new DirectShipmentRepository().GetPackageList(DirectShipmentId, CourierCompany, RateType);
            string TrackingNo = new DirectShipmentRepository().GetTrackingNo(DirectShipmentId);
            FrayteShipmentPackageLabel obj = new FrayteShipmentPackageLabel();
            obj.PackageLabel = new List<FraytePackageLabel>();
            obj.PackageLabel = detail;
            obj.PageStyleSheet = pageStyleSheet;
            obj.BootStrapStyleSheet = bootStrapStyleSheet;
            obj.pdfLogo = pdfLogo;

            string template15 = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/PrintLabel.cshtml"));
            var templateService = new TemplateService();
            var EmailBody = Engine.Razor.RunCompile(template15, "PackageLabel_15", null, obj);

            string pdfFileName = string.Empty;
            if (!string.IsNullOrEmpty(RateType))
            {
                pdfFileName = CourierCompany.Replace(" ", "") + "" + RateType.Replace(" ", "") + "-" + TrackingNo + ".html";
            }
            else
            {
                pdfFileName = CourierCompany.Replace(" ", "") + "-" + TrackingNo + ".html";
            }

            string pdfFilePath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");
            string pdfFullPath = pdfFilePath + @"\" + pdfFileName;

            using (FileStream fs = new FileStream(pdfFullPath, FileMode.Create))
            {
                using (StreamWriter w = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {
                    w.WriteLine(EmailBody);
                }
            }

            List<string> lstHtmlFiles = new List<string>();
            lstHtmlFiles.Add(pdfFullPath);

            //Before creating new PDF file, remove the earlier one.            
            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.LabelFolder + DirectShipmentId + "/"));
            string pdfPath = string.Empty;

            if (!string.IsNullOrEmpty(RateType))
            {
                pdfPath = HttpContext.Current.Server.MapPath("~/PackageLabel/" + DirectShipmentId + "/") + CourierCompany.Replace(" ", "") + "" + RateType.Replace(" ", "") + "-" + TrackingNo + ".pdf";
            }
            else
            {
                pdfPath = HttpContext.Current.Server.MapPath("~/PackageLabel/" + DirectShipmentId + "/") + CourierCompany.Replace(" ", "") + "-" + TrackingNo + ".pdf";
            }

            if (File.Exists(pdfPath))
            {
                File.Delete(pdfPath);
            }

            string pdfFile = string.Empty;
            if (!string.IsNullOrEmpty(RateType))
            {
                pdfFile = Frayte.WebApi.Utility.PDFGenerator.HtmlToPdf("~/PackageLabel/" + DirectShipmentId + "/", CourierCompany.Replace(" ", "") + "" + RateType.Replace(" ", "") + "-" + TrackingNo, lstHtmlFiles.ToArray(), null);
            }
            else
            {
                pdfFile = Frayte.WebApi.Utility.PDFGenerator.HtmlToPdf("~/PackageLabel/" + DirectShipmentId + "/", CourierCompany.Replace(" ", "") + "-" + TrackingNo, lstHtmlFiles.ToArray(), null);
            }

            if (System.IO.File.Exists(pdfFullPath))
            {
                System.IO.File.Delete(pdfFullPath);
            }

            return pdfFile;
        }

        #endregion
    }
}