

angular.module('ngApp', [
  'templates-app',
  'templates-common',
  'ui.bootstrap',
  'ngApp.home',
  'ngApp.utility',
  'ngApp.common',
  'ngApp.public',
  'ngApp.payment',
  'ui.router',
  'ngApp.setting',
  'ngApp.systemAlert',
  'toaster',
  'ngFileUpload',
   'ui.mask',
  'angular-loading-bar',
  'cfp.loadingBar',
  'uiGmapgoogle-maps',
  'pascalprecht.translate',
  'ngSanitize'
])

//Web Api Service URL

    .constant('config', {
        'SERVICE_URL': 'http://localhost:24047/api',
        'BUILD_URL': '../build/assets/',
        // 'BUILD_URL': '//localhost:62992/build/assets/',
        'IS_STATIC': 'False',
        'SITE_COUNTRY': 'CO.TH',
        'Payment_Link': 'http://localhost:16257/build/',
        'Public_Link': 'http://localhost:63124/',
        'App_Link': 'http://localhost:62992/',
        'Stripe_PaymentKey': 'pk_test_oSsZz9lGM9QdWxqsoDK7442m',
        'Paypal_PaymentKey_Sandbox': 'AZAi_j8QZo_B7jczaUrwCHrwScckYnJPD2TidtYmSl_0orNL0nCBI3dwcrQdmp_7ze_CSTDT8CjHvFxJ',
        'Paypal_PaymentKey_Production': 'AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN', //AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN
        'Paypal_PaymentMode': "sandbox" // Optional: specify 'sandbox' or 'production' environment
    })


//.constant('config', {
//    'SERVICE_URL': 'https://app.frayte.com/WebAPI/api',
//    'BUILD_URL': '../assets/',
//    'IS_STATIC': 'False',
//    'SITE_COUNTRY': 'CO.TH',
//    'Payment_Link': 'http://payment.frayte.com',
//    'Public_Link': 'http://frayte.co.th/',
//    'App_Link': 'https://app.frayte.com/',
//    'Stripe_PaymentKey': 'pk_test_oSsZz9lGM9QdWxqsoDK7442m',
//    'Paypal_PaymentKey_Sandbox': 'AZAi_j8QZo_B7jczaUrwCHrwScckYnJPD2TidtYmSl_0orNL0nCBI3dwcrQdmp_7ze_CSTDT8CjHvFxJ',
//    'Paypal_PaymentKey_Production': 'AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN', //AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN
//    'Paypal_PaymentMode': "sandbox" // Optional: specify 'sandbox' or 'production' environment
//})


//.constant('config', {
//    'SERVICE_URL': 'https://app.frayte.co.uk/WebAPI/api', 
//     'BUILD_URL': '//frayte.co.uk/assets/',  
//    'IS_STATIC': 'False',
//    'SITE_COUNTRY': 'CO.UK',
//    'Payment_Link': 'http://payment.frayte.co.uk',
//    'Public_Link': 'https://frayte.co.uk/',
//    'App_Link': 'https://app.frayte.co.uk/',
//    'Stripe_PaymentKey': 'pk_test_oSsZz9lGM9QdWxqsoDK7442m',
//    'Paypal_PaymentKey_Sandbox': 'AZAi_j8QZo_B7jczaUrwCHrwScckYnJPD2TidtYmSl_0orNL0nCBI3dwcrQdmp_7ze_CSTDT8CjHvFxJ',
//    'Paypal_PaymentKey_Production': 'AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN', //AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN
//    'Paypal_PaymentMode': "sandbox" // Optional: specify 'sandbox' or 'production' environment
//})

//    .constant('config', {
//    'SERVICE_URL': 'http://app.frayte.com/WebAPI/api', 'BUILD_URL': '../assets/',
//    'IS_STATIC': 'False', 'SITE_COUNTRY': 'COM',
//    'Payment_Link': 'http://payment.frayte.com',
//    'Public_Link': 'http://frayte.com/',
//    'App_Link': 'http://app.frayte.com/',
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
//    'Payment_Link': 'http://payment.godemowithus.com',
//    'Public_Link': 'http://fraytepublic.godemowithus.com/',
//    'App_Link': 'http://frayte.godemowithus.com/',
//    'Stripe_PaymentKey': 'pk_test_oSsZz9lGM9QdWxqsoDK7442m',
//    'Paypal_PaymentKey_Sandbox': 'AZAi_j8QZo_B7jczaUrwCHrwScckYnJPD2TidtYmSl_0orNL0nCBI3dwcrQdmp_7ze_CSTDT8CjHvFxJ',
//    'Paypal_PaymentKey_Production': 'AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN', //AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN
//    'Paypal_PaymentMode': "sandbox" // Optional: specify 'sandbox' or 'production' environment
//})

