angular.module('ngApp.breakBulk').controller("selectCustomersController", function ($scope, $uibModal, ModalService, config) {
    $scope.check = false;
  
    $scope.breakbulkCollapse = function () {
        $scope.breakbulkShow = !$scope.breakbulkShow;
    };

    $scope.addressCollapse = function () {
        $scope.addressShow = !$scope.addressShow;
    };

    $scope.customerInfoClick=function(){
        $scope.customerInfo = true;
        $scope.purchaseOrderInfo = false;
        $scope.saveInfo = false;
    };

    $scope.purchaseOrderInfoClick = function () {
        $scope.purchaseOrderInfo = true;
        $scope.customerInfo = false;
        $scope.saveInfo = false;
    };

    $scope.saveOrderInfoClick = function () {
        $scope.saveInfo = true;
        $scope.customerInfo = false;
        $scope.purchaseOrderInfo = false;
    };

    $scope.gridCheck = function () {
        $scope.grid = !$scope.grid;
    };

    $scope.checkClick = function () {
        $scope.check = !$scope.check;
    };

    //add-edit purchase order code
    $scope.addEditPurchaseOrder = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'breakbulkBookingAddEditPurchaseOrderController',
            templateUrl: 'breakbulk/breakbulkPurchaseOrder/breakbulkBookingAddEditPurchaseOrder.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end

    //confirm code
    $scope.confirmClick = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            //controller: 'customerAddressBookAddEditController',
            templateUrl: 'breakbulk/confirmationPopup/confirmationPopup.tpl.html',
            keyboard: true,
            windowClass: '',
            size: 'md',
            backdrop: 'static'
        });
    };
    //end


   //data code here
    $scope.companyName = [
        { id: 1, companyName: 'Irays SoftTech Pvt Ltd.', customerName: 'Pushkar Thapliyal', check:true },
        { id: 2, companyName: 'Hindustan Pvt Ltd.', customerName: 'Manish Jadli', check: false },
        { id: 3, companyName: 'Bharat Heavy Electronic Ltd.', customerName: 'Ram Ramacharya', check: false },
        { id: 4, companyName: 'Dabur Pvt Ltd.', customerName: 'Raman Thundr', check: false },
        { id: 5, companyName: 'Whytelcliff Pvt Ltd.', customerName: 'Jack Sam', check: false },
        { id: 6, companyName: 'Cliff Premiums Pvt Ltd.', customerName: 'Alex Ratherford', check: false },
        { id: 7, companyName: 'FRAYTE Logistics Ltd.', customerName: 'John Peterson', check: false },
        { id: 8, companyName: 'Lionstar Manufacturing Ltd.', customerName: 'Max Wilermuth', check: false },
        { id: 9, companyName: 'Star Jin Hui Garment Company Ltd.', customerName: 'Marshal Stepherd', check: false },
        { id: 10, companyName: 'Vytal Support (Hong Kong) Ltd.', customerName: 'Alex Brooke', check: false },
        { id: 11, companyName: 'Vytal Support (Thailand) Ltd.', customerName: 'Jack Karlis', check: false },
        { id: 12, companyName: 'Whytelcliff Consultants Ltd.', customerName: 'Stepherd Johnson', check: false },
        { id: 13, companyName: 'Whytelcliff Group Ltd.', customerName: 'Trnyp Hook', check: false },
        { id: 14, companyName: 'Childs Own Ltd.', customerName: 'Illena Dcruze', check: false },
        { id: 15, companyName: 'Irays SoftTech Pvt Ltd.', customerName: 'Berlin Zrerk', check: false }
    ];
    //end


    function init() {
        $scope.ImagePath = config.BUILD_URL;
        $scope.customerInfo = true;
        $scope.breakbulkShow = false;
        $scope.addressShow = false;
    
        //$scope.check = false;
    }
    init();
});