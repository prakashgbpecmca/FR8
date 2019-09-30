angular.module('ngApp.public').factory('PublicService', function ($http, config, SessionService) {

    var AdminConfirmReject = function (actionDetail) {

        return $http.post(config.SERVICE_URL + '/Customer/CustomerAdminAction', actionDetail);
    };

    var GetLatestTermAndConditionPublic = function (OperationZoneId, TermAndCondtionType, shortCode) {
        return $http.get(config.SERVICE_URL + '/TermAndCondition/GetlatestTermsAndCondition', {
            params: {
                OperationZoneId: OperationZoneId,
                TermAndCondtionType: TermAndCondtionType,
                shortCode: shortCode
            }
        });
    };

    return {
        AdminConfirmReject: AdminConfirmReject,
        GetLatestTermAndConditionPublic: GetLatestTermAndConditionPublic
    };

});