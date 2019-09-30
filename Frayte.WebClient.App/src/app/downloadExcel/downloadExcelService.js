/**
 * Service
 */
angular.module('ngApp.downloadExcel').factory('DownloadExcelService', function ($http, config, SessionService) {

    var GetPieceDetailsExcelPath = function () {
        return $http.get(config.SERVICE_URL + '/Setting/GetPieceDetailsExcelPath');
    };

    return {
        GetPieceDetailsExcelPath: GetPieceDetailsExcelPath
    };

});