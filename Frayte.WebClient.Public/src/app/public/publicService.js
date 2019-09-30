angular.module('ngApp.public').factory('PublicService', function ($http, config, SessionService) {

    var AdminConfirmReject = function (actionDetail) {

        return $http.post(config.SERVICE_URL + '/Customer/CustomerAdminAction', actionDetail);
    };

    var GetLatestTermAndConditionPublic = function (OperationZoneId, TermAndCondtionType, shortCode) {
        return $http.get(config.SERVICE_URL + '/TermAndCondition/GetlatestTermsAndCondition', {
            params: {
                OperationZoneId: OperationZoneId,
                TermAndCondtionType: TermAndCondtionType,
                shortCode: shortCode
            }
        });
    };

    var DirectBookingConfirmReject = function (CustomerAction, DirectShipmentId) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/DirectBookingReject', {
            params: {
                CustomerAction: CustomerAction,
                DirectShipmentId: DirectShipmentId
            }
        });
    };

    var CustomerAction = function (confirmationDetail) {
        return $http.post(config.SERVICE_URL + '/TradelaneBooking/CustomerAction', confirmationDetail);
    };

    var MAWBInitials = function () {
        return $http.get(config.SERVICE_URL + '/BookingInitials/DirectBookingReject');
    };
    var GetTradelaneAgents = function () {
        return $http.get(config.SERVICE_URL + '/MawbAllocation/GetAgents');
    };
    var GetMawbAllocation = function (action) {
        return $http.get(config.SERVICE_URL + '/TradelaneBooking/MAWBDetails',
            {
                params: {
                    action: action
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
    var GetAirlines = function () {
        return $http.get(config.SERVICE_URL + '/MawbAllocation/GetAirlines');
    };
    var SaveMAWBAllocation = function (MAList) {
        return $http.post(config.SERVICE_URL + '/MawbAllocation/SaveMawbAllocation', MAList);
    };
    var GetTimeZoneList = function () {
        return $http.get(config.SERVICE_URL + '/TimeZone/GetTimeZoneList');
    };
    
    var SaveMawbAllocation = function (mawbDetail) {
        return $http.post(config.SERVICE_URL + '/TradelaneBooking/SaveMAWBdetail', mawbDetail);
    };
    return {
        SaveMawbAllocation:SaveMawbAllocation,
        GetTradelaneAgentsGetTradelaneAgent: GetTradelaneAgents,
        GetMawbAllocation: GetMawbAllocation,
        GetShipmentHandlerId: GetShipmentHandlerId,
        GetAirlines: GetAirlines,
        SaveMAWBAllocation: SaveMAWBAllocation,
        GetTimeZoneList: GetTimeZoneList,
        MAWBInitials: MAWBInitials,
        CustomerAction: CustomerAction,
        AdminConfirmReject: AdminConfirmReject,
        GetLatestTermAndConditionPublic: GetLatestTermAndConditionPublic,
        DirectBookingConfirmReject: DirectBookingConfirmReject
    };

});