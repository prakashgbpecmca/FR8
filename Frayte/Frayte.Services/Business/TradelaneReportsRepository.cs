using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models;
using Frayte.Services.Models.Tradelane;
using Frayte.Services.DataAccess;
using System.Text.RegularExpressions;
using System.Data;
using Frayte.Services.Utility;
using RazorEngine.Templating;
using System.IO;
using System.Web;

namespace Frayte.Services.Business
{
    public class TradelaneReportsRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<TradelaneBookingCoLoadFormModel> ColoadReportObj(TradelaneBooking ShipmentDetail)
        {
            List<TradelaneBookingCoLoadFormModel> ColoadList = new List<TradelaneBookingCoLoadFormModel>();

            if (ShipmentDetail != null)
            {

                TradelaneBookingCoLoadFormModel ColoadModel = new TradelaneBookingCoLoadFormModel();
                ColoadModel.FrayteNumber = ShipmentDetail.FrayteNumber;
                ColoadModel.ColoadTitle = "CO LOAD BOOKING FORM (" + DateTime.UtcNow.Date.Year + ")";
                ColoadModel.ShipperAddress = UtilityRepository.ConcatinateAddress(ShipmentDetail.ShipFrom, ShipmentDetail.DepartureAirport.AirportCode);
                ColoadModel.ConsigneeAddress = UtilityRepository.ConcatinateAddress(ShipmentDetail.ShipTo, ShipmentDetail.DestinationAirport.AirportCode);
                ColoadModel.OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
                ColoadModel.CTCPerson = "";
                ColoadModel.ShipperPhoneNo = !string.IsNullOrEmpty(ShipmentDetail.ShipFrom.Phone) && !string.IsNullOrEmpty(ShipmentDetail.ShipFrom.Country.CountryPhoneCode) ? "(+" + ShipmentDetail.ShipFrom.Country.CountryPhoneCode + ") " + ShipmentDetail.ShipFrom.Phone : "";
                ColoadModel.ConsigneePhoneNo = !string.IsNullOrEmpty(ShipmentDetail.ShipTo.Phone) && !string.IsNullOrEmpty(ShipmentDetail.ShipTo.Country.CountryPhoneCode) ? "(+" + ShipmentDetail.ShipTo.Country.CountryPhoneCode + ") " + ShipmentDetail.ShipTo.Phone : "";
                ColoadModel.NotifyPartyAddress = ShipmentDetail.IsNotifyPartySameAsReceiver ? ColoadModel.ConsigneeAddress : UtilityRepository.ConcatinateAddress(ShipmentDetail.NotifyParty, "");
                ColoadModel.NotifyPartyPhoneNo = !string.IsNullOrEmpty(ShipmentDetail.NotifyParty.Phone) && !string.IsNullOrEmpty(ShipmentDetail.NotifyParty.Country.CountryPhoneCode) ? "(+" + ShipmentDetail.NotifyParty.Country.CountryPhoneCode + ") " + ShipmentDetail.NotifyParty.Phone : "";
                ColoadModel.CagroReadyDate = dbContext.TradelaneShipmentAllocations.Where(a => a.TradelaneShipmentId == ShipmentDetail.TradelaneShipmentId).FirstOrDefault() != null && dbContext.TradelaneShipmentAllocations.Where(a => a.TradelaneShipmentId == ShipmentDetail.TradelaneShipmentId).FirstOrDefault().CreatedOnUTC != null ? dbContext.TradelaneShipmentAllocations.Where(a => a.TradelaneShipmentId == ShipmentDetail.TradelaneShipmentId).FirstOrDefault().CreatedOnUTC.Value.Date.ToString("dd-MMM-yy") : "";
                ColoadModel.DepartureAirport = ShipmentDetail.DepartureAirport != null ? ShipmentDetail.DepartureAirport.AirportCode + " - " + ShipmentDetail.DepartureAirport.AirportName : "";
                ColoadModel.DestinationAirport = ShipmentDetail.DestinationAirport != null ? ShipmentDetail.DestinationAirport.AirportCode + " - " + ShipmentDetail.DestinationAirport.AirportName : "";
                ColoadModel.MawbNo = !string.IsNullOrEmpty(ShipmentDetail.MAWB) ? ShipmentDetail.AirlinePreference.AilineCode + " " + ShipmentDetail.MAWB.Substring(0, 4) + " " + ShipmentDetail.MAWB.Substring(4, 4) : "";
                ColoadModel.SpecialInstruction = "";
                ColoadModel.ExportLicenceNo = !string.IsNullOrEmpty(ShipmentDetail.ExportLicenceNo) ? ShipmentDetail.ExportLicenceNo : ""; ;
                ColoadModel.AirLine = ShipmentDetail.AirlinePreference != null && !string.IsNullOrEmpty(ShipmentDetail.AirlinePreference.AirLineName) ? ShipmentDetail.AirlinePreference.AirLineName : "";
                ColoadModel.TotalPackages = GetTotalPackages(ShipmentDetail.TradelaneShipmentId);
                ColoadModel.ShipmentDescription = !string.IsNullOrEmpty(ShipmentDetail.ShipmentDescription) ? ShipmentDetail.ShipmentDescription : "";
                ColoadModel.GrossWeight = GetGrossWeight(ShipmentDetail.TradelaneShipmentId);
                ColoadModel.Volume = ShipmentDetail.HAWBPackages.Select(a => a.TotalVolume).Sum().ToString();
                ColoadModel.CopyrightText = "Published by FRAYTE Logistics Ltd" + Environment.NewLine + "© CopyRight " + DateTime.UtcNow.Date.Year;
                ColoadList.Add(ColoadModel);
            }
            return ColoadList;
        }

