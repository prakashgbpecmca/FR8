using Frayte.Api.Business;
using Frayte.Api.Models;
using Frayte.Services.Business;
using Frayte.Services.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Frayte.Api.Controllers
{
    public class LabelController : ApiController
    {
        [HttpPost]
        public IHttpActionResult GetLabelInfo(LabelRecoveryDto LabelRecovery)
        {
            LabelResponeDto LableRespone = new LabelResponeDto();
            if (LabelRecovery.LabelRequest.LabelType == "JPG")
            {
                var labelInfo = new APIShipmentRepository().GetLabelInfo(LabelRecovery.LabelRequest);
                LableRespone.LabelDetail = new List<LabelDetails>();

                foreach (var item in labelInfo)
                {
                    var pdfFileName = AppSettings.LabelVirtualPath + "/PackageLabel/" + item.DirectShipmentId + "/" + item.Image;
                    var lableDeatil = new LabelDetails();
                    lableDeatil.LabelUrl = pdfFileName;
                    lableDeatil.LabelName = item.Image;
                    lableDeatil.TrackingNo = item.TrackingNo;
                    lableDeatil.LabelType = ".jpg";
                    LableRespone.LabelDetail.Add(lableDeatil);
                }
                if (labelInfo.Count > 0)
                {
                    LableRespone.Status = true;
                    LableRespone.Discription = "Lable Information Recevied Successfully.";
                    LableRespone.TrackingNo = labelInfo.FirstOrDefault().UniqueTrackingNumber;                  
                }
            }
            if (LabelRecovery.LabelRequest.LabelType == "PDF")
            {
                var lableinfo = new APIShipmentRepository().GetLabelInfo(LabelRecovery.LabelRequest);
                LableRespone.LabelDetail = new List<LabelDetails>();
                foreach (var item in lableinfo)
                {
                    item.Image = item.Image.Replace(".jpg", ".pdf");
                    var pdfFileName = AppSettings.LabelVirtualPath + "/PackageLabel/" + item.DirectShipmentId + "/" + item.Image;
                    var lableDeatil = new LabelDetails();
                    lableDeatil.LabelUrl = pdfFileName;
                    lableDeatil.TrackingNo = item.TrackingNo;
                    lableDeatil.LabelType = "PDF";
                    LableRespone.LabelDetail.Add(lableDeatil);
                }
                if (lableinfo.Count > 0)
                {
                    LableRespone.Status = true;
                    LableRespone.Discription = "Lable Information Recevied Successfully.";
                    LableRespone.TrackingNo = lableinfo.FirstOrDefault().UniqueTrackingNumber;
                }
            }
            return Ok(LableRespone);
        }
    }
}
