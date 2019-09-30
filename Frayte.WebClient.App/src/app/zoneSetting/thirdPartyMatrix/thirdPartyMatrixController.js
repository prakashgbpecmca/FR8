angular.module('ngApp.thirdPartyMatrix').controller('ThirdPartyMatrixController', function ($scope, config,UtilityService, AppSpinner, toaster, ZoneBaseRateCardService, $state, $translate, SessionService, ZonePostCodeService, ThirdPartyMatrixService) {

    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors',
                    'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation',
                    'PleaseCorrectValidationErrors', 'RateCardSave_Validation', 'Select_CourierAccount', 'THP_Matrix_Saved', 'Loading3rdPartyMatrix']).then(function (translations) {
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
                        $scope.Loading3rdPartyMatrix = translations.Loading3rdPartyMatrix;

                        getThirdPartyScreenInitials();
                    });
    };

    $scope.GetThirdPartyMatrixByOperaionZone = function () {
        ThirdPartyMatrixService.GetLogisticItemList($scope.OperationZone.OperationZoneId).then(function (response) {
            $scope.LogisticItems = response.data;
            $scope.CourierCompanies = response.data.LogisticCompanies;
            $scope.CourierCompany = response.data.LogisticCompanies[0];
            $scope.RateTypes = response.data.LogisticRateTypes;
            $scope.RateType = response.data.LogisticRateTypes[0];
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });
    };

    $scope.SearchThirdPartyMatrix = function () {
        AppSpinner.showSpinnerTemplate($scope.Loading3rdPartyMatrix, $scope.Template);
        $scope.ThirdPartyMatrixData = [];
        $scope.IsChanged = false;
        getThirdPartyZones();
    };

    $scope.createThirdPartyList = function (ThirdParty) {
        if (ThirdParty !== undefined && ThirdParty !== null) {
            for (var i = 0; i < $scope.Zones.length; i++) {
                if (ThirdParty.ApplyZone !== null && ThirdParty.ApplyZone.ZoneName === $scope.Zones[i].ZoneName) {
                    ThirdParty.ApplyZone = $scope.Zones[i];
                    $scope.ThirdPartyList.push(ThirdParty);
                }
            }
        }
    };

    $scope.SetApplyZoneForThirdParty = function (ThirdParty) {
        if (ThirdParty !== undefined && ThirdParty !== null) {
            var arrar = [];
            arrar.push(ThirdParty);
            ThirdPartyMatrixService.SaveThirdPartyMatrix(arrar).then(function (response) {
                toaster.pop({
                    type: 'success',
                    title: $scope.Success,
                    body: $scope.THPMatrixSaved,
                    showCloseButton: true
                });
                init();
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorSavingRecord,
                    showCloseButton: true
                });
            });
        }
    };

    var getScreenInitials = function () {
        var OperationZoneId = 0;
        var LogisticType = "ThirdParty";
        var CourierCompany = "";
        var RateType = "";
        var ModuleType = "DirectBooking";

        if ($scope.OperationZone !== undefined) {
            OperationZoneId = $scope.OperationZone.OperationZoneId;
        }

        if ($scope.CourierCompany !== undefined) {
            CourierCompany = $scope.CourierCompany.Value;
        }

        if ($scope.RateType !== undefined && $scope.CourierCompany !== undefined && $scope.CourierCompany.Value !== 'UKMail' && $scope.CourierCompany.Value !== 'Yodel' && $scope.CourierCompany.Value !== 'Hermes') {
            RateType = $scope.RateType.Value;
        }

        ThirdPartyMatrixService.GetThirdPartyMatrixDetail(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType).then(function (response) {
            if (response.data !== null && response.data.length > 0) {
                $scope.ThirdPartyMatrix = response.data;
                $scope.ThirdPartyMatrixData = [];
                var array = {
                    FromZone: '',
                    Row: []
                };
                for (var j = 0 ; j < $scope.ThirdPartyZones.length; j++) {
                    for (var i = 0; i < $scope.ThirdPartyMatrix.length ; i++) {
                        if ($scope.ThirdPartyZones[j].ZoneName === $scope.ThirdPartyMatrix[i].FromZone.ZoneName) {
                            array.FromZone = $scope.ThirdPartyMatrix[i].FromZone.ZoneDisplayName;
                            array.Row.push($scope.ThirdPartyMatrix[i]);
                        }
                    }

                    if (array.FromZone !== null && array.FromZone !== '' && array !== null && array.Row.length > 0) {
                        $scope.ThirdPartyMatrixData.push(array);
                    }

                    array = {
                        FromZone: '',
                        Row: []
                    };
                }
            }
            else {
                $scope.ThirdPartyMatrixData = [];
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

    var getThirdPartyZones = function () {
        var OperationZoneId = 0;
        var LogisticType = "ThirdParty";
        var CourierCompany = "";
        var RateType = "";
        var ModuleType = "DirectBooking";

        if ($scope.OperationZone !== undefined) {
            OperationZoneId = $scope.OperationZone.OperationZoneId;
        }

        if ($scope.CourierCompany !== undefined) {
            CourierCompany = $scope.CourierCompany.Value;
        }

        if ($scope.RateType !== undefined && $scope.CourierCompany !== undefined && $scope.CourierCompany.Value !== 'UKMail' && $scope.CourierCompany.Value !== 'Yodel' && $scope.CourierCompany.Value !== 'Hermes') {
            RateType = $scope.RateType.Value;
        }
        ThirdPartyMatrixService.GetZones(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType).then(function (response) {
            if (response.data !== null && response.data.length > 0) {
                $scope.ThirdPartyZones = [];
                for (k = 0; k < response.data.length; k++) {
                    $scope.ThirdPartyZones.push(response.data[k]);
                }
                $scope.ThirdPartyZones.unshift({ CourierComapny: '', LogisticType: '', ModuleType: '', OperationZoneId: 0, RateType: '', ZoneColor: '', ZoneDisplayName: 'N/A', ZoneRateName: 'N/A', ZoneId: 0, ZoneName: 'N/A' });
                $scope.ThirdPartyZonesLength = $scope.ThirdPartyZones.length - 1;
                getScreenInitials();
            }
            else {
                AppSpinner.hideSpinnerTemplate();
                $scope.ThirdPartyMatrixData = [];
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

    var getThirdPartyScreenInitials = function () {
        AppSpinner.showSpinnerTemplate($scope.Loading3rdPartyMatrix, $scope.Template);
        UtilityService.getLogisticServices($scope.OperationZone.OperationZoneId).then(function (response) {
            $scope.logisticServices = response.data; 
            $scope.CourierCompanies = UtilityService.getLogisticCompaniesByLogisticType($scope.logisticServices, "ThirdParty");
            $scope.CourierCompany = $scope.CourierCompanies[0];

            $scope.RateTypes = UtilityService.getRateTypesByCourierCompany($scope.logisticServices, $scope.CourierCompany.Value, "ThirdParty");

            if($scope.RateTypes.length){
                $scope.RateType = $scope.RateTypes[0];
            }
            
            AppSpinner.hideSpinnerTemplate();

        }, function (response) {
            AppSpinner.hideSpinnerTemplate();
            if (response.status !== 401) {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.RecordGettingError,
                    showCloseButton: true
                });
            }
        });
    };

    $scope.ThidPartyMatrixByCourierCompany = function () {
        $scope.RateTypes = UtilityService.getRateTypesByCourierCompany($scope.logisticServices, $scope.CourierCompany.Value, "ThirdParty");

        if ($scope.RateTypes.length) {
            $scope.RateType = $scope.RateTypes[0];
        }
    };
        

    $scope.ThirdPartyMatrixByZone = function () {

        getLogisticItems();
    };

    var getLogisticItems = function () {
        ThirdPartyMatrixService.GetLogisticItemList($scope.OperationZone.OperationZoneId).then(function (response) {
            $scope.LogisticItems = response.data;
            angular.copy($scope.LogisticItems, $scope.masterData);
            $scope.CourierCompanies = response.data.LogisticCompanies;
            $scope.RateTypes = response.data.LogisticRateTypes;
            $scope.LogisticTypes = response.data.LogisticTypes;
            $scope.LogisticType = $scope.LogisticTypes[1];
            $scope.CourierCompany = $scope.CourierCompanies[0];
            $scope.RateType = $scope.RateTypes[0];

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
        var userInfo = SessionService.getUser();

        $scope.IsChanged = true;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        //$scope.spinnerMessage = $scope.Loading3rdPartyMatrix;
        $scope.ThirdPartyList = [];

        $scope.OperationZone = { OperationZoneId: userInfo.OperationZoneId, OperationZoneName: "" };
        //getThirdPartyScreenInitials();
        AppSpinner.showSpinnerTemplate($scope.spinnerMessage, $scope.Template);

        $scope.ThirdPartyMatrixData = [];
        setModalOptions();
    }

    init();

});