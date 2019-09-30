angular.module('ngApp', [
  'templates-app',
  'templates-common',
  'ngApp.home',
  'ngApp.shipment',
  'ngApp.logon',
  'ngApp.receiver',
  'ngApp.shipper',
  'ngApp.forgetPassword',
  'ngApp.customer',
  'ngApp.newPassword',
  'ngApp.carrier',
  'ngApp.country',
  'ngApp.courier',
  'ngApp.agent',
  'ngApp.user',
  'ngApp.utility',
  'ngApp.warehouse',
  'ngApp.tradelane',
  'ngApp.common',
  'ngApp.public',
  'ui.router',
  'ui.validate',
  'ui.bootstrap',
  'ui.grid',
  'ui.grid.selection',
  'ngApp.setting',
  'angular-loading-bar',
  'cfp.loadingBar',
  'ngAnimate',
  'toaster',
  'ngFileUpload',
  'ngApp.currentShipment',
  'ngApp.pastShipment',
   'ngApp.trackAndTraceDashboard',
  'ngApp.profile',
  'ngApp.previewMAWB',
  'ngApp.loginview',
  'ngApp.weekdays',
  'ui.mask',
  'uiGmapgoogle-maps',
  'pascalprecht.translate',
  'textAngular',
  'digitalfondue.dftabmenu',
  'ngApp.termandcondition',
  'ngApp.downloadExcel',
  'ngApp.hsCode',
  "ngApp.hsCodejobs",
  'ngSanitize',
  'ngApp.eCommerce',
  'ngApp.security',
  'ngApp.buildConsignmment',
  'ngApp.reportSetting',
  'ngApp.specialDelivery',
  'ngApp.quotationTools',
  'ngApp.parcelhub',
  'ngApp.margin',
  'ngApp.courierAccount',
  'ngApp.zoneSetting',
  'ngApp.zoneCountry',
  'ngApp.exchangeRate',
  'ngApp.fuelSurCharge',
  'ngApp.systemAlert',
  'ngApp.adminCharge',
  'ngApp.thirdPartyMatrix',
  'ngApp.baseRateCard',
  'ngApp.zonePostCode',
  'ngApp.CountryZonePostCode',
  'ngApp.directBooking',
  'ngApp.customerSetting',
  'ngApp.AddressBook',
  'ngApp.bookingHome',
  'ngApp.accessLevel',
  'ngApp.uploadShipment',
  'colorpicker.module',
  'ngApp.dashBoard',
  'ngApp.ecommerceSetting',
  'ngApp.apiUser',
  'ngApp.directBookingUploadShipment',
  'ngApp.profileandsetting',
  'ngApp.servicecode',
  'ngApp.userprofile',
  'ngApp.tradelaneBooking',
  'ngApp.mileStone',
  'ngApp.tradelaneShipments',
  'ngApp.preAlert',
  'ngApp.tradelaneStaffBoard',
  'ngApp.tradelanePreAlertDashBoardBoard',
  'ngApp.breakBulk',
  'ngApp.express',
  'ngApp.customerStaff',
  'ngApp.tracking'
])

//Web Api Service URL
//.constant('config', { 'SERVICE_URL': 'http://frayteuk.godemowithus.com/WebApi/api', 'BUILD_URL': '../assets/', 'IS_STATIC': 'False' ,'SITE_COUNTRY':'COM' })
//.constant('config', { 'SERVICE_URL': 'http://admin-pc/webapi/api', 'BUILD_URL': '../build/assets/', 'IS_STATIC': 'False' ,'SITE_COUNTRY':'COM' })

