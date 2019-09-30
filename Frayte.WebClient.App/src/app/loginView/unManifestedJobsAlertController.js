angular.module('ngApp.loginview').controller('UnManifestedJobsAlertController', function ($scope, $state, $uibModal, $rootScope, $uibModalInstance, config) {

    $scope.CreateManifestHome = function () {
      
        $state.go('loginView.userTabs.manifest.userManifest', {}, { reload: true });

        $rootScope.ChangeManifest = true;
        $uibModalInstance.dismiss('');
    };
    function init() {
        $scope.ImagePath = config.BUILD_URL;
    }
    init();
});