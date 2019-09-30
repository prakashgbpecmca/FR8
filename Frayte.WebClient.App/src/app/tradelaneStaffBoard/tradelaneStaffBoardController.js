angular.module('ngApp.tradelaneStaffBoard').controller('TradelaneStaffBoardController', function ($scope, $rootScope, config, $state, $location, $translate, $stateParams, SessionService, $uibModal, uiGridConstants, toaster, ModalService, TradelaneService, CarrierService, CountryService, ShipmentService, UserService, $anchorScroll, TradelaneMilestoneService, TradelaneStaffBoardService) {

    $scope.PreAlertStateChange = function () {
        $rootScope.ShipmentShowType = 'PreAlert';
        $state.go('loginView.userTabs.tradelane-shipments');
    };

    $scope.UnallocatedShipmentChange = function () {
        $rootScope.ShipmentShowType = 'UnallocatedShipment';
        $state.go('loginView.userTabs.tradelane-shipments');
    };

    function init() {
        $rootScope.ShipmentShowType = null;
        var userInfo = SessionService.getUser();
        $scope.ImagePath = config.BUILD_URL;
        
        TradelaneStaffBoardService.GetInitial().then(function (response) {
            $scope.Data = response.data;
            if ($scope.Data) {
                $scope.PreAlertCount = $scope.Data.PreAlertCount;
                $scope.UnallocatedShipmentCount = $scope.Data.UnallocatedShipmentCount;
            }
        }, function () {


        });
    }

    init();
});