using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Text;
using System.Web.Http;
using Report.Generator.ManifestReport;
using System.Net.Http.Headers;
using Elmah;
using Frayte.Services.Utility;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class CustomerController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GetCourierType()
        {
            List<FrayteShipmentMethod> lstShipmentMethods = new CourierRepository().GetShipmentMethods("Courier");
            return this.Request.CreateResponse(
                    HttpStatusCode.OK,
                    new
                    {
                        ShipmentMethods = lstShipmentMethods
                    });
        }

        [Authorize(Roles = "Admin, Staff, Customer")]
        [HttpGet]
        public List<FrayteUser> GetCustomerList(int UserId)
        {
            List<FrayteUser> users = new List<FrayteUser>();
            users = new FrayteUserRepository().GetCustomerTypeList((int)FrayteUserRole.Customer, (int)FrayteAddressType.MainAddress, UserId);
            return users;
        }

        [HttpGet]
        public FrayteCustomer GetCustomerDetail(int customerId)
        {
            FrayteCustomer customerDetail = new FrayteCustomer();
            customerDetail = new CustomerRepository().GetCustomerDetail(customerId);
            return customerDetail;
        }

        [HttpGet]
        public HttpResponseMessage GetCustomerCompanyDetail(int UserId)
        {
            bool detail = new CustomerRepository().CustomerCompanyDetail(UserId);
            return this.Request.CreateResponse(HttpStatusCode.OK, detail);
        }

        [HttpPost]
        public IHttpActionResult CustomerAdminAction(FrayteAdminAction actionDetail)
        {
            new ShipmentEmailRepository().SendEmailOperationStaff(actionDetail);
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult SaveCustomerMarginCost(List<FrayteCustomerMarginCost> customerMarginCost)
        {
            FrayteResult result = new FrayteResult();
            if (customerMarginCost != null)
            {
                result = new CustomerRepository().SaveCustomerMarginCost(customerMarginCost);
            }
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult SaveCustomerAdvanceMarginCost(List<FrayteCustomerAdvanceRateCard> advanceMarginCost)
        {
            FrayteResult result = new FrayteResult();
            if (advanceMarginCost != null)
            {
                result = new CustomerRepository().SaveCustomerAdvanceMarginCost(advanceMarginCost);
            }
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult SaveTrackingConfiguration(FrayteTrackingConfiguration trackingConfiguration)
        {
            var result = new CustomerRepository().SaveTrackingConfiguration(trackingConfiguration);
            return Ok(result);
        }

        [HttpGet]
        public FrayteTrackingConfiguration GetTrackingConfiguration(int customerId)
        {
            return new CustomerRepository().GetTrackingConfiguration(customerId);
        }

        [HttpPost]
        public IHttpActionResult SaveCustomerRateCardDetail(FrayteCustomerRateCard frayteCustomerRateCard)
        {
            FrayteResult result = new FrayteResult();
            if (frayteCustomerRateCard != null)
            {
                result = new CustomerRepository().SaveCustomerRateCardDetail(frayteCustomerRateCard);
            }
            return Ok(result);
        }

        [HttpGet]
        public FrayteCustomerRateCard GetCustomerRateCardDetail(int UserId)
        {
            FrayteCustomerRateCard customerRateCardDeatil;
            try
            {
                customerRateCardDeatil = new CustomerRepository().GetCustomerRateCardDetail(UserId);
            }
            catch (Exception ex)
            {
                customerRateCardDeatil = null;
            }

            return customerRateCardDeatil;
        }

        [HttpGet]
        public List<FrayteLogisticServiceItem> GetBusinessUnitLogisticServices(int OperationZoneId, int RoleId, int CreatedBy)
        {
            List<FrayteLogisticServiceItem> item = new List<FrayteLogisticServiceItem>();
            item = new CustomerRepository().BusinessUnitLogisticServiceItems(OperationZoneId, RoleId, CreatedBy);
            return item;
        }

        [HttpGet]
        public FrayteResult RemoveCustomerLogistic(int logisticServiceId, int userId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                result = new CustomerRepository().RemoveCustomerLogistic(logisticServiceId, userId);
                return result;
            }
            catch (Exception ex)
            {
                result.Status = false;
                return result;
            }
        }

        [HttpGet]
        public List<FrayteCustomerMarginCost> GetCustomerMargin(int UserId, int OperationZoneId, string CourierCompany, string ModuleType)
        {
            List<FrayteCustomerMarginCost> result = new CustomerRepository().GetCustomerMarginCost(UserId, OperationZoneId, CourierCompany, ModuleType);
            return result;
        }

        [HttpGet]
        public List<FrayteCustomerAdvanceMarginCost> GetCustomerAdvanceMarginCost(int OperationZoneId, int CustomerId, string LogisticCompany, string LogisticType, string RateType, string PackageType, string ParcelType, string DocType, string ModuleType)
        {
            List<FrayteCustomerAdvanceMarginCost> result = null;
            if (LogisticType == FrayteLogisticType.UKShipment)
            {
                result = new CustomerRepository().CustomerUKAdvanceMarginCost(OperationZoneId, CustomerId, LogisticCompany, LogisticType, RateType, PackageType, ParcelType, ModuleType);
            }
            else if (LogisticType == FrayteLogisticType.EUExport || LogisticType == FrayteLogisticType.EUImport)
            {
                result = new CustomerRepository().CustomerEUAdvanceMarginCost(OperationZoneId, CustomerId, LogisticCompany, LogisticType, RateType, ModuleType);
            }
            else if (LogisticType == FrayteLogisticType.Import ||
                     LogisticType == FrayteLogisticType.Export ||
                     LogisticType == FrayteLogisticType.ThirdParty)
            {
                result = new CustomerRepository().CustomerAdvanceMarginCost(OperationZoneId, CustomerId, LogisticCompany, LogisticType, RateType, DocType, ModuleType);
            }
            return result;
        }

        [HttpPost]
        public IHttpActionResult UploadCustomers()
        {
            List<FrayteCustomer> frayteCustomers = new List<FrayteCustomer>();

            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count > 0)
            {
                HttpFileCollection files = httpRequest.Files;

                HttpPostedFile file = files[0];

                if (!string.IsNullOrEmpty(file.FileName))
                {
                    string connString = "";
                    string filename = DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss_") + file.FileName;
                    string filepath = HttpContext.Current.Server.MapPath("~/UploadFiles/" + filename);

                    file.SaveAs(filepath);

                    connString = new ShipmentRepository().getExcelConnectionString(filename, filepath);

                    OleDbConnection oledbConn = new OleDbConnection(connString);
                    try
                    {
                        oledbConn.Open();
                        OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Sheet1$]", oledbConn);
                        OleDbDataAdapter oleda = new OleDbDataAdapter();
                        oleda.SelectCommand = cmd;
                        DataSet ds = new DataSet();
                        oleda.Fill(ds, "Customers");

                        var exceldata = ds.Tables[0];

                        if (exceldata != null && exceldata.Rows.Count > 0)
                        {
                            if (new CustomerRepository().CheckValidExcel(exceldata))
                            {
                                frayteCustomers = new CustomerRepository().GetAllCustomers(exceldata);

                                foreach (var customer in frayteCustomers)
                                {
                                    FrayteResult result = new CustomerRepository().SaveCustomer(customer);
                                }
                            }
                            else
                            {
                                return BadRequest("Excel file not valid");
                            }
                        }
                        oledbConn.Close();
                        if ((System.IO.File.Exists(filepath)))
                        {
                            System.IO.File.Delete(filepath);
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        oledbConn.Close();
                        if ((System.IO.File.Exists(filepath)))
                        {
                            System.IO.File.Delete(filepath);
                        }
                    }
                }
            }

            return Ok(frayteCustomers);
        }

        [HttpGet]
        public FrayteResult DeleteCustomer(int customerId)
        {
            return new FrayteUserRepository().MarkForDelete(customerId);
        }

        [HttpGet]
        public IHttpActionResult GetCustomerDetailByAccountNumber(string accountNumber)
        {
            CustomerBasicDetail result = new CustomerRepository().GetCustomerDetailByAccountNumber(accountNumber);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult CheckAccountNumber(string accountNumber)
        {
            FrayteResult result = new CustomerRepository().CheckAccountNumber(accountNumber);

            if (result != null && result.Status)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public List<FrayteOperationZone> GetOperationZone()
        {
            var list = new CustomerRepository().GetOperationZone();
            return list;
        }

        [HttpGet]
        public List<LogisticShipmentType> GetShipmentType(int OperationZoneId, string CourierCompany, string ModuleType)
        {
            var list = new CustomerRepository().GetShipmentType(OperationZoneId, CourierCompany, ModuleType);
            return list;
        }

        [HttpGet]
        public HttpResponseMessage GetInitials(int OperationZoneId, string LogisticCompany, int UserId)
        {
            //Step 1: Get LogisticCompany List
            var lstLogistic = new CustomerRepository().CustomerMarginLogistic(OperationZoneId, UserId);

            //Step 2: Get Margin Option List
            List<MarginOptions> lstMargin = new List<MarginOptions>();
            if (!string.IsNullOrEmpty(LogisticCompany))
            {
                lstMargin = new CustomerRepository().GetMarginOption(OperationZoneId, LogisticCompany);
            }
            else
            {
                lstMargin = new CustomerRepository().GetMarginOption(OperationZoneId);
            }

            return this.Request.CreateResponse(
                HttpStatusCode.OK,
                new
                {
                    MarginOption = lstMargin,
                    Logistic = lstLogistic
                });
        }

        [HttpGet]
        public List<MarginOptions> GetMarginOptionPercentage(int OperationZoneId, string CourierCompany, string MarginOption)
        {
            List<MarginOptions> option = new CustomerRepository().GetMarginOptionPercentage(OperationZoneId, CourierCompany, MarginOption);
            return option;
        }

        [HttpGet]
        public IHttpActionResult GetCustomerModules(int customerId)
        {
            FrayteCustomerModule customerModules = new CustomerRepository().GetCustomerModules(customerId);
            return Ok(customerModules);
        }

        [HttpGet]
        public FrayteCustomerMarginOptions GetCustomerMarginOptions(int OperationZoneId, string CourierCompany)
        {
            var option = new CustomerRepository().CustomerMarginOptions(OperationZoneId, CourierCompany);
            return option;
        }

        [HttpGet]
        public List<FrayteCustomerLogisticCompany> GetCustomerMarginLogistic(int OperationZoneId, int UserId)
        {
            var logistic = new CustomerRepository().CustomerMarginLogistic(OperationZoneId, UserId);
            return logistic;
        }

        [HttpGet]
        public List<FrayteCustomerLogisticCompany> GetCustomerMarginLogistic(int OperationZoneId)
        {
            var logistic = new CustomerRepository().MarginLogisticItem(OperationZoneId);
            return logistic;
        }

        [HttpPost]
        public IHttpActionResult AddCustomerMarginOptions(FrayteCustomerMarginOptions Options)
        {
            new CustomerRepository().AddCustomerMarginOptions(Options);
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult GenerateCustomerBaseRateCard(CustomerRate Rate)
        {
            FrayteManifestName result = new FrayteManifestName();
            if (Rate.FileType == FrayteCustomerBaseRateFileType.Excel)
            {
                result = new CustomerBaseRateReport().CustomerBaseRate(Rate);
            }
            else if (Rate.FileType == FrayteCustomerBaseRateFileType.Pdf)
            {
                result = new CustomerBaseRateReport().CustomerBaseRate(Rate);
            }
            else if (Rate.SendingOption == FrayteCustomerBaseRateFileType.Summery)
            {
                result = new CustomerBaseRateReport().CustomerBaseRateSummery(Rate);
            }
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GenerateSupplementoryCharges(CustomerRate Rate)
        {
            FrayteManifestName result = new FrayteManifestName();
            result = new CustomerRepository().DownloadSupplemantoryChargePdf(Rate);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult SendCustomerRateCardAsEmail(CustomerRate Customer)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var filepath = new CustomerBaseRateReport().CustomerBaseRate(Customer);
                result = new ShipmentEmailRepository().EmailCustomerRateCard(Customer, filepath.FileName, filepath.FilePath);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return Ok(result);
        }

        [HttpPost]
        public HttpResponseMessage DownLoadRateCardReport(ReportFileCustomerRate fileName)
        {
            try
            {
                if (fileName != null && !string.IsNullOrEmpty(fileName.FileName))
                {
                    return DownloadrateCardReport(fileName.FileName, fileName.UserId);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public HttpResponseMessage DownloadSupplemantoryCharge(ReportFileCustomerRate fileName)
        {
            try
            {
                if (fileName != null && !string.IsNullOrEmpty(fileName.FileName))
                {
                    return DownloadrateSupplemantoryFile(fileName.FileName);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public IHttpActionResult EnterTNTRateCard(int LogisticServiceId, int OperationZoneId, string Currency)
        {
            new CustomerRepository().SaveTNTRates(LogisticServiceId, OperationZoneId, Currency);
            return Ok();
        }

        [HttpGet]
        public FrayteCustomerApiDetail CustomerApiDetail(int CustomerId)
        {
            FrayteCustomerApiDetail api = new CustomerRepository().CustomerApi(CustomerId);
            return api;
        }

        [HttpGet]
        public IHttpActionResult CustomerCompanyDetail(int UserId)
        {
            var detail = new CustomerRepository().GetSpecialCustomerDetail(UserId);
            return Ok(detail);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public FrayteCustomerAddressBookExcel UploadAddressBookExcel(int CustomerId)
        {
            FrayteCustomerAddressBookExcel frayteAddressBookexcel = new FrayteCustomerAddressBookExcel();

            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count > 0)
            {
                HttpFileCollection files = httpRequest.Files;

                HttpPostedFile file = files[0];

                if (!string.IsNullOrEmpty(file.FileName))
                {
                    string connString = "";
                    string filename = DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss_") + file.FileName;
                    string filepath = HttpContext.Current.Server.MapPath("~/UploadFiles/" + filename);

                    file.SaveAs(filepath);

                    connString = new ShipmentRepository().getExcelConnectionString(filename, filepath);
                    string fileExtension = new DirectShipmentRepository().getFileExtensionString(filename);

                    if (!string.IsNullOrEmpty(fileExtension))
                    {
                        OleDbConnection oledbConn = new OleDbConnection(connString);
                        try
                        {
                            DataSet ds = new DataSet();
                            if (fileExtension == FrayteFileExtension.CSV)
                            {
                                oledbConn.Open();
                                OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + Path.GetFileName(filename) + "]", oledbConn);
                                OleDbDataAdapter oleda = new OleDbDataAdapter();
                                oleda.SelectCommand = cmd;
                                oleda.Fill(ds, "AddressBook");
                            }
                            else
                            {
                                oledbConn.Open();
                                DataTable dbSchema = oledbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                string firstSheetName = dbSchema.Rows[0]["TABLE_NAME"].ToString();
                                var query = "SELECT * FROM " + "[" + firstSheetName + "]";
                                using (var adapter = new OleDbDataAdapter(query, oledbConn))
                                {
                                    adapter.Fill(ds, "AddressBook");
                                }
                            }

                            var exceldata = ds.Tables[0];

                            string AddressBookColumn = "FirstName,LastName,CompanyName,Email,PhoneNo,Address1,Area,Address2,City,State,Zip,CountryName";
                            bool IsExcelValid = new CustomerRepository().CheckUploadExcelColumns(AddressBookColumn, exceldata);
                            if (!IsExcelValid)
                            {
                                frayteAddressBookexcel.Status = false;
                                frayteAddressBookexcel.RowErrors = new List<string>();
                                frayteAddressBookexcel.RowErrors.Add("Columns are not matching with provided template columns. Please check the column names.");
                                return frayteAddressBookexcel;
                            }
                            else
                            {
                                if (exceldata != null && exceldata.Rows.Count > 0)
                                {
                                    List<string> _errorrows = new List<string>();
                                    List<FrayteCustomerAddressBook> _addressbook = new CustomerRepository().GetAddressBookDetail(CustomerId, exceldata, ref _errorrows);
                                    if (_errorrows.Count > 0)
                                    {
                                        frayteAddressBookexcel.Status = false;
                                        frayteAddressBookexcel.RowErrors = new List<string>();
                                        frayteAddressBookexcel.RowErrors = _errorrows;
                                        return frayteAddressBookexcel;
                                    }
                                    else
                                    {
                                        List<string> _lengtherror = new CustomerRepository().ValidateAddressBookDataLength(_addressbook);
                                        if (_lengtherror.Count > 0)
                                        {
                                            frayteAddressBookexcel.Status = false;
                                            frayteAddressBookexcel.RowErrors = new List<string>();
                                            frayteAddressBookexcel.RowErrors = _lengtherror;
                                            return frayteAddressBookexcel;
                                        }
                                        else
                                        {
                                            bool IsInserted = new CustomerRepository().InsertCustomerAddressBook(_addressbook);
                                            if (!IsInserted)
                                            {
                                                frayteAddressBookexcel.Status = false;
                                                frayteAddressBookexcel.RowErrors = new List<string>();
                                                frayteAddressBookexcel.RowErrors.Add("Address Book Can Not Save");
                                                return frayteAddressBookexcel;
                                            }
                                            else
                                            {
                                                frayteAddressBookexcel.Status = true;
                                                return frayteAddressBookexcel;
                                            }
                                        }
                                    }
                                }
                                oledbConn.Close();
                                if ((System.IO.File.Exists(filepath)))
                                {
                                    System.IO.File.Delete(filepath);
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        finally
                        {
                            oledbConn.Close();
                            if ((System.IO.File.Exists(filepath)))
                            {
                                System.IO.File.Delete(filepath);
                            }
                        }
                    }
                }
            }
            return frayteAddressBookexcel;
        }

        #region Customer Setup

        [HttpGet]
        public CustomerConfigurationSetUp GetCustomerSetUp(int UserId)
        {
            CustomerConfigurationSetUp detail = new CustomerRepository().GetCustomerSetUp(UserId);
            return detail;
        }

        [HttpPost]
        public IHttpActionResult SaveCustomerSetUp(CustomerConfigurationSetUp customerConfiguration)
        {
            FrayteResult result = new CustomerRepository().SaveCustomerSetUp(customerConfiguration);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult UploadCustomerLogo()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                HttpFileCollection files = httpRequest.Files;

                //This code will execute only when user will upload the document

                if (files.Count > 0)
                {
                    int UserId = Convert.ToInt32(httpRequest.Form["UserId"].ToString());
                    var docfiles = new List<string>();
                    HttpPostedFile file = files[0];

                    string filePathToSave = AppSettings.EmailServicePath + "//EmailTeamplate//" + UserId;
                    string fileFullPath = filePathToSave + "//Images//";

                    filePathToSave = fileFullPath;

                    string fileToSaveForDirectory = HttpContext.Current.Server.MapPath("~/FrayteEmailService/EmailTeamplate/" + UserId + "/Images/");

                    if (!System.IO.Directory.Exists(filePathToSave))
                        System.IO.Directory.CreateDirectory(filePathToSave);

                    if (!System.IO.Directory.Exists(fileToSaveForDirectory))
                        System.IO.Directory.CreateDirectory(fileToSaveForDirectory);

                    if (!string.IsNullOrEmpty(file.FileName))
                    {
                        if (File.Exists(filePathToSave + file.FileName))
                        {
                            File.Delete(filePathToSave + file.FileName);
                        }

                        //Save in server folder
                        fileFullPath = filePathToSave + file.FileName;
                        file.SaveAs(fileFullPath);

                        if (File.Exists(fileToSaveForDirectory + file.FileName))
                        {
                            File.Delete(fileToSaveForDirectory + file.FileName);
                        }

                        //Save in server folder
                        string fileFullPathFor = fileToSaveForDirectory + file.FileName;
                        file.SaveAs(fileFullPathFor);

                        //Save file name and other information in DB
                        CustomerConfigurationSetUp result = new CustomerRepository().SaveCustomerLogo(filePathToSave, file.FileName, UserId);
                        return Ok(result);
                    }
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest();
            }
        }

        #endregion

        [HttpPost]
        public IHttpActionResult UploadUserDocument()
        {
            string status = string.Empty;
            int RandomGeneratedNo = 0;
            UserDocumentFile UDF = new UserDocumentFile();
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                HttpFileCollection files = httpRequest.Files;
                HttpPostedFile file = files[0];
                if (!string.IsNullOrEmpty(file.FileName))
                {
                    try
                    {
                        string DocType = httpRequest.Form["DocType"].ToString();
                        string Doc = httpRequest.Form["Doc"].ToString();
                        string CustomerId = httpRequest.Form["CustomerId"].ToString();
                        string filename = file.FileName;
                        UDF.status = false;
                        if (string.IsNullOrEmpty(CustomerId) || CustomerId == "0")
                        {
                            string documentType = string.Empty;
                            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/UploadFiles/UserDocuments/TempUserDocument/")))
                                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/UploadFiles/UserDocuments/TempUserDocument/"));

                            RandomGeneratedNo = new Random().Next(10000000, 99999999);
                            string filepath = HttpContext.Current.Server.MapPath("~/UploadFiles/UserDocuments/TempUserDocument/" + RandomGeneratedNo + filename);
                            if (!string.IsNullOrEmpty(DocType))
                            {
                                filepath = HttpContext.Current.Server.MapPath("~/UploadFiles/UserDocuments/TempUserDocument/" + DocType);
                                if (System.IO.File.Exists(filepath))
                                {
                                    UDF.FileName = filename;
                                    UDF.SavedFileName = RandomGeneratedNo + filename;
                                    UDF.status = false;
                                }
                                else
                                {
                                    file.SaveAs(filepath);
                                    UDF.FileName = filename;
                                    UDF.SavedFileName = RandomGeneratedNo + filename;
                                    UDF.status = true;
                                }
                            }
                            else
                            {
                                if (System.IO.File.Exists(filepath))
                                {
                                    UDF.FileName = filename;
                                    UDF.SavedFileName = RandomGeneratedNo + filename;
                                    UDF.status = false;
                                }
                                else
                                {
                                    file.SaveAs(filepath);
                                    UDF.FileName = filename;
                                    UDF.SavedFileName = RandomGeneratedNo + filename;
                                    UDF.status = true;
                                }
                            }
                        }
                        else
                        {
                            if (System.IO.Directory.Exists(AppSettings.UploadFolderPath + "/UserDocuments/" + Convert.ToInt32(CustomerId)) &&
                                System.IO.File.Exists(AppSettings.UploadFolderPath + "/UserDocuments/" + Convert.ToInt32(CustomerId) + "/" + filename))
                            {
                                UDF.status = false;
                                UDF.FileName = filename;
                                UDF.SavedFileName = DocType;
                            }
                            else
                            {
                                if ((System.IO.File.Exists(AppSettings.UploadFolderPath + "/UserDocuments/TempUserDocument/" + DocType)) && DocType.Contains(filename))
                                {
                                    UDF.FileName = filename;
                                    UDF.SavedFileName = DocType;
                                    UDF.status = false;
                                }
                                else
                                {
                                    string[] filePaths = Directory.GetFiles(AppSettings.UploadFolderPath + "/UserDocuments/TempUserDocument/");
                                    if (filePaths.Length > 0)
                                    {
                                        foreach (string filePath in filePaths)
                                        {
                                            File.Delete(filePath);
                                        }
                                    }
                                    RandomGeneratedNo = new Random().Next(10000000, 99999999);
                                    string filepath = HttpContext.Current.Server.MapPath("~/UploadFiles/UserDocuments/TempUserDocument/" + RandomGeneratedNo + filename);
                                    file.SaveAs(filepath);
                                    UDF.FileName = filename;
                                    UDF.SavedFileName = RandomGeneratedNo + filename;
                                    UDF.status = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                }
            }
            return Ok(UDF);
        }

        #region ---Private Method

        private HttpResponseMessage DownloadrateCardReport(string fileName, int UserId)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/ReportFiles/CustomerRateCard/" + UserId + "/" + fileName);
            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    ms.Write(bytes, 0, (int)file.Length);
                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                    httpResponseMessage.Content = new ByteArrayContent(bytes);
                    httpResponseMessage.Content.Headers.Add("download-status", "downloaded");
                    httpResponseMessage.Content.Headers.Add("x-filename", fileName);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = fileName;
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    return httpResponseMessage;
                }
            }
        }

        private HttpResponseMessage DownloadrateSupplemantoryFile(string fileName)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/SupplemantoryChargeFile/" + fileName);
            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    ms.Write(bytes, 0, (int)file.Length);
                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                    httpResponseMessage.Content = new ByteArrayContent(bytes);
                    httpResponseMessage.Content.Headers.Add("download-status", "downloaded");
                    httpResponseMessage.Content.Headers.Add("x-filename", fileName);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = fileName;
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    return httpResponseMessage;
                }
            }
        }

        #endregion

        #region ---Public Method

        public List<OperationZone> OperationZone()
        {
            var list = new CustomerRepository().GetOpearationZone();
            return list;
        }

        public FrayteOperationZone GetBusinessOperationZoneId()
        {
            FrayteOperationZone OperationZone = UtilityRepository.GetOperationZone();
            return OperationZone;
        }

        #endregion
    }
}