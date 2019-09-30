angular.module('ngApp.accessLevel').factory("AccessLevelService", function ($http, config) {

    var GetRoleModules = function (userId, roleId, moduleType) {
        return $http.get(config.SERVICE_URL + "/ModuleLevel/GetRoleModules", {
            params: {
                userId: userId,
                roleId: roleId,
                moduleType: moduleType
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