using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Business
{
    public class CourierRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<FrayteCourier> GetCourierList()
        {
            List<FrayteCourier> lstCourier = new List<FrayteCourier>();

            var result = dbContext.Couriers.ToList();

            foreach (Courier courier in result)
            {
                FrayteCourier frayteCourier = new FrayteCourier();
                frayteCourier.CourierId = courier.CourierId;
                frayteCourier.Name = courier.CourierName;
                frayteCourier.DisplayName = courier.DisplayName != null ? courier.DisplayName : FrayteLogisticServiceDisplayType.UKMail;
                frayteCourier.Website = courier.Website;
                frayteCourier.CourierType = courier.ShipmentType;
                frayteCourier.LatestBookingTime = UtilityRepository.GetTimeZoneTime(courier.LatestBookingTime);

                lstCourier.Add(frayteCourier);
            }
            return lstCourier;
        }

        public List<FrayteCourier> GetUKCourier()
        {
            List<FrayteCourier> lstCourier = new List<FrayteCourier>();
            var result = dbContext.Couriers.ToList().Skip(3);
            foreach (Courier courier in result)
            {
                FrayteCourier frayteCourier = new FrayteCourier();
                frayteCourier.CourierId = courier.CourierId;
                frayteCourier.Name = courier.CourierName;
                frayteCourier.DisplayName = courier.DisplayName;
                frayteCourier.Website = courier.Website;
                frayteCourier.CourierType = courier.ShipmentType;
                frayteCourier.LatestBookingTime = UtilityRepository.GetTimeZoneTime(courier.LatestBookingTime);
                lstCourier.Add(frayteCourier);
            }
            return lstCourier;
        }

        public FrayteCourier SaveCourier(FrayteCourier courier)
        {
            Courier newCourier;
            if (courier.CourierId > 0)
            {
                newCourier = dbContext.Couriers.Where(p => p.CourierId == courier.CourierId).FirstOrDefault();

                newCourier.CourierName = courier.Name;
                newCourier.DisplayName = CommonConversion.DisplayName(courier.Name);
                newCourier.Website = courier.Website;
                newCourier.ShipmentType = courier.CourierType;
                newCourier.LatestBookingTime = UtilityRepository.GetTimeFromString(courier.LatestBookingTime).Value;
            }
            else
            {
                newCourier = new Courier();
                newCourier.CourierId = courier.CourierId;
                newCourier.CourierName = courier.Name;
                newCourier.DisplayName = CommonConversion.DisplayName(courier.Name);
                newCourier.Website = courier.Website;
                newCourier.ShipmentType = courier.CourierType;
                newCourier.LatestBookingTime = UtilityRepository.GetTimeFromString(courier.LatestBookingTime).Value;

                dbContext.Couriers.Add(newCourier);
            }

            try
            {
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            courier.CourierId = newCourier.CourierId;

            return courier;
        }

        public FrayteResult DeleteCourier(int courierId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var courier = new Courier { CourierId = courierId };
                dbContext.Couriers.Attach(courier);
                dbContext.Couriers.Remove(courier);
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

        public List<FrayteShipmentCourier> GetShipmentCourierList()
        {
            List<FrayteShipmentCourier> lstCouriers = new List<FrayteShipmentCourier>();

            var result = dbContext.Couriers.ToList();

            foreach (Courier courier in result)
            {
                if (courier.ShipmentType == "Air")
                {
                    FrayteShipmentCourier frayteCourier = new FrayteShipmentCourier();
                    frayteCourier.CourierId = courier.CourierId;
                    frayteCourier.Name = courier.CourierName;
                    frayteCourier.DisplayName = courier.DisplayName;
                    frayteCourier.Website = courier.Website;
                    frayteCourier.CourierType = courier.ShipmentType;
                    frayteCourier.LatestBookingTime = UtilityRepository.GetWorkingTime(courier.LatestBookingTime).Value;

                    lstCouriers.Add(frayteCourier);
                }
            }
            foreach (Courier courier in result)
            {
                if (courier.ShipmentType == "Sea")
                {
                    FrayteShipmentCourier frayteCourier = new FrayteShipmentCourier();
                    frayteCourier.CourierId = courier.CourierId;
                    frayteCourier.Name = courier.CourierName;
                    frayteCourier.DisplayName = courier.DisplayName;
                    frayteCourier.Website = courier.Website;
                    frayteCourier.CourierType = courier.ShipmentType;
                    frayteCourier.LatestBookingTime = UtilityRepository.GetWorkingTime(courier.LatestBookingTime).Value;

                    lstCouriers.Add(frayteCourier);
                }
            }
            foreach (Courier courier in result)
            {
                if (courier.ShipmentType == "Courier")
                {
                    FrayteShipmentCourier frayteCourier = new FrayteShipmentCourier();
                    frayteCourier.CourierId = courier.CourierId;
                    frayteCourier.Name = courier.CourierName;
                    if (courier.CourierName == FrayteCourierCompany.UK_EU)
                    {
                        frayteCourier.DisplayName = FrayteLogisticServiceDisplayType.UKMail;
                    }
                    else
                    {
                        frayteCourier.DisplayName = courier.DisplayName;
                    }
                    //frayteCourier.DisplayName = courier.DisplayName;
                    frayteCourier.Website = courier.Website;
                    frayteCourier.CourierType = courier.ShipmentType;
                    frayteCourier.LatestBookingTime = UtilityRepository.GetWorkingTime(courier.LatestBookingTime).Value;

                    lstCouriers.Add(frayteCourier);
                }
            }
            foreach (Courier courier in result)
            {
                if (courier.ShipmentType == "Expryes")
                {
                    FrayteShipmentCourier frayteCourier = new FrayteShipmentCourier();
                    frayteCourier.CourierId = courier.CourierId;
                    frayteCourier.Name = courier.CourierName;
                    frayteCourier.DisplayName = courier.DisplayName;
                    frayteCourier.Website = courier.Website;
                    frayteCourier.CourierType = courier.ShipmentType;
                    frayteCourier.LatestBookingTime = UtilityRepository.GetWorkingTime(courier.LatestBookingTime).Value;

                    lstCouriers.Add(frayteCourier);
                }
            }
            return lstCouriers;
        }

        public List<FrayteShipmentMethod> GetShipmentMethods(string shipmentType)
        {
            List<FrayteShipmentMethod> lstShipmentMethods = new List<FrayteShipmentMethod>();

            var result = dbContext.Couriers.Where(p => p.ShipmentType == shipmentType).ToList();

            foreach (Courier courier in result)
            {
                FrayteShipmentMethod frayteCourier = new FrayteShipmentMethod();
                frayteCourier.ShipmentMethodId = courier.CourierId;
                frayteCourier.ShipmentMethodName = courier.CourierName;
                if (courier.DisplayName == null || courier.DisplayName == "")
                {
                    frayteCourier.ShipmentDisplayName = FrayteLogisticServiceDisplayType.UKMail;
                }
                else
                {
                    frayteCourier.ShipmentDisplayName = courier.DisplayName;
                }

                lstShipmentMethods.Add(frayteCourier);
            }
            return lstShipmentMethods;
        }

        public FrayteCourier GetCourier(int p)
        {
            FrayteCourier courier = new FrayteCourier();
            var data = dbContext.Couriers.Find(p);
            if (data != null)
            {
                courier.CourierId = data.CourierId;
                courier.Name = data.CourierName;
            }
            return courier;
        }
    }
}
