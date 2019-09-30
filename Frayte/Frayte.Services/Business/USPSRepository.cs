using Frayte.Services.Models;
using Frayte.Services.DataAccess;
using Frayte.Services.Models.USPS;
using Frayte.Services.Models.Express;
using Frayte.Services.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace Frayte.Services.Business
{
    public class USPSRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public USPSRequest MapExpressShipmentToUSPSShipmentRequest(ExpressShipmentModel shipment)
        {
            USPSRequest request = new USPSRequest();

            try
            {
                request.header = new USPSHeader();
                request.header.key = "";
                request.header.version = "";

                request.shipment = new USPSShipment();
                request.shipment.date = DateTime.Now.ToString("yyyy-MM-dd");
                request.shipment.labelType = "PNG";
                request.shipment.mailClass = "EX";
                request.shipment.packageType = "LEGAL FLAT RATE ENV";
                request.shipment.weight = shipment.Service.ActualWeight;

                request.shipment.dimensions = new USPSDimensions();
                request.shipment.dimensions.height = shipment.Packages.FirstOrDefault().Height;
                request.shipment.dimensions.length = shipment.Packages.FirstOrDefault().Length;
                request.shipment.dimensions.width = shipment.Packages.FirstOrDefault().Width;

                request.shipment.sender = new USPSSenderAddress();
                request.shipment.sender.address1 = shipment.ShipFrom.Address;
                request.shipment.sender.address2 = shipment.ShipFrom.Address2;
                request.shipment.sender.city = shipment.ShipFrom.City;
                request.shipment.sender.company = shipment.ShipFrom.CompanyName;
                request.shipment.sender.country = shipment.ShipFrom.Country.Code2;
                request.shipment.sender.name = shipment.ShipFrom.FirstName + " " + shipment.ShipFrom.LastName;
                request.shipment.sender.phone = shipment.ShipFrom.Phone;
                request.shipment.sender.state = dbContext.CountryStates.Where(x => x.CountryId == shipment.ShipFrom.Country.CountryId && x.StateName == shipment.ShipFrom.State).FirstOrDefault().StateCode;
                request.shipment.sender.zip = shipment.ShipFrom.PostCode;
                request.shipment.sender.zip4 = "1111";

                request.shipment.receiver = new USPSSenderAddress();
                request.shipment.receiver.address1 = shipment.ShipTo.Address;
                request.shipment.receiver.address2 = shipment.ShipTo.Address2;
                request.shipment.receiver.city = shipment.ShipTo.City;
                request.shipment.receiver.company = shipment.ShipTo.CompanyName;
                request.shipment.receiver.country = shipment.ShipTo.Country.Code2;
                request.shipment.receiver.name = shipment.ShipTo.FirstName + " " + shipment.ShipTo.LastName;
                request.shipment.receiver.state = dbContext.CountryStates.Where(x => x.CountryId == shipment.ShipTo.Country.CountryId && x.StateName == shipment.ShipTo.State).FirstOrDefault().StateCode;
                request.shipment.receiver.zip = shipment.ShipTo.PostCode;
                request.shipment.receiver.zip4 = "1111";

                request.shipment.options = new USPSOptions();
                request.shipment.options.insurance = 0;
                request.shipment.options.signature = "N";

                request.shipment.items = new List<USPSItems>();
                foreach (var item in shipment.Packages)
                {
                    if (item.CartonValue > 0)
                    {
                        for (int i = 0; i < item.CartonValue; i++)
                        {
                            USPSItems piece = new USPSItems();
                            if (shipment.PakageCalculatonType.ToUpper() == FraytePakageCalculationType.kgtoCms.ToUpper())
                            {
                                piece.description = item.Content;
                                piece.quantity = item.CartonValue;
                                piece.unitValue = item.Value;
                                piece.weight = (shipment.Packages[i].Weight) * 35.27396195M;
                                piece.weightUnit = "oz";
                            }
                            else if (shipment.PakageCalculatonType.ToUpper() == FraytePakageCalculationType.LbToInchs.ToUpper())
                            {
                                piece.description = item.Content;
                                piece.quantity = item.CartonValue;
                                piece.unitValue = item.Value;
                                piece.weight = (shipment.Packages[i].Weight) * 16;
                                piece.weightUnit = "oz";
                            }
                            request.shipment.items.Add(piece);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return request;
        }

        public USPSResponse CreateShipment(USPSRequest request, int ExpressId)
        {
            USPSResponse response = new USPSResponse();

            FrayteLogisticIntegration logisticIntegration = UtilityRepository.getLogisticIntegration(UtilityRepository.GetOperationZone().OperationZoneId, AppSettings.ApplicationMode, FrayteIntegration.USPS);

            string res = string.Empty;

            request.header.key = logisticIntegration.InetgrationKey;
            request.header.version = logisticIntegration.AppVersion;

            var usps = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            res = GetResponse(logisticIntegration, usps);
            response = JsonConvert.DeserializeObject<USPSResponse>(res);
            var error = JsonConvert.DeserializeObject<USPSError>(res);            
            return response;
        }

        public string GetResponse(FrayteLogisticIntegration logisticIntegration, string json)
        {
            try
            {
                string url = string.Empty;
                string response = string.Empty;

                WebClient client = new WebClient();
                client.UseDefaultCredentials = true;
                client.Credentials = CredentialCache.DefaultCredentials;

                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                response = client.UploadString(logisticIntegration.ServiceUrl, "POST", json);
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IntegrtaionResult MapUSPSIntegrationResponse(USPSResponse response)
        {
            IntegrtaionResult integrtaionResult = new IntegrtaionResult();

            if (response.trackingNumber != null)
            {
                integrtaionResult.Status = true;
                integrtaionResult.CourierName = FrayteCourierCompany.USPS;
                integrtaionResult.TrackingNumber = response.trackingNumber;
                integrtaionResult.PickupRef = null;
                integrtaionResult.PieceTrackingDetails = new List<CourierPieceDetail>();

                CourierPieceDetail obj = new CourierPieceDetail();
                obj.DirectShipmentDetailId = 0;
                obj.PieceTrackingNumber = response.trackingNumber;
                obj.ImageUrl = response.labelImage;
                integrtaionResult.PieceTrackingDetails.Add(obj);
                return integrtaionResult;
            }
            else
            {
                integrtaionResult.Status = false;
                integrtaionResult.Error = new FratyteError();                
                integrtaionResult.Error.IsMailSend = true;
                integrtaionResult.Error.Custom = new List<string>();
                integrtaionResult.Error.Address = new List<string>();
                integrtaionResult.Error.Package = new List<string>();
                integrtaionResult.Error.Service = new List<string>();
                integrtaionResult.Error.ServiceError = new List<string>();
                integrtaionResult.Error.Miscellaneous = new List<string>();
                integrtaionResult.Error.Miscellaneous.Add(response.error);
                return integrtaionResult;
            }
        }

        public void MappingExpressCourierPieceDetail(IntegrtaionResult integrtaionResult, ExpressShipmentModel expessBookingDetail, int ExpressShipmentid)
        {
            if (ExpressShipmentid > 0)
            {
                int k = 0, i = 0;
                List<int> _shiId = new List<int>();
                foreach (var Obj in expessBookingDetail.Packages)
                {
                    _shiId = new DirectShipmentRepository().GetExpressDirectShipmentDetailID(ExpressShipmentid);
                    for (int j = 1; j <= Obj.CartonValue; j++)
                    {
                        integrtaionResult.PieceTrackingDetails[k].DirectShipmentDetailId = _shiId[i];
                        k++;
                    }
                    i++;
                }
            }
        }

        public string DownloadExpressUSPSImage(CourierPieceDetail pieceDetails, int totalPiece, int count, int ExpressShipmentid)
        {
            string Imagename = string.Empty;

            if (pieceDetails.ImageUrl != null)
            {
                string labelName = string.Empty;
                labelName = FrayteShortName.USPS;

                // Create a file to write to.
                Imagename = labelName + "_" + pieceDetails.PieceTrackingNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + count + " of " + totalPiece + ")" + ".png";
                if (AppSettings.LabelSave == "")
                {
                    if (System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressShipmentid + "/"))
                    {
                        File.WriteAllText(AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressShipmentid + "/" + Imagename, pieceDetails.ImageUrl);
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressShipmentid + "/");
                        File.WriteAllText(AppSettings.WebApiPath + "/PackageLabel/Express/" + ExpressShipmentid + "/" + Imagename, pieceDetails.ImageUrl);
                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(AppSettings.LabelFolder + "/Express/" + ExpressShipmentid + "/"))
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            File.WriteAllText(AppSettings.LabelFolder + "/Express/" + ExpressShipmentid + "/" + Imagename, pieceDetails.ImageUrl);
                        }
                        else
                        {
                            string path = HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + ExpressShipmentid + "/" + Imagename);
                            File.WriteAllText(path, pieceDetails.ImageUrl);
                        }
                    }
                    else
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "/Express/" + ExpressShipmentid);
                            File.WriteAllText(AppSettings.LabelFolder + "/Express/" + ExpressShipmentid + "/" + Imagename, pieceDetails.ImageUrl);
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/Express/" + ExpressShipmentid));
                            string path = HttpContext.Current.Server.MapPath("~/PackageLabel/Express/" + ExpressShipmentid + "/" + Imagename);
                            try
                            {
                                bool status = false;
                                byte[] bytes = Convert.FromBase64String(pieceDetails.ImageUrl);
                                Image image;
                                using (MemoryStream ms = new MemoryStream(bytes))
                                {
                                    image = Image.FromStream(ms);
                                    System.Drawing.Bitmap bmpUploadedImage = new System.Drawing.Bitmap(image);
                                    System.Drawing.Image thumbnailImage = bmpUploadedImage.GetThumbnailImage(812, 1624, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
                                    thumbnailImage.Save(@"" + path);
                                }
                                status = File.Exists(path);
                                if (status)
                                {
                                    return Imagename;
                                }
                                else
                                {
                                    Imagename = string.Empty;
                                }
                            }
                            catch (Exception ex)
                            {
                                Imagename = string.Empty;
                            }
                        }
                    }
                }
            }
            return Imagename;
        }

        internal bool ThumbnailCallback()
        {
            return true;
        }
    }

}
