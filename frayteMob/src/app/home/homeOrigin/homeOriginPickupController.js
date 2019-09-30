
angular.module('ngApp.home').controller('HomeOriginPickupController', function ($scope, config, $uibModal) {
    $scope.pickUp = 'Pick Up';
    $scope.scanExportManifest = function () {
        $scope.isActive = true;
    };

    $scope.scanBag = function () {
        $scope.isActive = false;
    };

    //cancel code here
    $scope.cancelBag = function () {
        var ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'home/homeOrigin/homeOriginCancel.tpl.html',
            controller: 'HomeOriginCancelController',
            keyboard: true,
            windowClass: '',
            size: 'lg'
        });
    };
    //end

    //confirm dispatch code here
    $scope.confirmDispatch = function () {
        var ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'home/homeOrigin/homeOriginConfirm.tpl.html',
            controller: 'HomeOriginConfirmController',
            keyboard: true,
            windowClass: '',
            size: 'lg'
        });
    };
    //end


    function init() {
        $scope.scanExportManifest();
    }
    init();
});