using Frayte.Api.Models;
using Frayte.Services;
using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Models.ApiModal;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Frayte.Api.Business
{
    public class ErrorRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<FrayteApiErrorDto> GetApiErrorCode()
        {
            List<FrayteApiErrorDto> _error = new List<FrayteApiErrorDto>();
            _error = (from er in dbContext.ApiErrorCodes
                      select new FrayteApiErrorDto
                      {
                          ErrorCode = er.ErrorCode.HasValue ? er.ErrorCode.Value : 0,
                          Description = er.Description
                      }).ToList();
            return _error;
        }
    }
}