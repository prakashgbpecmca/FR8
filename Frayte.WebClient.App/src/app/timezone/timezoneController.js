
angular.module('ngApp.timezone').controller('TimeZoneController', function ($scope, $state, $window, $location, $filter, $translate, TimeZoneService, SessionService, $uibModal, $log, uiGridConstants, toaster, ModalService) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['Timezone', 'DeleteHeader', 'This', 'DeleteBody', 'Detail', 'FrayteError', 'FrayteInformation', 'ErrorDeletingRecord', 'ErrorGetting', 'SuccessfullyDelete', 'Timezone', 'The', 'information']).then(function (translations) {
            $scope.headerTextTimezone = translations.Timezone + " " + translations.DeleteHeader;
            $scope.bodyTextTimezone = translations.DeleteBody + " " + translations.This + " " + translations.Timezone + " " + translations.Detail;

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;

            $scope.TextErrorDeletingRecord = translations.ErrorDeletingRecord;

            $scope.TextErrorGettingTimeZoneRecord = translations.ErrorGetting + " " + translations.Timezone + " " + translations.records;
            $scope.TextSuccessfullyDelete = translations.SuccessfullyDelete + " " + translations.The + " " + translations.Timezone + " " + translations.information;

        });
    };

    $scope.AddEditTimeZone = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'timezone/timezoneAddEdit.tpl.html',
            controller: 'TimeZoneAddEditController',
            windowClass: 'AddEditTimezone-Modal',
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

                timezones: function () {
                    return $scope.timezones;
                },
                timezone: function () {
                    if (row === undefined) {
                        return {
                            TimezoneId: 0,
                            Name: '',
                            Offset: '',
                            OffsetShort: ''
                        };
                    }
                    else {
                        return row.entity;
                    }
                }
            }
        });

        modalInstance.result.then(function (timezones) {
            //To Dos : Here we need to write the code to add/edit the existing scope
            $scope.timezones = timezones;
        }, function () {
            //User cancled the pop-up            
        });
    };

    $scope.DeleteTimeZone = function (row) {
        var modalOptions = {
            headerText: $scope.headerTextTimezone,
            bodyText: $scope.bodyTextTimezone + '?'
        };
        ModalService.Confirm({}, modalOptions).then(function (result) {
            TimeZoneService.DeleteTimeZone(row.entity.TimezoneId).then(function (response) {
                if (response.data.Status) {
                    var index = $scope.gridOptions.data.indexOf(row.entity);
                    $scope.gridOptions.data.splice(index, 1);
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteInformation,
                        body: $scope.TextSuccessfullyDelete,
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
            data: $scope.timezones,
            columnDefs: [
              { name: 'Name', displayName: 'TimeZoneName', headerCellFilter: 'translate' },
              { name: 'OffsetShort', displayName: 'Offset', headerCellFilter: 'translate' },
              { name: 'Offset', displayName: 'OffsetDetail', headerCellFilter: 'translate' },
              { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "timezone/timezoneEditButton.tpl.html", width: 65 }
            ]
        };
    };


    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();
        $scope.gridheight = SessionService.getScreenHeight();
        $scope.SetGridOptions();
        TimeZoneService.GetTimeZoneList().then(function (response) {
            $scope.gridOptions.data = response.data;
            $scope.timezones = response.data;
            $scope.gridOptions.excessRows = $scope.gridOptions.data.length;

        }, function () {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGettingTimeZoneRecord,
                showCloseButton: true
            });
        });
    }

    init();

});