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

    //var GettrackingDetail = function (carriertype, id) {
    //    return $http.get(config.SERVICE_URL + '/Shipment/GetTrackingdeatil',
    //    {
    //        params: {
    //            CarrierName: carriertype,
    //            TrackingNumber: id
    //        }
    //    });
    //};

    var GettrackingDetail = function (carriertype, id) {
        return $http.get(config.SERVICE_URL + '/AftershipTracking/GetTracking',
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

    var GetThreeFuelSurCharge = function (OperationZoneId, Year) {
        return $http.get(config.SERVICE_URL + '/FuelSurCharge/GetThreeFuelSurCharge',
            {
                params: {
                    OperationZoneId: OperationZoneId,
                    Year: Year
                }
            });
    };

    var CurrentOperationZone = function () {
        return $http.get(config.SERVICE_URL + '/OperationZone/GetCurrentOperationZone');
    };

    var GetDateFromFile = function () {
        return $http.get(config.SERVICE_URL + '/Shipment/GetDateFormFile');
    };

    var GettrackingEcomm = function (carriertype, id) {
        
        return $http.get(config.SERVICE_URL + '/Shipment/GetTracking',
        {
            params: {
                CarrierName: carriertype,
                TrackingNumber: id
            }
        });
    };

    //var GetTradelaneShipmentDetail = function (FrayteNumber, FrayteType) {

    //    return $http.get(config.SERVICE_URL + '/TradelaneShipments/GetShipmentDetail',
    //    {
    //        params: {
    //            FrayteNumber: FrayteNumber,
    //            FrayteType: FrayteType
    //        }
    //    });
    //};

    var GetTradelaneShipmentDetail = function (FrayteNumber) {

        return $http.get(config.SERVICE_URL + '/Tracking/GetShipmentDetail',
        {
            params: {
                FrayteNumber: FrayteNumber
            }
        });
    };

    var GetShipmentTradelaneDetail = function (TradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + '/TradelaneUpdateTracking/GetShipmentDetail',
            {
                params: {
                    TradelaneShipmentId: TradelaneShipmentId
                }
            });
    };

    var GetUpdateTracking = function (TradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + '/TradelaneUpdateTracking/GetTradelaneShipmentTracking',
            {
                params: {
                    TradelaneShipmentId: TradelaneShipmentId
                }
            });
    };

    var GetDirectBookingDetail = function (FrayteRefNo) {
        return $http.get(config.SERVICE_URL + '/TradelaneBooking/GetDirectBookingShipment',
            {
                params: {
                    FrayteRefNo: FrayteRefNo
                   
                }
            });
    };

    var SearchTradelaneTracking = function (Num, NumType) {
        return $http.get(config.SERVICE_URL + '/TradelaneUpdateTracking/GetTradelaneTracking',
            {
                params: {
                    Number: Num,
                    NumberType: NumType
                }
            });
    };

    var SearchTracking = function (Num) {
        return $http.get(config.SERVICE_URL + '/TradelaneUpdateTracking/GetTracking',
            {
                params: {
                    Number: Num
                }
            });
    };

    var GetTrackingStatus = function (TradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + '/TradelaneUpdateTracking/GetShipmentTrackingStatus',
            {
                params: {
                    TradelaneShipmentId: TradelaneShipmentId
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
        IsManifestSupprt: IsManifestSupprt,
        GetThreeFuelSurCharge: GetThreeFuelSurCharge,
        CurrentOperationZone: CurrentOperationZone,
        GetDateFromFile: GetDateFromFile,
        GettrackingEcomm: GettrackingEcomm,
        GetTradelaneShipmentDetail: GetTradelaneShipmentDetail,
        GetShipmentTradelaneDetail: GetShipmentTradelaneDetail,
        GetUpdateTracking: GetUpdateTracking,
        GetDirectBookingDetail: GetDirectBookingDetail,
        SearchTradelaneTracking: SearchTradelaneTracking,
        GetTrackingStatus: GetTrackingStatus,
        SearchTracking: SearchTracking
    };
});