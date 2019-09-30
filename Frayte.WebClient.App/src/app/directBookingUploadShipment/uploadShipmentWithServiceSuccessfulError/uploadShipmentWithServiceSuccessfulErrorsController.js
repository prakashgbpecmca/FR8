angular.module('ngApp.directBookingUploadShipment').controller('GetServiceErrorsController', function ($scope, toaster, $translate, Upload, uiGridConstants, DownloadExcelService, config, $state, SessionService, UploadShipmentService, $uibModal, ShipmentData, DbUploadShipmentService) {


    $scope.GetErrorsForWithServiceShipments = function (Shipment) {
        $scope.ErrorDetailList = [];
        DbUploadShipmentService.GetFrayteError(Shipment.ShipmentId).then(function (Response) {
            $scope.ErrorList = Response.data;
            if ($scope.ErrorList) {
                if ($scope.ErrorList.Address != null && $scope.ErrorList.Address.length > 0) {
                    for (i = 0; i < $scope.ErrorList.Address.length; i++) {
                        $scope.ErrorDetailList.push($scope.ErrorList.Address[i].Value[0]);
                    }
                }
                if ($scope.ErrorList.Package != null && $scope.ErrorList.Package.length > 0) {
                    for (j = 0; j < $scope.ErrorList.Package.length; j++) {
                        $scope.ErrorDetailList.push($scope.ErrorList.Package[j].Value[0]);
                    }
                }
                if ($scope.ErrorList.Custom != null && $scope.ErrorList.Custom.length > 0) {
                    for (k = 0; k < $scope.ErrorList.Custom.length; k++) {
                        $scope.ErrorDetailList.push($scope.ErrorList.Custom[k].Value[0]);
                    }
                }
                if ($scope.ErrorList.Service != null && $scope.ErrorList.Service.length > 0) {
                    for (l = 0; l < $scope.ErrorList.Service.length; l++) {
                        $scope.ErrorDetailList.push($scope.ErrorList.Service[l].Value[0]);
                    }
                }
                if ($scope.ErrorList.ServiceError != null && $scope.ErrorList.ServiceError.length > 0) {
                    for (m = 0; m < $scope.ErrorList.ServiceError.length; m++) {
                        $scope.ErrorDetailList.push($scope.ErrorList.ServiceError[m].Value[0]);
                    }
                }
                if ($scope.ErrorList.MiscErrors != null && $scope.ErrorList.MiscErrors.length > 0) {
                    for (n = 0; n < $scope.ErrorList.MiscErrors.length; n++) {
                        $scope.ErrorDetailList.push($scope.ErrorList.MiscErrors[n].Value[0]);
                    }
                }
                if ($scope.ErrorList.Miscellaneous != null && $scope.ErrorList.Miscellaneous.length > 0) {
                    for (o = 0; o < $scope.ErrorList.Miscellaneous.length; o++) {
                        $scope.ErrorDetailList.push($scope.ErrorList.Miscellaneous[o].Value[0]);
                    }
                }
            }
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