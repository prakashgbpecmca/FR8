using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System.Data.Entity.Validation;

namespace Frayte.Services.Business
{
    public class ShipmentExprysRepositery
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<ConsignmentBag> GetConsignmentBag()
        {
            var lstConsignmentBag = dbContext.ConsignmentBags.ToList();
            return lstConsignmentBag;
        }

        public List<FrayteAWBShipmentId> GetFrayteAWB()
        {
            List<FrayteAWBShipmentId> AwbShipmentId = new List<FrayteAWBShipmentId>();
            int Exprysid = dbContext.Couriers.Where(x => x.ShipmentType == FrayteShipmentType.Expryes).Select(x => x.CourierId).FirstOrDefault();
            var lstFrayteAWB = dbContext.Shipments.Where(x => x.DeliveredBy == Exprysid).ToList();
            foreach (var ship in lstFrayteAWB)
            {
                FrayteAWBShipmentId Awb = new FrayteAWBShipmentId();
                Awb.FrayteAWB = ship.FrayteAWB;
                Awb.ShipmentId = ship.ShipmentId;
                AwbShipmentId.Add(Awb);

            }
            return AwbShipmentId;
        }

        public void SaveConsignmentBag(FrayteConsignment cc)
        {
            try
            {
                ConsignmentBag cb;
                if (cc != null && cc.BagId == 0)
                {
                    cb = new ConsignmentBag();
                    cb.Description = cc.Description;
                    dbContext.ConsignmentBags.Add(cb);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void SaveConsigmentBagDetails(FrayteShipmentExpreysBagDetail ConsigmentBag)
        {
            try
            {
                if (ConsigmentBag != null)
                {
                    //Save data in shipment bag
                    ShipmentBag sb = new ShipmentBag();
                    sb.BagName = ConsigmentBag.BagName;
                    sb.Barcode = ConsigmentBag.Barcode;
                    sb.CreatedOn = DateTime.UtcNow;
                    dbContext.ShipmentBags.Add(sb);
                    dbContext.SaveChanges();
                    ConsigmentBag.ShipmentBagId = sb.ShipmentBagId;

                    //Adding Shipment Bag Details 
                    if (ConsigmentBag != null && ConsigmentBag.BagDetail !=null)
                    {
                        ShipmentBagDetail bagDetail = new ShipmentBagDetail();
                        bagDetail.ShipmentBagId = ConsigmentBag.ShipmentBagId;
                        bagDetail.FrayteAWB = ConsigmentBag.BagDetail.FrayteAWB;
                        var shi = dbContext.Shipments.Where(p => p.FrayteAWB == ConsigmentBag.BagDetail.FrayteAWB).FirstOrDefault();
                        bagDetail.CartonQty = ConsigmentBag.BagDetail.CartonQty;
                        bagDetail.ShipmentId = shi.ShipmentId;
                        dbContext.ShipmentBagDetails.Add(bagDetail);
                        dbContext.SaveChanges();
                        ConsigmentBag.BagDetail.ShipmentBagDetailId = bagDetail.ShipmentBagDetailId;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        //public void AddToBag(FrayteShipmentBag sbm)
        //{
        //    try
        //    {
        //        if (sbm != null)
        //        {
        //            ShipmentBag sb = new ShipmentBag();
        //            sb.BagName = sbm.BagName;
        //            sb.Barcode = sbm.Barcode;
        //            sb.CreatedOn = DateTime.UtcNow;
        //            dbContext.ShipmentBags.Add(sb);
        //            dbContext.SaveChanges();

        //            //Adding Shipment Bag Details 

        //            //if (sbm.FrayteShipmentBagDetail != null && sbm.FrayteShipmentBagDetail.Count > 0)
        //            //{
        //            //    foreach (FrayteShipmentBagDetail shbagDetail in sbm.FrayteShipmentBagDetail)
        //            //    {
        //            //        SaveShipmentBagDetail(sb.ShipmentBagId, shbagDetail);
        //            //    }
        //            //}
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        //public void SaveShipmentBagDetail(int shipmentbagID, FrayteShipmentBagDetail fshdetail)
        //{
        //    try
        //    {
        //        if (fshdetail != null && shipmentbagID > 0 && fshdetail.ShipmentId > 0)
        //        {
        //            ShipmentBagDetail sb = new ShipmentBagDetail();
        //            sb.ShipmentBagId = shipmentbagID;
        //            sb.ShipmentId = fshdetail.ShipmentId;
        //            sb.CartonQty = fshdetail.CartonQty;
        //            sb.FrayteAWB = fshdetail.FrayteAWB;
        //            sb.ShipmentId = dbContext.Shipments.Where(x => x.FrayteAWB == sb.FrayteAWB).Select(x => x.ShipmentId).FirstOrDefault();
        //            dbContext.ShipmentBagDetails.Add(sb);
        //            try
        //            {
        //                dbContext.SaveChanges();
        //            }
        //            catch (DbEntityValidationException ex)
        //            {
        //                foreach (var entityValidationErrors in ex.EntityValidationErrors)
        //                {
        //                    foreach (var validationError in entityValidationErrors.ValidationErrors)
        //                    {
        //                        string ss = "Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        public void SaveShipmentBagBarcode(int ShipmentBagId, string Barcode)
        {
            try
            {
                ShipmentBag shipment;
                if (ShipmentBagId > 0 && !String.IsNullOrEmpty(Barcode))
                {
                    shipment = new ShipmentBag();
                    shipment = dbContext.ShipmentBags.Find(ShipmentBagId);
                    if (shipment != null)
                    {
                        shipment.Barcode = Barcode;
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public List<FrayteUserShipment> GetExprysShipment()
        {
            List<FrayteUserShipment> lstUserShipment = new List<FrayteUserShipment>();

            var result = (from s in dbContext.Shipments
                          join c in dbContext.Users on s.CustomerId equals c.UserId
                          join sa in dbContext.UserAddresses on s.ShipperAddressId equals sa.UserAddressId
                          join sac in dbContext.Countries on sa.CountryId equals sac.CountryId
                          join ra in dbContext.UserAddresses on s.ReceiverAddressId equals ra.UserAddressId
                          join rac in dbContext.Countries on ra.CountryId equals rac.CountryId
                          join cr in dbContext.Couriers on s.DeliveredBy equals cr.CourierId
                          join ss in dbContext.ShipmentStatus on s.ShipmentStatusId equals ss.ShipmentStatusId
                          join dd in dbContext.Couriers on s.DeliveredBy equals dd.CourierId
                          where s.ShipmentStatusId != (int)FrayteShipmentStatus.Close && dd.ShipmentType == FrayteShipmentType.Expryes

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
            return lstUserShipment;
        }
    }
}
