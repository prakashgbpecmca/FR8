using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Business;
using Frayte.Services.Models;
using frayteWinApp.Utility;
using Newtonsoft.Json;

namespace frayteWinApp.Utility
{
    public static class CommunicateWebApi
    {
        public static FryatePODResult GetPODPath()
        {
            string result = ProcessWebApi.ExecuteGetRequest("ShipmentPDF_Report", null);
            FryatePODResult result1 = JsonConvert.DeserializeObject<FryatePODResult>(result);
            return result1;
        }

        public static FryateRateCardResult GetPDFPath(int UserId)
        {
            var OzoneId = UtilityRepository.GetOperationZone();
            string result = ProcessWebApi.ExecuteRateCardRequest("GeneratePDF?UserId=" + UserId, null);
            FryateRateCardResult result2 = JsonConvert.DeserializeObject<FryateRateCardResult>(result);
            return result2;
        }

        public static FryateExcelResult GetExcelPath(int UserId)
        {
            var OzoneId = UtilityRepository.GetOperationZone();
            string result = ProcessWebApi.ExecuteRateCardRequest("GenerateExcel?UserId=" + UserId, null);
            FryateExcelResult result3 = JsonConvert.DeserializeObject<FryateExcelResult>(result);
            return result3;
        }

        //public static List<ShipmentTracking> GetTrackingDetail()
        //{
        //    string tracking = ProcessWebApi.ExecuteDirectShipmentRequest("ParcelHubStatus", null);
        //    List<ShipmentTracking> result4 = JsonConvert.DeserializeObject<List<ShipmentTracking>>(tracking);
        //    return result4;
        //}

        //public static FrayteStatus GetUpdateFuelSurCharge(DateTime datetime)
        //{
        //    //MM/dd/yyyy
        //    string hittingURL = "GetUpdateStatus?datetime=" + datetime.Month.ToString() + "/" + datetime.Day.ToString() + "/" + datetime.Year.ToString();
        //    string status = ProcessWebApi.ExecuteFuelSurChargeRequest(hittingURL, null);
        //    FrayteStatus frayte = JsonConvert.DeserializeObject<FrayteStatus>(status);
        //    return frayte;
        //}

        //public static FrayteStatus GetSentMailStatus(DateTime datetime)
        //{
        //    //MM/dd/yyyy
        //    string hittingURL = "GetSendMailDate?datetime=" + datetime.Month.ToString() + "/" + datetime.Day.ToString() + "/" + datetime.Year.ToString();
        //    string status = ProcessWebApi.ExecuteSentMailRequest(hittingURL, null);
        //    FrayteStatus frayte = JsonConvert.DeserializeObject<FrayteStatus>(status);
        //    return frayte;
        //}

        //public static FrayteStatus GetRateSentMailStatus()
        //{
        //    string hittingURL = "GetSendMailDate";
        //    string status = ProcessWebApi.ExecuteRateSentMailRequest(hittingURL, null);
        //    FrayteStatus frayte = JsonConvert.DeserializeObject<FrayteStatus>(status);
        //    return frayte;
        //}

        //public static FrayteStatus GetExchangeHistoryDate()
        //{
        //    string hittingURL = "GetSendMailDate";
        //    string status = ProcessWebApi.ExecuteRateSentMailRequest(hittingURL, null);
        //    FrayteStatus frayte = JsonConvert.DeserializeObject<FrayteStatus>(status);
        //    return frayte;
        //}

        //public static FrayteStatus ExchangeRateHistoryStatus()
        //{
        //    string hittingURL = "ExchangeHistoryUpdateStatus";
        //    string status = ProcessWebApi.ExecuteExchangeRateHistoryStatus(hittingURL, null);
        //    FrayteStatus frayte = JsonConvert.DeserializeObject<FrayteStatus>(status);
        //    return frayte;
        //}
    }
}
