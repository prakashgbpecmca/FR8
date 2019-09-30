angular.module('ngApp.tradelanePreAlertDashBoardBoard').factory('tradelanePreAlertService', function ($http, config, SessionService) {

    var GetInitial = function () {
        return $http.get(config.SERVICE_URL + '/TradelaneStaffDashBoard/StaffDashBoardInitialDetail');
    };

    return {
        GetInitial: GetInitial
    };
});