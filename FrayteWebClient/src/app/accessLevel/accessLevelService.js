angular.module('ngApp.accessLevel').factory("AccessLevelService", function ($http, config) {

    var GetRoleModules = function (userId, roleId) {
        return $http.get(config.SERVICE_URL + "/ModuleLevel/GetRoleModules", {
            params: {
                userId: userId,
                roleId: roleId
            }
        });
    };

    var SaveRoleModulePermission = function (acessRoleModule) {
        return $http.post(config.SERVICE_URL + "/ModuleLevel/SaveRoleModule", acessRoleModule);
    };

    return {
        GetRoleModules: GetRoleModules,
        SaveRoleModulePermission: SaveRoleModulePermission
    };
});