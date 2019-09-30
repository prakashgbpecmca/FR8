using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models;
using Frayte.Services.DataAccess;
using System.Net;
using System.IO;
using System.Web;
using Frayte.Services.Utility;
using System.Web.Hosting;
using System.Data.Entity.Validation;
using Spire.Barcode;
using System.Drawing;
using Microsoft.Office.Interop.Excel;
using System.Data;

namespace Frayte.Services.Business
{
    public class eCommerceShipmentRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public FrayteResult DeleteeCommerceShipmetPcsDetail(int DirectShipmentDetailId)
        {
            FrayteResult result = new FrayteResult();
            var directShipmentdetail = dbContext.DirectShipmentDetailDrafts.Find(DirectShipmentDetailId);
            if (directShipmentdetail != null)
            {
                dbContext.DirectShipmentDetailDrafts.Remove(directShipmentdetail);
                dbContext.SaveChanges();
                result.Status = true;
            }
            return result;
        }

        public List<DirectBookingCustomer> GetDirectBookingCustomers(int userId)
        {
            // To Do : customer should come according to moduleType
            var operationzone = UtilityRepository.GetOperationZone();
            var customers = (from r in dbContext.Users
                             join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                             join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                             join CM in dbContext.CustomerMarginCosts on r.UserId equals CM.CustomerId
                             where
                                ur.RoleId == (int)FrayteUserRole.Customer &&
                                r.IsActive == true &&
                                r.OperationZoneId == operationzone.OperationZoneId
                             select new DirectBookingCustomer
                             {
                                 CustomerId = r.UserId,
                                 CustomerName = r.ContactName,
                                 CompanyName = r.CompanyName,
                                 AccountNumber = ua.AccountNo,
                                 EmailId = r.Email,
                                 ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                 CustomerCurrency = ua.CreditLimitCurrencyCode,
                                 OperationZoneId = r.OperationZoneId
                             }).Distinct().ToList();
            return customers;
        }

        #region Save & Update eCommerceShipment
        public FratyteError SaveBooking(FrayteCommerceShipmentDraft eCommerceBookingDetail)
        {
            try
            {
                //Step 0.1: Set Proper PostCode
                // SetPostCode(eCommerceBookingDetail);

                //Step 1: Save ShipFrom
                eCommerceBookingDetail.ShipFrom.CustomerId = eCommerceBookingDetail.CustomerId;
                eCommerceBookingDetail.ShipFrom.AddressType = FrayteFromToAddressType.FromAddress;
                SaveeCommerceShipmentAddress(eCommerceBookingDetail.ShipFrom);

                //Step 2: Save ShipTo
                eCommerceBookingDetail.ShipTo.CustomerId = eCommerceBookingDetail.CustomerId;
                eCommerceBookingDetail.ShipTo.AddressType = FrayteFromToAddressType.ToAddress;
                SaveeCommerceShipmentAddress(eCommerceBookingDetail.ShipTo);

                //Step 3: Save Direct Shipmnet + Reference Detail
                SaveeCommerceShipmnetDetail(eCommerceBookingDetail);

                //Step 4: Save Direct Shipment Detail
                SaveeCommerceShipmentDetailPackages(eCommerceBookingDetail);

                //Save 5: Save date time when All HSCode are mapped

                SetMappedOnOneCommerce(eCommerceBookingDetail);

            }
            catch (DbEntityValidationException dbEx)
            {
                string validationErrorMsg = "";
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        validationErrorMsg = validationErrorMsg + string.Format("Property: {0} Error: {1}",
                        validationError.PropertyName,
                        validationError.ErrorMessage);
                    }
                }

                List<string> errors = new List<string>();
                errors.Add(validationErrorMsg);

