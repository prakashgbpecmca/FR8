angular.module('ngApp.zoneCountry').controller('ZoneCountryController', function ($scope, config, AppSpinner, UtilityService, ThirdPartyMatrixService, ZoneBaseRateCardService, $state, $location, $filter, $translate, SessionService, $uibModal, $log, uiGridConstants, ZoneCountryService, toaster, ModalService) {

    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors',
                    'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation',
                    'PleaseCorrectValidationErrors', 'RateCardSave_Validation', 'Select_CourierAccount', 'THP_Matrix_Saved', 'LoadingCountryZones']).then(function (translations) {
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
                        $scope.LoadingCountryZones = translations.LoadingCountryZones;
                        //getLogisticServices Call
                        getLogisticServices();
                    });
    };


    // Add Edit ZOne Country
    $scope.AddEditZoneCountry = function (row) {

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'zoneSetting/zoneCountry/zoneCountryAddEdit.tpl.html',
            controller: 'ZoneCountryAddEditController',
            windowClass: '',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                mode: function () {
                    if (row === undefined) {
                        return 'Add';
                    }
                    else {
                        return 'Modify';
                    }
                },
                zoneCountry: function () {
                    if (row === undefined) {
                        return {
                            ZoneCountryId: 0,
                            ZoneId: 0,
                            OperationZoneId: 0,
                            OperationZoneName: '',
                            ZoneName: '',
                            Fraytezonezountry: []
                        };
                    }
                    else {
                        return row.entity;
                    }
                },
                LogisticType: function () {
                    return $scope.LogisticType;
                },
                CourierCompany: function () {
                    return $scope.CourierCompany;
                },
                RateType: function () {
                    return $scope.RateType;
                },
                ModuleType: function () {
                    return $scope.ModuleType;
                }
            }
        });
        modalInstance.result.then(function (LogisticType) {
            AppSpinner.showSpinnerTemplate($scope.spinnerMessage, $scope.Template);
            $scope.LogisticType = LogisticType;
            getScreenInitials();

        }, function () {
        });
    };

    $scope.ShowCountryList = function (row, field) {
        if (row === undefined) {
            return '';
        }
        else {
            var value = '';
            var data = row.entity.Fraytezonezountry;
            for (var i = 0 ; i < data.length; i++) {
                value += ', ' + data[i].CountryName;
                row.entity.Value = value;
            }
            return value.slice(1);
        }
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
              { name: 'OperationZoneName', displayName: "Business_Unit", headerCellFilter: 'translate', width: "15%" },
              { name: 'ZoneDisplayName', displayName: "Zone_Name", headerCellFilter: 'translate', width: "15%" },
              { name: 'Value', displayName: "Country", cellTemplate: '<div class="ui-grid-cell-contents">{{grid.appScope.ShowCountryList(row,col.field)}}</div>', headerCellFilter: 'translate', width: "59%" },
              { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "zoneSetting/zoneCountry/zoneEdit.tpl.html" }
            ]
        };
    };

    var getScreenInitials = function () {

        var OperationZoneId = 0;
        var LogisticType = "";
        var CourierCompany = "";
        var RateType = "";
        var ModuleType = "DirectBooking";

        if ($scope.OperationZone) {
            OperationZoneId = $scope.OperationZone.OperationZoneId;
        }

        if ($scope.LogisticType) {
            LogisticType = $scope.LogisticType.Value;
        }
        if ($scope.CourierCompany) {
            CourierCompany = $scope.CourierCompany.Value;
        }

        if ($scope.RateType) {
            RateType = $scope.RateType.Value;
        }

        ZoneCountryService.GetZoneCountry(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType).then(function (response) {
            $scope.gridOptions.data = response.data;
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

    $scope.toggleRowSelection = function () {
        $scope.gridApi.selection.clearSelectedRows();
        $scope.gridOptions.enableRowSelection = !$scope.gridOptions.enableRowSelection;
        $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS);
    };

    $scope.ZoneCountryByOperationZone = function () {
        getLogisticServices();
    };


    $scope.SearchZoneCountry = function () {
        getScreenInitials();
    };

    $scope.ZoneCountryByLogisticType = function () {
        $scope.CourierCompanies = UtilityService.getLogisticCompaniesByLogisticType($scope.logisticServices, $scope.LogisticType.Value);
        $scope.CourierCompany = $scope.CourierCompanies[0];

        $scope.RateTypes = UtilityService.getRateTypesByCourierCompany($scope.logisticServices, $scope.CourierCompany.Value, $scope.LogisticType.Value);
        $scope.RateType = $scope.RateTypes[0];
    };

    $scope.ZoneCountryByCourierCompany = function () {
        $scope.RateTypes = UtilityService.getRateTypesByCourierCompany($scope.logisticServices, $scope.CourierCompany.Value, $scope.LogisticType.Value);
        $scope.RateType = $scope.RateTypes[0];
    };

    var getLogisticServices = function () {
        AppSpinner.showSpinnerTemplate($scope.LoadingCountryZones, $scope.Template);
        UtilityService.getLogisticServices($scope.OperationZone.OperationZoneId).then(function (response) {
            if (response.data) {
                $scope.logisticServices = response.data;

                $scope.LogisticTypes = UtilityService.getLogisticTypesByOperationZone($scope.logisticServices, $scope.OperationZone.OperationZoneId);
                $scope.LogisticType = $scope.LogisticTypes[0];

                $scope.CourierCompanies = UtilityService.getLogisticCompaniesByLogisticType($scope.logisticServices, $scope.LogisticType.Value);
                $scope.CourierCompany = $scope.CourierCompanies[0];

                $scope.RateTypes = UtilityService.getRateTypesByCourierCompany($scope.logisticServices, $scope.CourierCompany.Value, $scope.LogisticType.Value);
                if ($scope.RateTypes.length) {
                    $scope.RateType = $scope.RateTypes[0];
                }

                getScreenInitials();
            }
        }, function () {
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

    function init() {
        $scope.ModuleType = "DirectBooking";
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        //$scope.spinnerMessage = $scope.LoadingCountryZones;

        var userInfo = SessionService.getUser();

        $scope.OperationZone = { OperationZoneId: userInfo.OperationZoneId, OperationZoneName: "" };

        $scope.gridheight = SessionService.getScreenHeight();
        $scope.SetGridOptions();


        $scope.gridOptions.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
        };


        setModalOptions();

    }

    init();

});