angular.module('ngApp.express').controller('ExpressServicesController', function ($scope, config, toaster, AppSpinner, uiGridConstants, $translate, ExpressBookingService, ModalService, $uibModalInstance, $http, $window, SessionService, $rootScope, $uibModal, UtilityService, ExpressBookingObj) {
    $scope.getTotalKgs = function () {
        if ($scope.expObj === undefined) {
            return;
        }
        else if ($scope.expObj.Packages === undefined || $scope.expObj.Packages === null) {
            return 0;
        }
        else if ($scope.expObj.Packages.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.expObj.Packages.length; i++) {
                var product = $scope.expObj.Packages[i];
                if (product.Weight === null || product.Weight === undefined) {
                    total += parseFloat(0);
                }
                else {
                    if (product.CartonValue === undefined || product.CartonValue === null) {
                        var carton = parseFloat(0);
                        total = total + parseFloat(product.Weight) * carton;
                    }
                    else {
                        total = total + parseFloat(product.Weight) * product.CartonValue;
                    }
                }
            }

            sum = total.toFixed(2);
            var num = [];
            num = sum.toString().split('.');

            if (num.length > 1) {
                var as = parseFloat(num[1]);
                if (as.toString().length === 1) {
                    as = as.toString() + "0";
                    as = parseFloat(as);
                }
                if (as === 0) {
                    return total.toFixed(2);
                }
                else if (as === 50) {
                    return total.toFixed(2);
                }
                else {
                    if (as > 49) {
                        var r = parseFloat(num[0]) + 1;
                        return r.toFixed(2);

                    }
                    else {

                        var s = parseFloat(num[0]) + 0.50;
                        return s.toFixed(2);
                    }
                }
            }
            else {
                return total.toFixed(2);
            }

        }
        else {
            return 0;
        }
    };



    $scope.setCustomerRateCard = function (RateCard) {
        if (RateCard !== undefined && RateCard !== null) {
            $uibModalInstance.close(RateCard);
        }
    };

    var setCourierCompanyImage = function () {
        if ($scope.services.length) {
            for (var i = 0 ; i < $scope.services.length ; i++) {
                $scope.services[i].ImageURL = $scope.ImagePath + $scope.services[i].CarrierLogo;
            }
        }
    };
    var getScreenInitials = function () {

        AppSpinner.showSpinnerTemplate("Getting available services for shipment", $scope.Template);

        ExpressBookingService.ExpressHubServices($scope.expressServiceObj).then(function (response) {

            $scope.services = response.data;
            AppSpinner.hideSpinnerTemplate();

            setCourierCompanyImage();

        }, function () {
            console.log("Could not get services");
        });

    };

    function init() {
        $scope.services = [];
        $scope.expressServiceObj = {
            FromCountryId: 0,
            FromPostCode: '',
            FromState: '',
            ToCountryId: 0,
            ToPostCode: '',
            ToState: '',
            TotalWeight: 0.00,
            CustomerId: 0

        };


        $scope.ImagePath = config.BUILD_URL;
        var userInfo = SessionService.getUser();
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';

        $rootScope.GetServiceValue = null;

        if (ExpressBookingObj) {
            $scope.expObj = ExpressBookingObj;
            $scope.expressServiceObj.FromCountryId = $scope.expObj.ShipFrom.Country.CountryId;
            $scope.expressServiceObj.FromPostCode = $scope.expObj.ShipFrom.PostCode;
            $scope.expressServiceObj.ToCountryId = $scope.expObj.ShipTo.Country.CountryId;
            $scope.expressServiceObj.ToPostCode = $scope.expObj.ShipTo.PostCode;
            $scope.expressServiceObj.ToState = $scope.expObj.ShipTo.State;
            $scope.expressServiceObj.TotalWeight = $scope.getTotalKgs();
            $scope.expressServiceObj.CustomerId = $scope.expObj.CustomerId;
            getScreenInitials();
        }
        else {

        }


    }

    init();

});
