
angular.module('ngApp.login').controller('LoginController', function ($scope, $state, config) {


    $scope.onSubmit = function () {
        if ($scope.username == 'manish' && $scope.password == '12345') {
            $state.go('home.bag');

        }else{
            console.log('Sorry you have enter wrong information.');
        }
};

    function init() {
        $scope.ImagePath = config.BUILD_URL;
    }
    init();
    
});