/**
 * Controller
 */
angular.module('ngApp.shipper').controller('ShipperAddEditController', function ($scope, $location, $filter, $translate, $state, SessionService, ShipperService, ShipmentService, $uibModal, toaster, uiGridConstants, $stateParams, ModalService, ReceiverService) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['Address', 'Book', 'DeleteHeader', 'DeleteBody', 'Tradelane', 'Detail', 'FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation', 'ErrorGetting', 'records', 'Branch']).then(function (translations) {
            $scope.headerTextPickupAddress = translations.Address + " " + translations.Book + " " + translations.DeleteHeader;
            $scope.bodyTextPickupAddress = translations.DeleteBody + " " + translations.Address;

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;

            $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.TextValidation = translations.PleaseCorrectValidationErrors;

            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
            $scope.TextSavingError = translations.ErrorSavingRecord;

            $scope.TextErrorGetting = translations.ErrorGetting + " " + translations.customer + " " + translations.Branch + " " + translations.Detail + " " + translations.records;
        });
    };

    //Pickup Address Panel.    
    $scope.ShowHidePickupPanel = function () {
        $scope.ShowPickupPanel = !$scope.ShowPickupPanel;
    };

    $scope.userType = '';
    $scope.UserMode = '';

    $scope.AddEditMode = function () {
        if ($scope.shipperDetail.UserId > 0) {
            return "Modify";
        }
        else {
            return "Add";
        }
    };

    $scope.goBack = function () {
        if ($state.is('admin.shippers-detail')) {
            $state.go('admin.shippers');
        }
        else if ($state.is('admin.receiver-detail')) {
            $state.go('admin.receivers');
        }
        else if ($state.is('customer.receiver-detail')) {
            $state.go('customer.receivers');
        }
        else if ($state.is('shipper.receiver-detail')) {
            $state.go('shipper.receivers');
        }
        else if ($state.is('shipper.shipment.addreceiver')) {
            $state.go('shipper.shipment.addressdetail', {}, { reload: true });
        }
        else if ($state.is('shipper.manage-detail')) {
            $state.go('shipper.current-shipment');
        }
    };

    $scope.NotShipperLogin = function () {
        if ($state.is('shipper.manage-detail')) {
            return false;
        }
        else {
            return true;
        }
    };

    $scope.submit = function (isValid, shipperDetail) {
        if (isValid) {

            if (shipperDetail.TelephoneNo !== undefined && shipperDetail.TelephoneNo !== '') {
                shipperDetail.TelephoneNo = '(+' + $scope.shipperDetail.TelephoneCode.PhoneCode + ') ' + shipperDetail.TelephoneNo;
            }

            if (shipperDetail.MobileNo !== undefined && shipperDetail.MobileNo !== '') {
                shipperDetail.MobileNo = '(+' + $scope.shipperDetail.MobileCode.PhoneCode + ') ' + shipperDetail.MobileNo;
            }

            if (shipperDetail.FaxNumber !== undefined && shipperDetail.FaxNumber !== '') {
                shipperDetail.FaxNumber = '(+' + $scope.shipperDetail.FaxCode.PhoneCode + ') ' + shipperDetail.FaxNumber;
            }

            if ($scope.UserMode === "Shipper") {
                ShipperService.SaveShipper(shipperDetail).then(function (response) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteInformation,
                        body: $scope.TextSuccessfullySavedInformation,
                        showCloseButton: true
                    });

                    $scope.goBack();

                }, function () {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorSavingRecord,
                        showCloseButton: true
                    });
                });
            }
            else if ($scope.UserMode === "Receiver") {
                if ($state.is('shipper.shipment.addreceiver')) {
                    shipperDetail.ShipperId = $scope.ShipperLoggedInId;
                    ReceiverService.SaveLoggedInReceiver(shipperDetail).then(function (response) {
                        toaster.pop({
                            type: 'success',
                            title: $scope.TitleFrayteInformation,
                            body: $scope.TextSuccessfullySavedInformation,
                            showCloseButton: true
                        });

                        //To Do: need to redirect back to the main state.
                        $scope.goBack();

                    }, function () {
                        toaster.pop({
                            type: 'error',
                            title: $scope.TitleFrayteError,
                            body: $scope.TextSavingError,
                            showCloseButton: true
                        });
                    });
                }
                else {
                    ReceiverService.SaveReceiver(shipperDetail).then(function (response) {
                        toaster.pop({
                            type: 'success',
                            title: $scope.TitleFrayteInformation,
                            body: $scope.TextSuccessfullySavedInformation,
                            showCloseButton: true
                        });

                        //To Do: need to redirect back to the main state.
                        $scope.goBack();

                    }, function () {
                        toaster.pop({
                            type: 'error',
                            title: $scope.TitleFrayteError,
                            body: $scope.TextSavingError,
                            showCloseButton: true
                        });
                    });
                }
            }
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

    $scope.RearrangeSerialNumbers = function (collectionObject, objectType) {

        if (collectionObject.length > 0) {
            for (var i = 0; i < collectionObject.length; i++) {
                collectionObject[i].SN = i + 1;
            }

        }
        if (objectType == 'PAB') {
            $scope.shipperDetail.PickupAddresses = collectionObject;
        }
    };

    //Address Book
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

                    return $scope.shipperDetail.PickupAddresses;
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
                            Country: ''
                        };
                    }
                    else {
                        return row.entity;
                    }
                }
            }
        });
        modalInstance.result.then(function (addressBooks) {
            $scope.addressBooks = addressBooks;
        }, function () {
        });
    };

    $scope.DeleteAddressBooks = function (row) {
        var modalOptions = {
            headerText: $scope.headerTextPickupAddress,
            bodyText: $scope.bodyTextPickupAddress + '?'
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            //Remove the row from shipperDetail.Pickup Address Book collection(array)
            var index = $scope.shipperDetail.PickupAddresses.indexOf(row.entity);
            $scope.shipperDetail.PickupAddresses.splice(index, 1);
            $scope.RearrangeSerialNumbers($scope.shipperDetail.PickupAddresses, 'PAB');
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
        enableGridMenu: false,
        enableColumnMenus: false,
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

    $scope.GetShipperInitials = function () {

        ShipmentService.GetInitials().then(function (response) {

            $scope.WorkingWeekDays = response.data.WorkingWeekDays;
            var weekDays = $scope.WorkingWeekDays;
            $scope.countries = response.data.Countries;
            $scope.timezones = response.data.TimeZones;
            $scope.countryPhoneCodes = response.data.CountryPhoneCodes;

            if ($scope.UserMode === "Shipper") {

                if ($state.is('shipper.manage-detail')) {
                    var userInfo = SessionService.getUser();

                    if (userInfo === undefined || userInfo === null || userInfo.SessionId === undefined || userInfo.SessionId === '') {
                        $state.go('shipper.current-shipment');
                    }
                    else {

                        $scope.shipperId = userInfo.EmployeeId;
                    }
                }
                else {

                    $scope.shipperId = $stateParams.shipperId;
                }
                if ($scope.shipperId > 0) {

                    ShipperService.GetShipperDetail($scope.shipperId).then(function (response) {

                        //Step 1: Set Basic Detail and Main Address
                        $scope.shipperDetail = response.data;
                        $scope.shipperDetail.RoleId = 5;


                        $scope.SetPhoneCodeAndNumber('TelephoneNo', $scope.shipperDetail.TelephoneNo);
                        $scope.SetPhoneCodeAndNumber('MobileNo', $scope.shipperDetail.MobileNo);
                        $scope.SetPhoneCodeAndNumber('Fax', $scope.shipperDetail.FaxNumber);

                        //Step 3: Set Address book grid
                        $scope.RearrangeSerialNumbers($scope.shipperDetail.PickupAddresses, 'PAB');
                        $scope.gridOptionsAddressBook.data = $scope.shipperDetail.PickupAddresses;


                        if ($scope.shipperDetail && $scope.shipperDetail.UserAddress != null && $scope.shipperDetail.UserAddress.Country.Code === "HKG") {
                            $scope.setStateDisable = true;
                            $scope.setZipDisable = true;
                            $scope.shipperDetail.UserAddress.Zip = null;
                            $scope.shipperDetail.UserAddress.State = null;
                        }
                        if ($scope.shipperDetail && $scope.shipperDetail.UserAddress != null && $scope.shipperDetail.UserAddress.Country.Code === "GBR") {
                            $scope.setStateDisable = true;
                            $scope.shipperDetail.UserAddress.State = null;
                        }
                    });
                }
                else {

                    //Step 1: Set Basic Detail and Main Address
                    $scope.NewShipper();
                    // Set Default WorkingWeek Day
                    if (weekDays !== null && weekDays !== undefined && weekDays.length > 0) {
                        for (var n = 0; n < weekDays.length; n++) {
                            if (weekDays[n].IsDefault) {
                                $scope.shipperDetail.WorkingWeekDay = weekDays[n];
                                break;
                            }
                        }
                    }
                    $scope.shipperDetail.RoleId = 5;

                    //Step 3: Set Address book grid
                    $scope.gridOptionsAddressBook.data = $scope.shipperDetail.PickupAddresses;
                }
            }
            else if ($scope.UserMode === "Receiver") {

                $scope.receiverId = $stateParams.receiverId;
                if ($scope.receiverId > 0) {
                    ReceiverService.GetReceiverDetail($scope.receiverId).then(function (response) {
                        //Step 1: Set Basic Detail and Main Address
                        $scope.shipperDetail = response.data;
                        $scope.shipperDetail.RoleId = 4;


                        $scope.SetPhoneCodeAndNumber('TelephoneNo', $scope.shipperDetail.TelephoneNo);
                        $scope.SetPhoneCodeAndNumber('MobileNo', $scope.shipperDetail.MobileNo);
                        $scope.SetPhoneCodeAndNumber('Fax', $scope.shipperDetail.FaxNumber);

                        //Step 3: Set Address book grid
                        $scope.RearrangeSerialNumbers($scope.shipperDetail.PickupAddresses, 'PAB');
                        $scope.gridOptionsAddressBook.data = $scope.shipperDetail.PickupAddresses;

                        if ($scope.shipperDetail && $scope.shipperDetail.UserAddress != null && $scope.shipperDetail.UserAddress.Country.Code === "HKG") {
                            $scope.setStateDisable = true;
                            $scope.setZipDisable = true;
                            $scope.shipperDetail.UserAddress.Zip = null;
                            $scope.shipperDetail.UserAddress.State = null;
                        }
                        if ($scope.shipperDetail && $scope.shipperDetail.UserAddress != null && $scope.shipperDetail.UserAddress.Country.Code === "GBR") {
                            $scope.setStateDisable = true;
                            $scope.shipperDetail.UserAddress.State = null;
                        }
                    });
                }
                else {

                    //Step 1: Set Basic Detail and Main Address
                    $scope.NewShipper();
                    // Set Default WorkingWeek Day
                    if (weekDays !== null && weekDays !== undefined && weekDays.length > 0) {
                        for (var m = 0; m < weekDays.length; m++) {
                            if (weekDays[m].IsDefault) {
                                $scope.shipperDetail.WorkingWeekDay = weekDays[m];
                                break;
                            }
                        }
                    }
                    $scope.shipperDetail.RoleId = 4;

                    //Step 3: Set Address book grid
                    $scope.gridOptionsAddressBook.data = $scope.shipperDetail.PickupAddresses;

                    //$scope.shipperDetail.WorkingStartTime = moment.utc().format();
                    //$scope.shipperDetail.WorkingEndTime = moment.utc().format();
                }
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGetting,
                showCloseButton: true
            });
        });
    };

    $scope.SetPhoneCodeAndNumber = function (userType, telephoneNumber) {
        if (telephoneNumber !== undefined && telephoneNumber !== null && telephoneNumber !== '') {
            var n = telephoneNumber.indexOf(")");
            var code = telephoneNumber.substring(0, n + 1);

            if (userType === 'TelephoneNo') {
                $scope.shipperDetail.TelephoneNo = telephoneNumber.replace(code, "").trim();
            }
            else if (userType === 'MobileNo') {
                $scope.shipperDetail.MobileNo = telephoneNumber.replace(code, "").trim();
            }
            else if (userType === 'Fax') {
                $scope.shipperDetail.FaxNumber = telephoneNumber.replace(code, "").trim();
            }

            var countryCode = telephoneNumber.substring(2, n);
            var objects = $scope.countryPhoneCodes;

            for (var i = 0; i < objects.length; i++) {
                if (objects[i].PhoneCode === countryCode) {
                    if (userType === 'TelephoneNo') {
                        $scope.shipperDetail.TelephoneCode = objects[i];
                    }
                    else if (userType === 'MobileNo') {
                        $scope.shipperDetail.MobileCode = objects[i];
                    }
                    else if (userType === 'Fax') {
                        $scope.shipperDetail.FaxCode = objects[i];
                    }
                    break;
                }
            }
        }
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

    $scope.NewShipper = function () {
        $scope.shipperDetail = {
            "PickupAddresses": [],
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

    $scope.InitailDetailOfCountry = function (data, showData, countries) {
        if (showData !== null && showData !== undefined && countries !== undefined && countries !== null) {
            if (!data) {
                $scope.shipperDetail.FaxCode = showData;
                $scope.shipperDetail.TelephoneCode = showData;
                $scope.shipperDetail.MobileCode = showData;
                for (var i = 0; i < countries.length; i++) {
                    if (countries[i].Name == showData.Name) {
                        $scope.shipperDetail.UserAddress.Country = countries[i];
                        // Set Time Zone based on Country Code.
                        $scope.shipperDetail.Timezone = countries[i].TimeZoneDetail;
                        break;
                    }
                }
                $scope.data = false;
            }
        }
    };
    $scope.InitailDetailOfCountryCodes = function (data, countryPhoneFaxCode, country) {
        if (country.Code !== null && country.Code !== '' && country.Code !== undefined) {
            if (country.Code === "HKG") {
                $scope.setStateDisable = true;
                $scope.setZipDisable = true;
                $scope.shipperDetail.UserAddress.Zip = null;
                $scope.shipperDetail.UserAddress.State = null;
            }
            else if (country.Code === "GBR") {
                $scope.setStateDisable = true;
                $scope.setZipDisable = false;
                $scope.shipperDetail.UserAddress.State = null;
            }
            else {
                $scope.setZipDisable = false;
                $scope.setStateDisable = false;
            }
        }
        if (!data) {
            for (var i = 0; i < countryPhoneFaxCode.length; i++) {
                if (countryPhoneFaxCode[i].Name == country.Name) {
                    $scope.shipperDetail.FaxCode = countryPhoneFaxCode[i];
                    $scope.shipperDetail.TelephoneCode = countryPhoneFaxCode[i];
                    $scope.shipperDetail.MobileCode = countryPhoneFaxCode[i];
                    break;
                }
            }

            // set timezone based on country
            $scope.shipperDetail.Timezone = country.TimeZoneDetail;


            // On selecting country need to set country phone code.
            $scope.data = false;
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

        if ($state.is('shipper.manage-detail')) {
            $scope.quickBooingIcon.value = true;
        }
        // set Multilingual Modal Popup Options
        setModalOptions();

        if ($state.is('shipper.shipment.addreceiver')) {
            $scope.ShipperLoggedInId = $stateParams.shipperId;
        }
        //hide Panels
        // $scope.ShowPickupAdd = false;
        $scope.ShowPickupPanel = false;

        $scope.isShipperLogin = true;

        if ($state.is('admin.shippers-detail')) {
            $scope.UserMode = "Shipper";
            $translate('Shipper').then(function (Shipper) {
                $scope.userType = Shipper;
            });
        }
        else if ($state.is('admin.receiver-detail')) {
            $scope.UserMode = "Receiver";
            $translate('Receiver').then(function (Receiver) {
                $scope.userType = Receiver;
            });
        }
        else if ($state.is('customer.receiver-detail')) {
            $scope.UserMode = "Receiver";
            $translate('Receiver').then(function (Receiver) {
                $scope.userType = Receiver;
            });
        }
        else if ($state.is('shipper.receiver-detail')) {
            $scope.UserMode = "Receiver";
            $translate('Receiver').then(function (Receiver) {
                $scope.userType = Receiver;
            });
        }
        else if ($state.is('shipper.shipment.addreceiver')) {
            $scope.UserMode = "Receiver";
            $translate('Receiver').then(function (Receiver) {
                $scope.userType = Receiver;
            });
        }
        else if ($state.is('shipper.manage-detail')) {
            $scope.UserMode = "Shipper";
            $translate('Shipper').then(function (Shipper) {
                $scope.userType = Shipper;
            });
        }

        $scope.GetShipperInitials();
        // On selecting country need to set country phone code.
        $scope.data = false;
    }

    init();
});