/**
 * Service
 */
angular.module('ngApp.directBookingUploadShipment').factory('DbUploadShipmentService', function ($http, config, SessionService) {

    var GetUnSuccessfulShipments = function (CustomerId) {
        return $http.get(config.SERVICE_URL + '/DirectBookingUploadShipment/GetUnSuccessfulShipments',
            {
                params: {
                    CustomerId: CustomerId
                }
            });
    };

    var SaveServiceCode = function (ServiceCode, DirectShipmentDraftId) {
        return $http.get(config.SERVICE_URL + '/DirectBookingUploadShipment/SaveServiceCode',
            {
                params: {
                    ServiceCode: ServiceCode,
                    DirectShipmentDraftId: DirectShipmentDraftId
                }
            });
    };
 

    var GetShipmentsFromDraft = function (CustomerId) {
        return $http.get(config.SERVICE_URL + '/DirectBookingUploadShipment/GetShipmentsFromDraft',
            {
                params: {
                    CustomerId: CustomerId
                }
            });
    };

    var GetErrorDetail = function (ShipmentId, ServiceType) {
        return $http.get(config.SERVICE_URL + '/DirectBookingUploadShipment/GetShipmentErrors',
            {
                params: {
                    ShipmentId: ShipmentId,
                    ServiceType: ServiceType
                }
            });
    };

    var GetFrayteError = function (DirectShipmentDraftId) {
        return $http.get(config.SERVICE_URL + '/DirectBookingUploadShipment/GetFrayteError',
            {
                params: {
                    DirectShipmentDraftId: DirectShipmentDraftId
                }
            });
    };

    var GetServiceCode = function () {
        return $http.get(config.SERVICE_URL + '/DirectBookingUploadShipment/GetServiceCode');
    };

    var GenerateUnsucessfulShipmentsWithServcie = function (Shipments) {
        return $http.post(config.SERVICE_URL + '/DirectBookingUploadShipment/GenerateUnsucessfulShipmentsWithServcie', Shipments);
    };
    var GenerateUnsuccessfulShipmentWithoutService = function (ShipmentsWithoutService) {
        return $http.post(config.SERVICE_URL + '/DirectBookingUploadShipment/GenerateUnsucessfulShipmentsWithoutService', ShipmentsWithoutService);
    };
 
    var SaveShipmentWithService = function (UploadShipment) {
        return $http.post(config.SERVICE_URL + '/DirectBookingUploadShipment/SelectService', UploadShipment);
    };

    var RemoveShipmentWithService = function (UploadShipment) {
        return $http.post(config.SERVICE_URL + '/DirectBookingUploadShipment/RemoveShipmentWithService', UploadShipment);
    };

    var SaveeCommerceUploadBooking = function (Shipment) {
        return $http.post(config.SERVICE_URL + '/DirectBookingUploadShipment/SaveEcommUploadShipment', Shipment);
    };

    var SaveeCommerceWithServiceUploadBooking = function (Shipment) {
        return $http.post(config.SERVICE_URL + '/DirectBookingUploadShipment/SaveShipmentDraftForm', Shipment);
    };

    var GetUpdatedBatchProcess = function (CustomerId) {
        return $http.get(config.SERVICE_URL + '/DirectBookingUploadShipment/GetUpdatedBatchProcess',
           {
               params: {
                   CustomerId: CustomerId
               }
           });
    };
    var SaveShipmentFromErrorPopup = function (UploadShipment) {
        return $http.post(config.SERVICE_URL + '/DirectBookingUploadShipment/FrayteUploadShipments', UploadShipment);
    };

    var SaveShipmentFromErrorPopupSelectService = function (UploadShipment) {
        return $http.post(config.SERVICE_URL + '/DirectBookingUploadShipment/SaveShipmentDraft', UploadShipment);
    }; 

    var GetLogisticServiceCode = function (OperationZoneId, CustomerId) {
        return $http.get(config.SERVICE_URL + '/DirectBookingUploadShipment/GetLogisticServiceCode',
           {
               params: {
                   OperationZoneId : OperationZoneId,
                   CustomerId: CustomerId
               }
           });
    };
    var GetSessionNameList = function (UserId) {
        return $http.get(config.SERVICE_URL + '/DirectBookingUploadShipment/GetSessionNameList', {
            params: {
                UserId: UserId
            }
        });
    };
    var GetSessionList = function (UserId) {
        return $http.get(config.SERVICE_URL + '/DirectBookingUploadShipment/GetSessionList', {
            params: {
                UserId: UserId
            }
        });
    };
    var GetShipmentList = function (SessionId) {
        return $http.get(config.SERVICE_URL + '/DirectBookingUploadShipment/GetShipmentDraftList', {
            params: {
                SessionId: SessionId
            }
        });
    };
    var GetShipments = function (SessionId) {
        return $http.get(config.SERVICE_URL + '/DirectBookingUploadShipment/GetShipmentList', {
            params: {
                SessionId: SessionId
            }
        });
    };
    var DeleteShipment = function (ShipementId) {
        return $http.get(config.SERVICE_URL + '/DirectBookingUploadShipment/DeleteShipment', {
            params: {
                DirectShipmentDraftId: ShipementId
            }
        });
    };
    var SaveDirectBooking = function (directBookingDetail) {
        return $http.post(config.SERVICE_URL + '/DirectBookingUploadShipment/SaveShipmentDraftForm', directBookingDetail);
    };
    var SaveIsSessionPrint = function (SessionId) {
        return $http.get(config.SERVICE_URL + '/DirectBookingUploadShipment/SaveIsSessionPrint', {
            params: {
                SessionId: SessionId
            }
        });
    };
    return {
        GetUnSuccessfulShipments: GetUnSuccessfulShipments,
        GetErrorDetail: GetErrorDetail,
        GetShipmentsFromDraft: GetShipmentsFromDraft,
        GenerateUnsuccessfulShipmentWithoutService: GenerateUnsuccessfulShipmentWithoutService,
        GetServiceCode: GetServiceCode,
        SaveShipmentWithService: SaveShipmentWithService,
        SaveeCommerceUploadBooking: SaveeCommerceUploadBooking,
        GetUpdatedBatchProcess: GetUpdatedBatchProcess,
        RemoveShipmentWithService: RemoveShipmentWithService,
        GenerateUnsucessfulShipmentsWithServcie: GenerateUnsucessfulShipmentsWithServcie,
        SaveShipmentFromErrorPopup: SaveShipmentFromErrorPopup,
        SaveShipmentFromErrorPopupSelectService: SaveShipmentFromErrorPopupSelectService,
        SaveeCommerceWithServiceUploadBooking: SaveeCommerceWithServiceUploadBooking,
        GetLogisticServiceCode: GetLogisticServiceCode,
        GetSessionNameList: GetSessionNameList,
        GetSessionList: GetSessionList,
        GetShipmentList: GetShipmentList,
        DeleteShipment: DeleteShipment,
        SaveDirectBooking: SaveDirectBooking,
        GetShipments: GetShipments,
        SaveServiceCode: SaveServiceCode,
        GetFrayteError: GetFrayteError,
        SaveIsSessionPrint: SaveIsSessionPrint
    };
});