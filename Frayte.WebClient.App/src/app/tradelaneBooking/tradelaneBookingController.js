angular.module('ngApp.tradelaneBooking').controller("TradelaneBookingController", function ($scope, $timeout, Upload, toaster, ModalService, $uibModal,   CustomerService, PostCodeService, TopAirlineService, $rootScope, TradelaneBookingService, $translate, UtilityService, AppSpinner, TopCurrencyService, TopCountryService, $state, DirectBookingService, SessionService, $stateParams, config) {

    //collapse download template code
    $scope.downloadTemplate = function () {
        $scope.isDownloadTemplate = true;
        $scope.isPackageInformation = false;
    };

    //packageInformation collapse code
    $scope.packageInformation = function () {
        $scope.isPackageInformation = true;
        $scope.isDownloadTemplate = false;
    };

    var fillNotifyPartyCountry = function (Country) {
        if ($scope.tradelaneBooking && $scope.tradelaneBooking.IsNotifyPartySameAsReceiver) {
            $scope.tradelaneBooking.NotifyParty.Country = Country;
        }
    };

    var fillNotifyPartyPostCode = function (PostCode) {
        if ($scope.tradelaneBooking && $scope.tradelaneBooking.IsNotifyPartySameAsReceiver) {
            $scope.tradelaneBooking.NotifyParty.PostCode = PostCode;
        }
    };

    var fillNotifyPartyCompanyName = function (CompanyName) {
        if ($scope.tradelaneBooking && $scope.tradelaneBooking.IsNotifyPartySameAsReceiver) {
            $scope.tradelaneBooking.NotifyParty.CompanyName = CompanyName;
        }
    };

    var fillNotifyPartyContactFirstName = function (ContactFirstName) {
        if ($scope.tradelaneBooking && $scope.tradelaneBooking.IsNotifyPartySameAsReceiver) {
            $scope.tradelaneBooking.NotifyParty.FirstName = ContactFirstName;
        }
    };

    var fillNotifyPartyContactLastName = function (ContactLastName) {
        if ($scope.tradelaneBooking && $scope.tradelaneBooking.IsNotifyPartySameAsReceiver) {
            $scope.tradelaneBooking.NotifyParty.LastName = ContactLastName;
        }
    };

    var fillNotifyPartyAddress1 = function (Address1) {
        if ($scope.tradelaneBooking && $scope.tradelaneBooking.IsNotifyPartySameAsReceiver) {
            $scope.tradelaneBooking.NotifyParty.Address = Address1;
        }
    };

    var fillNotifyPartyAddress2 = function (Address2) {
        if ($scope.tradelaneBooking && $scope.tradelaneBooking.IsNotifyPartySameAsReceiver) {
            $scope.tradelaneBooking.NotifyParty.Address2 = Address2;
        }
    };

    var fillNotifyPartyCity = function (City) {
        if ($scope.tradelaneBooking && $scope.tradelaneBooking.IsNotifyPartySameAsReceiver) {
            $scope.tradelaneBooking.NotifyParty.City = City;
        }
    };

    var fillNotifyPartyState = function (State) {
        if ($scope.tradelaneBooking && $scope.tradelaneBooking.IsNotifyPartySameAsReceiver) {
            $scope.tradelaneBooking.NotifyParty.State = State;
        }
    };

    var fillNotifyPartyArea = function (Area) {
        if ($scope.tradelaneBooking && $scope.tradelaneBooking.IsNotifyPartySameAsReceiver) {
            $scope.tradelaneBooking.NotifyParty.Area = Area;
        }
    };

    var fillNotifyPartyAdditionalNote = function (AdditionalNote) {
        if ($scope.tradelaneBooking && $scope.tradelaneBooking.IsNotifyPartySameAsReceiver) {
            $scope.tradelaneBooking.NotifyPartyAdditionalNote = AdditionalNote;
        }
    };

    var fillNotifyPartyTelephoneNumber = function (Phone) {
        if ($scope.tradelaneBooking && $scope.tradelaneBooking.IsNotifyPartySameAsReceiver) {
            $scope.tradelaneBooking.NotifyParty.Phone = Phone;
        }
    };

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

    //Booking Form tab  Validation  
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
            TradelaneBookingForm.destinationAirport && TradelaneBookingForm.destinationAirport.$valid &&
            TradelaneBookingForm.shipToMail && TradelaneBookingForm.shipToMail.$valid
        ) {
            flag = true;
        }
        else {
            flag = false;
        }

        return flag;
    };

    $scope.notifyPartyValidation = function (TradelaneBookingForm) {
        if ($scope.tradelaneBooking && $scope.tradelaneBooking.IsNotifyPartySameAsReceiver) {
            return true;
        }
        if (TradelaneBookingForm.notifyAddress1 && TradelaneBookingForm.notifyAddress1.$valid &&
            TradelaneBookingForm.notifyCompanyName && TradelaneBookingForm.notifyCompanyName.$valid &&
            TradelaneBookingForm.notifyFirstName && TradelaneBookingForm.notifyFirstName.$valid &&
            TradelaneBookingForm.notifyLastName && TradelaneBookingForm.notifyLastName.$valid &&
            TradelaneBookingForm.notifyCountry && TradelaneBookingForm.notifyCountry.$valid &&
            TradelaneBookingForm.notifyPostcode && TradelaneBookingForm.notifyPostcode.$valid &&
            TradelaneBookingForm.notifyState && TradelaneBookingForm.notifyState.$valid &&
            TradelaneBookingForm.notifyCityName && TradelaneBookingForm.notifyCityName.$valid &&
            TradelaneBookingForm.notifyPhone && TradelaneBookingForm.notifyPhone.$valid &&
            TradelaneBookingForm.notifyMail && TradelaneBookingForm.notifyMail.$valid
        ) {
            flag = true;
        }
        else {
            flag = false;
        }
        return flag;
    };

    $scope.serviceOptionValidation = function (TradelaneBookingForm) {

        if (TradelaneBookingForm.shipmentHandler && TradelaneBookingForm.shipmentHandler.$valid &&
            TradelaneBookingForm.airlinePreference && TradelaneBookingForm.airlinePreference.$valid &&
            TradelaneBookingForm.reference1 && TradelaneBookingForm.reference1.$valid
        ) {
            flag = true;
        }
        else {
            flag = false;
        }
        return flag;
    };

    $scope.shipmentDetailValidation = function (TradelaneBookingForm) {
        if ($scope.tradelaneBooking && TradelaneBookingForm.msdsBattery && TradelaneBookingForm.msdsBattery.$valid &&
            TradelaneBookingForm.unBattery && TradelaneBookingForm.unBattery.$valid &&
            TradelaneBookingForm.batteryDeclarationForm && TradelaneBookingForm.batteryDeclarationForm.$valid &&
            TradelaneBookingForm.paymentPartyAccountNo && TradelaneBookingForm.paymentPartyAccountNo.$valid &&
            TradelaneBookingForm.declaredCurrency && TradelaneBookingForm.declaredCurrency.$valid &&
            TradelaneBookingForm.declaredValue && TradelaneBookingForm.declaredValue.$valid && parseFloat($scope.tradelaneBooking.DeclaredValue) &&
            TradelaneBookingForm.shipmentDescription && TradelaneBookingForm.shipmentDescription.$valid &&
            $scope.tradelaneBooking && $scope.tradelaneBooking.DangerousGoods &&
            $scope.hawbGroupedPackages && $scope.hawbGroupedPackages.length
        ) {
            flag = true;
        }
        else {
            flag = false;
        }
        return flag;
    };

    $scope.toggleNotifyParty = function () {
        if ($scope.tradelaneBooking.IsNotifyPartySameAsReceiver) {
            $scope.tradelaneBooking.NotifyParty = {
                TradelaneShipmentAddressId: 0,
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
        }
        else {
            //   $scope.tradelaneBooking.NotifyParty = $scope.tradelaneBooking.ShipTo;
            $scope.tradelaneBooking.NotifyPartyAdditionalNote = $scope.tradelaneBooking.ReceiverAdditionalNote;
            $scope.NotifyPartyPhoneCode = $scope.ShipToPhoneCode;
        }
    };

    //Set Multilingual for Modal Popup
    var setMultilingualOtions = function () {
        $translate(['FrayteError', 'FrayteWarning', 'FrayteInformation', 'FrayteValidation', 'FrayteSuccess', 'CustomInfoCheckTerms',
            'PleaseSelectionDeliveryOption', 'PleaseSelectTermAndConditions', 'PleaseCorrectValidationErrors',
            'ErrorSavingShipment', 'PleaseSelectValidFile', 'UploadedSuccessfully', 'ErrorOccuredDuringUpload',
            'ErrorGettingShipmentDetailServer', 'SaveDraft_Validation', 'PackageShipment_Validation',
            'SelectShipment_Validation', 'CustomInformation_Validation', 'BookingSave_Validation',
            'ServiceSideError_Validation', 'Success', 'RemovePackage_Validation', 'Validation_Error', 'GetService_Validation',
            'SelectCustomer_Validation', 'SelectCurrency_Validation', 'FrayteWarning_Validation', 'SelectCustomerAddressBook_Validation',
            'FrayteServiceError_Validation', 'ReceiveDetail_Validation', 'InitialData_Validation', 'DirectBooking_Error_Message', 'PlacingBooking',
            'SomeProblemOccured', 'ErrorUploadingForm', 'SuccessfullyUploadedForm', 'SomeProblemOccurredTryAgainLater', 'PleaseCorrectValidationError',
            'BookingPlacedSucessfully', 'ProblemPlaceBookingTryAgain', 'SetCustomerAccountNoFirst', 'NoAssignedHAWBSureCancel', 'Confirmation', 'PleaseUploadExcelPlaceBooking',
            'PleaseAssignHAWBPackagesFirst', 'SomeErrorOccuredTryAgain', 'Set_ship_from_and_ship_to_country_first', 'Set_ship_from_or_shiptocountry_and_customer_first',
            'Loading_Postcode_Addresses', 'Saving_Shipment', 'NotRequiredFoundCheckUploadDownloadTemplateExcelColumns', 'ErrorUploadingDocumentTryAgain',
            'AcceptHazardousDangerousGoods', 'DocumentAleadyUploadedFor', 'CountryValidationError']).then(function (translations) {
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
                $scope.TextValidation = translations.PleaseCorrectValidationErrors;
                $scope.ValidationError = translations.Validation_Error;
                $scope.GetServiceValidation = translations.GetService_Validation;
                $scope.SelectCustomerValidation = translations.SelectCustomer_Validation;
                $scope.SelectCurrencyValidation = translations.SelectCurrency_Validation;
                $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
                $scope.SelectCustomerAddressBookValidation = translations.SelectCustomerAddressBook_Validation;
                $scope.ReceiveDetailValidation = translations.FrayteServiceError_Validation;
                $scope.FrayteServiceErrorValidation = translations.ReceiveDetail_Validation;
                $scope.InitialDataValidation = translations.InitialData_Validation;
                $scope.PlacingBooking = translations.PlacingBooking;
                $scope.SomeProblemOccured = translations.SomeProblemOccured;
                $scope.ErrorUploadingForm = translations.ErrorUploadingForm;
                $scope.SuccessfullyUploadedForm = translations.SuccessfullyUploadedForm;
                $scope.SomeProblemOccurredTryAgainLater = translations.SomeProblemOccurredTryAgainLater;
                $scope.PleaseCorrectValidationError = translations.PleaseCorrectValidationError;
                $scope.BookingPlacedSucessfully = translations.BookingPlacedSucessfully;
                $scope.ProblemPlaceBookingTryAgain = translations.ProblemPlaceBookingTryAgain;
                $scope.SetCustomerAccountNoFirst = translations.SetCustomerAccountNoFirst;
                $scope.NoAssignedHAWBSureCancel = translations.NoAssignedHAWBSureCancel;
                $scope.Confirmation = translations.Confirmation;
                $scope.PleaseUploadExcelPlaceBooking = translations.PleaseUploadExcelPlaceBooking;
                $scope.PleaseAssignHAWBPackagesFirst = translations.PleaseAssignHAWBPackagesFirst;
                $scope.SomeErrorOccuredTryAgain = translations.SomeErrorOccuredTryAgain;
                $scope.Set_ship_from_and_ship_to_country_first = translations.Set_ship_from_and_ship_to_country_first;
                $scope.Set_ship_from_or_shiptocountry_and_customer_first = translations.Set_ship_from_or_shiptocountry_and_customer_first;
                $scope.Loading_Postcode_Addresses = translations.Loading_Postcode_Addresses;
                $scope.Saving_Shipment = translations.Saving_Shipment;
                $scope.ErrorUploadingDocumentTryAgain = translations.ErrorUploadingDocumentTryAgain;
                $scope.NotRequiredFoundCheckUploadDownloadTemplateExcelColumns = translations.NotRequiredFoundCheckUploadDownloadTemplateExcelColumns;
                $scope.AcceptHazardousDangerousGoods = translations.AcceptHazardousDangerousGoods;
                $scope.DocumentAleadyUploadedFor = translations.DocumentAleadyUploadedFor;
                $scope.CountryValidationError = translations.CountryValidationError;
            });
    };

    //AddressBook 
    $scope.addressBook = function (UserType) {

        if (!$scope.customerId) {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
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
                    if ($scope.tradelaneBooking.ShipTo && $scope.tradelaneBooking.ShipTo.Country && $scope.tradelaneBooking.ShipTo.Country.CountryId) {
                        return $scope.tradelaneBooking.ShipTo.Country.CountryId;
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
                    $scope.tradelaneBooking.ShipFrom = addressBooks;
                    if (addressBooks.State) {
                        $scope.tradelaneBooking.ShipFrom.State = addressBooks.State;
                    }

                    $scope.tradelaneBooking.ShipFrom.Country = addressBooks.Country;
                    $scope.tradelaneBooking.ShipFrom.TradelaneShipmentAddressId = 0;
                    $scope.SetShipinfo($scope.tradelaneBooking.ShipFrom.Country, UserType);
                }
                else if (UserType === 'Receiver') {
                    $scope.tradelaneBooking.ShipTo = addressBooks;
                    if (addressBooks.State) {
                        $scope.tradelaneBooking.ShipTo.State = addressBooks.State;
                    }

                    $scope.tradelaneBooking.ShipTo.Country = addressBooks.Country;
                    $scope.tradelaneBooking.ShipTo.TradelaneShipmentAddressId = 0;
                    $scope.SetShipinfo($scope.tradelaneBooking.ShipTo.Country, UserType);
                }
                // set form in dirty state for progress bar
                if ($scope.TradelaneBookingForm) {
                    $scope.TradelaneBookingForm.$dirty = true;
                }
            }
        });
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

            getAirports(Country, Action);
        }
    };

    var getAirports = function (Country, Action) {
        if (Country && Action) {
            TradelaneBookingService.AirPorts(Country.CountryId).then(function (response) {
                if (response.data) {
                    if (Action === "Shipper") {
                        if (response.data.length === 1) {
                            $scope.DepartureAirportCodes = response.data;
                            $scope.tradelaneBooking.DepartureAirport = $scope.DepartureAirportCodes[0];
                        }
                        else {
                            if ($scope.FromNewAirport !== undefined && $scope.FromNewAirport !== null && $scope.FromNewAirport !== '') {
                                $scope.DepartureAirportCodes = response.data;
                                angular.forEach($scope.DepartureAirportCodes, function (item, key) {
                                    if ($scope.FromNewAirport.AirportCode === item.AirportCode) {
                                        $scope.tradelaneBooking.DepartureAirport = item;
                                    }
                                });
                            }
                            else {
                                $scope.DepartureAirportCodes = response.data;
                            }
                        }
                    }
                    else if (Action === "Receiver") {
                        if (response.data.length === 1) {
                            $scope.DestinationAirportCodes = response.data;
                            $scope.tradelaneBooking.DestinationAirport = $scope.DestinationAirportCodes[0];
                        }
                        else {
                            if ($scope.ToNewAirport !== undefined && $scope.ToNewAirport !== null && $scope.ToNewAirport !== '') {
                                $scope.DestinationAirportCodes = response.data;
                                angular.forEach($scope.DestinationAirportCodes, function (item, key) {
                                    if ($scope.ToNewAirport.AirportCode === item.AirportCode) {
                                        $scope.tradelaneBooking.DestinationAirport = item;
                                    }
                                });
                            }
                            else {
                                $scope.DestinationAirportCodes = response.data;
                            }
                        }
                    }
                }
            }, function () {

            });
        }
    };

    $scope.disabled = false;

    // Post code search 
    $scope.GetPostCodeAddress = function (PostCode, CountryCode2, Type) {
        if (PostCode && PostCode.length > 5 && ((Type === 'Shipper' && PostCode.length > $scope.tradelaneBooking.ShipFrom.PostCode.length) || (Type === 'Receiver' && PostCode.length > $scope.tradelaneBooking.ShipTo.PostCode.length) || (Type === 'notify' && PostCode.length > $scope.tradelaneBooking.NotifyParty.PostCode.length)) && CountryCode2 && CountryCode2 === "GB") {
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
                $scope.tradelaneBooking.ShipFrom.PostCode = $item.PostCode;
                $scope.tradelaneBooking.ShipFrom.Address = $item.Address1;
                $scope.tradelaneBooking.ShipFrom.Address2 = $item.Address2;
                $scope.tradelaneBooking.ShipFrom.Area = $item.Area;
                $scope.tradelaneBooking.ShipFrom.City = $item.City;
                $scope.tradelaneBooking.ShipFrom.CompanyName = $item.CompanyName;
            }
            else if (Type === 'Receiver') {
                $scope.tradelaneBooking.ShipTo.PostCode = $item.PostCode;
                $scope.tradelaneBooking.ShipTo.Address = $item.Address1;
                $scope.tradelaneBooking.ShipTo.Address2 = $item.Address2;
                $scope.tradelaneBooking.ShipTo.Area = $item.Area;
                $scope.tradelaneBooking.ShipTo.City = $item.City;
                $scope.tradelaneBooking.ShipTo.CompanyName = $item.CompanyName;
            }
            else if (Type === 'notify') {
                $scope.tradelaneBooking.NotifyParty.PostCode = $item.PostCode;
                $scope.tradelaneBooking.NotifyParty.Address = $item.Address1;
                $scope.tradelaneBooking.NotifyParty.Address2 = $item.Address2;
                $scope.tradelaneBooking.NotifyParty.Area = $item.Area;
                $scope.tradelaneBooking.NotifyParty.City = $item.City;
                $scope.tradelaneBooking.NotifyParty.CompanyName = $item.CompanyName;
            }
        }
    };

    $scope.SetPostCodeAddressValue = function (Type) {
        if ($scope.fillPostlValues && $scope.fillPostlValues.length) {
            if (Type === 'Shipper') {
                $scope.tradelaneBooking.ShipFrom.City = $scope.fillPostlValues[0].City;
            }
            else if (Type === "Receiver") {
                $scope.tradelaneBooking.ShipTo.City = $scope.fillPostlValues[0].City;
            }
            else if (Type === "notify") {
                $scope.tradelaneBooking.NotifyParty.City = $scope.fillPostlValues[0].City;
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
                $scope.tradelaneBooking.ShipFrom.PostCode = null;
                $scope.tradelaneBooking.ShipFrom.State = null;
            }
            else if (Type === 'Receiver') {
                $scope.tradelaneBooking.ShipTo.PostCode = null;
                $scope.tradelaneBooking.ShipTo.State = null;
            }
            else if (Type === 'notify') {
                $scope.tradelaneBooking.NotifyParty.PostCode = null;
                $scope.tradelaneBooking.NotifyParty.State = null;
            }
        }
        else if (Country.Code === 'GBR') {
            if (Type === 'Shipper') {
                $scope.tradelaneBooking.ShipFrom.State = null;
            }
            else if (Type === 'Receiver') {
                $scope.tradelaneBooking.ShipTo.State = null;
            }
            else if (Type === 'notify') {
                $scope.tradelaneBooking.NotifyParty.State = null;
            }
        }
    };

    // Validate Customer Account No
    $scope.isValidAccountNumber = true;

    $scope.CheckAccountNumber = function (accountNumber) {
        $scope.isValidAccountNumber = true;
        if (accountNumber !== undefined && accountNumber.length === 9) {

            CustomerService.GetCustomerDetailByAccountNumber(accountNumber).then(function (response) {
                if (response.status === 200) {
                    if (response.data && response.data.CustomerId) {
                        $scope.customerId = response.data.CustomerId;
                        $scope.tradelaneBooking.CustomerId = response.data.CustomerId;

                        $scope.isValidAccountNumber = true;
                    }
                    else {
                        $scope.isValidAccountNumber = false;
                    }
                }
                else {
                    $scope.isValidAccountNumber = false;
                }

            },
                function errorCallback(response) {
                    $scope.isValidAccountNumber = false;
                });
        }
    };

    // New Booking Json 
    var newBooking = function () {
        $scope.tradelaneBooking = {
            TradelaneShipmentId: 0,
            OpearionZoneId: 0,
            CustomerId: $scope.customerId,
            BatteryDeclarationType: 'None',
            CustomerAccountNumber: null,
            ShipmentStatusId: $scope.ShipmentStatus.Draft,
            CreatedBy: $scope.CreatedBy,
            CreatedOnUtc: new Date(),
            ShipFrom: {
                TradelaneShipmentAddressId: 0,
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
            ShipperAdditionalNote: '',
            ShipTo: {
                TradelaneShipmentAddressId: 0,
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
            ReceiverAdditionalNote: '',
            NotifyParty: {
                TradelaneShipmentAddressId: 0,
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
            },
            NotifyPartyAdditionalNote: '',
            IsNotifyPartySameAsReceiver: true,
            PayTaxAndDuties: "Receiver",
            TaxAndDutyAccountNumber: '',
            CustomsSigner: '',
            TaxAndDutyAcceptedBy: '',
            Currency: null,
            Packages: [{
                TradelaneShipmentDetailId: 0,
                TradelaneShipmentId: 0,
                Length: null,
                Width: null,
                Height: null,
                Weight: null,
                CartonNumber: null,
                HAWB: null
            }],
            ShipmentHandlerMethod: null,
            PakageCalculatonType: 'kgToCms',
            UpdatedBy: 0,
            UpdatedOnUtc: new Date(),
            FrayteNumber: '',
            LogisticType: '',
            DepartureAirportCode: '',
            DestinationAirportCode: '',
            ShipmentReference: '',
            ShipmentDescription: '',
            DeclaredValue: null,
            DeclaredCurrency: null,
            AirlinePreference: null,
            TotalEstimatedWeight: null,
            DeclaredCustomValue: null,
            InsuranceAmount: null,
            CertificateOfOrigin: '',
            ExportLicenceNo: '',
            DangerousGoods: true,
            MAWB: '',
            MAWBAgentId: '',
            ManifestName: '',
            AdditionalInfo: '',
            Incoterm: null
        };
    };

    $scope.ClearForm = function (form) {
        form.$setPristine();
        form.$setUntouched();
        newBooking();
        window.scrollTo(0, 0);
        //$scope.setScroll('top');
        //$anchorScroll.yOffset = 700;
    };

    // Place Booking 
    $scope.PlaceBooking = function (tradelaneBooking, IsValid) {
        if (IsValid) {
            if (!parseFloat($scope.tradelaneBooking.DeclaredValue)) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteValidation,
                    body: $scope.TextValidation,
                    showCloseButton: true
                });
                return;
            }

            if (!$scope.Booking || !$scope.Booking.TotalShipments) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.PleaseUploadExcelPlaceBooking,
                    showCloseButton: true
                });

                window.scrollTo(0, 500);

                return;
            }

            if (!$scope.tradelaneBooking.DangerousGoods) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.AcceptHazardousDangerousGoods,
                    showCloseButton: true
                });
                return;
            }

            if ($scope.Booking.TotalShipments && !$scope.Booking.AllocatedShipments) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.PleaseAssignHAWBPackagesFirst,
                    showCloseButton: true
                });
                return;
            }

            if ($scope.Booking.UnAllocatedShipments) {


                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'tradelaneBooking/hawbConfirmation.tpl.html',
                    controller: 'HAWBConfirmationController',
                    windowClass: 'directBookingDetail',
                    size: 'md',
                    backdrop: 'static',
                    keyboard: false,
                    resolve: {
                        TotalShipments: function () {
                            return $scope.Booking.TotalShipments;
                        },
                        AllocatedShipments: function () {
                            return $scope.Booking.AllocatedShipments;
                        },
                        Type: function () {
                            return "Booking";
                        }
                    }
                });
                modalInstance.result.then(function (response) {
                    updateBooking(tradelaneBooking);
                }, function () {
                });
            }
            else {
                updateBooking(tradelaneBooking);
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

    var updateBooking = function (tradelaneBooking) {

        AppSpinner.showSpinnerTemplate($scope.PlacingBooking, $scope.Template);

        if ($scope.userInfo.RoleId !== 3) {
            $scope.tradelaneBooking.ShipmentStatusId = $scope.ShipmentStatus.Pending;
        }
        else {
            $scope.tradelaneBooking.ShipmentStatusId = $scope.ShipmentStatus.ShipmentBooked;
        }

        TradelaneBookingService.PlaceBooking($scope.tradelaneBooking).then(function (response) {
            $timeout(function () {
                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteSuccess,
                    body: $scope.BookingSaveValidation,
                    showCloseButton: true
                });
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'tradelaneBooking/tradelaneBookingDetail/tradelaneBookingDetail.tpl.html',
                    controller: 'TradelaneBookingDetailController',
                    windowClass: 'directBookingDetail',
                    size: 'lg',
                    backdrop: 'static',
                    keyboard: false,
                    resolve: {
                        ShipmentId: function () {
                            return $scope.tradelaneBooking.TradelaneShipmentId;
                        },
                        ModuleType: function () {
                            return '';
                        }
                    }
                });
                modalInstance.result.then(function (response) {
                    $state.go("loginView.userTabs.tradelane-shipments");
                }, function () {
                });
            }, 2000);
            AppSpinner.hideSpinnerTemplate();
        }, function (error) {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.ProblemPlaceBookingTryAgain,
                showCloseButton: true
            });
            AppSpinner.hideSpinnerTemplate();
        });
    };

    $scope.AddFromAirport = function (Country) {
        if (Country !== undefined && Country !== null && Country !== '' && Country.CountryId > 0) {
            modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'tradelaneBooking/tradelaneBookingAirportAddEdit.tpl.html',
                controller: 'TradelaneBookingAirportAddEditController',
                windowClass: '',
                size: 'md',
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    CountryId: function () {
                        return Country.CountryId;
                    }
                }
            });

            modalInstance.result.then(function (response) {
                if (response !== undefined && response !== null && response !== '' && response.Status === true) {
                    $scope.FromNewAirport = response.Airport;
                    getAirports(Country, 'Shipper');
                }
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.CountryValidationError,
                showCloseButton: true
            });
        }
    };

    $scope.AddToAirport = function (Country) {
        if (Country !== undefined && Country !== null && Country !== '' && Country.CountryId > 0) {
            modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'tradelaneBooking/tradelaneBookingAirportAddEdit.tpl.html',
                controller: 'TradelaneBookingAirportAddEditController',
                windowClass: '',
                size: 'md',
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    CountryId: function () {
                        return Country.CountryId;
                    }
                }
            });
            modalInstance.result.then(function (response) {
                if (response !== undefined && response !== null && response !== '' && response.Status === true) {
                    $scope.ToNewAirport = response.Airport;
                    getAirports(Country, 'Receiver');
                }
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.CountryValidationError,
                showCloseButton: true
            });
        }
    };

    $scope.shipFromToggleState = function (Country) {
        return TradelaneBookingService.toggleState(Country);
    };

    $scope.shipToToggleState = function (Country) {
        return TradelaneBookingService.toggleState(Country);
    };

    $scope.notifyPartyToggleState = function (Country) {
        return TradelaneBookingService.toggleState(Country);
    };

    $scope.checkValidation = function (TradelaneBookingForm) {
        if ((TradelaneBookingForm.shipFromCountry && !TradelaneBookingForm.shipFromCountry.$valid) ||
            (TradelaneBookingForm.shipToCountry && !TradelaneBookingForm.shipToCountry.$valid) ||
            ((TradelaneBookingForm.accountNo && TradelaneBookingForm.accountNo.$invalid) || !$scope.isValidAccountNumber)
        ) {
            return true;
        }
        else {
            return false;
        }
    };

    $scope.saveAsDraft = function (TradelaneBookingForm) {
        if ((TradelaneBookingForm.shipFromCountry && !TradelaneBookingForm.shipFromCountry.$valid) ||
            (TradelaneBookingForm.shipToCountry && !TradelaneBookingForm.shipToCountry.$valid) ||
            ((TradelaneBookingForm.accountNo && TradelaneBookingForm.accountNo.$invalid) || !$scope.isValidAccountNumber)
        ) {
            return;
        }
        else {
            $scope.saveBooking(TradelaneBookingForm);
        }
    };

    $scope.saveBooking = function (TradelaneBookingForm) {
        if ((TradelaneBookingForm.shipFromCountry && !TradelaneBookingForm.shipFromCountry.$valid) ||
            (TradelaneBookingForm.shipToCountry && !TradelaneBookingForm.shipToCountry.$valid) ||
            ((TradelaneBookingForm.accountNo && TradelaneBookingForm.accountNo.$invalid) || !$scope.isValidAccountNumber)
        ) {
            if ($scope.userInfo.RoleId !== 3) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.Set_ship_from_or_shiptocountry_and_customer_first,
                    showCloseButton: true
                });
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.Set_ship_from_and_ship_to_country_first,
                    showCloseButton: true
                });
            }
            return;
        }
        else {
            if ($scope.tradelaneBooking.TradelaneShipmentId) {
                if ($scope.Status === "Draft") {

                    AppSpinner.showSpinnerTemplate($scope.Saving_Shipment, $scope.Template);

                    TradelaneBookingService.PlaceBooking($scope.tradelaneBooking).then(function (response) {
                        if (response.data) {
                            $scope.tradelaneBooking.TradelaneShipmentId = response.data.TradelaneShipmentId;
                            $scope.tradelaneBooking.FrayteNumber = response.data.FrayteNumber;
                            $scope.tradelaneBooking.ShipFrom.TradelaneShipmentAddressId = response.data.ShipFrom.TradelaneShipmentAddressId;
                            $scope.tradelaneBooking.ShipTo.TradelaneShipmentAddressId = response.data.ShipTo.TradelaneShipmentAddressId;

                            if (response.data.NotifyParty.TradelaneShipmentAddressId) {
                                $scope.tradelaneBooking.NotifyParty.TradelaneShipmentAddressId = response.data.NotifyParty.TradelaneShipmentAddressId;
                            }
                            AppSpinner.hideSpinnerTemplate();
                            if ($scope.Status === "Draft") {
                                $timeout(function () {
                                    //$scope.DirectShipmentDetail(response.data.DirectShipmentId);
                                    toaster.pop({
                                        type: 'success',
                                        title: $scope.TitleFrayteSuccess,
                                        body: $scope.BookingSaveValidation,
                                        showCloseButton: true
                                    });
                                    var modalInstance = $uibModal.open({
                                        animation: true,
                                        templateUrl: 'tradelaneBooking/tradelaneBookingDetail/tradelaneBookingDetail.tpl.html',
                                        controller: 'TradelaneBookingDetailController',
                                        windowClass: 'directBookingDetail',
                                        size: 'lg',
                                        backdrop: 'static',
                                        keyboard: false,
                                        resolve: {
                                            ShipmentId: function () {
                                                return $scope.tradelaneBooking.TradelaneShipmentId;
                                            }
                                        }
                                    });
                                    modalInstance.result.then(function (response) {
                                        $state.go("loginView.userTabs.tradelane-shipments");
                                    }, function () {
                                        //     $state.go("loginView.userTabs.tradelane-shipments");
                                    });
                                }, 2000);
                            }
                        }
                    }, function (error) {
                        AppSpinner.hideSpinnerTemplate();
                        toaster.pop({
                            type: 'error',
                            title: $scope.TitleFrayteError,
                            body: $scope.SomeProblemOccurredTryAgainLater,
                            showCloseButton: true
                        });
                    });
                }
            }
            else {
                $scope.tradelaneBooking.ShipmentStatusId = $scope.ShipmentStatus.Draft;

                if ($scope.Status === "Draft") {
                    AppSpinner.showSpinnerTemplate($scope.Saving_Shipment, $scope.Template);
                }
                TradelaneBookingService.PlaceBooking($scope.tradelaneBooking).then(function (response) {
                    if (response.data) {

                        AppSpinner.hideSpinnerTemplate();

                        $scope.tradelaneBooking.TradelaneShipmentId = response.data.TradelaneShipmentId;
                        $scope.tradelaneBooking.FrayteNumber = response.data.FrayteNumber;
                        $scope.tradelaneBooking.ShipFrom.TradelaneShipmentAddressId = response.data.ShipFrom.TradelaneShipmentAddressId;
                        $scope.tradelaneBooking.ShipTo.TradelaneShipmentAddressId = response.data.ShipTo.TradelaneShipmentAddressId;

                        if (response.data.NotifyParty.TradelaneShipmentAddressId) {
                            $scope.tradelaneBooking.NotifyParty.TradelaneShipmentAddressId = response.data.NotifyParty.TradelaneShipmentAddressId;
                        }

                        if ($scope.Status === "Draft") {
                            $timeout(function () {
                                //$scope.DirectShipmentDetail(response.data.DirectShipmentId);
                                toaster.pop({
                                    type: 'success',
                                    title: $scope.TitleFrayteSuccess,
                                    body: $scope.BookingSaveValidation,
                                    showCloseButton: true
                                });
                                var modalInstance = $uibModal.open({
                                    animation: true,
                                    templateUrl: 'tradelaneBooking/tradelaneBookingDetail/tradelaneBookingDetail.tpl.html',
                                    controller: 'TradelaneBookingDetailController',
                                    windowClass: 'directBookingDetail',
                                    size: 'lg',
                                    backdrop: 'static',
                                    keyboard: false,
                                    resolve: {
                                        ShipmentId: function () {
                                            return $scope.tradelaneBooking.TradelaneShipmentId;
                                        }
                                    }
                                });
                                modalInstance.result.then(function (response) {
                                    $state.go("loginView.userTabs.tradelane-shipments");
                                }, function () {
                                    //  $state.go("loginView.userTabs.tradelane-shipments");
                                });
                            }, 2000);
                        }


                    }
                }, function (error) {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.SomeProblemOccurredTryAgainLater,
                        showCloseButton: true
                    });
                });

            }

        }
    };

    var uploadExcel = function () {
        $scope.WhileAddingExcel();
    };

    //Upload BatteryFrom 
    $scope.WhileAddingMsBatteryForm = function ($files, $file, $event) {
        if (!$file) {
            return;
        }
        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.TextPleaseSelectValidFile,
                showCloseButton: true
            });
            return;
        }

        $scope.msdsBatteryFileName = $file.name;

        // Upload the excel file here.
        $scope.uploadMSDS = Upload.upload({
            url: config.SERVICE_URL + '/TradelaneBooking/UploadBatteryForms',
            file: $file,
            fields: {
                ShipmentId: $scope.tradelaneBooking.TradelaneShipmentId,
                UserId: $scope.tradelaneBooking.CreatedBy
            }
        });

        $scope.uploadMSDS.progress($scope.progressuploadMSDS);

        $scope.uploadMSDS.success($scope.successuploadMSDS);

        $scope.uploadMSDS.error($scope.erroruploadMSDS);
    };

    $scope.progressuploadMSDS = function (evt) {
        //To Do:  show excel uploading progress message 
    };

    $scope.successuploadMSDS = function (data, status, headers, config) {
        if (status = 200) {

            if (data.Status) {
                $scope.MSDS = $scope.msdsBatteryFileName;
                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteSuccess,
                    body: $scope.SuccessfullyUploadedForm,
                    showCloseButton: true
                });
            }
            else {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.ErrorUploadingDocumentTryAgain,
                    showCloseButton: true
                });
            }
        }
        else {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteWarning,
                body: $scope.ErrorUploadingDocumentTryAgain,
                showCloseButton: true
            });
        }
    };

    $scope.erroruploadMSDS = function (err) {
        if (err && err.Message === "BatteryForm") {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.DocumentAleadyUploadedFor + err.Message,
                showCloseButton: true
            });
        }
        else {

            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteWarning,
                body: $scope.ErrorUploadingDocumentTryAgain,
                showCloseButton: true
            });
        }
    };

    //Upload BatteryFrom  UN38
    $scope.WhileAddingUN38BatteryForm = function ($files, $file, $event) {
        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.TextPleaseSelectValidFile,
                showCloseButton: true
            });
            return;
        }
        $scope.un38BatteryFileName = $file.name;
        // Upload the excel file here.
        $scope.uploadUN38 = Upload.upload({
            url: config.SERVICE_URL + '/TradelaneBooking/UploadBatteryForms',
            file: $file,
            fields: {
                ShipmentId: $scope.tradelaneBooking.TradelaneShipmentId,
                UserId: $scope.tradelaneBooking.CreatedBy
            }
        });

        $scope.uploadUN38.progress($scope.progressuploadUN38);

        $scope.uploadUN38.success($scope.successuploadUN38);

        $scope.uploadUN38.error($scope.erroruploadUN38);
    };

    $scope.progressuploadUN38 = function (evt) {
        //To Do:  show excel uploading progress message 
    };

    $scope.successuploadUN38 = function (data, status, headers, config) {
        if (status = 200) {

            if (data.Status) {
                $scope.UN38 = $scope.un38BatteryFileName;
                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteSuccess,
                    body: $scope.SuccessfullyUploadedForm,
                    showCloseButton: true
                });
            }
            else {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.ErrorUploadingDocumentTryAgain,
                    showCloseButton: true
                });
            }
        }
        else {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteWarning,
                body: $scope.ErrorUploadingDocumentTryAgain,
                showCloseButton: true
            });

        }
    };

    $scope.erroruploadUN38 = function (err) {
        if (err && err.Message === "BatteryForm") {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.DocumentAleadyUploadedFor + err.Message,
                showCloseButton: true
            });
        }
        else {

            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteWarning,
                body: $scope.ErrorUploadingDocumentTryAgain,
                showCloseButton: true
            });
        }
    };

    //Upload BatteryFrom  UN38
    $scope.WhileAddingBatteryDeclarationForm = function ($files, $file, $event) {
        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.TextPleaseSelectValidFile,
                showCloseButton: true
            });
            return;
        }
        $scope.batteryDeclarationFormFileNBame = $file.name;
        // Upload the excel file here.
        $scope.uploadBatteryDeclarationForm = Upload.upload({
            url: config.SERVICE_URL + '/TradelaneBooking/UploadBatteryForms',
            file: $file,
            fields: {
                ShipmentId: $scope.tradelaneBooking.TradelaneShipmentId,
                UserId: $scope.tradelaneBooking.CreatedBy
            }
        });

        $scope.uploadUN38.progress($scope.progressuploadBatteryDeclarationForm);

        $scope.uploadUN38.success($scope.successuploadBatteryDeclarationForm);

        $scope.uploadUN38.error($scope.erroruploadBatteryDeclarationForm);
    };

    $scope.progressuploadBatteryDeclarationForm = function (evt) {
        //To Do:  show excel uploading progress message 
    };

    $scope.successuploadBatteryDeclarationForm = function (data, status, headers, config) {
        if (status = 200) {

            if (data.Status) {
                $scope.BatteryDeclaration = $scope.batteryDeclarationFormFileNBame;
                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteSuccess,
                    body: $scope.SuccessfullyUploadedForm,
                    showCloseButton: true
                });
            }
            else {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.ErrorUploadingDocumentTryAgain,
                    showCloseButton: true
                });
            }
        }
        else {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteWarning,
                body: $scope.ErrorUploadingDocumentTryAgain,
                showCloseButton: true
            });
        }
    };

    $scope.erroruploadBatteryDeclarationForm = function (err) {
        if (err && err.Message === "BatteryForm") {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.DocumentAleadyUploadedFor + err.Message,
                showCloseButton: true
            });
        }
        else {

            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteWarning,
                body: $scope.ErrorUploadingDocumentTryAgain,
                showCloseButton: true
            });
        }
    };

    //Uploads and Download excel for Pieces grid
    $scope.WhileAddingExcel = function ($files, $file, $event) {
        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.TextPleaseSelectValidFile,
                showCloseButton: true
            });
            return;
        }

        // Upload the excel file here.
        $scope.uploadExcel = Upload.upload({
            url: config.SERVICE_URL + '/TradelaneBooking/GetPiecesDetailFromExcel',
            file: $file,
            fields: {
                ShipmentId: $scope.tradelaneBooking.TradelaneShipmentId
            }
        });

        $scope.uploadExcel.progress($scope.progressExcel);

        $scope.uploadExcel.success($scope.successExcel);

        $scope.uploadExcel.error($scope.errorExcel);
    };

    $scope.progressExcel = function (evt) {
        //To Do:  show excel uploading progress message 
    };

    $scope.successExcel = function (data, status, headers, config) {
        if (status = 200) {
            if (data.Message == "Ok" || data.Message == "ok" || data.Message == "OK") {
                //to show pop - up
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'tradelaneBooking/pacakgeDetail.tpl.html',
                    controller: 'TradelanePacakgeDetailController',
                    windowClass: 'directBookingDetail',
                    size: 'lg',
                    backdrop: 'static',
                    keyboard: false,
                    resolve: {
                        ShipmentId: function () {
                            return $scope.tradelaneBooking.TradelaneShipmentId;
                        },
                        PackageCalculatonType: function () {
                            return $scope.tradelaneBooking.PakageCalculatonType;
                        },
                        FrayteNumber: function () {
                            return $scope.tradelaneBooking.FrayteNumber;
                        },
                        Packages: function () {
                            return null;
                        },
                        CallingFrom: function () {
                            return "Excel";
                        },
                        HAWB: function () {
                            return '';
                        },
                        HAWBNumber: function () {
                            return 0;
                        },
                        TotalUploaded: function () {
                            return data.TotalUploaded;
                        },
                        SuccessUploaded: function () {
                            return data.SuccessUploaded;
                        },
                        CustomerId: function () {
                            if ($scope.customerId !== undefined && $scope.customerId !== null && $scope.customerId !== '') {
                                return $scope.customerId;
                            }
                            else {
                                return 0;
                            }
                        }
                    }
                });
                modalInstance.result.then(function (response) {
                    hawbpackages();
                }, function () {
                });
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.NotRequiredFoundCheckUploadDownloadTemplateExcelColumns,
                    showCloseButton: true
                });
            }
        }
        else {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.Errorwhil_uploading_the_excel,
                showCloseButton: true
            });
        }
    };

    $scope.errorExcel = function (err) {
        toaster.pop({
            type: 'error',
            title: $scope.TitleFrayteError,
            body: $scope.Errorwhil_uploading_the_excel,
            showCloseButton: true
        });
    };

    $scope.showPopUp = function (HAWB, HAWBNumber) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'tradelaneBooking/pacakgeDetail.tpl.html',
            controller: 'TradelanePacakgeDetailController',
            windowClass: 'directBookingDetail',
            size: 'lg',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                ShipmentId: function () {
                    return $scope.tradelaneBooking.TradelaneShipmentId;
                },
                PackageCalculatonType: function () {
                    return $scope.tradelaneBooking.PakageCalculatonType;
                },
                FrayteNumber: function () {
                    return $scope.tradelaneBooking.FrayteNumber;
                },
                Packages: function () {
                    return null;
                },
                CallingFrom: function () {
                    return "Excel";
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
                },
                CustomerId: function () {
                    return $scope.customerId;
                }
            }
        });
        modalInstance.result.then(function (response) {
            hawbpackages();
        }, function () {
        });
    };

    $scope.totalCarton = function () {
        if ($scope.hawbGroupedPackages && $scope.hawbGroupedPackages.length) {
            var sum = 0;
            for (var i = 0; i < $scope.hawbGroupedPackages.length; i++) {
                sum += Math.abs(parseInt($scope.hawbGroupedPackages[i].TotalCartons, 10));
            }
            return sum;
        }
        else {
            return 0;
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

    $scope.totalShipments = function () {

        if ($scope.hawbGroupedPackages && $scope.hawbGroupedPackages.length) {
            var sum = 0;
            for (var i = 0; i < $scope.hawbGroupedPackages.length; i++) {

                sum += $scope.hawbGroupedPackages[i].Packages.length;
            }
            return sum;
        }
        else {
            return 0;
        }
    };

    $scope.totalWeight = function () {
        if ($scope.hawbGroupedPackages && $scope.hawbGroupedPackages.length) {
            var sum = 0;
            for (var i = 0; i < $scope.hawbGroupedPackages.length; i++) {
                // this not showing weight in decimals 
                // sum += Math.abs(parseInt($scope.hawbGroupedPackages[i].EstimatedWeight, 10)); 
                sum = parseFloat(sum + $scope.hawbGroupedPackages[i].EstimatedWeight).toFixed(2);
            }
            return sum;
        }
        else {
            return 0;
        }
    };

    function compare(a, b) {
        if (!isNaN(parseInt(a.HAWB, 10)) && !isNaN(parseInt(b.HAWB, 10))) {
            if (parseInt(a.HAWB, 10) < parseInt(b.HAWB, 10)) { return -1; }
            if (parseInt(a.HAWB, 10) > parseInt(b.HAWB, 10)) { return 1; }
        }
        return 0;
    }

    var setHAWBDocumentOrder = function () {
        if ($scope.hawbGroupedPackages) {
            $scope.hawbGroupedPackages.sort(compare);
        }
    };

    var hawbpackages = function () {
        TradelaneBookingService.GetGroupedHAWBPieces($scope.tradelaneBooking.TradelaneShipmentId).then(function (response) {
            $scope.hawbGroupedPackages = response.data;
            setHAWBDocumentOrder();
            TradelaneBookingService.IsAllHawbAssigned($scope.tradelaneBooking.TradelaneShipmentId).then(function (response) {
                $scope.Booking = response.data;
            }, function () {
                $scope.Booking = {};

                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.SomeProblemOccured,
                    showCloseButton: true
                });
            });
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.SomeProblemOccured,
                showCloseButton: true
            });

        });
    };

    //Uploads and Download excel for Pieces grid
    $scope.shipmentDetail = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'tradelaneBooking/tradelaneBookingDetail/tradelaneBookingDetail.tpl.html',
            controller: 'TradelaneBookingDetailController',
            windowClass: 'directBookingDetail',
            size: 'lg',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                ShipmentId: function () {
                    return 2;
                }
            }
        });
        modalInstance.result.then(function (response) {


        }, function () {
        });
    };

    $scope.checkCurrencyValue = function (currencyElm) {
        if (currencyElm) {
            if (!parseFloat($scope.tradelaneBooking.DeclaredValue)) {
                currencyElm.$invalid = true;
                currencyElm.$valid = false;
            }
            else {
                currencyElm.$invalid = false;
                currencyElm.$valid = true;
            }
        }

    };

    //Screen Initials
    var getShipmentDetails = function () {
        TradelaneBookingService.GetBookingDetail($scope.shipmentId, "ShipmentClone").then(function (response) {
            if (response.data) {
                $scope.tradelaneBooking = response.data;
                $scope.customerId = response.data.CustomerId;

                getCustomerDefaultAddress();

                $scope.SetShipinfo($scope.tradelaneBooking.ShipFrom.Country, "Shipper");
                $scope.SetShipinfo($scope.tradelaneBooking.ShipTo.Country, "Receiver");
                if ($scope.tradelaneBooking.NotifyParty && $scope.tradelaneBooking.NotifyParty.Country && $scope.tradelaneBooking.NotifyParty.Country.CountryId) {
                    $scope.SetShipinfo($scope.tradelaneBooking.NotifyParty.Country, "notify");
                }

                if (!parseFloat($scope.tradelaneBooking.DeclaredValue)) {
                    $scope.tradelaneBooking.DeclaredValue = null;
                }
                //
                $scope.changeKgToLb($scope.tradelaneBooking.PakageCalculatonType);
                if ($scope.userInfo.RoleId !== 3) {
                    TradelaneBookingService.GetCustomers($scope.userInfo.EmployeeId, "Tradelane").then(function (response) {
                        $scope.customers = response.data;
                        var dbCustomers = [];
                        for (i = 0; i < $scope.customers.length; i++) {
                            if ($scope.customers[i].CustomerId) {
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
                            if ($scope.tradelaneBooking.CustomerId === $scope.customers[k].CustomerId) {
                                $scope.CustomerDetail = $scope.customers[k];
                            }
                        }
                    }, function () {
                        toaster.pop({
                            type: 'error',
                            title: $scope.TitleFrayteError,
                            body: $scope.SomeErrorOccuredTryAgain,
                            showCloseButton: true
                        });
                    });
                }
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

        TradelaneBookingService.BookingInitials($scope.userInfo.EmployeeId).then(function (response) {

            AppSpinner.hideSpinnerTemplate();

            // Set Country type according to given order
            $scope.CountriesRepo = TopCountryService.TopCountryList(response.data.Countries);
            $scope.PiecesExcelDownloadPathXlsx = response.data.PiecesExcelDownloadPathXlsx;
            $scope.PiecesExcelDownloadPathXls = response.data.PiecesExcelDownloadPathXls;
            $scope.PiecesExcelDownloadPathCsv = response.data.PiecesExcelDownloadPathCsv;
            // Set Currency type according to given order

            $scope.CurrencyTypes = TopCurrencyService.TopCurrencyList(response.data.CurrencyTypes);

            $scope.ShipmentMethods = response.data.ShipmentMethods;
            $scope.CustomerDetail = response.data.CustomerDetail;
            $scope.Incoterms = response.data.Incoterm;

            $scope.CustomerAddress = response.data.CustomerAddress;
            $scope.AirLines = TopAirlineService.TopAirlineList(response.data.AirLines);

            $scope.ClearButtonEnable = false;
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

                $scope.changeKgToLb($scope.tradelaneBooking.PakageCalculatonType);

                if ($scope.RoleId === 3) {
                    $scope.tradelaneBooking.CustomerAccountNumber = $scope.CustomerDetail.AccountNumber;

                    $scope.SetCustomerAddress();
                    // need to set departure , destination airports 

                }
                else {

                    TradelaneBookingService.GetCustomers($scope.userInfo.EmployeeId, "Tradelane").then(function (response) {
                        $scope.customers = response.data;

                        var dbCustomers = [];
                        for (i = 0; i < $scope.customers.length; i++) {
                            if ($scope.customers[i].CustomerId) {
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

                            //var cus = {
                            //    CustomerId : 0,
                            //    CustomerName: "Ad new customer",
                            //    CompanyName: "Ad new customer",
                            //    AccountNumber : "xxx-xxx-xxx",
                            //    EmailId :"",
                            //    ValidDays : 0,
                            //    CustomerCurrency : "",
                            //    OperationZoneId : 0
                            //}; 
                            //$scope.customers.push(cus);  
                        }

                    }, function () {
                        toaster.pop({
                            type: 'error',
                            title: $scope.TitleFrayteError,
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
                $scope.tradelaneBooking.CustomerAccountNumber = CustomerDetail.AccountNumber;
                $scope.tradelaneBooking.customerId = CustomerDetail.CustomerId;
                $scope.customerId = CustomerDetail.CustomerId;

                $scope.newCustomerAccountInfo = false;
                $scope.isValidAccountNumber = true;

                getCustomerDefaultAddress();
            }
            else {
                $scope.tradelaneBooking.CustomerAccountNumber = '';
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
                $scope.tradelaneBooking.ShipFrom.Country = $scope.CustomerAddress.ShipFrom.Country;
                $scope.tradelaneBooking.ShipFrom.PostCode = $scope.CustomerAddress.ShipFrom.PostCode;
                $scope.tradelaneBooking.ShipFrom.FirstName = $scope.CustomerAddress.ShipFrom.FirstName;
                $scope.tradelaneBooking.ShipFrom.LastName = $scope.CustomerAddress.ShipFrom.LastName;
                $scope.tradelaneBooking.ShipFrom.CompanyName = $scope.CustomerAddress.ShipFrom.CompanyName;
                $scope.tradelaneBooking.ShipFrom.Address = $scope.CustomerAddress.ShipFrom.Address;
                $scope.tradelaneBooking.ShipFrom.Address2 = $scope.CustomerAddress.ShipFrom.Address2;
                $scope.tradelaneBooking.ShipFrom.City = $scope.CustomerAddress.ShipFrom.City;
                $scope.tradelaneBooking.ShipFrom.Area = $scope.CustomerAddress.ShipFrom.Suburb;
                $scope.tradelaneBooking.ShipFrom.State = $scope.CustomerAddress.ShipFrom.State;
                $scope.tradelaneBooking.ShipFrom.Phone = $scope.CustomerAddress.ShipFrom.Phone;

                $scope.SetShipinfo($scope.tradelaneBooking.ShipFrom.Country, "Shipper");

                $scope.shipFromDefault = $scope.CustomerAddress.ShipFrom.IsDefault;

            }
            else {
                $scope.shipFromDefault = false;
            }

            if ($scope.CustomerAddress.ShipTo) {
                $scope.tradelaneBooking.ShipTo.Country = $scope.CustomerAddress.ShipTo.Country;
                $scope.tradelaneBooking.ShipTo.PostCode = $scope.CustomerAddress.ShipTo.PostCode;
                $scope.tradelaneBooking.ShipTo.FirstName = $scope.CustomerAddress.ShipTo.FirstName;
                $scope.tradelaneBooking.ShipTo.LastName = $scope.CustomerAddress.ShipTo.LastName;
                $scope.tradelaneBooking.ShipTo.CompanyName = $scope.CustomerAddress.ShipTo.CompanyName;
                $scope.tradelaneBooking.ShipTo.Address = $scope.CustomerAddress.ShipTo.Address;
                $scope.tradelaneBooking.ShipTo.Address2 = $scope.CustomerAddress.ShipTo.Address2;
                $scope.tradelaneBooking.ShipTo.City = $scope.CustomerAddress.ShipTo.City;
                $scope.tradelaneBooking.ShipTo.Area = $scope.CustomerAddress.ShipTo.Suburb;
                $scope.tradelaneBooking.ShipTo.State = $scope.CustomerAddress.ShipTo.State;
                $scope.tradelaneBooking.ShipTo.Phone = $scope.CustomerAddress.ShipTo.Phone;

                $scope.SetShipinfo($scope.tradelaneBooking.ShipTo.Country, "Receiver");

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

    var getCustomerDefaultAddress = function () {
        TradelaneBookingService.userDefalutAddress($scope.customerId).then(function (response) {
            if (response.data) {
                $scope.CustomerAddress = response.data;
                $scope.SetCustomerAddress();
            }

        }, function () {

        });
    };

    //aditya code
    $scope.SetValueNotZero = function (CartonNumber) {
        if (CartonNumber !== undefined && CartonNumber !== null && CartonNumber === "0") {
            return;
        }
        return CartonNumber;
    };

    $scope.AddPackage = function () {
        $scope.HideContent = true;
        $scope.tradelaneBooking.Packages.push({
            TradelaneShipmentDetailId: 0,
            Length: null,
            Width: null,
            Height: null,
            Weight: null,
            CartonNumber: null,
            HAWB: null
        });
        var dbpac = $scope.tradelaneBooking.Packages.length - 1;
        for (i = 0; i < $scope.tradelaneBooking.Packages.length; i++) {

            if (i === dbpac) {
                $scope.tradelaneBooking.Packages[i].pacVal = true;
            }
            else {
                $scope.tradelaneBooking.Packages[i].pacVal = false;
            }
        }
    };

    $scope.RemovePackage = function (Package) {
        if (Package !== undefined && Package !== null) {
            var index = $scope.tradelaneBooking.Packages.indexOf(Package);
            if ($scope.tradelaneBooking.Packages.length === 2) {
                $scope.HideContent = false;
            }
            if (Package.TradelaneShipmentDetailId > 0) {
                DirectBookingService.DeleteDirectShipmentPackage(Package.TradelaneShipmentDetailId).then(function (response) {
                    if (response.data.Status) {
                        toaster.pop({
                            type: 'success',
                            title: $scope.TitleFrayteSuccess,
                            body: $scope.RemovePackageValidation,
                            showCloseButton: true
                        });
                        $scope.tradelaneBooking.Packages.splice(index, 1);
                        $scope.Packges = angular.copy($scope.tradelaneBooking.Packages);
                        $scope.tradelaneBooking.Packages = [];
                        $scope.tradelaneBooking.Packages = $scope.Packges;
                    }

                }, function () {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.RemovePackageValidation,
                        showCloseButton: true
                    });
                });
            }
            else {
                if (index === $scope.tradelaneBooking.Packages.length - 1) {
                    var dbpac = $scope.tradelaneBooking.Packages.length - 2;
                    for (i = 0; i < $scope.tradelaneBooking.Packages.length; i++) {

                        if (i === dbpac) {
                            $scope.tradelaneBooking.Packages[i].pacVal = true;
                        }
                        else {
                            $scope.tradelaneBooking.Packages[i].pacVal = false;
                        }
                    }
                }
                else {
                    var dbpac1 = $scope.tradelaneBooking.Packages.length - 1;
                    for (i = 0; i < $scope.tradelaneBooking.Packages.length; i++) {

                        if (i === dbpac1) {
                            $scope.tradelaneBooking.Packages[i].pacVal = true;
                        }
                        else {
                            $scope.tradelaneBooking.Packages[i].pacVal = false;
                        }
                    }
                }
                $scope.tradelaneBooking.Packages.splice(index, 1);
                $scope.Packges = angular.copy($scope.tradelaneBooking.Packages);
                $scope.tradelaneBooking.Packages = [];
                $scope.tradelaneBooking.Packages = $scope.Packges;
            }
        }
    };

    $scope.SetMaxixumWeight = function (Weight, CartonNumber, pieceDetailOption, tradelaneBooking) {
        if (pieceDetailOption === "kgToCms") {
            if (Weight) {
                $scope.isdisabled = true;
            }
            else {
                if (tradelaneBooking.Packages.length > 1) {
                    var isWegt = false;
                    angular.forEach(tradelaneBooking.Packages, function (CartonNumber, key) {

                        if (CartonNumber.Weight) {
                            $scope.isdisabled = true;
                            isWegt = true;
                        }
                        else {
                            if (!isWegt) {
                                $scope.isdisabled = false;
                            }
                        }
                    });
                }
                else {
                    $scope.isdisabled = false;
                }
            }

            // for kg
            if (parseInt(Weight, 10) > 70) {
                //$scope.Package.Weight = '';
                var finalWeightkg = '';
                var weightkg = Weight.toString().split('');
                var weightkgnew = weightkg.splice(weightkg.length - 1);
                for (i = 0; i < weightkg.length; i++) {
                    finalWeightkg = finalWeightkg + weightkg[i];
                }
                Weight = finalWeightkg;
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.MaxWeight70kGS,
                    showCloseButton: true
                });
            }
            return Weight;
        }
        if (pieceDetailOption === "lbToInchs") {

            if (Weight) {
                $scope.isdisabled = true;

            }
            else {
                if (tradelaneBooking.Packages.length > 1) {
                    var isWegtlbs = false;
                    angular.forEach(tradelaneBooking.Packages, function (CartonNumber, key) {

                        if (CartonNumber.Weight) {
                            $scope.isdisabled = true;
                            isWegtlbs = true;
                        }
                        else {
                            if (!isWegtlbs) {
                                $scope.isdisabled = false;
                            }
                        }
                    });

                }
                else {
                    $scope.isdisabled = false;
                }
            }
            if (parseInt(Weight, 10) > 154) {

                //$scope.Package.Weight = '';
                var finalWeightlb = '';
                var weightlbs = Weight.toString().split('');
                var weightlbsnew = weightlbs.splice(weightlbs.length - 1);
                for (i = 0; i < weightlbs.length; i++) {
                    finalWeightlb = finalWeightlb + weightlbs[i];
                }
                Weight = finalWeightlb;
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.MaxWeight154KGS,
                    showCloseButton: true
                });
            }
        }
        return Weight;
    };

    $scope.totalPieces = function (tradelaneBooking) {
        if (tradelaneBooking !== undefined && tradelaneBooking !== null && tradelaneBooking.Packages !== null && tradelaneBooking.Packages.length) {
            var sum = 0;
            for (var i = 0; i < tradelaneBooking.Packages.length; i++) {
                if (tradelaneBooking.Packages[i].CartonNumber !== "" && tradelaneBooking.Packages[i].CartonNumber !== null && tradelaneBooking.Packages[i].CartonNumber !== undefined) {
                    sum += Math.abs(parseInt(tradelaneBooking.Packages[i].CartonNumber, 10));
                }
            }
            return sum;
        }
        else {
            return 0;
        }
    };

    $scope.getTotalWeightKgs = function () {
        if ($scope.tradelaneBooking === undefined) {
            return;
        }
        else if ($scope.tradelaneBooking.Packages === undefined || $scope.tradelaneBooking.Packages === null) {
            return 0;
        }
        else if ($scope.tradelaneBooking.Packages.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.tradelaneBooking.Packages.length; i++) {
                var product = $scope.tradelaneBooking.Packages[i];
                if (product.Weight === null || product.Weight === undefined) {
                    total += parseFloat(0);
                }
                else {
                    if (product.CartonNumber === undefined || product.CartonNumber === null) {
                        var catroon = parseFloat(0);
                        total = total + parseFloat(product.Weight) * catroon;
                    }
                    else {
                        total = total + parseFloat(product.Weight) /** product.CartonNumber*/;
                    }
                }
            }
            return parseFloat(total).toFixed(2);
        }
        else {
            return 0;
        }
    };

    $scope.getTotalKgs = function () {
        if ($scope.tradelaneBooking === undefined) {
            return;
        }
        else if ($scope.tradelaneBooking.Packages === undefined || $scope.tradelaneBooking.Packages === null) {
            return 0;
        }
        else if ($scope.tradelaneBooking.Packages.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.tradelaneBooking.Packages.length; i++) {
                var product = $scope.tradelaneBooking.Packages[i];
                if (product.Weight === null || product.Weight === undefined) {
                    total += parseFloat(0);
                }
                else {
                    if (product.CartonNumber === undefined || product.CartonNumber === null) {
                        var catroon = parseFloat(0);
                        total = total + parseFloat(product.Weight) * catroon;
                    }
                    else {
                        total = total + parseFloat(product.Weight) /** product.CartonNumber*/;
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

    $scope.getChargeableWeight = function (items, prop) {
        if ($scope.tradelaneBooking === undefined) {
            return;
        }

        else if ($scope.tradelaneBooking.Packages === undefined || $scope.tradelaneBooking.Packages === null) {
            return 0;
        }

        if ($scope.tradelaneBooking.Packages.length >= 0) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.tradelaneBooking.Packages.length; i++) {
                var product = $scope.tradelaneBooking.Packages[i];
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
                if (product.CartonNumber === null || product.CartonNumber === undefined) {
                    qty = parseFloat(0);
                }
                else {
                    qty = parseFloat(product.CartonNumber);
                }
                if (len > 0 && wid > 0 && height > 0) {
                    if ($scope.tradelaneBooking.PakageCalculatonType === "kgToCms") {
                        total += ((len * wid * height) / 5000) * qty;
                    }
                    else if ($scope.tradelaneBooking.PakageCalculatonType === "lbToInchs") {
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

    $scope.allocateHAWBConfirmation = function () {        
  
    };
 
    $scope.allocateHAWB = function () {

       //for (i = 0; i < $scope.Item.length; i++) {

           //if (Item[i].Length !== undefined && Item[i].Length !== null && Item[i].Length !== '' &&
           //    Item[i].CartonNumber !== undefined && Item[i].CartonNumber !== null && Item[i].CartonNumber !== '' &&
           //    Item[i].Width !== undefined && Item[i].Width !== null && Item[i].Width !== '' &&
           //    Item[i].Height !== undefined && Item[i].Height !== null && Item[i].Height !== '' &&
           //    Item[i].Weight !== undefined && Item[i].Weight !== null && Item[i].Weight !== '') {

                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'tradelaneBooking/allocateHawbConfirmation.tpl.html',
                    controller: 'TradelaneBookingAllocateHawbController',
                    windowClass: '',
                    size: 'md',
                    backdrop: 'static',
                    keyboard: false

                });
                modalInstance.result.then(function (response) {
                    if (response === 'Yes') {

                        $scope.tradelaneBooking.Packages = $scope.tradelaneBooking.Packages.map(function (address) {
                            address.CartonValue = "1";
                            return address;
                        });
                        //$scope.tradelaneBooking.Packages.TradelaneShipmentId = $scope.tradelaneBooking.TradelaneShipmentId;
                        $scope.tradelaneBooking.Packages = $scope.tradelaneBooking.Packages.map(function (address1) {
                            address1.TradelaneShipmentId = $scope.tradelaneBooking.TradelaneShipmentId;
                            return address1;
                        });

                        TradelaneBookingService.SavePcsHAWB($scope.tradelaneBooking.Packages).then(function (response) {
                        });

                        var modalInstance = $uibModal.open({
                            animation: true,
                            templateUrl: 'tradelaneBooking/pacakgeDetail.tpl.html',
                            controller: 'TradelanePacakgeDetailController',
                            windowClass: 'directBookingDetail',
                            size: 'lg',
                            backdrop: 'static',
                            keyboard: false,
                            resolve: {
                                ShipmentId: function () {
                                    return $scope.tradelaneBooking.TradelaneShipmentId;
                                },
                                PackageCalculatonType: function () {
                                    return $scope.tradelaneBooking.PakageCalculatonType;
                                },
                                FrayteNumber: function () {
                                    return $scope.tradelaneBooking.FrayteNumber;
                                },

                                Packages: function () {
                                    return $scope.tradelaneBooking.Packages;
                                },

                                CallingFrom: function () {
                                    return "Allocate";
                                },
                                HAWB: function () {
                                    return '';
                                },
                                HAWBNumber: function () {
                                    return 0;
                                },
                                CustomerId: function () {
                                    return $scope.customerId;
                                },
                                TotalUploaded: function () {
                                    return $scope.tradelaneBooking.Packages.length;
                                },
                                SuccessUploaded: function () {
                                    return $scope.tradelaneBooking.Packages.length;
                                }
                            }
                        });
                        modalInstance.result.then(function (response) {
                            hawbpackages();
                        }, function () {
                        });
                    }
                    else {

                    }
                    hawbpackages();
                }, function () {
                });
            //}
      // }
        
    };

    $scope.allo = function (TradelaneBookingForm) {
        if (TradelaneBookingForm.$valid === true) {
            $scope.allocateHAWB();

        } else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextValidation,
                showCloseButton: true
            });
        }
    };
    //aditya code ends

    //toggle fields
    function init() {
        $scope.isPackageInformation = true;
        $scope.submitted = true;

        $scope.emailFormat = /^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,10};$/;
        //$scope.emailFormat = /(([a-zA-Z\-0-9\.]+@)([a-zA-Z\-0-9\.]+)[;]*)+/g;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.ShipmentStatus = {
            Draft: 27,
            ShipmentBooked: 28,
            Pending: 29,
            Delivered: 35,
            Rejeted: 34,
            Departed: 30,
            Intransit: 31,
            Arrived: 32
        };

        $scope.ImagePath = config.BUILD_URL;
        $scope.batteryDeclaration = {
            key: 'None',
            value: 'None'
        };

        $scope.batteryDeclarations = [
            {
                key: 'None',
                value: 'None'
            },
            {
                key: '966SII',
                value: '966SII'
            },
            {
                key: 'PI68SII',
                value: 'PI68SII'
            }
        ];

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

        $scope.CreatedBy = $scope.userInfo.EmployeeId;
        $scope.CountriesRepo = [];
        $scope.CurrencyTypes = [];
        //new SetMultilingualOtions();
        setMultilingualOtions();
        if (isNaN($stateParams.shipmentId)) {
            //$rootScope.isNotFound = true;
        }
        else {
            getScreenInitials();
        }
    }

    init();
});
