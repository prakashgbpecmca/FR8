angular.module('ngApp.express').factory("ExpressManifestService", function ($http, config) {

    var GetManifestDetail = function (TrackManifest) {
        return $http.post(config.SERVICE_URL + '/ExpressManifest/GetManifests', TrackManifest);
    };

    var GetNonManifestedShipments = function (OperationZoneId, UserId, CreatedBy, moduleType, subModuleType) {
        return $http.get(config.SERVICE_URL + '/Manifest/GetNonManifestedShipments', {
            params: {
                OperationZoneId: OperationZoneId,
                UserId: UserId,
                CreatedBy: CreatedBy,
                moduleType: moduleType,
                subModuleType: subModuleType
            }
        });
    };

    var ViewManifest = function (ManifestId, moduleType, subModuleType) {
        if (subModuleType === undefined || subModuleType === null) {
            subModuleType = "";
        }
        return $http.get(config.SERVICE_URL + '/Manifest/GetManifestDetail', {
            params: {
                ManifestId: ManifestId,
                moduleType: moduleType,
                subModuleType: subModuleType
            }
        });
    };

    var ViewManifestDetail = function (ManifestId) {
        return $http.get(config.SERVICE_URL + '/ExpressManifest/GetManifestShipments', {
            params: {
                ManifestId: ManifestId
            }
        });
    };

    var GetBagLabel = function (BagId) {
        return $http.get(config.SERVICE_URL + '/ExpressManifest/GetBagLabel', {
            params: {
                BagId: BagId
            }
        });
    };

    var GetBagShipments = function (BagId) {
        return $http.get(config.SERVICE_URL + '/ExpressManifest/GetShipments', {
            params: {
                BagId: BagId
            }
        });
    };

    var GetNonManifestedBags = function (HubId, CustomerId) {
        return $http.get(config.SERVICE_URL + '/ExpressManifest/GetBagDetail', {
            params: {
                HubId: HubId,
                CustomerId: CustomerId
            }
        });
    };

    var getHubs = function () {
        return $http.get(config.SERVICE_URL + "/ExpressManifest/GetHubs");
    };

    var GetManifestedShipments = function (FrayteManifestShipment) {
        return $http.post(config.SERVICE_URL + '/Manifest/CreateManifest', FrayteManifestShipment);
    };

    var GetExportManifest = function (DownloadPDFModel) {
        return $http.post(config.SERVICE_URL + '/ExpressManifest/GetExportManifest', DownloadPDFModel);
    };

    var GetDriverManifest = function (DownloadPDFModel) {
        return $http.post(config.SERVICE_URL + '/ExpressManifest/GetDriverManifest', DownloadPDFModel);
    };

    var GetCustomManifest = function (ManifestId) {
        return $http.get(config.SERVICE_URL + '/ExpressManifest/GetCustomManifest?ManifestId=' + ManifestId);
    };

    return {
        GetNonManifestedShipments: GetNonManifestedShipments,
        GetManifestDetail: GetManifestDetail,
        ViewManifestDetail: ViewManifestDetail,
        GetManifestedShipments: GetManifestedShipments,
        GetNonManifestedBags: GetNonManifestedBags,
        getHubs: getHubs,
        GetBagShipments: GetBagShipments,
        GetExportManifest: GetExportManifest,
        GetDriverManifest: GetDriverManifest,
        GetBagLabel: GetBagLabel,
        GetCustomManifest: GetCustomManifest
    };
});