angular.module('ngApp.eCommerce')
       .controller("eCommerceInvoiceAndAcountingController", function ($scope, $interval,$uibModal, AppSpinner, UtilityService, DateFormatChange, eCommerceBookingService, SessionService, toaster) {
           $scope.GetForMattedDate = function (date) {
               if (date) {
                   return UtilityService.GetForMattedDate(date);
               }
           };

           var getScreenInitials = function () {
               eCommerceBookingService.InVoiceAndAcounting($scope.createdBy, $scope.$parent.ShipmentId).then(function (response) {
                   $scope.InVoiceAndAcountingDetail = response.data;
               }, function () {

               });
           };

           $scope.addCreditNote = function () {
               var modalInstance = $uibModal.open({
                   animation: true,
                   controller: 'CreditNoteDetailController',
                   templateUrl: 'eCommerceBooking/eCommerceBookingDetail/creditNoteDetail.tpl.html',
                   backdrop: 'static',
                   size: 'md',
                   resolve: {
                       CreditNote: function () {
                           return $scope.creditNote;
                       }
                   }
               });

               modalInstance.result.then(function () {
                   getScreenInitials();
               }, function () { 
               });
           };



           function init() {
               $scope.Template = 'directBooking/ajaxLoader.tpl.html';
               var userInfo = SessionService.getUser();
               if (userInfo) {
                   $scope.createdBy = userInfo.EmployeeId;
               }
               else {
                   $scope.createdBy = 0;
               }

               $scope.creditNote = {
                   eCommerceUserCreditNoteId: 0,
                   ShipmentId: $scope.$parent.ShipmentId,
                   CreditNoteReference: "",
                   Amount: "",
                   CurrencyCode: "",
                   IssuedBy: $scope.createdBy,
                   IssuedOnUtc: "",
                   UsedOnUtc: "",
                   IssuedTo: "",
                   Status: "",
                   eCommreceInvoiceId: 0
               };
               if ($scope.$parent.Status !=="Draft") {
                   getScreenInitials();
               }
             
           }
           init();
       });