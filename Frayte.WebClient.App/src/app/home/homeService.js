/**
 * Service
 */
angular.module('ngApp.home').factory('HomeService', function ($http, config, SessionService) {

    var bulkTracking = [];
    var ParcelHubTracking = function (carriertype,TrackingNo) {
        return $http.get(config.SERVICE_URL + '/Shipment/ParcelHubTracking',
        {
            params: {
                CarrierName: carriertype,
                TrackingNo: TrackingNo
            }
        });
    };
    var GettrackingDetail = function (carriertype, id) {
        return $http.get(config.SERVICE_URL + '/Shipment/GetTrackingdeatil',
        {
            params: {
                CarrierName: carriertype,
                TrackingNumber: id
            }
        });
    };
    var GetShipmentDetail = function (shipmentId) {
        return $http.get(config.SERVICE_URL + '/Shipment/GetShipmentDetail',
            {
                params: {
                    shipmentId: shipmentId
                }
            });
    };

    var GetBulkTrackingDetails = function (BulkTrackingDetails) {
        return $http.post(config.SERVICE_URL + '/Shipment/GetBulkTrackingDetails', BulkTrackingDetails);
    };
    var GetCarriers = function () {
        return $http.get(config.SERVICE_URL + '/Courier/GetUKCourier');
    };
    var GetCurrentOperationZone = function () {
        return $http.get(config.SERVICE_URL + '/OperationZone/GetCurrentOperationZone');
    };
    var IsManifestSupprt = function (userId) {
        return $http.get(config.SERVICE_URL + '/Manifest/IsManifestSupprt', {
            params: {
                userId :userId
            }
        });
    };
    
    return {
        GettrackingDetail: GettrackingDetail,
        GetCarriers: GetCarriers,
        GetBulkTrackingDetails: GetBulkTrackingDetails,
        bulkTracking: bulkTracking,
        ParcelHubTracking: ParcelHubTracking,
        GetCurrentOperationZone: GetCurrentOperationZone,
        IsManifestSupprt: IsManifestSupprt
    };
});