//.constant('config', {
//    'SERVICE_URL': 'https://app.frayte.com/WebAPI/api',
//    'BUILD_URL': '../assets/',
//    'IS_STATIC': 'False',
//    'SITE_COUNTRY': 'COM',
//    'Payment_Link': 'http://payment.frayte.com',
//    'Public_Link': 'https://frayte.com/',
//    'App_Link': 'https://app.frayte.com/',
//    'Stripe_PaymentKey': 'pk_test_oSsZz9lGM9QdWxqsoDK7442m',
//    'Paypal_PaymentKey_Sandbox': 'AZAi_j8QZo_B7jczaUrwCHrwScckYnJPD2TidtYmSl_0orNL0nCBI3dwcrQdmp_7ze_CSTDT8CjHvFxJ',
//    'Paypal_PaymentKey_Production': 'AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN', //AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN
//    'Paypal_PaymentMode': "sandbox" // Optional: specify 'sandbox' or 'production' environment
//})

//   .constant('config', {
//    'SERVICE_URL': 'http://app.godemowithus.com/WebAPI/api',
//    'BUILD_URL': '../assets/',
//    'IS_STATIC': 'False',
//    'SITE_COUNTRY': 'CO.UK',
//    'Payment_Link': 'http://payment.godemowithus.com',
//    'Public_Link': 'http://public.godemowithus.com/',
//    'App_Link': 'http://app.godemowithus.com/',
//    'Stripe_PaymentKey': 'pk_test_oSsZz9lGM9QdWxqsoDK7442m',
//    'Paypal_PaymentKey_Sandbox': 'AZAi_j8QZo_B7jczaUrwCHrwScckYnJPD2TidtYmSl_0orNL0nCBI3dwcrQdmp_7ze_CSTDT8CjHvFxJ',
//    'Paypal_PaymentKey_Production': 'AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN', //AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN
//    'Paypal_PaymentMode': "sandbox" // Optional: specify 'sandbox' or 'production' environment
//})

//.constant('config', {
//'SERVICE_URL': 'http://tradelane.godemowithus.com/WebAPI/api',
//'BUILD_URL': '../assets/',
//'IS_STATIC': 'False',
//'SITE_COUNTRY': 'CO.UK',
//'Payment_Link': 'http://payment.godemowithus.com',
//'Public_Link': 'http://tradelane-public.godemowithus.com/',
//'App_Link': 'http://tradelane.godemowithus.com/',
//'Stripe_PaymentKey': 'pk_test_oSsZz9lGM9QdWxqsoDK7442m',
//'Paypal_PaymentKey_Sandbox': 'AZAi_j8QZo_B7jczaUrwCHrwScckYnJPD2TidtYmSl_0orNL0nCBI3dwcrQdmp_7ze_CSTDT8CjHvFxJ',
//'Paypal_PaymentKey_Production': 'AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN', //AeLGroOx_Ul4QisTJIyhgjWdC0XQsQaHXAFfzwVyidfYbR4xSNOLNj_EchCG0HnhntC2XjWnZhnPpbfN
//'Paypal_PaymentMode': "sandbox" // Optional: specify 'sandbox' or 'production' environment
//})

//.config(function myAppConfig($stateProvider, $urlRouterProvider, $httpProvider) {
.config(function myAppConfig($stateProvider, $urlRouterProvider, $httpProvider, $locationProvider) {
    $locationProvider.html5Mode(true);
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

            //toaster.pop({
            //    type: 'error',
            //    title: 'Frayte-Error',
            //    body: 'An error occurred, please check the console for detail',
            //    showCloseButton: true
            //});
        };
    });
})
    .run(function run($window, $rootScope) {

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
.config(['$translateProvider', function ($translateProvider) {
    $translateProvider.useStaticFilesLoader({
        prefix: '../downloadform/locale-',
        suffix: '.json'
    });
    $translateProvider.preferredLanguage('en');
    //  $translateProvider.useSanitizeValueStrategy('sanitize');
}])
.controller('AppCtrl', function AppCtrl($scope, config, $location, $http, $log, $rootScope, $translate, SessionService, $state) {
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
        var language = "";
        if ($location.$$absUrl.search('frayte.hk') > -1 || $location.$$absUrl.search('frayte.com.hk') > -1) {
            language = "chTrad";
        }
        else if ($location.$$absUrl.search('frayte.th') > -1) {
            language = "th";
        }
        else {
            language = SessionService.getLanguage();
        }

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
        if (language === "th") {
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
