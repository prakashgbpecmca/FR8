
angular.module('ngApp.home').controller('HomeBarcodeCancelController', function ($scope, $uibModal, $uibModalInstance) {

    $scope.scanAwb = function () {
        $uibModalInstance.close('test');
    };
    $scope.notHome = function () {
        $uibModalInstance.close('testParameter');
    };

    function init() { }
    init();

});