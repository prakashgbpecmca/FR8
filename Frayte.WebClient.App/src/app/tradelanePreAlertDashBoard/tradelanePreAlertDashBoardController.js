angular.module('ngApp.tradelanePreAlertDashBoardBoard').controller('TradelanePreAlertDashBoardController', function ($scope, $rootScope, config, $state, $location, $translate, $stateParams, SessionService, $uibModal, uiGridConstants, toaster, ModalService, TradelaneService, CarrierService, CountryService, ShipmentService, UserService, $anchorScroll, TradelaneMilestoneService, TradelaneStaffBoardService) {

    //$scope.collapse_A = function () {
    //    $scope.value = !$scope.value;
    //};
    //$scope.collapse_B = function () {
    //    $scope.valued = !$scope.valued;
    //};
    //$scope.collapse_C = function () {
    //    $scope.valued1 = !$scope.valued1;
    //};
    //$scope.collapse_D = function () {
    //    $scope.valued2 = !$scope.valued2;
    //};

    $scope.tradelanePopup = function (type) {
        $scope.instance = $uibModal.open({
            templateUrl: 'tradelanePreAlertDashBoard/tradelanePreAlertPopup.tpl.html',
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            backdrop: 'static'
        });
    };

    $scope.PreAlertStateChange = function () {
        $rootScope.ShipmentShowType = 'PreAlert';
        $state.go('loginView.userTabs.tradelane-shipments');
    };

    $scope.UnallocatedShipmentChange = function () {
        $rootScope.ShipmentShowType = 'UnallocatedShipment';
        $state.go('loginView.userTabs.tradelane-shipments');
    };

    $scope.MileStoneStateChange = function () {
        $rootScope.ShipmentShowType = 'MilestoneNotCompleted';
        $state.go('loginView.userTabs.tradelane-shipments');
    };

    function init() {
        $rootScope.ShipmentShowType = null;
        var userInfo = SessionService.getUser();
        
        TradelaneStaffBoardService.GetInitial().then(function (response) {
            $scope.Data = response.data;
            if ($scope.Data) {
                $scope.PreAlertCount = $scope.Data.PreAlertCount;
                $scope.UnallocatedShipmentCount = $scope.Data.UnallocatedShipmentCount;
                $scope.MilestonenotCompletedCount = $scope.Data.MilestoneNotCompletedCount;
            }
        }, function () {


        });
    }

    init();
});