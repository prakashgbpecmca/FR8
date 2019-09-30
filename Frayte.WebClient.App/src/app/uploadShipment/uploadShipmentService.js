/**
 * Service
 */
angular.module('ngApp.uploadShipment').factory('UploadShipmentService', function ($http, config, SessionService) {

    var GetUnSuccessfulShipments = function (CustomerId) {
        return $http.get(config.SERVICE_URL + '/eCommerceUploadShipment/GetUnSuccessfulShipments',
            {
                params: {
                    CustomerId: CustomerId
                }
            });
    };

    var GetShipmentsFromDraft = function (CustomerId) {
        return $http.get(config.SERVICE_URL + '/eCommerceUploadShipment/GetShipmentsFromDraft',
            {
                params: {
                    CustomerId: CustomerId
                }
            });
    };

    var GetErrorDetail = function (ShipmentId, ServiceType) {
        return $http.get(config.SERVICE_URL + '/eCommerceUploadShipment/GetShipmentErrors',
            {
                params: {
                    ShipmentId: ShipmentId,
                    ServiceType: ServiceType
                }
            });
    }; 

    var GetServices = function (ShipmentId, ServiceType) {
        return $http.get(config.SERVICE_URL + '/eCommerceUploadShipment/GetServices');
    };

  

    var GenerateUnsucessfulShipmentsWithServcie = function (Shipments) {
        return $http.post(config.SERVICE_URL + '/eCommerceUploadShipment/GenerateUnsucessfulShipmentsWithServcie', Shipments);
    };
    var GenerateUnsuccessfulShipmentWithoutService = function (ShipmentsWithoutService) {
        return $http.post(config.SERVICE_URL + '/eCommerceUploadShipment/GenerateUnsucessfulShipmentsWithoutService', ShipmentsWithoutService); 
    };
    //var GenerateUnsuccessfulShipmentWithService = function (CustomerId) {
    //    return $http.get(config.SERVICE_URL + '/eCommerceUploadShipment/GenerateUnsucessfulShipmentsWithService',
    //       {
    //           params: {
    //               CustomerId: CustomerId
    //           }
    //       });
    //};
    var SaveShipmentWithService = function (UploadShipment) {
        return $http.post(config.SERVICE_URL + '/eCommerceUploadShipment/SelectService',UploadShipment);
    };

    var RemoveShipmentWithService = function (UploadShipment) {
        return $http.post(config.SERVICE_URL + '/eCommerceUploadShipment/RemoveShipmentWithService', UploadShipment);
    };

    var SaveeCommerceUploadBooking = function (Shipment) {
        return $http.post(config.SERVICE_URL + '/eCommerceUploadShipment/SaveEcommUploadShipment', Shipment);
    };

    var SaveeCommerceWithServiceUploadBooking = function (Shipment) {
        return $http.post(config.SERVICE_URL + '/eCommerceUploadShipment/SaveShipmentDraftForm', Shipment);
    };

    var GetUpdatedBatchProcess = function (CustomerId) {
        return $http.get(config.SERVICE_URL + '/eCommerceUploadShipment/GetUpdatedBatchProcess',
           {
               params: {
                   CustomerId: CustomerId
               }
           });
    };
    var SaveShipmentFromErrorPopup = function (UploadShipment) {
        return $http.post(config.SERVICE_URL + '/eCommerceUploadShipment/FrayteUploadShipments', UploadShipment);
    };

    var SaveShipmentFromErrorPopupSelectService = function (UploadShipment) {
        return $http.post(config.SERVICE_URL + '/eCommerceUploadShipment/SaveShipmentDraft', UploadShipment);
    };
    return {
        GetUnSuccessfulShipments: GetUnSuccessfulShipments,
        GetErrorDetail: GetErrorDetail,
        GetShipmentsFromDraft: GetShipmentsFromDraft,
        GenerateUnsuccessfulShipmentWithoutService: GenerateUnsuccessfulShipmentWithoutService,
        GetServices: GetServices,
        SaveShipmentWithService: SaveShipmentWithService,
        SaveeCommerceUploadBooking: SaveeCommerceUploadBooking,
        GetUpdatedBatchProcess: GetUpdatedBatchProcess,
        RemoveShipmentWithService: RemoveShipmentWithService,
        GenerateUnsucessfulShipmentsWithServcie: GenerateUnsucessfulShipmentsWithServcie,
        SaveShipmentFromErrorPopup: SaveShipmentFromErrorPopup,
        SaveShipmentFromErrorPopupSelectService: SaveShipmentFromErrorPopupSelectService,
        SaveeCommerceWithServiceUploadBooking: SaveeCommerceWithServiceUploadBooking
        
       
    };
});