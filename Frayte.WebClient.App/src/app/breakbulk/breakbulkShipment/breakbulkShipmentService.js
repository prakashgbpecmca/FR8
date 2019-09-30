angular.module('ngApp.breakBulk')
    .factory("BreakbulkShipmentService", function ($http, config) {

        //var GetExpressCustomers = function (RoleId, UserId) {
        //    return $http.get(config.SERVICE_URL + '/ExpressShipment/GetExpressCustomers',
        //        {
        //            params: {
        //                RoleId: RoleId,
        //                UserId: UserId
        //            }
        //        });
        //};

        var GetInitials = function (userId) {
            return $http.get(config.SERVICE_URL + '/BreakBulk/GetInitials?userId=' + userId);
        };

        var GetPOPurchaseOrderD = function (track) {
            return $http.post(config.SERVICE_URL + '/BreakBulk/GetPOPurchaseOrderD', track);
        };

        var GetJobPurchaseOrderD = function (track) {
            return $http.post(config.SERVICE_URL + '/BreakBulk/GetJobPurchaseOrderD', track);
        };

        var GetBBKShipmentStatusList = function (BookingType) {
            return $http.get(config.SERVICE_URL + '/BreakBulk/GetBBKShipmentStatusList',
                {
                    params: {
                        BookingType: BookingType
                    }
                });
        };

       
       
        return {
           
            //GetExpressCustomers: GetExpressCustomers,
            GetInitials: GetInitials,
            GetPOPurchaseOrderD: GetPOPurchaseOrderD,
            GetJobPurchaseOrderD: GetJobPurchaseOrderD,
            GetBBKShipmentStatusList: GetBBKShipmentStatusList
            
        };

    });
