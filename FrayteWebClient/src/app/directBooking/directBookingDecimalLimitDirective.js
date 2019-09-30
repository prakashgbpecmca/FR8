angular.module("ngApp.directBooking")
.directive('onlyDecimalLimit', function () {
    return {
        require: 'ngModel',
        link: function (scope) {
            scope.$watch('Package.Weight', function (newValue, oldValue) {
                var arr = String(newValue).split("");
                if (newValue === undefined) { return; }
                if (newValue === null) { return; }
                if (arr.length === 0) { return; }
                if (arr.length === 1 && (arr[0] === '.')) { return; }
                if (arr.length === 2 && newValue === '-.') { return; }
                if (newValue !== undefined && isNaN(newValue)) {
                    scope.Package.Weight = oldValue;
                }
                else {
                    if (parseFloat(newValue) > 99999) {

                        scope.Package.Weight = parseFloat(oldValue).toFixed(1);
                    }
                    else {
                        var data = newValue.toString().split('.');
                        if (data.length > 1 && data[1] !== '' && data[1].length > 1) {
                            scope.Package.Weight = parseFloat(oldValue).toFixed(1);
                        }
                        //  scope.Package.Weight = parseFloat(newValue).toFixed(1);
                    }
                }
            });
        }
    };
});