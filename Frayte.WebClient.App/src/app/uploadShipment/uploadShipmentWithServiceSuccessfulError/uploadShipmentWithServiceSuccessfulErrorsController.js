angular.module('ngApp.uploadShipment').controller('WithServiceSuccessfulErrorsController', function ($scope, toaster, $translate, Upload, uiGridConstants, DownloadExcelService, config, $state, SessionService, UploadShipmentService, $uibModal, ShipmentData) {
    

    $scope.GetErrorsForWithServiceShipments = function (Shipment) {
        UploadShipmentService.GetErrorDetail(Shipment.ShipmentId, "ECOMMERCE_SS").then(function (Response) {
            $scope.ErrorList = Response.data;
        },
        function () {

        });
    };


    function init() {
        $scope.ShipmentData = ShipmentData;
        $scope.GetErrorsForWithServiceShipments($scope.ShipmentData);
    }

    init();

});