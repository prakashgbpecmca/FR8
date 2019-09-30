using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace Report.Generator.ReportTemplate.UKRateCards
{
    public partial class DHL3rdPartyEconomy : DevExpress.XtraReports.UI.XtraReport
    {
        int rowno = 0;
        public DHL3rdPartyEconomy()
        {
            InitializeComponent();
        }

        private void Detail2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            rowno++;
            if (rowno == 2)
            {
                xrTableCell63.Text = "Origin";
                xrTableCell63.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
                xrTableCell63.ForeColor = System.Drawing.Color.White;
            }
            else if (rowno == 3)
            {
                xrTableCell63.Text = "Zone";
                xrTableCell63.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                xrTableCell63.ForeColor = System.Drawing.Color.White;
            }
            else if (rowno == 5)
            {
                xrTableCell63.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom));
                xrTableCell64.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                xrTableCell65.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                xrTableCell66.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                xrTableCell67.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                xrTableCell68.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
                xrTableCell69.Borders = ((DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom));
            }
            else
            {
                xrTableCell63.Text = "";
            }
        }
    }
}
