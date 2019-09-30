angular.module('ngApp.tradelaneShipments').controller('TradelaneShipmentsHAWBontroller', function ($scope, $timeout, toaster, ModalService, $uibModal, CustomerService, PostCodeService, $rootScope, $translate, UtilityService, AppSpinner, TopCurrencyService, TopCountryService, $state, DirectBookingService, SessionService, $stateParams, config, CustomerId, CallFrom, TLSDetailId, TradelaneBookingService, $uibModalInstance) {

    $scope.hawb = 'HAWB';

    //Address book image code
    $scope.photoUrl = config.BUILD_URL + "addressBook.png";
    $scope.photoHazard = config.BUILD_URL + "Hazard_logo.png";

    //Prevent Form Submission on enter key press 
    function stopEnterKey(evt) {
        var evt1 = (evt) ? evt : ((event) ? event : null);
        var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
        if ((evt1.keyCode == 13) && (node.type == "text" || node.type == "checkbox" ||
            node.type == "radio" || node.type == "email" || node.type == "number" || node.type == "textarea")) {
            return false;
        }
    }

    document.onkeypress = stopEnterKey;

    $scope.toggleNotifyParty = function () {
        if ($scope.hawbAddress.IsNotifyPartySameAsReceiver) {
            $scope.hawbAddress.NotifyParty = {
                TradelaneShipmentAddressId: 0,
                TradelaneShipmentDetailId: 0,
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
                IsMailSend: false
            };
            $scope.hawbAddress.NotifyParty = $scope.hawbAddress.ShipTo;
            //$scope.hawbAddress.TradelaneShipmentDetailId = 
        }
        else {            
            $scope.NotifyPartyPhoneCode = $scope.ShipToPhoneCode;
        }
    };

    //Set Multilingual for Modal Popup
    var setMultilingualOtions = function () {
        $translate(['FrayteError', 'FrayteWarning', 'FrayteSuccess', 'FrayteValidation',
                'PleaseCorrectValidationError', 'SuccessfullyAssignedHAWB',
                'Loading_Postcode_Addresses']).then(function (translations) {
                $scope.TitleFrayteError = translations.FrayteError;
                $scope.TitleFrayteWarning = translations.FrayteWarning;
                $scope.TitleFrayteInformation = translations.FrayteSuccess;
                $scope.TitleFrayteValidation = translations.FrayteValidation;
                $scope.TitleFrayteSuccess = translations.FrayteSuccess;
                $scope.TextValidation = translations.PleaseCorrectValidationError;
                $scope.Loading_Postcode_Addresses = translations.Loading_Postcode_Addresses;
                $scope.SuccessfullyAssignedHAWB = translations.SuccessfullyAssignedHAWB;
            });
    };

    var address = function () {
        $scope.hawbAddress = {
            ShipFrom: {
                TradelaneShipmentAddressId: 0,
                TradelaneShipmentDetailId: 0,
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
                IsDefault: false
            },
            ShipTo: {
                TradelaneShipmentAddressId: 0,
                TradelaneShipmentDetailId: 0,
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
                IsDefault: false
            },
            NotifyParty: {
                TradelaneShipmentAddressId: 0,
                TradelaneShipmentDetailId: 0,
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
                IsDefault: false
            },
            IsNotifyPartySameAsReceiver: false
        };
    };

    //AddressBook 
    $scope.addressBook = function (UserType) {
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
                    return 0;
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
                    return $scope.customerId;
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
                    $scope.hawbAddress.ShipFrom = addressBooks;
                    if (addressBooks.State) {
                        $scope.hawbAddress.ShipFrom.State = addressBooks.State;
                    }

                    $scope.hawbAddress.ShipFrom.Country = addressBooks.Country;
                    $scope.hawbAddress.ShipFrom.TradelaneShipmentAddressId = 0;
                    $scope.hawbAddress.ShipFrom.TradelaneShipmentDetailId = $scope.TradelaneShipmentDetailId;
                    $scope.SetShipinfo($scope.hawbAddress.ShipFrom.Country, UserType);
                }
                else if (UserType === 'Receiver') {
                    $scope.hawbAddress.ShipTo = addressBooks;
                    if (addressBooks.State) {
                        $scope.hawbAddress.ShipTo.State = addressBooks.State;
                    }

                    $scope.hawbAddress.ShipTo.Country = addressBooks.Country;
                    $scope.hawbAddress.ShipTo.TradelaneShipmentAddressId = 0;
                    $scope.hawbAddress.ShipTo.TradelaneShipmentDetailId = $scope.TradelaneShipmentDetailId;
                    $scope.SetShipinfo($scope.hawbAddress.ShipTo.Country, UserType);
                }                
                // set form in dirty state for progress bar
                if ($scope.HAWBAddressForm) {
                    $scope.HAWBAddressForm.$dirty = true;
                }
            }
        });
    };

    $scope.shipFromValidation = function (HAWBAddressForm) {
        if (HAWBAddressForm.shipFromAddress1 && HAWBAddressForm.shipFromAddress1.$valid &&
            HAWBAddressForm.shipFromCompanyName && HAWBAddressForm.shipFromCompanyName.$valid &&
            HAWBAddressForm.shipFromFirstName && HAWBAddressForm.shipFromFirstName.$valid &&
            HAWBAddressForm.shipFromLastName && HAWBAddressForm.shipFromLastName.$valid &&
            HAWBAddressForm.shipFromCountry && HAWBAddressForm.shipFromCountry.$valid &&
            HAWBAddressForm.shipFromState && HAWBAddressForm.shipFromState.$valid &&
            HAWBAddressForm.shipFromCityName && HAWBAddressForm.shipFromCityName.$valid &&
            HAWBAddressForm.shipFromPhone && HAWBAddressForm.shipFromPhone.$valid &&
            HAWBAddressForm.shipFromMail && HAWBAddressForm.shipFromMail.$valid
        ) {
            return true;
        }
        else {
            return false;
        }
    };

    $scope.shipToValidation = function (HAWBAddressForm) {
        if (HAWBAddressForm.shipToAddress1 && HAWBAddressForm.shipToAddress1.$valid &&
            HAWBAddressForm.shipToCompanyName && HAWBAddressForm.shipToCompanyName.$valid &&
            HAWBAddressForm.shipToFirstName && HAWBAddressForm.shipToFirstName.$valid &&
            HAWBAddressForm.shipToLastName && HAWBAddressForm.shipToLastName.$valid &&
            HAWBAddressForm.shipToCountry && HAWBAddressForm.shipToCountry.$valid &&
            HAWBAddressForm.shipToState && HAWBAddressForm.shipToState.$valid &&
            HAWBAddressForm.shipToCityName && HAWBAddressForm.shipToCityName.$valid &&
            HAWBAddressForm.shipToPhone && HAWBAddressForm.shipToPhone.$valid &&
            HAWBAddressForm.shipToMail && HAWBAddressForm.shipToMail.$valid
        ) {
            return true;
        }
        else {
            return false;
        }
    };

    $scope.NotifyPartyValidation = function (HAWBAddressForm) {
        if ($scope.hawbAddress && $scope.hawbAddress.IsNotifyPartySameAsReceiver) {
            return true;
        }
        if (HAWBAddressForm.notifyAddress1 && HAWBAddressForm.notifyAddress1.$valid &&
            HAWBAddressForm.notifyCompanyName && HAWBAddressForm.notifyCompanyName.$valid &&
            HAWBAddressForm.notifyFirstName && HAWBAddressForm.notifyFirstName.$valid &&
            HAWBAddressForm.notifyLastName && HAWBAddressForm.notifyLastName.$valid &&
            HAWBAddressForm.notifyCountry && HAWBAddressForm.notifyCountry.$valid &&
            HAWBAddressForm.notifyState && HAWBAddressForm.notifyState.$valid &&
            HAWBAddressForm.notifyCityName && HAWBAddressForm.notifyCityName.$valid &&
            HAWBAddressForm.notifyPhone && HAWBAddressForm.notifyPhone.$valid &&
            HAWBAddressForm.notifyMail && HAWBAddressForm.notifyMail.$valid
        ) {
            return true;
        }
        else {
            return false;
        }
    };

    $scope.SetNotifyPartyAddress = function () {
        if ($scope.hawbAddress.IsNotifyPartySameAsReceiver) {
            $scope.hawbAddress.NotifyParty = $scope.hawbAddress.ShipTo;
            $scope.hawbAddress.NotifyParty.TradelaneShipmentDetailId = $scope.TradelaneShipmentDetailId;
        }
        else {
            $scope.hawbAddress.NotifyParty.TradelaneShipmentDetailId = $scope.TradelaneShipmentDetailId;
        }
    };

    $scope.SaveHAWBAddress = function (HAWBAddressForm) {
        if ($scope.shipFromValidation(HAWBAddressForm) && $scope.shipToValidation(HAWBAddressForm) && $scope.NotifyPartyValidation(HAWBAddressForm)) {
            $scope.SetNotifyPartyAddress();
            TradelaneBookingService.SaveHAWBAddress($scope.hawbAddress).then(function (response) {
                if (response.data !== undefined && response.data !== null && response.data !== '' && response.data === true) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.SuccessfullyAssignedHAWB,
                        showCloseButton: true
                    });
                    $uibModalInstance.close($scope.hawbAddress);
                }
            });            
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.TextValidation,
                showCloseButton: true
            });
        }
    };

    //Set Country Phone Code
    $scope.SetShipinfo = function (Country, Action) {
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
            }
            else if (Action === 'notify') {
                if (Country.Code === 'GBR') {
                    $scope.MaximamLengthNotiFy = 9;
                }
                else {
                    $scope.MaximamLengthNotyiFy = 16;
                }
                $scope.showPostCodeDropDown = false;
                for (var k = 0; k < $scope.CountryPhoneCodes.length; k++) {
                    if ($scope.CountryPhoneCodes[k].CountryCode === Country.Code) {
                        $scope.NotifyPartyPhoneCode = "(+" + $scope.CountryPhoneCodes[k].PhoneCode + ")";
                        break;
                    }
                }
            }
            setStatePostCodeForHKGUK(Country, Action);
        }
    };    

    $scope.disabled = false;

    // Post code search 
    $scope.GetPostCodeAddress = function (PostCode, CountryCode2, Type) {
        if (PostCode && PostCode.length > 5 && ((Type === 'Shipper' && PostCode.length > $scope.hawbAddress.ShipFrom.PostCode.length) || (Type === 'Receiver' && PostCode.length > $scope.hawbAddress.ShipTo.PostCode.length) || (Type === 'notify' && PostCode.length > $scope.hawbAddress.NotifyParty.PostCode.length)) && CountryCode2 && CountryCode2 === "GB") {
            $scope.disabled = true;
            AppSpinner.showSpinnerTemplate($scope.Loading_Postcode_Addresses, $scope.Template);
            return PostCodeService.AllPostCode(PostCode, CountryCode2).then(function (response) {
                $scope.disabled = false;
                $scope.PostCodeAddressValue = false;
                $scope.ReceiverPostCodeAddressValue = false;
                $scope.NotifyCodeAddressValue = false;
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
                    else if (Type === 'notify') {
                        $scope.NotifyCodeAddressValue = true;
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
                else if (Type === 'notify') {
                    $scope.NotifyCodeAddressValue = true;
                }
                AppSpinner.hideSpinnerTemplate();
            });
        }
    };

    $scope.onSelectPostCode = function ($item, $model, $label, $event, PostCode, Type) {
        if (PostCode && Type) {
            if (Type === 'Shipper') {
                $scope.hawbAddress.ShipFrom.PostCode = $item.PostCode;
                $scope.hawbAddress.ShipFrom.Address = $item.Address1;
                $scope.hawbAddress.ShipFrom.Address2 = $item.Address2;
                $scope.hawbAddress.ShipFrom.Area = $item.Area;
                $scope.hawbAddress.ShipFrom.City = $item.City;
                $scope.hawbAddress.ShipFrom.CompanyName = $item.CompanyName;
            }
            else if (Type === 'Receiver') {
                $scope.hawbAddress.ShipTo.PostCode = $item.PostCode;
                $scope.hawbAddress.ShipTo.Address = $item.Address1;
                $scope.hawbAddress.ShipTo.Address2 = $item.Address2;
                $scope.hawbAddress.ShipTo.Area = $item.Area;
                $scope.hawbAddress.ShipTo.City = $item.City;
                $scope.hawbAddress.ShipTo.CompanyName = $item.CompanyName;
            }
            else if (Type === 'notify') {
                $scope.hawbAddress.NotifyParty.PostCode = $item.PostCode;
                $scope.hawbAddress.NotifyParty.Address = $item.Address1;
                $scope.hawbAddress.NotifyParty.Address2 = $item.Address2;
                $scope.hawbAddress.NotifyParty.Area = $item.Area;
                $scope.hawbAddress.NotifyParty.City = $item.City;
                $scope.hawbAddress.NotifyParty.CompanyName = $item.CompanyName;
            }
        }
    };

    $scope.SetPostCodeAddressValue = function (Type) {
        if ($scope.fillPostlValues && $scope.fillPostlValues.length) {
            if (Type === 'Shipper') {
                $scope.hawbAddress.ShipFrom.City = $scope.fillPostlValues[0].City;
            }
            else if (Type === "Receiver") {
                $scope.hawbAddress.ShipTo.City = $scope.fillPostlValues[0].City;
            }
            else if (Type === "notify") {
                $scope.hawbAddress.NotifyParty.City = $scope.fillPostlValues[0].City;
            }
        }
        else {
            if (Type === 'Shipper') {
                $scope.PostCodeAddressValue = false;
            }
            else if (Type === "Receiver") {
                $scope.ShipperPostCodeAddressValue = false;
            }
            else if (Type === "notify") {
                $scope.NotifyCodeAddressValue = false;
            }
            else {
                $scope.PostCodeAddressValue = false;
                $scope.NotifyCodeAddressValue = false;
                $scope.ShipperPostCodeAddressValue = false;
            }
        }
    };

    var setStatePostCodeForHKGUK = function (Country, Type) {
        if (Country.Code === 'HKG') {
            if (Type === 'Shipper') {
                $scope.hawbAddress.ShipFrom.PostCode = null;
                $scope.hawbAddress.ShipFrom.State = null;
            }
            else if (Type === 'Receiver') {
                $scope.hawbAddress.ShipTo.PostCode = null;
                $scope.hawbAddress.ShipTo.State = null;
            }
            else if (Type === 'notify') {
                $scope.hawbAddress.NotifyParty.PostCode = null;
                $scope.hawbAddress.NotifyParty.State = null;
            }
        }
        else if (Country.Code === 'GBR') {
            if (Type === 'Shipper') {
                $scope.hawbAddress.ShipFrom.State = null;
            }
            else if (Type === 'Receiver') {
                $scope.hawbAddress.ShipTo.State = null;
            }
            else if (Type === 'notify') {
                $scope.hawbAddress.NotifyParty.State = null;
            }
        }
    };

    $scope.showPopUp = function (HAWB, HAWBNumber) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'hawbAddress/pacakgeDetail.tpl.html',
            controller: 'TradelanePacakgeDetailController',
            windowClass: 'directBookingDetail',
            size: 'lg',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                ShipmentId: function () {
                    return $scope.hawbAddress.TradelaneShipmentId;
                },
                PackageCalculatonType: function () {
                    return $scope.hawbAddress.PakageCalculatonType;
                },
                FrayteNumber: function () {
                    return $scope.hawbAddress.FrayteNumber;
                },
                HAWB: function () {
                    if (HAWB) {
                        return HAWB;
                    }
                    else {
                        return "";
                    }
                },
                HAWBNumber: function () {
                    if (HAWBNumber) {
                        return HAWBNumber;
                    }
                    else {
                        return 0;
                    }
                },

                TotalUploaded: function () {
                    return $scope.Booking.TotalShipments;
                },
                SuccessUploaded: function () {
                    return 0;
                }
            }
        });
        modalInstance.result.then(function (response) {
            hawbpackages();
        }, function () {
        });
    };
   
    var getScreenInitials = function () {
        TradelaneBookingService.BookingInitials($scope.userInfo.EmployeeId).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            // Set Country type according to given order
            $scope.CountriesRepo = TopCountryService.TopCountryList(response.data.Countries);
            $scope.CountryPhoneCodes = response.data.CountryPhoneCodes;
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

    //If Shipper select new customer or he is not assigned any 
    $scope.newCustomerAccountInfo = false;

    $scope.setCustomerInfo = function (CustomerDetail) {
        if (CustomerDetail) {
            if (CustomerDetail.CustomerId) {
                $scope.hawbAddress.CustomerAccountNumber = CustomerDetail.AccountNumber;
                $scope.hawbAddress.customerId = CustomerDetail.CustomerId;
                $scope.customerId = CustomerDetail.CustomerId;

                $scope.newCustomerAccountInfo = false;
                $scope.isValidAccountNumber = true;

                getCustomerDefaultAddress();
            }
            else {
                $scope.hawbAddress.CustomerAccountNumber = '';
                $scope.newCustomerAccountInfo = true;
            }
        }
    };

    $scope.SetCustomerAddress = function () {

        if (parseInt($stateParams.shipmentId, 10)) {
            if ($scope.CustomerAddress.ShipFrom) {
                $scope.shipFromDefault = $scope.CustomerAddress.ShipFrom.IsDefault;
            }
            else {
                $scope.shipFromDefault = false;
            }
            if ($scope.CustomerAddress.ShipTo) {
                $scope.shipToDefault = $scope.CustomerAddress.ShipTo.IsDefault;
            }
            else {
                $scope.shipToDefault = false;
            }
        }
        else {
            if ($scope.CustomerAddress.ShipFrom) {
                $scope.hawbAddress.ShipFrom.Country = $scope.CustomerAddress.ShipFrom.Country;
                $scope.hawbAddress.ShipFrom.PostCode = $scope.CustomerAddress.ShipFrom.PostCode;
                $scope.hawbAddress.ShipFrom.FirstName = $scope.CustomerAddress.ShipFrom.FirstName;
                $scope.hawbAddress.ShipFrom.LastName = $scope.CustomerAddress.ShipFrom.LastName;
                $scope.hawbAddress.ShipFrom.CompanyName = $scope.CustomerAddress.ShipFrom.CompanyName;
                $scope.hawbAddress.ShipFrom.Address = $scope.CustomerAddress.ShipFrom.Address;
                $scope.hawbAddress.ShipFrom.Address2 = $scope.CustomerAddress.ShipFrom.Address2;
                $scope.hawbAddress.ShipFrom.City = $scope.CustomerAddress.ShipFrom.City;
                $scope.hawbAddress.ShipFrom.Area = $scope.CustomerAddress.ShipFrom.Suburb;
                $scope.hawbAddress.ShipFrom.State = $scope.CustomerAddress.ShipFrom.State;
                $scope.hawbAddress.ShipFrom.Phone = $scope.CustomerAddress.ShipFrom.Phone;

                $scope.SetShipinfo($scope.hawbAddress.ShipFrom.Country, "Shipper");

                $scope.shipFromDefault = $scope.CustomerAddress.ShipFrom.IsDefault;

            }
            else {
                $scope.shipFromDefault = false;
            }

            if ($scope.CustomerAddress.ShipTo) {
                $scope.hawbAddress.ShipTo.Country = $scope.CustomerAddress.ShipTo.Country;
                $scope.hawbAddress.ShipTo.PostCode = $scope.CustomerAddress.ShipTo.PostCode;
                $scope.hawbAddress.ShipTo.FirstName = $scope.CustomerAddress.ShipTo.FirstName;
                $scope.hawbAddress.ShipTo.LastName = $scope.CustomerAddress.ShipTo.LastName;
                $scope.hawbAddress.ShipTo.CompanyName = $scope.CustomerAddress.ShipTo.CompanyName;
                $scope.hawbAddress.ShipTo.Address = $scope.CustomerAddress.ShipTo.Address;
                $scope.hawbAddress.ShipTo.Address2 = $scope.CustomerAddress.ShipTo.Address2;
                $scope.hawbAddress.ShipTo.City = $scope.CustomerAddress.ShipTo.City;
                $scope.hawbAddress.ShipTo.Area = $scope.CustomerAddress.ShipTo.Suburb;
                $scope.hawbAddress.ShipTo.State = $scope.CustomerAddress.ShipTo.State;
                $scope.hawbAddress.ShipTo.Phone = $scope.CustomerAddress.ShipTo.Phone;

                $scope.SetShipinfo($scope.hawbAddress.ShipTo.Country, "Receiver");

                $scope.shipToDefault = $scope.CustomerAddress.ShipTo.IsDefault;

            }
            else {
                $scope.shipToDefault = false;
            }
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

    function init() {
        
        $scope.submitted = true;
        $scope.emailFormat = /^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,10};$/;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';

        $scope.ImagePath = config.BUILD_URL;
        $scope.TradelaneShipmentDetailId = TLSDetailId;

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

        }
        if ($scope.userInfo.RoleId === 3) {
            $scope.customerId = $scope.userInfo.EmployeeId;
        }
        else {
            if (CallFrom === 'PackageDetail') {
                $scope.customerId = CustomerId;
            }
            else {
                $scope.customerId = 0;
            }
        }

        $scope.CreatedBy = $scope.userInfo.EmployeeId;
        $scope.CountriesRepo = [];
        $scope.CurrencyTypes = [];
        setMultilingualOtions();
        if (isNaN($stateParams.shipmentId)) {
        }
        else {
            getScreenInitials();
        }

        address();
    }

    init();
});