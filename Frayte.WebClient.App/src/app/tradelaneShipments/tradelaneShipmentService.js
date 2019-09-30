angular.module('ngApp.mileStone').factory('TradelaneShipmentService', function ($http, config, SessionService) {

    var GetShipmentStatus = function (BookingType) {
        return $http.get(config.SERVICE_URL + '/TradelaneShipments/GetDirectShipmentStatusList',
            {
                params: {
                    BookingType: BookingType
                }
            });
    };

    var GetTradelaneShipments = function (track) {
        return $http.post(config.SERVICE_URL + '/TradelaneShipments/GetTradelaneShipments', track);
    };

    var GetTradelaneCustomers = function () {
        return $http.get(config.SERVICE_URL + '/TradelaneShipments/GetTradlaneCustomers');
    };

    var GetTradelaneAgents = function () {
        return $http.get(config.SERVICE_URL + '/MawbAllocation/GetAgents');
    };

    var GetMawbAllocation = function (TradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + '/MawbAllocation/GetMawbAllocation',
            {
                params: {
                    TradelaneShipmentId: TradelaneShipmentId
                }
            });
    };

    var GetMawbDocumentName = function (TradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + '/MawbAllocation/GetMawbDocumentName',
            {
                params: {
                    TradelaneShipmentId: TradelaneShipmentId
                }
            });
    };

    var GetShipmentHandlerId = function (TradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + '/MawbAllocation/GetShipmentHandlerId',
            {
                params: {
                    TradelaneShipmentId: TradelaneShipmentId
                }
            });
    };

    var GetTimeZoneName = function (TradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + '/MawbAllocation/GetTimeZoneName?TradelaneShipmentId=' + TradelaneShipmentId);
    };

    var GetAirlines = function () {
        return $http.get(config.SERVICE_URL + '/MawbAllocation/GetAirlines');
    };

    var SaveMAWBAllocation = function (MAList) {
        return $http.post(config.SERVICE_URL + '/MawbAllocation/SaveMawbAllocation', MAList);
    };

    var GetTimeZoneList = function () {
        return $http.get(config.SERVICE_URL + '/TimeZone/GetTimeZoneList');
    };

    var DeleteMawbAllocation = function (AllocationId) {
        return $http.get(config.SERVICE_URL + '/MawbAllocation/DeleteMawbAllocation',
            {
                params: {
                    AllocationId: AllocationId
                }
            });
    };

    var DeleteTradelaneShipment = function (TradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + '/TradelaneShipments/DeleteTradelaneShipment',
            {
                params: {
                    TradelaneShipmentId: TradelaneShipmentId
                }
            });
    };

    var GetUpdateTracking = function (TradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + '/TradelaneUpdateTracking/GetTradelaneShipmentTracking',
            {
                params: {
                    TradelaneShipmentId: TradelaneShipmentId
                }
            });
    };

    var SaveUpdateTracking = function (TrackingList) {
        return $http.post(config.SERVICE_URL + '/TradelaneUpdateTracking/SaveTradelaneShipmentOperationalTracking', TrackingList);
    };

    var SaveTradelaneShipmentTracking = function (TrackingList) {
        return $http.post(config.SERVICE_URL + '/TradelaneUpdateTracking/SaveTradelaneTracking', TrackingList);
    };

    var GetAirports = function (TradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + '/TradelaneUpdateTracking/GetAirPorts');
    };

    var GetTradelaneMilestone = function (ShipmentHandlerMethodId) {
        return $http.get(config.SERVICE_URL + '/TradelaneUpdateTracking/GetTradelaneMilestone',
            {
                params: {
                    ShipmentHandlerMethodId: ShipmentHandlerMethodId
                }
            });
    };

    var GetShipmentHandlerMethodId = function (TradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + '/TradelaneUpdateTracking/GetShipmentHandlerMethodId',
            {
                params: {
                    TradelaneShipmentId: TradelaneShipmentId
                }
            });
    };

    var DeleteTradelaneOperationalTracking = function (TradelaneShipmentTrackingId) {
        return $http.get(config.SERVICE_URL + '/TradelaneUpdateTracking/DeleteTradelaneOperationalTracking',
            {
                params: {
                    TradelaneShipmentTrackingId: TradelaneShipmentTrackingId
                }
            });
    };
    var DeleteTradelaneTracking = function (FlightDetailTrackingId) {
        return $http.get(config.SERVICE_URL + '/TradelaneUpdateTracking/DeleteTradelaneTracking',
            {
                params: {
                    FlightDetailTrackingId: FlightDetailTrackingId
                }
            });
    };

    var GetShipmentDetail = function (TradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + '/TradelaneUpdateTracking/GetShipmentDetail',
            {
                params: {
                    TradelaneShipmentId: TradelaneShipmentId
                }
            });
    }; 

    var SendClaimShipment = function (ClaimShipment) {
        return $http.post(config.SERVICE_URL + '/TradelaneShipments/SendClaimShipment', ClaimShipment);
    };

    var SendResolvedClaimShipment = function (ClaimShipment) {
        return $http.post(config.SERVICE_URL + '/TradelaneShipments/SendResolvedClaimShipment', ClaimShipment);
    };

    var SendMawbCorrectionShipment = function (ClaimShipment) {
        return $http.post(config.SERVICE_URL + '/TradelaneShipments/SendMawbCorrectionShipment', ClaimShipment);
    }; 

    var GetAgentMail = function (TradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + '/TradelaneShipments/GetAgentMail',
            {
                params: {
                    TradelaneShipmentId: TradelaneShipmentId
                }
            });
    };

    var GetTrackingStatus = function (TradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + '/TradelaneUpdateTracking/GetShipmentTrackingStatus',
            {
                params: {
                    TradelaneShipmentId: TradelaneShipmentId
                }
            });
    };

    var AddMAWBCustomized = function (mawbCustomizefield) {
        return $http.post(config.SERVICE_URL + '/TradelaneShipments/AddMAWBCustomized', mawbCustomizefield);
    };

    var GetMawbCustomizePdf = function (TradelaneShipmentId) {
        return $http.get(config.SERVICE_URL + '/TradelaneShipments/GetMawbCustomizePdf?TradelaneShipmentId=' + TradelaneShipmentId);
    };

    return {
        GetTimeZoneName: GetTimeZoneName,
        GetShipmentStatus: GetShipmentStatus,
        GetTradelaneShipments: GetTradelaneShipments,
        GetTradelaneCustomers: GetTradelaneCustomers,
        GetTradelaneAgents: GetTradelaneAgents,
        GetMawbAllocation: GetMawbAllocation,
        GetShipmentHandlerId: GetShipmentHandlerId,
        GetAirlines: GetAirlines,
        SaveMAWBAllocation: SaveMAWBAllocation,
        GetTimeZoneList: GetTimeZoneList,
        DeleteMawbAllocation: DeleteMawbAllocation,
        DeleteTradelaneShipment: DeleteTradelaneShipment,
        GetUpdateTracking: GetUpdateTracking,
        SaveUpdateTracking: SaveUpdateTracking,
        SaveTradelaneShipmentTracking: SaveTradelaneShipmentTracking,
        GetAirports: GetAirports,
        GetShipmentHandlerMethodId: GetShipmentHandlerMethodId,
        GetTradelaneMilestone: GetTradelaneMilestone,
        DeleteTradelaneOperationalTracking: DeleteTradelaneOperationalTracking,
        DeleteTradelaneTracking: DeleteTradelaneTracking,
        GetShipmentDetail: GetShipmentDetail,
        SendClaimShipment: SendClaimShipment,
        GetAgentMail: GetAgentMail,
        SendResolvedClaimShipment: SendResolvedClaimShipment,
        SendMawbCorrectionShipment: SendMawbCorrectionShipment,
        GetTrackingStatus: GetTrackingStatus,
        GetMawbDocumentName: GetMawbDocumentName,
        AddMAWBCustomized: AddMAWBCustomized,
        GetMawbCustomizePdf: GetMawbCustomizePdf
    };
});