using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.ETower
{
    public class Datum
    {
        public string status { get; set; }
        public object errors { get; set; }
        public string orderId { get; set; }
        public string labelContent { get; set; }
        public string referenceNo { get; set; }
        public string trackingNo { get; set; }
    }

    public class EtowerResponseModel
    {
        public string status { get; set; }
        public List<Error> errors { get; set; }
        public List<Datum> data { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
       
    }



    public class EtowerErrorModel
    {
        public int code { get; set; }
        public string message { get; set; }
    }



    public class DatumModel
    {
        public string status { get; set; }
        public List<EtowerErrorModel> errors { get; set; }
        public object orderId { get; set; }
        public string referenceNo { get; set; }
        public string trackingNo { get; set; }
    }

    public class EtowerError
    {
        public string status { get; set; }
        public List<EtowerErrorModel> errors { get; set; }
        public List<DatumModel> data { get; set; }
    }


    public class LabelDatum
    {
        public string status { get; set; }
        public object errors { get; set; }
        public string labelContent { get; set; }
        public string orderId { get; set; }
        public string trackingNo { get; set; }
    }

    public class LabelResponseModel
    {
        public string status { get; set; }
        public object errors { get; set; }
        public List<LabelDatum> data { get; set; }
    }
}
