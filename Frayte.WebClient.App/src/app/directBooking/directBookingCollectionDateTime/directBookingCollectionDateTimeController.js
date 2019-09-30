angular.module('ngApp.directBooking').controller('DirectBookingCollectionDateTimeController', function ($scope, $uibModal, $uibModalInstance, $translate, PackageLength, toaster, ShipFromCountryId, DirectBookingService, $rootScope) {

    $scope.closePopUp = function () {
        $scope.PlaceBookingObj.Type = "Close";
        $scope.PlaceBookingObj.BookingStatus = 'Current';
        $scope.PlaceBookingObj.CollectionDate = $scope.CollectionDate;
        $scope.PlaceBookingObj.CollectionTime = $scope.CollectionTime;
        if ($scope.IsCollection === true) {
            $scope.PlaceBookingObj.IsCollection = $scope.IsCollection;
        }
        $uibModalInstance.close($scope.PlaceBookingObj);
    };

    var setModalOptions = function () {
        $translate(['FrayteWarning', 'CollectionTime_Required', 'CollectionDate_Required']).then(function (translations) {
            $scope.Frayte_Warning = translations.FrayteWarning;
            $scope.CollectionTimeRequired = translations.CollectionTime_Required;
            $scope.CollectionDateRequired = translations.CollectionDate_Required;

            $scope.CountryCurrentDateTime();
        });
    };

    $scope.CountryCurrentDateTime = function () {
        DirectBookingService.GetCountryCurrentDateTime($scope.FromCountryId).then(function (response) {
            if (response.data !== undefined && response.data !== null && response.data !== '') {
                $scope.CurrentDate = response.data.CurrentDate;
                $scope.CurrentTime = response.data.CurrentTime;
            }
            else {
                $scope.CurrentDate = "";
                $scope.CurrentTime = "";
            }
        });
    };

    $scope.OpenCalender = function ($event) {
        $scope.status.opened = true;
    };

    $scope.PlaceBooking = function (directBookingForm, IsValid, directBooking) {
        if ($scope.IsCollection !== undefined && $scope.IsCollection !== null && $scope.IsCollection !== '' && $scope.IsCollection === true) {
            if (($scope.CollectionTime === undefined || $scope.CollectionTime === '' || $scope.CollectionTime === null)) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.Frayte_Warning,
                    body: $scope.CollectionTimeRequired,
                    showCloseButton: true
                });
            }
            else {
                $scope.PlaceBookingObj.Type = "Confirm";
                $scope.PlaceBookingObj.BookingStatus = 'Current';
                $scope.PlaceBookingObj.CollectionDate = $scope.CollectionDate;
                $scope.PlaceBookingObj.CollectionTime = $scope.CollectionTime;
                if ($scope.IsCollection === true) {
                    $scope.PlaceBookingObj.IsCollection = $scope.IsCollection;
                }
                $scope.PlaceBookingObj.Message = $scope.Message;
                $uibModalInstance.close($scope.PlaceBookingObj);
            }
        }
        else {
            $scope.PlaceBookingObj.Type = "Confirm";
            $scope.PlaceBookingObj.BookingStatus = 'Current';
            $scope.PlaceBookingObj.CollectionDate = $scope.CollectionDate;
            $scope.PlaceBookingObj.CollectionTime = $scope.CollectionTime;
            if ($scope.IsCollection === true) {
                $scope.PlaceBookingObj.IsCollection = $scope.IsCollection;
            }
            $scope.PlaceBookingObj.Message = $scope.Message;
            $uibModalInstance.close($scope.PlaceBookingObj);
        }
    };

    $scope.GetDateDay = function (CollectionDate) {
        var date = CollectionDate;
        if (date !== undefined && date !== null && date !== '') {
            if (date.getDay() === 6) {
                $scope.Message = true;
            }
            else {
                $scope.Message = false;
            }
        }
        var todayDate = new Date();
        var colleDate = new Date(CollectionDate);

        if (todayDate < colleDate) {
            $scope.IsCollectionDateTime = true;
        }
        else {
            $scope.IsCollectionDateTime = false;
        }
    };

    function init() {

        $scope.IsCollectionDateTime = false;
        $scope.IsCollection = false;

        $scope.PlaceBookingObj = {
            directBookingForm: {},
            BookingStatus: "",
            isValid: false,
            CollectionTime: "",
            CollectionDate: new Date(),
            Type: "",
            IsCollection: false
        };

        $scope.FromCountryId = ShipFromCountryId;

        $scope.status = {
            opened: false
        };

        $scope.dateOptions = {
            formatYear: 'yy',
            startingDay: 1,
            minDate: new Date(),
            dateDisabled: function (data) {
                var date = data.date,
                mode = data.mode;
                return mode === 'day' && (date.getDay() === 0);
            }
        };

        //|| date.getDay() === 6

        $scope.IsShowColleDateMsg = false;
        $scope.CollectionDate = new Date();
        setModalOptions();
    }
    init();
});