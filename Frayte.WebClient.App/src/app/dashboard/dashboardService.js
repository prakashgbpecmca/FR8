/**
 * Service
 */
angular.module('ngApp.dashBoard').factory('DashBoardService', function ($http, config, SessionService) {

    
    var GetDashBoardInitialData = function () {
        return $http.get(config.SERVICE_URL + '/DashBoard/GetDashBoardInitialDetail');
    };
    //var SaveShipmentFromErrorPopup = function (UploadShipment) {
    //    return $http.post(config.SERVICE_URL + '/eCommerceUploadShipment/FrayteUploadShipments', UploadShipment);
    //};

   
    return {
        GetDashBoardInitialData: GetDashBoardInitialData
       
    };
});