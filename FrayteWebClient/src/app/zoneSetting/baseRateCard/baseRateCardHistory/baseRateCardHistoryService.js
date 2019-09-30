angular.module('ngApp.baseRateCard').factory('ZoneBaseRateCardHistoryService', function ($http, config, SessionService) {

    var GetYear = function () {
        return $http.get(config.SERVICE_URL + '/ViewBaseRateCard/DistinctYear');
    };

    var GetBaseRateCard = function (Year) {
        return $http.get(config.SERVICE_URL + '/ViewBaseRateCard/GetLogisticServiceItems', {
            params: {
                Yaer: Year
            }
        });
    };

    var GenerateReport = function (LogisticServiceId, Year) {
        return $http.get(config.SERVICE_URL + '/ViewBaseRateCard/GenerateReport', {
            params: {
                LogisticServiceId: LogisticServiceId,
                Yaer: Year
            }
        });
    };
   
    return {
        GetYear: GetYear,
        GetBaseRateCard: GetBaseRateCard,
        GenerateReport: GenerateReport
    };
});