angular.module("ngApp.hsCodejobs").controller("HSCodeOperatorsController", function ($scope, JobService, UserId, toaster, AppSpinner, $translate) {

    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'ErrorGetting', 'operators']).then(function (translations) {
            $scope.FrayteError = translations.FrayteError;
            $scope.ErrorGettingoperators = translations.ErrorGetting + " " + translations.operators;            

        });
    };

    var getOperatorsWithJobs = function () {
        AppSpinner.showSpinnerTemplate("Loading Operators", $scope.Template);
        JobService.OperatorsWithJobs($scope.UserId).then(function (response) {
            if (response.status === 200) {
                if (response.data) {
                    $scope.OperatorsWithJobs = response.data;
                }
            }
            else {
                toaster.pop({
                    type: "error",
                    title: $scope.FrayteError,
                    body: $scope.ErrorGettingoperators,
                    showCloseButton: true
                });
            }
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            toaster.pop({
                type: "error",
                title: $scope.FrayteError,
                body: $scope.ErrorGettingoperators,
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