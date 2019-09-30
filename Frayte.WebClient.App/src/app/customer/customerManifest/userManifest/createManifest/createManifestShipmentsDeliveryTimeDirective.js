angular.module("ngApp.customer")
       .directive("estimatedDeliveryTime", function ($filter) {

           var linker = function ($scope, element, attrs) {
               var setCollectionTime = function () {
                   console.log("Directive Scope", $scope);
                   if ($scope.shipmentETAETDDetail !== undefined) {
                       var time = $scope.shipmentETAETDDetail.EstimatedTimeofDelivery;
                       if (time !== null && time.length === 4) {

                           var h = $filter('filter')($scope.hours, { hour: time.substring(0, 2) });
                           if (h.length) {
                               $scope.timeHrs = h[0];
                           }
                           else {
                               $scope.timeHrs = null;
                           }

                           var m = $filter('filter')($scope.minutes, { minute: time.substring(2, 4) });
                           if (m.length) {
                               $scope.timeMins = m[0];
                           }
                           else {
                               $scope.timeMins = null;
                           }

                       }
                   }

               };
               $scope.$watch('shipmentETAETDDetail.EstimatedTimeofDelivery', function (newValue, oldValue) {
                   setCollectionTime(newValue);
               });
           };
           var collectionTimeController2 = function ($scope, $filter) {

               $scope.setCollectionHours = function () {
                   if ($scope.timeHrs !== undefined && $scope.timeHrs !== null && $scope.timeMins !== undefined && $scope.timeMins !== null && $scope.shipmentETAETDDetail !== undefined) {
                       var str = $scope.timeHrs.hour + $scope.timeMins.minute;
                       $scope.shipmentETAETDDetail.EstimatedTimeofDelivery = str;
                   }
               };

               $scope.setCollectionMinutes = function () {
                   if ($scope.timeHrs !== undefined && $scope.timeHrs !== null && $scope.timeMins !== undefined && $scope.timeMins !== null && $scope.shipmentETAETDDetail !== undefined) {
                       var str = $scope.timeHrs.hour + $scope.timeMins.minute;
                       $scope.shipmentETAETDDetail.EstimatedTimeofDelivery = str;
                   }
               };

               function init() {
                   $scope.hours = [
                       { Id: 24, hour: "00" },
                                    { Id: 1, hour: "01" },
                                    { Id: 2, hour: "02" },
                                    { Id: 3, hour: "03" },
                                    { Id: 4, hour: "04" },
                                    { Id: 5, hour: "05" },
                                    { Id: 6, hour: "06" },
                                    { Id: 7, hour: "07" },
                                    { Id: 8, hour: "08" },
                                    { Id: 9, hour: "09" },
                                    { Id: 10, hour: "10" },
                                    { Id: 11, hour: "11" },
                                    { Id: 12, hour: "12" },
                                    { Id: 13, hour: "13" },
                                    { Id: 14, hour: "14" },
                                    { Id: 15, hour: "15" },
                                    { Id: 16, hour: "16" },
                                    { Id: 17, hour: "17" },
                                    { Id: 18, hour: "18" },
                                    { Id: 19, hour: "19" },
                                    { Id: 20, hour: "20" },
                                    { Id: 21, hour: "21" },
                                    { Id: 22, hour: "22" },
                                    { Id: 23, hour: "23" }

                   ];
                   $scope.minutes = [
                                    { Id: 60, minute: "00" },
                                    { Id: 1, minute: "01" },
                                    { Id: 2, minute: "02" },
                                    { Id: 3, minute: "03" },
                                    { Id: 4, minute: "04" },
                                    { Id: 5, minute: "05" },
                                    { Id: 6, minute: "06" },
                                    { Id: 7, minute: "07" },
                                    { Id: 8, minute: "08" },
                                    { Id: 9, minute: "09" },
                                    { Id: 10, minute: "10" },
                                    { Id: 11, minute: "11" },
                                    { Id: 12, minute: "12" },
                                    { Id: 13, minute: "13" },
                                    { Id: 14, minute: "14" },
                                    { Id: 15, minute: "15" },
                                    { Id: 16, minute: "16" },
                                    { Id: 17, minute: "17" },
                                    { Id: 18, minute: "18" },
                                    { Id: 19, minute: "19" },
                                    { Id: 20, minute: "20" },
                                    { Id: 21, minute: "21" },
                                    { Id: 22, minute: "22" },
                                    { Id: 23, minute: "23" },
                                    { Id: 24, minute: "24" },
                                    { Id: 25, minute: "25" },
                                    { Id: 26, minute: "26" },
                                    { Id: 27, minute: "27" },
                                    { Id: 28, minute: "28" },
                                    { Id: 29, minute: "29" },
                                    { Id: 30, minute: "30" },
                                    { Id: 31, minute: "31" },
                                    { Id: 32, minute: "32" },
                                    { Id: 33, minute: "33" },
                                    { Id: 34, minute: "34" },
                                    { Id: 35, minute: "35" },
                                    { Id: 36, minute: "36" },
                                    { Id: 37, minute: "37" },
                                    { Id: 38, minute: "38" },
                                    { Id: 39, minute: "39" },
                                    { Id: 40, minute: "40" },
                                    { Id: 41, minute: "41" },
                                    { Id: 42, minute: "42" },
                                    { Id: 43, minute: "43" },
                                    { Id: 44, minute: "44" },
                                    { Id: 45, minute: "45" },
                                    { Id: 46, minute: "46" },
                                    { Id: 47, minute: "47" },
                                    { Id: 48, minute: "48" },
                                    { Id: 49, minute: "49" },
                                    { Id: 50, minute: "50" },
                                    { Id: 51, minute: "51" },
                                    { Id: 52, minute: "52" },
                                    { Id: 53, minute: "53" },
                                    { Id: 54, minute: "54" },
                                    { Id: 55, minute: "55" },
                                    { Id: 56, minute: "56" },
                                    { Id: 57, minute: "57" },
                                    { Id: 58, minute: "58" },
                                    { Id: 59, minute: "59" }
                   ];
               }
               init();

           };
           return {
               restrict: "AE",
               templateUrl: 'customer/customerManifest/userManifest/createManifest/createManifestDeliveryTime.tpl.html',
               controller: collectionTimeController2,
               link: linker
           };


       });