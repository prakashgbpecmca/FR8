(function (module) {

    var localStorage = function ($window) {

    //    var store = $window.localStorage;
        var add = function (key, value) {
          //  value = angular.toJson(value);
            if (value) {
                $window.sessionStorage[key] = JSON.stringify(value);
                //store.setItem(key, value);
            }
        };

        var get = function (key) {
         //   var value = store.getItem(key);
          //  if (value) {
                return $window.sessionStorage[key] ? JSON.parse($window.sessionStorage[key]) : undefined;
                //value = angular.fromJson(value);
          //  }

           // return value;
        };

        var remove = function (key) {
            $window.sessionStorage.removeItem(key);
           // store.removeItem(key);
        };

        return {
            add: add,
            get: get,
            remove: remove
        };


    };

    module.factory("localStorage", localStorage);

}(angular.module("ngApp.security")));