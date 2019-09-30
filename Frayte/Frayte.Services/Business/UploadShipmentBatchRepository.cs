using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models;
using Frayte.Services.DataAccess;
using Newtonsoft.Json;
using Frayte.Services.Utility;
using Frayte.Services.Models.TNT;
using System.IO;
using XStreamline.Log;

namespace Frayte.Services.Business
{
    public class UploadShipmentBatchRepository
    {

        public Tuple<FrayteResult, DirectBookingShipmentDraftDetail, int> DBUploadShipmentUpsIntegration(FrayteUploadshipment res)
        {
            FrayteResult FR = new FrayteResult();
            var count = 0;
            int DirectShipmentid = 0;

            var result1 = new DirectBookingUploadShipmentRepository().DirectBookingObj(res);
            #region COLLECTION Date 

            DateTime mindatetime = result1.ReferenceDetail.CollectionDate.Value;
            DateTime maxdatetime = result1.ReferenceDetail.CollectionDate.Value.AddDays(1);

            if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Sunday)
            {
                mindatetime = result1.ReferenceDetail.CollectionDate.Value.AddDays(1);
            }
            else if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Saturday)
            {

                mindatetime = result1.ReferenceDetail.CollectionDate.Value.AddDays(2);
            }

            result1.ReferenceDetail.CollectionDate = mindatetime;
            #endregion
            //update customerratecard in draft
            new DirectShipmentRepository().SaveDirectShipmnetDetail(result1);

            //step1. Mapping With directBookingDetail to ShipmentRequestDto
            var shipmentRequestDto = new UPSRepository().MapDirectBookingDetailToShipmentRequestDto(result1);

            //Step2.Create Shipment
            var shipmentResult = new UPSRepository().CreateShipment(shipmentRequestDto, result1.ReferenceDetail);

            //step3 Mapping ShipmentResonse to IntegrtaionResult
            var result = new UPSRepository().MapUPSIntegrationResponse(shipmentResult);

