using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public static class SMSCountries
    {
        public const string UK = "GBR";
        public const string IND = "IND";
        public const string AUS = "AUS";
        public const string USA = "USA";

    }

    public class eCommerceBusinessZone
    {
        public const string UK = "GBR";
        public const string HKG = "HKG";
    }
    public static class SMSNotificationType
    {
        public const string LabelCreated = "ShipmentLabelCreated";
    }

    public static class SiteURL
    {
        public const string SiteUK = "frayte.co.uk";
        public const string SiteHKG = "frayte.com";
    }

    public static class SMSShipmentNotification
    {
        public const string FirstName = "@FirstName";
        public const string LastName = "@LastName";
        public const string Amount = "@Amount";
        public const string CurrencyCode = "@CurrencyCode";
        public const string FrayteNumber = "@FrayteNumber";
        public const string TrackingNumber = "@TrackingNo";
        public const string CourierCompany = "@CourierCompany";
        public const string Email = "@Email";
        public const string URL = "@Url";
    }
}
