angular.module("ngApp.security")
       .config(function ($httpProvider) {
           $httpProvider.interceptors.push("LoginRedirectService");
       })
       .factory("LoginRedirectService", function ($q, $location, CurrentUserService, $rootScope) {
           var lastPath = "/";
           var token = CurrentUserService.profile;
           var responseError = function (response) {
               if (response.status === 401) {
                   if (!token || !token.token) {
                       lastPath = $location.path();
                       $location.path("/login");
                   }
                   else {
                       // need to show un authorize page
                       $rootScope.unAuthorize = true; 
                   }
               }
               else {
                   $rootScope.unAuthorize = false;
               }

               return $q.reject(response);
           };
           var redirectPostLogin = function () {
               $location.path(lastPath);
               lastPath = "/";
           };
           var response = function () {
               $rootScope.unAuthorize = false;
           };
           return {
               responseError: responseError,
           //    response: response,
               redirectPostLogin: redirectPostLogin
           };

       });