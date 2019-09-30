
angular.module('ngApp.common')
    .directive('spinner', function ($window) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                scope.spinner = null;
                scope.$watch(attrs.spinner, function (options) {
                    if (scope.spinner) {
                        scope.spinner.stop();
                    }
                    scope.spinner = new $window.Spinner(options);
                    scope.spinner.spin(element[0]);
                }, true);
            }
        };
    })
    .directive('spinnerTemplate', function ($window) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
               // scope.spinner = null;
                scope.$watch(attrs.spinnerTemplate, function (value) {
                    //if (scope.spinner) {
                    //    scope.spinner.stop();
                    //}
                    //scope.spinner = new $window.Spinner(options);
                    //scope.spinner.spin(element[0]);
                }, true);
            }
        };
    });

