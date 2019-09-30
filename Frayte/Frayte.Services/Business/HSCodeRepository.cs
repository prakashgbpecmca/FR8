using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models;
using Frayte.Services.DataAccess;
using System.Text.RegularExpressions;
using Frayte.Services.Utility;
namespace Frayte.Services.Business
{
    public class HSCodeRepository
    {
        FrayteEntities dbContext = new FrayteEntities();
        #region Get Non HsCodes Shipments
        public List<NonHsCodeShipments> GetNonHSCodeShipments(TrackHSCodeShipment obj)
        {
            List<NonHsCodeShipments> list = new List<NonHsCodeShipments>();
            int SkipRows = 0;
            SkipRows = (obj.CurrentPage - 1) * obj.TakeRows;
            var data = dbContext.spGet_TrackNonHSCodeShipments(FrayteShipmentServiceType.eCommerce, obj.FromDate, obj.ToDate, obj.ShipmentStatusId, obj.FrayteNumber, obj.TrackingNo, SkipRows, obj.TakeRows, obj.OperationZoneId).ToList();
            if (data != null && data.Count > 0)
            {
                NonHsCodeShipments ship;
                foreach (var re in data)
                {
                    ship = new NonHsCodeShipments();
                    ship.ShipmentId = re.DirectShipmentId;
                    if (!string.IsNullOrEmpty(re.FromCompany))
                    {
                        ship.ShippedFromCompany = re.FromCompany;
                    }
                    else
                    {

                    }
                    if (!string.IsNullOrEmpty(re.ToCompany))
                    {
                        ship.ShippedToCompany = re.ToCompany;
                    }
                    else
                    {

                    }
                    ship.ShippingDate = re.CreatedOn;
                    ship.Status = re.StatusName;
                    ship.TrackingNo = re.TrackingNo;
                    ship.DisplayStatus = re.DisplayStatusName;
                    ship.CourierCompany = re.LogisticCompany;
                    ship.CourierCompanyDisplay = re.LogisticCompanyDisplay;
                    ship.Customer = re.ContactName;
                    ship.FrayteNumber = re.FrayteNumber;
                    ship.ManifestId = re.ManifestId.HasValue ? re.ManifestId.Value : 0;
                    ship.Reference1 = re.Reference1;

                    list.Add(ship);
                }

            }

            return list;
        }

        public HSCodeMapped ISAllHSCodeMapped(int eCommerceShipmentDetailid)
        {
            HSCodeMapped result = new HSCodeMapped();
             
            var data = dbContext.eCommerceShipmentDetails.Find(eCommerceShipmentDetailid);
            if (data != null)
            {
                var shipDetail = dbContext.eCommerceShipmentDetails
                                              .Where(p => p.eCommerceShipmentId == data.eCommerceShipmentId &&
                                              string.IsNullOrEmpty(p.HSCode)).ToList();
                var ship = dbContext.eCommerceShipments.Find(data.eCommerceShipmentId);
                if(ship.EstimatedDateofArrival.HasValue && ship.EstimatedDateofDelivery.HasValue && ship.EstimatedTimeofArrival.HasValue && ship.EstimatedTimeofDelivery.HasValue)
                {
                    if (shipDetail == null || (shipDetail != null && shipDetail.Count == 0))
                    {
                        result.Status = true;
                        result.Id = data.eCommerceShipmentId;
                        return result;
                    }
                }
               
                result.Status = false;
                result.Id = data.eCommerceShipmentId;
                return result;
            }
            result.Status = false;
            result.Id = data.eCommerceShipmentId;
            return result;
        }
        #endregion

        #region Set HSCode
        public FrayteResult SetShipmentHSCode(int eCommerceShipmentDetailid, string HSCode)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                if (eCommerceShipmentDetailid > 0)
                {
                    var data = dbContext.eCommerceShipmentDetails.Find(eCommerceShipmentDetailid);
                    if (data != null)
                    {
                        data.HSCode = HSCode;
                        dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        result.Status = true;
                    }
                    else
                    {
                        result.Status = false;
                    }

                    try
                    {
                        var shipDetail = dbContext.eCommerceShipmentDetails
                                             .Where(p => p.eCommerceShipmentId == data.eCommerceShipmentId &&
                                             !string.IsNullOrEmpty(p.HSCode)).ToList();
                        if (shipDetail == null || (shipDetail != null || shipDetail.Count == 0))
                        {
                            var ship = dbContext.eCommerceShipments.Find(data.eCommerceShipmentId);
                            if (ship != null)
                            {
                                ship.MappedOn = DateTime.UtcNow;
                                dbContext.Entry(ship).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
            }
            return result;
        }
        #endregion

        #region GEt HSCode
        public List<HSCodeDescription> GetHSCodes(string serachTerm, int countryId)
        {
            List<HSCodeDescription> data = new List<HSCodeDescription>();
            if (CommonConversion.ConvertToInt(serachTerm) > 0)
            {
                data = dbContext.HSCodes.Where(p => p.HSCode1.Contains(serachTerm) && p.CountryId == countryId)
                        .Select(p => new HSCodeDescription { HSCode = p.HSCode1, Description = p.Description, HsCodeId = p.HSCodeId })
                .ToList();
            }
            else if (serachTerm != "^[0-9]+$")
            {
                data = dbContext.HSCodes.Where(p => p.Description.Contains(serachTerm) && p.CountryId == countryId)
                      .Select(p => new HSCodeDescription { HSCode = p.HSCode1, Description = p.Description, HsCodeId = p.HSCodeId })
              .ToList();
            }


            return data;
        }
        #endregion

    }
}
