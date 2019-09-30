angular.module('ngApp.common')
         .directive('preventEnterSubmit', function () {
             return {
                 link: function (scope, element, attrs) {
                     $(element).keypress(function (e) {
                         if (e.keyCode == 13) {
                             console.log("Enter pressed " + element.val());
                           //  alert("Enter pressed " + element.val());
                             e.preventDefault();
                         }
                     });
                 }
             };
         });