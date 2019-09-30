angular.module('ngApp.systemAlert')
       .controller('SystemAlertController', function ($scope,AppSpinner, SystemAlertService, ModalService, $state, $translate, uiGridConstants, TermAndConditionService, config, $filter, LogonService, SessionService, $uibModal, toaster, $rootScope, DateFormatChange) {

           //$scope.SearchSystemAlert = function () {

           //};
           var setModalOptions = function () {
               $translate(['System_Alert_Confirmation', 'Delete_The_Alert', 'Record_Deleted_Successfully', 'Frayte_Success', 'Frayte_Error', 'Error_While_Deleting_The_Records', 'Error_While_Getting_Record']).then(function (translations) {

                   $scope.SystemAlertConfirmation = translations.System_Alert_Confirmation;
                   $scope.DeleteTheAlert = translations.Delete_The_Alert;
                   $scope.RecordDeletedSuccessfully = translations.Record_Deleted_Successfully;
                   $scope.FrayteSuccess = translations.Frayte_Success;
                   $scope.FrayteError = translations.Frayte_Error;
                   $scope.ErrorWhileDeletingTheRecords = translations.Error_While_Deleting_The_Records;
                   $scope.ErrorWhileGettingRecord = translations.Error_While_Getting_Record;
              


               });
           };

           $scope.ChangeFromdate = function (FromDate) {

               var newdate = [];

               newdate = new Date(FromDate);
               var gtDate = newdate.getDate();
               var gtDate1 = ++gtDate;
               var gtMonth = newdate.getMonth();
               var month1 = ++gtMonth;
               var gtYear = newdate.getFullYear();
               var nDate = month1 + "/" + gtDate1 + "/" + gtYear;

               $scope.trackSystemAlert.FromDate = new Date(nDate);
           };

           $scope.ChangeTodate = function (ToDate) {

               var newdate = [];

               newdate = new Date(ToDate);
               var gtDate = newdate.getDate();
               var gtDate1 = ++gtDate;
               var gtMonth = newdate.getMonth();
               var month1 = ++gtMonth;
               var gtYear = newdate.getFullYear();
               var nDate = month1 + "/" + gtDate1 + "/" + gtYear;

               $scope.trackSystemAlert.ToDate = new Date(nDate);
           };

           $scope.openCalender = function ($event) {
               $scope.status.opened = true;
           };

           $scope.openCalender1 = function ($event) {
               $scope.status1.opened = true;
           };

           $scope.status1 = {
               opened: false
           };

           $scope.status = {
               opened: false
           };

           $scope.pageChanged = function (TrackManifest) {
               $scope.GetManifest();
           };

           $scope.DeleteSystemAlert = function (row) {
               if (row !== undefined) {
                   var modalOptions = {
                       headerText: $scope.SystemAlertConfirmation,
                       bodyText: $scope.DeleteTheAlert
                   };

                   ModalService.Confirm({}, modalOptions).then(function (result) {
                       SystemAlertService.DeleteSystemAlert(row.entity.SystemAlertId).then(function (response) {
                           $rootScope.GetSystemAlert($scope.CurrentOperationZoneId);
                           init();
                           toaster.pop({
                               type: 'success',
                               title: $scope.FrayteSuccess,
                               body: $scope.RecordDeletedSuccessfully,
                               showCloseButton: true
                           });
                       }, function () {
                           toaster.pop({
                               type: 'error',
                               title: $scope.FrayteError,
                               body: $scope.ErrorWhileDeletingTheRecords,
                               showCloseButton: true
                           });
                       });
                   });
               }
             
           };
           $scope.AddEditSystemAlert = function (row) {
               var modalInstance = $uibModal.open({
                   animation: true,
                   templateUrl: 'setting/systemAlerts/systemAlertAddEdit.tpl.html',
                   controller: 'SystemAlertAddEditController',
                   size: 'lg',
                   backdrop: 'static',
                   resolve: {
                       systemAlertId: function () {
                           if (row === undefined) {
                               return 0;
                           }
                           else {
                               return row.entity.SystemAlertId;
                           }
                       },
                       systemAlert: function () {
                           if (row === undefined) {
                               return {
                                    SystemAlertId  :0,
                                    Heading        :'',
                                    Description    :'',
                                    FromDate       :'',
                                    Date           :'',
                                    ToDate         :'',
                                    TimeZoneDetail :null,
                                    IsActive       :true,
                                    OperationZoneId: $scope.OperationZone.OperationZoneId
                               };
                           }
                           else {
                            return row.entity;
                           }
                       },
                       mode: function () {
                           if (row === undefined) {
                               return "Add";
                           }
                           else {
                               return "Edit";
                           }
                       }
                   }
               });

               modalInstance.result.then(function () {
                   init();
               }, function () {

               });
           };

           $rootScope.getAllSystemAlerts = function () {
               AppSpinner.showSpinnerTemplate("Loading Service Alerts", $scope.Template);
               $scope.trackSystemAlert.OperationZoneId = $scope.OperationZone.OperationZoneId;
               SystemAlertService.GetAllSystemAlerts($scope.trackSystemAlert).then(function (response) {
                   if (response.data !== null || response.data !== undefined) {
                       for (i = 0; i < response.data.length; i++) {
                           response.data[i].FromDate = DateFormatChange.DateFormatChange(response.data[i].FromDate);
                           response.data[i].ToDate = DateFormatChange.DateFormatChange(response.data[i].ToDate);
                       }
                       $scope.gridOptions.data = response.data;
                       $scope.SystemAlerts = response.data;
                       if (response.data.length) {
                           $scope.totalItemCount = response.data[0].TotalRows;
                       }
                   }
                   AppSpinner.hideSpinnerTemplate();
               }, function () {
                   AppSpinner.hideSpinnerTemplate();
                   toaster.pop({
                       type: 'error',
                       title: $scope.FrayteError,
                       body: $scope.ErrorWhileGettingRecord,
                       showCloseButton: true
                   });
               });
           };
           var getCurrentOperationZone = function () {

               TermAndConditionService.GetCurrentOperationZone().then(function (response) {
                   if (response.data !== null) {
                       $scope.OperationZone = response.data;
                       $scope.CurrentOperationZoneId = response.data.OperationZoneId;
                       $rootScope.getAllSystemAlerts();
                   }
                   else {
                       toaster.pop({
                           type: 'error',
                           title: $scope.FrayteError,
                           body: $scope.ErrorWhileGettingRecord,
                           showCloseButton: true
                       });
                   }
               }, function () {
                   toaster.pop({
                       type: 'error',
                       title: $scope.FrayteError,
                       body: $scope.ErrorWhileGettingRecord,
                       showCloseButton: true
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
                   enableGridMenu: true,
                   enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
                   enableVerticalScrollbar: true,
                   columnDefs: [
                     { name: 'Heading', displayName: 'Alert_Heading', headerCellFilter: 'translate', width: '30%', cellTemplate: '<div class="ui-grid-cell-contents"><span ng-bind-html="row.entity[col.field]"></span></div>' },
                     { name: 'FromDate', displayName: 'From_Date', headerCellFilter: 'translate', width: '20%' },
                     { name: 'ToDate', displayName: 'To_Date', headerCellFilter: 'translate', width: '20%' },
                     { name: 'TimeZoneDetail.Name', displayName: 'TimeZone', headerCellFilter: 'translate', width: '20%' },
                     { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "setting/systemAlerts/systemAlertEditButton.tpl.html" }
                   ]
               };
           };

           $scope.toggleRowSelection = function () {
               $scope.gridApi.selection.clearSelectedRows();
               $scope.gridOptions.enableRowSelection = !$scope.gridOptions.enableRowSelection;
               $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS);
           };
           function init() {
               $scope.gridheight = SessionService.getScreenHeight();
               $scope.SetGridOptions();
               $scope.gridOptions.onRegisterApi = function (gridApi) {
                   $scope.gridApi = gridApi;
               };
               $scope.Template = 'directBooking/ajaxLoader.tpl.html';


               // Pagination Logic 
               $scope.viewby = 50;
               $scope.currentPage = 1;
               $scope.itemsPerPage = $scope.viewby;
               $scope.totalItemCount = 1000;

               $scope.maxSize = 2; //Number of pager buttons to show
               $scope.trackSystemAlert = {
                   OperationZoneId: 0,
                   FromDate: '',
                   ToDate: '',
                   CurrentPage: $scope.currentPage,
                   TakeRows: $scope.itemsPerPage
               };
               getCurrentOperationZone();
               setModalOptions();
           }

           init();
       });