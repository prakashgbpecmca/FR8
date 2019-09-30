angular.module('ngApp.profileandsetting').service('ProfileandSettingService', function ($http, config, SessionService) {
    var CustomerApiDetail = function (CustomerId) {
        return $http.get(config.SERVICE_URL + '/Customer/CustomerApiDetail',
        {
            params: {
                CustomerId: CustomerId
            }
        });
    };

    return {
        ApiDetail: CustomerApiDetail
    };
});