﻿angular.module("ngApp.hsCodejobs").controller("HSCodeJobsController", function ($scope, UtilityService, UserId, toaster, HSCodeService, uiGridConstants, JobService, AppSpinner, $interval, $translate) {
    $scope.GetCorrectFormattedDatePanel = function (date) {
        return UtilityService.GetForMattedDate(date);
    };
    $scope.GetCorrectFormattedTime = function (time) {
        return UtilityService.GetFormattedtime(time);
    };

    $scope.getHSCodes = function (query) {
        var Qlen = query.length;
        if (query !== undefined && query !== null && Qlen > 2) {

            return HSCodeService.GetHSCodes(query, 228).then(function (response) {
                return response.data;
            });
        }
    };

    var setHSCode = function () {
        angular.forEach($scope.jobs, function (obj) {
            var flag = true;
            if (obj.Packages.length) {
                angular.forEach(obj.Packages, function (obj1) {
                    if (!obj1.HSCode) {
                        flag = false;
                    }
                });
            }
            obj.IsHsCodeSet = flag;
            if (obj.IsHsCodeSet) {
                obj.collapse = true;
                obj.collapseToggle = false;
                obj.OrderNumber = $scope.jobs.length + 1;
            }
        });

    };
    $scope.onSelect = function ($item, $model, $label, $event, Package) {
        Package.HSCode = "";
        AppSpinner.showSpinnerTemplate($scope.UpdatingHSCode, $scope.Template);
        JobService.SetShipmentHSCodeNew(Package.eCommerceShipmentDetailId, $item.HSCode, "", $item.HsCodeId).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            if (response.data.Status) {
                Package.HSCode = $item.HSCode;
            }
            else {
                Package.IsHSCodeSet = false;
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.FRAYTE_HSCode,
                    showCloseButton: true
                });
            }
            setHSCode();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            setHSCode();
            Package.IsHSCodeSet = false;
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.FRAYTE_HSCode,
                showCloseButton: true
            });
        });
    };

    // HSCode
    $scope.detectHSCodeChange = function (Package, Type) {
        if (Package.IsHSCodeSet) {
            if (Type === "HSCode") {
                Package.Content = "";
            }
            else if (Type === "Content") {
                Package.HSCode = "";
            }
            JobService.SetShipmentHSCodeNew(Package.eCommerceShipmentDetailId, "", Package.Content, 0).then(function (response) {
                if (response.data.Status) {
                    Package.IsHSCodeSet = false;
                }
                else {
                    Package.IsHSCodeSet = true;
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteError,
                        body: $scope.FRAYTE_HSCode,
                        showCloseButton: true
                    });
                }
            }, function () {
                Package.IsPrinted = true;
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.FRAYTE_HSCode,
                    showCloseButton: true
                });
            });
        }
    };

    // Multilingual

    var setMultilingualOptions = function () {
        $translate(['FrayteInformation', 'FrayteWarning', 'FrayteError', 'FRAYTE_HSCode_Error', 'ErrorGetting', 'operators', 'jobs', 'FrayteError_NoOperator',
            'ErrorWhileAssigninJob', 'PleaseSelectJobFirst', 'SuccessfullyAssignJob', 'PleaseOperatorFromDropdown', 'ThereIsNoAssignedJob', 'UpdatingHSCode',
        'LoadingJobs', 'AssigningJobs']).then(function (translations) {
            $scope.Success = translations.FrayteInformation;
            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.FrayteError = translations.FrayteError;
            $scope.FRAYTE_HSCode = translations.FRAYTE_HSCode_Error;
            $scope.FrayteErrorNoOperator = translations.FrayteError_NoOperator;
            $scope.ErrorWhileAssigninJob = translations.ErrorWhileAssigninJob;
            $scope.PleaseSelectJobFirst = translations.PleaseSelectJobFirst;
            $scope.SuccessfullyAssignJob = translations.SuccessfullyAssignJob;
            $scope.PleaseOperatorFromDropdown = translations.PleaseOperatorFromDropdown;
            $scope.ThereIsNoAssignedJob = translations.ThereIsNoAssignedJob;
            $scope.ErrorGettingoperators = translations.ErrorGetting + " " + translations.operators;
            $scope.ErrorGettingjobs = translations.ErrorGetting + " " + translations.jobs;
            $scope.UpdatingHSCode = translations.UpdatingHSCode;
            $scope.LoadingJobs = translations.LoadingJobs;
            $scope.AssigningJobs = translations.AssigningJobs;
        });
    };

    //

    $scope.allJobsEvent = function () {
        angular.forEach($scope.jobs, function (eachObj) {
            eachObj.IsSelected = $scope.SelectAll;
        });
    };

    $scope.toggleRowSelect = function (data) {
        if (data) {
            data.RowSelect = true;
        }
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
                        body: $scope.FrayteErrorNoOperator,
                        showCloseButton: true
                    });
                }
                AppSpinner.hideSpinnerTemplate();
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
                title: $scope.FrayteError,
                body: $scope.ErrorGettingoperators,
                showCloseButton: true
            });
        });
    };
    var getDestinationCountries = function () {
        JobService.GetDestinationCountries($scope.UserId).then(function (response) {
            getMangerOperators();
            if (response.status === 200 && response.data) {
                $scope.Countries = response.data;
            }

        }, function () {
            getMangerOperators();
        });
    };
    var getScreenInitials = function (Type) {

        AppSpinner.showSpinnerTemplate($scope.LoadingJobs, $scope.Template);
        JobService.GetAssignedJobs($scope.track).then(function (response) {
            $scope.SelectAll = false;
            if (response.status === 200 && response.data !== null) {
                if (response.data.length) {
                    $scope.jobs = response.data;
                    $scope.totalItemCount = response.data[0].TotalRows;
                    $scope.gridOptions.data = $scope.jobs;
                    for (var i = 0; i < $scope.jobs.length; i++) {
                        $scope.jobs[i].OrderNumber = i + 1;
                        $scope.jobs[i].collapseToggle = false;
                        $scope.jobs[i].IsSelected = false;
                        $scope.jobs[i].collapse = true;
                        $scope.jobs[i].IsHsCodeSet = false;
                    }

                    setHSCodeProperty();

                } else {
                    toaster.pop({
                        type: "warning",
                        title: $scope.FrayteWarning,
                        body: $scope.ThereIsNoAssignedJob,
                        showCloseButton: true
                    });
                }
                if (Type === undefined) {
                    getDestinationCountries();
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

    var setHSCodeProperty = function () {
        angular.forEach($scope.jobs, function (obj) {
            var flag = true;
            if (obj.Packages.length) {
                angular.forEach(obj.Packages, function (obj1) {
                    if (!obj1.IsPrinted) {
                        flag = false;
                    }
                });
            }
            obj.IsHsCodeSet = flag;
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
        angular.forEach($scope.jobs, function (eachObj) {
            if (eachObj.IsSelected) {
                $scope.OpeartorJob.jobs.push(eachObj);
            }
        });

        if ($scope.OpeartorJob.jobs && $scope.OpeartorJob.jobs.length) {
            AppSpinner.showSpinnerTemplate($scope.AssigningJobs, $scope.Template);
            JobService.ReAssignJobs($scope.OpeartorJob).then(function (response) {
                angular.forEach($scope.jobs, function (eachObj) {
                    eachObj.IsSelected = false;
                });
                $scope.SelectAll = false;
                if (response.status === 200) {
                    if (response.data !== null && response.data.Status) {
                        toaster.pop({
                            type: "success",
                            title: $scope.Success,
                            body: $scope.SuccessfullyAssignJob,
                            showCloseButton: true
                        });
                        $scope.OpeartorJob.jobs = [];
                    }
                    else {

                        toaster.pop({
                            type: "error",
                            title: $scope.FrayteError,
                            body: $scope.ErrorWhileAssigninJob,
                            showCloseButton: true
                        });
                    }
                    AppSpinner.hideSpinnerTemplate();
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
            AppSpinner.hideSpinnerTemplate();
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
                  { name: 'FrayteNumber', displayName: 'Frayte Ref#', width: '20%', headerCellFilter: 'translate', enableFiltering: false, enableSorting: false },
                  { name: 'FromCountry', displayName: 'Origin', headerCellFilter: 'translate', width: '20%' },
                  { name: 'ToCountry', displayName: 'Destination', headerCellFilter: 'translate', width: '20%' },
                  { name: 'ShipmentDescription', displayName: 'Descrtption_of_goods', headerCellFilter: 'translate', width: '35%' },
                  { name: 'Edit', displayName: '', enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "jobs/jobsEditButton.tpl.html" }

            ]
        };
    };
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

    $scope.Search = function () {
        $scope.jobs = [];
        getScreenInitials("Search");
    };
    $scope.pageChanged = function () {
        getScreenInitials("PageChanged");
    };
    var getJobInProgressJobCount = function () {
        JobService.GetJobsInProgressCount().then(function (response) {
            if (response.status === 200 && response.data) {
                $scope.jobCount = response.data;
            }
        }, function () {

        });
    };
    $scope.changePagination = function () {
        $scope.pageChanged();
    };

    $scope.callAtInterval = function () {
        getJobInProgressJobCount("Intervel");
    };

    $scope.stop = function () {
        $interval.cancel($scope.promise1);
    };
    $scope.$on('$destroy', function () {
        $scope.stop();
    });
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
            AllShipments: true,
            CurrentPage: $scope.currentPage,
            TakeRows: $scope.itemsPerPage,
            OperatorId: 0,
            DestinationCountry: 0
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
        getJobInProgressJobCount();

        $scope.promise1 = $interval(function () { $scope.callAtInterval(); }, 20000);

        setMultilingualOptions();

    }
    init();
});