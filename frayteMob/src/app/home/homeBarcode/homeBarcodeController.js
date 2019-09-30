
angular.module('ngApp.home').controller('HomeBarcodeController', function ($scope, $uibModal) {

    $scope.scanAwb = function () {
        $scope.driver = true;
    };
    $scope.receiving = function () {
        $scope.driver = false;
    };

    $scope.firstDriverClick = function () {
        $scope.firstDriver = !$scope.firstDriver;
    };
    $scope.secondDriverClick = function () {
        $scope.secondDriver = !$scope.secondDriver;
    };

    //receiving confirm code here
    $scope.receivingConfirm = function () {
        var ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'home/homeBarcode/homeBarcodeConfirm.tpl.html',
            controller: 'HomeBarcodeConfirmController',
            keyboard: true,
            windowClass: '',
            size: 'lg'
        });
    };
    //end

    //receiving confirm code here
    $scope.cancel = function () {
        var ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'home/homeBarcode/homeBarcodeCancel.tpl.html',
            controller: 'HomeBarcodeCancelController',
            keyboard: true,
            windowClass: '',
            size: 'lg'
        });
        ModalInstance.result.then(function (ab) {
            if (ab === "test") {
                $scope.scanAwb();
            }
        });
    };
    //end


    function init() {
        $scope.on = false;
        $scope.firstDriver = true;
        $scope.scanAwb();
    }
    init();

});