

angular.module('ngApp.home').controller('HomeHandoverController', function ($scope, config, $uibModal) {
    $scope.handover = 'Handover';

    $scope.scanDriverManifest = function () {
        $scope.isActive = true;
        $scope.isScanBag = false;
        $scope.isTakePicture = false;
        $scope.isScanning = false;
        $scope.isScanAWB = false;
        $scope.isSignature = false;
    };
    $scope.scanBag = function () {
        $scope.isActive = false;
        $scope.isScanBag = true;
        $scope.isTakePicture = false;
        $scope.isScanning = false;
        $scope.isScanAWB = false;
        $scope.isSignature = false;
    };
    $scope.takePicture = function () {
        $scope.isActive = false;
        $scope.isScanBag = false;
        $scope.isTakePicture = true;
        $scope.isScanning = false;
        $scope.isScanAWB = false;
        $scope.isSignature = false;
    };
    $scope.scanning = function () {
        $scope.isActive = false;
        $scope.isScanBag = false;
        $scope.isTakePicture = false;
        $scope.isScanning = true;
        $scope.isScanAWB = false;
        $scope.isSignature = false;
    };
    $scope.scanAWB = function () {
        $scope.isActive = false;
        $scope.isScanBag = false;
        $scope.isTakePicture = false;
        $scope.isScanning = false;
        $scope.isScanAWB = true;
        $scope.isSignature = false;
    };
    $scope.signature = function () {
        $scope.isActive = false;
        $scope.isScanBag = false;
        $scope.isTakePicture = false;
        $scope.isScanning = false;
        $scope.isScanAWB = false;
        $scope.isSignature = true;
    };
    $scope.scanBagClick = function () {
        $scope.collapse = !$scope.collapse;
    };
    $scope.scanBagClick1 = function () {
        $scope.collapse1 = !$scope.collapse1;
    };
    $scope.scanBagClick2 = function () {
        $scope.collapse2 = !$scope.collapse2;
    };
    $scope.scanBagClick3 = function () {
        $scope.collapse3 = !$scope.collapse3;
    };

    //confirm code here
    $scope.confirm = function () {
        var ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'home/homeHandover/homeHandoverConfirm.tpl.html',
            controller: 'homeHandoverConfirmController',
            keyboard: true,
            windowClass: '',
            size: 'lg'
        });
        ModalInstance.result.then(function (ab) {
            if (ab === "test") {
                $scope.scanDriverManifest();
            }
        });
    };
    //end

    //confirm code here
    $scope.cancel = function () {
        var ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'home/homeHandover/homeHandoverCancel.tpl.html',
            controller: 'homeHandoverCancelController',
            keyboard: true,
            windowClass: '',
            size: 'lg'
        });
        ModalInstance.result.then(function (ab) {
            if (ab === "test1") {
                $scope.scanDriverManifest();
            }
        });
    };
    //end

    function init() {
        $scope.ImagePath = config.BUILD_URL;
        $scope.scanDriverManifest();
    }
    init();
});