using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteLogisticIntegration
    {
        public int LogisticIntegrationId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ServiceUrl { get; set; }
        public string Mode { get; set; }
        public string InetgrationKey { get; set; }
        public string IntegrationName { get; set; }
        public string AppVersion { get; set; }
        public string AppId { get; set; }
        public string UserAgent { get; set; }
        public string VoidApiUrl { get; set; }
        public string PickupApiUrl { get; set; }
        public string LabelApiUrl { get; set; }

    }

    public static class FrayteIntegration
    {
        public const string EasyPost = "EasyPost";
        public const string ParcelHub = "ParcelHub";
        public const string TNT = "TNT";
        public const string UPS = "UPS";
        public const string DHL = "DHL";
        public const string Aftership = "aftership";
        public const string DPD = "DPD";
        public const string SKYPOSTAL = "SKYPOSTAL";
        public const string AU = "AU";
        public const string EAM = "EAM";
        public const string DPDCH = "DPDCH";
        public const string ETOWER = "ETOWER";
        public const string BRING = "BRING";
        public const string HERMES = "HERMES";
        public const string USPS = "USPS";
    }
}
