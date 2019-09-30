angular.module('ngApp.zonePostCode').controller('zonePostCodeController', function (AppSpinner, ThirdPartyMatrixService, $scope, ZonePostCodeService, ZoneBaseRateCardService, $state, $location, $filter, $translate, SessionService, $uibModal, $log, uiGridConstants, toaster, ModalService) {

    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors',
            'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation',
            'PleaseCorrectValidationErrors', 'RateCardSave_Validation', 'Select_CourierAccount', 'THP_Matrix_Saved', 'DataSaved_Validation']).then(function (translations) {

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

    var getZonePostList = function () {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        AppSpinner.showSpinnerTemplate('Loading Zone Post/Zip Code', $scope.Template);
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
        if ($scope.CourierCompany.Value === 'DHL' && $scope.OperationZone.OperationZoneId === 2) {
            RateType = $scope.RateType.Value;
        }

         
        ZonePostCodeService.GetPostCodeList(OperationZoneId, "UKShipment", CourierCompany, RateType, ModuleType).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            $scope.ZonePostCodeList = response.data;
            setZonePostCodeJson();
            if (response.data === null || response.data === undefined ||
                response.data.length === 0) {
                $scope.ZonePostCodeAvailable = false;
            }
            else {
                $scope.ZonePostCodeAvailable = true;
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
        if ($scope.CourierCompany.Value === 'DHL' && $scope.OperationZone.OperationZoneId === 2) {
            RateType = $scope.RateType.Value;
        }
 
        ZonePostCodeService.GetZoneList(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType).then(function (response) {
            $scope.Zones = response.data;
            getZonePostList();
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });
    };

    var getScreenInitials = function () {
        ZonePostCodeService.GetOperationZone().then(function (response) {
            $scope.OperationZones = response.data;
            $scope.OperationZone = $scope.OperationZones[1];
            getPostCodeScreenInitials();
         

        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });
    };

    var getPostCodeScreenInitials = function () {
        ThirdPartyMatrixService.GetLogisticItemList($scope.OperationZone.OperationZoneId).then(function (response) {
            $scope.LogisticItems = response.data;
            $scope.CourierCompanies = response.data.LogisticCompanies;
            
            $scope.RateTypes = response.data.LogisticRateTypes;
            //for (var i = 0; i < $scope.CourierCompanies.length; i++) {
            //    if ($scope.CourierCompanies[i].Value === "DHL" || $scope.CourierCompanies[i].Value === "FedEx" || $scope.CourierCompanies[i].Value === "TNT") {
            //        $scope.CourierCompanies.splice(i,1);
            //    }
            //}
            $scope.CourierCompany = response.data.LogisticCompanies[1];
            getZoneList();
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
        $scope.ModuleType = "DirectBooking";
        $scope.OperationZone = { OperationZoneId: 1, OperationZoneName: 'HKG' };
       
        $scope.flag = false;
        $scope.ZonePostCodeAvailable = true;
        var data1 = {};
        getScreenInitials();
        setModalOptions();
    }

    init();


});