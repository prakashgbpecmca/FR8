/** 
 * Controller
 */
angular.module('ngApp.home').controller('HomeErrorController', function ($scope,UtilityService, $rootScope,SessionService, $state) {

    $scope.rootStateHeader = function () {
     
        if ($scope.userInfo) {
            try {
                $state.go(UtilityService.GetCurrentRoute($scope.userInfo.tabs, "booking-home.quick-booking"), {}, { reload: true });
            }
            catch (e) {
                $state.go("login");
            }
        }
        else {
            
        }

    };
    function init() {
        $scope.userInfo = SessionService.getUser();
    }

    init();

});