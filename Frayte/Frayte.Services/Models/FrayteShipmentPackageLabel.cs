using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteShipmentPackageLabel
    {
        public string PageStyleSheet { get; set; }
        public string BootStrapStyleSheet { get; set; }
        public string pdfLogo { get; set; }
        public List<FraytePackageLabel> PackageLabel { get; set; }
    }

    public class FraytePackageLabel
    {
        public string LabelPath { get; set; }
    }

    public class FraytePackageResult
    {
        public string PackagePath { get; set; }
        public string FileName { get; set; }
        public bool IsDownloaded { get; set; }
    }

    public class FrayteManifestName : FrayteTNTResult
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public bool FileStatus { get; set; }
        public string TotalWeight { get; set; }
        public string TotalCost { get; set; }
        public string TransitTime { get; set; }
        public string OfferValidity { get; set; }
    }
}
