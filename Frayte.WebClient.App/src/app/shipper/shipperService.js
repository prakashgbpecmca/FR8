/**
 * Service
 */
angular.module('ngApp.shipper').factory('ShipperService', function ($http, config, SessionService) {

    var GetShipperDetail = function (shipperId) {
        return $http.get(config.SERVICE_URL + '/Shipper/GetShipperDetail',            
            {
                params: {
                    shipperId: shipperId
                }
            });
    };

    var GetShipperList = function () {
        return $http.get(config.SERVICE_URL + '/Shipper/GetShipperList');
    };

    var SaveShipper = function (shipperDetail) {
        return $http.post(config.SERVICE_URL + '/Shipper/SaveShipper', shipperDetail);
    };

    var DeleteShipper = function (shipperId) {
        return $http.get(config.SERVICE_URL + '/Shipper/DeleteShipper',
            {
                params: {
                    shipperId: shipperId
                }
            });
    };

    var GetSearchShippers = function (shipperName) {
        return $http.get(config.SERVICE_URL + '/Shipper/GetSearchShippers',
            {
                params: {
                    shipperName: shipperName
                }
            });
    };

    var GetShipperMainAddress = function (shipperId) {
        return $http.get(config.SERVICE_URL + '/Shipper/GetShipperMainAddress',
            {
                params: {
                    shipperId: shipperId
                }
            });
    };

    var GetShippeOtherAddresses = function (shipperId) {
        return $http.get(config.SERVICE_URL + '/Shipper/GetShippeOtherAddresses',
            {
                params: {
                    shipperId: shipperId
                }
            });
    };

    var SaveShipperOtherAddress = function (shipperOtherAddress) {
        return $http.post(config.SERVICE_URL + '/Shipper/SaveShipperOtherAddress', shipperOtherAddress);
    };

    var GetShipperReceivers = function (shipperId) {
        return $http.get(config.SERVICE_URL + '/Shipper/GetShipperReceivers',
            {
                params: {
                    shipperId: shipperId
                }
            });
    };

    return {
        GetShipperList: GetShipperList,
        SaveShipper: SaveShipper,        
        DeleteShipper: DeleteShipper,
        GetShipperDetail: GetShipperDetail,
        GetSearchShippers: GetSearchShippers,
        GetShipperMainAddress: GetShipperMainAddress,
        GetShippeOtherAddresses: GetShippeOtherAddresses,
        SaveShipperOtherAddress: SaveShipperOtherAddress,
        GetShipperReceivers: GetShipperReceivers
    };

});