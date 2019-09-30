
//angular.module('ngApp.hsCode')
//      .controller('HSCodeOperationController', function ($scope, UserId, toaster, HSCodeService, uiGridConstants, JobService, AppSpinner, $interval, $translate) {

//          // Multilngual 

//          var setMultilingualOptions = function () {
//              $translate(['FrayteInformation', 'FrayteWarning', 'FrayteError', 'FRAYTE_HSCode_Error', 'ErrorGetting', 'operators', 'jobs', 'FrayteError_NoOperator', 'ErrorWhileAssigninJob', 'PleaseSelectJobFirst', 'SuccessfullyAssignJob', 'PleaseOperatorFromDropdown', 'ThereIsNoAssignedJob']).then(function (translations) {
//                  $scope.Success = translations.FrayteInformation;
//                  $scope.FrayteWarning = translations.FrayteWarning;
//                  $scope.FrayteError = translations.FrayteError;
//                  $scope.FRAYTE_HSCode = translations.FRAYTE_HSCode_Error;
//                  $scope.FrayteErrorNoOperator = translations.FrayteError_NoOperator;
//                  $scope.ErrorWhileAssigninJob = translations.ErrorWhileAssigninJob;
//                  $scope.PleaseSelectJobFirst = translations.PleaseSelectJobFirst;
//                  $scope.SuccessfullyAssignJob = translations.SuccessfullyAssignJob;
//                  $scope.PleaseOperatorFromDropdown = translations.PleaseOperatorFromDropdown;
//                  $scope.ThereIsNoAssignedJob = translations.ThereIsNoAssignedJob;
//                  $scope.ErrorGettingoperators = translations.ErrorGetting + " " + translations.operators;
//                  $scope.ErrorGettingjobs = translations.ErrorGetting + " " + translations.jobs;

//              });
//          };
           
//          // HS Code Look UP

//          $scope.getHSCodes = function (query) {
//              var Qlen = query.length;
//              if (query !== undefined && query !== null && Qlen > 2) {

//                  return HSCodeService.GetHSCodes(query, 228).then(function (response) {
//                      return response.data;
//                  });
//              }
//          };
//          var setHSCode = function () {
//              angular.forEach($scope.jobs, function (obj) {
//                  var flag = true;
//                  if (obj.Packages.length) {
//                      angular.forEach(obj.Packages, function (obj1) {
//                          if (!obj1.HSCode) {
//                              flag = false;
//                          }
//                      });
//                  }
//                  obj.IsHsCodeSet = flag;
//              });
//          };
//          $scope.onSelect = function ($item, $model, $label, $event, Package) {
//              Package.HSCode = "";
//              JobService.SetShipmentHSCodeNew(Package.eCommerceShipmentDetailId, $item.HSCode, "", $item.HsCodeId).then(function (response) {
//                  if (response.data.Status) {
//                      Package.HSCode = $item.HSCode;
//                  }
//                  else {
//                      Package.IsHSCodeSet = false;
//                      toaster.pop({
//                          type: 'error',
//                          title: $scope.FrayteError,
//                          body: $scope.FRAYTE_HSCode,
//                          showCloseButton: true
//                      });
//                  }
//                  setHSCode();
//              }, function () {
//                  setHSCode();
//                  Package.IsHSCodeSet = false;
//                  toaster.pop({
//                      type: 'error',
//                      title: $scope.FrayteError,
//                      body: $scope.FRAYTE_HSCode,
//                      showCloseButton: true
//                  });
//              });
//          };
//          $scope.detectHSCodeChange = function (Package, Type) {
//              if (Package.IsHSCodeSet) {
//                  if (Type === "HSCode") {
//                      Package.Content = "";
//                  }
//                  else if (Type === "Content") {
//                      Package.HSCode = "";
//                  }
//                  JobService.SetShipmentHSCodeNew(Package.eCommerceShipmentDetailId, "", Package.Content, 0).then(function (response) {
//                      if (response.data.Status) {
//                          Package.IsHSCodeSet = false;
//                      }
//                      else {
//                          Package.IsHSCodeSet = true;
//                          toaster.pop({
//                              type: 'error',
//                              title: "Frayte-Error",
//                              body: "Could not set the HSCode. Try again.",
//                              showCloseButton: true
//                          });
//                      }
//                  }, function () {
//                      Package.IsPrinted = true;
//                      toaster.pop({
//                          type: 'error',
//                          title: "Frayte-Error",
//                          body: "Could not set the HSCode. Try again.",
//                          showCloseButton: true
//                      });
//                  });
//              }
//          };
        
