angular.module('ngApp.common').directive('fourdigittwodecimalplace', [function () {
    return {
        require: '?ngModel',
        link: function ($scope, element, $attrs, ngModelCtrl) {
            
            if (!ngModelCtrl) {
                return;
            }

            ngModelCtrl.$parsers.push(function (val) {
                if (angular.isUndefined(val)) {
                    val = '';
                }

                var clean = val.replace(/[^-0-9\.]/g, '');
                var negativeCheck = clean.split('-');
                var decimalCheck = clean.split('.');
                if (!angular.isUndefined(negativeCheck[1])) {
                    negativeCheck[1] = negativeCheck[1].slice(0, negativeCheck[1].length);
                    clean = negativeCheck[0] + '-' + negativeCheck[1];
                    if (negativeCheck[0].length > 0) {
                        clean = negativeCheck[0];
                    }
                }

                if (!angular.isUndefined(decimalCheck[0])) {
                    if (decimalCheck[0].length > 4) {
                        decimalCheck[0] = decimalCheck[0].slice(0, 4);
                        clean = decimalCheck[0];
                    }
                }

                if (!angular.isUndefined(decimalCheck[0]) || !angular.isUndefined(decimalCheck[1])) {
                    if (decimalCheck[0].length > 4) {
                        decimalCheck[0] = decimalCheck[0].slice(0, 4);
                    }
                    else {
                        if (decimalCheck[1] !== undefined && decimalCheck[1] !== null && decimalCheck[1].length > 0) {
                            decimalCheck[1] = decimalCheck[1].slice(0, 2);
                            clean = decimalCheck[0] + '.' + decimalCheck[1];
                        }
                    }
                }

                if (val !== clean) {
                    ngModelCtrl.$setViewValue(clean);
                    ngModelCtrl.$render();
                }
                return clean;
            });

            element.bind('keypress', function (event) {
                if (event.keyCode === 32) {
                    event.preventDefault();
                }
            });            
        }
    };
}]);