//.constant('config', {
//    'SERVICE_URL': 'https://app.frayte.com/WebAPI/api',
//    'BUILD_URL': '//app.frayte.com/assets/',
//    'IS_STATIC': 'False', 
//    'SITE_COUNTRY': 'COM',
//    'SITE_COMPANY': "FRAYTE", //FRAYTE //MAXLOGISTIC
//    'Public_Link': 'https://www.FRAYTE.com/',
//    'Login_Link': 'https://app.frayte.com/#/login',
//    'Stripe_PaymentKey': 'pk_test_oSsZz9lGM9QdWxqsoDK7442m',
//    'Paypal_PaymentKey_Sandbox': 'AZAi_j8QZo_B7jczaUrwCHrwScckYnJPD2TidtYmSl_0orNL0nCBI3dwcrQdmp_7ze_CSTDT8CjHvFxJ',
//    'Paypal_PaymentKey_Production': 'AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN', //AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN
//    'Paypal_PaymentMode': "sandbox" // Optional: specify 'sandbox' or 'production' environment
//})

//.constant('config', {
//    'SERVICE_URL': 'http://frayte.godemowithus.com/WebAPI/api',
//    'BUILD_URL': '../assets/',
//    'IS_STATIC': 'False', 
//    'SITE_COUNTRY': 'COM',
//    'SITE_COMPANY': "FRAYTE", //FRAYTE //MAXLOGISTIC
//    'Public_Link': 'http://fraytepublic.godemowithus.com/',
//    'Login_Link': 'http://fraytepublic.godemowithus.com/#/login',
//    'Stripe_PaymentKey': 'pk_test_oSsZz9lGM9QdWxqsoDK7442m',
//    'Paypal_PaymentKey_Sandbox': 'AZAi_j8QZo_B7jczaUrwCHrwScckYnJPD2TidtYmSl_0orNL0nCBI3dwcrQdmp_7ze_CSTDT8CjHvFxJ',
//    'Paypal_PaymentKey_Production': 'AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN', //AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN
//    'Paypal_PaymentMode': "sandbox" // Optional: specify 'sandbox' or 'production' environment
//})

//.constant('config', {
//    'SERVICE_URL': 'https://app.frayte.co.uk/WebAPI/api',
//    'BUILD_URL': '//app.frayte.co.uk/assets/',
//    'IS_STATIC': 'False',
//    'SITE_COUNTRY': 'CO.UK',
//    'SITE_COMPANY': "FRAYTE", //FRAYTE //MAXLOGISTIC
//    'Public_Link': 'https://www.FRAYTE.co.uk/',
//    'Login_Link': 'https://app.frayte.co.uk/#/login',
//    'Stripe_PaymentKey': 'pk_test_oSsZz9lGM9QdWxqsoDK7442m',
//    'Paypal_PaymentKey_Sandbox': 'AZAi_j8QZo_B7jczaUrwCHrwScckYnJPD2TidtYmSl_0orNL0nCBI3dwcrQdmp_7ze_CSTDT8CjHvFxJ',
//    'Paypal_PaymentKey_Production': 'AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN', //AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN
//    'Paypal_PaymentMode': "sandbox" // Optional: specify 'sandbox' or 'production' environment
//})

//.constant('config', {
//    'SERVICE_URL': 'http://app.godemowithus.com/WebAPI/api',
//    'BUILD_URL': '../assets/',
//    'IS_STATIC': 'False',
//    'SITE_COUNTRY': 'CO.UK',
//    'SITE_COMPANY': "FRAYTE", //FRAYTE //MAXLOGISTIC
//    'Public_Link': 'http://public.godemowithus.com/',
//    'Login_Link': 'http://app.godemowithus.com/#/login',
//    'Stripe_PaymentKey': 'pk_test_oSsZz9lGM9QdWxqsoDK7442m',
//    'Paypal_PaymentKey_Sandbox': 'AZAi_j8QZo_B7jczaUrwCHrwScckYnJPD2TidtYmSl_0orNL0nCBI3dwcrQdmp_7ze_CSTDT8CjHvFxJ',
//    'Paypal_PaymentKey_Production': 'AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN', //AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN
//    'Paypal_PaymentMode': "sandbox" // Optional: specify 'sandbox' or 'production' environment
//})

