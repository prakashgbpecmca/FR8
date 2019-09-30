/**
 * Controller
 */
angular.module('ngApp.user').controller('MobileTrackingConfiguration', function ($scope, config, toaster, UserId, AppSpinner, $uibModalInstance, UserService) {

    $scope.breakbulk = function () {
        $scope.isBreakbluk = !$scope.isBreakbluk;
    };
    $scope.directBooking = function () {
        $scope.isDirectBooking = !$scope.isDirectBooking;
    };
    $scope.eCommerce = function () {
        $scope.isEcommerce = !$scope.isEcommerce;
    };
    $scope.express = function () {
        $scope.isExpress = !$scope.isExpress;
    };
    $scope.tradelane = function () {
        $scope.isTradelane = !$scope.isTradelane;
    };
    $scope.bag = function () {
        $scope.isBag = !$scope.isBag;
    };
    $scope.hawb = function () {
        $scope.isHawb = !$scope.isHawb;
    };
    $scope.mawb = function () {
        $scope.isMawb = !$scope.isMawb;
    };
    $scope.pod = function () {
        $scope.isPod = !$scope.isPod;
    };
    $scope.updatetTadelane = function () {
        $scope.isActive = !$scope.isActive;
    };

    var getScreenInitials = function () {
        AppSpinner.showSpinnerTemplate("Loading mobile user configuration", $scope.Template);
        UserService.GetMobileConfiguration($scope.userId).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            $scope.userMobileConfiguration = response.data;
            if (!$scope.userMobileConfiguration.BreakBulkConfiguration &&
                !$scope.userMobileConfiguration.DirectBookingConfiguration &&
                !$scope.userMobileConfiguration.ECommerceConfiguration &&
                !$scope.userMobileConfiguration.TradelaneConfiguration &&
                !$scope.userMobileConfiguration.ExpressConfiguration) {
                $scope.check = false;
            }
            else {
                $scope.check = true;
                if ($scope.userMobileConfiguration.BreakBulkConfiguration && $scope.userMobileConfiguration.BreakBulkConfiguration.length) {
                    $scope.active = 0;
                }

                if ($scope.userMobileConfiguration.DirectBookingConfiguration && $scope.userMobileConfiguration.DirectBookingConfiguration.length) {
                    $scope.active = 1;
                }

                if ($scope.userMobileConfiguration.ECommerceConfiguration && $scope.userMobileConfiguration.ECommerceConfiguration.length) {
                    $scope.active = 2;
                }

                if ($scope.userMobileConfiguration.ExpressConfiguration && $scope.userMobileConfiguration.ExpressConfiguration.length) {
                    $scope.active = 3;
                }

                if ($scope.userMobileConfiguration.TradelaneConfiguration && $scope.userMobileConfiguration.TradelaneConfiguration.length) {
                    $scope.active = 4;
                }
            }
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: "Frayte-Error",
                body: "Error while getting data.",
                showCloseButton: true
            });
        });
    };

    $scope.changeMessage = function (Message) {
        if (Message === "" || Message === null) {
            return false;
        }
        else {
            return true;
        }
    };



    $scope.save = function () {

        AppSpinner.showSpinnerTemplate("Saving user configuration", $scope.Template);
        UserService.SaveMobileconfiguration($scope.userMobileConfiguration).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            $uibModalInstance.close($scope.userDetail);

        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: "Frayte-Error",
                body: "Error while saving data.",
                showCloseButton: true
            });
        });
    };

    $scope.getUserDetail = function () {
        //Get User details 
        UserService.GetUserDetail($scope.userId).then(function (response) {
            $scope.userDetail = response.data;
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGettingUserDetail,
                showCloseButton: true
            });
        });
    };

    function init() {
        //image path code here
        $scope.ImagePath = config.BUILD_URL;
        //end
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';

        $scope.userId = UserId;
        $scope.title = "Demo";
        $scope.isHawb = false;

        getScreenInitials();
        $scope.getUserDetail();
    }

    init();

});