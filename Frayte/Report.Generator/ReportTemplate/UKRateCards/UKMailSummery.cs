using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace Report.Generator.ReportTemplate.UKRateCards
{
    public partial class UKMailSummery : DevExpress.XtraReports.UI.XtraReport
    {
        public UKMailSummery()
        {
            InitializeComponent();
        }

        private void xrTableCell13_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (xrTableCell9.Text == Convert.ToString(0.0m))
            {
                xrTableCell9.Text = "N/A";
            }
            if (xrTableCell12.Text == Convert.ToString(0.0m))
            {
                xrTableCell12.Text = "N/A";
            }
            if (xrTableCell13.Text == Convert.ToString(0.0m))
            {
                xrTableCell13.Text = "N/A";
            }
        }
    }
}
