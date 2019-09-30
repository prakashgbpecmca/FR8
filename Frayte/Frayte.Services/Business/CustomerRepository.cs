using Frayte.Services.DataAccess;
using Frayte.Services.Utility;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Web;
using RazorEngine.Templating;
using System.IO;
using System.Data.Entity.Validation;
using Frayte.Services.Models.BreakBulk;

namespace Frayte.Services.Business
{
    public class CustomerRepository
    {
        string excelname = string.Empty;

        FrayteEntities dbContext = new FrayteEntities();

        public FrayteCustomerMargin margin { get; private set; }

        public FrayteCustomer GetCustomerDetail(int customerId)
        {
            FrayteCustomer customerDetail = new FrayteCustomer();
            WorkingWeekDay workingDays = new WorkingWeekDay();
            //Step 1: Get Customer's basic information
            var customer = dbContext.Users.Where(p => p.UserId == customerId).FirstOrDefault();

            if (customer != null)
            {
                customerDetail = UtilityRepository.CustomerMapping(customer);

                var role = (from ur in dbContext.UserRoles
                            join uu in dbContext.Users on ur.UserId equals uu.UserId
                            where uu.UserId == customerId
                            select new { ur.RoleId }).FirstOrDefault();

                if (role != null)
                {
                    customerDetail.RoleId = role.RoleId;
                }

                // Get Working Week day
                if (customerDetail.WorkingWeekDay.WorkingWeekDayId > 0)
                {
                    workingDays = dbContext.WorkingWeekDays.Find(customerDetail.WorkingWeekDay.WorkingWeekDayId);
                }


                if (workingDays != null)
                {
                    customerDetail.WorkingWeekDay = workingDays;
                }

                //Step 1.1: Get Customer's time zone
                var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == customer.TimezoneId).FirstOrDefault();
                if (timeZone != null)
                {
                    customerDetail.Timezone = new TimeZoneModal();
                    customerDetail.Timezone.TimezoneId = timeZone.TimezoneId;
                    customerDetail.Timezone.Name = timeZone.Name;
                    customerDetail.Timezone.Offset = timeZone.Offset;
                    customerDetail.Timezone.OffsetShort = timeZone.OffsetShort;
                }

                //Step 2: Get Customer's other information
                var customerOtherDetails = dbContext.UserAdditionals.Where(p => p.UserId == customerId).FirstOrDefault();
                if (customerOtherDetails != null)
                {
                    customerDetail.AccountNumber = customerOtherDetails.AccountNo;
                    customerDetail.AccountName = customerOtherDetails.AccountName;
                    customerDetail.AccountMail = customerOtherDetails.AccountMail;
                    customerDetail.CreditLimit = customerOtherDetails.CreditLimit.HasValue ? customerOtherDetails.CreditLimit.Value : 0;
                    customerDetail.CreditLimitCurrencyCode = customerOtherDetails.CreditLimitCurrencyCode;
                    customerDetail.TermsOfPayment = customerOtherDetails.TermsOfThePayment;
                    customerDetail.TaxAndDuties = customerOtherDetails.TaxAndDuties;
                    if (customerOtherDetails.IsDirectBooking.HasValue)
                    {
                        customerDetail.IsDirectBooking = customerOtherDetails.IsDirectBooking.Value;
                    }
                    else
                    {
                        customerDetail.IsDirectBooking = false;
                    }
                    if (customerOtherDetails.IsTradelaneBooking.HasValue)
                    {
                        customerDetail.IsTradeLaneBooking = customerOtherDetails.IsTradelaneBooking.Value;
                    }
                    else
                    {
                        customerDetail.IsTradeLaneBooking = false;
                    }
                    if (customerOtherDetails.IsBreakBulkBooking.HasValue)
                    {
                        customerDetail.IsBreakBulkBooking = customerOtherDetails.IsBreakBulkBooking.Value;
                    }
                    else
                    {
                        customerDetail.IsBreakBulkBooking = false;
                    }
                    if (customerOtherDetails.IsECommerce.HasValue)
                    {
                        customerDetail.IsECommerce = customerOtherDetails.IsECommerce.Value;
                        customerDetail.FreeStorageTime = UtilityRepository.GetTimeZoneTime(customerOtherDetails.FreeStorageTime);
                        customerDetail.FreeStorageChargeCurrencyCode = customerOtherDetails.FreeStorageCurrencyCode;
                        customerDetail.FreeStorageCharge = customerOtherDetails.FreeStorageCharge.HasValue ? customerOtherDetails.FreeStorageCharge.Value : 0;
                    }
                    else
                    {
                        customerDetail.IsECommerce = false;
                    }
                    if (customerOtherDetails.IsShipperTaxAndDuty.HasValue)
                    {
                        customerDetail.IsShipperTaxAndDuty = customerOtherDetails.IsShipperTaxAndDuty.Value;
                    }
                    else
                    {
                        customerDetail.IsShipperTaxAndDuty = false;
                    }
                    if (customerOtherDetails.IsAllowRate.HasValue)
                    {
                        customerDetail.IsAllowRate = customerOtherDetails.IsAllowRate.Value;
                    }
                    else
                    {
                        customerDetail.IsAllowRate = false;
                    }
                    if (customerOtherDetails.IsApiAllow.HasValue)
                    {
                        customerDetail.IsApiAllow = customerOtherDetails.IsApiAllow.Value;
                    }
                    else
                    {
                        customerDetail.IsApiAllow = false;
                    }
                    if (customerOtherDetails.IsWarehouseTransport.HasValue)
                    {
                        customerDetail.IsWarehouseTransport = customerOtherDetails.IsWarehouseTransport.Value;
                    }
                    else
                    {
                        customerDetail.IsWarehouseTransport = false;
                    }
                    if (customerOtherDetails.IsExpressSolutions.HasValue)
                    {
                        customerDetail.IsExpressSolutions = customerOtherDetails.IsExpressSolutions.Value;
                    }
                    else
                    {
                        customerDetail.IsExpressSolutions = false;
                    }
                    if (customerOtherDetails.IsServiceSelected.HasValue)
                    {
                        customerDetail.IsServiceSelected = customerOtherDetails.IsServiceSelected.Value;
                    }
                    else
                    {
                        customerDetail.IsServiceSelected = false;
                    }

                    if (customerOtherDetails.IsWithoutService.HasValue)
                    {
                        customerDetail.IsWithoutService = customerOtherDetails.IsWithoutService.Value;
                    }
                    else
                    {
                        customerDetail.IsWithoutService = false;
                    }

                    customerDetail.DaysValidity = customerOtherDetails.DaysValidity.HasValue ? customerOtherDetails.DaysValidity.Value : 0;
                    customerDetail.CustomerRateCardType = customerOtherDetails.CustomerRateCardType;
                    customerDetail.CustomerType = customerOtherDetails.CustomerType;
                    customerDetail.UserType = customerOtherDetails.UserType;
                    //Get associated Frayte User's detail
                    GetAssociateUsersDetail(customerDetail, customerOtherDetails);
                }

                //Step 3: Get Customer's Address information
                var customerAddress = dbContext.UserAddresses.Where(p => p.UserId == customerId &&
                                                                   (p.AddressTypeId == (int)FrayteAddressType.MainAddress ||
                                                                    p.AddressTypeId == (int)FrayteAddressType.OtherAddress)).ToList();
                if (customerAddress != null)
                {
                    customerDetail.OtherAddresses = new List<FrayteAddress>();

                    foreach (UserAddress address in customerAddress)
                    {
                        if (address.AddressTypeId == (int)FrayteAddressType.MainAddress)
                        {
                            //Step 3.1: Set Customer main address
                            customerDetail.UserAddress = new FrayteAddress();
                            customerDetail.UserAddress = UtilityRepository.UserAddressMapping(address);

                            //Step : Get country information
                            var country = dbContext.Countries.Where(p => p.CountryId == address.CountryId).FirstOrDefault();
                            if (country != null)
                            {
                                customerDetail.UserAddress.Country = new FrayteCountryCode();
                                customerDetail.UserAddress.Country.CountryId = country.CountryId;
                                customerDetail.UserAddress.Country.Code = country.CountryCode;
                                customerDetail.UserAddress.Country.Code2 = country.CountryCode2;
                                customerDetail.UserAddress.Country.Name = country.CountryName;
                            }
                        }
                        else
                        {
                            //Step 3.2: Set Customer's other addresses
                            FrayteAddress otherAddress = UtilityRepository.UserAddressMapping(address);

                            //Step : Get country information
                            var country = dbContext.Countries.Where(p => p.CountryId == otherAddress.Country.CountryId).FirstOrDefault();
                            if (country != null)
                            {
                                otherAddress.Country = new FrayteCountryCode();
                                otherAddress.Country.CountryId = country.CountryId;
                                otherAddress.Country.Code = country.CountryCode;
                                otherAddress.Country.Name = country.CountryName;
                            }

                            customerDetail.OtherAddresses.Add(otherAddress);
                        }
                    }
                }

                //Step 4: Get Customer's Tradelane information
                customerDetail.Tradelanes = new TradelaneRepository().GetTradelaneList(customerId);

                //Step 5: Get Customer's POD Setting
                customerDetail.CustomerPODSetting = GetCustomerSetting(customerId, FrayteCustomerPODRateSetting.POD);

                //Step 6: Get Customer's rate card detail
                var data = dbContext.CustomerLogistics.Where(p => p.UserId == customerId).ToList();
                if (data != null && data.Count > 0)
                {
                    customerDetail.IsRateCardAssigned = true;
                }
                else
                {
                    customerDetail.IsRateCardAssigned = false;
                }

                //GetCustomerDocument
                if (System.IO.Directory.Exists(AppSettings.UploadFolderPath + "/UserDocuments/" + customerDetail.UserId))
                {
                    string[] filePaths = Directory.GetFiles(AppSettings.UploadFolderPath + "/UserDocuments/" + customerDetail.UserId);
                    var Filename = filePaths[0].Split('\\');
                    customerDetail.UserDocument = Filename[Filename.Length - 1];
                }
                else
                {
                    customerDetail.UserDocument = "";
                }
            }
            return customerDetail;
        }

        public string GetCustomerAccNo(string AccNo)
        {
            var Result = dbContext.UserAdditionals.Where(a => a.AccountNo != null && a.AccountNo != "" && a.AccountNo.Substring(0, 3) == AccNo).FirstOrDefault();
            var Res = "";
            if (Result != null)
            {
                Res = Result.AccountNo;
            }
            else
            {
                Res = "";
            }
            return Res;
        }

