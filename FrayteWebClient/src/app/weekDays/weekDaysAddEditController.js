/**
 * Service
 */
angular.module('ngApp.weekdays').controller('WeekDaysAddEditController', function ($scope, $state, singleWeekDay, $location, WeekDaysService, $filter, $translate, LogonService, SessionService, $uibModal, $uibModalInstance, $log, toaster, mode) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;

            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
        });
    };

    $scope.SetFullDayOrHalfDay = function (day, dayType) {
        var newdays = $scope.days;
        for (var i = 0 ; i < newdays.length; i++) {
            if (newdays[i].Id == day.Id) {
                if (dayType === 'FullDay' && day.isWeekFullDay) {
                    newdays[i].isWeekFullDay = true;
                    newdays[i].isWeekHalfDay = false;
                    newdays[i].halfTime = null;
                }

                if (dayType === 'HalfDay' && day.isWeekFullDay) {
                    newdays[i].isWeekFullDay = false;
                    newdays[i].isWeekHalfDay = true;
                    newdays[i].halfTime = null;
                }

                break;
            }
        }
    };

    $scope.SaveWorkingWeekDays = function (isValid, weekData, newdays) {
        if (isValid) {

            var editWorkingWeekDays = [];
            for (var i = 0; i < newdays.length; i++) {
                if (newdays[i].isWeekFullDay || newdays[i].isWeekHalfDay) {
                    var free = {
                        WorkingWeekDayDetailId: newdays[i].WorkingWeekDayDetailId,
                        DayId: newdays[i].Id,
                        DayName: newdays[i].Name,
                        DayHalfTime: newdays[i].halfTime
                    };

                    editWorkingWeekDays.push(free);
                }
            }

            if (editWorkingWeekDays.length > 0) {
                $scope.weekDetail.WorkingWeekDetails = editWorkingWeekDays;
            }

            WeekDaysService.SaveWeekDay($scope.weekDetail).then(function (response) {
                if (response.status === 200) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteInformation,
                        body: $scope.TextSuccessfullySavedInformation,
                        showCloseButton: true
                    });
                }

                $uibModalInstance.close('Save');

            }, function () {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorSavingRecord,
                    showCloseButton: true
                });
            });
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

    $scope.WorkingWeekDaysJson = function () {
        $scope.days = [
            {
                WorkingWeekDayDetailId: 0,
                Id: 1,
                Name: 'Monday',
                isWeekFullDay: false,
                isWeekHalfDay: false,
                halfTime: null


            },
            {
                WorkingWeekDayDetailId: 0,
                Id: 2,
                Name: 'Tuesday',
                isWeekFullDay: false,
                isWeekHalfDay: false,
                halfTime: null

            },
            {
                WorkingWeekDayDetailId: 0,
                Id: 3,
                Name: 'Wednesday',
                isWeekFullDay: false,
                isWeekHalfDay: false,
                halfTime: null
            },
            {
                WorkingWeekDayDetailId: 0,
                Id: 4,
                Name: 'Thursday',
                isWeekFullDay: false,
                isWeekHalfDay: false,
                halfTime: null

            },
            {
                WorkingWeekDayDetailId: 0,
                Id: 5,
                Name: 'Friday',
                isWeekFullDay: false,
                isWeekHalfDay: false,
                halfTime: null
            },
            {
                WorkingWeekDayDetailId: 0,
                Id: 6,
                Name: 'Saturday',
                isWeekFullDay: false,
                isWeekHalfDay: false,
                halfTime: null

            },
            {
                WorkingWeekDayDetailId: 0,
                Id: 7,
                Name: 'Sunday',
                isWeekFullDay: false,
                isWeekHalfDay: false,
                halfTime: null
            }
        ];
    };

    $scope.SetWorkingWeekDaysJson = function () {

        $scope.weekDetail = {
            WorkingWeekDayId: singleWeekDay.WorkingWeekDayId,
            Description: singleWeekDay.Description,
            IsDefault: singleWeekDay.IsDefault,
            WorkingWeekDetails: singleWeekDay.WorkingWeekDetails
        };

        var daysR = $scope.days;
        var dataWeekDetail = $scope.weekDetail.WorkingWeekDetails;

        //Set Main Days
        for (var i = 0; i < dataWeekDetail.length; i++) {
            for (var j = 0; j < daysR.length; j++) {
                if (dataWeekDetail[i].DayId === daysR[j].Id && (dataWeekDetail[i].DayHalfTime === undefined || dataWeekDetail[i].DayHalfTime === null)) {
                    daysR[j].WorkingWeekDayDetailId = dataWeekDetail[i].WorkingWeekDayDetailId;
                    daysR[j].isWeekFullDay = true;
                    daysR[j].isWeekHalfDay = false;
                    daysR[j].halfTime = null;
                }
                else if (dataWeekDetail[i].DayId === daysR[j].Id && (dataWeekDetail[i].DayHalfTime !== null || dataWeekDetail[i].DayHalfTime !== undefined)) {
                    daysR[j].WorkingWeekDayDetailId = dataWeekDetail[i].WorkingWeekDayDetailId;
                    daysR[j].isWeekFullDay = false;
                    daysR[j].isWeekHalfDay = true;
                    daysR[j].halfTime = dataWeekDetail[i].DayHalfTime;
                }
            }
        }

        $scope.days = daysR;
    };

    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();

        $scope.WorkingWeekDaysJson();

        if (mode === "Add") {
            $translate('Add').then(function (add) {
                $scope.mode = add;
            });

            $scope.weekDetail = {
                Description: "",
                IsDefault:false,
                WorkingWeekDayId: 0,
                WorkingWeekDetails: []
            };
        }
        else if (mode === "Modify") {
            $translate('Modify').then(function (modify) {
                $scope.mode = modify;
            });

            $scope.SetWorkingWeekDaysJson();
        }
    }

    init();


});