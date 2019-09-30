/**
 * Controller
 */
angular.module('ngApp.user').controller('UserDetailController', function ($scope, UtilityService, $uibModalInstance, RoleId, $location, UserId, SystemRoles, $filter, AppSpinner, $translate, SessionService, UserService, $uibModal, toaster, ShipmentService, $log, $state, $stateParams, ModalService, TopCountryService, TimeStringtoDateTime) {
    // make state and zip non required
    $scope.setStateAndZip = function (Code, stateZip) {
        if (Code !== null && Code !== '' && Code !== undefined) {
            if (Code === "HKG" && (stateZip === 'zip' || stateZip === 'state')) {
                return false;
            }
            else if (Code === "GBR" && stateZip === 'state') {
                return false;
            }
            else {
                return true;
            }
        }
        else {
            return true;
        }
    };

    $scope.customerOptionalField = function (action) {
        if (action !== undefined && action !== null && action !== '' && action === 'State') {
            if ($scope.userDetail && $scope.userDetail && $scope.userDetail.Country) {
                if ($scope.userDetail.Country.Code !== 'HKG' && $scope.userDetail.Country.Code !== 'GBR') {
                    return true;
                }
                else {
                    // $scope.directBooking.ShipFrom.State = '';
                    return false;
                }
            }
        }
        else if (action !== undefined && action !== null && action !== '' && action === 'PostCode') {
            if ($scope.userDetail && $scope.userDetail.Country) {
                if ($scope.userDetail.Country.Code !== 'HKG') {
                    return true;
                }
                else {
                    //  $scope.directBooking.ShipFrom.PostCode = '';
                    return false;
                }
            }
        }
    };

    // Set Country Phone Code
    $scope.SetPhoneCodeInfo = function (Country) {
        if (Country.Code !== null && Country.Code !== '' && Country.Code !== undefined) {
            if (Country.Code === "HKG") {
                $scope.setStateDisable = true;
                $scope.setZipDisable = true;
                $scope.userDetail.UserAddress.Zip = null;
                $scope.userDetail.UserAddress.State = null;
            }
            else if (Country.Code === "GBR") {
                $scope.setStateDisable = true;
                $scope.setZipDisable = false;
                $scope.userDetail.UserAddress.State = null;
            }
            else {
                $scope.setZipDisable = false;
                $scope.setStateDisable = false;
            }
        }

        if (Country !== undefined && Country !== null) {
            for (var i = 0 ; i < $scope.countryPhoneCodes.length ; i++) {
                if ($scope.countryPhoneCodes[i].CountryCode === Country.Code) {
                    $scope.code = "(+" + $scope.countryPhoneCodes[i].PhoneCode + ")";
                    break;
                }
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

    $scope.setCountryTimeZone = function (Country) {
        if (Country) {
            angular.forEach($scope.timezones, function (value, key) {
                if (value.TimezoneId == Country.TimeZoneDetail.TimezoneId) {
                    $scope.userDetail.Timezone = value;
                }
            });
        }
    };

    $scope.updateCountryTimeZone = function (TimeZone) {
        if (TimeZone) {
            angular.forEach($scope.timezones, function (value, key) {
                if (value.TimezoneId == TimeZone.TimezoneId) {
                    $scope.userDetail.Timezone = value;
                }
            });
        }
    };

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['user', 'detail', 'customer', 'ErrorGetting', 'FrayteInformation', 'Branch', 'FrayteValidation', 'FrayteError',
            'SuccessfullySavedInformation', 'PleaseCorrectValidationErrors', 'ErrorSavingRecord', 'Cancel_Validation', 'Cancel', 'Confirmation',
            'Successfully_Saved_User_Information', 'SavingUserDetail', 'LoadingUserDetail', 'TimeValidation']).then(function (translations) {
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
                $scope.LoadingUserDetail = translations.LoadingUserDetail;
                $scope.SavingUserDetail = translations.SavingUserDetail;
                $scope.ValidTime = translations.TimeValidation;

                $scope.GetUserInitials();
            });
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
                $scope.userDetail.WorkingStartTime = null;
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
                $scope.userDetail.WorkingEndTime = null;
            }
        }
    };

    //This will hide Frayteuser Panel by default.    
    $scope.ShowHideFrayteuserPanel = function () {
        $scope.ShowFrayteuserPanel = !$scope.ShowFrayteuserPanel;
    };

    $scope.UserGoBack = function () {
        var modalOptions = {
            headerText: $scope.CancelConfirmation,
            bodyText: $scope.CancelValidation
        };

        ModalService.Confirm({}, modalOptions).then(function () {
            $uibModalInstance.close($scope.userDetail);

        }, function () {


        });
    };

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

    $scope.NewUser = function () {
        $scope.userDetail = {
            "RoleName": "",
            "ManagerUser": {
                "UserId": 0,
                "AssociateType": "Manager",
                "ContactName": "",
                "Email": "",
                "TelephoneNo": "",
                "WorkingHours": ""

            },
            "UserAddress": {
                "OfficeAddressId": 0,
                "AddressTypeId": 0,
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
            "WorkingStartTime": setWorkingStartTime(),
            "WorkingEndTime": setWorkingEndTime(),
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
            "RoleId": $scope.RoleId,
            "LoginUserId": $scope.LoginUserId,
            "IsDirectBooking": false,
            "IsTradelaneBooking": false,
            "IsBreakBulk": false,
            "IsExpressBooking": false
        };
    };

    // Check if the email is alrady registered
    $scope.isEmailRegisered = false;

    $scope.checkEmailValidity = function (Email) {
        if (!Email) {
            UtilityService.UserEmailValidity($scope.userDetail.Email, "User").then(function (response) {
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

    $scope.GetUsers = function (query) {
        return UserService.GetAssociatedUsers(query, $scope.OperationZoneId).then(function (response) {
            return response.data;
        });
    };

    $scope.SaveUserDetail = function (isValid, userDetail) {
        if (isValid) {
            userDetail.UserAddress.UserId = $scope.UserId;
            userDetail.UserAddress.Country = $scope.Country;
            userDetail.StartTime = userDetail.WorkingStartTime;
            userDetail.EndTime = userDetail.WorkingEndTime;

            AppSpinner.showSpinnerTemplate($scope.SavingUserDetail, $scope.Template);
            UserService.SaveUser(userDetail).then(function (response) {
                if (response.data.Status) {
                    AppSpinner.hideSpinnerTemplate();
                    $uibModalInstance.close(userDetail);
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteInformation,
                        body: $scope.Successfully_Saved_User_Informations,
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

    $scope.GetRole = function (roleId) {
        var objects = $scope.frayteRoles;

        for (var i = 0; i < objects.length; i++) {
            if (objects[i].Id === roleId) {
                return objects[i];
            }
        }
    };

    $scope.GetUserInitials = function () {
        AppSpinner.showSpinnerTemplate($scope.LoadingUserDetail, $scope.Template);
        UserService.GetInitials().then(function (response) {
            $scope.WorkingWeekDays = response.data.WorkingWeekDays;
            $scope.countries = response.data.Countries;
            $scope.Countries = TopCountryService.TopCountryList(response.data.Countries);
            $scope.timezones = response.data.TimeZones;
            $scope.countryPhoneCodes = response.data.CountryPhoneCodes;
            $scope.operationTimeZone = response.data.OperationTimeZone;

            //After successfull initials get the user detail
            if ($scope.UserId === undefined || $scope.UserId === null || $scope.UserId === "0" || $scope.UserId === 0) {
                $scope.NewUser();

                if ($scope.OperationZoneName = "HKG") {
                    for (var a = 0 ; a < $scope.countries.length; a++) {
                        if ($scope.countries[a].Code == "HKG") {
                            $scope.SetPhoneCodeInfo($scope.countries[a]);
                            break;
                        }
                    }
                }
                else {
                    for (var b = 0 ; b < $scope.countries.length; b++) {
                        if ($scope.countries[b].Code == "GBR") {
                            $scope.SetPhoneCodeInfo($scope.countries[b]);
                            break;
                        }
                    }
                }

                // Set Default WorkingWeek Day
                if ($scope.WorkingWeekDays !== null && $scope.WorkingWeekDays !== undefined && $scope.WorkingWeekDays.length > 0) {
                    var weekDays = $scope.WorkingWeekDays;
                    for (var n = 0; n < weekDays.length; n++) {
                        if (weekDays[n].IsDefault) {
                            $scope.userDetail.WorkingWeekDay = weekDays[n];
                            break;
                        }
                    }
                }

                // Set the default time zone
                if ($scope.operationTimeZone !== null) {
                    var found = $filter('filter')($scope.timezones, { TimezoneId: $scope.operationTimeZone.TimezoneId });
                    if (found.length) {
                        $scope.userDetail.Timezone = found[0];
                    }
                }

                AppSpinner.hideSpinnerTemplate();
            }
            else {
                //Get User details 
                UserService.GetUserDetail($scope.UserId).then(function (response) {
                    $scope.userDetail = response.data;
                    $scope.userDetail.WorkingStartTime = response.data.startTime;
                    $scope.userDetail.WorkingEndTime = response.data.EndTime;
                    if ($scope.userDetail.UserAddress !== null && $scope.userDetail.UserAddress.Country !== null) {
                        for (i = 0; i < $scope.Countries.length; i++) {
                            if ($scope.Countries[i].Name === $scope.userDetail.UserAddress.Country.Name) {
                                $scope.userDetail.Country = $scope.userDetail.UserAddress.Country;
                            }
                        }
                    }

                    if ($scope.userDetail.Timezone !== null) {
                        $scope.SetCountryPhoneCodeIssue($scope.userDetail.Country);
                        $scope.updateCountryTimeZone($scope.userDetail.TimeZone);
                    }

                    $scope.RoleModel = $scope.GetRole($scope.userDetail.RoleId);

                    // Set the default time zone
                    if ($scope.operationTimeZone !== null && $scope.userDetail.Timezone !== null) {
                        var found = $filter('filter')($scope.timezones, { TimezoneId: $scope.userDetail.Timezone.TimezoneId });
                        if (found.length) {
                            $scope.userDetail.Timezone = found[0];
                        }
                    }
                    AppSpinner.hideSpinnerTemplate();
                }, function () {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorGettingUserDetail,
                        showCloseButton: true
                    });
                });
            }

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

    $scope.InitailDetailOfCountry = function (data, showData, countries) {
        if (!data) {
            $scope.userDetail.FaxCode = showData;
            $scope.userDetail.TelephoneCode = showData;
            $scope.userDetail.MobileCode = showData;
            $scope.data = true;
        }
    };

    $scope.ManagerDetailEmpty = function (ManagerDetail) {
        if (ManagerDetail !== null && ManagerDetail !== undefined) {
            $scope.userDetail.ManagerUser = null;

            // ManagerDetail.Manager = 0;

            ManagerDetail.ContactName = "";
            ManagerDetail.Email = "";
            ManagerDetail.TelephoneNo = "";
            ManagerDetail.WorkingHours = "";
        }
    };

    $scope.InitailDetailOfCountryCodes = function (Country) {
        $scope.Country = Country;
    };

    var getUserTab = function (tabs, tabKey) {
        if (tabs !== undefined && tabs !== null && tabs.length) {
            var tab = {};
            for (var i = 0; i < tabs.length; i++) {
                if (tabs[i].tabKey === tabKey) {
                    tab = tabs[i];
                    break;
                }
            }
            return tab;
        }
    };

    function init() {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        var sessionInfo = SessionService.getUser();
        $scope.tabs = sessionInfo.tabs;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        //$scope.spinnerMessage = $scope.LoadingUserDetail;

        setModalOptions();

        $scope.ShowFrayteuserPanel = true;

        $scope.UserId = UserId;
        $scope.frayteRoles = SystemRoles;
        $scope.RoleId = RoleId;
        var userInfo1 = SessionService.getUser();
        $scope.OperationZoneId = userInfo1.OperationZoneId;
        $scope.OperationZoneName = userInfo1.OperationZoneName;
        $scope.CreatedBy = userInfo1.EmployeeId;
        $scope.LoginUserId = userInfo1.EmployeeId;

        //$scope.GetUserInitials();
        $scope.data = false;
    }

    init();

});