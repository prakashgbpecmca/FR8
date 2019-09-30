using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Business
{
    public class ShipperRepository
    {
        public void SaveEasyPostAddressId(int userAddressId, string easyPostAddressId)
        {
            try
            {
                var UserAddress = dbContext.UserAddresses.Find(userAddressId);
                UserAddress.EasyPostAddressId = easyPostAddressId;
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {

            }
        }
        FrayteEntities dbContext = new FrayteEntities();
        public FrayteShipperReceiver GetShipperDetail(int shipperId)
        {
            FrayteShipperReceiver shipperReceiver = new FrayteShipperReceiver();
            WorkingWeekDay workingDays = new WorkingWeekDay();
            //Step 1: Get Shipper basic information
            var shipper = dbContext.Users.Where(p => p.UserId == shipperId).FirstOrDefault();

            if (shipper != null)
            {

                //Set User's bacic information
                shipperReceiver = UtilityRepository.ShipperReceiverMapping(shipper);
                // get Working Week day

                if (shipperReceiver.WorkingWeekDay.WorkingWeekDayId > 0)
                {
                    workingDays = dbContext.WorkingWeekDays.Find(shipperReceiver.WorkingWeekDay.WorkingWeekDayId);
                }


                if (workingDays != null)
                {

                    shipperReceiver.WorkingWeekDay = workingDays;

                }
                //Step 1.1: Get time zone
                var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == shipper.TimezoneId).FirstOrDefault();
                if (timeZone != null)
                {
                    shipperReceiver.Timezone = new TimeZoneModal();
                    shipperReceiver.Timezone.TimezoneId = timeZone.TimezoneId;
                    shipperReceiver.Timezone.Name = timeZone.Name;
                    shipperReceiver.Timezone.Offset = timeZone.Offset;
                    shipperReceiver.Timezone.OffsetShort = timeZone.OffsetShort;
                }

                //Step 2: Get Shipper's Address information
                var shipperAddress = dbContext.UserAddresses.Where(p => p.UserId == shipperId).ToList();
                if (shipperAddress != null)
                {
                    shipperReceiver.PickupAddresses = new List<FrayteAddress>();

                    foreach (UserAddress address in shipperAddress)
                    {
                        if (address.AddressTypeId == (int)FrayteAddressType.MainAddress)
                        {
                            //Step 2.1: Set Shipper.ShipperAddress
                            shipperReceiver.UserAddress = new FrayteAddress();
                            shipperReceiver.UserAddress = UtilityRepository.UserAddressMapping(address);

                            //Step : Get country information
                            var country = dbContext.Countries.Where(p => p.CountryId == address.CountryId).FirstOrDefault();
                            if (country != null)
                            {
                                shipperReceiver.UserAddress.Country = new FrayteCountryCode();
                                shipperReceiver.UserAddress.Country.CountryId = country.CountryId;
                                shipperReceiver.UserAddress.Country.Code = country.CountryCode;
                                shipperReceiver.UserAddress.Country.Name = country.CountryName;
                                shipperReceiver.UserAddress.Country.TimeZoneDetail = new TimeZoneModal();
                                if (country.TimeZoneId != null && country.TimeZoneId > 0)
                                {
                                    var time = dbContext.Timezones.Find(country.TimeZoneId);
                                    shipperReceiver.UserAddress.Country.TimeZoneDetail.Name = time.Name;
                                    shipperReceiver.UserAddress.Country.TimeZoneDetail.Offset = time.Offset;
                                    shipperReceiver.UserAddress.Country.TimeZoneDetail.OffsetShort = time.OffsetShort;
                                    shipperReceiver.UserAddress.Country.TimeZoneDetail.TimezoneId = time.TimezoneId;
                                }

                            }
                        }
                        else
                        {
                            //Step 2.2: Set Agent's other addresses
                            FrayteAddress otherAddress = UtilityRepository.UserAddressMapping(address);

                            //Step : Get country information
                            var country = dbContext.Countries.Where(p => p.CountryId == otherAddress.Country.CountryId).FirstOrDefault();
                            if (country != null)
                            {
                                otherAddress.Country = new FrayteCountryCode();
                                otherAddress.Country.CountryId = country.CountryId;
                                otherAddress.Country.Code = country.CountryCode;
                                otherAddress.Country.Name = country.CountryName;
                                otherAddress.Country.TimeZoneDetail = new TimeZoneModal();
                                if (country.TimeZoneId != null && country.TimeZoneId > 0)
                                {
                                    var time = dbContext.Timezones.Find(country.TimeZoneId);
                                    otherAddress.Country.TimeZoneDetail.Name = time.Name;
                                    otherAddress.Country.TimeZoneDetail.Offset = time.Offset;
                                    otherAddress.Country.TimeZoneDetail.OffsetShort = time.OffsetShort;
                                    otherAddress.Country.TimeZoneDetail.TimezoneId = time.TimezoneId;
                                }
                            }

                            shipperReceiver.PickupAddresses.Add(otherAddress);

                        }
                    }
                }
            }

            return shipperReceiver;

        }

        public List<FrayteUser> GetShipperList()
        {
            List<FrayteUser> users = new List<FrayteUser>();

            users = new FrayteUserRepository().GetUserTypeList((int)FrayteUserRole.Shipper, (int)FrayteAddressType.MainAddress);

            return users;
        }

        public List<FrayteUserModel> GetShipperList(string shipperName)
        {
            List<FrayteUserModel> users = new List<FrayteUserModel>();

            users = (from u in dbContext.Users
                     join r in dbContext.UserRoles on u.UserId equals r.UserId
                     where u.ContactName.Contains(shipperName)
                     && r.RoleId == (int)FrayteUserRole.Shipper
                     select new FrayteUserModel()
                     {
                         Name = u.ContactName,
                         UserId = u.UserId
                     }).ToList();


            return users;
        }

        public FrayteResult SaveShipper(FrayteShipperReceiver frayteUser)
        {
            FrayteResult result = new FrayteResult();
            FrayteUserRepository userRepository = new FrayteUserRepository();

            //Step 1: Save User Detail
            userRepository.SaveUserDetail(frayteUser);

            //Step 2: Save user role
            userRepository.SaveUserRole(frayteUser.UserId, (int)FrayteUserRole.Shipper);

            //Step 3: Save User Address information
            frayteUser.UserAddress.AddressTypeId = (int)FrayteAddressType.MainAddress;
            frayteUser.UserAddress.UserId = frayteUser.UserId;
            userRepository.SaveUserAddress(frayteUser.UserAddress);

            //Step 4: Save Shipper Pickup address information
            if (frayteUser.PickupAddresses != null && frayteUser.PickupAddresses.Count > 0)
            {
                foreach (FrayteAddress address in frayteUser.PickupAddresses)
                {
                    address.AddressTypeId = (int)FrayteAddressType.OtherAddress;
                    address.UserId = frayteUser.UserId;
                    userRepository.SaveUserAddress(address);
                }
            }

            result.Status = true;
            return result;
        }

        public FrayteResult DeleteShipper(int userId)
        {
            return new FrayteUserRepository().MarkForDelete(userId);
        }

        public FrayteAddress GetShipperMainAddress(int shipperId)
        {
            FrayteAddress shipperMainAddress = new FrayteAddress();

            var address = dbContext.UserAddresses.Where(p => p.UserId == shipperId && p.AddressTypeId == (int)FrayteAddressType.MainAddress).FirstOrDefault();

            if (address != null)
            {
                shipperMainAddress = UtilityRepository.UserAddressMapping(address);
            }

            return shipperMainAddress;
        }

        public List<FrayteAddress> GetShippeOtherAddresses(int shipperId)
        {
            List<FrayteAddress> shipperOtherAddresses = new List<FrayteAddress>();

            var addresses = dbContext.UserAddresses.Where(p => p.UserId == shipperId &&
                            (p.AddressTypeId == (int)FrayteAddressType.OtherAddress ||
                             p.AddressTypeId == (int)FrayteAddressType.MainAddress)).ToList();

            if (addresses != null)
            {
                foreach (UserAddress userAddress in addresses)
                {
                    FrayteAddress address = UtilityRepository.UserAddressMapping(userAddress);

                    var countryResult = dbContext.Countries.Where(p => p.CountryId == address.Country.CountryId).FirstOrDefault();
                    if (countryResult != null)
                    {
                        address.Country.Name = countryResult.CountryName;
                        address.Country.Code = countryResult.CountryCode;
                    }

                    shipperOtherAddresses.Add(address);

                }

            }

            return shipperOtherAddresses;
        }

        public FrayteResult SaveShipperOtherAddress(FrayteAddress shipperOtherAddress)
        {
            FrayteResult result = new FrayteResult();

            UserAddress otherAddress = new UserAddress();
            if (shipperOtherAddress.UserAddressId > 0)
            {
                otherAddress = dbContext.UserAddresses.Find(shipperOtherAddress.UserAddressId);
                if (otherAddress != null)
                {
                    otherAddress.UserId = shipperOtherAddress.UserId;
                    otherAddress.Address = shipperOtherAddress.Address;
                    otherAddress.Address2 = shipperOtherAddress.Address2;
                    otherAddress.Address3 = shipperOtherAddress.Address3;
                    otherAddress.City = shipperOtherAddress.City;
                    otherAddress.Suburb = shipperOtherAddress.Suburb;
                    otherAddress.State = shipperOtherAddress.State;
                    otherAddress.Zip = shipperOtherAddress.Zip;
                    otherAddress.CountryId = shipperOtherAddress.Country.CountryId;

                    dbContext.SaveChanges();
                    result.Status = true;
                }
            }
            else
            {
                otherAddress.AddressTypeId = (int)FrayteAddressType.OtherAddress;
                otherAddress.UserId = shipperOtherAddress.UserId;
                otherAddress.Address = shipperOtherAddress.Address;
                otherAddress.Address2 = shipperOtherAddress.Address2;
                otherAddress.Address3 = shipperOtherAddress.Address3;
                otherAddress.City = shipperOtherAddress.City;
                otherAddress.Suburb = shipperOtherAddress.Suburb;
                otherAddress.State = shipperOtherAddress.State;
                otherAddress.Zip = shipperOtherAddress.Zip;
                otherAddress.CountryId = shipperOtherAddress.Country.CountryId;

                dbContext.UserAddresses.Add(otherAddress);
                dbContext.SaveChanges();
                result.Status = true;
            }


            return result;
        }

        public List<FrayteUserModel> GetShipperReceivers(int shipperId)
        {
            var shipperReceivers = (from u in dbContext.Users
                                    join sr in dbContext.ReceiverShippers on u.UserId equals sr.ReceiverId
                                    where sr.ShipperId == shipperId
                                    select new FrayteUserModel()
                                    {
                                        Name = u.ContactName,
                                        UserId = u.UserId
                                    }).ToList();

            return shipperReceivers;
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
            if (!columns.Contains("MobileNo"))
            {
                valid = false;
            }
            if (!columns.Contains("FaxNumber"))
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
            if (!columns.Contains("Address"))
            {
                valid = false;
            }
            if (!columns.Contains("City"))
            {
                valid = false;
            }
            if (!columns.Contains("State"))
            {
                valid = false;
            }
            if (!columns.Contains("Zip"))
            {
                valid = false;
            }
            if (!columns.Contains("Country"))
            {
                valid = false;
            }

            return valid;
        }

        public List<FrayteShipperReceiver> GetShippers(DataTable exceldata)
        {
            List<FrayteShipperReceiver> shippers = new List<FrayteShipperReceiver>();

            FrayteShipperReceiver shipper;

            foreach (DataRow shipmentdetail in exceldata.Rows)
            {
                shipper = new FrayteShipperReceiver();

                shipper.UserId = 0;
                shipper.RoleId = (int)FrayteUserRole.Shipper;
                shipper.CompanyName = shipmentdetail["CompanyName"].ToString();
                shipper.ContactName = shipmentdetail["ContactName"].ToString();
                shipper.ShortName = shipmentdetail["ShortName"].ToString();
                shipper.Email = shipmentdetail["Email"].ToString();

                shipper.TelephoneNo = shipmentdetail["TelephoneNo"].ToString();
                shipper.MobileNo = shipmentdetail["MobileNo"].ToString();

                shipper.FaxNumber = shipmentdetail["FaxNumber"].ToString();
                shipper.WorkingStartTime = Convert.ToDateTime(shipmentdetail["WorkingStartTime"]);
                shipper.WorkingEndTime = Convert.ToDateTime(shipmentdetail["WorkingEndTime"]);
                shipper.WorkingWeekDay = new WorkingWeekDay();
                string workingDay = shipmentdetail["WorkingWeekDay"].ToString();
                var workingWeekDayResult = dbContext.WorkingWeekDays.Where(p => p.Description == workingDay).FirstOrDefault();
                if (workingWeekDayResult != null)
                {
                    shipper.WorkingWeekDay = workingWeekDayResult;
                }
                else
                {
                    shipper.WorkingWeekDay.Description = shipmentdetail["WorkingWeekDay"].ToString();
                }

                shipper.Timezone = new TimeZoneModal();
                string weekTimezone = shipmentdetail["Timezone"].ToString();
                var timeZoneResult = dbContext.Timezones.Where(p => p.Name == weekTimezone).FirstOrDefault();
                if (timeZoneResult != null)
                {
                    shipper.Timezone.TimezoneId = timeZoneResult.TimezoneId;
                    shipper.Timezone.Name = timeZoneResult.Name;
                    shipper.Timezone.Offset = timeZoneResult.Offset;
                    shipper.Timezone.OffsetShort = timeZoneResult.OffsetShort;
                }
                else
                {
                    shipper.Timezone.Name = shipmentdetail["Timezone"].ToString();
                }

                shipper.VATGST = shipmentdetail["VATGST"].ToString();
                shipper.CreatedOn = DateTime.UtcNow;

                FrayteAddress shipperAddress = new FrayteAddress();

                shipperAddress.Address = shipmentdetail["Address"].ToString();
                shipperAddress.Address2 = shipmentdetail["Address2"].ToString();
                shipperAddress.Address3 = shipmentdetail["Address3"].ToString();
                shipperAddress.Suburb = shipmentdetail["Suburb"].ToString();
                shipperAddress.City = shipmentdetail["City"].ToString();
                shipperAddress.State = shipmentdetail["State"].ToString();
                shipperAddress.Zip = shipmentdetail["Zip"].ToString();
                shipperAddress.Country = new FrayteCountryCode();
                string countryName = shipmentdetail["Country"].ToString();
                var country = dbContext.Countries.Where(p => p.CountryName == countryName).FirstOrDefault();
                if (country != null)
                {
                    shipperAddress.Country.CountryId = country.CountryId;
                    shipperAddress.Country.Code = country.CountryCode;
                    shipperAddress.Country.Name = country.CountryName;

                }
                else
                {
                    shipperAddress.Country.Code = shipmentdetail["Country"].ToString();
                }

                shipper.UserAddress = shipperAddress;

                shippers.Add(shipper);
            }

            return shippers;
        }
    }


}
