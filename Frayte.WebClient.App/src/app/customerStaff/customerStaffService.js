angular.module('ngApp.customerStaff').factory('CustomerStaffService', function ($http, config, SessionService) {

    var GetInitials = function (track) {
        return $http.post(config.SERVICE_URL + '/CustomerStaff/GetInitials', track);
    };

    var GetCustomerStaffDetail = function (UserId) {
        return $http.get(config.SERVICE_URL + '/CustomerStaff/GetCustomerStaffDetail',
           {
               params: {
                   UserId: UserId
               }
           });
    };

    var GetCustomer = function (Name, RoleId) {
        return $http.get(config.SERVICE_URL + '/CustomerStaff/GetCustomer',
           {
               params: {
                   Name: Name,
                   RoleId: RoleId
               }
           });
    };

    var RemoveAssociateCustomer = function(CustomerStaffDetailId){
        return $http.get(config.SERVICE_URL + '/CustomerStaff/RemoveAssociateCustomer',
           {
               params: {
                   CustomerStaffDetailId: CustomerStaffDetailId
               }
           });
    };

    var RemoveCustomerStaff = function (UserId) {
        return $http.get(config.SERVICE_URL + '/CustomerStaff/RemoveCustomerSatff',
           {
               params: {
                   UserId: UserId
               }
           });
    };

    return {
        GetInitials: GetInitials,
        GetCustomerStaffDetail: GetCustomerStaffDetail,
        GetCustomer: GetCustomer,
        RemoveAssociateCustomer: RemoveAssociateCustomer,
        RemoveCustomerStaff: RemoveCustomerStaff
    };

});