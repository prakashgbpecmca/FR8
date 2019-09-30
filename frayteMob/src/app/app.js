

angular.module('ngApp', [
  'templates-app',
  'templates-common',
  'ui.bootstrap',
  'ngApp.home',
  'ui.router',
  'toaster',
  'ngFileUpload',
  'ui.mask',
  'angular-loading-bar',
  'cfp.loadingBar',
  'uiGmapgoogle-maps',
  'pascalprecht.translate',
  'ngSanitize',
  'ngApp.login'
])

     .constant('config', {
         'SERVICE_URL': 'http://localhost:56359/api/',
         'IMAGE_URL': 'http://localhost:56359/Images/',
         'BUILD_URL': '../build/assets/'
     })

.config(function myAppConfig($stateProvider, $urlRouterProvider, $httpProvider, $locationProvider) { 
        $urlRouterProvider.otherwise('/');
    })
.config(['cfpLoadingBarProvider', function (cfpLoadingBarProvider) {
       cfpLoadingBarProvider.includeSpinner = false;
   }])
.run(function ($window, SessionService) {
    window.onbeforeunload = function (evt) {
        SessionService.windowUnload(evt);
    };
})

.controller('AppCtrl', function AppCtrl($scope, config) {



    $scope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
        if (angular.isDefined(toState.data.pageTitle) && toState.data.pageTitle !== 'Home') {
            $scope.pageTitle = toState.data.pageTitle + ' | FRAYTE';
        }
        else if (angular.isDefined(toState.data.pageTitle) && toState.data.pageTitle === 'Home') {
            $scope.pageTitle = 'FRAYTE';
        }
    });
    $scope.ImagePath = config.BUILD_URL;

    function init() {
        $scope.ImagePath = config.BUILD_URL;
    }

    init();
});
