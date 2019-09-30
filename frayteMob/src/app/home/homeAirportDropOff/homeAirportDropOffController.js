
angular.module('ngApp.home').controller('HomeAirportDropOffController', function ($scope, config, $uibModal) {
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
        templateUrl: 'home/homeAirportDropOff/homeAirportDropOffCancel.tpl.html',
        controller: 'HomeAirportDropOffCancelController',
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
        templateUrl: 'home/homeAirportDropOff/homeAirportDropOffConfirm.tpl.html',
        controller: 'HomeAirportDropOffConfirmController',
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