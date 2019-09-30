using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using Frayte.Services.Models.Express;
using System.IO;
using Frayte.Services.DataAccess;
using Frayte.Services.Models.Tradelane;
using System.Web;
using Frayte.Services.Models.Aftership;
using Frayte.Services.Models.DPD;
using Frayte.Services.Models.DPD_CH;
using Frayte.Services.Models.Bring;
using Frayte.Services.Models.SKYPOSTAL;
using Frayte.Services.Models.ETower;
using Frayte.Services.Models.USPS;
using Report.Generator.ManifestReport;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Frayte.WebApi.Controllers
{
    public class ExpressController : ApiController
    {
        #region Shipmenet initials  

        [HttpGet]
        public IHttpActionResult AWBlabelPath(string AWBNumber)
        {
            ExpressAwbLabel label = new ExpressAwbLabel();
            label.path = new ExpressRepository().AWBlabelPath(AWBNumber);
            label.physicalPath = HttpContext.Current.Server.MapPath("~/AwbImage/") + AWBNumber.Replace(" ", "") + ".jpg";
            if (File.Exists(label.physicalPath))
            {
                return Ok(label.path);
            }
            else
            {
                return Ok("");
            }
        }

        [HttpPost]
        public List<HubService> ExpressHubServices(ExpressServiceRequestModel serviceObj)
        {
            List<HubService> services = new ExpressRepository().GetExpressServices(serviceObj);
            return services;
        }

        [HttpGet]
        public IHttpActionResult GetHubAddress(int countryId, string postcode, string state)
        {
            try
            {
                ExpressAddressModel address = new ExpressRepository().getHubAddress(countryId, postcode, state);
                return Ok(address);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public IHttpActionResult GetCustomerAWBs(int customerId, string AWB)
        {

            var awbs = new ExpressRepository().GetCustomerAWBs(customerId, AWB);
            return Ok(awbs);
        }

        [HttpGet]
        public HttpResponseMessage BookingInitials(int userId)
        {
            //Step 1: Get Country list
            List<FrayteCountryCode> lstCountry = new List<FrayteCountryCode>();
            lstCountry = new CountryRepository().lstCountry();

            //Step 2: Get Shipment Hanler Methods
            var lstShipmentHandlerMethods = new MasterDataRepository().GetShipmentHandlerMethod();

            //Step 3: Get Currency
            var lstCurrencyTypes = new MasterDataRepository().GetCurrencyType();

            //Step4: Get Customer Detail
            var customerDetail = new TradelaneBookingRepository().GetCustomerDetail(userId);
            //Step4.1: Get Customer Detail
            var customerAddress = new TradelaneBookingRepository().UserDefaultAddresses(userId);

            // Step 5 : Get Country PhoneCode
            List<FrayteCountryPhoneCode> lstCountryPhones = new List<FrayteCountryPhoneCode>();

            lstCountryPhones = new CountryRepository().GetCountryPhoneCodeList();

            // step:6 get all operation zones
            var OperaionZones = new OperationZoneRepository().GetOperationZone();

            // step:7 get all Airlines 
            var listAirlines = new MasterDataRepository().GetAirlines();

            var timeZones = new TimeZoneRepository().GetShipmentTimeZones();

            MasterDataRepository mdRepository = new MasterDataRepository();
            //Step 2: Get Parcel Type
            var lstParcelTypes = mdRepository.GetParcelType();


            return this.Request.CreateResponse(
                HttpStatusCode.OK,
                new
                {
                    Countries = lstCountry,
                    CurrencyTypes = lstCurrencyTypes,
                    ShipmentMethods = lstShipmentHandlerMethods,
                    CustomerDetail = customerDetail,
                    CustomerAddress = customerAddress,
                    CountryPhoneCodes = lstCountryPhones,
                    AirLines = listAirlines,
                    TimeZones = timeZones,
                    OperaionZones = OperaionZones,
                    ParcelTypes = lstParcelTypes,
                });
        }

        public List<TradelaneCustomer> GetCustomers(int userId, string moduleType)
        {
            try
            {
                List<TradelaneCustomer> customers = new ExpressRepository().GetCustomers(userId, moduleType);
                return customers;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public List<ExpressState> GetCountryState(int CountryId)
        {
            try
            {
                List<ExpressState> States = new ExpressRepository().GetCountryState(CountryId);
                return States;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public List<ExpressState> GetFromCountryState(int CountryId)
        {
            try
            {
                List<ExpressState> States = new ExpressRepository().GetFromCountryState(CountryId);
                return States;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion 

        #region Place Booking 

        public ExpressShipmentModel SaveShipment(ExpressShipmentModel shipment)
        {
            ExpressShipmentModel model = new ExpressRepository().SaveShipment(shipment);

            if (shipment.Error.Status && shipment.ShipmentStatusId == (int)FrayteExpressShipmentStatus.Current)
            {
                if (shipment.Service.HubCarrier == FrayteCourierCompany.DHL)
                {
                    #region DHL Integration

                    IntegrtaionResult result = new IntegrtaionResult();
                    //step 1. Mapping With express model to ShipmentRequestDto 

                    var shipmentRequestDto = new DHLRepository().MapExpressShipmentToDHLShipmentRequestDto(shipment);

                    //Step 2. Create Xml
                    string shipmentXML = string.Empty;
                    //Version5.0
                    shipmentXML = new DHLRepository().CreateXMLForDHL(shipmentRequestDto);

                    //Version6.2 not in use
                    string xml_in = File.ReadAllText(@shipmentXML);

                    //step 3. Create Shipment
                    var DHLResponse = new DHLRepository().CreateShipment(xml_in, shipmentRequestDto);

                    //step 4. Mapping ShipmentResonse to IntegrtaionResult
                    result = new DHLRepository().MapDHLIntegrationResponse(DHLResponse);

                    if (result.Status)
                    {
                        if (shipment.Service.HubCarrier == FrayteCourierCompany.DHL)
                        {
                            //Step 4.1 Mapping ShipmentResult to IntegrtaionResult
                            new ExpressRepository().MappingCourierPieceDetail(result, shipment);
                        }
                        //Step 4.2 : Save Main Tracking Number
                        new ExpressRepository().SaveMainTrackingDetail(shipment, result, xml_in, "");

                        // AfterShipIntegration
                        if (AppSettings.ApplicationMode == FrayteApplicationMode.Live)
                        {
                            FrayteAfterShipTracking aftershipTracking = new AftershipTrackingRepository().MapDirectShipmentObjToAfterShip(shipment.ExpressId, FrayteShipmentServiceType.Express);

                            if (aftershipTracking != null && AppSettings.ApplicationMode == FrayteApplicationMode.Live)
                            {
                                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Integration in aftership"));
                                new AftershipTrackingRepository().CreateTracking(aftershipTracking);
                            }
                        }

                        //Step 5:  //Start downloading the images from DHL server and making PDF
                        var count = 1;
                        var totalpiece = result.PieceTrackingDetails.Count();
                        totalpiece = totalpiece - 1;

                        foreach (var data in result.PieceTrackingDetails.Take(totalpiece))
                        {
                            if (shipment.Service.HubCarrier == FrayteCourierCompany.DHL)
                            {
                                // Step 5.1 
                                data.LabelName = new DHLRepository().ExpressDownloadDHLImage(data, totalpiece, count, shipment.ExpressId);
                            }
                            if (!data.PieceTrackingNumber.Contains("AirwayBillNumber_"))
                            {
                                //Step 5.2
                                new ExpressRepository().SavePackageDetail(data, result.CourierName);
                            }
                            count++;
                        }
                        var CourierPieceDetail = result.PieceTrackingDetails.Where(t => t.PieceTrackingNumber.Contains("AirwayBillNumber_")).FirstOrDefault();
                        if (CourierPieceDetail != null)
                        {
                            var data1 = new DHLRepository().ExpressDownloadDHLImage(CourierPieceDetail, totalpiece, 0, shipment.ExpressId);

                            // save all dhl image  
                            new ExpressRepository().SaveLogisticLabelImage(shipment.ExpressId, data1);
                        }
                        if (result.Status)
                        {
                            var status = Generate_Seperate_PackageLabelPDF(shipment.ExpressId, shipment, result);

                            // Send Booking confirmation email   
                            if (status.Status)
                            {
                                ExpressEmailModel emailModel = new ExpressEmailModel();
                                emailModel = new ExpressRepository().Fill_EXS_E1Model(emailModel, shipment);
                                if (emailModel != null)
                                {
                                    new ExpressRepository().SendEmail_EXS_E1(emailModel);
                                }
                            }
                        }
                    }
                    else
                    {
                        shipment.Error = result.Error;
                    }

                    #endregion
                }
                if (shipment.Service.HubCarrier == FrayteCourierCompany.DPD)
                {
                    #region DPD Integration

                    IntegrtaionResult result = new IntegrtaionResult();
                    string xml_in = string.Empty;
                    if (shipment.Service.HubCarrier == FrayteCourierCompany.DPD)
                    {
                        //step 1. Mapping With directBookingDetail to ShipmentRequestDto
                        DPDRequestModel shipmentRequestDto = new DPDRepository().MapExpressBookingDetailToShipmentRequestDto(shipment);

                        //Create Shipment
                        var shipmentResult = new DPDRepository().CreateShipment(shipmentRequestDto);

                        //Mapping ShipmentResonse to IntegrtaionResult
                        result = new DPDRepository().MapDPDIntegrationResponse(shipmentResult);
                    }

                    if (result.Status)
                    {
                        if (shipment.Service.HubCarrier == FrayteCourierCompany.DPD)
                        {
                            //Mapping ShipmentResult to IntegrtaionResult
                            new ExpressRepository().MappingCourierPieceDetail(result, shipment);
                        }

                        //Step3 : Save Main Tracking Number
                        new ExpressRepository().SaveMainTrackingDetail(shipment, result, xml_in, "");

                        //Start downloading the images from DPD server and making PDF
                        var count = 1;
                        foreach (var data in result.PieceTrackingDetails)
                        {
                            if (shipment.Service.HubCarrier == FrayteCourierCompany.DPD)
                            {
                                //Step3.1
                                data.LabelName = new DPDRepository().ExpressDownloadDPDHtmlImage(data, result.PieceTrackingDetails.Count(), count, shipment.ExpressId, shipment.Service.HubCarrierDisplay);
                            }

                            //Step3.2
                            new ExpressRepository().SavePackageDetail(data, result.CourierName);
                            count++;
                        }

                        //Step:4 Generate PDF
                        Generate_Seperate_PackageLabelPDF(shipment.ExpressId, shipment, result);
                        shipment.Error = new FratyteError()
                        {
                            Status = result.Status,
                        };
                    }
                    else
                    {
                        shipment.Error = result.Error;
                    }

                    #endregion
                }
                if (shipment.Service.HubCarrier == FrayteCourierCompany.DPDCH)
                {
                    #region DPDCH  Integration

                    IntegrtaionResult result = new IntegrtaionResult();
                    string xml_in = string.Empty;
                    var shipmentResult = new DPDChResponseModel();
                    if (shipment.Service.HubCarrier == FrayteCourierCompany.DPDCH)
                    {
                        //step 1. Mapping With directBookingDetail to ShipmentRequestDto
                        DPDChRequestModel shipmentRequestDto = new DPDSwitzerlandRepository().MapExpressBookingDetailToShipmentRequestDto(shipment);

                        //Create Shipment
                        shipmentResult = new DPDSwitzerlandRepository().CreateShipment(shipmentRequestDto, FrayteShipmentServiceType.Express);

                        //Mapping ShipmentResonse to IntegrtaionResult
                        result = new DPDSwitzerlandRepository().MapDPDCHIntegrationResponse(shipmentResult);
                    }

                    if (result.Status)
                    {
                        if (shipment.Service.HubCarrier == FrayteCourierCompany.DPDCH)
                        {
                            //Step 1.1 Mapping ShipmentResult to IntegrtaionResult
                            new ExpressRepository().MappingCourierPieceDetail(result, shipment);
                        }

                        //Step3 : Save Main Tracking Number
                        new ExpressRepository().SaveMainTrackingDetail(shipment, result, shipmentResult.Request, shipmentResult.Response);

                        //Start downloading the images from DPD server and making PDF
                        var count = 1;
                        foreach (var data in result.PieceTrackingDetails)
                        {
                            if (shipment.Service.HubCarrier == FrayteCourierCompany.DPDCH)
                            {
                                //Step3.1
                                data.LabelName = new DPDSwitzerlandRepository().DownloadExpressDPDCHBytetoImage(data, result.PieceTrackingDetails.Count(), count, shipment.ExpressId);
                            }

                            //Step3.2
                            new ExpressRepository().SavePackageDetail(data, result.CourierName);
                            count++;
                        }
                        //Step:4 Generate PDF
                        Generate_Seperate_PackageLabelPDF(shipment.ExpressId, shipment, result);

                        // Send Booking confirmation email   
                        if (result.Status)
                        {
                            ExpressEmailModel emailModel = new ExpressEmailModel();
                            emailModel = new ExpressRepository().Fill_EXS_E1Model(emailModel, shipment);
                            if (emailModel != null)
                            {
                                new ExpressRepository().SendEmail_EXS_E1(emailModel);
                            }
                        }
                        shipment.Error = new FratyteError()
                        {
                            Status = result.Status,
                        };
                    }
                    else
                    {
                        shipment.Error = result.Error;
                    }

                    #endregion
                }
                if (shipment.Service.HubCarrier == FrayteCourierCompany.BRING)
                {
                    #region BRING Integration

                    IntegrtaionResult result = new IntegrtaionResult();
                    BringResponseModel shipmentResult = new BringResponseModel();
                    if (shipment.Service.HubCarrier == FrayteCourierCompany.BRING)
                    {
                        BringRequestModel shipmentRequestDto = new BringRepository().MapExpressBookingDetailToShipmentRequestDto(shipment);

                        //Create Shipment
                        shipmentResult = new BringRepository().CreateShipment(shipmentRequestDto, shipment.ExpressId, FrayteShipmentServiceType.Express);
                        result = new BringRepository().MapBringIntegrationResponse(shipmentResult);
                    }

                    if (result.Status)
                    {
                        if (shipment.Service.HubCarrier == FrayteCourierCompany.BRING)
                        {
                            //Mapping ShipmentResult to IntegrtaionResult
                            new ExpressRepository().MappingCourierPieceDetail(result, shipment);
                        }

                        //Save Package Label Tracking detail
                        new ExpressRepository().SaveMainTrackingDetail(shipment, result, shipmentResult.request, shipmentResult.response);

                        //Start downloading the images from DPD server and making PDF
                        var count = 1;
                        foreach (var data in result.PieceTrackingDetails)
                        {
                            if (shipment.Service.HubCarrier == FrayteCourierCompany.BRING)
                            {
                                //Step3.1
                                data.LabelName = new BringRepository().ExpressDownloadBringlabelPDF(data, result.PieceTrackingDetails.Count(), count, shipment.ExpressId);
                            }
                            new ExpresShipmentRepository().SavePackageDetail(data, result.CourierName);
                            count++;
                        }

                        //Step:4 Generate PDF
                        Generate_Seperate_PackageLabelPDF(shipment.ExpressId, shipment, result);

                        // Send Booking confirmation email   
                        if (result.Status)
                        {
                            ExpressEmailModel emailModel = new ExpressEmailModel();
                            emailModel = new ExpressRepository().Fill_EXS_E1Model(emailModel, shipment);
                            if (emailModel != null)
                            {
                                new ExpressRepository().SendEmail_EXS_E1(emailModel);
                            }
                        }

                        shipment.Error = new FratyteError()
                        {
                            Status = result.Status,
                        };
                    }
                    else
                    {
                        shipment.Error = result.Error;
                    }

                    #endregion
                }
                if (shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.SKYPOSTAL)
                {
                    #region SkyPostal Integration

                    SkyPostalRequest request = new SkyPostalRequest();
                    SkyPostalResponse response = new SkyPostalResponse();
                    IntegrtaionResult result = new IntegrtaionResult();

                    if (shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.SKYPOSTAL)
                    {
                        request = new SkyPostalRepository().MapExpressBookingDetailToShipmentRequestDto(shipment);

                        // Create Shipment
                        response = new SkyPostalRepository().CreateShipment(request, shipment.ExpressId);
                        result = new SkyPostalRepository().MapSkyPostalIntegrationResponse(response);
                    }

                    if (result.Status)
                    {
                        if (shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.SKYPOSTAL)
                        {
                            //Mapping ShipmentResult to IntegrtaionResult
                            new SkyPostalRepository().MappingExpressCourierPieceDetail(result, shipment, shipment.ExpressId);
                        }

                        //Save Package Label Tracking detail
                        new ExpressRepository().SaveMainTrackingDetail(shipment, result, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response));

                        //Start downloading the images from DPD server and making PDF
                        var count = 1;
                        foreach (var data in result.PieceTrackingDetails)
                        {
                            if (shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.SKYPOSTAL)
                            {
                                //Step3.1
                                data.LabelName = new SkyPostalRepository().DownloadExpressSkyPostalImage(data, result.PieceTrackingDetails.Count(), count, shipment.ExpressId);
                            }
                            new ExpresShipmentRepository().SavePackageDetail(data, result.CourierName);
                            count++;
                        }

                        //Step:4 Generate PDF
                        Generate_Seperate_PackageLabelPDF(shipment.ExpressId, shipment, result);

                        // Send Booking confirmation email   
                        if (result.Status)
                        {
                            ExpressEmailModel emailModel = new ExpressEmailModel();
                            emailModel = new ExpressRepository().Fill_EXS_E1Model(emailModel, shipment);
                            if (emailModel != null)
                            {
                                new ExpressRepository().SendEmail_EXS_E1(emailModel);
                            }
                        }

                        shipment.Error = new FratyteError()
                        {
                            Status = result.Status,
                        };
                    }
                    else
                    {
                        shipment.Error = result.Error;
                    }

                    #endregion 
                }
                if (shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.USPS)
                {
                    #region USPS Integration

                    USPSRequest request = new USPSRequest();
                    USPSResponse response = new USPSResponse();
                    IntegrtaionResult result = new IntegrtaionResult();

                    if (shipment.Service.HubCarrier == FrayteCourierCompany.USPS)
                    {
                        request = new USPSRepository().MapExpressShipmentToUSPSShipmentRequest(shipment);

                        //Create Shipment
                        response = new USPSRepository().CreateShipment(request, shipment.ExpressId);
                        result = new USPSRepository().MapUSPSIntegrationResponse(response);
                    }

                    if (result.Status)
                    {
                        //Mapping ShipmentResult to IntegrtaionResult
                        new USPSRepository().MappingExpressCourierPieceDetail(result, shipment, shipment.ExpressId);

                        //Save Package Label Tracking detail
                        new ExpressRepository().SaveMainTrackingDetail(shipment, result, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response));

                        //Start downloading the images from DPD server and making PDF
                        var count = 1;
                        foreach (var data in result.PieceTrackingDetails)
                        {
                            //Step3.1 
                            data.LabelName = new USPSRepository().DownloadExpressUSPSImage(data, result.PieceTrackingDetails.Count(), count, shipment.ExpressId);

                            //Save logistic label image if receive image in response
                            new ExpressRepository().SaveLogisticLabelImage(shipment.ExpressId, data.LabelName);
                            new ExpresShipmentRepository().SavePackageDetail(data, result.CourierName);
                            count++;
                        }

                        //Step:4 Generate PDF
                        Generate_Seperate_PackageLabelPDF(shipment.ExpressId, shipment, result);

                        // Send Booking confirmation email   
                        if (result.Status)
                        {
                            ExpressEmailModel emailModel = new ExpressEmailModel();
                            emailModel = new ExpressRepository().Fill_EXS_E1Model(emailModel, shipment);
                            if (emailModel != null)
                            {
                                new ExpressRepository().SendEmail_EXS_E1(emailModel);
                            }
                        }

                        shipment.Error = new FratyteError()
                        {
                            Status = result.Status,
                        };
                    }
                    else
                    {
                        shipment.Error = result.Error;
                    }

                    #endregion
                }
                if (shipment.Service.HubCarrier == FrayteCourierCompany.DomesticA || shipment.Service.HubCarrier == FrayteCourierCompany.EAMExpress)
                {
                    #region EAM Global Integration

                    IntegrtaionResult result = new IntegrtaionResult();
                    if (shipment.Service.HubCarrier == FrayteCourierCompany.DomesticA || shipment.Service.HubCarrier == FrayteCourierCompany.EAMExpress)
                    {
                        var shipmentRequestDto = new EAMGlobalRepository().MapExpressBookingDetailToShipmentRequestDto(shipment);
                        string shipmentXML = new EAMGlobalRepository().CreateXMLForEAM(shipmentRequestDto);
                        string xml_in = File.ReadAllText(@shipmentXML);

                        //Create Shipment
                        var shipmentResult = new EAMGlobalRepository().CreateShipment(xml_in, shipment.ExpressId);
                        result = new EAMGlobalRepository().MapExpressEAMGlobalIntegrationResponse(shipmentResult, shipment.Packages, shipment.Service);
                    }

                    if (result.Status)
                    {
                        if (shipment.Service.HubCarrier == FrayteCourierCompany.DomesticA || shipment.Service.HubCarrier == FrayteCourierCompany.EAMExpress)
                        {
                            //Mapping ShipmentResult to IntegrtaionResult
                            new ExpressRepository().MappingCourierPieceDetail(result, shipment);
                        }

                        //Save Package Label Tracking detail
                        new ExpressRepository().SaveMainTrackingDetail(shipment, result, "", "");

                        //Start downloading the images from DPD server and making PDF
                        var count = 1;
                        foreach (var data in result.PieceTrackingDetails)
                        {
                            if (shipment.Service.HubCarrier == FrayteCourierCompany.DomesticA || shipment.Service.HubCarrier == FrayteCourierCompany.EAMExpress)
                            {
                                //Step3.1
                                data.LabelName = new EAMGlobalRepository().DownloadExpressEAMImageTOPDF(data, result.PieceTrackingDetails.Count(), count, shipment.ExpressId, shipment.Service);
                            }
                            // Step3.2
                            new ExpressRepository().SavePackageDetail(data, result.CourierName);
                            count++;
                        }
                        //Step:4 Generate PDF
                        Generate_Seperate_PackageLabelPDF(shipment.ExpressId, shipment, result);
                        if (result.Status)
                        {
                            ExpressEmailModel emailModel = new ExpressEmailModel();
                            emailModel = new ExpressRepository().Fill_EXS_E1Model(emailModel, shipment);
                            if (emailModel != null)
                            {
                                new ExpressRepository().SendEmail_EXS_E1(emailModel);
                            }
                        }

                        shipment.Error = new FratyteError()
                        {
                            Status = result.Status,
                        };
                    }
                    else
                    {
                        shipment.Error = result.Error;
                    }

                    #endregion
                }
                if (shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.CANADAPOST)
                {
                    #region Canada Post Integration

                    IntegrtaionResult result = new IntegrtaionResult();
                    EtowerResponseModel shipmentResult = new EtowerResponseModel();
                    if (shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.CANADAPOST)
                    {
                        var shipmentRequestDto = new ETowerRepository().MapExpressBookingDetailToShipmentRequestDto(shipment);

                        //Create Shipment
                        shipmentResult = new ETowerRepository().CreateShipment(shipmentRequestDto, shipment.ExpressId);
                        result = new ETowerRepository().MapCanadaPostIntegrationResponse(shipmentResult, shipmentRequestDto[0].orderItems);
                    }

                    if (result.Status)
                    {
                        if (shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.CANADAPOST)
                        {
                            //Mapping ShipmentResult to IntegrtaionResult
                            new ETowerRepository().MappingExpressCourierPieceDetail(result, shipment.ExpressId);
                        }

                        new ExpressRepository().SaveMainTrackingDetail(shipment, result, shipmentResult.Request, shipmentResult.Response);

                        //Start downloading the images from DPD server and making PDF
                        var count = 1;
                        foreach (var data in result.PieceTrackingDetails)
                        {
                            if (shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.CANADAPOST)
                            {
                                //Step3.1
                                data.LabelName = new ETowerRepository().DownloadExpressCanadaPostBytetoImage(data, result.PieceTrackingDetails.Count(), count, shipment.ExpressId);
                            }
                            // Step3.2
                            new ExpressRepository().SavePackageDetail(data, result.CourierName);
                            count++;
                        }
                        //Step:4 Generate PDF
                        Generate_Seperate_PackageLabelPDF(shipment.ExpressId, shipment, result);

                        // Send Booking confirmation email   
                        if (result.Status)
                        {
                            ExpressEmailModel emailModel = new ExpressEmailModel();
                            emailModel = new ExpressRepository().Fill_EXS_E1Model(emailModel, shipment);
                            if (emailModel != null)
                            {
                                new ExpressRepository().SendEmail_EXS_E1(emailModel);
                            }
                        }
                        shipment.Error = new FratyteError()
                        {
                            Status = result.Status,
                        };
                    }
                    else
                    {
                        shipment.Error = result.Error;
                    }

                    #endregion
                }

                // Avinash
                // 17-Apr-2019

                if (shipment.Service.HubCarrier == FrayteCourierCompany.Yodel)
                {
                    #region Yodel Integration

                    //Map Direct Booking object with parcel hub objects
                    Frayte.Services.Models.ParcelHub.ParcelHubShipmentRequest request = new ParcelHubRepository().MapExpressBookingDetailToShipmentRequest(shipment);
                    //Create shipment in Parcel hub
                    Frayte.Services.Models.ParcelHub.ParcelHubResponse response = new ParcelHubRepository().CreateShipment(request);

                    if (response.Error.IsMailSend)
                    {
                        //Send error mail to developer
                        shipment.Error = new FratyteError();
                        shipment.Error.Custom = new List<string>();
                        shipment.Error.Package = new List<string>();
                        shipment.Error.Address = new List<string>();
                        shipment.Error.Service = new List<string>();
                        shipment.Error.ServiceError = new List<string>();
                        shipment.Error.Miscellaneous = new List<string>();
                        shipment.Error.MiscErrors = new List<FrayteKeyValue>();
                        shipment.Error = response.Error;

                        new DirectShipmentRepository().SaveExpressEasyPosyPickUpObject(Newtonsoft.Json.JsonConvert.SerializeObject(response.Error).ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(shipment).ToString(), shipment.ExpressId);
                    }
                    else
                    {
                        //Mapping ShipmentResonse to IntegrtaionResult
                        var result = new ParcelHubRepository().MappingExpressParcelHubToIntegrationResult(shipment, response, null);

                        if (result.Status)
                        {
                            //Mapping ShipmentResult to IntegrtaionResult
                            new ParcelHubRepository().MappingExpressCourierPieceDetail(result, shipment, shipment.ExpressId);

                            //Save Package Label Tracking detail
                            new ExpressRepository().SaveMainTrackingDetail(shipment, result, response.Request, response.Response);

                            FratyteError Error = new ParcelHubRepository().DownloadExpressParcelHubPackageImage(shipment, result, shipment.ExpressId);

                            //Start Making Final One pdf file for all package label
                            Generate_Seperate_PackageLabelPDF(shipment.ExpressId, shipment, result);

                            if (result.Status)
                            {
                                ExpressEmailModel emailModel = new ExpressEmailModel();
                                emailModel = new ExpressRepository().Fill_EXS_E1Model(emailModel, shipment);
                                if (emailModel != null)
                                {
                                    new ExpressRepository().SendEmail_EXS_E1(emailModel);
                                }
                            }
                            shipment.Error = new FratyteError()
                            {
                                Status = result.Status,
                            };
                        }
                        else
                        {
                            if (result.Error.IsMailSend)
                            {
                                //Send error mail to developer
                                shipment.Error = new FratyteError();
                                shipment.Error.Custom = new List<string>();
                                shipment.Error.Package = new List<string>();
                                shipment.Error.Address = new List<string>();
                                shipment.Error.Service = new List<string>();
                                shipment.Error.ServiceError = new List<string>();
                                shipment.Error.Miscellaneous = new List<string>();
                                shipment.Error.MiscErrors = new List<FrayteKeyValue>();
                                shipment.Error = result.Error;
                            }
                        }
                    }

                    #endregion
                }
                if (shipment.Service.HubCarrier == FrayteCourierCompany.Hermes)
                {
                    #region Hermes Integration

                    shipment.Error = new FratyteError();
                    shipment.Error.Custom = new List<string>();
                    shipment.Error.Package = new List<string>();
                    shipment.Error.Address = new List<string>();
                    shipment.Error.Service = new List<string>();
                    shipment.Error.ServiceError = new List<string>();
                    shipment.Error.Miscellaneous = new List<string>();
                    shipment.Error.MiscErrors = new List<FrayteKeyValue>();

                    Frayte.Services.Models.ParcelHub.ParcelHubShipmentRequest request = new ParcelHubRepository().MapExpressBookingDetailToShipmentRequest(shipment);
                    //Create shipment in Parcel hub
                    Frayte.Services.Models.ParcelHub.ParcelHubResponse response = new ParcelHubRepository().CreateHermesShipment(request);

                    if (response.Error.IsMailSend)
                    {
                        //Send error mail to developer
                        shipment.Error = response.Error;
                        new DirectShipmentRepository().SaveExpressEasyPosyPickUpObject(Newtonsoft.Json.JsonConvert.SerializeObject(response.Error).ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(shipment).ToString(), shipment.ExpressId);
                    }
                    else
                    {
                        //Mapping ShipmentResonse to IntegrtaionResult
                        var result = new ParcelHubRepository().MappingExpressParcelHubToIntegrationResult(shipment, response, null);

                        if (result.Status)
                        {
                            //Mapping ShipmentResult to IntegrtaionResult
                            new ParcelHubRepository().MappingExpressCourierPieceDetail(result, shipment, shipment.ExpressId);

                            //Save Package Label Tracking detail
                            new ExpressRepository().SaveMainTrackingDetail(shipment, result, response.Request, response.Response);

                            //Start Making Final One pdf file for all package label
                            Generate_Seperate_PackageLabelPDF(shipment.ExpressId, shipment, result);

                            if (result.Status)
                            {
                                ExpressEmailModel emailModel = new ExpressEmailModel();
                                emailModel = new ExpressRepository().Fill_EXS_E1Model(emailModel, shipment);
                                if (emailModel != null)
                                {
                                    new ExpressRepository().SendEmail_EXS_E1(emailModel);
                                }
                            }
                            shipment.Error = new FratyteError()
                            {
                                Status = result.Status,
                            };
                        }
                        else
                        {
                            if (result.Error.IsMailSend)
                            {
                                shipment.Error = result.Error;
                            }
                            else
                            {                               
                                shipment.Error = result.Error;
                            }
                        }
                    }
                    #endregion
                }
                if (shipment.Service.HubCarrier == FrayteCourierCompany.FrayteDomesticJP || shipment.Service.HubCarrier == FrayteCourierCompany.DomesticB)
                {
                    #region FrayteDomestic Integration

                    try
                    {
                        shipment.TrackingNo = shipment.AWBNumber;
                        new ExpressRepository().UpdateExpress(shipment.ExpressId, shipment.Service.HubCarrierServiceId, shipment.TrackingNo);
                        new FrayteMAWBReport().FrayteMAWB(shipment, "EXS");

                        ExpressEmailModel emailModel = new ExpressEmailModel();
                        emailModel = new ExpressRepository().Fill_EXS_E1Model(emailModel, shipment);
                        if (emailModel != null)
                        {
                            new ExpressRepository().SendEmail_EXS_E1(emailModel);
                        }
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }

                    #endregion
                }
            }
            return shipment;
        }

        public FrayteResult Generate_Seperate_PackageLabelPDF(int DirectShipmentId, ExpressShipmentModel shipment, IntegrtaionResult integrtaionResult)
        {
            FrayteResult result = new FrayteResult();
            result.Errors = new List<string>();

            List<string> list = new List<string>();
            List<string> list1 = new List<string>();

            var Packages = new ExpressRepository().GetPackageDetails(DirectShipmentId);

            if (Packages != null)
            {
                if (shipment.Service.HubCarrier != FrayteCourierCompany.AU && shipment.Service.HubCarrier.ToUpper() != FrayteCourierCompany.SKYPOSTAL &&
                    shipment.Service.HubCarrier != FrayteCourierCompany.BRING && shipment.Service.HubCarrier != FrayteCourierCompany.DPDCH &&
                    shipment.Service.HubCarrier != FrayteCourierCompany.DomesticA && shipment.Service.HubCarrier != FrayteCourierCompany.EAMExpress &&
                    shipment.Service.HubCarrier.ToUpper() != FrayteCourierCompany.CANADAPOST && shipment.Service.HubCarrier != FrayteCourierCompany.Hermes)
                {
                    foreach (var pack in Packages)
                    {
                        var packageTracking = getPackageImamagePath(pack.ExpressDetailId);
                        if (packageTracking != null && packageTracking.Count > 0)
                        {
                            foreach (var data in packageTracking)
                            {
                                list.Add(data.PackageLabelName);
                                list1.Add(data.PackageLabelName);
                                var resultReport = new Report.Generator.ManifestReport.PackageLabelReport().ExpressGenerateAllLabelReport(DirectShipmentId, null, list1, shipment.Service.HubCarrier, "");
                                if (!resultReport.Status)
                                {
                                    result.Status = false;
                                    result.Errors.Add("All the labels Pdf are not generated for " + DirectShipmentId.ToString());
                                }
                                list1.Remove(data.PackageLabelName);
                            }
                        }
                        else
                        {
                            result.Status = false;
                            result.Errors.Add("All the labels Pdf are not generated for " + DirectShipmentId.ToString());
                        }
                    }
                }

                string RateType = shipment.Service.RateType;
                string CourierCompany = shipment.Service.HubCarrier;
                string TrackingNo = new ExpressRepository().GetTrackingNo(DirectShipmentId);
                string labelName = string.Empty;
                if (shipment.Service.HubCarrier == FrayteCourierCompany.Yodel)
                {
                    labelName = FrayteShortName.Yodel;
                }
                else if (shipment.Service.HubCarrier == FrayteCourierCompany.Hermes)
                {
                    labelName = FrayteShortName.Hermes;
                }
                else if (shipment.Service.HubCarrier == FrayteCourierCompany.UKMail)
                {
                    labelName = FrayteShortName.UKMail;
                }
                else if (shipment.Service.HubCarrier == FrayteCourierCompany.DHL)
                {
                    labelName = FrayteShortName.DHL;
                }
                else if (shipment.Service.HubCarrier == FrayteCourierCompany.TNT)
                {
                    labelName = FrayteShortName.TNT;
                }
                else if (shipment.Service.HubCarrier == FrayteCourierCompany.UPS)
                {
                    labelName = FrayteShortName.UPS;
                }
                else if (shipment.Service.HubCarrier == FrayteCourierCompany.DPD)
                {
                    labelName = FrayteShortName.DPD;
                }
                else if (shipment.Service.HubCarrier == FrayteCourierCompany.AU)
                {
                    labelName = FrayteShortName.AU;
                }
                else if (shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.SKYPOSTAL)
                {
                    labelName = FrayteShortName.SKYPOSTAL;
                }
                else if (shipment.Service.HubCarrier == FrayteCourierCompany.BRING)
                {
                    labelName = FrayteShortName.BRING;
                }
                else if (shipment.Service.HubCarrier == FrayteCourierCompany.DomesticA)
                {
                    labelName = FrayteShortName.DomesticA;
                }
                else if (shipment.Service.HubCarrier == FrayteCourierCompany.EAMExpress)
                {
                    labelName = FrayteShortName.EAMExpress;
                }
                else if (shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.CANADAPOST)
                {
                    labelName = FrayteShortName.CANADAPOST;
                }
                else if (shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.USPS)
                {
                    labelName = FrayteShortName.USPS;
                }

                string LogisticLabel = string.Empty;

                int totalCartoonValue = shipment.Packages.Sum(k => k.CartonValue);
                if (!string.IsNullOrEmpty(shipment.Service.RateType))
                {
                    if (TrackingNo.Contains("Order_"))
                    {
                        var Trackinginfo = TrackingNo.Replace("Order_", "");
                        LogisticLabel = labelName + "_" + Trackinginfo + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                    }
                    else
                    {
                        if (shipment.Service.HubCarrier == FrayteCourierCompany.DPD || shipment.Service.HubCarrier == FrayteCourierCompany.TNT)
                        {
                            if (shipment.Service.HubCarrier == FrayteCourierCompany.DPD)
                            {
                                for (int i = 0; i < integrtaionResult.PieceTrackingDetails.Count; i++)
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
                            if (shipment.Service.HubCarrier == FrayteCourierCompany.AU || shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.SKYPOSTAL ||
                                shipment.Service.HubCarrier == FrayteCourierCompany.BRING || shipment.Service.HubCarrier == FrayteCourierCompany.DPDCH ||
                                shipment.Service.HubCarrier == FrayteCourierCompany.DomesticA || shipment.Service.HubCarrier == FrayteCourierCompany.EAMExpress ||
                                shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.CANADAPOST)
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
                        if (shipment.Service.HubCarrier == FrayteCourierCompany.DPD || shipment.Service.HubCarrier == FrayteCourierCompany.TNT)
                        {
                            if (shipment.Service.HubCarrier == FrayteCourierCompany.DPD)
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
                            if (shipment.Service.HubCarrier == FrayteCourierCompany.AU || shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.SKYPOSTAL ||
                                shipment.Service.HubCarrier == FrayteCourierCompany.BRING || shipment.Service.HubCarrier == FrayteCourierCompany.DPDCH ||
                                shipment.Service.HubCarrier == FrayteCourierCompany.DomesticA || shipment.Service.HubCarrier == FrayteCourierCompany.EAMExpress ||
                                shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.CANADAPOST || shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.CANADAPOST)
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
                if (shipment.Service.HubCarrier == FrayteCourierCompany.DHL)
                {
                    var shipmentImange = new ExpressRepository().GetShipmentImage(DirectShipmentId);
                    list.Add(shipmentImange.LogisticLabelImage);
                }
                // After Creting label  save in DirectShipmentTable
                if (shipment.Service.HubCarrier != FrayteCourierCompany.DPD && shipment.Service.HubCarrier.ToUpper() != FrayteCourierCompany.TNT &&
                    shipment.Service.HubCarrier != FrayteCourierCompany.AU && shipment.Service.HubCarrier.ToUpper() != FrayteCourierCompany.SKYPOSTAL &&
                    shipment.Service.HubCarrier != FrayteCourierCompany.BRING && shipment.Service.HubCarrier != FrayteCourierCompany.DPDCH &&
                    shipment.Service.HubCarrier != FrayteCourierCompany.DomesticA && shipment.Service.HubCarrier != FrayteCourierCompany.EAMExpress &&
                    shipment.Service.HubCarrier.ToUpper() != FrayteCourierCompany.CANADAPOST && shipment.Service.HubCarrier != FrayteCourierCompany.Hermes)
                {
                    var Result = new Report.Generator.ManifestReport.PackageLabelReport().ExpressGenerateAllLabelReport(DirectShipmentId, shipment, list, shipment.Service.HubCarrier, LogisticLabel);
                    result.Status = true;
                    if (Result.Status)
                    {
                        new ExpressRepository().SaveLogisticLabel(DirectShipmentId, LogisticLabel);
                        result.Status = true;
                    }
                }
                else
                {
                    new ExpressRepository().SaveLogisticLabel(DirectShipmentId, LogisticLabel);
                    result.Status = true;
                }
            }
            else
            {
                result.Status = false;
                result.Errors.Add("All the labels Pdf are not generated for " + DirectShipmentId.ToString());
            }
            return result;
        }

        public List<ExpressDetailPackageLabel> getPackageImamagePath(int ExpressDetailId)
        {
            return new ExpressRepository().GetPackageTracking(ExpressDetailId);
        }

        #endregion

        #region  Express Services 

        public IHttpActionResult ExpressServices(ExpressServiceObj sericeObj)
        {
            List<HubService> serice = new List<HubService>();
            if (sericeObj != null)
            {
                serice = new ExpressRepository().ExpressSerice(sericeObj);
            }

            return Ok();
        }

        #endregion

        #region Express Shipment Detail 

        [HttpGet]
        public ExpressShipmentModel ScannedShipmentDetail(int shipmentId, string callingType)
        {
            ExpressShipmentModel shipment = new ExpressRepository().ScannedShipmentDetail(shipmentId, callingType);
            return shipment;
        }

        [HttpGet]
        public IHttpActionResult PrintLabel(int id, string type)
        {
            FrayteResult result = new ExpressRepository().PrintLabel(id, type);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult LabelEmail(ExpressLabelEmail labelObj)
        {
            FrayteResult result = new ExpressRepository().SendLabelEmail(labelObj);
            return Ok(result);
        }

        [HttpGet]
        public List<object> ExpressTimeandDate()
        {
            List<object> times = new ExpressRepository().getExpressTimeZone();
            return times;
        }

        #endregion

        #region Avinash Code

        #region Product catalog

        [HttpGet]
        public IHttpActionResult FetchProductcatalog(int CustomerId, int HubId)
        {
            return Ok(new ExpressRepository().GetProductCatalog(CustomerId, HubId));
        }
        #endregion

        #region GetExpressTrackandTrace

        [HttpPost]
        public IHttpActionResult GenerateExpressTrackAndTraceExcel(ExpressTrackandTrace trackdetail)
        {
            FrayteManifestName result = new FrayteManifestName();
            result = new FrayteTrackAndTraceReport().GenerateExpressTrackandTraceDetail(trackdetail);
            return Ok(result);
        }

        [HttpPost]
        public HttpResponseMessage DownloadExpressTrackAndTraceReport(ReportFile fileName)
        {
            try
            {
                if (fileName != null && !string.IsNullOrEmpty(fileName.FileName))
                {
                    return DownloadExpressTrackAndTraceReport(fileName.FileName);
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

        private HttpResponseMessage DownloadExpressTrackAndTraceReport(string fileName)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/ReportFiles/ExpressTrack&Trace/" + fileName);
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

        #endregion

        #endregion
    }
}