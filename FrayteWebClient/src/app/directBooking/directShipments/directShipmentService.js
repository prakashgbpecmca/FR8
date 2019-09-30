angular.module('ngApp.directBooking').factory('DirectShipmentService', function ($http, config, SessionService) {

    var GetDirectShipments = function (track) {
        return $http.post(config.SERVICE_URL + '/DirectBooking/GetDirectShipmentsList',track);
    };
    var GetDirectShipmentStatus = function (BookingType,UserId) {
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

    var GenerateCommercialInvoice = function (DirectShipmentId) {

        return $http.get(config.SERVICE_URL + '/DirectBooking/CreateCommercialInvoice', {
            params: {
                DirectShipmentId: DirectShipmentId
            }
        });
    };

    return {
        GetDirectShipments: GetDirectShipments,
        GetDirectShipmentStatus: GetDirectShipmentStatus,
        GetDirectBookingDetail: GetDirectBookingDetail,
        DeleteDirectBooking: DeleteDirectBooking,
        GenerateTrackAndTraceReport: GenerateTrackAndTraceReport,
        GenerateCommercialInvoice: GenerateCommercialInvoice


    };

});