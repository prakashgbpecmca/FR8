using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public static class eCommerceAppManifestTracking
    {
        public const string Tracking1 = "Received";
    }

    public static class eCommerceAppShipmentTracking
    {
        public const string Tracking1 = "Received";
    }
    public class eCommerceAppErrorMessage
    {
        public const string InvalidBarcode = "Invalid Barcode Number";
        public const string ErrorProcessingBarcode = "Could not process the barcode. Please try again.";
        public const string ErrorProcessing = "Somethinh has gone wrong. Please try again.";
    }

    public static class eCommerceAppTaxAndDutyStatus
    {
        public const string TaxAndDutyPaid = "Paid";
        public const string TaxAndDutyUnPaid = "UnPaid";
        public const string TaxAndDutyPartiallyPaid = "PartiallyPaid";
    }
}
