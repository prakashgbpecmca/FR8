angular.module('ngApp.directBooking').controller('DirectBookingAddressBookController', function ($scope, mode, PhoneCodes, $uibModalInstance, Countries, AddressDetail, $filter, $state, $translate, $uibModal, $timeout, toaster, $stateParams, DirectBookingService, SessionService, uiGridConstants) {

    var setMultilingualOptions = function () {
        $translate(['Success', 'Record_Saved', 'FrayteError_Validation', 'ErrorSavingRecord', 'FrayteWarning_Validation', 'PleaseCorrectValidationErrors', 'Address_Saved_Successfully', 'Already_Exist_Or_Save_Other_Address']).then(function (translations) {
            $scope.Success = translations.Success;
            $scope.Address_Saved_Successfully = translations.Address_Saved_Successfully;
            $scope.FrayteErrorValidation = translations.FrayteError_Validation;
            $scope.ErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
            $scope.PleaseCorrectValidationErrors = translations.PleaseCorrectValidationErrors;
            $scope.Duplicate_Address_Validation = translations.Already_Exist_Or_Save_Other_Address;
        });
    };

    $scope.SaveAddressBook = function (IsValid) {
        if (IsValid) {
            DirectBookingService.SaveAddressBook($scope.AddressBook).then(function (response) {
                if (response.data.Status === true) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.Success,
                        body: $scope.Address_Saved_Successfully,
                        showCloseButton: true
                    });
                    $uibModalInstance.close();
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteWarningValidation,
                        body: $scope.Duplicate_Address_Validation,
                        showCloseButton: true
                    });
                }
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteErrorValidation,
                    body: $scope.ErrorSavingRecord,
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarningValidation,
                body: $scope.PleaseCorrectValidationErrors,
                showCloseButton: true
            });
        }
    };

    // Set Country Phone Code
    $scope.setAddressBookinfo = function (Country) {
        if (Country !== undefined && Country !== null) {
            for (var i = 0 ; i < $scope.CountryPhoneCodes.length ; i++) {
                if ($scope.CountryPhoneCodes[i].CountryCode === Country.Code) {
                    $scope.ShipFromPhoneCode = "(+" + $scope.CountryPhoneCodes[i].PhoneCode + ")";
                    break;
                }
            }
            if (Country.Code === "HKG") {
                $scope.AddressBook.PostCode = null;
            }
            if (Country.Code === "GBR") {
                $scope.AddressBook.State = null;
            }
        }
    };

    $scope.Cancel = function () {
        $uibModalInstance.close();
    };

    $scope.Dismiss1 = function () {
        $uibModalInstance.close();
    };

    function init() {

        $scope.emailFormat = /^[a-z0-9._]+@[a-z]+\.[a-z.]/;
        $scope.AddressBook = AddressDetail;
        $scope.CountryPhoneCodes = PhoneCodes;
        if ($scope.AddressBook !== undefined && $scope.AddressBook !== null && $scope.AddressBook.Country !== null) {
            $scope.setAddressBookinfo($scope.AddressBook.Country);
        }
        $scope.mode = mode;

        $scope.CountriesRepo = Countries;

        setMultilingualOptions();
    }

    init();

});