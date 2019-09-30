using Frayte.Services.Business;
using Frayte.Services.Utility;
using Frayte.Services.Models.Tradelane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using System.IO;
using DevExpress.XtraPrinting;
using Frayte.Services.Models.Express;
using Frayte.Services.DataAccess;
using Frayte.Services.Models.FrayteMAWB;

namespace Frayte.Services.Business
{

    public class FRAYTEAWBRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<FrayteOWNMAWB> GetFrayteMAWBPDFDataSource(int expressId, string moduleType)
        {
            if (moduleType == "EXS")
            {
                List<FrayteOWNMAWB> lstMAWB = new List<FrayteOWNMAWB>();
                var grossweight = (from ex in dbContext.Expresses
                                   join ed in dbContext.ExpressDetails on ex.ExpressId equals ed.ExpressId
                                   where ex.ExpressId == expressId
                                   select ed).ToList();

                var result = (from e in dbContext.Expresses
                              join fromadd in dbContext.ExpressAddresses on e.FromAddressId equals fromadd.ExpressAddressId
                              join c in dbContext.Countries on fromadd.CountryId equals c.CountryId
                              join toadd in dbContext.ExpressAddresses on e.ToAddressId equals toadd.ExpressAddressId
                              join cn in dbContext.Countries on toadd.CountryId equals cn.CountryId
                              join ed in dbContext.ExpressDetails on e.ExpressId equals ed.ExpressId
                              join u in dbContext.Users on e.CustomerId equals u.UserId
                              join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                              join ecd in dbContext.ExpressCustomDetails on e.ExpressId equals ecd.ShipmentId
                              join hcs in dbContext.HubCarrierServices on e.HubCarrierServiceId equals hcs.HubCarrierServiceId
                              join hh in dbContext.HubCarriers on hcs.HubCarrierId equals hh.HubCarrierId
                              join hb in dbContext.Hubs on hh.HubId equals hb.HubId
                              where e.ExpressId == expressId
                              select new FrayteOWNMAWB
                              {
                                  FromCompanyName = fromadd.CompanyName,
                                  FromFirstName = fromadd.ContactFirstName,
                                  FromLastName = fromadd.ContactLastName,
                                  FromAddress1 = fromadd.Address1,
                                  FromAddress2 = fromadd.Address2,
                                  FromArea = fromadd.Area,
                                  FromState = fromadd.State,
                                  FromCity = fromadd.City,
                                  FromCountry = c.CountryName,
                                  FromZip = fromadd.Zip,
                                  FromPhoneCode = c.CountryPhoneCode,
                                  FromPhone = fromadd.PhoneNo,
                                  FromEmail = fromadd.Email,
                                  FromShipperReference = e.ShipmentReference,
                                  FromBillToAccount = ua.AccountNo,
                                  FromTaxDutyPayment = e.PaymentPartyTaxAndDuty,
                                  FromOrigin = "BKK",

                                  ToCompanyName = toadd.CompanyName,
                                  ToFirstName = toadd.ContactFirstName,
                                  ToLastName = toadd.ContactLastName,
                                  ToAddress1 = toadd.Address1,
                                  ToAddress2 = toadd.Address2,
                                  ToArea = toadd.Area,
                                  ToState = toadd.State,
                                  ToCity = toadd.City,
                                  ToCountry = cn.CountryName,
                                  ToZip = toadd.Zip,
                                  ToPhoneCode = cn.CountryPhoneCode,
                                  ToPhone = toadd.PhoneNo,
                                  ToDescription = ecd.ContentsExplanation,
                                  ToGrossWeight = "",
                                  ToEmail = toadd.Email,
                                  ToDestination = hb.Code,
                                  FinalMileCarrier = "",
                                  ConsignmentNo = "",
                                  AirWayBill = e.AWBBarcode,
                                  Code1 = "",
                                  Barcode2 = "",
                                  DisplayValueBarcode2 = "",
                                  Barcode3 = e.AWBBarcode,
                                  DisplayValueBarcode3 = "",
                                  Service = "NDX",
                                  IncoTerms = "",
                                  Currency = e.DeclaredCurrencyCode,

                                  LabelPrintDate = (DateTime.Now),
                                  TableProperties = new TableData()
                                  {
                                      TableCurrency = e.DeclaredCurrencyCode,
                                      TableDescription = ed.PiecesContent,
                                      TableValue = ed.Value
                                  },

                                  TotalValue = dbContext.ExpressDetails.Where(x => x.ExpressId == e.ExpressId).Sum(x => x.Value),
                                  ServiceCode = moduleType,
                                  AdditionalInfo = e.AdditionalInfo

                              }).ToList();
                var i = 1;
                foreach (var data in result)
                {
                    FrayteOWNMAWB obj = new FrayteOWNMAWB();
                    var query1 = data.FromFirstName + " " + data.FromLastName + "\n" + data.FromAddress1;
                    var query2 = data.FromCity;
                    var query3 = data.FromCountry;
                    var finalQuery = "";
                    if (!string.IsNullOrEmpty(data.FromCompanyName))
                    {
                        finalQuery = data.FromCompanyName + "\n" + query1 + "\n";
                    }
                    else
                    {
                        finalQuery = string.Empty + query1 + "\n";
                    }
                    if (!string.IsNullOrEmpty(data.FromAddress2))
                    {
                        finalQuery = finalQuery + data.FromAddress2 + "\n";
                    }
                    else
                    {
                        finalQuery = finalQuery + string.Empty;
                    }

                    if (!string.IsNullOrEmpty(data.FromArea))
                    {
                        finalQuery = finalQuery + data.FromArea + "\n" + query2 + " ";
                    }
                    else
                    {
                        finalQuery = finalQuery + string.Empty + query2 + " ";
                    }

                    if (!string.IsNullOrEmpty(data.FromState))
                    {
                        finalQuery = finalQuery + data.FromState;
                    }
                    else
                    {
                        finalQuery = finalQuery + string.Empty;
                    }

                    if (!string.IsNullOrEmpty(data.FromZip))
                    {
                        finalQuery = finalQuery + data.FromZip + "\n" + query3;
                    }
                    else
                    {
                        finalQuery = finalQuery + string.Empty + query3;
                    }
                    obj.FromAddress = finalQuery;
                    obj.FromPhone = "(+" + data.FromPhoneCode + ")" + " " + data.FromPhone;
                    obj.FromShipperReference = data.FromShipperReference;
                    obj.FromEmail = data.FromEmail;
                    obj.FromBillToAccount = data.FromBillToAccount;
                    obj.FromTaxDutyPayment = data.FromTaxDutyPayment;
                    obj.FromOrigin = data.FromOrigin;

                    var toquery1 = data.ToFirstName + " " + data.ToLastName + "\n" + data.ToAddress1;
                    var toquery2 = data.ToCity;
                    var toquery3 = data.ToCountry;
                    var tofinalQuery = "";
                    if (!string.IsNullOrEmpty(data.ToCompanyName))
                    {
                        tofinalQuery = data.ToCompanyName + "\n" + toquery1 + "\n";
                    }
                    else
                    {
                        tofinalQuery = string.Empty + toquery1 + "\n";
                    }
                    if (!string.IsNullOrEmpty(data.ToAddress2))
                    {
                        tofinalQuery = tofinalQuery + data.ToAddress2 + "\n";
                    }
                    else
                    {
                        tofinalQuery = tofinalQuery + string.Empty;
                    }

                    if (!string.IsNullOrEmpty(data.ToArea))
                    {
                        tofinalQuery = tofinalQuery + data.ToArea + "\n" + toquery2 + " ";
                    }
                    else
                    {
                        tofinalQuery = tofinalQuery + string.Empty + toquery2 + " ";
                    }

                    if (!string.IsNullOrEmpty(data.ToState))
                    {
                        tofinalQuery = tofinalQuery + data.ToState + " ";
                    }
                    else
                    {
                        tofinalQuery = tofinalQuery + string.Empty;
                    }

                    if (!string.IsNullOrEmpty(data.ToZip))
                    {
                        tofinalQuery = tofinalQuery + data.ToZip + "\n" + toquery3;
                    }
                    else
                    {
                        tofinalQuery = tofinalQuery + string.Empty + toquery3;
                    }
                    obj.ToAddress = tofinalQuery;
                    obj.ToPhone = "(+" + data.ToPhoneCode + ")" + " " + data.ToPhone;
                    obj.ToDescription = data.ToDescription;
                    obj.ToGrossWeight = grossweight.Sum(x => x.Weight) + " Kgs";
                    obj.ToEmail = data.ToEmail;
                    obj.ToDestination = data.ToDestination;
                    obj.FinalMileCarrier = "";
                    obj.ConsignmentNo = "";
                    obj.AirWayBill = "Consignment Number: " + data.AirWayBill.Substring(0, 3) + " " + data.AirWayBill.Substring(3, 3) + " " + data.AirWayBill.Substring(6, 3) + " " + data.AirWayBill.Substring(9, 3);
                    obj.Code1 = data.FromOrigin + "-" + (data.AirWayBill).ToString().Substring(data.AirWayBill.Length - 6, 6) + "-" + data.ToDestination;
                    obj.Barcode2 = "CTN" + data.AirWayBill + i.ToString("000");
                    obj.DisplayValueBarcode2 = "CTN" + " " + data.AirWayBill.Substring(0, 3) + " " + data.AirWayBill.Substring(3, 3) + " " + data.AirWayBill.Substring(6, 3) + " " + data.AirWayBill.Substring(9, 3) + " " + i.ToString("000") + " OF " + result.Count().ToString("000");
                    obj.Barcode3 = data.Barcode3;
                    obj.DisplayValueBarcode3 = data.AirWayBill.Substring(0, 3) + " " + data.AirWayBill.Substring(3, 3) + " " + data.AirWayBill.Substring(6, 3) + " " + data.AirWayBill.Substring(9, 3); ;
                    obj.Service = "NDX";
                    if (data.FromTaxDutyPayment == "Receiver")
                    {
                        obj.IncoTerms = "DDU";
                    }
                    else if (data.FromTaxDutyPayment == "Shiper")
                    {
                        obj.IncoTerms = "DDP";
                    }
                    else if (data.FromTaxDutyPayment == "ThirdParty")
                    {
                        obj.IncoTerms = "DDU";
                    }

                    obj.LabelPrintDate = data.LabelPrintDate;
                    obj.TableProperties = data.TableProperties;

                    obj.TotalCustomsValue = data.TotalValue + " " + data.Currency;
                    obj.ServiceCode = data.ServiceCode;
                    obj.AdditionalInfo = data.AdditionalInfo;
                    obj.LabelPerShipment = i + " of " + result.Count();
                    lstMAWB.Add(obj);
                    i++;
                }

                if (lstMAWB.Count > 0)
                {
                    return lstMAWB;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
    }
}
