
angular.module('ngApp.public').controller('PublicTermAndConditionController', function ($scope, $anchorScroll, TermAndConditionService, $location, $sce, PublicService, $state, $stateParams, toaster, config, $translate) {

    //Set Multilingual for Modal Popup
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'Error_While_Getting_Record']).then(function (translations) {

            $scope.FrayteError = translations.FrayteError;
            $scope.Error_While_Getting_Record = translations.Error_While_Getting_Record;

        });
    };

    var setScroll = function (printSectionId) {
        $location.hash(printSectionId);
        $anchorScroll();
    };

    var getLatestTermAndConditionPublic = function () {
        PublicService.GetLatestTermAndConditionPublic($scope.OperationZone.OperationZoneId, "Public", $scope.CompanyCode).then(function (response) {
            $scope.TermAndCondition = $scope.ShipperNotification = $sce.trustAsHtml(response.data.Detail);
        }, function (reason) {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.Error_While_Getting_Record,
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
                    title: $scope.FrayteError,
                    body: $scope.Error_While_Getting_Record,
                    showCloseButton: true
                });
            }

        }, function (reason) {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.Error_While_Getting_Record,
                showCloseButton: true
            });
        });
    };
    function init() {

        $scope.CompanyCode = config.SITE_COMPANY === 'MAXLOGISTIC' ? 'MEX' : '';

        getCurrentOperationZone();
        setMultilingualOptions();
        setScroll('top');
        $anchorScroll.yOffset = 700;
    }

    init();

});