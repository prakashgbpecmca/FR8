/**
 * Service
 */
angular.module('ngApp.systemAlert').
    factory('SystemAlertService', function ($http, config, SessionService) {

        var getAllSystemAlerts = function (trackSystemAlert) {
            return $http.post(config.SERVICE_URL + '/SystemAlert/GetSystemAlerts', trackSystemAlert);
        };

        var getSystemAlertDetail = function (operationZoneId, systemAlertId) {
            return $http.get(config.SERVICE_URL + '/SystemAlert/GetSystemAlertDetail', {
                params: {
                    operationZoneId: operationZoneId,
                    systemAlertId: systemAlertId
                }
            });
        };
        var saveUpdateSystemAlerts = function (frayteSystemAlert) {
            return $http.post(config.SERVICE_URL + '/SystemAlert/SaveUpdateSystemAlerts', frayteSystemAlert);
        };

        var GetTimeZones= function () {
            return $http.get(config.SERVICE_URL + '/Shipment/GetInitials');
        };
        var GetPublicSystemAlert = function (OperationZoneId, currentDate) {
            return $http.get(config.SERVICE_URL + '/SystemAlert/GetPublicSystemAlerts', {
                params: {
                    OperationZoneId: OperationZoneId,
                    currentDate: currentDate
                }
            });
        };
        var DeleteSystemAlert = function (systemAlertId) {
            return $http.get(config.SERVICE_URL + '/SystemAlert/DeleteSystemAlert', {
                params: {
                    systemAlertId: systemAlertId
                }
            });
        };

        var GetPublicSystemAlertDetail = function (OperationZoneId, systemAlertHeading) {
            return $http.get(config.SERVICE_URL + '/SystemAlert/GetPublicSystemAlertDetail', {
                params: {
                    OperationZoneId: OperationZoneId,
                    systemAlertHeading: systemAlertHeading
                }
            });
        };

        var SystemAlertHeadingAvailability = function (OperationZoneId, systemAlertHeading) {
            return $http.get(config.SERVICE_URL + '/SystemAlert/SystemAlertHeadingAvailability', {
                params: {
                    OperationZoneId: OperationZoneId,
                    systemAlertHeading: systemAlertHeading
                }
            });
        };

        return {
            GetAllSystemAlerts: getAllSystemAlerts,
            GetSystemAlertDetail: getSystemAlertDetail,
            SaveUpdateSystemAlerts: saveUpdateSystemAlerts,
            GetTimeZones: GetTimeZones,
            GetPublicSystemAlert: GetPublicSystemAlert,
            GetPublicSystemAlertDetail: GetPublicSystemAlertDetail,
            SystemAlertHeadingAvailability: SystemAlertHeadingAvailability,
            DeleteSystemAlert: DeleteSystemAlert
        };

    });