.constant('config', {
    'SERVICE_URL': 'http://localhost:24047/api',
    'BUILD_URL': '../build/assets/',
    'IS_STATIC': 'False',
    'SITE_COUNTRY': 'CO.UK',
    'SITE_COMPANY': "FRAYTE", //MAXLOGISTIC
    'Public_Link': 'http://localhost:63124/',
    'Login_Link': 'http://localhost:62992/build/index.html#/login',
    'Stripe_PaymentKey': 'pk_test_LAzRL6p00u5RC7f89c4YQU4e', // Live : pk_live_CQWu900naWwQnWMIjeAadIMZ Test: pk_test_LAzRL6p00u5RC7f89c4YQU4e
    'Paypal_PaymentKey_Sandbox': 'AbR6rmsfDGvZ_PSXOD1DbuUWRzjQh0CjjTH3Crz9efP1VuwXYxpTwYgQuwIQhmi-oaVIbNfZu1rm_a08',
    'Paypal_PaymentKey_Production': 'AYRUTwDNJdmR_vsPjhK8XhdKkplwXWgrml7tHJqsnyjRIEK0gU55B-bq4CVEd0EkXX7005GokVN9XQuY',
    'Paypal_PaymentMode': "sandbox" // Optional: specify 'sandbox' or 'production' environment
})

//.constant('config', {
//    'SERVICE_URL': 'http://tradelane.godemowithus.com/WebAPI/api',
//    'BUILD_URL': '../assets/',
//    'IS_STATIC': 'False',
//    'SITE_COUNTRY': 'CO.UK',
//    'SITE_COMPANY': "FRAYTE", //FRAYTE //MAXLOGISTIC
//    'Public_Link': 'http://public.godemowithus.com/',
//    'Login_Link': 'http://public.godemowithus.com/#/login',
//    'Stripe_PaymentKey': 'pk_test_oSsZz9lGM9QdWxqsoDK7442m',
//    'Paypal_PaymentKey_Sandbox': 'AZAi_j8QZo_B7jczaUrwCHrwScckYnJPD2TidtYmSl_0orNL0nCBI3dwcrQdmp_7ze_CSTDT8CjHvFxJ',
//    'Paypal_PaymentKey_Production': 'AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN', //AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN
//    'Paypal_PaymentMode': "sandbox" // Optional: specify 'sandbox' or 'production' environment
//})
 
//.constant('config', {
//    'SERVICE_URL': 'http://mex.godemowithus.com/WebAPI/api',
//    'BUILD_URL': '../assets/',
//    'IS_STATIC': 'False',
//    'SITE_COUNTRY': 'CO.UK',
//    'SITE_COMPANY': "MAXLOGISTIC", //FRAYTE //MAXLOGISTIC
//    'Public_Link': 'http://mex.godemowithus.com/',
//    'Login_Link': 'http://mex.godemowithus.com/#/login',
//    'Stripe_PaymentKey': 'pk_test_oSsZz9lGM9QdWxqsoDK7442m',
//    'Paypal_PaymentKey_Sandbox': 'AZAi_j8QZo_B7jczaUrwCHrwScckYnJPD2TidtYmSl_0orNL0nCBI3dwcrQdmp_7ze_CSTDT8CjHvFxJ',
//    'Paypal_PaymentKey_Production': 'AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN', //AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN
//    'Paypal_PaymentMode': "sandbox" // Optional: specify 'sandbox' or 'production' environment
//})

//.constant('config', {
//    'SERVICE_URL': 'http://app.mexlogistics.com/WebAPI/api',
//    'BUILD_URL': '../assets/',
//    'IS_STATIC': 'False', 
//    'SITE_COUNTRY': 'CO.UK',
//    'SITE_COMPANY': "MAXLOGISTIC",
//    'Public_Link': 'http://app.mexlogistics.com/',
//    'Login_Link': 'http://app.mexlogistics.com/#/login',
//    'Stripe_PaymentKey': 'pk_test_oSsZz9lGM9QdWxqsoDK7442m',
//    'Paypal_PaymentKey_Sandbox': 'AZAi_j8QZo_B7jczaUrwCHrwScckYnJPD2TidtYmSl_0orNL0nCBI3dwcrQdmp_7ze_CSTDT8CjHvFxJ',
//    'Paypal_PaymentKey_Production': 'AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN', //AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN
//    'Paypal_PaymentMode': "sandbox" // Optional: specify 'sandbox' or 'production' environment
//})

