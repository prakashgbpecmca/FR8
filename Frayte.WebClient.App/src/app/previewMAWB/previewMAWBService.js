angular.module("ngApp.previewMAWB")
.factory("PreviewMAWBService", function ($http , config) {

    var MAWBPreview = function (tradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + '/TradelaneBooking/MAWBPreview',
         {
             params: {
                 tradelaneShipmentId: tradelaneShipmentId
             }
         });
    };
    var ChangeAddress = function (address) {
        return $http.post(config.SERVICE_URL + '/TradelaneBooking/ChangeMAWBAddress', address);
    };
    return {
        ChangeAddress:ChangeAddress,
        MAWBPreview: MAWBPreview
    };
});