            if (result.Status)
            {
                //step1. After Shipment Create need to save the information in database 
                DirectShipmentid = new DirectShipmentRepository().SaveShipment(result1, result);
                if (DirectShipmentid > 0)
                {
                    var Id = new eCommerceUploadShipmentRepository().SaveBatchProcessProcessedShipment(1, result1.CustomerId);

                }
                if (result.CourierName == FrayteCourierCompany.UPS)
                {
                    //Step 1.1 Mapping ShipmentResult to IntegrtaionResult
                    new UPSRepository().MappingCourierPieceDetail(result, result1, DirectShipmentid);
                }

                //Step2 : Save Package Label Tracking detail
                new UPSRepository().SaveTrackingDetail(result1, result, DirectShipmentid);

                //Step3:  //Start downloading the images from UPS server and making PDF
                count = 1;
                foreach (var data in result.PieceTrackingDetails)
                {
                    if (result.CourierName == FrayteCourierCompany.UPS)
                    {
                        //Step3.1
                        data.LabelName = new UPSRepository().DownloadUPSImage(data, result.PieceTrackingDetails.Count(), count, DirectShipmentid);
                    }

                    // Step3.2
                    new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                    count++;
                }

                result1.Error = new FratyteError()
                {
                    Status = result.Status
                };
                FR.Status = true;
                //FrayteShipmentResponse = new APIShipmentRepository().MappingFrayteShipmentResponse(directBookingDetail, result, DirectShipmentid.ToString(), ratecard, Request.RateCardId);
            }
            else
            {
                FR.Status = false;
                result1.Error = result.Error;
                new eCommerceUploadShipmentRepository().SaveBatchProcessUnprocessedShipment(1, res.CustomerId);
                //Send mail to developer
                new ShipmentEmailRepository().SendShipmentErrorMail(result1, result1.Error);
            }
            return Tuple.Create(FR, result1, DirectShipmentid);
        }

        public Tuple<FrayteResult, DirectBookingShipmentDraftDetail, int, TNTResponseDto, List<FraytePackageTrackingDetail>> DBUploadShipmentTNTIntegration(FrayteUploadshipment res)
        {
            FrayteResult FR = new FrayteResult();
            var count = 0;
            int DirectShipmentid = 0;
            //string xsUserFolder = @"C:\FMS\" + "FrayteSchedularlog.txt";
            string xsUserFolder = @"D:\ProjectFrayte\" + "FrayteScheduler.txt";
            BaseLog.Instance.SetLogFile(xsUserFolder);
            Logger _log = Get_Log();
            _log.Error("entered in else tnt");
            var directBookingDetail = new DirectBookingUploadShipmentRepository().DirectBookingObj(res);
            #region COLLECTION Date 

            DateTime mindatetime = directBookingDetail.ReferenceDetail.CollectionDate.Value;
            DateTime maxdatetime = directBookingDetail.ReferenceDetail.CollectionDate.Value.AddDays(1);

            if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Sunday)
            {
                mindatetime = directBookingDetail.ReferenceDetail.CollectionDate.Value.AddDays(1);
            }
            else if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Saturday)
            {

                mindatetime = directBookingDetail.ReferenceDetail.CollectionDate.Value.AddDays(2);
            }

            directBookingDetail.ReferenceDetail.CollectionDate = mindatetime;
            #endregion
            _log.Error("entered in else tnt1");
            //update customerratecard in draft
            new DirectShipmentRepository().SaveDirectShipmnetDetail(directBookingDetail);
            _log.Error("entered in else tnt2");
            var shipmentPackageTrackingDetail = new List<FraytePackageTrackingDetail>();
            // TNT Integration is for HK as we have account detail for HK only
            _log.Error("entered in else tnt3");
            var TNTObj = new TNTRepository().MapDirectBookingObjToTNTobj(directBookingDetail);
            _log.Error("entered in else tnt4");
            var TNTXml = new TNTRepository().CreateTNTXMl(TNTObj);
            _log.Error("entered in else tnt5");
            var TNTResponse = new TNTRepository().CreateShipment(TNTXml, directBookingDetail.DirectShipmentDraftId);
            _log.Error("entered in else tnt6");
            directBookingDetail.Error = TNTResponse.Error; //TNTIntegration(directBookingDetail, out DirectShipmentid);
            _log.Error("entered in else tnt7");
            if (directBookingDetail.Error != null && directBookingDetail.Error.Status == true)
            {
                //Finally Save the Order Id too to get the information of multiple shpment in Tracking Detail page.
                DirectShipmentid = new DirectShipmentRepository().SaveShipment(directBookingDetail, TNTResponse.TNTShipmentResponse.TrackingCode, directBookingDetail.CustomerId, null, null);
                if (DirectShipmentid > 0)
                {
                    var Id = new eCommerceUploadShipmentRepository().SaveBatchProcessProcessedShipment(1, directBookingDetail.CustomerId);

                }
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

                if (directBookingDetail != null)
                {
                    directBookingDetail.Error = new FratyteError();
                    directBookingDetail.Error.Status = directBookingDetail.Error.Status;
                }
            }
            else
            {
                new eCommerceUploadShipmentRepository().SaveBatchProcessUnprocessedShipment(1, res.CustomerId);
            }


            return Tuple.Create(FR, directBookingDetail, DirectShipmentid, TNTResponse, shipmentPackageTrackingDetail);

        }

        public static Logger Get_Log()
        {
            Logger log = BaseLog.Instance.GetLogger(null);
            return log;
        }

        public Tuple<FrayteResult, DirectBookingShipmentDraftDetail, int> DBUploadShipmentDHLIntegration(FrayteUploadshipment res)
        {
            IntegrtaionResult result = new IntegrtaionResult();
            FrayteResult FR = new FrayteResult();
            //string xsUserFolder = @"C:\FMS\" + "FrayteSchedularlog.txt";
            string xsUserFolder = @"D:\ProjectFrayte\" + "FrayteScheduler.txt";
            BaseLog.Instance.SetLogFile(xsUserFolder);
            Logger _log = Get_Log();
            int DirectShipmentid = 0;
            var directBookingDetail = new DirectBookingUploadShipmentRepository().DirectBookingObj(res);
            #region COLLECTION Date 

            DateTime mindatetime = directBookingDetail.ReferenceDetail.CollectionDate.Value;
            DateTime maxdatetime = directBookingDetail.ReferenceDetail.CollectionDate.Value.AddDays(1);

            if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Sunday)
            {
                mindatetime = directBookingDetail.ReferenceDetail.CollectionDate.Value.AddDays(1);
            }
            else if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Saturday)
            {

                mindatetime = directBookingDetail.ReferenceDetail.CollectionDate.Value.AddDays(2);
            }

            directBookingDetail.ReferenceDetail.CollectionDate = mindatetime;
            #endregion
            _log.Error("Get direct Booking Object");
            //update customerratecard in draft
            new DirectShipmentRepository().SaveDirectShipmnetDetail(directBookingDetail);
            _log.Error("saved direct Booking Object");
            //step1. Mapping With directBookingDetail to ShipmentRequestDto
            var shipmentRequestDto = new DHLRepository().MapDirectBookingDetailToDHLShipmentRequestDto(directBookingDetail);

            //Step 2. Create Xml 
            var shipmentXML = new DHLRepository().CreateXMLForDHL(shipmentRequestDto);
            _log.Error("done xml dhl shipment");
            _log.Error(shipmentXML);
            string xml_in = "";
            try
            {
                xml_in = File.ReadAllText(shipmentXML);
                //xml_in = File.ReadAllText(@"C:\FMS\app.godemowithus.com\WebApi\UploadFiles\PDFGenerator\HTMLFile\tempDHLShipment.xml");
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
                _log.Error(e.InnerException);
                _log.Error(e.StackTrace);
            }
            _log.Error("reading xml dhl shipment");
            //step 3. Create Shipment
            var DHLResponse = new DHLRepository().CreateShipment(xml_in, shipmentRequestDto);
           // _log.Error("done integration dhl shipment" + DHLResponse.Error.ErrorDescription.ToString());
            //step 4. Mapping ShipmentResonse to IntegrtaionResult
            result = new DHLRepository().MapDHLIntegrationResponse(DHLResponse);
            if (result.Status)
            {
                //step2. After Shipment Create need to save the information in database 
                DirectShipmentid = new DirectShipmentRepository().SaveShipment(directBookingDetail, result);
                if (DirectShipmentid > 0)
                {
                    _log.Error("done save dhl shipment in db" + directBookingDetail.CustomerId);
                    var Id = new eCommerceUploadShipmentRepository().SaveBatchProcessProcessedShipment(1, directBookingDetail.CustomerId);
                    _log.Error(Id.ToString());
                }
                _log.Error("done save dhl shipment in db");
                if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DHL)
                {
                    //Step 1.1 Mapping ShipmentResult to IntegrtaionResult
                    new DHLRepository().MappingCourierPieceDetail(result, directBookingDetail, DirectShipmentid);
                }

                //Step3 : Save Package Label Tracking detail
                new DHLRepository().SaveTrackingDetail(directBookingDetail, result, DirectShipmentid);

                //Step4:  //Start downloading the images from UPS server and making PDF

                var count = 1;
                var totalpiece = result.PieceTrackingDetails.Count();
                totalpiece = totalpiece - 1;
                foreach (var data in result.PieceTrackingDetails.Take(totalpiece))
                {

                    if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DHL)
                    {
                        _log.Error("downloading shipment image");
                        //Step3.1
                        data.LabelName = new DHLRepository().DownloadDHLImage(data, totalpiece, count, DirectShipmentid);
                        _log.Error("downloaded shipment image");
                    }

                    //// Step3.2
                    //new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);
                    //_log.Error("saved package detail shipment image");
                    if (!data.PieceTrackingNumber.Contains("AirwayBillNumber_"))
                    {
                        // Step3.2
                        new DirectShipmentRepository().SavePackageDetail(data, result.CourierName);

                    }
                    count++;
                }
                var CourierPieceDetail = result.PieceTrackingDetails.Where(t => t.PieceTrackingNumber.Contains("AirwayBillNumber_")).FirstOrDefault();
                if (CourierPieceDetail != null)
                {
                    var data1 = new DHLRepository().DownloadDHLImage(CourierPieceDetail, totalpiece, 0, DirectShipmentid);
                }
                if (result != null)
                {
                    directBookingDetail.Error = new FratyteError();
                    directBookingDetail.Error.Status = result.Status;
                }

                return Tuple.Create(FR, directBookingDetail, DirectShipmentid);
            }
            else
            {
                directBookingDetail.Error = result.Error;
                new eCommerceUploadShipmentRepository().SaveBatchProcessUnprocessedShipment(1, directBookingDetail.CustomerId);
                //Send mail to developer
                new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, directBookingDetail.Error);
            }
            return Tuple.Create(FR, directBookingDetail, DirectShipmentid);
        }

        public Tuple<FrayteResult, DirectBookingShipmentDraftDetail, int> DBUploadShipmentParcelHubIntegration(FrayteUploadshipment res)
        {
            FrayteResult FR = new FrayteResult();
            IntegrtaionResult result = new IntegrtaionResult();
            var count = 0;
            int DirectShipmentid = 0;
            var directBookingDetail = new DirectBookingUploadShipmentRepository().DirectBookingObj(res);
            #region COLLECTION Date 

            DateTime mindatetime = directBookingDetail.ReferenceDetail.CollectionDate.Value;
            DateTime maxdatetime = directBookingDetail.ReferenceDetail.CollectionDate.Value.AddDays(1);

            if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Sunday)
            {
                mindatetime = directBookingDetail.ReferenceDetail.CollectionDate.Value.AddDays(1);
            }
            else if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Saturday)
            {

                mindatetime = directBookingDetail.ReferenceDetail.CollectionDate.Value.AddDays(2);
            }

            directBookingDetail.ReferenceDetail.CollectionDate = mindatetime;
            #endregion
            //update customerratecard in draft
            new DirectShipmentRepository().SaveDirectShipmnetDetail(directBookingDetail);

            // Parcelhub Integration starts
            //Step 2: Shipment Integrations
            #region Parcel Hub Integration

            //Map Direct Booking object with parcel hub objects
            Frayte.Services.Models.ParcelHub.ParcelHubShipmentRequest request = new ParcelHubRepository().MapDirectBookingDetailToShipmentRequest(directBookingDetail);

            //Create shipment in Parcel hub
            Frayte.Services.Models.ParcelHub.ParcelHubResponse response = new ParcelHubRepository().CreateShipment(request);

            if (response.Error.IsMailSend)
            {
                if (directBookingDetail.Error.IsMailSend)
                {
                    //Send error mail to developer
                    new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, response.Error);
                }
            }
            else
            {
                //Mapping ShipmentResonse to IntegrtaionResult                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
                result = new ParcelHubRepository().MappingParcelHubToIntegrationResult(directBookingDetail, response, null);

                if (result.Status)
                {
                    //Save Shipment Detail Into Our DB
                    DirectShipmentid = new DirectShipmentRepository().SaveShipment(directBookingDetail, result);
                    if (DirectShipmentid > 0)
                    {
                        var Id = new eCommerceUploadShipmentRepository().SaveBatchProcessProcessedShipment(1, directBookingDetail.CustomerId);

                    }
                    //Mapping ShipmentResult to IntegrtaionResult
                    new ParcelHubRepository().MappingCourierPieceDetail(result, directBookingDetail, DirectShipmentid);

                    //Save Package Label Tracking detail
                    new ParcelHubRepository().SaveTrackingDetail(directBookingDetail, result, DirectShipmentid);

                    //Start downloading the images from UPS server and making PDF
                    FratyteError Error = new ParcelHubRepository().DownloadParcelHubPackageImage(directBookingDetail, result, DirectShipmentid);

                }
                else
                {
                    if (result.Error.IsMailSend)
                    {
                        new eCommerceUploadShipmentRepository().SaveBatchProcessUnprocessedShipment(1, res.CustomerId);
                        //Send error mail to developer
                        new ShipmentEmailRepository().SendShipmentErrorMail(directBookingDetail, directBookingDetail.Error);
                    }
                }
            }


            #endregion

            return Tuple.Create(FR, directBookingDetail, DirectShipmentid);
        }
    }
}
