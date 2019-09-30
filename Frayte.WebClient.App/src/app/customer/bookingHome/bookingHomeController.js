angular.module('ngApp.bookingHome').controller('BookingHomeController', function ($scope, UtilityService, AppSpinner, $state, $translate, toaster, $timeout, SessionService, CustomerService, config) {


    $scope.DirectBookingDisable = function (Isdirectbooking) {
        if (Isdirectbooking === false) {
            return true;
        }
        else {
            return false;
        }
    };

    $scope.FourIcon = function () {

        var FourIconLogic = [];
        FourIconLogic[0] = $scope.CustomerModules.IsDirectBooking;
        FourIconLogic[1] = $scope.CustomerModules.IsTradeLaneBooking;
        FourIconLogic[2] = $scope.CustomerModules.IsBreakBulkBooking;
        FourIconLogic[3] = $scope.CustomerModules.IseCommerceBooking;
        FourIconLogic[4] = $scope.CustomerModules.IsWarehouseAndTransport;
        FourIconLogic[5] = $scope.CustomerModules.IsExpresSolutions;
        var j = 0;
        for (i = 0; i < FourIconLogic.length; i++) {
            if (FourIconLogic[i] === true) {
                j++;
            }

        }
        if (j === 4) {
            return true;
        }
        else {
            return false;
        }

        //if ($scope.CustomerModules.IsDirectBooking && $scope.CustomerModules.IsTradeLaneBooking && $scope.CustomerModules.IsBreakBulkBooking && $scope.CustomerModules.IseCommerceBooking && ($scope.CustomerModules.IsWarehouseAndTransport !== true && $scope.CustomerModules.IsExpresSolutions !== true)) {
        //    return true;
        //}
        //else if ($scope.CustomerModules.IsTradeLaneBooking && $scope.CustomerModules.IsBreakBulkBooking && $scope.CustomerModules.IseCommerceBooking && $scope.CustomerModules.IsWarehouseAndTransport && ($scope.CustomerModules.IsExpresSolutions !== true && $scope.CustomerModules.IsDirectBooking !== true)) {
        //    return true;
        //}
        //else if ($scope.CustomerModules.IsBreakBulkBooking && $scope.CustomerModules.IseCommerceBooking && $scope.CustomerModules.IsWarehouseAndTransport && $scope.CustomerModules.IsExpresSolutions && ($scope.CustomerModules.IsDirectBooking !== true && $scope.CustomerModules.IsTradeLaneBooking !== true)) {
        //    return true;
        //}
        //else if ($scope.CustomerModules.IseCommerceBooking && $scope.CustomerModules.IsWarehouseAndTransport && $scope.CustomerModules.IsExpresSolutions && $scope.CustomerModules.IsDirectBooking && ($scope.CustomerModules.IsTradeLaneBooking !== true && $scope.CustomerModules.IsBreakBulkBooking !== true)) {
        //    return true;
        //}
        //else if ($scope.CustomerModules.IsWarehouseAndTransport && $scope.CustomerModules.IsExpresSolutions && $scope.CustomerModules.IsDirectBooking && $scope.CustomerModules.IsTradeLaneBooking && ($scope.CustomerModules.IsBreakBulkBooking !== true && $scope.CustomerModules.IseCommerceBooking !== true)) {
        //    return true;
        //}
        //else if ($scope.CustomerModules.IsExpresSolutions && $scope.CustomerModules.IsDirectBooking && $scope.CustomerModules.IsTradeLaneBooking && $scope.CustomerModules.IsBreakBulkBooking && ($scope.CustomerModules.IseCommerceBooking !== true && $scope.CustomerModules.IsWarehouseAndTransport !== true)) {
        //    return true;
        //}
        //else {
        //    return false;
        //}
    };

    $scope.goToBooking = function (ShipmentType) {
        var route = "";
        var current = $state.current.name;
        if (ShipmentType === "DirectBooking") {
            AppSpinner.showSpinnerTemplate($scope.spinnerMessage, $scope.Template);
            if ($scope.tab !== undefined && $scope.tab !== null && $scope.tab.childTabs !== null) {
                route = UtilityService.GetCurrentRoute($scope.tab.childTabs, "booking-home.direct-booking");
            }
            $state.go(route, { directShipmentId: 0 }, { relaod: true });
        }
        else if (ShipmentType === "eCommerce") {
            route = UtilityService.GetCurrentRoute($scope.tab.childTabs, "booking-home.eCommerce-booking");
            $state.go(route, { shipmentId: 0 }, { relaod: false });
        }
    };

    $scope.goToAddressBook = function () {
        $state.go('customer.booking-home.address-book', { userId: $scope.customerId }, { relaod: true });
    };
    var setMultilingualOptions = function () {
        $translate(['FrayteWarning_Validation', 'ReceiveDetail_Validation', 'LoadingQuickBooking']).then(function (translations) {
            $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
            $scope.ReceiveDetailValidation = translations.ReceiveDetail_Validation;
            $scope.LoadingQuickBooking = translations.LoadingQuickBooking;
        });
    };

    var getUserTab = function (tabs, tabKey) {
        if (tabs !== undefined && tabs !== null && tabs.length) {
            var tab = {};
            for (var i = 0; i < tabs.length; i++) {
                if (tabs[i].tabKey === tabKey) {
                    tab = tabs[i];
                    break;
                }
            }
            return tab;
        }
    };

    function init() {
        $scope.GetServiceValue = null;
        $scope.spinnerMessage = $scope.LoadingQuickBooking;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';

        var userInfo = SessionService.getUser();
        $scope.tabs = userInfo.tabs;
        $scope.tab = getUserTab($scope.tabs, "QuickDirect_Booking");
        $scope.customerId = userInfo.EmployeeId;
        $scope.RoleId = userInfo.RoleId;
        $scope.buildURL = config.BUILD_URL;

        //Get module detail assigned to the customer
        if ($scope.RoleId !== 3 && $scope.RoleId !== 17) {
            $scope.showBooking = true;
            $scope.CustomerModules = {
                IsDirectBooking: true,
                IsBreakBulkBooking: true,
                IsTradeLaneBooking: true,
                IseCommerceBooking: true,
                IsWarehouseAndTransport: true,
                IsExpresSolutions: true
            };
        }
        else {

            AppSpinner.showSpinnerTemplate($scope.spinnerMessage, $scope.Template);
            CustomerService.GetCustomerModules($scope.customerId).then(function (response) {
                if (response.status === 200) {
                    $scope.CustomerModules = response.data;
                    if (!$scope.CustomerModules.IseCommerceBooking && !$scope.CustomerModules.IsTradeLaneBooking &&
                        $scope.CustomerModules.IsDirectBooking &&
                        !$scope.CustomerModules.IsBreakBulkBooking &&
                        !$scope.CustomerModules.IsWarehouseAndTransport &&
                        !$scope.CustomerModules.IsExpresSolutions
                        ) {
                        $timeout(function () {
                            $scope.goToBooking('DirectBooking');
                        }, 0);

                    }
                    else if ($scope.CustomerModules.IsTradeLaneBooking && !$scope.CustomerModules.IsDirectBooking && !$scope.CustomerModules.IsBreakBulkBooking) {

                        $scope.showBooking = true;// To Do :
                    }
                    else if (!$scope.CustomerModules.IsTradeLaneBooking && !$scope.CustomerModules.IsDirectBooking && $scope.CustomerModules.IsBreakBulkBooking) {
                        // To Do :
                        $scope.showBooking = true;
                    }
                    else if ($scope.CustomerModules.IseCommerceBooking &&
                        !$scope.CustomerModules.IsTradeLaneBooking &&
                        !$scope.CustomerModules.IsDirectBooking &&
                        !$scope.CustomerModules.IsBreakBulkBooking &&
                        !$scope.CustomerModules.IsWarehouseAndTransport &&
                        !$scope.CustomerModules.IsExpresSolutions) {
                        $timeout(function () {
                            $scope.goToBooking('eCommerce');
                        }, 0);
                    } else {
                        $scope.showBooking = true;
                        AppSpinner.hideSpinnerTemplate();
                    }
                }
                else {
                    $scope.showBooking = true;
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteWarningValidation,
                        body: $scope.ReceiveDetailValidation,
                        showCloseButton: true
                    });
                }

            });
        }
        //new SetMultilingualOptions();
        setMultilingualOptions();

    }
    init();
});