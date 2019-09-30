angular.module('ngApp.eCommerce')
       .controller("eCommerceCommunicationController", function ($scope, $interval, AppSpinner, UtilityService, DateFormatChange, eCommerceBookingService, SessionService, toaster, $translate) {


           //set translate code start
           var setMultilingualOptions = function () {
               $translate(['FrayteError', 'FrayteWarning', 'Frayte_Success', 'ErrorSavingDataPleaseTryAgain', 'CommunicationAddedSuccessfully', 'EmailSentSuccessfully', 'CorrectValidationError',
               'SavingCommunication', 'SendingEmail']).then(function (translations) {

                   $scope.FrayteWarning = translations.FrayteWarning;
                   $scope.Frayte_Success = translations.Frayte_Success;
                   $scope.FrayteError = translations.FrayteError;
                   $scope.ErrorSavingDataPleaseTryAgain = translations.ErrorSavingDataPleaseTryAgain;
                   $scope.CommunicationAddedSuccessfully = translations.CommunicationAddedSuccessfully;
                   $scope.EmailSentSuccessfully = translations.EmailSentSuccessfully;
                   $scope.CorrectValidationError = translations.CorrectValidationError;
                   $scope.SendingEmail=translations.SendingEmail;
                   $scope.SavingCommunication = translations.SavingCommunication;
               });
           };


           $scope.GetCorrectFormattedDate = function (date, time) {
               // Geting Correct Date Format
               if (date !== null && date !== '' && date !== undefined) {
                   var newDate = new Date(date);
                   var newTime;
                   if (time !== undefined && time !== null) {
                       newTime = time;
                   }

                   var days = ["SUNDAY", "MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY"];
                   //var monthNames = ["January", "February", "March", "April", "May", "June","July", "August", "September", "October", "November", "December" ];
                   //var dformat = days[newDate.getDay()] + ', ' + monthNames[newDate.getMonth()] + ' ' + newDate.getDate() + ', ' + newDate.getFullYear();
                   var dformat = days[newDate.getDay()] + ', ' + DateFormatChange.DateFormatChange(newDate);
                   if (time !== undefined && time !== null) {
                       //dformat = days[newDate.getDay()] + ', ' + monthNames[newDate.getMonth()] + ' ' + newDate.getDate() + ', ' + newDate.getFullYear() + ':' + newTime;
                       dformat = days[newDate.getDay()] + ', ' + DateFormatChange.DateFormatChange(newDate) + ':' + newTime;
                   }
                   return dformat;
               }
               else {
                   return;
               }
           };

           $scope.save = function (valid, form) {
               if (valid) {
                   AppSpinner.showSpinnerTemplate($scope.SavingCommunication, $scope.Template);
                   eCommerceBookingService.CreateCommunication($scope.invoiceCommunication).then(function (response) {
                       if (response.data.Status) {
                           getScreenInitials();
                           toaster.pop({
                               type: 'success',
                               title: $scope.Frayte_Success,
                               body: $scope.CommunicationAddedSuccessfully,
                               showCloseButton: true
                           });
                           form.$setPristine();
                           form.$setUntouched();
                           $scope.submitted1 = false;
                           $scope.invoiceCommunication.Description = "";
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

           $scope.SaveEmail = function (valid, form) {
               if (valid) {
                   AppSpinner.showSpinnerTemplate($scope.SendingEmail, $scope.Template);
                   eCommerceBookingService.SaveEmailCommunication($scope.emailCommunication).then(function (response) {
                       if (response.data.Status) {
                           getemailCommunications();
                           toaster.pop({
                               type: 'success',
                               title: $scope.Frayte_Success,
                               body: $scope.EmailSentSuccessfully,
                               showCloseButton: true
                           });
                           form.$setPristine();
                           form.$setUntouched();
                           $scope.submitted = false;
                           $scope.emailCommunication.EmailBody = "";
                           $scope.emailCommunication.EmailSubject = "";
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

           $scope.ResendMail = function (emailCommunication) {
               $scope.emailCommunication = {
                   eCommerceEmailCommunicationId: 0,
                   ShipmentId: $scope.$parent.ShipmentId,
                   CreatedBy: $scope.createdBy,
                   EmailSentOnDate: emailCommunication.EmailSentOnDate,
                   EmailSentTime: emailCommunication.EmailSentTime,
                   EmailSubject: emailCommunication.EmailSubject,
                   EmailBody: emailCommunication.EmailBody,
                   CC: emailCommunication.CC,
                   BCC: emailCommunication.BCC,
                   SentTo: emailCommunication.SentTo
               };
           };



           var getemailCommunications = function () {
               eCommerceBookingService.GetEmailCommunication($scope.createdBy, $scope.$parent.ShipmentId).then(function (response) {
                   $scope.emailCommunications = response.data;
               }, function () {

               });
           };
           var getScreenInitials = function () {
               eCommerceBookingService.GetCommunications($scope.createdBy, $scope.$parent.ShipmentId).then(function (response) {
                   $scope.communications = response.data;
               }, function () {

               });
           };

           $scope.callAtInterval = function () {
               getScreenInitials();
           };
           $scope.stop = function () {
             //  $interval.cancel($scope.promise);
           };
           $scope.$on('$destroy', function () {
             //  $scope.stop();
           });
           function init() {
               $scope.Template = 'directBooking/ajaxLoader.tpl.html';
             //  $scope.promise = $interval(function () { $scope.callAtInterval(); }, 20000);
               var userInfo = SessionService.getUser();
               if (userInfo) {
                   $scope.createdBy = userInfo.EmployeeId;
               }
               else {
                   $scope.createdBy = 0;
               }


               $scope.options = [
                   {
                       key: "Private",
                       value: "Private"
                   }, {
                       key: "Public",
                       value: "Public"
                   }
               ];
               $scope.invoiceCommunication = {
                   ShipmentId: $scope.$parent.ShipmentId,
                   CreatedBy: $scope.createdBy,
                   Description: "",
                   AccessType: "Public"
               };

               $scope.emailCommunication = {
                   eCommerceEmailCommunicationId: 0,
                   ShipmentId: $scope.$parent.ShipmentId,
                   CreatedBy: $scope.createdBy,
                   EmailSentOnDate: new Date(),
                   EmailSentTime: "",
                   EmailSubject: "",
                   EmailBody: "",
                   CC: "",
                   BCC: "",
                   SentTo: ""
               };
               if ($scope.$parent.Status !== "Draft") {
                   getScreenInitials();
                   getemailCommunications();
               }
               setMultilingualOptions();
              
           }
           init();
       });