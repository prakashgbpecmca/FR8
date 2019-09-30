angular.module("ngApp.security")
       .factory("formEncodeService", function () {
           return function (data) {
               var pairs = [];
               for (var key in data) {
                   pairs.push(encodeURIComponent(key) + "=" + encodeURIComponent(data[key]));
               }

               return pairs.join("&").replace("/%20/g", "+");

           };
       });