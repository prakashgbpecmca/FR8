///
/// Oauth service is responsible for getting token and storing it in local storage
///

angular.module("ngApp.security")
       .factory("OauthService", function (config, $injector, formEncodeService, CurrentUserService) {

           var http = $injector.get("$http");

           var getToken = function (username, password) {
               // In reqest header must provide the  Content-Type as "application/x-www-form-urlencode"
               var Config = {
                   headers: {
                       "Content-Type": "application/x-www-form-urlencoded"
                   }
               };

               // data must be url encode before sending it to server
               var data = formEncodeService({
                   username: username,
                   password: password,
                   grant_type: "password" // must set grant_type to 'password' otherwise it won't work
               });

               return http.post(config.SERVICE_URL + "/token", data, Config);
           };

           return {
               oauthToken: getToken
           };
       });