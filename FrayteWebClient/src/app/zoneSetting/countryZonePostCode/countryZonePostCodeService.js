

angular.module('ngApp.CountryZonePostCode').factory('CountryZonePostCodeService', function ($http, config, SessionService) {

    
    var GetZones = function (OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType) {
        return $http.get(config.SERVICE_URL + '/Zone/GetZoneList', {
            params: {
                OperationZoneId: OperationZoneId,
                LogisticType: LogisticType,
                CourierCompany: CourierCompany,
                RateType: RateType,
                ModuleType: ModuleType
            }
        });
    };

    var GetZoneCountryPostCode = function (OperationZoneId, CourierCompany, LogisticType, RateType) {
        return $http.get(config.SERVICE_URL + '/ZoneCountryPostCode/GetZoneCountryPostCode', {
            params: {
                OperationZoneId: OperationZoneId,
                LogisticCompany: CourierCompany,
                LogisticType: LogisticType,
                RateType: RateType
                
            }
        });
    };

    var SaveZoneCountryPostCode = function (CountryZonePostCodeDetail) {
        return $http.post(config.SERVICE_URL + '/ZoneCountryPostCode/SaveZoneCountryPostCode', CountryZonePostCodeDetail);
    };

    var DeleteZoneCountryPostCode = function (LogisticZoneCountryPostCodeId) {
        return $http.get(config.SERVICE_URL + '/ZoneCountryPostCode/DeleteZoneCountryPostCode', {
            params: {
                LogisticZoneCountryPostCodeId: LogisticZoneCountryPostCodeId
            }
        });
    };


    var GetCountryCodeList = function () {
        return $http.get(config.SERVICE_URL + '/Country/GetCountryCodeList');
    };

   
    return {
        GetZones: GetZones,
        GetZoneCountryPostCode: GetZoneCountryPostCode,
        DeleteZoneCountryPostCode: DeleteZoneCountryPostCode,
        GetCountryCodeList: GetCountryCodeList,
        SaveZoneCountryPostCode: SaveZoneCountryPostCode
    };

});