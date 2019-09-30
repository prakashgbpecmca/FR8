angular.module('ngApp.ecommerceSetting').controller('EcommerceSettingController', function ($scope, $state, $translate, uiGridConstants, SettingService, config, $filter, LogonService, SessionService, $uibModal, toaster, $rootScope) {

    $scope.changeState = function (tab) {
        if (tab !== undefined) {
            $state.go(tab.route);
        }
    };
    $scope.activeRate = function (tab) {
        if ($scope.userInfo) {
            if ($scope.tab && tab) {

                var currentState = $state.current.name;
                var str = tab.route;
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
        $scope.tab = getUserTab($scope.tabs, "eComm_Setting");
    }
    init();
});