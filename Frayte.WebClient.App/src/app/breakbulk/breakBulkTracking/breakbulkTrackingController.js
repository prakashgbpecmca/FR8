
angular.module('ngApp.breakBulk').controller("breakbulkTrackingController", function ($scope, $uibModal, ModalService, config) {

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

    //breakbulkBookingCustomers code here
    $scope.breakbulkBookingCustomers = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'selectCustomersController',
            templateUrl: 'breakbulk/selectCustomers/selectCustomers.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end
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

    //breakbulk shipment detail code here
    $scope.breakbulkShipmentDetail = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'breakbulkDetailController',
            templateUrl: 'breakbulk/details/breakbulkShipmentDetail.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg'
        });
    };
    //end

    //breakbulk shipment detail code here
    $scope.BreakBulkTrackingDetail = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'breakbulkTrackingDetailController',
            templateUrl: 'breakbulk/trackingDetails/breakbulkShipmentDetail.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg'
        });
    };
    //end


    //end

    function init() {
        $scope.ImagePath = config.BUILD_URL;
    }
    init();

});