.config(function myAppConfig($stateProvider, $urlRouterProvider, $httpProvider, $locationProvider) {
    //  $locationProvider.html5Mode(true);
    $urlRouterProvider.otherwise('/404-page-not-found');
})

.config(function (uiGmapGoogleMapApiProvider) {
    uiGmapGoogleMapApiProvider.configure({
        //    key: 'your api key',
        v: '3.20', //defaults to latest 3.X anyhow
        libraries: 'weather,geometry,visualization'
    });
})

.config(['cfpLoadingBarProvider', function (cfpLoadingBarProvider) {
    cfpLoadingBarProvider.includeSpinner = false;
    //cfpLoadingBarProvider.loadingBarTemplate = '<div id="loading-bar"><div class="spinner-icon">Frayte Loading .... </div></div>';
}])

//.config(function (datepickerConfig, datepickerPopupConfig) {
//    datepickerConfig.showWeeks = false;
//    datepickerConfig.startingDay = 1;
//    datepickerConfig.formatYear = 'yy';
//    datepickerConfig.datepickermode = 'day';
//    datepickerPopupConfig.showButtonBar = false;
//})

.config(function ($provide) {
    $provide.decorator("$exceptionHandler", function ($delegate, $injector) {
        return function (exception, cause) {
            $delegate(exception, cause);

            var toaster = $injector.get("toaster");

            toaster.pop({
                type: 'error',
                title: 'Frayte-Error',
                body: 'An error occurred, please check the console for detail',
                showCloseButton: true
            });
        };
    });
})

 //.config(['$translateProvider', function ($translateProvider) {
    //    $translateProvider.useStaticFilesLoader({
    //        prefix: '../build/downloadform/locale-',
    //        suffix: '.json'
    //    });
    //    $translateProvider.preferredLanguage('en');
    //    //  $translateProvider.useSanitizeValueStrategy('sanitize');
    //}])

.config(['$translateProvider', function ($translateProvider) {
    $translateProvider.useStaticFilesLoader({
        prefix: '../downloadform/locale-',
        suffix: '.json'
    });
    $translateProvider.preferredLanguage('en');
    //  $translateProvider.useSanitizeValueStrategy('sanitize');
}])

