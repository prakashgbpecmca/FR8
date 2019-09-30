using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace Report.Generator.ReportTemplate.HKRateCards
{
    public partial class TNTExportExpress : DevExpress.XtraReports.UI.XtraReport
    {
        int rowno = 0;
        public TNTExportExpress()
        {
            InitializeComponent();
        }

        private void Detail1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            rowno++;
            switch (rowno)
            {
                case 1:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 2:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 3:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 4:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 5:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = true;
                    this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(0.07281494F, 11.00F);
                    this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(0.15F, 2.00F);
                    this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(0.15F, 13.00F);
                    this.xrTableCell1.Borders = (DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
                    this.xrTableCell2.Borders = (DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
                    this.xrTableCell3.Borders = (DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
                    this.xrTableCell4.Borders = (DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
                    this.xrTableCell5.Borders = (DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
                    this.xrTableCell6.Borders = (DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
                    this.xrTableCell7.Borders = (DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
                    this.xrTableCell8.Borders = (DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
                    this.xrTableCell9.Borders = (DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
                    this.xrTableCell10.Borders = (DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
                    this.xrTableCell11.Borders = (DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
                    this.xrTableCell23.Borders = (DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
                    this.xrTableCell24.Borders = (DevExpress.XtraPrinting.BorderSide)(DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom);
                    this.GroupHeader1.HeightF = 22.00F;
                    break;
                case 6:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 7:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 8:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 9:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 10:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 11:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 12:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 13:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 14:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 15:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 16:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 17:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 18:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 19:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 20:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 21:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 22:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 23:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 24:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 25:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 26:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 27:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 28:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 29:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 30:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 31:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 32:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 33:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 34:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 35:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 36:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 37:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 38:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 39:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 40:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 41:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 42:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 43:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 44:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 45:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(0.15F, 14.29f);
                    this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(0.15F, 25.29f);
                    this.GroupHeader1.HeightF = 33.29166F;
                    break;
                case 46:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 47:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 48:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 49:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
                case 50:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Transparent;
                    this.xrLabel2.Visible = false;
                    break;
                case 51:
                    this.xrTable2.EvenStyleName = "xrControlStyle5";
                    this.xrTable2.BackColor = System.Drawing.Color.Silver;
                    this.xrLabel2.Visible = false;
                    break;
            }
        }
    }
}
