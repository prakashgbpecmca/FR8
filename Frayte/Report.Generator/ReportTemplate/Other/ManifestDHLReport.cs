﻿using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit;

namespace Report.Generator.ReportTemplate.Other
{
    public partial class ManifestDHLReport : DevExpress.XtraReports.UI.XtraReport
    {
        public ManifestDHLReport()
        {
            InitializeComponent();
        }

        private void xrRichText1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = xrRichText1.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 6;
                xrRichText1.Rtf = docServer.RtfText;
            }
        }

        private void xrRichText2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = xrRichText2.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 6;
                xrRichText2.Rtf = docServer.RtfText;
            }
        }

        private void xrRichText3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = xrRichText3.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 6;
                xrRichText3.Rtf = docServer.RtfText;
            }
        }
    }
}
