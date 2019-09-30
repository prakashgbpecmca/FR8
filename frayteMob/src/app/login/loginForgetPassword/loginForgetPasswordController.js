angular.module('ngApp.login').controller('loginForgetPasswordController', function ($scope, config) {
    $scope.forgetPassword = 'Forget Password';

    function init() {
        $scope.ImagePath = config.BUILD_URL;
    }
    init();

});