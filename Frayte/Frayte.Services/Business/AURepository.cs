using Frayte.Services.Models;
using Frayte.Services.Models.ApiModal;
using Frayte.Services.Models.AU;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Xml.Linq;

namespace Frayte.Services.Business
{
    public class AURepository
    {
        public AUResponseModel CreateShipment(AURequestModel auRequest)
        {
            AUResponseModel respone = new AUResponseModel();

            string result = string.Empty;

            respone.Status = WebApiCalling(auRequest, ref respone);
            respone.Quantity = auRequest.Quantity;
            respone.InvoiceNumber = auRequest.InvoiceNumber;
            return respone;

        }
        private bool WebApiCalling(AURequestModel auRequest, ref AUResponseModel respone)
        {
            bool status = false;

            string qeury = "Client_Id=" + auRequest.ClientId.Trim() + "&Customer_Id=" + auRequest.CustomerId.Trim() +
                         "&Warehouse_Id=" + auRequest.WarehouseId.Trim() + "&User_Id=" + auRequest.UserId.Trim() +
                         "&user_name=" + auRequest.UserName.Trim() + "&invoice_number=" + auRequest.InvoiceNumber.Trim() +
                         "&delname=" + auRequest.ShipTo.Name.Trim() + "&deladdr1=" + auRequest.ShipTo.Address1.Trim() +
                         "&deladdr2=" + auRequest.ShipTo.Address2 + "&deladdr3=" + auRequest.ShipTo.City +
                         "&deladdr4=" + auRequest.ShipTo.State.Trim() + "&State=" + auRequest.ShipTo.State.Trim() + "&postcode=" + auRequest.ShipTo.PostalCode.Trim() + "&delPhone=" + auRequest.ShipTo.Phone.Trim() +
                         "&delemail=" + (string.IsNullOrWhiteSpace(auRequest.ShipTo.Email) ? "''" : auRequest.ShipTo.Email) + "&product_description=" + auRequest.ProductDescription.Trim() +
                         "&shipped_quantity=" + Convert.ToInt32(auRequest.Quantity) + "&value=" + auRequest.Value + "&weight=" + auRequest.Weight +
                         "&shipped_date=" + auRequest.CollectionDate.Trim() + "&Currency=" + auRequest.Currency.Trim() +
                         "&ShipperName1=" + auRequest.ShipFrom.Name.Trim() + "&ShipperAddr1=" + auRequest.ShipFrom.Address1.Trim() +
                         "&ShipperAddr2=" + (string.IsNullOrWhiteSpace(auRequest.ShipTo.Address2.Trim()) ? "''" : auRequest.ShipTo.Address2.Trim()) + "&ShipperAddr3=" + auRequest.ShipFrom.City.Trim() +
                         "&ShipperCity=" + auRequest.ShipFrom.City.Trim() + "&ShipperState=" + (string.IsNullOrWhiteSpace(auRequest.ShipFrom.State.Trim()) ? auRequest.ShipFrom.City.Trim() : auRequest.ShipFrom.State.Trim()) +
                         "&ShipperPostcode=" + auRequest.ShipFrom.PostalCode + "&ShipperCountry=" + auRequest.ShipFrom.Country +
                         "&DestinationCountry=" + auRequest.DestinationCountry + "&Carriertype=" + auRequest.CarrierType;
            string url = "http://eretail.amservices.net.au:82/api/PDFLabelV2?" + qeury;

            var uri = new Uri(url);
            WebClient client = new WebClient();
            client.UseDefaultCredentials = true;
            client.Credentials = CredentialCache.DefaultCredentials;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            SslProtocols _Tls12 = (SslProtocols)0x00000C00;
            SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
            ServicePointManager.SecurityProtocol = Tls12;
            //lable 
            string Image = string.Empty;
            string labelName = string.Empty;
            labelName = FrayteShortName.AU;

            // Create a file to write to.



            Image = labelName + "_" + auRequest.InvoiceNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + "1" + " of " + "1" + ")" + ".pdf";

            var targeturl = AppSettings.UploadFolderPath + "/Shipments/" + auRequest.DirectShipmentDraftid + "/";
            //end
            if (System.IO.Directory.Exists(targeturl))
            {
                if (AppSettings.ShipmentCreatedFrom == "BATCH")
                {
                    client.DownloadFile(uri, targeturl + Image);
                    status = File.Exists(targeturl + Image);
                    respone.Status = status;
                    respone.TempDownload_Folder = targeturl;
                    respone.TempDownlaodLabelName = Image;

                }
                else
                {

                    client.DownloadFile(uri, targeturl + Image);
                    status = File.Exists(targeturl + Image);
                    respone.Status = status;
                    respone.TempDownload_Folder = targeturl;
                    respone.TempDownlaodLabelName = Image;
                }

            }
            else
            {
                if (AppSettings.ShipmentCreatedFrom == "BATCH")
                {
                    System.IO.Directory.CreateDirectory(targeturl);
                    client.DownloadFile(uri, targeturl + Image);
                    status = File.Exists(targeturl + Image);
                    respone.Status = status;
                    respone.TempDownload_Folder = targeturl;
                    respone.TempDownlaodLabelName = Image;
                }
                else
                {

                    System.IO.Directory.CreateDirectory(targeturl);

                     client.DownloadFile(uri, targeturl + Image);

                    status = File.Exists(targeturl + Image);
                    respone.Status = status;
                    respone.TempDownload_Folder = targeturl;
                    respone.TempDownlaodLabelName = Image;
                }
            }

            return status;
        }
        public AURequestModel MapDirectBookingDetailToShipmentRequestDto(DirectBookingShipmentDraftDetail directBookingDetail)
        {
            try
            {
                AURequestModel au = new AURequestModel();
                //Address
                au.ShipFrom = MappShipFrom(directBookingDetail.ShipFrom);
                au.ShipTo = MappShipTo(directBookingDetail.ShipTo);
                var referance = directBookingDetail.FrayteNumber + "-" + directBookingDetail.ReferenceDetail.Reference1;

                var ProductDescription = string.Join("-", directBookingDetail.Packages.Select(x => x.Content.ToString()).ToArray());
                var productDescriptionWithSpecialInstrucation = ProductDescription + " SPI " + referance;
                //Addition 
                au.ClientId = "232";
                au.CustomerId = "197";
                au.WarehouseId = "221";
                au.UserId = directBookingDetail.CustomerRateCard.CourierAccountNo;
                au.UserName = "Frayte";
                au.DirectShipmentDraftid = directBookingDetail.DirectShipmentDraftId;
                au.InvoiceNumber = directBookingDetail.FrayteNumber;
                au.ProductDescription = productDescriptionWithSpecialInstrucation;
                au.Quantity = directBookingDetail.Packages.Sum(k => k.CartoonValue);
                au.Value = directBookingDetail.Packages.Sum(k => k.Value);
                au.Weight = directBookingDetail.CustomerRateCard.Weight;
                au.CollectionDate = directBookingDetail.ReferenceDetail.CollectionDate.Value.ToString("MM/dd/yyyy");
                au.Currency = directBookingDetail.Currency.CurrencyCode;
                au.DestinationCountry = "AUS";
                au.CarrierType = "TOLLCARTONS";

                return au;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Shipment is not created " + ex.Message));
                return null;
            }
        }

