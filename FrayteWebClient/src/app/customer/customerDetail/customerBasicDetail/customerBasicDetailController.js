angular.module('ngApp.customer').controller('CustomerBasicDetailController', function (AppSpinner, UtilityService, $scope, $state, $location, $translate, $stateParams, $filter, CustomerService, SessionService, $uibModal, uiGridConstants, toaster, ModalService, TradelaneService, CarrierService, CountryService, ShipmentService, UserService, TopCountryService, TopCurrencyService, $rootScope) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['Address', 'Book', 'DeleteHeader', 'DeleteBody', 'Tradelane', 'SuccessfullySavedInformation', 'FrayteInformation', 'FrayteValidation',
            'PleaseCorrectValidationErrors', 'FrayteError', 'ErrorSavingRecord', 'ErrorGetting', 'customer', 'detail', 'Zone',
            'Shipment_Type', 'Select_CourierAccount', 'Cancel', 'Confirmation', 'Cancel_Validation', 'FrayteSuccess', 'FrayteWarning']).then(function (translations) {
                $scope.headerTextOtherAddress = translations.Address + " " + translations.Book + " " + translations.DeleteHeader;
                $scope.bodyTextOtherAddress = translations.DeleteBody + " " + translations.Address;
                $scope.headerTextTradeLane = translations.Tradelane + " " + translations.DeleteHeader;
                $scope.bodyTextTradeLane = translations.DeleteBody + " " + translations.Tradelane + " " + translations.detail;

                $scope.TitleFrayteInformation = translations.FrayteSuccess;
                $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;

                $scope.TitleFrayteValidation = translations.FrayteWarning;
                $scope.TextValidation = translations.PleaseCorrectValidationErrors;

                $scope.TitleFrayteError = translations.FrayteError;
                $scope.TextSavingError = translations.ErrorSavingRecord;

                $scope.TextErrorGetting = translations.ErrorGetting + " " + translations.customer + " " + translations.detail;
                $scope.GettingZoneError = translations.ErrorGetting + " " + translations.Zone;
                $scope.TextErrorGettingShipment = translations.ErrorGetting + " " + translations.customer + " " + translations.Shipment_Type;
                $scope.SelectCourierAccount = translations.Select_CourierAccount;
                $scope.CancelConfirmation = translations.Cancel + " " + translations.Confirmation;
                $scope.CancelValidation = translations.Cancel_Validation;
            });
    };

    $scope.GoBack = function () {

        if ($scope.tabs !== undefined && $scope.tabs !== null ) {
            var route = UtilityService.GetCurrentRoute($scope.tabs, "customers");
            $state.go(route);

        }

        //if ($state.is('admin.customer-detail.basic-detail')) {
        //    $state.go('admin.customers', {}, { reload: true });
        //    //$state.go('shipper.current-shipment', {}, { reload: true });
        //}
        //else if ($state.is('dbuser.customer-detail.basic-detail')) {
        //    $state.go('dbuser.customers', {}, { reload: true });
        //}
        //else if ($state.is('customer.manage-detail')) {
        //    $state.go('customer.booking-home.booking-welcome', {}, { reload: true });
        //}

    };
    $scope.Cancel = function () {
        var modalOptions = {
            headerText: $scope.CancelConfirmation,
            bodyText: $scope.CancelValidation
        };

        //$scope.CheckDisable = function (Disable) {
        //    if(Disable === false)
        //    {
        //        return isCollapsed1;
        //    }
        //    //else {
        //    //    $scope.isCollapsed1 = false;
        //    //}
        //};

        ModalService.Confirm({}, modalOptions).then(function () {
            if ($scope.tabs !== undefined && $scope.tabs !== null) {
                var route = UtilityService.GetCurrentRoute($scope.tabs, "customers");
                $state.go(route);

            }
        }, function () {

        });
    };

    //This will hide Address Panel by default.    
    $scope.ShowHidePickupPanel = function () {
        $scope.ShowPickupPanel = !$scope.ShowPickupPanel;
    };
    //Service option hide/show
    $scope.ShowHideServiceOptionPanel = function () {
        $scope.ShowServiceOptionPanel = !$scope.ShowServiceOptionPanel;
    };

    //This will hide Tradelane Panel by default.    
    $scope.ShowHideTradelanePanel = function () {
        $scope.ShowTradelanePanel = !$scope.ShowTradelanePanel;
    };
    $scope.ShowHideMarginPanel = function () {
        $scope.ShowMarginPanel = !$scope.ShowMarginPanel;
    };
    //This will hide Frayteuser Panel by default.    
    $scope.ShowHideFrayteuserPanel = function () {
        $scope.ShowFrayteuserPanel = !$scope.ShowFrayteuserPanel;
    };

    $scope.RearrangeSerialNumbers = function (collectionObject, objectType) {

        if (collectionObject.length > 0) {
            for (var i = 0; i < collectionObject.length; i++) {
                collectionObject[i].SN = i + 1;
            }
        }
        if (objectType == 'PAB') {
            $scope.customerDetail.OtherAddresses = collectionObject;
        }
        else if (objectType == 'TL') {
            $scope.customerDetail.Tradelanes = collectionObject;
        }
    };

    $scope.LoadGridOptions = function () {

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
            //enableVerticalScrollbar: uiGridConstants.scrollbars.NEVER,
            enableVerticalScrollbar: true,
            columnDefs: [
              { name: 'Address', headerCellFilter: 'translate', width: '18%' },
              { name: 'City', headerCellFilter: 'translate', width: '18%' },
              { name: 'State', headerCellFilter: 'translate', width: '18%' },
              { name: 'Zip', headerCellFilter: 'translate', width: '18%' },
              { name: 'Country.Name', displayName: 'Country', headerCellFilter: 'translate', width: '18%' },
              { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "shipper/shipperAddressBook/addressBookEditButton.tpl.html" }
            ]
        };

        $scope.gridOptionsTradelane = {
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
            //enableVerticalScrollbar: uiGridConstants.scrollbars.NEVER,
            enableVerticalScrollbar: true,
            columnDefs: [
                { name: 'OriginatingCountry.Name', displayName: 'Origin_Country', headerCellFilter: 'translate', width: '18%' },
                { name: 'OriginatingAgent.Name', displayName: 'Origin_Agent', headerCellFilter: 'translate', width: '18%' },
                { name: 'DestinationCountry.Name', displayName: 'Destination_Country', headerCellFilter: 'translate', width: '18%' },
                { name: 'DestinationAgent.Name', displayName: 'Destination_Agent', headerCellFilter: 'translate', width: '18%' },
                { name: 'Carrier.CarrierName', displayName: 'Carrier', headerCellFilter: 'translate', width: '18%' },
                //{ name: 'Direct', displayName: 'Direct/Deffered', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.Direct === true ? "Direct" : (row.entity.Deffered === true ? "Deffered" : "") }}</div>' },
                { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "tradelane/tradelaneEditButton.tpl.html" }
            ]
        };
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
                    return $scope.customerDetail.OtherAddresses;
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
            $scope.customerDetail.OtherAddresses = addressBooks;
        });
    };

    $scope.DeleteAddressBooks = function (row) {
        var modalOptions = {
            headerText: $scope.headerTextOtherAddress,
            bodyText: $scope.bodyTextOtherAddress + '?'
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            //Remove the row from customerDetail.Other Address Book collection(array)
            var index = $scope.customerDetail.OtherAddresses.indexOf(row.entity);
            $scope.customerDetail.OtherAddresses.splice(index, 1);
            $scope.RearrangeSerialNumbers($scope.customerDetail.OtherAddresses, 'PAB');
        });
    };
    //End Address Book

    //Start : Trade lane 
    $scope.AddEditTradelane = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'tradelane/tradelaneAddEdit.tpl.html',
            controller: 'TradelaneAddEditController',
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
                countries: function () {
                    return $scope.countries;
                },
                tradelanes: function () {
                    return $scope.customerDetail.Tradelanes;
                },
                tradelane: function () {
                    if (row === undefined) {
                        return {
                            SN: 0,
                            TradelaneId: 0,
                            Route: '',
                            OriginatingAgent: null,
                            OriginatingCountry: null,
                            DestinationAgent: null,
                            DestinationCountry: null,
                            Direct: false,
                            Deffered: false,
                            DirectDefferedType: null,
                            Carrier: null,
                            TransitTime: ''
                        };
                    }
                    else {
                        return row.entity;
                    }
                }
            }
        });

        modalInstance.result.then(function (tradelanes) {
            $scope.customerDetail.Tradelanes = tradelanes;
        });

    };

    $scope.RemoveTradelane = function (row) {
        var modalOptions = {
            headerText: $scope.headerTextTradeLane,
            bodyText: $scope.bodyTextTradeLane + '?'
            //headerText: 'Tradelane delete confirmation',
            //bodyText: 'Are you sure want to delete the tradelane detail?'
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            var index = $scope.customerDetail.Tradelanes.indexOf(row.entity);
            $scope.customerDetail.Tradelanes.splice(index, 1);
            $scope.RearrangeSerialNumbers($scope.customerDetail.Tradelanes, 'TL');
        });
    };
    //End : Trade lane

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
    //            $scope.customerDetail.TelephoneNo = telephoneNumber.replace(code, "").trim();

    //        }
    //        else if (userType === 'MobileNo') {
    //            $scope.customerDetail.MobileNo = telephoneNumber.replace(code, "").trim();
    //        }
    //        else if (userType === 'Fax') {
    //            $scope.customerDetail.FaxNumber = telephoneNumber.replace(code, "").trim();
    //        }

    //        var countryCode = telephoneNumber.substring(2, n);
    //        var objects = $scope.countryPhoneCodes;

    //        for (var i = 0; i < objects.length; i++) {
    //            if (objects[i].PhoneCode === countryCode) {
    //                if (userType === 'TelephoneNo') {
    //                    $scope.customerDetail.TelephoneCode = objects[i];
    //                    $scope.TelephonePhoneCode = objects[i];
    //                }
    //                else if (userType === 'MobileNo') {
    //                    $scope.customerDetail.MobileCode = objects[i];

    //                }
    //                else if (userType === 'Fax') {

    //                    $scope.customerDetail.FaxCode = objects[i];

    //                }
    //                break;
    //            }
    //        }
    //    }
    //    else {
    //        if (userType === 'Fax') {
    //            $scope.customerDetail.FaxCode = $scope.TelephonePhoneCode;
    //        }
    //        else if (userType === 'MobileNo') {
    //            $scope.customerDetail.MobileCode = $scope.TelephonePhoneCode;
    //        }
    //    }
    //};

    $scope.SetCurrencyCode = function (customerDetail) {
        var objects = $scope.currency;

        if (objects !== undefined && objects !== null) {
            for (var i = 0; i < objects.length; i++) {
                if (objects[i].CurrencyCode === customerDetail.CreditLimitCurrencyCode) {
                    $scope.customerDetail.DeclaredCurrency = objects[i];
                    break;
                }
            }
        }
    };

    // SetCustomerDetailSetting
    $scope.SetCustomerSettingDetail = function (Courier) {
        if (Courier.IsSelected) {
            if ($scope.customerDetail.CustomerRateSetting.CustomerSettingDetail.length === 0) {
                $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail.push({ CustomerSettingDetailId: 0, CustomerSettingId: 0, CourierShipment: Courier });
            }
            else {
                var flag = false;
                for (var i = 0; i < $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail.length; i++) {
                    if ($scope.customerDetail.CustomerRateSetting.CustomerSettingDetail[i].CourierShipment !== null && $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail[i].CourierShipment.CourierId === Courier.CourierId) {
                        flag = true;
                        break;
                    }
                }
                if (!flag) {
                    $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail.push({ CustomerSettingDetailId: 0, CustomerSettingId: 0, CourierShipment: Courier });
                }
            }
        }
        else {
            if ($scope.customerDetail.CustomerRateSetting.CustomerSettingDetail !== null && $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail.length > 0) {
                for (var j = 0; j < $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail.length; j++) {
                    if ($scope.customerDetail.CustomerRateSetting.CustomerSettingDetail[j].CourierShipment !== null && $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail[j].CourierShipment.CourierId === Courier.CourierId) {
                        $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail.splice(j, 1);
                        break;
                    }
                }
            }

        }
    };
    $scope.GetCustomerDetailsInitials = function () {
        CustomerService.GetZoneDetail().then(function (response) {
            $scope.Zones = response.data;
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.GettingZoneError,
                showCloseButton: true
            });
        });


        //CustomerService.GetShipmentType().then(function (response) {
        //    $scope.ShipmentTypes = response.data;
        //}, function () {
        //    toaster.pop({
        //        type: 'error',
        //        title: $scope.TitleFrayteError,
        //        body: $scope.TextErrorGettingShipment,
        //        showCloseButton: true
        //    });
        //});

        $scope.CurrencyTypes = [];
        ShipmentService.GetInitials().then(function (response) {

            $scope.WorkingWeekDays = response.data.WorkingWeekDays;
            $scope.countries = response.data.Countries;
            $scope.timezones = response.data.TimeZones;
            $scope.Couriers = response.data.Couriers;
            $scope.countryPhoneCodes = response.data.CountryPhoneCodes;
            var curr = response.data.CurrencyTypes;
            // Set countries
            $scope.Countries = TopCountryService.TopCountryList($scope.countries);
            // Set Currency type 
            $scope.currency = TopCurrencyService.TopCurrencyList(curr);
            $scope.OperationZone = response.data.OperationZone;
            if ($scope.customerId > 0) {
                AppSpinner.showSpinnerTemplate($scope.spinnerMessage, $scope.Template);
                CustomerService.GetCustomerDetail($scope.customerId).then(function (response) {
                    //Step 1: Set Basic Detail and Main Address
                    response.data.CreditLimit = Number(parseFloat(response.data.CreditLimit).toFixed(2)).toLocaleString('en', { minimumFractionDigits: 2 });
                    $scope.customerDetail = response.data;
                    $scope.customerDetail.UpdatedBy = $scope.CreatedByInfo.EmployeeId;
                    $scope.customerDetail.CreatedByRoleId = $scope.CreatedByInfo.RoleId;
                    $scope.SetPhoneCodeInfo($scope.customerDetail.UserAddress.Country);
                    customerPODSettingVisibility();
                    if ($scope.customerDetail.CustomerPODSetting.ScheduleSetting === "Scheduled") {
                        scheduleDayVisibilty();
                    }

                    //if ($scope.customerDetail.CustomerRateSetting !== null && $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail !== null && $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail.length > 0) {
                    //    for (var d = 0; d < $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail.length; d++) {
                    //        for (var e = 0; e < $scope.Couriers.length; e++) {
                    //            if ($scope.customerDetail.CustomerRateSetting.CustomerSettingDetail[d].CourierShipment.CourierId === $scope.Couriers[e].CourierId) {
                    //                $scope.Couriers[e].IsSelected = true;
                    //            }
                    //        }
                    //    }
                    //}
                    //  customerRateSettingVisibility();
                    //if ($scope.customerDetail.CustomerRateSetting.ScheduleSetting === "Scheduled") {
                    //    scheduleDayRateVisibilty();
                    //    $scope.ShowCustomerSettingDetail = true;
                    //}
                    //else {
                    //    $scope.ShowCustomerSettingDetail = false;
                    //}
                    //Step 2: Set Address book grid
                    $scope.RearrangeSerialNumbers($scope.customerDetail.OtherAddresses, 'PAB');
                    $scope.gridOptionsAddressBook.data = $scope.customerDetail.OtherAddresses;

                    //Step 3: Set Trade lane grid
                    $scope.RearrangeSerialNumbers($scope.customerDetail.Tradelanes, 'TL');
                    $scope.gridOptionsTradelane.data = $scope.customerDetail.Tradelanes;


                    //$scope.SetPhoneCodeAndNumber('TelephoneNo', $scope.customerDetail.TelephoneNo);
                    //$scope.SetPhoneCodeAndNumber('MobileNo', $scope.customerDetail.MobileNo);
                    //$scope.SetPhoneCodeAndNumber('Fax', $scope.customerDetail.FaxNumber);

                    // Step 4: Set MarginCost For Customer
                    // $scope.setMarginCostjson();

                    // Set PODDate ForMat
                    if (response.data.CustomerPODSetting !== null && response.data.CustomerPODSetting.ScheduleDate !== null) {
                        var PODDate = moment.utc(response.data.CustomerPODSetting.ScheduleDate).toDate();
                        $scope.customerDetail.CustomerPODSetting.ScheduleDate = PODDate;
                    }
                    // Set CustomerRateDate ForMat
                    if (response.data.CustomerRateSetting !== null && response.data.CustomerRateSetting.ScheduleDate !== null) {
                        var RateDate = moment.utc(response.data.CustomerRateSetting.ScheduleDate).toDate();
                        $scope.customerDetail.CustomerRateSetting.ScheduleDate = RateDate;
                    }

                    // Set State and Zip for "HKG" and "GBR"
                    if ($scope.customerDetail && $scope.customerDetail.UserAddress != null && $scope.customerDetail.UserAddress.Country.Code === "HKG") {
                        $scope.setStateDisable = true;
                        $scope.setZipDisable = true;
                        $scope.customerDetail.UserAddress.Zip = null;
                        $scope.customerDetail.UserAddress.State = null;
                    }
                    if ($scope.customerDetail && $scope.customerDetail.UserAddress != null && $scope.customerDetail.UserAddress.Country.Code === "GBR") {
                        $scope.setStateDisable = true;
                        $scope.customerDetail.UserAddress.State = null;
                    }

                    $scope.SetCurrencyCode($scope.customerDetail);
                    AppSpinner.hideSpinnerTemplate();
                }, function () {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorGetting,
                        //title: 'Frayte-Error',
                        //body: 'Error while getting customer detail',
                        showCloseButton: true
                    });
                });
            }
            else {
                //Step 1: Set Basic Detail and Main Address

                $scope.NewCustomer();
                for (var a = 0 ; a < $scope.Couriers.length; a++) {
                    $scope.Couriers[a].IsSelected = false;
                }
                if ($scope.OperationZoneId = 1) {
                    for (var c = 0 ; c < $scope.countries.length; c++) {
                        if ($scope.countries[c].Code == "HKG") {
                            $scope.SetPhoneCodeInfo($scope.countries[c]);
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
                //customerMarginCostArray();
                //$scope.setMarginCostjson();
                customerPODSettingVisibility();

                if ($scope.customerDetail.CustomerPODSetting.ScheduleSetting === "Scheduled") {
                    scheduleDayVisibilty();
                }
                //       customerRateSettingVisibility();

                if ($scope.customerDetail.CustomerPODSetting.ScheduleSetting === "Scheduled") {
                    scheduleDayRateVisibilty();
                }

                // Set Default WorkingWeek Day
                if ($scope.WorkingWeekDays !== null && $scope.WorkingWeekDays !== undefined && $scope.WorkingWeekDays.length > 0) {
                    var weekDays = $scope.WorkingWeekDays;
                    for (var n = 0; n < weekDays.length; n++) {
                        if (weekDays[n].IsDefault) {
                            $scope.customerDetail.WorkingWeekDay = weekDays[n];
                            break;
                        }
                    }
                }
                //Step 3: Set Address book grid
                $scope.gridOptionsAddressBook.data = $scope.customerDetail.OtherAddresses;
                //Step 4: Set Trade lane grid
                $scope.gridOptionsTradelane.data = $scope.customerDetail.Tradelanes;
            }
        });
    };


    // Customer POD Setting 
    $scope.changePODSheduleDaySetting = function (ScheduleDay) {
        if (ScheduleDay !== undefined && ScheduleDay !== null && ScheduleDay !== '') {
            if ($scope.customerDetail.CustomerPODSetting !== null && $scope.customerDetail.CustomerPODSetting.ScheduleSetting === "Scheduled" && $scope.customerDetail.CustomerPODSetting.ScheduleType === 'Weekly') {
                $scope.WeekDayVisibility = true;
            }
            else {
                $scope.WeekDayVisibility = false;
            }
        }
    };
    $scope.changePODSheduleSetting = function (SheduleSetting) {
        if (SheduleSetting !== undefined && SheduleSetting !== null && SheduleSetting !== '') {

            customerPODSettingVisibility();
            if ($scope.customerDetail.CustomerPODSetting !== null && $scope.customerDetail.CustomerPODSetting.ScheduleSetting === "Scheduled") {
                scheduleDayVisibilty();
                $scope.customerDetail.CustomerPODSetting.ScheduleType = 'Daily';
                $scope.customerDetail.CustomerPODSetting.ScheduleDay = '';
                $scope.customerDetail.CustomerPODSetting.ScheduleDate = new Date();
                $scope.customerDetail.CustomerPODSetting.ScheduleTime = '';
                $scope.customerDetail.CustomerPODSetting.AdditionalMails = '';
                if ($scope.customerDetail.CustomerPODSetting !== null && $scope.customerDetail.CustomerPODSetting.ScheduleType === 'Weekly') {
                    $scope.WeekDayVisibility = true;
                }
                else {
                    $scope.WeekDayVisibility = false;
                }
            }
            else {
                $scope.customerDetail.CustomerPODSetting.ScheduleType = '';
                $scope.customerDetail.CustomerPODSetting.ScheduleDay = '';
                $scope.customerDetail.CustomerPODSetting.ScheduleDate = null;
                $scope.customerDetail.CustomerPODSetting.ScheduleTime = '';
                $scope.customerDetail.CustomerPODSetting.AdditionalMails = '';

            }
        }
    };
    var customerPODSettingVisibility = function () {
        if ($scope.customerDetail.CustomerPODSetting !== null !== null && $scope.customerDetail.CustomerPODSetting.ScheduleSetting === "Scheduled") {
            $scope.PODSettingVisibility = true;
        }
        else {
            $scope.PODSettingVisibility = false;
        }
    };

    var scheduleDayVisibilty = function () {
        if ($scope.customerDetail.CustomerPODSetting !== null && $scope.customerDetail.CustomerPODSetting.ScheduleType === "Daily" || $scope.customerDetail.CustomerPODSetting.ScheduleType === "Monthly" || $scope.customerDetail.CustomerPODSetting.ScheduleType === "Yearly") {
            $scope.WeekDayVisibility = false;
        }
        else {
            $scope.WeekDayVisibility = true;
        }
    };

    // Customer rate Card
    $scope.changeRateSheduleDaySetting = function (ScheduleDay) {
        if (ScheduleDay !== undefined && ScheduleDay !== null && ScheduleDay !== '') {
            if ($scope.customerDetail.CustomerRateSetting !== null && $scope.customerDetail.CustomerRateSetting.ScheduleSetting === "Scheduled" && $scope.customerDetail.CustomerRateSetting.ScheduleType === 'Weekly') {
                $scope.RateWeekDayVisibility = true;
            }
            else {
                $scope.RateWeekDayVisibility = false;
            }
        }
    };
    $scope.changeRateSheduleSetting = function (SheduleSetting) {
        if (SheduleSetting !== undefined && SheduleSetting !== null && SheduleSetting !== '') {

            //  customerRateSettingVisibility();
            if ($scope.customerDetail.CustomerRateSetting !== null && $scope.customerDetail.CustomerRateSetting.ScheduleSetting === "Scheduled") {
                scheduleDayRateVisibilty();
                $scope.customerDetail.CustomerRateSetting.ScheduleType = 'Daily';
                $scope.customerDetail.CustomerRateSetting.ScheduleDay = '';
                $scope.customerDetail.CustomerRateSetting.ScheduleDate = new Date();
                $scope.customerDetail.CustomerRateSetting.ScheduleTime = '';
                $scope.customerDetail.CustomerRateSetting.AdditionalMails = '';
                if ($scope.customerDetail.CustomerRateSetting !== null && $scope.customerDetail.CustomerRateSetting.ScheduleType === 'Weekly') {
                    $scope.RateWeekDayVisibility = true;
                }
                else {
                    $scope.RateWeekDayVisibility = false;
                }
                $scope.ShowCustomerSettingDetail = true;
            }
            else {
                $scope.customerDetail.CustomerRateSetting.ScheduleType = '';
                $scope.customerDetail.CustomerRateSetting.ScheduleDay = '';
                $scope.customerDetail.CustomerRateSetting.ScheduleDate = null;
                $scope.customerDetail.CustomerRateSetting.ScheduleTime = '';
                $scope.customerDetail.CustomerRateSetting.AdditionalMails = '';
                $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail = [];
                $scope.ShowCustomerSettingDetail = false;
                for (var a = 0 ; a < $scope.Couriers.length; a++) {
                    $scope.Couriers[a].IsSelected = false;
                }


            }
        }
    };
    var customerRateSettingVisibility = function () {
        if ($scope.customerDetail.CustomerRateSetting !== null !== null && $scope.customerDetail.CustomerRateSetting.ScheduleSetting === "Scheduled") {
            $scope.RateSettingVisibility = true;
        }
        else {
            $scope.RateSettingVisibility = false;
        }
    };

    var scheduleDayRateVisibilty = function () {
        if ($scope.customerDetail.CustomerRateSetting !== null && $scope.customerDetail.CustomerRateSetting.ScheduleType === "Daily" || $scope.customerDetail.CustomerRateSetting.ScheduleType === "Monthly" || $scope.customerDetail.CustomerRateSetting.ScheduleType === "Yearly") {
            $scope.RateWeekDayVisibility = false;
        }
        else {
            $scope.RateWeekDayVisibility = true;
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
    $scope.customerOptionalField = function (action) {
        if (action !== undefined && action !== null && action !== '' && action === 'State') {
            if ($scope.customerDetail !== undefined && $scope.customerDetail !== null && $scope.customerDetail.UserAddress !== null && $scope.customerDetail.UserAddress.Country !== null) {
                if ($scope.customerDetail.UserAddress.Country.Code !== 'HKG' && $scope.customerDetail.UserAddress.Country.Code !== 'GBR') {
                    return true;
                }
                else {
                    // $scope.directBooking.ShipFrom.State = '';
                    return false;
                }
            }
        }
        else if (action !== undefined && action !== null && action !== '' && action === 'PostCode') {
            if ($scope.customerDetail !== undefined && $scope.customerDetail !== null && $scope.customerDetail.UserAddress !== null && $scope.customerDetail.UserAddress.Country !== null) {
                if ($scope.customerDetail.UserAddress.Country.Code !== 'HKG') {
                    return true;
                }
                else {
                    //  $scope.directBooking.ShipFrom.PostCode = '';
                    return false;
                }
            }
        }

    };
    $scope.NewCustomer = function () {
        $scope.customerDetail = {
            AccountNumber: '',
            AccountName: '',
            AccountMail: '',
            CreditLimit: '',
            CreditLimitCurrencyCode: '',
            TermsOfPayment: '',
            TaxAndDuties: '',
            AccountUser: {
                UserId: 0,
                AssociateType: '',
                ContactName: '',
                Email: '',
                TelephoneNo: '',
                WorkingHours: ''
            },
            DocumentUser: {
                UserId: 0,
                AssociateType: '',
                ContactName: '',
                Email: '',
                TelephoneNo: '',
                WorkingHours: ''
            },
            ManagerUser: {
                UserId: 0,
                AssociateType: '',
                ContactName: '',
                Email: '',
                TelephoneNo: '',
                WorkingHours: ''
            },
            OperationUser: {
                UserId: 0,
                AssociateType: '',
                ContactName: '',
                Email: '',
                TelephoneNo: '',
                WorkingHours: ''
            },
            SalesRepresentative: {
                UserId: 0,
                AssociateType: '',
                ContactName: '',
                Email: '',
                TelephoneNo: '',
                WorkingHours: ''
            },
            OtherAddresses: [],
            Tradelanes: [],
            UserId: 0,
            CargoWiseId: '',
            CargoWiseBardCode: '',
            CompanyName: '',
            ClientId: 0,
            IsClient: true,
            CountryOfOperation: '',
            ContactName: '',
            Email: '',
            TelephoneCode: '',
            TelephoneNo: '',
            MobileNo: '',
            FaxCode: '',
            FaxNumber: '',
            WorkingStartTime: setWorkingStartTime(),
            WorkingEndTime: setWorkingEndTime(),
            Timezone: null,
            WorkingWeekDay: null,
            VATGST: '',
            ShortName: '',
            Position: '',
            Skype: '',
            CreatedOn: '',
            CreatedBy: $scope.CreatedByInfo.EmployeeId,
            CreatedByRoleId: $scope.CreatedByInfo.RoleId,
            UpdatedOn: '',
            UpdatedBy: $scope.CreatedByInfo.EmployeeId,
            UserAddress: {
                UserAddressId: 0,
                UserId: 0,
                AddressTypeId: 0,
                Address: '',
                Address2: '',
                Address3: '',
                Suburb: '',
                City: '',
                State: '',
                Zip: '',
                Country: null
            },
            RoleId: 0,
            IsDirectBooking: false,
            IsTradeLaneBooking: false,
            IsBreakBulkBooking: false,
            IsShipperTaxAndDuty: false,
            IsAllowRate: false,
            IsECommerce: false,
            CustomerMargin: [],
            CustomerPODSetting: {
                CustomererSettingId: 0,
                ScheduleSetting: 'PerShipment',
                ScheduleType: 'Daily',
                ScheduleDay: '',
                ScheduleDate: new Date(),
                ScheduleTime: '',
                AdditionalMails: '',
                ScheduleSettingType: 'POD'
            },
            CustomerRateSetting: {
                CustomererSettingId: 0,
                ScheduleSetting: 'Scheduled',
                ScheduleType: 'Daily',
                ScheduleDay: '',
                ScheduleDate: new Date(),
                ScheduleTime: '',
                AdditionalMails: '',
                ScheduleSettingType: 'RateCard',
                IsExcel: false,
                IsPdf: true,
                CustomerSettingDetail: []
            }
        };
    };




    $scope.openCalender = function ($event) {
        $scope.status.opened = true;
    };
    $scope.status = {
        opened: false
    };
    $scope.openCalender1 = function ($event) {
        $scope.status1.opened = true;
    };
    $scope.status1 = {
        opened: false
    };
    $scope.SetWeekDay = function (item) {
    };
    $scope.SaveCustomerDetail = function (isValid, customerDetail) {

        if (isValid) {

            if (customerDetail.DeclaredCurrency !== undefined && customerDetail.DeclaredCurrency !== '') {
                customerDetail.CreditLimitCurrencyCode = customerDetail.DeclaredCurrency.CurrencyCode;
            }

            //if (customerDetail.TelephoneNo !== undefined && customerDetail.TelephoneNo !== '') {
            //    customerDetail.TelephoneNo = '(+' + $scope.customerDetail.TelephoneCode.PhoneCode + ') ' + customerDetail.TelephoneNo;
            //}

            //if (customerDetail.MobileNo !== undefined && customerDetail.MobileNo !== '') {
            //    customerDetail.MobileNo = '(+' + $scope.customerDetail.MobileCode.PhoneCode + ') ' + customerDetail.MobileNo;
            //}

            //if (customerDetail.FaxNumber !== undefined && customerDetail.FaxNumber !== '' && customerDetail.FaxNumber !== null) {
            //    customerDetail.FaxNumber = '(+' + $scope.customerDetail.FaxCode.PhoneCode + ') ' + customerDetail.FaxNumber;
            //}

            customerDetail.WorkingStartTime = moment(customerDetail.WorkingStartTime).format();
            customerDetail.WorkingEndTime = moment(customerDetail.WorkingEndTime).format();

            AppSpinner.showSpinnerTemplate("Saving Customer Detail", $scope.Template);
            CustomerService.SaveCustomer(customerDetail).then(function (response) {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteInformation,
                    body: $scope.TextSuccessfullySavedInformation,
                    //title: 'Frayte-Information',
                    //body: 'Successfully saved information',
                    showCloseButton: true
                });
                $scope.GoBack();
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextSavingError,
                    //title: 'Frayte-Error',
                    //body: 'Error while saving the reocrd',
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextValidation,
                //title: 'Frayte-Validation',
                //body: 'Please correct the validation errors first.',
                showCloseButton: true
            });
        }
    };

    //Set initail Detail Of Country 
    $scope.InitailDetailOfCountry = function (data, showData, countries) {
        if (!data) {
            $scope.customerDetail.FaxCode = showData;
            $scope.customerDetail.TelephoneCode = showData;
            for (var i = 0; i < countries.length; i++) {
                if (countries[i].Name == showData.Name) {
                    $scope.customerDetail.UserAddress.Country = countries[i];
                    break;
                }
            }
            $scope.data = true;
        }
    };
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
    $scope.InitailDetailOfCountryCodes = function (data, countryPhoneFaxCode, country) {


        $scope.SetPhoneCodeInfo(country);
        // Set Customer State and zip
        if (country.Code !== null && country.Code !== '' && country.Code !== undefined) {
            if (country.Code === "HKG") {
                $scope.setStateDisable = true;
                $scope.setZipDisable = true;
                $scope.customerDetail.UserAddress.Zip = null;
                $scope.customerDetail.UserAddress.State = null;
            }
            else if (country.Code === "GBR") {
                $scope.setStateDisable = true;
                $scope.setZipDisable = false;
                $scope.customerDetail.UserAddress.State = null;
            }
            else {
                $scope.setZipDisable = false;
                $scope.setStateDisable = false;
            }
        }

        if (!data) {
            for (var i = 0; i < countryPhoneFaxCode.length; i++) {
                if (countryPhoneFaxCode[i].Name == country.Name) {
                    $scope.customerDetail.FaxCode = countryPhoneFaxCode[i];
                    $scope.customerDetail.TelephoneCode = countryPhoneFaxCode[i];
                    break;
                }
            }
            $scope.data = true;
        }
    };
    $scope.toggleRowSelection = function () {
        $scope.gridApi.selection.clearSelectedRows();
        $scope.gridOptions.enableRowSelection = !$scope.gridOptions.enableRowSelection;
        $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS);
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
        $scope.spinnerMessage = 'Loading Customer Detail';
        $rootScope.GetServiceValue = null;
        var sessionInfo = SessionService.getUser();
         
        $scope.tabs = sessionInfo.tabs; 
        if (sessionInfo !== undefined && sessionInfo !== null) {
            $scope.CreatedByInfo = sessionInfo;
        }
        // For Customer rate  Card Sretting
        $scope.ShowCustomerSettingDetail = true;

        $scope.FinalCustomerMargin = [];
        //Terms of Payments
        $scope.TermsOfPayments = [
            { name: '7 Days' },
            { name: '14 Days' },
            { name: '21 Days' },
            { name: '30 Days' },
            { name: '60 Days' },
            { name: 'Pre-Payment' },
            { name: 'Terms of the payment for Freight' }
        ];
        $scope.weekDaysList = [
            { Name: 'Sunday' },
            { Name: 'Monday' },
            { Name: 'Tuesday' },
            { Name: 'Wednesday' },
            { Name: 'Thursday' },
            { Name: 'Friday' },
            { Name: 'Starday' }
        ];
        $scope.ScheduleTypes = [
            { Name: 'Daily' },
            { Name: 'Weekly' },
            { Name: 'Monthly' },
            { Name: 'Yearly' }
        ];
        $scope.ScheduleSettings = [
           { Name: 'PerShipment', DisplayName: 'POD Per Shipment' },
           { Name: 'Scheduled', DisplayName: 'Consolidate POD Report' }
        ];
        $scope.LoadGridOptions();
        $scope.gridOptionsAddressBook.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
        };
        $scope.gridOptionsTradelane.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
        };

        $scope.customerId = $stateParams.customerId;

        if ($state.is('admin.customer-detail.basic-detail')) {

            if ($stateParams.customerId === undefined) {
                $scope.customerId = "0";
            }
            else {
                var userInfo1 = SessionService.getUser();
                $scope.OperationZoneId = userInfo1.OperationZoneId;
                $scope.OperationZoneName = userInfo1.OperationZoneName;
                $scope.customerId = $stateParams.customerId;
            }
        }
        else if ($state.is('customer.manage-detail')) {
            $scope.UserInfo = SessionService.getUser();

            if ($scope.UserInfo === undefined || $scope.UserInfo === null || $scope.UserInfo.SessionId === undefined || $scope.UserInfo.SessionId === '') {
                $state.go('customer.current-shipment');
            }
            else {
                $scope.customerId = $scope.UserInfo.EmployeeId;
            }
        }

        // Get shipping methods of type courier


        //getting timezones, country and  their code
        $scope.GetCustomerDetailsInitials();

        // set Multilingual Modal Popup Options
        setModalOptions();

        //hide Panels
        $scope.ShowPickupPanel = false;
        $scope.ShowTradelanePanel = false;
        $scope.ShowFrayteuserPanel = false;
        $scope.data = false;
        $scope.RateSettingPanel = false;
        $scope.PODSetting = false;


    }

    init();

});