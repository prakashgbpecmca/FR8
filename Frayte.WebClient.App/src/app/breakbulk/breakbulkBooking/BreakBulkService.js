angular.module('ngApp.breakBulk').factory('BreakBulkService', function ($http, config) {

    var GetInitials = function (userId) {
        return $http.get(config.SERVICE_URL + '/BreakBulk/GetInitials?userId=' + userId);
    };

    var GenerateConsignmentNumber = function (userId) {
        return $http.get(config.SERVICE_URL + '/BreakBulk/GenerateConsignmentNumber?userId=' + userId);
    };

    var GetAirlines = function (countryId) {
        return $http.get(config.SERVICE_URL + '/BreakBulk/AirPortList', {
            params: {
                countryId: countryId
            }
        });
    };

    var SaveAddressBook = function (FrayteAddressBook) {
        return $http.post(config.SERVICE_URL + '/DirectBooking/EditCustomerAddress', FrayteAddressBook);
    };

    var SaveCustomerCustomField = function (CustomField) {
        return $http.post(config.SERVICE_URL + '/BreakBulk/SaveCustomerCustomField', CustomField);
    };
    var Gethubs = function () {
        return $http.get(config.SERVICE_URL + '/ExpressManifest/GetHubs');
    };

    var GetHubAddress = function (countryId, postcode, state) {
        return $http.get(config.SERVICE_URL + '/BreakBulk/GetHubAddress', {
            params: {
                countryId: countryId,
                postcode: postcode,
                state: state
            }
        });
    };

    var GetCountryState = function (CountryId) {
        return $http.get(config.SERVICE_URL + "/Express/GetCountryState", {
            params: {
                CountryId: CountryId
            }
        });
    };

    var GetFromCountryState = function (CountryId) {
        return $http.get(config.SERVICE_URL + "/Express/GetFromCountryState", {
            params: {
                CountryId: CountryId
            }
        });
    };

    var GetBreakBulkBookingDetail = function (PurchaseOrderId) {
            return $http.get(config.SERVICE_URL + '/BreakBulk/GetBreakBulkBookingDetail',
                {
                    params: {
                        PurchaseOrderId: PurchaseOrderId
                    }
                });
        };

    //var SaveCustomerShipmentType = function (obj) {
    //    return $http.post(config.SERVICE_URL + '/BreakBulk/SaveCustomerShipmentType', obj);
    //};

    var SavePurchaseOrderData = function (shipment) {
        return $http.post(config.SERVICE_URL + '/BreakBulk/SavePurchaseOrderData', shipment);
    };

    var BreakBulkHubServices = function (serviceObj) {
        return $http.post(config.SERVICE_URL + "/BreakBulk/BreakBulkHubServices", serviceObj);
    };

    var Canadaproductcatalog = function (CanadaProductCatalog) {
        return $http.post(config.SERVICE_URL + '/ExpressManifest/AddCanadaProductCatalog', CanadaProductCatalog);
    };

    var Swissaproductcatalog = function (SwissProductCatalog) {
        return $http.post(config.SERVICE_URL + '/ExpressManifest/AddSWissProductCatalog', SwissProductCatalog);
    };

    var USAproductcatalog = function (USAProductCatalog) {
        return $http.post(config.SERVICE_URL + '/ExpressManifest/AddUSAProductCatalog', USAProductCatalog);
    };

    var UKproductcatalog = function (UKProductCatalog) {
        return $http.post(config.SERVICE_URL + '/ExpressManifest/AddUKProductCatalog', UKProductCatalog);
    };

    var Japanproductcatalog = function (JapanProductCatalog) {
        return $http.post(config.SERVICE_URL + '/ExpressManifest/AddNRTProductCatalog', JapanProductCatalog);
    };

    var Singaporeproductcatalog = function (UKProductCatalog) {
        return $http.post(config.SERVICE_URL + '/ExpressManifest/AddSINProductCatalog', UKProductCatalog);
    };

    var Norwayproductcatalog = function (UKProductCatalog) {
        return $http.post(config.SERVICE_URL + '/ExpressManifest/AddNorwayProductCatalog', UKProductCatalog);
    };

    var getCurrency = function () {
        return $http.get(config.SERVICE_URL + '/ExpressManifest/GetCurrency');
    };

    var getCanadaProductCatalog = function (track) {
        return $http.post(config.SERVICE_URL + '/ExpressManifest/FetchCANProductcatalog', track);
    };

    var getSwissProductCatalog = function (track) {
        return $http.post(config.SERVICE_URL + '/ExpressManifest/FetchSwissProductcatalog', track);
    };

    var getUKProductCatalog = function (track) {
        return $http.post(config.SERVICE_URL + '/ExpressManifest/FetchUKProductcatalog', track);
    };

    var getUSAProductCatalog = function (track) {
        return $http.post(config.SERVICE_URL + '/ExpressManifest/FetchUSAProductcatalog', track);
    };

    var getJapanProductCatalog = function (track) {
        return $http.post(config.SERVICE_URL + '/ExpressManifest/FetchNRTProductcatalog', track);
    };

    var getSingaporeProductCatalog = function (track) {
        return $http.post(config.SERVICE_URL + '/ExpressManifest/FetchSINProductcatalog', track);
    };

    var getNorwayProductCatalog = function (track) {
        return $http.post(config.SERVICE_URL + '/ExpressManifest/FetchNorwayProductcatalog', track);
    };

    var editCatalog = function (ProductcatalogId) {
        return $http.get(config.SERVICE_URL + '/ExpressManifest/EditProductCatalog?ProductcatalogId=' + ProductcatalogId);
    };

    return {
        Gethubs: Gethubs,
        GetInitials: GetInitials,
        GetAirlines: GetAirlines,
        GetHubAddress: GetHubAddress,
        SaveAddressBook: SaveAddressBook,
        SaveCustomerCustomField: SaveCustomerCustomField,
        SavePurchaseOrderData: SavePurchaseOrderData,
        GetBreakBulkBookingDetail: GetBreakBulkBookingDetail,
        //SaveCustomerShipmentType: SaveCustomerShipmentType,
        GetFromCountryState: GetFromCountryState,
        GetCountryState: GetCountryState,
        BreakBulkHubServices: BreakBulkHubServices,
        GenerateConsignmentNumber: GenerateConsignmentNumber,
        Canadaproductcatalog: Canadaproductcatalog,
        Swissaproductcatalog: Swissaproductcatalog,
        USAproductcatalog: USAproductcatalog,
        UKproductcatalog: UKproductcatalog,
        Japanproductcatalog: Japanproductcatalog,
        Singaporeproductcatalog: Singaporeproductcatalog,
        Norwayproductcatalog: Norwayproductcatalog,
        getCanadaProductCatalog: getCanadaProductCatalog,
        getSwissProductCatalog: getSwissProductCatalog,
        getUKProductCatalog: getUKProductCatalog,
        getUSAProductCatalog: getUSAProductCatalog,
        getJapanProductCatalog: getJapanProductCatalog,
        getSingaporeProductCatalog: getSingaporeProductCatalog,
        getNorwayProductCatalog: getNorwayProductCatalog,
        getCurrency: getCurrency,
        editCatalog: editCatalog
    };
});