angular.module('ngApp.baseRateCard').controller('BaseRateCardLimitController', function ($scope, $uibModalInstance,OperationZone, RateLimit, $filter, $state, toaster, $translate, $uibModal, ZoneBaseRateCardService, $window) {
    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation', 'PleaseCorrectValidationErrors', 'RateCardSaveLimit_Validation']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
            $scope.RecordGettingError = translations.ErrorGetting + " " + translations.records;
            $scope.Success = translations.Success;
            $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
            $scope.UpdateRecordValidation = translations.UpdateRecord_Validation;
            $scope.PleaseCorrectValidationErrors = translations.PleaseCorrectValidationErrors;
            $scope.RateCardSaveLimitValidation = translations.RateCardSaveLimit_Validation;
        });
    };
    $scope.submit = function (IsValid) {
        if (IsValid) {
            $scope.BaseRateLimitJson.push($scope.RateLimit);
            ZoneBaseRateCardService.SaveZoneBaseRateCardLimit($scope.BaseRateLimitJson).then(function (response) {
                if (response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.Success,
                        body: $scope.RateCardSaveLimit_Validation,
                        showCloseButton: true
                    });
                    $uibModalInstance.close($scope.RateLimit);
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorSavingRecord,
                        showCloseButton: true
                    });
                }

            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorSavingRecord,
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

    var getCurrencyCodes = function () {
        ZoneBaseRateCardService.GetOperationZoneCurrencyCode($scope.OperationZone.OperationZoneId, 'Buy').then(function (response) {
            if (response.data !== null) {
                $scope.CurrencyCodes = response.data;
            }

        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });
    };
    function init() {
        $scope.BaseRateLimitJson = [];
        $scope.OperationZone = OperationZone;
        getCurrencyCodes();
        $scope.RateLimit = RateLimit;
        setModalOptions();

    }
    init();
});