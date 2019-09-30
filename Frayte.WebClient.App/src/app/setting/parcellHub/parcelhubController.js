/** 
 * Controller
// */
angular.module('ngApp.parcelhub').controller('ParcelHubController', function ($scope, ModalService , $state, ParcelHubService, $translate, uiGridConstants, SettingService, config, $filter, LogonService, SessionService, $uibModal, toaster) {
 
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors',
            'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation',
            'PleaseCorrectValidationErrors', 'RateCardSave_Validation', 'Select_CourierAccount', 'THP_Matrix_Saved', 'DataSaved_Validation',
        'Record_Saved', 'APIkeyUserkey_Saved_Validation', 'ParcelHubKeysSavingErrorValidation', 'PleaseTryLater', 'ParcelHubKeyRemoved_Validation',
        'PacelHub', 'Keys', 'Delete_Confirmation']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
            $scope.RecordGettingError = translations.ErrorGetting + " " + translations.records;
            $scope.Success = translations.Success;
            $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
            $scope.PleaseCorrectValidationErrors = translations.PleaseCorrectValidationErrors;
            $scope.RecordSaved = translations.Record_Saved;
            $scope.APIkeyUserkeySavedValidation = translations.APIkeyUserkey_Saved_Validation;
            $scope.ParcelHubKeysSavingErrorValidation = translations.ParcelHubKeysSavingError_Validation;
            $scope.ParcelHubKeyRemovedTryLaterValidation = translations.ParcelHubKeysSavingError_Validation + " " + translations.PleaseTryLater;
            $scope.ParcelHubKeyGettingValidation = translations.ErrorGetting + " " + translations.PacelHub + " " + translations.Keys;
            $scope.ParcelHubKeyRemovedValidation = translations.ParcelHubKeyRemoved_Validation;
            $scope.Delete_Confirmation = translations.Delete_Confirmation;
            $scope.Delete_Parcel_Hub_Key = translations.Delete_Parcel_Hub_Key;
        });
    };
    $scope.AddEditAPIKeys = function(row){
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'setting/parcellHub/parcelhubAddEdit.tpl.html',
            controller: 'ParcelHubAddEditController',
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
                ParcelHubKey: function () {
                    if (row === undefined) {
                        return {
                            APIId: 0,
                            APIName: '',
                            APIKey: ''
                        };
                    }
                    else {
                        return row.entity;
                    }
                }
            }
        });
        modalInstance.result.then(function (ParcelHubKey) {
            init();
        }, function () {
        });
        
    };
    $scope.DeleteAPIKeys = function (row) {
        var modalOptions = {
            headerText: $scope.Delete_Confirmation,
            bodyText: $scope.Delete_Parcel_Hub_Key
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            ParcelHubService.DeleteParcelHubKey(row.entity.APIId).then(function (response) {
                if (response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.Success,
                        body: $scope.ParcelHubKeyRemovedValidation,
                        showCloseButton: true
                    });
                    init();
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.ParcelHubKeyRemovedTryLaterValidation,
                        showCloseButton: true
                    });
                }
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.ParcelHubKeyRemovedTryLaterValidation,
                    showCloseButton: true
                });
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
          { name: 'APIName', displayName: 'User_Key', headerCellFilter: 'translate', width: "60%" },
          { name: 'APIKey', displayName: 'API_Key', headerCellFilter: 'translate', width: "28%" },
          { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "setting/parcellHub/parcelhubEditButton.tpl.html"}
        ]
    };
};

$scope.toggleRowSelection = function () {
    $scope.gridApi.selection.clearSelectedRows();
    $scope.gridOptions.enableRowSelection = !$scope.gridOptions.enableRowSelection;
    $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS);
};
function init() {
    $scope.gridheight = SessionService.getScreenHeight();
    $scope.SetGridOptions();
    ParcelHubService.GetParcelHubApiKeys().then(function (response) {
        $scope.gridOptions.data = response.data;
    }, function (reason) {
        if (reason.status !== 401) {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.ParcelHubKeyGettingValidation,
                showCloseButton: true
            });
        }

    });
    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $scope.gridApi = gridApi;
    };
    setModalOptions();
}

init();

});