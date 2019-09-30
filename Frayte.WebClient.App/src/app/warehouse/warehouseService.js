/**
 * Service
 */
angular.module('ngApp.warehouse').factory('WarehouseService', function ($http, config, SessionService) {

    var GetWarehouseList = function () {
        return $http.get(config.SERVICE_URL + '/Warehouse/GetAllWarehouseList');
    };

    var GetWarehouseDetail = function (warehouseId) {
        return $http.get(config.SERVICE_URL + '/Warehouse/GetWarehouseDetail',
             {
                 params: {
                     warehouseId: warehouseId
                 }
             });

    };

    var SaveWarehouse = function (warehouseDetail) {
        return $http.post(config.SERVICE_URL + '/Warehouse/SaveWarehouse', warehouseDetail);
    };

    var DeleteWarehouse = function (warehouseId) {
        return $http.get(config.SERVICE_URL + '/Warehouse/DeleteWarehouse',
            {
                params: {
                    warehouseId: warehouseId
                }
            });
    };

    var SaveWarehouseMap = function (base64ImageString) {
        return $http.post(config.SERVICE_URL + '/Warehouse/SaveWarehouseMap', base64ImageString);
    };

    return {
        GetWarehouseList: GetWarehouseList,
        SaveWarehouse: SaveWarehouse,
        DeleteWarehouse: DeleteWarehouse,
        SaveWarehouseMap: SaveWarehouseMap,
        GetWarehouseDetail: GetWarehouseDetail
    };
});