angular.module('ngApp.directBooking').factory('DirectShipmentService', function ($http, config, SessionService) {
  
    var GetDirectShipments = function (track) {
        return $http.post(config.SERVICE_URL + '/DirectBooking/GetDirectShipmentsList', track);
    };

    var GetDirectShipmentStatus = function (BookingType, UserId) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/GetDirectShipmentStatusList', {
            params: {
                BookingType: BookingType,
                UserId: UserId
            }
        });
    };

    var GetDirectBookingDetail = function (directShipmentId, CallingType) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/GetDirectBookingDetail', {
            params: {
                directShipmentId: directShipmentId,
                CallingType: CallingType
            }
        });
    };

    var DeleteDirectBooking = function (DirectBookingId) {

        return $http.get(config.SERVICE_URL + '/DirectBooking/DeleteDirectBooking', {
            params: {
                DirectBookingId: DirectBookingId
            }
        });
    };

    var GenerateTrackAndTraceReport = function (trackdetail) {
        return $http.post(config.SERVICE_URL + '/DirectBooking/GenerateTrackAndTraceExcel', trackdetail);
    };

    var GenerateSupplementoryCharges = function (DirectShipmentId) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/GenerateSupplementoryCharges', {
            params: {
                DirectShipmentId: DirectShipmentId
            }
        });
    };

    var GenerateCommercialInvoice = function (DirectShipmentId, CustomerName) {

        return $http.get(config.SERVICE_URL + '/DirectBooking/CreateCommercialInvoice', {
            params: {
                DirectShipmentId: DirectShipmentId,
                CustomerName: CustomerName
            }
        });
    };

    var MarkDirectShipmentDraftAsPublic = function (DirectShipmentDraftId, IsPublic) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/MarkDraftShipmentAsPublic', {
            params: {
                DirectShipmentDraftId: DirectShipmentDraftId,
                IsPublic: IsPublic
            }
        });
    };

    var FrayteAftershipStatusTag = {
        Pending: 0,
        InfoReceived: 1,
        InTransit: 2,
        OutForDelivery: 3,
        AttemptFail: 4,
        Delivered: 5,
        Exception: 6,
        Expired: 7
    };

    var AftershipStatus =
                {
                    Pending: "Pending",
                    PendingDisplay: "Shipment Booked",
                    InfoReceived: "InfoReceived",
                    InfoReceivedDisplay: "Info Received",
                    InTransit: "InTransit",
                    InTransitDisplay: "In Transit",
                    OutForDelivery: "OutForDelivery",
                    OutForDeliveryDisplay: "Out For Delivery",
                    AttemptFail: "AttemptFail",
                    AttemptFailDisplay: "Failed Attempt",
                    Delivered: "Delivered",
                    DeliveredDisplay: "Delivered",
                    Exception: "Exception",
                    ExceptionDislay: "Exception",
                    Expired: "Expired",
                    ExpiredDisplay: "Expired"
                };
    var FrayteAftershipStatusImage = {
        Pending: 'Pending.png',
        InfoReceived: 'InfoReceived.png',
        InTransit: 'InTransit.png',
        OutForDelivery: 'OutForDelivery.png',
        AttemptFail: 'FailedAttemt.png',
        Delivered: 'Delivered.png',
        Exception: 'Exception.png',
        Expired: 'Expired.png'
    };

    var TrackAndTraceDashboard = function (userId) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/GetMultipleTrackings', {
            params: {
                userId: userId
            }
        });
    };

    var TrackAndtraceTrackings = function (track) {
        return $http.post(config.SERVICE_URL + '/DirectBooking/TrackAndTraceDashboard', track);
    };

    return {
        GetDirectShipments: GetDirectShipments,
        GetDirectShipmentStatus: GetDirectShipmentStatus,
        GetDirectBookingDetail: GetDirectBookingDetail,
        DeleteDirectBooking: DeleteDirectBooking,
        GenerateTrackAndTraceReport: GenerateTrackAndTraceReport,
        GenerateCommercialInvoice: GenerateCommercialInvoice,
        GenerateSupplementoryCharges: GenerateSupplementoryCharges,
        MarkDirectShipmentDraftAsPublic: MarkDirectShipmentDraftAsPublic,
        FrayteAftershipStatusTag: FrayteAftershipStatusTag,
        FrayteAftershipStatusImage: FrayteAftershipStatusImage,
        TrackAndTraceDashboard: TrackAndTraceDashboard,
        TrackAndtraceTrackings: TrackAndtraceTrackings,
        AftershipStatus: AftershipStatus
    };

});