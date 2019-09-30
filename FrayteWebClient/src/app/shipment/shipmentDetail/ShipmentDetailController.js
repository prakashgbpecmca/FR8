/**
 * Controller
 */
angular.module('ngApp.shipment').controller('ShipmentDetailController', function ($scope, $state, $location, $stateParams, $filter, CustomerService, SessionService, $uibModal, uiGridConstants, toaster, ShipmentService, shipmentId, shipmentDetail) {
    $scope.getTotalPieces = function () {
        if ($scope.shipmentDetail === undefined) {
            return 0;
        }
        else if ($scope.shipmentDetail.ShipmentDetails === undefined) {
            return 0;
        }
        else if ($scope.shipmentDetail.ShipmentDetails.length >= 1) {
            var total = parseInt(0, 10);
            for (var i = 0; i < $scope.shipmentDetail.ShipmentDetails.length; i++) {
                var product = $scope.shipmentDetail.ShipmentDetails[i];
                if (product.Pieces === null || product.Pieces === undefined) {
                    total += parseInt(0, 10);
                }
                else {
                    total = total + parseInt(product.Pieces, 10);
                }
            }
            return total;
        }
        else {
            return 0;
        }
    };

    $scope.getTotalKgs = function () {
        if ($scope.shipmentDetail === undefined) {
            return 0;
        }
        else if ($scope.shipmentDetail.ShipmentDetails === undefined) {
            return 0;
        }
        else if ($scope.shipmentDetail.ShipmentDetails.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.shipmentDetail.ShipmentDetails.length; i++) {
                var product = $scope.shipmentDetail.ShipmentDetails[i];
                if (product.WeightKg === null || product.WeightKg === undefined) {
                    total += parseFloat(0);
                }
                else {
                    total = total + parseFloat(product.WeightKg);
                }
            }
            return total;
        }
        else {
            return 0;
        }
    };

    $scope.getTotalVolWeight = function () {
        if ($scope.shipmentDetail === undefined) {
            return 0;
        }
        else if ($scope.shipmentDetail.ShipmentDetails === undefined) {
            return 0;
        }
        else if ($scope.shipmentDetail.ShipmentDetails.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.shipmentDetail.ShipmentDetails.length; i++) {
                var product = $scope.shipmentDetail.ShipmentDetails[i];
                var len = 0;
                var wid = 0;
                var height = 0;
                if (product.Lcms === null || product.Lcms === undefined) {
                    len = parseFloat(0);
                }
                else {
                    len = parseFloat(product.Lcms);
                }

                if (product.Wcms === null || product.Wcms === undefined) {
                    wid = parseFloat(0);
                }
                else {
                    wid = parseFloat(product.Wcms);
                }

                if (product.Hcms === null || product.Hcms === undefined) {
                    height = parseFloat(0);
                }
                else {
                    height = parseFloat(product.Hcms);
                }

                if (len > 0 && wid > 0 && height > 0) {
                    total += ((len * wid * height) / 5000);
                }
            }
            return total;
        }
        else {
            return 0;
        }
    };

    function init() {
        $scope.ShipmentId = shipmentId;
        $scope.shipmentDetail = shipmentDetail;
    }

    init();
});