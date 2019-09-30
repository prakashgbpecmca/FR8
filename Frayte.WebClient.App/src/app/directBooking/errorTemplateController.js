angular.module('ngApp.directBooking').controller('errorTemplateController', function ($scope, FrayteError, $uibModalInstance, $state, $uibModal, $stateParams) {
    
   // $uibModalInstance.close();
    function init() {

        $scope.FrayteError = FrayteError;
    }

    init();
});