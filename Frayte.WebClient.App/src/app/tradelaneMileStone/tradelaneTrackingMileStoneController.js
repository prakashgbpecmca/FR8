angular.module('ngApp.mileStone').controller('TradelaneTrackingMileStoneController', function ($scope, TradelaneMilestoneService, $translate, $uibModal, toaster, ModalService) {
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'FrayteSuccess', 'Frayte_Success', 'FrayteWarning',
             'PleaseCorrectValidationErrors', 'FrayteWarning_Validation', 'DeleteTradelaneMilestone', 'SureDeleteMilestone',
        'MileStoneDeletedSuccessfully', 'ErrorDeletingRecords', 'ErrorGettingRecords', 'NoRecordSelectedShipmentHandler', 'NoRecordShipmentHandlerMethods']).then(function (translations) {

                 $scope.Frayte_Warning = translations.FrayteWarning;
                 $scope.Frayte_Success = translations.Frayte_Success;
                 $scope.ShipmentDeletedFromSession = translations.ShipmentDeletedFromSession;
                 $scope.TitleFrayteError = translations.FrayteError;
                 $scope.TitleFrayteInformation = translations.FrayteInformation;
                 $scope.TitleFrayteValidation = translations.FrayteValidation;
                 $scope.TitleFrayteSuccess = translations.FrayteSuccess;
                 $scope.TextValidation = translations.PleaseCorrectValidationErrors;
                 $scope.DeleteTradelaneMilestone = translations.DeleteTradelaneMilestone;
                 $scope.SureDeleteMilestone = translations.SureDeleteMilestone;
                 $scope.MileStoneDeletedSuccessfully = translations.MileStoneDeletedSuccessfully;
                 $scope.ErrorDeletingRecords = translations.ErrorDeletingRecords;
                 $scope.ErrorGettingRecords = translations.ErrorGettingRecords;
                 $scope.NoRecordSelectedShipmentHandler = translations.NoRecordSelectedShipmentHandler;
                 $scope.NoRecordShipmentHandlerMethods = translations.NoRecordShipmentHandlerMethods;
             });
    };

    $scope.DeleteTradelaneMileStone = function (MileStoneId) {
        
        var modalOptions = {
            headerText: $scope.DeleteTradelaneMilestone,
            bodyText: $scope.SureDeleteMilestone
        };

        ModalService.Confirm({}, modalOptions).then(function () {
            TradelaneMilestoneService.DeleteMileStone(MileStoneId).then(function (response) {
                if (response.data) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.Frayte_Success,
                        body: $scope.MileStoneDeletedSuccessfully,
                        showCloseButton: true
                    });
                    $scope.val = true;
                    $scope.getTrackingMileStones();
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.ErrorDeletingRecords,
                        showCloseButton: true
                    });

                }
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.ErrorDeletingRecords,
                    showCloseButton: true
                });

            });
        });

      
    };

  
    $scope.MileStoneAddEdit = function (row) {
        //var modalInstance;
        if (row === undefined || row === null) {
           var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'tradelaneMileStone/tradelaneMileStoneAddEdit.tpl.html',
                controller: 'TradelaneMileStoneAddEditController',
                windowClass: '',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    TrackingMileStone: function () {
                        return "";
                    },
                    Mode: function () {
                        return "Add";
                    },
                    TrackingMileStoneLastRecord : function(){
                        return $scope.TrackingMileStones[$scope.TrackingMileStones.length - 1];
                    },
                    ShipmentHandlerMethod: function () {
                        return $scope.ShipmentHandlerMethod;
                    }
                }
            });
            modalInstance.result.then(function () {
                $scope.getTrackingMileStones();
            }, function () {
                
            });
        }
        else {
            var modalInstance1 = $uibModal.open({
                animation: true,
                templateUrl: 'tradelaneMileStone/tradelaneMileStoneAddEdit.tpl.html',
                controller: 'TradelaneMileStoneAddEditController',
                windowClass: '',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    TrackingMileStone: function () {
                        return row;
                    },
                    Mode: function () {
                        return "Edit";
                    },
                    TrackingMileStoneLastRecord: function () {
                        return "";
                    },
                    ShipmentHandlerMethod: function () {
                        return $scope.ShipmentHandlerMethod;
                    }
                }
            });
            modalInstance1.result.then(function () {
                $scope.getTrackingMileStones();
            }, function () {

            });

        }

    };

    $scope.getTrackingMileStones = function () {
        TradelaneMilestoneService.GetTrackingMileStone($scope.ShipmentHandlerMethod.ShipmentHandlerMethodId).then(function (response) {
            if (response.data.length > 0) {
                $scope.TrackingMileStones = response.data;
            }
            else {
                $scope.TrackingMileStones = response.data;
                if (!$scope.val) {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteValidation,
                        body: $scope.NoRecordSelectedShipmentHandler,
                        showCloseButton: true
                    });
                }
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.ErrorGettingRecords,
                showCloseButton: true
            });
        });
    };




    function init() {
        $scope.val = false;
        TradelaneMilestoneService.GetShipmentHandlerMethods().then(function (response) {
            if (response.data.length > 0) {
                $scope.ShipmentHandlerList = response.data;
                $scope.ShipmentHandlerMethod = $scope.ShipmentHandlerList[0];
                $scope.getTrackingMileStones();
            }
            else {
                $scope.ShipmentHandlerList = response.data;
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteValidation,
                    body: $scope.NoRecordhipmentHandlerMethods,
                    showCloseButton: true
                });
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.ErrorGettingRecords,
                showCloseButton: true
            });
        });
        setMultilingualOptions();
    }
    init();
});