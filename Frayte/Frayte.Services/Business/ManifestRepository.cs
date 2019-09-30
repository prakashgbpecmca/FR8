using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Spire.Barcode;
using System.Drawing;
using System.Web;
using Frayte.Services.Utility;
using System.IO;
using System.Data;
using System.Data.OleDb;
using Frayte.Services.Models.Express;
using System.Data.Entity.Validation;
using System.Web.Hosting;
using System.Runtime.InteropServices;


namespace Frayte.Services.Business
{
    public class ManifestRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public MainfestDetailModel GetNonManifestedShipments(int OperationZoneId, int UserId, int CreatedBy, string moduleType, string subModuleType)
        {
            MainfestDetailModel MDM = new MainfestDetailModel();
            MDM.Shipments = new List<FrayteUserDirectShipment>();
            MDM.Customers = new List<FrayteCustomer>();
            var userDetail = (from r in dbContext.Users
                              join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                              where r.UserId == CreatedBy
                              select new
                              {
                                  RoleId = ur.RoleId
                              }).FirstOrDefault();

            var list = dbContext.spGet_GetNonManifestedShipments(OperationZoneId, UserId, CreatedBy, userDetail.RoleId, moduleType, subModuleType).ToList();

            if (list != null && list.Count > 0)
            {
                FrayteUserDirectShipment frayte;
                foreach (var detail in list)
                {
                    frayte = new FrayteUserDirectShipment();
                    frayte.ShipmentId = detail.DirectShipmentId;
                    frayte.ShipmentCode = "";
                    frayte.Customer = dbContext.Users.Where(a => a.UserId == detail.CustomerId).FirstOrDefault().CompanyName != null ? dbContext.Users.Where(a => a.UserId == detail.CustomerId).FirstOrDefault().CompanyName : "";
                    frayte.ShippedFromCompany = detail.FromCompany;
                    frayte.ShippedToCompany = detail.ToCompany;
                    frayte.ShippingBy = detail.LogisticCompany;
                    frayte.DisplayName = detail.LogisticCompanyDisplay;
                    frayte.RateType = detail.RateType;
                    frayte.RateTypeDisplay = detail.RateTypeDisplay;
                    frayte.ShippingDate = detail.CreatedOn;
                    frayte.Reference1 = detail.Reference1;
                    frayte.FrayteNumber = detail.FrayteNumber;
                    frayte.Status = detail.StatusName;
                    frayte.TrackingNo = detail.TrackingNo;
                    frayte.TotalPieces = detail.TotalCarton != null ? detail.TotalCarton.Value : 0;
                    frayte.TotalWeight = detail.ChargeableWeight != null ? detail.ChargeableWeight.Value : 0;
                    MDM.Shipments.Add(frayte);
                }

                var CustomerList = list.GroupBy(a => a.CustomerId).ToList();
                foreach (var Cus in CustomerList)
                {
                    //FrayteCustomer FC = new FrayteCustomer();
                    var Cust = (from r in dbContext.Users
                                join ur in dbContext.UserAdditionals on r.UserId equals ur.UserId
                                where r.UserId == Cus.Key
                                select new FrayteCustomer
                                {
                                    ContactName = r.ContactName,
                                    AccountNumber = ur.AccountNo,
                                    CompanyName = r.CompanyName,
                                    UserId = r.UserId

                                }
                            ).FirstOrDefault();
                    MDM.Customers.Add(Cust);
                }

            }
            return MDM;
        }

