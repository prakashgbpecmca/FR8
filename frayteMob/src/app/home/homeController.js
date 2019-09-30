/** 
 * Controller
 */
angular.module('ngApp.home').controller('HomeController', function ($scope, config) {

    $scope.CloseSidePanel = function () {
        document.getElementById("mySidenav").style.width = "0";
    };

    $scope.openNav = function () {
        document.getElementById("mySidenav").style.width = "250px";
        document.body.style.backgroundColor = "rgba(0,0,0,0.4)";
        document.getElementById("mySidenav").style.width = "0";
    };

    function init() {
        $scope.ImagePath = config.BUILD_URL;
        
    }
    init();

});