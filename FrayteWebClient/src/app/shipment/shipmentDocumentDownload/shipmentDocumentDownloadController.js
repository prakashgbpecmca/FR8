angular.module('ngApp.pastShipment').controller('ShipmentDocumentDownloadController', function ($scope, $state, $translate, $location, $stateParams, $filter, CustomerService, SessionService, $uibModal, uiGridConstants, toaster, ShipmentService, shipmentId) {
    
    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteError', 'ErrorGetting', 'past', 'shipments']).then(function (translations) {
            $scope.TitleFrayteError = translations.FrayteError;

            $scope.TextErrorGettingShipmentRecord = translations.ErrorGetting + " " + translations.shipment + " " + translations.document + " " + translations.records;
        });
    };

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
          { name: 'SN', displayName: 'SN', headerCellFilter: 'translate', enableFiltering: false, width: 50 },
          { name: 'DocumentType', displayName: 'DocumentType', headerCellFilter: 'translate' },
          { name: 'DocumentTitle', displayName: 'Document', headerCellFilter: 'translate' },
          { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "shipment/shipmentDocumentDownload/shipmentDocumentDownloadLink.tpl.html", width: 100 }
        ]
    };

    $scope.RearrangeSerialNumbers = function (collectionObject) {

        if (collectionObject.length > 0) {
            for (var i = 0; i < collectionObject.length; i++) {
                collectionObject[i].SN = i + 1;
            }
        }

        return collectionObject;
    };

    $scope.LoadPastShipmentDownloads = function (shipmentId) {
        ShipmentService.GetShipmentDocuments(shipmentId).then(function (response) {
            //Need to crete SN
            var snData = $scope.RearrangeSerialNumbers(response.data);
            $scope.gridOptions.data = snData;
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGettingShipmentRecord,
                showCloseButton: true
            });
        });
    };

    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();

        $scope.ShipmentId = shipmentId;
        $scope.LoadPastShipmentDownloads($scope.ShipmentId);

    }

    init();

});
