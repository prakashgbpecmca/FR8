angular.module('ngApp.baseRateCard').controller('BaseRateCurrencyCourierAccountAddEditController', function ($scope, $filter, CourierAccounts, BaseRateCell, $state, OperationZone, toaster, $translate, $uibModal, $uibModalInstance, ZoneBaseRateCardService) {

    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors',
                    'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation',
                    'PleaseCorrectValidationErrors', 'RateCardSave_Validation', 'Select_CourierAccount', 'Enter_Base_Rate']).then(function (translations) {
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
                    $scope.RateCardSaveValidation = translations.RateCardSave_Validation;
                    $scope.SelectCourierAccount = translations.Select_CourierAccount;
                    $scope.EnterRate = translations.Enter_Base_Rate;
            });
    };

    $scope.submit = function (IsValid) {
        if (IsValid) {
            if ($scope.RateCard !== null && $scope.RateCard.courierAccount === null) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarningValidation,
                    body: $scope.SelectCourierAccount,
                    showCloseButton: true
                });
            }
            else if ($scope.RateCard !== null && $scope.RateCard.courierAccount !== null && $scope.RateCard.courierAccount.LogisticServiceCourierAccountId === 0) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarningValidation,
                    body: $scope.SelectCourierAccount,
                    showCloseButton: true
                });
            }
            else {
                $scope.RateCard.IsChanged = true;
                $scope.ZoneBaseRateCardSaveJson.push($scope.RateCard);
                ZoneBaseRateCardService.SaveZoneBaseRateCard($scope.ZoneBaseRateCardSaveJson).then(function (response) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.Success,
                        body: $scope.RateCardSaveValidation,
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
            }
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarningValidation,
                body: $scope.EnterRate,
                showCloseButton: true
            });
        }
    };

    $scope.Cancel = function (Rate) {
        if (Rate === undefined || Rate === null || Rate === '') {
            $scope.RateCard.Rate = $scope.RateCard.BusinessRate;
            $scope.ZoneBaseRateCardSaveJson.push($scope.RateCard);
            $uibModalInstance.dismiss('cancel');
        }
        else if (Rate === '0') {
            $scope.RateCard.Rate = $scope.RateCard.BusinessRate;
            $scope.ZoneBaseRateCardSaveJson.push($scope.RateCard);
            $uibModalInstance.dismiss('cancel');
        }
        else {
            $uibModalInstance.dismiss('cancel');
        }
    };

    $scope.refreshData = function () {
        $scope.CourierAccounts = $filter('filter')($scope.data, $scope.searchText, undefined);
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

    $scope.SetCourierAccountForBaseRate = function (CourierAccount) {
        if (CourierAccount !== undefined && CourierAccount !== null) {
            $scope.RateCard.courierAccount = CourierAccount;
        }
        for (var i = 0 ; i < $scope.CourierAccounts.length; i++) {
            if ($scope.CourierAccounts[i].IsSelected) {
                $scope.CourierAccounts[i].IsSelected = false;
            }
        }
        CourierAccount.IsSelected = true;
    };

    function init() {
        $scope.ZoneBaseRateCardSaveJson = [];
        $scope.RateCard = BaseRateCell;
        $scope.RateCardOperationZone = OperationZone;
        $scope.CourierAccounts = CourierAccounts;
        $scope.data = CourierAccounts;
        for (var i = 0 ; i < $scope.CourierAccounts.length; i++) {
            if ($scope.RateCard.courierAccount.LogisticServiceCourierAccountId == $scope.CourierAccounts[i].LogisticServiceCourierAccountId) {
                $scope.CourierAccounts[i].IsSelected = true;
            }
            else {
                $scope.CourierAccounts[i].IsSelected = false;
            }
        }
        getScreenInitials();
        setModalOptions();
    }

    init();
});