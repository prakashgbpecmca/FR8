angular.module('ngApp.eCommerce')
       .controller("CreditNoteDetailController", function ($scope, CreditNote, $uibModalInstance, $interval, TopCurrencyService, AppSpinner, UtilityService, DateFormatChange, eCommerceBookingService, SessionService, toaster, $translate) {

           //set translate code start
           var setMultilingualOptions = function () {
               $translate(['FrayteError', 'FrayteWarning', 'Frayte_Success', 'ErrorSavingDataPleaseTryAgain', 'CommunicationAddedSuccessfully', 'EmailSentSuccessfully', 'CorrectValidationError',
               'SavingCreditNote']).then(function (translations) {

                   $scope.FrayteWarning = translations.FrayteWarning;
                   $scope.Frayte_Success = translations.Frayte_Success;
                   $scope.FrayteError = translations.FrayteError;
                   $scope.ErrorSavingDataPleaseTryAgain = translations.ErrorSavingDataPleaseTryAgain;
                   $scope.CommunicationAddedSuccessfully = translations.CommunicationAddedSuccessfully;
                   $scope.EmailSentSuccessfully = translations.EmailSentSuccessfully;
                   $scope.CorrectValidationError = translations.CorrectValidationError;
                   $scope.SavingCreditNote = translations.SavingCreditNote;
               });
           };

           $scope.save = function (valid) {
               if (valid) {
                   AppSpinner.showSpinnerTemplate($scope.SavingCreditNote, $scope.Template);
                   if ($scope.CurrencyCode) {
                       $scope.creditNote.CurrencyCode = $scope.CurrencyCode.CurrencyCode;
                   }
                   eCommerceBookingService.AddUserCreditNote($scope.creditNote).then(function (response) {
                       if (response.data.Status) {
                           toaster.pop({
                               type: 'success',
                               title: $scope.Frayte_Success,
                               body: $scope.CommunicationAddedSuccessfully,
                               showCloseButton: true
                           });
                           $uibModalInstance.close();
                       }
                       else {
                           toaster.pop({
                               type: 'error',
                               title: $scope.FrayteError,
                               body: $scope.ErrorSavingDataPleaseTryAgain,
                               showCloseButton: true
                           });
                       }
                       AppSpinner.hideSpinnerTemplate();
                   }, function () {
                       AppSpinner.hideSpinnerTemplate();
                       toaster.pop({
                           type: 'error',
                           title: $scope.FrayteError,
                           body: $scope.ErrorSavingDataPleaseTryAgain,
                           showCloseButton: true
                       });
                   });
               }
               else {
                   toaster.pop({
                       type: 'warning',
                       title: $scope.FrayteWarning,
                       body: $scope.CorrectValidationError,
                       showCloseButton: true
                   });
               }
           };

           function init() {
               $scope.Template = 'directBooking/ajaxLoader.tpl.html';
               $scope.creditNote = CreditNote;
               eCommerceBookingService.GetCurrencies().then(function (response) {
                   $scope.CurrencyTypes = TopCurrencyService.TopCurrencyList(response.data);
               }, function () { });
               setMultilingualOptions();
           }

           init();
       });