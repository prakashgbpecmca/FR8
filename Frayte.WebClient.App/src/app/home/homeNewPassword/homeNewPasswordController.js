angular.module('ngApp.newPassword')
       .controller("NewPasswordController", function ($scope, $stateParams, $state, $translate, SessionService, CurrentUserService, AppSpinner, LogonService, NewPasswoirdService, OauthService, $timeout, toaster) {
           var setModalOptions = function () {
               $translate(['FrayteSuccess', 'Frayte-Error', 'SuccessfullyChangedPassword', 'FrayteWarning', 'FrayteValidation', 'PleaseCorrectValidationErrors', 'InternalServerErrorOccurred', 'UploadedSuccessfully', 'ErrorOccuredDuringUpload', 'PleaseSelectValidImagefile',
               'UserAccessError', 'UserDoesNotExistPleaseVerify', 'CuurentPasswordInvalid', 'ChangingPassword']).then(function (translations) {
                   $scope.TitleFrayteSuccess = translations.FrayteSuccess;
                   $scope.TitleFrayteWarning = translations.FrayteWarning;
                   $scope.TitleFrayteError = translations.FrayteError;
                   $scope.TitleFrayteValidation = translations.FrayteValidation;
                   $scope.TextSuccessfullyChangedPassword = translations.SuccessfullyChangedPassword;
                   $scope.TextValidation = translations.PleaseCorrectValidationErrors;
                   $scope.TextInternalServerErrorOccurred = translations.InternalServerErrorOccurred;
                   $scope.UserAccessError = translations.UserAccessError;
                   $scope.UserDoesNotExistPleaseVerify = translations.UserDoesNotExistPleaseVerify;
                   $scope.CuurentPasswordInvalid = translations.CuurentPasswordInvalid;
                   $scope.ChangingPassword = translations.ChangingPassword;

               });
           };

           // 
           $scope.checkPasswordValidation = function (str) {
               if (str) {
                   if (str.search(/[A-Z]/) < 0) {
                       $scope.message = 'Password_must_contain_at_least_one_capital_letter';

                       $scope.error = true;
                   }
                   else if (str.search(/[0-9]/) < 0) {
                       $scope.message = 'Password_must_contain_at_least_one_digit';

                       $scope.error = true;
                   }
                   else if (str.length < 8) {
                       $scope.message = 'Password_must_be_atleast_8_characters';

                       $scope.error = true;
                   }
                   else {
                       $scope.error = false;
                   }
               }
           };

           //home login code
           $scope.HomeState = function () {
               $state.go('login');
           };
           //end

           $scope.ChangePassword = function (isValid) {
               if (isValid) {
                   AppSpinner.showSpinnerTemplate($scope.ChangingPassword, $scope.Template);
                   NewPasswoirdService.ChangeFirstPassword($scope.user).then(function (response) {
                       AppSpinner.hideSpinnerTemplate();
                       if (response.status === 200) {
                           $scope.userInfo = response.data;
                           if ($scope.userInfo.EmployeeRoleId === 19) {

                               $state.go("resetPasswordSuccesful");

                           }
                           else {
                               $scope.credentials.UserName = $scope.userInfo.UserName;
                               //if (response.data && response.data.SessionId) {

                               if (!CurrentUserService.profile.token) {
                                   OauthService.oauthToken($scope.userInfo.UserName, $scope.user.NewPassword).then(
                                   function (response) {
                                       // store token in local storage
                                       if (response && response && response.data.access_token) {
                                           CurrentUserService.setProfile($scope.userInfo.EmployeeMail, response.data.access_token);
                                           $scope.getuserTabs();
                                       }
                                       else {
                                           toaster.pop({
                                               type: 'warning',
                                               title: $scope.UserAccessError,
                                               body: $scope.UserDoesNotExistPleaseVerify,
                                               showCloseButton: true
                                           });
                                       }

                                   }, function (error) {
                                       toaster.pop({
                                           type: 'error',
                                           title: $scope.TitleFrayteValidation,
                                           body: $scope.TextErrorWhileLogin,
                                           showCloseButton: true
                                       });
                                   });
                               } else {
                                   $scope.getuserTabs();
                               }
                           }
                         

                       } else {
                           toaster.pop({
                               type: 'warning',
                               title: $scope.TitleFrayteWarning,
                               body: $scope.TextInternalServerErrorOccurred,
                               showCloseButton: true
                           });
                       }

                   }, function () {
                       AppSpinner.hideSpinnerTemplate();
                       toaster.pop({
                           type: 'warning',
                           title: $scope.TitleFrayteWarning,
                           body: $scope.CuurentPasswordInvalid,
                           showCloseButton: true
                       });
                   });
               }
               else {
                   toaster.pop({
                       type: 'warning',
                       title: $scope.TitleFrayteValidation,
                       body: $scope.TextValidation,
                       showCloseButton: true
                   });
               }
           };


           $scope.getuserTabs = function (isValid) {
               LogonService.logon($scope.credentials).then(function (response) {
                   if (response.data === null) {
                       toaster.pop({
                           type: 'warning',
                           title: $scope.UserAccessError,
                           body: $scope.UserDoesNotExistPleaseVerify,
                           showCloseButton: true
                       });
                   }
                   else {
                       if (response.data.SessionId) {
                           if (response.data.IsLastLogin) {
                               // if (!$scope.token || !$scope.token.token) {
                               SessionService.setUser({
                                   UserName: response.data.UserName,
                                   EmployeeName: response.data.EmployeeName,
                                   UserType: response.data.UserType,
                                   EmployeeId: response.data.EmployeeId,
                                   EmployeeMail: response.data.EmployeeMail,
                                   SessionId: response.data.SessionId,
                                   IsRateShow: response.data.IsRateShow,
                                   RoleId: response.data.EmployeeRoleId,
                                   EmployeeCustomerId: response.data.EmployeeCustomerId,
                                   EmployeeCustomerRoleId: response.data.EmployeeCustomerRoleId,
                                   EmployeeCompanyName: response.data.EmployeeCompanyName,
                                   EmployeeCompanyLogo: response.data.EmployeeCompanyLogo,
                                   PhotoUrl: response.data.PhotoUrl,
                                   OperationZoneId: response.data.OperationZoneId,
                                   OperationZoneName: response.data.OperationZoneName,
                                   ValidDays: response.data.ValidDays,
                                   CustomerCurrency: response.data.CustomerCurrency,
                                   tabs: response.data.frayteTabs,
                                   modules: null,
                                   selectedModule: ''
                               });

                               if (response.data.EmployeeCustomerDetail) {
                                   SessionService.setUserCustomer({
                                       EmployeeTrackingEmail: response.data.EmployeeCustomerTrackingEmail,
                                       EmployeeCustomerId: response.data.EmployeeCustomerId,
                                       EmployeeCustomerRoleId: response.data.EmployeeCustomerRoleId,
                                       EmployeeCompanyName: response.data.EmployeeCustomerCompany,
                                       EmployeeCompanyLogo: response.data.EmployeeCustomerCompanyLogo,
                                       EmployeeCustomerService: response.data.CustomerService
                                   });
                               }

                               if (response.data.frayteTabs && response.data.frayteTabs.length) {
                                   // $state.go(response.data.frayteTabs[0].route, {}, { reload: true });
                               }
                               $state.go('loginView.services');
                           }
                           else {
                               $state.go("newPassword", { userId: response.data.EmployeeId });
                           }

                       } else {
                           toaster.pop({
                               type: 'warning',
                               title: $scope.TitleFrayteValidation,
                               body: $scope.TextPleaseEnterValidUserPass,
                               showCloseButton: true
                           });
                       }
                   }
               }, function () {
                   toaster.pop({
                       type: 'error',
                       title: $scope.TitleFrayteValidation,
                       body: $scope.TextErrorWhileLogin,
                       showCloseButton: true
                   });
               });

           };

           function init() {
               setModalOptions();
               $scope.Template = 'directBooking/ajaxLoader.tpl.html';
               $scope.userId = $stateParams.userId;
               $scope.error = false;
               $scope.user = {
                   UserId: $scope.userId,
                   CurrentPassword: '',
                   NewPassword: '',
                   ConfirmPassword: ''
               };

               $scope.credentials = {
                   UserName: "",
                   Password: $scope.user.NewPassword
               };

           }
           init();
       });