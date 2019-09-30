using Frayte.Services;
using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Models.ApiModal;
using Frayte.Services.Models.Express;
using Frayte.Services.Models.ParcelHub;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Frayte.Services.Business
{
    public class FrayteApiErrorCodeRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public FrayteRateResponse SaveInitializeApiError(string Error)
        {
            FrayteRateResponse apierror = new FrayteRateResponse();
            if (Error != null && Error != "")
            {
                //Step 1.0 Get last record from apierrorcode table
                ApiErrorCode errordetail = dbContext.ApiErrorCodes.ToList().Last();
                if (errordetail != null)
                {
                    int errorcode = errordetail.ErrorCode.HasValue ? errordetail.ErrorCode.Value : 0;

                    apierror.Status = false;
                    apierror.Description = null;
                    apierror.ShipmentId = null;
                    apierror.RateCard = null;

                    //Step 1.1 Checking same development error
                    List<ApiErrorCode> _error = dbContext.ApiErrorCodes.Where(p => p.DevelopmentErrorDetail == Error).ToList();
                    if (_error.Count > 0)
                    {
                        apierror.Errors = new List<FrayteApiError>();
                        {
                            foreach (ApiErrorCode ai in _error)
                            {
                                //Step 1.2 Make error response
                                FrayteApiError tt = new FrayteApiError()
                                {
                                    ErrorCode = ai.ErrorCode.HasValue ? ai.ErrorCode.Value : 0,
                                    Description = ai.Description
                                };
                                apierror.Errors.Add(tt);
                            }
                        }
                    }
                    else
                    {
                        //Step 1.3 Insert error into apierrorcode table
                        ApiErrorCode code = new ApiErrorCode();
                        code.ErrorCode = errorcode + 1;
                        code.Description = Error;
                        code.DevelopmentErrorDetail = Error;
                        code.Courier = null;
                        code.CourierErrorCode = null;
                        code.CourierErrorDescription = null;
                        code.ParentErrorCodeId = errordetail.ErrorCodeId + 1;
                        if (code != null)
                        {
                            dbContext.ApiErrorCodes.Add(code);
                            dbContext.SaveChanges();
                        }

                        //Step 1.4 Make error response
                        apierror.Errors = new List<FrayteApiError>();
                        {
                            FrayteApiError tt = new FrayteApiError()
                            {
                                ErrorCode = errorcode + 1,
                                Description = Error
                            };
                            apierror.Errors.Add(tt);
                        }
                    }
                }
            }
            return apierror;
        }

        public List<FrayteApiError> SaveFinilizeApiError(FratyteError error, string CourierCompany, string Error)
        {
            List<FrayteApiError> _code = new List<FrayteApiError>();

            if (error != null)
            {
                //Step 1.0 Get last record from apierrorcode table
                ApiErrorCode errordetail = dbContext.ApiErrorCodes.ToList().Last();
                if (errordetail != null)
                {
                    int errorcode = errordetail.ErrorCode.HasValue ? errordetail.ErrorCode.Value : 0;

                    if (error.MiscErrors != null && error.MiscErrors.Count > 0)
                    {
                        foreach (FrayteKeyValue misc in error.MiscErrors)
                        {
                            foreach (string item in misc.Value)
                            {
                                if (item != "" || item != null)
                                {
                                    List<ApiErrorCode> _error = dbContext.ApiErrorCodes.Where(p => p.CourierErrorDescription == item).ToList();
                                    if (_error.Count > 0)
                                    {
                                        foreach (var ai in _error)
                                        {
                                            FrayteApiError tt = new FrayteApiError()
                                            {
                                                ErrorCode = ai.ErrorCode.HasValue ? ai.ErrorCode.Value : 0,
                                                Description = ai.Description
                                            };
                                            _code.Add(tt);
                                        }
                                    }
                                    else
                                    {
                                        //Step 1.2 Insert error into apierrorcode table
                                        ApiErrorCode code = new ApiErrorCode();
                                        code.ErrorCode = errorcode + 1;
                                        code.Description = item;
                                        code.DevelopmentErrorDetail = null;
                                        code.Courier = CourierCompany;
                                        code.CourierErrorCode = null;
                                        code.CourierErrorDescription = item;
                                        code.ParentErrorCodeId = errordetail.ErrorCodeId + 1;
                                        if (code != null)
                                        {
                                            dbContext.ApiErrorCodes.Add(code);
                                            dbContext.SaveChanges();
                                        }

                                        //Step 1.3 Make error response
                                        FrayteApiError tt = new FrayteApiError()
                                        {
                                            ErrorCode = errorcode + 1,
                                            Description = item
                                        };
                                        _code.Add(tt);

                                        //Step 1.4 Preserve previous error code
                                        errorcode = errorcode + 1;
                                    }
                                }
                            }
                        }
                    }

                    if (error.Address != null && error.Address.Count > 0)
                    {
                        foreach (string address in error.Address)
                        {
                            string[] add = new string[2];
                            List<ApiErrorCode> _error;
                            if (CourierCompany == FrayteLogisticServiceType.DHL)
                            {
                                add = address.Split('/');
                                var type = add[1];
                                //Step 1.1 Checking same development error
                                _error = dbContext.ApiErrorCodes.Where(p => p.CourierErrorDescription == type).ToList();
                            }
                            else
                            {
                                //Step 1.1 Checking same development error
                                _error = dbContext.ApiErrorCodes.Where(p => p.CourierErrorDescription == address).ToList();
                            }

                            if (_error.Count > 0)
                            {
                                foreach (var ai in _error)
                                {
                                    FrayteApiError tt = new FrayteApiError()
                                    {
                                        ErrorCode = ai.ErrorCode.HasValue ? ai.ErrorCode.Value : 0,
                                        Description = ai.Description
                                    };
                                    _code.Add(tt);
                                }
                            }
                            else
                            {
                                //Step 1.2 Insert error into apierrorcode table
                                ApiErrorCode code = new ApiErrorCode();
                                code.ErrorCode = errorcode + 1;
                                code.DevelopmentErrorDetail = null;
                                code.Courier = CourierCompany;
                                if (CourierCompany == FrayteLogisticServiceType.DHL)
                                {
                                    code.Description = add[1];
                                    code.CourierErrorCode = add[0];
                                    code.CourierErrorDescription = add[1];
                                }
                                else
                                {
                                    code.Description = address;
                                    code.CourierErrorCode = null;
                                    code.CourierErrorDescription = address;
                                }
                                code.ParentErrorCodeId = errordetail.ErrorCodeId + 1;
                                if (code != null)
                                {
                                    dbContext.ApiErrorCodes.Add(code);
                                    dbContext.SaveChanges();
                                }

                                //Step 1.3 Make error response
                                FrayteApiError tt = new FrayteApiError();
                                tt.ErrorCode = errorcode + 1;
                                if (CourierCompany == FrayteLogisticServiceType.DHL)
                                {
                                    add = address.Split('/');
                                    tt.Description = add[1];
                                }
                                else
                                {
                                    tt.Description = address;
                                }
                                _code.Add(tt);

                                //Step 1.4 Preserve previous error code
                                errorcode = errorcode + 1;
                            }
                        }
                    }

                    if (error.Custom != null && error.Custom.Count > 0)
                    {
                        foreach (string custom in error.Custom)
                        {
                            //Step 1.1 Checking same development error
                            List<ApiErrorCode> _error = dbContext.ApiErrorCodes.Where(p => p.CourierErrorDescription == custom).ToList();
                            if (_error.Count > 0)
                            {
                                foreach (var ai in _error)
                                {
                                    FrayteApiError tt = new FrayteApiError()
                                    {
                                        ErrorCode = ai.ErrorCode.HasValue ? ai.ErrorCode.Value : 0,
                                        Description = ai.Description
                                    };
                                    _code.Add(tt);
                                }
                            }
                            else
                            {
                                //Step 1.2 Insert error into apierrorcode table
                                ApiErrorCode code = new ApiErrorCode();
                                code.ErrorCode = errorcode + 1;
                                code.Description = custom;
                                code.DevelopmentErrorDetail = null;
                                code.Courier = CourierCompany;
                                code.CourierErrorCode = null;
                                code.CourierErrorDescription = custom;
                                code.ParentErrorCodeId = errordetail.ErrorCodeId + 1;
                                if (code != null)
                                {
                                    dbContext.ApiErrorCodes.Add(code);
                                    dbContext.SaveChanges();
                                }

                                //Step 1.3 Make error response
                                FrayteApiError tt = new FrayteApiError()
                                {
                                    ErrorCode = errorcode + 1,
                                    Description = custom
                                };
                                _code.Add(tt);

                                //Step 1.4 Preserve previous error code
                                errorcode = errorcode + 1;
                            }
                        }
                    }

                    if (error.Package != null && error.Package.Count > 0)
                    {
                        foreach (string package in error.Package)
                        {
                            //Step 1.1 Checking same development error
                            List<ApiErrorCode> _error = dbContext.ApiErrorCodes.Where(p => p.CourierErrorDescription == package).ToList();
                            if (_error.Count > 0)
                            {
                                foreach (var ai in _error)
                                {
                                    FrayteApiError tt = new FrayteApiError()
                                    {
                                        ErrorCode = ai.ErrorCode.HasValue ? ai.ErrorCode.Value : 0,
                                        Description = ai.Description
                                    };
                                    _code.Add(tt);
                                }
                            }
                            else
                            {
                                //Step 1.2 Insert error into apierrorcode table
                                ApiErrorCode code = new ApiErrorCode();
                                code.ErrorCode = errorcode + 1;
                                code.Description = package;
                                code.DevelopmentErrorDetail = null;
                                code.Courier = CourierCompany;
                                code.CourierErrorCode = null;
                                code.CourierErrorDescription = package;
                                code.ParentErrorCodeId = errordetail.ErrorCodeId + 1;
                                if (code != null)
                                {
                                    dbContext.ApiErrorCodes.Add(code);
                                    dbContext.SaveChanges();
                                }

                                //Step 1.3 Make error response
                                FrayteApiError tt = new FrayteApiError()
                                {
                                    ErrorCode = errorcode + 1,
                                    Description = package
                                };
                                _code.Add(tt);

                                //Step 1.4 Preserve previous error code
                                errorcode = errorcode + 1;
                            }
                        }
                    }

                    if (error.Miscellaneous != null && error.Miscellaneous.Count > 0)
                    {
                        foreach (string miscellaneous in error.Miscellaneous)
                        {
                            //Step 1.1 Checking same development error
                            List<ApiErrorCode> _error = dbContext.ApiErrorCodes.Where(p => p.CourierErrorDescription == miscellaneous).ToList();
                            if (_error.Count > 0)
                            {
                                foreach (var ai in _error)
                                {
                                    FrayteApiError tt = new FrayteApiError()
                                    {
                                        ErrorCode = ai.ErrorCode.HasValue ? ai.ErrorCode.Value : 0,
                                        Description = ai.Description
                                    };
                                    _code.Add(tt);
                                }
                            }
                            else
                            {
                                //Step 1.2 Insert error into apierrorcode table
                                ApiErrorCode code = new ApiErrorCode();
                                code.ErrorCode = errorcode + 1;
                                code.Description = miscellaneous;
                                code.DevelopmentErrorDetail = null;
                                code.Courier = CourierCompany;
                                code.CourierErrorCode = null;
                                code.CourierErrorDescription = miscellaneous;
                                code.ParentErrorCodeId = errordetail.ErrorCodeId + 1;
                                if (code != null)
                                {
                                    dbContext.ApiErrorCodes.Add(code);
                                    dbContext.SaveChanges();
                                }

                                //Step 1.3 Make error response
                                FrayteApiError tt = new FrayteApiError()
                                {
                                    ErrorCode = errorcode + 1,
                                    Description = miscellaneous
                                };
                                _code.Add(tt);

                                //Step 1.4 Preserve previous error code
                                errorcode = errorcode + 1;
                            }
                        }
                    }

                    if (error.Service != null && error.Service.Count > 0)
                    {
                        foreach (string service in error.Service)
                        {
                            //Step 1.1 Checking same development error
                            List<ApiErrorCode> _error = dbContext.ApiErrorCodes.Where(p => p.CourierErrorDescription == service).ToList();
                            if (_error.Count > 0)
                            {
                                foreach (var ai in _error)
                                {
                                    FrayteApiError tt = new FrayteApiError()
                                    {
                                        ErrorCode = ai.ErrorCode.HasValue ? ai.ErrorCode.Value : 0,
                                        Description = ai.Description
                                    };
                                    _code.Add(tt);
                                }
                            }
                            else
                            {
                                //Step 1.2 Insert error into apierrorcode table
                                ApiErrorCode code = new ApiErrorCode();
                                code.ErrorCode = errorcode + 1;
                                code.Description = service;
                                code.DevelopmentErrorDetail = null;
                                code.Courier = CourierCompany;
                                code.CourierErrorCode = null;
                                code.CourierErrorDescription = service;
                                code.ParentErrorCodeId = errordetail.ErrorCodeId + 1;
                                if (code != null)
                                {
                                    dbContext.ApiErrorCodes.Add(code);
                                    dbContext.SaveChanges();
                                }

                                //Step 1.3 Make error response
                                FrayteApiError tt = new FrayteApiError()
                                {
                                    ErrorCode = errorcode + 1,
                                    Description = service
                                };
                                _code.Add(tt);

                                //Step 1.4 Preserve previous error code
                                errorcode = errorcode + 1;
                            }
                        }
                    }

                    if (error.ServiceError != null && error.ServiceError.Count > 0)
                    {
                        foreach (string serviceerror in error.ServiceError)
                        {
                            //Step 1.1 Checking same development error
                            List<ApiErrorCode> _error = dbContext.ApiErrorCodes.Where(p => p.CourierErrorDescription == serviceerror).ToList();
                            if (_error.Count > 0)
                            {
                                foreach (var ai in _error)
                                {
                                    FrayteApiError tt = new FrayteApiError()
                                    {
                                        ErrorCode = ai.ErrorCode.HasValue ? ai.ErrorCode.Value : 0,
                                        Description = ai.Description
                                    };
                                    _code.Add(tt);
                                }
                            }
                            else
                            {
                                //Step 1.2 Insert error into apierrorcode table
                                ApiErrorCode code = new ApiErrorCode();
                                code.ErrorCode = errorcode + 1;
                                code.Description = serviceerror;
                                code.DevelopmentErrorDetail = null;
                                code.Courier = CourierCompany;
                                code.CourierErrorCode = null;
                                code.CourierErrorDescription = serviceerror;
                                code.ParentErrorCodeId = errordetail.ErrorCodeId + 1;
                                if (code != null)
                                {
                                    dbContext.ApiErrorCodes.Add(code);
                                    dbContext.SaveChanges();
                                }

                                //Step 1.3 Make error response
                                FrayteApiError tt = new FrayteApiError()
                                {
                                    ErrorCode = errorcode + 1,
                                    Description = serviceerror
                                };
                                _code.Add(tt);

                                //Step 1.4 Preserve previous error code
                                errorcode = errorcode + 1;
                            }
                        }
                    }
                }
            }
            else if (Error != null && Error != "")
            {
                //Step 1.0 Get last record from apierrorcode table
                ApiErrorCode errordetail = dbContext.ApiErrorCodes.ToList().Last();
                if (errordetail != null)
                {
                    int errorcode = errordetail.ErrorCode.HasValue ? errordetail.ErrorCode.Value : 0;

                    //Step 1.1 Checking same development error
                    List<ApiErrorCode> _error = dbContext.ApiErrorCodes.Where(p => p.DevelopmentErrorDetail == Error).ToList();
                    if (_error.Count > 0)
                    {
                        foreach (ApiErrorCode ai in _error)
                        {
                            //Step 1.2 Make error response
                            FrayteApiError tt = new FrayteApiError()
                            {
                                ErrorCode = ai.ErrorCode.HasValue ? ai.ErrorCode.Value : 0,
                                Description = ai.Description
                            };
                            _code.Add(tt);
                        }
                    }
                    else
                    {
                        //Step 1.3 Insert error into apierrorcode table
                        ApiErrorCode code = new ApiErrorCode();
                        code.ErrorCode = errorcode + 1;
                        code.Description = Error;
                        code.DevelopmentErrorDetail = Error;
                        code.Courier = null;
                        code.CourierErrorCode = null;
                        code.CourierErrorDescription = null;
                        code.ParentErrorCodeId = errordetail.ErrorCodeId + 1;
                        if (code != null)
                        {
                            dbContext.ApiErrorCodes.Add(code);
                            dbContext.SaveChanges();
                        }

                        //Step 1.4 Make error response
                        FrayteApiError tt = new FrayteApiError()
                        {
                            ErrorCode = errorcode + 1,
                            Description = Error
                        };
                        _code.Add(tt);
                    }
                }
            }
            return _code;
        }

        public ExpressErrorResponse SaveExpressApiError(string Error)
        {
            ExpressErrorResponse apierror = new ExpressErrorResponse();
            if (Error != null && Error != "")
            {
                //Step 1.0 Get last record from apierrorcode table
                ApiErrorCode errordetail = dbContext.ApiErrorCodes.ToList().Last();
                if (errordetail != null)
                {
                    int errorcode = errordetail.ErrorCode.HasValue ? errordetail.ErrorCode.Value : 0;

                    apierror.Status = false;
                    apierror.Description = null;
                    apierror.ShipmentId = null;

                    //Step 1.1 Checking same development error
                    List<ApiErrorCode> _error = dbContext.ApiErrorCodes.Where(p => p.DevelopmentErrorDetail == Error).ToList();
                    if (_error.Count > 0)
                    {
                        apierror.Errors = new List<FrayteApiError>();
                        {
                            foreach (ApiErrorCode ai in _error)
                            {
                                //Step 1.2 Make error response
                                FrayteApiError tt = new FrayteApiError()
                                {
                                    ErrorCode = ai.ErrorCode.HasValue ? ai.ErrorCode.Value : 0,
                                    Description = ai.Description
                                };
                                apierror.Errors.Add(tt);
                            }
                        }
                    }
                    else
                    {
                        //Step 1.3 Insert error into apierrorcode table
                        ApiErrorCode code = new ApiErrorCode();
                        code.ErrorCode = errorcode + 1;
                        code.Description = Error;
                        code.DevelopmentErrorDetail = Error;
                        code.Courier = null;
                        code.CourierErrorCode = null;
                        code.CourierErrorDescription = null;
                        code.ParentErrorCodeId = errordetail.ErrorCodeId + 1;
                        if (code != null)
                        {
                            dbContext.ApiErrorCodes.Add(code);
                            dbContext.SaveChanges();
                        }

                        //Step 1.4 Make error response
                        apierror.Errors = new List<FrayteApiError>();
                        {
                            FrayteApiError tt = new FrayteApiError()
                            {
                                ErrorCode = errorcode + 1,
                                Description = Error
                            };
                            apierror.Errors.Add(tt);
                        }
                    }
                }
            }
            return apierror;
        }
    }
}