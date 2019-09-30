angular.module('ngApp.zonePostCode').controller('zonePostCodeController', function (AppSpinner, ThirdPartyMatrixService, UtilityService, $scope, config, ZonePostCodeService, ZoneBaseRateCardService, $state, $location, $filter, $translate, SessionService, $uibModal, $log, uiGridConstants, toaster, ModalService) {

    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors',
            'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation',
            'PleaseCorrectValidationErrors', 'RateCardSave_Validation', 'Select_CourierAccount', 'THP_Matrix_Saved', 'DataSaved_Validation',
        'LoadingZonePostZipCode']).then(function (translations) {

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
            $scope.LoadingZonePostZipCode = translations.LoadingZonePostZipCode;

            getPostCodeScreenInitials();
        });
    };

    // Demo Pagination
    $scope.viewby = 16;
    //    $scope.totalItems = $scope.data.length;

    $scope.currentPage = 1;
    $scope.itemsPerPage = $scope.viewby;
    $scope.maxSize = 2; //Number of pager buttons to show
    // Demo Pagination

    $scope.pageChanged = function (ZonePostCode) {
        ZonePostCodeService.GetZonePostCodeList($scope.OperationZone.OperationZoneId, "UKShipment", ZonePostCode.ZoneId, ZonePostCode.searchPostCode, ZonePostCode.currentPage, ZonePostCode.itemsPerPage).then(function (response) {
            if (response.data === null || response.data === undefined ||
                response.data.length === 0) {
                //$scope.ZonePostCodeAvailable = false;
                ZonePostCode.totalItemCount = 0;
                ZonePostCode.Row = null;
            }
            else {
                //$scope.ZonePostCodeAvailable = true;
                ZonePostCode.totalItemCount = response.data[0].TotalRows;
                ZonePostCode.Row = response.data;
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

    var filterPostCode = function (PostCode) {
        return PostCode.PostCode === $scope.searchPostCode.length;
    };

    $scope.refreshPostCode = function (ZonePostCode) {
        if (ZonePostCode.searchPostCode.length >= 2) {
            ZonePostCode.currentPage = 1;
            $scope.pageChanged(ZonePostCode);
        }
        else if (ZonePostCode.searchPostCode.length === 0) {
            ZonePostCode.currentPage = 1;
            $scope.pageChanged(ZonePostCode);
        }
    };

    $scope.ZonePostCodeByOperationZone = function () {
        getZonePostList();
    };

    $scope.AddEditPostCodeModal = function (ZonePostCode) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'zoneSetting/zonePostCode/zonePostCodeAddEdit.tpl.html',
            controller: 'ZonePostCodeAddEditController',
            windowClass: 'zonePostCode-Modal',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                ZonePostCode: function () {
                    if (ZonePostCode !== undefined) {
                        return Object.create(ZonePostCode);
                    }
                },
                OperationZone: function () {
                    return $scope.OperationZone;
                },
                CourierCompany: function () {
                    return $scope.CourierCompany;
                },
                ModuleType: function () {
                    return $scope.ModuleType;
                }

            }
        });
        modalInstance.result.then(function (PostCode) {
            getZoneList();
        });
    };

    var setZonePostCodeJson = function () {
        $scope.ZonePostCodeJson = [];
        var obj = {
            currentPage: $scope.currentPage,
            itemsPerPage: $scope.viewby,
            maxSize: $scope.maxSize,
            totalItemCount: 0,
            searchPostCode: '',
            ZoneId: 0,
            Zone: '',
            ZoneDisplayName: '',
            ZoneColor: '',
            TransitTime: 0,
            Row: []
        };

        for (var b = 0; b < $scope.Zones.length; b++) {
            obj.ZoneId = $scope.Zones[b].ZoneId,
            obj.Zone = $scope.Zones[b].ZoneName;
            obj.ZoneColor = $scope.Zones[b].ZoneColor;

            for (var a = 0; a < $scope.ZonePostCodeList.length; a++) {
                if ($scope.Zones[b].ZoneId === $scope.ZonePostCodeList[a].PostCodeZone.ZoneId) {
                    obj.totalItemCount = $scope.ZonePostCodeList[a].TotalRows,
                    obj.ZoneDisplayName = $scope.ZonePostCodeList[a].PostCodeZone.ZoneDisplayName;
                    obj.TransitTime = $scope.ZonePostCodeList[a].TransitTime;
                    obj.Row.push($scope.ZonePostCodeList[a]);
                }
            }

            $scope.ZonePostCodeJson.push(obj);

            obj = {
                currentPage: $scope.currentPage,
                itemsPerPage: $scope.viewby,
                maxSize: $scope.maxSize,
                totalItemCount: 2000,
                searchPostCode: '',
                ZoneId: 0,
                Zone: '',
                Row: []
            };
        }

    };

    $scope.ShowTransitTime = function (Transit) {
        if (Transit > 1) {
            return Transit + " " + "Days";
        }
        else {
            return Transit + " " + "Day";
        }
    };

    var getZonePostList = function () {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        AppSpinner.showSpinnerTemplate($scope.LoadingZonePostZipCode, $scope.Template);
        var LogisticServiceType = "";
        if ($scope.LogisticService !== undefined) {
            LogisticServiceType = $scope.LogisticService.LogisticServiceName;
        }

        var OperationZoneId = 0;
        var LogisticType = "";
        var CourierCompany = "";
        var RateType = "";
        var ModuleType = "DirectBooking";

        if ($scope.OperationZone !== undefined) {
            OperationZoneId = $scope.OperationZone.OperationZoneId;
        }

        LogisticType = "UKShipment";

        if ($scope.CourierCompany !== undefined) {
            CourierCompany = $scope.CourierCompany.Value;
        }
        if ($scope.OperationZone.OperationZoneId === 2 && ($scope.CourierCompany.Value === 'DHL' || $scope.CourierCompany.Value === 'DPD')) {
            RateType = $scope.RateType.Value;
        }

        ZonePostCodeService.GetPostCodeList(OperationZoneId, "UKShipment", CourierCompany, RateType, ModuleType).then(function (response) {
            AppSpinner.hideSpinnerTemplate();

            if (response.data === null || response.data === undefined ||
                response.data.length === 0) {
                $scope.ZonePostCodeAvailable = false;
            }
            else {
                $scope.ZonePostCodeAvailable = true;
                $scope.ZonePostCodeList = response.data;
                setZonePostCodeJson();
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
    };
    $scope.SearchZonePostCode = function () {
        getZoneList();
    };

    var getZoneList = function () {

        var OperationZoneId = 0;
        var LogisticType = "";
        var CourierCompany = "";
        var RateType = "";
        var ModuleType = "DirectBooking";

        if ($scope.OperationZone !== undefined) {
            OperationZoneId = $scope.OperationZone.OperationZoneId;
        }

        LogisticType = "UKShipment";

        if ($scope.CourierCompany !== undefined) {
            CourierCompany = $scope.CourierCompany.Value;
        }
        if ($scope.OperationZone.OperationZoneId === 2 && ($scope.CourierCompany.Value === 'DHL' || $scope.CourierCompany.Value === 'DPD')) {
            RateType = $scope.RateType.Value;
        }

        ZonePostCodeService.GetZoneList(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType).then(function (response) {
            $scope.Zones = response.data;
            getZonePostList();
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
    };


    $scope.OperationZone = { OperationZoneId: 0, OperationZoneName: "" };

    $scope.zonePostCodeByCourierCompany = function () {
        $scope.RateTypes = UtilityService.getRateTypesByCourierCompany($scope.logisticServices, $scope.CourierCompany.Value, "UKShipment");
        if ($scope.RateTypes.length) {
            $scope.RateType = $scope.RateTypes[0];
        }

    };



    var getPostCodeScreenInitials = function () {
        UtilityService.getLogisticServices($scope.OperationZone.OperationZoneId).then(function (response) {
            $scope.logisticServices = response.data;


            $scope.CourierCompanies = UtilityService.getLogisticCompaniesByLogisticType($scope.logisticServices, "UKShipment");
            if ($scope.CourierCompanies.length) {
                $scope.CourierCompany = $scope.CourierCompanies[0];

                $scope.RateTypes = UtilityService.getRateTypesByCourierCompany($scope.logisticServices, $scope.CourierCompany.Value, "UKShipment");
                if ($scope.RateTypes.length) {
                    $scope.RateType = $scope.RateTypes[0];
                }
            }

            getZoneList();
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
    };

    function init() {

        var userInfo = SessionService.getUser();
        $scope.ModuleType = "DirectBooking";

        $scope.OperationZone = { OperationZoneId: userInfo.OperationZoneId, OperationZoneName: "" };

        $scope.flag = false;
        $scope.ZonePostCodeAvailable = true;
        var data1 = {};
        setModalOptions();
    }

    init();


});