angular.module('ngApp.breakBulk').controller("breakbulkManifestGeneratorController", function ($scope, $uibModal, ModalService, config) {

    //breakbluk create tradelane shipment code here
    $scope.createTradelaneShipment = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'createTradelaneShipmentControlller',
            templateUrl: 'breakbulk/createTradelaneShipment/createTradelaneShipment.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg'
        });
    };
    //end

    $scope.createManifest = [
        { id: 1, customer: 'SERIOUS STUFF LIMITED', poNo: 'PO012347', jobNo: '012347', trackingNo: '125 256 85', carrier: 'Delivered', grossWeight: '5 kgs/8 shipments', createdOn: '29-May-2019', active: false },
        { id: 2, customer: 'SERIOUS STUFF LIMITED', poNo: 'PO012347', jobNo: '012347', trackingNo: '125 256 85', carrier: 'Delivered', grossWeight: '5 kgs/8 shipments', createdOn: '29-May-2019', active: false },
        { id: 3, customer: 'SERIOUS STUFF LIMITED', poNo: 'PO012347', jobNo: '012347', trackingNo: '125 256 85', carrier: 'Delivered', grossWeight: '5 kgs/8 shipments', createdOn: '29-May-2019', active: false },
        { id: 4, customer: 'SERIOUS STUFF LIMITED', poNo: 'PO012347', jobNo: '012347', trackingNo: '125 256 85', carrier: 'Delivered', grossWeight: '5 kgs/8 shipments', createdOn: '29-May-2019', active: false },
        { id: 5, customer: 'SERIOUS STUFF LIMITED', poNo: 'PO012347', jobNo: '012347', trackingNo: '125 256 85', carrier: 'Delivered', grossWeight: '5 kgs/8 shipments', createdOn: '29-May-2019', active: false },
        { id: 6, customer: 'SERIOUS STUFF LIMITED', poNo: 'PO012347', jobNo: '012347', trackingNo: '125 256 85', carrier: 'Delivered', grossWeight: '5 kgs/8 shipments', createdOn: '29-May-2019', active: false }
    ];



    //checkbox check code

    $scope.CheckUncheckHeader = function () {
        $scope.IsAllChecked = true;
        for (var i = 0; i < $scope.createManifest.length; i++) {
            if (!$scope.createManifest[i].active) {
                $scope.IsAllChecked = false;
                break;
            }
        }
    };
    $scope.CheckUncheckHeader();

    $scope.CheckUncheckAll = function () {
        for (var i = 0; i < $scope.createManifest.length; i++) {
            $scope.createManifest[i].active = $scope.IsAllChecked;
        }
    };

    //end



    function init() {
        $scope.ImagePath = config.BUILD_URL;
    }
    init();



});




