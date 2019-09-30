/**
 * Service
 */
angular.module('ngApp.customer').factory('CustomerService', function ($http, config, SessionService) {

    var GetBookingTypes = function ( userId) {
        return $http.get(config.SERVICE_URL + '/Manifest/GetBookingTypes', {
            params: {
                UserId: userId
            }
        });
    };
    var GetCustomerList = function () {
        return $http.get(config.SERVICE_URL + '/Customer/GetCustomerList');
    };

    var GetCustomerDetail = function (customerId) {
        return $http.get(config.SERVICE_URL + '/Customer/GetCustomerDetail',
            {
                params: {
                    customerId: customerId
                }
            });
    };

    var SaveCustomer = function (customerDetail) {
        return $http.post(config.SERVICE_URL + '/Customer/SaveCustomer', customerDetail);
    };

    var DeleteCustomer = function (customerId) {
        return $http.get(config.SERVICE_URL + '/Customer/DeleteCustomer',
            {
                params: {
                    customerId: customerId
                }
            });
    };

    var CheckAccountNumber = function (accountNumber) {
        return $http.get(config.SERVICE_URL + '/Customer/CheckAccountNumber',
            {
                params: {
                    accountNumber: accountNumber
                }
            });
    };

    var GetZoneList = function (OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType) {
        return $http.get(config.SERVICE_URL + '/Zone/GetZoneList', {
            params: {
                OperationZoneId: OperationZoneId,
                LogisticType: LogisticType,
                CourierCompany: CourierCompany,
                RateType: RateType,
                ModuleType: ModuleType
            }
        });
    };
    var GetZoneDetail = function () {
        return $http.get(config.SERVICE_URL + '/Zone/ZoneList');
    };

    var GetCustomerMarginCost = function () {
        return $http.get(config.SERVICE_URL + '/Customer/GetCustomerMarginCost');
    };
    var GetShipmentType = function (OperationZoneId, CourierCompany, ModuleType) {
        return $http.get(config.SERVICE_URL + '/Customer/GetShipmentType', {
            params: {

                OperationZoneId: OperationZoneId,
                CourierCompany: CourierCompany,
                ModuleType: ModuleType
            }
        });
    };
    var ZoneListByOperationZone = function (OperationZoneId) {
        return $http.get(config.SERVICE_URL + '/Zone/ZoneListByOperationZone', {
            params: {
                OperationZoneId: OperationZoneId
            }
        });
    };

    var GetCustomerModules = function (customerId) {
        return $http.get(config.SERVICE_URL + '/Customer/GetCustomerModules',
            {
                params: {
                    customerId: customerId
                }
            });
    };
    var GetCustomerMargin = function (UserId, OperationZoneId, CourierCompany, ModuleType) {
        return $http.get(config.SERVICE_URL + '/Customer/GetCustomerMargin',
           {
               params: {
                   UserId: UserId,
                   OperationZoneId: OperationZoneId,
                   CourierCompany: CourierCompany,
                   ModuleType: ModuleType
               }
           });
    };
    var GetUKShipmentType = function () {
        return $http.get(config.SERVICE_URL + '/Customer/GetUKShipmentType');
    };
    var SaveCustomerMargin = function (customerMarginCost) {
        return $http.post(config.SERVICE_URL + '/Customer/SaveCustomerMarginCost', customerMarginCost);
    };
    var GetInitial = function (OperationZoneId, LogisticCompany, UserId) {
        return $http.get(config.SERVICE_URL + '/Customer/GetInitials', {
            params: {
                OperationZoneId: OperationZoneId,
                LogisticCompany: LogisticCompany,
                UserId: UserId
            }
        });
    };
    var CustomerMarginOption = function (OperationZoneId, LogisticCompany, MarginOption) {
        return $http.get(config.SERVICE_URL + '/Customer/GetMarginOptionPercentage', {
            params: {
                OperationZoneId: OperationZoneId,
                CourierCompany: LogisticCompany,
                MarginOption: MarginOption
            }
        });
    };

    var GetManifestDetail = function (TrackManifest) {
        return $http.post(config.SERVICE_URL + '/Manifest/GetManifests', TrackManifest);
    };

    var GetNonManifestedShipments = function (OperationZoneId, UserId, moduleType) {
        return $http.get(config.SERVICE_URL + '/Manifest/GetNonManifestedShipments', {
            params: {
                OperationZoneId: OperationZoneId,
                UserId: UserId,
                moduleType : moduleType
            }
        });
    };
    var ViewManifest = function (ManifestId,moduleType) {
        return $http.get(config.SERVICE_URL + '/Manifest/GetManifestDetail', {
            params: {
                ManifestId: ManifestId,
                moduleType: moduleType
            }
        });
    };
    var GetManifestedShipments = function (FrayteManifestShipment) {
        return $http.post(config.SERVICE_URL + '/Manifest/CreateManifest', FrayteManifestShipment);
    };

    var GenerateManifest = function (ManifestId, moduleType, UserId) {
        return $http.get(config.SERVICE_URL + '/Manifest/GenerateManifest', {
            params: {
                ManifestId: ManifestId,
                moduleType: moduleType,
                UserId: UserId
            }
        });
    };

    var DownloadReport = function (fileName) {
        return $http.get(config.SERVICE_URL + '/Manifest/DownloadReport', {
            params: {
                fileName: fileName
            }
        });
    };

    //var GetLogisticServices = function () {
    //    return $http.get(config.SERVICE_URL + '/ViewBaseRateCard/GetLogisticServices');
    //};

    var GetLogisticServices = function (OperationZoneId) {
        return $http.get(config.SERVICE_URL + '/Customer/GetBusinessUnitLogisticServices', {
            params: {
                OperationZoneId: OperationZoneId
            }
        });
    };
    var GetCustomerRateCardDetail = function (UserId) {
        return $http.get(config.SERVICE_URL + '/Customer/GetCustomerRateCardDetail', {
            params: {
                UserId: UserId
            }
        });

    };

    var SaveCustomerRateCardDetail = function (frayteCustomerRateCard) {
        return $http.post(config.SERVICE_URL + '/Customer/SaveCustomerRateCardDetail', frayteCustomerRateCard);
    };

    var RemoveCustomerLogistic = function (logisticServiceId, userId) {
        return $http.get(config.SERVICE_URL + '/Customer/RemoveCustomerLogistic', {
            params: {
                logisticServiceId:logisticServiceId,
                userId: userId
            }
        });
    };

    var RemoveShipmentFromManifest = function (shipmentId, moduleType) {
        return $http.get(config.SERVICE_URL + '/Manifest/RemoveShipmentFromManifest', {
            params: {
                shipmentId: shipmentId,
                moduleType: moduleType
            }
        });
    };

    var GetOperationZones = function () {
        return $http.get(config.SERVICE_URL + '/FrayteZone/OperationZone');
    };
    var GetCurrentOperationZones = function () {
        return $http.get(config.SERVICE_URL + '/OperationZone/GetCurrentOperationZone');
    };

    var GetLogisticItems = function (OperationZoneId) {
        return $http.get(config.SERVICE_URL + '/LogisticItem/GetLogisticItemList', {
            params: {
                OperationZoneId: OperationZoneId
                
            }
        });
    };
    var GetCustomerAdvanceMarginCost = function (OperationZoneId, CustomerId, LogisticCompany, LogisticType, RateType, PackageType, ParcelType, DocType, ModuleType) {
        return $http.get(config.SERVICE_URL + '/Customer/GetCustomerAdvanceMarginCost', {
            params: {
                OperationZoneId: OperationZoneId,
                CustomerId: CustomerId,
                LogisticCompany: LogisticCompany,
                LogisticType: LogisticType,
                RateType: RateType,
                PackageType: PackageType,
                ParcelType: ParcelType,
                DocType: DocType,
                ModuleType: ModuleType

            }
        });
    };
    var GetShipmentTypes = function (OperationZoneId, CourierCompany, LogisticType, RateType, ModuleType) {
        return $http.get(config.SERVICE_URL + '/BaseRateCard/GetShipmentType', {
            params: {
                OperationZoneId: OperationZoneId,
                
                CourierCompany: CourierCompany,
                LogisticType: LogisticType,
                RateType: RateType,
                ModuleType: ModuleType

            }
        });
    };

    var SaveCustomerAdvanceMarginCost = function (advanceMarginCost) {
        return $http.post(config.SERVICE_URL + '/Customer/SaveCustomerAdvanceMarginCost', advanceMarginCost);
    };

    var GetCustomManifestDetail = function () {
        return $http.get(config.SERVICE_URL + '/Manifest/GetCustomManifestDetail');
    };
    
    var ViewCustomManifestDetail = function (ManifestId) {
        return $http.get(config.SERVICE_URL + '/Manifest/ViewCustomManifestDetail', {
            params: {
                ManifestId: ManifestId
            }
        });
    };

    var GetCustomManifests = function (trackManifest) {
        return $http.post(config.SERVICE_URL + '/Manifest/GetCustomManifests', trackManifest);
    };

    var GenerateCustomManifest = function (ManifestId, ManifestName) {
        return $http.get(config.SERVICE_URL + '/Manifest/GenerateCustomManifest', {
            params: {
                ManifestId: ManifestId,
                ManifestName: ManifestName
            }
        });
    };

    return {
        GetCustomerList: GetCustomerList,
        GetCustomerDetail: GetCustomerDetail,
        SaveCustomer: SaveCustomer,
        DeleteCustomer: DeleteCustomer,
        CheckAccountNumber: CheckAccountNumber,
        GetZoneList: GetZoneList,
        GetZoneDetail: GetZoneDetail,
        GetCustomerMarginCost: GetCustomerMarginCost,
        GetShipmentType: GetShipmentType,
        GetCustomerModules: GetCustomerModules,
        GetCustomerMargin: GetCustomerMargin,
        GetUKShipmentType: GetUKShipmentType,
        SaveCustomerMargin: SaveCustomerMargin,
        ZoneListByOperationZone: ZoneListByOperationZone,
        GetInitial: GetInitial,
        CustomerMarginOption: CustomerMarginOption,
        GetManifestDetail: GetManifestDetail,
        GetNonManifestedShipments: GetNonManifestedShipments,
        GetManifestedShipments: GetManifestedShipments,
        ViewManifest: ViewManifest,
        DownloadReport: DownloadReport,
        GenerateManifest: GenerateManifest,
        GetLogisticServices: GetLogisticServices,
        GetCustomerRateCardDetail: GetCustomerRateCardDetail,
        SaveCustomerRateCardDetail: SaveCustomerRateCardDetail,
        RemoveCustomerLogistic: RemoveCustomerLogistic,
        RemoveShipmentFromManifest: RemoveShipmentFromManifest,
        GetBookingTypes: GetBookingTypes,
        GetOperationZones: GetOperationZones,
        GetCurrentOperationZones: GetCurrentOperationZones,
        GetLogisticItems: GetLogisticItems,
        GetCustomerAdvanceMarginCost: GetCustomerAdvanceMarginCost,
        GetShipmentTypes: GetShipmentTypes,
        SaveCustomerAdvanceMarginCost: SaveCustomerAdvanceMarginCost,
        GetCustomManifestDetail: GetCustomManifestDetail,
        ViewCustomManifestDetail: ViewCustomManifestDetail,
        GetCustomManifests: GetCustomManifests,
        GenerateCustomManifest: GenerateCustomManifest

    };

});