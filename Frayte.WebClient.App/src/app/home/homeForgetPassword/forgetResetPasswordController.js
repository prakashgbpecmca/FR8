angular.module('ngApp.forgetPassword')
       .controller("ResetPasswordController", function ($scope, ForgetPasswordService, $state, $stateParams, $location, $translate, AppSpinner, toaster) {
           var setModalOptions = function () {
               $translate(['FrayteSuccess', 'Frayte-Error', 'SuccessfullyChangedPassword', 'FrayteWarning', 'FrayteValidation', 'PleaseCorrectValidationErrors', 'InternalServerErrorOccurred', 'UploadedSuccessfully', 'ErrorOccuredDuringUpload', 'PleaseSelectValidImagefile',
               'PasswordSetSuccessfully', 'SettingPassword']).then(function (translations) {
                   $scope.TitleFrayteSuccess = translations.FrayteSuccess;
                   $scope.TitleFrayteWarning = translations.FrayteWarning;
                   $scope.TitleFrayteError = translations.FrayteError;
                   $scope.TitleFrayteValidation = translations.FrayteValidation;
                   $scope.TextSuccessfullyChangedPassword = translations.SuccessfullyChangedPassword;
                   $scope.TextValidation = translations.PleaseCorrectValidationErrors;
                   $scope.TextInternalServerErrorOccurred = translations.InternalServerErrorOccurred;
                   $scope.PasswordSetSuccessfully = translations.PasswordSetSuccessfully;
                   $scope.SettingPassword = translations.SettingPassword;
               });
           };

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
                   AppSpinner.showSpinnerTemplate($scope.SettingPassword, $scope.Template);
                   ForgetPasswordService.RecoverPassword($scope.user).then(function (response) {
                       AppSpinner.hideSpinnerTemplate();
                       if (response.status === 200) {
                           if (response.data === 19) {
                               toaster.pop({
                                   type: 'success',
                                   title: $scope.TitleFrayteSuccess,
                                   body: $scope.PasswordSetSuccessfully,
                                   showCloseButton: true
                               });
                               $state.go("resetPasswordSuccesful");
                           }
                           else {
                               $scope.IsPasswordRecover = true;
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
                           body: $scope.TextInternalServerErrorOccurred,
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
           function init() {
               setModalOptions();
               $scope.IsPasswordRecover = false;
               $scope.Template = 'directBooking/ajaxLoader.tpl.html';
               $scope.userId = $stateParams.userId;
               var str = $location.$$url;
               var arr = str.split("?");
               $scope.error = false;
               $scope.user = {
                   UserId: $scope.userId,
                   NewPassword: '',
                   ConfirmPassword: '',
                   Token: arr[1]
               };



           }
           init();
       });