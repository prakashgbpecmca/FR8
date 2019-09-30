angular.module('ngApp.tradelaneBooking').factory("TradelaneBookingService", function ($http, config) {

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
            else if (Country.Code === 'USA') {
                return false;
            }
            else {
                return true;
            }
        }
    };

    var BookingInitials = function (userId) {
        return $http.get(config.SERVICE_URL + '/TradelaneBooking/BookingInitials', {
            params: {
                userId: userId
            }
        });
    };

    var DeleteDirectShipmentPackage = function (TradelaneShipmentDetailId) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/DeletePcsDetail', {
            params: {
                TradelaneShipmentDetailId: TradelaneShipmentDetailId
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

    var GetHAWBAddress = function (HAWB) {
        return $http.get(config.SERVICE_URL + "/TradelaneBooking/GetHAWBAddress?HAWB=" + HAWB);
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
        return $http.get(config.SERVICE_URL + "/TradelaneBooking/GetCustomers", {
            params: {
                userId: userId,
                moduleType: moduleType
            }
        });
    };
    var upMAWBAsHAWB = function (obj) {
        return $http.post(config.SERVICE_URL + "/TradelaneBooking/ShipmentMAWBAsHAWB", obj);
    };

    var SavePcsHAWB = function (Pcs) {
        return $http.post(config.SERVICE_URL + "/TradelaneBooking/SavePcsHAWB", Pcs);
    };
    var UpdateHAWBNumber = function (shipmentId, hawbNumber) {
        return $http.get(config.SERVICE_URL + "/TradelaneBooking/UpdateHAWBNumber", {
            params: {
                shipmentId: shipmentId,
                hawbNumber: hawbNumber
            }
        });
    };

    var GetBookingDetail = function (shipmentId, callingType) {
        return $http.get(config.SERVICE_URL + "/TradelaneBooking/TradelaneBookingDetails", {
            params: {
                shipmentId: shipmentId,
                callingType: callingType
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

    var AssignedHAWBDetail = function (TradelanshipmentId) {
        return $http.get(config.SERVICE_URL + "/TradelaneBooking/AssignedHAWBDetail", {
            params: {
                TradelanshipmentId: TradelanshipmentId
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

    var addAirport = function (CountryId, AirportName, AirportCode) {
        return $http.get(config.SERVICE_URL + "/TradelaneBooking/AddAirport", {
            params: {
                CountryId: CountryId,
                AirportName: AirportName,
                AirportCode: AirportCode
            }
        });
    };

    var SaveHAWBAddress = function (hawbaddress) {
        return $http.post(config.SERVICE_URL + '/TradelaneBooking/SaveHAWBAddress', hawbaddress);
    };

    var IsHAWBAddressExist = function (HAWBNo) {
        return $http.get(config.SERVICE_URL + '/TradelaneBooking/IsHAWBAddressExist?HAWBNo=' + HAWBNo);
    };

    var UpdateHAWBAddress = function (address) {
        return $http.post(config.SERVICE_URL + '/TradelaneBooking/UpdateHAWBAddress', address);
    };

    var getHAWB = function (TradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + '/TradelaneBooking/getHAWB?TradelaneShipmentId=' + TradelaneShipmentId);
    };

    var CreateHAWBLabel = function (HAWB, index, TradelaneShipmentDetailId) {
        return $http.get(config.SERVICE_URL + '/TradelaneBooking/CreateHAWBLabel', {
            params: {
                HAWB: HAWB,
                index: index,
                TradelaneShipmentDetailId: TradelaneShipmentDetailId
            }
        });
    };

    var DownloadHAWBLabelDataSource = function (fileName, TradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + '/TradelaneBooking/DownloadHAWBLabelDataSource', {
            params: {
                fileName: fileName,
                TradelaneShipmentId: TradelaneShipmentId
            }
        });
    };

    var CreateDestinationManifest = function (TradelaneShipmentId, userId) {
        return $http.get(config.SERVICE_URL + '/TradelaneBooking/CreateDestinationManifest', {
            params: {
            TradelaneShipmentId: TradelaneShipmentId,
            userId: userId
            }
        });
    };

    var DownloadDestinationManifest = function (fileName, TradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + '/TradelaneBooking/DownloadDestinationManifest', {
            params: {
                fileName: fileName,
                TradelaneShipmentId: TradelaneShipmentId
            }
        });
    };

    return {
        userDefalutAddress: userDefalutAddress,
        GetTradelanePackageWeight: GetTradelanePackageWeight,
        removeDocument: removeDocument,
        AssignedHAWBDetail: AssignedHAWBDetail,
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
        DeleteDirectShipmentPackage: DeleteDirectShipmentPackage,
        AirPorts: AirPorts,
        PlaceBooking: PlaceBooking,
        addAirport: addAirport,
        upMAWBAsHAWB: upMAWBAsHAWB,
        SavePcsHAWB: SavePcsHAWB,
        SaveHAWBAddress: SaveHAWBAddress,
        GetHAWBAddress: GetHAWBAddress,
        IsHAWBAddressExist: IsHAWBAddressExist,
        UpdateHAWBAddress: UpdateHAWBAddress,
        getHAWB: getHAWB,
        CreateHAWBLabel: CreateHAWBLabel,
        DownloadHAWBLabelDataSource: DownloadHAWBLabelDataSource,
        CreateDestinationManifest: CreateDestinationManifest,
        DownloadDestinationManifest: DownloadDestinationManifest
    };
});
