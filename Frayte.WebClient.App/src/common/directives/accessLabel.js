angular.module('ngApp.common').directive('accessLabel', function () {
    return {

        restrict: 'A',
        link: function (scope, elem, attrs) {
            $(elem).on('scroll', function (evt) {

                var elements = document.getElementsByClassName("width80");
                angular.forEach(elements, function (value, key) {
                    elements[key].scrollLeft = evt.target.scrollLeft;
                });
            });
        }

    };

});

angular.module('ngApp.common').directive('quotationTool', function () {
    return {

        restrict: 'A',
        link: function (scope, elem, attrs) {
            $(elem).on('scroll', function (evt) {

                var elements = document.getElementsByClassName("width96");
                angular.forEach(elements, function (value, key) {
                    elements[key].scrollLeft = evt.target.scrollLeft;
                });
            });
        }

    };

});