angular.module('ngApp.baseRateCard').controller('BaseRateCardWeightDimensionLimitHermesController', function ($scope, ParcelType,BaseRateCardDimensionLimit, BaseRateCardWeightLimit, $uibModalInstance, OperationZone, RateLimit, $filter, $state, toaster, $translate, $uibModal, ZoneBaseRateCardService, $window) {
    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation', 'PleaseCorrectValidationErrors', 'RateCardSaveLimit_Validation']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;

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
            saveWeightLimit();
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
    var saveDimensionLimit = function () {
        for (var i = 0 ; i < $scope.BaseRateCardDimensionLimit.length; i++) {
            if ($scope.BaseRateCardDimensionLimit[i].Parceltype === $scope.ParcelType) {
                $scope.BaseRateCardDimensionLimit[i].DimensionRate = $scope.RateLimit.OverWeightDimensionRate.OverWeightLimitRate;
            }
        }
        ZoneBaseRateCardService.SaveDimensionBaseRate($scope.BaseRateCardDimensionLimit).then(function (response) {
            if (response.data.Status) {
                toaster.pop({
                    type: 'success',
                    title: $scope.Success,
                    body: $scope.RateCardSaveLimitValidation,
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
    };
    var saveWeightLimit = function () {
        for (var i = 0 ; i < $scope.BaseRateCardWeightLimit.length; i++) {
            if ($scope.BaseRateCardWeightLimit[i].ParcelType === $scope.ParcelType) {
                $scope.BaseRateCardWeightLimit[i].OverWeightLimitRate = $scope.RateLimit.OverWeightDimensionRate.OverWeightLimitRate;
            }
        }
        ZoneBaseRateCardService.SaveZoneBaseRateCardLimit($scope.BaseRateCardWeightLimit).then(function (response) {
            if (response.data.Status) {
                saveDimensionLimit();
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
        $scope.BaseRateCardDimensionLimit = BaseRateCardDimensionLimit;
        $scope.BaseRateCardWeightLimit = BaseRateCardWeightLimit;
        $scope.ParcelType = ParcelType;
        setModalOptions();
    }
    init();
});