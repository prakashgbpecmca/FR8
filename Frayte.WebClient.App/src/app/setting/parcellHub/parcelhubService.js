angular.module('ngApp.parcelhub').factory('ParcelHubService', function ($http, config, SessionService) {

    var GetParcelHubApiKeys = function () {
        return $http.get(config.SERVICE_URL + '/ParcelHubKey/GetParcelHubApiKeys');
    };
    var SaveParcelHubKey = function (ParcelHubKey) {
        return $http.post(config.SERVICE_URL + '/ParcelHubKey/SaveParcelHubKey', ParcelHubKey);
    };

    var DeleteParcelHubKey = function (APIId) {
        return $http.post(config.SERVICE_URL + '/ParcelHubKey/DeleteParcelHubKey' + '?APIId=' + APIId);
    };
    return {
        GetParcelHubApiKeys: GetParcelHubApiKeys,
        SaveParcelHubKey: SaveParcelHubKey,
        DeleteParcelHubKey: DeleteParcelHubKey
    };

});