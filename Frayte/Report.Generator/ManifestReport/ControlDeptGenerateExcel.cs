using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Business;
using Frayte.Services.Utility;
using Frayte.Services.Models;
using Report.Generator.ReportTemplate;
using System.Web;
using System.Net;
using System.Net.Http;
using DevExpress.XtraReports.Parameters;
using System.IO;

namespace Report.Generator.ManifestReport
{
    public class ControlDeptGenerateExcel
    {
        public FrayteResult GenerateManifestReport()
        {
            var ss = new eCommerceControlDeptRepository().GetUnpaidShipments();

            FrayteResult FR = new FrayteResult();
            ReportTemplate.Other.ControlDeptExcel report = new ReportTemplate.Other.ControlDeptExcel();
            report.DataSource = ss;
            report.ExportToXlsx(AppSettings.ControlDeptFolderPath);
            var FileName = "ControlDeptExcel.xlsx";
            var FilePath = AppSettings.ControlDeptFolderPath;
            new eCommerceControlDeptRepository().MailSendToControlDept(FileName, FilePath, ss);
            return FR;
        }
    }
}
