angular.module('ngApp.ecommerceSetting').controller('HsCodeSettingController', function ($scope, ecommerceSettingService, $uibModal, toaster, $translate, ModalService) {

    var setModalOptions = function () {
        $translate(['Address', 'Book', 'DeleteHeader', 'DeleteBody', 'Tradelane', 'SuccessfullySavedInformation', 'FrayteInformation', 'FrayteError', 'ErrorSavingRecord', 'ErrorGetting',
            'records', 'CustomerDelete_Validation', 'Delete', 'Confirmation', 'HS_Code', 'AHs_Record', 'Error_While_Deleting_The_Records', 'Record_Deleted_Successfully']).then(function (translations) {
                        $scope.headerTextOtherAddress = translations.Address + " " + translations.Book + " " + translations.DeleteHeader;
                        $scope.bodyTextOtherAddress = translations.DeleteBody + " " + translations.Address;
                        $scope.headerTextTradeLane = translations.Tradelane + " " + translations.DeleteHeader;
                        $scope.bodyTextTradeLane = translations.DeleteBody + " " + translations.Tradelane + " " + translations.detail;
                        $scope.TitleFrayteInformation = translations.FrayteInformation;
                        $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
                        $scope.TitleFrayteValidation = translations.FrayteValidation;
                        $scope.TextValidation = translations.PleaseCorrectValidationErrors;
                        $scope.TitleFrayteError = translations.FrayteError;
                        $scope.TextSavingError = translations.ErrorSavingRecord;
                        $scope.TextErrorGetting = translations.ErrorGetting + " " + translations.customer + " " + translations.detail;
                        $scope.HsCodeDeleteHeader = translations.HS_Code + " " + translations.Delete + " " + translations.Confirmation;
                        $scope.HsCodeDelete = translations.DeleteBody + " " + translations.AHs_Record;
                        $scope.GettingZoneError = translations.ErrorGetting + " " + translations.Zone;
                        $scope.TextErrorGettingShipment = translations.ErrorGetting + " " + translations.customer + " " + translations.Shipment_Type;
                        $scope.SelectCourierAccount = translations.Select_CourierAccount;
                        $scope.CustomerDeleteValidation = translations.CustomerDelete_Validation;
                        $scope.CustomerDeleteErrorValidation = translations.CustomerDeleteError_Validation;
                        $scope.RecordGettingError = translations.ErrorGetting + " " + translations.records;
                        $scope.Error_While_DeletingThe_Records = translations.Error_While_Deleting_The_Records;
                        $scope.RecordDeleted_Successfully = translations.Record_Deleted_Successfully;
                    });
    };



    $scope.HSCodeRecords = function () {
        ecommerceSettingService.GetHSCodeDetailList($scope.Country.CountryId).then(function (response) {
            $scope.HSCodeList = response.data;
          
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });
    };

    $scope.DeleteHSCode = function (HSCode) {
        var modalOptions = {
            headerText: $scope.HsCodeDeleteHeader,

            bodyText: $scope.HsCodeDelete
        };
        ModalService.Confirm({}, modalOptions).then(function (result) {
            ecommerceSettingService.DeleteHSCodeSetting(HSCode.HsCodeId).then(function (response) {
                $scope.HSCodeData = response.data;
                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteInformation,
                    body: $scope.RecordDeleted_Successfully,
                    showCloseButton: true,
                    closeHtml: '<button></button>'
                });
                $scope.HSCodeRecords();

            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: Error_While_DeletingThe_Records,
                    showCloseButton: true,
                    closeHtml: '<button>Close</button>'
                });
            });
        });
    };

    $scope.AddEditHSCode = function (HSCode) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'eCommerceSetting/hsCodeSetting/hsCodeAddEdit.tpl.html',
            controller: 'HSCodeAddEditController',
            windowClass: '',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                HsCodeDetail : function () {
                    if (HSCode !== null && HSCode !== undefined) {
                        
                        return HSCode;
                    }
                    else {
                        return HSCode = {
                            HSCodeId : 0,
                            HSCode1: '',
                            Description: '',
                            Vat: '',
                            Duty: ''
                        };
                    }
                },
                mode: function () {
                    if (HSCode !== null && HSCode !== undefined) {
                        return "Edit";
                    }
                    else {
                        return "Add";
                    }
                },
                CountryList: function () {
                    return $scope.CountryList;
                }
            }
        });
        modalInstance.result.then(function () {
            $scope.HSCodeRecords();
        });
    };
    var getInitial = function () {
        ecommerceSettingService.GetCountryList().then(function (response) {
            $scope.CountryList = response.data;
            $scope.Country = $scope.CountryList[0];
            $scope.HSCodeRecords();
        }, function () {


        });
    };

    function init() {
        getInitial();
        setModalOptions();
    }
    init();
});