angular.module('ngApp.pastShipment').controller('PastShipmentController', function ($scope, $state, $location, $filter, $translate, CustomerService, SessionService, $uibModal, uiGridConstants, toaster, CountryService, ShipmentService) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteError', 'ErrorGetting', 'past', 'shipments']).then(function (translations) {
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TextErrorGetting = translations.ErrorGetting + " " + translations.past + " " + translations.shipments;
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
              { name: 'DateOfDelivery', displayName: 'DateOfDelivery', headerCellFilter: 'translate', cellFilter: 'dateFilter:this' },
              { name: 'Status', headerCellFilter: 'translate' },
              { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "shipment/pastShipment/pastShipmentEditButton.tpl.html", width: 65 }
            ]
        };
    };

    $scope.LoadPastShipments = function () {
        ShipmentService.GetPastShipment(1).then(function (response) {
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
        // set Multilingual Modal Popup Options
        setModalOptions();
        if ('shipper.past-shipment') {
            $scope.quickBooingIcon.value = false;
        }
        $scope.gridheight = SessionService.getScreenHeight();
        $scope.SetGridOptions();
        $scope.LoadPastShipments();
    }

    init();

});