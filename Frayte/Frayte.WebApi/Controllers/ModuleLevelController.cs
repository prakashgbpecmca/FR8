using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Utility;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class ModuleLevelController : ApiController
    {
        #region  Modules for AccessLevel
        [HttpGet]
        public IHttpActionResult GetModules(int userId)
        {
            try
            {
                var data = new ModuleLevelRepository().GetModules(userId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest("Something has gone wrong.");
            }
        }
        #endregion

        #region Roles for AccessLevel
        [HttpGet]
        public IHttpActionResult GetRoles(int userId)
        {
            try
            {
                var data = new ModuleLevelRepository().GetUserRoles(userId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest("Something has gone wrong.");
            }

        }
        #endregion

        #region  Role Modules 
        public IHttpActionResult GetRoleModules(int userId, int roleId , string moduleType)
        {
            try
            {
                var data = new ModuleLevelRepository().GetRoleModules(userId, roleId , moduleType);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest();
            } 
        }

        [HttpPost]
        public FrayteResult SaveRoleModule(AccessRoleModule acessRoleModule)
        {
            try
            {
                FrayteResult result = new FrayteResult();
                result = new ModuleLevelRepository().SavePermissionToRole(acessRoleModule);
                return result;
            }
            catch (Exception ex)
            {
                return new FrayteResult() { Status = false };

            }
        }
        #endregion
         
        #region DBEntries

        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult ModuleDBEntry()
        {
            FrayteResult result =
            new ModuleLevelRepository().ModuleDBEntry();
            return Ok(result);
        }

        #endregion

    }
}
