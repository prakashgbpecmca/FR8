/**
 * Service
 */
angular.module('ngApp.logon').factory('LogonService', function ($http, config) {

    var logon = function (credentials) {
        return $http.post(config.SERVICE_URL + '/Login/LoginUser', credentials);
    };

    var changePassword = function (changePasswordDetail) {
        return $http.post(config.SERVICE_URL + '/Login/ChangePassword', changePasswordDetail);
    };

    var getUserTabs = function (userId, roleId) {
        return $http.get(config.SERVICE_URL + '/Login/GetUserTabs',
           {
               params: {
                   userId: userId,
                   roleId: roleId
               }
           });
    };

    return {
        logon: logon,
        changePassword: changePassword,
        getUserTabs: getUserTabs
    };
});
