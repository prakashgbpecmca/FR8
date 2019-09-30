using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.FrayteMAWB
{
   public class FrayteOWNMAWB
    {
        public string FromAddress { get; set; }
        public string FromAddress1 { get; set; }
        public string FromAddress2 { get; set; }
        public string FromCity { get; set; }
        public string FromState { get; set; }
        public string FromZip { get; set; }
        public string FromCountry { get; set; }
        public string FromFirstName { get; set; }
        public string FromLastName { get; set; }
        public string FromCompanyName { get; set; }
        public string FromArea { get; set; }
        public string FromPhone { get; set; }
        public string FromEmail { get; set; }
        public string FromPhoneCode { get; set; }
        public string ToPhoneCode { get; set; }
        public string FromShipperReference { get; set; }
        public string FromBillToAccount { get; set; }
        public string FromTaxDutyPayment { get; set; }
        public string FromReasonForExport { get; set; }
        public string FromOrigin { get; set; }
        public string ToAddress { get; set; }
        public string ToPhone { get; set; }
        public string ToDescription { get; set; }
        public string ToGrossWeight { get; set; }
        public string ToEmail { get; set; }
        public string ToDestination { get; set; }
        public string ToAddress1 { get; set; }
        public string ToAddress2 { get; set; }
        public string ToCity { get; set; }
        public string ToState { get; set; }
        public string ToZip { get; set; }
        public string ToCountry { get; set; }
        public string ToFirstName { get; set; }
        public string ToLastName { get; set; }
        public string ToCompanyName { get; set; }
        public string ToArea { get; set; }
        public string FinalMileCarrier { get; set; }
        public string AirWayBill { get; set; }
        public string Service { get; set; }
        public string IncoTerms { get; set; }
        public string Code1 { get; set; }
        public string Barcode2 { get; set; }
        public string DisplayValueBarcode2 { get; set; }
        public string Barcode3 { get; set; }
        public string DisplayValueBarcode3 { get; set; }
        public DateTime LabelPrintDate { get; set; }
        public string ServiceCode { get; set; }
        public decimal TotalValue { get; set; }
        public string TotalCustomsValue { get; set; }
        public TableData TableProperties { get; set; }
        public string AdditionalInfo { get; set; }
        public string LabelPerShipment { get; set; }
        public string Currency { get; set; }      
        public string ConsignmentNo { get; set; }
    }
    public class TableData
    {
        public string TableDescription { get; set; }
        public decimal TableValue { get; set; }
        public string TableCurrency { get; set; }
    }
}
