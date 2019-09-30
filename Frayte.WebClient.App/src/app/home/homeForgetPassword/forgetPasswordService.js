/**
 * Service
 */
angular.module('ngApp.forgetPassword').factory('ForgetPasswordService', function ($http, config) {

    //var loginContollerApi = config.SERVICE_URL + '/Login';

    var SendRecoveyMail = function (recovery) {
        return $http.post(config.SERVICE_URL + '/Account/ForgetPassword', recovery);
    };
    var RecoverPassword = function (recoverPassword) {
        return $http.post(config.SERVICE_URL + '/Account/RecoverPassword', recoverPassword);
    };
    return {
        SendRecoveyMail: SendRecoveyMail,
        RecoverPassword: RecoverPassword
    };
});
