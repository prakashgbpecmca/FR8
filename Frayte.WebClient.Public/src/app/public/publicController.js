/** 
 * Controller
 */
angular.module('ngApp.public').controller('PublicController', function ($scope, $state, $stateParams, config,$location, $anchorScroll) {

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

    function init() {
        if ($state.is('public.customer-action')) {
            $scope.customerConfirmReject = true;
        }
        else {
            $scope.customerConfirmReject = false;
        }
        
        $scope.ImagePath = config.BUILD_URL;
    }
    $scope.ImagePath = config.BUILD_URL;
    $scope.FrayteWebSite = config.SITE_COUNTRY;
    if ($scope.FrayteWebSite === 'COM') {
        $scope.saleslink = 'sales@frayte.com';
    }
    else if ($scope.FrayteWebSite === 'CO.UK') {
        $scope.saleslink = 'sales@frayte.co.uk';
    }


    init();

});