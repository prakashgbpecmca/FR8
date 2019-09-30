angular.module('ngApp.breakBulk').controller("breakbulkCreateCartonController", function ($scope, $uibModal) {

    //breakbulk create carton size code
    $scope.createCartonSize = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'BreakbulkCreateCartonSizeController',
            templateUrl: 'breakbulk/details/breakbulkCreateCartonSize.tpl.html',
            keyboard: true,
            windowClass: '',
            size: 'md',
            backdrop: 'static'
        });
    };
    //end  

    //breakbulk add product code
    $scope.breakbulkAddProduct = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'BreakbulkProductAddEditController',
            templateUrl: 'breakbulk/details/breakbulkProductAddEdit.tpl.html',
            keyboard: true,
            windowClass: '',
            size: 'md',
            backdrop: 'static'
        });
    };
    //end 

    //breakbulk po add edit code
    $scope.breakbulkAddEditPo = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'BreakbulkPOAddEditController',
            templateUrl: 'breakbulk/breakbulkPO/breakbulkPOAddEdit.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end 

    //breakbulk product catalog code
    $scope.productCatalog = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'ProductCatalogAddEditController',
            templateUrl: 'breakbulk/productCatalog/productCatalogAddEdit.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end 

    //breakbulk map product catalog code
    $scope.mapProductCatalog = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'BreakbulkMapAddEditController',
            templateUrl: 'breakbulk/details/breakbulkMapAddEdit.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end 

    function init() { }
    init();

});