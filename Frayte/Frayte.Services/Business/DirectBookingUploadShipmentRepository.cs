using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Models.DHL;
using Frayte.Services.Models.TNT;
using Frayte.Services.Utility;
using Microsoft.Win32;
using Newtonsoft.Json;
using Spire.Barcode;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using XStreamline.Log;
//using static DevExpress.XtraExport.Helpers.TableCellCss;
//using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;

namespace Frayte.Services.Business
{
    public class DirectBookingUploadShipmentRepository
    {

        FrayteEntities dbContext = new FrayteEntities();

        public FratyteError GetFrayteError(int DirectShipmentDraftId)
        {
            FratyteError errorResult = new FratyteError();

            var result = dbContext.DirectShipmentDrafts.FirstOrDefault(t => t.DirectShipmentDraftId == DirectShipmentDraftId);
            if (result != null && ((result.EasyPostErrorObject != null && (result.EasyPostErrorObject.StartsWith("<") || result.EasyPostErrorObject.StartsWith("{")))
                || (result.EasyPostPickUpObject != null && (result.EasyPostPickUpObject.StartsWith("<") || result.EasyPostPickUpObject.StartsWith("{")))))
            {
                if (result.LogisticServiceType == FrayteCourierCompany.UKMail ||
                   result.LogisticServiceType == FrayteCourierCompany.Yodel ||
                   result.LogisticServiceType == FrayteCourierCompany.Hermes)
                {

                    FratyteError Result = new FratyteError();
                    if (!string.IsNullOrWhiteSpace(result.EasyPostPickUpObject))
                    {

                        Result = JsonConvert.DeserializeObject<FratyteError>(result.EasyPostPickUpObject);


                    }
                    if (!string.IsNullOrWhiteSpace(result.EasyPostErrorObject))
                    {
                        Result = JsonConvert.DeserializeObject<FratyteError>(result.EasyPostErrorObject);

                    }
                    errorResult = Result;
                }
                #region TNT  
                else if (result.LogisticServiceType == FrayteCourierCompany.TNT && !string.IsNullOrWhiteSpace(result.EasyPostPickUpObject))
                {

                    FratyteError error = new FratyteError();
                    TNTResponseDto TNTResponse = new TNTResponseDto();
                    error.MiscErrors = new List<FrayteKeyValue>();
                    // _log.Error("enter in else section");
                    FrayteKeyValue er1;

                    // Read error messages from xml  

                    XDocument xml = XDocument.Parse(result.EasyPostPickUpObject);

                    var list1 = (from r in xml.Descendants("parse_error")
                                 select new
                                 {
                                     LineNumber = r.Element("error_line") != null ? r.Element("error_line").Value : "",
                                     Reason = r.Element("error_reason") != null ? r.Element("error_reason").Value : "",
                                     Source = r.Element("error_srcText") != null ? r.Element("error_srcText").Value : ""
                                 }).ToList();

                    // Log to elmah
                    if (list1.Count > 0)
                    {
                        //_log.Error(list1[0].Source);
                        dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(list1);
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));

                        er1 = new FrayteKeyValue();
                        er1.Value = new List<string>();
                        er1.Key = "Miscellaneous";
                        er1.Value.Add("TNT server could not parse the request. Please contact the administrator.");
                    }
                    else
                    {
                        // There are validations errors
                        var list = (from r in xml.Descendants("ERROR")
                                    select new
                                    {
                                        ErrorCode = r.Element("CODE") != null ? r.Element("CODE").Value : "",
                                        Description = r.Element("DESCRIPTION") != null ? r.Element("DESCRIPTION").Value : "",
                                        Source = r.Element("SOURCE") != null ? r.Element("SOURCE").Value : ""
                                    }).ToList();
                        //_log.Error(list[0].Description);
                        // Log to elmah
                        if (list.Count > 0)
                        {
                            dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));
                        }
                        if (list.Count > 0)
                        {

                            //_log.Error(list[0].Description);
                            // Step 1 : Get all address erros
                            var addressErrors = list.Where(p => p.Description.Contains("address") || p.Description.Contains("town")).ToList();

                            er1 = new FrayteKeyValue();
                            if (addressErrors.Count > 0)
                            {
                                er1.Value = new List<string>();
                                er1.Key = "Address";
                                er1.Value = new List<string>();
                                foreach (var data in addressErrors)
                                {
                                    er1.Value.Add(data.Description);
                                }
                                if (er1.Value.Count > 0)
                                    error.MiscErrors.Add(er1);

                                // remove from main list
                                foreach (var data in addressErrors)
                                {
                                    list.Remove(data);
                                }
                            }
                            // Step 2 : Get all address erros
                            var packageErrors = list.Where(p => p.Description.Contains("Length") ||
                            p.Description.Contains("Weight") ||
                            p.Description.Contains("Width") ||
                            p.Description.Contains("Height")).ToList();

                            if (packageErrors.Count > 0)
                            {
                                er1 = new FrayteKeyValue();
                                er1.Key = "Package";
                                er1.Value = new List<string>();
                                foreach (var data in packageErrors)
                                {
                                    er1.Value.Add(data.Description);
                                }
                                if (er1.Value.Count > 0)
                                    error.MiscErrors.Add(er1);
                                // remove from main list
                                foreach (var data in packageErrors)
                                {
                                    list.Remove(data);
                                }
                            }

                            // remaining errors  are off type miscellaneous
                            if (list.Count > 0)
                            {
                                er1 = new FrayteKeyValue();
                                er1.Key = "Miscellaneous";
                                er1.Value = new List<string>();
                                foreach (var data in list)
                                {
                                    er1.Value.Add(data.Description);
                                }
                                if (er1.Value.Count > 0)
                                    error.MiscErrors.Add(er1);
                            }
                        }
                    }
                    error.Status = false;

