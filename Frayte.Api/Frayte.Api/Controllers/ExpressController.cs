using Frayte.Api.Business;
using Frayte.Api.Models;
using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Models.Aftership;
using Frayte.Services.Models.DHL;
using Frayte.Services.Models.Express;
using Frayte.Services.Models.Tradelane;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Frayte.Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ExpressController : ApiController
    {
        [Route("api/Express/ExpressShipmentBooking")]
        [HttpPost]
        public IHttpActionResult ExpressShipmentBooking(ExpressShipmentRequest Shipment)
        {
            try
            {
                List<FrayteUploadshipment> _upload = new ExpressShipmentRepository().JsonValidate(Shipment, FrayteCallingType.FrayteApi);
                if (_upload.Count > 0)
                {
                    return Ok(_upload[0].Errors);
                }
                else
                {
                    var shipment = new ExpressShipmentRepository().MappingFrayteRequestToExpressBookingDetail(Shipment);
                    ExpressShipmentResponseModel ExpressResponse = new ExpressShipmentResponseModel();
                    ExpressShipmentModel model = new ExpressRepository().SaveShipment(shipment);
                    if (shipment.Error.Status && model.ShipmentStatusId == (int)FrayteExpressShipmentStatus.Scanned)
                    {
                        #region Logistic Integration
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


                            //Version 6.2  not in use
                            // shipmentXML = new DHLUKRepositry().CreateXMLForDHLUK(shipmentRequestDto);
                            string xml_in = string.Empty;

                            xml_in = File.ReadAllText(@shipmentXML);

                            //step 3. Create Shipment
                            var DHLResponse = new DHLResponseDto();

                            DHLResponse = new DHLRepository().CreateShipment(xml_in, shipmentRequestDto);

                            //step 4. Mapping ShipmentResonse to IntegrtaionResult

                            result = new DHLRepository().MapDHLIntegrationResponse(DHLResponse);

                            if (result.Status)
                            {
                                if (shipment.Service.HubCarrier == FrayteCourierCompany.DHL)
                                {
                                    //Step 1.1 Mapping ShipmentResult to IntegrtaionResult
                                    try
                                    {
                                        new ExpressRepository().MappingCourierPieceDetail(result, shipment);
                                    }
                                    catch (Exception Ex)
                                    {
                                        throw (new FrayteApiException("MapDHLCourierPieceDetailError", Ex));
                                    }
                                }
                                //Step3 : Save Main Tracking Number
                                new ExpressRepository().SaveMainTrackingDetail(shipment, result, xml_in, "");
                                new ExpressRepository().SaveTrackingDetail(shipment, result);


                                // AfterShipIntegration

                                if (AppSettings.ApplicationMode == FrayteApplicationMode.Live)
                                {
                                    FrayteAfterShipTracking aftershipTracking = new AftershipTrackingRepository().MapDirectShipmentObjToAfterShip(shipment.ExpressId, FrayteShipmentServiceType.Express);

                                    if (aftershipTracking != null && AppSettings.ApplicationMode == FrayteApplicationMode.Live)
                                    {
                                        //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Integration in aftership"));
                                        new AftershipTrackingRepository().CreateTracking(aftershipTracking);
                                    }
                                }


                                //Step4:  //Start downloading the images from DHL server and making PDF
                                var count = 1;
                                var totalpiece = result.PieceTrackingDetails.Count();
                                totalpiece = totalpiece - 1;

                                foreach (var data in result.PieceTrackingDetails.Take(totalpiece))
                                {
                                    if (shipment.Service.HubCarrier == FrayteCourierCompany.DHL)
                                    {
                                        // Step3.1 
                                        data.LabelName = new DHLRepository().ExpressDownloadDHLImage(data, totalpiece, count, shipment.ExpressId);
                                    }
                                    if (!data.PieceTrackingNumber.Contains("AirwayBillNumber_"))
                                    {
                                        //Step3.2
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
                                        new ExpressShipmentRepository().MappingFrayteResponseToExpressBookingDetail(shipment, result, ExpressResponse);
                                    }
                                }
                            }
                            else
                            {
                                shipment.Error = result.Error;
                            }
                            #endregion
                        }
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
                                        new ExpressShipmentRepository().MappingFrayteResponseToExpressBookingDetail(shipment, result, ExpressResponse);
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

                            Frayte.Services.Models.ParcelHub.ParcelHubShipmentRequest request = new ParcelHubRepository().MapExpressBookingDetailToShipmentRequest(shipment);
                            //Create shipment in Parcel hub
                            Frayte.Services.Models.ParcelHub.ParcelHubResponse response = new ParcelHubRepository().CreateHermesShipment(request);

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
                                        new ExpressShipmentRepository().MappingFrayteResponseToExpressBookingDetail(shipment, result, ExpressResponse);
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
                        if (shipment.Service.HubCarrier.ToUpper().Contains(FrayteCourierCompany.EAM))
                        {
                            #region EAM Global Integration

                            IntegrtaionResult result = new IntegrtaionResult();
                            if (shipment.Service.HubCarrier.ToUpper().Contains(FrayteCourierCompany.EAM))
                            {
                                var shipmentRequestDto = new EAMGlobalRepository().MapExpressBookingDetailToShipmentRequestDto(shipment);
                                string shipmentXML = new EAMGlobalRepository().CreateXMLForEAM(shipmentRequestDto);
                                string xml_in = File.ReadAllText(@shipmentXML);

                                //Create Shipment
                                var shipmentResult = new EAMGlobalRepository().CreateShipment(xml_in, shipment.ExpressId);
                                result = new EAMGlobalRepository().MapExpressEAMGlobalIntegrationResponse(shipmentResult, shipment.Packages);
                            }

                            if (result.Status)
                            {
                                if (shipment.Service.HubCarrier.ToUpper().Contains(FrayteCourierCompany.EAM))
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
                                    if (shipment.Service.HubCarrier.ToUpper().Contains(FrayteCourierCompany.EAM))
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
                                    new ExpressShipmentRepository().MappingFrayteResponseToExpressBookingDetail(shipment, result, ExpressResponse);
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

                        #endregion

                        return Ok(ExpressResponse);
                    }
                }

            }
            catch (Exception Ex)
            {

                string error = ReadException(Ex);
                ExpressErrorResponse Response = new FrayteApiErrorCodeRepository().SaveExpressApiError(error);
                return Ok(Response);
            }
            return Ok();
        }

        //[Route("api/Express/GetTracking")]
        //[HttpPost]
        //public IHttpActionResult GetTracking(ExpressTrackingRequestModel TrackingDetail)
        //{
        //    //new UpdateTradelaneTrackingRepository().GetTracking(TrackingDetail.Number);
           
        //    return Ok(new ExpressShipmentRepository().GetTracking(TrackingDetail.Number));
        //}

        public string ReadException(Exception ex)
        {
            if (ex.InnerException != null)
            {
                string value = ReadException(ex.InnerException);
                //string value = ex.InnerException.ToString();
                return value;
            }
            else
            {
                return ex.Message;
            }
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
                if (shipment.Service.HubCarrier != FrayteCourierCompany.AU && shipment.Service.HubCarrier.ToUpper() != FrayteCourierCompany.SKYPOSTAL && shipment.Service.HubCarrier != FrayteCourierCompany.BRING && shipment.Service.HubCarrier != FrayteCourierCompany.DPDCH && shipment.Service.HubCarrier != FrayteCourierCompany.EAM && shipment.Service.HubCarrier.ToUpper() != FrayteCourierCompany.CANADAPOST && shipment.Service.HubCarrier != FrayteCourierCompany.Hermes)
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
                else if (shipment.Service.HubCarrier == FrayteCourierCompany.EAM)
                {
                    labelName = FrayteShortName.EAM;
                }
                else if (shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.CANADAPOST)
                {
                    labelName = FrayteShortName.CANADAPOST;
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
                            if (shipment.Service.HubCarrier == FrayteCourierCompany.AU || shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.SKYPOSTAL || shipment.Service.HubCarrier == FrayteCourierCompany.BRING || shipment.Service.HubCarrier == FrayteCourierCompany.DPDCH || shipment.Service.HubCarrier.Contains(FrayteCourierCompany.EAM) || shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.CANADAPOST)
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
                            if (shipment.Service.HubCarrier == FrayteCourierCompany.AU || shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.SKYPOSTAL || shipment.Service.HubCarrier == FrayteCourierCompany.BRING || shipment.Service.HubCarrier == FrayteCourierCompany.DPDCH || shipment.Service.HubCarrier == FrayteCourierCompany.EAM || shipment.Service.HubCarrier.ToUpper() == FrayteCourierCompany.CANADAPOST)
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
                if (shipment.Service.HubCarrier != FrayteCourierCompany.DPD && shipment.Service.HubCarrier.ToUpper() != FrayteCourierCompany.TNT && shipment.Service.HubCarrier != FrayteCourierCompany.AU && shipment.Service.HubCarrier.ToUpper() != FrayteCourierCompany.SKYPOSTAL && shipment.Service.HubCarrier != FrayteCourierCompany.BRING && shipment.Service.HubCarrier != FrayteCourierCompany.DPDCH && shipment.Service.HubCarrier != FrayteCourierCompany.EAM && shipment.Service.HubCarrier.ToUpper() != FrayteCourierCompany.CANADAPOST && shipment.Service.HubCarrier != FrayteCourierCompany.Hermes)
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
    }
}
