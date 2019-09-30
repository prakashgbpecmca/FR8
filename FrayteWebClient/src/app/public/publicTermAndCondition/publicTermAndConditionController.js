
angular.module('ngApp.public').controller('PublicTermAndConditionController', function ($scope, $anchorScroll, TermAndConditionService, $location, $sce, PublicService, $state, $stateParams, toaster, config) {
    var setScroll = function (printSectionId) {
        $location.hash(printSectionId);
        $anchorScroll();
    };

    var getLatestTermAndConditionPublic = function () {
        PublicService.GetLatestTermAndConditionPublic($scope.OperationZone.OperationZoneId, "Public").then(function (response) {
            $scope.TermAndCondition = $scope.ShipperNotification = $sce.trustAsHtml(response.data.Detail);
        }, function (reason) {
            toaster.pop({
                type: 'error',
                title: "Frayte-Error",
                body: "Error While Getting Record",
                showCloseButton: true
            });
        });
    };

    var getCurrentOperationZone = function () {
        TermAndConditionService.GetCurrentOperationZone().then(function (response) {
            $scope.OperationZone = response.data;
            if ($scope.OperationZone !== null) {
                getLatestTermAndConditionPublic();
            }
            else {
                toaster.pop({
                    type: 'error',
                    title: "Frayte-Error",
                    body: "Error While Getting Record",
                    showCloseButton: true
                });
            }

        }, function (reason) {
            toaster.pop({
                type: 'error',
                title: "Frayte-Error",
                body: "Error While Getting Record",
                showCloseButton: true
            });
        });
    };
    function init() {

        getCurrentOperationZone();

        setScroll('top');
        $anchorScroll.yOffset = 700;
    }

    init();

});