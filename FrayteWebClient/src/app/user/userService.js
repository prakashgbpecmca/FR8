/**
 * Service
 */
angular.module('ngApp.user').factory('UserService', function ($http, config, SessionService) {

    var GetInitials = function () {
        return $http.get(config.SERVICE_URL + '/FrayteUser/GetInitials');
    };

    var GetUserDetail = function (userId) {
        return $http.get(config.SERVICE_URL + '/FrayteUser/GetUserDetail',
            {
                params: {
                    userId: userId
                }
            });
    };

    var GetUserList = function (trackUser) {
        return $http.post(config.SERVICE_URL + '/FrayteUser/GetUserList', trackUser);
    };

    var SaveUser = function (userDetail) {
        return $http.post(config.SERVICE_URL + '/FrayteUser/SaveUser', userDetail);
    };

    var DeleteUser = function (userId) {
        return $http.get(config.SERVICE_URL + '/FrayteUser/DeleteUser',
            {
                params: {
                    userId: userId
                }
            });
    };

    var GetAssociatedUsers = function (searchName) {
        return $http.get(config.SERVICE_URL + '/FrayteUser/GetAssociatedUsers',
            {
                params: {
                    searchName: searchName
                }
            });
    };

    var CheckUserEmail = function (email) {
        return $http.get(config.SERVICE_URL + '/FrayteUser/CheckUserEmail',
            {
                params: {
                    email: email
                }
            });
    };
    var GetSystemRoles = function (UserId) {
        return $http.get(config.SERVICE_URL + '/FrayteUser/GetSystemRoles', {
            params: {
                UserId: UserId

            }
        });
    };
    return {
        GetInitials: GetInitials,
        GetUserDetail: GetUserDetail,
        SaveUser: SaveUser,
        GetUserList: GetUserList,
        DeleteUser: DeleteUser,
        GetAssociatedUsers: GetAssociatedUsers,
        CheckUserEmail: CheckUserEmail,
        GetSystemRoles: GetSystemRoles
    };

});