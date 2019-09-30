
angular.module('ngApp.home').controller('homeairlineULDEditController', function ($scope, $uibModal, $location, $log, $sce, $document, adminPopupIn, config) {
    $scope.airlineULD = {};

    init();
    function init() {
        $scope.airlineULD = adminPopupIn;
        $scope.ImgPath = config.BUILD_URL;
    }

});