        public int GetUnmanifestedJobCount(int userId)
        {
            try
            {
                int count = 0;
                var OperationZone = UtilityRepository.GetOperationZone();
                var userRole = (from r in dbContext.Users
                                join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                                where r.UserId == userId
                                select new
                                {
                                    RoleId = ur.RoleId
                                }).FirstOrDefault();

                if (userRole.RoleId == (int)FrayteUserRole.Staff || userRole.RoleId == (int)FrayteUserRole.Admin)
                {
                    var customers = (from r in dbContext.Users
                                     join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                                     join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                                     where ur.RoleId == (int)FrayteUserRole.Customer && r.IsActive == true && r.OperationZoneId == OperationZone.OperationZoneId
                                     select new
                                     {
                                         UserId = r.UserId,
                                         IsDirectBooking = ua.IsDirectBooking,
                                         IseCommerceBooking = ua.IsECommerce
                                     }).ToList();

                    foreach (var item in customers)
                    {
                        if (item.IseCommerceBooking.HasValue && item.IseCommerceBooking.Value)
                        {
                            var eCommShipments = (from r in dbContext.eCommerceShipments
                                                  where ((r.ManifestId == 0 || r.ManifestId == null) && r.OpearionZoneId == OperationZone.OperationZoneId)
                                                  select r).ToList();
                            count += eCommShipments.Where(p => p.CustomerId == p.CustomerId).ToList().Count;

                        }
                        if (item.IsDirectBooking.HasValue && item.IsDirectBooking.Value)
                        {
                            var directShipments = (from r in dbContext.DirectShipments
                                                   where ((r.ManifestId == 0 || r.ManifestId == null) && r.OpearionZoneId == OperationZone.OperationZoneId && (r.ShipmentStatusId == (int)FrayteShipmentStatus.Current || r.ShipmentStatusId == (int)FrayteShipmentStatus.Past))
                                                   select r).ToList();
                            count += directShipments.Where(p => p.CustomerId == item.UserId).ToList().Count;
                        }
                    }
                }
                else
                {
                    var userDetail = (from r in dbContext.Users
                                      join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                                      where r.UserId == userId
                                      select new
                                      {
                                          RoleId = ur.RoleId
                                      }).FirstOrDefault();

                    if (userDetail != null)
                    {
                        int customerId = 0;
                        if (userDetail.RoleId == (int)FrayteUserRole.UserCustomer)
                        {
                            customerId = dbContext.UserCustomers.Where(p => p.CustomerId == userId).FirstOrDefault().UserId;
                        }

                        var customerAditional = dbContext.UserAdditionals.Where(p => p.UserId == (userDetail.RoleId == (int)FrayteUserRole.UserCustomer ? customerId : userId)).FirstOrDefault();
                        if (customerAditional.IsECommerce.HasValue && customerAditional.IsECommerce.Value)
                        {
                            var eCommShipments = (from r in dbContext.eCommerceShipments
                                                  where ((r.ManifestId == 0 || r.ManifestId == null) && r.OpearionZoneId == OperationZone.OperationZoneId)
                                                  select r).ToList();
                            count += eCommShipments.Where(p => p.CustomerId == userId).ToList().Count;
                        }
                        if (customerAditional.IsDirectBooking.HasValue && customerAditional.IsDirectBooking.Value)
                        {
                            if (userDetail.RoleId == (int)FrayteUserRole.UserCustomer)
                            {

                                var directShipments = (from r in dbContext.DirectShipments
                                                       where ((r.ManifestId == 0 || r.ManifestId == null) && r.OpearionZoneId == OperationZone.OperationZoneId && (r.ShipmentStatusId == (int)FrayteShipmentStatus.Current || r.ShipmentStatusId == (int)FrayteShipmentStatus.Past))
                                                       select r).ToList();
                                count += directShipments.Where(p => p.CreatedBy == userId && p.CustomerId == customerId).ToList().Count;
                            }
                            else
                            {
                                var directShipments = (from r in dbContext.DirectShipments
                                                       where ((r.ManifestId == 0 || r.ManifestId == null) && r.OpearionZoneId == OperationZone.OperationZoneId && (r.ShipmentStatusId == (int)FrayteShipmentStatus.Current || r.ShipmentStatusId == (int)FrayteShipmentStatus.Past))
                                                       select r).ToList();
                                count += directShipments.Where(p => p.CustomerId == userId).ToList().Count;
                            }
                        }
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return 0;
            }
        }

        public FrayteResult SetETAETD(ETAETDManifest manifestETAETD)
        {
            FrayteResult result = new FrayteResult();

            try
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(manifestETAETD.EstimatedDateofDelivery.Value.ToString() + manifestETAETD.EstimatedDateofDelivery.Value.Kind.ToString()));
                if (manifestETAETD != null && manifestETAETD.Shipments != null && manifestETAETD.Shipments.Count > 0)
                {
                    foreach (var shipment in manifestETAETD.Shipments)
                    {
                        var data = dbContext.eCommerceShipments.Find(shipment.ShipmentId);
                        if (data != null)
                        {
                            data.EstimatedDateofArrival = UtilityRepository.ConvertDateTimetoUniversalTime(manifestETAETD.EstimatedDateofArrival.Value);
                            data.EstimatedDateofDelivery = UtilityRepository.ConvertDateTimetoUniversalTime(manifestETAETD.EstimatedDateofDelivery.Value);
                            //data.EstimatedTimeofArrival = UtilityRepository.GetTimeFromString(manifestETAETD.EstimatedTimeofArrival);
                            //data.EstimatedTimeofDelivery = UtilityRepository.GetTimeFromString(manifestETAETD.EstimatedTimeofDelivery);
                            data.EstimatedTimeofArrival = UtilityRepository.TimeSpanConversion(data.EstimatedDateofArrival.Value.Hour, data.EstimatedDateofArrival.Value.Minute, data.EstimatedDateofArrival.Value.Second);
                            data.EstimatedTimeofDelivery = UtilityRepository.TimeSpanConversion(data.EstimatedDateofDelivery.Value.Hour, data.EstimatedDateofDelivery.Value.Minute, data.EstimatedDateofDelivery.Value.Second);
                            dbContext.SaveChanges();
                            result.Status = true;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Status = false;
                return result;
            }

        }

        public FrayteResult CreateManifest(FrayteManifestShipment manifestShipments)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                // group the shipments based on CourierCompany 
                var dasta = manifestShipments.DirectShipments.Select(p => p.ShippingBy).Distinct().ToList();
                if (dasta != null && dasta.Count > 0)
                {
                    foreach (var a in dasta)
                    {
                        Manifest manifest = new Manifest();
                        manifest.ModuleType = manifestShipments.ModuleType;
                        manifest.CreatedOn = DateTime.UtcNow;
                        manifest.CreatedBy = manifestShipments.CreatedBy;
                        manifest.CustomerId = manifestShipments.UserId;
                        manifest.SubModuleType = manifestShipments.SubModuleType;
                        dbContext.Manifests.Add(manifest);
                        dbContext.SaveChanges();
                        manifest.ManifestName = "MNUK-" + DateTime.UtcNow.Year.ToString().Substring(2, 2) + DateTime.UtcNow.ToString("MM") + GetFormattedManifestId(manifest.ManifestId);
                        dbContext.Entry(manifest).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        var ships = manifestShipments.DirectShipments.Where(p => p.ShippingBy == a).ToList();
                        CreateDirectShipmentsManifest(manifest.ManifestId, manifestShipments.ModuleType, ships);
                        CreateManifestBarCode(manifest);
                    }
                }
                result.Status = true;
            }
            catch (Exception ex)
            {
                result.Status = false;
            }

            return result;
        }

        public FrayteUserManifest IsManifestSupprt(int userId)
        {
            FrayteUserManifest userManifest = new FrayteUserManifest();
            try
            {
                var OperationZone = UtilityRepository.GetOperationZone();
                var opzDetail = dbContext.OperationZones.Find(OperationZone.OperationZoneId);
                var userDetail = dbContext.UserRoles.Where(p => p.UserId == userId).FirstOrDefault();
                if (opzDetail != null && userDetail != null)
                {
                    if (opzDetail.IsManifestSupport && userDetail.RoleId == (int)FrayteUserRole.Customer)
                    {
                        var data = new CustomerRepository().GetCustomerModules(userId);

                        if (data != null)
                        {
                            userManifest.Services = new List<CustomerService>();
                            if (data.IsDirectBooking)
                            {
                                userManifest.ManifestSupport = true;
                                userManifest.Services.Add(new CustomerService { OrderNumber = 1, ServiceName = "DirectBooking" });
                            }
                            if (data.IseCommerceBooking)
                            {
                                userManifest.ManifestSupport = true;
                                userManifest.Services.Add(new CustomerService { OrderNumber = 2, ServiceName = "eCommerce" });
                            }
                            if (!data.IseCommerceBooking && !data.IsDirectBooking)
                            {
                                userManifest.ManifestSupport = false;
                            }
                        }
                        else
                        {
                            userManifest.ManifestSupport = false;
                        }
                    }
                    else if ((opzDetail.IsManifestSupport && userDetail.RoleId == (int)FrayteUserRole.Admin) || (opzDetail.IsManifestSupport && userDetail.RoleId == (int)FrayteUserRole.Staff))
                    {
                        userManifest.ManifestSupport = true;
                    }
                }
                else
                {
                    userManifest.ManifestSupport = false;
                }
            }
            catch (Exception ex)
            {
                userManifest.ManifestSupport = false;
            }
            return userManifest;

        }

        public List<ManifestReport> DownLoadManifest(int ManifestId, string moduleType, int UserId, int RoleId)
        {
            List<ManifestReport> _manifest = new List<ManifestReport>();
            try
            {
                var detail = (from uu in dbContext.Users
                              join tz in dbContext.Timezones on uu.TimezoneId equals tz.TimezoneId
                              where uu.UserId == UserId
                              select new
                              {
                                  uu.ContactName,
                                  tz.OffsetShort
                              }).FirstOrDefault();

                //To Do : DownLoad Manifest -> call to dev-express report
                if (moduleType == FrayteShipmentServiceType.DirectBooking || moduleType == FrayteShipmentServiceType.eCommerce)
                {
                    if (detail != null)
                    {
                        var data = dbContext.spGet_DirectBookingManifestDetail(ManifestId, moduleType, RoleId).ToList();

                        if (data != null && data.Count > 0)
                        {
                            ManifestReport report;
                            foreach (var Obj in data)
                            {
                                report = new ManifestReport();
                                report.ManifestFileName = Obj.ManifestName;
                                report.ManifestName = Obj.LogisticCompanyDisplay + " " + Obj.RateTypeDisplay + " - Manifest - " + Obj.ManifestName;
                                report.PrintedBy = detail.ContactName;
                                report.CustomerName = Obj.ContactName;
                                report.CompanyName = Obj.CompanyName;
                                report.ManifestDate = Obj.CreatedOn.ToString("dd-MMM-yyyy HH:mm") + " " + "(" + detail.OffsetShort + ")";
                                report.TimeZone = "(" + detail.OffsetShort + ")";
                                report.AccountNo = Obj.AccountNo;
                                report.Collection = new List<ManifestCollection>()
                                {
                                    new ManifestCollection()
                                    {
                                        Customer = Obj.ContactName,
                                        FromCompany = Obj.FromAddress,
                                        ToCompany = Obj.ToAddress,
                                        DisplayName = Obj.LogisticCompanyDisplay + " " + Obj.RateTypeDisplay,
                                        LogisticCompany = Obj.LogisticCompanyDisplay,
                                        RateTypeDisplay =  Obj.RateTypeDisplay,
                                        LogisticType = Obj.LogisticType,
                                        ShippingDate = Obj.DirectShipmentCreate,
                                        Status = Obj.DisplayStatusName,
                                        Reference = Obj.Reference1,
                                        FrayteNumber = Obj.TrackingNo,
                                        PlateNo = Obj.PlateNo,
                                        TotalPcs = (int)Obj.TotalPcs,
                                        TotalWeight = (float)Obj.TotalWeight
                                    }
                                };
                                _manifest.Add(report);
                            }
                        }
                    }
                }
                else if (moduleType == "sd")
                {
                    var data = dbContext.spGet_ManifestDetail(ManifestId, moduleType).FirstOrDefault();
                    var data1 = dbContext.spGet_DirectBookingManifestDetail(ManifestId, moduleType, RoleId).ToList();
                    _manifest = (from ds in dbContext.eCommerceShipments
                                 join dsd in dbContext.eCommerceShipmentDetails on ds.eCommerceShipmentId equals dsd.eCommerceShipmentId
                                 join us in dbContext.Users on ds.CustomerId equals us.UserId
                                 join dsa in dbContext.eCommerceShipmentAddresses on ds.FromAddressId equals dsa.eCommerceShipmentAddressId
                                 join dsad in dbContext.eCommerceShipmentAddresses on ds.ToAddressId equals dsad.eCommerceShipmentAddressId
                                 join cc in dbContext.Countries on dsa.CountryId equals cc.CountryId
                                 join cc1 in dbContext.Countries on dsad.CountryId equals cc1.CountryId
                                 join cl in dbContext.CountryLogistics on dsad.CountryId equals cl.CountryId
                                 join ss in dbContext.ShipmentStatus on ds.ShipmentStatusId equals ss.ShipmentStatusId
                                 join mf in dbContext.Manifests on ds.ManifestId equals mf.ManifestId
                                 where
                                       mf.ManifestId == ManifestId
                                 select new ManifestReport
                                 {
                                     ManifestName = mf.ManifestName,
                                     ManifestDate = mf.CreatedOn.ToString(),
                                     CustomerName = us.ContactName,
                                     PrintedBy = detail.ContactName + " " + "(" + detail.OffsetShort + ")",
                                     TimeZone = "(" + detail.OffsetShort + ")",
                                     TotalShipments = data.Total_Count.HasValue ? data.Total_Count.Value : 0,
                                     TotalWeights = (float)data.Weight,
                                     Collection = new List<ManifestCollection>()
                                     {
                                            new ManifestCollection()
                                            {
                                                  Customer = us.ContactName,
                                                  FromCompany = dsa.Address1 + " " + dsa.Area + " " + dsa.City + " " + dsa.State + " " + cc.CountryName,
                                                  ToCompany = dsad.Address1 + " " + dsad.Area + " " + dsad.City + " " + dsad.State + " " + cc1.CountryName,
                                                  DisplayName = cl.LogisicServiceDisplay,
                                                  ShippingDate = ds.CreatedOn,
                                                  Status = ss.DisplayStatusName,
                                                  Reference = ds.Reference1,
                                                  FrayteNumber = ds.FrayteNumber,
                                                  TotalPcs = dsd.CartoonValue,
                                                  TotalWeight = (float) (dsd.CartoonValue * dsd.Weight)
                                            }
                                     }
                                 }).ToList();
                }
                return _manifest;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        public FrayteViewManifest GetManifestDetail(int manifestId, string moduleType)
        {
            var list = dbContext.spGet_GetManifestedShipments(manifestId, moduleType).ToList();
            List<FrayteUserDirectShipment> lstUserShipment = new List<FrayteUserDirectShipment>();
            if (list != null && list.Count > 0)
            {
                FrayteUserDirectShipment frayte;
                foreach (var detail in list)
                {
                    frayte = new FrayteUserDirectShipment();
                    frayte.ShipmentId = detail.DirectShipmentId;
                    frayte.ShipmentCode = "";
                    frayte.Customer = detail.ContactName;
                    frayte.ShippedFromCompany = detail.FromCompany;
                    frayte.ShippedToCompany = detail.ToCompany;
                    frayte.ShippingBy = detail.LogisticCompany;
                    frayte.DisplayName = detail.LogisticCompanyDisplay;
                    frayte.RateType = detail.RateType;
                    frayte.RateTypeDisplay = detail.RateTypeDisplay;
                    frayte.ShippingDate = detail.CreatedOn;
                    frayte.Reference1 = detail.Reference1;
                    frayte.FrayteNumber = detail.FrayteNumber;
                    frayte.Status = detail.StatusName;
                    frayte.TrackingNo = detail.TrackingNo;
                    frayte.ManifestName = detail.ManifestName;
                    lstUserShipment.Add(frayte);

                }
            }

            var dasta = lstUserShipment.Select(p => new { p.ShippingBy, p.RateType, p.RateTypeDisplay, p.DisplayName, p.ManifestName }).Distinct().ToList();

            FrayteManifestDetail abc;
            FrayteViewManifest viewmanifest = new FrayteViewManifest();
            if (dasta != null && dasta.Count > 0)
            {
                viewmanifest.ManifestName = dasta[0].ManifestName;
                viewmanifest.ManifestedList = new List<FrayteManifestDetail>();
                foreach (var a in dasta)
                {
                    abc = new FrayteManifestDetail();
                    abc.CourierCompany = a.ShippingBy;
                    abc.RateType = a.RateType;
                    abc.RateTypeDisplay = a.RateTypeDisplay;
                    abc.CourierCompanyDisplay = a.DisplayName;

                    abc.DirectShipments = new List<FrayteUserDirectShipment>();
                    abc.DirectShipments = (from e in lstUserShipment
                                           where e.ShippingBy == a.ShippingBy &&
                                                 e.RateType == a.RateType
                                           select new FrayteUserDirectShipment
                                           {
                                               Customer = e.Customer,
                                               ShipmentId = e.ShipmentId,
                                               ShipmentCode = "",
                                               ShippedFromCompany = e.ShippedFromCompany,
                                               ShippedToCompany = e.ShippedToCompany,
                                               ShippingBy = e.ShippingBy,
                                               DisplayName = e.DisplayName,
                                               RateType = e.RateType,
                                               RateTypeDisplay = e.RateTypeDisplay,
                                               ShippingDate = e.ShippingDate,
                                               Reference1 = e.Reference1,
                                               FrayteNumber = e.FrayteNumber,
                                               Status = e.Status,
                                               TrackingNo = e.TrackingNo
                                           }).ToList();

                    viewmanifest.ManifestedList.Add(abc);
                }
            }
            return viewmanifest;
        }

        public List<FrayteManifest> GetManifests(TrackManifest trackManifest)
        {

            var userDetail = (from r in dbContext.Users
                              join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                              where r.UserId == trackManifest.CreatedBy
                              select new
                              {
                                  RoleId = ur.RoleId
                              }).FirstOrDefault();

            List<FrayteManifest> manifests = new List<FrayteManifest>();
            if (userDetail != null)
            {
                int SkipRows = 0;
                SkipRows = (trackManifest.CurrentPage - 1) * trackManifest.TakeRows;
                var list = dbContext.spGet_TrackManifest(trackManifest.FromDate, trackManifest.ToDate, SkipRows, trackManifest.TakeRows, trackManifest.ModuleType, trackManifest.subModuleType, trackManifest.UserId, trackManifest.CreatedBy, userDetail.RoleId, trackManifest.ManifestName).ToList();

                FrayteManifest manifestData;
                if (list != null && list.Count > 0)
                {
                    foreach (var data in list)
                    {
                        manifestData = new FrayteManifest();
                        manifestData.CustomerId = data.CustomerId;
                        manifestData.CreateOn = data.CreatedOn;
                        manifestData.NoOfShipments = data.TotalNoOfShipments.HasValue ? data.TotalNoOfShipments.Value : 0;
                        manifestData.Courier = data.Courier;
                        manifestData.CourierDisplay = data.CourierDisplay;
                        manifestData.TotalWeight = (float)data.TotalWeight;
                        manifestData.ManifestId = data.ManifestId;
                        manifestData.ManifestName = data.ManifestName;
                        manifestData.ModuleType = data.ModuleType;
                        manifestData.SubModuleType = data.SubModuleType;
                        manifestData.TotalRows = data.TotalRows.Value;
                        manifestData.Customer = dbContext.Users.Where(a => a.UserId == data.CustomerId).FirstOrDefault().CompanyName != null ? dbContext.Users.Where(a => a.UserId == data.CustomerId).FirstOrDefault().CompanyName : "";
                        manifests.Add(manifestData);
                    }
                }
            }

            return manifests;
        }

        public FrayteResult RemoveShipmentFromManifest(int shipmentId, string moduleType)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                if (moduleType == FrayteShipmentServiceType.DirectBooking)
                {
                    var shipment = dbContext.DirectShipments.Find(shipmentId);
                    if (shipment != null)
                    {
                        shipment.ManifestId = null;
                        dbContext.SaveChanges();
                        result.Status = true;
                    }
                }
                else if (moduleType == FrayteShipmentServiceType.eCommerce)
                {
                    var shipment = dbContext.eCommerceShipments.Find(shipmentId);
                    if (shipment != null)
                    {
                        shipment.ManifestId = null;
                        dbContext.SaveChanges();
                        result.Status = true;
                    }
                }
                else
                {
                    result.Status = false;
                }
            }
            catch (Exception e)
            {
                result.Status = false;
            }
            return result;
        }

        public List<FrayteBookingType> GetFrayteBookingType(int userId)
        {
            try
            {
                List<FrayteBookingType> BookingTypes = new List<FrayteBookingType>();

                var Role = dbContext.UserRoles.Where(p => p.UserId == userId).FirstOrDefault();
                if (Role.RoleId == (int)FrayteUserRole.Customer)
                {
                    var data = new CustomerRepository().GetCustomerModules(userId);
                    if (data != null)
                    {
                        if (data.IsDirectBooking)
                        {
                            BookingTypes.Add(new FrayteBookingType() { BookingType = FrayteShipmentServiceType.DirectBooking, BookingTypeDisplay = FrayteShipmentServiceType.DirectBookingDisplay });
                        }
                        if (data.IseCommerceBooking)
                        {
                            BookingTypes.Add(new FrayteBookingType() { BookingType = FrayteShipmentServiceType.eCommerce, BookingTypeDisplay = FrayteShipmentServiceType.eCommerceDisplay });
                        }
                    }
                }
                else
                {
                    BookingTypes.Add(new FrayteBookingType() { BookingType = FrayteShipmentServiceType.DirectBooking, BookingTypeDisplay = FrayteShipmentServiceType.DirectBookingDisplay });
                    BookingTypes.Add(new FrayteBookingType() { BookingType = FrayteShipmentServiceType.eCommerce, BookingTypeDisplay = FrayteShipmentServiceType.eCommerceDisplay });
                }

                return BookingTypes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<DirectBookingCustomer> GetCustomerList()
        {
            var OperationZone = UtilityRepository.GetOperationZone();
            var result = (from U in dbContext.Users
                          join UR in dbContext.UserRoles on U.UserId equals UR.UserId
                          join UA in dbContext.UserAdditionals on U.UserId equals UA.UserId
                          where
                            UR.RoleId == (int)FrayteUserRole.Customer &&
                            U.IsActive == true &&
                            UA.IsDirectBooking == true &&
                            U.OperationZoneId == OperationZone.OperationZoneId
                          select new DirectBookingCustomer
                          {
                              CustomerId = U.UserId,
                              CompanyName = ((U.CompanyName == null || U.CompanyName == "") ? U.ContactName : U.CompanyName),
                              AccountNumber = UA.AccountNo
                          }).OrderBy(p => p.CompanyName).ToList();

            return result;
        }

        public List<CustomManifestDetail> GetCustomManifestDetail()
        {
            List<CustomManifestDetail> CMfD = new List<CustomManifestDetail>();

            var CMD = (from ECCM in dbContext.eCommerceCustomManifests
                       join ECS in dbContext.eCommerceShipments on ECCM.CustomManifestId equals ECS.CustomManifestId into ps
                       from p in ps.DefaultIfEmpty()
                       join ECSD in dbContext.eCommerceShipmentDetails on p.eCommerceShipmentId equals ECSD.eCommerceShipmentId into ps3
                       from p3 in ps3.DefaultIfEmpty()
                       join ESA in dbContext.eCommerceShipmentAddresses on p.FromAddressId equals ESA.eCommerceShipmentAddressId into ps1
                       from p1 in ps1.DefaultIfEmpty()
                       join ESAD in dbContext.eCommerceShipmentAddresses on p.ToAddressId equals ESAD.eCommerceShipmentAddressId into ps2
                       from p2 in ps2.DefaultIfEmpty()
                       join CL in dbContext.CountryLogistics on p2.CountryId equals CL.CountryId into ps4
                       from p4 in ps4.DefaultIfEmpty()
                       select new CustomManifestDetail
                       {
                           ManifestId = ECCM == null ? 0 : ECCM.CustomManifestId,
                           ManifestName = (ECCM == null || ECCM.ManifestName == "") ? "" : ECCM.ManifestName,
                           Courier = (p4 == null || p4.LogisticService == "") ? "" : p4.LogisticService,
                           ManifestDate = ECCM.CreatedOn,
                           NumberofShipments = (from d in dbContext.eCommerceShipments where d.CustomManifestId == (p.CustomManifestId == null ? 0 : p.CustomManifestId) select d.CustomManifestId).Count(),
                           TotalWeight = p3 == null ? 0 : p3.CartoonValue * p3.Weight
                       }
                    ).ToList();

            if (CMD.Count > 0)
            {
                var result = CMD.GroupBy(p => p.ManifestId)
                            .Select(x => new CustomManifestDetail
                            {
                                ManifestId = x.Select(c => c.ManifestId).FirstOrDefault(),
                                ManifestName = x.Select(c => c.ManifestName).FirstOrDefault(),
                                Courier = x.Select(c => c.Courier).FirstOrDefault(),
                                ManifestDate = x.Select(c => c.ManifestDate).FirstOrDefault(),
                                NumberofShipments = x.Select(c => c.NumberofShipments).FirstOrDefault(),
                                TotalWeight = x.Select(c => c.TotalWeight).FirstOrDefault()
                            }).ToList();

                return result.OrderBy(p => p.ManifestId).ToList();
            }
            else
            {
                return null;
            }

            // return CMD;
        }

        public List<CustomManifestDetail> GetCustomManifests(TrackCustomManifest trackManifest)
        {
            int SkipRows = 0;
            SkipRows = (trackManifest.CurrentPage - 1) * trackManifest.TakeRows;
            List<CustomManifestDetail> CMfD = new List<CustomManifestDetail>();
            //var list = dbContext.spGet_CustomManifestSearch(trackManifest.ManifestName, trackManifest.ShipperName, trackManifest.ConsigneeName, trackManifest.FromDate, trackManifest.ToDate, SkipRows, trackManifest.TakeRows).ToList();
            var list = dbContext.spGet_CustomManifestSearch(trackManifest.ManifestName, trackManifest.FromDate, trackManifest.ToDate, SkipRows, trackManifest.TakeRows).ToList();
            if (list.Count > 0)
            {
                CMfD = new List<CustomManifestDetail>();
                CMfD = list.GroupBy(p => new { p.ManifestId, p.ManifestName })
                           .Select(x => new CustomManifestDetail
                           {
                               ManifestId = x.Select(c => c.ManifestId).FirstOrDefault(),
                               ManifestName = x.Select(c => c.ManifestName == "" ? "" : c.ManifestName).FirstOrDefault(),

                               ManifestDate = x.Select(c => c.ManifestDate).FirstOrDefault(),
                               NumberofShipments = (int)x.Select(c => c.TotalShipment == null ? 0 : c.TotalShipment).FirstOrDefault(),
                               TotalWeight = x.Select(c => c.TotalWeight).FirstOrDefault(),
                               TotalRows = (int)x.Select(c => c.TotalRows).FirstOrDefault()
                           }).ToList();

                return CMfD.OrderBy(p => p.ManifestId).ToList();
                //return CMfD;
            }
            else
            {

                return CMfD;
            }

            //List<CustomManifestDetail> manifests = new List<CustomManifestDetail>();
            //CustomManifestDetail manifestData;
            //if (list != null && list.Count > 0)
            //{
            //    foreach (var data in list)
            //    {
            //        manifestData = new CustomManifestDetail();
            //        //manifestData.CustomerId = data.;
            //        manifestData.CreateOn = (DateTime)data.ManifestDate;
            //        manifestData.NoOfShipments = (int)data.TotalShipment;
            //        manifestData.Courier = data.Courier;
            //        manifestData.CourierDisplay = "";
            //        manifestData.TotalWeight = (decimal)data.TotalWeight;
            //        manifestData.ManifestId = data.ManifestId;
            //        manifestData.ManifestName = data.ManifestName;
            //        //manifestData.ModuleType = data.ModuleType;
            //        //manifestData.TotalRows = data.TotalRows;
            //        manifests.Add(manifestData);
            //    }
            //}

            //return CMfD;
        }

        public List<eCommerceViewManifest> ViewCustomManifestDetail(int ManifestId)
        {
            List<CustomManifestDetail> CMfD = new List<CustomManifestDetail>();

            //var CMD = (from ECCM in dbContext.eCommerceCustomManifests

            //           join ECS in dbContext.eCommerceShipments on ECCM.CustomManifestId equals ECS.CustomManifestId
            //           join ESS in dbContext.ShipmentStatus on ECS.ShipmentStatusId equals ESS.ShipmentStatusId
            //           join ECSD in dbContext.eCommerceShipmentDetails on ECS.eCommerceShipmentId equals ECSD.eCommerceShipmentId
            //           join ECPTD in dbContext.eCommercePackageTrackingDetails on ECSD.eCommerceShipmentDetailId equals ECPTD.eCommerceShipmentDetailId
            //           join ESA in dbContext.eCommerceShipmentAddresses on ECS.FromAddressId equals ESA.eCommerceShipmentAddressId
            //           join ESAD in dbContext.eCommerceShipmentAddresses on ECS.ToAddressId equals ESAD.eCommerceShipmentAddressId
            //           join CL in dbContext.CountryLogistics on ESAD.CountryId equals CL.CountryId
            //           where ECCM.CustomManifestId == ManifestId
            //           select new eCommerceViewManifest
            //           {
            //               ManifestId = ECCM.CustomManifestId,
            //               Status = ESS.StatusName,
            //               ShipmentDetail = CL.LogisticService,
            //               TrackingNo = ECPTD.TrackingNo,
            //               FrayteNumber = ECS.FrayteNumber,
            //               ShippedFromCompany = ESA.CompanyName,
            //               ShippedToCompany = ESAD.CompanyName,
            //           }
            //        ).ToList();
            var CMD = (from ECS in dbContext.eCommerceShipments
                       let Id = ECS.eCommerceShipmentId
                       join ECCM in dbContext.eCommerceCustomManifests on ECS.CustomManifestId equals ECCM.CustomManifestId
                       join ESS in dbContext.ShipmentStatus on ECS.ShipmentStatusId equals ESS.ShipmentStatusId
                       join ECSD in dbContext.eCommerceShipmentDetails on ECS.eCommerceShipmentId equals ECSD.eCommerceShipmentId
                       join ECPTD in dbContext.eCommercePackageTrackingDetails on ECSD.eCommerceShipmentDetailId equals ECPTD.eCommerceShipmentDetailId
                       join ESA in dbContext.eCommerceShipmentAddresses on ECS.FromAddressId equals ESA.eCommerceShipmentAddressId
                       join ESAD in dbContext.eCommerceShipmentAddresses on ECS.ToAddressId equals ESAD.eCommerceShipmentAddressId
                       join CL in dbContext.CountryLogistics on ESAD.CountryId equals CL.CountryId
                       where ECS.CustomManifestId == ManifestId
                       select new eCommerceViewManifest
                       {
                           ManifestId = ECCM.CustomManifestId,
                           Status = ESS.StatusName,
                           ShipmentDetail = CL.LogisticService,
                           TrackingNo = ECPTD.TrackingNo,
                           FrayteNumber = ECS.FrayteNumber,
                           ShippedFromCompany = ESA.CompanyName,
                           ShippedToCompany = ESAD.CompanyName,
                           PackageDetail = (from a in dbContext.eCommerceShipmentDetails
                                            where a.eCommerceShipmentId == Id
                                            select new UploadShipmentPackage
                                            {
                                                CartoonValue = a.CartoonValue,
                                                Length = a.Length,
                                                Width = a.Width,
                                                Weight = a.Weight,
                                                Height = a.Height,
                                                Value = a.DeclaredValue.HasValue ? a.DeclaredValue.Value : 0,
                                                Content = a.PiecesContent
                                            }).ToList()
                       }
                  ).ToList();
            List<eCommerceViewManifest> CMfD1 = new List<eCommerceViewManifest>();

            CMfD1 = CMD.GroupBy(p => new { p.TrackingNo })
                         .Select(x => new eCommerceViewManifest
                         {
                             ManifestId = x.Select(c => c.ManifestId).FirstOrDefault(),
                             FrayteNumber = x.Select(c => c.FrayteNumber == "" ? "" : c.FrayteNumber).FirstOrDefault(),
                             Status = x.Select(c => c.Status).FirstOrDefault(),
                             ShipmentDetail = x.Select(c => c.ShipmentDetail).FirstOrDefault(),
                             TrackingNo = x.Select(c => c.TrackingNo).FirstOrDefault(),
                             ShippedFromCompany = x.Select(c => c.ShippedFromCompany).FirstOrDefault(),
                             ShippedToCompany = x.Select(c => c.ShippedToCompany).FirstOrDefault(),
                             PackageDetail = x.Select(c => c.PackageDetail.ToList()).FirstOrDefault()
                         }).ToList();

            return CMfD1.OrderBy(p => p.TrackingNo).ToList();
        }

        public List<UploadShipmentPackage> CustomManifestPackageDetail(int EcommerceShipmentId)
        {
            List<UploadShipmentPackage> Package = new List<UploadShipmentPackage>();
            Package = (from a in dbContext.eCommerceShipmentDetails
                       where a.eCommerceShipmentId == EcommerceShipmentId
                       select new UploadShipmentPackage
                       {
                           CartoonValue = a.CartoonValue,
                           Length = a.Length,
                           Width = a.Width,
                           Weight = a.Weight,
                           Height = a.Height,
                           Value = a.DeclaredValue.HasValue ? a.DeclaredValue.Value : 0,
                           Content = a.PiecesContent
                       }).ToList();

            return Package;
        }

        public FrayteManifestExcel DownLoadCustomManifestFromExcel(string filename, string filepath)
        {
            FrayteManifestExcel frayteManifestDetailexcel = new Services.Models.FrayteManifestExcel();

            List<Frayte.Services.Models.FrayteManifestOnExcel> _Manifestdetail = new List<Frayte.Services.Models.FrayteManifestOnExcel>();

            string connString = new DirectShipmentRepository().getExcelConnectionString(filename, filepath);
            string fileExtension = "";
            fileExtension = new DirectShipmentRepository().getFileExtensionString(filename);
            try
            {
                if (!string.IsNullOrEmpty(fileExtension))
                {

                    var ds = new DataSet();
                    if (fileExtension == FrayteFileExtension.CSV)
                    {
                        try
                        {
                            using (var conn = new OleDbConnection(connString))
                            {
                                conn.Open();
                                var query = "SELECT * FROM [" + Path.GetFileName(filename) + "]";
                                using (var adapter = new OleDbDataAdapter(query, conn))
                                {
                                    adapter.Fill(ds, "CustomManifest");
                                }
                            }

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {

                        using (var conn = new OleDbConnection(connString))
                        {
                            conn.Open();
                            DataTable dbSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string firstSheetName = dbSchema.Rows[0]["TABLE_NAME"].ToString();
                            var query = "SELECT * FROM " + "[" + firstSheetName + "]";//[Sheet1$]";
                            using (var adapter = new OleDbDataAdapter(query, conn))
                            {
                                adapter.Fill(ds, "CustomManifest");
                            }
                        }
                    }

                    var exceldata = ds.Tables[0];

                    string PiecesColumnList = "CustomCommoditymap,CustomsEntryType,CustomsTotalValue,CustomsTotalVat,CustomsDuty";
                    //bool IsExcelValid = UtilityRepository.CheckUploadExcelFormat(PiecesColumnList, exceldata);
                    bool IsExcelValid = true;
                    if (!IsExcelValid)
                    {
                        frayteManifestDetailexcel.Message = "Columns are not matching with provided template columns. Please check the column names.";
                    }
                    else
                    {
                        if (exceldata.Rows.Count > 0)
                        {
                            List<FrayteManifestOnExcel> fManifestXL = new List<FrayteManifestOnExcel>();
                            FrayteManifestOnExcel CManifestExcel;
                            foreach (System.Data.DataRow edata in exceldata.Rows)
                            {
                                CManifestExcel = new FrayteManifestOnExcel();
                                CManifestExcel.TrackingNumber = edata["TrackingNumber"].ToString() == "" ? "" : edata["TrackingNumber"].ToString();
                                CManifestExcel.Reference = edata["Reference"].ToString() == "" ? "" : edata["Reference"].ToString();
                                CManifestExcel.InternalAccountNumber = edata["InternalAccountNumber"] != null ? CommonConversion.ConvertToInt(edata, "InternalAccountNumber") : 0;
                                CManifestExcel.ShipperName = edata["ShipperName"].ToString() == "" ? "" : edata["ShipperName"].ToString();
                                CManifestExcel.ShipperAddress1 = edata["ShipperAddress1"].ToString() == "" ? "" : edata["ShipperAddress1"].ToString();
                                CManifestExcel.ShipperAddress2 = edata["ShipperAddress2"].ToString() == "" ? "" : edata["ShipperAddress2"].ToString();
                                CManifestExcel.ShipperCity = edata["ShipperCity"].ToString() == "" ? "" : edata["ShipperCity"].ToString();
                                CManifestExcel.ShipperZip = edata["ShipperZip"].ToString() == "" ? "" : edata["ShipperZip"].ToString();
                                CManifestExcel.ShipperState = edata["ShipperState"].ToString() == "" ? "" : edata["ShipperState"].ToString();
                                CManifestExcel.ShipperPhoneNo = edata["ShipperPhoneNo"].ToString() == "" ? "" : edata["ShipperPhoneNo"].ToString();
                                CManifestExcel.ShipperEmail = edata["ShipperEmail"].ToString() == "" ? "" : edata["ShipperEmail"].ToString();
                                CManifestExcel.ShipperCountryCode = edata["ShipperCountryCode"].ToString() == "" ? "" : edata["ShipperCountryCode"].ToString();
                                CManifestExcel.ConsigneeName = edata["ConsigneeName"].ToString() == "" ? "" : edata["ConsigneeName"].ToString();
                                CManifestExcel.ConsigneeAddress1 = edata["ConsigneeAddress1"].ToString() == "" ? "" : edata["ConsigneeAddress1"].ToString();
                                CManifestExcel.ConsigneeAddress2 = edata["ConsigneeAddress2"].ToString() == "" ? "" : edata["ConsigneeAddress2"].ToString();
                                CManifestExcel.ConsigneeCity = edata["ConsigneeCity"].ToString() == "" ? "" : edata["ConsigneeCity"].ToString();
                                CManifestExcel.ConsigneeState = edata["ConsigneeState"].ToString() == "" ? "" : edata["ConsigneeState"].ToString();
                                CManifestExcel.ConsigneePhoneNo = edata["ConsigneePhoneNo"].ToString() == "" ? "" : edata["ConsigneePhoneNo"].ToString();
                                CManifestExcel.ConsigneeEmail = edata["ConsigneeEmail"].ToString() == "" ? "" : edata["ConsigneeEmail"].ToString();
                                CManifestExcel.Pieces = edata["Pieces"] != null ? CommonConversion.ConvertToInt(edata, "Pieces") : 0;
                                CManifestExcel.TotalWeight = edata["TotalWeight"] != null ? CommonConversion.ConvertToInt(edata, "TotalWeight") : 0;
                                CManifestExcel.WeightUOM = edata["WeightUOM"].ToString() == "" ? "" : edata["WeightUOM"].ToString();
                                CManifestExcel.TotalValue = edata["TotalValue"] != null ? CommonConversion.ConvertToInt(edata, "TotalValue") : 0;
                                CManifestExcel.Currency = edata["Currency"].ToString() == "" ? "" : edata["Currency"].ToString();
                                CManifestExcel.Incoterms = edata["Incoterms"].ToString() == "" ? "" : edata["Incoterms"].ToString();
                                CManifestExcel.ItemDescription = edata["ItemDescription"].ToString() == "" ? "" : edata["ItemDescription"].ToString();
                                CManifestExcel.ItemHScodes = edata["ItemHScodes"].ToString() == "" ? "" : edata["ItemHScodes"].ToString();
                                CManifestExcel.ItemValue = edata["ItemValue"] != null ? CommonConversion.ConvertToInt(edata, "ItemValue") : 0;
                                CManifestExcel.CustomCommodityMap = edata["CustomCommodityMap"].ToString() == "" ? "" : edata["CustomCommodityMap"].ToString();
                                CManifestExcel.CustomEntryType = edata["CustomEntryType"].ToString() == "" ? "" : edata["CustomEntryType"].ToString();
                                CManifestExcel.CustomTotalValue = edata["CustomTotalValue"] != null ? CommonConversion.ConvertToDecimal(edata, "CustomTotalValue") : 0;
                                CManifestExcel.CustomTotalVAT = edata["CustomTotalVAT"] != null ? CommonConversion.ConvertToDecimal(edata, "CustomTotalVAT") : 0;
                                CManifestExcel.CustomDuty = edata["CustomDuty"] != null ? CommonConversion.ConvertToDecimal(edata, "CustomDuty") : 0;
                                fManifestXL.Add(CManifestExcel);

                            }
                            ManifestExcelWrite(fManifestXL, filename);
                            frayteManifestDetailexcel.FrayteManifestDetail = new List<Frayte.Services.Models.FrayteManifestOnExcel>();
                            frayteManifestDetailexcel.FrayteManifestDetail = _Manifestdetail;
                            frayteManifestDetailexcel.Message = "OK";
                        }

                        else
                        {
                            frayteManifestDetailexcel.Message = "No records found.";
                        }
                    }

                }
                else
                {
                    frayteManifestDetailexcel.Message = "Excel file not valid";
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                if (ex != null && !string.IsNullOrEmpty(ex.Message) && ex.Message.Contains("Sheet1$"))
                {
                    frayteManifestDetailexcel.Message = "Sheet name is invalid.";
                }
                else
                {
                    frayteManifestDetailexcel.Message = "Error while uploading the excel.";
                }
                return frayteManifestDetailexcel;

            }

            return frayteManifestDetailexcel;
        }

        public FrayteManifestExcel ManifestExcelWrite(List<FrayteManifestOnExcel> ManifestData, string ManifestFileName)
        {
            FrayteManifestExcel FMEx = new FrayteManifestExcel();
            //CSV Writesss
            try
            {
                StreamWriter sw;
                //File.Create(@"D:\ProjectFrayte\Frayte\Frayte.WebApi\Manifestedshipments\" + CreateManifest(ManifestData) + ".csv");
                if (ManifestFileName == "")
                {
                    ManifestFileName = CreateManifest(ManifestData);
                    sw = new StreamWriter(AppSettings.ManifestFolderPath + "\\" + ManifestFileName + ".csv", false);
                }
                else
                {
                    sw = new StreamWriter(AppSettings.ManifestFolderPath + "\\" + ManifestFileName + ".csv", false);
                }
                Type type = typeof(FrayteManifestOnExcel);
                int count = type.GetProperties().Length;
                for (var k = 0; k < count; k++)
                {
                    var pro = typeof(FrayteManifestOnExcel).GetProperties();
                    sw.Write(pro[k].Name);
                    if (k < count)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);

                // Now write all the rows.
                //foreach (var dr in ManifestData)
                //{
                //var p = typeof(FrayteManifestOnExcel).GetProperties();
                for (var l = 0; l < ManifestData.Count; l++)
                {
                    sw.Write(ManifestData[l].ECommerceShipmentId);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].TrackingNumber);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].Reference));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].InternalAccountNumber);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ShipperName));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ShipperAddress1));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    string shipperAddress2 = ChangeStringCommatoSpace(ManifestData[l].ShipperAddress2);
                    sw.Write(shipperAddress2);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    //sw.Write(ChangeStringCommatoSpace(ManifestData[l].ShipperAddress3));
                    //if (l < ManifestData.Count)
                    //    sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ShipperCity));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].ShipperZip);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ShipperState));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].ShipperPhoneNo);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ShipperEmail));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].ShipperCountryCode);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneeName));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneeAddress1));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneeAddress2));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    //sw.Write(ManifestData[l].ConsigneeAddress3);
                    //if (l <= ManifestData.Count)
                    //    sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneeCity));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].ConsigneeZip != null ? ManifestData[l].ConsigneeZip : "");
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    string state = ChangeStringCommatoSpace(ManifestData[l].ConsigneeState);
                    sw.Write(state);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneePhoneNo));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneeEmail));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneeCountryCode != null ? ManifestData[l].ConsigneeCountryCode : ""));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].Pieces);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].TotalWeight);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].WeightUOM);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].TotalValue);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].Currency));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].Incoterms));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ItemDescription));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ItemHScodes != null ? ManifestData[l].ItemHScodes : ""));
                    //if (l <= ManifestData.Count)
                    //    sw.Write(",");
                    //sw.Write(ChangeStringCommatoSpace(ManifestData[l].ItemQuantity));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].ItemValue);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].CustomCommodityMap != null ? ManifestData[l].CustomCommodityMap : "");
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].CustomEntryType != null ? ManifestData[l].CustomEntryType : "");
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].CustomTotalValue != null ? ManifestData[l].CustomTotalValue : 0);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].CustomTotalVAT != null ? ManifestData[l].CustomTotalVAT : 0);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].CustomDuty != null ? ManifestData[l].CustomDuty : 0);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(sw.NewLine);
                }
                sw.Close();

                FMEx.Message = "True";
            }
            catch (Exception ex)
            {

                FMEx.Message = "False";
            }

            return FMEx;
        }

        public string ManifestExcelWriteFromUI(List<FrayteManifestOnExcel> ManifestData, string ManifestFileName)
        {
            FrayteManifestExcel FMEx = new FrayteManifestExcel();
            //CSV Writesss
            try
            {
                StreamWriter sw;
                //File.Create(@"D:\ProjectFrayte\Frayte\Frayte.WebApi\Manifestedshipments\" + CreateManifest(ManifestData) + ".csv");
                if (ManifestFileName == "")
                {
                    ManifestFileName = CreateManifest(ManifestData);
                    if (ManifestFileName.Contains(".csv"))
                    {
                        sw = new StreamWriter(AppSettings.ManifestFolderPath + "\\" + ManifestFileName, false);
                    }
                    else
                    {
                        sw = new StreamWriter(AppSettings.ManifestFolderPath + "\\" + ManifestFileName + ".csv", false);
                    }

                }
                else
                {
                    if (ManifestFileName.Contains(".csv"))
                    {
                        sw = new StreamWriter(AppSettings.ManifestFolderPath + "\\" + ManifestFileName, false);
                    }
                    else
                    {
                        sw = new StreamWriter(AppSettings.ManifestFolderPath + "\\" + ManifestFileName + ".csv", false);
                    }
                }
                Type type = typeof(FrayteManifestOnExcel);
                int count = type.GetProperties().Length;
                for (var k = 0; k < count; k++)
                {
                    var pro = typeof(FrayteManifestOnExcel).GetProperties();
                    sw.Write(pro[k].Name);
                    if (k < count)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);

                // Now write all the rows.
                //foreach (var dr in ManifestData)
                //{
                //var p = typeof(FrayteManifestOnExcel).GetProperties();
                for (var l = 0; l < ManifestData.Count; l++)
                {
                    sw.Write(ManifestData[l].ECommerceShipmentId);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].TrackingNumber == null ? "" : ManifestData[l].TrackingNumber);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].Reference == null ? "" : ManifestData[l].Reference));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].InternalAccountNumber);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ShipperName == null ? "" : ManifestData[l].ShipperName));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ShipperAddress1 == null ? "" : ManifestData[l].ShipperAddress1));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    string shipperAddress2 = ChangeStringCommatoSpace(ManifestData[l].ShipperAddress2 == null ? "" : ManifestData[l].ShipperAddress2);
                    sw.Write(shipperAddress2);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    //sw.Write(ChangeStringCommatoSpace(ManifestData[l].ShipperAddress3));
                    //if (l < ManifestData.Count)
                    //    sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ShipperCity == null ? "" : ManifestData[l].ShipperCity));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].ShipperZip == null ? "" : ManifestData[l].ShipperZip);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ShipperState == null ? "" : ManifestData[l].ShipperState));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].ShipperPhoneNo == null ? "" : ManifestData[l].ShipperPhoneNo);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ShipperEmail == null ? "" : ManifestData[l].ShipperEmail));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].ShipperCountryCode == null ? "" : ManifestData[l].ShipperCountryCode);
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneeName == null ? "" : ManifestData[l].ConsigneeName));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneeAddress1 == null ? "" : ManifestData[l].ConsigneeAddress1));
                    if (l < ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneeAddress2 == null ? "" : ManifestData[l].ConsigneeAddress2));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    //sw.Write(ManifestData[l].ConsigneeAddress3);
                    //if (l <= ManifestData.Count)
                    //    sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneeCity == null ? "" : ManifestData[l].ConsigneeCity));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].ConsigneeZip);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    string state = ChangeStringCommatoSpace(ManifestData[l].ConsigneeState == null ? "" : ManifestData[l].ConsigneeState);
                    sw.Write(state);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneePhoneNo == null ? "" : ManifestData[l].ConsigneePhoneNo));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneeEmail == null ? "" : ManifestData[l].ConsigneeEmail));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ConsigneeCountryCode == null ? "" : ManifestData[l].ConsigneeCountryCode));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].Pieces);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].TotalWeight);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].WeightUOM == null ? "" : ManifestData[l].WeightUOM);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].TotalValue);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].Currency == null ? "" : ManifestData[l].Currency));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].Incoterms == null ? "" : ManifestData[l].Incoterms));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ItemDescription == null ? "" : ManifestData[l].ItemDescription));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(ManifestData[l].ItemHScodes == null ? "" : ManifestData[l].ItemHScodes));
                    //if (l <= ManifestData.Count)
                    //    sw.Write(",");
                    //sw.Write(ChangeStringCommatoSpace(ManifestData[l].ItemQuantity));
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].ItemValue);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].CustomCommodityMap == null ? "" : ManifestData[l].CustomCommodityMap);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].CustomEntryType == null ? "" : ManifestData[l].CustomEntryType);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].CustomTotalValue);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].CustomTotalVAT);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].CustomDuty);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].EstimatedDateofArrival);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].EstimatedTimeofArrival);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].EstimatedDateofDelivery);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(ManifestData[l].EstimatedTimeofDelivery);
                    if (l <= ManifestData.Count)
                        sw.Write(",");
                    sw.Write(sw.NewLine);
                }
                sw.Close();

                FMEx.Message = "True";
            }
            catch (Exception ex)
            {

                FMEx.Message = "False";
            }

            return ManifestFileName;
        }

        public string CreateManifest(List<FrayteManifestOnExcel> ManifestData)
        {
            string ManifestName = "";
            //FrayteResult result = new FrayteResult();
            try
            {
                // group the shipments based on CourierCompany 
                //var dasta = manifestShipments.where(p => p.eCommerceShipmentId).Distinct().ToList();


                eCommerceCustomManifest manifest = new eCommerceCustomManifest();

                manifest.ManifestName = "";
                manifest.CreatedOn = DateTime.UtcNow;
                manifest.ModuleType = "eCommerce";

                dbContext.eCommerceCustomManifests.Add(manifest);
                dbContext.SaveChanges();

                //"MNUK-170300001"

                manifest.ManifestName = "MNUKCUS" + GetFormattedManifestId(manifest.CustomManifestId);
                ManifestName = "MNUKCUS" + GetFormattedManifestId(manifest.CustomManifestId);

                dbContext.Entry(manifest).State = System.Data.Entity.EntityState.Modified;
                // dbContext.SaveChanges();
                eCommerceShipment edata = new eCommerceShipment();
                foreach (var mData in ManifestData)
                {
                    var result = dbContext.eCommerceShipments.Where(a => a.eCommerceShipmentId == mData.ECommerceShipmentId).FirstOrDefault();
                    if (result != null)
                    {
                        result.CustomManifestId = manifest.CustomManifestId;
                        //dbContext.eCommerceShipments.Add(edata);
                        dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;

                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

                //result.Status = false;
            }
            return ManifestName;
        }

        public string ChangeStringCommatoSpace(string res)
        {
            string a = "";
            var result = res.Split(',');
            for (int i = 0; i < result.Length; i++)
            {
                a = a + result[i];
            }
            return a;
        }

        public List<FrayteManifestOnExcel> GetMainfestData(int ManifestId, string ManifestFileName)
        {

            var ManifestData = (from ECS in dbContext.eCommerceShipments
                                join EADF in dbContext.eCommerceShipmentAddresses on ECS.FromAddressId equals EADF.eCommerceShipmentAddressId
                                join EADT in dbContext.eCommerceShipmentAddresses on ECS.ToAddressId equals EADT.eCommerceShipmentAddressId
                                join PDD in dbContext.eCommerceShipmentDetails on ECS.eCommerceShipmentId equals PDD.eCommerceShipmentId
                                join TN in dbContext.eCommercePackageTrackingDetails on PDD.eCommerceShipmentDetailId equals TN.eCommerceShipmentDetailId
                                join TD in dbContext.eCommercePackageTrackingDetails on PDD.eCommerceShipmentDetailId equals TD.eCommerceShipmentDetailId
                                join Cur in dbContext.CurrencyTypes on ECS.CurrencyCode equals Cur.CurrencyCode
                                join Cun in dbContext.Countries on EADF.CountryId equals Cun.CountryId
                                where ECS.CustomManifestId == ManifestId
                                select new FrayteManifestOnExcel
                                {
                                    ECommerceShipmentId = ECS.eCommerceShipmentId,
                                    TrackingNumber = TN.TrackingNo,
                                    Reference = ECS.Reference1,
                                    InternalAccountNumber = PDD.eCommerceShipmentDetailId,
                                    ShipperName = EADF.ContactFirstName + " " + EADF.ContactLastName,
                                    ShipperAddress1 = EADF.Address1,
                                    ShipperAddress2 = EADF.Address2,
                                    ShipperCity = EADF.City,
                                    ShipperZip = EADF.Zip,
                                    ShipperState = EADF.State,

                                    ShipperPhoneNo = EADF.PhoneNo,
                                    ShipperEmail = EADF.Email,
                                    ShipperCountryCode = Cun.CountryName,
                                    ConsigneeName = EADT.ContactFirstName + " " + EADT.ContactLastName,
                                    ConsigneeAddress1 = EADT.Address1,
                                    ConsigneeAddress2 = EADT.Address2,
                                    ConsigneeCity = EADT.City,
                                    ConsigneeZip = EADT.Zip,
                                    ConsigneeState = EADT.State,
                                    ConsigneePhoneNo = EADT.PhoneNo,
                                    ConsigneeEmail = EADT.Email,
                                    ConsigneeCountryCode = Cun.CountryName,
                                    WeightUOM = ECS.PackageCaculatonType,
                                    Currency = Cur.CurrencyDescription,
                                    Pieces = PDD.CartoonValue,
                                    TotalWeight = PDD.Weight * PDD.CartoonValue,
                                    TotalValue = PDD.CartoonValue * PDD.DeclaredValue,
                                    Incoterms = "DDP",
                                    ItemDescription = ECS.ContentDescription,
                                    ItemHScodes = PDD.HSCode,
                                    ItemValue = PDD.DeclaredValue
                                }).ToList();

            if (ManifestData.Count > 0)
            {
                // Send Mail before Creating Manifest

                var res = ManifestExcelWrite(ManifestData, ManifestFileName);

                // Send Mail before Creating Manifest

                // Make IsManifested Flag True
                if (res.Message == "True")
                {
                    //foreach (var a in ManifestData)
                    //{
                    //    var result = dbContext.eCommerceShipments.Where(p => p.eCommerceShipmentId == a.ECommerceShipmentId).FirstOrDefault();

                    //    if (result != null)
                    //    {
                    //        result.IsManifested = true;
                    //        dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                    //        dbContext.SaveChanges();
                    //    }
                    //}
                }
            }
            else
            {
                Console.Write("There is some problem generating Excel");
            }
            return ManifestData;
        }

        public List<FrayteManifestOnExcel> GetMainfestDataUI(int ManifestId, string ManifestFileName)
        {

            //var ManifestData = (from ECS in dbContext.eCommerceShipments
            //                    join EADF in dbContext.eCommerceShipmentAddresses on ECS.FromAddressId equals EADF.eCommerceShipmentAddressId
            //                    join EADT in dbContext.eCommerceShipmentAddresses on ECS.ToAddressId equals EADT.eCommerceShipmentAddressId
            //                    join PDD in dbContext.eCommerceShipmentDetails on ECS.eCommerceShipmentId equals PDD.eCommerceShipmentId
            //                    join TN in dbContext.eCommercePackageTrackingDetails on PDD.eCommerceShipmentDetailId equals TN.eCommerceShipmentDetailId
            //                    join TD in dbContext.eCommercePackageTrackingDetails on PDD.eCommerceShipmentDetailId equals TD.eCommerceShipmentDetailId
            //                    join Cur in dbContext.CurrencyTypes on ECS.CurrencyCode equals Cur.CurrencyCode
            //                    join Cun in dbContext.Countries on EADF.CountryId equals Cun.CountryId
            //                    where ECS.CustomManifestId == ManifestId
            //                    select new FrayteManifestOnExcel
            //                    {
            //                        ECommerceShipmentId = ECS.eCommerceShipmentId,
            //                        TrackingNumber = TN.TrackingNo,
            //                        Reference = ECS.Reference1,
            //                        InternalAccountNumber = PDD.eCommerceShipmentDetailId,
            //                        ShipperName = EADF.ContactFirstName + " " + EADF.ContactLastName,
            //                        ShipperAddress1 = EADF.Address1,
            //                        ShipperAddress2 = EADF.Address2,
            //                        ShipperCity = EADF.City,
            //                        ShipperZip = EADF.Zip,
            //                        ShipperState = EADF.State,
            //                        ShipperPhoneNo = EADF.PhoneNo,
            //                        ShipperEmail = EADF.Email,
            //                        ShipperCountryCode = Cun.CountryName,
            //                        ConsigneeName = EADT.ContactFirstName + " " + EADT.ContactLastName,
            //                        ConsigneeAddress1 = EADT.Address1,
            //                        ConsigneeAddress2 = EADT.Address2,
            //                        ConsigneeCity = EADT.City,
            //                        ConsigneeZip = EADT.Zip,
            //                        ConsigneeState = EADT.State,
            //                        ConsigneePhoneNo = EADT.PhoneNo,
            //                        ConsigneeEmail = EADT.Email,
            //                        ConsigneeCountryCode = Cun.CountryName,
            //                        WeightUOM = ECS.PackageCaculatonType,
            //                        Currency = Cur.CurrencyDescription,
            //                        Pieces = PDD.CartoonValue,
            //                        TotalWeight = PDD.Weight * PDD.CartoonValue,
            //                        TotalValue = PDD.CartoonValue * PDD.DeclaredValue,
            //                        Incoterms = "DDP",
            //                        ItemDescription = ECS.ContentDescription,
            //                        ItemHScodes = PDD.HSCode,
            //                        ItemValue = PDD.DeclaredValue,
            //                        EstimatedDateofArrival = ECS.EstimatedDateofArrival,
            //                        EstimatedTimeofArrival = ECS.EstimatedTimeofArrival,
            //                        EstimatedDateofDelivery = ECS.EstimatedDateofDelivery,
            //                        EstimatedTimeofDelivery = ECS.EstimatedTimeofDelivery
            //                    }).ToList();

            var ManifestData = (from PDD in dbContext.eCommerceShipmentDetails
                                join ECS in dbContext.eCommerceShipments on PDD.eCommerceShipmentId equals ECS.eCommerceShipmentId
                                join EADF in dbContext.eCommerceShipmentAddresses on ECS.FromAddressId equals EADF.eCommerceShipmentAddressId
                                join EADT in dbContext.eCommerceShipmentAddresses on ECS.ToAddressId equals EADT.eCommerceShipmentAddressId

                                join TN in dbContext.eCommercePackageTrackingDetails on PDD.eCommerceShipmentDetailId equals TN.eCommerceShipmentDetailId
                                join TD in dbContext.eCommercePackageTrackingDetails on PDD.eCommerceShipmentDetailId equals TD.eCommerceShipmentDetailId
                                join Cur in dbContext.CurrencyTypes on ECS.CurrencyCode equals Cur.CurrencyCode
                                join Cun in dbContext.Countries on EADF.CountryId equals Cun.CountryId
                                where ECS.CustomManifestId == ManifestId
                                select new FrayteManifestOnExcel
                                {
                                    ECommerceShipmentId = ECS.eCommerceShipmentId,
                                    TrackingNumber = TN.TrackingNo,
                                    Reference = ECS.Reference1,
                                    InternalAccountNumber = PDD.eCommerceShipmentDetailId,
                                    ShipperName = EADF.ContactFirstName + " " + EADF.ContactLastName,
                                    ShipperAddress1 = EADF.Address1,
                                    ShipperAddress2 = EADF.Address2,
                                    ShipperCity = EADF.City,
                                    ShipperZip = EADF.Zip,
                                    ShipperState = EADF.State,
                                    ShipperPhoneNo = EADF.PhoneNo,
                                    ShipperEmail = EADF.Email,
                                    ShipperCountryCode = Cun.CountryName,
                                    ConsigneeName = EADT.ContactFirstName + " " + EADT.ContactLastName,
                                    ConsigneeAddress1 = EADT.Address1,
                                    ConsigneeAddress2 = EADT.Address2,
                                    ConsigneeCity = EADT.City,
                                    ConsigneeZip = EADT.Zip,
                                    ConsigneeState = EADT.State,
                                    ConsigneePhoneNo = EADT.PhoneNo,
                                    ConsigneeEmail = EADT.Email,
                                    ConsigneeCountryCode = Cun.CountryName,
                                    WeightUOM = ECS.PackageCaculatonType,
                                    Currency = Cur.CurrencyDescription,
                                    Pieces = PDD.CartoonValue,
                                    TotalWeight = PDD.Weight * PDD.CartoonValue,
                                    TotalValue = PDD.CartoonValue * PDD.DeclaredValue,
                                    Incoterms = "DDP",
                                    ItemDescription = ECS.ContentDescription,
                                    ItemHScodes = PDD.HSCode,
                                    ItemValue = PDD.DeclaredValue,
                                    EstimatedDateofArrival = ECS.EstimatedDateofArrival,
                                    EstimatedTimeofArrival = ECS.EstimatedTimeofArrival,
                                    EstimatedDateofDelivery = ECS.EstimatedDateofDelivery,
                                    EstimatedTimeofDelivery = ECS.EstimatedTimeofDelivery
                                }).ToList();

            if (ManifestData.Count > 0)
            {
                // Send Mail before Creating Manifest

                //var res = ManifestExcelWrite(ManifestData, ManifestFileName);

                // Send Mail before Creating Manifest

                // Make IsManifested Flag True
                //if (res.Message == "True")
                //{
                //foreach (var a in ManifestData)
                //{
                //    var result = dbContext.eCommerceShipments.Where(p => p.eCommerceShipmentId == a.ECommerceShipmentId).FirstOrDefault();

                //    if (result != null)
                //    {
                //        result.IsManifested = true;
                //        dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                //        dbContext.SaveChanges();
                //    }
                //}
                //}
            }
            else
            {
                Console.Write("There is some problem generating Excel");
            }
            return ManifestData;
        }

        public List<ManifestShipmentModel> GetManifestShipments(int ManifestId)
        {
            List<ManifestShipmentModel> Result = new List<ManifestShipmentModel>();
            try
            {
                Result = (from DS in dbContext.DirectShipments
                          join US in dbContext.Users on DS.CreatedBy equals US.UserId
                          join USS in dbContext.Users on DS.CustomerId equals USS.UserId
                          join USA in dbContext.UserAdditionals on DS.CustomerId equals USA.UserId
                          join UA in dbContext.UserAddresses on DS.CustomerId equals UA.UserId
                          join DSA in dbContext.DirectShipmentAddresses on DS.FromAddressId equals DSA.DirectShipmentAddressId
                          join CC in dbContext.Countries on DSA.CountryId equals CC.CountryId
                          join DSAD in dbContext.DirectShipmentAddresses on DS.ToAddressId equals DSAD.DirectShipmentAddressId
                          join CCA in dbContext.Countries on DSAD.CountryId equals CCA.CountryId
                          join ST in dbContext.ShipmentTypes on DS.ShipmentTypeId equals ST.ShipmentTypeId into STT
                          from ST in STT.DefaultIfEmpty()
                          join CUD in dbContext.ShipmentCustomDetails on DS.DirectShipmentId equals CUD.ShipmentId into CUDD
                          from CUD in CUDD.DefaultIfEmpty()
                          join LSST in dbContext.LogisticServiceShipmentTypes on DS.ShipmentTypeId equals LSST.LogisticServiceShipmentTypeId into LSSTT
                          from LSST in LSSTT.DefaultIfEmpty()
                          join LSCA in dbContext.LogisticServiceCourierAccounts on DS.CourierAccountId equals LSCA.LogisticServiceCourierAccountId into LSCAA
                          from LSCA in LSCAA.DefaultIfEmpty()
                          join LS in dbContext.LogisticServices on LSCA.LogisticServiceId equals LS.LogisticServiceId into LSS
                          from LS in LSS.DefaultIfEmpty()
                          join SS in dbContext.ShipmentStatus on DS.ShipmentStatusId equals SS.ShipmentStatusId into SSA
                          from SS in SSA.DefaultIfEmpty()
                          where DS.ManifestId == ManifestId
                          select new ManifestShipmentModel
                          {
                              StatusType = SS.DisplayStatusName,
                              Courier = LS.LogisticCompanyDisplay + " " + ((LS.RateTypeDisplay != null || LS.RateTypeDisplay != "") ? LS.RateTypeDisplay : ""), //Logistic Service Type
                              LogisticType = DS.LogisticType != null && DS.LogisticType != "" && DS.LogisticType == "UKShipment" ? "UK Shipment" : DS.LogisticType,

                              CustomerCompany = !string.IsNullOrEmpty(USS.CompanyName) ? USS.CompanyName : USS.ContactName,
                              CustomerAccountNo = !string.IsNullOrEmpty(USA.AccountNo) ? USA.AccountNo : string.Empty,
                              CustomerCountry = UA.CountryId > 0 ? dbContext.Countries.Where(a => a.CountryId == UA.CountryId).FirstOrDefault().CountryName : "",
                              CustomerEmail = !string.IsNullOrEmpty(USS.Email) ? USS.Email : string.Empty,
                              CustomerName = !string.IsNullOrEmpty(USS.ContactName) ? USS.ContactName : string.Empty,
                              CustomerPhoneNo = !string.IsNullOrEmpty(USS.TelephoneNo) ? USS.TelephoneNo : string.Empty,
                              CustomerPostCode = !string.IsNullOrEmpty(UA.Zip) ? UA.Zip : string.Empty,
                              CustomerAddress1 = !string.IsNullOrEmpty(UA.Address) ? UA.Address : string.Empty,
                              CustomerAddress2 = !string.IsNullOrEmpty(UA.Address2) ? UA.Address2 : string.Empty,
                              CustomerAddress3 = !string.IsNullOrEmpty(UA.Address3) ? UA.Address3 : string.Empty,
                              State = !string.IsNullOrEmpty(UA.State) ? UA.State : string.Empty,
                              City = !string.IsNullOrEmpty(UA.City) ? UA.City : string.Empty,
                              Suburb = !string.IsNullOrEmpty(UA.Suburb) ? UA.Suburb : string.Empty,

                              ShipFromName = DSA.ContactFirstName + " " + DSA.ContactLastName,
                              ShipFromCompany = DSA.CompanyName,
                              ShipFromAddress = DSA.Address1,
                              ShipFromAddress2 = DSA.Address2,
                              ShipFromCity = DSA.City,
                              ShipFromState = DSA.State,
                              ShipFromPostCode = DSA.Zip,
                              ShipFromCountry = CC.CountryName,
                              ShipFromPhone = DSA.PhoneNo,
                              ShipFromEmail = DSA.Email,

                              ShipToName = DSAD.ContactFirstName + " " + DSAD.ContactLastName,
                              ShipToCompany = DSAD.CompanyName,
                              ShipToAddress = DSAD.Address1,
                              ShipToAddress2 = DSAD.Address2,
                              ShipToCity = DSAD.City,
                              ShipToState = DSAD.State,
                              ShipToPostCode = DSAD.Zip,
                              ShipToCountry = CCA.CountryName,
                              ShipToPhone = DSAD.PhoneNo,
                              ShipToEmail = DSAD.Email,

                              ContentsType = CUD.ContentsType,
                              RestrictionType = CUD.RestrictionType,
                              ContentsExplanation = CUD.ContentsExplanation,
                              NonDeliveryOption = CUD.NonDeliveryOption,
                              CustomsSigner = CUD.CustomsSigner,

                              TrackingNo = DS.TrackingDetail.Replace("Order_", ""),
                              FrayteNumber = DS.FrayteNumber,
                              ParcelType = DS.ParcelType,
                              ShipmentReference = DS.Reference1,
                              ShipmentType = ST.Description,
                              ShipmentWeight = DS.ChargeableWeight.Value,
                              PackageCalculationType = DS.PackageCaculatonType,
                              PaymentPartyTaxAndDuties = DS.PaymentPartyTaxAndDuties,
                              PaymentPartyTaxAndDutiesAcceptedBy = DS.TaxAndDutiesAcceptedBy,
                              TotalCartons = dbContext.DirectShipmentDetails.Where(a => a.DirectShipmentId == DS.DirectShipmentId).ToList().Sum(a => a.CartoonValue),
                              ManifestNumber = dbContext.Manifests.Where(a => a.ManifestId == DS.ManifestId).FirstOrDefault().ManifestName,
                              ShipmentContent = DS.ContentDescription,
                              CollectionDate = DS.CollectionDate ?? null,
                              CollectionTime = DS.CollectionTime.HasValue ? DS.CollectionTime.Value.ToString().Substring(0, 5) : "",
                              CreatedBy = US.ContactName,
                              CreatedOn = DS.CreatedOn,
                              DeclaredValueCurrency = DS.CurrencyCode,
                              EstimatedCost = DS.BaseRate != null && DS.Margin != null ? DS.BaseRate.Value + DS.Margin.Value + DS.AdditionalSurcharge.Value + DS.FuelSurCharge.Value : 0.0m,
                              FuelSurcharge = DS.FuelSurCharge != null ? DS.FuelSurCharge.Value : 0,
                              FuelPercent = DS.FuelSurchargePercent == null ? "0.0" : DS.FuelSurchargePercent.Value.ToString(),
                              FuelMonthYear = DS.FuelMonthYear,
                              PickUpRef = DS.PickUpRef != null ? DS.PickUpRef : "",
                              DeliveryDateValue = DS.DeliveryDate,
                              DeliveryTime = DS.DeliveryTime.HasValue ? DS.DeliveryTime.Value.ToString() : "",
                              SignedBy = DS.SignedBy
                          }).ToList();

            }
            catch (Exception e)
            {

            }

            foreach (var res in Result)
            {
                if (res.DeliveryDateValue != null)
                {
                    res.DeliveryDate = res.DeliveryDateValue.Value.ToString("dd-MMM-yyyy");
                }
                if (res.CollectionDate != null && res.CollectionTime != null)
                {
                    res.CollectionDateandTime = res.CollectionDate.Value.ToString("dd-MMM-yyyy") + " " + res.CollectionTime;
                }
                if (!string.IsNullOrEmpty(res.FuelPercent) && res.FuelMonthYear != null)
                {
                    res.FuelPercent = res.FuelPercent + " (" + UtilityRepository.MonthName(res.FuelMonthYear.Value) + " " + res.FuelMonthYear.Value.Year + ")";
                }
            }
            return Result;
        }

        public string GetDeliveryDate(DateTime DT)
        {
            return DT.ToString("dd-MM-yyyy");
        }

        #region Private Methods

        #region Create Manifest

        private void CreateDirectShipmentsManifest(int ManifestId, string moduleType, List<FrayteUserDirectShipment> manifestShipments)
        {
            if (manifestShipments != null && manifestShipments.Count > 0)
            {
                foreach (var data in manifestShipments)
                {
                    if (moduleType == FrayteShipmentServiceType.DirectBooking)
                    {
                        var shipment = dbContext.DirectShipments.Find(data.ShipmentId);
                        shipment.ManifestId = ManifestId;
                        dbContext.Entry(shipment).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    else if (moduleType == FrayteShipmentServiceType.eCommerce)
                    {
                        var shipment = dbContext.eCommerceShipments.Find(data.ShipmentId);
                        shipment.ManifestId = ManifestId;
                        dbContext.Entry(shipment).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();




                    }
                }
            }

        }

        private string GetFormattedManifestId(int manifestId)
        {

            string manifestName = string.Empty;
            if (manifestId.ToString().Length == 1)
            {
                manifestName = "000" + manifestId.ToString();
            }
            else if (manifestId.ToString().Length == 2)
            {
                manifestName = "00" + manifestId.ToString();
            }
            else if (manifestId.ToString().Length == 3)
            {
                manifestName = "0" + manifestId.ToString();
            }
            else
            {
                manifestName = manifestId.ToString();
            }

            return manifestName;
        }

        private void CreateManifestBarCode(Manifest manifest)
        {
            var userDetail = (from r in dbContext.Users
                              join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                              where r.UserId == manifest.CreatedBy
                              select new
                              {
                                  RoleId = ur.RoleId
                              }
                               ).FirstOrDefault();
            var data = dbContext.spGet_TrackManifest(null, null, 0, 10, "eCommerce", "", manifest.CustomerId, manifest.CreatedBy, userDetail.RoleId, manifest.ManifestName).FirstOrDefault();

            if (data != null)
            {
                SaveManifestBarcode(manifest, data);
            }
        }

        private void SaveManifestBarcode(Manifest manifest, spGet_TrackManifest_Result manifestDetail)
        {
            BarcodeSettings settings = new BarcodeSettings();
            string data = string.Empty;
            string type = "Code128";
            short fontSize = 8;
            string font = "SimSun";

            data = manifest.ManifestName + "|" + manifestDetail.TotalWeight.ToString() + "|" + manifestDetail.TotalNoOfShipments.ToString();

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
            string filePathToSave = AppSettings.eCommerceManifest;
            filePathToSave = HttpContext.Current.Server.MapPath(filePathToSave + manifest.ManifestName);

            if (!System.IO.Directory.Exists(filePathToSave))
                System.IO.Directory.CreateDirectory(filePathToSave);

            //barcode.Save(filePathToSave + "barcode_" + ".Png");
            barcode.Save(System.Web.Hosting.HostingEnvironment.MapPath(AppSettings.eCommerceLabelFolder + "Manifest/" + manifest.ManifestName + ".Png"));
            //Save  ManifestBarCode
            manifest.BarCodeNumber = settings.Data;
            dbContext.Entry(manifest).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }

        #endregion

        #endregion

        #region Avinash Code 

        //Avinash 
        //16-05-2019 

        #region Custommanifest

        public FrayteManifestName GenerateCustomManifest(int ManifestId)
        {
            FrayteManifestName result = new FrayteManifestName();

            var db = (from ex in dbContext.ExpressBags
                      join eb in dbContext.Expresses on ex.BagId equals eb.BagId
                      join hcs in dbContext.HubCarrierServices on eb.HubCarrierServiceId equals hcs.HubCarrierServiceId
                      join hb in dbContext.HubCarriers on hcs.HubCarrierId equals hb.HubCarrierId
                      join hh in dbContext.Hubs on hb.HubId equals hh.HubId
                      join hcss in dbContext.HubCarrierServiceCountryStates on hh.HubId equals hcss.HubId into leftjoin
                      from le in leftjoin.DefaultIfEmpty()
                      where ex.ManifestId == ManifestId
                      select new
                      {
                          Hubcarrier = hb.Carrier,
                          HubId = hh.HubId,
                          StateName = le.StateDisplay == null ? "" : le.StateDisplay
                      }).FirstOrDefault();

            if (db != null)
            {
                if (db.Hubcarrier == FrayteCourierCompany.DPDCH)
                {
                    result = GetCustomManifestSwitzerland(ManifestId);
                    goto End;
                }

                if (db.Hubcarrier.ToUpper() == FrayteCourierCompany.CANADAPOST)
                {
                    result = GetCustomManifestcanada(ManifestId);
                    goto End;
                }

                if (db.Hubcarrier.ToUpper() == FrayteCourierCompany.SKYPOSTAL)
                {
                    result = GetCustomManifestUSA(ManifestId);
                    goto End;
                }

                if (db.Hubcarrier == FrayteCourierCompany.DHL || db.Hubcarrier == FrayteCourierCompany.Yodel || db.Hubcarrier == FrayteCourierCompany.Hermes)
                {
                    result = GetCustomManifestUK(ManifestId);
                    goto End;
                }

                if (db.Hubcarrier == FrayteCourierCompany.DomesticA || db.Hubcarrier == FrayteCourierCompany.DomesticB || db.Hubcarrier == FrayteCourierCompany.EAMExpress)
                {
                    result = GetCustomManifestSIN(ManifestId);
                    goto End;
                }

                if (db.Hubcarrier == FrayteCourierCompany.BRING)
                {
                    result = GetCustomManifestOSL(ManifestId);
                    goto End;
                }

                if (db.Hubcarrier == FrayteCourierCompany.FrayteDomesticJP)
                {
                    result = GetCustomManifestNRT(ManifestId);
                    goto End;
                }
            }
            End:
            return result;
        }

        public FrayteManifestName GetCustomManifestSwitzerland(int ManifestId)
        {
            FrayteManifestName result = new FrayteManifestName();

            var db = (from a in dbContext.ExpressManifests
                      join b in dbContext.ExpressBags on a.ExpressManifestId equals b.ManifestId
                      join c in dbContext.Expresses on b.BagId equals c.BagId
                      join ed in dbContext.ExpressDetails on c.ExpressId equals ed.ExpressId

                      join pc in dbContext.ProductCatalogs on ed.ProductCatalogId equals pc.ProductCatalogId
                      into leftjoin
                      from ps in leftjoin.DefaultIfEmpty()

                      join zrh in dbContext.ProductCatalogDetailSWISSes on ps.ProductCatalogId equals zrh.ProductCatalogId
                         into leftnjoin
                      from pk in leftnjoin.DefaultIfEmpty()

                      join hc in dbContext.HubCarrierServices on c.HubCarrierServiceId equals hc.HubCarrierServiceId
                      join u in dbContext.UserAdditionals on a.CustomerId equals u.UserId
                      join hb in dbContext.HubCarriers on hc.HubCarrierId equals hb.HubCarrierId
                      join hh in dbContext.Hubs on hb.HubId equals hh.HubId
                      join d in dbContext.ExpressAddresses on c.FromAddressId equals d.ExpressAddressId
                      join e in dbContext.ExpressDetails on c.ExpressId equals e.ExpressId
                      join f in dbContext.Countries on d.CountryId equals f.CountryId
                      join g in dbContext.ExpressAddresses on c.ToAddressId equals g.ExpressAddressId
                      join h in dbContext.Countries on g.CountryId equals h.CountryId
                      join td in dbContext.TradelaneShipments on a.TradelaneShipmentId equals td.TradelaneShipmentId
                      join tsa in dbContext.TradelaneShipmentAllocations on td.TradelaneShipmentId equals tsa.TradelaneShipmentId
                      join tsd in dbContext.TradelaneShipmentDetails on tsa.TradelaneShipmentId equals tsd.TradelaneShipmentId
                      join tf in dbContext.TradelaneFlightDetails on td.TradelaneShipmentId equals tf.TradelaneShipmentId
                      where a.ExpressManifestId == ManifestId
                      select new CustomManifest
                      {
                          //Shipper Information
                          ShipperName = d.ContactFirstName + " " + d.ContactLastName,
                          ShipperAddress1 = d.Address1,
                          ShipperAddress2 = d.Address2,
                          ShipperAddress3 = d.Area,
                          ShipperCity = d.City,
                          ShipperZip = d.Zip,
                          ShipperState = d.State,
                          ShipperPhoneNo = d.PhoneNo,
                          ShipperEmail = d.Email,
                          ShipperCountryCode = f.CountryCode,
                          CountryName = f.CountryName,
                          //Consignee Information                         
                          ConsigneeName = g.ContactFirstName + " " + g.ContactLastName,
                          ConsigneeAddress1 = g.Address1,
                          ConsigneeAddress2 = g.Address2,
                          ConsigneeAddress3 = g.Area,
                          ConsigneeCity = g.City,
                          Consigneestate = g.State,
                          ConsigneePhoneNo = g.PhoneNo,
                          ConsigneeEmail = g.Email,
                          Consigneepostcode = g.Zip,
                          Consigneecountry = f.CountryCode2,
                          ExporterCountryName = h.CountryName,
                          //Details 
                          PiecesContent = e.PiecesContent,
                          Weight = e.Weight,
                          ShipmentReference = c.ShipmentReference,
                          DeclaredCurrencyCode = c.DeclaredCurrencyCode,
                          Carrier = hb.Carrier,
                          HubId = hh.HubId,
                          AccountNo = u.AccountNo,
                          TrackingNumber = c.TrackingNumber,
                          MAWBNumber = td.AirlineId + " " + td.MAWB,
                          FlightNumber = tf.FlightNumber,
                          FlightDate = tf.ArrivalDate,
                          HAWBNumber = tsd.HAWB,
                          Pieces = tf.Pieces,
                          NetWeight = tf.TotalWeight,
                          VolumeWeight = tf.TotalVolume,
                          ReferenceInvoiceNR = pk != null ? pk.ReferenceInvoiceNR : "",
                          Currency = pk != null ? pk.Currency : "",
                          VersendLand = pk != null ? pk.VersendLand : "",
                          Anzahl = pk != null ? pk.Anzahl : "",
                          Netto = pk != null ? pk.Netto : "",
                          Brutto = pk != null ? pk.Brutto : "",
                          TarifCode = pk != null ? pk.TarifCode : "",
                          SwissValue = pk != null ? pk.Value : "",
                          OriginCountry = pk != null ? pk.OriginCountry : "",
                          PacType = pk != null ? pk.PacType : "",
                          PacQty = pk != null ? pk.PacQty : "",
                          ZeichenPackstucke = pk != null ? pk.ZeichenPackstucke : "",
                          ArtVorpapier = pk != null ? pk.ArtVorpapier : "",
                          ZeichenVorpapier = pk != null ? pk.ZeichenVorpapier : "",
                          ProductDescription = ps != null ? ps.ProductDescription : "",
                          DepartureAirpotCode = tf.DepartureAirportCode

                      }).ToList();

            if (db.Count > 0 && db != null)
            {
                string myNumber = db[0].AccountNo.Substring(0, 3);

                try
                {
                    string folderpath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");

                    StreamWriter sw;
                    var randomnumber = CommonConversion.GetNewRandonNumber();


                    if (File.Exists(folderpath + "/Custom_Manifest_EXS" + "_" + (db[0].MAWBNumber != null ? db[0].MAWBNumber : "") + "_" + (db[0].DepartureAirpotCode != null ? db[0].DepartureAirpotCode : "") + "_" + (db[0].FlightNumber != null ? db[0].FlightNumber : "") + "_" + (db[0].FlightDate != null ? db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv"))
                    {
                        File.Delete(folderpath + "/Custom_Manifest_EXS" + "_" + (db[0].MAWBNumber != null ? db[0].MAWBNumber : "") + "_" + (db[0].DepartureAirpotCode != null ? db[0].DepartureAirpotCode : "") + "_" + (db[0].FlightNumber != null ? db[0].FlightNumber : "") + "_" + (db[0].FlightDate != null ? db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv");
                        sw = new StreamWriter(folderpath + "/Custom_Manifest_EXS" + "_" + (db[0].MAWBNumber != null ? db[0].MAWBNumber : "") + "_" + (db[0].DepartureAirpotCode != null ? db[0].DepartureAirpotCode : "") + "_" + (db[0].FlightNumber != null ? db[0].FlightNumber : "") + "_" + (db[0].FlightDate != null ? db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv", false);
                        result.FileName = "Custom_Manifest_EXS" + "_" + (db[0].MAWBNumber != null ? db[0].MAWBNumber : "") + "_" + (db[0].DepartureAirpotCode != null ? db[0].DepartureAirpotCode : "") + "_" + (db[0].FlightNumber != null ? db[0].FlightNumber : "") + "_" + (db[0].FlightDate != null ? db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FilePath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/HTMLFile/Custom_Manifest_EXS" + "_" + (db[0].MAWBNumber != null ? db[0].MAWBNumber : "") + "_" + (db[0].DepartureAirpotCode != null ? db[0].DepartureAirpotCode : "") + "_" + (db[0].FlightNumber != null ? db[0].FlightNumber : "") + "_" + (db[0].FlightDate != null ? db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FileStatus = true;
                    }
                    else
                    {
                        sw = new StreamWriter(folderpath + "/Custom_Manifest_EXS" + "_" + db[0].MAWBNumber == null ? "" : db[0].MAWBNumber + "_" + db[0].DepartureAirpotCode == null ? "" : db[0].DepartureAirpotCode + "_" + db[0].FlightNumber == null ? "" : db[0].FlightNumber + "_" + db[0].FlightDate == null ? "" : db[0].FlightDate + "_" + "v2-0" + ".csv", false);
                        result.FileName = "Custom_Manifest_EXS" + "_" + (db[0].MAWBNumber != null ? db[0].MAWBNumber : "") + "_" + (db[0].DepartureAirpotCode != null ? db[0].DepartureAirpotCode : "") + "_" + (db[0].FlightNumber != null ? db[0].FlightNumber : "") + "_" + (db[0].FlightDate != null ? db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FilePath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/HTMLFile/Custom_Manifest_EXS" + "_" + (db[0].MAWBNumber != null ? db[0].MAWBNumber : "") + "_" + (db[0].DepartureAirpotCode != null ? db[0].DepartureAirpotCode : "") + "_" + (db[0].FlightNumber != null ? db[0].FlightNumber : "") + "_" + (db[0].FlightDate != null ? db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FileStatus = true;
                    }

                    int HubId = db[0].HubId;
                    var my_switzerland = dbContext.Hubs.Where(x => x.HubId == HubId).Select(p => p.CustomManifestColumns).FirstOrDefault();
                    string[] columnNames_Swiss = my_switzerland.Split(',');
                    for (var k = 0; k < columnNames_Swiss.Length; k++)
                    {
                        sw.Write(columnNames_Swiss[k]);
                        if (k < columnNames_Swiss.Length)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);

                    for (var l = 0; l < db.Count; l++)
                    {
                        sw.Write(db[l].ShipperName);
                        if (l < db.Count)
                            sw.Write(",");

                        if (db[l].ShipperAddress1 != null)
                        {
                            if (db[l].ShipperAddress1.Contains(","))
                            {
                                sw.Write(db[l].ShipperAddress1.Replace(',', ' '));
                                if (l < db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(db[l].ShipperAddress1);
                                if (l < db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < db.Count)
                                sw.Write(",");
                        }

                        if (db[l].ShipperAddress2 != null)
                        {
                            if (db[l].ShipperAddress2.Contains(","))
                            {
                                sw.Write(db[l].ShipperAddress2.Replace(',', ' '));
                                if (l < db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(db[l].ShipperAddress2);
                                if (l < db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < db.Count)
                                sw.Write(",");
                        }


                        sw.Write(db[l].ShipperCity);
                        if (l < db.Count)
                            sw.Write(",");


                        if (db[l].ShipperState != null)
                        {
                            if (db[l].ShipperState.Contains(","))
                            {
                                sw.Write(db[l].ShipperState.Replace(',', ' '));
                                if (l < db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(db[l].ShipperState);
                                if (l < db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < db.Count)
                                sw.Write(",");
                        }


                        sw.Write(db[l].ShipperZip);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(db[l].CountryName);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(string.Empty);
                        if (l < db.Count)
                            sw.Write(",");


                        sw.Write(db[l].ConsigneeName);
                        if (l < db.Count)
                            sw.Write(",");


                        if (db[l].ConsigneeAddress1 != null)
                        {
                            if (db[l].ConsigneeAddress1.Contains(","))
                            {
                                sw.Write(db[l].ConsigneeAddress1.Replace(',', ' '));
                                if (l < db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(db[l].ConsigneeAddress1);
                                if (l < db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < db.Count)
                                sw.Write(",");
                        }

                        if (db[l].ConsigneeAddress2 != null)
                        {
                            if (db[l].ConsigneeAddress2.Contains(","))
                            {
                                sw.Write(db[l].ConsigneeAddress2.Replace(',', ' '));
                                if (l < db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(db[l].ConsigneeAddress2);
                                if (l < db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < db.Count)
                                sw.Write(",");
                        }


                        sw.Write(db[l].ConsigneeCity);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(db[l].Consigneestate);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(db[l].Consigneepostcode);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(db[l].ExporterCountryName);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(db[l].ReferenceInvoiceNR);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(db[l].VersendLand);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(db[l].Anzahl);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(db[l].Netto);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(db[l].Brutto);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(db[l].TarifCode);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(db[l].ProductDescription);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(db[l].DeclaredCurrencyCode);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(db[l].SwissValue);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(db[l].OriginCountry);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(db[l].PacType);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(db[l].PacQty);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(db[l].ZeichenPackstucke);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(db[l].ArtVorpapier);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(db[l].ZeichenVorpapier);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(string.Empty);
                        if (l < db.Count)
                            sw.Write(",");

                        sw.Write(sw.NewLine);
                    }
                    sw.Close();
                }
                catch
                {
                    result.FileStatus = false;
                    return result;
                }
            }
            return result;
        }

        public FrayteManifestName GetCustomManifestcanada(int ManifestId)
        {
            FrayteManifestName result = new FrayteManifestName();

            var _db = (from a in dbContext.ExpressManifests
                       join b in dbContext.ExpressBags on a.ExpressManifestId equals b.ManifestId
                       join c in dbContext.Expresses on b.BagId equals c.BagId

                       join ed in dbContext.ExpressDetails on c.ExpressId equals ed.ExpressId

                       join pc in dbContext.ProductCatalogs on ed.ProductCatalogId equals pc.ProductCatalogId

                       into leftjoin
                       from ps in leftjoin.DefaultIfEmpty()

                       join zrh in dbContext.ProductCatalogDetailCANs on ps.ProductCatalogId equals zrh.ProductCatalogId
                       into leftnjoin
                       from pk in leftnjoin.DefaultIfEmpty()

                       join hc in dbContext.HubCarrierServices on c.HubCarrierServiceId equals hc.HubCarrierServiceId
                       join u in dbContext.UserAdditionals on a.CustomerId equals u.UserId
                       join hb in dbContext.HubCarriers on hc.HubCarrierId equals hb.HubCarrierId
                       join hh in dbContext.Hubs on hb.HubId equals hh.HubId
                       join d in dbContext.ExpressAddresses on c.FromAddressId equals d.ExpressAddressId
                       join e in dbContext.ExpressDetails on c.ExpressId equals e.ExpressId
                       join f in dbContext.Countries on d.CountryId equals f.CountryId
                       join g in dbContext.ExpressAddresses on c.ToAddressId equals g.ExpressAddressId
                       join h in dbContext.Countries on g.CountryId equals h.CountryId
                       join td in dbContext.TradelaneShipments on a.TradelaneShipmentId equals td.TradelaneShipmentId
                       join tsa in dbContext.TradelaneShipmentAllocations on td.TradelaneShipmentId equals tsa.TradelaneShipmentId
                       join tsd in dbContext.TradelaneShipmentDetails on tsa.TradelaneShipmentId equals tsd.TradelaneShipmentId
                       join tf in dbContext.TradelaneFlightDetails on td.TradelaneShipmentId equals tf.TradelaneShipmentId
                       where a.ExpressManifestId == ManifestId
                       select new CustomManifest
                       {
                           //Shipper Information
                           ShipperName = d.ContactFirstName + " " + d.ContactLastName,
                           ShipperAddress1 = d.Address1,
                           ShipperAddress2 = d.Address2,
                           ShipperAddress3 = d.Area,
                           ShipperCity = d.City,
                           ShipperZip = d.Zip,
                           ShipperState = d.State,
                           ShipperPhoneNo = d.PhoneNo,
                           ShipperEmail = d.Email,
                           ShipperCountryCode = f.CountryCode,
                           CountryName = f.CountryName,
                           //Consignee Information                         
                           ConsigneeName = g.ContactFirstName + " " + g.ContactLastName,
                           ConsigneeAddress1 = g.Address1,
                           ConsigneeAddress2 = g.Address2,
                           ConsigneeAddress3 = g.Area,
                           ConsigneeCity = g.City,
                           Consigneestate = g.State,
                           ConsigneePhoneNo = g.PhoneNo,
                           ConsigneeEmail = g.Email,
                           Consigneepostcode = g.Zip,
                           Consigneecountry = f.CountryCode2,
                           ExporterCountryName = h.CountryName,
                           //Details 
                           PiecesContent = e.PiecesContent,
                           Weight = e.Weight,
                           ShipmentReference = c.ShipmentReference,
                           DeclaredCurrencyCode = c.DeclaredCurrencyCode,
                           Carrier = hb.Carrier,
                           HubId = hh.HubId,
                           AccountNo = u.AccountNo,
                           TrackingNumber = c.TrackingNumber,
                           MAWBNumber = td.AirlineId + " " + td.MAWB,
                           FlightNumber = tf.FlightNumber,
                           FlightDate = tf.ArrivalDate,
                           HAWBNumber = tsd.HAWB,
                           Pieces = tf.Pieces,
                           NetWeight = tf.TotalWeight,
                           VolumeWeight = tf.TotalVolume,
                           Reference = pk != null ? pk.Reference : "",
                           InternalAccountnumber = pk != null ? pk.InternalAccountnumber : "",
                           WeightUOM = pk != null ? pk.WeightUOM : "",
                           TotalValue = pk != null ? pk.TotalValue : "",
                           CanadaCurrency = pk != null ? pk.Currency : "",
                           ItemHSCode = pk != null ? pk.ItemHSCode : "",
                           CanadaPieces = pk != null ? pk.Pieces : "",
                           SKU = pk != null ? pk.SKU : "",
                           ProvinceCode = pk != null ? pk.ProvinceCode.ToUpper() : "",
                           ProductDescription = ps != null ? ps.ProductDescription : "",
                           Incoterm = "DDP",
                           DepartureAirpotCode = tf.DepartureAirportCode

                       }).ToList();

            if (_db.Count > 0 && _db != null)
            {
                string myNumber = _db[0].AccountNo.Substring(0, 3);
                try
                {
                    string folderpath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");

                    StreamWriter sw;
                    var randomnumber = CommonConversion.GetNewRandonNumber();

                    if (File.Exists(folderpath + "/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv"))
                    {
                        File.Delete(folderpath + "/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv");
                        sw = new StreamWriter(folderpath + "/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv", false);
                        result.FileName = "Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FilePath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/HTMLFile/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FileStatus = true;
                    }
                    else
                    {
                        sw = new StreamWriter(folderpath + "/Custom_Manifest_EXS" + "_" + _db[0].MAWBNumber == null ? "" : _db[0].MAWBNumber + "_" + _db[0].DepartureAirpotCode == null ? "" : _db[0].DepartureAirpotCode + "_" + _db[0].FlightNumber == null ? "" : _db[0].FlightNumber + "_" + _db[0].FlightDate == null ? "" : _db[0].FlightDate + "_" + "v2-0" + ".csv", false);
                        result.FileName = "Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FilePath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/HTMLFile/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FileStatus = true;
                    }

                    int HubId = _db[0].HubId;
                    var my_canada = dbContext.Hubs.Where(x => x.HubId == HubId).Select(p => p.CustomManifestColumns).FirstOrDefault();
                    string[] columnNames_canada = my_canada.Split(',');
                    for (var k = 0; k < columnNames_canada.Length; k++)
                    {
                        sw.Write(columnNames_canada[k]);

                        if (k < columnNames_canada.Length)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);
                    for (var l = 0; l < _db.Count; l++)
                    {

                        sw.Write(_db[l].TrackingNumber);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Reference);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].InternalAccountnumber);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ShipperName);
                        if (l < _db.Count)
                            sw.Write(",");

                        if (_db[l].ShipperAddress1 != null)
                        {
                            if (_db[l].ShipperAddress1.Contains(","))
                            {
                                sw.Write(_db[l].ShipperAddress1.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ShipperAddress1);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        if (_db[l].ShipperAddress2 != null)
                        {
                            if (_db[l].ShipperAddress2.Contains(","))
                            {
                                sw.Write(_db[l].ShipperAddress2.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ShipperAddress2);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        if (_db[l].ShipperAddress3 != null)
                        {
                            if (_db[l].ShipperAddress3.Contains(","))
                            {
                                sw.Write(_db[l].ShipperAddress3.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ShipperAddress3);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        sw.Write(_db[l].ShipperCity);
                        if (l < _db.Count)
                            sw.Write(",");


                        sw.Write(_db[l].ShipperState);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ShipperZip);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ShipperCountryCode);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ConsigneeName);
                        if (l < _db.Count)
                            sw.Write(",");


                        if (_db[l].ConsigneeAddress1 != null)
                        {
                            if (_db[l].ConsigneeAddress1.Contains(","))
                            {
                                sw.Write(_db[l].ConsigneeAddress1.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ConsigneeAddress1);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        if (_db[l].ConsigneeAddress2 != null)
                        {
                            if (_db[l].ConsigneeAddress2.Contains(","))
                            {
                                sw.Write(_db[l].ConsigneeAddress2.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ConsigneeAddress2);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }


                        if (_db[l].ConsigneeAddress3 != null)
                        {
                            if (_db[l].ConsigneeAddress3.Contains(","))
                            {
                                sw.Write(_db[l].ConsigneeAddress3.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ConsigneeAddress3);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }


                        sw.Write(_db[l].ConsigneeCity);
                        if (l < _db.Count)
                            sw.Write(",");


                        sw.Write(_db[l].Consigneestate);
                        if (l < _db.Count)
                            sw.Write(",");


                        sw.Write(_db[l].ProvinceCode);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Consigneepostcode);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Consigneecountry);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ConsigneeEmail);
                        if (l < _db.Count)
                            sw.Write(",");


                        sw.Write(_db[l].ConsigneePhoneNo);
                        if (l < _db.Count)
                            sw.Write(",");


                        sw.Write(_db[l].CanadaPieces);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Weight);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].WeightUOM);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].TotalValue);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Currency);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Incoterm);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ProductDescription);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ItemHSCode);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].CartonQty);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Value);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ShipperCountryCode);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].SKU);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(sw.NewLine);
                    }
                    sw.Close();
                }
                catch (Exception ex)
                {
                    result.FileStatus = false;
                    return result;
                }
            }
            return result;
        }

        public FrayteManifestName GetCustomManifestUSA(int ManifestId)
        {
            FrayteManifestName result = new FrayteManifestName();

            var _db = (from a in dbContext.ExpressManifests
                       join b in dbContext.ExpressBags on a.ExpressManifestId equals b.ManifestId
                       join c in dbContext.Expresses on b.BagId equals c.BagId

                       join ed in dbContext.ExpressDetails on c.ExpressId equals ed.ExpressId

                       join pc in dbContext.ProductCatalogs on ed.ProductCatalogId equals pc.ProductCatalogId
                       into leftjoin
                       from ps in leftjoin.DefaultIfEmpty()

                       join zrh in dbContext.ProductCatalogDetailUSAs on ps.ProductCatalogId equals zrh.ProductCatalogId
                       into leftnjoin
                       from pk in leftnjoin.DefaultIfEmpty()

                       join hc in dbContext.HubCarrierServices on c.HubCarrierServiceId equals hc.HubCarrierServiceId
                       join u in dbContext.UserAdditionals on a.CustomerId equals u.UserId
                       join hb in dbContext.HubCarriers on hc.HubCarrierId equals hb.HubCarrierId
                       join hh in dbContext.Hubs on hb.HubId equals hh.HubId
                       join d in dbContext.ExpressAddresses on c.FromAddressId equals d.ExpressAddressId
                       join e in dbContext.ExpressDetails on c.ExpressId equals e.ExpressId
                       join f in dbContext.Countries on d.CountryId equals f.CountryId
                       join g in dbContext.ExpressAddresses on c.ToAddressId equals g.ExpressAddressId
                       join h in dbContext.Countries on g.CountryId equals h.CountryId
                       join td in dbContext.TradelaneShipments on a.TradelaneShipmentId equals td.TradelaneShipmentId
                       join tsa in dbContext.TradelaneShipmentAllocations on td.TradelaneShipmentId equals tsa.TradelaneShipmentId
                       join tsd in dbContext.TradelaneShipmentDetails on tsa.TradelaneShipmentId equals tsd.TradelaneShipmentId
                       join tf in dbContext.TradelaneFlightDetails on td.TradelaneShipmentId equals tf.TradelaneShipmentId
                       where a.ExpressManifestId == ManifestId
                       select new CustomManifest
                       {
                           //Shipper Information
                           ShipperName = d.ContactFirstName + " " + d.ContactLastName,
                           ShipperAddress1 = d.Address1,
                           ShipperAddress2 = d.Address2,
                           ShipperAddress3 = d.Area,
                           ShipperCity = d.City,
                           ShipperZip = d.Zip,
                           ShipperState = d.State,
                           ShipperPhoneNo = d.PhoneNo,
                           ShipperEmail = d.Email,
                           ShipperCountryCode = f.CountryCode,
                           CountryName = f.CountryName,
                           //Consignee Information                         
                           ConsigneeName = g.ContactFirstName + " " + g.ContactLastName,
                           ConsigneeAddress1 = g.Address1,
                           ConsigneeAddress2 = g.Address2,
                           ConsigneeAddress3 = g.Area,
                           ConsigneeCity = g.City,
                           Consigneestate = g.State,
                           ConsigneePhoneNo = g.PhoneNo,
                           ConsigneeEmail = g.Email,
                           Consigneepostcode = g.Zip,
                           Consigneecountry = f.CountryCode2,
                           ExporterCountryName = h.CountryName,
                           //Details 
                           PiecesContent = e.PiecesContent,
                           Weight = e.Weight,
                           ShipmentReference = c.ShipmentReference,
                           DeclaredCurrencyCode = c.DeclaredCurrencyCode,
                           Carrier = hb.Carrier,
                           HubId = hh.HubId,
                           HubCode = hh.Code,
                           AccountNo = u.AccountNo,
                           TrackingNumber = c.TrackingNumber,
                           MAWBNumber = td.AirlineId + " " + td.MAWB,
                           FlightNumber = tf.FlightNumber,
                           FlightDate = tf.ArrivalDate,
                           HAWBNumber = tsd.HAWB,
                           Pieces = tf.Pieces,
                           NetWeight = tf.TotalWeight,
                           VolumeWeight = tf.TotalVolume,
                           Reference = pk.Reference,
                           InternalAccountnumber = pk.InternalAccountnumber,
                           WeightUOM = pk != null ? pk.WeightUOM : "",
                           CanadaCurrency = pk != null ? pk.Currency : "",
                           ItemHSCode = pk != null ? pk.ItemHSCode : "",
                           CanadaPieces = pk != null ? pk.Pieces : "",
                           ProductDescription = ps.ProductDescription,
                           CustomEntryType = pk != null ? pk.CustomEntryType : "",
                           CustomTotalValue = pk != null ? pk.CustomTotalValue : "",
                           CustomTotalVAT = pk != null ? pk.CustomTotalVAT : "",
                           CustomDuty = pk != null ? pk.CustomDuty : "",
                           Incoterm = "DDP",
                           Itemvalue = pk != null ? pk.ItemValue : "",
                           CustomCommodityMap = pk != null ? pk.CustomCommodityMap : "",
                           ECommerceShipmentId = pk != null ? pk.ECommerceShipmentId : "",
                           DepartureAirpotCode = tf.DepartureAirportCode

                       }).ToList();

            if (_db.Count > 0 && _db != null)
            {
                string myNumber = _db[0].AccountNo.Substring(0, 3);

                try
                {
                    string folderpath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");

                    StreamWriter sw;
                    var randomnumber = CommonConversion.GetNewRandonNumber();

                    if (File.Exists(folderpath + "/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv"))
                    {
                        File.Delete(folderpath + "/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv");
                        sw = new StreamWriter(folderpath + "/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv", false);
                        result.FileName = "Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FilePath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/HTMLFile/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FileStatus = true;
                    }
                    else
                    {
                        sw = new StreamWriter(folderpath + "/Custom_Manifest_EXS" + "_" + _db[0].MAWBNumber == null ? "" : _db[0].MAWBNumber + "_" + _db[0].DepartureAirpotCode == null ? "" : _db[0].DepartureAirpotCode + "_" + _db[0].FlightNumber == null ? "" : _db[0].FlightNumber + "_" + _db[0].FlightDate == null ? "" : _db[0].FlightDate + "_" + "v2-0" + ".csv", false);
                        result.FileName = "Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FilePath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/HTMLFile/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FileStatus = true;
                    }

                    int HubId = _db[0].HubId;
                    var my_USA = dbContext.Hubs.Where(x => x.HubId == HubId).Select(p => p.CustomManifestColumns).FirstOrDefault();
                    string[] columnNames_USA = my_USA.Split(',');
                    for (var k = 0; k < columnNames_USA.Length; k++)
                    {
                        sw.Write(columnNames_USA[k]);

                        if (k < columnNames_USA.Length)
                        {
                            sw.Write(",");
                        }
                    }

                    sw.Write(sw.NewLine);
                    for (var l = 0; l < _db.Count; l++)
                    {
                        sw.Write(_db[l].ECommerceShipmentId);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].TrackingNumber);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Reference);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].InternalAccountnumber);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ShipperName);
                        if (l < _db.Count)
                            sw.Write(",");

                        if (_db[l].ShipperAddress1 != null)
                        {
                            if (_db[l].ShipperAddress1.Contains(","))
                            {
                                sw.Write(_db[l].ShipperAddress1.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ShipperAddress1);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        if (_db[l].ShipperAddress2 != null)
                        {
                            if (_db[l].ShipperAddress2.Contains(","))
                            {
                                sw.Write(_db[l].ShipperAddress2.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ShipperAddress2);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        if (_db[l].ShipperAddress3 != null)
                        {
                            if (_db[l].ShipperAddress3.Contains(","))
                            {
                                sw.Write(_db[l].ShipperAddress3.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ShipperAddress3);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        sw.Write(_db[l].ShipperCity);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ShipperZip);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ShipperState);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ShipperPhoneNo);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ShipperEmail);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ShipperCountryCode);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ConsigneeName);
                        if (l < _db.Count)
                            sw.Write(",");


                        if (_db[l].ConsigneeAddress1 != null)
                        {
                            if (_db[l].ConsigneeAddress1.Contains(","))
                            {
                                sw.Write(_db[l].ConsigneeAddress1.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ConsigneeAddress1);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        if (_db[l].ConsigneeAddress2 != null)
                        {
                            if (_db[l].ConsigneeAddress2.Contains(","))
                            {
                                sw.Write(_db[l].ConsigneeAddress2.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ConsigneeAddress2);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }


                        if (_db[l].ConsigneeAddress3 != null)
                        {
                            if (_db[l].ConsigneeAddress3.Contains(","))
                            {
                                sw.Write(_db[l].ConsigneeAddress3.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ConsigneeAddress3);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }


                        sw.Write(_db[l].ConsigneeCity);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Consigneepostcode);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Consigneestate);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ConsigneePhoneNo);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ConsigneeEmail);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Consigneecountry);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Pieces);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Weight);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].WeightUOM);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].TotalValue);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Currency);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Incoterm);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ProductDescription);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ItemHSCode);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].CartonQty);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Itemvalue);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].CustomCommodityMap);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].CustomEntryType);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].CustomTotalValue);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].CustomTotalVAT);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].CustomDuty);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(sw.NewLine);
                    }
                    sw.Close();
                }
                catch
                {
                    result.FileStatus = false;
                    return result;
                }
            }
            return result;
        }

        public FrayteManifestName GetCustomManifestNRT(int ManifestId)
        {
            FrayteManifestName result = new FrayteManifestName();

            var _db = (from a in dbContext.ExpressManifests
                       join b in dbContext.ExpressBags on a.ExpressManifestId equals b.ManifestId
                       join c in dbContext.Expresses on b.BagId equals c.BagId
                       join ed in dbContext.ExpressDetails on c.ExpressId equals ed.ExpressId

                       join pc in dbContext.ProductCatalogs on ed.ProductCatalogId equals pc.ProductCatalogId into leftjoin
                       from ps in leftjoin.DefaultIfEmpty()

                       join zrh in dbContext.ProductCatalogDetailJapans on ps.ProductCatalogId equals zrh.ProductCatalogId into leftnjoin
                       from pk in leftnjoin.DefaultIfEmpty()

                       join hc in dbContext.HubCarrierServices on c.HubCarrierServiceId equals hc.HubCarrierServiceId
                       join u in dbContext.UserAdditionals on a.CustomerId equals u.UserId
                       join hb in dbContext.HubCarriers on hc.HubCarrierId equals hb.HubCarrierId
                       join hh in dbContext.Hubs on hb.HubId equals hh.HubId
                       join d in dbContext.ExpressAddresses on c.FromAddressId equals d.ExpressAddressId
                       join e in dbContext.ExpressDetails on c.ExpressId equals e.ExpressId
                       join f in dbContext.Countries on d.CountryId equals f.CountryId
                       join g in dbContext.ExpressAddresses on c.ToAddressId equals g.ExpressAddressId
                       join h in dbContext.Countries on g.CountryId equals h.CountryId
                       join td in dbContext.TradelaneShipments on a.TradelaneShipmentId equals td.TradelaneShipmentId
                       join tsa in dbContext.TradelaneShipmentAllocations on td.TradelaneShipmentId equals tsa.TradelaneShipmentId
                       join tsd in dbContext.TradelaneShipmentDetails on tsa.TradelaneShipmentId equals tsd.TradelaneShipmentId
                       join tf in dbContext.TradelaneFlightDetails on td.TradelaneShipmentId equals tf.TradelaneShipmentId
                       where a.ExpressManifestId == ManifestId
                       select new CustomManifest
                       {
                           //Shipper Information
                           ShipperName = d.ContactFirstName + " " + d.ContactLastName,
                           ShipperAddress1 = d.Address1,
                           ShipperAddress2 = d.Address2,
                           ShipperAddress3 = d.Area,
                           ShipperCity = d.City,
                           ShipperZip = d.Zip,
                           ShipperState = d.State,
                           ShipperPhoneNo = d.PhoneNo,
                           ShipperEmail = d.Email,
                           ShipperCountryCode = f.CountryCode,
                           CountryName = f.CountryName,
                           // Consignee Information
                           ConsigneeName = g.ContactFirstName + " " + g.ContactLastName,
                           ConsigneeAddress1 = g.Address1,
                           ConsigneeAddress2 = g.Address2,
                           ConsigneeAddress3 = g.Area,
                           ConsigneeCity = g.City,
                           Consigneestate = g.State,
                           ConsigneePhoneNo = g.PhoneNo,
                           ConsigneeEmail = g.Email,
                           Consigneepostcode = g.Zip,
                           Consigneecountry = f.CountryCode2,
                           ExporterCountryName = h.CountryName,
                           //  Details
                           PiecesContent = e.PiecesContent,
                           Weight = e.Weight,
                           ShipmentReference = c.ShipmentReference,
                           DeclaredCurrencyCode = c.DeclaredCurrencyCode,
                           Value = e.Value,
                           Carrier = hb.Carrier,
                           HubId = hh.HubId,
                           AccountNo = u.AccountNo,
                           TrackingNumber = c.TrackingNumber,
                           MAWBNumber = td.MAWB,
                           FlightNumber = tsa.FlightNumber,
                           FlightDate = tsa.EstimatedDateofDelivery,
                           HAWBNumber = tsd.HAWB,
                           Pieces = tf.Pieces,
                           NetWeight = tf.TotalWeight,
                           VolumeWeight = tf.TotalVolume,
                           Incoterm = "DDP",
                           No = pk != null ? pk.No : "",
                           CodeKg = pk != null ? pk.CodeKg : "",
                           DeclaredValueOfCustom = pk != null ? pk.DeclaredValueOfCustom : "",
                           Currency = pk != null ? pk.Currency : "",
                           Origin = pk != null ? pk.Origin : "",
                           OriginCode = pk != null ? pk.OriginCode : "",
                           ProductDescription = ps != null ? ps.ProductDescription : "",
                           DepartureAirpotCode = tf.DepartureAirportCode
                       }).ToList();
            if (_db.Count > 0 && _db != null)
            {
                string myNumber = _db[0].AccountNo.Substring(0, 3);
                try
                {
                    string folderpath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");

                    StreamWriter sw;
                    var randomnumber = CommonConversion.GetNewRandonNumber();

                    if (File.Exists(folderpath + "/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv"))
                    {
                        File.Delete(folderpath + "/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv");
                        sw = new StreamWriter(folderpath + "/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv", false);
                        result.FileName = "Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FilePath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/HTMLFile/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FileStatus = true;
                    }
                    else
                    {
                        sw = new StreamWriter(folderpath + "/Custom_Manifest_EXS" + "_" + _db[0].MAWBNumber == null ? "" : _db[0].MAWBNumber + "_" + _db[0].DepartureAirpotCode == null ? "" : _db[0].DepartureAirpotCode + "_" + _db[0].FlightNumber == null ? "" : _db[0].FlightNumber + "_" + _db[0].FlightDate == null ? "" : _db[0].FlightDate + "_" + "v2-0" + ".csv", false);
                        result.FileName = "Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FilePath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/HTMLFile/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FileStatus = true;
                    }

                    int HubId = _db[0].HubId;
                    var my_NRT = dbContext.Hubs.Where(x => x.HubId == HubId).Select(p => p.CustomManifestColumns).FirstOrDefault();
                    string[] columnNames_NRT = my_NRT.Split(',');
                    for (var k = 0; k < columnNames_NRT.Length; k++)
                    {
                        sw.Write(columnNames_NRT[k]);

                        if (k < columnNames_NRT.Length)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);

                    for (var l = 0; l < _db.Count; l++)
                    {
                        sw.Write(_db[l].MAWBNumber);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].FlightNumber);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].FlightDate.HasValue ? _db[l].FlightDate.Value.ToString("dd-MMM-yyyy") : "01-01-2019");
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].No);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].HAWBNumber);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Pieces);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].NetWeight);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].VolumeWeight);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].CodeKg);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ProductDescription);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ConsigneeName);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ConsigneeCity);
                        if (l < _db.Count)
                            sw.Write(",");

                        if (_db[l].ConsigneeAddress1 != null)
                        {
                            if (_db[l].ConsigneeAddress1.Contains(","))
                            {
                                sw.Write(_db[l].ConsigneeAddress1.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ConsigneeAddress1);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        sw.Write(_db[l].ConsigneePhoneNo);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ConsigneePhoneNo);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Consigneepostcode);
                        if (l < _db.Count)
                            sw.Write(",");


                        sw.Write(_db[l].ShipperName);
                        if (l < _db.Count)
                            sw.Write(",");

                        if (_db[l].ShipperAddress1 != null)
                        {
                            if (_db[l].ShipperAddress1.Contains(","))
                            {
                                sw.Write(_db[l].ShipperAddress1.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ShipperAddress1);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        sw.Write(_db[l].ShipperPhoneNo);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ShipperZip);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ShipperPhoneNo);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Currency);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].DeclaredValueOfCustom);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Origin);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].OriginCode);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(sw.NewLine);
                    }
                    sw.Close();
                }
                catch (Exception)
                {
                    result.FileStatus = false;
                    return result;
                }
            }
            return result;
        }

        public FrayteManifestName GetCustomManifestUK(int ManifestId)
        {
            FrayteManifestName result = new FrayteManifestName();

            var _db = (from a in dbContext.ExpressManifests
                       join b in dbContext.ExpressBags on a.ExpressManifestId equals b.ManifestId
                       join c in dbContext.Expresses on b.BagId equals c.BagId
                       join ed in dbContext.ExpressDetails on c.ExpressId equals ed.ExpressId

                       join pc in dbContext.ProductCatalogs on ed.ProductCatalogId equals pc.ProductCatalogId into leftjoin
                       from ps in leftjoin.DefaultIfEmpty()

                       join zrh in dbContext.ProductCatalogDetailUKs on ps.ProductCatalogId equals zrh.ProductCatalogId into leftnjoin
                       from pk in leftnjoin.DefaultIfEmpty()

                       join hc in dbContext.HubCarrierServices on c.HubCarrierServiceId equals hc.HubCarrierServiceId
                       join u in dbContext.UserAdditionals on a.CustomerId equals u.UserId
                       join hb in dbContext.HubCarriers on hc.HubCarrierId equals hb.HubCarrierId
                       join hh in dbContext.Hubs on hb.HubId equals hh.HubId
                       join d in dbContext.ExpressAddresses on c.FromAddressId equals d.ExpressAddressId
                       join e in dbContext.ExpressDetails on c.ExpressId equals e.ExpressId
                       join f in dbContext.Countries on d.CountryId equals f.CountryId
                       join g in dbContext.ExpressAddresses on c.ToAddressId equals g.ExpressAddressId
                       join h in dbContext.Countries on g.CountryId equals h.CountryId
                       join td in dbContext.TradelaneShipments on a.TradelaneShipmentId equals td.TradelaneShipmentId
                       join tsa in dbContext.TradelaneShipmentAllocations on td.TradelaneShipmentId equals tsa.TradelaneShipmentId
                       join tsd in dbContext.TradelaneShipmentDetails on tsa.TradelaneShipmentId equals tsd.TradelaneShipmentId
                       join tf in dbContext.TradelaneFlightDetails on td.TradelaneShipmentId equals tf.TradelaneShipmentId
                       where a.ExpressManifestId == ManifestId
                       select new CustomManifest
                       {
                           ShipperName = d.ContactFirstName + " " + d.ContactLastName,
                           ShipperAddress1 = d.Address1,
                           ShipperAddress2 = d.Address2,
                           ShipperAddress3 = d.Area,
                           ShipperCity = d.City,
                           ShipperZip = d.Zip,
                           ShipperState = d.State,
                           ShipperPhoneNo = d.PhoneNo,
                           ShipperEmail = d.Email,
                           ShipperCountryCode = f.CountryCode,
                           CountryName = f.CountryName,
                           //Consignee Information                         
                           ConsigneeName = g.ContactFirstName + " " + g.ContactLastName,
                           ConsigneeAddress1 = g.Address1,
                           ConsigneeAddress2 = g.Address2,
                           ConsigneeAddress3 = g.Area,
                           ConsigneeCity = g.City,
                           Consigneestate = g.State,
                           ConsigneePhoneNo = g.PhoneNo,
                           ConsigneeEmail = g.Email,
                           Consigneepostcode = g.Zip,
                           Consigneecountry = f.CountryCode2,
                           ExporterCountryName = h.CountryName,

                           PiecesContent = e.PiecesContent,
                           Weight = e.Weight,
                           ShipmentReference = c.ShipmentReference,
                           DeclaredCurrencyCode = c.DeclaredCurrencyCode,
                           Value = e.Value,
                           Carrier = hb.Carrier,
                           HubId = hh.HubId,
                           AccountNo = u.AccountNo,
                           TrackingNumber = c.TrackingNumber,
                           MAWBNumber = td.AirlineId + " " + td.MAWB,
                           FlightNumber = tf.FlightNumber,
                           FlightDate = tf.ArrivalDate,
                           HAWBNumber = tsd.HAWB,
                           Pieces = tf.Pieces,
                           NetWeight = tf.TotalWeight,
                           VolumeWeight = tf.TotalVolume,
                           Vat = pk != null ? pk.Vat : "",
                           Pce = pk != null ? pk.Pce : "",
                           Number = pk != null ? pk.Number : "",
                           TCode = pk != null ? pk.TCode : "",
                           UKCurrency = pk != null ? pk.Currency : "",
                           UKValue = pk != null ? pk.Value : "",
                           Weblink = pk != null ? pk.Weblink : "",
                           ProductDescription = ps != null ? ps.ProductDescription : "",
                           DepartureAirpotCode = tf.DepartureAirportCode

                       }).ToList();

            if (_db.Count > 0 && _db != null)
            {
                string myNumber = _db[0].AccountNo.Substring(0, 3);

                try
                {
                    string folderpath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");

                    StreamWriter sw;
                    var randomnumber = CommonConversion.GetNewRandonNumber();

                    if (File.Exists(folderpath + "/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv"))
                    {
                        File.Delete(folderpath + "/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv");
                        sw = new StreamWriter(folderpath + "/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv", false);
                        result.FileName = "Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FilePath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/HTMLFile/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FileStatus = true;
                    }
                    else
                    {
                        sw = new StreamWriter(folderpath + "/Custom_Manifest_EXS" + "_" + _db[0].MAWBNumber == null ? "" : _db[0].MAWBNumber + "_" + _db[0].DepartureAirpotCode == null ? "" : _db[0].DepartureAirpotCode + "_" + _db[0].FlightNumber == null ? "" : _db[0].FlightNumber + "_" + _db[0].FlightDate == null ? "" : _db[0].FlightDate + "_" + "v2-0" + ".csv", false);
                        result.FileName = "Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FilePath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/HTMLFile/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FileStatus = true;
                    }

                    int hubid = _db[0].HubId;
                    var my_uk = dbContext.Hubs.Where(x => x.HubId == hubid).Select(p => p.CustomManifestColumns).FirstOrDefault();
                    string[] columnnames_uk = my_uk.Split(',');
                    for (var k = 0; k < columnnames_uk.Length; k++)
                    {
                        sw.Write(columnnames_uk[k]);

                        if (k < columnnames_uk.Length)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);

                    for (var l = 0; l < _db.Count; l++)
                    {

                        sw.Write(_db[l].ShipperName);
                        if (l < _db.Count)
                            sw.Write(",");

                        if (_db[l].ShipperAddress1 != null)
                        {
                            if (_db[l].ShipperAddress1.Contains(","))
                            {
                                sw.Write(_db[l].ShipperAddress1.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ShipperAddress1);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        if (_db[l].ShipperAddress2 != null)
                        {
                            if (_db[l].ShipperAddress2.Contains(","))
                            {
                                sw.Write(_db[l].ShipperAddress2.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ShipperAddress2);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        sw.Write(_db[l].ShipperCity);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ShipperState);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ShipperZip);
                        if (l < _db.Count)
                            sw.Write(",");


                        sw.Write(_db[l].ShipperCountryCode);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Vat);
                        if (l < _db.Count)
                            sw.Write(",");


                        sw.Write(_db[l].ConsigneeName);
                        if (l < _db.Count)
                            sw.Write(",");


                        if (_db[l].ConsigneeAddress1 != null)
                        {
                            if (_db[l].ConsigneeAddress1.Contains(","))
                            {
                                sw.Write(_db[l].ConsigneeAddress1.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ConsigneeAddress1);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        if (_db[l].ConsigneeAddress2 != null)
                        {
                            if (_db[l].ConsigneeAddress2.Contains(","))
                            {
                                sw.Write(_db[l].ConsigneeAddress2.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ConsigneeAddress2);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        sw.Write(_db[l].ConsigneeCity);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Consigneestate);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Consigneepostcode);
                        if (l < _db.Count)
                            sw.Write(",");


                        sw.Write(_db[l].Consigneecountry);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Pce);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Weight);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Number);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].TCode);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ProductDescription);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Currency);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].UKValue);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Weblink);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(sw.NewLine);
                    }

                    sw.Close();
                }
                catch (Exception ex)
                {
                    result.FileStatus = false;
                    return result;
                }
            }
            return result;
        }

        public FrayteManifestName GetCustomManifestOSL(int ManifestId)
        {
            FrayteManifestName result = new FrayteManifestName();

            var _db = (from a in dbContext.ExpressManifests
                       join b in dbContext.ExpressBags on a.ExpressManifestId equals b.ManifestId
                       join c in dbContext.Expresses on b.BagId equals c.BagId
                       join ed in dbContext.ExpressDetails on c.ExpressId equals ed.ExpressId

                       join pc in dbContext.ProductCatalogs on ed.ProductCatalogId equals pc.ProductCatalogId into leftjoin
                       from ps in leftjoin.DefaultIfEmpty()

                       join zrh in dbContext.ProductCatalogDetailNorways on ps.ProductCatalogId equals zrh.ProductCatalogId into leftnjoin
                       from pk in leftnjoin.DefaultIfEmpty()

                       join hc in dbContext.HubCarrierServices on c.HubCarrierServiceId equals hc.HubCarrierServiceId
                       join u in dbContext.UserAdditionals on a.CustomerId equals u.UserId
                       join hb in dbContext.HubCarriers on hc.HubCarrierId equals hb.HubCarrierId
                       join hh in dbContext.Hubs on hb.HubId equals hh.HubId
                       join d in dbContext.ExpressAddresses on c.FromAddressId equals d.ExpressAddressId
                       join e in dbContext.ExpressDetails on c.ExpressId equals e.ExpressId
                       join f in dbContext.Countries on d.CountryId equals f.CountryId
                       join g in dbContext.ExpressAddresses on c.ToAddressId equals g.ExpressAddressId
                       join h in dbContext.Countries on g.CountryId equals h.CountryId
                       join td in dbContext.TradelaneShipments on a.TradelaneShipmentId equals td.TradelaneShipmentId
                       join tsa in dbContext.TradelaneShipmentAllocations on td.TradelaneShipmentId equals tsa.TradelaneShipmentId
                       join tsd in dbContext.TradelaneShipmentDetails on tsa.TradelaneShipmentId equals tsd.TradelaneShipmentId
                       join tf in dbContext.TradelaneFlightDetails on td.TradelaneShipmentId equals tf.TradelaneShipmentId
                       where a.ExpressManifestId == ManifestId
                       select new CustomManifest
                       {
                           ShipperName = d.ContactFirstName + " " + d.ContactLastName,
                           ShipperAddress1 = d.Address1,
                           ShipperAddress2 = d.Address2,
                           ShipperAddress3 = d.Area,
                           ShipperCity = d.City,
                           ShipperZip = d.Zip,
                           ShipperState = d.State,
                           ShipperPhoneNo = d.PhoneNo,
                           ShipperEmail = d.Email,
                           ShipperCountryCode = f.CountryCode,
                           CountryName = f.CountryName,
                           //Consignee Information                         
                           ConsigneeName = g.ContactFirstName + " " + g.ContactLastName,
                           ConsigneeAddress1 = g.Address1,
                           ConsigneeAddress2 = g.Address2,
                           ConsigneeAddress3 = g.Area,
                           ConsigneeCity = g.City,
                           Consigneestate = g.State,
                           ConsigneePhoneNo = g.PhoneNo,
                           ConsigneeEmail = g.Email,
                           Consigneepostcode = g.Zip,
                           Consigneecountry = f.CountryCode2,
                           ExporterCountryName = h.CountryName,

                           PiecesContent = e.PiecesContent,
                           Weight = e.Weight,
                           ShipmentReference = c.ShipmentReference,
                           DeclaredCurrencyCode = c.DeclaredCurrencyCode,
                           Value = e.Value,
                           Carrier = hb.Carrier,
                           HubId = hh.HubId,
                           AccountNo = u.AccountNo,
                           TrackingNumber = c.TrackingNumber,
                           MAWBNumber = td.AirlineId + " " + td.MAWB,
                           FlightNumber = tf.FlightNumber,
                           FlightDate = tf.ArrivalDate,
                           HAWBNumber = tsd.HAWB,
                           Pieces = tf.Pieces,
                           NetWeight = tf.TotalWeight,
                           VolumeWeight = tf.TotalVolume,
                           Vat = pk != null ? pk.Vat : "",
                           Pce = pk != null ? pk.Pce : "",
                           Number = pk != null ? pk.Number : "",
                           TCode = pk != null ? pk.TCode : "",
                           UKCurrency = pk != null ? pk.Currency : "",
                           UKValue = pk != null ? pk.Value : "",
                           Weblink = pk != null ? pk.Weblink : "",
                           ProductDescription = ps != null ? ps.ProductDescription : "",
                           DepartureAirpotCode = tf.DepartureAirportCode

                       }).ToList();

            if (_db.Count > 0 && _db != null)
            {
                string myNumber = _db[0].AccountNo.Substring(0, 3);

                try
                {
                    string folderpath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");

                    StreamWriter sw;
                    var randomnumber = CommonConversion.GetNewRandonNumber();

                    if (File.Exists(folderpath + "/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv"))
                    {
                        File.Delete(folderpath + "/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv");
                        sw = new StreamWriter(folderpath + "/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv", false);
                        result.FileName = "Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FilePath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/HTMLFile/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FileStatus = true;
                    }
                    else
                    {
                        sw = new StreamWriter(folderpath + "/Custom_Manifest_EXS" + "_" + _db[0].MAWBNumber == null ? "" : _db[0].MAWBNumber + "_" + _db[0].DepartureAirpotCode == null ? "" : _db[0].DepartureAirpotCode + "_" + _db[0].FlightNumber == null ? "" : _db[0].FlightNumber + "_" + _db[0].FlightDate == null ? "" : _db[0].FlightDate + "_" + "v2-0" + ".csv", false);
                        result.FileName = "Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FilePath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/HTMLFile/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FileStatus = true;
                    }

                    int hubid = _db[0].HubId;
                    var my_uk = dbContext.Hubs.Where(x => x.HubId == hubid).Select(p => p.CustomManifestColumns).FirstOrDefault();
                    string[] columnnames_uk = my_uk.Split(',');
                    for (var k = 0; k < columnnames_uk.Length; k++)
                    {
                        sw.Write(columnnames_uk[k]);

                        if (k < columnnames_uk.Length)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);

                    for (var l = 0; l < _db.Count; l++)
                    {

                        sw.Write(_db[l].ShipperName);
                        if (l < _db.Count)
                            sw.Write(",");

                        if (_db[l].ShipperAddress1 != null)
                        {
                            if (_db[l].ShipperAddress1.Contains(","))
                            {
                                sw.Write(_db[l].ShipperAddress1.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ShipperAddress1);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        if (_db[l].ShipperAddress2 != null)
                        {
                            if (_db[l].ShipperAddress2.Contains(","))
                            {
                                sw.Write(_db[l].ShipperAddress2.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ShipperAddress2);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        sw.Write(_db[l].ShipperCity);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ShipperState);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ShipperZip);
                        if (l < _db.Count)
                            sw.Write(",");


                        sw.Write(_db[l].ShipperCountryCode);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Vat);
                        if (l < _db.Count)
                            sw.Write(",");


                        sw.Write(_db[l].ConsigneeName);
                        if (l < _db.Count)
                            sw.Write(",");


                        if (_db[l].ConsigneeAddress1 != null)
                        {
                            if (_db[l].ConsigneeAddress1.Contains(","))
                            {
                                sw.Write(_db[l].ConsigneeAddress1.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ConsigneeAddress1);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        if (_db[l].ConsigneeAddress2 != null)
                        {
                            if (_db[l].ConsigneeAddress2.Contains(","))
                            {
                                sw.Write(_db[l].ConsigneeAddress2.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ConsigneeAddress2);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        sw.Write(_db[l].ConsigneeCity);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Consigneestate);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Consigneepostcode);
                        if (l < _db.Count)
                            sw.Write(",");


                        sw.Write(_db[l].Consigneecountry);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Pce);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Weight);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Number);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].TCode);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ProductDescription);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Currency);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].UKValue);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Weblink);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(sw.NewLine);
                    }

                    sw.Close();
                }
                catch (Exception ex)
                {
                    result.FileStatus = false;
                    return result;
                }
            }
            return result;
        }

        public FrayteManifestName GetCustomManifestSIN(int ManifestId)
        {
            FrayteManifestName result = new FrayteManifestName();

            var _db = (from a in dbContext.ExpressManifests
                       join b in dbContext.ExpressBags on a.ExpressManifestId equals b.ManifestId
                       join c in dbContext.Expresses on b.BagId equals c.BagId
                       join ed in dbContext.ExpressDetails on c.ExpressId equals ed.ExpressId

                       join pc in dbContext.ProductCatalogs on ed.ProductCatalogId equals pc.ProductCatalogId into leftjoin
                       from ps in leftjoin.DefaultIfEmpty()

                       join zrh in dbContext.ProductCatalogDetailSingapores on ps.ProductCatalogId equals zrh.ProductCatalogId into leftnjoin
                       from pk in leftnjoin.DefaultIfEmpty()

                       join hc in dbContext.HubCarrierServices on c.HubCarrierServiceId equals hc.HubCarrierServiceId
                       join u in dbContext.UserAdditionals on a.CustomerId equals u.UserId
                       join hb in dbContext.HubCarriers on hc.HubCarrierId equals hb.HubCarrierId
                       join hh in dbContext.Hubs on hb.HubId equals hh.HubId
                       join d in dbContext.ExpressAddresses on c.FromAddressId equals d.ExpressAddressId
                       join e in dbContext.ExpressDetails on c.ExpressId equals e.ExpressId
                       join f in dbContext.Countries on d.CountryId equals f.CountryId
                       join g in dbContext.ExpressAddresses on c.ToAddressId equals g.ExpressAddressId
                       join h in dbContext.Countries on g.CountryId equals h.CountryId
                       join td in dbContext.TradelaneShipments on a.TradelaneShipmentId equals td.TradelaneShipmentId
                       join tsa in dbContext.TradelaneShipmentAllocations on td.TradelaneShipmentId equals tsa.TradelaneShipmentId
                       join tsd in dbContext.TradelaneShipmentDetails on tsa.TradelaneShipmentId equals tsd.TradelaneShipmentId
                       join tf in dbContext.TradelaneFlightDetails on td.TradelaneShipmentId equals tf.TradelaneShipmentId
                       where a.ExpressManifestId == ManifestId
                       select new CustomManifest
                       {
                           ShipperName = d.ContactFirstName + " " + d.ContactLastName,
                           ShipperAddress1 = d.Address1,
                           ShipperAddress2 = d.Address2,
                           ShipperAddress3 = d.Area,
                           ShipperCity = d.City,
                           ShipperZip = d.Zip,
                           ShipperState = d.State,
                           ShipperPhoneNo = d.PhoneNo,
                           ShipperEmail = d.Email,
                           ShipperCountryCode = f.CountryCode,
                           CountryName = f.CountryName,
                           //Consignee Information                         
                           ConsigneeName = g.ContactFirstName + " " + g.ContactLastName,
                           ConsigneeAddress1 = g.Address1,
                           ConsigneeAddress2 = g.Address2,
                           ConsigneeAddress3 = g.Area,
                           ConsigneeCity = g.City,
                           Consigneestate = g.State,
                           ConsigneePhoneNo = g.PhoneNo,
                           ConsigneeEmail = g.Email,
                           Consigneepostcode = g.Zip,
                           Consigneecountry = f.CountryCode2,
                           ExporterCountryName = h.CountryName,

                           PiecesContent = e.PiecesContent,
                           Weight = e.Weight,
                           ShipmentReference = c.ShipmentReference,
                           DeclaredCurrencyCode = c.DeclaredCurrencyCode,
                           Value = e.Value,
                           Carrier = hb.Carrier,
                           HubId = hh.HubId,
                           AccountNo = u.AccountNo,
                           TrackingNumber = c.TrackingNumber,
                           MAWBNumber = td.AirlineId + " " + td.MAWB,
                           FlightNumber = tf.FlightNumber,
                           FlightDate = tf.ArrivalDate,
                           HAWBNumber = tsd.HAWB,
                           Pieces = tf.Pieces,
                           NetWeight = tf.TotalWeight,
                           VolumeWeight = tf.TotalVolume,
                           Vat = pk != null ? pk.Vat : "",
                           Pce = pk != null ? pk.Pce : "",
                           Number = pk != null ? pk.Number : "",
                           TCode = pk != null ? pk.TCode : "",
                           UKCurrency = pk != null ? pk.Currency : "",
                           UKValue = pk != null ? pk.Value : "",
                           Weblink = pk != null ? pk.Weblink : "",
                           ProductDescription = ps != null ? ps.ProductDescription : "",
                           DepartureAirpotCode = tf.DepartureAirportCode

                       }).ToList();

            if (_db.Count > 0 && _db != null)
            {
                string myNumber = _db[0].AccountNo.Substring(0, 3);

                try
                {
                    string folderpath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");

                    StreamWriter sw;
                    var randomnumber = CommonConversion.GetNewRandonNumber();


                    if (File.Exists(folderpath + "/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv"))
                    {
                        File.Delete(folderpath + "/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv");
                        sw = new StreamWriter(folderpath + "/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv", false);
                        result.FileName = "Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FilePath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/HTMLFile/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FileStatus = true;
                    }
                    else
                    {
                        sw = new StreamWriter(folderpath + "/Custom_Manifest_EXS" + "_" + _db[0].MAWBNumber == null ? "" : _db[0].MAWBNumber + "_" + _db[0].DepartureAirpotCode == null ? "" : _db[0].DepartureAirpotCode + "_" + _db[0].FlightNumber == null ? "" : _db[0].FlightNumber + "_" + _db[0].FlightDate == null ? "" : _db[0].FlightDate + "_" + "v2-0" + ".csv", false);
                        result.FileName = "Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FilePath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/HTMLFile/Custom_Manifest_EXS" + "_" + (_db[0].MAWBNumber != null ? _db[0].MAWBNumber : "") + "_" + (_db[0].DepartureAirpotCode != null ? _db[0].DepartureAirpotCode : "") + "_" + (_db[0].FlightNumber != null ? _db[0].FlightNumber : "") + "_" + (_db[0].FlightDate != null ? _db[0].FlightDate.ToString() : "") + "_" + "v2-0" + ".csv";
                        result.FileStatus = true;
                    }

                    int hubid = _db[0].HubId;
                    var my_uk = dbContext.Hubs.Where(x => x.HubId == hubid).Select(p => p.CustomManifestColumns).FirstOrDefault();
                    string[] columnnames_uk = my_uk.Split(',');
                    for (var k = 0; k < columnnames_uk.Length; k++)
                    {
                        sw.Write(columnnames_uk[k]);

                        if (k < columnnames_uk.Length)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);

                    for (var l = 0; l < _db.Count; l++)
                    {

                        sw.Write(_db[l].ShipperName);
                        if (l < _db.Count)
                            sw.Write(",");

                        if (_db[l].ShipperAddress1 != null)
                        {
                            if (_db[l].ShipperAddress1.Contains(","))
                            {
                                sw.Write(_db[l].ShipperAddress1.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ShipperAddress1);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        if (_db[l].ShipperAddress2 != null)
                        {
                            if (_db[l].ShipperAddress2.Contains(","))
                            {
                                sw.Write(_db[l].ShipperAddress2.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ShipperAddress2);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        sw.Write(_db[l].ShipperCity);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ShipperState);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ShipperZip);
                        if (l < _db.Count)
                            sw.Write(",");


                        sw.Write(_db[l].ShipperCountryCode);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Vat);
                        if (l < _db.Count)
                            sw.Write(",");


                        sw.Write(_db[l].ConsigneeName);
                        if (l < _db.Count)
                            sw.Write(",");


                        if (_db[l].ConsigneeAddress1 != null)
                        {
                            if (_db[l].ConsigneeAddress1.Contains(","))
                            {
                                sw.Write(_db[l].ConsigneeAddress1.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ConsigneeAddress1);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        if (_db[l].ConsigneeAddress2 != null)
                        {
                            if (_db[l].ConsigneeAddress2.Contains(","))
                            {
                                sw.Write(_db[l].ConsigneeAddress2.Replace(',', ' '));
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                            else
                            {
                                sw.Write(_db[l].ConsigneeAddress2);
                                if (l < _db.Count)
                                    sw.Write(",");
                            }
                        }
                        else
                        {
                            sw.Write("");
                            if (l < _db.Count)
                                sw.Write(",");
                        }

                        sw.Write(_db[l].ConsigneeCity);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Consigneestate);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Consigneepostcode);
                        if (l < _db.Count)
                            sw.Write(",");


                        sw.Write(_db[l].Consigneecountry);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Pce);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Weight);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Number);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].TCode);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].ProductDescription);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Currency);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].UKValue);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(_db[l].Weblink);
                        if (l < _db.Count)
                            sw.Write(",");

                        sw.Write(sw.NewLine);
                    }

                    sw.Close();
                }
                catch (Exception ex)
                {
                    result.FileStatus = false;
                    return result;
                }
            }
            return result;
        }

        #endregion

        #region SaveProductCatalog

        public bool SaveCanadaProductCatalog(CanadaProductcatalog obj)
        {
            try
            {
                if (obj.ProductCatalogId > 0)
                {
                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            var pc = dbContext.ProductCatalogs.Find(obj.ProductCatalogId);
                            if (pc != null)
                            {
                                pc.CustomerId = obj.CustomerId;
                                pc.HubId = obj.HubId;
                                pc.ProductDescription = obj.ProductDescription;
                                pc.Length = obj.Length;
                                pc.Height = obj.Height;
                                pc.Width = obj.Width;
                                pc.Weight = obj.Weight;
                                pc.Declaredvalue = obj.DeclaredValue;
                                dbContext.Entry(pc).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }

                            var pcd = dbContext.ProductCatalogDetailCANs.Where(x => x.ProductCatalogId == pc.ProductCatalogId).FirstOrDefault();
                            if (pcd != null)
                            {
                                pcd.ProductCatalogId = pc.ProductCatalogId;
                                pcd.HubId = obj.HubId;
                                pcd.InternalAccountnumber = obj.InternalAccountnumber;
                                pcd.ItemHSCode = obj.ItemHSCode;
                                pcd.Pieces = obj.Pieces;
                                pcd.ProvinceCode = obj.ProvinceCode;
                                pcd.Reference = obj.Reference;
                                pcd.SKU = obj.SKU;
                                pcd.WeightUOM = obj.WeightUOM.UnitValue;
                                pcd.TotalValue = obj.TotalValue;
                                pcd.Currency = obj.Currency.CurrencyCode;
                                dbContext.Entry(pcd).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }
                }
                else
                {
                    // save data to Master Product catalog table
                    ProductCatalog pc = new ProductCatalog();
                    pc.CustomerId = obj.CustomerId;
                    pc.HubId = obj.HubId;
                    pc.ProductDescription = obj.ProductDescription;
                    pc.Length = obj.Length;
                    pc.Height = obj.Height;
                    pc.Width = obj.Width;
                    pc.Weight = obj.Weight;
                    pc.Declaredvalue = obj.DeclaredValue;
                    dbContext.ProductCatalogs.Add(pc);
                    dbContext.SaveChanges();

                    // Save data to Child Table 
                    ProductCatalogDetailCAN my = new ProductCatalogDetailCAN();
                    my.ProductCatalogId = pc.ProductCatalogId;
                    my.HubId = obj.HubId;
                    my.InternalAccountnumber = obj.InternalAccountnumber;
                    my.ItemHSCode = obj.ItemHSCode;
                    my.Pieces = obj.Pieces;
                    my.ProvinceCode = obj.ProvinceCode;
                    my.Reference = obj.Reference;
                    my.SKU = obj.SKU;
                    my.WeightUOM = obj.WeightUOM.UnitValue;
                    my.TotalValue = obj.TotalValue;
                    my.Currency = obj.Currency.CurrencyCode;
                    dbContext.ProductCatalogDetailCANs.Add(my);
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

        public bool SaveSWISSProductCatalog(SwissProductcatalog obj)
        {
            try
            {
                if (obj.ProductCatalogId > 0)
                {
                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            var pc = dbContext.ProductCatalogs.Find(obj.ProductCatalogId);
                            if (pc != null)
                            {
                                pc.CustomerId = obj.CustomerId;
                                pc.HubId = obj.HubId;
                                pc.ProductDescription = obj.ProductDescription;
                                pc.Length = obj.Length;
                                pc.Height = obj.Height;
                                pc.Width = obj.Width;
                                pc.Weight = obj.Weight;
                                pc.Declaredvalue = obj.DeclaredValue;
                                dbContext.Entry(pc).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }

                            var pcd = dbContext.ProductCatalogDetailSWISSes.Where(x => x.ProductCatalogId == pc.ProductCatalogId).FirstOrDefault();
                            if (pcd != null)
                            {
                                pcd.ProductCatalogId = pc.ProductCatalogId;
                                pcd.HubId = obj.HubId;
                                pcd.ReferenceInvoiceNR = obj.ReferenceInvoiceNR;
                                pcd.Currency = obj.Currency.CurrencyCode;
                                pcd.VersendLand = obj.VersendLand;
                                pcd.Anzahl = obj.Anzahl;
                                pcd.Netto = obj.Netto;
                                pcd.Brutto = obj.Brutto;
                                pcd.TarifCode = obj.TarifCode;
                                pcd.Value = obj.Value;
                                pcd.OriginCountry = obj.OriginCountry;
                                pcd.PacType = obj.PacType;
                                pcd.PacQty = obj.PacQty;
                                pcd.ZeichenPackstucke = obj.ZeichenPackstucke;
                                pcd.ArtVorpapier = obj.ArtVorpapier;
                                pcd.ZeichenVorpapier = obj.ZeichenVorpapier;
                                dbContext.Entry(pcd).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }
                }
                else
                {
                    // save data to Master Product catalog table
                    ProductCatalog pc = new ProductCatalog();
                    pc.CustomerId = obj.CustomerId;
                    pc.HubId = obj.HubId;
                    pc.ProductDescription = obj.ProductDescription;
                    pc.Length = obj.Length;
                    pc.Height = obj.Height;
                    pc.Width = obj.Width;
                    pc.Weight = obj.Weight;
                    pc.Declaredvalue = obj.DeclaredValue;
                    dbContext.ProductCatalogs.Add(pc);
                    dbContext.SaveChanges();

                    ProductCatalogDetailSWISS my = new ProductCatalogDetailSWISS();
                    my.ProductCatalogId = pc.ProductCatalogId;
                    my.HubId = obj.HubId;
                    my.ReferenceInvoiceNR = obj.ReferenceInvoiceNR;
                    my.Currency = obj.Currency.CurrencyCode;
                    my.VersendLand = obj.VersendLand;
                    my.Anzahl = obj.Anzahl;
                    my.Netto = obj.Netto;
                    my.Brutto = obj.Brutto;
                    my.TarifCode = obj.TarifCode;
                    my.Value = obj.Value;
                    my.OriginCountry = obj.OriginCountry;
                    my.PacType = obj.PacType;
                    my.PacQty = obj.PacQty;
                    my.ZeichenPackstucke = obj.ZeichenPackstucke;
                    my.ArtVorpapier = obj.ArtVorpapier;
                    my.ZeichenVorpapier = obj.ZeichenVorpapier;
                    dbContext.ProductCatalogDetailSWISSes.Add(my);
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

        public bool SaveUKproductCatalog(UKProductcatalog obj)
        {
            try
            {
                if (obj.ProductCatalogId > 0)
                {
                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            var pc = dbContext.ProductCatalogs.Find(obj.ProductCatalogId);
                            if (pc != null)
                            {
                                pc.CustomerId = obj.CustomerId;
                                pc.HubId = obj.HubId;
                                pc.ProductDescription = obj.ProductDescription;
                                pc.Length = obj.Length;
                                pc.Height = obj.Height;
                                pc.Width = obj.Width;
                                pc.Weight = obj.Weight;
                                pc.Declaredvalue = obj.DeclaredValue;
                                dbContext.Entry(pc).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }

                            var pcd = dbContext.ProductCatalogDetailUKs.Where(x => x.ProductCatalogId == pc.ProductCatalogId).FirstOrDefault();
                            if (pcd != null)
                            {
                                pcd.ProductCatalogId = pc.ProductCatalogId;
                                pcd.HubId = obj.HubId;
                                pcd.Vat = obj.Vat;
                                pcd.Pce = obj.Pce;
                                pcd.Number = obj.Number;
                                pcd.TCode = obj.TCode;
                                pcd.Currency = obj.Currency.CurrencyCode;
                                pcd.Value = obj.Value;
                                pcd.Weblink = obj.Weblink;
                                dbContext.Entry(pcd).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }
                }
                else
                {
                    // save data to Master Product catalog table
                    ProductCatalog pc = new ProductCatalog();
                    pc.CustomerId = obj.CustomerId;
                    pc.HubId = obj.HubId;
                    pc.ProductDescription = obj.ProductDescription;
                    pc.Length = obj.Length;
                    pc.Height = obj.Height;
                    pc.Width = obj.Width;
                    pc.Weight = obj.Weight;
                    pc.Declaredvalue = obj.DeclaredValue;
                    dbContext.ProductCatalogs.Add(pc);
                    dbContext.SaveChanges();

                    ProductCatalogDetailUK my = new ProductCatalogDetailUK();
                    my.ProductCatalogId = pc.ProductCatalogId;
                    my.HubId = obj.HubId;
                    my.ProductCatalogId = pc.ProductCatalogId;
                    my.HubId = obj.HubId;
                    my.Vat = obj.Vat;
                    my.Pce = obj.Pce;
                    my.Number = obj.Number;
                    my.TCode = obj.TCode;
                    my.Currency = obj.Currency.CurrencyCode;
                    my.Value = obj.Value;
                    my.Weblink = obj.Weblink;
                    dbContext.ProductCatalogDetailUKs.Add(my);
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

        public bool SaveUSAproductCatalog(USAProductcatalog obj)
        {
            try
            {
                if (obj.ProductCatalogId > 0)
                {
                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            var pc = dbContext.ProductCatalogs.Find(obj.ProductCatalogId);

                            if (pc != null)
                            {
                                pc.CustomerId = obj.CustomerId;
                                pc.HubId = obj.HubId;
                                pc.ProductDescription = obj.ProductDescription;
                                pc.Length = obj.Length;
                                pc.Height = obj.Height;
                                pc.Width = obj.Width;
                                pc.Weight = obj.Weight;
                                pc.Declaredvalue = obj.DeclaredValue;
                                dbContext.Entry(pc).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }

                            var pcd = dbContext.ProductCatalogDetailUSAs.Where(x => x.ProductCatalogId == pc.ProductCatalogId).FirstOrDefault();
                            if (pcd != null)
                            {
                                pcd.ProductCatalogId = pc.ProductCatalogId;
                                pcd.HubId = obj.HubId;
                                pcd.Reference = obj.Reference;
                                pcd.InternalAccountnumber = obj.InternalAccountnumber;
                                pcd.WeightUOM = obj.WeightUOM.UnitValue;
                                pcd.ItemValue = obj.ItemValue;
                                pcd.ItemHSCode = obj.ItemHSCode;
                                pcd.CustomEntryType = obj.CustomEntryType;
                                pcd.CustomTotalValue = obj.CustomTotalValue;
                                pcd.CustomTotalVAT = obj.CustomTotalVAT;
                                pcd.CustomDuty = obj.CustomDuty;
                                pcd.Pieces = obj.Pieces;
                                pcd.CustomCommodityMap = obj.CustomCommodityMap;
                                pcd.ECommerceShipmentId = obj.ECommerceShipmentId;
                                dbContext.Entry(pcd).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }
                }
                else
                {
                    // save data to Master Product catalog table
                    ProductCatalog pc = new ProductCatalog();
                    pc.CustomerId = obj.CustomerId;
                    pc.HubId = obj.HubId;
                    pc.ProductDescription = obj.ProductDescription;
                    pc.Length = obj.Length;
                    pc.Height = obj.Height;
                    pc.Width = obj.Width;
                    pc.Weight = obj.Weight;
                    pc.Declaredvalue = obj.DeclaredValue;
                    dbContext.ProductCatalogs.Add(pc);
                    dbContext.SaveChanges();

                    ProductCatalogDetailUSA my = new ProductCatalogDetailUSA();
                    my.ProductCatalogId = pc.ProductCatalogId;
                    my.HubId = obj.HubId;
                    my.Reference = obj.Reference;
                    my.InternalAccountnumber = obj.InternalAccountnumber;
                    my.WeightUOM = obj.WeightUOM.UnitValue;
                    my.ItemValue = obj.ItemValue;
                    my.ItemHSCode = obj.ItemHSCode;
                    my.CustomEntryType = obj.CustomEntryType;
                    my.CustomTotalValue = obj.CustomTotalValue;
                    my.CustomTotalVAT = obj.CustomTotalVAT;
                    my.CustomDuty = obj.CustomDuty;
                    my.Pieces = obj.Pieces;
                    my.CustomCommodityMap = obj.CustomCommodityMap;
                    my.ECommerceShipmentId = obj.ECommerceShipmentId;
                    dbContext.ProductCatalogDetailUSAs.Add(my);
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

        public bool SaveNorwayproductCatalog(NorwayProductcatalog obj)
        {
            try
            {
                if (obj.ProductCatalogId > 0)
                {
                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            var pc = dbContext.ProductCatalogs.Find(obj.ProductCatalogId);
                            if (pc != null)
                            {
                                pc.CustomerId = obj.CustomerId;
                                pc.HubId = obj.HubId;
                                pc.ProductDescription = obj.ProductDescription;
                                pc.Length = obj.Length;
                                pc.Height = obj.Height;
                                pc.Width = obj.Width;
                                pc.Weight = obj.Weight;
                                pc.Declaredvalue = obj.DeclaredValue;
                                dbContext.Entry(pc).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }

                            var pcd = dbContext.ProductCatalogDetailNorways.Where(x => x.ProductCatalogId == pc.ProductCatalogId).FirstOrDefault();
                            if (pcd != null)
                            {
                                pcd.ProductCatalogId = pc.ProductCatalogId;
                                pcd.HubId = obj.HubId;
                                pcd.Vat = obj.Vat;
                                pcd.Pce = obj.Pce;
                                pcd.Number = obj.Number;
                                pcd.TCode = obj.TCode;
                                pcd.Currency = obj.Currency.CurrencyCode;
                                pcd.Value = obj.Value;
                                pcd.Weblink = obj.Weblink;
                                dbContext.Entry(pcd).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }
                }
                else
                {
                    // save data to Master Product catalog table
                    ProductCatalog pc = new ProductCatalog();
                    pc.CustomerId = obj.CustomerId;
                    pc.HubId = obj.HubId;
                    pc.ProductDescription = obj.ProductDescription;
                    pc.Length = obj.Length;
                    pc.Height = obj.Height;
                    pc.Width = obj.Width;
                    pc.Weight = obj.Weight;
                    pc.Declaredvalue = obj.DeclaredValue;
                    dbContext.ProductCatalogs.Add(pc);
                    dbContext.SaveChanges();

                    ProductCatalogDetailNorway my = new ProductCatalogDetailNorway();
                    my.ProductCatalogId = pc.ProductCatalogId;
                    my.HubId = obj.HubId;
                    my.Vat = obj.Vat;
                    my.Pce = obj.Pce;
                    my.Number = obj.Number;
                    my.TCode = obj.TCode;
                    my.Currency = obj.Currency.CurrencyCode;
                    my.Value = obj.Value;
                    my.Weblink = obj.Weblink;
                    dbContext.ProductCatalogDetailNorways.Add(my);
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

        public bool SaveSINproductCatalog(SINProductcatalog obj)
        {
            try
            {
                if (obj.ProductCatalogId > 0)
                {
                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            var pc = dbContext.ProductCatalogs.Find(obj.ProductCatalogId);
                            if (pc != null)
                            {
                                pc.CustomerId = obj.CustomerId;
                                pc.HubId = obj.HubId;
                                pc.ProductDescription = obj.ProductDescription;
                                pc.Length = obj.Length;
                                pc.Height = obj.Height;
                                pc.Width = obj.Width;
                                pc.Weight = obj.Weight;
                                pc.Declaredvalue = obj.DeclaredValue;
                                dbContext.Entry(pc).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }

                            var pcd = dbContext.ProductCatalogDetailSingapores.Where(x => x.ProductCatalogId == pc.ProductCatalogId).FirstOrDefault();
                            if (pcd != null)
                            {
                                pcd.ProductCatalogId = pc.ProductCatalogId;
                                pcd.HubId = obj.HubId;
                                pcd.Vat = obj.Vat;
                                pcd.Pce = obj.Pce;
                                pcd.Number = obj.Number;
                                pcd.TCode = obj.TCode;
                                pcd.Currency = obj.Currency.CurrencyCode;
                                pcd.Value = obj.Value;
                                pcd.Weblink = obj.Weblink;
                                dbContext.Entry(pcd).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }
                }
                else
                {
                    // save data to Master Product catalog table
                    ProductCatalog pc = new ProductCatalog();
                    pc.CustomerId = obj.CustomerId;
                    pc.HubId = obj.HubId;
                    pc.ProductDescription = obj.ProductDescription;
                    pc.Length = obj.Length;
                    pc.Height = obj.Height;
                    pc.Width = obj.Width;
                    pc.Weight = obj.Weight;
                    pc.Declaredvalue = obj.DeclaredValue;
                    dbContext.ProductCatalogs.Add(pc);
                    dbContext.SaveChanges();

                    ProductCatalogDetailSingapore my = new ProductCatalogDetailSingapore();
                    my.ProductCatalogId = pc.ProductCatalogId;
                    my.HubId = obj.HubId;
                    my.Vat = obj.Vat;
                    my.Pce = obj.Pce;
                    my.Number = obj.Number;
                    my.TCode = obj.TCode;
                    my.Currency = obj.Currency.CurrencyCode;
                    my.Value = obj.Value;
                    my.Weblink = obj.Weblink;
                    dbContext.ProductCatalogDetailSingapores.Add(my);
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

        public bool SaveJapanProductCatalog(JapanProductcatalog obj)
        {
            try
            {
                if (obj.ProductCatalogId > 0)
                {
                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            var pc = dbContext.ProductCatalogs.Find(obj.ProductCatalogId);
                            if (pc != null)
                            {

                                pc.CustomerId = obj.CustomerId;
                                pc.HubId = obj.HubId;
                                pc.ProductDescription = obj.ProductDescription;
                                pc.Length = obj.Length;
                                pc.Height = obj.Height;
                                pc.Width = obj.Width;
                                pc.Weight = obj.Weight;
                                pc.Declaredvalue = obj.DeclaredValue;
                                dbContext.Entry(pc).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }

                            var pcd = dbContext.ProductCatalogDetailJapans.Where(x => x.ProductCatalogId == pc.ProductCatalogId).FirstOrDefault();
                            if (pcd != null)
                            {
                                pcd.ProductCatalogId = pc.ProductCatalogId;
                                pcd.HubId = obj.HubId;
                                pcd.No = obj.Number;
                                pcd.DeclaredValueOfCustom = obj.DeclaredValueOfCustom;
                                pcd.CodeKg = obj.Code;
                                pcd.Origin = obj.Origin;
                                pcd.OriginCode = obj.OriginCode;
                                pcd.Currency = obj.Currency.CurrencyCode;
                                dbContext.Entry(pcd).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }
                }
                else
                {
                    // save data to Master Product catalog table
                    ProductCatalog pc = new ProductCatalog();
                    pc.CustomerId = obj.CustomerId;
                    pc.HubId = obj.HubId;
                    pc.ProductDescription = obj.ProductDescription;
                    pc.Length = obj.Length;
                    pc.Height = obj.Height;
                    pc.Width = obj.Width;
                    pc.Weight = obj.Weight;
                    pc.Declaredvalue = obj.DeclaredValue;
                    dbContext.ProductCatalogs.Add(pc);
                    dbContext.SaveChanges();

                    ProductCatalogDetailJapan my = new ProductCatalogDetailJapan();
                    my.ProductCatalogId = pc.ProductCatalogId;
                    my.HubId = obj.HubId;
                    my.No = obj.Number;
                    my.DeclaredValueOfCustom = obj.DeclaredValueOfCustom;
                    my.CodeKg = obj.Code;
                    my.Origin = obj.Origin;
                    my.OriginCode = obj.OriginCode;
                    my.Currency = obj.Currency.CurrencyCode;
                    dbContext.ProductCatalogDetailJapans.Add(my);
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

        #endregion

        #region GetProductcatalog

        public List<ProductcatalogDetail> GetSwissProductCatalog(FrayteProductCatalogTrack track)
        {
            int SkipRows = 0;
            SkipRows = (track.CurrentPage - 1) * track.TakeRows;

            var db = (from pc in dbContext.ProductCatalogs
                      join pcs in dbContext.ProductCatalogDetailSWISSes on pc.ProductCatalogId equals pcs.ProductCatalogId
                      join us in dbContext.Users on pc.CustomerId equals us.UserId
                      join hh in dbContext.Hubs on pc.HubId equals hh.HubId
                      where pc.CustomerId == track.CustomerId &&
                            pc.HubId == track.HubId &&
                            (pc.ProductDescription.Contains(track.ProductDescription))
                      select new ProductcatalogDetail
                      {
                          ProductcatalogDetailId = pcs.ProductCatalogDetailSWISSId,
                          ProductcatalogId = pc.ProductCatalogId,
                          CustomerName = us.CompanyName,
                          HubName = hh.Code,
                          ProductDescription = pc.ProductDescription,
                          Value = pcs.Value,
                          Length = pc.Length.HasValue ? pc.Length.Value : 0.0M,
                          Height = pc.Height.HasValue ? pc.Height.Value : 0.0M,
                          Width = pc.Width.HasValue ? pc.Width.Value : 0.0M,
                          Weight = pc.Weight.HasValue ? pc.Weight.Value : 0.0M,
                          DeclaredValue = pc.Declaredvalue.HasValue ? pc.Declaredvalue.Value : 0.0M,
                          ItemHSCode = pcs.TarifCode,
                          CurrencyCode = pcs.Currency,
                          WeightUOM = ""
                      }).ToList();

            int total = db.Count();
            db = db.OrderBy(p => p.ProductcatalogDetailId).Skip(SkipRows).Take(track.TakeRows).ToList();
            db.ForEach(p => p.TotalRows = total);

            return db;
        }

        public List<ProductcatalogDetail> GetUkProductCatalog(FrayteProductCatalogTrack track)
        {
            int SkipRows = 0;
            SkipRows = (track.CurrentPage - 1) * track.TakeRows;

            var db = (from pc in dbContext.ProductCatalogs
                      join pcs in dbContext.ProductCatalogDetailUKs on pc.ProductCatalogId equals pcs.ProductCatalogId
                      join us in dbContext.Users on pc.CustomerId equals us.UserId
                      join hh in dbContext.Hubs on pc.HubId equals hh.HubId
                      where pc.CustomerId == track.CustomerId &&
                            pc.HubId == track.HubId &&
                         (pc.ProductDescription.Contains(track.ProductDescription))
                      select new ProductcatalogDetail
                      {
                          ProductcatalogDetailId = pcs.ProductCatalogDetailUKId,
                          ProductcatalogId = pc.ProductCatalogId,
                          CustomerName = us.CompanyName,
                          HubName = hh.Code,
                          ProductDescription = pc.ProductDescription,
                          Length = pc.Length.HasValue ? pc.Length.Value : 0.0M,
                          Height = pc.Height.HasValue ? pc.Height.Value : 0.0M,
                          Width = pc.Width.HasValue ? pc.Width.Value : 0.0M,
                          Weight = pc.Weight.HasValue ? pc.Weight.Value : 0.0M,
                          DeclaredValue = pc.Declaredvalue.HasValue ? pc.Declaredvalue.Value : 0.0M,
                          CurrencyCode = pcs.Currency,
                          Value = pcs.Value,
                          ItemHSCode = pcs.TCode

                      }).ToList();

            int total = db.Count();
            db = db.OrderBy(p => p.ProductcatalogDetailId).Skip(SkipRows).Take(track.TakeRows).ToList();
            db.ForEach(p => p.TotalRows = total);

            return db;
        }

        public List<ProductcatalogDetail> GetCanadaProductCatalog(FrayteProductCatalogTrack track)
        {
            int SkipRows = 0;
            SkipRows = (track.CurrentPage - 1) * track.TakeRows;

            var db = (from pc in dbContext.ProductCatalogs
                      join pcs in dbContext.ProductCatalogDetailCANs on pc.ProductCatalogId equals pcs.ProductCatalogId
                      join us in dbContext.Users on pc.CustomerId equals us.UserId
                      join hh in dbContext.Hubs on pc.HubId equals hh.HubId
                      where pc.CustomerId == track.CustomerId &&
                            pc.HubId == track.HubId &&
                            (pc.ProductDescription.Contains(track.ProductDescription))
                      select new ProductcatalogDetail
                      {
                          ProductcatalogDetailId = pcs.ProductCatalogDetailCANId,
                          ProductcatalogId = pc.ProductCatalogId,
                          CustomerName = us.CompanyName,
                          HubName = hh.Code,
                          ProductDescription = pc.ProductDescription,
                          Value = pcs.TotalValue,
                          ItemHSCode = pcs.ItemHSCode,
                          Length = pc.Length.HasValue ? pc.Length.Value : 0.0M,
                          Height = pc.Height.HasValue ? pc.Height.Value : 0.0M,
                          Width = pc.Width.HasValue ? pc.Width.Value : 0.0M,
                          Weight = pc.Weight.HasValue ? pc.Weight.Value : 0.0M,
                          DeclaredValue = pc.Declaredvalue.HasValue ? pc.Declaredvalue.Value : 0.0M,
                          CurrencyCode = pcs.Currency,
                          WeightUOM = pcs.WeightUOM
                      }).ToList();

            int total = db.Count();
            db = db.OrderBy(p => p.ProductcatalogDetailId).Skip(SkipRows).Take(track.TakeRows).ToList();
            db.ForEach(p => p.TotalRows = total);

            return db;
        }

        public List<ProductcatalogDetail> GetUSAProductCatalog(FrayteProductCatalogTrack track)
        {
            int SkipRows = 0;
            SkipRows = (track.CurrentPage - 1) * track.TakeRows;

            var db = (from pc in dbContext.ProductCatalogs
                      join pcs in dbContext.ProductCatalogDetailUSAs on pc.ProductCatalogId equals pcs.ProductCatalogId
                      join us in dbContext.Users on pc.CustomerId equals us.UserId
                      join hh in dbContext.Hubs on pc.HubId equals hh.HubId
                      where pc.CustomerId == track.CustomerId &&
                            pc.HubId == track.HubId &&
                         (pc.ProductDescription.Contains(track.ProductDescription))
                      select new ProductcatalogDetail
                      {
                          ProductcatalogDetailId = pcs.ProductCatalogDetailUSAId,
                          ProductcatalogId = pc.ProductCatalogId,
                          CustomerName = us.CompanyName,
                          HubName = hh.Code,
                          ProductDescription = pc.ProductDescription,
                          Value = pcs.ItemValue,
                          ItemHSCode = pcs.ItemHSCode,
                          Length = pc.Length.HasValue ? pc.Length.Value : 0.0M,
                          Height = pc.Height.HasValue ? pc.Height.Value : 0.0M,
                          Width = pc.Width.HasValue ? pc.Width.Value : 0.0M,
                          Weight = pc.Weight.HasValue ? pc.Weight.Value : 0.0M,
                          DeclaredValue = pc.Declaredvalue.HasValue ? pc.Declaredvalue.Value : 0.0M,
                          CurrencyCode = pcs.Currency,
                          WeightUOM = pcs.WeightUOM
                      }).ToList();

            int total = db.Count();
            db = db.OrderBy(p => p.ProductcatalogDetailId).Skip(SkipRows).Take(track.TakeRows).ToList();
            db.ForEach(p => p.TotalRows = total);

            return db;
        }

        public List<ProductcatalogDetail> GetNorwayProductCatalog(FrayteProductCatalogTrack track)
        {
            int SkipRows = 0;
            SkipRows = (track.CurrentPage - 1) * track.TakeRows;

            var db = (from pc in dbContext.ProductCatalogs
                      join pcs in dbContext.ProductCatalogDetailNorways on pc.ProductCatalogId equals pcs.ProductCatalogId
                      join us in dbContext.Users on pc.CustomerId equals us.UserId
                      join hh in dbContext.Hubs on pc.HubId equals hh.HubId
                      where pc.CustomerId == track.CustomerId &&
                            pc.HubId == track.HubId &&
                            pc.ProductDescription.Contains(track.ProductDescription)
                      select new ProductcatalogDetail
                      {
                          ProductcatalogDetailId = pcs.ProductCatalogDetailNorwayId,
                          ProductcatalogId = pc.ProductCatalogId,
                          CustomerName = us.CompanyName,
                          HubName = hh.Code,
                          ProductDescription = pc.ProductDescription,
                          Length = pc.Length.HasValue ? pc.Length.Value : 0.0M,
                          Height = pc.Height.HasValue ? pc.Height.Value : 0.0M,
                          Width = pc.Width.HasValue ? pc.Width.Value : 0.0M,
                          Weight = pc.Weight.HasValue ? pc.Weight.Value : 0.0M,
                          DeclaredValue = pc.Declaredvalue.HasValue ? pc.Declaredvalue.Value : 0.0M,
                          CurrencyCode = pcs.Currency,
                          Value = pcs.Value,
                          ItemHSCode = pcs.TCode
                      }).ToList();

            int total = db.Count();
            db = db.OrderBy(p => p.ProductcatalogDetailId).Skip(SkipRows).Take(track.TakeRows).ToList();
            db.ForEach(p => p.TotalRows = total);

            return db;
        }

        public List<ProductcatalogDetail> GetSINProductCatalog(FrayteProductCatalogTrack track)
        {
            int SkipRows = 0;
            SkipRows = (track.CurrentPage - 1) * track.TakeRows;

            var db = (from pc in dbContext.ProductCatalogs
                      join pcs in dbContext.ProductCatalogDetailSingapores on pc.ProductCatalogId equals pcs.ProductCatalogId
                      join us in dbContext.Users on pc.CustomerId equals us.UserId
                      join hh in dbContext.Hubs on pc.HubId equals hh.HubId
                      where pc.CustomerId == track.CustomerId &&
                            pc.HubId == track.HubId &&
                           (pc.ProductDescription.Contains(track.ProductDescription))
                      select new ProductcatalogDetail
                      {
                          ProductcatalogDetailId = pcs.ProductCatalogDetailSINId,
                          ProductcatalogId = pc.ProductCatalogId,
                          CustomerName = us.CompanyName,
                          HubName = hh.Code,
                          ProductDescription = pc.ProductDescription,
                          Length = pc.Length.HasValue ? pc.Length.Value : 0.0M,
                          Height = pc.Height.HasValue ? pc.Height.Value : 0.0M,
                          Width = pc.Width.HasValue ? pc.Width.Value : 0.0M,
                          Weight = pc.Weight.HasValue ? pc.Weight.Value : 0.0M,
                          DeclaredValue = pc.Declaredvalue.HasValue ? pc.Declaredvalue.Value : 0.0M,
                          CurrencyCode = pcs.Currency,
                          Value = pcs.Value,
                          ItemHSCode = pcs.TCode
                      }).ToList();

            int total = db.Count();
            db = db.OrderBy(p => p.ProductcatalogDetailId).Skip(SkipRows).Take(track.TakeRows).ToList();
            db.ForEach(p => p.TotalRows = total);

            return db;
        }

        public List<ProductcatalogDetail> GetNRTProductCatalog(FrayteProductCatalogTrack track)
        {
            int SkipRows = 0;
            SkipRows = (track.CurrentPage - 1) * track.TakeRows;

            var db = (from pc in dbContext.ProductCatalogs
                      join pcs in dbContext.ProductCatalogDetailJapans on pc.ProductCatalogId equals pcs.ProductCatalogId
                      join us in dbContext.Users on pc.CustomerId equals us.UserId
                      join hh in dbContext.Hubs on pc.HubId equals hh.HubId
                      where pc.CustomerId == track.CustomerId &&
                            pc.HubId == track.HubId &&
                            (pc.ProductDescription.Contains(track.ProductDescription))
                      select new ProductcatalogDetail
                      {
                          ProductcatalogDetailId = pcs.ProductCatalogDetailJAPANId,
                          ProductcatalogId = pc.ProductCatalogId,
                          CustomerName = us.CompanyName,
                          HubName = hh.Code,
                          ProductDescription = pc.ProductDescription,
                          Length = pc.Length.HasValue ? pc.Length.Value : 0.0M,
                          Height = pc.Height.HasValue ? pc.Height.Value : 0.0M,
                          Width = pc.Width.HasValue ? pc.Width.Value : 0.0M,
                          Weight = pc.Weight.HasValue ? pc.Weight.Value : 0.0M,
                          DeclaredValue = pc.Declaredvalue.HasValue ? pc.Declaredvalue.Value : 0.0M,
                          CurrencyCode = pcs.Currency
                      }).ToList();

            int total = db.Count();
            db = db.OrderBy(p => p.ProductcatalogDetailId).Skip(SkipRows).Take(track.TakeRows).ToList();
            db.ForEach(p => p.TotalRows = total);

            return db;
        }

        #endregion

        #region EditProductcatalog

        public Tuple<UKProductcatalog, USAProductcatalog, JapanProductcatalog, SwissProductcatalog, NorwayProductcatalog, SINProductcatalog, CanadaProductcatalog, Tuple<string>> EditProductcatalog(int ProductcatalogId)
        {
            UKProductcatalog lhr = new UKProductcatalog();
            USAProductcatalog jfk = new USAProductcatalog();
            JapanProductcatalog nrt = new JapanProductcatalog();
            SwissProductcatalog zrh = new SwissProductcatalog();
            NorwayProductcatalog osl = new NorwayProductcatalog();
            SINProductcatalog sin = new SINProductcatalog();
            CanadaProductcatalog yvr = new CanadaProductcatalog();

            var db = (from pc in dbContext.ProductCatalogs
                      join hh in dbContext.Hubs on pc.HubId equals hh.HubId
                      where pc.ProductCatalogId == ProductcatalogId
                      select new
                      {
                          HubName = hh.Code,
                          HubId = hh.HubId
                      }).FirstOrDefault();

            if (db != null)
            {
                #region LHR

                if (db.HubName == "LHR")
                {
                    lhr = (from pk in dbContext.ProductCatalogs
                           join pcu in dbContext.ProductCatalogDetailUKs
                           on pk.ProductCatalogId equals pcu.ProductCatalogId
                           where pk.ProductCatalogId == ProductcatalogId
                           select new UKProductcatalog
                           {
                               ProductCatalogId = pk.ProductCatalogId,
                               ProductDescription = pk.ProductDescription,
                               Length = pk.Length.HasValue ? pk.Length.Value : 0.0M,
                               Width = pk.Width.HasValue ? pk.Width.Value : 0.0M,
                               Height = pk.Height.HasValue ? pk.Height.Value : 0.0M,
                               Weight = pk.Weight.HasValue ? pk.Weight.Value : 0.0M,
                               DeclaredValue = pk.Declaredvalue.HasValue ? pk.Declaredvalue.Value : 0.0M,
                               CustomerId = pk.CustomerId,
                               HubId = pk.HubId,
                               Vat = pcu.Vat,
                               Pce = pcu.Pce,
                               Number = pcu.Number,
                               TCode = pcu.TCode,
                               Currency = new FrayteCurrency()
                               {
                                   CurrencyCode = pcu.Currency,
                                   CurrencyDescription = ""
                               },
                               Value = pcu.Value,
                               Weblink = pcu.Weblink
                           }).FirstOrDefault();
                }

                #endregion

                #region OSL

                if (db.HubName == "OSL")
                {
                    osl = (from pk in dbContext.ProductCatalogs
                           join pcu in dbContext.ProductCatalogDetailNorways
                           on pk.ProductCatalogId equals pcu.ProductCatalogId
                           where pk.ProductCatalogId == ProductcatalogId
                           select new NorwayProductcatalog
                           {
                               ProductCatalogId = pk.ProductCatalogId,
                               ProductDescription = pk.ProductDescription,
                               Length = pk.Length.HasValue ? pk.Length.Value : 0.0M,
                               Width = pk.Width.HasValue ? pk.Width.Value : 0.0M,
                               Height = pk.Height.HasValue ? pk.Height.Value : 0.0M,
                               Weight = pk.Weight.HasValue ? pk.Weight.Value : 0.0M,
                               DeclaredValue = pk.Declaredvalue.HasValue ? pk.Declaredvalue.Value : 0.0M,
                               CustomerId = pk.CustomerId,
                               HubId = pk.HubId,
                               Vat = pcu.Vat,
                               Pce = pcu.Pce,
                               Number = pcu.Number,
                               TCode = pcu.TCode,
                               Currency = new FrayteCurrency()
                               {
                                   CurrencyCode = pcu.Currency,
                                   CurrencyDescription = ""
                               },
                               Value = pcu.Value,
                               Weblink = pcu.Weblink
                           }).FirstOrDefault();
                }

                #endregion

                #region YVR && YYZ

                if (db.HubName == "YVR" || db.HubName == "YYZ")
                {
                    yvr = (from pk in dbContext.ProductCatalogs
                           join pcu in dbContext.ProductCatalogDetailCANs
                           on pk.ProductCatalogId equals pcu.ProductCatalogId
                           where pk.ProductCatalogId == ProductcatalogId
                           select new CanadaProductcatalog
                           {
                               ProductCatalogId = pk.ProductCatalogId,
                               ProductDescription = pk.ProductDescription,
                               Length = pk.Length.HasValue ? pk.Length.Value : 0.0M,
                               Width = pk.Width.HasValue ? pk.Width.Value : 0.0M,
                               Height = pk.Height.HasValue ? pk.Height.Value : 0.0M,
                               Weight = pk.Weight.HasValue ? pk.Weight.Value : 0.0M,
                               DeclaredValue = pk.Declaredvalue.HasValue ? pk.Declaredvalue.Value : 0.0M,
                               CustomerId = pk.CustomerId,
                               HubId = pk.HubId,
                               Reference = pcu.Reference,
                               InternalAccountnumber = pcu.InternalAccountnumber,
                               WeightUOM = new FrayteWeightUOM()
                               {
                                   UnitDisplay = "",
                                   UnitValue = pcu.WeightUOM
                               },
                               TotalValue = pcu.TotalValue,
                               Currency = new FrayteCurrency()
                               {
                                   CurrencyCode = pcu.Currency,
                                   CurrencyDescription = ""
                               },
                               ItemHSCode = pcu.ItemHSCode,
                               Pieces = pcu.Pieces,
                               SKU = pcu.SKU,
                               ProvinceCode = pcu.ProvinceCode
                           }).FirstOrDefault();
                }

                #endregion

                #region JFK && ORD && SFO

                if (db.HubName == "JFK" || db.HubName == "ORD" || db.HubName == "SFO")
                {
                    jfk = (from pk in dbContext.ProductCatalogs
                           join pcu in dbContext.ProductCatalogDetailUSAs
                           on pk.ProductCatalogId equals pcu.ProductCatalogId
                           where pk.ProductCatalogId == ProductcatalogId
                           select new USAProductcatalog
                           {
                               ProductCatalogId = pk.ProductCatalogId,
                               ProductDescription = pk.ProductDescription,
                               Length = pk.Length.HasValue ? pk.Length.Value : 0.0M,
                               Width = pk.Width.HasValue ? pk.Width.Value : 0.0M,
                               Height = pk.Height.HasValue ? pk.Height.Value : 0.0M,
                               Weight = pk.Weight.HasValue ? pk.Weight.Value : 0.0M,
                               DeclaredValue = pk.Declaredvalue.HasValue ? pk.Declaredvalue.Value : 0.0M,
                               CustomerId = pk.CustomerId,
                               HubId = pk.HubId,
                               Reference = pcu.Reference,
                               InternalAccountnumber = pcu.InternalAccountnumber,
                               WeightUOM = new FrayteWeightUOM()
                               {
                                   UnitDisplay = "",
                                   UnitValue = pcu.WeightUOM
                               },
                               Currency = new FrayteCurrency()
                               {
                                   CurrencyCode = pcu.Currency,
                                   CurrencyDescription = ""
                               },
                               ItemHSCode = pcu.ItemHSCode,
                               Pieces = pcu.Pieces,
                               CustomEntryType = pcu.CustomEntryType,
                               CustomTotalValue = pcu.CustomTotalValue,
                               CustomTotalVAT = pcu.CustomTotalVAT,
                               CustomDuty = pcu.CustomDuty,
                               ItemValue = pcu.ItemValue,
                               CustomCommodityMap = pcu.CustomCommodityMap,
                               ECommerceShipmentId = pcu.ECommerceShipmentId
                           }).FirstOrDefault();
                }

                #endregion

                #region SIN

                if (db.HubName == "SIN")
                {
                    sin = (from pk in dbContext.ProductCatalogs
                           join pcu in dbContext.ProductCatalogDetailSingapores
                           on pk.ProductCatalogId equals pcu.ProductCatalogId
                           where pk.ProductCatalogId == ProductcatalogId
                           select new SINProductcatalog
                           {
                               ProductCatalogId = pk.ProductCatalogId,
                               ProductDescription = pk.ProductDescription,
                               Length = pk.Length.HasValue ? pk.Length.Value : 0.0M,
                               Width = pk.Width.HasValue ? pk.Width.Value : 0.0M,
                               Height = pk.Height.HasValue ? pk.Height.Value : 0.0M,
                               Weight = pk.Weight.HasValue ? pk.Weight.Value : 0.0M,
                               DeclaredValue = pk.Declaredvalue.HasValue ? pk.Declaredvalue.Value : 0.0M,
                               CustomerId = pk.CustomerId,
                               HubId = pk.HubId,
                               Vat = pcu.Vat,
                               Pce = pcu.Pce,
                               Number = pcu.Number,
                               TCode = pcu.TCode,
                               Currency = new FrayteCurrency()
                               {
                                   CurrencyCode = pcu.Currency,
                                   CurrencyDescription = ""
                               },
                               Value = pcu.Value,
                               Weblink = pcu.Weblink
                           }).FirstOrDefault();
                }

                #endregion

                #region ZRH

                if (db.HubName == "ZRH")
                {
                    zrh = (from pk in dbContext.ProductCatalogs
                           join pcu in dbContext.ProductCatalogDetailSWISSes
                           on pk.ProductCatalogId equals pcu.ProductCatalogId
                           where pk.ProductCatalogId == ProductcatalogId
                           select new SwissProductcatalog
                           {
                               ProductCatalogId = pk.ProductCatalogId,
                               ProductDescription = pk.ProductDescription,
                               Length = pk.Length.HasValue ? pk.Length.Value : 0.0M,
                               Width = pk.Width.HasValue ? pk.Width.Value : 0.0M,
                               Height = pk.Height.HasValue ? pk.Height.Value : 0.0M,
                               Weight = pk.Weight.HasValue ? pk.Weight.Value : 0.0M,
                               DeclaredValue = pk.Declaredvalue.HasValue ? pk.Declaredvalue.Value : 0.0M,
                               CustomerId = pk.CustomerId,
                               HubId = pk.HubId,
                               ReferenceInvoiceNR = pcu.ReferenceInvoiceNR,
                               VersendLand = pcu.VersendLand,
                               Anzahl = pcu.Anzahl,
                               Currency = new FrayteCurrency()
                               {
                                   CurrencyCode = pcu.Currency,
                                   CurrencyDescription = ""
                               },
                               Netto = pcu.Netto,
                               Brutto = pcu.Brutto,
                               TarifCode = pcu.TarifCode,
                               Value = pcu.Value,
                               OriginCountry = pcu.OriginCountry,
                               PacType = pcu.PacType,
                               PacQty = pcu.PacQty,
                               ZeichenPackstucke = pcu.ZeichenPackstucke,
                               ArtVorpapier = pcu.ArtVorpapier,
                               ZeichenVorpapier = pcu.ZeichenVorpapier
                           }).FirstOrDefault();
                }

                #endregion

                #region NRT

                if (db.HubName == "NRT")
                {
                    nrt = (from pk in dbContext.ProductCatalogs
                           join pcu in dbContext.ProductCatalogDetailJapans
                           on pk.ProductCatalogId equals pcu.ProductCatalogId
                           where pk.ProductCatalogId == ProductcatalogId
                           select new JapanProductcatalog
                           {
                               ProductCatalogId = pk.ProductCatalogId,
                               ProductDescription = pk.ProductDescription,
                               Length = pk.Length.HasValue ? pk.Length.Value : 0.0M,
                               Width = pk.Width.HasValue ? pk.Width.Value : 0.0M,
                               Height = pk.Height.HasValue ? pk.Height.Value : 0.0M,
                               Weight = pk.Weight.HasValue ? pk.Weight.Value : 0.0M,
                               DeclaredValue = pk.Declaredvalue.HasValue ? pk.Declaredvalue.Value : 0.0M,
                               CustomerId = pk.CustomerId,
                               HubId = pk.HubId,
                               Number = pcu.No,
                               Code = pcu.CodeKg,
                               DeclaredValueOfCustom = pcu.DeclaredValueOfCustom,
                               Currency = new FrayteCurrency()
                               {
                                   CurrencyCode = pcu.Currency,
                                   CurrencyDescription = ""
                               },
                               Origin = pcu.Origin,
                               OriginCode = pcu.OriginCode
                           }).FirstOrDefault();
                }

                #endregion
            }

            return Tuple.Create(lhr, jfk, nrt, zrh, osl, sin, yvr, db != null ? (db.HubName + ";" + db.HubId.ToString()) : null);
        }

        #endregion

        #endregion
    }
}