angular.module('ngApp.baseRateCard').controller('baseRateCardWithoutAccountController', function ($uibModalInstance, ZoneBaseRateCardService, ParcelServiceType, PakageType, $scope, BaseRateCell, OperationZone, toaster, $state, $location, $translate) {
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
            $scope.RateCard.IsChanged = true;
            $scope.ZoneBaseRateCardSaveJson.push($scope.RateCard);
            ZoneBaseRateCardService.SaveZoneBaseRateCard($scope.ZoneBaseRateCardSaveJson).then(function (response) {
                toaster.pop({
                    type: 'success',
                    title: $scope.Success,
                    body: $scope.RateCardSaveLimitValidation,
                    showCloseButton: true
                });
                $uibModalInstance.close($scope.RateCard);
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorSavingRecord,
                    showCloseButton: true

                });
            });
            //  $uibModalInstance.close($scope.RateCard);

        }
    };
        var getScreenInitials = function () {

            ZoneBaseRateCardService.GetOperationZoneCurrencyCode($scope.RateCardOperationZone.OperationZoneId, 'Buy').then(function (response) {
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
            $scope.ZoneBaseRateCardSaveJson = [];
            $scope.PakageType = PakageType;
            $scope.ParcelType = ParcelServiceType;
            $scope.RateCard = BaseRateCell;
            $scope.RateCardOperationZone = OperationZone;
            getScreenInitials();
            setModalOptions();
        }

        init();
    });
