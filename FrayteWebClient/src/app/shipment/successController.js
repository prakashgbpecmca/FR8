angular.module('ngApp.shipment').controller('SuccessController', function ($scope, $location, $state, $filter, config, ShipmentService, TimeZoneService, CountryService, CourierService, HomeService, SessionService, $uibModal, $log, toaster, $stateParams, shipmentChild, userType, pdfPath) {

    $scope.shipmentChild = shipmentChild;
    $scope.FrayteUserType = {
        UserType: userType
    };

    function init() {
        $scope.ImagePath = config.BUILD_URL;
        $scope.pdfPath = pdfPath;
    }

    init();

});