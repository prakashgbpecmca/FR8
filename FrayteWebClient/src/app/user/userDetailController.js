/**
 * Controller
 */
angular.module('ngApp.user').controller('UserDetailController', function ($scope, UtilityService, $uibModalInstance, RoleId, $location, UserId, SystemRoles, $filter, AppSpinner, $translate, SessionService, UserService, $uibModal, toaster, ShipmentService, $log, $state, $stateParams, ModalService, TopCountryService) {

    // Set Country Phone Code
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

    $scope.SetCountryPhoneCodeIssue = function (Country) {
        //ShipmentService.GetCountryByTimezone(timeZone.TimezoneId).then(function (response) {
        //    var Country = response.data;
        //    if (Country !== null) {
        //        $scope.SetPhoneCodeInfo(Country);
        //    }

        //}, function () { });
        if (Country !== null) {
            $scope.SetPhoneCodeInfo(Country);
            $scope.Country = Country;
        }
    };


    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['user', 'detail', 'customer', 'ErrorGetting', 'FrayteInformation', 'Branch', 'FrayteValidation', 'FrayteError',
            'SuccessfullySavedInformation', 'PleaseCorrectValidationErrors', 'ErrorSavingRecord', 'Cancel_Validation', 'Cancel', 'Confirmation']).then(function (translations) {

                $scope.TitleFrayteError = translations.FrayteError;
                $scope.TitleFrayteInformation = translations.FrayteInformation;
                $scope.TitleFrayteValidation = translations.FrayteValidation;

                $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
                $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
                $scope.TextValidation = translations.PleaseCorrectValidationErrors;

                $scope.TextErrorGettingUserDetail = translations.ErrorGetting + " " + translations.user + " " + translations.detail;
                $scope.TextErrorGettingCustomerRecord = translations.ErrorGetting + " " + translations.customer + " " + translations.Branch + " " + translations.detail + " " + translations.records;
                $scope.CancelConfirmation = translations.Cancel + " " + translations.Confirmation;
                $scope.CancelValidation = translations.Cancel_Validation;
            });
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
            "CreatedBy": 0,
            "UpdatedOn": "",
            "UpdatedBy": 0,
            //"UserAddress": null,
            "RoleId": $scope.RoleId
        };
    };

    $scope.GetUsers = function (query) {
        return UserService.GetAssociatedUsers(query).then(function (response) {
            return response.data;
        });
    };

    //$scope.SetPhoneCodeAndNumber = function (userType, telephoneNumber) {
    //    if (telephoneNumber !== undefined && telephoneNumber !== null && telephoneNumber !== '') {
    //        var n = telephoneNumber.indexOf(")");
    //        var code = telephoneNumber.substring(0, n + 1);

    //        if (userType === 'TelephoneNo') {
    //            $scope.userDetail.TelephoneNo = telephoneNumber.replace(code, "").trim();
    //        }
    //        else if (userType === 'MobileNo') {
    //            $scope.userDetail.MobileNo = telephoneNumber.replace(code, "").trim();
    //        }
    //        else if (userType === 'Fax') {
    //            $scope.userDetail.FaxNumber = telephoneNumber.replace(code, "").trim();
    //        }
    //        var countryCode = telephoneNumber.substring(2, n);
    //        var objects = $scope.countryPhoneCodes;

    //        for (var i = 0; i < objects.length; i++) {
    //            if (objects[i].PhoneCode === countryCode) {
    //                if (userType === 'TelephoneNo') {
    //                    $scope.userDetail.TelephoneCode = objects[i];
    //                    $scope.userPhomneCode = objects[i];
    //                }
    //                else if (userType === 'MobileNo') {
    //                    $scope.userDetail.MobileCode = objects[i];
    //                }
    //                else if (userType === 'Fax') {
    //                    $scope.userDetail.FaxCode = objects[i];
    //                }
    //                break;
    //            }
    //        }
    //    }
    //    else {
    //     if (userType === 'MobileNo') {
    //         $scope.userDetail.MobileCode = $scope.userPhomneCode;
    //     }
    //     else if (userType === 'Fax') {
    //         $scope.userDetail.FaxCode = $scope.userPhomneCode;
    //     }
    //    }
    //};

    $scope.SaveUserDetail = function (isValid, userDetail) {
        if (isValid) {

            userDetail.UserAddress.UserId = $scope.UserId;
            userDetail.UserAddress.Country = $scope.Country;


            //if (userDetail.TelephoneNo !== undefined && userDetail.TelephoneNo !== '') {
            //    userDetail.TelephoneNo = '(+' + $scope.userDetail.TelephoneCode.PhoneCode + ') ' + userDetail.TelephoneNo;
            //}

            //if (userDetail.MobileNo !== undefined && userDetail.MobileNo !== '') {
            //    userDetail.MobileNo = '(+' + $scope.userDetail.MobileCode.PhoneCode + ') ' + userDetail.MobileNo;
            //}

            //if (userDetail.FaxNumber !== undefined && userDetail.FaxNumber !== '') {
            //    userDetail.FaxNumber = '(+' + $scope.userDetail.FaxCode.PhoneCode + ') ' + userDetail.FaxNumber;
            //}

            UserService.SaveUser(userDetail).then(function (response) {

                $uibModalInstance.close(userDetail);
                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteInformation,
                    body: $scope.TextSuccessfullySavedInformation,
                    showCloseButton: true
                });
            }, function () {
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
        AppSpinner.showSpinnerTemplate($scope.spinnerMessage, $scope.Template);
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
                    if ($scope.userDetail.UserAddress !== null && $scope.userDetail.UserAddress.Country !== null) {
                        for (i = 0; i < $scope.Countries.length; i++) {
                            if ($scope.Countries[i].Name === $scope.userDetail.UserAddress.Country.Name) {
                                $scope.userDetail.Country = $scope.userDetail.UserAddress.Country;
                            }
                        }
                    }

                    //$scope.SetPhoneCodeAndNumber('TelephoneNo', $scope.userDetail.TelephoneNo);
                    //$scope.SetPhoneCodeAndNumber('MobileNo', $scope.userDetail.MobileNo);
                    //$scope.SetPhoneCodeAndNumber('Fax', $scope.userDetail.FaxNumber);
                    if ($scope.userDetail.Timezone !== null) {
                        $scope.SetCountryPhoneCodeIssue($scope.userDetail.Country);
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
        var sessionInfo = SessionService.getUser();
        $scope.tabs = sessionInfo.tabs;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.spinnerMessage = 'Loading User Detail';
        // set Multilingual Modal Popup Options
        setModalOptions();

        //$scope.frayteRoles = [
        //{ Id: 6, Name: 'Staff' },
        //{ Id: 8, Name: 'HSCodeOperator' }
        //];

        $scope.ShowFrayteuserPanel = true;

        $scope.UserId = UserId;
        $scope.frayteRoles = SystemRoles;
        $scope.RoleId = RoleId;
        var userInfo1 = SessionService.getUser();
        $scope.OperationZoneId = userInfo1.OperationZoneId;
        $scope.OperationZoneName = userInfo1.OperationZoneName;



        $scope.GetUserInitials();
        $scope.data = false;
    }

    init();

});