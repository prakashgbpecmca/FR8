angular.module("ngApp.trackAndTraceDashboard")
.factory("TrackAndTraceDashboardService", function ($http, config) {

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
                    PendingDisplay: "Pending",
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
        return $http.get(config.SERVICE_URL + '/AftershipTracking/GetMultipleTrackings', {
            params: {
                userId: userId
            }
        });
    };

    var TrackAndtraceTrackings = function (track) {
        return $http.post(config.SERVICE_URL + '/AftershipTracking/TrackAndTraceDashboard', track);
    };

    return {
        FrayteAftershipStatusTag: FrayteAftershipStatusTag,
        FrayteAftershipStatusImage: FrayteAftershipStatusImage,
        TrackAndTraceDashboard: TrackAndTraceDashboard,
        TrackAndtraceTrackings: TrackAndtraceTrackings,
        AftershipStatus: AftershipStatus
    };

});