
angular.module('ngApp.common').directive('myScroller', function () {
    return {

        restrict: 'A',
        link: function (scope, elem, attrs) {
            $(elem).on('scroll', function (evt) {
         
                var elements = document.getElementsByClassName("baseratcard-overflow");
                angular.forEach(elements, function (value, key) {
                    elements[key].scrollLeft = evt.target.scrollLeft;
                });
            });
        }

    };

});