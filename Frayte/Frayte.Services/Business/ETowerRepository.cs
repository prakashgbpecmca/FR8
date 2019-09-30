using Frayte.Services.Models;
using Frayte.Services.Models.ETower;
using Frayte.Services.Models.Express;
using Frayte.Services.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Web.Hosting;

namespace Frayte.Services.Business
{
    public class ETowerRepository
    {
        public EtowerResponseModel CreateShipment(List<ETowerRequestModel> request, int DirectShipmentDraftId)
        {
            EtowerResponseModel response = new EtowerResponseModel();
            var logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.ETOWER);
            string url = logisticIntegration.ServiceUrl + "/services/shipper/orders";
            var shipmentRequestjson = JsonConvert.SerializeObject(request);
            var result = ETowerApiCalling(url, shipmentRequestjson, "Shipment");
            response = Newtonsoft.Json.JsonConvert.DeserializeObject<EtowerResponseModel>(result);
            response.Request = shipmentRequestjson;
            response.Response = result;
            if (response.status == "Success")
            {
                LabelResponseModel label = new LabelResponseModel();

                #region Label Dwonload

                var eTowerLableRequest = new EtowerLableRequest()
                {
                    labelFormat = "PDF",
                    labelType = 1,
                    merged = false,
                    packinglist = true,
                };
                eTowerLableRequest.orderIds = new List<string>();
                eTowerLableRequest.orderIds.Add(response.data[0].orderId);
                var labeljson = JsonConvert.SerializeObject(eTowerLableRequest);
                string Labelurl = logisticIntegration.ServiceUrl + "/services/shipper/labels";
                var lableResponse = ETowerApiCalling(Labelurl, labeljson, "Label");
                label = JsonConvert.DeserializeObject<LabelResponseModel>(lableResponse);

                if (label.status == "Success")
                {
                    response.data[0].labelContent = label.data[0].labelContent;
                }
                else
                {
                    //Label Error
                }

                #endregion
            }
            else
            {
                var EtowerError = JsonConvert.DeserializeObject<EtowerError>(result);
            }
            return response;
        }

