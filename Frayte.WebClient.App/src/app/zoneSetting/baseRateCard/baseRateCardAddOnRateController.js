angular.module('ngApp.baseRateCard').controller('baseRateCardAddOnRateController', function ($scope, $uibModalInstance, AppSpinner, $http, $filter, $state, toaster, $translate, $uibModal, ZoneBaseRateCardService, $window, OperationZoneId, LogisticType, CourierCompany, RateType, ParcelType, PackageType, ModuleType) {

    ZoneBaseRateCardService.GetAddOnRate(OperationZoneId, LogisticType, CourierCompany, RateType, ParcelType, PackageType, ModuleType).then(function (response) {
        if (LogisticType === 'UKShipment') {
            if (CourierCompany === 'DHL') {
                $scope.UKdhladdonRate = response.data;
                ukdhladdonrate();
            }
            else {
                $scope.UKaddonRate = response.data;
                ukaddonrate();
            }
        }
        else {
            $scope.AddOnRateList = response.data;
            addonrate();
        }
    });

    var setModalOptions = function () {
        $translate(['SuccessfullyUpdate', 'Update_Add_On_Rate', 'FrayteSuccess', 'FrayteValidation', 'FrayteError', 'FrayteWarning', 'ErrorSavingRecord',
            'Add_On_Rate_Not_Update', 'LoadingAddOnRateCard']).then(function (translations) {
            $scope.SuccessfullyUpdate = translations.SuccessfullyUpdate;
            $scope.UpdateAddOnRateSucess = translations.Update_Add_On_Rate;
            $scope.TitleFrayteInformation = translations.FrayteSuccess;
            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.Frayte_Warning = translations.FrayteWarning;
            $scope.TextSavingError = translations.ErrorSavingRecord;
            $scope.AddOnNotUpdate = translations.Add_On_Rate_Not_Update;
            $scope.LoadingAddOnRateCard = translations.LoadingAddOnRateCard;
        });
    };

    var getzones = function () {
        if (LogisticType === 'UKShipment') {
            if (CourierCompany === 'DHL') {
                ZoneBaseRateCardService.GetZones(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType).then(function (response) {
                    $scope.Zones = response.data;
                }, function () {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.RecordGettingError,
                        showCloseButton: true
                    });
                });
            }
            else {
                ZoneBaseRateCardService.GetZones(OperationZoneId, '', CourierCompany, '', ModuleType).then(function (response) {
                    $scope.Zones = response.data;
                }, function () {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.RecordGettingError,
                        showCloseButton: true
                    });
                });
            }
        }
        else {
            ZoneBaseRateCardService.GetZones(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType).then(function (response) {
                $scope.Zones = response.data;
            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.RecordGettingError,
                    showCloseButton: true
                });
            });
        }
    };

    var addonrate = function () {

        $scope.finalrateList = [];

        if ($scope.AddOnRateList.length > 0) {
            getzones();
            for (var i = 0; i < $scope.AddOnRateList.length; i++) {
                var finalJson = {
                    Shipment: "",
                    WeightFrom: 0,
                    WeightTo: 0,
                    Rate: {}
                };

                finalJson.Shipment = $scope.AddOnRateList[i].shipmentType;
                finalJson.WeightFrom = $scope.AddOnRateList[i].WeightFrom;
                finalJson.WeightTo = $scope.AddOnRateList[i].WeightTo;
                finalJson.Rate = $scope.AddOnRateList[i].Rate;

                if (finalJson.Rate.length > 0) {
                    $scope.finalrateList.push(finalJson);
                }
            }
        }
    };

    var ukaddonrate = function () {

        $scope.ukfinaladdonrate = [];

        ukratearray = {
            NWD: [],
            NWDN: [],
            NWD1030: [],
            NWD0900: [],
            SAT: [],
            SAT1030: [],
            SAT0900: [],
            UKDHL: []
        };

        if ($scope.UKaddonRate.length > 0) {
            getzones();
            for (var j = 0; j < $scope.UKaddonRate.length; j++) {
                if ($scope.UKaddonRate[j].shipmentType.LogisticDescription === "NWD") {
                    ukratearray.NWD.push($scope.UKaddonRate[j]);
                }
                if ($scope.UKaddonRate[j].shipmentType.LogisticDescription === "NWDN") {
                    ukratearray.NWDN.push($scope.UKaddonRate[j]);
                }
                if ($scope.UKaddonRate[j].shipmentType.LogisticDescription === "NWD1030") {
                    ukratearray.NWD1030.push($scope.UKaddonRate[j]);
                }
                if ($scope.UKaddonRate[j].shipmentType.LogisticDescription === "NWD0900") {
                    ukratearray.NWD0900.push($scope.UKaddonRate[j]);
                }
                if ($scope.UKaddonRate[j].shipmentType.LogisticDescription === "SAT") {
                    ukratearray.SAT.push($scope.UKaddonRate[j]);
                }
                if ($scope.UKaddonRate[j].shipmentType.LogisticDescription === "SAT1030") {
                    ukratearray.SAT1030.push($scope.UKaddonRate[j]);
                }
                if ($scope.UKaddonRate[j].shipmentType.LogisticDescription === "SAT0900") {
                    ukratearray.SAT0900.push($scope.UKaddonRate[j]);
                }
            }

            for (var type in ukratearray) {
                var newArray = ukratearray[type];
                if (newArray.length > 0) {
                    var finalJson = {
                        DocType: "",
                        WeightFrom: 0,
                        WeightTo: 0,
                        Rate: []
                    };

                    for (var k = 0 ; k < newArray.length ; k++) {
                        finalJson.DocType = newArray[k].shipmentType.LogisticDescriptionDisplay;
                        finalJson.WeightFrom = newArray[k].LogisticWeight.WeightFrom;
                        finalJson.WeightTo = newArray[k].LogisticWeight.WeightTo;
                        finalJson.Rate.push(newArray[k]);
                    }

                    if (finalJson.Rate.length > 0) {
                        $scope.ukfinaladdonrate.push(finalJson);
                    }
                }
            }
        }
    };

    var ukdhladdonrate = function () {
        $scope.ukdhlfinaladdonrate = [];

        ukdhlratearray = {
            UKDHL: []
        };

        if ($scope.UKdhladdonRate.length > 0) {
            getzones();
            for (var j = 0; j < $scope.UKdhladdonRate.length; j++) {
                ukdhlratearray.UKDHL.push($scope.UKdhladdonRate[j]);
            }

            for (var type in ukdhlratearray) {
                var newArray = ukdhlratearray[type];
                if (newArray.length > 0) {
                    var finalJson = {
                        DocType: "",
                        WeightFrom: 0,
                        WeightTo: 0,
                        Rate: []
                    };

                    for (var k = 0 ; k < newArray.length ; k++) {
                        finalJson.DocType = newArray[k].shipmentType.LogisticDescriptionDisplay;
                        finalJson.WeightFrom = newArray[k].LogisticWeight.WeightFrom;
                        finalJson.WeightTo = newArray[k].LogisticWeight.WeightTo;
                        finalJson.Rate.push(newArray[k]);

                        if (finalJson.Rate.length > 0) {
                            $scope.ukdhlfinaladdonrate.push(finalJson);
                        }

                        finalJson = {
                            DocType: "",
                            WeightFrom: 0,
                            WeightTo: 0,
                            Rate: []
                        };
                    }                    
                }
            }
        }
    };

    $scope.UpdateUKDHLAddonrate = function (ukdhlfinaladdonrate) {
        ZoneBaseRateCardService.UpdateUKAddonRate(ukdhlfinaladdonrate).then(function (response) {
            if (response.data.Status === true) {
                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteInformation,
                    body: $scope.UpdateAddOnRateSucess,
                    showCloseButton: true
                });
                $uibModalInstance.close();
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.Frayte_Warning,
                    body: $scope.AddOnNotUpdate,
                    showCloseButton: true
                });
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextSavingError,
                showCloseButton: true
            });
        });
    };

    $scope.UpdateUKAddonRate = function (UKAddOnRate) {
        ZoneBaseRateCardService.UpdateUKAddonRate(UKAddOnRate).then(function (response) {
            if (response.data.Status === true) {
                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteInformation,
                    body: $scope.UpdateAddOnRateSucess,
                    showCloseButton: true
                });
                $uibModalInstance.close();
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.Frayte_Warning,
                    body: $scope.AddOnNotUpdate,
                    showCloseButton: true
                });
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextSavingError,
                showCloseButton: true
            });
        });
    };

    $scope.UpdateAddOnRate = function (AddOnRate) {
        ZoneBaseRateCardService.UpdateAddOnRate(AddOnRate).then(function (response) {
            if (response.data.Status === true) {
                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteInformation,
                    body: $scope.UpdateAddOnRateSucess,
                    showCloseButton: true
                });
                $uibModalInstance.close();
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.Frayte_Warning,
                    body: $scope.AddOnNotUpdate,
                    showCloseButton: true
                });
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextSavingError,
                showCloseButton: true
            });
        });
    };

    function init() {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.spinnerMessage = $scope.LoadingAddOnRateCard;
        $scope.submitted = true;
        setModalOptions();
    }

    init();
});