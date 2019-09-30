angular.module('ngApp.quotationTools').controller("QuotationToolMailController", function ($scope, config, UtilityService, quotationDetail, $rootScope, AppSpinner, toaster, QuotationService, $uibModalInstance, CustomerName, CustomerEmail, item, CustomerDetail, CustId, CustName, CustEmail, Header, MailContentText, SalesRepresentativeDetail, CompanyName, SessionService, $translate) {

    var setMultilingualOptions = function () {
        $translate(['Success', 'Record_Saved', 'FrayteError_Validation', 'ErrorSavingRecord', 'GettingDetails_Error', 'CancelShipmentError_Validation',
                'GeneratePdfError_Validation', 'GeneratePdfError_Validation', 'SuccessfullySendlLabel_Validation', 'SendingMailError_Validation',
                'EnterValidEmailAdd', 'TrackShipmentNotTrack_Error', 'FrayteWarning', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation',
                'ErrorSavingRecord', 'FrayteError', 'FrayteSuccess', 'SuccessfullySentMail', 'SendingMailError_Validation', 'Select_ServiceFirst',
                'SendingQuotationEmail', 'SendingRateCardEmail']).then(function (translations) {
                    $scope.Frayte_Warning = translations.FrayteWarning;
                    $scope.ErrorSaving_Record = translations.ErrorSavingRecord;
                    $scope.RecordSaved = translations.Record_Saved;
                    $scope.Frayte_Error = translations.FrayteError;
                    $scope.Frayte_Success = translations.FrayteSuccess;
                    $scope.Successfully_SentMail = translations.SuccessfullySentMail;
                    $scope.SendingMailErrorValidation = translations.SendingMailError_Validation;
                    $scope.PleaseCorrect_ValidationErrors = translations.PleaseCorrectValidationErrors;
                    $scope.Select_ServiceFirst = translations.Select_ServiceFirst;
                    $scope.SendingQuotationEmail = translations.SendingQuotationEmail;
                    $scope.SendingRateCardEmail = translations.SendingRateCardEmail;
                });
    };

    $scope.getChargeableWeight = function (items, prop) {
        if ($scope.quotationDetail === undefined) {
            return;
        }

        else if ($scope.quotationDetail.QuotationPackages === undefined || $scope.quotationDetail.QuotationPackages === null) {
            return 0;
        }

        if ($scope.quotationDetail.QuotationPackages.length >= 0) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.quotationDetail.QuotationPackages.length; i++) {
                var product = $scope.quotationDetail.QuotationPackages[i];
                var len = 0;
                var wid = 0;
                var height = 0;
                var qty = 0;
                if (product.Length === null || product.Length === undefined) {
                    len = parseFloat(0);
                }
                else {
                    len = parseFloat(product.Length);
                }

                if (product.Width === null || product.Width === undefined) {
                    wid = parseFloat(0);
                }
                else {
                    wid = parseFloat(product.Width);
                }

                if (product.Height === null || product.Height === undefined) {
                    height = parseFloat(0);
                }
                else {
                    height = parseFloat(product.Height);
                }
                if (product.CartoonValue === null || product.CartoonValue === undefined) {
                    qty = parseFloat(0);
                }
                else {
                    qty = parseFloat(product.CartoonValue);
                }
                if (len > 0 && wid > 0 && height > 0) {
                    total += ((len * wid * height) / 5000) * qty;
                }
            }


            var sum = total.toFixed(2);
            if (sum === 0.00) {
                return 0;
            }
            else {
                var num = [];
                num = sum.toString().split('.');
                var kgs = parseFloat($scope.getTotalKgs());
                if (num.length > 1) {
                    var as = parseFloat(num[1]);
                    if (as === 0) {
                        if (kgs > parseFloat(num[0]).toFixed(1)) {
                            return $scope.getTotalKgs();
                        }
                        else {
                            return parseFloat(num[0]).toFixed(1);
                        }
                    }
                    else {
                        if (as > 49) {

                            var r = parseFloat(num[0]) + 1;
                            if (kgs > r.toFixed(1)) {
                                return $scope.getTotalKgs();
                            }
                            else {
                                return r.toFixed(1);
                            }
                        }
                        else {
                            var s = parseFloat(num[0]) + 0.50;
                            if (kgs > s.toFixed(1)) {
                                return $scope.getTotalKgs();
                            }
                            else {
                                return s.toFixed(1);
                            }
                        }
                    }
                }
                else {
                    if (kgs > parseFloat(num[0]).toFixed(1)) {
                        return $scope.getTotalKgs();
                    }
                    else {
                        return parseFloat(num[0]).toFixed(1);
                    }
                }
            }
        }
    };

    $scope.getTotalKgs = function () {
        if ($scope.quotationDetail === undefined) {
            return;
        }

        else if ($scope.quotationDetail.QuotationPackages === undefined || $scope.quotationDetail.QuotationPackages === null) {
            return 0;
        }
        else if ($scope.quotationDetail.QuotationPackages.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.quotationDetail.QuotationPackages.length; i++) {
                var product = $scope.quotationDetail.QuotationPackages[i];
                if (product.Weight === null || product.Weight === undefined) {
                    total += parseFloat(0);
                }
                else {
                    if (product.CartoonValue === undefined || product.CartoonValue === null) {
                        var catroon = parseFloat(0);
                        total = total + parseFloat(product.Weight) * catroon;
                    }
                    else {
                        total = total + parseFloat(product.Weight) * product.CartoonValue;
                    }
                }
            }
            return Math.ceil(total).toFixed(1);
        }
        else {
            return 0;
        }
    };

    $scope.SumTotalCost = function (EstimatedCost, AdditionalSurcharge, FuelSurCharge) {
        if (EstimatedCost !== undefined && EstimatedCost !== null && EstimatedCost !== '' && AdditionalSurcharge !== undefined && AdditionalSurcharge !== null && AdditionalSurcharge !== '' && FuelSurCharge !== undefined && FuelSurCharge !== null && FuelSurCharge !== '') {
            var total = parseFloat(EstimatedCost.toString().replace(",", "")) + parseFloat(AdditionalSurcharge.toString().replace(",", "")) + parseFloat(FuelSurCharge.toString().replace(",", ""));
            return total.toFixed(2);
        }
        else {

        }
    };

    $scope.GetDays = function (ValidDays, CreatedOn) {
        if (ValidDays > 1) {
            return "Days";
        }
        else {
            return "Day";
        }
    };

    $scope.sendQuotationMail = function (IsValid, EmailDetail) {
        if (IsValid) {
            if ($scope.Header !== undefined || $scope.Header !== '' || $scope.Header !== null) {
                if ($scope.Header === "Send Quotation Email") {
                    if ($scope.quotationDetail !== undefined && $scope.quotationDetail.QuotationRateCard !== null && $scope.quotationDetail.QuotationRateCard.LogisticServiceId > 0) {
                        $rootScope.GetServiceValue = null;

                        AppSpinner.showSpinnerTemplate($scope.SendingQuotationEmail, $scope.Template);
                        $scope.quotationDetail.TotalEstimatedWeight = $scope.getChargeableWeight();
                        if ($scope.quotationDetail.AddressType !== undefined && $scope.quotationDetail.AddressType !== '' && $scope.quotationDetail.AddressType !== null) {
                            $scope.quotationDetail.AddressType = $scope.quotationDetail.AddressType.Name;
                        }
                        else {
                            $scope.quotationDetail.AddressType = null;
                        }
                        var mmonth = new Date($scope.quotationDetail.FuelMonthYear);
                        var nname = $scope.MonthName[mmonth.getMonth()];
                        var yyear = mmonth.getFullYear().toString().substr(2, 2);
                        var Emaildata = {
                            Name: EmailDetail.CustomerName,
                            CompanyName: (CompanyName !== undefined && CompanyName !== null ? CompanyName : $scope.CompanyName),
                            EmailTo: EmailDetail.Email,
                            EmailCc: EmailDetail.ToCCEmail,
                            EmailBcc: EmailDetail.ToBCCEmail,
                            SenderName: SalesRepresentativeDetail.Name,
                            SenderEMail: SalesRepresentativeDetail.Email,
                            SenderDept: SalesRepresentativeDetail.DeptName,
                            MailContent: EmailDetail.MailContent,
                            FuelMonth: nname + '-' + yyear,
                            LoginUserId: $scope.LoginUserId,
                            QuotationDetail: $scope.quotationDetail
                        };

                        QuotationService.SendQuotationMail(Emaildata).then(function (response) {
                            if (response.status === 200 && response.data !== null && response.data.Status) {
                                toaster.pop({
                                    type: 'success',
                                    title: $scope.Frayte_Success,
                                    body: $scope.Successfully_SentMail,
                                    showCloseButton: true
                                });
                                AppSpinner.hideSpinnerTemplate();
                                $uibModalInstance.close();
                            }
                            else {
                                AppSpinner.hideSpinnerTemplate();
                                toaster.pop({
                                    type: 'error',
                                    title: $scope.Frayte_Error,
                                    body: $scope.SendingMailErrorValidation,
                                    showCloseButton: true
                                });
                                $uibModalInstance.dismiss();
                            }

                        }, function () {
                            AppSpinner.hideSpinnerTemplate();
                            toaster.pop({
                                type: 'error',
                                title: $scope.Frayte_Error,
                                body: $scope.SendingMailErrorValidation,
                                showCloseButton: true
                            });
                            $uibModalInstance.dismiss();
                        });
                    }
                }
                else if ($scope.Header === "Send Rate Card Email") {
                    if (CustomerDetail !== undefined && CustomerDetail !== null) {
                        AppSpinner.showSpinnerTemplate($scope.SendingRateCardEmail, $scope.Template);
                        if ($scope.Email !== EmailDetail.Email) {
                            $scope.Email = EmailDetail.Email;
                            $scope.CustomerName = EmailDetail.CustomerName;
                        }
                        else {
                            $scope.Email = CustEmail;
                            $scope.CustomerName = CustName;
                        }

                        $scope.Customer = {
                            UserId: CustId,
                            LogisticServiceId: CustomerDetail.LogisticServiceId,
                            LogisticCompany: CustomerDetail.LogisticCompany,
                            LogisticType: CustomerDetail.LogisticType,
                            RateType: CustomerDetail.RateType,
                            FileType: '',
                            CustomerName: $scope.CustomerName,
                            CustomerEmail: $scope.Email,
                            SenderName: SalesRepresentativeDetail.Name,
                            SenderEMail: SalesRepresentativeDetail.Email,
                            SenderDept: SalesRepresentativeDetail.DeptName,
                            SendingOption: 'EMAIL',
                            CCEmail: EmailDetail.ToCCEmail,
                            BCCEmail: EmailDetail.ToBCCEmail,
                            MailContent: EmailDetail.MailContent
                        };
                        if ($scope.quotationDetail !== null && CustomerDetail !== null && CustomerDetail !== undefined) {
                            QuotationService.SendCustomerRateCardAsEmail($scope.Customer).then(function (response) {
                                if (response.status === 200 && response.data !== undefined && response.data !== null) {
                                    toaster.pop({
                                        type: 'success',
                                        title: $scope.Frayte_Success,
                                        body: $scope.Successfully_SentMail,
                                        showCloseButton: true
                                    });
                                    AppSpinner.hideSpinnerTemplate();
                                    $uibModalInstance.close();
                                }
                                else {
                                    AppSpinner.hideSpinnerTemplate();
                                    toaster.pop({
                                        type: 'error',
                                        title: $scope.Frayte_Error,
                                        body: $scope.SendingMailErrorValidation,
                                        showCloseButton: true
                                    });
                                }
                            }, function () {
                                AppSpinner.hideSpinnerTemplate();
                                toaster.pop({
                                    type: 'error',
                                    title: $scope.Frayte_Error,
                                    body: $scope.SendingMailErrorValidation,
                                    showCloseButton: true
                                });

                            });
                        }
                        else {
                            toaster.pop({
                                type: 'warning',
                                title: $scope.Frayte_Warning,
                                body: $scope.PleaseCorrect_ValidationErrors,
                                showCloseButton: true
                            });
                        }
                    }
                }
                else if ($scope.Header === "Send My Quote Email") {
                    if ($scope.item !== undefined && $scope.item !== null) {
                        AppSpinner.showSpinnerTemplate($scope.SendingQuotationEmail, $scope.Template);
                        var month = new Date(item.FuelMonthYear);
                        var name = $scope.MonthName[month.getMonth()];
                        var year = month.getFullYear().toString().substr(2, 2);
                        var CustomerEmailDetail = {
                            Name: EmailDetail.CustomerName,
                            CompanyName: item.CompanyName,
                            EmailTo: EmailDetail.Email,
                            EmailCc: EmailDetail.ToCCEmail,
                            EmailBcc: EmailDetail.ToBCCEmail,
                            SenderName: SalesRepresentativeDetail.Name,
                            SenderEMail: SalesRepresentativeDetail.Email,
                            SenderDept: SalesRepresentativeDetail.DeptName,
                            MailContent: EmailDetail.MailContent,
                            FuelMonth: name + '-' + year,
                            LoginUserId: $scope.LoginUserId,
                            QuotationDetail: item,
                            IsRateSend: EmailDetail.IsRateSend
                        };
                        QuotationService.SendViewPastQuotationMail(CustomerEmailDetail).then(function (response) {
                            if (response.status === 200 && response.data !== null && response.data.Status) {
                                toaster.pop({
                                    type: 'success',
                                    title: $scope.Frayte_Success,
                                    body: $scope.Successfully_SentMail,
                                    showCloseButton: true
                                });
                                AppSpinner.hideSpinnerTemplate();
                                $uibModalInstance.close();
                            }
                            else {
                                AppSpinner.hideSpinnerTemplate();
                                toaster.pop({
                                    type: 'error',
                                    title: $scope.Frayte_Error,
                                    body: $scope.SendingMailErrorValidation,
                                    showCloseButton: true
                                });
                                $uibModalInstance.dismiss();
                            }
                        }, function () {
                            AppSpinner.hideSpinnerTemplate();
                            toaster.pop({
                                type: 'error',
                                title: $scope.Frayte_Error,
                                body: $scope.SendingMailErrorValidation,
                                showCloseButton: true
                            });
                            $uibModalInstance.dismiss();
                        });
                    }
                }
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.Frayte_Warning,
                    body: $scope.SelectServiceFirst,
                    showCloseButton: true
                });
            }
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.PleaseCorrect_ValidationErrors,
                showCloseButton: true
            });
        }
    };

    function quoteno(QuotationId) {
        if (QuotationId.toString().length === 1) {
            return "000" + QuotationId;
        }
        else if (QuotationId.toString().length === 2) {
            return "00" + QuotationId;
        }
        else if (QuotationId.toString().length === 3) {
            return "0" + QuotationId;
        }
        else if (QuotationId.toString().length === 4) {
            return QuotationId;
        }
    }

    var specialCustomerCompany = function (UserId) {
        QuotationService.GetSpecialCustomerCompany(UserId).then(function (response) {
            if (response.data !== undefined && response.data !== null && response.data !== '') {
                $scope.Websitefrayte = response.data.MainWebsite;
                $scope.Phone = response.data.PhoneNo;
                $scope.SalesRepresentativedata.Name = response.data.OperationStaffName;
                $scope.SalesRepresentativedata.DeptName = response.data.UserPosition;
                $scope.SalesRepresentativedata.Email = response.data.OperationStaffEmail;
            }
        });
    };

    function init() {
        $scope.emailFormat = /^[a-z0-9._]+@[a-z]+\.[a-z.]/;
        $scope.MonthName = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        $scope.SalesRepresentativedata = SalesRepresentativeDetail;
        $scope.Header = Header;
        $scope.QuotationEmail = {};
        $scope.QuotationEmail.MailContent = MailContentText;

        if (CustomerName !== undefined && CustomerEmail !== undefined) {
            $scope.CustomerName = CustomerName;
            $scope.Email = CustomerEmail;
            $scope.QuotationEmail.CustomerName = CustomerName;
            $scope.QuotationEmail.Email = CustomerEmail;
        }
        else {
            $scope.Email = CustEmail;
            $scope.CustEmail = CustEmail;
            $scope.CustomerName = CustName;
            $scope.QuotationEmail.CustomerName = CustName;
            $scope.QuotationEmail.Email = CustEmail;
        }
        if (quotationDetail !== undefined) {
            $scope.quotationDetail = quotationDetail;
            $scope.quotationDetail.CustomerId = CustId;
            var date = new Date();
            var month = (date.getMonth() + 1).toString();
            if (quotationDetail.OperationZoneId === 1) {
                $scope.QuoteNo = 'QUHKG-' + date.getFullYear().toString().substr(-2) + (month.length == 1 ? "0" + month : month) + quoteno(quotationDetail.QuotationShipmentId);
            }
            else if (quotationDetail.OperationZoneId === 2) {
                $scope.QuoteNo = 'QUUK-' + date.getFullYear().toString().substr(-2) + (month.length == 1 ? "0" + month : month) + quoteno(quotationDetail.QuotationShipmentId);
            }
            if (CustomerDetail !== undefined) {
                $scope.CompanyName = CustomerDetail.CompanyName;
            }
            else {
                $scope.CompanyName = CompanyName;
            }

            var qmonth = new Date(quotationDetail.FuelMonthYear);
            var qname = $scope.MonthName[qmonth.getMonth()];
            var qyear = qmonth.getFullYear().toString().substr(2, 2);
            $scope.FuelMonth = qname + '-' + qyear;
        }
        else if (item !== undefined) {
            $scope.item = item;
            var date1 = new Date();
            var month1 = (date1.getMonth() + 1).toString();
            if (item.QuotationShipmentId.toString().length === 1) {

            }
            if (item.OperationZoneId === 1) {
                $scope.QuoteNo = 'QUHKG-' + date1.getFullYear().toString().substr(-2) + (month1.length == 1 ? "0" + month1 : month1) + quoteno(item.QuotationShipmentId);
            }
            else if (item.OperationZoneId === 2) {
                $scope.QuoteNo = 'QUUK-' + date1.getFullYear().toString().substr(-2) + (month1.length == 1 ? "0" + month1 : month1) + quoteno(item.QuotationShipmentId);
            }
            if (CustomerDetail !== undefined) {
                $scope.CompanyName = CustomerDetail.CompanyName;
            }
            else {
                $scope.CompanyName = item.CompanyName;
            }
            var fmonth = new Date(item.FuelMonthYear);
            var fname = $scope.MonthName[fmonth.getMonth()];
            var fyear = fmonth.getFullYear().toString().substr(2, 2);
            $scope.FuelMonth = fname + '-' + fyear;
        }
        else if (CustomerDetail !== undefined) {
            $scope.CustomerDetail = CustomerDetail;
            $scope.CustId = CustId;
            $scope.CustName = CustName;
            $scope.QuotationEmail.CustomerName = CustName;
            $scope.QuotationEmail.Email = CustEmail;
        }
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.webURL = config.Public_Link;
        var userInfo = SessionService.getUser();
        $scope.LoginUserId = userInfo.EmployeeId;
        if (userInfo.OperationZoneId === 1) {
            $scope.Websitefrayte = UtilityService.getPublicSiteName();
            $scope.Phone = "(+852) 2148 4880";
        }
        else {
            if (userInfo.UserType === "SPECIAL") {
                $scope.UserId = userInfo.EmployeeId;
                specialCustomerCompany($scope.UserId);
            }
            else {
                $scope.Websitefrayte = UtilityService.getPublicSiteName();
                $scope.Phone = "(+44) 01792 277295";
            }
        }
        $scope.QuotationEmail.IsRateSend = true;
        setMultilingualOptions();
    }

    init();
});