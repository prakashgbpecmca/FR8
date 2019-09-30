
angular.module('ngApp.zoneCountry').factory('ZoneCountryService', function ($http, config, SessionService) {
 
    var GetOperationZones = function () {
        return $http.get(config.SERVICE_URL + '/FrayteZone/OperationZone');
    };

    var GetCountries = function (OperationZoneId, LogisticZoneId, LogisticType, CourierCompany, RateType, ModuleType) {
        return $http.get(config.SERVICE_URL + '/FrayteZone/Country',
            {
                params: {
                    OperationZoneId: OperationZoneId,
                    LogisticZoneId: LogisticZoneId,
                    LogisticType: LogisticType,
                    CourierCompany: CourierCompany,
                    RateType: RateType,
                    ModuleType: ModuleType
                }
            });
    };

    var GetZoneCountry = function (OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType) {
        return $http.get(config.SERVICE_URL + '/FrayteZone/ZoneCountryDetail', {
            params: {
                OperationZoneId: OperationZoneId,
                LogisticType: LogisticType,
                CourierCompany: CourierCompany,
                RateType: RateType,
                ModuleType: ModuleType
            }
        });
    };  
    
    var SaveZoneCountry = function (zonecountry) {
        return $http.post(config.SERVICE_URL + '/FrayteZone/SaveFrayteZoneCountry', zonecountry);
    };

    return {
        GetOperationZones: GetOperationZones,
        SaveZoneCountry: SaveZoneCountry,
        GetZoneCountry: GetZoneCountry,
        GetCountries: GetCountries
    };

});