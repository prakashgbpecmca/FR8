angular.module('ngApp.accessLevel')
   .controller("AccessLevelController", function ($scope, $state, AccessLevelService, SessionService, toaster, AppSpinner, $translate) {

       var setMultilingualOptions = function () {
           $translate(['SuccessfullyUpdated', 'CouldNotSetPermission', 'ErrorGettingRecord', 'FrayteError', 'FrayteSuccess', 'UpdatingAccessLevel', 'LoadingAccessLevel']).then(function (translations) {
               $scope.SuccessfullyUpdated = translations.SuccessfullyUpdated;
               $scope.CouldNotSetPermission = translations.CouldNotSetPermission;
               $scope.ErrorGettingRecord = translations.ErrorGettingRecord;
               $scope.FrayteError = translations.FrayteError;
               $scope.FrayteSuccess = translations.FrayteSuccess;
               $scope.UpdatingAccessLevel = translations.UpdatingAccessLevel;
               $scope.LoadingAccessLevel = translations.LoadingAccessLevel;

               // Initial Deatil call here --> so that Spinner Message should be multilingual
               getInitialDetail();
           });
       };

       $scope.setScroll = function () {
           var elements = document.getElementsByClassName("width80");
           var scroll = document.getElementById("scroll");
           angular.forEach(elements, function (value, key) {
               elements[key].scrollLeft = scroll.scrollLeft;
           });
       };
       $scope.callScroll = function () {
           var evt = document.getElementById("scroll");
           angular.element(document).ready(function () {
               var elements = document.getElementsByClassName("width80");
               angular.forEach(elements, function (value, key) {
                   elements[key].scrollLeft = 0;
               });
           });
       };
       $scope.getScrollPosition = function () {
           var scroll = document.getElementById("scroll");
           if (scroll) {
               return scroll.scrollLeft;
           }
       };

       $scope.setWidth = function (length) {
           if (length !== undefined && length !== null && length !== "0" && length !== 0 && length > 0) {
               return length * 18;
           }
           else {
               return 0;
           }
       };
       var setChildModuleLevelIsActive = function (ChildRoleModules, accessRole, Type) {
           if (ChildRoleModules !== undefined && ChildRoleModules !== null && ChildRoleModules.length) {
               for (var i = 0; i < ChildRoleModules.length; i++) {
                   for (j = 0; j < ChildRoleModules[i].RoleModules.length; j++) {
                       if (accessRole.RoleId === ChildRoleModules[i].RoleModules[j].RoleId) {
                           if (Type === undefined || Type === null) {
                               ChildRoleModules[i].RoleModules[j].IsActive = accessRole.IsActive;
                           }
                           else {
                               ChildRoleModules[i].RoleModules[j].IsActive = !ChildRoleModules[i].RoleModules[j].IsActive;
                           }
                       }
                   }
               }
           }
       };

       $scope.saveRoleModulePermission = function (accessRole, ChildRoleModules) {

           setChildModuleLevelIsActive(ChildRoleModules, accessRole);

           AppSpinner.showSpinnerTemplate($scope.UpdatingAccessLevel, $scope.Template);
           AccessLevelService.SaveRoleModulePermission(accessRole).then(function (response) {

               if (response.data && response.data.Status) {
                   toaster.pop({
                       type: "success",
                       title: $scope.FrayteSuccess,
                       body: $scope.SuccessfullyUpdated,
                       showCloseButton: true
                   });

               }
               else {
                   accessRole.IsActive = !accessRole.IsActive;
                   setChildModuleLevelIsActive(ChildRoleModules, accessRole, "error");
                   toaster.pop({
                       type: "error",
                       title: $scope.FrayteError,
                       body: $scope.CouldNotSetPermission,
                       showCloseButton: true
                   });
               }
               AppSpinner.hideSpinnerTemplate();

           }, function () {
               accessRole.IsActive = !accessRole.IsActive;
               setChildModuleLevelIsActive(ChildRoleModules, accessRole, "error");
               AppSpinner.hideSpinnerTemplate();
               toaster.pop({
                   type: "error",
                   title: $scope.FrayteError,
                   body: $scope.CouldNotSetPermission,
                   showCloseButton: true
               });
           });
       };

       $scope.changeAccessLevel = function () {
           getInitialDetail();
       };
       var getInitialDetail = function () {
           AppSpinner.showSpinnerTemplate($scope.LoadingAccessLevel, $scope.Template);
           AccessLevelService.GetRoleModules($scope.userInfo.EmployeeId, $scope.userInfo.RoleId, $scope.moduleType).then(function (response) {
               if (response.status == 200 && response.data) {
                   $scope.roleModules = response.data;
                   for (var i = 0; i < $scope.roleModules.ModuleRoles.length; i++) {
                       if ($scope.roleModules.ModuleRoles[i].ChildRoleModules === null || $scope.roleModules.ModuleRoles[i].ChildRoleModules.length < 1) {
                           $scope.roleModules.ModuleRoles[i].isIcon = false;
                       }
                       else {
                           $scope.roleModules.ModuleRoles[i].isIcon = true;
                       }
                   }
               }
               else {
                   toaster.pop({
                       type: "error",
                       title: $scope.FrayteError,
                       body: $scope.ErrorGettingRecord,
                       showCloseButton: true
                   });
               }
               AppSpinner.hideSpinnerTemplate();
           }, function (response) {
               AppSpinner.hideSpinnerTemplate();
               if (response.status !== 401) {
                   toaster.pop({
                       type: "error",
                       title: $scope.FrayteError,
                       body: $scope.ErrorGettingRecord,
                       showCloseButton: true
                   });
               }
           });
       };

       // Take care of this later
       var getScreenInitials = function () {
           AccessLevelService.GetRoles($scope.userInfo.EmployeeId).then(function (response) {
               if (response.status === 200 && response.data) {
                   $scope.roles = response.data;
                   angular.forEach($scope.roles, function (eachObj) {
                       if (eachObj.RoleId === 9) {
                           $scope.role = eachObj;
                       }
                   });

               }
               else {
                   toaster.pop({
                       type: "error",
                       title: $scope.FrayteError,
                       body: $scope.ErrorGettingRecord,
                       showCloseButton: true
                   });
               }
           }, function () {
               toaster.pop({
                   type: "error",
                   title: $scope.FrayteError,
                   body: $scope.ErrorGettingRecord,
                   showCloseButton: true
               });
           });
       };
       // 
       function init() {

           $scope.moduleTypes = [
               {
                   key: 'DirectBooking',
                   value: 'DirectBooking'
               },
               {
                   key: 'eCommerce',
                   value: 'eCommerce'
               },
               {
                   key: 'Tradelane',
                   value: 'Tradelane'
               },
               {
                   key: 'Break Bulk',
                   value: 'BreakBulk'
               },
               {
                   key: 'Express Solutions',
                   value: 'ExpressSolution'
               }
           ];

           $scope.GetServiceValue = null;
           $scope.Template = 'directBooking/ajaxLoader.tpl.html';


           if (SessionService.getUser()) {
               $scope.userInfo = SessionService.getUser();
               if ($scope.userInfo.moduleType) {
                   $scope.moduleType = $scope.userInfo.moduleType;
               }
               else {
                   $scope.moduleType = 'DirectBooking';
               }
               setMultilingualOptions();

           }
           else {
               $state.go('home.welcome');
           }



       }
       init();
   });