        private ShipFromModel MappShipFrom(DirectBookingDraftCollection collection)
        {
            ShipFromModel shipfrom = new ShipFromModel();
            shipfrom.Name = collection.FirstName + collection.LastName;
            shipfrom.Address1 = collection.Address;
            shipfrom.Address2 = string.IsNullOrWhiteSpace(collection.Address2.Trim()) ? "" : collection.Address2.Trim();
            shipfrom.Country = collection.Country.Code2;
            shipfrom.City = collection.City;
            shipfrom.State = collection.State;
            shipfrom.PostalCode = collection.PostCode;
            shipfrom.Phone = collection.Phone;
            shipfrom.Email = collection.Email;
            return shipfrom;
        }

        private ShipToModel MappShipTo(DirectBookingDraftCollection collection)
        {
            ShipToModel shipTo = new ShipToModel();
            shipTo.Name = collection.FirstName + collection.LastName;
            shipTo.Address1 = collection.Address;
            shipTo.Address2 = string.IsNullOrWhiteSpace(collection.Address2.Trim()) ? "" : collection.Address2.Trim();

            shipTo.City = collection.City;
            shipTo.State = collection.State;
            shipTo.PostalCode = collection.PostCode;
            shipTo.Country = collection.Country.Code2;
            shipTo.Phone = collection.Phone;
            shipTo.Email = collection.Email;
            return shipTo;
        }
        public IntegrtaionResult MapAUIntegrationResponse(AUResponseModel auResponse)
        {
            IntegrtaionResult integrtaionResult = new IntegrtaionResult();

            if (auResponse != null)
            {
                integrtaionResult.Status = auResponse.Status;
                integrtaionResult.CourierName = FrayteCourierCompany.AU;
                integrtaionResult.TrackingNumber = auResponse.InvoiceNumber;
                integrtaionResult.PickupRef = null;
                integrtaionResult.PieceTrackingDetails = new List<CourierPieceDetail>();

                for (int i = 0; i < auResponse.Quantity; i++)
                {
                    string PieceTrackingNumber = string.Concat(auResponse.InvoiceNumber, i.ToString());
                    CourierPieceDetail obj = new CourierPieceDetail();
                    obj.DirectShipmentDetailId = 0;
                    obj.PieceTrackingNumber = PieceTrackingNumber;
                    obj.ImageUrl = auResponse.TempDownload_Folder;
                    obj.LabelName = auResponse.TempDownlaodLabelName;
                    integrtaionResult.PieceTrackingDetails.Add(obj);
                }


            }
            else
            {
                integrtaionResult.Error = new FratyteError();
                integrtaionResult.Error.Miscellaneous = new List<string>();
                integrtaionResult.Error.Miscellaneous.Add(auResponse.Error);

                integrtaionResult.ErrorCode = new List<FrayteApiError>();
                {
                    List<FrayteApiError> _api = new FrayteApiErrorCodeRepository().SaveFinilizeApiError(integrtaionResult.Error, FrayteLogisticServiceType.DPD, null);
                    integrtaionResult.ErrorCode = _api;
                }
                integrtaionResult.Status = false;
            }

            return integrtaionResult;
        }

