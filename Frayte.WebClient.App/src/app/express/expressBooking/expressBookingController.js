angular.module('ngApp.express').controller('ExpressBookingController', function ($scope, TradelaneBookingService, PostCodeService, $sce, $uibModalStack, UtilityService, Upload, $rootScope, AppSpinner, TopCountryService, $location, TopCurrencyService, config, $filter, $state, ModalService, $translate, $uibModal, $timeout, toaster, $stateParams, DirectBookingService, SessionService, TimeStringtoDateTime, ExpressBookingService) {

    $scope.setActualWeight = function () {
        $scope.expressBooking.ActualWeight = $scope.getTotalWeightKgs();
    };

    $scope.checkWeightValidation = function () {
        var fg = parseFloat($scope.expressBooking.ActualWeight);
        if (isNaN(fg)) {
            $scope.expressBooking.ActualWeight = null;
            return;
        }

        if (!fg) {
            $scope.expressBooking.ActualWeight = null;
            return;
        }
    };

    //Set Multilingual for Modal Popup
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteWarning', 'FrayteSuccess', 'Confirmation', 'SomeErrorOccuredTryAgain', 'InitialDataValidation', 'SelectCustomerAddressBook_Validation',
        'BookingSave_Validation', 'Select_Address_Form_First', 'Select_Address_To_First', 'Fill_Package_Information', 'SelectCustomer_Validation',
        'SelectCurrency_Validation', 'Sure_Clear_The_Form', 'Enter_Valid_AWB_Number_First', 'Select_From_Contry_Save_Draft', 'Select_To_Contry_Save_Draft',
        'Fill_AWB_To_Save_Shipment_Draft', 'Error_While_Placing_Booking_Try_Again', 'While_We_Placing_Booking', 'CorrectValidationErrorFirst']).then(function (translations) {
            $scope.FrayteError = translations.FrayteError;
            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.Confirmation = translations.Confirmation;
            $scope.SomeErrorOccuredTryAgain = translations.SomeErrorOccuredTryAgain;
            $scope.InitialDataValidation = translations.InitialDataValidation;
            $scope.SelectCustomerAddressBookValidation = translations.SelectCustomerAddressBook_Validation;
            $scope.BookingSaveValidation = translations.BookingSave_Validation;
            $scope.SelectCustomer_Validation = translations.SelectCustomer_Validation;
            $scope.Select_Address_Form_First = translations.Select_Address_Form_First;
            $scope.Select_Address_To_First = translations.Select_Address_To_First;
            $scope.Fill_Package_Information = translations.Fill_Package_Information;
            $scope.SelectCurrencyValidation = translations.SelectCurrency_Validation;
            $scope.Sure_Clear_The_Form = translations.Sure_Clear_The_Form;
            $scope.Enter_Valid_AWB_Number_First = translations.Enter_Valid_AWB_Number_First;
            $scope.Select_From_Contry_Save_Draft = translations.Select_From_Contry_Save_Draft;
            $scope.Select_To_Contry_Save_Draft = translations.Select_To_Contry_Save_Draft;
            $scope.Fill_AWB_To_Save_Shipment_Draft = translations.Fill_AWB_To_Save_Shipment_Draft;
            $scope.Error_While_Placing_Booking_Try_Again = translations.Error_While_Placing_Booking_Try_Again;
            $scope.While_We_Placing_Booking = translations.While_We_Placing_Booking;
            $scope.CorrectValidationErrorFirst = translations.CorrectValidationErrorFirst;

            // imageZoom("myimage", "myresult");
        });
    };

    //Optional Field
    $scope.shipFromToggleState = function (Country) {
        return TradelaneBookingService.toggleState(Country);
    };

    $scope.shipToToggleState = function (Country) {
        return TradelaneBookingService.toggleState(Country);
    };

    //search Postcode address
    $scope.SetPostCodeAddressValue = function (Type) {

        if ($scope.fillPostlValues && $scope.fillPostlValues.length) {
            if (Type === 'Shipper') {
                $scope.expressBooking.ShipFrom.City = $scope.fillPostlValues[0].City;
            }
            else if (Type === "Receiver") {
                $scope.expressBooking.ShipTo.City = $scope.fillPostlValues[0].City;
            }
        }
        else {
            if (Type === 'Shipper') {
                $scope.PostCodeAddressValue = false;
            }
            else if (Type === "Receiver") {
                $scope.ShipperPostCodeAddressValue = false;
            }
            else {
                $scope.PostCodeAddressValue = false;
                $scope.ShipperPostCodeAddressValue = false;
            }
        }
    };

    $scope.GetPostCodeAddress = function (PostCode, CountryCode2, Type) {
        if (PostCode && PostCode.length > 5 && ((Type === 'Shipper' && PostCode.length > $scope.expressBooking.ShipFrom.PostCode.length) || (Type === 'Receiver' && PostCode.length > $scope.expressBooking.ShipTo.PostCode.length)) && CountryCode2 && CountryCode2 === "GB") {
            $scope.disabled = true;
            AppSpinner.showSpinnerTemplate($scope.Loading_Postcode_Addresses, $scope.Template);
            return PostCodeService.AllPostCode(PostCode, CountryCode2).then(function (response) {
                $scope.disabled = false;
                $scope.PostCodeAddressValue = false;
                $scope.ReceiverPostCodeAddressValue = false;
                if (response) {
                    $scope.fillPostlValues = response;
                }
                else {
                    if (Type === 'Shipper') {
                        $scope.PostCodeAddressValue = true;
                    }
                    else if (Type === 'Receiver') {
                        $scope.ReceiverPostCodeAddressValue = true;
                    }
                }
                AppSpinner.hideSpinnerTemplate();
                return response;
            }, function () {
                $scope.disabled = false;
                if (Type === 'Shipper') {
                    $scope.PostCodeAddressValue = true;
                }
                else if (Type === 'Receiver') {
                    $scope.ReceiverPostCodeAddressValue = false;
                }
                AppSpinner.hideSpinnerTemplate();
            });
        }
    };

    $scope.onSelectPostCode = function ($item, $model, $label, $event, PostCode, Type) {
        if (PostCode && Type) {
            if (Type === 'Shipper') {
                $scope.expressBooking.ShipFrom.PostCode = $item.PostCode;
                $scope.expressBooking.ShipFrom.Address = $item.Address1;
                $scope.expressBooking.ShipFrom.Address2 = $item.Address2;
                $scope.expressBooking.ShipFrom.Area = $item.Area;
                $scope.expressBooking.ShipFrom.City = $item.City;
                $scope.expressBooking.ShipFrom.CompanyName = $item.CompanyName;
            }
            else if (Type === 'Receiver') {
                $scope.expressBooking.ShipTo.PostCode = $item.PostCode;
                $scope.expressBooking.ShipTo.Address = $item.Address1;
                $scope.expressBooking.ShipTo.Address2 = $item.Address2;
                $scope.expressBooking.ShipTo.Area = $item.Area;
                $scope.expressBooking.ShipTo.City = $item.City;
                $scope.expressBooking.ShipTo.CompanyName = $item.CompanyName;
            }
        }
    };

    //breakbulk product catalog code
    $scope.productCatalog = function () {
        if ($scope.CustomerDetail !== null && $scope.CustomerDetail !== undefined && $scope.CustomerDetail !== '' && $scope.CustomerDetail.CustomerId > 0 &&
            $scope.expressBooking.ShipTo.Country !== null && $scope.expressBooking.ShipTo.Country !== undefined && $scope.expressBooking.ShipTo.Country !== '' && $scope.expressBooking.ShipTo.Country.CountryId > 0) {
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
                        return $scope.CustomerDetail.CustomerId;
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

    //breakbulk map product catalog code
    $scope.mapProductCatalog = function (index) {
        if ($scope.CustomerDetail !== null && $scope.CustomerDetail !== undefined && $scope.CustomerDetail !== '' && $scope.CustomerDetail.CustomerId > 0) {
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
                            return $scope.CustomerDetail.CustomerId;
                        },
                        ModuleType: function () {
                            return 'ExpressBooking';
                        },
                        HubDetail: function () {
                            return $scope.hubAddress;
                        }
                    }
                });
                ModalInstance.result.then(function (ProductCatalog) {
                    if (ProductCatalog !== undefined && ProductCatalog !== null && ProductCatalog !== '') {
                        $scope.expressBooking.Packages[index].Length = ProductCatalog.Length;
                        $scope.expressBooking.Packages[index].Width = ProductCatalog.Width;
                        $scope.expressBooking.Packages[index].Height = ProductCatalog.Height;
                        $scope.expressBooking.Packages[index].Weight = ProductCatalog.Weight;
                        $scope.expressBooking.Packages[index].Value = ProductCatalog.DeclaredValue;
                        $scope.expressBooking.Packages[index].Content = ProductCatalog.ProductDescription;
                        $scope.expressBooking.Packages[index].ProductCatalogId = ProductCatalog.ProductcatalogId;
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

    $scope.expressDataa = {};

    var setCourierCompanyImage = function () {
        if ($scope.services.length) {
            for (var i = 0 ; i < $scope.services.length ; i++) {
                $scope.services[i].ImageURL = $scope.ImagePath + $scope.services[i].CarrierLogo;
            }
        }
    };

    // get Services
    $scope.expressServices = function (ExpressBookingForm) {

        if (!$scope.expressBooking.CustomerId) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.SelectCustomerAddressBookValidation,
                showCloseButton: true
            });

            return;
        }
        if (!$scope.expressBooking.ShipFrom.Country || !$scope.expressBooking.ShipFrom.Country.CountryId) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.Select_Address_Form_First,
                showCloseButton: true
            });

            return;
        }
        if (!$scope.expressBooking.ShipTo.Country || !$scope.expressBooking.ShipTo.Country.CountryId || !$scope.expressBooking.ShipTo.PostCode) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.Select_Address_To_First,
                showCloseButton: true
            });

            return;
        }

        var flag = true;
        for (var j = 0 ; j < $scope.expressBooking.Packages.length; j++) {
            var packageForm1 = ExpressBookingForm['tags' + j];
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
                title: $scope.FrayteWarning,
                body: $scope.Fill_Package_Information,
                showCloseButton: true
            });

            return;
        }
        if (!ExpressBookingForm.$valid) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.CorrectValidationErrorFirst,
                showCloseButton: true
            });
            return;
        }
        if (!$scope.expressBooking.CustomInformation.CustomsCertify) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.CorrectValidationErrorFirst,
                showCloseButton: true
            });
            return;
        }

        $scope.expressServiceObj = {
            FromCountryId: 0,
            FromPostCode: '',
            ToCountryId: 0,
            ToPostCode: '',
            TotalWeight: 0.00,
            CustomerId: 0

        };

        $scope.expressServiceObj.FromCountryId = $scope.expressBooking.ShipFrom.Country.CountryId;
        $scope.expressServiceObj.FromPostCode = $scope.expressBooking.ShipFrom.PostCode;
        $scope.expressServiceObj.ToCountryId = $scope.expressBooking.ShipTo.Country.CountryId;
        $scope.expressServiceObj.ToPostCode = $scope.expressBooking.ShipTo.PostCode;
        $scope.expressServiceObj.ToState = $scope.expressBooking.ShipTo.State !== undefined && $scope.expressBooking.ShipTo.State !== null && $scope.expressBooking.ShipTo.State !== "" ? $scope.expressBooking.ShipTo.State : "";
        $scope.expressServiceObj.TotalWeight = $scope.getTotalKgs();
        $scope.expressServiceObj.CustomerId = $scope.expressBooking.CustomerId;

        ExpressBookingService.ExpressHubServices($scope.expressServiceObj).then(function (response) {

            $scope.services = response.data;
            setCourierCompanyImage();
            if ($scope.services.length === 1) {
                $scope.expressBooking.Service = $scope.services[0];
            }
            else {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'express/expressService/expressService.tpl.html',
                    controller: 'ExpressServicesController',
                    windowClass: 'DirectBookingService',
                    size: 'lg',
                    backdrop: 'static',
                    resolve: {
                        ExpressBookingObj: function () {
                            return $scope.expressBooking;
                        }
                    }
                });

                modalInstance.result.then(function (customerRateCard) {
                    if (customerRateCard) {
                        $scope.expressBooking.Service = customerRateCard;
                    }
                }, function () {

                });
            }
        }, function () {
            console.log("Could not get services");
        });

    };

    // Customer Change
    $scope.setCustomerInfo = function (CustomerDetail) {
        $scope.expressBooking.CustomerId = CustomerDetail.CustomerId;
        $scope.expressBooking.AWBNumber = "";
    };

    $scope.SetValueNotZero = function (Value) {
        if (Value !== undefined && Value !== null && Value === "0") {
            return;
        }
        return Value;
    };

    function stopEnterKey(evt) {
        var evt1 = (evt) ? evt : ((event) ? event : null);
        var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
        if ((evt1.keyCode == 13) && (node.type == "text" || node.type == "checkbox" ||
                     node.type == "radio" || node.type == "email" || node.type == "number" || node.type == "textarea")) {
            return false;
        }
    }

    document.onkeypress = stopEnterKey;
    $scope.SetValueNotZero = function (Value) {
        if (Value !== undefined && Value !== null && Value === "0") {
            return;
        }
        return Value;
    };

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

    $scope.getTotalKgs = function () {
        if ($scope.expressBooking === undefined) {
            return;
        }
        else if ($scope.expressBooking.Packages === undefined || $scope.expressBooking.Packages === null) {
            return 0;
        }
        else if ($scope.expressBooking.Packages.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.expressBooking.Packages.length; i++) {
                var product = $scope.expressBooking.Packages[i];
                if (product.Weight === null || product.Weight === undefined) {
                    total += parseFloat(0);
                }
                else {
                    if (product.CartonValue === undefined || product.CartonValue === null) {
                        var carton = parseFloat(0);
                        total = total + parseFloat(product.Weight) * carton;
                    }
                    else {
                        total = total + parseFloat(product.Weight) * product.CartonValue;
                    }
                }
            }

            sum = total.toFixed(2);
            var num = [];
            num = sum.toString().split('.');

            if (num.length > 1) {
                var as = parseFloat(num[1]);
                if (as.toString().length === 1) {
                    as = as.toString() + "0";
                    as = parseFloat(as);
                }
                if (as === 0) {
                    return total.toFixed(2);
                }
                else if (as === 50) {
                    return total.toFixed(2);
                }
                else {
                    if (as > 49) {
                        var r = parseFloat(num[0]) + 1;
                        return r.toFixed(2);
                    }
                    else {
                        var s = parseFloat(num[0]) + 0.50;
                        return s.toFixed(2);
                    }
                }
            }
            else {
                return total.toFixed(2);
            }
        }
        else {
            return 0;
        }
    };

    $scope.totalPieces = function (directBooking) {
        if (directBooking !== undefined && directBooking !== null && directBooking.Packages !== null && directBooking.Packages.length) {
            var sum = 0;
            for (var i = 0; i < directBooking.Packages.length; i++) {
                if (directBooking.Packages[i].CartonValue !== "" && directBooking.Packages[i].CartonValue !== null && directBooking.Packages[i].CartonValue !== undefined) {
                    sum += Math.abs(parseInt(directBooking.Packages[i].CartonValue, 10));
                }
            }
            return sum;
        }
        else {
            return 0;
        }
    };

    $scope.GoToState = function () {
        $rootScope.State1 = false;
        $state.go('customer.direct-shipments');
    };

    var shipmentDetailTabValid = function (ExpressBookingForm) {
        if (ExpressBookingForm && $scope.expressBooking) {
            var flag = false;
            if ($scope.expressBooking.Service &&
                    ExpressBookingForm.paymentPartyCurrencyType !== undefined && ExpressBookingForm.paymentPartyCurrencyType.$valid &&
                    ExpressBookingForm.reference1 !== undefined && ExpressBookingForm.reference1.$valid &&
                    ExpressBookingForm.actualWeight && ExpressBookingForm.actualWeight.$valid
                ) {
                for (var j = 0 ; j < $scope.expressBooking.Packages.length; j++) {
                    var packageForm1 = ExpressBookingForm['tags' + j];
                    if (packageForm1 !== undefined && packageForm1.$valid) {
                        flag = true;
                    }
                    else {
                        flag = false;
                        break;
                    }
                }
            }
            else {
                if (
                    ExpressBookingForm.paymentPartyCurrencyType !== undefined && ExpressBookingForm.paymentPartyCurrencyType.$valid &&
                    ExpressBookingForm.reference1 !== undefined && ExpressBookingForm.reference1.$valid &&
                    ExpressBookingForm.actualWeight && ExpressBookingForm.actualWeight.$valid
                    ) {
                    for (var i = 0 ; i < $scope.expressBooking.Packages.length; i++) {
                        var packageForm = ExpressBookingForm['tags' + i];
                        if (packageForm !== undefined && packageForm.$valid) {
                            flag = true;
                        }
                        else {
                            flag = false;
                            break;
                        }
                    }
                }
                else {
                    flag = false;
                }
            }
            return flag;
        }
    };

    $scope.customInfoValidation = function (ExpressBookingForm, ContentType) {
        if (ExpressBookingForm && $scope.expressBooking) {
            flag = false;
            if (ExpressBookingForm.contentsType !== undefined && ExpressBookingForm.contentsType.$valid &&
                ExpressBookingForm.contentsExplanation !== undefined && ExpressBookingForm.contentsExplanation.$valid &&
                ExpressBookingForm.restrictionType !== undefined && ExpressBookingForm.restrictionType.$valid &&
                ExpressBookingForm.restrictionComments !== undefined && ExpressBookingForm.restrictionComments.$valid &&
                ExpressBookingForm.nonDeliveryOption !== undefined && ExpressBookingForm.nonDeliveryOption.$valid &&
                ExpressBookingForm.customsSigner !== undefined && ExpressBookingForm.customsSigner.$valid &&
                $scope.expressBooking.CustomInformation.CustomsCertify) {
                flag = true;
            }
            else {
                flag = false;
            }
            return flag;
        }
    };

    $scope.shipToValidation = function (ExpressBookingForm) {
        if (ExpressBookingForm && $scope.expressBooking) {
            if ($scope.expressBooking.ShipTo.Country !== undefined && $scope.expressBooking.ShipTo.Country !== null &&
                ($scope.expressBooking.ShipTo.Country.Code === 'CAN' || $scope.expressBooking.ShipTo.Country.Code === 'USA' || $scope.expressBooking.ShipTo.Country.Code === 'AUS')) {
                if (ExpressBookingForm.shipToAddress1 && ExpressBookingForm.shipToAddress1.$valid &&
                    ExpressBookingForm.shipToCompanyName && ExpressBookingForm.shipToCompanyName.$valid &&
                    ExpressBookingForm.shipToFirstName && ExpressBookingForm.shipToFirstName.$valid &&
                    ExpressBookingForm.shipToLastName && ExpressBookingForm.shipToLastName.$valid &&
                    ExpressBookingForm.shipToCountry && ExpressBookingForm.shipToCountry.$valid &&
                    ExpressBookingForm.shipToPostcode && ExpressBookingForm.shipToPostcode.$valid &&
                    ExpressBookingForm.shipToCountryState && ExpressBookingForm.shipToCountryState.$valid &&
                    ExpressBookingForm.shipToCityName && ExpressBookingForm.shipToCityName.$valid &&
                    ExpressBookingForm.shipToPhone && ExpressBookingForm.shipToPhone.$valid &&
                    ExpressBookingForm.shipToMail && ExpressBookingForm.shipToMail.$valid
                    ) {
                    flag = true;
                }
                else {
                    flag = false;
                }
                return flag;
            }
            else {
                if (ExpressBookingForm.shipToAddress1 && ExpressBookingForm.shipToAddress1.$valid &&
                    ExpressBookingForm.shipToCompanyName && ExpressBookingForm.shipToCompanyName.$valid &&
                    ExpressBookingForm.shipToFirstName && ExpressBookingForm.shipToFirstName.$valid &&
                    ExpressBookingForm.shipToLastName && ExpressBookingForm.shipToLastName.$valid &&
                    ExpressBookingForm.shipToCountry && ExpressBookingForm.shipToCountry.$valid &&
                    ExpressBookingForm.shipToPostcode && ExpressBookingForm.shipToPostcode.$valid &&
                    ExpressBookingForm.shipToState && ExpressBookingForm.shipToState.$valid &&
                    ExpressBookingForm.shipToCityName && ExpressBookingForm.shipToCityName.$valid &&
                    ExpressBookingForm.shipToPhone && ExpressBookingForm.shipToPhone.$valid &&
                    ExpressBookingForm.shipToMail && ExpressBookingForm.shipToMail.$valid
                    ) {
                    flag = true;
                }
                else {
                    flag = false;
                }
                return flag;
            }
        }
    };

    $scope.shipFormValidation = function (ExpressBookingForm) {
        if (ExpressBookingForm && $scope.expressBooking) {
            if ($scope.expressBooking.ShipFrom.Country !== undefined && $scope.expressBooking.ShipFrom.Country !== null &&
                ($scope.expressBooking.ShipFrom.Country.Code === 'CAN' || $scope.expressBooking.ShipFrom.Country.Code === 'USA' || $scope.expressBooking.ShipFrom.Country.Code === 'AUS')) {
                if (ExpressBookingForm.shipFromAddress1 && ExpressBookingForm.shipFromAddress1.$valid &&
                    ExpressBookingForm.shipFromCompanyName && ExpressBookingForm.shipFromCompanyName.$valid &&
                    ExpressBookingForm.shipFromFirstName && ExpressBookingForm.shipFromFirstName.$valid &&
                    ExpressBookingForm.shipFromLastName && ExpressBookingForm.shipFromLastName.$valid &&
                    ExpressBookingForm.shipFromCountry && ExpressBookingForm.shipFromCountry.$valid &&
                    ExpressBookingForm.shipFromPostcode && ExpressBookingForm.shipFromPostcode.$valid &&
                    ExpressBookingForm.shipFromCountryState && ExpressBookingForm.shipFromCountryState.$valid &&
                    ExpressBookingForm.shipFromCityName && ExpressBookingForm.shipFromCityName.$valid &&
                    ExpressBookingForm.shipFromPhone && ExpressBookingForm.shipFromPhone.$valid &&
                    ExpressBookingForm.shipFromMail && ExpressBookingForm.shipFromMail.$valid
                    ) {
                    flag = true;
                }
                else {
                    flag = false;
                }
                return flag;
            }
            else {
                if (ExpressBookingForm.shipFromAddress1 && ExpressBookingForm.shipFromAddress1.$valid &&
                    ExpressBookingForm.shipFromCompanyName && ExpressBookingForm.shipFromCompanyName.$valid &&
                    ExpressBookingForm.shipFromFirstName && ExpressBookingForm.shipFromFirstName.$valid &&
                    ExpressBookingForm.shipFromLastName && ExpressBookingForm.shipFromLastName.$valid &&
                    ExpressBookingForm.shipFromCountry && ExpressBookingForm.shipFromCountry.$valid &&
                    ExpressBookingForm.shipFromPostcode && ExpressBookingForm.shipFromPostcode.$valid &&
                    ExpressBookingForm.shipFromState && ExpressBookingForm.shipFromState.$valid &&
                    ExpressBookingForm.shipFromCityName && ExpressBookingForm.shipFromCityName.$valid &&
                    ExpressBookingForm.shipFromPhone && ExpressBookingForm.shipFromPhone.$valid &&
                    ExpressBookingForm.shipFromMail && ExpressBookingForm.shipFromMail.$valid
                    ) {
                    flag = true;
                }
                else {
                    flag = false;
                }
                return flag;
            }
        }
    };

    $scope.shipToTab = function (ExpressBookingForm) {
        if (ExpressBookingForm && $scope.directBooking) {
            return shipToTabValid(ExpressBookingForm);
        }
    };

    $scope.currencyVisibility = function (directBooking) {
        if (directBooking !== undefined && directBooking !== null &&
          directBooking.ShipFrom !== undefined && directBooking.ShipFrom !== null &&
          directBooking.ShipTo !== undefined && directBooking.ShipTo !== null &&
          directBooking.ShipFrom.Country !== undefined && directBooking.ShipFrom.Country !== null &&
          directBooking.ShipTo.Country !== undefined && directBooking.ShipTo.Country !== null &&
          directBooking.ShipFrom.Country.Code === 'GBR' && directBooking.ShipTo.Country.Code === 'GBR') {

            return false;
        }
        else {
            return true;
        }
    };

    $scope.shipmentDetailTab = function (ExpressBookingForm) {
        if (ExpressBookingForm !== undefined) {
            return shipmentDetailTabValid(ExpressBookingForm);
        }
    };

    $scope.shipFromOptionalField = function (action) {
        if (action !== undefined && action !== null && action !== '' && action === 'State') {
            if ($scope.expressBooking !== undefined && $scope.expressBooking !== null && $scope.expressBooking.ShipFrom !== null && $scope.expressBooking.ShipFrom.Country !== null) {
                if ($scope.expressBooking.ShipFrom.Country.Code === 'HKG' || $scope.expressBooking.ShipFrom.Country.Code === 'GBR') {
                    return false;
                }
                else if ($scope.expressBooking.ShipFrom.Country.Code === 'CAN' || $scope.expressBooking.ShipFrom.Country.Code === 'USA' || $scope.expressBooking.ShipFrom.Country.Code === 'AUS') {
                    return false;
                }
                else {
                    return true;
                }
            }
        }
        else if (action !== undefined && action !== null && action !== '' && action === 'PostCode') {
            if ($scope.expressBooking !== undefined && $scope.expressBooking !== null && $scope.expressBooking.ShipFrom !== null && $scope.expressBooking.ShipFrom.Country !== null) {
                if ($scope.expressBooking.ShipFrom.Country.Code !== 'HKG') {
                    return true;
                }
                else {
                    $scope.expressBooking.ShipFrom.PostCode = '';
                    return false;
                }
            }
        }
    };

    $scope.shipToOptionalField = function (action) {
        if (action !== undefined && action !== null && action !== '' && action === 'State') {
            if ($scope.expressBooking !== undefined && $scope.expressBooking !== null && $scope.expressBooking.ShipTo !== null && $scope.expressBooking.ShipTo.Country !== null) {
                if ($scope.expressBooking.ShipTo.Country.Code === 'HKG' || $scope.expressBooking.ShipTo.Country.Code === 'GBR') {
                    return false;
                }
                else if ($scope.expressBooking.ShipTo.Country.Code === 'CAN' || $scope.expressBooking.ShipTo.Country.Code === 'USA' || $scope.expressBooking.ShipTo.Country.Code === 'AUS') {
                    return false;
                }
                else {
                    return true;
                }
            }
        }
        else if (action !== undefined && action !== null && action !== '' && action === 'PostCode') {
            if ($scope.expressBooking !== undefined && $scope.expressBooking !== null && $scope.expressBooking.ShipTo !== null && $scope.expressBooking.ShipTo.Country !== null) {
                if ($scope.expressBooking.ShipTo.Country.Code !== 'HKG') {
                    return true;
                }
                else {
                    $scope.directBooking.ShipTo.PostCode = '';
                    return true;
                }
            }
        }
    };

    $scope.getTotalWeightKgs = function () {
        if ($scope.expressBooking === undefined) {
            return;
        }
        else if ($scope.expressBooking.Packages === undefined || $scope.expressBooking.Packages === null) {
            return 0;
        }
        else if ($scope.expressBooking.Packages.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.expressBooking.Packages.length; i++) {
                var product = $scope.expressBooking.Packages[i];
                if (product.Weight === null || product.Weight === undefined) {
                    total += parseFloat(0);
                }
                else {
                    if (product.CartonValue === undefined || product.CartonValue === null) {
                        var catroon = parseFloat(0);
                        total = total + parseFloat(product.Weight) * catroon;
                    }
                    else {
                        total = total + parseFloat(product.Weight) * product.CartonValue;
                    }
                }
            }
            return parseFloat(total).toFixed(2);
        }
        else {
            return 0;
        }
    };

    $scope.getChargeableWeight = function (items, prop) {
        if ($scope.directBooking === undefined) {
            return;
        }

        else if ($scope.directBooking.Packages === undefined || $scope.directBooking.Packages === null) {
            return 0;
        }

        if ($scope.directBooking.Packages.length >= 0) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.directBooking.Packages.length; i++) {
                var product = $scope.directBooking.Packages[i];
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
                    if ($scope.directBooking.PakageCalculatonType === "kgToCms") {
                        total += ((len * wid * height) / 5000) * qty;
                    }
                    else if ($scope.directBooking.PakageCalculatonType === "lbToInchs") {
                        total += ((len * wid * height) / 166) * qty;
                    }
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
                    if (as.length === 1) {
                        as = as + "0";
                        as = parseFloat(as);
                    }
                    if (as === 0) {
                        if (kgs > parseFloat(num[0]).toFixed(2)) {
                            return $scope.getTotalKgs();
                        }
                        else {
                            return parseFloat(num[0]).toFixed(2);
                        }
                    }
                    else {
                        if (as > 49) {
                            var r = parseFloat(num[0]) + 1;
                            if (kgs > r.toFixed(2)) {
                                return $scope.getTotalKgs();
                            }
                            else {
                                return r.toFixed(2);
                            }
                        }
                        else {
                            var s = parseFloat(num[0]) + 0.50;
                            if (kgs > s.toFixed(2)) {
                                return $scope.getTotalKgs();
                            }
                            else {
                                return s.toFixed(2);
                            }
                        }
                    }
                }
                else {
                    if (kgs > parseFloat(num[0]).toFixed(2)) {
                        return $scope.getTotalKgs();
                    }
                    else {
                        return parseFloat(num[0]).toFixed(2);
                    }
                }
            }
        }
    };

    $scope.setCustomContentDescription = function (ContentDescription, CustomType) {
        if (CustomType !== undefined && CustomType === 'ContentExplanation') {
            if (ContentDescription !== undefined && ContentDescription !== null && ContentDescription !== '') {
                if ($scope.ShowCustomInfoSection($scope.directBooking)) {
                    $scope.directBooking.CustomInfo.ContentsExplanation = ContentDescription;
                }
            }
        }
        else if (CustomType !== undefined && CustomType === 'RestrictionExplanation') {
            if (ContentDescription !== undefined && ContentDescription !== null && ContentDescription !== '') {
                if ($scope.ShowCustomInfoSection($scope.directBooking)) {
                    $scope.directBooking.CustomInfo.RestrictionComments = ContentDescription;
                }
            }
        }
    };

    // New Booking Json 
    var newBooking = function () {
        $scope.expressBooking = {
            ExpressId: 0,
            OperationZoneId: 0,
            CustomerId: $scope.customerId,
            AWBNumber: '',
            CustomerAccountNumber: null,
            ShipmentStatusId: $scope.ShipmentStatus.Draft,
            CreatedBy: $scope.loggedInUserId,
            CreatedOnUtc: new Date(),
            ShipFrom: {
                ExpressAddressId: 0,
                Country: null,
                PostCode: "",
                FirstName: "",
                LastName: "",
                CompanyName: "",
                Address1: "",
                Address2: "",
                City: "",
                State: "",
                Area: "",
                Phone: "",
                Email: "",
                IsDefault: false,
                IsMailSend: false
            },
            ShipTo: {
                ExpressAddressId: 0,
                Country: null,
                PostCode: "",
                FirstName: "",
                LastName: "",
                CompanyName: "",
                Address1: "",
                Address2: "",
                City: "",
                State: "",
                Area: "",
                Phone: "",
                Email: "",
                IsDefault: false,
                IsMailSend: false
            },
            CustomInformation: {
                ShipmentCustomDetailId: 0,
                ContentsType: null,
                ContentsExplanation: 'Goods',
                RestrictionType: $scope.RestrictionType[0].value,
                RestrictionComments: 'N/A',
                CustomsCertify: false,
                CustomsSigner: '',
                NonDeliveryOption: $scope.NonDeliveryOption[1].value
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
            PayTaxAndDuties: "Receiver",
            TaxAndDutyAccountNumber: '',
            CustomsSigner: '',
            TaxAndDutyAcceptedBy: '',
            Currency: null,
            Packages: [{
                ExpressDetailId: 0,
                ExpressId: 0,
                CartonValue: null,
                Length: null,
                Width: null,
                Height: null,
                Weight: null,
                Value: null,
                Content: null,
                ProductCatalogId: 0
            }],
            ActualWeight: null,
            ShipmentHandlerMethod: null,
            PakageCalculatonType: 'kgToCms',
            UpdatedBy: 0,
            UpdatedOnUtc: new Date(),
            FrayteNumber: '',
            LogisticType: '',
            ShipmentReference: '',
            ShipmentDescription: '',
            DeclaredValue: null,
            DeclaredCurrency: null,
            TotalEstimatedWeight: null,
            DangerousGoods: true,
            ShipmentAdditionalInfo: '',
            AdditionalInfo: '',
            ParcelType: $scope.ParcelTypes[0]
        };
    };

    $scope.checkTime = function (ExpressBookingForm) {
        if (ExpressBookingForm && $scope.directBooking && $scope.directBooking.ReferenceDetail && $scope.directBooking.ReferenceDetail) {
            var d = new Date();
            var min = "";
            if (d.getMinutes().toString().length === 1) {
                min = "0" + d.getMinutes().toString();
            }
            else {
                min = d.getMinutes().toString();
            }
            var str = d.getHours().toString() + min;

            var colDate = $scope.directBooking.ReferenceDetail.CollectionDate;
            var dd = new Date(colDate);
            var month = '';
            if (dd.getMonth().toString().length === 1) {
                month = "0" + dd.getMonth().toString();
            }
            else {
                month = dd.getMonth().toString();
            }

            var dateString1 = month + "-" + dd.getDate().toString() + "-" + dd.getFullYear().toString();
            var dateString2 = d.getMonth().toString() + "-" + d.getDate().toString() + "-" + d.getFullYear().toString();

            var CollectionDate = new Date(dateString1);
            var CurrentDate = new Date(dateString2);
            if (CollectionDate < CurrentDate) {
                ExpressBookingForm.collectiontime.$dirty = true;
                $scope.isCollectionTimeValid = false;
                return true;
            }
            else {
                $scope.isCollectionTimeValid = true;
                return false;
            }
        }
    };

    $scope.AddPackage = function () {
        $scope.HideContent = true;
        $scope.expressBooking.Packages.push({
            ExpressDetailId: 0,
            CartonValue: null,
            Length: null,
            Width: null,
            Height: null,
            Weight: null,
            Value: null,
            Content: '',
            ProductCatalogId: 0
        });
        var dbpac = $scope.expressBooking.Packages.length - 1;
        for (i = 0; i < $scope.expressBooking.Packages.length; i++) {

            if (i === dbpac) {
                $scope.expressBooking.Packages[i].pacVal = true;
            }
            else {
                $scope.expressBooking.Packages[i].pacVal = false;
            }
        }
    };

    $scope.RemovePackage = function (Package) {
        if (Package !== undefined && Package !== null) {
            var index = $scope.expressBooking.Packages.indexOf(Package);
            if ($scope.expressBooking.Packages.length === 2) {
                $scope.HideContent = false;
            }
            if (Package.Expres > 0) {

            }
            else {
                if (index === $scope.expressBooking.Packages.length - 1) {
                    var dbpac = $scope.expressBooking.Packages.length - 2;
                    for (i = 0; i < $scope.expressBooking.Packages.length; i++) {
                        if (i === dbpac) {
                            $scope.expressBooking.Packages[i].pacVal = true;
                        }
                        else {
                            $scope.expressBooking.Packages[i].pacVal = false;
                        }
                    }
                }
                else {
                    var dbpac1 = $scope.expressBooking.Packages.length - 1;
                    for (i = 0; i < $scope.expressBooking.Packages.length; i++) {
                        if (i === dbpac1) {
                            $scope.expressBooking.Packages[i].pacVal = true;
                        }
                        else {
                            $scope.expressBooking.Packages[i].pacVal = false;
                        }
                    }
                }
                $scope.expressBooking.Packages.splice(index, 1);
                $scope.Packges = angular.copy($scope.expressBooking.Packages);
                $scope.expressBooking.Packages = [];
                $scope.expressBooking.Packages = $scope.Packges;
            }
        }
    };

    $scope.PackagesTotal = function (items, prop) {
        if (items === null || items === undefined) {
            return 0;
        }
        else {
            return items.reduce(function (a, b) {
                var convertB = 0.0;
                if (b[prop] !== undefined && b[prop] !== null) {
                    convertB = parseFloat(b[prop]);
                }
                var convertA = 0.0;
                if (a !== undefined && a !== null) {
                    convertA = parseFloat(a);
                }

                convertc = convertA + convertB;
                var f = convertc.toFixed(2);
                var swd = Number(parseFloat(convertc).toFixed(2)).toLocaleString('en', {
                    minimumFractionDigits: 2
                });

                return f;

            }, 0);
        }
    };

    $scope.GetServices = function (directBooking, ExpressBookingForm) {

        $scope.RemainFieldLength = remainedFieldsPopup(directBooking);

        if (directBooking !== undefined) {
            $rootScope.GetServiceValue = true;
        }
        if (directBooking !== undefined && ExpressBookingForm !== undefined &&
           directBooking !== null &&
           directBooking.ShipFrom !== undefined &&
           directBooking.ShipFrom !== null &&
           directBooking.ShipTo !== undefined &&
           directBooking.ShipTo !== null &&
           directBooking.ShipFrom.Country !== null &&
           directBooking.ShipTo.Country !== null &&
           directBooking.Packages !== undefined &&
           directBooking.Packages !== null &&
           $scope.RemainFieldLength.length === 0) {

            var weightTotal = $scope.PackagesTotal(directBooking.Packages, 'Weight');
            weightTotal = parseFloat(weightTotal);
            var flag = false;
            for (var i = 0 ; i < $scope.directBooking.Packages.length; i++) {
                var packageForm = ExpressBookingForm['tags' + i];
                if (packageForm !== undefined && packageForm.$valid) {
                    flag = true;
                }
                else {
                    flag = false;
                    break;
                }
            }

            if (weightTotal === 0 || !flag) {
                //toaster.pop({
                //    type: 'warning',
                //    title: $scope.TitleFrayteWarning,
                //    body: $scope.GetServiceValidation,
                //    showCloseButton: true
                //});
                //$scope.RemainFieldLength = remainedFieldsPopup(directBooking);
                return;
            }
            if (directBooking.CustomerId === null || directBooking.CustomerId === 0) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarning,
                    body: $scope.SelectCustomerValidation,
                    showCloseButton: true
                });

                return;
            }
            if (directBooking.Currency === null || directBooking.Currency === undefined) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarning,
                    body: $scope.SelectCurrencyValidation,
                    showCloseButton: true
                });

                return;
            }

            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'directBooking/directBookingServices/directBookingGetService.tpl.html',
                controller: 'DirectBookingServicesController',
                windowClass: 'DirectBookingService',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    directBookingObj: function () {
                        return directBooking;
                    },
                    IsRateShow: function () {
                        return $scope.IsRateShow;
                    },
                    CustomerDetail: function () {
                        return $scope.CustomerDetail;
                    },
                    AddressType: function () {
                        if ($scope.LogisticCompany !== undefined && $scope.LogisticCompany !== null && $scope.LogisticCompany !== '') {
                            if ($scope.LogisticCompany === 'Yodel') {
                                return $scope.AddressType;
                            }
                        }
                        else {
                            return '';
                        }
                    },
                    LogisticService: function () {
                        return $scope.LogisticCompany;
                    },
                    CallingFrom: function () {
                        return 'DirectBooking';
                    }
                }
            });

            modalInstance.result.then(function (customerRateCard) {
                if (customerRateCard !== undefined && customerRateCard !== null) {
                    $scope.directBooking.CustomerRateCard = customerRateCard;
                    $scope.ContentCount(directBooking.Packages);
                    for (i = 0; i < $scope.RestrictionType.length; i++) {
                        if ($scope.RestrictionType[0].value === $scope.RestrictionType[i].value) {
                            $scope.directBooking.CustomInfo.RestrictionType = $scope.RestrictionType[i].value;
                        }
                    }
                    $scope.directBooking.CustomInfo.RestrictionComments = 'N/A';
                    $scope.directBooking.CustomInfo.ContentsExplanation = $scope.Contentfinallength;
                    $scope.checkcustomerRateCard = customerRateCard;

                    if ($scope.directBooking.CustomerRateCard.LogisticServiceId > 0) {
                        for (var aa = 0; aa < $scope.ShipmentMethods.length; aa++) {

                        }
                        if ($scope.ShowCustomInfoSection($scope.directBooking) && $scope.directBooking.CustomerRateCard.LogisticServiceId > 0) {
                            $scope.active = 2;
                        }
                    }
                    if ($scope.directBooking.CustomerRateCard.LogisticServiceId > 0 && ($scope.directBooking.CustomerRateCard.CourierName === "DHL" || $scope.directBooking.CustomerRateCard.CourierName === "TNT" || $scope.directBooking.CustomerRateCard.CourierName === "UPS")) {
                        if ($scope.directBooking.ReferenceDetail.CollectionDate === null || $scope.directBooking.ReferenceDetail.CollectionDate === undefined) {
                            //$scope.directBooking.ReferenceDetail.CollectionDate = new Date();
                        }
                        else {
                            $scope.directBooking.ReferenceDetail.CollectionDate = null;
                            $scope.directBooking.ReferenceDetail.CollectionTime = "";
                        }

                        var modalInstance = $uibModal.open({
                            animation: true,
                            templateUrl: 'directBooking/directBookingCollectionDateTime/directBookingCollectionDateTime.tpl.html',
                            controller: 'DirectBookingCollectionDateTimeController',
                            windowClass: 'DirectBookingService',
                            size: 'md',
                            backdrop: 'static',
                            resolve: {
                                PackageLength: function () {
                                    return $scope.directBooking.Packages.length;
                                }
                            }
                        });
                        modalInstance.result.then(function (ReferenceDetail) {
                            if (ReferenceDetail) {
                                $scope.directBooking.ReferenceDetail.CollectionDate = ReferenceDetail.CollectionDate;
                                $scope.directBooking.ReferenceDetail.CollectionTime = ReferenceDetail.CollectionTime;
                            }
                            $scope.BookingType = ReferenceDetail.BookingStatus;
                            if (ReferenceDetail.Type === "Confirm") {
                                $scope.ButtonValue = "PlaceBooking";
                                $scope.PlaceBooking($scope.ExpressBookingForm, false, $scope.directBooking);
                            }
                        }, function (ReferenceDetail) {
                        });
                    }

                    //setScroll("pull-down");
                    //remainedFieldsPopup(directBooking);
                    window.scrollTo(0, 1500);

                }
            }, function () {

            });
        }
        else {
            //toaster.pop({
            //    type: 'warning',
            //    title: $scope.TitleFrayteWarning,
            //    body: $scope.GetServiceValidation,
            //    showCloseButton: true
            //});
        }
    };

    $scope.cancelServices = function () {
        $scope.expressBooking.Service = {
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

    var clearDirectBookingForm = function (form) {

        form.$setPristine();
        form.$setUntouched();
        newBooking();
        $scope.active = 3;

        $scope.CustomerDetail = null;

        if ($scope.RoleId === 3) {
            $scope.expressBooking.CustomerId = $scope.customerId;
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

    //Change lb to kg Package Details
    $scope.changeKgToLb = function (pieceDetailOption) {
        if (pieceDetailOption === "kgToCms") {
            // for kg
            $scope.isdisabled = false;

            $translate('kGS').then(function (kGS) {
                $scope.Lb_Kgs = kGS;
            });
            $translate('KG').then(function (KG) {

                $scope.Lb_Kg = KG;
            });
            $translate('CMS').then(function (CMS) {
                $scope.Lb_Inch = CMS;
            });
        }
        if (pieceDetailOption === "lbToInchs") {
            // for LB
            $scope.isdisabled = false;
            $translate('LB').then(function (LB) {
                $scope.Lb_Kgs = LB;
            });
            $translate('LBs').then(function (LBs) {

                $scope.Lb_Kg = LBs;
            });
            $translate('INCHS').then(function (Inchs) {
                $scope.Lb_Inch = Inchs;
            });
        }
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

    //scroll to top
    window.scrollTo(0, 0);

    var getAWBImage = function () {

        ExpressBookingService.AWBlabelPath($scope.expressBooking.AWBNumber).then(function (response) {
            $scope.AWBImagePath = response.data;
        }, function (error) {
        });

    };

    $scope.onSelectAWB = function ($item, $model, $label, $event, Shipper) {
        $scope.expressBooking.AWBNumber = $item;
        getAWBImage();

        if (!$scope.expressBooking.CustomerId) {
            var arr = $scope.expressBooking.AWBNumber.split(' ');
            if (arr.length == 4) {
                var accountNumbner = arr[0];
                for (var i = 0 ; i < $scope.customers.length; i++) {
                    var arr1 = $scope.customers[i].AccountNumber.split('-');

                    if (arr1.length === 3) {
                        if (arr1[0] === accountNumbner) {
                            $scope.CustomerDetail = $scope.customers[i];
                            break;
                        }
                    }
                    //if ($scope.customers[i].CustomerAccountNumberR === accountNumbner) {
                    //    $scope.CustomerDetail = $scope.customers[i];
                    //    break; 
                    //}
                }
                if ($scope.CustomerDetail) {
                    $scope.expressBooking.CustomerId = $scope.CustomerDetail.CustomerId;
                }
            }
        }
    };

    $scope.HUBAddress = function (Type) {
        if ($scope.fillPostlValues && $scope.fillPostlValues.length) {
            if (Type === 'Shipper') {
                $scope.expressBooking.ShipFrom.City = $scope.fillPostlValues[0].City;
            }
            else if (Type === "Receiver") {
                $scope.expressBooking.ShipTo.City = $scope.fillPostlValues[0].City;
            }
        }
        if ($scope.expressBooking.ShipTo && $scope.expressBooking.ShipTo.Country) {
            getHubAddress();

        }
    };

    var getHubAddress = function () {

        var Postcode = "";
        var State = "";

        if (!$scope.expressBooking.ShipTo.PostCode) {
        }
        else {
            Postcode = $scope.expressBooking.ShipTo.PostCode;
        }

        if (!$scope.expressBooking.ShipTo.State) {
        }
        else {
            State = $scope.expressBooking.ShipTo.State;
        }

        ExpressBookingService.getHubAddress($scope.expressBooking.ShipTo.Country.CountryId, Postcode, State).then(function (response) {

            $scope.hubAddress = response.data;

        }, function (error) {

        });
        if ($scope.expressBooking.ShipTo.Country.CountryId === 38) {
            $scope.GetCountryStates();
        }
        if ($scope.expressBooking.ShipTo.Country.CountryId === 229) {
            $scope.GetCountryUSAStates();
        }
    };

    //FromCountryStateList
    $scope.GetFromCountryStates = function () {
        if ($scope.expressBooking.ShipFrom.Country !== undefined && $scope.expressBooking.ShipFrom.Country !== null && $scope.expressBooking.ShipFrom.Country !== '') {
            ExpressBookingService.GetFromCountryState($scope.expressBooking.ShipFrom.Country.CountryId).then(function (response) {
                $scope.CountryFromStateList = response.data;
            }, function (error) {
            });
        }
    };

    //ToCountryStateList
    $scope.GetToCountryStates = function () {
        if ($scope.expressBooking.ShipTo.Country !== undefined && $scope.expressBooking.ShipTo.Country !== null && $scope.expressBooking.ShipTo.Country !== '') {
            ExpressBookingService.GetFromCountryState($scope.expressBooking.ShipTo.Country.CountryId).then(function (response) {
                $scope.CountryToStateList = response.data;
            }, function (error) {
            });
        }
    };

    //ToCountryStateList
    $scope.GetCountryStates = function () {
        if ($scope.expressBooking.ShipTo.Country !== undefined && $scope.expressBooking.ShipTo.Country !== null && $scope.expressBooking.ShipTo.Country !== '') {
            ExpressBookingService.GetCountryState($scope.expressBooking.ShipTo.Country.CountryId).then(function (response) {
                $scope.CountryStateList = response.data;
            }, function (error) {
            });
        }
    };

    //ToCountryStateList
    $scope.GetCountryUSAStates = function () {
        ExpressBookingService.GetFromCountryState($scope.expressBooking.ShipTo.Country.CountryId).then(function (response) {
            $scope.CountryStateList = response.data;
            if ($scope.CountryStateList.length > 0) {
                for (i = 0; i < $scope.CountryStateList.length; i++) {
                    $scope.CountryStateList[i].StateDisplay = $scope.CountryStateList[i].State;
                }
            }
        }, function (error) {
        });
    };

    var resetAddress = function (type) {
        if (type === "Shipper") {
            $scope.expressBooking.ShipFrom.PostCode = "";
            $scope.expressBooking.ShipFrom.FirstName = "";
            $scope.expressBooking.ShipFrom.LastName = "";
            $scope.expressBooking.ShipFrom.CompanyName = "";
            $scope.expressBooking.ShipFrom.Address1 = "";
            $scope.expressBooking.ShipFrom.Address2 = "";
            $scope.expressBooking.ShipFrom.City = "";
            $scope.expressBooking.ShipFrom.State = "";
            $scope.expressBooking.ShipFrom.Area = "";
            $scope.expressBooking.ShipFrom.Phone = "";
        }
        if (type === "Receiver") {

            $scope.expressBooking.ShipTo.PostCode = "";
            $scope.expressBooking.ShipTo.FirstName = "";
            $scope.expressBooking.ShipTo.LastName = "";
            $scope.expressBooking.ShipTo.CompanyName = "";
            $scope.expressBooking.ShipTo.Address1 = "";
            $scope.expressBooking.ShipTo.Address2 = "";
            $scope.expressBooking.ShipTo.City = "";
            $scope.expressBooking.ShipTo.State = "";
            $scope.expressBooking.ShipTo.Area = "";
            $scope.expressBooking.ShipTo.Phone = "";
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
                for (var i = 0 ; i < $scope.CountryPhoneCodes.length ; i++) {
                    if ($scope.CountryPhoneCodes[i].CountryCode === Country.Code) {
                        $scope.ShipFromPhoneCode = "(+" + $scope.CountryPhoneCodes[i].PhoneCode + ")";
                        break;
                    }
                }
                if (!Type) {
                    resetAddress(Action);
                }
                if ($scope.expressBooking.ShipFrom.Country.CountryId === 38 || $scope.expressBooking.ShipFrom.Country.CountryId === 229) {
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
                for (var j = 0 ; j < $scope.CountryPhoneCodes.length ; j++) {
                    if ($scope.CountryPhoneCodes[j].Name === Country.Name) {
                        $scope.ShipToPhoneCode = "(+" + $scope.CountryPhoneCodes[j].PhoneCode + ")";
                        break;
                    }
                }
                if (!Type) {
                    resetAddress(Action);
                }
                if ($scope.expressBooking.ShipTo.Country.CountryId === 38 || $scope.expressBooking.ShipTo.Country.CountryId === 229) {
                    $scope.GetToCountryStates();
                }
                getHubAddress();
            }
        }
    };

    $scope.GetAWBs = function (awb) {
        if (awb && awb.length > 2) {
            if (!$scope.CustomerDetail || !$scope.CustomerDetail.CustomerId) {
                $scope.customerId = 0;
            }
            else {
                $scope.customerId = $scope.CustomerDetail.CustomerId;
            }
            return ExpressBookingService.GetCustomerAWBs($scope.customerId, awb).then(function (response) {
                if (!response.data.length) {
                    $scope.NoAwbAvailable = true;
                }
                else {
                    $scope.NoAwbAvailable = false;
                }
                return response.data;

            }, function (error) {

            });
        }
    };

    var getShipmentDetails = function (CallingType) {

        ExpressBookingService.GetBookingDetail($scope.shipmentId, CallingType).then(function (response) {
            if (response.data) {
                $scope.expressBooking = response.data;

                if (!$scope.expressBooking.CustomInformation.ContentsExplanation) {
                    $scope.expressBooking.CustomInformation.ContentsExplanation = 'Goods';
                }
                if (!$scope.expressBooking.CustomInformation.RestrictionType) {
                    $scope.expressBooking.CustomInformation.RestrictionType = $scope.RestrictionType[0].value;
                }
                if (!$scope.expressBooking.CustomInformation.RestrictionComments) {
                    $scope.expressBooking.CustomInformation.RestrictionComments = 'N/A';
                }
                if (!$scope.expressBooking.CustomInformation.NonDeliveryOption) {
                    $scope.expressBooking.CustomInformation.NonDeliveryOption = $scope.NonDeliveryOption[1].value;
                }

                if (!$scope.expressBooking.ActualWeight) {
                    $scope.expressBooking.ActualWeight = null;
                }
                $scope.expressBooking.CreatedBy = $scope.userInfo.EmployeeId;
                if (!$scope.expressBooking.PayTaxAndDuties) {
                    $scope.expressBooking.PayTaxAndDuties = "Receiver";
                }

                if (!$scope.expressBooking.PakageCalculatonType) {
                    $scope.expressBooking.PakageCalculatonType = "kgToCms";
                }
                if ($scope.expressBooking.AWBNumber) {
                    getAWBImage();
                }
                $scope.SetShipinfo($scope.expressBooking.ShipFrom.Country, "Shipper", "Fixed");
                $scope.SetShipinfo($scope.expressBooking.ShipTo.Country, "Receiver", "Fixed");

                if (!parseFloat($scope.expressBooking.DeclaredValue)) {
                    $scope.expressBooking.DeclaredValue = null;
                }

                for (i = 0; i < $scope.ParcelTypes.length; i++) {
                    if ($scope.expressBooking.ParcelType.ParcelType === $scope.ParcelTypes[i].ParcelType) {
                        $scope.expressBooking.ParcelType = $scope.ParcelTypes[i];
                    }
                }

                for (var i = 0; i < $scope.expressBooking.Packages.length; i++) {
                    if (!$scope.expressBooking.Packages[i].CartonValue) {
                        $scope.expressBooking.Packages[i].CartonValue = null;
                    }
                    if (!$scope.expressBooking.Packages[i].Length) {
                        $scope.expressBooking.Packages[i].Length = null;
                    }
                    if (!$scope.expressBooking.Packages[i].Width) {
                        $scope.expressBooking.Packages[i].Width = null;
                    }
                    if (!$scope.expressBooking.Packages[i].Height) {
                        $scope.expressBooking.Packages[i].Height = null;
                    }
                    if (!$scope.expressBooking.Packages[i].Weight) {
                        $scope.expressBooking.Packages[i].Weight = null;
                    }
                    if (!$scope.expressBooking.Packages[i].Value) {
                        $scope.expressBooking.Packages[i].Value = null;
                    }
                }

                var dbpac = $scope.expressBooking.Packages.length - 1;
                for (i = 0; i < $scope.expressBooking.Packages.length; i++) {

                    if (i === dbpac) {
                        $scope.expressBooking.Packages[i].pacVal = true;
                    }
                    else {
                        $scope.expressBooking.Packages[i].pacVal = false;
                    }
                }

                //
                $scope.changeKgToLb($scope.expressBooking.PakageCalculatonType);
                if ($scope.userInfo.RoleId !== 3) {
                    ExpressBookingService.GetCustomers($scope.userInfo.EmployeeId, "Express").then(function (response) {
                        $scope.customers = response.data;
                        var dbCustomers = [];
                        for (i = 0; i < $scope.customers.length; i++) {
                            if ($scope.customers[i].CustomerId) {
                                $scope.customers[i].CustomerAccountNumberR = $scope.customers[i].AccountNumber;
                                var dbr = $scope.customers[i].AccountNumber.split("");
                                var accno = "";
                                for (var j = 0; j < dbr.length; j++) {
                                    accno = accno + dbr[j];
                                    if (j == 2 || j == 5) {
                                        accno = accno + "-";
                                    }
                                }
                                $scope.customers[i].AccountNumber = accno;
                            }
                        }

                        for (k = 0; k < $scope.customers.length; k++) {
                            if ($scope.expressBooking.CustomerId === $scope.customers[k].CustomerId) {
                                $scope.CustomerDetail = $scope.customers[k];
                            }
                        }
                    }, function () {
                        toaster.pop({
                            type: 'error',
                            title: $scope.FrayteError,
                            body: $scope.SomeErrorOccuredTryAgain,
                            showCloseButton: true
                        });
                    });
                }
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.SomeErrorOccuredTryAgain,
                showCloseButton: true
            });
        });
    };

    var getScreenInitials = function () {

        ExpressBookingService.BookingInitials($scope.userInfo.EmployeeId).then(function (response) {
            AppSpinner.hideSpinnerTemplate();

            // Set Country type according to given order
            $scope.CountriesRepo = TopCountryService.TopCountryList(response.data.Countries);
            $scope.PiecesExcelDownloadPathXlsx = response.data.PiecesExcelDownloadPathXlsx;
            $scope.PiecesExcelDownloadPathXls = response.data.PiecesExcelDownloadPathXls;
            $scope.PiecesExcelDownloadPathCsv = response.data.PiecesExcelDownloadPathCsv;
            $scope.ParcelTypes = response.data.ParcelTypes;

            //Set Currency type according to given order
            $scope.CurrencyTypes = TopCurrencyService.TopCurrencyList(response.data.CurrencyTypes);
            $scope.ShipmentMethods = response.data.ShipmentMethods;
            $scope.CustomerDetail = response.data.CustomerDetail;
            $scope.CustomerAddress = response.data.CustomerAddress;

            //$scope.AirLines = TopAirlineService.TopAirlineList(response.data.AirLines);
            $scope.ClearButtonEnable = false;
            $scope.CountryPhoneCodes = response.data.CountryPhoneCodes;

            $scope.shipmentId = 0;

            if ($stateParams.shipmentId) {
                $scope.shipmentId = parseInt($stateParams.shipmentId, 10);
            }

            if ($scope.shipmentId > 0) {
                var CallingType = '';

                if ($state.current.name === "loginView.userTabs.express-solution-booking-clone") {
                    CallingType = "ShipmentClone";
                } else if ($state.current.name === "loginView.userTabs.express-solution-booking-return") {
                    CallingType = "ShipmentReturn";
                }else {
                    CallingType = "ScanAWB";
                }
                getShipmentDetails(CallingType);
            }
            else {
                newBooking();

                $scope.changeKgToLb($scope.expressBooking.PakageCalculatonType);

                $scope.expressBooking.CreatedBy = $scope.userInfo.EmployeeId;
                if ($scope.RoleId === 3) {
                    $scope.expressBooking.CustomerId = $scope.userInfo.EmployeeId;
                }
                else {

                    ExpressBookingService.GetCustomers($scope.userInfo.EmployeeId, "ExpressBooking").then(function (response) {
                        $scope.customers = response.data;

                        var dbCustomers = [];
                        for (i = 0; i < $scope.customers.length; i++) {

                            if ($scope.customers[i].CustomerId) {

                                $scope.customers[i].CustomerAccountNumberR = $scope.customers[i].AccountNumber;

                                var dbr = $scope.customers[i].AccountNumber.split("");
                                var accno = "";
                                for (var j = 0; j < dbr.length; j++) {
                                    accno = accno + dbr[j];
                                    if (j == 2 || j == 5) {
                                        accno = accno + "-";
                                    }
                                }
                                $scope.customers[i].AccountNumber = accno;
                            }
                        }

                        if ($scope.userInfo.RoleId === 5) {
                        }

                    }, function () {
                        toaster.pop({
                            type: 'error',
                            title: $scope.FrayteError,
                            body: $scope.SomeErrorOccuredTryAgain,
                            showCloseButton: true
                        });
                    });
                }
            }
        },
        function (response) {
            AppSpinner.hideSpinnerTemplate();
            if (response.status !== 401) {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.InitialDataValidation,
                    showCloseButton: true
                });
            }
        });
    };

    // AddressBook 
    $scope.addressBook = function (UserType) {
        if (!$scope.expressBooking.CustomerId) {
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
                    return "Tradelane";
                },
                toCountryId: function () {
                    if ($scope.expressBooking.ShipTo && $scope.expressBooking.ShipTo.Country && $scope.expressBooking.ShipTo.Country.CountryId) {
                        return $scope.expressBooking.ShipTo.Country.CountryId;
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
                    return $scope.expressBooking.CustomerId;
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
                    $scope.GetFromCountryStates();
                    $scope.expressBooking.ShipFrom = addressBooks;
                    if (addressBooks.State) {
                        if (addressBooks.Country.Code === 'USA' || addressBooks.Country.Code === 'CAN' || addressBooks.Country.Code === 'AUS') {
                            for (var i = 0; i < $scope.CountryFromStateList.length; i++) {
                                if ($scope.CountryFromStateList[i].StateDisplay === addressBooks.State) {
                                    $scope.expressBooking.ShipFrom.State = $scope.CountryFromStateList[i].State;
                                }
                            }
                        }
                        else {
                            $scope.expressBooking.ShipFrom.State = addressBooks.State;
                        }
                    }
                    $scope.expressBooking.ShipFrom.Country = addressBooks.Country;
                    $scope.expressBooking.ShipFrom.ExpressAddressId = 0;
                    $scope.SetShipinfo($scope.expressBooking.ShipFrom.Country, UserType, "Fixed");
                }
                else if (UserType === 'Receiver') {
                    $scope.GetToCountryStates();
                    $scope.expressBooking.ShipTo = addressBooks;
                    if (addressBooks.State) {
                        if (addressBooks.Country.Code === 'USA' || addressBooks.Country.Code === 'CAN' || addressBooks.Country.Code === 'AUS') {
                            for (var j = 0; j < $scope.CountryToStateList.length; j++) {
                                if ($scope.CountryToStateList[j].StateDisplay === addressBooks.State) {
                                    $scope.expressBooking.ShipTo.State = $scope.CountryToStateList[j].State;
                                }
                            }
                        }
                        else {
                            $scope.expressBooking.ShipTo.State = addressBooks.State;
                        }
                    }
                    $scope.expressBooking.ShipTo.Country = addressBooks.Country;
                    $scope.expressBooking.ShipTo.ExpressAddressId = 0;
                    $scope.SetShipinfo($scope.expressBooking.ShipTo.Country, UserType, "Fixed");
                }

                // set form in dirty state for progress bar
                if ($scope.expressBookingForm) {
                    $scope.expressBookingForm.$dirty = true;
                }
            }
        });
    };

    $scope.ErrorTemplate = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBooking/errrorTemplate.tpl.html',
            controller: 'errorTemplateController',
            windowClass: 'ErrorTemplate-Modal',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                FrayteError: function () {
                    return row;
                }
            }
        });
    };

    $scope.PlaceBooking = function (IsValid) {
        if ($scope.NoAwbAvailable) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.Enter_Valid_AWB_Number_First,
                showCloseButton: true
            });
            return;
        }
        if ($scope.BookingType === 'Current') {

            $scope.expressBooking.ShipmentStatusId = $scope.ShipmentStatus.Current;
        }
        else {
            $scope.expressBooking.ShipmentStatusId = $scope.ShipmentStatus.Draft;
            if (!$scope.expressBooking.ShipFrom.Country || !$scope.expressBooking.ShipFrom.Country.CountryId) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarning,
                    body: $scope.Select_From_Contry_Save_Draft,
                    showCloseButton: true
                });
                return;
            }
            if (!$scope.expressBooking.ShipTo.Country || !$scope.expressBooking.ShipTo.Country.CountryId) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarning,
                    body: $scope.Select_To_Contry_Save_Draft,
                    showCloseButton: true
                });
                return;
            }
            if (!$scope.expressBooking.CustomerId) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarning,
                    body: $scope.Select_To_Contry_Save_Draft,
                    showCloseButton: true
                });
                return;
            }
            if (!$scope.expressBooking.AWBNumber) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarning,
                    body: $scope.Fill_AWB_To_Save_Shipment_Draft,
                    showCloseButton: true
                });
                return;
            }
        }
        if ((IsValid && $scope.expressBooking.CustomInformation.CustomsCertify) || $scope.expressBooking.ShipmentStatusId === $scope.ShipmentStatus.Draft) {
            $rootScope.GetServiceValue = false;
            AppSpinner.showSpinnerTemplate("", $scope.Template);
            ExpressBookingService.SaveShipment($scope.expressBooking).then(function (response) {
                $rootScope.GetServiceValue = null;
                if (response.data.Error.Status) {
                    $scope.expressBooking.ExpressId = response.data.ExpressId;
                    $timeout(function () {
                        toaster.pop({
                            type: 'success',
                            title: $scope.FrayteSuccess,
                            body: $scope.BookingSaveValidation,
                            showCloseButton: true
                        });
                        var modalInstance = $uibModal.open({
                            animation: true,
                            templateUrl: 'express/expressDetail/expressShipmentDetail.tpl.html',
                            controller: 'ExpressDetailController',
                            windowClass: 'directBookingDetail',
                            size: 'lg',
                            backdrop: 'static',
                            keyboard: false,
                            resolve: {
                                ShipmentId: function () {
                                    return $scope.expressBooking.ExpressId;
                                }
                            }
                        });
                        modalInstance.result.then(function (response) {
                            $state.go("loginView.userTabs.express-solution-shipments");
                        }, function () {
                            $state.go("loginView.userTabs.express-solution-shipments");
                        });
                    }, 1000);
                }
                else {
                    AppSpinner.hideSpinnerTemplate();
                    $scope.expressBooking.ShipFrom = response.data.ShipFrom;
                    $scope.expressBooking.ShipTo = response.data.ShipTo;
                    $scope.expressBooking.CustomInformation = response.data.CustomInformation;
                    $scope.expressBooking.Packages = response.data.Packages;
                    $scope.ErrorTemplate(response.data.Error);

                }
                AppSpinner.hideSpinnerTemplate();

            }, function () {
                $rootScope.GetServiceValue = null;
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.Error_While_Placing_Booking_Try_Again,
                    showCloseButton: true
                });
                AppSpinner.hideSpinnerTemplate();
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteError,
                body: $scope.CorrectValidationErrorFirst,
                showCloseButton: true
            });
        }

    };

    $scope.rotateImageAC = function () {

        var img = $('#myimage');
        if (img.hasClass('north')) {
            img.attr('class', 'west');
        } else if (img.hasClass('west')) {
            img.attr('class', 'south');
        } else if (img.hasClass('south')) {
            img.attr('class', 'east');
        } else if (img.hasClass('east')) {
            img.attr('class', 'north');
        }
    };

    $scope.rotateImageC = function () {

        var img = $('#myimage');
        if (img.hasClass('north')) {
            img.attr('class', 'east');
            //  img.attr('class', 'west');
        } else if (img.hasClass('west')) {
            img.attr('class', 'south');
        } else if (img.hasClass('south')) {
            img.attr('class', 'east');
        } else if (img.hasClass('east')) {
            img.attr('class', 'north');
        }
    };

    // Zoom image 
    function imageZoom(imgID, resultID) {
        var img, lens, result, cx, cy;
        img = document.getElementById(imgID);
        result = document.getElementById(resultID);
        /*create lens:*/
        lens = document.createElement("DIV");
        lens.setAttribute("class", "img-zoom-lens");
        /*insert lens:*/
        img.parentElement.insertBefore(lens, img);
        /*calculate the ratio between result DIV and lens:*/
        cx = result.offsetWidth / lens.offsetWidth;
        cy = result.offsetHeight / lens.offsetHeight;
        /*set background properties for the result DIV:*/
        result.style.backgroundImage = "url('" + img.src + "')";
        result.style.backgroundSize = (img.width * cx) + "px " + (img.height * cy) + "px";
        /*execute a function when someone moves the cursor over the image, or the lens:*/
        lens.addEventListener("mousemove", moveLens);
        img.addEventListener("mousemove", moveLens);
        /*and also for touch screens:*/
        lens.addEventListener("touchmove", moveLens);
        img.addEventListener("touchmove", moveLens);
        function moveLens(e) {
            var pos, x, y;
            /*prevent any other actions that may occur when moving over the image:*/
            e.preventDefault();
            /*get the cursor's x and y positions:*/
            pos = getCursorPos(e);
            /*calculate the position of the lens:*/
            x = pos.x - (lens.offsetWidth / 2);
            y = pos.y - (lens.offsetHeight / 2);
            /*prevent the lens from being positioned outside the image:*/
            if (x > img.width - lens.offsetWidth) { x = img.width - lens.offsetWidth; }
            if (x < 0) { x = 0; }
            if (y > img.height - lens.offsetHeight) { y = img.height - lens.offsetHeight; }
            if (y < 0) { y = 0; }
            /*set the position of the lens:*/
            lens.style.left = x + "px";
            lens.style.top = y + "px";
            /*display what the lens "sees":*/
            result.style.backgroundPosition = "-" + (x * cx) + "px -" + (y * cy) + "px";
        }
        function getCursorPos(e) {
            var a, x = 0, y = 0;
            e = e || window.event;
            /*get the x and y positions of the image:*/
            a = img.getBoundingClientRect();
            /*calculate the cursor's x and y coordinates, relative to the image:*/
            x = e.pageX - a.left;
            y = e.pageY - a.top;
            /*consider any page scrolling:*/
            x = x - window.pageXOffset;
            y = y - window.pageYOffset;
            return { x: x, y: y };
        }
    }

    function init() {

        $scope.BookingType = 'Draft';
        setMultilingualOptions();
        $scope.emailFormat = /^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,10}$/;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.ShipmentStatus = {
            Draft: 39,
            Current: 38,
            Scanned: 37
        };

        $scope.ContentsType = [
             {
                 name: 'Documents',
                 value: 'documents'
             },
                 {
                     name: 'Gift',
                     value: 'gift'
                 },
             {
                 name: 'Merchandise',
                 value: 'merchandise'
             },
             {
                 name: 'Returned Goods',
                 value: 'returned_goods'
             },
             {
                 name: 'Sample',
                 value: 'sample'
             },
             {
                 name: 'Other',
                 value: 'other'
             }
        ];

        $scope.RestrictionType = [
            {
                name: 'None',
                value: 'none'
            },
            {
                name: 'Other',
                value: 'other'
            },
            {
                name: 'Quarantine',
                value: 'quarantine'
            },
            {
                name: 'Sanitary Phytosanitary Inspection',
                value: 'sanitary_phytosanitary_inspection'
            }
        ];

        $scope.NonDeliveryOption = [
           {
               name: 'By Default Abandon',
               value: 'abandon'
           },
           {
               name: 'Return to Sender',
               value: 'return'
           }
        ];

        $scope.ItemCatogory = [
        {
            name: 'Sold',
            value: 'sold'
        },
        {
            name: 'Samples',
            value: 'samples'
        },
        {
            name: 'Gift',
            value: 'gift'
        },
        {
            name: 'Documents',
            value: 'documents'
        },
        {
            name: 'Commercial Sample',
            value: 'commercial_sample'
        },
        {
            name: 'Returned Goods',
            value: 'returned_goods'
        }];


        $scope.ImagePath = config.BUILD_URL;

        $scope.photoUrl = config.BUILD_URL + "addressBook.png";
        $scope.photoHazard = config.BUILD_URL + "Hazard_logo.png";
        $scope.active = 3;
        $scope.BookingType = "";
        $scope.htmlPopover = $sce.trustAsHtml('<b style="color: red">I can</b> have <div class="label label-success">HTML</div> content');
        //$scope.CountryPopOver = $sce.trustAsHtml('<span style : "width : 269px">China (CN1) covers Guangdong Province only.China (CN2) covers all other provinces.<span>');
        //$scope.CountryPopOver = $sce.trustAsHtml("China covers Guangdong Province only.");


        var userInfo = SessionService.getUser();
        $scope.userInfo = userInfo;
        $scope.loggedInUserId = $scope.userInfo.EmployeeId;
        $scope.tabs = userInfo.tabs;
        $scope.tab = getUserTab($scope.tabs, "QuickDirect_Booking");
        $scope.RoleId = userInfo.RoleId;

        if ($scope.RoleId === 3) {
            $scope.customerId = $scope.userInfo.EmployeeId;
        }
        else {
            $scope.customerId = 0;
        }
        $scope.submitted = true;

        getScreenInitials();
    }

    init();
});