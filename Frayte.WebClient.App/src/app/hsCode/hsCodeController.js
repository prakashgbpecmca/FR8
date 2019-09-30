
angular.module('ngApp.hsCode')
      .controller('HSCodeController', function ($scope, UtilityService, ModalService, toaster, HSCodeService, JobService, SessionService, AppSpinner, $translate) {

          // Multilngual 
          $scope.GetCorrectFormattedDatePanel = function (date) {
              return UtilityService.GetForMattedDate(date);
          };
          $scope.GetCorrectFormattedTime = function (time) {
              return UtilityService.GetFormattedtime(time);
          };
          var setMultilingualOptions = function () {
              $translate(['FrayteInformation', 'FrayteWarning', 'FrayteError', 'FRAYTE_HSCode_Error', 'ErrorGetting', 'operators', 'jobs', 'FrayteError_NoOperator', 'ErrorWhileAssigninJob', 'PleaseSelectJobFirst', 'SuccessfullyAssignJob', 'PleaseOperatorFromDropdown', 'ThereIsNoAssignedJob',
              'Confirmation', 'RefreshChangesHSCodeMapping', 'UpdatingHSCode', 'LoadingJobs']).then(function (translations) {
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
                  $scope.Confirmation = translations.Confirmation;
                  $scope.RefreshChangesHSCodeMapping = translations.RefreshChangesHSCodeMapping;
                  $scope.UpdatingHSCode = translations.UpdatingHSCode;
                  $scope.LoadingJobs = translations.LoadingJobs;
              });
          };

          // HS Code Look UP

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


              // Keep the first job open

              $scope.jobs.sort(function (a, b) { return a.OrderNumber - b.OrderNumber; });
              if (!$scope.jobs[0].IsHsCodeSet) {
                  $scope.jobs[0].collapse = false;
                  $scope.jobs[0].collapseToggle = true;
              }
              //if ($scope.jobs && $scope.jobs.length) {
              //    angular.forEach($scope.jobs, function (obj) { 
              //        if (!obj.IsHsCodeSet) { 
              //        } 
              //    }); 
              //}

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
                  setHSCode();
                  AppSpinner.hideSpinnerTemplate();
                  Package.IsHSCodeSet = false;
                  toaster.pop({
                      type: 'error',
                      title: $scope.FrayteError,
                      body: $scope.FRAYTE_HSCode,
                      showCloseButton: true
                  });
              });
          };
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
          $scope.allJobsEvent = function () {
              angular.forEach($scope.jobs, function (eachObj) {
                  eachObj.IsSelected = $scope.SelectAll;
              });
          };


          $scope.refreshScreen = function () {

              var modalOptions = {
                  headerText: $scope.Confirmation,
                  bodyText: $scope.RefreshChangesHSCodeMapping
              };

              ModalService.Confirm({}, modalOptions).then(function (result) {

                  // 
                  getScreenInitials();

              }, function () {

              });

          };
          var getScreenInitials = function (Type) {
              AppSpinner.showSpinnerTemplate($scope.LoadingJobs, $scope.Template);
              JobService.GetAssignedJobs($scope.track).then(function (response) {
                  $scope.SelectAll = false;
                  AppSpinner.hideSpinnerTemplate();
                  if (response.status === 200 && response.data !== null) {
                      if (response.data.length) {
                          $scope.jobs = response.data;
                          $scope.totalItemCount = response.data[0].TotalRows;
                          for (var i = 0; i < $scope.jobs.length; i++) {
                              $scope.jobs[i].collapseToggle = false;
                              $scope.jobs[i].IsSelected = false;
                              $scope.jobs[i].collapse = true;
                              $scope.jobs[i].IsHsCodeSet = false;
                              $scope.jobs[i].OrderNumber = i + 1;
                          }

                          setHSCode();

                      } else {
                          $scope.jobs = [];

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

          $scope.Search = function () {
              $scope.jobs = [];
              getScreenInitials("Search");
          };
          $scope.pageChanged = function () {
              getScreenInitials("PageChanged");
          };
          $scope.changePagination = function () {
              $scope.pageChanged();
          };

          function init() {

              var userInfo = SessionService.getUser();
              if (userInfo) {
                  $scope.UserId = userInfo.EmployeeId;
              }
              else {
                  // redirect to log in page

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
                  AllShipments: false,
                  CurrentPage: $scope.currentPage,
                  TakeRows: $scope.itemsPerPage,
                  OperatorId: $scope.UserId,
                  DestinationCountry: 0
              };

              $scope.OpeartorJob = {
                  OperatorId: null,
                  jobs: []
              };

              $scope.Template = 'directBooking/ajaxLoader.tpl.html';

              getScreenInitials();

              setMultilingualOptions();

          }
          init();
      });
