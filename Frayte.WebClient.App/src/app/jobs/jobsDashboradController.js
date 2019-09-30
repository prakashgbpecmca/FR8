angular.module("ngApp.hsCodejobs").controller("JobsDashboradController", function ($scope, $uibModal, SessionService, toaster, JobService, AppSpinner, $interval, $translate) {

    //images path
    //$scope.totalJobsWithNoHsCodes = '../build/assets/total-no-hs-code.png';
    //$scope.totalOperators = '../build/assets/total_operator.png';
    //$scope.totalHsCodesOutput = '../build/assets/total-hs-code.png';
    //$scope.totalJobsInProgress = '../build/assets/total-jobs-in-progress.png';
    //end

    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'ErrorWhileLoadingPage', 'LoadingDashboard']).then(function (translations) {
            $scope.FrayteError = translations.FrayteError;
            $scope.ErrorWhileLoadingPage = translations.ErrorWhileLoadingPage;
            $scope.LoadingDashboard = translations.LoadingDashboard;
        });
    };


    //$scope.AvgJobsPerHour = function () {
    //    var modalInstance = $uibModal.open({
    //        animation: true,
    //        templateUrl: 'jobs/jobsPerHour/jobsPerHour.tpl.html',
    //        controller: 'JobsPerHourController',
    //        windowClass: 'DirectBookingDetail',
    //        size: 'md',
    //        backdrop: 'static',
    //        keyboard: false,
    //        resolve: {
    //            UserId: function () {
    //                return $scope.UserId;
    //            }
    //        }
    //    });
    //    modalInstance.result.then(function () {
    //        getScreenInitials();
    //    }, function () {
    //        getScreenInitials();
    //    });
    //};

    $scope.NoHSCodeJobs = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'jobs/jobsWithNoHSCodes/noHSCodeJobs.tpl.html',
            controller: 'NoHSCodeJobsController',
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                UserId: function () {
                    return $scope.UserId;
                }
            }
        });
        modalInstance.result.then(function () {
            getScreenInitials();
        }, function () {
            getScreenInitials();
        });
    };

    $scope.HSCodeOperators = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'jobs/hsCodeOperators/hsCodeOperator.tpl.html',
            controller: 'HSCodeOperatorsController',
            windowClass: 'DirectBookingDetail',
            size: 'md',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                UserId: function () {
                    return $scope.UserId;
                }
            }
        });
        modalInstance.result.then(function () {
            getScreenInitials();
        }, function () {
            getScreenInitials();
        });
    };

    $scope.JobsInProgress = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'jobs/hsCodeJobs/hsCodeJobs.tpl.html',
            controller: 'HSCodeJobsController',
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                UserId: function () {
                    return $scope.UserId;
                }
            }
        });
        modalInstance.result.then(function () {
            getScreenInitials();
        }, function () {
            getScreenInitials();
        });
    };

    var getScreenInitials = function (Type) {
        if (Type === undefined) {
            AppSpinner.showSpinnerTemplate($scope.LoadingDashboard, $scope.Template);
        }

        JobService.GetJobsDetails($scope.UserId).then(function (response) {
            if (response.status === 200) {
                $scope.JobDetail = response.data;
            }
            else {
                toaster.pop({
                    type: "error",
                    title: $scope.FrayteError,
                    body: $scope.ErrorWhileLoadingPage,
                    showCloseButton: true
                });
            }
            AppSpinner.hideSpinnerTemplate();
        }, function (response) {
            if (response.status !== 401) {
                toaster.pop({
                    type: "error",
                    title: $scope.FrayteError,
                    body: $scope.ErrorWhileLoadingPage,
                    showCloseButton: true
                });
            }
            AppSpinner.hideSpinnerTemplate();
        });

    };

    $scope.callAtInterval = function () {
        getScreenInitials("Intervel");
    };
    $scope.stop = function () {
        $interval.cancel($scope.promise);
    };
    $scope.$on('$destroy', function () {
        $scope.stop();
    });
    function init() {
        var userInfo = SessionService.getUser();
        $scope.UserId = userInfo.EmployeeId;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        getScreenInitials();
        setMultilingualOptions();
        $scope.promise = $interval(function () { $scope.callAtInterval(); }, 20000);

    }
    init();
});