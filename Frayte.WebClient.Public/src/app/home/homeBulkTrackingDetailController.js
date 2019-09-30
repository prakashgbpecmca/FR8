angular.module('ngApp.home').controller('HomeBulkTrackingDetailController', function ($scope, $location, $state, $stateParams, config, $filter,HomeService, SessionService, $log, toaster) {
    
    function init() {
        $scope.bulkTrackingDetail = HomeService.bulkTracking;
        $scope.collapsed = true;
    }

    init();

});