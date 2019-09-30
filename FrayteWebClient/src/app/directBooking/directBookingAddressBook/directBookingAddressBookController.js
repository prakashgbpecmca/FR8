angular.module('ngApp.directBooking').controller('DirectBookingAddressBookController', function ($scope, mode, PhoneCodes, $uibModalInstance, Countries, AddressDetail, $filter, $state, $translate, $uibModal, $timeout, toaster, $stateParams, DirectBookingService, SessionService, uiGridConstants) {
   
    var setMultilingualOptions = function () {
        $translate(['Success', 'Record_Saved', 'FrayteError_Validation', 'ErrorSavingRecord', 'FrayteWarning_Validation', 'PleaseCorrectValidationErrors']).then(function (translations) {
            $scope.Success = translations.Success;
            $scope.RecordSaved = translations.Record_Saved;
            $scope.FrayteErrorValidation = translations.FrayteError_Validation;
            $scope.ErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
            $scope.PleaseCorrectValidationErrors = translations.PleaseCorrectValidationErrors;

        });
    };

    $scope.SaveAddressBook = function (IsValid) {

        if (IsValid) {
            DirectBookingService.SaveAddressBook($scope.AddressBook).then(function () {
                toaster.pop({
                    type: 'success',
                    title: $scope.Success,
                    body: $scope.RecordSaved,
                    showCloseButton: true
                });
                $uibModalInstance.close();

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


    function init() {

       

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