        public string ETowerApiCalling(string url, string shipmentRequestjson, string type)
        {
            try
            {
                string response = string.Empty;
                WebClient client = new WebClient();
                client.UseDefaultCredentials = true;
                client.Credentials = CredentialCache.DefaultCredentials;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                ServicePointManager.SecurityProtocol = Tls12;

                #region DateHeader

                var CaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Canada Central Standard Time");

                var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
                var dateTimeZone = TimeZoneInfo.ConvertTimeToUtc(date, TimeZoneInfo.Local);
                var gmtTime = dateTimeZone.ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'");
                string Authorization = string.Empty;

                #endregion

                #region Authorization

                var key = Encoding.UTF8.GetBytes("zjz4BKuTZ5spTY62QQtKyQ");
                if (type == "Shipment")
                {
                    var shipUrl = "POST\n" + gmtTime + "\nhttp://www.etowertech.com/services/shipper/orders";

                    Authorization = HMACSHA1("zjz4BKuTZ5spTY62QQtKyQ", shipUrl);
                }
                else if (type == "Label")
                {
                    Authorization = string.Empty;
                    var labelUrl = "POST\n" + gmtTime + "\nhttp://www.etowertech.com/services/shipper/labels";
                    Authorization = HMACSHA1("zjz4BKuTZ5spTY62QQtKyQ", labelUrl);
                }

                #endregion

                //Wed, 05 Dec 2018 06:05:58 GMT
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers["X-WallTech-Date"] = gmtTime;
                client.Headers["Authorization"] = "WallTech pclKaPYZz9Ie4N6QFpnnU5:" + Authorization.Trim();
                response = client.UploadString(url, "POST", shipmentRequestjson);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string HMACSHA1(string key, string url)
        {
            //Step 1: Converting the secret key into UTF8 bytes
            Byte[] secretBytes = UTF8Encoding.UTF8.GetBytes(key);

            //Step 2: Applying HMACSHA1 on UTF8 generated bytes
            HMACSHA1 hmac = new HMACSHA1(secretBytes);

            //Step 3: Converting date into UTF8 bytes
            //Byte[] dataBytes = UTF8Encoding.UTF8.GetBytes(date);
            Byte[] dataBytes = UTF8Encoding.UTF8.GetBytes(url);

            //Step 4: Computing the date UTF8 bytes with HMACSHA1
            Byte[] calcHash = hmac.ComputeHash(dataBytes);

            //Step 5: Converting result into Base 64 string.
            String calcHashString = Convert.ToBase64String(calcHash);

            //oHo2uIOfOKLfDf5flI3BRb8mOEo=
            return calcHashString;
        }

        public List<ETowerRequestModel> MapDirectBookingDetailToShipmentRequestDto(DirectBookingShipmentDraftDetail directBookingDetail)
        {
            List<ETowerRequestModel> eTowerRequest = new List<ETowerRequestModel>();

            ETowerRequestModel etower = new ETowerRequestModel()
            {
                trackingNo = "",
                referenceNo = directBookingDetail.FrayteNumber,
                addressLine1 = directBookingDetail.ShipTo.Address,
                addressLine2 = directBookingDetail.ShipTo.Address2,
                addressLine3 = "",
                city = directBookingDetail.ShipTo.City,
                country = directBookingDetail.ShipTo.Country.Code2,
                description = string.Join("-", directBookingDetail.Packages.Select(x => x.Content.ToString()).ToArray()),
                nativeDescription = "",
                email = directBookingDetail.ShipTo.Email,
                facility = "",
                instruction = "Canada Post",
                invoiceCurrency = directBookingDetail.Currency.CurrencyCode,
                invoiceValue = directBookingDetail.Packages.Sum(k => k.Value),
                phone = directBookingDetail.ShipTo.Phone,
                platform = "",
                postcode = directBookingDetail.ShipTo.PostCode,
                recipientCompany = directBookingDetail.ShipTo.CompanyName,
                recipientName = directBookingDetail.ShipTo.FirstName + " " + directBookingDetail.ShipTo.LastName,
                serviceCode = "",
                serviceOption = "Expedited",
                sku = "",
                state = directBookingDetail.ShipTo.State,
                weightUnit = "KG",
                weight = directBookingDetail.CustomerRateCard.Weight,
                dimensionUnit = "CM",
                length = directBookingDetail.Packages[0].Length,
                width = directBookingDetail.Packages[0].Width,
                height = directBookingDetail.Packages[0].Height,
                volume = 0,
                shipperName = directBookingDetail.ShipFrom.FirstName + " " + directBookingDetail.ShipFrom.LastName,
                shipperAddressLine1 = directBookingDetail.ShipFrom.Address,
                shipperAddressLine2 = directBookingDetail.ShipFrom.Address2,
                shipperAddressLine3 = "",
                shipperCity = directBookingDetail.ShipFrom.City,
                shipperState = directBookingDetail.ShipFrom.State,
                shipperPostcode = directBookingDetail.ShipFrom.PostCode,
                shipperCountry = directBookingDetail.ShipFrom.Country.Code2,
                shipperPhone = directBookingDetail.ShipFrom.Phone,
                recipientTaxId = "",
                authorityToLeave = "",
                incoterm = "DDP",
                lockerService = "",
                extendData = new ExtendData
                {
                    nationalNumber = "",
                    nationalIssueDate = "",
                    cyrillicName = "",
                    imei = "",
                    isImei = true,
                    vendorid = "",
                    gstexemptioncode = "",
                    abnnumber = "",
                    sortCode = "",
                },
            };
            etower.orderItems = new List<OrderItem>();
            var count = 1;
            for (int i = 0; i < directBookingDetail.Packages.Count; i++)
            {
                OrderItem order = new OrderItem()
                {
                    itemNo = count.ToString(),
                    sku = directBookingDetail.FrayteNumber + "*" + count + "-" + directBookingDetail.Packages.Count,
                    description = directBookingDetail.Packages[i].Content,
                    nativeDescription = directBookingDetail.FrayteNumber + "-" + directBookingDetail.ReferenceDetail.Reference1,
                    hsCode = directBookingDetail.FrayteNumber,
                    originCountry = directBookingDetail.ShipFrom.Country.Name,
                    unitValue = (directBookingDetail.Packages[i].Value / directBookingDetail.Packages[i].CartoonValue),
                    itemCount = directBookingDetail.Packages[i].CartoonValue,
                    weight = directBookingDetail.Packages[i].Weight,
                    productURL = ""
                };
                count++;
                etower.orderItems.Add(order);
            }
            eTowerRequest.Add(etower);
            return eTowerRequest;
        }

        public List<ETowerRequestModel> MapExpressBookingDetailToShipmentRequestDto(ExpressShipmentModel expressBookingDetail)
        {
            List<ETowerRequestModel> eTowerRequest = new List<ETowerRequestModel>();

            ETowerRequestModel etower = new ETowerRequestModel()
            {
                trackingNo = "",
                referenceNo = expressBookingDetail.AWBNumber.Replace(" ", ""),
                addressLine1 = expressBookingDetail.ShipTo.Address,
                addressLine2 = expressBookingDetail.ShipTo.Address2,
                addressLine3 = "",
                city = expressBookingDetail.ShipTo.City,
                country = expressBookingDetail.ShipTo.Country.Code2,

                description = string.Join("-", expressBookingDetail.Packages.Select(x => x.Content.ToString()).ToArray()),
                nativeDescription = "",
                email = expressBookingDetail.ShipTo.Email,
                facility = "",
                instruction = "Canada Post",
                invoiceCurrency = expressBookingDetail.DeclaredCurrency.CurrencyCode,
                invoiceValue = expressBookingDetail.Packages.Sum(k => k.Value),
                phone = expressBookingDetail.ShipTo.Phone,
                platform = "",
                postcode = expressBookingDetail.ShipTo.PostCode,
                recipientCompany = expressBookingDetail.ShipTo.CompanyName,
                recipientName = expressBookingDetail.ShipTo.FirstName + " " + expressBookingDetail.ShipTo.LastName,
                serviceCode = "",
                serviceOption = "Expedited",
                sku = "",
                state = expressBookingDetail.ShipTo.State,
                weightUnit = "KG",
                weight = expressBookingDetail.Service.BillingWeight,
                dimensionUnit = "CM",
                length = expressBookingDetail.Packages[0].Length,
                width = expressBookingDetail.Packages[0].Width,
                height = expressBookingDetail.Packages[0].Height,
                volume = 0,
                shipperName = expressBookingDetail.ShipFrom.FirstName + " " + expressBookingDetail.ShipFrom.LastName,
                shipperAddressLine1 = expressBookingDetail.ShipFrom.Address,
                shipperAddressLine2 = expressBookingDetail.ShipFrom.Address2,
                shipperAddressLine3 = "",
                shipperCity = expressBookingDetail.ShipFrom.City,
                shipperState = expressBookingDetail.ShipFrom.State,
                shipperPostcode = expressBookingDetail.ShipFrom.PostCode,
                shipperCountry = expressBookingDetail.ShipFrom.Country.Code2,
                shipperPhone = expressBookingDetail.ShipFrom.Phone,
                recipientTaxId = "",
                authorityToLeave = "",
                incoterm = "DDP",
                lockerService = "",
                extendData = new ExtendData
                {
                    nationalNumber = "",
                    nationalIssueDate = "",
                    cyrillicName = "",
                    imei = "",
                    isImei = true,
                    vendorid = "",
                    gstexemptioncode = "",
                    abnnumber = "",
                    sortCode = "",
                },
            };
            etower.orderItems = new List<OrderItem>();
            var count = 1;
            for (int i = 0; i < expressBookingDetail.Packages.Count; i++)
            {
                OrderItem order = new OrderItem()
                {
                    itemNo = count.ToString(),
                    sku = expressBookingDetail.AWBNumber.Replace(" ", "") + "*" + count + "-" + expressBookingDetail.Packages.Count,
                    description = expressBookingDetail.Packages[i].Content,
                    nativeDescription = expressBookingDetail.FrayteNumber + "-" + expressBookingDetail.ShipmentReference,
                    hsCode = expressBookingDetail.AWBNumber.Replace(" ", ""),
                    originCountry = expressBookingDetail.ShipFrom.Country.Name,
                    unitValue = (expressBookingDetail.Packages[i].Value / expressBookingDetail.Packages[i].CartonValue),
                    itemCount = expressBookingDetail.Packages[i].CartonValue,
                    weight = expressBookingDetail.Packages[i].Weight,
                    productURL = ""
                };
                count++;
                etower.orderItems.Add(order);
            }
            eTowerRequest.Add(etower);
            return eTowerRequest;
        }

        public IntegrtaionResult MapCanadaPostIntegrationResponse(EtowerResponseModel eTowerResponse, List<OrderItem> orderItems)
        {
            IntegrtaionResult integrtaionResult = new IntegrtaionResult();

            if (eTowerResponse.status == "Success")
            {
                integrtaionResult.Status = true;
                integrtaionResult.CourierName = FrayteCourierCompany.CANADAPOST;
                integrtaionResult.TrackingNumber = eTowerResponse.data[0].trackingNo;
                integrtaionResult.PickupRef = null;
                integrtaionResult.PieceTrackingDetails = new List<CourierPieceDetail>();
                int counter = 1;
                for (int i = 0; i < orderItems.Count; i++)
                {

                    CourierPieceDetail obj = new CourierPieceDetail();
                    obj.DirectShipmentDetailId = 0;
                    obj.PieceTrackingNumber = string.Concat(eTowerResponse.data[0].trackingNo, counter);
                    obj.ImageByte = eTowerResponse.data[0].labelContent;
                    integrtaionResult.PieceTrackingDetails.Add(obj);
                    counter++;
                }
            }
            else
            {
                integrtaionResult.Error = new FratyteError();
                integrtaionResult.Error.Service = new List<string>();
                foreach (var item in eTowerResponse.errors)
                {
                    string error = string.Empty;
                    error = item.code + "-" + "" + item.message;
                    integrtaionResult.Error.Service.Add(error);
                }

                integrtaionResult.Status = false;
            }

            return integrtaionResult;
        }

        public void MappingCourierPieceDetail(IntegrtaionResult integrtaionResult, int DirectShipmentid)
        {
            if (DirectShipmentid > 0)
            {

                List<int> _shiId = new List<int>();
                for (int i = 0; i < integrtaionResult.PieceTrackingDetails.Count; i++)
                {
                    _shiId = new DirectShipmentRepository().GetDirectShipmentDetailID(DirectShipmentid);

                    integrtaionResult.PieceTrackingDetails[i].DirectShipmentDetailId = _shiId[i];

                }
            }
        }

        public void MappingExpressCourierPieceDetail(IntegrtaionResult integrtaionResult, int ExpressShipmentid)
        {
            if (ExpressShipmentid > 0)
            {
                List<int> _shiId = new List<int>();
                for (int i = 0; i < integrtaionResult.PieceTrackingDetails.Count; i++)
                {
                    _shiId = new DirectShipmentRepository().GetExpressDirectShipmentDetailID(ExpressShipmentid);
                   
                        integrtaionResult.PieceTrackingDetails[i].DirectShipmentDetailId = _shiId[i];
                       
                }
            }
        }

        public bool SaveTrackingDetail(DirectBookingShipmentDraftDetail directBookingDetail, IntegrtaionResult result, int DirectShipmentid)
        {
            var count = 1;
            foreach (var Obj in result.PieceTrackingDetails)
            {
                Frayte.Services.Models.Package package = new Frayte.Services.Models.Package();
                package.DirectShipmentDetailId = Obj.DirectShipmentDetailId;

                package.LabelName = Obj.PieceTrackingNumber;
                new DirectShipmentRepository().SavePackageDetail(package, "", Obj.DirectShipmentDetailId, directBookingDetail.CustomerRateCard.CourierName, count);
                count++;
            }
            return true;
        }

        public string DownloadCanadaPostBytetoImage(CourierPieceDetail pieceDetails, int totalPiece, int count, int DirectShipmentid)
        {
            string Image = string.Empty;
            if (Image != null)
            {
                string labelName = string.Empty;
                labelName = FrayteCourierCompany.CANADAPOST;

                // Create a file to write to.
                Image = labelName + "_" + pieceDetails.PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".pdf";

                if (AppSettings.LabelSave == "")
                {
                    if (System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/"))
                    {
                        string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image);
                        try
                        {
                            byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);
                            File.WriteAllBytes(path, sPDFDecoded);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/");
                        string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image);
                        try
                        {
                            byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);
                            File.WriteAllBytes(path, sPDFDecoded);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            Console.ReadLine();
                        }
                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(AppSettings.LabelFolder + "/" + DirectShipmentid + "/"))
                    {
                        string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image);
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);

                            File.WriteAllBytes(path, sPDFDecoded);
                        }
                        else
                        {
                            try
                            {
                                byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);

                                File.WriteAllBytes(path, sPDFDecoded);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                                Console.ReadLine();
                            }
                        }
                    }
                    else
                    {
                        string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid + "/" + Image);
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "/" + DirectShipmentid);
                            try
                            {
                                byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);
                                File.WriteAllBytes(path, sPDFDecoded);
                            }
                            catch (Exception e)
                            {

                            }
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid));

                            try
                            {
                                byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);
                                File.WriteAllBytes(path, sPDFDecoded);
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }
                }
            }
            return Image;
        }

        public string DownloadExpressCanadaPostBytetoImage(CourierPieceDetail pieceDetails, int totalPiece, int count, int Expressid)
        {
            string Image = string.Empty;
            if (Image != null)
            {
                string labelName = string.Empty;
                labelName = FrayteCourierCompany.CANADAPOST;

                // Create a file to write to.
                Image = labelName + "_" + pieceDetails.PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".pdf";

                if (AppSettings.LabelSave == "")
                {
                    if (System.IO.Directory.Exists(AppSettings.WebApiPath + "PackageLabel/Express/" + Expressid + "/"))
                    {
                        string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "Express/" + Expressid + "/" + Image);
                        try
                        {
                            byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);
                            File.WriteAllBytes(path, sPDFDecoded);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "PackageLabel/Express/" + Expressid + "/");
                        string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "Express/" + Expressid + "/" + Image);
                        try
                        {
                            byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);
                            File.WriteAllBytes(path, sPDFDecoded);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            Console.ReadLine();
                        }
                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(AppSettings.LabelFolder + "Express/" + Expressid))
                    {
                        string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "Express/" + Expressid + "/" + Image);
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);

                            File.WriteAllBytes(path, sPDFDecoded);
                        }
                        else
                        {
                            try
                            {
                                byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);

                                File.WriteAllBytes(path, sPDFDecoded);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                                Console.ReadLine();
                            }
                        }
                    }
                    else
                    {
                        string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "Express/" + Expressid + "/" + Image);
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "Express/" + Expressid);
                            try
                            {
                                byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);
                                File.WriteAllBytes(path, sPDFDecoded);
                            }
                            catch (Exception e)
                            {

                            }
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "Express/" + Expressid));

                            try
                            {
                                byte[] sPDFDecoded = Convert.FromBase64String(pieceDetails.ImageByte);
                                File.WriteAllBytes(path, sPDFDecoded);
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }
                }
            }
            return Image;
        }
    }
}