        private string GetVolume(int TradelaneShipmentId)
        {
            var TotalVolume = 0.0m;
            var total = "";
            var packages = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).ToList();
            if (packages.Count > 0)
            {
                foreach (var p in packages)
                {
                    TotalVolume = TotalVolume + (p.Height * p.Length * p.Width) / 6000;
                }
                total = Math.Round(TotalVolume) + " " + "Cbm";
                return total;
            }
            else
            {
                return "";
            }
        }

        private string GetTotalPackages(int TradelaneShipmentId)
        {
            decimal TotalPkg = 0;
            var total = "";
            var packages = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).ToList();
            if (packages.Count > 0)
            {
                foreach (var p in packages)
                {
                    TotalPkg = TotalPkg + p.CartonValue;
                }
                total = Math.Round(TotalPkg) + " " + "Pkgs";
                return total;
            }
            else
            {
                return "";
            }
        }

        private string GetGrossWeight(int TradelaneShipmentId)
        {
            var TotalWeight = 0.0m;
            var total = "";
            var packages = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).ToList();
            if (packages.Count > 0)
            {
                foreach (var p in packages)
                {
                    TotalWeight = TotalWeight + (p.CartonValue * p.Weight);
                }
                total = Math.Round(TotalWeight, 2).ToString() + " " + "Kgs";
                return total;
            }
            else
            {
                return "";
            }

        }

        #region HAWB
        public List<TradelaneBookingReportHAWB> GetHAWBObj(int tradelaneShipmentId, string documentTypeName)
        {
            try
            {
                List<TradelaneBookingReportHAWB> list = new List<TradelaneBookingReportHAWB>();
                TradelaneBookingReportHAWB reportModel = new TradelaneBookingReportHAWB();
                var ShipmentDetail = new TradelaneBookingRepository().GetTradelaneBookingDetails(tradelaneShipmentId, "");

                string mawb = string.Empty;

                if (!string.IsNullOrEmpty(ShipmentDetail.MAWB))
                {
                    if (ShipmentDetail.MAWB.Length == 8)
                    {
                        mawb = ShipmentDetail.MAWB.Substring(0, 4) + " " + ShipmentDetail.MAWB.Substring(3, 4);
                    }
                    else
                    {
                        mawb = ShipmentDetail.MAWB;
                    }
                }

                reportModel.ShipperAddress = UtilityRepository.ConcatinateAddress(ShipmentDetail.ShipFrom, ShipmentDetail.DepartureAirport.AirportCode); ;
                reportModel.ShipperAccountNumber = string.Empty;
                reportModel.ConsigneeAddress = UtilityRepository.ConcatinateAddress(ShipmentDetail.ShipTo, ShipmentDetail.DestinationAirport.AirportCode);
                reportModel.ConsigneeAccountNumber = string.Empty;
                reportModel.NotifyParty = ShipmentDetail.IsNotifyPartySameAsReceiver ? "NOTIFY: " + reportModel.ConsigneeAddress : "ALSO NOTIFY: " + ShipmentDetail.NotifyParty.FirstName + " " + ShipmentDetail.NotifyParty.LastName + Environment.NewLine + UtilityRepository.ConcatinateAddress(ShipmentDetail.NotifyParty, "");
                reportModel.CarrierAgent = string.Empty;
                reportModel.AccountNo = string.Empty;
                reportModel.AirportofDeparture = ShipmentDetail.DepartureAirport.AirportName;
                reportModel.DestinationAirport = ShipmentDetail.DestinationAirport.AirportName;
                reportModel.DestinationAirportCode = ShipmentDetail.DestinationAirport.AirportCode;
                reportModel.CurencyCode = ShipmentDetail.DeclaredCurrency.CurrencyCode;
                reportModel.DeclaredValueForCarriage = ShipmentDetail.DeclaredValue;
                reportModel.DeclaredValueForCustoms = ShipmentDetail.DeclaredCustomValue.HasValue ? ShipmentDetail.DeclaredCustomValue.Value : 0;
                reportModel.AmountOfInsurance = ShipmentDetail.InsuranceAmount.HasValue ? ShipmentDetail.InsuranceAmount.Value : 0;
                reportModel.Airline = ShipmentDetail.AirlinePreference.AirLineName;
                reportModel.MAWB = mawb;
                reportModel.MAWBCode = ShipmentDetail.AirlinePreference.AilineCode;
                reportModel.MAWBWithCode = ShipmentDetail.AirlinePreference.AilineCode + " " + mawb;
                reportModel.MAWBCountryCode = ShipmentDetail.DepartureAirport.AirportCode;
                reportModel.HAWB = documentTypeName;
                reportModel.HAWBBarCode = documentTypeName;
                reportModel.ShipmentDescription = ShipmentDetail.ShipmentDescription;

                if (UtilityRepository.GetOperationZone().OperationZoneId == 1)
                {
                    reportModel.IssuedBy = "FRAYTE LOGISTICS LIMITED" + Environment.NewLine;
                    reportModel.IssuedBy += "501 5/F KWONG LOONG TAI BUILDING" + Environment.NewLine;
                    reportModel.IssuedBy += "1016-1018 TAI NAN WEST STREET" + ", " + "CHEUNG SHA WAN" + " ," + "HON";
                }
                else
                {
                    reportModel.IssuedBy = "FRAYTE LOGISTICS LIMITED" + Environment.NewLine;
                    reportModel.IssuedBy += "501 5/F KWONG LOONG TAI BUILDING" + Environment.NewLine;
                    reportModel.IssuedBy += "1016-1018 TAI NAN WEST STREET" + ", " + "CHEUNG SHA WAN" + " ," + "HON";
                }

                HAWBTradelanePackage package;
                var collection = (from r in dbContext.TradelaneShipmentDetails
                                  join s in dbContext.TradelaneShipments on r.TradelaneShipmentId equals s.TradelaneShipmentId
                                  select new
                                  {
                                      HAWBNumber = s.HAWBNumber,
                                      TradelaneShipmentId = r.TradelaneShipmentId,
                                      TradelaneShipmentDetailId = r.TradelaneShipmentDetailId,
                                      CartonNumber = r.CartonNumber,
                                      CartonValue = r.CartonValue,
                                      Length = r.Length,
                                      Width = r.Width,
                                      Height = r.Height,
                                      Weight = r.Weight,
                                      HAWB = r.HAWB
                                  }).Where(p => p.TradelaneShipmentId == tradelaneShipmentId && p.HAWB == documentTypeName).ToList();

                if (collection.Count > 0)
                {
                    package = collection.GroupBy(x => x.HAWB)
                                        .Select(group => new HAWBTradelanePackage
                                        {
                                            TradelaneShipmentId = group.FirstOrDefault().TradelaneShipmentId,
                                            HAWB = group.Key,
                                            HAWBNumber = group.FirstOrDefault().HAWBNumber.HasValue ? group.FirstOrDefault().HAWBNumber.Value : 0,
                                            TotalCartons = group.Select(p => p.CartonValue).Sum(),
                                            EstimatedWeight = group.Select(p => p.Weight * p.CartonValue).Sum(),
                                            TotalVolume = group.Select(p => (p.Length * p.Width * p.Height) * p.CartonValue).Sum(),
                                            TotalWeight = group.Select(p => p.Weight * p.CartonValue).Sum(),
                                            Packages = group.Select(subGroup => new TradelanePackage
                                            {
                                                CartonNumber = subGroup.CartonNumber,
                                                CartonValue = subGroup.CartonValue,
                                                HAWB = subGroup.HAWB,
                                                Height = subGroup.Height,
                                                Length = subGroup.Length,
                                                TradelaneShipmentDetailId = subGroup.TradelaneShipmentDetailId,
                                                TradelaneShipmentId = subGroup.TradelaneShipmentId,
                                                Weight = subGroup.Weight,
                                                Width = subGroup.Width
                                            }).ToList()
                                        }).FirstOrDefault();
                }
                else
                {
                    return null;
                }

                if (package != null)
                {

                    reportModel.TotalCartons = package.TotalCartons;
                    reportModel.TotalWeight = package.TotalWeight;
                    reportModel.EstimatedWeight = package.EstimatedWeight;
                    reportModel.TotalVolume = ShipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? Math.Round(package.TotalVolume / (100 * 100 * 100), 2) : Math.Round(package.TotalVolume / (39.37M * 39.37M * 39.37M), 2);
                    reportModel.DimensionUnit = ShipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? "CM" : "IN";
                    reportModel.WeightUnit = ShipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? "KG" : "LB";
                    reportModel.VolumeUnit = "CBM";

                    reportModel.ShipmentDescription += Environment.NewLine + "VOL " + reportModel.TotalVolume + " " + reportModel.VolumeUnit;
                    reportModel.ShipmentDescription += Environment.NewLine + "VOL WEIGHT = " + reportModel.TotalWeight + " " + reportModel.WeightUnit;
                    reportModel.CreatedOn = DateTime.UtcNow;
                    reportModel.Signature = "Frayte Logistics SPX";
                    reportModel.CreatedOnCountry = "Hong Kong";

                }
                list.Add(reportModel);
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #region MAWB

        public List<TradelaneBookingReportMAWB> GetMAWBObj(int tradelaneShipmentId)
        {
            try
            {
                List<TradelaneBookingReportMAWB> list = new List<TradelaneBookingReportMAWB>();
                TradelaneBookingReportMAWB reportModel = new TradelaneBookingReportMAWB();
                var ShipmentDetail = new TradelaneBookingRepository().GetTradelaneBookingDetails(tradelaneShipmentId, "");
                var Custompdf = new TradelaneShipmentRepository().GetMawbCustomizePdf(tradelaneShipmentId);
                string mawb = string.Empty;
                if (!string.IsNullOrEmpty(ShipmentDetail.MAWB))
                {
                    if (ShipmentDetail.MAWB.Length == 8)
                    {
                        mawb = ShipmentDetail.MAWB.Substring(0, 4) + " " + ShipmentDetail.MAWB.Substring(3, 4);
                    }
                    else
                    {
                        mawb = ShipmentDetail.MAWB;
                    }
                }
                reportModel.MAWB = mawb;
                reportModel.ShipperAddress = UtilityRepository.ConcatinateAddress(ShipmentDetail.ShipFrom, ShipmentDetail.DepartureAirport.AirportCode);
                reportModel.ShipperAccountNumber = string.Empty;
                reportModel.ConsigneeAddress = UtilityRepository.ConcatinateAddress(ShipmentDetail.ShipTo, ShipmentDetail.DestinationAirport.AirportCode);
                reportModel.ConsigneeAccountNumber = string.Empty;
                reportModel.NotifyParty = ShipmentDetail.IsNotifyPartySameAsReceiver ? "NOTIFY: " + reportModel.ConsigneeAddress : "NOTIFY: " + UtilityRepository.ConcatinateAddress(ShipmentDetail.NotifyParty, "");
                reportModel.CarrierAgent = string.Empty;
                reportModel.AirportofDeparture = ShipmentDetail.ShipFrom.Country.Name;
                reportModel.DestinationAirport = ShipmentDetail.DestinationAirport.AirportName;
                reportModel.DestinationAirportCode = ShipmentDetail.DestinationAirport.AirportCode;
                reportModel.CurencyCode = ShipmentDetail.DeclaredCurrency.CurrencyCode;                
                reportModel.AmountOfInsurance = ShipmentDetail.InsuranceAmount.HasValue ? ShipmentDetail.InsuranceAmount.Value : 0;
                reportModel.MAWBCode = ShipmentDetail.AirlinePreference.AilineCode;
                reportModel.MAWBWithCode = ShipmentDetail.AirlinePreference.AilineCode + " " + mawb;
                reportModel.MAWBCountryCode = ShipmentDetail.ShipFrom.Country.Code;
                reportModel.ShipmentDescription = ShipmentDetail.ShipmentDescription;
                reportModel.Airline = ShipmentDetail.AirlinePreference.AirLineName;

                reportModel.IssuingCarriersAgentNameandCity = Custompdf.IssuingCarriersAgentNameandCity;
                reportModel.AccountNo = Custompdf.AccountNo;
                reportModel.DeclaredValueForCarriage = Custompdf.DeclaredValueForCarriage;
                reportModel.DeclaredValueForCustoms = Custompdf.DeclaredValueForCustoms;
                reportModel.ValuationCharge = Custompdf.ValuationCharge;
                reportModel.Tax = Custompdf.Tax;
                reportModel.TotalOtherChargesDueAgent = Custompdf.TotalOtherChargesDueAgent;
                reportModel.TotalOtherChargesDueCarrier = Custompdf.TotalOtherChargesDueCarrier;
                reportModel.OtherCharges = Custompdf.OtherCharges;
                reportModel.ChargesAtDestination = Custompdf.ChargesAtDestination;
                reportModel.TotalCollectCharges = Custompdf.TotalCollectCharges;
                reportModel.CurrencyConversionRates = Custompdf.CurrencyConversionRates;
                reportModel.TotalPrepaid = Custompdf.TotalPrepaid;
                reportModel.TotalCollect = Custompdf.TotalCollect;
                reportModel.HandlingInformation = Custompdf.HandlingInformation;
                reportModel.AgentsIATACode = Custompdf.AgentsIATACode;

                if (UtilityRepository.GetOperationZone().OperationZoneId == 1)
                {
                    reportModel.IssuedBy = "FRAYTE LOGISTICS LIMITED" + Environment.NewLine;
                    reportModel.IssuedBy += "501 5/F KWONG LOONG TAI BUILDING" + Environment.NewLine;
                    reportModel.IssuedBy += "1016-1018 TAI NAN WEST STREET" + ", " + "CHEUNG SHA WAN" + " ," + "HON";
                }
                else
                {
                    reportModel.IssuedBy = "FRAYTE LOGISTICS LIMITED" + Environment.NewLine;
                    reportModel.IssuedBy += "501 5/F KWONG LOONG TAI BUILDING" + Environment.NewLine;
                    reportModel.IssuedBy += "1016-1018 TAI NAN WEST STREET" + ", " + "CHEUNG SHA WAN" + " ," + "HON";
                }

                List<HAWBTradelanePackage> package;
                var collection = (from r in dbContext.TradelaneShipmentDetails
                                  join s in dbContext.TradelaneShipments on r.TradelaneShipmentId equals s.TradelaneShipmentId
                                  select new
                                  {
                                      HAWBNumber = s.HAWBNumber,
                                      TradelaneShipmentId = r.TradelaneShipmentId,
                                      TradelaneShipmentDetailId = r.TradelaneShipmentDetailId,
                                      CartonNumber = r.CartonNumber,
                                      CartonValue = r.CartonValue,
                                      Length = r.Length,
                                      Width = r.Width,
                                      Height = r.Height,
                                      Weight = r.Weight,
                                      HAWB = r.HAWB
                                  }).Where(p => p.TradelaneShipmentId == tradelaneShipmentId).ToList();

                if (collection.Count > 0)
                {
                    package = collection.GroupBy(x => x.HAWB)
                                        .Select(group => new HAWBTradelanePackage
                                        {
                                            TradelaneShipmentId = group.FirstOrDefault().TradelaneShipmentId,
                                            HAWB = group.Key,
                                            HAWBNumber = group.FirstOrDefault().HAWBNumber.HasValue ? group.FirstOrDefault().HAWBNumber.Value : 0,
                                            TotalCartons = group.Select(p => p.CartonValue).Sum(),
                                            EstimatedWeight = group.Select(p => p.Weight * p.CartonValue).Sum(),
                                            TotalVolume = group.Select(p => (p.Length * p.Width * p.Height) * p.CartonValue).Sum(),
                                            TotalWeight = group.Select(p => p.Weight * p.CartonValue).Sum(),
                                            Packages = group.Select(subGroup => new TradelanePackage
                                            {
                                                CartonNumber = subGroup.CartonNumber,
                                                CartonValue = subGroup.CartonValue,
                                                HAWB = subGroup.HAWB,
                                                Height = subGroup.Height,
                                                Length = subGroup.Length,
                                                TradelaneShipmentDetailId = subGroup.TradelaneShipmentDetailId,
                                                TradelaneShipmentId = subGroup.TradelaneShipmentId,
                                                Weight = subGroup.Weight,
                                                Width = subGroup.Width
                                            }).ToList()
                                        }).ToList();
                }
                else
                {
                    return null;
                }

                if (package != null)
                {
                    reportModel.TotalCartons = package.Sum(p => p.TotalCartons);
                    reportModel.TotalWeight = package.Sum(p => p.TotalWeight);
                    reportModel.EstimatedWeight = package.Sum(p => p.EstimatedWeight);
                    reportModel.TotalVolume = reportModel.TotalVolume = ShipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? Math.Round(package.Sum(p => p.TotalVolume) / (100 * 100 * 100), 2) : Math.Round(package.Sum(p => p.TotalVolume) / (39.37M * 39.37M * 39.37M), 2);
                    reportModel.DimensionUnit = ShipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? "CM" : "IN";
                    reportModel.WeightUnit = ShipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? "KG" : "LB";
                    reportModel.VolumeUnit = ShipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? "CBM" : "CBM";

                    reportModel.ShipmentDescription += Environment.NewLine + "VOL " + reportModel.TotalVolume + " " + reportModel.VolumeUnit;
                    reportModel.ShipmentDescription += Environment.NewLine + "VOL WEIGHT = " + reportModel.TotalWeight + " " + reportModel.WeightUnit;
                    reportModel.CreatedOn = DateTime.UtcNow;
                    reportModel.Signature = "Frayte Logistics SPX";
                    reportModel.CreatedOnCountry = "Hong Kong";
                }

                list.Add(reportModel);
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public TradelaneReportMAWBModel MAWBReportObj(int tradelaneShipmentId)
        {
            try
            {
                TradelaneReportMAWBModel reportModel = new TradelaneReportMAWBModel();
                reportModel.ShipmentDetail = new TradelaneBookingRepository().GetTradelaneBookingDetails(tradelaneShipmentId, "");

                string mawb = string.Empty;
                if (!string.IsNullOrEmpty(reportModel.ShipmentDetail.MAWB))
                {
                    if (reportModel.ShipmentDetail.MAWB.Length == 8)
                    {
                        mawb = reportModel.ShipmentDetail.MAWB.Substring(0, 4) + " " + reportModel.ShipmentDetail.MAWB.Substring(3, 4);
                    }
                    else
                    {
                        mawb = reportModel.ShipmentDetail.MAWB;
                    }
                }
                reportModel.MAWB = mawb;

                var collection = (from r in dbContext.TradelaneShipmentDetails
                                  join s in dbContext.TradelaneShipments on r.TradelaneShipmentId equals s.TradelaneShipmentId
                                  select new
                                  {
                                      HAWBNumber = s.HAWBNumber,
                                      TradelaneShipmentId = r.TradelaneShipmentId,
                                      TradelaneShipmentDetailId = r.TradelaneShipmentDetailId,
                                      CartonNumber = r.CartonNumber,
                                      CartonValue = r.CartonValue,
                                      Length = r.Length,
                                      Width = r.Width,
                                      Height = r.Height,
                                      Weight = r.Weight,
                                      HAWB = r.HAWB
                                  }).Where(p => p.TradelaneShipmentId == tradelaneShipmentId).ToList();

                if (collection.Count > 0)
                {
                    var package = collection.GroupBy(x => x.HAWB)
                                          .Select(group => new HAWBTradelanePackage
                                          {
                                              TradelaneShipmentId = group.FirstOrDefault().TradelaneShipmentId,
                                              HAWB = group.Key,
                                              HAWBNumber = group.FirstOrDefault().HAWBNumber.HasValue ? group.FirstOrDefault().HAWBNumber.Value : 0,
                                              TotalCartons = group.Select(p => p.CartonValue).Sum(),
                                              EstimatedWeight = group.Select(p => p.Weight * p.CartonValue).Sum(),
                                              TotalVolume = group.Select(p => (p.Length * p.Width * p.Height) * p.CartonValue).Sum(),
                                              TotalWeight = group.Select(p => p.Weight * p.CartonValue).Sum(),
                                              Packages = group.Select(subGroup => new TradelanePackage
                                              {
                                                  CartonNumber = subGroup.CartonNumber,
                                                  CartonValue = subGroup.CartonValue,
                                                  HAWB = subGroup.HAWB,
                                                  Height = subGroup.Height,
                                                  Length = subGroup.Length,
                                                  TradelaneShipmentDetailId = subGroup.TradelaneShipmentDetailId,
                                                  TradelaneShipmentId = subGroup.TradelaneShipmentId,
                                                  Weight = subGroup.Weight,
                                                  Width = subGroup.Width
                                              }).ToList()
                                          }).ToList();

                    reportModel.EstimatedWeight = package.Sum(p => p.EstimatedWeight);
                    reportModel.TotalCartons = package.Sum(p => p.TotalCartons);
                    reportModel.TotalWeight = package.Sum(p => p.TotalWeight);
                    reportModel.TotalVolume = package.Sum(p => p.TotalVolume);

                    reportModel.TotalVolume = reportModel.TotalVolume = reportModel.ShipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? Math.Round(package.Sum(p => p.TotalVolume) / (100 * 100 * 100), 2) : Math.Round(package.Sum(p => p.TotalVolume) / (39.37M * 39.37M * 39.37M), 2);
                    reportModel.DimensionUnit = reportModel.ShipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? "CM" : "IN";
                    reportModel.WeightUnit = reportModel.ShipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? "KG" : "LB";
                    reportModel.VolumeUnit = reportModel.ShipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? "CBM" : "CBM";
                }
                return reportModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #region Manifest
        public List<TradelaneManifestReport> ManifestReportModel(int tradelaneShipmentId, int userId)
        {

            List<TradelaneManifestReport> list = new List<TradelaneManifestReport>();

            TradelaneManifestReport reportModel = new TradelaneManifestReport();
            TradelaneBooking shipmentDetail = new TradelaneBookingRepository().GetTradelaneBookingDetails(tradelaneShipmentId, "");

            if (shipmentDetail != null)
            {
                if (userId > 0)
                {
                    reportModel.PrintedBy = dbContext.Users.Find(userId).ContactName;
                }
                else
                {
                    reportModel.PrintedBy = dbContext.Users.Find(shipmentDetail.CreatedBy).ContactName;
                }
                if (!string.IsNullOrEmpty(reportModel.MAWB))
                {
                    reportModel.MAWB = shipmentDetail.AirlinePreference.AilineCode + " " + shipmentDetail.MAWB.Substring(0, 4) + " " + shipmentDetail.MAWB.Substring(4, 4);
                }
                else
                {
                    reportModel.MAWB = "";
                }
                reportModel.FrayteNumber = shipmentDetail.FrayteNumber;
                reportModel.AirlineCode = shipmentDetail.AirlinePreference.AilineCode;
                reportModel.Console = string.Empty;
                reportModel.CreatedOn = DateTime.UtcNow;
                reportModel.Arrival = string.Empty;

                if (shipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId != 5)
                {
                    if (shipmentDetail.MAWBAgentId.HasValue && shipmentDetail.MAWBAgentId.Value > 0)
                    {
                        var allocation = dbContext.TradelaneShipmentAllocations.Where(p => p.TradelaneShipmentId == tradelaneShipmentId).FirstOrDefault();
                        if (allocation != null)
                        {
                            if (allocation.EstimatedDateofDelivery.HasValue && !string.IsNullOrEmpty(allocation.FlightNumber))
                            {
                                reportModel.FlightAndDate = allocation.FlightNumber + " / " + shipmentDetail.DepartureAirport.AirportCode + " / " + allocation.EstimatedDateofDelivery.Value.ToString("dd-MMM-yy");
                            }
                            reportModel.ETD = allocation.EstimatedDateofDelivery;
                            reportModel.ETA = allocation.EstimatedDateofArrival;
                            fillExportAgentDetail(reportModel, shipmentDetail.MAWBAgentId.Value);
                            reportModel.ImportAgent = "";
                        }
                        else
                        {
                            reportModel.ImportAgent = "";
                            reportModel.ExportAgent = "";
                        }
                    }
                    else
                    {
                        reportModel.ImportAgent = "";
                        reportModel.ExportAgent = "";
                    }
                }
                else
                {
                    var shipmentAllocation = dbContext.TradelaneShipmentAllocations.Where(p => p.TradelaneShipmentId == tradelaneShipmentId).ToList();
                    if (shipmentAllocation.Count > 1)
                    {
                        var allocation = shipmentAllocation.Where(p => p.LegNum == "Leg1").FirstOrDefault();

                        if (allocation.EstimatedDateofDelivery.HasValue && !string.IsNullOrEmpty(allocation.FlightNumber))
                        {
                            reportModel.FlightAndDate = allocation.FlightNumber + " / " + shipmentDetail.DepartureAirport.AirportCode + " / " + allocation.EstimatedDateofDelivery.Value.ToString("dd-MMM-yy");
                        }
                        int ExportAgent = shipmentAllocation.Where(p => p.LegNum == "Leg1").FirstOrDefault().AgentId.Value;
                        int ImportAgent = shipmentAllocation.Where(p => p.LegNum == "Leg2").FirstOrDefault().AgentId.Value;
                        fillExportAgentDetail(reportModel, ExportAgent);
                        fillImportAgentDetail(reportModel, ImportAgent);

                    }
                    else
                    {
                        reportModel.ExportAgent = "";
                        reportModel.ImportAgent = "";
                    }
                }
                reportModel.Loading = shipmentDetail.DepartureAirport.AirportCode;
                reportModel.DisCharge = shipmentDetail.DestinationAirport.AirportCode;
                reportModel.TotalPackages = shipmentDetail.HAWBPackages.Sum(p => p.PackagesCount);
                reportModel.TotalVolume = Math.Round(shipmentDetail.HAWBPackages.Sum(p => p.TotalVolume), 2).ToString() + " " + (shipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? "CM3" : "IN3");
                reportModel.TotalWeight = shipmentDetail.HAWBPackages.Sum(p => p.TotalWeight).ToString() + " " + (shipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? "KG" : "LB");
                reportModel.TotalCartons = shipmentDetail.HAWBPackages.Sum(p => p.TotalCartons);
                reportModel.TotalShipments = shipmentDetail.HAWBPackages.Count();
                fillImportPackageDetail(reportModel, shipmentDetail);
            }

            list.Add(reportModel);
            return list;
        }
        private void fillImportPackageDetail(TradelaneManifestReport reportModel, TradelaneBooking shipmentDetail)
        {
            reportModel.ShipmentDetail = new List<TradelaneManifestReportDetail>();
            TradelaneManifestReportDetail detail;
            foreach (var item in shipmentDetail.HAWBPackages)
            {
                detail = new TradelaneManifestReportDetail();
                detail.HAWB = item.HAWB;
                detail.TotalWeight = item.TotalWeight.ToString() + " " + (shipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? "KG" : "LB");
                detail.TotalVolume = Math.Round(item.TotalVolume, 2).ToString() + " " + (shipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? "CM3" : "IN3");
                detail.TotalCarton = item.TotalCartons.ToString() + " CTN";
                detail.BillNumbers = item.HAWB + Environment.NewLine + Environment.NewLine + "Col HBL: " + string.Empty + Environment.NewLine + "Shipper Ref: " + shipmentDetail.ShipperAdditionalNote;
                detail.Shipper = shipmentDetail.ShipFrom.CompanyName;
                detail.Receiver = shipmentDetail.ShipTo.CompanyName;
                detail.Origin = shipmentDetail.DepartureAirport.AirportCode;
                detail.Destination = shipmentDetail.DestinationAirport.AirportCode;
                detail.ShipmentDescription = shipmentDetail.ShipmentDescription;
                reportModel.ShipmentDetail.Add(detail);
            }

        }
        private void fillExportAgentDetail(TradelaneManifestReport reportModel, int userId)
        {
            var userDetail = new FrayteUserRepository().GetInternalUserDetail(userId);

            if (userDetail != null && userDetail.UserAddress != null)
            {
                reportModel.ExportAgent = string.Empty;

                if (!string.IsNullOrEmpty(userDetail.ContactName))
                {
                    reportModel.ExportAgent += userDetail.ContactName;
                }
                if (!string.IsNullOrEmpty(userDetail.CompanyName))
                {
                    reportModel.ExportAgent += Environment.NewLine + userDetail.ContactName;
                }
                if (!string.IsNullOrEmpty(userDetail.UserAddress.Address))
                {
                    reportModel.ExportAgent += Environment.NewLine + userDetail.UserAddress.Address;
                }
                if (!string.IsNullOrEmpty(userDetail.UserAddress.Address2))
                {
                    reportModel.ExportAgent += Environment.NewLine + userDetail.UserAddress.Address2;
                }
                if (!string.IsNullOrEmpty(userDetail.UserAddress.City))
                {
                    reportModel.ExportAgent += Environment.NewLine + userDetail.UserAddress.City;
                }
                if (!string.IsNullOrEmpty(userDetail.UserAddress.Zip))
                {
                    reportModel.ExportAgent += Environment.NewLine + userDetail.UserAddress.Zip;
                }
                if (!string.IsNullOrEmpty(userDetail.UserAddress.State))
                {
                    reportModel.ExportAgent += Environment.NewLine + userDetail.UserAddress.State;
                }
                if (!string.IsNullOrEmpty(userDetail.UserAddress.Country.Name))
                {
                    reportModel.ExportAgent += Environment.NewLine + userDetail.UserAddress.Country.Name;
                }
                if (!string.IsNullOrEmpty(userDetail.TelephoneNo))
                {
                    reportModel.ExportAgent += Environment.NewLine + "(" + userDetail.UserAddress.Country.CountryPhoneCode + ") " + userDetail.TelephoneNo;
                }
            }
            else
            {
                reportModel.ExportAgent = "";
            }

        }
        private void fillImportAgentDetail(TradelaneManifestReport reportModel, int userId)
        {
            var userDetail = new FrayteUserRepository().GetInternalUserDetail(userId);

            if (userDetail != null && userDetail.UserAddress != null)
            {
                reportModel.ImportAgent = string.Empty;

                if (!string.IsNullOrEmpty(userDetail.ContactName))
                {
                    reportModel.ImportAgent += userDetail.ContactName;
                }
                if (!string.IsNullOrEmpty(userDetail.CompanyName))
                {
                    reportModel.ImportAgent += Environment.NewLine + userDetail.ContactName;
                }
                if (!string.IsNullOrEmpty(userDetail.UserAddress.Address))
                {
                    reportModel.ImportAgent += Environment.NewLine + userDetail.UserAddress.Address;
                }
                if (!string.IsNullOrEmpty(userDetail.UserAddress.Address2))
                {
                    reportModel.ImportAgent += Environment.NewLine + userDetail.UserAddress.Address2;
                }
                if (!string.IsNullOrEmpty(userDetail.UserAddress.City))
                {
                    reportModel.ImportAgent += Environment.NewLine + userDetail.UserAddress.City;
                }
                if (!string.IsNullOrEmpty(userDetail.UserAddress.Zip))
                {
                    reportModel.ImportAgent += Environment.NewLine + userDetail.UserAddress.Zip;
                }
                if (!string.IsNullOrEmpty(userDetail.UserAddress.State))
                {
                    reportModel.ImportAgent += Environment.NewLine + userDetail.UserAddress.State;
                }
                if (!string.IsNullOrEmpty(userDetail.UserAddress.Country.Name))
                {
                    reportModel.ImportAgent += Environment.NewLine + userDetail.UserAddress.Country.Name;
                }
                if (!string.IsNullOrEmpty(userDetail.TelephoneNo))
                {
                    reportModel.ImportAgent += Environment.NewLine + "(" + userDetail.UserAddress.Country.CountryPhoneCode + ") " + userDetail.TelephoneNo;
                }
            }
            else
            {
                reportModel.ImportAgent = "";
            }

        }
        #endregion

        public List<TradelaneCartonLabelReport> GetCartonLabelObj(int tradelaneShipmentId, string hawb)
        {
            try
            {
                List<TradelaneCartonLabelReport> list = new List<TradelaneCartonLabelReport>();


                TradelaneCartonLabelReport reportModel = new TradelaneCartonLabelReport();
                var ShipmentDetail = new TradelaneBookingRepository().GetTradelaneBookingDetails(tradelaneShipmentId, "");
                reportModel.MAWB = ShipmentDetail.AirlinePreference.AilineCode + " " + ShipmentDetail.MAWB.Substring(0, 4) + " " + ShipmentDetail.MAWB.Substring(4, 4);
                reportModel.MAWBBarCode = ShipmentDetail.AirlinePreference.AilineCode + ShipmentDetail.MAWB;
                reportModel.TotalPieces = dbContext.TradelaneShipmentDetails.Where(a => a.TradelaneShipmentId == tradelaneShipmentId).ToList().Count.ToString();
                reportModel.HawbScannedCarton = dbContext.TradelaneShipmentDetails.Where(a => a.HAWB == hawb && a.IsScaned == true).Count();
                reportModel.HAWBBarCodeValue = hawb;
                reportModel.HAWBTotalPieces = ShipmentDetail.HAWBPackages.Where(a => a.HAWB == hawb).FirstOrDefault().PackagesCount;
                reportModel.HAWB = hawb + " " + ShipmentDetail.DestinationAirport.AirportCode;
                reportModel.HAWBBarCode = hawb + " " + ShipmentDetail.DestinationAirport.AirportCode;
                reportModel.DestinationAirportCode = ShipmentDetail.DestinationAirport.AirportCode;
                reportModel.Destination = ShipmentDetail.DestinationAirport.AirportCode;
                reportModel.Departure = ShipmentDetail.ShipFrom.Country.Name;
                reportModel.CarrierCode2 = ShipmentDetail.AirlinePreference.CarrierCode2;
                reportModel.DepartureAirportCode = ShipmentDetail.DepartureAirport.AirportCode;
                list.Add(reportModel);
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
