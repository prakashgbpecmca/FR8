angular.module('ngApp.fuelSurCharge').controller('FuelSurChargeController', function ($scope, $state, uiGridConstants, config, $filter, FuelService, TopCountryService, TopCurrencyService, SessionService, $uibModal, toaster, $translate) {

    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors',
            'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation',
            'PleaseCorrectValidationErrors', 'RateCardSave_Validation', 'Select_CourierAccount', 'THP_Matrix_Saved', 'DataSaved_Validation',
        'Record_Saved', 'Year', 'Month']).then(function (translations) {

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
            $scope.RecordSaved = translations.Record_Saved;

        });
    };


    $scope.fuelDetail = {

        OperationZoneId: 0,
        Year: new Date(),
        UserId: $scope.UserId

    };

    $scope.getFuelMonthYear = function (FuelMonthYear) {
        if (FuelMonthYear !== undefined && FuelMonthYear !== null) {

            var date = new Date(FuelMonthYear);
            var days = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
            var getmn1 = days[date.getMonth()];
            var getyr = date.getFullYear();
            var strDate = getmn1 + " " + getyr;
            return strDate;
        }
    };

    $scope.GenerateFuelSurCharge = function () {
        $scope.val1 = true;
        $scope.fuelDetail.OperationZoneId = $scope.OperationZone.OperationZoneId;
        $scope.fuelDetail.Year = $scope.Year.YearDate;
        FuelService.fuelService($scope.fuelDetail).then(function (response) {
            if (response.status = 200) {
                toaster.pop({
                    type: 'success',
                    title: $scope.Success,
                    body: $scope.RecordSaved,
                    showCloseButton: true
                });

                getFuelService();
            }
        }, function (response) {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorSavingRecord,
                showCloseButton: true
            });
        });


    };


    $scope.updateFuelSurCharge = function (IsValid) {
        if (IsValid) {
            FuelService.UpdateFuelSurCharge($scope.FuelSurCharges).then(function (response) {

                if (response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.Success,
                        body: $scope.RecordSaved,
                        showCloseButton: true
                    });
                    getFuelService();
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorSavingRecord,
                        showCloseButton: true
                    });
                    getFuelService();
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

    var setFuelChargeJson = function () {

        if ($scope.FuelSurCharges !== undefined && $scope.FuelSurCharges !== null && $scope.FuelSurCharges.length > 0) {

            // To Do: get current month from Date 
            var month = new Date();
            var date = new Date();
            var month1 = month.getMonth() - 2;

            for (var i = 0; i < $scope.FuelSurCharges.length ; i++) {
                // TO Do get month 
                if (new Date($scope.FuelSurCharges[i].FuelMonthYear).getFullYear() === date.getFullYear()) {
                    $scope.gettingmonth = new Date($scope.FuelSurCharges[i].FuelMonthYear).getMonth();
                    if ($scope.gettingmonth < month1) {
                        $scope.FuelSurCharges[i].IsDisable = true;
                    }
                    else {
                        $scope.FuelSurCharges[i].IsDisable = false;
                    }
                }
                else if (new Date($scope.FuelSurCharges[i].FuelMonthYear).getFullYear() < date.getFullYear()) {
                    $scope.FuelSurCharges[i].IsDisable = true;
                }
                else {
                    $scope.FuelSurCharges[i].IsDisable = false;
                }

            }
        }
    };

    var getFuelService = function () {
        FuelService.GetfuelService($scope.OperationZone.OperationZoneId, $scope.Year).then(function (response) {

            if (response.status = 200) {
                if (response.data !== null && response.data.length > 0) {

                    $scope.FuelSurCharges = response.data;
                    setFuelChargeJson();

                    $scope.val = true;
                }
                else {
                    $scope.val = false;
                }

            }
            else {
                toaster.pop({
                    type: 'success',
                    title: $scope.Success,
                    body: $scope.RecordGettingError,
                    showCloseButton: true
                });
            }

        });
    };
    $scope.myfunction = function (Year) {
        
        getFuelService();

    };
    $scope.fuelSurChargeByOperationZone = function () {


        setDomesticFuel();
        getFuelService();
    };

    $scope.setFuelLength = function (FuelSurCharge, packageForm, FuelPercent) {
        if (FuelSurCharge !== undefined && packageForm !== undefined && FuelPercent !== undefined && FuelPercent.length) {
            if (parseFloat(FuelPercent) > 99) {
                packageForm.$invalid = true;
            }
            else {
                packageForm.$invalid = false;
            }
        }
    };

    var setDomesticFuel = function () {
        if ($scope.OperationZone.OperationZoneId === 2) {
            $scope.domesticFuelShow = false;
        }
        else {
            $scope.domesticFuelShow = true;
        }
    };

    var getFuelYearList = function () {
        FuelService.GetDistinctFuelSurchargeYear().then(function (response) {
            $scope.Years = response.data;
            if ($scope.Years.length) {
                var date = new Date();
                for (var i = 0 ;i< $scope.Years.length; i++){
                    if ($scope.Years[i] === date.getFullYear()) {
                        $scope.Year = $scope.Years[i];
                    }
                }
                if ($scope.Year === undefined || $scope.Year === null) {
                    $scope.Year = $scope.Years[0];
                }
                getFuelService();
            }
            else {

            }
        }, function () {

        });
    };
    function init() {

        $scope.UserId = 0;

        //$scope.FuelSurCharges = [];
        var userInfo = SessionService.getUser();
        if (userInfo !== undefined && userInfo !== null) {
            $scope.OperationZone = {
                OperationZoneId: userInfo.OperationZoneId,
                OperationZoneName: userInfo.OperationZoneName
            };
            setDomesticFuel();

            $scope.UserId = userInfo.EmployeeId;
        }

        FuelService.GetOperationZone().then(function (response) {

            if (response.status = 200) {

                $scope.OperationZones = response.data;
                 
                getFuelYearList();
            }


        }, function (response) {
            alert('An error occurred');
        });
        setModalOptions();
    }
    init();

});