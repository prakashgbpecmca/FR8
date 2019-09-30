angular.module('ngApp.fuelSurCharge').controller('FuelSurChargeController', function ($scope, config, $state, uiGridConstants, $filter, FuelService, TopCountryService, TopCurrencyService, SessionService, $uibModal, toaster, $translate) {

    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors',
            'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation',
            'PleaseCorrectValidationErrors', 'RateCardSave_Validation', 'Select_CourierAccount', 'THP_Matrix_Saved', 'DataSaved_Validation',
            'Record_Saved', 'Year', 'Month', 'Fuel_Surcharge_Save_Successfully']).then(function (translations) {
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
                $scope.Fuel_Surcharge_Save_Successfully = translations.Fuel_Surcharge_Save_Successfully;
            });
    };

    $scope.fuelDetail = {
        OperationZoneId: 0,
        Year: new Date(),
        UserId: $scope.UserId,
        LogisticCompany: 0
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
        $scope.fuelDetail.Year = new Date();
        $scope.fuelDetail.LogisticCompany = $scope.LogisticCompany;
        FuelService.fuelService($scope.fuelDetail).then(function (response) {
            if (response.status = 200) {
                toaster.pop({
                    type: 'success',
                    title: $scope.Success,
                    body: $scope.Fuel_Surcharge_Save_Successfully,
                    showCloseButton: true
                });

                $scope.getFuelService();
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
            for (var i = 0; i < $scope.FuelSurCharges.length; i++) {
                $scope.FuelSurCharges[i].UpdatedBy = $scope.UserId;
            }

            FuelService.UpdateFuelSurCharge($scope.FuelSurCharges).then(function (response) {
                if (response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.Success,
                        body: $scope.Fuel_Surcharge_Save_Successfully,
                        showCloseButton: true
                    });
                    $scope.getFuelService();
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorSavingRecord,
                        showCloseButton: true
                    });
                    $scope.getFuelService();
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
            var month1 = month.getMonth() + 1;
            var m = 0;
            $scope.monthandyear = [];

            for (var k = 0; k < $scope.NewMonthYear.length; k++) {
                if ($scope.Year === $scope.NewMonthYear[k].Year) {
                    $scope.monthandyear.push($scope.NewMonthYear[k]);
                }
            }

            $scope.monthyear = angular.copy($scope.monthandyear);

            for (var i = 0; i < $scope.FuelSurCharges.length; i++) {
                for (var j = 0; j < $scope.FuelSurCharges[i].Type.length; j++) {
                    var mmvalue = 0;
                    for (var l = 0; l < $scope.FuelSurCharges[i].Type[j].MonthYear.length; l++) {
                        if ($scope.Year === $scope.monthyear[l].Year) {
                            if ($scope.FuelSurCharges[i].Type[j].MonthYear[l].Month === $scope.monthyear[l].Month) {
                                $scope.monthandyear[l].Month = $scope.MonthName[l];
                                if (m === 0) {
                                    $scope.monthandyear[l].Year = $scope.monthandyear[l].Year.toString().substr(2, 2);
                                }

                                //TO Do get month 
                                if ($scope.FuelSurCharges[i].Type[j].MonthYear[l].Year === date.getFullYear()) {
                                    $scope.gettingmonth = $scope.FuelSurCharges[i].Type[j].MonthYear[l].Month;
                                    if (mmvalue === 0 || mmvalue > 1) {
                                        $scope.FuelSurCharges[i].Type[j].MonthYear[l].IsDisable = true;
                                    }
                                    if (mmvalue === 1) {
                                        $scope.FuelSurCharges[i].Type[j].MonthYear[l].IsDisable = false;
                                        mmvalue++;
                                    }
                                    if ($scope.gettingmonth === month1) {
                                        $scope.FuelSurCharges[i].Type[j].MonthYear[l].IsDisable = false;
                                        mmvalue++;
                                    }
                                }
                            }
                        }
                    }
                    m++;
                }
            }
        }
    };

    var getMonthYear = function () {
        FuelService.GetfuelMonthYear($scope.OperationZone.OperationZoneId).then(function (response) {
            if (response.data.length > 0) {
                $scope.MonthYear = response.data;
                $scope.NewMonthYear = angular.copy(response.data);
                //$scope.NewMonthYear.Row.push(response.data);
            }
            else {

            }
        });
    };

    $scope.getFuelService = function () {

        getMonthYear();

        FuelService.GetfuelService($scope.OperationZone.OperationZoneId, $scope.Year).then(function (response) {
            if (response.status = 200) {
                $scope.FuelSurCharges = response.data;
                if (response.data !== null && response.data.length > 0) {

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

    };

    $scope.fuelSurChargeByOperationZone = function (OperationZone) {
        $scope.OperationZone.OperationZoneId = OperationZone.OperationZoneId;
        getLogisticCompanies();
        setDomesticFuel();
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
            if ($scope.Years.length && $scope.LogisticCompanies.length > 0) {
                var date = new Date();
                for (var i = 0 ; i < $scope.Years.length; i++) {
                    if ($scope.Years[i] === date.getFullYear()) {
                        $scope.Year = $scope.Years[i];
                    }
                }
                if ($scope.Year === undefined || $scope.Year === null) {
                    $scope.Year = $scope.Years[0];
                }
                $scope.getFuelService();
            }
            else {

            }
        }, function () {

        });
    };

    var getLogisticCompanies = function () {
        FuelService.GetLogisticCompanies($scope.OperationZone.OperationZoneId).then(function (response) {
            $scope.LogisticCompanies = response.data;
            $scope.LogisticCompany = response.data[0].LogisticCompany;
            if (response.data.length > 0) {
                getFuelYearList();
            }
        },
        function () {

        });
    };

    function init() {

        $scope.UserId = 0;

        var userInfo = SessionService.getUser();
        if (userInfo !== undefined && userInfo !== null) {
            $scope.OperationZone = {
                OperationZoneId: userInfo.OperationZoneId,
                OperationZoneName: userInfo.OperationZoneName
            };
            getLogisticCompanies();
            setDomesticFuel();
            $scope.UserId = userInfo.EmployeeId;
        }

        $scope.MonthName = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

        FuelService.GetOperationZone().then(function (response) {

            if (response.status = 200) {
                $scope.OperationZones = response.data;
            }
        }, function (response) {
            if (response.status !== 401) {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.RecordGettingError,
                    showCloseButton: true
                });
            }
        });

        setModalOptions();
    }

    init();

});