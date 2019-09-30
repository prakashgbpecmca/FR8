/** 
 * Controller
 */
angular.module('ngApp.public').controller('PublicTrackingFAQController', function ($scope, $state, $stateParams, $translate, $location, config, $filter, SessionService, $timeout, toaster) {
 
    function init() {
        $scope.pageTitle = "Tracking FAQ's";
    }
    init();

});