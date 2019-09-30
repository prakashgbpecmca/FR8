
angular.module('ngApp.breakBulk').controller("BreakbulkPurchaseOrderDetail", function ($scope, $uibModal, ModalService, config) {

    $scope.generateLabelClick = function (id) {
        //$scope.isGenerateLabel = !$scope.isGenerateLabel;
        $scope.isGenerateLabel = ($scope.isGenerateLabel == id) ? -1 : id;
    };

    $scope.generateLabels = [
        {
            id: 1, po: '01234', jobs: '4', description: 'Delivered', active: false, collaspe: [
                { id: 1, po: '01234', jobs: '1', description: 'Delivered', active: false },
                { id: 2, po: '01234', jobs: '1', description: 'Delivered', active: false },
                { id: 3, po: '01234', jobs: '1', description: 'Delivered', active: false },
                { id: 4, po: '01234', jobs: '1', description: 'Delivered', active: false }
            ]
        },
        {
            id: 1, po: '01235', jobs: '1', description: 'Delivered', active: false, collaspe: [
                { id: 1, po: '01235', jobs: '1', description: 'Delivered', active: false }
            ]
        },
        {
            id: 1, po: '01236', jobs: '2', description: 'Delivered', active: false, collaspe: [
                { id: 1, po: '01236', jobs: '1', description: 'Delivered', active: false },
                { id: 2, po: '01236', jobs: '1', description: 'Delivered', active: false }
            ]
        }
    ];
        

    //breakbulk get service order code
    $scope.breakbulkGetService = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'BreakbulkGetserviceController',
            templateUrl: 'breakbulk/breakbulkGetservice/breakbulkGetservice.tpl.html',
            keyboard: true,
            windowClass: '',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end

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

    function init() {
        $scope.ImagePath = config.BUILD_URL;
        $scope.photoHazard = config.BUILD_URL + "Hazard_logo.png";
    }
    init();

});