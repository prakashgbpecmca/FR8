
angular.module('ngApp.parcelhub').controller('ParcelHubAddEditController', function ($scope, ParcelHubService, $state, mode, ParcelHubKey, $translate, uiGridConstants, SettingService, config, $filter, LogonService, $uibModalInstance,SessionService, $uibModal, toaster) {
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors',
            'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation',
            'PleaseCorrectValidationErrors', 'RateCardSave_Validation', 'Select_CourierAccount', 'THP_Matrix_Saved', 'DataSaved_Validation',
        'Record_Saved', 'APIkeyUserkey_Saved_Validation', 'ParcelHubKeysSavingErrorValidation']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
            $scope.RecordGettingError = translations.ErrorGetting + " " + translations.records;
            $scope.Success = translations.Success;
            $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
            $scope.PleaseCorrectValidationErrors = translations.PleaseCorrectValidationErrors;
            $scope.RecordSaved = translations.Record_Saved;
            $scope.APIkeyUserkeySavedValidation = translations.APIkeyUserkey_Saved_Validation;
            $scope.ParcelHubKeysSavingErrorValidation = translations.ParcelHubKeysSavingError_Validation;
        });
    };
    $scope.submit = function (IsValid) {
        if (IsValid) {
            ParcelHubService.SaveParcelHubKey($scope.ParcelHubKey).then(function (response) {
                if (response.data !== null && response.data.Status === true) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.Success,
                        body: $scope.APIkeyUserkeySavedValidation,
                        showCloseButton: true
                    });
                    $uibModalInstance.close($scope.ParcelHubKey);
                }
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.ParcelHubKeysSavingErrorValidation,
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

    function init() {
        if (mode !== undefined) {
            $translate(mode).then(function (mode) {
                $scope.Mode = mode;
            });
        }

        if (ParcelHubKey !== undefined) {
            $scope.ParcelHubKey = ParcelHubKey;
            setModalOptions();
        }
    }
    init();

});
