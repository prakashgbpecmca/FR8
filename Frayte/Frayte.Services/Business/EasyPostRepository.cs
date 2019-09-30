using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Frayte.Services.DataAccess;
using System.Drawing;
using EasyPost;
using Newtonsoft.Json;
using XStreamline.Log;

namespace Frayte.Services.Business
{
    public class EasyPostRepository
    {

        FrayteEntities dbContext = new FrayteEntities();

        #region EasyPost Integration
        public static Logger Get_Log()
        {
            Logger log = BaseLog.Instance.GetLogger(null);
            return log;
        }
        public EasyPostResult CreateShipment(EasyPostShipmentDetail shipmentDetail)
        {
            string xsUserFolder = @"C:\FMS\" + "FrayteSchedularlog.txt";
            //string xsUserFolder = @"D:\ProjectFrayte\" + "FrayteScheduler.txt";
            BaseLog.Instance.SetLogFile(xsUserFolder);
            Logger _log = Get_Log();
            _log.Error("Enter in easypost");
            EasyPostResult result = new EasyPostResult();
            var OperationZoneId = 0;
            result.Errors = new FratyteError();
            _log.Error("Enter in easypost1");
            result.Order = new Order();
            var Bat = AppSettings.ShipmentCreatedFrom;
            //if(Bat == "BATCH")
            //{
            //    OperationZoneId = new eCommerceUploadShipmentRepository().GetOperationZone();
            //}
            //else
            //{
            //     OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
            //}

            var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.EasyPost);
            _log.Error("Enter in easypost2");
            if (logisticIntegration != null)
            {
                _log.Error("Enter in easypost3");
                //Step 1: Setup Easy Post Key
                EasyPost.ClientManager.SetCurrent(logisticIntegration.InetgrationKey);
                _log.Error("Enter in easypost4");
                //Step 2: Create From Address
                EasyPost.Address fromAddress = new EasyPost.Address();
                setEasyPostFromAddress(shipmentDetail, result.Errors, fromAddress);
                _log.Error("Enter in easypost5");
                //Step 3. Create To Address            
                EasyPost.Address toAddress = new EasyPost.Address();
                setEasyPostToAddress(shipmentDetail, result.Errors, toAddress);
                _log.Error("Enter in easypost6");
                EasyPost.Order dbOrder = new EasyPost.Order();
                if (AppSettings.ApplicationMode == FrayteApplicationMode.Test)
                {
                    dbOrder.mode = EasyPostMode.Test;
                }
                else
                {
                    dbOrder.mode = EasyPostMode.Live;
                }
                _log.Error("Enter in easypost7");
                dbOrder.from_address = fromAddress;
                dbOrder.to_address = toAddress;
                dbOrder.shipments = new List<EasyPost.Shipment>();
                dbOrder.created_at = DateTime.Now;
                _log.Error("Enter in easypost8");
                // Step 4: Add custom info 
                EasyPost.CustomsInfo customsInfo = new EasyPost.CustomsInfo();

                //  customsInfo = CreateCustomInformation(eCommerceBookingDetail);
                  //dbOrder.customs_info = customsInfo;

                //Step 5: Create Shipment and add to orders 
                _log.Error("Enter in easypost9");
                setShipmentsToOrder(shipmentDetail, dbOrder);
                _log.Error("Enter in easypost10");
                //Step 6: create & buy order in easypost
                createEasyPostOrder(shipmentDetail, dbOrder, result.Errors);
                _log.Error("Enter in 11");
                if (result.Errors.Status)
                {
                    result.Order = dbOrder;
                }
            }
            else
            {
                _log.Error("Enter in easypost12");
                result.Errors.Status = false;
                result.Errors.Miscellaneous = new List<string>();
                result.Errors.Miscellaneous.Add("no logistic available for applied logistic");
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Fill the record in LogisticIntegration Table"));
            }

            return result;
        }

        public EasyPostResult CreateDirectBookingShipment(EasyPostShipmentDetail shipmentDetail)
        {
            EasyPostResult result = new EasyPostResult();
            result.Errors = new FratyteError();
            result.Order = new Order();

            var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.EasyPost);

            if (logisticIntegration != null)
            {
                //Step 1: Setup Easy Post Key
                EasyPost.ClientManager.SetCurrent(logisticIntegration.InetgrationKey);

                //Step 2: Create From Address
                EasyPost.Address fromAddress = new EasyPost.Address();
                setDirectBookingEasyPostFromAddress(shipmentDetail, result.Errors, fromAddress);

                //Step 3. Create To Address            
                EasyPost.Address toAddress = new EasyPost.Address();
                setDirectBookingEasyPostToAddress(shipmentDetail, result.Errors, toAddress);

                EasyPost.Order dbOrder = new EasyPost.Order();
                if (AppSettings.ApplicationMode == FrayteApplicationMode.Test)
                {
                    dbOrder.mode = EasyPostMode.Test;
                }
                else
                {
                    dbOrder.mode = EasyPostMode.Live;
                }

                dbOrder.from_address = fromAddress;
                dbOrder.to_address = toAddress;
                dbOrder.shipments = new List<EasyPost.Shipment>();
                dbOrder.created_at = DateTime.UtcNow;

                //Step 4: Create Shipment and add to orders
                setDirectBookingToOrder(shipmentDetail, dbOrder);

                //Step 5: Create & buy order in easypost
                createDirectBookingEasyPostOrder(shipmentDetail, dbOrder, result.Errors);

                if (result.Errors.Status)
                {
                    result.Order = dbOrder;
                }
            }
            else
            {
                result.Errors.Status = false;
                result.Errors.Miscellaneous = new List<string>();
                result.Errors.Miscellaneous.Add("No logistic available for applied logistic");
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Fill the record in LogisticIntegration Table"));
            }

