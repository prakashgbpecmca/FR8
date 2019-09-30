angular.module('ngApp.adminCharge').controller('AdminChargesController', function ($scope, config, SettingService, AppSpinner, ModalService, ExchangeRateService, SessionService, toaster, $state, uiGridConstants, $location, $uibModal, $translate) {
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors',
                    'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation',
                    'PleaseCorrectValidationErrors', 'RateCardSave_Validation', 'Select_CourierAccount', 'THP_Matrix_Saved', 'DataSaved_Validation',
                    'Record_Saved', 'Year', 'Month', 'Exchange_Rates_Saved', 'WarningSavingExchangeRate', 'WarningExchangeRateZeroGreaterValue', 'Confirmation', 'FrayteSuccess',
        'SureRemoveIt', 'SuccessfullyRemoveCustomerCharges', 'ErrorDeletingPleaseTryAgain', 'SuccessfullyDeletedAdminCharges', 'ErrorSavingAdminCharge',
        'RemovingCustomerAdminCharges', 'LoadingAdminCharges', 'DeletingAdminCharges']).then(function (translations) {
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
            $scope.Warning_Exchange_Rate_Zero_Greater_Value = translations.WarningExchangeRateZeroGreaterValue;
            $scope.Confirmation = translations.Confirmation;
            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.SureRemoveIt = translations.SureRemoveIt;
            $scope.SuccessfullyRemoveCustomerCharges = translations.SuccessfullyRemoveCustomerCharges;
            $scope.ErrorDeletingPleaseTryAgain = translations.ErrorDeletingPleaseTryAgain;
            $scope.SuccessfullyDeletedAdminCharges = translations.SuccessfullyDeletedAdminCharges;
            $scope.ErrorSavingAdminCharge = translations.ErrorSavingAdminCharge;
            $scope.RemovingCustomerAdminCharge= translations.RemovingCustomerAdminCharge;
            $scope.LoadingAdminCharges = translations.LoadingAdminCharges;
            $scope.DeletingAdminCharges = translations.DeletingAdminCharges;
        });
    };


    $scope.removeCustomerAdminCharges = function (customerAdminCharges) {

        var modalOptions = {
            headerText: $scope.Confirmation,
            bodyText: $scope.SureRemoveIt
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            AppSpinner.showSpinnerTemplate($scope.RemovingCustomerAdminCharges, $scope.Template);
            SettingService.removeCustomerAdminCharges(customerAdminCharges.CustomerId, $scope.createdBy).then(function (response) {
                if (response.data && response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.SuccessfullyRemoveCustomerCharges,
                        showCloseButton: true
                    });
                    getCustomerSpecificAdminCharges();
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.ErrorDeletingPleaseTryAgain,
                        showCloseButton: true
                    });
                }
                AppSpinner.hideSpinnerTemplate();
            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.ErrorDeletingPleaseTryAgain,
                    showCloseButton: true
                });
            });
        }, function () {

        });
    };

    $scope.addEditCustomerAdmnCharge = function (ScreenType, AdminCharge, customerAdminCharges, Mode) {

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'setting/adminCharge/adminChargeAddEdit.tpl.html',
            controller: 'CustomerAdminChargeAddEditController',
            windowClass: '',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                CustomerAdminCharge: function () {
                    if (customerAdminCharges) {
                        return customerAdminCharges;
                    }
                    else {
                        return {
                            CustomerId: 0,
                            CustomerName: '',
                            Charges: []
                        };
                    }
                },
                AdminChages: function () {
                    var adminChargesCopy = angular.copy($scope.adminCharges);
                    return adminChargesCopy;
                },
                Mode: function () {
                    if (Mode) {
                        return Mode;
                    } else {
                        return "Add";
                    }
                },
                ScreenType: function () {
                    return ScreenType;
                },
                AdminCharge: function () {
                    if (AdminCharge) {
                        return AdminCharge;
                    }
                    else {
                        return {
                            AdminChargeId: 0,
                            CreatedBy: $scope.createdBy,
                            ChargeType: "Fixed",
                            Key: '',
                            Value: '',
                            Amount: '',
                            CreatedOn: new Date(),
                            CurrencyCode: 'GBP'
                        };
                    }
                }


            }
        });
        modalInstance.result.then(function () {
            getScreenInitials();
        }, function () {
            getScreenInitials();
        });

    };
    $scope.showHideCustomerAdmin = function () {
        if ($scope.adminCharges && $scope.adminCharges.length) {
            var flag = false;
            for (var i = 0 ; i < $scope.adminCharges.length ; i++) {
                if ($scope.adminCharges[i].AdminChargeId) {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        return false;
    };

    var getCustomerSpecificAdminCharges = function () {
        AppSpinner.showSpinnerTemplate($scope.LoadingAdminCharges, $scope.Template);
        SettingService.getCustomerSpecificAdminCharges().then(function (response) {
            if (response.data != null) {
                $scope.customerAdminCharges = response.data;
            }
            else {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.RecordGettingError,
                    showCloseButton: true
                });
            }
            AppSpinner.hideSpinnerTemplate();
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
    var getScreenInitials = function () {
        AppSpinner.showSpinnerTemplate($scope.LoadingAdminCharges, $scope.Template);
        SettingService.getAdminCharges($scope.createdBy).then(function (response) {
            if (response.data != null) {
                $scope.adminCharges = response.data;

                getCustomerSpecificAdminCharges();
            }
            else {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.RecordGettingError,
                    showCloseButton: true
                });
            }
            AppSpinner.hideSpinnerTemplate();
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

    var removeChangeFromAdminCharge = function (charge) {
        angular.forEach($scope.adminCharges, function (obj, key) {
            var fg = obj;
            var f = key;
            if (obj.AdminChargeId === charge.AdminChargeId) {
                $scope.adminCharges.splice(key, 1);
            }
        });
    };

    $scope.removeCharge = function (charge) {
        if (charge) {
            if (charge.AdminChargeId) {
                AppSpinner.showSpinnerTemplate($scope.DeletingAdminCharges, $scope.Template);
                SettingService.deleteAdminCharge(charge.AdminChargeId).then(function (response) {
                    if (response.data && response.data.Status) {
                        toaster.pop({
                            type: 'success',
                            title: $scope.Success,
                            body: $scope.SuccessfullyDeletedAdminCharges,
                            showCloseButton: true
                        });
                        removeChangeFromAdminCharge(charge);
                    }
                    else {
                        toaster.pop({
                            type: 'error',
                            title: $scope.TitleFrayteError,
                            body: $scope.ErrorSavingAdminCharge,
                            showCloseButton: true
                        });
                    }
                    AppSpinner.hideSpinnerTemplate();
                }, function () {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteErro,
                        body: $scope.ErrorSavingAdminCharge,
                        showCloseButton: true
                    });
                });
            }
            else {
                removeChangeFromAdminCharge(charge);
            }

        }
    };

    function init() {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.submitted = true;
        $scope.adminCharges = [];

        var userInfo = SessionService.getUser();
        if (userInfo) {
            $scope.createdBy = userInfo.EmployeeId;
            getScreenInitials();
            setModalOptions();
        }
        else {
        }
    }

    init();
});