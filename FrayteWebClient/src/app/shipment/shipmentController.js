/** 
 * Controller
 */
angular.module('ngApp.shipment').controller('ShipmentController', function ($scope, config, $rootScope, $translate, $location, Upload, $state, $filter, ShipmentService, TimeZoneService, CountryService, CourierService, HomeService, ShipperService, ReceiverService, SessionService, $uibModal, $log, toaster, $stateParams, CustomerService, UserService) {
    // Disable input controls in Custom information panel
    $scope.disableSigner = function (value) {

        $scope.shipment.CustomInfo.CustomsSigner = '';
        if (value) {
            $scope.disableCustomerSigner = false;
        }
        else {
            $scope.disableCustomerSigner = true;
        }
    };
    $scope.disableRestriction = function (RestrictionType) {
        $scope.shipment.CustomInfo.RestrictionComments = '';
        if (RestrictionType === "other") {
            $scope.disableRestrictionComment = false;

        }
        else if (RestrictionType === "none") {
            $scope.shipment.CustomInfo.RestrictionComments = 'N/A';
            $scope.disableRestrictionComment = false;
        }
        else {
            $scope.disableRestrictionComment = true;
        }
    };
    $scope.disableContent = function (ContentType) {
        $scope.shipment.CustomInfo.ContentsExplanation = '';
        if (ContentType === "other") {
            $scope.disableContentExplanation = false;

        }
        else {
            $scope.disableContentExplanation = true;
        }

    };

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'FrayteSuccess', 'CustomInfoCheckTerms', 'PleaseSelectionDeliveryOption', 'PleaseSelectTermAndConditions', 'PleaseCorrectValidationErrors', 'ErrorSavingShipment', 'PleaseSelectValidFile', 'UploadedSuccessfully', 'ErrorOccuredDuringUpload', 'ErrorGettingShipmentDetailServer']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TitleFrayteSuccess = translations.FrayteSuccess;

            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextPleaseSelectionDeliveryOption = translations.PleaseSelectionDeliveryOption;

            $scope.TextErrorSavingShipment = translations.ErrorSavingShipment;
            $scope.TextCustomInfoCheckTerms = translations.CustomInfoCheckTerms;
            $scope.TextPleaseSelectTermAndConditions = translations.PleaseSelectTermAndConditions;

            $scope.TextPleaseSelectValidFile = translations.PleaseSelectValidFile;
            $scope.TextUploadedSuccessfully = translations.UploadedSuccessfully;
            $scope.TextErrorOccuredDuringUpload = translations.ErrorOccuredDuringUpload;
            $scope.TextErrorGettingShipmentDetailServer = translations.ErrorGettingShipmentDetailServer;
        });
    };

    //Working Week Days

    // Custom Info on PickUpAddress
    $scope.PickUpCustomInfo = function (Country) {
        if ($scope.shipment.DeliveredBy !== null) {
            if ($scope.shipment.DeliveredBy.CourierType == "Courier") {
                if ($scope.shipment.ExportType !== "Export") {
                    if (Country.CountryId !== $scope.shipment.ReceiverAddress.Country.CountryId) {
                        $scope.CustomInfo = true;
                        $scope.showAndHideCustomInfo();
                    }
                    else {
                        $scope.CustomInfo = false;
                        $scope.shipment.CustomInfo = null;
                    }
                }
            }
        }
        if (Country.TimeZoneDetail !== null && Country.TimeZoneDetail.TimezoneId > 0) {
            $scope.shipment.Timezone = Country.TimeZoneDetail;
            $scope.restoreTimeZone1 = Country.TimeZoneDetail;
        }
        // Set The Phone Code for Pickup Addreess
        var objects = $scope.countryPhoneCodes;
        for (var i = 0; i < objects.length; i++) {
            if (objects[i].Name.indexOf(Country.Name) > -1) {
                $scope.shipment.ShipmentPickupContactPhoneNumberCode = objects[i];

                break;
            }
        }
    };

    // Set Pickup Country based on TimeZone
    $scope.SelectPickupCountry = function (timeZone) {
        ShipmentService.GetCountryByTimezone(timeZone.TimezoneId).then(function (response) {
            if ($scope.shipment.OtherPickupAddress) {
                if (response.data !== null) {
                    $scope.shipment.PickupAddress.Country = response.data;
                    var objects = $scope.countryPhoneCodes;
                    for (var i = 0; i < objects.length; i++) {
                        if (objects[i].Name.indexOf($scope.shipment.PickupAddress.Country.Name) > -1) {
                            $scope.shipment.ShipmentPickupContactPhoneNumberCode = objects[i];

                            break;
                        }
                    }
                }
                else {
                    console.log("Can not set null.");
                }

            }
        },
       function () {
           console.log("Can not set country based on timezone");
       });

    };
    // For Showing Custom Info Section 
    $scope.showAndHideCustomInfo = function () {
        if ($scope.shipment.CustomInfo !== null) {
            if ($scope.shipment.CustomInfo.ContentsType !== null && $scope.shipment.CustomInfo.ContentsType === "other") {
                $scope.disableContentExplanation = false;
            }
            else {
                $scope.disableContentExplanation = true;
            }

            if ($scope.shipment.CustomInfo.RestrictionType !== null && $scope.shipment.CustomInfo.RestrictionType === "other") {
                $scope.disableRestrictionComment = false;
            }
            else {
                $scope.disableRestrictionComment = true;
            }

            if ($scope.shipment.CustomInfo.CustomsCertify !== null && $scope.shipment.CustomInfo.CustomsCertify) {
                $scope.disableCustomerSigner = false;
            }
            else {
                $scope.disableCustomerSigner = true;
            }
        }
        else {
            $scope.disableCustomerSigner = true;
            $scope.disableContentExplanation = true;
            $scope.disableRestrictionComment = true;
        }
    };
    // selectCustomInfo
    $scope.selectCustomInfo = function (wareHouseCountry) {
        if ($scope.shipment.DeliveredBy !== null && $scope.shipment.DeliveredBy.CourierType === "Courier") {
            if (wareHouseCountry.CountryId == $scope.shipment.ReceiverAddress.Country.CountryId) {
                $scope.CustomInfo = false;
                $scope.shipment.CustomInfo = null;
            }
            else {
                $scope.CustomInfo = true;
                $scope.showAndHideCustomInfo();

            }
        }
    };
    // select payment party and Custom Information
    $scope.selectpaymentParty = function (shippigType) {
        if (shippigType.CourierType === "Air") {
            $scope.onlyShow = true;
            $scope.CustomInfo = false;
            $scope.shipment.CustomInfo = null;
            $scope.CourierCase = false;
        }
       else if (shippigType.CourierType === "Courier") {
           $scope.shipment.FrayteShipmentPortOfDeparture = null;
            $scope.shipment.FrayteShipmentPortOfArrival = null;
            if ($scope.shipment.Warehouse == null || $scope.shipment.Warehouse.CountryId === 0) {
                if ($scope.shipment.PickupAddress.Country !== null) {
                    if ($scope.shipment.PickupAddress.Country.CountryId !== $scope.shipment.ReceiverAddress.Country.CountryId) {
                        $scope.CustomInfo = true;
                        $scope.showAndHideCustomInfo();
                    }
                    else {
                        $scope.CustomInfo = false;
                        $scope.shipment.CustomInfo = null;
                    }
                }
            }
            else {
                if ($scope.shipment.Warehouse.CountryId !== $scope.shipment.ReceiverAddress.Country.CountryId) {
                    $scope.CustomInfo = true;
                    $scope.showAndHideCustomInfo();
                }
                else {
                    $scope.CustomInfo = false;
                    $scope.shipment.CustomInfo = null;
                }
            }
            $scope.CourierCase = true;
            $scope.shipment.PaymentPartyAccountNo = null;
            $scope.onlyShow = false;

        }
    else  if (shippigType.CourierType === "Sea") {
            $scope.CustomInfo = false;
            $scope.shipment.CustomInfo = null;
            $scope.shipment.PaymentPartyAccountNo = null;
            $scope.onlyShow = false;
            $scope.CourierCase = false;
    }
    else if (shippigType.CourierType === "Expryes") {
        $scope.shipment.FrayteShipmentPortOfDeparture = null;
        $scope.shipment.FrayteShipmentPortOfArrival = null;
    }

    };

    $scope.selectType = function (type) {
        if (type === "Receiver") {
            if ($scope.newPaymentParty === type) {
                $scope.shipment.PaymentPartyAccountNo = $scope.newVara;
            }
            else {
                $scope.shipment.PaymentPartyAccountNo = null;
            }
            $translate('Receiver').then(function (receiver) {
                $scope.accountHolder = receiver;
            });
            //$scope.accountHolder = "Receiver";
        }
        if (type === "Shipper") {
            if ($scope.newPaymentParty === type) {
                $scope.shipment.PaymentPartyAccountNo = $scope.newVara;
            }
            else {
                $scope.shipment.PaymentPartyAccountNo = null;
            }

            $translate('Shipper').then(function (shipper) {
                $scope.accountHolder = shipper;
            });
            //$scope.accountHolder = "Shipper";
        }
        if (type === "ThirdParty") {
            if ($scope.newPaymentParty === type) {
                $scope.shipment.PaymentPartyAccountNo = $scope.newVara;
            }
            else {
                $scope.shipment.PaymentPartyAccountNo = null;
            }
            $translate('ThirdParty').then(function (ThirdParty) {
                $scope.accountHolder = ThirdParty;
            });
            // $scope.accountHolder = "Third Party";
        }
    };

    $scope.selectTypeTaxDuties = function (type) {
        if (type === "Receiver") {
            if ($scope.newPaymentPartyTaxDuties === type) {
                $scope.shipment.PaymentPartyTaxDutiesAccountNo = $scope.newVara1;
            }
            else {
                $scope.shipment.PaymentPartyTaxDutiesAccountNo = null;
            }
            $translate('Receiver').then(function (receiver) {
                $scope.accountHoldertaxDuties = receiver;
            });
            //$scope.accountHolder = "Receiver";
        }
        if (type === "Shipper") {
            if ($scope.newPaymentPartyTaxDuties === type) {
                $scope.shipment.PaymentPartyTaxDutiesAccountNo = $scope.newVara1;
            }
            else {
                $scope.shipment.PaymentPartyTaxDutiesAccountNo = null;
            }

            $translate('Shipper').then(function (shipper) {
                $scope.accountHoldertaxDuties = shipper;
            });
            //$scope.accountHolder = "Shipper";
        }
        if (type === "ThirdParty") {
            if ($scope.newPaymentPartyTaxDuties === type) {
                $scope.shipment.PaymentPartyTaxDutiesAccountNo = $scope.newVara1;
            }
            else {
                $scope.shipment.PaymentPartyTaxDutiesAccountNo = null;
            }
            $translate('ThirdParty').then(function (ThirdParty) {
                $scope.accountHoldertaxDuties = ThirdParty;
            });
            // $scope.accountHolder = "Third Party";
        }
    };
    //Address Book
    $scope.AddToAddressBook = function (userType, userId) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'shipper/shipperAddressBook/addressBookAddEdit.tpl.html',
            controller: 'AddressBookAddEditController',
            windowClass: '',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                mode: function () {
                    return 'Add';
                },
                addressBooks: function () {
                    return [];
                },
                addressBook: function () {
                    return {
                        SN: 0,
                        UserId: userId,
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
            }
        });

        modalInstance.result.then(function (otherAddresses) {
            //It will return collection of Addresses, here it will always be single collection
            //Save the new address in db
            var address = otherAddresses[0];
            address.UserId = userId;
            if (userType === 'SHIPPER') {
                ShipperService.SaveShipperOtherAddress(address).then(function (response) {
                    $scope.shipment.ShipperAddress = address;
                });
            }
            if (userType === 'RECEIVER') {
                ReceiverService.SaveReceiverOtherAddress(address).then(function (response) {
                    $scope.shipment.ReceiverAddress = address;
                });
            }

        });
    };

    $scope.OtherAddressFrom = function (userType, userId) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'shipment/shipmentOtherAddress/otheraddressFrom.tpl.html',
            controller: 'OtherAddressController',
            windowClass: '',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                userType: function () {
                    return userType;
                },
                userId: function () {
                    return userId;
                }
            }
        });

        modalInstance.result.then(function (otherAddress) {
            if (userType === 'SHIPPER') {
                $scope.shipment.ShipperAddress = otherAddress;
            }
            else if (userType === 'RECEIVER') {
                $scope.shipment.ReceiverAddress = otherAddress;
            }

        });
    };
    $scope.disableAddEditReceiver = true;
    $scope.GetReceiverAddress = function (receiverDetail) {
        ReceiverService.GetReceiverDetail(receiverDetail.UserId).then(function (response) {
            $scope.shipment.Receiver.UserId = response.data.UserId;
            $scope.shipment.Receiver.ShortName = response.data.ShortName;
            $scope.shipment.Receiver.ContactName = response.data.ContactName;
            $scope.shipment.Receiver.CompanyName = response.data.CompanyName;
            $scope.shipment.Receiver.TelephoneNo = response.data.TelephoneNo;
            $scope.shipment.Receiver.MobileNo = response.data.MobileNo;
            //$scope.shipment.Receiver.FaxNumber = response.data.FaxNumber;
            $scope.shipment.Receiver.Email = response.data.Email;
            $scope.shipment.Receiver.VATGST = response.data.VATGST;
            $scope.shipment.Receiver.WorkingStartTime = response.data.WorkingStartTime;
            $scope.shipment.Receiver.WorkingEndTime = response.data.WorkingEndTime;
            $scope.shipment.Receiver.Timezone = response.data.Timezone;
            $scope.shipment.ReceiverAddress = response.data.UserAddress;
            $scope.disableAddEditReceiver = false;
        });
    };

    // Edit Shipper Receiver Address When Shipper is logged in
    $scope.AddEditShipperReceiverAddress = function (userType, userId, shipperReceiverAddress) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'shipper/shipperAddressBook/addressBookAddEdit.tpl.html',
            controller: 'AddressBookAddEditController',
            windowClass: '',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                mode: function () {
                    return 'Edit';
                },
                addressBooks: function () {
                    return [];
                },
                addressBook: function () {
                    return {
                        SN: 0,
                        UserId: userId,
                        UserAddressId: shipperReceiverAddress.UserAddressId,
                        Address: shipperReceiverAddress.Address,
                        Address2: shipperReceiverAddress.Address2,
                        Address3: shipperReceiverAddress.Address3,
                        Suburb: shipperReceiverAddress.Suburb,
                        City: shipperReceiverAddress.City,
                        State: shipperReceiverAddress.State,
                        Zip: shipperReceiverAddress.Zip,
                        Country: shipperReceiverAddress.Country
                    };
                }
            }
        });

        modalInstance.result.then(function (otherAddresses) {
            //It will return collection of Addresses, here it will always be single collection
            //Save the new address in db
            var address = otherAddresses[0];
            address.UserId = userId;
            if (userType === 'SHIPPER') {
                ShipperService.SaveShipperOtherAddress(address).then(function (response) {
                    $scope.shipment.ShipperAddress = address;
                });
            }
            if (userType === 'RECEIVER') {
                ReceiverService.SaveReceiverOtherAddress(address).then(function (response) {
                    $scope.shipment.ReceiverAddress = address;
                });
            }

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


    $scope.newShipment = function () {
        $scope.shipment = {
            PurchaseOrderNo: '',
            CustomerEmail: '',
            CustomerAccountNumber: 0,
            Shipper: {
                UserId: 0,
                //ShortName: '',
                ContactName: '',
                CompanyName: '',
                TelephoneNo: '',
                MobileNo: '',
                //FaxNumber: '',
                Email: '',
                VATGST: '',
                WorkingStartTime: setWorkingStartTime(),
                WorkingEndTime: setWorkingEndTime(),
                Timezone: null,
                WorkingWeekDay: null
            },
            ShipperAddress: {
                UserAddressId: 0,
                Country: null,
                Address: '',
                Address2: '',
                Address3: '',
                Zip: '',
                City: '',
                Suburb: '',
                State: ''
            },
            Receiver: {
                UserId: 0,
                //ShortName: '',
                ContactName: '',
                CompanyName: '',
                TelephoneNo: '',
                MobileNo: '',
                //FaxNumber: '',
                Email: '',
                VATGST: '',
                WorkingStartTime: setWorkingStartTime(),
                WorkingEndTime: setWorkingEndTime(),
                Timezone: null,
                WorkingWeekDay: null
            },
            ReceiverAddress: {
                UserAddressId: 0,
                Country: null,
                Address: '',
                Address2: '',
                Address3: '',
                Zip: '',
                City: '',
                Suburb: '',
                State: ''
            },
            FrayteShipmentPortOfDeparture: null,
            FrayteShipmentPortOfArrival: null,
            ShipmentTerm: null,
            PackagingType: null,
            SpecialDelivery: null,
            checkCustomInfo: false,
            CustomInfo: {
                ShipmentEasyPostId: 0,
                ContentsType: null,
                ContentsExplanation: '',
                RestrictionType: null,
                RestrictionComments: '',
                CustomsCertify: null,
                CustomsSigner: '',
                NonDeliveryOption: null,
                EelPfc: ''
            },
            PickupAddress: {
                UserAddressId: 0,
                Country: null,
                Address: '',
                Address2: '',
                Address3: '',
                Zip: '',
                City: '',
                Suburb: '',
                State: ''
            },
            PiecesCaculatonType: 'kgToCms',
            ShipmentDetails: [{
                JobNumber: '',
                JobStyle: '',
                HSCode: '',
                CartonQty: null,
                Pieces: null,
                WeightKg: null,
                Lcms: null,
                Wcms: null,
                Hcms: null,
                PiecesContent: ''
            }
            ],
            ContentDescription: '',
            Guidelines: '',
            ShipmentDuitable: null,
            ShippingReference: '',
            ShippingDate: new Date(),
            ShippingTime: getTime(),
            PaymentParty: '',
            PaymentPartyAccountNo: '',
            PaymentPartyTaxDuties: '',
            PaymentPartyTaxDutiesAccountNo: '',
            DeclaredValue: null,
            DeclaredCurrency: null,
            DeliveredBy: null,
            OtherPickupAddress: false,
            ShipmentPickupContactName: '',
            ShipmentPickupContactPhoneNumber: '',
            LocationType: '',
            LocationOfShipment: '',
            SpecialInstruction: '',
            PickupDate: new Date(),
            ShipmentReadyBy: '',
            OfficeCloseAt: '',
            Timezone: null,
            ExportType: 'Import',
            Warehouse: null,
            TransportToWarehouse: null,
            TermAndCondition: false,
            TermAndConditionId: $scope.TermAndConditionId,
            UserRoleId: $scope.UserRoleType.Shipper,
            IsLogin: false
        };
    };



    $scope.showOtherCourier = function (OtherCourier) {
        if (OtherCourier === "Other") {
            $scope.otherCourierTranport = true;
        }
        else {
            $scope.otherCourierTranport = false;
            $scope.shipment.OtherTransportToWareHouseCarrier = null;
        }
    };
    var getTime = function () {
        var d = new Date();
        var h = d.getHours();
        var m = d.getMinutes();
        if (h.toString().length < 2) {
            h = '0' + h.toString();
        }
        if (m.toString().length < 2) {
            m = '0' + m.toString();
        }

        return h.toString() + m.toString();
    };
    $scope.countrySearch = function (query) {
        var results = query ? $scope.countries.filter(createFilterFor(query)) : $scope.countries,
       deferred;
        if (self.simulateQuery) {
            deferred = $q.defer();
            $timeout(function () { deferred.resolve(results); }, Math.random() * 1000, false);
            return deferred.promise;
        } else {
            return results;
        }
    };

    function createFilterFor(query) {
        var lowercaseQuery = angular.lowercase(query);

        return function filterFn(country) {
            if (country === null) {
                return null;
            }
            else {
                return (country.Name.toLowerCase().indexOf(lowercaseQuery) === 0);
            }
        };

    }

    $scope.submit = function (isValid, shipment) {
        if ((isValid && $scope.isValidAccountNumber) && (!$scope.isValidEmail && !$scope.isValidEmailRec)) {
            switch ($scope.shipmentStep) {
                case 1:
                    $scope.shipmentStepStatus.addressdetail = true;
                    $scope.shipmentStep = 2;
                    // Check Receiver And Shipper, Warehouse Country for custom info section
                    $scope.CheckCustomInfo();
                    // Get Country Ports
                    //if (shipment.ShipperAddress!==null && shipment.ShipperAddress.Country !== null && shipment.ShipperAddress.Country.CountryId > 0) {
                    //    ShipmentService.GetCountryPort(shipment.ShipperAddress.Country.CountryId).then(function (response) {
                    //    $scope.portsOfDeparture= response.data;
                    //}, function () {
                    //});
                    //}
                    //if (shipment.ReceiverAddress !== null && shipment.ReceiverAddress.Country !== null && shipment.ReceiverAddress.Country.CountryId > 0) {
                    //    ShipmentService.GetCountryPort(shipment.ReceiverAddress.Country.CountryId).then(function (response) {
                    //        $scope.portsOfArrival = response.data;
                    //    }, function () {
                    //    });
                    //}

                    //Copy Shipper's address in Pickup address
                    if (shipment.PickupAddress.UserAddressId === 0 && !shipment.OtherPickupAddress) {
                        $scope.checked = true;
                        shipment.ShipmentPickupContactName = shipment.Shipper.ContactName;
                        shipment.ShipmentPickupContactPhoneNumber = shipment.Shipper.TelephoneNo;
                        shipment.ShipmentPickupContactPhoneNumberCode = shipment.Shipper.TelephoneCode;
                        shipment.PickupAddress.Country = shipment.ShipperAddress.Country;
                        shipment.PickupAddress.Address = shipment.ShipperAddress.Address;
                        shipment.PickupAddress.Address2 = shipment.ShipperAddress.Address2;
                        shipment.PickupAddress.Address3 = shipment.ShipperAddress.Address3;
                        shipment.PickupAddress.Zip = shipment.ShipperAddress.Zip;
                        shipment.PickupAddress.City = shipment.ShipperAddress.City;
                        shipment.PickupAddress.Suburb = shipment.ShipperAddress.Suburb;
                        shipment.PickupAddress.State = shipment.ShipperAddress.State;

                        // code to set the timezone.
                        if (shipment.PickupAddress.Country !== null && shipment.PickupAddress.Country.TimeZoneDetail !== null && shipment.PickupAddress.Country.TimeZoneDetail.TimezoneId > 0) {
                            $scope.shipment.Timezone = shipment.PickupAddress.Country.TimeZoneDetail;
                            $scope.restoreTimeZone = shipment.PickupAddress.Country.TimeZoneDetail;
                        }
                    }

                    if ($scope.isLogin) {
                        $state.go('shipper.shipment.serviceoption');
                    }
                    else {
                        $state.go('home.shipment.serviceoption');
                    }

                    break;

                case 2:

                    $scope.shipmentStepStatus.serviceoption = true;
                    if ($scope.shipment.DeliveredBy.CourierId === 0) {
                        toaster.pop({
                            type: 'warning',
                            title: $scope.TitleFrayteError,
                            body: $scope.TextPleaseSelectionDeliveryOption,
                            showCloseButton: true
                        });
                    }
                    else {
                        if ($scope.shipment.CustomInfo !== null && $scope.shipment.CustomInfo.ContentsType !== null && $scope.shipment.CustomInfo.RestrictionType !== null) {
                            if ($scope.shipment.checkCustomInfo) {
                                $scope.shipmentStep = 3;
                                if ($scope.isLogin) {
                                    $state.go('shipper.shipment.shipmentdetail');
                                }
                                else {
                                    $state.go('home.shipment.shipmentdetail');
                                }
                            }
                            else {
                                toaster.pop({
                                    type: 'warning',
                                    title: $scope.TitleFrayteValidation,
                                    body: $scope.TextCustomInfoCheckTerms,
                                    showCloseButton: true
                                });
                            }
                        }
                        else {
                            $scope.shipmentStep = 3;
                            if ($scope.isLogin) {
                                $state.go('shipper.shipment.shipmentdetail');
                            }
                            else {
                                $state.go('home.shipment.shipmentdetail');
                            }
                        }
                        //$scope.shipmentStep = 3;
                        //if ($scope.isLogin) {
                        //    $state.go('shipper.shipment.shipmentdetail');
                        //}
                        //else {
                        //    $state.go('home.shipment.shipmentdetail');
                        //}

                    }
                    break;
                case 3:

                    $scope.shipmentStepStatus.shipmentdetail = true;
                    $scope.shipmentStep = 4;

                    if ($scope.isLogin) {
                        $state.go('shipper.shipment.confirmshipment');
                    }
                    else {
                        $state.go('home.shipment.confirmshipment');
                    }

                    break;
                default:

                    if ($scope.isLogin) {
                        $state.go('shipper.shipment.addressdetail');
                    }
                    else {
                        $state.go('home.shipment.addressdetail');
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

    // Set Receiver Country 
    //$scope.SetShipperReceiverState = function (Code) {
    //    debugger;
    //    if (Code !== null && Code !== '' && Code !== undefined) {
    //        if (Code === "HKG" || Code === "GBR") {
    //            return false;
    //        }
    //        else {
    //            return true;
    //        }
    //    }
    //};
    // 
    $scope.SetShipperReceiverState = function (Code, stateZip) {
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
    $scope.StepBack = function (shipment) {
        switch ($scope.shipmentStep) {
            case 3:
                $scope.shipmentStepStatus.serviceoption = false;
                $scope.shipmentStep = 2;
                if (shipment.PickupAddress.UserAddressId === 0 && !shipment.OtherPickupAddress) {
                    shipment.PickupAddress.Country = shipment.ShipperAddress.Country;
                    shipment.PickupAddress.Address = shipment.ShipperAddress.Address;
                    shipment.PickupAddress.Address2 = shipment.ShipperAddress.Address2;
                    shipment.PickupAddress.Address3 = shipment.ShipperAddress.Address3;
                    shipment.PickupAddress.Zip = shipment.ShipperAddress.Zip;
                    shipment.PickupAddress.City = shipment.ShipperAddress.City;
                    shipment.PickupAddress.Suburb = shipment.ShipperAddress.Suburb;
                    shipment.PickupAddress.State = shipment.ShipperAddress.State;
                }
                if ($scope.isLogin) {
                    $state.go('shipper.shipment.serviceoption');
                }
                else {
                    $state.go('home.shipment.serviceoption');
                }
                break;
            case 4:
                $scope.shipmentStepStatus.shipmentdetail = false;
                $scope.shipmentStep = 3;
                if ($scope.isLogin) {
                    $state.go('shipper.shipment.shipmentdetail');
                }
                else {
                    $state.go('home.shipment.shipmentdetail');
                }
                break;
            case 2:
                $scope.shipmentStepStatus.addressdetail = false;
                $scope.shipmentStep = 1;
                if ($scope.isLogin) {
                    $state.go('shipper.shipment.addressdetail');
                    $scope.setSelectedReceiver($scope.shipment.Receiver);
                }
                else {
                    $state.go('home.shipment.addressdetail');
                }
                break;
            default:
                if ($scope.isLogin) {
                    $state.go('shipper.shipment.addressdetail');
                }
                else {
                    $state.go('home.shipment.addressdetail');
                }
        }
    };

    $scope.setSelectedReceiver = function (receiverDetail) {
        var objects = $scope.ShipperReceivers;

        if (objects !== undefined && objects !== null) {
            for (var i = 0; i < objects.length; i++) {
                if (objects[i].UserId === receiverDetail.UserId) {
                    $scope.ReceiverDetail = objects[i];
                    break;
                }
            }
        }
    };

    $scope.CancelForm = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'shipment/shipmentCancel.tpl.html',
            windowClass: 'Logon-Modal',
            size: 'sm'
        });

        modalInstance.result.then(function (reason) {
            if (reason == 'cancel') {
                if ($scope.isLogin) {
                    $state.go('shipper.current-shipment');
                }
                else {
                    $state.go('home.welcome');
                }
            }
        });

    };

    $scope.PlaceBooking = function (shipment) {
        if (shipment.TermAndCondition) {

            //Before placing the booking, need to make sure that user fill the information in requred steps
            if (!$scope.shipmentStepStatus.addressdetail ||
                !$scope.shipmentStepStatus.shipmentdetail ||
                !$scope.shipmentStepStatus.serviceoption) {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'shipment/shipmentValidations.tpl.html',
                    windowClass: '',
                    size: 'lg'
                });

                modalInstance.result.then(function (stepInfo) {
                    if (stepInfo === 'gotosetp1') {
                        $scope.shipmentStep = 1;
                        if ($scope.isLogin) {
                            $state.go('shipper.shipment.addressdetail');
                        }
                        else {
                            $state.go('home.shipment.addressdetail');
                        }
                    }
                });

                return;
            }

            $scope.bookingPlaced.Status = true;
            var shipmentId = shipment.ShipmentId;
            shipment.PickupDate = moment(shipment.PickupDate).format();
            shipment.ShippingDate = moment(shipment.ShippingDate).format();

            if (!$scope.isLogin) {

                if (shipment.Shipper.TelephoneNo !== undefined && shipment.Shipper.TelephoneNo !== '') {
                    shipment.Shipper.TelephoneNo = '(+' + $scope.shipment.Shipper.TelephoneCode.PhoneCode + ') ' + shipment.Shipper.TelephoneNo;
                }

                if (shipment.Shipper.MobileNo !== undefined && shipment.Shipper.MobileNo !== '') {
                    shipment.Shipper.MobileNo = '(+' + $scope.shipment.Shipper.MobileCode.PhoneCode + ') ' + shipment.Shipper.MobileNo;
                }

                //if (shipment.Shipper.FaxNumber !== undefined && shipment.Shipper.FaxNumber !== '') {
                //    shipment.Shipper.FaxNumber = '(+' + $scope.shipment.Shipper.FaxCode.PhoneCode + ') ' + shipment.Shipper.FaxNumber;
                //}

                if (shipment.Receiver.TelephoneNo !== undefined && shipment.Receiver.TelephoneNo !== '') {
                    shipment.Receiver.TelephoneNo = '(+' + $scope.shipment.Receiver.TelephoneCode.PhoneCode + ') ' + shipment.Receiver.TelephoneNo;
                }

                if (shipment.Receiver.MobileNo !== undefined && shipment.Receiver.MobileNo !== '') {
                    shipment.Receiver.MobileNo = '(+' + $scope.shipment.Receiver.MobileCode.PhoneCode + ') ' + shipment.Receiver.MobileNo;
                }

                //if (shipment.Receiver.FaxNumber !== undefined && shipment.Receiver.FaxNumber !== '') {
                //    shipment.Receiver.FaxNumber = '(+' + $scope.shipment.Receiver.FaxCode.PhoneCode + ') ' + shipment.Receiver.FaxNumber;
                //}
            }

            if (shipment.ShipmentPickupContactPhoneNumber !== undefined && shipment.ShipmentPickupContactPhoneNumber !== '') {
                shipment.ShipmentPickupContactPhoneNumber = '(+' + shipment.ShipmentPickupContactPhoneNumberCode.PhoneCode + ') ' + shipment.ShipmentPickupContactPhoneNumber;
            }
            if (!$scope.CustomInfo || $scope.CustomInfo === undefined) {
                $scope.shipment.CustomInfo = null;
            }
            ShipmentService.SaveShipment(shipment).then(function (response) {
                if (response.status) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'shipment/shipmentCreatedSuccessfully.tpl.html',
                        controller: 'SuccessController',
                        windowClass: '',
                        size: 'lg',
                        resolve: {
                            shipmentChild: function () {
                                return $scope.shipment;
                            },
                            userType: function () {
                                return $scope.UserType;
                            },
                            pdfPath: function () {
                                return response.data;
                            }
                        }
                    });

                    modalInstance.result.then(function () {
                    },
                    function () {
                        if ($scope.isLogin) {
                            $state.go('shipper.current-shipment');
                            //$state.go('shipper.current-shipment', {}, { reload: true });
                        }
                        else {
                            $state.go('home.welcome');
                        }
                    });
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorSavingShipment,
                        showCloseButton: true
                    });
                }

            }, function () {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorSavingShipment,
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextPleaseSelectTermAndConditions,
                showCloseButton: true
            });
        }

    };

    $scope.changeValue = function (value) {
        if (value === "Export") {
            $scope.shipment.TrackingNumber = null;
            $scope.shipment.OtherTransportToWareHouseCarrier = null;
            $scope.shipment.TransportToWareHouseCarrier = null;
            $scope.shipment.VehicleRegNo = null;
            $scope.shipment.Timezone = null;
            $scope.shipment.ShipmentReadyBy = null;
            $scope.shipment.PickupDate = null;
            $scope.shipment.Warehouse = null;
            $scope.shipment.TransportToWarehouse = null;
        }
        if (value === "ThirdPartyExport") {
            // set custom information panel
            if ($scope.shipment.DeliveredBy !== null && $scope.shipment.DeliveredBy.CourierType == "Courier" && $scope.shipment.PickupAddress.Country !== null) {
                if ($scope.shipment.PickupAddress.Country.CountryId !== $scope.shipment.ReceiverAddress.Country.CountryId) {
                    $scope.CustomInfo = true;
                    $scope.showAndHideCustomInfo();
                }
                else {
                    $scope.CustomInfo = false;
                    $scope.shipment.CustomInfo = null;
                }
            }
            $scope.shipment.TrackingNumber = null;
            $scope.shipment.OtherTransportToWareHouseCarrier = null;
            $scope.shipment.TransportToWareHouseCarrier = null;
            $scope.shipment.VehicleRegNo = null;
            $scope.shipment.Timezone = null;
            $scope.shipment.ShipmentReadyBy = null;
            $scope.shipment.PickupDate = null;
            $scope.shipment.Warehouse = null;
            $scope.shipment.TransportToWarehouse = null;
            $scope.onlyShowTrackingNumber = false;
            $scope.onlyShowVehicleRegNo = false;
            $scope.onlyShowDateTime = false;
        }
    };

    $scope.SelectOption = function (transportToWareHouse) {
        if (transportToWareHouse.Name === "Own Transport") {
            $scope.onlyShowTrackingNumber = false;
            $scope.onlyShowVehicleRegNo = true;
            $scope.onlyShowDateTime = false;
            $scope.shipment.TrackingNumber = null;
            $scope.shipment.OtherTransportToWareHouseCarrier = null;
            $scope.shipment.TransportToWareHouseCarrier = null;
            $scope.shipment.Timezone = null;
            $scope.shipment.ShipmentReadyBy = null;
            $scope.shipment.PickupDate = null;
        }
        if (transportToWareHouse.Name === "Courier") {
            $scope.otherCourierTranport = false;
            $scope.shipment.OtherTransportToWareHouseCarrier = null;
            $scope.onlyShowTrackingNumber = true;
            $scope.onlyShowVehicleRegNo = false;
            $scope.onlyShowDateTime = false;
            $scope.shipment.VehicleRegNo = null;
            $scope.shipment.Timezone = null;
            $scope.shipment.ShipmentReadyBy = null;
            $scope.shipment.PickupDate = null;

        }
        if (transportToWareHouse.Name === "Frayte To Collect") {
            $scope.onlyShowTrackingNumber = false;
            $scope.onlyShowVehicleRegNo = false;
            $scope.shipment.OtherTransportToWareHouseCarrier = null;
            $scope.onlyShowDateTime = true;
            $scope.shipment.PickupDate = new Date();
            $scope.shipment.TrackingNumber = null;
            $scope.shipment.TransportToWareHouseCarrier = null;
            $scope.shipment.VehicleRegNo = null;
            if ($scope.shipment.OtherPickupAddress) {
                if ($scope.restoreTimeZone1 !== undefined && $scope.restoreTimeZone1 !== null && $scope.restoreTimeZone1.TimezoneId > 0) {
                    $scope.shipment.Timezone = $scope.restoreTimeZone1;
                }
            }
            else {

                $scope.shipment.Timezone = $scope.restoreTimeZone;
            }

        }

    };
    // Need to set country based on pickup address country code
    $scope.PickupAddressCountry = function (data) {
        var countries = $scope.countries;
        if (data !== null) {
            for (var i = 0; i < countries.length; i++) {
                if (countries[i].Name == data.Name) {
                    $scope.shipment.PickupAddress.Country = countries[i];
                    break;
                }
            }
        }
        //To Do
        // Set Time Zone Based on Country Code

    };


    //Pieces Handing

    //$scope.pieceDetailOption 
    $scope.changeKgToLb = function (pieceDetailOption) {
        if (pieceDetailOption === "kgToCms") {
            // for kg
            $translate('kGS').then(function (kGS) {
                $scope.Lb_Kgs = kGS;
            });
            $translate('CMS').then(function (CMS) {
                $scope.Lb_Inch = CMS;
            });
        }
        if (pieceDetailOption === "lbToInchs") {
            // for LB
            $translate('LB').then(function (LB) {
                $scope.Lb_Kgs = LB;
            });
            $translate('Inchs').then(function (Inchs) {
                $scope.Lb_Inch = Inchs;
            });
        }
    };


    $scope.AddPieces = function () {
        $scope.shipment.ShipmentDetails.push({
            JobNumber: '',
            JobStyle: '',
            Pieces: null,
            WeightKg: null,
            Lcms: null,
            Wcms: null,
            Hcms: null,
            PiecesContent: ''
        });
    };

    $scope.WhileAddingExcel = function ($files, $file, $event) {
        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextPleaseSelectValidFile,
                showCloseButton: true
            });
            return;
        }
        // download excel
        $scope.downloadExcel = function () {
            ShipmentService.DownLoadSampleFile().then(function (data) {
            });
        };
        // Upload the excel file here.
        $scope.uploadExcel = Upload.upload({

            url: config.SERVICE_URL + '/Shipment/GetPiecesDetail',
            fields: {
                shipmentid: $scope.shipmentId
            },
            file: $file

        });

        $scope.uploadExcel.progress($scope.progressExcel);

        $scope.uploadExcel.success($scope.successExcel);

        $scope.uploadExcel.error($scope.errorExcel);
    };

    $scope.progressExcel = function (evt) {
        //console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
        //toaster.clear();
        //toaster.pop({
        //    type: 'success',
        //    title: 'uploading',
        //    body: 'percent: ' + parseInt(100.0 * evt.loaded / evt.total, 10),
        //    showCloseButton: true
        //});
    };

    $scope.successExcel = function (data, status, headers, config) {
        if (status = 200) {

            toaster.pop({
                type: 'success',
                title: $scope.TitleFrayteSuccess,
                body: $scope.TextUploadedSuccessfully,
                showCloseButton: true
            });

            //  To Do: Logic to add row in Piecees grid.
            var result = data;

            if (result.FrayteShipmentDetail !== null) {
                for (var i = 0 ; i < result.FrayteShipmentDetail.length; i++) {
                    $scope.shipment.ShipmentDetails.push(result.FrayteShipmentDetail[i]);
                }
                if ($scope.piecesCountInitial == 1) {
                    $scope.shipment.ShipmentDetails.splice(0, 1);
                }
                $scope.piecesCountInitial++;
            }

        }

    };

    $scope.errorExcel = function (err) {
        toaster.pop({
            type: 'error',
            title: $scope.TitleFrayteError,
            body: $scope.TextErrorOccuredDuringUpload,
            showCloseButton: true
        });
    };

    //Pieces Removing
    $scope.RemovePieces = function (shippingDetail) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'shipment/shipmentDetailDelete.tpl.html',
            windowClass: '',
            size: 'md'
        });

        modalInstance.result.then(function (reason) {
            if (reason == 'remove') {
                var index = $scope.shipment.ShipmentDetails.indexOf(shippingDetail);
                $scope.shipment.ShipmentDetails.splice(index, 1);
            }
        },
        function () {
        });
    };

    $scope.showAddPieces = function () {

        if ($scope.shipment === undefined) {
            return false;
        }
        else if ($scope.shipment.ShipmentDetails === undefined) {
            return false;
        }
        else if ($scope.shipment.ShipmentDetails.length >= 2) {
            return true;
        }
        else {
            return false;
        }
    };

    $scope.getTotalPieces = function () {
        if ($scope.shipment === undefined) {
            return 0;
        }
        else if ($scope.shipment.ShipmentDetails === undefined) {
            return 0;
        }
        else if ($scope.shipment.ShipmentDetails.length >= 1) {
            var total = parseInt(0, 10);
            for (var i = 0; i < $scope.shipment.ShipmentDetails.length; i++) {
                var product = $scope.shipment.ShipmentDetails[i];
                if (product.Pieces === null || product.Pieces === undefined) {
                    total += parseInt(0, 10);
                }
                else {
                    total = total + parseInt(product.Pieces, 10);
                }
            }
            return total;
        }
        else {
            return 0;
        }
    };

    $scope.getTotalKgs = function () {
        if ($scope.shipment === undefined) {
            return 0;
        }
        else if ($scope.shipment.ShipmentDetails === undefined) {
            return 0;
        }
        else if ($scope.shipment.ShipmentDetails.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.shipment.ShipmentDetails.length; i++) {
                var product = $scope.shipment.ShipmentDetails[i];
                if (product.WeightKg === null || product.WeightKg === undefined) {
                    total += parseFloat(0);
                }
                else {
                    total = total + parseFloat(product.WeightKg) * product.CartonQty;
                }
            }
            return Math.ceil(total);
        }
        else {
            return 0;
        }
    };

    $scope.getTotalCBM = function () {
        if ($scope.shipment === undefined) {
            return;
        }

        var shippingMethod = $scope.shipment.DeliveredBy;
        if ($scope.shipment === undefined) {
            return 0;
        }
        else if ($scope.shipment.ShipmentDetails === undefined) {
            return 0;
        }
        else if ($scope.shipment.ShipmentDetails.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.shipment.ShipmentDetails.length; i++) {
                var product = $scope.shipment.ShipmentDetails[i];
                var len = 0;
                var wid = 0;
                var height = 0;
                if (product.Lcms === null || product.Lcms === undefined) {
                    len = parseFloat(0);
                }
                else {
                    len = parseFloat(product.Lcms);
                }

                if (product.Wcms === null || product.Wcms === undefined) {
                    wid = parseFloat(0);
                }
                else {
                    wid = parseFloat(product.Wcms);
                }

                if (product.Hcms === null || product.Hcms === undefined) {
                    height = parseFloat(0);
                }
                else {
                    height = parseFloat(product.Hcms);
                }

                if (len > 0 && wid > 0 && height > 0) {
                    if (shippingMethod.CourierType === "Sea") {
                        if ($scope.shipment.PiecesCaculatonType === "kgToCms") {
                            total += ((len / 100) * (wid / 100) * (height / 100)) * product.CartonQty;
                        }
                        else if ($scope.shipment.PiecesCaculatonType === "lbToInchs") {
                            total += ((len / 39.37) * (wid / 39.37) * (height / 39.37)) * product.CartonQty;
                        }

                    }
                }
            }
            return total.toFixed(2);
        }
        else {
            return 0;
        }
    };

    $scope.getEstimatedChargeableCBM = function (totalCBM) {
        if (totalCBM === undefined && totalCBM === '' && totalCBM === null) {
            return 0;
        }
        if (totalCBM <= 2.5) {
            return 2.5;
        }
        else {
            var num = [];
            num = totalCBM.toString().split('.');
            if (num.length > 1) {
                var as = parseFloat(num[1]);
                if (as === 0) {
                    return parseFloat(num[0]).toFixed(2);
                }
                else {
                    if (as > 49) {
                        return parseFloat(num[0]) + 1;
                    }
                    else {
                        return parseFloat(num[0]) + 0.50;
                    }
                }
              
            }
            else {
                return parseFloat(num[0]).toFixed(2);
            }
        }
    };

    $scope.getTotalVolWeight = function () {
        if ($scope.shipment === undefined) {
            return;
        }

        var shippingMethod = $scope.shipment.DeliveredBy;
        if ($scope.shipment === undefined) {
            return 0;
        }
        else if ($scope.shipment.ShipmentDetails === undefined) {
            return 0;
        }
        else if ($scope.shipment.ShipmentDetails.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.shipment.ShipmentDetails.length; i++) {
                var product = $scope.shipment.ShipmentDetails[i];
                var len = 0;
                var wid = 0;
                var height = 0;
                if (product.Lcms === null || product.Lcms === undefined) {
                    len = parseFloat(0);
                }
                else {
                    len = parseFloat(product.Lcms);
                }

                if (product.Wcms === null || product.Wcms === undefined) {
                    wid = parseFloat(0);
                }
                else {
                    wid = parseFloat(product.Wcms);
                }

                if (product.Hcms === null || product.Hcms === undefined) {
                    height = parseFloat(0);
                }
                else {
                    height = parseFloat(product.Hcms);
                }

                if (len > 0 && wid > 0 && height > 0) {
                    if (shippingMethod.CourierType === "Air") {
                        total += ((len * wid * height) / 6000) * product.CartonQty;

                    }
                    else if (shippingMethod.CourierType === "Sea") {
                        total += ((len * wid * height) / 1000000) * product.CartonQty;

                    }
                    else if (shippingMethod.CourierType === "Courier") {
                        total += ((len * wid * height) / 5000) * product.CartonQty;

                    }

                }
            }
            return Math.ceil(total);
        }
        else {
            return 0;
        }
    };

    $scope.getDate = function () {
        var dateVal = $filter('date')(shipment.ShippingDate, 'dd/MM/yyyy');
        return dateVal;
    };

    $scope.CheckCustomInfo = function () {
        if ($scope.shipment.DeliveredBy !== null) {
            if ($scope.shipment.DeliveredBy.CourierType == "Courier") {
                if ($scope.shipment.ExportType == "Export") {
                    if ($scope.shipment.Warehouse !== null) {
                        if ($scope.shipment.Warehouse.CountryId > 0 && $scope.shipment.Warehouse.CountryId !== $scope.shipment.ReceiverAddress.Country.CountryId) {
                            $scope.CustomInfo = true;
                            $scope.showAndHideCustomInfo();
                        }
                        else {
                            $scope.CustomInfo = false;
                            $scope.shipment.CustomInfo = null;
                        }
                    }

                }
                if ($scope.shipment.ExportType == null || $scope.shipment.ExportType == "ThirdPartyExport") {
                    if ($scope.shipment.OtherPickupAddress === false) {
                        if ($scope.shipment.ShipperAddress.Country.CountryId !== $scope.shipment.ReceiverAddress.Country.CountryId) {
                            $scope.CustomInfo = true;
                            $scope.showAndHideCustomInfo();
                        }
                        else {
                            $scope.CustomInfo = false;
                            $scope.shipment.CustomInfo = null;
                        }
                    }
                    else {
                        if ($scope.shipment.PickupAddress !== null && $scope.shipment.PickupAddress.Country !== null && $scope.shipment.PickupAddress.Country.CountryId > 0) {
                            if ($scope.shipment.PickupAddress.Country.CountryId !== $scope.shipment.ReceiverAddress.Country.CountryId) {
                                $scope.CustomInfo = true;
                                $scope.showAndHideCustomInfo();
                            }
                            else {
                                $scope.CustomInfo = false;
                                $scope.shipment.CustomInfo = null;
                            }
                        }
                    }
                }
            }
        }
    };

    // Add receiver while shipper is logged in and making shipment 

    $scope.AddEditReceiver = function (row) {
        $state.go('shipper.shipment.addreceiver', { "shipperId": $scope.shipment.Shipper.UserId, "receiverId": 0 });
    };

    $scope.OtherPickupAddress = function (otherPickupAddress) {
        // Check for Custom Info Section
        $scope.CheckCustomInfo();

        // Make fields readonly and editable
        if (otherPickupAddress) {
            $scope.checked = false;
        }
        else {
            $scope.checked = true;
        }

        if (otherPickupAddress) {
            $scope.shipment.PickupAddress.Country = null;
            $scope.shipment.PickupAddress.Address = '';
            $scope.shipment.PickupAddress.Address2 = '';
            $scope.shipment.PickupAddress.Address3 = '';
            $scope.shipment.PickupAddress.Zip = '';
            $scope.shipment.PickupAddress.City = '';
            $scope.shipment.PickupAddress.Suburb = '';
            $scope.shipment.PickupAddress.State = '';
            $scope.shipment.ShipmentPickupContactPhoneNumber = '';
            $scope.shipment.ShipmentPickupContactName = '';
            $scope.shipment.ShipmentPickupContactPhoneNumberCode = null;
        }
        else {
            $scope.shipment.ShipmentPickupContactName = $scope.shipment.Shipper.ContactName;
            $scope.shipment.ShipmentPickupContactPhoneNumber = $scope.shipment.Shipper.TelephoneNo;
            $scope.shipment.ShipmentPickupContactPhoneNumberCode = $scope.shipment.Shipper.TelephoneCode;
            $scope.shipment.PickupAddress.Country = $scope.shipment.ShipperAddress.Country;
            $scope.shipment.PickupAddress.Address = $scope.shipment.ShipperAddress.Address;
            $scope.shipment.PickupAddress.Address2 = $scope.shipment.ShipperAddress.Address2;
            $scope.shipment.PickupAddress.Address3 = $scope.shipment.ShipperAddress.Address3;
            $scope.shipment.PickupAddress.Zip = $scope.shipment.ShipperAddress.Zip;
            $scope.shipment.PickupAddress.City = $scope.shipment.ShipperAddress.City;
            $scope.shipment.PickupAddress.Suburb = $scope.shipment.ShipperAddress.Suburb;
            $scope.shipment.PickupAddress.State = $scope.shipment.ShipperAddress.State;
            $scope.shipment.Timezone = $scope.restoreTimeZone;
        }
    };

    // option for courier dropdown'
    $scope.CourierExportTypes = [{
        name: 'SF Express'
    }, {
        name: 'DHL'
    }, {
        name: 'UPS'
    }, {
        name: 'TNT'
    }, {
        name: 'Fedex'
    }, {
        name: 'Other'
    }];
    $scope.OpenTermAndCondition = function () {

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'termAndCondition/termAndConditionDetail.tpl.html',
            controller: 'TermAndConditionDetailController',
            windowClass: '',
            size: 'lg',
            resolve: {
                termAndConditionId: function () {
                    return $scope.TermAndConditionId;
                }
            }
        });
    };
    // term and condition
    $scope.termAndCondition = function (asd) {
        $scope.shipment.TermAndCondition = !$scope.shipment.TermAndCondition;
    };

    // Set Shipper Country and CountryPhoneCode based on Timezone
    $scope.shipperCountryOnTomeZone = function (timeZone) {
        if (timeZone !== undefined && timeZone !== null && timeZone.TimezoneId > 0) {
            ShipmentService.GetCountryByTimezone(timeZone.TimezoneId).then(function (response) {
                if (response.data !== null) {
                    $scope.shipment.ShipperAddress.Country = response.data;
                    if ($scope.shipment.ShipperAddress !== null && $scope.shipment.ShipperAddress.Country !== null && $scope.shipment.ShipperAddress.Country.Code === "HKG") {
                        $scope.setShipperStateDisable = true;
                        $scope.setShipperZipDisable = true;
                        $scope.shipment.ShipperAddress.Zip = null;
                        $scope.shipment.ShipperAddress.State = null;
                    }
                    else if ($scope.shipment.ShipperAddress !== null && $scope.shipment.ShipperAddress.Country !== null && $scope.shipment.ShipperAddress.Country.Code === "GBR") {
                        $scope.setShipperStateDisable = true;
                        $scope.setShipperZipDisable = false;
                        $scope.shipment.ShipperAddress.State = null;
                    }
                    else {
                        $scope.setShipperZipDisable = false;
                        $scope.setShipperStateDisable = false;
                    }
                    var objects = $scope.countryPhoneCodes;
                    for (var i = 0; i < objects.length; i++) {
                        if (objects[i].Name.indexOf($scope.shipment.ShipperAddress.Country.Name) > -1) {
                            $scope.shipment.Shipper.TelephoneCode = objects[i];
                            $scope.shipment.Shipper.MobileCode = objects[i];
                            break;
                        }
                    }
                }
                else {
                    console.log("Can not set null.");
                }


            },
          function () {
              console.log("Can not set country based on timezone");
          });
        }

    };
    // Set Receiver Country and CountryPhoneCode based on Timezone
    $scope.receiverCountryOnTomeZone = function (timeZone) {
        if (timeZone !== undefined && timeZone !== null && timeZone.TimezoneId > 0) {
            ShipmentService.GetCountryByTimezone(timeZone.TimezoneId).then(function (response) {
                if (response.data !== null) {
                    $scope.shipment.ReceiverAddress.Country = response.data;
                    if ($scope.shipment.ReceiverAddress !== null && $scope.shipment.ReceiverAddress.Country !== null && $scope.shipment.ReceiverAddress.Country.Code === "HKG") {
                        $scope.setReceiverStateDisable = true;
                        $scope.setReceiverZipDisable = true;
                        $scope.shipment.ReceiverAddress.Zip = null;
                        $scope.shipment.ReceiverAddress.State = null;
                    }
                    else if ($scope.shipment.ReceiverAddress !== null && $scope.shipment.ReceiverAddress.Country !== null && $scope.shipment.ReceiverAddress.Country.Code === "GBR") {
                        $scope.setReceiverStateDisable = true;
                        $scope.setReceiverZipDisable = false;
                        $scope.shipment.ReceiverAddress.State = null;
                    }
                    else {
                        $scope.setReceiverZipDisable = false;
                        $scope.setReceiverStateDisable = false;
                    }
                    var objects = $scope.countryPhoneCodes;
                    for (var i = 0; i < objects.length; i++) {
                        if (objects[i].Name.indexOf($scope.shipment.ReceiverAddress.Country.Name) > -1) {
                            $scope.shipment.Receiver.TelephoneCode = objects[i];
                            $scope.shipment.Receiver.MobileCode = objects[i];
                            break;
                        }
                    }
                }
                else {
                    console.log("Can not set null.");
                }


            },
          function () {
              console.log("Can not set country based on timezone");
          });
        }

    };

    // Receiver Country ng-change check for countryid for warehouse or pickupAddress country
    $scope.SetCountryPhoneCode = function (userType, country) {
        if (country === null) {
            return;
        }
        else {
            if (country.Code !== null && country.Code !== '' && country.Code !== undefined) {
                if (userType === 'Shipper') {
                    if (country.Code === "HKG") {
                        $scope.setShipperStateDisable = true;
                        $scope.setShipperZipDisable = true;
                        $scope.shipment.ShipperAddress.Zip = null;
                        $scope.shipment.ShipperAddress.State = null;
                    }
                    else if (country.Code === "GBR") {
                        $scope.setShipperStateDisable = true;
                        $scope.setShipperZipDisable = false;
                        $scope.shipment.ShipperAddress.State = null;
                    }
                    else {
                        $scope.setShipperZipDisable = false;
                        $scope.setShipperStateDisable = false;
                    }

                    // Set Shipper timeZone Based on Country
                    $scope.shipment.Shipper.Timezone = country.TimeZoneDetail;
                }
                else if (userType === 'Receiver') {
                    if (country.Code === "HKG") {
                        $scope.setReceiverStateDisable = true;
                        $scope.setReceiverZipDisable = true;
                        $scope.shipment.ReceiverAddress.Zip = null;
                        $scope.shipment.ReceiverAddress.State = null;
                    }
                    else if (country.Code === "GBR") {
                        $scope.setReceiverStateDisable = true;
                        $scope.setReceiverZipDisable = false;
                        $scope.shipment.ReceiverAddress.State = null;
                    }
                    else {
                        $scope.setReceiverZipDisable = false;
                        $scope.setReceiverStateDisable = false;
                    }

                    // Set Reciver timeZone Based on Country
                    $scope.shipment.Receiver.Timezone = country.TimeZoneDetail;
                }
            }
            var objects = $scope.countryPhoneCodes;
            for (var i = 0; i < objects.length; i++) {
                if (objects[i].Name.indexOf(country.Name) > -1) {
                    if (userType === 'Shipper') {
                        $scope.shipment.Shipper.TelephoneCode = objects[i];
                        $scope.shipment.Shipper.MobileCode = objects[i];
                        $scope.shipment.Shipper.FaxCode = objects[i];
                    }
                    else if (userType === 'Receiver') {
                        $scope.shipment.Receiver.TelephoneCode = objects[i];
                        $scope.shipment.Receiver.MobileCode = objects[i];
                        $scope.shipment.Receiver.FaxCode = objects[i];
                    }
                    break;
                }
            }
        }

    };

    $scope.SetPhoneCodeAndNumber = function (userType, telephoneNumber) {
        if (telephoneNumber !== undefined && telephoneNumber !== null && telephoneNumber !== '') {
            var n = telephoneNumber.indexOf(")");
            var code = telephoneNumber.substring(0, n + 1);

            if (userType === 'ShipperTelephoneNo') {
                $scope.shipment.Shipper.TelephoneNo = telephoneNumber.replace(code, "").trim();
            }
            else if (userType === 'ShipperMobileNo') {
                $scope.shipment.Shipper.MobileNo = telephoneNumber.replace(code, "").trim();
            }
                //else if (userType === 'ShipperFax') {
                //    $scope.shipment.Shipper.FaxNumber = telephoneNumber.replace(code, "").trim();
                //}
            else if (userType === 'ReceiverTelephoneNo') {
                $scope.shipment.Receiver.TelephoneNo = telephoneNumber.replace(code, "").trim();
            }
            else if (userType === 'ReceiverMobileNo') {
                $scope.shipment.Receiver.MobileNo = telephoneNumber.replace(code, "").trim();
            }
                //else if (userType === 'ReceiverFax') {
                //    $scope.shipment.Receiver.FaxNumber = telephoneNumber.replace(code, "").trim();
                //}
            else if (userType === 'PickupContactPhoneNo') {
                $scope.shipment.ShipmentPickupContactPhoneNumber = telephoneNumber.replace(code, "").trim();
            }


            var countryCode = telephoneNumber.substring(2, n);
            var objects = $scope.countryPhoneCodes;

            for (var i = 0; i < objects.length; i++) {
                if (objects[i].PhoneCode === countryCode) {
                    if (userType === 'ShipperTelephoneNo') {
                        $scope.shipment.Shipper.TelephoneCode = objects[i];
                    }
                    else if (userType === 'ShipperMobileNo') {
                        $scope.shipment.Shipper.MobileCode = objects[i];
                    }
                    else if (userType === 'ShipperFax') {
                        $scope.shipment.Shipper.FaxCode = objects[i];
                    }
                    else if (userType === 'ReceiverTelephoneNo') {
                        $scope.shipment.Receiver.TelephoneCode = objects[i];
                    }
                    else if (userType === 'ReceiverMobileNo') {
                        $scope.shipment.Receiver.MobileCode = objects[i];
                    }
                    else if (userType === 'ReceiverFax') {
                        $scope.shipment.Receiver.FaxCode = objects[i];
                    }
                    else if (userType === 'PickupContactPhoneNo') {
                        $scope.shipment.ShipmentPickupContactPhoneNumberCode = objects[i];
                    }
                    break;
                }
            }
        }
    };

    $scope.ShowConfirmation = function (shippingDetail) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'shipment/shipmentDetailDelete.tpl.html',
            windowClass: 'Logon-Modal',
            size: 'sm'
        });

        modalInstance.result.then(function (reason) {
            if (reason == 'remove') {
                var index = $scope.shipment.ShipmentDetails.indexOf(shippingDetail);
                $scope.shipment.ShipmentDetails.splice(index, 1);
            }
        },
        function () {
        });
    };

    $scope.getSelectedDeliveryOption = function (deliveredBy) {
        var objects = $scope.couriers;

        for (var i = 0; i < objects.length; i++) {
            if (objects[i].CourierId === deliveredBy.CourierId) {
                return objects[i];
            }
        }
    };

    $scope.getPackagingTypeOption = function (packagingType) {
        var objects = $scope.PackagingTypes;

        for (var i = 0; i < objects.length; i++) {
            if (objects[i].PackagingTypeId === packagingType.PackagingTypeId) {
                return objects[i];
            }
        }
    };

    $scope.CheckAccountNumber = function (accountNumber) {
        $scope.isValidAccountNumber = true;
        if (accountNumber !== undefined && accountNumber.length === 9) {
            CustomerService.CheckAccountNumber(accountNumber).then(function (response) {
                $scope.isValidAccountNumber = true;
            },
            function errorCallback(response) {
                $scope.isValidAccountNumber = false;
            });
        }
    };

    $scope.CheckEmailSh = function (email) {
        $scope.isValidEmail = false;
        if (email !== undefined && email !== null) {
            UserService.CheckUserEmail(email).then(function (response) {
                $scope.isValidEmail = true;
            },
            function errorCallback(response) {
                $scope.isValidEmail = false;
            });
        }
    };

    $scope.CheckEmailRec = function (email) {
        $scope.isValidEmailRec = false;
        if (email !== undefined && email !== null) {
            UserService.CheckUserEmail(email).then(function (response) {
                $scope.isValidEmailRec = true;
            },
            function errorCallback(response) {
                $scope.isValidEmailRec = false;
            });
        }
    };
  
    function init() {

        $scope.gridOptions.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
        };

        // set Multilingual Modal Popup Options
        setModalOptions();

        $scope.isLogin = false;
        $scope.ReceiverDetail = null;

        $scope.isValidAccountNumber = true;

        $scope.ContentsType = [
             {
                 name: 'Merchandise',
                 value: 'merchandise'
             },
             {
                 name: 'Returned Goods',
                 value: 'returned_goods'
             },
             {
                 name: 'Documents',
                 value: 'documents'
             },
             {
                 name: 'Gift',
                 value: 'gift'
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
               name: 'Abandon',
               value: 'abandon'
           },
           {
               name: 'Return',
               value: 'return'
           }
        ];

        $scope.LocationType = [
              { name: 'Warehouse' },
              { name: 'Office' }
        ];

        $scope.UserRoleType = {
            Admin: 1,
            Agent: 2,
            Customer: 3,
            Receiver: 4,
            Shipper: 5,
            Staff: 6,
            Warehouse: 7
        };

        $scope.shipmentStep = 1;

        $scope.shipmentStepStatus = {
            addressdetail: false,
            shipmentdetail: false,
            serviceoption: false,
            confirmshipment: false
        };

        $scope.otherpickupaddress = {
            checkStatus: false
        };

        $scope.openCalender = function ($event) {
            $scope.status.opened = true;
        };

        $scope.bookingPlaced = {
            Status: false
        };

        $scope.status = {
            opened: false
        };

        var userInfo = SessionService.getUser();
        if (userInfo === undefined || userInfo === null || userInfo.SessionId === undefined || userInfo.SessionId === '') {
            $scope.isLogin = false;
        }
        else {
            $scope.isLogin = true;
        }

        switch ($state.current.name) {
            case 'home.shipment.addressdetail':
                $scope.shipmentStep = 1;
                break;
            case 'home.shipment.serviceoption':
                $scope.shipmentStep = 2;
                break;
            case 'home.shipment.shipmentdetail':
                $scope.shipmentStep = 3;
                break;
            case 'home.shipment.confirmshipment':
                $scope.shipmentStep = 4;
                break;
            case 'shipper.shipment.addressdetail':
                $scope.shipmentStep = 1;
                break;
            case 'shipper.shipment.serviceoption':
                $scope.shipmentStep = 2;
                break;
            case 'shipper.shipment.shipmentdetail':
                $scope.shipmentStep = 3;
                break;
            case 'shipper.shipment.confirmshipment':
                $scope.shipmentStep = 4;
                break;
            default:
                $scope.shipmentStep = 1;
        }

        $scope.CurrencyTypes = [];

        ShipmentService.GetInitials().then(function (response) {
            $scope.WorkingWeekDays = response.data.WorkingWeekDays;

            $scope.countriesStore = response.data.Countries;

            $scope.countries = [];
            // Set Country Order as per Client Requirement
            var CountryData = $scope.countriesStore;
            for (var p = 0; p < CountryData.length; p++) {
                if (CountryData[p].Code === "CHN") {
                    $scope.countries.push({
                        CountryId: CountryData[p].CountryId,
                        Name: CountryData[p].Name,
                        Code: CountryData[p].Code,
                        Code2: CountryData[p].Code2,
                        TimeZoneDetail: CountryData[p].TimeZoneDetail,
                        OrderNumber: 1
                    });
                    CountryData.splice(p, 1);
                    break;
                }

            }
            for (var b = 0; b < CountryData.length; b++) {
                if (CountryData[b].Code === "HKG") {
                    $scope.countries.push({
                        CountryId: CountryData[b].CountryId,
                        Name: CountryData[b].Name,
                        Code: CountryData[b].Code,
                        Code2: CountryData[b].Code2,
                        TimeZoneDetail: CountryData[b].TimeZoneDetail,
                        OrderNumber: 2
                    });
                    CountryData.splice(b, 1);
                    break;
                }

            }
            for (var c = 0; c < CountryData.length; c++) {
                if (CountryData[c].Code === "IND") {
                    $scope.countries.push({
                        CountryId: CountryData[c].CountryId,
                        Name: CountryData[c].Name,
                        Code: CountryData[c].Code,
                        Code2: CountryData[c].Code2,
                        TimeZoneDetail: CountryData[c].TimeZoneDetail,
                        OrderNumber: 3
                    });
                    CountryData.splice(c, 1);
                    break;
                }

            }
            for (var d = 0; d < CountryData.length; d++) {
                if (CountryData[d].Code === "AUS") {
                    $scope.countries.push({
                        CountryId: CountryData[d].CountryId,
                        Name: CountryData[d].Name,
                        Code: CountryData[d].Code,
                        Code2: CountryData[d].Code2,
                        TimeZoneDetail: CountryData[d].TimeZoneDetail,
                        OrderNumber: 4
                    });
                    CountryData.splice(d, 1);
                    break;
                }

            }
            for (var e = 0; e < CountryData.length; e++) {
                if (CountryData[e].Code === "GBR") {
                    $scope.countries.push({
                        CountryId: CountryData[e].CountryId,
                        Name: CountryData[e].Name,
                        Code: CountryData[e].Code,
                        Code2: CountryData[e].Code2,
                        TimeZoneDetail: CountryData[e].TimeZoneDetail,
                        OrderNumber: 5
                    });
                    CountryData.splice(e, 1);
                    break;
                }

            }
            for (var f = 0; f < CountryData.length; f++) {
                if (CountryData[f].Code === "CAN") {
                    $scope.countries.push({
                        CountryId: CountryData[f].CountryId,
                        Name: CountryData[f].Name,
                        Code: CountryData[f].Code,
                        Code2: CountryData[f].Code2,
                        TimeZoneDetail: CountryData[f].TimeZoneDetail,
                        OrderNumber: 6
                    });
                    CountryData.splice(f, 1);
                    break;
                }

            }
            var z = 7;
            for (var h = 0; h < CountryData.length; h++) {
                $scope.countries.push({
                    CountryId: CountryData[h].CountryId,
                    Name: CountryData[h].Name,
                    Code: CountryData[h].Code,
                    Code2: CountryData[h].Code2,
                    TimeZoneDetail: CountryData[h].TimeZoneDetail,
                    OrderNumber: z
                });
                z++;
            }


            $scope.countryPhoneCodes = response.data.CountryPhoneCodes;
            $scope.timezones = response.data.TimeZones;
            $scope.warehouses = response.data.Warehouses;
            $scope.shipmentTypes = response.data.ShipmentTypes;
            $scope.transportToWarehouses = response.data.TransportToWarehouses;
            $scope.TermAndConditionId = response.data.TermAndConditionId;
            $scope.ShipmentTerms = response.data.ShipmentTerms;
            $scope.PackagingTypes = response.data.PackagingTypes;
            $scope.SpecialDeliveries = response.data.SpecialDeliveries;
            var curr = response.data.CurrencyTypes;
            $scope.ports = response.data.ShipmentPorts;
            // Set Shipping Method 
            $scope.couriers = [];
            var shipingMethods = response.data.Couriers;
            for (var j = 0; j < shipingMethods.length; j++) {
                $scope.couriers.push({
                    CourierId: shipingMethods[j].CourierId,
                    CourierType: shipingMethods[j].CourierType,
                    LatestBookingTime: shipingMethods[j].LatestBookingTime,
                    Name: shipingMethods[j].Name,
                    TransitTime: shipingMethods[j].TransitTime,
                    Website: shipingMethods[j].Website,
                    orderNumber: j
                });
            }
            // Set Currency type 
            for (var i = 0; i < curr.length; i++) {
                $scope.CurrencyTypes.push({
                    CurrencyCode: curr[i].CurrencyCode,
                    CurrencyDescription: curr[i].CurrencyDescription,
                    OrderNumber: i,
                    Disabled: false
                });
                if (i === 5) {
                    $scope.CurrencyTypes.push({
                        CurrencyCode: "  -------------   ",
                        CurrencyDescription: "    -----------------",
                        OrderNumber: i,
                        Disabled: true
                    });
                }
            }

            $scope.PiecesExcelDownloadPath = response.data.PiecesExcelDownloadPath;

            if ($stateParams.Id === undefined) {
                $scope.shipmentId = "0";
            }
            else {
                $scope.shipmentId = $stateParams.Id;
            }

            if ($stateParams.UserType === undefined) {
                $scope.UserType = "5";//Shipper
            }
            else {
                $scope.UserType = $stateParams.UserType;
            }

            if ($scope.shipmentId === "0") {
                $scope.newShipment();
                // Set Default WorkingWeek Day
                if ($scope.WorkingWeekDays !== null && $scope.WorkingWeekDays !== undefined && $scope.WorkingWeekDays.length > 0) {
                    var weekDays = $scope.WorkingWeekDays;
                    for (var n = 0; n < weekDays.length; n++) {
                        if (weekDays[n].IsDefault) {
                            $scope.shipment.Shipper.WorkingWeekDay = weekDays[n];
                            $scope.shipment.Receiver.WorkingWeekDay = weekDays[n];
                            break;
                        }
                    }
                }
                $scope.changeKgToLb($scope.shipment.PiecesCaculatonType);
                $scope.disableContentExplanation = true;
                $scope.disableRestrictionComment = true;
                $scope.disableCustomerSigner = true;
                $scope.CustomInfo = false;
                if ($scope.isLogin) {
                    $scope.shipment.IsLogin = true;
                    //Get Shipper main address and also get receiver's list
                    ShipperService.GetShipperDetail(userInfo.EmployeeId).then(function (response) {
                        $scope.shipment.Shipper.UserId = response.data.UserId;
                        $scope.shipment.Shipper.ShortName = response.data.ShortName;
                        $scope.shipment.Shipper.ContactName = response.data.ContactName;
                        $scope.shipment.Shipper.CompanyName = response.data.CompanyName;
                        $scope.shipment.Shipper.TelephoneNo = response.data.TelephoneNo;
                        $scope.shipment.Shipper.MobileNo = response.data.MobileNo;
                        //$scope.shipment.Shipper.FaxNumber = response.data.FaxNumber;
                        $scope.shipment.Shipper.Email = response.data.Email;
                        $scope.shipment.Shipper.VATGST = response.data.VATGST;
                        $scope.shipment.Shipper.WorkingStartTime = response.data.WorkingStartTime;
                        $scope.shipment.Shipper.WorkingEndTime = response.data.WorkingEndTime;
                        $scope.shipment.Shipper.Timezone = response.data.Timezone;
                        $scope.shipment.ShipperAddress = response.data.UserAddress;
                        //Here we need to set he value for shipper's pickup address field. if (!$scope.isLogin) {
                        $scope.SetPhoneCodeAndNumber('ShipperTelephoneNo', $scope.shipment.Shipper.TelephoneNo);
                        //$scope.SetPhoneCodeAndNumber('PickupContactPhoneNo', $scope.shipment.ShipmentPickupContactPhoneNumber);


                        //Get Shipper's receivers
                        ShipperService.GetShipperReceivers(userInfo.EmployeeId).then(function (response) {
                            $scope.ShipperReceivers = response.data;
                        });

                    });
                }
            }
            else {
                //Get the shipment detail from serdisableContentver
                ShipmentService.GetShipmentDetail($scope.shipmentId).then(function (response) {
                    $scope.shipment = response.data;
                    //To Do: Set State And Zip for pickup address 

                    // Set State and Zip for shipper and  receiver for 'HKG' and 'GBR'
                    if ($scope.shipment && $scope.shipment.ShipperAddress != null && $scope.shipment.ShipperAddress.Country.Code === "HKG") {
                        $scope.setShipperStateDisable = true;
                        $scope.setShipperZipDisable = true;
                        $scope.shipment.ShipperAddress.Zip = null;
                        $scope.shipment.ShipperAddress.State = null;
                    }
                    if ($scope.shipment && $scope.shipment.ShipperAddress != null && $scope.shipment.ShipperAddress.Country.Code === "GBR") {
                        $scope.setShipperStateDisable = true;
                        $scope.shipment.ShipperAddress.State = null;
                    }
                    if ($scope.shipment && $scope.shipment.ReceiverAddress != null && $scope.shipment.ReceiverAddress.Country.Code === "HKG") {
                        $scope.setReceiverStateDisable = true;
                        $scope.setReceiverZipDisable = true;
                        $scope.shipment.ReceiverAddress.Zip = null;
                        $scope.shipment.ReceiverAddress.State = null;
                    }
                    if ($scope.shipment && $scope.shipment.ReceiverAddress != null && $scope.shipment.ReceiverAddress.Country.Code === "GBR") {
                        $scope.setReceiverStateDisable = true;
                        $scope.shipment.ReceiverAddress.State = null;
                    }

                    $scope.changeKgToLb($scope.shipment.PiecesCaculatonType);
                    if ($scope.shipment.CustomInfo != null && $scope.shipment.CustomInfo.RestrictionType !== null && $scope.shipment.CustomInfo.ContentsType !== null) {
                        if ($scope.shipment.CustomInfo.RestrictionType === "other") {
                            $scope.disableRestrictionComment = false;
                        }
                        else if ($scope.shipment.CustomInfo.RestrictionType === "none") {
                            $scope.disableRestrictionComment = false;
                        }
                        else {
                            $scope.disableRestrictionComment = true;
                        }

                        if ($scope.shipment.CustomInfo.ContentsType === "other") {
                            $scope.disableContentExplanation = false;
                        }
                        else {
                            $scope.disableContentExplanation = true;
                        }

                        // set check custom info check box
                        $scope.shipment.checkCustomInfo = true;
                    }

                    $scope.newPaymentParty = $scope.shipment.PaymentParty;
                    $scope.newPaymentPartyTaxDuties = $scope.shipment.PaymentPartyTaxDuties;
                    if ($scope.shipment.OtherPickupAddress) {
                        $scope.checked = false;
                    }
                    else {
                        $scope.checked = true;
                    }
                    if ($scope.shipment.TransportToWarehouse !== undefined && $scope.shipment.TransportToWarehouse !== null) {
                        if ($scope.shipment.TransportToWarehouse.TransportToWarehouseId == 1) {
                            $scope.otherCourierTranport = false;
                            $scope.onlyShowTrackingNumber = false;
                            $scope.onlyShowVehicleRegNo = true;
                            $scope.onlyShowDateTime = false;
                        }
                        if ($scope.shipment.TransportToWarehouse.TransportToWarehouseId == 2) {
                            $scope.temp = false;
                            $scope.onlyShowTrackingNumber = true;
                            var data = $scope.CourierExportTypes;
                            for (var m = 0; m < data.length; m++) {
                                if (data[m].name !== $scope.shipment.TransportToWareHouseCarrier) {
                                    $scope.temp = true;
                                }
                                else {
                                    $scope.temp = false;
                                    break;
                                }
                            }
                            if ($scope.temp) {
                                $scope.otherCourierTranport = true;
                                $scope.shipment.OtherTransportToWareHouseCarrier = $scope.shipment.TransportToWareHouseCarrier;
                                $scope.shipment.TransportToWareHouseCarrier = "Other";
                            }
                            $scope.onlyShowVehicleRegNo = false;
                            $scope.onlyShowDateTime = false;
                        }
                        if ($scope.shipment.TransportToWarehouse.TransportToWarehouseId == 4) {
                            $scope.otherCourierTranport = false;
                            $scope.onlyShowTrackingNumber = false;
                            $scope.onlyShowVehicleRegNo = false;
                            $scope.onlyShowDateTime = true;
                        }
                    }


                    //if ($scope.shipment.PaymentParty === "Receiver") {
                    //    $scope.accountHolder = "Receiver";
                    //}
                    //if ($scope.shipment.PaymentParty === "Shipper") {
                    //    $scope.accountHolder = "Shipper";
                    //}

                    if ($scope.shipment.DeliveredBy !== null && $scope.shipment.DeliveredBy.CourierType === "Air") {
                        $scope.CourierCase = false;
                        $scope.onlyShow = true;
                        $scope.newVara = $scope.shipment.PaymentPartyAccountNo;
                        $scope.newVara1 = $scope.shipment.PaymentPartyTaxDutiesAccountNo;
                        //Show Custom Info Panel
                        $scope.CustomInfo = false;
                    }
                    if ($scope.shipment.DeliveredBy !== null && $scope.shipment.DeliveredBy.CourierType === "Courier") {

                        if ($scope.shipment.Warehouse.WarehouseId > 0 && $scope.shipment.Warehouse.CountryId !== $scope.shipment.ReceiverAddress.Country.CountryId) {

                            $scope.CustomInfo = true;
                        }
                        else {
                            if ($scope.shipment.PickupAddress !== null && $scope.shipment.PickupAddress.UserAddressId > 0 && $scope.shipment.PickupAddress.Country.CountryId !== $scope.shipment.ReceiverAddress.Country.CountryId) {

                                $scope.CustomInfo = true;
                            }
                            if ($scope.shipment.PickupAddress.UserAddressId === 0 && $scope.shipment.ShipperAddress.Country.CountryId !== $scope.shipment.ReceiverAddress.Country.CountryId) {

                                $scope.CustomInfo = true;
                            }
                        }
                        $scope.CourierCase = true;
                    }
                    if ($scope.shipment.DeliveredBy !== null && $scope.shipment.DeliveredBy.CourierType === "Sea") {
                        $scope.CourierCase = false;
                        //Show Custom Info Panel
                        $scope.CustomInfo = false;
                    }
                    $scope.selectType($scope.shipment.PaymentParty);
                    $scope.selectTypeTaxDuties($scope.shipment.PaymentPartyTaxDuties);

                    if (!$scope.isLogin) {
                        $scope.SetPhoneCodeAndNumber('ShipperTelephoneNo', $scope.shipment.Shipper.TelephoneNo);
                        $scope.SetPhoneCodeAndNumber('ShipperMobileNo', $scope.shipment.Shipper.MobileNo);
                        //$scope.SetPhoneCodeAndNumber('ShipperFax', $scope.shipment.Shipper.FaxNumber);

                        $scope.SetPhoneCodeAndNumber('ReceiverTelephoneNo', $scope.shipment.Receiver.TelephoneNo);
                        $scope.SetPhoneCodeAndNumber('ReceiverMobileNo', $scope.shipment.Receiver.MobileNo);
                        //$scope.SetPhoneCodeAndNumber('ReceiverFax', $scope.shipment.Receiver.FaxNumber);
                    }

                    $scope.SetPhoneCodeAndNumber('PickupContactPhoneNo', $scope.shipment.ShipmentPickupContactPhoneNumber);

                    $scope.shipment.ShippingDate = moment.utc($scope.shipment.ShippingDate).toDate();
                    $scope.shipment.PickupDate = moment.utc($scope.shipment.PickupDate).toDate();
                    $scope.shipment.DeliveredBy = $scope.getSelectedDeliveryOption($scope.shipment.DeliveredBy);
                    $scope.shipment.PackagingType = $scope.getPackagingTypeOption($scope.shipment.PackagingType);

                    if ($scope.UserType !== undefined && $scope.UserType !== null) {
                        $scope.shipment.UserRoleId = $scope.UserType;
                    }

                    if ($scope.isLogin) {
                        $scope.shipment.IsLogin = true;
                        $rootScope.$broadcast('UpdateShipmentTabCaption', 'Modify');
                        //Get Shipper's receivers
                        ShipperService.GetShipperReceivers(userInfo.EmployeeId).then(function (response) {
                            $scope.ShipperReceivers = response.data;
                            $scope.setSelectedReceiver($scope.shipment.Receiver);
                        });
                    }

                    //Set shipment's Term and Condition
                    $scope.TermAndConditionId = $scope.shipment.TermAndConditionId;

                }, function () {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorGettingShipmentDetailServer,
                        showCloseButton: true
                    });
                });
            }

        }, function () {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGettingShipmentDetailServer,
                showCloseButton: true
            });
        });

        // For Pieces Deatil Section 
        $scope.kgToCms = true;
        $scope.kgToCms = false;
        // Pieces detail
        $scope.piecesCountInitial = 1;
        if ($state.is('shipper.shipment.addressdetail')) {
            $scope.quickBooingIcon.value = false;
        }
    }
    init();
    var newVara = '';
});