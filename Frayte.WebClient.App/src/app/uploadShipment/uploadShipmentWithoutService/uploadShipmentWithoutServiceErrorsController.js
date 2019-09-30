angular.module('ngApp.uploadShipment').controller('WithoutServiceErrorsController', function ($scope, toaster, $translate, Upload, $uibModalInstance, uiGridConstants, DownloadExcelService, config, $state, SessionService, UploadShipmentService, $uibModal, ShipmentData, DirectBookingService, TopCurrencyService, $uibModalStack, $rootScope, $anchorScroll) {

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



    $scope.GetErrorsForShipments = function (Shipment) {
        UploadShipmentService.GetErrorDetail(Shipment.ShipmentId, "ECOMMERCE_WS").then(function (Response) {
            $scope.Shipment = Response.data;
            $scope.ErrorList = Response.data;
            
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

    //set scroll to click the ok button in directbooking
    $scope.setScroll = function (printSectionId) {
        $location.hash(printSectionId);
        $anchorScroll();
    };

    $scope.Shipment1 = function (DirectBooking, directbooking) {
        if (DirectBooking.$valid) {
            $scope.vk = directbooking[0];
            UploadShipmentService.SaveShipmentFromErrorPopup($scope.vk).then(function (response) {
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

    $scope.Makeshipment = function (gridDirectBooking) {


        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'uploadShipment/uploadShipmentEcommForm/uploadShipmentForm.tpl.html',
            controller: 'uploadeCommerceBookingController',
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

    //$scope.DropDownBinding = function (er) {
        
    //    if (er.FileType === "DropDown" && er.FieldName === "ParcelType") {
    //        $scope.GeneralDropDownSource = $scope.ParcelTypes;
    //        $scope.Par = 'ParcelType.ParcelDescription';
    //        $scope.TrackBYOBJ = 'ParcelType.ParcelType';
    //        return true;
    //    }
    //    if (er.FileType === "DropDown" && er.FieldName === "Currency") {
    //        //$scope.GeneralDropDownSource = $scope.CurrencyTypes;
    //        //$scope.Par = 'ParcelType.CurrencyCode';
    //        //$scope.vm = 'ParcelType.CurrencyDescription';
    //        //$scope.str = "+ ' - ' +";
    //        //$scope.TrackBYOBJ = 'ParcelType.CurrencyCode';
    //        return true;
    //    }

    //};

    function init() {
        $scope.str = '';
        $scope.vm = '';
        $scope.Shipment = [{
            ShipFrom: {},
            ShipTo: {}
        }];
        $rootScope.FormName = "WithoutServiceForm";
        $scope.vk = {};
        $scope.submitted = true;
        var userInfo = SessionService.getUser();
        $scope.customerId = userInfo.EmployeeId;

        DirectBookingService.GetInitials($scope.customerId).then(function (response) {
            

            // Set Currency type according to given order
            $scope.CurrencyTypes = TopCurrencyService.TopCurrencyList(response.data.CurrencyTypes);
            $scope.ParcelTypes = response.data.ParcelTypes;
            $scope.ShipmentMethods = response.data.ShipmentMethods;
            $scope.CustomerDetail = response.data.CustomerDetail;
        }, function () {

        });

        $scope.ShipmentData = ShipmentData;
        $scope.GetErrorsForShipments($scope.ShipmentData);
        $anchorScroll.yOffset = 700;
        setMultilingualOptions();
    }

    init();

});