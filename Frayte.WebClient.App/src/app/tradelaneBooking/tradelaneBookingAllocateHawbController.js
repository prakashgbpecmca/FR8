angular.module('ngApp.tradelaneBooking').controller('TradelaneBookingAllocateHawbController', function ($scope, $uibModalInstance) {

    $scope.airport = 'Airport';

    $scope.continueHAWB = function (value) {
        $uibModalInstance.close(value);
    };

    $scope.closeHAWB = function (value) {
        $uibModalInstance.close(value);
    };

    function init() { }

    init();

});