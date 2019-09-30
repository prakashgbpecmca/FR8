angular.module('ngApp.currentShipment').controller('CurrentShipmentController', function ($rootScope, $scope, $state, $translate, $location, $filter, CustomerService, ShipmentService, SessionService, $uibModal, uiGridConstants, toaster, CountryService, ModalService) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteError', 'ErrorGetting', 'current', 'shipments', 'AWBConfirmation', 'MailSentToConsigneeGoodsPickup', 'Ok']).then(function (translations) {

            $scope.OkText = translations.Ok;
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TextHeaderAWBConfirm = translations.AWBConfirmation;
            $scope.TextBodyMailSentToConsigneeGoodsPickup = translations.MailSentToConsigneeGoodsPickup;
            $scope.TextErrorGetting = translations.ErrorGetting + " " + translations.current + " " + translations.shipments;            
        });
    };

    $scope.ShipmentDetail = function (row) {
        ShipmentService.GetShipmentDetail(row.entity.ShipmentId).then(function (response) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'shipment/shipmentDetail/shipmentDetail.tpl.html',
                controller: 'ShipmentDetailController',
                windowClass: '',
                size: 'lg',
                resolve: {
                    shipmentId: function () {
                        return row.entity.ShipmentId;
                    },
                    shipmentDetail: function () {
                        return response.data;
                    }
                }
            });
        });
    };

    $scope.DownloadShipmentDocuments = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'shipment/shipmentDocumentDownload/shipmentDocumentDownload.tpl.html',
            controller: 'ShipmentDocumentDownloadController',
            windowClass: '',
            size: 'md',
            resolve: {
                shipmentId: function () {
                    return row.entity.ShipmentId;
                }
            }
        });
    };

    $scope.UploadAWB = function (row) {

        var scope = $rootScope.$new();
        scope.params = { shipmentId: row.entity.ShipmentId };

        var modalInstance = $uibModal.open({
            animation: true,
            scope: scope,
            templateUrl: 'public/publicUploadAwb/uploadAwb.tpl.html',
            controller: 'UploadAWBController',
            windowClass: 'UploadAwb-Modal',
            size: 'md'
        });
    };

    $scope.UploadCommercialInvoice = function (row) {

        var scope = $rootScope.$new();
        scope.params = { shipmentId: row.entity.ShipmentId };

        var modalInstance = $uibModal.open({
            animation: true,
            scope: scope,
            templateUrl: 'public/shipmentDocument/shipmentDocument.tpl.html',
            controller: 'ShipmentDocumentController',
            windowClass: 'UploadAwb-Modal',
            size: 'lg'
        });
    };

    $scope.ConfirmAWB = function () {
        
        //Mail will be sent out on this action. 
        //No popup design for this.

        //Just show confirmation message.        
        ModalService.Alert({}, {
            headerText: $scope.TextHeaderAWBConfirm,
            bodyText: $scope.TextBodyMailSentToConsigneeGoodsPickup + '?',
            okText: $scope.OkText
        });
    };

    $scope.UpdateDropOffTime = function (row) {

        var scope = $rootScope.$new();
        scope.params = { shipmentId: row.entity.ShipmentId };

        var modalInstance = $uibModal.open({
            animation: true,
            scope: scope,
            templateUrl: 'public/publicShipmentDropOffTime/publicShipmentDropOffTime.tpl.html',
            controller: 'ShipmentDropOffTimeController',
            windowClass: 'ShipmentDropOffTime-Model',
            size: ''
        });
    };

    $scope.SetGridOptions = function () {
        $scope.gridOptions = {
            showFooter: true,
            enableSorting: true,
            multiSelect: false,
            enableFiltering: true,
            enableRowSelection: true,
            enableSelectAll: false,
            enableRowHeaderSelection: false,
            selectionRowHeaderWidth: 35,
            noUnselect: true,
            enableGridMenu: true,
            enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
            enableVerticalScrollbar: uiGridConstants.scrollbars.NEVER,
            columnDefs: [
              //{ name: 'ShipmentCode', displayName: 'S.O#' },
              { name: 'ShipmentId', displayName: 'SO', headerCellFilter: 'translate' },
              { name: 'Customer', headerCellFilter: 'translate' },
              { name: 'ShippedFrom', displayName: 'ShippedFrom', headerCellFilter: 'translate' },
              { name: 'ShippedTo', displayName: 'ShippedTo', headerCellFilter: 'translate' },
              //{ name: 'ShipmentDetail', displayName: 'Shipment Detail' },
              { name: 'ShippingType', displayName: 'ShippingType', headerCellFilter: 'translate' },
              { name: 'ShippingDate', displayName: 'ShippingDate', headerCellFilter: 'translate', cellFilter: 'dateFilter:this' },
              { name: 'Status', headerCellFilter: 'translate' },
              { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "shipment/currentShipment/currentShipmentEditButton.tpl.html", width: 65 }
            ]
        };
    };

    $scope.LoadCurrentShipments = function () {
        ShipmentService.GetCurrentShipment(1).then(function (response) {
            $scope.gridOptions.data = response.data;
            //For full List/Data in UI-GRID
            $scope.gridOptions.excessRows = $scope.gridOptions.data.length;
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGetting,
                showCloseButton: true
            });
        });
    };

    function init() {
        if ($state.is('shipper.current-shipment')){
            $scope.quickBooingIcon.value = true;
        }
        else {
            $scope.quickBooingIcon.value = false;
        }
        // set Multilingual Modal Popup Options
        setModalOptions();

        $scope.gridheight = SessionService.getScreenHeight();
        $scope.SetGridOptions();
        $scope.LoadCurrentShipments();
    }

    init();

});