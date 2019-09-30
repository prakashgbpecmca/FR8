angular.module('ngApp.margin').factory('MarginService', function ($http, config) {

    var GetCompany = function (OperationZoneId) {
        return $http.get(config.SERVICE_URL + '/Customer/GetCustomerMarginLogistic',
            {
                params: {
                    OperationZoneId: OperationZoneId
                }
            });
    };

    var GetOptions = function (OperationZoneId, CompanyName) {
        return $http.get(config.SERVICE_URL + '/Customer/GetCustomerMarginOptions',
            {
                params: {
                    
                    OperationZoneId: OperationZoneId,
                    CourierCompany: CompanyName
                }
            });
    };

    var AddOptions = function (Options) {
        return $http.post(config.SERVICE_URL + '/Customer/AddCustomerMarginOptions', Options);
    };
    return {
        GetCompany: GetCompany,
        GetOptions: GetOptions,
        AddOptions: AddOptions
    };
});