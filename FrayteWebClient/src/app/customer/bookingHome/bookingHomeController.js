angular.module('ngApp.bookingHome').controller('BookingHomeController', function ($scope, UtilityService,AppSpinner, $state, $translate, toaster, $timeout, SessionService, CustomerService, config) {


    $scope.DirectBookingDisable = function (Isdirectbooking) {
        if (Isdirectbooking === false) {
            return true;
        }
        else {
            return false;
        }
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
        else if (ShipmentType === "eCommerce" && $scope.RoleId === 3) {
            if (current.substr(0, 6) === 'admin.') {
                $state.go('admin.booking-home.direct-booking', { shipmentId: 0 }, { relaod: true });
            }
            else if (current.substr(0, 6) === 'dbuser') {
                $state.go('dbuser.booking-home.direct-booking', { shipmentId: 0 }, { relaod: true });
            }
            else if (current.substr(0, 6) === 'custom') {
                $state.go('customer.booking-home.eCommerce-booking', { shipmentId: 0 }, { relaod: false });
            }
        }

    };

    $scope.goToAddressBook = function () {

        $state.go('customer.booking-home.address-book', { userId: $scope.customerId }, { relaod: true });
    };
    var SetMultilingualOptions = function () {
        $translate(['FrayteWarning_Validation', 'ReceiveDetail_Validation']).then(function (translations) {
            $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
            $scope.ReceiveDetailValidation = translations.ReceiveDetail_Validation;
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
        $scope.spinnerMessage = 'Loading Quick Booking';
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';

        var userInfo = SessionService.getUser();
        $scope.tabs = userInfo.tabs;
        $scope.tab = getUserTab($scope.tabs, "QuickDirect_Booking");
        $scope.customerId = userInfo.EmployeeId;
        $scope.RoleId = userInfo.RoleId;
        $scope.buildURL = config.BUILD_URL;

        //Get module detail assigned to the customer
        if ($scope.RoleId !==3) {
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
                        // To Do :
                    }
                    else if (!$scope.CustomerModules.IsTradeLaneBooking && !$scope.CustomerModules.IsDirectBooking && $scope.CustomerModules.IsBreakBulkBooking) {
                        // To Do :
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
                    }
                    else {
                        AppSpinner.hideSpinnerTemplate();
                    }
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteWarningValidation,
                        body: $scope.ReceiveDetailValidation,
                        showCloseButton: true
                    });
                }

            });
        }
        new SetMultilingualOptions();

    }
    init();
});