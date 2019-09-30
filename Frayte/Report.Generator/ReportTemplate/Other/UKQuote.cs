using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using Frayte.Services.Business;
using System.Text.RegularExpressions;

namespace Report.Generator.ReportTemplate.Other
{
    public partial class UKQuote : DevExpress.XtraReports.UI.XtraReport
    {
        string user = string.Empty;
        public UKQuote(string ShipFrom, string ShipTo, string UserType)
        {
            InitializeComponent();

            user = UserType;
            string FormatText = @"{\rtf1\deff0{\fonttbl{\f0 Calibri;}{\f1 Times New Roman;}{\f2 Arial;}}{\colortbl ;\red0\green0\blue0 ;\red0\green0\blue255 ;\caccentsix\ctint255\cshade255\red247\green150\blue70 ;}{\*\defchp \f1\fs24}{\stylesheet {\ql\f1\fs24 Normal;}{\*\cs1\f1\fs24\cf1 Default Paragraph Font;}{\*\cs2\sbasedon1\f1\fs24\cf1 Line Number;}{\*\cs3\ul\f1\fs24\cf2 Hyperlink;}{\*\ts4\tsrowd\f1\fs24\ql\tscellpaddfl3\tscellpaddl108\tscellpaddfb3\tscellpaddfr3\tscellpaddr108\tscellpaddft3\tsvertalt\cltxlrtb Normal Table;}{\*\ts5\tsrowd\sbasedon4\f1\fs24\ql\trbrdrt\brdrs\brdrw10\trbrdrl\brdrs\brdrw10\trbrdrb\brdrs\brdrw10\trbrdrr\brdrs\brdrw10\trbrdrh\brdrs\brdrw10\trbrdrv\brdrs\brdrw10\tscellpaddfl3\tscellpaddl108\tscellpaddfr3\tscellpaddr108\tsvertalt\cltxlrtb Table Simple 1;}}{\*\listoverridetable}\nouicompat\splytwnine\htmautsp\pard\plain\ql\sl275\slmult1{\ul\f2\fs30\cf3 ShipFrom}{\f2\fs18\cf3  }{\f2\fs20\cf1 TO}{\fs22\cf1  }{\ul\f2\fs30\cf3 ShipTo}\fs22\cf1\par\pard\plain\ql\sl275\slmult1\f1\fs24\cf1\par}";
            FormatText = FormatText.Replace("ShipFrom", ShipFrom);
            FormatText = FormatText.Replace("ShipTo", ShipTo);
            xrRichText1.Rtf = FormatText;
        }

        private void xrLabel8_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (user == "SPECIAL")
            {
                this.xrLabel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(32)))), ((int)(((byte)(109)))));
            }
            else
            {
                this.xrLabel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(61)))), ((int)(((byte)(90)))));
            }
        }

        private void xrLabel12_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel xr = (XRLabel)sender;
            xr.Text = Regex.Replace(xr.Text, @"\s", "") == "0.00" ? "N/A" : xr.Text;
        }

        private void xrLabel41_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel xr1 = (XRLabel)sender;
            xr1.Text = Regex.Replace(xr1.Text, @"\s", "") == "0.00" ? "N/A" : xr1.Text;
        }

        private void xrLabel20_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel xr2 = (XRLabel)sender;
            xr2.Text = Regex.Replace(xr2.Text, @"\s", "") == "0.00" ? "N/A" : xr2.Text;
        }

        private void xrLabel42_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel xr3 = (XRLabel)sender;
            xr3.Text = Regex.Replace(xr3.Text, @"\s", "") == "0.00" ? "N/A" : xr3.Text;
        }

        private void xrLabel49_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel xr4 = (XRLabel)sender;
            xr4.Text = Regex.Replace(xr4.Text, @"\s", "") == "0.00" ? "N/A" : xr4.Text;
        }
    }
}
