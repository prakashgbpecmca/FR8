angular.module('ngApp.express').factory('ExpressScanAwbService', function ($http, config, SessionService) {

    var GetCustomerList = function (UserId) {
        return $http.get(config.SERVICE_URL + '/ExpressScannedAWB/GetCustomers',
            {
                params: {
                    UserId: UserId
                }
            });
    };
    var ImageToByte = function (AWBId) {
        return $http.get(config.SERVICE_URL + '/ExpressScannedAWB/ImageToByte',
            {
                params: {
                    AWBId: AWBId
                }
            });
    };
    var GetScannedAWB = function (Track) {
        return $http.post(config.SERVICE_URL + '/ExpressScannedAWB/GetScannedAWB',Track);
    };
    var MultipleScannedDelete = function (scannedAwbList) {
        return $http.post(config.SERVICE_URL + '/ExpressScannedAWB/DeleteScannedAWBList', scannedAwbList);
    };
    return {
        GetCustomerList: GetCustomerList,
        GetScannedAWB: GetScannedAWB,
        ImageToByte: ImageToByte,
        MultipleScannedDelete: MultipleScannedDelete
    };
});