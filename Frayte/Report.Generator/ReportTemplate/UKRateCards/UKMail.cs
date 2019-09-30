using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace Report.Generator.ReportTemplate.UKRateCards
{
    public partial class UKMail : DevExpress.XtraReports.UI.XtraReport
    {
        public UKMail()
        {
            InitializeComponent();
        }

        private void xrTableCell8_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (xrTableCell6.Text == Convert.ToString(0.0m))
            {
                xrTableCell6.Text = "N/A";
            }
            if (xrTableCell7.Text == Convert.ToString(0.0m))
            {
                xrTableCell7.Text = "N/A";
            }
            if (xrTableCell8.Text == Convert.ToString(0.0m))
            {
                xrTableCell8.Text = "N/A";
            }
        }
    }
}
