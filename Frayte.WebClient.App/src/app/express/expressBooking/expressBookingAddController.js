
angular.module('ngApp.express').controller('ExpressBookingAddController', function ($scope, $uibModal) {

    //add product catalog code here
    $scope.editProductCatalog = function (type) {
        var modalInstance = $uibModal.open({
            templateUrl: 'express/expressBooking/expressBookingEdit.tpl.html',
            controller: 'ExpressBookingEditController',
            windowClass: '',
            size: 'lg',
            resolve: {
                Type: function () {
                    return type;
                }
            }
        });
    };
    //end


    function init() { }
    init();

});