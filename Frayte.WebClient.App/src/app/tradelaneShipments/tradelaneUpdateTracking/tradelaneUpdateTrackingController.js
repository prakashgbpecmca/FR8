angular.module('ngApp.tradelaneShipments').controller('TradelaneUpdateTrackingController', function ($scope, $state, TradelaneMilestoneService, $translate, $uibModal, toaster, ModalService, SessionService, AppSpinner, DirectBookingService, DirectShipmentService, $rootScope, TradelaneShipmentService, uiGridConstants, config, DateFormatChange, ShipmentInfo, PopupType, TimeStringtoDateTime) {

    //translation code here
    var setMultilingualOptions = function () {
        $translate(['FrayteSuccess', 'FrayteWarning', 'FrayteError', 'DeletedSuccessfully', 'Sure_Delete_Flight_Tracking_Detail', 'Delete_Tracking_Confirmation',
        'Sure_Delete_Tracking_Detail', 'Delete_Flight_Tracking_Confirmation']).then(function (translations) {

            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.FrayteError = translations.FrayteError;
            $scope.DeletedSuccessfully = translations.DeletedSuccessfully;
            $scope.Sure_Delete_Flight_Tracking_Detail = translations.Sure_Delete_Flight_Tracking_Detail;
            $scope.Delete_Tracking_Confirmation = translations.Delete_Tracking_Confirmation;
            $scope.Sure_Delete_Tracking_Detail = translations.Sure_Delete_Tracking_Detail;
            $scope.Delete_Flight_Tracking_Confirmation = translations.Delete_Flight_Tracking_Confirmation;

        });
    };

    //end
    //update tracking collapsable code here
    $scope.updateTrackingCollapse = function () {
        $scope.collapse = !$scope.collapse;
    };
    //end

    $scope.AddTracking = function (row) {
        if (row === undefined || row === null) {
            ModalInstance = $uibModal.open({
                Animation: true,
                templateUrl: 'tradelaneShipments/tradelaneUpdateTracking/tradelaneAddTracking/tradelaneAddTracking.tpl.html',
                controller: 'TradelaneAddTrackingController',
                keyboard: true,
                windowClass: '',
                size: 'md',
                backdrop: 'static',
                resolve: {
                    ShipmentInfo: function () {
                        return $scope.Shipment;
                    },
                    TrackingInfo: function () {
                        return null;
                    }
                }
            });
            ModalInstance.result.then(function (response) {
                $scope.GetUpdateTracking();
            }, function () {

            });
        }
        else {
            ModalInstance = $uibModal.open({
                Animation: true,
                templateUrl: 'tradelaneShipments/tradelaneUpdateTracking/tradelaneAddTracking/tradelaneAddTracking.tpl.html',
                controller: 'TradelaneAddTrackingController',
                keyboard: true,
                windowClass: '',
                size: 'md',
                backdrop: 'static',
                resolve: {
                    ShipmentInfo: function () {
                        return $scope.Shipment;
                    },
                    TrackingInfo: function () {
                        return row;
                    }
                }
            });
            ModalInstance.result.then(function (response) {
                $scope.GetUpdateTracking();
            }, function () {

            });
        }
    };

    $scope.AddFlightDetail = function (row) {
        if (row === undefined || row === null) {
            ModalInstance = $uibModal.open({
                Animation: true,
                templateUrl: 'tradelaneShipments/tradelaneUpdateTracking/tradelaneAddFlightDetail/tradelaneAddFlightDetail.tpl.html',
                controller: 'TradelaneFlightDetailController',
                keyboard: true,
                windowClass: 'DirectBookingDetail',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    ShipmentInfo: function () {
                        return $scope.Shipment;
                    },
                    TrackingInfo: function () {
                        return null;
                    }
                }
            });
            ModalInstance.result.then(function (response) {
                $scope.GetUpdateTracking();
            }, function () {

            });
        }
        else {
            ModalInstance = $uibModal.open({
                Animation: true,
                templateUrl: 'tradelaneShipments/tradelaneUpdateTracking/tradelaneAddFlightDetail/tradelaneAddFlightDetail.tpl.html',
                controller: 'TradelaneFlightDetailController',
                keyboard: true,
                windowClass: 'DirectBookingDetail',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    ShipmentInfo: function () {
                        return $scope.Shipment;
                    },
                    TrackingInfo: function () {
                        return row;
                    }
                }
            });
            ModalInstance.result.then(function (response) {
                $scope.GetUpdateTracking();
            }, function () {

            });
        }
    };
    $scope.GetUpdateTracking = function () {
        TradelaneShipmentService.GetShipmentDetail($scope.TradelaneShipmentId).then(function (response) {
            if (response.data) {
                $scope.TRadelaneShipmentDetail = response.data;
                $scope.TRadelaneShipmentDetail.Volume = $scope.TRadelaneShipmentDetail.Volume !== undefined && $scope.TRadelaneShipmentDetail.Volume !== null & $scope.TRadelaneShipmentDetail.Volume !== "" ? $scope.TRadelaneShipmentDetail.Volume.toFixed(2): 0.00;
                $scope.MawbShow = $scope.TRadelaneShipmentDetail.Airlines.AilineCode + " " + $scope.TRadelaneShipmentDetail.Mawb.substring(0, 4) + " " + $scope.TRadelaneShipmentDetail.Mawb.substring(4, 8);
            }
        }, function () {

        });
        TradelaneShipmentService.GetUpdateTracking($scope.TradelaneShipmentId).then(function (response) {
            if (response.data) {

                $scope.TRadelaneTrackingList = response.data.TradelaneTrackingDetail;
                $scope.TRadelaneTrackingOperationalList = response.data.TradelaneOperationalDetail.reverse();

                for (i = 0; i < $scope.TRadelaneTrackingOperationalList.length; i++) {
                    $scope.TRadelaneTrackingOperationalList[i].CreatedOnUtc = DateFormatChange.DateFormatChange($scope.TRadelaneTrackingOperationalList[i].CreatedOnUtc);
                }
                for (j = 0; j < $scope.TRadelaneTrackingList.length; j++) {
                    if ($scope.TRadelaneTrackingList[j].DepartureDate !== null) {
                        $scope.TRadelaneTrackingList[j].DepartureDate = DateFormatChange.DateFormatChange($scope.TRadelaneTrackingList[j].DepartureDate);
                        $scope.TRadelaneTrackingList[j].Departure = moment($scope.TRadelaneTrackingList[j].DepartureTime, "hmm").format("HH:mm");
                    }
                    else {
                        $scope.TRadelaneTrackingList[j].DepartureDate = "";
                    }
                    if ($scope.TRadelaneTrackingList[j].ArrivalDate != null) {
                        $scope.TRadelaneTrackingList[j].ArrivalDate = DateFormatChange.DateFormatChange($scope.TRadelaneTrackingList[j].ArrivalDate);
                        $scope.TRadelaneTrackingList[j].Arrival = moment($scope.TRadelaneTrackingList[j].ArrivalTime, "hmm").format("HH:mm");
                    }
                    else {
                        $scope.TRadelaneTrackingList[j].ArrivalDate = "";
                    }
                }

            }


        }, function () {

        });

        TradelaneShipmentService.GetTrackingStatus($scope.TradelaneShipmentId).then(function (response) {
            if (response.data) {
                $scope.TRadelaneShipmentStatus = response.data;
                for (i = 0; i < $scope.TRadelaneShipmentStatus.length; i++) {
                    for (j = 0; j < $scope.TRadelaneShipmentStatus[i].TrackingStatus.length; j++) {
                        if ($scope.TRadelaneShipmentStatus[i].TrackingStatus.length > 0) {

                        }
                        $scope.TRadelaneShipmentStatus[i].TrackingStatus[j].Date = DateFormatChange.DateFormatChange($scope.TRadelaneShipmentStatus[i].TrackingStatus[j].Date);
                    }
                }
            }
        }, function () {

        });

    };

    //Delete Operational Tracking
    $scope.DeleteTradelaneTracking = function (row) {
        var modalOptions = {
            headerText: $scope.Delete_Tracking_Confirmation,
            bodyText: $scope.Sure_Delete_Tracking_Detail
        };
        ModalService.Confirm({}, modalOptions).then(function (result) {
            TradelaneShipmentService.DeleteTradelaneOperationalTracking(row.TradelaneShipmentTrackingId).then(function (response) {
                if (response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.DeletedSuccessfully,
                        showCloseButton: true
                    });
                    $scope.GetUpdateTracking();
                }
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.SuccessfullyDeletedDraftShipment,
                    showCloseButton: true
                });
            });
        });
    };

    $scope.DeleteTradelaneFlightTracking = function (row) {
        var modalOptions = {
            headerText: $scope.Delete_Flight_Tracking_Confirmation,
            bodyText: $scope.Sure_Delete_Flight_Tracking_Detail
        };
        ModalService.Confirm({}, modalOptions).then(function (result) {
            TradelaneShipmentService.DeleteTradelaneTracking(row.TradelaneFlightId).then(function (response) {
                if (response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.DeletedSuccessfully,
                        showCloseButton: true
                    });
                    $scope.GetUpdateTracking();
                }
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.SuccessfullyDeletedDraftShipment,
                    showCloseButton: true
                });
            });
        });
    };

    


    function init() {
        $scope.ShowMultiButton = false;
        if (PopupType === 'View') {
            $scope.ShowMultiButton = true;
        }
        $scope.collapse = false;
        $scope.Shipment = ShipmentInfo;
        $scope.TradelaneShipmentId = ShipmentInfo.TradelaneShipmentId;
        $scope.GetUpdateTracking();
        setMultilingualOptions();
    }
    init();
});