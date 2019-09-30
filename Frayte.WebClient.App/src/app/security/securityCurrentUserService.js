angular.module("ngApp.security")
       .factory("CurrentUserService", function ($window, localStorage) {
            

           var USERKEY = "utoken";

           var init = function () {
               var user = {
                   username: "",
                   token: ""
               };
                
               localUser = localStorage.get(USERKEY);
               if (localUser) {
                   user.username = localUser.username;
                   user.token = localUser.token;
               }

               return user;
           };

           var profile = init();

           var setProfile = function (username, token) {
               profile.username = username;
               profile.token = token;
               localStorage.add(USERKEY, profile);
           };

           return {
               profile: profile,
               setProfile: setProfile
           };
       });