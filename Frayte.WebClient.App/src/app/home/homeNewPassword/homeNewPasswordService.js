angular.module("ngApp.newPassword")
        .factory("NewPasswoirdService", function ($http, config) {

            var ChangeFirstPassword = function (changePasswordDetail) {
                return $http.post(config.SERVICE_URL + '/Account/ChangeFirstPassword', changePasswordDetail);
            };

            return {
                ChangeFirstPassword: ChangeFirstPassword
            };
        });
