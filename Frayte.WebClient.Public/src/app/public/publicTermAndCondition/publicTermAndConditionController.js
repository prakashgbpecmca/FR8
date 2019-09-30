
angular.module('ngApp.public').controller('PublicTermAndConditionController', function ($scope, $anchorScroll, HomeService, $location, $sce, PublicService, $state, $stateParams, toaster, config, $translate) {

    //multilingual key code
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'ErrorGettingRecord']).then(function (translations) {
            $scope.FrayteError = translations.FrayteError;
            $scope.ErrorGettingRecord = translations.ErrorGettingRecord;
        });
    };

    var setScroll = function (printSectionId) {
        $location.hash(printSectionId);
        $anchorScroll();
    };


    var getLatestTermAndConditionPublic = function () {
        PublicService.GetLatestTermAndConditionPublic($scope.OperationZone.OperationZoneId, "Public" ,"").then(function (response) {
            $scope.TermAndCondition = $scope.ShipperNotification = $sce.trustAsHtml(response.data.Detail);
        }, function (reason) {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ErrorGettingRecord,
                showCloseButton: true
            });
        });
    };

    var getCurrentOperationZone = function () {
        HomeService.GetCurrentOperationZone().then(function (response) {
            $scope.OperationZone = response.data;
            if ($scope.OperationZone !== null) {
                getLatestTermAndConditionPublic();
            }
            else {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.ErrorGettingRecord,
                    showCloseButton: true
                });
            }

        }, function (reason) {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ErrorGettingRecord,
                showCloseButton: true
            });
        });
    };
    function init() {

        getCurrentOperationZone();

        setScroll('top');
        $anchorScroll.yOffset = 700;
        setMultilingualOptions();
    }

    init();

});