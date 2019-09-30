angular.module('ngApp.CountryZonePostCode').controller('CountryZonePostCodeController', function (toaster, $scope, $uibModal, uiGridConstants, SessionService, AppSpinner, ThirdPartyMatrixService, ZoneBaseRateCardService, CountryZonePostCodeService, $translate) {


    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteSuccess', 'FrayteValidation', 'Record_Deleted_Successfully', 'ErrorDeletingRecord', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteSuccess = translations.FrayteSuccess;
            $scope.TitleFrayteWarning = translations.FrayteWarning;


            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
            $scope.RecordGettingError = translations.ErrorGetting + " " + translations.records;
            $scope.Success = translations.Success;
            $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
            $scope.UpdateRecordValidation = translations.UpdateRecord_Validation;
            $scope.Record_DeletedSuccessfully = translations.Record_Deleted_Successfully;
            $scope.Error_DeletingRecord = translations.ErrorDeletingRecord;
        });
    };

    $scope.AddEditCountryZonePostCode = function (row) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'zoneSetting/countryZonePostCode/countryZonePostCodeAddEdit.tpl.html',
                controller: 'CountryZonePostCodeAddEditController',
                windowClass: '',
                size: 'lg',
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
                    body: $scope.Record_DeletedSuccessfully,
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
            enableGridMenu: true,
            enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
            enableVerticalScrollbar: true,
            columnDefs: [
              { name: 'CountryName', displayName: "Country", headerCellFilter: 'translate', width: "22%" },
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

    var getScreenInitials = function () {

        var OperationZoneId = 0;
        var LogisticType = "";
        var CourierCompany = "";
        var RateType = "";
        var ModuleType = "DirectBooking";

        if ($scope.OperationZone !== undefined) {
            OperationZoneId = $scope.OperationZone.OperationZoneId;
        }

        if ($scope.LogisticType !== undefined) {
            LogisticType = $scope.LogisticType.Value;
        }
        if ($scope.CourierCompany !== undefined) {
            CourierCompany = $scope.CourierCompany.Value;
        }

        if ($scope.RateType !== undefined) {
            RateType = $scope.RateType.Value;
        }

        CountryZonePostCodeService.GetZoneCountryPostCode(OperationZoneId, CourierCompany, LogisticType, RateType).then(function (response) {
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
        ThirdPartyMatrixService.GetLogisticItemList($scope.OperationZone.OperationZoneId).then(function (response) {
            $scope.LogisticItems = response.data;
            $scope.CourierCompanies = response.data.LogisticCompanies;
            $scope.RateTypes = response.data.LogisticRateTypes;
            $scope.LogisticTypes = response.data.LogisticTypes;
            //$scope.CourierCompany = response.data.LogisticCompanies[1];
            $scope.CourierCompany = response.data.LogisticCompanies[0];
            $scope.LogisticType = response.data.LogisticTypes[0];
            $scope.RateType = response.data.LogisticRateTypes[0];
            getScreenInitials();
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
    function init() {
        $scope.ModuleType = "DirectBooking";
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.spinnerMessage = 'Loading Country Zone Post/Zip Code';

        $scope.gridheight = SessionService.getScreenHeight();
        $scope.SetGridOptions();
        AppSpinner.showSpinnerTemplate($scope.spinnerMessage, $scope.Template);
        ZoneBaseRateCardService.GetOperationZone().then(function (response) {
            $scope.OperationZones = response.data;
            $scope.OperationZone = response.data[0];
            getZoneCountryInitials();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });
        //$scope.gridOptions.onRegisterApi = function (gridApi) {
        //    $scope.gridApi = gridApi;
        //};
        
        setModalOptions();
    }

    init();
});