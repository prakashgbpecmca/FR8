/** 
 * Controller
 */
angular.module('ngApp.loginview').controller('LoginViewController', function ($rootScope, localStorage, $scope, HomeService, DownloadExcelService, $state, $location, $translate, config, $filter, LogonService, SessionService, $uibModal, toaster, SystemAlertService, DateFormatChange, UtilityService, $timeout, $window) {


    //notification code here
    //$scope.notification = function () {
    //    $scope.openNotification = !$scope.openNotification;
    //};
    //end





    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['setting', 'ErrorGetting', 'detail', 'FrayteError']).then(function (translations) {
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TextErrorGettingSettingDetail = translations.ErrorGetting + " " + translations.setting + " " + translations.detail;
        });
        $translate(['access', 'FrayteError', 'information', 'ErrorGetting']).then(function (translations) {
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TextErrorGettingAccessInfo = translations.ErrorGetting + " " + translations.access + " " + translations.information;
        });
    };

    $scope.$on("updateUserInfo", function (event, userInfoDetail) {
        $scope.userInfo = userInfoDetail;
    });

    $scope.$on("UpdateShipmentTabCaption", function (event, tabDetail) {
        if ($scope.tabs !== undefined) {
            $scope.tabs.forEach(function (tab) {
                //if (tab.heading === 'New Shipment') {
                //    tab.heading = 'Modify Shipment';
                //}
            });
        }
    });
    $scope.ProfileModal = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'profile/profile.tpl.html',
            controller: 'ProfileController',
            windowClass: '',
            size: 'lg'
        });
    };

 
    $scope.RedirectSystemAlertDetail = function (Heading) {
        SessionService.removeUser();
        var url = config.Public_Link;
        var str = config.Public_Link;
        if (str.search("localhost") > -1) {
            url = url + "build";
        }
        $scope.sURL = url + "/public/systemAlertDetail/" + Heading;
        //   window.location.href = url + "/public/systemAlertDetail/" + Heading;
    };

    var logOut = function () {

        var USERKEY = "utoken";
        SessionService.removeUser();
        SessionService.removeLanguage();
        SessionService.removeModuleType();
        localStorage.remove(USERKEY);
        var url = "";
        if (config.Public_Link.search("localhost") === -1) {
            if (SessionService.getPublicSite()) {
                url = SessionService.getPublicSite();
            }
            else {
                url = config.Public_Link;
            }
        }
        else {
            url = config.Public_Link + "build";
        }

        window.location.href = url;

    };
    $rootScope.Logout = function () {
        if ($rootScope.selectedModule) {
            $scope.userInfo = SessionService.getUser();
            if ($rootScope.unmanifestedCount && ($scope.userInfo.RoleId === 3 || $scope.userInfo.RoleId === 6)) {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'loginView/unManifestedJobsAlert.tpl.html',
                    controller: 'UnManifestedJobsAlertController',
                    windowClass: '',
                    size: 'md'
                });
                modalInstance.result.then(function () {
                    logOut();
                }, function () {
                    //
                });
            }
            else {
                logOut();
            }
        }
        else {
            logOut();
        }
    };
    $scope.unAuthorizeClicked = function () {

    };
    $scope.rootStateHeader = function () {
        SessionService.removeModuleType();
        $state.go("loginView.services");
        $rootScope.isNotFound = false;
    };
    $scope.rootState = function () {
        $rootScope.isNotFound = false;
    };


    //getsystemalert

    $rootScope.GetSystemAlert = function (OperationZoneId) {
        var CurrentDate = new Date();
        SystemAlertService.GetPublicSystemAlert(OperationZoneId, CurrentDate).then(function (response) {
            $scope.result = response.data;
            $scope.result.reverse();
            if ($scope.result.length > 0) {
                $scope.reslength = false;
                $scope.val = false;
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
                $rootScope.reslength = true;
            }
            if ($scope.result.length > 0) {
                $rootScope.reslength = false;
                $scope.Count = $scope.result.length;
                $scope.PublicPageDate = $scope.result[0].Date;
                $scope.GMT = $scope.result[0].TimeZoneDetail.OffsetShort;
            }
            else {
                $rootScope.reslength = true;
            }
        });
    };

    var isMenifestVisible = function () {
        HomeService.IsManifestSupprt($scope.userInfo.EmployeeId).then(function (response) {
            if (response.data != null) {
                $scope.ManifestDetail = response.data;
                $rootScope.isShow = response.data.ManifestSupport;
            }
            else {
                $rootScope.isShow = false;
            }
            if ($rootScope.isShow) {
                $rootScope.ShowManifestButton = true;
            }
            else {
                $rootScope.ShowManifestButton = false;
            }

        }, function () {
            $rootScope.ShowManifestButton = false;
        });
    };

    $scope.SystemAlertMessage = function () {
        if ($scope.Count > 1) {
            return 'System_Alert';
        }
        else {
            return 'System_AlertMessage';
        }

    };

    //small and large menu switch when screen scroll
    //(function ($) {
    //    $(document).ready(function () {
    //        //$(".small-menu").hide();
    //        $(".large-menu").show();
    //        $(function () {
    //            $(window).scroll(function () {
    //                if ($(this).scrollTop() > 150) {
    //                    $(".small-menu").show();
    //                    $(".large-menu").hide();
    //                }
    //                else {
    //                    $(".small-menu").hide();
    //                    $(".large-menu").show();
    //                }
    //            });
    //        });
    //        $('.menu-button').click(function () {
    //            $(".large-menu").show();
    //            $(".small-menu").hide();
    //        });
    //    });
    //}(jQuery));
    $scope.moduleType = function () {
        var moduleType = SessionService.getModuleType();
        return moduleType;
    };


    //getsystemalert

    $rootScope.GetSystemAlert = function (OperationZoneId) {
        var CurrentDate = new Date();
        SystemAlertService.GetPublicSystemAlert(OperationZoneId, CurrentDate).then(function (response) {
            $scope.result = response.data;
            $scope.result.reverse();
            if ($scope.result.length > 0) {
                $scope.reslength = false;
                $scope.val = false;
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
                $rootScope.reslength = true;
            }
            if ($scope.result.length > 0) {
                $rootScope.reslength = false;
                $scope.Count = $scope.result.length;
                $scope.PublicPageDate = $scope.result[0].Date;
                $scope.GMT = $scope.result[0].TimeZoneDetail.OffsetShort;
            }
            else {
                $rootScope.reslength = true;
            }
        });
    };

    $scope.SystemAlertMessage = function () {
        if ($scope.Count > 1) {
            return 'System_Alert';
        }
        else {
            return 'System_AlertMessage';
        }

    };
    $scope.RedirectSystemAlertDetail = function (Heading) {
        SessionService.removeUser();
        var url = config.Public_Link;
        var str = config.Public_Link;
        if (str.search("localhost") > -1) {
            url = url + "build";
        }
        $scope.sURL = url + "/service-alert-detail/" + Heading;
        //   window.location.href = url + "/public/systemAlertDetail/" + Heading;
    };

    $rootScope.goToBooking = function (ShipmentType) {
        if (ShipmentType) {
            if (ShipmentType === 'DirectBooking') {
                $rootScope.selectedImage = 'directbooking-30.png';
                $rootScope.selectedModule = "Direct Booking";
                $rootScope.IsDirectBookingSelected = true;
                $rootScope.IseCommerceBookingSelected = false;

            }
            if (ShipmentType === 'eCommerce') {
                $rootScope.selectedModule = "eCommerce";
                $rootScope.selectedImage = 'ecommerce-30.png';
                $rootScope.IsDirectBookingSelected = false;
                $rootScope.IseCommerceBookingSelected = true;

            }
            if (ShipmentType === 'Tradelane') {
                return;
            }

            SessionService.setModuleType(ShipmentType);
        }

        // Get the tabe base don ModuleType 

        $rootScope.getUserTabs(ShipmentType);
    };

    $rootScope.getUserTabs = function (ShipmentType) {
        UtilityService.getUserTabs($scope.customerId, $scope.RoleId, ShipmentType).then(function (response) {
            if (response.data) {
                var user = SessionService.getUser();
                user.tabs = response.data;
                SessionService.removeUser();
                user.moduleType = ShipmentType;
                SessionService.setUser(user);
                var user1 = SessionService.getUser();
                $rootScope.frayteTabs = response.data;
                if (response.data && response.data.length) {
                    if (response.data[0].route.search('direct-booking') > -1) {
                        $state.go(response.data[0].route, { directShipmentId: 0 }, { relaod: true });
                    }
                    else if (response.data[0].route.search('eCommerce-booking') > -1 || response.data[0].route.search('tradelane-booking') > -1) {
                        $state.go(response.data[0].route, { shipmentId: 0 }, { relaod: true });
                    }
                    else {
                        $state.go(response.data[0].route, {}, { reload: true });
                    }
                }
            }
        }, function () {
            toaster.pop({
                type: 'error',
                showCloseButton: true
            });
        });
    };

    $rootScope.setModuleType = function (ShipmentType) {
        if (ShipmentType) {
            if (ShipmentType === 'DirectBooking') {
                $rootScope.selectedImage = 'directbooking-30.png';
                $translate(['Direct_Bookingg']).then(function (translations) {
                    $rootScope.selectedModule = translations.Direct_Bookingg;
                });

                $rootScope.IsDirectBookingSelected = true;
                $rootScope.IseCommerceBookingSelected = false;

            }
            if (ShipmentType === 'eCommerce') {
                $translate(['Ecommerce']).then(function (translations) {
                    $rootScope.selectedModule = translations.Ecommerce;
                });

                $rootScope.selectedImage = 'ecommerce-30.png';
                $rootScope.IsDirectBookingSelected = false;
                $rootScope.IseCommerceBookingSelected = true;

            }
            if (ShipmentType === 'Tradelane') {
                return;
            }

            SessionService.setModuleType(ShipmentType);
        }
    };
    function init() {
       // $scope.ImagePath = config.BUILD_URL;
       // $scope.openNotification = false;

        $scope.isCollapsed = false;
        var state = $state;
        setModalOptions();

        var url = config.Public_Link;
        var str = config.Public_Link;
        if (str.search("localhost") > -1) {
            url = url + "build";
        }
        $scope.sURL = url + "/service-alert-detail/";

        var userInfo = SessionService.getUser();
        $scope.userInfo = SessionService.getUser();
        if ($scope.userInfo.modules) {
            $rootScope.CustomerModules = $scope.userInfo.modules;
        }

        $scope.customerId = userInfo.EmployeeId;

        $scope.RoleId = userInfo.RoleId;

        $scope.DayName = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
        $scope.CurrentDate = new Date();
        $scope.CurrentDay = $scope.DayName[$scope.CurrentDate.getDay()];
        $scope.CurrentDateTime = $scope.CurrentDay + ", " + DateFormatChange.DateFormatChange(new Date());
        var OperationZoneId = $scope.userInfo.OperationZoneId;
        $rootScope.GetSystemAlert(OperationZoneId);
        if (SessionService.getModuleType()) {
            if (SessionService.getModuleType() === 'DirectBooking') {
                $translate(['Direct_Bookingg']).then(function (translations) {
                    $rootScope.selectedModule = translations.Direct_Bookingg;
                });

                $rootScope.IsDirectBookingSelected = true;
                $rootScope.IseCommerceBookingSelected = false;
                $rootScope.selectedImage = 'directbooking-30.png';
            }
            if (SessionService.getModuleType() === 'eCommerce') {
                $rootScope.IsDirectBookingSelected = false;
                $rootScope.IseCommerceBookingSelected = true;
                $rootScope.selectedImage = 'ecommerce-30.png';
                $translate(['Ecommerce']).then(function (translations) {
                    $rootScope.selectedModule = translations.Ecommerce;
                });
            }
            if (SessionService.getModuleType() === 'Tradelane') {
                $rootScope.selectedImage = 'tradelane-30.png';
            }
            if ($scope.userInfo.moduleType === 'ExpressSolution') {
                $rootScope.selectedImage = 'express-solutions-30.png';
                $translate(['Express_Booking']).then(function (translations) {
                    $rootScope.selectedModule = translations.Express_Booking;
                });
            }
        }

    }
    init();

});