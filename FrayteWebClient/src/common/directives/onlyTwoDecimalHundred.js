angular.module('ngApp.common').directive('sbMaxPrecision', function () {
    return {
        
        require: 'ngModel',
        link: function (scope, element, attributes, ngModel) {


            function maxPrecision(value) {
                if (!isNaN(value)) {
                    var validity = countDecimalPlaces(value) <= attributes.sbMaxPrecision;
                    ngModel.$setValidity('max-precision', validity);

                }
                if (value >= 99.99) {
                    
                }

                return value;
            }
            function countDecimalPlaces(value) {
                var decimalPos = String(value).indexOf('.');
                if (decimalPos === -1) {
                    return 0;
                } else {
                    return String(value).length - decimalPos - 1;
                }
            }

            ngModel.$parsers.push(maxPrecision);
            ngModel.$formatters.push(maxPrecision);
        }

    };
});