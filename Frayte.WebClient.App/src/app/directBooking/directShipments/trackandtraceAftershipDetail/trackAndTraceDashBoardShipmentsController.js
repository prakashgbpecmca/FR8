angular.module("ngApp.directBooking").controller("DashBoardShipmentController", function ($scope, $translate, DateFormatChange, AppSpinner, StatusId, CustomerId, Status, config, TrackAndTraceDashboardService, $uibModalInstance) {

    //Set Multilingual for Modal Popup
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'FrayteSuccess', 'ErrorGettingRecordPleaseTryAgain', 'Loading',
                    'DEtail', 'Shipment_Booked', 'Delivered', 'OutforDelivery', 'InTransit', 'InfoReceived', 'Expired', 'FailedAttempt', 'Shipment_Exception']).then(function (translations) {
                $scope.FrayteError = translations.FrayteError;
                $scope.FrayteInformation = translations.FrayteInformation;
                $scope.FrayteValidation = translations.FrayteValidation;
                $scope.FrayteSuccess = translations.FrayteSuccess;
                $scope.ErrorGettingRecordPleaseTryAgain = translations.ErrorGettingRecordPleaseTryAgain;
                $scope.ShipmentLoading = translations.Loading;
                $scope.Detail = translations.DEtail;
                $scope.ShipmentBooked = translations.Shipment_Booked;
                $scope.ShipmentDelivered = translations.Delivered;
                $scope.ShipmentOutforDelivery = translations.OutforDelivery;
                $scope.ShipmentInTransit = translations.InTransit;
                $scope.ShipmentInfoReceived = translations.InfoReceived;
                $scope.ShipmentExpired = translations.Expired;
                $scope.ShipmentFailedAttempt = translations.FailedAttempt;
                $scope.ShipmentException = translations.Shipment_Exception;

                getScreenInitials();
            });
    };

    $scope.GetCorrectFormattedDate = function (date, time) {
        // Geting Correct Date Format
        if (date !== null && date !== '' && date !== undefined) {
            var newDate = new Date(date);
            var newTime;
            if (time !== undefined && time !== null) {
                newTime = time;
            }

            var days = ["SUNDAY", "MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY"];
            //var monthNames = ["January", "February", "March", "April", "May", "June","July", "August", "September", "October", "November", "December" ];
            //var dformat = days[newDate.getDay()] + ', ' + monthNames[newDate.getMonth()] + ' ' + newDate.getDate() + ', ' + newDate.getFullYear();
            var dformat = days[newDate.getDay()] + ', ' + DateFormatChange.DateFormatChange(newDate);
            if (time !== undefined && time !== null) {
                //dformat = days[newDate.getDay()] + ', ' + monthNames[newDate.getMonth()] + ' ' + newDate.getDate() + ', ' + newDate.getFullYear() + ':' + newTime;
                dformat = days[newDate.getDay()] + ', ' + DateFormatChange.DateFormatChange(newDate) + ':' + newTime;
            }
            return dformat;
        }
        else {
            return;
        }
    };

    var setTrackingStatus = function () {

        for (var i = 0 ; i < $scope.trackings.length; i++) {
            if ($scope.trackings[i].StatusId == TrackAndTraceDashboardService.FrayteAftershipStatusTag.Pending) {
                $scope.trackings[i].Image = TrackAndTraceDashboardService.FrayteAftershipStatusImage.Pending;
            }
            else if ($scope.trackings[i].StatusId == TrackAndTraceDashboardService.FrayteAftershipStatusTag.InfoReceived) {
                $scope.trackings[i].Image = TrackAndTraceDashboardService.FrayteAftershipStatusImage.InfoReceived;
            }
            else if ($scope.trackings[i].StatusId == TrackAndTraceDashboardService.FrayteAftershipStatusTag.InTransit) {
                $scope.trackings[i].Image = TrackAndTraceDashboardService.FrayteAftershipStatusImage.InTransit;
            }
            else if ($scope.trackings[i].StatusId == TrackAndTraceDashboardService.FrayteAftershipStatusTag.OutForDelivery) {
                $scope.trackings[i].Image = TrackAndTraceDashboardService.FrayteAftershipStatusImage.OutForDelivery;
            }
            else if ($scope.trackings[i].StatusId == TrackAndTraceDashboardService.FrayteAftershipStatusTag.AttemptFail) {
                $scope.trackings[i].Image = TrackAndTraceDashboardService.FrayteAftershipStatusImage.AttemptFail;
            }
            else if ($scope.trackings[i].StatusId == TrackAndTraceDashboardService.FrayteAftershipStatusTag.Delivered) {
                $scope.trackings[i].Image = TrackAndTraceDashboardService.FrayteAftershipStatusImage.Delivered;
            }
            else if ($scope.trackings[i].StatusId == TrackAndTraceDashboardService.FrayteAftershipStatusTag.Exception) {
                $scope.trackings[i].Image = TrackAndTraceDashboardService.FrayteAftershipStatusImage.Exception;
            }
            else if ($scope.trackings[i].StatusId == TrackAndTraceDashboardService.FrayteAftershipStatusTag.Expired) {
                $scope.trackings[i].Image = TrackAndTraceDashboardService.FrayteAftershipStatusImage.Expired;
            }

        }

    };

    var setStatusDisplay = function (Display) {
        if (Display !== undefined && Display !== null && Display !== '') {
            if (Display === 'Pending') {
                return $scope.ShipmentBooked;
            }
            else if (Display === 'OutForDelivery') {
                return $scope.ShipmentOutforDelivery;
            }
            else if (Display === 'Delivered') {
                return $scope.ShipmentDelivered;
            }
            else if (Display === 'Exception') {
                return $scope.ShipmentException;
            }
            else if (Display === 'InTransit') {
                return $scope.ShipmentInTransit;
            }
            else if (Display === 'Expired') {
                return $scope.ShipmentExpired;
            }
            else if (Display === 'InfoReceived') {
                return $scope.ShipmentInfoReceived;
            }
            else if (Display === 'AttemptFail') {
                return $scope.ShipmentFailedAttempt;
            }
        }
        else {
            return '';
        }
    };

    var getScreenInitials = function () {
        $scope.Display = setStatusDisplay($scope.StatusDisplay);
        AppSpinner.showSpinnerTemplate($scope.ShipmentLoading + ' ' + $scope.Display + ' ' + $scope.Detail, $scope.Template);
        TrackAndTraceDashboardService.TrackAndtraceTrackings($scope.trackAfterShipTracking).then(function (response) {
            if (response.status === 200 && response.data.Status) {
                $scope.trackings = response.data.Tracking;
                setTrackingStatus();
                AppSpinner.hideSpinnerTemplate();
            }
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ErrorGettingRecordPleaseTryAgain,
                showCloseButton: true
            });
        });
    };

    function init() {

        $scope.ImagePath = config.BUILD_URL;
        $scope.statusIds = TrackAndTraceDashboardService.FrayteAftershipStatusTag;
        $scope.statusMessages = TrackAndTraceDashboardService.AftershipStatus;
        $scope.StatusDisplay = Status;
        setMultilingualOptions();
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.trackAfterShipTracking = {
            CustomerId: CustomerId,
            StatusId: StatusId,
            createdAtMax: new Date(),
            createdAtMin: new Date(),
            Keyword: "",
            Lang: "",
            Limit: 200,
            Page: 1
        };
    }

    init();
});