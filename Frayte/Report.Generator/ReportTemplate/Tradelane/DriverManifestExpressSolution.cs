﻿using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit;

namespace Report.Generator.ReportTemplate.Tradelane
{
    public partial class DriverManifestExpressSolution : DevExpress.XtraReports.UI.XtraReport
    {
        public DriverManifestExpressSolution()
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
               ////docServer.RtfText = xrrichtext.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 9;
                //xrRichText2.Rtf = docServer.RtfText;
            }
        }

        private void xrRichText5_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = xrRichText5.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 9;
                xrRichText5.Rtf = docServer.RtfText;
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
                docServer.Document.DefaultCharacterProperties.Italic = true;
                docServer.Document.DefaultCharacterProperties.FontSize = 8;
                xrRichText4.Rtf = docServer.RtfText;
            }
        }
    }
}
