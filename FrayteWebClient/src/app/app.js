

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
  'ngApp.timezone',
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
  'ngApp.profile',
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
  'ngApp.payment',
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
  'ngApp.thirdPartyMatrix',
  'ngApp.baseRateCard',
  'ngApp.zonePostCode',
  'ngApp.CountryZonePostCode',
  'ngApp.directBooking',
  'ngApp.customerSetting',
  'ngApp.AddressBook',
  'ngApp.bookingHome',
  'ngApp.accessLevel',
  'colorpicker.module'
])

//Web Api Service URL
//.constant('config', { 'SERVICE_URL': 'http://frayteuk.godemowithus.com/WebApi/api', 'BUILD_URL': '../assets/', 'IS_STATIC': 'False' ,'SITE_COUNTRY':'COM' })
//.constant('config', { 'SERVICE_URL': 'http://admin-pc/webapi/api', 'BUILD_URL': '../build/assets/', 'IS_STATIC': 'False' ,'SITE_COUNTRY':'COM' })
.constant('config', {
    'SERVICE_URL': 'http://localhost:24047/api', 'BUILD_URL': '../build/assets/',
    'IS_STATIC': 'False', 'SITE_COUNTRY': 'COM',
    'Stripe_PaymentKey': 'pk_test_oSsZz9lGM9QdWxqsoDK7442m',
    'Paypal_PaymentKey_Sandbox': 'AZAi_j8QZo_B7jczaUrwCHrwScckYnJPD2TidtYmSl_0orNL0nCBI3dwcrQdmp_7ze_CSTDT8CjHvFxJ',
    'Paypal_PaymentKey_Production': 'AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN', //AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN
    'Paypal_PaymentMode': "sandbox" // Optional: specify 'sandbox' or 'production' environment
})
//Live Stripe Key = "pk_live_5sqdhHj6ioDoN8Ts6oNErfn3"

//.constant('config', {
//    'SERVICE_URL': 'http://localhost:24047/api', 'BUILD_URL': '../build/assets/',
//    'IS_STATIC': 'False', 'SITE_COUNTRY': 'CO.UK',
//    'Stripe_PaymentKey': 'pk_test_oSsZz9lGM9QdWxqsoDK7442m',
//    'Paypal_PaymentKey_Sandbox': 'AbR6rmsfDGvZ_PSXOD1DbuUWRzjQh0CjjTH3Crz9efP1VuwXYxpTwYgQuwIQhmi-oaVIbNfZu1rm_a08',
//    'Paypal_PaymentKey_Production': 'AYRUTwDNJdmR_vsPjhK8XhdKkplwXWgrml7tHJqsnyjRIEK0gU55B-bq4CVEd0EkXX7005GokVN9XQuY',
//    'Paypal_PaymentMode': "sandbox" // Optional: specify 'sandbox' or 'production' environment
//})

.config(function myAppConfig($stateProvider, $urlRouterProvider, $httpProvider, $locationProvider) {
    //$locationProvider.html5Mode(true);
    $urlRouterProvider.otherwise('/home/welcome');
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

.config(['$translateProvider', function ($translateProvider) {
    $translateProvider.useStaticFilesLoader({
        prefix: '../build/downloadform/locale-',
        suffix: '.json'
    });
    $translateProvider.preferredLanguage('en');
    //  $translateProvider.useSanitizeValueStrategy('sanitize');
}])

.run(function run($window, $rootScope,SessionService, $state, $uibModal) {
    window.onbeforeunload = function (evt) {
        SessionService.windowUnload(evt);
    };
    $rootScope.directBookingChange = true;
    $rootScope.manageDirectBookingChange = true;
    $rootScope.$on('$stateChangeStart', function (e, toState, toParams, fromState, fromParams) {
        console.log("From State" + ": " + fromState.name);
        console.log("To State" + ": " + toState.name);
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

            
            if (FromState === "direct-booking" && fromState.data.IsFired && $rootScope.manageDirectBookingChange &&
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
                    $state.go(toState.name);

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

.controller('AppCtrl', function AppCtrl($scope, config, $location, $uibModal, $http, $log, $rootScope, $translate, SessionService, $state) {
    // Spinner

    /* jshint validthis:true */

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
            $scope.pageTitle = toState.data.pageTitle + ' | FRAYTE | Logistics Solutions';
        }
        else if (angular.isDefined(toState.data.pageTitle) && toState.data.pageTitle === 'Home') {
            //$scope.pageTitle = toState.data.pageTitle + 
            $scope.pageTitle = 'FRAYTE | Air Freight | eCommerce | Courier | Sea Freight';
        }
    });

    $scope.Data = {
        selectOption: 'en'
    };

    $scope.changeLanguage = function (langKey) {
        if (langKey === "en") {
            $scope.Data = {
                selectOption: 'en'
            };
            $scope.isDisabled1 = true;
            $scope.isDisabled2 = false;
            $scope.isDisabled3 = false;
        }
        if (langKey === "chSim") {
            $scope.Data = {
                selectOption: 'chSim'
            };
            $scope.isDisabled1 = false;
            $scope.isDisabled2 = true;
            $scope.isDisabled3 = false;
        }
        if (langKey === "chTrad") {
            $scope.Data = {
                selectOption: 'chTrad'
            };
            $scope.isDisabled1 = false;
            $scope.isDisabled2 = false;
            $scope.isDisabled3 = true;
        }
        SessionService.setLanguage(langKey);
        $translate.use(langKey);

        //Reload/Refresh the screen 
        $state.reload();
    };

    //$rootScope.reslength = false;
    //$rootScope.val = true;
    $scope.ImagePath = config.BUILD_URL;
    $scope.IsStatic = config.IS_STATIC;
    $scope.IsSiteCountry = config.SITE_COUNTRY;
    function init() {
        //$http.get("../build/downloadform/lan-en.txt").then(function (response) {
        //    translationsEN = response.data;
        //});
        //$http.get("../build/downloadform/lan-chTrad.txt").then(function (response) {
        //    translationsCh_Traditional = response.data;
        //});
        //$http.get("../build/downloadform/lan-chSimple.txt").then(function (response) {
        //    translationsCh_Simplified = response.data;
        //});
        var language = SessionService.getLanguage();
        $translate.use(language);

        if (language === "en") {
            $scope.Data = {
                selectOption: 'en'
            };
            $scope.isDisabled1 = true;
            $scope.isDisabled2 = false;
            $scope.isDisabled3 = false;
        }
        if (language === "chSim") {
            $scope.Data = {
                selectOption: 'chSim'
            };
            $scope.isDisabled1 = false;
            $scope.isDisabled2 = true;
            $scope.isDisabled3 = false;
        }
        if (language === "chTrad") {
            $scope.Data = {
                selectOption: 'chTrad'
            };
            $scope.isDisabled1 = false;
            $scope.isDisabled2 = false;
            $scope.isDisabled3 = true;
        }
    }

    init();
})

;

//slider module
//angular.module('ui.bootstrap.carousel', ['ui.bootstrap.transition'])
//.controller('CarouselController', ['$scope', '$timeout', '$transition', '$q', function ($scope, $timeout, $transition, $q) {
//}]).directive('carousel', [function () {
//    return {

//    };
//}]);
