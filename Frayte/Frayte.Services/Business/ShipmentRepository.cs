using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Xml;
using System.Web;
using Frayte.Services.Utility;
using System.Data.Entity.Validation;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;

namespace Frayte.Services.Business
{
    public class ShipmentRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        #region -- Public Methods --

        public void UpdateCargoWiseShipmentId(string CargoWiseId, int Shipmentid)
        {
            Shipment FrayteShipment = dbContext.Shipments.Where(p => p.ShipmentId == Shipmentid).FirstOrDefault();
            FrayteShipment.CargoWiseSo = CargoWiseId;
            dbContext.SaveChanges();
        }

        public void SaveEasyPostRate(int shipmentId, EasyPost.Rate shipmentRate)
        {
            ShipmentEasyPostRate EasyPostRate = new ShipmentEasyPostRate();
            EasyPost.Shipment shipment = new EasyPost.Shipment();
            EasyPostRate.ShipmentId = shipmentId;
            EasyPostRate.EasyPostShipmentId = shipmentRate.shipment_id;
            EasyPostRate.Carrier = shipmentRate.carrier;
            EasyPostRate.CarrierAccountId = shipmentRate.carrier_account_id;
            EasyPostRate.Currency = shipmentRate.currency;
            EasyPostRate.CreatedAt = shipmentRate.created_at.Value;
            EasyPostRate.DeliveryDate = shipmentRate.delivery_date;
            EasyPostRate.DeliveryDateGuaranteed = shipmentRate.delivery_date_guaranteed;
            EasyPostRate.DeliveryDays = shipmentRate.delivery_days;
            EasyPostRate.EstDeliveryDays = shipmentRate.est_delivery_days;
            EasyPostRate.EasyPostRateId = shipmentRate.id;
            EasyPostRate.ListCurrency = shipmentRate.list_currency;
            EasyPostRate.ListRate = shipmentRate.list_rate;
            EasyPostRate.Mode = shipmentRate.mode;
            EasyPostRate.Rate = shipmentRate.rate;
            EasyPostRate.RetailCurrency = shipmentRate.retail_currency;
            EasyPostRate.RetailRate = shipmentRate.retail_rate;
            EasyPostRate.Service = shipmentRate.service;
            dbContext.ShipmentEasyPostRates.Add(EasyPostRate);
            dbContext.SaveChanges();
        }

        // sea initial 
        public List<ShipmentType> GetShipmentTypes()
        {
            var list = dbContext.ShipmentTypes.ToList();
            return list;
        }

        public FrayteShipmentTelex GetSeaInitial(int shipmentId)
        {
            FrayteShipmentTelex initial = new FrayteShipmentTelex();
            var shipment = dbContext.Shipments.Find(shipmentId);

            initial.ShipmentId = shipment.ShipmentId;
            initial.FlightVessel = shipment.FlightVessel;
            initial.MABBL = shipment.MABBL;
            return initial;
        }

        public int GetTradelaneId(int shipmentId)
        {
            int id = dbContext.Shipments.Find(shipmentId).TradeLaneId;
            return id;
        }

        public ShipmentTracking GetTracking(string CarrierName, string TrackingNumber)
        {
            var eCommerceTrackingDetail = (from a in dbContext.eCommerceTrackings
                                           where a.TrackingNumber == TrackingNumber
                                           select new ManifestTrackingModel()
                                           {
                                               eCommerceShipmentId = a.eCommerceShipmentId,
                                               TrackingDescription = a.TrackingDescription,
                                               TrackingMode = a.TrackingMode,
                                               TrackingNo = a.TrackingNumber,
                                               CreatedBy = a.CreatedBy,
                                               CreatedOnUtc = a.CreatedOnUtc,
                                               TrackingDescriptionCode = a.TrackingDescriptionCode,
                                               FrayteNo = a.FrayteNumber
                                           }).FirstOrDefault();
            var res = Convert.ToDouble(dbContext.eCommerceShipmentDetails.Where(a => a.eCommerceShipmentId == eCommerceTrackingDetail.eCommerceShipmentId).ToList().Select(a => a.Weight).Sum());
            var eCommerceShipmentDetail = (from eComm in dbContext.eCommerceShipments
                                           join eCommAddress in dbContext.eCommerceShipmentAddresses
                                                on eComm.ToAddressId equals eCommAddress.eCommerceShipmentAddressId
                                           join CL in dbContext.CountryLogistics on eCommAddress.CountryId equals CL.CountryId
                                           where eComm.eCommerceShipmentId == eCommerceTrackingDetail.eCommerceShipmentId
                                           select new ShipmentTracking()
                                           {
                                               DirectShipmentId = eComm.eCommerceShipmentId,
                                               TrackingNumber = eCommerceTrackingDetail.TrackingNo,
                                               CreatedAtDate = eComm.CreatedOn.ToString(),
                                               CreatedAtTime = eComm.CreatedOn.ToString(),
                                               UpdatedAtDate = eComm.UpdatedOn.ToString(),
                                               UpdatedAtTime = eComm.UpdatedOn.Value.ToString(),
                                               EstimatedDeliveryDate = eComm.EstimatedDateofDelivery.ToString(),
                                               EstimatedDeliveryTime = eComm.EstimatedTimeofDelivery.ToString(),
                                               NoOfPieces = dbContext.eCommerceShipmentDetails.Where(a => a.eCommerceShipmentId == eCommerceTrackingDetail.eCommerceShipmentId).ToList().Count,
                                               EstimatedWeight = res,
                                               Status = eCommerceTrackingDetail.TrackingDescription,
                                               TrackingDetails = new List<ShipmentTrackingDetail>()
                                               {
                                                    new ShipmentTrackingDetail()
                                                   {
                                                      IsCollapsed = true,
                                                      Activity = eCommerceTrackingDetail.TrackingDescription,
                                                      Pieces = new List<string>() { eCommerceTrackingDetail.TrackingNo },
                                                      Date = eCommerceTrackingDetail.CreatedOnUtc,
                                                      Time = eCommerceTrackingDetail.CreatedOnUtc.ToString()
                                                   }

                                               },
                                               TrackingPicesDetail = new FrayteServiceProviderDetail()
                                               {
                                                   provider = new FrayteProviderDetail()
                                                   {
                                                       name = CL.LogisticService
                                                   },
                                                   service = new FrayteServiceDetail()
                                                   {
                                                       name = CL.LogisticService,
                                                       description = CL.LogisicServiceDisplay

                                                   },
                                                   trackingReferences = new List<string>()
                                                   {
                                                       eCommerceTrackingDetail.TrackingNo
                                                   }
                                               },
                                               Carrier = CL.LogisticService

                                           }).FirstOrDefault();
            eCommerceShipmentDetail.CreatedAtTime = Convert.ToDateTime(eCommerceShipmentDetail.CreatedAtDate).TimeOfDay.ToString();
            eCommerceShipmentDetail.UpdatedAtTime = Convert.ToDateTime(eCommerceShipmentDetail.UpdatedAtDate).TimeOfDay.ToString();
            eCommerceShipmentDetail.CreatedAtDate = Convert.ToDateTime(eCommerceShipmentDetail.CreatedAtDate).ToString();
            eCommerceShipmentDetail.UpdatedAtDate = Convert.ToDateTime(eCommerceShipmentDetail.UpdatedAtDate).ToString();
            eCommerceShipmentDetail.TrackingDetails.First().Time = Convert.ToDateTime(eCommerceShipmentDetail.TrackingDetails.First().Time).ToShortTimeString();



            return eCommerceShipmentDetail;
        }

        public FrayteShipment GetShipmentDetail(int shipmentId)
        {
            FrayteShipment frayteShipment = new FrayteShipment();
            frayteShipment.ShipmentId = shipmentId;

            //Step 1 : Get Shipment information
            GetShipment(frayteShipment);

            string courierType = new ShipmentRepository().GetShipmentCourierType(frayteShipment.ShipmentId);
            //Step 1.1 : Get CustomInformation
            if (courierType == FrayteShipmentType.Courier)
            {
                GetCustomInformation(frayteShipment);
            }

            //Step 2: Get Shipment detail
            GetShipmentDetail(frayteShipment);

            //Step 3: Get Shipper detail
            GetShipperDetail(frayteShipment);

            //Step 4: Get Shipper address detail
            GetShipperAddressDetail(frayteShipment);

            //Step 5: Get Receiver detail
            GetReceiverDetail(frayteShipment);

            //Step 6: Get Receiver address detail
            GetReceiverAddressDetail(frayteShipment);

            //Step 7: Get pickup address detail
            GetPickupAddressDetail(frayteShipment);

            //Step 8: Get courier detail
            GetCourierDetailForShipment(frayteShipment);

            //Step 9: Get Warehouse and TransportToWarehouse detail
            GetWarehouseAndTransportToWarehouse(frayteShipment);

            //Step 9: Get shipment time zone detail
            GetShipmentTimezoneDetail(frayteShipment);

            //Step 10 : Get CountryPorts 
            GetCountryPorts(frayteShipment);

            //Step 11 : GetTradelane


            return frayteShipment;
        }

        private void GetCountryPorts(FrayteShipment frayteshipment)
        {
            if (frayteshipment != null && frayteshipment.ShipperAddress != null && frayteshipment.ShipperAddress.Country != null && frayteshipment.ShipperAddress.Country.CountryId > 0)
            {
                var portsofdepartures = dbContext.CountryShipmentPorts.Where(p => p.CountryId == frayteshipment.ShipperAddress.Country.CountryId).FirstOrDefault();
                if (portsofdepartures != null)
                {
                    frayteshipment.FrayteShipmentPortOfDeparture = portsofdepartures;
                }
            }
            if (frayteshipment != null && frayteshipment.ReceiverAddress != null && frayteshipment.ReceiverAddress.Country != null && frayteshipment.ReceiverAddress.Country.CountryId > 0)
            {
                var portsofarrival = dbContext.CountryShipmentPorts.Where(p => p.CountryId == frayteshipment.ReceiverAddress.Country.CountryId).FirstOrDefault();
                if (portsofarrival != null)
                {
                    frayteshipment.FrayteShipmentPortOfArrival = portsofarrival;
                }
            }
        }

        private void GetCustomInformation(FrayteShipment frayteShipment)
        {
            ShipmentEasyPost CustomInfo = dbContext.ShipmentEasyPosts.Where(p => p.ShipmentId == frayteShipment.ShipmentId).FirstOrDefault();
            frayteShipment.CustomInfo = new CustomInformation();
            if (CustomInfo != null)
            {
                //frayteShipment.CustomInfo.ShipmentEasyPostId = CustomInfo.ShipmentEasyPostId;
                frayteShipment.CustomInfo.ShipmentId = CustomInfo.ShipmentId;
                frayteShipment.CustomInfo.ContentsType = CustomInfo.ContentsType;
                frayteShipment.CustomInfo.ContentsExplanation = CustomInfo.ContentsExplanation;
                frayteShipment.CustomInfo.RestrictionType = CustomInfo.RestrictionType;
                frayteShipment.CustomInfo.RestrictionComments = CustomInfo.RestrictionComments;
                frayteShipment.CustomInfo.CustomsCertify = CustomInfo.CustomsCertify;
                frayteShipment.CustomInfo.CustomsSigner = CustomInfo.CustomsSigner;
                frayteShipment.CustomInfo.NonDeliveryOption = CustomInfo.NonDeliveryOption;
                frayteShipment.CustomInfo.EelPfc = CustomInfo.EelPfc;

            }
        }

        public FrayteShipmentShipperReceiver GetShipmentShipperReceiverDetail(int shipmentId)
        {
            FrayteShipmentShipperReceiver shipmentDetail = new FrayteShipmentShipperReceiver();


            var frayteShipmentShipperReceiver = dbContext.spGet_ShipmentShipperReceiverDetail(shipmentId).FirstOrDefault();
            if (frayteShipmentShipperReceiver != null)
            {
                shipmentDetail.ShipmentId = frayteShipmentShipperReceiver.ShipmentId;
                shipmentDetail.FrayteCargoWiseSo = frayteShipmentShipperReceiver.CargoWiseSo;
                shipmentDetail.PurchaseOrderNo = frayteShipmentShipperReceiver.PurchaseOrderNumber;
                shipmentDetail.Shipper = new FrayteUser();
                shipmentDetail.ShipperAddress = new FrayteAddress();
                shipmentDetail.Shipper.ContactName = frayteShipmentShipperReceiver.ShipperContactName;
                shipmentDetail.Shipper.CompanyName = frayteShipmentShipperReceiver.ShipperCompanyName;
                shipmentDetail.Shipper.TelephoneNo = frayteShipmentShipperReceiver.ShipperTelephoneNo;
                shipmentDetail.Shipper.MobileNo = frayteShipmentShipperReceiver.ShipperMobileNo;
                shipmentDetail.Shipper.Email = frayteShipmentShipperReceiver.ShipperEmail;
                shipmentDetail.ShipperAddress.Address = frayteShipmentShipperReceiver.ShipperAddress;
                shipmentDetail.ShipperAddress.Address2 = frayteShipmentShipperReceiver.ShipperAddress2;
                shipmentDetail.ShipperAddress.Address3 = frayteShipmentShipperReceiver.ShipperAddress3;
                shipmentDetail.ShipperAddress.City = frayteShipmentShipperReceiver.ShipperCity;
                shipmentDetail.ShipperAddress.Zip = frayteShipmentShipperReceiver.ShipperZip;
                shipmentDetail.ShipperAddress.State = frayteShipmentShipperReceiver.ShipperState;
                shipmentDetail.ShipperAddress.Country = new FrayteCountryCode();
                shipmentDetail.ShipperAddress.Country.Name = frayteShipmentShipperReceiver.ShipperCountryName;
                shipmentDetail.Receiver = new FrayteUser();
                shipmentDetail.ReceiverAddress = new FrayteAddress();
                shipmentDetail.Receiver.ContactName = frayteShipmentShipperReceiver.ReceiverContactName;
                shipmentDetail.Receiver.CompanyName = frayteShipmentShipperReceiver.ReceiverCompanyName;
                shipmentDetail.Receiver.TelephoneNo = frayteShipmentShipperReceiver.ReceiverTelephoneNo;
                shipmentDetail.Receiver.MobileNo = frayteShipmentShipperReceiver.ReceiverMobileNo;
                shipmentDetail.Receiver.Email = frayteShipmentShipperReceiver.ReceiverEmail;
                shipmentDetail.ReceiverAddress.Address = frayteShipmentShipperReceiver.ReceiverAddress;
                shipmentDetail.ReceiverAddress.Address2 = frayteShipmentShipperReceiver.ReceiverAddress2;
                shipmentDetail.ReceiverAddress.Address3 = frayteShipmentShipperReceiver.ReceiverAddress3;
                shipmentDetail.ReceiverAddress.City = frayteShipmentShipperReceiver.ReceiverCity;
                shipmentDetail.ReceiverAddress.Zip = frayteShipmentShipperReceiver.ReceiverZip;
                shipmentDetail.ReceiverAddress.State = frayteShipmentShipperReceiver.ReceiverState;
                shipmentDetail.ReceiverAddress.Country = new FrayteCountryCode();
                shipmentDetail.ReceiverAddress.Country.Name = frayteShipmentShipperReceiver.ReceiverCountryName;
            }

            return shipmentDetail;
        }

        public FrayteResult SaveAmmendShipment(FrayteCustomerAmendShipment frayteAmendShipment)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                FrayteUserRepository userRepository = new FrayteUserRepository();

                //Step 1: Save Receiver Detail
                userRepository.SaveUserDetail(frayteAmendShipment.Receiver);

                //Step 2: Save Receiver Address information
                userRepository.SaveUserAddress(frayteAmendShipment.ReceiverAddress);

                //Step 3: Save Shipment 
                SaveShipment(frayteAmendShipment);

                // Step 4: Save Shipment Detail
                if (frayteAmendShipment.DeliveredBy.CourierType == FrayteShipmentType.Courier)
                {
                    SaveShipmentDetailNew(frayteAmendShipment);
                }

