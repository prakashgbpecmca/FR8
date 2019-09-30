using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace Report.Generator.ReportTemplate.UKRateCards
{
    public partial class HermesSummery : DevExpress.XtraReports.UI.XtraReport
    {
        public HermesSummery()
        {
            InitializeComponent();
        }

        private void Hermes_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            
        }
    }
}