        public CustomerBasicDetail GetCustomerDetailByAccountNumber(string accountNumber)
        {
            try
            {
                var customer = (from r in dbContext.UserAdditionals
                                join u in dbContext.Users on r.UserId equals u.UserId
                                where r.AccountNo == accountNumber
                                select new CustomerBasicDetail
                                {
                                    CustomerId = u.UserId,
                                    CustomerEmail = u.UserEmail,
                                    AccountNumber = r.AccountNo
                                }).FirstOrDefault();

                return customer;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public FrayteTrackingConfiguration GetTrackingConfiguration(int customerId)
        {
            FrayteTrackingConfiguration trackingConfiguration = new FrayteTrackingConfiguration();
            try
            {
                var detail = dbContext.UserTrackingConfigurations.Where(p => p.UserId == customerId).FirstOrDefault();
                var customerDetail = dbContext.Users.Find(customerId);
                if (detail != null)
                {

                    trackingConfiguration.UserTrackingConfigurationId = detail.UserTrackingConfigurationId;
                    trackingConfiguration.CustomerId = detail.UserId;
                    trackingConfiguration.AttemptFailEmails = !string.IsNullOrEmpty(detail.AttemptFailEmails) ? detail.AttemptFailEmails.Split(';').Select(p => new FrayteEmailModel { Email = p }).ToList() : new List<FrayteEmailModel> { new FrayteEmailModel { Email = customerDetail.UserEmail } };
                    trackingConfiguration.DeliveredEmails = !string.IsNullOrEmpty(detail.DeliveredEmails) ? detail.DeliveredEmails.Split(';').Select(p => new FrayteEmailModel { Email = p }).ToList() : new List<FrayteEmailModel> { new FrayteEmailModel { Email = customerDetail.UserEmail } }; ;
                    trackingConfiguration.ExceptionEmails = !string.IsNullOrEmpty(detail.ExceptionEmails) ? detail.ExceptionEmails.Split(';').Select(p => new FrayteEmailModel { Email = p }).ToList() : new List<FrayteEmailModel> { new FrayteEmailModel { Email = customerDetail.UserEmail } }; ;
                    trackingConfiguration.ExpiredEmails = !string.IsNullOrEmpty(detail.ExpiredEmails) ? detail.ExpiredEmails.Split(';').Select(p => new FrayteEmailModel { Email = p }).ToList() : new List<FrayteEmailModel> { new FrayteEmailModel { Email = customerDetail.UserEmail } }; ;
                    trackingConfiguration.InfoReceivedEmails = !string.IsNullOrEmpty(detail.InfoReceivedEmails) ? detail.InfoReceivedEmails.Split(';').Select(p => new FrayteEmailModel { Email = p }).ToList() : new List<FrayteEmailModel> { new FrayteEmailModel { Email = customerDetail.UserEmail } }; ;
                    trackingConfiguration.InTransitEmails = !string.IsNullOrEmpty(detail.InTransitEmails) ? detail.InTransitEmails.Split(';').Select(p => new FrayteEmailModel { Email = p }).ToList() : new List<FrayteEmailModel> { new FrayteEmailModel { Email = customerDetail.UserEmail } }; ;
                    trackingConfiguration.OutForDeliveryEmails = !string.IsNullOrEmpty(detail.OutForDeliveryEmails) ? detail.OutForDeliveryEmails.Split(';').Select(p => new FrayteEmailModel { Email = p }).ToList() : new List<FrayteEmailModel> { new FrayteEmailModel { Email = customerDetail.UserEmail } }; ;
                    trackingConfiguration.PendingEmails = !string.IsNullOrEmpty(detail.PendingEmails) ? detail.PendingEmails.Split(';').Select(p => new FrayteEmailModel { Email = p }).ToList() : new List<FrayteEmailModel> { new FrayteEmailModel { Email = customerDetail.UserEmail } }; ;
                }
                else
                {
                    trackingConfiguration.AttemptFailEmails = new List<FrayteEmailModel> { new FrayteEmailModel { Email = customerDetail.UserEmail } };
                    trackingConfiguration.DeliveredEmails = new List<FrayteEmailModel> { new FrayteEmailModel { Email = customerDetail.UserEmail } };
                    trackingConfiguration.ExceptionEmails = new List<FrayteEmailModel> { new FrayteEmailModel { Email = customerDetail.UserEmail } };
                    trackingConfiguration.InfoReceivedEmails = new List<FrayteEmailModel> { new FrayteEmailModel { Email = customerDetail.UserEmail } };
                    trackingConfiguration.InTransitEmails = new List<FrayteEmailModel> { new FrayteEmailModel { Email = customerDetail.UserEmail } };
                    trackingConfiguration.OutForDeliveryEmails = new List<FrayteEmailModel> { new FrayteEmailModel { Email = customerDetail.UserEmail } };
                    trackingConfiguration.PendingEmails = new List<FrayteEmailModel> { new FrayteEmailModel { Email = customerDetail.UserEmail } };

                    trackingConfiguration.CustomerId = customerId;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return trackingConfiguration;
        }

        public CustomerConfigurationSetUp GetCustomerSetUp(int userId)
        {
            CustomerConfigurationSetUp customerDetail = new CustomerConfigurationSetUp();
            var customerConfiguration = dbContext.CustomerCompanyDetails.Where(p => p.UserId == userId).FirstOrDefault();
            if (customerConfiguration != null)
            {
                customerDetail.UserId = userId;
                customerDetail.SMTPDisplayName = customerConfiguration.SMTPDisplayName;
                customerDetail.SMTPEnableSsl = (!string.IsNullOrEmpty(customerConfiguration.SMTPEnableSsl) && customerConfiguration.SMTPEnableSsl == "Y" ? true : false);
                customerDetail.SMTPFromMail = customerConfiguration.SMTPFromMail;
                customerDetail.SMTPHostName = customerConfiguration.SMTPHostName;
                customerDetail.SMTPPassword = customerConfiguration.SMTPPassword;
                customerDetail.SMTPport = customerConfiguration.SMTPport;
                customerDetail.SMTPUserName = customerConfiguration.SMTPUserName;
                customerDetail.UserPosition = customerConfiguration.UserPosition;
                customerDetail.OperationStaff = customerConfiguration.OperationStaff;
                customerDetail.OperationStaffEmail = customerConfiguration.OperationStaffEmail;
                customerDetail.CompanyName = customerConfiguration.CompanyName;

                var arr = customerConfiguration.OperationStaffPhone.Split(')');
                if (arr != null && arr.Count() > 1)
                {
                    customerDetail.OperationStaffPhoneCode = arr[0] + ")";
                    customerDetail.OperationStaffPhone = arr[1].TrimStart();
                }
                else
                {
                    customerDetail.OperationStaffPhone = customerConfiguration.OperationStaffPhone;
                }

                customerDetail.LogoFileName = customerConfiguration.LogoFileName;
                customerDetail.LogoFilePath = AppSettings.WebApiPath + "FrayteEmailService\\EmailTeamplate\\" + userId + "\\Images\\" + customerConfiguration.LogoFileName;
                customerDetail.SiteAddress = customerConfiguration.SiteAddress;
            }
            else
            {
                customerDetail.UserId = userId;

                var customerCountry = (from r in dbContext.Users
                                       join u in dbContext.UserAddresses on r.UserId equals u.UserId
                                       join d in dbContext.Countries on u.CountryId equals d.CountryId
                                       where r.UserId == userId
                                       select d
                                      ).FirstOrDefault();
                if (customerCountry != null)
                {
                    customerDetail.OperationStaffPhoneCode = customerCountry.CountryPhoneCode;
                }
            }
            return customerDetail;
        }

        public FrayteResult SaveCustomerSetUp(CustomerConfigurationSetUp customerConfiguration)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var user = dbContext.Users.Find(customerConfiguration.UserId);
                if (user != null)
                {
                    CustomerCompanyDetail detail;
                    detail = dbContext.CustomerCompanyDetails.Where(p => p.UserId == customerConfiguration.UserId).FirstOrDefault();
                    if (detail != null)
                    {
                        detail.SMTPDisplayName = customerConfiguration.SMTPDisplayName;
                        detail.SMTPEnableSsl = customerConfiguration.SMTPEnableSsl ? "Y" : "N";
                        detail.SMTPFromMail = customerConfiguration.SMTPFromMail;
                        detail.SMTPHostName = customerConfiguration.SMTPHostName;
                        detail.SMTPPassword = customerConfiguration.SMTPPassword;
                        detail.SMTPport = customerConfiguration.SMTPport;
                        detail.CompanyName = customerConfiguration.CompanyName;
                        detail.SMTPUserName = customerConfiguration.SMTPUserName;
                        detail.UserPosition = customerConfiguration.UserPosition;
                        detail.OperationStaff = customerConfiguration.OperationStaff;
                        detail.OperationStaffEmail = customerConfiguration.OperationStaffEmail;
                        detail.OperationStaffPhone = customerConfiguration.OperationStaffPhoneCode + customerConfiguration.OperationStaffPhone;
                        detail.SiteAddress = customerConfiguration.SiteAddress;
                        detail.SiteLink = user.CompanyName;
                        dbContext.SaveChanges();
                        result.Status = true;
                    }
                    else
                    {
                        detail = new CustomerCompanyDetail();
                        detail.SMTPDisplayName = customerConfiguration.SMTPDisplayName;
                        detail.SMTPEnableSsl = customerConfiguration.SMTPEnableSsl ? "Y" : "N";
                        detail.SMTPFromMail = customerConfiguration.SMTPFromMail;
                        detail.SMTPHostName = customerConfiguration.SMTPHostName;
                        detail.SMTPPassword = customerConfiguration.SMTPPassword;
                        detail.SMTPport = customerConfiguration.SMTPport;
                        detail.CompanyName = customerConfiguration.CompanyName;
                        detail.SMTPUserName = customerConfiguration.SMTPUserName;
                        detail.UserPosition = customerConfiguration.UserPosition;
                        detail.OperationStaff = customerConfiguration.OperationStaff;
                        detail.OperationStaffEmail = customerConfiguration.OperationStaffEmail;
                        detail.OperationStaffPhone = customerConfiguration.OperationStaffPhoneCode + customerConfiguration.OperationStaffPhone;
                        detail.SiteAddress = customerConfiguration.SiteAddress;
                        detail.SiteLink = user.CompanyName;
                        dbContext.CustomerCompanyDetails.Add(detail);
                        dbContext.SaveChanges();
                        result.Status = true;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public FrayteTrackingConfiguration SaveTrackingConfiguration(FrayteTrackingConfiguration trackingConfiguration)
        {
            try
            {
                if (trackingConfiguration != null)
                {
                    UserTrackingConfiguration userTrackingConfiguration;

                    if (trackingConfiguration.UserTrackingConfigurationId == 0)
                    {
                        userTrackingConfiguration = new UserTrackingConfiguration();
                        userTrackingConfiguration.UserId = trackingConfiguration.CustomerId;
                        userTrackingConfiguration.AttemptFailEmails = (trackingConfiguration.AttemptFailEmails != null && trackingConfiguration.AttemptFailEmails.Count > 0) ? String.Join(";", trackingConfiguration.AttemptFailEmails.Select(p => p.Email).ToArray()) : "";
                        userTrackingConfiguration.DeliveredEmails = (trackingConfiguration.DeliveredEmails != null && trackingConfiguration.DeliveredEmails.Count > 0) ? String.Join(";", trackingConfiguration.DeliveredEmails.Select(p => p.Email).ToArray()) : "";
                        userTrackingConfiguration.ExceptionEmails = (trackingConfiguration.ExceptionEmails != null && trackingConfiguration.ExceptionEmails.Count > 0) ? String.Join(";", trackingConfiguration.ExceptionEmails.Select(p => p.Email).ToArray()) : "";
                        userTrackingConfiguration.ExpiredEmails = (trackingConfiguration.ExpiredEmails != null && trackingConfiguration.ExpiredEmails.Count > 0) ? String.Join(";", trackingConfiguration.ExpiredEmails.Select(p => p.Email).ToArray()) : "";
                        userTrackingConfiguration.InfoReceivedEmails = (trackingConfiguration.InfoReceivedEmails != null && trackingConfiguration.InfoReceivedEmails.Count > 0) ? String.Join(";", trackingConfiguration.InfoReceivedEmails.Select(p => p.Email).ToArray()) : "";
                        userTrackingConfiguration.InTransitEmails = (trackingConfiguration.InTransitEmails != null && trackingConfiguration.InTransitEmails.Count > 0) ? String.Join(";", trackingConfiguration.InTransitEmails.Select(p => p.Email).ToArray()) : "";
                        userTrackingConfiguration.OutForDeliveryEmails = (trackingConfiguration.OutForDeliveryEmails != null && trackingConfiguration.OutForDeliveryEmails.Count > 0) ? String.Join(";", trackingConfiguration.OutForDeliveryEmails.Select(p => p.Email).ToArray()) : "";
                        userTrackingConfiguration.PendingEmails = (trackingConfiguration.PendingEmails != null && trackingConfiguration.PendingEmails.Count > 0) ? String.Join(";", trackingConfiguration.PendingEmails.Select(p => p.Email).ToArray()) : "";

                        dbContext.UserTrackingConfigurations.Add(userTrackingConfiguration);
                        dbContext.SaveChanges();

                        trackingConfiguration.UserTrackingConfigurationId = userTrackingConfiguration.UserTrackingConfigurationId;
                    }
                    else
                    {
                        userTrackingConfiguration = dbContext.UserTrackingConfigurations.Find(trackingConfiguration.UserTrackingConfigurationId);
                        if (userTrackingConfiguration != null)
                        {
                            userTrackingConfiguration.UserId = trackingConfiguration.CustomerId;
                            userTrackingConfiguration.AttemptFailEmails = (trackingConfiguration.AttemptFailEmails != null && trackingConfiguration.AttemptFailEmails.Count > 0) ? String.Join(";", trackingConfiguration.AttemptFailEmails.Select(p => p.Email).ToArray()) : "";
                            userTrackingConfiguration.DeliveredEmails = (trackingConfiguration.DeliveredEmails != null && trackingConfiguration.DeliveredEmails.Count > 0) ? String.Join(";", trackingConfiguration.DeliveredEmails.Select(p => p.Email).ToArray()) : "";
                            userTrackingConfiguration.ExceptionEmails = (trackingConfiguration.ExceptionEmails != null && trackingConfiguration.ExceptionEmails.Count > 0) ? String.Join(";", trackingConfiguration.ExceptionEmails.Select(p => p.Email).ToArray()) : "";
                            userTrackingConfiguration.ExpiredEmails = (trackingConfiguration.ExpiredEmails != null && trackingConfiguration.ExpiredEmails.Count > 0) ? String.Join(";", trackingConfiguration.ExpiredEmails.Select(p => p.Email).ToArray()) : "";
                            userTrackingConfiguration.InfoReceivedEmails = (trackingConfiguration.InfoReceivedEmails != null && trackingConfiguration.InfoReceivedEmails.Count > 0) ? String.Join(";", trackingConfiguration.InfoReceivedEmails.Select(p => p.Email).ToArray()) : "";
                            userTrackingConfiguration.InTransitEmails = (trackingConfiguration.InTransitEmails != null && trackingConfiguration.InTransitEmails.Count > 0) ? String.Join(";", trackingConfiguration.InTransitEmails.Select(p => p.Email).ToArray()) : "";
                            userTrackingConfiguration.OutForDeliveryEmails = (trackingConfiguration.OutForDeliveryEmails != null && trackingConfiguration.OutForDeliveryEmails.Count > 0) ? String.Join(";", trackingConfiguration.OutForDeliveryEmails.Select(p => p.Email).ToArray()) : "";
                            userTrackingConfiguration.PendingEmails = (trackingConfiguration.PendingEmails != null && trackingConfiguration.PendingEmails.Count > 0) ? String.Join(";", trackingConfiguration.PendingEmails.Select(p => p.Email).ToArray()) : "";

                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return trackingConfiguration;
        }

        public CustomerConfigurationSetUp SaveCustomerLogo(string filePathToSave, string fileName, int userId)
        {
            CustomerConfigurationSetUp customerConfiguration = new CustomerConfigurationSetUp();
            try
            {
                CustomerCompanyDetail detail;
                detail = dbContext.CustomerCompanyDetails.Where(p => p.UserId == userId).FirstOrDefault();

                FrayteResult result = new FrayteResult();

                if (detail != null)
                {
                    detail.UserId = userId;
                    detail.LogoFileName = fileName;
                    dbContext.SaveChanges();
                    result.Status = true;
                }
                else
                {
                    detail = new CustomerCompanyDetail();
                    detail.UserId = userId;
                    detail.LogoFileName = fileName;
                    dbContext.CustomerCompanyDetails.Add(detail);
                    dbContext.SaveChanges();
                    result.Status = true;
                }
                if (result.Status)
                {
                    customerConfiguration.LogoFileName = fileName;
                    customerConfiguration.LogoFilePath = AppSettings.WebApiPath + "FrayteEmailService\\EmailTeamplate\\" + userId + "\\Images\\" + customerConfiguration.LogoFileName;
                }
            }
            catch (Exception ex)
            {
            }

            return customerConfiguration;
        }

        public FrayteCustomerModule GetCustomerModules(int customerId)
        {
            FrayteCustomerModule customerModule = new FrayteCustomerModule();
            UserAdditional customerModuleDetail = dbContext.UserAdditionals.Find(customerId);

            if (customerModuleDetail != null)
            {
                customerModule.IsDirectBooking = customerModuleDetail.IsDirectBooking.HasValue ? customerModuleDetail.IsDirectBooking.Value : false;
                customerModule.IsTradeLaneBooking = customerModuleDetail.IsTradelaneBooking.HasValue ? customerModuleDetail.IsTradelaneBooking.Value : false;
                customerModule.IsBreakBulkBooking = customerModuleDetail.IsBreakBulkBooking.HasValue ? customerModuleDetail.IsBreakBulkBooking.Value : false;
                customerModule.IseCommerceBooking = customerModuleDetail.IsECommerce.HasValue ? customerModuleDetail.IsECommerce.Value : false;
                customerModule.IsWarehouseAndTransport = customerModuleDetail.IsWarehouseTransport.HasValue ? customerModuleDetail.IsWarehouseTransport.Value : false;
                customerModule.IsExpresSolutions = customerModuleDetail.IsExpressSolutions.HasValue ? customerModuleDetail.IsExpressSolutions.Value : false;
            }
            return customerModule;
        }

        public List<FrayteUser> GetCustomerList()
        {
            List<FrayteUser> users = new List<FrayteUser>();
            users = new FrayteUserRepository().GetUserTypeList((int)FrayteUserRole.Customer, (int)FrayteAddressType.MainAddress);
            return users;
        }

        #region Save Update Customer

        public void SaveCustomerTradelane(FrayteTradelane tradelane, int userId)
        {
            CustomerTradeLane tradeLaneResult = dbContext.CustomerTradeLanes.Where(p => p.TradeLaneId == tradelane.TradelaneId && p.UserId == userId).FirstOrDefault();
            if (tradeLaneResult == null)
            {
                tradeLaneResult = new CustomerTradeLane();
                tradeLaneResult.TradeLaneId = tradelane.TradelaneId;
                tradeLaneResult.UserId = userId;
                dbContext.CustomerTradeLanes.Add(tradeLaneResult);
                dbContext.SaveChanges();
            }
        }

        public List<string> customerbasicdetail(List<string> _amendment, FrayteCustomer newcustomer, FrayteCustomer customerDetail, FrayteCustomer frayteUser)
        {
            _amendment = new List<string>();
            if (customerDetail.AccountNumber != frayteUser.AccountNumber)
            {
                newcustomer.AccountNumber = frayteUser.AccountNumber;
                _amendment.Add("Account Number|" + customerDetail.AccountNumber + " changed to " + newcustomer.AccountNumber);
            }
            if (customerDetail.CompanyName != frayteUser.CompanyName)
            {
                newcustomer.CompanyName = frayteUser.CompanyName;
                _amendment.Add("Company Name|" + customerDetail.CompanyName + " changed to " + newcustomer.CompanyName);
            }

            if (customerDetail.ContactName != frayteUser.ContactName)
            {
                newcustomer.ContactName = frayteUser.ContactName;
                _amendment.Add("Contact Name|" + customerDetail.ContactName + " changed to " + newcustomer.ContactName);
            }
            if (customerDetail.Email != frayteUser.Email)
            {
                newcustomer.Email = frayteUser.Email;
                _amendment.Add("Email|" + customerDetail.Email + " changed to " + newcustomer.Email);
            }
            if (customerDetail.TelephoneNo != frayteUser.TelephoneNo)
            {
                newcustomer.TelephoneNo = frayteUser.TelephoneNo;
                _amendment.Add("Telephone No|" + customerDetail.TelephoneNo + " changed to " + newcustomer.TelephoneNo);
            }
            if (customerDetail.FaxNumber != frayteUser.FaxNumber)
            {
                newcustomer.FaxNumber = frayteUser.FaxNumber;
                _amendment.Add("Fax Number|" + customerDetail.FaxNumber + " changed to " + newcustomer.FaxNumber);
            }
            if (customerDetail.AccountName != frayteUser.AccountName)
            {
                newcustomer.AccountName = frayteUser.AccountName;
                _amendment.Add("Account Number|" + customerDetail.AccountName + " changed to" + newcustomer.AccountNumber);
            }
            if (customerDetail.AccountMail != frayteUser.AccountMail)
            {
                newcustomer.AccountMail = frayteUser.AccountMail;
                _amendment.Add("Account Email|" + customerDetail.AccountMail + " changed to " + newcustomer.AccountMail);
            }
            if (customerDetail.WorkingStartTime.ToString("HH:mm") != frayteUser.WorkingStartTime.ToString("HH:mm"))
            {
                newcustomer.WorkingStartTime = frayteUser.WorkingStartTime;
                string starttime = newcustomer.WorkingStartTime.TimeOfDay.ToString();
                _amendment.Add("Working Start Time|" + customerDetail.WorkingStartTime.ToString("HH:mm") + " changed to " + starttime.Substring(0, 5));
            }
            if (customerDetail.WorkingEndTime.ToString("HH:mm") != frayteUser.WorkingEndTime.ToString("HH:mm"))
            {
                newcustomer.WorkingEndTime = frayteUser.WorkingEndTime;
                string endtime = newcustomer.WorkingEndTime.TimeOfDay.ToString();
                _amendment.Add("Working End Time|" + customerDetail.WorkingEndTime.ToString("HH:mm") + " changed to " + endtime.Substring(0, 5));
            }
            if (customerDetail.VATGST != frayteUser.VATGST)
            {
                newcustomer.VATGST = frayteUser.VATGST;
                _amendment.Add("VATGST|" + customerDetail.VATGST + " changed to " + newcustomer.VATGST);
            }
            if (customerDetail.WorkingWeekDay.Description != frayteUser.WorkingWeekDay.Description)
            {
                newcustomer.WorkingWeekDay = frayteUser.WorkingWeekDay;
                _amendment.Add("Working Week Day|" + customerDetail.WorkingWeekDay.Description + " changed to " + newcustomer.WorkingWeekDay.Description);
            }
            if (customerDetail.CreditLimitCurrencyCode != frayteUser.CreditLimitCurrencyCode)
            {
                newcustomer.CreditLimitCurrencyCode = frayteUser.CreditLimitCurrencyCode;
                _amendment.Add("Credit Limit Currency|" + customerDetail.CreditLimitCurrencyCode + " changed to " + newcustomer.CreditLimitCurrencyCode);
            }
            if (customerDetail.CreditLimit != frayteUser.CreditLimit)
            {
                newcustomer.CreditLimit = frayteUser.CreditLimit;
                _amendment.Add("Credit Limit|" + customerDetail.CreditLimit + " changed to " + newcustomer.CreditLimit);
            }
            if (customerDetail.Timezone.Name != frayteUser.Timezone.Name)
            {
                newcustomer.Timezone = frayteUser.Timezone;
                _amendment.Add("Time zone|" + customerDetail.Timezone.Name + " changed to " + newcustomer.Timezone.Name);
            }
            if (customerDetail.TermsOfPayment != frayteUser.TermsOfPayment)
            {
                newcustomer.TermsOfPayment = frayteUser.TermsOfPayment;
                _amendment.Add("Terms Of Payment|" + customerDetail.TermsOfPayment + " changed to " + newcustomer.TermsOfPayment);
            }
            if (customerDetail.TaxAndDuties != frayteUser.TaxAndDuties)
            {
                newcustomer.TaxAndDuties = frayteUser.TaxAndDuties;
                _amendment.Add("Tax And Duties|" + customerDetail.TaxAndDuties + " changed to " + newcustomer.TaxAndDuties);
            }
            if (customerDetail.IsDirectBooking != frayteUser.IsDirectBooking)
            {
                newcustomer.IsDirectBooking = frayteUser.IsDirectBooking;
                _amendment.Add("Direct Booking|" + (customerDetail.IsDirectBooking == true ? "Assign" : "Not Assign") + " changed to " + (newcustomer.IsDirectBooking == true ? "Assign" : "Not Assign"));
            }
            if (customerDetail.IsTradeLaneBooking != frayteUser.IsTradeLaneBooking)
            {
                newcustomer.IsTradeLaneBooking = frayteUser.IsTradeLaneBooking;
                _amendment.Add("Tradelane Booking|" + (customerDetail.IsTradeLaneBooking == true ? "Assign" : "Not Assign") + " changed to " + (newcustomer.IsTradeLaneBooking == true ? "Assign" : "Not Assign"));
            }
            if (customerDetail.IsBreakBulkBooking != frayteUser.IsBreakBulkBooking)
            {
                newcustomer.IsBreakBulkBooking = frayteUser.IsBreakBulkBooking;
                _amendment.Add("Breakbulk Booking|" + (customerDetail.IsBreakBulkBooking == true ? "Assign" : "Not Assign") + " changed to " + (newcustomer.IsBreakBulkBooking == true ? "Assign" : "Not Assign"));
            }
            if (customerDetail.IsECommerce != frayteUser.IsECommerce)
            {
                newcustomer.IsECommerce = frayteUser.IsECommerce;
                _amendment.Add("ECommerce|" + (customerDetail.IsECommerce == true ? "Assign" : "Not Assign") + " changed to " + (newcustomer.IsECommerce == true ? "Assign" : "Not Assign"));
            }
            if (customerDetail.IsFuelSurcharge != frayteUser.IsFuelSurcharge)
            {
                newcustomer.IsFuelSurcharge = frayteUser.IsFuelSurcharge;
                _amendment.Add("Fuel Surcharge Allow|" + (customerDetail.IsFuelSurcharge == true ? "Assign" : "Not Assign") + " changed to " + (newcustomer.IsFuelSurcharge == true ? "Assign" : "Not Assign"));
            }
            if (customerDetail.IsCurrency != frayteUser.IsCurrency)
            {
                newcustomer.IsCurrency = frayteUser.IsCurrency;
                _amendment.Add("Currency Allow|" + (customerDetail.IsCurrency == true ? "Assign" : "Not Assign") + " changed to " + (newcustomer.IsCurrency == true ? "Assign" : "Not Assign"));
            }
            if (customerDetail.IsShipperTaxAndDuty != frayteUser.IsShipperTaxAndDuty)
            {
                newcustomer.IsShipperTaxAndDuty = frayteUser.IsShipperTaxAndDuty;
                _amendment.Add("Shipper Tax And Duty|" + (customerDetail.IsShipperTaxAndDuty == true ? "Assign" : "Not Assign") + " changed to " + (newcustomer.IsShipperTaxAndDuty == true ? "Assign" : "Not Assign"));
            }
            if (customerDetail.IsAllowRate != frayteUser.IsAllowRate)
            {
                newcustomer.IsShipperTaxAndDuty = frayteUser.IsAllowRate;
                _amendment.Add("Allow Rate|" + (customerDetail.IsAllowRate == true ? "Assign" : "Not Assign") + " changed to " + (newcustomer.IsAllowRate == true ? "Assign" : "Not Assign"));
            }
            if (customerDetail.IsExpressSolutions != frayteUser.IsExpressSolutions)
            {
                newcustomer.IsExpressSolutions = frayteUser.IsExpressSolutions;
                _amendment.Add("Express Solution Allow|" + (customerDetail.IsExpressSolutions == true ? "Assign" : "Not Assign") + " changed to " + (newcustomer.IsExpressSolutions == true ? "Assign" : "Not Assign"));
            }
            if (customerDetail.IsWarehouseTransport != frayteUser.IsWarehouseTransport)
            {
                newcustomer.IsWarehouseTransport = frayteUser.IsWarehouseTransport;
                _amendment.Add("Warehouse Transport Allow|" + (customerDetail.IsWarehouseTransport == true ? "Assign" : "Not Assign") + " changed to " + (newcustomer.IsWarehouseTransport == true ? "Assign" : "Not Assign"));
            }
            if (customerDetail.DaysValidity != frayteUser.DaysValidity)
            {
                newcustomer.DaysValidity = frayteUser.DaysValidity;
                _amendment.Add("Quotation Days Validity|" + (customerDetail.DaysValidity == 1 ? customerDetail.DaysValidity + " Day" : customerDetail.DaysValidity + " Days") + " changed to " + (newcustomer.DaysValidity == 1 ? newcustomer.DaysValidity + " Day" : newcustomer.DaysValidity + " Days"));
            }
            if (customerDetail.CustomerRateCardType != frayteUser.CustomerRateCardType)
            {
                newcustomer.CustomerRateCardType = frayteUser.CustomerRateCardType;
                _amendment.Add("Customer Rate Card Type|" + customerDetail.CustomerRateCardType + " changed to " + newcustomer.CustomerRateCardType);
            }
            if (customerDetail.CustomerType != frayteUser.CustomerType)
            {
                newcustomer.CustomerType = frayteUser.CustomerType;
                _amendment.Add("Customer Type|" + customerDetail.CustomerType + " changed to " + newcustomer.CustomerType);
            }
            if (customerDetail.IsApiAllow != frayteUser.IsApiAllow)
            {
                newcustomer.IsApiAllow = frayteUser.IsApiAllow;
                _amendment.Add("API Allow|" + (customerDetail.IsApiAllow == true ? "Assign" : "Not Assign") + " changed to " + (newcustomer.IsApiAllow == true ? "Assign" : "Not Assign"));
            }
            if (customerDetail.UserType != frayteUser.UserType)
            {
                newcustomer.UserType = frayteUser.UserType;
                _amendment.Add("User Type|" + customerDetail.UserType + " changed to " + newcustomer.UserType);
            }
            return _amendment;
        }

        public List<string> billingaddress(List<string> _amendment, FrayteCustomer newcustomer, FrayteCustomer customerDetail, FrayteCustomer frayteUser)
        {
            _amendment = new List<string>();
            newcustomer.UserAddress = new FrayteAddress();
            if (customerDetail.UserAddress != null && frayteUser.UserAddress != null && customerDetail.UserAddress.Country.Name != frayteUser.UserAddress.Country.Name)
            {
                newcustomer.UserAddress.Country = frayteUser.UserAddress.Country;
                _amendment.Add("Counrty|" + customerDetail.UserAddress.Country.Name + " changed to " + newcustomer.UserAddress.Country.Name);
            }
            if (customerDetail.UserAddress != null && frayteUser.UserAddress != null && customerDetail.UserAddress.Address != frayteUser.UserAddress.Address)
            {
                newcustomer.UserAddress.Address = frayteUser.UserAddress.Address;
                _amendment.Add("Address|" + customerDetail.UserAddress.Address + " changed to " + newcustomer.UserAddress.Address);
            }
            if (customerDetail.UserAddress != null && frayteUser.UserAddress != null && customerDetail.UserAddress.Address2 != frayteUser.UserAddress.Address2)
            {
                newcustomer.UserAddress.Address2 = frayteUser.UserAddress.Address2;
                _amendment.Add("Address2|" + customerDetail.UserAddress.Address2 + " changed to " + newcustomer.UserAddress.Address2);
            }
            if (customerDetail.UserAddress != null && frayteUser.UserAddress != null && customerDetail.UserAddress.Address3 != frayteUser.UserAddress.Address3)
            {
                newcustomer.UserAddress.Address3 = frayteUser.UserAddress.Address3;
                _amendment.Add("Address3|" + customerDetail.UserAddress.Address3 + " changed to " + newcustomer.UserAddress.Address3);
            }
            if (customerDetail.UserAddress != null && frayteUser.UserAddress != null && customerDetail.UserAddress.City != frayteUser.UserAddress.City)
            {
                newcustomer.UserAddress.City = frayteUser.UserAddress.City;
                _amendment.Add("City|" + customerDetail.UserAddress.City + " changed to " + newcustomer.UserAddress.City);
            }
            if (customerDetail.UserAddress != null && frayteUser.UserAddress != null && customerDetail.UserAddress.Suburb != frayteUser.UserAddress.Suburb)
            {
                newcustomer.UserAddress.Suburb = frayteUser.UserAddress.Suburb;
                _amendment.Add("Suburb|" + customerDetail.UserAddress.Suburb + " changed to " + newcustomer.UserAddress.Suburb);
            }
            if (customerDetail.UserAddress != null && frayteUser.UserAddress != null && customerDetail.UserAddress.State != frayteUser.UserAddress.State)
            {
                newcustomer.UserAddress.State = frayteUser.UserAddress.State;
                _amendment.Add("State|" + customerDetail.UserAddress.State + " changed to " + newcustomer.UserAddress.State);
            }
            if (customerDetail.UserAddress != null && frayteUser.UserAddress != null && customerDetail.UserAddress.Zip != frayteUser.UserAddress.Zip)
            {
                newcustomer.UserAddress.Zip = frayteUser.UserAddress.Zip;
                _amendment.Add("Zip|" + customerDetail.UserAddress.Zip + " changed to " + newcustomer.UserAddress.Zip);
            }
            return _amendment;
        }

        public List<string> frayteassociateuser(List<string> _amendment, FrayteCustomer newcustomer, FrayteCustomer customerDetail, FrayteCustomer frayteUser)
        {
            _amendment = new List<string>();
            newcustomer.ManagerUser = new FrayteCustomerAssociatedUser();
            newcustomer.DocumentUser = new FrayteCustomerAssociatedUser();
            newcustomer.AccountUser = new FrayteCustomerAssociatedUser();
            newcustomer.OperationUser = new FrayteCustomerAssociatedUser();
            newcustomer.SalesRepresentative = new FrayteCustomerAssociatedUser();

            if (customerDetail.ManagerUser != null && frayteUser.ManagerUser != null && customerDetail.ManagerUser.UserId != frayteUser.ManagerUser.UserId)
            {
                newcustomer.ManagerUser = frayteUser.ManagerUser;
                _amendment.Add("Manager User|" + customerDetail.ManagerUser.ContactName + " changed to " + newcustomer.ManagerUser.ContactName);
            }
            if (customerDetail.ManagerUser == null && frayteUser.ManagerUser != null)
            {
                newcustomer.ManagerUser = frayteUser.ManagerUser;
                _amendment.Add("Manager User|" + customerDetail.ManagerUser.ContactName + " changed to " + newcustomer.ManagerUser.ContactName);
            }
            if (customerDetail.ManagerUser != null && frayteUser.ManagerUser == null)
            {
                newcustomer.ManagerUser = customerDetail.ManagerUser;
                _amendment.Add("Manager User|" + customerDetail.ManagerUser.ContactName + " changed to " + newcustomer.ManagerUser.ContactName);
            }
            if (customerDetail.DocumentUser != null && frayteUser.DocumentUser != null && customerDetail.DocumentUser.UserId != frayteUser.DocumentUser.UserId)
            {
                newcustomer.DocumentUser = frayteUser.DocumentUser;
                _amendment.Add("Document User|" + customerDetail.DocumentUser.ContactName + " changed to " + newcustomer.DocumentUser.ContactName);
            }
            if (customerDetail.DocumentUser == null && frayteUser.DocumentUser != null)
            {
                newcustomer.DocumentUser = frayteUser.DocumentUser;
                _amendment.Add("Document User|" + customerDetail.DocumentUser.ContactName + " changed to " + newcustomer.DocumentUser.ContactName);
            }
            if (customerDetail.DocumentUser != null && frayteUser.DocumentUser == null)
            {
                newcustomer.DocumentUser = customerDetail.DocumentUser;
                _amendment.Add("Document User|" + customerDetail.DocumentUser.ContactName + " changed to " + newcustomer.DocumentUser.ContactName);
            }
            if (customerDetail.AccountUser != null && frayteUser.AccountUser != null && customerDetail.AccountUser.UserId != frayteUser.AccountUser.UserId)
            {
                newcustomer.AccountUser = frayteUser.AccountUser;
                _amendment.Add("Account User|" + customerDetail.AccountUser.ContactName + " changed to " + newcustomer.AccountUser.ContactName);
            }
            if (customerDetail.AccountUser == null && frayteUser.AccountUser != null)
            {
                newcustomer.AccountUser = frayteUser.AccountUser;
                _amendment.Add("Account User|" + customerDetail.AccountUser.ContactName + " changed to " + newcustomer.AccountUser.ContactName);
            }
            if (customerDetail.AccountUser != null && frayteUser.AccountUser == null)
            {
                newcustomer.AccountUser = customerDetail.AccountUser;
                _amendment.Add("Account User|" + customerDetail.AccountUser.ContactName + " changed to " + newcustomer.AccountUser.ContactName);
            }
            if (customerDetail.OperationUser != null && frayteUser.OperationUser != null && customerDetail.OperationUser.UserId != frayteUser.OperationUser.UserId)
            {
                newcustomer.OperationUser = frayteUser.OperationUser;
                _amendment.Add("Operation User|" + customerDetail.OperationUser.ContactName + " changed to " + newcustomer.OperationUser.ContactName);
            }
            if (customerDetail.SalesRepresentative != null && frayteUser.SalesRepresentative != null && customerDetail.SalesRepresentative.UserId != frayteUser.SalesRepresentative.UserId)
            {
                newcustomer.SalesRepresentative = frayteUser.SalesRepresentative;
                _amendment.Add("Sales Representative|" + customerDetail.SalesRepresentative.ContactName + " changed to " + newcustomer.SalesRepresentative.ContactName);
            }
            if (customerDetail.SalesRepresentative == null && frayteUser.SalesRepresentative != null)
            {
                newcustomer.SalesRepresentative = frayteUser.SalesRepresentative;
                _amendment.Add("Sales Representative|" + customerDetail.SalesRepresentative.ContactName + " changed to " + newcustomer.SalesRepresentative.ContactName);
            }
            if (customerDetail.SalesRepresentative != null && frayteUser.SalesRepresentative == null)
            {
                newcustomer.SalesRepresentative = customerDetail.SalesRepresentative;
                _amendment.Add("Sales Representative|" + customerDetail.SalesRepresentative.ContactName + " changed to " + newcustomer.SalesRepresentative.ContactName);
            }
            return _amendment;
        }

        public List<string> serviceoption(List<string> _amendment, FrayteCustomer newcustomer, FrayteCustomer customerDetail, FrayteCustomer frayteUser)
        {
            _amendment = new List<string>();
            if (customerDetail.IsDirectBooking != frayteUser.IsDirectBooking)
            {
                newcustomer.IsDirectBooking = frayteUser.IsDirectBooking;
                _amendment.Add("Direct Booking|" + (customerDetail.IsDirectBooking == true ? "Assign" : "Not Assign") + " changed to " + (newcustomer.IsDirectBooking == true ? "Assign" : "Not Assign"));
            }
            if (customerDetail.IsTradeLaneBooking != frayteUser.IsTradeLaneBooking)
            {
                newcustomer.IsTradeLaneBooking = frayteUser.IsTradeLaneBooking;
                _amendment.Add("Trade Lane Booking|" + (customerDetail.IsTradeLaneBooking == true ? "Assign" : "Not Assign") + " changed to " + (newcustomer.IsTradeLaneBooking == true ? "Assign" : "Not Assign"));
            }
            if (customerDetail.IsBreakBulkBooking != frayteUser.IsBreakBulkBooking)
            {
                newcustomer.IsBreakBulkBooking = frayteUser.IsBreakBulkBooking;
                _amendment.Add("Brak Bulk Booking|" + (customerDetail.IsBreakBulkBooking == true ? "Assign" : "Not Assign") + " changed to " + (newcustomer.IsBreakBulkBooking == true ? "Assign" : "Not Assign"));
            }
            return _amendment;
        }

        public List<string> proofofdelivery(List<string> _amendment, FrayteCustomer newcustomer, FrayteCustomer customerDetail, FrayteCustomer frayteUser)
        {
            _amendment = new List<string>();
            newcustomer.CustomerPODSetting = new FrayteCustomerSetting();
            if (customerDetail.CustomerPODSetting.ScheduleSetting != frayteUser.CustomerPODSetting.ScheduleSetting)
            {
                newcustomer.CustomerPODSetting.ScheduleSetting = frayteUser.CustomerPODSetting.ScheduleSetting;
                _amendment.Add("Schedule Setting|" + (!string.IsNullOrEmpty(customerDetail.CustomerPODSetting.ScheduleSetting) ? customerDetail.CustomerPODSetting.ScheduleSetting : "Not Assign") + " changed to " + (!string.IsNullOrEmpty(newcustomer.CustomerPODSetting.ScheduleSetting) ? newcustomer.CustomerPODSetting.ScheduleSetting : "Not Assign"));
            }
            if (customerDetail.CustomerPODSetting.ScheduleType != frayteUser.CustomerPODSetting.ScheduleType)
            {
                newcustomer.CustomerPODSetting.ScheduleType = frayteUser.CustomerPODSetting.ScheduleType;
                _amendment.Add("Schedule Type|" + (!string.IsNullOrEmpty(customerDetail.CustomerPODSetting.ScheduleType) ? customerDetail.CustomerPODSetting.ScheduleType : "Not Assign") + " changed to " + (!string.IsNullOrEmpty(newcustomer.CustomerPODSetting.ScheduleType) ? newcustomer.CustomerPODSetting.ScheduleType : "Not Assign"));
            }
            if (customerDetail.CustomerPODSetting.ScheduleDate != frayteUser.CustomerPODSetting.ScheduleDate)
            {
                newcustomer.CustomerPODSetting.ScheduleDate = frayteUser.CustomerPODSetting.ScheduleDate;
                _amendment.Add("Schedule Date|" + (customerDetail.CustomerPODSetting.ScheduleDate.HasValue ? customerDetail.CustomerPODSetting.ScheduleDate.Value.ToString("dd-MMM-yyyy") : "Not Assign") + " changed to " + (newcustomer.CustomerPODSetting.ScheduleDate.HasValue ? newcustomer.CustomerPODSetting.ScheduleDate.Value.ToString("dd-MMM-yyyy") : "Not Assign"));
            }
            if (customerDetail.CustomerPODSetting.ScheduleTime != frayteUser.CustomerPODSetting.ScheduleTime)
            {
                newcustomer.CustomerPODSetting.ScheduleTime = frayteUser.CustomerPODSetting.ScheduleTime;
                _amendment.Add("Schedule Time|" + (!string.IsNullOrEmpty(customerDetail.CustomerPODSetting.ScheduleTime) ? customerDetail.CustomerPODSetting.ScheduleTime : "Not Assign") + " changed to " + (!string.IsNullOrEmpty(newcustomer.CustomerPODSetting.ScheduleTime) ? newcustomer.CustomerPODSetting.ScheduleTime : "Not Assign"));
            }
            if (customerDetail.CustomerPODSetting.ScheduleDay != frayteUser.CustomerPODSetting.ScheduleDay)
            {
                newcustomer.CustomerPODSetting.ScheduleDay = frayteUser.CustomerPODSetting.ScheduleDay;
                _amendment.Add("Schedule Day|" + (!string.IsNullOrEmpty(customerDetail.CustomerPODSetting.ScheduleDay) ? customerDetail.CustomerPODSetting.ScheduleDay : "Not Assign") + " changed to " + (!string.IsNullOrEmpty(newcustomer.CustomerPODSetting.ScheduleDay) ? newcustomer.CustomerPODSetting.ScheduleDay : "Not Assign"));
            }
            if (customerDetail.CustomerPODSetting.AdditionalMails != frayteUser.CustomerPODSetting.AdditionalMails)
            {
                newcustomer.CustomerPODSetting.AdditionalMails = frayteUser.CustomerPODSetting.AdditionalMails;
                _amendment.Add("Additional Mails|" + (!string.IsNullOrEmpty(customerDetail.CustomerPODSetting.AdditionalMails) ? customerDetail.CustomerPODSetting.AdditionalMails : "Not Assign") + " changed to " + (!string.IsNullOrEmpty(newcustomer.CustomerPODSetting.AdditionalMails) ? newcustomer.CustomerPODSetting.AdditionalMails : "Not Assign"));
            }
            return _amendment;
        }

        public List<string> ratecardschedule(List<string> _amendment, FrayteCustomer newcustomer, FrayteCustomer customerDetail, FrayteCustomer frayteUser)
        {
            _amendment = new List<string>();
            newcustomer.CustomerRateSetting = new FrayteCustomerSetting();
            if (customerDetail.CustomerRateSetting.ScheduleType != frayteUser.CustomerRateSetting.ScheduleType)
            {
                newcustomer.CustomerRateSetting.ScheduleType = frayteUser.CustomerRateSetting.ScheduleType;
                _amendment.Add("Schedule Type|" + customerDetail.CustomerRateSetting.ScheduleType + " changed to " + newcustomer.CustomerRateSetting.ScheduleType);
            }
            if (customerDetail.CustomerRateSetting.ScheduleDate != frayteUser.CustomerRateSetting.ScheduleDate)
            {
                newcustomer.CustomerRateSetting.ScheduleDate = frayteUser.CustomerRateSetting.ScheduleDate;
                _amendment.Add("Schedule Date|" + customerDetail.CustomerRateSetting.ScheduleDate.Value.ToString("dd-MMM-yyyy") + " changed to " + newcustomer.CustomerRateSetting.ScheduleDate.Value.ToString("dd-MMM-yyyy"));
            }
            if (customerDetail.CustomerRateSetting.ScheduleTime != frayteUser.CustomerRateSetting.ScheduleTime)
            {
                newcustomer.CustomerRateSetting.ScheduleTime = frayteUser.CustomerRateSetting.ScheduleTime;
                _amendment.Add("ScheduleTime|" + customerDetail.CustomerRateSetting.ScheduleTime + " changed to " + newcustomer.CustomerRateSetting.ScheduleTime);
            }
            if (customerDetail.CustomerRateSetting.ScheduleDay != frayteUser.CustomerRateSetting.ScheduleDay)
            {
                newcustomer.CustomerRateSetting.ScheduleDay = frayteUser.CustomerRateSetting.ScheduleDay;
                _amendment.Add("ScheduleDay|" + customerDetail.CustomerRateSetting.ScheduleDay + " changed to " + newcustomer.CustomerRateSetting.ScheduleDay);
            }
            if (customerDetail.CustomerRateSetting.AdditionalMails != frayteUser.CustomerRateSetting.AdditionalMails)
            {
                newcustomer.CustomerRateSetting.AdditionalMails = frayteUser.CustomerRateSetting.AdditionalMails;
                _amendment.Add("Additional Mails|" + customerDetail.CustomerRateSetting.AdditionalMails + " changed to " + newcustomer.CustomerRateSetting.AdditionalMails);
            }
            return _amendment;
        }

        public List<string> UpdatedCustomerDetail(List<string> _amendment, FrayteCustomer newcustomer, FrayteCustomer customerDetail, FrayteCustomer frayteUser)
        {
            List<string> _amen1 = customerbasicdetail(_amendment, newcustomer, customerDetail, frayteUser);
            List<string> _amen2 = billingaddress(_amendment, newcustomer, customerDetail, frayteUser);
            List<string> _amen3 = frayteassociateuser(_amendment, newcustomer, customerDetail, frayteUser);
            List<string> _amen4 = serviceoption(_amendment, newcustomer, customerDetail, frayteUser);
            List<string> _amen5 = proofofdelivery(_amendment, newcustomer, customerDetail, frayteUser);
            _amendment = _amen1.Concat(_amen2).Concat(_amen3).Concat(_amen4).Concat(_amen5).ToList();
            return _amendment;
        }

        public FrayteResult SaveCustomer(FrayteCustomer frayteUser)
        {
            FrayteResult result = new FrayteResult();

            FrayteUserRepository userRepository = new FrayteUserRepository();
            //Step 1: Save User Detail
            userRepository.SaveUserDetail(frayteUser);

            //Step 2: Save user customer detail
            SaveCustomerAdditional(frayteUser);

            //Step 3: Save user role
            userRepository.SaveUserRole(frayteUser.UserId, (int)FrayteUserRole.Customer);

            //Step 4: Save Customer Address information
            frayteUser.UserAddress.AddressTypeId = (int)FrayteAddressType.MainAddress;
            frayteUser.UserAddress.UserId = frayteUser.UserId;
            userRepository.SaveUserAddress(frayteUser.UserAddress);

            //Step 5: Save Customer other address information
            if (frayteUser.OtherAddresses != null && frayteUser.OtherAddresses.Count > 0)
            {
                foreach (FrayteAddress address in frayteUser.OtherAddresses)
                {
                    address.AddressTypeId = (int)FrayteAddressType.OtherAddress;
                    address.UserId = frayteUser.UserId;
                    userRepository.SaveUserAddress(address);
                }
            }

            //Step 6: Upload/Edit Customer Margin Cost
            // EditCustomerMarginCost(frayteUser.CustomerMargin, frayteUser.UserId);

            //Step 7: Save/Edit Customer POD Setting
            SaveCustomerSetting(frayteUser.CustomerPODSetting, frayteUser.UserId, frayteUser.Timezone);

            //Step 8: Save Customer Rate Setting
            //SaveCustomerSetting(frayteUser.CustomerRateSetting, frayteUser.UserId);

            //Step 9: Save Trade lane information
            if (frayteUser.Tradelanes != null && frayteUser.Tradelanes.Count > 0)
            {
                foreach (FrayteTradelane tradelane in frayteUser.Tradelanes)
                {
                    //9.1 : First save tradelane information
                    new TradelaneRepository().SaveTradelane(tradelane);

                    //9.2 : Save Tradelane and Customer (UserId) relation
                    CustomerTradeLane tradeLaneResult = dbContext.CustomerTradeLanes.Where(p => p.TradeLaneId == tradelane.TradelaneId && p.UserId == frayteUser.UserId).FirstOrDefault();
                    if (tradeLaneResult == null)
                    {
                        tradeLaneResult = new CustomerTradeLane();
                        tradeLaneResult.TradeLaneId = tradelane.TradelaneId;
                        tradeLaneResult.UserId = frayteUser.UserId;
                        dbContext.CustomerTradeLanes.Add(tradeLaneResult);
                        dbContext.SaveChanges();
                    }
                }
            }

            result.Status = true;
            return result;
        }

        #endregion

        public FrayteResult DeleteCustomer(int customerId)
        {
            return new FrayteUserRepository().MarkForDelete(customerId);
        }

        public bool CheckValidExcel(DataTable table)
        {
            bool valid = true;

            DataColumnCollection columns = table.Columns;

            if (!columns.Contains("CompanyName"))
            {
                valid = false;
            }
            if (!columns.Contains("ContactName"))
            {
                valid = false;
            }
            if (!columns.Contains("ShortName"))
            {
                valid = false;
            }
            if (!columns.Contains("Email"))
            {
                valid = false;
            }
            if (!columns.Contains("TelephoneNo"))
            {
                valid = false;
            }
            if (!columns.Contains("WorkingStartTime"))
            {
                valid = false;
            }
            if (!columns.Contains("WorkingEndTime"))
            {
                valid = false;
            }
            if (!columns.Contains("WorkingWeekDay"))
            {
                valid = false;
            }
            if (!columns.Contains("Timezone"))
            {
                valid = false;
            }
            if (!columns.Contains("BillingAddress"))
            {
                valid = false;
            }
            if (!columns.Contains("BillingCity"))
            {
                valid = false;
            }
            if (!columns.Contains("BillingState"))
            {
                valid = false;
            }
            if (!columns.Contains("BillingZip"))
            {
                valid = false;
            }
            if (!columns.Contains("BillingCountry"))
            {
                valid = false;
            }
            if (!columns.Contains("AccountName"))
            {
                valid = false;
            }
            if (!columns.Contains("AccountMail"))
            {
                valid = false;
            }
            if (!columns.Contains("CreditLimit"))
            {
                valid = false;
            }
            if (!columns.Contains("TermsOfPayment"))
            {
                valid = false;
            }
            if (!columns.Contains("OperationStaffUser"))
            {
                valid = false;
            }
            return valid;
        }

        public List<FrayteCustomer> GetAllCustomers(DataTable exceldata)
        {
            List<FrayteCustomer> customers = new List<FrayteCustomer>();

            FrayteCustomer customer;

            foreach (DataRow shipmentdetail in exceldata.Rows)
            {
                customer = new FrayteCustomer();

                customer.UserId = 0;
                customer.RoleId = (int)FrayteUserRole.Agent;
                customer.CompanyName = shipmentdetail["CompanyName"].ToString();
                customer.ContactName = shipmentdetail["ContactName"].ToString();
                customer.ShortName = shipmentdetail["ShortName"].ToString();
                customer.Email = shipmentdetail["Email"].ToString();

                customer.TelephoneNo = shipmentdetail["TelephoneNo"].ToString();
                customer.MobileNo = shipmentdetail["MobileNo"].ToString();

                customer.FaxNumber = shipmentdetail["FaxNumber"].ToString();
                customer.WorkingStartTime = Convert.ToDateTime(shipmentdetail["WorkingStartTime"]);
                customer.WorkingEndTime = Convert.ToDateTime(shipmentdetail["WorkingEndTime"]);
                customer.WorkingWeekDay = new WorkingWeekDay();
                string workingDay = shipmentdetail["WorkingWeekDay"].ToString();
                var workingWeekDayResult = dbContext.WorkingWeekDays.Where(p => p.Description == workingDay).FirstOrDefault();
                if (workingWeekDayResult != null)
                {
                    customer.WorkingWeekDay = workingWeekDayResult;
                }
                else
                {
                    customer.WorkingWeekDay.Description = shipmentdetail["WorkingWeekDay"].ToString();
                }

                customer.Timezone = new TimeZoneModal();
                string weekTimezone = shipmentdetail["Timezone"].ToString();
                var timeZoneResult = dbContext.Timezones.Where(p => p.Name == weekTimezone).FirstOrDefault();
                if (timeZoneResult != null)
                {
                    customer.Timezone.TimezoneId = timeZoneResult.TimezoneId;
                    customer.Timezone.Name = timeZoneResult.Name;
                    customer.Timezone.Offset = timeZoneResult.Offset;
                    customer.Timezone.OffsetShort = timeZoneResult.OffsetShort;
                }
                else
                {
                    customer.Timezone.Name = shipmentdetail["Timezone"].ToString();
                }

                customer.VATGST = shipmentdetail["VATGST"].ToString();
                customer.CreatedOn = DateTime.UtcNow;

                FrayteAddress customerAddress = new FrayteAddress();

                customerAddress.Address = shipmentdetail["BillingAddress"].ToString();
                customerAddress.Address2 = shipmentdetail["BillingAddress2"].ToString();
                customerAddress.Address3 = shipmentdetail["BillingAddress3"].ToString();
                customerAddress.Suburb = shipmentdetail["BillingSuburb"].ToString();
                customerAddress.City = shipmentdetail["BillingCity"].ToString();
                customerAddress.State = shipmentdetail["BillingState"].ToString();
                customerAddress.Zip = shipmentdetail["BillingZip"].ToString();
                customer.AccountName = shipmentdetail["AccountName"].ToString();
                customer.AccountMail = shipmentdetail["AccountMail"].ToString();
                customer.CreditLimitCurrencyCode = shipmentdetail["CreditLimitCurrencyCode"].ToString();
                customer.CreditLimit = CommonConversion.ConvertToDecimal(shipmentdetail["CreditLimit"].ToString());
                customer.TermsOfPayment = shipmentdetail["TermsOfPayment"].ToString();
                customer.TaxAndDuties = shipmentdetail["TaxAndDuties"].ToString();
                customer.OperationUser = new FrayteCustomerAssociatedUser();
                string operationUser = shipmentdetail["OperationStaffUser"].ToString();
                var operationStaff = dbContext.Users.Where(p => p.ContactName == operationUser).FirstOrDefault();
                if (operationStaff != null)
                {
                    customer.OperationUser.UserId = operationStaff.UserId;
                }
                customerAddress.Country = new FrayteCountryCode();
                string countryName = shipmentdetail["BillingCountry"].ToString();
                var country = dbContext.Countries.Where(p => p.CountryName == countryName).FirstOrDefault();
                if (country != null)
                {
                    customerAddress.Country.CountryId = country.CountryId;
                    customerAddress.Country.Code = country.CountryCode;
                    customerAddress.Country.Name = country.CountryName;

                }
                else
                {
                    customerAddress.Country.Code = shipmentdetail["Country"].ToString();
                }

                customer.UserAddress = customerAddress;

                customers.Add(customer);
            }

            return customers;
        }

        private void GetAssociateUsersDetail(FrayteCustomer customerDetail, UserAdditional customer)
        {
            List<int> associateFrayteUserIds = new List<int>();
            if (customer.AccountUserId.HasValue)
            {
                associateFrayteUserIds.Add(customer.AccountUserId.Value);
            }

            if (customer.DocumentUserId.HasValue)
            {
                associateFrayteUserIds.Add(customer.DocumentUserId.Value);
            }

            if (customer.ManagerUserId.HasValue)
            {
                associateFrayteUserIds.Add(customer.ManagerUserId.Value);
            }

            if (customer.OperationUserId.HasValue)
            {
                associateFrayteUserIds.Add(customer.OperationUserId.Value);
            }
            if (customer.SalesUserId.HasValue)
            {
                associateFrayteUserIds.Add(customer.SalesUserId.Value);
            }

            if (associateFrayteUserIds.Count > 0)
            {
                var result = dbContext.Users.Where(p => associateFrayteUserIds.Contains(p.UserId)).ToList();

                if (result != null)
                {
                    if (customer.AccountUserId.HasValue)
                    {
                        User findUser = result.Where(p => p.UserId == customer.AccountUserId.Value).FirstOrDefault();
                        if (findUser != null)
                        {
                            var phonecode = (from ua in dbContext.UserAddresses
                                             join c in dbContext.Countries on ua.CountryId equals c.CountryId
                                             where ua.UserId == findUser.UserId
                                             select new
                                             {
                                                 PhoneCode = c.CountryPhoneCode
                                             }).FirstOrDefault();

                            FrayteCustomerAssociatedUser associateUser = new FrayteCustomerAssociatedUser();
                            associateUser.UserId = findUser.UserId;
                            associateUser.AssociateType = "Account";
                            associateUser.ContactName = findUser.ContactName;
                            associateUser.Email = findUser.Email;
                            associateUser.TelephoneNo = phonecode.PhoneCode == "" ? findUser.TelephoneNo : "(+" + phonecode.PhoneCode + ") " + findUser.TelephoneNo;
                            associateUser.WorkingHours = UtilityRepository.GetWorkingHours(findUser.WorkingStartTime, findUser.WorkingEndTime);

                            customerDetail.AccountUser = associateUser;
                        }
                    }

                    if (customer.DocumentUserId.HasValue)
                    {
                        User findUser = result.Where(p => p.UserId == customer.DocumentUserId.Value).FirstOrDefault();
                        if (findUser != null)
                        {
                            var phonecode = (from ua in dbContext.UserAddresses
                                             join c in dbContext.Countries on ua.CountryId equals c.CountryId
                                             where ua.UserId == findUser.UserId
                                             select new
                                             {
                                                 PhoneCode = c.CountryPhoneCode
                                             }).FirstOrDefault();

                            FrayteCustomerAssociatedUser associateUser = new FrayteCustomerAssociatedUser();
                            associateUser.UserId = findUser.UserId;
                            associateUser.AssociateType = "Document";
                            associateUser.ContactName = findUser.ContactName;
                            associateUser.Email = findUser.Email;
                            associateUser.TelephoneNo = phonecode.PhoneCode == "" ? findUser.TelephoneNo : "(+" + phonecode.PhoneCode + ") " + findUser.TelephoneNo;
                            associateUser.WorkingHours = UtilityRepository.GetWorkingHours(findUser.WorkingStartTime, findUser.WorkingEndTime);
                            customerDetail.DocumentUser = associateUser;
                        }
                    }

                    if (customer.ManagerUserId.HasValue)
                    {
                        User findUser = result.Where(p => p.UserId == customer.ManagerUserId.Value).FirstOrDefault();
                        if (findUser != null)
                        {
                            var phonecode = (from ua in dbContext.UserAddresses
                                             join c in dbContext.Countries on ua.CountryId equals c.CountryId
                                             where ua.UserId == findUser.UserId
                                             select new
                                             {
                                                 PhoneCode = c.CountryPhoneCode
                                             }).FirstOrDefault();

                            FrayteCustomerAssociatedUser associateUser = new FrayteCustomerAssociatedUser();
                            associateUser.UserId = findUser.UserId;
                            associateUser.AssociateType = "Manager";
                            associateUser.ContactName = findUser.ContactName;
                            associateUser.Email = findUser.Email;
                            associateUser.TelephoneNo = phonecode.PhoneCode == "" ? findUser.TelephoneNo : "(+" + phonecode.PhoneCode + ") " + findUser.TelephoneNo;
                            associateUser.WorkingHours = UtilityRepository.GetWorkingHours(findUser.WorkingStartTime, findUser.WorkingEndTime);
                            customerDetail.ManagerUser = associateUser;
                        }
                    }

                    if (customer.OperationUserId.HasValue)
                    {
                        User findUser = result.Where(p => p.UserId == customer.OperationUserId.Value).FirstOrDefault();
                        if (findUser != null)
                        {
                            var phonecode = (from ua in dbContext.UserAddresses
                                             join c in dbContext.Countries on ua.CountryId equals c.CountryId
                                             where ua.UserId == findUser.UserId
                                             select new
                                             {
                                                 PhoneCode = c.CountryPhoneCode
                                             }).FirstOrDefault();

                            FrayteCustomerAssociatedUser associateUser = new FrayteCustomerAssociatedUser();
                            associateUser.UserId = findUser.UserId;
                            associateUser.AssociateType = "Operation";
                            associateUser.ContactName = findUser.ContactName;
                            associateUser.Email = findUser.Email;
                            associateUser.TelephoneNo = phonecode.PhoneCode == "" ? findUser.TelephoneNo : "(+" + phonecode.PhoneCode + ") " + findUser.TelephoneNo;
                            associateUser.WorkingHours = UtilityRepository.GetWorkingHours(findUser.WorkingStartTime, findUser.WorkingEndTime);
                            associateUser.Position = findUser.Position;
                            associateUser.CompanyName = findUser.CompanyName;
                            customerDetail.OperationUser = associateUser;
                        }
                    }
                    if (customer.SalesUserId.HasValue)
                    {
                        User findUser = result.Where(p => p.UserId == customer.SalesUserId.Value).FirstOrDefault();
                        if (findUser != null)
                        {
                            var phonecode = (from ua in dbContext.UserAddresses
                                             join c in dbContext.Countries on ua.CountryId equals c.CountryId
                                             where ua.UserId == findUser.UserId
                                             select new
                                             {
                                                 PhoneCode = c.CountryPhoneCode
                                             }).FirstOrDefault();

                            FrayteCustomerAssociatedUser associateUser = new FrayteCustomerAssociatedUser();
                            associateUser.UserId = findUser.UserId;
                            associateUser.AssociateType = "Sales Representative";
                            associateUser.ContactName = findUser.ContactName;
                            associateUser.Email = findUser.Email;
                            associateUser.TelephoneNo = phonecode.PhoneCode == "" ? findUser.TelephoneNo : "(+" + phonecode.PhoneCode + ") " + findUser.TelephoneNo;
                            associateUser.WorkingHours = UtilityRepository.GetWorkingHours(findUser.WorkingStartTime, findUser.WorkingEndTime);
                            customerDetail.SalesRepresentative = associateUser;
                        }
                    }
                }
            }
        }

        public FrayteResult RemoveCustomerLogistic(int logisticServiceId, int userId)
        {
            FrayteResult result = new FrayteResult();
            var data = dbContext.CustomerLogistics.Where(p => p.UserId == userId && p.LogisticServiceId == logisticServiceId).FirstOrDefault();
            if (data != null)
            {
                dbContext.CustomerLogistics.Attach(data);
                dbContext.CustomerLogistics.Remove(data);
                dbContext.SaveChanges();
                result.Status = true;

                // Delete Customer Margin Cost
                var ratetype = dbContext.UserAdditionals.Where(p => p.UserId == userId).FirstOrDefault();
                if (ratetype != null)
                {
                    if (ratetype.CustomerRateCardType == FrayteCustomerRateCardType.NORMAL)
                    {
                        var logistic = (from ls in dbContext.LogisticServices
                                        join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                                        join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                        join lsw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lsw.LogisticServiceShipmentTypeId
                                        join cmc in dbContext.CustomerMarginCosts on lsw.LogisticServiceWeightId equals cmc.LogisticServiceWeightId
                                        where
                                              ls.LogisticServiceId == logisticServiceId &&
                                              cmc.CustomerId == userId &&
                                              cmc.LogisticServiceZoneId == lsz.LogisticServiceZoneId &&
                                              cmc.LogisticServiceWeightId == lsw.LogisticServiceWeightId &&
                                              (cmc.LogisticShipmentTypeId == 0 || cmc.LogisticShipmentTypeId == lsst.LogisticServiceShipmentTypeId)
                                        select
                                              cmc).ToList();


                        if (logistic != null && logistic.Count > 0)
                        {
                            dbContext.CustomerMarginCosts.RemoveRange(logistic);
                            dbContext.SaveChanges();
                        }
                    }
                    else if (ratetype.CustomerRateCardType == FrayteCustomerRateCardType.ADVANCE)
                    {
                        var logistic = (from ls in dbContext.LogisticServices
                                        join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                                        join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                        join lsw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lsw.LogisticServiceShipmentTypeId
                                        join camc in dbContext.CustomerAdvanceMarginCosts on lsw.LogisticServiceWeightId equals camc.LogisticServiceWeightId
                                        where
                                              ls.LogisticServiceId == logisticServiceId &&
                                              camc.CustomerId == userId &&
                                              camc.LogisticServiceZoneId == lsz.LogisticServiceZoneId &&
                                              camc.LogisticServiceWeightId == lsw.LogisticServiceWeightId &&
                                              camc.LogisticServiceShipmentTypeId == lsst.LogisticServiceShipmentTypeId
                                        select
                                              camc).ToList();

                        if (logistic != null && logistic.Count > 0)
                        {
                            dbContext.CustomerAdvanceMarginCosts.RemoveRange(logistic);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            else
            {
                result.Status = false;
            }
            return result;
        }

        public FrayteCustomerRateCard GetCustomerRateCardDetail(int userId)
        {
            FrayteCustomerRateCard rateCardDetail = new FrayteCustomerRateCard();

            var OpeartionZone = UtilityRepository.GetOperationZone();
            rateCardDetail.UserId = userId;
            rateCardDetail.OperationZoneId = OpeartionZone.OperationZoneId;
            rateCardDetail.RegistredServices = new List<FrayteRegistredServices>();
            var customerLogistics = dbContext.CustomerLogistics.Where(p => p.UserId == userId).ToList();
            FrayteRegistredServices service;
            if (customerLogistics != null && customerLogistics.Count > 0)
            {
                foreach (var data in customerLogistics)
                {
                    service = new FrayteRegistredServices();
                    service.LogisticServiceId = data.LogisticServiceId;
                    service.LogisticServiceType = (data.LogisticServiceType == null ? "" : data.LogisticServiceType);
                    rateCardDetail.RegistredServices.Add(service);
                }
            }

            rateCardDetail.CustomerRateSetting = GetCustomerSetting(userId, FrayteCustomerPODRateSetting.RateCard);
            return rateCardDetail;
        }

        public List<FrayteLogisticServiceItem> BusinessUnitLogisticServiceItems(int OperationZoneId, int RoleId, int CreatedBy)
        {
            List<FrayteLogisticServiceItem> list = new List<FrayteLogisticServiceItem>();

            if (RoleId == (int)FrayteUserRole.Admin || RoleId == (int)FrayteUserRole.Staff)
            {
                list = (from ls in dbContext.LogisticServices
                        where
                            ls.OperationZoneId == OperationZoneId &&
                            ls.IsActive == true
                        select new FrayteLogisticServiceItem
                        {
                            LogisticServiceId = ls.LogisticServiceId,
                            LogisticCompany = ls.LogisticCompany,
                            LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                            LogisticType = ls.LogisticType,
                            LogisticTypeDisplay = ls.LogisticTypeDisplay,
                            RateType = ls.RateType,
                            RateTypeDisplay = ls.RateTypeDisplay
                        }).ToList();
            }
            else if (RoleId == (int)FrayteUserRole.Customer)
            {
                list = (from ls in dbContext.LogisticServices
                        join cl in dbContext.CustomerLogistics on ls.LogisticServiceId equals cl.LogisticServiceId
                        where
                            cl.UserId == CreatedBy &&
                            ls.OperationZoneId == OperationZoneId &&
                            ls.IsActive == true
                        select new FrayteLogisticServiceItem
                        {
                            LogisticServiceId = ls.LogisticServiceId,
                            LogisticCompany = ls.LogisticCompany,
                            LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                            LogisticType = ls.LogisticType,
                            LogisticTypeDisplay = ls.LogisticTypeDisplay,
                            RateType = ls.RateType,
                            RateTypeDisplay = ls.RateTypeDisplay
                        }).ToList();
            }
            return list;
        }

        public FrayteResult SaveCustomerRateCardDetail(FrayteCustomerRateCard frayteCustomerRateCard)
        {
            FrayteResult result = new FrayteResult();

            if (frayteCustomerRateCard.RegistredServices != null && frayteCustomerRateCard.RegistredServices.Count > 0)
            {
                foreach (var data in frayteCustomerRateCard.RegistredServices)
                {
                    CustomerLogistic customerService = dbContext.CustomerLogistics.Where(p => p.LogisticServiceId == data.LogisticServiceId && p.UserId == frayteCustomerRateCard.UserId).FirstOrDefault();
                    if (customerService != null)
                    {
                        //no need to add logistic service if alerady exist but only Yodel need to updates
                        if (customerService.LogisticServiceId == 13)
                        {
                            customerService.UserId = frayteCustomerRateCard.UserId;
                            customerService.LogisticServiceId = data.LogisticServiceId;
                            customerService.LogisticServiceType = (data.LogisticServiceType == null ? null : data.LogisticServiceType);
                            dbContext.CustomerLogistics.Add(customerService);
                            dbContext.Entry(customerService).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        CustomerLogistic service = new CustomerLogistic();
                        service.UserId = frayteCustomerRateCard.UserId;
                        service.LogisticServiceId = data.LogisticServiceId;
                        service.LogisticServiceType = (data.LogisticServiceType == null ? null : data.LogisticServiceType);
                        dbContext.CustomerLogistics.Add(service);
                        dbContext.SaveChanges();
                    }
                }
            }

            if (frayteCustomerRateCard.CustomerRateSetting != null)
            {
                CustomerSetting pod;
                if (frayteCustomerRateCard.CustomerRateSetting != null)
                {
                    var customerDetail = dbContext.CustomerSettings.Where(x => x.UserId == frayteCustomerRateCard.UserId && x.ScheduleSettingType == frayteCustomerRateCard.CustomerRateSetting.ScheduleSettingType).FirstOrDefault();
                    if (customerDetail != null && customerDetail.CustomerSettingId > 0)
                    {
                        if (frayteCustomerRateCard.CustomerRateSetting.ScheduleSetting == FrayteReportSettingDays.PerShipment)
                        {
                            customerDetail.UserId = frayteCustomerRateCard.UserId;
                            customerDetail.ScheduleSetting = frayteCustomerRateCard.CustomerRateSetting.ScheduleSetting;
                            customerDetail.ScheduleType = null;
                            customerDetail.ScheduleDate = null;
                            customerDetail.ScheduleDay = null;
                            customerDetail.ScheduleTime = TimeSpan.Parse("00:00:00");
                            customerDetail.AdditionalMails = null;
                            customerDetail.ScheduleSettingType = frayteCustomerRateCard.CustomerRateSetting.ScheduleSettingType;
                            customerDetail.IsPdf = false;
                            customerDetail.IsExcel = false;
                        }
                        else if (frayteCustomerRateCard.CustomerRateSetting.ScheduleSetting == FrayteReportSettingDays.Scheduled)
                        {
                            customerDetail.UserId = frayteCustomerRateCard.UserId;
                            customerDetail.ScheduleSetting = frayteCustomerRateCard.CustomerRateSetting.ScheduleSetting;
                            customerDetail.ScheduleType = frayteCustomerRateCard.CustomerRateSetting.ScheduleType;
                            customerDetail.ScheduleDate = UtilityRepository.ConvertDateTimetoUniversalTime(Convert.ToDateTime(frayteCustomerRateCard.CustomerRateSetting.ScheduleDate));
                            customerDetail.ScheduleTime = TimeSpan.Parse(UtilityRepository.GetFormattedTimeFromString(frayteCustomerRateCard.CustomerRateSetting.ScheduleTime));
                            customerDetail.ScheduleSettingType = frayteCustomerRateCard.CustomerRateSetting.ScheduleSettingType;
                            if (frayteCustomerRateCard.CustomerRateSetting.ScheduleType == FrayteReportSettingDays.Weekly)
                            {
                                customerDetail.ScheduleDay = frayteCustomerRateCard.CustomerRateSetting.ScheduleDay;
                            }
                            else
                            {
                                customerDetail.ScheduleDay = null;
                            }
                            customerDetail.AdditionalMails = frayteCustomerRateCard.CustomerRateSetting.AdditionalMails;
                            customerDetail.IsPdf = frayteCustomerRateCard.CustomerRateSetting.IsPdf;
                            customerDetail.IsExcel = frayteCustomerRateCard.CustomerRateSetting.IsExcel;
                        }
                        dbContext.Entry(customerDetail).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        pod = new CustomerSetting();
                        pod.UserId = frayteCustomerRateCard.UserId;
                        if (frayteCustomerRateCard.CustomerRateSetting.ScheduleSetting == FrayteReportSettingDays.PerShipment)
                        {
                            pod.ScheduleSetting = frayteCustomerRateCard.CustomerRateSetting.ScheduleSetting;
                            pod.ScheduleType = null;
                            pod.ScheduleDate = null;
                            pod.ScheduleDay = null;
                            pod.ScheduleTime = TimeSpan.Parse("00:00:00");
                            pod.AdditionalMails = null;
                            pod.CreatedOn = DateTime.UtcNow;
                            pod.UpdatedOn = null;
                            pod.ScheduleSettingType = frayteCustomerRateCard.CustomerRateSetting.ScheduleSettingType;
                            pod.IsPdf = false;
                            pod.IsExcel = false;
                        }
                        else if (frayteCustomerRateCard.CustomerRateSetting.ScheduleSetting == FrayteReportSettingDays.Scheduled)
                        {
                            pod.ScheduleSetting = frayteCustomerRateCard.CustomerRateSetting.ScheduleSetting;
                            pod.ScheduleType = frayteCustomerRateCard.CustomerRateSetting.ScheduleType;
                            pod.ScheduleDate = UtilityRepository.ConvertDateTimetoUniversalTime(Convert.ToDateTime(frayteCustomerRateCard.CustomerRateSetting.ScheduleDate));
                            pod.ScheduleTime = TimeSpan.Parse(UtilityRepository.GetFormattedTimeFromString(frayteCustomerRateCard.CustomerRateSetting.ScheduleTime)); //customerDetail.ScheduleDate.Value.TimeOfDay;
                            if (frayteCustomerRateCard.CustomerRateSetting.ScheduleDay == FrayteReportSettingDays.Weekly)
                            {
                                pod.ScheduleDay = frayteCustomerRateCard.CustomerRateSetting.ScheduleDay;
                            }
                            else
                            {
                                pod.ScheduleDay = null;
                            }
                            pod.ScheduleSettingType = frayteCustomerRateCard.CustomerRateSetting.ScheduleSettingType;
                            pod.AdditionalMails = frayteCustomerRateCard.CustomerRateSetting.AdditionalMails;
                            pod.CreatedOn = DateTime.UtcNow;
                            pod.UpdatedOn = DateTime.UtcNow;
                            pod.IsPdf = frayteCustomerRateCard.CustomerRateSetting.IsPdf;
                            pod.IsExcel = frayteCustomerRateCard.CustomerRateSetting.IsExcel;
                        }
                        dbContext.CustomerSettings.Add(pod);
                        dbContext.SaveChanges();

                        SaveCustomerSettingDetail(frayteCustomerRateCard.CustomerRateSetting.CustomerSettingDetail, pod.CustomerSettingId);
                    }
                }
                else
                {
                    UploadCustomerPODSetting(frayteCustomerRateCard.UserId);
                }
            }
            return result;
        }

        public void SaveCustomerAdditional(FrayteCustomer frayteCustomerUser)

        {
            UserAdditional customerDetail = dbContext.UserAdditionals.Where(p => p.UserId == frayteCustomerUser.UserId).FirstOrDefault();

            if (customerDetail != null)
            {
                customerDetail.AccountName = frayteCustomerUser.AccountName;
                customerDetail.AccountMail = frayteCustomerUser.AccountMail;
                customerDetail.CreditLimit = frayteCustomerUser.CreditLimit;
                customerDetail.CreditLimitCurrencyCode = frayteCustomerUser.CreditLimitCurrencyCode;
                customerDetail.TermsOfThePayment = frayteCustomerUser.TermsOfPayment;
                customerDetail.TaxAndDuties = frayteCustomerUser.TaxAndDuties;
                customerDetail.APIKey = UtilityRepository.Encrypt(customerDetail.AccountNo + "," + customerDetail.AccountMail, EncriptionKey.PrivateKey);
                if (frayteCustomerUser.AccountUser != null)
                {
                    customerDetail.AccountUserId = frayteCustomerUser.AccountUser.UserId;
                }

                if (frayteCustomerUser.DocumentUser != null)
                {
                    customerDetail.DocumentUserId = frayteCustomerUser.DocumentUser.UserId;
                }

                if (frayteCustomerUser.ManagerUser != null)
                {
                    customerDetail.ManagerUserId = frayteCustomerUser.ManagerUser.UserId;
                }

                if (frayteCustomerUser.OperationUser != null)
                {
                    customerDetail.OperationUserId = frayteCustomerUser.OperationUser.UserId;
                }
                if (frayteCustomerUser.SalesRepresentative != null)
                {
                    customerDetail.SalesUserId = frayteCustomerUser.SalesRepresentative.UserId;
                }
                if (frayteCustomerUser.IsDirectBooking == true)
                {
                    customerDetail.IsDirectBooking = frayteCustomerUser.IsDirectBooking;
                }
                else
                {
                    customerDetail.IsDirectBooking = false;
                }
                if (frayteCustomerUser.IsTradeLaneBooking == true)
                {
                    customerDetail.IsTradelaneBooking = frayteCustomerUser.IsTradeLaneBooking;
                }
                else
                {
                    customerDetail.IsTradelaneBooking = false;
                }
                if (frayteCustomerUser.IsBreakBulkBooking == true)
                {
                    customerDetail.IsBreakBulkBooking = frayteCustomerUser.IsBreakBulkBooking;
                }
                else
                {
                    customerDetail.IsBreakBulkBooking = false;
                }
                if (frayteCustomerUser.IsECommerce == true)
                {
                    customerDetail.IsECommerce = frayteCustomerUser.IsECommerce;
                    customerDetail.FreeStorageTime = UtilityRepository.GetTimeFromString(frayteCustomerUser.FreeStorageTime);
                    customerDetail.FreeStorageCharge = frayteCustomerUser.FreeStorageCharge;
                    customerDetail.FreeStorageCurrencyCode = frayteCustomerUser.FreeStorageChargeCurrencyCode;
                }
                else
                {
                    customerDetail.IsECommerce = false;
                }
                if (frayteCustomerUser.IsShipperTaxAndDuty == true)
                {
                    customerDetail.IsShipperTaxAndDuty = frayteCustomerUser.IsShipperTaxAndDuty;
                }
                else
                {
                    customerDetail.IsShipperTaxAndDuty = false;
                }
                if (frayteCustomerUser.IsAllowRate == true)
                {
                    customerDetail.IsAllowRate = frayteCustomerUser.IsAllowRate;
                }
                else
                {
                    customerDetail.IsAllowRate = false;
                }

                if (frayteCustomerUser.IsServiceSelected == true)
                {
                    customerDetail.IsServiceSelected = frayteCustomerUser.IsServiceSelected;
                }
                else
                {
                    customerDetail.IsServiceSelected = false;
                }

                if (frayteCustomerUser.IsWithoutService == true)
                {
                    customerDetail.IsWithoutService = frayteCustomerUser.IsWithoutService;
                }
                else
                {
                    customerDetail.IsWithoutService = false;
                }
                if (frayteCustomerUser.IsApiAllow == true)
                {
                    customerDetail.IsApiAllow = frayteCustomerUser.IsApiAllow;
                }
                else
                {
                    customerDetail.IsApiAllow = false;
                }

                customerDetail.IsWarehouseTransport = frayteCustomerUser.IsWarehouseTransport;
                customerDetail.IsExpressSolutions = frayteCustomerUser.IsExpressSolutions;
                customerDetail.DaysValidity = frayteCustomerUser.DaysValidity;
                customerDetail.CustomerRateCardType = frayteCustomerUser.CustomerRateCardType;
                customerDetail.CustomerType = frayteCustomerUser.CustomerType;
                customerDetail.UserType = frayteCustomerUser.UserType;
            }
            else
            {
                customerDetail = new UserAdditional();
                customerDetail.UserId = frayteCustomerUser.UserId;
                customerDetail.AccountNo = frayteCustomerUser.AccountNumber;
                customerDetail.AccountName = frayteCustomerUser.AccountName;
                customerDetail.AccountMail = frayteCustomerUser.AccountMail;
                customerDetail.CreditLimit = frayteCustomerUser.CreditLimit;
                customerDetail.CreditLimitCurrencyCode = frayteCustomerUser.CreditLimitCurrencyCode;
                customerDetail.TermsOfThePayment = frayteCustomerUser.TermsOfPayment;
                customerDetail.TaxAndDuties = frayteCustomerUser.TaxAndDuties;

                if (frayteCustomerUser.AccountUser != null)
                {
                    customerDetail.AccountUserId = frayteCustomerUser.AccountUser.UserId;
                }

                if (frayteCustomerUser.DocumentUser != null)
                {
                    customerDetail.DocumentUserId = frayteCustomerUser.DocumentUser.UserId;
                }

                if (frayteCustomerUser.ManagerUser != null)
                {
                    customerDetail.ManagerUserId = frayteCustomerUser.ManagerUser.UserId;
                }

                if (frayteCustomerUser.OperationUser != null)
                {
                    customerDetail.OperationUserId = frayteCustomerUser.OperationUser.UserId;
                }
                if (frayteCustomerUser.SalesRepresentative != null)
                {
                    customerDetail.SalesUserId = frayteCustomerUser.SalesRepresentative.UserId;
                }
                if (frayteCustomerUser.IsDirectBooking == true)
                {
                    customerDetail.IsDirectBooking = frayteCustomerUser.IsDirectBooking;
                }
                else
                {
                    customerDetail.IsDirectBooking = false;
                }
                if (frayteCustomerUser.IsTradeLaneBooking == true)
                {
                    customerDetail.IsTradelaneBooking = frayteCustomerUser.IsTradeLaneBooking;
                }
                else
                {
                    customerDetail.IsTradelaneBooking = false;
                }
                if (frayteCustomerUser.IsBreakBulkBooking == true)
                {
                    customerDetail.IsBreakBulkBooking = frayteCustomerUser.IsBreakBulkBooking;
                }
                else
                {
                    customerDetail.IsBreakBulkBooking = false;
                }
                if (frayteCustomerUser.IsECommerce == true)
                {
                    customerDetail.IsECommerce = frayteCustomerUser.IsECommerce;
                    customerDetail.FreeStorageTime = UtilityRepository.GetTimeFromString(frayteCustomerUser.FreeStorageTime);
                    customerDetail.FreeStorageCharge = frayteCustomerUser.FreeStorageCharge;
                    customerDetail.FreeStorageCurrencyCode = frayteCustomerUser.FreeStorageChargeCurrencyCode;
                }
                else
                {
                    customerDetail.IsECommerce = false;
                }
                if (frayteCustomerUser.IsShipperTaxAndDuty == true)
                {
                    customerDetail.IsShipperTaxAndDuty = frayteCustomerUser.IsShipperTaxAndDuty;
                }
                else
                {
                    customerDetail.IsShipperTaxAndDuty = false;
                }
                if (frayteCustomerUser.IsAllowRate == true)
                {
                    customerDetail.IsAllowRate = frayteCustomerUser.IsAllowRate;
                }
                else
                {
                    customerDetail.IsAllowRate = false;
                }
                if (frayteCustomerUser.IsServiceSelected == true)
                {
                    customerDetail.IsServiceSelected = frayteCustomerUser.IsServiceSelected;
                }
                else
                {
                    customerDetail.IsServiceSelected = false;
                }

                if (frayteCustomerUser.IsWithoutService == true)
                {
                    customerDetail.IsWithoutService = frayteCustomerUser.IsWithoutService;
                }
                else
                {
                    customerDetail.IsWithoutService = false;
                }

                if (frayteCustomerUser.IsApiAllow == true)
                {
                    customerDetail.IsApiAllow = frayteCustomerUser.IsApiAllow;
                }
                else
                {
                    customerDetail.IsApiAllow = false;
                }

                customerDetail.IsWarehouseTransport = frayteCustomerUser.IsWarehouseTransport;
                customerDetail.IsExpressSolutions = frayteCustomerUser.IsExpressSolutions;
                customerDetail.DaysValidity = frayteCustomerUser.DaysValidity;
                customerDetail.CustomerRateCardType = frayteCustomerUser.CustomerRateCardType;
                customerDetail.APIKey = UtilityRepository.Encrypt(customerDetail.AccountNo + "," + customerDetail.AccountMail, EncriptionKey.PrivateKey);
                customerDetail.CustomerType = frayteCustomerUser.CustomerType;
                customerDetail.UserType = frayteCustomerUser.UserType;
                dbContext.UserAdditionals.Add(customerDetail);
            }

            if (customerDetail != null)
            {
                dbContext.SaveChanges();
            }
        }

        public void SaveUserCustomerDetail(int UserId, int CustomerId)
        {
            try
            {
                UserCustomer uc = new UserCustomer();
                uc.UserId = UserId;
                uc.CustomerId = CustomerId;
                dbContext.UserCustomers.Add(uc);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public FrayteResult CheckAccountNumber(string accountNumber)
        {
            FrayteResult result = new FrayteResult();

            var customerResult = dbContext.UserAdditionals.Where(p => p.AccountNo == accountNumber).FirstOrDefault();

            if (customerResult != null)
            {
                result.Status = true;
            }

            return result;
        }

        public List<FrayteOperationZone> GetOperationZone()
        {
            List<FrayteOperationZone> _operation = new List<FrayteOperationZone>();
            var operation = dbContext.OperationZones.ToList();
            FrayteOperationZone fz;
            foreach (var rr in operation)
            {
                fz = new FrayteOperationZone();
                fz.OperationZoneId = rr.OperationZoneId;
                fz.OperationZoneName = rr.Name;
                _operation.Add(fz);
            }
            return _operation;
        }

        public List<LogisticShipmentType> GetShipmentType(int OperationZoneId, string CourierCompany, string ModuleType)
        {
            List<LogisticShipmentType> _shipType = new List<LogisticShipmentType>();
            _shipType = (from ls in dbContext.LogisticServices
                         join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                         where ls.LogisticCompany == CourierCompany &&
                               ls.OperationZoneId == OperationZoneId &&
                               ls.ModuleType == ModuleType
                         select new LogisticShipmentType
                         {
                             ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                             LogisticServiceId = lsst.LogisticServiceId,
                             OperationZoneId = ls.OperationZoneId,
                             RateType = ls.RateType,
                             LogisticType = ls.LogisticType,
                             LogisticDescription = lsst.LogisticDescription,
                             LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType
                         }).ToList();

            return _shipType;
        }

        #region ---Public Method

        public List<OperationZone> GetOpearationZone()
        {
            var list = dbContext.OperationZones.ToList();
            return list;
        }

        #endregion

        #region ---Private Method        

        private FrayteCustomerSetting GetCustomerSetting(int customerId, string ScheduleSettingType)
        {
            FrayteCustomerSetting customPOD = new FrayteCustomerSetting();
            try
            {
                var UserDetail = dbContext.Users.Where(ab => ab.UserId == customerId).FirstOrDefault();
                var TimeZone = dbContext.Timezones.Where(a => a.TimezoneId == UserDetail.TimezoneId).FirstOrDefault();
                var TimeZoneInformation = TimeZoneInfo.FindSystemTimeZoneById(TimeZone.Name);
                var Setting = dbContext.CustomerSettings.Where(x => x.UserId == customerId && x.ScheduleSettingType == ScheduleSettingType).FirstOrDefault();
                if (Setting != null)
                {
                    if (Setting.CustomerSettingId > 0)
                    {
                        customPOD.CustomerSettingId = Setting.CustomerSettingId;
                        customPOD.ScheduleSetting = Setting.ScheduleSetting;
                        customPOD.ScheduleType = Setting.ScheduleType;
                        customPOD.ScheduleSettingType = Setting.ScheduleSettingType;
                        if (Setting.ScheduleDate.HasValue)
                        {
                            customPOD.ScheduleDate = UtilityRepository.UtcDateToOtherTimezone(Convert.ToDateTime(Setting.ScheduleDate), Setting.ScheduleTime, TimeZoneInformation).Item1;
                            System.DateTime today = System.DateTime.Now;
                            System.TimeSpan duration = new System.TimeSpan(36, 4, 15, 0);
                        }
                        else
                        {
                            customPOD.ScheduleDate = null;
                        }
                        customPOD.ScheduleDay = Setting.ScheduleDay;
                        customPOD.ScheduleTime = UtilityRepository.UtcDateToOtherTimezone(Convert.ToDateTime(Setting.ScheduleDate), Setting.ScheduleTime, TimeZoneInformation).Item2;
                        customPOD.AdditionalMails = Setting.AdditionalMails;
                        if (Setting.IsPdf.HasValue)
                        {
                            customPOD.IsPdf = Setting.IsPdf.Value;
                        }
                        if (Setting.IsPdf.HasValue)
                        {
                            customPOD.IsExcel = Setting.IsExcel.Value;
                        }
                        customPOD.CustomerSettingDetail = new List<FrayteCustomerSettingDetail>();
                        FrayteCustomerSettingDetail fsd;
                        var csid = dbContext.CustomerSettingDetails.Where(x => x.CustomerSettingId == Setting.CustomerSettingId).ToList();
                        if (csid != null)
                        {
                            foreach (var cdetail in csid)
                            {
                                fsd = new FrayteCustomerSettingDetail();
                                fsd.CustomerSettingDetailId = cdetail.CustomerSettingDetailId;
                                fsd.CustomerSettingId = cdetail.CustomerSettingId;
                                fsd.CourierShipment = new FrayteShipmentCourier();
                                var cid = dbContext.Couriers.Where(x => x.CourierId == cdetail.CourierId).FirstOrDefault();
                                if (cid != null)
                                {
                                    fsd.CourierShipment.CourierId = cid.CourierId;
                                    fsd.CourierShipment.CourierType = cid.ShipmentType;
                                    fsd.CourierShipment.Name = cid.CourierName;
                                    fsd.CourierShipment.Website = cid.Website;
                                }
                                customPOD.CustomerSettingDetail.Add(fsd);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return customPOD;
        }

        public void SaveCustomerSetting(FrayteCustomerSetting userSetting, int UserId, TimeZoneModal TimeZone)
        {
            try
            {
                CustomerSetting pod;
                if (userSetting != null)
                {
                    var customerDetail = dbContext.CustomerSettings.Where(x => x.UserId == UserId && x.ScheduleSettingType == userSetting.ScheduleSettingType).FirstOrDefault();
                    if (customerDetail != null && customerDetail.CustomerSettingId > 0)
                    {
                        if (userSetting.ScheduleSetting == FrayteReportSettingDays.PerShipment)
                        {
                            customerDetail.UserId = UserId;
                            customerDetail.ScheduleSetting = userSetting.ScheduleSetting;
                            customerDetail.ScheduleType = null;
                            customerDetail.ScheduleDate = null;
                            customerDetail.ScheduleDay = null;
                            customerDetail.ScheduleTime = TimeSpan.Parse("00:00:00");
                            customerDetail.AdditionalMails = userSetting.AdditionalMails;
                            customerDetail.ScheduleSettingType = userSetting.ScheduleSettingType;
                            customerDetail.IsPdf = false;
                            customerDetail.IsExcel = false;
                        }
                        else if (userSetting.ScheduleSetting == FrayteReportSettingDays.Scheduled)
                        {
                            customerDetail.UserId = UserId;
                            customerDetail.ScheduleSetting = userSetting.ScheduleSetting;
                            customerDetail.ScheduleType = userSetting.ScheduleType;
                            customerDetail.ScheduleDate = UtilityRepository.ConvertDateTimetoUniversalTime(Convert.ToDateTime(userSetting.ScheduleDate));
                            customerDetail.ScheduleTime = UtilityRepository.TimeSpanConversion(TimeSpan.Parse(UtilityRepository.GetFormattedTimeString(userSetting.ScheduleTime)).Hours, TimeSpan.Parse(UtilityRepository.GetFormattedTimeString(userSetting.ScheduleTime)).Minutes, TimeSpan.Parse(UtilityRepository.GetFormattedTimeString(userSetting.ScheduleTime)).Seconds);
                            customerDetail.ScheduleSettingType = userSetting.ScheduleSettingType;
                            if (userSetting.ScheduleType == FrayteReportSettingDays.Weekly)
                            {
                                customerDetail.ScheduleDay = userSetting.ScheduleDay;
                            }
                            else
                            {
                                customerDetail.ScheduleDay = null;
                            }
                            customerDetail.AdditionalMails = userSetting.AdditionalMails;
                            customerDetail.IsPdf = userSetting.IsPdf;
                            customerDetail.IsExcel = userSetting.IsExcel;
                        }
                        dbContext.Entry(customerDetail).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        SaveCustomerSettingDetail(userSetting.CustomerSettingDetail, customerDetail.CustomerSettingId);
                    }
                    else
                    {
                        pod = new CustomerSetting();
                        pod.UserId = UserId;
                        if (userSetting.ScheduleSetting == FrayteReportSettingDays.PerShipment)
                        {
                            pod.ScheduleSetting = userSetting.ScheduleSetting;
                            pod.ScheduleType = userSetting.ScheduleType;
                            pod.ScheduleDate = UtilityRepository.ConvertDateTimetoUniversalTime(Convert.ToDateTime(userSetting.ScheduleDate));
                            pod.ScheduleDay = userSetting.ScheduleDay;
                            pod.ScheduleTime = TimeSpan.Parse("00:00:00");
                            pod.AdditionalMails = userSetting.AdditionalMails;
                            pod.CreatedOn = DateTime.UtcNow;
                            pod.UpdatedOn = null;
                            pod.ScheduleSettingType = userSetting.ScheduleSettingType;
                            pod.IsPdf = false;
                            pod.IsExcel = false;
                        }
                        else if (userSetting.ScheduleSetting == FrayteReportSettingDays.Scheduled)
                        {
                            pod.ScheduleSetting = userSetting.ScheduleSetting;
                            pod.ScheduleType = userSetting.ScheduleType;
                            pod.ScheduleDate = UtilityRepository.ConvertDateTimetoUniversalTime(Convert.ToDateTime(userSetting.ScheduleDate)); ;
                            if (userSetting.ScheduleDay == FrayteReportSettingDays.Weekly)
                            {
                                pod.ScheduleDay = userSetting.ScheduleDay;
                            }
                            else
                            {
                                pod.ScheduleDay = null;
                            }
                            //pod.ScheduleTime = UtilityRepository.TimeSpanConversion(pod.ScheduleDate.Value.Hour, pod.ScheduleDate.Value.Minute, pod.ScheduleDate.Value.Second);
                            customerDetail.ScheduleTime = UtilityRepository.TimeSpanConversion(TimeSpan.Parse(UtilityRepository.GetFormattedTimeString(userSetting.ScheduleTime)).Hours, TimeSpan.Parse(UtilityRepository.GetFormattedTimeString(userSetting.ScheduleTime)).Minutes, TimeSpan.Parse(UtilityRepository.GetFormattedTimeString(userSetting.ScheduleTime)).Seconds);
                            pod.ScheduleSettingType = userSetting.ScheduleSettingType;
                            pod.AdditionalMails = userSetting.AdditionalMails;
                            pod.CreatedOn = DateTime.UtcNow;
                            pod.UpdatedOn = DateTime.UtcNow;
                            pod.IsPdf = userSetting.IsPdf;
                            pod.IsExcel = userSetting.IsExcel;
                        }
                        dbContext.CustomerSettings.Add(pod);
                        dbContext.SaveChanges();

                        SaveCustomerSettingDetail(userSetting.CustomerSettingDetail, pod.CustomerSettingId);
                    }
                }
                else
                {
                    UploadCustomerPODSetting(UserId);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void SaveCustomerSettingDetail(List<FrayteCustomerSettingDetail> _SettingDetail, int CustomerSettingId)
        {
            try
            {
                CustomerSettingDetail csd;
                if (_SettingDetail.Count > 0)
                {
                    foreach (var deatil in _SettingDetail)
                    {
                        var customerSettingDetail = dbContext.CustomerSettingDetails.Where(x => x.CustomerSettingDetailId == deatil.CustomerSettingDetailId).FirstOrDefault();
                        if (customerSettingDetail != null)
                        {
                            if (customerSettingDetail.CustomerSettingDetailId > 0)
                            {
                                customerSettingDetail.CustomerSettingDetailId = deatil.CustomerSettingDetailId;
                                customerSettingDetail.CustomerSettingId = deatil.CustomerSettingId;
                                customerSettingDetail.CourierId = deatil.CourierShipment.CourierId;
                                dbContext.Entry(customerSettingDetail).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                        }
                        else
                        {
                            csd = new CustomerSettingDetail();
                            csd.CustomerSettingId = CustomerSettingId;
                            csd.CourierId = deatil.CourierShipment.CourierId;
                            dbContext.CustomerSettingDetails.Add(csd);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        //private void UploadCustomerMarginCost(int UserId)
        //{
        //    try
        //    {
        //        CustomerMarginCost cmc;

        //        //Step 1: Get Import, Export, ThirdParty Zone Names
        //        var ExportZone = dbContext.Zones.Where(p => p.LogisticType == FrayteLogisticType.Export && p.OperationZoneId == 2).ToList();
        //        var ImportZone = dbContext.Zones.Where(p => p.LogisticType == FrayteLogisticType.Import && p.OperationZoneId == 2).ToList();
        //        var ThirdZone = dbContext.Zones.Where(p => p.LogisticType == FrayteLogisticType.ThirdParty && p.OperationZoneId == 2).ToList();

        //        //Step 2: Get UKShipment, EUImport, EUExport Zone Names
        //        var UKShipment = dbContext.Zones.Where(p => p.LogisticType == FrayteLogisticType.UKShipment && p.OperationZoneId == 2).ToList();
        //        var EUImport = dbContext.Zones.Where(p => p.LogisticType == FrayteLogisticType.EUImport && p.OperationZoneId == 2).ToList();
        //        var EUExport = dbContext.Zones.Where(p => p.LogisticType == FrayteLogisticType.EUExport && p.OperationZoneId == 2).ToList();

        //        //Step 3: Get Shipment Type For Import, Export, ThirdParty
        //        var ShipmentDoc = dbContext.ShipmentTypes.Where(p => p.ShipmentTypeId < 5).ToList();

        //        //Step 4: Get Shipment Type For UKShipment, EUImport, EUExport
        //        //var UKShipmentDoc = dbContext.ShipmentTypes.Where(p => p.ShipmentTypeId > 4).ToList();

        //        foreach (var DocType in ShipmentDoc)
        //        {
        //            foreach (var export in ExportZone)
        //            {
        //                cmc = new CustomerMarginCost();
        //                cmc.UserId = UserId;
        //                cmc.ZoneId = export.ZoneId;
        //                cmc.ShipmentTypeId = DocType.ShipmentTypeId;
        //                cmc.PercentOfMargin = 0;
        //                cmc.OpeartionZoneId = 2;
        //                cmc.LogisticType = FrayteLogisticType.Export;
        //                dbContext.CustomerMarginCosts.Add(cmc);
        //                dbContext.SaveChanges();
        //            }
        //        }

        //        foreach (var DocType in ShipmentDoc)
        //        {
        //            foreach (var import in ImportZone)
        //            {
        //                cmc = new CustomerMarginCost();
        //                cmc.UserId = UserId;
        //                cmc.ZoneId = import.ZoneId;
        //                cmc.ShipmentTypeId = DocType.ShipmentTypeId;
        //                cmc.PercentOfMargin = 0;
        //                cmc.OpeartionZoneId = 2;
        //                cmc.LogisticType = FrayteLogisticType.Import;
        //                dbContext.CustomerMarginCosts.Add(cmc);
        //                dbContext.SaveChanges();
        //            }
        //        }

        //        foreach (var DocType in ShipmentDoc)
        //        {
        //            foreach (var third in ThirdZone)
        //            {
        //                cmc = new CustomerMarginCost();
        //                cmc.UserId = UserId;
        //                cmc.ZoneId = third.ZoneId;
        //                cmc.ShipmentTypeId = DocType.ShipmentTypeId;
        //                cmc.PercentOfMargin = 0;
        //                cmc.OpeartionZoneId = 2;
        //                cmc.LogisticType = FrayteLogisticType.ThirdParty;
        //                dbContext.CustomerMarginCosts.Add(cmc);
        //                dbContext.SaveChanges();
        //            }
        //        }

        //        foreach (var uk in UKShipment)
        //        {
        //            cmc = new CustomerMarginCost();
        //            cmc.UserId = UserId;
        //            cmc.ZoneId = uk.ZoneId;
        //            cmc.ShipmentTypeId = 0;
        //            cmc.PercentOfMargin = 0;
        //            cmc.OpeartionZoneId = 2;
        //            cmc.LogisticType = FrayteLogisticType.UKShipment;
        //            dbContext.CustomerMarginCosts.Add(cmc);
        //            dbContext.SaveChanges();
        //        }

        //        foreach (var eui in EUExport)
        //        {
        //            cmc = new CustomerMarginCost();
        //            cmc.UserId = UserId;
        //            cmc.ZoneId = eui.ZoneId;
        //            cmc.ShipmentTypeId = 0;
        //            cmc.PercentOfMargin = 0;
        //            cmc.OpeartionZoneId = 2;
        //            cmc.LogisticType = FrayteLogisticType.EUExport;
        //            dbContext.CustomerMarginCosts.Add(cmc);
        //            dbContext.SaveChanges();
        //        }

        //        foreach (var eue in EUExport)
        //        {
        //            cmc = new CustomerMarginCost();
        //            cmc.UserId = UserId;
        //            cmc.ZoneId = eue.ZoneId;
        //            cmc.ShipmentTypeId = 0;
        //            cmc.PercentOfMargin = 0;
        //            cmc.OpeartionZoneId = 2;
        //            cmc.LogisticType = FrayteLogisticType.EUImport;
        //            dbContext.CustomerMarginCosts.Add(cmc);
        //            dbContext.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        private void UploadCustomerPODSetting(int UserId)
        {
            CustomerSetting pod;
            var CustomerSetting = dbContext.CustomerSettings.Where(p => p.UserId == UserId).ToList();
            if (CustomerSetting.Count == 1)
            {
                pod = new CustomerSetting();
                pod.UserId = UserId;
                pod.ScheduleSetting = FrayteReportSettingDays.PerShipment;
                pod.ScheduleType = null;
                pod.ScheduleDate = null;
                pod.ScheduleDay = null;
                pod.ScheduleTime = TimeSpan.Parse("10:00:00");
                pod.AdditionalMails = null;
                pod.CreatedOn = DateTime.UtcNow;
                pod.UpdatedOn = null;
                pod.ScheduleSettingType = FrayteCustomerMailSetting.RateCard;
                pod.IsPdf = true;
                pod.IsExcel = true;
                dbContext.CustomerSettings.Add(pod);
                dbContext.SaveChanges();
            }
            else if (CustomerSetting.Count == 0)
            {
                pod = new CustomerSetting();
                pod.UserId = UserId;
                pod.ScheduleSetting = FrayteReportSettingDays.PerShipment;
                pod.ScheduleType = null;
                pod.ScheduleDate = null;
                pod.ScheduleDay = null;
                pod.ScheduleTime = TimeSpan.Parse("10:00:00");
                pod.AdditionalMails = null;
                pod.CreatedOn = DateTime.UtcNow;
                pod.UpdatedOn = null;
                pod.ScheduleSettingType = FrayteCustomerMailSetting.POD;
                pod.IsPdf = true;
                pod.IsExcel = true;
                dbContext.CustomerSettings.Add(pod);
                dbContext.SaveChanges();
            }
        }

        #endregion

        public FrayteResult SaveCustomerMarginCost(List<FrayteCustomerMarginCost> customerMarginCost)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                CustomerMarginCost cmc;
                if (customerMarginCost != null && customerMarginCost.Count > 0)
                {
                    foreach (var fcc in customerMarginCost)
                    {
                        foreach (var cc in fcc.CustomerMargin)
                        {
                            int shipmentid = (cc.ShipmentType == null ? 0 : cc.ShipmentType.ShipmentTypeId);
                            var detail = dbContext.CustomerMarginCosts.Where(p => p.CustomerId == fcc.UserId && p.OperationZoneId == fcc.OperationZoneId && p.LogisticServiceZoneId == cc.Zone.ZoneId && p.LogisticShipmentTypeId == shipmentid).ToList();
                            if (detail != null && detail.Count > 0)
                            {
                                foreach (var Obj in detail)
                                {
                                    Obj.CustomerMarginOptionId = fcc.CustomerMarginOptionId;
                                    Obj.Percentage = cc.PercentOfMargin == null ? 0.0m : cc.PercentOfMargin.Value;
                                    dbContext.Entry(Obj).State = System.Data.Entity.EntityState.Modified;
                                    dbContext.SaveChanges();
                                }
                            }
                            else
                            {
                                cmc = new CustomerMarginCost();
                                cmc.CustomerId = fcc.UserId;
                                cmc.OperationZoneId = fcc.OperationZoneId;
                                cmc.CustomerMarginOptionId = fcc.CustomerMarginOptionId;
                                if (fcc.CustomerMargin != null && fcc.CustomerMargin.Count > 0)
                                {
                                    foreach (var cm in fcc.CustomerMargin)
                                    {
                                        if (cm.ShipmentType != null && cm.ShipmentType.ShipmentTypeId > 0)
                                        {
                                            var weight = dbContext.LogisticServiceWeights.Where(p => p.LogisticServiceShipmentTypeId == cm.ShipmentType.ShipmentTypeId).ToList();
                                            if (weight != null && weight.Count > 0)
                                            {
                                                foreach (var Obj in weight)
                                                {
                                                    cmc.LogisticServiceZoneId = cm.Zone.ZoneId;
                                                    cmc.LogisticShipmentTypeId = cm.ShipmentType.ShipmentTypeId;
                                                    cmc.LogisticServiceWeightId = Obj.LogisticServiceWeightId;
                                                    cmc.Percentage = cm.PercentOfMargin == null ? 0.0m : cm.PercentOfMargin.Value;
                                                    cmc.ModuleType = fcc.ModuleType;
                                                    dbContext.CustomerMarginCosts.Add(cmc);
                                                    dbContext.SaveChanges();
                                                }
                                            }
                                            else
                                            {
                                                cmc.LogisticServiceZoneId = cm.Zone.ZoneId;
                                                cmc.LogisticShipmentTypeId = cm.ShipmentType.ShipmentTypeId;
                                                cmc.LogisticServiceWeightId = 0;
                                                cmc.Percentage = cm.PercentOfMargin == null ? 0.0m : cm.PercentOfMargin.Value;
                                                cmc.ModuleType = fcc.ModuleType;
                                                dbContext.CustomerMarginCosts.Add(cmc);
                                                dbContext.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            cmc.LogisticServiceZoneId = cm.Zone.ZoneId;
                                            cmc.LogisticShipmentTypeId = 0;
                                            cmc.LogisticServiceWeightId = 0;
                                            cmc.Percentage = cm.PercentOfMargin == null ? 0.0m : cm.PercentOfMargin.Value;
                                            cmc.ModuleType = fcc.ModuleType;
                                            dbContext.CustomerMarginCosts.Add(cmc);
                                            dbContext.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    result.Status = true;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return result;
        }

        public FrayteResult SaveCustomerAdvanceMarginCost(List<FrayteCustomerAdvanceRateCard> advanceMarginCost)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                if (advanceMarginCost != null && advanceMarginCost.Count > 0)
                {
                    foreach (var Obj in advanceMarginCost)
                    {
                        var detail = dbContext.CustomerAdvanceMarginCosts.Where(p => p.CustomerAdvanceMarginCostId == Obj.CustomerAdvanceMarginCostId).FirstOrDefault();
                        if (detail != null)
                        {
                            detail.AdvancePercentage = Obj.AdvancePercentage;
                            dbContext.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            CustomerAdvanceMarginCost margin = new CustomerAdvanceMarginCost();
                            margin.CustomerId = Obj.CustomerId;
                            margin.OperationZoneId = Obj.OperationZoneId;
                            margin.LogisticServiceZoneId = Obj.LogisticServiceZoneId;
                            margin.LogisticServiceWeightId = Obj.LogisticServiceWeightId;
                            margin.LogisticServiceShipmentTypeId = Obj.LogisticServiceShipmentTypeId;
                            margin.AdvancePercentage = Obj.AdvancePercentage;
                            margin.ModuleType = Obj.ModuleType;
                            if (margin != null)
                            {
                                dbContext.CustomerAdvanceMarginCosts.Add(margin);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                    result.Status = true;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return result;
        }

        public List<FrayteCustomerMarginCost> GetCustomerMarginCost(int UserId, int OperationZoneId, string CourierCompany, string ModuleType)
        {
            List<FrayteCustomerMarginCost> _cusMargin = new List<FrayteCustomerMarginCost>();
            try
            {
                var customer = (from ls in dbContext.LogisticServices
                                join cl in dbContext.CustomerLogistics on ls.LogisticServiceId equals cl.LogisticServiceId
                                join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                                join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                join cmc in dbContext.CustomerMarginCosts on lsz.LogisticServiceZoneId equals cmc.LogisticServiceZoneId
                                join cmoi in dbContext.CustomerMarginOptions on cmc.CustomerMarginOptionId equals cmoi.CustomerMarginOptionId
                                where ls.LogisticCompany == CourierCompany &&
                                      ls.OperationZoneId == OperationZoneId &&
                                      ls.ModuleType == ModuleType &&
                                      ls.IsActive == true &&
                                      cmc.CustomerId == UserId &&
                                      cl.UserId == UserId &&
                                      cmc.OperationZoneId == OperationZoneId &&
                                      cmc.LogisticShipmentTypeId == lsst.LogisticServiceShipmentTypeId &&
                                      cmc.LogisticServiceZoneId == lsz.LogisticServiceZoneId
                                orderby ls.MarginOrder ascending
                                select new FrayteCustomerMarginCost
                                {
                                    UserId = cmc.CustomerId,
                                    OperationZoneId = ls.OperationZoneId,
                                    CustomerMarginOptionId = cmoi.CustomerMarginOptionId,
                                    CourierCompany = ls.LogisticCompany,
                                    CourierCompanyDisplay = ls.LogisticCompanyDisplay,
                                    RateType = ls.RateType,
                                    RateTypeDisplay = ls.RateTypeDisplay,
                                    LogisticType = ls.LogisticType,
                                    LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                    ModuleType = ls.ModuleType,
                                    CustomerMargin = new List<FrayteCustomerMargin>()
                                    {
                                      new FrayteCustomerMargin()
                                      {
                                          Zone = new FrayteZone()
                                          {
                                              ZoneId = lsz.LogisticServiceZoneId,
                                              OperationZoneId = ls.OperationZoneId,
                                              ZoneName = lsz.ZoneName,
                                              ZoneDisplayName = lsz.ZoneDisplayName,
                                              ZoneColor = lsz.ZoneColor,
                                              LogisticType = ls.LogisticType,
                                              CourierComapny = ls.LogisticCompany,
                                              RateType = ls.RateType,
                                              ModuleType = ls.ModuleType
                                          },
                                          ShipmentType = new LogisticShipmentType()
                                          {
                                              ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                              LogisticServiceId = lsst.LogisticServiceId,
                                              LogisticDescription = lsst.LogisticDescription,
                                              LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                              LogisticType = lsst.LogisticType,
                                              RateType = ls.RateType,
                                              RateTypeDisplay = ls.RateTypeDisplay
                                          },
                                          PercentOfMargin = cmc.Percentage
                                       }
                                    },
                                    IsChange = false
                                }).ToList();

                if (customer != null && customer.Count > 0)
                {
                    var list = customer
                               .GroupBy(p => new { p.RateType, p.LogisticType })
                               .Select(g => new FrayteCustomerMarginCost
                               {
                                   RateType = g.Key.RateType,
                                   LogisticType = g.Key.LogisticType,
                                   RateTypeDisplay = g.Select(c => c.RateTypeDisplay).First(),
                                   LogisticTypeDisplay = g.Select(c => c.LogisticTypeDisplay).First(),
                                   UserId = g.Select(c => c.UserId).First(),
                                   CustomerMarginOptionId = g.Select(c => c.CustomerMarginOptionId).First(),
                                   OperationZoneId = g.Select(c => c.OperationZoneId).First(),
                                   CourierCompany = g.Select(c => c.CourierCompany).First(),
                                   CourierCompanyDisplay = g.Select(c => c.CourierCompanyDisplay).First(),
                                   ModuleType = g.Select(c => c.ModuleType).First(),
                                   IsChange = true,
                                   CustomerMargin = g.SelectMany(c => c.CustomerMargin)
                                                     .GroupBy(m => new { m.ShipmentType.ShipmentTypeId, m.Zone.ZoneId })
                                                     .Select(t => new FrayteCustomerMargin
                                                     {
                                                         Zone = t.Select(d => d.Zone).First(),
                                                         ShipmentType = t.Select(d => d.ShipmentType).First(),
                                                         PercentOfMargin = t.Select(d => d.PercentOfMargin).First()
                                                     }).ToList()
                               }).ToList();

                    _cusMargin = list;
                }
                else
                {
                    var margincost = (from ls in dbContext.LogisticServices
                                      join cl in dbContext.CustomerLogistics on ls.LogisticServiceId equals cl.LogisticServiceId
                                      join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                                      join lsst in dbContext.LogisticServiceShipmentTypes on lsz.LogisticServiceId equals lsst.LogisticServiceId into leftJoin
                                      from lsstTemp in leftJoin.DefaultIfEmpty()
                                      join cmc in dbContext.CustomerMarginCosts.Where(p => p.CustomerId == UserId && p.OperationZoneId == OperationZoneId)
                                           on lsz.LogisticServiceZoneId equals cmc.LogisticServiceZoneId into leftTemp
                                      from cost in leftTemp.DefaultIfEmpty()
                                      join cmoi in dbContext.CustomerMarginOptions on cost.CustomerMarginOptionId equals cmoi.CustomerMarginOptionId into leftOptionId
                                      from option in leftOptionId.DefaultIfEmpty()
                                      where ls.OperationZoneId == OperationZoneId &&
                                            ls.ModuleType == ModuleType &&
                                            ls.LogisticCompany == CourierCompany &&
                                            ls.LogisticType != FrayteLogisticType.UKShipment &&
                                            ls.IsActive == true &&
                                            cl.UserId == UserId
                                      orderby ls.MarginOrder ascending
                                      select new FrayteCustomerMarginCost
                                      {
                                          UserId = UserId,
                                          OperationZoneId = ls.OperationZoneId,
                                          CustomerMarginOptionId = (option == null ? 0 : option.CustomerMarginOptionId),
                                          CourierCompany = ls.LogisticCompany,
                                          CourierCompanyDisplay = ls.LogisticCompanyDisplay,
                                          RateType = ls.RateType,
                                          RateTypeDisplay = ls.RateTypeDisplay,
                                          LogisticType = ls.LogisticType,
                                          LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                          ModuleType = ls.ModuleType,
                                          CustomerMargin = new List<FrayteCustomerMargin>()
                                          {
                                              new FrayteCustomerMargin()
                                              {
                                                 Zone = new FrayteZone()
                                                 {
                                                     ZoneId = lsz.LogisticServiceZoneId,
                                                     OperationZoneId = ls.OperationZoneId,
                                                     ZoneName = lsz.ZoneName,
                                                     ZoneDisplayName = lsz.ZoneDisplayName,
                                                     ZoneColor = lsz.ZoneColor,
                                                     LogisticType = ls.LogisticType,
                                                     CourierComapny = ls.LogisticCompany,
                                                     RateType = ls.RateType,
                                                     ModuleType = ls.ModuleType
                                                },
                                                ShipmentType = new LogisticShipmentType()
                                                {
                                                     ShipmentTypeId = lsstTemp.LogisticServiceShipmentTypeId,
                                                     LogisticServiceId = lsstTemp.LogisticServiceId,
                                                     LogisticDescription = lsstTemp.LogisticDescription,
                                                     LogisticDescriptionDisplay = lsstTemp.LogisticDescriptionDisplayType,
                                                     LogisticType = lsstTemp.LogisticType,
                                                     RateType = ls.RateType,
                                                     RateTypeDisplay = ls.RateTypeDisplay
                                                },
                                                PercentOfMargin = (cost == null ? 0 : cost.Percentage)
                                             }
                                          },
                                          IsChange = false
                                      }).ToList();

                    if (CourierCompany == FrayteLogisticServiceType.DHL)
                    {
                        var margin = (from ls in dbContext.LogisticServices
                                      join cl in dbContext.CustomerLogistics on ls.LogisticServiceId equals cl.LogisticServiceId
                                      join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                                      join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                      join cmc in dbContext.CustomerMarginCosts.Where(p => p.CustomerId == UserId && p.OperationZoneId == OperationZoneId)
                                           on lsz.LogisticServiceZoneId equals cmc.LogisticServiceZoneId into leftTemp
                                      from cost in leftTemp.DefaultIfEmpty()
                                      join cmoi in dbContext.CustomerMarginOptions on cost.CustomerMarginOptionId equals cmoi.CustomerMarginOptionId into leftOptionId
                                      from option in leftOptionId.DefaultIfEmpty()
                                      where ls.OperationZoneId == OperationZoneId &&
                                            ls.LogisticCompany == CourierCompany &&
                                            ls.ModuleType == ModuleType &&
                                            ls.LogisticType == FrayteLogisticType.UKShipment &&
                                            ls.IsActive == true &&
                                            cl.UserId == UserId
                                      select new FrayteCustomerMarginCost
                                      {
                                          UserId = UserId,
                                          OperationZoneId = ls.OperationZoneId,
                                          CustomerMarginOptionId = (option == null ? 0 : option.CustomerMarginOptionId),
                                          CourierCompany = ls.LogisticCompany,
                                          CourierCompanyDisplay = ls.LogisticCompanyDisplay,
                                          RateType = ls.RateType,
                                          RateTypeDisplay = ls.RateTypeDisplay,
                                          LogisticType = ls.LogisticType,
                                          LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                          ModuleType = ls.ModuleType,
                                          CustomerMargin = new List<FrayteCustomerMargin>()
                                          {
                                              new FrayteCustomerMargin()
                                              {
                                                 Zone = new FrayteZone()
                                                 {
                                                     ZoneId = lsz.LogisticServiceZoneId,
                                                     OperationZoneId = ls.OperationZoneId,
                                                     ZoneName = lsz.ZoneName,
                                                     ZoneDisplayName = lsz.ZoneDisplayName,
                                                     ZoneColor = lsz.ZoneColor,
                                                     LogisticType = ls.LogisticType,
                                                     CourierComapny = ls.LogisticCompany,
                                                     RateType = ls.RateType,
                                                     ModuleType = ls.ModuleType
                                                },
                                                ShipmentType = new LogisticShipmentType()
                                                {
                                                     ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                     LogisticServiceId = lsst.LogisticServiceId,
                                                     LogisticDescription = lsst.LogisticDescription,
                                                     LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                     LogisticType = lsst.LogisticType,
                                                     RateType = ls.RateType,
                                                     RateTypeDisplay = ls.RateTypeDisplay
                                                },
                                                PercentOfMargin = (cost == null ? 0 : cost.Percentage)
                                             }
                                          },
                                          IsChange = false
                                      }).ToList();

                        if (margincost != null && margincost.Count > 0 || margin != null && margin.Count > 0)
                        {
                            var list = margincost
                                       .GroupBy(p => new { p.LogisticType, p.RateType })
                                       .Select(x => new FrayteCustomerMarginCost
                                       {
                                           RateType = x.Select(c => c.RateType).First(),
                                           RateTypeDisplay = x.Select(c => c.RateTypeDisplay).First(),
                                           LogisticType = x.Select(c => c.LogisticType).First(),
                                           LogisticTypeDisplay = x.Select(c => c.LogisticTypeDisplay).First(),
                                           UserId = x.Select(c => c.UserId).First(),
                                           OperationZoneId = x.Select(c => c.OperationZoneId).First(),
                                           CustomerMarginOptionId = x.Select(c => c.CustomerMarginOptionId).First(),
                                           CourierCompany = x.Select(c => c.CourierCompany).First(),
                                           CourierCompanyDisplay = x.Select(c => c.CourierCompanyDisplay).First(),
                                           ModuleType = x.Select(c => c.ModuleType).First(),
                                           IsChange = false,
                                           CustomerMargin = x.SelectMany(c => c.CustomerMargin)
                                                           .GroupBy(m => new { m.ShipmentType.ShipmentTypeId, m.Zone.ZoneId })
                                                           .Select(t => new FrayteCustomerMargin
                                                           {
                                                               Zone = t.Select(d => d.Zone).First(),
                                                               ShipmentType = t.Select(d => d.ShipmentType).First(),
                                                               PercentOfMargin = t.Select(d => d.PercentOfMargin).First()
                                                           }).ToList()
                                       }).ToList();

                            var Uklist = margin
                                        .GroupBy(p => new { p.CourierCompany, p.LogisticType })
                                        .Select(x => new FrayteCustomerMarginCost
                                        {
                                            RateType = x.Select(c => c.RateType).First(),
                                            RateTypeDisplay = x.Select(c => c.RateTypeDisplay).First(),
                                            LogisticType = x.Select(c => c.LogisticType).First(),
                                            LogisticTypeDisplay = x.Select(c => c.LogisticTypeDisplay).First(),
                                            UserId = x.Select(c => c.UserId).First(),
                                            OperationZoneId = x.Select(c => c.OperationZoneId).First(),
                                            CustomerMarginOptionId = x.Select(c => c.CustomerMarginOptionId).First(),
                                            CourierCompany = x.Select(c => c.CourierCompany).First(),
                                            CourierCompanyDisplay = x.Select(c => c.CourierCompanyDisplay).First(),
                                            ModuleType = x.Select(c => c.ModuleType).First(),
                                            IsChange = false,
                                            CustomerMargin = x.SelectMany(c => c.CustomerMargin)
                                                            .GroupBy(m => new { m.ShipmentType.ShipmentTypeId, m.Zone.ZoneId })
                                                            .Select(t => new FrayteCustomerMargin
                                                            {
                                                                Zone = t.Select(d => d.Zone).First(),
                                                                ShipmentType = t.Select(d => d.ShipmentType).First(),
                                                                PercentOfMargin = t.Select(d => d.PercentOfMargin).First()
                                                            }).ToList()
                                        }).ToList();

                            _cusMargin = list.Concat(Uklist).ToList();
                        }
                    }
                    else
                    {
                        var margin = (from ls in dbContext.LogisticServices
                                      join cl in dbContext.CustomerLogistics on ls.LogisticServiceId equals cl.LogisticServiceId
                                      join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                                      join cmc in dbContext.CustomerMarginCosts.Where(p => p.CustomerId == UserId && p.OperationZoneId == OperationZoneId)
                                           on lsz.LogisticServiceZoneId equals cmc.LogisticServiceZoneId into leftTemp
                                      from cost in leftTemp.DefaultIfEmpty()
                                      join cmoi in dbContext.CustomerMarginOptions on cost.CustomerMarginOptionId equals cmoi.CustomerMarginOptionId into leftOption
                                      from option in leftOption.DefaultIfEmpty()
                                      where ls.OperationZoneId == OperationZoneId &&
                                            ls.LogisticCompany == CourierCompany &&
                                            ls.ModuleType == ModuleType &&
                                            ls.LogisticType == FrayteLogisticType.UKShipment &&
                                            ls.IsActive == true &&
                                            cl.UserId == UserId
                                      select new FrayteCustomerMarginCost
                                      {
                                          UserId = UserId,
                                          OperationZoneId = ls.OperationZoneId,
                                          CustomerMarginOptionId = (option == null ? 0 : option.CustomerMarginOptionId),
                                          CourierCompany = ls.LogisticCompany,
                                          CourierCompanyDisplay = ls.LogisticCompanyDisplay,
                                          RateType = ls.RateType,
                                          RateTypeDisplay = ls.RateTypeDisplay,
                                          LogisticType = ls.LogisticType,
                                          LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                          ModuleType = ls.ModuleType,
                                          CustomerMargin = new List<FrayteCustomerMargin>()
                                          {
                                              new FrayteCustomerMargin()
                                              {
                                                 Zone = new FrayteZone()
                                                 {
                                                     ZoneId = lsz.LogisticServiceZoneId,
                                                     OperationZoneId = ls.OperationZoneId,
                                                     ZoneName = lsz.ZoneName,
                                                     ZoneDisplayName = lsz.ZoneDisplayName,
                                                     ZoneColor = lsz.ZoneColor,
                                                     LogisticType = ls.LogisticType,
                                                     CourierComapny = ls.LogisticCompany,
                                                     RateType = ls.RateType,
                                                     ModuleType = ls.ModuleType
                                                },
                                                ShipmentType = new LogisticShipmentType(),
                                                PercentOfMargin = (cost == null ? 0 : cost.Percentage)
                                             }
                                          },
                                          IsChange = false
                                      }).ToList();

                        if (margincost != null && margincost.Count > 0 || margin != null && margin.Count > 0)
                        {
                            var list = margincost
                                       .GroupBy(p => new { p.LogisticType, p.RateType })
                                       .Select(x => new FrayteCustomerMarginCost
                                       {
                                           RateType = x.Select(c => c.RateType).First(),
                                           RateTypeDisplay = x.Select(c => c.RateTypeDisplay).First(),
                                           LogisticType = x.Select(c => c.LogisticType).First(),
                                           LogisticTypeDisplay = x.Select(c => c.LogisticTypeDisplay).First(),
                                           UserId = x.Select(c => c.UserId).First(),
                                           OperationZoneId = x.Select(c => c.OperationZoneId).First(),
                                           CustomerMarginOptionId = x.Select(c => c.CustomerMarginOptionId).First(),
                                           CourierCompany = x.Select(c => c.CourierCompany).First(),
                                           CourierCompanyDisplay = x.Select(c => c.CourierCompanyDisplay).First(),
                                           ModuleType = x.Select(c => c.ModuleType).First(),
                                           IsChange = false,
                                           CustomerMargin = x.SelectMany(c => c.CustomerMargin)
                                                           .GroupBy(m => new { m.ShipmentType.ShipmentTypeId, m.Zone.ZoneId })
                                                           .Select(t => new FrayteCustomerMargin
                                                           {
                                                               Zone = t.Select(d => d.Zone).First(),
                                                               ShipmentType = t.Select(d => d.ShipmentType).First(),
                                                               PercentOfMargin = t.Select(d => d.PercentOfMargin).First()
                                                           }).ToList()
                                       }).ToList();

                            var Uklist = margin
                                        .GroupBy(p => new { p.CourierCompany, p.LogisticType })
                                        .Select(x => new FrayteCustomerMarginCost
                                        {
                                            RateType = x.Select(c => c.RateType).First(),
                                            RateTypeDisplay = x.Select(c => c.RateTypeDisplay).First(),
                                            LogisticType = x.Select(c => c.LogisticType).First(),
                                            LogisticTypeDisplay = x.Select(c => c.LogisticTypeDisplay).First(),
                                            UserId = x.Select(c => c.UserId).First(),
                                            OperationZoneId = x.Select(c => c.OperationZoneId).First(),
                                            CustomerMarginOptionId = x.Select(c => c.CustomerMarginOptionId).First(),
                                            CourierCompany = x.Select(c => c.CourierCompany).First(),
                                            CourierCompanyDisplay = x.Select(c => c.CourierCompanyDisplay).First(),
                                            ModuleType = x.Select(c => c.ModuleType).First(),
                                            IsChange = false,
                                            CustomerMargin = x.SelectMany(c => c.CustomerMargin)
                                                              .GroupBy(m => new { m.ShipmentType.ShipmentTypeId, m.Zone.ZoneId })
                                                              .Select(t => new FrayteCustomerMargin
                                                              {
                                                                  Zone = t.Select(d => d.Zone).First(),
                                                                  PercentOfMargin = t.Select(d => d.PercentOfMargin).First()
                                                              }).ToList()
                                        }).ToList();

                            _cusMargin = list.Concat(Uklist).ToList();
                        }
                    }
                }
                return _cusMargin;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<FrayteCustomerAdvanceMarginCost> CustomerAdvanceMarginCost(int OperationZoneId, int CustomerId, string LogisticCompany, string LogisticType, string RateType, string DocType, string ModuleType)
        {
            List<FrayteCustomerAdvanceMarginCost> _baserate = new List<FrayteCustomerAdvanceMarginCost>();

            try
            {
                var BaseRateCard = (from camc in dbContext.CustomerAdvanceMarginCosts
                                    join lsst in dbContext.LogisticServiceShipmentTypes on camc.LogisticServiceShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                                    join oz in dbContext.OperationZones on OperationZoneId equals oz.OperationZoneId
                                    join ls in dbContext.LogisticServices on lsst.LogisticServiceId equals ls.LogisticServiceId
                                    join lsw in dbContext.LogisticServiceWeights on camc.LogisticServiceWeightId equals lsw.LogisticServiceWeightId
                                    join lsz in dbContext.LogisticServiceZones on camc.LogisticServiceZoneId equals lsz.LogisticServiceZoneId

                                    where
                                          ls.LogisticType == LogisticType &&
                                          ls.LogisticCompany == LogisticCompany &&
                                          ls.ModuleType == ModuleType &&
                                          ls.OperationZoneId == OperationZoneId &&
                                          ls.RateType == RateType &&
                                          lsst.LogisticDescription == DocType &&
                                          lsst.LogisticType == LogisticType &&
                                          camc.CustomerId == CustomerId

                                    select new FrayteCustomerAdvanceMarginCost
                                    {
                                        OperationZoneId = camc.OperationZoneId,
                                        CustomerAdvanceMarginCostId = camc.CustomerAdvanceMarginCostId,
                                        zone = new FrayteZone
                                        {
                                            OperationZoneId = ls.OperationZoneId,
                                            ZoneId = (lsz == null ? 0 : lsz.LogisticServiceZoneId),
                                            ZoneName = (lsz == null ? "" : lsz.ZoneName),
                                            ZoneDisplayName = (lsz == null ? "" : lsz.ZoneDisplayName),
                                            LogisticType = ls.LogisticType,
                                            CourierComapny = ls.LogisticCompany,
                                            RateType = ls.RateType,
                                            ModuleType = ls.ModuleType
                                        },
                                        shipmentType = new LogisticShipmentType
                                        {
                                            ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                            LogisticServiceId = lsst.LogisticServiceId,
                                            LogisticType = lsst.LogisticType,
                                            LogisticDescription = lsst.LogisticDescription,
                                            LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType
                                        },
                                        LogisticWeight = new FrayteLogisticWeight
                                        {
                                            LogisticWeightId = lsw.LogisticServiceWeightId,
                                            WeightFrom = lsw.WeightFromDisplay,
                                            WeightTo = lsw.WeightToDisplay,
                                            UnitOfMeasurement = lsw.UOM,
                                            WeightUnit = lsw.WeightUnit,
                                            ShipmentType = new LogisticShipmentType
                                            {
                                                ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                LogisticServiceId = lsst.LogisticServiceId,
                                                LogisticType = lsst.LogisticType,
                                                LogisticDescription = lsst.LogisticDescription,
                                                LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType
                                            },
                                        },
                                        Rate = camc.AdvancePercentage,
                                        ModuleType = camc.ModuleType

                                    }).ToList();

                if (BaseRateCard != null && BaseRateCard.Count > 0)
                {
                    return BaseRateCard;
                }
                else
                {
                    var RateCard = (from ls in dbContext.LogisticServices
                                    join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                                    join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                    join lsw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lsw.LogisticServiceShipmentTypeId
                                    join oz in dbContext.OperationZones on OperationZoneId equals oz.OperationZoneId
                                    join camc in dbContext.CustomerAdvanceMarginCosts on new { lsst.LogisticServiceShipmentTypeId, lsw.LogisticServiceWeightId, lsz.LogisticServiceZoneId, CustomerId } equals
                                                                                         new { camc.LogisticServiceShipmentTypeId, camc.LogisticServiceWeightId, camc.LogisticServiceZoneId, camc.CustomerId } into leftJoin
                                    from rate in leftJoin.DefaultIfEmpty()

                                    where
                                          ls.LogisticType == LogisticType &&
                                          ls.LogisticCompany == LogisticCompany &&
                                          ls.ModuleType == ModuleType &&
                                          ls.OperationZoneId == OperationZoneId &&
                                          ls.RateType == RateType &&
                                          lsst.LogisticDescription == DocType &&
                                          lsst.LogisticType == LogisticType

                                    select new FrayteCustomerAdvanceMarginCost
                                    {
                                        OperationZoneId = OperationZoneId,
                                        CustomerAdvanceMarginCostId = (rate == null ? 0 : rate.CustomerAdvanceMarginCostId),
                                        zone = new FrayteZone
                                        {
                                            OperationZoneId = ls.OperationZoneId,
                                            ZoneId = (lsz == null ? 0 : lsz.LogisticServiceZoneId),
                                            ZoneName = (lsz == null ? "" : lsz.ZoneName),
                                            ZoneDisplayName = (lsz == null ? "" : lsz.ZoneDisplayName),
                                            LogisticType = ls.LogisticType,
                                            CourierComapny = ls.LogisticCompany,
                                            RateType = ls.RateType,
                                            ModuleType = ls.ModuleType
                                        },
                                        shipmentType = new LogisticShipmentType
                                        {
                                            ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                            LogisticServiceId = lsst.LogisticServiceId,
                                            LogisticType = lsst.LogisticType,
                                            LogisticDescription = lsst.LogisticDescription,
                                            LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType
                                        },
                                        LogisticWeight = new FrayteLogisticWeight
                                        {
                                            LogisticWeightId = lsw.LogisticServiceWeightId,
                                            WeightFrom = lsw.WeightFromDisplay,
                                            WeightTo = lsw.WeightToDisplay,
                                            UnitOfMeasurement = lsw.UOM,
                                            WeightUnit = lsw.WeightUnit,
                                            ShipmentType = new LogisticShipmentType
                                            {
                                                ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                LogisticServiceId = lsst.LogisticServiceId,
                                                LogisticType = lsst.LogisticType,
                                                LogisticDescription = lsst.LogisticDescription,
                                                LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType
                                            },
                                        },
                                        Rate = (rate == null ? 0 : rate.AdvancePercentage),
                                        ModuleType = (rate == null ? "DirectBooking" : rate.ModuleType)

                                    }).ToList();

                    return RateCard;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<FrayteCustomerAdvanceMarginCost> CustomerUKAdvanceMarginCost(int OperationZoneId, int CustomerId, string LogisticCompany, string LogisticType, string RateType, string PackageType, string ParcelType, string ModuleType)
        {
            List<FrayteCustomerAdvanceMarginCost> _baserate = new List<FrayteCustomerAdvanceMarginCost>();

            try
            {
                if (LogisticCompany == FrayteLogisticServiceType.Yodel)
                {
                    #region Yodel Advance Margin Rate

                    var BaseRateCard = (from camc in dbContext.CustomerAdvanceMarginCosts
                                        join lsst in dbContext.LogisticServiceShipmentTypes on camc.LogisticServiceShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                                        join oz in dbContext.OperationZones on OperationZoneId equals oz.OperationZoneId
                                        join ls in dbContext.LogisticServices on lsst.LogisticServiceId equals ls.LogisticServiceId
                                        join lsw in dbContext.LogisticServiceWeights on camc.LogisticServiceWeightId equals lsw.LogisticServiceWeightId
                                        join lsz in dbContext.LogisticServiceZones on camc.LogisticServiceZoneId equals lsz.LogisticServiceZoneId

                                        where
                                            ls.LogisticType == LogisticType &&
                                            ls.LogisticCompany == LogisticCompany &&
                                            ls.OperationZoneId == OperationZoneId &&
                                            ls.ModuleType == ModuleType &&
                                            camc.CustomerId == CustomerId

                                        select new FrayteCustomerAdvanceMarginCost
                                        {
                                            OperationZoneId = camc.OperationZoneId,
                                            CustomerAdvanceMarginCostId = camc.CustomerAdvanceMarginCostId,
                                            zone = new FrayteZone
                                            {
                                                OperationZoneId = ls.OperationZoneId,
                                                ZoneId = lsz.LogisticServiceZoneId,
                                                ZoneName = lsz.ZoneName,
                                                ZoneDisplayName = lsz.ZoneDisplayName,
                                                LogisticType = ls.LogisticType,
                                                CourierComapny = ls.LogisticCompany,
                                                RateType = ls.RateType,
                                                ModuleType = ls.ModuleType
                                            },

                                            shipmentType = new LogisticShipmentType
                                            {
                                                ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                LogisticServiceId = lsst.LogisticServiceId,
                                                LogisticType = lsst.LogisticType,
                                                LogisticDescription = lsst.LogisticDescription,
                                                LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                LogisticService = new LogisticShipmentService
                                                {
                                                    LogisticServiceId = ls.LogisticServiceId,
                                                    OperationZoneId = ls.OperationZoneId,
                                                    LogisticCompany = ls.LogisticCompany,
                                                    LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                    LogisticType = ls.LogisticType,
                                                    LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                    RateType = ls.RateType,
                                                    RateTypeDisplay = ls.RateTypeDisplay,
                                                    ModuleType = ls.ModuleType
                                                },
                                            },
                                            LogisticWeight = new FrayteLogisticWeight
                                            {
                                                LogisticWeightId = lsw.LogisticServiceWeightId,
                                                WeightFrom = lsw.WeightFromDisplay,
                                                WeightTo = lsw.WeightToDisplay,
                                                UnitOfMeasurement = lsw.UOM,
                                                WeightUnit = lsw.WeightUnit,
                                                ShipmentType = new LogisticShipmentType
                                                {
                                                    ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                    LogisticServiceId = lsst.LogisticServiceId,
                                                    LogisticType = lsst.LogisticType,
                                                    LogisticDescription = lsst.LogisticDescription,
                                                    LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                    LogisticService = new LogisticShipmentService
                                                    {
                                                        LogisticServiceId = ls.LogisticServiceId,
                                                        OperationZoneId = ls.OperationZoneId,
                                                        LogisticCompany = ls.LogisticCompany,
                                                        LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                        LogisticType = ls.LogisticType,
                                                        LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                        RateType = ls.RateType,
                                                        RateTypeDisplay = ls.RateTypeDisplay,
                                                        ModuleType = ls.ModuleType
                                                    },
                                                },
                                            },
                                            Rate = camc.AdvancePercentage,
                                            ModuleType = camc.ModuleType

                                        }).ToList();

                    if (BaseRateCard != null && BaseRateCard.Count > 0)
                    {
                        return BaseRateCard;
                    }
                    else
                    {
                        var RateCard = (from ls in dbContext.LogisticServices
                                        join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                                        join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                        join lsw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lsw.LogisticServiceShipmentTypeId
                                        join oz in dbContext.OperationZones on OperationZoneId equals oz.OperationZoneId
                                        join camc in dbContext.CustomerAdvanceMarginCosts on new { lsst.LogisticServiceShipmentTypeId, lsw.LogisticServiceWeightId, lsz.LogisticServiceZoneId, CustomerId } equals
                                                                                             new { camc.LogisticServiceShipmentTypeId, camc.LogisticServiceWeightId, camc.LogisticServiceZoneId, camc.CustomerId } into leftJoin
                                        from rate in leftJoin.DefaultIfEmpty()
                                        where
                                            ls.LogisticType == LogisticType &&
                                            ls.LogisticCompany == LogisticCompany &&
                                            ls.OperationZoneId == OperationZoneId &&
                                            ls.ModuleType == ModuleType &&
                                            lsw.PackageType == PackageType &&
                                            lsw.ParcelType == ParcelType

                                        select new FrayteCustomerAdvanceMarginCost
                                        {
                                            OperationZoneId = OperationZoneId,
                                            CustomerAdvanceMarginCostId = (rate == null ? 0 : rate.CustomerAdvanceMarginCostId),
                                            zone = new FrayteZone
                                            {
                                                OperationZoneId = ls.OperationZoneId,
                                                ZoneId = lsz.LogisticServiceZoneId,
                                                ZoneName = lsz.ZoneName,
                                                ZoneDisplayName = lsz.ZoneDisplayName,
                                                LogisticType = ls.LogisticType,
                                                CourierComapny = ls.LogisticCompany,
                                                RateType = ls.RateType,
                                                ModuleType = ls.ModuleType
                                            },

                                            shipmentType = new LogisticShipmentType
                                            {
                                                ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                LogisticServiceId = lsst.LogisticServiceId,
                                                LogisticType = lsst.LogisticType,
                                                LogisticDescription = lsst.LogisticDescription,
                                                LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                LogisticService = new LogisticShipmentService
                                                {
                                                    LogisticServiceId = ls.LogisticServiceId,
                                                    OperationZoneId = ls.OperationZoneId,
                                                    LogisticCompany = ls.LogisticCompany,
                                                    LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                    LogisticType = ls.LogisticType,
                                                    LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                    RateType = ls.RateType,
                                                    RateTypeDisplay = ls.RateTypeDisplay,
                                                    ModuleType = ls.ModuleType
                                                },
                                            },
                                            LogisticWeight = new FrayteLogisticWeight
                                            {
                                                LogisticWeightId = lsw.LogisticServiceWeightId,
                                                WeightFrom = lsw.WeightFromDisplay,
                                                WeightTo = lsw.WeightToDisplay,
                                                UnitOfMeasurement = lsw.UOM,
                                                WeightUnit = lsw.WeightUnit,
                                                ShipmentType = new LogisticShipmentType
                                                {
                                                    ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                    LogisticServiceId = lsst.LogisticServiceId,
                                                    LogisticType = lsst.LogisticType,
                                                    LogisticDescription = lsst.LogisticDescription,
                                                    LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                    LogisticService = new LogisticShipmentService
                                                    {
                                                        LogisticServiceId = ls.LogisticServiceId,
                                                        OperationZoneId = ls.OperationZoneId,
                                                        LogisticCompany = ls.LogisticCompany,
                                                        LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                        LogisticType = ls.LogisticType,
                                                        LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                        RateType = ls.RateType,
                                                        RateTypeDisplay = ls.RateTypeDisplay,
                                                        ModuleType = ls.ModuleType
                                                    },
                                                },
                                            },
                                            Rate = (rate == null ? 0 : rate.AdvancePercentage),
                                            ModuleType = (rate == null ? "DirectBooking" : rate.ModuleType)

                                        }).ToList();

                        return RateCard;
                    }

                    #endregion
                }
                else if (LogisticCompany == FrayteLogisticServiceType.Hermes)
                {
                    #region Hermes Advance Margin Rate

                    var BaseRateCard = (from camc in dbContext.CustomerAdvanceMarginCosts
                                        join lsst in dbContext.LogisticServiceShipmentTypes on camc.LogisticServiceShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                                        join lsz in dbContext.LogisticServiceZones on camc.LogisticServiceZoneId equals lsz.LogisticServiceZoneId
                                        join ls in dbContext.LogisticServices on lsz.LogisticServiceId equals ls.LogisticServiceId
                                        join lsw in dbContext.LogisticServiceWeights on camc.LogisticServiceWeightId equals lsw.LogisticServiceWeightId
                                        join lswl in dbContext.LogisticServiceWeightLimits on new { lsw.PackageType, lsw.ParcelType } equals
                                                                                              new { lswl.PackageType, lswl.ParcelType } into leftJoinLWL
                                        from ljWLL in leftJoinLWL.DefaultIfEmpty()

                                        where
                                             ls.LogisticType == LogisticType &&
                                             ls.LogisticCompany == LogisticCompany &&
                                             ls.OperationZoneId == OperationZoneId &&
                                             lsw.PackageType == PackageType &&
                                             camc.CustomerId == CustomerId

                                        select new FrayteCustomerAdvanceMarginCost
                                        {
                                            OperationZoneId = camc.OperationZoneId,
                                            CustomerAdvanceMarginCostId = camc.CustomerAdvanceMarginCostId,
                                            zone = new FrayteZone
                                            {
                                                OperationZoneId = ls.OperationZoneId,
                                                ZoneId = lsz.LogisticServiceZoneId,
                                                ZoneName = lsz.ZoneName,
                                                ZoneDisplayName = lsz.ZoneDisplayName,
                                                LogisticType = ls.LogisticType,
                                                CourierComapny = ls.LogisticCompany,
                                                RateType = ls.RateType,
                                                ModuleType = ls.ModuleType
                                            },

                                            shipmentType = new LogisticShipmentType
                                            {
                                                ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                LogisticServiceId = lsst.LogisticServiceId,
                                                LogisticType = lsst.LogisticType,
                                                LogisticDescription = lsst.LogisticDescription,
                                                LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                LogisticService = new LogisticShipmentService
                                                {
                                                    LogisticServiceId = ls.LogisticServiceId,
                                                    OperationZoneId = ls.OperationZoneId,
                                                    LogisticCompany = ls.LogisticCompany,
                                                    LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                    LogisticType = ls.LogisticType,
                                                    LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                    RateType = ls.RateType,
                                                    RateTypeDisplay = ls.RateTypeDisplay,
                                                    ModuleType = ls.ModuleType
                                                },
                                            },
                                            LogisticWeight = new FrayteLogisticWeight
                                            {
                                                LogisticWeightId = lsw.LogisticServiceWeightId,
                                                WeightFrom = lsw.WeightFromDisplay,
                                                WeightTo = lsw.WeightToDisplay,
                                                UnitOfMeasurement = lsw.UOM,
                                                WeightUnit = lsw.WeightUnit,
                                                ShipmentType = new LogisticShipmentType
                                                {
                                                    ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                    LogisticServiceId = lsst.LogisticServiceId,
                                                    LogisticType = lsst.LogisticType,
                                                    LogisticDescription = lsst.LogisticDescription,
                                                    LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                    LogisticService = new LogisticShipmentService
                                                    {
                                                        LogisticServiceId = ls.LogisticServiceId,
                                                        OperationZoneId = ls.OperationZoneId,
                                                        LogisticCompany = ls.LogisticCompany,
                                                        LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                        LogisticType = ls.LogisticType,
                                                        LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                        RateType = ls.RateType,
                                                        RateTypeDisplay = ls.RateTypeDisplay,
                                                        ModuleType = ls.ModuleType
                                                    },
                                                },
                                                PackageType = lsw.PackageType,
                                                ParcelType = lsw.ParcelType,
                                                WeightLimit = ljWLL == null ? 0 : ljWLL.WeightLimit
                                            },
                                            Rate = camc.AdvancePercentage,
                                            ModuleType = camc.ModuleType

                                        }).ToList();

                    if (BaseRateCard != null && BaseRateCard.Count > 0)
                    {
                        return BaseRateCard;
                    }
                    else
                    {
                        var RateCard = (from ls in dbContext.LogisticServices
                                        join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                                        join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                        join lsw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lsw.LogisticServiceShipmentTypeId
                                        join lswl in dbContext.LogisticServiceWeightLimits on new { lsw.PackageType, lsw.ParcelType } equals
                                                                                              new { lswl.PackageType, lswl.ParcelType } into leftJoinLWL
                                        from ljWLL in leftJoinLWL.DefaultIfEmpty()
                                        join oz in dbContext.OperationZones on OperationZoneId equals oz.OperationZoneId
                                        join camc in dbContext.CustomerAdvanceMarginCosts on new { lsst.LogisticServiceShipmentTypeId, lsw.LogisticServiceWeightId, lsz.LogisticServiceZoneId, CustomerId } equals
                                                                                             new { camc.LogisticServiceShipmentTypeId, camc.LogisticServiceWeightId, camc.LogisticServiceZoneId, camc.CustomerId } into leftJoin
                                        from rate in leftJoin.DefaultIfEmpty()
                                        where
                                             ls.LogisticType == LogisticType &&
                                             ls.LogisticCompany == LogisticCompany &&
                                             ls.OperationZoneId == OperationZoneId &&
                                             lsw.PackageType == PackageType

                                        select new FrayteCustomerAdvanceMarginCost
                                        {
                                            OperationZoneId = OperationZoneId,
                                            CustomerAdvanceMarginCostId = (rate == null ? 0 : rate.CustomerAdvanceMarginCostId),
                                            zone = new FrayteZone
                                            {
                                                OperationZoneId = ls.OperationZoneId,
                                                ZoneId = lsz.LogisticServiceZoneId,
                                                ZoneName = lsz.ZoneName,
                                                ZoneDisplayName = lsz.ZoneDisplayName,
                                                LogisticType = ls.LogisticType,
                                                CourierComapny = ls.LogisticCompany,
                                                RateType = ls.RateType,
                                                ModuleType = ls.ModuleType
                                            },

                                            shipmentType = new LogisticShipmentType
                                            {
                                                ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                LogisticServiceId = lsst.LogisticServiceId,
                                                LogisticType = lsst.LogisticType,
                                                LogisticDescription = lsst.LogisticDescription,
                                                LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                LogisticService = new LogisticShipmentService
                                                {
                                                    LogisticServiceId = ls.LogisticServiceId,
                                                    OperationZoneId = ls.OperationZoneId,
                                                    LogisticCompany = ls.LogisticCompany,
                                                    LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                    LogisticType = ls.LogisticType,
                                                    LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                    RateType = ls.RateType,
                                                    RateTypeDisplay = ls.RateTypeDisplay,
                                                    ModuleType = ls.ModuleType
                                                },
                                            },
                                            LogisticWeight = new FrayteLogisticWeight
                                            {
                                                LogisticWeightId = lsw.LogisticServiceWeightId,
                                                WeightFrom = lsw.WeightFromDisplay,
                                                WeightTo = lsw.WeightToDisplay,
                                                UnitOfMeasurement = lsw.UOM,
                                                WeightUnit = lsw.WeightUnit,
                                                ShipmentType = new LogisticShipmentType
                                                {
                                                    ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                    LogisticServiceId = lsst.LogisticServiceId,
                                                    LogisticType = lsst.LogisticType,
                                                    LogisticDescription = lsst.LogisticDescription,
                                                    LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                    LogisticService = new LogisticShipmentService
                                                    {
                                                        LogisticServiceId = ls.LogisticServiceId,
                                                        OperationZoneId = ls.OperationZoneId,
                                                        LogisticCompany = ls.LogisticCompany,
                                                        LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                        LogisticType = ls.LogisticType,
                                                        LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                        RateType = ls.RateType,
                                                        RateTypeDisplay = ls.RateTypeDisplay,
                                                        ModuleType = ls.ModuleType
                                                    },
                                                },
                                                PackageType = lsw.PackageType,
                                                ParcelType = lsw.ParcelType,
                                                WeightLimit = ljWLL == null ? 0 : ljWLL.WeightLimit
                                            },
                                            Rate = (rate == null ? 0 : rate.AdvancePercentage),
                                            ModuleType = (rate == null ? "DirectBooking" : rate.ModuleType)

                                        }).ToList();

                        return RateCard;
                    }

                    #endregion
                }
                else if (LogisticCompany == FrayteLogisticServiceType.DHL)
                {
                    #region DHL Advance Margin Rate

                    var BaseRateCard = (from camc in dbContext.CustomerAdvanceMarginCosts
                                        join lsst in dbContext.LogisticServiceShipmentTypes on camc.LogisticServiceShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                                        join lsz in dbContext.LogisticServiceZones on camc.LogisticServiceZoneId equals lsz.LogisticServiceZoneId
                                        join ls in dbContext.LogisticServices on lsz.LogisticServiceId equals ls.LogisticServiceId
                                        join lsw in dbContext.LogisticServiceWeights on camc.LogisticServiceWeightId equals lsw.LogisticServiceWeightId

                                        where
                                             ls.LogisticType == LogisticType &&
                                             ls.LogisticCompany == LogisticCompany &&
                                             ls.RateType == RateType &&
                                             ls.OperationZoneId == OperationZoneId &&
                                             camc.CustomerId == CustomerId

                                        select new FrayteCustomerAdvanceMarginCost
                                        {
                                            OperationZoneId = camc.OperationZoneId,
                                            CustomerAdvanceMarginCostId = camc.CustomerAdvanceMarginCostId,
                                            zone = new FrayteZone
                                            {
                                                OperationZoneId = ls.OperationZoneId,
                                                ZoneId = lsz.LogisticServiceZoneId,
                                                ZoneName = lsz.ZoneName,
                                                ZoneDisplayName = lsz.ZoneDisplayName,
                                                LogisticType = ls.LogisticType,
                                                CourierComapny = ls.LogisticCompany,
                                                RateType = ls.RateType,
                                                ModuleType = ls.ModuleType
                                            },
                                            shipmentType = new LogisticShipmentType
                                            {
                                                ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                LogisticServiceId = lsst.LogisticServiceId,
                                                LogisticType = lsst.LogisticType,
                                                LogisticDescription = lsst.LogisticDescription,
                                                LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                LogisticService = new LogisticShipmentService
                                                {
                                                    LogisticServiceId = ls.LogisticServiceId,
                                                    OperationZoneId = ls.OperationZoneId,
                                                    LogisticCompany = ls.LogisticCompany,
                                                    LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                    LogisticType = ls.LogisticType,
                                                    LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                    RateType = ls.RateType,
                                                    RateTypeDisplay = ls.RateTypeDisplay,
                                                    ModuleType = ls.ModuleType
                                                },
                                            },
                                            LogisticWeight = new FrayteLogisticWeight
                                            {
                                                LogisticWeightId = lsw.LogisticServiceWeightId,
                                                WeightFrom = lsw.WeightFromDisplay,
                                                WeightTo = lsw.WeightToDisplay,
                                                UnitOfMeasurement = lsw.UOM,
                                                WeightUnit = lsw.WeightUnit,
                                                ShipmentType = new LogisticShipmentType
                                                {
                                                    ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                    LogisticServiceId = lsst.LogisticServiceId,
                                                    LogisticType = lsst.LogisticType,
                                                    LogisticDescription = lsst.LogisticDescription,
                                                    LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                    LogisticService = new LogisticShipmentService
                                                    {
                                                        LogisticServiceId = ls.LogisticServiceId,
                                                        OperationZoneId = ls.OperationZoneId,
                                                        LogisticCompany = ls.LogisticCompany,
                                                        LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                        LogisticType = ls.LogisticType,
                                                        LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                        RateType = ls.RateType,
                                                        RateTypeDisplay = ls.RateTypeDisplay,
                                                        ModuleType = ls.ModuleType
                                                    },
                                                },
                                                PackageType = lsw.PackageType,
                                                ParcelType = lsw.ParcelType,
                                                WeightLimit = 0
                                            },
                                            Rate = camc.AdvancePercentage,
                                            ModuleType = camc.ModuleType
                                        }).ToList();

                    if (BaseRateCard != null && BaseRateCard.Count > 0)
                    {
                        return BaseRateCard;
                    }
                    else
                    {
                        var RateCard = (from ls in dbContext.LogisticServices
                                        join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                                        join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                        join lsw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lsw.LogisticServiceShipmentTypeId
                                        join oz in dbContext.OperationZones on OperationZoneId equals oz.OperationZoneId
                                        join camc in dbContext.CustomerAdvanceMarginCosts on new { lsst.LogisticServiceShipmentTypeId, lsw.LogisticServiceWeightId, lsz.LogisticServiceZoneId, CustomerId } equals
                                                                                             new { camc.LogisticServiceShipmentTypeId, camc.LogisticServiceWeightId, camc.LogisticServiceZoneId, camc.CustomerId } into leftJoin
                                        from rate in leftJoin.DefaultIfEmpty()
                                        where
                                             ls.LogisticType == LogisticType &&
                                             ls.LogisticCompany == LogisticCompany &&
                                             ls.RateType == RateType &&
                                             ls.OperationZoneId == OperationZoneId

                                        select new FrayteCustomerAdvanceMarginCost
                                        {
                                            OperationZoneId = OperationZoneId,
                                            CustomerAdvanceMarginCostId = (rate == null ? 0 : rate.CustomerAdvanceMarginCostId),
                                            zone = new FrayteZone
                                            {
                                                OperationZoneId = ls.OperationZoneId,
                                                ZoneId = lsz.LogisticServiceZoneId,
                                                ZoneName = lsz.ZoneName,
                                                ZoneDisplayName = lsz.ZoneDisplayName,
                                                LogisticType = ls.LogisticType,
                                                CourierComapny = ls.LogisticCompany,
                                                RateType = ls.RateType,
                                                ModuleType = ls.ModuleType
                                            },
                                            shipmentType = new LogisticShipmentType
                                            {
                                                ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                LogisticServiceId = lsst.LogisticServiceId,
                                                LogisticType = lsst.LogisticType,
                                                LogisticDescription = lsst.LogisticDescription,
                                                LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                LogisticService = new LogisticShipmentService
                                                {
                                                    LogisticServiceId = ls.LogisticServiceId,
                                                    OperationZoneId = ls.OperationZoneId,
                                                    LogisticCompany = ls.LogisticCompany,
                                                    LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                    LogisticType = ls.LogisticType,
                                                    LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                    RateType = ls.RateType,
                                                    RateTypeDisplay = ls.RateTypeDisplay,
                                                    ModuleType = ls.ModuleType
                                                },
                                            },
                                            LogisticWeight = new FrayteLogisticWeight
                                            {
                                                LogisticWeightId = lsw.LogisticServiceWeightId,
                                                WeightFrom = lsw.WeightFromDisplay,
                                                WeightTo = lsw.WeightToDisplay,
                                                UnitOfMeasurement = lsw.UOM,
                                                WeightUnit = lsw.WeightUnit,
                                                ShipmentType = new LogisticShipmentType
                                                {
                                                    ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                    LogisticServiceId = lsst.LogisticServiceId,
                                                    LogisticType = lsst.LogisticType,
                                                    LogisticDescription = lsst.LogisticDescription,
                                                    LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                    LogisticService = new LogisticShipmentService
                                                    {
                                                        LogisticServiceId = ls.LogisticServiceId,
                                                        OperationZoneId = ls.OperationZoneId,
                                                        LogisticCompany = ls.LogisticCompany,
                                                        LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                        LogisticType = ls.LogisticType,
                                                        LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                        RateType = ls.RateType,
                                                        RateTypeDisplay = ls.RateTypeDisplay,
                                                        ModuleType = ls.ModuleType
                                                    },
                                                },
                                                PackageType = lsw.PackageType,
                                                ParcelType = lsw.ParcelType,
                                                WeightLimit = 0
                                            },
                                            Rate = (rate == null ? 0 : rate.AdvancePercentage),
                                            ModuleType = (rate == null ? "DirectBooking" : rate.ModuleType)
                                        }).ToList();

                        return RateCard;
                    }
                    #endregion
                }
                else
                {
                    #region UKMail Advance Margin Rate

                    var BaseRateCard = (from camc in dbContext.CustomerAdvanceMarginCosts
                                        join lsst in dbContext.LogisticServiceShipmentTypes on camc.LogisticServiceShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                                        join lsz in dbContext.LogisticServiceZones on camc.LogisticServiceZoneId equals lsz.LogisticServiceZoneId
                                        join ls in dbContext.LogisticServices on lsz.LogisticServiceId equals ls.LogisticServiceId
                                        join lsw in dbContext.LogisticServiceWeights on camc.LogisticServiceWeightId equals lsw.LogisticServiceWeightId
                                        join lswl in dbContext.LogisticServiceWeightLimits on new { lsw.PackageType, lsw.ParcelType } equals
                                                                                              new { lswl.PackageType, lswl.ParcelType } into leftJoinLWL
                                        from ljWLL in leftJoinLWL.DefaultIfEmpty()

                                        where
                                             ls.LogisticType == LogisticType &&
                                             ls.LogisticCompany == LogisticCompany &&
                                             ls.OperationZoneId == OperationZoneId &&
                                             lsw.PackageType == PackageType &&
                                             lsw.ParcelType == ParcelType &&
                                             camc.CustomerId == CustomerId

                                        select new FrayteCustomerAdvanceMarginCost
                                        {
                                            OperationZoneId = camc.OperationZoneId,
                                            CustomerAdvanceMarginCostId = camc.CustomerAdvanceMarginCostId,
                                            zone = new FrayteZone
                                            {
                                                OperationZoneId = ls.OperationZoneId,
                                                ZoneId = lsz.LogisticServiceZoneId,
                                                ZoneName = lsz.ZoneName,
                                                ZoneDisplayName = lsz.ZoneDisplayName,
                                                LogisticType = ls.LogisticType,
                                                CourierComapny = ls.LogisticCompany,
                                                RateType = ls.RateType,
                                                ModuleType = ls.ModuleType
                                            },

                                            shipmentType = new LogisticShipmentType
                                            {
                                                ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                LogisticServiceId = lsst.LogisticServiceId,
                                                LogisticType = lsst.LogisticType,
                                                LogisticDescription = lsst.LogisticDescription,
                                                LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                LogisticService = new LogisticShipmentService
                                                {
                                                    LogisticServiceId = ls.LogisticServiceId,
                                                    OperationZoneId = ls.OperationZoneId,
                                                    LogisticCompany = ls.LogisticCompany,
                                                    LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                    LogisticType = ls.LogisticType,
                                                    LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                    RateType = ls.RateType,
                                                    RateTypeDisplay = ls.RateTypeDisplay,
                                                    ModuleType = ls.ModuleType
                                                },
                                            },
                                            LogisticWeight = new FrayteLogisticWeight
                                            {
                                                LogisticWeightId = lsw.LogisticServiceWeightId,
                                                WeightFrom = lsw.WeightFromDisplay,
                                                WeightTo = lsw.WeightToDisplay,
                                                UnitOfMeasurement = lsw.UOM,
                                                WeightUnit = lsw.WeightUnit,
                                                ShipmentType = new LogisticShipmentType
                                                {
                                                    ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                    LogisticServiceId = lsst.LogisticServiceId,
                                                    LogisticType = lsst.LogisticType,
                                                    LogisticDescription = lsst.LogisticDescription,
                                                    LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                    LogisticService = new LogisticShipmentService
                                                    {
                                                        LogisticServiceId = ls.LogisticServiceId,
                                                        OperationZoneId = ls.OperationZoneId,
                                                        LogisticCompany = ls.LogisticCompany,
                                                        LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                        LogisticType = ls.LogisticType,
                                                        LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                        RateType = ls.RateType,
                                                        RateTypeDisplay = ls.RateTypeDisplay,
                                                        ModuleType = ls.ModuleType
                                                    },
                                                },
                                                PackageType = lsw.PackageType,
                                                ParcelType = lsw.ParcelType,
                                                WeightLimit = ljWLL == null ? 0 : ljWLL.WeightLimit
                                            },
                                            Rate = camc.AdvancePercentage,
                                            ModuleType = camc.ModuleType

                                        }).ToList();

                    if (BaseRateCard != null && BaseRateCard.Count > 0)
                    {
                        return BaseRateCard;
                    }
                    else
                    {
                        var RateCard = (from ls in dbContext.LogisticServices
                                        join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                                        join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                                        join lsw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lsw.LogisticServiceShipmentTypeId
                                        join lswl in dbContext.LogisticServiceWeightLimits on new { lsw.PackageType, lsw.ParcelType } equals
                                                                                              new { lswl.PackageType, lswl.ParcelType } into leftJoinLWL
                                        from ljWLL in leftJoinLWL.DefaultIfEmpty()
                                        join camc in dbContext.CustomerAdvanceMarginCosts on new { lsst.LogisticServiceShipmentTypeId, lsw.LogisticServiceWeightId, lsz.LogisticServiceZoneId, CustomerId } equals
                                                                                             new { camc.LogisticServiceShipmentTypeId, camc.LogisticServiceWeightId, camc.LogisticServiceZoneId, camc.CustomerId } into leftJoin
                                        from rate in leftJoin.DefaultIfEmpty()
                                        where
                                             ls.LogisticType == LogisticType &&
                                             ls.LogisticCompany == LogisticCompany &&
                                             ls.OperationZoneId == OperationZoneId &&
                                             lsw.PackageType == PackageType &&
                                             lsw.ParcelType == ParcelType

                                        select new FrayteCustomerAdvanceMarginCost
                                        {
                                            OperationZoneId = OperationZoneId,
                                            CustomerAdvanceMarginCostId = (rate == null ? 0 : rate.CustomerAdvanceMarginCostId),
                                            zone = new FrayteZone
                                            {
                                                OperationZoneId = ls.OperationZoneId,
                                                ZoneId = lsz.LogisticServiceZoneId,
                                                ZoneName = lsz.ZoneName,
                                                ZoneDisplayName = lsz.ZoneDisplayName,
                                                LogisticType = ls.LogisticType,
                                                CourierComapny = ls.LogisticCompany,
                                                RateType = ls.RateType,
                                                ModuleType = ls.ModuleType
                                            },

                                            shipmentType = new LogisticShipmentType
                                            {
                                                ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                LogisticServiceId = lsst.LogisticServiceId,
                                                LogisticType = lsst.LogisticType,
                                                LogisticDescription = lsst.LogisticDescription,
                                                LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                LogisticService = new LogisticShipmentService
                                                {
                                                    LogisticServiceId = ls.LogisticServiceId,
                                                    OperationZoneId = ls.OperationZoneId,
                                                    LogisticCompany = ls.LogisticCompany,
                                                    LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                    LogisticType = ls.LogisticType,
                                                    LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                    RateType = ls.RateType,
                                                    RateTypeDisplay = ls.RateTypeDisplay,
                                                    ModuleType = ls.ModuleType
                                                },
                                            },
                                            LogisticWeight = new FrayteLogisticWeight
                                            {
                                                LogisticWeightId = lsw.LogisticServiceWeightId,
                                                WeightFrom = lsw.WeightFromDisplay,
                                                WeightTo = lsw.WeightToDisplay,
                                                UnitOfMeasurement = lsw.UOM,
                                                WeightUnit = lsw.WeightUnit,
                                                ShipmentType = new LogisticShipmentType
                                                {
                                                    ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                    LogisticServiceId = lsst.LogisticServiceId,
                                                    LogisticType = lsst.LogisticType,
                                                    LogisticDescription = lsst.LogisticDescription,
                                                    LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                    LogisticService = new LogisticShipmentService
                                                    {
                                                        LogisticServiceId = ls.LogisticServiceId,
                                                        OperationZoneId = ls.OperationZoneId,
                                                        LogisticCompany = ls.LogisticCompany,
                                                        LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                        LogisticType = ls.LogisticType,
                                                        LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                        RateType = ls.RateType,
                                                        RateTypeDisplay = ls.RateTypeDisplay,
                                                        ModuleType = ls.ModuleType
                                                    },
                                                },
                                                PackageType = lsw.PackageType,
                                                ParcelType = lsw.ParcelType,
                                                WeightLimit = ljWLL == null ? 0 : ljWLL.WeightLimit
                                            },
                                            Rate = (rate == null ? 0 : rate.AdvancePercentage),
                                            ModuleType = (rate == null ? "DirectBooking" : rate.ModuleType)

                                        }).ToList();

                        return RateCard;
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<FrayteCustomerAdvanceMarginCost> CustomerEUAdvanceMarginCost(int OperationZoneId, int CustomerId, string LogisticCompany, string LogisticType, string RateType, string ModuleType)
        {
            return null;
        }

        public List<MarginOptions> GetMarginOption(int OperationZoneId, string LogisticCompany)
        {
            List<MarginOptions> option = new List<MarginOptions>();
            var result = (from mo in dbContext.CustomerMarginOptions
                          where mo.OperationZoneId == OperationZoneId &&
                                mo.LogisticCompany == LogisticCompany
                          select new MarginOptions
                          {
                              CustomerMarginOptionId = mo.CustomerMarginOptionId,
                              OperationZoneId = mo.OperationZoneId,
                              OptionName = mo.Options,
                              OptionNameDisplay = mo.OptionsDisplay
                          }).ToList();

            if (result != null && result.Count > 0)
            {
                option = result
                        .GroupBy(p => p.OptionName)
                        .Select(x => new MarginOptions
                        {
                            CustomerMarginOptionId = x.Select(c => c.CustomerMarginOptionId).First(),
                            OperationZoneId = x.Select(c => c.OperationZoneId).First(),
                            OptionName = x.Select(c => c.OptionName).First(),
                            OptionNameDisplay = x.Select(c => c.OptionNameDisplay).First()
                        }).ToList();

                return option;
            }
            return option;
        }

        public List<MarginOptions> GetMarginOption(int OperationZoneId)
        {
            List<MarginOptions> option = new List<MarginOptions>();
            var result = (from mo in dbContext.CustomerMarginOptions
                          where mo.OperationZoneId == OperationZoneId
                          select new MarginOptions
                          {
                              CustomerMarginOptionId = mo.CustomerMarginOptionId,
                              OperationZoneId = mo.OperationZoneId,
                              OptionName = mo.Options,
                              OptionNameDisplay = mo.OptionsDisplay
                          }).ToList();

            if (result != null && result.Count > 0)
            {
                option = result
                        .GroupBy(p => p.OptionName)
                        .Select(x => new MarginOptions
                        {
                            CustomerMarginOptionId = x.Select(c => c.CustomerMarginOptionId).First(),
                            OperationZoneId = x.Select(c => c.OperationZoneId).First(),
                            OptionName = x.Select(c => c.OptionName).First(),
                            OptionNameDisplay = x.Select(c => c.OptionNameDisplay).First()
                        }).ToList();

                return option;
            }
            return option;
        }

        public List<MarginOptions> GetMarginOptionPercentage(int OperationZoneId, string CourierCompany, string MarginOption)
        {
            List<MarginOptions> option = new List<MarginOptions>();
            option = (from mo in dbContext.CustomerMarginOptions
                      where mo.OperationZoneId == OperationZoneId &&
                            mo.LogisticCompany == CourierCompany &&
                            mo.Options == MarginOption
                      select new MarginOptions
                      {
                          CustomerMarginOptionId = mo.CustomerMarginOptionId,
                          OperationZoneId = mo.OperationZoneId,
                          OptionName = mo.Options,
                          OptionNameDisplay = mo.OptionsDisplay,
                          LogisticCompany = mo.LogisticCompany,
                          LogisticCompanyDisplay = mo.LogisticCompanyDisplay,
                          ShipmentType = mo.ShipmentType,
                          ShipmentTypeDisplay = mo.ShipmentTypeDisplayText,
                          MarginPercentage = mo.MarginPercentage
                      }).ToList();

            return option;
        }

        public void UpdateCustomer(int UserId)
        {
            var user = dbContext.Users.Find(UserId);
            if (user != null && user.UserId > 0)
            {
                user.IsActive = true;
                dbContext.Entry(user).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public FrayteCustomerMarginOptions CustomerMarginOptions(int OperationZoneId, string CourierCompany)
        {
            FrayteCustomerMarginOptions fcm;
            var item = dbContext.CustomerMarginOptions.Where(p => p.OperationZoneId == OperationZoneId && p.LogisticCompany == CourierCompany).ToList();
            if (item != null && item.Count > 0)
            {
                var comp = item.Select(p => new { p.OperationZoneId, p.LogisticCompany, p.LogisticCompanyDisplay }).FirstOrDefault();
                fcm = new FrayteCustomerMarginOptions();
                fcm.OperationZoneId = comp.OperationZoneId;
                fcm.LogisticCompany = comp.LogisticCompany;
                fcm.LogisticCompanyDisplay = comp.LogisticCompanyDisplay;

                var opt = item.GroupBy(x => new { x.Options, x.OptionsDisplay }).Select(y => new { y.Key.Options, y.Key.OptionsDisplay }).ToList();
                fcm.Options = new List<FrayetMarginOptions>();

                if (opt != null && opt.Count > 0)
                {
                    foreach (var Obj in opt)
                    {
                        FrayetMarginOptions fm = new FrayetMarginOptions();
                        fm.OptionName = Obj.Options;
                        fm.OptionDisplayName = Obj.OptionsDisplay;

                        var rate = item.Where(x => x.OperationZoneId == fcm.OperationZoneId && x.LogisticCompany == fcm.LogisticCompany && x.Options == Obj.Options).ToList();
                        fm.MarginRates = new List<FrayteMarginRates>();

                        if (rate != null && rate.Count > 0)
                        {
                            foreach (var rr in rate)
                            {
                                FrayteMarginRates fmr = new FrayteMarginRates();
                                {
                                    fmr.CustomerMarginOptionId = rr.CustomerMarginOptionId;
                                    fmr.ShipmentType = rr.ShipmentType;
                                    fmr.ShipmentTypeDisplayText = rr.ShipmentTypeDisplayText;
                                    fmr.MarginPercentage = rr.MarginPercentage;
                                }
                                fm.MarginRates.Add(fmr);
                            }
                        }
                        fcm.Options.Add(fm);
                    }
                }
                return fcm;
            }
            return null;
        }

        public List<FrayteCustomerLogisticCompany> CustomerMarginLogistic(int OperationZoneId, int UserId)
        {
            FrayteCustomerLogisticCompany frayte;
            List<FrayteCustomerLogisticCompany> _logistic = new List<FrayteCustomerLogisticCompany>();
            var item = (from ls in dbContext.LogisticServices
                        join cl in dbContext.CustomerLogistics on ls.LogisticServiceId equals cl.LogisticServiceId
                        where cl.UserId == UserId
                        select new
                        {
                            ls.LogisticCompany,
                            ls.LogisticCompanyDisplay
                        }).Distinct().ToList();
            if (item != null && item.Count > 0)
            {
                foreach (var Obj in item)
                {
                    frayte = new FrayteCustomerLogisticCompany();
                    frayte.LogisticCompany = Obj.LogisticCompany;
                    frayte.LogisticCompanyDisplay = Obj.LogisticCompanyDisplay;
                    _logistic.Add(frayte);
                }
                return _logistic;
            }
            return _logistic;
        }

        public List<FrayteCustomerLogisticCompany> MarginLogisticItem(int OperationZoneId)
        {
            FrayteCustomerLogisticCompany frayte;
            List<FrayteCustomerLogisticCompany> _logistic = new List<FrayteCustomerLogisticCompany>();
            var item = (from ls in dbContext.CustomerMarginOptions
                        where ls.OperationZoneId == OperationZoneId
                        select new
                        {
                            ls.LogisticCompany,
                            ls.LogisticCompanyDisplay
                        }).Distinct().ToList();

            if (item != null && item.Count > 0)
            {
                foreach (var Obj in item)
                {
                    frayte = new FrayteCustomerLogisticCompany();
                    frayte.LogisticCompany = Obj.LogisticCompany;
                    frayte.LogisticCompanyDisplay = Obj.LogisticCompanyDisplay;
                    _logistic.Add(frayte);
                }
                return _logistic;
            }
            return _logistic;
        }

        public void AddCustomerMarginOptions(FrayteCustomerMarginOptions MarginOptions)
        {

            CustomerMarginOption cmo;
            if (MarginOptions != null)
            {
                if (MarginOptions.OptionId != null && MarginOptions.OptionId.Count > 0)
                {
                    cmo = new CustomerMarginOption();
                    foreach (int Id in MarginOptions.OptionId)
                    {
                        var OptId = dbContext.CustomerMarginOptions.Where(p => p.CustomerMarginOptionId == Id).FirstOrDefault(x => x.CustomerMarginOptionId == Id);
                        cmo = OptId;
                        dbContext.CustomerMarginOptions.Remove(cmo);
                        dbContext.SaveChanges();
                    }
                }
                else
                {
                    foreach (var Obj in MarginOptions.Options)
                    {
                        foreach (var rr in Obj.MarginRates)
                        {
                            var MarginOpt = dbContext.CustomerMarginOptions.Where(p => p.CustomerMarginOptionId != rr.CustomerMarginOptionId);
                            if (rr.CustomerMarginOptionId > 0)
                            {
                                cmo = new CustomerMarginOption();
                                cmo.OperationZoneId = MarginOptions.OperationZoneId;
                                cmo.LogisticCompany = MarginOptions.LogisticCompany;
                                cmo.LogisticCompanyDisplay = MarginOptions.LogisticCompanyDisplay;
                                cmo.Options = Obj.OptionName;
                                cmo.OptionsDisplay = Obj.OptionDisplayName;
                                cmo.CustomerMarginOptionId = rr.CustomerMarginOptionId;
                                cmo.ShipmentType = rr.ShipmentType;
                                cmo.ShipmentTypeDisplayText = rr.ShipmentTypeDisplayText;
                                cmo.MarginPercentage = rr.MarginPercentage;
                                dbContext.Entry(cmo).State = System.Data.Entity.EntityState.Modified;
                            }
                            else
                            {
                                cmo = new CustomerMarginOption();
                                cmo.OperationZoneId = MarginOptions.OperationZoneId;
                                cmo.LogisticCompany = MarginOptions.LogisticCompany;
                                cmo.LogisticCompanyDisplay = MarginOptions.LogisticCompanyDisplay;
                                cmo.Options = Obj.OptionName;
                                cmo.OptionsDisplay = Obj.OptionDisplayName;
                                cmo.ShipmentType = rr.ShipmentType;
                                cmo.ShipmentTypeDisplayText = rr.ShipmentTypeDisplayText;
                                cmo.MarginPercentage = rr.MarginPercentage;
                                dbContext.CustomerMarginOptions.Add(cmo);
                            }
                            dbContext.SaveChanges();
                        }

                    }
                }
            }
        }

        public List<FrayteCustomerBaseRate> GetBaseRate(CustomerRate User)
        {
            List<FrayteCustomerBaseRate> _baserate = new List<FrayteCustomerBaseRate>();
            try
            {
                FrayteOperationZone OperationZone = UtilityRepository.GetOperationZone();
                var BaseRate = dbContext.spGet_CustomerBaseRateCard(User.LogisticServiceId, OperationZone.OperationZoneId, User.UserId).ToList();

                FrayteCustomerBaseRate customer;
                foreach (var item in BaseRate)
                {
                    customer = new FrayteCustomerBaseRate();
                    customer.ZoneId = item.LogisticServiceZoneId;
                    customer.ZoneName = item.ZoneName;
                    customer.ZoneDisplayName = item.ZoneDisplayName;
                    customer.CourierCompany = item.LogisticCompany;
                    customer.RateType = item.RateType;
                    customer.ShipmentTypeId = item.LogisticServiceShipmentTypeId;
                    customer.ShipmentType = item.LogisticDescription;
                    customer.ShipmentDisplayType = item.LogisticDescriptionDisplayType;
                    customer.ReportShipmentDisplay = item.LogisticDescriptionReportDisplayType;
                    customer.LogisticType = item.LogisticType;
                    customer.PackageType = item.PackageType;
                    customer.ParcelType = item.ParcelType;
                    customer.PackageDisplayType = item.PackageDisplayType;
                    customer.ParcelDisplayType = item.ParcelDisplayType;
                    customer.LogisticWeightId = item.LogisticServiceWeightId;
                    customer.WeightFrom = item.WeightFrom;
                    customer.Weight = item.WeightTo;
                    customer.Rate = item.LogisticRate > 0 ? Math.Round(item.LogisticRate, 2) : 0.0m;
                    customer.MarginCost = item.Percentage > 0 ? item.Percentage.Value : 0.0m;
                    customer.CustomerCurrency = item.CustomerCurrencyCode;
                    _baserate.Add(customer);
                }
                return _baserate;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<FrayteCustomerBaseRate> GetBaseRate(CustomerRate User, int OperationZoneId)
        {
            List<FrayteCustomerBaseRate> _baserate = new List<FrayteCustomerBaseRate>();
            try
            {
                var BaseRate = dbContext.spGet_CustomerBaseRateCard(User.LogisticServiceId, OperationZoneId, User.UserId).ToList();

                FrayteCustomerBaseRate customer;
                foreach (var item in BaseRate)
                {
                    customer = new FrayteCustomerBaseRate();
                    customer.ZoneId = item.LogisticServiceZoneId;
                    customer.ZoneName = item.ZoneName;
                    customer.ZoneDisplayName = item.ZoneDisplayName;
                    customer.CourierCompany = item.LogisticCompany;
                    customer.RateType = item.RateType;
                    customer.ShipmentTypeId = item.LogisticServiceShipmentTypeId;
                    customer.ShipmentType = item.LogisticDescription;
                    customer.ShipmentDisplayType = item.LogisticDescriptionDisplayType;
                    customer.ReportShipmentDisplay = item.LogisticDescriptionReportDisplayType;
                    customer.LogisticType = item.LogisticType;
                    customer.PackageType = item.PackageType;
                    customer.ParcelType = item.ParcelType;
                    customer.PackageDisplayType = item.PackageDisplayType;
                    customer.ParcelDisplayType = item.ParcelDisplayType;
                    customer.LogisticWeightId = item.LogisticServiceWeightId;
                    customer.WeightFrom = item.WeightFrom;
                    customer.Weight = item.WeightTo;
                    customer.Rate = item.LogisticRate > 0 ? Math.Round(item.LogisticRate, 2) : 0.0m;
                    customer.MarginCost = item.Percentage > 0 ? item.Percentage.Value : 0.0m;
                    customer.CustomerCurrency = item.CustomerCurrencyCode;
                    _baserate.Add(customer);
                }
                return _baserate;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<FrayteBaseRateZoneCountry> GetBaseRateZoneCountry(int LogisticServiceId)
        {
            List<FrayteBaseRateZoneCountry> _basecountry = new List<FrayteBaseRateZoneCountry>();
            try
            {
                var country = dbContext.Get_ZoneWiseCountry(LogisticServiceId).ToList();
                FrayteBaseRateZoneCountry basecountry;
                if (country != null && country.Count > 0)
                {
                    foreach (var Obj in country)
                    {
                        basecountry = new FrayteBaseRateZoneCountry();
                        basecountry.Country1 = Obj.CountryName1;
                        basecountry.ZoneName1 = Obj.ZoneDisplayName1;
                        basecountry.Country2 = Obj.CountryName2;
                        basecountry.ZoneName2 = Obj.ZoneDisplayName2;
                        basecountry.Country3 = Obj.CountryName3;
                        basecountry.ZoneName3 = Obj.ZoneDisplayName3;
                        basecountry.Country4 = Obj.CountryName4;
                        basecountry.ZoneName4 = Obj.ZoneDisplayName4;
                        _basecountry.Add(basecountry);
                    }
                }
                return _basecountry;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<CustomerAddOnRate> GetAddOnRate(int LogisticServiceId)
        {
            List<CustomerAddOnRate> _AddOnRate = new List<CustomerAddOnRate>();

            _AddOnRate = (from lsa in dbContext.LogisticServiceAddOns
                          join lswl in dbContext.LogisticServiceWeightLimits on lsa.LogisticServiceWeightLimitId equals lswl.LogisticServiceWeightLimitId
                          join lsz in dbContext.LogisticServiceZones on lsa.LogisticServiceZoneId equals lsz.LogisticServiceZoneId
                          join ls in dbContext.LogisticServices on lsa.LogisticServiceId equals ls.LogisticServiceId
                          where
                               ls.LogisticServiceId == LogisticServiceId
                          select new CustomerAddOnRate
                          {
                              AddOnRate = lsa.AddOnRate,
                              WeightFrom = lsa.WeightFrom,
                              WeightTo = lsa.WeightTo,
                              LogisticServiceZoneId = lsa.LogisticServiceZoneId,
                              LogisticServiceAddOnId = lsa.LogisticServiceAddOnId,
                              AddOnDescription = lswl.AddOnDescription
                          }).OrderBy(p => p.LogisticServiceAddOnId).ToList();

            return _AddOnRate;
        }

        public decimal GetCustomerMargin(int UserId, int LogisticServiceId, int OperationZoneId)
        {
            var customerdetail = (from ua in dbContext.UserAdditionals
                                  where ua.UserId == UserId
                                  select ua.CustomerRateCardType).FirstOrDefault();

            if (customerdetail != null)
            {
                if (customerdetail == FrayteCustomerRateCardType.ADVANCE)
                {
                    decimal margin = (from cm in dbContext.CustomerAdvanceMarginCosts
                                      join lsst in dbContext.LogisticServiceShipmentTypes on cm.LogisticServiceShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                                      join ls in dbContext.LogisticServices on lsst.LogisticServiceId equals ls.LogisticServiceId
                                      where cm.CustomerId == UserId &&
                                            cm.OperationZoneId == OperationZoneId &&
                                            ls.LogisticServiceId == LogisticServiceId
                                      select cm.AdvancePercentage).FirstOrDefault();

                    if (margin > 0.00m)
                    {
                        return margin;
                    }
                    else
                    {
                        return 0.00m;
                    }
                }
                else if (customerdetail == FrayteCustomerRateCardType.NORMAL)
                {
                    decimal margin = (from cm in dbContext.CustomerMarginCosts
                                      join lsst in dbContext.LogisticServiceShipmentTypes on cm.LogisticShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                                      join ls in dbContext.LogisticServices on lsst.LogisticServiceId equals ls.LogisticServiceId
                                      where cm.CustomerId == UserId &&
                                            cm.OperationZoneId == OperationZoneId &&
                                            ls.LogisticServiceId == LogisticServiceId
                                      select cm.Percentage).FirstOrDefault();

                    if (margin > 0.00m)
                    {
                        return margin;
                    }
                    else
                    {
                        return 0.00m;
                    }
                }
                else
                {
                    return 0.00m;
                }
            }
            else
            {
                return 0.00m;
            }
        }

        public FrayteManifestName DownloadSupplemantoryChargePdf(CustomerRate Rate)
        {
            FrayteManifestName name = new FrayteManifestName();

            var file = (from ls in dbContext.LogisticServices
                        where ls.LogisticServiceId == Rate.LogisticServiceId
                        select ls).FirstOrDefault();

            if (file.SupplemantoryFileName != null && file.SupplemantoryFileName != "")
            {
                name.FileName = file.SupplemantoryFileName;
                name.FilePath = AppSettings.WebApiPath + "SupplemantoryChargeFile/" + file.SupplemantoryFileName;
                name.FileStatus = true;
            }
            else
            {
                name.FileStatus = false;
            }

            return name;
        }

        public void SaveTNTRates(int LogisticServiceId, int OperationZoneId, string Currency)
        {
            var zone = dbContext.LogisticServiceZones.Where(p => p.LogisticServiceId == LogisticServiceId).ToList();
            if (zone != null)
            {
                foreach (var zz in zone)
                {
                    var ship = dbContext.LogisticServiceShipmentTypes.Where(p => p.LogisticServiceId == LogisticServiceId).ToList();
                    if (ship != null)
                    {
                        foreach (var ss in ship)
                        {
                            var courier = dbContext.LogisticServiceCourierAccounts.Where(p => p.LogisticServiceId == LogisticServiceId).ToList();
                            if (courier != null)
                            {
                                foreach (var cc in courier)
                                {
                                    var weight = dbContext.LogisticServiceWeights.Where(p => p.LogisticServiceShipmentTypeId == ss.LogisticServiceShipmentTypeId).ToList();
                                    if (weight != null)
                                    {
                                        foreach (var ww in weight)
                                        {
                                            LogisticServiceBaseRateCard lsbrc = new LogisticServiceBaseRateCard();
                                            lsbrc.OperationZoneId = OperationZoneId;
                                            lsbrc.LogisticServiceZoneId = zz.LogisticServiceZoneId;
                                            lsbrc.LogisticServiceWeightId = ww.LogisticServiceWeightId;
                                            lsbrc.LogisticServiceShipmentTypeId = ss.LogisticServiceShipmentTypeId;
                                            lsbrc.LogisticServiceDimensionId = 0;
                                            lsbrc.LogisticServiceCourierAccountId = cc.LogisticServiceCourierAccountId;
                                            lsbrc.LogisticRate = 10;
                                            lsbrc.LogisticCurrency = Currency;
                                            lsbrc.ModuleType = "DirectBooking";
                                            dbContext.LogisticServiceBaseRateCards.Add(lsbrc);
                                            dbContext.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public LogisticServiceDuration LogisticServiceDates(int LogisticServiceId)
        {
            LogisticServiceDuration duration = dbContext.LogisticServices.Where(p => p.LogisticServiceId == LogisticServiceId)
                                                                         .Select(p => new LogisticServiceDuration
                                                                         {
                                                                             LogisticServiceId = p.LogisticServiceId,
                                                                             IssuedDate = p.IssuedDate.Value,
                                                                             ExpiryDate = p.ExpiryDate.Value,
                                                                             RateTypeDisplay = p.RateTypeDisplay
                                                                         }).FirstOrDefault();

            return duration;
        }

        public FrayteCustomerApiDetail CustomerApi(int CustomerId)
        {
            var detail = (from uu in dbContext.Users
                          join ua in dbContext.UserAdditionals on uu.UserId equals ua.UserId
                          where uu.UserId == CustomerId
                          select new FrayteCustomerApiDetail
                          {
                              CustomerName = uu.ContactName,
                              APIKey = ua.APIKey,
                              AccountNo = ua.AccountNo
                          }).FirstOrDefault();

            return detail;
        }

        public bool CustomerCompanyDetail(int UserId)
        {
            var detail = (from ccd in dbContext.CustomerCompanyDetails
                          join u in dbContext.Users on ccd.UserId equals u.UserId
                          where u.UserId == UserId
                          select ccd).FirstOrDefault();

            if (detail != null && detail.UserId > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public FrayteSpecialCustomerCompanyDetail GetSpecialCustomerDetail(int UserId)
        {
            FrayteSpecialCustomerCompanyDetail detail = new FrayteSpecialCustomerCompanyDetail();
            detail = (from ccd in dbContext.CustomerCompanyDetails
                      where ccd.UserId == UserId
                      select new FrayteSpecialCustomerCompanyDetail
                      {
                          ComapnyName = ccd.CompanyName,
                          MainWebsite = ccd.MainWebsite,
                          OperationStaffName = ccd.OperationStaff,
                          OperationStaffEmail = ccd.OperationStaffEmail,
                          UserPosition = ccd.UserPosition,
                          PhoneNo = ccd.OperationStaffPhone
                      }).FirstOrDefault();
            return detail;
        }

        public List<FrayteCustomerAddressBook> GetAddressBookDetail(int CustomerId, System.Data.DataTable exceldata, ref List<string> _errorrows)
        {
            List<FrayteCustomerAddressBook> _addressbook = new List<FrayteCustomerAddressBook>();
            FrayteCustomerAddressBook addressbook;

            int RowNo = 2;
            foreach (DataRow detail in exceldata.Rows)
            {
                try
                {
                    addressbook = new FrayteCustomerAddressBook();
                    addressbook.CustomerId = CustomerId;
                    addressbook.FromAddress = false;
                    addressbook.ToAddress = true;
                    addressbook.FirstName = detail["FirstName"] != null ? CommonConversion.ConvertToString(detail, "FirstName") : "";
                    addressbook.LastName = detail["LastName"] != null ? CommonConversion.ConvertToString(detail, "LastName") : "";
                    addressbook.CompanyName = detail["CompanyName"] != null ? CommonConversion.ConvertToString(detail, "CompanyName") : "";
                    addressbook.Email = detail["Email"] != null ? CommonConversion.ConvertToString(detail, "Email") : "";
                    addressbook.Phone = detail["PhoneNo"] != null ? CommonConversion.ConvertToString(detail, "PhoneNo") : "";
                    addressbook.Address1 = detail["Address1"] != null ? CommonConversion.ConvertToString(detail, "Address1") : "";
                    addressbook.Area = detail["Area"] != null ? CommonConversion.ConvertToString(detail, "Area") : "";
                    addressbook.Address2 = detail["Address2"] != null ? CommonConversion.ConvertToString(detail, "Address2") : "";
                    string cityname = CommonConversion.ConvertToString(detail, "City");
                    if (cityname == null || cityname == string.Empty)
                    {
                        _errorrows.Add("In row No : " + RowNo + " City is required");
                    }
                    else
                    {
                        addressbook.City = detail["City"] != null ? CommonConversion.ConvertToString(detail, "City") : "";
                    }
                    addressbook.State = detail["State"] != null ? CommonConversion.ConvertToString(detail, "State") : "";
                    addressbook.Zip = detail["Zip"] != null ? CommonConversion.ConvertToString(detail, "Zip") : "";
                    string countryname = CommonConversion.ConvertToString(detail, "CountryName");
                    if (countryname == null || countryname == string.Empty)
                    {
                        _errorrows.Add("In row No : " + RowNo + " Country Name is required");
                    }
                    else
                    {
                        var country = dbContext.Countries.Where(p => p.CountryName == countryname).FirstOrDefault();
                        if (country != null && country.CountryId > 0)
                        {
                            addressbook.CountryId = country.CountryId;
                        }
                        else
                        {
                            _errorrows.Add("In row No : " + RowNo + " not a valid country name");
                        }
                    }
                    addressbook.IsActive = true;
                    addressbook.TableType = "AddressBook";
                    addressbook.IsFavorites = false;
                    addressbook.IsDefault = false;
                    _addressbook.Add(addressbook);
                    RowNo++;
                }
                catch
                {
                    _errorrows.Add("Row No : " + RowNo);
                    RowNo++;
                    continue;
                }
            }
            return _addressbook;
        }

        public List<string> ValidateAddressBookDataLength(List<FrayteCustomerAddressBook> _addressbook)
        {
            int RowNo = 2;
            List<string> _lengtherror = new List<string>();
            foreach (FrayteCustomerAddressBook book in _addressbook)
            {
                if (book.FirstName.Length > 50)
                {
                    _lengtherror.Add("In row No : " + RowNo + " column name FirstName data length is exceed from max length 50");
                }
                if (book.LastName.Length > 50)
                {
                    _lengtherror.Add("In row No : " + RowNo + " column name LastName data length is exceed from max length 50");
                }
                if (book.CompanyName.Length > 100)
                {
                    _lengtherror.Add("In row No : " + RowNo + " column name CompanyName data length is exceed from max length 100");
                }
                if (book.Email.Length > 100)
                {
                    _lengtherror.Add("In row No : " + RowNo + " column name Email data length is exceed from max length 100");
                }
                if (book.Phone.Length > 20)
                {
                    _lengtherror.Add("In row No : " + RowNo + " column name Phone data length is exceed from max length 20");
                }
                if (book.Address1.Length > 100)
                {
                    _lengtherror.Add("In row No : " + RowNo + " column name Address1 data length is exceed from max length 100");
                }
                if (book.Area.Length > 100)
                {
                    _lengtherror.Add("In row No : " + RowNo + " column name Area data length is exceed from max length 100");
                }
                if (book.Address2.Length > 100)
                {
                    _lengtherror.Add("In row No : " + RowNo + " column name Address2 data length is exceed from max length 100");
                }
                if (book.City.Length > 50)
                {
                    _lengtherror.Add("In row No : " + RowNo + " column name City data length is exceed from max length 50");
                }
                if (book.State.Length > 50)
                {
                    _lengtherror.Add("In row No : " + RowNo + " column name State data length is exceed from max length 50");
                }
                if (book.Zip.Length > 10)
                {
                    _lengtherror.Add("In row No : " + RowNo + " column name Zip data length is exceed from max length 10");
                }
                RowNo++;
            }

            if (_lengtherror.Count > 0)
            {
                return _lengtherror;
            }
            else
            {
                return _lengtherror;
            }
        }

        public bool InsertCustomerAddressBook(List<FrayteCustomerAddressBook> _addressbook)
        {
            try
            {
                AddressBook ad;
                foreach (FrayteCustomerAddressBook book in _addressbook)
                {
                    ad = new AddressBook();
                    ad.FromAddress = book.FromAddress;
                    ad.ToAddress = book.ToAddress;
                    ad.ContactFirstName = book.FirstName;
                    ad.ContactLastName = book.LastName;
                    ad.CompanyName = book.CompanyName;
                    ad.Email = book.Email;
                    ad.PhoneNo = book.Phone;
                    ad.Address1 = book.Address1;
                    ad.Area = book.Area;
                    ad.Address2 = book.Address2;
                    ad.City = book.City;
                    ad.State = book.State;
                    ad.Zip = book.Zip;
                    ad.CountryId = book.CountryId;
                    ad.IsActive = book.IsActive;
                    ad.TableType = book.TableType;
                    ad.IsFavorites = book.IsFavorites;
                    ad.IsDefault = book.IsDefault;
                    dbContext.AddressBooks.Add(ad);
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckUploadExcelColumns(string AddressBookColumn, DataTable dtExcelData)
        {
            if (dtExcelData != null)
            {
                if (dtExcelData.Columns.Count != AddressBookColumn.Split(',').Length)
                {
                    return false;
                }
                else
                {
                    foreach (DataColumn col in dtExcelData.Columns)
                    {
                        if (!AddressBookColumn.Split(',').Contains(col.ColumnName.Trim()))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
      
    }
}