/**
 * Service
 */
angular.module('ngApp.courierAccount').factory('CourierAccountService', function ($http, config, SessionService) {

    var GetCourierAccounts = function (OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType) {
        return $http.get(config.SERVICE_URL + '/CourierAccount/GetCourierAccounts', {

            params: {
                OperationZoneId: OperationZoneId,
                LogisticType: LogisticType,
                CourierCompany: CourierCompany,
                RateType: RateType,
                ModuleType: ModuleType
            }
        });
    };

    var GetInfo = function (OperationZoneId) {
        return $http.get(config.SERVICE_URL + '/LogisticItem/GetLogisticItemList', {

            params: {
                OperationZoneId: OperationZoneId
            }
        });
    };

    var GetOperationZones = function () {
        return $http.get(config.SERVICE_URL + '/CourierAccount/OperationZoneList');
    };

    var ShipmentCourierList = function () {
        return $http.get(config.SERVICE_URL + '/CourierAccount/ShipmentCourierList');
    };
    var SaveCourierAccount = function (fcAccount) {
        return $http.post(config.SERVICE_URL + '/CourierAccount/SaveCourierAccount', fcAccount);
    };
 
    var DeleteCourierAccount = function (courieraccountId) {
        return $http.get(config.SERVICE_URL + '/CourierAccount/DeleteCourierAccount',
           {
               params: {
                   courieraccountId: courieraccountId
               }
           });
    };
    return {
        GetCourierAccounts: GetCourierAccounts,
        GetInfo : GetInfo,
        GetOperationZones: GetOperationZones,
        ShipmentCourierList: ShipmentCourierList,
        SaveCourierAccount: SaveCourierAccount,
        DeleteCourierAccount: DeleteCourierAccount
    };

});