                    errorResult = error;
                }
                #endregion

                #region UPS
                else if (result.LogisticServiceType == FrayteCourierCompany.UPS && (!string.IsNullOrWhiteSpace(result.EasyPostPickUpObject) || !string.IsNullOrWhiteSpace(result.EasyPostErrorObject)))
                {

                    var upsResult = new UPSShipmentResponseDto();
                    var upsError = new UPSErrorDto();
                    if (!string.IsNullOrWhiteSpace(result.EasyPostPickUpObject))
                    {
                        upsError = JsonConvert.DeserializeObject<UPSErrorDto>(result.EasyPostPickUpObject);
                        var error = upsError.Fault.detail.Errors.ErrorDetail.PrimaryErrorCode.Description;
                        upsResult.Error = new FratyteError();
                        upsResult.Error.Service = new List<string>();
                        upsResult.Error.Service.Add(error);

                        upsResult.Error.Status = false;

                    }
                    if (!string.IsNullOrWhiteSpace(result.EasyPostErrorObject))
                    {
                        upsError = JsonConvert.DeserializeObject<UPSErrorDto>(result.EasyPostPickUpObject);
                        var error = upsError.Fault.detail.Errors.ErrorDetail.PrimaryErrorCode.Description;
                        upsResult.Error = new FratyteError();
                        upsResult.Error.Service = new List<string>();
                        upsResult.Error.Service.Add(error);

                        upsResult.Error.Status = false;
                    }
                    errorResult = upsResult.Error;
                }
                #endregion

                #region DHL
                else if (result.LogisticServiceType == FrayteCourierCompany.DHL || result.LogisticServiceType == FrayteCourierCompany.DHLExpress)
                {
                    if (!string.IsNullOrWhiteSpace(result.EasyPostPickUpObject))
                    {
                        var pickupxml = XDocument.Parse(result.EasyPostPickUpObject);
                        var Error = (from r in pickupxml.Descendants("Condition")
                                     select new FrayteKeyValue
                                     {
                                         Key = r.Element("ConditionCode") != null ? r.Element("ConditionCode").Value : "",
                                         Value = new List<string> { r.Element("ConditionData") != null ? r.Element("ConditionData").Value : "" },
                                     }).ToList();


                        errorResult.MiscErrors = Error;
                    }
                    if (!string.IsNullOrWhiteSpace(result.EasyPostErrorObject))
                    {
                        var shimpmentxml = XDocument.Parse(result.EasyPostErrorObject);
                        var Error = (from r in shimpmentxml.Descendants("Condition")
                                     select new FrayteKeyValue
                                     {
                                         Key = r.Element("ConditionCode") != null ? r.Element("ConditionCode").Value : "",
                                         Value = new List<string> { r.Element("ConditionData") != null ? r.Element("ConditionData").Value : "" },
                                     }).ToList();


                        errorResult.MiscErrors = Error;
                    }

                }
                #endregion 
            }
            else
            {
                errorResult.MiscErrors = new List<FrayteKeyValue>();
                FrayteKeyValue fkv = new FrayteKeyValue();
                errorResult.MiscErrors.Add(fkv);
                errorResult.MiscErrors[0].Value = new List<string>();
                errorResult.MiscErrors.FirstOrDefault().Value.Add(result.EasyPostErrorObject);
            }
            return errorResult;
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

            return result;
        }

        public FrayteResult UpdateServiceCode(FrayteeCommerceUserShipment FU)
        {
            FrayteResult fr = new FrayteResult();
            fr.Status = false;
            var result = dbContext.DirectShipmentDrafts.Where(a => a.DirectShipmentDraftId == FU.ShipmentId).FirstOrDefault();
            if (result != null)
            {
                result.ServiceCode = FU.ServiceCode;
                dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                fr.Status = true;
            }
            return fr;
        }

        public List<SessionModel> GetSessionNameList(int UserId)
        {
            List<SessionModel> SessionList = new List<SessionModel>();
            var res = dbContext.DirectShipmentDrafts.Where(b => b.CustomerId == UserId && b.SessionId != null && b.SessionId != 0 && (b.FromAddressId != null && b.ToAddressId != null)).Select(a => a.SessionId).Distinct().ToList();
            if (res != null && res.Count > 0)
            {
                foreach (var r in res)
                {
                    var result = dbContext.DirectBulkUploadSessions.Where(a => a.SessionId == r.Value).FirstOrDefault();
                    SessionModel SessionData = new SessionModel();
                    SessionData.SessionId = result.SessionId;
                    SessionData.SessionName = result.SessionName;
                    SessionData.CreatedOn = result.CreatedOn;
                    SessionData.PrintedOn = result.PrintedOn;
                    SessionData.SessionStatus = result.SessionStatus;
                    SessionList.Add(SessionData);
                }
            }
            return SessionList;
        }

        public List<SessionModel> GetSessionList(int UserId)
        {
            List<SessionModel> SessionList = new List<SessionModel>();
            var result = dbContext.DirectBulkUploadSessions.Where(a => a.UserId == UserId).ToList();
            if (result != null && result.Count > 0)
            {
                foreach (var res in result)
                {
                    SessionModel SessionData = new SessionModel();
                    SessionData.SessionId = res.SessionId;
                    SessionData.SessionName = res.SessionName;
                    SessionData.CreatedOn = res.CreatedOn;
                    SessionData.PrintedOn = res.PrintedOn;
                    SessionData.SessionStatus = res.SessionStatus;
                    SessionList.Add(SessionData);
                }
                foreach (var session in SessionList)
                {
                    session.TotalShipments = GetShipmentList(session.SessionId).Count;
                }
            }
            return SessionList;
        }

        public List<FrayteUserDirectShipment> GetDraftShipmentList(int SessionId)
        {
            List<FrayteUploadshipment> result = new List<FrayteUploadshipment>();
            if (SessionId > 0)
            {
                result = (from DSD in dbContext.DirectShipmentDrafts
                          let id = DSD.DirectShipmentDraftId
                          join FAD in dbContext.AddressBooks
                          on DSD.FromAddressId equals FAD.AddressBookId
                          join TAD in dbContext.AddressBooks
                          on DSD.ToAddressId equals TAD.AddressBookId
                          join CL in dbContext.CountryLogistics on TAD.CountryId equals CL.CountryId into PS
                          from CL in PS.DefaultIfEmpty()
                          join Usr in dbContext.Users on DSD.CustomerId equals Usr.UserId
                          join Ctry in dbContext.Countries on FAD.CountryId equals Ctry.CountryId into AB
                          from Ctry in AB.DefaultIfEmpty()
                          join Ctr in dbContext.Countries on TAD.CountryId equals Ctr.CountryId into AS
                          from Ctr in AS.DefaultIfEmpty()
                          join CustInfo in dbContext.ShipmentCustomDetailDrafts on DSD.DirectShipmentDraftId equals CustInfo.ShipmentDraftId into VS
                          from CusIn in VS.DefaultIfEmpty()
                          join SS in dbContext.ShipmentStatus on DSD.ShipmentStatusId equals SS.ShipmentStatusId
                          //join DSDD in dbContext.DirectShipmentDetailDrafts on DSD.DirectShipmentDraftId equals DSDD.DirectShipmentDraftId
                          where
                                 DSD.BookingApp == "DirectBooking_SS" && DSD.SessionId == SessionId &&
                                 (DSD.FromAddressId != null || DSD.ToAddressId != null)
                          select new FrayteUploadshipment()
                          {
                              DirectShipmentDraftId = DSD.DirectShipmentDraftId,
                              CustomerId = DSD.CustomerId,
                              IsSelectServiceStatus = DSD.IsSelectServiceStatus,
                              ShipFrom = new DirectBookingCollection()
                              {
                                  Country = new FrayteCountryCode()
                                  {
                                      Code = Ctr == null ? "" : Ctry.CountryCode,
                                      Code2 = Ctr == null ? "" : Ctry.CountryCode2,
                                      CountryId = Ctr == null ? 0 : Ctry.CountryId,
                                      Name = Ctr == null ? "" : Ctry.CountryName
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
                              CustomInfo = new CustomInformation()
                              {
                                  ContentsType = CusIn == null ? "" : CusIn.ContentsType,
                                  ContentsExplanation = CusIn == null ? "" : CusIn.ContentsExplanation,
                                  RestrictionType = CusIn == null ? "" : CusIn.RestrictionType,
                                  RestrictionComments = CusIn == null ? "" : CusIn.RestrictionComments,
                                  NonDeliveryOption = CusIn == null ? "" : CusIn.NonDeliveryOption,
                                  CustomsSigner = CusIn == null ? "" : CusIn.CustomsSigner,
                                  ShipmentCustomDetailId = CusIn == null ? 0 : CusIn.ShipmentCustomDetailDraftId
                              },
                              PackageCalculationType = DSD.PackageCaculatonType,
                              PayTaxAndDuties = DSD.PaymentPartyTaxAndDuties,
                              parcelType = DSD.ParcelType,
                              CurrencyCode = DSD.CurrencyCode,
                              ShipmentReference = DSD.Reference1,
                              ShipmentDescription = DSD.ContentDescription,
                              CollectionDate = DSD.CollectionDate.ToString(),
                              CollectionTime = DSD.CollectionTime.ToString(),
                              EstimatedDateofArrival = DSD.EstimatedDateofArrival,
                              EstimatedTimeofArrival = DSD.EstimatedTimeofArrival.ToString(),
                              EstimatedDateofDelivery = DSD.EstimatedDateofDelivery,
                              EstimatedTimeofDelivery = DSD.EstimatedTimeofDelivery.ToString()
                          }).ToList();
            }

            foreach (var a in result)
            {
                if (!string.IsNullOrEmpty(a.CollectionTime))
                {
                    var res = a.CollectionTime.ToString().Split(':');
                    string ab = res[0] + res[1];
                    if (ab == "0000" || ab == "" || ab == null)
                    {
                        a.CollectionTime = "";
                    }
                }
                else
                {
                    a.CollectionTime = "";
                }

                if (!string.IsNullOrEmpty(a.CollectionDate))
                {
                    if (Convert.ToDateTime(a.CollectionDate) == DateTime.MinValue.AddYears(1800))
                        a.CollectionDate = "";
                }

            }


            ErrorLog(result, "ECOMMERCE_SS");
            //FrayteeCommerceWithServiceShipmentfilter shipmentfilter = new FrayteeCommerceWithServiceShipmentfilter();
            //shipmentfilter.SucessfulShipments = new List<FrayteUserDirectShipment>();
            //shipmentfilter.UnsucessfulShipments = new List<FrayteUserDirectShipment>();
            var Shipments = new List<FrayteUserDirectShipment>();
            foreach (var res in result)
            {
                //if (res.Errors.Count > 0)
                //{

                //    var getShipment = GetUnSuccessfulShipmentsWithService(res.DirectShipmentDraftId);
                //    if (getShipment != null)
                //    {
                //        getShipment.IsSuccessFull = true;
                //        getShipment.IsSelectServiceStatus = res.IsSelectServiceStatus;
                //        shipmentfilter.UnsucessfulShipments.Add(getShipment);
                //    }

                //}
                //else
                //{
                //    var getShipment = GetUnSuccessfulShipmentsWithService(res.DirectShipmentDraftId);
                //    getShipment.IsSuccessFull = false;
                //    getShipment.IsSelectServiceStatus = res.IsSelectServiceStatus;
                //    shipmentfilter.SucessfulShipments.Add(getShipment);
                //}
                if (res.Errors.Count > 0)
                {

                    var getShipment = GetUnSuccessfulShipmentsWithService(res.DirectShipmentDraftId);
                    getShipment.TotalPieces = 0;
                    getShipment.TotalWeight = 0;
                    if (getShipment != null)
                    {
                        getShipment.IsSuccessFull = true;
                        getShipment.IsSelectServiceStatus = res.IsSelectServiceStatus;
                        for (int i = 0; i < res.Package.Count; i++)
                        {
                            getShipment.TotalPieces = getShipment.TotalPieces + res.Package[i].CartoonValue;
                            getShipment.TotalWeight = getShipment.TotalWeight + res.Package[i].Weight * res.Package[i].CartoonValue;
                        }
                        Shipments.Add(getShipment);
                    }

                }
                else
                {
                    var getShipment = GetUnSuccessfulShipmentsWithService(res.DirectShipmentDraftId);
                    getShipment.IsSuccessFull = false;
                    getShipment.IsSelectServiceStatus = res.IsSelectServiceStatus;
                    for (int i = 0; i < res.Package.Count; i++)
                    {
                        getShipment.TotalPieces = getShipment.TotalPieces + res.Package[i].CartoonValue;
                        getShipment.TotalWeight = getShipment.TotalWeight + res.Package[i].Weight * res.Package[i].CartoonValue;
                    }
                    Shipments.Add(getShipment);
                }
            }
            return Shipments;
        }

        public List<FrayteUserDirectShipment> GetShipmentList(int SessionId)
        {
            var result = new List<FrayteUploadshipment>();
            if (SessionId > 0)
            {
                result = (from DSD in dbContext.DirectShipments
                          let id = DSD.DirectShipmentId
                          join FAD in dbContext.DirectShipmentAddresses
                          on DSD.FromAddressId equals FAD.DirectShipmentAddressId
                          join TAD in dbContext.DirectShipmentAddresses
                          on DSD.ToAddressId equals TAD.DirectShipmentAddressId
                          join CL in dbContext.Countries on FAD.CountryId equals CL.CountryId into DSA
                          from CL in DSA.DefaultIfEmpty()
                          join CL1 in dbContext.Countries on TAD.CountryId equals CL1.CountryId into DS
                          from CL1 in DS.DefaultIfEmpty()
                          join Usr in dbContext.Users on DSD.CustomerId equals Usr.UserId
                          join CustInfo in dbContext.ShipmentCustomDetails on DSD.DirectShipmentId equals CustInfo.ShipmentId into VS
                          from CusIn in VS.DefaultIfEmpty()
                          join SS in dbContext.ShipmentStatus on DSD.ShipmentStatusId equals SS.ShipmentStatusId
                          where DSD.SessionId == SessionId
                          select new FrayteUploadshipment()
                          {
                              DirectShipmentDraftId = DSD.DirectShipmentId,
                              CustomerId = DSD.CustomerId,
                              ShipFrom = new DirectBookingCollection()
                              {
                                  Country = new FrayteCountryCode()
                                  {
                                      Code = CL == null ? "" : CL.CountryCode,
                                      Code2 = CL == null ? "" : CL.CountryCode2,
                                      CountryId = CL == null ? 0 : CL.CountryId,
                                      Name = CL == null ? "" : CL.CountryName
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
                                      Code = CL1 == null ? "" : CL1.CountryCode,
                                      Code2 = CL1 == null ? "" : CL1.CountryCode2,
                                      CountryId = CL1 == null ? 0 : CL1.CountryId,
                                      Name = CL1 == null ? "" : CL1.CountryName
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
                              CustomInfo = new CustomInformation()
                              {
                                  ContentsType = CusIn == null ? "" : CusIn.ContentsType,
                                  ContentsExplanation = CusIn == null ? "" : CusIn.ContentsExplanation,
                                  RestrictionType = CusIn == null ? "" : CusIn.RestrictionType,
                                  RestrictionComments = CusIn == null ? "" : CusIn.RestrictionComments,
                                  NonDeliveryOption = CusIn == null ? "" : CusIn.NonDeliveryOption,
                                  CustomsSigner = CusIn == null ? "" : CusIn.CustomsSigner,
                                  ShipmentCustomDetailId = CusIn == null ? 0 : CusIn.ShipmentCustomDetailId
                              },
                              PackageCalculationType = DSD.PackageCaculatonType,
                              PayTaxAndDuties = DSD.PaymentPartyTaxAndDuties,
                              parcelType = DSD.ParcelType,
                              CurrencyCode = DSD.CurrencyCode,
                              ShipmentReference = DSD.Reference1,
                              ShipmentDescription = DSD.ContentDescription,
                              CollectionDate = DSD.CollectionDate.ToString(),
                              CollectionTime = DSD.CollectionTime.ToString(),
                              //EstimatedDateofArrival = DSD.,
                              //EstimatedTimeofArrival = DSD.EstimatedTimeofArrival.ToString(),
                              EstimatedDateofDelivery = DSD.DeliveryDate,
                              EstimatedTimeofDelivery = DSD.DeliveryTime.ToString()
                          }).ToList();
            }

            var Shipments = new List<FrayteUserDirectShipment>();
            foreach (var res in result)
            {
                var getShipment = GetDirectShipments(res.DirectShipmentDraftId);
                Shipments.Add(getShipment);
            }
            return Shipments;
        }

        public FrayteUserDirectShipment GetDirectShipments(int DirectShipmentId)
        {

            //List<FrayteCommerceShipmentDraft> result = dbContext.DirectShipmentDrafts.Where(a => a.BookingApp == "ECOMMERCE_WS").ToList();

            var result = (from DSD in dbContext.DirectShipments
                          join FAD in dbContext.DirectShipmentAddresses
                          on DSD.FromAddressId equals FAD.DirectShipmentAddressId
                          join TAD in dbContext.DirectShipmentAddresses
                          on DSD.ToAddressId equals TAD.DirectShipmentAddressId
                          join CL1 in dbContext.Countries on FAD.CountryId equals CL1.CountryId into DS
                          from CL1 in DS.DefaultIfEmpty()
                          join CL in dbContext.Countries on TAD.CountryId equals CL.CountryId into DSA
                          from CL in DS.DefaultIfEmpty()
                          join Usr in dbContext.Users on DSD.CustomerId equals Usr.UserId
                          join SS in dbContext.ShipmentStatus on DSD.ShipmentStatusId equals SS.ShipmentStatusId
                          join Dtl in dbContext.DirectShipmentDetails on DSD.DirectShipmentId equals Dtl.DirectShipmentId
                          join PTD in dbContext.PackageTrackingDetails on Dtl.DirectShipmentDetailId equals PTD.DirectShipmentDetailId
                          join LSCA in dbContext.LogisticServiceCourierAccounts on DSD.CourierAccountId equals LSCA.LogisticServiceCourierAccountId into LSC
                          from LSCA in LSC.DefaultIfEmpty()
                          join LS in dbContext.LogisticServices on LSCA.LogisticServiceId equals LS.LogisticServiceId into LS1
                          from LS in LS1.DefaultIfEmpty()
                          where DSD.DirectShipmentId == DirectShipmentId
                          select new FrayteUserDirectShipment()
                          {
                              ShipmentId = DSD.DirectShipmentId,
                              ShippedFromCompany = FAD.CompanyName,
                              ShippedToCompany = TAD.CompanyName,
                              FrayteNumber = DSD.FrayteNumber,
                              TrackingNo = PTD.TrackingNo,
                              Customer = Usr.ContactName,
                              Status = SS.StatusName,
                              DisplayStatus = SS.DisplayStatusName,
                              ShippingBy = DSD.LogisticServiceType,
                              LogisticType = LS.LogisticCompanyDisplay + " " + LS.RateType,
                              LogisticTypeDisplay = DSD.LogisticServiceType,
                              SessionId = DSD.SessionId != null && DSD.SessionId != 0 ? (int)DSD.SessionId : 0
                          }).FirstOrDefault();

            return result;

        }

        public FrayteUserDirectShipment GetUnSuccessfulShipmentsWithService(int DirectShipmentDraftId)
        {

            //List<FrayteCommerceShipmentDraft> result = dbContext.DirectShipmentDrafts.Where(a => a.BookingApp == "ECOMMERCE_WS").ToList();

            var result = (from DSD in dbContext.DirectShipmentDrafts
                          join FAD in dbContext.AddressBooks
                          on DSD.FromAddressId equals FAD.AddressBookId
                          join TAD in dbContext.AddressBooks
                          on DSD.ToAddressId equals TAD.AddressBookId
                          join CL1 in dbContext.Countries on FAD.CountryId equals CL1.CountryId into DSA
                          from CL1 in DSA.DefaultIfEmpty()
                          join CL in dbContext.Countries on TAD.CountryId equals CL.CountryId into DS
                          from CL in DS.DefaultIfEmpty()
                          join Usr in dbContext.Users on DSD.CustomerId equals Usr.UserId
                          join SS in dbContext.ShipmentStatus on DSD.ShipmentStatusId equals SS.ShipmentStatusId

                          where DSD.BookingApp == "DirectBooking_SS" && DSD.DirectShipmentDraftId == DirectShipmentDraftId
                          select new FrayteUserDirectShipment()
                          {
                              ShipmentId = DSD.DirectShipmentDraftId,
                              FromAddress = FAD.Address1,
                              ToAddress = TAD.Address1,
                              ServiceCode = DSD.ServiceCode,
                              FromCountry = CL1.CountryName,
                              ToCountry = CL.CountryName,
                              ShippedFromCompany = FAD.CompanyName,
                              ShippedToCompany = TAD.CompanyName,
                              FrayteNumber = DSD.FrayteNumber,
                              Customer = Usr.ContactName,
                              Status = SS.StatusName,
                              DisplayStatus = SS.DisplayStatusName,
                              ShippingBy = DSD.LogisticType,
                              IsEasyPostError = (DSD.EasyPostErrorObject == null || DSD.EasyPostErrorObject == "") && (DSD.EasyPostPickUpObject == null || DSD.EasyPostPickUpObject == "") ? false : true,
                              SessionId = DSD.SessionId != null && DSD.SessionId != 0 ? (int)DSD.SessionId : 0
                          }).FirstOrDefault();

            return result;

        }

        public List<FrayteUserDirectShipment> GetShipmentsFromDraft(int CustomerId)
        {

            List<FrayteUploadshipment> result = (from DSD in dbContext.DirectShipmentDrafts
                                                 let id = DSD.DirectShipmentDraftId
                                                 join FAD in dbContext.AddressBooks
                                                 on DSD.FromAddressId equals FAD.AddressBookId
                                                 join TAD in dbContext.AddressBooks
                                                 on DSD.ToAddressId equals TAD.AddressBookId
                                                 join CL in dbContext.CountryLogistics on TAD.CountryId equals CL.CountryId into PS
                                                 from CL in PS.DefaultIfEmpty()
                                                 join Usr in dbContext.Users on DSD.CustomerId equals Usr.UserId
                                                 join Ctry in dbContext.Countries on FAD.CountryId equals Ctry.CountryId into AB
                                                 from Ctry in AB.DefaultIfEmpty()
                                                 join Ctr in dbContext.Countries on TAD.CountryId equals Ctr.CountryId into AS
                                                 from Ctr in AS.DefaultIfEmpty()
                                                 join CustInfo in dbContext.ShipmentCustomDetailDrafts on DSD.DirectShipmentDraftId equals CustInfo.ShipmentDraftId into VS
                                                 from CusIn in VS.DefaultIfEmpty()
                                                 join SS in dbContext.ShipmentStatus on DSD.ShipmentStatusId equals SS.ShipmentStatusId
                                                 //join DSDD in dbContext.DirectShipmentDetailDrafts on DSD.DirectShipmentDraftId equals DSDD.DirectShipmentDraftId
                                                 where
                                                        DSD.BookingApp == "DirectBooking_SS" && DSD.CustomerId == CustomerId
                                                 select new FrayteUploadshipment()
                                                 {
                                                     DirectShipmentDraftId = DSD.DirectShipmentDraftId,
                                                     CustomerId = DSD.CustomerId,
                                                     IsSelectServiceStatus = DSD.IsSelectServiceStatus,
                                                     ShipFrom = new DirectBookingCollection()
                                                     {
                                                         Country = new FrayteCountryCode()
                                                         {
                                                             Code = Ctr == null ? "" : Ctry.CountryCode,
                                                             Code2 = Ctr == null ? "" : Ctry.CountryCode2,
                                                             CountryId = Ctr == null ? 0 : Ctry.CountryId,
                                                             Name = Ctr == null ? "" : Ctry.CountryName
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
                                                     CustomInfo = new CustomInformation()
                                                     {
                                                         ContentsType = CusIn == null ? "" : CusIn.ContentsType,
                                                         ContentsExplanation = CusIn == null ? "" : CusIn.ContentsExplanation,
                                                         RestrictionType = CusIn == null ? "" : CusIn.RestrictionType,
                                                         RestrictionComments = CusIn == null ? "" : CusIn.RestrictionComments,
                                                         NonDeliveryOption = CusIn == null ? "" : CusIn.NonDeliveryOption,
                                                         CustomsSigner = CusIn == null ? "" : CusIn.CustomsSigner,
                                                         ShipmentCustomDetailId = CusIn == null ? 0 : CusIn.ShipmentCustomDetailDraftId
                                                     },
                                                     PackageCalculationType = DSD.PackageCaculatonType,
                                                     PayTaxAndDuties = DSD.PaymentPartyTaxAndDuties,
                                                     parcelType = DSD.ParcelType,
                                                     CurrencyCode = DSD.CurrencyCode,
                                                     ShipmentReference = DSD.Reference1,
                                                     ShipmentDescription = DSD.ContentDescription,
                                                     CollectionDate = DSD.CollectionDate.ToString(),
                                                     CollectionTime = DSD.CollectionTime.ToString(),
                                                     EstimatedDateofArrival = DSD.EstimatedDateofArrival,
                                                     EstimatedTimeofArrival = DSD.EstimatedTimeofArrival.ToString(),
                                                     EstimatedDateofDelivery = DSD.EstimatedDateofDelivery,
                                                     EstimatedTimeofDelivery = DSD.EstimatedTimeofDelivery.ToString()
                                                 }).ToList();

            foreach (var a in result)
            {
                if (!string.IsNullOrEmpty(a.CollectionTime))
                {
                    var res = a.CollectionTime.ToString().Split(':');
                    string ab = res[0] + res[1];
                    if (ab == "0000" || ab == "" || ab == null)
                    {
                        a.CollectionTime = "";
                    }
                }
                else
                {
                    a.CollectionTime = "";
                }

                if (!string.IsNullOrEmpty(a.CollectionDate))
                {
                    if (Convert.ToDateTime(a.CollectionDate) == DateTime.MinValue.AddYears(1800))
                        a.CollectionDate = "";
                }

            }


            ErrorLog(result, "ECOMMERCE_SS");
            //FrayteeCommerceWithServiceShipmentfilter shipmentfilter = new FrayteeCommerceWithServiceShipmentfilter();
            //shipmentfilter.SucessfulShipments = new List<FrayteUserDirectShipment>();
            //shipmentfilter.UnsucessfulShipments = new List<FrayteUserDirectShipment>();
            var Shipments = new List<FrayteUserDirectShipment>();
            foreach (var res in result)
            {
                //if (res.Errors.Count > 0)
                //{

                //    var getShipment = GetUnSuccessfulShipmentsWithService(res.DirectShipmentDraftId);
                //    if (getShipment != null)
                //    {
                //        getShipment.IsSuccessFull = true;
                //        getShipment.IsSelectServiceStatus = res.IsSelectServiceStatus;
                //        shipmentfilter.UnsucessfulShipments.Add(getShipment);
                //    }

                //}
                //else
                //{
                //    var getShipment = GetUnSuccessfulShipmentsWithService(res.DirectShipmentDraftId);
                //    getShipment.IsSuccessFull = false;
                //    getShipment.IsSelectServiceStatus = res.IsSelectServiceStatus;
                //    shipmentfilter.SucessfulShipments.Add(getShipment);
                //}
                if (res.Errors.Count > 0)
                {

                    var getShipment = GetUnSuccessfulShipmentsWithService(res.DirectShipmentDraftId);
                    if (getShipment != null)
                    {
                        getShipment.IsSuccessFull = true;
                        getShipment.IsSelectServiceStatus = res.IsSelectServiceStatus;
                        Shipments.Add(getShipment);
                    }

                }
                else
                {
                    var getShipment = GetUnSuccessfulShipmentsWithService(res.DirectShipmentDraftId);
                    getShipment.IsSuccessFull = false;
                    getShipment.IsSelectServiceStatus = res.IsSelectServiceStatus;
                    Shipments.Add(getShipment);
                }
            }
            return Shipments;
        }

        public List<LogisticServiceShipment> GetServiceCode()
        {
            List<LogisticServiceShipment> LogisticServiceShipmentList = new List<LogisticServiceShipment>();
            var GetServiceList = dbContext.LogisticServiceShipmentTypes.ToList();
            foreach (var res in GetServiceList)
            {
                var LogisticServiceShipmentModel = new LogisticServiceShipment();
                LogisticServiceShipmentModel.LogisticServiceShipmentTypeId = res.LogisticServiceShipmentTypeId;
                LogisticServiceShipmentModel.LogisticServiceId = res.LogisticServiceId;
                LogisticServiceShipmentModel.ServiceCode = res.LogisticServiceShipmentCode;
                LogisticServiceShipmentList.Add(LogisticServiceShipmentModel);
            }
            return LogisticServiceShipmentList;
        }

        public FrayteResult SaveServiceCode(string ServiceCode, int DirectShipmentDraftId)
        {
            FrayteResult fr = new FrayteResult();
            fr.Status = false;
            var result = dbContext.DirectShipmentDrafts.Where(a => a.DirectShipmentDraftId == DirectShipmentDraftId).FirstOrDefault();
            if (result != null)
            {
                result.ServiceCode = ServiceCode;
                if (result.EasyPostErrorObject != null && result.EasyPostErrorObject != "" && result.EasyPostErrorObject.Contains("This shipment can not create"))
                {
                    result.EasyPostErrorObject = null;
                }
                dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                fr.Status = true;
            }
            return fr;
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
            return result;

        }

        public List<FrayteUploadshipment> GetShipmentErrors(int ShipmentId, string ServiceType)
        {
            List<FrayteUploadshipment> result = (from DSD in dbContext.DirectShipmentDrafts
                                                 let id = DSD.DirectShipmentDraftId
                                                 join FAD in dbContext.AddressBooks
                                                      on DSD.FromAddressId equals FAD.AddressBookId
                                                 join TAD in dbContext.AddressBooks
                                                      on DSD.ToAddressId equals TAD.AddressBookId
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
                                                 join SDD in dbContext.ShipmentCustomDetailDrafts
                                                 on DSD.DirectShipmentDraftId equals SDD.ShipmentDraftId into SDDS
                                                 from SDD in SDDS.DefaultIfEmpty()
                                                     //join DSDD in dbContext.DirectShipmentDetailDrafts on DSD.DirectShipmentDraftId equals DSDD.DirectShipmentDraftId
                                                 where
                                                        DSD.BookingApp == ServiceType && DSD.DirectShipmentDraftId == ShipmentId
                                                 select new FrayteUploadshipment()
                                                 {
                                                     EasyPostError = DSD.EasyPostErrorObject == null || DSD.EasyPostErrorObject == "" ? "" : DSD.EasyPostErrorObject,
                                                     EasyPostPickUpObj = DSD.EasyPostPickUpObject == null || DSD.EasyPostPickUpObject == "" ? "" : DSD.EasyPostPickUpObject,
                                                     DirectShipmentDraftId = DSD.DirectShipmentDraftId,
                                                     SessionId = DSD.SessionId.Value,
                                                     FrayteNumber = DSD.FrayteNumber,
                                                     UserId = DSD.CreatedBy.Value > 0 ? DSD.CreatedBy.Value : 0,

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
                                                         DirectShipmentAddressId = FAD.AddressBookId
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
                                                         DirectShipmentAddressId = TAD.AddressBookId
                                                     },
                                                     Package = (from b in dbContext.DirectShipmentDrafts
                                                                join a in dbContext.DirectShipmentDetailDrafts on b.DirectShipmentDraftId equals a.DirectShipmentDraftId
                                                                where b.DirectShipmentDraftId == ShipmentId
                                                                select new UploadShipmentPackage
                                                                {
                                                                    DirectShipmentDetailDraftId = a.DirectShipmentDetailDraftId,
                                                                    CartoonValue = a.CartoonValue.HasValue ? a.CartoonValue.Value : 0,
                                                                    Length = a.Length.HasValue ? a.Length.Value : 0,
                                                                    Width = a.Width.HasValue ? a.Width.Value : 0,
                                                                    Weight = a.Weight.HasValue ? a.Weight.Value : 0,
                                                                    Height = a.Height.HasValue ? a.Height.Value : 0,
                                                                    Value = a.DeclaredValue.HasValue ? a.DeclaredValue.Value : 0,
                                                                    Content = a.PiecesContent
                                                                }).ToList(),
                                                     CustomInfo = new CustomInformation()
                                                     {
                                                         ContentsExplanation = SDD == null ? "" : SDD.ContentsExplanation,
                                                         ContentsType = SDD == null ? "" : SDD.ContentsType,
                                                         RestrictionType = SDD == null ? "" : SDD.RestrictionType,
                                                         RestrictionComments = SDD == null ? "" : SDD.RestrictionComments,
                                                         CustomsSigner = SDD == null ? "" : SDD.CustomsSigner,
                                                         NonDeliveryOption = SDD == null ? "" : SDD.NonDeliveryOption,
                                                         ShipmentCustomDetailId = SDD == null ? 0 : SDD.ShipmentCustomDetailDraftId
                                                     },
                                                     ServiceCode = DSD.ServiceCode,
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
                                                     CollectionDate = DSD.CollectionDate.ToString(),
                                                     CollectionTime = DSD.CollectionTime.ToString(),
                                                     CourierCompany = DSD.LogisticServiceType,
                                                     //CourierCompanyDisplay = CL.LogisicServiceDisplay,
                                                     ShipmentStatusId = DSD.ShipmentStatusId.Value,
                                                     OpearionZoneId = DSD.OpearionZoneId.Value

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
                        FVObj.FieldLabel = "From PostCode";
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
                    if (columns.Contains("FromEmail is not in correct format"))
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
                        FVObj.GeneralObj = (dbContext.Countries.ToList() as IEnumerable<object>).Cast<object>().ToList();
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("ToPostCode"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "To PostCode";
                        FVObj.FieldName = "PostCode";
                        FVObj.FileType = "text";
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
                    if (columns.Contains("ToEmail is not in correct format"))
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
                    if (columns.Contains("Collection Date"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "Collection Date";
                        FVObj.FieldName = "CollectionDate";
                        FVObj.FileType = "Date";
                        FVObj.LabelName = "collectionDateLable";
                        FVObj.ShowDiv = "MainObjProperty";
                        FVObj.InputTypeName = "collectionDate";
                        FVObj.RequiredMessage = "Collection Date is required.";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("Collection Time"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "Collection Time";
                        FVObj.FieldName = "CollectionTime";
                        FVObj.FileType = "Time";
                        FVObj.LabelName = "collectionTimeLable";
                        FVObj.ShowDiv = "MainObjProperty";
                        FVObj.InputTypeName = "collectionTime";
                        FVObj.RequiredMessage = "Collection Time is required.";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("Contents Type"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "Contents Type";
                        FVObj.FieldName = "ContentsType";
                        FVObj.FileType = "DropDown";
                        FVObj.LabelName = "contentsTypeLable";
                        FVObj.ShowDiv = "CustomInfo";
                        FVObj.InputTypeName = "contentsType";
                        FVObj.RequiredMessage = "Contents Type is required.";
                        //FVObj.DashString = "+ ' - ' +";
                        FVObj.TrackBYOBJ = "ParcelType.Id";
                        FVObj.IterationFor1 = "ParcelType.Name";
                        //FVObj.IterationFor2 = "ParcelType.CurrencyDescription";
                        FVObj.IterationForAs = "ParcelType.Value";
                        FVObj.GeneralObj = (ContentTypeList().ToList() as IEnumerable<object>).Cast<object>().ToList();
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("Contents Explanation"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "Contents Explanation";
                        FVObj.FieldName = "ContentsExplanation";
                        FVObj.FileType = "text";
                        FVObj.LabelName = "contentsExplanationLable";
                        FVObj.ShowDiv = "CustomInfo";
                        FVObj.InputTypeName = "contentsExplanation";
                        FVObj.RequiredMessage = "Contents Explanation is required.";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("Restriction Type"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "Restriction Type";
                        FVObj.FieldName = "RestrictionType";
                        FVObj.FileType = "DropDown";
                        FVObj.LabelName = "restrictionTypeLable";
                        FVObj.ShowDiv = "CustomInfo";
                        FVObj.InputTypeName = "restrictionType";
                        FVObj.RequiredMessage = "Restriction Type is required.";
                        //FVObj.DashString = "+ ' - ' +";
                        FVObj.TrackBYOBJ = "ParcelType.Id";
                        FVObj.IterationFor1 = "ParcelType.Name";
                        //FVObj.IterationFor2 = "ParcelType.CurrencyDescription";
                        FVObj.IterationForAs = "ParcelType.Value";
                        FVObj.GeneralObj = (RestrictionTypeList().ToList() as IEnumerable<object>).Cast<object>().ToList();
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("Restriction Comments"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "Restriction Comments";
                        FVObj.FieldName = "RestrictionComments";
                        FVObj.FileType = "text";
                        FVObj.LabelName = "restrictionCommentsLable";
                        FVObj.ShowDiv = "CustomInfo";
                        FVObj.InputTypeName = "restrictionComments";
                        FVObj.RequiredMessage = "Restriction Comments is required.";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("Customs Signer"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "Customs Signer";
                        FVObj.FieldName = "CustomsSigner";
                        FVObj.FileType = "text";
                        FVObj.LabelName = "customsSignerLable";
                        FVObj.ShowDiv = "CustomInfo";
                        FVObj.InputTypeName = "customsSigner";
                        FVObj.RequiredMessage = "Customs Signer is required.";
                        res.RemainedFields.Add(FVObj);
                    }
                    if (columns.Contains("Non Delivery Option"))
                    {
                        FailedValidationObj FVObj = new FailedValidationObj();
                        FVObj.FieldLabel = "Non Delivery Option";
                        FVObj.FieldName = "NonDeliveryOption";
                        FVObj.FileType = "text";
                        FVObj.LabelName = "nonDeliveryOptionLable";
                        FVObj.ShowDiv = "CustomInfo";
                        FVObj.InputTypeName = "nonDeliveryOption";
                        FVObj.RequiredMessage = "Non Delivery Option is required.";
                        //FVObj.DashString = "+ ' - ' +";
                        FVObj.TrackBYOBJ = "ParcelType.Id";
                        FVObj.IterationFor1 = "ParcelType.Name";
                        //FVObj.IterationFor2 = "ParcelType.CurrencyDescription";
                        FVObj.IterationForAs = "ParcelType.Value";
                        FVObj.GeneralObj = (NonDeliveryOptionList().ToList() as IEnumerable<object>).Cast<object>().ToList();
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
                                           join FAD in dbContext.AddressBooks
                                           on DSD.FromAddressId equals FAD.AddressBookId
                                           join TAD in dbContext.AddressBooks
                                           on DSD.ToAddressId equals TAD.AddressBookId
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
                                               ModuleType = "DirectBooking",
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
                                               CourierCompany = DSD.LogisticServiceType,
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

                if (shipmentdetail["From Country Code"].ToString() == "" && shipmentdetail["To Country Code"].ToString() == "" &&
                    shipmentdetail["From Address1"].ToString() == "" && shipmentdetail["To Address1"].ToString() == "")
                {
                    Package = new UploadShipmentPackage();

                    Package.CartoonValue = Convert.ToInt32(Math.Floor(CommonConversion.ConvertToDecimal(shipmentdetail["Carton Qty"].ToString().Trim() != "" || shipmentdetail["Carton Qty"].ToString().Trim() != null ? shipmentdetail["Carton Qty"].ToString().Trim() : "")));

                    Package.Length = CommonConversion.ConvertToDecimal(shipmentdetail["Length"].ToString().Trim() != "" || shipmentdetail["Length"].ToString().Trim() != null ? shipmentdetail["Length"].ToString().Trim() : "");

                    Package.Width = CommonConversion.ConvertToDecimal(shipmentdetail["Width"].ToString().Trim() != "" || shipmentdetail["Width"].ToString().Trim() != null ? shipmentdetail["Width"].ToString().Trim() : "");

                    Package.Height = CommonConversion.ConvertToDecimal(shipmentdetail["Height"].ToString().Trim() != "" || shipmentdetail["Height"].ToString().Trim() != null ? shipmentdetail["Height"].ToString().Trim() : "");

                    Package.Weight = CommonConversion.ConvertToDecimal(shipmentdetail["Weight"].ToString().Trim() != "" || shipmentdetail["Weight"].ToString().Trim() != null ? shipmentdetail["Weight"].ToString().Trim() : "");

                    Package.Value = CommonConversion.ConvertToDecimal(shipmentdetail["Declared Value"].ToString().Trim() != "" || shipmentdetail["Declared Value"].ToString().Trim() != null ? shipmentdetail["Declared Value"].ToString().Trim() : "");

                    Package.Content = shipmentdetail["Shipment Contents"].ToString().Trim() != "" || shipmentdetail["Shipment Contents"].ToString().Trim() != null ? shipmentdetail["Shipment Contents"].ToString().Trim() : "";

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
                    UploadShipment.CustomInfo = new CustomInformation();

                    UploadShipment.ShipFrom.Country.Code = shipmentdetail["From Country Code"].ToString().Trim() != "" || shipmentdetail["From CountryCode"].ToString().Trim() != null ? shipmentdetail["From Country Code"].ToString().Trim() : "";

                    UploadShipment.ShipFrom.PostCode = shipmentdetail["From Post Code"].ToString().Trim() != "" || shipmentdetail["From Post Code"].ToString().Trim() != null ? shipmentdetail["From Post Code"].ToString().Trim() : "";

                    UploadShipment.ShipFrom.FirstName = shipmentdetail["From Contact First Name"].ToString().Trim() != "" || shipmentdetail["From Contact First Name"].ToString().Trim() != null ? shipmentdetail["From Contact First Name"].ToString().Trim() : "";

                    UploadShipment.ShipFrom.LastName = shipmentdetail["From Contact Last Name"].ToString().Trim() != "" || shipmentdetail["From Contact Last Name"].ToString().Trim() != null ? shipmentdetail["From Contact Last Name"].ToString().Trim() : "";

                    UploadShipment.ShipFrom.CompanyName = shipmentdetail["From Company Name"].ToString().Trim() != "" || shipmentdetail["From Company Name"].ToString().Trim() != null ? shipmentdetail["From Company Name"].ToString().Trim() : "";

                    UploadShipment.ShipFrom.Address = shipmentdetail["From Address1"].ToString().Trim() != "" || shipmentdetail["From Address1"].ToString().Trim() != null ? shipmentdetail["From Address1"].ToString().Trim() : "";

                    UploadShipment.ShipFrom.Address2 = shipmentdetail["From Address2"].ToString().Trim() != "" || shipmentdetail["From Address2"].ToString().Trim() != null ? shipmentdetail["From Address2"].ToString().Trim() : "";

                    UploadShipment.ShipFrom.State = shipmentdetail["From State"].ToString().Trim() != "" || shipmentdetail["From State"].ToString().Trim() != null ? shipmentdetail["From State"].ToString().Trim() : "";
                    UploadShipment.ShipFrom.City = shipmentdetail["From City"].ToString().Trim() != "" || shipmentdetail["From City"].ToString().Trim() != null ? shipmentdetail["From City"].ToString().Trim() : "";

                    UploadShipment.ShipFrom.Phone = shipmentdetail["From TelephoneNo"].ToString().Trim() != "" || shipmentdetail["From TelephoneNo"].ToString().Trim() != null ? shipmentdetail["From TelephoneNo"].ToString().Trim() : "";

                    UploadShipment.ShipFrom.Email = shipmentdetail["From Email"].ToString().Trim() != "" || shipmentdetail["From Email"].ToString().Trim() != null ? shipmentdetail["From Email"].ToString().Trim() : "";

                    UploadShipment.ShipTo.Country.Code = shipmentdetail["To Country Code"].ToString().Trim() != "" || shipmentdetail["To Country Code"].ToString().Trim() != null ? shipmentdetail["To Country Code"].ToString().Trim() : "";

                    UploadShipment.ShipTo.PostCode = shipmentdetail["To Post Code"].ToString().Trim() != "" || shipmentdetail["To Post Code"].ToString().Trim() != null ? shipmentdetail["To Post Code"].ToString().Trim() : "";

                    UploadShipment.ShipTo.FirstName = shipmentdetail["To Contact First Name"].ToString().Trim() != "" || shipmentdetail["To Contact First Name"].ToString().Trim() != null ? shipmentdetail["To Contact First Name"].ToString().Trim() : "";

                    UploadShipment.ShipTo.LastName = shipmentdetail["To Contact Last Name"].ToString().Trim() != "" || shipmentdetail["To Contact Last Name"].ToString().Trim() != null ? shipmentdetail["To Contact Last Name"].ToString().Trim() : "";

                    UploadShipment.ShipTo.CompanyName = shipmentdetail["To Company Name"].ToString().Trim() != "" || shipmentdetail["To Company Name"].ToString().Trim() != null ? shipmentdetail["To Company Name"].ToString().Trim() : "";

                    UploadShipment.ShipTo.Address = shipmentdetail["To Address1"].ToString().Trim() != "" || shipmentdetail["To Address1"].ToString().Trim() != null ? shipmentdetail["To Address1"].ToString().Trim() : "";

                    UploadShipment.ShipTo.Address2 = shipmentdetail["To Address2"].ToString().Trim() != "" || shipmentdetail["To Address2"].ToString().Trim() != null ? shipmentdetail["To Address2"].ToString().Trim() : "";

                    UploadShipment.ShipTo.State = shipmentdetail["To State"].ToString().Trim() != "" || shipmentdetail["To State"].ToString().Trim() != null ? shipmentdetail["To State"].ToString().Trim() : "";
                    UploadShipment.ShipTo.City = shipmentdetail["To City"].ToString().Trim() != "" || shipmentdetail["To City"].ToString().Trim() != null ? shipmentdetail["To City"].ToString().Trim() : "";

                    UploadShipment.ShipTo.Phone = shipmentdetail["To TelephoneNo"].ToString().Trim() != "" || shipmentdetail["To TelephoneNo"].ToString().Trim() != null ? shipmentdetail["To TelephoneNo"].ToString().Trim() : "";

                    UploadShipment.ShipTo.Email = shipmentdetail["To Email"].ToString().Trim() != "" || shipmentdetail["To Email"].ToString().Trim() != null ? shipmentdetail["To Email"].ToString().Trim() : "";

                    UploadShipment.PackageCalculationType = shipmentdetail["Package Calculation Type"].ToString().Trim() != "" || shipmentdetail["Package Calculation Type"].ToString().Trim() != null ? shipmentdetail["Package Calculation Type"].ToString().Trim() : "";

                    UploadShipment.PayTaxAndDuties = "Receiver";
                    UploadShipment.parcelType = shipmentdetail["Parcel Type"].ToString().Trim() != "" || shipmentdetail["Parcel Type"].ToString().Trim() != null ? shipmentdetail["Parcel Type"].ToString().Trim() : null;

                    UploadShipment.CurrencyCode = shipmentdetail["Currency"].ToString().Trim() != "" || shipmentdetail["Currency"].ToString().Trim() != null ? shipmentdetail["Currency"].ToString().Trim() : "";

                    UploadShipment.ShipmentReference = shipmentdetail["Shipment Reference"].ToString().Trim() != "" || shipmentdetail["Shipment Reference"].ToString().Trim() != null ? shipmentdetail["Shipment Reference"].ToString().Trim() : "";

                    UploadShipment.ShipmentDescription = shipmentdetail["Shipment Description"].ToString().Trim() != "" || shipmentdetail["Shipment Description"].ToString().Trim() != null ? shipmentdetail["Shipment Description"].ToString().Trim() : null;

                    if (ServiceType == "ECOMMERCE_WS")
                    {
                        UploadShipment.TrackingNo = shipmentdetail["TrackingNo"].ToString().Trim() != "" || shipmentdetail["TrackingNo"].ToString().Trim() != null ? shipmentdetail["TrackingNo"].ToString().Trim() : "";
                        UploadShipment.CourierCompany = shipmentdetail["CourierCompany"].ToString().Trim() != "" || shipmentdetail["CourierCompany"].ToString().Trim() != null ? shipmentdetail["CourierCompany"].ToString().Trim() : "";

                    }
                    UploadShipment.CourierCompany = LogisticService.ToString().Trim() != "" || LogisticService.ToString().Trim() != null ? LogisticService.ToString().Trim() : "";
                    UploadShipment.ServiceCode = shipmentdetail["Service Code"].ToString().Trim() != "" || shipmentdetail["Service Code"].ToString().Trim() != null ? shipmentdetail["Service Code"].ToString().Trim() : "";
                    UploadShipment.CustomInfo.ContentsType = shipmentdetail["Contents Type"].ToString().Trim() != "" || shipmentdetail["Contents Type"].ToString().Trim() != null ? shipmentdetail["Contents Type"].ToString().Trim() : "";
                    UploadShipment.CustomInfo.RestrictionType = shipmentdetail["Restriction Type"].ToString().Trim() != "" || shipmentdetail["Restriction Type"].ToString().Trim() != null ? shipmentdetail["Restriction Type"].ToString().Trim() : "";
                    UploadShipment.CustomInfo.ContentsExplanation = shipmentdetail["Contents Explanation"].ToString().Trim() != "" || shipmentdetail["Contents Explanation"].ToString().Trim() != null ? shipmentdetail["Contents Explanation"].ToString().Trim() : "";
                    UploadShipment.CustomInfo.RestrictionComments = shipmentdetail["Restriction Comments"].ToString().Trim() != "" || shipmentdetail["Restriction Comments"].ToString().Trim() != null ? shipmentdetail["Restriction Comments"].ToString().Trim() : "";
                    UploadShipment.CustomInfo.CustomsSigner = shipmentdetail["Customs Signature"].ToString().Trim() != "" || shipmentdetail["Customs Signature"].ToString().Trim() != null ? shipmentdetail["Customs Signature"].ToString().Trim() : "";
                    UploadShipment.CustomInfo.NonDeliveryOption = shipmentdetail["Non Delivery Option"].ToString().Trim() != "" || shipmentdetail["Non Delivery Option"].ToString().Trim() != null ? shipmentdetail["Non Delivery Option"].ToString().Trim() : "";

                    UploadShipment.CollectionDate = shipmentdetail["Collection Date"].ToString().Trim() != "" || shipmentdetail["Collection Date"].ToString().Trim() != null ? shipmentdetail["Collection Date"].ToString().Trim() : "";
                    UploadShipment.CollectionTime = shipmentdetail["Collection Time"].ToString().Trim() != "" || shipmentdetail["Collection Time"].ToString().Trim() != null ? shipmentdetail["Collection Time"].ToString().Trim() : "";
                    //UploadShipment.CollectionDateUI =  Convert.ToDateTime(shipmentdetail["CollectionDate"]) != null ? Convert.ToDateTime(shipmentdetail["CollectionDate"]) : DateTime.MinValue;
                    //UploadShipment.CollectionTimeUI = (TimeSpan)shipmentdetail["CollectionTime"] != null ? (TimeSpan)shipmentdetail["CollectionTime"] : TimeSpan.Zero;
                    //UploadShipment.CustomInfo.CatagoryOfItem = shipmentdetail["CatagoryOfItem"].ToString().Trim() != "" || shipmentdetail["CatagoryOfItem"].ToString() != null ? shipmentdetail["CatagoryOfItem"].ToString() : "";

                    //UploadShipment.EstimatedDateofArrival = Convert.ToDateTime(shipmentdetail["ETADate"] != null ? shipmentdetail["ETADate"].ToString() : "");

                    //UploadShipment.EstimatedTimeofArrival = shipmentdetail["ETATime"].ToString() != "" || shipmentdetail["ETATime"].ToString() != null ? shipmentdetail["ETATime"].ToString() : "";

                    //UploadShipment.EstimatedDateofDelivery = Convert.ToDateTime(shipmentdetail["ETADate"] != null ? shipmentdetail["ETADate"].ToString() : "");

                    //UploadShipment.EstimatedTimeofDelivery = shipmentdetail["ETDTime"].ToString() != "" || shipmentdetail["ETDTime"].ToString() != null ? shipmentdetail["ETDTime"].ToString() : "";

                    //UploadShipmentList.Add(UploadShipment);
                    //var CV = Math.Floor(CommonConversion.ConvertToDecimal(shipmentdetail["Carton Qty"].ToString().Trim()));
                    //Package.CartoonValue = Convert.ToInt32(CV);
                    Package.CartoonValue = Convert.ToInt32(Math.Floor(CommonConversion.ConvertToDecimal(shipmentdetail["Carton Qty"].ToString().Trim() != "" || shipmentdetail["Carton Qty"].ToString().Trim() != null ? shipmentdetail["Carton Qty"].ToString().Trim() : "")));

                    Package.Length = CommonConversion.ConvertToDecimal(shipmentdetail["Length"].ToString().Trim() != "" || shipmentdetail["Length"].ToString().Trim() != null ? shipmentdetail["Length"].ToString().Trim() : "");

                    Package.Width = CommonConversion.ConvertToDecimal(shipmentdetail["Width"].ToString().Trim() != "" || shipmentdetail["Width"].ToString().Trim() != null ? shipmentdetail["Width"].ToString().Trim() : "");

                    Package.Height = CommonConversion.ConvertToDecimal(shipmentdetail["Height"].ToString().Trim() != "" || shipmentdetail["Height"].ToString().Trim() != null ? shipmentdetail["Height"].ToString().Trim() : "");

                    Package.Weight = CommonConversion.ConvertToDecimal(shipmentdetail["Weight"].ToString().Trim() != "" || shipmentdetail["Weight"].ToString().Trim() != null ? shipmentdetail["Weight"].ToString().Trim() : "");

                    Package.Value = CommonConversion.ConvertToDecimal(shipmentdetail["Declared Value"].ToString().Trim() != "" || shipmentdetail["Declared Value"].ToString().Trim() != null ? shipmentdetail["Declared Value"].ToString().Trim() : "");

                    Package.Content = shipmentdetail["Shipment Contents"].ToString().Trim() != "" || shipmentdetail["Shipment Contents"].ToString().Trim() != null ? shipmentdetail["Shipment Contents"].ToString().Trim() : "";

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
            foreach (var UploadShipment in Shipment)
            {
                UploadShipment.Errors = new List<string>();
                if (UploadShipment.ShipFrom.Country != null && !string.IsNullOrEmpty(UploadShipment.ShipFrom.Country.Code))
                {
                    var result = dbContext.Countries.Where(a => a.CountryCode == UploadShipment.ShipFrom.Country.Code || a.CountryCode2 == UploadShipment.ShipFrom.Country.Code || a.CountryName == UploadShipment.ShipFrom.Country.Code).FirstOrDefault();
                    if (result == null)
                    {
                        UploadShipment.Errors.Add("From Country Code is wrong please fill correct and upload shipment again");
                    }
                    else
                    {
                        UploadShipment.ShipFrom.Country.Code = result.CountryCode;
                        UploadShipment.ShipFrom.Country.Code2 = result.CountryCode2;
                        UploadShipment.ShipFrom.Country.CountryId = result.CountryId;
                        UploadShipment.ShipFrom.Country.Name = result.CountryName;
                    }
                }
                if (UploadShipment.ShipTo.Country != null && !string.IsNullOrEmpty(UploadShipment.ShipTo.Country.Code))
                {
                    var result = dbContext.Countries.Where(a => a.CountryCode == UploadShipment.ShipTo.Country.Code || a.CountryCode2 == UploadShipment.ShipTo.Country.Code || a.CountryName == UploadShipment.ShipTo.Country.Code).FirstOrDefault();
                    if (result == null)
                    {
                        UploadShipment.Errors.Add("To Country Code is wrong please fill correct and upload shipment again");
                    }
                    else
                    {
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
                { UploadShipment.Errors.Add("From Email is not in correct format please fill in correct format and upload shipment again"); }

                if (!string.IsNullOrEmpty(UploadShipment.ShipTo.Email) && !IsValid(UploadShipment.ShipTo.Email))
                { UploadShipment.Errors.Add("To Email is not in correct format please fill in correct format and upload shipment again"); }

                if (UploadShipment.ShipFrom.Country != null && (UploadShipment.ShipFrom.Country.Code == "" || UploadShipment.ShipFrom.Country.Code == null))
                { UploadShipment.Errors.Add("From Country Code is empty please fill and upload shipment again"); }

                if ((UploadShipment.ShipFrom.PostCode == "" || UploadShipment.ShipFrom.PostCode == null || (UploadShipment.ShipFrom.PostCode != null && !new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9_ ]*$").IsMatch(UploadShipment.ShipFrom.PostCode)) && UploadShipment.ShipFrom.Country.Code != "HKG"))
                { UploadShipment.Errors.Add("From Post Code is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipFrom.CompanyName == UploadShipment.ShipFrom.FirstName + " " + UploadShipment.ShipFrom.LastName)
                { UploadShipment.Errors.Add("From CompanyName, From Contact First Name and Last Name never same"); }

                if (UploadShipment.ShipFrom.FirstName == "" || UploadShipment.ShipFrom.FirstName == null)
                { UploadShipment.Errors.Add("From Contact First Name is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipFrom.LastName == "" || UploadShipment.ShipFrom.LastName == null)
                { UploadShipment.Errors.Add("From Contact Last Name is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipFrom.CompanyName == "" || UploadShipment.ShipFrom.CompanyName == null)
                { UploadShipment.Errors.Add("From Company Name is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipFrom.Address == "" || UploadShipment.ShipFrom.Address == null)
                { UploadShipment.Errors.Add("From Address1 is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipFrom.City == "" || UploadShipment.ShipFrom.City == null)
                { UploadShipment.Errors.Add("From City is empty please fill and upload shipment again"); }

                if (string.IsNullOrWhiteSpace(UploadShipment.ShipFrom.State))
                {
                    if (UploadShipment.ShipFrom.Country.Code2 == "HK")
                    {

                    }
                    else
                    {
                        UploadShipment.Errors.Add("From State is empty please fill and upload shipment again");
                    }
                }
                if (UploadShipment.ShipFrom.Phone == "" || UploadShipment.ShipFrom.Phone == null || !new System.Text.RegularExpressions.Regex("^[0-9]+$").IsMatch(UploadShipment.ShipFrom.Phone))
                { UploadShipment.Errors.Add("From TelephoneNo is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipTo.Country != null && (UploadShipment.ShipTo.Country.Code == "" || UploadShipment.ShipTo.Country.Code == null))
                { UploadShipment.Errors.Add("To Country Code is empty please fill and upload shipment again"); }

                if ((UploadShipment.ShipTo.PostCode == "" || UploadShipment.ShipTo.PostCode == null || (UploadShipment.ShipTo.PostCode != null && !new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9_ ]*$").IsMatch(UploadShipment.ShipTo.PostCode)) && UploadShipment.ShipTo.Country.Code != "HKG"))
                { UploadShipment.Errors.Add("To PostCode is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipTo.CompanyName == UploadShipment.ShipTo.FirstName + " " + UploadShipment.ShipTo.LastName)
                { UploadShipment.Errors.Add("To CompanyName, To Contact First Name and Last Name never same"); }

                if (UploadShipment.ShipTo.FirstName == "" || UploadShipment.ShipTo.FirstName == null)
                { UploadShipment.Errors.Add("To Contact First Name is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipTo.LastName == "" || UploadShipment.ShipTo.LastName == null)
                { UploadShipment.Errors.Add("To Contact Last Name is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipTo.CompanyName == "" || UploadShipment.ShipTo.CompanyName == null)
                { UploadShipment.Errors.Add("To CompanyName is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipTo.Address == "" || UploadShipment.ShipTo.Address == null)
                { UploadShipment.Errors.Add("To Address1 is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipTo.City == "" || UploadShipment.ShipTo.City == null)
                { UploadShipment.Errors.Add("To City is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipTo.Phone == "" || UploadShipment.ShipTo.Phone == null || (UploadShipment.ShipTo.Phone != null && !new System.Text.RegularExpressions.Regex("^[0-9]+$").IsMatch(UploadShipment.ShipTo.Phone)))
                { UploadShipment.Errors.Add("To TelephoneNo is empty please fill and upload shipment again"); }

                if (UploadShipment.PackageCalculationType == "" || UploadShipment.PackageCalculationType == null || UploadShipment.PackageCalculationType.ToUpper() != "KGTOCMS" && UploadShipment.PackageCalculationType.ToUpper() != "LBTOINCHS")
                { UploadShipment.Errors.Add("Package Calculation Type is empty or you entered incorrectly please fill and upload shipment again"); }
                UploadShipment.PayTaxAndDuties = "Receiver";

                if (UploadShipment.parcelType == "" || UploadShipment.parcelType == null || UploadShipment.parcelType.ToUpper() != "PARCEL" && UploadShipment.PackageCalculationType.ToUpper() != "LETTER")
                { UploadShipment.Errors.Add("Parcel Type is empty please fill and upload shipment again"); }

                if (UploadShipment.CurrencyCode == "" || UploadShipment.CurrencyCode == null)
                { UploadShipment.Errors.Add("Currency is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipmentReference == "" || UploadShipment.ShipmentReference == null)
                { UploadShipment.Errors.Add("Shipment Reference is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipmentDescription == "" || UploadShipment.ShipmentDescription == null)
                { UploadShipment.Errors.Add("Shipment Description is empty please fill and upload shipment again"); }

                if (ServiceType == FrayteCallingType.FrayteApi)
                {
                    if (string.IsNullOrEmpty(UploadShipment.CollectionDate))
                    {
                        UploadShipment.Errors.Add("Collection Date is empty please fill and upload shipment again");
                    }
                    else
                    {
                        DateTime CollectionDate = DateTime.ParseExact(UploadShipment.CollectionDate, "dd/MM/yyyy", null);
                        DateTime currentDate = DateTime.ParseExact(DateTime.Now.Date.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);
                        if (CollectionDate.Date < currentDate.Date)
                        {
                            UploadShipment.Errors.Add("Collection Date never small from current date");
                        }

                        if (CollectionDate.Date > currentDate.Date.AddDays(5))
                        { UploadShipment.Errors.Add("Collection Date never greater from current date"); }
                    }
                }
                else
                {
                    if ((UploadShipment.CollectionDate != null && UploadShipment.CollectionDate != "" && Convert.ToDateTime(UploadShipment.CollectionDate) == DateTime.MinValue.AddYears(1800)) || (UploadShipment.CollectionDate != null && UploadShipment.CollectionDate != "") || UploadShipment.CollectionDate == null || UploadShipment.CollectionDate == "")
                    { UploadShipment.Errors.Add("Collection Date is empty please fill and upload shipment again"); }
                }

                if (UploadShipment.CollectionTime == "00:00:00.0000000" || UploadShipment.CollectionTime == "" || UploadShipment.CollectionTime == null || (UploadShipment.CollectionTime != null && UploadShipment.CollectionDate != "" && UploadShipment.CollectionDate != null && UploadShipment.CollectionTime != "" && UploadShipment.CollectionTime != null && DateTime.ParseExact(UploadShipment.CollectionDate, "dd/MM/yyyy", null) <= DateTime.Now.Date && TimeSpan.Parse(UploadShipment.CollectionTime) < DateTime.Now.TimeOfDay))
                { UploadShipment.Errors.Add("Collection Time is empty please fill and upload shipment again"); }

                if (UploadShipment.ShipFrom.Country != null && UploadShipment.ShipTo.Country != null && UploadShipment.ShipFrom.Country.Code != UploadShipment.ShipTo.Country.Code)
                {
                    if (UploadShipment.CustomInfo.ContentsType == "" || UploadShipment.CustomInfo.ContentsType == null)
                    { UploadShipment.Errors.Add("Contents Type is empty please fill and upload shipment again"); }

                    if ((UploadShipment.CustomInfo.ContentsExplanation == "" || UploadShipment.CustomInfo.ContentsExplanation == null) && UploadShipment.CustomInfo.ContentsType.Contains("other"))
                    { UploadShipment.Errors.Add("Contents Explanation is empty please fill and upload shipment again"); }

                    if (UploadShipment.CustomInfo.RestrictionType == "" || UploadShipment.CustomInfo.RestrictionType == null)
                    { UploadShipment.Errors.Add("Restriction Type is empty please fill and upload shipment again"); }

                    if ((UploadShipment.CustomInfo.RestrictionComments == "" || UploadShipment.CustomInfo.RestrictionComments == null) && UploadShipment.CustomInfo.RestrictionType.Contains("other"))
                    { UploadShipment.Errors.Add("Restriction Comments is empty please fill and upload shipment again"); }

                    if (UploadShipment.CustomInfo.NonDeliveryOption == "" || UploadShipment.CustomInfo.NonDeliveryOption == null)
                    { UploadShipment.Errors.Add("Non Delivery Option is empty please fill and upload shipment again"); }

                    if (UploadShipment.CustomInfo.CustomsSigner == "" || UploadShipment.CustomInfo.CustomsSigner == null)
                    { UploadShipment.Errors.Add("Customs Signer is empty please fill and upload shipment again"); }
                }

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


                    if (Package.Weight == 0 || Package.Weight * Package.CartoonValue >= 70 * Package.CartoonValue)
                    { UploadShipment.Errors.Add("Weight is empty in Line no" + i + " please fill and upload shipment again"); }


                    if (Package.Value == 0 && UploadShipment.ShipFrom.Country != null && UploadShipment.ShipTo.Country != null && UploadShipment.ShipFrom.Country.Code != UploadShipment.ShipTo.Country.Code)
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

            if (!columns.Contains("From Country Code"))
            {
                valid = false;
            }
            if (!columns.Contains("From Post Code"))
            {
                valid = false;
            }
            if (!columns.Contains("From Contact First Name"))
            {
                valid = false;
            }
            if (!columns.Contains("From Contact Last Name"))
            {
                valid = false;
            }
            if (!columns.Contains("From Company Name"))
            {
                valid = false;
            }
            if (!columns.Contains("From Address1"))
            {
                valid = false;
            }
            if (!columns.Contains("From Address2"))
            {
                valid = false;
            }
            if (!columns.Contains("From City"))
            {
                valid = false;
            }
            if (!columns.Contains("From TelephoneNo"))
            {
                valid = false;
            }
            if (!columns.Contains("From Email"))
            {
                valid = false;
            }
            if (!columns.Contains("To Country Code"))
            {
                valid = false;
            }
            if (!columns.Contains("To Post Code"))
            {
                valid = false;
            }
            if (!columns.Contains("To Contact First Name"))
            {
                valid = false;
            }
            if (!columns.Contains("To Contact Last Name"))
            {
                valid = false;
            }
            if (!columns.Contains("To Company Name"))
            {
                valid = false;
            }
            if (!columns.Contains("To Address1"))
            {
                valid = false;
            }
            if (!columns.Contains("To Address2"))
            {
                valid = false;
            }
            if (!columns.Contains("To City"))
            {
                valid = false;
            }
            if (!columns.Contains("To TelephoneNo"))
            {
                valid = false;
            }
            if (!columns.Contains("To Email"))
            {
                valid = false;
            }
            //if (!columns.Contains("CourierCompany"))
            //{
            //    valid = false;
            //}
            if (!columns.Contains("Package Calculation Type"))
            {
                valid = false;
            }
            if (!columns.Contains("parcel Type"))
            {
                valid = false;
            }
            if (!columns.Contains("Currency"))
            {
                valid = false;
            }
            if (!columns.Contains("Shipment Reference"))
            {
                valid = false;
            }
            if (!columns.Contains("Shipment Description"))
            {
                valid = false;
            }
            if (!columns.Contains("Collection Date"))
            {
                valid = false;
            }
            if (!columns.Contains("Collection Time"))
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
            if (!columns.Contains("Carton Qty"))
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
            if (!columns.Contains("Declared Value"))
            {
                valid = false;
            }
            if (!columns.Contains("Shipment Contents"))
            {
                valid = false;
            }
            return valid;
        }

        public int SaveSession(int UserId)
        {
            var DBUS = dbContext.DirectBulkUploadSessions.Where(a => a.SessionId == 0).FirstOrDefault();
            var DBUSNew = dbContext.DirectBulkUploadSessions.Where(a => a.UserId == UserId).ToList();
            if (DBUS == null)
            {
                DBUS = new DirectBulkUploadSession();
                DBUS.CreatedOn = DateTime.UtcNow;
                DBUS.SessionStatus = "InProgress";
                dbContext.DirectBulkUploadSessions.Add(DBUS);
                dbContext.SaveChanges();
                DBUS.SessionName = "Session " + (Convert.ToInt32(DBUSNew.Count) + 1);
                DBUS.UserId = UserId;
                dbContext.Entry(DBUS).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

            }
            return DBUS.SessionId;
        }

        public int UpdateSession(int SessionId, int TotalShipments)
        {
            var DBUS = dbContext.DirectBulkUploadSessions.Where(a => a.SessionId == SessionId).FirstOrDefault();
            if (DBUS != null)
            {

                DBUS.TotalShipments = DBUS.TotalShipments + TotalShipments;
                DBUS.SessionStatus = "Booked";
                dbContext.Entry(DBUS).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return DBUS.SessionId;
            }
            else
            {
                return 0;
            }
        }

        public FrayteResult SaveShipment(FrayteUploadshipment shipment, int CustomerId, string ServiceType)
        {
            shipment.CustomerId = CustomerId;
            shipment.ShipFrom.CustomerId = CustomerId;
            shipment.ShipFrom.AddressType = FrayteFromToAddressType.FromAddress;
            //SaveeCommerceShipmentAddress(shipment.ShipFrom);
            SaveDirectShipmentAddress(shipment.ShipFrom);

            ////Step 2: Save ShipTo
            shipment.ShipTo.CustomerId = CustomerId;
            shipment.ShipTo.AddressType = FrayteFromToAddressType.ToAddress;
            //SaveeCommerceShipmentAddress(shipment.ShipTo);
            SaveDirectShipmentAddress(shipment.ShipTo);

            //Step 3: Save Direct Shipmnet + Reference Detail
            SaveeCommerceShipmnetDetail(shipment, ServiceType);

            //SaveDirectShipmnetDetail(shipment);

            //Step 4: Save Direct Shipment Detail
            SaveeCommerceShipmentDetailPackages(shipment);

            //step 5:

            SaveCustomInformation(shipment);

            ////Save 5: Save date time when All HSCode are mapped

            SetMappedOnOneCommerce(shipment);

            //TextWriter tw = File.CreateText(@"C:\FMS\ecomm.godemowithus.com\WebApi\abc.txt");
            //tw.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shipment).ToCharArray());
            //tw.Close();

            FrayteResult result = new FrayteResult();
            result.Status = true;

            return result;
        }

        private void SaveDirectShipmentAddress(DirectBookingCollection shipFrom)
        {
            if (shipFrom.Country.CountryId > 0)
            {
                if (shipFrom.DirectShipmentAddressId == 0)
                {
                    // Step 1.4 : Add address to addressBook if not exist aleady
                    if (shipFrom.AddressType == FrayteFromToAddressType.FromAddress)
                    {
                        var addressBookData = dbContext.AddressBooks.Where(p => p.Address1 == shipFrom.Address &&
                                                                           p.Address2 == shipFrom.Address2 &&
                                                                           p.City == shipFrom.City &&
                                                                           p.State == shipFrom.State &&
                                                                           p.PhoneNo == shipFrom.Phone &&
                                                                           p.Area == shipFrom.Area &&
                                                                           p.CompanyName == shipFrom.CompanyName &&
                                                                           p.ContactFirstName == shipFrom.FirstName &&
                                                                           p.ContactLastName == shipFrom.LastName &&
                                                                           p.CountryId == shipFrom.Country.CountryId &&
                                                                           p.CustomerId == shipFrom.CustomerId &&
                                                                           p.Email == shipFrom.Email &&
                                                                           p.Zip == shipFrom.PostCode &&
                                                                           p.IsActive == true &&
                                                                           p.FromAddress == true).ToList();
                        if (addressBookData != null && addressBookData.Count > 0)
                        {
                            shipFrom.DirectShipmentAddressId = addressBookData[0].AddressBookId;
                        }
                        else
                        {
                            AddressBook dbShipFrom = new AddressBook();
                            dbShipFrom.CustomerId = shipFrom.CustomerId;
                            dbShipFrom.FromAddress = true;
                            dbShipFrom.ToAddress = false;
                            dbShipFrom.Address1 = shipFrom.Address != null && shipFrom.Address != "" ? UtilityRepository.GetString(shipFrom.Address, 100) : "";
                            dbShipFrom.Address2 = shipFrom.Address2 != null && shipFrom.Address2 != "" ? UtilityRepository.GetString(shipFrom.Address2, 100) : "";
                            dbShipFrom.Area = shipFrom.Area != null && shipFrom.Area != "" ? UtilityRepository.GetString(shipFrom.Area, 100) : "";
                            dbShipFrom.City = shipFrom.City != null && shipFrom.City != "" ? UtilityRepository.GetString(shipFrom.City, 20) : "";
                            dbShipFrom.CompanyName = shipFrom.CompanyName;
                            dbShipFrom.ContactFirstName = shipFrom.FirstName;
                            dbShipFrom.ContactLastName = shipFrom.LastName;
                            dbShipFrom.CountryId = shipFrom.Country.CountryId;
                            dbShipFrom.Email = shipFrom.Email;
                            dbShipFrom.PhoneNo = shipFrom.Phone.Replace(" ", "");
                            dbShipFrom.State = shipFrom.State != null && shipFrom.State != "" ? UtilityRepository.GetString(shipFrom.State, 20) : "";
                            dbShipFrom.Zip = shipFrom.PostCode.Replace(" ", "") != null && shipFrom.PostCode.Replace(" ", "") != "" ? UtilityRepository.PostCodeVerification(shipFrom.PostCode.Replace(" ", ""), shipFrom.Country.Code2) : "";
                            dbShipFrom.IsActive = true;
                            dbShipFrom.TableType = FrayteTableType.AddressBook;
                            //dbShipFrom.IsFavorites = shipFrom.IsFavorites;

                            dbContext.AddressBooks.Add(dbShipFrom);
                            if (dbShipFrom != null)
                            {
                                dbContext.SaveChanges();
                            }
                            shipFrom.DirectShipmentAddressId = dbShipFrom.AddressBookId;
                        }
                    }
                    else if (shipFrom.AddressType == FrayteFromToAddressType.ToAddress)
                    {
                        var addressBookData = dbContext.AddressBooks.Where(p => p.Address1 == shipFrom.Address &&
                                                                           p.Address2 == shipFrom.Address2 &&
                                                                           p.City == shipFrom.City &&
                                                                           p.State == shipFrom.State &&
                                                                           p.PhoneNo == shipFrom.Phone &&
                                                                           p.Area == shipFrom.Area &&
                                                                           p.CompanyName == shipFrom.CompanyName &&
                                                                           p.ContactFirstName == shipFrom.FirstName &&
                                                                           p.ContactLastName == shipFrom.LastName &&
                                                                           p.CountryId == shipFrom.Country.CountryId &&
                                                                           p.CustomerId == shipFrom.CustomerId &&
                                                                           p.Email == shipFrom.Email &&
                                                                           p.Zip == shipFrom.PostCode &&
                                                                           p.IsActive == true &&
                                                                           p.ToAddress == true).ToList();
                        if (addressBookData != null && addressBookData.Count > 0)
                        {
                            shipFrom.DirectShipmentAddressId = addressBookData[0].AddressBookId;
                        }
                        else
                        {
                            AddressBook dbShipFrom = new AddressBook();
                            dbShipFrom.CustomerId = shipFrom.CustomerId;
                            dbShipFrom.FromAddress = false;
                            dbShipFrom.ToAddress = true;
                            dbShipFrom.Address1 = shipFrom.Address != null && shipFrom.Address != "" ? UtilityRepository.GetString(shipFrom.Address, 100) : "";
                            dbShipFrom.Address2 = shipFrom.Address2 != null && shipFrom.Address2 != "" ? UtilityRepository.GetString(shipFrom.Address2, 100) : "";
                            dbShipFrom.Area = shipFrom.Area != null && shipFrom.Area != "" ? UtilityRepository.GetString(shipFrom.Area, 100) : "";
                            dbShipFrom.City = shipFrom.City != null && shipFrom.City != "" ? UtilityRepository.GetString(shipFrom.City, 20) : "";
                            dbShipFrom.CompanyName = shipFrom.CompanyName;
                            dbShipFrom.ContactFirstName = shipFrom.FirstName;
                            dbShipFrom.ContactLastName = shipFrom.LastName;
                            dbShipFrom.CountryId = shipFrom.Country.CountryId;
                            dbShipFrom.Email = shipFrom.Email;
                            dbShipFrom.PhoneNo = shipFrom.Phone.Replace(" ", ""); ;
                            dbShipFrom.State = shipFrom.State != null && shipFrom.State != "" ? UtilityRepository.GetString(shipFrom.State, 20) : "";
                            dbShipFrom.Zip = shipFrom.PostCode.Replace(" ", "") != null && shipFrom.PostCode.Replace(" ", "") != "" ? UtilityRepository.PostCodeVerification(shipFrom.PostCode.Replace(" ", ""), shipFrom.Country.Code2) : "";
                            dbShipFrom.IsActive = true;
                            dbShipFrom.TableType = FrayteTableType.AddressBook;
                            //dbShipFrom.IsFavorites = shipFrom.IsFavorites;

                            dbContext.AddressBooks.Add(dbShipFrom);
                            if (dbShipFrom != null)
                            {
                                dbContext.SaveChanges();
                            }
                            shipFrom.DirectShipmentAddressId = dbShipFrom.AddressBookId;
                        }
                    }
                }
                else
                {
                    AddressBook dbShipFrom = dbContext.AddressBooks.Find(shipFrom.DirectShipmentAddressId);
                    if (dbShipFrom != null)
                    {
                        dbShipFrom.FromAddress = dbShipFrom.ToAddress == true ? false : true;
                        dbShipFrom.ToAddress = dbShipFrom.FromAddress == true ? false : true;
                        dbShipFrom.CustomerId = shipFrom.CustomerId;
                        dbShipFrom.Address1 = shipFrom.Address != null && shipFrom.Address != "" ? UtilityRepository.GetString(shipFrom.Address, 100) : "";
                        dbShipFrom.Address2 = shipFrom.Address2 != null && shipFrom.Address2 != "" ? UtilityRepository.GetString(shipFrom.Address2, 100) : "";
                        dbShipFrom.Area = shipFrom.Area != null && shipFrom.Area != "" ? UtilityRepository.GetString(shipFrom.Area, 100) : "";
                        dbShipFrom.City = shipFrom.City != null && shipFrom.City != "" ? UtilityRepository.GetString(shipFrom.City, 20) : "";
                        dbShipFrom.CompanyName = shipFrom.CompanyName;
                        dbShipFrom.ContactFirstName = shipFrom.FirstName;
                        dbShipFrom.ContactLastName = shipFrom.LastName;
                        dbShipFrom.CountryId = shipFrom.Country.CountryId;
                        dbShipFrom.Email = shipFrom.Email;
                        dbShipFrom.PhoneNo = shipFrom.Phone.Replace(" ", ""); ;
                        dbShipFrom.State = shipFrom.State != null && shipFrom.State != "" ? UtilityRepository.GetString(shipFrom.State, 20) : "";
                        dbShipFrom.Zip = shipFrom.PostCode.Replace(" ", "") != null && shipFrom.PostCode.Replace(" ", "") != "" ? UtilityRepository.PostCodeVerification(shipFrom.PostCode.Replace(" ", ""), shipFrom.Country.Code2) : ""; ;
                        dbShipFrom.IsActive = true;
                        dbShipFrom.TableType = FrayteTableType.AddressBook;
                        //dbShipFrom.IsFavorites = shipFrom.IsFavorites == true ? true : false;
                    }

                    if (dbShipFrom != null)
                    {
                        dbContext.Entry(dbShipFrom).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    shipFrom.DirectShipmentAddressId = dbShipFrom.AddressBookId;
                }
            }
        }

        private void SaveCustomInformation(FrayteUploadshipment directBookingShippingDetail)
        {
            if (directBookingShippingDetail.ShipFrom.Country.Code == directBookingShippingDetail.ShipTo.Country.Code)
            {
                var removeShipmentCustomDetail = dbContext.ShipmentCustomDetailDrafts.Where(p => p.ShipmentDraftId == directBookingShippingDetail.DirectShipmentDraftId &&
                                                                                            p.ShipmentServiceType == FrayteShipmentServiceType.DirectBooking).FirstOrDefault();
                if (removeShipmentCustomDetail != null)
                {
                    dbContext.ShipmentCustomDetailDrafts.Remove(removeShipmentCustomDetail);
                    dbContext.SaveChanges();
                }

                return;
            }

            ShipmentCustomDetailDraft shipmentCustomDetail = new ShipmentCustomDetailDraft();

            if (directBookingShippingDetail.CustomInfo != null && directBookingShippingDetail.CustomInfo.ShipmentCustomDetailId > 0)
            {
                shipmentCustomDetail = dbContext.ShipmentCustomDetailDrafts.Find(directBookingShippingDetail.CustomInfo.ShipmentCustomDetailId);
                if (shipmentCustomDetail != null)
                {
                    shipmentCustomDetail.ShipmentDraftId = directBookingShippingDetail.DirectShipmentDraftId;
                    shipmentCustomDetail.ShipmentServiceType = FrayteShipmentServiceType.DirectBooking;

                    //if (directBookingShippingDetail.CustomerRateCard != null && directBookingShippingDetail.CustomerRateCard.CourierId > 0)
                    //{
                    //    if (directBookingShippingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.UK_EU)
                    //    {
                    //        //ParcelHub Custom Details
                    //        shipmentCustomDetail.CatagoryOfItem = directBookingShippingDetail.CustomInfo.CatagoryOfItem;
                    //        shipmentCustomDetail.CatagoryOfItemExplanation = directBookingShippingDetail.CustomInfo.CatagoryOfItemExplanation;
                    //        shipmentCustomDetail.CustomsCertify = directBookingShippingDetail.CustomInfo.CustomsCertify;
                    //        directBookingShippingDetail.CustomInfo.TermOfTrade = directBookingShippingDetail.PayTaxAndDuties;
                    //        shipmentCustomDetail.TermOfTrade = directBookingShippingDetail.CustomInfo.TermOfTrade;
                    //    }
                    //    else
                    //    {
                    //EasyPost Custome Details
                    shipmentCustomDetail.ContentsType = directBookingShippingDetail.CustomInfo.ContentsType;
                    shipmentCustomDetail.ContentsExplanation = directBookingShippingDetail.CustomInfo.ContentsExplanation;
                    shipmentCustomDetail.RestrictionType = directBookingShippingDetail.CustomInfo.RestrictionType;
                    shipmentCustomDetail.RestrictionComments = directBookingShippingDetail.CustomInfo.RestrictionComments;
                    shipmentCustomDetail.CustomsCertify = directBookingShippingDetail.CustomInfo.CustomsCertify;
                    shipmentCustomDetail.CustomsSigner = directBookingShippingDetail.CustomInfo.CustomsSigner;
                    shipmentCustomDetail.NonDeliveryOption = directBookingShippingDetail.CustomInfo.NonDeliveryOption != null && directBookingShippingDetail.CustomInfo.NonDeliveryOption != "" ? UtilityRepository.GetString(directBookingShippingDetail.CustomInfo.NonDeliveryOption, 10) : "";
                    shipmentCustomDetail.EelPfc = directBookingShippingDetail.CustomInfo.EelPfc;
                    dbContext.Entry(shipmentCustomDetail).State = System.Data.Entity.EntityState.Modified;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            if (directBookingShippingDetail.ShippingMethodId > 0)
                    //            {
                    //                var CourierName = dbContext.Couriers.Find(directBookingShippingDetail.ShippingMethodId).CourierName;
                    //                if (CourierName == FrayteCourierCompany.UK_EU)
                    //                {
                    //                    //ParcelHub Custom Details
                    //                    shipmentCustomDetail.CatagoryOfItem = directBookingShippingDetail.CustomInfo.CatagoryOfItem;
                    //                    shipmentCustomDetail.CatagoryOfItemExplanation = directBookingShippingDetail.CustomInfo.CatagoryOfItemExplanation;
                    //                    shipmentCustomDetail.CustomsCertify = directBookingShippingDetail.CustomInfo.CustomsCertify;
                    //                    directBookingShippingDetail.CustomInfo.TermOfTrade = directBookingShippingDetail.PayTaxAndDuties;
                    //                    shipmentCustomDetail.TermOfTrade = directBookingShippingDetail.CustomInfo.TermOfTrade;
                    //                }
                    //                else
                    //                {
                    //                    //EasyPost Custome Details
                    //                    shipmentCustomDetail.ContentsType = directBookingShippingDetail.CustomInfo.ContentsType;
                    //                    shipmentCustomDetail.ContentsExplanation = directBookingShippingDetail.CustomInfo.ContentsExplanation;
                    //                    shipmentCustomDetail.RestrictionType = directBookingShippingDetail.CustomInfo.RestrictionType;
                    //                    shipmentCustomDetail.RestrictionComments = directBookingShippingDetail.CustomInfo.RestrictionComments;
                    //                    shipmentCustomDetail.CustomsCertify = directBookingShippingDetail.CustomInfo.CustomsCertify;
                    //                    shipmentCustomDetail.CustomsSigner = directBookingShippingDetail.CustomInfo.CustomsSigner;
                    //                    shipmentCustomDetail.NonDeliveryOption = directBookingShippingDetail.CustomInfo.NonDeliveryOption;
                    //                    shipmentCustomDetail.EelPfc = directBookingShippingDetail.CustomInfo.EelPfc;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    shipmentCustomDetail = new ShipmentCustomDetailDraft();
                    //    shipmentCustomDetail.ShipmentDraftId = directBookingShippingDetail.DirectShipmentDraftId;
                    //    shipmentCustomDetail.ShipmentServiceType = FrayteShipmentServiceType.DirectBooking;
                    //    var CourierName = "";
                    //    if (directBookingShippingDetail.ShippingMethodId > 0)
                    //    {
                    //        var Courier = dbContext.Couriers.Find(directBookingShippingDetail.ShippingMethodId);
                    //        if (Courier != null)
                    //        {
                    //            CourierName = Courier.CourierName;
                    //        }
                    //    }
                    //    if (CourierName == FrayteCourierCompany.UK_EU)
                    //    {
                    //        //ParcelHub Custom Details
                    //        shipmentCustomDetail.CatagoryOfItem = directBookingShippingDetail.CustomInfo.CatagoryOfItem;
                    //        shipmentCustomDetail.CatagoryOfItemExplanation = directBookingShippingDetail.CustomInfo.CatagoryOfItemExplanation;
                    //        shipmentCustomDetail.CustomsCertify = directBookingShippingDetail.CustomInfo.CustomsCertify;
                    //        shipmentCustomDetail.TermOfTrade = directBookingShippingDetail.CustomInfo.TermOfTrade;
                    //        directBookingShippingDetail.CustomInfo.TermOfTrade = directBookingShippingDetail.PayTaxAndDuties;
                    //        shipmentCustomDetail.TermOfTrade = directBookingShippingDetail.CustomInfo.TermOfTrade;
                    //    }
                    //    else
                    //    {
                    //        //EasyPost Custome Details
                    //        shipmentCustomDetail.ContentsType = directBookingShippingDetail.CustomInfo.ContentsType;
                    //        shipmentCustomDetail.ContentsExplanation = directBookingShippingDetail.CustomInfo.ContentsExplanation;
                    //        shipmentCustomDetail.RestrictionType = directBookingShippingDetail.CustomInfo.RestrictionType;
                    //        shipmentCustomDetail.RestrictionComments = directBookingShippingDetail.CustomInfo.RestrictionComments;
                    //        shipmentCustomDetail.CustomsCertify = directBookingShippingDetail.CustomInfo.CustomsCertify;
                    //        shipmentCustomDetail.CustomsSigner = directBookingShippingDetail.CustomInfo.CustomsSigner;
                    //        shipmentCustomDetail.NonDeliveryOption = directBookingShippingDetail.CustomInfo.NonDeliveryOption;
                    //        shipmentCustomDetail.EelPfc = directBookingShippingDetail.CustomInfo.EelPfc;
                }
            }
            else
            {
                shipmentCustomDetail.ShipmentDraftId = directBookingShippingDetail.DirectShipmentDraftId;
                shipmentCustomDetail.ShipmentServiceType = FrayteShipmentServiceType.DirectBooking;
                shipmentCustomDetail.ContentsType = directBookingShippingDetail.CustomInfo.ContentsType;
                shipmentCustomDetail.ContentsExplanation = directBookingShippingDetail.CustomInfo.ContentsExplanation;
                shipmentCustomDetail.RestrictionType = directBookingShippingDetail.CustomInfo.RestrictionType;
                shipmentCustomDetail.RestrictionComments = directBookingShippingDetail.CustomInfo.RestrictionComments;
                shipmentCustomDetail.CustomsCertify = directBookingShippingDetail.CustomInfo.CustomsCertify;
                shipmentCustomDetail.CustomsSigner = directBookingShippingDetail.CustomInfo.CustomsSigner;
                shipmentCustomDetail.NonDeliveryOption = directBookingShippingDetail.CustomInfo.NonDeliveryOption != null && directBookingShippingDetail.CustomInfo.NonDeliveryOption != "" ? UtilityRepository.GetString(directBookingShippingDetail.CustomInfo.NonDeliveryOption, 10) : "";
                shipmentCustomDetail.EelPfc = directBookingShippingDetail.CustomInfo.EelPfc;
                dbContext.ShipmentCustomDetailDrafts.Add(shipmentCustomDetail);
            }

            if (shipmentCustomDetail != null)
            {
                dbContext.SaveChanges();
            }
        }

        private void SaveeCommerceShipmentAddress(DirectBookingCollection address)
        {
            var Country = dbContext.Countries.Where(a => a.CountryCode == address.Country.Code || a.CountryCode2 == address.Country.Code || a.CountryName == address.Country.Code || a.CountryId == address.Country.CountryId).FirstOrDefault();
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
                dbAddress.Address1 = UtilityRepository.GetString(address.Address, eCommerceString.AddressStringLength);
                dbAddress.Address2 = UtilityRepository.GetString(address.Address2, eCommerceString.AddressStringLength);
                dbAddress.Area = address.Area;
                dbAddress.City = address.City;
                dbAddress.CompanyName = address.CompanyName;
                dbAddress.ContactFirstName = address.FirstName;
                dbAddress.ContactLastName = address.LastName;
                dbAddress.CountryId = Country == null ? 0 : Country.CountryId;
                dbAddress.Email = address.Email;
                dbAddress.PhoneNo = UtilityRepository.GetString(address.Phone, eCommerceString.PhoneStringLength);
                dbAddress.State = address.State;
                dbAddress.Zip = address.PostCode;
                dbAddress.IsActive = true;
                dbAddress.TableType = FrayteTableType.AddressBook;
                dbAddress.ModuleType = FrayteShipmentServiceType.eCommerce;
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
                    dbContext.Entry(dbAddress).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
                address.DirectShipmentAddressId = dbAddress.DirectShipmentAddressDraftId;
            }
        }

        public List<Frayte.Services.Models.CountryLogistic> GetServices()
        {
            List<Frayte.Services.Models.CountryLogistic> CLL = new List<Frayte.Services.Models.CountryLogistic>();

            var result = dbContext.CountryLogistics.ToList();
            foreach (var res in result)
            {
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

        private bool CheckEuropeCountries(DirectBookingFindService serviceRequest)
        {
            bool isFromCountryInEurope = false;
            bool isToCountryInEurope = false;

            //Check fromCountryCode
            if (serviceRequest.FromCountry.Code == "GBR")
            {
                isFromCountryInEurope = true;
            }
            {
                var countryFound = (from zc in dbContext.LogisticServiceZoneCountries
                                    join
                                        c in dbContext.Countries on zc.CountryId equals c.CountryId
                                    join
                                        z in dbContext.LogisticServiceZones on zc.LogisticServiceZoneId equals z.LogisticServiceZoneId
                                    join
                                        ls in dbContext.LogisticServices on z.LogisticServiceId equals ls.LogisticServiceId
                                    where ls.OperationZoneId == serviceRequest.OperationZoneId && c.CountryId == serviceRequest.FromCountry.CountryId
                                    && (ls.LogisticType == FrayteLogisticType.EUImport || ls.LogisticType == FrayteLogisticType.EUExport)
                                    select zc).ToList();
                if (countryFound != null && countryFound.Count > 0)
                {
                    isFromCountryInEurope = true;
                }
            }

            if (serviceRequest.ToCountry.Code == "GBR")
            {
                isToCountryInEurope = true;
            }
            {
                var countryFound = (from zc in dbContext.LogisticServiceZoneCountries
                                    join
                                        c in dbContext.Countries on zc.CountryId equals c.CountryId
                                    join
                                        z in dbContext.LogisticServiceZones on zc.LogisticServiceZoneId equals z.LogisticServiceZoneId
                                    join
                                        ls in dbContext.LogisticServices on z.LogisticServiceId equals ls.LogisticServiceId
                                    where ls.OperationZoneId == serviceRequest.OperationZoneId && c.CountryId == serviceRequest.ToCountry.CountryId
                                    && (ls.LogisticType == FrayteLogisticType.EUImport || ls.LogisticType == FrayteLogisticType.EUExport)
                                    select zc).ToList();

                if (countryFound != null && countryFound.Count > 0)
                {
                    isToCountryInEurope = true;
                }
            }

            return isFromCountryInEurope && isToCountryInEurope;

        }

        private void SaveDirectShipmnetDetail(FrayteUploadshipment directBookingShippingDetail)
        {
            DirectShipmentDraft dbDirectShipment;
            FrayteOperationZone operationZone = UtilityRepository.GetOperationZone();

            if (directBookingShippingDetail.DirectShipmentDraftId == 0)
            {
                dbDirectShipment = new DirectShipmentDraft();
                dbDirectShipment.ShipmentStatusId = directBookingShippingDetail.ShipmentStatusId;
                dbDirectShipment.CurrencyCode = directBookingShippingDetail.CurrencyCode;

                DirectBookingFindService serviceRequest = new DirectBookingFindService();
                serviceRequest.FromCountry = directBookingShippingDetail.ShipFrom.Country;
                serviceRequest.ToCountry = directBookingShippingDetail.ShipTo.Country;
                serviceRequest.OperationZoneId = operationZone.OperationZoneId;
                bool isEuropeCountry = CheckEuropeCountries(serviceRequest);

                dbDirectShipment.LogisticType = UtilityRepository.GetLogisticType(operationZone.OperationZoneName, directBookingShippingDetail.ShipFrom.Country.Code, directBookingShippingDetail.ShipTo.Country.Code, isEuropeCountry);

                if (dbDirectShipment.LogisticType == FrayteLogisticType.UKShipment)
                {
                    //dbDirectShipment.LogisticServiceType = directBookingShippingDetail.CustomerRateCard.CourierName;
                }
                else
                {
                    dbDirectShipment.LogisticServiceType = directBookingShippingDetail.CustomerRateCard.CourierName;
                }

                if (dbDirectShipment.ShipmentStatusId == DirectBookingShippingStatus.Draft && directBookingShippingDetail.CustomerRateCard != null)
                {
                    #region -- Set Base Rate --

                    if (directBookingShippingDetail.CustomerRateCard.ZoneRateCardId > 0)
                    {
                        if (directBookingShippingDetail.CustomerRateCard.LogisticDescription == FrayteShipmentType.HeavyWeight)
                        {
                            dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate * directBookingShippingDetail.CustomerRateCard.Weight;
                        }
                        else
                        {
                            dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate;
                        }
                        dbDirectShipment.ShipmentTypeId = directBookingShippingDetail.CustomerRateCard.LogisticShipmentId;
                        dbDirectShipment.CourierAccountId = directBookingShippingDetail.CustomerRateCard.CourierAccountId;
                        dbDirectShipment.Margin = (dbDirectShipment.BaseRate) * (directBookingShippingDetail.CustomerRateCard.MarginPercent / 100);
                        dbDirectShipment.FuelSurCharge = (dbDirectShipment.BaseRate + dbDirectShipment.Margin) * (decimal)(directBookingShippingDetail.CustomerRateCard.FuelSurcharge / 100);

                        //Additional Surcharge
                        dbDirectShipment.AdditionalSurcharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge);
                        dbDirectShipment.FuelMonthYear = directBookingShippingDetail.CustomerRateCard.FuelDate;
                        dbDirectShipment.FuelSurchargePercent = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurcharge);
                    }
                    else
                    {
                        dbDirectShipment.ShipmentTypeId = directBookingShippingDetail.ShippingMethodId;
                        directBookingShippingDetail.CustomerRateCard.LogisticServiceId = 0;
                        dbDirectShipment.CourierAccountId = directBookingShippingDetail.CustomerRateCard.CourierId;
                        dbDirectShipment.BaseRate = (directBookingShippingDetail.CustomerRateCard.Rate) - (directBookingShippingDetail.CustomerRateCard.Margin);
                        dbDirectShipment.Margin = directBookingShippingDetail.CustomerRateCard.Margin;
                        dbDirectShipment.FuelSurCharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurcharge);
                        dbDirectShipment.AdditionalSurcharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge);
                        dbDirectShipment.FuelSurchargePercent = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurchargePercent);
                        dbDirectShipment.FuelMonthYear = directBookingShippingDetail.CustomerRateCard.FuelDate;
                    }

                    #endregion
                }
                else if (dbDirectShipment.ShipmentStatusId == DirectBookingShippingStatus.Current && directBookingShippingDetail.CustomerRateCard != null)
                {
                    #region -- Set Base Rate --

                    if (directBookingShippingDetail.CustomerRateCard.ZoneRateCardId > 0)
                    {
                        if (directBookingShippingDetail.CustomerRateCard.LogisticDescription == FrayteShipmentType.HeavyWeight)
                        {
                            dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate * directBookingShippingDetail.CustomerRateCard.Weight;
                        }
                        else
                        {
                            dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate;
                        }
                        dbDirectShipment.Margin = (dbDirectShipment.BaseRate) * (directBookingShippingDetail.CustomerRateCard.MarginPercent / 100);
                        dbDirectShipment.FuelSurCharge = (dbDirectShipment.BaseRate + dbDirectShipment.Margin) * (decimal)(directBookingShippingDetail.CustomerRateCard.FuelSurcharge / 100);
                        dbDirectShipment.ShipmentTypeId = directBookingShippingDetail.CustomerRateCard.LogisticShipmentId;
                        dbDirectShipment.CourierAccountId = directBookingShippingDetail.CustomerRateCard.CourierAccountId;

                        //Additional Surcharge
                        dbDirectShipment.AdditionalSurcharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge);
                        dbDirectShipment.FuelMonthYear = directBookingShippingDetail.CustomerRateCard.FuelDate;
                        dbDirectShipment.FuelSurchargePercent = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurcharge);
                    }
                    else
                    {
                        dbDirectShipment.ShipmentTypeId = directBookingShippingDetail.ShippingMethodId;
                        directBookingShippingDetail.CustomerRateCard.LogisticServiceId = 0;
                        dbDirectShipment.CourierAccountId = directBookingShippingDetail.CustomerRateCard.CourierId;
                        dbDirectShipment.BaseRate = (directBookingShippingDetail.CustomerRateCard.Rate) - (directBookingShippingDetail.CustomerRateCard.Margin);
                        dbDirectShipment.Margin = directBookingShippingDetail.CustomerRateCard.Margin;
                        dbDirectShipment.FuelSurCharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurcharge);
                        dbDirectShipment.AdditionalSurcharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge);
                        dbDirectShipment.FuelSurchargePercent = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurchargePercent);
                        dbDirectShipment.FuelMonthYear = directBookingShippingDetail.CustomerRateCard.FuelDate;
                    }

                    #endregion
                }
                else if (dbDirectShipment.ShipmentStatusId == DirectBookingShippingStatus.Cancel && directBookingShippingDetail.CustomerRateCard != null)
                {
                    #region -- Set Base Raet --

                    if (directBookingShippingDetail.CustomerRateCard.ZoneRateCardId > 0)
                    {
                        if (directBookingShippingDetail.CustomerRateCard.LogisticDescription == FrayteShipmentType.HeavyWeight)
                        {
                            dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate * directBookingShippingDetail.CustomerRateCard.Weight;
                        }
                        else
                        {
                            dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate;
                        }
                        dbDirectShipment.Margin = (dbDirectShipment.BaseRate) * (directBookingShippingDetail.CustomerRateCard.MarginPercent / 100);
                        dbDirectShipment.FuelSurCharge = (dbDirectShipment.BaseRate + dbDirectShipment.Margin) * (decimal)(directBookingShippingDetail.CustomerRateCard.FuelSurcharge / 100);
                        dbDirectShipment.ShipmentTypeId = directBookingShippingDetail.CustomerRateCard.LogisticShipmentId;
                        dbDirectShipment.CourierAccountId = directBookingShippingDetail.CustomerRateCard.CourierAccountId;

                        //Additional Surcharge
                        dbDirectShipment.AdditionalSurcharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge);
                        dbDirectShipment.FuelMonthYear = directBookingShippingDetail.CustomerRateCard.FuelDate;
                        dbDirectShipment.FuelSurchargePercent = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurcharge);
                    }
                    else
                    {
                        dbDirectShipment.ShipmentTypeId = directBookingShippingDetail.ShippingMethodId;
                        directBookingShippingDetail.CustomerRateCard.LogisticServiceId = 0;
                        dbDirectShipment.CourierAccountId = directBookingShippingDetail.CustomerRateCard.CourierId;
                        dbDirectShipment.BaseRate = (directBookingShippingDetail.CustomerRateCard.Rate) - (directBookingShippingDetail.CustomerRateCard.Margin);
                        dbDirectShipment.Margin = directBookingShippingDetail.CustomerRateCard.Margin;
                        dbDirectShipment.FuelSurCharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurcharge);
                        dbDirectShipment.AdditionalSurcharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge);
                        dbDirectShipment.FuelSurchargePercent = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurchargePercent);
                        dbDirectShipment.FuelMonthYear = directBookingShippingDetail.CustomerRateCard.FuelDate;
                    }

                    #endregion
                }

                //Set Reference Detail
                dbDirectShipment.Reference1 = directBookingShippingDetail.ShipmentReference;
                dbDirectShipment.ContentDescription = directBookingShippingDetail.ShipmentDescription;
                dbDirectShipment.SpecialInstruction = directBookingShippingDetail.ShipmentDescription;
                if (directBookingShippingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.UKMail || directBookingShippingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Yodel || directBookingShippingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Hermes)
                {
                    dbDirectShipment.CollectionDate = null;
                    dbDirectShipment.CollectionTime = null;
                }
                else
                {
                    //dbDirectShipment.CollectionDate = directBookingShippingDetail .CollectionDate.HasValue ? UtilityRepository.ConvertDateTimetoUniversalTime((DateTime)directBookingShippingDetail.ReferenceDetail.CollectionDate) : DateTime.UtcNow;
                    //dbDirectShipment.CollectionTime = UtilityRepository.TimeSpanConversion(dbDirectShipment.CollectionDate.Value.Hour, dbDirectShipment.CollectionDate.Value.Minute, dbDirectShipment.CollectionDate.Value.Second);
                }
                dbDirectShipment.CustomerId = directBookingShippingDetail.CustomerId;
                dbDirectShipment.FromAddressId = directBookingShippingDetail.ShipFrom.DirectShipmentAddressId;
                dbDirectShipment.ToAddressId = directBookingShippingDetail.ShipTo.DirectShipmentAddressId;
                dbDirectShipment.IsPODMailSent = false;
                dbDirectShipment.OpearionZoneId = operationZone.OperationZoneId;
                dbDirectShipment.ParcelType = directBookingShippingDetail.parcelType == null ? "" : directBookingShippingDetail.parcelType;
                dbDirectShipment.PaymentPartyTaxAndDuties = directBookingShippingDetail.PayTaxAndDuties;
                //dbDirectShipment.PaymentPartyTaxAndDutiesAccountNo = directBookingShippingDetail.PaymentPartyAccountNumber;
                dbDirectShipment.CreatedBy = directBookingShippingDetail.CustomerId;
                dbDirectShipment.LastUpdated = DateTime.UtcNow;
                dbDirectShipment.PackageCaculatonType = directBookingShippingDetail.PackageCalculationType;
                dbDirectShipment.TaxAndDutiesAcceptedBy = directBookingShippingDetail.PayTaxAndDuties;
                dbDirectShipment.FrayteNumber = null;
                directBookingShippingDetail.FrayteNumber = CommonConversion.GetNewFrayteNumber();
                dbDirectShipment.ModuleType = FrayteShipmentServiceType.DirectBooking;
                //dbDirectShipment.AddressType = directBookingShippingDetail.AddressType;
                dbContext.DirectShipmentDrafts.Add(dbDirectShipment);
            }
            else
            {
                dbDirectShipment = dbContext.DirectShipmentDrafts.Find(directBookingShippingDetail.DirectShipmentDraftId);
                if (dbDirectShipment != null)
                {
                    dbDirectShipment.ShipmentStatusId = directBookingShippingDetail.ShipmentStatusId;
                    dbDirectShipment.CurrencyCode = directBookingShippingDetail.CurrencyCode;

                    DirectBookingFindService serviceRequest = new DirectBookingFindService();
                    serviceRequest.FromCountry = directBookingShippingDetail.ShipFrom.Country;
                    serviceRequest.ToCountry = directBookingShippingDetail.ShipTo.Country;
                    serviceRequest.OperationZoneId = operationZone.OperationZoneId;
                    bool isEuropeCountry = CheckEuropeCountries(serviceRequest);

                    dbDirectShipment.LogisticType = UtilityRepository.GetLogisticType(operationZone.OperationZoneName, directBookingShippingDetail.ShipFrom.Country.Code, directBookingShippingDetail.ShipTo.Country.Code, isEuropeCountry);
                    if (directBookingShippingDetail.CustomerRateCard != null)
                    {
                        dbDirectShipment.LogisticServiceType = directBookingShippingDetail.CustomerRateCard.LogisticServiceType;
                    }
                    else
                    {
                        dbDirectShipment.LogisticServiceType = null;
                    }

                    if (dbDirectShipment.LogisticType == FrayteLogisticType.UKShipment)
                    {
                        if (directBookingShippingDetail.CustomerRateCard != null)
                        {
                            dbDirectShipment.LogisticServiceType = directBookingShippingDetail.CustomerRateCard.CourierName;
                        }
                        else
                        {
                            dbDirectShipment.LogisticServiceType = null;
                        }
                    }
                    else
                    {
                        if (directBookingShippingDetail.CustomerRateCard != null)
                        {
                            dbDirectShipment.LogisticServiceType = directBookingShippingDetail.CustomerRateCard.CourierName;
                        }
                        else
                        {
                            dbDirectShipment.LogisticServiceType = null;
                        }
                    }

                    if (dbDirectShipment.ShipmentStatusId == DirectBookingShippingStatus.Draft && directBookingShippingDetail.CustomerRateCard != null && directBookingShippingDetail.CustomerRateCard.ZoneRateCardId > 0)
                    {
                        #region -- Update Base Rate --

                        if (directBookingShippingDetail.CustomerRateCard.LogisticDescription == FrayteShipmentType.HeavyWeight)
                        {
                            dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate * directBookingShippingDetail.CustomerRateCard.Weight;
                        }
                        else
                        {
                            dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate;
                        }
                        dbDirectShipment.Margin = (dbDirectShipment.BaseRate) * (directBookingShippingDetail.CustomerRateCard.MarginPercent / 100);
                        dbDirectShipment.FuelSurCharge = (dbDirectShipment.BaseRate + dbDirectShipment.Margin) * (decimal)(directBookingShippingDetail.CustomerRateCard.FuelSurcharge / 100);
                        dbDirectShipment.ShipmentTypeId = directBookingShippingDetail.CustomerRateCard.LogisticShipmentId;
                        dbDirectShipment.CourierAccountId = directBookingShippingDetail.CustomerRateCard.CourierAccountId;

                        dbDirectShipment.AdditionalSurcharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge);
                        dbDirectShipment.FuelMonthYear = directBookingShippingDetail.CustomerRateCard.FuelDate;
                        dbDirectShipment.FuelSurchargePercent = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurcharge);

                        #endregion
                    }
                    else if (dbDirectShipment.ShipmentStatusId == DirectBookingShippingStatus.Current && directBookingShippingDetail.CustomerRateCard != null && directBookingShippingDetail.CustomerRateCard.ZoneRateCardId > 0)
                    {
                        #region -- Update Base Rate --

                        if (directBookingShippingDetail.CustomerRateCard.LogisticDescription == FrayteShipmentType.HeavyWeight)
                        {
                            dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate * directBookingShippingDetail.CustomerRateCard.Weight;
                        }
                        else
                        {
                            dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate;
                        }
                        dbDirectShipment.Margin = (dbDirectShipment.BaseRate) * (directBookingShippingDetail.CustomerRateCard.MarginPercent / 100);
                        dbDirectShipment.FuelSurCharge = (dbDirectShipment.BaseRate + dbDirectShipment.Margin) * (decimal)(directBookingShippingDetail.CustomerRateCard.FuelSurcharge / 100);
                        dbDirectShipment.ShipmentTypeId = directBookingShippingDetail.CustomerRateCard.LogisticShipmentId;
                        dbDirectShipment.CourierAccountId = directBookingShippingDetail.CustomerRateCard.CourierAccountId;

                        dbDirectShipment.AdditionalSurcharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge);
                        dbDirectShipment.FuelMonthYear = directBookingShippingDetail.CustomerRateCard.FuelDate;
                        dbDirectShipment.FuelSurchargePercent = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurcharge);

                        #endregion
                    }
                    else if (dbDirectShipment.ShipmentStatusId == DirectBookingShippingStatus.Cancel && directBookingShippingDetail.CustomerRateCard != null && directBookingShippingDetail.CustomerRateCard.ZoneRateCardId > 0)
                    {
                        #region -- Update Base Raet --

                        if (directBookingShippingDetail.CustomerRateCard.LogisticDescription == FrayteShipmentType.HeavyWeight)
                        {
                            dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate * directBookingShippingDetail.CustomerRateCard.Weight;
                        }
                        else
                        {
                            dbDirectShipment.BaseRate = directBookingShippingDetail.CustomerRateCard.BaseRate;
                        }
                        dbDirectShipment.Margin = (dbDirectShipment.BaseRate) * (directBookingShippingDetail.CustomerRateCard.MarginPercent / 100);
                        dbDirectShipment.FuelSurCharge = (dbDirectShipment.BaseRate + dbDirectShipment.Margin) * (decimal)(directBookingShippingDetail.CustomerRateCard.FuelSurcharge / 100);
                        dbDirectShipment.ShipmentTypeId = directBookingShippingDetail.CustomerRateCard.LogisticShipmentId;
                        dbDirectShipment.CourierAccountId = directBookingShippingDetail.CustomerRateCard.CourierAccountId;

                        dbDirectShipment.AdditionalSurcharge = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.AdditionalSurcharge);
                        dbDirectShipment.FuelMonthYear = directBookingShippingDetail.CustomerRateCard.FuelDate;
                        dbDirectShipment.FuelSurchargePercent = Convert.ToDecimal(directBookingShippingDetail.CustomerRateCard.FuelSurcharge);

                        #endregion
                    }

                    //Set Reference Detail
                    dbDirectShipment.Reference1 = directBookingShippingDetail.ShipmentReference;
                    dbDirectShipment.ContentDescription = directBookingShippingDetail.ShipmentDescription;
                    dbDirectShipment.SpecialInstruction = directBookingShippingDetail.ShipmentDescription;
                    if (directBookingShippingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.UKMail || directBookingShippingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Yodel || directBookingShippingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Hermes)
                    {
                        dbDirectShipment.CollectionDate = null;
                        dbDirectShipment.CollectionTime = null;
                    }
                    else
                    {
                        //dbDirectShipment.CollectionDate = directBookingShippingDetail.ReferenceDetail.CollectionDate.HasValue ? UtilityRepository.ConvertDateTimetoUniversalTime((DateTime)directBookingShippingDetail.ReferenceDetail.CollectionDate) : DateTime.UtcNow;
                        //dbDirectShipment.CollectionTime = UtilityRepository.TimeSpanConversion(dbDirectShipment.CollectionDate.Value.Hour, dbDirectShipment.CollectionDate.Value.Minute, dbDirectShipment.CollectionDate.Value.Second);
                    }
                    dbDirectShipment.CustomerId = directBookingShippingDetail.CustomerId;
                    dbDirectShipment.FromAddressId = directBookingShippingDetail.ShipFrom.DirectShipmentAddressId;
                    dbDirectShipment.ToAddressId = directBookingShippingDetail.ShipTo.DirectShipmentAddressId;
                    dbDirectShipment.IsPODMailSent = false;
                    dbDirectShipment.OpearionZoneId = operationZone.OperationZoneId;
                    dbDirectShipment.ParcelType = directBookingShippingDetail.parcelType;
                    dbDirectShipment.PaymentPartyTaxAndDuties = directBookingShippingDetail.PayTaxAndDuties;
                    //dbDirectShipment.PaymentPartyTaxAndDutiesAccountNo = directBookingShippingDetail.PaymentPartyAccountNumber;
                    dbDirectShipment.TaxAndDutiesAcceptedBy = directBookingShippingDetail.PayTaxAndDuties;

                    dbDirectShipment.CreatedBy = directBookingShippingDetail.CustomerId;
                    dbDirectShipment.LastUpdated = DateTime.UtcNow;
                    dbDirectShipment.PackageCaculatonType = directBookingShippingDetail.PackageCalculationType;
                    //string FrayteNo = CommonConversion.GetNewFrayteNumber();
                    //if (FrayteNo != null || FrayteNo != "")
                    //{
                    dbDirectShipment.FrayteNumber = null;
                    directBookingShippingDetail.FrayteNumber = CommonConversion.GetNewFrayteNumber();
                    //}
                    dbDirectShipment.ModuleType = FrayteShipmentServiceType.DirectBooking;
                    //dbDirectShipment.AddressType = directBookingShippingDetail.AddressType;
                    dbContext.Entry(dbDirectShipment).State = System.Data.Entity.EntityState.Modified;
                }
            }

            if (dbDirectShipment != null)
            {
                dbContext.SaveChanges();
            }

            directBookingShippingDetail.DirectShipmentDraftId = dbDirectShipment.DirectShipmentDraftId;
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
                dbDirectShipment.CollectionDate = eCommerceUploadShipmentBookingDetail.CollectionDate != null && eCommerceUploadShipmentBookingDetail.CollectionDate != "" ? Convert.ToDateTime(eCommerceUploadShipmentBookingDetail.CollectionDate) : DateTime.MinValue.AddYears(1800);
                dbDirectShipment.CollectionTime = eCommerceUploadShipmentBookingDetail.CollectionTime != null && eCommerceUploadShipmentBookingDetail.CollectionTime != "" ? UtilityRepository.GetTimeFromString(eCommerceUploadShipmentBookingDetail.CollectionTime) : UtilityRepository.GetTimeFromString("0000");
                dbDirectShipment.CustomerId = eCommerceUploadShipmentBookingDetail.CustomerId;
                dbDirectShipment.FromAddressId = eCommerceUploadShipmentBookingDetail.ShipFrom.DirectShipmentAddressId;
                dbDirectShipment.ToAddressId = eCommerceUploadShipmentBookingDetail.ShipTo.DirectShipmentAddressId;
                dbDirectShipment.IsPODMailSent = false;
                dbDirectShipment.OpearionZoneId = operationZone.OperationZoneId;
                dbDirectShipment.ParcelType = eCommerceUploadShipmentBookingDetail.parcelType;
                dbDirectShipment.PaymentPartyTaxAndDuties = eCommerceUploadShipmentBookingDetail.PayTaxAndDuties;
                //dbDirectShipment.CreatedBy = eCommerceUploadShipmentBookingDetail.CreatedBy;
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
                dbDirectShipment.ServiceCode = eCommerceUploadShipmentBookingDetail.ServiceCode;
                dbDirectShipment.SessionId = eCommerceUploadShipmentBookingDetail.SessionId;
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
                    dbDirectShipment.CollectionDate = Convert.ToDateTime(eCommerceUploadShipmentBookingDetail.CollectionDate);
                    dbDirectShipment.CollectionTime = UtilityRepository.GetTimeFromString(eCommerceUploadShipmentBookingDetail.CollectionTime);
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
                    dbDirectShipment.ModuleType = FrayteShipmentServiceType.eCommerce;
                    //dbDirectShipment.EstimatedTimeofDelivery = UtilityRepository.GetTimeFromDateString(eCommerceUploadShipmentBookingDetail.EstimatedTimeofDelivery);
                    //dbDirectShipment.EstimatedDateofDelivery = eCommerceUploadShipmentBookingDetail.EstimatedDateofDelivery;
                    //dbDirectShipment.EstimatedTimeofArrival = UtilityRepository.GetTimeFromDateString(eCommerceUploadShipmentBookingDetail.EstimatedTimeofArrival);
                    //dbDirectShipment.EstimatedDateofArrival = eCommerceUploadShipmentBookingDetail.EstimatedDateofArrival;
                    dbDirectShipment.LogisticServiceType = eCommerceUploadShipmentBookingDetail.CourierCompany;
                    dbDirectShipment.BookingApp = ServiceType;
                    dbDirectShipment.TrackingDetail = eCommerceUploadShipmentBookingDetail.TrackingNo;
                    dbDirectShipment.ServiceCode = eCommerceUploadShipmentBookingDetail.ServiceCode;
                    dbDirectShipment.SessionId = eCommerceUploadShipmentBookingDetail.SessionId;
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

                //SaveeCommerceBarcode(dbShipment);

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
        //public void SaveeCommerceBarcode(eCommerceShipment shipment)
        //{
        //    string bar = string.Empty;
        //    bar = shipment.FrayteNumber;
        //    float total = 0;
        //    var piecesDetail = dbContext.eCommerceShipmentDetails.Where(p => p.eCommerceShipmentId == shipment.eCommerceShipmentId).ToList();
        //    if (piecesDetail != null && piecesDetail.Count > 0)
        //    {
        //        foreach (var d in piecesDetail)
        //        {
        //            total += (float)(d.Weight) * d.CartoonValue;
        //        }
        //    }

        //    var country = (from r in dbContext.eCommerceShipmentAddresses
        //                   join cc in dbContext.Countries on r.CountryId equals cc.CountryId
        //                   where r.eCommerceShipmentAddressId == shipment.ToAddressId
        //                   select new FrayteCountryCode
        //                   {
        //                       Code = cc.CountryCode,
        //                       Code2 = cc.CountryCode2,
        //                       CountryId = cc.CountryId,
        //                       Name = cc.CountryName
        //                   }
        //                   ).FirstOrDefault();
        //    if (country != null)
        //    {
        //        bar = bar + "|" + country.Code;
        //    }
        //    bar = bar + "|" + total.ToString();

        //    BarcodeSettings settings = new BarcodeSettings();
        //    string data = string.Empty;
        //    string type = "Code128";
        //    short fontSize = 8;
        //    string font = "SimSun";
        //    data = bar;

        //    settings.Data2D = data;
        //    settings.Data = data;
        //    settings.Type = (BarCodeType)Enum.Parse(typeof(BarCodeType), type);

        //    if (fontSize != 0 && fontSize.ToString().Length > 0 && Int16.TryParse(fontSize.ToString(), out fontSize))
        //    {
        //        if (font != null && font.Length > 0)
        //        {
        //            settings.TextFont = new System.Drawing.Font(font, fontSize, FontStyle.Bold);
        //        }
        //    }
        //    short barHeight = 15;
        //    if (barHeight != 0 && barHeight.ToString().Length > 0 && Int16.TryParse(barHeight.ToString(), out barHeight))
        //    {
        //        settings.BarHeight = barHeight;
        //    }

        //    BarCodeGenerator generator = new BarCodeGenerator(settings);
        //    System.Drawing.Image barcode = generator.GenerateImage();


        //    // Path where we will have barcode 
        //    string filePathToSave = AppSettings.eCommerceLabelFolder;
        //    if (AppSettings.ShipmentCreatedFrom == "BATCH")
        //    {
        //        filePathToSave = AppSettings.eCommerceUploadLabelFolder + shipment.eCommerceShipmentId;
        //        //filePathToSave = HttpContext.Current.Server.MapPath(filePathToSave + shipment.eCommerceShipmentId);

        //    }
        //    else
        //    {

        //        filePathToSave = HttpContext.Current.Server.MapPath(filePathToSave + shipment.eCommerceShipmentId);
        //    }

        //    if (!System.IO.Directory.Exists(filePathToSave))
        //        System.IO.Directory.CreateDirectory(filePathToSave);

        //    if (AppSettings.ShipmentCreatedFrom == "BATCH")
        //    {
        //        barcode.Save(AppSettings.eCommerceUploadLabelFolder + shipment.eCommerceShipmentId + "/" + shipment.FrayteNumber + ".Png");

        //    }
        //    else
        //    {
        //        barcode.Save(System.Web.Hosting.HostingEnvironment.MapPath(AppSettings.eCommerceLabelFolder + shipment.eCommerceShipmentId + "/" + shipment.FrayteNumber + ".Png"));
        //    }
        //    //Save  ShipmentBarCode
        //    shipment.BarCodeNumber = settings.Data;
        //    dbContext.Entry(shipment).State = System.Data.Entity.EntityState.Modified;
        //    dbContext.SaveChanges();
        //}
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

        public static Logger Get_Log()
        {
            Logger log = BaseLog.Instance.GetLogger(null);
            return log;
        }

        public List<FrayteUploadshipment> SaveShipmentWithService(string res)
        {
            //string xsUserFolder = @"C:\FMS\" + "FrayteSchedularlog.txt";
            string xsUserFolder = @"D:\ProjectFrayte\" + "FrayteScheduler.txt";
            BaseLog.Instance.SetLogFile(xsUserFolder);
            Logger _log = Get_Log();
            _log.Error("Enter in save Shipment");
            var ShipmentIdArray = res.Split(',');
            var DirectShipmentDraftIds = ShipmentIdArray[0].Split('|');
            var c = Convert.ToInt64(ShipmentIdArray[1]);
            _log.Error("Enter in save Shipment2");
            //var b = dbContext.CountryLogistics.Where(aa => aa.CountryLogisticId == c).FirstOrDefault();
            //List<int> DirectShipmentDraftIds = new List<int>();
            List<FrayteUploadshipment> Shipment = new List<FrayteUploadshipment>();
            var ListCount = 0;
            foreach (var DirectShipmentDraftId in DirectShipmentDraftIds)
            {
                try
                {
                    _log.Error("Enter in save Shipment3");
                    var result = GetShipmentErrors(Convert.ToInt32(DirectShipmentDraftId), "DirectBooking_SS");
                    Shipment.Add(result[0]);
                    _log.Error(Shipment[0].DirectShipmentDraftId.ToString());
                    _log.Error("Enter in save Shipment4");
                    //Step 1: Get/Set Operation Zone Detail
                    FrayteOperationZone OperationZone = UtilityRepository.GetOperationZone();

                    //Get OperationZone Wise Exchange Rate
                    var exchangeRate = (from OZ in dbContext.OperationZones
                                        join OZER in dbContext.OperationZoneExchangeRates on OZ.OperationZoneId equals OZER.OperationZoneId
                                        where
                                            OZ.OperationZoneId == OperationZone.OperationZoneId &&
                                            OZER.ExchangeType == FrayteOperationZoneExchangeType.Sell &&
                                            OZER.CurrencyCode == OZ.Currency
                                        select new
                                        {
                                            OZ.Currency,
                                            OZER.ExchangeRate
                                        }).FirstOrDefault();

                    var CustId = Shipment[ListCount].CustomerId;

                    //Get Exchange Rate
                    var customercurrency = (from US in dbContext.Users
                                            join UA in dbContext.UserAdditionals on US.UserId equals UA.UserId
                                            join OZER in dbContext.OperationZoneExchangeRates on UA.CreditLimitCurrencyCode equals OZER.CurrencyCode
                                            where
                                                US.UserId == CustId &&
                                                OZER.OperationZoneId == OperationZone.OperationZoneId &&
                                                OZER.ExchangeType == FrayteOperationZoneExchangeType.Sell
                                            select new
                                            {
                                                UA.CreditLimitCurrencyCode,
                                                OZER.ExchangeRate
                                            }).FirstOrDefault();
                    var TotalWeight = 0.0m;
                    foreach (var Weight in result[0].Package)
                    {
                        TotalWeight = TotalWeight + (Weight.CartoonValue * Weight.Weight);
                    }

                    var DocType = "";
                    var ParcelType = "";
                    var OperationZoneName = "";

                    if (Shipment[ListCount].ShipFrom.Country.Code == Shipment[ListCount].ShipTo.Country.Code && Shipment[ListCount].parcelType == "Parcel")
                    {
                        ParcelType = "Multiple";
                        DocType = null;
                    }
                    else if (Shipment[ListCount].ShipFrom.Country.Code == Shipment[ListCount].ShipTo.Country.Code && Shipment[ListCount].parcelType == "Letter")
                    {
                        ParcelType = "Single";
                        DocType = null;
                    }
                    else if (Shipment[ListCount].ShipFrom.Country.Code != Shipment[ListCount].ShipTo.Country.Code && TotalWeight > 2)
                    {
                        //DocType = "Nondoc";
                        ParcelType = null;
                    }
                    else if (Shipment[ListCount].ShipFrom.Country.Code != Shipment[ListCount].ShipTo.Country.Code && TotalWeight <= 2)
                    {
                        //DocType = "Doc";
                        ParcelType = null;
                    }

                    if (Shipment[ListCount].OpearionZoneId == 1)
                    {
                        OperationZoneName = "HKG";
                    }
                    else if (Shipment[ListCount].OpearionZoneId == 2)
                    {
                        OperationZoneName = "GBR";
                    }

                    var GetService = dbContext.spGet_UploadShipmentDirectBookingServices(result[0].DirectShipmentDraftId, result[0].ServiceCode, TotalWeight, result[0].OpearionZoneId, result[0].ShipFrom.Country.CountryId, result[0].ShipFrom.Country.Code,
                         result[0].ShipTo.Country.CountryId, result[0].ShipTo.Country.Code, result[0].ShipFrom.PostCode, result[0].ShipTo.PostCode, ParcelType, result[0].CustomerId, DateTime.Now, OperationZone.OperationZoneName, result[0].AddressType).ToList();
                    _log.Error(GetService.FirstOrDefault().LogisticCompany.ToString());
                    if (GetService.Count > 0 && GetService[0].LogisticServiceBaseRateCardId > 0)
                    {
                        _log.Error("we got service");
                        Shipment[ListCount].CustomerRateCard = new DirectBookingService();
                        //Shipment[0].CustomerRateCard.BaseRate = GetService[0].BaseRate;
                        Shipment[ListCount].CustomerRateCard.CourierAccountCountryCode = GetService[0].CourierAccountCountryCode;
                        Shipment[ListCount].CustomerRateCard.CourierAccountNo = GetService[0].CourierAccountNo;
                        Shipment[ListCount].CustomerRateCard.CourierDescription = GetService[0].CourierDescription;
                        Shipment[ListCount].CustomerRateCard.IntegrationAccountId = GetService[0].IntegrationAccountId;
                        Shipment[ListCount].CustomerRateCard.CourierName = GetService[0].LogisticCompany;
                        Shipment[ListCount].CustomerRateCard.DisplayName = GetService[0].LogisticCompanyDisplay;
                        Shipment[ListCount].CustomerRateCard.CurrencyCode = GetService[0].LogisticCurrency;
                        Shipment[ListCount].CustomerRateCard.LogisticDescription = GetService[0].LogisticDescription;
                        //Shipment[0].CustomerRateCard.Rate = GetService[0].LogisticRate.Value;
                        Shipment[ListCount].CustomerRateCard.ZoneRateCardId = GetService[0].LogisticServiceBaseRateCardId.Value;
                        Shipment[ListCount].CustomerRateCard.CourierAccountId = GetService[0].LogisticServiceCourierAccountId.Value;
                        Shipment[ListCount].CustomerRateCard.LogisticServiceId = GetService[0].LogisticServiceId.Value;
                        Shipment[ListCount].CustomerRateCard.LogisticShipmentId = GetService[0].LogisticServiceShipmentTypeId.Value;
                        Shipment[ListCount].CustomerRateCard.LogisticType = GetService[0].LogisticType;
                        //ShipmenListCount[0].CustomerRateCard. = GetService[0].LogisticTypeDisplay;
                        Shipment[ListCount].CustomerRateCard.MarginPercent = GetService[0].MarginPercent.Value;
                        Shipment[ListCount].CustomerRateCard.PakageType = GetService[0].PackageType;
                        Shipment[ListCount].CustomerRateCard.ParcelServiceType = GetService[0].ParcelType;
                        Shipment[ListCount].CustomerRateCard.RateType = GetService[0].RateType;
                        Shipment[ListCount].CustomerRateCard.RateTypeDisplay = GetService[0].RateTypeDisplay;
                        Shipment[ListCount].CustomerRateCard.UnitOfMeasurement = GetService[0].UOM;
                        Shipment[ListCount].CustomerRateCard.Weight = GetService[0].UserWeight.Value;
                        if (exchangeRate.Currency == GetService[0].LogisticCurrency)
                        {
                            //Step 1.1: Convert Business unit rate into Customer currency rate
                            if (customercurrency != null)
                            {
                                decimal baserate = GetService[0].BaseRate.Value * customercurrency.ExchangeRate;
                                Shipment[ListCount].CustomerRateCard.Rate = Math.Round(baserate + (baserate * GetService[0].MarginPercent.Value) / 100, 2);
                                Shipment[ListCount].CustomerRateCard.BaseRate = Math.Round((GetService[0].BaseRate.Value * customercurrency.ExchangeRate), 2);
                                Shipment[ListCount].CustomerRateCard.AdditionalSurcharge = (float)Math.Round((GetService[0].AddOnRate == null ? 0.0m : GetService[0].AddOnRate.Value) * customercurrency.ExchangeRate, 2);
                            }
                            else
                            {
                                Shipment[ListCount].CustomerRateCard.Rate = Math.Round(GetService[0].BaseRate.Value + (GetService[0].BaseRate.Value * GetService[0].MarginPercent.Value) / 100, 2);
                                Shipment[ListCount].CustomerRateCard.BaseRate = Math.Round(GetService[0].BaseRate.Value, 2);
                                Shipment[ListCount].CustomerRateCard.AdditionalSurcharge = (float)Math.Round((GetService[0].AddOnRate == null ? 0.0m : GetService[0].AddOnRate.Value), 2);
                            }
                        }
                        else if (exchangeRate != null && customercurrency != null)
                        {
                            //Step 1.1: Convert baserate into Business unit rate
                            decimal businessrate = GetService[0].BaseRate.Value / exchangeRate.ExchangeRate;

                            //Step 1.2: Convert Business unit rate into Customer currency rate
                            decimal baserate = businessrate * customercurrency.ExchangeRate;
                            Shipment[ListCount].CustomerRateCard.Rate = Math.Round(baserate + (baserate * GetService[0].MarginPercent.Value) / 100, 2);
                            Shipment[ListCount].CustomerRateCard.BaseRate = Math.Round(((GetService[0].BaseRate.Value / exchangeRate.ExchangeRate) * customercurrency.ExchangeRate), 2);
                            Shipment[ListCount].CustomerRateCard.AdditionalSurcharge = (float)Math.Round(((GetService[0].AddOnRate == null ? 0.0m : GetService[0].AddOnRate.Value) / exchangeRate.ExchangeRate) * customercurrency.ExchangeRate, 2);
                        }
                        else if (exchangeRate == null && customercurrency != null)
                        {
                            //Step 1.1: Convert rate into Customer currency rate
                            decimal baserate = GetService[0].BaseRate.Value * customercurrency.ExchangeRate;
                            Shipment[ListCount].CustomerRateCard.Rate = Math.Round(baserate + (baserate * GetService[0].MarginPercent.Value) / 100, 2);
                            Shipment[ListCount].CustomerRateCard.BaseRate = Math.Round(GetService[0].BaseRate.Value * customercurrency.ExchangeRate, 2);
                            Shipment[ListCount].CustomerRateCard.AdditionalSurcharge = (float)Math.Round((GetService[0].AddOnRate == null ? 0.0m : GetService[0].AddOnRate.Value) * customercurrency.ExchangeRate, 2);
                        }
                        else if (exchangeRate != null && customercurrency == null)
                        {
                            //Step 1.1: Convert baserate into Business unit rate 
                            decimal businessrate = GetService[0].BaseRate.Value / exchangeRate.ExchangeRate;
                            Shipment[ListCount].CustomerRateCard.Rate = Math.Round(businessrate + (businessrate * GetService[0].MarginPercent.Value) / 100, 2);
                            Shipment[ListCount].CustomerRateCard.BaseRate = Math.Round(GetService[0].BaseRate.Value / exchangeRate.ExchangeRate, 2);
                            Shipment[ListCount].CustomerRateCard.AdditionalSurcharge = (float)Math.Round((GetService[0].AddOnRate == null ? 0.0m : GetService[0].AddOnRate.Value) / exchangeRate.ExchangeRate, 2);
                        }

                    }
                    //else
                    //{
                    //    var Id1 = Convert.ToInt32(DirectShipmentDraftId);
                    //    var DirectShipment1 = dbContext.DirectShipmentDrafts.Where(a => a.DirectShipmentDraftId == Id1).FirstOrDefault();
                    //    new eCommerceUploadShipmentRepository().SaveBatchProcessUnprocessedShipment(1, DirectShipment1.CustomerId);
                    //}

                    var Id = Convert.ToInt32(DirectShipmentDraftId);
                    var DirectShipment = dbContext.DirectShipmentDrafts.Where(a => a.DirectShipmentDraftId == Id).FirstOrDefault();
                    if (DirectShipment != null)
                    {
                        //DirectShipment.IsSelectServiceStatus = false;
                        dbContext.Entry(DirectShipment).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }

                }
                catch (Exception e)
                {
                    // continue;
                }
                ListCount++;
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
            var resultnew = dbContext.DirectShipmentDetailDrafts.Where(ab => ab.DirectShipmentDraftId == ShipmentId).ToList();
            if (resultnew.Count > 0)
            {
                foreach (var res in resultnew)
                {
                    dbContext.DirectShipmentDetailDrafts.Remove(res);
                    dbContext.SaveChanges();
                }
            }

            if (result != null)
            {
                var result1 = dbContext.ShipmentCustomDetailDrafts.Where(a => a.ShipmentDraftId == result.DirectShipmentDraftId).FirstOrDefault();
                if (result1 != null)
                {
                    dbContext.ShipmentCustomDetailDrafts.Remove(result1);
                    dbContext.SaveChanges();
                }
            }

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

        public DirectBookingShipmentDraftDetail DirectBookingObj(FrayteUploadshipment frayteShipmentRequest)
        {
            DirectBookingShipmentDraftDetail directBookingDetail = new DirectBookingShipmentDraftDetail();
            // customerfrom = GetCustomerDetail(frayteShipmentRequest.Security.AccountNumber);
            var country = GetCountry(frayteShipmentRequest.ShipFrom.Country);
            directBookingDetail.ShipFrom = new DirectBookingDraftCollection()
            {
                //CustomerName = customerfrom.CustomerName,
                AddressBookId = frayteShipmentRequest.ShipFrom.DirectShipmentAddressId,
                CompanyName = frayteShipmentRequest.ShipFrom.CompanyName,
                Address = frayteShipmentRequest.ShipFrom.Address,
                Address2 = frayteShipmentRequest.ShipFrom.Address2,
                City = frayteShipmentRequest.ShipFrom.City,
                Email = frayteShipmentRequest.ShipFrom.Email,
                PostCode = frayteShipmentRequest.ShipFrom.PostCode,
                State = frayteShipmentRequest.ShipFrom.State,
                Area = frayteShipmentRequest.ShipFrom.Area,
                Phone = frayteShipmentRequest.ShipFrom.Phone,
                CurrencyCode = frayteShipmentRequest.CurrencyCode,

                Country = new FrayteCountryCode()
                {
                    Code = country.CountryCode,
                    Code2 = country.CountryCode2,
                    CountryId = country.CountryId,
                    Name = country.CountryName,
                    TimeZoneDetail = null
                },
                CustomerId = frayteShipmentRequest.CustomerId,
                FirstName = frayteShipmentRequest.ShipFrom.FirstName,
                LastName = frayteShipmentRequest.ShipFrom.LastName,
                IsFavorites = true,
                IsMailSend = false,
            };
            var country1 = GetCountry(frayteShipmentRequest.ShipTo.Country);
            directBookingDetail.DirectShipmentDraftId = frayteShipmentRequest.DirectShipmentDraftId;
            directBookingDetail.ShipmentStatusId = frayteShipmentRequest.ShipmentStatusId;
            directBookingDetail.PakageCalculatonType = frayteShipmentRequest.PackageCalculationType;
            directBookingDetail.CustomerId = frayteShipmentRequest.CustomerId;
            directBookingDetail.SessionId = frayteShipmentRequest.SessionId;
            directBookingDetail.CreatedBy = frayteShipmentRequest.UserId;
            directBookingDetail.ShipTo = new DirectBookingDraftCollection()
            {
                //CustomerName = customerfrom.CustomerName,
                CompanyName = frayteShipmentRequest.ShipTo.CompanyName,
                Address = frayteShipmentRequest.ShipTo.Address,
                Address2 = frayteShipmentRequest.ShipTo.Address2,
                City = frayteShipmentRequest.ShipTo.City,
                Email = frayteShipmentRequest.ShipTo.Email,
                PostCode = frayteShipmentRequest.ShipTo.PostCode,
                State = frayteShipmentRequest.ShipTo.State,
                Area = frayteShipmentRequest.ShipTo.Area,
                Phone = frayteShipmentRequest.ShipTo.Phone,
                CurrencyCode = frayteShipmentRequest.CurrencyCode,
                Country = new FrayteCountryCode()
                {
                    Code = country1.CountryCode,
                    Code2 = country1.CountryCode2,
                    CountryId = country1.CountryId,
                    Name = country1.CountryName,
                    TimeZoneDetail = null
                },
                CustomerId = frayteShipmentRequest.CustomerId,
                FirstName = frayteShipmentRequest.ShipTo.FirstName,
                LastName = frayteShipmentRequest.ShipTo.LastName,
                AddressBookId = frayteShipmentRequest.ShipTo.DirectShipmentAddressId,
                IsMailSend = false,
            };
            directBookingDetail.Currency = new CurrencyType()
            {
                CurrencyCode = frayteShipmentRequest.CurrencyCode,
                CurrencyDescription = "",
            };
            if (frayteShipmentRequest.CustomerRateCard != null && frayteShipmentRequest.CustomerRateCard.CourierName == "TNT")
            {
                frayteShipmentRequest.PayTaxAndDuties = "Shipper";
            }
            directBookingDetail.PayTaxAndDuties = frayteShipmentRequest.PayTaxAndDuties;
            directBookingDetail.Packages = new List<PackageDraft>();
            foreach (var package in frayteShipmentRequest.Package)
            {
                var item = new PackageDraft()
                {
                    CartoonValue = package.CartoonValue,
                    Height = Convert.ToDecimal(package.Height),
                    Length = Convert.ToDecimal(package.Length),
                    Width = Convert.ToDecimal(package.Width),
                    Content = package.Content,
                    Weight = Convert.ToDecimal(package.Weight),
                    Value = package.Value
                };
                directBookingDetail.Packages.Add(item);
            };
            directBookingDetail.ParcelType = new FrayteParcelType()
            {
                ParcelType = frayteShipmentRequest.parcelType,
                ParcelDescription = ""
            };
            if (directBookingDetail.ShipFrom.Country.Code != directBookingDetail.ShipTo.Country.Code)
            {
                directBookingDetail.CustomInfo = new Services.Models.CustomInformation()
                {
                    ContentsType = frayteShipmentRequest.CustomInfo.ContentsType,
                    RestrictionType = frayteShipmentRequest.CustomInfo.RestrictionType,
                    ContentsExplanation = frayteShipmentRequest.CustomInfo.ContentsExplanation,
                    RestrictionComments = frayteShipmentRequest.CustomInfo.RestrictionComments,
                    CustomsCertify = true,
                    CustomsSigner = frayteShipmentRequest.CustomInfo.CustomsSigner,
                    NonDeliveryOption = frayteShipmentRequest.CustomInfo.NonDeliveryOption,
                };
            }
            else
            {
                directBookingDetail.CustomInfo = new Services.Models.CustomInformation()
                {
                    TermOfTrade = frayteShipmentRequest.PayTaxAndDuties,

                };
            }
            directBookingDetail.ReferenceDetail = new ReferenceDetail();
            /*directBookingDetail.ReferenceDetail.CollectionDate = Convert.ToDateTime(frayteShipmentRequest.EstimatedDateofDelivery)*/
            ;
            directBookingDetail.ReferenceDetail.CollectionDate = frayteShipmentRequest.CollectionDate != null && frayteShipmentRequest.CollectionDate != "" ? Convert.ToDateTime(frayteShipmentRequest.CollectionDate) : DateTime.MinValue;
            if (frayteShipmentRequest.CustomerRateCard != null && frayteShipmentRequest.CustomerRateCard.CourierName == "TNT")
            {
                frayteShipmentRequest.CollectionTime = frayteShipmentRequest.CollectionTime.Split(':')[0] + ':' + frayteShipmentRequest.CollectionTime.Split(':')[1];
            }
            if (frayteShipmentRequest.CustomerRateCard != null && frayteShipmentRequest.CustomerRateCard.CourierName == "DHL")
            {
                frayteShipmentRequest.CollectionTime = frayteShipmentRequest.CollectionTime.Split(':')[0] + frayteShipmentRequest.CollectionTime.Split(':')[1];
            }
            directBookingDetail.ReferenceDetail.CollectionTime = frayteShipmentRequest.CollectionTime;
            directBookingDetail.ReferenceDetail.ContentDescription = frayteShipmentRequest.ShipmentDescription;
            directBookingDetail.ReferenceDetail.Reference1 = frayteShipmentRequest.ShipmentReference;
            directBookingDetail.CustomerRateCard = new DirectBookingService()
            {
                ZoneRateCardId = frayteShipmentRequest.CustomerRateCard.ZoneRateCardId,
                CustomerId = frayteShipmentRequest.CustomerId,
                WeightType = frayteShipmentRequest.CustomerRateCard.WeightType,
                LogisticServiceId = frayteShipmentRequest.CustomerRateCard.LogisticServiceId,
                Rate = frayteShipmentRequest.CustomerRateCard.Rate,
                BaseRate = frayteShipmentRequest.CustomerRateCard.BaseRate,
                MarginPercent = frayteShipmentRequest.CustomerRateCard.MarginPercent,
                CourierName = frayteShipmentRequest.CustomerRateCard.CourierName,
                DisplayName = frayteShipmentRequest.CustomerRateCard.DisplayName,
                CourierAccountId = frayteShipmentRequest.CustomerRateCard.CourierAccountId,
                CourierAccountNo = frayteShipmentRequest.CustomerRateCard.CourierAccountNo,
                CourierDescription = frayteShipmentRequest.CustomerRateCard.CourierDescription,
                CourierAccountCountryCode = frayteShipmentRequest.CustomerRateCard.CourierAccountCountryCode,
                IntegrationAccountId = frayteShipmentRequest.CustomerRateCard.IntegrationAccountId,
                LogisticType = frayteShipmentRequest.CustomerRateCard.LogisticType,
                LogisticShipmentId = frayteShipmentRequest.CustomerRateCard.LogisticShipmentId,
                LogisticDescription = frayteShipmentRequest.CustomerRateCard.LogisticDescription,
                LogisticServiceType = frayteShipmentRequest.CustomerRateCard.LogisticServiceType,
                UnitOfMeasurement = frayteShipmentRequest.CustomerRateCard.UnitOfMeasurement,
                TotalEstimatedCharge = frayteShipmentRequest.CustomerRateCard.TotalEstimatedCharge,
                CurrencyCode = frayteShipmentRequest.CustomerRateCard.CurrencyCode,
                CustomerCurrency = frayteShipmentRequest.CustomerRateCard.CustomerCurrency,
                RateType = frayteShipmentRequest.CustomerRateCard.RateType,
                RateTypeDisplay = frayteShipmentRequest.CustomerRateCard.RateTypeDisplay,
                PackageCalculationType = frayteShipmentRequest.CustomerRateCard.PackageCalculationType,
                Weight = Convert.ToDecimal(frayteShipmentRequest.Package.FirstOrDefault().Weight),
                Length = Convert.ToDecimal(frayteShipmentRequest.Package.FirstOrDefault().Length),
                Width = Convert.ToDecimal(frayteShipmentRequest.Package.FirstOrDefault().Width),
                Height = Convert.ToDecimal(frayteShipmentRequest.Package.FirstOrDefault().Height),
                TransitTime = frayteShipmentRequest.CustomerRateCard.TransitTime,
                FuelDate = DateTime.Now.AddDays(5),
                FuelMonth = "feb",
                FuelSurcharge = 10.00f
            };
            return directBookingDetail;
        }

        public List<ContentTypeModel> ContentTypeList()
        {
            List<ContentTypeModel> ContentType = new List<ContentTypeModel>() {
                new ContentTypeModel() { Id = 1, Name = "Documents", Value = "documents" },
                new ContentTypeModel() { Id = 2, Name = "Gift", Value = "gift" },
                new ContentTypeModel() { Id = 3, Name = "Merchandise", Value = "merchandise" },
                new ContentTypeModel() { Id = 4, Name = "Returned Goods", Value = "returned_goods" },
                new ContentTypeModel() { Id = 5, Name = "Sample", Value = "sample" },
                new ContentTypeModel() { Id = 6, Name = "Other", Value = "other" }
            };
            return ContentType;
        }

        public List<RestrictionTypeModel> RestrictionTypeList()
        {
            List<RestrictionTypeModel> RestrictionType = new List<RestrictionTypeModel>() {
                new RestrictionTypeModel() { Id = 1, Name = "None", Value = "none" },
                new RestrictionTypeModel() { Id = 2, Name = "Other", Value = "other" },
                new RestrictionTypeModel() { Id = 3, Name = "Quarantine", Value = "quarantine" },
                new RestrictionTypeModel() { Id = 4, Name = "Sanitary Phytosanitary Inspection", Value = "sanitary_phytosanitary_inspection" }
            };
            return RestrictionType;
        }

        public List<NonDeliveryOptionModel> NonDeliveryOptionList()
        {
            List<NonDeliveryOptionModel> NonDeliveryOption = new List<NonDeliveryOptionModel>() {
                new NonDeliveryOptionModel() { Id = 1, Name = "By Default Abandon", Value = "abandon" },
                new NonDeliveryOptionModel() { Id = 2, Name = "Return to Sender", Value = "return" }
            };
            return NonDeliveryOption;
        }

        public List<ItemCatogoryModel> ItemCatogoryList()
        {
            List<ItemCatogoryModel> ItemCatogory = new List<ItemCatogoryModel>() {
                new ItemCatogoryModel() { Id = 1, Name = "Sold", Value = "sold" },
                new ItemCatogoryModel() { Id = 2, Name = "Gift", Value = "gift" },
                new ItemCatogoryModel() { Id = 3, Name = "Documents", Value = "documents" },
                new ItemCatogoryModel() { Id = 4, Name = "Returned Goods", Value = "returned_goods" },
                new ItemCatogoryModel() { Id = 5, Name = "Samples", Value = "samples" },
                new ItemCatogoryModel() { Id = 6, Name = "Commercial Sample", Value = "Commercial Sample" }
            };
            return ItemCatogory;
        }

        public List<FrayteServiceCode> GetLogisticServiceCode(int OperationZoneId, int CustomerId)
        {
            List<FrayteServiceCode> _code = new List<FrayteServiceCode>();
            if (CustomerId > 0)
            {
                var service = dbContext.spGet_LogisticServiceCode(OperationZoneId, CustomerId).ToList();
                if (service != null)
                {
                    FrayteServiceCode code;
                    foreach (var Obj in service)
                    {
                        code = new FrayteServiceCode();
                        code.LogisticCompany = Obj.LogisticCompany;
                        code.LogisticCompanyDisplay = Obj.LogisticCompanyDisplay;
                        code.LogisticDescription = Obj.LogisticDescription;
                        code.ServiceCode = Obj.LogisticServiceShipmentcode;
                        code.WeightFrom = Obj.WeightFrom.Value;
                        code.WeightTo = Obj.WeightTo.Value;
                        code.LogisticType = Obj.LogisticType == null || Obj.LogisticType == "" ? "" : Obj.LogisticType;
                        code.RateType = Obj.RateType == null || Obj.RateType == "" ? "" : Obj.RateType;
                        _code.Add(code);
                    }
                }
            }
            return _code;
        }

        public String ConvertStringToTime(string CollectionTime)
        {
            try
            {
                if (!string.IsNullOrEmpty(CollectionTime) && CollectionTime.Contains(":"))
                {
                    DateTime dtCheck = DateTime.MinValue;
                    TimeSpan dtTimeCheck = TimeSpan.Zero;
                    DateTime.TryParse(CollectionTime, out dtCheck);
                    TimeSpan.TryParse(CollectionTime, out dtTimeCheck);

                    string hour = "";
                    string minutes = "";

                    if (dtTimeCheck != TimeSpan.Zero)
                    {
                        CollectionTime = dtCheck.TimeOfDay.ToString("HHmm");
                    }
                    else if (dtCheck != DateTime.MinValue)
                    {
                        CollectionTime = dtCheck.ToString("HHmm");
                    }
                    else
                    {
                        if (CollectionTime.ToUpper().Contains("AM"))
                        {
                            CollectionTime = null;
                        }
                        else if (CollectionTime.ToUpper().Contains("PM"))
                        {
                            CollectionTime = CollectionTime.Replace("PM", "");
                            var Time1 = CollectionTime.Split(':');

                            if (Time1.Length > 0)
                            {
                                hour = Time1[0];
                                if (Time1.Length > 1)
                                {
                                    minutes = Time1[1];
                                    CollectionTime = hour + minutes;
                                }
                                else
                                {
                                    CollectionTime = null;
                                }
                            }
                            else
                            {
                                CollectionTime = null;
                            }
                        }
                        else if (CollectionTime.Contains(":"))
                        {
                            var Time1 = CollectionTime.Split(':');
                            if (Time1.Length > 0)
                            {
                                hour = Time1[0];
                                int hr = 0;
                                int.TryParse(hour, out hr);
                                if (hour != "00" && hr == 0)
                                {
                                    CollectionTime = null;
                                }
                                else if (hr > 23)
                                {
                                    CollectionTime = null;
                                }
                                else if (Time1.Length > 1)
                                {
                                    minutes = Time1[1];
                                    int mnt = 0;
                                    int.TryParse(minutes, out mnt);
                                    if (mnt > 59)
                                    {
                                        CollectionTime = null;
                                    }
                                    else
                                    {
                                        CollectionTime = hour.ToString() + mnt.ToString();
                                    }
                                }
                                else
                                {
                                    CollectionTime = null;
                                }
                            }
                            else
                            {
                                CollectionTime = null;
                            }
                        }
                        else
                        {
                            CollectionTime = null;
                        }
                    }


                    //var TimeArray = CollectionTime.Split(' ');
                    //var Time = TimeArray[1].Split(':');

                    //if (TimeArray[2].Contains("PM"))
                    //{
                    //    int TimeArr = Convert.ToInt32(Time[0]) + 12;
                    //    CollectionTime = TimeArr.ToString() + Time[1];
                    //}
                    //else if (CollectionTime.Contains("AM"))
                    //{
                    //    CollectionTime = "0" + Time[0] + Time[1];
                    //}
                    //else if (!CollectionTime.Contains("PM") && !CollectionTime.Contains("AM"))
                    //{
                    //    CollectionTime = Time[0] + Time[1];
                    //}

                }
                else if (!string.IsNullOrEmpty(CollectionTime) && !CollectionTime.Contains(":"))
                {
                    CollectionTime = null;
                }
                else if (string.IsNullOrEmpty(CollectionTime))
                {
                    CollectionTime = null;
                }
            }
            catch (Exception ex)
            {
                CollectionTime = null;
            }

            return CollectionTime;
        }

        public String ConvertStringToDate(string CollectionDate)
        {

            if (!string.IsNullOrEmpty(CollectionDate) && CollectionDate.Contains("/"))
            {
                var DateArray1 = CollectionDate.Split(' ');
                var DateArray = DateArray1[0].Split('/');
                if (Convert.ToInt32(DateArray[0]) < 13)
                {
                    CollectionDate = DateArray[0] + "/" + DateArray[1] + "/" + DateArray[2];
                }
                else
                {
                    return "";
                }
            }
            else if (!string.IsNullOrEmpty(CollectionDate) && CollectionDate.Contains("-"))
            {
                var DateArray = CollectionDate.Split('-');
                if (Convert.ToInt32(DateArray[0]) < 13)
                {
                    CollectionDate = DateArray[0] + "/" + DateArray[1] + "/" + DateArray[2];
                }
                else
                {
                    return "";
                }
            }
            else if (string.IsNullOrEmpty(CollectionDate))
            {
                CollectionDate = null;
            }
            return CollectionDate;
        }

        public HttpResponseMessage DownloadCustomManifest(string fileName)
        {
            //string filePath = HttpContext.Current.Server.MapPath("~/Manifestedshipments/" + fileName);
            string filePath = "D:/ProjectFrayte/Frayte/Frayte.WebApi/pdfdownload / Manifestedshipments/" + fileName;
            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    ms.Write(bytes, 0, (int)file.Length);
                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                    httpResponseMessage.Content = new ByteArrayContent(bytes);
                    httpResponseMessage.Content.Headers.Add("download-status", "downloaded");
                    httpResponseMessage.Content.Headers.Add("x-filename", fileName);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = fileName;
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    return httpResponseMessage;
                }
            }
        }

        public List<string> getLabels(int SesionId)
        {
            List<string> str = new List<string>();
            //var result = (from a in dbContext.DirectShipments
            //              join b in dbContext.DirectShipmentDetails on a.DirectShipmentId equals b.DirectShipmentId
            //              join c in dbContext.PackageTrackingDetails on b.DirectShipmentDetailId equals c.DirectShipmentDetailId
            //              where a.SessionId == SesionId
            //              select new fileName
            //              {
            //                  FileName = c.PackageImage
            //              }).ToList();
            // Getting shipmentIds based on sessionid
            var result = (from a in dbContext.DirectShipments
                          where a.SessionId == SesionId
                          select new fileName
                          {
                              FileName = a.DirectShipmentId.ToString()
                          }).ToList();

            //Current Browser Download folder path
            //String path = String.Empty;
            //RegistryKey rKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main");
            //if (rKey != null)
            //    path = (String)rKey.GetValue("Default Download Directory");
            //if (String.IsNullOrEmpty(path))
            //    path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\downloads";
            string path = AppSettings.ClientFolderPath;
            //Create Directory if not exist and read and download pdf in client download folder.
            foreach (var a in result)
            {
                if (!Directory.Exists(path + '/' + a.FileName))
                {
                    var di = Directory.CreateDirectory(path + '/' + a.FileName);
                    string[] files = Directory.GetFiles(AppSettings.FilePath + Convert.ToInt32(Convert.ToInt32(a.FileName)));

                    if (files.Length > 0)
                    {

                        foreach (var ab in files.Where(file => file.ToUpper().Contains(".PDF") && file.ToUpper().Contains("ALL")))
                        {
                            var ac = ab.LastIndexOf('f');
                            var av = (ac + 1) - 56;
                            var st = ab.Substring(56, av);
                            str.Add(@di.FullName + '/' + st);
                            byte[] bytes = System.IO.File.ReadAllBytes(ab);
                            System.IO.File.WriteAllBytes(@di.FullName + '/' + st, bytes);
                        }
                    }
                }
                else
                {
                    string[] files = Directory.GetFiles(path + '/' + Convert.ToInt32(Convert.ToInt32(a.FileName)));

                    if (files.Length > 0)
                    {
                        foreach (var ab in files.Where(file => file.ToUpper().Contains(".PDF") && file.ToUpper().Contains("ALL")))
                        {
                            var ac = ab.LastIndexOf('f');
                            var av = (ac + 1) - 56;
                            var st = ab.Substring(56, av);
                            str.Add(@ab);
                        }
                    }
                }
            }


            return str;
        }

        public List<int> GetSessionIds()
        {
            var Result = dbContext.DirectBulkUploadSessions.Where(a => a.IsSessionPrint == true).Select(a => a.SessionId).ToList();
            return Result;
        }

        public bool SaveIsSessionPrint(int SessionId)
        {
            var r = false;
            var result = dbContext.DirectBulkUploadSessions.Where(a => a.SessionId == SessionId).FirstOrDefault();
            if (result != null)
            {
                result.IsSessionPrint = true;
                dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                r = true;
            }
            return r;
        }

        public bool SaveIsSessionPrintFalse(int SessionId)
        {
            var r = false;
            var result = dbContext.DirectBulkUploadSessions.Where(a => a.SessionId == SessionId).FirstOrDefault();
            if (result != null)
            {
                result.IsSessionPrint = false;
                dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                r = true;
            }
            return r;
        }


        /// <summary>
        /// Modification Done By Avinash
        /// </summary>
        /// <param name="ExpressId"></param>
        /// <returns></returns>
        /// 

        #region Avinash 17-Apr-2019

        //public FratyteError GetExpressFrayteError(int ExpressId)
        //{
        //    FratyteError errorResult = new FratyteError();

        //    var result = dbContext.Expresses.FirstOrDefault(t => t.ExpressId == ExpressId);
        //    if (result != null && ((result.EasyPostErrorObject != null && (result.EasyPostErrorObject.StartsWith("<") || result.EasyPostErrorObject.StartsWith("{")))
        //        || (result.EasyPostPickUpObject != null && (result.EasyPostPickUpObject.StartsWith("<") || result.EasyPostPickUpObject.StartsWith("{")))))
        //    {
        //        if (result.LogisticServiceType == FrayteCourierCompany.UKMail ||
        //           result.LogisticServiceType == FrayteCourierCompany.Yodel ||
        //           result.LogisticServiceType == FrayteCourierCompany.Hermes)
        //        {

        //            FratyteError Result = new FratyteError();
        //            if (!string.IsNullOrWhiteSpace(result.EasyPostPickUpObject))
        //            {

        //                result = jsonconvert.deserializeobject<fratyteerror>(result.easypostpickupobject);


        //            }
        //            if (!string.IsNullOrWhiteSpace(result.EasyPostErrorObject))
        //            {
        //                Result = JsonConvert.DeserializeObject<FratyteError>(result.EasyPostErrorObject);

        //            }
        //            errorResult = Result;
        //        }
        //        #region TNT  
        //        else if (result.PacakgeCalculationType == FrayteCourierCompany.TNT && !string.IsNullOrWhiteSpace(result.EasyPostPickUpObject))
        //        {

        //            FratyteError error = new FratyteError();
        //            TNTResponseDto TNTResponse = new TNTResponseDto();
        //            error.MiscErrors = new List<FrayteKeyValue>();
        //            // _log.Error("enter in else section");
        //            FrayteKeyValue er1;

        //            // Read error messages from xml  

        //            XDocument xml = XDocument.Parse(result.EasyPostPickUpObject);

        //            var list1 = (from r in xml.Descendants("parse_error")
        //                         select new
        //                         {
        //                             LineNumber = r.Element("error_line") != null ? r.Element("error_line").Value : "",
        //                             Reason = r.Element("error_reason") != null ? r.Element("error_reason").Value : "",
        //                             Source = r.Element("error_srcText") != null ? r.Element("error_srcText").Value : ""
        //                         }).ToList();

        //            // Log to elmah
        //            if (list1.Count > 0)
        //            {
        //                //_log.Error(list1[0].Source);
        //                dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(list1);
        //                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));

        //                er1 = new FrayteKeyValue();
        //                er1.Value = new List<string>();
        //                er1.Key = "Miscellaneous";
        //                er1.Value.Add("TNT server could not parse the request. Please contact the administrator.");
        //            }
        //            else
        //            {
        //                // There are validations errors
        //                var list = (from r in xml.Descendants("ERROR")
        //                            select new
        //                            {
        //                                ErrorCode = r.Element("CODE") != null ? r.Element("CODE").Value : "",
        //                                Description = r.Element("DESCRIPTION") != null ? r.Element("DESCRIPTION").Value : "",
        //                                Source = r.Element("SOURCE") != null ? r.Element("SOURCE").Value : ""
        //                            }).ToList();
        //                //_log.Error(list[0].Description);
        //                // Log to elmah
        //                if (list.Count > 0)
        //                {
        //                    dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
        //                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));
        //                }
        //                if (list.Count > 0)
        //                {

        //                    //_log.Error(list[0].Description);
        //                    // Step 1 : Get all address erros
        //                    var addressErrors = list.Where(p => p.Description.Contains("address") || p.Description.Contains("town")).ToList();

        //                    er1 = new FrayteKeyValue();
        //                    if (addressErrors.Count > 0)
        //                    {
        //                        er1.Value = new List<string>();
        //                        er1.Key = "Address";
        //                        er1.Value = new List<string>();
        //                        foreach (var data in addressErrors)
        //                        {
        //                            er1.Value.Add(data.Description);
        //                        }
        //                        if (er1.Value.Count > 0)
        //                            error.MiscErrors.Add(er1);

        //                        // remove from main list
        //                        foreach (var data in addressErrors)
        //                        {
        //                            list.Remove(data);
        //                        }
        //                    }
        //                    // Step 2 : Get all address erros
        //                    var packageErrors = list.Where(p => p.Description.Contains("Length") ||
        //                    p.Description.Contains("Weight") ||
        //                    p.Description.Contains("Width") ||
        //                    p.Description.Contains("Height")).ToList();

        //                    if (packageErrors.Count > 0)
        //                    {
        //                        er1 = new FrayteKeyValue();
        //                        er1.Key = "Package";
        //                        er1.Value = new List<string>();
        //                        foreach (var data in packageErrors)
        //                        {
        //                            er1.Value.Add(data.Description);
        //                        }
        //                        if (er1.Value.Count > 0)
        //                            error.MiscErrors.Add(er1);
        //                        // remove from main list
        //                        foreach (var data in packageErrors)
        //                        {
        //                            list.Remove(data);
        //                        }
        //                    }

        //                    // remaining errors  are off type miscellaneous
        //                    if (list.Count > 0)
        //                    {
        //                        er1 = new FrayteKeyValue();
        //                        er1.Key = "Miscellaneous";
        //                        er1.Value = new List<string>();
        //                        foreach (var data in list)
        //                        {
        //                            er1.Value.Add(data.Description);
        //                        }
        //                        if (er1.Value.Count > 0)
        //                            error.MiscErrors.Add(er1);
        //                    }
        //                }
        //            }
        //            error.Status = false;

        //            errorResult = error;
        //        }
        //        #endregion

        //        #region UPS
        //        else if (result.LogisticServiceType == FrayteCourierCompany.UPS && (!string.IsNullOrWhiteSpace(result.EasyPostPickUpObject) || !string.IsNullOrWhiteSpace(result.EasyPostErrorObject)))
        //        {

        //            var upsResult = new UPSShipmentResponseDto();
        //            var upsError = new UPSErrorDto();
        //            if (!string.IsNullOrWhiteSpace(result.EasyPostPickUpObject))
        //            {
        //                upsError = JsonConvert.DeserializeObject<UPSErrorDto>(result.EasyPostPickUpObject);
        //                var error = upsError.Fault.detail.Errors.ErrorDetail.PrimaryErrorCode.Description;
        //                upsResult.Error = new FratyteError();
        //                upsResult.Error.Service = new List<string>();
        //                upsResult.Error.Service.Add(error);

        //                upsResult.Error.Status = false;

        //            }
        //            if (!string.IsNullOrWhiteSpace(result.EasyPostErrorObject))
        //            {
        //                upsError = JsonConvert.DeserializeObject<UPSErrorDto>(result.EasyPostPickUpObject);
        //                var error = upsError.Fault.detail.Errors.ErrorDetail.PrimaryErrorCode.Description;
        //                upsResult.Error = new FratyteError();
        //                upsResult.Error.Service = new List<string>();
        //                upsResult.Error.Service.Add(error);

        //                upsResult.Error.Status = false;
        //            }
        //            errorResult = upsResult.Error;
        //        }
        //        #endregion

        //        #region DHL
        //        else if (result.LogisticServiceType == FrayteCourierCompany.DHL || result.LogisticServiceType == FrayteCourierCompany.DHLExpress)
        //        {
        //            if (!string.IsNullOrWhiteSpace(result.EasyPostPickUpObject))
        //            {
        //                var pickupxml = XDocument.Parse(result.EasyPostPickUpObject);
        //                var Error = (from r in pickupxml.Descendants("Condition")
        //                             select new FrayteKeyValue
        //                             {
        //                                 Key = r.Element("ConditionCode") != null ? r.Element("ConditionCode").Value : "",
        //                                 Value = new List<string> { r.Element("ConditionData") != null ? r.Element("ConditionData").Value : "" },
        //                             }).ToList();


        //                errorResult.MiscErrors = Error;
        //            }
        //            if (!string.IsNullOrWhiteSpace(result.EasyPostErrorObject))
        //            {
        //                var shimpmentxml = XDocument.Parse(result.EasyPostErrorObject);
        //                var Error = (from r in shimpmentxml.Descendants("Condition")
        //                             select new FrayteKeyValue
        //                             {
        //                                 Key = r.Element("ConditionCode") != null ? r.Element("ConditionCode").Value : "",
        //                                 Value = new List<string> { r.Element("ConditionData") != null ? r.Element("ConditionData").Value : "" },
        //                             }).ToList();


        //                errorResult.MiscErrors = Error;
        //            }

        //        }
        //        #endregion 
        //    }
        //    else
        //    {
        //        errorResult.MiscErrors = new List<FrayteKeyValue>();
        //        FrayteKeyValue fkv = new FrayteKeyValue();
        //        errorResult.MiscErrors.Add(fkv);
        //        errorResult.MiscErrors[0].Value = new List<string>();
        //        errorResult.MiscErrors.FirstOrDefault().Value.Add(result.EasyPostErrorObject);
        //    }
        //    return errorResult;
        //}

        #endregion

    }
}
