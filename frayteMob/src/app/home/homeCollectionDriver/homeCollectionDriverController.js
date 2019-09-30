
angular.module('ngApp.home').controller('HomeCollectionDriverController', function ($scope, config, $uibModal) {
    $scope.scanAwb = 'Scan AWB';

    $scope.isSkipuploadClick = function () {
        $scope.isSkipupload = true;
        $scope.isEditTakePicture = false;
        $scope.isTakePicture = false;
        $scope.isHandover = false;
    };

    $scope.isTakePictureClick = function () {
        $scope.isTakePicture = true;
        $scope.isEditTakePicture = false;
        $scope.isSkipupload = false;
        $scope.isHandover = false;
    };

    $scope.isEditTakePictureClick = function () {
        $scope.isEditTakePicture = true;
        $scope.isTakePicture = false;
        $scope.isSkipupload = false;
        $scope.isHandover = false;
    };
   
    $scope.noRecords = function () {
        $scope.isActive = true;
        $scope.isTakePicture = false;
        $scope.isHandover = false;
        $scope.isEditTakePicture = false;
        $scope.isSkipupload = false;
    };

    $scope.takePicture = function () {
        $scope.isTakePicture = true;
        $scope.isActive = false;
        $scope.isHandover = false;
        $scope.isEditTakePicture = false;
        $scope.isSkipupload = false;
    };

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
            templateUrl: 'home/homeCollectionDriver/homeCollectionDriverScanning.tpl.html',
            controller: 'HomeCollectionDriverScanningController',
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
            templateUrl: 'home/homeCollectionDriver/homeCollectionDriverCancel.tpl.html',
            controller: 'HomeCollectionDriverCancelController',
            keyboard: true,
            windowClass: '',
            size: 'lg'
        });
    };
    //end

    function init() {
        $scope.ImagePath = config.BUILD_URL;
        $scope.isActive = true;
    }
    init();

});