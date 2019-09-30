
angular.module('ngApp.home').controller('homeTradelaneDetailController', function ($scope, $uibModal, $location, $log, $filter, config) {
    $scope.ComingSoon = 'Coming soon...';


    $scope.ImagePath = config.BUILD_URL;
    $scope.FrayteWebSite = config.SITE_COUNTRY;
    if ($scope.FrayteWebSite === 'COM') {
        $scope.saleslink = 'sales@frayte.com';
    }else if ($scope.FrayteWebSite === 'CO.UK') {
        $scope.saleslink = 'sales@frayte.co.uk';
    } else if ($scope.FrayteWebSite === 'CO.TH') {
        $scope.saleslink = 'sales@frayte.co.th';
    }

});