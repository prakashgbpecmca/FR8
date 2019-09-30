angular.module("ngApp.security")
        .config(function ($httpProvider) {
            $httpProvider.interceptors.push("AddTokenService");
        })
       .factory("AddTokenService", function ($injector,$q) {

           var currentUserService = $injector.get("CurrentUserService");
           var request = function (config) {
               if (currentUserService.profile.token) {
                   config.headers.Authorization = "Bearer " + currentUserService.profile.token;
               } 
               return $q.when(config);
           };

           return {
               request: request
           };

       });