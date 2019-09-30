
angular.module('ngApp.home').controller('HomeDispatchController', function ($scope, $uibModal) {
    $scope.dispatch = 'Dispatch';

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
            templateUrl: 'home/homeCarton/homeDispatch/homeCancel.tpl.html',
            controller: 'HomeCancelController',
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
            templateUrl: 'home/homeCarton/homeDispatch/homeConfirmDispatch.tpl.html',
            controller: 'HomeConfirmDispatchController',
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