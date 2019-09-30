/** 
 * Controller
 */
angular.module('ngApp.home').controller('HomeController', function (AppSpinner, $scope, $location, $state, $stateParams, config, $filter, CountryService, CourierService, HomeService, SessionService, $uibModal, $log, toaster, SystemAlertService, DateFormatChange, $anchorScroll) {
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
        if (i === 0)
        {
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

    //state to Tracking Detail Page
    $scope.trackingDetail = function (IsValid, easyPostDetail) {
        if (IsValid) {
        
            if (easyPostDetail.CarrierName === "UK/EU - Shipment") {
                
                AppSpinner.showSpinnerTemplate("Loading Bulk Tracking...", $scope.Template);
                $state.go('home.tracking-hub', { carrierType: "UKEUShipment", trackingId: easyPostDetail.TrackingNumber });
                AppSpinner.hideSpinnerTemplate();
            }
            else {
                AppSpinner.showSpinnerTemplate("Loading Bulk Tracking...", $scope.Template);
                $state.go('home.tracking-hub', { carrierType: easyPostDetail.CarrierName, trackingId: easyPostDetail.TrackingNumber });
                AppSpinner.showSpinnerTemplate("Loading Bulk Tracking...", $scope.Template);
                
            }
            $scope.easyPostDetail.TrackingNumber = null;
            $scope.easyPostDetail.CarrierName = null;
        }
    };

    $scope.open = function (size) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'logon/logon.tpl.html',
            controller: 'LogonController',
            windowClass: 'Logon-Modal',
            size: 'sm'
        });
    };

    $scope.publicContentPages = function () {
        var str = $state.current.name;
        if (str.substr(0, 14) === "home.shipment.") {
            return false;
        }
        else if (str.substr(0, 12) === ("home.contact")) {
            return false;
        }
        else {
            return true;
        }

        //var isShipment = $state.current.name.indexOf("home.shipment.");
        //return isShipment;
    };

    $scope.publicAndTrackPages = function () {
        var str = $state.current.name;
        if (str.substr(0, 14) === "home.shipment.") {
            return false;
        }
        else if (str.substr(0, 13) === ("home.tracking")) {
            return false;
        }
        else if (str.substr(0, 18) === ("home.bulk-tracking")) {
            return false;
        }
        else if (str.substr(0, 12) === ("home.welcome")) {
            return false;
        }
        else if (str.substr(0, 15) === ("home.parcel-hub")) {
            return false;
        }
        else if (str.substr(0, 17) === ("home.new-tracking")) {
            return false;
        }
        else {
            return true;
        }

        //var isShipment = $state.current.name.indexOf("home.shipment.");
        //return isShipment;
    };

    // bulk tracking
    $scope.goToBulkTracking = function () {
        $state.go('home.bulk-tracking');
    };


    //small and large menu switch when screen scroll
    (function ($) {
        $(document).ready(function () {
            //$(".small-menu").hide();
            $(".large-menu").show();
            $(function () {
                $(window).scroll(function () {
                    if ($(this).scrollTop() > 150) {
                        $(".small-menu").show();
                        $(".large-menu").hide();
                    }
                    else {
                        $(".small-menu").hide();
                        $(".large-menu").show();
                    }
                });
            });
            $('.menu-button').click(function () {
                $(".small-menu").hide();
                $(".large-menu").show();
            });
        });
    }(jQuery));

    function init() {
       

        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        HomeService.GetCurrentOperationZone().then(function (response) {
            if (response.data) {
                var OperationZoneId = response.data.OperationZoneId;
                var CurrentDate = new Date();
                SystemAlertService.GetPublicSystemAlert(OperationZoneId, CurrentDate).then(function (response) {
                    $scope.result = response.data;
                    $scope.result.reverse();
                    if ($scope.result.length > 0) {
                        $scope.Month = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                        for (i = 0; i < $scope.result.length; i++) {
                            var Newdate = $scope.result[i].FromDate;

                            var newdate = [];
                            var Date = [];
                            var Time = [];
                            var gtMonth1 = [];
                            newdate = Newdate.split('T');
                            Date = newdate[0].split('-');
                            var gtDate = Date[2];
                            gtMonth1 = Date[1].split('');
                            var gtMonth2 = Date[1];
                            var gtMonth3 = gtMonth1[0] === "0" ? gtMonth1[1] : gtMonth2;
                            var gtMonth4 = gtMonth3--;
                            var gtMonth = $scope.Month[gtMonth3];
                            var gtYear = Date[0];
                            Time = $scope.result[i].FromTime.split(':');
                            var gtHour = Time[0];
                            var gtMin = Time[1];


                            $scope.result[i].Date = gtDate + "-" + gtMonth + "-" + gtYear + " - " + gtHour + ":" + gtMin;

                        }
                    }
                    else {
                        $scope.reslength = true;
                    }
                    if ($scope.result.length > 0) {
                        $scope.reslength = false;
                        $scope.Count = $scope.result.length;
                        $scope.PublicPageDate = $scope.result[0].Date;
                        $scope.GMT = $scope.result[0].TimeZoneDetail.OffsetShort;
                    }
                    else {
                        $scope.reslength = true;
                    }
                    
                });
            }
        });
        
        $scope.ImagePath = config.BUILD_URL;
        $scope.FrayteWebSite = config.SITE_COUNTRY;
        if ($scope.FrayteWebSite === 'COM') {
            $scope.saleslink = 'sales@frayte.com';
        }
        else if ($scope.FrayteWebSite === 'CO.UK') {
            $scope.saleslink = 'sales@frayte.co.uk';
        }

        $scope.easyPostDetail = {
            CarrierName: '',
            TrackingNumber: ''

        };
        HomeService.GetCarriers().then(function (response) {
            $scope.carriers = [];
            var data = response.data;
            for (var i = 0 ; i < data.length; i++) {
                if (data[i].CourierType === "Courier") {
                    data[i].Name = data[i].Name.replace("Courier", "").trim();
                    $scope.carriers.push(data[i]);
                }
            }

        }, function () {

        });
        //Active current menu in main Menu on active state
        $scope.isCurrentPath = function (path) {
            return $state.is(path);
        };
        $scope.val = false;
        $scope.DateFormat = DateFormatChange.DateFormatChange(new Date());
       // $scope.setScroll('top');
        $anchorScroll.yOffset = 700;
    }

    init();

});