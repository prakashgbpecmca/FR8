angular.module('ngApp.directBooking').controller('UploadShipmentGuideLineController', function ($scope, DirectBookingService, SessionService, TopCountryService, TopCurrencyService, DbUploadShipmentService, IsCollapedFillExcel) {
 
    function init() {

        var userInfo = SessionService.getUser();
        $scope.RoleId = userInfo.RoleId;
        $scope.CustomerId = userInfo.EmployeeId;
        $scope.OperationZoneId = userInfo.OperationZoneId;
        $scope.ModuleType = "Ecommerce";
        DirectBookingService.GetInitials($scope.CustomerId).then(function (response) {
           
            // Set Country type according to given order
            $scope.CountriesRepo = response.data.Countries;
            // Set Currency type according to given order
            $scope.CurrencyTypes = response.data.CurrencyTypes;
            $scope.ParcelTypes = response.data.ParcelTypes;
            $scope.ShipmentMethods = response.data.ShipmentMethods;
            $scope.CustomerDetail = response.data.CustomerDetail;
        });
        DbUploadShipmentService.GetLogisticServiceCode($scope.OperationZoneId, $scope.CustomerId).then(function (response) {
            if (response.data.length > 0) {
                $scope.ServcieCodeDetail = response.data;
            }
        });
        if (!IsCollapedFillExcel) {
            $scope.isCollapsed7 = IsCollapedFillExcel;
        }
        else {
            $scope.isCollapsed7 = true;
        }
        $scope.GuideLineFillExcel = [
   { "name": "From Country" },
   { "name": "FromCountryCode" },
   { "name": "FromPostCode" },
   { "name": "FromContactFirstName" },
   { "name": "FromContactLastName" },
   { "name": "FromCompanyName" },
   { "name": "FromAddress1" },
   { "name": "FromAddress2" },
   { "name": "FromCity" },
   { "name": "FromTelephoneNo" },
   { "name": "FromEmail" },
   { "name": "ToCountryCode" },
   { "name": "ToPostCode" },
   { "name": "ToContactFirstName" },
   { "name": "ToContactLastName" },
   { "name": "ToCompanyName" },
   { "name": "ToAddress1" },
   { "name": "ToAddress2" },
   { "name": "ToCity" },
   { "name": "ToTelephoneNo" },
   { "name": "ToEmail" },
   { "name": "PackageCalculationType" },
   { "name": "ParcelType" },
   { "name": "Currency" },
   { "name": "ShipmentReference" },
   { "name": "ShipmentDescription" },
   { "name": "TrackingNo" },
   { "name": "CourierCompany" },
   { "name": "CartonValue" },
   { "name": "Length" },
   { "name": "Width" },
   { "name": "Height" },
   { "name": "Weight" },
   { "name": "DeclaredValue" },
   { "name": "ShipmentContents" }
        ];
        
    }

    init();

});