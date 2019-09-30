angular.module('ngApp.customer').controller('TrackingConfiguration', function ($scope, config, $state,TrackAndTraceDashboardService, AppSpinner, $location, $translate, $stateParams, $filter, CustomerService, SessionService, $uibModal, uiGridConstants, toaster, ModalService, TradelaneService, CarrierService, CountryService, ShipmentService, UserService, $anchorScroll) {


    //collapse code
    $scope.inTransitClick = function () {
        $scope.inTransit = !$scope.inTransit;
    };
    $scope.outForDeliveryClick = function () {
        $scope.outForDelivery = !$scope.outForDelivery;
    };
    $scope.deliverClick = function () {
        $scope.deliver = !$scope.deliver;
    };
    $scope.exceptionClick = function () {
        $scope.exception = !$scope.exception;
    };
    $scope.failedAttemptClick = function () {
        $scope.failedAttempt = !$scope.failedAttempt;
    };
    $scope.infoReceivedClick = function () {
        $scope.infoReceived = !$scope.infoReceived;
    };


    //end



    // set translation key
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteWarning', 'FrayteSuccess', 'ErrorSavingRecored', 'SavedRecordsSuccessfully', 'LoadingTrackingConfiguration', 'CorrectValidationErrorFirst']).then(function (translations) {
            $scope.FrayteError = translations.FrayteError;
            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.ErrorSavingRecored = translations.ErrorSavingRecored;
            $scope.SavedRecordsSuccessfully = translations.SavedRecordsSuccessfully;
            $scope.LoadingTrackingConfiguration = translations.LoadingTrackingConfiguration;
            $scope.CorrectValidationErrorFirst = translations.CorrectValidationErrorFirst;
            // Initial Deatil call here --> so that Spinner Message should be multilingual 
            $scope.GetCustomerDetailsInitials();
        });
    };

    $scope.SaveTrackingConfiguration = function (IsValid) {
        if (IsValid) {

            AppSpinner.showSpinnerTemplate('', $scope.Template);
            CustomerService.SaveTrackingConfiguration($scope.customerTrackingConfiguration).then(function (response) {
                if (response.status === 200) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.SavedRecordsSuccessfully,
                        showCloseButton: true
                    });
                    $scope.customerTrackingConfiguration = response.data;
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteError,
                        body: $scope.ErrorSavingRecored,
                        showCloseButton: true
                    });
                }
                AppSpinner.hideSpinnerTemplate();
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.ErrorSavingRecored,
                    showCloseButton: true
                });


                AppSpinner.hideSpinnerTemplate();
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.CorrectValidationErrorFirst,
                showCloseButton: true
            });
        }
    };

    $scope.GetCustomerDetailsInitials = function () {
        AppSpinner.showSpinnerTemplate($scope.LoadingTrackingConfiguration, $scope.Template);
        CustomerService.GetTrackingConfiguration($scope.customerId).then(function (response) {

            $scope.customerTrackingConfiguration = response.data;
            
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ErrorSavingRecored,
                showCloseButton: true
            });
            AppSpinner.hideSpinnerTemplate();
        });
    };
    
    $scope.selectTrackingConfiguration = function () {
        if( $scope.customerTrackingConfiguration.IsDeliveredEmail &&
        $scope.customerTrackingConfiguration.IsInTransitEmail &&
        $scope.customerTrackingConfiguration.IsOutForDeliveryEmail &&
        $scope.customerTrackingConfiguration.IsPendingEmail &&
        $scope.customerTrackingConfiguration.IsAttemptFailEmail &&
        $scope.customerTrackingConfiguration.IsInfoReceivedEmail &&
        $scope.customerTrackingConfiguration.IsExceptionEmail &&
        $scope.customerTrackingConfiguration.IsExpiredEmail) {
            $scope.selectAll = true;
        }
        else {
            $scope.selectAll = false;
        }
    };

    $scope.selectAllConfiguration = function () {
        if ($scope.selectAll) {
            $scope.customerTrackingConfiguration.IsDeliveredEmail = true;
            $scope.customerTrackingConfiguration.IsInTransitEmail = true;
            $scope.customerTrackingConfiguration.IsOutForDeliveryEmail = true;
            $scope.customerTrackingConfiguration.IsPendingEmail = true;
            $scope.customerTrackingConfiguration.IsAttemptFailEmail = true;
            $scope.customerTrackingConfiguration.IsInfoReceivedEmail = true;
            $scope.customerTrackingConfiguration.IsExceptionEmail = true;
            $scope.customerTrackingConfiguration.IsExpiredEmail = true;
        }
        else {
            $scope.customerTrackingConfiguration.IsDeliveredEmail = false;
            $scope.customerTrackingConfiguration.IsInTransitEmail = false;
            $scope.customerTrackingConfiguration.IsOutForDeliveryEmail = false;
            $scope.customerTrackingConfiguration.IsPendingEmail = false;
            $scope.customerTrackingConfiguration.IsAttemptFailEmail = false;
            $scope.customerTrackingConfiguration.IsInfoReceivedEmail = false;
            $scope.customerTrackingConfiguration.IsExceptionEmail = false;
            $scope.customerTrackingConfiguration.IsExpiredEmail = false;
        }
    };
     
    // info received 
    $scope.removeInfoReceived = function (index) {
        $scope.customerTrackingConfiguration.InfoReceivedEmails.splice(index, 1);
        if (!$scope.customerTrackingConfiguration.InfoReceivedEmails.length) {
            $scope.addMailInfoReceived();
        }
    };
    $scope.addMailInfoReceived = function () {
        var email = {
            Email: ""
        };
        $scope.customerTrackingConfiguration.InfoReceivedEmails.push(email);
    };
     
    // Out for delivery 
    $scope.removeoutForDeliveryEmail = function (index) {
        $scope.customerTrackingConfiguration.OutForDeliveryEmails.splice(index, 1);
        if (!$scope.customerTrackingConfiguration.OutForDeliveryEmails.length) {
            $scope.addoutForDeliveryEmail();
        }
    };
    $scope.addoutForDeliveryEmail = function () {
        var email = {
            Email: ""
        };
        $scope.customerTrackingConfiguration.OutForDeliveryEmails.push(email);
    };

    // Deleivered email 
    $scope.removeDeliveredEmails = function (index) {
        $scope.customerTrackingConfiguration.DeliveredEmails.splice(index, 1);
        if (!$scope.customerTrackingConfiguration.DeliveredEmails.length) {
            $scope.addDeliveredEmails();
        }
    };
    $scope.addDeliveredEmails = function () {
        var email = {
            Email: ""
        };
        $scope.customerTrackingConfiguration.DeliveredEmails.push(email);
    };


    // Attempt falied 
    $scope.removeAttemptFailEmail = function (index) {
        $scope.customerTrackingConfiguration.AttemptFailEmails.splice(index, 1);
        if (!$scope.customerTrackingConfiguration.AttemptFailEmails.length) {
            $scope.addAttemptFailEmail();
        }
    };
    $scope.addAttemptFailEmail = function () {
        var email = {
            Email: ""
        };
        $scope.customerTrackingConfiguration.AttemptFailEmails.push(email);
    };

    // Expired emails 
    $scope.removeExceptionEmail = function (index) {
        $scope.customerTrackingConfiguration.ExceptionEmails.splice(index, 1);
        if (!$scope.customerTrackingConfiguration.ExceptionEmails.length) {
            $scope.addExceptionEmail();
        }
    };
    $scope.addExceptionEmail = function () {
        var email = {
            Email: ""
        };
        $scope.customerTrackingConfiguration.ExceptionEmails.push(email);
    };
    // Intransit 
    $scope.removeMailInTransit = function (index) { 
            $scope.customerTrackingConfiguration.InTransitEmails.splice(index, 1);
                if (!$scope.customerTrackingConfiguration.InTransitEmails.length) {
                    $scope.addMailInTransit();
                } 
    };
    $scope.addMailInTransit = function () { 
        var email = {
            Email : ""
        };
        $scope.customerTrackingConfiguration.InTransitEmails.push(email);
    };
     
    function init() {
        $scope.infoReceived = true;
        $scope.inTransit = false;
        $scope.outForDelivery = false;
        $scope.deliver = false;
        $scope.failedAttempt = false;
        $scope.exception = false;

        $scope.submitted = true;

        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.selectAll = false;
        $scope.emailFormat = /^[a-z0-9._]+@[a-z]+\.[a-z.]/;

        $scope.trackingConfiguration = {
            Delivered: {
                StatusId:TrackAndTraceDashboardService.FrayteAftershipStatusTag.Delivered,
                Status: TrackAndTraceDashboardService.AftershipStatus.Delivered,
                StatusDisplay: TrackAndTraceDashboardService.AftershipStatus.DeliveredDisplay,
                ImageName: TrackAndTraceDashboardService.FrayteAftershipStatusImage.Delivered
            },
            InTransit: {
                StatusId: TrackAndTraceDashboardService.FrayteAftershipStatusTag.InTransit,
                Status: TrackAndTraceDashboardService.AftershipStatus.InTransit,
                StatusDisplay: TrackAndTraceDashboardService.AftershipStatus.InTransitDisplay,
                ImageName: TrackAndTraceDashboardService.FrayteAftershipStatusImage.InTransit
            },
            OutForDelivery: {
                StatusId: TrackAndTraceDashboardService.FrayteAftershipStatusTag.OutForDelivery,
                Status: TrackAndTraceDashboardService.AftershipStatus.OutForDelivery,
                StatusDisplay: TrackAndTraceDashboardService.AftershipStatus.OutForDeliveryDisplay,
                ImageName: TrackAndTraceDashboardService.FrayteAftershipStatusImage.OutForDelivery
            },
            Pending: {
                StatusId: TrackAndTraceDashboardService.FrayteAftershipStatusTag.Pending,
                Status: TrackAndTraceDashboardService.AftershipStatus.Pending,
                StatusDisplay: TrackAndTraceDashboardService.AftershipStatus.PendingDisplay,
                ImageName: TrackAndTraceDashboardService.FrayteAftershipStatusImage.Pending
            },
            AttemptFail: {
                StatusId: TrackAndTraceDashboardService.FrayteAftershipStatusTag.AttemptFail,
                Status: TrackAndTraceDashboardService.AftershipStatus.AttemptFail,
                StatusDisplay: TrackAndTraceDashboardService.AftershipStatus.AttemptFailDisplay,
                ImageName: TrackAndTraceDashboardService.FrayteAftershipStatusImage.AttemptFail
            },
            InfoReceived: {
                StatusId: TrackAndTraceDashboardService.FrayteAftershipStatusTag.InfoReceived,
                Status: TrackAndTraceDashboardService.AftershipStatus.InfoReceived,
                StatusDisplay: TrackAndTraceDashboardService.AftershipStatus.InfoReceivedDisplay,
                ImageName: TrackAndTraceDashboardService.FrayteAftershipStatusImage.InfoReceived
            },
            Exception: {
                StatusId: TrackAndTraceDashboardService.FrayteAftershipStatusTag.Exception,
                Status: TrackAndTraceDashboardService.AftershipStatus.Exception,
                StatusDisplay: TrackAndTraceDashboardService.AftershipStatus.ExceptionDislay,
                ImageName: TrackAndTraceDashboardService.FrayteAftershipStatusImage.Exception
            },
            Expired: {
                StatusId: TrackAndTraceDashboardService.FrayteAftershipStatusTag.Expired,
                Status: TrackAndTraceDashboardService.AftershipStatus.Expired,
                StatusDisplay: TrackAndTraceDashboardService.AftershipStatus.ExpiredDisplay,
                ImageName: TrackAndTraceDashboardService.FrayteAftershipStatusImage.Expired
            }

        };

        if ($stateParams.customerId === undefined) {
            $scope.customerId = "0";
        }
        else {
            var userInfo1 = SessionService.getUser();
            $scope.customerId = $stateParams.customerId;
        
        }
        setMultilingualOptions();
    }

    init();
});