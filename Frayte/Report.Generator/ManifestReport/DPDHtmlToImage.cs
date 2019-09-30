using DevExpress.XtraPrinting;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using Report.Generator.ReportTemplate;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace Report.Generator.ManifestReport
{

    public class DPDHtmlToImage
    {
        public string HtmlToImage(int DirectshipmentId, string html, string trackingCode)
        {
            FrayteResult result = new FrayteResult();
            string resultPath = string.Empty;
            try
            {
                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(html);
                writer.Flush();
                stream.Position = 0;

                ReportTemplate.Other.DPDHtmlToImageReport HtmlToImage = new ReportTemplate.Other.DPDHtmlToImageReport();
                HtmlToImage.xrRichText1.LoadFile(stream, DevExpress.XtraReports.UI.XRRichTextStreamType.HtmlText);

                if (AppSettings.LabelSave == "")
                {
                    resultPath = AppSettings.WebApiPath + "/PackageLabel/" + DirectshipmentId + "/" + "TNT_" + trackingCode + ".jpg";
                    if (!System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/" + DirectshipmentId + "/"))

                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/" + DirectshipmentId + "/");

                    ImageExportOptions opts = new ImageExportOptions(ImageFormat.Png);
                    opts.ExportMode = ImageExportMode.SingleFile;
                    opts.Resolution = 250;
                    HtmlToImage.ExportToImage(resultPath, opts);
                    result.Status = true;
                }
                else
                {
                    if (AppSettings.ShipmentCreatedFrom == "BATCH")
                    {
                        resultPath = AppSettings.WebApiPath + "/PackageLabel/" + DirectshipmentId + "/" + "TNT_" + trackingCode + ".jpg";
                        if (!System.IO.Directory.Exists(AppSettings.LabelFolder + DirectshipmentId + "/"))
                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + DirectshipmentId + "/");
                    }
                    else
                    {
                        resultPath = HttpContext.Current.Server.MapPath("~/PackageLabel/" + DirectshipmentId + "/" + "TNT_" + trackingCode + ".jpg");
                        if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(AppSettings.LabelFolder + DirectshipmentId + "/")))
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.LabelFolder + DirectshipmentId + "/"));
                    }
                    ImageExportOptions opts = new ImageExportOptions(ImageFormat.Png);
                    opts.ExportMode = ImageExportMode.SingleFile;
                    opts.Resolution = 250;
                    HtmlToImage.ExportToImage(resultPath, opts);
                    result.Status = true;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
            }

            if (result.Status)
            {
                return resultPath;
            }
            else
            {
                return "";
            }
        }
    }

}
