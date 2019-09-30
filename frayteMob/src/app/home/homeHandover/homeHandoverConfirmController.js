

angular.module('ngApp.home').controller('homeHandoverConfirmController', function ($scope, $uibModal, $uibModalInstance) {
    $scope.confirm = 'Confirm';

    $scope.scanDriverManifest = function () {
        $uibModalInstance.close('test');
    };
    $scope.notRedirect = function () {
        $uibModalInstance.close('testParameter');
    };

    function init(){}
    init();

});