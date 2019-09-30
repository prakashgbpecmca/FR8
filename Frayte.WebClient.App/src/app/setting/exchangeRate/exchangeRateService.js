angular.module('ngApp.exchangeRate').factory('ExchangeRateService', function ($http, config, SessionService) {

    var GetOperationZone = function () {
        return $http.get(config.SERVICE_URL + '/ExchangeRate/GetOperationZone');
    };
    var GetCurrencyDetail = function () {
        return $http.get(config.SERVICE_URL + '/ExchangeRate/GetCurrencyDetail');
    };

    var GetOperationExchangeRate = function (operationZoneId) {
        return $http.get(config.SERVICE_URL + '/ExchangeRate/GetOperationExchangeRate',
            {
                params: {
                    operationZoneId: operationZoneId
                }
            });
    };
    var GetExchangeYears = function (OperationZoneId, Type) {
        return $http.get(config.SERVICE_URL + '/ExchangeRate/DistinctYear',
          {
              params: {
                  OperationZoneId: OperationZoneId,
                  Type: Type
              }
          });
    };
    var GetExchangeMonth = function (OperationZoneId, Type) {
        return $http.get(config.SERVICE_URL + '/ExchangeRate/DistinctMonth',
          {
              params: {
                  OperationZoneId: OperationZoneId,
                  Type: Type
              }
          });
    };
    var SaveExchangeRate = function (exchangeRate) {
        return $http.post(config.SERVICE_URL + '/ExchangeRate/SaveExchangeRate', exchangeRate);
    };
    var GetOperationExchangeRateHistory = function (search) {
        return $http.post(config.SERVICE_URL + '/ExchangeRate/ExchangeRateHistory',search);
    };
    return {
        GetOperationZone: GetOperationZone,
        GetCurrencyDetail: GetCurrencyDetail,
        SaveExchangeRate: SaveExchangeRate,
        GetOperationExchangeRate: GetOperationExchangeRate,
        GetExchangeYears: GetExchangeYears,
        GetExchangeMonth: GetExchangeMonth,
        GetOperationExchangeRateHistory: GetOperationExchangeRateHistory
    };

});