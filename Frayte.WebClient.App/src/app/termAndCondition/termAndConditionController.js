angular.module('ngApp.termandcondition').controller('TermAndConditionController', function (AppSpinner, $scope, $state, $translate, uiGridConstants, TermAndConditionService, config, $filter, LogonService, SessionService, $uibModal, toaster) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteError', 'ErrorGetting', 'TermsAndCondition', 'records', 'Loading_Terms_and_Conditions']).then(function (translations) {
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TextErrorGettingTermAndCondition = translations.ErrorGetting + " " + translations.TermsAndCondition;
            $scope.RecordGettingError = translations.ErrorGetting + " " + translations.records;
            $scope.LoadingTermsAndConditions = translations.Loading_Terms_and_Conditions;

            getAllTermAndCondition();
        });
    };

    $scope.AddEditTermAndCondition = function (row, ViewValue) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'termAndCondition/termAndConditionAddEdit.tpl.html',
            controller: 'TermAndConditionAddEditController',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                termAndConditionId: function () {
                    if (row === undefined) {
                        return 0;
                    }
                    else {
                        return row.entity.TermAndConditionId;
                    }
                },
                ViewValue: function () {
                    return ViewValue;
                },
                isMaxTermAndCondition: function () {
                    if (row !== undefined & row.entity.TermAndConditionId > 0) {

                        var maxValuePublic1 = Math.max.apply(Math, $scope.gridOptions.data.map(function (t) { if (t.TermAndConditionType === "Public" && t.OperationZoneId === 1) { return t.TermAndConditionId; } else { return 0; } }));
                        var maxValuePublic2 = Math.max.apply(Math, $scope.gridOptions.data.map(function (t) { if (t.TermAndConditionType === "Public" && t.OperationZoneId === 2) { return t.TermAndConditionId; } else { return 0; } }));

                        if (row.entity.TermAndConditionType === "Public" && row.entity.OperationZoneId === 1) {
                            return (row.entity.TermAndConditionId === maxValuePublic1);
                        }
                        else if (row.entity.TermAndConditionType === "Public" && row.entity.OperationZoneId === 2) {
                            return (row.entity.TermAndConditionId === maxValuePublic2);
                        }
                    }
                    else {
                        return false;
                    }
                }
            }
        });

        modalInstance.result.then(function () {
            init();
        }, function () {
        });
    };

    var temp = '<div class="ui-grid-cell-contents"><span ng-bind-html="row.entity[col.field]"></span></div>';
    $scope.SetGridOptions = function () {
        $scope.gridOptions = {
            showFooter: true,
            enableSorting: true,
            multiSelect: false,
            enableFiltering: true,
            enableRowSelection: true,
            enableSelectAll: false,
            enableRowHeaderSelection: false,
            selectionRowHeaderWidth: 35,
            noUnselect: true,
            enableGridMenu: false,
            enableColumnMenus: false,
            enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
            enableVerticalScrollbar: true,
            columnDefs: [
              { name: 'Detail', displayName: 'TermsAndCondition', headerCellFilter: 'translate', width: '72%', cellTemplate: temp },
              { name: 'CreatedOn', displayName: 'Created_On', headerCellFilter: 'translate', width: '17%', cellFilter: 'dateFilter:this' },
              { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "termAndCondition/termAndConditionEditButton.tpl.html" }
            ]
        };
    };

    var getAllTermAndCondition = function () {
        AppSpinner.showSpinnerTemplate($scope.LoadingTermsAndConditions, $scope.Template);
        if ($scope.OperationZone !== undefined && $scope.OperationZone !== null) {
            TermAndConditionService.GetAllTermsAndCondition($scope.OperationZone.OperationZoneId , $scope.userInfo.EmployeeId).then(function (response) {
                $scope.gridOptions.data = response.data;
                $scope.gridOptions.excessRows = $scope.gridOptions.data.length;
                AppSpinner.hideSpinnerTemplate();
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorGettingTermAndCondition,
                    showCloseButton: true
                });
            });
        }
    };

    //var getCurrentOperationZone = function () {
    //    TermAndConditionService.GetCurrentOperationZone().then(function (response) {
    //        if (response.data !== null) {
    //            $scope.OperationZone = response.data;
    //            getAllTermAndCondition();
    //        }
    //        else {
    //            toaster.pop({
    //                type: 'error',
    //                title: $scope.TitleFrayteError,
    //                body: $scope.TextErrorGettingTermAndCondition,
    //                showCloseButton: true
    //            });
    //        }
    //    }, function (reason) {
    //        if (reason.status !== 401) {
    //            toaster.pop({
    //                type: 'error',
    //                title: $scope.TitleFrayteError,
    //                body: $scope.TextErrorGettingTermAndCondition,
    //                showCloseButton: true
    //            });
    //        }
    //    });
    //};

    $scope.termAndConditionByOperationZone = function () {
        getAllTermAndCondition();
    };

    //var getOperationZone = function () {
    //    TermAndConditionService.GetOperationZoneList().then(function (response) {
    //        $scope.OperationZones = response.data;
    //    }, function (reason) {
    //        if (reason.status !== 401) {
    //            toaster.pop({
    //                type: 'error',
    //                title: $scope.TitleFrayteError,
    //                body: $scope.RecordGettingError,
    //                showCloseButton: true
    //            });
    //        }
    //    });
    //};

    $scope.toggleRowSelection = function () {
        $scope.gridApi.selection.clearSelectedRows();
        $scope.gridOptions.enableRowSelection = !$scope.gridOptions.enableRowSelection;
        $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS);
    };

    function init() {

        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        var userInfo = SessionService.getUser();
        $scope.userInfo = SessionService.getUser();
        if (userInfo !== undefined && userInfo !== null) {
            $scope.OperationZone = {
                OperationZoneId: userInfo.OperationZoneId,
                OperationZoneName: userInfo.OperationZoneName
            };            
        }
        //getOperationZone();
        //getCurrentOperationZone();
        // set Multilingual Modal Popup Options
        setModalOptions();
        $scope.gridheight = SessionService.getScreenHeight();
        $scope.SetGridOptions();
        $scope.gridOptions.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
        };
    }

    init();

});