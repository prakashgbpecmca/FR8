angular.module("ngApp.hsCodejobs").controller("NoHSCodeJobsController", function ($scope,UtilityService, UserId, $uibModal, toaster, $uibModalInstance, uiGridConstants, JobService, AppSpinner, $translate) {
    $scope.GetCorrectFormattedDatePanel = function (date) {
        return UtilityService.GetForMattedDate(date);
    };
    $scope.GetCorrectFormattedTime = function (time) {
        return UtilityService.GetFormattedtime(time);
    };
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteWarning', 'PleaseOperatorFromDropdown', 'SuccessfullyAssignJob', 'ErrorWhileGettingRecord', 'ErrorGetting', 'operators', 'FrayteError_NoOperator',
            'ThereIsNoUnassignedJob', 'jobs', 'FrayteSuccess', 'ErrorWhileAssigninJob', 'PleaseSelectJobFirst', 'AssigningJobs', 'LoadingJobs']).then(function (translations) {
            $scope.FrayteError = translations.FrayteError;
            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.PleaseOperatorFromDropdown = translations.PleaseOperatorFromDropdown;
            $scope.ErrorWhileGettingRecord = translations.ErrorWhileGettingRecord;
            $scope.ErrorGettingoperators = translations.ErrorGetting + " " + translations.operators;
            $scope.FrayteError_NoOperator = translations.FrayteError_NoOperator;
            $scope.ThereIsNoUnassignedJob = translations.ThereIsNoUnassignedJob;
            $scope.ErrorGettingjobs = translations.ErrorGetting + " " + translations.jobs;
            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.ErrorWhileAssigninJob = translations.ErrorWhileAssigninJob;
            $scope.PleaseSelectJobFirst = translations.PleaseSelectJobFirst;
            $scope.SuccessfullyAssignJob = translations.SuccessfullyAssignJob;
            $scope.LoadingJobs = translations.LoadingJobs;
            $scope.AssigningJobs = translations.AssigningJobs;
        });
    };

    $scope.operatorsJob = function (Operator) {
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
                    return Operator;
                }
            }

        });
    };

    $scope.AssignJobs = function () {
        if (!$scope.OpeartorJob.OperatorId) {
            toaster.pop({
                type: "warning",
                title: $scope.FrayteWarning,
                body: $scope.PleaseOperatorFromDropdown,
                showCloseButton: true
            });
            return;
        }

        if ($scope.OpeartorJob && $scope.OpeartorJob.jobs && $scope.OpeartorJob.jobs.length) {
            AppSpinner.showSpinnerTemplate($scope.AssigningJobs, $scope.Template);
            JobService.AssignJobsToOperator($scope.OpeartorJob).then(function (response) {
                if (response.status === 200) {
                    if (response.data !== null && response.data.Status) {
                        toaster.pop({
                            type: "success",
                            title: $scope.FrayteSuccess,
                            body: $scope.SuccessfullyAssignJob,
                            showCloseButton: true
                        });
                        getOperatorsWithJobs();
                        for (var i = 0; i < $scope.jobs.length; i++) {
                            for (var j = 0; j < $scope.OpeartorJob.jobs.length; j++) {
                                if ($scope.jobs[i].ShipmentId === $scope.OpeartorJob.jobs[j].ShipmentId) {
                                    $scope.jobs.splice(i, 1);
                                }
                            }

                        }
                        $scope.OpeartorJob.jobs = [];
                    }
                    else {
                        AppSpinner.hideSpinnerTemplate();
                        toaster.pop({
                            type: "error",
                            title: $scope.FrayteError,
                            body: $scope.ErrorWhileAssigninJob,
                            showCloseButton: true
                        });
                    }
                }
                else {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: "error",
                        title: $scope.FrayteError,
                        body: $scope.ErrorWhileAssigninJob,
                        showCloseButton: true
                    });
                }
            }, function () {
                toaster.pop({
                    type: "error",
                    title: $scope.FrayteError,
                    body: $scope.ErrorWhileAssigninJob,
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: "warning",
                title: $scope.FrayteWarning,
                body: $scope.PleaseSelectJobFirst,
                showCloseButton: true
            });
        }

    };

    $scope.closePage = function () {
        $uibModalInstance.close();
    };
    $scope.pageChanged = function (track) {
        getScreenInitials("PageChanges");
    };

    var getOperatorsWithJobs = function () {
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
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: "error",
                title: $scope.FrayteError,
                body: $scope.ErrorGettingoperators,
                showCloseButton: true
            });
        });
    };

    var getMangerOperators = function () {
        JobService.MangerOperators($scope.UserId).then(function (response) {
            if (response.status === 200) {
                if (response.data && response.data.length) {
                    $scope.Operators = response.data;
                }
                else {
                    toaster.pop({
                        type: "warning",
                        title: $scope.FrayteWarning,
                        body: $scope.FrayteError_NoOperator,
                        showCloseButton: true
                    });
                }

                getOperatorsWithJobs();
            }
            else {

                AppSpinner.hideSpinnerTemplate();

                toaster.pop({
                    type: "error",
                    title: $scope.FrayteError,
                    body: $scope.ErrorGettingoperators,
                    showCloseButton: true
                });
            }
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: "error",
                title: $sc.FrayteError,
                body: $scope.ErrorGettingoperators,
                showCloseButton: true
            });
        });
    };
    var getScreenInitials = function (Type) {
        AppSpinner.showSpinnerTemplate($scope.LoadingJobs, $scope.Template);
        JobService.GetUnAssignedJobs($scope.track).then(function (response) {
            if (response.status === 200 && response.data !== null) {
                if (response.data.length) {
                    $scope.jobs = response.data;
                    $scope.totalItemCount = response.data[0].TotalRows;
                    $scope.gridOptions.data = $scope.jobs;

                } else {
                    toaster.pop({
                        type: "warning",
                        title: $scope.FrayteWarning,
                        body: $scope.ThereIsNoUnassignedJob,
                        showCloseButton: true
                    });
                }
                if (Type === undefined) {
                    getMangerOperators();
                }
                else {
                    AppSpinner.hideSpinnerTemplate();
                }

            }
            else {

                AppSpinner.hideSpinnerTemplate();

                toaster.pop({
                    type: "error",
                    title: $scope.FrayteError,
                    body: $scope.ErrorGettingjobs,
                    showCloseButton: true
                });
            }
        }, function (response) {

            AppSpinner.hideSpinnerTemplate();
            if (response.status !== 401) {
                toaster.pop({
                    type: "error",
                    title: $scope.FrayteError,
                    body: $scope.ErrorGettingjobs,
                    showCloseButton: true
                });
            }
        });

    };

    // Grid OPtions
    var setGridOptions = function () {
        $scope.gridOptions = {
            showFooter: true,
            enableSorting: true,
            multiSelect: true,
            enableFiltering: true,
            enableRowSelection: true,
            enableSelectAll: true,
            enableRowHeaderSelection: true,
            selectionRowHeaderWidth: 35,
            enableGridMenu: false,
            enableColumnMenus: false,
            enableVerticalScrollbar: true,
            enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
            columnDefs: [
                  { name: 'FrayteNumber', displayName: 'Frayte_ShipmentNo', width: '20%', headerCellFilter: 'translate', enableFiltering: false, enableSorting: false },
                  { name: 'FromCountry', displayName: 'Origin', headerCellFilter: 'translate', width: '10%' },
                  { name: 'ToCountry', displayName: 'Destination', headerCellFilter: 'translate', width: '10%' },
                  { name: 'ShipmentDescription', displayName: 'Description_Of_Goods', headerCellFilter: 'translate', width: '30%' },
                  { name: 'EstimatedDateOfDeparture  ', displayName: 'Estimated Date of Departure', cellTemplate: eTDDateTemplate, headerCellFilter: 'translate', width: '13%' },
                  { name: 'EstimatedTimeOfDeparture', displayName: 'Estimated Time of Departure', cellTemplate: eTDTimeTemplate, headerCellFilter: 'translate' }

            ]
        };

    };
    var eTDDateTemplate = '<div class="ui-grid-cell-contents"> {{grid.appScope.GetCorrectFormattedDatePanel(row.entity.EstimatedDateOfDeparture)}}</div>';
    var eTDTimeTemplate = '<div class="ui-grid-cell-contents">{{grid.appScope.GetCorrectFormattedTime(row.entity.EstimatedTimeOfDeparture)}}</div>';
   
    $scope.toggleRowSelection = function () {
        $scope.gridApi.selection.clearSelectedRows();
        $scope.gridOptions.enableRowSelection = !$scope.gridOptions.enableRowSelection;
        $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS);
    };
    $scope.selectButtonClick = function (row, $event) {
        row.isSelected = !row.isSelected;
    };
    $scope.rowSelection = function () {

        // Single row Selection
        $scope.gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.createval = true;
            if (row.isSelected === true) {
                $scope.OpeartorJob.jobs.push(row.entity);
            }
            else {

                for (i = 0; i < $scope.OpeartorJob.jobs.length; i++) {
                    if (row.entity.ShipmentId === $scope.OpeartorJob.jobs[i].ShipmentId) {
                        $scope.OpeartorJob.jobs.splice(i, 1);
                    }
                }
            }

        });

        $scope.createval = false;

        // Multiple row selections
        $scope.gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            $scope.createval = true;
            if (rows[0].isSelected === true) {
                for (i = 0; i < rows.length; i++) {
                    $scope.OpeartorJob.jobs.push(rows[i].entity);
                }
            }
            else {
                for (i = 0; i < $scope.jobs.length; i++) {
                    $scope.OpeartorJob.jobs = [];
                }
            }

        });
    };

    $scope.changePagination = function () {
        $scope.pageChanged();
    };

    function init() {

        $scope.UserId = UserId;

        // Pagination Logic 
        $scope.viewby = 50;
        $scope.currentPage = 1;
        $scope.itemsPerPage = $scope.viewby;
        $scope.totalItemCount = 1000;
        $scope.maxSize = 2; //Number of pager buttons to show
        $scope.numbers = [$scope.viewby, 100, 200];


        // Track obj
        $scope.track = {
            FromDate: '',
            ToDate: '',
            CurrentPage: $scope.currentPage,
            TakeRows: $scope.itemsPerPage
        };

        $scope.OpeartorJob = {
            OperatorId: null,
            jobs: []
        };

        $scope.Template = 'directBooking/ajaxLoader.tpl.html';

        setGridOptions();
        $scope.gridOptions.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
            $scope.rowSelection();
        };


        getScreenInitials();
        setMultilingualOptions();

    }
    init();
});