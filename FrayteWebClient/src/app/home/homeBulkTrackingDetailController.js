angular.module('ngApp.home').controller('HomeBulkTrackingDetailController', function ($scope, $location, $state, $stateParams, config, $filter, CountryService, CourierService, HomeService, SessionService, $uibModal, $log, toaster) {
    
    function init() {
        $scope.bulkTrackingDetail = HomeService.bulkTracking;
        $scope.collapsed = true;
    }

    init();

});