.run(function run($window, $rootScope, SessionService, $state, $uibModal) {
    window.onbeforeunload = function (evt) {
        SessionService.windowUnload(evt);
    };
    $rootScope.directBookingChange = true;
    $rootScope.manageDirectBookingChange = true;
    $rootScope.$on('$stateChangeStart', function (e, toState, toParams, fromState, fromParams) {
        if (fromState !== undefined && fromState !== null && fromState.name !== undefined) {
            var array = fromState.name.split('.');
            var FromState = "";
            if (array !== undefined && array !== null && array.length > 2) {
                FromState = array[2].substr(0, 14);
            }
            if (FromState === "direct-booking" && !fromState.data.IsFired) {
                e.preventDefault();
                return;
            }
            else {
                if (array !== undefined && array !== null && array.length > 2) {
                    FromState = array[2].substr(0, 17);
                }
                if (FromState === "eCommerce-booking" && !fromState.data.IsFired) {
                    e.preventDefault();
                    return;
                }
            }



            if ((FromState === "direct-booking" || FromState === "direct-booking-cl" || FromState === "direct-booking-re") && fromState.data.IsFired && $rootScope.manageDirectBookingChange &&
                $rootScope.directBookingChange) {
                e.preventDefault();
                fromState.data.IsFired = false;
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'directBooking/directBookingStateChange.tpl.html',
                    windowClass: '',
                    size: 'md',
                    backdrop: 'static'
                });
                modalInstance.result.then(function () {
                    // Go tp To State
                    fromState.data.IsFired = true;  // so that next time pop-up should be open
                    $rootScope.directBookingChange = false;
                    $state.go(toState.name);

                }, function () {
                    // prevent state change
                    fromState.data.IsFired = true;  // so that next time pop-up should be open
                    $rootScope.directBookingChange = true;
                    $state.go(fromState.name);
                });

            }
            if (FromState === "eCommerce-booking" && fromState.data.IsFired && $rootScope.manageDirectBookingChange && $rootScope.directBookingChange) {
                e.preventDefault();
                fromState.data.IsFired = false;
                var modalInstance1 = $uibModal.open({
                    animation: true,
                    templateUrl: 'directBooking/directBookingStateChange.tpl.html',
                    windowClass: '',
                    size: 'md',
                    backdrop: 'static'
                });
                modalInstance1.result.then(function () {
                    // Go tp To State
                    fromState.data.IsFired = true;  // so that next time pop-up should be open
                    $rootScope.directBookingChange = false;
                    $state.go(toState.name, { moduleType: "eCb" });

                }, function () {
                    // prevent state change
                    fromState.data.IsFired = true;  // so that next time pop-up should be open
                    $rootScope.directBookingChange = true;
                    $state.go(fromState.name);
                });

            }
        }
    });

    $rootScope.online = navigator.onLine;
    $window.addEventListener("offline", function () {
        $rootScope.$apply(function () {
            $rootScope.online = false;
        });
    }, false);
    $window.addEventListener("online", function () {
        $rootScope.$apply(function () {
            $rootScope.online = true;
        });
    }, false);
})

