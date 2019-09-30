angular.module('ngApp.preAlert').factory('PreAlertService', function ($http, config, SessionService) {

    var PreALertInitials = function (shipmentId) {
        return $http.get(config.SERVICE_URL + '/TradelaneShipments/PreALertInitials',
            {
                params: {
                    shipmentId: shipmentId
                }
            });
    };

    var getTradelaneDocuments = function (shipmentId) {
        return $http.get(config.SERVICE_URL + '/TradelaneShipments/GetShipmentOtherDocuments',
          {
              params: {
                  shipmentId: shipmentId
              }
          });
    };

    var SendPreAlertEmail = function (preAlerDetail) {
        return $http.post(config.SERVICE_URL + "/TradelaneShipments/SendPreAlertEmail", preAlerDetail);
    };

    var removeOtherDoc = function (tradelaneShipmentDocument) {
        return $http.get(config.SERVICE_URL + '/TradelaneShipments/RemoveOtherDocument',
     {
         params: {
             tradelaneShipmentDocument: tradelaneShipmentDocument
         }
     });
    };
    return {
        removeOtherDoc: removeOtherDoc,
        SendPreAlertEmail: SendPreAlertEmail,
        PreALertInitials: PreALertInitials,
        getTradelaneDocuments: getTradelaneDocuments
    };
});