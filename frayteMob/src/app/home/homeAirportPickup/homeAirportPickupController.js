
angular.module('ngApp.home').controller('HomeAirportPickupController', function ($scope, config, $uibModal) {
$scope.scanExportManifest = function () {
    $scope.isActive = true;
};

$scope.scanBag = function () {
    $scope.isActive = false;
};

//cancel code here
$scope.cancelBag = function () {
    var ModalInstance = $uibModal.open({
        Animation: true,
        templateUrl: 'home/homeAirportPickup/homeAirportPickupCancel.tpl.html',
        controller: 'HomeAirportPickupCancelController',
        keyboard: true,
        windowClass: '',
        size: 'lg'
    });
};
//end

//confirm dispatch code here
$scope.confirmDispatch = function () {
    var ModalInstance = $uibModal.open({
        Animation: true,
        templateUrl: 'home/homeAirportPickup/homeAirportPickupConfirm.tpl.html',
        controller: 'HomeAirportPickupConfirmController',
        keyboard: true,
        windowClass: '',
        size: 'lg'
    });
};
//end


function init() {
    $scope.scanExportManifest();
}
init();

});