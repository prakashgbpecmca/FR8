using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using Frayte.Services.Business;

namespace Report.Generator.ReportTemplate.Other
{
    public partial class HKQuote : DevExpress.XtraReports.UI.XtraReport
    {
        public HKQuote(string ShipFrom, string ShipTo)
        {
            InitializeComponent();

            string FormatText = @"{\rtf1\deff0{\fonttbl{\f0 Calibri;}{\f1 Times New Roman;}{\f2 Arial;}}{\colortbl ;\red0\green0\blue0 ;\red0\green0\blue255 ;\caccentsix\ctint255\cshade255\red247\green150\blue70 ;}{\*\defchp \f1\fs24}{\stylesheet {\ql\f1\fs24 Normal;}{\*\cs1\f1\fs24\cf1 Default Paragraph Font;}{\*\cs2\sbasedon1\f1\fs24\cf1 Line Number;}{\*\cs3\ul\f1\fs24\cf2 Hyperlink;}{\*\ts4\tsrowd\f1\fs24\ql\tscellpaddfl3\tscellpaddl108\tscellpaddfb3\tscellpaddfr3\tscellpaddr108\tscellpaddft3\tsvertalt\cltxlrtb Normal Table;}{\*\ts5\tsrowd\sbasedon4\f1\fs24\ql\trbrdrt\brdrs\brdrw10\trbrdrl\brdrs\brdrw10\trbrdrb\brdrs\brdrw10\trbrdrr\brdrs\brdrw10\trbrdrh\brdrs\brdrw10\trbrdrv\brdrs\brdrw10\tscellpaddfl3\tscellpaddl108\tscellpaddfr3\tscellpaddr108\tsvertalt\cltxlrtb Table Simple 1;}}{\*\listoverridetable}\nouicompat\splytwnine\htmautsp\pard\plain\ql\sl275\slmult1{\ul\f2\fs30\cf3 ShipFrom}{\f2\fs18\cf3  }{\f2\fs20\cf1 TO}{\fs22\cf1  }{\ul\f2\fs30\cf3 ShipTo}\fs22\cf1\par\pard\plain\ql\sl275\slmult1\f1\fs24\cf1\par}";
            FormatText = FormatText.Replace("ShipFrom", ShipFrom);
            FormatText = FormatText.Replace("ShipTo", ShipTo);
            xrRichText1.Rtf = FormatText;            
        }
    }
}
