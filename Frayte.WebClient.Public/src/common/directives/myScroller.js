

angular.module('ngApp.common')
    .directive('myScroller', function () {
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

    })
    .directive('onScroll', function () {
        var previousScroll = 0;
        var link = function ($scope, $element, attrs) {
            $element.bind('scroll', function (evt) {
                var currentScroll = $element.scrollTop();
                $scope.$eval(attrs["onScroll"], { $event: evt, $direct: currentScroll > previousScroll ? 1 : -1 });
                previousScroll = currentScroll;
            });
        };

        return {
            restrict: "A",
            link: link
        };

    });
