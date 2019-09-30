angular.module('ngApp.tradelaneShipments').controller('TradelaneFlightDetailController', function ($scope, $state, TradelaneMilestoneService, $translate, $uibModal, toaster, ModalService, SessionService, AppSpinner, DirectBookingService, DirectShipmentService, $rootScope, TradelaneShipmentService, uiGridConstants, config, DateFormatChange, ShipmentInfo, TrackingInfo, $uibModalInstance) {

    //translation code here
    var setMultilingualOptions = function () {
        $translate(['FrayteSuccess', 'FrayteWarning', 'FrayteError', 'SaveDraft_Validation', 'Save_TradelaneFlight_Tracking']).then(function (translations) {

            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.FrayteError = translations.FrayteError;
            $scope.SaveDraft_Validation = translations.SaveDraft_Validation;
            $scope.SaveTradelaneFlight_Tracking = translations.Save_TradelaneFlight_Tracking;

        });
    };

    //end


    $scope.OpenCalender = function ($event) {
        $scope.status.opened = true;
    };
    $scope.OpenCalender1 = function ($event) {
        $scope.status1.opened = true;
    };
    $scope.ChangeDepartureDate = function (FromDate) {
        $scope.AddTracking.DepartureDate = $scope.SetTimeinDateObj(FromDate);
     
    };

    $scope.ChangeArrivalDate = function (ToDate) {
        $scope.AddTracking.ArrivalDate = $scope.SetTimeinDateObj(ToDate);
    };
    $scope.SetTimeinDateObj = function (DateValue) {
        var newdate1 = new Date();
        newdate = new Date(DateValue);
        var gtDate = newdate.getDate();
        var gtMonth = newdate.getMonth();
        var gtYear = newdate.getFullYear();
        var hour = newdate1.getHours();
        var min = newdate1.getMinutes();
        var Sec = newdate1.getSeconds();
        var MilSec = new Date().getMilliseconds();
        return new Date(gtYear, gtMonth, gtDate, hour, min, Sec, MilSec);
    };
    $scope.SaveAddFlightTracking = function (isValid, TrackingList) {
        if (isValid) {
            TradelaneShipmentService.SaveTradelaneShipmentTracking(TrackingList).then(function (response) {
                if (response.data.Status) {
                    $scope.TRadelaneTrackingList = response.data;
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.SaveTradelaneFlight_Tracking,
                        showCloseButton: true
                    });
                    $uibModalInstance.close();
                }

            }, function () {

                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.SaveDraft_Validation,
                    showCloseButton: true
                });
            });

        }
        else {
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

    $scope.NewAddtrackingModel = function () {
        $scope.AddTracking = {
            TradelaneShipmentId: ShipmentInfo.TradelaneShipmentId,
            FlightNo: '',
            TrackingDescription: '',
            AirportCode: '',
            DepartureDate: new Date(),
            DepartureTime: '',
            ArrivalDate: new Date(),
            ArrivalTime: '',
            DepartureAirportCode: '',
            DestinationAirportCode: '',
            TotalPeices: null,
            TotalWeight: null,
            Volume: null,
            BookingStatus: ''
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
        $scope.RoleId = userInfo.RoleId;
        $scope.UserId = userInfo.EmployeeId;
        $scope.collapse = false;
        $scope.NewAddtrackingModel();
        if (TrackingInfo !== null) {
            $scope.AddTracking = TrackingInfo;
            $scope.AddTracking.DepartureDate = new Date(TrackingInfo.DepartureDate);
            $scope.AddTracking.ArrivalDate = new Date(TrackingInfo.ArrivalDate);
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
        $scope.status1 = {
            opened: false
        };
        $scope.dateOptions = {
            formatYear: 'yy',
            startingDay: 1,
            minDate: new Date(),
            dateDisabled: function (data) {
                var date = data.date,
                mode = data.mode;
                return mode === 'day' && (date.getDay() === 0 || date.getDay() === 6);
            }
        };
        $scope.dateOptions1 = {
            formatYear: 'yy',
            startingDay: 1,
            minDate: new Date(),
            dateDisabled: function (data) {
                var date = data.date,
                mode = data.mode;
                return mode === 'day' && (date.getDay() === 0 || date.getDay() === 6);
            }
        };
    }
    init();
});