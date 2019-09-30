/**
 * Service
 */
angular.module('ngApp.shipment').factory('ShipmentService', function ($http, config, SessionService) {

    var GetWarehouseList = function () {
        return $http.get(config.SERVICE_URL + '/Warehouse/GetWarehouseList');
    };

    var GetTransportToWarehouse = function () {
        return $http.get(config.SERVICE_URL + '/Warehouse/GetTransportToWarehouse');
    };

    var GetInitials = function () {
        return $http.get(config.SERVICE_URL + '/Shipment/GetInitials');
    };

    var GetShipmentDetail = function (shipmentId) {
        return $http.get(config.SERVICE_URL + '/Shipment/GetShipmentDetail',
            {
                params: {
                    shipmentId: shipmentId
                }
            });
    };
    var GetCountryPort = function (countryId) {
        return $http.get(config.SERVICE_URL + '/Shipment/GetCountryPort',
            {
                params: {
                    countryId: countryId
                }
            });
    };
    var GetCountryByTimezone = function (timezoneId) {
        return $http.get(config.SERVICE_URL + '/Country/GetCountryByTimezone',
            {
                params: {
                    timezoneId: timezoneId
                }
            });
    };
    var GetShipmentShipperReceiverDetail = function (shipmentId) {
        return $http.get(config.SERVICE_URL + '/Shipment/GetShipmentShipperReceiverDetail',
           {
               params: {
                   shipmentId: shipmentId
               }
           });
    };
    var GetDestinatingAgent = function (shipmentId) {
        return $http.get(config.SERVICE_URL + '/Agent/GetDestinatingAgent',
          {
              params: {
                  shipmentId: shipmentId
              }
          });
    };
    var GetETAETDDetail = function (shipmentId) {
        return $http.get(config.SERVICE_URL + '/Shipment/GetETAETDDetail',
          {
              params: {
                  shipmentId: shipmentId
              }
          });
    };
    var GetExprysShipment = function () {
        return $http.get(config.SERVICE_URL + '/ShipmentExprys/GetExprysShipment');
    };
    var OperationStaffConfirmation = function (shipmentId) {
        return $http.post(config.SERVICE_URL + '/Shipment/OperationStaffConfirmation'+'?shipmentId='+shipmentId
        );

    };
    var GetOriginatingAgent = function (shipmentId) {
        return $http.get(config.SERVICE_URL + '/Agent/GetOriginatingAgent',
         {
             params: {
                 shipmentId: shipmentId
             }
         });
    };

    var UpdatePOlPODDetail = function (telexShipment) {
        return $http.post(config.SERVICE_URL + '/Shipment/UpdatePOlPODDetail', telexShipment);
    };

    var getSeaInitial = function (shipmentId) {
        return $http.get(config.SERVICE_URL + '/Shipment/GetSeaInitial',
        {
            params: {
                shipmentId: shipmentId
            }
        });
    };
    var getPrefixViaTradelane = function (shipmentId) {
        return $http.get(config.SERVICE_URL + '/Shipment/getPrefixViaTradelane',
       {
           params: {
               shipmentId: shipmentId
           }
       });
    };
    var SaveShipment = function (shipment) {
        return $http.post(config.SERVICE_URL + '/Shipment/SaveShipment', shipment);
    };
    var SaveAmmendShipment = function (frayteAmendShipment) {
        return $http.post(config.SERVICE_URL + '/Shipment/SaveAmmendShipment', frayteAmendShipment);
    };
    var SetShipmentBarCodeSO = function (shipmentBarCde) {
        return $http.post(config.SERVICE_URL + '/Shipment/SetShipmentBarCodeSO', shipmentBarCde);
    };
    var CustomerAction = function (confirmationDetail) {
        return $http.post(config.SERVICE_URL + '/Shipment/CustomerAction', confirmationDetail);
    };

    var UpdateDropOffDetail = function (dropOffDetail) {
        return $http.post(config.SERVICE_URL + '/Shipment/UpdateDropOffDetail', dropOffDetail);
    };

    var UpdateOriginatingAgentDetail = function (publicAgentConfirm) {
        return $http.post(config.SERVICE_URL + '/Shipment/UpdateOriginatingAgentDetail', publicAgentConfirm);
    };

    var OriginatingAgentReject = function (publicAgentReject) {
        return $http.post(config.SERVICE_URL + '/Shipment/OriginatingAgentReject', publicAgentReject);
    };

    var UpdateDestinatingAgentAnticipatedDetail = function (anticipatedDateTime) {
        return $http.post(config.SERVICE_URL + '/Shipment/UpdateDestinatingAgentAnticipatedDetail', anticipatedDateTime);
    };
    var UpdateShipperAnticipatedDetail = function (anticipatedDateTime) {
        return $http.post(config.SERVICE_URL + '/Shipment/UpdateShipperAnticipatedDetail', anticipatedDateTime);
    };

    var UpdateFlightSeaDetail = function (agentUpdateSeaDetails) {
        return $http.post(config.SERVICE_URL + '/Shipment/UpdateFlightSeaDetail', agentUpdateSeaDetails);
    };

    var GetReselectAgentShipmentDetail = function (shipmentId) {
        return $http.get(config.SERVICE_URL + '/shipment/GetReselectAgentShipmentDetail',
            {
                params: {
                    shipmentId: shipmentId
                }
            });
    };

    var ReselectAgent = function (agentReselectDetail) {
        return $http.post(config.SERVICE_URL + '/Shipment/ReselectAgent', agentReselectDetail);
    };

    var GetCurrentShipment = function (roleId) {
        return $http.get(config.SERVICE_URL + '/Shipment/GetCurrentShipment',
             {
                 params: {
                     roleId: roleId
                 }
             });
    };

    var GetPastShipment = function (roleId) {
        return $http.get(config.SERVICE_URL + '/Shipment/GetPastShipment',
             {
                 params: {
                     roleId: roleId
                 }
             });
    };

    var GetShipmentDocuments = function (shipmentId) {
        return $http.get(config.SERVICE_URL + '/Shipment/GetShipmentDocuments',
            {
                params: {
                    shipmentId: shipmentId
                }
            });
    };
    return {
        GetWarehouseList: GetWarehouseList,
        GetTransportToWarehouse: GetTransportToWarehouse,
        GetInitials: GetInitials,
        GetShipmentDetail: GetShipmentDetail,
        GetShipmentShipperReceiverDetail: GetShipmentShipperReceiverDetail,
        SaveShipment: SaveShipment,
        CustomerAction: CustomerAction,
        UpdateDropOffDetail: UpdateDropOffDetail,
        UpdateOriginatingAgentDetail: UpdateOriginatingAgentDetail,
        OriginatingAgentReject: OriginatingAgentReject,
        UpdateDestinatingAgentAnticipatedDetail: UpdateDestinatingAgentAnticipatedDetail,
        UpdateShipperAnticipatedDetail:UpdateShipperAnticipatedDetail,
        UpdateFlightSeaDetail: UpdateFlightSeaDetail,
        GetReselectAgentShipmentDetail: GetReselectAgentShipmentDetail,
        ReselectAgent: ReselectAgent,
        GetCurrentShipment: GetCurrentShipment,
        GetPastShipment: GetPastShipment,
        GetShipmentDocuments: GetShipmentDocuments,
        GetOriginatingAgent: GetOriginatingAgent,
        GetDestinatingAgent: GetDestinatingAgent,
        getPrefixViaTradelane: getPrefixViaTradelane,
        OperationStaffConfirmation: OperationStaffConfirmation,
        getSeaInitial: getSeaInitial,
        UpdatePOlPODDetail: UpdatePOlPODDetail,
        GetCountryByTimezone: GetCountryByTimezone,
        SetShipmentBarCodeSO: SetShipmentBarCodeSO,
        GetExprysShipment: GetExprysShipment,
        GetCountryPort: GetCountryPort,
        GetETAETDDetail: GetETAETDDetail,
        SaveAmmendShipment: SaveAmmendShipment

    };

});