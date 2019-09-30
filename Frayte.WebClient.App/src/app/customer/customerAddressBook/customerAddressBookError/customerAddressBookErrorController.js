angular.module('ngApp.AddressBook').controller('CustomerAddressBookErrorController', function ($scope, ErrorData) {

    function init() {
        $scope.Errors = ErrorData;
    }

    init();
});