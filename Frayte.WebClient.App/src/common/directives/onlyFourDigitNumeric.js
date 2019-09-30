angular.module('ngApp.common').directive('fourDigitNumeric', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            if (!ngModelCtrl) {
                return;
            }
            ngModelCtrl.$parsers.push(function (val) {
                var clean = val.replace(/[^0-9\.]+/g, '');

                var ar = clean.split('.');
                if (clean.length >= 9) {
                    if (ar[0] > 999) {
                        ar[0] = ar[0].substring(0, ar[0].length - 1);
                        clean = ar[0] + "." + ar[1];
                    }
                    else {
                        clean = clean.substring(0, clean.length - 1);
                    }
                }
                else {
                    if (ar[0].length >= 4) {
                        ar[0] = ar[0].substring(0, ar[0].length - 1);
                        clean = ar[0];
                    }
                    if (ar[1] !== undefined && ar[1] !== "") {
                        if (ar[1].length > 4) {
                            ar[1] = ar[1].substring(0, ar[1].length - 1);
                            clean = ar[0] + "." + ar[1];
                        }
                    }
                }

                ngModelCtrl.$setViewValue(clean);
                ngModelCtrl.$render();
                return clean;
            });

            element.bind('keypress', function (event) {
                if (event.keyCode === 32) {
                    event.preventDefault();
                }
            });
        }
    };
});