

angular.module('ngApp.express').controller('ExpressBookingEditController', function ($scope, $uibModal, Type) {




    function init() {
        if (Type) {
            if (Type === "add") {
                $scope.value = "Add";
            }
            else {
                $scope.value = "Edit";
            }
        }
    }
    init();

});