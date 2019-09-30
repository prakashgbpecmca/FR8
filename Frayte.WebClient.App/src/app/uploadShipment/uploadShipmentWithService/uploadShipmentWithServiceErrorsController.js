angular.module('ngApp.uploadShipment').controller('WithServiceErrorsController', function ($scope, toaster, $translate, Upload, uiGridConstants, DownloadExcelService, config, $state, SessionService, UploadShipmentService, $uibModal, ShipmentData, $uibModalInstance, $rootScope) {
    

    //Set Multilingual for Modal Popup
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'FrayteSuccess', 'Shipment_Created_Successfully', 'PleaseFillMandatoryField']).then(function (translations) {

            $scope.FrayteError = translations.FrayteError;
            $scope.FrayteInformation = translations.FrayteInformation;
            $scope.FrayteValidation = translations.FrayteValidation;
            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.Shipment_Created_Successfully = translations.Shipment_Created_Successfully;
            $scope.PleaseFillMandatoryField = translations.PleaseFillMandatoryField;
        });
    };





    $scope.OpenCalender = function ($event) {
        $scope.status.opened = true;
    };

    $scope.status = {
        opened: false
    };
    $scope.GetErrorsForWithServiceShipments = function (Shipment) {
        UploadShipmentService.GetErrorDetail(Shipment.ShipmentId, "ECOMMERCE_SS").then(function (Response) {
            $scope.ErrorList = Response.data;
            $scope.Shipment = Response.data;
            for (i = 0; i < $scope.ErrorList[0].Errors.length; i++) {
                if ($scope.ErrorList[0].Errors[i].includes("CartonValue") ||
                    $scope.ErrorList[0].Errors[i].includes("Length") ||
                    $scope.ErrorList[0].Errors[i].includes("Width") ||
                    $scope.ErrorList[0].Errors[i].includes("Height") ||
                    $scope.ErrorList[0].Errors[i].includes("Weight") ||
                    $scope.ErrorList[0].Errors[i].includes("DeclaredValue") ||
                    $scope.ErrorList[0].Errors[i].includes("ShipmentContents")) {
                    $scope.ErrorFormShow = true;
                    break;
                }
                else {
                    $scope.ErrorFormShow = false;
                }
            }
        },
        function () {

        });
    };

    $scope.Shipment1 = function (DirectBooking, directbooking) {
        if (DirectBooking.$valid) {
            if (directbooking[0] != null && directbooking[0].PakageCalculatonType != null) {
                directbooking[0].PackageCalculationType = directbooking[0].PakageCalculatonType;
            }
            $scope.vk = directbooking[0];
            UploadShipmentService.SaveShipmentFromErrorPopupSelectService($scope.vk).then(function (response) {
                var ab = response.data;
                toaster.pop({
                    type: 'success',
                    title: $scope.FrayteSuccess,
                    body: $scope.Shipment_Created_Successfully,
                    showCloseButton: true
                });
                $uibModalInstance.close('testParameter');

            }, function () {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteValidation,
                    body: $scope.PleaseFillMandatoryField,
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteValidation,
                body: $scope.PleaseFillMandatoryField,
                showCloseButton: true
            });
        }
    };

    $scope.SaveUploadShipmentWithService = function (gridDirectBooking) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'uploadShipment/uploadShipmentWithServiceEcommForm/uploadShipmentWithServiceForm.tpl.html',
            controller: 'uploadeCommerceWithServiceController',
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            resolve: {
                ShipmentId: function () {
                    return gridDirectBooking.ShipmentId;
                },
                TrackingNo: function () {
                    return gridDirectBooking.TrackingNo;
                },
                CourierCompany: function () {
                    return gridDirectBooking.DisplayName;
                }
            }
        });
        modalInstance.result.then(function () {
            $uibModalInstance.close('testParameter');
        }, function () {
        });
    };

    function init() {
        $rootScope.FormName = "WithServiceForm";
        $scope.ShipmentData = ShipmentData;
        $scope.GetErrorsForWithServiceShipments($scope.ShipmentData);
        $scope.submitted = true;
        setMultilingualOptions();
        $scope.status = {
            opened: false
        };

        $scope.dateOptions = {
            formatYear: 'yy',
            startingDay: 1,
            minDate: new Date()
        };
    }

    init();

});