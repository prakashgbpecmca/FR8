using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace Report.Generator.ReportTemplate.HKRateCards
{
    public partial class TNTImportEconomySummery : DevExpress.XtraReports.UI.XtraReport
    {
        int rowno = 0;
        int growno = 0;
        public TNTImportEconomySummery()
        {
            InitializeComponent();
        }

        private void Detail1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            rowno++;
            switch (rowno)
            {
                case 1:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    break;
                case 2:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    break;
                case 3:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    break;
                case 4:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    break;
                case 5:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    break;
                case 6:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    break;
                case 7:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    break;
                case 8:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    break;
                case 9:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    break;
                case 10:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    break;
                case 11:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    break;
                case 12:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    break;
                case 13:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    break;
                case 14:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    break;
                case 15:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    break;
                case 16:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    break;
                case 17:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    break;
                case 18:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    break;
                case 19:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    break;
                case 20:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    break;
                case 21:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    break;
                case 22:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    break;
                case 23:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    break;
                case 24:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    break;
                case 25:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    break;
                case 26:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    break;
                case 27:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    break;
                case 28:
                    this.xrTable2.EvenStyleName = "xrControlStyle3";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    break;
            }
        }

        private void GroupHeader1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            growno++;
            switch (growno)
            {
                case 1:
                    this.xrLabel2.Visible = true;
                    break;
                case 2:
                    this.xrLabel2.Visible = false;
                    this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(0.645813F, 12.00F);
                    this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(0.645813F, 32.00F);
                    break;
            }
        }
    }
}
