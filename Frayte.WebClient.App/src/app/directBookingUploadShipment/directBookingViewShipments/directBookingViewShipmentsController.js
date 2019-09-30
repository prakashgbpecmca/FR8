angular.module('ngApp.directBookingUploadShipment').controller('DirectBookingViewShipmentController', function ($scope, $rootScope, toaster, config, $state, SessionService, UploadShipmentService, $uibModal, $http, $window, AppSpinner, ModalService, $interval, $timeout, DbUploadShipmentService, SessionId) {
    $scope.TrackingPage = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'home/trackingnew.tpl.html',
            controller: 'HomeTrackingController',
            windowClass: '',
            size: 'lg',
            resolve: {
                ShipmentData: function () {
                    return;
                },
                ShipmentData1: function () {
                    return;
                },
                ShipmentData2: function () {
                    return row;
                }
            }
        });
        modalInstance.result.then(function () {

        }, function () {
        });
    };
    $scope.DirectShipmentDetail = function (row) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'directBooking/directBookingDetail/directBookingDetail.tpl.html',
                controller: 'DirectBookingDetailController',
                windowClass: 'DirectBookingDetail',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    shipmentId: function () {
                        return row.ShipmentId;
                    },
                    ShipmentStatus: function () {
                        return "Current";
                    }
                }
            });
    };

    $scope.DirectShipments = function () {
        //if ($scope.track.BookingMethod === 'DirectBooking') {
        //    AppSpinner.showSpinnerTemplate('Loading Track & Trace', $scope.Template);
        //}
        //else if ($scope.track.BookingMethod === 'eCommerce') {
        //    AppSpinner.showSpinnerTemplate('Loading Track & Trace', $scope.Template);
        //}

        DbUploadShipmentService.GetShipments($scope.SessionId).then(function (response) {
         
            for (var i = 0 ; i < response.data.length; i++) {

                //if (response.data[i].BooingApp ===)

                if (response.data[i].ShippingBy === "UKMail" || response.data[i].ShippingBy === "Yodel" || response.data[i].ShippingBy === "Hermes") {
                    response.data[i].CourierName = "UKEUShipment";
                }
                else {
                    response.data[i].CourierName = response.data[i].ShippingBy;
                }
                if (response.data[i].TrackingNo === "" || response.data[i].TrackingNo === null) {
                    response.data[i].TrackingCode = "123";
                }
                else {
                    response.data[i].TrackingCode = response.data[i].TrackingNo;
                }

                if (response.data[i].RateType == null) {
                    response.data[i].DisplayName = response.data[i].DisplayName + "";
                }
                else {
                    response.data[i].DisplayName = response.data[i].DisplayName + " " + response.data[i].RateType;
                }

            }
            $scope.DirectShipmentList = response.data;
            for (i = 0; i < response.data.length; i++) {

                if (response.data[i].Status === 'Cancel') {
                    response.data.splice(i, 1);
                }
            }

            AppSpinner.hideSpinnerTemplate();
        }, function (error) {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.Frayte_Error,
                body: $scope.GettingDataErrorValidation,
                showCloseButton: true
            });
        });
    };

    function init() {
        $scope.SessionId = SessionId;
        // Pagination Logic 
        $scope.viewby = 50;
        $scope.currentPage = 1;
        $scope.itemsPerPage = $scope.viewby;
        $scope.totalItemCount = 1000;

        $scope.maxSize = 2; //Number of pager buttons to show

        var userInfo = SessionService.getUser();

        $scope.DirectShipments();
    }
    init();

});