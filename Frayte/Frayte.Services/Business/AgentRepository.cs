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
    public class AgentRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public int? GetDestinatingId(int id)
        {
            int? destinatingAgentId = getDestinatigAgentId(id);
            return destinatingAgentId;
        }

        private int? getDestinatigAgentId(int id)
        {
            var B = dbContext.Shipments.Find(id);
            if (B != null)
            {
                return B.DestinatingAgentId;
            }
            else
            {
                return 0;
            }

        }

        public int? GetOriginatingId(int id)
        {
            int? agentId = getAgentId(id);
            return agentId;
        }

        private int? getAgentId(int id)
        {
            var a = dbContext.Shipments.Find(id);
            if (a != null)
            {
                return a.OriginatingAgentId;
            }
            else
            {
                return 0;
            }
        }

        public List<FrayteUserModel> GetAgents()
        {
            List<FrayteUser> users = new List<FrayteUser>();

            users = new FrayteUserRepository().GetUserTypeList((int)FrayteUserRole.Agent, (int)FrayteAddressType.MainAddress);

            List<FrayteUserModel> lstUsers = new List<FrayteUserModel>();

            foreach (FrayteUser user in users)
            {
                FrayteUserModel model = new FrayteUserModel();
                model.UserId = user.UserId;
                model.Name = user.ContactName;

                lstUsers.Add(model);
            }
            return lstUsers;
        }

        public List<FrayteAgentModel> GetAgents(int countryId)
        {
            List<FrayteUser> users = new List<FrayteUser>();

            users = new FrayteUserRepository().GetUserTypeList((int)FrayteUserRole.Agent, (int)FrayteAddressType.MainAddress);

            List<FrayteAgentModel> lstUsers = new List<FrayteAgentModel>();

            foreach (FrayteUser user in users)
            {
                var userShipmentType = dbContext.UserShipmentTypes.Where(p => p.UserId == user.UserId).FirstOrDefault();
                if (user.UserAddress.Country!= null  && user.UserAddress.Country.CountryId == countryId)
                {
                    FrayteAgentModel model = new FrayteAgentModel();
                    model.UserId = user.UserId;
                    model.Name = user.ContactName;
                    if (userShipmentType != null)
                    {
                        if (userShipmentType.IsAir != null)
                        {
                            model.IsAir = userShipmentType.IsAir.Value;
                        }
                        if (userShipmentType.IsSea != null)
                        {
                            model.IsSea = userShipmentType.IsSea.Value;
                        }
                        if (userShipmentType.IsExpryes != null)
                        {
                            model.IsExpryes = userShipmentType.IsExpryes.Value;
                        }

                    }
                    lstUsers.Add(model);
                }
            }
            return lstUsers;
        }

        public FrayteAgent GetAgentDetail(int? agentId)
        {
            FrayteAgent agentDetail = new FrayteAgent();
            WorkingWeekDay workingDays = new WorkingWeekDay();
            //Step 1: Get Agent's basic information
            var agent = dbContext.Users.Where(p => p.UserId == agentId).FirstOrDefault();

            if (agent != null)
            {
                agentDetail = UtilityRepository.AgentMapping(agent);
                agentDetail.RoleId = (int)FrayteUserRole.Agent;
                // get Working Week Time
                if (agentDetail.WorkingWeekDay.WorkingWeekDayId > 0)
                {
                    workingDays = dbContext.WorkingWeekDays.Find(agentDetail.WorkingWeekDay.WorkingWeekDayId);
                }


                if (workingDays != null)
                {

                    agentDetail.WorkingWeekDay = workingDays;

                }
                //Step 1.1: Get Agent's time zone
                var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == agent.TimezoneId).FirstOrDefault();
                if (timeZone != null)
                {
                    agentDetail.Timezone = new TimeZoneModal();
                    agentDetail.Timezone.TimezoneId = timeZone.TimezoneId;
                    agentDetail.Timezone.Name = timeZone.Name;
                    agentDetail.Timezone.Offset = timeZone.Offset;
                    agentDetail.Timezone.OffsetShort = timeZone.OffsetShort;
                }

                //Step 2: Get Agent's other information
                var agentOtherDetails = dbContext.UserAdditionals.Where(p => p.UserId == agentId).FirstOrDefault();
                if (agentOtherDetails != null)
                {
                    //Get associated Frayte User's detail
                    GetAssociateUsersDetail(agent.UserId, agentDetail);
                }

                // Step 3: Get AgentShipmentType
                if (agentDetail != null && agentDetail.UserId > 0)
                {
                    var agentShipmentType = dbContext.UserShipmentTypes.Where(p => p.UserId == agent.UserId).FirstOrDefault();
                    if (agentShipmentType != null)
                    {
                        agentDetail.UserShipmentTypeId = agentShipmentType.UserShipmentTypeId;
                        agentDetail.IsAir = agentShipmentType.IsAir;
                        agentDetail.IsSea = agentShipmentType.IsSea;
                        agentDetail.IsExpryes = agentShipmentType.IsExpryes;
                    }

                }

                //Step 4: Get Agents's Address information
                var agentAddress = dbContext.UserAddresses.Where(p => p.UserId == agentId &&
                                            (p.AddressTypeId == (int)FrayteAddressType.MainAddress ||
                                            p.AddressTypeId == (int)FrayteAddressType.OtherAddress)).ToList();
                if (agentAddress != null)
                {
                    agentDetail.OtherAddresses = new List<FrayteAddress>();

                    foreach (UserAddress address in agentAddress)
                    {
                        if (address.AddressTypeId == (int)FrayteAddressType.MainAddress)
                        {
                            //Step 3.1: Set Agent main address
                            agentDetail.UserAddress = new FrayteAddress();
                            agentDetail.UserAddress = UtilityRepository.UserAddressMapping(address);

                            //Step : Get country information
                            var country = dbContext.Countries.Where(p => p.CountryId == address.CountryId).FirstOrDefault();
                            if (country != null)
                            {
                                agentDetail.UserAddress.Country = new FrayteCountryCode();
                                agentDetail.UserAddress.Country.CountryId = country.CountryId;
                                agentDetail.UserAddress.Country.Code = country.CountryCode;
                                agentDetail.UserAddress.Country.Name = country.CountryName;
                            }

                        }
                        else
                        {
                            //Step 4.2: Set Agent's other addresses
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

                            agentDetail.OtherAddresses.Add(otherAddress);
                        }
                    }
                }
            }
            return agentDetail;
        }

        public List<FrayteUser> GetAgentList()
        {
            List<FrayteUser> users = new List<FrayteUser>();

            users = new FrayteUserRepository().GetUserTypeList((int)FrayteUserRole.Agent, (int)FrayteAddressType.MainAddress);

            return users;
        }

        public FrayteResult SaveAgent(FrayteAgent frayteUser)
        {
            FrayteResult result = new FrayteResult();

            FrayteUserRepository userRepository = new FrayteUserRepository();

            //Step 1: Save Agent Detail
            userRepository.SaveUserDetail(frayteUser);

            //Step 2: Save Agent's additional detail
            SaveAgentAdditional(frayteUser);

            // Step 3: Save UserShipmentType
            SaveAgentShipmentType(frayteUser);

            //Step 4: Save Agent role
            userRepository.SaveUserRole(frayteUser.UserId, (int)FrayteUserRole.Agent);

            //Step 5: Save Agent Address information
            frayteUser.UserAddress.AddressTypeId = (int)FrayteAddressType.MainAddress;
            frayteUser.UserAddress.UserId = frayteUser.UserId;
            userRepository.SaveUserAddress(frayteUser.UserAddress);

            //Step 6: Save Agent other address information
            if (frayteUser.OtherAddresses != null && frayteUser.OtherAddresses.Count > 0)
            {
                foreach (FrayteAddress address in frayteUser.OtherAddresses)
                {
                    address.AddressTypeId = (int)FrayteAddressType.OtherAddress;
                    address.UserId = frayteUser.UserId;
                    userRepository.SaveUserAddress(address);
                }
            }

            //Step 7: Save Agent's Associated Users information
            if (frayteUser.AssociatedUsers != null && frayteUser.AssociatedUsers.Count > 0)
            {
                foreach (FrayteAgentAssociatedUser agentUser in frayteUser.AssociatedUsers)
                {
                    AgentAssociatedUser saveUser = new AgentAssociatedUser();

                    if (agentUser.AgentAssociatedUserId > 0)
                    {
                        // Update AgentAssociated user Info
                        var associatedUser = dbContext.AgentAssociatedUsers.Where(p => p.AgentAssociatedUserId == agentUser.AgentAssociatedUserId).FirstOrDefault();
                        if (associatedUser != null)
                        {
                            associatedUser.Name = agentUser.Name;
                            associatedUser.UserType = agentUser.UserType;
                            associatedUser.Email = agentUser.Email;
                            associatedUser.TelephoneNo = agentUser.TelephoneNo;

                            associatedUser.WorkingStartTime = UtilityRepository.GetTimeZoneUTCTime(agentUser.WorkingStartTime, frayteUser.Timezone.TimezoneId).Value;
                            associatedUser.WorkingEndTime = UtilityRepository.GetTimeZoneUTCTime(agentUser.WorkingEndTime, frayteUser.Timezone.TimezoneId).Value;
                            associatedUser.WorkingWeekDays = agentUser.WorkingWeekDays;
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            saveUser.AgentId = frayteUser.UserId;
                            saveUser.Name = agentUser.Name;
                            saveUser.UserType = agentUser.UserType;
                            saveUser.Email = agentUser.Email;
                            saveUser.TelephoneNo = agentUser.TelephoneNo;
                            saveUser.WorkingStartTime = UtilityRepository.GetTimeZoneUTCTime(agentUser.WorkingStartTime, frayteUser.Timezone.TimezoneId).Value;
                            saveUser.WorkingEndTime = UtilityRepository.GetTimeZoneUTCTime(agentUser.WorkingEndTime, frayteUser.Timezone.TimezoneId).Value;
                            saveUser.WorkingWeekDays = agentUser.WorkingWeekDays;
                            dbContext.AgentAssociatedUsers.Add(saveUser);
                            dbContext.SaveChanges();
                        }

                    }
                    else
                    {
                        // Insert Agent Associated User Info
                        saveUser.AgentId = frayteUser.UserId;
                        saveUser.Name = agentUser.Name;
                        saveUser.UserType = agentUser.UserType;
                        saveUser.Email = agentUser.Email;
                        saveUser.TelephoneNo = agentUser.TelephoneNo;
                        saveUser.WorkingStartTime = UtilityRepository.GetTimeZoneUTCTime(agentUser.WorkingStartTime, frayteUser.Timezone.TimezoneId).Value;
                        saveUser.WorkingEndTime = UtilityRepository.GetTimeZoneUTCTime(agentUser.WorkingEndTime, frayteUser.Timezone.TimezoneId).Value;
                        saveUser.WorkingWeekDays = agentUser.WorkingWeekDays;
                        dbContext.AgentAssociatedUsers.Add(saveUser);
                        dbContext.SaveChanges();
                    }
                }
            }

            result.Status = true;

            return result;
        }

        public FrayteResult DeleteAgent(int userId)
        {
            return new FrayteUserRepository().MarkForDelete(userId);
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

        public List<FrayteAgent> GetAllAgents(DataTable exceldata)
        {
            List<FrayteAgent> agents = new List<FrayteAgent>();

            FrayteAgent agent;

            foreach (DataRow shipmentdetail in exceldata.Rows)
            {
                agent = new FrayteAgent();

                agent.UserId = 0;
                agent.RoleId = (int)FrayteUserRole.Agent;
                agent.CompanyName = shipmentdetail["CompanyName"].ToString();
                agent.ContactName = shipmentdetail["ContactName"].ToString();
                agent.ShortName = shipmentdetail["ShortName"].ToString();
                agent.Email = shipmentdetail["Email"].ToString();

                agent.TelephoneNo = shipmentdetail["TelephoneNo"].ToString();
                agent.MobileNo = shipmentdetail["MobileNo"].ToString();

                agent.FaxNumber = shipmentdetail["FaxNumber"].ToString();
                agent.WorkingStartTime = Convert.ToDateTime(shipmentdetail["WorkingStartTime"].ToString());
                agent.WorkingEndTime = Convert.ToDateTime(shipmentdetail["WorkingEndTime"].ToString());
                agent.WorkingWeekDay = new WorkingWeekDay();
                string workingDay = shipmentdetail["WorkingWeekDay"].ToString();
                var workingWeekDayResult = dbContext.WorkingWeekDays.Where(p => p.Description == workingDay).FirstOrDefault();
                if (workingWeekDayResult != null)
                {
                    agent.WorkingWeekDay = workingWeekDayResult;
                }
                else
                {
                    agent.WorkingWeekDay.Description = shipmentdetail["WorkingWeekDay"].ToString();
                }

                agent.Timezone = new TimeZoneModal();
                string weekTimezone = shipmentdetail["Timezone"].ToString();
                var timeZoneResult = dbContext.Timezones.Where(p => p.Name == weekTimezone).FirstOrDefault();
                if (timeZoneResult != null)
                {
                    agent.Timezone.TimezoneId = timeZoneResult.TimezoneId;
                    agent.Timezone.Name = timeZoneResult.Name;
                    agent.Timezone.Offset = timeZoneResult.Offset;
                    agent.Timezone.OffsetShort = timeZoneResult.OffsetShort;
                }
                else
                {
                    agent.Timezone.Name = shipmentdetail["Timezone"].ToString();
                }

                agent.VATGST = shipmentdetail["VATGST"].ToString();
                agent.CreatedOn = DateTime.UtcNow;

                FrayteAddress agentAddress = new FrayteAddress();

                agentAddress.Address = shipmentdetail["Address"].ToString();
                agentAddress.Address2 = shipmentdetail["Address2"].ToString();
                agentAddress.Address3 = shipmentdetail["Address3"].ToString();
                agentAddress.Suburb = shipmentdetail["Suburb"].ToString();
                agentAddress.City = shipmentdetail["City"].ToString();
                agentAddress.State = shipmentdetail["State"].ToString();
                agentAddress.Zip = shipmentdetail["Zip"].ToString();
                agentAddress.Country = new FrayteCountryCode();
                string countryName = shipmentdetail["Country"].ToString();
                var country = dbContext.Countries.Where(p => p.CountryName == countryName).FirstOrDefault();
                if (country != null)
                {
                    agentAddress.Country.CountryId = country.CountryId;
                    agentAddress.Country.Code = country.CountryCode;
                    agentAddress.Country.Name = country.CountryName;

                }
                else
                {
                    agentAddress.Country.Code = shipmentdetail["Country"].ToString();
                }

                agent.UserAddress = agentAddress;

                agents.Add(agent);
            }

            return agents;
        }


        #region -- Private Methods --

        private void GetAssociateUsersDetail(int agentId, FrayteAgent agentDetail)
        {
            List<FrayteAgentAssociatedUser> userAssociated = new List<FrayteAgentAssociatedUser>();
            var users = dbContext.AgentAssociatedUsers.Where(p => p.AgentId == agentId);
            foreach (var user in users)
            {
                FrayteAgentAssociatedUser newUser = new FrayteAgentAssociatedUser();
                newUser.AgentAssociatedUserId = user.AgentAssociatedUserId;
                newUser.AgentId = user.AgentId;
                newUser.Name = user.Name;
                newUser.UserType = user.UserType;
                newUser.Email = user.Email;
                newUser.TelephoneNo = user.TelephoneNo;
                newUser.WorkingStartTime = UtilityRepository.GetTimeZoneTime(user.WorkingStartTime, agentDetail.Timezone.TimezoneId);
                newUser.WorkingEndTime = UtilityRepository.GetTimeZoneTime(user.WorkingEndTime, agentDetail.Timezone.TimezoneId);
                newUser.WorkingWeekDays = user.WorkingWeekDays;

                userAssociated.Add(newUser);
            }
            agentDetail.AssociatedUsers = userAssociated;
        }

        private void SaveAgentAdditional(FrayteAgent frayteAgentUser)
        {
            UserAdditional customerDetail = dbContext.UserAdditionals.Where(p => p.UserId == frayteAgentUser.UserId).FirstOrDefault();

            if (customerDetail != null)
            {
                customerDetail.UserId = frayteAgentUser.UserId;
                customerDetail.AccountNo = "000000000";

                if (frayteAgentUser.AccountUser != null)
                {
                    customerDetail.AccountUserId = frayteAgentUser.AccountUser.UserId;
                }

                if (frayteAgentUser.DocumentUser != null)
                {
                    customerDetail.DocumentUserId = frayteAgentUser.DocumentUser.UserId;
                }

                if (frayteAgentUser.ManagerUser != null)
                {
                    customerDetail.ManagerUserId = frayteAgentUser.ManagerUser.UserId;
                }

                if (frayteAgentUser.OperationUser != null)
                {
                    customerDetail.OperationUserId = frayteAgentUser.OperationUser.UserId;
                }
            }
            else
            {
                if (frayteAgentUser.AccountUser != null ||
                    frayteAgentUser.DocumentUser != null ||
                    frayteAgentUser.ManagerUser != null ||
                    frayteAgentUser.OperationUser != null)
                {
                    customerDetail = new UserAdditional();
                    customerDetail.UserId = frayteAgentUser.UserId;
                    customerDetail.AccountNo = "000000000";

                    if (frayteAgentUser.AccountUser != null)
                    {
                        customerDetail.AccountUserId = frayteAgentUser.AccountUser.UserId;
                    }

                    if (frayteAgentUser.DocumentUser != null)
                    {
                        customerDetail.DocumentUserId = frayteAgentUser.DocumentUser.UserId;
                    }

                    if (frayteAgentUser.ManagerUser != null)
                    {
                        customerDetail.ManagerUserId = frayteAgentUser.ManagerUser.UserId;
                    }

                    if (frayteAgentUser.OperationUser != null)
                    {
                        customerDetail.OperationUserId = frayteAgentUser.OperationUser.UserId;
                    }

                    dbContext.UserAdditionals.Add(customerDetail);
                }
            }

            if (customerDetail != null)
            {
                dbContext.SaveChanges();
            }
        }

        private void SaveAgentShipmentType(FrayteAgent frayteAgentUserShipmentType)
        {
            UserShipmentType newUserShipmentType;
            if (frayteAgentUserShipmentType != null)
            {
                if (frayteAgentUserShipmentType.UserShipmentTypeId == 0)
                {
                    newUserShipmentType = new UserShipmentType();
                    newUserShipmentType.UserId = frayteAgentUserShipmentType.UserId;
                    newUserShipmentType.IsAir = frayteAgentUserShipmentType.IsAir;
                    newUserShipmentType.IsSea = frayteAgentUserShipmentType.IsSea;
                    newUserShipmentType.IsExpryes = frayteAgentUserShipmentType.IsExpryes;
                    dbContext.UserShipmentTypes.Add(newUserShipmentType);
                }
                else
                {
                    newUserShipmentType = dbContext.UserShipmentTypes.Find(frayteAgentUserShipmentType.UserShipmentTypeId);
                    newUserShipmentType.UserId = frayteAgentUserShipmentType.UserId;
                    newUserShipmentType.IsAir = frayteAgentUserShipmentType.IsAir;
                    newUserShipmentType.IsSea = frayteAgentUserShipmentType.IsSea;
                    newUserShipmentType.IsExpryes = frayteAgentUserShipmentType.IsExpryes;
                }
                if (newUserShipmentType != null)
                {
                    dbContext.SaveChanges();
                }
            }

        }
        #endregion -- Private Methods --

    }
}
