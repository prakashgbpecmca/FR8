﻿angular.module('ngApp.breakBulk').controller("breakbulkBookingController", function ($scope, $uibModal, ModalService, ExpressBookingService, TopCountryService, $state, $stateParams, config, $rootScope, toaster, $translate, SessionService, BreakBulkService) {

    $scope.breakbulk = 'BreakBulk';

    $scope.breakbulkCollapse = function () {
        $scope.breakbulkShow = !$scope.breakbulkShow;
    };

    $scope.addressCollapse = function () {
        $scope.addressShow = !$scope.addressShow;
    };

    //date code
    $scope.today = function () {
        $scope.dt = new Date();
    };
    $scope.today();

    $scope.clear = function () {
        $scope.dt = null;
    };

    $scope.inlineOptions = {
        customClass: getDayClass,
        minDate: new Date(),
        showWeeks: true
    };

    $scope.dateOptions = {
        dateDisabled: disabled,
        formatYear: 'yy',
        maxDate: new Date(2020, 5, 22),
        minDate: new Date(),
        startingDay: 1
    };

    // Disable weekend selection
    function disabled(data) {
        var date = data.date,
            mode = data.mode;
        return mode === 'day' && (date.getDay() === 0 || date.getDay() === 6);
    }

    $scope.toggleMin = function () {
        $scope.inlineOptions.minDate = $scope.inlineOptions.minDate ? null : new Date();
        $scope.dateOptions.minDate = $scope.inlineOptions.minDate;
    };

    $scope.toggleMin();

    $scope.open1 = function () {
        $scope.popup1.opened = true;
    };

    $scope.open2 = function () {
        $scope.popup2.opened = true;
    };

    $scope.setDate = function (year, month, day) {
        $scope.dt = new Date(year, month, day);
    };

    $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
    $scope.format = $scope.formats[0];
    $scope.altInputFormats = ['M!/d!/yyyy'];

    $scope.popup1 = {
        opened: false
    };

    $scope.popup2 = {
        opened: false
    };

    var tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    var afterTomorrow = new Date();
    afterTomorrow.setDate(tomorrow.getDate() + 1);
    $scope.events = [
        {
            date: tomorrow,
            status: 'full'
        },
        {
            date: afterTomorrow,
            status: 'partially'
        }
    ];

    function getDayClass(data) {
        var date = data.date,
            mode = data.mode;
        if (mode === 'day') {
            var dayToCheck = new Date(date).setHours(0, 0, 0, 0);

            for (var i = 0; i < $scope.events.length; i++) {
                var currentDay = new Date($scope.events[i].date).setHours(0, 0, 0, 0);

                if (dayToCheck === currentDay) {
                    return $scope.events[i].status;
                }
            }
        }

        return '';
    }
    //end

    //breakbulk product catalog code
    $scope.productCatalog = function () {
        if ($scope.PaymentParty !== null && $scope.PaymentParty !== undefined && $scope.PaymentParty !== '' && $scope.PaymentParty.CustomerId > 0 &&
            $scope.BBKBooking.ShipTo.Country !== null && $scope.BBKBooking.ShipTo.Country !== undefined &&
            $scope.BBKBooking.ShipTo.Country !== '' && $scope.BBKBooking.ShipTo.Country.CountryId > 0) {
            ModalInstance = $uibModal.open({
                Animation: true,
                controller: 'ProductCatalogAddEditController',
                templateUrl: 'breakbulk/productCatalog/productCatalogAddEdit.tpl.html',
                keyboard: true,
                windowClass: 'CustomerAddress-Edit',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    CustomerId: function () {
                        return $scope.PaymentParty.CustomerId;
                    },
                    HubDetail: function () {
                        return $scope.hubAddress;
                    },
                    ProductcatalogId: function () {
                        return 0;
                    }
                }
            });
        }
        else {
            var ModalInstance = $uibModal.open({
                Animation: true,
                controller: 'ProductCatalogValidationController',
                templateUrl: 'breakbulk/productCatalog/productCatalogValidation.tpl.html',
                keyboard: true,
                size: 'lg',
                backdrop: 'static'
            });
        }
    };
    //end

    //breakbulk map product catalog code
    $scope.mapProductCatalog = function (index) {
        if ($scope.PaymentParty !== null && $scope.PaymentParty !== undefined && $scope.PaymentParty !== '' && $scope.PaymentParty.CustomerId > 0) {
            if ($scope.hubAddress !== null && $scope.hubAddress !== undefined && $scope.hubAddress !== '' && $scope.hubAddress.HubId > 0) {
                var ModalInstance = $uibModal.open({
                    Animation: true,
                    controller: 'BreakbulkMapAddEditController',
                    templateUrl: 'breakbulk/details/breakbulkMapAddEdit.tpl.html',
                    keyboard: true,
                    windowClass: 'CustomerAddress-Edit',
                    size: 'lg',
                    backdrop: 'static',
                    resolve: {
                        CustomerId: function () {
                            return $scope.PaymentParty.CustomerId;
                        },
                        ModuleType: function () {
                            return 'BreakbulkBooking';
                        },
                        HubDetail: function () {
                            return $scope.hubAddress;
                        }
                    }
                });
                ModalInstance.result.then(function (ProductCatalog) {
                    if (ProductCatalog !== undefined && ProductCatalog !== null && ProductCatalog !== '') {
                        //$scope.expressBooking.Packages[index].Length = ProductCatalog.Length;
                        //$scope.expressBooking.Packages[index].Width = ProductCatalog.Width;
                        //$scope.expressBooking.Packages[index].Height = ProductCatalog.Height;
                        //$scope.expressBooking.Packages[index].Weight = ProductCatalog.Weight;
                        //$scope.expressBooking.Packages[index].Value = ProductCatalog.DeclaredValue;
                        $scope.BBKBooking.PurchaseOrderDetail[index].Description = ProductCatalog.ProductDescription;
                        $scope.BBKBooking.PurchaseOrderDetail[index].ProductCatalogId = ProductCatalog.ProductcatalogId;
                        //$scope.getTotalKgs();
                        //$scope.setActualWeight();
                    }
                }, function () {
                });
            }
            else {
                var ModalInstancevalid = $uibModal.open({
                    Animation: true,
                    controller: 'ProductCatalogValidationController',
                    templateUrl: 'breakbulk/productCatalog/productCatalogValidation.tpl.html',
                    keyboard: true,
                    size: 'lg',
                    backdrop: 'static'
                });
            }
        }
        else {
            var ModalInstancevaliddate = $uibModal.open({
                Animation: true,
                controller: 'ProductCatalogValidationController',
                templateUrl: 'breakbulk/productCatalog/productCatalogValidation.tpl.html',
                keyboard: true,
                size: 'lg',
                backdrop: 'static'
            });
        }
    };
    //end

    // address book image code
    $scope.photoUrl = config.BUILD_URL + "addressBook.png";
    $scope.photoHazard = config.BUILD_URL + "Hazard_logo.png";
    //end

    //add-edit purchase order code
    $scope.addEditPurchaseOrder = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'breakbulkBookingAddEditPurchaseOrderController',
            templateUrl: 'breakbulk/breakbulkPurchaseOrder/breakbulkBookingAddEditPurchaseOrder.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end

    //delete purchase order code
    $scope.deletePurchaseOrder = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            //controller: 'customerAddressBookAddEditController',
            templateUrl: 'breakbulk/breakbulkPurchaseOrder/deletePurchaseOrder.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'md',
            backdrop: 'static'
        });
    };
    //end

    //breakbulkBookingCustomers code here
    $scope.breakbulkBookingCustomers = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'selectCustomersController',
            templateUrl: 'breakbulk/selectCustomers/selectCustomers.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end

    //breakbulk shipment detail code here
    $scope.breakbulkShipmentDetail = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'breakbulkDetailController',
            templateUrl: 'breakbulk/details/breakbulkShipmentDetail.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg'
        });
    };
    //end

    //breakbulk get service order code
    $scope.breakbulkGetService = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'BreakbulkGetserviceController',
            templateUrl: 'breakbulk/breakbulkGetservice/breakbulkGetservice.tpl.html',
            keyboard: true,
            windowClass: '',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end

    //breakbulk address book code
    $scope.breakbulkAddress = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'BreakbulkAddressBookController',
            templateUrl: 'breakbulk/details/breakbulkAddressBook.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end

    //breakbulk custom field code
    $scope.customField = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'BreakbulkBookingCustomFieldController',
            templateUrl: 'breakbulk/breakbulkBooking/breakbulkBookingCustomField.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'md',
            backdrop: 'static'
        });
        ModalInstance.result.then(function (res) {

          if (res !== null && res.customField1 === undefined && res !== '') {
                $scope.custm0 = {
                    ModuleType: "BreakBulk",
                    CustomFieldId: 0,
                    CustomFieldValue: $scope.customName1bbk = "",
                    CustomFieldName: $scope.BBKBooking.CustomField1.CustomFieldName = res.customField1Name,
                    CustomFieldType: $scope.BBKBooking.CustomField1.CustomFieldType = "Text",
                    UserId: $scope.UserId = $scope.BBKBooking.CustomerId
                };
                //$scope.BBKBooking.CustomField1.CustomFieldName =  res.customField1Name;
                //$scope.BBKBooking.CustomField1.CustomFieldValue = "";
                //$scope.BBKBooking.CustomField1.CustomFieldType = "Text";
                //$scope.UserId = $scope.BBKBooking.CustomerId;
            }
            else {
                  $scope.customName1bbk = res.customField1.split(',');
              $scope.custm0 = {
                    ModuleType: "BreakBulk",
                    CustomFieldId: 0,
                    CustomFieldValue: $scope.BBKBooking.CustomField1.CustomFieldValue = res.customField1,
                    CustomFieldName: $scope.BBKBooking.CustomField1.CustomFieldName = res.customField1Name,
                    CustomFieldType: $scope.BBKBooking.CustomField1.CustomFieldType = "Dropdown",
                    UserId: $scope.UserId = $scope.BBKBooking.CustomerId
                };
                //$scope.customName1bbk = res.customField1.split(',');
                //$scope.BBKBooking.CustomField1.CustomFieldName = res.customField1Name;
                //$scope.BBKBooking.CustomField1.CustomFieldType = "Dropdown";
                //$scope.BBKBooking.CustomField1.CustomFieldValue = res.customField1;
                //$scope.UserId = $scope.BBKBooking.CustomerId;
            }

            BreakBulkService.SaveCustomerCustomField($scope.custm0).then(function (response) {
                if (response.data !== undefined) {

                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteSuccess,
                        body: $scope.BookingSaveValidation,
                        showCloseButton: true
                    });

                }
            });
        });

    };

    $scope.CustomF1Change = function (customname1bbk) {
        if (customname1bbk !== null && customname1bbk !== '') {
            $scope.BBKBooking.CustomField1.CustomFieldValue = customname1bbk;


        }
    
        };

    $scope.customField1 = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'BreakbulkBookingCustomFieldController',
            templateUrl: 'breakbulk/breakbulkBooking/breakbulkBookingCustomField.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'md',
            backdrop: 'static'
        });
        ModalInstance.result.then(function (res1) {
            if (res1 !== null && res1.customField1 === undefined && res1 !== '') {

                $scope.custm2 = {
                    ModuleType: "BreakBulk",
                    CustomFieldId: 0,
                    CustomFieldValue: $scope.customName2bbk = "",
                    CustomFieldName: $scope.BBKBooking.CustomField2.CustomFieldName = res1.customField1Name,
                    CustomFieldType: $scope.BBKBooking.CustomField2.CustomFieldType = "Text",
                    UserId: $scope.UserId = $scope.BBKBooking.CustomerId
                };
                //$scope.customName2bbk = "";
                //$scope.BBKBooking.CustomField2.CustomFieldName = res1.customField1Name;
                //$scope.BBKBooking.CustomField2.CustomFieldType = "Text";
                //$scope.UserId = $scope.BBKBooking.CustomerId;

            }

            else {
                     $scope.customName2bbk = res1.customField1.split(',');
                $scope.custm2 = {
                    ModuleType: "BreakBulk",
                    CustomFieldId: 0,
                    CustomFieldValue: $scope.BBKBooking.CustomField2.CustomFieldValue = res1.customField1,
                    CustomFieldName: $scope.BBKBooking.CustomField2.CustomFieldName = res1.customField1Name,
                    CustomFieldType: $scope.BBKBooking.CustomField2.CustomFieldType = "Dropdown",
                    UserId: $scope.UserId = $scope.BBKBooking.CustomerId
                };

                //$scope.customName2bbk = res1.customField1.split(',');
                //$scope.BBKBooking.CustomField2.CustomFieldName = res1.customField1Name;
                //$scope.BBKBooking.CustomField2.CustomFieldType = "Dropdown";
                //$scope.BBKBooking.CustomField2.CustomFieldValue = res1.customField1;
                //$scope.UserId = $scope.BBKBooking.CustomerId;



            }

            BreakBulkService.SaveCustomerCustomField($scope.custm2).then(function (response) {
                if (response.data !== undefined) {

                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteSuccess,
                        body: $scope.BookingSaveValidation,
                        showCloseButton: true
                    });

                }
            });
        });
    };
    $scope.CustomF2Change = function (customnamebbk2) {
        if (customnamebbk2 !== null && customnamebbk2 !== '') {
            $scope.BBKBooking.CustomField2.CustomFieldValue = customnamebbk2;
        }

    };

    $scope.customField2 = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'BreakbulkBookingCustomFieldController',
            templateUrl: 'breakbulk/breakbulkBooking/breakbulkBookingCustomField.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'md',
            backdrop: 'static'
        });
        ModalInstance.result.then(function (res2) {

            if (res2 !== null && res2.customField1 === undefined && res2 !== '') {

                $scope.custm = {
                    ModuleType: "BreakBulk",
                    CustomFieldId: 0,
                    CustomFieldValue: $scope.customName3bbk = "",
                    CustomFieldName: $scope.BBKBooking.CustomField3.CustomFieldName = res2.customField1Name,
                    CustomFieldType: $scope.BBKBooking.CustomField3.CustomFieldType = "Text",
                    UserId: $scope.UserId = $scope.BBKBooking.CustomerId
                };
                //$scope.customName3bbk = "";
                //$scope.BBKBooking.CustomField3.CustomFieldName = res2.customField1Name;
                //$scope.BBKBooking.CustomField3.CustomFieldType = "Text";
                //$scope.UserId = $scope.BBKBooking.CustomerId;

            }

            else {
                                $scope.customName3bbk = res2.customField1.split(',');

                $scope.custm = {
                    ModuleType: "BreakBulk",
                    CustomFieldId : 0,
                    CustomFieldValue: $scope.BBKBooking.CustomField3.CustomFieldValue = res2.customField1,
                    CustomFieldName: $scope.BBKBooking.CustomField3.CustomFieldName = res2.customField1Name,
                    CustomFieldType: $scope.BBKBooking.CustomField3.CustomFieldType = "Dropdown",
                    UserId: $scope.UserId = $scope.BBKBooking.CustomerId
                };

                //$scope.customName3bbk = res2.customField1.split(',');
                //$scope.BBKBooking.CustomField3.CustomFieldName = res2.customField1Name;
                //$scope.BBKBooking.CustomField3.CustomFieldType = "Dropdown";
                //$scope.BBKBooking.CustomField3.CustomFieldValue = res2.customField1;
                //$scope.UserId = $scope.BBKBooking.CustomerId;


            }
            BreakBulkService.SaveCustomerCustomField($scope.custm).then(function (response) {
                if (response.data !== undefined) {

                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteSuccess,
                        body: $scope.BookingSaveValidation,
                        showCloseButton: true
                    });

                }
            });
        });
    };
    $scope.CustomF3Change = function (customnamebbk2) {
        if (customnamebbk2 !== null && customnamebbk2 !== '') {
            $scope.BBKBooking.CustomField3.CustomFieldValue = customnamebbk2;
        }

    };
    //end

    // AddressBook 
    $scope.addressBook = function (UserType) {
        if (!$scope.BBKBooking.CustomerId) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.SelectCustomerAddressBookValidation,
                showCloseButton: true
            });
            return;
        }

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBooking/directBookingAddEdit.tpl.html',
            controller: 'DirectBookingAddEditController',
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                moduleType: function () {
                    return "Breakbulk";
                },
                toCountryId: function () {
                    if ($scope.BBKBooking.ShipTo && $scope.BBKBooking.ShipTo.Country && $scope.BBKBooking.ShipTo.Country.CountryId) {
                        return $scope.BBKBooking.ShipTo.Country.CountryId;
                    }
                    else {
                        return 0;
                    }
                },
                addressType: function () {
                    if (UserType === 'Shipper') {
                        return "FromAddress";
                    }
                    else if (UserType === 'Reeceiver') {
                        return "ToAddress";
                    }
                    else {
                        return "All";
                    }
                },
                customerId: function () {
                    return $scope.BBKBooking.CustomerId;
                },
                Countries: function () {
                    return $scope.CountriesRepo;
                },
                CountryPhoneCodes: function () {
                    return $scope.CountryPhoneCodes;
                }
            }
        });

        modalInstance.result.then(function (addressBooks) {
            if (addressBooks) {
                if (UserType === 'Shipper') {
                    $scope.BBKBooking.ShipFrom = addressBooks;
                    $scope.BBKBooking.ShipFrom.AlertEmail = addressBooks.Email;
                    $scope.BBKBooking.ShipFrom.ContactFirstName = addressBooks.FirstName;
                    $scope.BBKBooking.ShipFrom.ContactLastName = addressBooks.LastName;
                    $scope.BBKBooking.ShipFrom.PhoneNo = addressBooks.Phone;
                    $scope.BBKBooking.ShipFrom.Address1 = addressBooks.Address;
                    if (addressBooks.State) {
                        $scope.BBKBooking.ShipFrom.State = addressBooks.State;
                    }

                    $scope.BBKBooking.ShipFrom.Country = addressBooks.Country;
                    $scope.BBKBooking.ShipFrom.ExpressAddressId = 0;
                    $scope.SetShipinfo($scope.BBKBooking.ShipFrom.Country, UserType, "Fixed");


                }
                else if (UserType === 'Receiver') {
                    $scope.BBKBooking.ShipTo = addressBooks;
                    $scope.BBKBooking.ShipTo.AlertEmail = addressBooks.Email;
                    $scope.BBKBooking.ShipTo.ContactFirstName = addressBooks.FirstName;
                    $scope.BBKBooking.ShipTo.ContactLastName = addressBooks.LastName;
                    $scope.BBKBooking.ShipTo.PhoneNo = addressBooks.Phone;
                    $scope.BBKBooking.ShipTo.Address1 = addressBooks.Address;

                    if (addressBooks.State) {
                        $scope.BBKBooking.ShipTo.State = addressBooks.State;
                    }

                    $scope.BBKBooking.ShipTo.Country = addressBooks.Country;
                    $scope.BBKBooking.ShipTo.ExpressAddressId = 0;
                    $scope.SetShipinfo($scope.BBKBooking.ShipTo.Country, UserType, "Fixed");

                }


                // set form in dirty state for progress bar
                if ($scope.TradelaneBookingForm) {
                    $scope.TradelaneBookingForm.$dirty = true;
                }
            }
        });

    };

    var resetAddress = function (type) {
        if (type === "Shipper") {
            $scope.BBKBooking.ShipFrom.PostCode = "";
            $scope.BBKBooking.ShipFrom.ContactFirstName = "";
            $scope.BBKBooking.ShipFrom.ContactLastName = "";
            $scope.BBKBooking.ShipFrom.CompanyName = "";
            $scope.BBKBooking.ShipFrom.Address1 = "";
            $scope.BBKBooking.ShipFrom.Address2 = "";
            $scope.BBKBooking.ShipFrom.City = "";
            $scope.BBKBooking.ShipFrom.State = "";
            $scope.BBKBooking.ShipFrom.Area = "";
            $scope.BBKBooking.ShipFrom.PhoneNo = "";
        }
        if (type === "Receiver") {

            $scope.BBKBooking.ShipTo.PostCode = "";
            $scope.BBKBooking.ShipTo.ContactFirstName = "";
            $scope.BBKBooking.ShipTo.ContactLastName = "";
            $scope.BBKBooking.ShipTo.CompanyName = "";
            $scope.BBKBooking.ShipTo.Address1 = "";
            $scope.BBKBooking.ShipTo.Address2 = "";
            $scope.BBKBooking.ShipTo.City = "";
            $scope.BBKBooking.ShipTo.State = "";
            $scope.BBKBooking.ShipTo.Area = "";
            $scope.BBKBooking.ShipTo.PhoneNo = "";
        }
    };

    $scope.HUBAddress = function (Type) {
        if ($scope.fillPostlValues && $scope.fillPostlValues.length) {
            if (Type === 'Shipper') {
                $scope.BBKBooking.ShipFrom.City = $scope.fillPostlValues[0].City;
            }
            else if (Type === "Receiver") {
                $scope.BBKBooking.ShipTo.City = $scope.fillPostlValues[0].City;
            }
        }
        if ($scope.BBKBooking.ShipTo && $scope.BBKBooking.ShipTo.Country) {
            getHubAddress();

        }
    };

    $scope.SetShipinfo = function (Country, Action, Type) {
        if (Country) {
            if (Action === 'Shipper') {
                if (Country.Code === 'GBR') {
                    $scope.MaximamLengthShipFrom = 9;
                }
                else {
                    $scope.MaximamLengthShipFrom = 16;
                }
                $scope.showPostCodeDropDown = false;
                for (var i = 0; i < $scope.CountryPhoneCodes.length; i++) {
                    if ($scope.CountryPhoneCodes[i].CountryCode === Country.Code) {
                        $scope.ShipFromPhoneCode = "(+" + $scope.CountryPhoneCodes[i].PhoneCode + ")";
                        break;
                    }
                }
                fromAirport(Country, Action, Type);
                if (!Type) {
                    resetAddress(Action);
                }
                if ($scope.BBKBooking.ShipFrom.Country.CountryId === 38 || $scope.BBKBooking.ShipFrom.Country.CountryId === 229) {
                    $scope.GetFromCountryStates();
                }
            }
            else if (Action === 'Receiver') {
                if (Country.Code === 'GBR') {
                    $scope.MaximamLengthShipTo = 9;
                }
                else {
                    $scope.MaximamLengthShipTo = 16;
                }
                for (var j = 0; j < $scope.CountryPhoneCodes.length; j++) {
                    if ($scope.CountryPhoneCodes[j].Name === Country.Name) {
                        $scope.ShipToPhoneCode = "(+" + $scope.CountryPhoneCodes[j].PhoneCode + ")";
                        break;
                    }
                }
                if (!Type) {
                    resetAddress(Action);
                }
                getHubAddress();
            }
        }
    };

    var fromAirport = function (Country, Action, Type) {
        if (Country !== undefined && Country !== null && Country !== '') {
            BreakBulkService.GetAirlines(Country.CountryId).then(function (response) {
                if (response.data !== undefined) {
                    $scope.departairport = response.data;
                }
            });
        }
    };


    // Booking Form tab  Validation  
    $scope.shipFormValidation = function (TradelaneBookingForm) {
        if (TradelaneBookingForm.shipFromAddress1 && TradelaneBookingForm.shipFromAddress1.$valid &&
            TradelaneBookingForm.shipFromCompanyName && TradelaneBookingForm.shipFromCompanyName.$valid &&
            TradelaneBookingForm.shipFromFirstName && TradelaneBookingForm.shipFromFirstName.$valid &&
            TradelaneBookingForm.shipFromLastName && TradelaneBookingForm.shipFromLastName.$valid &&
            TradelaneBookingForm.shipFromCountry && TradelaneBookingForm.shipFromCountry.$valid &&
            TradelaneBookingForm.shipFromPostcode && TradelaneBookingForm.shipFromPostcode.$valid &&
            TradelaneBookingForm.shipFromState && TradelaneBookingForm.shipFromState.$valid &&
            TradelaneBookingForm.shipFromCityName && TradelaneBookingForm.shipFromCityName.$valid &&
            TradelaneBookingForm.departureAirPort && TradelaneBookingForm.departureAirPort.$valid &&
            TradelaneBookingForm.shipFromPhone && TradelaneBookingForm.shipFromPhone.$valid &&
            TradelaneBookingForm.shipFromMail && TradelaneBookingForm.shipFromMail.$valid
        ) {
            flag = true;
        }
        else {
            flag = false;
        }

        return flag;
    };

    $scope.shipToValidation = function (TradelaneBookingForm) {
        if (TradelaneBookingForm.shipToAddress1 && TradelaneBookingForm.shipToAddress1.$valid &&
            TradelaneBookingForm.shipToCompanyName && TradelaneBookingForm.shipToCompanyName.$valid &&
            TradelaneBookingForm.shipToFirstName && TradelaneBookingForm.shipToFirstName.$valid &&
            TradelaneBookingForm.shipToLastName && TradelaneBookingForm.shipToLastName.$valid &&
            TradelaneBookingForm.shipToCountry && TradelaneBookingForm.shipToCountry.$valid &&
            TradelaneBookingForm.shipToPostcode && TradelaneBookingForm.shipToPostcode.$valid &&
            TradelaneBookingForm.shipToState && TradelaneBookingForm.shipToState.$valid &&
            TradelaneBookingForm.shipToCityName && TradelaneBookingForm.shipToCityName.$valid &&
            TradelaneBookingForm.shipToPhone && TradelaneBookingForm.shipToPhone.$valid &&
            TradelaneBookingForm.shipToMail && TradelaneBookingForm.shipToMail.$valid
        ) {
            flag = true;
        }
        else {
            flag = false;
        }

        return flag;
    };

    $scope.PurchaseOrderValidation = function (TradelaneBookingForm) {

        if (TradelaneBookingForm.poNumber && TradelaneBookingForm.poNumber.$valid &&
            TradelaneBookingForm.styleNumber && TradelaneBookingForm.styleNumber.$valid &&
            TradelaneBookingForm.styleName && TradelaneBookingForm.styleName.$valid &&
            TradelaneBookingForm.consignmentNo && TradelaneBookingForm.consignmentNo.$valid &&
            TradelaneBookingForm.shipmentType && TradelaneBookingForm.shipmentType.$valid &&
            TradelaneBookingForm.deliveryType && TradelaneBookingForm.deliveryType.$valid &&
            TradelaneBookingForm.addIncoterm && TradelaneBookingForm.addIncoterm.$valid  &&
            TradelaneBookingForm.exfactoryDate && TradelaneBookingForm.exfactoryDate.$valid
            //&&
            //TradelaneBookingForm.customField1 && TradelaneBookingForm.customField1.$valid &&
            //TradelaneBookingForm.customField2 && TradelaneBookingForm.customField2.$valid &&
            //TradelaneBookingForm.customField3 && TradelaneBookingForm.customField3.$valid 

        )
        //{
        //    for (var j = 0; j < $scope.BBKBooking.PurchaseOrderDetail.length; j++) {
        //        var packageForm1 = TradelaneBookingForm['tags' + j];
        //        if (packageForm1 !== undefined && packageForm1.$valid) {
        //            flag = true;
        //        }
        //        else {
        //            flag = false;
        //            break;
        //        }
        //    }
        //}

        {
            flag = true;
        }
        

        else {
            flag = false;
        }
        return flag;
    };

    $scope.serviceOptionValidation = function (TradelaneBookingForm) {

        if (
            
            TradelaneBookingForm.addFactory && TradelaneBookingForm.addFactory.$valid &&
            TradelaneBookingForm.shipmentPaymentAccount && TradelaneBookingForm.shipmentPaymentAccount.$valid 


        )
       

        {
            flag = true;
        }


        else {
            flag = false;
        }
        return flag;
    };
    $scope.notifyPartyValidation = function (TradelaneBookingForm) {

        if (
            TradelaneBookingForm.shipmentPaymentAccount && TradelaneBookingForm.shipmentPaymentAccount.$valid 
            
        ) {
            flag = true;
        }


        else {
            flag = false;
        }
        return flag;
    };


    //$scope.shipmentDetailValidation = function (TradelaneBookingForm) {

    //    if (
    //        TradelaneBookingForm[packageForm].cartton && TradelaneBookingForm[packageForm].cartton.$valid &&
    //        TradelaneBookingForm[packageForm].cartton1 && TradelaneBookingForm[packageForm].cartton1.$valid &&
    //        TradelaneBookingForm[packageForm].content && TradelaneBookingForm[packageForm].content.$valid &&
    //        TradelaneBookingForm[packageForm].currencyValue && TradelaneBookingForm[packageForm].currencyValue.$valid 
            

    //    ) {
    //        flag = true;
    //    }


    //    else {
    //        flag = false;
    //    }
    //    return flag;
    //};



    //Set Multilingual for Modal Popup
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteWarning', 'FrayteInformation', 'FrayteValidation', 'FrayteSuccess', 'CustomInfoCheckTerms',
            'PleaseSelectionDeliveryOption', 'PleaseSelectTermAndConditions', 'PleaseCorrectValidationErrors',
            'ErrorSavingShipment', 'PleaseSelectValidFile', 'UploadedSuccessfully', 'ErrorOccuredDuringUpload',
            'ErrorGettingShipmentDetailServer', 'SaveDraft_Validation', 'PackageShipment_Validation',
            'SelectShipment_Validation', 'CustomInformation_Validation', 'BookingSave_Validation',
            'ServiceSideError_Validation', 'Success', 'RemovePackage_Validation', 'Validation_Error', 'GetService_Validation',
            'SelectCustomer_Validation', 'SelectCurrency_Validation', 'FrayteWarning_Validation', 'SelectCustomerAddressBook_Validation',
            'FrayteServiceError_Validation', 'ReceiveDetail_Validation', 'InitialData_Validation', 'DirectBooking_Error_Message', 'Confirmation',
            'RemoveChangingParcelTypeDoc', 'Errorwhil_uploading_the_excel', 'PackageAddeddSuccessfully', 'MaxWeight154KGS', 'MaxWeight70kGS',
            'Sure_Clear_The_Form', 'Loading_Post_Code_Address']).then(function (translations) {
                $scope.TitleFrayteError = translations.FrayteError;
                $scope.TitleFrayteWarning = translations.FrayteWarning;
                $scope.TitleFrayteInformation = translations.FrayteSuccess;
                $scope.TitleFrayteValidation = translations.FrayteValidation;
                $scope.TitleFrayteSuccess = translations.FrayteSuccess;
                $scope.TextValidation = translations.DirectBooking_Error_Message;
                $scope.TextPleaseSelectionDeliveryOption = translations.PleaseSelectionDeliveryOption;
                $scope.TextErrorSavingShipment = translations.ErrorSavingShipment;
                $scope.TextCustomInfoCheckTerms = translations.CustomInfoCheckTerms;
                $scope.TextPleaseSelectTermAndConditions = translations.PleaseSelectTermAndConditions;
                $scope.TextPleaseSelectValidFile = translations.PleaseSelectValidFile;
                $scope.TextUploadedSuccessfully = translations.UploadedSuccessfully;
                $scope.TextErrorOccuredDuringUpload = translations.ErrorOccuredDuringUpload;
                $scope.TextErrorGettingShipmentDetailServer = translations.ErrorGettingShipmentDetailServer;
                $scope.SaveDraft = translations.SaveDraft_Validation;
                $scope.PackageShipment_Validation = translations.PackageShipment_Validation;
                $scope.PleaseSelectShipment = translations.SelectShipment_Validation;
                $scope.CustomInformationValidation = translations.CustomInformation_Validation;
                $scope.BookingSaveValidation = translations.BookingSave_Validation;
                $scope.ServiceSideErrorValidation = translations.ServiceSideError_Validation;
                $scope.Success = translations.Success;
                $scope.RemovePackageValidation = translations.RemovePackage_Validation;
                $scope.ValidationError = translations.Validation_Error;
                $scope.GetServiceValidation = translations.GetService_Validation;
                $scope.SelectCustomerValidation = translations.SelectCustomer_Validation;
                $scope.SelectCurrencyValidation = translations.SelectCurrency_Validation;
                $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
                $scope.SelectCustomerAddressBookValidation = translations.SelectCustomerAddressBook_Validation;
                $scope.ReceiveDetailValidation = translations.FrayteServiceError_Validation;
                $scope.FrayteServiceErrorValidation = translations.ReceiveDetail_Validation;
                $scope.InitialDataValidation = translations.InitialData_Validation;
                $scope.Confirmation = translations.Confirmation;
                $scope.RemoveChangingParcelTypeDoc = translations.RemoveChangingParcelTypeDoc;
                $scope.Errorwhil_uploading_the_excel = translations.Errorwhil_uploading_the_excel;
                $scope.PackageAddeddSuccessfully = translations.PackageAddeddSuccessfully;
                $scope.MaxWeight154KGS = translations.MaxWeight154KGS;
                $scope.MaxWeight70kGS = translations.MaxWeight70kGS;
                $scope.Sure_Clear_The_Form = translations.Sure_Clear_The_Form;
                $scope.Loading_Post_Code_Address = translations.Loading_Post_Code_Address;
            });
    };

   
    //json for BBK
    var newBooking = function () {
        $scope.BBKBooking = {
            HubCarrierServiceId: 0,
            CustomerId: 0,
            FactoryUserId: "",
            POStatusId: 0,           
            CreatedOn: new Date(),
            CreatedBy: $scope.CreatedBy,
            Careof: $scope.Careof,
            UpdatedOn: new Date(),
            UpdatedBy: 0,
            ShipFrom:
            {
                AddressBookId: 0,
                Country: null,
                Airport: null,
                CompanyName: "",
                ContactFirstName: "",
                ContactLastName: "",
                Address1: "",
                Address2: "",
                City: "",
                State: "",
                Area: "",
                PhoneNo: "",
                PostCode: "",
                AlertEmail: "",
                AddressType: "FromAddress",
                IsFavorites: false,
                IsDefaultAddess: false
            },
            ShipTo:
            {
                AddressBookId: 0,
                Country: null,
                CompanyName: "",
                ContactFirstName: "",
                ContactLastName: "",
                Address1: "",
                Address2: "",
                City: "",
                State: "",
                Area: "",
                PhoneNo: "",
                PostCode: "",
                AlertEmail: "",
                AddressType: "ToAddress",
                IsFavorites: false,
                IsDefaultAddess: false
            },
            PurchaseOrder:
            {
                HubCarrierServiceId: 0,
                PONumber: "",
                StyleNumber: "",
                StyleName: "",
                ConsignmentNumber: 0,
                ShipmentType: "",
                DeliveryType: "",
                IncotermId: "",
                ExFactoryDate: "",
                DefaultShipmentType: ""
            },
            Service: {
                HubCarrierId: 0,
                LogoName: '',
                HubCarrier: '',
                CourierId: 0,
                LogisticServiceId: 0,
                LogisticServiceType: '',
                HubCarrierDisplay: '',
                CourierAccountNo: '',
                RateType: '',
                TransitTime: ''
            },
            CustomField1: {
                CustomFieldId: 0,
                CustomFieldType: "",
                CustomFieldName: "",
                CustomFieldValue: "",
                ModuleType: "BreakBulk"
            },
            CustomField2: {
                CustomFieldId: 0,
                CustomFieldType: "",
                CustomFieldName: "",
                CustomFieldValue: "",
                ModuleType: "BreakBulk"
            },
            CustomField3: {
                CustomFieldId: 0,
                CustomFieldType: "",
                CustomFieldName: "",
                CustomFieldValue: "",
                ModuleType: "BreakBulk"
            },
            PurchaseOrderDetail: [{
                PurchaseOrderDetailId: 0,
                PurchaseOrderId: 0,
                ProductCatalogId: 0,
                JobStatusId: 0,
                JobNo: "",
                JobName: "",
                Description: "",
                OrderQTY: ""
            }]
        };
    };

    var getHubAddress = function () {

        var Postcode = "";
        var State = "";

        if (!$scope.BBKBooking.ShipTo.PostCode) {
        }
        else {
            Postcode = $scope.BBKBooking.ShipTo.PostCode;
        }

        if (!$scope.BBKBooking.ShipTo.State) {
        }
        else {
            State = $scope.BBKBooking.ShipTo.State;
        }

        BreakBulkService.GetHubAddress($scope.BBKBooking.ShipTo.Country.CountryId, Postcode, State).then(function (response) {
            $scope.hubAddress = response.data;
        });

        if ($scope.BBKBooking.ShipTo.Country.CountryId === 38) {
            $scope.GetCountryStates();
        }
        if ($scope.BBKBooking.ShipTo.Country.CountryId === 229) {
            $scope.GetCountryUSAStates();
        }

    };



    //FromCountryStateList
    $scope.GetFromCountryStates = function (shipFromCountry) {
        ExpressBookingService.GetFromCountryState($scope.BBKBooking.ShipFrom.Country.CountryId).then(function (response) {
            $scope.CountryFromStateList = response.data;
        }, function (error) {
        });
    };

    //ToCountryStateList
    $scope.GetCountryStates = function (shipToCountry) {
        ExpressBookingService.GetCountryState($scope.BBKBooking.ShipTo.Country.CountryId).then(function (response) {
            $scope.CountryStateList = response.data;
        }, function (error) {
        });
    };


    //ToCountryStateList
    $scope.GetCountryUSAStates = function () {
        ExpressBookingService.GetFromCountryState($scope.BBKBooking.ShipTo.Country.CountryId).then(function (response) {
            $scope.CountryStateList = response.data;
            if ($scope.CountryStateList.length > 0) {
                for (i = 0; i < $scope.CountryStateList.length; i++) {
                    $scope.CountryStateList[i].StateDisplay = $scope.CountryStateList[i].State;
                }
            }
        }, function (error) {
        });
    };
 
    // shipfromcountry phonecode
    $scope.FromCountries = function (shipFromCountry, PaymentParty) {

        if (shipFromCountry !== null && PaymentParty !== null && PaymentParty !== undefined) {
            $scope.BBKBooking.ShipFrom.CustomerId = PaymentParty.CustomerId;

            BreakBulkService.GetInitials(userId).then(function (response) {
                if (response.data !== undefined) {
                    if (response.data.Countries !== null && response.data.Countries !== undefined) {
                        for (j = 0; j < response.data.CountryPhoneCodes.length; j++) {
                            if (shipFromCountry.Code === response.data.CountryPhoneCodes[j].CountryCode) {
                                $scope.ShipFromPhoneCode = "(+" + response.data.CountryPhoneCodes[j].PhoneCode + ")";
                            }
                        }
                    }
                }
            });
        }

        else  {

            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.SelectCustomerAddressBookValidation,
                showCloseButton: true
            });
        }
    };

    //shiptocountry phonecode
    $scope.ToCountries = function (shipToCountry) {
        if (shipToCountry !== null && shipToCountry !== '' && shipToCountry !== undefined) {

            BreakBulkService.GetInitials(userId).then(function (response) {
                if (response.data !== undefined) {
                    getHubAddress();
                    if (response.data.Countries !== null && response.data.Countries !== undefined) {
                        for (j = 0; j < response.data.CountryPhoneCodes.length; j++) {
                            if (shipToCountry.Code === response.data.CountryPhoneCodes[j].CountryCode) {
                                $scope.ShipToPhoneCode = "(+" + response.data.CountryPhoneCodes[j].PhoneCode + ")";
                            }
                        }
                    }
                }
            });
            //newBooking();
        }
    };

    //departure airport

    $scope.FromAirport = function (CountryId) {
        if (CountryId !== undefined && CountryId !== null && CountryId !== '') {
            BreakBulkService.GetAirlines(CountryId).then(function (response) {
                if (response.data !== undefined) {
                    $scope.departairport = response.data;
                }
            });
        }
    };

    $scope.shipFromOptionalField = function (action) {
        if (action !== undefined && action !== null && action !== '' && action === 'State') {
            if ($scope.BBKBooking !== undefined && $scope.BBKBooking !== null && $scope.BBKBooking.ShipFrom !== null && $scope.BBKBooking.ShipFrom.Country !== null) {
                if ($scope.BBKBooking.ShipFrom.Country.Code === 'HKG' || $scope.BBKBooking.ShipFrom.Country.Code === 'GBR') {
                    return false;
                }
                else if ($scope.BBKBooking.ShipFrom.Country.Code === 'CAN' || $scope.BBKBooking.ShipFrom.Country.Code === 'USA' || $scope.BBKBooking.ShipFrom.Country.Code === 'AUS') {
                    return false;
                }
                else {
                    return true;
                }
            }
        }
        else if (action !== undefined && action !== null && action !== '' && action === 'PostCode') {
            if ($scope.BBKBooking !== undefined && $scope.BBKBooking !== null && $scope.BBKBooking.ShipFrom !== null && $scope.BBKBooking.ShipFrom.Country !== null) {
                if ($scope.BBKBooking.ShipFrom.Country.Code !== 'HKG') {
                    return true;
                }
                else {
                    $scope.BBKBooking.ShipFrom.PostCode = '';
                    return false;
                }
            }
        }
    };

    $scope.shipToOptionalField = function (action) {
        if (action !== undefined && action !== null && action !== '' && action === 'State') {
            if ($scope.BBKBooking !== undefined && $scope.BBKBooking !== null && $scope.BBKBooking.ShipTo !== null && $scope.BBKBooking.ShipTo.Country !== null) {
                if ($scope.BBKBooking.ShipTo.Country.Code === 'HKG' || $scope.BBKBooking.ShipTo.Country.Code === 'GBR') {
                    return false;
                }
                else if ($scope.BBKBooking.ShipTo.Country.Code === 'CAN' || $scope.BBKBooking.ShipTo.Country.Code === 'USA' || $scope.BBKBooking.ShipTo.Country.Code === 'AUS') {
                    return false;
                }
                else {
                    return true;
                }
            }
        }
        else if (action !== undefined && action !== null && action !== '' && action === 'PostCode') {
            if ($scope.BBKBooking !== undefined && $scope.BBKBooking !== null && $scope.BBKBooking.ShipTo !== null && $scope.BBKBooking.ShipTo.Country !== null) {
                if ($scope.BBKBooking.ShipTo.Country.Code !== 'HKG') {
                    return true;
                }
                else {
                    $scope.BBKBooking.ShipTo.PostCode = '';
                    return true;
                }
            }
        }
    };
  
    //careof
    $scope.careof = function (CompanyName) {
        if (CompanyName !== undefined && CompanyName !== null && CompanyName !== '' && $scope.RoleId === 1) {         
            $scope.BBKBooking.Careof = CompanyName;
        }
    };

    //consignmentNo
    $scope.getconsignmentNo = function (PaymentParty) {
        if (PaymentParty !== undefined && PaymentParty !== null && PaymentParty !== '' && PaymentParty.CustomerId > 0) {
            //$scope.NewDirectBooking();
            $scope.BBKBooking.CustomerId = PaymentParty.CustomerId;
            $scope.CustDetail = PaymentParty;
            $scope.PaymentParty = PaymentParty;
            BreakBulkService.GenerateConsignmentNumber(PaymentParty.CustomerId).then(function (response) {
                if (response.data !== undefined && response.data !== null && response.data !== '') {
                    $scope.BBKBooking.PurchaseOrder.ConsignmentNumber = response.data;
                }
            });
        }
    };

    //checkbox set as default
    $scope.DefShipType = function (PaymentParty) {
        if (PaymentParty !== undefined && PaymentParty !== null && PaymentParty !== '') {
            for (i = 0; i < $scope.ShipmentMethod.length; i++) {
                if ($scope.ShipmentMethod[i].ShipmentHandlerMethodName === $scope.PaymentParty.DefaultShipmentType) {
                    $scope.shipmenttype = $scope.ShipmentMethod[i];
                    $scope.BBKBooking.PurchaseOrder.DefaultShipmentType = $scope.shipmenttype;
                    $scope.BBKBooking.PurchaseOrder.CustomerId = PaymentParty.CustomerId;

                }
            }
        }
    };
    //$scope.DefShipType = function (ShipmentType) {
    //    if (ShipmentType !== null && ShipmentType !== null && ShipmentType !== '') {

    //        $scope.BBKBooking.PurchaseOrder.DefaultShipmentType = ShipmentType.ShipmentHandlerMethodDisplay;


    //        //$scope.DefaultShip = {
    //        //    //DefaultShipmentType: $scope.Default = $scope.BBKBooking.PurchaseOrder.DefaultShipmentType,
    //        //    DefaultShipmentType: $scope.Default = $scope.ShipmentMethod.ShipmentHandlerMethodDisplay,
    //        //    UserId: $scope.UserId = $scope.BBKBooking.CustomerId
    //        //};

    //        //BreakBulkService.SaveCustomerShipmentType($scope.DefaultShip).then(function (response) {
    //        //    if (response.data !== undefined) {

    //        //        toaster.pop({
    //        //            type: 'success',
    //        //            title: $scope.TitleFrayteSuccess,
    //        //            body: $scope.BookingSaveValidation,
    //        //            showCloseButton: true
    //        //        });

    //        //    }
    //        //});
    //    }
            
    //        //else {
    //        //    $scope.Default = false;
    //        //}                
    //};

    //shipment type
    $scope.shipmentType = function (PaymentParty) {
        if (PaymentParty !== undefined && PaymentParty !== null && PaymentParty !== '') {
            for (i = 0; i < $scope.ShipmentMethod.length; i++) {
                if ($scope.ShipmentMethod[i].ShipmentHandlerMethodName === $scope.PaymentParty.DefaultShipmentType) {
                    $scope.shipmenttype = $scope.ShipmentMethod[i];
                }
            }
        }
    };

    //ex factory date
    $scope.ChangeTodate = function (ToDate) {

        var dateOne = new Date(from);
        if (new Date(dateOne) !== '') {
        }
        //$scope.BBKBooking.PurchaseOrder.ExFactoryDate = null;
    

        $scope.SetTimeinDateObj = function (DateValue) {
            var newdate1 = new Date();
            newdate = new Date(DateValue);
            var gtDate = newdate.getDate();
            var gtMonth = newdate.getMonth();
            var gtYear = newdate.getFullYear();
            var hour = newdate1.getHours();
            var min = newdate1.getMinutes();
            var Sec = newdate1.getSeconds();
            var MilSec = new Date().getMilliseconds();
            return new Date(gtYear, gtMonth, gtDate, hour, min, Sec, MilSec);
        };
        $scope.BBKBooking.PurchaseOrder.ExFactoryDate = $scope.SetTimeinDateObj(ToDate);

        var fromDate = new Date($scope.BBKBooking.PurchaseOrder.ExFactoryDate);

        var from = ((fromDate.getMonth() + 1).toString().length === 1 ? "0" + (fromDate.getMonth() + 1) : (fromDate.getMonth() + 1)) + '/' + (fromDate.getDate().toString().length === 1 ? "0" + fromDate.getDate() : fromDate.getDate()) + '/' + fromDate.getFullYear();

    };

    //length weight shipment information   
    $scope.jobno = 1;
    //$scope.BBKBooking.PurchaseOrderDetail = [{ JobNo: "", Desc: "", OrderQTY: "", JobName: "" }];
    $scope.AddPackage = function () {
        $scope.jobno++;
        $scope.TotalJobs = $scope.jobno;
        $scope.BBKBooking.PurchaseOrderDetail.push({
            PurchaseOrderDetailId: 0,
            PurchaseOrderId: 0,
            ProductCatalogId: 0,
            JobStatusId:0,
            JobNo: "",
            JobName: "",
            Description: "",
            OrderQTY: "" });
        $scope.HideContent = false;
        //var dbpac = $scope.BBKBooking.Package.length - 1;
        //for (i = 0; i < $scope.BBKBooking.Package.length; i++) {

        //    if (i === dbpac) {
        //        $scope.BBKBooking.Package[i].pacVal = true;
        //    }
        //    else {
        //        $scope.BBKBooking.Package[i].pacVal = false;
        //    }
        //}
    };

    $scope.OrderQTY = function (PurchaseOrderDetails) {

        if (PurchaseOrderDetails === undefined || PurchaseOrderDetails === null) {
            return 0;
        }
         if ($scope.BBKBooking.PurchaseOrderDetail.length >= 1) {
            var Order = parseFloat(0);
            for (var i = 0; i < $scope.BBKBooking.PurchaseOrderDetail.length; i++) {
                Order += Number($scope.BBKBooking.PurchaseOrderDetail[i].OrderQTY);
                Order += parseFloat(0);

            }
            return Order;
        }
        else {
            return 0;
        }
    };

    //REMOVE JOB NUMBER AND ORDER QTY
    $scope.RemovePackage = function (element) {
        $scope.HideContent = true;
        $scope.jobno--;
        $scope.TotalJobs = $scope.jobno;
        var index = $scope.BBKBooking.PurchaseOrderDetail.indexOf(element); //gets the current user index
        $scope.BBKBooking.PurchaseOrderDetail.splice(index, 1); // remove the user from array by using the index

    };

    //clearform
    var clearDirectBookingForm = function (form) {

        form.$setPristine();
        form.$setUntouched();
        newBooking();
        //$scope.active = 3;

        $scope.payments  = null;

        if ($scope.RoleId === 3) {
            $scope.BBKBooking.CustomerId = $scope.customerId;
        }

        if ($scope.ShipFromPhoneCode) {
            $scope.ShipFromPhoneCode = null;
        }

        if ($scope.ShipToPhoneCode) {
            $scope.ShipToPhoneCode = null;
        }

        //clear declaration hub address
        $scope.hubAddress = {
            Country: null,
            PostCode: null,
            CompanyName: null,
            Address: null,
            Address2: null,
            City: null,
            State: null,
            Area: null
        };
        //end

        window.scrollTo(0, 0);
    };


    $scope.ClearForm = function (form) {

        if (form.$dirty) {
            var modalOptions = {
                headerText: $scope.Confirmation,
                bodyText: $scope.Sure_Clear_The_Form
            };

            ModalService.Confirm({}, modalOptions).then(function (result) {
                clearDirectBookingForm(form);
            }, function () {

            });
        }
        else {
            clearDirectBookingForm(form);
        }
    };

    var setCourierCompanyImage = function () {
        if ($scope.services.length) {
            for (var i = 0; i < $scope.services.length; i++) {
                $scope.services[i].ImageURL = $scope.ImagePath + $scope.services[i].CarrierLogo;
            }
        }
    };

    //$scope.assign = function () {
    //    $scope.BBKBooking.PurchaseOrder.ExFactoryDate = $('#idName').val();
    //};


    //GET SERVICES EXPRESS
    $scope.GetServices = function (TradelaneBookingForm, BBKBooking) {

        if (BBKBooking.ShipFrom.Country == null || BBKBooking.ShipFrom.Country === undefined || BBKBooking.ShipFrom.Country === '' ||
            BBKBooking.ShipFrom.PostCode == null || BBKBooking.ShipFrom.PostCode === undefined || BBKBooking.ShipFrom.PostCode === '' ||
            BBKBooking.ShipFrom.ContactFirstName == null || BBKBooking.ShipFrom.ContactFirstName === undefined || BBKBooking.ShipFrom.ContactFirstName === '' ||
            BBKBooking.ShipFrom.ContactLastName == null || BBKBooking.ShipFrom.ContactLastName === undefined || BBKBooking.ShipFrom.ContactLastName === '' ||
            BBKBooking.ShipFrom.Airport == null || BBKBooking.ShipFrom.Airport === undefined || BBKBooking.ShipFrom.Airport === '' ||
            BBKBooking.ShipFrom.CompanyName == null || BBKBooking.ShipFrom.CompanyName === undefined || BBKBooking.ShipFrom.CompanyName === '' ||
            BBKBooking.ShipFrom.Address1 == null || BBKBooking.ShipFrom.Address1 === undefined || BBKBooking.ShipFrom.Address1 === '' ||
            BBKBooking.ShipFrom.City == null || BBKBooking.ShipFrom.City === undefined || BBKBooking.ShipFrom.City === '' ||
            BBKBooking.ShipFrom.PhoneNo == null || BBKBooking.ShipFrom.PhoneNo === undefined || BBKBooking.ShipFrom.PhoneNo === '' ||

            BBKBooking.ShipTo.Country == null || BBKBooking.ShipTo.Country === undefined || BBKBooking.ShipTo.Country === '' ||
            BBKBooking.ShipTo.PostCode == null || BBKBooking.ShipTo.PostCode === undefined || BBKBooking.ShipTo.PostCode === '' ||
            BBKBooking.ShipTo.ContactFirstName == null || BBKBooking.ShipTo.ContactFirstName === undefined || BBKBooking.ShipTo.ContactFirstName === '' ||
            BBKBooking.ShipTo.ContactLastName == null || BBKBooking.ShipTo.ContactLastName === undefined || BBKBooking.ShipTo.ContactLastName === '' ||
            BBKBooking.ShipTo.Address1 == null || BBKBooking.ShipTo.Address1 === undefined || BBKBooking.ShipTo.Address1 === '' ||
            BBKBooking.ShipTo.City == null || BBKBooking.ShipTo.City === undefined || BBKBooking.ShipTo.City === '' ||
            BBKBooking.ShipTo.PhoneNo == null || BBKBooking.ShipTo.PhoneNo === undefined || BBKBooking.ShipTo.PhoneNo === '' ||

            BBKBooking.PurchaseOrder.PONumber == null || BBKBooking.PurchaseOrder.PONumber === undefined || BBKBooking.PurchaseOrder.PONumber === '' ||
            BBKBooking.PurchaseOrder.StyleNumber == null || BBKBooking.PurchaseOrder.StyleNumber === undefined || BBKBooking.PurchaseOrder.StyleNumber === '' ||
            BBKBooking.PurchaseOrder.StyleName == null || BBKBooking.PurchaseOrder.StyleName === undefined || BBKBooking.PurchaseOrder.StyleName === '' ||
            BBKBooking.PurchaseOrder.ConsignmentNumber == null || BBKBooking.PurchaseOrder.ConsignmentNumber === undefined || BBKBooking.PurchaseOrder.ConsignmentNumber === '' ||
            BBKBooking.PurchaseOrder.ShipmentType == null || BBKBooking.PurchaseOrder.ShipmentType === undefined || BBKBooking.PurchaseOrder.ShipmentType === '' ||
            BBKBooking.PurchaseOrder.DeliveryType == null || BBKBooking.PurchaseOrder.DeliveryType === undefined || BBKBooking.PurchaseOrder.DeliveryType === '' ||
            BBKBooking.PurchaseOrder.IncotermId == null || BBKBooking.PurchaseOrder.IncotermId === undefined || BBKBooking.PurchaseOrder.IncotermId === '' ||
            //BBKBooking.CustomField1.CustomFieldName == null || BBKBooking.CustomField1.CustomFieldName === undefined || BBKBooking.CustomField1.CustomFieldName === '' ||
            //BBKBooking.CustomField1.CustomFieldValue == null || BBKBooking.CustomField1.CustomFieldValue === undefined || BBKBooking.CustomField1.CustomFieldValue === '' ||

            //BBKBooking.PurchaseOrder.IncotermId == null || BBKBooking.PurchaseOrder.IncotermId === undefined || BBKBooking.PurchaseOrder.IncotermId === '' ||
            BBKBooking.PurchaseOrder.ExFactoryDate == null || BBKBooking.PurchaseOrder.ExFactoryDate === undefined || BBKBooking.PurchaseOrder.ExFactoryDate === '') {


            //CustomField1: {
            //    CustomFieldId: 0,
            //        CustomFieldType: "",
            //            CustomFieldName: "",
            //                CustomFieldValue: "",
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.TextValidation,
                showCloseButton: true
            });

        }

        else {

            if (!$scope.BBKBooking.CustomerId) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.TextValidation,
                    showCloseButton: true
                });

                return;
            }
            if (!$scope.BBKBooking.ShipFrom.Country || !$scope.BBKBooking.ShipFrom.Country.CountryId) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarning,
                    body: $scope.Select_Address_Form_First,
                    showCloseButton: true
                });

                return;
            }
            if (!$scope.BBKBooking.ShipTo.Country || !$scope.BBKBooking.ShipTo.Country.CountryId || !$scope.BBKBooking.ShipTo.PostCode) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarning,
                    body: $scope.Select_Address_To_First,
                    showCloseButton: true
                });

                return;
            }

            var flag = true;
            for (var j = 0; j < $scope.BBKBooking.PurchaseOrderDetail.length; j++) {
                var packageForm1 = TradelaneBookingForm['tags' + j];
                if (packageForm1 && packageForm1.$valid) {
                    flag = true;
                }
                else {
                    flag = false;
                    break;
                }
            }

            if (!flag) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.TextValidation,
                    showCloseButton: true
                });

                return;
            }

            $scope.breakbulkServiceObj = {
                FromCountryId: 0,
                FromPostCode: '',
                ToCountryId: 0,
                ToPostCode: '',
                //TotalWeight: 0.00,
                CustomerId: 0
            };

            $scope.breakbulkServiceObj.FromCountryId = $scope.BBKBooking.ShipFrom.Country.CountryId;
            $scope.breakbulkServiceObj.FromPostCode = $scope.BBKBooking.ShipFrom.PostCode;
            $scope.breakbulkServiceObj.ToCountryId = $scope.BBKBooking.ShipTo.Country.CountryId;
            $scope.breakbulkServiceObj.ToPostCode = $scope.BBKBooking.ShipTo.PostCode;
            $scope.breakbulkServiceObj.ToState = $scope.BBKBooking.ShipTo.State !== undefined && $scope.BBKBooking.ShipTo.State !== null && $scope.BBKBooking.ShipTo.State !== "" ? $scope.BBKBooking.ShipTo.State : "";
            //$scope.breakbulkServiceObj.TotalWeight = weightTotal;
            $scope.breakbulkServiceObj.CustomerId = $scope.BBKBooking.CustomerId;

            BreakBulkService.BreakBulkHubServices($scope.breakbulkServiceObj).then(function (response) {

                $scope.services = response.data;
                setCourierCompanyImage();
                if ($scope.services.length === 1) {
                    $scope.BBKBooking.Service = $scope.services[0];
                }
                else {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'breakbulk/breakbulkGetservice/breakbulkGetservice.tpl.html',
                        controller: 'BreakbulkGetserviceController',
                        windowClass: 'BreakBulkService',
                        size: 'lg',
                        backdrop: 'static',
                        resolve: {
                            BreakbulkBookingObj: function () {
                                return $scope.BBKBooking;
                            }
                        }
                    });

                    modalInstance.result.then(function (RateCard) {
                        if (RateCard) {
                            $scope.BBKBooking.Service = RateCard;
                        }
                    }, function () {

                    });
                }
            }, function () {
                console.log("Could not get services");
            });

        }

    };

    //remove services
    $scope.cancelServices = function () {
        $scope.BBKBooking.Service = {
            HubCarrierId: 0,
            LogoName: '',
            HubCarrier: '',
            CourierId: 0,
            LogisticServiceId: 0,
            LogisticServiceType: '',
            HubCarrierDisplay: '',
            CourierAccountNo: '',
            RateType: '',
            TransitTime: ''
        };
    };

    //placebooking
    $scope.PlaceBooking = function (TradelaneBookingForm, isValid, BBKBooking) {

        if (BBKBooking.ShipFrom.Country == null || BBKBooking.ShipFrom.Country === undefined || BBKBooking.ShipFrom.Country === '' ||
            BBKBooking.ShipFrom.PostCode == null || BBKBooking.ShipFrom.PostCode === undefined || BBKBooking.ShipFrom.PostCode === '' ||
            BBKBooking.ShipFrom.ContactFirstName == null || BBKBooking.ShipFrom.ContactFirstName === undefined || BBKBooking.ShipFrom.ContactFirstName === '' ||
            BBKBooking.ShipFrom.ContactLastName == null || BBKBooking.ShipFrom.ContactLastName === undefined || BBKBooking.ShipFrom.ContactLastName === '' ||
            BBKBooking.ShipFrom.Airport == null || BBKBooking.ShipFrom.Airport === undefined || BBKBooking.ShipFrom.Airport === '' ||
            BBKBooking.ShipFrom.CompanyName == null || BBKBooking.ShipFrom.CompanyName === undefined || BBKBooking.ShipFrom.CompanyName === '' ||
            BBKBooking.ShipFrom.Address1 == null || BBKBooking.ShipFrom.Address1 === undefined || BBKBooking.ShipFrom.Address1 === '' ||
            BBKBooking.ShipFrom.City == null || BBKBooking.ShipFrom.City === undefined || BBKBooking.ShipFrom.City === '' ||
            BBKBooking.ShipFrom.PhoneNo == null || BBKBooking.ShipFrom.PhoneNo === undefined || BBKBooking.ShipFrom.PhoneNo === '' ||
        
            BBKBooking.ShipTo.Country == null || BBKBooking.ShipTo.Country === undefined || BBKBooking.ShipTo.Country === '' ||
            BBKBooking.ShipTo.PostCode == null || BBKBooking.ShipTo.PostCode === undefined || BBKBooking.ShipTo.PostCode === '' ||
            BBKBooking.ShipTo.ContactFirstName == null || BBKBooking.ShipTo.ContactFirstName === undefined || BBKBooking.ShipTo.ContactFirstName === '' ||
            BBKBooking.ShipTo.ContactLastName == null || BBKBooking.ShipTo.ContactLastName === undefined || BBKBooking.ShipTo.ContactLastName === '' ||
            BBKBooking.ShipTo.Address1 == null || BBKBooking.ShipTo.Address1 === undefined || BBKBooking.ShipTo.Address1 === '' ||
            BBKBooking.ShipTo.City == null || BBKBooking.ShipTo.City === undefined || BBKBooking.ShipTo.City === '' ||
            BBKBooking.ShipTo.PhoneNo == null || BBKBooking.ShipTo.PhoneNo === undefined || BBKBooking.ShipTo.PhoneNo === '' ||

            BBKBooking.PurchaseOrder.PONumber == null || BBKBooking.PurchaseOrder.PONumber === undefined || BBKBooking.PurchaseOrder.PONumber === '' ||
            BBKBooking.PurchaseOrder.StyleNumber == null || BBKBooking.PurchaseOrder.StyleNumber === undefined || BBKBooking.PurchaseOrder.StyleNumber === '' ||
            BBKBooking.PurchaseOrder.StyleName == null || BBKBooking.PurchaseOrder.StyleName === undefined || BBKBooking.PurchaseOrder.StyleName === '' ||
            BBKBooking.PurchaseOrder.ConsignmentNumber == null || BBKBooking.PurchaseOrder.ConsignmentNumber === undefined || BBKBooking.PurchaseOrder.ConsignmentNumber === '' ||
            BBKBooking.PurchaseOrder.ShipmentType == null || BBKBooking.PurchaseOrder.ShipmentType === undefined || BBKBooking.PurchaseOrder.ShipmentType === '' ||
            BBKBooking.PurchaseOrder.DeliveryType == null || BBKBooking.PurchaseOrder.DeliveryType === undefined || BBKBooking.PurchaseOrder.DeliveryType === '' ||
            BBKBooking.PurchaseOrder.IncotermId == null || BBKBooking.PurchaseOrder.IncotermId === undefined || BBKBooking.PurchaseOrder.IncotermId === '' ||
            BBKBooking.PurchaseOrder.ExFactoryDate == null || BBKBooking.PurchaseOrder.ExFactoryDate === undefined || BBKBooking.PurchaseOrder.ExFactoryDate === '' ) {

            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.TextValidation,
                showCloseButton: true
            });

        }

        else {

            //if ($scope.BookingType === "Draft") {
            //    $scope.RemainFieldLength = [];
            //}
            //else {
            //    $scope.RemainFieldLength = remainedFieldsPopup(BBKBooking);
            //}


            //if ($scope.RemainFieldLength.length === 0) {
            if ($scope.BookingType !== "Draft") {

                for (var k = 0; k < $scope.BBKBooking.PurchaseOrderDetail.length; k++) {

                    $scope.BBKBooking.PurchaseOrderDetail[k].JobStatusId = 48;
                }

                var flag = false;
                for (var i = 0; i < $scope.BBKBooking.PurchaseOrderDetail.length; i++) {
                    var packageForm = TradelaneBookingForm['tags' + i];
                    if (packageForm !== undefined && packageForm.$valid) {
                        flag = true;
                    }
                    else {
                        flag = false;
                        break;
                    }
                }
                if (!flag) {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteWarning,
                        body: $scope.TextValidation,
                        showCloseButton: true
                    });
                    return;
                }
            }



            if ($scope.BookingType === "Draft" && BBKBooking.ShipFrom.Country !== null && BBKBooking.ShipTo.Country !== null || ($scope.BookingType === 'JobPlaced')) {

                if ($scope.BookingType === "Draft") {
                    BBKBooking.POStatusId = 46;
                    //$rootScope.GetServiceValue = '';
                    //AppSpinner.showSpinnerTemplate('Saving Shipment', $scope.Template);
                }
                else {
                    $scope.BBKBooking.POStatusId = 45;
                    BBKBooking.PurchaseOrderDetail.JobStatusId = 48;

                    //AppSpinner.showSpinnerTemplate('', $scope.Template);
                }
            }

            BreakBulkService.SavePurchaseOrderData($scope.BBKBooking).then(function (response) {
                if (response.data !== undefined) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteSuccess,
                        body: $scope.BookingSaveValidation,
                        showCloseButton: true
                    });

                    $state.go("loginView.userTabs.break-bulk-shipments");

                    //$scope.BBKBooking.FactoryUserId = response.data.FactoryUserId;
                    //var userInfo = SessionService.getUser();
                    //$scope.userInfo = userInfo;
                    //userId = $scope.userInfo.EmployeeId;
                    //$scope.RoleId = userInfo.RoleId;             

                }
            });
        }
        //}
    };

    //save as draft
    $scope.SaveDraftBooking = function (TradelaneBookingForm, isValid, BBKBooking) {
        if ($scope.BookingType == "Draft") {

            for (var k = 0; k < $scope.BBKBooking.PurchaseOrderDetail.length; k++) {

                $scope.BBKBooking.PurchaseOrderDetail[k].JobStatusId = 49;
            }


                
                var flag = false;
                for (var i = 0; i < $scope.BBKBooking.PurchaseOrderDetail.length; i++) {
                    var packageForm = TradelaneBookingForm['tags' + i];
                    if (packageForm !== undefined && packageForm.$valid) {
                        flag = true;
                    }
                    else {
                        flag = false;
                        break;
                    }
                }
                if (!flag) {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteWarning,
                        body: $scope.TextValidation,
                        showCloseButton: true
                    });
                    return;
                }
            }



        if ($scope.BookingType !== "Draft") {
            $scope.BookingType = "JobPlaced";
        }
        if ($scope.BookingType === "Draft" && $scope.BBKBooking.ShipFrom.Country !== null && $scope.BBKBooking.ShipTo.Country !== null || ($scope.BookingType === 'Draft')) {
            if ($scope.BookingType === "Draft") {
                $scope.BBKBooking.POStatusId = 46;
                $scope.BBKBooking.PurchaseOrderDetail.JobStatusId = 49;

                //$rootScope.GetServiceValue = '';
                //AppSpinner.showSpinnerTemplate('Saving Shipment', $scope.Template);
            }
            else {
                $scope.BBKBooking.POStatusId = 45;
                $scope.BBKBooking.PurchaseOrderDetail.JobStatusId = 48;

                $scope.BBKBooking.BookingStatusType = "JobPlaced";
                //AppSpinner.showSpinnerTemplate('', $scope.Template);
            }

            BreakBulkService.SavePurchaseOrderData($scope.BBKBooking).then(function (response) {
                if (response.data !== undefined) {

                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteSuccess,
                        body: $scope.BookingSaveValidation,
                        showCloseButton: true
                    });

                }
            });
        }
    };

    var validateEmailPattern = function (email) {
        var emailFormat = /^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,10}$/;
        return emailFormat.test(email);
    };

    $scope.ValidateFromEmail = function (FromEmail) {
        if (FromEmail !== undefined && FromEmail !== null && FromEmail !== '') {
            if (FromEmail.includes(';')) {
                var emails = FromEmail.split(';');
                for (i = 0; i < emails.length; i++) {
                    if (emails[i] !== undefined && emails[i] !== null && emails[i] !== '') {
                        if (!validateEmailPattern(emails[i].trim())) {
                            $scope.frompattern = true;
                        }
                        else {
                            $scope.frompattern = false;
                        }
                    }
                }
            }
            else if (FromEmail.includes(',')) {
                var mail = FromEmail.split(',');
                for (i = 0; i < mail.length; i++) {
                    if (mail[i] !== undefined && mail[i] !== null && mail[i] !== '') {
                        if (!validateEmailPattern(mail[i].trim())) {
                            $scope.frompattern = true;
                        } else {
                            $scope.frompattern = false;
                        }
                    }
                }
            }
            else {
                if (!validateEmailPattern(FromEmail.trim())) {
                    $scope.frompattern = true;
                } else {
                    $scope.frompattern = false;
                }
            }
        }
    };

    $scope.ValidateToEmail = function (ToEmail) {
        if (ToEmail !== undefined && ToEmail !== null && ToEmail !== '') {
            if (ToEmail.includes(';')) {
                var emails = ToEmail.split(';');
                for (i = 0; i < emails.length; i++) {
                    if (emails[i] !== undefined && emails[i] !== null && emails[i] !== '') {
                        if (!validateEmailPattern(emails[i].trim())) {
                            $scope.topattern = true;
                        }
                        else {
                            $scope.topattern = false;
                        }
                    }
                }
            }
            else if (ToEmail.includes(',')) {
                var mail = ToEmail.split(',');
                for (i = 0; i < mail.length; i++) {
                    if (mail[i] !== undefined && mail[i] !== null && mail[i] !== '') {
                        if (!validateEmailPattern(mail[i].trim())) {
                            $scope.topattern = true;
                        } else {
                            $scope.topattern = false;
                        }
                    }
                }
            }
            else {
                if (!validateEmailPattern(ToEmail.trim())) {
                    $scope.topattern = true;
                } else {
                    $scope.topattern = false;
                }
            }
        }
    };

    //clone
    var getShipmentDetails = function () {
        BreakBulkService.GetBreakBulkBookingDetail($scope.shipmentId, "ShipmentClone").then(function (response) {
            if (response.data) {
                $scope.BBKBooking = response.data;
                $scope.customerId = response.data.CustomerId;   
                $scope.BBKBooking.PurchaseOrder.ShipmentType = response.data.PurchaseOrder.DefaultShipmentType;

                //$scope.SetShipinfo($scope.BBKBooking.ShipFrom.Country, "Shipper");
                //$scope.SetShipinfo($scope.BBKBooking.ShipTo.Country, "Receiver");
                
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.SomeErrorOccuredTryAgain,
                showCloseButton: true
            });
        });
    };

    var getScreenInitials = function () {
            BreakBulkService.GetInitials($scope.userInfo.EmployeeId).then(function (response) {
       //AppSpinner.hideSpinnerTemplate();

                    $scope.payments = response.data.PaymentParty;
                    $scope.factories = response.data.Factories;
                    $scope.CountriesRepo = TopCountryService.TopCountryList(response.data.Countries);
                    //$scope.ToCountry = TopCountryService.TopCountryList(response.data.Countries);
                    $scope.ShipmentMethod = response.data.ShipmentMethods;
                    $scope.Incoterms = response.data.Incoterm;
                    $scope.CountryPhoneCodes = response.data.CountryPhoneCodes;

            var shipmentId = 0;

            if ($stateParams.shipmentId) {
                shipmentId = parseInt($stateParams.shipmentId, 10);
            }

            if (shipmentId > 0) {
                getShipmentDetails();
            }
            else {
                newBooking();
                
            }
        },
            function (response) {
                AppSpinner.hideSpinnerTemplate();
                if (response.status !== 401) {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.InitialDataValidation,
                        showCloseButton: true
                    });
                }
            });
    };

    function init() {
        $scope.submitted = true;
        $scope.CompareDate = false;
        $rootScope.GetServiceValue = '';


        $scope.emailFormat = /^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,10};$/;

        setMultilingualOptions();

        $scope.TotalJobs = 0;
        $scope.ImagePath = config.BUILD_URL;
        $scope.breakbulkShow = false;
        $scope.addressShow = false;

        var userInfo = SessionService.getUser();
        $scope.userInfo = userInfo;
        userId = $scope.userInfo.EmployeeId;
        $scope.RoleId = userInfo.RoleId;


        if ($scope.RoleId === 3) {
            $scope.CustomerId = $scope.userInfo.EmployeeId;
            $scope.CreatedBy = $scope.userInfo.EmployeeId;


        }
        else if ($scope.RoleId === 1 || $scope.RoleId === 6) {
            $scope.CreatedBy = $scope.userInfo.EmployeeId;
            //$scope.CreatedOn = $scope.userInfo.EmployeeName;
        }

        $scope.CreatedBy = $scope.userInfo.EmployeeId;


        BreakBulkService.GetInitials(userId).then(function (response) {
            if (response.data !== undefined && response.data !== null && response.data !== '') {
                $scope.payments = response.data.PaymentParty;


                var dbCustomers = [];
                for (i = 0; i < $scope.payments.length; i++) {
                    if ($scope.payments[i].CustomerId) {
                        var dbr = $scope.payments[i].AccountNumber.split("");
                        var accno = "";
                        for (var j = 0; j < dbr.length; j++) {
                            accno = accno + dbr[j];
                            if (j == 2 || j == 5) {
                                accno = accno + "-";
                            }
                        }
                        $scope.payments[i].AccountNumber = accno;
                    }
                }

                $scope.factories = response.data.Factories;
                $scope.CountriesRepo = TopCountryService.TopCountryList(response.data.Countries);
                //$scope.ToCountry = TopCountryService.TopCountryList(response.data.Countries);
                $scope.ShipmentMethod = response.data.ShipmentMethods;
                $scope.Incoterms = response.data.Incoterm;
                $scope.CountryPhoneCodes = response.data.CountryPhoneCodes;

            }
            newBooking();
        });


        //clone
        if (SessionService.getUser()) {
            $scope.userInfo = SessionService.getUser();
        }
        else {
            //redirect to login page
            $state.go('login');
        }

        if ($stateParams.shipmentId && $stateParams.shipmentId !== "0") {
            $scope.shipmentId = $stateParams.shipmentId;
        }
        else {
            //newBooking();
        }
        if ($scope.userInfo.RoleId === 3) {
            $scope.customerId = $scope.userInfo.EmployeeId;
        }
        else {
            $scope.customerId = 0;
        }

        if (isNaN($stateParams.shipmentId)) {
            //$rootScope.isNotFound = true;
        }
        else {
            getScreenInitials();
        }


    }

    init();

});
