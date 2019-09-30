/** 
 * Controller
 */
angular.module('ngApp.home').controller('HomeNewTrackingController', function ($scope, $location,$anchorScroll, $state, $stateParams, config, $filter, CountryService, CourierService, HomeService, SessionService, $uibModal, $log, toaster) {

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