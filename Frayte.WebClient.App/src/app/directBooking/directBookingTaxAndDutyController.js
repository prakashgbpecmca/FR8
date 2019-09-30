angular.module('ngApp.directBooking').controller('DirectBookingTaxAndDutyController', function ($uibModalInstance, $rootScope, AppSpinner, $sce, TopCountryService, $location, $anchorScroll, TopCurrencyService, $scope, config, $filter, $state, ModalService, $translate, $uibModal, $timeout, toaster, $stateParams, DirectBookingService, SessionService) {
    
    var setMultilingualOptions = function () {
        $translate(['FrayteWarning_Validation', 'PleaseCorrectValidationErrors']).then(function (translations) {
            $scope.FrayteWarning_Validation = translations.FrayteWarning_Validation;
            $scope.PleaseCorrectValidationErrors = translations.PleaseCorrectValidationErrors;
        });
    };


    $scope.submit = function (IsValid) {
        if (IsValid) {
            $uibModalInstance.close($scope.TaxDutyName);
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning_Validation,
                body: $scope.PleaseCorrectValidationErrors,
                showCloseButton: true
            });
        }
        
    };

    function init() {
        $scope.TaxDutyName = '';
        setMultilingualOptions();
    }

    init();
});
