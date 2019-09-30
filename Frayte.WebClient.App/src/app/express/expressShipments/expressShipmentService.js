angular.module('ngApp.express').factory("ExpressShipmentService", function ($http, config) {

    var GetExpressStatusList = function (BookingType) {
        return $http.get(config.SERVICE_URL + '/ExpressShipment/GetExpressStatusList',
            {
                params: {
                    BookingType: BookingType
                }
            });
    };

    var GetExpressShipments = function (track) {
        return $http.post(config.SERVICE_URL + '/ExpressShipment/GetExpressShipments', track);
    };

    var GetExpressCustomers = function (RoleId, UserId) {
        return $http.get(config.SERVICE_URL + '/ExpressShipment/GetExpressCustomers',
            {
                params: {
                    RoleId: RoleId,
                    UserId: UserId
                }
            });
    };

    var DeleteExpressShipment = function (ExpressShipmentId) {
        return $http.get(config.SERVICE_URL + '/ExpressShipment/DeleteExpressShipment',
            {
                params: {
                    ExpressShipmentId: ExpressShipmentId
                }
            });
    };

    var GetExpressShipmentTracking = function (ExpressShipmentId) {
        return $http.get(config.SERVICE_URL + '/ExpressShipment/GetExpressAWBTracking',
            {
                params: {
                    ExpressShipmentId: ExpressShipmentId
                }
            });
    };

    var GetExpressBagTracking = function (BagId) {
        return $http.get(config.SERVICE_URL + '/ExpressShipment/GetExpressBagTracking',
            {
                params: {
                    BagId: BagId
                }
            });
    };
           
    var GetExpressTracking = function (Number) {
        return $http.get(config.SERVICE_URL + '/ExpressShipment/GetExpressTracking',
            {
                params: {
                    Number: Number
                }
            });
    };

    var GenerateTrackAndTraceReport = function (track) {
        return $http.post(config.SERVICE_URL + '/Express/GenerateExpressTrackAndTraceExcel', track);
    };

    return {
        GetExpressStatusList: GetExpressStatusList,
        GetExpressShipments: GetExpressShipments,
        GetExpressCustomers: GetExpressCustomers,
        DeleteExpressShipment: DeleteExpressShipment,
        GetExpressShipmentTracking: GetExpressShipmentTracking,
        GetExpressBagTracking: GetExpressBagTracking,
        GetExpressTracking: GetExpressTracking,
        GenerateTrackAndTraceReport: GenerateTrackAndTraceReport
    };
});