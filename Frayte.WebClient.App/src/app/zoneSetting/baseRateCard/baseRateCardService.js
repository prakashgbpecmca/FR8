
angular.module('ngApp.baseRateCard').factory('ZoneBaseRateCardService', function ($http, config, SessionService) {

    var UpdateLogisticServiceDuration = function (obj) {
        return $http.post(config.SERVICE_URL + '/BaseRateCard/UpdateLogisticSerice', obj);
    };

    var getLogisticServiceDuration = function (OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType) {
        return $http.get(config.SERVICE_URL + '/BaseRateCard/GetLogisrticServiceDuration', {
            params: {
                OperationZoneId: OperationZoneId,
                LogisticType: LogisticType,
                CourierCompany: CourierCompany,
                RateType: RateType,
                ModuleType: ModuleType
            }
        });
    };

    var GetOperationZone = function () {
        return $http.get(config.SERVICE_URL + '/BaseRateCard/GetOperationZone');
    };

    var GetZones = function (OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType) {
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

    var GetZoneBaseRateCard = function (OperationZoneId, LogisticType, CourierCompany, RateType, PackageType, ParcelType, DocType, ModuleType) {
        return $http.get(config.SERVICE_URL + '/BaseRateCard/GetZoneBaseRateCard', {
            params: {
                OperationZoneId: OperationZoneId,
                LogisticType: LogisticType,
                CourierCompany: CourierCompany,
                RateType: RateType,
                PackageType: PackageType,
                ParcelType: ParcelType,
                DocType: DocType,
                ModuleType: ModuleType
            }
        });
    };

    var GetZoneBaseRateCardLimit = function (OperationZoneId, LogisticType, PackageType, ParcelServiceType, LogisticServiceType) {
        return $http.get(config.SERVICE_URL + '/BaseRateCard/GetZoneBaseRateCardLimit', {
            params: {
                OperationZoneId: OperationZoneId,
                LogisticType: LogisticType,
                PackageType: PackageType,
                ParcelServiceType: ParcelServiceType,
                LogisticServiceType: LogisticServiceType
            }
        });
    };

    var GetCourierAccount = function (OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType) {
        return $http.get(config.SERVICE_URL + '/CourierAccount/GetCourierAccounts', {
            params: {
                OperationZoneId: OperationZoneId,
                LogisticType: LogisticType,
                CourierCompany: CourierCompany,
                RateType: RateType,
                ModuleType: ModuleType
            }
        });
    };

    var GetShipmentType = function (OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType) {
        return $http.get(config.SERVICE_URL + '/BaseRateCard/GetShipmentType', {
            params: {
                OperationZoneId: OperationZoneId,
                LogisticType: LogisticType,
                CourierCompany: CourierCompany,
                RateType: RateType,
                ModuleType: ModuleType
            }
        });
    };

    var GetShipmentDocTypes = function () {
        return $http.get(config.SERVICE_URL + '/Customer/GetShipmentType');
    };

    var GetWeight = function (OperationZoneId, LogisticType, CourierCompany, RateType, PackageType, ParcelType, ModuleType) {
        return $http.get(config.SERVICE_URL + '/BaseRateCard/GetWeight', {
            params: {
                OperationZoneId: OperationZoneId,
                LogisticType: LogisticType,
                CourierCompany: CourierCompany,
                RateType: RateType,
                PackageType: PackageType,
                ParcelType: ParcelType,
                ModuleType: ModuleType
            }
        });
    };

    var SaveZoneBaseRateCard = function (_ratecard) {
        return $http.post(config.SERVICE_URL + '/BaseRateCard/EditZoneBaseRateCard', _ratecard);
    };

    var GetOperationZoneCurrencyCode = function (OperationZoneId, exchangeType) {
        return $http.get(config.SERVICE_URL + '/BaseRateCard/GetOperationZoneCurrencyCode',
            {
                params: {
                    OperationZoneId: OperationZoneId,
                    exchangeType: exchangeType
                }
            });
    };

    var SaveZoneBaseRateCardLimit = function (_baseRateCardLimit) {
        return $http.post(config.SERVICE_URL + '/BaseRateCard/SaveZoneBaseRateCardLimit', _baseRateCardLimit);
    };

    var GetDimensionBaseRateCard = function (OperationZoneId, LogisticType, LogisticServiceType, ParcelType, PackageType) {
        return $http.get(config.SERVICE_URL + '/BaseRateCard/GetDimensionBaseRateCard',
     {
         params: {
             OperationZoneId: OperationZoneId,
             LogisticType: LogisticType,
             LogisticServiceType: LogisticServiceType,
             ParcelType: ParcelType,
             PackageType: PackageType
         }
     });
    };

    var SaveDimensionBaseRate = function (_baseRateCardLimit) {
        return $http.post(config.SERVICE_URL + '/BaseRateCard/SaveDimensionBaseRate', _baseRateCardLimit);
    };

    var GetLogisticItemList = function (OperationZoneId) {
        return $http.get(config.SERVICE_URL + '/LogisticItem/GetLogisticItemList',
            {
                params: {
                    OperationZoneId: OperationZoneId
                }
            });
    };

    var GetAddOnRate = function (OperationZoneId, LogisticType, CourierCompany, RateType, ParcelType, PackageType, ModuleType) {
        if (LogisticType === 'UKShipment') {
            return $http.get(config.SERVICE_URL + '/BaseRateCard/GetZoneUKAddOnRate', {
                params: {
                    OperationZoneId: OperationZoneId,
                    LogisticType: LogisticType,
                    CourierCompany: CourierCompany,
                    RateType: RateType,
                    ParcelType: ParcelType,
                    PackageType: PackageType,
                    ModuleType: ModuleType
                }
            });
        }
        else {
            return $http.get(config.SERVICE_URL + '/BaseRateCard/GetZoneAddOnRate', {
                params: {
                    OperationZoneId: OperationZoneId,
                    LogisticType: LogisticType,
                    CourierCompany: CourierCompany,
                    RateType: RateType,
                    ParcelType: ParcelType,
                    PackageType: PackageType,
                    ModuleType: ModuleType
                }
            });
        }
    };

    var UpdateUKAddonRate = function (UKAddOnRate) {
        return $http.post(config.SERVICE_URL + '/BaseRateCard/UpdateUKAddOnRate', UKAddOnRate);
    };

    var UpdateAddOnRate = function (AddOnRate) {
        return $http.post(config.SERVICE_URL + '/BaseRateCard/UpdateAddOnRate', AddOnRate);
    };

    var GetValidAddOnRateId = function (OperationZoneId, LogisticType, CourierCompany, RateType, ParcelType, PackageType, ModuleType) {
        return $http.get(config.SERVICE_URL + '/BaseRateCard/GetAddOnRateIdValid', {
            params: {
                OperationZoneId: OperationZoneId,
                LogisticType: LogisticType,
                CourierCompany: CourierCompany,
                RateType: RateType,
                ParcelType: ParcelType,
                PackageType: PackageType,
                ModuleType: ModuleType
            }
        });
    };

    var DownloadExcelTemplate = function (CourierCompany, LogisticType, RateType) {
        return $http.get(config.SERVICE_URL + '/BaseRateCardUpdate/DownloadRateCardExcelTemplate', {
            params: {
                CourierCompany: CourierCompany,
                LogisticType: LogisticType,
                RateType: RateType
            }
        });
    };

    return {
        GetOperationZone: GetOperationZone,
        GetOperationZoneCurrencyCode: GetOperationZoneCurrencyCode,
        GetZones: GetZones,
        GetZoneBaseRateCard: GetZoneBaseRateCard,
        GetShipmentType: GetShipmentType,
        GetCourierAccount: GetCourierAccount,
        GetWeight: GetWeight,
        SaveZoneBaseRateCard: SaveZoneBaseRateCard,
        GetZoneBaseRateCardLimit: GetZoneBaseRateCardLimit,
        SaveZoneBaseRateCardLimit: SaveZoneBaseRateCardLimit,
        GetDimensionBaseRateCard: GetDimensionBaseRateCard,
        SaveDimensionBaseRate: SaveDimensionBaseRate,
        GetLogisticItemList: GetLogisticItemList,
        GetShipmentDocTypes: GetShipmentDocTypes,
        GetAddOnRate: GetAddOnRate,
        UpdateUKAddonRate: UpdateUKAddonRate,
        UpdateAddOnRate: UpdateAddOnRate,
        GetValidAddOnRateId: GetValidAddOnRateId,
        UpdateLogisticServiceDuration: UpdateLogisticServiceDuration,
        GetLogisticServiceDuration: getLogisticServiceDuration,
        DownloadExcelTemplate: DownloadExcelTemplate
    };

});