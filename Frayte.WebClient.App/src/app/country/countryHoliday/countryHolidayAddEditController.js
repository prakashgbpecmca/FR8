/**
 * Controller
 */
angular.module('ngApp.country').controller('CountryHolidayAddEditController', function ($scope, $uibModal, publicHolidays, $translate, toaster, publicHoliday, mode, $uibModalInstance) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteValidation', 'PleaseCorrectValidationErrors']).then(function (translations) {

            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TextValidation = translations.PleaseCorrectValidationErrors;

        });
    };

    $scope.updateHoliday = function (holidayDetail) {
        var objects = $scope.publicHolidays;
        for (var i = 0; i < objects.length; i++) {
            if (objects[i].SN === holidayDetail.SN) {
                objects[i] = holidayDetail;
                break;
            }
        }
    };

    $scope.submit = function (isValid, holidayDetail) {
        if (isValid) {
            if (holidayDetail.SN === undefined || holidayDetail.SN === 0) {
                holidayDetail.SN = $scope.publicHolidays.length + 1;
                $scope.publicHolidays.push(holidayDetail);
            }
            else {
                //Need to update the holiday collection and then return back to main grid
                $scope.updateHoliday(holidayDetail);
            }

            $uibModalInstance.close($scope.publicHolidays);
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextValidation,
                showCloseButton: true
            });
        }
    };

    $scope.openCalender = function ($event) {
        $scope.status.opened = true;
    };

    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();

        //$scope.mode = mode;
        if (mode === "Add") {
            $translate('Add').then(function (add) {
                $scope.mode = add;
            });
        }
        if (mode === "Modify") {
            $translate('Modify').then(function (modify) {
                $scope.mode = modify;
            });
        }

        $scope.publicHolidays = publicHolidays;

        $scope.holidayDetail = {
            SN: publicHoliday.SN,
            CountryPublicHolidayId: publicHoliday.CountryPublicHolidayId,
            PublicHolidayDate: publicHoliday.PublicHolidayDate,
            Description: publicHoliday.Description
        };

        $scope.status = {
            opened: false
        };

        if ($scope.holidayDetail !== undefined && $scope.holidayDetail.PublicHolidayDate !== undefined && $scope.holidayDetail.PublicHolidayDate !== null) {
            $scope.holidayDetail.PublicHolidayDate = moment.utc($scope.holidayDetail.PublicHolidayDate).toDate();
        }
    }

    init();
});