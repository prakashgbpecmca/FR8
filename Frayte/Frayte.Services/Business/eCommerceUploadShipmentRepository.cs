using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using System.Data.Entity.Validation;
using Microsoft.Office.Interop.Excel;
using System.Data;
using LinqToExcel;
using System.Collections;
using System.Net.Mail;
using System.IO;
using System.Web;
using Spire.Barcode;
using System.Drawing;
using AutoMapper;

namespace Frayte.Services.Business
{
    public class eCommerceUploadShipmentRepository
    {
        FrayteEntities dbContext = new FrayteEntities();


        public List<FrayteUserDirectShipment> GetUnSuccessfulShipments(int CustomerId)
        {

            //List<FrayteCommerceShipmentDraft> result = dbContext.DirectShipmentDrafts.Where(a => a.BookingApp == "ECOMMERCE_WS").ToList();

            var result = (from DSD in dbContext.DirectShipmentDrafts
                          join FAD in dbContext.DirectShipmentAddressDrafts
                          on DSD.FromAddressId equals FAD.DirectShipmentAddressDraftId
                          join TAD in dbContext.DirectShipmentAddressDrafts
                          on DSD.ToAddressId equals TAD.DirectShipmentAddressDraftId
                          join CL in dbContext.CountryLogistics on TAD.CountryId equals CL.CountryId into AS
                          from CL in AS.DefaultIfEmpty()
                          join Usr in dbContext.Users on DSD.CustomerId equals Usr.UserId
                          join SS in dbContext.ShipmentStatus on DSD.ShipmentStatusId equals SS.ShipmentStatusId

                          where DSD.BookingApp == "ECOMMERCE_WS" && DSD.CustomerId == CustomerId
                          select new FrayteUserDirectShipment()
                          {
                              ShipmentId = DSD.DirectShipmentDraftId,
                              ShippedFromCompany = FAD.CompanyName,
                              ShippedToCompany = TAD.CompanyName,
                              FrayteNumber = DSD.FrayteNumber,
                              Customer = Usr.ContactName,
                              Status = SS.StatusName,
                              DisplayStatus = SS.DisplayStatusName,
                              ShippingBy = CL.LogisticService,
                              DisplayName = CL.LogisicServiceDisplay,
                              TrackingNo = DSD.TrackingDetail
                          }).ToList();

            return result;

        }



        public UploadShipmentBatchProcess GetUpdatedBatchProcess(int CustomerId)
        {
            var result = (from BP in dbContext.eCommerceUploadShipmentBatchProcesses
                          where BP.CustomerId == CustomerId
                          select new UploadShipmentBatchProcess()
                          {
                              CustomerId = BP.CustomerId == null ? 0 : BP.CustomerId,
                              ProcessedShipment = BP.ProcessedShipment == null ? 0 : BP.ProcessedShipment,
                              UnprocessedShipment = BP.UnprocessedShipment == null ? 0 : BP.UnprocessedShipment,
                              TotalShipments = BP.TotalBatchProcess

                          }).FirstOrDefault();

            //var res = dbContext.eCommerceUploadShipmentBatchProcesses.Where(a => a.CustomerId == CustomerId).FirstOrDefault();
            //if(res != null)
            //{
            //    res.TotalBatchProcess = result.TotalShipments;
            //    dbContext.Entry(res).State = System.Data.Entity.EntityState.Modified;
            //}

            return result;
        }

        public FrayteUserDirectShipment GetUnSuccessfulShipmentsWithService(int DirectShipmentDraftId)
        {

            //List<FrayteCommerceShipmentDraft> result = dbContext.DirectShipmentDrafts.Where(a => a.BookingApp == "ECOMMERCE_WS").ToList();

            var result = (from DSD in dbContext.DirectShipmentDrafts
                          join FAD in dbContext.DirectShipmentAddressDrafts
                          on DSD.FromAddressId equals FAD.DirectShipmentAddressDraftId
                          join TAD in dbContext.DirectShipmentAddressDrafts
                          on DSD.ToAddressId equals TAD.DirectShipmentAddressDraftId
                          join CL in dbContext.CountryLogistics on TAD.CountryId equals CL.CountryId into DS
                          from CL in DS.DefaultIfEmpty()
                          join Usr in dbContext.Users on DSD.CustomerId equals Usr.UserId
                          join SS in dbContext.ShipmentStatus on DSD.ShipmentStatusId equals SS.ShipmentStatusId

                          where DSD.BookingApp == "ECOMMERCE_SS" && DSD.DirectShipmentDraftId == DirectShipmentDraftId
                          select new FrayteUserDirectShipment()
                          {
                              ShipmentId = DSD.DirectShipmentDraftId,
                              ShippedFromCompany = FAD.CompanyName,
                              ShippedToCompany = TAD.CompanyName,
                              FrayteNumber = DSD.FrayteNumber,
                              Customer = Usr.ContactName,
                              Status = SS.StatusName,
                              DisplayStatus = SS.DisplayStatusName,
                              ShippingBy = CL.LogisticService,
                              DisplayName = CL.LogisicServiceDisplay,
                              IsEasyPostError = DSD.EasyPostErrorObject == null || DSD.EasyPostErrorObject == "" ? false : true


                          }).FirstOrDefault();

            return result;

        }

        public FrayteeCommerceWithServiceShipmentfilter GetShipmentsFromDraft(int CustomerId)
        {

            List<FrayteUploadshipment> result = (from DSD in dbContext.DirectShipmentDrafts
                                                 let id = DSD.DirectShipmentDraftId
                                                 join FAD in dbContext.DirectShipmentAddressDrafts
                                                 on DSD.FromAddressId equals FAD.DirectShipmentAddressDraftId
                                                 join TAD in dbContext.DirectShipmentAddressDrafts
                                                 on DSD.ToAddressId equals TAD.DirectShipmentAddressDraftId
                                                 join CL in dbContext.CountryLogistics on TAD.CountryId equals CL.CountryId into PS
                                                 from CL in PS.DefaultIfEmpty()
                                                 join Usr in dbContext.Users on DSD.CustomerId equals Usr.UserId
                                                 join Ctr in dbContext.Countries on TAD.CountryId equals Ctr.CountryId into AS
                                                 from Ctr in AS.DefaultIfEmpty()
                                                 join SS in dbContext.ShipmentStatus on DSD.ShipmentStatusId equals SS.ShipmentStatusId
                                                 //join DSDD in dbContext.DirectShipmentDetailDrafts on DSD.DirectShipmentDraftId equals DSDD.DirectShipmentDraftId
                                                 where
                                                        DSD.BookingApp == "ECOMMERCE_SS" && DSD.CustomerId == CustomerId
                                                 select new FrayteUploadshipment()
                                                 {
                                                     DirectShipmentDraftId = DSD.DirectShipmentDraftId,
                                                     CustomerId = DSD.CustomerId,
                                                     IsSelectServiceStatus = DSD.IsSelectServiceStatus,
                                                     ShipFrom = new DirectBookingCollection()
                                                     {
                                                         Country = new FrayteCountryCode()
                                                         {
                                                             Code = Ctr == null ? "" : Ctr.CountryCode,
                                                             Code2 = Ctr == null ? "" : Ctr.CountryCode2,
                                                             CountryId = Ctr == null ? 0 : Ctr.CountryId,
                                                             Name = Ctr == null ? "" : Ctr.CountryName
                                                         },
                                                         PostCode = FAD.Zip,
                                                         FirstName = FAD.ContactFirstName,
                                                         LastName = FAD.ContactLastName,
                                                         CompanyName = FAD.CompanyName,
                                                         Address = FAD.Address1,
                                                         Address2 = FAD.Address2,
                                                         City = FAD.City,
                                                         Phone = FAD.PhoneNo,
                                                         Email = FAD.Email
                                                     },

                                                     ShipTo = new DirectBookingCollection()
                                                     {
                                                         Country = new FrayteCountryCode()
                                                         {
                                                             Code = Ctr == null ? "" : Ctr.CountryCode,
                                                             Code2 = Ctr == null ? "" : Ctr.CountryCode2,
                                                             CountryId = Ctr == null ? 0 : Ctr.CountryId,
                                                             Name = Ctr == null ? "" : Ctr.CountryName
                                                         },
                                                         PostCode = TAD.Zip,
                                                         FirstName = TAD.ContactFirstName,
                                                         LastName = TAD.ContactLastName,
                                                         CompanyName = TAD.CompanyName,
                                                         Address = TAD.Address1,
                                                         Address2 = TAD.Address2,
                                                         City = TAD.City,
                                                         Phone = TAD.PhoneNo,
                                                         Email = TAD.Email
                                                     },
                                                     Package = (from b in dbContext.DirectShipmentDrafts
                                                                join a in dbContext.DirectShipmentDetailDrafts on b.DirectShipmentDraftId equals a.DirectShipmentDraftId
                                                                where a.DirectShipmentDraftId == id
                                                                select new UploadShipmentPackage
                                                                {
                                                                    CartoonValue = a.CartoonValue.HasValue ? a.CartoonValue.Value : 0,
                                                                    Length = a.Length.HasValue ? a.Length.Value : 0,
                                                                    Width = a.Width.HasValue ? a.Width.Value : 0,
                                                                    Weight = a.Weight.HasValue ? a.Weight.Value : 0,
                                                                    Height = a.Height.HasValue ? a.Height.Value : 0,
                                                                    Value = a.DeclaredValue.HasValue ? a.DeclaredValue.Value : 0,
                                                                    Content = a.PiecesContent
                                                                }).ToList(),
                                                     PackageCalculationType = DSD.PackageCaculatonType,
                                                     PayTaxAndDuties = DSD.PaymentPartyTaxAndDuties,
                                                     parcelType = DSD.ParcelType,
                                                     CurrencyCode = DSD.CurrencyCode,
                                                     ShipmentReference = DSD.Reference1,
                                                     ShipmentDescription = DSD.ContentDescription,
                                                     EstimatedDateofArrival = DSD.EstimatedDateofArrival,
                                                     EstimatedTimeofArrival = DSD.EstimatedTimeofArrival.ToString(),
                                                     EstimatedDateofDelivery = DSD.EstimatedDateofDelivery,
                                                     EstimatedTimeofDelivery = DSD.EstimatedTimeofDelivery.ToString()
                                                 }).ToList();

            //foreach(var a in result)
            //{
            //    //a.Package = new List<UploadShipmentPackage>();
            //    a.Package = dbContext.DirectShipmentDetailDrafts.Where(a => a.DirectShipmentDraftId == a.DirectShipmentDraftId).ToList();
            //}

            ErrorLog(result, "ECOMMERCE_SS");
            FrayteeCommerceWithServiceShipmentfilter shipmentfilter = new FrayteeCommerceWithServiceShipmentfilter();
            shipmentfilter.SucessfulShipments = new List<FrayteUserDirectShipment>();
            shipmentfilter.UnsucessfulShipments = new List<FrayteUserDirectShipment>();
            foreach (var res in result)
            {
                if (res.Errors.Count > 0)
                {

                    var getShipment = GetUnSuccessfulShipmentsWithService(res.DirectShipmentDraftId);
                    getShipment.IsSuccessFull = true;
                    getShipment.IsSelectServiceStatus = res.IsSelectServiceStatus;
                    shipmentfilter.UnsucessfulShipments.Add(getShipment);
                }
                else
                {
                    var getShipment = GetUnSuccessfulShipmentsWithService(res.DirectShipmentDraftId);
                    getShipment.IsSuccessFull = false;
                    getShipment.IsSelectServiceStatus = res.IsSelectServiceStatus;
                    shipmentfilter.SucessfulShipments.Add(getShipment);
                }
            }

            return shipmentfilter;

        }

        public List<FrayteUploadshipment> GetShipmentsFromDraftTable(int CustomerId, string ServiceType)
        {

            List<FrayteUploadshipment> result = (from DSD in dbContext.DirectShipmentDrafts
                                                 let id = DSD.DirectShipmentDraftId
                                                 join FAD in dbContext.DirectShipmentAddressDrafts
                                                 on DSD.FromAddressId equals FAD.DirectShipmentAddressDraftId
                                                 join TAD in dbContext.DirectShipmentAddressDrafts
                                                 on DSD.ToAddressId equals TAD.DirectShipmentAddressDraftId
                                                 join CL in dbContext.CountryLogistics on TAD.CountryId equals CL.CountryId into PS
                                                 from CL in PS.DefaultIfEmpty()
                                                 join Usr in dbContext.Users on DSD.CustomerId equals Usr.UserId
                                                 join Ctr in dbContext.Countries on TAD.CountryId equals Ctr.CountryId into AS
                                                 from Ctr in AS.DefaultIfEmpty()
                                                 join SS in dbContext.ShipmentStatus on DSD.ShipmentStatusId equals SS.ShipmentStatusId
                                                 //join DSDD in dbContext.DirectShipmentDetailDrafts on DSD.DirectShipmentDraftId equals DSDD.DirectShipmentDraftId
                                                 where
                                                        DSD.BookingApp == ServiceType && DSD.CustomerId == CustomerId
                                                 select new FrayteUploadshipment()
                                                 {
                                                     DirectShipmentDraftId = DSD.DirectShipmentDraftId,
                                                     CustomerId = DSD.CustomerId,
                                                     ShipFrom = new DirectBookingCollection()
                                                     {
                                                         Country = new FrayteCountryCode()
                                                         {
                                                             Code = Ctr == null ? "" : Ctr.CountryCode,
                                                             Code2 = Ctr == null ? "" : Ctr.CountryCode2,
                                                             CountryId = Ctr == null ? 0 : Ctr.CountryId,
                                                             Name = Ctr == null ? "" : Ctr.CountryName
                                                         },
                                                         PostCode = FAD.Zip,
                                                         FirstName = FAD.ContactFirstName,
                                                         LastName = FAD.ContactLastName,
                                                         CompanyName = FAD.CompanyName,
                                                         Address = FAD.Address1,
                                                         Address2 = FAD.Address2,
                                                         City = FAD.City,
                                                         Phone = FAD.PhoneNo,
                                                         Email = FAD.Email
                                                     },

                                                     ShipTo = new DirectBookingCollection()
                                                     {
                                                         Country = new FrayteCountryCode()
                                                         {
                                                             Code = Ctr == null ? "" : Ctr.CountryCode,
                                                             Code2 = Ctr == null ? "" : Ctr.CountryCode2,
                                                             CountryId = Ctr == null ? 0 : Ctr.CountryId,
                                                             Name = Ctr == null ? "" : Ctr.CountryName
                                                         },
                                                         PostCode = TAD.Zip,
                                                         FirstName = TAD.ContactFirstName,
                                                         LastName = TAD.ContactLastName,
                                                         CompanyName = TAD.CompanyName,
                                                         Address = TAD.Address1,
                                                         Address2 = TAD.Address2,
                                                         City = TAD.City,
                                                         Phone = TAD.PhoneNo,
                                                         Email = TAD.Email
                                                     },
                                                     Package = (from b in dbContext.DirectShipmentDrafts
                                                                join a in dbContext.DirectShipmentDetailDrafts on b.DirectShipmentDraftId equals a.DirectShipmentDraftId
                                                                where a.DirectShipmentDraftId == id
                                                                select new UploadShipmentPackage
                                                                {
                                                                    CartoonValue = a.CartoonValue.HasValue ? a.CartoonValue.Value : 0,
                                                                    Length = a.Length.HasValue ? a.Length.Value : 0,
                                                                    Width = a.Width.HasValue ? a.Width.Value : 0,
                                                                    Weight = a.Weight.HasValue ? a.Weight.Value : 0,
                                                                    Height = a.Height.HasValue ? a.Height.Value : 0,
                                                                    Value = a.DeclaredValue.HasValue ? a.DeclaredValue.Value : 0,
                                                                    Content = a.PiecesContent
                                                                }).ToList(),
                                                     PackageCalculationType = DSD.PackageCaculatonType,
                                                     PayTaxAndDuties = DSD.PaymentPartyTaxAndDuties,
                                                     parcelType = DSD.ParcelType,
                                                     CurrencyCode = DSD.CurrencyCode,
                                                     ShipmentReference = DSD.Reference1,
                                                     ShipmentDescription = DSD.ContentDescription,
                                                     EstimatedDateofArrival = DSD.EstimatedDateofArrival,
                                                     EstimatedTimeofArrival = DSD.EstimatedTimeofArrival.ToString(),
                                                     EstimatedDateofDelivery = DSD.EstimatedDateofDelivery,
                                                     EstimatedTimeofDelivery = DSD.EstimatedTimeofDelivery.ToString()
                                                 }).ToList();

            //foreach(var a in result)
            //{
            //    //a.Package = new List<UploadShipmentPackage>();
            //    a.Package = dbContext.DirectShipmentDetailDrafts.Where(a => a.DirectShipmentDraftId == a.DirectShipmentDraftId).ToList();
            //}




            return result;

        }


