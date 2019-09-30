/** 
 * Controller
 */
angular.module('ngApp.home').controller('HomeNewTrackingController', function ($scope, $location,$anchorScroll, $state, $stateParams, config, $filter, HomeService, SessionService,$log) {

    //state to Tracking Detail Page
    var setScroll = function () {
        $location.hash('seach');
        $anchorScroll();
    };
    function init() {
        $anchorScroll.yOffset = 100;
        setScroll();
    }

    init();

});