angular.module('ngApp.zoneCountry').controller('ZoneSettingController', function ($scope, $state, $location, $filter, $translate, SessionService, $uibModal, $log, uiGridConstants, toaster, ModalService, $rootScope) {

    $scope.thirdPartyMartix = function () {
        var current = $state.current.name;
        if (current.substr(0, 6) === 'dbuser') {
            $state.go('dbuser.zone-setting.third-party-matrix');
        }
        else if (current.substr(0, 6) === 'admin.') {
            $state.go('admin.zone-setting.third-party-matrix');
        }
    };
    $scope.baseRateCard = function () {
        var current = $state.current.name;
        if (current.substr(0, 6) === 'dbuser') {
            $state.go('dbuser.zone-setting.base-rate-card');
        }
        else if (current.substr(0, 6) === 'admin.') {
            $state.go('admin.zone-setting.base-rate-card');
        }

    };
    $scope.zoneCountry = function () {
        var current = $state.current.name;
        if (current.substr(0, 6) === 'dbuser') {
            $state.go('dbuser.zone-setting.zone-country');
        }
        else if (current.substr(0, 6) === 'admin.') {
            $state.go('admin.zone-setting.zone-country');
        }

    };
    $scope.zonePostCode = function () {
        var current = $state.current.name;
        if (current.substr(0, 6) === 'dbuser') {
            $state.go('dbuser.zone-setting.zone-postCode');
        }
        else if (current.substr(0, 6) === 'admin.') {
            $state.go('admin.zone-setting.zone-postCode');
        }

    };

    $scope.countryZonePostCode = function () {
        var current = $state.current.name;
        if (current.substr(0, 6) === 'dbuser') {
            $state.go('dbuser.zone-setting.countryzone-postCode');
        }
        else if (current.substr(0, 6) === 'admin.') {
            $state.go('admin.zone-setting.countryzone-postCode');
        }
    };

    $scope.activeZoneCountry = function () {
        if ($state.is('admin.zone-setting.zone-country')) {
            return true;
        }
        else if ($state.is('dbuser.zone-setting.zone-country')) {
            return true;
        }
        return false;
    };
    $scope.activeZonePostCode = function () {
        if ($state.is('admin.zone-setting.zone-postCode')) {
            return true;
        }
        else if ($state.is('dbuser.zone-setting.zone-postCode')) {
            return true;
        }
        return false;
        // return $state.is('admin.zone-setting.zone-postCode');
    };
    $scope.activeCountrtyZonePostCode = function () {
        if ($state.is('admin.zone-setting.countryzone-postCode')) {
            return true;
        }
        else if ($state.is('dbuser.zone-setting.countryzone-postCode')) {
            return true;
        }
        return false;
    };
    $scope.activeBaseRateCard = function () {
        if ($state.is('admin.zone-setting.base-rate-card')) {
            return true;
        }
        else if ($state.is('dbuser.zone-setting.base-rate-card')) {
            return true;
        }
        return false;
        // return $state.is('admin.zone-setting.base-rate-card');
    };
    $scope.activeThirdPartyMartix = function () {
        if ($state.is('admin.zone-setting.third-party-matrix')) {
            return true;
        }
        else if ($state.is('dbuser.zone-setting.third-party-matrix')) {
            return true;
        }
        return false;
        // return $state.is('admin.zone-setting.third-party-matrix');
    };

    $scope.activeRate = function (tab) {
        if ($scope.userInfo) {
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

    };

    $scope.changeState = function (tab) {
        if (tab !== undefined) {
            $state.go(tab.route);
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
            return tab;
        }
    };
    function init() {
        $rootScope.GetServiceValue = '';
        var obj = SessionService.getUser();
        $scope.tabs = obj.tabs;
        $scope.tab = getUserTab($scope.tabs, "Zone_Setting");
        $scope.userInfo = obj;
        if ($scope.userInfo === undefined || $scope.userInfo === null || $scope.userInfo.SessionId === undefined || $scope.userInfo.SessionId === '') {
            $state.go('home.welcome');
        }
        
    }

    init();

});