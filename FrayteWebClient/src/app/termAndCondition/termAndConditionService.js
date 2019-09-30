/**
 * Service
 */
angular.module('ngApp.termandcondition').factory('TermAndConditionService', function ($http, config, SessionService) {

    var GetTermAndCondition = function (i) {
        return $http.get(config.SERVICE_URL + '/TermAndCondition/GetTermAndCondition',
            {
                params: {
                    termConditionId: i
                }
            });
    };

    var GetAllTermsAndCondition = function (OperationZoneId) {
        return $http.get(config.SERVICE_URL + '/TermAndCondition/GetAllTermsAndCondition', {
            params: {
                OperationZoneId: OperationZoneId
            }

        });
    };

    var GetLatestTermAndCondition = function () {
        return $http.get(config.SERVICE_URL + '/TermAndCondition/GetlatestTermsAndCondition');
    };

    var SaveTermAndCondition = function (termAndConditionDetail) {
        return $http.post(config.SERVICE_URL + '/TermAndCondition/SaveTermAndCondition', termAndConditionDetail);
    };



    var GetOperationZoneList = function () {
        return $http.get(config.SERVICE_URL + '/OperationZone/OperationZone');
    };

    var GetCurrentOperationZone = function () {
        return $http.get(config.SERVICE_URL + '/OperationZone/GetCurrentOperationZone');
    };
    return {
        GetTermAndCondition: GetTermAndCondition,
        GetLatestTermAndCondition:GetLatestTermAndCondition,
        GetAllTermsAndCondition: GetAllTermsAndCondition,
        SaveTermAndCondition: SaveTermAndCondition,
        GetOperationZoneList: GetOperationZoneList,
        GetCurrentOperationZone :GetCurrentOperationZone
    };

});