using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Utility;
using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.DataAccess;
using System.Web;
using Report.Generator.ReportTemplate;
namespace Report.Generator.ManifestReport
{
    public class eCommerceBagLabelReport
    {
        public FrayteResult generateeCommerceBagLabelReport(eCommerceReportBagLabel eCommerceBagLabelReport)
        {
            FrayteResult result = new FrayteResult();
            eCommerceBagLabelReport.Date= eCommerceBagLabelReport.CreatedOn.ToString("dd")+ " / "+ eCommerceBagLabelReport.CreatedOn.ToString("MM");
            List<eCommerceReportBagLabel> source = new List<eCommerceReportBagLabel>();
            source.Add(eCommerceBagLabelReport);

            ReportTemplate.Other.eCommerceBagLabel re = new ReportTemplate.Other.eCommerceBagLabel();
            re.DataSource = source;
            re.Parameters["LogoPath"].Value = eCommerceBagLabelReport.BarCodePath;
            string filePathToSave = AppSettings.eCommerceBag;
            filePathToSave = HttpContext.Current.Server.MapPath(filePathToSave) + eCommerceBagLabelReport.ShipmentBagId.ToString();

            if (!System.IO.Directory.Exists(filePathToSave))
                System.IO.Directory.CreateDirectory(filePathToSave);
            re.ExportToImage(HttpContext.Current.Server.MapPath(AppSettings.eCommerceBag) + eCommerceBagLabelReport.ShipmentBagId.ToString() + "/" + eCommerceBagLabelReport.BagName + ".Jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);

            result.Status = true;

            return result;
        }
    }
}
