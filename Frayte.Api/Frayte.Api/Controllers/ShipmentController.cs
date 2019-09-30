using Frayte.Api.Business;
using Frayte.Api.Models;
using Frayte.Services.Models.ParcelHub;
using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Models.ApiModal;
using Frayte.Services.DataAccess;
using Frayte.Services.Models.TNT;
using Frayte.Services.Models.DHL;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web;
using System.IO;
using System.Web.Http.Cors;
using Frayte.Services.Models.Aftership;

namespace Frayte.Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ShipmentController : ApiController
    {
        [Route("api/Shipment/InitializeShip")]
        [HttpPost]
        public IHttpActionResult InitializeShip(FrayteShipmentRequest FrayteShipmentRequest)
        {
            DirectShipmentRepository directBooking = new DirectShipmentRepository();
            DirectBookingShipmentDraftDetail directBookingDetail;
            try
            {
                List<FrayteUploadshipment> _upload = new APIShipmentRepository().JsonValidate(FrayteShipmentRequest, FrayteCallingType.FrayteApi);
                if (_upload.Count > 0)
                {
                    return Ok(_upload[0].Errors);
                }
                else
                {
                    directBookingDetail = new APIShipmentRepository().MappingFrayteRequestToDirectBookingDetail(FrayteShipmentRequest);
                    FrayteRateResponse RateResponse = new FrayteRateResponse();
                    //Special Customer Mapping
                    directBookingDetail = directBooking.SaveDirectBooking(directBookingDetail);

                    string resultjson = string.Empty;
                    if (directBookingDetail.Error.Status)
                    {
                        var userInfo = new APIShipmentRepository().UserInfo(FrayteShipmentRequest.Security.APIKey);
                        var countryDetails = new APIShipmentRepository().CountryInfo(FrayteShipmentRequest);
                        var serviceRequest = new APIShipmentRepository().MappingDirectBookingFindService(FrayteShipmentRequest, countryDetails, userInfo);
                        var ReteDetail = new DirectShipmentRepository().GetServices(serviceRequest);
                        if (ReteDetail != null && ReteDetail.Count > 0)
                        {
                            List<FrayteRateCard> RateCard = new APIShipmentRepository().MappingFrayteRateResponse(ReteDetail, FrayteShipmentRequest.PackageCalculationType, directBookingDetail.Currency.CurrencyCode);
                            if (RateCard != null)
                            {
                                RateResponse.ShipmentId = CryptoEngine.Encrypt(directBookingDetail.DirectShipmentDraftId.ToString(), Models.EncriptionKey.PrivateKey);
                                RateResponse.RateCard = RateCard;
                                RateResponse.Status = true;
                                RateResponse.Description = "Rates Recevied Successfully";
                            }
                        }
                        else
                        {
                            RateResponse = null;
                            List<DirectBookingService> overweight = new QuotationRepository().GetServices(serviceRequest);
                            if (overweight != null && overweight.Count > 0)
                            {
                                FrayteRateResponse Response = new FrayteApiErrorCodeRepository().SaveInitializeApiError("No service available as your shipment is over weight for your assigned services");
                                new ShipmentEmailRepository().SendApiErrorMail(directBookingDetail, Response.Errors);
                                return Ok(Response);
                            }
                        }
                    }
                    return Ok(RateResponse);
                }
            }
            catch (FrayteApiException ex)
            {
                string error = ReadException(ex);
                FrayteRateResponse Response = new FrayteApiErrorCodeRepository().SaveInitializeApiError(error);
                new ShipmentEmailRepository().SendApiErrorMail(directBookingDetail = null, Response.Errors);
                return Ok(Response);
            }
        }

        [Route("api/Shipment/FinalizeShip")]
        [HttpPost]
        public IHttpActionResult FinalizeShip(IntegrationRequest request)
        {
            DirectBookingShipmentDraftDetail directBookingDetail = new DirectBookingShipmentDraftDetail();
            try
            {
                //Decrypt response for update rate card values
                var FrayteShipmentResponse = new FrayteShipmentResponseDto();
                var DecryptDraftId = CryptoEngine.Decrypt(request.ShipmentId, Models.EncriptionKey.PrivateKey);
                string[] ratecard = new APIShipmentRepository().UpdateDraftRateCard(int.Parse(DecryptDraftId.ToString()), request.RateCardId);

                int DirectShipmentid = 0;
                directBookingDetail = new APIShipmentRepository().GetDirectShipmentDraftDetail(int.Parse(DecryptDraftId.ToString()));

                //Step 1: Shipment Integrations
                if (ratecard.Length > 0)
                {
                    #region UPS Integrations

                    if (ratecard[8].ToString() == FrayteCourierCompany.UPS)
                    {
                        IntegrtaionResult result = new IntegrtaionResult();

                        //step1. Mapping With directBookingDetail to ShipmentRequestDto
                        var shipmentRequestDto = new UPSRepository().MapDirectBookingDetailToShipmentRequestDto(directBookingDetail);

                        //Step2.Create Shipment
                        var shipmentResult = new UPSRepository().CreateShipment(shipmentRequestDto, directBookingDetail.ReferenceDetail);

                        //step3 Mapping ShipmentResonse to IntegrtaionResult
                        result = new UPSRepository().MapUPSIntegrationResponse(shipmentResult);

                        if (result.Status)
                        {
                            //step1. After Shipment Create need to save the information in database 
                            DirectShipmentid = new DirectShipmentRepository().SaveShipment(directBookingDetail, result);

                            if (ratecard[8].ToString() == FrayteCourierCompany.UPS)
                            {
                                //Step 1.1 Mapping ShipmentResult to IntegrtaionResult
                                new UPSRepository().MappingCourierPieceDetail(result, directBookingDetail, DirectShipmentid);
                            }

                            //Step2 : Save Package Label Tracking detail
                            new UPSRepository().SaveTrackingDetail(directBookingDetail, result, DirectShipmentid);

                            //Step3:  //Start downloading the images from UPS server and making PDF
                            var count = 1;
                            foreach (var data in result.PieceTrackingDetails)
                            {
                                if (ratecard[8].ToString() == FrayteCourierCompany.UPS)
                                {
                                    //Step3.1
                                    data.LabelName = new UPSRepository().DownloadUPSImage(data, result.PieceTrackingDetails.Count(), count, DirectShipmentid);
                                }

                                // Step3.2
                                new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                                count++;
                            }

                            //Step:4 Generate PDF
                            Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, result);
                            directBookingDetail.Error = new FratyteError()
                            {
                                Status = result.Status,
                            };
                            FrayteShipmentResponse = new APIShipmentRepository().MappingFrayteShipmentResponse(directBookingDetail, result, DirectShipmentid.ToString(), ratecard, request.RateCardId);

                            if (directBookingDetail.Error.Status)
                            {
                                new ShipmentEmailRepository().SendDirectBookingConfirmationMail(directBookingDetail, DirectShipmentid);
                            }
                        }
                        else
                        {
                            directBookingDetail.Error = result.Error;
                            //Send error mail to developer
                            new ShipmentEmailRepository().SendApiErrorMail(directBookingDetail, result.ErrorCode);
                            return Ok(result.ErrorCode);
                        }
                    }

                    #endregion

                    #region TNT Integration

                    else if (ratecard[8].ToString() == FrayteCourierCompany.TNT)
                    {
                        var shipmentPackageTrackingDetail = new List<FraytePackageTrackingDetail>();
                        TNTResponseDto TNTResponse = new TNTResponseDto();
                        IntegrtaionResult result = new IntegrtaionResult();

                        //Integration
                        var TNTObj = new TNTRepository().MapDirectBookingObjToTNTobj(directBookingDetail);
                        var TNTXml = new TNTRepository().CreateTNTXMl(TNTObj);

                        TNTResponse = new TNTRepository().CreateShipment(TNTXml, int.Parse(DecryptDraftId.ToString()));

                        directBookingDetail.Error = TNTResponse.Error;
                        string dataSetHtml = string.Empty;

                        if (TNTResponse.Error.Status == false)
                        {
                            result = new TNTRepository().MapTNTIntegrationResponse(TNTResponse, shipmentPackageTrackingDetail, TNTResponse.TNTShipmentResponse.TrackingCode);
                            directBookingDetail.Error = result.Error;
                            //Send error mail to developer
                            new ShipmentEmailRepository().SendApiErrorMail(directBookingDetail, result.ErrorCode);
                            return Ok(result.ErrorCode);
                        }
                        else if (directBookingDetail.Error != null && directBookingDetail.Error.Status == true)
                        {
                            result = new TNTRepository().MapTNTIntegrationResponse(TNTResponse, shipmentPackageTrackingDetail, TNTResponse.TNTShipmentResponse.TrackingCode);
                            //Finally Save the Order Id too to get the information of multiple shpment in Tracking Detail page.
                            DirectShipmentid = new DirectShipmentRepository().SaveShipment(directBookingDetail, TNTResponse.TNTShipmentResponse.TrackingCode, directBookingDetail.CustomerId, TNTResponse.TNTShipmentResponse.BookingRefNo, null);

                            // save tracking code 
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
                                
                                TNTResponse.Error.Status = true;
                            }
                            else
                            {
                                TNTResponse.Error.Status = false;
                                var er = new FrayteKeyValue();
                                er.Key = "Misscllaneous Errors";
                                er.Value = new List<string>() { "Something bad happen please try again later." };
                                TNTResponse.Error.MiscErrors.Add(er);
                                result = new TNTRepository().MapIntegrationErrorResponse(TNTResponse);

                                directBookingDetail.Error = result.Error;
                                //Send mail to developer
                                new ShipmentEmailRepository().SendApiErrorMail(directBookingDetail, result.ErrorCode);
                                return Ok(result.ErrorCode);
                            }
                            Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, result);
                            new TNTRepository().SaveTempShipmentXML(TNTResponse, DirectShipmentid);

                            FrayteShipmentResponse = new APIShipmentRepository().MappingFrayteShipmentResponse(directBookingDetail, result, DirectShipmentid.ToString(), ratecard, request.RateCardId);

                            if (directBookingDetail.Error.Status)
                            {
                                new ShipmentEmailRepository().SendDirectBookingConfirmationMail(directBookingDetail, DirectShipmentid);
                            }
                        }
                    }

                    #endregion

                    #region UKMail, Yodel, Hermes

                    else if (ratecard[8].ToString() == FrayteCourierCompany.UKMail || ratecard[8].ToString() == FrayteCourierCompany.Yodel || ratecard[8].ToString() == FrayteCourierCompany.Hermes)
                    {
                        ParcelHubResponse response = new ParcelHubResponse();
                        try
                        {
                            //Parcel Hub Integration here
                            Frayte.Services.Models.ParcelHub.ParcelHubShipmentRequest parcelhubrequest = new ParcelHubRepository().MapDirectBookingDetailToShipmentRequest(directBookingDetail);

                            //Create Shipment in Parcel hub
                            response = new ParcelHubRepository().CreateShipment(parcelhubrequest);

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

                                //Return Shipment Response
                                FrayteShipmentResponse = new APIShipmentRepository().MappingFrayteShipmentResponse(directBookingDetail, result, DirectShipmentid.ToString(), ratecard, request.RateCardId);

                                if (directBookingDetail.Error.Status)
                                {
                                    //new ShipmentEmailRepository().SendDirectBookingConfirmationMail(directBookingDetail, DirectShipmentid);
                                }
                            }
                            else
                            {
                                directBookingDetail.Error = result.Error;
                                //Send error mail to developer
                                new ShipmentEmailRepository().SendApiErrorMail(directBookingDetail, result.ErrorCode);
                                return Ok(result.ErrorCode);
                            }
                        }
                        catch (Exception ex)
                        {
                            var result = new ParcelHubRepository().MappingParcelHubToIntegrationResult(directBookingDetail, response, ex.Message);
                        }
                    }

                    #endregion

                    #region DHL Integration

                    else if (ratecard[8].ToString() == FrayteCourierCompany.DHL)
                    {
                        IntegrtaionResult result = new IntegrtaionResult();
                        DHLResponseDto DHLResponse = new DHLResponseDto();
                        try
                        {
                            //Step 1 : Mapping DirectBookingDetail to DHL Model
                            var shipmentRequestDto = new DHLRepository().MapDirectBookingDetailToDHLShipmentRequestDto(directBookingDetail);
                            //Step 2 : Create Xml 
                            var shipmentXML = new DHLRepository().CreateXMLForDHL(shipmentRequestDto);
                            string xml_in = File.ReadAllText(@shipmentXML);

                            //Step 3 : Create Shipment                      
                            DHLResponse = new DHLRepository().CreateShipment(xml_in, shipmentRequestDto);

                            //step 4 : Mapping ShipmentResonse to IntegrtaionResult
                            result = new DHLRepository().MapDHLIntegrationResponse(DHLResponse);
                            if (result.Status)
                            {
                                //step 4.1 : After Shipment Create need to save the information in database 
                                DirectShipmentid = new DirectShipmentRepository().SaveShipment(directBookingDetail, result);

                                if (ratecard[8].ToString() == FrayteCourierCompany.DHL)
                                {
                                    //Step 4.2 Mapping ShipmentResult to IntegrtaionResult
                                    new DHLRepository().MappingCourierPieceDetail(result, directBookingDetail, DirectShipmentid);
                                }

                                //Step 4.3 : Save Package Label Tracking detail
                                new DHLRepository().SaveTrackingDetail(directBookingDetail, result, DirectShipmentid);

                                //Step 4.4 :  //Start downloading the images from DHL server and making PDF
                                var count = 1;
                                var totalpiece = result.PieceTrackingDetails.Count();
                                totalpiece = totalpiece - 1;
                                foreach (var data in result.PieceTrackingDetails.Take(totalpiece))
                                {
                                    if (ratecard[8].ToString() == FrayteCourierCompany.DHL)
                                    {
                                        //Step 4.4.1 :
                                        data.LabelName = new DHLRepository().DownloadDHLImage(data, totalpiece, count, DirectShipmentid);
                                    }

                                    // Step 4.4.2 :
                                    if (!data.PieceTrackingNumber.Contains("AirwayBillNumber_"))
                                    {
                                        new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                                        count++;
                                    }
                                }
                                var CourierPieceDetail = result.PieceTrackingDetails.Where(t => t.PieceTrackingNumber.Contains("AirwayBillNumber_")).FirstOrDefault();
                                if (CourierPieceDetail != null)
                                {
                                    var data1 = new DHLRepository().DownloadDHLImage(CourierPieceDetail, totalpiece, 0, DirectShipmentid);
                                }

                                Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, result);

                                directBookingDetail.Error = new FratyteError()
                                {
                                    Status = result.Status,
                                };
                                FrayteShipmentResponse = new APIShipmentRepository().MappingFrayteShipmentResponse(directBookingDetail, result, DirectShipmentid.ToString(), ratecard, request.RateCardId);

                                if (directBookingDetail.Error.Status)
                                {
                                    new ShipmentEmailRepository().SendDirectBookingConfirmationMail(directBookingDetail, DirectShipmentid);
                                }
                            }
                            else
                            {
                                directBookingDetail.Error = result.Error;
                                //Send mail to developer
                                new ShipmentEmailRepository().SendApiErrorMail(directBookingDetail, result.ErrorCode);
                                return Ok(result.ErrorCode);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    #endregion

                    #region AU Integration

                    else if (ratecard[8].ToString() == FrayteCourierCompany.AU)
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
                                    //Step3.1
                                    data.LabelName = new AURepository().DownloadAUPDF(data, result.PieceTrackingDetails.Count(), count, DirectShipmentid);
                                }

                                // Step3.2
                                new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                                count++;
                            }

                            //Step:4 Generate PDF
                            Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, result);
                            directBookingDetail.Error = new FratyteError()
                            {
                                Status = result.Status,
                            };

                            FrayteShipmentResponse = new APIShipmentRepository().MappingFrayteShipmentResponse(directBookingDetail, result, DirectShipmentid.ToString(), ratecard, request.RateCardId);

                            if (directBookingDetail.Error.Status)
                            {
                                new ShipmentEmailRepository().SendDirectBookingConfirmationMail(directBookingDetail, DirectShipmentid);
                            }
                        }
                        else
                        {
                            directBookingDetail.Error = result.Error;
                            try
                            {
                                //step 1. Mapping With directBookingDetail to ShipmentRequestDto
                                new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, result.Error);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                    #endregion

                    #region DPD Integration

                    else if (ratecard[8].ToString() == FrayteCourierCompany.DPD)
                    {
                        IntegrtaionResult result = new IntegrtaionResult();
                        directBookingDetail.CustomerRateCard.CourierName = "DPD";
                        //step 1. Mapping With directBookingDetail to ShipmentRequestDto
                        var shipmentRequestDto = new DPDRepository().MapDirectBookingDetailToShipmentRequestDto(directBookingDetail);
                        //Create Shipment
                        var shipmentResult = new DPDRepository().CreateShipment(shipmentRequestDto);

                        //Mapping ShipmentResonse to IntegrtaionResult
                        result = new DPDRepository().MapDPDIntegrationResponse(shipmentResult);

                        if (result.Status)
                        {
                            //After Shipment Create need to save the information in database 
                            DirectShipmentid = new DirectShipmentRepository().SaveShipment(directBookingDetail, result);

                            if (ratecard[8].ToString() == FrayteCourierCompany.DPD)
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
                                if (ratecard[8].ToString() == FrayteCourierCompany.DPD)
                                {
                                    //Step3.1
                                    data.LabelName = new DPDRepository().DownloadDPDHtmlImage(data, result.PieceTrackingDetails.Count(), count, DirectShipmentid, directBookingDetail.CustomerRateCard);
                                }

                                // Step3.2
                                new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                                count++;
                            }
                            //Step:4 Generate PDF
                            Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, result);

                            directBookingDetail.Error = new FratyteError()
                            {
                                Status = result.Status,
                            };

                            FrayteShipmentResponse = new APIShipmentRepository().MappingFrayteShipmentResponse(directBookingDetail, result, DirectShipmentid.ToString(), ratecard, request.RateCardId);

                            if (directBookingDetail.Error.Status)
                            {
                                new ShipmentEmailRepository().SendDirectBookingConfirmationMail(directBookingDetail, DirectShipmentid);
                            }
                        }
                        else
                        {
                            directBookingDetail.Error = result.Error;
                            try
                            {
                                //step 1. Mapping With directBookingDetail to ShipmentRequestDto
                                new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, result.Error);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                    #endregion

                    #region SKYPOSTAL Integration

                    else if (ratecard[8].ToString() == FrayteCourierCompany.SKYPOSTAL)
                    {
                        IntegrtaionResult result = new IntegrtaionResult();
                        if (ratecard[8].ToString() == FrayteCourierCompany.SKYPOSTAL)
                        {
                            var shipmentRequestDto = new SkyPostalRepository().MapDirectBookingDetailToShipmentRequestDto(directBookingDetail);
                            //Create Shipment
                            var shipmentResult = new SkyPostalRepository().CreateShipment(shipmentRequestDto, directBookingDetail.DirectShipmentDraftId);
                            result = new SkyPostalRepository().MapSkyPostalIntegrationResponse(shipmentResult, shipmentRequestDto.detail);
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
                                    //Step3.1
                                    data.LabelName = new SkyPostalRepository().DownloadSkyPostalImage(data, result.PieceTrackingDetails.Count(), count, DirectShipmentid);
                                }

                                // Step3.2
                                new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                                count++;
                            }

                            //Step:4 Generate PDF
                            Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, result);

                            directBookingDetail.Error = new FratyteError()
                            {
                                Status = result.Status,
                            };

                            FrayteShipmentResponse = new APIShipmentRepository().MappingFrayteShipmentResponse(directBookingDetail, result, DirectShipmentid.ToString(), ratecard, request.RateCardId);

                            if (directBookingDetail.Error.Status)
                            {
                                new ShipmentEmailRepository().SendDirectBookingConfirmationMail(directBookingDetail, DirectShipmentid);
                            }
                        }
                        else
                        {
                            directBookingDetail.Error = result.Error;
                            try
                            {
                                //step 1. Mapping With directBookingDetail to ShipmentRequestDto
                                new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, result.Error);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                    #endregion

                    #region EAM Global Integration

                    else if (ratecard[8].ToString().Contains(FrayteCourierCompany.EAM))
                    {
                        IntegrtaionResult result = new IntegrtaionResult();
                        if (ratecard[8].ToString().Contains(FrayteCourierCompany.EAM))
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
                                    //Step3.1
                                    data.LabelName = new EAMGlobalRepository().DownloadEAMImageTOPDF(data, result.PieceTrackingDetails.Count(), count, DirectShipmentid, directBookingDetail.CustomerRateCard);
                                }

                                // Step3.2
                                new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                                count++;
                            }

                            //Step:4 Generate PDF
                            Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, result);

                            directBookingDetail.Error = new FratyteError()
                            {
                                Status = result.Status,
                            };

                            FrayteShipmentResponse = new APIShipmentRepository().MappingFrayteShipmentResponse(directBookingDetail, result, DirectShipmentid.ToString(), ratecard, request.RateCardId);

                            if (directBookingDetail.Error.Status)
                            {
                                new ShipmentEmailRepository().SendDirectBookingConfirmationMail(directBookingDetail, DirectShipmentid);
                            }
                        }
                        else
                        {
                            directBookingDetail.Error = result.Error;
                            try
                            {
                                //step 1. Mapping With directBookingDetail to ShipmentRequestDto
                                new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, result.Error);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                    #endregion

                    #region DPD-CH Integration

                    else if (ratecard[8].ToString() == FrayteCourierCompany.DPDCH)
                    {
                        IntegrtaionResult result = new IntegrtaionResult();

                        if (ratecard[8].ToString() == FrayteCourierCompany.DPDCH)
                        {
                            //step 1. Mapping With directBookingDetail to ShipmentRequestDto
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
                                    //Step3.1
                                    data.LabelName = new DPDSwitzerlandRepository().DownloadDPDCHBytetoImage(data, result.PieceTrackingDetails.Count(), count, DirectShipmentid);
                                }
                                // Step3.2
                                new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                                count++;
                            }
                            //Step:4 Generate PDF
                            Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, result);
                            directBookingDetail.Error = new FratyteError()
                            {
                                Status = result.Status,
                            };
                            FrayteShipmentResponse = new APIShipmentRepository().MappingFrayteShipmentResponse(directBookingDetail, result, DirectShipmentid.ToString(), ratecard, request.RateCardId);

                            if (directBookingDetail.Error.Status)
                            {
                                new ShipmentEmailRepository().SendDirectBookingConfirmationMail(directBookingDetail, DirectShipmentid);
                            }
                        }

                        else
                        {
                            directBookingDetail.Error = result.Error;
                            try
                            {
                                //step 1. Mapping With directBookingDetail to ShipmentRequestDto
                                new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, result.Error);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                    #endregion

                    #region Bring Integration

                    else if (ratecard[8].ToString() == FrayteCourierCompany.BRING)
                    {
                        IntegrtaionResult result = new IntegrtaionResult();
                        if (ratecard[8].ToString() == FrayteCourierCompany.BRING)
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

                            if (ratecard[8].ToString() == FrayteCourierCompany.BRING)
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
                                if (ratecard[8].ToString() == FrayteCourierCompany.BRING)
                                {
                                    //Step3.1
                                    data.LabelName = new BringRepository().DownloadBringlabelPDF(data, result.PieceTrackingDetails.Count(), count, DirectShipmentid);
                                }

                                // Step3.2
                                new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                                count++;
                            }

                            //Step:4 Generate PDF
                            Generate_Seperate_PackageLabelPDF(DirectShipmentid, directBookingDetail, result);
                            directBookingDetail.Error = new FratyteError()
                            {
                                Status = result.Status,
                            };

                            FrayteShipmentResponse = new APIShipmentRepository().MappingFrayteShipmentResponse(directBookingDetail, result, DirectShipmentid.ToString(), ratecard, request.RateCardId);

                            if (directBookingDetail.Error.Status)
                            {
                                new ShipmentEmailRepository().SendDirectBookingConfirmationMail(directBookingDetail, DirectShipmentid);
                            }
                            FrayteShipmentResponse = new APIShipmentRepository().MappingFrayteShipmentResponse(directBookingDetail, result, DirectShipmentid.ToString(), ratecard, request.RateCardId);

                            if (directBookingDetail.Error.Status)
                            {
                                new ShipmentEmailRepository().SendDirectBookingConfirmationMail(directBookingDetail, DirectShipmentid);
                            }
                        }
                        else
                        {
                            directBookingDetail.Error = result.Error;
                            try
                            {
                                //step 1. Mapping With directBookingDetail to ShipmentRequestDto
                                new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, result.Error);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                    #endregion

                    #region Canada Post Integration

                    else if (ratecard[8].ToString() == FrayteCourierCompany.CANADAPOST)
                    {
                        IntegrtaionResult result = new IntegrtaionResult();
                        if (ratecard[8].ToString() == FrayteCourierCompany.CANADAPOST)
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

                            if (ratecard[8].ToString() == FrayteCourierCompany.CANADAPOST)
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
                                if (ratecard[8].ToString() == FrayteCourierCompany.CANADAPOST)
                                {
                                    //Step3.1
                                    data.LabelName = new ETowerRepository().DownloadCanadaPostBytetoImage(data, result.PieceTrackingDetails.Count(), count, DirectShipmentid);
                                }

                                // Step3.2
                                new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                                count++;
                            }

                            //Step:4 Generate PDF
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
                                //step 1. Mapping With directBookingDetail to ShipmentRequestDto
                                new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, result.Error);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                    #endregion

                    #region aftership Integration

                    if (directBookingDetail.Error.Status)
                    {
                        FrayteAfterShipTracking aftershipTracking = new AftershipTrackingRepository().MapDirectShipmentObjToAfterShip(DirectShipmentid, FrayteShipmentServiceType.DirectBooking);
                        if (aftershipTracking != null && AppSettings.ApplicationMode == FrayteApplicationMode.Live)
                        {
                            //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Integration in aftership"));
                            new AftershipTrackingRepository().CreateTracking(aftershipTracking);
                        }
                        // Offline tracking 
                        List<FrayteOfflineTracking> frayteOfflineTracking = new TrackingRepository().MapDirectShipmentObjTacking(DirectShipmentid, directBookingDetail);

                        new TrackingRepository().UpdateOfflineTracking(frayteOfflineTracking);
                    }

                    #endregion
                }
                return Ok(FrayteShipmentResponse);
            }
            catch (FrayteApiException ex)
            {
                string error = ReadException(ex);
                List<FrayteApiError> Response = new FrayteApiErrorCodeRepository().SaveFinilizeApiError(null, null, error);
                new ShipmentEmailRepository().SendApiErrorMail(directBookingDetail = null, Response);
                return Ok(Response);
            }
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
                if (directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.AU && directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.SKYPOSTAL && directBookingDetail.CustomerRateCard.CourierName!= FrayteCourierCompany.DPD)
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
                    labelName = FrayteShortName.DPD;
                }
                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.AU)
                {
                    labelName = FrayteShortName.AU;
                }
                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.SKYPOSTAL)
                {
                    labelName = FrayteShortName.SKYPOSTAL;
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
                        if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DPD || directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.TNT)
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
                            if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.AU || directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.SKYPOSTAL)
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

                            if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.AU || directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.SKYPOSTAL)
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
                if (directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.DPD && directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.TNT && directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.AU && directBookingDetail.CustomerRateCard.CourierName != FrayteCourierCompany.SKYPOSTAL)
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
                result.Errors.Add("All the labels Pdf are not generated fo " + DirectShipmentId.ToString());
            }
            return result;
        }

        public List<PackageTrackingDetail> getPackageImamagePath(int DirectShipmentDetailId)
        {
            return new DirectShipmentRepository().GetPackageTracking(DirectShipmentDetailId);
        }

        public string ReadException(Exception ex)
        {
            if (ex.InnerException != null)
            {
                string value = ReadException(ex.InnerException);
                return value;
            }
            else
            {
                return ex.Message;
            }
        }
    }
}