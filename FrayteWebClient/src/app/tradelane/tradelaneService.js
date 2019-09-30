/**
 * Service
 */
angular.module('ngApp.tradelane').factory('TradelaneService', function ($http, config, SessionService) {

    var GetTradelaneList = function () {
        return $http.get(config.SERVICE_URL + '/Tradelane/GetTradelaneList');
    };

    var SaveTradelane = function (tradelaneDetail) {
        return $http.post(config.SERVICE_URL + '/Tradelane/SaveTradelane', tradelaneDetail);
    };

    var DeleteTradelane = function (tradelaneId) {
        return $http.get(config.SERVICE_URL + '/Tradelane/DeleteTradelane',
            {
                params: {
                    tradelaneId: tradelaneId
                }
            });
    };

    return {
        SaveTradelane: SaveTradelane,
        GetTradelaneList: GetTradelaneList,
        DeleteTradelane: DeleteTradelane
    };
});