//          $scope.allJobsEvent = function () {
//              angular.forEach($scope.jobs, function (eachObj) {
//                  eachObj.IsSelected = $scope.SelectAll;
//              });
//          }; 
         
//          var getScreenInitials = function (Type) {
//              AppSpinner.showSpinnerTemplate("Loading jobs", $scope.Template);
//              JobService.GetAssignedJobs($scope.track).then(function (response) {
//                  $scope.SelectAll = false;
//                  if (response.status === 200 && response.data !== null) {
//                      if (response.data.length) {
//                          $scope.jobs = response.data;
//                          $scope.totalItemCount = response.data[0].TotalRows; 
//                          for (var i = 0; i < $scope.jobs.length; i++) {
//                              $scope.jobs[i].collapseToggle = false;
//                              $scope.jobs[i].IsSelected = false;
//                              $scope.jobs[i].collapse = true;
//                              $scope.jobs[i].IsHsCodeSet = false;
//                          }

//                          setHSCodeProperty();

//                      } else {
//                          toaster.pop({
//                              type: "warning",
//                              title: $scope.FrayteWarning,
//                              body: $scope.ThereIsNoAssignedJob,
//                              showCloseButton: true
//                          });
//                      }
//                      if (Type === undefined) {
//                          getDestinationCountries();
//                      }
//                      else {
//                          AppSpinner.hideSpinnerTemplate();
//                      }
//                  }
//                  else {

//                      AppSpinner.hideSpinnerTemplate();

//                      toaster.pop({
//                          type: "error",
//                          title: $scope.FrayteError,
//                          body: $scope.ErrorGettingjobs,
//                          showCloseButton: true
//                      });
//                  }
//              }, function () {

//                  AppSpinner.hideSpinnerTemplate();

//                  toaster.pop({
//                      type: "error",
//                      title: $scope.FrayteError,
//                      body: $scope.ErrorGettingjobs,
//                      showCloseButton: true
//                  });
//              });
//          };

//          $scope.Search = function () {
//              $scope.jobs = [];
//              getScreenInitials("Search");
//          };
//          $scope.pageChanged = function () {
//              getScreenInitials("PageChanged");
//          };
//          $scope.changePagination = function () {
//              $scope.pageChanged();
//          };
        
//          function init() {

//              var userInfo = SessionService.getUser();
//              if (userInfo) {
//                  $scope.UserId = userInfo.EmployeeId;
//              }
//              else {
//                  // redirect to log in page

//              }
               
//              // Pagination Logic 
//              $scope.viewby = 50;
//              $scope.currentPage = 1;
//              $scope.itemsPerPage = $scope.viewby;
//              $scope.totalItemCount = 1000;
//              $scope.maxSize = 2; //Number of pager buttons to show
//              $scope.numbers = [$scope.viewby, 100, 200];

//              // Track obj
//              $scope.track = {
//                  FromDate: '',
//                  ToDate: '',
//                  CurrentPage: $scope.currentPage,
//                  TakeRows: $scope.itemsPerPage,
//                  OperatorId: $scope.UserId,
//                  DestinationCountry: 0
//              };

//              $scope.OpeartorJob = {
//                  OperatorId: null,
//                  jobs: []
//              };

//              $scope.Template = 'directBooking/ajaxLoader.tpl.html';
 
//              getScreenInitials(); 
 
//              setMultilingualOptions();

//          }
//          init();
//      });
