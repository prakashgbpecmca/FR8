angular.module("ngApp.hsCodejobs")
       .controller("OperatorJobsController", function ($scope, $uibModalInstance,UtilityService, AppSpinner, JobService, OperatorId, toaster, $translate) {
           $scope.GetCorrectFormattedDatePanel = function (date) {
               return UtilityService.GetForMattedDate(date);
           };
           $scope.GetCorrectFormattedTime = function (time) {
               return UtilityService.GetFormattedtime(time);
           };
           var setMultilingualOptions = function () {
               $translate(['FrayteError', 'FrayteWarning', 'ErrorGetting', 'ThereIsNoUnassignedJob', 'jobs', 'ThereIsNoAssignedJob', 'LoadingJobs']).then(function (translations) {
                   $scope.FrayteError = translations.FrayteError;
                   $scope.FrayteWarning = translations.FrayteWarning;
                   $scope.ThereIsNoUnassignedJob = translations.ThereIsNoUnassignedJob;
                   $scope.ThereIsNoAssignedJob = translations.ThereIsNoAssignedJob;
                   $scope.ErrorGettingjobs = translations.ErrorGetting + " " + translations.jobs;
                   $scope.LoadingJobs = translations.LoadingJobs;
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

           var getScreenInitials = function () {
               AppSpinner.showSpinnerTemplate($scope.LoadingJobs, $scope.Template);
               JobService.GetAssignedJobs($scope.track).then(function (response) {
                   $scope.SelectAll = false;
                   if (response.status === 200 && response.data !== null) {
                       if (response.data.length) {
                           $scope.jobs = response.data;
                           $scope.totalItemCount = response.data[0].TotalRows;
                           for (var i = 0; i < $scope.jobs.length; i++) {
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

                       AppSpinner.hideSpinnerTemplate();
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
               }, function () {
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
           $scope.changePagination = function () {
               $scope.pageChanged();
           };

           $scope.pageChanged = function () {
               getScreenInitials("PageChanged");
           };
           function init() {
               // will get operator id from parent controller
               if (OperatorId) {
                   $scope.OperatorId = OperatorId;
               }

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
                   OperatorId: $scope.OperatorId ? $scope.OperatorId : 0,
                   DestinationCountry: 0
               };

               $scope.OpeartorJob = {
                   OperatorId: $scope.OperatorId,
                   jobs: []
               };

               $scope.Template = 'directBooking/ajaxLoader.tpl.html';
               getScreenInitials();
               setMultilingualOptions();
           }
           init();
       });