/**
 * Controller
 */
angular.module('ngApp.shipper').controller('AddressBookAddEditController', function ($uibModalStack, $state, $scope, $uibModal, addressBooks, addressBook, mode, $translate, ShipmentService, $uibModalInstance, toaster, ModalService) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteValidation', 'FrayteError', 'ErrorGetting', 'PleaseCorrectValidationErrors', 'customer', 'Branch', 'detail',
            'records', 'Cancel_Validation', 'Confirmation', 'Cancel']).then(function (translations) {
            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TitleFrayteError = translations.FrayteError;

            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextErrorGettingCustomerRecords = translations.ErrorGetting + " " + translations.customer + " " + translations.Branch + " " + translations.detail + " " + translations.records;
            $scope.CancelConfirmation = translations.Cancel + " " + translations.Confirmation;
            $scope.CancelValidation = translations.Cancel_Validation;
        });
    };

    $scope.submit = function (isValid, pickupAddress) {
        if (isValid) {
            if (pickupAddress.SN === undefined || pickupAddress.SN === 0) {
                pickupAddress.SN = $scope.addressBooks.length + 1;
                $scope.addressBooks.push(pickupAddress);
            }
            else {
                //Need to update the holiday collection and then return back to main grid
                $scope.updateAddressBookCollection(pickupAddress);
            }

            $uibModalInstance.close($scope.addressBooks);
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

    $scope.updateAddressBookCollection = function (pickupAddress) {
        var objects = $scope.addressBooks;

        for (var i = 0; i < objects.length; i++) {
            if (objects[i].SN === pickupAddress.SN) {
                objects[i] = pickupAddress;
                break;
            }
        }
    };

    $scope.GetCustomerBranchInitials = function () {
        ShipmentService.GetInitials().then(function (response) {
            $scope.countries = response.data.Countries;
            $scope.timezones = response.data.TimeZones;
            $scope.countryPhoneCodes = response.data.CountryPhoneCodes;
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGettingCustomerRecords,
                showCloseButton: true
            });
        });
    };
    // Set State And Zip on Country Select
    $scope.SetShipperReceiverStateZip = function (country) {
        if (country !== undefined && country !== "" && country !== null && country.Code !== null && country.Code !== '' && country.Code !== undefined) {
            if (country.Code === "HKG") {
                $scope.setStateDisable = true;
                $scope.setZipDisable = true;
                $scope.pickupAddress.Zip = null;
                $scope.pickupAddress.State = null;
            }
            else if (country.Code === "GBR") {
                $scope.setStateDisable = true;
                $scope.setZipDisable = false;
                $scope.pickupAddress.State = null;
            }
            else {
                $scope.setZipDisable = false;
                $scope.setStateDisable = false;
            }
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

        //Set initial value and other things
        //$scope.mode = mode;

        if (mode === "Add") {
            $translate('Add').then(function (add) {
                $scope.mode = add;
            });
        }
        if (mode === "Modify") {
            $translate('Modify').then(function (modify) {
                $scope.mode = modify;
            });
        }
        $scope.addressBooks = addressBooks;

        $scope.pickupAddress = {
            SN: addressBook.SN,
            UserAddressId: addressBook.UserAddressId,
            Address: addressBook.Address,
            Address2: addressBook.Address2,
            Address3: addressBook.Address3,
            Suburb: addressBook.Suburb,
            City: addressBook.City,
            State: addressBook.State,
            Zip: addressBook.Zip,
            Country: addressBook.Country
        };

        $scope.GetCustomerBranchInitials();
    }

    $scope.AddressGoBack = function () {
        var modalOptions = {
            headerText: $scope.CancelConfirmation,
            bodyText: $scope.CancelValidation
        };

        ModalService.Confirm({}, modalOptions).then(function () {
            //$state.go('dbuser.customer-detail.basic-detail');
            $uibModalStack.dismissAll('ok');
        }, function () {


        });
    };

    init();

});