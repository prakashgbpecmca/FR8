using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Models.ApiModal;
using Frayte.Services.Models.DHL;
using Frayte.Services.Models.Express;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using XStreamline.Log;

namespace Frayte.Services.Business
{
    public class DHLRepository
    {
        public DHLResponseDto CreateShipment(string shipmentXml, DHLShipmentRequestDto DHLShipmentRequest)
        {
            DHLResponseDto DHLResponse = new DHLResponseDto();
            var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.DHL);

            if (logisticIntegration != null)
            {
                string trackingCode = string.Empty;
                string pickreferance = string.Empty;
                string pickupStatus = string.Empty;
                string pickupResult = string.Empty;
                string pickupXml = string.Empty;
                var shipmentPackageTrackingDetail = new List<FraytePackageTrackingDetail>();
                //Send XML
                var shipmentResult = CallWebservice(shipmentXml, logisticIntegration.ServiceUrl); //utf-8 only
                DHLResponse.ServiceResponse = shipmentResult;
                try
                {
                    var Status = ReadXMLDocument(@shipmentResult);
                    if (Status == "Success")
                    {
                        var xml = XDocument.Parse(@shipmentResult);
                        DHLShipmentRequest.AWBNumber = GetElementValue(xml.Descendants("AirwayBillNumber"));
                        if (DHLShipmentRequest.IsPickup)
                        {
                            #region Capability

                            var Capability = new DHLRepository().CreateXMLForDHLPickupforCapability(DHLShipmentRequest);
                            string xmlCapability = File.ReadAllText(@Capability);

                            var CapabilityResult = CallWebservice(xmlCapability, logisticIntegration.ServiceUrl);
                            var Capabilityxml = XDocument.Parse(@CapabilityResult);
                            string ElmahResult = string.Empty;
                            string ReadyTime = GetElementValue(Capabilityxml.Descendants("BookingTime"));
                            string PickupCutoffTime = GetElementValue(Capabilityxml.Descendants("PickupCutoffTime"));

                            if (string.IsNullOrWhiteSpace(ReadyTime) && string.IsNullOrWhiteSpace(PickupCutoffTime))
                            {
                                ElmahResult = "AirwayBillNumber:-" + DHLShipmentRequest.AWBNumber + "CapabilityResult:-" + CapabilityResult;
                                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(ElmahResult));
                                PickupCutoffTime = "1700";
                                ReadyTime = DHLShipmentRequest.CollectionTime;

                            }
                            else
                            {
                                ReadyTime = Regex.Replace(ReadyTime, "[^0-9]", "");

                                PickupCutoffTime = Regex.Replace(PickupCutoffTime, "[^0-9]", "");

                                ElmahResult = "AirwayBillNumber:-" + DHLShipmentRequest.AWBNumber + "CapabilityResult:-" + CapabilityResult;

                                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(ElmahResult));
                            }



                            #endregion

                            #region Pickup

                            pickupXml = new DHLRepository().CreateXMLForDHLPickup(DHLShipmentRequest, PickupCutoffTime, ReadyTime);

                            string pickupXml_in = File.ReadAllText(@pickupXml);

                            pickupResult = CallWebservice(pickupXml_in, logisticIntegration.ServiceUrl);
                            var PUxml = XDocument.Parse(@pickupResult);

                            pickreferance = GetElementValue(PUxml.Descendants("ConfirmationNumber"));
                            pickupStatus = ReadXMLDocument(@pickupResult);


                            //Version 5.0

                            #endregion
                        }

                        if (pickupStatus == "Success" || (Status == "Success" && string.IsNullOrWhiteSpace(pickupStatus)))
                        {
                            DHLResponse.Pieces = (from r in xml.Descendants("Piece")
                                                  select new Piece
                                                  {
                                                      DataIdentifier = r.Element("DataIdentifier") != null ? r.Element("DataIdentifier").Value : "",
                                                      Depth = r.Element("Depth") != null ? r.Element("Depth").Value : "",
                                                      DimWeight = r.Element("DimWeight") != null ? r.Element("DimWeight").Value : "",
                                                      Height = r.Element("Height") != null ? r.Element("Height").Value : "",
                                                      Weight = r.Element("Weight") != null ? r.Element("Weight").Value : "",
                                                      Width = r.Element("Width") != null ? r.Element("Width").Value : "",
                                                      LicensePlate = r.Element("LicensePlate") != null ? r.Element("LicensePlate").Value : "",
                                                      LicensePlateBarCode = r.Element("LicensePlateBarCode") != null ? r.Element("LicensePlateBarCode").Value : "",
                                                      PieceNumber = r.Element("PieceNumber") != null ? r.Element("PieceNumber").Value : "",
                                                  }).ToList();

                            var itemToRemove = DHLResponse.Pieces.Single(r => r.LicensePlate == "");
                            if (itemToRemove != null)
                            {
                                DHLResponse.Pieces.Remove(itemToRemove);
                            }

                            var count = DHLResponse.Pieces.Count();
                            count++;
                            string Image = string.Empty;
                            Image = FrayteShortName.DHL + "_" + DHLShipmentRequest.AWBNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".jpg";

                            itemToRemove.LicensePlate = "AirwayBillNumber_" + DHLShipmentRequest.AWBNumber;

                            DHLResponse.ShipmentImange = Image;
                            DHLResponse.PickupRef = pickreferance;
                            DHLResponse.Pieces.Add(itemToRemove);

                            DHLResponse.ImageString = GetElementValue(xml.Descendants("LabelImage").Elements("OutputImage"));
                            DHLResponse.DHLOrderId = "Order_" + DHLShipmentRequest.AWBNumber;
                            DHLResponse.Status = true;

                            if (string.IsNullOrWhiteSpace(pickupStatus) && DHLShipmentRequest.IsPickup)
                            {
                                var pickupxml = XDocument.Parse(@pickupResult);
                                var Error = (from r in pickupxml.Descendants("Condition")
                                             select new DHLError
                                             {
                                                 ErrorCode = r.Element("ConditionCode") != null ? r.Element("ConditionCode").Value : "",
                                                 ErrorDescription = r.Element("ConditionData") != null ? r.Element("ConditionData").Value : "",
                                             }).ToList();

                                new DirectShipmentRepository().SaveEasyPosyPickUpObject(@pickupResult, shipmentXml, DHLShipmentRequest.DraftShipmentId);
                            }
                        }
                        else
                        {
                            var pickupxml = XDocument.Parse(@pickupResult);
                            var Error = (from r in pickupxml.Descendants("Condition")
                                         select new DHLError
                                         {
                                             ErrorCode = r.Element("ConditionCode") != null ? r.Element("ConditionCode").Value : "",
                                             ErrorDescription = r.Element("ConditionData") != null ? r.Element("ConditionData").Value : "",
                                         }).ToList();

                            DHLResponse.Status = false;
                            DHLResponse.Error = Error.FirstOrDefault();
                            new DirectShipmentRepository().SaveEasyPosyPickUpObject(@pickupResult, shipmentXml, DHLShipmentRequest.DraftShipmentId);
                        }

                    }
                    else
                    {
                        var xml = XDocument.Parse(@shipmentResult);
                        var Error = (from r in xml.Descendants("Condition")
                                     select new DHLError
                                     {
                                         ErrorCode = r.Element("ConditionCode") != null ? r.Element("ConditionCode").Value : "",
                                         ErrorDescription = r.Element("ConditionData") != null ? r.Element("ConditionData").Value : "",
                                     }).ToList();

                        DHLResponse.Status = false;
                        DHLResponse.Error = Error.FirstOrDefault();
                        new DirectShipmentRepository().SaveEasyPostErrorObject(@shipmentResult, shipmentXml, DHLShipmentRequest.DraftShipmentId);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return DHLResponse;
        }

