/**
 * Controller
 */
angular.module('ngApp.agent').controller('AgentDetailController', function ($scope, $location, $filter, $translate, $state, SessionService, UserService, AgentService, CountryService, $uibModal, ModalService, toaster, uiGridConstants, TradelaneService, TimeZoneService, ShipmentService, $stateParams) {

    var setModalOptions = function () {
        $translate(['Address', 'Book', 'AssociatedUser', 'This', 'DeleteHeader', 'DeleteBody', 'FrayteError', 'FrayteInformation', 'FrayteValidation', 'detail', 'user', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation', 'ErrorGetting', 'customer', 'Branch', 'records']).then(function (translations) {
            $scope.headerTextAddressBook = translations.Address + " " + translations.Book + " " + translations.DeleteHeader;
            $scope.bodyTextAddressBook = translations.DeleteBody + " " + translations.This + " " + translations.Address;
            $scope.headerTextAssociatedUser = translations.AssociatedUser + " " + translations.DeleteHeader;
            $scope.bodyTextAssociatedUser = translations.DeleteBody + " " + translations.This + " " + translations.AssociatedUser;

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;

            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;

            $scope.TextErrorGettingUserDetail = translations.ErrorGetting + " " + translations.user + " " + translations.detail;
            $scope.TextErrorGettingCustomerDetail = translations.ErrorGetting + " " + translations.customer + " " + translations.Branch + " " + translations.detail + " " + translations.records;
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
    $scope.NewAgent = function () {
        $scope.agentDetail = {
            "AccountUser": {
                "UserId": 0,
                "AssociateType": "",
                "ContactName": "",
                "Email": "",
                "TelephoneNo": "",
                "WorkingHours": ""
            },
            "DocumentUser": {
                "UserId": 0,
                "AssociateType": "",
                "ContactName": "",
                "Email": "",
                "TelephoneNo": "",
                "WorkingHours": ""
            },
            "ManagerUser": {
                "UserId": 0,
                "AssociateType": "",
                "ContactName": "",
                "Email": "",
                "TelephoneNo": "",
                "WorkingHours": ""
            },
            "OperationUser": {
                "UserId": 0,
                "AssociateType": "",
                "ContactName": "",
                "Email": "",
                "TelephoneNo": "",
                "WorkingHours": ""
            },
            "AssociatedUsers": [],
            "OtherAddresses": [],
            "UserId": 0,
            "CargoWiseId": "",
            "CargoWiseBardCode": "",
            "CompanyName": "",
            "ClientId": 0,
            "IsClient": false,
            "CountryOfOperation": "",
            "ContactName": "",
            "Email": "",
            "TelephoneNo": "",
            "MobileNo": "",
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
            "UserShipmentTypeId":0,
            "IsAir": false,
            "IsExpryes": false,
            "IsSea": false,
            "UserAddress": {
                "UserAddressId": 0,
                "UserId": 0,
                "AddressTypeId": 0,
                "Address": "",
                "Address2": "",
                "Address3": "",
                "Suburb": "",
                "City": "",
                "State": "",
                "Zip": "",
                "Country": null
            },
            "RoleId": 0
        };
    };

    $scope.GetUsers = function (query) {
        return UserService.GetAssociatedUsers(query).then(function (response) {
            return response.data;
        });
    };

    $scope.AgentSave = function (isValid, agentDetail) {
        if (isValid) {
            if (agentDetail.TelephoneNo !== undefined && agentDetail.TelephoneNo !==null && agentDetail.TelephoneNo !== '') {
                agentDetail.TelephoneNo = '(+' + $scope.agentDetail.TelephoneCode.PhoneCode + ') ' + agentDetail.TelephoneNo;
            }

            if (agentDetail.MobileNo !== undefined && agentDetail.MobileNo !==null && agentDetail.MobileNo !== '') {
                agentDetail.MobileNo = '(+' + $scope.agentDetail.MobileCode.PhoneCode + ') ' + agentDetail.MobileNo;
            }

            if (agentDetail.FaxNumber !== undefined && agentDetail.FaxNumber !==null && agentDetail.FaxNumber !== '') {
                agentDetail.FaxNumber = '(+' + $scope.agentDetail.FaxCode.PhoneCode + ') ' + agentDetail.FaxNumber;
            }

            AgentService.SaveAgent(agentDetail).then(function (response) {
                $state.go('admin.agents');
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

    //This will hide Address Panel by default.    
    $scope.ShowHideOtherAdressPanel = function () {
        $scope.ShowOtherAddressPanel = !$scope.ShowOtherAddressPanel;
    };

    //This will hide Frayteuser Panel by default.    
    $scope.ShowHideFrayteuserPanel = function () {
        $scope.ShowAssociatedUserPanel = !$scope.ShowAssociatedUserPanel;
    };

    $scope.RearrangeSerialNumbers = function (collectionObject, objectType) {

        if (collectionObject.length > 0) {
            for (var i = 0; i < collectionObject.length; i++) {
                collectionObject[i].SN = i + 1;
            }

        }
        if (objectType == 'PAB') {
            $scope.agentDetail.OtherAddresses = collectionObject;
        }
    };

    //Start : Address Panel
    $scope.AddEditAddressBook = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'shipper/shipperAddressBook/addressBookAddEdit.tpl.html',
            controller: 'AddressBookAddEditController',
            windowClass: '',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                mode: function () {
                    if (row === undefined) {
                        return 'Add';
                    }
                    else {
                        return 'Modify';
                    }
                },
                addressBooks: function () {
                    return $scope.agentDetail.OtherAddresses;
                },
                addressBook: function () {
                    if (row === undefined) {
                        return {
                            SN: 0,
                            UserAddressId: 0,
                            Address: '',
                            Address2: '',
                            Address3: '',
                            Suburb: '',
                            City: '',
                            State: '',
                            Zip: '',
                            Country: null
                        };
                    }
                    else {
                        return row.entity;
                    }
                }
            }
        });

        modalInstance.result.then(function (otherAddresses) {
            $scope.agentDetail.OtherAddresses = otherAddresses;
        }, function () {
        });
    };

    $scope.DeleteAddressBooks = function (row) {
        var modalOptions = {
            headerText: $scope.headerTextAddressBook,
            bodyText: $scope.bodyTextAddressBook + '?'
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            //Remove the row from shipperDetail.Pickup Address Book collection(array)
            var index = $scope.agentDetail.OtherAddresses.indexOf(row.entity);
            $scope.agentDetail.OtherAddresses.splice(index, 1);
            $scope.RearrangeSerialNumbers($scope.agentDetail.OtherAddresses, 'PAB');
        });

    };

    $scope.gridOptionsAddressBook = {
        showFooter: true,
        enableSorting: true,
        multiSelect: false,
        enableFiltering: true,
        enableRowSelection: true,
        enableSelectAll: false,
        enableRowHeaderSelection: false,
        selectionRowHeaderWidth: 35,
        noUnselect: true,
        enableGridMenu: true,
        enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
        enableVerticalScrollbar: uiGridConstants.scrollbars.NEVER,
        columnDefs: [
          { name: 'Address', headerCellFilter: 'translate' },
          { name: 'City', headerCellFilter: 'translate' },
          { name: 'State', headerCellFilter: 'translate' },
          { name: 'Zip', headerCellFilter: 'translate' },
           { name: 'Country.Name', displayName: 'Country', headerCellFilter: 'translate' },
          { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "shipper/shipperAddressBook/addressBookEditButton.tpl.html", width: 65 }
        ]
    };

    //End : Address Panel lane

    $scope.GetAgentScreenInitials = function () {

        ShipmentService.GetInitials().then(function (response) {
            $scope.WorkingWeekDays = response.data.WorkingWeekDays;
            $scope.countries = response.data.Countries;
            $scope.timezones = response.data.TimeZones;
            $scope.countryPhoneCodes = response.data.CountryPhoneCodes;
            //After successfull initials get the user detail
            if ($scope.AgentId === undefined || $scope.AgentId === null || $scope.AgentId === "0") {

                //Step 1: Set Basic Detail and Main Address
                $scope.NewAgent();
                // Set Default WorkingWeek Day
                if ($scope.WorkingWeekDays !== null && $scope.WorkingWeekDays !== undefined && $scope.WorkingWeekDays.length > 0) {
                    var weekDays = $scope.WorkingWeekDays;
                    for (var n = 0; n < weekDays.length; n++) {
                        if (weekDays[n].IsDefault) {
                            $scope.agentDetail.WorkingWeekDay = weekDays[n];
                            break;
                        }
                    }
                }
                //Step 2: Set Address book grid
                $scope.gridOptionsAddressBook.data = $scope.agentDetail.OtherAddresses;
                $scope.gridOptionsAssociatedUsers.data = $scope.agentDetail.AssociatedUsers;

            }
            else {

                //Get User details 
                AgentService.GetAgentDetail($scope.AgentId).then(function (response) {
                    //Step 1: Set Basic Detail and Main Address
                    $scope.agentDetail = response.data;

                    $scope.SetPhoneCodeAndNumber('TelephoneNo', $scope.agentDetail.TelephoneNo);
                    $scope.SetPhoneCodeAndNumber('MobileNo', $scope.agentDetail.MobileNo);
                    $scope.SetPhoneCodeAndNumber('Fax', $scope.agentDetail.FaxNumber);

                    //Step 2: Set Address book grid
                    $scope.RearrangeSerialNumbers($scope.agentDetail.OtherAddresses, 'PAB');
                    $scope.gridOptionsAddressBook.data = $scope.agentDetail.OtherAddresses;
                    $scope.gridOptionsAssociatedUsers.data = $scope.agentDetail.AssociatedUsers;

                    if ($scope.agentDetail && $scope.agentDetail.UserAddress != null && $scope.agentDetail.UserAddress.Country.Code === "HKG") {
                        $scope.setStateDisable = true;
                        $scope.setZipDisable = true;
                        $scope.agentDetail.UserAddress.Zip = null;
                        $scope.agentDetail.UserAddress.State = null;
                    }
                    if ($scope.agentDetail && $scope.agentDetail.UserAddress != null && $scope.agentDetail.UserAddress.Country.Code === "GBR") {
                        $scope.setStateDisable = true;
                        $scope.agentDetail.UserAddress.State = null;
                    }

                }, function () {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorGettingUserDetail,
                        showCloseButton: true
                    });
                });
            }

        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGettingCustomerDetail,
                showCloseButton: true
            });
        });
    };

    $scope.SetPhoneCodeAndNumber = function (userType, telephoneNumber) {
        if (telephoneNumber !== undefined && telephoneNumber !== null && telephoneNumber !== '') {
            var n = telephoneNumber.indexOf(")");
            var code = telephoneNumber.substring(0, n + 1);

            if (userType === 'TelephoneNo') {
                $scope.agentDetail.TelephoneNo = telephoneNumber.replace(code, "").trim();
            }
            else if (userType === 'MobileNo') {
                $scope.agentDetail.MobileNo = telephoneNumber.replace(code, "").trim();
            }
            else if (userType === 'Fax') {
                $scope.agentDetail.FaxNumber = telephoneNumber.replace(code, "").trim();
            }

            var countryCode = telephoneNumber.substring(2, n);
            var objects = $scope.countryPhoneCodes;

            for (var i = 0; i < objects.length; i++) {
                if (objects[i].PhoneCode === countryCode) {
                    if (userType === 'TelephoneNo') {
                        $scope.agentDetail.TelephoneCode = objects[i];
                    }
                    else if (userType === 'MobileNo') {
                        $scope.agentDetail.MobileCode = objects[i];
                    }
                    else if (userType === 'Fax') {
                        $scope.agentDetail.FaxCode = objects[i];
                    }
                    break;
                }
            }
        }
    };

    // Associated User Panel    
    var workingHourTemplate = '<div class="ui-grid-cell-contents">{{grid.appScope.GetWorkingHoursDetail(row)}}</div>';

    $scope.GetWorkingHoursDetail = function (row) {
        if (row === undefined) {
            return '';
        }
        else {
            var str = row.entity.WorkingStartTime;
            var str1 = row.entity.WorkingEndTime;
            var newStr = str.substring(0, 2) + ':' + str.substring(2, 4);
            var newStr1 = str1.substring(0, 2) + ':' + str1.substring(2, 4);
            return newStr + ' - ' + newStr1;
        }
    };

    $scope.gridOptionsAssociatedUsers = {
        showFooter: true,
        enableSorting: true,
        multiSelect: false,
        enableFiltering: true,
        enableRowSelection: true,
        enableSelectAll: false,
        enableRowHeaderSelection: false,
        selectionRowHeaderWidth: 35,
        noUnselect: true,
        enableGridMenu: true,
        enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
        enableVerticalScrollbar: uiGridConstants.scrollbars.NEVER,
        columnDefs: [
          { name: 'Name', headerCellFilter: 'translate' },
          { name: 'UserType', displayName: 'UserType', headerCellFilter: 'translate' },
          { name: 'Email', headerCellFilter: 'translate' },
          { name: 'TelephoneNo', displayName: 'TelephoneNo', headerCellFilter: 'translate' },
          { name: 'WorkingStartTime', displayName: 'WorkingTime', headerCellFilter: 'translate', cellTemplate: workingHourTemplate },
          { name: 'WorkingWeekDays', displayName: 'WorkingDays', headerCellFilter: 'translate' },
          { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "agent/agentUserAssociated/agentAssociatedUserAddEditCellTemplate.tpl.html", width: 65 }
        ]
    };

    $scope.AddAssociateUser = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'agent/agentUserAssociated/addAssociatedUser.tpl.html',
            controller: 'AddAssociatedController',
            windowClass: '',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                mode: function () {
                    if (row === undefined) {
                        return 'Add';
                    }
                    else {
                        return 'Modify';
                    }
                },
                associatedUsers: function () {
                    return $scope.agentDetail.AssociatedUsers;
                },
                associatedUser: function () {
                    if (row === undefined) {
                        return {
                            AgentAssociatedUserId: 0,
                            AgentId: 0,
                            UserType: null,
                            Name: '',
                            Email: '',
                            TelephoneNo: '',
                            WorkingStartTime: '',
                            WorkingEndTime: '',
                            WorkingWeekDays: null
                        };
                    }
                    else {
                        return row.entity;
                    }
                }
            }
        });

        modalInstance.result.then(function (associatedUser) {
            $scope.agentDetail.AssociatedUsers = associatedUser;
        }, function () {
        });
    };

    // delete associatedUser
    $scope.DeleteAssociatedUser = function (row) {
        var modalOptions = {
            headerText: $scope.headerTextAssociatedUser,
            bodyText: $scope.bodyTextAssociatedUser + '?'
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            //Remove the row from Associated User's collection(array)
            var index = $scope.agentDetail.AssociatedUsers.indexOf(row.entity);
            $scope.agentDetail.AssociatedUsers.splice(index, 1);
        });

    };

    // Set initail Detail Of Country 
    $scope.InitailDetailOfCountry = function (data, showData, countries) {
        if (!data) {
            $scope.agentDetail.FaxCode = showData;
            $scope.agentDetail.TelephoneCode = showData;
            $scope.agentDetail.MobileCode = showData;
            for (var i = 0; i < countries.length; i++) {
                if (countries[i].Name == showData.Name) {
                    $scope.agentDetail.UserAddress.Country = countries[i];
                    break;
                }
            }
            $scope.data = true;
        }
    };
    $scope.InitailDetailOfCountryCodes = function (data, countryPhoneFaxCode, country) {
        // Set Customer State and zip
        if (country.Code !== null && country.Code !== '' && country.Code !== undefined) {
            if (country.Code === "HKG") {
                $scope.setStateDisable = true;
                $scope.setZipDisable = true;
                $scope.agentDetail.UserAddress.Zip = null;
                $scope.agentDetail.UserAddress.State = null;
            }
            else if (country.Code === "GBR") {
                $scope.setStateDisable = true;
                $scope.setZipDisable = false;
                $scope.agentDetail.UserAddress.State = null;
            }
            else {
                $scope.setZipDisable = false;
                $scope.setStateDisable = false;
            }
        }

        if (!data) {
            for (var i = 0; i < countryPhoneFaxCode.length; i++) {
                if (countryPhoneFaxCode[i].Name == country.Name) {
                    $scope.agentDetail.FaxCode = countryPhoneFaxCode[i];
                    $scope.agentDetail.TelephoneCode = countryPhoneFaxCode[i];
                    $scope.agentDetail.MobileCode = countryPhoneFaxCode[i];
                    break;
                }
            }
            $scope.data = true;
        }
    };

    // Set State And Zip for "HKG" and "GBR"
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

    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();


        //hide Pickup Address Panel
        $scope.ShowOtherAddressPanel = false;
        //hide Frayteuser Panel
        $scope.ShowAssociatedUserPanel = false;
        if ($stateParams.AgentId === undefined) {
            $scope.AgentId = "0";
        }
        else {
            $scope.AgentId = $stateParams.AgentId;
        }
        //getting timezones, country and  their code
        $scope.GetAgentScreenInitials();

        $scope.data = false;
    }
    init();

});
