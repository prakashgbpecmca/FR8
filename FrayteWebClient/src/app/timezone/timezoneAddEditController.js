/**
 * Controller
 */
angular.module('ngApp.timezone').controller('TimeZoneAddEditController', function ($scope, $location,$translate, TimeZoneService, SessionService, $uibModal, $uibModalInstance, timezones, timezone, $log, toaster, mode) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation']).then(function (translations) {
            
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;         
	
            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
            $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            });
    };

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

    $scope.GlobalTimezone = {
        SelectedTimezone: {}
    };

    $scope.timezones = timezones;

    $scope.timezoneDetail = {
        TimezoneId: timezone.TimezoneId,
        Name: timezone.Name,
        Offset: timezone.Offset,
        OffsetShort: timezone.OffsetShort
    };

    $scope.submit = function (isValid, timezoneDetail) {
        if (isValid) {
            var timezoneId = timezoneDetail.TimezoneId;
            TimeZoneService.SaveTimeZone(timezoneDetail).then(function (response) {
                if (timezoneId === undefined || timezoneId === 0) {
                    //Here we need to add the data in $scope.timezones
                    timezoneDetail.TimezoneId = response.data.TimezoneId;
                    $scope.timezones.push(timezoneDetail);
                }
                else {
                    //Need to update the timezones collection and then return back to main grid
                    $scope.updateTimeZone(timezoneDetail);
                }

                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteInformation,
                    body: $scope.TextSuccessfullySavedInformation,
                    showCloseButton: true
                });

                $uibModalInstance.close($scope.timezones);

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

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.updateTimeZone = function (timezoneDetail) {
        var objects = $scope.timezones;

        for (var i = 0; i < objects.length; i++) {
            if (objects[i].TimezoneId === timezoneDetail.TimezoneId) {
                objects[i] = timezoneDetail;
                break;
            }
        }
    };

    $scope.SetTimezone = function () {
        $scope.timezoneDetail = {
            TimezoneId: 0,
            Name: $scope.GlobalTimezone.SelectedTimezone.Name,
            Offset: $scope.GlobalTimezone.SelectedTimezone.Offset
        };
    };

    $scope.GlobalTimezones = [
        { Name: 'IST-Indian Starndard Time', Offset: '+5:30' },
        { Name: 'ISL-Indian Starndard Long', Offset: '+4:30' }
    ];

    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();       
    }

    init();

});