angular.module('ngApp.CountryZonePostCode').controller('CountryZonePostCodeController', function (toaster, $scope, config, UtilityService, $uibModal, uiGridConstants, SessionService, AppSpinner, ThirdPartyMatrixService, ZoneBaseRateCardService, CountryZonePostCodeService, $translate) {


    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteSuccess', 'FrayteValidation', 'Record_Deleted_Successfully', 'ErrorDeletingRecord', 'ErrorSavingRecord',
            'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation',
            'UpdateRecord_Validation', 'Country_ZonePost_Code_Deleted', 'LoadingCountryZonePostZipCode']).then(function (translations) {
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteSuccess = translations.FrayteSuccess;
            $scope.TitleFrayteWarning = translations.FrayteWarning;
            $scope.LoadingCountryZonePostZipCode = translations.LoadingCountryZonePostZipCode;
            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
            $scope.RecordGettingError = translations.ErrorGetting + " " + translations.records;
            $scope.Success = translations.Success;
            $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
            $scope.UpdateRecordValidation = translations.UpdateRecord_Validation;
            $scope.Country_ZonePost_Code_Deleted = translations.Country_ZonePost_Code_Deleted;
            $scope.Error_DeletingRecord = translations.ErrorDeletingRecord;
            // getZoneCountryInitials Call
            getZoneCountryInitials();
        });
        
    };

    $scope.AddEditCountryZonePostCode = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'zoneSetting/countryZonePostCode/countryZonePostCodeAddEdit.tpl.html',
            controller: 'CountryZonePostCodeAddEditController',
            windowClass: '',
            size: 'md',
            backdrop: 'static',
            resolve: {
                mode: function () {
                    if (row === undefined) {
                        return 'Add';
                    }
                    else {
                        return 'Edit';
                    }
                },
                CountryZonePostCode: function () {
                    if (row === undefined) {
                        return {

                            LogisticZoneCountryPostCodeId: 0,
                            OperationZoneId: null,
                            LogisticCompany: '',
                            LogisticType: '',
                            RateType: '',
                            CountryName: '',
                            FromPostCode: '',
                            ToPostCode: '',
                            Zone: '',
                            ZoneDisplay: ''



                        };
                    }
                    else {
                        return row.entity;
                    }
                },
                OperationZone: function () {
                    return $scope.OperationZone;
                },
                LogisticType: function () {
                    return $scope.LogisticType;
                },
                LogisticCompanies: function () {
                    return $scope.CourierCompany;
                },
                RateType: function () {
                    return $scope.RateType;
                }
            }
        });
        modalInstance.result.then(function (countryAccount) {
            getScreenInitials();
        }, function () {
        });

    };

    $scope.DeleteCountryZonePostCode = function (row) {
        CountryZonePostCodeService.DeleteZoneCountryPostCode(row.entity.LogisticZoneCountryPostCodeId).then(function (response) {
            getScreenInitials();
            if (response.status === 200) {
                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteSuccess,
                    body: $scope.Country_ZonePost_Code_Deleted,
                    showCloseButton: true
                });

            }

        },
        function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.Error_DeletingRecord,
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
              { name: 'CountryName', displayName: "Country", headerCellFilter: 'translate', width: "23%" },
              { name: 'FromPostCode', displayName: "From_Postcode", headerCellFilter: 'translate', width: "22%" },
               { name: 'ToPostCode', displayName: "To_Postcode", headerCellFilter: 'translate', width: "22%" },
                { name: 'ZoneDisplay', displayName: "Zone_Name", headerCellFilter: 'translate', width: "22%" },
              //{ name: 'ToPostCode', displayName: "Country", cellTemplate: '<div class="ui-grid-cell-contents">{{grid.appScope.ShowCountryList(row,col.field)}}</div>', headerCellFilter: 'translate', width: "60%" },
              { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "zoneSetting/countryZonePostCode/countryZonePostCodeEdit.tpl.html" }



            ]
        };
    };

    $scope.SearchCountryZonePostCode = function () {
        getScreenInitials();
    };

    $scope.ZoneCountryByCourierCompany = function () {
        $scope.RateTypes = UtilityService.getRateTypesByCourierCompany($scope.logisticServices, $scope.CourierCompany.Value, $scope.LogisticType.Value);

        if ($scope.RateTypes.length) {

            $scope.RateType = $scope.RateTypes[0];
        }
    };

    $scope.ZoneCountryByLogisticType = function () {
        $scope.CourierCompanies = UtilityService.getLogisticCompaniesByLogisticType($scope.logisticServices, $scope.LogisticType.Value);

        if ($scope.CourierCompanies.length) {
            $scope.CourierCompany = $scope.CourierCompanies[0];
        }

        $scope.RateTypes = UtilityService.getRateTypesByCourierCompany($scope.logisticServices, $scope.CourierCompany.Value, $scope.LogisticType.Value);

        if ($scope.RateTypes.length) {

            $scope.RateType = $scope.RateTypes[0];
        }
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

        CountryZonePostCodeService.GetZoneCountryPostCode(OperationZoneId, CourierCompany, LogisticType, RateType).then(function (response) {
            $scope.gridOptions.data = response.data;
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

    $scope.ZoneCountryByOperationZone = function () {

        ThirdPartyMatrixService.GetLogisticItemList($scope.OperationZone.OperationZoneId).then(function (response) {
            $scope.LogisticItems = response.data;
            $scope.CourierCompanies = response.data.LogisticCompanies;
            $scope.RateTypes = response.data.LogisticRateTypes;
            $scope.LogisticTypes = response.data.LogisticTypes;
            //$scope.CourierCompany = response.data.LogisticCompanies[1];
            $scope.CourierCompany = response.data.LogisticCompanies[0];
            $scope.LogisticType = response.data.LogisticTypes[0];
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

    var getZoneCountryInitials = function () {
        AppSpinner.showSpinnerTemplate($scope.LoadingCountryZonePostZipCode, $scope.Template);
        UtilityService.getLogisticServices($scope.OperationZone.OperationZoneId).then(function (response) {

            $scope.logisticServices = response.data;

            $scope.LogisticTypes = UtilityService.getLogisticTypesByOperationZone($scope.logisticServices, $scope.OperationZone.OperationZoneId);
            $scope.LogisticType = $scope.LogisticTypes[0];

            $scope.CourierCompanies = UtilityService.getLogisticCompaniesByLogisticType($scope.logisticServices, $scope.LogisticType.Value);

            if ($scope.CourierCompanies.length) {
                $scope.CourierCompany = $scope.CourierCompanies[0];
            }

            $scope.RateTypes = UtilityService.getRateTypesByCourierCompany($scope.logisticServices, $scope.CourierCompany.Value, $scope.LogisticType.Value);

            if ($scope.RateTypes.length) {

                $scope.RateType = $scope.RateTypes[0];
            }

            getScreenInitials();
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

    function init() {
        var userInfo = SessionService.getUser();
        $scope.ModuleType = "DirectBooking";
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        //$scope.spinnerMessage = $scope.LoadingCountryZonePostZipCode;

        $scope.OperationZone = { OperationZoneId: userInfo.OperationZoneId, OperationZoneName: "" };
        //getZoneCountryInitials();
        $scope.gridheight = SessionService.getScreenHeight();
        $scope.SetGridOptions();
        setModalOptions();
    }

    init();
});