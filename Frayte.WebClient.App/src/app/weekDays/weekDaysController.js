
angular.module('ngApp.weekdays').controller('WeekDaysController', function ($scope, $state, WeekDaysService, $location, $filter, $translate, SessionService, $uibModal, $log, uiGridConstants, toaster, ModalService) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['WeekDay', 'Working', 'DeleteHeader', 'The', 'DeleteBody', 'Detail', 'FrayteError', 'FrayteInformation', 'SuccessfullyDelete', 'ErrorDeletingRecord', 'ErrorGetting', 'information', 'records']).then(function (translations) {
            $scope.headerTextWeekDay = translations.Working + " " + translations.WeekDay + " " + translations.DeleteHeader;
            $scope.bodyTextWeekDay = translations.DeleteBody + " " + translations.The + " " + translations.Working + " " + translations.WeekDay;

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;

            $scope.TextSuccessfullyDeleteWorkingWeekDay = translations.SuccessfullyDelete + " " + translations.The + " " + translations.Working + " " + translations.WeekDay + " " + translations.information;
            $scope.TextErrorDeletingRecord = translations.ErrorDeletingRecord;
            $scope.TextErrorGettingWeekdaysRecord = translations.ErrorGetting + " " + translations.WeekDay + " " + translations.records;
        });
    };

    $scope.AddEditWeekDays = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'weekDays/weekDaysAddEdit.tpl.html',
            controller: 'WeekDaysAddEditController',
            windowClass: 'WeekDays-Modal',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                mode: function () {
                    if (row === undefined) {
                        return 'Add';
                    }
                    else {
                        return 'Modify';
                    }
                },
                weekDaysList: function () {
                    return $scope.weekDataList;
                },
                singleWeekDay: function () {
                    if (row === undefined) {
                        return {
                            DayId: 0,
                            DayName: '',
                            DayHalfTime: null,
                            WorkingWeekDayDetailId: 0
                        };
                    }
                    else {
                        return row.entity;
                    }
                }
            }
        });

        modalInstance.result.then(function (saveStatus) {
            if (saveStatus === 'Save') {
                $scope.LoadWeekDays();
            }
        });
    };

    $scope.RemoveWeekDay = function (row) {
        var modalOptions = {
            headerText: $scope.headerTextWeekDay,
            bodyText: $scope.bodyTextWeekDay + " " + '(' + row.entity.Description + ') ?'
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            WeekDaysService.DeleteWorkingWeekDay(row.entity.WorkingWeekDayId).then(function (response) {
                if (response.status) {
                    var index = $scope.gridOptions.data.indexOf(row.entity);
                    $scope.gridOptions.data.splice(index, 1);
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteInformation,
                        body: $scope.TextSuccessfullyDeleteWorkingWeekDay,
                        showCloseButton: true
                    });
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteError,
                        body: response.data.Errors[0],
                        showCloseButton: true
                    });
                }
            }, function () {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorDeletingRecord,
                    showCloseButton: true
                });
            });
        });
    };

    $scope.ShowYesOrNo = function (row, field) {
        if (row === undefined) {
            return '';
        }
        else {
            var value = '';
            var data = row.entity.WorkingWeekDetails;
            for (var i = 0 ; i < data.length; i++) {
                if (data[i].DayName === field && (data[i].DayHalfTime === undefined || data[i].DayHalfTime === null)) {
                    value = "Yes";
                    break;
                }
                else {
                    value = "No";
                }
            }
            return value;
        }
    };

    $scope.ShowHalfTime = function (row, field) {
        if (row === undefined) {
            return '';
        }
        else {
            var value = '';
            var data = row.entity.WorkingWeekDetails;
            for (var i = 0 ; i < data.length; i++) {
                if (data[i].DayHalfTime != null) {
                    var str = data[i].DayHalfTime;
                    var str1 = str.slice(0, 2);
                    var str2 = str.slice(2, 4);
                    var str3 = str1 + ':' + str2;
                    value = "," + value + data[i].DayName.slice(0, 3) + '- ' + str3;
                }
            }
            return value.slice(1);
        }
    };

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
            enableVerticalScrollbar: uiGridConstants.scrollbars.NEVER,
            //columnDefs: [
            // { name: 'Description' },
            // { name: 'Monday', displayName: 'Mon', cellTemplate: '<div class="ui-grid-cell-contents">{{grid.appScope.ShowYesOrNo(row,col.field)}}</div>'},
            // { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "weekDays/weekDaysEditButton.tpl.html", width: 65 }
            //]
            columnDefs: [
              { name: 'Description', headerCellFilter: 'translate' },
              { name: 'Monday', displayName: 'Mon', headerCellFilter: 'translate', cellTemplate: '<div class="ui-grid-cell-contents">{{grid.appScope.ShowYesOrNo(row,col.field)}}</div>' },
              { name: 'Tuesday', displayName: 'Tue', headerCellFilter: 'translate', cellTemplate: '<div class="ui-grid-cell-contents">{{grid.appScope.ShowYesOrNo(row,col.field)}}</div>' },
              { name: 'Wednesday', displayName: 'Wed', headerCellFilter: 'translate', cellTemplate: '<div class="ui-grid-cell-contents">{{grid.appScope.ShowYesOrNo(row,col.field)}}</div>' },
              { name: 'Thursday', displayName: 'Thur', headerCellFilter: 'translate', cellTemplate: '<div class="ui-grid-cell-contents">{{grid.appScope.ShowYesOrNo(row,col.field)}}</div>' },
              { name: 'Friday', displayName: 'Fri', headerCellFilter: 'translate', cellTemplate: '<div class="ui-grid-cell-contents">{{grid.appScope.ShowYesOrNo(row,col.field)}}</div>' },
              { name: 'Saturday', displayName: 'Sat', headerCellFilter: 'translate', cellTemplate: '<div class="ui-grid-cell-contents">{{grid.appScope.ShowYesOrNo(row,col.field)}}</div>' },
              { name: 'Sunday', displayName: 'Sun', headerCellFilter: 'translate', cellTemplate: '<div class="ui-grid-cell-contents">{{grid.appScope.ShowYesOrNo(row,col.field)}}</div>' },
              { name: 'WorkingDayHalfTime', displayName: 'HalfDayTime', headerCellFilter: 'translate', cellTemplate: '<div class="ui-grid-cell-contents">{{grid.appScope.ShowHalfTime(row,col.field)}}</div>' },
              { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "weekDays/weekDaysEditButton.tpl.html", width: 65 }
            ]
        };
    };

    $scope.LoadWeekDays = function () {
        WeekDaysService.GetWeekDaysList().then(function (response) {
            $scope.gridOptions.data = response.data;
            $scope.weekDataList = response.data;
            $scope.gridOptions.excessRows = $scope.gridOptions.data.length;

        }, function () {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGettingWeekdaysRecord,
                showCloseButton: true
            });
        });
    };

    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();
        $scope.gridheight = SessionService.getScreenHeight();
        $scope.SetGridOptions();
        $scope.LoadWeekDays();
    }

    init();

});