angular.module('ngApp.mileStone').controller('TradelaneMileStoneAddEditController', function ($scope, $translate, $uibModal, TrackingMileStone, Mode, toaster, TradelaneMilestoneService, ShipmentHandlerMethod, $uibModalInstance, SessionService, TrackingMileStoneLastRecord) {
        var setMultilingualOptions = function () {
            $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'FrayteSuccess', 'Frayte_Success', 'FrayteWarning',
                 'PleaseCorrectValidationErrors', 'FrayteWarning_Validation', 'TradelaneMilestoneNotValid', 'ErrorSavingMilestone', 'MileStoneSavedSuccessfully']).then(function (translations) {
                     $scope.Frayte_Warning = translations.FrayteWarning;
                     $scope.Frayte_Success = translations.Frayte_Success;
                     $scope.ShipmentDeletedFromSession = translations.ShipmentDeletedFromSession;
                     $scope.TitleFrayteError = translations.FrayteError;
                     $scope.TitleFrayteInformation = translations.FrayteInformation;
                     $scope.TitleFrayteValidation = translations.FrayteValidation;
                     $scope.TitleFrayteSuccess = translations.FrayteSuccess;
                     $scope.TextValidation = translations.PleaseCorrectValidationErrors;
                     $scope.MileStoneSavedSuccessfully = translations.MileStoneSavedSuccessfully;
                     $scope.ErrorSavingMilestone = translations.ErrorSavingMilestone;
                     $scope.TradelaneMilestoneNotValid = translations.PleaseCorrectValidationErrors;
                 });
        };

        $scope.CheckTrackingMileStoneKey = function () {
            TradelaneMilestoneService.CheckTrackingMileStoneKey($scope.TrackingMileStone).then(function (response) {
                if (response.data) {
                    $scope.ShipmentHandlerList = response.data;
                    $scope.flag = true;
                }
                else {
                    $scope.flag = false;
                }
            });
        };
        $scope.CheckOrderNo = function (order) {
            if (order.OrderNumber !== undefined && order.OrderNumber !== null && order.OrderNumber !== "") {
                TradelaneMilestoneService.CheckOrderNo(order).then(function (response) {
                    if (response.data) {

                        $scope.flag1 = true;
                    }
                    else {
                        $scope.flag1 = false;
                    }
                });
            }
            else {
                $scope.flag1 = false;
            }
        };

        $scope.SaveMileStone = function (isValid, TrackingMileStone) {
            if (isValid && !$scope.flag1 && TrackingMileStone.MileStoneKey.length === 3 && !$scope.flag) {
                TradelaneMilestoneService.SaveMilestone($scope.TrackingMileStone).then(function (response) {
                    if (response.data > 0) {
                        toaster.pop({
                            type: 'success',
                            title: $scope.Frayte_Success,
                            body: $scope.MileStoneSavedSuccessfully,
                            showCloseButton: true
                        });
                        $uibModalInstance.close({ reload: true });
                    }
                    else {
                        toaster.pop({
                            type: 'error',
                            title: $scope.TitleFrayteError,
                            body: $scope.ErrorSavingMilestone,
                            showCloseButton: true
                        });
                    }
                }, function () {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.ErrorSavingMilestone,
                        showCloseButton: true
                    });
                });
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.Frayte_Warning,
                    body: $scope.TradelaneMilestoneNotValid,
                    showCloseButton: true
                });
            }
        };

        $scope.TrackingMileStoneModel = function () {
            $scope.TrackingMileStone = {
                TrackingMileStoneId: 0,
                MileStoneKey: "",
                Description: "",
                OrderNumber: null,
                ShipmentHandlerMethodId: ShipmentHandlerMethod.ShipmentHandlerMethodId,
                CreatedOnUtc: null,
                CreatedBy: $scope.CustomerId,
                UpdatedOnUtc: null,
                UpdatedBy: $scope.CustomerId
            };
        };
        function init() {
            var userInfo = SessionService.getUser();
            $scope.RoleId = userInfo.RoleId;
            $scope.CustomerId = userInfo.EmployeeId;
            if (TrackingMileStone !== "") {
                $scope.TrackingMileStone = TrackingMileStone;
                $scope.TrackingMileStone.ShipmentHandlerMethodId = ShipmentHandlerMethod.ShipmentHandlerMethodId;
                $scope.TrackingMileStone.UpdatedBy = $scope.CustomerId;
            }
            else {
                $scope.TrackingMileStoneModel();
            }
            $scope.Mode = Mode;
            //if ($scope.Mode === "Add") {
            //    $scope.TrackingMileStone.OrderNumber = 1;
            //}
            //else if(){
            //    ///$scope.TrackingMileStone.OrderNumber = $scope.TrackingMileStone.OrderNumber;
            //}
            setMultilingualOptions();
        }
        init();
    });