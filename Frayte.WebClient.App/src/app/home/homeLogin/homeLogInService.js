/**
 * Service
 */
angular.module('ngApp.home').factory('HomeLogonService', function ($http, config) {

    var logon = function (credentials) {
        return $http.post(config.SERVICE_URL + '/Login/Authenticate', credentials);
    };

    var getCodeAsPassword = function (codeToEmail) {
        return $http.get(config.SERVICE_URL + '/Login/GetCodeByEmail', {
            params: {
                Email: codeToEmail
            }
        });
    };
    var changePassword = function (userName, password) {
        return $http.get(config.SERVICE_URL + '/Login/ChangePassword', {
            params: {
                userID: userName,
                passwrd: password
            }
        });
    };
    return {
        logon: logon,
        getCodeAsPassword: getCodeAsPassword,
        changePassword: changePassword
    };
});