                result.Status = true;

            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Errors = new List<string>();
                result.Errors.Add(ex.Message);
            }
            return result;
        }

        private void SaveShipmentDetailNew(FrayteCustomerAmendShipment frayteAmendShipment)
        {
            if (frayteAmendShipment != null && frayteAmendShipment.ShipmentId > 0)
            {
                var shipmentDetail = dbContext.ShipmentDetails.Where(p => p.ShipmentId == frayteAmendShipment.ShipmentId).ToList();
                if (shipmentDetail != null && shipmentDetail.Count > 1)
                {
                    int i = 0;
                    foreach (var shipementD in shipmentDetail)
                    {
                        if (i > 0)
                        {
                            dbContext.ShipmentDetails.Remove(shipementD);
                        }
                        i++;
                    }
                    dbContext.SaveChanges();
                }
            }
        }

        public FrayteResult SaveShipment(FrayteShipment shipment)
        {
            FrayteResult result = new FrayteResult();

            try
            {
                FrayteUserRepository userRepository = new FrayteUserRepository();

                if (!shipment.IsLogin)
                {
                    //Step 1.1: Save Shipper Detail
                    userRepository.SaveUserDetail(shipment.Shipper);

                    //Step 1.2: Save Shipper role
                    userRepository.SaveUserRole(shipment.Shipper.UserId, (int)FrayteUserRole.Shipper);

                    //Step 1.3: Save shipper Address information
                    shipment.ShipperAddress.AddressTypeId = 1;
                    shipment.ShipperAddress.UserId = shipment.Shipper.UserId;
                    userRepository.SaveUserAddress(shipment.ShipperAddress);

                    //Step 1.4: Save Shipper Pickup address information
                    if (shipment.OtherPickupAddress)
                    {
                        shipment.PickupAddress.AddressTypeId = 2;
                        shipment.PickupAddress.UserId = shipment.Shipper.UserId;
                        userRepository.SaveUserAddress(shipment.PickupAddress);
                    }
                    //Step 2.1: Save Receiver Detail
                    userRepository.SaveUserDetail(shipment.Receiver);

                    //Step 2.2: Save Receiver role
                    userRepository.SaveUserRole(shipment.Receiver.UserId, (int)FrayteUserRole.Receiver);

                    //Step 2.3: Save Receiver Address information
                    shipment.ReceiverAddress.AddressTypeId = (int)FrayteAddressType.MainAddress;
                    shipment.ReceiverAddress.UserId = shipment.Receiver.UserId;
                    userRepository.SaveUserAddress(shipment.ReceiverAddress);
                }

                //Step 8: Save Shipment 
                SaveShipmentInfo(shipment);

                // Save Custominformation if Shipment type is "Courier"
                if (shipment.DeliveredBy.CourierType == "Courier")
                {
                    // shipment.CustomInfo.ShipmentId = shipment.ShipmentId;
                    SaveCustomInformation(shipment.ShipmentId, shipment.CustomInfo);
                }

                //Step 9: Save ShipmentDetail
                SaveShipmentDetail(shipment.ShipmentId, shipment.ShipmentDetails);

                result.Status = true;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Errors = new List<string>();
                result.Errors.Add(ex.Message);
            }
            return result;
        }

        private void SaveCustomInformation(int ShipmentId, CustomInformation CustomInfomation)
        {
            var ShipmentPost = dbContext.ShipmentEasyPosts.Where(p => p.ShipmentId == ShipmentId).FirstOrDefault();
            ShipmentEasyPost FrayteCustom;
            if (ShipmentPost != null && CustomInfomation == null)
            {
                dbContext.ShipmentEasyPosts.Remove(ShipmentPost);
                dbContext.SaveChanges();
            }
            if (CustomInfomation != null && CustomInfomation.ShipmentCustomDetailId == 0)
            {
                FrayteCustom = new ShipmentEasyPost();

                // If Shipment is updated from non custom to custom
                if (ShipmentPost != null)
                {
                    ShipmentPost.ShipmentId = ShipmentId;
                    ShipmentPost.ContentsType = CustomInfomation.ContentsType;
                    ShipmentPost.ContentsExplanation = CustomInfomation.ContentsExplanation;
                    ShipmentPost.RestrictionType = CustomInfomation.RestrictionType;
                    ShipmentPost.RestrictionComments = CustomInfomation.RestrictionComments;
                    ShipmentPost.CustomsCertify = CustomInfomation.CustomsCertify;
                    ShipmentPost.CustomsSigner = CustomInfomation.CustomsSigner;
                    ShipmentPost.NonDeliveryOption = CustomInfomation.NonDeliveryOption;
                    ShipmentPost.EelPfc = CustomInfomation.EelPfc;
                    dbContext.SaveChanges();
                }
                else
                {
                    FrayteCustom.ShipmentId = ShipmentId;
                    FrayteCustom.ContentsType = CustomInfomation.ContentsType;
                    FrayteCustom.ContentsExplanation = CustomInfomation.ContentsExplanation;
                    FrayteCustom.RestrictionType = CustomInfomation.RestrictionType;
                    FrayteCustom.RestrictionComments = CustomInfomation.RestrictionComments;
                    FrayteCustom.CustomsCertify = CustomInfomation.CustomsCertify;
                    FrayteCustom.CustomsSigner = CustomInfomation.CustomsSigner;
                    FrayteCustom.NonDeliveryOption = CustomInfomation.NonDeliveryOption;
                    FrayteCustom.EelPfc = CustomInfomation.EelPfc;
                    dbContext.ShipmentEasyPosts.Add(FrayteCustom);
                    dbContext.SaveChanges();
                }


            }
            else if (CustomInfomation != null && CustomInfomation.ShipmentCustomDetailId > 0)
            {
                FrayteCustom = dbContext.ShipmentEasyPosts.Find(CustomInfomation.ShipmentCustomDetailId);
                if (FrayteCustom != null)
                {
                    FrayteCustom.ShipmentId = ShipmentId;
                    FrayteCustom.ContentsType = CustomInfomation.ContentsType;
                    FrayteCustom.ContentsExplanation = CustomInfomation.ContentsExplanation;
                    FrayteCustom.RestrictionType = CustomInfomation.RestrictionType;
                    FrayteCustom.RestrictionComments = CustomInfomation.RestrictionComments;
                    FrayteCustom.CustomsCertify = CustomInfomation.CustomsCertify;
                    FrayteCustom.CustomsSigner = CustomInfomation.CustomsSigner;
                    FrayteCustom.NonDeliveryOption = CustomInfomation.NonDeliveryOption;
                    FrayteCustom.EelPfc = CustomInfomation.EelPfc;
                    dbContext.SaveChanges();
                }
            }

        }

        public int CustomerAction(FrayteCustomerActionShippment confirmationDetail)
        {
            var shipment = dbContext.Shipments.Where(p => p.CustomerConfirmCode.ToString() == confirmationDetail.ConfirmationCode).FirstOrDefault();

            if (shipment != null)
            {
                //Update the shipment status Confirm/Rejected
                int customerId = shipment.CustomerId.HasValue ? shipment.CustomerId.Value : 0;
                if (confirmationDetail.ActionType == ShipmentCustomerAction.Reject)
                {
                    shipment.ShipmentStatusId = (int)FrayteShipmentStatus.CustomerReject;
                    dbContext.SaveChanges();
                    SaveShipmentStatus(shipment.ShipmentId, (int)FrayteShipmentStatus.CustomerReject, customerId);
                }
                else if (confirmationDetail.ActionType == ShipmentCustomerAction.Confirm)
                {
                    shipment.ShipmentStatusId = (int)FrayteShipmentStatus.CustomerConfirm;
                    dbContext.SaveChanges();
                    SaveShipmentStatus(shipment.ShipmentId, (int)FrayteShipmentStatus.CustomerConfirm, customerId);
                }

                return shipment.ShipmentId;
            }

            return 0;
        }

        public List<FrayteUserShipment> GetCurrentShipment(int roleId)
        {
            List<FrayteUserShipment> lstUserShipment = new List<FrayteUserShipment>();
            if (roleId == (int)FrayteUserRole.Admin)
            {
                //Show all current shipment
                var result = (from s in dbContext.Shipments
                              join c in dbContext.Users on s.CustomerId equals c.UserId
                              join sa in dbContext.UserAddresses on s.ShipperAddressId equals sa.UserAddressId
                              join sac in dbContext.Countries on sa.CountryId equals sac.CountryId
                              join ra in dbContext.UserAddresses on s.ReceiverAddressId equals ra.UserAddressId
                              join rac in dbContext.Countries on ra.CountryId equals rac.CountryId
                              join cr in dbContext.Couriers on s.DeliveredBy equals cr.CourierId
                              join ss in dbContext.ShipmentStatus on s.ShipmentStatusId equals ss.ShipmentStatusId
                              where s.ShipmentStatusId != (int)FrayteShipmentStatus.Close
                              //To Do: Need to put closed status here in where condition

                              select new FrayteUserShipment
                              {
                                  ShipmentId = s.ShipmentId,
                                  ShipmentCode = "",
                                  CargoWiseSo = s.CargoWiseSo,
                                  Customer = c.ContactName,
                                  ShippedFrom = sac.CountryName,
                                  ShippedTo = rac.CountryName,
                                  ShippingType = cr.ShipmentType,
                                  ShippingDate = s.ShippingDate,
                                  DateOfDelivery = s.FinalDeliveryDate,
                                  Status = ss.StatusName
                              }).ToList();

                lstUserShipment = result;
            }

            return lstUserShipment;
        }

        public List<FrayteUserShipment> GetPastShipment(int roleId)
        {
            List<FrayteUserShipment> lstUserShipment = new List<FrayteUserShipment>();

            //Show all current shipment
            var result = (from s in dbContext.Shipments
                          join c in dbContext.Users on s.CustomerId equals c.UserId
                          join sa in dbContext.UserAddresses on s.ShipperAddressId equals sa.UserAddressId
                          join sac in dbContext.Countries on sa.CountryId equals sac.CountryId
                          join ra in dbContext.UserAddresses on s.ReceiverAddressId equals ra.UserAddressId
                          join rac in dbContext.Countries on ra.CountryId equals rac.CountryId
                          join cr in dbContext.Couriers on s.DeliveredBy equals cr.CourierId
                          join ss in dbContext.ShipmentStatus on s.ShipmentStatusId equals ss.ShipmentStatusId
                          where s.ShipmentStatusId == (int)FrayteShipmentStatus.Close
                          //To Do: Need to put closed status here in where condition

                          select new FrayteUserShipment
                          {
                              ShipmentId = s.ShipmentId,
                              ShipmentCode = "",
                              CargoWiseSo = s.CargoWiseSo,
                              Customer = c.ContactName,
                              ShippedFrom = sac.CountryName,
                              ShippedTo = rac.CountryName,
                              ShippingType = cr.ShipmentType,
                              DateOfDelivery = s.FinalDeliveryDate,
                              Status = ss.StatusName
                          }).ToList();


            return result;
        }

        public string GetShipmentCourierType(int shipmentId)
        {
            //select s.deliveredby, c.CourierName, c.CourierType, s.* from Shipment s
            //inner join Courier c on s.DeliveredBy = c.CourierId

            var result = (from s in dbContext.Shipments
                          join c in dbContext.Couriers on s.DeliveredBy equals c.CourierId
                          where s.ShipmentId == shipmentId
                          select c.ShipmentType).FirstOrDefault();
            if (result != null)
            {
                return result;
            }
            else
            {
                return "";
            }

        }

        public bool IsOutstandingDocumentsUploaded(int shipmentId)
        {
            var shipment = dbContext.Shipments.Where(p => p.ShipmentId == shipmentId).FirstOrDefault();

            if (shipment != null)
            {
                if (!string.IsNullOrEmpty(shipment.CommercialInvoice))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public bool IsMAWBUploaded(int shipmentId)
        {
            var shipment = dbContext.Shipments.Where(p => p.ShipmentId == shipmentId).FirstOrDefault();

            if (shipment != null)
            {
                if (!string.IsNullOrEmpty(shipment.AWB))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public List<FrayteShipmentDocument> GetShipmentDocuments(int shipmentId, string shipmentDocumentPath)
        {
            List<FrayteShipmentDocument> shipmentDocuments = new List<FrayteShipmentDocument>();

            var shipment = dbContext.Shipments.Where(p => p.ShipmentId == shipmentId).FirstOrDefault();

            if (shipment != null)
            {
                if (!string.IsNullOrEmpty(shipment.CommercialInvoice))
                {
                    FrayteShipmentDocument document = new FrayteShipmentDocument();
                    document.ShipmentId = shipmentId;
                    document.DocumentType = "Commercial Invoice";
                    document.DocumentTitle = shipment.CommercialInvoice;
                    document.DocumentPath = shipmentDocumentPath + shipment.CommercialInvoice;
                    shipmentDocuments.Add(document);
                }

                if (!string.IsNullOrEmpty(shipment.PackingList))
                {
                    FrayteShipmentDocument document = new FrayteShipmentDocument();
                    document.ShipmentId = shipmentId;
                    document.DocumentType = "Packing List";
                    document.DocumentTitle = shipment.PackingList;
                    document.DocumentPath = shipmentDocumentPath + shipment.PackingList;
                    shipmentDocuments.Add(document);
                }

                if (!string.IsNullOrEmpty(shipment.CustomDocument))
                {
                    FrayteShipmentDocument document = new FrayteShipmentDocument();
                    document.ShipmentId = shipmentId;
                    document.DocumentType = "Custom Document";
                    document.DocumentTitle = shipment.CustomDocument;
                    document.DocumentPath = shipmentDocumentPath + shipment.CustomDocument;
                    shipmentDocuments.Add(document);
                }

                if (!string.IsNullOrEmpty(shipment.FinalImage))
                {
                    FrayteShipmentDocument document = new FrayteShipmentDocument();
                    document.ShipmentId = shipmentId;
                    document.DocumentType = "POD";
                    document.DocumentTitle = shipment.FinalImage;
                    document.DocumentPath = shipmentDocumentPath + shipment.FinalImage;
                    shipmentDocuments.Add(document);
                }
            }

            return shipmentDocuments;
        }

        public void SaveShipmentDocument(int shipmentId, string shipmentDocumentType, Shipment shipment)
        {
            var shipmentDB = dbContext.Shipments.Where(p => p.ShipmentId == shipmentId).FirstOrDefault();

            if (shipmentDB != null)
            {
                switch (shipmentDocumentType)
                {
                    case "CommercialInvoice":
                        shipmentDB.CommercialInvoice = shipment.CommercialInvoice;
                        shipmentDB.PackingList = shipment.PackingList;
                        shipmentDB.CustomDocument = shipment.CustomDocument;
                        break;
                    case "AirWayBill":
                        shipmentDB.AirWayBill = shipment.AirWayBill;
                        shipmentDB.AirWayBillDocument = shipment.AirWayBillDocument;
                        break;

                    case "TelexDocument":
                        shipmentDB.SeaTelexDocument = shipment.SeaTelexDocument;
                        shipmentDB.TelexReleaseBy = shipment.TelexReleaseBy;
                        shipmentDB.SeaPOD = shipment.SeaPOD;
                        shipmentDB.SeaPOL = shipment.SeaPOL;
                        break;
                }

                dbContext.SaveChanges();
            }

            //ShipmentDocument shipmentDocument = new ShipmentDocument();
            //shipmentDocument.ShipmentId = shipmentId;
            //shipmentDocument.DocumentType = shipmentDocumentType;
            //shipmentDocument.DocumentName = CommercialInvoiceFileName;

            //dbContext.ShipmentDocuments.Add(shipmentDocument);
            //dbContext.SaveChanges();
        }

        public FrayteResult UpdateDropOffDetail(FrayteShipmentDropOff dropOffDetail)
        {
            FrayteResult result = new FrayteResult();

            var shipment = dbContext.Shipments.Where(p => p.ShipmentId == dropOffDetail.ShipmentId).FirstOrDefault();

            if (shipment != null)
            {
                shipment.WarehouseDropOffDate = dropOffDetail.DropOffDate;
                shipment.WarehouseDropOffTime = UtilityRepository.GetTimeFromString(dropOffDetail.DropOffTime);
                dbContext.SaveChanges();

                result.Status = true;
                return result;
            }
            else
            {
                result.Status = false;
                return result;
            }
        }

        public FrayteResult UpdateOriginatingAgentDetail(FrayteShipmentOriginatingAgentDetails originatingAgentDetail)
        {
            FrayteResult result = new FrayteResult();

            var shipment = dbContext.Shipments.Where(p => p.ShipmentId == originatingAgentDetail.ShipmentId).FirstOrDefault();

            if (shipment != null)
            {
                //Step 1: Save Address information
                originatingAgentDetail.DeliveryAddress.AddressTypeId = (int)FrayteAddressType.OtherAddress;
                originatingAgentDetail.DeliveryAddress.UserId = shipment.OriginatingAgentId.HasValue ? shipment.OriginatingAgentId.Value : 0;
                if (originatingAgentDetail.DeliveryAddress.UserAddressId == 0)
                {
                    new FrayteUserRepository().SaveUserAddress(originatingAgentDetail.DeliveryAddress);

                }
                //Step 2: Update Shipment detail
                shipment.ShipmentStatusId = (int)FrayteShipmentStatus.AgentConfirm;
                shipment.OriginatingAddressId = originatingAgentDetail.DeliveryAddress.UserAddressId;
                //
                shipment.OriginatingPlannedDepartureDate = originatingAgentDetail.OriginatingPlannedDepartureDate;
                shipment.OriginatingPlannedDepartureTime = UtilityRepository.GetTimeFromString(originatingAgentDetail.OriginatingPlannedDepartureTime);
                shipment.OriginatingPlannedArrivalDate = originatingAgentDetail.OriginatingPlannedArrivalDate;
                shipment.OriginatingPlannedArrivalTime = UtilityRepository.GetTimeFromString(originatingAgentDetail.OriginatingPlannedArrivalTime);
                //

                // shipment.OriginatingPlannedDepartureDate = originatingAgentDetail.DeliveryDate;
                // shipment.OriginatingPlannedDepartureTime = UtilityRepository.GetWorkingTime(originatingAgentDetail.DeliveryTime);
                dbContext.SaveChanges();

                result.Status = true;
            }
            else
            {
                result.Status = false;
            }

            return result;
        }

        public FrayteResult OriginatingAgentReject(FrayteShipmentOriginatingAgentReject rejectionReason)
        {
            FrayteResult result = new FrayteResult();

            var shipment = dbContext.Shipments.Where(p => p.ShipmentId == rejectionReason.ShipmentId).FirstOrDefault();

            if (shipment != null)
            {
                shipment.ShipmentStatusId = (int)FrayteShipmentStatus.AgentReject;
                dbContext.SaveChanges();

                ShipmentAgentRejection agentRejection = new ShipmentAgentRejection();
                agentRejection.CreatedOn = DateTime.UtcNow;
                agentRejection.OriginatingAgentId = shipment.OriginatingAgentId.HasValue ? shipment.OriginatingAgentId.Value : 0;
                agentRejection.RejectionReason = rejectionReason.RejectionReason;
                agentRejection.ShipmentId = rejectionReason.ShipmentId;
                dbContext.ShipmentAgentRejections.Add(agentRejection);
                dbContext.SaveChanges();
            }

            result.Status = true;
            return result;

        }

        public FrayteResult UpdateDestinatingAgentAnticipatedDetail(FrayteShipmentDestinatingAgentAnticipatedDetail destinatingAgentAnticipatedDetail)
        {
            FrayteResult result = new FrayteResult();

            var shipment = dbContext.Shipments.Where(p => p.ShipmentId == destinatingAgentAnticipatedDetail.ShipmentId).FirstOrDefault();

            if (shipment != null)
            {
                shipment.ShipmentStatusId = (int)FrayteShipmentStatus.DestinatingAgentAnticipated;
                shipment.DestinatingDeliveryDate = destinatingAgentAnticipatedDetail.AnticipatedDeliveryDate;
                shipment.DestinatingDeliveryTime = UtilityRepository.GetTimeFromString(destinatingAgentAnticipatedDetail.AnticipatedDeliveryTime);
                if (!String.IsNullOrEmpty(destinatingAgentAnticipatedDetail.UnexpectedClearance))
                {
                    shipment.UnexpectedClearance = destinatingAgentAnticipatedDetail.UnexpectedClearance;
                }
                if (!String.IsNullOrEmpty(destinatingAgentAnticipatedDetail.OtherCustomIssues))
                {
                    shipment.DestinatingDeliveryCustomIssue = destinatingAgentAnticipatedDetail.OtherCustomIssues;
                }
                dbContext.SaveChanges();

                result.Status = true;
            }
            else
            {
                result.Status = false;
            }

            return result;
        }

        public FrayteResult UpdateShipperAnticipatedDetail(FrayteShipmentDestinatingAgentAnticipatedDetail destinatingAgentAnticipatedDetail)
        {
            FrayteResult result = new FrayteResult();

            var shipment = dbContext.Shipments.Where(p => p.ShipmentId == destinatingAgentAnticipatedDetail.ShipmentId).FirstOrDefault();

            if (shipment != null)
            {
                shipment.DestinatingDeliveryDate = destinatingAgentAnticipatedDetail.AnticipatedDeliveryDate;
                shipment.DestinatingDeliveryTime = UtilityRepository.GetTimeFromString(destinatingAgentAnticipatedDetail.AnticipatedDeliveryTime);
                dbContext.SaveChanges();
                result.Status = true;
            }
            else
            {
                result.Status = false;
            }

            return result;
        }

        public FrayteResult UpdateFlightSeaDetail(FrayteShipmentFlightSeaDetail flightSeaDetail)
        {
            FrayteResult result = new FrayteResult();

            var shipment = dbContext.Shipments.Where(p => p.ShipmentId == flightSeaDetail.ShipmentId).FirstOrDefault();

            if (shipment != null)
            {
                shipment.ShipmentStatusId = (int)FrayteShipmentStatus.UpdateFlightSea;
                shipment.FlightVessel = flightSeaDetail.FlightVessel;
                shipment.ETDDate = flightSeaDetail.ETDDate;
                shipment.ETDTime = UtilityRepository.GetTimeFromString(flightSeaDetail.ETDTime);
                shipment.ETADate = flightSeaDetail.ETADate;
                shipment.ETATime = UtilityRepository.GetTimeFromString(flightSeaDetail.ETATime);
                shipment.MABBL = flightSeaDetail.MABBL;
                dbContext.SaveChanges();

                result.Status = true;
            }
            else
            {
                result.Status = false;
            }

            return result;
        }

        public FrayteResult UpdateAWBDetail(FrayteShipmentAWBDetail awbDetail)
        {
            FrayteResult result = new FrayteResult();

            var shipment = dbContext.Shipments.Where(p => p.ShipmentId == awbDetail.ShipmentId).FirstOrDefault();

            if (shipment != null)
            {
                shipment.ShipmentStatusId = (int)FrayteShipmentStatus.AWBUploaded;
                shipment.AWB = awbDetail.AWB;
                shipment.OtherDocs = awbDetail.OtherDocs;
                dbContext.SaveChanges();

                result.Status = true;
            }
            else
            {
                result.Status = false;
            }

            return result;
        }

        public FrayteResult ReselectAgent(FrayteShipmentReslectAgent agentDetail)
        {
            FrayteResult result = new FrayteResult();

            var shipment = dbContext.Shipments.Where(p => p.ShipmentId == agentDetail.ShipmentId).FirstOrDefault();

            if (shipment != null)
            {
                shipment.ShipmentStatusId = (int)FrayteShipmentStatus.NewBooking;
                shipment.OriginatingAgentId = agentDetail.OriginatingAgent.UserId;
                dbContext.SaveChanges();

                result.Status = true;
            }
            else
            {
                result.Status = false;
            }

            return result;
        }

        public FrayteShipmentReslectAgentModel GetReselectAgentShipmentDetail(int shipmentId)
        {
            FrayteShipmentReslectAgentModel result = new FrayteShipmentReslectAgentModel();
            //Customer,Shipping By, destination country, 

            //result.ShipmentId = shipmentId;
            //result.CustomerName = "";
            //result.Country = new FrayteCountryCode() { CountryId = 95, Code = "IND", Name = "India" };
            //result.Agent = new FrayteUserModel();
            //result.ShippingMethod = "FedEx";



            var result1 = (from s in dbContext.Shipments
                           join u in dbContext.Users on s.CustomerId equals u.UserId into custTemp
                           from ct in custTemp
                           join a in dbContext.Users on s.OriginatingAgentId equals a.UserId into tempAgent
                           from at in tempAgent
                           join car in dbContext.Couriers on s.DeliveredBy equals car.CourierId
                           join ra in dbContext.UserAddresses on s.OriginatingAddressId equals ra.UserAddressId
                           join ctr in dbContext.Countries on ra.CountryId equals ctr.CountryId
                           where s.ShipmentId == shipmentId
                           select new FrayteShipmentReslectAgentModel()
                           {
                               ShipmentId = s.ShipmentId,
                               CustomerName = ct.ContactName,
                               Country = new FrayteCountryCode() { Name = ctr.CountryName, Code = ctr.CountryCode, CountryId = ctr.CountryId },
                               ShippingMethod = car.CourierName,
                               Agent = new FrayteUserModel() { Name = at.ContactName, UserId = at.UserId }
                           }).FirstOrDefault();

            return result1;
        }

        public FrayteResult UdatePODDetail(FrayteShipmentPODDetail podDetail)
        {
            FrayteResult result = new FrayteResult();

            var shipment = dbContext.Shipments.Where(p => p.ShipmentId == podDetail.ShipmentId).FirstOrDefault();

            if (shipment != null)
            {
                //Get desinating agent time zone name
                var timezoneResult = (from a in dbContext.Users
                                      join t in dbContext.Timezones on a.TimezoneId equals t.TimezoneId
                                      where a.UserId == shipment.DestinatingAgentId
                                      select t.Name).FirstOrDefault();

                shipment.ShipmentStatusId = (int)FrayteShipmentStatus.Close;
                shipment.FinalDeliveryDate = podDetail.PODDeliveryDate;
                if (!string.IsNullOrEmpty(timezoneResult))
                {
                    shipment.FinalDeliveryTime = UtilityRepository.GetTimeZoneUTCTime(podDetail.PODDeliveryTime, timezoneResult);
                }
                else
                {
                    shipment.FinalDeliveryTime = UtilityRepository.GetTimeZoneUTCTime(podDetail.PODDeliveryTime, shipment.TimezoneId);
                }
                if (!String.IsNullOrEmpty(podDetail.ExceptionNote))
                {
                    shipment.ExceptionNote = podDetail.ExceptionNote;
                }
                shipment.Signature = podDetail.Signature;
                shipment.FinalImage = podDetail.SignatureImage;
                dbContext.SaveChanges();

                result.Status = true;
            }
            else
            {
                result.Status = false;
            }

            return result;
        }

        public spGet_ShipmentDetailE1_Result GetE1Detail(int ShipmentId)
        {
            var e1Detail = dbContext.spGet_ShipmentDetailE1(ShipmentId).FirstOrDefault();
            return e1Detail;
        }

        public spGet_ShipmentDetailE2_Result GetE2Detail(int ShipmentId)
        {
            var e2Detail = dbContext.spGet_ShipmentDetailE2(ShipmentId).FirstOrDefault();
            return e2Detail;
        }

        public spGet_ShipmentDetailE3_Result GetE3Detail(int ShipmentId)
        {
            var e3Detail = dbContext.spGet_ShipmentDetailE3(ShipmentId).FirstOrDefault();
            return e3Detail;
        }

        public spGet_ShipmentDetailE31_Result GetE31Detail(int ShipmentId)
        {
            var e31Detail = dbContext.spGet_ShipmentDetailE31(ShipmentId).FirstOrDefault();
            return e31Detail;
        }

        public spGet_ShipmentDetailE4_Result GetE4Detail(int ShipmentId)
        {
            var e4Detail = dbContext.spGet_ShipmentDetailE4(ShipmentId).FirstOrDefault();
            return e4Detail;
        }

        public spGet_ShipmentDetailE5_Result GetE5Detail(int ShipmentId)
        {
            var e5Detail = dbContext.spGet_ShipmentDetailE5(ShipmentId).FirstOrDefault();
            return e5Detail;
        }

        public spGet_ShipmentDetailE51_Result GetE51Detail(int ShipmentId)
        {
            var e51Detail = dbContext.spGet_ShipmentDetailE51(ShipmentId).FirstOrDefault();
            return e51Detail;
        }

        public spGet_ShipmentDetailE6_Result GetE6Detail(int ShipmentId)
        {
            var e6Detail = dbContext.spGet_ShipmentDetailE6(ShipmentId).FirstOrDefault();
            return e6Detail;
        }

        public spGet_ShipmentDetailE61_Result GetE61Detail(int ShipmentId)
        {
            var e61Detail = dbContext.spGet_ShipmentDetailE61(ShipmentId).FirstOrDefault();
            return e61Detail;
        }

        public spGet_ShipmentDetailE64_Result GetE64Detail(int ShipmentId)
        {
            var e64Detail = dbContext.spGet_ShipmentDetailE64(ShipmentId).FirstOrDefault();
            return e64Detail;
        }

        public spGet_ShipmentDetailE65_Result GetE65Detail(int ShipmentId)
        {
            var e65Detail = dbContext.spGet_ShipmentDetailE65(ShipmentId).FirstOrDefault();
            return e65Detail;
        }

        public spGet_ShipmentDetailE67_Result GetE67Detail(int ShipmentId)
        {
            var e67Detail = dbContext.spGet_ShipmentDetailE67(ShipmentId).FirstOrDefault();
            return e67Detail;
        }

        public spGet_ShipmentDetailE7_Result GetE7Detail(int ShipmentId)
        {
            var e7Detail = dbContext.spGet_ShipmentDetailE7(ShipmentId).FirstOrDefault();
            return e7Detail;
        }

        public spGet_ShipmentDetailE71_Result GetE71Detail(int ShipmentId)
        {
            var e71Detail = dbContext.spGet_ShipmentDetailE71(ShipmentId).FirstOrDefault();
            return e71Detail;
        }

        public spGet_ShipmentDetailE8_Result GetE8Detail(int ShipmentId)
        {
            var e8Detail = dbContext.spGet_ShipmentDetailE8(ShipmentId).FirstOrDefault();
            return e8Detail;
        }

        public spGet_ShipmentDetailE81_Result GetE81Detail(int ShipmentId)
        {
            var e81Detail = dbContext.spGet_ShipmentDetailE81(ShipmentId).FirstOrDefault();
            return e81Detail;
        }

        public spGet_ShipmentDetailE9_Result GetE9Detail(int ShipmentId)
        {
            var e9Detail = dbContext.spGet_ShipmentDetailE9(ShipmentId).FirstOrDefault();
            return e9Detail;
        }

        public spGet_ShipmentDetailE91_Result GetE91Detail(int ShipmentId)
        {
            var e91Detail = dbContext.spGet_ShipmentDetailE91(ShipmentId).FirstOrDefault();
            return e91Detail;
        }

        public spGet_ShipmentDetailE10_Result GetE10Detail(int ShipmentId)
        {
            var e10Detail = dbContext.spGet_ShipmentDetailE10(ShipmentId).FirstOrDefault();
            return e10Detail;
        }

        public spGet_ShipmentDetailE101_Result GetE101Detail(int ShipmentId)
        {
            var e101Detail = dbContext.spGet_ShipmentDetailE101(ShipmentId).FirstOrDefault();
            return e101Detail;
        }

        public spGet_ShipmentDetailE11_Result GetE11Detail(int ShipmentId)
        {
            var e11Detail = dbContext.spGet_ShipmentDetailE11(ShipmentId).FirstOrDefault();
            return e11Detail;
        }

        public spGet_ShipmentDetailE12_Result GetE12Detail(int ShipmentId)
        {
            var e12Detail = dbContext.spGet_ShipmentDetailE12(ShipmentId).FirstOrDefault();
            return e12Detail;
        }

        public spGet_ShipmentDetailE13_Result GetE13Detail(int ShipmentId)
        {
            var e13Detail = dbContext.spGet_ShipmentDetailE13(ShipmentId).FirstOrDefault();
            return e13Detail;
        }

        public spGet_ShipmentDetailE131_Result GetE131Detail(int ShipmentId)
        {
            var e131Detail = dbContext.spGet_ShipmentDetailE131(ShipmentId).FirstOrDefault();
            return e131Detail;
        }

        public spGet_ShipmentDetailE14_Result GetE14Detail(int ShipmentId)
        {
            var e14Detail = dbContext.spGet_ShipmentDetailE14(ShipmentId).FirstOrDefault();
            return e14Detail;
        }

        public spGet_ShipmentDetailE15_Result GetE15Detail(int ShipmentId)
        {
            var e15Detail = dbContext.spGet_ShipmentDetailE15(ShipmentId).FirstOrDefault();
            return e15Detail;
        }

        public spGet_ShipmentDetailE16_Result GetE16Detail(int ShipmentId)
        {
            var e16Detail = dbContext.spGet_ShipmentDetailE16(ShipmentId).FirstOrDefault();
            return e16Detail;
        }

        public bool CheckValidExcel(DataTable table)
        {
            bool valid = true;

            DataColumnCollection columns = table.Columns;

            if (!columns.Contains("JobNumber"))
            {
                valid = false;
            }
            if (!columns.Contains("JobStyle"))
            {
                valid = false;
            }
            if (!columns.Contains("HSCode"))
            {
                valid = false;
            }
            if (!columns.Contains("CartonQty"))
            {
                valid = false;
            }
            if (!columns.Contains("Pieces"))
            {
                valid = false;
            }
            if (!columns.Contains("WeightKg"))
            {
                valid = false;
            }
            if (!columns.Contains("Lcms"))
            {
                valid = false;
            }
            if (!columns.Contains("Wcms"))
            {
                valid = false;
            }
            if (!columns.Contains("Hcms"))
            {
                valid = false;
            }
            if (!columns.Contains("PiecesContent"))
            {
                valid = false;
            }

            return valid;
        }

        public List<FrayteShipmentDetail> GetPiecesDetail(int shipmentId, DataTable exceldata)
        {
            List<FrayteShipmentDetail> _shipmentdetail = new List<FrayteShipmentDetail>();
            FrayteShipmentDetail frayteshipment;

            foreach (DataRow shipmentdetail in exceldata.Rows)
            {
                frayteshipment = new FrayteShipmentDetail();

                frayteshipment.ShipmentDetailId = 0;
                frayteshipment.ShipmentId = shipmentId;
                frayteshipment.JobNumber = shipmentdetail["JobNumber"].ToString();
                frayteshipment.PiecesContent = shipmentdetail["PiecesContent"].ToString();
                frayteshipment.JobStyle = shipmentdetail["JobStyle"].ToString();
                frayteshipment.HSCode = CommonConversion.ConvertToInt(shipmentdetail["HSCode"].ToString());

                frayteshipment.CartonQty = shipmentdetail["CartonQty"] != null ? Convert.ToInt32(shipmentdetail["CartonQty"].ToString()) : 0;
                frayteshipment.Pieces = shipmentdetail["Pieces"] != null ? Convert.ToInt32(shipmentdetail["Pieces"].ToString()) : 0;

                frayteshipment.WeightKg = shipmentdetail["WeightKg"] != null ? Convert.ToDecimal(shipmentdetail["WeightKg"].ToString()) : 0;
                frayteshipment.Lcms = shipmentdetail["Lcms"] != null ? Convert.ToDecimal(shipmentdetail["Lcms"].ToString()) : 0;
                frayteshipment.Wcms = shipmentdetail["Wcms"] != null ? Convert.ToDecimal(shipmentdetail["Wcms"].ToString()) : 0;
                frayteshipment.Hcms = shipmentdetail["Hcms"] != null ? Convert.ToDecimal(shipmentdetail["Hcms"].ToString()) : 0;

                _shipmentdetail.Add(frayteshipment);
            }

            return _shipmentdetail;
        }

        public string getExcelConnectionString(string FileName, string filepath)
        {
            if (Path.GetExtension(FileName) == ".xlsx")
            {
                return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=Excel 12.0";
            }
            else if (Path.GetExtension(FileName) == ".xls")
            {
                return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";Extended Properties=Excel 8.0";
            }
            return "";
        }

        public bool IsTelexUploaded(int shipmentId)
        {
            var shipment = dbContext.Shipments.Where(p => p.ShipmentId == shipmentId).FirstOrDefault();

            if (shipment != null)
            {
                if (!string.IsNullOrEmpty(shipment.SeaPOL) &&
                    !string.IsNullOrEmpty(shipment.SeaPOD) &&
                    !string.IsNullOrEmpty(shipment.SeaTelexDocument) &&
                    !string.IsNullOrEmpty(shipment.TelexReleaseBy))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        //internal object GetE101Detail(int ShipmentId)
        //{
        //    var e101Detail = dbContext.spGet_ShipmentDetailE101(ShipmentId);
        //    return e101Detail;
        //}

        public string CreateCargoWiseXML(int shipmentId)
        {
            FrayteShipment shipmentDetail = GetShipmentDetail(shipmentId);

            //xmlPath = It will be the path where xml will saved            
            string xmlPath = HttpContext.Current.Server.MapPath("~/UploadFiles/Shipments/" + shipmentId);
            if (!Directory.Exists(xmlPath))
            {
                Directory.CreateDirectory(xmlPath);
            }

            xmlPath = xmlPath + "/CargoWiseShipment_" + shipmentId + ".xml";
            XmlWriter xmlWriter = XmlWriter.Create(xmlPath);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("UniversalInterchange", "http://www.cargowise.com/Schemas/Universal/2011/11");
            xmlWriter.WriteAttributeString("version", "1.0");

            xmlWriter.WriteStartElement("Header");
            xmlWriter.WriteStartElement("SenderID");
            xmlWriter.WriteString("IntegrateCargo");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("RecipientID");
            xmlWriter.WriteString("FRYHKGHKG");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();

            ////Start of Body
            xmlWriter.WriteStartElement("Body");

            //Start of UniversalShipment
            xmlWriter.WriteStartElement("UniversalShipment", "http://www.cargowise.com/Schemas/Universal/2011/11");
            xmlWriter.WriteAttributeString("xmlns", "http://www.cargowise.com/Schemas/Universal/2011/11");
            xmlWriter.WriteAttributeString("version", "1.0");
            // Start of shipment 
            xmlWriter.WriteStartElement("Shipment");

            #region Data Context

            // Start of DataContext 
            xmlWriter.WriteStartElement("DataContext");

            // Start of DataTargetCollection 
            xmlWriter.WriteStartElement("DataTargetCollection");
            xmlWriter.WriteStartElement("DataTarget");
            xmlWriter.WriteStartElement("Type");
            if (shipmentDetail.DeliveredBy.CourierType == "Air" || shipmentDetail.DeliveredBy.CourierType == "Sea")
            {
                xmlWriter.WriteString("ForwardingShipment");
            }
            else
            {
                xmlWriter.WriteString("ForwardingBooking");
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
            // End of DataTargetCollection
            xmlWriter.WriteEndElement();

            //Start of DataProvider
            xmlWriter.WriteStartElement("DataProvider");
            xmlWriter.WriteString("FRYHKGHKG");
            //End of DataProvider
            xmlWriter.WriteEndElement();

            // End of DataContext
            xmlWriter.WriteEndElement();

            // Start of ContainerMode
            xmlWriter.WriteStartElement("ContainerMode");
            xmlWriter.WriteStartElement("Code");
            xmlWriter.WriteString("LSE");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("Description");
            xmlWriter.WriteString("Loose");
            xmlWriter.WriteEndElement();
            // End of ContainerMode
            xmlWriter.WriteEndElement();

            // Start of ServiceLevel
            xmlWriter.WriteStartElement("ServiceLevel");
            xmlWriter.WriteStartElement("Code");
            xmlWriter.WriteString(shipmentDetail.ShipmentDuitable.Code);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("Description");
            xmlWriter.WriteString(shipmentDetail.ShipmentDuitable.Description);
            xmlWriter.WriteEndElement();
            // End of ServiceLevel
            xmlWriter.WriteEndElement();

            #endregion

            //Start of BookingConfirmationReference
            xmlWriter.WriteStartElement("BookingConfirmationReference");
            xmlWriter.WriteString(shipmentDetail.ShippingReference);
            //End of BookingConfirmationReference
            xmlWriter.WriteEndElement();

            //Start of GoodsDescription
            xmlWriter.WriteStartElement("GoodsDescription");
            xmlWriter.WriteString(shipmentDetail.ContentDescription);
            //End of GoodsDescription
            xmlWriter.WriteEndElement();

            if (shipmentDetail.DeclaredValue.HasValue)
            {
                //Start of GoodsValue
                xmlWriter.WriteStartElement("GoodsValue");
                xmlWriter.WriteString(shipmentDetail.DeclaredValue.Value.ToString());
                //End of GoodsValue
                xmlWriter.WriteEndElement();

                // Start of GoodsValueCurrency
                xmlWriter.WriteStartElement("GoodsValueCurrency");
                xmlWriter.WriteStartElement("Code");
                xmlWriter.WriteString(shipmentDetail.DeclaredCurrency.CurrencyCode);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("Description");
                xmlWriter.WriteString(shipmentDetail.DeclaredCurrency.CurrencyDescription);
                xmlWriter.WriteEndElement();
                // End of GoodsValueCurrency
                xmlWriter.WriteEndElement();
            }


            // Start of ShipmentIncoTerm
            // Shipment Detial -> Shipment Term
            xmlWriter.WriteStartElement("ShipmentIncoTerm");
            xmlWriter.WriteStartElement("Code");
            xmlWriter.WriteString(shipmentDetail.ShipmentTerm.Code);
            xmlWriter.WriteEndElement();
            // End of ShipmentIncoTerm
            xmlWriter.WriteEndElement();

            // Start of OuterPacksPackageType
            // Shipment Detial -> Packaging Type
            xmlWriter.WriteStartElement("OuterPacksPackageType");
            xmlWriter.WriteStartElement("Code");
            xmlWriter.WriteString(shipmentDetail.PackagingType.Code);
            xmlWriter.WriteEndElement();
            // End of ShipmentIncoTerm
            xmlWriter.WriteEndElement();

            // Start of TransportMode
            xmlWriter.WriteStartElement("TransportMode");
            xmlWriter.WriteStartElement("Code");
            if (shipmentDetail.DeliveredBy.CourierType == FrayteShipmentType.Courier)
            {
                // This is special handling
                xmlWriter.WriteString("Air");
            }
            else
            {
                xmlWriter.WriteString(shipmentDetail.DeliveredBy.CourierType);
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("Description");
            xmlWriter.WriteString(shipmentDetail.DeliveredBy.Name);
            xmlWriter.WriteEndElement();
            //End of TransportMode
            xmlWriter.WriteEndElement();

            // Start of LocalProcessing
            xmlWriter.WriteStartElement("LocalProcessing");

            // Start of PickupRequiredBy 
            xmlWriter.WriteStartElement("PickupRequiredBy");
            xmlWriter.WriteString(shipmentDetail.ShippingDate.ToString());

            // End of PickupRequiredBy
            xmlWriter.WriteEndElement();

            // Start of OrderNumberCollection
            xmlWriter.WriteStartElement("OrderNumberCollection");

            // Start of OrderNumber
            xmlWriter.WriteStartElement("OrderNumber");
            xmlWriter.WriteStartElement("OrderReference");
            xmlWriter.WriteString(shipmentDetail.PurchaseOrderNo);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("Sequence");
            xmlWriter.WriteString("1");
            xmlWriter.WriteEndElement();
            // End of OrderNumber
            xmlWriter.WriteEndElement();

            // End of OrderNumberCollection
            xmlWriter.WriteEndElement();

            // End of LocalProcessing
            xmlWriter.WriteEndElement();

            #region CustomizedFieldCollection

            // Start of CustomizedFieldCollection
            xmlWriter.WriteStartElement("CustomizedFieldCollection");

            // Start of CustomizedField 1
            xmlWriter.WriteStartElement("CustomizedField");

            xmlWriter.WriteStartElement("Key");
            xmlWriter.WriteString("FRAYTE SHIPMENT ID");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("DataType");
            xmlWriter.WriteString("String");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("Value");
            xmlWriter.WriteString(shipmentDetail.ShipmentId.ToString());
            xmlWriter.WriteEndElement();

            // End of CustomizedField 1
            xmlWriter.WriteEndElement();

            // Start of CustomizedField 2
            xmlWriter.WriteStartElement("CustomizedField");

            xmlWriter.WriteStartElement("Key");
            xmlWriter.WriteString("SPECIAL INSTRUCTIONS");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("DataType");
            xmlWriter.WriteString("String");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("Value");
            xmlWriter.WriteString(shipmentDetail.SpecialInstruction);
            xmlWriter.WriteEndElement();

            // End of CustomizedField 2
            xmlWriter.WriteEndElement();

            // Start of CustomizedField 3
            xmlWriter.WriteStartElement("CustomizedField");

            xmlWriter.WriteStartElement("Key");
            xmlWriter.WriteString("SPECIAL NEEDS");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("DataType");
            xmlWriter.WriteString("String");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("Value");
            xmlWriter.WriteString(shipmentDetail.SpecialDelivery.Detail);

            xmlWriter.WriteEndElement();

            // End of CustomizedField 3
            xmlWriter.WriteEndElement();

            if (shipmentDetail.DeliveredBy != null && shipmentDetail.DeliveredBy.CourierType == FrayteShipmentType.Courier)
            {
                // Start of CustomizedField 4
                xmlWriter.WriteStartElement("CustomizedField");

                xmlWriter.WriteStartElement("Key");
                xmlWriter.WriteString("SHIPPING COURIER");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("DataType");
                xmlWriter.WriteString("String");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("Value");
                xmlWriter.WriteString(shipmentDetail.DeliveredBy.Name);
                xmlWriter.WriteEndElement();

                // End of CustomizedField 4
                xmlWriter.WriteEndElement();
            }

            // End of CustomizedFieldCollection
            xmlWriter.WriteEndElement();

            #endregion CustomizedFieldCollection

            #region OrganizationAddressCollection

            // Start of OrganizationAddressCollection
            xmlWriter.WriteStartElement("OrganizationAddressCollection");

            #region " Receiver Detail"

            // Start of OrganizationAddress 1
            xmlWriter.WriteStartElement("OrganizationAddress");

            // Start of AddressType
            xmlWriter.WriteStartElement("AddressType");
            xmlWriter.WriteString("ConsigneeDocumentaryAddress");
            // End of AddressType
            xmlWriter.WriteEndElement();

            // Start of Address1
            xmlWriter.WriteStartElement("Address1");
            xmlWriter.WriteString(shipmentDetail.ReceiverAddress.Address);
            // End of Address1
            xmlWriter.WriteEndElement();

            // Start of Address2
            xmlWriter.WriteStartElement("Address2");
            xmlWriter.WriteString(shipmentDetail.ReceiverAddress.Address2);
            // End of Address1
            xmlWriter.WriteEndElement();

            // Start of AddressOverride
            xmlWriter.WriteStartElement("AddressOverride");
            xmlWriter.WriteString("false");
            // End of AddressOverride
            xmlWriter.WriteEndElement();

            // Start of City
            xmlWriter.WriteStartElement("City");
            xmlWriter.WriteString(shipmentDetail.ReceiverAddress.City);
            // End of City
            xmlWriter.WriteEndElement();

            // Start of CompanyName
            xmlWriter.WriteStartElement("CompanyName");
            xmlWriter.WriteString(shipmentDetail.Receiver.CompanyName);
            // End of CompanyName
            xmlWriter.WriteEndElement();

            // Start of Country
            xmlWriter.WriteStartElement("Country");
            xmlWriter.WriteStartElement("Code");
            xmlWriter.WriteString(shipmentDetail.ReceiverAddress.Country.Code2);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("Name");
            xmlWriter.WriteString(shipmentDetail.ReceiverAddress.Country.Name);
            xmlWriter.WriteEndElement();
            // End of Country
            xmlWriter.WriteEndElement();

            // Start of Email
            xmlWriter.WriteStartElement("Email");
            xmlWriter.WriteString(shipmentDetail.Receiver.Email);
            // End of Email
            xmlWriter.WriteEndElement();

            // Start of Fax
            xmlWriter.WriteStartElement("Fax");
            xmlWriter.WriteString(shipmentDetail.Receiver.FaxNumber);
            // End of Fax
            xmlWriter.WriteEndElement();

            // Start of Phone
            xmlWriter.WriteStartElement("Phone");
            xmlWriter.WriteString(shipmentDetail.Receiver.TelephoneNo);
            // End of Phone
            xmlWriter.WriteEndElement();

            //// Start of Port
            //xmlWriter.WriteStartElement("Port");
            //xmlWriter.WriteStartElement("Code");
            //xmlWriter.WriteString("AUSYD");
            //xmlWriter.WriteEndElement();
            //xmlWriter.WriteStartElement("Name");
            //xmlWriter.WriteString("Sydney");
            //xmlWriter.WriteEndElement();
            //// End of Port
            //xmlWriter.WriteEndElement();

            // Start of Postcode
            xmlWriter.WriteStartElement("Postcode");
            xmlWriter.WriteString(shipmentDetail.ReceiverAddress.Zip);
            // End of Postcode
            xmlWriter.WriteEndElement();

            // Start of ScreeningStatus
            xmlWriter.WriteStartElement("ScreeningStatus");
            xmlWriter.WriteStartElement("Code");
            xmlWriter.WriteString("UNK");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("Description");
            xmlWriter.WriteString("Unknown");
            xmlWriter.WriteEndElement();
            // End of ScreeningStatus
            xmlWriter.WriteEndElement();

            // Start of State
            xmlWriter.WriteStartElement("State");
            xmlWriter.WriteString(shipmentDetail.ReceiverAddress.State);
            // End of State
            xmlWriter.WriteEndElement();

            // Start of RegistrationNumberCollection
            xmlWriter.WriteStartElement("RegistrationNumberCollection");
            // Start of RegistrationNumber
            xmlWriter.WriteStartElement("RegistrationNumber");
            // Start of Type
            xmlWriter.WriteStartElement("Type");
            xmlWriter.WriteStartElement("Code");
            xmlWriter.WriteString("GTX");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("Description");
            xmlWriter.WriteString("Government Tax File Code");
            xmlWriter.WriteEndElement();
            // End of Type
            xmlWriter.WriteEndElement();

            // Start of Value
            xmlWriter.WriteStartElement("Value");
            xmlWriter.WriteString(shipmentDetail.Receiver.VATGST);
            // End of Value
            xmlWriter.WriteEndElement();

            // End of RegistrationNumber
            xmlWriter.WriteEndElement();
            // End of RegistrationNumberCollection
            xmlWriter.WriteEndElement();

            // End of OrganizationAddress 1
            xmlWriter.WriteEndElement();

            #endregion "Receiver Detail"

            #region "Shipper Detail "

            // Start of OrganizationAddress 3
            xmlWriter.WriteStartElement("OrganizationAddress");

            // Start of AddressType
            xmlWriter.WriteStartElement("AddressType");
            xmlWriter.WriteString("ConsignorDocumentaryAddress");
            // End of AddressType
            xmlWriter.WriteEndElement();

            // Start of Address1
            xmlWriter.WriteStartElement("Address1");
            xmlWriter.WriteString(shipmentDetail.ShipperAddress.Address);
            // End of Address1
            xmlWriter.WriteEndElement();

            // Start of Address2
            xmlWriter.WriteStartElement("Address2");
            xmlWriter.WriteString(shipmentDetail.ShipperAddress.Address2);
            // End of Address1
            xmlWriter.WriteEndElement();

            // Start of AddressOverride
            xmlWriter.WriteStartElement("AddressOverride");
            xmlWriter.WriteString("false");
            // End of AddressOverride
            xmlWriter.WriteEndElement();

            // Start of City
            xmlWriter.WriteStartElement("City");
            xmlWriter.WriteString(shipmentDetail.ShipperAddress.Country.Name);
            // End of City
            xmlWriter.WriteEndElement();

            // Start of CompanyName
            xmlWriter.WriteStartElement("CompanyName");
            xmlWriter.WriteString(shipmentDetail.Shipper.CompanyName);
            // End of CompanyName
            xmlWriter.WriteEndElement();

            // Start of Country
            xmlWriter.WriteStartElement("Country");

            xmlWriter.WriteStartElement("Code");
            xmlWriter.WriteString(shipmentDetail.ShipperAddress.Country.Code2);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Name");
            xmlWriter.WriteString(shipmentDetail.ShipperAddress.Country.Name);
            xmlWriter.WriteEndElement();

            // End of Country
            xmlWriter.WriteEndElement();

            // Start of Email
            xmlWriter.WriteStartElement("Email");
            xmlWriter.WriteString(shipmentDetail.Shipper.Email);
            // End of Email
            xmlWriter.WriteEndElement();

            // Start of Fax
            xmlWriter.WriteStartElement("Fax");
            xmlWriter.WriteString(shipmentDetail.Shipper.FaxNumber);
            // End of Fax
            xmlWriter.WriteEndElement();

            // Start of Phone
            xmlWriter.WriteStartElement("Phone");
            xmlWriter.WriteString(shipmentDetail.Shipper.TelephoneNo);
            // End of Phone
            xmlWriter.WriteEndElement();

            //// Start of Port
            //xmlWriter.WriteStartElement("Port");
            //xmlWriter.WriteStartElement("Code");
            //xmlWriter.WriteString("HKHKG");
            //xmlWriter.WriteEndElement();
            //xmlWriter.WriteStartElement("Name");
            //xmlWriter.WriteString("Hong Kong");
            //xmlWriter.WriteEndElement();
            //// End of Port
            //xmlWriter.WriteEndElement();

            // Start of Postcode
            xmlWriter.WriteStartElement("Postcode");
            xmlWriter.WriteString(shipmentDetail.ShipperAddress.Zip);
            // End of Postcode
            xmlWriter.WriteEndElement();

            // Start of ScreeningStatus
            xmlWriter.WriteStartElement("ScreeningStatus");
            xmlWriter.WriteStartElement("Code");
            xmlWriter.WriteString("UNK");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("Description");
            xmlWriter.WriteString("Unknown");
            xmlWriter.WriteEndElement();
            // End of ScreeningStatus
            xmlWriter.WriteEndElement();

            // Start of State
            xmlWriter.WriteStartElement("State");
            xmlWriter.WriteString(shipmentDetail.ShipperAddress.State);
            // End of State
            xmlWriter.WriteEndElement();

            //// Start of RegistrationNumberCollection
            //xmlWriter.WriteStartElement("RegistrationNumberCollection");
            //// Start of RegistrationNumber
            //xmlWriter.WriteStartElement("RegistrationNumber");

            //// Start of Type
            //xmlWriter.WriteStartElement("Type");

            //xmlWriter.WriteStartElement("Code");
            //xmlWriter.WriteString("GTX");
            //xmlWriter.WriteEndElement();
            //xmlWriter.WriteStartElement("Description");
            //xmlWriter.WriteString("Government Tax File Code");
            //xmlWriter.WriteEndElement();

            //// End of Type
            //xmlWriter.WriteEndElement();

            //// Start of CountryOfIssue
            //xmlWriter.WriteStartElement("CountryOfIssue");
            //xmlWriter.WriteStartElement("Code");
            //xmlWriter.WriteString(shipmentDetail.ShipperAddress.Country.Code);
            //xmlWriter.WriteEndElement();
            //xmlWriter.WriteStartElement("Description");
            //xmlWriter.WriteString(shipmentDetail.ShipperAddress.Country.Name);
            //xmlWriter.WriteEndElement();
            //// End of CountryOfIssue
            //xmlWriter.WriteEndElement();

            //// Start of Value
            //xmlWriter.WriteStartElement("Value");
            //xmlWriter.WriteString(shipmentDetail.Shipper.VATGST);
            //// End of Value 
            //xmlWriter.WriteEndElement();

            //// End of RegistrationNumber
            //xmlWriter.WriteEndElement();
            //// End of RegistrationNumberCollection
            //xmlWriter.WriteEndElement();


            // End of OrganizationAddress 3
            xmlWriter.WriteEndElement();

            #endregion "Shipper Detail "

            if (shipmentDetail.OtherPickupAddress)
            {
                #region "Shipper Pickup Details"

                // Start of OrganizationAddress 4
                xmlWriter.WriteStartElement("OrganizationAddress");

                // Start of AddressType
                xmlWriter.WriteStartElement("AddressType");
                xmlWriter.WriteString("ConsignorPickupDeliveryAddress");
                // End of AddressType
                xmlWriter.WriteEndElement();

                // Start of Address1
                xmlWriter.WriteStartElement("Address1");
                xmlWriter.WriteString(shipmentDetail.PickupAddress.Address);
                // End of Address1
                xmlWriter.WriteEndElement();

                // Start of Address2
                xmlWriter.WriteStartElement("Address2");
                xmlWriter.WriteString(shipmentDetail.PickupAddress.Address2);
                // End of Address2
                xmlWriter.WriteEndElement();

                // Start of AddressOverride
                xmlWriter.WriteStartElement("AddressOverride");
                xmlWriter.WriteString("false");
                // End of AddressOverride
                xmlWriter.WriteEndElement();

                // Start of City
                xmlWriter.WriteStartElement("City");
                xmlWriter.WriteString(shipmentDetail.PickupAddress.City);
                // End of City
                xmlWriter.WriteEndElement();

                // Start of CompanyName
                xmlWriter.WriteStartElement("CompanyName");
                xmlWriter.WriteString(shipmentDetail.Shipper.CompanyName);
                // End of CompanyName
                xmlWriter.WriteEndElement();

                // Start of Country
                xmlWriter.WriteStartElement("Country");
                xmlWriter.WriteStartElement("Code");
                xmlWriter.WriteString(shipmentDetail.PickupAddress.Country.Code2);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("Name");
                xmlWriter.WriteString(shipmentDetail.PickupAddress.Country.Name);
                xmlWriter.WriteEndElement();
                // End of Country
                xmlWriter.WriteEndElement();

                // Start of Email
                xmlWriter.WriteStartElement("Email");
                xmlWriter.WriteString(shipmentDetail.Shipper.Email);
                // End of Email
                xmlWriter.WriteEndElement();

                // Start of Phone
                xmlWriter.WriteStartElement("Phone");
                xmlWriter.WriteString(shipmentDetail.ShipmentPickupContactPhoneNumber);
                // End of Phone
                xmlWriter.WriteEndElement();

                //// Start of Port
                //xmlWriter.WriteStartElement("Port");
                //xmlWriter.WriteStartElement("Code");
                //xmlWriter.WriteString("HKHKG");
                //xmlWriter.WriteEndElement();
                //xmlWriter.WriteStartElement("Name");
                //xmlWriter.WriteString("Hong Kong");
                //xmlWriter.WriteEndElement();
                //// End of Port
                //xmlWriter.WriteEndElement();

                // Start of Postcode
                xmlWriter.WriteStartElement("Postcode");
                xmlWriter.WriteString(shipmentDetail.PickupAddress.Zip);
                // End of Postcode
                xmlWriter.WriteEndElement();

                // Start of ScreeningStatus
                xmlWriter.WriteStartElement("ScreeningStatus");
                xmlWriter.WriteStartElement("Code");
                xmlWriter.WriteString("UNK");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("Description");
                xmlWriter.WriteString("Unknown");
                xmlWriter.WriteEndElement();
                // End of ScreeningStatus
                xmlWriter.WriteEndElement();

                // Start of State
                xmlWriter.WriteStartElement("State");
                xmlWriter.WriteString(shipmentDetail.PickupAddress.State);
                // End of State
                xmlWriter.WriteEndElement();

                // End of OrganizationAddress 4
                xmlWriter.WriteEndElement();

                #endregion "Shipper Pickup Details"
            }

            // End of OrganizationAddressCollection
            xmlWriter.WriteEndElement();

            #endregion OrganizationAddressCollection

            #region PackingLineCollection

            // Start of PackingLineCollection
            xmlWriter.WriteStartElement("PackingLineCollection");

            foreach (FrayteShipmentDetail package in shipmentDetail.ShipmentDetails)
            {
                // Start of PackingLine
                xmlWriter.WriteStartElement("PackingLine");

                // Start of ExportReferenceNumber
                xmlWriter.WriteStartElement("ExportReferenceNumber");
                xmlWriter.WriteString(package.JobNumber);
                // End of ExportReferenceNumber
                xmlWriter.WriteEndElement();

                // Start of DetailedDescription
                xmlWriter.WriteStartElement("DetailedDescription");
                xmlWriter.WriteString(package.JobStyle);
                // End of DetailedDescription
                xmlWriter.WriteEndElement();

                // Start of HarmonisedCode
                xmlWriter.WriteStartElement("HarmonisedCode");
                xmlWriter.WriteString(package.HSCode.ToString());
                // End of HarmonisedCode
                xmlWriter.WriteEndElement();

                // Start of ItemNo
                xmlWriter.WriteStartElement("ItemNo");
                xmlWriter.WriteString(package.CartonQty.ToString());
                // End of ItemNo
                xmlWriter.WriteEndElement();

                // Start of PackQty
                xmlWriter.WriteStartElement("PackQty");
                xmlWriter.WriteString(package.Pieces.ToString());
                // End of PackQty
                xmlWriter.WriteEndElement();

                // Start of Weight
                xmlWriter.WriteStartElement("Weight");
                xmlWriter.WriteString(package.WeightKg.Value.ToString());
                // End of Weight
                xmlWriter.WriteEndElement();

                // Start of Length
                xmlWriter.WriteStartElement("Length");
                xmlWriter.WriteString(package.Lcms.ToString());
                // End of Length
                xmlWriter.WriteEndElement();

                // Start of Width
                xmlWriter.WriteStartElement("Width");
                xmlWriter.WriteString(package.Wcms.ToString());
                // End of Width
                xmlWriter.WriteEndElement();

                // Start of Height
                xmlWriter.WriteStartElement("Height");
                xmlWriter.WriteString(package.Hcms.ToString());
                // End of Height
                xmlWriter.WriteEndElement();

                // Start of GoodsDescription
                xmlWriter.WriteStartElement("GoodsDescription");
                xmlWriter.WriteString(package.PiecesContent);
                // End of GoodsDescription
                xmlWriter.WriteEndElement();

                // End of PackingLine
                xmlWriter.WriteEndElement();
            }

            // End of PackingLineCollection
            xmlWriter.WriteEndElement();

            #endregion PackingLineCollection

            // End of shipment
            xmlWriter.WriteEndElement();

            //End of UniversalShipment
            xmlWriter.WriteEndElement();

            //// End of Body
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();


            return xmlPath;
        }

        public void SaveEasyPostDetailTrackingDeatil(FrayteShipment shipmentDetail, EasyPost.Shipment Shipment)
        {
            ShipmentEasyPost EasyPostDeatil;

            EasyPostDeatil = dbContext.ShipmentEasyPosts.Find(shipmentDetail.CustomInfo.ShipmentCustomDetailId);
            if (EasyPostDeatil != null)
            {
                EasyPostDeatil.EasyPostShipmentId = Shipment.id;
                EasyPostDeatil.TrackingCode = Shipment.tracking_code;
                EasyPostDeatil.Carrier = Shipment.tracker.carrier;
                EasyPostDeatil.PostageLabel = Shipment.postage_label.label_url;
                EasyPostDeatil.BarcodeURL = Shipment.barcode_url;
                EasyPostDeatil.BatchMessage = Shipment.batch_message;
                EasyPostDeatil.BatchStatus = Shipment.batch_status;
                EasyPostDeatil.CreatedAt = Shipment.created_at;
                EasyPostDeatil.StampURL = Shipment.stamp_url;
                EasyPostDeatil.ShipmentServiceType = FrayteShipmentServiceType.TradeLaneBooking;
                dbContext.SaveChanges();
            }
            else
            {
                EasyPostDeatil = new ShipmentEasyPost();
                EasyPostDeatil.ShipmentId = shipmentDetail.ShipmentId;
                EasyPostDeatil.EasyPostShipmentId = Shipment.id;
                EasyPostDeatil.TrackingCode = Shipment.tracking_code;
                EasyPostDeatil.Carrier = Shipment.tracker.carrier;
                EasyPostDeatil.PostageLabel = Shipment.postage_label.label_url;
                EasyPostDeatil.BarcodeURL = Shipment.barcode_url;
                EasyPostDeatil.BatchMessage = Shipment.batch_message;
                EasyPostDeatil.BatchStatus = Shipment.batch_status;
                EasyPostDeatil.CreatedAt = Shipment.created_at;
                EasyPostDeatil.StampURL = Shipment.stamp_url;
                EasyPostDeatil.ShipmentServiceType = FrayteShipmentServiceType.TradeLaneBooking;
                dbContext.ShipmentEasyPosts.Add(EasyPostDeatil);
                dbContext.SaveChanges();
            }
        }

        #region Tracking
        #region TNT Tracking Detail
        private FrayteShipmentTracking GetTNTTrackingInfo(string carrierName, string trackingNumberCode)
        {
            // Generate Tacking request xml
            string trackXml = TNTTrackingRequestXml(carrierName, trackingNumberCode);

            string xml_in = File.ReadAllText(@trackXml);
            var trackingReply = CallWebservice(xml_in);

            if (!string.IsNullOrEmpty(trackingReply))
            {
                FrayteShipmentTracking tracking = TNTTrackingFromXML(carrierName, trackingNumberCode, trackingReply);
                return tracking;
            }
            else
            {
                return null;
            }
        }

        private FrayteShipmentTracking TNTTrackingFromXML(string carrierName, string trackingNumberCode, string trackingReply)
        {
            FrayteShipmentTracking trckInfo = new FrayteShipmentTracking();


            trckInfo.Status = true;
            trckInfo.Tracking = new List<ShipmentTracking>();

            XDocument xml = XDocument.Parse(@trackingReply);
            var list = (from r in xml.Descendants("StatusData")
                        select new
                        {
                            StatusDescription = r.Element("StatusDescription") != null ? r.Element("StatusDescription").Value : "",
                            LocalEventDate = r.Element("LocalEventDate") != null ? r.Element("LocalEventDate").Value : "",
                            LocalEventTime = r.Element("LocalEventTime") != null ? r.Element("LocalEventTime").Value : "",
                            Location = r.Element("DepotName") != null ? r.Element("DepotName").Value : "",
                            Code = r.Element("Depot") != null ? r.Element("Depot").Value : ""
                        }).ToList();
            // 
            ShipmentTracking tracking = new ShipmentTracking();
            tracking.TrackingDetails = new List<ShipmentTrackingDetail>();
            ShipmentTrackingDetail detail;
            if (list.Count > 0)
            {
                // Tracking Header Information 

                tracking.Status = list[0].StatusDescription;
                tracking.Carrier = carrierName;
                tracking.Courier = "TNT";
                tracking.TrackingNumber = trackingNumberCode;
                tracking.ShowHideValue = "Hide";
                tracking.SignedBy = ReadXMLDocument(trackingReply, "Signatory");
                tracking.EstimatedDeliveryTime = ReadXMLDocument(trackingReply, "DeliveryTime");
                tracking.CreatedAtDate = UtilityRepository.GetFormattedDateInMMDDYYYY(ReadXMLDocument(trackingReply, "CollectionDate"));
                tracking.EstimatedDeliveryDate = UtilityRepository.GetFormattedDateInMMDDYYYY(ReadXMLDocument(trackingReply, "DeliveryDate"));
                tracking.UpdatedAtDate = UtilityRepository.GetFormattedDateInMMDDYYYY(list[0].LocalEventDate);
                //tracking.UpdatedAtTime = UtilityRepository.GetFormattedTimeFromString(list[0].LocalEventTime);
                tracking.EstimatedWeight = UtilityRepository.GetEstimatedWeightByTrackingNumber(trackingNumberCode).HasValue ? UtilityRepository.GetEstimatedWeightByTrackingNumber(trackingNumberCode).Value : 0.0;
                tracking.IsHeaderShow = true;

                // Tracking Detail
                foreach (var data in list)
                {
                    detail = new ShipmentTrackingDetail();
                    detail.Activity = data.StatusDescription;
                    detail.Location = data.Location;
                    detail.Time = UtilityRepository.GetFormattedTimeFromString(data.LocalEventTime);
                    detail.Date = UtilityRepository.GettDateTimeFromString(data.LocalEventDate);  // data.LocalEventDate; convert string to datetime 
                    tracking.TrackingDetails.Add(detail);
                }
                trckInfo.Tracking.Add(tracking);
            }
            return trckInfo;
        }

        private static string ReadXMLDocument(string xml_in, string type)
        {
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(xml_in));

            XElement xmlFile = XElement.Load(ms);
            string result = string.Empty;
            var status = xmlFile.Descendants("Consignment").Elements(type);
            result = GetElementValue(status);
            return result;
        }
        private static string GetElementValue(IEnumerable<XElement> elements)
        {
            foreach (var sat in elements)
            {
                return sat.Value;
            }

            return "";
        }
        #region TNT requets xml
        private string TNTTrackingRequestXml(string carrierName, string trackingNumberCode)
        {
            string xmlPath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");
            if (!Directory.Exists(xmlPath))
            {
                Directory.CreateDirectory(xmlPath);
            }

            xmlPath = xmlPath + "/tempTNTTrackRequest.xml";

            using (var xmlWriter = XmlWriter.Create(xmlPath))
            {
                xmlWriter.WriteStartDocument();

                xmlWriter.WriteStartElement("TrackRequest");
                xmlWriter.WriteAttributeString("locale", "en_US");
                xmlWriter.WriteAttributeString("version", "3.1");

                //1. SearchCriteria
                xmlWriter.WriteStartElement("SearchCriteria");
                xmlWriter.WriteAttributeString("marketType", "INTERNATIONAL");
                xmlWriter.WriteAttributeString("originCountry", "US");

                //ConsignmentNumber 
                xmlWriter.WriteStartElement("ConsignmentNumber"); //To Do:  Later we will have list of ConsigmentNumber 
                xmlWriter.WriteString(trackingNumberCode); //450262315
                xmlWriter.WriteEndElement();
                // End of ConsignmentNumber

                // End of SearchCriteria
                xmlWriter.WriteEndElement();

                //2. Create LevelOfDetail
                xmlWriter.WriteStartElement("LevelOfDetail");

                // Complete
                xmlWriter.WriteStartElement("Complete");
                xmlWriter.WriteAttributeString("destinationAddress", "true");
                xmlWriter.WriteAttributeString("originAddress", "true");
                xmlWriter.WriteAttributeString("package", "true");
                xmlWriter.WriteAttributeString("shipment", "true");
                xmlWriter.WriteEndElement();
                //EndComplete

                // POD
                xmlWriter.WriteStartElement("POD");
                xmlWriter.WriteAttributeString("format", "URL");
                xmlWriter.WriteEndElement();
                //End Of POD

                // End of LevelOfDetail
                xmlWriter.WriteEndElement();

                // End of TrackRequest
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }

            return xmlPath;

        }
        #endregion
        private static string CallWebservice(string body)
        {


            string serverUrl = "https://express.tnt.com/expressconnect/track.do?version=3.1";
            try
            {
                //Call Express Connect

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("authorization", "Basic ZnJheXRlLXRudDpGcmF5dGVIS0dB");

                    var values = new Dictionary<string, string>
                        {
                        {"xml_in", body}
                        };
                    //urlencode content

                    var content = new FormUrlEncodedContent(values);
                    //call webservice
                    var result = client.PostAsync(serverUrl, content).Result;
                    return result.Content.ReadAsStringAsync().Result;
                }
            }
            catch (System.Exception ex)
            {
                return "";
            }
        }

        #endregion

        public FrayteShipmentTracking GetAllTrackerInfo(List<EasyPostTracking> TrackingInfo)
        {
            FrayteShipmentTracking track = new FrayteShipmentTracking();

            foreach (var TrackData in TrackingInfo)
            {
                if (TrackData != null && TrackData.Carrier != null && !String.IsNullOrEmpty(TrackData.Carrier.Name))
                {
                    if (TrackData.Carrier.Name == "UK/EU - Shipment")
                    {
                        return GetParcelHubTracingInfo(TrackData.Carrier.Name, TrackData.TrackingCode);
                    }
                    else
                    {
                        return GetTrackerInfo(TrackData.Carrier.Name, TrackData.TrackingCode);
                    }
                }
            }
            return track;
        }

        public FrayteShipmentTracking GetTrackerInfo(string CarrierName, string TrackingNumberCode)
        {
            if (!string.IsNullOrEmpty(CarrierName) && !string.IsNullOrEmpty(TrackingNumberCode))
            {

                return new AftershipTrackingRepository().GetTracking(CarrierName, TrackingNumberCode);


                //if (CarrierName.Contains("TNT"))
                //{
                //    FrayteShipmentTracking asds = GetTNTTrackingInfo(CarrierName, TrackingNumberCode);
                //    if (asds != null && asds.Tracking != null && asds.Tracking.Count > 0)
                //        return asds;
                //    else
                //        return null;
                //}

                ////UPS Tracking 

                //if (CarrierName.Contains("UPS"))
                //{
                //    FrayteShipmentTracking UpsTracking = new UPSRepository().GetUPSTrackingInfo(CarrierName, TrackingNumberCode);
                //    if (UpsTracking != null && UpsTracking.Tracking != null && UpsTracking.Tracking.Count > 0)
                //        return UpsTracking;
                //    else
                //        return null;

                //}

                //FrayteShipmentTracking easyPostTrackingDetails = new FrayteShipmentTracking();

                //ShipmentTracking trackingDetail = new ShipmentTracking();
                //var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.EasyPost);
                //if (logisticIntegration != null)
                //{
                //    EasyPost.ClientManager.SetCurrent(logisticIntegration.InetgrationKey);
                //    EasyPost.Tracker tracker;

                //    if (AppSettings.ApplicationMode == FrayteApplicationMode.Live)
                //    {
                //        tracker = EasyPost.Tracker.Create(CarrierName, TrackingNumberCode);
                //    }
                //    else
                //    {
                //        tracker = EasyPost.Tracker.Create("UPS", "EZ4000000004");
                //    }

                //    #region Old Code

                //    //var trackDetail = dbContext.PackageTrackingDetails.Where(p => p.TrackingNo == TrackingNumberCode).Distinct().FirstOrDefault();
                //    //if (trackDetail != null && trackDetail.PackageTrackingDetailId > 0)
                //    //{
                //    //    var createtime = (from PTD in dbContext.PackageTrackingDetails
                //    //                      join DSD in dbContext.DirectShipmentDetails on PTD.DirectShipmentDetailId equals DSD.DirectShipmentDetailId
                //    //                      join DS in dbContext.DirectShipments on DSD.DirectShipmentId equals DS.DirectShipmentId
                //    //                      where PTD.TrackingNo == TrackingNumberCode
                //    //                      select new
                //    //                      {
                //    //                          CreationDate = DS.CreatedOn
                //    //                      }).FirstOrDefault();

                //    //    var carrier = (from PTD in dbContext.PackageTrackingDetails
                //    //                   join DSD in dbContext.DirectShipmentDetails on PTD.DirectShipmentDetailId equals DSD.DirectShipmentDetailId
                //    //                   join DS in dbContext.DirectShipments on DSD.DirectShipmentId equals DS.DirectShipmentId
                //    //                   join CC in dbContext.Couriers on DS.ShippingMethodId equals CC.CourierId
                //    //                   where PTD.TrackingNo == TrackingNumberCode
                //    //                   select new
                //    //                   {
                //    //                       CarrierName = CC.DisplayName,
                //    //                       CreateDate = DS.CreatedOn,
                //    //                       Weight = DSD.Weight,
                //    //                       SignedBy = DS.SignedBy
                //    //                   }).ToList();

                //    //    if (carrier != null)
                //    //    {
                //    //        if (DateTime.Now >= createtime.CreationDate.AddMinutes(30))
                //    //        {
                //    //            trackingDetail.IsHeaderShow = true;
                //    //            trackingDetail.TrackingNumber = TrackingNumberCode;
                //    //            trackingDetail.ShowHideValue = "Hide";
                //    //            trackingDetail.EstimatedWeight = (double)carrier.Sum(x => x.Weight);

                //    //            foreach (var Obj in carrier)
                //    //            {
                //    //                trackingDetail.Carrier = Obj.CarrierName;
                //    //                trackingDetail.CreatedAtDate = Obj.CreateDate.ToString("MM/dd/yyyy");
                //    //                trackingDetail.CreatedAtTime = Obj.CreateDate.ToString("HH:MM");
                //    //                trackingDetail.SignedBy = Obj.SignedBy;
                //    //            }

                //    //            trackingDetail.Status = tracker.status.ToUpper();

                //    //            if (tracker.est_delivery_date.HasValue)
                //    //            {
                //    //                trackingDetail.EstimatedDeliveryDate = tracker.est_delivery_date.Value.Date.ToString("MM/dd/yyyy");
                //    //                var date = tracker.est_delivery_date.Value;
                //    //                trackingDetail.EstimatedDeliveryTime = tracker.est_delivery_date.Value.ToString("HH:MM");
                //    //            }

                //    //            if (trackingDetail.SignedBy == "" || trackingDetail.SignedBy == null)
                //    //            {
                //    //                trackingDetail.SignedBy = tracker.signed_by;
                //    //            }

                //    //            if (tracker.updated_at.HasValue)
                //    //            {
                //    //                trackingDetail.UpdatedAtDate = tracker.updated_at.Value.Date.ToString("MM/dd/yyyy"); ;
                //    //                trackingDetail.UpdatedAtTime = tracker.updated_at.Value.ToString("HH:MM");
                //    //            }

                //    //            if (tracker != null)
                //    //            {
                //    //                trackingDetail.TrackingDetails = new List<ShipmentTrackingDetail>();
                //    //                if (tracker.tracking_details.Count > 0)
                //    //                {
                //    //                    foreach (var trackerdetail in tracker.tracking_details)
                //    //                    {
                //    //                        var track = new ShipmentTrackingDetail();
                //    //                        track.IsCollapsed = false;
                //    //                        if (trackerdetail.datetime.HasValue)
                //    //                        {
                //    //                            track.Date = trackerdetail.datetime.Value.Date;
                //    //                            track.Time = trackerdetail.datetime.Value.ToString("HH:MM");
                //    //                        }
                //    //                        track.Activity = trackerdetail.message;
                //    //                        if (trackerdetail.tracking_location != null && !string.IsNullOrEmpty(trackerdetail.tracking_location.city) && !string.IsNullOrEmpty(trackerdetail.tracking_location.country))
                //    //                        {
                //    //                            track.Location = trackerdetail.tracking_location.city + " , " + trackerdetail.tracking_location.country;
                //    //                        }
                //    //                        trackingDetail.TrackingDetails.Add(track);
                //    //                    }
                //    //                }
                //    //                easyPostTrackingDetails.Tracking = new List<ShipmentTracking>();
                //    //                easyPostTrackingDetails.Status = true;
                //    //                easyPostTrackingDetails.Tracking.Add(trackingDetail);
                //    //                return easyPostTrackingDetails;
                //    //            }
                //    //            else
                //    //            {
                //    //                easyPostTrackingDetails.Tracking = new List<ShipmentTracking>();
                //    //                easyPostTrackingDetails.Status = false;
                //    //                return easyPostTrackingDetails;
                //    //            }
                //    //        }
                //    //        else
                //    //        {
                //    //            easyPostTrackingDetails.Tracking = new List<ShipmentTracking>();
                //    //            easyPostTrackingDetails.Status = false;
                //    //            return easyPostTrackingDetails;
                //    //        }
                //    //    }
                //    //}
                //    //else
                //    //{

                //    #endregion

                //    if (tracker != null)
                //    {
                //        trackingDetail.CarriertrackingId = tracker.id;
                //        trackingDetail.IsHeaderShow = true;
                //        trackingDetail.TrackingNumber = tracker.tracking_code;
                //        trackingDetail.ShowHideValue = "Hide";
                //        trackingDetail.Carrier = tracker.carrier;
                //        trackingDetail.Status = tracker.status.ToUpper();
                //        if (tracker.est_delivery_date.HasValue)
                //        {
                //            trackingDetail.EstimatedDeliveryDate = tracker.est_delivery_date.Value.Date.ToString("MM/dd/yyyy");
                //            var date = tracker.est_delivery_date.Value;
                //            trackingDetail.EstimatedDeliveryTime = tracker.est_delivery_date.Value.ToString("HH:MM");
                //        }

                //        trackingDetail.EstimatedWeight = tracker.weight;
                //        trackingDetail.SignedBy = tracker.signed_by;
                //        if (tracker.created_at.HasValue)
                //        {
                //            trackingDetail.CreatedAtDate = tracker.created_at.Value.Date.ToString("MM/dd/yyyy");
                //            trackingDetail.CreatedAtTime = tracker.created_at.Value.ToString("HH:MM");
                //        }
                //        if (tracker.updated_at.HasValue)
                //        {
                //            trackingDetail.UpdatedAtDate = tracker.updated_at.Value.Date.ToString("MM/dd/yyyy");
                //            trackingDetail.UpdatedAtTime = tracker.updated_at.Value.ToString("HH:MM");
                //        }
                //        trackingDetail.TrackingDetails = new List<ShipmentTrackingDetail>();
                //        if (tracker.tracking_details.Count > 0)
                //        {
                //            foreach (var trackerdetail in tracker.tracking_details)
                //            {
                //                var track = new ShipmentTrackingDetail();
                //                track.IsCollapsed = false;
                //                if (trackerdetail.datetime.HasValue)
                //                {
                //                    track.Date = trackerdetail.datetime.Value.Date;
                //                    track.Time = trackerdetail.datetime.Value.ToString("HH:MM");
                //                }
                //                track.Activity = trackerdetail.message;
                //                if (trackerdetail.tracking_location != null && !string.IsNullOrEmpty(trackerdetail.tracking_location.city) && !string.IsNullOrEmpty(trackerdetail.tracking_location.country))
                //                {
                //                    track.Location = trackerdetail.tracking_location.city + " , " + trackerdetail.tracking_location.country;
                //                }
                //                trackingDetail.TrackingDetails.Add(track);
                //            }
                //        }
                //        easyPostTrackingDetails.Tracking = new List<ShipmentTracking>();
                //        easyPostTrackingDetails.Status = true;
                //        easyPostTrackingDetails.Tracking.Add(trackingDetail);
                //        return easyPostTrackingDetails;
                //    }
                //    else
                //    {
                //        easyPostTrackingDetails.Tracking = new List<ShipmentTracking>();
                //        easyPostTrackingDetails.Status = true;
                //        return easyPostTrackingDetails;
                //    }
                //}
            }
            return null;
        }

        public FrayteShipmentTracking GetParcelHubTracingInfo(string carrierName, string TrackingNo)
        {
            try
            {
                if (TrackingNo != null)
                {

                    return new AftershipTrackingRepository().GetTracking(carrierName, TrackingNo);
                }
                else
                {
                    return null;
                }
                //if (TrackingNo != null)
                //{
                //    FrayteShipmentTracking trackstatus = new FrayteShipmentTracking();

                //    var createtime = (from DS in dbContext.DirectShipments
                //                      where DS.TrackingDetail == TrackingNo
                //                      select new
                //                      {
                //                          DS.CreatedOn
                //                      }).FirstOrDefault();

                //    if (createtime != null)
                //    {
                //        if (DateTime.Now >= createtime.CreatedOn.AddMinutes(30))
                //        {
                //            var keylist = dbContext.APIKeyDetails.ToList();
                //            foreach (var apikey in keylist)
                //            {
                //                string url1 = apikey.APIName + TrackingNo;

                //                HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(url1);

                //                HttpWebResponse response1 = (HttpWebResponse)request1.GetResponse();

                //                if (response1.ContentLength > 2)
                //                {
                //                    #region "Api"

                //                    Stream stream = response1.GetResponseStream();
                //                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                //                    StreamReader readStream = new StreamReader(stream, encode);
                //                    List<FrayteParcelHub> _track = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FrayteParcelHub>>(readStream.ReadToEnd());

                //                    FrayteParcelHub ph = _track.FirstOrDefault();

                //                    #region Provider Information For Api

                //                    FrayteServiceProviderDetail fsd = GetServiceProviderDetail(ph.ShipmentId, apikey.APIId);

                //                    #endregion

                //                    string url = "http://track.phservice.co.uk/api/shipments/" + ph.ShipmentId;
                //                    url += "/events?key=" + apikey.APIKey;
                //                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //                    Stream stream1 = response.GetResponseStream();
                //                    Encoding encode1 = System.Text.Encoding.GetEncoding("utf-8");
                //                    StreamReader readStream1 = new StreamReader(stream1, encode1);
                //                    string value = readStream1.ReadToEnd();

                //                    var json1 = JsonConvert.DeserializeObject<dynamic>(value);
                //                    var data1 = ((JObject)json1).Children();

                //                    List<FraytePiecesDetail> lstPiecesDetails = new List<FraytePiecesDetail>();
                //                    List<ParcelHubTrackingObject> lstTrackingObjects = new List<ParcelHubTrackingObject>();
                //                    HashSet<DateTime> Uniqueset = new HashSet<DateTime>();

                //                    foreach (var myData in data1)
                //                    {
                //                        var convertedObj = myData;
                //                        FraytePiecesDetail piecesDetail = new FraytePiecesDetail();
                //                        piecesDetail.PartNo = ((Newtonsoft.Json.Linq.JProperty)convertedObj).Name;
                //                        piecesDetail.TrackingDetail = new List<ParcelHubTrackingObject>();

                //                        foreach (var cobj in ((Newtonsoft.Json.Linq.JProperty)convertedObj).Value.Children())
                //                        {
                //                            ParcelHubTrackingObject obj = new ParcelHubTrackingObject();
                //                            obj.eventClass = cobj["eventClass"].ToString();
                //                            obj.eventId = cobj["eventId"].ToString();
                //                            obj.eventType = cobj["eventType"].ToString();
                //                            obj.timestamp = Convert.ToDateTime(cobj["timestamp"].ToString());
                //                            obj.details = cobj["details"].ToString();
                //                            Uniqueset.Add(obj.timestamp);
                //                            lstTrackingObjects.Add(obj);
                //                            piecesDetail.TrackingDetail.Add(obj);
                //                        }

                //                        lstPiecesDetails.Add(piecesDetail);
                //                    }

                //                    var maxvalue = lstTrackingObjects.Max(p => p.timestamp);

                //                    List<List<FrayteParcelHubTrackingDetail>> details = new List<List<FrayteParcelHubTrackingDetail>>();
                //                    trackstatus.Tracking = new List<ShipmentTracking>();

                //                    ShipmentTracking st = new ShipmentTracking();

                //                    st.TrackingDetails = new List<ShipmentTrackingDetail>();
                //                    ShipmentTrackingDetail sd;

                //                    List<DateTime> groupdate = (from t in Uniqueset group t by t into g select g.Key).ToList();

                //                    foreach (DateTime unique in groupdate)
                //                    {
                //                        foreach (var partobj in lstPiecesDetails)
                //                        {
                //                            var filteredRecords = partobj.TrackingDetail.Except(partobj.TrackingDetail.Where(x => x.details.Contains(FrayteParcelHubTrackingDetailHide.Awaiting) || x.details.Contains(FrayteParcelHubTrackingDetailHide.Received) || x.details.Contains(FrayteParcelHubTrackingDetailHide.CustomerStatus) || x.details.Contains(FrayteParcelHubTrackingDetailHide.DateFailure)));

                //                            foreach (var dd in filteredRecords)
                //                            {
                //                                if (unique == dd.timestamp)
                //                                {
                //                                    var result = st.TrackingDetails.Where(p => p.Date == unique.Date && p.Time == unique.ToString("HH:mm")).FirstOrDefault();

                //                                    if (result == null)
                //                                    {
                //                                        sd = new ShipmentTrackingDetail();
                //                                        sd.IsCollapsed = false;
                //                                        sd.Date = dd.timestamp.Date;
                //                                        sd.Time = dd.timestamp.ToString("HH:mm");
                //                                        sd.Activity = dd.details;
                //                                        sd.EventType = dd.eventType;
                //                                        sd.Location = "";
                //                                        sd.Pieces = new List<string>();
                //                                        sd.Pieces.Add(partobj.PartNo);
                //                                        st.TrackingDetails.Add(sd);
                //                                    }
                //                                    else
                //                                    {
                //                                        if (result.Pieces.Contains(partobj.PartNo))
                //                                        {

                //                                        }
                //                                        else
                //                                        {
                //                                            result.Pieces.Add(partobj.PartNo);
                //                                        }
                //                                    }
                //                                }
                //                            }
                //                        }
                //                    }

                //                    trackstatus.Tracking.Add(st);
                //                    trackstatus.Status = true;

                //                    //Step Add header information at tracking page
                //                    st.TrackingNumber = TrackingNo;
                //                    st.IsHeaderShow = true;
                //                    st.ShowHideValue = "Hide";
                //                    if (fsd != null && !string.IsNullOrEmpty(fsd.service.name) && !string.IsNullOrEmpty(fsd.service.description))
                //                    {
                //                        var carrier = (from DS in dbContext.DirectShipments
                //                                       join CA in dbContext.LogisticServiceCourierAccounts on DS.CourierAccountId equals CA.LogisticServiceCourierAccountId
                //                                       join Ls in dbContext.LogisticServices on CA.LogisticServiceId equals Ls.LogisticServiceId
                //                                       where DS.TrackingDetail == TrackingNo
                //                                       select new
                //                                       {
                //                                           CarrierName = Ls.LogisticCompanyDisplay
                //                                       }).FirstOrDefault();

                //                        if (carrier != null)
                //                        {
                //                            st.Carrier = carrier.CarrierName.ToUpper() + " (" + fsd.service.name + ")";//+ " " + fsd.service.description
                //                        }
                //                        else
                //                        {
                //                            st.Carrier = "";
                //                        }
                //                    }
                //                    else
                //                    {
                //                        st.Carrier = "";
                //                    }
                //                    st.NoOfPieces = fsd.trackingReferences.Count();

                //                    st.CreatedAtDate = trackstatus.Tracking.Min(x => x.TrackingDetails.Min(y => y.Date)).Date.ToString("MM/dd/yyyy"); //_shipmentTracking.Min(x => x.TrackingDetails.Min(y => y.Date)).Date.ToString("MM/dd/yyyy");
                //                    DateTime maxdate = st.TrackingDetails.Max(p => p.Date).Date;
                //                    string time = st.TrackingDetails.Where(x => x.Date == maxdate).Max(p => p.Time);

                //                    List<string> signinfo = lstTrackingObjects.Where(p => p.timestamp == maxvalue).Select(p => p.details).ToList();

                //                    for (int i = 0; i < st.TrackingDetails.Count; i++)
                //                    {
                //                        if (st.TrackingDetails[i].Date.ToString("dd/MM/yyyy") == maxdate.ToString("dd/MM/yyyy") && st.TrackingDetails[i].Time == time)
                //                        {
                //                            if (st.TrackingDetails[i].EventType == "Delivered" || st.TrackingDetails[i].EventType == "delivered")
                //                            {
                //                                st.Status = st.TrackingDetails[i].EventType.ToUpper();
                //                            }
                //                            else if (st.TrackingDetails[i].EventType == "Delayed" || st.TrackingDetails[i].EventType == "delayed")
                //                            {
                //                                st.Status = st.TrackingDetails[i].EventType.ToUpper();
                //                            }
                //                        }
                //                    }

                //                    if (st.Status == "DELAYED" || st.Status == null || st.Status == "")
                //                    {
                //                        for (int j = 0; j < st.TrackingDetails.Count; j++)
                //                        {
                //                            DateTime max = st.TrackingDetails.Max(p => p.Date).Date.AddDays(-j);
                //                            string newtime = st.TrackingDetails[j].Time;
                //                            if (st.TrackingDetails[j].Date.ToString("dd/MM/yyyy") == max.ToString("dd/MM/yyyy") && st.TrackingDetails[j].Time == newtime)
                //                            {
                //                                st.Status = st.TrackingDetails[j].EventType.ToUpper();
                //                            }
                //                            else
                //                            {
                //                                if (st.Status == "SHIPMENT CREATED")
                //                                {

                //                                }
                //                                else
                //                                {
                //                                    st.Status = st.TrackingDetails[j].EventType.ToUpper();
                //                                }
                //                            }
                //                        }
                //                    }

                //                    st.UpdatedAtDate = lstPiecesDetails.Max(x => x.TrackingDetail.Max(y => y.timestamp)).Date.ToString("MM/dd/yyyy");

                //                    if (fsd != null)
                //                    {
                //                        st.TrackingPicesDetail = new FrayteServiceProviderDetail();
                //                        st.TrackingPicesDetail.trackingReferences = fsd.trackingReferences;
                //                    }

                //                    if (st.Status != null && !string.IsNullOrEmpty(st.Status))
                //                    {
                //                        if (st.Status.ToUpper().Contains("SHIPMENT DELIVERED") || st.Status.ToUpper().Contains("DELIVERED") || st.Status.Contains("delivered"))
                //                        {
                //                            foreach (var sign in signinfo)
                //                            {
                //                                st.SignedBy = sign.ToUpper();
                //                            }
                //                        }
                //                        else
                //                        {
                //                            st.SignedBy = "";
                //                        }
                //                    }
                //                    else
                //                    {
                //                        st.SignedBy = "";
                //                    }

                //                    var weight = (from DS in dbContext.DirectShipments
                //                                  join DSD in dbContext.DirectShipmentDetails on DS.DirectShipmentId equals DSD.DirectShipmentId
                //                                  where DS.TrackingDetail == TrackingNo
                //                                  select new
                //                                  {
                //                                      DSD.Weight
                //                                  }).ToList();

                //                    if (weight != null)
                //                    {
                //                        st.EstimatedWeight = Convert.ToDouble(weight.Sum(x => x.Weight));
                //                    }
                //                    return trackstatus;

                //                    #endregion
                //                }
                //            }
                //        }
                //        else
                //        {
                //            trackstatus.Tracking = new List<ShipmentTracking>();
                //            trackstatus.Status = false;
                //            return trackstatus;
                //        }
                //    }
                //    else
                //    {
                //        int j = 0;
                //        var keylist = dbContext.APIKeyDetails.ToList();
                //        foreach (var apikey in keylist)
                //        {
                //            string url1 = apikey.APIName + TrackingNo;

                //            HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(url1);

                //            HttpWebResponse response1 = (HttpWebResponse)request1.GetResponse();

                //            if (response1.ContentLength > 2)
                //            {
                //                #region "Api"

                //                Stream stream = response1.GetResponseStream();
                //                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                //                StreamReader readStream = new StreamReader(stream, encode);
                //                List<FrayteParcelHub> _track = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FrayteParcelHub>>(readStream.ReadToEnd());

                //                FrayteParcelHub ph = _track.FirstOrDefault();

                //                #region Provider Information For Api

                //                FrayteServiceProviderDetail fsd = GetServiceProviderDetail(ph.ShipmentId, apikey.APIId);

                //                #endregion

                //                string url = "http://track.phservice.co.uk/api/shipments/" + ph.ShipmentId;
                //                url += "/events?key=" + apikey.APIKey;
                //                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //                Stream stream1 = response.GetResponseStream();
                //                Encoding encode1 = System.Text.Encoding.GetEncoding("utf-8");
                //                StreamReader readStream1 = new StreamReader(stream1, encode1);
                //                string value = readStream1.ReadToEnd();

                //                var json1 = JsonConvert.DeserializeObject<dynamic>(value);
                //                var data1 = ((JObject)json1).Children();

                //                List<FraytePiecesDetail> lstPiecesDetails = new List<FraytePiecesDetail>();
                //                List<ParcelHubTrackingObject> lstTrackingObjects = new List<ParcelHubTrackingObject>();
                //                HashSet<DateTime> Uniqueset = new HashSet<DateTime>();

                //                foreach (var myData in data1)
                //                {
                //                    var convertedObj = myData;
                //                    FraytePiecesDetail piecesDetail = new FraytePiecesDetail();
                //                    piecesDetail.PartNo = ((Newtonsoft.Json.Linq.JProperty)convertedObj).Name;
                //                    piecesDetail.TrackingDetail = new List<ParcelHubTrackingObject>();

                //                    foreach (var cobj in ((Newtonsoft.Json.Linq.JProperty)convertedObj).Value.Children())
                //                    {
                //                        ParcelHubTrackingObject obj = new ParcelHubTrackingObject();
                //                        obj.eventClass = cobj["eventClass"].ToString();
                //                        obj.eventId = cobj["eventId"].ToString();
                //                        obj.eventType = cobj["eventType"].ToString();
                //                        obj.timestamp = Convert.ToDateTime(cobj["timestamp"].ToString());
                //                        obj.details = cobj["details"].ToString();
                //                        Uniqueset.Add(obj.timestamp);
                //                        lstTrackingObjects.Add(obj);
                //                        piecesDetail.TrackingDetail.Add(obj);
                //                    }

                //                    lstPiecesDetails.Add(piecesDetail);
                //                }

                //                var maxvalue = lstTrackingObjects.Max(p => p.timestamp);

                //                List<List<FrayteParcelHubTrackingDetail>> details = new List<List<FrayteParcelHubTrackingDetail>>();
                //                trackstatus.Tracking = new List<ShipmentTracking>();

                //                ShipmentTracking st = new ShipmentTracking();

                //                st.TrackingDetails = new List<ShipmentTrackingDetail>();
                //                ShipmentTrackingDetail sd;

                //                List<DateTime> groupdate = (from t in Uniqueset group t by t into g select g.Key).ToList();

                //                foreach (DateTime unique in groupdate)
                //                {
                //                    foreach (var partobj in lstPiecesDetails)
                //                    {
                //                        var filteredRecords = partobj.TrackingDetail.Except(partobj.TrackingDetail.Where(x => x.details.Contains(FrayteParcelHubTrackingDetailHide.Awaiting) || x.details.Contains(FrayteParcelHubTrackingDetailHide.Received) || x.details.Contains(FrayteParcelHubTrackingDetailHide.CustomerStatus) || x.details.Contains(FrayteParcelHubTrackingDetailHide.DateFailure))); //

                //                        foreach (var dd in filteredRecords)
                //                        {
                //                            if (unique == dd.timestamp)
                //                            {
                //                                var result = st.TrackingDetails.Where(p => p.Date == unique.Date && p.Time == unique.ToString("HH:mm")).FirstOrDefault();

                //                                if (result == null)
                //                                {
                //                                    sd = new ShipmentTrackingDetail();
                //                                    sd.IsCollapsed = false;
                //                                    sd.Date = dd.timestamp.Date;
                //                                    sd.Time = dd.timestamp.ToString("HH:mm");
                //                                    sd.Activity = dd.details;
                //                                    sd.EventType = dd.eventType;
                //                                    sd.Location = "";
                //                                    sd.Pieces = new List<string>();
                //                                    sd.Pieces.Add(partobj.PartNo);
                //                                    st.TrackingDetails.Add(sd);
                //                                }
                //                                else
                //                                {
                //                                    if (result.Pieces.Contains(partobj.PartNo))
                //                                    {

                //                                    }
                //                                    else
                //                                    {
                //                                        result.Pieces.Add(partobj.PartNo);
                //                                    }
                //                                }
                //                            }
                //                        }
                //                    }
                //                }
                //                trackstatus.Tracking.Add(st);
                //                trackstatus.Status = true;

                //                //Step Add header information at tracking page
                //                st.TrackingNumber = TrackingNo;
                //                st.IsHeaderShow = true;
                //                st.ShowHideValue = "Hide";
                //                if (fsd != null && !string.IsNullOrEmpty(fsd.service.name) && !string.IsNullOrEmpty(fsd.service.description))
                //                {
                //                    if (fsd.provider.name == FrayteCourierAccountCode.UKMail)
                //                    {
                //                        st.Carrier = FrayteLogisticServiceDisplayType.UKMail.ToUpper() + " (" + fsd.service.description + ")";
                //                    }
                //                    else
                //                    {
                //                        st.Carrier = fsd.provider.name.ToUpper() + " (" + fsd.service.description + ")";
                //                    }
                //                }
                //                else
                //                {
                //                    st.Carrier = "";
                //                }
                //                st.NoOfPieces = fsd.trackingReferences.Count();

                //                st.CreatedAtDate = trackstatus.Tracking.Min(x => x.TrackingDetails.Min(y => y.Date)).Date.ToString("MM/dd/yyyy");
                //                DateTime maxdate = st.TrackingDetails.Max(p => p.Date).Date;
                //                string time = st.TrackingDetails.Where(x => x.Date == maxdate).Max(p => p.Time);

                //                List<string> signinfo = lstTrackingObjects.Where(p => p.timestamp == maxvalue).Select(p => p.details).ToList();

                //                for (int i = 0; i < st.TrackingDetails.Count; i++)
                //                {
                //                    if (st.TrackingDetails[i].Date.ToString("dd/MM/yyyy") == maxdate.ToString("dd/MM/yyyy") && st.TrackingDetails[i].Time == time)
                //                    {
                //                        if (st.TrackingDetails[i].EventType == "Delivered" || st.TrackingDetails[i].EventType == "delivered")
                //                        {
                //                            st.Status = st.TrackingDetails[i].EventType.ToUpper();
                //                        }
                //                        else if (st.TrackingDetails[i].EventType == "Delayed" || st.TrackingDetails[i].EventType == "delayed")
                //                        {
                //                            st.Status = st.TrackingDetails[i].EventType.ToUpper();
                //                        }
                //                    }
                //                }

                //                if (st.Status == "DELAYED" || st.Status == null || st.Status == "")
                //                {
                //                    for (int k = 0; k < st.TrackingDetails.Count; k++)
                //                    {
                //                        DateTime max = st.TrackingDetails.Max(p => p.Date).Date.AddDays(-k);
                //                        string newtime = st.TrackingDetails[k].Time;
                //                        if (st.TrackingDetails[k].Date.ToString("dd/MM/yyyy") == max.ToString("dd/MM/yyyy") && st.TrackingDetails[k].Time == newtime)
                //                        {
                //                            st.Status = st.TrackingDetails[k].EventType.ToUpper();
                //                        }
                //                        else
                //                        {
                //                            if (st.Status == "SHIPMENT CREATED")
                //                            {

                //                            }
                //                            else
                //                            {
                //                                st.Status = st.TrackingDetails[k].EventType.ToUpper();
                //                            }
                //                        }
                //                    }
                //                }

                //                st.UpdatedAtDate = lstPiecesDetails.Max(x => x.TrackingDetail.Max(y => y.timestamp)).Date.ToString("MM/dd/yyyy");

                //                if (fsd != null)
                //                {
                //                    st.TrackingPicesDetail = new FrayteServiceProviderDetail();
                //                    st.TrackingPicesDetail.trackingReferences = fsd.trackingReferences;
                //                }

                //                if (st.Status != null && !string.IsNullOrEmpty(st.Status))
                //                {
                //                    if (st.Status.ToUpper().Contains("SHIPMENT DELIVERED") || st.Status.ToUpper().Contains("DELIVERED") || st.Status.Contains("delivered"))
                //                    {
                //                        foreach (var sign in signinfo)
                //                        {
                //                            st.SignedBy = sign.ToUpper();
                //                        }
                //                    }
                //                    else
                //                    {
                //                        st.SignedBy = "";
                //                    }
                //                }
                //                else
                //                {
                //                    st.SignedBy = "";
                //                }
                //                st.EstimatedWeight = 0;
                //                return trackstatus;

                //                #endregion

                //                j++;
                //            }
                //        }
                //        if (j == 0)
                //        {
                //            trackstatus.Tracking = new List<ShipmentTracking>();
                //            trackstatus.Status = false;
                //            return trackstatus;
                //        }
                //    }
                //}
                //return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #endregion

        public List<ShipmentTracking> GetParcelHubStatus()
        {
            List<ShipmentTracking> _shipmentTracking = new List<ShipmentTracking>();
            try
            {
                //Get Current Shipment List
                var DirectShipment = dbContext.DirectShipments.Where(p => p.ShipmentStatusId == (int)FrayteShipmentStatus.Current || p.ShipmentStatusId == (int)FrayteShipmentStatus.Delayed).ToList();
                foreach (var ds in DirectShipment)
                {
                    if (ds.TrackingDetail != null)
                    {
                        if (ds.TrackingDetail.Contains("order_"))
                        { }
                        else
                        {
                            var keylist = dbContext.APIKeyDetails.ToList();
                            foreach (var apikey in keylist)
                            {
                                string url1 = apikey.APIName + ds.TrackingDetail;

                                HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(url1);

                                ServicePointManager.DefaultConnectionLimit = 2000;

                                HttpWebResponse response1 = (HttpWebResponse)request1.GetResponse();

                                if (response1.ContentLength > 2)
                                {
                                    Stream stream = response1.GetResponseStream();
                                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                                    StreamReader readStream = new StreamReader(stream, encode);
                                    List<FrayteParcelHub> _track = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FrayteParcelHub>>(readStream.ReadToEnd());

                                    FrayteParcelHub ph = _track.FirstOrDefault();

                                    #region Provider Information For Api

                                    FrayteServiceProviderDetail fsd = GetServiceProviderDetail(ph.ShipmentId, apikey.APIId);

                                    #endregion

                                    string url = "http://track.phservice.co.uk/api/shipments/" + ph.ShipmentId;
                                    url += "/events?key=" + apikey.APIKey;
                                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                    Stream stream1 = response.GetResponseStream();
                                    Encoding encode1 = System.Text.Encoding.GetEncoding("utf-8");
                                    StreamReader readStream1 = new StreamReader(stream1, encode1);
                                    string value = readStream1.ReadToEnd();

                                    var json1 = JsonConvert.DeserializeObject<dynamic>(value);
                                    var data1 = ((JObject)json1).Children();

                                    List<ParcelHubTrackingObject> lstTrackingObjects = new List<ParcelHubTrackingObject>();
                                    List<FraytePiecesDetail> lstPiecesDetails = new List<FraytePiecesDetail>();
                                    HashSet<DateTime> Uniqueset = new HashSet<DateTime>();

                                    foreach (var myData in data1)
                                    {
                                        var convertedObj = myData;
                                        FraytePiecesDetail piecesDetail = new FraytePiecesDetail();
                                        piecesDetail.PartNo = ((Newtonsoft.Json.Linq.JProperty)convertedObj).Name;
                                        piecesDetail.TrackingDetail = new List<ParcelHubTrackingObject>();

                                        foreach (var cobj in ((Newtonsoft.Json.Linq.JProperty)convertedObj).Value.Children())
                                        {
                                            ParcelHubTrackingObject obj = new ParcelHubTrackingObject();
                                            obj.eventClass = cobj["eventClass"].ToString();
                                            obj.eventId = cobj["eventId"].ToString();
                                            obj.eventType = cobj["eventType"].ToString();
                                            obj.timestamp = Convert.ToDateTime(cobj["timestamp"].ToString());
                                            obj.details = cobj["details"].ToString();
                                            Uniqueset.Add(obj.timestamp);
                                            lstTrackingObjects.Add(obj);
                                            piecesDetail.TrackingDetail.Add(obj);
                                        }

                                        lstPiecesDetails.Add(piecesDetail);
                                    }

                                    var maxvalue = lstTrackingObjects.Max(p => p.timestamp);

                                    List<List<FrayteParcelHubTrackingDetail>> details = new List<List<FrayteParcelHubTrackingDetail>>();
                                    ShipmentTracking st = new ShipmentTracking();

                                    st.TrackingDetails = new List<ShipmentTrackingDetail>();
                                    ShipmentTrackingDetail sd;

                                    List<DateTime> groupdate = (from t in Uniqueset group t by t into g select g.Key).ToList();

                                    foreach (DateTime unique in groupdate)
                                    {
                                        foreach (var partobj in lstPiecesDetails)
                                        {
                                            var filteredRecords = partobj.TrackingDetail.Except(partobj.TrackingDetail.Where(x => x.details.Contains(FrayteParcelHubTrackingDetailHide.Awaiting) || x.details.Contains(FrayteParcelHubTrackingDetailHide.Received) || x.details.Contains(FrayteParcelHubTrackingDetailHide.CustomerStatus) || x.details.Contains(FrayteParcelHubTrackingDetailHide.DateFailure)));

                                            foreach (var dd in filteredRecords)
                                            {
                                                if (unique == dd.timestamp)
                                                {
                                                    var result = st.TrackingDetails.Where(p => p.Date == unique.Date && p.Time == unique.ToString("HH:mm")).FirstOrDefault();

                                                    if (result == null)
                                                    {
                                                        sd = new ShipmentTrackingDetail();
                                                        sd.IsCollapsed = false;
                                                        sd.Date = dd.timestamp.Date;
                                                        sd.Time = dd.timestamp.ToString("HH:mm");
                                                        sd.Activity = dd.details;
                                                        sd.EventType = dd.eventType;
                                                        sd.Location = "";
                                                        sd.Pieces = new List<string>();
                                                        sd.Pieces.Add(partobj.PartNo);
                                                        st.TrackingDetails.Add(sd);
                                                    }
                                                    else
                                                    {
                                                        if (result.Pieces.Contains(partobj.PartNo))
                                                        {

                                                        }
                                                        else
                                                        {
                                                            result.Pieces.Add(partobj.PartNo);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    _shipmentTracking.Add(st);

                                    //Step Add header information at tracking page
                                    st.DirectShipmentId = ds.DirectShipmentId;
                                    st.TrackingNumber = "";
                                    st.IsHeaderShow = true;
                                    st.ShowHideValue = "Hide";
                                    if (fsd != null && !string.IsNullOrEmpty(fsd.service.name) && !string.IsNullOrEmpty(fsd.service.description))
                                    {
                                        if (fsd.provider.name != null)
                                        {
                                            st.Carrier = fsd.provider.name.ToUpper() + " (" + fsd.service.name + ")";
                                        }
                                        else
                                        {
                                            st.Carrier = "";
                                        }
                                    }
                                    else
                                    {
                                        st.Carrier = "";
                                    }
                                    st.NoOfPieces = fsd.trackingReferences.Count();

                                    st.CreatedAtDate = _shipmentTracking.Min(x => x.TrackingDetails.Min(y => y.Date)).Date.ToString("MM/dd/yyyy");
                                    DateTime maxdate = st.TrackingDetails.Max(p => p.Date).Date;
                                    string time = st.TrackingDetails.Where(x => x.Date == maxdate).Max(p => p.Time);

                                    List<string> signinfo = lstTrackingObjects.Where(p => p.timestamp == maxvalue).Select(p => p.details).ToList();

                                    for (int i = 0; i < st.TrackingDetails.Count; i++)
                                    {
                                        if (st.TrackingDetails[i].Date.ToString("dd/MM/yyyy") == maxdate.ToString("dd/MM/yyyy") && st.TrackingDetails[i].Time == time)
                                        {
                                            if (st.TrackingDetails[i].EventType == "Delivered" || st.TrackingDetails[i].EventType == "delivered")
                                            {
                                                st.Status = st.TrackingDetails[i].EventType.ToUpper();
                                            }
                                        }
                                    }

                                    st.UpdatedAtDate = lstPiecesDetails.Max(x => x.TrackingDetail.Max(y => y.timestamp)).Date.ToString("MM/dd/yyyy");

                                    if (fsd != null)
                                    {
                                        st.TrackingPicesDetail = new FrayteServiceProviderDetail();
                                        st.TrackingPicesDetail.trackingReferences = fsd.trackingReferences;
                                    }

                                    if (st.Status != null && !string.IsNullOrEmpty(st.Status))
                                    {
                                        if (st.Status.ToUpper().Contains("SHIPMENT DELIVERED") || st.Status.ToUpper().Contains("DELIVERED") || st.Status.Contains("delivered"))
                                        {
                                            foreach (var sign in signinfo)
                                            {
                                                st.SignedBy = sign.ToUpper();
                                            }
                                        }
                                        else
                                        {
                                            st.SignedBy = "";
                                        }
                                    }
                                    else
                                    {
                                        st.SignedBy = "";
                                    }
                                }
                            }
                        }
                    }
                }
                return _shipmentTracking;
            }
            catch (Exception ex)
            {
                return null;
            }
            return _shipmentTracking;
        }

        public FrayteEasyPostShipment GetShipmentIdByTrackingCode(string code)
        {
            FrayteEasyPostShipment su;
            var data = dbContext.ShipmentEasyPosts.Where(p => p.TrackingCode == code).FirstOrDefault();
            if (data != null)
            {
                su = new FrayteEasyPostShipment();
                su.ShipmentId = data.ShipmentId;
                su.ShipmentServiceType = data.ShipmentServiceType;
                return su;
            }
            else
            {
                return null;
            }
        }

        public int GetDirectShipmentIdByShipmentId(int ShipmentId)
        {
            var data = dbContext.DirectShipments.Where(p => p.DirectShipmentId == ShipmentId).FirstOrDefault();
            return data.DirectShipmentId;
        }

        public int GetShipmentId(int ShipmentId)
        {
            var data = dbContext.Shipments.Where(p => p.ShipmentId == ShipmentId).FirstOrDefault();
            return data.ShipmentId;
        }

        public string GetCargoWiseSo(int FrayteShipmentId)
        {
            var cargo = dbContext.Shipments.Find(FrayteShipmentId).CargoWiseSo;
            return cargo;
        }

        public void SaveBarCodeSO(int ShipmentId, string barcode)
        {
            Shipment shipment;
            if (ShipmentId > 0 && !String.IsNullOrEmpty(barcode))
            {
                shipment = new Shipment();
                shipment = dbContext.Shipments.Find(ShipmentId);
                if (shipment != null)
                {
                    shipment.BarCodeSo = barcode;
                    dbContext.SaveChanges();
                }
            }
        }

        public FrayteResult CheckValidBarCode(string Barcode)
        {
            FrayteResult result = new FrayteResult();
            var shipment = dbContext.Shipments.Where(p => p.BarCodeSo == Barcode).FirstOrDefault();
            if (shipment != null && shipment.ShipmentId > 0)
            {
                result.Status = true;
                return result;
            }
            else
            {
                result.Status = false;
                return result;
            }
        }

        public List<CountryShipmentPort> GetCountryPort(int countryId)
        {
            var ports = dbContext.CountryShipmentPorts.Where(p => p.CountryId == countryId).ToList();
            if (ports != null && ports.Count > 0)
            {
                return ports;
            }
            else
            {
                return null;
            }
        }

        public FrayteShipmentFlightSeaDetail GetETAETDDetail(int shipmentId)
        {
            FrayteShipmentFlightSeaDetail frayteETAETD = new FrayteShipmentFlightSeaDetail();
            var shipment = dbContext.Shipments.Find(shipmentId);
            if (shipment != null && shipment.OriginatingPlannedDepartureDate != null && shipment.OriginatingPlannedArrivalDate != null)
            {
                frayteETAETD.ShipmentId = shipment.ShipmentId;
                frayteETAETD.ETADate = shipment.OriginatingPlannedArrivalDate.Value;
                frayteETAETD.ETDDate = shipment.OriginatingPlannedDepartureDate.Value;
                frayteETAETD.ETDTime = UtilityRepository.GetTimeZoneTime(shipment.OriginatingPlannedDepartureTime);
                frayteETAETD.ETATime = UtilityRepository.GetTimeZoneTime(shipment.OriginatingPlannedArrivalTime);
                return frayteETAETD;
            }
            else
            {
                return null;
            }
        }

        public void UpdateDirectShipmentStatus(int DirectShipmentId, int ShipmentStatusId, string SignedBy, string DeliveryDate, string DeliveryTime)
        {
            try
            {
                var rs = dbContext.DirectShipments.Where(p => p.DirectShipmentId == DirectShipmentId).FirstOrDefault();
                if (rs != null && rs.DirectShipmentId > 0)
                {
                    rs.SignedBy = SignedBy;
                    rs.DeliveryDate = DateTime.ParseExact(DeliveryDate, "MM/dd/yyyy", null);
                    rs.DeliveryTime = TimeSpan.Parse(DeliveryTime);
                    rs.ShipmentStatusId = ShipmentStatusId;
                    // Update
                    dbContext.Entry(rs).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void UpdateDirectShipmentStatus(int DirectShipmentId, int ShipmentStatusId)
        {
            try
            {
                var rs = dbContext.DirectShipments.Where(p => p.DirectShipmentId == DirectShipmentId).FirstOrDefault();
                if (rs != null && rs.DirectShipmentId > 0)
                {
                    rs.ShipmentStatusId = ShipmentStatusId;
                    // Update
                    dbContext.Entry(rs).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void UpdateShipmentStatus(int ShipmentId, int ShipmentStatusId)
        {
            try
            {
                var rs = dbContext.Shipments.Where(p => p.ShipmentId == ShipmentId).FirstOrDefault();
                if (rs != null && rs.ShipmentId > 0)
                {
                    rs.ShipmentStatusId = ShipmentStatusId;
                    // Update
                    dbContext.Entry(rs).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }

        //public FrayteShipmentTracking GetPODSingedDetail(int DirectShipmentId)
        //{
        //    try
        //    {
        //        FrayteShipmentTracking _tracking = new FrayteShipmentTracking();
        //        var CarrierName = dbContext.Carriers.Find(dbContext.DirectShipments.Find(DirectShipmentId).ShippingMethodId).CarrierName;
        //        var EasyShipmentDetail = dbContext.DirectShipmentDetails.Where(p => p.DirectShipmentId == DirectShipmentId).FirstOrDefault();
        //        if (EasyShipmentDetail != null)
        //        {
        //            var package = dbContext.PackageTrackingDetails.Where(p => p.DirectShipmentDetailId == EasyShipmentDetail.DirectShipmentDetailId).FirstOrDefault();
        //            if (package != null && package.PackageTrackingDetailId > 0)
        //            {
        //                _tracking = new ShipmentRepository().GetTrackerInfo(CarrierName, package.TrackingNo);
        //            }
        //        }
        //        else
        //        {
        //            var ParcelHubDetail = dbContext.DirectShipments.Where(p => p.DirectShipmentId == DirectShipmentId).FirstOrDefault();
        //            if (ParcelHubDetail != null && ParcelHubDetail.TrackingDetail != null)
        //            {
        //                _tracking = new ShipmentRepository().GetParcelHubTracingInfo(FrayteCourierCompany.UK_EU, ParcelHubDetail.TrackingDetail);
        //            }
        //        }
        //        return _tracking;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        #region-- Private Methods [Save Shipment information] --

        private FrayteServiceProviderDetail GetServiceProviderDetail(int ShipmentId, int APIId)
        {
            FrayteServiceProviderDetail _servicedetail = new FrayteServiceProviderDetail();
            var list = dbContext.APIKeyDetails.Where(p => p.APIId == APIId).FirstOrDefault();
            if (list != null && list.APIId > 0)
            {
                string providerurl = "http://track.phservice.co.uk/api/shipments/" + ShipmentId + "?key=" + list.APIKey;
                HttpWebRequest providerrequest = (HttpWebRequest)WebRequest.Create(providerurl);
                HttpWebResponse providerresponse = (HttpWebResponse)providerrequest.GetResponse();

                if (providerresponse.ContentLength > 2)
                {
                    Stream providerstream = providerresponse.GetResponseStream();
                    Encoding providerencode = System.Text.Encoding.GetEncoding("utf-8");
                    StreamReader providerreadStream = new StreamReader(providerstream, providerencode);
                    _servicedetail = Newtonsoft.Json.JsonConvert.DeserializeObject<FrayteServiceProviderDetail>(providerreadStream.ReadToEnd());
                    return _servicedetail;
                }
            }
            return _servicedetail;
        }

        private void SaveUser(FrayteUser frayteUser)
        {
            User user;
            if (frayteUser.UserId == 0)
            {
                user = new User();
                user.ShortName = frayteUser.ShortName;
                user.CompanyName = frayteUser.CompanyName;
                user.ContactName = frayteUser.ContactName;
                user.Email = frayteUser.Email;
                user.TelephoneNo = frayteUser.TelephoneNo;
                user.MobileNo = frayteUser.MobileNo;
                user.FaxNumber = frayteUser.FaxNumber;
                //if (frayteUser.WorkingStartTime.HasValue)
                //{
                //    user.WorkingStartTime = frayteUser.WorkingStartTime.Value.ToUniversalTime().TimeOfDay;
                //}

                //if (frayteUser.WorkingEndTime.HasValue)
                //{
                //    user.WorkingEndTime = frayteUser.WorkingEndTime.Value.ToUniversalTime().TimeOfDay;
                //}
                user.IsActive = true;
                user.VATGST = frayteUser.VATGST;
                user.CreatedOn = DateTime.UtcNow;
                user.CreatedBy = 1;
                dbContext.Users.Add(user);
            }
            else
            {
                user = dbContext.Users.Where(p => p.UserId == frayteUser.UserId).FirstOrDefault();

                if (user != null)
                {
                    user.ShortName = frayteUser.ShortName;
                    user.CompanyName = frayteUser.CompanyName;
                    user.ContactName = frayteUser.ContactName;
                    user.Email = frayteUser.Email;
                    user.TelephoneNo = frayteUser.TelephoneNo;
                    user.MobileNo = frayteUser.MobileNo;
                    user.FaxNumber = frayteUser.FaxNumber;
                    //if (frayteUser.WorkingStartTime.HasValue)
                    //{
                    //    user.WorkingStartTime = frayteUser.WorkingStartTime.Value.ToUniversalTime().TimeOfDay;
                    //}

                    //if (frayteUser.WorkingEndTime.HasValue)
                    //{
                    //    user.WorkingEndTime = frayteUser.WorkingEndTime.Value.ToUniversalTime().TimeOfDay;
                    //}
                    user.VATGST = frayteUser.VATGST;
                    user.UpdatedOn = DateTime.UtcNow;
                    user.UpdatedBy = 1;
                }
            }

            if (user != null)
            {
                dbContext.SaveChanges();
            }
            frayteUser.UserId = user.UserId;
        }

        private void SaveUserRole(int userId, int roleId)
        {
            UserRole userRole = dbContext.UserRoles.Where(p => p.UserId == userId && p.RoleId == roleId).FirstOrDefault();
            if (userRole == null)
            {
                userRole = new UserRole();
                userRole.UserId = userId;
                userRole.RoleId = roleId;

                dbContext.UserRoles.Add(userRole);
                dbContext.SaveChanges();
            }
        }

        //private void SaveUserLogin(FrayteUser frayteUser)
        //{
        //    var result = dbContext.UserLogins.Where(p => p.UserId == frayteUser.UserId).FirstOrDefault();

        //    if (result == null)
        //    {
        //        UserLogin userLogin = new UserLogin();
        //        userLogin.UserId = frayteUser.UserId;
        //        userLogin.UserName = frayteUser.Email;
        //        //To Dos: Set proper randon password with salt value
        //        Random rnd = new Random();
        //        userLogin.Password = rnd.Next(1000, 9999).ToString();                 
        //        userLogin.PasswordSalt = userLogin.Password;
        //        userLogin.IsActive = true;

        //        dbContext.UserLogins.Add(userLogin);
        //        dbContext.SaveChanges();
        //    }
        //}

        private void SaveUserAddress(FrayteAddress frayteUserAddress)
        {
            UserAddress userAddress;
            if (frayteUserAddress.UserAddressId == 0)
            {
                userAddress = new UserAddress();
                userAddress.UserId = frayteUserAddress.UserId;
                userAddress.AddressTypeId = frayteUserAddress.AddressTypeId;
                userAddress.Address = frayteUserAddress.Address;
                userAddress.Address2 = frayteUserAddress.Address2;
                userAddress.Address3 = frayteUserAddress.Address3;
                userAddress.City = frayteUserAddress.City;
                userAddress.Suburb = frayteUserAddress.Suburb;
                userAddress.State = frayteUserAddress.State;
                userAddress.Zip = frayteUserAddress.Zip;
                userAddress.CountryId = frayteUserAddress.Country.CountryId;

                dbContext.UserAddresses.Add(userAddress);
            }
            else
            {
                userAddress = dbContext.UserAddresses.Where(p => p.UserAddressId == frayteUserAddress.UserAddressId).FirstOrDefault();
                if (userAddress != null)
                {
                    userAddress.UserId = frayteUserAddress.UserId;
                    userAddress.AddressTypeId = frayteUserAddress.AddressTypeId;
                    userAddress.Address = frayteUserAddress.Address;
                    userAddress.Address2 = frayteUserAddress.Address2;
                    userAddress.Address3 = frayteUserAddress.Address3;
                    userAddress.City = frayteUserAddress.City;
                    userAddress.Suburb = frayteUserAddress.Suburb;
                    userAddress.State = frayteUserAddress.State;
                    userAddress.Zip = frayteUserAddress.Zip;
                    userAddress.CountryId = frayteUserAddress.Country.CountryId;
                }
            }

            if (userAddress != null)
            {
                dbContext.SaveChanges();
            }

            frayteUserAddress.UserAddressId = userAddress.UserAddressId;
        }

        private void SaveShipmentInfo(FrayteShipment shipment)
        {
            Shipment newshipment;
            if (shipment.ShipmentId == 0)
            {
                newshipment = new Shipment();
                newshipment.ShipmentStatusId = (int)FrayteShipmentStatus.NewBooking;
                newshipment.PurchaseOrderNumber = shipment.PurchaseOrderNo;

                //Get Customer information.
                var customer = dbContext.UserAdditionals.Where(p => p.AccountNo == shipment.CustomerAccountNumber.ToString()).FirstOrDefault();
                if (customer != null)
                {
                    newshipment.CustomerId = customer.UserId;
                }

                newshipment.ShipperId = shipment.Shipper.UserId;
                newshipment.ShipperAddressId = shipment.ShipperAddress.UserAddressId;
                newshipment.ReceiverId = shipment.Receiver.UserId;
                newshipment.ReceiverAddressId = shipment.ReceiverAddress.UserAddressId;
                if (shipment.ShipmentTerm != null)
                {
                    newshipment.ShipmentTermCode = shipment.ShipmentTerm.Code;
                }
                if (shipment.PackagingType != null)
                {
                    newshipment.PackagingTypeId = shipment.PackagingType.PackagingTypeId;
                }
                if (shipment.SpecialDelivery != null)
                {
                    newshipment.SpecialDeliveryId = shipment.SpecialDelivery.SpecialDeliveryId;
                }
                newshipment.ContentDescription = shipment.ContentDescription;
                newshipment.Guidelines = shipment.Guidelines;
                if (shipment.ShipmentDuitable != null)
                {
                    newshipment.ShipmentDuitable = shipment.ShipmentDuitable.Code;
                }
                newshipment.ShippingReference = shipment.ShippingReference;
                newshipment.ShippingDate = shipment.ShippingDate;
                if (shipment.ExportType == FrayteExportType.Export)
                {
                    if (shipment.Warehouse != null)
                    {
                        var wareHouse = dbContext.Warehouses.Find(shipment.Warehouse.WarehouseId);
                        var wareHouseTimeZone = dbContext.Timezones.Find(wareHouse.TimeZoneId);
                        if (wareHouseTimeZone != null)
                        {
                            newshipment.ShippingTime = UtilityRepository.GetTimeZoneUTCTime(shipment.ShippingTime, wareHouseTimeZone.Name).Value;
                        }
                    }
                }

                if (shipment.ExportType == FrayteExportType.Import)
                {
                    newshipment.ShippingTime = UtilityRepository.GetTimeZoneUTCTime(shipment.ShippingTime, shipment.Shipper.Timezone.Name).Value;
                }
                if (shipment.DeliveredBy != null && (shipment.DeliveredBy.CourierType == FrayteShipmentType.Air || shipment.DeliveredBy.CourierType == FrayteShipmentType.Sea))
                {
                    if (shipment.FrayteShipmentPortOfDeparture != null && shipment.FrayteShipmentPortOfDeparture.CountryShipmentPortId > 0)
                    {
                        newshipment.PortOfDepartureId = shipment.FrayteShipmentPortOfDeparture.CountryShipmentPortId;
                    }
                    if (shipment.FrayteShipmentPortOfArrival != null && shipment.FrayteShipmentPortOfArrival.CountryShipmentPortId > 0)
                    {
                        newshipment.PortOfArrivalId = shipment.FrayteShipmentPortOfArrival.CountryShipmentPortId;
                    }
                }

                newshipment.PaymentParty = shipment.PaymentParty;
                newshipment.PaymentPartyAccountNo = shipment.PaymentPartyAccountNo;
                newshipment.PaymentPartyTaxDuties = shipment.PaymentPartyTaxDuties;
                newshipment.PaymentPartyTaxDutiesAccountNo = shipment.PaymentPartyTaxDutiesAccountNo;
                newshipment.DeclaredValue = shipment.DeclaredValue;
                newshipment.DeclaredCurrency = shipment.DeclaredCurrency.CurrencyCode;
                newshipment.DeliveredBy = shipment.DeliveredBy.CourierId;
                if (shipment.OtherPickupAddress)
                {
                    newshipment.ShipmentPickupAddressId = shipment.PickupAddress.UserAddressId;
                }
                newshipment.ShipmentPickupContactName = shipment.ShipmentPickupContactName;
                newshipment.ShipmentPickupContactPhoneNumber = shipment.ShipmentPickupContactPhoneNumber;

                if (shipment.TransportToWarehouse != null && shipment.TransportToWarehouse.TransportToWarehouseId > 0)
                {
                    newshipment.TransportToWarehouseId = shipment.TransportToWarehouse.TransportToWarehouseId;
                }
                else
                {
                    newshipment.TransportToWarehouseId = 0;
                }

                if (shipment.Warehouse != null && shipment.Warehouse.WarehouseId > 0)
                {
                    newshipment.WarehouseId = shipment.Warehouse.WarehouseId;
                }
                else
                {
                    newshipment.WarehouseId = 0;
                }

                newshipment.PiecesCaculatonType = shipment.PiecesCaculatonType;

                //No Need to set the OriginatingAgentId and DestinatingAgentId here
                //We will set it only after customer confirm  the shipment.
                //Set OriginatingAgentId and DestinatingAgentId
                //SetShipmentTradelane(newshipment, shipment);

                newshipment.LocationType = shipment.LocationType;
                newshipment.LocationOfShipment = shipment.LocationOfShipment;
                newshipment.SpecialInstruction = shipment.SpecialInstruction;
                if (shipment.Warehouse != null && shipment.Warehouse.WarehouseId > 0)
                {
                    if (shipment.TransportToWarehouse.TransportToWarehouseId == 4)
                    {
                        newshipment.PickupDate = (shipment.PickupDate.HasValue ? shipment.PickupDate.Value.ToUniversalTime() : shipment.PickupDate);
                        newshipment.ShipmentReadyBy = UtilityRepository.GetTimeZoneUTCTime(shipment.ShipmentReadyBy, shipment.Timezone.TimezoneId);
                        newshipment.TimezoneId = shipment.Timezone.TimezoneId;
                    }

                    if (shipment.TransportToWarehouse.TransportToWarehouseId == 2)
                    {
                        newshipment.TrackingNumber = shipment.TrackingNumber;
                        if (shipment.OtherTransportToWareHouseCarrier != null)
                        {
                            newshipment.TransportToWareHouseCarrier = shipment.OtherTransportToWareHouseCarrier;
                        }
                        else
                        {
                            newshipment.TransportToWareHouseCarrier = shipment.TransportToWareHouseCarrier;
                        }

                    }
                    if (shipment.TransportToWarehouse.TransportToWarehouseId == 1)
                    {
                        newshipment.VehicleRegNo = shipment.VehicleRegNo;
                    }
                }


                newshipment.TermAndConditionId = shipment.TermAndConditionId;
                newshipment.CustomerConfirmCode = Guid.NewGuid();
                newshipment.CreatedOn = DateTime.UtcNow;
                newshipment.CreatedBy = 1;

                dbContext.Shipments.Add(newshipment);
            }
            else
            {
                newshipment = dbContext.Shipments.Where(p => p.ShipmentId == shipment.ShipmentId).FirstOrDefault();
                if (shipment != null)
                {
                    newshipment.PurchaseOrderNumber = shipment.PurchaseOrderNo;
                    newshipment.ShipperId = shipment.Shipper.UserId;
                    newshipment.ShipperAddressId = shipment.ShipperAddress.UserAddressId;
                    newshipment.ReceiverId = shipment.Receiver.UserId;
                    newshipment.ReceiverAddressId = shipment.ReceiverAddress.UserAddressId;
                    if (shipment.ShipmentTerm != null)
                    {
                        newshipment.ShipmentTermCode = shipment.ShipmentTerm.Code;
                    }
                    if (shipment.PackagingType != null)
                    {
                        newshipment.PackagingTypeId = shipment.PackagingType.PackagingTypeId;
                    }

                    if (shipment.SpecialDelivery != null)
                    {
                        newshipment.SpecialDeliveryId = shipment.SpecialDelivery.SpecialDeliveryId;
                    }

                    newshipment.ContentDescription = shipment.ContentDescription;
                    newshipment.Guidelines = shipment.Guidelines;
                    newshipment.ShipmentDuitable = shipment.ShipmentDuitable.Code;
                    newshipment.ShippingReference = shipment.ShippingReference;
                    newshipment.ShippingDate = shipment.ShippingDate;
                    if (shipment.ExportType == "Export")
                    {
                        if (shipment.Warehouse != null)
                        {
                            var wareHouse = dbContext.Warehouses.Find(shipment.Warehouse.WarehouseId);
                            var wareHouseTimeZone = dbContext.Timezones.Find(wareHouse.TimeZoneId);
                            if (wareHouseTimeZone != null)
                            {
                                newshipment.ShippingTime = UtilityRepository.GetTimeZoneUTCTime(shipment.ShippingTime, wareHouseTimeZone.Name).Value;
                            }
                        }
                    }

                    if (shipment.ExportType == FrayteExportType.Import)
                    {
                        newshipment.ShippingTime = UtilityRepository.GetTimeZoneUTCTime(shipment.ShippingTime, shipment.Shipper.Timezone.Name).Value;
                    }

                    if (shipment.DeliveredBy != null && (shipment.DeliveredBy.CourierType == FrayteShipmentType.Air || shipment.DeliveredBy.CourierType == FrayteShipmentType.Sea))
                    {
                        if (shipment.FrayteShipmentPortOfDeparture != null && shipment.FrayteShipmentPortOfDeparture.CountryShipmentPortId > 0)
                        {
                            newshipment.PortOfDepartureId = shipment.FrayteShipmentPortOfDeparture.CountryShipmentPortId;
                        }
                        if (shipment.FrayteShipmentPortOfArrival != null && shipment.FrayteShipmentPortOfArrival.CountryShipmentPortId > 0)
                        {
                            newshipment.PortOfArrivalId = shipment.FrayteShipmentPortOfArrival.CountryShipmentPortId;
                        }
                    }
                    newshipment.PaymentParty = shipment.PaymentParty;
                    newshipment.PaymentPartyAccountNo = shipment.PaymentPartyAccountNo;
                    newshipment.PaymentPartyTaxDuties = shipment.PaymentPartyTaxDuties;
                    newshipment.PaymentPartyTaxDutiesAccountNo = shipment.PaymentPartyTaxDutiesAccountNo;
                    newshipment.DeclaredValue = shipment.DeclaredValue;
                    newshipment.DeclaredCurrency = shipment.DeclaredCurrency.CurrencyCode;
                    newshipment.DeliveredBy = shipment.DeliveredBy.CourierId;
                    if (shipment.OtherPickupAddress)
                    {
                        newshipment.ShipmentPickupAddressId = shipment.PickupAddress.UserAddressId;
                    }
                    newshipment.ShipmentPickupContactName = shipment.ShipmentPickupContactName;

                    newshipment.ShipmentPickupContactPhoneNumber = shipment.ShipmentPickupContactPhoneNumber;

                    if (shipment.Warehouse != null && shipment.Warehouse.WarehouseId > 0)
                    {
                        newshipment.WarehouseId = shipment.Warehouse.WarehouseId;

                        if (shipment.TransportToWarehouse != null && shipment.TransportToWarehouse.TransportToWarehouseId > 0)
                        {
                            newshipment.TransportToWarehouseId = shipment.TransportToWarehouse.TransportToWarehouseId;
                        }

                        else
                        {
                            newshipment.TransportToWarehouseId = 0;
                        }
                        if (shipment.TransportToWarehouse.TransportToWarehouseId == 4)
                        {
                            newshipment.TrackingNumber = null;
                            newshipment.TransportToWareHouseCarrier = null;
                            newshipment.VehicleRegNo = null;
                            newshipment.PickupDate = (shipment.PickupDate.HasValue ? shipment.PickupDate.Value.ToUniversalTime() : shipment.PickupDate);
                            newshipment.ShipmentReadyBy = UtilityRepository.GetTimeZoneUTCTime(shipment.ShipmentReadyBy, shipment.Timezone.Name);
                            newshipment.TimezoneId = shipment.Timezone.TimezoneId;
                        }
                        if (shipment.TransportToWarehouse.TransportToWarehouseId == 2)
                        {
                            newshipment.TrackingNumber = shipment.TrackingNumber;
                            if (shipment.OtherTransportToWareHouseCarrier != null)
                            {
                                newshipment.TransportToWareHouseCarrier = shipment.OtherTransportToWareHouseCarrier;
                            }
                            else
                            {
                                newshipment.TransportToWareHouseCarrier = shipment.TransportToWareHouseCarrier;
                            }

                            newshipment.VehicleRegNo = null;
                            newshipment.PickupDate = null;
                            newshipment.ShipmentReadyBy = null;
                            newshipment.TimezoneId = 0;
                        }
                        if (shipment.TransportToWarehouse.TransportToWarehouseId == 1)
                        {
                            newshipment.VehicleRegNo = shipment.VehicleRegNo;
                            newshipment.TrackingNumber = null;
                            newshipment.TransportToWareHouseCarrier = null;
                            newshipment.PickupDate = null;
                            newshipment.ShipmentReadyBy = null;
                            newshipment.TimezoneId = 0;
                        }

                    }
                    else
                    {

                        newshipment.WarehouseId = 0;
                        newshipment.TransportToWarehouseId = 0;
                        newshipment.TimezoneId = 0;
                        newshipment.VehicleRegNo = null;
                        newshipment.TrackingNumber = null;
                        newshipment.TransportToWareHouseCarrier = null;
                        newshipment.PickupDate = null;
                        newshipment.ShipmentReadyBy = null;
                    }
                    newshipment.PiecesCaculatonType = shipment.PiecesCaculatonType;

                    //No Need to set the OriginatingAgentId and DestinatingAgentId here
                    //We will set it only after customer confirm  the shipment.
                    //Set OriginatingAgentId and DestinatingAgentId
                    //SetShipmentTradelane(newshipment, shipment);

                    newshipment.LocationType = shipment.LocationType;
                    newshipment.LocationOfShipment = shipment.LocationOfShipment;
                    newshipment.SpecialInstruction = shipment.SpecialInstruction;

                    newshipment.TermAndConditionId = shipment.TermAndConditionId;
                    newshipment.UpdatedOn = DateTime.UtcNow;
                    newshipment.UpdatedBy = 1;
                }
            }

            if (shipment != null)
            {
                dbContext.SaveChanges();
            }
            shipment.CustomerConfirmCode = newshipment.CustomerConfirmCode;
            shipment.ShipmentId = newshipment.ShipmentId;
        }

        private void SetShipmentTradelane(Shipment newshipment, FrayteShipment frayteShipment)
        {
            int originCountryId = 0;
            if (newshipment.ShipmentPickupAddressId > 0)
            {
                originCountryId = frayteShipment.PickupAddress.Country.CountryId;
            }
            else
            {
                originCountryId = frayteShipment.ShipperAddress.Country.CountryId;
            }

            var customerTradelane = (from ct in dbContext.CustomerTradeLanes
                                     join t in dbContext.Tradelanes on ct.TradeLaneId equals t.TradelaneId
                                     where ct.UserId == newshipment.CustomerId &&
                                           t.OriginCountryId == originCountryId &&
                                           t.DestinationCountryId == frayteShipment.ReceiverAddress.Country.CountryId
                                     select t).FirstOrDefault();

            if (customerTradelane != null)
            {
                newshipment.OriginatingAgentId = customerTradelane.OriginatingAgentId;
                newshipment.DestinatingAgentId = customerTradelane.DestinationAgentId;
            }
        }

        public void SetShipmentTradelane(int shipmentId)
        {
            if (shipmentId > 0)
            {
                dbContext.spSet_ShipmentTradeLane(shipmentId);
            }
        }

        private void SaveShipmentDetail(int shipmentId, List<FrayteShipmentDetail> lstShipmentDetail)
        {
            foreach (FrayteShipmentDetail frayteShipmentDetail in lstShipmentDetail)
            {
                ShipmentDetail shipmentDetail;
                if (frayteShipmentDetail.ShipmentDetailId == 0)
                {
                    shipmentDetail = new ShipmentDetail();
                    shipmentDetail.ShipmentId = shipmentId;
                    shipmentDetail.JobNumber = frayteShipmentDetail.JobNumber;
                    shipmentDetail.JobStyle = frayteShipmentDetail.JobStyle;
                    shipmentDetail.HSCode = frayteShipmentDetail.HSCode.ToString();
                    shipmentDetail.CartonQty = frayteShipmentDetail.CartonQty;
                    shipmentDetail.Pieces = frayteShipmentDetail.Pieces;
                    shipmentDetail.WeightKg = frayteShipmentDetail.WeightKg;
                    shipmentDetail.Lcms = frayteShipmentDetail.Lcms;
                    shipmentDetail.Wcms = frayteShipmentDetail.Wcms;
                    shipmentDetail.Hcms = frayteShipmentDetail.Hcms;
                    shipmentDetail.PiecesContent = frayteShipmentDetail.PiecesContent;

                    dbContext.ShipmentDetails.Add(shipmentDetail);
                }
                else
                {
                    shipmentDetail = dbContext.ShipmentDetails.Where(p => p.ShipmentDetailId == frayteShipmentDetail.ShipmentDetailId).FirstOrDefault();
                    if (shipmentDetail != null)
                    {
                        shipmentDetail.ShipmentId = shipmentId;
                        shipmentDetail.JobNumber = frayteShipmentDetail.JobNumber;
                        shipmentDetail.JobStyle = frayteShipmentDetail.JobStyle;
                        shipmentDetail.HSCode = frayteShipmentDetail.HSCode.ToString();
                        shipmentDetail.CartonQty = frayteShipmentDetail.CartonQty;
                        shipmentDetail.Pieces = frayteShipmentDetail.Pieces;
                        shipmentDetail.WeightKg = frayteShipmentDetail.WeightKg;
                        shipmentDetail.Lcms = frayteShipmentDetail.Lcms;
                        shipmentDetail.Wcms = frayteShipmentDetail.Wcms;
                        shipmentDetail.Hcms = frayteShipmentDetail.Hcms;
                        shipmentDetail.PiecesContent = frayteShipmentDetail.PiecesContent;

                    }
                }

                if (shipmentDetail != null)
                {
                    dbContext.SaveChanges();
                }

                frayteShipmentDetail.ShipmentDetailId = shipmentDetail.ShipmentDetailId;
            }
        }

        public void SaveShipmentStatus(int shipmentId, int shipmentStatusId, int createdBy)
        {
            //Step 1: Update Shipment Status in main table (shipment)
            var shipment = dbContext.Shipments.Where(p => p.ShipmentId == shipmentId).FirstOrDefault();

            if (shipment != null)
            {
                shipment.ShipmentStatusId = shipmentStatusId;
                dbContext.SaveChanges();

                //Step 2: Update the status in detail detail table
                ShipmentProgress shipmentProgress = new ShipmentProgress();
                shipmentProgress.ShipmentId = shipmentId;
                shipmentProgress.ShipmentStatusId = shipmentStatusId;
                shipmentProgress.CreatedBy = createdBy;
                shipmentProgress.CreatedOn = DateTime.UtcNow;

                dbContext.ShipmentProgresses.Add(shipmentProgress);
                dbContext.SaveChanges();
            }
        }

        private void SaveShipment(FrayteCustomerAmendShipment frayteAmendShipment)
        {
            var shipment = dbContext.Shipments.Find(frayteAmendShipment.ShipmentId);
            if (shipment != null)
            {
                if (frayteAmendShipment.FrayteShipmentPortOfArrival != null && frayteAmendShipment.FrayteShipmentPortOfArrival.CountryShipmentPortId > 0 && frayteAmendShipment.DeliveredBy != null)
                {
                    if (frayteAmendShipment.DeliveredBy.CourierType == FrayteShipmentType.Courier)
                    {
                        if (shipment.PortOfArrivalId.HasValue)
                        {
                            shipment.PortOfArrivalId = null;
                        }
                    }
                    else
                    {
                        shipment.PortOfArrivalId = frayteAmendShipment.FrayteShipmentPortOfArrival.CountryShipmentPortId;
                    }

                }
                if (frayteAmendShipment.DeliveredBy != null)
                {
                    shipment.DeliveredBy = frayteAmendShipment.DeliveredBy.CourierId;
                }
                dbContext.SaveChanges();
            }

        }

        #endregion

        #region -- Private Methods [Get Shipment Information] --

        private void GetShipmentTimezoneDetail(FrayteShipment frayteShipment)
        {
            if (frayteShipment.Timezone != null && frayteShipment.Timezone.TimezoneId > 0)
            {
                Timezone timezone = dbContext.Timezones.Where(p => p.TimezoneId == frayteShipment.Timezone.TimezoneId).FirstOrDefault();
                if (timezone != null)
                {
                    frayteShipment.Timezone.Name = timezone.Name;
                    frayteShipment.Timezone.Offset = timezone.Offset;
                }
            }
        }

        private void GetCourierDetailForShipment(FrayteShipment frayteShipment)
        {
            if (frayteShipment.DeliveredBy.CourierId > 0)
            {
                Courier courier = dbContext.Couriers.Where(p => p.CourierId == frayteShipment.DeliveredBy.CourierId).FirstOrDefault();
                if (courier != null)
                {
                    frayteShipment.DeliveredBy.Name = courier.CourierName;
                    frayteShipment.DeliveredBy.Website = courier.Website;
                    frayteShipment.DeliveredBy.CourierType = courier.ShipmentType;
                    frayteShipment.DeliveredBy.LatestBookingTime = UtilityRepository.GetWorkingTime(courier.LatestBookingTime).Value;
                }
            }
        }

        private void GetShipperAddressDetail(FrayteShipment frayteShipment)
        {
            if (frayteShipment.ShipperAddress.UserAddressId > 0)
            {
                UserAddress userAddress = dbContext.UserAddresses.Where(p => p.UserAddressId == frayteShipment.ShipperAddress.UserAddressId).FirstOrDefault();
                if (userAddress != null)
                {
                    frayteShipment.ShipperAddress.UserId = userAddress.UserId;
                    frayteShipment.ShipperAddress.AddressTypeId = userAddress.AddressTypeId;
                    frayteShipment.ShipperAddress.Address = userAddress.Address;
                    frayteShipment.ShipperAddress.Address2 = userAddress.Address2;
                    frayteShipment.ShipperAddress.Address3 = userAddress.Address3;
                    frayteShipment.ShipperAddress.Suburb = userAddress.Suburb;
                    frayteShipment.ShipperAddress.City = userAddress.City;
                    frayteShipment.ShipperAddress.State = userAddress.State;
                    frayteShipment.ShipperAddress.Zip = userAddress.Zip;
                    frayteShipment.ShipperAddress.Country = new FrayteCountryCode();
                    frayteShipment.ShipperAddress.Country.CountryId = userAddress.CountryId;

                    Country country = dbContext.Countries.Where(p => p.CountryId == userAddress.CountryId).FirstOrDefault();
                    if (country != null)
                    {
                        frayteShipment.ShipperAddress.Country.Name = country.CountryName;
                        frayteShipment.ShipperAddress.Country.Code = country.CountryCode;
                        frayteShipment.ShipperAddress.Country.Code2 = country.CountryCode2;
                        frayteShipment.ShipperAddress.Country.TimeZoneDetail = new TimeZoneModal();
                        if (country.TimeZoneId != null && country.TimeZoneId > 0)
                        {
                            var timeZone = dbContext.Timezones.Find(country.TimeZoneId.Value);
                            if (timeZone != null)
                            {
                                frayteShipment.ShipperAddress.Country.TimeZoneDetail.TimezoneId = timeZone.TimezoneId;
                                frayteShipment.ShipperAddress.Country.TimeZoneDetail.Name = timeZone.Name;
                                frayteShipment.ShipperAddress.Country.TimeZoneDetail.OffsetShort = timeZone.OffsetShort;
                                frayteShipment.ShipperAddress.Country.TimeZoneDetail.Offset = timeZone.Offset;
                            }
                        }

                    }
                }
            }
        }

        private void GetReceiverAddressDetail(FrayteShipment frayteShipment)
        {
            if (frayteShipment.ReceiverAddress.UserAddressId > 0)
            {
                UserAddress userAddress = dbContext.UserAddresses.Where(p => p.UserAddressId == frayteShipment.ReceiverAddress.UserAddressId).FirstOrDefault();
                if (userAddress != null)
                {
                    frayteShipment.ReceiverAddress.UserId = userAddress.UserId;
                    frayteShipment.ReceiverAddress.AddressTypeId = userAddress.AddressTypeId;
                    frayteShipment.ReceiverAddress.Address = userAddress.Address;
                    frayteShipment.ReceiverAddress.Address2 = userAddress.Address2;
                    frayteShipment.ReceiverAddress.Address3 = userAddress.Address3;
                    frayteShipment.ReceiverAddress.Suburb = userAddress.Suburb;
                    frayteShipment.ReceiverAddress.City = userAddress.City;
                    frayteShipment.ReceiverAddress.State = userAddress.State;
                    frayteShipment.ReceiverAddress.Zip = userAddress.Zip;
                    frayteShipment.ReceiverAddress.Country = new FrayteCountryCode();
                    frayteShipment.ReceiverAddress.Country.CountryId = userAddress.CountryId;

                    Country country = dbContext.Countries.Where(p => p.CountryId == userAddress.CountryId).FirstOrDefault();
                    if (country != null)
                    {
                        frayteShipment.ReceiverAddress.Country.Name = country.CountryName;
                        frayteShipment.ReceiverAddress.Country.Code = country.CountryCode;
                        frayteShipment.ReceiverAddress.Country.Code2 = country.CountryCode2;
                        frayteShipment.ReceiverAddress.Country.TimeZoneDetail = new TimeZoneModal();
                        if (country.TimeZoneId.HasValue)
                        {
                            var timeZone = dbContext.Timezones.Find(country.TimeZoneId.Value);
                            if (timeZone != null)
                            {
                                frayteShipment.ReceiverAddress.Country.TimeZoneDetail.TimezoneId = timeZone.TimezoneId;
                                frayteShipment.ReceiverAddress.Country.TimeZoneDetail.Name = timeZone.Name;
                                frayteShipment.ReceiverAddress.Country.TimeZoneDetail.OffsetShort = timeZone.OffsetShort;
                                frayteShipment.ReceiverAddress.Country.TimeZoneDetail.Offset = timeZone.Offset;
                            }
                        }

                    }
                }
            }
        }

        private void GetPickupAddressDetail(FrayteShipment frayteShipment)
        {
            if (frayteShipment.PickupAddress.UserAddressId > 0)
            {
                UserAddress userAddress = dbContext.UserAddresses.Where(p => p.UserAddressId == frayteShipment.PickupAddress.UserAddressId).FirstOrDefault();
                if (userAddress != null)
                {
                    frayteShipment.PickupAddress.UserId = userAddress.UserId;
                    frayteShipment.PickupAddress.AddressTypeId = userAddress.AddressTypeId;
                    frayteShipment.PickupAddress.Address = userAddress.Address;
                    frayteShipment.PickupAddress.Address2 = userAddress.Address2;
                    frayteShipment.PickupAddress.Address3 = userAddress.Address3;
                    frayteShipment.PickupAddress.Suburb = userAddress.Suburb;
                    frayteShipment.PickupAddress.City = userAddress.City;
                    frayteShipment.PickupAddress.State = userAddress.State;
                    frayteShipment.PickupAddress.Zip = userAddress.Zip;
                    frayteShipment.PickupAddress.Country = new FrayteCountryCode();
                    frayteShipment.PickupAddress.Country.CountryId = userAddress.CountryId;

                    Country country = dbContext.Countries.Where(p => p.CountryId == userAddress.CountryId).FirstOrDefault();
                    if (country != null)
                    {
                        frayteShipment.PickupAddress.Country.Name = country.CountryName;
                        frayteShipment.PickupAddress.Country.Code = country.CountryCode;
                        frayteShipment.PickupAddress.Country.Code2 = country.CountryCode2;
                    }
                }
            }
        }

        private void GetShipperDetail(FrayteShipment frayteShipment)
        {
            if (frayteShipment.Shipper.UserId > 0)
            {
                User frayteUser = dbContext.Users.Where(p => p.UserId == frayteShipment.Shipper.UserId).FirstOrDefault();
                if (frayteUser != null)
                {
                    frayteShipment.Shipper.CargoWiseId = frayteUser.CargoWiseId;
                    frayteShipment.Shipper.CargoWiseBardCode = frayteUser.CargoWiseBardCode;
                    if (frayteUser.WorkingWeekDayId > 0)
                    {
                        var weekDays = dbContext.WorkingWeekDays.Find(frayteUser.WorkingWeekDayId);
                        {
                            frayteShipment.Shipper.WorkingWeekDay = new WorkingWeekDay();
                            frayteShipment.Shipper.WorkingWeekDay = weekDays;
                        }

                    }
                    frayteShipment.Shipper.WorkingWeekDay = new WorkingWeekDay();
                    frayteShipment.Shipper.WorkingWeekDay.WorkingWeekDayId = frayteUser.WorkingWeekDayId.Value;
                    frayteShipment.Shipper.CompanyName = frayteUser.CompanyName;
                    frayteShipment.Shipper.ClientId = frayteUser.ClientId;
                    frayteShipment.Shipper.IsClient = frayteUser.IsClient;
                    frayteShipment.Shipper.CountryOfOperation = frayteUser.CountryOfOperation;
                    frayteShipment.Shipper.ContactName = frayteUser.ContactName;
                    frayteShipment.Shipper.Email = frayteUser.Email;
                    frayteShipment.Shipper.TelephoneNo = frayteUser.TelephoneNo;
                    frayteShipment.Shipper.MobileNo = frayteUser.MobileNo;
                    frayteShipment.Shipper.FaxNumber = frayteUser.FaxNumber;
                    frayteShipment.Shipper.Timezone = UtilityRepository.GetTimezoneModelDetail(frayteUser.TimezoneId);
                    frayteShipment.Shipper.WorkingStartTime = UtilityRepository.TimeZoneTime(frayteUser.WorkingStartTime, frayteShipment.Shipper.Timezone.Name);
                    frayteShipment.Shipper.WorkingEndTime = UtilityRepository.TimeZoneTime(frayteUser.WorkingEndTime, frayteShipment.Shipper.Timezone.Name);

                    frayteShipment.Shipper.VATGST = frayteUser.VATGST;
                    frayteShipment.Shipper.ShortName = frayteUser.ShortName;
                    frayteShipment.Shipper.Position = frayteUser.Position;
                    frayteShipment.Shipper.Skype = frayteUser.Skype;
                }
            }
        }

        private void GetReceiverDetail(FrayteShipment frayteShipment)
        {
            if (frayteShipment.Receiver.UserId > 0)
            {
                User frayteUser = dbContext.Users.Where(p => p.UserId == frayteShipment.Receiver.UserId).FirstOrDefault();
                if (frayteUser != null)
                {
                    frayteShipment.Receiver.CargoWiseId = frayteUser.CargoWiseId;
                    frayteShipment.Receiver.CargoWiseBardCode = frayteUser.CargoWiseBardCode;
                    if (frayteUser.WorkingWeekDayId > 0)
                    {
                        var weekDays = dbContext.WorkingWeekDays.Find(frayteUser.WorkingWeekDayId);
                        {
                            frayteShipment.Receiver.WorkingWeekDay = new WorkingWeekDay();
                            frayteShipment.Receiver.WorkingWeekDay = weekDays;
                        }

                    }
                    frayteShipment.Receiver.CompanyName = frayteUser.CompanyName;
                    frayteShipment.Receiver.ClientId = frayteUser.ClientId;
                    frayteShipment.Receiver.IsClient = frayteUser.IsClient;
                    frayteShipment.Receiver.CountryOfOperation = frayteUser.CountryOfOperation;
                    frayteShipment.Receiver.ContactName = frayteUser.ContactName;
                    frayteShipment.Receiver.Email = frayteUser.Email;
                    frayteShipment.Receiver.TelephoneNo = frayteUser.TelephoneNo;
                    frayteShipment.Receiver.MobileNo = frayteUser.MobileNo;
                    frayteShipment.Receiver.FaxNumber = frayteUser.FaxNumber;
                    frayteShipment.Receiver.Timezone = UtilityRepository.GetTimezoneModelDetail(frayteUser.TimezoneId);
                    frayteShipment.Receiver.WorkingStartTime = UtilityRepository.TimeZoneTime(frayteUser.WorkingStartTime, frayteShipment.Receiver.Timezone.Name);
                    frayteShipment.Receiver.WorkingEndTime = UtilityRepository.TimeZoneTime(frayteUser.WorkingEndTime, frayteShipment.Receiver.Timezone.Name);

                    frayteShipment.Receiver.VATGST = frayteUser.VATGST;
                    frayteShipment.Receiver.ShortName = frayteUser.ShortName;
                    frayteShipment.Receiver.Position = frayteUser.Position;
                    frayteShipment.Receiver.Skype = frayteUser.Skype;
                }
            }
        }

        private void GetShipmentDetail(FrayteShipment frayteShipment)
        {
            var shipmentDetails = dbContext.ShipmentDetails.Where(p => p.ShipmentId == frayteShipment.ShipmentId).ToList();
            if (shipmentDetails != null)
            {
                frayteShipment.ShipmentDetails = new List<FrayteShipmentDetail>();
                foreach (ShipmentDetail shipmentDetail in shipmentDetails)
                {
                    FrayteShipmentDetail frayteShipmentDetail = new FrayteShipmentDetail();
                    frayteShipmentDetail.ShipmentDetailId = shipmentDetail.ShipmentDetailId;
                    frayteShipmentDetail.ShipmentId = shipmentDetail.ShipmentId;
                    frayteShipmentDetail.JobNumber = shipmentDetail.JobNumber;
                    frayteShipmentDetail.JobStyle = shipmentDetail.JobStyle;
                    frayteShipmentDetail.HSCode = CommonConversion.ConvertToInt(shipmentDetail.HSCode);
                    frayteShipmentDetail.CartonQty = shipmentDetail.CartonQty.HasValue ? shipmentDetail.CartonQty.Value : 0;
                    frayteShipmentDetail.Pieces = shipmentDetail.Pieces;
                    frayteShipmentDetail.WeightKg = shipmentDetail.WeightKg;
                    frayteShipmentDetail.Lcms = shipmentDetail.Lcms;
                    frayteShipmentDetail.Wcms = shipmentDetail.Wcms;
                    frayteShipmentDetail.Hcms = shipmentDetail.Hcms;
                    frayteShipmentDetail.PiecesContent = shipmentDetail.PiecesContent;
                    frayteShipment.ShipmentDetails.Add(frayteShipmentDetail);
                }
            }
        }

        private void GetShipment(FrayteShipment frayteShipment)
        {
            Shipment shipment = dbContext.Shipments.Where(p => p.ShipmentId == frayteShipment.ShipmentId).FirstOrDefault();

            if (shipment != null)
            {
                frayteShipment.CargoWiseSo = shipment.CargoWiseSo;
                frayteShipment.BarCodeSo = shipment.BarCodeSo;
                frayteShipment.PurchaseOrderNo = shipment.PurchaseOrderNumber;
                frayteShipment.CustomerId = shipment.CustomerId;
                frayteShipment.CustomerAccountNumber = UtilityRepository.GetAccountNumber(shipment.ShipmentId);
                frayteShipment.OriginatingAgentId = shipment.OriginatingAgentId;
                frayteShipment.OriginatingPlannedDepartureDate = shipment.OriginatingPlannedDepartureDate;
                frayteShipment.OriginatingPlannedDepartureTime = UtilityRepository.GetTimeZoneTime(shipment.OriginatingPlannedDepartureTime);
                frayteShipment.OriginatingPlannedArrivalDate = shipment.OriginatingPlannedArrivalDate;
                frayteShipment.OriginatingPlannedArrivalTime = UtilityRepository.GetTimeZoneTime(shipment.OriginatingPlannedArrivalTime);
                frayteShipment.DestinatingAgentId = shipment.DestinatingAgentId;
                frayteShipment.Shipper = new FrayteUser();
                frayteShipment.Shipper.UserId = shipment.ShipperId;
                // two new field 
                frayteShipment.MABBL = shipment.MABBL;
                frayteShipment.FlightVessel = shipment.FlightVessel;

                frayteShipment.ShipperAddress = new FrayteAddress();
                frayteShipment.ShipperAddress.UserId = shipment.ShipperId;
                frayteShipment.ShipperAddress.UserAddressId = shipment.ShipperAddressId;

                frayteShipment.Receiver = new FrayteUser();
                frayteShipment.Receiver.UserId = shipment.ReceiverId;

                frayteShipment.ReceiverAddress = new FrayteAddress();
                frayteShipment.ReceiverAddress.UserId = shipment.ReceiverId;
                frayteShipment.ReceiverAddress.UserAddressId = shipment.ReceiverAddressId;

                frayteShipment.ShipmentTerm = new ShipmentTerm();
                frayteShipment.ShipmentTerm.Code = shipment.ShipmentTermCode;
                var shipmentTermResult = dbContext.ShipmentTerms.Where(p => p.Code == shipment.ShipmentTermCode).FirstOrDefault();
                if (shipmentTermResult != null)
                {
                    frayteShipment.ShipmentTerm.Detail = shipmentTermResult.Detail;
                }

                frayteShipment.PackagingType = new PackagingType();
                if (shipment.PackagingTypeId.HasValue)
                {
                    frayteShipment.PackagingType.PackagingTypeId = shipment.PackagingTypeId.Value;
                    var pakagingTypeResult = dbContext.PackagingTypes.Where(p => p.PackagingTypeId == shipment.PackagingTypeId).FirstOrDefault();
                    if (pakagingTypeResult != null)
                    {
                        frayteShipment.PackagingType.Name = pakagingTypeResult.Name;
                        frayteShipment.PackagingType.Code = pakagingTypeResult.Code;
                    }
                }

                frayteShipment.SpecialDelivery = new SpecialDelivery();
                frayteShipment.SpecialDelivery.SpecialDeliveryId = shipment.SpecialDeliveryId.HasValue ? shipment.SpecialDeliveryId.Value : 0;
                if (frayteShipment.SpecialDelivery.SpecialDeliveryId > 0)
                {
                    var specialDeliveryResult = dbContext.SpecialDeliveries.Where(p => p.SpecialDeliveryId == frayteShipment.SpecialDelivery.SpecialDeliveryId).FirstOrDefault();
                    if (specialDeliveryResult != null)
                    {
                        frayteShipment.SpecialDelivery.Detail = specialDeliveryResult.Detail;
                    }
                }
                frayteShipment.PiecesCaculatonType = shipment.PiecesCaculatonType;
                frayteShipment.PickupAddress = new FrayteAddress();
                frayteShipment.PickupAddress.UserId = shipment.ShipperId;
                frayteShipment.PickupAddress.UserAddressId = shipment.ShipmentPickupAddressId;

                frayteShipment.ShipmentPickupContactName = shipment.ShipmentPickupContactName;
                frayteShipment.ShipmentPickupContactPhoneNumber = shipment.ShipmentPickupContactPhoneNumber;

                frayteShipment.ContentDescription = shipment.ContentDescription;
                frayteShipment.Guidelines = shipment.Guidelines;
                frayteShipment.ShipmentDuitable = new ShipmentType();
                frayteShipment.ShipmentDuitable = dbContext.ShipmentTypes.Where(p => p.Code == shipment.ShipmentDuitable).FirstOrDefault();
                frayteShipment.ShippingReference = shipment.ShippingReference;
                frayteShipment.ShippingDate = shipment.ShippingDate;
                // get timezone id 

                frayteShipment.ShippingTime = UtilityRepository.GetTimeZoneTime(shipment.ShippingTime, getTimeZoneId(shipment));
                frayteShipment.PaymentParty = shipment.PaymentParty;
                frayteShipment.PaymentPartyAccountNo = shipment.PaymentPartyAccountNo;
                frayteShipment.PaymentPartyTaxDuties = shipment.PaymentPartyTaxDuties;
                frayteShipment.PaymentPartyTaxDutiesAccountNo = shipment.PaymentPartyTaxDutiesAccountNo;
                frayteShipment.DeclaredValue = shipment.DeclaredValue.HasValue ? shipment.DeclaredValue.Value : 0;

                frayteShipment.DeclaredCurrency = new CurrencyType();
                frayteShipment.DeclaredCurrency.CurrencyCode = shipment.DeclaredCurrency;

                frayteShipment.DeliveredBy = new FrayteShipmentCourier();
                frayteShipment.DeliveredBy.CourierId = shipment.DeliveredBy;
                frayteShipment.OtherPickupAddress = shipment.ShipmentPickupAddressId != 0;
                frayteShipment.LocationType = shipment.LocationType;
                frayteShipment.LocationOfShipment = shipment.LocationOfShipment;
                frayteShipment.SpecialInstruction = shipment.SpecialInstruction;
                if (shipment.TransportToWarehouseId == 4)
                {
                    frayteShipment.PickupDate = shipment.PickupDate.HasValue ? shipment.PickupDate.Value.ToUniversalTime() : shipment.PickupDate;
                    frayteShipment.ShipmentReadyBy = UtilityRepository.GetTimeZoneTime(shipment.ShipmentReadyBy, shipment.TimezoneId);
                    frayteShipment.Timezone = UtilityRepository.GetTimezoneModelDetail(shipment.TimezoneId);

                }
                if (shipment.TransportToWarehouseId == 2)
                {
                    frayteShipment.TrackingNumber = shipment.TrackingNumber;
                    frayteShipment.TransportToWareHouseCarrier = shipment.TransportToWareHouseCarrier;
                }
                if (shipment.TransportToWarehouseId == 1)
                {
                    frayteShipment.VehicleRegNo = shipment.VehicleRegNo;
                }
                //if (shipment.OfficeCloseAt.HasValue)
                //{
                //    frayteShipment.ShipmentReadyBy = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month,
                //                  DateTime.UtcNow.Day, shipment.OfficeCloseAt.Value.Hours, shipment.OfficeCloseAt.Value.Minutes, shipment.OfficeCloseAt.Value.Seconds);
                //}
                frayteShipment.ExportType = shipment.TransportToWarehouseId.HasValue ? (shipment.TransportToWarehouseId.Value > 0 ? FrayteExportType.Export : FrayteExportType.Import) : FrayteExportType.Import;

                frayteShipment.Warehouse = new ShipmentWarehouse();
                frayteShipment.Warehouse.WarehouseId = shipment.WarehouseId.HasValue ? shipment.WarehouseId.Value : 0;
                if (frayteShipment.Warehouse.WarehouseId > 0)
                {
                    frayteShipment.Warehouse.CountryId = dbContext.Warehouses.Find(frayteShipment.Warehouse.WarehouseId).CountryId;
                }
                frayteShipment.TransportToWarehouse = new TransportToWarehouse();
                frayteShipment.TransportToWarehouse.TransportToWarehouseId = shipment.TransportToWarehouseId.HasValue ? shipment.TransportToWarehouseId.Value : 0;

                frayteShipment.TermAndConditionId = shipment.TermAndConditionId;
                //frayteShipment.TradeLaneId = shipment.TradeLaneId;
                //frayteShipment.CommercialInvoice = shipment.CommercialInvoice;
                //frayteShipment.PackingList = shipment.PackingList;
                //frayteShipment.CustomDocument = shipment.CustomDocument;
                if (shipment.PortOfArrivalId > 0)
                {
                    frayteShipment.FrayteShipmentPortOfArrival = new CountryShipmentPort();
                    frayteShipment.FrayteShipmentPortOfArrival = dbContext.CountryShipmentPorts.Find(shipment.PortOfArrivalId);
                }
                if (shipment.PortOfDepartureId > 0)
                {
                    frayteShipment.FrayteShipmentPortOfDeparture = new CountryShipmentPort();
                    frayteShipment.FrayteShipmentPortOfDeparture = dbContext.CountryShipmentPorts.Find(shipment.PortOfDepartureId);
                }
            }
        }

        private int getTimeZoneId(Shipment shipment)
        {
            if (shipment.WarehouseId > 0)
            {

                FrayteWarehouse warehouseDetail = new FrayteWarehouse();
                warehouseDetail = new WarehouseRepository().GetWarehouseDetail(shipment.WarehouseId);

                return warehouseDetail.Timezone.TimezoneId;

            }
            else
            {
                FrayteUser shipper = new ShipperRepository().GetShipperDetail(shipment.ShipperId);
                return shipper.Timezone.TimezoneId;

            }
        }

        private void GetWarehouseAndTransportToWarehouse(FrayteShipment frayteShipment)
        {
            if (frayteShipment.Warehouse != null && frayteShipment.Warehouse.WarehouseId > 0)
            {
                Warehouse warehouse = dbContext.Warehouses.Where(p => p.WarehouseId == frayteShipment.Warehouse.WarehouseId).FirstOrDefault();
                if (warehouse != null)
                {
                    frayteShipment.Warehouse.WarehouseName = warehouse.LocationName;
                }
            }

            if (frayteShipment.TransportToWarehouse != null && frayteShipment.TransportToWarehouse.TransportToWarehouseId > 0)
            {
                TransportToWarehouse warehouseTransport = dbContext.TransportToWarehouses.Where(p => p.TransportToWarehouseId == frayteShipment.TransportToWarehouse.TransportToWarehouseId).FirstOrDefault();
                if (warehouseTransport != null)
                {
                    frayteShipment.TransportToWarehouse.Name = warehouseTransport.Name;
                }
            }
        }

        #endregion

        #endregion
    }
}