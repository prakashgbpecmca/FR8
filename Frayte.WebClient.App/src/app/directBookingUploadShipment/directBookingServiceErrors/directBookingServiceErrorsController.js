angular.module('ngApp.directBooking').controller('DirectBookingServiceErrorsController', function ($scope, toaster, $translate, Upload, uiGridConstants, DownloadExcelService, config, $state, SessionService, UploadShipmentService, $uibModal, ShipmentData, $uibModalInstance, $rootScope, DbUploadShipmentService) {

    // set translation key
    var setMultilingualOptions = function () {
        $translate(['FrayteWarning', 'Frayte_Success', 'FrayteError', 'PleaseFillMandatoryField', 'ShipmentSavedSuccessfully']).then(function (translations) {

            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.Frayte_Success = translations.Frayte_Success;
            $scope.FrayteError = translations.FrayteError;
            $scope.ShipmentSavedSuccessfully = translations.ShipmentSavedSuccessfully;
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
        DbUploadShipmentService.GetErrorDetail(Shipment.ShipmentId, "DirectBooking_SS").then(function (Response) {
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
            if (directbooking[0].CollectionTime !== undefined && directbooking[0].CollectionTime !== null && directbooking[0].CollectionTime !== "") {
                var str = '';
                str = directbooking[0].CollectionTime.split(':');
                if (str.length > 0) {
                    directbooking[0].CollectionTime = str[0] + str[1];
                }
                
            }
            $scope.vk = directbooking[0];
            $scope.vk.SessionId = ShipmentData.SessionId > 0 ? ShipmentData.SessionId : 0;

            DbUploadShipmentService.SaveShipmentFromErrorPopupSelectService($scope.vk).then(function (response) {
                var ab = response.data;
                toaster.pop({
                    type: 'success',
                    title: $scope.Frayte_Success,
                    body: $scope.ShipmentSavedSuccessfully,
                    showCloseButton: true
                });
                $uibModalInstance.close('testParameter');

            }, function () {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarning,
                    body: $scope.PleaseFillMandatoryField,
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.PleaseFillMandatoryField,
                showCloseButton: true
            });
        }
    };

    $scope.Makeshipment = function (gridDirectBooking) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBookingUploadShipment/directBookingForm/directBookingForm.tpl.html',
            controller: 'DirectBookingFormController',
            windowClass: 'DirectBookingService',
            size: 'lg',
            backdrop: true,
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
            
        }, function () {
        });
    };
    $scope.SaveUploadShipmentWithService = function (gridDirectBooking) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBookingUploadShipment/directBookingForm/directBookingForm.tpl.html',
            controller: 'DirectBookingFormController',
            windowClass: 'DirectBookingService',
            backdrop: true,
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

        }, function () {
        });
    };
    function init() {
        $rootScope.FormName = "WithServiceForm";
        $scope.ShipmentData = ShipmentData;
        $scope.GetErrorsForWithServiceShipments($scope.ShipmentData);
        $scope.submitted = true;
        $scope.status = {
            opened: false
        };

        $scope.dateOptions = {
            formatYear: 'yy',
            startingDay: 1,
            minDate: new Date(),
            dateDisabled: function (data) {
                var date = data.date,
                mode = data.mode;
                return mode === 'day' && (date.getDay() === 0 || date.getDay() === 6);
            }
        };
        setMultilingualOptions();
    }

    init();

});