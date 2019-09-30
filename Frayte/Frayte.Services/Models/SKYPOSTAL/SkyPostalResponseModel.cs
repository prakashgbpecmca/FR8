using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.SKYPOSTAL
{
    public class SkyPostalResponseModel
    {
        public int success { get; set; }
        public List<ResponseDetail> response { get; set; }
        public FratyteError Error { get; set; }
        public string Rawrequest { get; set; }
        public string Rawresponse { get; set; }
    }
    public class ResponseDetail
    {
        public string TRCK_NMR_FOL { get; set; }
        public string REFERENCE_NUMBER { get; set; }
        public string PRE_RECEPT_HEADER_ID { get; set; }
        public string INVOICE_URL { get; set; }
        public string LABEL_URL { get; set; }
        public string LABEL_URL_PDF { get; set; }
        public string LABEL_ZPL { get; set; }
    }

}
