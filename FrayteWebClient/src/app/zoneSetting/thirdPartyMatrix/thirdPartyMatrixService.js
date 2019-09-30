
angular.module('ngApp.thirdPartyMatrix').factory('ThirdPartyMatrixService', function ($http, config, SessionService) {

    var GetOperationZone = function () {
        return $http.get(config.SERVICE_URL + '/ThirdPartyMatrix/GetOperationZone');
    };
    var GetZones = function (OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType) {
        return $http.get(config.SERVICE_URL + '/ThirdPartyMatrix/GetZoneDetail', {
            params: {
                OperationZoneId: OperationZoneId,
                LogisticType: LogisticType,
                CourierCompany: CourierCompany,
                RateType: RateType,
                ModuleType: ModuleType
            }
        });
    };

    var GetThirdPartyMatrixDetail = function (OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType) {
        return $http.get(config.SERVICE_URL + '/ThirdPartyMatrix/GetThirdPartyMatrixDetail', {
            params: {
                OperationZoneId: OperationZoneId,
                LogisticType: LogisticType,
                CourierCompany: CourierCompany,
                RateType: RateType,
                ModuleType: ModuleType
            }
        });
    };

    var SaveThirdPartyMatrix = function (_party) {
        return $http.post(config.SERVICE_URL + '/ThirdPartyMatrix/EditThirdPartyMarix', _party);
    };
    var GetLogisticItemList = function (OperationZoneId) {
        return $http.get(config.SERVICE_URL + '/LogisticItem/GetLogisticItemList',
            {
                params: {
                    OperationZoneId: OperationZoneId
                }
            });
    };
    return {
        GetOperationZone: GetOperationZone,
        GetZones: GetZones,
        GetThirdPartyMatrixDetail: GetThirdPartyMatrixDetail,
        SaveThirdPartyMatrix: SaveThirdPartyMatrix,
        GetLogisticItemList: GetLogisticItemList

    };

});