        public List<FrayteUploadshipment> GetShipmentErrors(int ShipmentId, string ServiceType)
        {


            List<FrayteUploadshipment> result = (from DSD in dbContext.DirectShipmentDrafts
                                                 let id = DSD.DirectShipmentDraftId
                                                 join FAD in dbContext.DirectShipmentAddressDrafts
                                                      on DSD.FromAddressId equals FAD.DirectShipmentAddressDraftId
                                                 join TAD in dbContext.DirectShipmentAddressDrafts
                                                      on DSD.ToAddressId equals TAD.DirectShipmentAddressDraftId
                                                 join CL in dbContext.CountryLogistics
                                                      on TAD.CountryId equals CL.CountryId into DS
                                                 from CL in DS.DefaultIfEmpty()
                                                 join Usr in dbContext.Users
                                                      on DSD.CustomerId equals Usr.UserId
                                                 join Ctr in dbContext.Countries
                                                      on TAD.CountryId equals Ctr.CountryId into AS
                                                 from Ctr in AS.DefaultIfEmpty()
                                                 join Ctry in dbContext.Countries
                                                      on FAD.CountryId equals Ctry.CountryId into ASS
                                                 from Ctry in ASS.DefaultIfEmpty()
                                                 join SS in dbContext.ShipmentStatus
                                                      on DSD.ShipmentStatusId equals SS.ShipmentStatusId
                                                 //join DSDD in dbContext.DirectShipmentDetailDrafts on DSD.DirectShipmentDraftId equals DSDD.DirectShipmentDraftId
                                                 where
                                                        DSD.BookingApp == ServiceType && DSD.DirectShipmentDraftId == ShipmentId
                                                 select new FrayteUploadshipment()
                                                 {
                                                     EasyPostError = DSD.EasyPostErrorObject == null || DSD.EasyPostErrorObject == "" ? "" : DSD.EasyPostErrorObject,
                                                     DirectShipmentDraftId = DSD.DirectShipmentDraftId,
                                                     FrayteNumber = DSD.FrayteNumber,

                                                     ShipFrom = new DirectBookingCollection()
                                                     {
                                                         Country = new FrayteCountryCode()
                                                         {
                                                             Code = Ctry == null ? "" : Ctry.CountryCode,
                                                             Code2 = Ctry == null ? "" : Ctry.CountryCode2,
                                                             CountryId = Ctry == null ? 0 : Ctry.CountryId,
                                                             Name = Ctry == null ? "" : Ctry.CountryName
                                                         },
                                                         PostCode = FAD.Zip,
                                                         FirstName = FAD.ContactFirstName,
                                                         LastName = FAD.ContactLastName,
                                                         CompanyName = FAD.CompanyName,
                                                         Address = FAD.Address1,
                                                         Address2 = FAD.Address2,
                                                         City = FAD.City,
                                                         Phone = FAD.PhoneNo,
                                                         Email = FAD.Email,
                                                         DirectShipmentAddressId = FAD.DirectShipmentAddressDraftId
                                                     },

                                                     ShipTo = new DirectBookingCollection()
                                                     {
                                                         Country = new FrayteCountryCode()
                                                         {
                                                             Code = Ctr == null ? "" : Ctr.CountryCode,
                                                             Code2 = Ctr == null ? "" : Ctr.CountryCode2,
                                                             CountryId = Ctr == null ? 0 : Ctr.CountryId,
                                                             Name = Ctr == null ? "" : Ctr.CountryName
                                                         },
                                                         PostCode = TAD.Zip,
                                                         FirstName = TAD.ContactFirstName,
                                                         LastName = TAD.ContactLastName,
                                                         CompanyName = TAD.CompanyName,
                                                         Address = TAD.Address1,
                                                         Address2 = TAD.Address2,
                                                         City = TAD.City,
                                                         Phone = TAD.PhoneNo,
                                                         Email = TAD.Email,
                                                         DirectShipmentAddressId = TAD.DirectShipmentAddressDraftId
                                                     },
                                                     Package = (from b in dbContext.DirectShipmentDrafts
                                                                join a in dbContext.DirectShipmentDetailDrafts on b.DirectShipmentDraftId equals a.DirectShipmentDraftId
                                                                where b.DirectShipmentDraftId == ShipmentId
                                                                select new UploadShipmentPackage
                                                                {
                                                                    CartoonValue = a.CartoonValue.HasValue ? a.CartoonValue.Value : 0,
                                                                    Length = a.Length.HasValue ? a.Length.Value : 0,
                                                                    Width = a.Width.HasValue ? a.Width.Value : 0,
                                                                    Weight = a.Weight.HasValue ? a.Weight.Value : 0,
                                                                    Height = a.Height.HasValue ? a.Height.Value : 0,
                                                                    Value = a.DeclaredValue.HasValue ? a.DeclaredValue.Value : 0,
                                                                    Content = a.PiecesContent
                                                                }).ToList(),
                                                     CustomerId = DSD.CustomerId,
                                                     PackageCalculationType = DSD.PackageCaculatonType,
                                                     PayTaxAndDuties = DSD.PaymentPartyTaxAndDuties,
                                                     parcelType = DSD.ParcelType,
                                                     CurrencyCode = DSD.CurrencyCode,
                                                     ShipmentReference = DSD.Reference1,
                                                     ShipmentDescription = DSD.ContentDescription,
                                                     EstimatedDateofArrival = DSD.EstimatedDateofArrival,
                                                     EstimatedTimeofArrival = DSD.EstimatedTimeofArrival.ToString(),
                                                     EstimatedDateofDelivery = DSD.EstimatedDateofDelivery,
                                                     EstimatedTimeofDelivery = DSD.EstimatedTimeofDelivery.ToString(),
                                                     TrackingNo = DSD.TrackingDetail,
                                                     CourierCompany = DSD.LogisticServiceType,
                                                     CourierCompanyDisplay = CL.LogisicServiceDisplay

                                                 }).ToList();

            //foreach(var a in result)
            //{
            //    //a.Package = new List<UploadShipmentPackage>();
            //    a.Package = dbContext.DirectShipmentDetailDrafts.Where(a => a.DirectShipmentDraftId == a.DirectShipmentDraftId).ToList();
            //}

            ErrorLog(result, ServiceType);

            foreach (var res in result)
            {
                res.RemainedFields = new List<FailedValidationObj>();

                foreach (var columns in res.Errors)
                {

                    if (columns.Contains("FromCountryCode"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "From Country";
                        FVObj.FieldName = "Country";
                        FVObj.FileType = "DropDown";
                        FVObj.ShowDiv = "FromShipper";
                        FVObj.DashString = "";
                        FVObj.TrackBYOBJ = "ParcelType.CountryId";
                        FVObj.IterationFor1 = "ParcelType.CountryName";
                        FVObj.IterationForAs = "ParcelType";
                        FVObj.IterationFor2 = "";
                        FVObj.InputTypeName = "fromCountry";
                        FVObj.ConditionalRequired = "Shipment[0].ShipFrom.[er.FieldName].CountryId === 0";
                        FVObj.LabelName = "fromCountryLabel";
                        FVObj.RequiredMessage = "CountryValidationError";
                        FVObj.GeneralObj = (dbContext.Countries.ToList() as IEnumerable<object>).Cast<object>().ToList();
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("FromPostCode"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "From FromPostCode";
                        FVObj.FieldName = "FromPostCode";
                        FVObj.ShowDiv = "FromShipper";
                        FVObj.FileType = "text";
                        FVObj.LabelName = "fromPostCodeLabel";
                        FVObj.RequiredMessage = "PostalCodeValidationError";
                        FVObj.InputTypeName = "fromPostCode";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("FromContactFirstName"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "From Contact First Name";
                        FVObj.FieldName = "FirstName";
                        FVObj.ShowDiv = "FromShipper";
                        FVObj.FileType = "text";
                        FVObj.LabelName = "fromFirstNameLabel";
                        FVObj.RequiredMessage = "FirstName_Required";
                        FVObj.InputTypeName = "fromFirstName";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("FromContactLastName"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "From Contact Last Name";
                        FVObj.FieldName = "LastName";
                        FVObj.ShowDiv = "FromShipper";
                        FVObj.FileType = "text";
                        FVObj.LabelName = "fromLastNameLabel";
                        FVObj.InputTypeName = "fromLastName";
                        FVObj.RequiredMessage = "LastName_Required";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("FromCompanyName"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "From Company Name";
                        FVObj.FieldName = "CompanyName";
                        FVObj.ShowDiv = "FromShipper";
                        FVObj.FileType = "text";
                        FVObj.LabelName = "fromCompanyNameLabel";
                        FVObj.InputTypeName = "fromCompanyName";
                        FVObj.RequiredMessage = "CompanyNameValidationError";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("FromAddress1"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "From Address 1";
                        FVObj.FieldName = "Address";
                        FVObj.FileType = "text";
                        FVObj.ShowDiv = "FromShipper";
                        FVObj.LabelName = "fromAddressNameLabel";
                        FVObj.InputTypeName = "fromAddress";
                        FVObj.RequiredMessage = "AddressValidationError";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("FromAddress2"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "From Address 2";
                        FVObj.FieldName = "Address2";
                        FVObj.FileType = "text";
                        FVObj.ShowDiv = "FromShipper";
                        FVObj.LabelName = "fromAddress2NameLabel";
                        FVObj.InputTypeName = "fromAddress2";
                        FVObj.RequiredMessage = "Address 2 is required.";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("FromCity"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "From City";
                        FVObj.FieldName = "City";
                        FVObj.FileType = "text";
                        FVObj.ShowDiv = "FromShipper";
                        FVObj.LabelName = "fromCityNameLabel";
                        FVObj.InputTypeName = "fromCity";
                        FVObj.RequiredMessage = "CityValidationError";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("FromTelephoneNo"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "From TelephoneNo";
                        FVObj.FieldName = "Phone";
                        FVObj.FileType = "text";
                        FVObj.ShowDiv = "FromShipper";
                        FVObj.LabelName = "fromPhoneLabel";
                        FVObj.InputTypeName = "fromPhone";
                        FVObj.RequiredMessage = "TelephoneValidationError";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("FromEmail"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "From Email";
                        FVObj.FieldName = "Email";
                        FVObj.FileType = "email";
                        FVObj.ShowDiv = "FromShipper";
                        FVObj.LabelName = "fromEmailLabel";
                        FVObj.InputTypeName = "fromEmail";
                        FVObj.RequiredMessage = "EmailValidationError";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("ToCountryCode"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "To Country";
                        FVObj.FieldName = "Country";
                        FVObj.FileType = "DropDown";
                        FVObj.ShowDiv = "ToShipper";
                        FVObj.DashString = "";
                        FVObj.TrackBYOBJ = "ParcelType.CountryId";
                        FVObj.IterationFor1 = "ParcelType.CountryName";
                        FVObj.IterationFor2 = "";
                        FVObj.InputTypeName = "toCountry";
                        FVObj.IterationForAs = "ParcelType";
                        FVObj.ConditionalRequired = "Country.Code === 0";
                        FVObj.RequiredMessage = "CountryValidationError";
                        FVObj.LabelName = "toCountryLabel";
                        FVObj.GeneralObj = (new MasterDataRepository().ToCountryList() as IEnumerable<object>).Cast<object>().ToList();
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("ToPostCode"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "To PostCode";
                        FVObj.FieldName = "PostCode";
                        FVObj.FileType = "Text";
                        FVObj.ShowDiv = "ToShipper";
                        FVObj.LabelName = "toPostCodeLabel";
                        FVObj.InputTypeName = "toPostCode";
                        FVObj.RequiredMessage = "PostalCodeValidationError";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("ToContactFirstName"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "To Contact First Name";
                        FVObj.FieldName = "FirstName";
                        FVObj.FileType = "text";
                        FVObj.ShowDiv = "ToShipper";
                        FVObj.LabelName = "toFirstNameLabel";
                        FVObj.InputTypeName = "toFirstName";
                        FVObj.RequiredMessage = "FirstName_Required";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("ToContactLastName"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "To Contact Last Name";
                        FVObj.FieldName = "LastName";
                        FVObj.FileType = "text";
                        FVObj.ShowDiv = "ToShipper";
                        FVObj.LabelName = "toLastNameLabel";
                        FVObj.InputTypeName = "toLastName";
                        FVObj.RequiredMessage = "LastName_Required";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("ToCompanyName"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "To Company Name";
                        FVObj.FieldName = "CompanyName";
                        FVObj.FileType = "text";
                        FVObj.ShowDiv = "ToShipper";
                        FVObj.LabelName = "toCompanyNameLabel";
                        FVObj.InputTypeName = "toCompanyName";
                        FVObj.RequiredMessage = "CompanyNameValidationError";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("ToAddress1"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "To Address1";
                        FVObj.FieldName = "Address";
                        FVObj.FileType = "text";
                        FVObj.ShowDiv = "ToShipper";
                        FVObj.LabelName = "toAddress1Label";
                        FVObj.InputTypeName = "toAddress1";
                        FVObj.RequiredMessage = "AddressValidationError";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("ToAddress2"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "To Address2";
                        FVObj.FieldName = "Address2";
                        FVObj.FileType = "text";
                        FVObj.ShowDiv = "ToShipper";
                        FVObj.LabelName = "toAddress2Label";
                        FVObj.InputTypeName = "toAddress2";
                        FVObj.RequiredMessage = "Address2 is required.";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("ToCity"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "To City";
                        FVObj.FieldName = "City";
                        FVObj.FileType = "text";
                        FVObj.ShowDiv = "ToShipper";
                        FVObj.LabelName = "toCityLabel";
                        FVObj.InputTypeName = "toCity";
                        FVObj.RequiredMessage = "CityValidationError";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("ToTelephoneNo"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "To TelephoneNo";
                        FVObj.FieldName = "Phone";
                        FVObj.FileType = "text";
                        FVObj.ShowDiv = "ToShipper";
                        FVObj.LabelName = "toPhoneLabel";
                        FVObj.InputTypeName = "toPhone";
                        FVObj.RequiredMessage = "TelephoneValidationError";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("ToEmail"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "To Email";
                        FVObj.FieldName = "Email";
                        FVObj.FileType = "email";
                        FVObj.ShowDiv = "ToShipper";
                        FVObj.LabelName = "toEmailLabel";
                        FVObj.InputTypeName = "toEmail";
                        FVObj.RequiredMessage = "EmailValidationError";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("PackageCalculationType"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "Package Calculation Type";
                        FVObj.FieldName = "PakageCalculatonType";
                        FVObj.FileType = "radio";
                        FVObj.InputTypeName = "pakageCalculatonType";
                        FVObj.LabelName = "pakageCalculatonTypeLabel";
                        FVObj.RequiredMessage = "PackageCalculatonType is required.";
                        FVObj.RadioButtonValues = new List<string>() { "KgToCms", "lbToInchs" };

                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("ParcelType"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "Parcel Type";
                        FVObj.FieldName = "ParcelType";
                        FVObj.FileType = "DropDown";
                        FVObj.DashString = "";
                        FVObj.TrackBYOBJ = "ParcelType.ParcelType";
                        FVObj.IterationFor1 = "ParcelType.ParcelDescription";
                        FVObj.IterationFor2 = "";
                        FVObj.InputTypeName = "parcelType";
                        FVObj.LabelName = "parcelTypeLabel";
                        FVObj.IterationForAs = "ParcelType.ParcelType";
                        FVObj.RequiredMessage = "ParcelType_required";
                        FVObj.ShowDiv = "MainObjProperty";
                        FVObj.GeneralObj = (new MasterDataRepository().GetParcelType() as IEnumerable<object>).Cast<object>().ToList();
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("Currency"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "Currency";
                        FVObj.FieldName = "Currency";
                        FVObj.FileType = "DropDown";
                        FVObj.DashString = "+ ' - ' +";
                        FVObj.TrackBYOBJ = "ParcelType.CurrencyCode";
                        FVObj.IterationFor1 = "ParcelType.CurrencyCode";
                        FVObj.IterationFor2 = "ParcelType.CurrencyDescription";
                        FVObj.InputTypeName = "currency";
                        FVObj.LabelName = "currencyLabel";
                        FVObj.ShowDiv = "MainObjProperty";
                        FVObj.IterationForAs = "ParcelType.CurrencyCode";
                        FVObj.RequiredMessage = "Currency_required";
                        FVObj.GeneralObj = (dbContext.CurrencyTypes.ToList() as IEnumerable<object>).Cast<object>().ToList();
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("ShipmentReference"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "From Country";
                        FVObj.FieldName = "Reference1";
                        FVObj.FileType = "text";
                        FVObj.LabelName = "courierCompanyLable";
                        FVObj.ShowDiv = "MainObjProperty";
                        FVObj.InputTypeName = "courierCompany";
                        FVObj.RequiredMessage = "Reference_required";
                        res.RemainedFields.Add(FVObj);
                    }

                    if (columns.Contains("CourierCompany"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "Courier Company";
                        FVObj.FieldName = "CourierCompany";
                        FVObj.FileType = "text";
                        FVObj.LabelName = "courierCompanyLable";
                        FVObj.ShowDiv = "MainObjProperty";
                        FVObj.InputTypeName = "courierCompany";
                        FVObj.RequiredMessage = "Courier Company is required.";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("TrackingNo"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "Tracking No";
                        FVObj.FieldName = "TrackingNo";
                        FVObj.FileType = "text";
                        FVObj.LabelName = "trackingNoLable";
                        FVObj.ShowDiv = "MainObjProperty";
                        FVObj.InputTypeName = "trackingNo";
                        FVObj.RequiredMessage = "Tracking No is required.";
                        res.RemainedFields.Add(FVObj);
                    }

                    //if (!columns.Contains("ETADate"))
                    //{
                    //    FailedValidationObj FVObj = new FailedValidationObj();
                    //}
                    //if (!columns.Contains("ETATime"))
                    //{
                    //    FailedValidationObj FVObj = new FailedValidationObj();
                    //}
                    //if (!columns.Contains("ETDDate"))
                    //{
                    //    FailedValidationObj FVObj = new FailedValidationObj();
                    //}
                    //if (!columns.Contains("ETDTime"))
                    //{
                    //    FailedValidationObj FVObj = new FailedValidationObj();
                    //}

                    //if (columns.Contains("CartonValue"))
                    //{
                    //    FailedValidationObj FVObj = new FailedValidationObj();
                    //    FVObj.FieldLabel = "From Country";
                    //    FVObj.FieldName = "FromCountry";
                    //    FVObj.FieldValidation = "Text";
                    //    FVObj.FileType = "Text";
                    //    res.RemainedFields.Add(FVObj);
                    //}
                    //if (columns.Contains("Length"))
                    //{
                    //    FailedValidationObj FVObj = new FailedValidationObj();
                    //    FVObj.FieldLabel = "From Country";
                    //    FVObj.FieldName = "FromCountry";
                    //    FVObj.FieldValidation = "Text";
                    //    FVObj.FileType = "Text";
                    //    res.RemainedFields.Add(FVObj);
                    //}
                    //if (columns.Contains("Width"))
                    //{
                    //    FailedValidationObj FVObj = new FailedValidationObj();
                    //    FVObj.FieldLabel = "From Country";
                    //    FVObj.FieldName = "FromCountry";
                    //    FVObj.FieldValidation = "Text";
                    //    FVObj.FileType = "Text";
                    //    res.RemainedFields.Add(FVObj);
                    //}
                    //if (columns.Contains("Height"))
                    //{
                    //    FailedValidationObj FVObj = new FailedValidationObj();
                    //    FVObj.FieldLabel = "From Country";
                    //    FVObj.FieldName = "FromCountry";
                    //    FVObj.FieldValidation = "Text";
                    //    FVObj.FileType = "Text";
                    //    res.RemainedFields.Add(FVObj);
                    //}
                    //if (columns.Contains("Weight"))
                    //{
                    //    FailedValidationObj FVObj = new FailedValidationObj();
                    //    FVObj.FieldLabel = "From Country";
                    //    FVObj.FieldName = "FromCountry";
                    //    FVObj.FieldValidation = "Text";
                    //    FVObj.FileType = "Text";
                    //    res.RemainedFields.Add(FVObj);
                    //}
                    //if (columns.Contains("DeclaredValue"))
                    //{
                    //    FailedValidationObj FVObj = new FailedValidationObj();
                    //    FVObj.FieldLabel = "From Country";
                    //    FVObj.FieldName = "FromCountry";
                    //    FVObj.FieldValidation = "Text";
                    //    FVObj.FileType = "Text";
                    //    res.RemainedFields.Add(FVObj);
                    //}
                    //if (columns.Contains("ShipmentContents"))
                    //{
                    //    FailedValidationObj FVObj = new FailedValidationObj();
                    //    FVObj.FieldLabel = "From Country";
                    //    FVObj.FieldName = "FromCountry";
                    //    FVObj.FieldValidation = "Text";
                    //    FVObj.FileType = "Text";
                    //    res.RemainedFields.Add(FVObj);
                    //}


                }
            }



            return result;

        }

        public FrayteUploadshipment GetShipmentFromDraft(int ShipmentId, string ServiceType)
        {


            FrayteUploadshipment result = (from DSD in dbContext.DirectShipmentDrafts
                                           let id = DSD.DirectShipmentDraftId
                                           join FAD in dbContext.DirectShipmentAddressDrafts
                                           on DSD.FromAddressId equals FAD.DirectShipmentAddressDraftId
                                           join TAD in dbContext.DirectShipmentAddressDrafts
                                           on DSD.ToAddressId equals TAD.DirectShipmentAddressDraftId
                                           //join CL in dbContext.CountryLogistics on TAD.CountryId equals CL.CountryId
                                           join Usr in dbContext.Users on DSD.CustomerId equals Usr.UserId
                                           join Ctr in dbContext.Countries on TAD.CountryId equals Ctr.CountryId
                                           join SS in dbContext.ShipmentStatus on DSD.ShipmentStatusId equals SS.ShipmentStatusId
                                           //join DSDD in dbContext.DirectShipmentDetailDrafts on DSD.DirectShipmentDraftId equals DSDD.DirectShipmentDraftId
                                           where
                                                  DSD.BookingApp == ServiceType && DSD.DirectShipmentDraftId == ShipmentId
                                           select new FrayteUploadshipment()
                                           {
                                               BookingApp = DSD.BookingApp,
                                               ModuleType = "ECOMMERCE",
                                               CustomerId = DSD.CustomerId,
                                               DirectShipmentDraftId = DSD.DirectShipmentDraftId,
                                               ShipFrom = new DirectBookingCollection()
                                               {
                                                   Country = new FrayteCountryCode()
                                                   {
                                                       Code = Ctr.CountryCode,
                                                       Code2 = Ctr.CountryCode2,
                                                       CountryId = Ctr.CountryId,
                                                       Name = Ctr.CountryName
                                                   },

                                                   PostCode = FAD.Zip,
                                                   FirstName = FAD.ContactFirstName,
                                                   LastName = FAD.ContactLastName,
                                                   CompanyName = FAD.CompanyName,
                                                   Address = FAD.Address1,
                                                   Address2 = FAD.Address2,
                                                   City = FAD.City,
                                                   Phone = FAD.PhoneNo,
                                                   Email = FAD.Email
                                               },

                                               ShipTo = new DirectBookingCollection()
                                               {
                                                   Country = new FrayteCountryCode()
                                                   {
                                                       Code = Ctr.CountryCode,
                                                       Code2 = Ctr.CountryCode2,
                                                       CountryId = Ctr.CountryId,
                                                       Name = Ctr.CountryName
                                                   },
                                                   PostCode = TAD.Zip,
                                                   FirstName = TAD.ContactFirstName,
                                                   LastName = TAD.ContactLastName,
                                                   CompanyName = TAD.CompanyName,
                                                   Address = TAD.Address1,
                                                   Address2 = TAD.Address2,
                                                   City = TAD.City,
                                                   Phone = TAD.PhoneNo,
                                                   Email = TAD.Email
                                               },
                                               Package = (from b in dbContext.DirectShipmentDrafts
                                                          join a in dbContext.DirectShipmentDetailDrafts on b.DirectShipmentDraftId equals a.DirectShipmentDraftId
                                                          where b.DirectShipmentDraftId == ShipmentId
                                                          select new UploadShipmentPackage
                                                          {
                                                              CartoonValue = a.CartoonValue.HasValue ? a.CartoonValue.Value : 0,
                                                              Length = a.Length.HasValue ? a.Length.Value : 0,
                                                              Width = a.Width.HasValue ? a.Width.Value : 0,
                                                              Weight = a.Weight.HasValue ? a.Weight.Value : 0,
                                                              Height = a.Height.HasValue ? a.Height.Value : 0,
                                                              Value = a.DeclaredValue.HasValue ? a.DeclaredValue.Value : 0,
                                                              Content = a.PiecesContent
                                                          }).ToList(),
                                               CourierCompany = "",
                                               PackageCalculationType = DSD.PackageCaculatonType,
                                               PayTaxAndDuties = "Receiver",
                                               FrayteNumber = DSD.FrayteNumber,
                                               parcelType = DSD.ParcelType,
                                               CurrencyCode = DSD.CurrencyCode,
                                               ShipmentReference = DSD.Reference1,
                                               ShipmentDescription = DSD.ContentDescription,
                                               EstimatedDateofArrival = DSD.EstimatedDateofArrival,
                                               EstimatedTimeofArrival = DSD.EstimatedTimeofArrival.ToString(),
                                               EstimatedDateofDelivery = DSD.EstimatedDateofDelivery,
                                               EstimatedTimeofDelivery = DSD.EstimatedTimeofDelivery.ToString()
                                           }).FirstOrDefault();

            //foreach(var a in result)
            //{
            //    //a.Package = new List<UploadShipmentPackage>();
            //    a.Package = dbContext.DirectShipmentDetailDrafts.Where(a => a.DirectShipmentDraftId == a.DirectShipmentDraftId).ToList();
            //}

            //ErrorLog(result, ServiceType);
            return result;

        }


        public List<FrayteUploadshipment> GetAllShipments(System.Data.DataTable exceldata, string ServiceType, string LogisticService)
        {
            List<FrayteUploadshipment> UploadShipmentList = new List<FrayteUploadshipment>();




            //FrayteUploadshipment UploadShipment;
            UploadShipmentPackage Package = new UploadShipmentPackage();
            FrayteUploadshipment UploadShipment = new FrayteUploadshipment();
            UploadShipment.Errors = new List<string>();
            UploadShipment.ShipFrom = new DirectBookingCollection();
            UploadShipment.ShipTo = new DirectBookingCollection();
            UploadShipment.Package = new List<UploadShipmentPackage>();
            int i = 0;

            foreach (DataRow shipmentdetail in exceldata.Rows)
            {
                //UploadShipmentList[i].Package = new List<UploadShipmentPackage>();

                //List<UploadShipmentPackage> UploadShipment1 = new FrayteUploadshipment();
                //var pack = UploadShipment.Package;
                //pack = new List<UploadShipmentPackage>();


                if (shipmentdetail["FromCountryCode"].ToString() == "" && shipmentdetail["ToCountryCode"].ToString() == "" &&
                    shipmentdetail["FromAddress1"].ToString() == "" && shipmentdetail["ToAddress1"].ToString() == "")
                {
                    Package = new UploadShipmentPackage();
                    //UploadShipment.Errors = new List<string>();
                    //UploadShipment1 = new FrayteUploadshipment().Package;

                    //Package.CartoonValue = CommonConversion.ConvertToInt(shipmentdetail["CartonValue"].ToString());
                    //Package.Length = CommonConversion.ConvertToDecimal(shipmentdetail["Length"].ToString());
                    //Package.Width = CommonConversion.ConvertToDecimal(shipmentdetail["Width"].ToString());
                    //Package.Height = CommonConversion.ConvertToDecimal(shipmentdetail["Height"].ToString());
                    //Package.Weight = CommonConversion.ConvertToDecimal(shipmentdetail["Weight"].ToString());
                    //Package.Value = CommonConversion.ConvertToDecimal(shipmentdetail["DeclaredValue"].ToString());
                    //Package.Content = shipmentdetail["ShipmentContents"].ToString();

                    Package.CartoonValue = CommonConversion.ConvertToInt(shipmentdetail["CartonValue"].ToString() != "" || shipmentdetail["CartonValue"].ToString() != null ? shipmentdetail["CartonValue"].ToString() : "");

                    Package.Length = CommonConversion.ConvertToDecimal(shipmentdetail["Length"].ToString() != "" || shipmentdetail["Length"].ToString() != null ? shipmentdetail["Length"].ToString() : "");

                    Package.Width = CommonConversion.ConvertToDecimal(shipmentdetail["Width"].ToString() != "" || shipmentdetail["Width"].ToString() != null ? shipmentdetail["Width"].ToString() : "");

                    Package.Height = CommonConversion.ConvertToDecimal(shipmentdetail["Height"].ToString() != "" || shipmentdetail["Height"].ToString() != null ? shipmentdetail["Height"].ToString() : "");

                    Package.Weight = CommonConversion.ConvertToDecimal(shipmentdetail["Weight"].ToString() != "" || shipmentdetail["Weight"].ToString() != null ? shipmentdetail["Weight"].ToString() : "");

                    Package.Value = CommonConversion.ConvertToDecimal(shipmentdetail["DeclaredValue"].ToString() != "" || shipmentdetail["DeclaredValue"].ToString() != null ? shipmentdetail["DeclaredValue"].ToString() : "");

                    Package.Content = shipmentdetail["ShipmentContents"].ToString() != "" || shipmentdetail["ShipmentContents"].ToString() != null ? shipmentdetail["ShipmentContents"].ToString() : "";

                    UploadShipment.Package.Add(Package);
                    //if (UploadShipmentList.Count >0)
                    //{

                    //    UploadShipmentList[i-1].Package.Add(Package);
                    //}

                }
                else
                {
                    var a = exceldata.Rows.IndexOf(shipmentdetail);
                    if (exceldata.Rows.IndexOf(shipmentdetail) > 0)
                    {
                        UploadShipmentList.Add(UploadShipment);
                        UploadShipment = new FrayteUploadshipment();
                        UploadShipment.Errors = new List<string>();
                        UploadShipment.Package = new List<UploadShipmentPackage>();
                        UploadShipment.ShipFrom = new DirectBookingCollection();
                        UploadShipment.ShipTo = new DirectBookingCollection();
                        Package = new UploadShipmentPackage();
                        UploadShipment.CustomInfo = new CustomInformation();
                        //UploadShipmentList[exceldata.Rows.IndexOf(shipmentdetail)].Package.Add();
                        //UploadShipmentList[0].Add();
                    }
                    //    if (shipmentdetail["FromCountry"].ToString() != "" && shipmentdetail["ToCountry"].ToString() != "" &&
                    //shipmentdetail["FromAddress1"].ToString() == "" && shipmentdetail["ToAddress1"].ToString() == "")
                    //    {
                    //        UploadShipmentList[0].Package.Add(UploadShipment);
                    //    }
                    //UploadShipment = new FrayteUploadshipment();
                    UploadShipment.ShipFrom.Country = new FrayteCountryCode();
                    UploadShipment.ShipTo.Country = new FrayteCountryCode();

                    UploadShipment.ShipFrom.Country.Code = shipmentdetail["FromCountryCode"].ToString() != "" || shipmentdetail["FromCountryCode"].ToString() != null ? shipmentdetail["FromCountryCode"].ToString() : "";

                    UploadShipment.ShipFrom.PostCode = shipmentdetail["FromPostCode"].ToString() != "" || shipmentdetail["FromPostCode"].ToString() != null ? shipmentdetail["FromPostCode"].ToString() : "";

                    UploadShipment.ShipFrom.FirstName = shipmentdetail["FromContactFirstName"].ToString() != "" || shipmentdetail["FromContactFirstName"].ToString() != null ? shipmentdetail["FromContactFirstName"].ToString() : "";

                    UploadShipment.ShipFrom.LastName = shipmentdetail["FromContactLastName"].ToString() != "" || shipmentdetail["FromContactLastName"].ToString() != null ? shipmentdetail["FromContactLastName"].ToString() : "";

                    UploadShipment.ShipFrom.CompanyName = shipmentdetail["FromCompanyName"].ToString() != "" || shipmentdetail["FromCompanyName"].ToString() != null ? shipmentdetail["FromCompanyName"].ToString() : "";

                    UploadShipment.ShipFrom.Address = shipmentdetail["FromAddress1"].ToString() != "" || shipmentdetail["FromAddress1"].ToString() != null ? shipmentdetail["FromAddress1"].ToString() : "";

                    UploadShipment.ShipFrom.Address2 = shipmentdetail["FromAddress2"].ToString() != "" || shipmentdetail["FromAddress2"].ToString() != null ? shipmentdetail["FromAddress2"].ToString() : "";

                    UploadShipment.ShipFrom.City = shipmentdetail["FromCity"].ToString() != "" || shipmentdetail["FromCity"].ToString() != null ? shipmentdetail["FromCity"].ToString() : "";

                    UploadShipment.ShipFrom.Phone = shipmentdetail["FromTelephoneNo"].ToString() != "" || shipmentdetail["FromTelephoneNo"].ToString() != null ? shipmentdetail["FromTelephoneNo"].ToString() : "";

                    UploadShipment.ShipFrom.Email = shipmentdetail["FromEmail"].ToString() != "" || shipmentdetail["FromEmail"].ToString() != null ? shipmentdetail["FromEmail"].ToString() : "";

                    UploadShipment.ShipTo.Country.Code = shipmentdetail["ToCountryCode"].ToString() != "" || shipmentdetail["ToCountryCode"].ToString() != null ? shipmentdetail["ToCountryCode"].ToString() : "";

                    UploadShipment.ShipTo.PostCode = shipmentdetail["ToPostCode"].ToString() != "" || shipmentdetail["ToPostCode"].ToString() != null ? shipmentdetail["ToPostCode"].ToString() : "";

                    UploadShipment.ShipTo.FirstName = shipmentdetail["ToContactFirstName"].ToString() != "" || shipmentdetail["ToContactFirstName"].ToString() != null ? shipmentdetail["ToContactFirstName"].ToString() : "";

                    UploadShipment.ShipTo.LastName = shipmentdetail["ToContactLastName"].ToString() != "" || shipmentdetail["ToContactLastName"].ToString() != null ? shipmentdetail["ToContactLastName"].ToString() : "";

                    UploadShipment.ShipTo.CompanyName = shipmentdetail["ToCompanyName"].ToString() != "" || shipmentdetail["ToCompanyName"].ToString() != null ? shipmentdetail["ToCompanyName"].ToString() : "";

                    UploadShipment.ShipTo.Address = shipmentdetail["ToAddress1"].ToString() != "" || shipmentdetail["ToAddress1"].ToString() != null ? shipmentdetail["ToAddress1"].ToString() : "";

                    UploadShipment.ShipTo.Address2 = shipmentdetail["ToAddress2"].ToString() != "" || shipmentdetail["ToAddress2"].ToString() != null ? shipmentdetail["ToAddress2"].ToString() : "";

                    UploadShipment.ShipTo.City = shipmentdetail["ToCity"].ToString() != "" || shipmentdetail["ToCity"].ToString() != null ? shipmentdetail["ToCity"].ToString() : "";

                    UploadShipment.ShipTo.Phone = shipmentdetail["ToTelephoneNo"].ToString() != "" || shipmentdetail["ToTelephoneNo"].ToString() != null ? shipmentdetail["ToTelephoneNo"].ToString() : "";

                    UploadShipment.ShipTo.Email = shipmentdetail["ToEmail"].ToString() != "" || shipmentdetail["ToEmail"].ToString() != null ? shipmentdetail["ToEmail"].ToString() : "";

                    UploadShipment.PackageCalculationType = shipmentdetail["PackageCalculationType"].ToString() != "" || shipmentdetail["PackageCalculationType"].ToString() != null ? shipmentdetail["PackageCalculationType"].ToString() : "";

                    UploadShipment.PayTaxAndDuties = "Receiver";
                    UploadShipment.parcelType = shipmentdetail["ParcelType"].ToString() != "" || shipmentdetail["ParcelType"].ToString() != null ? shipmentdetail["ParcelType"].ToString() : null;

                    UploadShipment.CurrencyCode = shipmentdetail["Currency"].ToString() != "" || shipmentdetail["Currency"].ToString() != null ? shipmentdetail["Currency"].ToString() : "";

                    UploadShipment.ShipmentReference = shipmentdetail["ShipmentReference"].ToString() != "" || shipmentdetail["ShipmentReference"].ToString() != null ? shipmentdetail["ShipmentReference"].ToString() : "";

                    UploadShipment.ShipmentDescription = shipmentdetail["ShipmentDescription"].ToString() != "" || shipmentdetail["ShipmentDescription"].ToString() != null ? shipmentdetail["ShipmentDescription"].ToString() : null;

                    if (ServiceType == "ECOMMERCE_WS")
                    {
                        UploadShipment.TrackingNo = shipmentdetail["TrackingNo"].ToString() != "" || shipmentdetail["TrackingNo"].ToString() != null ? shipmentdetail["TrackingNo"].ToString() : "";
                        UploadShipment.CourierCompany = shipmentdetail["CourierCompany"].ToString() != "" || shipmentdetail["CourierCompany"].ToString() != null ? shipmentdetail["CourierCompany"].ToString() : "";

                    }
                    UploadShipment.CourierCompany = LogisticService.ToString() != "" || LogisticService.ToString() != null ? LogisticService.ToString() : "";
                    //UploadShipment.ServiceCode = shipmentdetail["ServiceCode"].ToString() != "" || shipmentdetail["ServiceCode"].ToString() != null ? shipmentdetail["ServiceCode"].ToString() : "";

                    //UploadShipment.EstimatedDateofArrival = Convert.ToDateTime(shipmentdetail["ETADate"] != null ? shipmentdetail["ETADate"].ToString() : "");

                    //UploadShipment.EstimatedTimeofArrival = shipmentdetail["ETATime"].ToString() != "" || shipmentdetail["ETATime"].ToString() != null ? shipmentdetail["ETATime"].ToString() : "";

                    //UploadShipment.EstimatedDateofDelivery = Convert.ToDateTime(shipmentdetail["ETADate"] != null ? shipmentdetail["ETADate"].ToString() : "");

                    //UploadShipment.EstimatedTimeofDelivery = shipmentdetail["ETDTime"].ToString() != "" || shipmentdetail["ETDTime"].ToString() != null ? shipmentdetail["ETDTime"].ToString() : "";

                    //UploadShipmentList.Add(UploadShipment);

                    Package.CartoonValue = CommonConversion.ConvertToInt(shipmentdetail["CartonValue"].ToString() != "" || shipmentdetail["CartonValue"].ToString() != null ? shipmentdetail["CartonValue"].ToString() : "");

                    Package.Length = CommonConversion.ConvertToDecimal(shipmentdetail["Length"].ToString() != "" || shipmentdetail["Length"].ToString() != null ? shipmentdetail["Length"].ToString() : "");

                    Package.Width = CommonConversion.ConvertToDecimal(shipmentdetail["Width"].ToString() != "" || shipmentdetail["Width"].ToString() != null ? shipmentdetail["Width"].ToString() : "");

                    Package.Height = CommonConversion.ConvertToDecimal(shipmentdetail["Height"].ToString() != "" || shipmentdetail["Height"].ToString() != null ? shipmentdetail["Height"].ToString() : "");

                    Package.Weight = CommonConversion.ConvertToDecimal(shipmentdetail["Weight"].ToString() != "" || shipmentdetail["Weight"].ToString() != null ? shipmentdetail["Weight"].ToString() : "");

                    Package.Value = CommonConversion.ConvertToDecimal(shipmentdetail["DeclaredValue"].ToString() != "" || shipmentdetail["DeclaredValue"].ToString() != null ? shipmentdetail["DeclaredValue"].ToString() : "");

                    Package.Content = shipmentdetail["ShipmentContents"].ToString() != "" || shipmentdetail["ShipmentContents"].ToString() != null ? shipmentdetail["ShipmentContents"].ToString() : "";

                    UploadShipment.Package.Add(Package);
                    i++;
                }
            }
            UploadShipmentList.Add(UploadShipment);
            return UploadShipmentList;
        }

        public int SaveEcommFormUploadShipment(FrayteCommerceUploadShipmentDraft Shipment)
        {
            FrayteResult FR = new FrayteResult();
            FrayteUploadshipment shipments = new FrayteUploadshipment();
            shipments.BookingApp = Shipment.BookingApp;
            shipments.BookingStatusType = Shipment.BookingStatusType;
            shipments.Service = Shipment.CourierCompany;
            shipments.CourierCompany = Shipment.LogisticCompany != null ? Shipment.LogisticCompany : "";
            shipments.BookingApp = Shipment.BookingApp;
            shipments.ModuleType = "eCommerce";
            shipments.CustomerId = Shipment.CustomerId;
            shipments.DirectShipmentDraftId = Shipment.DirectShipmentDraftId;
            shipments.ShipFrom = new DirectBookingCollection()
            {
                Country = new FrayteCountryCode()
                {
                    Code = Shipment.ShipFrom.Country.Code,
                    Code2 = Shipment.ShipFrom.Country.Code2,
                    CountryId = Shipment.ShipFrom.Country.CountryId,
                    Name = Shipment.ShipFrom.Country.Name
                },

                PostCode = Shipment.ShipFrom.PostCode,
                FirstName = Shipment.ShipFrom.FirstName,
                LastName = Shipment.ShipFrom.LastName,
                CompanyName = Shipment.ShipFrom.CompanyName == null ? "" : Shipment.ShipFrom.CompanyName,
                Address = Shipment.ShipFrom.Address,
                Address2 = Shipment.ShipFrom.Address2,
                City = Shipment.ShipFrom.City,
                Phone = Shipment.ShipFrom.Phone,
                Email = Shipment.ShipFrom.Email
            };

            shipments.ShipTo = new DirectBookingCollection()
            {
                Country = new FrayteCountryCode()
                {
                    Code = Shipment.ShipTo.Country.Code,
                    Code2 = Shipment.ShipTo.Country.Code2,
                    CountryId = Shipment.ShipTo.Country.CountryId,
                    Name = Shipment.ShipTo.Country.Name
                },
                PostCode = Shipment.ShipTo.PostCode,
                FirstName = Shipment.ShipTo.FirstName,
                LastName = Shipment.ShipTo.LastName,
                CompanyName = Shipment.ShipTo.CompanyName == null ? "" : Shipment.ShipTo.CompanyName,
                Address = Shipment.ShipTo.Address,
                Address2 = Shipment.ShipTo.Address2,
                City = Shipment.ShipTo.City,
                Phone = Shipment.ShipTo.Phone,
                Email = Shipment.ShipTo.Email
            };
            shipments.Package = new List<UploadShipmentPackage>();
            foreach (var ship in Shipment.Packages)
            {
                UploadShipmentPackage USP = new UploadShipmentPackage();
                USP.CartoonValue = ship.CartoonValue;
                USP.Content = ship.Content;
                USP.DirectShipmentDetailDraftId = ship.DirectShipmentDetailDraftId;
                USP.Height = ship.Height;
                USP.Weight = ship.Weight;
                USP.Width = ship.Width;
                USP.Value = ship.Value;
                USP.Length = ship.Length;
                shipments.Package.Add(USP);
            }
            //shipments.CourierCompany = Shipment.LogisicServiceDisplay,
            shipments.PackageCalculationType = Shipment.PakageCalculatonType;
            shipments.PayTaxAndDuties = "Receiver";
            shipments.FrayteNumber = Shipment.FrayteNumber;
            shipments.parcelType = Shipment.ParcelType.ParcelType;
            shipments.CurrencyCode = Shipment.Currency.CurrencyCode;
            shipments.ShipmentReference = Shipment.ReferenceDetail.Reference1;
            shipments.ShipmentDescription = Shipment.ReferenceDetail.ContentDescription;
            shipments.TrackingNo = Shipment.TrackingNo != null ? Shipment.TrackingNo : Shipment.TrackingCode;


            var getId = new eCommerceUploadShipmentRepository().SaveOrderNumber(shipments, shipments.TrackingNo, Shipment.CustomerId);



            return getId.Item1;
        }



        public void ErrorLog(List<FrayteUploadshipment> Shipment, string ServiceType)
        {
            //FrayteUploadshipment shipments = new FrayteUploadshipment();

            foreach (var UploadShipment in Shipment)
            {
                UploadShipment.Errors = new List<string>();
                if (!string.IsNullOrEmpty(UploadShipment.ShipFrom.Country.Code))
                {
                    var result = dbContext.Countries.Where(a => a.CountryCode == UploadShipment.ShipFrom.Country.Code || a.CountryCode2 == UploadShipment.ShipFrom.Country.Code || a.CountryName == UploadShipment.ShipFrom.Country.Code).FirstOrDefault();
                    if (result == null)
                    {
                        UploadShipment.Errors.Add("FromCountryCode is wrong please fill correct and upload shipment again");
                    }
                    else
                    {
                        UploadShipment.ShipFrom.Country.Code = result.CountryCode;
                        UploadShipment.ShipFrom.Country.Code2 = result.CountryCode2;
                        UploadShipment.ShipFrom.Country.CountryId = result.CountryId;
                        UploadShipment.ShipFrom.Country.Name = result.CountryName;

                    }
                }
                if (!string.IsNullOrEmpty(UploadShipment.ShipTo.Country.Code))
                {
                    var result = dbContext.Countries.Where(a => a.CountryCode == UploadShipment.ShipTo.Country.Code || a.CountryCode2 == UploadShipment.ShipTo.Country.Code || a.CountryName == UploadShipment.ShipTo.Country.Code).FirstOrDefault();
                    if (result == null)
                    {
                        UploadShipment.Errors.Add("ToCountryCode is wrong please fill correct and upload shipment again");
                    }
                    else
                    {
                        //UploadShipment.ShipTo.Country = new FrayteCountryCode();
                        UploadShipment.ShipTo.Country.Code = result.CountryCode;
                        UploadShipment.ShipTo.Country.Code2 = result.CountryCode2;
                        UploadShipment.ShipTo.Country.CountryId = result.CountryId;
                        UploadShipment.ShipTo.Country.Name = result.CountryName;

                    }
                }

                if (!string.IsNullOrEmpty(UploadShipment.CurrencyCode))
                {
                    var result = dbContext.CurrencyTypes.Where(a => a.CurrencyCode == UploadShipment.CurrencyCode).FirstOrDefault();
                    if (result == null)
                    {
                        UploadShipment.Errors.Add("Currency Code is wrong please fill correct and upload shipment again");
                    }
                }

                if (!string.IsNullOrEmpty(UploadShipment.TrackingNo))
                {
                    var result = dbContext.eCommerceShipments.Where(a => a.TrackingDetail == UploadShipment.TrackingNo).FirstOrDefault();
                    if (result != null)
                    {
                        UploadShipment.Errors.Add("Entered TrackingNo already has been used by other shipment");
                    }
                }
                if (!string.IsNullOrEmpty(UploadShipment.ShipFrom.Email) && !IsValid(UploadShipment.ShipFrom.Email))
                {
                    UploadShipment.Errors.Add("FromEmail is not in correct format please fill in correct format and upload shipment again");
                }
                if (!string.IsNullOrEmpty(UploadShipment.ShipTo.Email) && !IsValid(UploadShipment.ShipTo.Email))
                {
                    UploadShipment.Errors.Add("ToEmail is not in correct format please fill in correct format and upload shipment again");
                }


                if (UploadShipment.ShipFrom.Country.Code == "" || UploadShipment.ShipFrom.Country.Code == null)
                { UploadShipment.Errors.Add("FromCountryCode is empty please fill and upload shipment again"); }


                if ((UploadShipment.ShipFrom.PostCode == "" || UploadShipment.ShipFrom.PostCode == null) || !new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9]+$").IsMatch(UploadShipment.ShipFrom.PostCode) && UploadShipment.ShipFrom.Country.Code != "HKG")
                { UploadShipment.Errors.Add("FromPostCode is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipFrom.FirstName == "" || UploadShipment.ShipFrom.FirstName == null)
                { UploadShipment.Errors.Add("FromContactFirstName is empty please fill and upload shipment again"); }


                if (UploadShipment.ShipFrom.LastName == "" || UploadShipment.ShipFrom.LastName == null)
                { UploadShipment.Errors.Add("FromContactLastName is empty please fill and upload shipment again"); }


                if (UploadShipment.ShipFrom.CompanyName == "" || UploadShipment.ShipFrom.CompanyName == null)
                {
                    UploadShipment.Errors.Add("FromCompanyName is empty please fill and upload shipment again");
                }

                if (UploadShipment.ShipFrom.Address == "" || UploadShipment.ShipFrom.Address == null)
                { UploadShipment.Errors.Add("FromAddress1 is empty please fill and upload shipment again"); }


                if (UploadShipment.ShipFrom.Address2 == "" || UploadShipment.ShipFrom.Address2 == null)
                { UploadShipment.Errors.Add("FromAddress2 is empty please fill and upload shipment again"); }

                //if (UploadShipment.ShipFrom.State == "" || UploadShipment.ShipFrom.State == null)
                //{ UploadShipment.Errors.Add("FromState is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipFrom.City == "" || UploadShipment.ShipFrom.City == null)
                { UploadShipment.Errors.Add("FromCity is empty please fill and upload shipment again"); }

                //if (UploadShipment.ShipFrom.Area == "" || UploadShipment.ShipFrom.Area == null)
                //{ UploadShipment.Errors.Add("FromArea is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipFrom.Phone == "" || UploadShipment.ShipFrom.Phone == null || !new System.Text.RegularExpressions.Regex("^[0-9]+$").IsMatch(UploadShipment.ShipFrom.Phone))
                { UploadShipment.Errors.Add("FromTelephoneNo is empty please fill and upload shipment again"); }


                if (UploadShipment.ShipFrom.Email == "" || UploadShipment.ShipFrom.Email == null)
                { UploadShipment.Errors.Add("FromEmail is empty please fill and upload shipment again"); }


                if (UploadShipment.ShipTo.Country.Code == "" || UploadShipment.ShipTo.Country.Code == null)
                { UploadShipment.Errors.Add("ToCountryCode is empty please fill and upload shipment again"); }


                //if (UploadShipment.ShipTo.PostCode == "" || UploadShipment.ShipTo.PostCode == null)
                //{ UploadShipment.Errors.Add("ToPost/ZipCode is empty please fill and upload shipment again"); }

                if ((UploadShipment.ShipTo.PostCode == "" || UploadShipment.ShipTo.PostCode == null) || !new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9]+$").IsMatch(UploadShipment.ShipTo.PostCode) && UploadShipment.ShipTo.Country.Code != "HKG")
                { UploadShipment.Errors.Add("ToPostCode is empty please fill and upload shipment again"); }


                if (UploadShipment.ShipTo.FirstName == "" || UploadShipment.ShipTo.FirstName == null)
                { UploadShipment.Errors.Add("ToContactFirstName is empty please fill and upload shipment again"); }


                if (UploadShipment.ShipTo.LastName == "" || UploadShipment.ShipTo.LastName == null)
                { UploadShipment.Errors.Add("ToContactLastName is empty please fill and upload shipment again"); }


                if (UploadShipment.ShipTo.CompanyName == "" || UploadShipment.ShipTo.CompanyName == null)
                { UploadShipment.Errors.Add("ToCompanyName is empty please fill and upload shipment again"); }


                if (UploadShipment.ShipTo.Address == "" || UploadShipment.ShipTo.Address == null)
                { UploadShipment.Errors.Add("ToAddress1 is empty please fill and upload shipment again"); }


                if (UploadShipment.ShipTo.Address2 == "" || UploadShipment.ShipTo.Address2 == null)
                { UploadShipment.Errors.Add("ToAddress2 is empty please fill and upload shipment again"); }

                //if (UploadShipment.ShipTo.State == "" || UploadShipment.ShipTo.State == null)
                //{ UploadShipment.Errors.Add("ToState is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipTo.City == "" || UploadShipment.ShipTo.City == null)
                { UploadShipment.Errors.Add("ToCity is empty please fill and upload shipment again"); }

                //if (UploadShipment.ShipTo.Area == "" || UploadShipment.ShipTo.Area == null)
                //{ UploadShipment.Errors.Add("ToArea is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipTo.Phone == "" || UploadShipment.ShipTo.Phone == null || !new System.Text.RegularExpressions.Regex("^[0-9]+$").IsMatch(UploadShipment.ShipTo.Phone))
                { UploadShipment.Errors.Add("ToTelephoneNo is empty please fill and upload shipment again"); }


                if (UploadShipment.ShipTo.Email == "" || UploadShipment.ShipTo.Email == null)
                { UploadShipment.Errors.Add("ToEmail is empty please fill and upload shipment again"); }


                if (UploadShipment.PackageCalculationType == "" || UploadShipment.PackageCalculationType == null || UploadShipment.PackageCalculationType.ToUpper() != "KGTOCMS" && UploadShipment.PackageCalculationType.ToUpper() != "LBTOINCHS")
                { UploadShipment.Errors.Add("PackageCalculationType is empty please fill and upload shipment again"); }
                else if (UploadShipment.PackageCalculationType.ToUpper() == "KGTOCMS")
                {
                    UploadShipment.PackageCalculationType = FraytePakageCalculationType.kgtoCms;
                }
                else if (UploadShipment.PackageCalculationType.ToUpper() == "LBTOINCHS")
                {
                    UploadShipment.PackageCalculationType = FraytePakageCalculationType.LbToInchs;
                }

                UploadShipment.PayTaxAndDuties = "Receiver";


                if (UploadShipment.parcelType == "" || UploadShipment.parcelType == null || UploadShipment.parcelType.ToUpper() != "PARCEL" && UploadShipment.PackageCalculationType.ToUpper() != "LETTER")
                { UploadShipment.Errors.Add("ParcelType is empty please fill and upload shipment again"); }
                else if (UploadShipment.parcelType.ToUpper() == "LETTER")
                {
                    UploadShipment.parcelType = "Letter";
                }
                else if (UploadShipment.parcelType.ToUpper() == "PARCEL")
                {
                    UploadShipment.parcelType = "Parcel";
                }

                if (UploadShipment.CurrencyCode == "" || UploadShipment.CurrencyCode == null)
                { UploadShipment.Errors.Add("Currency is empty please fill and upload shipment again"); }


                if (UploadShipment.ShipmentReference == "" || UploadShipment.ShipmentReference == null)
                { UploadShipment.Errors.Add("ShipmentReference is empty please fill and upload shipment again"); }


                if (UploadShipment.ShipmentDescription == "" || UploadShipment.ShipmentDescription == null)
                { UploadShipment.Errors.Add("ShipmentDescription is empty please fill and upload shipment again"); }

                if (ServiceType == "ECOMMERCE_WS")
                {
                    if (UploadShipment.TrackingNo == "" || UploadShipment.TrackingNo == null)
                    { UploadShipment.Errors.Add("TrackingNo is empty please fill and upload shipment again"); }


                    if (UploadShipment.CourierCompany == "" || UploadShipment.CourierCompany == null)
                    { UploadShipment.Errors.Add("CourierCompany is empty please fill and upload shipment again"); }

                }
                //if (UploadShipment.EstimatedDateofDelivery < new DateTime())
                //{ UploadShipment.Errors.Add("ETADate is empty please fill and upload shipment again"); }


                //if (UploadShipment.EstimatedTimeofDelivery == "" || UploadShipment.EstimatedTimeofDelivery == null)
                //{ UploadShipment.Errors.Add("ETDTime is empty please fill and upload shipment again"); }

                //if (UploadShipment.ServiceCode == "")
                //{
                //    if (UploadShipment.CustomInfo.ContentsType == "" || UploadShipment.CustomInfo.ContentsType == "")
                //    {
                //        UploadShipment.Errors.Add("ContentsType is empty please fill and upload shipment again");
                //    }

                //    if (UploadShipment.CustomInfo.ContentsType == "" || UploadShipment.CustomInfo.ContentsType == "")
                //    {
                //        UploadShipment.Errors.Add("ContentsType is empty please fill and upload shipment again");
                //    }

                //}

                //UploadShipmentList.Add(UploadShipment);
                int i = 0;

                foreach (var Package in UploadShipment.Package)
                {
                    i++;
                    if (Package.CartoonValue == 0)
                    { UploadShipment.Errors.Add("CartonValue is empty in Line no" + i + "please fill and upload shipment again"); }


                    if (Package.Length == 0)
                    { UploadShipment.Errors.Add("Length is empty in Line no" + i + " please fill and upload shipment again"); }


                    if (Package.Width == 0)
                    { UploadShipment.Errors.Add("Width is empty in Line no" + i + " please fill and upload shipment again"); }


                    if (Package.Height == 0)
                    { UploadShipment.Errors.Add("Height is empty in Line no" + i + " please fill and upload shipment again"); }


                    if (Package.Weight == 0)
                    { UploadShipment.Errors.Add("Weight is empty in Line no" + i + " please fill and upload shipment again"); }


                    if (Package.Value == 0)
                    { UploadShipment.Errors.Add("DeclaredValue is empty in Line no" + i + " please fill and upload shipment again"); }


                    if (Package.Content == "" || Package.Content == null)
                    { UploadShipment.Errors.Add("ShipmentContents is empty in Line no" + i + " please fill and upload shipment again"); }
                    i++;
                }
            }
        }
        public bool CheckValidWithoutServiceExcel(System.Data.DataTable table)
        {
            bool valid = true;

            DataColumnCollection columns = table.Columns;

            if (!columns.Contains("FromCountryCode"))
            {
                valid = false;
            }
            if (!columns.Contains("FromPostCode"))
            {
                valid = false;
            }
            if (!columns.Contains("FromContactFirstName"))
            {
                valid = false;
            }
            if (!columns.Contains("FromContactLastName"))
            {
                valid = false;
            }
            if (!columns.Contains("FromCompanyName"))
            {
                valid = false;
            }
            if (!columns.Contains("FromAddress1"))
            {
                valid = false;
            }
            if (!columns.Contains("FromAddress2"))
            {
                valid = false;
            }
            if (!columns.Contains("FromCity"))
            {
                valid = false;
            }
            if (!columns.Contains("FromTelephoneNo"))
            {
                valid = false;
            }
            if (!columns.Contains("FromEmail"))
            {
                valid = false;
            }
            if (!columns.Contains("ToCountryCode"))
            {
                valid = false;
            }
            if (!columns.Contains("ToPostCode"))
            {
                valid = false;
            }
            if (!columns.Contains("ToContactFirstName"))
            {
                valid = false;
            }
            if (!columns.Contains("ToContactLastName"))
            {
                valid = false;
            }
            if (!columns.Contains("ToCompanyName"))
            {
                valid = false;
            }
            if (!columns.Contains("ToAddress1"))
            {
                valid = false;
            }
            if (!columns.Contains("ToAddress2"))
            {
                valid = false;
            }
            if (!columns.Contains("ToCity"))
            {
                valid = false;
            }
            if (!columns.Contains("ToTelephoneNo"))
            {
                valid = false;
            }
            if (!columns.Contains("ToEmail"))
            {
                valid = false;
            }
            if (!columns.Contains("PackageCalculationType"))
            {
                valid = false;
            }
            if (!columns.Contains("parcelType"))
            {
                valid = false;
            }
            if (!columns.Contains("Currency"))
            {
                valid = false;
            }
            if (!columns.Contains("ShipmentReference"))
            {
                valid = false;
            }
            if (!columns.Contains("ShipmentDescription"))
            {
                valid = false;
            }
            if (!columns.Contains("CourierCompany"))
            {
                valid = false;
            }
            if (!columns.Contains("TrackingNo"))
            {
                valid = false;
            }
            //if (!columns.Contains("ETADate"))
            //{
            //    valid = false;
            //}
            //if (!columns.Contains("ETATime"))
            //{
            //    valid = false;
            //}
            //if (!columns.Contains("ETDDate"))
            //{
            //    valid = false;
            //}
            //if (!columns.Contains("ETDTime"))
            //{
            //    valid = false;
            //}
            if (!columns.Contains("CartonValue"))
            {
                valid = false;
            }
            if (!columns.Contains("Length"))
            {
                valid = false;
            }
            if (!columns.Contains("Width"))
            {
                valid = false;
            }
            if (!columns.Contains("Height"))
            {
                valid = false;
            }
            if (!columns.Contains("Weight"))
            {
                valid = false;
            }
            if (!columns.Contains("DeclaredValue"))
            {
                valid = false;
            }
            if (!columns.Contains("ShipmentContents"))
            {
                valid = false;
            }
            return valid;
        }
        public bool CheckValidWithServiceExcel(System.Data.DataTable table)
        {
            bool valid = true;

            DataColumnCollection columns = table.Columns;

            if (!columns.Contains("FromCountryCode"))
            {
                valid = false;
            }
            if (!columns.Contains("FromPostCode"))
            {
                valid = false;
            }
            if (!columns.Contains("FromContactFirstName"))
            {
                valid = false;
            }
            if (!columns.Contains("FromContactLastName"))
            {
                valid = false;
            }
            if (!columns.Contains("FromCompanyName"))
            {
                valid = false;
            }
            if (!columns.Contains("FromAddress1"))
            {
                valid = false;
            }
            if (!columns.Contains("FromAddress2"))
            {
                valid = false;
            }
            //if (!columns.Contains("FromState"))
            //{
            //    valid = false;
            //}
            //if (!columns.Contains("FromArea"))
            //{
            //    valid = false;
            //}
            if (!columns.Contains("FromCity"))
            {
                valid = false;
            }
            if (!columns.Contains("FromTelephoneNo"))
            {
                valid = false;
            }
            if (!columns.Contains("FromEmail"))
            {
                valid = false;
            }
            if (!columns.Contains("ToCountryCode"))
            {
                valid = false;
            }
            if (!columns.Contains("ToPostCode"))
            {
                valid = false;
            }
            if (!columns.Contains("ToContactFirstName"))
            {
                valid = false;
            }
            if (!columns.Contains("ToContactLastName"))
            {
                valid = false;
            }
            if (!columns.Contains("ToCompanyName"))
            {
                valid = false;
            }
            if (!columns.Contains("ToAddress1"))
            {
                valid = false;
            }
            if (!columns.Contains("ToAddress2"))
            {
                valid = false;
            }
            //if (!columns.Contains("ToState"))
            //{
            //    valid = false;
            //}
            //if (!columns.Contains("ToArea"))
            //{
            //    valid = false;
            //}
            if (!columns.Contains("ToCity"))
            {
                valid = false;
            }
            if (!columns.Contains("ToTelephoneNo"))
            {
                valid = false;
            }
            if (!columns.Contains("ToEmail"))
            {
                valid = false;
            }
            //if (!columns.Contains("CourierCompany"))
            //{
            //    valid = false;
            //}
            if (!columns.Contains("PackageCalculationType"))
            {
                valid = false;
            }
            if (!columns.Contains("parcelType"))
            {
                valid = false;
            }
            if (!columns.Contains("Currency"))
            {
                valid = false;
            }
            if (!columns.Contains("ShipmentReference"))
            {
                valid = false;
            }
            if (!columns.Contains("ShipmentDescription"))
            {
                valid = false;
            }
            //if (!columns.Contains("BillingCountry"))
            //{
            //    valid = false;
            //}
            //if (!columns.Contains("TrackingNo"))
            //{
            //    valid = false;
            //}
            //if (!columns.Contains("ETADate"))
            //{
            //    valid = false;
            //}
            //if (!columns.Contains("ETATime"))
            //{
            //    valid = false;
            //}
            //if (!columns.Contains("ETDDate"))
            //{
            //    valid = false;
            //}
            //if (!columns.Contains("ETDTime"))
            //{
            //    valid = false;
            //}
            if (!columns.Contains("CartonValue"))
            {
                valid = false;
            }
            if (!columns.Contains("Length"))
            {
                valid = false;
            }
            if (!columns.Contains("Width"))
            {
                valid = false;
            }
            if (!columns.Contains("Height"))
            {
                valid = false;
            }
            if (!columns.Contains("Weight"))
            {
                valid = false;
            }
            if (!columns.Contains("DeclaredValue"))
            {
                valid = false;
            }
            if (!columns.Contains("ShipmentContents"))
            {
                valid = false;
            }
            return valid;
        }

        public FrayteResult SaveShipment(FrayteUploadshipment shipment, int CustomerId, string ServiceType)
        {


            shipment.CustomerId = CustomerId;
            shipment.ShipFrom.CustomerId = CustomerId;
            shipment.ShipFrom.AddressType = FrayteFromToAddressType.FromAddress;
            SaveeCommerceShipmentAddress(shipment.ShipFrom);

            ////Step 2: Save ShipTo
            shipment.ShipTo.CustomerId = CustomerId;
            shipment.ShipTo.AddressType = FrayteFromToAddressType.ToAddress;
            SaveeCommerceShipmentAddress(shipment.ShipTo);

            //Step 3: Save Direct Shipmnet + Reference Detail
            SaveeCommerceShipmnetDetail(shipment, ServiceType);

            //Step 4: Save Direct Shipment Detail
            SaveeCommerceShipmentDetailPackages(shipment);

            ////Save 5: Save date time when All HSCode are mapped

            SetMappedOnOneCommerce(shipment);

            //TextWriter tw = File.CreateText(@"C:\FMS\ecomm.godemowithus.com\WebApi\abc.txt");
            //tw.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shipment).ToCharArray());
            //tw.Close();

            FrayteResult result = new FrayteResult();
            result.Status = true;

            return result;
        }

        private void SaveeCommerceShipmentAddress(DirectBookingCollection address)
        {

            var Country = dbContext.Countries.Where(a => a.CountryCode == address.Country.Code || a.CountryCode2 == address.Country.Code || a.CountryName == address.Country.Code || a.CountryId == address.Country.CountryId).FirstOrDefault();
            //address.ToCountry.CountryId = Convert.ToInt32(dbContext.Countries.Where(a => a.CountryName == address.ToCountry.Name).FirstOrDefault());


            //if (Country.CountryId > 0)
            //{
            if (address.DirectShipmentAddressId == 0)
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
                dbAddress.Address1 = address.Address != null && address.Address != "" ? UtilityRepository.GetString(address.Address, eCommerceString.AddressStringLength) : "";
                dbAddress.Address2 = address.Address2 != null && address.Address2 != "" ? UtilityRepository.GetString(address.Address2, eCommerceString.AddressStringLength) : "";
                //dbAddress.Area = address.Area;
                dbAddress.City = address.City != null && address.City != "" ? UtilityRepository.GetString(address.City, 20) : "";
                dbAddress.CompanyName = address.CompanyName;
                dbAddress.ContactFirstName = address.FirstName;
                dbAddress.ContactLastName = address.LastName;
                dbAddress.CountryId = Country == null ? 0 : Country.CountryId;
                dbAddress.Email = address.Email;
                dbAddress.PhoneNo = address.Phone != null && address.Phone != "" ? UtilityRepository.GetString(address.Phone, eCommerceString.PhoneStringLength) : "";
                dbAddress.State = address.State != null && address.State != "" ? UtilityRepository.GetString(address.State, 20) : "";
                dbAddress.Zip = address.PostCode;
                dbAddress.IsActive = true;
                dbAddress.TableType = FrayteTableType.AddressBook;
                dbAddress.ModuleType = FrayteShipmentServiceType.DirectBooking;
                dbContext.DirectShipmentAddressDrafts.Add(dbAddress);

                if (dbAddress != null)
                {
                    dbContext.SaveChanges();
                }
                address.DirectShipmentAddressId = dbAddress.DirectShipmentAddressDraftId;
            }
            else
            {
                DirectShipmentAddressDraft dbAddress = dbContext.DirectShipmentAddressDrafts.Find(address.DirectShipmentAddressId);
                if (dbAddress != null)
                {
                    dbAddress.FromAddress = dbAddress.ToAddress == true ? false : true;
                    dbAddress.ToAddress = dbAddress.FromAddress == true ? false : true;
                    dbAddress.CustomerId = address.CustomerId;
                    dbAddress.Address1 = address.Address != null && address.Address != "" ? UtilityRepository.GetString(address.Address, eCommerceString.AddressStringLength) : "";
                    dbAddress.Address2 = address.Address2 != null && address.Address2 != "" ? UtilityRepository.GetString(address.Address2, eCommerceString.AddressStringLength) : "";
                    dbAddress.Area = address.Area != null && address.Area != "" ? UtilityRepository.GetString(address.Area, 100) : "";
                    dbAddress.City = address.City != null && address.City != "" ? UtilityRepository.GetString(address.City, 20) : "";
                    dbAddress.CompanyName = address.CompanyName;
                    dbAddress.ContactFirstName = address.FirstName;
                    dbAddress.ContactLastName = address.LastName;
                    dbAddress.CountryId = address.Country.CountryId;
                    dbAddress.Email = address.Email;
                    dbAddress.PhoneNo = address.Phone != null && address.Phone != "" ? UtilityRepository.GetString(address.Phone, eCommerceString.PhoneStringLength) : "";
                    dbAddress.State = address.State != null && address.State != "" ? UtilityRepository.GetString(address.State, 20) : "";
                    dbAddress.Zip = address.PostCode;
                    dbAddress.IsActive = true;
                    dbAddress.TableType = FrayteTableType.AddressBook;
                }

                if (dbAddress != null)
                {
                    dbContext.Entry(dbAddress).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
                address.DirectShipmentAddressId = dbAddress.DirectShipmentAddressDraftId;
            }

            //}
        }

        public List<Frayte.Services.Models.CountryLogistic> GetServices()
        {
            List<Frayte.Services.Models.CountryLogistic> CLL = new List<Frayte.Services.Models.CountryLogistic>();

            var result = dbContext.CountryLogistics.ToList();
            foreach (var res in result)
            {
                //Mapper.CreateMap<DataAccess.CountryLogistic, Frayte.Services.Models.CountryLogistic>();
                //AutoMapper.Mapper.Map(res, CL);
                Frayte.Services.Models.CountryLogistic CL = new Frayte.Services.Models.CountryLogistic();
                CL.AccountId = res.AccountId;
                CL.AccountNo = Convert.ToInt32(res.AccountNo);
                CL.CountryCode = res.CountryCode;
                CL.CountryId = res.CountryId;
                CL.CountryLogisticId = res.CountryLogisticId;
                CL.Description = res.Description;
                CL.LogisticService = res.LogisticService;
                CL.LogisticServiceDisplay = res.LogisicServiceDisplay;

                CLL.Add(CL);
            }

            return CLL;
        }

        private void SaveeCommerceShipmnetDetail(FrayteUploadshipment eCommerceUploadShipmentBookingDetail, string ServiceType)
        {
            //dbDirectShipment;
            DirectShipmentDraft dbDirectShipment = new DirectShipmentDraft();
            FrayteOperationZone operationZone = UtilityRepository.GetOperationZone();
            eCommerceUploadShipmentBookingDetail.CreatedOn = DateTime.UtcNow;
            if (eCommerceUploadShipmentBookingDetail.DirectShipmentDraftId == 0)
            {


                dbDirectShipment.ShipmentStatusId = 14;
                dbDirectShipment.CurrencyCode = eCommerceUploadShipmentBookingDetail.CurrencyCode;

                //Set Reference Detail
                dbDirectShipment.Reference1 = eCommerceUploadShipmentBookingDetail.ShipmentReference != null && eCommerceUploadShipmentBookingDetail.ShipmentReference != "" ? UtilityRepository.GetString(eCommerceUploadShipmentBookingDetail.ShipmentReference, 11) : "";
                dbDirectShipment.ContentDescription = eCommerceUploadShipmentBookingDetail.ShipmentDescription;
                //dbDirectShipment.SpecialInstruction = eCommerceUploadShipmentBookingDetail.SpecialInstruction;
                //dbDirectShipment.CollectionDate = Convert.ToDateTime(eCommerceUploadShipmentBookingDetail.CollectionDate);
                //dbDirectShipment.CollectionTime = UtilityRepository.GetTimeFromString(eCommerceUploadShipmentBookingDetail.CollectionTime);
                dbDirectShipment.CustomerId = eCommerceUploadShipmentBookingDetail.CustomerId;
                dbDirectShipment.FromAddressId = eCommerceUploadShipmentBookingDetail.ShipFrom.DirectShipmentAddressId;
                dbDirectShipment.ToAddressId = eCommerceUploadShipmentBookingDetail.ShipTo.DirectShipmentAddressId;
                dbDirectShipment.IsPODMailSent = false;
                dbDirectShipment.OpearionZoneId = operationZone.OperationZoneId;
                dbDirectShipment.ParcelType = eCommerceUploadShipmentBookingDetail.parcelType;
                dbDirectShipment.PaymentPartyTaxAndDuties = eCommerceUploadShipmentBookingDetail.PayTaxAndDuties;
                //dbDirectShipment.CreatedBy = eCommerceBookingDetail.CreatedBy;
                dbDirectShipment.CreatedBy = eCommerceUploadShipmentBookingDetail.CustomerId;
                dbDirectShipment.LastUpdated = DateTime.UtcNow;
                dbDirectShipment.PackageCaculatonType = eCommerceUploadShipmentBookingDetail.PackageCalculationType;
                //dbDirectShipment.TaxAndDutiesAcceptedBy = eCommerceBookingDetail.TaxAndDutiesAcceptedBy;
                dbDirectShipment.ModuleType = FrayteShipmentServiceType.DirectBooking;
                //dbDirectShipment.EstimatedTimeofDelivery = UtilityRepository.GetTimeFromDateString(eCommerceUploadShipmentBookingDetail.EstimatedTimeofDelivery);
                //dbDirectShipment.EstimatedDateofDelivery = eCommerceUploadShipmentBookingDetail.EstimatedDateofDelivery;
                //dbDirectShipment.EstimatedTimeofArrival = UtilityRepository.GetTimeFromDateString(eCommerceUploadShipmentBookingDetail.EstimatedTimeofArrival);
                //dbDirectShipment.EstimatedDateofArrival = eCommerceUploadShipmentBookingDetail.EstimatedDateofArrival;
                dbDirectShipment.LogisticServiceType = eCommerceUploadShipmentBookingDetail.CourierCompany;
                dbDirectShipment.BookingApp = ServiceType;
                dbDirectShipment.TrackingDetail = eCommerceUploadShipmentBookingDetail.TrackingNo;
                //string FrayteNo = CommonConversion.GetNewFrayteNumber();
                //if (FrayteNo != null || FrayteNo != "")
                //{
                //    dbDirectShipment.FrayteNumber = FrayteNo;
                //    eCommerceUploadShipmentBookingDetail.FrayteNumber = FrayteNo;
                //}
                dbContext.DirectShipmentDrafts.Add(dbDirectShipment);
                if (dbDirectShipment != null)
                {
                    dbContext.SaveChanges();
                }

                eCommerceUploadShipmentBookingDetail.DirectShipmentDraftId = dbDirectShipment.DirectShipmentDraftId;
            }
            else
            {
                dbDirectShipment = dbContext.DirectShipmentDrafts.Find(eCommerceUploadShipmentBookingDetail.DirectShipmentDraftId);
                if (dbDirectShipment != null)
                {
                    dbDirectShipment.ShipmentStatusId = 14;
                    dbDirectShipment.CurrencyCode = eCommerceUploadShipmentBookingDetail.CurrencyCode;

                    //Set Reference Detail
                    dbDirectShipment.Reference1 = eCommerceUploadShipmentBookingDetail.ShipmentReference;
                    dbDirectShipment.ContentDescription = eCommerceUploadShipmentBookingDetail.ShipmentDescription;
                    //dbDirectShipment.SpecialInstruction = eCommerceUploadShipmentBookingDetail.SpecialInstruction;
                    //dbDirectShipment.CollectionDate = Convert.ToDateTime(eCommerceUploadShipmentBookingDetail.CollectionDate);
                    //dbDirectShipment.CollectionTime = UtilityRepository.GetTimeFromString(eCommerceUploadShipmentBookingDetail.CollectionTime);
                    dbDirectShipment.CustomerId = eCommerceUploadShipmentBookingDetail.CustomerId;
                    dbDirectShipment.FromAddressId = eCommerceUploadShipmentBookingDetail.ShipFrom.DirectShipmentAddressId;
                    dbDirectShipment.ToAddressId = eCommerceUploadShipmentBookingDetail.ShipTo.DirectShipmentAddressId;
                    dbDirectShipment.IsPODMailSent = false;
                    dbDirectShipment.OpearionZoneId = operationZone.OperationZoneId;
                    dbDirectShipment.ParcelType = eCommerceUploadShipmentBookingDetail.parcelType;
                    dbDirectShipment.PaymentPartyTaxAndDuties = eCommerceUploadShipmentBookingDetail.PayTaxAndDuties;
                    //dbDirectShipment.CreatedBy = eCommerceBookingDetail.CreatedBy;
                    dbDirectShipment.CreatedBy = eCommerceUploadShipmentBookingDetail.CustomerId;
                    dbDirectShipment.LastUpdated = DateTime.UtcNow;
                    dbDirectShipment.PackageCaculatonType = eCommerceUploadShipmentBookingDetail.PackageCalculationType;
                    //dbDirectShipment.TaxAndDutiesAcceptedBy = eCommerceBookingDetail.TaxAndDutiesAcceptedBy;
                    dbDirectShipment.ModuleType = FrayteShipmentServiceType.DirectBooking;
                    //dbDirectShipment.EstimatedTimeofDelivery = UtilityRepository.GetTimeFromDateString(eCommerceUploadShipmentBookingDetail.EstimatedTimeofDelivery);
                    //dbDirectShipment.EstimatedDateofDelivery = eCommerceUploadShipmentBookingDetail.EstimatedDateofDelivery;
                    //dbDirectShipment.EstimatedTimeofArrival = UtilityRepository.GetTimeFromDateString(eCommerceUploadShipmentBookingDetail.EstimatedTimeofArrival);
                    //dbDirectShipment.EstimatedDateofArrival = eCommerceUploadShipmentBookingDetail.EstimatedDateofArrival;
                    dbDirectShipment.LogisticServiceType = eCommerceUploadShipmentBookingDetail.CourierCompany;
                    dbDirectShipment.BookingApp = ServiceType;
                    dbDirectShipment.TrackingDetail = eCommerceUploadShipmentBookingDetail.TrackingNo;
                    //string FrayteNo = CommonConversion.GetNewFrayteNumber();
                    //if (FrayteNo != null || FrayteNo != "")
                    //{
                    //    dbDirectShipment.FrayteNumber = FrayteNo;
                    //    eCommerceUploadShipmentBookingDetail.FrayteNumber = FrayteNo;
                    //}
                    dbContext.DirectShipmentDrafts.Add(dbDirectShipment);
                    // dbDirectShipment.FrayteNumber = eCommerceBookingDetail.FrayteNumber;

                }
                if (dbDirectShipment != null)
                {

                    dbContext.Entry(dbDirectShipment).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
                eCommerceUploadShipmentBookingDetail.DirectShipmentDraftId = dbDirectShipment.DirectShipmentDraftId;
            }
        }

        private void SaveeCommerceShipmentDetailPackages(FrayteUploadshipment eCommerceBookingDetail)
        {
            if (eCommerceBookingDetail.Package != null && eCommerceBookingDetail.Package.Count > 0)
            {
                foreach (UploadShipmentPackage package in eCommerceBookingDetail.Package)

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
                            //packageDetail.HSCode = package.HSCode;
                            packageDetail.HSCode = FHSCode != "" ? FHSCode : null;
                            packageDetail.Width = package.Width;
                        }
                        if (packageDetail != null)
                        {
                            dbContext.Entry(packageDetail).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        package.DirectShipmentDetailDraftId = packageDetail.DirectShipmentDetailDraftId;
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
                        if (packageDetail != null)
                        {
                            dbContext.SaveChanges();
                        }

                        package.DirectShipmentDetailDraftId = packageDetail.DirectShipmentDetailDraftId;
                    }
                }
            }
        }
        private void SetMappedOnOneCommerce(FrayteUploadshipment eCommerceBookingDetail)
        {
            var data = dbContext.DirectShipmentDrafts.Find(eCommerceBookingDetail.DirectShipmentDraftId);
            if (data != null)
            {
                data.MappedOn = DateTime.UtcNow;
                dbContext.SaveChanges();
            }
        }

        // Save data to final tabels
        public Tuple<int, string> SaveOrderNumber(FrayteUploadshipment dbDetail, string orderId, int CustomerId)
        {
            int eCommerceShipmentId = 0;
            dbDetail.WareHouseId = 0;
            if (orderId != null || orderId != "")
            {
                foreach (var hsCodedetial in dbDetail.Package)
                {
                    var HsCode = dbContext.HSCodes.Where(a => a.Description.Contains(hsCodedetial.Content)).FirstOrDefault();
                }
                var FromCountry = dbContext.Countries.Where(a => a.CountryCode == dbDetail.ShipFrom.Country.Code || a.CountryCode2 == dbDetail.ShipFrom.Country.Code || a.CountryName == dbDetail.ShipFrom.Country.Code || a.CountryId == dbDetail.ShipFrom.Country.CountryId).FirstOrDefault();

                var ToCountry = dbContext.Countries.Where(a => a.CountryCode == dbDetail.ShipTo.Country.Code || a.CountryCode2 == dbDetail.ShipTo.Country.Code || a.CountryName == dbDetail.ShipTo.Country.Code || a.CountryId == dbDetail.ShipTo.Country.CountryId).FirstOrDefault();
                var warehouse = dbContext.Warehouses.Where(a => a.CountryId == ToCountry.CountryId).FirstOrDefault();
                dbDetail.WareHouseId = warehouse.WarehouseId;

                //Step 1.0 Save Draft Data Into eCommerce Shipment Table
                dbContext.spGet_SaveDraftAsDirectShipment(dbDetail.DirectShipmentDraftId, orderId, "ECOMMERCE", warehouse.WarehouseId, DateTime.UtcNow, (int)FrayteeCommerceShipmentStatus.Current, CustomerId, null, null, null, CommonConversion.GetNewFrayteNumber(), null, null);

                //Step 1.1 Save Tracking No in eCommerce Table
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
                    address.PhoneNo = UtilityRepository.GetString(dbDetail.ShipFrom.Phone, eCommerceString.PhoneStringLength);
                    address.Address1 = UtilityRepository.GetString(dbDetail.ShipFrom.Address, eCommerceString.AddressStringLength);
                    address.Area = dbDetail.ShipFrom.Area;
                    address.Address2 = UtilityRepository.GetString(dbDetail.ShipFrom.Address2, eCommerceString.AddressStringLength);
                    address.City = dbDetail.ShipFrom.City;
                    address.State = dbDetail.ShipFrom.State;
                    address.Zip = dbDetail.ShipFrom.PostCode;
                    address.CountryId = FromCountry.CountryId;
                    address.IsActive = true;
                    address.TableType = FrayteTableType.DirectBooking;
                    dbContext.eCommerceShipmentAddresses.Add(address);
                    if (address != null)
                    {
                        dbContext.SaveChanges();
                    }

                    //Update Direct Shipment From Address Id
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
                    address.PhoneNo = UtilityRepository.GetString(dbDetail.ShipTo.Phone, eCommerceString.PhoneStringLength);
                    address.Address1 = UtilityRepository.GetString(dbDetail.ShipTo.Address, eCommerceString.AddressStringLength);
                    address.Area = dbDetail.ShipTo.Area;
                    address.Address2 = UtilityRepository.GetString(dbDetail.ShipTo.Address2, eCommerceString.AddressStringLength);
                    address.City = dbDetail.ShipTo.City;
                    address.State = dbDetail.ShipTo.State;
                    address.Zip = dbDetail.ShipTo.PostCode;
                    address.CountryId = ToCountry.CountryId;
                    address.IsActive = true;
                    address.TableType = FrayteTableType.DirectBooking;
                    dbContext.eCommerceShipmentAddresses.Add(address);

                    if (address != null)
                    {
                        dbContext.SaveChanges();
                    }

                    //Update Direct Shipment To Address Id
                    if (dbShipment != null)
                    {
                        dbShipment.ToAddressId = address.eCommerceShipmentAddressId;
                        dbContext.SaveChanges();
                    }
                }

                // Step 1.3 : Delete record from DirectShipmentAddressDraft

                var fromResult = dbContext.DirectShipmentAddressDrafts.Find(dbDetail.ShipFrom.DirectShipmentAddressId);
                if (fromResult != null)
                {
                    dbContext.DirectShipmentAddressDrafts.Remove(fromResult);
                    dbContext.SaveChanges();
                }
                var toResult = dbContext.DirectShipmentAddressDrafts.Find(dbDetail.ShipTo.DirectShipmentAddressId);
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
                    dbToAddressBook.Address1 = UtilityRepository.GetString(dbDetail.ShipFrom.Address, eCommerceString.AddressStringLength);
                    dbToAddressBook.Address2 = UtilityRepository.GetString(dbDetail.ShipFrom.Address2, eCommerceString.AddressStringLength);
                    dbToAddressBook.City = dbDetail.ShipFrom.City;
                    dbToAddressBook.State = dbDetail.ShipFrom.State;
                    dbToAddressBook.PhoneNo = UtilityRepository.GetString(dbDetail.ShipFrom.Phone, eCommerceString.PhoneStringLength);
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
                    dbToAddressBook.Address1 = UtilityRepository.GetString(dbDetail.ShipTo.Address, eCommerceString.AddressStringLength);
                    dbToAddressBook.Address2 = UtilityRepository.GetString(dbDetail.ShipTo.Address2, eCommerceString.AddressStringLength);
                    dbToAddressBook.City = dbDetail.ShipTo.City;
                    dbToAddressBook.State = dbDetail.ShipTo.State;
                    dbToAddressBook.PhoneNo = UtilityRepository.GetString(dbDetail.ShipTo.Phone, eCommerceString.PhoneStringLength);
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
            var FN = "";
            if (eCommerceShipmentId > 0)
            {
                var res = dbContext.eCommerceShipments.Where(a => a.eCommerceShipmentId == eCommerceShipmentId).FirstOrDefault();
                FN = res.FrayteNumber;

            }

            return Tuple.Create(eCommerceShipmentId, FN);
        }

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

        public HSCodeMapped ISAllHSCodeMapped(int eCommerceShipmentId)
        {
            HSCodeMapped result = new HSCodeMapped();
            try
            {
                var shipDetail = dbContext.eCommerceShipmentDetails
                                          .Where(p => p.eCommerceShipmentId == eCommerceShipmentId &&
                                          string.IsNullOrEmpty(p.HSCode)).ToList();
                if (shipDetail == null || (shipDetail != null && shipDetail.Count == 0))
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
            if (AppSettings.ShipmentCreatedFrom == "BATCH")
            {
                filePathToSave = AppSettings.eCommerceUploadLabelFolder + shipment.eCommerceShipmentId;
                //filePathToSave = HttpContext.Current.Server.MapPath(filePathToSave + shipment.eCommerceShipmentId);

            }
            else
            {

                filePathToSave = HttpContext.Current.Server.MapPath(filePathToSave + shipment.eCommerceShipmentId);
            }

            if (!System.IO.Directory.Exists(filePathToSave))
                System.IO.Directory.CreateDirectory(filePathToSave);

            if (AppSettings.ShipmentCreatedFrom == "BATCH")
            {
                barcode.Save(AppSettings.eCommerceUploadLabelFolder + shipment.eCommerceShipmentId + "/" + shipment.FrayteNumber + ".Png");

            }
            else
            {
                barcode.Save(System.Web.Hosting.HostingEnvironment.MapPath(AppSettings.eCommerceLabelFolder + shipment.eCommerceShipmentId + "/" + shipment.FrayteNumber + ".Png"));
            }
            //Save  ShipmentBarCode
            shipment.BarCodeNumber = settings.Data;
            dbContext.Entry(shipment).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }
        #endregion

        public FrayteeCommerceUploadShipmentLabelReport GeteCommerceUploadShipmentLabelReportDetail(FrayteeCommerceUploadShipmentLabelReport obj)
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

        public FrayteCommercePackageTrackingDetail SaveEasyPostDetailTrackingDeatil(FrayteUploadshipment directBookingShippingDetail, UploadShipmentPackage package, EasyPost.Shipment Shipment, int eCommerceShipmentId, int increment)
        {
            FrayteCommercePackageTrackingDetail fraytePackageTrackingDetail = new FrayteCommercePackageTrackingDetail();
            eCommerceShipmentEasyPost EasyPostDeatil;

            var shipmentdetail = dbContext.eCommerceShipmentDetails.Where(p => p.eCommerceShipmentId == eCommerceShipmentId).ToList();
            if (shipmentdetail != null && shipmentdetail.Count > 0)
            {
                int ShipmentDetailId = shipmentdetail[increment].eCommerceShipmentDetailId;
                EasyPostDeatil = dbContext.eCommerceShipmentEasyPosts.Where(p => p.ShipmentId == ShipmentDetailId && p.ShipmentServiceType == FrayteShipmentServiceType.eCommerce).FirstOrDefault();//package.DirectShipmentDetailId
                if (EasyPostDeatil != null)
                {
                    EasyPostDeatil.ShipmentId = shipmentdetail[increment].eCommerceShipmentDetailId;
                    EasyPostDeatil.EasyPostShipmentId = Shipment.id;
                    EasyPostDeatil.ShipmentServiceType = FrayteShipmentServiceType.eCommerce;
                    EasyPostDeatil.TrackingCode = Shipment.tracking_code;
                    if (Shipment.tracker != null)
                    {
                        EasyPostDeatil.Carrier = Shipment.tracker.carrier;
                    }
                    if (Shipment.postage_label != null)
                    {
                        EasyPostDeatil.PostageLabel = Shipment.postage_label.label_url;
                    }
                    EasyPostDeatil.BarcodeURL = Shipment.barcode_url;
                    EasyPostDeatil.BatchMessage = Shipment.batch_message;
                    EasyPostDeatil.BatchStatus = Shipment.batch_status;
                    EasyPostDeatil.CreatedAt = Shipment.created_at;
                    EasyPostDeatil.StampURL = Shipment.stamp_url;

                    dbContext.SaveChanges();
                }
                else
                {
                    EasyPostDeatil = new eCommerceShipmentEasyPost();
                    EasyPostDeatil.ShipmentId = shipmentdetail[increment].eCommerceShipmentDetailId;
                    EasyPostDeatil.EasyPostShipmentId = Shipment.id;
                    EasyPostDeatil.ShipmentServiceType = FrayteShipmentServiceType.eCommerce;
                    EasyPostDeatil.TrackingCode = Shipment.tracking_code;
                    if (Shipment.tracker != null)
                    {
                        EasyPostDeatil.Carrier = Shipment.tracker.carrier;
                    }
                    if (Shipment.postage_label != null)
                    {
                        EasyPostDeatil.PostageLabel = Shipment.postage_label.label_url;
                    }
                    EasyPostDeatil.BarcodeURL = Shipment.barcode_url;
                    EasyPostDeatil.BatchMessage = Shipment.batch_message;
                    EasyPostDeatil.BatchStatus = Shipment.batch_status;
                    EasyPostDeatil.CreatedAt = Shipment.created_at;
                    EasyPostDeatil.StampURL = Shipment.stamp_url;
                    dbContext.eCommerceShipmentEasyPosts.Add(EasyPostDeatil);
                    dbContext.SaveChanges();
                }

                //Finally update the label            
                eCommercePackageTrackingDetail packageTrackingDetail = new eCommercePackageTrackingDetail();
                packageTrackingDetail.eCommerceShipmentDetailId = shipmentdetail[increment].eCommerceShipmentDetailId;
                packageTrackingDetail.TrackingNo = Shipment.tracking_code;
                dbContext.eCommercePackageTrackingDetails.Add(packageTrackingDetail);
                dbContext.SaveChanges();

                fraytePackageTrackingDetail.eCommerceShipmentDetailId = shipmentdetail[increment].eCommerceShipmentDetailId;
                fraytePackageTrackingDetail.eCommercePackageTrackingDetailId = packageTrackingDetail.eCommercePackageTrackingDetailId;
                fraytePackageTrackingDetail.TrackingNo = Shipment.tracking_code;
                fraytePackageTrackingDetail.PackageImage = "";
                fraytePackageTrackingDetail.LabelUrl = Shipment.postage_label.label_url;
                fraytePackageTrackingDetail.IsDownloaded = false;
                fraytePackageTrackingDetail.IsPrinted = false;

                // Traking detail in ecommerce object for AWB label 

                package.TrackingNo = fraytePackageTrackingDetail.TrackingNo;


            }
            return fraytePackageTrackingDetail;
        }

        public DataAccess.CountryLogistic GetLogisticService(string LogisticService)
        {
            return dbContext.CountryLogistics.Where(p => p.LogisticService == LogisticService).FirstOrDefault();
        }
        public Country GetCountry(FrayteCountryCode country)
        {
            return dbContext.Countries.Where(p => p.CountryCode == country.Code).FirstOrDefault();
        }
        public bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }



        public FayteeCommerceUploadShipmentExcel UnsuccessfulExcelWrite(List<FrayteUploadshipment> Shipment, string ServiceType)
        {
            FayteeCommerceUploadShipmentExcel FMEx = new FayteeCommerceUploadShipmentExcel();
            //CSV Writesss
            try
            {
                //var ManifestFileName = CreateManifest(ManifestData);
                //File.Create(@"D:\ProjectFrayte\Frayte\Frayte.WebApi\Manifestedshipments\" + CreateManifest(ManifestData) + ".csv");
                //StreamWriter sw = new StreamWriter(@"D:\ProjectFrayte\Frayte\Frayte.WebApi\Manifestedshipments\ManifestTest\" + ManifestFileName + ".csv", false);
                var ShipmentName = "";
                if (ServiceType == "ECOMMERCE_WS")
                {
                    ShipmentName = "Incomplete Shipments Without Courier Service Download";
                }
                else if (ServiceType == "ECOMMERCE_SS")
                {
                    ShipmentName = "Incomplete Shipments With Courier Service Download";
                }
                else if (ServiceType == "DirectBooking_SS")
                {
                    ShipmentName = "Incomplete Shipments With Direct Booking Courier Service Download";
                }

                StreamWriter sw = new StreamWriter(AppSettings.ManifestFolderPath + ShipmentName + ".csv", false);
                if (ServiceType == "ECOMMERCE_WS")
                {

                    Type type = typeof(FrayteUploadShipmentWithOutServiceOnExcel);
                    int count = type.GetProperties().Length;
                    for (var k = 0; k < count; k++)
                    {
                        var pro = typeof(FrayteUploadShipmentWithOutServiceOnExcel).GetProperties();
                        sw.Write(pro[k].Name);
                        if (k < count)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);
                }
                else if (ServiceType == "ECOMMERCE_SS")
                {

                    Type type = typeof(FrayteUploadShipmentWithServiceOnExcel);
                    int count = type.GetProperties().Length;
                    for (var k = 0; k < count; k++)
                    {
                        var pro = typeof(FrayteUploadShipmentWithServiceOnExcel).GetProperties();
                        sw.Write(pro[k].Name);
                        if (k < count)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);
                }
                // Now write all the rows.
                //foreach (var dr in ManifestData)
                //{
                //var p = typeof(FrayteManifestOnExcel).GetProperties();
                for (var l = 0; l < Shipment.Count; l++)
                {
                    sw.Write(Shipment[l].ShipFrom.Country.Code);
                    if (l < Shipment.Count)
                        sw.Write(",");
                    sw.Write(Shipment[l].ShipFrom.PostCode == null ? "" : Shipment[l].ShipFrom.PostCode);
                    if (l < Shipment.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(Shipment[l].ShipFrom.FirstName));
                    if (l < Shipment.Count)
                        sw.Write(",");
                    sw.Write(Shipment[l].ShipFrom.LastName);
                    if (l < Shipment.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(Shipment[l].ShipFrom.CompanyName == null ? "" : Shipment[l].ShipFrom.CompanyName));
                    if (l < Shipment.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(Shipment[l].ShipFrom.Address));
                    if (l < Shipment.Count)
                        sw.Write(",");
                    string shipperAddress2 = ChangeStringCommatoSpace(Shipment[l].ShipFrom.Address2);
                    sw.Write(shipperAddress2);
                    if (l < Shipment.Count)
                        sw.Write(",");
                    //sw.Write(ChangeStringCommatoSpace(Shipment[l].ShipperAddress3));
                    //if (l < Shipment.Count)
                    //    sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(Shipment[l].ShipFrom.City));
                    if (l < Shipment.Count)
                        sw.Write(",");
                    sw.Write(Shipment[l].ShipFrom.Phone);
                    if (l < Shipment.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(Shipment[l].ShipFrom.Email == null ? "" : Shipment[l].ShipFrom.Email));
                    if (l < Shipment.Count)
                        sw.Write(",");
                    sw.Write(Shipment[l].ShipTo.Country.Code);
                    if (l < Shipment.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(Shipment[l].ShipTo.PostCode));
                    if (l < Shipment.Count)
                        sw.Write(",");
                    //sw.Write(Shipment[l].ShipTo.Country.Code);
                    //if (l < Shipment.Count)
                    //    sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(Shipment[l].ShipTo.FirstName));
                    if (l < Shipment.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(Shipment[l].ShipTo.LastName));
                    if (l < Shipment.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(Shipment[l].ShipTo.CompanyName == null ? "" : Shipment[l].ShipTo.CompanyName));
                    if (l <= Shipment.Count)
                        sw.Write(",");
                    //sw.Write(Shipment[l].ConsigneeAddress3);
                    //if (l <= Shipment.Count)
                    //    sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(Shipment[l].ShipTo.Address));
                    if (l <= Shipment.Count)
                        sw.Write(",");
                    sw.Write(Shipment[l].ShipTo.Address2);
                    if (l <= Shipment.Count)
                        sw.Write(",");
                    string state = ChangeStringCommatoSpace(Shipment[l].ShipTo.City);
                    sw.Write(state);
                    if (l <= Shipment.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(Shipment[l].ShipTo.Phone));
                    if (l <= Shipment.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(Shipment[l].ShipTo.Email == null ? "" : Shipment[l].ShipTo.Email));
                    if (l <= Shipment.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(Shipment[l].PackageCalculationType));
                    if (l <= Shipment.Count)
                        sw.Write(",");
                    sw.Write(Shipment[l].parcelType);
                    if (l <= Shipment.Count)
                        sw.Write(",");
                    sw.Write(Shipment[l].CurrencyCode);
                    if (l <= Shipment.Count)
                        sw.Write(",");
                    sw.Write(Shipment[l].ShipmentReference);
                    if (l <= Shipment.Count)
                        sw.Write(",");
                    sw.Write(ChangeStringCommatoSpace(Shipment[l].ShipmentDescription));
                    if (l <= Shipment.Count)
                        sw.Write(",");
                    if (ServiceType == "ECOMMERCE_WS")
                    {
                        sw.Write(ChangeStringCommatoSpace(Shipment[l].TrackingNo == null ? "" : Shipment[l].TrackingNo));
                        if (l <= Shipment.Count)
                            sw.Write(",");
                        sw.Write(ChangeStringCommatoSpace(Shipment[l].CourierCompany == null ? "" : Shipment[l].CourierCompany));
                        if (l <= Shipment.Count)
                            sw.Write(",");
                    }
                    int i = 0;
                    foreach (var package in Shipment[l].Package)
                    {
                        if (i > 0)
                        {
                            sw.Write(sw.NewLine);
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");
                            sw.Write(",");

                            if (ServiceType == "ECOMMERCE_WS")
                            {
                                sw.Write(",");
                                sw.Write(",");
                            }

                        }
                        sw.Write(package.CartoonValue);
                        if (l <= Shipment.Count)
                            sw.Write(",");
                        sw.Write(package.Length);
                        if (l <= Shipment.Count)
                            sw.Write(",");
                        sw.Write(package.Width);
                        if (l <= Shipment.Count)
                            sw.Write(",");
                        sw.Write(package.Height);

                        if (l <= Shipment.Count)
                            sw.Write(",");
                        sw.Write(package.Weight);
                        if (l <= Shipment.Count)
                            sw.Write(",");
                        sw.Write(package.Value);
                        if (l <= Shipment.Count)
                            sw.Write(",");
                        sw.Write(package.Content);
                        i++;
                    }

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

        public List<FrayteUploadshipment> SaveShipmentWithService(string res)
        {
            var ShipmentIdArray = res.Split(',');
            var DirectShipmentDraftIds = ShipmentIdArray[0].Split('|');
            var c = Convert.ToInt64(ShipmentIdArray[1]);
            var b = dbContext.CountryLogistics.Where(aa => aa.CountryLogisticId == c).FirstOrDefault();
            //List<int> DirectShipmentDraftIds = new List<int>();
            List<FrayteUploadshipment> Shipment = new List<FrayteUploadshipment>();

            foreach (var DirectShipmentDraftId in DirectShipmentDraftIds)
            {

                var result = GetShipmentErrors(Convert.ToInt32(DirectShipmentDraftId), "ECOMMERCE_SS");
                result[0].Service = new Models.CountryLogistic()
                {
                    CountryLogisticId = b.CountryLogisticId,
                    AccountId = b.AccountId,
                    AccountNo = Convert.ToInt32(b.AccountNo),
                    CountryId = b.CountryId,
                    CountryCode = b.CountryCode,
                    LogisticService = b.LogisticService,
                    LogisticServiceDisplay = b.LogisicServiceDisplay
                };
                Shipment.Add(result[0]);
                int Id = Convert.ToInt32(DirectShipmentDraftId);

                var DirectShipment = dbContext.DirectShipmentDrafts.Where(a => a.DirectShipmentDraftId == Id).FirstOrDefault();
                if (DirectShipment != null)
                {
                    DirectShipment.IsSelectServiceStatus = true;
                    dbContext.Entry(DirectShipment).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }


            }



            return Shipment;
        }

        public void SelectServiceShipmentStatus(FrayteUploadshipment result)
        {
            var DirectShipment = dbContext.DirectShipmentDrafts.Where(a => a.DirectShipmentDraftId == result.DirectShipmentDraftId).FirstOrDefault();
            if (DirectShipment != null)
            {
                DirectShipment.IsSelectServiceStatus = false;
                dbContext.Entry(DirectShipment).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteShipment(int ShipmentId)
        {
            var result = dbContext.DirectShipmentDrafts.Where(a => a.DirectShipmentDraftId == ShipmentId).FirstOrDefault();
            if (result != null)
            {
                dbContext.DirectShipmentDrafts.Remove(result);
                dbContext.SaveChanges();
            }
        }

        public int SaveBatchProcessProcessedShipment(int count, int custId)
        {
            //var processlist = dbContext.eCommerceUploadShipmentBatchProcesses.ToList();
            //dbContext.eCommerceUploadShipmentBatchProcesses.RemoveRange(processlist);
            //var result = dbContext.eCommerceUploadShipmentBatchProcesses.Where(a => a.eCommerceUploadShipmentId == count).FirstOrDefault();
            eCommerceUploadShipmentBatchProcess USBP = new eCommerceUploadShipmentBatchProcess();

            var result = dbContext.eCommerceUploadShipmentBatchProcesses.Where(a => a.CustomerId == custId).FirstOrDefault();
            if (result != null)
            {
                result.ProcessedShipment = result.ProcessedShipment + count;
                dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            else
            {
                USBP.ProcessedShipment = count;
                USBP.CustomerId = custId;
                dbContext.eCommerceUploadShipmentBatchProcesses.Add(USBP);
                //dbContext.Entry(USBP).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }


            return USBP.eCommerceUploadShipmentId;
        }

        public int SaveBatchProcessUnprocessedShipment(int count, int custId)
        {
            //var processlist = dbContext.eCommerceUploadShipmentBatchProcesses.ToList();
            //dbContext.eCommerceUploadShipmentBatchProcesses.RemoveRange(processlist);
            //var result = dbContext.eCommerceUploadShipmentBatchProcesses.Where(a => a.eCommerceUploadShipmentId == count).FirstOrDefault();
            eCommerceUploadShipmentBatchProcess USBP = new eCommerceUploadShipmentBatchProcess();

            var result = dbContext.eCommerceUploadShipmentBatchProcesses.Where(a => a.CustomerId == custId).FirstOrDefault();
            if (result != null)
            {
                result.UnprocessedShipment = result.UnprocessedShipment + count;
                dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            else
            {
                USBP.UnprocessedShipment = count;
                USBP.CustomerId = custId;
                dbContext.eCommerceUploadShipmentBatchProcesses.Add(USBP);
                dbContext.SaveChanges();
            }


            return USBP.eCommerceUploadShipmentId;
        }

        public int RemoveBatchProcessShipment(int custId, int TotalShipment)
        {
            //var processlist = dbContext.eCommerceUploadShipmentBatchProcesses.ToList();
            //dbContext.eCommerceUploadShipmentBatchProcesses.RemoveRange(processlist);
            //var result = dbContext.eCommerceUploadShipmentBatchProcesses.Where(a => a.eCommerceUploadShipmentId == count).FirstOrDefault();
            eCommerceUploadShipmentBatchProcess USBP = new eCommerceUploadShipmentBatchProcess();

            var result = dbContext.eCommerceUploadShipmentBatchProcesses.Where(a => a.CustomerId == custId).FirstOrDefault();
            if (result != null)
            {
                result.UnprocessedShipment = 0;
                result.ProcessedShipment = 0;
                result.TotalBatchProcess = TotalShipment;
                dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            else
            {
                USBP.UnprocessedShipment = 0;
                USBP.ProcessedShipment = 0;
                USBP.CustomerId = custId;
                USBP.TotalBatchProcess = TotalShipment;
                dbContext.eCommerceUploadShipmentBatchProcesses.Add(USBP);
                dbContext.SaveChanges();
            }


            return USBP.eCommerceUploadShipmentId;
        }

        public FrayteCommerceShipmentDraft eCommerceShipmentDraft(FrayteUploadshipment fs, int CustomerId)
        {
            FrayteCommerceShipmentDraft EPSD = new FrayteCommerceShipmentDraft();
            EPSD.Packages = new List<PackageDraft>();
            //EPSD.Packages = new List<EasyPostPackage>();
            if (fs != null)
            {
                EPSD.CustomerId = CustomerId;
                EPSD.DirectShipmentDraftId = fs.DirectShipmentDraftId;
                EPSD.TrackingCode = fs.TrackingNo;
                EPSD.FrayteNumber = fs.FrayteNumber;
                EPSD.CourierCompany = fs.CourierCompany;
                EPSD.CreatedOn = fs.CreatedOn;
                EPSD.ShipFrom = new eCommerceShipmentAddressDraft()
                {
                    Country = new FrayteCountryCode()
                    {
                        Code = fs.ShipFrom.Country.Code,
                        Code2 = fs.ShipFrom.Country.Code2,
                        CountryId = fs.ShipFrom.Country.CountryId,
                        Name = fs.ShipFrom.Country.Name
                    },
                    PostCode = fs.ShipFrom.PostCode,
                    FirstName = fs.ShipFrom.FirstName,
                    LastName = fs.ShipFrom.LastName,
                    CompanyName = fs.ShipFrom.CompanyName,
                    Address = fs.ShipFrom.Address,
                    Address2 = fs.ShipFrom.Address2,
                    City = fs.ShipFrom.City,
                    Phone = fs.ShipFrom.Phone,
                    Email = fs.ShipFrom.Email,
                    DirectShipmentAddressDraftId = fs.ShipFrom.DirectShipmentAddressId

                };
                EPSD.ShipTo = new eCommerceShipmentAddressDraft()
                {
                    Country = new FrayteCountryCode()
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
                    Email = fs.ShipTo.Email,
                    DirectShipmentAddressDraftId = fs.ShipFrom.DirectShipmentAddressId

                };
                EPSD.PakageCalculatonType = fs.PackageCalculationType;
                EPSD.CustomerId = fs.CustomerId;
                EPSD.PayTaxAndDuties = fs.PayTaxAndDuties;
                EPSD.ParcelType = new FrayteParcelType()
                {
                    ParcelType = fs.parcelType,
                    ParcelDescription = ""
                };
                EPSD.Currency = new CurrencyType()
                {
                    CurrencyCode = fs.CurrencyCode,
                    CurrencyDescription = ""
                };
                EPSD.ReferenceDetail = new ReferenceDetail()
                {
                    Reference1 = fs.ShipmentReference,
                    ContentDescription = fs.ShipmentDescription,

                };

                foreach (var package in fs.Package)
                {
                    var pack = new PackageDraft();
                    pack.CartoonValue = package.CartoonValue;
                    pack.Content = package.Content;
                    pack.Height = package.Height;
                    pack.Length = package.Length;
                    pack.Value = package.Value;
                    pack.Weight = package.Weight;
                    pack.Width = package.Width;
                    pack.TrackingNo = fs.TrackingNo;
                    EPSD.Packages.Add(pack);
                }


            }
            return EPSD;
        }

        public FrayteUploadshipment eCommerceModelChangeFrayteUploadShipment(FrayteCommerceUploadShipmentDraft fs, int CustomerId)
        {
            FrayteUploadshipment EPSD = new FrayteUploadshipment();
            EPSD.Package = new List<UploadShipmentPackage>();
            //EPSD.Packages = new List<EasyPostPackage>();
            if (fs != null)
            {
                EPSD.CustomerId = CustomerId;
                EPSD.DirectShipmentDraftId = fs.DirectShipmentDraftId;
                EPSD.TrackingNo = fs.TrackingCode;
                EPSD.FrayteNumber = fs.FrayteNumber;
                EPSD.CourierCompany = fs.LogisticCompany;
                EPSD.ShipFrom = new DirectBookingCollection()
                {
                    Country = new FrayteCountryCode()
                    {
                        Code = fs.ShipFrom.Country.Code,
                        Code2 = fs.ShipFrom.Country.Code2,
                        CountryId = fs.ShipFrom.Country.CountryId,
                        Name = fs.ShipFrom.Country.Name
                    },
                    PostCode = fs.ShipFrom.PostCode,
                    FirstName = fs.ShipFrom.FirstName,
                    LastName = fs.ShipFrom.LastName,
                    CompanyName = fs.ShipFrom.CompanyName,
                    Address = fs.ShipFrom.Address,
                    Address2 = fs.ShipFrom.Address2,
                    City = fs.ShipFrom.City,
                    Phone = fs.ShipFrom.Phone,
                    Email = fs.ShipFrom.Email,
                    DirectShipmentAddressId = fs.ShipFrom.DirectShipmentAddressDraftId

                };
                EPSD.ShipTo = new DirectBookingCollection()
                {
                    Country = new FrayteCountryCode()
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
                    Email = fs.ShipTo.Email,
                    DirectShipmentAddressId = fs.ShipFrom.DirectShipmentAddressDraftId

                };
                EPSD.PackageCalculationType = fs.PakageCalculatonType;
                EPSD.CustomerId = fs.CustomerId;
                EPSD.PayTaxAndDuties = fs.PayTaxAndDuties;
                EPSD.parcelType = fs.ParcelType.ParcelType;

                EPSD.CurrencyCode = fs.Currency.CurrencyCode;

                EPSD.ShipmentReference = fs.ReferenceDetail.Reference1;
                EPSD.ShipmentDescription = fs.ReferenceDetail.ContentDescription;


                foreach (var package in fs.Packages)
                {
                    var pack = new UploadShipmentPackage();
                    pack.CartoonValue = package.CartoonValue;
                    pack.Content = package.Content;
                    pack.Height = package.Height;
                    pack.Length = package.Length;
                    pack.Value = package.Value;
                    pack.Weight = package.Weight;
                    pack.Width = package.Width;
                    pack.TrackingNo = fs.TrackingCode;
                    EPSD.Package.Add(pack);
                }


            }
            return EPSD;
        }

        public FrayteCommerceShipmentDraft eCommerceChangeModelShipmentDraft(FrayteCommerceUploadShipmentDraft fs, int CustomerId)
        {
            FrayteCommerceShipmentDraft EPSD = new FrayteCommerceShipmentDraft();
            EPSD.Packages = new List<PackageDraft>();
            //EPSD.Packages = new List<EasyPostPackage>();
            if (fs != null)
            {
                EPSD.CustomerId = CustomerId;
                EPSD.DirectShipmentDraftId = fs.DirectShipmentDraftId;
                EPSD.TrackingCode = fs.TrackingNo != null ? fs.TrackingNo : fs.TrackingCode;
                EPSD.FrayteNumber = fs.FrayteNumber;
                EPSD.CourierCompany = fs.CourierCompany != null ? fs.CourierCompany.LogisticService : fs.LogisticCompany;
                EPSD.ShipFrom = new eCommerceShipmentAddressDraft()
                {
                    Country = new FrayteCountryCode()
                    {
                        Code = fs.ShipFrom.Country.Code,
                        Code2 = fs.ShipFrom.Country.Code2,
                        CountryId = fs.ShipFrom.Country.CountryId,
                        Name = fs.ShipFrom.Country.Name
                    },
                    PostCode = fs.ShipFrom.PostCode,
                    FirstName = fs.ShipFrom.FirstName,
                    LastName = fs.ShipFrom.LastName,
                    CompanyName = fs.ShipFrom.CompanyName,
                    Address = fs.ShipFrom.Address,
                    Address2 = fs.ShipFrom.Address2,
                    City = fs.ShipFrom.City,
                    Phone = fs.ShipFrom.Phone,
                    Email = fs.ShipFrom.Email,
                    DirectShipmentAddressDraftId = fs.ShipFrom.DirectShipmentAddressDraftId

                };
                EPSD.ShipTo = new eCommerceShipmentAddressDraft()
                {
                    Country = new FrayteCountryCode()
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
                    Email = fs.ShipTo.Email,
                    DirectShipmentAddressDraftId = fs.ShipFrom.DirectShipmentAddressDraftId

                };
                EPSD.PakageCalculatonType = fs.PakageCalculatonType;
                EPSD.CustomerId = fs.CustomerId;
                EPSD.PayTaxAndDuties = fs.PayTaxAndDuties;
                EPSD.ParcelType = new FrayteParcelType()
                {
                    ParcelType = fs.ParcelType.ParcelType,
                    ParcelDescription = ""
                };
                EPSD.Currency = new CurrencyType()
                {
                    CurrencyCode = fs.Currency.CurrencyCode,
                    CurrencyDescription = ""
                };
                EPSD.ReferenceDetail = new ReferenceDetail()
                {
                    Reference1 = fs.ReferenceDetail.Reference1,
                    ContentDescription = fs.ReferenceDetail.ContentDescription,

                };

                foreach (var package in fs.Packages)
                {
                    var pack = new PackageDraft();
                    pack.CartoonValue = package.CartoonValue;
                    pack.Content = package.Content;
                    pack.Height = package.Height;
                    pack.Length = package.Length;
                    pack.Value = package.Value;
                    pack.Weight = package.Weight;
                    pack.Width = package.Width;
                    pack.TrackingNo = fs.TrackingNo;
                    EPSD.Packages.Add(pack);
                }


            }
            return EPSD;
        }

        public FrayteCommerceUploadShipmentDraft eCommerceUploadShipmentDraft(FrayteUploadshipment fs, int CustomerId)
        {
            FrayteCommerceUploadShipmentDraft EPSD = new FrayteCommerceUploadShipmentDraft();
            EPSD.Packages = new List<PackageDraft>();
            //EPSD.Packages = new List<EasyPostPackage>();
            if (fs != null)
            {

                EPSD.CustomerId = CustomerId;
                EPSD.DirectShipmentDraftId = fs.DirectShipmentDraftId;
                EPSD.TrackingCode = fs.TrackingNo;
                EPSD.FrayteNumber = fs.FrayteNumber;
                EPSD.BookingApp = fs.BookingApp;
                EPSD.BookingStatusType = fs.BookingStatusType;
                var result = dbContext.CountryLogistics.Where(aa => aa.LogisticService.Contains(fs.CourierCompany)).FirstOrDefault();
                if (result != null)
                {
                    EPSD.CourierCompany = new Models.CountryLogistic();
                    EPSD.CourierCompany.Description = result.Description;
                    EPSD.CourierCompany.CountryLogisticId = result.CountryLogisticId;
                    EPSD.CourierCompany.AccountId = result.AccountId;
                    EPSD.CourierCompany.AccountNo = Convert.ToInt32(result.AccountNo);
                    EPSD.CourierCompany.LogisticService = result.LogisticService;
                    EPSD.CourierCompany.LogisticServiceDisplay = result.LogisicServiceDisplay;

                }

                EPSD.ShipFrom = new eCommerceShipmentAddressDraft()
                {
                    Country = new FrayteCountryCode()
                    {
                        Code = fs.ShipFrom.Country.Code,
                        Code2 = fs.ShipFrom.Country.Code2,
                        CountryId = fs.ShipFrom.Country.CountryId,
                        Name = fs.ShipFrom.Country.Name
                    },
                    PostCode = fs.ShipFrom.PostCode,
                    FirstName = fs.ShipFrom.FirstName,
                    LastName = fs.ShipFrom.LastName,
                    CompanyName = fs.ShipFrom.CompanyName,
                    Address = fs.ShipFrom.Address,
                    Address2 = fs.ShipFrom.Address2,
                    City = fs.ShipFrom.City,
                    Phone = fs.ShipFrom.Phone,
                    Email = fs.ShipFrom.Email,
                    DirectShipmentAddressDraftId = fs.ShipFrom.DirectShipmentAddressId

                };
                EPSD.ShipTo = new eCommerceShipmentAddressDraft()
                {
                    Country = new FrayteCountryCode()
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
                    Email = fs.ShipTo.Email,
                    DirectShipmentAddressDraftId = fs.ShipFrom.DirectShipmentAddressId

                };
                EPSD.PakageCalculatonType = fs.PackageCalculationType;
                EPSD.CustomerId = fs.CustomerId;
                EPSD.PayTaxAndDuties = fs.PayTaxAndDuties;
                EPSD.ParcelType = new FrayteParcelType()
                {
                    ParcelType = fs.parcelType,
                    ParcelDescription = ""
                };
                EPSD.Currency = new CurrencyType()
                {
                    CurrencyCode = fs.CurrencyCode,
                    CurrencyDescription = ""
                };
                EPSD.ReferenceDetail = new ReferenceDetail()
                {
                    Reference1 = fs.ShipmentReference,
                    ContentDescription = fs.ShipmentDescription,

                };

                foreach (var package in fs.Package)
                {
                    var pack = new PackageDraft();
                    pack.CartoonValue = package.CartoonValue;
                    pack.Content = package.Content;
                    pack.Height = package.Height;
                    pack.Length = package.Length;
                    pack.Value = package.Value;
                    pack.Weight = package.Weight;
                    pack.Width = package.Width;
                    pack.TrackingNo = fs.TrackingNo;
                    EPSD.Packages.Add(pack);
                }


            }
            return EPSD;
        }

        public int GetOperationZone()
        {
            FrayteOperationZone fz = new FrayteOperationZone();
            string value = AppSettings.OperationZone;
            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
            fz.OperationZoneId = int.Parse(val[0].ToString());
            fz.OperationZoneName = val[1].ToString();
            return fz.OperationZoneId;
        }

    }

}
