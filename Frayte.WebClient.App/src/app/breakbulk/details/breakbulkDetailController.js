
angular.module('ngApp.breakBulk').controller("breakbulkDetailController", function ($scope, $uibModal, ModalService, config) {

    $scope.poShipment = function () {
        $scope.isPoShipment = true;
        $scope.isMultipleShipment = false;
        $scope.isMultiplePoShipment = false;
    };

    $scope.multipleShipment = function () {
        $scope.isMultipleShipment = true;
        $scope.isPoShipment = false;
        $scope.isMultiplePoShipment = false;
    };

    $scope.multiplePoShipment = function () {
        $scope.isMultiplePoShipment = true;
        $scope.isMultipleShipment = false;
        $scope.isPoShipment = false;
    };

    $scope.first = function () {
        $scope.isFirst = !$scope.isFirst;
        $scope.isSecond = false;
        $scope.isThird = false;
    };

    $scope.second = function () {
        $scope.isFirst = false;
        $scope.isSecond = !$scope.isSecond;
        $scope.isThird = false;
    };

    $scope.third = function () {
        $scope.isFirst = false;
        $scope.isSecond = false;
        $scope.isThird = !$scope.isThird;
    };

    $scope.shipmentOrderDetails = [
        { id: 1, jobno: '012345', jobname: 'Phone', order: '200', trackingNo:'0123456781',courier:'DHL Express', styleNo:'12345', description: 'Samsung Phone', purchaseOrder: '200', updatedOn: '28-May-2019', status: 'Order Placed', active: false },
        { id: 2, jobno: '012346', jobname: 'Watch', order: '40', trackingNo: '0123456782',courier: 'DHL Express', styleNo: '12346',description: 'Smart Watch', purchaseOrder: '40', updatedOn: '28-May-2019', status: 'Order Placed', active: false },
        { id: 3, jobno: '012347', jobname: 'Cases', order: '200', trackingNo: '0123456783', courier: 'DHL Express', styleNo: '12347',description: 'Phone Cases', purchaseOrder: '200', updatedOn: '28-May-2019', status: 'Order Placed', active: false }
    ];


    //breakbulk purchase order detail code
    $scope.purchaseOrderDetail = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            //controller: 'customerAddressBookAddEditController',
            templateUrl: 'breakbulk/details/breakbulkPurchaseOrderDetail.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end

    //view confirm purchase order detail code
    $scope.viewConfirmPurchaseOrder = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            //controller: 'customerAddressBookAddEditController',
            templateUrl: 'breakbulk/details/viewConfirmPurchaseOrder.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end

    //breakbulk tracking code
    $scope.breakbulkTracking = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'breakbulkTrackingController',
            templateUrl: 'breakbulk/breakBulkTracking/breakbulkTracking.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end


    function init() {
        $scope.ImagePath = config.BUILD_URL;
        $scope.isPoShipment = true;
        $scope.isFirst = true;
    }
    init();

});
