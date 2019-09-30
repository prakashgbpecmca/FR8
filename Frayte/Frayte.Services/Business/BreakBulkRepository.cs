using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Models.BreakBulk;
using Frayte.Services.Models.Tradelane;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Data.Entity.Migrations;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Business
{
    public class BreakBulkRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<TradelaneShipmentHandlerMethod> GetShipmentHandlerMethod()
        {
            List<TradelaneShipmentHandlerMethod> list = (from r in dbContext.ShipmentHandlerMethods
                                                         select new TradelaneShipmentHandlerMethod
                                                         {
                                                             ShipmentHandlerMethodId = r.ShipmentHandlerMethodId,
                                                             ShipmentHandlerMethodName = r.ShipmentHandlerMethodName,
                                                             ShipmentHandlerMethodType = r.ShipmentHandlerMethodType,
                                                             ShipmentHandlerMethodCode = r.ShipmentHandlerMethodCode,
                                                             ShipmentHandlerMethodDisplay = r.ShipmentHandlerMethodCode + " - " + r.ShipmentHandlerMethodName
                                                         }).ToList();

            return list;
        }

        public List<FrayteIncoterm> GetIncoterms()
        {
            var lstTimeZone = (from c in dbContext.Incoterms
                               select new FrayteIncoterm()
                               {
                                   IncotermID = c.IncotermId,
                                   ShipmentType = c.ShipmentHandlerMethodType,
                                   IncotermCode = c.IncotermCode,
                                   IncotermDisplayValue = c.IncotermDisplayValue,
                                   IncotermValue = c.IncotermValue
                               }).ToList();

            return lstTimeZone;
        }

        public List<TradelaneCustomer> GetCustomers(int userId)
        {
            var operationzone = UtilityRepository.GetOperationZone();

            List<TradelaneCustomer> customers;
            if (new TradelaneBookingRepository().GetUserRole(userId) == (int)FrayteUserRole.Admin)
            {
                customers = (from r in dbContext.Users
                             join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                             join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                             where
                                ur.RoleId == (int)FrayteUserRole.Customer &&
                                ua.IsBreakBulkBooking == true &&
                                r.IsActive == true &&
                                r.OperationZoneId == operationzone.OperationZoneId
                             select new TradelaneCustomer
                             {
                                 CustomerId = r.UserId,
                                 CustomerName = r.ContactName,
                                 CompanyName = r.CompanyName,
                                 AccountNumber = ua.AccountNo,
                                 EmailId = r.UserEmail,
                                 ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                 CustomerCurrency = ua.CreditLimitCurrencyCode,
                                 OperationZoneId = r.OperationZoneId,
                                 DefaultShipmentType = ua.DefaultShipmentType
                             }).Distinct().OrderBy(p => p.CompanyName).ToList();

                for (int i = 0; i < customers.Count; i++)
                {
                    customers[i].OrderNumber = i + 1;
                }
            }
            else
            {
                customers = (from cs in dbContext.CustomerStaffs
                             join r in dbContext.Users on cs.UserId equals r.UserId
                             join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                             join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                             where
                                cs.CustomerStaffId == userId &&
                                ur.RoleId == (int)FrayteUserRole.Customer &&
                                 ua.IsBreakBulkBooking == true &&
                                cs.IsActive == true &&
                                r.IsActive == true &&
                                r.OperationZoneId == operationzone.OperationZoneId
                             select new TradelaneCustomer
                             {
                                 CustomerId = r.UserId,
                                 CustomerName = r.ContactName,
                                 CompanyName = r.CompanyName,
                                 AccountNumber = ua.AccountNo,
                                 EmailId = r.UserEmail,
                                 ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                 CustomerCurrency = ua.CreditLimitCurrencyCode,
                                 OperationZoneId = r.OperationZoneId,
                             }).Distinct().ToList();

                for (int i = 0; i < customers.Count; i++)
                {
                    customers[i].OrderNumber = i + 1;
                }
            }
            return customers;
        }

        public List<TradelaneCustomer> GetFactories()
        {
            var operationzone = UtilityRepository.GetOperationZone();
            var customers = (from r in dbContext.Users
                             join sc in dbContext.TradelaneShipperCustomers on r.UserId equals sc.CustomerId
                             join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                             join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                             where
                                ur.RoleId == (int)FrayteUserRole.FactoryUser &&
                                ua.IsBreakBulkBooking == true &&
                                r.IsActive == true &&
                                r.OperationZoneId == operationzone.OperationZoneId
                             select new TradelaneCustomer
                             {
                                 CustomerId = r.UserId,
                                 CustomerName = r.ContactName,
                                 CompanyName = r.CompanyName,
                                 AccountNumber = ua.AccountNo,
                                 EmailId = r.UserEmail,
                                 ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                 CustomerCurrency = ua.CreditLimitCurrencyCode,
                                 OperationZoneId = r.OperationZoneId
                             }).Distinct().OrderBy(p => p.CompanyName).ToList();
            return customers;
        }

        public List<CustomFieldModel> GetCustomField(int userId)
        {
            var customField = (from cf in dbContext.CustomerCustomFields
                               where cf.UserId == userId
                               select new CustomFieldModel()
                               {
                                   CustomFieldId = cf.CustomerCustomFieldId,
                                   CustomFieldName = cf.CustomFieldName,
                                   CustomFieldType = cf.CustomFieldType,
                                   CustomFieldValue = cf.CustomFieldValue,
                                   ModuleType = cf.ModuleType
                               }).ToList();
            return customField;
        }

        public BreakBulkShipmentDetail SavePurchaseOrderData(BreakBulkShipmentDetail shipment)
        {
            try
            {
                var userDetail = (from r in dbContext.Users
                                  join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                                  where r.UserId == shipment.CreatedBy
                                  select new
                                  {
                                      UserRole = ur.RoleId,
                                      UserId = r.UserId
                                  }).FirstOrDefault();

                //Step 1: Save ShipFrom
                shipment.ShipFrom.CustomerId = userDetail.UserRole == (int)FrayteUserRole.UserCustomer ? shipment.CreatedBy : shipment.CustomerId;
                shipment.ShipFrom.AddressType = FrayteFromToAddressType.FromAddress;
                SaveDirectShipmentAddress(shipment.ShipFrom);

                //Step 2: Save ShipTo
                shipment.ShipTo.CustomerId = userDetail.UserRole == (int)FrayteUserRole.UserCustomer ? shipment.CreatedBy : shipment.CustomerId;
                shipment.ShipTo.AddressType = FrayteFromToAddressType.ToAddress;
                SaveDirectShipmentAddress(shipment.ShipTo);

                //Step 3: Save PurchesOrder 
                SavePurchaseOrder(shipment);

                //Step 4: Save PurchesOrderDetail
                SavePurchaseOrderDetail(shipment);

                //Step 5: Save CustomFeild
                SaveCustomFeild(shipment);

                //Step 6: Save DefaultShipmentType                        
                SaveCustomerShipmentType(shipment.CustomerId, shipment.PurchaseOrder.DefaultShipmentType);

                shipment.Error = new FratyteError();
                shipment.Error.Status = true;

                return shipment;
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
                throw (new FrayteApiException("EntityError", dbEx));
            }
        }

        public void SaveDirectShipmentAddress(AddressCollection shipFrom)
        {
            try
            {
                if (shipFrom.Country.CountryId > 0)
                {
                    if (shipFrom.AddressBookId == 0)
                    {
                        // Step 1.4 : Add address to addressBook if not exist aleady
                        if (shipFrom.AddressType == FrayteFromToAddressType.FromAddress)
                        {
                            var addressBookData = dbContext.AddressBooks.Where(p => p.Address1 == shipFrom.Address1 &&
                                                                               p.Address2 == shipFrom.Address2 &&
                                                                               p.City == shipFrom.City &&
                                                                               p.State == shipFrom.State &&
                                                                               p.PhoneNo == shipFrom.PhoneNo &&
                                                                               p.Area == shipFrom.Area &&
                                                                               p.CompanyName == shipFrom.CompanyName &&
                                                                               p.ContactFirstName == shipFrom.ContactFirstName &&
                                                                               p.ContactLastName == shipFrom.ContactLastName &&
                                                                               p.CountryId == shipFrom.Country.CountryId &&
                                                                               p.CustomerId == shipFrom.CustomerId &&
                                                                               p.Email == shipFrom.AlertEmail &&
                                                                               p.Zip == shipFrom.PostCode &&
                                                                               p.IsActive == true &&
                                                                               p.FromAddress == true).ToList();
                            if (addressBookData != null && addressBookData.Count > 0)
                            {
                                shipFrom.AddressBookId = addressBookData[0].AddressBookId;
                            }
                            else
                            {
                                AddressBook dbShipFrom = new AddressBook();
                                dbShipFrom.CustomerId = shipFrom.CustomerId;
                                dbShipFrom.FromAddress = true;
                                dbShipFrom.ToAddress = false;
                                dbShipFrom.Address1 = shipFrom.Address1;
                                dbShipFrom.Address2 = shipFrom.Address2;
                                dbShipFrom.Area = shipFrom.Area;
                                dbShipFrom.City = shipFrom.City;
                                dbShipFrom.CompanyName = shipFrom.CompanyName;
                                dbShipFrom.ContactFirstName = shipFrom.ContactFirstName;
                                dbShipFrom.ContactLastName = shipFrom.ContactLastName;
                                dbShipFrom.CountryId = shipFrom.Country.CountryId;
                                dbShipFrom.Email = shipFrom.AlertEmail;
                                dbShipFrom.PhoneNo = shipFrom.PhoneNo;
                                dbShipFrom.State = shipFrom.State;
                                dbShipFrom.Zip = shipFrom.PostCode;
                                dbShipFrom.IsActive = true;
                                dbShipFrom.TableType = FrayteTableType.AddressBook;
                                dbShipFrom.IsFavorites = shipFrom.IsFavorites;
                                dbShipFrom.IsDefault = false;
                                dbContext.AddressBooks.Add(dbShipFrom);
                                if (dbShipFrom != null)
                                {
                                    dbContext.SaveChanges();
                                }
                                shipFrom.AddressBookId = dbShipFrom.AddressBookId;
                            }
                        }
                        else if (shipFrom.AddressType == FrayteFromToAddressType.ToAddress)
                        {
                            var addressBookData = dbContext.AddressBooks.Where(p => p.Address1 == shipFrom.Address1 &&
                                                                               p.Address2 == shipFrom.Address2 &&
                                                                               p.City == shipFrom.City &&
                                                                               p.State == shipFrom.State &&
                                                                               p.PhoneNo == shipFrom.PhoneNo &&
                                                                               p.Area == shipFrom.Area &&
                                                                               p.CompanyName == shipFrom.CompanyName &&
                                                                               p.ContactFirstName == shipFrom.ContactFirstName &&
                                                                               p.ContactLastName == shipFrom.ContactLastName &&
                                                                               p.CountryId == shipFrom.Country.CountryId &&
                                                                               p.CustomerId == shipFrom.CustomerId &&
                                                                               p.Email == shipFrom.AlertEmail &&
                                                                               p.Zip == shipFrom.PostCode &&
                                                                               p.IsActive == true &&
                                                                               p.ToAddress == true).ToList();
                            if (addressBookData != null && addressBookData.Count > 0)
                            {
                                shipFrom.AddressBookId = addressBookData[0].AddressBookId;
                            }
                            else
                            {
                                AddressBook dbShipFrom = new AddressBook();
                                dbShipFrom.CustomerId = shipFrom.CustomerId;
                                dbShipFrom.FromAddress = false;
                                dbShipFrom.ToAddress = true;
                                dbShipFrom.Address1 = shipFrom.Address1;
                                dbShipFrom.Address2 = shipFrom.Address2;
                                dbShipFrom.Area = shipFrom.Area;
                                dbShipFrom.City = shipFrom.City;
                                dbShipFrom.CompanyName = shipFrom.CompanyName;
                                dbShipFrom.ContactFirstName = shipFrom.ContactFirstName;
                                dbShipFrom.ContactLastName = shipFrom.ContactLastName;
                                dbShipFrom.CountryId = shipFrom.Country.CountryId;
                                dbShipFrom.Email = shipFrom.AlertEmail;
                                dbShipFrom.PhoneNo = shipFrom.PhoneNo;
                                dbShipFrom.State = shipFrom.State;
                                dbShipFrom.Zip = shipFrom.PostCode;
                                dbShipFrom.IsActive = true;
                                dbShipFrom.TableType = FrayteTableType.AddressBook;
                                dbShipFrom.IsFavorites = shipFrom.IsFavorites;
                                dbShipFrom.IsDefault = false;
                                dbContext.AddressBooks.Add(dbShipFrom);
                                if (dbShipFrom != null)
                                {
                                    dbContext.SaveChanges();
                                }
                                shipFrom.AddressBookId = dbShipFrom.AddressBookId;
                            }
                        }
                    }
                    else
                    {
                        AddressBook dbShipFrom = dbContext.AddressBooks.Find(shipFrom.AddressBookId);
                        if (dbShipFrom != null)
                        {
                            dbShipFrom.FromAddress = dbShipFrom.ToAddress == true ? false : true;
                            dbShipFrom.ToAddress = dbShipFrom.FromAddress == true ? false : true;
                            dbShipFrom.CustomerId = shipFrom.CustomerId;
                            dbShipFrom.Address1 = shipFrom.Address1;
                            dbShipFrom.Address2 = shipFrom.Address2;
                            dbShipFrom.Area = shipFrom.Area;
                            dbShipFrom.City = shipFrom.City;
                            dbShipFrom.CompanyName = shipFrom.CompanyName;
                            dbShipFrom.ContactFirstName = shipFrom.ContactFirstName;
                            dbShipFrom.ContactLastName = shipFrom.ContactLastName;
                            dbShipFrom.CountryId = shipFrom.Country.CountryId;
                            dbShipFrom.Email = shipFrom.AlertEmail;
                            dbShipFrom.PhoneNo = shipFrom.PhoneNo;
                            dbShipFrom.State = shipFrom.State;
                            dbShipFrom.Zip = shipFrom.PostCode;
                            dbShipFrom.IsActive = true;
                            dbShipFrom.TableType = FrayteTableType.AddressBook;
                            dbShipFrom.IsFavorites = shipFrom.IsFavorites == true ? true : false;
                        }
                        if (dbShipFrom != null)
                        {
                            dbContext.Entry(dbShipFrom).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                        shipFrom.AddressBookId = dbShipFrom.AddressBookId;
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessage = ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage;
                var propertyName = ex.EntityValidationErrors.First().ValidationErrors.First().PropertyName;
                throw (new FrayteApiException("AddressIssue", ex));
            }
        }

        public void SavePurchaseOrder(BreakBulkShipmentDetail obj)
        {
            try
            {
                var operationzone = UtilityRepository.GetOperationZone();
                PurchaseOrder dbPurchaseOrder = new PurchaseOrder();
                if (obj.PurchaseOrder.purchaseOrderId > 0)
                {
                    dbPurchaseOrder = dbContext.PurchaseOrders.Where(x => x.PurchaseOrderId == obj.PurchaseOrder.purchaseOrderId).FirstOrDefault();
                    dbPurchaseOrder.PurchaseOrderId = obj.PurchaseOrder.purchaseOrderId;
                }
                dbPurchaseOrder.PONumber = obj.PurchaseOrder.PONumber;
                dbPurchaseOrder.CareOf = obj.CareOf;
                dbPurchaseOrder.CreatedBy = obj.CreatedBy;
                dbPurchaseOrder.CreatedOnUtc = obj.CreatedOn;
                dbPurchaseOrder.CustomerId = obj.CustomerId;
                dbPurchaseOrder.customField1 = obj.CustomField1.CustomFieldName;
                dbPurchaseOrder.CustomField2 = obj.CustomField2.CustomFieldName;
                dbPurchaseOrder.CustomField3 = obj.CustomField3.CustomFieldName;
                dbPurchaseOrder.DeaprtureAirportId = obj.ShipFrom.Airport.AirportCodeId;
                dbPurchaseOrder.ConsignmentNumber = obj.PurchaseOrder.ConsignmentNumber;
                dbPurchaseOrder.DeliveryType = obj.PurchaseOrder.DeliveryType;
                dbPurchaseOrder.ExFactoryDate = obj.PurchaseOrder.ExFactoryDate.Date;
                dbPurchaseOrder.FactoryUserId = obj.FactoryUserId;
                dbPurchaseOrder.FromAddrressId = obj.ShipFrom.AddressBookId;
                dbPurchaseOrder.ToAddrressId = obj.ShipTo.AddressBookId;
                dbPurchaseOrder.IncotermId = obj.PurchaseOrder.IncotermId.IncotermID;
                dbPurchaseOrder.HubCarrierServiceId = obj.PurchaseOrder.HubCarrierServiceId;
                dbPurchaseOrder.ShipmentType = obj.PurchaseOrder.ShipmentType.ShipmentHandlerMethodDisplay;
                dbPurchaseOrder.StyleName = obj.PurchaseOrder.StyleName;
                dbPurchaseOrder.StyleNumber = obj.PurchaseOrder.StyleNumber;
                dbPurchaseOrder.UpdatedBy = obj.UpdatedBy;
                dbPurchaseOrder.UpdatedOn = obj.UpdatedOn;
                dbPurchaseOrder.ShipmentStatusId = obj.POStatusId;
                dbPurchaseOrder.OperationZoneId = operationzone.OperationZoneId;

                if (obj.PurchaseOrder.purchaseOrderId > 0)
                    dbContext.Entry(dbPurchaseOrder).State = System.Data.Entity.EntityState.Modified;

                else
                    dbContext.PurchaseOrders.Add(dbPurchaseOrder);
                dbContext.SaveChanges();
                obj.PurchaseOrder.purchaseOrderId = dbPurchaseOrder.PurchaseOrderId;
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessage = ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage;
                var propertyName = ex.EntityValidationErrors.First().ValidationErrors.First().PropertyName;
                throw (new FrayteApiException("PurchaseOrderIssue", ex));
            }
        }

        public void SavePurchaseOrderDetail(BreakBulkShipmentDetail obj)
        {
            try
            {
                PurchaseOrderDetail dbPurchaseOrderDetail = new PurchaseOrderDetail();
                foreach (var data in obj.PurchaseOrderDetail)
                {
                    if (data.PurchaseOrderDetailId > 0)
                    {
                        dbPurchaseOrderDetail = dbContext.PurchaseOrderDetails.Where(x => x.PurchaseOrderDetailId == data.PurchaseOrderDetailId).FirstOrDefault();
                        dbPurchaseOrderDetail.PurchaseOrderDetailId = data.PurchaseOrderDetailId;
                    }
                    dbPurchaseOrderDetail.PurchaseOrderId = obj.PurchaseOrder.purchaseOrderId;
                    dbPurchaseOrderDetail.ShipmentStatusId = data.JobStatusId;
                    dbPurchaseOrderDetail.JobNo = data.JobNo;
                    dbPurchaseOrderDetail.JobName = data.JobName;
                    dbPurchaseOrderDetail.OrderQTY = data.OrderQTY;
                    dbPurchaseOrderDetail.Description = data.Description;
                    dbPurchaseOrderDetail.ProductCatalogId = data.ProductCatalogId;

                    if (data.PurchaseOrderDetailId > 0)
                        dbContext.Entry(dbPurchaseOrderDetail).State = System.Data.Entity.EntityState.Modified;
                    else
                        dbContext.PurchaseOrderDetails.Add(dbPurchaseOrderDetail);

                    dbContext.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessage = ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage;
                var propertyName = ex.EntityValidationErrors.First().ValidationErrors.First().PropertyName;
                throw (new FrayteApiException("PurchaseOrderDetailIssue", ex));
            }
        }

        public void SaveCustomFeild(BreakBulkShipmentDetail Obj)
        {
            try
            {
                //CustomerCustomField customField = new CustomerCustomField();
                //if (Obj.CustomField1.CustomFieldId > 0)
                //{
                //    customField = dbContext.CustomerCustomFields.Where(x => x.CustomerCustomFieldId == Obj.CustomField1.CustomFieldId).FirstOrDefault();
                //    customField.CustomerCustomFieldId = Obj.CustomField1.CustomFieldId;
                //}
                //customField.CustomFieldName = Obj.CustomField1.CustomFieldName;
                //customField.CustomFieldType = Obj.CustomField1.CustomFieldType;
                //customField.CustomFieldValue = Obj.CustomField1.CustomFieldValue;
                //customField.ModuleType = Obj.CustomField1.ModuleType;
                //customField.UserId = Obj.CustomerId;

                //if (Obj.CustomField1.CustomFieldId > 0)
                //    dbContext.Entry(customField).State = System.Data.Entity.EntityState.Modified;
                //else
                //    dbContext.CustomerCustomFields.Add(customField);

                //dbContext.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessage = ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage;
                var propertyName = ex.EntityValidationErrors.First().ValidationErrors.First().PropertyName;
                throw (new FrayteApiException("CustomFieldIssue", ex));
            }
        }

        public List<HubService> GetBreakBulkServices(BreakBulkServiceModel serviceObj)
        {
            List<HubService> services = new List<HubService>();
            HubService service = new HubService();
            int hubId = 0;
            if (serviceObj != null)
            {
                if (serviceObj.ToCountryId > 0)
                {
                    var hubCarrierServiceCountry = dbContext.HubCarrierServiceCountries.Where(p => p.CountryId == serviceObj.ToCountryId).FirstOrDefault();
                    if (hubCarrierServiceCountry != null)
                    {
                        hubId = hubCarrierServiceCountry.HubId;
                        services = (from h in dbContext.Hubs
                                    join hc in dbContext.HubCarriers on h.HubId equals hc.HubId
                                    join hcs in dbContext.HubCarrierServices on hc.HubCarrierId equals hcs.HubCarrierId
                                    join hcsc in dbContext.HubCarrierServiceCountries on hcs.HubCarrierServiceId equals hcsc.HubCarrierServiceId
                                    join ccs in dbContext.CustomerHubCarrierServices on hcs.HubCarrierServiceId equals ccs.HubCarrierServiceId
                                    where hcs.IsActive == true && hcsc.CountryId == serviceObj.ToCountryId
                                    && h.HubId == hubId && ccs.CustomerId == serviceObj.CustomerId
                                    select new HubService
                                    {
                                        HubCarrierId = hc.HubCarrierId,
                                        HubCarrierServiceId = hcs.HubCarrierServiceId,
                                        HubCarrier = hc.Carrier,
                                        NetworkCode = hcs.NetworkCode,
                                        HubCarrierDisplay = hc.CarrierService,
                                        CourierAccountNo = hcs.AccountNumber,
                                        RateType = hcs.ServiceType,
                                        RateTypeDisplay = hcs.ServiceTypeDisplay,
                                        TransitTime = hcs.TransitTime,
                                        ActualWeight = serviceObj.TotalWeight,
                                        CarrierLogo = hcs.Logo,
                                        WeightRoundLogic = hcs.WeightRoundLogic
                                    }).ToList();
                    }
                    else
                    {
                        var hubCarrierServiceCountryState = dbContext.HubCarrierServiceCountryStates.Where(p => p.CountryId == serviceObj.ToCountryId && p.State == serviceObj.ToState).FirstOrDefault();

                        if (hubCarrierServiceCountryState != null)
                        {
                            services = (from h in dbContext.Hubs
                                        join hc in dbContext.HubCarriers on h.HubId equals hc.HubId
                                        join hcs in dbContext.HubCarrierServices on hc.HubCarrierId equals hcs.HubCarrierId
                                        join hccs in dbContext.HubCarrierServiceCountryStates on hcs.HubCarrierServiceId equals hccs.HubCarrierServiceId
                                        join ccs in dbContext.CustomerHubCarrierServices on hcs.HubCarrierServiceId equals ccs.HubCarrierServiceId
                                        where hcs.IsActive == true && hccs.CountryId == serviceObj.ToCountryId && hccs.State == serviceObj.ToState &&
                                        ccs.CustomerId == serviceObj.CustomerId && h.HubId == hubId
                                        select new HubService
                                        {
                                            HubCarrierId = hc.HubCarrierId,
                                            HubCarrierServiceId = hcs.HubCarrierServiceId,
                                            HubCarrier = hc.Carrier,
                                            HubCarrierDisplay = hc.CarrierService,
                                            NetworkCode = hcs.NetworkCode,
                                            CourierAccountNo = hcs.AccountNumber,
                                            RateType = hcs.ServiceType,
                                            RateTypeDisplay = hcs.ServiceTypeDisplay,
                                            TransitTime = hcs.TransitTime,
                                            ActualWeight = serviceObj.TotalWeight,
                                            CarrierLogo = hcs.Logo,
                                            WeightRoundLogic = hcs.WeightRoundLogic
                                        }).ToList();
                        }
                        else
                        {
                            var hubCarrierServiceCountryPostCode = dbContext.HubCarrierServiceCountryPostCodes.Where(p => p.CountryId == serviceObj.ToCountryId && p.PostCode == serviceObj.ToPostCode).FirstOrDefault();
                            if (hubCarrierServiceCountryPostCode != null)
                            {
                                services = (from h in dbContext.Hubs
                                            join hc in dbContext.HubCarriers on h.HubId equals hc.HubId
                                            join hcs in dbContext.HubCarrierServices on hc.HubCarrierId equals hcs.HubCarrierId
                                            join ccs in dbContext.CustomerHubCarrierServices on hcs.HubCarrierServiceId equals ccs.HubCarrierServiceId
                                            join hcsp in dbContext.HubCarrierServiceCountryPostCodes on hcs.HubCarrierServiceId equals hcsp.HubCarrierServiceId
                                            where hcs.IsActive == true && hcsp.CountryId == serviceObj.ToCountryId && hcsp.PostCode == serviceObj.ToPostCode
                                            && ccs.CustomerId == serviceObj.CustomerId && h.HubId == hubId
                                            select new HubService
                                            {
                                                HubCarrierId = hc.HubCarrierId,
                                                HubCarrierServiceId = hcs.HubCarrierServiceId,
                                                HubCarrier = hc.Carrier,
                                                HubCarrierDisplay = hc.CarrierService,
                                                NetworkCode = hcs.NetworkCode,
                                                CourierAccountNo = hcs.AccountNumber,
                                                RateType = hcs.ServiceType,
                                                RateTypeDisplay = hcs.ServiceTypeDisplay,
                                                TransitTime = hcs.TransitTime,
                                                ActualWeight = serviceObj.TotalWeight,
                                                CarrierLogo = hcs.Logo,
                                                WeightRoundLogic = hcs.WeightRoundLogic
                                            }).ToList();
                            }
                        }
                    }
                    if (services.Count > 0)
                    {
                        var hub = dbContext.Hubs.Find(hubId);

                        foreach (var item in services)
                        {
                            if (hub != null && hub.CountryId == 228 && serviceObj.ToCountryId == 228)
                            {
                                item.LogisticServiceType = "UK Domestic";
                            }
                            // to do : implement weight round logic 
                            if (item.WeightRoundLogic == "Five")
                            {
                            }
                            else if (item.WeightRoundLogic == "One")
                            {
                            }
                            else
                            {

                            }
                            item.BillingWeight = Math.Round(item.ActualWeight, 2);
                        }
                    }
                }
            }
            return services;
        }

        public FrayteHubAddress GetHubAddress(int countryId, string postCode, string state)
        {
            FrayteHubAddress hubAddress;

            hubAddress = (from r in dbContext.Hubs
                          join hc in dbContext.HubCarrierServiceCountries on r.HubId equals hc.HubId
                          join c in dbContext.Countries on r.CountryId equals c.CountryId
                          where hc.CountryId == countryId
                          select new FrayteHubAddress
                          {
                              ContactFirstName = r.ContactFirstName,
                              ContactLastName = r.ContactLastName,
                              HubId = r.HubId,
                              HubCode = r.Code,
                              Address1 = r.Address,
                              Address2 = r.Address2,
                              Area = r.Area,
                              City = r.City,
                              Country = new FrayteCountryCode
                              {
                                  Code = c.CountryCode,
                                  Code2 = c.CountryCode2,
                                  CountryId = c.CountryId,
                                  CountryPhoneCode = c.CountryPhoneCode,
                                  Name = c.CountryName
                              },
                              AlertEmail = r.Email,
                              CompanyName = r.Name,
                              PhoneNo = r.TelephoneNo,
                              PostCode = r.PostCode,
                              State = r.State
                          }).FirstOrDefault();

            if (hubAddress == null)
            {
                hubAddress = (from r in dbContext.Hubs
                              join hc in dbContext.HubCarrierServiceCountryStates on r.HubId equals hc.HubId
                              join c in dbContext.Countries on r.CountryId equals c.CountryId
                              where hc.CountryId == countryId && hc.State == state
                              select new FrayteHubAddress
                              {
                                  ContactFirstName = r.ContactFirstName,
                                  ContactLastName = r.ContactLastName,
                                  HubId = r.HubId,
                                  HubCode = r.Code,
                                  Address1 = r.Address,
                                  Address2 = r.Address2,
                                  Area = r.Area,
                                  City = r.City,
                                  Country = new FrayteCountryCode
                                  {
                                      Code = c.CountryCode,
                                      Code2 = c.CountryCode2,
                                      CountryId = c.CountryId,
                                      CountryPhoneCode = c.CountryPhoneCode,
                                      Name = c.CountryName
                                  },
                                  AlertEmail = r.Email,
                                  CompanyName = r.Name,
                                  PhoneNo = r.TelephoneNo,
                                  PostCode = r.PostCode,
                                  State = r.State
                              }).FirstOrDefault();
            }
            if (hubAddress == null)
            {
                hubAddress = (from r in dbContext.Hubs
                              join hc in dbContext.HubCarrierServiceCountryPostCodes on r.HubId equals hc.HubCarrierServiceId
                              join c in dbContext.Countries on r.CountryId equals c.CountryId
                              where hc.CountryId == countryId && hc.PostCode == postCode
                              select new FrayteHubAddress
                              {
                                  ContactFirstName = r.ContactFirstName,
                                  ContactLastName = r.ContactLastName,
                                  HubId = r.HubId,
                                  HubCode = r.Code,
                                  Address1 = r.Address,
                                  Address2 = r.Address2,
                                  Area = r.Area,
                                  City = r.City,
                                  Country = new FrayteCountryCode
                                  {
                                      Code = c.CountryCode,
                                      Code2 = c.CountryCode2,
                                      CountryId = c.CountryId,
                                      CountryPhoneCode = c.CountryPhoneCode,
                                      Name = c.CountryName
                                  },
                                  AlertEmail = r.Email,
                                  CompanyName = r.Name,
                                  PhoneNo = r.TelephoneNo,
                                  PostCode = r.PostCode,
                                  State = r.State
                              }).FirstOrDefault();
            }
            if (hubAddress == null)
            {
                hubAddress = new FrayteHubAddress();
            }
            return hubAddress;
        }

        public List<BBKAirport> GetAirlines(int countryId)
        {
            List<BBKAirport> list = (from r in dbContext.Airports
                                     where r.CountryId == countryId
                                     select new BBKAirport
                                     {
                                         AirportCodeId = r.AirportCodeId,
                                         CountrtId = r.CountryId,
                                         AirportCode = r.AirportCode,
                                         AirportName = r.AirportName
                                     }).ToList();
            return list;
        }

        public string GenerateConsignmentNumber(int userId)
        {
            try
            {
                var userAcntNo = dbContext.UserAdditionals.Where(x => x.UserId == userId).FirstOrDefault();
                string consignmentNumber;
                Random r = new Random();
                int randNum = r.Next(1000000);
                string sixDigitNumber = randNum.ToString("D6");
                consignmentNumber = (userAcntNo.AccountNo).Substring(0, 3) + ModuleNumber.BBK + sixDigitNumber;
                return consignmentNumber;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool SaveCustomerCustomField(customerCustomFieldModel CustomField)
        {
            try
            {
                CustomerCustomField obj = new CustomerCustomField();
                if (CustomField.CustomerCustomFieldId == 0)
                {
                    obj.CustomFieldName = CustomField.customFieldName;
                    obj.CustomFieldType = CustomField.CustomFieldType;
                    obj.CustomFieldValue = CustomField.CustomFieldValue;
                    obj.UserId = CustomField.UserId;
                    obj.ModuleType = CustomField.ModuleType;
                    dbContext.CustomerCustomFields.Add(obj);
                    dbContext.SaveChanges();
                }
                else
                {
                    obj.CustomerCustomFieldId = CustomField.CustomerCustomFieldId;
                    obj.CustomFieldName = CustomField.customFieldName;
                    obj.CustomFieldType = CustomField.CustomFieldType;
                    obj.CustomFieldValue = CustomField.CustomFieldValue;
                    obj.UserId = CustomField.UserId;
                    obj.ModuleType = CustomField.ModuleType;
                    dbContext.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }

                return true;
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessage = ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage;
                var propertyName = ex.EntityValidationErrors.First().ValidationErrors.First().PropertyName;
                return false;
            }
        }

        public bool SaveCustomerShipmentType(int customerId, string DefaultShipmentType)
        {
            try
            {
                var result = dbContext.UserAdditionals.Where(s => s.UserId == customerId).FirstOrDefault();
                if (result != null)
                {
                    result.DefaultShipmentType = DefaultShipmentType;
                    dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessage = ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage;
                var propertyName = ex.EntityValidationErrors.First().ValidationErrors.First().PropertyName;
                return false;
            }
        }

        public BreakBulkShipmentDetail GetBreakBulkBookingDetail(int PurchaseOrderId)
        {
            BreakBulkShipmentDetail dbDetail = new BreakBulkShipmentDetail();
            dbDetail.PurchaseOrder = new FraytePurchaseOrder();
            dbDetail.PurchaseOrder.purchaseOrderId = PurchaseOrderId;

            //Step 1: Get Shipment Detail
            GetPurchsaeOrderDetail(dbDetail);

            //Step 2: Get Shipment Packages Detail
            GetPurchaseOrderPackagesDetail(dbDetail);

            //Step 3: From Address
            GetPurchsaeOrderFromAddress(dbDetail);

            //Step 4: To Address
            GetPurchsaeOrderToAddress(dbDetail);

            return dbDetail;
        }

        internal void GetPurchsaeOrderDetail(BreakBulkShipmentDetail dbDetail)
        {
            PurchaseOrder result = dbContext.PurchaseOrders.Find(dbDetail.PurchaseOrder.purchaseOrderId);
            if (result != null)
            {
                var incoterm = (from i in dbContext.Incoterms where i.IncotermId == result.IncotermId select i).FirstOrDefault();

                dbDetail.CustomerId = result.CustomerId;
                dbDetail.FactoryUserId = result.FactoryUserId;
                dbDetail.ShipFrom = new AddressCollection();
                dbDetail.ShipFrom.AddressBookId = result.FromAddrressId;
                dbDetail.ShipTo = new AddressCollection();
                dbDetail.ShipTo.AddressBookId = result.ToAddrressId;
                dbDetail.ShipFrom.Airport = new FrayteAirport();
                dbDetail.ShipFrom.Airport.AirportCodeId = result.DeaprtureAirportId;
                dbDetail.PurchaseOrder.HubCarrierServiceId = result.HubCarrierServiceId;
                dbDetail.CustomField1 = new CustomFieldModel();
                dbDetail.CustomField1.CustomFieldName = result.customField1;
                dbDetail.CustomField1.CustomFieldValue = result.customField1;
                dbDetail.CustomField2 = new CustomFieldModel();
                dbDetail.CustomField2.CustomFieldName = result.CustomField2;
                dbDetail.CustomField2.CustomFieldValue = result.CustomField2;
                dbDetail.CustomField3 = new CustomFieldModel();
                dbDetail.CustomField3.CustomFieldName = result.CustomField3;
                dbDetail.CustomField3.CustomFieldValue = result.CustomField3;
                dbDetail.CreatedBy = result.CreatedBy;
                dbDetail.CareOf = result.CareOf;
                dbDetail.POStatusId = result.ShipmentStatusId;

                dbDetail.PurchaseOrder = new FraytePurchaseOrder();
                dbDetail.PurchaseOrder.ConsignmentNumber = result.ConsignmentNumber;
                dbDetail.PurchaseOrder.DefaultShipmentType = result.ShipmentType;
                dbDetail.PurchaseOrder.PONumber = result.PONumber;
                dbDetail.PurchaseOrder.purchaseOrderId = result.PurchaseOrderId;
                dbDetail.PurchaseOrder.StyleName = result.StyleName;
                dbDetail.PurchaseOrder.StyleNumber = result.StyleNumber;
                dbDetail.PurchaseOrder.DeliveryType = result.DeliveryType;
                dbDetail.PurchaseOrder.ExFactoryDate = result.ExFactoryDate;
                dbDetail.PurchaseOrder.IncotermId = new FrayteIncoterm();
                if (incoterm != null)
                {
                    dbDetail.PurchaseOrder.IncotermId.IncotermID = incoterm.IncotermId;
                    dbDetail.PurchaseOrder.IncotermId.IncotermCode = incoterm.IncotermCode;
                    dbDetail.PurchaseOrder.IncotermId.IncotermValue = incoterm.IncotermValue;
                    dbDetail.PurchaseOrder.IncotermId.IncotermDisplayValue = incoterm.IncotermDisplayValue;
                    dbDetail.PurchaseOrder.IncotermId.ShipmentType = incoterm.ShipmentHandlerMethodType;
                }

            }
        }

        internal void GetPurchaseOrderPackagesDetail(BreakBulkShipmentDetail dbDetail)
        {
            FraytePurchaseOrderDetail dbPackage;
            dbDetail.PurchaseOrderDetail = new List<FraytePurchaseOrderDetail>();

            var result = (from PO in dbContext.PurchaseOrders
                          join POD in dbContext.PurchaseOrderDetails on PO.PurchaseOrderId equals POD.PurchaseOrderId
                          where PO.PurchaseOrderId == dbDetail.PurchaseOrder.purchaseOrderId
                          select new
                          {
                              PurchaseOrderDetailId = 0,
                              PurchaseOrderId = POD.PurchaseOrderId,
                              ProductCatalogId = POD.ProductCatalogId,
                              POStatusId = 0,
                              JobNo = POD.JobNo,
                              JobName = POD.JobName,
                              Description = POD.Description,
                              OrderQTY = POD.OrderQTY
                          }).ToList();

            if (result != null && result.Count > 0)
            {
                foreach (var Obj in result)
                {
                    dbPackage = new FraytePurchaseOrderDetail();
                    dbPackage.JobName = Obj.JobName;
                    dbPackage.JobNo = Obj.JobNo;
                    dbPackage.OrderQTY = Obj.OrderQTY;
                    dbPackage.JobStatusId = Obj.POStatusId;
                    dbPackage.ProductCatalogId = Obj.ProductCatalogId;
                    dbPackage.PurchaseOrderDetailId = Obj.PurchaseOrderDetailId;
                    dbPackage.PurchaseOrderId = Obj.PurchaseOrderId;
                    dbPackage.Description = Obj.Description;
                    dbDetail.PurchaseOrderDetail.Add(dbPackage);
                }
            }
            else
            {
                foreach (var Obj in result)
                {
                    dbPackage = new FraytePurchaseOrderDetail();
                    dbPackage.JobName = Obj.JobName;
                    dbPackage.JobNo = Obj.JobNo;
                    dbPackage.OrderQTY = Obj.OrderQTY;
                    dbPackage.JobStatusId = Obj.POStatusId;
                    dbPackage.ProductCatalogId = Obj.ProductCatalogId;
                    dbPackage.PurchaseOrderDetailId = Obj.PurchaseOrderDetailId;
                    dbPackage.PurchaseOrderId = Obj.PurchaseOrderId;
                    dbPackage.Description = Obj.Description;
                    dbDetail.PurchaseOrderDetail.Add(dbPackage);
                }
            }
        }

        internal void GetPurchsaeOrderFromAddress(BreakBulkShipmentDetail dbDetail)
        {
            AddressBook dbShipFrom = dbContext.AddressBooks.Find(dbDetail.ShipFrom.AddressBookId);
            if (dbShipFrom != null)
            {
                var airportid = dbDetail.ShipFrom.Airport.AirportCodeId;
                dbDetail.ShipFrom = new AddressCollection();
                dbDetail.ShipFrom.AddressBookId = dbShipFrom.AddressBookId;
                dbDetail.ShipFrom.CustomerId = dbShipFrom.CustomerId;
                dbDetail.ShipFrom.Address1 = dbShipFrom.Address1;
                dbDetail.ShipFrom.Address2 = dbShipFrom.Address2;
                dbDetail.ShipFrom.Area = dbShipFrom.Area;
                dbDetail.ShipFrom.City = dbShipFrom.City;
                dbDetail.ShipFrom.CompanyName = dbShipFrom.CompanyName;
                dbDetail.ShipFrom.ContactFirstName = dbShipFrom.ContactFirstName;
                dbDetail.ShipFrom.ContactLastName = dbShipFrom.ContactLastName;

                var country = (from c in dbContext.Countries where c.CountryId == dbShipFrom.CountryId select c).FirstOrDefault();
                if (country != null)
                {
                    dbDetail.ShipFrom.Country = new FrayteCountryCode();
                    dbDetail.ShipFrom.Country.CountryId = country.CountryId;
                    dbDetail.ShipFrom.Country.Code = country.CountryCode;
                    dbDetail.ShipFrom.Country.Code2 = country.CountryCode2;
                    dbDetail.ShipFrom.Country.CountryPhoneCode = country.CountryPhoneCode;
                }

                var airport = (from ar in dbContext.Airports where ar.AirportCodeId == airportid select ar).FirstOrDefault();
                if (airport != null)
                {
                    dbDetail.ShipFrom.Airport = new FrayteAirport();
                    dbDetail.ShipFrom.Airport.AirportCodeId = airport.AirportCodeId;
                    dbDetail.ShipFrom.Airport.AirportName = airport.AirportCode;
                }

                dbDetail.ShipFrom.AlertEmail = dbShipFrom.Email;
                dbDetail.ShipFrom.PhoneNo = dbShipFrom.PhoneNo;
                dbDetail.ShipFrom.State = dbShipFrom.State;
                dbDetail.ShipFrom.PostCode = dbShipFrom.Zip;
                dbDetail.ShipFrom.IsFavorites = dbShipFrom.IsFavorites == true ? true : false;
            }
        }

        internal void GetPurchsaeOrderToAddress(BreakBulkShipmentDetail dbDetail)
        {
            AddressBook dbShipTo = dbContext.AddressBooks.Find(dbDetail.ShipTo.AddressBookId);
            if (dbShipTo != null)
            {
                dbDetail.ShipTo = new AddressCollection();
                dbDetail.ShipTo.AddressBookId = dbShipTo.AddressBookId;
                dbDetail.ShipTo.CustomerId = dbShipTo.CustomerId;
                dbDetail.ShipTo.Address1 = dbShipTo.Address1;
                dbDetail.ShipTo.Address2 = dbShipTo.Address2;
                dbDetail.ShipTo.Area = dbShipTo.Area;
                dbDetail.ShipTo.City = dbShipTo.City;
                dbDetail.ShipTo.CompanyName = dbShipTo.CompanyName;
                dbDetail.ShipTo.ContactFirstName = dbShipTo.ContactFirstName;
                dbDetail.ShipTo.ContactLastName = dbShipTo.ContactLastName;

                var country = (from c in dbContext.Countries where c.CountryId == dbShipTo.CountryId select c).FirstOrDefault();
                if (country != null)
                {
                    dbDetail.ShipTo.Country = new FrayteCountryCode();
                    dbDetail.ShipTo.Country.CountryId = country.CountryId;
                    dbDetail.ShipTo.Country.Code = country.CountryCode;
                    dbDetail.ShipTo.Country.Code2 = country.CountryCode2;
                    dbDetail.ShipTo.Country.CountryPhoneCode = country.CountryPhoneCode;
                }

                dbDetail.ShipTo.AlertEmail = dbShipTo.Email;
                dbDetail.ShipTo.PhoneNo = dbShipTo.PhoneNo;
                dbDetail.ShipTo.State = dbShipTo.State;
                dbDetail.ShipTo.PostCode = dbShipTo.Zip;
                dbDetail.ShipTo.IsFavorites = dbShipTo.IsFavorites == true ? true : false;
            }
        }

        public List<FraytePOviewPurchaseOrder> TrackPOPurchaseOrder(FraytePurchaseOrderTrack track)
        {
            List<FraytePOviewPurchaseOrder> list = new List<FraytePOviewPurchaseOrder>();

            int SkipRows = 0;
            SkipRows = (track.CurrentPage - 1) * track.TakeRows;
            DateTime? fromdate;
            DateTime? todate;

            if (track.FromDate.HasValue)
            {
                fromdate = track.FromDate.Value.Date;
            }
            else
            {
                fromdate = track.FromDate;
            }

            if (track.ToDate.HasValue)
            {
                todate = track.ToDate.Value.Date;
            }
            else
            {
                todate = track.ToDate;
            }

            track.OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;

            var result = dbContext.spGet_BKKPoViewPurchaseOrder(track.PoNo, track.JobNo, track.styleNo, todate, SkipRows, track.TakeRows, fromdate,
                                                                track.CustomerId, track.MAWB, track.OperationZoneId, track.ShipmentStatusId, track.TrackingNo,
                                                                track.CustomField, track.FrayteNumber).ToList();

            try
            {
                FraytePOviewPurchaseOrder po;
                if (result != null && result.Count > 0)
                {
                    foreach (spGet_BKKPoViewPurchaseOrder_Result sp in result)
                    {
                        po = new FraytePOviewPurchaseOrder();
                        po.AirportCode = sp.AirportCode;
                        po.ExFactoryDate = sp.ExFactoryDate;
                        po.CreatedOnUtc = sp.CreatedOnUtc;
                        po.PONumber = sp.PONumber;
                        po.POStatus = sp.POStatus;
                        po.PurchaseOrderId = sp.PurchaseOrderId;
                        po.StyleName = sp.StyleName;
                        po.StyleNumber = sp.StyleNumber;
                        po.TotalRows = sp.TotalRows;
                        po.jobs = new List<Jobview>();
                        Jobview jj;
                        {
                            jj = new Jobview();
                            jj.Carrier = sp.Carrier;
                            jj.JobName = sp.JobName;
                            jj.JobNo = sp.JobNo;
                            jj.JobStatus = sp.JobStatus;
                            jj.OrderQTY = sp.OrderQTY;
                            jj.ServiceType = sp.ServiceType;
                            jj.TrackingNo = sp.TrackingNo;
                        };
                        po.jobs.Add(jj);
                        list.Add(po);
                    }
                }

                list = list.GroupBy(a => new { a.PurchaseOrderId }).Select(d => new FraytePOviewPurchaseOrder
                {
                    PurchaseOrderId = d.Key.PurchaseOrderId,
                    PONumber = d.FirstOrDefault().PONumber,
                    POStatus = d.FirstOrDefault().POStatus,
                    StyleName = d.FirstOrDefault().StyleName,
                    StyleNumber = d.FirstOrDefault().StyleNumber,
                    TotalRows = d.FirstOrDefault().TotalRows,
                    ExFactoryDate = d.FirstOrDefault().ExFactoryDate,
                    AirportCode = d.FirstOrDefault().AirportCode,
                    CreatedOnUtc = d.FirstOrDefault().CreatedOnUtc,
                    jobs = d.Select(a => new Jobview
                    {
                        JobNo = a.jobs.Select(s => s.JobNo).FirstOrDefault(),
                        JobName = a.jobs.Select(s => s.JobName).FirstOrDefault(),
                        JobStatus = a.jobs.Select(s => s.JobStatus).FirstOrDefault(),
                        Carrier = a.jobs.Select(s => s.Carrier).FirstOrDefault(),
                        OrderQTY = a.jobs.Select(s => s.OrderQTY).FirstOrDefault(),
                        ServiceType = a.jobs.Select(s => s.ServiceType).FirstOrDefault(),
                        TrackingNo = a.jobs.Select(s => s.TrackingNo).FirstOrDefault()
                    }).ToList()

                }).ToList();
                return list;
            }
            catch (Exception ex)
            {

            }
            return list;
        }

        public List<FrayteJobViewPurchaseOrder> TrackJobPurchaseOrder(FraytePurchaseOrderTrack track)
        {
            List<FrayteJobViewPurchaseOrder> list = new List<FrayteJobViewPurchaseOrder>();

            int SkipRows = 0;
            SkipRows = (track.CurrentPage - 1) * track.TakeRows;
            DateTime? fromdate;
            DateTime? todate;

            if (track.FromDate.HasValue)
            {
                fromdate = track.FromDate.Value.Date;
            }
            else
            {
                fromdate = track.FromDate;
            }

            if (track.ToDate.HasValue)
            {
                todate = track.ToDate.Value.Date;
            }
            else
            {
                todate = track.ToDate;
            }

            track.OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;

            var result = dbContext.spGet_BKKJobViewPurchaseOrder(track.PoNo, track.JobNo, track.styleNo, todate, SkipRows, track.TakeRows, fromdate,
                                                                 track.CustomerId, track.MAWB, track.OperationZoneId, track.ShipmentStatusId, track.TrackingNo,
                                                                 track.CustomField, track.FrayteNumber).ToList();

            try
            {
                FrayteJobViewPurchaseOrder po;
                if (result != null && result.Count > 0)
                {
                    foreach (spGet_BKKJobViewPurchaseOrder_Result sp in result)
                    {
                        po = new FrayteJobViewPurchaseOrder();
                        po.JobNo = sp.JobNo;
                        po.CreatedOnUtc = sp.CreatedOnUtc;
                        po.JobStatus = sp.JobStatus;
                        po.OrderQTY = sp.OrderQTY;
                        po.PONumber = sp.PONumber;
                        po.PurchaseOrderDetailId = sp.PurchaseOrderDetailId;
                        po.ServiceType = sp.ServiceType;
                        po.StyleNumber = sp.StyleNumber;
                        po.TrackingNo = sp.TrackingNo;
                        po.Carrier = sp.Carrier;
                        po.TotalRows = sp.TotalRows;
                        list.Add(po);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {

            }
            return list;
        }

        public List<BBKShipmentStatus> GetBBKShipmentStatusList(string BookingType)
        {
            var result = (from std in dbContext.ShipmentStatus
                          where std.BookingType == BookingType
                          select new BBKShipmentStatus
                          {
                              BBKStatus = std.BBKStatusFor,

                              ShipmentStatusId = std.ShipmentStatusId,
                              StatusName = std.StatusName,
                              BookingType = std.BookingType,
                              DisplayStatusName = std.DisplayStatusName

                          }).ToList();



            return result;
        }
    }
}
