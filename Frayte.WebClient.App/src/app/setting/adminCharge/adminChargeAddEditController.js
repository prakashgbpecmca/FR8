angular.module('ngApp.adminCharge').controller('CustomerAdminChargeAddEditController', function ($scope, DirectBookingService, $uibModalInstance, config, AppSpinner, ModalService, CustomerAdminCharge, SessionService, toaster, $state, AdminChages, SettingService, Mode, $translate, ScreenType, AdminCharge) {


    //Set Multilingual for Modal Popup
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'FrayteSuccess', 'ErrorGettingRecordPleaseTryAgain', 'SuccessfullyCreatedAdminCharges',
        'ErrorSavingDataPleaseTryAgain', 'CorrectValidationErrorFirst', 'SavingCustomerAdminCharge', 'CreatingAdminCharges', 'SavingAdminCharges']).then(function (translations) {

            $scope.FrayteError = translations.FrayteError;
            $scope.FrayteInformation = translations.FrayteInformation;
            $scope.FrayteValidation = translations.FrayteValidation;
            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.ErrorGettingRecordPleaseTryAgain = translations.ErrorGettingRecordPleaseTryAgain;
            $scope.SuccessfullyCreatedAdminCharges = translations.SuccessfullyCreatedAdminCharges;
            $scope.ErrorSavingDataPleaseTryAgain = translations.ErrorSavingDataPleaseTryAgain;
            $scope.CorrectValidationErrorFirst = translations.CorrectValidationErrorFirst;
            $scope.SavingCustomerAdminCharge = translations.SavingCustomerAdminCharge;
            $scope.CreatingAdminCharges = translations.CreatingAdminCharges;
            $scope.SavingAdminCharges = translations.SavingAdminCharges;
        });
    };



    var getCustomers = function () {
        AppSpinner.showSpinnerTemplate("", $scope.Template);
        SettingService.getCustomersWithoutCharges($scope.userInfo.EmployeeId, "eCommerce", $scope.mode).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            $scope.directBookingCustomers = response.data;
            if ($scope.directBookingCustomers && $scope.directBookingCustomers.length) {
                for (i = 0; i < $scope.directBookingCustomers.length; i++) {

                    var dbr = $scope.directBookingCustomers[i].AccountNumber.split("");
                    var accno = "";
                    for (var j = 0; j < dbr.length; j++) {
                        accno = accno + dbr[j];
                        if (j == 2 || j == 5) {
                            accno = accno + "-";
                        }
                    }
                    $scope.directBookingCustomers[i].AccountNumber = accno;
                }
            }

            if ($scope.mode === 'Edit') {
                for (var i = 0 ; i < $scope.directBookingCustomers.length ; i++) {
                    if ($scope.directBookingCustomers[i].CustomerId === $scope.customerAdminCharge.CustomerId) {
                        $scope.customerDetail = $scope.directBookingCustomers[i];
                        break;
                    }
                }
            }
        }, function () {
            AppSpinner.hideSpinnerTemplate();
        });
    };

    $scope.addEditCustomerAdminCharge = function () {
        AppSpinner.showSpinnerTemplate($scope.SavingCustomerAdminCharge, $scope.Template);
        SettingService.addEditCustomerAdminCharge($scope.customerAdminCharge).then(function (response) {
            if (response.data && response.data.Status) {
                toaster.pop({
                    type: 'success',
                    title: $scope.FrayteSuccess,
                    body: $scope.SuccessfullyCreatedAdminCharges,
                    showCloseButton: true
                });
            }
            else {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.ErrorSavingDataPleaseTryAgain,
                    showCloseButton: true
                });
            }

            $uibModalInstance.close();
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ErrorSavingDataPleaseTryAgain,
                showCloseButton: true
            });
        });
    };



    $scope.addCustomerSpecificAdminCharge = function (isValid) {
        if (isValid && $scope.customerAdminCharge.CustomerId) {
            AppSpinner.showSpinnerTemplate($scope.CreatingAdminCharges, $scope.Template);
            SettingService.saveCustomerCharge($scope.customerAdminCharge).then(function (response) {
                if (response.data && response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.SuccessfullyCreatedAdminCharges,
                        showCloseButton: true
                    });

                    // need to close the pop-up
                    $uibModalInstance.close();
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteError,
                        body: $scope.ErrorSavingDataPleaseTryAgain,
                        showCloseButton: true
                    });
                }
                AppSpinner.hideSpinnerTemplate();
            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.ErrorSavingDataPleaseTryAgain,
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteValidation,
                body: $scope.CorrectValidationErrorFirst,
                showCloseButton: true
            });
        }
    };

    $scope.setCustomer = function (CustomerId) {
        $scope.customerAdminCharge.CustomerId = CustomerId;
    };


    $scope.addAdminCharge = function (isValid) {
        if (isValid) {
            var arr = [];
            arr.push($scope.adminCharge);
            AppSpinner.showSpinnerTemplate($scope.SavingAdminCharges, $scope.Template);
            SettingService.addAdminCharge(arr).then(function (response) {
                if (response.data != null) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.SuccessfullyCreatedAdminCharges,
                        showCloseButton: true
                    });

                    $uibModalInstance.close();
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteError,
                        body: $scope.ErrorSavingDataPleaseTryAgain,
                        showCloseButton: true
                    });
                }
                AppSpinner.hideSpinnerTemplate();
            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.ErrorSavingDataPleaseTryAgain,
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteValidation,
                body: $scope.CorrectValidationErrorFirst,
                showCloseButton: true
            });
        }

    };
    function init() {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.submitted = false;
        $scope.userInfo = SessionService.getUser();
        $scope.customerAdminCharge = CustomerAdminCharge;
        setMultilingualOptions();
        if (Mode) {
            $scope.mode = Mode;

        }
        if (ScreenType) {
            $scope.screenType = ScreenType;
        }
        if (AdminCharge) {
            $scope.adminCharge = AdminCharge;
        }
        if ($scope.customerAdminCharge.CustomerId) {

        }
        else {
            $scope.customerAdminCharge.Charges = AdminChages;
            if ($scope.customerAdminCharge.Charges && $scope.customerAdminCharge.Charges.length) {
                for (var i = 0 ; i < $scope.customerAdminCharge.Charges.length; i++) {
                    $scope.customerAdminCharge.Charges[i].AdminChargeId = 0;
                }
            }
        }
        getCustomers();
    }

    init();
});