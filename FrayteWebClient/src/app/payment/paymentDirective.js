angular.module('ngApp.payment')
.directive('isNumber', function () {
    return {
        require: 'ngModel',
        link: function (scope) {
            scope.$watch('Pv_Amount', function (newValue, oldValue) {
                if (newValue === undefined) { return;}
                var arr = String(newValue).split("");
                if (arr.length === 0) { return; }
                if (arr.length === 1 && (arr[0] === '.')) { return; }
                if (arr.length === 2 && newValue === '-.') { return; }
                if (isNaN(newValue)) {
                    scope.Pv_Amount = oldValue;
                }
            });
            scope.$watch('PaymentPaypal.TotalAmount', function (newValue, oldValue) {
                if (newValue === undefined) { return; }
                var arr = String(newValue).split("");
                if (arr.length === 0) { return; }
                if (arr.length === 1 && (arr[0] === '.')) { return; }
                if (arr.length === 2 && newValue === '-.') { return; }
                if (isNaN(newValue)) {
                    scope.PaymentPaypal.TotalAmount = oldValue;
                }
            });
        }
    };
})
.directive('aDisabled', function () {
    return {
        compile: function (tElement, tAttrs, transclude) {
            //Disable ngClick
            tAttrs["ngClick"] = "!(" + tAttrs["aDisabled"] + ") && (" + tAttrs["ngClick"] + ")";

            //Toggle "disabled" to class when aDisabled becomes true
            return function (scope, iElement, iAttrs) {
                scope.$watch(iAttrs["aDisabled"], function (newValue) {
                    if (newValue !== undefined) {
                        iElement.toggleClass("disabled", newValue);
                    }
                });

                //Disable href on click
                iElement.on("click", function (e) {
                    if (scope.$eval(iAttrs["aDisabled"])) {
                        e.preventDefault();
                    }
                });
            };
        }
    };
});