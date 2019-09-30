/** 
 * Controller
 */
angular.module('ngApp.loginview').controller('ServicesController', function ($rootScope, AppSpinner, CustomerService, localStorage, $scope, HomeService, DownloadExcelService, $state, $location, $translate, config, $filter, LogonService, SessionService, $uibModal, toaster, SystemAlertService, DateFormatChange, UtilityService, $timeout, $window) {

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


    $rootScope.goToBooking = function (ShipmentType) {

        if (ShipmentType) {
            if (ShipmentType === 'DirectBooking') {
                $rootScope.selectedImage = 'directbooking-30.png';
                $translate(['Direct_Bookingg']).then(function (translations) {
                    $rootScope.selectedModule = translations.Direct_Bookingg;
                });

                $rootScope.IsDirectBookingSelected = true;
                $rootScope.IseCommerceBookingSelected = false;
                $rootScope.IsTradelaneSelected = false;
            }
            if (ShipmentType === 'eCommerce') {
                $translate(['Ecommerce']).then(function (translations) {
                    $rootScope.selectedModule = translations.Ecommerce;
                });

                $rootScope.selectedImage = 'ecommerce-30.png';
                $rootScope.IsDirectBookingSelected = false;
                $rootScope.IseCommerceBookingSelected = true;
                $rootScope.IsTradelaneSelected = false;
            }
            if (ShipmentType === 'Tradelane') {
                $translate(['Tradelane']).then(function (translations) {
                    $rootScope.selectedModule = translations.Tradelane;
                });
                $rootScope.selectedImage = 'tradelane-30.png';
                $rootScope.IsDirectBookingSelected = false;
                $rootScope.IseCommerceBookingSelected = false;
                $rootScope.IsTradelaneSelected = true;
            }
            if (ShipmentType === 'BreakBulk') {
                $translate(['BreakBulk']).then(function (translations) {
                    $rootScope.selectedModule = translations.BreakBulk;
                });
                $rootScope.selectedImage = 'breakbulk-30.png';
                $rootScope.IsDirectBookingSelected = false;
                $rootScope.IseCommerceBookingSelected = false;
                $rootScope.IsTradelaneSelected = false;
                $rootScope.IsBreakBulkBooking = true;
            }
            if (ShipmentType === 'ExpressSolution') {
                $translate(['ExpressSolution']).then(function (translations) {
                    $rootScope.selectedModule = translations.ExpressSolution;
                });
                $rootScope.selectedImage = 'express-solutions-30.png';
                $rootScope.IsDirectBookingSelected = false;
                $rootScope.IseCommerceBookingSelected = false;
                $rootScope.IsTradelaneSelected = false;
                $rootScope.IsExpresSolutions = false;
            }
            SessionService.setModuleType(ShipmentType);
        }

        // Get the tabe base don ModuleType 

        $rootScope.getUserTabs(ShipmentType);


    };

    $rootScope.getUserTabs = function (ShipmentType) {
        UtilityService.getUserTabs($scope.customerId, $scope.RoleId, ShipmentType).then(function (response) {
            if (response.data.tabs && response.data.tabs.length) {
                var user = SessionService.getUser();
                user.tabs = response.data.tabs;
                SessionService.removeUser();
                user.moduleType = ShipmentType;
                SessionService.setUser(user);
                var user1 = SessionService.getUser();
                $rootScope.frayteTabs = response.data.tabs;
                if (SessionService.getModuleType() === "ExpressSolution") {
                    var tab = {};
                    if (response.data.tabs && response.data.tabs.length) {

                        for (var i = 0; i < response.data.tabs.length; i++) {
                            if (response.data.tabs[i].route === "loginView.userTabs.express-solution-create-shipment") {
                                tab = response.data.tabs[i];
                                break;
                            }
                        } 
                    }
                    $state.go(tab.route, {}, { reload: true });
                }
                else {
                    if (response.data && response.data.tabs.length) {
                        if (response.data.tabs[0].route.search('direct-booking') > -1) {
                            $state.go(response.data.tabs[0].route, { directShipmentId: 0 }, { relaod: true });
                        }
                        else if (response.data.tabs[0].route.search('eCommerce-booking') > -1 || response.data.tabs[0].route.search('tradelane-booking') > -1) {
                            $state.go(response.data.tabs[0].route, { shipmentId: 0 }, { relaod: true });
                        }

                        else {
                            $state.go(response.data.tabs[0].route, {}, { reload: true });
                        }
                    }
                }

            }
            else if (response.data.Status === 'Margin_Cost_Not_Set_For_Customer') {
                toaster.pop({
                    type: 'warning',
                    title: "Frayte-Warning",
                    body: "Your rate card setting is not completed yet. Please contact administrator.",
                    showCloseButton: true
                });
            }
        }, function () {
            toaster.pop({
                type: 'error',
                showCloseButton: true
            });
        });
    };

    $scope.goToAddressBook = function () {
        $state.go('customer.booking-home.address-book', { userId: $scope.customerId }, { relaod: true });
    };
    var setMultilingualOptions = function () {
        $translate(['FrayteWarning_Validation', 'ReceiveDetail_Validation', 'LoadingDashboard']).then(function (translations) {
            $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
            $scope.ReceiveDetailValidation = translations.ReceiveDetail_Validation;
            $scope.LoadingDashboard = translations.LoadingDashboard;
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

        $rootScope.IsDirectBookingSelected = false;
        $rootScope.IseCommerceBookingSelected = true;

        $rootScope.isNotFound = false;
        $scope.GetServiceValue = null;
        $scope.spinnerMessage = $scope.LoadingDashboard;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';

        var userInfo = SessionService.getUser();
        $scope.tabs = userInfo.tabs;
        $scope.tab = getUserTab($scope.tabs, "QuickDirect_Booking");
        $scope.customerId = userInfo.EmployeeCustomerId ? userInfo.EmployeeCustomerId : userInfo.EmployeeId;
        $scope.RoleId = userInfo.RoleId;
        $scope.buildURL = config.BUILD_URL;

        //Get module detail assigned to the customer
        if ($scope.RoleId !== 3 && $scope.RoleId !== 17) {
            $scope.showBooking = true;
            $rootScope.CustomerModules = {
                IsDirectBooking: true,
                IsBreakBulkBooking: true,
                IsTradeLaneBooking: true,
                IseCommerceBooking: true,
                IsWarehouseAndTransport: true,
                IsExpresSolutions: true
            };

            userInfo.modules = $rootScope.CustomerModules;

            // Need to set the availbles sevices in the session
            SessionService.removeUser();
            SessionService.setUser(userInfo);
            AppSpinner.hideSpinnerTemplate();
        }
        else {
            $scope.showBooking = true;
            AppSpinner.showSpinnerTemplate($scope.spinnerMessage, $scope.Template);
            CustomerService.GetCustomerModules($scope.customerId).then(function (response) {
                if (response.status === 200) {
                    $rootScope.CustomerModules = response.data;
                    userInfo.modules = $rootScope.CustomerModules;

                    // Need to set the availbles sevices in the session
                    SessionService.removeUser();
                    SessionService.setUser(userInfo);
                    AppSpinner.hideSpinnerTemplate();

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