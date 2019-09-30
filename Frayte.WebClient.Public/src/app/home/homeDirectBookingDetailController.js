
angular.module('ngApp.home').controller('homeDirectBookingDetailController', function ($scope, $uibModal, $location, $log, $filter, config) {

  //start angular carousel
  $scope.left = function () {
    var elmnt = document.getElementById("scrolling-wallpaper");
    elmnt.scrollLeft = elmnt.scrollLeft + 173;
  };
  $scope.right = function () {
    var elmnt = document.getElementById("scrolling-wallpaper");
    elmnt.scrollLeft = elmnt.scrollLeft - 173;
  };
  //end


  $scope.ImagePath = config.BUILD_URL;
  $scope.FrayteWebSite = config.SITE_COUNTRY;
  if ($scope.FrayteWebSite === 'COM') {
    $scope.saleslink = 'sales@frayte.com';
  }
  else if ($scope.FrayteWebSite === 'CO.UK') {
    $scope.saleslink = 'sales@frayte.co.uk';
  } else if ($scope.FrayteWebSite === 'CO.TH') {
      $scope.saleslink = 'sales@frayte.co.uk';
  }

});