        public void MappingCourierPieceDetail(IntegrtaionResult integrtaionResult, DirectBookingShipmentDraftDetail directBookingDetail, int DirectShipmentid)
        {
            if (DirectShipmentid > 0)
            {
                int k = 0, i = 0;
                List<int> _shiId = new List<int>();

                _shiId = new DirectShipmentRepository().GetDirectShipmentDetailID(DirectShipmentid);
                for (i = 0; i < integrtaionResult.PieceTrackingDetails.Count(); i++)
                {
                    integrtaionResult.PieceTrackingDetails[i].DirectShipmentDetailId = _shiId[0];
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


        public string DownloadAUPDF(CourierPieceDetail pieceDetails, int totalPiece, int count, int DirectShipmentid)
        {
            string Image = string.Empty;


            if (Image != null)
            {

                string labelName = string.Empty;
                labelName = FrayteShortName.AU;
                Image = pieceDetails.LabelName;
                // Create a file to write to.

                if (AppSettings.LabelSave == "")
                {
                    if (System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentid + "/"))
                    {

                    }
                    else
                    {

                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(AppSettings.LabelFolder + "/" + DirectShipmentid + "/"))
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {

                        }
                        else
                        {

                        }

                    }
                    else
                    {
                        // labelimage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "/" + DirectShipmentid);
                        }

                        else
                        {

                            System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid));
                            if (System.IO.Directory.Exists(pieceDetails.ImageUrl))
                            {
                                string[] files = System.IO.Directory.GetFiles(pieceDetails.ImageUrl);

                                // Copy the files and overwrite destination files if they already exist.
                                foreach (string s in files)
                                {
                                    // Use static Path methods to extract only the file name from the path.

                                    var destFile = System.IO.Path.Combine(HostingEnvironment.MapPath(AppSettings.LabelFolder + "/" + DirectShipmentid), pieceDetails.LabelName);
                                    System.IO.File.Copy(s, destFile, true);

                                    System.IO.Directory.Delete(pieceDetails.ImageUrl, true);
                                }
                            }

                        }
                    }
                }


            }
            return Image;


        }

        public string AUTrackingWebApiCalling(string Invoicenumber)
        {
            string result = string.Empty;
            string url = "http://eretail.amservices.net.au:82/api/ETracking?trackdetails=" + Invoicenumber;

            //

            WebClient client = new WebClient();
            client.UseDefaultCredentials = true;
            client.Credentials = CredentialCache.DefaultCredentials;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            SslProtocols _Tls12 = (SslProtocols)0x00000C00;
            SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
            ServicePointManager.SecurityProtocol = Tls12;

            result = client.DownloadString(url);

            return result;
        }


    }
}
