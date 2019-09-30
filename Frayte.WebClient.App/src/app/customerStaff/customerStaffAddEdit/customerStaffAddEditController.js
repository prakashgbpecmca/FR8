angular.module('ngApp.customerStaff').controller('AddCustomerStaffController', function ($scope, toaster, SessionService, CustomerStaffService, AppSpinner, UserService, TopCountryService, UtilityService, $translate, ModalService, $uibModalInstance, UserId) {

    // Set Default WorkingStartTime and WorkingEndTime
    var setWorkingStartTime = function () {
        var h = "09";
        var m = "00";
        return h.toString() + m.toString();
    };

    var setWorkingEndTime = function () {
        var h = "17";
        var m = "00";
        return h.toString() + m.toString();
    };

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['user', 'detail', 'customer', 'ErrorGetting', 'FrayteInformation', 'Branch', 'FrayteValidation', 'FrayteError',
                    'SuccessfullySavedInformation', 'PleaseCorrectValidationErrors', 'ErrorSavingRecord', 'Cancel_Validation', 'Cancel', 'Confirmation',
                    'Successfully_Saved_User_Information', 'SavingUserDetail', 'Loading_Customer_Staff_Detail', 'TimeValidation', 'Add_Customer_Staff',
                    'LoadingCustomerDetail', 'Save_Customer_Staff', 'Edit_Customer_Staff', 'Saved_Customer_Staff_Information']).then(function (translations) {
                    $scope.TitleFrayteError = translations.FrayteError;
                    $scope.TitleFrayteInformation = translations.FrayteInformation;
                    $scope.TitleFrayteValidation = translations.FrayteValidation;
                    $scope.Successfully_Saved_User_Informations = translations.Successfully_Saved_User_Information;
                    $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
                    $scope.TextValidation = translations.PleaseCorrectValidationErrors;
                    $scope.TextErrorGettingUserDetail = translations.ErrorGetting + " " + translations.user + " " + translations.detail;
                    $scope.TextErrorGettingCustomerRecord = translations.ErrorGetting + " " + translations.customer + " " + translations.Branch + " " + translations.detail + " " + translations.records;
                    $scope.CancelConfirmation = translations.Cancel + " " + translations.Confirmation;
                    $scope.CancelValidation = translations.Cancel_Validation;
                    $scope.LoadingCustomerStaffDetail = translations.Loading_Customer_Staff_Detail;
                    $scope.SavingUserDetail = translations.SavingUserDetail;
                    $scope.ValidTime = translations.TimeValidation;
                    $scope.AddCustomerStaff = translations.Add_Customer_Staff;
                    $scope.EditCustomerStaff = translations.Edit_Customer_Staff;
                    $scope.LoadingCustomerDetail = translations.LoadingCustomerDetail;
                    $scope.SaveCustomerStaff = translations.Save_Customer_Staff;
                    $scope.SavedCustomerStaffInformation = translations.Saved_Customer_Staff_Information;

                    $scope.HeaderMessage();
                    $scope.GetUserInitials();
                });
    };

    $scope.GetUserInitials = function () {
        AppSpinner.showSpinnerTemplate($scope.LoadingCustomerStaffDetail, $scope.Template);
        UserService.GetInitials().then(function (response) {
            $scope.WorkingWeekDays = response.data.WorkingWeekDays;
            $scope.countries = response.data.Countries;
            $scope.Countries = TopCountryService.TopCountryList(response.data.Countries);
            $scope.timezones = response.data.TimeZones;
            $scope.countryPhoneCodes = response.data.CountryPhoneCodes;
            $scope.operationTimeZone = response.data.OperationTimeZone;

            if (UserId !== undefined && UserId !== null && UserId !== '' && UserId > 0) {
                editCustomerStaff(UserId);
            }

            AppSpinner.hideSpinnerTemplate();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGettingCustomerRecord,
                showCloseButton: true
            });
        });
    };

    var editCustomerStaff = function (UserId) {
        if (UserId !== undefined && UserId !== null && UserId !== '') {
            AppSpinner.showSpinnerTemplate($scope.LoadingCustomerStaffDetail, $scope.Template);
            CustomerStaffService.GetCustomerStaffDetail(UserId).then(function (response) {
                if (response.data !== undefined && response.data !== '' && response.data !== null) {
                    $scope.CustomerStaff = response.data;
                    $scope.CustomerStaff.LoginUserId = $scope.LoginUserId;
                    if ($scope.CustomerStaff.AssociateCustomer.length === 0) {
                        $scope.AddCustomer();
                    }
                    else {
                        var len = $scope.CustomerStaff.AssociateCustomer.length - 1;
                        for (i = 0; i < $scope.CustomerStaff.AssociateCustomer.length; i++) {
                            if (i === len) {
                                $scope.CustomerStaff.AssociateCustomer[i].custShow = true;
                            }
                            else {
                                $scope.CustomerStaff.AssociateCustomer[i].custShow = false;
                            }
                        }
                    }
                    for (i = 0; i < $scope.Countries.length; i++) {
                        if ($scope.Countries[i].CountryId === response.data.UserAddress.Country.CountryId) {
                            $scope.CustomerStaff.Country = $scope.Countries[i];
                            $scope.SetPhoneCodeInfo($scope.Countries[i]);
                        }
                    }
                }
                AppSpinner.hideSpinnerTemplate();
            },
            function () {
                AppSpinner.hideSpinnerTemplate();
                //if (response.status !== 401) {
                //    toaster.pop({
                //        type: 'error',
                //        title: $scope.TitleFrayteError,
                //        body: $scope.InitialDataValidation,
                //        showCloseButton: true
                //    });
                //}
            });
        }
    };

    $scope.InitailDetailOfCountryCodes = function (data, countryPhoneFaxCode, country) {

        $scope.SetPhoneCodeInfo(country);
        // Set Customer State and zip
        if (country.Code !== null && country.Code !== '' && country.Code !== undefined) {
            setCountryTimeZone(country);
            if (country.Code === "HKG") {
                $scope.setStateDisable = true;
                $scope.setZipDisable = true;
                $scope.CustomerStaff.UserAddress.Zip = null;
                $scope.CustomerStaff.UserAddress.State = null;
            }
            else if (country.Code === "GBR") {
                $scope.setStateDisable = true;
                $scope.setZipDisable = false;
                $scope.CustomerStaff.UserAddress.State = null;
            }
            else {
                $scope.setZipDisable = false;
                $scope.setStateDisable = false;
            }
        }

        if (!data) {
            for (var i = 0; i < countryPhoneFaxCode.length; i++) {
                if (countryPhoneFaxCode[i].Name == country.Name) {
                    $scope.CustomerStaff.FaxCode = countryPhoneFaxCode[i];
                    $scope.CustomerStaff.TelephoneCode = countryPhoneFaxCode[i];
                    break;
                }
            }
            $scope.data = true;
        }
    };

    //Set Country Phone Code
    $scope.SetPhoneCodeInfo = function (Country) {
        if (Country !== undefined && Country !== null) {
            for (var i = 0 ; i < $scope.countryPhoneCodes.length ; i++) {
                if ($scope.countryPhoneCodes[i].CountryCode === Country.Code) {
                    $scope.code = "(+" + $scope.countryPhoneCodes[i].PhoneCode + ")";
                    break;
                }
            }
        }
    };

    var setCountryTimeZone = function (Country) {
        if (Country) {
            angular.forEach($scope.timezones, function (value, key) {
                if (value.TimezoneId == Country.TimeZoneDetail.TimezoneId) {
                    $scope.CustomerStaff.Timezone = value;
                }
            });
        }
    };

    $scope.SaveCustomerStaffDetail = function (isValid, CustomerStaff) {
        if (isValid) {
            AppSpinner.showSpinnerTemplate($scope.SaveCustomerStaff, $scope.Template);
            $scope.CustomerStaff.UserAddress.Country = CustomerStaff.Country;
            UserService.SaveUser(CustomerStaff).then(function (response) {
                if (response.data.Status) {
                    AppSpinner.hideSpinnerTemplate();
                    $uibModalInstance.close(CustomerStaff);
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteInformation,
                        body: $scope.SavedCustomerStaffInformation,
                        showCloseButton: true
                    });
                }
                else {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorSavingRecord,
                        showCloseButton: true
                    });
                }
            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorSavingRecord,
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextValidation,
                showCloseButton: true
            });
        }
    };

    $scope.Cancel = function () {
        var modalOptions = {
            headerText: $scope.CancelConfirmation,
            bodyText: $scope.CancelValidation
        };
        ModalService.Confirm({}, modalOptions).then(function () {
            $uibModalInstance.dismiss();
        }, function () {

        });
    };

    $scope.checkEmailValidity = function (Email) {
        if (!Email) {
            UtilityService.UserEmailValidity($scope.CustomerStaff.Email, "CustomerStaff").then(function (response) {
                if (response.data) {
                    $scope.isEmailRegisered = response.data.Status;
                }
                else {
                    $scope.isEmailRegisered = false;
                }
            }, function (error) {

            });
        }
    };

    $scope.NewCustomerStaff = function () {
        $scope.CustomerStaff = {
            "RoleName": "",
            "ManagerUser": {
                "UserId": 0,
                "AssociateType": "",
                "ContactName": "",
                "Email": "",
                "TelephoneNo": "",
                "WorkingHours": ""
            },
            "AssociateCustomer": [{
                "CustomerStaffDetailId": null,
                "CustomerId": null,
                "CustomerName": null,
                "Email": null,
                "ContactNo": null,
                "WorkingHours": null
            }],
            "UserAddress": {
                "OfficeAddressId": 0,
                "AddressTypeId": 1,
                "UserId": 0,
                "Address": '',
                "Address2": '',
                "Address3": '',
                "Suburb": '',
                "City": '',
                "State": '',
                "Zip": ''
            },
            "Country": null,
            "IsCurrency": false,
            "IsFuelSurCharge": false,
            "UserId": 0,
            "CargoWiseId": "",
            "CargoWiseBardCode": "",
            "CompanyName": "",
            "ClientId": 0,
            "IsClient": true,
            "CountryOfOperation": "",
            "ContactName": "",
            "Email": "",
            "TelephoneCode": '',
            "TelephoneNo": "",
            "MobileCode": '',
            "MobileNo": "",
            "FaxCode": '',
            "FaxNumber": "",
            "startTime": setWorkingStartTime(),
            "EndTime": setWorkingEndTime(),
            "WorkingStartTime": null,
            "WorkingEndTime": null,
            "Timezone": null,
            "WorkingWeekDay": null,
            "VATGST": "",
            "ShortName": "",
            "Position": "",
            "Skype": "",
            "CreatedOn": "",
            "CreatedBy": $scope.CreatedBy,
            "UpdatedOn": "",
            "UpdatedBy": "",
            "RoleId": 20,
            "LoginUserId": $scope.LoginUserId
        };
    };

    $scope.customerOptionalField = function (action) {
        if (action !== undefined && action !== null && action !== '' && action === 'State') {
            if ($scope.CustomerStaff && $scope.CustomerStaff && $scope.CustomerStaff.Country) {
                if ($scope.CustomerStaff.Country.Code !== 'HKG' && $scope.CustomerStaff.Country.Code !== 'GBR') {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        else if (action !== undefined && action !== null && action !== '' && action === 'PostCode') {
            if ($scope.CustomerStaff && $scope.CustomerStaff.Country) {
                if ($scope.CustomerStaff.Country.Code !== 'HKG') {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
    };

    $scope.ValidStartTime = function (starttime) {
        if (starttime !== undefined && starttime !== null && starttime !== '') {
            if (parseInt(starttime, 0) > 2359) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteValidation,
                    body: $scope.ValidTime,
                    showCloseButton: true
                });
                $scope.CustomerStaff.WorkingStartTime = null;
            }
        }
    };

    $scope.GetCustomerDetail = function (Name, index) {

        AppSpinner.showSpinnerTemplate($scope.LoadingCustomerDetail, $scope.Template);
        return CustomerStaffService.GetCustomer(Name, $scope.RoleId).then(function (response) {
            if (response.data.length > 0) {
                for (i = 0; i < response.data.length; i++) {
                    response.data[i].FillContactName = response.data[i].ContactName;
                    response.data[i].Index = index;
                }
                $scope.record = response.data;
                AppSpinner.hideSpinnerTemplate();
            }
            else {
                AppSpinner.hideSpinnerTemplate();
            }
            return response.data;
        });
    };

    $scope.SetCustomerValue = function (item, model, label, Index) {
        if (item !== undefined && Index !== undefined && Index !== null && Index !== '') {
            $scope.CustomerStaff.AssociateCustomer[Index].CustomerStaffDetailId = model.CustomerStaffDetailId;
            $scope.CustomerStaff.AssociateCustomer[Index].CustomerId = model.UserId;
            $scope.CustomerStaff.AssociateCustomer[Index].CustomerName = model.ContactName;
            $scope.CustomerStaff.AssociateCustomer[Index].Email = model.Email;
            $scope.CustomerStaff.AssociateCustomer[Index].ContactNo = model.TelephoneNo;
            $scope.CustomerStaff.AssociateCustomer[Index].WorkingHours = model.WorkingHours;
        }
    };

    $scope.AddCustomer = function () {
        $scope.CustomerStaff.AssociateCustomer.push({
            CustomerStaffDetailId: 0,
            CustomerId: null,
            CustomerName: null,
            Email: null,
            ContactNo: null,
            WorkingHours: null
        });
        var len = $scope.CustomerStaff.AssociateCustomer.length - 1;
        for (i = 0; i < $scope.CustomerStaff.AssociateCustomer.length; i++) {
            if (i === len) {
                $scope.CustomerStaff.AssociateCustomer[i].custShow = true;
            }
            else {
                $scope.CustomerStaff.AssociateCustomer[i].custShow = false;
            }
        }
    };

    $scope.RemoveDetail = function (Associatecutomer) {
        if (Associatecutomer !== undefined && Associatecutomer !== null) {
            var index = $scope.CustomerStaff.AssociateCustomer.indexOf(Associatecutomer);
            if (Associatecutomer.CustomerStaffDetailId > 0) {
                CustomerStaffService.RemoveAssociateCustomer(Associatecutomer.CustomerStaffDetailId).then(function (response) {
                    if (response.data.Status === true) {                        
                        $scope.CustomerStaff.AssociateCustomer.splice(index, 1);
                        $scope.AssociateCustopmerDetail = angular.copy($scope.CustomerStaff.AssociateCustomer);
                        $scope.CustomerStaff.AssociateCustomer = [];
                        $scope.CustomerStaff.AssociateCustomer = $scope.AssociateCustopmerDetail;
                        var len = $scope.CustomerStaff.AssociateCustomer.length - 1;
                        for (i = 0; i < $scope.CustomerStaff.AssociateCustomer.length; i++) {
                            if (i === len) {
                                $scope.CustomerStaff.AssociateCustomer[i].custShow = true;
                            }
                            else {
                                $scope.CustomerStaff.AssociateCustomer[i].custShow = false;
                            }
                        }
                    }
                }, function () {
                    //toaster.pop({
                    //    type: 'error',
                    //    title: $scope.TitleFrayteError,
                    //    body: $scope.RemovePackageValidation,
                    //    showCloseButton: true
                    //});
                });
            }
            else {
                $scope.CustomerStaff.AssociateCustomer.splice(index, 1);
                $scope.AssociateCustopmerDetail = angular.copy($scope.CustomerStaff.AssociateCustomer);
                $scope.CustomerStaff.AssociateCustomer = [];
                $scope.CustomerStaff.AssociateCustomer = $scope.AssociateCustopmerDetail;
                var len = $scope.CustomerStaff.AssociateCustomer.length - 1;
                for (i = 0; i < $scope.CustomerStaff.AssociateCustomer.length; i++) {
                    if (i === len) {
                        $scope.CustomerStaff.AssociateCustomer[i].custShow = true;
                    }
                    else {
                        $scope.CustomerStaff.AssociateCustomer[i].custShow = false;
                    }
                }
            }
        }
    };

    $scope.ValidEndTime = function (endtime) {
        if (endtime !== undefined && endtime !== null && endtime !== '') {
            if (parseInt(endtime, 0) > 2359) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteValidation,
                    body: $scope.ValidTime,
                    showCloseButton: true
                });
                $scope.CustomerStaff.WorkingEndTime = null;
            }
        }
    };

    $scope.SetCountryPhoneCodeIssue = function (Country) {
        if (Country.Code !== null && Country.Code !== '' && Country.Code !== undefined) {
            $scope.SetPhoneCodeInfo(Country);
            $scope.updateCountryTimeZone(Country.TimeZoneDetail);
            if (Country !== null) {
                $scope.SetPhoneCodeInfo(Country);
                $scope.Country = Country;
            }
        }
    };

    $scope.customerPanel = function () {
        if ($scope.IsCustomerDetailCollapsed === true) {
            $scope.IsCustomerDetailCollapsed = false;
        }
        else {
            $scope.IsCustomerDetailCollapsed = true;
        }
    };

    $scope.HeaderMessage = function () {
        if (UserId !== undefined && UserId !== null && UserId !== '' && UserId > 0) {
            $scope.Header = $scope.EditCustomerStaff;
        }
        else {
            $scope.Header = $scope.AddCustomerStaff;
        }
    };

    function init() {
        var userInfo = SessionService.getUser();
        $scope.CreatedBy = userInfo.EmployeeId;
        $scope.LoginUserId = userInfo.EmployeeId;
        $scope.RoleId = userInfo.RoleId;
        $scope.submitted = true;

        $scope.IsCustomerDetailCollapsed = false;

        $scope.NewCustomerStaff();

        setModalOptions();
    }

    init();

});