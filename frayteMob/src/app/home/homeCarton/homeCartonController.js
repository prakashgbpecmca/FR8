
angular.module('ngApp.home').controller('HomeCartonController', function ($scope, $uibModal) {

    $scope.noRecords = function () {
        $scope.firstActive = true;
        $scope.isActive = false;
        $scope.isOpenBag = false;
        $scope.isCloseBag = false;
        $scope.isCreateBag = false;
        $scope.isTntBag = false;
    };
    $scope.afterScanAWB = function () {
        $scope.isActive = true;
        $scope.isCreateBag = false;
        $scope.isOpenBag = false;
        $scope.isCloseBag = false;
        $scope.firstActive = false;
        $scope.isTntBag = false;
    };
    $scope.createBag = function () {
        $scope.isCreateBag = true;
        $scope.isActive = false;
        $scope.isOpenBag = false;
        $scope.isCloseBag = false;
        $scope.firstActive = false;
        $scope.isTntBag = false;
    };
    $scope.createBagCollapse = function () {
        $scope.createBagCollapseActive = !$scope.createBagCollapseActive;
    };
    $scope.openBag = function () {
        $scope.isOpenBag = true;
        $scope.isCloseBag = false;
        $scope.isCreateBag = false;
        $scope.firstActive = false;
        $scope.isTntBag = false;
    };
    $scope.closeBag = function () {
        $scope.isCreateBag = false;
        $scope.isCloseBag = true;
        $scope.isOpenBag = false;
        $scope.firstActive = false;
        $scope.isTntBag = false;
    };

    $scope.tntBag = function () {
        $scope.isActive = false;
        $scope.isCreateBag = false;
        $scope.isOpenBag = false;
        $scope.isCloseBag = false;
        $scope.firstActive = false;
        $scope.isTntBag = true;

    };


    //send print code here
    $scope.sendPrint = function () {
        var ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'home/homeCarton/homeSendPrint.tpl.html',
            controller: 'HomeSendPrintController',
            keyboard: true,
            windowClass: '',
            size: 'lg'
        });
    };
    //end

    //close popup code here
    $scope.closePopup = function () {
        var ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'home/homeCarton/homeCartonNotScan/homeCartonNotScan.tpl.html',
            controller: 'HomeCartonNotScanController',
            keyboard: true,
            windowClass: '',
            size: 'lg'
        });
        ModalInstance.result.then(function (ab) {
            if (ab === "test") {
                $scope.createBag();
            }
        });
    };
    //end


    function init() {
        $scope.isActive = false;
        $scope.isCreateBag = false;
        $scope.isOpenBag = false;
        $scope.isCloseBag = false;
        $scope.firstActive = true;
    }
    init();

});