angular.module("ngApp.customer")
       .controller("CreateManifestETAETDController", function ($scope, toaster, $uibModalInstance, TimeStringtoDateTime, $translate) {

           var setMultilingualOptions = function () {
               $translate(['FrayteError', 'CorrectValidationErrorFirst']).then(function (translations) {
                   $scope.FrayteError = translations.FrayteError;
                   $scope.CorrectValidationErrorFirst = translations.CorrectValidationErrorFirst;

               });
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
           $scope.dateOptions = {
               formatYear: 'yy',
               //minDate: new Date(),
               startingDay: 1
           };

           $scope.dateOptions1 = {
               formatYear: 'yy',
               minDate: new Date(),
               startingDay: 1
           };


           var toggleMin = function () {
               //var Previous = new Date();
               //Previous.setDate(tomorrow.getDate() - 1);

               $scope.dateOptions.minDate = $scope.dateOptions.minDate ? null : new Date();
           };

           var toggleMin1 = function () {
               //var Previous = new Date();
               //Previous.setDate(tomorrow.getDate() - 1);

               $scope.dateOptions1.minDate = $scope.dateOptions1.minDate ? null : new Date();
           };

           $scope.DateComparision = function () {
               var TDYear;
               var TDMonth;
               var TDDate;
               var FDYear;
               var FDMonth;
               var FDDate;

               if ($scope.shipmentETAETDDetail.EstimatedDateofArrival !== "" && $scope.shipmentETAETDDetail.EstimatedDateofArrival !== null && $scope.shipmentETAETDDetail.EstimatedDateofDelivery !== "" && $scope.shipmentETAETDDetail.EstimatedDateofDelivery !== null) {
                   TDYear = $scope.shipmentETAETDDetail.EstimatedDateofArrival.getFullYear();
                   TDMonth = $scope.shipmentETAETDDetail.EstimatedDateofArrival.getMonth();
                   TDDate = $scope.shipmentETAETDDetail.EstimatedDateofArrival.getDate();
                   if ($scope.shipmentETAETDDetail.EstimatedDateofDelivery !== "") {
                       FDYear = $scope.shipmentETAETDDetail.EstimatedDateofDelivery.getFullYear();

                       FDMonth = $scope.shipmentETAETDDetail.EstimatedDateofDelivery.getMonth();
                       FDDate = $scope.shipmentETAETDDetail.EstimatedDateofDelivery.getDate();

                   }

                   if (FDYear === TDYear && FDMonth <= TDMonth && FDDate < TDDate) {
                       $scope.CompareDate = false;
                   }
                   else if (FDYear === TDYear && FDMonth < TDMonth && FDDate >= TDDate) {
                       $scope.CompareDate = true;
                   }
                   else if (FDYear < TDYear && (FDMonth <= TDMonth || FDMonth >= TDMonth) && (FDDate < TDDate || FDMonth >= TDMonth)) {
                       $scope.CompareDate = false;
                   }

                   else {
                       $scope.CompareDate = true;
                   }
               }

           };

           $scope.save = function (IsValid) {
               
               if (IsValid) {

                   $scope.shipmentETAETDDetail.EstimatedDateofDelivery = TimeStringtoDateTime.ConvertString($scope.shipmentETAETDDetail.EstimatedDateofDelivery, $scope.shipmentETAETDDetail.EstimatedTimeofDelivery);
                   $scope.shipmentETAETDDetail.EstimatedDateofArrival = TimeStringtoDateTime.ConvertString($scope.shipmentETAETDDetail.EstimatedDateofArrival, $scope.shipmentETAETDDetail.EstimatedTimeofArrival);


                   //var format = moment($scope.shipmentETAETDDetail.EstimatedTimeofDelivery, "hmmss").format("HH:mm:ss");
                   //var ab = format.split(':');
                   //$scope.shipmentETAETDDetail.EstimatedDateofDelivery.setHours(ab[0]);
                   //$scope.shipmentETAETDDetail.EstimatedDateofDelivery.setMinutes(ab[1]);
                   //$scope.shipmentETAETDDetail.EstimatedDateofDelivery.setSeconds(ab[2]);

                   //var format1 = moment($scope.shipmentETAETDDetail.EstimatedTimeofArrival, "hmmss").format("HH:mm:ss");
                   //var ab1 = format1.split(':');
                   //$scope.shipmentETAETDDetail.EstimatedDateofArrival.setHours(ab1[0]);
                   //$scope.shipmentETAETDDetail.EstimatedDateofArrival.setMinutes(ab1[1]);
                   //$scope.shipmentETAETDDetail.EstimatedDateofArrival.setSeconds(ab1[2]);
                   // save erta etd 
                   $uibModalInstance.close($scope.shipmentETAETDDetail);

               }
               else {
                   toaster.pop({
                       type: 'warning',
                       title: $scope.FrayteError,
                       body: $scope.CorrectValidationErrorFirst,
                       showCloseButton: true
                   });
               }
           };

           function init() {
               $scope.pageTitle = "ETA & ETD";
               $scope.minDate = new Date();
               $scope.shipmentETAETDDetail = {
                   Shipments: [],
                   EstimatedDateofArrival: null,
                   EstimatedTimeofArrival: "",
                   EstimatedDateofDelivery: null,
                   EstimatedTimeofDelivery: ""
               };
               setMultilingualOptions();
           }

           init();

       });