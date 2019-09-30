angular.module('ngApp.tracking').controller("PublicTrackingConfiguration", function ($scope, $uibModal, ModalService, config, SessionService) {


    function init() {

        $scope.ImagePath = config.BUILD_URL;

        var moduleType = SessionService.getModuleType();
        if (moduleType) {
            $scope.moduleType = moduleType;
        }
        else {
            $scope.moduleType = "Tradelane";
        }
    }
    init();

});
