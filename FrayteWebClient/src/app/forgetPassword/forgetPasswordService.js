/**
 * Service
 */
angular.module('ngApp.forgetPassword').factory('ForgetPasswordService', function ($http, config) {

    //var loginContollerApi = config.SERVICE_URL + '/Login';

    var SendRecoveyMail = function (recovery) {
        return $http.post(config.SERVICE_URL + '/Login/RecoveryEmail', recovery);
    };

    return {
        SendRecoveyMail: SendRecoveyMail
    };
});