        public DHLShipmentRequestDto MapDirectBookingDetailToDHLShipmentRequestDto(DirectBookingShipmentDraftDetail DraftDetail)
        {
            try
            {
                DHLShipmentRequestDto DHLShipmentRequest = new DHLShipmentRequestDto();

                var RegionCode = new CountryRepository().GetCountryByNameAndContryCode(DraftDetail.ShipTo.Country.Name, DraftDetail.ShipTo.Country.Code).RegionCode;
                DHLShipmentRequest.Request = new DHLRequestDto();
                DHLShipmentRequest.Request.ServiceHeader = new ServiceHeaderDto();
                DHLShipmentRequest.DraftShipmentId = DraftDetail.DirectShipmentDraftId;
                DHLShipmentRequest.Request.ServiceHeader.MessageReference = "DHL Core Integration";
                DHLShipmentRequest.Request.ServiceHeader.MessageTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                DHLShipmentRequest.Request.ServiceHeader.SiteID = "";
                DHLShipmentRequest.Request.ServiceHeader.Password = "";

                DHLShipmentRequest.RegionCode = RegionCode;
                DHLShipmentRequest.LanguageCode = "en";
                DHLShipmentRequest.PiecesEnabled = "Y";
                DHLShipmentRequest.Billing = new BillingDto();
                if (DraftDetail.PayTaxAndDuties == FrayteShippingPaymentType.Shipper)
                {
                    DHLShipmentRequest.Billing.ShippingPaymentType = "S";
                    DHLShipmentRequest.Billing.ShipperAccountNumber = DraftDetail.CustomerRateCard.CourierAccountNo;
                    DHLShipmentRequest.Billing.DutyAccountNumber = DraftDetail.CustomerRateCard.CourierAccountNo;
                    DHLShipmentRequest.Billing.BillingAccountNumber = DraftDetail.CustomerRateCard.CourierAccountNo;
                    DHLShipmentRequest.Billing.DutyPaymentType = "S";
                }
                if (DraftDetail.PayTaxAndDuties == FrayteShippingPaymentType.Receiver)
                {
                    DHLShipmentRequest.Billing.ShipperAccountNumber = DraftDetail.CustomerRateCard.CourierAccountNo;
                    DHLShipmentRequest.Billing.ShippingPaymentType = "S";
                    DHLShipmentRequest.Billing.BillingAccountNumber = DraftDetail.CustomerRateCard.CourierAccountNo;
                    DHLShipmentRequest.Billing.DutyAccountNumber = DraftDetail.CustomerRateCard.CourierAccountNo;
                    DHLShipmentRequest.Billing.DutyPaymentType = "R";
                }
                if (DraftDetail.PayTaxAndDuties == FrayteShippingPaymentType.ThirdParty)
                {
                    DHLShipmentRequest.Billing.ShippingPaymentType = "T";
                    DHLShipmentRequest.Billing.ShipperAccountNumber = DraftDetail.PaymentPartyAccountNumber;
                    DHLShipmentRequest.Billing.BillingAccountNumber = DraftDetail.PaymentPartyAccountNumber;
                    DHLShipmentRequest.Billing.DutyAccountNumber = DraftDetail.PaymentPartyAccountNumber;
                    DHLShipmentRequest.Billing.DutyPaymentType = "T";
                }
                DHLShipmentRequest.Consignee = new ConsigneeDto();

                DHLShipmentRequest.Consignee.CompanyName = string.IsNullOrWhiteSpace(DraftDetail.ShipTo.CompanyName) ? DraftDetail.ShipTo.FirstName.Trim() + " " + DraftDetail.ShipTo.LastName.Trim() : DraftDetail.ShipTo.CompanyName.Trim();
                DHLShipmentRequest.Consignee.AddressLine = new List<DHLAddressDto>();
                var DHLAddressDto = new DHLAddressDto();
                DHLAddressDto.AddressLine = DraftDetail.ShipTo.Address;
                DHLShipmentRequest.Consignee.AddressLine.Add(DHLAddressDto);
                var DHLAddress1 = new DHLAddressDto();
                DHLAddress1.AddressLine = DraftDetail.ShipTo.Address2;
                DHLShipmentRequest.Consignee.AddressLine.Add(DHLAddress1);
                DHLShipmentRequest.Consignee.City = DraftDetail.ShipTo.City;
                DHLShipmentRequest.Consignee.CountryCode = DraftDetail.ShipTo.Country.Code2;
                DHLShipmentRequest.Consignee.DivisionCode = string.IsNullOrWhiteSpace(DraftDetail.ShipTo.State) ? DraftDetail.ShipTo.City : DraftDetail.ShipTo.State;
                DHLShipmentRequest.Consignee.CountryName = DraftDetail.ShipTo.Country.Name;
                DHLShipmentRequest.Consignee.Contact = new ContactDto()
                {
                    PersonName = DraftDetail.ShipTo.FirstName.Trim() + " " + DraftDetail.ShipTo.LastName.Trim(),
                    PhoneNumber = DraftDetail.ShipTo.Phone
                };
                DHLShipmentRequest.Consignee.PostalCode = DraftDetail.ShipTo.PostCode;
                DHLShipmentRequest.Dutiable = new DutiableDto()
                {
                    CurrencyCode = DraftDetail.Currency.CurrencyCode,
                    DeclaredValue = DraftDetail.Packages.Sum(s => s.Value).ToString("0.##"),

                };
                DHLShipmentRequest.Reference = new ReferenceDto()
                {
                    ReferenceID = DraftDetail.FrayteNumber + "-" + DraftDetail.ReferenceDetail.Reference1,
                    ReferenceType = "DHL",
                };
                DHLShipmentRequest.ShipmentDetails = new ShipmentDetailsDto()
                {
                    CurrencyCode = DraftDetail.Currency.CurrencyCode,
                    Date = Convert.ToDateTime(DraftDetail.ReferenceDetail.CollectionDate.Value.ToString("yyyy-MM-dd"))
                };

                string Contents = string.Empty;
                DHLShipmentRequest.ShipmentDetails.Pieces = new List<PieceDto>();
                foreach (var item in DraftDetail.Packages)
                {
                    if (item.CartoonValue > 0)
                    {
                        for (int i = 0; i < item.CartoonValue; i++)
                        {
                            var piece = new PieceDto();
                            Contents = Contents + " " + item.Content;
                            if (DraftDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms)
                            {
                                piece.Height = item.Height.ToString("0.##");
                                piece.Width = item.Width.ToString("0.##");
                                piece.Depth = item.Length.ToString("0.##");
                                piece.Weight = Convert.ToDecimal(item.Weight.ToString("0.##"));
                                piece.Value = (item.Value / item.CartoonValue).ToString("0.##");
                                piece.ProductDescription = item.Content;
                            }
                            else if (DraftDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs)
                            {
                                piece.Height = (item.Height * 1).ToString("0.##");
                                piece.Width = (item.Width * 1).ToString("0.##");
                                piece.Depth = (item.Length * 1).ToString("0.##");
                                piece.Weight = Convert.ToDecimal(item.Weight.ToString("0.##"));
                                piece.Value = (item.Value / item.CartoonValue).ToString("0.##");
                                piece.ProductDescription = item.Content;
                            }
                            DHLShipmentRequest.ShipmentDetails.Pieces.Add(piece);
                        }
                    }
                }
                var trimmedString = Contents.Length > 90 ? Contents.Substring(0, 90) : Contents;

                DHLShipmentRequest.ShipmentDetails.Contents = trimmedString;

                var TotalNumberOfPieces = 0;
                foreach (var package in DraftDetail.Packages)
                {

                    TotalNumberOfPieces += package.CartoonValue;
                }
                DHLShipmentRequest.ShipmentDetails.NumberOfPieces = TotalNumberOfPieces.ToString();
                DHLShipmentRequest.ShipmentDetails.Weight = DraftDetail.CustomerRateCard.Weight.ToString("0.##");
                DHLShipmentRequest.ShipmentDetails.DimensionUnit = DraftDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs ? "I" : "C";
                DHLShipmentRequest.ShipmentDetails.WeightUnit = DraftDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs ? "L" : "K";

                if (DHLShipmentRequest.RegionCode.Trim() == "EU")
                {
                    DHLShipmentRequest.ShipmentDetails.IsDutiable = "N";
                    if (DraftDetail.CustomerRateCard.LogisticServiceType == "UK Domestic")
                    {
                        DHLShipmentRequest.ShipmentDetails.GlobalProductCode = "N";
                        DHLShipmentRequest.ShipmentDetails.LocalProductCode = "C";
                        DHLShipmentRequest.DoorTo = "TD";
                    }
                    else
                    {
                        if (DraftDetail.CustomerRateCard.RateType == "Economy")
                        {
                            DHLShipmentRequest.ShipmentDetails.GlobalProductCode = "W";
                            DHLShipmentRequest.ShipmentDetails.LocalProductCode = "X";
                            DHLShipmentRequest.DoorTo = "TD";
                        }

                        if (DraftDetail.CustomerRateCard.RateType == "Express")
                        {
                            DHLShipmentRequest.ShipmentDetails.GlobalProductCode = "U";
                            DHLShipmentRequest.ShipmentDetails.LocalProductCode = "U";
                            DHLShipmentRequest.DoorTo = "TD";
                        }
                    }
                }
                else
                {
                    if (DraftDetail.CustomerRateCard.RateType == "Economy")
                    {
                        DHLShipmentRequest.ShipmentDetails.GlobalProductCode = "H";
                        DHLShipmentRequest.ShipmentDetails.LocalProductCode = "I";
                        DHLShipmentRequest.DoorTo = "DD";
                        DHLShipmentRequest.ShipmentDetails.IsDutiable = "Y";
                    }

                    if (DraftDetail.CustomerRateCard.RateType == "Express" && DraftDetail.ParcelType.ParcelType == "Parcel")
                    {
                        DHLShipmentRequest.ShipmentDetails.GlobalProductCode = "P";
                        DHLShipmentRequest.ShipmentDetails.LocalProductCode = "P";
                        DHLShipmentRequest.DoorTo = "TD";
                        DHLShipmentRequest.ShipmentDetails.IsDutiable = "Y";
                    }
                    if (DraftDetail.CustomerRateCard.RateType == "Express" && DraftDetail.ParcelType.ParcelType == "Letter")
                    {
                        DHLShipmentRequest.ShipmentDetails.GlobalProductCode = "D";
                        DHLShipmentRequest.ShipmentDetails.LocalProductCode = "D";
                        DHLShipmentRequest.DoorTo = "TD";
                        DHLShipmentRequest.ShipmentDetails.IsDutiable = "N";
                    }
                }

                DHLShipmentRequest.ShipmentDetails.Date = Convert.ToDateTime(DraftDetail.ReferenceDetail.CollectionDate.Value.ToString("yyyy-MM-dd"));
                DHLShipmentRequest.ShipmentDetails.Contents = DraftDetail.CustomInfo.ContentsType;

                DHLShipmentRequest.ShipmentDetails.PackageType = "EE";
                DHLShipmentRequest.Shipper = new DHLShipperDto();

                DHLShipmentRequest.Shipper.CompanyName = string.IsNullOrWhiteSpace(DraftDetail.ShipFrom.CompanyName) ? DraftDetail.ShipFrom.FirstName.Trim() + " " + DraftDetail.ShipFrom.LastName.Trim() : DraftDetail.ShipFrom.CompanyName; ;
                DHLShipmentRequest.Shipper.City = DraftDetail.ShipFrom.City;
                DHLShipmentRequest.Shipper.CountryCode = DraftDetail.ShipFrom.Country.Code2;
                DHLShipmentRequest.Shipper.CountryName = DraftDetail.ShipFrom.Country.Name;
                DHLShipmentRequest.Shipper.DivisionCode = string.IsNullOrWhiteSpace(DraftDetail.ShipFrom.State) ? DraftDetail.ShipFrom.City : DraftDetail.ShipFrom.State;
                DHLShipmentRequest.Shipper.PostalCode = DraftDetail.ShipFrom.PostCode;
                DHLShipmentRequest.Shipper.ShipperID = DraftDetail.CustomerRateCard.CourierAccountNo;

                DHLShipmentRequest.Shipper.Contact = new ContactDto()
                {
                    PersonName = DraftDetail.ShipFrom.FirstName + "" + DraftDetail.ShipFrom.LastName,
                    PhoneNumber = DraftDetail.ShipFrom.Phone
                };
                DHLShipmentRequest.Shipper.AddressLine = new List<DHLAddressDto>();
                var address1 = new DHLAddressDto();
                address1.AddressLine = DraftDetail.ShipFrom.Address;

                DHLShipmentRequest.Shipper.AddressLine.Add(address1);
                var address2 = new DHLAddressDto();
                address2.AddressLine = DraftDetail.ShipFrom.Address2;

                DHLShipmentRequest.Shipper.AddressLine.Add(address2);

                //Enable NDS ToDo Please uncomment
                if (DraftDetail.CustomerRateCard.OptionalServices != null)
                {
                    foreach (var services in DraftDetail.CustomerRateCard.OptionalServices)
                    {
                        if (services.IsEnable && services.ServiceCode == FrayteLogisticOptionServices.NDS)
                        {
                            DHLShipmentRequest.IsNDSEnable = services.IsEnable;
                        }
                    }
                }
                DHLShipmentRequest.IsPickup = DraftDetail.ReferenceDetail.IsCollection;
                DHLShipmentRequest.LabelImageFormat = "ZPL2";
                DHLShipmentRequest.CollectionTime = !string.IsNullOrWhiteSpace(DraftDetail.ReferenceDetail.CollectionTime) ? DraftDetail.ReferenceDetail.CollectionTime : "";

                return DHLShipmentRequest;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region Express Mapping 

        public DHLShipmentRequestDto MapExpressShipmentToDHLShipmentRequestDto(ExpressShipmentModel shipment)
        {
            try
            {
                DHLShipmentRequestDto DHLShipmentRequest = new DHLShipmentRequestDto();
                var RegionCode = new CountryRepository().GetCountryByNameAndContryCode(shipment.ShipTo.Country.Name, shipment.ShipTo.Country.Code).RegionCode;
                DHLShipmentRequest.Request = new DHLRequestDto();

                DHLShipmentRequest.Request.ServiceHeader = new ServiceHeaderDto();
                DHLShipmentRequest.DraftShipmentId = shipment.ExpressId;

                DHLShipmentRequest.Request.ServiceHeader.MessageReference = "DHL Core Integration";
                DHLShipmentRequest.Request.ServiceHeader.MessageTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                DHLShipmentRequest.Request.ServiceHeader.SiteID = "";
                DHLShipmentRequest.Request.ServiceHeader.Password = "";

                DHLShipmentRequest.RegionCode = RegionCode;
                DHLShipmentRequest.LanguageCode = "en";
                DHLShipmentRequest.PiecesEnabled = "Y";
                DHLShipmentRequest.Billing = new BillingDto();
                if (shipment.PayTaxAndDuties.ToUpper() == FrayteShippingPaymentType.Shipper.ToUpper())
                {
                    DHLShipmentRequest.Billing.ShippingPaymentType = "S";
                    DHLShipmentRequest.Billing.ShipperAccountNumber = shipment.Service.CourierAccountNo;
                    DHLShipmentRequest.Billing.DutyAccountNumber = shipment.Service.CourierAccountNo;
                    DHLShipmentRequest.Billing.BillingAccountNumber = shipment.Service.CourierAccountNo;
                    DHLShipmentRequest.Billing.DutyPaymentType = "S";
                }
                if (shipment.PayTaxAndDuties.ToUpper() == FrayteShippingPaymentType.Receiver.ToUpper())
                {
                    DHLShipmentRequest.Billing.ShipperAccountNumber = shipment.Service.CourierAccountNo;
                    DHLShipmentRequest.Billing.ShippingPaymentType = "S";
                    DHLShipmentRequest.Billing.BillingAccountNumber = shipment.Service.CourierAccountNo;
                    DHLShipmentRequest.Billing.DutyAccountNumber = shipment.Service.CourierAccountNo;
                    DHLShipmentRequest.Billing.DutyPaymentType = "R";
                }
                if (shipment.PayTaxAndDuties.ToUpper() == FrayteShippingPaymentType.ThirdParty.ToUpper())
                {
                    DHLShipmentRequest.Billing.ShippingPaymentType = "T";
                    DHLShipmentRequest.Billing.ShipperAccountNumber = shipment.TaxAndDutiesAccountNo;
                    DHLShipmentRequest.Billing.BillingAccountNumber = shipment.TaxAndDutiesAccountNo;
                    DHLShipmentRequest.Billing.DutyAccountNumber = shipment.TaxAndDutiesAccountNo;
                    DHLShipmentRequest.Billing.DutyPaymentType = "T";
                }
                DHLShipmentRequest.Consignee = new ConsigneeDto();

                DHLShipmentRequest.Consignee.CompanyName = string.IsNullOrWhiteSpace(shipment.ShipTo.CompanyName) ? shipment.ShipTo.FirstName.Trim() + " " + shipment.ShipTo.LastName.Trim() : shipment.ShipTo.CompanyName.Trim();
                //DHLShipmentRequest.Consignee.CompanyName = string.IsNullOrWhiteSpace(shipment.ShipTo.CompanyName) ? shipment.ShipTo.FirstName.Trim() + " " + shipment.ShipTo.LastName.Trim() : shipment.ShipTo.CompanyName.Trim();
                DHLShipmentRequest.Consignee.AddressLine = new List<DHLAddressDto>();
                var DHLAddressDto = new DHLAddressDto();
                DHLAddressDto.AddressLine = shipment.ShipTo.Address;
                DHLShipmentRequest.Consignee.AddressLine.Add(DHLAddressDto);
                var DHLAddress1 = new DHLAddressDto();
                DHLAddress1.AddressLine = shipment.ShipTo.Address2;
                DHLShipmentRequest.Consignee.AddressLine.Add(DHLAddress1);
                DHLShipmentRequest.Consignee.City = shipment.ShipTo.City;
                DHLShipmentRequest.Consignee.CountryCode = shipment.ShipTo.Country.Code2;
                DHLShipmentRequest.Consignee.DivisionCode = string.IsNullOrWhiteSpace(shipment.ShipTo.State) ? shipment.ShipTo.City : shipment.ShipTo.State;
                DHLShipmentRequest.Consignee.CountryName = shipment.ShipTo.Country.Name;
                DHLShipmentRequest.Consignee.Contact = new ContactDto()
                {
                    PersonName = shipment.ShipTo.FirstName.Trim() + " " + shipment.ShipTo.LastName.Trim(),
                    PhoneNumber = shipment.ShipTo.Phone
                };
                DHLShipmentRequest.Consignee.PostalCode = shipment.ShipTo.PostCode;
                DHLShipmentRequest.Dutiable = new DutiableDto()
                {
                    CurrencyCode = shipment.DeclaredCurrency.CurrencyCode,
                    DeclaredValue = shipment.Packages.Sum(s => s.Value).ToString("0.##"),
                };
                DHLShipmentRequest.Reference = new ReferenceDto()
                {
                    ReferenceID = shipment.FrayteNumber + "-" + shipment.ShipmentReference,
                    ReferenceType = "DHL",
                };
                DHLShipmentRequest.ShipmentDetails = new ShipmentDetailsDto()
                {
                    CurrencyCode = shipment.DeclaredCurrency.CurrencyCode,
                };

                string Contents = string.Empty;
                DHLShipmentRequest.ShipmentDetails.Pieces = new List<PieceDto>();
                foreach (var item in shipment.Packages)
                {
                    if (item.CartonValue > 0)
                    {
                        for (int i = 0; i < item.CartonValue; i++)
                        {
                            var piece = new PieceDto();
                            Contents = Contents + " " + item.Content;
                            if (shipment.PakageCalculatonType.ToUpper() == FraytePakageCalculationType.kgtoCms.ToUpper())
                            {
                                piece.Height = item.Height.ToString("0.##");
                                piece.Width = item.Width.ToString("0.##");
                                piece.Depth = item.Length.ToString("0.##");
                                piece.Weight = Convert.ToDecimal(item.Weight.ToString("0.##"));
                                piece.Value = (item.Value / item.CartonValue).ToString();
                                piece.ProductDescription = item.Content;
                            }
                            else if (shipment.PakageCalculatonType.ToUpper() == FraytePakageCalculationType.LbToInchs.ToUpper())
                            {
                                piece.Height = (item.Height * 1).ToString("0.##");
                                piece.Width = (item.Width * 1).ToString("0.##");
                                piece.Depth = (item.Length * 1).ToString("0.##");
                                piece.Weight = Convert.ToDecimal(item.Weight.ToString("0.##"));
                                piece.Value = (item.Value / item.CartonValue).ToString("0.##");
                                piece.ProductDescription = item.Content;
                            }
                            DHLShipmentRequest.ShipmentDetails.Pieces.Add(piece);
                        }
                    }
                }
                var trimmedString = Contents.Length > 90 ? Contents.Substring(0, 90) : Contents;

                DHLShipmentRequest.ShipmentDetails.Contents = trimmedString;

                var TotalNumberOfPieces = 0;
                foreach (var package in shipment.Packages)
                {
                    TotalNumberOfPieces += package.CartonValue;
                }
                DHLShipmentRequest.ShipmentDetails.NumberOfPieces = TotalNumberOfPieces.ToString();
                DHLShipmentRequest.ShipmentDetails.Weight = shipment.Packages.Sum(p => p.Weight).ToString("0.##");
                DHLShipmentRequest.ShipmentDetails.DimensionUnit = shipment.PakageCalculatonType == FraytePakageCalculationType.LbToInchs ? "I" : "C";
                DHLShipmentRequest.ShipmentDetails.WeightUnit = shipment.PakageCalculatonType == FraytePakageCalculationType.LbToInchs ? "L" : "K";

                if (DHLShipmentRequest.RegionCode.Trim() == "EU")
                {
                    DHLShipmentRequest.ShipmentDetails.IsDutiable = "N";
                    if (shipment.Service.LogisticServiceType == "UK Domestic")
                    {
                        DHLShipmentRequest.ShipmentDetails.GlobalProductCode = "N";
                        DHLShipmentRequest.ShipmentDetails.LocalProductCode = "C";
                        DHLShipmentRequest.DoorTo = "TD";
                    }
                    else
                    {
                        if (shipment.Service.RateType == "Economy")
                        {
                            DHLShipmentRequest.ShipmentDetails.GlobalProductCode = "W";
                            DHLShipmentRequest.ShipmentDetails.LocalProductCode = "X";
                            DHLShipmentRequest.DoorTo = "TD";
                        }

                        if (shipment.Service.RateType == "Express")
                        {
                            DHLShipmentRequest.ShipmentDetails.GlobalProductCode = "U";
                            DHLShipmentRequest.ShipmentDetails.LocalProductCode = "U";
                            DHLShipmentRequest.DoorTo = "TD";
                        }
                    }
                }
                else
                {
                    if (shipment.Service.RateType == "Economy")
                    {
                        DHLShipmentRequest.ShipmentDetails.GlobalProductCode = "H";
                        DHLShipmentRequest.ShipmentDetails.LocalProductCode = "I";
                        DHLShipmentRequest.DoorTo = "DD";
                        DHLShipmentRequest.ShipmentDetails.IsDutiable = "Y";
                    }
                    if (shipment.Service.RateType == "Express" && shipment.ParcelType.ParcelType == "Parcel")
                    {
                        DHLShipmentRequest.ShipmentDetails.GlobalProductCode = "P";
                        DHLShipmentRequest.ShipmentDetails.LocalProductCode = "P";
                        DHLShipmentRequest.DoorTo = "TD";
                        DHLShipmentRequest.ShipmentDetails.IsDutiable = "Y";
                    }
                    if (shipment.Service.RateType == "Express" && shipment.ParcelType.ParcelType == "Letter")
                    {
                        DHLShipmentRequest.ShipmentDetails.GlobalProductCode = "D";
                        DHLShipmentRequest.ShipmentDetails.LocalProductCode = "D";
                        DHLShipmentRequest.DoorTo = "TD";
                        DHLShipmentRequest.ShipmentDetails.IsDutiable = "N";
                    }
                    if (shipment.Service.RateType == "Express")
                    {
                        DHLShipmentRequest.ShipmentDetails.GlobalProductCode = "P";
                        DHLShipmentRequest.ShipmentDetails.LocalProductCode = "P";
                        DHLShipmentRequest.DoorTo = "TD";
                        DHLShipmentRequest.ShipmentDetails.IsDutiable = "Y";
                    }
                }

                DHLShipmentRequest.ShipmentDetails.Date = Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-dd"));
                DHLShipmentRequest.ShipmentDetails.Contents = shipment.CustomInformation.ContentsType;

                DHLShipmentRequest.ShipmentDetails.PackageType = "EE";

                DHLShipmentRequest.Shipper = new DHLShipperDto();

                var shipFromInfo = new ExpressRepository().getHubAddress(shipment.ShipTo.Country.CountryId, shipment.ShipTo.PostCode, shipment.ShipTo.State);
                DHLShipmentRequest.Shipper.CompanyName = shipFromInfo.CompanyName;
                DHLShipmentRequest.Shipper.City = shipFromInfo.City;
                DHLShipmentRequest.Shipper.CountryCode = shipFromInfo.Country.Code2;
                DHLShipmentRequest.Shipper.CountryName = shipFromInfo.Country.Name;
                DHLShipmentRequest.Shipper.DivisionCode = string.IsNullOrWhiteSpace(shipFromInfo.State) ? shipFromInfo.City : shipFromInfo.State;
                DHLShipmentRequest.Shipper.PostalCode = shipFromInfo.PostCode;
                DHLShipmentRequest.Shipper.Contact = new ContactDto()
                {
                    PersonName = shipFromInfo.CompanyName,
                    PhoneNumber = shipFromInfo.Phone
                };
                DHLShipmentRequest.Shipper.AddressLine = new List<DHLAddressDto>();
                var address1 = new DHLAddressDto();
                address1.AddressLine = shipFromInfo.Address;
                DHLShipmentRequest.Shipper.AddressLine.Add(address1);
                var address2 = new DHLAddressDto();
                address2.AddressLine = shipFromInfo.Address2;
                DHLShipmentRequest.Shipper.AddressLine.Add(address2);
                DHLShipmentRequest.Shipper.ShipperID = shipment.Service.CourierAccountNo;

                //Enable NDS ToDo Please uncomment
                if (shipment.Service.OptionalServices != null)
                {
                    foreach (var services in shipment.Service.OptionalServices)
                    {
                        if (services.IsEnable && services.ServiceCode == FrayteLogisticOptionServices.NDS)
                        {
                            DHLShipmentRequest.IsNDSEnable = services.IsEnable;
                        }
                    }
                }

                DHLShipmentRequest.LabelImageFormat = "ZPL2";
                //DHLShipmentRequest.CollectionTime = !string.IsNullOrWhiteSpace(shipment.CreatedOnTime) ? shipment.CreatedOnTime : "";
                return DHLShipmentRequest;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string ExpressDownloadDHLImage(CourierPieceDetail pieceDetails, int totalPiece, int count, int shipmentId)
        {
            string Image = string.Empty;
            bool isAll = true;
            //byte[] byteInfo = Convert.FromBase64String(pieceDetails.ImageByte);
            try
            {
                string labelName = string.Empty;
                labelName = FrayteShortName.DHL;
                if (count == 0)
                {
                    string PieceTrackingNumber = null;
                    if (pieceDetails.PieceTrackingNumber.Contains("AirwayBillNumber_"))
                    {
                        PieceTrackingNumber = pieceDetails.PieceTrackingNumber.Replace("AirwayBillNumber_", "");
                    }
                    Image = labelName + "_" + PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".jpg";
                    count = totalPiece + 1;
                    isAll = false;
                }
                else
                {
                    Image = labelName + "_" + pieceDetails.PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".jpg";
                    isAll = false;
                }

                if (AppSettings.LabelSave == "")
                {
                    if (Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/Express/" + shipmentId + "/"))
                    {
                        var response = ZPL2WebApiRespone(pieceDetails.ImageByte, count, isAll);
                        var responseStream = response.GetResponseStream();
                        var fileStream = File.Create(AppSettings.WebApiPath + "/PackageLabel/Express/" + shipmentId + "/" + Image); // change file name for PNG images
                        responseStream.CopyTo(fileStream);
                        responseStream.Close();
                        fileStream.Close();
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/Express/" + shipmentId + "/");
                        var response = ZPL2WebApiRespone(pieceDetails.ImageByte, count, isAll);
                        var responseStream = response.GetResponseStream();
                        var fileStream = File.Create(AppSettings.WebApiPath + "/PackageLabel/Express/" + shipmentId + "/" + Image); // change file name for PNG images
                        responseStream.CopyTo(fileStream);
                        responseStream.Close();
                        fileStream.Close();
                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(AppSettings.LabelFolder + "Express/" + shipmentId + "/"))
                    {
                        var response = ZPL2WebApiRespone(pieceDetails.ImageByte, count, isAll);
                        var responseStream = response.GetResponseStream();
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            var fileStream = File.Create(AppSettings.LabelFolder + "Express/" + shipmentId + "/" + Image); // change file name for PNG images
                            responseStream.CopyTo(fileStream);
                            responseStream.Close();
                            fileStream.Close();
                        }
                        else
                        {
                            var fileStream = File.Create(HostingEnvironment.MapPath(AppSettings.LabelFolder + "Express/" + shipmentId) + "/" + Image); // change file name for PNG images
                            responseStream.CopyTo(fileStream);
                            responseStream.Close();
                            fileStream.Close();
                        }
                    }
                    else
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "Express/" + shipmentId);
                            var response = ZPL2WebApiRespone(pieceDetails.ImageByte, count, isAll);
                            var responseStream = response.GetResponseStream();
                            var fileStream = File.Create(AppSettings.LabelFolder + "Express/" + shipmentId + "/" + Image); // change file name for PNG images
                            responseStream.CopyTo(fileStream);
                            responseStream.Close();
                            fileStream.Close();
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "Express/" + shipmentId));
                            var response = ZPL2WebApiRespone(pieceDetails.ImageByte, count, isAll);
                            var responseStream = response.GetResponseStream();
                            var fileStream = File.Create(HostingEnvironment.MapPath(AppSettings.LabelFolder + "Express/" + shipmentId) + "/" + Image); // change file name for PNG images
                            responseStream.CopyTo(fileStream);
                            responseStream.Close();
                            fileStream.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ex.ToString();
            }

            //Step16.1: Create direcory for save package label
            return Image;
        }

        #endregion

        public string CreateXMLForDHL(DHLShipmentRequestDto dhlRequestObject)
        {
            string xmlPath = string.Empty;

            try
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();

                xmlWriterSettings.Encoding = new UTF8Encoding(true);

                xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;

                xmlWriterSettings.Indent = true;
                var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.DHL);

                dhlRequestObject.Request.ServiceHeader.SiteID = logisticIntegration.UserName;

                dhlRequestObject.Request.ServiceHeader.Password = logisticIntegration.Password;

                if (AppSettings.LabelSave == "")
                {
                    xmlPath = AppSettings.WebApiPath + "/UploadFiles/PDFGenerator/HTMLFile";
                }
                else
                {
                    if (AppSettings.ShipmentCreatedFrom == "BATCH")
                    {
                        xmlPath = AppSettings.WebApiPath + "/UploadFiles/PDFGenerator/HTMLFile";
                    }
                    else
                    {
                        xmlPath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");
                    }
                }

                if (!Directory.Exists(xmlPath))
                {
                    Directory.CreateDirectory(xmlPath);
                }

                xmlPath = xmlPath + "/tempDHLShipment.xml";

                if (File.Exists(xmlPath))
                {
                    File.Delete(xmlPath);
                }

                using (var xmlWriter = XmlWriter.Create(xmlPath))
                {
                    xmlWriter.WriteStartDocument();
                    if (logisticIntegration != null)
                    {
                        xmlWriter.WriteStartElement("req", "ShipmentRequest", "http://www.dhl.com");
                        xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                        xmlWriter.WriteAttributeString(null, "schemaVersion", null, "5.0");
                        xmlWriter.WriteAttributeString("xsi", "schemaLocation", null, "http://www.dhl.com ship-val-global-req.xsd");

                        //RequestNode
                        CreateRequestNode(xmlWriter, dhlRequestObject);

                        if (dhlRequestObject.RegionCode.Trim() == "AM")
                        {
                            xmlWriter.WriteStartElement("RegionCode");
                            xmlWriter.WriteString(dhlRequestObject.RegionCode.Trim());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("RequestedPickupTime");
                            xmlWriter.WriteString("Y");
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("NewShipper");
                            xmlWriter.WriteString("Y");
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("LanguageCode");
                            xmlWriter.WriteString(dhlRequestObject.LanguageCode);
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("PiecesEnabled");
                            xmlWriter.WriteString(dhlRequestObject.PiecesEnabled);
                            xmlWriter.WriteEndElement();
                        }
                        if (dhlRequestObject.RegionCode.Trim() == "AP")
                        {
                            xmlWriter.WriteStartElement("RegionCode");
                            xmlWriter.WriteString(dhlRequestObject.RegionCode.Trim());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("LanguageCode");
                            xmlWriter.WriteString(dhlRequestObject.LanguageCode);
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("PiecesEnabled");
                            xmlWriter.WriteString(dhlRequestObject.PiecesEnabled);
                            xmlWriter.WriteEndElement();
                        }
                        if (dhlRequestObject.RegionCode.Trim() == "EU")
                        {
                            xmlWriter.WriteStartElement("RegionCode");
                            xmlWriter.WriteString(dhlRequestObject.RegionCode.Trim());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("NewShipper");
                            xmlWriter.WriteString("N");
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("LanguageCode");
                            xmlWriter.WriteString(dhlRequestObject.LanguageCode);
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("PiecesEnabled");
                            xmlWriter.WriteString(dhlRequestObject.PiecesEnabled);
                            xmlWriter.WriteEndElement();
                        }

                        //BillingNode
                        CreateBilling(xmlWriter, dhlRequestObject);
                        //2. Create Consignee
                        Consignee(xmlWriter, dhlRequestObject);

                        if (dhlRequestObject.RegionCode.Trim() != "EU")
                        {
                            xmlWriter.WriteStartElement("Dutiable");

                            xmlWriter.WriteStartElement("DeclaredValue");

                            int DeclaredValue = 0;
                            int.TryParse(dhlRequestObject.Dutiable.DeclaredValue, out DeclaredValue);
                            string dhlValue = DeclaredValue == 0 ? "0.01" : DeclaredValue.ToString();
                            xmlWriter.WriteString(dhlValue);
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("DeclaredCurrency");
                            xmlWriter.WriteString(dhlRequestObject.Dutiable.CurrencyCode);
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("ScheduleB");
                            xmlWriter.WriteString("");
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("ExportLicense");
                            xmlWriter.WriteString("");
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("ShipperEIN");
                            xmlWriter.WriteString("");
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("ShipperIDType");
                            xmlWriter.WriteString("S");
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("ImportLicense");
                            xmlWriter.WriteString("");
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("ConsigneeEIN");
                            xmlWriter.WriteString("");
                            xmlWriter.WriteEndElement();

                            if (dhlRequestObject.Billing.DutyPaymentType == "S")
                            {
                                xmlWriter.WriteStartElement("TermsOfTrade");
                                xmlWriter.WriteString("DDP");
                                xmlWriter.WriteEndElement();
                            }
                            if (dhlRequestObject.Billing.DutyPaymentType == "R")
                            {
                                xmlWriter.WriteStartElement("TermsOfTrade");
                                xmlWriter.WriteString("DDU");
                                xmlWriter.WriteEndElement();
                            }
                            if (dhlRequestObject.Billing.DutyPaymentType == "T")
                            {
                                xmlWriter.WriteStartElement("TermsOfTrade");
                                xmlWriter.WriteString("DDU");
                                xmlWriter.WriteEndElement();
                            }

                            xmlWriter.WriteEndElement();
                            //3. Export Declaration Node 
                            ExportDeclarationNode(xmlWriter, dhlRequestObject);
                        }

                        xmlWriter.WriteStartElement("Reference");

                        xmlWriter.WriteStartElement("ReferenceID");
                        xmlWriter.WriteString(dhlRequestObject.Reference.ReferenceID);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("ReferenceType");
                        xmlWriter.WriteString(dhlRequestObject.Reference.ReferenceType);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        ShipmentDetails(xmlWriter, dhlRequestObject);

                        ShipperDetails(xmlWriter, dhlRequestObject);
                        //Enable NDS:-
                        if (dhlRequestObject.IsNDSEnable)
                        {
                            xmlWriter.WriteStartElement("SpecialService");
                            xmlWriter.WriteStartElement("SpecialServiceType");
                            xmlWriter.WriteString("NN");
                            xmlWriter.WriteEndElement();
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteStartElement("LabelImageFormat");
                        xmlWriter.WriteString(dhlRequestObject.LabelImageFormat);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndDocument();
                }
                return xmlPath;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static void CreateRequestNode(XmlWriter xmlWriter, DHLShipmentRequestDto dhlRequestDto)
        {
            xmlWriter.WriteStartElement("Request");
            xmlWriter.WriteStartElement("ServiceHeader");

            xmlWriter.WriteStartElement("MessageTime");
            xmlWriter.WriteString(dhlRequestDto.Request.ServiceHeader.MessageTime);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("MessageReference");
            xmlWriter.WriteString("DHL Shipment Integration Api");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("SiteID");
            xmlWriter.WriteString(dhlRequestDto.Request.ServiceHeader.SiteID);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Password");
            xmlWriter.WriteString(dhlRequestDto.Request.ServiceHeader.Password);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        private static void CreateBilling(XmlWriter xmlWriter, DHLShipmentRequestDto dhlRequestDto)
        {
            xmlWriter.WriteStartElement("Billing");

            if (dhlRequestDto.Billing.ShippingPaymentType == "S")
            {
                xmlWriter.WriteStartElement("ShipperAccountNumber");
                xmlWriter.WriteString(dhlRequestDto.Billing.ShipperAccountNumber);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("ShippingPaymentType");
                xmlWriter.WriteString(dhlRequestDto.Billing.ShippingPaymentType);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("DutyPaymentType");
                xmlWriter.WriteString(dhlRequestDto.Billing.DutyPaymentType);
                xmlWriter.WriteEndElement();
            }
            if (dhlRequestDto.Billing.ShippingPaymentType == "R")
            {
                xmlWriter.WriteStartElement("ShipperAccountNumber");
                xmlWriter.WriteString(dhlRequestDto.Billing.ShipperAccountNumber);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("ShippingPaymentType");
                xmlWriter.WriteString(dhlRequestDto.Billing.ShippingPaymentType);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("DutyPaymentType");
                xmlWriter.WriteString(dhlRequestDto.Billing.DutyPaymentType);
                xmlWriter.WriteEndElement();
            }

            if (dhlRequestDto.Billing.ShippingPaymentType == "T")
            {
                xmlWriter.WriteStartElement("ShipperAccountNumber");
                xmlWriter.WriteString(dhlRequestDto.Billing.ShipperAccountNumber);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("ShippingPaymentType");
                xmlWriter.WriteString(dhlRequestDto.Billing.ShippingPaymentType);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("BillingAccountNumber");
                xmlWriter.WriteString(dhlRequestDto.Billing.BillingAccountNumber);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("DutyPaymentType");
                xmlWriter.WriteString(dhlRequestDto.Billing.DutyPaymentType);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("DutyAccountNumber");
                xmlWriter.WriteString(dhlRequestDto.Billing.DutyAccountNumber);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }

        private static void Consignee(XmlWriter xmlWriter, DHLShipmentRequestDto dhlRequestDto)
        {
            xmlWriter.WriteStartElement("Consignee");

            xmlWriter.WriteStartElement("CompanyName");
            xmlWriter.WriteString(dhlRequestDto.Consignee.CompanyName);
            xmlWriter.WriteEndElement();

            foreach (var address in dhlRequestDto.Consignee.AddressLine)
            {
                xmlWriter.WriteStartElement("AddressLine");
                xmlWriter.WriteString(address.AddressLine);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteStartElement("City");
            xmlWriter.WriteString(dhlRequestDto.Consignee.City);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("DivisionCode");
            xmlWriter.WriteString(dhlRequestDto.Consignee.DivisionCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("PostalCode");
            xmlWriter.WriteString(dhlRequestDto.Consignee.PostalCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CountryCode");
            xmlWriter.WriteString(dhlRequestDto.Consignee.CountryCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CountryName");
            xmlWriter.WriteString(dhlRequestDto.Consignee.CountryName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Contact");

            xmlWriter.WriteStartElement("PersonName");
            xmlWriter.WriteString(dhlRequestDto.Consignee.Contact.PersonName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("PhoneNumber");
            xmlWriter.WriteString(dhlRequestDto.Consignee.Contact.PhoneNumber);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }

        private static void ShipmentDetails(XmlWriter xmlWriter, DHLShipmentRequestDto dhlRequestDto)
        {
            xmlWriter.WriteStartElement("ShipmentDetails");

            xmlWriter.WriteStartElement("NumberOfPieces");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.NumberOfPieces);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Pieces");

            foreach (var Piece in dhlRequestDto.ShipmentDetails.Pieces)
            {
                xmlWriter.WriteStartElement("Piece");

                xmlWriter.WriteStartElement("Weight");
                xmlWriter.WriteString(Piece.Weight.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Width");
                xmlWriter.WriteString(Piece.Width.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Height");
                xmlWriter.WriteString(Piece.Height.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Depth");
                xmlWriter.WriteString(Piece.Depth.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Weight");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.Weight.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("WeightUnit");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.WeightUnit.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("GlobalProductCode");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.GlobalProductCode.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("LocalProductCode");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.LocalProductCode.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Date");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.Date.ToString("yyyy-MM-dd"));
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Contents");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.Contents);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("DimensionUnit");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.DimensionUnit.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("PackageType");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.PackageType);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("IsDutiable");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.IsDutiable);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CurrencyCode");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.CurrencyCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }

        private static void PickupShipmentDetails(XmlWriter xmlWriter, DHLShipmentRequestDto dhlRequestDto)
        {
            xmlWriter.WriteStartElement("ShipmentDetails");

            xmlWriter.WriteStartElement("AccountType");
            xmlWriter.WriteString("D");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("AccountNumber");
            xmlWriter.WriteString(dhlRequestDto.Billing.ShipperAccountNumber);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("BillToAccountNumber");
            xmlWriter.WriteString(dhlRequestDto.Billing.ShipperAccountNumber);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("AWBNumber");
            xmlWriter.WriteString(dhlRequestDto.AWBNumber);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("NumberOfPieces");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.NumberOfPieces);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Weight");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.Weight);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("WeightUnit");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.WeightUnit);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("GlobalProductCode");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.GlobalProductCode);
            xmlWriter.WriteEndElement();

            //xmlWriter.WriteStartElement("LocalProductCode");
            //xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.LocalProductCode);
            //xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("DoorTo");
            xmlWriter.WriteString("DD");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("DimensionUnit");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.DimensionUnit);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Pieces");
            var pieces = dhlRequestDto.ShipmentDetails.Pieces.FirstOrDefault();

            xmlWriter.WriteStartElement("Weight");
            xmlWriter.WriteString(pieces.Weight.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Width");
            xmlWriter.WriteString(pieces.Width.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Height");
            xmlWriter.WriteString(pieces.Height.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Depth");
            xmlWriter.WriteString(pieces.Depth.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }

        private static void ShipperDetails(XmlWriter xmlWriter, DHLShipmentRequestDto dhlRequestDto)
        {
            xmlWriter.WriteStartElement("Shipper");

            xmlWriter.WriteStartElement("ShipperID");
            xmlWriter.WriteString(dhlRequestDto.Shipper.ShipperID);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CompanyName");
            xmlWriter.WriteString(dhlRequestDto.Shipper.CompanyName);
            xmlWriter.WriteEndElement();

            foreach (var address in dhlRequestDto.Shipper.AddressLine)
            {
                xmlWriter.WriteStartElement("AddressLine");
                xmlWriter.WriteString(address.AddressLine);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteStartElement("City");
            xmlWriter.WriteString(dhlRequestDto.Shipper.City);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("DivisionCode");
            xmlWriter.WriteString(dhlRequestDto.Shipper.DivisionCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("PostalCode");
            xmlWriter.WriteString(dhlRequestDto.Shipper.PostalCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CountryCode");
            xmlWriter.WriteString(dhlRequestDto.Shipper.CountryCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CountryName");
            xmlWriter.WriteString(dhlRequestDto.Shipper.CountryName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Contact");

            xmlWriter.WriteStartElement("PersonName");
            xmlWriter.WriteString(dhlRequestDto.Shipper.Contact.PersonName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("PhoneNumber");
            xmlWriter.WriteString(dhlRequestDto.Shipper.Contact.PhoneNumber);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }

        private static void ExportDeclarationNode(XmlWriter xmlWriter, DHLShipmentRequestDto dhlRequestDto)
        {
            xmlWriter.WriteStartElement("ExportDeclaration");
            var count = 1;
            foreach (var item in dhlRequestDto.ShipmentDetails.Pieces)
            {
                xmlWriter.WriteStartElement("ExportLineItem");

                xmlWriter.WriteStartElement("LineNumber");
                xmlWriter.WriteString(count.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Quantity");
                xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.NumberOfPieces);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("QuantityUnit");
                xmlWriter.WriteString("PCS");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Description");
                xmlWriter.WriteString(!string.IsNullOrWhiteSpace(item.ProductDescription) ? item.ProductDescription : " product description");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Value");
                xmlWriter.WriteString(item.Value);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Weight");

                xmlWriter.WriteStartElement("Weight");
                xmlWriter.WriteString(item.Weight.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("WeightUnit");
                xmlWriter.WriteString("K");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("ManufactureCountryCode");
                xmlWriter.WriteString(dhlRequestDto.Shipper.CountryCode);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();

                count++;
            }
            xmlWriter.WriteEndElement();
        }

        public string CreateXMLForDHLPickup(DHLShipmentRequestDto dhlRequestObject, string PickupCutoffTime, string Pickupreadytime)
        {
            string xmlPath = string.Empty;
            try
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = new UTF8Encoding(true);
                xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
                xmlWriterSettings.Indent = true;
                var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.DHL);

                dhlRequestObject.Request.ServiceHeader.SiteID = logisticIntegration.UserName;
                dhlRequestObject.Request.ServiceHeader.Password = logisticIntegration.Password;

                //xmlPath = It will be the path where xml will saved  

                if (AppSettings.LabelSave == "")
                {
                    xmlPath = AppSettings.WebApiPath + "/UploadFiles/PDFGenerator/HTMLFile";
                }
                else
                {
                    if (AppSettings.ShipmentCreatedFrom == "BATCH")
                    {
                        xmlPath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/HTMLFile";
                    }
                    else
                    {
                        xmlPath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");
                    }
                }

                if (!Directory.Exists(xmlPath))
                {
                    Directory.CreateDirectory(xmlPath);
                }

                xmlPath = xmlPath + "/tempDHLPickup.xml";

                if (File.Exists(xmlPath))
                {
                    File.Delete(xmlPath);
                }
                using (var xmlWriter = XmlWriter.Create(xmlPath))
                {
                    xmlWriter.WriteStartDocument();
                    if (logisticIntegration != null)
                    {
                        xmlWriter.WriteStartElement("req", "BookPURequest", "http://www.dhl.com");
                        xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                        xmlWriter.WriteAttributeString("xsi", "schemaLocation", null, "http://www.dhl.com book-pickup-global-req.xsd");
                        xmlWriter.WriteAttributeString(null, "schemaVersion", null, "1.0");
                        //RequestNode
                        CreateRequestNode(xmlWriter, dhlRequestObject);

                        xmlWriter.WriteStartElement("RegionCode");
                        xmlWriter.WriteString(!string.IsNullOrWhiteSpace(dhlRequestObject.RegionCode) ? dhlRequestObject.RegionCode.Trim() : "");
                        xmlWriter.WriteEndElement();
                        
                        //Requestor
                        CreateRequestorNode(xmlWriter, dhlRequestObject);

                        //place
                        CreatePlaceNode(xmlWriter, dhlRequestObject);

                        //Pickup
                        CreatePickupNode(xmlWriter, dhlRequestObject, PickupCutoffTime, Pickupreadytime);

                        // PickupContact
                        xmlWriter.WriteStartElement("PickupContact");

                        xmlWriter.WriteStartElement("PersonName");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.Contact.PersonName.ToString());
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Phone");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.Contact.PhoneNumber.ToString());
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("PhoneExtension");
                        xmlWriter.WriteString("123");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        //ShipmentDetails
                        PickupShipmentDetails(xmlWriter, dhlRequestObject);

                    }
                    xmlWriter.WriteEndDocument();
                }
                return xmlPath;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static void CreateRequestorNode(XmlWriter xmlWriter, DHLShipmentRequestDto dhlRequestDto)
        {
            xmlWriter.WriteStartElement("Requestor");

            xmlWriter.WriteStartElement("AccountType");
            xmlWriter.WriteString("D");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("AccountNumber");
            xmlWriter.WriteString(dhlRequestDto.Billing.ShipperAccountNumber);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("RequestorContact");

            xmlWriter.WriteStartElement("PersonName");
            xmlWriter.WriteString(dhlRequestDto.Shipper.Contact.PersonName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Phone");
            xmlWriter.WriteString(dhlRequestDto.Shipper.Contact.PhoneNumber);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("PhoneExtension");
            xmlWriter.WriteString("123");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CompanyName");
            xmlWriter.WriteString(dhlRequestDto.Shipper.CompanyName);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        private static void CreatePlaceNode(XmlWriter xmlWriter, DHLShipmentRequestDto dhlRequestDto)
        {
            xmlWriter.WriteStartElement("Place");

            xmlWriter.WriteStartElement("LocationType");
            xmlWriter.WriteString("B");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CompanyName");
            xmlWriter.WriteString(dhlRequestDto.Shipper.CompanyName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Address1");
            xmlWriter.WriteString(dhlRequestDto.Shipper.AddressLine[0].AddressLine);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Address2");
            xmlWriter.WriteString(!string.IsNullOrWhiteSpace(dhlRequestDto.Shipper.AddressLine[1].AddressLine) ? dhlRequestDto.Shipper.AddressLine[1].AddressLine : "");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("PackageLocation");
            xmlWriter.WriteString(dhlRequestDto.Shipper.DivisionCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("City");
            xmlWriter.WriteString(dhlRequestDto.Shipper.City);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CountryCode");
            xmlWriter.WriteString(dhlRequestDto.Shipper.CountryCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("PostalCode");
            xmlWriter.WriteString(dhlRequestDto.Shipper.PostalCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }

        private static void CreatePickupNode(XmlWriter xmlWriter, DHLShipmentRequestDto dhlRequestDto, string PickupCutoffTime, string CapabilityReadyTime)
        {

            DateTime mindatetime = dhlRequestDto.ShipmentDetails.Date;

            if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Sunday)
            {
                mindatetime = dhlRequestDto.ShipmentDetails.Date.AddDays(1);
            }
            //else if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Saturday)
            //{

            //    mindatetime = dhlRequestDto.ShipmentDetails.Date.AddDays(2);
            //}
            xmlWriter.WriteStartElement("Pickup");

            xmlWriter.WriteStartElement("PickupDate");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.Date.ToString("yyyy-MM-dd"));
            xmlWriter.WriteEndElement();

            #region
            if (!string.IsNullOrWhiteSpace(PickupCutoffTime))
            {
                int pct = Convert.ToInt32(PickupCutoffTime);
                if (PickupCutoffTime.Length == 2)
                {

                    PickupCutoffTime = PickupCutoffTime + "00";

                }
                if (PickupCutoffTime.Length == 1)
                {
                    PickupCutoffTime = PickupCutoffTime + "00";
                }
                if (CapabilityReadyTime.Length == 2)
                {
                    CapabilityReadyTime = CapabilityReadyTime + "00";
                }
                if (CapabilityReadyTime.Length == 1)
                {
                    CapabilityReadyTime = CapabilityReadyTime + "00";
                }
            }
            var CutoffTime = GetFormmatedTime(PickupCutoffTime);
            TimeSpan cts = TimeSpan.Parse(CutoffTime);

            string URT = GetFormmatedTime(dhlRequestDto.CollectionTime);
            TimeSpan urt = TimeSpan.Parse(URT);
            string CRT = GetFormmatedTime(CapabilityReadyTime);
            TimeSpan crt = TimeSpan.Parse(CRT);

            TimeSpan readyTime = new TimeSpan();

            if (urt <= crt)
            {
                readyTime = urt;

                if (cts.Hours - urt.Hours < 1)
                {
                    readyTime = cts.Subtract(TimeSpan.FromHours(1));
                }
            }
            else
            {
                readyTime = crt;
                if (cts.Hours - readyTime.Hours < 1)
                {
                    readyTime = cts.Subtract(TimeSpan.FromHours(1));
                }
            }

            #endregion

            xmlWriter.WriteStartElement("ReadyByTime");
            xmlWriter.WriteString(readyTime.ToString("hh':'mm"));
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CloseTime");
            xmlWriter.WriteString(cts.ToString("hh':'mm"));
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Pieces");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.Pieces.Count().ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("weight");

            xmlWriter.WriteStartElement("Weight");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.Weight);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("WeightUnit");
            xmlWriter.WriteString("K");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }

        private static object GetFormmatedTime()
        {
            throw new NotImplementedException();
        }

        private static string CallWebservice(string body, string serverUrl)
        {
            try
            {
                WebRequest requestRate = HttpWebRequest.Create(serverUrl);
                requestRate.ContentType = "application/x-www-form-urlencoded";
                requestRate.Method = "POST";

                using (var stream = requestRate.GetRequestStream())
                {
                    var arrBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(body);
                    stream.Write(arrBytes, 0, arrBytes.Length);
                    stream.Close();
                }

                WebResponse responseRate = requestRate.GetResponse();
                var respStream = responseRate.GetResponseStream();
                var reader = new StreamReader(respStream, System.Text.Encoding.ASCII);
                string strResponse = reader.ReadToEnd();
                respStream.Close();
                return strResponse;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        private static string ReadXMLDocument(string xml_in)
        {
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(xml_in));

            XElement xmlFile = XElement.Load(ms);

            var status = xmlFile.Descendants("Note").Elements("ActionNote");
            string val = GetElementValue(status);

            return val;
        }

        private static string GetElementValue(IEnumerable<XElement> elements)
        {
            foreach (var sat in elements)
            {
                return sat.Value;
            }

            return "";
        }

        public IntegrtaionResult MapDHLIntegrationResponse(DHLResponseDto DHLResponse)
        {
            IntegrtaionResult integrtaionResult = new IntegrtaionResult();

            integrtaionResult.IntegrationReponse = DHLResponse.ServiceResponse;

            if (DHLResponse.Status)
            {
                integrtaionResult.Status = true;
                integrtaionResult.ShipmentImage = DHLResponse.ShipmentImange;
                integrtaionResult.PickupRef = DHLResponse.PickupRef;
                integrtaionResult.CourierName = FrayteCourierCompany.DHL;
                integrtaionResult.TrackingNumber = DHLResponse.DHLOrderId;
                integrtaionResult.PieceTrackingDetails = new List<CourierPieceDetail>();
                foreach (var data in DHLResponse.Pieces)
                {
                    CourierPieceDetail obj = new CourierPieceDetail();
                    obj.DirectShipmentDetailId = 0;
                    obj.PieceTrackingNumber = data.LicensePlate;
                    obj.ImageByte = Base64Decode(DHLResponse.ImageString);
                    integrtaionResult.PieceTrackingDetails.Add(obj);
                }
            }
            else
            {
                integrtaionResult.Error = new FratyteError();
                integrtaionResult.Error.Address = new List<string>();
                {
                    var error = DHLResponse.Error.ErrorCode + "/" + DHLResponse.Error.ErrorDescription;
                    integrtaionResult.Error.Address.Add(error);
                }
                integrtaionResult.ErrorCode = new List<FrayteApiError>();
                {
                    List<FrayteApiError> _api = new FrayteApiErrorCodeRepository().SaveFinilizeApiError(integrtaionResult.Error, FrayteLogisticServiceType.DHL, null);
                    integrtaionResult.ErrorCode = _api;
                }
                integrtaionResult.Status = false;
            }
            return integrtaionResult;
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public string DownloadDHLImage(CourierPieceDetail pieceDetails, int totalPiece, int count, int DirectShipmentid)
        {
            string Image = string.Empty;
            bool isAll = false;
            //byte[] byteInfo = Convert.FromBase64String(pieceDetails.ImageByte);
            try
            {
                string labelName = string.Empty;
                labelName = FrayteShortName.DHL;
                if (count == 0)
                {
                    string PieceTrackingNumber = null;
                    if (pieceDetails.PieceTrackingNumber.Contains("AirwayBillNumber_"))
                    {
                        PieceTrackingNumber = pieceDetails.PieceTrackingNumber.Replace("AirwayBillNumber_", "");
                    }
                    Image = labelName + "_" + PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".jpg";
                    count = totalPiece + 1;
                    isAll = true;
                }
                else
                {
                    Image = labelName + "_" + pieceDetails.PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".jpg";
                    isAll = false;
                }

                if (AppSettings.LabelSave == "")
                {
                    if (Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/"))
                    {
                        var response = ZPL2WebApiRespone(pieceDetails.ImageByte, count, isAll);
                        var responseStream = response.GetResponseStream();
                        var fileStream = File.Create(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/" + Image); // change file name for PNG images
                        responseStream.CopyTo(fileStream);
                        responseStream.Close();
                        fileStream.Close();
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/");
                        var response = ZPL2WebApiRespone(pieceDetails.ImageByte, count, isAll);
                        var responseStream = response.GetResponseStream();
                        var fileStream = File.Create(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/" + Image); // change file name for PNG images
                        responseStream.CopyTo(fileStream);
                        responseStream.Close();
                        fileStream.Close();
                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(AppSettings.LabelFolder + "/" + DirectShipmentid + "/"))
                    {
                        var response = ZPL2WebApiRespone(pieceDetails.ImageByte, count, isAll);
                        var responseStream = response.GetResponseStream();
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            var fileStream = File.Create(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image); // change file name for PNG images
                            responseStream.CopyTo(fileStream);
                            responseStream.Close();
                            fileStream.Close();
                        }
                        else
                        {
                            var fileStream = File.Create(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid) + "/" + Image); // change file name for PNG images
                            responseStream.CopyTo(fileStream);
                            responseStream.Close();
                            fileStream.Close();
                        }
                    }
                    else
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "/" + DirectShipmentid);
                            var response = ZPL2WebApiRespone(pieceDetails.ImageByte, count, isAll);
                            var responseStream = response.GetResponseStream();
                            var fileStream = File.Create(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image); // change file name for PNG images
                            responseStream.CopyTo(fileStream);
                            responseStream.Close();
                            fileStream.Close();
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid));
                            var response = ZPL2WebApiRespone(pieceDetails.ImageByte, count, isAll);
                            var responseStream = response.GetResponseStream();
                            var fileStream = File.Create(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid) + "/" + Image); // change file name for PNG images
                            responseStream.CopyTo(fileStream);
                            responseStream.Close();
                            fileStream.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ex.ToString();
            }

            //Step16.1: Create direcory for save package label
            return Image;
        }

        private WebResponse ZPL2WebApiRespone(string ImageByte, int count, bool isAll)
        {
            count = count - 1;
            byte[] zpl = Encoding.UTF8.GetBytes(ImageByte);

            // adjust print density (8dpmm), label width (4 inches), label height (6 inches), and label index (0) as necessary
            HttpWebRequest request;
            if (isAll)
            {
                request = (HttpWebRequest)WebRequest.Create("http://api.labelary.com/v1/printers/8dpmm/labels/4x8" + "/" + count + "/");
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create("http://api.labelary.com/v1/printers/8dpmm/labels/4x8" + "/" + count + "/");
            }

            request.Method = "POST";
            request.Accept = "image/png"; // omit this line to get PNG images back
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = zpl.Length;

            var requestStream = request.GetRequestStream();
            requestStream.Write(zpl, 0, zpl.Length);
            requestStream.Close();
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseStream = response.GetResponseStream();
                return response;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public bool SaveTrackingDetail(DirectBookingShipmentDraftDetail directBookingDetail, IntegrtaionResult result, int DirectShipmentid)
        {
            var count = 1;
            foreach (var Obj in result.PieceTrackingDetails)
            {
                if (!Obj.PieceTrackingNumber.Contains("AirwayBillNumber_"))
                {
                    Frayte.Services.Models.Package package = new Frayte.Services.Models.Package();
                    package.DirectShipmentDetailId = Obj.DirectShipmentDetailId;

                    package.LabelName = Obj.PieceTrackingNumber;
                    new DirectShipmentRepository().SavePackageDetail(package, "", Obj.DirectShipmentDetailId, directBookingDetail.CustomerRateCard.CourierName, count);
                    count++;
                }
            }
            return true;
        }

        public void MappingCourierPieceDetail(IntegrtaionResult integrtaionResult, DirectBookingShipmentDraftDetail directBookingDetail, int DirectShipmentid)
        {
            if (DirectShipmentid > 0)
            {
                int k = 0, i = 0;
                List<int> _shiId = new List<int>();
                foreach (var Obj in directBookingDetail.Packages)
                {
                    _shiId = new DirectShipmentRepository().GetDirectShipmentDetailID(DirectShipmentid);
                    for (int j = 1; j <= Obj.CartoonValue; j++)
                    {
                        integrtaionResult.PieceTrackingDetails[k].DirectShipmentDetailId = _shiId[i];
                        k++;
                    }
                    i++;
                }
            }
        }

        private static string GetFormmatedTime(string time)
        {
            if (!string.IsNullOrEmpty(time))
            {
                if (time.Length == 3)
                {
                    time = "0" + time;
                }
                if (time.Contains(":"))
                {
                    time = time.Replace(":", "");
                }

                string hh = time.Substring(0, 2);
                string mm = time.Substring(2, 2);
                string finalTime = string.Format("{0}:{1}", hh, mm);
                return finalTime;
            }

            return null;
        }

        public string CreateXMLForDHLPickupforCapability(DHLShipmentRequestDto dhlRequestObject)
        {
            string xmlPath = string.Empty;
            try
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = new UTF8Encoding(true);
                xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
                xmlWriterSettings.Indent = true;
                var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.DHL);

                dhlRequestObject.Request.ServiceHeader.SiteID = logisticIntegration.UserName;
                dhlRequestObject.Request.ServiceHeader.Password = logisticIntegration.Password;

                if (AppSettings.LabelSave == "")
                {
                    xmlPath = AppSettings.WebApiPath + "/UploadFiles/PDFGenerator/HTMLFile";
                }
                else
                {
                    if (AppSettings.ShipmentCreatedFrom == "BATCH")
                    {
                        xmlPath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/HTMLFile";
                    }
                    else
                    {
                        xmlPath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");
                    }
                }

                if (!Directory.Exists(xmlPath))
                {
                    Directory.CreateDirectory(xmlPath);
                }

                xmlPath = xmlPath + "/DHLPickupCapability.xml";

                if (File.Exists(xmlPath))
                {
                    File.Delete(xmlPath);
                }
                using (var xmlWriter = XmlWriter.Create(xmlPath))
                {
                    xmlWriter.WriteStartDocument();
                    if (logisticIntegration != null)
                    {
                        xmlWriter.WriteStartElement("p", "DCTRequest", "http://www.dhl.com");
                        xmlWriter.WriteAttributeString("xmlns", "p1", null, "http://www.dhl.com/datatypes");
                        xmlWriter.WriteAttributeString("xmlns", "p2", null, "http://www.dhl.com/DCTRequestdatatypes");
                        xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                        xmlWriter.WriteAttributeString("xsi", "schemaLocation", null, "http://www.dhl.com DCT-req.xsd");

                        //start
                        xmlWriter.WriteStartElement("GetCapability");
                        xmlWriter.WriteStartElement("Request");

                        xmlWriter.WriteStartElement("ServiceHeader");

                        xmlWriter.WriteStartElement("MessageTime");
                        xmlWriter.WriteString(dhlRequestObject.Request.ServiceHeader.MessageTime);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("MessageReference");
                        xmlWriter.WriteString("12345678912345678912345678901");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("SiteID");
                        xmlWriter.WriteString(dhlRequestObject.Request.ServiceHeader.SiteID);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Password");
                        xmlWriter.WriteString(dhlRequestObject.Request.ServiceHeader.Password);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();
                        // end ServiceHeader

                        xmlWriter.WriteEndElement();
                        // Request End

                        xmlWriter.WriteStartElement("From");

                        xmlWriter.WriteStartElement("CountryCode");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.CountryCode);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Postalcode");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.PostalCode);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("City");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.City);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();
                        //end from

                        xmlWriter.WriteStartElement("BkgDetails");

                        xmlWriter.WriteStartElement("PaymentCountryCode");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.CountryCode);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Date");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.Date.ToString("yyyy-MM-dd"));
                        xmlWriter.WriteEndElement();

                        var time = GetFormmatedTime(dhlRequestObject.CollectionTime);
                        TimeSpan ts = TimeSpan.Parse(time);
                        string minutes = ts.Minutes <= 9 ? "0" + ts.Minutes : ts.Minutes.ToString();
                        string readyTime = "PT" + ts.Hours + "H" + minutes + "M";

                        xmlWriter.WriteStartElement("ReadyTime");
                        xmlWriter.WriteString(readyTime);
                        xmlWriter.WriteEndElement();

                        if (dhlRequestObject.Shipper.CountryCode == "GB")
                        {
                            xmlWriter.WriteStartElement("ReadyTimeGMTOffset");
                            xmlWriter.WriteString("+1:00");
                            xmlWriter.WriteEndElement();
                        }
                        else
                        {
                            TimeZoneModal timezone = TimeZoneDetail(dhlRequestObject.Shipper.CountryCode);
                            var kk = timezone.OffsetShort.Replace("GMT", string.Empty);
                            xmlWriter.WriteStartElement("ReadyTimeGMTOffset");
                            xmlWriter.WriteString(kk);
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteStartElement("DimensionUnit");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.DimensionUnit == "C" ? "CM" : "IN");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("WeightUnit");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.WeightUnit == "K" ? "KG" : "LB");
                        xmlWriter.WriteEndElement();

                        //start
                        xmlWriter.WriteStartElement("Pieces");
                        int i = 1;
                        foreach (var Piece in dhlRequestObject.ShipmentDetails.Pieces)
                        {
                            xmlWriter.WriteStartElement("Piece");

                            xmlWriter.WriteStartElement("PieceID");
                            xmlWriter.WriteString(i.ToString());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("Height");
                            xmlWriter.WriteString(Piece.Height);
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("Depth");
                            xmlWriter.WriteString(Piece.Depth);
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("Width");
                            xmlWriter.WriteString(Piece.Width);
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("Weight");
                            xmlWriter.WriteString(Piece.Weight.ToString("0.##"));
                            xmlWriter.WriteEndElement();
                            i++;
                            xmlWriter.WriteEndElement();
                        }

                        //end Pieces
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("IsDutiable");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.IsDutiable);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("NetworkTypeCode");
                        xmlWriter.WriteString(dhlRequestObject.DoorTo);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();
                        //end BkgDetails

                        //start To
                        xmlWriter.WriteStartElement("To");

                        xmlWriter.WriteStartElement("CountryCode");
                        xmlWriter.WriteString(dhlRequestObject.Consignee.CountryCode);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Postalcode");
                        xmlWriter.WriteString(dhlRequestObject.Consignee.PostalCode);
                        xmlWriter.WriteEndElement();


                        xmlWriter.WriteEndElement();
                        //end To

                        //start Dutiable
                        xmlWriter.WriteStartElement("Dutiable");

                        xmlWriter.WriteStartElement("DeclaredCurrency");
                        xmlWriter.WriteString(dhlRequestObject.Dutiable.CurrencyCode);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("DeclaredValue");
                        xmlWriter.WriteString(dhlRequestObject.Dutiable.DeclaredValue);
                        xmlWriter.WriteEndElement();


                        xmlWriter.WriteEndElement();
                        //end Dutiable
                        xmlWriter.WriteEndElement();
                        // GetCapability End

                        xmlWriter.WriteEndElement();

                        //End DCTRequest
                    }
                    xmlWriter.WriteEndDocument();
                }
                return xmlPath;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private TimeZoneModal TimeZoneDetail(string CountryCode2)
        {
            using (var dbContext = new FrayteEntities())
            {
                var TimeZone = (from uu in dbContext.Countries
                                join tz in dbContext.Timezones on uu.TimeZoneId equals tz.TimezoneId
                                where uu.CountryCode2 == CountryCode2
                                select tz).FirstOrDefault();
                TimeZoneModal TZ = new TimeZoneModal();
                if (TimeZone != null)
                {
                    TZ.Name = TimeZone.Name;
                    TZ.Offset = TimeZone.Offset;
                    TZ.OffsetShort = TimeZone.OffsetShort;
                    TZ.TimezoneId = TimeZone.TimezoneId;
                }
                return TZ;
            }
        }
    }

    public class DHLUKRepositry
    {
        public string CreateXMLForDHLUK(DHLShipmentRequestDto dhlRequestObject)
        {
            string xmlPath = string.Empty;

            try
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();

                xmlWriterSettings.Encoding = new UTF8Encoding(true);

                xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;

                xmlWriterSettings.Indent = true;
                var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.DHL);

                dhlRequestObject.Request.ServiceHeader.SiteID = logisticIntegration.UserName;

                dhlRequestObject.Request.ServiceHeader.Password = logisticIntegration.Password;

                if (AppSettings.LabelSave == "")
                {
                    xmlPath = AppSettings.WebApiPath + "/UploadFiles/PDFGenerator/HTMLFile";
                }
                else
                {
                    if (AppSettings.ShipmentCreatedFrom == "BATCH")
                    {
                        xmlPath = AppSettings.WebApiPath + "/UploadFiles/PDFGenerator/HTMLFile";
                    }
                    else
                    {
                        xmlPath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");
                    }
                }

                if (!Directory.Exists(xmlPath))
                {
                    Directory.CreateDirectory(xmlPath);
                }

                xmlPath = xmlPath + "/tempDHLUKShipment.xml";
                if (File.Exists(xmlPath))
                {
                    File.Delete(xmlPath);
                }
                using (var xmlWriter = XmlWriter.Create(xmlPath))
                {
                    xmlWriter.WriteStartDocument();
                    if (logisticIntegration != null)
                    {
                        xmlWriter.WriteStartElement("req", "ShipmentRequest", "http://www.dhl.com");
                        xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                        xmlWriter.WriteAttributeString(null, "schemaVersion", null, "6.2");
                        xmlWriter.WriteAttributeString("xsi", "schemaLocation", null, "http://www.dhl.com ship-val-global-req.xsd");

                        //RequestNode
                        CreateRequestNodeUK(xmlWriter, dhlRequestObject);

                        if (dhlRequestObject.RegionCode.Trim() == "AM")
                        {
                            xmlWriter.WriteStartElement("RegionCode");
                            xmlWriter.WriteString(dhlRequestObject.RegionCode.Trim());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("NewShipper");
                            xmlWriter.WriteString("N");
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("LanguageCode");
                            xmlWriter.WriteString(dhlRequestObject.LanguageCode);
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("PiecesEnabled");
                            xmlWriter.WriteString(dhlRequestObject.PiecesEnabled);
                            xmlWriter.WriteEndElement();
                        }
                        if (dhlRequestObject.RegionCode.Trim() == "AP")
                        {
                            xmlWriter.WriteStartElement("RegionCode");
                            xmlWriter.WriteString(dhlRequestObject.RegionCode.Trim());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("LanguageCode");
                            xmlWriter.WriteString(dhlRequestObject.LanguageCode);
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("PiecesEnabled");
                            xmlWriter.WriteString(dhlRequestObject.PiecesEnabled);
                            xmlWriter.WriteEndElement();
                        }
                        if (dhlRequestObject.RegionCode.Trim() == "EU")
                        {
                            xmlWriter.WriteStartElement("RegionCode");
                            xmlWriter.WriteString(dhlRequestObject.RegionCode.Trim());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("NewShipper");
                            xmlWriter.WriteString("N");
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("LanguageCode");
                            xmlWriter.WriteString(dhlRequestObject.LanguageCode);
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("PiecesEnabled");
                            xmlWriter.WriteString(dhlRequestObject.PiecesEnabled);
                            xmlWriter.WriteEndElement();
                        }

                        //BillingNode
                        CreateBillingUK(xmlWriter, dhlRequestObject);
                        //Create Consignee
                        ConsigneeUK(xmlWriter, dhlRequestObject);

                        //Commodity
                        xmlWriter.WriteStartElement("Commodity");

                        xmlWriter.WriteStartElement("CommodityCode");
                        xmlWriter.WriteString("cc");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("CommodityName");
                        xmlWriter.WriteString("cn");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Dutiable");

                        xmlWriter.WriteStartElement("DeclaredValue");

                        int DeclaredValue = 0;
                        int.TryParse(dhlRequestObject.Dutiable.DeclaredValue, out DeclaredValue);
                        string dhlValue = DeclaredValue == 0 ? "0.01" : DeclaredValue.ToString();
                        xmlWriter.WriteString(dhlValue);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("DeclaredCurrency");
                        xmlWriter.WriteString(dhlRequestObject.Dutiable.CurrencyCode);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("ScheduleB");
                        xmlWriter.WriteString("");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("ExportLicense");
                        xmlWriter.WriteString("");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("ShipperEIN");
                        xmlWriter.WriteString("");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("ShipperIDType");
                        xmlWriter.WriteString("S");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("ImportLicense");
                        xmlWriter.WriteString("");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("ConsigneeEIN");
                        xmlWriter.WriteString("");
                        xmlWriter.WriteEndElement();

                        if (dhlRequestObject.Billing.DutyPaymentType == "S")
                        {
                            xmlWriter.WriteStartElement("TermsOfTrade");
                            xmlWriter.WriteString("DDP");
                            xmlWriter.WriteEndElement();
                        }
                        if (dhlRequestObject.Billing.DutyPaymentType == "R")
                        {
                            xmlWriter.WriteStartElement("TermsOfTrade");
                            xmlWriter.WriteString("DDU");
                            xmlWriter.WriteEndElement();
                        }
                        if (dhlRequestObject.Billing.DutyPaymentType == "T")
                        {
                            xmlWriter.WriteStartElement("TermsOfTrade");
                            xmlWriter.WriteString("DDU");
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteEndElement();

                        //Referance Details
                        xmlWriter.WriteStartElement("Reference");

                        xmlWriter.WriteStartElement("ReferenceID");
                        xmlWriter.WriteString(dhlRequestObject.Reference.ReferenceID);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("ReferenceType");
                        xmlWriter.WriteString(dhlRequestObject.Reference.ReferenceType);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        ShipmentDetailsUK(xmlWriter, dhlRequestObject);

                        ShipperDetailsUK(xmlWriter, dhlRequestObject);
                        //Enable NDS:-
                        if (dhlRequestObject.IsNDSEnable)
                        {
                            xmlWriter.WriteStartElement("SpecialService");
                            xmlWriter.WriteStartElement("SpecialServiceType");
                            xmlWriter.WriteString("NN");
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteEndElement();
                        }

                        //Register Place
                        PlaceDetailsUK(xmlWriter);

                        xmlWriter.WriteStartElement("EProcShip");
                        xmlWriter.WriteString("N");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("LabelImageFormat");
                        xmlWriter.WriteString(dhlRequestObject.LabelImageFormat);
                        xmlWriter.WriteEndElement();


                        xmlWriter.WriteStartElement("RequestArchiveDoc");
                        xmlWriter.WriteString("N");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Label");

                        xmlWriter.WriteStartElement("HideAccount");
                        xmlWriter.WriteString("N");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("LabelTemplate");
                        xmlWriter.WriteString("8X4_CI_thermal");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                    }
                    xmlWriter.WriteEndDocument();
                }
                return xmlPath;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static void CreateRequestNodeUK(XmlWriter xmlWriter, DHLShipmentRequestDto dhlRequestDto)
        {
            xmlWriter.WriteStartElement("Request");
            xmlWriter.WriteStartElement("ServiceHeader");

            xmlWriter.WriteStartElement("MessageTime");
            xmlWriter.WriteString(dhlRequestDto.Request.ServiceHeader.MessageTime);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("MessageReference");
            xmlWriter.WriteString("12345678912345678912345678901");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("SiteID");
            xmlWriter.WriteString(dhlRequestDto.Request.ServiceHeader.SiteID);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Password");
            xmlWriter.WriteString(dhlRequestDto.Request.ServiceHeader.Password);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("MetaData");

            xmlWriter.WriteStartElement("SoftwareName");
            xmlWriter.WriteString("XMLPI");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("SoftwareVersion");
            xmlWriter.WriteString("6.2");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }

        private static void CreateBillingUK(XmlWriter xmlWriter, DHLShipmentRequestDto dhlRequestDto)
        {
            xmlWriter.WriteStartElement("Billing");

            if (dhlRequestDto.Billing.ShippingPaymentType == "S")
            {

                xmlWriter.WriteStartElement("ShipperAccountNumber");
                xmlWriter.WriteString(dhlRequestDto.Billing.ShipperAccountNumber);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("ShippingPaymentType");
                xmlWriter.WriteString(dhlRequestDto.Billing.ShippingPaymentType);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("DutyPaymentType");
                xmlWriter.WriteString(dhlRequestDto.Billing.DutyPaymentType);
                xmlWriter.WriteEndElement();
            }
            if (dhlRequestDto.Billing.ShippingPaymentType == "R")
            {
                xmlWriter.WriteStartElement("ShipperAccountNumber");
                xmlWriter.WriteString(dhlRequestDto.Billing.ShipperAccountNumber);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("ShippingPaymentType");
                xmlWriter.WriteString(dhlRequestDto.Billing.ShippingPaymentType);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("DutyPaymentType");
                xmlWriter.WriteString(dhlRequestDto.Billing.DutyPaymentType);
                xmlWriter.WriteEndElement();
            }

            if (dhlRequestDto.Billing.ShippingPaymentType == "T")
            {
                xmlWriter.WriteStartElement("ShipperAccountNumber");
                xmlWriter.WriteString(dhlRequestDto.Billing.ShipperAccountNumber);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("ShippingPaymentType");
                xmlWriter.WriteString(dhlRequestDto.Billing.ShippingPaymentType);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("BillingAccountNumber");
                xmlWriter.WriteString(dhlRequestDto.Billing.BillingAccountNumber);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("DutyPaymentType");
                xmlWriter.WriteString(dhlRequestDto.Billing.DutyPaymentType);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("DutyAccountNumber");
                xmlWriter.WriteString(dhlRequestDto.Billing.DutyAccountNumber);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }

        private static void ConsigneeUK(XmlWriter xmlWriter, DHLShipmentRequestDto dhlRequestDto)
        {
            xmlWriter.WriteStartElement("Consignee");

            xmlWriter.WriteStartElement("CompanyName");
            xmlWriter.WriteString(dhlRequestDto.Consignee.CompanyName);
            xmlWriter.WriteEndElement();

            foreach (var address in dhlRequestDto.Consignee.AddressLine)
            {
                xmlWriter.WriteStartElement("AddressLine");
                xmlWriter.WriteString(address.AddressLine);
                xmlWriter.WriteEndElement();
            }
            if (dhlRequestDto.RegionCode.Trim() == "EU")
            {
                xmlWriter.WriteStartElement("City");
                xmlWriter.WriteString(dhlRequestDto.Consignee.City);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Division");
                xmlWriter.WriteString(dhlRequestDto.Consignee.City);
                xmlWriter.WriteEndElement();

                //xmlWriter.WriteStartElement("DivisionCode");
                //xmlWriter.WriteString(dhlRequestDto.Consignee.DivisionCode);
                //xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("PostalCode");
                xmlWriter.WriteString(dhlRequestDto.Consignee.PostalCode);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("CountryCode");
                xmlWriter.WriteString(dhlRequestDto.Consignee.CountryCode);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("CountryName");
                xmlWriter.WriteString(dhlRequestDto.Consignee.CountryName);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("FederalTaxId");
                xmlWriter.WriteString("consigneefedtax");
                xmlWriter.WriteEndElement();
            }
            if (dhlRequestDto.RegionCode.Trim() == "AP")
            {
                xmlWriter.WriteStartElement("City");
                xmlWriter.WriteString(dhlRequestDto.Consignee.City);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("PostalCode");
                xmlWriter.WriteString(dhlRequestDto.Consignee.PostalCode);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("CountryCode");
                xmlWriter.WriteString(dhlRequestDto.Consignee.CountryCode);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("CountryName");
                xmlWriter.WriteString(dhlRequestDto.Consignee.CountryName);
                xmlWriter.WriteEndElement();
            }
            if (dhlRequestDto.RegionCode.Trim() == "AM")
            {
                xmlWriter.WriteStartElement("City");
                xmlWriter.WriteString(dhlRequestDto.Consignee.City);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("PostalCode");
                xmlWriter.WriteString(dhlRequestDto.Consignee.PostalCode);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("CountryCode");
                xmlWriter.WriteString(dhlRequestDto.Consignee.CountryCode);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("CountryName");
                xmlWriter.WriteString(dhlRequestDto.Consignee.CountryName);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteStartElement("Contact");

            xmlWriter.WriteStartElement("PersonName");
            xmlWriter.WriteString(dhlRequestDto.Consignee.Contact.PersonName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("PhoneNumber");
            xmlWriter.WriteString(dhlRequestDto.Consignee.Contact.PhoneNumber);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }

        private static void ShipmentDetailsUK(XmlWriter xmlWriter, DHLShipmentRequestDto dhlRequestDto)
        {
            xmlWriter.WriteStartElement("ShipmentDetails");

            xmlWriter.WriteStartElement("NumberOfPieces");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.NumberOfPieces);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Pieces");
            for (int i = 0; i < dhlRequestDto.ShipmentDetails.Pieces.Count(); i++)
            {
                xmlWriter.WriteStartElement("Piece");

                xmlWriter.WriteStartElement("PieceID");
                xmlWriter.WriteString((i + 1).ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Weight");
                xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.Pieces[i].Weight.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Width");
                xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.Pieces[i].Width.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Height");
                xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.Pieces[i].Height.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Depth");
                xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.Pieces[i].Depth.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
            }
            foreach (var Piece in dhlRequestDto.ShipmentDetails.Pieces)
            {
                //xmlWriter.WriteStartElement("Piece");

                //xmlWriter.WriteStartElement("PieceID");
                //xmlWriter.WriteString(Piece.Weight.ToString());
                //xmlWriter.WriteEndElement();

                //xmlWriter.WriteStartElement("Weight");
                //xmlWriter.WriteString(Piece.Weight.ToString());
                //xmlWriter.WriteEndElement();

                //xmlWriter.WriteStartElement("Width");
                //xmlWriter.WriteString(Piece.Width.ToString());
                //xmlWriter.WriteEndElement();

                //xmlWriter.WriteStartElement("Height");
                //xmlWriter.WriteString(Piece.Height.ToString());
                //xmlWriter.WriteEndElement();

                //xmlWriter.WriteStartElement("Depth");
                //xmlWriter.WriteString(Piece.Depth.ToString());
                //xmlWriter.WriteEndElement();

                //xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Weight");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.Weight.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("WeightUnit");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.WeightUnit.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("GlobalProductCode");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.GlobalProductCode.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("LocalProductCode");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.LocalProductCode.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Date");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.Date.ToString("yyyy-MM-dd"));
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Contents");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.Contents);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("DoorTo");
            xmlWriter.WriteString("DD");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("DimensionUnit");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.DimensionUnit.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("PackageType");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.PackageType);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("IsDutiable");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.IsDutiable);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CurrencyCode");
            xmlWriter.WriteString(dhlRequestDto.ShipmentDetails.CurrencyCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }

        private static void ShipperDetailsUK(XmlWriter xmlWriter, DHLShipmentRequestDto dhlRequestDto)
        {
            xmlWriter.WriteStartElement("Shipper");

            xmlWriter.WriteStartElement("ShipperID");
            xmlWriter.WriteString(dhlRequestDto.Shipper.ShipperID);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CompanyName");
            xmlWriter.WriteString(dhlRequestDto.Shipper.CompanyName);
            xmlWriter.WriteEndElement();

            foreach (var address in dhlRequestDto.Shipper.AddressLine)
            {
                xmlWriter.WriteStartElement("AddressLine");
                xmlWriter.WriteString(address.AddressLine);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteStartElement("City");
            xmlWriter.WriteString(dhlRequestDto.Shipper.City);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Division");
            xmlWriter.WriteString(dhlRequestDto.Shipper.DivisionCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("DivisionCode");
            xmlWriter.WriteString(dhlRequestDto.Shipper.DivisionCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("PostalCode");
            xmlWriter.WriteString(dhlRequestDto.Shipper.PostalCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CountryCode");
            xmlWriter.WriteString(dhlRequestDto.Shipper.CountryCode);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CountryName");
            xmlWriter.WriteString(dhlRequestDto.Shipper.CountryName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Contact");

            xmlWriter.WriteStartElement("PersonName");
            xmlWriter.WriteString(dhlRequestDto.Shipper.Contact.PersonName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("PhoneNumber");
            xmlWriter.WriteString(dhlRequestDto.Shipper.Contact.PhoneNumber);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }

        private static void PlaceDetailsUK(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Place");

            xmlWriter.WriteStartElement("ResidenceOrBusiness");
            xmlWriter.WriteString("B");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CompanyName");
            xmlWriter.WriteString("FRAYTE Logistics Ltd");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("AddressLine");
            xmlWriter.WriteString("Unit 11, SA1 Industrial Park");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("AddressLine");
            xmlWriter.WriteString("Langdon Road");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("City");
            xmlWriter.WriteString("Swansea");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("CountryCode");
            xmlWriter.WriteString("GB");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("DivisionCode");
            xmlWriter.WriteString("ca");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Division");
            xmlWriter.WriteString("ca");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("PostalCode");
            xmlWriter.WriteString("SA1 8QY");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }

        public IntegrtaionResult MapDHLIntegrationResponseUK(DHLResponseDto DHLResponse)
        {
            IntegrtaionResult integrtaionResult = new IntegrtaionResult();

            if (DHLResponse.Status)
            {
                integrtaionResult.Status = true;
                integrtaionResult.ShipmentImage = DHLResponse.ShipmentImange;
                integrtaionResult.PickupRef = DHLResponse.PickupRef;
                integrtaionResult.CourierName = FrayteCourierCompany.DHL;
                integrtaionResult.TrackingNumber = DHLResponse.DHLOrderId;
                integrtaionResult.PieceTrackingDetails = new List<CourierPieceDetail>();
                foreach (var data in DHLResponse.Pieces)
                {
                    CourierPieceDetail obj = new CourierPieceDetail();
                    obj.DirectShipmentDetailId = 0;
                    obj.PieceTrackingNumber = data.LicensePlate;
                    obj.ImageByte = Base64DecodeUK(DHLResponse.ImageString);
                    integrtaionResult.PieceTrackingDetails.Add(obj);
                }
            }
            else
            {
                integrtaionResult.Error = new FratyteError();
                integrtaionResult.Error.Address = new List<string>();
                {
                    var error = DHLResponse.Error.ErrorCode + "/" + DHLResponse.Error.ErrorDescription;
                    integrtaionResult.Error.Address.Add(error);
                }
                integrtaionResult.ErrorCode = new List<FrayteApiError>();
                {
                    List<FrayteApiError> _api = new FrayteApiErrorCodeRepository().SaveFinilizeApiError(integrtaionResult.Error, FrayteLogisticServiceType.DHL, null);
                    integrtaionResult.ErrorCode = _api;
                }
                integrtaionResult.Status = false;
            }
            return integrtaionResult;
        }

        public static string Base64DecodeUK(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public string DownloadDHLImageUK(CourierPieceDetail pieceDetails, int totalPiece, int count, int DirectShipmentid)
        {
            string Image = string.Empty;
            bool isAll = false;
            //byte[] byteInfo = Convert.FromBase64String(pieceDetails.ImageByte);
            try
            {
                string labelName = string.Empty;
                labelName = FrayteShortName.DHL;
                if (count == 0)
                {
                    string PieceTrackingNumber = null;
                    if (pieceDetails.PieceTrackingNumber.Contains("AirwayBillNumber_"))
                    {
                        PieceTrackingNumber = pieceDetails.PieceTrackingNumber.Replace("AirwayBillNumber_", "");
                    }
                    Image = labelName + "_" + PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".jpg";
                    count = totalPiece + 1;
                    isAll = true;
                }
                else
                {
                    Image = labelName + "_" + pieceDetails.PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".jpg";
                    isAll = false;
                }

                if (AppSettings.LabelSave == "")
                {
                    if (Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/"))
                    {
                        var response = ZPL2WebApiResponeUK(pieceDetails.ImageByte, count, isAll);
                        var responseStream = response.GetResponseStream();
                        var fileStream = File.Create(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/" + Image); // change file name for PNG images
                        responseStream.CopyTo(fileStream);
                        responseStream.Close();
                        fileStream.Close();
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/");
                        var response = ZPL2WebApiResponeUK(pieceDetails.ImageByte, count, isAll);
                        var responseStream = response.GetResponseStream();
                        var fileStream = File.Create(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/" + Image); // change file name for PNG images
                        responseStream.CopyTo(fileStream);
                        responseStream.Close();
                        fileStream.Close();
                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(AppSettings.LabelFolder + "/" + DirectShipmentid + "/"))
                    {
                        var response = ZPL2WebApiResponeUK(pieceDetails.ImageByte, count, isAll);
                        var responseStream = response.GetResponseStream();
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            var fileStream = File.Create(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image); // change file name for PNG images
                            responseStream.CopyTo(fileStream);
                            responseStream.Close();
                            fileStream.Close();
                        }
                        else
                        {
                            var fileStream = File.Create(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid) + "/" + Image); // change file name for PNG images
                            responseStream.CopyTo(fileStream);
                            responseStream.Close();
                            fileStream.Close();
                        }
                    }
                    else
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "/" + DirectShipmentid);
                            var response = ZPL2WebApiResponeUK(pieceDetails.ImageByte, count, isAll);
                            var responseStream = response.GetResponseStream();
                            var fileStream = File.Create(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image); // change file name for PNG images
                            responseStream.CopyTo(fileStream);
                            responseStream.Close();
                            fileStream.Close();
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid));
                            var response = ZPL2WebApiResponeUK(pieceDetails.ImageByte, count, isAll);
                            var responseStream = response.GetResponseStream();
                            var fileStream = File.Create(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid) + "/" + Image); // change file name for PNG images
                            responseStream.CopyTo(fileStream);
                            responseStream.Close();
                            fileStream.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ex.ToString();
            }

            //Step16.1: Create direcory for save package label
            return Image;
        }

        private WebResponse ZPL2WebApiResponeUK(string ImageByte, int count, bool isAll)
        {
            count = count - 1;
            byte[] zpl = Encoding.UTF8.GetBytes(ImageByte);

            // adjust print density (8dpmm), label width (4 inches), label height (6 inches), and label index (0) as necessary
            HttpWebRequest request;
            if (isAll)
            {
                request = (HttpWebRequest)WebRequest.Create("http://api.labelary.com/v1/printers/12dpmm/labels/4x6" + "/" + count + "/");
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create("http://api.labelary.com/v1/printers/8dpmm/labels/4x6" + "/" + count + "/");
            }


            request.Method = "POST";
            request.Accept = "image/png"; // omit this line to get PNG images back
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = zpl.Length;

            var requestStream = request.GetRequestStream();
            requestStream.Write(zpl, 0, zpl.Length);
            requestStream.Close();
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseStream = response.GetResponseStream();
                return response;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public bool SaveTrackingDetailUK(DirectBookingShipmentDraftDetail directBookingDetail, IntegrtaionResult result, int DirectShipmentid)
        {
            var count = 1;
            foreach (var Obj in result.PieceTrackingDetails)
            {
                //_shiId = new DirectShipmentRepository().GetDirectShipmentDetailID(DirectShipmentid);
                if (!Obj.PieceTrackingNumber.Contains("AirwayBillNumber_"))
                {
                    Frayte.Services.Models.Package package = new Frayte.Services.Models.Package();
                    package.DirectShipmentDetailId = Obj.DirectShipmentDetailId;

                    package.LabelName = Obj.PieceTrackingNumber;
                    new DirectShipmentRepository().SavePackageDetail(package, "", Obj.DirectShipmentDetailId, directBookingDetail.CustomerRateCard.CourierName, count);
                    count++;
                }
            }
            return true;
        }

        public void MappingCourierPieceDetailUK(IntegrtaionResult integrtaionResult, DirectBookingShipmentDraftDetail directBookingDetail, int DirectShipmentid)
        {
            if (DirectShipmentid > 0)
            {
                int k = 0, i = 0;
                List<int> _shiId = new List<int>();
                foreach (var Obj in directBookingDetail.Packages)
                {

                    _shiId = new DirectShipmentRepository().GetDirectShipmentDetailID(DirectShipmentid);
                    for (int j = 1; j <= Obj.CartoonValue; j++)
                    {

                        integrtaionResult.PieceTrackingDetails[k].DirectShipmentDetailId = _shiId[i];
                        k++;
                    }
                    i++;
                }
            }
        }

        private static string GetFormmatedTimeUK(string time)
        {
            if (!string.IsNullOrEmpty(time))
            {
                if (time.Length == 3)
                {
                    time = "0" + time;
                }
                if (time.Contains(":"))
                {
                    time = time.Replace(":", "");
                }
                string hh = time.Substring(0, 2);
                string mm = time.Substring(2, 2);
                string finalTime = string.Format("{0}:{1}", hh, mm);
                return finalTime;
            }

            return null;
        }

        public string CreateXMLForDHLPickupforAP(DHLShipmentRequestDto dhlRequestObject)
        {
            string xmlPath = string.Empty;
            try
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = new UTF8Encoding(true);
                xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
                xmlWriterSettings.Indent = true;
                var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.DHL);

                dhlRequestObject.Request.ServiceHeader.SiteID = logisticIntegration.UserName;
                dhlRequestObject.Request.ServiceHeader.Password = logisticIntegration.Password;

                //xmlPath = It will be the path where xml will saved  

                if (AppSettings.LabelSave == "")
                {
                    xmlPath = AppSettings.WebApiPath + "/UploadFiles/PDFGenerator/HTMLFile";
                }
                else
                {
                    if (AppSettings.ShipmentCreatedFrom == "BATCH")
                    {
                        xmlPath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/HTMLFile";
                    }
                    else
                    {
                        xmlPath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");
                    }
                }

                if (!Directory.Exists(xmlPath))
                {
                    Directory.CreateDirectory(xmlPath);
                }

                xmlPath = xmlPath + "/tempDHLPickupAP.xml";

                if (File.Exists(xmlPath))
                {
                    File.Delete(xmlPath);
                }
                using (var xmlWriter = XmlWriter.Create(xmlPath))
                {
                    xmlWriter.WriteStartDocument();
                    if (logisticIntegration != null)
                    {

                        xmlWriter.WriteStartElement("req", "BookPURequest", "http://www.dhl.com");
                        xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                        xmlWriter.WriteAttributeString("xsi", "schemaLocation", null, "http://www.dhl.com book-pickup-global-req.xsd");
                        xmlWriter.WriteAttributeString(null, "schemaVersion", null, "3.0");

                        //RequestNode                      
                        xmlWriter.WriteStartElement("Request");
                        xmlWriter.WriteStartElement("ServiceHeader");

                        xmlWriter.WriteStartElement("MessageTime");
                        xmlWriter.WriteString(dhlRequestObject.Request.ServiceHeader.MessageTime);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("MessageReference");
                        xmlWriter.WriteString("12345678912345678912345678901");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("SiteID");
                        xmlWriter.WriteString(dhlRequestObject.Request.ServiceHeader.SiteID);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Password");
                        xmlWriter.WriteString(dhlRequestObject.Request.ServiceHeader.Password);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("MetaData");

                        xmlWriter.WriteStartElement("SoftwareName");
                        xmlWriter.WriteString("XMLPI");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("SoftwareVersion");
                        xmlWriter.WriteString("1.0");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        //End RequestNode

                        xmlWriter.WriteStartElement("RegionCode");
                        xmlWriter.WriteString(!string.IsNullOrWhiteSpace(dhlRequestObject.RegionCode) ? dhlRequestObject.RegionCode.Trim() : "");
                        xmlWriter.WriteEndElement();


                        //Requestor
                        xmlWriter.WriteStartElement("Requestor");

                        xmlWriter.WriteStartElement("AccountType");
                        xmlWriter.WriteString("D");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("AccountNumber");
                        xmlWriter.WriteString(dhlRequestObject.Billing.ShipperAccountNumber);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("RequestorContact");

                        xmlWriter.WriteStartElement("PersonName");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.Contact.PersonName);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Phone");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.Contact.PhoneNumber);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("PhoneExtension");
                        xmlWriter.WriteString("123");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("CompanyName");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.CompanyName);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Address1");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.AddressLine[0] != null ? dhlRequestObject.Shipper.AddressLine[0].AddressLine : "");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Address2");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.AddressLine[1] != null ? dhlRequestObject.Shipper.AddressLine[1].AddressLine : "");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("City");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.City);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("CountryCode");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.CountryCode);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();
                        //End  Requestor


                        //place                      
                        xmlWriter.WriteStartElement("Place");

                        xmlWriter.WriteStartElement("LocationType");
                        xmlWriter.WriteString("B");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("CompanyName");
                        xmlWriter.WriteString("FRAYTE Logistics Ltd");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Address1");
                        xmlWriter.WriteString("Unit 11, SA1 Industrial Park");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Address2");
                        xmlWriter.WriteString("Langdon Road,");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("PackageLocation");
                        xmlWriter.WriteString("FRAYTE Logistics Ltd");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("City");
                        xmlWriter.WriteString("Swansea");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("CountryCode");
                        xmlWriter.WriteString("GB");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("PostalCode");
                        xmlWriter.WriteString("SA1 8QY");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        //End Place

                        //Pickup
                        DateTime mindatetime = dhlRequestObject.ShipmentDetails.Date;

                        if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Sunday)
                        {
                            mindatetime = dhlRequestObject.ShipmentDetails.Date.AddDays(1);
                        }
                        else if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Saturday)
                        {

                            mindatetime = dhlRequestObject.ShipmentDetails.Date.AddDays(2);
                        }
                        xmlWriter.WriteStartElement("Pickup");

                        xmlWriter.WriteStartElement("PickupDate");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.Date.ToString("yyyy-MM-dd"));
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("PickupTypeCode");
                        xmlWriter.WriteString("S");
                        xmlWriter.WriteEndElement();

                        var time = GetFormmatedTimeUK(dhlRequestObject.CollectionTime);
                        TimeSpan ts = TimeSpan.Parse(time);


                        xmlWriter.WriteStartElement("ReadyByTime");
                        xmlWriter.WriteString(ts.ToString("hh':'mm"));
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("CloseTime");
                        xmlWriter.WriteString("17:00");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Pieces");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.Pieces.Count().ToString());
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("weight");

                        var weight = dhlRequestObject.ShipmentDetails.Pieces.Sum(S => S.Weight);

                        xmlWriter.WriteStartElement("Weight");
                        xmlWriter.WriteString(weight.ToString("0.##"));
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("WeightUnit");
                        xmlWriter.WriteString("K");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();


                        //End Pickup

                        // PickupContact
                        xmlWriter.WriteStartElement("PickupContact");

                        xmlWriter.WriteStartElement("PersonName");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.Contact.PersonName.ToString());
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Phone");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.Contact.PhoneNumber.ToString());
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("PhoneExtension");
                        xmlWriter.WriteString("123");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();
                        //End  PickupContact

                        //ShipmentDetails

                        xmlWriter.WriteStartElement("ShipmentDetails");

                        xmlWriter.WriteStartElement("AccountType");
                        xmlWriter.WriteString("D");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("AccountNumber");
                        xmlWriter.WriteString(dhlRequestObject.Billing.ShipperAccountNumber);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("AWBNumber");
                        xmlWriter.WriteString(dhlRequestObject.AWBNumber);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("NumberOfPieces");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.NumberOfPieces);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Weight");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.Weight);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("WeightUnit");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.WeightUnit);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("GlobalProductCode");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.GlobalProductCode);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("DoorTo");
                        xmlWriter.WriteString("DD");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("DimensionUnit");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.DimensionUnit);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Pieces");

                        foreach (var item in dhlRequestObject.ShipmentDetails.Pieces)
                        {
                            xmlWriter.WriteStartElement("Piece");

                            xmlWriter.WriteStartElement("Weight");
                            xmlWriter.WriteString(item.Weight.ToString());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("Width");
                            xmlWriter.WriteString(item.Width.ToString());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("Height");
                            xmlWriter.WriteString(item.Height.ToString());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("Depth");
                            xmlWriter.WriteString(item.Depth.ToString());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteEndElement();
                        }
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        //End ShipmentDetails
                    }
                    xmlWriter.WriteEndDocument();
                }
                return xmlPath;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string CreateXMLForDHLPickupforAM(DHLShipmentRequestDto dhlRequestObject)
        {
            string xmlPath = string.Empty;
            try
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = new UTF8Encoding(true);
                xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
                xmlWriterSettings.Indent = true;
                var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.DHL);

                dhlRequestObject.Request.ServiceHeader.SiteID = logisticIntegration.UserName;
                dhlRequestObject.Request.ServiceHeader.Password = logisticIntegration.Password;

                //xmlPath = It will be the path where xml will saved  

                if (AppSettings.LabelSave == "")
                {
                    xmlPath = AppSettings.WebApiPath + "/UploadFiles/PDFGenerator/HTMLFile";
                }
                else
                {
                    if (AppSettings.ShipmentCreatedFrom == "BATCH")
                    {
                        xmlPath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/HTMLFile";
                    }
                    else
                    {
                        xmlPath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");
                    }
                }

                if (!Directory.Exists(xmlPath))
                {
                    Directory.CreateDirectory(xmlPath);
                }

                xmlPath = xmlPath + "/tempDHLPickupAM.xml";

                if (File.Exists(xmlPath))
                {
                    File.Delete(xmlPath);
                }
                using (var xmlWriter = XmlWriter.Create(xmlPath))
                {
                    xmlWriter.WriteStartDocument();
                    if (logisticIntegration != null)
                    {

                        xmlWriter.WriteStartElement("req", "BookPURequest", "http://www.dhl.com");
                        xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                        xmlWriter.WriteAttributeString("xsi", "schemaLocation", null, "http://www.dhl.com book-pickup-global-req.xsd");
                        xmlWriter.WriteAttributeString(null, "schemaVersion", null, "3.0");

                        //RequestNode                      
                        xmlWriter.WriteStartElement("Request");
                        xmlWriter.WriteStartElement("ServiceHeader");

                        xmlWriter.WriteStartElement("MessageTime");
                        xmlWriter.WriteString(dhlRequestObject.Request.ServiceHeader.MessageTime);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("MessageReference");
                        xmlWriter.WriteString("12345678912345678912345678901");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("SiteID");
                        xmlWriter.WriteString(dhlRequestObject.Request.ServiceHeader.SiteID);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Password");
                        xmlWriter.WriteString(dhlRequestObject.Request.ServiceHeader.Password);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("MetaData");

                        xmlWriter.WriteStartElement("SoftwareName");
                        xmlWriter.WriteString("XMLPI");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("SoftwareVersion");
                        xmlWriter.WriteString("1.0");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        //End RequestNode

                        xmlWriter.WriteStartElement("RegionCode");
                        xmlWriter.WriteString(!string.IsNullOrWhiteSpace(dhlRequestObject.RegionCode) ? dhlRequestObject.RegionCode.Trim() : "");
                        xmlWriter.WriteEndElement();


                        //Requestor
                        xmlWriter.WriteStartElement("Requestor");

                        xmlWriter.WriteStartElement("AccountType");
                        xmlWriter.WriteString("D");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("AccountNumber");
                        xmlWriter.WriteString(dhlRequestObject.Billing.ShipperAccountNumber);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("RequestorContact");

                        xmlWriter.WriteStartElement("PersonName");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.Contact.PersonName);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Phone");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.Contact.PhoneNumber);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("CompanyName");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.CompanyName);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Address1");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.AddressLine[0] != null ? dhlRequestObject.Shipper.AddressLine[0].AddressLine : "");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Address2");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.AddressLine[1] != null ? dhlRequestObject.Shipper.AddressLine[1].AddressLine : "");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("City");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.City);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("CountryCode");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.CountryCode);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();
                        //End  Requestor


                        //place                      
                        xmlWriter.WriteStartElement("Place");

                        xmlWriter.WriteStartElement("LocationType");
                        xmlWriter.WriteString("B");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("CompanyName");
                        xmlWriter.WriteString("FRAYTE Logistics Ltd");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Address1");
                        xmlWriter.WriteString("Unit 11, SA1 Industrial Park");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Address2");
                        xmlWriter.WriteString("Langdon Road,");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("PackageLocation");
                        xmlWriter.WriteString("FRAYTE Logistics Ltd");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("City");
                        xmlWriter.WriteString("Swansea");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("CountryCode");
                        xmlWriter.WriteString("GB");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("PostalCode");
                        xmlWriter.WriteString("SA1 8QY");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        //End Place

                        //Pickup
                        DateTime mindatetime = dhlRequestObject.ShipmentDetails.Date;

                        if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Sunday)
                        {
                            mindatetime = dhlRequestObject.ShipmentDetails.Date.AddDays(1);
                        }
                        else if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Saturday)
                        {

                            mindatetime = dhlRequestObject.ShipmentDetails.Date.AddDays(2);
                        }
                        xmlWriter.WriteStartElement("Pickup");

                        xmlWriter.WriteStartElement("PickupDate");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.Date.ToString("yyyy-MM-dd"));
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("PickupTypeCode");
                        xmlWriter.WriteString("S");
                        xmlWriter.WriteEndElement();

                        var time = GetFormmatedTimeUK(dhlRequestObject.CollectionTime);
                        TimeSpan ts = TimeSpan.Parse(time);


                        xmlWriter.WriteStartElement("ReadyByTime");
                        xmlWriter.WriteString(ts.ToString("hh':'mm"));
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("CloseTime");
                        xmlWriter.WriteString("17:00");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Pieces");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.Pieces.Count().ToString());
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("weight");

                        var weight = dhlRequestObject.ShipmentDetails.Pieces.Sum(S => S.Weight);

                        xmlWriter.WriteStartElement("Weight");
                        xmlWriter.WriteString(weight.ToString("0.##"));
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("WeightUnit");
                        xmlWriter.WriteString("K");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();


                        //End Pickup

                        // PickupContact
                        xmlWriter.WriteStartElement("PickupContact");

                        xmlWriter.WriteStartElement("PersonName");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.Contact.PersonName.ToString());
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Phone");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.Contact.PhoneNumber.ToString());
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("PhoneExtension");
                        xmlWriter.WriteString("123");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();
                        //End  PickupContact

                        //ShipmentDetails

                        xmlWriter.WriteStartElement("ShipmentDetails");

                        xmlWriter.WriteStartElement("AccountType");
                        xmlWriter.WriteString("D");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("AccountNumber");
                        xmlWriter.WriteString(dhlRequestObject.Billing.ShipperAccountNumber);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("AWBNumber");
                        xmlWriter.WriteString(dhlRequestObject.AWBNumber);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("NumberOfPieces");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.NumberOfPieces);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Weight");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.Weight);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("WeightUnit");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.WeightUnit);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("GlobalProductCode");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.GlobalProductCode);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("DoorTo");
                        xmlWriter.WriteString("DD");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("DimensionUnit");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.DimensionUnit);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Pieces");

                        foreach (var item in dhlRequestObject.ShipmentDetails.Pieces)
                        {
                            xmlWriter.WriteStartElement("Piece");

                            xmlWriter.WriteStartElement("Weight");
                            xmlWriter.WriteString(item.Weight.ToString());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("Width");
                            xmlWriter.WriteString(item.Width.ToString());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("Height");
                            xmlWriter.WriteString(item.Height.ToString());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("Depth");
                            xmlWriter.WriteString(item.Depth.ToString());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteEndElement();
                        }
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        //End ShipmentDetails
                    }
                    xmlWriter.WriteEndDocument();
                }
                return xmlPath;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string CreateXMLForDHLPickupforEU(DHLShipmentRequestDto dhlRequestObject)
        {
            string xmlPath = string.Empty;
            try
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = new UTF8Encoding(true);
                xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
                xmlWriterSettings.Indent = true;
                var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.DHL);

                dhlRequestObject.Request.ServiceHeader.SiteID = logisticIntegration.UserName;
                dhlRequestObject.Request.ServiceHeader.Password = logisticIntegration.Password;

                //xmlPath = It will be the path where xml will saved  

                if (AppSettings.LabelSave == "")
                {
                    xmlPath = AppSettings.WebApiPath + "/UploadFiles/PDFGenerator/HTMLFile";
                }
                else
                {
                    if (AppSettings.ShipmentCreatedFrom == "BATCH")
                    {
                        xmlPath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/HTMLFile";
                    }
                    else
                    {
                        xmlPath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");
                    }
                }

                if (!Directory.Exists(xmlPath))
                {
                    Directory.CreateDirectory(xmlPath);
                }

                xmlPath = xmlPath + "/tempDHLPickupEU.xml";

                if (File.Exists(xmlPath))
                {
                    File.Delete(xmlPath);
                }
                using (var xmlWriter = XmlWriter.Create(xmlPath))
                {
                    xmlWriter.WriteStartDocument();
                    if (logisticIntegration != null)
                    {
                        xmlWriter.WriteStartElement("req", "BookPURequest", "http://www.dhl.com");
                        xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                        xmlWriter.WriteAttributeString("xsi", "schemaLocation", null, "http://www.dhl.com book-pickup-global-req.xsd");
                        xmlWriter.WriteAttributeString(null, "schemaVersion", null, "3.0");

                        //RequestNode                      
                        xmlWriter.WriteStartElement("Request");
                        xmlWriter.WriteStartElement("ServiceHeader");

                        xmlWriter.WriteStartElement("MessageTime");
                        xmlWriter.WriteString(dhlRequestObject.Request.ServiceHeader.MessageTime);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("MessageReference");
                        xmlWriter.WriteString("12345678912345678912345678901");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("SiteID");
                        xmlWriter.WriteString(dhlRequestObject.Request.ServiceHeader.SiteID);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Password");
                        xmlWriter.WriteString(dhlRequestObject.Request.ServiceHeader.Password);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("MetaData");

                        xmlWriter.WriteStartElement("SoftwareName");
                        xmlWriter.WriteString("XMLPI");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("SoftwareVersion");
                        xmlWriter.WriteString("6.2");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();
                        //End RequestNode

                        xmlWriter.WriteStartElement("RegionCode");
                        xmlWriter.WriteString(!string.IsNullOrWhiteSpace(dhlRequestObject.RegionCode) ? dhlRequestObject.RegionCode.Trim() : "");
                        xmlWriter.WriteEndElement();

                        //Requestor
                        xmlWriter.WriteStartElement("Requestor");

                        xmlWriter.WriteStartElement("AccountType");
                        xmlWriter.WriteString("D");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("AccountNumber");
                        xmlWriter.WriteString(dhlRequestObject.Billing.ShipperAccountNumber);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("RequestorContact");

                        xmlWriter.WriteStartElement("PersonName");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.Contact.PersonName);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Phone");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.Contact.PhoneNumber);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("CompanyName");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.CompanyName);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Address1");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.AddressLine[0] != null ? dhlRequestObject.Shipper.AddressLine[0].AddressLine : "");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Address2");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.AddressLine[1] != null ? dhlRequestObject.Shipper.AddressLine[1].AddressLine : "");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("City");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.City);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("CountryCode");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.CountryCode);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();
                        //End  Requestor

                        //place                      
                        xmlWriter.WriteStartElement("Place");

                        xmlWriter.WriteStartElement("LocationType");
                        xmlWriter.WriteString("B");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("CompanyName");
                        xmlWriter.WriteString("FRAYTE Logistics Ltd");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Address1");
                        xmlWriter.WriteString("Unit 11, SA1 Industrial Park");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Address2");
                        xmlWriter.WriteString("Langdon Road,");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("PackageLocation");
                        xmlWriter.WriteString("FRAYTE Logistics Ltd");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("City");
                        xmlWriter.WriteString("Swansea");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("StateCode");
                        xmlWriter.WriteString("GB");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("CountryCode");
                        xmlWriter.WriteString("GB");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("PostalCode");
                        xmlWriter.WriteString("SA1 8QY");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();
                        //End Place

                        //Pickup
                        DateTime mindatetime = dhlRequestObject.ShipmentDetails.Date;

                        if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Sunday)
                        {
                            mindatetime = dhlRequestObject.ShipmentDetails.Date.AddDays(1);
                        }
                        else if (System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(mindatetime.DayOfWeek) == FraytePickUpDay.Saturday)
                        {

                            mindatetime = dhlRequestObject.ShipmentDetails.Date.AddDays(2);
                        }
                        xmlWriter.WriteStartElement("Pickup");

                        xmlWriter.WriteStartElement("PickupDate");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.Date.ToString("yyyy-MM-dd"));
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("PickupTypeCode");
                        xmlWriter.WriteString("S");
                        xmlWriter.WriteEndElement();

                        var time = GetFormmatedTimeUK(dhlRequestObject.CollectionTime);
                        TimeSpan ts = TimeSpan.Parse(time);


                        xmlWriter.WriteStartElement("ReadyByTime");
                        xmlWriter.WriteString(ts.ToString("hh':'mm"));
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("CloseTime");
                        xmlWriter.WriteString("17:00");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Pieces");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.Pieces.Count().ToString());
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("weight");

                        var weight = dhlRequestObject.ShipmentDetails.Pieces.Sum(S => S.Weight);

                        xmlWriter.WriteStartElement("Weight");
                        xmlWriter.WriteString(weight.ToString("0.##"));
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("WeightUnit");
                        xmlWriter.WriteString("K");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();
                        //End Pickup

                        // PickupContact
                        xmlWriter.WriteStartElement("PickupContact");

                        xmlWriter.WriteStartElement("PersonName");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.Contact.PersonName.ToString());
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Phone");
                        xmlWriter.WriteString(dhlRequestObject.Shipper.Contact.PhoneNumber.ToString());
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("PhoneExtension");
                        xmlWriter.WriteString("123");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();
                        //End  PickupContact

                        //ShipmentDetails

                        xmlWriter.WriteStartElement("ShipmentDetails");

                        xmlWriter.WriteStartElement("AccountType");
                        xmlWriter.WriteString("D");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("AccountNumber");
                        xmlWriter.WriteString(dhlRequestObject.Billing.ShipperAccountNumber);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("BillToAccountNumber");
                        xmlWriter.WriteString(dhlRequestObject.Billing.ShipperAccountNumber);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("AWBNumber");
                        xmlWriter.WriteString(dhlRequestObject.AWBNumber);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("NumberOfPieces");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.NumberOfPieces);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Weight");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.Weight);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("WeightUnit");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.WeightUnit);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("GlobalProductCode");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.GlobalProductCode);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("DoorTo");
                        xmlWriter.WriteString("DD");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("DimensionUnit");
                        xmlWriter.WriteString(dhlRequestObject.ShipmentDetails.DimensionUnit);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Pieces");

                        foreach (var item in dhlRequestObject.ShipmentDetails.Pieces)
                        {
                            xmlWriter.WriteStartElement("Piece");

                            xmlWriter.WriteStartElement("Weight");
                            xmlWriter.WriteString(item.Weight.ToString());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("Width");
                            xmlWriter.WriteString(item.Width.ToString());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("Height");
                            xmlWriter.WriteString(item.Height.ToString());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("Depth");
                            xmlWriter.WriteString(item.Depth.ToString());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteEndElement();
                        }
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("SpecialService");
                        xmlWriter.WriteString("S");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();

                        //End ShipmentDetails
                    }
                    xmlWriter.WriteEndDocument();
                }
                return xmlPath;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}