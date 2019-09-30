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
        return $http.get(config.SERVICE_URL + '/OperationZone/GetCurrentOperationZone');
    };

    //var GetfuelService = function (OperationZoneId, LogisticCompany, Year) {
    //    return $http.get(config.SERVICE_URL + '/FuelSurCharge/GetFuelSurCharge',
    //        {
    //            params: {
    //                OperationZoneId: OperationZoneId,
    //                LogisticCompany : LogisticCompany,
    //                Year: Year
    //            }
    //        });
    //};

    var GetfuelMonthYear = function (OperationZoneId) {
        return $http.get(config.SERVICE_URL + '/FuelSurCharge/GetFuelSurchargeMonthYear',
            {
                params: {
                    OperationZoneId: OperationZoneId
                }
            });
    };

    var GetfuelService = function (OperationZoneId, Year) {
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

    var GetLogisticCompanies = function (OperationZoneId) {
        return $http.get(config.SERVICE_URL + '/FuelSurCharge/GetDistinctLogisticCompany',
            {
                params: {
                    OperationZoneId: OperationZoneId
                }
            });
    };

    return {
        fuelService: fuelService,
        GetThreeFuelSurCharge: GetThreeFuelSurCharge,
        GetOperationZone: GetOperationZone,
        //GetfuelService: GetfuelService,
        UpdateFuelSurCharge: UpdateFuelSurCharge,
        CurrentOperationZone: CurrentOperationZone,
        GetDistinctFuelSurchargeYear: GetDistinctFuelSurchargeYear,
        GetLogisticCompanies: GetLogisticCompanies,
        GetfuelService: GetfuelService,
        GetfuelMonthYear: GetfuelMonthYear
    };
});