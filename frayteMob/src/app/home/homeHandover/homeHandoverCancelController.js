
angular.module('ngApp.home').controller('homeHandoverCancelController', function ($scope, $uibModal, $uibModalInstance) {

    $scope.scanDriverManifest = function () {
        $uibModalInstance.close('test1');
    };
    $scope.notRedirected = function () {
        $uibModalInstance.close('testParameter1');
    };

    function init() {

    }
    init();
});