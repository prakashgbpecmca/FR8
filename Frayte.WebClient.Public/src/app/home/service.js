angular.module('ngApp.home').factory('HomeService', function ($http, config) {

    var SearchTracking = function (Num) {
        return $http.get(config.SERVICE_URL + '/TradelaneUpdateTracking/GetTracking',
            {
                params: {
                    Number: Num
                }
            });
    };

    return {
        SearchTracking: SearchTracking
    };
});