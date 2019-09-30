angular.module('ngApp.express').controller("ExpressBagShipmentsController", function ($scope, $uibModal, config, SessionService, AppSpinner, toaster, ExpressManifestService, DateFormatChange, $translate, BagId) {

    $scope.GetShipments = function () {
        ExpressManifestService.GetBagShipments($scope.BagId).then(function (response) {
            if (response.data !== null && response.data.length > 0) {
                $scope.totalItemCount = response.data[0].TotalRows;
            }
            else {
                $scope.totalItemCount = 0;
            }
            for (i = 0; i < response.data.length; i++) {
                response.data[i].CreatedOn = DateFormatChange.DateFormatChange(response.data[i].CreatedOn) + " " + response.data[i].CreatedOnTime;
            }
            $scope.ExpressShipmentList = response.data;
            AppSpinner.hideSpinnerTemplate();
        }, function (error) {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.GettingDataErrorValidation,
                showCloseButton: true
            });
        });
    };

    $scope.ExpressTrackingDetail = function (ShipmentId) {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'expressTrackingController',
            templateUrl: 'express/expressTracking/expressTracking.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            resolve: {
                ExpressShipmentId: function () {
                    return ShipmentId;
                }
            }
        });
    };

    function init() {
        var userInfo = SessionService.getUser();
        $scope.$UserRoleId = userInfo.RoleId;
        $scope.StaffRoleId = userInfo.RoleId;
        $scope.UserId = userInfo.EmployeeId;
        if (BagId) {
            $scope.BagId = BagId;
        }
        $scope.GetShipments();
    }
    init();
});