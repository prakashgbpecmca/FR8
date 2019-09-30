/**
 * Service
 */
angular.module('ngApp.buildConsignmment').factory('BuildConsigmentService', function ($http, config, SessionService) {

 

    var GetBags = function () {
        return $http.get(config.SERVICE_URL + '/ShipmentExprys/GetConsignementBags');
    };

    var GetFrateAWBS = function () {
        return $http.get(config.SERVICE_URL + '/ShipmentExprys/GetAllFrayteAWB');
    };
    var SaveConsigmentBagDetails = function (model) {
        return $http.post(config.SERVICE_URL + '/ShipmentExprys/SaveConsigmentBagDetails', model);
    };
   
    return {
        GetBags: GetBags,
        GetFrateAWBS: GetFrateAWBS,
        SaveConsigmentBagDetails: SaveConsigmentBagDetails
    };

});