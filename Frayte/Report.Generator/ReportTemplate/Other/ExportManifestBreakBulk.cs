using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit;

namespace Report.Generator.ReportTemplate.Other
{
    public partial class ExportManifestBreakBulk : DevExpress.XtraReports.UI.XtraReport
    {
        public ExportManifestBreakBulk()
        {
            InitializeComponent();
        }

        private void xrRichText1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = xrRichText1.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 10;
                xrRichText1.Rtf = docServer.RtfText;
            }
        }

        private void xrRichText2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = xrRichText2.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 8;
                xrRichText2.Rtf = docServer.RtfText;
            }
        }

        private void xrRichText3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = xrRichText3.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 8;
                xrRichText3.Rtf = docServer.RtfText;
            }
        }

        private void xrRichText4_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = xrRichText4.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 8;
                xrRichText4.Rtf = docServer.RtfText;
            }
        }

        private void xrRichText5_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = xrRichText5.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 8;
                xrRichText5.Rtf = docServer.RtfText;
            }
        }

        private void xrRichText6_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = xrRichText6.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 8;
                xrRichText6.Rtf = docServer.RtfText;
            }
        }

        private void xrRichText7_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = xrRichText7.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 8;
                xrRichText7.Rtf = docServer.RtfText;
            }
        }

        private void xrRichText8_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = xrRichText8.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 8;
                xrRichText8.Rtf = docServer.RtfText;
            }
        }

        private void xrRichText9_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = xrRichText9.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 8;
                xrRichText9.Rtf = docServer.RtfText;
            }
        }

        private void xrRichText10_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = xrRichText10.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 8;
                xrRichText10.Rtf = docServer.RtfText;
            }
        }

        private void xrRichText11_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = xrRichText11.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 8;
                xrRichText11.Rtf = docServer.RtfText;
            }
        }

        private void xrRichText12_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = xrRichText12.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 8;
                xrRichText12.Rtf = docServer.RtfText;
            }
        }

        private void xrRichText13_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = xrRichText13.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 8;
                xrRichText13.Rtf = docServer.RtfText;
            }
        }

        private void xrRichText14_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = xrRichText14.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 10;
                xrRichText14.Rtf = docServer.RtfText;
            }
        }
    }
}
