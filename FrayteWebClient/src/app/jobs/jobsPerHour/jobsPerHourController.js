angular.module("ngApp.hsCodejobs").controller("JobsPerHourController", function ($scope, UserId, AppSpinner, JobService, toaster, $translate) {

    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'ErrorWhileGettingRecord']).then(function (translations) {
            $scope.FrayteError = translations.FrayteError;
            $scope.ErrorWhileGettingRecord = translations.ErrorWhileGettingRecord;

        });
    };

    var getOperatorsWithJobs = function () {
        AppSpinner.showSpinnerTemplate("Loading Operator's average jobs per hour.", $scope.Template);
        JobService.AvgJobsPerOperatorPerHour($scope.UserId).then(function (response) {
            if (response.status === 200) {
                if (response.data) {
                    $scope.OperatorsAvgJobs = response.data;
                }
            }
            else {
                toaster.pop({
                    type: "error",
                    title: $scope.FrayteError,
                    body: $scope.ErrorWhileGettingRecord,
                    showCloseButton: true
                });
            }
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            toaster.pop({
                type: "error",
                title: $scope.FrayteError,
                body: $scope.ErrorWhileGettingRecord,
                showCloseButton: true
            });
            AppSpinner.hideSpinnerTemplate();
        });
    };
    function init() {
        $scope.UserId = UserId;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';         
        getOperatorsWithJobs();
        setMultilingualOptions();
    }
    init();
});