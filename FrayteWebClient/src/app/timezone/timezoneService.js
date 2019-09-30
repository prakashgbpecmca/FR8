/**
 * Service
 */
angular.module('ngApp.timezone').factory('TimeZoneService', function ($http, config, SessionService) {
   
    var GetTimeZoneList = function () {
        return $http.get(config.SERVICE_URL + '/TimeZone/GetTimeZoneList');        
    };

    var GetShipmentTimeZones = function () {
        return $http.get(config.SERVICE_URL + '/TimeZone/GetShipmentTimeZones');
    };

    var SaveTimeZone = function (timezoneDetail) {
        return $http.post(config.SERVICE_URL + '/TimeZone/SaveTimeZone', timezoneDetail);        
    };

    var DeleteTimeZone = function (timezoneId) {
        return $http.get(config.SERVICE_URL + '/TimeZone/DeleteTimeZone',
            {
                params: {
                    timezoneId: timezoneId
                }
            });
    };

    return {
        SaveTimeZone: SaveTimeZone,
        GetTimeZoneList: GetTimeZoneList,
        GetShipmentTimeZones: GetShipmentTimeZones,
        DeleteTimeZone: DeleteTimeZone
    };
});