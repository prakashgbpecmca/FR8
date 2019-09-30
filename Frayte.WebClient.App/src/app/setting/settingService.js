/**
 * Service
 */
angular.module('ngApp.setting').factory('SettingService', function ($http, config, SessionService) {

    var GetPieceDetailsExcelPath = function () {
        return $http.get(config.SERVICE_URL + '/Setting/GetPieceDetailsExcelPath');
    };
    var addAdminCharge = function (charges) {
        return $http.post(config.SERVICE_URL + '/AdminCharges/CreateCharges', charges);
    };
    var getCustomerSpecificAdminCharges = function () {
        return $http.get(config.SERVICE_URL + '/AdminCharges/GetCustomerSpecificAdminCharges');
    };
    var getAdminCharges = function (userId) {
        return $http.get(config.SERVICE_URL + '/AdminCharges/GetAdminCharges', {
            params: {
                userId: userId
            }
        });
    };
    var deleteCustomerAdminCharge = function (adminChargeId) {
        return $http.get(config.SERVICE_URL + '/AdminCharges/adminChargeId', {
            params: {
                adminChargeId: adminChargeId
            }
        });
    };
    var deleteAdminCharge = function (adminChargeId) {
        return $http.get(config.SERVICE_URL + '/AdminCharges/DeleteAdminCharge', {
            params: {
                adminChargeId: adminChargeId
            }
        });
    };
    var saveCustomerCharge = function (charge) {
        return $http.post(config.SERVICE_URL + '/AdminCharges/SaveCustomerCharge', charge);
    };
    var getCustomersWithoutCharges = function (userId, moduleType , mode) {
        return $http.get(config.SERVICE_URL + '/AdminCharges/GetCustomersWithoutCharges', {
            params: {
                userId: userId,
                moduleType: moduleType,
                mode: mode
            }
        });
    };
    var removeCustomerAdminCharges = function (customerId, userId) {
        return $http.get(config.SERVICE_URL + '/AdminCharges/RemoveCustomerAdminCharges', {
            params: {
                customerId: customerId,
                userId: userId
            }
        });
    };
    return {
        removeCustomerAdminCharges:removeCustomerAdminCharges,
        getCustomersWithoutCharges:getCustomersWithoutCharges,
        saveCustomerCharge: saveCustomerCharge,
       deleteAdminCharge:deleteAdminCharge,
        getAdminCharges:getAdminCharges,
        getCustomerSpecificAdminCharges:getCustomerSpecificAdminCharges,
        addAdminCharge:addAdminCharge,
        GetPieceDetailsExcelPath: GetPieceDetailsExcelPath
    };

});