            return result;
        }

        #region EasyPost Private Methods

        private void createEasyPostOrder(EasyPostShipmentDetail shipmentDetail, Order dbOrder, FratyteError frayteError)
        {
            try
            {
                dbOrder.Create();
                Dictionary<decimal, EasyPost.Rate> minEasyPostRate = new Dictionary<decimal, EasyPost.Rate>();
                EasyPost.Rate appliedRate = new EasyPost.Rate();

                // Need to set LogisticType in the Wrapper Class
                //var shippingMethod = new eCommerceShipmentRepository().GetLogisticService(shipmentDetail.ShipTo.Country.CountryId);                                                                                                                     // If Rate.Count == 0 then imple ment Address Verification and send info to ui
                if (dbOrder.rates != null && dbOrder.rates.Count == 0 && dbOrder.messages.Count > 0)
                {
                    if (!string.IsNullOrEmpty(shipmentDetail.EasyPostService.Courier))
                    {
                        frayteError.Address = new List<string>();
                        frayteError.Custom = new List<string>();
                        frayteError.Package = new List<string>();
                        frayteError.Miscellaneous = new List<string>();
                        frayteError.Service = new List<string>();
                        frayteError.ServiceError = new List<string>();
                        foreach (var err in dbOrder.messages)
                        {
                            FrayteGeneralError error = JsonConvert.DeserializeObject<FrayteGeneralError>(err);
                            if (error.carrier == shipmentDetail.EasyPostService.Courier)
                            {
                                if (error.message.Contains("country") || error.message.Contains("postal code") || error.message.Contains("postcode"))
                                {
                                    frayteError.Address.Add(error.message);
                                }
                                else if (error.message.Contains("custom"))
                                {
                                    frayteError.Custom.Add(error.message);
                                }
                                else if (error.message.Contains("package"))
                                {
                                    frayteError.Package.Add(error.message);
                                }
                                else
                                {
                                    frayteError.IsMailSend = true;
                                    frayteError.Miscellaneous.Add(error.message);
                                }

                                SaveDirectShipmentObject(Newtonsoft.Json.JsonConvert.SerializeObject(dbOrder).ToString(), shipmentDetail.DraftShipmentId);
                                SaveEasyPostErrorObject(error.message, shipmentDetail.DraftShipmentId);

                            }
                        }
                    }
                    frayteError.Status = false;
                }
                else if (dbOrder.rates != null && dbOrder.rates.Count > 0)
                {
                    foreach (EasyPost.Rate rate in dbOrder.rates)
                    {
                        if (!string.IsNullOrEmpty(rate.service) &&
                            !string.IsNullOrEmpty(rate.carrier) && !string.IsNullOrEmpty(rate.carrier_account_id) &&
                            rate.carrier == shipmentDetail.EasyPostService.Courier && rate.carrier_account_id == shipmentDetail.EasyPostService.CourierAccountId)
                        {
                            if (!minEasyPostRate.ContainsKey(CommonConversion.ConvertToDecimal(rate.rate)))
                            {
                                minEasyPostRate.Add(CommonConversion.ConvertToDecimal(rate.rate), rate);
                            }
                        }
                    }
                    if (minEasyPostRate.Count > 0)
                    {
                        appliedRate = minEasyPostRate.OrderBy(p => p.Key).First().Value;
                        dbOrder.Buy(appliedRate.carrier, appliedRate.service);
                        frayteError.Status = true;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(shipmentDetail.EasyPostService.Courier))
                        {
                            frayteError.Address = new List<string>();
                            frayteError.Custom = new List<string>();
                            frayteError.Package = new List<string>();
                            frayteError.Miscellaneous = new List<string>();
                            frayteError.Service = new List<string>();
                            frayteError.ServiceError = new List<string>();
                            foreach (var err in dbOrder.messages)
                            {
                                FrayteGeneralError error = JsonConvert.DeserializeObject<FrayteGeneralError>(err);
                                if (error.carrier == shipmentDetail.EasyPostService.Courier)
                                {
                                    if (error.message.Contains("country") || error.message.Contains("postal code") || error.message.Contains("postcode"))
                                    {
                                        frayteError.Address.Add(error.message);
                                    }
                                    else if (error.message.Contains("custom"))
                                    {
                                        frayteError.Custom.Add(error.message);
                                    }
                                    else if (error.message.Contains("package"))
                                    {
                                        frayteError.Package.Add(error.message);
                                    }
                                    else
                                    {
                                        frayteError.IsMailSend = true;
                                        frayteError.Miscellaneous.Add(error.message);
                                    }
                                    SaveDirectShipmentObject(Newtonsoft.Json.JsonConvert.SerializeObject(dbOrder).ToString(), shipmentDetail.DraftShipmentId);
                                    SaveEasyPostErrorObject(error.message, shipmentDetail.DraftShipmentId);
                                }
                            }
                        }
                        frayteError.Status = false;
                    }
                }
            }
            catch (EasyPost.HttpException ex)
            {
                frayteError.Address = new List<string>();
                frayteError.Custom = new List<string>();
                frayteError.Package = new List<string>();
                frayteError.Miscellaneous = new List<string>();
                frayteError.Service = new List<string>();
                frayteError.ServiceError = new List<string>();
                FrayteGeneralError error = JsonConvert.DeserializeObject<FrayteGeneralError>(ex.Message);
                if (!string.IsNullOrEmpty(error.message))
                {
                    frayteError.Miscellaneous.Add(error.message);
                    SaveDirectShipmentObject(Newtonsoft.Json.JsonConvert.SerializeObject(dbOrder).ToString(), shipmentDetail.DraftShipmentId);
                    SaveEasyPostErrorObject(error.message, shipmentDetail.DraftShipmentId);
                }
                else
                {
                    FrayteEasyPostError error1 = JsonConvert.DeserializeObject<FrayteEasyPostError>(ex.Message);
                    if (error1.error != null && !string.IsNullOrEmpty(error1.error.message))
                    {
                        frayteError.Miscellaneous.Add(error1.error.message);
                    }
                    else
                    {
                        frayteError.Miscellaneous.Add("Error While Buying the Shipment");
                    }
                    SaveDirectShipmentObject(Newtonsoft.Json.JsonConvert.SerializeObject(dbOrder).ToString(), shipmentDetail.DraftShipmentId);
                    SaveEasyPostErrorObject(error1.error.message, shipmentDetail.DraftShipmentId);
                }

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                frayteError.Status = false;

            }
        }

        private void setShipmentsToOrder(EasyPostShipmentDetail shipmentDetail, Order dbOrder)
        {
            string xsUserFolder = @"C:\FMS\" + "FrayteSchedularlog.txt";
            //string xsUserFolder = @"D:\ProjectFrayte\" + "FrayteScheduler.txt";
            BaseLog.Instance.SetLogFile(xsUserFolder);
            Logger _log = Get_Log();
            _log.Error("Enter in easypostn1");
            foreach (var dbPackage in shipmentDetail.Packages)
            {
                _log.Error("Enter in easypostn2");
                if (dbPackage.CartoonValue > 0)
                {
                    _log.Error("Enter in easypostn3");
                    for (int i = 0; i < dbPackage.CartoonValue; i++)
                    {
                        _log.Error("Enter in easypostn4");
                        var weight = 0M;
                        var width = 0M;
                        var height = 0M;
                        var length = 0M;
                        if (shipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms)
                        {
                            weight = dbPackage.Weight * 35.274M;
                            width = dbPackage.Width * 0.393701M;
                            height = dbPackage.Height * 0.393701M;
                            length = dbPackage.Length * 0.393701M;

                        }
                        else if (shipmentDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs)
                        {
                            weight = dbPackage.Weight * 16;
                            width = dbPackage.Width;
                            height = dbPackage.Height;
                            length = dbPackage.Length;
                        }
                        else
                        {
                            weight = dbPackage.Weight;
                            width = dbPackage.Width;
                            height = dbPackage.Height;
                            length = dbPackage.Length;
                        }

                        //Step 4. Create Parcel            
                        EasyPost.Parcel parcel = EasyPost.Parcel.Create(new Dictionary<string, object>()
                        {
                          {"length", length},
                          {"width", width},
                          {"height", height},
                          {"weight", weight}
                        });

                        string parcelId = parcel.id;

                        EasyPost.Shipment shipment = new EasyPost.Shipment();

                        _log.Error("Enter in easypostn5");
                        // Check that originating and destinating country are different 
                        // For DirectBooking
                        if (shipmentDetail.ShipFrom.Country.CountryId != shipmentDetail.ShipTo.Country.CountryId)
                        {
                            _log.Error("Enter in easypostn6");

                            CreateCustomItem(shipmentDetail, dbOrder.customs_info, dbPackage);
                            _log.Error("Enter in easypostn7");
                        }
                        _log.Error("Enter in easypostn8");
                        //Step 5. Create Shipment                             
                        shipment.from_address = dbOrder.from_address;
                        shipment.to_address = dbOrder.to_address;
                        shipment.parcel = parcel;
                        if (shipmentDetail.PayTaxAndDuties == FrayteTermsOfTrade.Receiver)
                        {
                            _log.Error("Enter in easypostn9");
                            shipment.options = new EasyPost.Options()
                            {

                                print_custom_1 = shipmentDetail.FrayteNumber + "-" + shipmentDetail.ReferenceDetail.Reference1,
                                delivered_duty_paid = false,
                                bill_receiver_account = "419381297"
                                // bill_receiver_account = "959106125" 419381297
                            };
                            _log.Error("Enter in easypostn10");
                        }
                        else
                        {
                            _log.Error("Enter in easypostn11");
                            shipment.options = new EasyPost.Options()
                            {
                                print_custom_1 = shipmentDetail.FrayteNumber + "-" + shipmentDetail.ReferenceDetail.Reference1,
                                delivered_duty_paid = false,
                                // bill_receiver_account = "419381297"
                            };
                        }
                        dbOrder.shipments.Add(shipment);
                    }
                }
            }
        }

        // For Direct Booking For now
        private void CreateCustomItem(EasyPostShipmentDetail shipmentDetail, EasyPost.CustomsInfo customsInfo, EasyPostPackage dbPackage)
        {
            var weight = 0M;
            if (shipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms)
            {
                weight = dbPackage.Weight * 35.274M;
            }
            else if (shipmentDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs)
            {
                weight = dbPackage.Weight * 16;
            }
            else
            {
                weight = dbPackage.Weight;
            }
            EasyPost.CustomsItem customsItem1 = EasyPost.CustomsItem.Create(new Dictionary<string, object>() {

                  {"description", dbPackage.Content},
                  {"quantity", 1},
                  {"weight", weight},
                  {"value", dbPackage.Value},
                  {"origin_country", shipmentDetail.ShipFrom.Country.Code2},
                  {"hs_tariff_number", ""}
                });
            customsInfo.customs_items.Add(customsItem1);
        }

        // For Direct Booking For now
        private EasyPost.CustomsInfo CreateDirectBookingCustomInformation(EasyPostShipmentDetail shipmentDetail)
        {
            EasyPost.CustomsInfo customsInfo = new EasyPost.CustomsInfo();
            customsInfo.customs_certify = shipmentDetail.CustomInfo.CustomsCertify.Value ? "true" : "false";
            customsInfo.eel_pfc = shipmentDetail.CustomInfo.EelPfc;
            customsInfo.customs_signer = shipmentDetail.CustomInfo.CustomsSigner;
            customsInfo.contents_type = shipmentDetail.CustomInfo.ContentsType;
            customsInfo.contents_explanation = shipmentDetail.CustomInfo.ContentsExplanation;
            customsInfo.non_delivery_option = shipmentDetail.CustomInfo.NonDeliveryOption;
            customsInfo.restriction_type = shipmentDetail.CustomInfo.RestrictionType;
            customsInfo.restriction_comments = shipmentDetail.CustomInfo.RestrictionComments;
            customsInfo.customs_items = new List<EasyPost.CustomsItem>();

            return customsInfo;
        }

        private void setEasyPostToAddress(EasyPostShipmentDetail shipmentDetail, FratyteError frayteError, EasyPost.Address toAddress)
        {
            var shipToCompany = "";
            if (string.IsNullOrEmpty(shipmentDetail.ShipTo.CompanyName))
            {
                shipToCompany = shipmentDetail.ShipTo.FirstName + " " + shipmentDetail.ShipTo.LastName;
                toAddress.company = shipToCompany;
            }
            else
            {
                toAddress.company = shipmentDetail.ShipTo.CompanyName;
            }
            toAddress.street1 = shipmentDetail.ShipTo.Address;
            toAddress.street2 = shipmentDetail.ShipTo.Address2;
            toAddress.city = shipmentDetail.ShipTo.City;
            toAddress.state = shipmentDetail.ShipTo.State;
            toAddress.country = shipmentDetail.ShipTo.Country.Code2;
            toAddress.zip = shipmentDetail.ShipTo.PostCode;
            toAddress.phone = shipmentDetail.ShipTo.Phone;

            try
            {
                toAddress.Create();
            }
            catch (EasyPost.HttpException e)
            {
                FrayteGeneralError error = JsonConvert.DeserializeObject<FrayteGeneralError>(e.Message);
                frayteError.Address.Add(error.message);
                frayteError.Status = false;

            }

        }

        private void setEasyPostFromAddress(EasyPostShipmentDetail shipmentDetail, FratyteError frayteError, EasyPost.Address fromAddress)
        {
            var shipFromCompany = "";
            var wareHouse = new eCommerceShipmentRepository().getWareHouseDetail(shipmentDetail.ShipTo.Country.CountryId);
            if (wareHouse != null)
            {
                //  shipmentDetail.WareHouseId = wareHouse.WarehouseId; Don't need this
                if (string.IsNullOrEmpty(shipmentDetail.ShipFrom.CompanyName))
                {
                    shipFromCompany = shipmentDetail.ShipFrom.FirstName + " " + shipmentDetail.ShipFrom.LastName;
                    fromAddress.company = shipFromCompany;
                }
                else
                {
                    fromAddress.company = shipmentDetail.ShipFrom.CompanyName;
                }
                fromAddress.street1 = wareHouse.Address;
                fromAddress.street2 = wareHouse.Address2;
                fromAddress.city = wareHouse.City;
                fromAddress.state = wareHouse.State;
                fromAddress.country = wareHouse.Country.Code2;
                fromAddress.zip = wareHouse.Zip;
                fromAddress.phone = wareHouse.TelephoneNo;
            }
            else
            {
                if (string.IsNullOrEmpty(shipmentDetail.ShipFrom.CompanyName))
                {
                    shipFromCompany = shipmentDetail.ShipFrom.FirstName + " " + shipmentDetail.ShipFrom.LastName;
                    fromAddress.company = shipFromCompany;
                }
                else
                {
                    fromAddress.company = shipmentDetail.ShipFrom.CompanyName;
                }
                fromAddress.street1 = shipmentDetail.ShipFrom.Address;
                fromAddress.street2 = shipmentDetail.ShipFrom.Address2;
                fromAddress.city = shipmentDetail.ShipFrom.City;
                fromAddress.state = shipmentDetail.ShipFrom.State;
                fromAddress.country = shipmentDetail.ShipFrom.Country.Code2;
                fromAddress.zip = shipmentDetail.ShipFrom.PostCode;
                fromAddress.phone = shipmentDetail.ShipFrom.Phone;
            }
            try
            {
                fromAddress.Create();
            }
            catch (EasyPost.HttpException e)
            {
                FrayteGeneralError error = JsonConvert.DeserializeObject<FrayteGeneralError>(e.Message);
                frayteError.Address.Add(error.message);
                frayteError.Status = false;
            }
        }

        #endregion

        #region EasyPost Error Logging

        private void SaveDirectShipmentObject(string ShipmetObject, int ShipmentDraftId)
        {
            try
            {
                var detail = dbContext.DirectShipmentDrafts.Find(ShipmentDraftId);
                if (detail != null)
                {
                    detail.EasyPostOrderObject = ShipmetObject;
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

        private void SaveEasyPostErrorObject(string ErrorObject, int ShipmentDraftId)
        {
            try
            {
                var detail = dbContext.DirectShipmentDrafts.Find(ShipmentDraftId);
                if (detail != null)
                {
                    detail.EasyPostErrorObject = ErrorObject;
                    detail.LastUpdated = DateTime.UtcNow;
                    dbContext.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(ErrorObject));
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        private void SaveEasyPosyPickUpObject(string PickUpObject, int ShipmentDraftId)
        {
            try
            {
                var detail = dbContext.DirectShipmentDrafts.Find(ShipmentDraftId);
                if (detail != null)
                {
                    detail.EasyPostPickUpObject = PickUpObject;
                    detail.LastUpdated = DateTime.UtcNow;
                    dbContext.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(PickUpObject));
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        #endregion

        #endregion

        #region EasyPost Labels

        public FrayteResult CropeImage(FrayteCommercePackageTrackingDetail data, string CourierCompany, string easypostImage, string labelPath, int calculatedHeight, int LocationY, int of, int total)
        {
            FrayteResult result = new FrayteResult();
            string name = string.Empty;
            if (CourierCompany == FrayteCourierCompany.DHLExpress || CourierCompany == FrayteCourierCompany.DHL || CourierCompany == FrayteCourierCompany.DHL_Express)
            {
                name = FrayteShortName.DHL;
            }
            //demo Code
            //Image Crop Example - DHL
            string Image = name + "_" + data.TrackingNo + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + of + "of" + total + ")" + ".jpg";
            Bitmap source = new Bitmap(labelPath + easypostImage);
            Rectangle section = new Rectangle(new Point(0, LocationY), new Size(800, 1400));
            Bitmap CroppedImage = CropImage(source, section);
            CroppedImage.Save(labelPath + Image, System.Drawing.Imaging.ImageFormat.Jpeg);

            try
            {
                new eCommerceShipmentRepository().SaveImage(data, Image);
                result.Status = true;
            }
            catch (Exception ex)
            {
                result.Status = false;
            }
            return result;
        }

        #region Private Methods
        private Bitmap CropImage(Bitmap source, Rectangle section)
        {
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(section.Width, section.Height);

            Graphics g = Graphics.FromImage(bmp);

            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

            return bmp;
        }
        #endregion

        #endregion

        #region Map shipment to easypost

        #region Map DirectShipmentToEasyPostObj

        public EasyPostShipmentDetail MapShipmentToEasyPostShipment(FrayteCommerceShipmentDraft shipmentDetail)
        {
            try
            {
                EasyPostShipmentDetail shipment = new EasyPostShipmentDetail();

                shipment.DraftShipmentId = shipmentDetail.DirectShipmentDraftId;

                // Step 1: Map ShipFrom
                shipment.ShipFrom = new EasyPostAddress();
                mapAddress(shipmentDetail.ShipTo.Country.CountryId, shipmentDetail.ShipFrom, shipment.ShipFrom, EasyPostAddressType.ShipFrom);

                // Step 2: Map ShipFrom
                shipment.ShipTo = new EasyPostAddress();
                mapAddress(shipmentDetail.ShipTo.Country.CountryId, shipmentDetail.ShipTo, shipment.ShipTo, EasyPostAddressType.ShipTo);

                // Step 3: Map Packages
                shipment.Packages = new List<EasyPostPackage>();
                mapPackages(shipmentDetail.Packages, shipment.Packages);

                // Step 4 : map custom detail ( No Custom info for UK To UK in eCommerce )
                shipment.CustomInfo = new EasyPostCustomInformation();
                //mapCustomInfo(shipmentDetail.CustomInfo, shipment.CustomInfo);

                // Step 5: map easypost service
                shipment.EasyPostService = new EasyPostServicerService();
                mapService(shipmentDetail, shipment.EasyPostService);

                // Step 6: map other shipment detail
                mapOtherShipmentInfo(shipmentDetail, shipment);

                return shipment;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Could not map eCommerc shipment to easypost shipment : " + ex.Message));
                return null;
            }
        }

        public EasyPostShipmentDetail MapDirectShipmentToEasyPostShipment(DirectBookingShipmentDraftDetail shipmentDetail)
        {
            try
            {
                EasyPostShipmentDetail shipment = new EasyPostShipmentDetail();

                shipment.DraftShipmentId = shipmentDetail.DirectShipmentDraftId;

                // Step 1: Map ShipFrom
                shipment.ShipFrom = new EasyPostAddress();
                mapDirectBookingAddress(shipmentDetail.ShipFrom.Country.CountryId, shipmentDetail.ShipFrom, shipment.ShipFrom, EasyPostAddressType.ShipFrom);

                // Step 2: Map ShipFrom
                shipment.ShipTo = new EasyPostAddress();
                mapDirectBookingAddress(shipmentDetail.ShipTo.Country.CountryId, shipmentDetail.ShipTo, shipment.ShipTo, EasyPostAddressType.ShipTo);

                // Step 3: Map Packages
                shipment.Packages = new List<EasyPostPackage>();
                mapDirectBookingPackages(shipmentDetail.Packages, shipment.Packages);

                // Step 4 : map custom detail ( No Custom info for UK To UK in eCommerce )
                shipment.CustomInfo = new EasyPostCustomInformation();
                mapDirectBookingCustomInfo(shipmentDetail.CustomInfo, shipment.CustomInfo);

                // Step 5: map easypost service
                shipment.EasyPostService = new EasyPostServicerService();
                mapDirectBookingService(shipmentDetail, shipment.EasyPostService);

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

        #region Private Methods

        private void mapAddress(int ShipToCountrryId, eCommerceShipmentAddressDraft address, EasyPostAddress easyPostAddress, string addressType)
        {
            var wareHouseDetail = dbContext.Warehouses.Where(p => p.CountryId == ShipToCountrryId).FirstOrDefault();

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

            if (addressType == EasyPostAddressType.ShipFrom && wareHouseDetail != null)
            {
                easyPostAddress.FirstName = address.FirstName;
                easyPostAddress.LastName = address.LastName;
                easyPostAddress.CompanyName = address.CompanyName;
                easyPostAddress.Country = new EasyPostCountry();
                easyPostAddress.Country.Name = address.Country.Name;
                easyPostAddress.Address = wareHouseDetail.Address;
                easyPostAddress.Address2 = wareHouseDetail.Address2;
                easyPostAddress.City = wareHouseDetail.City;
                easyPostAddress.State = wareHouseDetail.State;
                easyPostAddress.Phone = wareHouseDetail.TelephoneNo;
                easyPostAddress.PostCode = wareHouseDetail.Zip;
                easyPostAddress.Country.CountryId = Country.CountryId;
                easyPostAddress.Country.Code = Country.CountryCode;
                easyPostAddress.Country.Code2 = Country.CountryCode2;
            }
            else
            {
                easyPostAddress.FirstName = address.FirstName;
                easyPostAddress.LastName = address.LastName;
                easyPostAddress.CompanyName = address.CompanyName;
                easyPostAddress.Country = new EasyPostCountry();
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

        private void mapPackages(List<PackageDraft> packages, List<EasyPostPackage> easyPostPackages)
        {
            EasyPostPackage package;
            foreach (var data in packages)
            {
                package = new EasyPostPackage();
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

        private void mapCustomInfo(CustomInformation customInfo, EasyPostCustomInformation easyPostCustomInfo)
        {

        }

        private void mapService(FrayteCommerceShipmentDraft shipmentDetail, EasyPostServicerService easyPostService)
        {
            var shippingMethod = new eCommerceShipmentRepository().GetLogisticService(shipmentDetail.ShipTo.Country.CountryId);
            if (shippingMethod != null && !string.IsNullOrEmpty(shippingMethod.LogisticService))
            {
                easyPostService.Courier = shippingMethod.LogisticService;
                easyPostService.CourierDisplay = shippingMethod.LogisicServiceDisplay;
                easyPostService.CourierAccountId = shippingMethod.AccountId;
                easyPostService.CourierAccountNumber = shippingMethod.AccountNo;
                shipmentDetail.CourierCompany = shippingMethod.LogisticService;
                shipmentDetail.CourierCompanyDisplay = shippingMethod.LogisicServiceDisplay;
            }
        }

        private void mapOtherShipmentInfo(FrayteCommerceShipmentDraft shipmentDetail, EasyPostShipmentDetail shipment)
        {
            shipment.BookingApp = shipmentDetail.BookingStatusType;
            shipment.ModuleType = shipmentDetail.ModuleType;
            shipment.Currency = new EasyPostCurrency();
            shipment.Currency.CurrencyCode = shipmentDetail.Currency.CurrencyCode;
            shipment.Currency.CurrencyDescription = shipmentDetail.Currency.CurrencyDescription;

            shipment.FrayteNumber = shipmentDetail.FrayteNumber;
            shipment.PakageCalculatonType = shipmentDetail.PakageCalculatonType;
            shipment.ParcelType = new EasyPostParcelType();
            shipment.ParcelType.ParcelType = shipmentDetail.ParcelType.ParcelType;
            shipment.ParcelType.ParcelDescription = shipmentDetail.ParcelType.ParcelDescription;
            shipment.PaymentPartyAccountNumber = shipmentDetail.PaymentPartyAccountNumber;
            shipment.PayTaxAndDuties = shipmentDetail.PayTaxAndDuties;
            shipment.TaxAndDutiesAcceptedBy = shipmentDetail.TaxAndDutiesAcceptedBy;
            shipment.CreatedOn = shipmentDetail.CreatedOn;
            shipment.ReferenceDetail = new EasyPostReferenceDetail();
            shipment.ReferenceDetail.Reference1 = shipmentDetail.ReferenceDetail.Reference1;
            shipment.ReferenceDetail.CollectionDate = shipmentDetail.ReferenceDetail.CollectionDate;
            shipment.ReferenceDetail.CollectionTime = shipmentDetail.ReferenceDetail.CollectionTime;
            shipment.ReferenceDetail.ContentDescription = shipmentDetail.ReferenceDetail.ContentDescription;
            shipment.ReferenceDetail.SpecialInstruction = shipmentDetail.ReferenceDetail.SpecialInstruction;
        }

        #endregion

        #endregion

        #region Map UploadShipmentToEasyPostObject

        public EasyPostShipmentDetail MapUploadShipmentToEasyPost(FrayteUploadshipment fs)
        {
            EasyPostShipmentDetail EPSD = new EasyPostShipmentDetail();
            EPSD.Packages = new List<EasyPostPackage>();
            if (fs != null)
            {
                var wareHouseDetail = dbContext.Warehouses.Where(p => p.CountryId == fs.ShipTo.Country.CountryId).FirstOrDefault();

                var Country = dbContext.Countries.Where(p => p.CountryId == fs.ShipTo.Country.CountryId).FirstOrDefault();

                //string Company = string.Empty;
                //if (string.IsNullOrEmpty(address.CompanyName))
                //{
                //    Company = address.FirstName + " " + address.LastName;
                //    easyPostAddress.CompanyName = Company;
                //}
                //else
                //{
                //    easyPostAddress.CompanyName = address.CompanyName;
                //}
                EPSD.DraftShipmentId = fs.DirectShipmentDraftId;
                //EPSD.ShipFrom = new EasyPostAddress()
                //{
                //    Country = new EasyPostCountry()
                //    {
                //        Code = fs.ShipFrom.Country.Code,
                //        Code2 = fs.ShipFrom.Country.Code2,
                //        CountryId = fs.ShipFrom.Country.CountryId,
                //        Name = fs.ShipFrom.Country.Name
                //    },
                //    PostCode = fs.ShipFrom.PostCode,
                //    FirstName = fs.ShipFrom.FirstName,
                //    LastName = fs.ShipFrom.LastName,
                //    CompanyName = fs.ShipFrom.CompanyName,
                //    Address = fs.ShipFrom.Address,
                //    Address2 = fs.ShipFrom.Address2,
                //    City = fs.ShipFrom.City,
                //    Phone = fs.ShipFrom.Phone,
                //    Email = fs.ShipFrom.Email

                //};
                EPSD.ShipFrom = new EasyPostAddress()
                {
                    Country = new EasyPostCountry()
                    {
                        Code = Country.CountryCode,
                        Code2 = Country.CountryCode2,
                        CountryId = Country.CountryId,
                        Name = Country.CountryName
                    },
                    PostCode = wareHouseDetail.Zip,
                    FirstName = fs.ShipFrom.FirstName,
                    LastName = fs.ShipFrom.LastName,
                    CompanyName = fs.ShipFrom.CompanyName,
                    Address = wareHouseDetail.Address,
                    Address2 = wareHouseDetail.Address2,
                    City = wareHouseDetail.City,
                    Phone = wareHouseDetail.TelephoneNo,
                    Email = wareHouseDetail.Email

                };
                EPSD.ShipTo = new EasyPostAddress()
                {
                    Country = new EasyPostCountry()
                    {
                        Code = fs.ShipTo.Country.Code,
                        Code2 = fs.ShipTo.Country.Code2,
                        CountryId = fs.ShipTo.Country.CountryId,
                        Name = fs.ShipTo.Country.Name
                    },
                    PostCode = fs.ShipTo.PostCode,
                    FirstName = fs.ShipTo.FirstName,
                    LastName = fs.ShipTo.LastName,
                    CompanyName = fs.ShipTo.CompanyName,
                    Address = fs.ShipTo.Address,
                    Address2 = fs.ShipTo.Address2,
                    City = fs.ShipTo.City,
                    Phone = fs.ShipTo.Phone,
                    Email = fs.ShipTo.Email

                };
                EPSD.PakageCalculatonType = fs.PackageCalculationType;
                EPSD.PayTaxAndDuties = fs.PayTaxAndDuties;
                EPSD.ParcelType = new EasyPostParcelType()
                {
                    ParcelType = fs.parcelType,
                    ParcelDescription = ""
                };
                EPSD.Currency = new EasyPostCurrency()
                {
                    CurrencyCode = fs.CurrencyCode,
                    CurrencyDescription = ""
                };
                EPSD.ReferenceDetail = new EasyPostReferenceDetail()
                {
                    Reference1 = fs.ShipmentReference,
                    ContentDescription = fs.ShipmentDescription,

                };

                EPSD.EasyPostService = new EasyPostServicerService()
                {
                    Courier = fs.Service.LogisticService,
                    CourierDisplay = fs.Service.LogisticServiceDisplay,
                    CourierAccountId = fs.Service.AccountId,
                    CourierAccountNumber = fs.Service.AccountNo.ToString()
                };

                foreach (var package in fs.Package)
                {
                    var pack = new EasyPostPackage();
                    pack.CartoonValue = package.CartoonValue;
                    pack.Content = package.Content;
                    pack.Height = package.Height;
                    pack.Length = package.Length;
                    pack.Value = package.Value;
                    pack.Weight = package.Weight;
                    pack.Width = package.Width;
                    EPSD.Packages.Add(pack);
                }
            }

            return EPSD;
        }

        #endregion

        #endregion

        #region Map DirectBooking to easyost and integrate in easy post

        internal protected void mapDirectBookingAddress(int ShipToCountrryId, DirectBookingDraftCollection address, EasyPostAddress easyPostAddress, string addressType)
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

            if (addressType == EasyPostAddressType.ShipFrom)
            {
                easyPostAddress.FirstName = address.FirstName;
                easyPostAddress.LastName = address.LastName;
                easyPostAddress.CompanyName = address.CompanyName;
                easyPostAddress.Country = new EasyPostCountry();
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
                easyPostAddress.Country = new EasyPostCountry();
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

        internal protected void mapDirectBookingPackages(List<PackageDraft> packages, List<EasyPostPackage> easyPostPackages)
        {
            EasyPostPackage package;
            foreach (var data in packages)
            {
                package = new EasyPostPackage();
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

        internal protected void mapDirectBookingService(DirectBookingShipmentDraftDetail shipmentDetail, EasyPostServicerService easyPostService)
        {
            if (shipmentDetail.CustomerRateCard != null)
            {
                easyPostService.Courier = shipmentDetail.CustomerRateCard.CourierName;
                easyPostService.CourierDisplay = shipmentDetail.CustomerRateCard.DisplayName;
                easyPostService.CourierAccountId = shipmentDetail.CustomerRateCard.IntegrationAccountId;
                easyPostService.CourierAccountNumber = shipmentDetail.CustomerRateCard.CourierAccountNo;
                easyPostService.RateType = shipmentDetail.CustomerRateCard.RateType;
            }
        }

        internal protected void mapDirectBookingCustomInfo(CustomInformation customInfo, EasyPostCustomInformation easyPostCustomInfo)
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

        internal protected void mapOtherDirectBookingInfo(DirectBookingShipmentDraftDetail shipmentDetail, EasyPostShipmentDetail shipment)
        {
            shipment.BookingApp = shipmentDetail.BookingStatusType;
            shipment.ModuleType = FrayteShipmentServiceType.DirectBooking;
            shipment.Currency = new EasyPostCurrency();
            shipment.Currency.CurrencyCode = shipmentDetail.Currency.CurrencyCode;
            shipment.Currency.CurrencyDescription = shipmentDetail.Currency.CurrencyDescription;

            shipment.FrayteNumber = shipmentDetail.FrayteNumber;
            shipment.PakageCalculatonType = shipmentDetail.PakageCalculatonType;
            shipment.ParcelType = new EasyPostParcelType();
            shipment.ParcelType.ParcelType = shipmentDetail.ParcelType.ParcelType;
            shipment.ParcelType.ParcelDescription = shipmentDetail.ParcelType.ParcelDescription;
            shipment.PaymentPartyAccountNumber = shipmentDetail.PaymentPartyAccountNumber;
            shipment.PayTaxAndDuties = shipmentDetail.PayTaxAndDuties;
            shipment.TaxAndDutiesAcceptedBy = shipmentDetail.TaxAndDutiesAcceptedBy;
            shipment.CreatedOn = DateTime.UtcNow;
            shipment.ReferenceDetail = new EasyPostReferenceDetail();
            shipment.ReferenceDetail.Reference1 = shipmentDetail.ReferenceDetail.Reference1;
            shipment.ReferenceDetail.CollectionDate = shipmentDetail.ReferenceDetail.CollectionDate;
            shipment.ReferenceDetail.CollectionTime = shipmentDetail.ReferenceDetail.CollectionTime;
            shipment.ReferenceDetail.ContentDescription = shipmentDetail.ReferenceDetail.ContentDescription;
            shipment.ReferenceDetail.SpecialInstruction = shipmentDetail.ReferenceDetail.SpecialInstruction;
        }

        internal protected void setDirectBookingEasyPostToAddress(EasyPostShipmentDetail shipmentDetail, FratyteError frayteError, EasyPost.Address toAddress)
        {
            var shipToCompany = "";
            if (string.IsNullOrEmpty(shipmentDetail.ShipTo.CompanyName))
            {
                shipToCompany = shipmentDetail.ShipTo.FirstName + " " + shipmentDetail.ShipTo.LastName;
                toAddress.company = shipToCompany;
            }
            else
            {
                toAddress.company = shipmentDetail.ShipTo.CompanyName;
            }
            toAddress.name = shipmentDetail.ShipTo.FirstName + " " + shipmentDetail.ShipTo.LastName;
            toAddress.street1 = shipmentDetail.ShipTo.Address;
            toAddress.street2 = shipmentDetail.ShipTo.Address2;
            toAddress.city = shipmentDetail.ShipTo.City;
            toAddress.state = shipmentDetail.ShipTo.State;
            toAddress.country = shipmentDetail.ShipTo.Country.Code2;
            toAddress.zip = shipmentDetail.ShipTo.PostCode;
            toAddress.phone = shipmentDetail.ShipTo.Phone;

            try
            {
                toAddress.Create();
            }
            catch (EasyPost.HttpException e)
            {
                FrayteGeneralError error = JsonConvert.DeserializeObject<FrayteGeneralError>(e.Message);
                frayteError.Address.Add(error.message);
                frayteError.Status = false;
            }
        }

        internal protected void setDirectBookingEasyPostFromAddress(EasyPostShipmentDetail shipmentDetail, FratyteError frayteError, EasyPost.Address fromAddress)
        {
            var shipFromCompany = "";
            if (string.IsNullOrEmpty(shipmentDetail.ShipFrom.CompanyName))
            {
                shipFromCompany = shipmentDetail.ShipFrom.FirstName + " " + shipmentDetail.ShipFrom.LastName;
                fromAddress.company = shipFromCompany;
            }
            else
            {
                fromAddress.company = shipmentDetail.ShipFrom.CompanyName;
            }
            fromAddress.name = shipmentDetail.ShipFrom.FirstName + " " + shipmentDetail.ShipFrom.LastName;
            fromAddress.street1 = shipmentDetail.ShipFrom.Address;
            fromAddress.street2 = shipmentDetail.ShipFrom.Address2;
            fromAddress.city = shipmentDetail.ShipFrom.City;
            fromAddress.state = shipmentDetail.ShipFrom.State;
            fromAddress.country = shipmentDetail.ShipFrom.Country.Code2;
            fromAddress.zip = shipmentDetail.ShipFrom.PostCode;
            fromAddress.phone = shipmentDetail.ShipFrom.Phone;

            try
            {
                fromAddress.Create();
            }
            catch (EasyPost.HttpException e)
            {
                FrayteGeneralError error = JsonConvert.DeserializeObject<FrayteGeneralError>(e.Message);
                frayteError.Address.Add(error.message);
                frayteError.Status = false;
            }
        }

        internal protected void setDirectBookingToOrder(EasyPostShipmentDetail shipmentDetail, Order dbOrder)
        {
            foreach (var dbPackage in shipmentDetail.Packages)
            {
                if (dbPackage.CartoonValue > 0)
                {
                    for (int i = 0; i < dbPackage.CartoonValue; i++)
                    {
                        var weight = 0M;
                        var width = 0M;
                        var height = 0M;
                        var length = 0M;
                        if (shipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms)
                        {
                            weight = dbPackage.Weight * 35.274M;
                            width = dbPackage.Width * 0.393701M;
                            height = dbPackage.Height * 0.393701M;
                            length = dbPackage.Length * 0.393701M;
                        }
                        else if (shipmentDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs)
                        {
                            weight = dbPackage.Weight * 16;
                            width = dbPackage.Width;
                            height = dbPackage.Height;
                            length = dbPackage.Length;
                        }
                        else
                        {
                            weight = dbPackage.Weight;
                            width = dbPackage.Width;
                            height = dbPackage.Height;
                            length = dbPackage.Length;
                        }

                        //Step 4. Create Parcel            
                        EasyPost.Parcel parcel = EasyPost.Parcel.Create(new Dictionary<string, object>()
                        {
                              {"length", length},
                              {"width", width},
                              {"height", height},
                              {"weight", weight}
                        });

                        string parcelId = parcel.id;

                        EasyPost.Shipment shipment = new EasyPost.Shipment();

                        // Check that originating and destinating country are different For DirectBooking
                        EasyPost.CustomsInfo customsInfo = new EasyPost.CustomsInfo();
                        if (shipmentDetail.ShipFrom.Country.CountryId != shipmentDetail.ShipTo.Country.CountryId)
                        {
                            customsInfo = CreateDirectBookingCustomInformation(shipmentDetail);
                            dbOrder.customs_info = customsInfo;
                            CreateCustomItem(shipmentDetail, dbOrder.customs_info, dbPackage);
                        }

                        //Step 5. Create Shipment                             
                        shipment.from_address = dbOrder.from_address;
                        shipment.to_address = dbOrder.to_address;
                        shipment.parcel = parcel;
                        shipment.customs_info = customsInfo;

                        if (shipmentDetail.PayTaxAndDuties == FrayteTermsOfTrade.Receiver)
                        {
                            shipment.options = new EasyPost.Options()
                            {
                                print_custom_1 = shipmentDetail.FrayteNumber + "-" + shipmentDetail.ReferenceDetail.Reference1,
                                delivered_duty_paid = false,
                                bill_receiver_account = shipmentDetail.EasyPostService.CourierAccountNumber,
                                declared_value = Convert.ToDouble(dbPackage.Value / dbPackage.CartoonValue)
                            };
                        }
                        else if (shipmentDetail.PayTaxAndDuties == FrayteTermsOfTrade.Shipper)
                        {
                            shipment.options = new EasyPost.Options()
                            {
                                print_custom_1 = shipmentDetail.FrayteNumber + "-" + shipmentDetail.ReferenceDetail.Reference1,
                                delivered_duty_paid = true,
                                bill_receiver_account = shipmentDetail.EasyPostService.CourierAccountNumber,
                                declared_value = Convert.ToDouble(dbPackage.Value / dbPackage.CartoonValue)
                            };
                        }
                        else
                        {
                            shipment.options = new EasyPost.Options()
                            {
                                print_custom_1 = shipmentDetail.FrayteNumber + "-" + shipmentDetail.ReferenceDetail.Reference1,
                                delivered_duty_paid = false,
                                bill_third_party_account = shipmentDetail.PaymentPartyAccountNumber,
                                declared_value = Convert.ToDouble(dbPackage.Value / dbPackage.CartoonValue)
                            };
                        }
                        dbOrder.shipments.Add(shipment);
                    }
                }
            }
        }

        internal protected void createDirectBookingEasyPostOrder(EasyPostShipmentDetail shipmentDetail, Order dbOrder, FratyteError frayteError)
        {
            try
            {
                frayteError.Address = new List<string>();
                frayteError.Custom = new List<string>();
                frayteError.Package = new List<string>();
                frayteError.Miscellaneous = new List<string>();
                frayteError.Service = new List<string>();
                frayteError.ServiceError = new List<string>();

                dbOrder.Create();
                Dictionary<decimal, EasyPost.Rate> minEasyPostRate = new Dictionary<decimal, EasyPost.Rate>();
                EasyPost.Rate appliedRate = new EasyPost.Rate();

                // Need to set LogisticType in the Wrapper Class
                if (dbOrder.rates != null && dbOrder.rates.Count == 0 && dbOrder.messages.Count > 0)
                {
                    if (!string.IsNullOrEmpty(shipmentDetail.EasyPostService.Courier))
                    {
                        foreach (var err in dbOrder.messages)
                        {
                            FrayteGeneralError error = JsonConvert.DeserializeObject<FrayteGeneralError>(err);
                            if (error.carrier == shipmentDetail.EasyPostService.Courier + shipmentDetail.EasyPostService.RateType)
                            {
                                if (error.message.Contains("country") || error.message.Contains("postal code") || error.message.Contains("postcode"))
                                {
                                    frayteError.Address.Add(error.message);
                                }
                                else if (error.message.Contains("custom"))
                                {
                                    frayteError.Custom.Add(error.message);
                                }
                                else if (error.message.Contains("package"))
                                {
                                    frayteError.Package.Add(error.message);
                                }
                                else
                                {
                                    frayteError.IsMailSend = true;
                                    frayteError.Miscellaneous.Add(error.message);
                                }

                                SaveDirectShipmentObject(Newtonsoft.Json.JsonConvert.SerializeObject(dbOrder).ToString(), shipmentDetail.DraftShipmentId);
                                SaveEasyPostErrorObject(error.message, shipmentDetail.DraftShipmentId);
                            }
                        }
                    }
                    frayteError.Status = false;
                }
                else if (dbOrder.rates != null && dbOrder.rates.Count > 0)
                {
                    var dbrate = dbOrder.rates.Except(dbOrder.rates.Where(x => x.service.Contains("MedicalExpress") ||
                                                                               x.service.Contains("MedicalExpressDoc") ||
                                                                               x.service.Contains("MedicalExpressNonDoc") ||
                                                                               x.service.Contains("MedicalExpressDoc&Nondoc") ||
                                                                               x.service.Contains("MedicalExpressDocAndNondoc") ||
                                                                               x.service.Contains("MedicalExpressDocandNondoc")));

                    if (shipmentDetail.ShipFrom.Country.Code2 == shipmentDetail.ShipTo.Country.Code2)
                    {
                        int j = 0;
                        foreach (EasyPost.Rate rate in dbrate)
                        {
                            if (rate.service == FrayteEasyPostService.DomesticExpress)
                            {
                                if (!string.IsNullOrEmpty(rate.service) &&
                                    !string.IsNullOrEmpty(rate.carrier) && !string.IsNullOrEmpty(rate.carrier_account_id) &&
                                    rate.carrier == shipmentDetail.EasyPostService.Courier + "" + shipmentDetail.EasyPostService.RateType && rate.carrier_account_id == shipmentDetail.EasyPostService.CourierAccountId)
                                {
                                    if (!minEasyPostRate.ContainsKey(CommonConversion.ConvertToDecimal(rate.rate)))
                                    {
                                        minEasyPostRate.Add(CommonConversion.ConvertToDecimal(rate.rate), rate);
                                        j++;
                                    }
                                }
                            }
                        }
                        if (j == 0)
                        {
                            frayteError.IsMailSend = true;
                            frayteError.Miscellaneous.Add("DHL Domestic Express service is not available for this shipment. Please contact to DHL administration for further assistance.");
                        }
                    }
                    else
                    {
                        int i = 0;
                        foreach (EasyPost.Rate rate in dbrate)
                        {
                            if (rate.service == FrayteEasyPostService.ExpressWorlwide || rate.service == FrayteEasyPostService.ExpressWorldwideECX)
                            {
                                if (!string.IsNullOrEmpty(rate.service) &&
                                    !string.IsNullOrEmpty(rate.carrier) && !string.IsNullOrEmpty(rate.carrier_account_id) &&
                                    rate.carrier == shipmentDetail.EasyPostService.Courier + "" + shipmentDetail.EasyPostService.RateType && rate.carrier_account_id == shipmentDetail.EasyPostService.CourierAccountId)
                                {
                                    if (!minEasyPostRate.ContainsKey(CommonConversion.ConvertToDecimal(rate.rate)))
                                    {
                                        minEasyPostRate.Add(CommonConversion.ConvertToDecimal(rate.rate), rate);
                                        i++;
                                    }
                                }
                            }
                        }
                        if (i == 0)
                        {
                            frayteError.IsMailSend = true;
                            frayteError.Miscellaneous.Add("DHL Express Worldwide service is not available for this shipment. Please contact to DHL administration for further assistance.");
                        }
                    }
                    if (minEasyPostRate.Count > 0)
                    {
                        appliedRate = minEasyPostRate.OrderBy(p => p.Key).First().Value;
                        dbOrder.Buy(appliedRate.carrier, appliedRate.service);
                        frayteError.Status = true;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(shipmentDetail.EasyPostService.Courier))
                        {
                            foreach (var err in dbOrder.messages)
                            {
                                FrayteGeneralError error = JsonConvert.DeserializeObject<FrayteGeneralError>(err);
                                if (error.carrier == shipmentDetail.EasyPostService.Courier + shipmentDetail.EasyPostService.RateType)
                                {
                                    if (error.message.Contains("country") || error.message.Contains("postal code") || error.message.Contains("postcode"))
                                    {
                                        frayteError.Address.Add(error.message);
                                    }
                                    else if (error.message.Contains("custom"))
                                    {
                                        frayteError.Custom.Add(error.message);
                                    }
                                    else if (error.message.Contains("package"))
                                    {
                                        frayteError.Package.Add(error.message);
                                    }
                                    else
                                    {
                                        frayteError.IsMailSend = true;
                                        frayteError.Miscellaneous.Add(error.message);
                                    }
                                    SaveDirectShipmentObject(Newtonsoft.Json.JsonConvert.SerializeObject(dbOrder).ToString(), shipmentDetail.DraftShipmentId);
                                    SaveEasyPostErrorObject(error.message, shipmentDetail.DraftShipmentId);
                                }
                            }
                        }
                        frayteError.Status = false;
                    }
                }

                if (shipmentDetail.ReferenceDetail.CollectionDate.Value != null && shipmentDetail.ReferenceDetail.CollectionTime != null)
                {
                    EasyPost.Pickup pickup = new EasyPost.Pickup();
                    if (minEasyPostRate.Count > 0)
                    {
                        DateTime mindatetime = shipmentDetail.ReferenceDetail.CollectionDate.Value;
                        DateTime maxdatetime = shipmentDetail.ReferenceDetail.CollectionDate.Value.AddDays(1);

                        if (mindatetime.Date < DateTime.Now.Date)
                        {
                            mindatetime = mindatetime.AddDays(1);
                        }

                        if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(maxdatetime.DayOfWeek) == System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek))
                        {
                            maxdatetime = maxdatetime.AddDays(1);
                        }

                        if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Sunday)
                        {
                            mindatetime = mindatetime.AddDays(1);
                        }
                        else if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Saturday)
                        {
                            mindatetime = mindatetime.AddDays(2);
                        }


                        if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(maxdatetime.DayOfWeek) == FraytePickUpDay.Sunday)
                        {
                            maxdatetime = maxdatetime.AddDays(2);
                            if (mindatetime.Date == maxdatetime.Date)
                            {
                                maxdatetime = maxdatetime.AddDays(3);
                            }
                        }
                        else if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(maxdatetime.DayOfWeek) == FraytePickUpDay.Saturday)
                        {
                            maxdatetime = maxdatetime.AddDays(3);
                            if (mindatetime.Date == maxdatetime.Date)
                            {
                                maxdatetime = maxdatetime.AddDays(4);
                            }
                            else if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(maxdatetime.DayOfWeek) == FraytePickUpDay.Saturday)
                            {
                                maxdatetime = maxdatetime.AddDays(2);
                            }
                            else if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(maxdatetime.DayOfWeek) == FraytePickUpDay.Sunday)
                            {
                                maxdatetime = maxdatetime.AddDays(1);
                            }
                        }

                        var parameters = new Dictionary<string, object>()
                        {
                            { "address", dbOrder.from_address },
                            { "shipment", dbOrder.shipments.FirstOrDefault() },
                            { "is_account_address", false },
                            { "min_datetime", mindatetime.ToString("yyyy/MM/dd") + " " + CommonConversion.ConvertStringToTime(shipmentDetail.ReferenceDetail.CollectionTime) },
                            { "max_datetime", maxdatetime.ToString("yyyy/MM/dd") + " " + CommonConversion.ConvertStringToTime(shipmentDetail.ReferenceDetail.CollectionTime) }
                        };

                        pickup = EasyPost.Pickup.Create(parameters);
                        if (pickup != null)
                        {
                            pickup.Buy(shipmentDetail.EasyPostService.Courier + "" + shipmentDetail.EasyPostService.RateType, pickup.pickup_rates[0].service);
                        }
                        else
                        {
                            new DirectShipmentRepository().SaveEasyPosyPickUpObject(Newtonsoft.Json.JsonConvert.SerializeObject(pickup).ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(shipmentDetail).ToString() ,shipmentDetail.DraftShipmentId);
                        }
                    }
                }
            }
            catch (EasyPost.HttpException ex)
            {
                FrayteGeneralError error = JsonConvert.DeserializeObject<FrayteGeneralError>(ex.Message);
                if (!string.IsNullOrEmpty(error.message))
                {
                    frayteError.Miscellaneous.Add(error.message);
                    SaveDirectShipmentObject(Newtonsoft.Json.JsonConvert.SerializeObject(dbOrder).ToString(), shipmentDetail.DraftShipmentId);
                    SaveEasyPostErrorObject(error.message, shipmentDetail.DraftShipmentId);
                }
                else
                {
                    FrayteEasyPostError error1 = JsonConvert.DeserializeObject<FrayteEasyPostError>(ex.Message);
                    if (error1.error != null && !string.IsNullOrEmpty(error1.error.message))
                    {
                        frayteError.Miscellaneous.Add(error1.error.message);
                    }
                    else
                    {
                        frayteError.Miscellaneous.Add("Error While Buying the Shipment");
                    }
                    SaveDirectShipmentObject(Newtonsoft.Json.JsonConvert.SerializeObject(dbOrder).ToString(), shipmentDetail.DraftShipmentId);
                    SaveEasyPostErrorObject(error1.error.message, shipmentDetail.DraftShipmentId);
                }

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                frayteError.Status = false;
            }
        }

        #endregion

        public IntegrtaionResult MapDHLIntegrationResponse(List<FraytePackageTrackingDetail> PackageResults, string ShipmentIdentificationNumber)
        {
            IntegrtaionResult integrtaionResult = new IntegrtaionResult();
            integrtaionResult.Status = true;
            integrtaionResult.CourierName = FrayteCourierCompany.DHL;
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

            return integrtaionResult;
        }
    }
}
