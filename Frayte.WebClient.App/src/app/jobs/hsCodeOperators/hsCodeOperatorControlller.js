angular.module("ngApp.hsCodejobs").controller("HSCodeOperatorsController", function ($scope,$uibModal, JobService, UserId, toaster, AppSpinner, $translate) {

    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'ErrorGetting', 'operators', 'LoadingOperators']).then(function (translations) {
            $scope.FrayteError = translations.FrayteError;
            $scope.ErrorGettingoperators = translations.ErrorGetting + " " + translations.operators;            
            $scope.LoadingOperators = translations.LoadingOperators;
        });
    };
    $scope.operatorsJob = function (OperatorId) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'jobs/operatorJobs/operatorJob.tpl.html',
            controller: 'OperatorJobsController',
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                OperatorId: function () {
                    return OperatorId;
                }
            }

        });
    };

    var getOperatorsWithJobs = function () {
        AppSpinner.showSpinnerTemplate($scope.LoadingOperators, $scope.Template);
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
            if (response.status !== 401) {
                toaster.pop({
                    type: "error",
                    title: $scope.FrayteError,
                    body: $scope.ErrorGettingoperators,
                    showCloseButton: true
                });
            }
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