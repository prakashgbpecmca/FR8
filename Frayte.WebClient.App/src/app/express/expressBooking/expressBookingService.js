angular.module('ngApp.express').factory("ExpressBookingService", function ($http, config) {

    var SendLabelEmail = function (labelObj) {
        return $http.post(config.SERVICE_URL + "/Express/LabelEmail", labelObj);
    };

    var PrintLabel = function (id, type) {
        return $http.get(config.SERVICE_URL + '/Express/PrintLabel', {
            params: {
                id: id,
                type: type
            }
        });
    };

    var SaveShipment = function (shipment) {
        return $http.post(config.SERVICE_URL + "/Express/SaveShipment", shipment);
    };

    var ExpressHubServices = function (serviceObj) {
        return $http.post(config.SERVICE_URL + "/Express/ExpressHubServices", serviceObj);
    };

    var AWBlabelPath = function (AWBNumber) {
        return $http.get(config.SERVICE_URL + '/Express/AWBlabelPath', {
            params: {
                AWBNumber: AWBNumber
            }
        });
    };

    var TradelaneShipmentDocument = {

        MAWB: "MAWB",
        MAWBDisplay: "Master Air Way Bill - MAWB",
        HAWB: "HAWB",
        HAWBDisplay: "House Air Way Bill - HAWB",
        Manifest: "Manifest",
        ManifestDisplay: "Manifest",
        ShipmentDetail: "ShipmentDetail",
        ShipmentDetailDisplay: "Shipment Detail",
        BatteryForm: "BatteryForm",
        BatteryFormDisplay: "Batter Declaration",
        CoLoadForm: "CoLoadForm",
        CoLoadFormDisplay: "Coload Form",
        OtherDocument: "OtherDocument",
        OtherDocumentDisplay: "Other Document",
        CartonLabel: "CartonLabel",
        CartonLabelDisplay: "Carton Label"
    };

    var toggleState = function (Country) {
        if (Country) {
            if (Country.Code === 'HKG' || Country.Code === 'GBR') {
                return false;
            }
            else {
                return true;
            }
        }
    };

    var BookingInitials = function (userId) {
        return $http.get(config.SERVICE_URL + '/Express/BookingInitials', {
            params: {
                userId: userId
            }
        });
    };

    var getHubAddress = function (countryId, postcode , state) {
        return $http.get(config.SERVICE_URL + "/Express/GetHubAddress", {
            params: {
                countryId: countryId,
                postcode: postcode,
                state: state
            }
        });
    };

    var GetBookingDetail = function (shipmentId, callingType) {
        return $http.get(config.SERVICE_URL + "/Express/ScannedShipmentDetail", {
            params: {
                shipmentId: shipmentId,
                callingType: callingType
            }
        });
    };
    var GetCustomerAWBs = function (customerId, AWB) {
        return $http.get(config.SERVICE_URL + "/Express/GetCustomerAWBs", {
            params: {
                customerId: customerId,
                AWB: AWB
            }
        });
    };

    var AirPorts = function (countryId) {
        return $http.get(config.SERVICE_URL + "/TradelaneBooking/AirPorts", {
            params: {
                countryId: countryId
            }
        });
    };

    var PlaceBooking = function (shipment) {
        return $http.post(config.SERVICE_URL + "/TradelaneBooking/PlaceBooking", shipment);
    };

    var GetTradelanePackageWeight = function (tradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + "/TradelaneBooking/GetTradelanePackageWeight", {
            params: {
                tradelaneShipmentId: tradelaneShipmentId
            }
        });
    };

    var getShipmentPackages = function (track) {

        return $http.post(config.SERVICE_URL + "/TradelaneBooking/GetShipmentPackages", track);
    };

    var IsAllHawbAssigned = function (tradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + "/TradelaneBooking/IsAllHawbAssigned", {
            params: {
                tradelaneShipmentId: tradelaneShipmentId
            }
        });
    };

    var AssigneedHAWB = function (packages) {
        return $http.post(config.SERVICE_URL + "/TradelaneBooking/AssigneedHAWB", packages);
    };

    var GetGroupedHAWBPieces = function (tradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + "/TradelaneBooking/GetGroupedHAWBPieces", {
            params: {
                tradelaneShipmentId: tradelaneShipmentId
            }
        });
    };

    var GetCustomers = function (userId, moduleType) {
        return $http.get(config.SERVICE_URL + "/Express/GetCustomers", {
            params: {
                userId: userId,
                moduleType: moduleType
            }
        });
    };

    var GetProductDescription = function (CustomerId, HubId) {
        return $http.get(config.SERVICE_URL + "/Express/FetchProductcatalog", {
            params: {
                CustomerId: CustomerId,
                HubId: HubId
            }
        });
    };

    var UpdateHAWBNumber = function (shipmentId, hawbNumber) {
        return $http.get(config.SERVICE_URL + "/TradelaneBooking/UpdateHAWBNumber", {
            params: {
                shipmentId: shipmentId,
                hawbNumber: hawbNumber
            }
        });
    };

    var ShipmentHAWB = function (shipmentId) {
        return $http.get(config.SERVICE_URL + "/TradelaneShipments/GetShipmentHAWB", {
            params: {
                shipmentId: shipmentId
            }
        });
    };

    var GetShipmentDocuments = function (shipmentId) {
        return $http.get(config.SERVICE_URL + "/TradelaneShipments/GetShipmentDocuments", {
            params: {
                shipmentId: shipmentId
            }
        });
    };

    var TradelaneShipmentDocuments = function (userId, shipmentId) {
        return $http.get(config.SERVICE_URL + "/TradelaneBooking/TradelaneShipmentDocuments", {
            params: {
                userId: userId,
                shipmentId: shipmentId
            }
        });
    };

    var CreateDocument = function (shipmentId, userId, documentType, documentTypeName) {
        return $http.get(config.SERVICE_URL + "/TradelaneShipments/CreateDocument", {
            params: {
                tradelaneShipmentId: shipmentId,
                userId: userId,
                documentType: documentType,
                documentTypeName: documentTypeName
            }
        });
    };

    var removeDocument = function (TradelaneShipmentDocumentId) {
        return $http.get(config.SERVICE_URL + "/TradelaneBooking/RemoveTradelaneDocument", {
            params: {
                TradelaneShipmentDocumentId: TradelaneShipmentDocumentId
            }
        });
    };

    var userDefalutAddress = function (userId) {
        return $http.get(config.SERVICE_URL + "/TradelaneBooking/UserDefaultAddresses", {
            params: {
                userId: userId
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

    return {
        SendLabelEmail:SendLabelEmail,
        PrintLabel:PrintLabel,
        SaveShipment: SaveShipment,
        ExpressHubServices: ExpressHubServices,
        AWBlabelPath: AWBlabelPath,
        getHubAddress: getHubAddress,
        GetCustomerAWBs: GetCustomerAWBs,
        userDefalutAddress: userDefalutAddress,
        GetTradelanePackageWeight: GetTradelanePackageWeight,
        removeDocument: removeDocument,
        CreateDocument: CreateDocument,
        TradelaneShipmentDocument: TradelaneShipmentDocument,
        TradelaneShipmentDocuments: TradelaneShipmentDocuments,
        GetShipmentDocuments: GetShipmentDocuments,
        ShipmentHAWB: ShipmentHAWB,
        GetBookingDetail: GetBookingDetail,
        UpdateHAWBNumber: UpdateHAWBNumber,
        GetCustomers: GetCustomers,
        GetGroupedHAWBPieces: GetGroupedHAWBPieces,
        AssigneedHAWB: AssigneedHAWB,
        IsAllHawbAssigned: IsAllHawbAssigned,
        getShipmentPackages: getShipmentPackages,
        toggleState: toggleState,
        BookingInitials: BookingInitials,
        AirPorts: AirPorts,
        PlaceBooking: PlaceBooking,
        GetCountryState: GetCountryState,
        GetFromCountryState: GetFromCountryState,
        GetProductDescription: GetProductDescription
    };
});