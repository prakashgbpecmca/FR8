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
    public class TracingRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<TracingComment> GetTracingComment()
        {
            var tarcinglist = dbContext.TracingComments.ToList();
            return tarcinglist;
        }

        public void SaveTracingComment(FrayteTracingComment ftc)
        {
            try
            {
                TracingComment tc;
                if (ftc != null && ftc.TracingCommentId == 0)
                {
                    tc = new TracingComment();
                    tc.Description = ftc.Description;
                    dbContext.TracingComments.Add(tc);
                    dbContext.SaveChanges();
                }
                //else
                //{
                //    tc = dbContext.TracingComments.Find(ftc.TracingCommentId);
                //    tc.Description = ftc.Description;
                //    dbContext.TracingComments.Add(tc);
                //    dbContext.SaveChanges();
                //}
            }
            catch (Exception ex)
            {

            }
        }

        public void SaveTracingDetail(FrayteShipmentDetailSave ship)
        {
            try
            {
                if (ship.FrayteShipmentTracingSave != null && ship.FrayteShipmentTracingSave.Count > 0)
                {
                    ShipmentTracing stt;
                    foreach (FrayteShipmentTracing ss in ship.FrayteShipmentTracingSave)
                    {
                        stt = new ShipmentTracing();
                        stt.Comment = ss.Comment;
                        stt.CommentDate = ss.CommentDate;
                        stt.ShipmentBagId = ss.ShipmentBagId;
                        dbContext.ShipmentTracings.Add(stt);
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {
                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {
                                    string exception = "Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public List<FrayteShipmentTracingDetail> GetTracingDetail(string Barcode)
        {
            List<FrayteShipmentTracingDetail> lstshipmentTracingDetail = new List<FrayteShipmentTracingDetail>();
            var result = dbContext.spGet_ShipmentTracingDetail(Barcode).ToList();
            FrayteShipmentTracingDetail fs;
            foreach (var rr in result)
            {
                fs = new FrayteShipmentTracingDetail();
                fs.FrayteAWB = rr.FrayteAWB;
                fs.From = rr.CountryFrom;
                fs.To = rr.CountryTo;
                fs.Comment = rr.Comment;
                fs.CommentDate = rr.CommentDate;
                lstshipmentTracingDetail.Add(fs);
            }
            return lstshipmentTracingDetail;
        }
    }
}
