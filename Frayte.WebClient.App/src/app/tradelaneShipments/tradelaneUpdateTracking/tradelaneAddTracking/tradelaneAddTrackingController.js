angular.module('ngApp.tradelaneShipments').controller('TradelaneAddTrackingController', function ($scope, $state, TradelaneMilestoneService, $translate, $uibModal, toaster, ModalService, SessionService, AppSpinner, DirectBookingService, DirectShipmentService, $rootScope, TradelaneShipmentService, uiGridConstants, config, DateFormatChange, ShipmentInfo, TrackingInfo, $uibModalInstance) {

    //translation code here
    var setMultilingualOptions = function () {
        $translate(['FrayteSuccess', 'FrayteWarning', 'FrayteError', 'SaveDraft_Validation', 'Save_TradelaneTracking', 'Adding_Tracking', 'Save_Tracking_Status_Delivered_Confirmation', 'Sure_Update_Shipment_Delivered']).then(function (translations) {

            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.FrayteError = translations.FrayteError;
            $scope.SaveDraft_Validation = translations.SaveDraft_Validation;
            $scope.Adding_Tracking = translations.Adding_Tracking;
            $scope.Save_Tracking_Status_Delivered_Confirmation = translations.Save_Tracking_Status_Delivered_Confirmation;
            $scope.Sure_Update_Shipment_Delivered = translations.Sure_Update_Shipment_Delivered;
            $scope.Save_Tradelane_Tracking = translations.Save_TradelaneTracking;
        });
    };

    //end

    $scope.OpenCalender = function ($event) {
        $scope.status.opened = true;
    };

    $scope.SaveAddTracking = function (isValid, TrackingList) {
        if (isValid && TrackingList.TrackingCode !== 'DLV') {
            AppSpinner.showSpinnerTemplate($scope.Adding_Tracking, $scope.Template);
            TradelaneShipmentService.SaveUpdateTracking(TrackingList).then(function (response) {
                AppSpinner.hideSpinnerTemplate();
                if (response.data.Status) {
                    $scope.TRadelaneTrackingList = response.data;
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.Save_Tradelane_Tracking,
                        showCloseButton: true
                    });
                    $uibModalInstance.close();
                }
                else {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteWarning,
                        body: response.data.Errors[0],
                        showCloseButton: true
                    });
                }
            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.SaveDraft_Validation,
                    showCloseButton: true
                });
            });

        }
        else if (isValid && TrackingList.TrackingCode === 'DLV') {
            var modalOptions = {
                headerText: $scope.Save_Tracking_Status_Delivered_Confirmation,
                bodyText: $scope.Sure_Update_Shipment_Delivered
            };

            ModalService.Confirm({}, modalOptions).then(function (result) {
                AppSpinner.showSpinnerTemplate($scope.Adding_Tracking, $scope.Template);
                TradelaneShipmentService.SaveUpdateTracking(TrackingList).then(function (response) {
                    AppSpinner.hideSpinnerTemplate();
                    if (response.data.Status) {
                        $scope.TRadelaneTrackingList = response.data;
                        toaster.pop({
                            type: 'success',
                            title: $scope.FrayteSuccess,
                            body: $scope.Save_Tradelane_Tracking,
                            showCloseButton: true
                        });
                        $uibModalInstance.close();
                    }

                }, function () {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteError,
                        body: $scope.SaveDraft_Validation,
                        showCloseButton: true
                    });
                });
            });

        }
        else {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.SaveDraft_Validation,
                showCloseButton: true
            });
        }

    };



    $scope.GetInitial = function () {
        TradelaneShipmentService.GetAirports().then(function (response) {
            if (response.data.length > 0) {
                $scope.Airports = response.data;
            }
        }, function () {
        });

        TradelaneShipmentService.GetShipmentHandlerMethodId($scope.TradelaneShipmentId).then(function (response) {
            if (response.data) {
                $scope.ShipmentHandlerMethodId = response.data;
                TradelaneShipmentService.GetTradelaneMilestone($scope.ShipmentHandlerMethodId).then(function (response) {
                    if (response.data.length > 0) {
                        $scope.TrackingMileStones = response.data;
                    }
                }, function () {
                });
            }
        }, function () {
        });

    };

    $scope.ChangeOperationalStatus = function (status) {
        if (status !== null && status === 'DLV') {
            for (i = 0; i < $scope.Airports.length; i++) {
                if ($scope.Airports[i].AirportName + ' ' + $scope.Airports[i].AirportCode === ShipmentInfo.DestinationAirport) {
                    $scope.AddTracking.AirportCode = $scope.Airports[i].AirportCode;
                }
            }
        }
        else {
            //$scope.AddTracking.AirportCode = "";
        }
    };

    $scope.NewAddtrackingModel = function () {
        $scope.AddTracking = {
            TradelaneShipmentId: ShipmentInfo.TradelaneShipmentId,
            TrackingCode: '',
            TrackingDescription: '',
            AirportCode: '',
            CreatedOnUtc: new Date(),
            CreatedBy: $scope.UserId
        };
    };



    $scope.ValidStartTime = function (starttime) {
        if (starttime !== undefined && starttime !== null && starttime !== '') {
            if (parseInt(starttime, 0) > 2359) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarning,
                    body: $scope.ValidTime,
                    showCloseButton: true
                });
                $scope.userDetail.WorkingStartTime = null;
            }
        }
    };
    function init() {
        setMultilingualOptions();
        var userInfo = SessionService.getUser();
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.RoleId = userInfo.RoleId;
        $scope.UserId = userInfo.EmployeeId;
        $scope.collapse = false;
        $scope.NewAddtrackingModel();
        if (TrackingInfo !== null) {
            $scope.AddTracking = TrackingInfo;
            $scope.AddTracking.CreatedOnUtc = new Date(TrackingInfo.CreatedOnUtc);
        }

        if (ShipmentInfo === null) {
            $scope.TradelaneShipmentId = 0;
        }
        else {
            $scope.TradelaneShipmentId = ShipmentInfo.TradelaneShipmentId;
        }
       
        $scope.GetInitial();
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