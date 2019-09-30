using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace Report.Generator.ReportTemplate.HKRateCards
{
    public partial class ThirdPartyMatrixEconomy : DevExpress.XtraReports.UI.XtraReport
    {
        int rowno = 0;
        public ThirdPartyMatrixEconomy()
        {
            InitializeComponent();
        }

        private void Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            rowno++;
            if (rowno == 6)
            {
                xrTableCell29.Text = "Origin";
                xrTableCell29.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
                xrTableCell29.ForeColor = System.Drawing.Color.White;
            }
            else if (rowno == 7)
            {
                xrTableCell29.Text = "Zone";
                xrTableCell29.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                xrTableCell29.ForeColor = System.Drawing.Color.White;
            }
            else if (rowno == 13)
            {
                xrTableCell29.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                xrTableCell29.Text = "";
            }
        }
    }
}