using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Frayte.Services.Models.FrayteQuotationEmailModel;

namespace Frayte.Services.Models
{
    public class FrayteQuotationShipment
    {
        public int QuotationShipmentId { get; set; }
        public int OperationZoneId { get; set; }
        public int CustomerId { get; set; }
        public string CompanyName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public QuotationAddress QuotationFromAddress { get; set; }
        public QuotationAddress QuotationToAddress { get; set; }
        public List<QuotationPackage> QuotationPackages { get; set; }
        public DirectBookingService QuotationRateCard { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string PakageCalculatonType { get; set; }
        public decimal? TotalEstimatedWeight { get; set; }
        public decimal? TotalWeight { get; set; }
        public string LogisticCompany { get; set; }
        public string RateType { get; set; }
        public FrayteParcelType ParcelType { get; set; }
        public string ShipmentType { get; set; }
        public string LogisticType { get; set; }
        public string LogisticTypeDisplay { get; set; }
        public string ShipmentTypeDisplay { get; set; }
        public string ParcelServiceType { get; set; }
        public string RateTypeDisplay { get; set; }
        public string LogisticCompanyDisplay { get; set; }
        public decimal BaseRate { get; set; }
        public decimal MarginCost { get; set; }
        public float AdditionalSurcharge { get; set; }
        public float FuelSurCharge { get; set; }
        public decimal? FuelPercent { get; set; }
        public DateTime? FuelMonthYear { get; set; }
        public float EstimatedCost { get; set; }
        public float EstimatedTotalCost { get; set; }
        public string CurrenyCode { get; set; }
        public string CourierDescription { get; set; }
        public string AddressType { get; set; }
        public int ValidDays { get; set; }
        public string TransitTime { get; set; }
        public DateTime ValidDate { get; set; }
    }

    public class QuotationPackage
    {
        public int QuotationShipmentDetailId { get; set; }
        public int QuotationShipmentId { get; set; }
        public int CartoonValue { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal Value { get; set; }
        public string Content { get; set; }
    }

    public class FrayteQuotationEmailMail
    {
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string EmailTo { get; set; }
        public string EmailCc { get; set; }
        public string EmailBcc { get; set; }
        public string SenderName { get; set; }
        public string SenderEMail { get; set; }
        public string SenderDept { get; set; }
        public string MailContent { get; set; }
        public string FuelMonth { get; set; }
        public int LoginUserId { get; set; }
        public bool IsRateSend { get; set; }
        public FrayteQuotationShipment QuotationDetail { get; set; }
    }

    public class FrayteQuotationEmailModel
    {
        public int QuotationShipmentId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CCEmail { get; set; }
        public string BCCEmail { get; set; }
        public string SiteAddress { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string EMailText { get; set; }
        public string FromCountry { get; set; }
        public string ToCountry { get; set; }
        public string FromPostCode { get; set; }
        public string ToPostCode { get; set; }
        public string TotalWeight { get; set; }
        public string TotalCost { get; set; }
        public int TransitTime { get; set; }
    }

    public class QuotationAddress
    {
        public string PostCode { get; set; }
        public FrayteCountryCode Country { get; set; }
    }

    public class FrayteSurchargeDetail
    {
        public string Surcharge { get; set; }
    }

    public class FrayteQuotationDetailPDF
    {
        public string PageStyleSheet { get; set; }
        public string BootStrapStyleSheet { get; set; }
        public string pdfLogo { get; set; }
        public FrayteQuotationShipment QuotationShipment { get; set; }
        public FrayteSurchargeDetail QuotationSurcharge { get; set; }
    }

    public class FrayteQuotationReport
    {
        public int QuotationShipmentId { get; set; }
        public int OperationZoneId { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public string QuoteCurrency { get; set; }
        public string FrayteAccountNo { get; set; }
        public DateTime QuoteIssueDate { get; set; }
        public string ShipFrom { get; set; }
        public string ShipTo { get; set; }
        public decimal TotalPrice { get; set; }
        public string OfferValidity { get; set; }
        public int CartoonQty { get; set; }        
        public decimal PricePerKg { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal Volume { get; set; }
        public string Service { get; set; }
        public string Description { get; set; }
        public string TransitTime { get; set; }
        public decimal Rate { get; set; }
        public decimal FuelSurcharge { get; set; }
        public decimal SupplementryCharge { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public decimal ChargeableWeight { get; set; }
        public string PackageCalculationType { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class FrayteSalesRepresentiveEmail
    {
        public string SalesEmail { get; set; }
        public string SalesRepresentiveName { get; set; }
        public string OperationStaffEmail { get; set; }
        public string OperationStaffName { get; set; }
        public string UserPhone { get; set; }
        public string DeptName { get; set; }
    }

    public class FrayteQuoteEmail
    {
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public string QuoteNo { get; set; }
        public string FromCountry { get; set; }
        public string ToCountry { get; set; }
        public string Carrier { get; set; }
        public string Weight { get; set; }
        public string TransitTime { get; set; }
        public string PriceValue { get; set; }
        public string Rate { get; set; }
        public string AddOnRate { get; set; }
        public string FuelSurcharge { get; set; }
        public string TotalPrice { get; set; }
        public string Validity { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string SiteAddress { get; set; }
        public string PhoneNo { get; set; }
        public string LogisticType { get; set; }
        public string ValidDate { get; set; }
        public string DeptName { get; set; }
        public int OperationZoneId { get; set; }
        public decimal FuelPercent { get; set; }
        public string FuelMonth { get; set; }
    }
}
