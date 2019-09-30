angular.module('ngApp.ecommerceSetting').controller('HSCodeAddEditController', function ($scope, ecommerceSettingService, $uibModal, HsCodeDetail, mode, CountryList, toaster, $uibModalInstance, $translate) {

    var setModalOptions = function () {
        $translate(['Address', 'Book', 'DeleteHeader', 'DeleteBody', 'Tradelane', 'SuccessfullySavedInformation', 'FrayteInformation', 'FrayteError', 'ErrorSavingRecord', 'ErrorGetting',
            'records', 'CustomerDelete_Validation', 'Delete', 'Confirmation', 'HS_Code', 'AHs_Record', 'Error_While_Deleting_The_Records', 'Record_Deleted_Successfully']).then(function (translations) {
                $scope.TitleFrayteInformation = translations.FrayteInformation;
                $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
                $scope.TitleFrayteError = translations.FrayteError;
                $scope.TextSavingError = translations.ErrorSavingRecord;
                $scope.RecordGettingError = translations.ErrorGetting + " " + translations.records;
            });
    };



    $scope.HSCodeAddEdit = function (AddressBookForm, HSCodeData) {
        if (AddressBookForm === true)
        {
            ecommerceSettingService.AddEditHSCode($scope.HsCodeDetail).then(function (response) {

                $scope.HSCodeData =response.data;
                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteInformation,
                    body: $scope.TextSuccessfullySavedInformation,
                    showCloseButton: true
                });
                $uibModalInstance.close();
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.RecordGettingError,
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        }
    };

   

    var getInitial = function () {

    };
    function init() {
        $scope.mode = mode;
        $scope.CountryList = CountryList;
        $scope.HsCodeDetail = HsCodeDetail;
        setModalOptions();
        getInitial();
    }

    init();

});