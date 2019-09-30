
angular.module('ngApp.breakBulk').controller("BreakbulkGetserviceController", function ($scope, config, $rootScope, AppSpinner, BreakBulkService, BreakbulkBookingObj, SessionService, toaster, $uibModalInstance, uiGridConstants, $translate, ModalService, $uibModal, $http, $window) {
    $scope.check = false;

    var getScreenInitials = function () {

        AppSpinner.showSpinnerTemplate("Getting available services for shipment", $scope.Template);

        BreakBulkService.BreakBulkHubServices($scope.breakbulkServiceObj).then(function (response) {

            $scope.services = response.data;
            AppSpinner.hideSpinnerTemplate();

            setCourierCompanyImage();

        }, function () {
            console.log("Could not get services");
        });

    };
    var setCourierCompanyImage = function () {
        if ($scope.services.length) {
            for (var i = 0; i < $scope.services.length; i++) {
                $scope.services[i].ImageURL = $scope.ImagePath + $scope.services[i].CarrierLogo;
            }
        }
    };

    $scope.setCustomerRateCard = function (RateCard) {
        if (RateCard !== undefined && RateCard !== null) {
            $uibModalInstance.close(RateCard);
        }
    };

  
    function init() {

        $scope.services = [];
        $scope.breakbulkServiceObj = {
            FromCountryId: 0,
            FromPostCode: '',
            FromState: '',
            ToCountryId: 0,
            ToPostCode: '',
            ToState: '',
            //TotalWeight: 0.00,
            CustomerId: 0

        };

        $scope.ImagePath = config.BUILD_URL;
        var userInfo = SessionService.getUser();
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';

        $rootScope.GetServiceValue = null;

        if (BreakbulkBookingObj) {
            $scope.expObj = BreakbulkBookingObj;
            $scope.breakbulkServiceObj.FromCountryId = $scope.expObj.ShipFrom.Country.CountryId;
            $scope.breakbulkServiceObj.FromPostCode = $scope.expObj.ShipFrom.PostCode;
            $scope.breakbulkServiceObj.ToCountryId = $scope.expObj.ShipTo.Country.CountryId;
            $scope.breakbulkServiceObj.ToPostCode = $scope.expObj.ShipTo.PostCode;
            $scope.breakbulkServiceObj.ToState = $scope.expObj.ShipTo.State;
            //$scope.breakbulkServiceObj.TotalWeight = weightTotal;
            $scope.breakbulkServiceObj.CustomerId = $scope.expObj.CustomerId;
            getScreenInitials();
        }
        else {

        }

    }
    init();

});