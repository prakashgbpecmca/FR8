angular.module('ngApp.public').
    controller('SystemAlertDescriptionController', function ($scope, SystemAlertService, HomeService, $stateParams) {
    //$scope.title = "SystemAlertDescription";


    var init = function () {
        
        var Heading = $stateParams.Heading;
        HomeService.GetCurrentOperationZone().then(function (response) {
            var OperationZoneId = response.data.OperationZoneId;
            if (OperationZoneId) {
                SystemAlertService.GetPublicSystemAlertDetail(OperationZoneId, Heading).then(function (response) {
                    $scope.result = response.data;
                });
            }
        });

        
    };

    init();
});