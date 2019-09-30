angular.module('ngApp.exchangeRate').controller('ExchangeRateController', function ($scope, config, AppSpinner, ModalService, ExchangeRateService, SessionService, toaster, $state, uiGridConstants, $location, $uibModal, $translate) {

    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors',
                    'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation',
                    'PleaseCorrectValidationErrors', 'RateCardSave_Validation', 'Select_CourierAccount', 'THP_Matrix_Saved', 'DataSaved_Validation',
                    'Record_Saved', 'Year', 'Month', 'Exchange_Rates_Saved', 'WarningSavingExchangeRate', 'WarningExchangeRateZeroGreaterValue',
                    'SureDeleteExchangeRate', 'Delete_Confirmation', 'LoadingExchangeRates']).then(function (translations) {
                    $scope.TitleFrayteError = translations.FrayteError;
                    $scope.TitleFrayteInformation = translations.FrayteInformation;
                    $scope.TitleFrayteValidation = translations.FrayteValidation;
                    $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
                    $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
                    $scope.RecordGettingError = translations.ErrorGetting + " " + translations.records;
                    $scope.YearGettingError = translations.ErrorGetting + " " + translations.Year;
                    $scope.MonthGettingError = translations.ErrorGetting + " " + translations.Month;
                    $scope.Success = translations.Success;
                    $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
                    $scope.UpdateRecordValidation = translations.UpdateRecord_Validation;
                    $scope.PleaseCorrectValidationErrors = translations.PleaseCorrectValidationErrors;
                    $scope.RateCardSaveValidation = translations.RateCardSave_Validation;
                    $scope.SelectCourierAccount = translations.Select_CourierAccount;
                    $scope.THPMatrixSaved = translations.THP_Matrix_Saved;
                    $scope.Exchange_Rates_Saved = translations.Exchange_Rates_Saved;
                    $scope.Warning_Saving_Exchange_Rate = translations.WarningSavingExchangeRate;
                    $scope.WarningExchangeRateZeroGreaterValue = translations.WarningExchangeRateZeroGreaterValue;
                    $scope.SureDeleteExchangeRate = translations.SureDeleteExchangeRate;
                    $scope.Delete_Confirmation = translations.Delete_Confirmation;
                    $scope.LoadingExchangeRates = translations.LoadingExchangeRates;
                     
                    // Initial Deatil call here --> Spinner Message should be multilingual
                    $scope.GetOperaionExchangeRate();
                 });
    };

    $scope.GetOperaionExchangeRate = function () {
        AppSpinner.showSpinnerTemplate($scope.LoadingExchangeRates, $scope.Template);
        ExchangeRateService.GetOperationExchangeRate($scope.OperationZone.OperationZoneId).then(function (response) {
            if (response.data !== null && response.data.length > 0) {
                $scope.OpearationExchangeRates = response.data;
                getExchangeRateYears();
                return true;
            }
            else {
                AppSpinner.hideSpinnerTemplate();
                $scope.OpearationExchangeRates = [];
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

    $scope.submit = function (isValid) {
        if (isValid) {
            if ($scope.ExchangeRates.length > 0) {
                for (var i = 0; i < $scope.ExchangeRates.length; i++) {
                    if ($scope.ExchangeRates[i].ExchangeRate > 0) {
                        $scope.NewExchangeRates.push($scope.ExchangeRates[i]);
                    }
                }
                if ($scope.NewExchangeRates.length > 0) {
                    ExchangeRateService.SaveExchangeRate($scope.NewExchangeRates).then(function () {
                        init();
                        toaster.pop({
                            type: 'success',
                            title: $scope.Success,
                            body: $scope.Exchange_Rates_Saved,
                            showCloseButton: true,
                            allowHtml: true,
                            closeHtml: '<button>Closexs</button>'
                        });

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
                    body: $scope.WarningExchangeRateZeroGreaterValue,
                    showCloseButton: true
                });
            }
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarningValidation,
                body: $scope.Warning_Saving_Exchange_Rate,
                showCloseButton: true
            });
        }
    };

    $scope.removeCourierAccount = function (ExchangeRate) {
        var modalOptions = {
            headerText: $scope.Delete_Confirmation,
            bodyText: $scope.SureDeleteExchangeRate
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            if (ExchangeRate !== null && ExchangeRate !== undefined && $scope.OpearationExchangeRates.length > 0) {
                for (var i = 0 ; i < $scope.OpearationExchangeRates.length; i++) {
                    if (ExchangeRate.OperationZoneExchangeRateId === $scope.OpearationExchangeRates[i].OperationZoneExchangeRateId && ExchangeRate.CurrencyDetail.CurrencyCode === $scope.OpearationExchangeRates[i].CurrencyDetail.CurrencyCode) {
                        $scope.OpearationExchangeRates.splice(i, 1);
                    }
                }
                if ($scope.ExchangeRates.length > 0) {
                    var flag1 = false;
                    for (var j = 0 ; j < $scope.ExchangeRates.length ; j++) {
                        if (ExchangeRate.OperationZoneExchangeRateId === $scope.ExchangeRates[j].OperationZoneExchangeRateId && ExchangeRate.CurrencyDetail.CurrencyCode === $scope.ExchangeRates[j].CurrencyDetail.CurrencyCode) {
                            $scope.ExchangeRates[j].IsActive = false;
                            flag1 = true;
                        }
                    }
                    if (!flag1) {
                        ExchangeRate.IsActive = false;
                        $scope.ExchangeRates.push(ExchangeRate);
                    }
                }
                else {
                    ExchangeRate.IsActive = false;
                    $scope.ExchangeRates.push(ExchangeRate);
                }

            }
        });
    };

    $scope.removeExchangeRate = function (ExchangeRate) {
        if (ExchangeRate !== null && ExchangeRate !== undefined && $scope.OpearationExchangeRates.length > 0) {
            for (var i = 0 ; i < $scope.OpearationExchangeRates.length; i++) {
                if (ExchangeRate.OperationZoneExchangeRateId === $scope.OpearationExchangeRates[i].OperationZoneExchangeRateId && ExchangeRate.CurrencyDetail.CurrencyCode === $scope.OpearationExchangeRates[i].CurrencyDetail.CurrencyCode) {
                    $scope.OpearationExchangeRates.splice(i, 1);
                }
            }
            if ($scope.ExchangeRates.length > 0) {
                var flag1 = false;
                for (var j = 0 ; j < $scope.ExchangeRates.length ; j++) {
                    if (ExchangeRate.OperationZoneExchangeRateId === $scope.ExchangeRates[j].OperationZoneExchangeRateId && ExchangeRate.CurrencyDetail.CurrencyCode === $scope.ExchangeRates[j].CurrencyDetail.CurrencyCode) {
                        $scope.ExchangeRates[j].IsActive = false;
                        flag1 = true;
                    }
                }
                if (!flag1) {
                    ExchangeRate.IsActive = false;
                    $scope.ExchangeRates.push(ExchangeRate);
                }
            }
            else {
                ExchangeRate.IsActive = false;
                $scope.ExchangeRates.push(ExchangeRate);
            }
        }
    };

    $scope.setHKDCurrency = function (ExchangeRate) {

        if (ExchangeRate.CurrencyDetail.CurrencyCode === 'HKD' && ExchangeRate.ExchangeType === 'Sell' && (ExchangeRate.ExchangeRate !== null || ExchangeRate.ExchangeRate !== undefined)) {
            ExchangeRate.ExchangeRate = "1";
        }
    };

    $scope.setExchangeRate = function (ExchangeRate) {
        if (ExchangeRate !== undefined && ExchangeRate.ExchangeRate !== null && ExchangeRate.ExchangeRate > 0) {
            if ($scope.ExchangeRates.length > 0) {
                var flag = false;
                for (var j = 0 ; j < $scope.ExchangeRates.length ; j++) {
                    if (ExchangeRate.OperationZoneExchangeRateId === $scope.ExchangeRates[j].OperationZoneExchangeRateId && ExchangeRate.CurrencyDetail.CurrencyCode === $scope.ExchangeRates[j].CurrencyDetail.CurrencyCode) {
                        $scope.ExchangeRates.splice(j, 1);
                        $scope.ExchangeRates.push(ExchangeRate);
                        flag = true;
                    }
                }
                if (!flag) {
                    $scope.ExchangeRates.push(ExchangeRate);
                }
            }
            else {
                for (var i = 0 ; i < $scope.OpearationExchangeRates.length ; i++) {
                    if (ExchangeRate.OperationZoneExchangeRateId === $scope.OpearationExchangeRates[i].OperationZoneExchangeRateId && ExchangeRate.CurrencyDetail.CurrencyCode === $scope.OpearationExchangeRates[i].CurrencyDetail.CurrencyCode) {
                        $scope.ExchangeRates.push(ExchangeRate);
                        break;
                    }
                }
            }
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarningValidation,
                body: $scope.WarningExchangeRateZeroGreaterValue,
                showCloseButton: true
            });
        }
    };

    $scope.AddEditExchangeRate = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'setting/exchangeRate/exchangeRateAddEdit.tpl.html',
            controller: 'ExchangeRateAddEditController',
            windowClass: 'AddEditCourier',
            size: 'md',
            backdrop: 'static',
            resolve: {
                ExchangeRatesCurrency: function () {
                    return $scope.OpearationExchangeRates;
                },
                OperationZone: function () {
                    return $scope.OperationZone;
                },
                ExchangeRatetype: function () {
                    return $scope.OperationExchangeType;
                }
            }
        });
        modalInstance.result.then(function (NewExchangeRates) {
            if (NewExchangeRates !== undefined && NewExchangeRates !== null && NewExchangeRates.length > 0) {
                for (var i = 0 ; i < NewExchangeRates.length; i++) {
                    $scope.OpearationExchangeRates.push(NewExchangeRates[i]);
                }
                $scope.submitted = true;
            }
        }, function () {
        });
    };

    var getExchangeRateYears = function () {
        ExchangeRateService.GetExchangeYears($scope.OperationZone.OperationZoneId, $scope.OperationExchangeType).then(function (response) {
            $scope.Years = response.data;
            if ($scope.Years !== null) {
                $scope.CurrentYear = $scope.Years[0];
                getMonthsExchageRate();
            }
            else {
                AppSpinner.hideSpinnerTemplate();
            }

        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.YearGettingError,
                showCloseButton: true
            });
        });
    };

    var getMonthsExchageRate = function () {
        ExchangeRateService.GetExchangeMonth($scope.OperationZone.OperationZoneId, $scope.OperationExchangeType).then(function (response) {
            $scope.Months = response.data;
            exchangeRateshistoryData();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.MonthGettingError,
                showCloseButton: true
            });
        });
    };

    var exchangeRateshistoryData = function () {
        $scope.search = {
            OperationZoneId: $scope.OperationZone.OperationZoneId,
            ExchangeType: $scope.OperationExchangeType,
            Year: $scope.CurrentYear,
            MonthName: $scope.CurrentMonth
        };
        ExchangeRateService.GetOperationExchangeRateHistory($scope.search).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            if (response.data !== null && response.data.length > 0) {
                for (i = 0; i < response.data.length; i++) {
                    var Sdate = new Date(response.data[i].StartDate);
                    var getmn = Sdate.getMonth();
                    var getmn1 = ++getmn;
                    var getday = Sdate.getDate();

                    var getyr = Sdate.getFullYear();
                    response.data[i].StartDate = getday + "/" + getmn1 + "/" + getyr;
                }
                for (j = 0; j < response.data.length; j++) {
                    var Fdate = new Date(response.data[j].FinishDate);
                    var getmn2 = Fdate.getMonth();
                    var getmn3 = ++getmn2;
                    var getday1 = Fdate.getDate();

                    var getyr3 = Fdate.getFullYear();
                    response.data[j].FinishDate = getday1 + "/" + getmn3 + "/" + getyr3;
                }
                $scope.gridOptions.data = response.data;
                $scope.showGridHistory = true;
            }
            else {
                $scope.showGridHistory = false;
                $scope.OpearationExchangeRatesHistory = [];
            }

        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });
    };

    $scope.SetGridOptions = function () {
        $scope.gridOptions = {
            showFooter: true,
            enableSorting: true,
            multiSelect: false,
            enableFiltering: true,
            enableRowSelection: true,
            enableSelectAll: false,
            enableRowHeaderSelection: false,
            selectionRowHeaderWidth: 35,
            noUnselect: true,
            enableGridMenu: false,
            enableColumnMenus: false,
            enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
            enableVerticalScrollbar: true,
            columnDefs: [
              {
                  name: 'OperationZone.OperationZoneName', displayName: 'Business_Unit', headerCellFilter: 'translate', width: '20%', sort: {
                      direction: uiGridConstants.ASC,
                      priority: 0
                  }
              },
              { name: 'CurrencyDetail.CurrencyCode', displayName: 'Currency', headerCellFilter: 'translate', width: '20%' },
              { name: 'ExchangeRate', displayName: 'Exchange_Rate', headerCellFilter: 'translate', width: '20%' },
              { name: 'StartDate', displayName: 'Start_Date', headerCellFilter: 'translate', width: '20%' },
              { name: 'FinishDate', displayName: 'End_Date', headerCellFilter: 'translate', width: '20%' }
            ]
        };
    };

    $scope.GetOperaionExchangeRateByYearMonth = function () {
        AppSpinner.showSpinnerTemplate($scope.spinnerMessage, $scope.Template);
        exchangeRateshistoryData();
        AppSpinner.hideSpinnerTemplate();
    };

    //var initialDetail = function () {
    //    ExchangeRateService.GetOperationZone().then(function (response) {
    //        if (response.data !== null) {
    //            $scope.OperationZones = response.data;
    //            if (config.SITE_COUNTRY === "COM") {
    //                $scope.OperationZone = $scope.OperationZones[0];
    //            }
    //            else {
    //                $scope.OperationZone = $scope.OperationZones[1];
    //            }
    //            $scope.GetOperaionExchangeRate();
    //        }
    //    }, function (response) {
    //        if (response.status !== 401) {
    //            toaster.pop({
    //                type: 'error',
    //                title: $scope.TitleFrayteError,
    //                body: $scope.RecordGettingError,
    //                showCloseButton: true
    //            });
    //        }
    //    });
    //};

    function init() {
        
        var userInfo = SessionService.getUser();
        if (userInfo !== undefined && userInfo !== null) {
            $scope.OperationZone = {
                OperationZoneId: userInfo.OperationZoneId,
                OperationZoneName: userInfo.OperationZoneName
            };
        }
        
        $scope.Template = 'directBooking/ajaxLoader.tpl.html'; 
        setModalOptions();

        $scope.isActive = true;
        var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
        var d = new Date();
        $scope.CurrentMonth = {
            MonthId: d.getMonth() + 1,
            MonthName: monthNames[d.getMonth()]
        };

        $scope.CurrentYear = d.getFullYear();
        $scope.gridheight = SessionService.getScreenHeight();
        $scope.SetGridOptions();
        $scope.submitted = false;
        $scope.OperationExchangeType = 'sell';        

        $scope.ExchangeRates = [];
        $scope.NewExchangeRates = [];

        getExchangeRateYears(); 
    }

    init();
});