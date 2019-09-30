using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Frayte.Services.Business
{
    public class WarehouseRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public int GetWareHouseId(int shipmentId)
        {
            int wareHouseId=dbContext.Shipments.Find(shipmentId).WarehouseId.Value;
            return wareHouseId;
        }
        public FrayteCountryCode GetWareHouseCountry(int? countryId)
        {
            FrayteCountryCode WareHouseCountry;
            if (countryId.HasValue && countryId > 0)
            {
                WareHouseCountry = new FrayteCountryCode();
                var country = dbContext.Countries.Find(countryId.Value);
                WareHouseCountry.Code = country.CountryCode;
                WareHouseCountry.Code2 = country.CountryCode2;
                WareHouseCountry.CountryId = country.CountryId;
                WareHouseCountry.Name = country.CountryName;
                return WareHouseCountry;
            }
            else
            {
               return null;
            }
        }
        public List<ShipmentWarehouse> GetWarehouseList()
        {
            var lstWarehouse = (from c in dbContext.Warehouses
                                select new ShipmentWarehouse()
                               {
                                   WarehouseId = c.WarehouseId,
                                   WarehouseName = c.LocationName,
                                   CountryId= c.CountryId
                               }).ToList();

            return lstWarehouse;
        }

        public FrayteWarehouse GetWarehouseDetail(int? warehouseId)
        {
            FrayteWarehouse warehouseDetail = new FrayteWarehouse();
            WorkingWeekDay workingDays = new WorkingWeekDay();
            var warehouse = dbContext.Warehouses.Where(p => p.WarehouseId == warehouseId).FirstOrDefault();

            if (warehouse != null)
            {
                warehouseDetail.WarehouseId = warehouse.WarehouseId;
                warehouseDetail.Address = warehouse.Address;
                warehouseDetail.Address2 = warehouse.Address2;
                warehouseDetail.Address3 = warehouse.Address3;
                warehouseDetail.City = warehouse.City;
                warehouseDetail.State = warehouse.State;
                warehouseDetail.Zip = warehouse.Zip;
                warehouseDetail.WorkingWeekDay = new WorkingWeekDay();
                if(warehouse.WorkingWeekDayId!=null)
                {
                    warehouseDetail.WorkingWeekDay.WorkingWeekDayId = warehouse.WorkingWeekDayId.Value;
                }

                if (warehouseDetail.WorkingWeekDay.WorkingWeekDayId > 0)
                {
                    workingDays = dbContext.WorkingWeekDays.Find(warehouseDetail.WorkingWeekDay.WorkingWeekDayId);
                }


                if (workingDays != null)
                {

                    warehouseDetail.WorkingWeekDay = workingDays;

                }
                warehouseDetail.Email = warehouse.Email;
                warehouseDetail.TelephoneNo = warehouse.TelephoneNo;
                warehouseDetail.MobileNo = warehouse.MobileNo;
                warehouseDetail.Fax = warehouse.Fax;
                warehouseDetail.WorkingStartTime = UtilityRepository.GetTimeZoneTime(warehouse.WorkingStartTime, warehouse.TimeZoneId);
                warehouseDetail.WorkingEndTime = UtilityRepository.GetTimeZoneTime(warehouse.WorkingEndTime, warehouse.TimeZoneId);
                warehouseDetail.Address3 = warehouse.Address3;
                warehouseDetail.Address3 = warehouse.Address3;
                warehouseDetail.Address3 = warehouse.Address3;

                warehouseDetail.LocationMapImage = warehouse.LocationMapImage;
                warehouseDetail.LocationName = warehouse.LocationName;
                warehouseDetail.MapDetail = new GrayteGoogleMap();
                warehouseDetail.MapDetail.latitude = warehouse.LocationLatitude.HasValue ?  warehouse.LocationLatitude.Value : 0;
                warehouseDetail.MapDetail.longitude = warehouse.LocationLongitude.HasValue ? warehouse.LocationLongitude.Value : 0;
                warehouseDetail.Zoom = warehouse.LocationZoom.HasValue ? warehouse.LocationZoom.Value : 0;
                warehouseDetail.MarkerDetail = new GrayteGoogleMap();
                warehouseDetail.MarkerDetail.latitude = warehouse.MarkerLatitude.HasValue ? warehouse.MarkerLatitude.Value : 0;
                warehouseDetail.MarkerDetail.longitude = warehouse.MarkerLongitude.HasValue ? warehouse.MarkerLongitude.Value : 0;

                //Get Country Details
                var country = dbContext.Countries.Where(p => p.CountryId == warehouse.CountryId).FirstOrDefault();
                if (country != null)
                {
                    warehouseDetail.Country = new FrayteCountryCode();
                    warehouseDetail.Country.CountryId = country.CountryId;
                    warehouseDetail.Country.Code = country.CountryCode;
                    warehouseDetail.Country.Code2 = country.CountryCode2;
                    warehouseDetail.Country.Name = country.CountryName;
                }
                else
                {
                    warehouseDetail.Country = new FrayteCountryCode();
                }

                //Get TimeZone details
                var timezone = dbContext.Timezones.Where(p => p.TimezoneId == warehouse.TimeZoneId).FirstOrDefault();
                if (timezone != null)
                {
                    warehouseDetail.Timezone = new TimeZoneModal();
                    warehouseDetail.Timezone.TimezoneId = timezone.TimezoneId;
                    warehouseDetail.Timezone.Name = timezone.Name;
                    warehouseDetail.Timezone.Offset = timezone.Offset;
                    warehouseDetail.Timezone.OffsetShort = timezone.OffsetShort;
                }
                else
                {
                    warehouseDetail.Timezone = new TimeZoneModal();
                }

                //Get warehouse manager detail
                var warehouseManager = dbContext.Users.Where(p => p.UserId == warehouse.ManagerId).FirstOrDefault();
                if (warehouseManager != null)
                {
                    warehouseDetail.Manager = new FrayteCustomerAssociatedUser();
                    warehouseDetail.Manager.UserId = warehouseManager.UserId;
                    warehouseDetail.Manager.AssociateType = FrayteAssociateType.Manager;
                    warehouseDetail.Manager.ContactName = warehouseManager.ContactName;
                    warehouseDetail.Manager.Email = warehouseManager.Email;
                    warehouseDetail.Manager.TelephoneNo = warehouseManager.TelephoneNo;
                    warehouseDetail.Manager.WorkingHours = UtilityRepository.GetWorkingHours(warehouseManager.WorkingStartTime, warehouseManager.WorkingEndTime);
                }
                else
                {
                    warehouseDetail.Manager = new FrayteCustomerAssociatedUser();
                }
                
            }

            return warehouseDetail;
        }

        public List<TransportToWarehouse> GetTransportToWarehouse()
        {
            var lstTransportToWarehouse = dbContext.TransportToWarehouses.ToList();

            //var lstTransportToWarehouse = (from c in dbContext.TransportToWarehouses
            //                               select new Frayte.Services.DataAccess.TransportToWarehouse()
            //                   {
            //                       TransportToWarehouseId = c.TransportToWarehouseId,
            //                       Name= c.Name
            //                   }).ToList();

            return lstTransportToWarehouse;
        }

        public List<FrayteWarehouse> GetAllWarehouseList()
        {
            List<FrayteWarehouse> lstFrayteWarehouse = new List<FrayteWarehouse>();

            var lstWarehouse = (from w in dbContext.Warehouses
                                join c in dbContext.Countries on w.CountryId equals c.CountryId
                                join tz in dbContext.Timezones on w.TimeZoneId equals tz.TimezoneId
                                join u in dbContext.Users on w.ManagerId equals u.UserId into tempManager
                                from us in tempManager.DefaultIfEmpty()
                                select new
                                {
                                    w.WarehouseId,                                    
                                    w.Address,
                                    w.Address2,
                                    w.Address3,
                                    w.City,
                                    w.State,
                                    w.Zip,
                                    w.CountryId,
                                    c.CountryCode,
                                    c.CountryName,
                                    w.LocationName,
                                    w.LocationLatitude,
                                    w.LocationMapImage,
                                    w.ManagerId,
                                    ManagerName = us.ContactName,
                                    ManagerEmail = us.Email,
                                    ManagerTelephone = us.TelephoneNo,
                                    ManagerStartTime = us.WorkingStartTime,
                                    ManagerEndTime = us.WorkingEndTime,
                                    w.Email,
                                    w.TelephoneNo,
                                    w.MobileNo,
                                    w.Fax,
                                    w.WorkingStartTime,
                                    w.WorkingEndTime,
                                    w.TimeZoneId,
                                    TimeZoneName = tz.Name,
                                    TimeZoneOffset = tz.Offset,
                                    TimezoneOffSetShort = tz.OffsetShort
                                }).ToList();

            if (lstWarehouse != null)
            {
                foreach(var w in lstWarehouse)
                {
                    FrayteWarehouse frayteWarehouse = new FrayteWarehouse();
                    frayteWarehouse.WarehouseId = w.WarehouseId;                    
                    frayteWarehouse.Address = w.Address;
                    frayteWarehouse.Address2 = w.Address2;
                    frayteWarehouse.Address3 = w.Address3;
                    frayteWarehouse.City = w.City;
                    frayteWarehouse.State = w.State;
                    frayteWarehouse.Zip = w.Zip;
                    frayteWarehouse.Country = new FrayteCountryCode();
                    frayteWarehouse.Country.CountryId = w.CountryId;
                    frayteWarehouse.Country.Code = w.CountryCode;
                    frayteWarehouse.Country.Name = w.CountryName; 
                    frayteWarehouse.LocationName = w.LocationName;
                    frayteWarehouse.LocationMapImage = w.LocationMapImage;
                    frayteWarehouse.Manager = new FrayteCustomerAssociatedUser();
                    frayteWarehouse.Manager.UserId = w.ManagerId.HasValue ? w.ManagerId.Value : 0;
                    if (frayteWarehouse.Manager.UserId > 0)
                    {
                        frayteWarehouse.Manager.ContactName = w.ManagerName;
                        frayteWarehouse.Manager.Email = w.ManagerEmail;
                        frayteWarehouse.Manager.TelephoneNo = w.ManagerTelephone;
                        frayteWarehouse.Manager.WorkingHours = UtilityRepository.GetWorkingHours(w.ManagerStartTime, w.ManagerEndTime);
                    }
                    frayteWarehouse.Email = w.Email;
                    frayteWarehouse.TelephoneNo  = w.TelephoneNo;
                    frayteWarehouse.MobileNo = w.MobileNo;
                    frayteWarehouse.Fax = w.Fax;
                    frayteWarehouse.WorkingStartTime = UtilityRepository.GetTimeZoneTime(w.WorkingStartTime, w.TimeZoneName);
                    frayteWarehouse.WorkingEndTime = UtilityRepository.GetTimeZoneTime(w.WorkingEndTime, w.TimeZoneName);
                    frayteWarehouse.Timezone = new TimeZoneModal();
                    frayteWarehouse.Timezone.TimezoneId = w.TimeZoneId;
                    frayteWarehouse.Timezone.Name = w.TimeZoneName;
                    frayteWarehouse.Timezone.Offset = w.TimeZoneOffset;
                    frayteWarehouse.Timezone.OffsetShort = w.TimezoneOffSetShort;
                    lstFrayteWarehouse.Add(frayteWarehouse);
                }
            }

            return lstFrayteWarehouse;
        }

        public FrayteWarehouse SaveWarehouse(FrayteWarehouse frayteWarehouse)
        {
            Warehouse warehouse;
            if (frayteWarehouse.WarehouseId > 0)
            {
                warehouse = dbContext.Warehouses.Where(p => p.WarehouseId == frayteWarehouse.WarehouseId).FirstOrDefault();
                if (warehouse != null)
                {                       
                    warehouse.Address = frayteWarehouse.Address;
                    warehouse.Address2 = frayteWarehouse.Address2;
                    warehouse.Address3 = frayteWarehouse.Address3;
                    warehouse.City = frayteWarehouse.City;
                    warehouse.State = frayteWarehouse.State;
                    warehouse.Zip = frayteWarehouse.Zip;
                    warehouse.CountryId = frayteWarehouse.Country.CountryId;
                    warehouse.LocationName = frayteWarehouse.LocationName;
                    warehouse.LocationLatitude = frayteWarehouse.MapDetail.latitude;
                    warehouse.LocationLongitude = frayteWarehouse.MapDetail.longitude;
                    warehouse.LocationZoom = frayteWarehouse.Zoom;
                    warehouse.WorkingWeekDayId = frayteWarehouse.WorkingWeekDay.WorkingWeekDayId;
                    //warehouse.LocationMapImage = frayteWarehouse.LocationMapImage;
                    warehouse.MarkerLatitude = frayteWarehouse.MarkerDetail.latitude;
                    warehouse.MarkerLongitude = frayteWarehouse.MarkerDetail.longitude;
                    warehouse.ManagerId = frayteWarehouse.Manager.UserId;
                    warehouse.Email = frayteWarehouse.Email;
                    warehouse.TelephoneNo = frayteWarehouse.TelephoneNo;
                    warehouse.MobileNo = frayteWarehouse.MobileNo;
                    warehouse.Fax = frayteWarehouse.Fax;
                    warehouse.WorkingStartTime = UtilityRepository.GetTimeZoneUTCTime(frayteWarehouse.WorkingStartTime, frayteWarehouse.Timezone.Name).Value;
                    warehouse.WorkingEndTime = UtilityRepository.GetTimeZoneUTCTime(frayteWarehouse.WorkingEndTime, frayteWarehouse.Timezone.Name).Value;
                    warehouse.TimeZoneId = frayteWarehouse.Timezone.TimezoneId;
                    
                }
            }
            else
            {
                warehouse = new Warehouse();                
                warehouse.Address = frayteWarehouse.Address;
                warehouse.Address2 = frayteWarehouse.Address2;
                warehouse.Address3 = frayteWarehouse.Address3;
                warehouse.City = frayteWarehouse.City;
                warehouse.State = frayteWarehouse.State;
                warehouse.Zip = frayteWarehouse.Zip;
                warehouse.CountryId = frayteWarehouse.Country.CountryId;                
                if (frayteWarehouse.Manager != null)
                {
                    warehouse.ManagerId = frayteWarehouse.Manager.UserId;
                }
                warehouse.LocationName = frayteWarehouse.LocationName;
                warehouse.LocationLatitude = frayteWarehouse.MapDetail.latitude;
                warehouse.LocationLongitude = frayteWarehouse.MapDetail.longitude;
                warehouse.LocationZoom = frayteWarehouse.Zoom;
                warehouse.WorkingWeekDayId = frayteWarehouse.WorkingWeekDay.WorkingWeekDayId;
                //warehouse.LocationMapImage = frayteWarehouse.LocationMapImage;
                warehouse.MarkerLatitude = frayteWarehouse.MarkerDetail.latitude;
                warehouse.MarkerLongitude = frayteWarehouse.MarkerDetail.longitude;                
                warehouse.Email = frayteWarehouse.Email;
                warehouse.TelephoneNo = frayteWarehouse.TelephoneNo;
                warehouse.MobileNo = frayteWarehouse.MobileNo;
                warehouse.Fax = frayteWarehouse.Fax;
                warehouse.WorkingStartTime = UtilityRepository.GetTimeZoneUTCTime(frayteWarehouse.WorkingStartTime, frayteWarehouse.Timezone.Name).Value;
                warehouse.WorkingEndTime = UtilityRepository.GetTimeZoneUTCTime(frayteWarehouse.WorkingEndTime, frayteWarehouse.Timezone.Name).Value;
                warehouse.TimeZoneId = frayteWarehouse.Timezone.TimezoneId;

                dbContext.Warehouses.Add(warehouse);
            }

            dbContext.SaveChanges();
            frayteWarehouse.WarehouseId = warehouse.WarehouseId;

            SaveWarehouseImage(warehouse, frayteWarehouse.LocationMapImage);

            return frayteWarehouse;
        }

        private void SaveWarehouseImage(Warehouse frayteWarehouse, string base64Image)
        {
            string warehouseImageFolder = HttpContext.Current.Server.MapPath("~/UploadFiles/Warehouse/");
            if (!Directory.Exists(warehouseImageFolder))
            {
                Directory.CreateDirectory(warehouseImageFolder);
            }

            var bytes = Convert.FromBase64String(base64Image);
            string filename = frayteWarehouse.WarehouseId + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".png";
            using (var imageFile = new FileStream(warehouseImageFolder + @"/" + filename, FileMode.Create))
            {
                imageFile.Write(bytes, 0, bytes.Length);
                imageFile.Flush();
            }

            var warehouse = dbContext.Warehouses.Where(p => p.WarehouseId == frayteWarehouse.WarehouseId).FirstOrDefault();
            if (warehouse != null)
            {
                warehouse.LocationMapImage = filename;
                dbContext.SaveChanges();
            }
        }

        public FrayteResult DeleteWarehouse(int warehouseId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var warehouse = new Warehouse { WarehouseId = warehouseId };
                dbContext.Warehouses.Attach(warehouse);
                dbContext.Warehouses.Remove(warehouse);
                dbContext.SaveChanges();
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
    }
}
