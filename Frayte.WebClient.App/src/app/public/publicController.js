/** 
 * Controller
 */
angular.module('ngApp.public').controller('PublicController', function ($scope, $state, $stateParams, config, $location, $anchorScroll, SessionService) {

    //start angular carousel
    $scope.myInterval = 5000;
    $scope.noWrapSlides = false;
    $scope.active = 0;
    var slides = $scope.slides = [];
    var currIndex = 0;

    $scope.addSlide = function () {

        var newWidth = 200 + slides.length + 1;

        slides.push({
            image: $scope.ImagePath + $scope.Image,
            id: currIndex++
        });

    };


    for (var i = 0; i < 6; i++) {
        if (i === 0) {
            $scope.Image = 'bray.png';
        }
        else if (i === 1) {

            $scope.Image = 'Serious_Stuff.png';
        }
        else if (i === 2) {
            $scope.Image = 'Client__2.png';
        }
        else if (i === 3) {
            $scope.Image = 'Client_3.png';
        }
        else if (i === 4) {
            $scope.Image = 'Client__5.png';
        }
        else if (i === 5) {
            $scope.Image = 'Client__6.png';
        }
        $scope.addSlide();
    }
    //end

    $scope.setScroll = function (printSectionId) {
        $location.hash(printSectionId);
        $anchorScroll();
    };

    $scope.headerImagePath = '';

    $scope.RedirectToPublic = function () {
        SessionService.removeUser();
        var url = config.Public_Link;
        var str = config.Public_Link;
        if (str.search("localhost") > -1) {
            url = url + "build";
        }
        window.location.href = url;
    };

    //$scope.rootState = function () {
    //    $scope.userInfo = SessionService.getUser();
    //    if (userInfo.RoleId === 1) {//Admin
    //        //$state.go('admin.direct-shipments', {}, { reload: true });
    //        $state.go('admin.booking-home.booking-welcome', {}, { reload: true });
    //    }
    //    else if (userInfo.RoleId === 3) {//Customer                                        
    //        //$state.go("customer.direct-shipments", {}, { reload: true });
    //        $state.go("customer.booking-home.booking-welcome", {}, { reload: true });
    //    }

    //    else if (userInfo.RoleId === 6) {//Staff                
    //        //$state.go('dbuser.direct-shipments', {}, { reload: true });
    //        $state.go('dbuser.booking-home.booking-welcome', {}, { reload: true });
    //    }

    //};

    function init() {
        var userInfo = SessionService.getUser();
        if ($state.is('public.customer-action')) {
            $scope.customerConfirmReject = true;
        }
        else {
            $scope.customerConfirmReject = false;
        }
        $scope.SiteCompany = config.Site_Company;
        $scope.ImagePath = config.BUILD_URL;
    }

    init();

});