angular.module("ngApp.trackAndTraceDashboard")
       .controller("TrackAndTraceDashBoardController", function ($scope, $uibModal, AppSpinner, $translate, CustomerService, $filter, toaster, SessionService, $state, TrackAndTraceDashboardService) {

    //Set Multilingual for Modal Popup
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'FrayteSuccess', 'ErrorGettingRecordPleaseTryAgain', 'LoadingTrackTraceDashboard']).then(function (translations) {

            $scope.FrayteError = translations.FrayteError;
            $scope.FrayteInformation = translations.FrayteInformation;
            $scope.FrayteValidation = translations.FrayteValidation;
            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.ErrorGettingRecordPleaseTryAgain = translations.ErrorGettingRecordPleaseTryAgain;
            $scope.LoadingTrackTraceDashboard = translations.LoadingTrackTraceDashboard;

            // Initial Deatil call here --> Spinner Message should be multilingual
            getInitialDetails();
        });
    };

    var setUserDashBoard = function () {

        angular.forEach($scope.shipments, function (eachObj) {
            if (eachObj.StatusId === $scope.AftershipStatusTag.Pending) {
                $scope.dashBoardDetail.Pending.TotalShipment += 1;
                $scope.dashBoardDetail.Pending.Trackings.push(eachObj);
            }
            if (eachObj.StatusId === $scope.AftershipStatusTag.Expired) {
                $scope.dashBoardDetail.Expired.TotalShipment += 1;
                $scope.dashBoardDetail.Expired.Trackings.push(eachObj);
            }
            if (eachObj.StatusId === $scope.AftershipStatusTag.Exception) {
                $scope.dashBoardDetail.Exception.TotalShipment += 1;
                $scope.dashBoardDetail.Exception.Trackings.push(eachObj);
            }
            if (eachObj.StatusId === $scope.AftershipStatusTag.Delivered) {
                $scope.dashBoardDetail.Delivered.TotalShipment += 1;
                $scope.dashBoardDetail.Delivered.Trackings.push(eachObj);
            }
            if (eachObj.StatusId === $scope.AftershipStatusTag.AttemptFail) {
                $scope.dashBoardDetail.AttemptFail.TotalShipment += 1;
                $scope.dashBoardDetail.AttemptFail.Trackings.push(eachObj);
            }
            if (eachObj.StatusId === $scope.AftershipStatusTag.OutForDelivery) {
                $scope.dashBoardDetail.OutForDelivery.TotalShipment += 1;
                $scope.dashBoardDetail.OutForDelivery.Trackings.push(eachObj);
            }
            if (eachObj.StatusId === $scope.AftershipStatusTag.InTransit) {
                $scope.dashBoardDetail.InTransit.TotalShipment += 1;
                $scope.dashBoardDetail.InTransit.Trackings.push(eachObj);
            }
            if (eachObj.StatusId === $scope.AftershipStatusTag.InfoReceived) {
                $scope.dashBoardDetail.InfoReceived.TotalShipment += 1;
                $scope.dashBoardDetail.InfoReceived.Trackings.push(eachObj);
            }
        });

        calculatePercentage();
    };

    var calculatePercentage = function () {

        var totalShipments = $scope.shipments.length;
        $scope.dashBoardDetail.InfoReceived.Percentage = ($scope.dashBoardDetail.InfoReceived.TotalShipment / totalShipments) * 100;
        $scope.dashBoardDetail.Expired.Percentage = ($scope.dashBoardDetail.Expired.TotalShipment / totalShipments) * 100;
        $scope.dashBoardDetail.Exception.Percentage = ($scope.dashBoardDetail.Exception.TotalShipment / totalShipments) * 100;
        $scope.dashBoardDetail.Delivered.Percentage = ($scope.dashBoardDetail.Delivered.TotalShipment / totalShipments) * 100;
        $scope.dashBoardDetail.AttemptFail.Percentage = ($scope.dashBoardDetail.AttemptFail.TotalShipment / totalShipments) * 100;
        $scope.dashBoardDetail.OutForDelivery.Percentage = ($scope.dashBoardDetail.OutForDelivery.TotalShipment / totalShipments) * 100;
        $scope.dashBoardDetail.InTransit.Percentage = ($scope.dashBoardDetail.InTransit.TotalShipment / totalShipments) * 100;
        $scope.dashBoardDetail.Pending.Percentage = ($scope.dashBoardDetail.Pending.TotalShipment / totalShipments) * 100;

    };

    var getScreenInitials = function () {
        TrackAndTraceDashboardService.TrackAndTraceDashboard($scope.customerId).then(function (response) {

            if (response.status === 200 && response.data.Status) {

                $scope.shipments = response.data.Tracking;

                setUserDashBoard();
                AppSpinner.hideSpinnerTemplate();
            }
        }, function () {
            toaster.pop({
                type: "error",
                status: $scope.FrayteError,
                body: $scope.ErrorGettingRecordPleaseTryAgain
            });
            AppSpinner.hideSpinnerTemplate();
        });

    };

    $scope.viewShipments = function (detail) {
        if (detail.TotalShipment) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'trackAndTraceDashBoard/trackAndTraceDashBoardShipments/trackAndTraceDashBoardShipments.tpl.html',
                controller: 'DashBoardShipmentController',
                windowClass: 'DirectBookingDetail',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    StatusId: function () {
                        return detail.StatusId;
                    },
                    CustomerId: function () {
                        return $scope.customerId;
                    }
                }
            });

            modalInstance.result.then(function () {

            }, function () {

            });
        }

    };

    var getInitialDetails = function () {
        $scope.customerId = $scope.userInfo.EmployeeId;
        AppSpinner.showSpinnerTemplate($scope.LoadingTrackTraceDashboard, $scope.Template);
        CustomerService.GetCustomerDetail($scope.customerId).then(function (response) {
            $scope.customerDetail = response.data;
            getScreenInitials();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ErrorGettingRecordPleaseTryAgain,
                showCloseButton: true
            });
        });

        $scope.AftershipStatusTag = {
            Pending: 0,
            InfoReceived: 1,
            InTransit: 2,
            OutForDelivery: 3,
            AttemptFail: 4,
            Delivered: 5,
            Exception: 6,
            Expired: 7
        };
        $scope.AftershipStatus =
        {
            Pending: "Pending",
            PendingDisplay: "Pending",
            InfoReceived: "InfoReceived",
            InfoReceivedDisplay: "Info Received",
            InTransit: "InTransit",
            InTransitDisplay: "In Transit",
            OutForDelivery: "OutForDelivery",
            OutForDeliveryDisplay: "Out For Delivery",
            AttemptFail: "AttemptFail",
            AttemptFailDisplay: "Failed Attempt",
            Delivered: "Delivered",
            DeliveredDisplay: "Delivered",
            Exception: "Exception",
            ExceptionDislay: "Exception",
            Expired: "Expired",
            ExpiredDisplay: "Expired"
        };

        $scope.dashBoardDetail = {
            Delivered: {
                StatusId: $scope.AftershipStatusTag.Delivered,
                Status: $scope.AftershipStatus.Delivered,
                StatusDisplay: $scope.AftershipStatus.DeliveredDisplay,
                TotalShipment: 0,
                Trackings: [],
                Percentage: 0,
                ImageName: 'Delivered.png'
            },
            InTransit: {
                StatusId: $scope.AftershipStatusTag.InTransit,
                Status: $scope.AftershipStatus.InTransit,
                StatusDisplay: $scope.AftershipStatus.InTransitDisplay,
                TotalShipment: 0,
                Trackings: [],
                Percentage: 0,
                ImageName: 'InTransit.png'
            },
            OutForDelivery: {
                StatusId: $scope.AftershipStatusTag.OutForDelivery,
                Status: $scope.AftershipStatus.OutForDelivery,
                StatusDisplay: $scope.AftershipStatus.OutForDeliveryDisplay,
                TotalShipment: 0,
                Trackings: [],
                Percentage: 0,
                ImageName: 'OutForDelivery.png'
            },
            Pending: {
                StatusId: $scope.AftershipStatusTag.Pending,
                Status: $scope.AftershipStatus.Pending,
                StatusDisplay: $scope.AftershipStatus.PendingDisplay,
                TotalShipment: 0,
                Trackings: [],
                Percentage: 0,
                ImageName: 'Pending.png'
            },
            AttemptFail: {
                StatusId: $scope.AftershipStatusTag.AttemptFail,
                Status: $scope.AftershipStatus.AttemptFail,
                StatusDisplay: $scope.AftershipStatus.AttemptFailDisplay,
                TotalShipment: 0,
                Trackings: [],
                Percentage: 0,
                ImageName: 'FailedAttemt.png'
            },
            InfoReceived: {
                StatusId: $scope.AftershipStatusTag.InfoReceived,
                Status: $scope.AftershipStatus.InfoReceived,
                StatusDisplay: $scope.AftershipStatus.InfoReceivedDisplay,
                TotalShipment: 0,
                Trackings: [],
                Percentage: 0,
                ImageName: 'InfoReceived.png'
            },
            Exception: {
                StatusId: $scope.AftershipStatusTag.Exception,
                Status: $scope.AftershipStatus.Exception,
                StatusDisplay: $scope.AftershipStatus.ExceptionDisplay,
                TotalShipment: 0,
                Trackings: [],
                Percentage: 0,
                ImageName: 'Exception.png'
            },
            Expired: {
                StatusId: $scope.AftershipStatusTag.Expired,
                Status: $scope.AftershipStatus.Expired,
                StatusDisplay: $scope.AftershipStatus.ExpiredDisplay,
                TotalShipment: 0,
                Trackings: [],
                Percentage: 0,
                ImageName: 'Expired.png'
            }

        };
    };

    function init() {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.spinnerMessage = $scope.LoadingTrackTraceDashboard;
        $scope.userInfo = SessionService.getUser();

        if ($scope.userInfo) {
            setMultilingualOptions();
        }
        else {
            $state.go("login");
        }
    }

    init();

});