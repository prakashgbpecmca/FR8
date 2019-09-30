angular.module('ngApp.customer').controller('CustomerBasicController', function ($scope, AppSpinner, $state, $location, $translate, $stateParams, $filter, CustomerService, SessionService, $uibModal, uiGridConstants, toaster, ModalService, TradelaneService, CarrierService, CountryService, ShipmentService, UserService) {

    $scope.basicDetail = function () {
        var current = $state.current.name;
        if (current.substr(0, 6) === 'dbuser') {
            $state.go('dbuser.customer-detail.basic-detail', { customerId: $scope.customerId });
        }
        else if (current.substr(0, 6) === 'admin.') {
            $state.go('admin.customer-detail.basic-detail', { customerId: $scope.customerId });
        }
       
    };
    $scope.customerRateCardSettingCost = function () {
        var current = $state.current.name;
        if (current.substr(0, 6) === 'dbuser') {
            $state.go('dbuser.customer-detail.customerRateCard', { customerId: $scope.customerId });
        }
        else if (current.substr(0, 6) === 'admin.') {
            $state.go('admin.customer-detail.customerRateCard', { customerId: $scope.customerId });
        }

    };
    $scope.marginCost = function () {
        var current = $state.current.name;
        if (current.substr(0, 6) === 'dbuser') {
            $state.go('dbuser.customer-detail.margincost', { customerId: $scope.customerId });
        }
        else if (current.substr(0, 6) === 'admin.') {
            $state.go('admin.customer-detail.margincost', { customerId: $scope.customerId });
        }
        
    };
    $scope.AdvanceRateCard = function () {
        var current = $state.current.name;
        if (current.substr(0, 6) === 'dbuser') {
            $state.go('dbuser.customer-detail.advanceratecard', { customerId: $scope.customerId });
        }
        else if (current.substr(0, 6) === 'admin.') {
            $state.go('admin.customer-detail.advanceratecard', { customerId: $scope.customerId });
        }

    };
    $scope.activeBasicDetail = function () {
        if ($state.is('dbuser.customer-detail.basic-detail')) {
            return true;
        }
        else if ($state.is('admin.customer-detail.basic-detail')) {
            return true;
        }
        return false;
        //   return $state.is('admin.customer-detail.basic-detail');
    };
    $scope.activeMarginCost = function () {
        if ($state.is('admin.customer-detail.margincost')) {
            return true;
        }
        if ($state.is('dbuser.customer-detail.margincost')) {
            return true;
        }
        return false;
        //   return $state.is('admin.customer-detail.margincost');
    };
    $scope.activeAdvanceRateCard = function () {
        if ($state.is('admin.customer-detail.advanceratecard')) {
            return true;
        }
        if ($state.is('dbuser.customer-detail.advanceratecard')) {
            return true;
        }
        return false;
        
    };

    $scope.activeRateCardSetting = function () {
        if ($state.is('admin.customer-detail.customerRateCard')) {
            return true;
        }
        if ($state.is('dbuser.customer-detail.customerRateCard')) {
            return true;
        }
        return false;
    };

    $scope.disableTab = function (tab) {
        if (tab !== undefined) {
            var str = tab.route;
            var arr = str.split(".");
            if (arr !== null && arr.length) {
                for (var j = 0; j < arr.length; j++) {
                    if (arr[j] === "basic-detail") {
                        return false;
                        
                    }
                    
                }
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    };

    $scope.activeRate = function (tab) {
        if ($scope.userInfo && $scope.userInfo.RoleId === 9 || $scope.userInfo && $scope.userInfo.RoleId === 8 ||
            $scope.userInfo.RoleId === 3 || $scope.userInfo.RoleId === 6 || $scope.userInfo.RoleId === 1) {
            if ($scope.tab && tab) {
                // msadmin.setting.systemAlerts
                var currentState = $state.current.name;
                var str = tab.route; //+ "." + $scope.tab.route2;
                var data = currentState.search(str);
                if (data > -1) {
                    return true;
                }
                else {
                    return false;
                }
            }
            return false;
        }
        else {
            if (tab !== undefined) {
                if (tab.tabKey === "Exchange_Rate" && $state.is('admin.setting.exchange-rate')) {
                    return true;
                }
                else if (tab.tabKey === "Exchange_Rate" && $state.is('dbuser.setting.exchange-rate')) {
                    return true;
                }
                else if (tab.tabKey === "Fuel_Surcharge" && $state.is('admin.setting.fuelSurCharge')) {
                    return true;
                }
                else if (tab.tabKey === "Fuel_Surcharge" && $state.is('dbuser.setting.fuelSurCharge')) {
                    return true;
                }
                else if (tab.tabKey === "ParcelHub_Keys" && $state.is('admin.setting.parcel-hub')) {
                    return true;
                }
                else if (tab.tabKey === "ParcelHub_Keys" && $state.is('dbuser.setting.parcel-hub')) {
                    return true;
                }
                else if (tab.tabKey === "TermsAndCondition" && $state.is('admin.setting.terms-and-condition')) {
                    return true;
                }
                else if (tab.tabKey === "Margin_Rate" && $state.is('admin.setting.margin-rate')) {
                    return true;
                }
                else if (tab.tabKey === "System_Alert" && $state.is('admin.setting.systemAlerts')) {
                    return true;
                }
                else if (tab.tabKey === "System_Alert" && $state.is('dbuser.setting.systemAlerts')) {
                    return true;
                }
            }
            return false;
        }

    };

    $scope.changeState = function (tab) {
        if (tab !== undefined) {
            $state.go(tab.route, {}, { reload: true });
        }
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
            
                if ($scope.CustomerDetail.CustomerRateCardType === "NORMAL") {
                    tab.childTabs.splice(3, 1);
                }
                else if ($scope.CustomerDetail.CustomerRateCardType === "ADVANCE") {
                    tab.childTabs.splice(1, 1);
                }
            
            return tab;
        }
    };
    function init() {
        $scope.customerId = $stateParams.customerId;
        var obj = SessionService.getUser();
        CustomerService.GetCustomerDetail($scope.customerId).then(function (response) {
            $scope.CustomerDetail = response.data;
            

            $scope.tabs = obj.tabs;
            if ($scope.CustomerDetail) {
                $scope.tab = getUserTab($scope.tabs, "Customers");
            }
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGetting,
                //title: 'Frayte-Error',
                //body: 'Error while getting customer detail',
                showCloseButton: true
            });
        });
       
        $scope.userInfo = obj;
        if ($scope.userInfo === undefined || $scope.userInfo === null || $scope.userInfo.SessionId === undefined || $scope.userInfo.SessionId === '') {
            $state.go('home.welcome');
        }
        

       // if ($state.is('admin.customer-detail.basic-detail')) {

            if ($stateParams.customerId === undefined) {
                $scope.customerId = "0";
            }
            else {
                var userInfo1 = SessionService.getUser();
                $scope.OperationZoneId = userInfo1.OperationZoneId;
                $scope.OperationZoneName = userInfo1.OperationZoneName;
                $scope.customerId = $stateParams.customerId;
            }
        //}
        //else if ($state.is('dbuser.customer-detail.basic-detail')) {
        //    if ($stateParams.customerId === undefined) {
        //        $scope.customerId = "0";
        //    }
        //    else {
        //        var userInfo2 = SessionService.getUser();
        //        $scope.OperationZoneId = userInfo2.OperationZoneId;
        //        $scope.OperationZoneName = userInfo2.OperationZoneName;
        //        $scope.customerId = $stateParams.customerId;
        //    }
        //}
        //else if ($state.is('customer.manage-detail')) {
        //    $scope.UserInfo = SessionService.getUser();

        //    if ($scope.UserInfo === undefined || $scope.UserInfo === null || $scope.UserInfo.SessionId === undefined || $scope.UserInfo.SessionId === '') {
        //        $state.go('customer.current-shipment');
        //    }
        //    else {
        //        $scope.customerId = $scope.UserInfo.EmployeeId;
        //    }
        //}
           
    }
    init();
});