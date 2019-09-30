/**
 * Service
 */
angular.module('ngApp.carrier').factory('CarrierService', function ($http, config, SessionService) {
   
    var GetCarrierList = function () {
        return $http.get(config.SERVICE_URL + '/Carrier/GetCarrierList');        
    };

    var GetCarrierTypeList = function (carrierType) {
        return $http.get(config.SERVICE_URL + '/Carrier/GetCarrierList',
            {
                params: {
                    carrierType: carrierType
                }
            });
    };

    var SaveCarrier = function (carrierDetail) {
        return $http.post(config.SERVICE_URL + '/Carrier/SaveCarrier', carrierDetail);
    };

    var DeleteCarrier = function (carrierId) {
        return $http.get(config.SERVICE_URL + '/Carrier/DeleteCarrier',
            {
                params: {
                    carrierId: carrierId
                }
            });
    };

    return {
        SaveCarrier: SaveCarrier,
        GetCarrierList: GetCarrierList,
        GetCarrierTypeList: GetCarrierTypeList,
        DeleteCarrier: DeleteCarrier
    };
    
});