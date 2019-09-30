/** 
 * Controller
 */
angular.module('ngApp.home').controller('HomeController', function (AppSpinner, $scope, $location, $state, $stateParams, config, $filter, HomeService, SessionService, $log, SystemAlertService, DateFormatChange, $anchorScroll, $translate) {


    //multilingual translation code here

    var setModalOptions = function () {
        $translate(['LoadingBulkTracking']).then(function (translations) {
            $scope.LoadingBulkTracking = translations.LoadingBulkTracking;
        });
    };
    var setModalOptions1 = function () {
        $translate(['LoadingTradelaneDetail']).then(function (translations) {
            $scope.LoadingTradelaneDetail = translations.LoadingTradelaneDetail;
        });
    };

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

    $scope.changeState = function () {
        $state.go('home.welcome');
        //$state.go('home.start');
    };
    $scope.setScroll = function (printSectionId) {
        $location.hash(printSectionId);
        $anchorScroll();
    };

    //state to Tracking Detail Page
    $scope.trackingDetail = function (IsValid, easyPostDetail) {
        if (IsValid) {

            if (easyPostDetail.CarrierName === "UK/EU - Shipment") {

                AppSpinner.showSpinnerTemplate($scope.LoadingBulkTracking, $scope.Template);
                $state.go('home.tracking-hub', { carrierType: "UKEUShipment", trackingId: easyPostDetail.TrackingNumber });
                AppSpinner.hideSpinnerTemplate();
            }
            else {
                AppSpinner.showSpinnerTemplate($scope.LoadingBulkTracking, $scope.Template);
                $state.go('home.tracking-hub', { carrierType: easyPostDetail.CarrierName, trackingId: easyPostDetail.TrackingNumber });
                //AppSpinner.showSpinnerTemplate("Loading Bulk Tracking...", $scope.Template);
                AppSpinner.hideSpinnerTemplate();

            }
            $scope.easyPostDetail.TrackingNumber = null;
            $scope.easyPostDetail.CarrierName = null;
        }
    };
    $scope.CheckMawb = function (mawb, type) {
        if (type === "Mawb" && mawb !== null && mawb !== "") {
            $scope.easyPostDetail.TrackingNumber = mawb.replace(/[^0-9- ]/, '');
        }
    };
    $scope.trackingDetailNew = function (IsValid, easyPostDetail) {
        if (IsValid) {

            //if ($scope.BookingTypeName.Name === "DirectBooking" && easyPostDetail.CarrierName !== "FrayteRefNo") {

            //    if (easyPostDetail.CarrierName === "Yodel" || easyPostDetail.CarrierName === "Hermes" || easyPostDetail.CarrierName === "UKMail") {

            //        AppSpinner.showSpinnerTemplate($scope.LoadingBulkTracking, $scope.Template);
            //        $state.go('home.tracking-hub', { carrierType: "UKEUShipment", trackingId: easyPostDetail.TrackingNumber });
            //        AppSpinner.hideSpinnerTemplate();
            //    }
            //    else {
            //        AppSpinner.showSpinnerTemplate($scope.LoadingBulkTracking, $scope.Template);
            //        $state.go('home.tracking-hub', { carrierType: easyPostDetail.CarrierName, trackingId: easyPostDetail.TrackingNumber });
            //        //AppSpinner.showSpinnerTemplate("Loading Bulk Tracking...", $scope.Template);
            //        AppSpinner.hideSpinnerTemplate();

            //    }
            //}
            //if ($scope.BookingTypeName.Name === "DirectBooking" && easyPostDetail.CarrierName === "FrayteRefNo") {
            //    //$scope.easyPostDetail.TrackingNumber = "";
            //    if ($scope.easyPostDetail.TrackingNumber != null) {
            //        $scope.easyPostDetail.TrackingNumber = $scope.easyPostDetail.TrackingNumber.toString();
            //    }
            //    else {
            //        $scope.easyPostDetail.TrackingNumber = "";
            //    }

            //    HomeService.GetDirectBookingDetail($scope.easyPostDetail.TrackingNumber).then(function (response) {
            //        $scope.DirectBookingshipment = response.data;
            //        if ($scope.DirectBookingshipment !== null && $scope.DirectBookingshipment !== undefined) {
            //            if ($scope.DirectBookingshipment.LogisticCompany === "Yodel" || $scope.DirectBookingshipment.LogisticCompany === "Hermes" || $scope.DirectBookingshipment.LogisticCompany === "UKMail") {

            //                AppSpinner.showSpinnerTemplate($scope.LoadingBulkTracking, $scope.Template);
            //                $state.go('home.tracking-hub', { carrierType: "UKEUShipment", trackingId: $scope.DirectBookingshipment.TrackingNo });
            //                AppSpinner.hideSpinnerTemplate();
            //            }
            //            else {
            //                AppSpinner.showSpinnerTemplate($scope.LoadingBulkTracking, $scope.Template);
            //                $state.go('home.tracking-hub', { carrierType: easyPostDetail.CarrierName, trackingId: $scope.DirectBookingshipment.TrackingNo });
            //                //AppSpinner.showSpinnerTemplate("Loading Bulk Tracking...", $scope.Template);
            //                AppSpinner.hideSpinnerTemplate();

            //            }
            //        }
            //    }, function () {

            //    });

            //}
            //else if ($scope.BookingTypeName.Name === "Tradelane") {
            //    HomeService.GetTradelaneShipmentDetail($scope.easyPostDetail.TrackingNumber, $scope.easyPostDetail.CarrierName).then(function (response) {
            //        $scope.Tradelaneshipment = response.data;
            //        if ($scope.Tradelaneshipment !== null && $scope.Tradelaneshipment !== undefined) {
            //            AppSpinner.showSpinnerTemplate($scope.LoadingTradelaneDetail, $scope.Template);
            //            $state.go('home.tradelanetracking-hub', { TradelaneShipmentId: $scope.Tradelaneshipment.TradelaneShipmentId });
            //            AppSpinner.hideSpinnerTemplate();
            //        }
            //    }, function () {

            //    });

            //}
            //HomeService.GetTradelaneShipmentDetail($scope.easyPostDetail.TrackingNumber).then(function (response) {
            //    $scope.Tradelaneshipment = response.data;
            //    if ($scope.Tradelaneshipment !== null && $scope.Tradelaneshipment !== undefined && $scope.Tradelaneshipment.ModuleType === "TradeLaneBooking") {
            //        AppSpinner.showSpinnerTemplate($scope.LoadingTradelaneDetail, $scope.Template);
            //        $state.go('home.tradelanetracking-hub', { "TrackingType": $scope.Tradelaneshipment.TrackingType, "SearchNumber": $scope.easyPostDetail.TrackingNumber });
            //        AppSpinner.hideSpinnerTemplate();
            //        $scope.easyPostDetail.TrackingNumber = null;
            //    }
            //    else if ($scope.Tradelaneshipment !== null && $scope.Tradelaneshipment !== undefined && $scope.Tradelaneshipment.ModuleType === "DirectBooking") {
            //        if ($scope.Tradelaneshipment.CarrierType === "Yodel" || $scope.Tradelaneshipment.CarrierType === "UKMail" || $scope.Tradelaneshipment.CarrierType === "Hermes") {
            //            $scope.Tradelaneshipment.CarrierType = "UKEUShipment";
            //        }
            //        else {

            //        }
            //        AppSpinner.showSpinnerTemplate($scope.LoadingBulkTracking, $scope.Template);
            //        $state.go('home.tracking-hub', { carrierType: $scope.Tradelaneshipment.CarrierType, trackingId: $scope.Tradelaneshipment.Number });
            //        AppSpinner.hideSpinnerTemplate();
            //    }
            //    else {
            //        $state.go('home.tracking-error', { "trackingId": "" });
            //    }
            //}, function () {

            //});
            AppSpinner.showSpinnerTemplate($scope.LoadingBulkTracking, $scope.Template);
            $state.go('home.tradelanetracking-hub', { SearchNumber: easyPostDetail.TrackingNumber });
            AppSpinner.hideSpinnerTemplate();
            //$scope.easyPostDetail.CarrierName = null;
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
            //else if (str.substr(0, 12) === ("home.welcome")) {
            //    return false;
            //}
        else if (str.substr(0, 15) === ("home.parcel-hub")) {
            return false;
        }
        else if (str.substr(0, 17) === ("home.new-tracking")) {
            return false;
        }
        else if (str.substr(0, 26) === ("home.tradelanetracking-hub")) {
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

    $scope.SystemAlertMessage = function () {
        if ($scope.Count > 1) {
            return 'System_Alert';
        }
        else {
            return 'System_AlertMessage';
        }

    };


        //HomeService.GetCurrentOperationZone().then(function (response) {
        //    if (response.data) {
        //        var OperationZoneId = response.data.OperationZoneId;
        //        var CurrentDate = new Date();
        //        SystemAlertService.GetPublicSystemAlert(OperationZoneId, CurrentDate).then(function (response) {
        //            $scope.result = response.data;
        //            $scope.result.reverse();
        //            if ($scope.result.length > 0) {
        //                $scope.Month = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        //                for (i = 0; i < $scope.result.length; i++) {
        //                    var Newdate = $scope.result[i].FromDate;

        //                    var newdate = [];
        //                    var Date = [];
        //                    var Time = [];
        //                    var gtMonth1 = [];
        //                    newdate = Newdate.split('T');
        //                    Date = newdate[0].split('-');
        //                    var gtDate = Date[2];
        //                    gtMonth1 = Date[1].split('');
        //                    var gtMonth2 = Date[1];
        //                    var gtMonth3 = gtMonth1[0] === "0" ? gtMonth1[1] : gtMonth2;
        //                    var gtMonth4 = gtMonth3--;
        //                    var gtMonth = $scope.Month[gtMonth3];
        //                    var gtYear = Date[0];
        //                    Time = $scope.result[i].FromTime.split(':');
        //                    var gtHour = Time[0];
        //                    var gtMin = Time[1];


        //                    $scope.result[i].Date = gtDate + "-" + gtMonth + "-" + gtYear + " - " + gtHour + ":" + gtMin;

        //                }
        //            }
        //            else {
        //                $scope.reslength = true;
        //            }
        //            if ($scope.result.length > 0) {
        //                $scope.reslength = false;
        //                $scope.Count = $scope.result.length;
        //                $scope.PublicPageDate = $scope.result[0].Date;
        //                $scope.GMT = $scope.result[0].TimeZoneDetail.OffsetShort;
        //            }
        //            else {
        //                $scope.reslength = true;
        //            }

        //        });
        //    }
        //});

    function init() {
        $scope.url = SessionService.GetSiteURL();
        var url = config.Public_Link;
        $scope.sURL = SessionService.GetPublicSiteURL() + "/service-alert-detail/";
        $scope.paymentlink = SessionService.GetPaymentSiteURL();
        if (url.search('localhost') === -1) {
            if ($location.$$absUrl.search('frayte.hk') > -1) {
                $scope.url = $scope.url + "#/locale-chTrad-hk";
            }
            else if ($location.$$absUrl.search('frayte.com.hk') > -1) {
                $scope.url = $scope.url + "#/locale-chTrad-hkcm";
            }
            else if ($location.$$absUrl.search('frayte.th') > -1) {
                $scope.url = $scope.url + "#/locale-th-th";
            }
            else {
                var lan = SessionService.getLanguage();
                $scope.url = $scope.url + "#/locale-" + lan;
            }
        }
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';


        $scope.ImagePath = config.BUILD_URL;
        $scope.FrayteWebSite = config.SITE_COUNTRY;
        if ($scope.FrayteWebSite === 'COM') {
            $scope.saleslink = 'sales@frayte.com';
        }else if ($scope.FrayteWebSite === 'CO.UK') {
            $scope.saleslink = 'sales@frayte.co.uk';
        } else if ($scope.FrayteWebSite === 'CO.TH') {
            $scope.saleslink = 'sales@frayte.co.th';
        }

        $scope.easyPostDetail = {
            CarrierName: '',
            TrackingNumber: ''

        };


        $scope.carriers = [{
            CourierId: 1,
            Name: "DHL",
            DisplayName: "DHL"

        }, {
            CourierId: 2,
            Name: "UK/EU - Shipment",
            DisplayName: "UK/EU - Shipment"
        }];

        //HomeService.GetCarriers().then(function (response) {
        //    $scope.carriers = [];
        //    var data = response.data;
        //    for (var i = 0 ; i < data.length; i++) {
        //        if (data[i].CourierType === "Courier") {
        //            data[i].Name = data[i].Name.replace("Courier", "").trim();
        //            $scope.carriers.push(data[i]);
        //        }
        //    } 
        //}, function () { 
        //});

        $scope.ModuleShipmentList = [{
            Id: 1,
            Name: "DirectBooking",
            DisplyName: "Direct Booking"
        },
        {
            Id: 2,
            Name: "Ecommerce",
            DisplyName: "Ecommerce"
        },
        {
            Id: 3,
            Name: "Tradelane",
            DisplyName: "Tradelane"
        }];

        $scope.ModuleTradelaneShipmentList = [{
            Id: 1,
            Name: "FrayteRefNo",
            DisplyName: "Shipment Ref No#"
        },
        {
            Id: 2,
            Name: "Mawb",
            DisplyName: "MAWB"
        }];

        $scope.ModuleEcommShipmentList = [{
            Id: 1,
            Name: "FrayteRefNo",
            DisplyName: "Shipment Ref No#"
        }
        ];

        $scope.ModuleDirectBookingList = [{
            Id: 1,
            Name: "DHL",
            DisplyName: "DHL"
        },
        {
            Id: 2,
            Name: "UPS",
            DisplyName: "UPS"
        },
        {
            Id: 3,
            Name: "TNT",
            DisplyName: "TNT"
        },
        {
            Id: 4,
            Name: "UKMail",
            DisplyName: "UK Mail"
        },
        {
            Id: 5,
            Name: "Yodel",
            DisplyName: "Yodel"
        },
        {
            Id: 6,
            Name: "Hermes",
            DisplyName: "Hermes"
        },
        {
            Id: 7,
            Name: "FrayteRefNo",
            DisplyName: "Frayte Ref No#"
        }];



        //Active current menu in main Menu on active state
        $scope.isCurrentPath = function (path) {
            return $state.is(path);
        };
        $scope.val = false;
        $scope.DateFormat = DateFormatChange.DateFormatChange(new Date());
        // $scope.setScroll('top');
        $anchorScroll.yOffset = 700;
        setModalOptions();
        setModalOptions1();
    }

    init();

});