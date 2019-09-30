
angular.module('ngApp.home').controller('HomeFuelSurChargeController', function ($scope, $location, $anchorScroll, $state, $stateParams, config, $filter, HomeService, SessionService, $uibModal, $log, toaster, $translate) {
    var setModalOptions = function () {
        $translate(['Address', 'Book', 'DeleteHeader', 'DeleteBody', 'Tradelane', 'SuccessfullySavedInformation', 'FrayteInformation',
            'FrayteValidation', 'PleaseCorrectValidationErrors', 'FrayteError', 'ErrorSavingRecord', 'ErrorGetting', 'customer',
            'detail', 'TrackingDetails_Validation', 'records', 'FrayteSuccess']).then(function (translations) {
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
                $scope.TrackingDetailsValidation = translations.TrackingDetails_Validation;
                $scope.TextErrorGettingRecords = translations.ErrorGetting + " " + translations.records;
                $scope.FrayteSuccess = translations.FrayteSuccess;
            });
    };
    function opezone() {
        HomeService.CurrentOperationZone().then(function (response) {
            if (response.status = 200) {
                if (response.data !== null) {
                    $scope.OperationZone = response.data;
                    getfuel();
                }

            }

        });
    }

    $scope.getFuelMonthYear = function (FuelMonthYear) {
        if (FuelMonthYear !== undefined && FuelMonthYear !== null) {
            var date = new Date(FuelMonthYear);
            var days = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
            var getmn1 = days[date.getMonth()];
            //var getmn = ++getmn1;
            var getyr = date.getFullYear();
            var strDate = getmn1 + " " + getyr;
            return strDate;
        }
    };

    function getfuel() {

        HomeService.GetThreeFuelSurCharge($scope.OperationZone.OperationZoneId, new Date()).then(function (response) {

            if (response.status = 200) {
                if (response.data !== null && response.data.length > 0) {
                    $scope.FuelSurCharges = response.data;
                }

            }
            else {
                toaster.pop({
                    type: 'success',
                    title: $scope.FrayteSuccess,
                    body: $scope.TextErrorGettingRecords,
                    showCloseButton: true
                });
            }

        });
    }

    function init() {
        $scope.val = false;
        opezone();
        setModalOptions();
    }

    init();
});