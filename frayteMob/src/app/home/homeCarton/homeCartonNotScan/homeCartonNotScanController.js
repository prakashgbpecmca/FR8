
angular.module('ngApp.home').controller('HomeCartonNotScanController', function ($scope, $uibModal, $uibModalInstance) {

    $scope.homeCarton = function () {
        $uibModalInstance.close('test');
    };
    $scope.notHome = function () {
        $uibModalInstance.close('testParameter');
    };

    function init() { }
    init();

});