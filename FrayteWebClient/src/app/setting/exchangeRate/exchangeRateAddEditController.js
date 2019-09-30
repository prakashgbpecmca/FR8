angular.module('ngApp.exchangeRate').controller('ExchangeRateAddEditController', function ($scope, ExchangeRatetype, $uibModalInstance, ExchangeRatesCurrency, OperationZone, $filter, toaster, ExchangeRateService, $state, $location, $translate) {

    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors',
            'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation',
            'PleaseCorrectValidationErrors', 'RateCardSave_Validation', 'Select_CourierAccount', 'THP_Matrix_Saved', 'DataSaved_Validation']).then(function (translations) {

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
                $scope.THPMatrixSaved = translations.THP_Matrix_Saved;

            });
    };
    $scope.AddCurrenyToExchangeRate = function (Currency) {
            if (Currency !== null && Currency !== undefined) {
                for (var i = 0 ; i < $scope.CurrencyTypes.length; i++) {
                    if (Currency.CurrencyCode === $scope.CurrencyTypes[i].CurrencyCode) {
                        if (Currency.IsSelected) {
                            var data = {
                                OperationZoneExchangeRateId: 0,
                                OperationZone: $scope.OperationZoneData,
                                CurrencyDetail: Currency,
                                ExchangeRate: null,
                                ExchangeType: $scope.ExhangeType,
                                IsActive: true
                            };
                            $scope.NewExchangeRates.push(data);
                        }
                        else {
                            if ($scope.NewExchangeRates !== null && $scope.NewExchangeRates.length>0) {
                                for (var a = 0 ; a < $scope.NewExchangeRates.length; a++) {
                                    if (Currency.CurrencyCode === $scope.NewExchangeRates[a].CurrencyDetail.CurrencyCode) {
                                        $scope.NewExchangeRates.splice(a, 1);
                                    }
                                }
                            }
                            
                        }
                       
                    }
                }

                //$scope.data = $scope.CurrencyTypes;
            }
        

    };
    $scope.refreshData = function () {
        $scope.CurrencyTypes = $filter('filter')($scope.data, $scope.searchText, undefined);
    };

    $scope.submit = function () {
        $uibModalInstance.close($scope.NewExchangeRates);
    };
    var getInitials = function (ExchangeRatesCurrencyList) {
        ExchangeRateService.GetCurrencyDetail().then(function (response) {
            if (response.data !== null) {
                $scope.CurrencyTypes = response.data;

                if ($scope.CurrencyTypes.length > 0 && ExchangeRatesCurrencyList !== undefined && ExchangeRatesCurrencyList !== null && ExchangeRatesCurrencyList.length > 0) {
                    for (var i = 0; i < ExchangeRatesCurrencyList.length; i++) {
                        for (var j = 0 ; j < $scope.CurrencyTypes.length ; j++) {
                            $scope.CurrencyTypes[j].IsSelected = false;
                            if (ExchangeRatesCurrencyList[i].CurrencyDetail !== null && $scope.CurrencyTypes[j].CurrencyCode === ExchangeRatesCurrencyList[i].CurrencyDetail.CurrencyCode) {
                                $scope.CurrencyTypes.splice(j, 1);
                            }
                        }
                    }
                }
                $scope.data = $scope.CurrencyTypes;
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
        if (OperationZone !== undefined && OperationZone !== null) {
            $scope.OperationZoneData = OperationZone;
        }
        $scope.ExhangeType = ExchangeRatetype;
        var ExchangeRatesCurrencyList = ExchangeRatesCurrency;
        $scope.NewExchangeRates = [];
        getInitials(ExchangeRatesCurrencyList);
        setModalOptions();
    }

    init();

});