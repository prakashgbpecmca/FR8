angular.module('ngApp.home').factory('HSCodeService', function ($http, config, SessionService) {
 
    var ShipmentWithoutHSCodes = function (obj) {
        return $http.post(config.SERVICE_URL + '/HSCode/GetNonHSCodeShipments', obj);
    };

    var GetHSCodes = function (searchTerm, countryId, serachType) {
        return $http.get(config.SERVICE_URL + '/HSCode/GetHSCodes',
            {
                params: {
                    serachTerm: searchTerm,
                    countryId: countryId,
                    searchType: serachType
                }
            });
    };

    var SetShipmentHSCode = function (eCommerceShipmentDetailid, HSCode) {
        return $http.get(config.SERVICE_URL + '/HSCode/SetShipmentHSCode',
          {
              params: {
                  eCommerceShipmentDetailid: eCommerceShipmentDetailid,
                  HSCode: HSCode
              }
          });
    };
    return {
        ShipmentWithoutHSCodes: ShipmentWithoutHSCodes,
        GetHSCodes: GetHSCodes,
        SetShipmentHSCode: SetShipmentHSCode
    };
});