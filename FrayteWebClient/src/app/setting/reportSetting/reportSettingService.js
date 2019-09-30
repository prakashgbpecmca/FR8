angular.module('ngApp.reportSetting').factory('ReportSettingService', function ($http, config, SessionService) {

    var SaveReportSetting = function (frayteReportSetting) {
        return $http.post(config.SERVICE_URL + '/ReportSetting/SaveReportSetting', frayteReportSetting);
    };

    var GetReportSettingDetails = function () {
        return $http.get(config.SERVICE_URL + '/ReportSetting/GetReportSettingDetail');
    };
    return {
        SaveReportSetting: SaveReportSetting,
        GetReportSettingDetails: GetReportSettingDetails
    };

});