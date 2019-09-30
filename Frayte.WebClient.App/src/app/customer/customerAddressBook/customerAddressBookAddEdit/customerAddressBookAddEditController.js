angular.module('ngApp.customer', []).controller('customerAddressBookAddEditController', function ($scope, $uibModalInstance, $filter, $state, $translate, $uibModal, $timeout, toaster, $stateParams, DirectBookingService, SessionService, uiGridConstants, AddressDetail, TopCountryService, ShipmentService, $rootScope, mode) {

    var setMultilingualOptions = function () {
        $translate(['Success', 'Record_Saved', 'FrayteError_Validation', 'ErrorSavingRecord', 'FrayteWarning_Validation', 'PleaseCorrectValidationErrors', 'AddressBook_Record_Saved_Successfully', 'Already_Exist_Or_Save_Other_Address']).then(function (translations) {
            $scope.Success = translations.Success;
            $scope.AddressBook_Record_Saved_Successfully = translations.AddressBook_Record_Saved_Successfully;
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
                    $rootScope.getAddressBook();
                    toaster.pop({
                        type: 'success',
                        title: $scope.Success,
                        body: $scope.AddressBook_Record_Saved_Successfully,
                        showCloseButton: true
                    });
                    $uibModalInstance.close({ reload: true });
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
        }
    };

    $scope.Cancel = function () {
        $uibModalInstance.close();
        $rootScope.getAddressBook();
    };

    $scope.Dismiss1 = function () {
        $uibModalInstance.close();
        $rootScope.getAddressBook();
    };


    function init() {

        $scope.emailFormat = /^[a-z0-9._]+@[a-z]+\.[a-z.]/;
        var userInfo = SessionService.getUser();
        if (userInfo !== undefined) {
            $scope.CustomerId = userInfo.EmployeeId;
            $scope.AddressBook = { CustomerId: userInfo.EmployeeId };
        }
        DirectBookingService.GetInitials($scope.CustomerId).then(function (response) {
            $scope.DirectBookingService = response.data.Countries;
        });
        ShipmentService.GetInitials().then(function (response) {
            $scope.CountriesRepo = TopCountryService.TopCountryList(response.data.Countries);
            $scope.CountryPhoneCodes = response.data.CountryPhoneCodes;
            if ($scope.CountryPhoneCodes !== undefined) {
                if ($scope.AddressBook !== undefined && $scope.AddressBook !== null && $scope.AddressBook.Country !== null) {
                    $scope.setAddressBookinfo($scope.AddressBook.Country);
                }
            }
        });

        $scope.AddressBook = AddressDetail;
        $scope.mode = mode;
        setMultilingualOptions();
    }

    init();

});