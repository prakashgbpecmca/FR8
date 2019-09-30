/**
 * Service
 */
angular.module('ngApp.mileStone').factory('TradelaneMilestoneService', function ($http, config, SessionService) {

    var GetTrackingMileStone = function (ShipmentHandlerMethodId) {
        return $http.get(config.SERVICE_URL + '/TrackingMileStone/GetTrackingMileStone',
            {
                params: {
                    ShipmentHandlerMethodId: ShipmentHandlerMethodId
                }
            });
    };

    var GetShipmentHandlerMethods = function () {
        return $http.get(config.SERVICE_URL + '/TrackingMileStone/GetShimentHandlerMethods');
    };

    var SaveMilestone = function (MileStone) {
        return $http.post(config.SERVICE_URL + '/TrackingMileStone/SaveMileStone', MileStone);
    };

    var DeleteMileStone = function (MileStoneId) {
        return $http.get(config.SERVICE_URL + '/TrackingMileStone/DeleteMileStoneRecord',
            {
                params: {
                    MileStoneId: MileStoneId
                }
            });
    };
    var CheckTrackingMileStoneKey = function (TrackingMSM) {
        return $http.post(config.SERVICE_URL + '/TrackingMileStone/CheckTrackingMileStoneKey', TrackingMSM);
    };

    var CheckOrderNo = function (order) {
        return $http.post(config.SERVICE_URL + '/TrackingMileStone/CheckOrderNo', order);
    };

    return {
        SaveMilestone: SaveMilestone,
        GetTrackingMileStone: GetTrackingMileStone,
        DeleteMileStone: DeleteMileStone,
        GetShipmentHandlerMethods: GetShipmentHandlerMethods,
        CheckTrackingMileStoneKey: CheckTrackingMileStoneKey,
        CheckOrderNo: CheckOrderNo
    };
});