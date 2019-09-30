angular.module('ngApp.express').factory("ExpressIntegrationShipmentService", function ($http, config) {

    var TradelaneHubInitials = function (customerId, hubId) {
        return $http.get(config.SERVICE_URL + "/ExpressManifest/TradelaneHubInitials", {
            params: {
                customerId: customerId,
                hubId: hubId
            }
        });
    };

    var SaveTradelaneIntegrationShipment = function (integratedShipment) {
        return $http.post(config.SERVICE_URL + "/ExpressManifest/SaveTradelaneIntegration", integratedShipment);
    };

    var GetTimeZoneName = function (BagId) {
        return $http.get(config.SERVICE_URL + "/ExpressManifest/GetTimeZoneName?BagId=" + BagId);
    };

    return {
        GetTimeZoneName: GetTimeZoneName,
        TradelaneHubInitials: TradelaneHubInitials,
        SaveTradelaneIntegrationShipment: SaveTradelaneIntegrationShipment
    };
});