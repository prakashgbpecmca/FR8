/**
 * Service
 */
angular.module('ngApp.courier').factory('CourierService', function ($http, config, SessionService) {
   
    var GetCourierList = function () {
        return $http.get(config.SERVICE_URL + '/Courier/GetCourierList');        
    };

    var SaveCourier = function (courierDetail) {
        return $http.post(config.SERVICE_URL + '/Courier/SaveCourier', courierDetail);
    };

    var DeleteCourier = function (courierId) {
        return $http.get(config.SERVICE_URL + '/Courier/DeleteCourier',
            {
                params: {
                    courierId: courierId
                }
            });
    };

    return {
        SaveCourier: SaveCourier,
        GetCourierList: GetCourierList,
        DeleteCourier: DeleteCourier
    };
    
});