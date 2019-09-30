angular.module('ngApp.baseRateCard').controller('BaseRateCardWeightDimensionLimitController', function ($scope, $uibModalInstance, OperationZone, RateLimit, $filter, $state, toaster, $translate, $uibModal, ZoneBaseRateCardService,RateLimitType, $window) {
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
            $scope.BaseRateLimitJson.push($scope.RateLimit);
            if ($scope.RateLimitType === "Weight") {
                ZoneBaseRateCardService.SaveZoneBaseRateCardLimit($scope.BaseRateLimitJson).then(function (response) {
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
            }
            if ($scope.RateLimitType === "Dimension") {
                ZoneBaseRateCardService.SaveDimensionBaseRate($scope.BaseRateLimitJson).then(function (response) {
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
            }
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

                //var parcel = $filter('filter')($scope.CurrencyCodes, { CurrencyCode: $scope.RateLimit.Currency });
                //if (parcel.length) {
                //    $scope.RateLimit.CurrencyCode = parcel[0];
                //}
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
        $scope.RateLimitType = RateLimitType;
        $scope.BaseRateLimitJson = [];
        $scope.OperationZone = OperationZone;
        getCurrencyCodes();
        $scope.RateLimit = RateLimit;
        setModalOptions();
    }
    init();
});