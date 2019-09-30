

angular.module('ngApp.home').controller('HomeCollectionDriverHandoverController', function ($scope, config, $uibModal) {

    $scope.handover = function () {
        $scope.isHandover = true;
        $scope.isEditTakePicture = false;
        $scope.isSkipupload = false;
        $scope.isTakePicture = false;
        $scope.isActive = false;
    };

    //scanning awb code here
    $scope.scanningAWB = function () {
        var ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'home/homeCollectionDriver/homeCollectionDriverHandover/homeCollectionDriverHandoverScanning.tpl.html',
            controller: 'HomeCollectionDriverHandoverScanningController',
            keyboard: true,
            windowClass: '',
            size: 'lg'
        });
    };
    //end

    //cancel code here
    $scope.cancelHandover = function () {
        var ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'home/homeCollectionDriver/homeCollectionDriverHandover/homeCollectionDriverHandoverCancel.tpl.html',
            controller: 'HomeCollectionDriverHandoverCancelController',
            keyboard: true,
            windowClass: '',
            size: 'lg'
        });
    };
    //end

    function init() {
        $scope.ImagePath = config.BUILD_URL;
        $scope.isHandover = true;
    }
    init();
});