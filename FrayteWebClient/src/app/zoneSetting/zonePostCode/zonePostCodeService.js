
angular.module('ngApp.zonePostCode').factory('ZonePostCodeService', function ($http, config, SessionService) {

    var GetOperationZone = function () {
        return $http.get(config.SERVICE_URL + '/OperationZone/OperationZone');
    };
    var GetCountryUKPostCode = function (OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType) {
        return $http.get(config.SERVICE_URL + '/ZonePostCode/GetCountryUKPostCode', {
            params: {
                OperationZoneId: OperationZoneId,
                LogisticType: LogisticType,
                CourierCompany: CourierCompany,
                RateType: RateType,
                ModuleType: ModuleType
            }
        });
    };

    var GetPostCodeList = function (OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType) {
        return $http.get(config.SERVICE_URL + '/ZonePostCode/GetPostCodeList', {
            params: {
                OperationZoneId: OperationZoneId,
                LogisticType: LogisticType,
                CourierCompany: CourierCompany,
                RateType: RateType,
                ModuleType: ModuleType
            }
        });
    };

    var GetZonePostCodeList = function (OperationZoneId, LogisticType, ZoneId, SearchPostcode, CurrentPage, TakeRows) {
        return $http.get(config.SERVICE_URL + '/ZonePostCode/GetZonePostCodeList', {
            params: {
                OperationZoneId: OperationZoneId,
                LogisticType: LogisticType,
                ZoneId: ZoneId,
                SearchPostcode: SearchPostcode,
                CurrentPage: CurrentPage,
                TakeRows: TakeRows
            }
        });
    };

    var GetCountryUK = function () {
        return $http.get(config.SERVICE_URL + '/ZonePostCode/GetCountryUK');
    };
    var SavePostCode = function (_postcode) {
        return $http.post(config.SERVICE_URL + '/ZonePostCode/SavePostCode', _postcode);
    };

    var GetZoneList = function (OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType) {
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
    return {
        GetOperationZone: GetOperationZone,
        GetCountryUKPostCode: GetCountryUKPostCode,
        GetPostCodeList: GetPostCodeList,
        GetCountryUK: GetCountryUK,
        GetZoneList: GetZoneList,
        SavePostCode: SavePostCode,
        GetZonePostCodeList: GetZonePostCodeList
    };

});