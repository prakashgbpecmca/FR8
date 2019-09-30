using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit;

namespace Report.Generator.ReportTemplate.Express
{
    public partial class FrayteMAWB : DevExpress.XtraReports.UI.XtraReport
    {
        public FrayteMAWB()
        {
            InitializeComponent();
          

        }

     

        private void xrRichText1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
             XRRichText richText = (XRRichText)sender;

            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = richText.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 10;
                richText.Rtf = docServer.RtfText;
            }
        }

        private void xrRichText2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRRichText richText = (XRRichText)sender;

            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = richText.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Arial";
                docServer.Document.DefaultCharacterProperties.FontSize = 10;
                richText.Rtf = docServer.RtfText;
            }
        }

        private void xrShape1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }
    }
}