.controller('AppCtrl', function AppCtrl($scope, config, $interval, UtilityService, $location, $uibModal, $http, $log, $rootScope, $translate, SessionService, $state) {

    $rootScope.SITECOMPANY = config.SITE_COMPANY;
    $rootScope.ImagePathRoot = config.BUILD_URL;
    //Hide the Action menu on grid scroll
    $rootScope.onGridScroll = function ($direct, $event) {
        hideCustomGridActionMenu();
    };

    //Find the Coordinate of the clicked html element

    $rootScope.findActionMenuCoordinate = function (event, id, CallingType, popUpSize) {

        var el = document.getElementById(id);

        var left = 0;
        var elen = document.getElementsByClassName("modal");
        var pos = document.getElementsByClassName("modal-dialog");
        if (pos && pos[0] && pos[0].offsetLeft) {
            if (popUpSize === "lg") {
                left = pos[0].offsetLeft;
            }
            else if (popUpSize === "md") {
                left = pos[0].offsetLeft;
            }
            else if (popUpSize === "sm") {
                left = pos[0].offsetLeft;
            }
        }
        var top = 0;
        if (elen && elen[0] && elen[0].scrollTop) {
            top = elen[0].scrollTop;
        }

        if (CallingType && CallingType === 'PopUp') {
            $rootScope.x = event.clientX - (left + (el.clientWidth + 28)); // //   set left of menu-pop accoding to it's width 
            $rootScope.y = event.clientY - (80 - top); //  gives the coordinates relative to the viewport in CSS pixels.
        }
        else {
            // For Normal Grid
            $rootScope.x = event.clientX - (el.clientWidth + 28); //   set left of menu-pop accoding to it's width  
            $rootScope.y = event.clientY; //  gives the coordinates relative to the viewport in CSS pixels.
        }

        // For Pop-Up Grid

        var dropdowns = document.getElementsByClassName("hideAction");
        var i;
        for (i = 0; i < dropdowns.length; i++) {
            var openDropdown = dropdowns[i];
            if (openDropdown.classList.contains('showAction') && openDropdown.id !== id) {
                openDropdown.classList.remove('showAction');
            }
        }

        document.getElementById(id).classList.toggle("showAction");

    };

    $rootScope.scrollActionHide = function () {
        hideCustomGridActionMenu();
    };

    var hideCustomGridActionMenu = function () {
        var dropdowns = document.getElementsByClassName("hideAction");
        var i;
        for (i = 0; i < dropdowns.length; i++) {
            var openDropdown = dropdowns[i];
            if (openDropdown.classList.contains('showAction')) {
                openDropdown.classList.remove('showAction');
            }
        }
    };

    window.onscroll = function () {
        hideCustomGridActionMenu();
    };

    window.onclick = function (event) {

        if (!event.target.matches('.dropbtn')) {
            hideCustomGridActionMenu();
        }
    };
    //

    $scope.showSpinner = false;
    //$scope.spinnerMessage = 'Frayte Processing...';

    $scope.spinnerOptions = {
        radius: 40,
        lines: 8,
        length: 0,
        width: 30,
        speed: 1.7,
        corners: 1.0,
        trail: 100,
        color: '#428bca'
    };

    activate();
    function activate() { }

    $rootScope.$on('spinner.toggle', function (event, args) {
        $scope.showSpinner = args.show;
        if (args.message) {
            $scope.spinnerMessage = args.message;
        }
    });
    $rootScope.$on('spinner.toggleTemplate', function (event, args) {
        $scope.showSpinner = args.show;
        if (args.template) {
            $scope.spinnerTemplate = args.template;
        }

        $scope.spinnerMessage = args.message;

    });
    $scope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
        if (angular.isDefined(toState.data.pageTitle) && toState.data.pageTitle !== 'Home') {
            $scope.pageTitle = toState.data.pageTitle + ($rootScope.SITECOMPANY === 'MAXLOGISTIC' ? ' | MEX | ' + "Logistics" : ' | FRAYTE |  ' + "Logistics Solutions");
        }
        else if (angular.isDefined(toState.data.pageTitle) && toState.data.pageTitle === 'Home') {
            //$scope.pageTitle = toState.data.pageTitle + 
            $scope.pageTitle = ($rootScope.SITECOMPANY === 'MAXLOGISTIC' ? ' MEX Logistics' : 'FRAYTE') + ' | Air Freight | eCommerce | Courier | Sea Freight';
        }
        if (toState && toState.name) {
            if (toState.name === 'login-chTrad-hkcm') {
                SessionService.setPublicSite('http://frayte.com.hk/');
                SessionService.setLanguage('chTrad');
                $scope.changeLanguage('chTrad');
            }

            else if (toState.name === 'login-chSim-hk') {
                SessionService.setPublicSite('http://frayte.hk/');
                SessionService.setLanguage('chSim');
                $scope.changeLanguage('chSim');
            }
            else if (toState.name === 'login-chTrad-hk') {
                SessionService.setPublicSite('http://frayte.hk/');
                SessionService.setLanguage('chTrad');
                $scope.changeLanguage('chTrad');
            }
            else if (toState.name === 'login-th-th') {
                SessionService.setPublicSite('http://frayte.th/');
                SessionService.setLanguage('th');
                $scope.changeLanguage('th');
            }
            else {
                if (toState.name === 'login-chTrad') {
                    SessionService.setLanguage('chTrad');
                    $scope.changeLanguage('chTrad');
                }
                else if (toState.name === 'login-chSim') {
                    SessionService.setLanguage('chSim');
                    $scope.changeLanguage('chSim');
                }
                else if (toState.name === 'login-th') {
                    SessionService.setLanguage('th');
                    $scope.changeLanguage('th');
                }
                else if (toState.name === 'en') {
                    SessionService.setLanguage('en');
                    $scope.changeLanguage('en');
                }
            }

        }

    });
    $scope.Data = {
        selectOption: 'en'
    };

    $scope.changeLanguage = function (langKey) {

        SessionService.setLanguage(langKey);
        $translate.use(langKey);

        if (langKey === "en") {
            $scope.Data = {
                selectOption: 'en'
            };
            $scope.isDisabled1 = true;
            $scope.isDisabled2 = false;
            $scope.isDisabled3 = false;
            $scope.isDisabled4 = false;
        }
        if (langKey === "chSim") {
            $scope.Data = {
                selectOption: 'chSim'
            };
            $scope.isDisabled1 = false;
            $scope.isDisabled2 = true;
            $scope.isDisabled3 = false;
            $scope.isDisabled4 = false;
        }
        if (langKey === "chTrad") {
            $scope.Data = {
                selectOption: 'chTrad'
            };
            $scope.isDisabled1 = false;
            $scope.isDisabled2 = false;
            $scope.isDisabled3 = true;
            $scope.isDisabled4 = false;
        }
        if (langKey === "th") {
            $scope.Data = {
                selectOption: 'th'
            };
            $scope.isDisabled1 = false;
            $scope.isDisabled2 = false;
            $scope.isDisabled3 = false;
            $scope.isDisabled4 = true;
        }

        // Need to show Module Name  Multilingual
        if (SessionService.getModuleType()) {
            $rootScope.setModuleType(SessionService.getModuleType());
        }

        //Reload/Refresh the screen

        //$state.reload();
    };

    //$rootScope.reslength = false;
    //$rootScope.val = true;
    $scope.ImagePath = config.BUILD_URL;
    $scope.IsStatic = config.IS_STATIC;
    $scope.IsSiteCountry = config.SITE_COUNTRY;
    $scope.Site_Company = config.SITE_COMPANY;
    $rootScope.SITECOMPANY = config.SITE_COMPANY;
    var getScreenInitials = function () {
        var userInfo = SessionService.getUser();
        if (userInfo) {
            UtilityService.GetUnmanifestedJobCount(userInfo.EmployeeId).then(function (respone) {
                if (respone.status === 200) {
                    if (respone.data) {
                        $rootScope.unmanifestedCount = respone.data;
                    }
                    else {
                        $rootScope.unmanifestedCount = 0;
                    }
                }
                else {
                    $rootScope.unmanifestedCount = 0;
                }

            }, function (error) {
                $scope.unmanifestedCount = 0;
            });
        }
    };
    $scope.callAtInterval = function () {
     //   getScreenInitials();
    };
    function init() {

        var userInfo = SessionService.getUser();

        $scope.promise = $interval(function () { $scope.callAtInterval(); }, 50009000);

        var language = SessionService.getLanguage();
        $translate.use(language);

        if (language === "en") {
            $scope.Data = {
                selectOption: 'en'
            };
            $scope.isDisabled1 = true;
            $scope.isDisabled2 = false;
            $scope.isDisabled3 = false;
            $scope.isDisabled4 = false;
        }
        if (language === "chSim") {
            $scope.Data = {
                selectOption: 'chSim'
            };
            $scope.isDisabled1 = false;
            $scope.isDisabled2 = true;
            $scope.isDisabled3 = false;
            $scope.isDisabled4 = false;
        }
        if (language === "chTrad") {
            $scope.Data = {
                selectOption: 'chTrad'
            };
            $scope.isDisabled1 = false;
            $scope.isDisabled2 = false;
            $scope.isDisabled3 = true;
            $scope.isDisabled4 = false;
        }
        if (language === 'th') {
            $scope.Data = {
                selectOption: 'th'
            };
            $scope.isDisabled1 = false;
            $scope.isDisabled2 = false;
            $scope.isDisabled3 = false;
            $scope.isDisabled4 = true;
        }
    }
    init();
});

//slider module
//angular.module('ui.bootstrap.carousel', ['ui.bootstrap.transition'])
//.controller('CarouselController', ['$scope', '$timeout', '$transition', '$q', function ($scope, $timeout, $transition, $q) {
//}]).directive('carousel', [function () {
//    return { 
//    };
//}]);
