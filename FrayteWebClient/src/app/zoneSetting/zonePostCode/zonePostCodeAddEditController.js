angular.module('ngApp.zonePostCode').controller('ZonePostCodeAddEditController', function (AppSpinner, $scope, OperationZone, ZonePostCodeService,CourierCompany,ModuleType, ZonePostCode, $state, $location, $filter, $translate, SessionService, $uibModal, $log, uiGridConstants, toaster, ModalService, $uibModalInstance) {

    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors', 'FrayteSuccess',
            'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation',
            'PleaseCorrectValidationErrors', 'RateCardSave_Validation', 'Select_CourierAccount', 'THP_Matrix_Saved', 'DataSaved_Validation']).then(function (translations) {

                $scope.TitleFrayteError = translations.FrayteError;
                $scope.TitleFrayteInformation = translations.FrayteInformation;
                $scope.TitleFrayteValidation = translations.FrayteValidation;

                $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
                $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
                $scope.RecordGettingError = translations.ErrorGetting + " " + translations.records;
                $scope.Success = translations.FrayteSuccess;
                $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
                $scope.UpdateRecordValidation = translations.UpdateRecord_Validation;
                $scope.PleaseCorrectValidationErrors = translations.PleaseCorrectValidationErrors;
                $scope.RateCardSaveValidation = translations.RateCardSave_Validation;
                $scope.SelectCourierAccount = translations.Select_CourierAccount;
                $scope.THPMatrixSaved = translations.THP_Matrix_Saved;

            });
    };
    $scope.viewby = 22;
    //$scope.totalItems = $scope.data.length;
    $scope.currentPage = 1;
    $scope.itemsPerPage = $scope.viewby;
    $scope.maxSize = 3; //Number of pager buttons to show

    $scope.viewby1 = 22;
    //$scope.totalItems = $scope.data.length;
    $scope.currentPage1 = 1;
    $scope.itemsPerPage1 = $scope.viewby1;
    $scope.maxSize1 = 7; //Number of pager buttons to show


    // ToDo : Filter PostCode based on Country
    //$scope.setUKCountryPostCode = function () {
    //    $scope.UKPostCodeJson = [];
    //    for (var i = 0; i < $scope.UKPostCodes.length; i++) {
    //        if ($scope.UkCountry.CountryUKId === $scope.UKPostCodes[i].CountryUKId) {
    //            $scope.UKPostCodeJson.push($scope.UKPostCodes[i]);
    //        }
    //    }
    //    $scope.data = $scope.UKPostCodeJson;
    //};

    $scope.pageChanged = function (ZonePostCode) {
        ZonePostCodeService.GetZonePostCodeList($scope.BusinessOperationZone.OperationZoneId, "UKShipment", ZonePostCode.ZoneId, ZonePostCode.searchPostCode, ZonePostCode.currentPage, ZonePostCode.itemsPerPage).then(function (response) {
            if (response.data === null || response.data === undefined ||
                response.data.length === 0) {                
                ZonePostCode.totalItemCount = 0;
                ZonePostCode.Row = null;
            }
            else {                
                ZonePostCode.totalItemCount = response.data[0].TotalRows;
                ZonePostCode.Row = response.data;
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

    $scope.submit = function () {

        ZonePostCodeService.SavePostCode($scope.PostCodeSaveJson).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'success',
                title: $scope.Success,
                body: $scope.TextSuccessfullySavedInformation,
                showCloseButton: true
            });
            $uibModalInstance.close($scope.ZonePostCode.Row);
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorSavingRecord,
                showCloseButton: true
            });
        });


    };
    $scope.SetPostCodeToZone = function (UkPost) {
        var flag = false;
        if (UkPost !== undefined && UkPost !== null) {
            var data = {
                PostCode: UkPost.PostCode,
                PostCodeZone: {
                    OperationZoneId: $scope.ZonePostCode.Row[0].PostCodeZone.OperationZoneId,
                    ZoneId: $scope.ZonePostCode.Row[0].PostCodeZone.ZoneId,
                    ZoneName: $scope.ZonePostCode.Row[0].PostCodeZone.ZoneName
                },
                ZonePostCodeId: 0,
                IsActive: true
            };

            //
            $scope.ZonePostCode.Row.push(data);
            if ($scope.PostCodeSaveJson.length > 0) {
                for (var a = 0; a < $scope.PostCodeSaveJson.length; a++) {
                    if (UkPost.PostCode === $scope.PostCodeSaveJson[a].PostCode) {
                        $scope.PostCodeSaveJson[a].IsActive = true;
                        flag = true;
                        $scope.PostCodeSaveJson.splice(a, 1);
                        break;
                    }
                }
                if(!flag){
                    $scope.PostCodeSaveJson.push(data);
                }
            }
            else {
                $scope.PostCodeSaveJson.push(data);
            }
            

            for (var i = 0; i < $scope.UKPostCodeJson.length; i++) {
                if (UkPost.PostCode === $scope.UKPostCodeJson[i].PostCode) {
                    $scope.UKPostCodeJson.splice(i, 1);
                    break;
                }
            }

        }
    };
    $scope.RemovePostCodeFromZone = function (ZonePostCode) {
        if (ZonePostCode !== undefined && ZonePostCode !== null) {
            for (var i = 0; i < $scope.ZonePostCode.Row.length; i++) {
                if (ZonePostCode.PostCode === $scope.ZonePostCode.Row[i].PostCode) {
                    $scope.ZonePostCode.Row[i].IsActive = false;
                    $scope.ZonePostCode.Row.splice(i, 1);
                    break;
                }
            }

            if ($scope.PostCodeSaveJson !== null) {
                ZonePostCode.IsActive = false;
                $scope.PostCodeSaveJson.push(ZonePostCode);
            }
            //if ($scope.PostCodeSaveJson !== null && $scope.PostCodeSaveJson.length > 0) {
            //    for (var a = 0; a < $scope.PostCodeSaveJson.length; a++) {
            //        if (ZonePostCode.PostCode === $scope.PostCodeSaveJson[a].PostCode) {
            //            $scope.PostCodeSaveJson.splice(i, 1);
            //        }
            //    }
            //}

            $scope.UKPostCodeJson.push({ CountryUKId: 0, PostCode: ZonePostCode.PostCode });
        }

    };

    $scope.refreshData = function () {
        $scope.UKPostCodeJson = $filter('filter')($scope.UKPostCodes, $scope.searchText, undefined);
    };
    var getAllUKPostCode = function () {
        var CourierCompany = "";
        var RateType = "";
        var ModuleType = "DirectBooking";
        if($scope.ZonePostCodeCourierCompany !== undefined){
            CourierCompany = $scope.ZonePostCodeCourierCompany.Value;
        }
        ZonePostCodeService.GetCountryUKPostCode($scope.BusinessOperationZone.OperationZoneId, "UKShipment", CourierCompany, RateType, ModuleType).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            $scope.UKPostCodes = response.data;
            $scope.UKPostCodeJson = response.data;
            // $scope.setUKCountryPostCode();
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

        getAllUKPostCode();

        //ZonePostCodeService.GetCountryUK().then(function (response) {
        //    $scope.UKCountries = response.data;
        //    getAllUKPostCode();

        //}, function () {
        //    toaster.pop({
        //        type: 'error',
        //        title: $scope.TitleFrayteError,
        //        body: 'Error While Getting Records.',
        //        showCloseButton: true
        //    });
        //});
    };

    function init() {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        //$scope.spinnerMessage = 'Loading Base Rate Card...';
        AppSpinner.showSpinnerTemplate('Loading Zone Post/Zip Code', $scope.Template);
        $scope.PostCodeSaveJson = [];
        $scope.BusinessOperationZone = OperationZone;
        if (CourierCompany !== undefined) {
            $scope.ZonePostCodeCourierCompany = CourierCompany;
        } 
        if (ModuleType !== undefined) {
            $scope.ZonePostCodeModuleType = ModuleType;
        }
        
        ZonePostCode.currentPage = $scope.currentPage;
        ZonePostCode.itemsPerPage = $scope.viewby;
        ZonePostCode.maxSize = $scope.maxSize;
        ZonePostCode.searchPostCode = '';        

        $scope.ZonePostCode = ZonePostCode;

        $scope.pageChanged($scope.ZonePostCode);

        getScreenInitials();
        setModalOptions();
    }

    init();
});