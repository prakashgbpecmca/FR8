angular.module('ngApp.dashBoard').controller('DashBoardController', function ($scope, DashBoardService) {


    function init() {
        DashBoardService.GetDashBoardInitialData().then(function (response) {
            $scope.IntialDashBoardData = response.data;

        }, function () {


        });
    }
    init();
});