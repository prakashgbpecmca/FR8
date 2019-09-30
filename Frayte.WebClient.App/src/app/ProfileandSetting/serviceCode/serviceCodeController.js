angular.module('ngApp.servicecode').controller('ServiceCodeController', function ($scope, SessionService, DbUploadShipmentService, CustomerService) {

    var serviceCodeJson = function () {

        CustomerService.GetLogisticServices($scope.OperationZoneId, $scope.RoleId, $scope.CreatedBy).then(function (response) {
            $scope.logisticServices = response.data;
            $scope.ServiceCodeArray = [];
            $scope.ServiceCodeFinalArray = [];
            var ServiceCodeObj = {};
            $scope.ServcieCodeJsonObject = {
                LogisticCompany: "",
                ServiceCodeArray: []
            };
            for (j = 0; j < $scope.logisticServices.length; j++) {
                if ($scope.logisticServices[j].RateType !== null) {
                    $scope.ServcieCodeJsonObject.LogisticCompany = $scope.logisticServices[j].LogisticCompany + " " + $scope.logisticServices[j].LogisticType + " " + $scope.logisticServices[j].RateType;
                }
                else {
                    $scope.ServcieCodeJsonObject.LogisticCompany = $scope.logisticServices[j].LogisticCompany + " " + $scope.logisticServices[j].LogisticTypeDisplay;
                }
                
                for (i = 0; i < $scope.ServcieCodeDetail.length; i++) {
                    if ($scope.ServcieCodeDetail[i].LogisticCompany === $scope.logisticServices[j].LogisticCompany && $scope.ServcieCodeDetail[i].LogisticType === $scope.logisticServices[j].LogisticType && (($scope.ServcieCodeDetail[i].RateType !== null && $scope.ServcieCodeDetail[i].RateType !== "" && $scope.logisticServices[j].RateType !== null && $scope.logisticServices[j].RateType !== "") && $scope.ServcieCodeDetail[i].RateType === $scope.logisticServices[j].RateType)) {
                        ServiceCodeObj.ServiceCode = $scope.ServcieCodeDetail[i].ServiceCode;
                        ServiceCodeObj.LogisticDescription = $scope.ServcieCodeDetail[i].LogisticDescription;
                        ServiceCodeObj.WeightFrom = $scope.ServcieCodeDetail[i].WeightFrom;
                        ServiceCodeObj.WeightTo = $scope.ServcieCodeDetail[i].WeightTo;
                        $scope.ServcieCodeJsonObject.ServiceCodeArray.push(ServiceCodeObj);
                        ServiceCodeObj = {};
                    }
                    if ($scope.ServcieCodeDetail[i].LogisticCompany === $scope.logisticServices[j].LogisticCompany && $scope.ServcieCodeDetail[i].LogisticType === $scope.logisticServices[j].LogisticType && $scope.logisticServices[j].RateType === null) {
                        ServiceCodeObj.ServiceCode = $scope.ServcieCodeDetail[i].ServiceCode;
                        ServiceCodeObj.LogisticDescription = $scope.ServcieCodeDetail[i].LogisticDescription;
                        ServiceCodeObj.WeightFrom = $scope.ServcieCodeDetail[i].WeightFrom;
                        ServiceCodeObj.WeightTo = $scope.ServcieCodeDetail[i].WeightTo;
                        $scope.ServcieCodeJsonObject.ServiceCodeArray.push(ServiceCodeObj);
                        ServiceCodeObj = {};
                    }
                }
                if ($scope.ServcieCodeJsonObject.ServiceCodeArray.length > 0) {
                    $scope.ServiceCodeFinalArray.push($scope.ServcieCodeJsonObject);
                    $scope.ServcieCodeJsonObject = {
                        LogisticCompany: "",
                        ServiceCodeArray: []
                    };
                }
            }
            $scope.ServiceCodeFinalArray = $scope.ServiceCodeFinalArray.sort(function (a, b) {
                var x = a.LogisticCompany.toLowerCase();
                var y = b.LogisticCompany.toLowerCase();
                if (x < y) { return -1; }
                if (x > y) { return 1; }
                return 0;
            });
        });

    };
    $scope.GetServices = function () {
        var userInfo = SessionService.getUser();
        $scope.CustomerId = userInfo.EmployeeId;
        $scope.RoleId = userInfo.RoleId;
        $scope.CreatedBy = userInfo.EmployeeId;
        $scope.OperationZoneId = userInfo.OperationZoneId;
        DbUploadShipmentService.GetLogisticServiceCode($scope.OperationZoneId, $scope.CustomerId).then(function (response) {
            if (response.data.length > 0) {
                $scope.ServcieCodeDetail = response.data;
                serviceCodeJson();
            }
        });

    };
    function init() {
        $scope.GetServices();
    }
    init();
});