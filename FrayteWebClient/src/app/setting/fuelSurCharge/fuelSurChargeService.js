angular.module('ngApp.fuelSurCharge').factory('FuelService', function ($http, config) {

    var fuelService = function (fuelDetail) {
        return $http.post(config.SERVICE_URL + '/FuelSurCharge/SaveFuelSurCharge', fuelDetail);
    };
    var GetOperationZone = function () {

        return $http.get(config.SERVICE_URL + '/OperationZone/OperationZone');
    };

    var UpdateFuelSurCharge = function (fuelSurCharge) {
        return $http.post(config.SERVICE_URL + '/FuelSurCharge/UpdateFuelSurCharge', fuelSurCharge);

    };

    var CurrentOperationZone = function () {
        debugger;
        return $http.get(config.SERVICE_URL + '/OperationZone/GetCurrentOperationZone');
    };

    var GetfuelService = function (OperationZoneId, Year) {
        debugger;
        return $http.get(config.SERVICE_URL + '/FuelSurCharge/GetFuelSurCharge',
            {
                params: {
                    OperationZoneId: OperationZoneId,
                    Year: Year
                }
            });
    };
    var GetThreeFuelSurCharge = function (OperationZoneId, Year) {
        return $http.get(config.SERVICE_URL + '/FuelSurCharge/GetThreeFuelSurCharge',
            {
                params: {
                    OperationZoneId: OperationZoneId,
                    Year: Year
                }
            });
    };

    var GetDistinctFuelSurchargeYear = function () {
        return $http.get(config.SERVICE_URL + '/FuelSurCharge/GetDistinctFuelSurchargeYear');
    };
    return {
        fuelService: fuelService,
        GetThreeFuelSurCharge:GetThreeFuelSurCharge,
        GetOperationZone: GetOperationZone,
        GetfuelService: GetfuelService,
        UpdateFuelSurCharge: UpdateFuelSurCharge,
        CurrentOperationZone: CurrentOperationZone,
        GetDistinctFuelSurchargeYear: GetDistinctFuelSurchargeYear
    };

});