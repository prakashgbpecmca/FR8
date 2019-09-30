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
    public class ReceiverRepository
    {
        FrayteEntities dbContext = new FrayteEntities();
        public void SaveEasyPostReceiverAddressId(int userAddressId, string easyPostAddressId)
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
        public FrayteShipperReceiver GetReceiverDetail(int receiverId)
        {
            FrayteShipperReceiver shipperReceiver = new FrayteShipperReceiver();
            WorkingWeekDay workingDays = new WorkingWeekDay();
            //Step 1: Get Shipper basic information
            var receiver = dbContext.Users.Where(p => p.UserId == receiverId).FirstOrDefault();

            if (receiver != null)
            {
                //Set User's bacic information
                shipperReceiver = UtilityRepository.ShipperReceiverMapping(receiver);
                if (shipperReceiver.WorkingWeekDay.WorkingWeekDayId > 0)
                {
                    workingDays = dbContext.WorkingWeekDays.Find(shipperReceiver.WorkingWeekDay.WorkingWeekDayId);
                }


                if (workingDays != null)
                {

                    shipperReceiver.WorkingWeekDay = workingDays;

                }
                //Step 1.1: Get time zone
                var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == receiver.TimezoneId).FirstOrDefault();
                if (timeZone != null)
                {
                    shipperReceiver.Timezone = new TimeZoneModal();
                    shipperReceiver.Timezone.TimezoneId = timeZone.TimezoneId;
                    shipperReceiver.Timezone.Name = timeZone.Name;
                    shipperReceiver.Timezone.Offset = timeZone.Offset;
                }

                //Step 2: Get Shipper's Address information
                var shipperAddress = dbContext.UserAddresses.Where(p => p.UserId == receiverId).ToList();
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
                            }

                            shipperReceiver.PickupAddresses.Add(otherAddress);

                        }
                    }
                }
            }

            return shipperReceiver;

        }

        public List<FrayteUser> GetReceiverList()
        {
            List<FrayteUser> users = new List<FrayteUser>();

            users = new FrayteUserRepository().GetUserTypeList((int)FrayteUserRole.Receiver, (int)FrayteAddressType.MainAddress);

            return users;
        }

        public FrayteResult SaveReceiver(FrayteShipperReceiver frayteUser)
        {
            FrayteResult result = new FrayteResult();
            FrayteUserRepository userRepository = new FrayteUserRepository();

            //Step 1: Save User Detail
            userRepository.SaveUserDetail(frayteUser);

            //Step 2: Save user role
            userRepository.SaveUserRole(frayteUser.UserId, (int)FrayteUserRole.Receiver);

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

        public FrayteResult DeleteReceiver(int receiverId)
        {
            return new FrayteUserRepository().MarkForDelete(receiverId);
        }

        public List<FrayteUserModel> GetAssignedShippers(int receiverId)
        {
            List<FrayteUserModel> assignedShipper = new List<FrayteUserModel>();

            assignedShipper = (from s in dbContext.ReceiverShippers
                           join u in dbContext.Users on s.ShipperId equals u.UserId
                           where s.ReceiverId == receiverId
                           select new FrayteUserModel()
                           {
                               Name = u.ContactName,
                               UserId = u.UserId
                           }).ToList();

            return assignedShipper;
        }

        public FrayteResult SaveReceiverShippers(ReceiverShipper receiverShipper)
        {
            FrayteResult saveResult = new FrayteResult();

            var result = dbContext.ReceiverShippers.Where(p => p.ReceiverId == receiverShipper.ReceiverId &&
                                                               p.ShipperId == receiverShipper.ShipperId).FirstOrDefault();
            
            if (result == null)
            {
                ReceiverShipper newReceiverShipper = new ReceiverShipper();
                newReceiverShipper.ReceiverId = receiverShipper.ReceiverId;
                newReceiverShipper.ShipperId = receiverShipper.ShipperId;
                dbContext.ReceiverShippers.Add(newReceiverShipper);
                dbContext.SaveChanges();

                saveResult.Status = true;
            }

            return saveResult;
        }

        public FrayteResult RemoveReceiverShippers(ReceiverShipper receiverShipper)
        {
            FrayteResult saveResult = new FrayteResult();

            var result = dbContext.ReceiverShippers.Where(p => p.ReceiverId == receiverShipper.ReceiverId &&
                                                               p.ShipperId == receiverShipper.ShipperId).FirstOrDefault();

            if (result != null)
            {
                dbContext.ReceiverShippers.Remove(result);
                dbContext.SaveChanges();

                saveResult.Status = true;
            }

            return saveResult;
        }

        public List<FrayteAddress> GetReceiverOtherAddresses(int shipperId)
        {
            List<FrayteAddress> receiverOtherAddresses = new List<FrayteAddress>();

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

                    receiverOtherAddresses.Add(address);

                }

            }

            return receiverOtherAddresses;
        }

        public FrayteResult SaveReceiverOtherAddress(FrayteAddress receiverOtherAddress)
        {
            FrayteResult result = new FrayteResult();
              UserAddress otherAddress = new UserAddress();
            if(receiverOtherAddress.UserAddressId > 0)
            {
                otherAddress = dbContext.UserAddresses.Find(receiverOtherAddress.UserAddressId);
                if(otherAddress != null){
                    otherAddress.UserId = receiverOtherAddress.UserId;
                    otherAddress.Address = receiverOtherAddress.Address;
                    otherAddress.Address2 = receiverOtherAddress.Address2;
                    otherAddress.Address3 = receiverOtherAddress.Address3;
                    otherAddress.City = receiverOtherAddress.City;
                    otherAddress.Suburb = receiverOtherAddress.Suburb;
                    otherAddress.State = receiverOtherAddress.State;
                    otherAddress.Zip = receiverOtherAddress.Zip;
                    otherAddress.CountryId = receiverOtherAddress.Country.CountryId;
                    dbContext.SaveChanges();
                    result.Status = true;
                }
            }
            else
            {
                otherAddress.AddressTypeId = (int)FrayteAddressType.OtherAddress;
                otherAddress.UserId = receiverOtherAddress.UserId;
                otherAddress.Address = receiverOtherAddress.Address;
                otherAddress.Address2 = receiverOtherAddress.Address2;
                otherAddress.Address3 = receiverOtherAddress.Address3;
                otherAddress.City = receiverOtherAddress.City;
                otherAddress.Suburb = receiverOtherAddress.Suburb;
                otherAddress.State = receiverOtherAddress.State;
                otherAddress.Zip = receiverOtherAddress.Zip;
                otherAddress.CountryId = receiverOtherAddress.Country.CountryId;
                dbContext.UserAddresses.Add(otherAddress);
                dbContext.SaveChanges();
                result.Status = true;
            }
      
            return result;
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

        public List<FrayteShipperReceiver> GetReceivers(DataTable exceldata)
        {
            List<FrayteShipperReceiver> receivers = new List<FrayteShipperReceiver>();

            FrayteShipperReceiver receiver;

            foreach (DataRow shipmentdetail in exceldata.Rows)
            {
                receiver = new FrayteShipperReceiver();

                receiver.UserId = 0;
                receiver.RoleId = (int)FrayteUserRole.Receiver;
                receiver.CompanyName = shipmentdetail["CompanyName"].ToString();
                receiver.ContactName = shipmentdetail["ContactName"].ToString();
                receiver.ShortName = shipmentdetail["ShortName"].ToString();
                receiver.Email = shipmentdetail["Email"].ToString();

                receiver.TelephoneNo = shipmentdetail["TelephoneNo"].ToString();
                receiver.MobileNo = shipmentdetail["MobileNo"].ToString();

                receiver.FaxNumber = shipmentdetail["FaxNumber"].ToString();
                receiver.WorkingStartTime = Convert.ToDateTime(shipmentdetail["WorkingStartTime"]);
                receiver.WorkingEndTime = Convert.ToDateTime(shipmentdetail["WorkingEndTime"]);
                receiver.WorkingWeekDay = new WorkingWeekDay();
                string workingDay = shipmentdetail["WorkingWeekDay"].ToString();
                var workingWeekDayResult = dbContext.WorkingWeekDays.Where(p => p.Description == workingDay).FirstOrDefault();
                if(workingWeekDayResult!=null)
                {
                    receiver.WorkingWeekDay = workingWeekDayResult;
                }
                else
                {
                    receiver.WorkingWeekDay.Description = shipmentdetail["WorkingWeekDay"].ToString();
                }

                receiver.Timezone = new TimeZoneModal();
                string weekTimezone = shipmentdetail["Timezone"].ToString();
                var timeZoneResult = dbContext.Timezones.Where(p => p.Name == weekTimezone).FirstOrDefault();
                if(timeZoneResult!=null)
                {
                    receiver.Timezone.TimezoneId = timeZoneResult.TimezoneId;
                    receiver.Timezone.Name = timeZoneResult.Name;
                    receiver.Timezone.Offset = timeZoneResult.Offset;
                    receiver.Timezone.OffsetShort = timeZoneResult.OffsetShort;
                }
                else
                {
                    receiver.Timezone.Name = shipmentdetail["Timezone"].ToString();
                }
               
                receiver.VATGST = shipmentdetail["VATGST"].ToString();
                receiver.CreatedOn = DateTime.UtcNow;

                FrayteAddress receiverAddress = new FrayteAddress();

                receiverAddress.Address = shipmentdetail["Address"].ToString();
                receiverAddress.Address2 = shipmentdetail["Address2"].ToString();
                receiverAddress.Address3 = shipmentdetail["Address3"].ToString();
                receiverAddress.Suburb = shipmentdetail["Suburb"].ToString();
                receiverAddress.City = shipmentdetail["City"].ToString();
                receiverAddress.State = shipmentdetail["State"].ToString();
                receiverAddress.Zip = shipmentdetail["Zip"].ToString();
                receiverAddress.Country = new FrayteCountryCode();
                string countryName = shipmentdetail["Country"].ToString();
                var country = dbContext.Countries.Where(p => p.CountryName == countryName).FirstOrDefault();
                if(country!= null)
                {
                    receiverAddress.Country.CountryId = country.CountryId;
                    receiverAddress.Country.Code = country.CountryCode;
                    receiverAddress.Country.Name = country.CountryName;
                    
                }
                else
                {
                    receiverAddress.Country.Code = shipmentdetail["Country"].ToString();
                }
                
                receiver.UserAddress = receiverAddress;

                receivers.Add(receiver);
            }

            return receivers;
        }
    }
}