                return new FratyteError() { Status = false, Miscellaneous = errors };
            }

            catch (Exception ex)
            {
                List<string> errors = new List<string>();
                errors.Add(ex.Message);

                return new FratyteError() { Status = false, Miscellaneous = errors };
            }

            return new FratyteError() { Status = true };
        }

        public bool IsInvoiceCreated(int eCommerceShipmentId)
        {
            try
            {
                var data = dbContext.eCommerceInvoices.Where(p => p.ShipmentId == eCommerceShipmentId).FirstOrDefault();
                if (data != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #region Private Methods

        private void SetMappedOnOneCommerce(FrayteCommerceShipmentDraft eCommerceBookingDetail)
        {
            var data = dbContext.DirectShipmentDrafts.Find(eCommerceBookingDetail.DirectShipmentDraftId);
            if (data != null)
            {
                data.MappedOn = DateTime.UtcNow;
                dbContext.SaveChanges();
            }
        }
        private void SaveCustomInformation(FrayteCommerceShipmentDraft eCommerceBookingDetail)
        {

            // remove the customer info if exists for same from and two address country
            if (eCommerceBookingDetail.ShipFrom.Country.Code == eCommerceBookingDetail.ShipTo.Country.Code)
            {
                var removeShipmentCustomDetail = dbContext.ShipmentCustomDetailDrafts.Where(p => p.ShipmentDraftId == eCommerceBookingDetail.DirectShipmentDraftId &&
                                                                                            p.ShipmentServiceType == FrayteShipmentServiceType.DirectBooking).FirstOrDefault();
                if (removeShipmentCustomDetail != null)
                {
                    dbContext.ShipmentCustomDetailDrafts.Remove(removeShipmentCustomDetail);
                    dbContext.SaveChanges();
                }
                return;
            }

            ShipmentCustomDetailDraft shipmentCustomDetail;

            if (eCommerceBookingDetail.CustomInfo != null && eCommerceBookingDetail.CustomInfo.ShipmentCustomDetailId > 0)
            {
                shipmentCustomDetail = dbContext.ShipmentCustomDetailDrafts.Find(eCommerceBookingDetail.CustomInfo.ShipmentCustomDetailId);
                if (shipmentCustomDetail != null)
                {
                    shipmentCustomDetail.ShipmentDraftId = eCommerceBookingDetail.DirectShipmentDraftId;
                    shipmentCustomDetail.ShipmentServiceType = FrayteShipmentServiceType.eCommerce;
                    shipmentCustomDetail.CatagoryOfItem = eCommerceBookingDetail.CustomInfo.CatagoryOfItem;
                    shipmentCustomDetail.CatagoryOfItemExplanation = eCommerceBookingDetail.CustomInfo.CatagoryOfItemExplanation;
                    shipmentCustomDetail.CustomsCertify = eCommerceBookingDetail.CustomInfo.CustomsCertify;
                    eCommerceBookingDetail.CustomInfo.TermOfTrade = eCommerceBookingDetail.PayTaxAndDuties;
                    shipmentCustomDetail.TermOfTrade = eCommerceBookingDetail.CustomInfo.TermOfTrade;
                    shipmentCustomDetail.ModuleType = eCommerceBookingDetail.CustomInfo.ModuleType;
                }
            }
            else
            {
                shipmentCustomDetail = new ShipmentCustomDetailDraft();
                shipmentCustomDetail.ShipmentDraftId = eCommerceBookingDetail.DirectShipmentDraftId;
                shipmentCustomDetail.ShipmentServiceType = FrayteShipmentServiceType.eCommerce;


                shipmentCustomDetail.ContentsType = eCommerceBookingDetail.CustomInfo.ContentsType;
                shipmentCustomDetail.ContentsExplanation = eCommerceBookingDetail.CustomInfo.ContentsExplanation;
                shipmentCustomDetail.RestrictionType = eCommerceBookingDetail.CustomInfo.RestrictionType;
                shipmentCustomDetail.RestrictionComments = eCommerceBookingDetail.CustomInfo.RestrictionComments;
                shipmentCustomDetail.CustomsCertify = eCommerceBookingDetail.CustomInfo.CustomsCertify;
                shipmentCustomDetail.CustomsSigner = eCommerceBookingDetail.CustomInfo.CustomsSigner;
                shipmentCustomDetail.NonDeliveryOption = eCommerceBookingDetail.CustomInfo.NonDeliveryOption;
                shipmentCustomDetail.EelPfc = eCommerceBookingDetail.CustomInfo.EelPfc;

                //ParcelHub Custom Details
                //shipmentCustomDetail.CatagoryOfItem = eCommerceBookingDetail.CustomInfo.CatagoryOfItem;
                //shipmentCustomDetail.CatagoryOfItemExplanation = eCommerceBookingDetail.CustomInfo.CatagoryOfItemExplanation;
                //shipmentCustomDetail.CustomsCertify = eCommerceBookingDetail.CustomInfo.CustomsCertify;
                //shipmentCustomDetail.TermOfTrade = eCommerceBookingDetail.CustomInfo.TermOfTrade;
                //eCommerceBookingDetail.CustomInfo.TermOfTrade = eCommerceBookingDetail.PayTaxAndDuties;
                //shipmentCustomDetail.TermOfTrade = eCommerceBookingDetail.CustomInfo.TermOfTrade;
                shipmentCustomDetail.ModuleType = eCommerceBookingDetail.CustomInfo.ModuleType;
                dbContext.ShipmentCustomDetailDrafts.Add(shipmentCustomDetail);
            }
            if (shipmentCustomDetail != null)
            {
                dbContext.SaveChanges();
            }
        }

        public void SaveAWBLabelName(FrayteCommercePackageTrackingDetail trackingDetail, string awbLabelName)
        {
            try
            {
                var tracking = dbContext.eCommercePackageTrackingDetails.Find(trackingDetail.eCommercePackageTrackingDetailId);
                if (tracking != null)
                {
                    tracking.FraytePackageImage = awbLabelName;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public FrayteDirectShipment GetShipmentDetail(int eCommerceShipmentId)
        {
            var result = (from DS in dbContext.eCommerceShipments
                          where DS.eCommerceShipmentId == eCommerceShipmentId
                          select new FrayteDirectShipment
                          {
                              DirectShipmentId = DS.eCommerceShipmentId,
                              FromAddressId = DS.FromAddressId,
                              ToAddressId = DS.ToAddressId,
                              ShipmentStatusId = DS.ShipmentStatusId
                          }).FirstOrDefault();

            return result;
        }
        private void SaveeCommerceShipmentDetailPackages(FrayteCommerceShipmentDraft eCommerceBookingDetail)
        {
            if (eCommerceBookingDetail.Packages != null && eCommerceBookingDetail.Packages.Count > 0)
            {
                foreach (PackageDraft package in eCommerceBookingDetail.Packages)

                {
                    var FHSCode = "";
                    var FindHsCode = dbContext.HSCodes.Where(p => p.Description.Contains(package.Content)).ToList();

                    foreach (var FHS in FindHsCode)
                    {
                        var fHS = FHS.Description.Split(',');
                        for (int i = 0; i < fHS.Length; i++)
                        {
                            if (fHS[i].ToLower() == package.Content.ToLower())
                            {
                                FHSCode = FHS.HSCode1;
                            }
                        }
                    }



                    DirectShipmentDetailDraft packageDetail;
                    if (package.DirectShipmentDetailDraftId > 0)
                    {
                        packageDetail = dbContext.DirectShipmentDetailDrafts.Find(package.DirectShipmentDetailDraftId);

                        if (packageDetail != null)
                        {
                            packageDetail.CartoonValue = package.CartoonValue;
                            packageDetail.DeclaredValue = package.Value;
                            packageDetail.DirectShipmentDraftId = eCommerceBookingDetail.DirectShipmentDraftId;
                            packageDetail.Height = package.Height;
                            packageDetail.Length = package.Length;
                            packageDetail.PiecesContent = package.Content;
                            packageDetail.Weight = package.Weight;
                            packageDetail.HSCode = package.HSCode;
                            packageDetail.HSCode = FHSCode != "" ? FHSCode : null;
                            packageDetail.Width = package.Width;
                        }
                    }
                    else
                    {
                        packageDetail = new DirectShipmentDetailDraft();
                        packageDetail.DeclaredValue = package.Value;
                        packageDetail.DirectShipmentDraftId = eCommerceBookingDetail.DirectShipmentDraftId;
                        packageDetail.CartoonValue = package.CartoonValue;
                        packageDetail.Height = package.Height;
                        packageDetail.Length = package.Length;
                        packageDetail.PiecesContent = package.Content;
                        packageDetail.Weight = package.Weight;
                        packageDetail.Width = package.Width;
                        packageDetail.HSCode = FHSCode != "" ? FHSCode : null;
                        dbContext.DirectShipmentDetailDrafts.Add(packageDetail);
                    }

                    if (packageDetail != null)
                    {
                        dbContext.SaveChanges();
                    }

                    package.DirectShipmentDetailDraftId = packageDetail.DirectShipmentDetailDraftId;
                }
            }
        }

        private void SaveeCommerceShipmnetDetail(FrayteCommerceShipmentDraft eCommerceBookingDetail)
        {
            DirectShipmentDraft dbDirectShipment;
            FrayteOperationZone operationZone = UtilityRepository.GetOperationZone();
            eCommerceBookingDetail.CreatedOn = DateTime.UtcNow;
            if (eCommerceBookingDetail.DirectShipmentDraftId == 0)
            {

                dbDirectShipment = new DirectShipmentDraft();
                dbDirectShipment.ShipmentStatusId = eCommerceBookingDetail.ShipmentStatusId;
                dbDirectShipment.CurrencyCode = eCommerceBookingDetail.Currency.CurrencyCode;

                //Set Reference Detail
                dbDirectShipment.Reference1 = eCommerceBookingDetail.ReferenceDetail.Reference1;
                dbDirectShipment.ContentDescription = eCommerceBookingDetail.ReferenceDetail.ContentDescription;
                dbDirectShipment.SpecialInstruction = eCommerceBookingDetail.ReferenceDetail.SpecialInstruction;
                //dbDirectShipment.CollectionDate = eCommerceBookingDetail.ReferenceDetail.CollectionDate;
                //dbDirectShipment.CollectionTime = UtilityRepository.GetTimeFromString(eCommerceBookingDetail.ReferenceDetail.CollectionTime);
                dbDirectShipment.CustomerId = eCommerceBookingDetail.CustomerId;
                dbDirectShipment.FromAddressId = eCommerceBookingDetail.ShipFrom.DirectShipmentAddressDraftId;
                dbDirectShipment.ToAddressId = eCommerceBookingDetail.ShipTo.DirectShipmentAddressDraftId;
                dbDirectShipment.IsPODMailSent = false;
                dbDirectShipment.OpearionZoneId = operationZone.OperationZoneId;
                dbDirectShipment.ParcelType = eCommerceBookingDetail.ParcelType.ParcelType;
                dbDirectShipment.PaymentPartyTaxAndDuties = eCommerceBookingDetail.PayTaxAndDuties;
                dbDirectShipment.CreatedBy = eCommerceBookingDetail.CreatedBy;
                dbDirectShipment.LastUpdated = DateTime.UtcNow;
                dbDirectShipment.PackageCaculatonType = eCommerceBookingDetail.PakageCalculatonType;
                dbDirectShipment.TaxAndDutiesAcceptedBy = eCommerceBookingDetail.TaxAndDutiesAcceptedBy;
                dbDirectShipment.ModuleType = eCommerceBookingDetail.ModuleType;
                dbDirectShipment.BookingApp = eCommerceBookingDetail.BookingApp;
                //string FrayteNo = CommonConversion.GetNewFrayteNumber();
                //if (FrayteNo != null || FrayteNo != "")
                //{
                //    dbDirectShipment.FrayteNumber = FrayteNo;
                //    eCommerceBookingDetail.FrayteNumber = FrayteNo;
                //}
                dbContext.DirectShipmentDrafts.Add(dbDirectShipment);
            }
            else
            {
                dbDirectShipment = dbContext.DirectShipmentDrafts.Find(eCommerceBookingDetail.DirectShipmentDraftId);
                if (dbDirectShipment != null)
                {
                    dbDirectShipment.ShipmentStatusId = eCommerceBookingDetail.ShipmentStatusId;
                    dbDirectShipment.CurrencyCode = eCommerceBookingDetail.Currency.CurrencyCode;

                    //Set Reference Detail
                    dbDirectShipment.Reference1 = eCommerceBookingDetail.ReferenceDetail.Reference1;
                    dbDirectShipment.ContentDescription = eCommerceBookingDetail.ReferenceDetail.ContentDescription;
                    dbDirectShipment.SpecialInstruction = eCommerceBookingDetail.ReferenceDetail.SpecialInstruction;
                    dbDirectShipment.CollectionDate = eCommerceBookingDetail.ReferenceDetail.CollectionDate;
                    dbDirectShipment.CollectionTime = UtilityRepository.GetTimeFromString(eCommerceBookingDetail.ReferenceDetail.CollectionTime);
                    dbDirectShipment.CustomerId = eCommerceBookingDetail.CustomerId;
                    dbDirectShipment.FromAddressId = eCommerceBookingDetail.ShipFrom.DirectShipmentAddressDraftId;
                    dbDirectShipment.ToAddressId = eCommerceBookingDetail.ShipTo.DirectShipmentAddressDraftId;
                    dbDirectShipment.IsPODMailSent = false;
                    dbDirectShipment.OpearionZoneId = operationZone.OperationZoneId;
                    dbDirectShipment.ParcelType = eCommerceBookingDetail.ParcelType.ParcelType;
                    dbDirectShipment.PaymentPartyTaxAndDuties = eCommerceBookingDetail.PayTaxAndDuties;
                    dbDirectShipment.TaxAndDutiesAcceptedBy = eCommerceBookingDetail.TaxAndDutiesAcceptedBy;
                    dbDirectShipment.CreatedBy = eCommerceBookingDetail.CreatedBy;
                    dbDirectShipment.LastUpdated = DateTime.UtcNow;
                    dbDirectShipment.PackageCaculatonType = eCommerceBookingDetail.PakageCalculatonType;
                    dbDirectShipment.ModuleType = eCommerceBookingDetail.ModuleType;
                    dbDirectShipment.BookingApp = eCommerceBookingDetail.BookingApp;
                    //string FrayteNo = CommonConversion.GetNewFrayteNumber();
                    //if (FrayteNo != null || FrayteNo != "")
                    //{
                    dbDirectShipment.FrayteNumber = null;
                    //    eCommerceBookingDetail.FrayteNumber = FrayteNo;
                    //}
                    dbContext.Entry(dbDirectShipment).State = System.Data.Entity.EntityState.Modified;
                }
            }

            if (dbDirectShipment != null)
            {
                dbContext.SaveChanges();
            }

            eCommerceBookingDetail.DirectShipmentDraftId = dbDirectShipment.DirectShipmentDraftId;

        }

        public void SaveFrayteLabel(int eCommerceShipmentId, string filename, string labelType)
        {
            try
            {
                var shipment = dbContext.eCommerceShipments.Find(eCommerceShipmentId);
                if (labelType == eCommLabelType.FrayteLabel)
                    shipment.FrayteLabel = filename;
                if (labelType == eCommLabelType.CourierLabel)
                    shipment.LogisticLabel = filename;
                dbContext.Entry(shipment).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        private void SaveeCommerceShipmentAddress(eCommerceShipmentAddressDraft address)
        {
            if (address.Country.CountryId > 0)
            {
                if (address.DirectShipmentAddressDraftId == 0)
                {
                    DirectShipmentAddressDraft dbAddress = new DirectShipmentAddressDraft();
                    dbAddress.CustomerId = address.CustomerId;
                    if (address.AddressType == FrayteFromToAddressType.FromAddress)
                    {
                        dbAddress.FromAddress = true;
                        dbAddress.ToAddress = false;
                    }
                    else if (address.AddressType == FrayteFromToAddressType.ToAddress)
                    {
                        dbAddress.FromAddress = false;
                        dbAddress.ToAddress = true;
                    }
                    dbAddress.Address1 = address.Address;
                    dbAddress.Address2 = address.Address2;
                    dbAddress.Area = address.Area;
                    dbAddress.City = address.City;
                    dbAddress.CompanyName = address.CompanyName;
                    dbAddress.ContactFirstName = address.FirstName;
                    dbAddress.ContactLastName = address.LastName;
                    dbAddress.CountryId = address.Country.CountryId;
                    dbAddress.Email = address.Email;
                    dbAddress.PhoneNo = address.Phone;
                    dbAddress.State = address.State;
                    dbAddress.Zip = address.PostCode;
                    dbAddress.IsActive = true;
                    dbAddress.TableType = FrayteTableType.AddressBook;
                    dbAddress.ModuleType = "eCommerce";
                    dbContext.DirectShipmentAddressDrafts.Add(dbAddress);

                    if (dbAddress != null)
                    {
                        dbContext.SaveChanges();
                    }
                    address.DirectShipmentAddressDraftId = dbAddress.DirectShipmentAddressDraftId;
                }
                else
                {
                    DirectShipmentAddressDraft dbAddress = dbContext.DirectShipmentAddressDrafts.Find(address.DirectShipmentAddressDraftId);
                    if (dbAddress != null)
                    {
                        dbAddress.FromAddress = dbAddress.ToAddress == true ? false : true;
                        dbAddress.ToAddress = dbAddress.FromAddress == true ? false : true;
                        dbAddress.CustomerId = address.CustomerId;
                        dbAddress.Address1 = address.Address;
                        dbAddress.Address2 = address.Address2;
                        dbAddress.Area = address.Area;
                        dbAddress.City = address.City;
                        dbAddress.CompanyName = address.CompanyName;
                        dbAddress.ContactFirstName = address.FirstName;
                        dbAddress.ContactLastName = address.LastName;
                        dbAddress.CountryId = address.Country.CountryId;
                        dbAddress.Email = address.Email;
                        dbAddress.PhoneNo = address.Phone;
                        dbAddress.State = address.State;
                        dbAddress.Zip = address.PostCode;
                        dbAddress.IsActive = true;
                        dbAddress.TableType = FrayteTableType.AddressBook;
                    }

                    if (dbAddress != null)
                    {
                        dbContext.SaveChanges();
                    }
                    address.DirectShipmentAddressDraftId = dbAddress.DirectShipmentAddressDraftId;
                }

            }
        }
        private void SetPostCode(FrayteCommerceShipmentDraft eCommerceBookingDetail)
        {
            //Step 1: Set ShipFrom PinCode
            if (eCommerceBookingDetail.ShipFrom != null)
            {
                eCommerceBookingDetail.ShipFrom.PostCode = UtilityRepository.PostCodeVerification(eCommerceBookingDetail.ShipFrom.PostCode, eCommerceBookingDetail.ShipFrom.Country.Code2);
            }
            //Step 2: Set ShipTo PinCode
            if (eCommerceBookingDetail.ShipTo != null)
            {
                eCommerceBookingDetail.ShipTo.PostCode = UtilityRepository.PostCodeVerification(eCommerceBookingDetail.ShipTo.PostCode, eCommerceBookingDetail.ShipTo.Country.Code2);
            }
        }
        #endregion
        #endregion

        #region getECommerceBooking Detail Page

        #region  View Shipment
        public FrayteeCommerceShipmentDetail GeteCommerceBookingDetail(int eCommerceShipmentId, string callingType)
        {
            FrayteeCommerceShipmentDetail dbDetail = new FrayteeCommerceShipmentDetail();

            dbDetail.eCommerceShipmentId = eCommerceShipmentId;

            //Step 1: Get Shipment Detail Draft
            GeteCommerceShipmnetDetail(dbDetail, callingType);

            //Step 2: Get Shipment Packages Draft Detail
            GeteCommerceShipmentPackagesDetail(dbDetail);

            //Step 3: Get Ship From and Ship To Detail Draft
            GeteCommerceShipmentCollectionDetail(dbDetail);

            // Step 3.1 : GetLogistic Service Info 
            GeteCommerceLogisticService(dbDetail);

            //Step 4: Get Custom Info Detail Draft
            //  GeteCommerceShipmentCustomDetail(dbDetail);

            //step 5 : Get Logistic Service Detail
            GeteCommerceServiceDetail(dbDetail, callingType);

            return dbDetail;

        }

        public eCommerceManualTracking SaveManualTracking(eCommerceManualTracking eCommerceManualTracking)
        {
            try
            {
                if (eCommerceManualTracking != null)
                {
                    eCommerceTracking tracking = new eCommerceTracking();
                    tracking.eCommerceShipmentId = eCommerceManualTracking.eCommerceShipmentId;
                    tracking.CreatedBy = eCommerceManualTracking.CreatedBy;
                    tracking.CreatedOnUtc = DateTime.UtcNow;
                    tracking.FrayteNumber = eCommerceManualTracking.FrayteNumber;
                    tracking.TrackingDescription = eCommerceManualTracking.TrackingDescription;
                    tracking.TrackingDescriptionCode = eCommerceManualTracking.TrackingDescriptionCode;
                    tracking.TrackingMode = eCommerceTrackingMode.ManualTracking;
                    tracking.TrackingNumber = eCommerceManualTracking.TrackingNumber;
                    dbContext.eCommerceTrackings.Add(tracking);
                    dbContext.SaveChanges();
                    eCommerceManualTracking.eCommerceTrackingId = tracking.eCommerceTrackingId;


                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return eCommerceManualTracking;
        }
        public List<eCommerceManualTracking> GetManualTracking(int shipmentId)
        {
            List<eCommerceManualTracking> list = new List<eCommerceManualTracking>();
            try
            {
                var collection = (from r in dbContext.eCommerceTrackings

                                  where r.eCommerceShipmentId == shipmentId
                                  select new
                                  {
                                      eCommerceShipmentId = r.eCommerceShipmentId,
                                      TrackingDescription = r.TrackingDescription,
                                      TrackingDescriptionCode = r.TrackingDescriptionCode,
                                      CreatedBy = r.CreatedBy,
                                      CreatedOn = r.CreatedOnUtc,
                                      FrayteNumber = r.FrayteNumber,
                                      eCommerceTrackingId = r.eCommerceTrackingId,
                                      TrackingNumber = r.TrackingNumber
                                  }
                  ).ToList();

                if (collection != null)
                {
                    eCommerceManualTracking track;
                    foreach (var item in collection)
                    {
                        track = new eCommerceManualTracking();
                        track.CreatedBy = item.CreatedBy;
                        track.CreatedOn = item.CreatedOn;
                        track.eCommerceShipmentId = item.eCommerceShipmentId;
                        track.eCommerceTrackingId = item.eCommerceTrackingId;
                        track.FrayteNumber = item.FrayteNumber;
                        track.TrackingDescription = item.TrackingDescription;
                        track.TrackingDescriptionCode = item.TrackingDescriptionCode;
                        track.TrackingNumber = item.TrackingNumber;

                        list.Add(track);
                    }
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return list;
        }
        #region Private methods getECommerceBooking Detail Page

        private void GeteCommerceLogisticService(FrayteeCommerceShipmentDetail dbDetail)
        {
            if (dbDetail.BookingApp == eCommerceShipmentType.eCommerceWS)
            {
                dbDetail.LogisticDetail = new eCommerceLogisticService();
                dbDetail.LogisticDetail.LogisticService = dbDetail.LogisticCompany;
                dbDetail.LogisticDetail.LogisticServiceDisplay = dbDetail.LogisticCompany;
            }
            else
            {
                var logisticDetail = dbContext.CountryLogistics.Where(p => p.CountryId == dbDetail.ShipTo.Country.CountryId).FirstOrDefault();

                if (logisticDetail != null)
                {
                    dbDetail.LogisticDetail = new eCommerceLogisticService();
                    dbDetail.LogisticDetail.AccountId = logisticDetail.AccountId;
                    dbDetail.LogisticDetail.AccountNo = logisticDetail.AccountNo;
                    dbDetail.LogisticDetail.CountryCode = logisticDetail.CountryCode;
                    dbDetail.LogisticDetail.CountryId = logisticDetail.CountryId;
                    dbDetail.LogisticDetail.CountryLogisticId = logisticDetail.CountryLogisticId;
                    dbDetail.LogisticDetail.Description = logisticDetail.Description;
                    dbDetail.LogisticDetail.LogisticService = logisticDetail.LogisticService;
                    dbDetail.LogisticDetail.LogisticServiceDisplay = logisticDetail.LogisicServiceDisplay;
                }
            }

        }

        public PrintLabel GeneratePacakgelabel(int eCommerceShipmentId, int id, string labelType)
        {
            PrintLabel file = new PrintLabel();
            var data = dbContext.eCommercePackageTrackingDetails.Find(id);
            if (data != null)
            {
                if (labelType == eCommLabelType.CourierLabel)
                {
                    file.FilePath = AppSettings.WebApiPath + "PackageLabel/eCommerce/" + eCommerceShipmentId.ToString() + "/" + data.PackageImage.Replace(".jpg", ".pdf");
                    file.FileName = data.PackageImage.Replace(".jpg", ".pdf");
                }
                else
                {
                    file.FilePath = AppSettings.WebApiPath + "PackageLabel/eCommerce/" + eCommerceShipmentId.ToString() + "/" + data.FraytePackageImage;
                    file.FileName = data.FraytePackageImage;
                }
            }




            return file;
        }

        public FrayteResult PrintAllLabels(int id, string labelType)
        {

            FrayteResult result = new FrayteResult();

            var collections = (from r in dbContext.eCommerceShipmentDetails
                               join ep in dbContext.eCommercePackageTrackingDetails on r.eCommerceShipmentDetailId equals ep.eCommerceShipmentDetailId
                               where r.eCommerceShipmentId == id
                               select ep
              ).ToList();

            return result;
        }

        public PrintLabel GetLabelFile(int id, string labelType)
        {
            PrintLabel file = new PrintLabel();
            var shipment = dbContext.eCommerceShipments.Find(id);
            if (labelType == eCommLabelType.CourierLabel)
            {
                file.FilePath = AppSettings.WebApiPath + "PackageLabel/eCommerce/" + id.ToString() + "/" + shipment.LogisticLabel;
                file.FileName = shipment.LogisticLabel;
            }
            else if (labelType == eCommLabelType.FrayteLabel)
            {
                file.FilePath = AppSettings.WebApiPath + "PackageLabel/eCommerce/" + id.ToString() + "/" + shipment.FrayteLabel;
                file.FileName = shipment.FrayteLabel;
            }

            return file;
        }

        private void GeteCommerceShipmentCollectionDetail(FrayteeCommerceShipmentDetail dbDetail)
        {
            var shipFrom = dbContext.eCommerceShipmentAddresses.Find(dbDetail.ShipFrom.eCommerceShipmentAddressId);
            if (shipFrom != null)
            {
                dbDetail.ShipFrom.Address = shipFrom.Address1;
                dbDetail.ShipFrom.Address2 = shipFrom.Address2;
                dbDetail.ShipFrom.CustomerId = dbDetail.CustomerId;
                dbDetail.ShipFrom.Area = shipFrom.Area;
                dbDetail.ShipFrom.City = shipFrom.City;
                dbDetail.ShipFrom.CompanyName = shipFrom.CompanyName;
                dbDetail.ShipFrom.FirstName = shipFrom.ContactFirstName;
                dbDetail.ShipFrom.LastName = shipFrom.ContactLastName;
                dbDetail.ShipFrom.Email = shipFrom.Email;
                dbDetail.ShipFrom.PostCode = shipFrom.Zip;

                dbDetail.ShipFrom.Country = new FrayteCountryCode();
                var country = dbContext.Countries.Find(shipFrom.CountryId);
                if (country != null)
                {
                    dbDetail.ShipFrom.Country.CountryId = country.CountryId;
                    dbDetail.ShipFrom.Country.Code = country.CountryCode;
                    dbDetail.ShipFrom.Country.Code2 = country.CountryCode2;
                    dbDetail.ShipFrom.Country.Name = country.CountryName;
                    dbDetail.ShipFrom.Phone = "( +" + country.CountryPhoneCode + " ) " + shipFrom.PhoneNo;
                }
                else
                {
                    dbDetail.ShipFrom.Phone = shipFrom.PhoneNo;
                }
            }
            var ShipTo = dbContext.eCommerceShipmentAddresses.Find(dbDetail.ShipTo.eCommerceShipmentAddressId);
            if (ShipTo != null)
            {
                dbDetail.ShipTo.Address = ShipTo.Address1;
                dbDetail.ShipTo.Address2 = ShipTo.Address2;
                dbDetail.ShipTo.CustomerId = dbDetail.CustomerId;
                dbDetail.ShipTo.Area = ShipTo.Area;
                dbDetail.ShipTo.City = ShipTo.City;
                dbDetail.ShipTo.CompanyName = ShipTo.CompanyName;
                dbDetail.ShipTo.FirstName = ShipTo.ContactFirstName;
                dbDetail.ShipTo.LastName = ShipTo.ContactLastName;
                dbDetail.ShipTo.Email = ShipTo.Email;
                dbDetail.ShipTo.PostCode = ShipTo.Zip;

                dbDetail.ShipTo.Country = new FrayteCountryCode();
                var country = dbContext.Countries.Find(ShipTo.CountryId);
                if (country != null)
                {
                    dbDetail.ShipTo.Country.CountryId = country.CountryId;
                    dbDetail.ShipTo.Country.Code = country.CountryCode;
                    dbDetail.ShipTo.Country.Code2 = country.CountryCode2;
                    dbDetail.ShipTo.Country.Name = country.CountryName;
                    dbDetail.ShipTo.Phone = "( +" + country.CountryPhoneCode + " ) " + ShipTo.PhoneNo;
                }
                else
                {
                    dbDetail.ShipTo.Phone = ShipTo.PhoneNo;
                }
            }
        }

        public object GetLabelDetail(int eCommerceShipmentId)
        {

            var data = dbContext.eCommerceShipments.Find(eCommerceShipmentId);
            return data;
        }

        private void GeteCommerceShipmentPackagesDetail(FrayteeCommerceShipmentDetail dbDetail)
        {
            #region
            dbDetail.Packages = new List<eCommercePackage>();

            var result = (from DS in dbContext.eCommerceShipments
                          join DSD in dbContext.eCommerceShipmentDetails on DS.eCommerceShipmentId equals DSD.eCommerceShipmentId
                          join PTD in dbContext.eCommercePackageTrackingDetails on DSD.eCommerceShipmentDetailId equals PTD.eCommerceShipmentDetailId
                          where DS.eCommerceShipmentId == dbDetail.eCommerceShipmentId
                          select new
                          {
                              PackageTrackingDetailId = PTD.eCommercePackageTrackingDetailId,
                              DirectShipmentDetailId = PTD.eCommerceShipmentDetailId,
                              IsPrinted = PTD.IsPrinted,
                              IsFrayteAWBPrinted = PTD.IsFRAYTEAWBPrinted,
                              FraytePackageImage = PTD.FraytePackageImage,
                              PackageImage = !string.IsNullOrEmpty(PTD.PackageImage) ? PTD.PackageImage.Replace(".jpg", ".pdf") : "",
                              PiecesContent = DSD.PiecesContent,
                              CartoonValue = DSD.CartoonValue,
                              HSCode = DSD.HSCode,
                              Height = DSD.Height,
                              Length = DSD.Length,
                              Width = DSD.Width,
                              Weight = DSD.Weight,
                              DeclaredValue = DSD.DeclaredValue,
                              UKDHLTrackingNo = DS.TrackingDetail,
                              EasyPostTrackingNo = PTD.TrackingNo
                          }).ToList();

            eCommercePackage dbPackage;
            if (result != null && result.Count > 0)
            {
                foreach (var Obj in result)
                {
                    dbPackage = new eCommercePackage();
                    dbPackage.Content = Obj.PiecesContent;
                    dbPackage.eCommerceShipmentDetailId = Obj.DirectShipmentDetailId;
                    dbPackage.eCommercePackageTrackingDetailId = Obj.PackageTrackingDetailId;
                    dbPackage.CartoonValue = Obj.CartoonValue / Obj.CartoonValue;
                    dbPackage.Height = Obj.Height;
                    dbPackage.Length = Obj.Length;
                    dbPackage.HSCode = Obj.HSCode;
                    if (Obj.DeclaredValue.HasValue)
                    {
                        dbPackage.Value = Obj.DeclaredValue.Value;
                    }
                    dbPackage.Weight = Obj.Weight;
                    dbPackage.Width = Obj.Width;
                    dbPackage.IsPrinted = Obj.IsPrinted;
                    dbPackage.IsFrayteAWBPrinted = Obj.IsFrayteAWBPrinted.HasValue ? Obj.IsFrayteAWBPrinted.Value : false;
                    dbPackage.TrackingNo = Obj.EasyPostTrackingNo;
                    dbPackage.LabelName = AppSettings.WebApiPath + "PackageLabel/eCommerce/" + dbDetail.eCommerceShipmentId + "/" + (!string.IsNullOrEmpty(Obj.PackageImage) ? Obj.PackageImage.Replace(".jpg", ".pdf") : "");
                    dbPackage.Label = !string.IsNullOrEmpty(Obj.PackageImage) ? Obj.PackageImage.Replace(".jpg", ".pdf") : "";
                    dbPackage.FrayteLabelName = AppSettings.WebApiPath + "PackageLabel/eCommerce/" + dbDetail.eCommerceShipmentId + "/" + Obj.FraytePackageImage;
                    dbPackage.FrayteLabel = Obj.FraytePackageImage;
                    dbDetail.Packages.Add(dbPackage);

                }
            }
            #endregion

            //var packageDetail = dbContext.eCommerceShipmentDetails.Where(p => p.eCommerceShipmentId == dbDetail.eCommerceShipmentId).ToList();
            //if (packageDetail != null && packageDetail.Count > 0)
            //{
            //    dbDetail.Packages = new List<eCommercePackage>();
            //    eCommercePackage package;
            //    foreach (var data in packageDetail)
            //    {
            //        package = new eCommercePackage();
            //        package.eCommerceShipmentDetailId = data.eCommerceShipmentDetailId;
            //        package.CartoonValue = data.CartoonValue;
            //        package.Content = data.PiecesContent;
            //        package.eCommercePackageTrackingDetailId = data.eCommerceShipmentDetailId;
            //        package.Weight = data.Weight;
            //        package.Length = data.Length;
            //        package.Width = data.Width;
            //        package.Height = data.Height;
            //        package.HSCode = data.HSCode;
            //        package.Value = data.DeclaredValue.HasValue ? data.DeclaredValue.Value : 0;
            //        var trackingDetail = dbContext.eCommercePackageTrackingDetails.Where(p => p.eCommerceShipmentDetailId == data.eCommerceShipmentDetailId).FirstOrDefault();
            //        if (trackingDetail != null)
            //        {
            //            package.eCommercePackageTrackingDetailId = trackingDetail.eCommercePackageTrackingDetailId;
            //            package.IsPrinted = trackingDetail.IsPrinted;
            //            package.TrackingNo = trackingDetail.TrackingNo;
            //            package.LabelName = AppSettings.WebApiPath + "PackageLabel/eCommerce/" + dbDetail.eCommerceShipmentId + "/" + trackingDetail.PackageImage.Replace(".jpg", ".pdf");
            //            package.FrayteLabelName = AppSettings.WebApiPath + "PackageLabel/eCommerce/" + dbDetail.eCommerceShipmentId + "/" + trackingDetail.FraytePackageImage;
            //        }
            //        dbDetail.Packages.Add(package);
            //    }
            //}
        }
        private void GeteCommerceShipmentCustomDetail(FrayteeCommerceShipmentDetail dbDetail)
        {
            dbDetail.CustomInfo = new CustomInformation();
            var customDetail = dbContext.eCommerceShipmentCustomDetails.Where(p => p.ShipmentId == dbDetail.eCommerceShipmentId).FirstOrDefault();
            if (customDetail != null)
            {
                dbDetail.CustomInfo.ContentsExplanation = customDetail.ContentsExplanation;
                dbDetail.CustomInfo.ContentsType = customDetail.ContentsType;
                dbDetail.CustomInfo.CustomsCertify = customDetail.CustomsCertify;
                dbDetail.CustomInfo.CustomsSigner = customDetail.CustomsSigner;
                dbDetail.CustomInfo.ShipmentId = customDetail.ShipmentId;
                dbDetail.CustomInfo.ShipmentCustomDetailId = customDetail.eCommerceShipmentCustomDetailId;
                dbDetail.CustomInfo.RestrictionComments = customDetail.RestrictionComments;
                dbDetail.CustomInfo.RestrictionType = customDetail.RestrictionType;
                dbDetail.CustomInfo.NonDeliveryOption = customDetail.NonDeliveryOption;
            }

        }
        private void GeteCommerceShipmnetDetail(FrayteeCommerceShipmentDetail dbDetail, string callingType)
        {
            var shipmentDetail = dbContext.eCommerceShipments.Find(dbDetail.eCommerceShipmentId);
            if (shipmentDetail != null)
            {
                dbDetail.CustomerId = shipmentDetail.CustomerId;
                dbDetail.Currency = new CurrencyType();
                dbDetail.Currency.CurrencyCode = shipmentDetail.CurrencyCode;

                dbDetail.DeliveryDate = shipmentDetail.DeliveryDate;
                dbDetail.DeliveryTime = shipmentDetail.DeliveryTime;
                dbDetail.FrayteNumber = shipmentDetail.FrayteNumber;
                dbDetail.OpearionZoneId = shipmentDetail.OpearionZoneId;
                dbDetail.ParcelType = new FrayteParcelType();
                dbDetail.ParcelType.ParcelType = shipmentDetail.ParcelType;
                dbDetail.PakageCalculatonType = shipmentDetail.PackageCaculatonType;
                dbDetail.PayTaxAndDuties = shipmentDetail.PaymentPartyTaxAndDuties;
                dbDetail.ReferenceDetail = new ReferenceDetail();
                dbDetail.ReferenceDetail.Reference1 = shipmentDetail.Reference1;
                dbDetail.ReferenceDetail.ContentDescription = shipmentDetail.ContentDescription;
                dbDetail.ShipmentStatusId = shipmentDetail.ShipmentStatusId;
                dbDetail.TaxAndDutiesAcceptedBy = shipmentDetail.TaxAndDutiesAcceptedBy;
                dbDetail.Warehouse = new eCommerceWareHouse();
                var warehouseDetail = dbContext.Warehouses.Where(p => p.WarehouseId == shipmentDetail.WarehouseId).FirstOrDefault();
                if (warehouseDetail != null)
                {
                    dbDetail.Warehouse.WarehouseId = warehouseDetail.WarehouseId;
                    dbDetail.Warehouse.Address = warehouseDetail.Address;
                    dbDetail.Warehouse.Address2 = warehouseDetail.Address2;
                    dbDetail.Warehouse.City = warehouseDetail.City;
                    dbDetail.Warehouse.State = warehouseDetail.State;
                    dbDetail.Warehouse.TelephoneNo = warehouseDetail.TelephoneNo;
                    dbDetail.Warehouse.Zip = warehouseDetail.Zip;
                    dbDetail.Warehouse.Email = warehouseDetail.Email;
                    dbDetail.Warehouse.Country = new FrayteCountryCode();
                    var country = dbContext.Countries.Find(dbDetail.Warehouse.WarehouseId);
                    if (country != null)
                    {
                        dbDetail.Warehouse.Country.Code = country.CountryCode;
                        dbDetail.Warehouse.Country.Code2 = country.CountryCode2;
                        dbDetail.Warehouse.Country.CountryId = country.CountryId;
                        dbDetail.Warehouse.Country.Name = country.CountryName;
                    }
                }
                dbDetail.ShipFrom = new FrayteeCommerceShipmentAddress();
                dbDetail.ShipFrom.eCommerceShipmentAddressId = shipmentDetail.FromAddressId;

                dbDetail.ShipTo = new FrayteeCommerceShipmentAddress();
                dbDetail.ShipTo.eCommerceShipmentAddressId = shipmentDetail.ToAddressId;
                dbDetail.CreatedOn = shipmentDetail.CreatedOn;
                var data = dbContext.Users.Find(shipmentDetail.CreatedBy);
                if (data != null)
                {
                    dbDetail.CreatedBy = data.ContactName + "-" + data.CompanyName;
                }
                dbDetail.ModuleType = FrayteShipmentServiceType.eCommerce;
                dbDetail.BookingApp = shipmentDetail.BookingApp;
                dbDetail.CourierCompany = shipmentDetail.LogisticServiceType;
                dbDetail.LogisticCompany = shipmentDetail.LogisticServiceType;

                dbDetail.EstimatedDateofDelivery = shipmentDetail.EstimatedDateofDelivery;
                dbDetail.EstimatedDateofArrival = shipmentDetail.EstimatedDateofArrival;
                dbDetail.EstimatedTimeofArrival = shipmentDetail.EstimatedTimeofArrival.HasValue ? UtilityRepository.GetFormattedTimeFromString(UtilityRepository.GetTimeZoneTime(shipmentDetail.EstimatedTimeofArrival)) : "";
                dbDetail.EstimatedTimeofDelivery = shipmentDetail.EstimatedTimeofDelivery.HasValue ? UtilityRepository.GetFormattedTimeFromString(UtilityRepository.GetTimeZoneTime(shipmentDetail.EstimatedTimeofDelivery)) : "";
            }
        }

        private void GeteCommerceServiceDetail(FrayteeCommerceShipmentDetail dbDetail, string callingType)
        {
            if (dbDetail.BookingApp == eCommerceShipmentType.eCommerceWS)
            {
                dbDetail.CourierCompany = dbDetail.LogisticCompany;
                dbDetail.CourierCompanyDisplay = dbDetail.LogisticCompany;
            }
            else
            {
                var shipppingMethod = dbContext.CountryLogistics.Where(p => p.CountryId == dbDetail.ShipTo.Country.CountryId).FirstOrDefault();
                if (shipppingMethod != null)
                {
                    dbDetail.CourierCompany = shipppingMethod.LogisticService;
                    dbDetail.CourierCompanyDisplay = shipppingMethod.LogisicServiceDisplay;
                }
            }

        }

        #endregion

        #endregion
        #region Invoice And Accounting
        public eCommerceInvoiceAccounting GetInVoiceAndAcounting(int userId, int shipmentId)
        {
            eCommerceInvoiceAccounting detail = new eCommerceInvoiceAccounting();

            var ships = (from r in dbContext.eCommerceShipmentDetails
                         where r.eCommerceShipmentId == shipmentId && string.IsNullOrEmpty(r.HSCode)
                         select r
               ).ToList();
            var shipment = dbContext.eCommerceShipments.Find(shipmentId);

            if ((shipment.EstimatedTimeofArrival.HasValue &&
                shipment.EstimatedDateofArrival.HasValue &&
                shipment.EstimatedDateofDelivery.HasValue &&
                shipment.EstimatedTimeofDelivery.HasValue) &&
                (ships == null || (ships != null && ships.Count == 0)))
            {
                detail.InvoiceReport = GeteCommerceInvoiceObj(shipmentId);
                // Fill extra data in invoice object
                GeteCommerceInvoceData(shipmentId, detail.InvoiceReport);
                // Get Invoive File Detail
                detail.InvoiceFile = GetInvoiceFileDetail(shipmentId);
            }

            detail.UserCreditNote = GetCreditNoteDetail(shipmentId, userId);



            // To Do : when we integrate payment then we need to send payment invoices like from paypal


            return detail;
        }

        private eCommerceFile GetInvoiceFileDetail(int shipmentId)
        {
            eCommerceFile file = new eCommerceFile();
            var invoice = dbContext.eCommerceInvoices.Where(p => p.ShipmentId == shipmentId).FirstOrDefault();
            if (invoice != null)
            {
                file.FileName = invoice.InvoiceFullName;
                file.FilePath = AppSettings.WebApiPath + "PackageLabel/eCommerce/" + shipmentId.ToString() + "/" + invoice.InvoiceFullName;
            }
            return file;
        }

        #region Private methods  Invoice And Accounting
        private List<eCommerceCreditNote> GetCreditNoteDetail(int shipmentId, int userId)
        {
            List<eCommerceCreditNote> list = new List<eCommerceCreditNote>();

            try
            {

                var userInfo = (from r in dbContext.Users
                                join tz in dbContext.Timezones on r.TimezoneId equals tz.TimezoneId
                                where r.UserId == userId
                                select tz
                            ).FirstOrDefault();

                var UserTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(userInfo.Name);

                var data = (from r in dbContext.eCommerceShipments
                            join da in dbContext.eCommerceShipmentAddresses on r.ToAddressId equals da.eCommerceShipmentAddressId
                            join cn in dbContext.eCommerceUserCreditNotes on da.Email equals cn.IssuedTo
                            //  join u in dbContext.Users on cn.IssuedBy equals u.UserId
                            where r.eCommerceShipmentId == shipmentId && cn.IssuedTo == da.Email && cn.UsedOnUtc == null
                            select new
                            {
                                eCommerceUserCreditNoteId = cn.eCommerceUserCreditNoteId,
                                Amount = cn.Amount.HasValue ? cn.Amount.Value : 0.0M,
                                CurrencyCode = cn.CurrencyCode,
                                CreditNoteRef = cn.CreditNoteReference,
                                //    IssuedBy = u.ContactName,
                                IssuedDate = cn.IssuedOnUtc,
                                IssuedTo = cn.IssuedTo,
                                Status = cn.Status
                            }
                    ).ToList();

                if (data != null)
                {
                    eCommerceCreditNote detail;
                    foreach (var item in data)
                    {
                        detail = new eCommerceCreditNote();
                        detail.eCommerceUserCreditNoteId = item.eCommerceUserCreditNoteId;
                        detail.Amount = item.Amount;
                        detail.CreditNoteRef = item.CreditNoteRef;
                        detail.CurrencyCode = item.CurrencyCode;
                        //   detail.IssuedBy = item.IssuedBy;
                        detail.IssuedDate = item.IssuedDate;
                        detail.IssuedDate = UtilityRepository.UtcDateToOtherTimezone(item.IssuedDate, item.IssuedDate.TimeOfDay, UserTimeZoneInfo).Item1;
                        detail.IssuedTime = UtilityRepository.GetFormattedTimeFromString(UtilityRepository.UtcDateToOtherTimezone(item.IssuedDate, item.IssuedDate.TimeOfDay, UserTimeZoneInfo).Item2);
                        detail.Status = item.Status;
                        detail.IssuedTo = item.IssuedTo;
                        list.Add(detail);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return list;
        }

        public void GeteCommerceInvoceData(int shipmentId, eCommerceTaxAndDutyInvoiceReport detail)
        {
            var invoiceData = dbContext.eCommerceInvoices.Where(p => p.ShipmentId == shipmentId).FirstOrDefault();
            if (invoiceData != null && detail != null)
            {
                detail.Status = invoiceData.Status;
                detail.DueDate = invoiceData.InvoiceDueDateUtc.HasValue ? invoiceData.InvoiceDueDateUtc.Value : DateTime.UtcNow;

            }
        }
        #endregion
        #endregion
        #region Communication
        public FrayteResult CreateCommunication(InVoiceCommunication inVoiceCommunication)
        {
            FrayteResult result = new FrayteResult();

            if (inVoiceCommunication != null)
            {
                eCommerceCommunication communication = new eCommerceCommunication();
                communication.ShipmentId = inVoiceCommunication.ShipmentId;
                communication.Description = inVoiceCommunication.Description;
                communication.CreatedBy = inVoiceCommunication.CreatedBy;
                communication.CreatedOnUtc = DateTime.UtcNow;
                communication.AccessType = inVoiceCommunication.AccessType;
                dbContext.eCommerceCommunications.Add(communication);
                dbContext.SaveChanges();
                result.Status = true;
            }
            return result;
        }

        public List<eCommerceInvoiceCommunication> GetCommunications(int userId, int shipmentId)
        {
            List<eCommerceInvoiceCommunication> list = new List<eCommerceInvoiceCommunication>();

            var userInfo = (from r in dbContext.Users
                            join tz in dbContext.Timezones on r.TimezoneId equals tz.TimezoneId
                            where r.UserId == userId
                            select tz
              ).FirstOrDefault();

            var UserTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(userInfo.Name);

            var details = (from r in dbContext.eCommerceCommunications
                           join u in dbContext.Users on r.CreatedBy equals u.UserId
                           join ur in dbContext.UserRoles on r.CreatedBy equals ur.UserId
                           join rn in dbContext.Roles on ur.RoleId equals rn.RoleId
                           where (r.CreatedBy == userId || r.AccessType == CommunicationAccessType.Public) && r.ShipmentId == shipmentId
                           select new
                           {
                               CreatedBy = u.ContactName,
                               CreatedRole = rn.RoleName,
                               CreatedRoleDisplay = rn.RoleDisplayName,
                               AccessType = r.AccessType,
                               CreatedOn = r.CreatedOnUtc,
                               Description = r.Description,
                               eCommerceCommunicationId = r.eCommerceCommunicationId,
                               ShipmentId = r.ShipmentId,
                           }
                           ).OrderByDescending(p => p.CreatedOn).ToList();


            if (details != null)
            {
                eCommerceInvoiceCommunication com;
                foreach (var data in details)
                {
                    com = new eCommerceInvoiceCommunication();
                    com.AccessType = data.AccessType;
                    com.CreatedBy = data.CreatedBy;
                    com.CreatedOn = UtilityRepository.UtcDateToOtherTimezone(data.CreatedOn, data.CreatedOn.TimeOfDay, UserTimeZoneInfo).Item1;
                    com.CreatedTime = UtilityRepository.GetFormattedTimeFromString(UtilityRepository.UtcDateToOtherTimezone(data.CreatedOn, data.CreatedOn.TimeOfDay, UserTimeZoneInfo).Item2);
                    com.Description = data.Description;
                    com.eCommerceCommunicationId = data.eCommerceCommunicationId;
                    com.ShipmentId = data.ShipmentId;
                    com.CreatedRoleDisplay = data.CreatedRoleDisplay;
                    com.CreatedRole = data.CreatedRole;
                    list.Add(com);
                }
            }
            return list;
        }

        public FrayteResult SaveEmailCommunication(InvoiceEmailCommunication emailCommunication)
        {
            FrayteResult result = new FrayteResult();
            try
            {

                var ToEmail = (from r in dbContext.eCommerceShipments
                               join sa in dbContext.eCommerceShipmentAddresses on r.ToAddressId equals sa.eCommerceShipmentAddressId
                               where r.eCommerceShipmentId == emailCommunication.ShipmentId
                               select sa.Email
                            ).FirstOrDefault();

                if (!string.IsNullOrEmpty(ToEmail))
                {
                    emailCommunication.SentTo = ToEmail;
                    bool status = new ShipmentEmailRepository().SendEmailCommunication(emailCommunication);
                    if (status)
                    {
                        eCommerceEmailCommuniation communication = new eCommerceEmailCommuniation();
                        if (emailCommunication != null)
                        {
                            communication.EmailSubject = emailCommunication.EmailSubject;
                            communication.EmailBody = emailCommunication.EmailBody;
                            communication.CC = emailCommunication.CC;
                            communication.BCC = emailCommunication.BCC;
                            communication.EmailSentOnUtc = DateTime.UtcNow;
                            communication.SentBy = emailCommunication.CreatedBy;
                            communication.ShipmentId = emailCommunication.ShipmentId;
                            communication.SentTo = emailCommunication.SentTo;
                            dbContext.eCommerceEmailCommuniations.Add(communication);
                            dbContext.SaveChanges();
                            result.Status = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
            }
            return result;
        }

        public List<UserEmailCommunication> GetEmailCommunication(int userId, int shipmentId)
        {
            List<UserEmailCommunication> list = new List<UserEmailCommunication>();
            var userInfo = (from r in dbContext.Users
                            join tz in dbContext.Timezones on r.TimezoneId equals tz.TimezoneId
                            where r.UserId == userId
                            select tz
              ).FirstOrDefault();

            var UserTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(userInfo.Name);

            var collection = (from r in dbContext.eCommerceEmailCommuniations
                              where r.SentBy == userId && r.ShipmentId == shipmentId
                              select new
                              {
                                  ShipmentId = r.ShipmentId,
                                  BCC = r.BCC,
                                  CC = r.CC,
                                  CreatedBy = r.SentBy,
                                  SentTo = r.SentTo,
                                  EmailBody = r.EmailBody,
                                  EmailSubject = r.EmailSubject,
                                  EmailSentOnDate = r.EmailSentOnUtc,
                                  eCommerceEmailCommunicationId = r.eCommerceEmailCommunicationId,
                              }).OrderByDescending(p => p.EmailSentOnDate).ToList();

            if (collection != null)
            {
                UserEmailCommunication emailComm;
                foreach (var item in collection)
                {
                    emailComm = new UserEmailCommunication();
                    emailComm.BCC = item.BCC;
                    emailComm.CC = item.CC;
                    emailComm.CreatedBy = item.CreatedBy;
                    emailComm.eCommerceEmailCommunicationId = item.eCommerceEmailCommunicationId;
                    emailComm.EmailBody = item.EmailBody;
                    emailComm.EmailSentOnDate = UtilityRepository.UtcDateToOtherTimezone(item.EmailSentOnDate, item.EmailSentOnDate.TimeOfDay, UserTimeZoneInfo).Item1;
                    emailComm.EmailSentTime = UtilityRepository.GetFormattedTimeFromString(UtilityRepository.UtcDateToOtherTimezone(item.EmailSentOnDate, item.EmailSentOnDate.TimeOfDay, UserTimeZoneInfo).Item2);
                    emailComm.EmailSubject = item.EmailSubject;
                    emailComm.SentTo = item.SentTo;
                    emailComm.ShipmentId = item.ShipmentId;
                    emailComm.CreatedBy = item.CreatedBy;
                    list.Add(emailComm);
                }
            }
            return list;
        }
        #endregion

        #region User credit Note
        public FrayteResult AddUserCreditNote(eCommerceInvoiceCreditNote creditNote)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var detail = (from r in dbContext.eCommerceShipments
                              join da in dbContext.eCommerceShipmentAddresses on r.ToAddressId equals da.eCommerceShipmentAddressId
                              where r.eCommerceShipmentId == creditNote.ShipmentId
                              select new
                              {
                                  ShipmentId = r.eCommerceShipmentId,
                                  IssuedTo = da.Email,
                                  Frayteumber = r.FrayteNumber
                              }
                              ).FirstOrDefault();



                if (creditNote != null && detail != null)
                {
                    eCommerceUserCreditNote userCreditNote = new eCommerceUserCreditNote();
                    userCreditNote.Amount = creditNote.Amount;
                    userCreditNote.CurrencyCode = creditNote.CurrencyCode;
                    userCreditNote.ShipmentId = detail.ShipmentId;
                    userCreditNote.CreditNoteReference = detail.Frayteumber;
                    userCreditNote.IssuedBy = creditNote.IssuedBy;
                    userCreditNote.IssuedOnUtc = DateTime.UtcNow;
                    userCreditNote.IssuedTo = detail.IssuedTo;
                    userCreditNote.Status = "Not Used";
                    dbContext.eCommerceUserCreditNotes.Add(userCreditNote);
                    dbContext.SaveChanges();
                    result.Status = true;
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return result;
        }
        #endregion
        #endregion

        #region Get eCommerceBooking Detail Clone,Return,Draft
        public FrayteCommerceShipmentDraft GeteCommerceBookingDetailDraft(int eCommerceShipmentDraftId, string callingType)
        {
            FrayteCommerceShipmentDraft dbDetail = new FrayteCommerceShipmentDraft();
            dbDetail.ShipFrom = new eCommerceShipmentAddressDraft();
            dbDetail.ShipTo = new eCommerceShipmentAddressDraft();

            dbDetail.DirectShipmentDraftId = eCommerceShipmentDraftId;

            //Step 1: Get Shipment Detail Draft
            GeteCommerceShipmnetDraftDetail(dbDetail, callingType);

            //Step 2: Get Shipment Packages Draft Detail
            GeteCommerceShipmentPackagesDraftDetail(dbDetail, callingType);

            //Step 3: Get Ship From and Ship To Detail Draft
            GeteCommerceShipmentCollectionDraftDetail(dbDetail, callingType);

            //Step 4: Get Custom Info Detail Draft
            //   GeteCommerceShipmentCustomDraftDetail(dbDetail, callingType);

            dbDetail.DirectShipmentDraftId = 0;
            dbDetail.ShipFrom.DirectShipmentAddressDraftId = 0;
            dbDetail.ShipTo.DirectShipmentAddressDraftId = 0;
            return dbDetail;

        }

        public FrayteCommerceShipmentDraft GeteCommerceWithServiceFormBookingDetailDraft(int eCommerceShipmentDraftId, string callingType)
        {
            FrayteCommerceShipmentDraft dbDetail = new FrayteCommerceShipmentDraft();
            dbDetail.ShipFrom = new eCommerceShipmentAddressDraft();
            dbDetail.ShipTo = new eCommerceShipmentAddressDraft();

            dbDetail.DirectShipmentDraftId = eCommerceShipmentDraftId;

            //Step 1: Get Shipment Detail Draft
            GeteCommerceShipmnetDraftDetail(dbDetail, callingType);

            //Step 2: Get Shipment Packages Draft Detail
            GeteCommerceShipmentPackagesDraftDetail(dbDetail, callingType);

            //Step 3: Get Ship From and Ship To Detail Draft
            GeteCommerceShipmentCollectionDraftDetail(dbDetail, callingType);

            //Step 4: Get Custom Info Detail Draft
            //   GeteCommerceShipmentCustomDraftDetail(dbDetail, callingType);

            return dbDetail;

        }
        #region Private Methods
        private void GeteCommerceShipmentCustomDraftDetail(FrayteCommerceShipmentDraft dbDetail, string callingType)
        {
            dbDetail.CustomInfo = new CustomInformation();
            if (callingType == FrayteCallingType.ShipmentClone || callingType == FrayteCallingType.ShipmentReturn)
            {
                // no need to send custom info 
            }
            else
            {
                ShipmentCustomDetailDraft customDetail = dbContext.ShipmentCustomDetailDrafts.Where(p => p.ShipmentDraftId == dbDetail.DirectShipmentDraftId && p.ShipmentServiceType == FrayteShipmentServiceType.eCommerce).FirstOrDefault();
                if (customDetail != null)
                {

                    dbDetail.CustomInfo.ShipmentId = dbDetail.DirectShipmentDraftId;
                    dbDetail.CustomInfo.ShipmentCustomDetailId = customDetail.ShipmentCustomDetailDraftId;
                    //Parcle Hub Details
                    dbDetail.CustomInfo.CatagoryOfItem = customDetail.CatagoryOfItem;
                    dbDetail.CustomInfo.CatagoryOfItemExplanation = customDetail.CatagoryOfItemExplanation;
                    dbDetail.CustomInfo.CommodityCode = customDetail.CommodityCode;
                    dbDetail.CustomInfo.TermOfTrade = customDetail.TermOfTrade;
                    dbDetail.CustomInfo.CustomsCertify = customDetail.CustomsCertify;
                    dbDetail.CustomInfo.TermOfTrade = dbDetail.PayTaxAndDuties;
                    dbDetail.CustomInfo.ModuleType = customDetail.ModuleType;
                }
            }
        }
        private void GeteCommerceShipmentCollectionDraftDetail(FrayteCommerceShipmentDraft dbDetail, string callingType)
        {
            if (callingType == FrayteCallingType.ShipmentClone || callingType == FrayteCallingType.ShipmentReturn)
            {
                //Ship From
                eCommerceShipmentAddress shipFrom = dbContext.eCommerceShipmentAddresses.Find(dbDetail.ShipFrom.DirectShipmentAddressDraftId);
                if (shipFrom != null)
                {
                    dbDetail.ShipFrom.Address = shipFrom.Address1;
                    dbDetail.ShipFrom.Address2 = shipFrom.Address2;
                    dbDetail.ShipFrom.Area = shipFrom.Area;
                    dbDetail.ShipFrom.City = shipFrom.City;
                    dbDetail.ShipFrom.CompanyName = shipFrom.CompanyName;

                    dbDetail.ShipFrom.Country = new FrayteCountryCode();
                    var country = dbContext.Countries.Where(p => p.CountryId == shipFrom.CountryId).FirstOrDefault();
                    if (country != null)
                    {
                        dbDetail.ShipFrom.Country.CountryId = country.CountryId;
                        dbDetail.ShipFrom.Country.Code = country.CountryCode;
                        dbDetail.ShipFrom.Country.Code2 = country.CountryCode2;
                        dbDetail.ShipFrom.Country.Name = country.CountryName;
                    }

                    dbDetail.ShipFrom.Email = shipFrom.Email;
                    dbDetail.ShipFrom.FirstName = shipFrom.ContactFirstName;
                    dbDetail.ShipFrom.LastName = shipFrom.ContactLastName;
                    dbDetail.ShipFrom.Phone = shipFrom.PhoneNo;
                    dbDetail.ShipFrom.PostCode = shipFrom.Zip;
                    dbDetail.ShipFrom.State = shipFrom.State;
                }

                //Ship To
                eCommerceShipmentAddress shipTo = dbContext.eCommerceShipmentAddresses.Find(dbDetail.ShipTo.DirectShipmentAddressDraftId);
                if (shipTo != null)
                {
                    dbDetail.ShipTo.Address = shipTo.Address1;
                    dbDetail.ShipTo.Address2 = shipTo.Address2;
                    dbDetail.ShipTo.Area = shipTo.Area;
                    dbDetail.ShipTo.City = shipTo.City;
                    dbDetail.ShipTo.CompanyName = shipTo.CompanyName;

                    dbDetail.ShipTo.Country = new FrayteCountryCode();
                    var country = dbContext.Countries.Where(p => p.CountryId == shipTo.CountryId).FirstOrDefault();
                    if (country != null)
                    {
                        dbDetail.ShipTo.Country.CountryId = country.CountryId;
                        dbDetail.ShipTo.Country.Code = country.CountryCode;
                        dbDetail.ShipTo.Country.Code2 = country.CountryCode2;
                        dbDetail.ShipTo.Country.Name = country.CountryName;
                    }

                    dbDetail.ShipTo.Email = shipTo.Email;
                    dbDetail.ShipTo.FirstName = shipTo.ContactFirstName;
                    dbDetail.ShipTo.LastName = shipTo.ContactLastName;
                    dbDetail.ShipTo.Phone = shipTo.PhoneNo;
                    dbDetail.ShipTo.PostCode = shipTo.Zip;
                    dbDetail.ShipTo.State = shipTo.State;
                }
            }
            else if (callingType == FrayteCallingType.ShipmentDraft)
            {
                //Ship From
                DirectShipmentAddressDraft shipFrom = dbContext.DirectShipmentAddressDrafts.Find(dbDetail.ShipFrom.DirectShipmentAddressDraftId);
                if (shipFrom != null)
                {
                    dbDetail.ShipFrom.CustomerId = shipFrom.CustomerId;
                    var CustomerName = dbContext.Users.Where(p => p.UserId == shipFrom.CustomerId).FirstOrDefault();
                    if (CustomerName != null)
                    {
                        dbDetail.ShipFrom.CustomerName = CustomerName.ContactName;
                    }
                    dbDetail.ShipFrom.Address = shipFrom.Address1;
                    dbDetail.ShipFrom.Address2 = shipFrom.Address2;
                    dbDetail.ShipFrom.Area = shipFrom.Area;
                    dbDetail.ShipFrom.City = shipFrom.City;
                    dbDetail.ShipFrom.CompanyName = shipFrom.CompanyName;

                    dbDetail.ShipFrom.Country = new FrayteCountryCode();
                    var country = dbContext.Countries.Where(p => p.CountryId == shipFrom.CountryId).FirstOrDefault();
                    if (country != null)
                    {
                        dbDetail.ShipFrom.Country.CountryId = country.CountryId;
                        dbDetail.ShipFrom.Country.Code = country.CountryCode;
                        dbDetail.ShipFrom.Country.Code2 = country.CountryCode2;
                        dbDetail.ShipFrom.Country.Name = country.CountryName;
                    }

                    dbDetail.ShipFrom.Email = shipFrom.Email;
                    dbDetail.ShipFrom.FirstName = shipFrom.ContactFirstName;
                    dbDetail.ShipFrom.LastName = shipFrom.ContactLastName;
                    dbDetail.ShipFrom.Phone = shipFrom.PhoneNo;
                    dbDetail.ShipFrom.PostCode = shipFrom.Zip;
                    dbDetail.ShipFrom.State = shipFrom.State;
                    dbDetail.ShipFrom.AddressType = shipFrom.TableType;
                    dbDetail.ShipFrom.ModuleType = shipFrom.ModuleType;
                }

                //Ship To
                DirectShipmentAddressDraft shipTo = dbContext.DirectShipmentAddressDrafts.Find(dbDetail.ShipTo.DirectShipmentAddressDraftId);
                if (shipTo != null)
                {
                    dbDetail.ShipTo.CustomerId = shipFrom.CustomerId;
                    var CustomerName = dbContext.Users.Where(p => p.UserId == shipTo.CustomerId).FirstOrDefault();
                    if (CustomerName != null)
                    {
                        dbDetail.ShipTo.CustomerName = CustomerName.ContactName;
                    }
                    dbDetail.ShipTo.Address = shipTo.Address1;
                    dbDetail.ShipTo.Address2 = shipTo.Address2;
                    dbDetail.ShipTo.Area = shipTo.Area;
                    dbDetail.ShipTo.City = shipTo.City;
                    dbDetail.ShipTo.CompanyName = shipTo.CompanyName;

                    dbDetail.ShipTo.Country = new FrayteCountryCode();
                    var country = dbContext.Countries.Where(p => p.CountryId == shipTo.CountryId).FirstOrDefault();
                    if (country != null)
                    {
                        dbDetail.ShipTo.Country.CountryId = country.CountryId;
                        dbDetail.ShipTo.Country.Code = country.CountryCode;
                        dbDetail.ShipTo.Country.Code2 = country.CountryCode2;
                        dbDetail.ShipTo.Country.Name = country.CountryName;
                    }

                    dbDetail.ShipTo.Email = shipTo.Email;
                    dbDetail.ShipTo.FirstName = shipTo.ContactFirstName;
                    dbDetail.ShipTo.LastName = shipTo.ContactLastName;
                    dbDetail.ShipTo.Phone = shipTo.PhoneNo;
                    dbDetail.ShipTo.PostCode = shipTo.Zip;
                    dbDetail.ShipTo.State = shipTo.State;
                    dbDetail.ShipTo.AddressType = shipTo.TableType;
                    dbDetail.ShipTo.ModuleType = shipTo.ModuleType;
                }
            }
        }


        public List<FraytePackageLabel> GetPackageList(int eCommerceShipmentId)
        {
            List<FraytePackageLabel> _package = new List<FraytePackageLabel>();

            var item = (from PTD in dbContext.eCommercePackageTrackingDetails
                        join DSD in dbContext.eCommerceShipmentDetails on PTD.eCommerceShipmentDetailId equals DSD.eCommerceShipmentDetailId
                        where DSD.eCommerceShipmentId == eCommerceShipmentId
                        select new
                        {
                            ImageName = PTD.PackageImage
                        }).FirstOrDefault();

            if (item != null)
            {
                FraytePackageLabel pl;

                pl = new FraytePackageLabel();
                pl.LabelPath = HostingEnvironment.MapPath("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/" + item.ImageName);
                _package.Add(pl);
            }
            return _package;
        }
        public string GetTrackingNo(int eCommerceShipmentId)
        {
            string TrackingNo = string.Empty;
            if (eCommerceShipmentId > 0)
            {
                var Obj = dbContext.eCommerceShipments.Find(eCommerceShipmentId);
                if (Obj != null)
                {
                    if (Obj.TrackingDetail.Contains("order"))
                    {
                        var track = (from DS in dbContext.eCommerceShipments
                                     join DSD in dbContext.eCommerceShipmentDetails on DS.eCommerceShipmentId equals DSD.eCommerceShipmentId
                                     join PTD in dbContext.eCommercePackageTrackingDetails on DSD.eCommerceShipmentDetailId equals PTD.eCommerceShipmentDetailId
                                     where DS.eCommerceShipmentId == eCommerceShipmentId
                                     select new
                                     {
                                         PTD.TrackingNo
                                     }).FirstOrDefault();

                        if (track != null)
                        {
                            TrackingNo = track.TrackingNo;
                        }
                    }
                    else
                    {
                        TrackingNo = Obj.TrackingDetail;
                    }
                }
            }
            return TrackingNo;
        }
        public eCommerceWareHouse getWareHouseDetail(int countryId)
        {
            var data = (from r in dbContext.Warehouses
                        join c in dbContext.Countries on r.CountryId equals c.CountryId
                        where r.CountryId == countryId
                        select new eCommerceWareHouse
                        {
                            Address = r.Address,
                            Address2 = r.Address2,
                            City = r.City,
                            State = r.State,
                            Email = r.Email,
                            TelephoneNo = r.TelephoneNo,
                            WarehouseId = r.WarehouseId,
                            Zip = r.Zip,
                            Country = new FrayteCountryCode()
                            {
                                Code = c.CountryCode,
                                Code2 = c.CountryCode2,
                                CountryId = c.CountryId,
                                Name = c.CountryName,
                            }
                        }
                     ).FirstOrDefault();

            return data;
        }
        public FrayteResult SetPrintPackageStatus(int eCommercePackageTrackingDetailId, string Type)
        {
            FrayteResult result = new FrayteResult();


            var packageDeatil = dbContext.eCommercePackageTrackingDetails.Find(eCommercePackageTrackingDetailId);
            if (packageDeatil != null)
            {
                if (Type == eCommLabelType.CourierLabel)
                    packageDeatil.IsPrinted = true;
                else if (Type == eCommLabelType.FrayteLabel)
                    packageDeatil.IsFRAYTEAWBPrinted = true;

                dbContext.SaveChanges();
                result.Status = true;
            }
            else
            {
                result.Status = false;
            }


            return result;
        }
        private void GeteCommerceShipmentPackagesDraftDetail(FrayteCommerceShipmentDraft dbDetail, string callingType)
        {
            PackageDraft dbPackage;
            dbDetail.Packages = new List<PackageDraft>();

            List<PackageDraft> result = new List<PackageDraft>();

            if (callingType == FrayteCallingType.ShipmentClone || callingType == FrayteCallingType.ShipmentReturn)
            {
                result = (from eCS in dbContext.eCommerceShipments
                          join eCSD in dbContext.eCommerceShipmentDetails on eCS.eCommerceShipmentId equals eCSD.eCommerceShipmentId
                          where eCS.eCommerceShipmentId == dbDetail.DirectShipmentDraftId
                          select new PackageDraft
                          {
                              DirectShipmentDetailDraftId = 0,
                              Content = eCSD.PiecesContent,
                              CartoonValue = eCSD.CartoonValue,
                              Height = eCSD.Height,
                              Length = eCSD.Length,
                              Width = eCSD.Width,
                              Weight = eCSD.Weight,
                              HSCode = eCSD.HSCode,
                              Value = eCSD.DeclaredValue.Value
                          }).ToList();
            }
            else if (callingType == FrayteCallingType.ShipmentDraft)
            {
                result = (from eCS in dbContext.DirectShipmentDrafts
                          join eCSD in dbContext.DirectShipmentDetailDrafts on eCS.DirectShipmentDraftId equals eCSD.DirectShipmentDraftId
                          where eCS.DirectShipmentDraftId == dbDetail.DirectShipmentDraftId
                          select new PackageDraft
                          {
                              DirectShipmentDetailDraftId = 0,
                              Content = eCSD.PiecesContent,
                              CartoonValue = eCSD.CartoonValue.Value,
                              Height = eCSD.Height.Value,
                              Length = eCSD.Length.Value,
                              Width = eCSD.Width.Value,
                              Weight = eCSD.Weight.Value,
                              HSCode = eCSD.HSCode,
                              Value = eCSD.DeclaredValue.Value
                          }).ToList();

            }
            if (result != null && result.Count > 0)
            {
                foreach (var data in result)
                {
                    dbDetail.Packages.Add(data);
                }
            }
            else
            {
                dbPackage = new PackageDraft();
                dbPackage.Content = "";
                dbPackage.DirectShipmentDetailDraftId = 0;
                dbPackage.PackageTrackingDetailId = 0;
                dbPackage.CartoonValue = 0;
                dbPackage.Height = 0;
                dbPackage.Length = 0;
                dbPackage.Value = 0;
                dbPackage.Weight = 0;
                dbPackage.Width = 0;
                dbPackage.HSCode = "";
                dbPackage.IsPrinted = false;
                dbPackage.TrackingNo = "";
                dbPackage.LabelName = "";
                dbDetail.Packages.Add(dbPackage);
            }



        }
        private void GeteCommerceShipmnetDraftDetail(FrayteCommerceShipmentDraft dbDetail, string callingType)
        {
            if (callingType == FrayteCallingType.ShipmentClone || callingType == FrayteCallingType.ShipmentReturn)
            {
                var eCommerceShipmentDetail = dbContext.eCommerceShipments.Find(dbDetail.DirectShipmentDraftId);
                if (eCommerceShipmentDetail != null)
                {
                    dbDetail.OpearionZoneId = eCommerceShipmentDetail.OpearionZoneId;
                    dbDetail.ShipmentStatusId = (int)FrayteeCommerceShipmentStatus.Draft;
                    dbDetail.CustomerId = eCommerceShipmentDetail.CustomerId;
                    dbDetail.TrackingCode = eCommerceShipmentDetail.TrackingDetail;
                    if (eCommerceShipmentDetail.ManifestId.HasValue)
                    {
                        dbDetail.ManifestId = eCommerceShipmentDetail.ManifestId.Value;
                    }
                    else
                    {
                        dbDetail.ManifestId = 0;
                    }

                    dbDetail.ShipFrom = new eCommerceShipmentAddressDraft();
                    dbDetail.ShipFrom.DirectShipmentAddressDraftId = eCommerceShipmentDetail.FromAddressId;

                    dbDetail.ShipTo = new eCommerceShipmentAddressDraft();
                    dbDetail.ShipTo.DirectShipmentAddressDraftId = eCommerceShipmentDetail.ToAddressId;
                    dbDetail.FrayteNumber = eCommerceShipmentDetail.FrayteNumber;
                    dbDetail.PayTaxAndDuties = eCommerceShipmentDetail.PaymentPartyTaxAndDuties;
                    dbDetail.PakageCalculatonType = eCommerceShipmentDetail.PackageCaculatonType;
                    dbDetail.ParcelType = new FrayteParcelType();

                    dbDetail.ParcelType.ParcelDescription = eCommerceShipmentDetail.ParcelType;
                    dbDetail.ParcelType.ParcelType = eCommerceShipmentDetail.ParcelType;

                    dbDetail.Currency = new CurrencyType();
                    dbDetail.Currency.CurrencyCode = eCommerceShipmentDetail.CurrencyCode;

                    dbDetail.CreatedBy = eCommerceShipmentDetail.CreatedBy;
                    dbDetail.WareHouseId = eCommerceShipmentDetail.WarehouseId;

                    dbDetail.ReferenceDetail = new ReferenceDetail();
                    dbDetail.ReferenceDetail.Reference1 = eCommerceShipmentDetail.Reference1;
                    dbDetail.ReferenceDetail.ContentDescription = eCommerceShipmentDetail.ContentDescription;
                    dbDetail.ModuleType = "eCommerce";
                    dbDetail.BookingApp = eCommerceShipmentDetail.BookingApp;

                }
            }
            else if (callingType == FrayteCallingType.ShipmentDraft)
            {
                var eCommerceShipmentDraftDetail = dbContext.DirectShipmentDrafts.Find(dbDetail.DirectShipmentDraftId);
                if (eCommerceShipmentDraftDetail != null)
                {
                    if (eCommerceShipmentDraftDetail.OpearionZoneId.HasValue)
                    {
                        dbDetail.OpearionZoneId = eCommerceShipmentDraftDetail.OpearionZoneId.Value;
                    }
                    if (eCommerceShipmentDraftDetail.ShipmentStatusId.HasValue)
                    {
                        dbDetail.ShipmentStatusId = eCommerceShipmentDraftDetail.ShipmentStatusId.Value;
                    }
                    //  if(eCommerceShipmentDraftDetail.CustomerId.)
                    dbDetail.CustomerId = eCommerceShipmentDraftDetail.CustomerId;

                    dbDetail.ShipFrom = new eCommerceShipmentAddressDraft();
                    if (eCommerceShipmentDraftDetail.FromAddressId.HasValue)
                    {
                        dbDetail.ShipFrom.DirectShipmentAddressDraftId = eCommerceShipmentDraftDetail.FromAddressId.Value;
                    }

                    dbDetail.ShipTo = new eCommerceShipmentAddressDraft();
                    if (eCommerceShipmentDraftDetail.ToAddressId.HasValue)
                    {
                        dbDetail.ShipTo.DirectShipmentAddressDraftId = eCommerceShipmentDraftDetail.ToAddressId.Value;
                    }


                    dbDetail.FrayteNumber = eCommerceShipmentDraftDetail.FrayteNumber;
                    dbDetail.PayTaxAndDuties = eCommerceShipmentDraftDetail.PaymentPartyTaxAndDuties;
                    dbDetail.PakageCalculatonType = eCommerceShipmentDraftDetail.PackageCaculatonType;
                    dbDetail.ParcelType = new FrayteParcelType();

                    dbDetail.ParcelType.ParcelDescription = eCommerceShipmentDraftDetail.ParcelType;
                    dbDetail.ParcelType.ParcelType = eCommerceShipmentDraftDetail.ParcelType;
                    dbDetail.Currency = new CurrencyType();
                    dbDetail.Currency.CurrencyCode = eCommerceShipmentDraftDetail.CurrencyCode;
                    if (eCommerceShipmentDraftDetail.CreatedBy.HasValue)
                    {
                        dbDetail.CreatedBy = eCommerceShipmentDraftDetail.CreatedBy.Value;
                    }

                    dbDetail.ReferenceDetail = new ReferenceDetail();
                    dbDetail.ReferenceDetail.Reference1 = eCommerceShipmentDraftDetail.Reference1;
                    dbDetail.ReferenceDetail.ContentDescription = eCommerceShipmentDraftDetail.ContentDescription;
                    dbDetail.ModuleType = eCommerceShipmentDraftDetail.ModuleType;
                    dbDetail.BookingApp = eCommerceShipmentDraftDetail.BookingApp;
                }
            }
        }
        #endregion
        #endregion

        #region Save & Update After Integration
        public int SaveOrderNumber(FrayteCommerceShipmentDraft dbDetail, string orderId, int CustomerId)
        {
            int eCommerceShipmentId = 0;
            if (orderId != null || orderId != "")
            {

                var HsCode = dbContext.HSCodes.Where(a => a.Description == dbDetail.ReferenceDetail.ContentDescription).FirstOrDefault();
                //Step 1.0 Save Draft Data Into Direct Shipment Table                
                dbContext.spGet_SaveDraftAsDirectShipment(dbDetail.DirectShipmentDraftId, orderId, dbDetail.ModuleType, dbDetail.WareHouseId, DateTime.UtcNow, (int)FrayteeCommerceShipmentStatus.Current, CustomerId, null, null, null, CommonConversion.GetNewFrayteNumber(), null, null);

                //Step 1.1 Save Tracking No in DirectShipment Table
                eCommerceShipment dbShipment = dbContext.eCommerceShipments.Where(p => p.TrackingDetail == orderId).FirstOrDefault();
                eCommerceShipmentId = dbShipment.eCommerceShipmentId;

                //Step 1.2 Save Direct Shipment Address from DirectShipmentAddressDraft
                eCommerceShipmentAddress address;
                if (dbDetail.ShipFrom != null)
                {
                    address = new eCommerceShipmentAddress();
                    address.ContactFirstName = dbDetail.ShipFrom.FirstName;
                    address.ContactLastName = dbDetail.ShipFrom.LastName;
                    address.CompanyName = dbDetail.ShipFrom.CompanyName;
                    address.Email = dbDetail.ShipFrom.Email;
                    address.PhoneNo = dbDetail.ShipFrom.Phone;
                    address.Address1 = dbDetail.ShipFrom.Address;
                    address.Area = dbDetail.ShipFrom.Area;
                    address.Address2 = dbDetail.ShipFrom.Address2;
                    address.City = dbDetail.ShipFrom.City;
                    address.State = dbDetail.ShipFrom.State;
                    address.Zip = dbDetail.ShipFrom.PostCode;
                    address.CountryId = dbDetail.ShipFrom.Country.CountryId;
                    address.IsActive = true;
                    address.TableType = FrayteTableType.DirectBooking;
                    dbContext.eCommerceShipmentAddresses.Add(address);
                    if (address != null)
                    {
                        dbContext.SaveChanges();
                    }

                    //Update eCommerce Shipment FromAddresId
                    if (dbShipment != null)
                    {
                        dbShipment.FromAddressId = address.eCommerceShipmentAddressId;
                        dbContext.SaveChanges();
                    }

                }
                if (dbDetail.ShipTo != null)
                {
                    address = new eCommerceShipmentAddress();
                    address.ContactFirstName = dbDetail.ShipTo.FirstName;
                    address.ContactLastName = dbDetail.ShipTo.LastName;
                    address.CompanyName = dbDetail.ShipTo.CompanyName;
                    address.Email = dbDetail.ShipTo.Email;
                    address.PhoneNo = dbDetail.ShipTo.Phone;
                    address.Address1 = dbDetail.ShipTo.Address;
                    address.Area = dbDetail.ShipTo.Area;
                    address.Address2 = dbDetail.ShipTo.Address2;
                    address.City = dbDetail.ShipTo.City;
                    address.State = dbDetail.ShipTo.State;
                    address.Zip = dbDetail.ShipTo.PostCode;
                    address.CountryId = dbDetail.ShipTo.Country.CountryId;
                    address.IsActive = true;
                    address.TableType = FrayteTableType.DirectBooking;
                    dbContext.eCommerceShipmentAddresses.Add(address);

                    if (address != null)
                    {
                        dbContext.SaveChanges();
                    }

                    //Update eCommerceShipment ToAddressId
                    if (dbShipment != null)
                    {
                        dbShipment.ToAddressId = address.eCommerceShipmentAddressId;
                        dbContext.SaveChanges();
                    }
                }

                // Step 1.3 : Delete record from DirectShipmentAddressDraft

                var fromResult = dbContext.DirectShipmentAddressDrafts.Find(dbDetail.ShipFrom.DirectShipmentAddressDraftId);
                if (fromResult != null)
                {
                    dbContext.DirectShipmentAddressDrafts.Remove(fromResult);
                    dbContext.SaveChanges();
                }
                var toResult = dbContext.DirectShipmentAddressDrafts.Find(dbDetail.ShipTo.DirectShipmentAddressDraftId);
                if (toResult != null)
                {
                    dbContext.DirectShipmentAddressDrafts.Remove(toResult);
                    dbContext.SaveChanges();
                }


                // Step 1.4 : Add address to addressBook if not exist aleady

                var addressBookData = dbContext.AddressBooks.Where(p => p.Address1 == dbDetail.ShipFrom.Address &&
                                                                        p.Address2 == dbDetail.ShipFrom.Address2 &&
                                                                        p.City == dbDetail.ShipFrom.City &&
                                                                        p.State == dbDetail.ShipFrom.State &&
                                                                        p.PhoneNo == dbDetail.ShipFrom.Phone &&
                                                                        p.Area == dbDetail.ShipFrom.Area &&
                                                                        p.CompanyName == dbDetail.ShipFrom.CompanyName &&
                                                                        p.ContactFirstName == dbDetail.ShipFrom.FirstName &&
                                                                        p.ContactLastName == dbDetail.ShipFrom.LastName &&
                                                                        p.CountryId == dbDetail.ShipFrom.Country.CountryId &&
                                                                        p.CustomerId == dbDetail.ShipFrom.CustomerId &&
                                                                        p.Email == dbDetail.ShipFrom.Email &&
                                                                        p.Zip == dbDetail.ShipFrom.PostCode &&
                                                                        p.IsActive == true &&
                                                                        p.FromAddress == true).ToList();
                if (addressBookData != null && addressBookData.Count > 0)
                {
                }
                else
                {
                    AddressBook dbToAddressBook = new AddressBook();
                    dbToAddressBook.Address1 = dbDetail.ShipFrom.Address;
                    dbToAddressBook.Address2 = dbDetail.ShipFrom.Address2;
                    dbToAddressBook.City = dbDetail.ShipFrom.City;
                    dbToAddressBook.State = dbDetail.ShipFrom.State;
                    dbToAddressBook.PhoneNo = dbDetail.ShipFrom.Phone;
                    dbToAddressBook.Zip = dbDetail.ShipFrom.PostCode;
                    dbToAddressBook.IsActive = true;
                    dbToAddressBook.Area = dbDetail.ShipFrom.Area;
                    dbToAddressBook.CompanyName = dbDetail.ShipFrom.CompanyName;
                    dbToAddressBook.ContactFirstName = dbDetail.ShipFrom.FirstName;
                    dbToAddressBook.ContactLastName = dbDetail.ShipFrom.LastName;
                    dbToAddressBook.CountryId = dbDetail.ShipFrom.Country.CountryId;
                    dbToAddressBook.CustomerId = dbDetail.ShipFrom.CustomerId;
                    dbToAddressBook.Email = dbDetail.ShipFrom.Email;
                    dbToAddressBook.FromAddress = true;
                    dbToAddressBook.ToAddress = false;
                    dbContext.AddressBooks.Add(dbToAddressBook);
                    dbContext.SaveChanges();

                }
                var toAddressBookData = dbContext.AddressBooks.Where(p => p.Address1 == dbDetail.ShipTo.Address &&
                                                                       p.Address2 == dbDetail.ShipTo.Address2 &&
                                                                       p.City == dbDetail.ShipTo.City &&
                                                                       p.State == dbDetail.ShipTo.State &&
                                                                       p.PhoneNo == dbDetail.ShipTo.Phone &&
                                                                       p.Area == dbDetail.ShipTo.Area &&
                                                                       p.CompanyName == dbDetail.ShipTo.CompanyName &&
                                                                       p.ContactFirstName == dbDetail.ShipTo.FirstName &&
                                                                       p.ContactLastName == dbDetail.ShipTo.LastName &&
                                                                       p.CountryId == dbDetail.ShipTo.Country.CountryId &&
                                                                       p.CustomerId == dbDetail.ShipTo.CustomerId &&
                                                                       p.Email == dbDetail.ShipTo.Email &&
                                                                       p.Zip == dbDetail.ShipTo.PostCode &&
                                                                       p.IsActive == true &&
                                                                       p.ToAddress == true).ToList();
                if (toAddressBookData != null && toAddressBookData.Count > 0)
                {
                }
                else
                {
                    AddressBook dbToAddressBook = new AddressBook();
                    dbToAddressBook.Address1 = dbDetail.ShipTo.Address;
                    dbToAddressBook.Address2 = dbDetail.ShipTo.Address2;
                    dbToAddressBook.City = dbDetail.ShipTo.City;
                    dbToAddressBook.State = dbDetail.ShipTo.State;
                    dbToAddressBook.PhoneNo = dbDetail.ShipTo.Phone;
                    dbToAddressBook.Zip = dbDetail.ShipTo.PostCode;
                    dbToAddressBook.IsActive = true;
                    dbToAddressBook.Area = dbDetail.ShipTo.Area;
                    dbToAddressBook.CompanyName = dbDetail.ShipTo.CompanyName;
                    dbToAddressBook.ContactFirstName = dbDetail.ShipTo.FirstName;
                    dbToAddressBook.ContactLastName = dbDetail.ShipTo.LastName;
                    dbToAddressBook.CountryId = dbDetail.ShipTo.Country.CountryId;
                    dbToAddressBook.CustomerId = dbDetail.ShipTo.CustomerId;
                    dbToAddressBook.Email = dbDetail.ShipTo.Email;
                    dbToAddressBook.FromAddress = true;
                    dbToAddressBook.ToAddress = false;
                    dbContext.AddressBooks.Add(dbToAddressBook);
                    dbContext.SaveChanges();

                }

                // Save HS Code for eCommerce Package

                SaveHSCode(dbShipment);

                // Save Barcode for shipment 

                SaveeCommerceBarcode(dbShipment);

            }
            return eCommerceShipmentId;
        }

        #region Save HSCode
        private void SaveHSCode(eCommerceShipment DBShipment)
        {
            var data = dbContext.eCommerceShipmentDetails.Where(p => p.eCommerceShipmentId == DBShipment.eCommerceShipmentId).ToList();
            if (data != null && data.Count > 0)
            {
                foreach (var d in data)
                {
                    var hs = dbContext.HSCodes.Where(p => p.Description == d.PiecesContent).FirstOrDefault();
                    if (hs != null)
                    {
                        d.HSCode = hs.HSCode1;
                        dbContext.Entry(d).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
            }
        }
        #endregion

        #region Save eCommerce Barcode
        public void SaveeCommerceBarcode(eCommerceShipment shipment)
        {
            string bar = string.Empty;
            bar = shipment.FrayteNumber;
            float total = 0;
            var piecesDetail = dbContext.eCommerceShipmentDetails.Where(p => p.eCommerceShipmentId == shipment.eCommerceShipmentId).ToList();
            if (piecesDetail != null && piecesDetail.Count > 0)
            {
                foreach (var d in piecesDetail)
                {
                    total += (float)(d.Weight) * d.CartoonValue;
                }
            }

            var country = (from r in dbContext.eCommerceShipmentAddresses
                           join cc in dbContext.Countries on r.CountryId equals cc.CountryId
                           where r.eCommerceShipmentAddressId == shipment.ToAddressId
                           select new FrayteCountryCode
                           {
                               Code = cc.CountryCode,
                               Code2 = cc.CountryCode2,
                               CountryId = cc.CountryId,
                               Name = cc.CountryName
                           }
                           ).FirstOrDefault();
            if (country != null)
            {
                bar = bar + "|" + country.Code;
            }
            bar = bar + "|" + total.ToString();

            BarcodeSettings settings = new BarcodeSettings();
            string data = string.Empty;
            string type = "Code128";
            short fontSize = 8;
            string font = "SimSun";
            data = bar;

            settings.Data2D = data;
            settings.Data = data;
            settings.Type = (BarCodeType)Enum.Parse(typeof(BarCodeType), type);

            if (fontSize != 0 && fontSize.ToString().Length > 0 && Int16.TryParse(fontSize.ToString(), out fontSize))
            {
                if (font != null && font.Length > 0)
                {
                    settings.TextFont = new System.Drawing.Font(font, fontSize, FontStyle.Bold);
                }
            }
            short barHeight = 15;
            if (barHeight != 0 && barHeight.ToString().Length > 0 && Int16.TryParse(barHeight.ToString(), out barHeight))
            {
                settings.BarHeight = barHeight;
            }

            BarCodeGenerator generator = new BarCodeGenerator(settings);
            Image barcode = generator.GenerateImage();


            // Path where we will have barcode 
            string filePathToSave = AppSettings.eCommerceLabelFolder;
            filePathToSave = HttpContext.Current.Server.MapPath(filePathToSave + shipment.eCommerceShipmentId);

            if (!System.IO.Directory.Exists(filePathToSave))
                System.IO.Directory.CreateDirectory(filePathToSave);
            barcode.Save(System.Web.Hosting.HostingEnvironment.MapPath(AppSettings.eCommerceLabelFolder + shipment.eCommerceShipmentId + "/" + shipment.FrayteNumber + ".Png"));
            //Save  ShipmentBarCode
            shipment.BarCodeNumber = settings.Data;
            dbContext.Entry(shipment).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }
        #endregion
        public FrayteCommercePackageTrackingDetail SaveEasyPostDetailTrackingDeatil(FrayteCommerceShipmentDraft directBookingShippingDetail, PackageDraft package, eCommerceShipmentDetail data, string TrackingNo, string LabelUrl, int eCommerceShipmentId, int increment)
        {

            FrayteCommercePackageTrackingDetail fraytePackageTrackingDetail;
            fraytePackageTrackingDetail = new FrayteCommercePackageTrackingDetail();
            //Finally update the label            
            eCommercePackageTrackingDetail packageTrackingDetail = new eCommercePackageTrackingDetail();
            packageTrackingDetail.eCommerceShipmentDetailId = data.eCommerceShipmentDetailId;
            packageTrackingDetail.TrackingNo = TrackingNo;
            dbContext.eCommercePackageTrackingDetails.Add(packageTrackingDetail);
            dbContext.SaveChanges();


            fraytePackageTrackingDetail.eCommerceShipmentDetailId = data.eCommerceShipmentDetailId;
            fraytePackageTrackingDetail.eCommercePackageTrackingDetailId = packageTrackingDetail.eCommercePackageTrackingDetailId;
            fraytePackageTrackingDetail.TrackingNo = TrackingNo;
            fraytePackageTrackingDetail.PackageImage = "";
            fraytePackageTrackingDetail.LabelUrl = LabelUrl;
            fraytePackageTrackingDetail.IsDownloaded = false;
            fraytePackageTrackingDetail.IsPrinted = false;

            // Traking detail in ecommerce object for AWB label 
            package.TrackingNo = TrackingNo;

            return fraytePackageTrackingDetail;
        }

        public string DownloadEasyPostImages(string TrackingCode, string LabelURL, int eCommerceShipmentId)
        {

            var label = new eCommerceShipmentRepository().EasyPostPackageImageName(TrackingCode, LabelURL, eCommerceShipmentId);
            return label;
        }
        public void SaveImage(FrayteCommercePackageTrackingDetail data, string image)
        {
            var PackageTracking = dbContext.eCommercePackageTrackingDetails.Find(data.eCommercePackageTrackingDetailId);
            PackageTracking.PackageImage = image;
            PackageTracking.IsDownloaded = true;
            PackageTracking.IsPrinted = false;
            dbContext.SaveChanges();
        }
        public string EasyPostPackageImageName(string TrackingNo, string PostageUrl, int eCommerceShipmentId)
        {
            string ImageName = string.Empty;
            if (!string.IsNullOrEmpty(PostageUrl))
            {
                if (PostageUrl.Contains("https"))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        byte[] data = webClient.DownloadData(PostageUrl);
                        System.Drawing.Image labelimage;
                        using (MemoryStream mem = new MemoryStream(data))
                        {
                            labelimage = System.Drawing.Image.FromStream(mem);

                            ImageName = TrackingNo + "_" + DateTime.Now.ToString("dd_MM_yyyy_ss_fff") + ".jpg";
                            if (AppSettings.ShipmentCreatedFrom == "BATCH")
                            {
                                System.IO.Directory.CreateDirectory(AppSettings.eCommerceUploadLabelFolder + eCommerceShipmentId + "/");
                                labelimage.Save(AppSettings.eCommerceUploadLabelFolder + eCommerceShipmentId + "/" + ImageName);
                            }
                            else
                            {
                                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder + eCommerceShipmentId + "/"));
                                labelimage.Save(HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder + eCommerceShipmentId + "/") + ImageName);
                            }

                            //labelimage.Save(HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder + eCommerceShipmentId + "/") + ImageName);
                            //labelimage.Save(AppSettings.eCommerceLabelFolder + eCommerceShipmentId + "/" + ImageName);
                        }
                    }
                }
            }
            return ImageName;
        }

        public string DownloadEasyPostImagesUploadShipment(List<FrayteCommercePackageTrackingDetail> shipmentPackageTrackingDetail, int eCommerceShipmentId)
        {
            //foreach (FrayteCommercePackageTrackingDetail trackingDetail in shipmentPackageTrackingDetail)
            //{
            //    var result = dbContext.eCommercePackageTrackingDetails.Find(trackingDetail.eCommercePackageTrackingDetailId);
            //    if (result != null)
            //    {
            //        result.PackageImage = new eCommerceShipmentRepository().EasyPostPackageImageName(trackingDetail.TrackingNo, trackingDetail.LabelUrl, eCommerceShipmentId);
            //        result.IsDownloaded = true;
            //        result.IsPrinted = false;
            //        dbContext.SaveChanges();
            //    }
            //}
            var label = new eCommerceShipmentRepository().EasyPostPackageImageNameUploadShipment(shipmentPackageTrackingDetail[0].TrackingNo, shipmentPackageTrackingDetail[0].LabelUrl, eCommerceShipmentId);
            return label;
        }

        public string EasyPostPackageImageNameUploadShipment(string TrackingNo, string PostageUrl, int eCommerceShipmentId)
        {
            string ImageName = string.Empty;
            if (!string.IsNullOrEmpty(PostageUrl))
            {
                if (PostageUrl.Contains("https"))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        byte[] data = webClient.DownloadData(PostageUrl);
                        System.Drawing.Image labelimage;
                        using (MemoryStream mem = new MemoryStream(data))
                        {
                            labelimage = System.Drawing.Image.FromStream(mem);

                            ImageName = TrackingNo + "_" + DateTime.Now.ToString("dd_MM_yyyy_ss_fff") + ".jpg";
                            System.IO.Directory.CreateDirectory(AppSettings.eCommerceUploadShipmentLabelFolder + eCommerceShipmentId + "/");
                            //labelimage.Save(HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder + eCommerceShipmentId + "/") + ImageName);
                            labelimage.Save(AppSettings.eCommerceUploadShipmentLabelFolder + eCommerceShipmentId + "/" + ImageName);
                        }
                    }
                }
            }
            return ImageName;
        }

        public DataAccess.CountryLogistic GetLogisticService(int countryId)
        {
            return dbContext.CountryLogistics.Where(p => p.CountryId == countryId).FirstOrDefault();
        }

        public string PackageLabelPDFPath(int eCommerceShipmentId, string BookingApp, string CourierName)
        {
            string Attachment = string.Empty;

            var item = (from r in dbContext.eCommerceShipments
                        where r.eCommerceShipmentId == eCommerceShipmentId
                        select new
                        {
                            FrayteLabel = r.FrayteLabel,
                            LogisticLabel = r.LogisticLabel
                        }).FirstOrDefault();


            var data = dbContext.eCommerceShipments.Find(eCommerceShipmentId);
            if (item != null && data != null)
            {
                if (File.Exists(HostingEnvironment.MapPath("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/" + item.FrayteLabel)))
                    Attachment += HostingEnvironment.MapPath("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/" + item.FrayteLabel);
                if (BookingApp == eCommerceShipmentType.eCommerceOnline || BookingApp == eCommerceShipmentType.eCommerceSS)
                {
                    if (File.Exists(HostingEnvironment.MapPath("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/" + item.LogisticLabel)))
                        Attachment += ";" + HostingEnvironment.MapPath("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/" + item.LogisticLabel);
                }
            }

            return Attachment;
        }
        public string PackageLabelPath(int eCommerceShipmentId)
        {
            string Attachment = string.Empty;

            var shipment = (from r in dbContext.eCommerceShipments
                            where r.eCommerceShipmentId == eCommerceShipmentId
                            select new
                            {
                                FrayteLabel = r.FrayteLabel,
                                LogisticLabel = r.LogisticLabel
                            }
                            ).FirstOrDefault();



            if (shipment != null)
            {
                Attachment += HostingEnvironment.MapPath("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/" + shipment.LogisticLabel);
                Attachment += ";" + HostingEnvironment.MapPath("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/" + shipment.FrayteLabel);

            }

            return Attachment;
        }

        public List<eCommerceShipmentDetail> GetPackageDetails(int eCommerceShipmentId)
        {
            var data = dbContext.eCommerceShipmentDetails.Where(p => p.eCommerceShipmentId == eCommerceShipmentId).ToList();
            if (data != null && data.Count > 0)
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        public eCommerceShipmentDetail GetPackageDetail(int eCommerceShipmentDetailId)
        {
            var data = dbContext.eCommerceShipmentDetails.Where(p => p.eCommerceShipmentDetailId == eCommerceShipmentDetailId).FirstOrDefault();
            if (data != null)
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        public List<eCommercePackageTrackingDetail> GeteCommercePackageTracking(int eCommerceShipmentDetailId)
        {
            try
            {
                var PackageTracking = dbContext.eCommercePackageTrackingDetails.Where(p => p.eCommerceShipmentDetailId == eCommerceShipmentDetailId).ToList();
                if (PackageTracking != null)
                {
                    return PackageTracking;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        #endregion

        #region eCommerceShipmemnt Label

        public FrayteeCommerceShipmentLabelReport GeteCommerceShipmentLabelReportDetail(FrayteeCommerceShipmentLabelReport obj)
        {
            var customerDetail = (from u in dbContext.Users
                                  join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                  join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                  join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                  where u.UserId == obj.eCommerceShipment.CustomerId
                                  select new
                                  {
                                      CustomerName = u.ContactName,
                                      CustomerEmail = u.Email,
                                      CompanyName = u.CompanyName,
                                      UserName = u1.ContactName,
                                      UserPosition = u1.Position,
                                      UserEmail = u1.Email,
                                      UserPhone = u1.TelephoneNo,
                                      UserSkype = u1.Skype,
                                      UserFax = u1.FaxNumber,
                                      CustomerAccountNo = ua.AccountNo
                                  }).FirstOrDefault();

            var courier = (from r in dbContext.CountryLogistics
                           join cc in dbContext.Countries on r.CountryId equals cc.CountryId
                           where r.CountryId == obj.eCommerceShipment.ShipTo.Country.CountryId
                           select new
                           {
                               CourierName = r.LogisticService,
                               CourierCompanyDisplay = r.LogisicServiceDisplay
                           }).FirstOrDefault();

            obj.CourierCompanyDisplay = courier.CourierCompanyDisplay;
            obj.ServiceType = eCommerceAWBLabel.ECN;
            obj.TaxAndDutyType = eCommerceAWBLabel.DDU;
            obj.CustomerName = customerDetail.CustomerName;
            obj.AccountNo = UtilityRepository.FrayteAccountNo(customerDetail.CustomerAccountNo);
            return obj;
        }
        public List<FrayteeCommerceShipmentLabelReport> GeteCommerceShipmentLabelReportDetail(List<FrayteeCommerceShipmentLabelReport> list)
        {
            List<FrayteeCommerceShipmentLabelReport> objList = new List<FrayteeCommerceShipmentLabelReport>();
            FrayteeCommerceShipmentLabelReport obj;
            foreach (var data in list)
            {
                obj = new FrayteeCommerceShipmentLabelReport();
                var customerDetail = (from u in dbContext.Users
                                      join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                      join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                      join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                      where u.UserId == data.eCommerceShipment.CustomerId
                                      select new
                                      {
                                          CustomerName = u.ContactName,
                                          CustomerEmail = u.Email,
                                          CompanyName = u.CompanyName,
                                          UserName = u1.ContactName,
                                          UserPosition = u1.Position,
                                          UserEmail = u1.Email,
                                          UserPhone = u1.TelephoneNo,
                                          UserSkype = u1.Skype,
                                          UserFax = u1.FaxNumber,
                                          CustomerAccountNo = ua.AccountNo
                                      }).FirstOrDefault();

                var courier = (from r in dbContext.CountryLogistics
                               join cc in dbContext.Countries on r.CountryId equals cc.CountryId
                               where r.CountryId == data.eCommerceShipment.ShipTo.Country.CountryId
                               select new
                               {
                                   CourierName = r.LogisticService,
                                   CourierCompanyDisplay = r.LogisicServiceDisplay
                               }).FirstOrDefault();

                data.CourierCompanyDisplay = courier.CourierCompanyDisplay;
                data.ServiceType = eCommerceAWBLabel.ECN;
                data.TaxAndDutyType = eCommerceAWBLabel.DDU;
                data.CustomerName = customerDetail.CustomerName;
                data.AccountNo = UtilityRepository.FrayteAccountNo(customerDetail.CustomerAccountNo);

                objList.Add(obj);
            }

            return list;
        }
        #endregion

        #region eCommmmerce Invoive Report

        public HSCodeMapped ISAllHSCodeMapped(int eCommerceShipmentId)
        {
            HSCodeMapped result = new HSCodeMapped();
            try
            {
                var shipDetail = dbContext.eCommerceShipmentDetails
                                          .Where(p => p.eCommerceShipmentId == eCommerceShipmentId &&

                                          string.IsNullOrEmpty(p.HSCode)).ToList();
                var shipentDetail = dbContext.eCommerceShipments.Find(eCommerceShipmentId);

                if ((shipentDetail.EstimatedDateofArrival.HasValue && shipentDetail.EstimatedTimeofArrival.HasValue &&
                    shipentDetail.EstimatedTimeofDelivery.HasValue && shipentDetail.EstimatedDateofDelivery.HasValue) &&
                        shipDetail == null || (shipDetail != null && shipDetail.Count == 0))
                {
                    result.Status = true;
                    result.Id = eCommerceShipmentId;
                }
                else
                {
                    result.Status = false;
                    result.Id = eCommerceShipmentId;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Id = eCommerceShipmentId;

            }
            return result;
        }

        public eCommerceTaxAndDutyInvoiceReport GeteCommerceInvoiceObj(int eCommerceShipmentId)
        {
            var adminSpecClearServ = 0.00M; // Later this will come from database
            var adminDisbursement = 11.00M;  // Later this will come from database
            var adminVAT = 0.00M; // Later this will come from database
            var OtherLevy = 0.00M;

            FrayteeCommerceShipmentDetail eCommerceBookingDetail = new eCommerceShipmentRepository().GeteCommerceBookingDetail(eCommerceShipmentId, "");

            var customerDetail = dbContext.Users.Find(eCommerceBookingDetail.CustomerId);
            var customerAddional = dbContext.UserAdditionals.Where(p => p.UserId == eCommerceBookingDetail.CustomerId).FirstOrDefault();
            eCommerceTaxAndDutyInvoiceReport obj = new eCommerceTaxAndDutyInvoiceReport();

            var customerAdditional = dbContext.UserAdditionals.Where(p => p.UserId == eCommerceBookingDetail.CustomerId).FirstOrDefault();
            var UserDetail = dbContext.Users.Where(ab => ab.UserId == eCommerceBookingDetail.CustomerId).FirstOrDefault();
            var TimeZone = dbContext.Timezones.Where(a => a.TimezoneId == UserDetail.TimezoneId).FirstOrDefault();
            var TimeZoneInformation = TimeZoneInfo.FindSystemTimeZoneById(TimeZone.Name);


            var ArrivalDateTime = UtilityRepository.UtcDateToOtherTimezone(eCommerceBookingDetail.EstimatedDateofArrival.Value, UtilityRepository.GetTimeFromString(UtilityRepository.GetFlatTimeString(eCommerceBookingDetail.EstimatedTimeofArrival)).Value, TimeZoneInformation).Item1;
            obj.ArrivalDateTime = ArrivalDateTime;
            obj.FreeStorageDateTime = ArrivalDateTime.AddHours(customerAdditional.FreeStorageTime.Value.TotalHours);
            obj.CongigmentNo = eCommerceBookingDetail.FrayteNumber;
            obj.FreeStorageTime = UtilityRepository.GetFormattedTimeFromString(UtilityRepository.GetTimeZoneTime(customerAddional.FreeStorageTime));

            var adminCharegs = new AdminChargesRepository().GetDefaultCustomerAdminCharges(dbContext.eCommerceShipments.Find(eCommerceShipmentId).CustomerId);

            var fg = DateTime.Now.ToString("dd MMM YYYY");

            obj.AdminCharges = new List<InvoiceAdminCharge>();
            InvoiceAdminCharge charge;
            foreach (var item in adminCharegs.Charges)
            {
                charge = new InvoiceAdminCharge();

                charge.ChargeShortname = item.Key;
                charge.ChargeDescription = item.Value;
                charge.CurrencyCode = item.CurrencyCode;
                charge.Amount = item.Amount;
                charge.AmountWithCurrency = item.CurrencyCode + " " + item.Amount.ToString();
                obj.AdminCharges.Add(charge);
            }

            obj.eCommerceBookingDetail = eCommerceBookingDetail;
            var bankDetail = dbContext.Banks.Where(p => p.CountryId == eCommerceBookingDetail.ShipTo.Country.CountryId).FirstOrDefault();
            if (bankDetail != null)
            {
                obj.BankDetail = new FaryteBankAccount();
                obj.BankDetail.Name = bankDetail.Name;
                obj.BankDetail.Address = new FrayteBankAddress();
                obj.BankDetail.Address.Address1 = bankDetail.Address1;
                obj.BankDetail.Address.Address2 = bankDetail.Address2;
                obj.BankDetail.Address.City = bankDetail.City;
                obj.BankDetail.Address.State = bankDetail.State;
                obj.BankDetail.Address.Zip = bankDetail.PostCode;
                obj.BankDetail.AcountNo = bankDetail.AccountNumber;
                obj.BankDetail.SortCode = bankDetail.SortCode;
                obj.BankDetail.IBANCode = bankDetail.IBANCode;
                obj.BankDetail.SwiftCode = bankDetail.SwiftCode;
            }

            // Duty and Vat calculation
            FrayteResult result = dutyVatCalculation(eCommerceShipmentId, eCommerceBookingDetail, obj);


            obj.CustomerName = customerDetail.ContactName;

            obj.CompanyDetail = new FrayteCompany();
            obj.CompanyDetail.CompanyName = "FRAYTE LOGISTICS Ltd";
            obj.CompanyDetail.AccountEmail = "rpu.cash@frayte.co.uk";
            obj.CompanyDetail.SalesEmail = "sales@frayte.co.uk";
            obj.CompanyDetail.CompanyNo = "8128717";
            obj.CompanyDetail.CompanyPhone = "01792 678017";
            obj.CompanyDetail.CashEmail = "cash@frayte.co.uk";
            obj.CompanyDetail.CrestCode = "XXXXXX";
            obj.CompanyDetail.Address = new FrayteBankAddress();
            obj.CompanyDetail.Address.Address1 = "34 North Street";
            obj.CompanyDetail.Address.Address2 = "Bridgwater";
            obj.CompanyDetail.Address.City = "Somerset";
            obj.CompanyDetail.Address.Zip = "TA6 3YD";
            obj.CompanyDetail.Address.Country = new FrayteCountryCode();
            obj.CompanyDetail.Address.Country.Name = "United Kingdom";
            obj.CompanyDetail.Address.Country.Code = "GBR";
            obj.CompanyDetail.Address.Country.Code2 = "UK";
            obj.CompanyDetail.Address.Country.CountryId = 228;
            //obj.CompanyDetail.Address.Country.

            string year = DateTime.Now.Year.ToString();

            obj.InvoiceDateTime = UtilityRepository.UtcDateToOtherTimezone(DateTime.UtcNow, DateTime.UtcNow.TimeOfDay, TimeZoneInformation).Item1;
            var inv = dbContext.eCommerceInvoices.Where(p => p.ShipmentId == eCommerceShipmentId).FirstOrDefault();
            string FrayteNo = string.Empty;
            if (inv != null)
            {
                FrayteNo = inv.InvoiceRef;
            }
            else
            {
                FrayteNo = CommonConversion.GetNewFrayteNumber();
            }

            if (!string.IsNullOrEmpty(FrayteNo))
            {
                obj.InvoiceRef = FrayteNo;
                obj.InvoiveFullName = "CCS - Customs Duty - Vat Invoice [" + obj.InvoiceRef + "]  [" + obj.CustomerName + "].pdf";
            }

            return obj;
        }
        public FrayteResult SaveeCommerceInvoice(int eCommerceShipmentId, eCommerceTaxAndDutyInvoiceReport obj)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var customerDetail = (from r in dbContext.eCommerceShipments
                                      join ua in dbContext.UserAdditionals on r.CustomerId equals ua.UserId
                                      where r.eCommerceShipmentId == eCommerceShipmentId
                                      select new
                                      {
                                          ShipmentDate = r.CreatedOn,
                                          TermsOfPayment = ua.TaxAndDuties
                                      }
                           ).FirstOrDefault();

                if (customerDetail != null)
                {
                    DateTime date = new DateTime();
                    if (customerDetail.TermsOfPayment == eCommerceInvoiceTermsOfPayment.SevenDays)
                    {
                        date = customerDetail.ShipmentDate == null ? DateTime.UtcNow.AddDays(7) : customerDetail.ShipmentDate.AddDays(7);
                    }
                    else if (customerDetail.TermsOfPayment == eCommerceInvoiceTermsOfPayment.FourteenDays)
                    {
                        date = customerDetail.ShipmentDate == null ? DateTime.UtcNow.AddDays(14) : customerDetail.ShipmentDate.AddDays(14);
                    }
                    else if (customerDetail.TermsOfPayment == eCommerceInvoiceTermsOfPayment.TwentyOneDays)
                    {
                        date = customerDetail.ShipmentDate == null ? DateTime.UtcNow.AddDays(21) : customerDetail.ShipmentDate.AddDays(21);
                    }
                    else if (customerDetail.TermsOfPayment == eCommerceInvoiceTermsOfPayment.ThirtyDays)
                    {
                        date = customerDetail.ShipmentDate == null ? DateTime.UtcNow.AddDays(30) : customerDetail.ShipmentDate.AddDays(30);
                    }
                    else if (customerDetail.TermsOfPayment == eCommerceInvoiceTermsOfPayment.SixtyDays)
                    {
                        date = customerDetail.ShipmentDate == null ? DateTime.UtcNow.AddDays(60) : customerDetail.ShipmentDate.AddDays(60);
                    }
                    else if (customerDetail.TermsOfPayment == eCommerceInvoiceTermsOfPayment.PrePayment)
                    {
                        date = customerDetail.ShipmentDate == null ? DateTime.UtcNow.AddDays(60) : customerDetail.ShipmentDate.AddDays(60);
                    }
                    else if (customerDetail.TermsOfPayment == eCommerceInvoiceTermsOfPayment.PaymentTermFreight)
                    {
                        date = customerDetail.ShipmentDate == null ? DateTime.UtcNow.AddDays(60) : customerDetail.ShipmentDate.AddDays(60);
                    }
                    else
                    {
                        date = customerDetail.ShipmentDate == null ? DateTime.UtcNow.AddDays(60) : customerDetail.ShipmentDate.AddDays(60);
                    }
                    eCommerceInvoice invoice = new eCommerceInvoice();
                    invoice.InvoiceDate = DateTime.UtcNow;
                    invoice.ShipmentId = eCommerceShipmentId;
                    invoice.TotalDuty = obj.TotalDuty;
                    invoice.TotalVAT = obj.TotalVAT;
                    invoice.TotalDeclaration = obj.TotalDeclaration;
                    invoice.AdminSpecClearServ = obj.AdminSpecClearServ;
                    invoice.AdminDisbursement = obj.AdminDisbursement;
                    invoice.TotalAdminVAT = obj.TotalAdminVAT;
                    invoice.OtherLevy = obj.TotalOtherLevy;
                    invoice.CurrencyCode = obj.CurrencyCode;
                    invoice.InvoiceRef = obj.InvoiceRef;
                    invoice.InvoiceFullName = obj.InvoiveFullName;
                    invoice.Status = eCommerceAppTaxAndDutyStatus.TaxAndDutyUnPaid;
                    invoice.InvoiceDueDateUtc = date;
                    dbContext.eCommerceInvoices.Add(invoice);
                    dbContext.SaveChanges();
                    result.Status = true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                result.Status = false;
            }

            return result;
        }


        // Generate Invoice After all the Hs Code for A shipment has beedn mapped
        public FrayteResult GenerateInvoice(int eCommerceShipmentId)
        {
            FrayteResult result = new FrayteResult();

            try
            {


                result.Status = true;

            }
            catch
            {
                result.Status = false;
            }
            return result;
        }
        public string InvoicePath(int eCommerceShipmentId)
        {
            string Attachment = string.Empty;

            var item = dbContext.eCommerceInvoices.Where(p => p.ShipmentId == eCommerceShipmentId).FirstOrDefault();

            if (item != null)
            {
                Attachment = HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder) + eCommerceShipmentId + "\\" + item.InvoiceFullName;
            }

            return Attachment;
        }
        #region Private Methods
        private FrayteResult dutyVatCalculation(int eCommerceShipmentId, FrayteeCommerceShipmentDetail eCommerceBookingDetail, eCommerceTaxAndDutyInvoiceReport obj)
        {
            FrayteResult result = new FrayteResult();
            try
            {

                var adminSpecClearServ = 0.00M; // Later this will come from database

                var totalAdminCharge = 0.00M;
                foreach (var item in obj.AdminCharges)
                {
                    totalAdminCharge += item.Amount;
                }
                var adminDisbursement = 11.00M;  // Later this will come from database
                var adminVAT = 0.00M; // Later this will come from database
                var OtherLevy = 0.00M;

                int TotalDeclaration = 0;
                var totalVat = 0.00M;
                var totalDuty = 0.00M;
                foreach (var data in eCommerceBookingDetail.Packages)
                {
                    if (eCommerceBookingDetail.Currency.CurrencyCode != "GBP")
                    {
                        var exchangeCurrency = dbContext.OperationZoneExchangeRates.Where(p => p.OperationZoneId == eCommerceBookingDetail.OpearionZoneId && p.CurrencyCode == eCommerceBookingDetail.Currency.CurrencyCode).FirstOrDefault();
                        var exchangeCurrencyGBP = dbContext.OperationZoneExchangeRates.Where(p => p.OperationZoneId == eCommerceBookingDetail.OpearionZoneId && p.CurrencyCode == "GBP").FirstOrDefault();
                        if (exchangeCurrency != null && exchangeCurrencyGBP != null)
                        {
                            var HKG = exchangeCurrency.ExchangeRate;
                            var asa = Math.Round((1 * data.Value / exchangeCurrency.ExchangeRate) * exchangeCurrencyGBP.ExchangeRate, 2);

                            var pe1r = dbContext.HSCodes.Where(p => p.HSCode1 == data.HSCode).FirstOrDefault();
                            var d1 = Math.Round((data.CartoonValue * asa) * (pe1r.Duty.HasValue ? pe1r.Duty.Value / 100 : 0), 2);
                            var e1 = Math.Round((data.CartoonValue * asa) * (pe1r.VAT.HasValue ? pe1r.VAT.Value / 100 : 0));
                            TotalDeclaration += data.CartoonValue;
                            totalDuty += d1;

                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        var per = dbContext.HSCodes.Where(p => p.HSCode1 == data.HSCode).FirstOrDefault();
                        var d = Math.Round((data.CartoonValue * data.Value) * (per.Duty.HasValue ? per.Duty.Value / 100 : 0), 2);
                        var e = Math.Round((data.CartoonValue * data.Value) * (per.VAT.HasValue ? per.VAT.Value / 100 : 0));
                        TotalDeclaration += data.CartoonValue;
                        totalDuty += d;
                        totalVat += e;
                    }



                }
                obj.TotalDuty = totalDuty;
                obj.TotalVAT = totalVat;
                obj.TotalOtherLevy = OtherLevy;
                obj.AdminSpecClearServ = adminSpecClearServ;
                obj.AdminDisbursement = adminDisbursement;

                obj.TotalAdminCharge = totalAdminCharge; // adminSpecClearServ + adminDisbursement + adminVAT;
                obj.TotalAdminVAT = totalVat;
                obj.CurrencyCode = "GBP";
                obj.TotalDeclaration = TotalDeclaration;
                obj.TotalCustomCharge = totalDuty + totalVat + OtherLevy;
                obj.NettCharge = Math.Round((obj.TotalCustomCharge + obj.TotalAdminCharge), 2); // remove total admin vat from calculation



                result.Status = true;
            }
            catch (Exception ex)
            {
                result.Status = false;
            }
            return result;
        }
        #endregion

        #endregion

        #region
        public List<FrayteManifestOnExcel> SaveCustomManifestDetail(System.Data.DataTable exceldata)
        {
            List<FrayteManifestOnExcel> fManifestXL = new List<FrayteManifestOnExcel>();
            foreach (System.Data.DataRow edata in exceldata.Rows)
            {
                int myData = int.Parse(edata["InternalAccountNumber"].ToString());
                eCommerceShipmentDetail eCSDetail = (from ecsd in dbContext.eCommerceShipmentDetails
                                                     where ecsd.eCommerceShipmentDetailId == myData
                                                     select ecsd).FirstOrDefault();

                if (eCSDetail != null)
                {
                    //eCSDetail.CustomCommodityMap = eCSDetail.CustomCommodityMap;
                    //eCSDetail.CustomsEntryType = eCSDetail.CustomsEntryType;
                    //eCSDetail.CustomsTotalValue = eCSDetail.CustomsTotalValue;
                    //eCSDetail.CustomsTotalVAT = eCSDetail.CustomsTotalVAT;
                    //eCSDetail.CustomsDuty = eCSDetail.CustomsDuty;

                    eCSDetail.CustomCommodityMap = edata["CustomCommodityMap"].ToString() == "" ? "" : edata["CustomCommodityMap"].ToString();
                    eCSDetail.CustomsEntryType = edata["CustomEntryType"].ToString() == "" ? "" : edata["CustomEntryType"].ToString();
                    eCSDetail.CustomsTotalValue = edata["CustomTotalValue"] != null ? CommonConversion.ConvertToDecimal(edata, "CustomTotalValue") : 0;
                    eCSDetail.CustomsTotalVAT = edata["CustomTotalVAT"] != null ? CommonConversion.ConvertToDecimal(edata, "CustomTotalVAT") : 0;
                    eCSDetail.CustomsDuty = edata["CustomDuty"] != null ? CommonConversion.ConvertToDecimal(edata, "CustomDuty") : 0;
                    dbContext.Entry(eCSDetail).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    if (eCSDetail.CustomCommodityMap != "" || eCSDetail.CustomsEntryType != "" || eCSDetail.CustomsTotalValue != 0 ||
                        eCSDetail.CustomsTotalVAT != 0 || eCSDetail.CustomsDuty != 0)
                    {
                        SaveCustomClearence(edata);
                    }
                }
            }
            return fManifestXL;
        }

        public void SaveCustomClearence(System.Data.DataRow edata)
        {
            int myData = int.Parse(edata["InternalAccountNumber"].ToString());
            var result = dbContext.eCommerceShipmentDetails.Where(p => p.eCommerceShipmentDetailId == myData).FirstOrDefault();

            if (result != null)
            {
                result.IsCustomClearance = true;
                dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }


        #endregion

        #region Shipment HS Code
        public FrayteResult SetShipmentHSCode(int eCommerceShipmentDetailid, string hSCode)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var data = dbContext.HSCodes.Where(p => p.HSCode1 == hSCode).FirstOrDefault();
                if (data != null)
                {
                    result.Status = true;
                }
                else
                {
                    result.Status = false;
                }

            }
            catch (Exception ex)
            {
                result.Status = false;
            }
            return result;
        }

        #endregion

        #region  eCommerce Invoive sheduler
        public List<eCommerceInvoice> GetShipmentInvoices(int eCommerceShimentId)
        {
            var list = dbContext.eCommerceInvoices.Where(p => p.ShipmentId == eCommerceShimentId).OrderByDescending(p => p.InvoiceId).ToList();
            return list;
        }
        public List<FrayteeCommerceInvoice> GetUnpaideCommerceInvoives()
        {
            var collection = (from r in dbContext.eCommerceInvoices
                              join i in dbContext.eCommerceShipments on r.ShipmentId equals i.eCommerceShipmentId
                              join ua in dbContext.UserAdditionals on i.CustomerId equals ua.UserId
                              where r.Status != eCommerceAppTaxAndDutyStatus.TaxAndDutyPaid && r.IsPrimary == true
                              select new
                              {
                                  eCommerceShipmentId = i.eCommerceShipmentId,
                                  CustomerId = i.CustomerId,
                                  InvoiceStatus = r.Status,
                                  InvoiceRef = r.InvoiceRef,
                                  InvoiceFullName = r.InvoiceFullName,
                                  InvoiceCurrencyCode = r.CurrencyCode,
                                  ETADate = i.EstimatedDateofArrival,
                                  ETATime = i.EstimatedTimeofArrival,
                                  InvoiceId = r.InvoiceId,
                                  FreeStorageCharge = ua.FreeStorageCharge,
                                  FreeStorageCurrency = ua.FreeStorageCurrencyCode,
                                  FreeStorageTime = ua.FreeStorageTime
                              }
                 ).ToList();

            List<FrayteeCommerceInvoice> list = new List<FrayteeCommerceInvoice>();
            FrayteeCommerceInvoice invoice;

            foreach (var item in collection)
            {
                var userInfo = (from r in dbContext.Users
                                join tz in dbContext.Timezones on r.TimezoneId equals tz.TimezoneId
                                where r.UserId == item.CustomerId
                                select tz
                           ).FirstOrDefault();

                var UserTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(userInfo.Name);

                invoice = new FrayteeCommerceInvoice();

                invoice.eCommerceShimentId = item.eCommerceShipmentId;
                invoice.CustomerId = item.CustomerId;
                invoice.Status = item.InvoiceStatus;
                invoice.InvoiceRef = item.InvoiceRef;
                invoice.InvoiceFullName = item.InvoiceFullName;
                invoice.InvoiceCurrencyCode = item.InvoiceCurrencyCode;
                invoice.ETADate = UtilityRepository.UtcDateToOtherTimezone(item.ETADate.Value, item.ETATime.Value, UserTimeZoneInfo).Item1;
                invoice.ETATime = UtilityRepository.GetTimeZoneTime(item.ETATime);
                invoice.InvoiceId = item.InvoiceId;
                invoice.FreeStorageCharge = item.FreeStorageCharge.Value;
                invoice.FreeStorageCurrency = item.FreeStorageCurrency;
                invoice.FreeStorageTime = item.FreeStorageTime.Value;
                list.Add(invoice);
            }
            return list;
        }

        public void GenerateNewInvoice(string invoiceToMake, FrayteeCommerceInvoice invoice)
        {
            generateNewInvoiceObject(invoice.eCommerceShimentId);
        }

        private void generateNewInvoiceObject(int eCommerceShimentId)
        {
            var shipmentInvoices = dbContext.eCommerceInvoices.Where(p => p.ShipmentId == eCommerceShimentId).OrderByDescending(p => p.InvoiceId).ToList();

            var totalFreeStorageTime = 0.00M;

            foreach (var item in shipmentInvoices)
            {
                totalFreeStorageTime += item.TotalFreeStorageCharge.HasValue ? item.TotalFreeStorageCharge.Value : 0.00M;

            }



            throw new NotImplementedException();

        }

        public eCommerceTaxAndDutyInvoiceReport GeteCommerceNewInvoiceObj(int eCommerceShipmentId)
        {

            var shipmentInvoices = dbContext.eCommerceInvoices.Where(p => p.ShipmentId == eCommerceShipmentId).OrderByDescending(p => p.InvoiceId).ToList();

            var totalFreeStorageTime = 0.00M;

            foreach (var item in shipmentInvoices)
            {
                totalFreeStorageTime += item.TotalFreeStorageCharge.HasValue ? item.TotalFreeStorageCharge.Value : 0.00M;

            }

            FrayteeCommerceShipmentDetail eCommerceBookingDetail = new eCommerceShipmentRepository().GeteCommerceBookingDetail(eCommerceShipmentId, "");

            var customerDetail = dbContext.Users.Find(eCommerceBookingDetail.CustomerId);
            var customerAddional = dbContext.UserAdditionals.Where(p => p.UserId == eCommerceBookingDetail.CustomerId).FirstOrDefault();
            eCommerceTaxAndDutyInvoiceReport obj = new eCommerceTaxAndDutyInvoiceReport();

            var customerAdditional = dbContext.UserAdditionals.Where(p => p.UserId == eCommerceBookingDetail.CustomerId).FirstOrDefault();
            var UserDetail = dbContext.Users.Where(ab => ab.UserId == eCommerceBookingDetail.CustomerId).FirstOrDefault();
            var TimeZone = dbContext.Timezones.Where(a => a.TimezoneId == UserDetail.TimezoneId).FirstOrDefault();
            var TimeZoneInformation = TimeZoneInfo.FindSystemTimeZoneById(TimeZone.Name);


            var ArrivalDateTime = UtilityRepository.UtcDateToOtherTimezone(eCommerceBookingDetail.EstimatedDateofArrival.Value, UtilityRepository.GetTimeFromString(UtilityRepository.GetFlatTimeString(eCommerceBookingDetail.EstimatedTimeofArrival)).Value, TimeZoneInformation).Item1;
            obj.ArrivalDateTime = ArrivalDateTime;
            obj.FreeStorageDateTime = ArrivalDateTime.AddHours(customerAdditional.FreeStorageTime.Value.TotalHours);
            obj.CongigmentNo = eCommerceBookingDetail.FrayteNumber;
            obj.FreeStorageTime = UtilityRepository.GetFormattedTimeFromString(UtilityRepository.GetTimeZoneTime(customerAddional.FreeStorageTime));

            var adminCharegs = new AdminChargesRepository().GetDefaultCustomerAdminCharges(dbContext.eCommerceShipments.Find(eCommerceShipmentId).CustomerId);

            var fg = DateTime.Now.ToString("dd MMM YYYY");

            obj.AdminCharges = new List<InvoiceAdminCharge>();
            InvoiceAdminCharge charge;
            foreach (var item in adminCharegs.Charges)
            {
                charge = new InvoiceAdminCharge();

                charge.ChargeShortname = item.Key;
                charge.ChargeDescription = item.Value;
                charge.CurrencyCode = item.CurrencyCode;
                charge.Amount = item.Amount;
                charge.AmountWithCurrency = item.CurrencyCode + " " + item.Amount.ToString();
                obj.AdminCharges.Add(charge);
            }

            obj.eCommerceBookingDetail = eCommerceBookingDetail;
            var bankDetail = dbContext.Banks.Where(p => p.CountryId == eCommerceBookingDetail.ShipTo.Country.CountryId).FirstOrDefault();
            if (bankDetail != null)
            {
                obj.BankDetail = new FaryteBankAccount();
                obj.BankDetail.Name = bankDetail.Name;
                obj.BankDetail.Address = new FrayteBankAddress();
                obj.BankDetail.Address.Address1 = bankDetail.Address1;
                obj.BankDetail.Address.Address2 = bankDetail.Address2;
                obj.BankDetail.Address.City = bankDetail.City;
                obj.BankDetail.Address.State = bankDetail.State;
                obj.BankDetail.Address.Zip = bankDetail.PostCode;
                obj.BankDetail.AcountNo = bankDetail.AccountNumber;
                obj.BankDetail.SortCode = bankDetail.SortCode;
                obj.BankDetail.IBANCode = bankDetail.IBANCode;
                obj.BankDetail.SwiftCode = bankDetail.SwiftCode;
            }

            // Duty and Vat calculation
            FrayteResult result = dutyVatCalculation(eCommerceShipmentId, eCommerceBookingDetail, obj);


            obj.CustomerName = customerDetail.ContactName;

            obj.CompanyDetail = new FrayteCompany();
            obj.CompanyDetail.CompanyName = "FRAYTE LOGISTICS Ltd";
            obj.CompanyDetail.AccountEmail = "rpu.cash@frayte.co.uk";
            obj.CompanyDetail.SalesEmail = "sales@frayte.co.uk";
            obj.CompanyDetail.CompanyNo = "8128717";
            obj.CompanyDetail.CompanyPhone = "01792 678017";
            obj.CompanyDetail.CashEmail = "cash@frayte.co.uk";
            obj.CompanyDetail.CrestCode = "XXXXXX";
            obj.CompanyDetail.Address = new FrayteBankAddress();
            obj.CompanyDetail.Address.Address1 = "34 North Street";
            obj.CompanyDetail.Address.Address2 = "Bridgwater";
            obj.CompanyDetail.Address.City = "Somerset";
            obj.CompanyDetail.Address.Zip = "TA6 3YD";
            obj.CompanyDetail.Address.Country = new FrayteCountryCode();
            obj.CompanyDetail.Address.Country.Name = "United Kingdom";
            obj.CompanyDetail.Address.Country.Code = "GBR";
            obj.CompanyDetail.Address.Country.Code2 = "UK";
            obj.CompanyDetail.Address.Country.CountryId = 228;
            //obj.CompanyDetail.Address.Country.

            string year = DateTime.Now.Year.ToString();

            obj.InvoiceDateTime = UtilityRepository.UtcDateToOtherTimezone(DateTime.UtcNow, DateTime.UtcNow.TimeOfDay, TimeZoneInformation).Item1;
            var inv = dbContext.eCommerceInvoices.Where(p => p.ShipmentId == eCommerceShipmentId).FirstOrDefault();
            string FrayteNo = string.Empty;
            if (inv != null)
            {
                FrayteNo = inv.InvoiceRef;
            }
            else
            {
                FrayteNo = CommonConversion.GetNewFrayteNumber();
            }

            if (!string.IsNullOrEmpty(FrayteNo))
            {
                obj.InvoiceRef = FrayteNo;
                obj.InvoiveFullName = "CCS - Customs Duty - Vat Invoice [" + obj.InvoiceRef + "]  [" + obj.CustomerName + "].pdf";
            }

            return obj;
        }
        #endregion

    }
}
