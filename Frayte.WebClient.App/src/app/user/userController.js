/** 
 * Controller
 */
angular.module('ngApp.user').controller('UserController', function ($scope, $state, UtilityService, $location, $filter, AppSpinner, $translate, UserService, SessionService, $uibModal, uiGridConstants, toaster, CountryService, ModalService) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['User', 'DeleteHeader', 'staff', 'DeleteBody', 'Detail', 'FrayteError', 'ErrorDeletingRecord', 'ErrorGetting', 'records',
        'LoadingUsers']).then(function (translations) {
            $scope.headerTextUser = translations.User + " " + translations.DeleteHeader;
            //$scope.bodyTextUser = translations.DeleteBody + " " + translations.This + " " + translations.user + " " + translations.Detail;
            $scope.bodyTextUser = translations.DeleteBody + " " + translations.User + " " + translations.Detail + "?";
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TextErrorDeletingRecord = translations.ErrorDeletingRecord;
            $scope.TextErrorGetting = translations.ErrorGetting + " " + translations.staff + " " + translations.User + " " + translations.records;
            $scope.CancelConfirmation = translations.Cancel + " " + translations.Confirmation;
            $scope.CancelValidation = translations.Cancel_Validation;
            $scope.LoadingUsers = translations.LoadingUsers;

            getScreenInitials();
        });
    };

    $scope.AddEditUser = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'user/userDetail.tpl.html',
            controller: 'UserDetailController',
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                UserId: function () {
                    if (row === undefined) {
                        return 0;
                    }
                    else {
                        return row.entity.UserId;
                    }
                },
                SystemRoles: function () {
                    return $scope.SystemRoles;
                },
                RoleId: function () {
                    return $scope.track.RoleId;
                }
            }
        });


        modalInstance.result.then(function (userDetail) {
            if (userDetail !== null && userDetail.RoleId >1) {
                $scope.track.RoleId = userDetail.RoleId;
            }
            $scope.LoadStaffUsers();
        }, function () {
        });
    };

    // Mobile configuration 
    $scope.MobileConfiguration = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'user/userMobileConfiguration/userMobileConfiguration.tpl.html',
            controller: 'MobileTrackingConfiguration',
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                UserId: function () {
                    if (row === undefined) {
                        return 0;
                    }
                    else {
                        return row.entity.UserId;
                    }
                }
            }
        });

        modalInstance.result.then(function (userDetail) {
            $scope.LoadStaffUsers(); 
        }, function () {
            $scope.LoadStaffUsers();

        });
    };

    $scope.RemoveUser = function (row) {

        var modalOptions = {
            headerText: $scope.headerTextUser,
            //bodyText: "Are you sure want to delete user Details?"
            bodyText: $scope.bodyTextUser
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            UserService.DeleteUser(row.entity.UserId).then(function (response) {
                if (response.data.Status) {
                    var index = $scope.gridOptions.data.indexOf(row.entity);
                    $scope.gridOptions.data.splice(index, 1);
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
          { name: 'ContactName', displayName: 'Name', headerCellFilter: 'translate', width: '15%' },
          { name: 'RoleName', displayName: 'Role', headerCellFilter: 'translate', width: '10%' },
          { name: 'Email', displayName: 'Email', headerCellFilter: 'translate', width: '20%' },
          { name: 'TelephoneNo', displayName: 'Phone_No', headerCellFilter: 'translate', width: '13%' },
          //{ name: 'Skype', displayName: 'Skype No', headerCellFilter: 'translate' },
          //{ name: 'FaxNumber', displayName: 'Fax No', headerCellFilter: 'translate' },
          { name: 'ManagerUser.ContactName', displayName: 'Manager_Name', headerCellFilter: 'translate', width: '15%' },
          { name: 'ManagerUser.Email', displayName: 'Manager_Email', headerCellFilter: 'translate', width: '16%' },
          { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "user/userEditButton.tpl.html" }
        ]
    };

    $scope.LoadStaffUsers = function () {
        AppSpinner.showSpinnerTemplate($scope.LoadingUsers, $scope.Template);
        UserService.GetUserList($scope.track).then(function (response) {
            if (response.data && response.data.length) {
                $scope.gridOptions.data = response.data;
                $scope.totalItemCount = response.data[0].TotalRows;
                //For full List/Data in UI-GRID
                $scope.gridOptions.excessRows = $scope.gridOptions.data.length;
            }
            else {
                $scope.gridOptions.data = [];
            }

            AppSpinner.hideSpinnerTemplate();
        }, function (error) {
            AppSpinner.hideSpinnerTemplate();
            if (error.status !== 401) {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorGetting,
                    showCloseButton: true
                });
             }
        });
    };

    $scope.toggleRowSelection = function () {
        $scope.gridApi.selection.clearSelectedRows();
        $scope.gridOptions.enableRowSelection = !$scope.gridOptions.enableRowSelection;
        $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS);
    };

    var getUserTab = function (tabs, tabKey) {
        if (tabs !== undefined && tabs !== null && tabs.length) {
            var tab = {};
            for (var i = 0; i < tabs.length; i++) {
                if (tabs[i].tabKey === tabKey) {
                    tab = tabs[i];
                    break;
                }
            }
            return tab;
        }
    };

    $scope.pageChanged = function () {
        $scope.LoadStaffUsers();
    };

    var getScreenInitials = function () {
        AppSpinner.showSpinnerTemplate($scope.LoadingUsers, $scope.Template);
        UserService.GetSystemRoles($scope.UserId).then(function (response) {
            if (response.data) {
                $scope.SystemRoles = response.data;
                angular.forEach($scope.SystemRoles, function (obj) {
                    if (obj.IsDefault) {
                        $scope.track.RoleId = obj.RoleId;
                    }
                });

                $scope.LoadStaffUsers();
            }
            else {
                AppSpinner.hideSpinnerTemplate();
            }
        }, function (reason) {
            AppSpinner.hideSpinnerTemplate();
            if (reason.status !== 401) {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorGetting,
                    showCloseButton: true
                });
            }
        });
    };

    function init() {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        //$scope.spinnerMessage = $scope.LoadingUsers;
        // Pagination Logic 
        $scope.viewby = 50;
        $scope.currentPage = 1;
        $scope.itemsPerPage = $scope.viewby;
        $scope.totalItemCount = 1000;

        $scope.maxSize = 2; //Number of pager buttons to show
        $scope.track = {
            RoleId: 0,
            CurrentPage: $scope.currentPage,
            TakeRows: $scope.itemsPerPage
        };

        var userInfo = SessionService.getUser();
        $scope.UserId = userInfo.EmployeeId;
        $scope.tabs = userInfo.tabs;
        $scope.tab = getUserTab($scope.tabs, "Users");
        // set Multilingual Modal Popup Options
        setModalOptions();
        $scope.gridheight = SessionService.getScreenHeight();
        //getScreenInitials();

        $scope.gridOptions.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
        };
    }

    init();

});