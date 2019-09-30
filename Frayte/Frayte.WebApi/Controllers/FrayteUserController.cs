using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using Frayte.WebApi.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class FrayteUserController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GetInitials()
        {
            List<WorkingWeekDay> lstWeekDays = new List<WorkingWeekDay>();

            lstWeekDays = new WeekDaysRepository().GetWeekDaysDataList();

            List<FrayteCountryCode> lstCountry = new List<FrayteCountryCode>();

            lstCountry = new CountryRepository().lstCountry();

            List<FrayteCountryPhoneCode> lstCountryPhones = new List<FrayteCountryPhoneCode>();

            lstCountryPhones = new CountryRepository().GetCountryPhoneCodeList();

            List<TimeZoneModal> lstTimeZone = new List<TimeZoneModal>();

            lstTimeZone = new TimeZoneRepository().GetShipmentTimeZones();

            TimeZoneModal operationTimeZone = new TimeZoneRepository().GetOperationTimezone();

            return this.Request.CreateResponse(
                HttpStatusCode.OK,
                new
                {
                    WorkingWeekDays = lstWeekDays,
                    Countries = lstCountry,
                    TimeZones = lstTimeZone,
                    CountryPhoneCodes = lstCountryPhones,
                    OperationTimeZone = operationTimeZone
                });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IHttpActionResult GetUserList(TrackFryateUser trackUser)
        {
            List<FrayteInternalUser> lstUsers = new List<FrayteInternalUser>();
            lstUsers = new FrayteUserRepository().GetInternalUserList(trackUser);
            return Ok<List<FrayteInternalUser>>(lstUsers);
        }

        [HttpGet]
        public List<FrayteSystemRole> GetSystemRoles(int UserId)
        {
            List<FrayteSystemRole> list = new List<FrayteSystemRole>();
            list = new FrayteUserRepository().GetSystemRoles(UserId);
            return list;
        }

        [HttpGet]
        public FrayteInternalUser GetUserDetail(int userId)
        {
            FrayteInternalUser user = new FrayteUserRepository().GetInternalUserDetail(userId);
            return user;
        }

        [AllowAnonymous]
        [HttpGet]
        public MobileInternalUserConfiguration GetMobileConfiguration(int userId)
        {
            MobileInternalUserConfiguration detail = new FrayteUserRepository().GetMobileConfiguration(userId);
            return detail;
        }

        [HttpPost]
        public FrayteResult SaveMobileconfiguration(MobileInternalUserConfiguration mobileUserConfiguration)
        {
            FrayteResult result = new FrayteUserRepository().SaveMobileconfiguration(mobileUserConfiguration);
            return result;
        }

        [HttpGet]
        public IHttpActionResult DeleteUser(int userId)
        {
            FrayteResult result = new FrayteUserRepository().MarkForDelete(userId);
            return Ok(result);
        }


        [HttpGet]
        public List<FrayteCustomerAssociatedUser> GetAssociatedUsers(string searchName)
        {
            //To Do: Get associated users list from db
            List<FrayteCustomerAssociatedUser> lstAssociatedUser = new List<FrayteCustomerAssociatedUser>();
            lstAssociatedUser = new FrayteUserRepository().GetAssociatedUsers(searchName);
            return lstAssociatedUser;
        }

        [HttpGet]
        public IHttpActionResult CheckUserEmail(string email)
        {
            FrayteResult result = new FrayteUserRepository().CheckUserEmail(email);

            if (result != null && result.Status)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
