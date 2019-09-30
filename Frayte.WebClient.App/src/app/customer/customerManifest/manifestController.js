angular.module('ngApp.customer').controller('ManifestController', function ($scope, $state, $location, $filter, $translate, SessionService, $uibModal, $log, uiGridConstants, toaster, ModalService) {

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

    $scope.changeState = function (tab) {
        if (tab !== undefined) {
            $state.go(tab.route);
        }
    };

    function init() {
        var userInfo = SessionService.getUser();

        if (userInfo === undefined || userInfo === null || userInfo.SessionId === undefined || userInfo.SessionId === '') {
            $state.go('home.welcome');
        }
        else {
            $scope.userInfo = userInfo;
            $scope.tabs = $scope.userInfo.tabs;
        }

        $scope.tab = getUserTab($scope.tabs, "Manifests");

        if ($scope.tab && $scope.tab.childTabs && $scope.tab.childTabs.length) {
            for (var i = 0 ; i < $scope.tab.childTabs.length ; i++) {
                if ($scope.tab.childTabs[i].tabKey === "Custom_Manifest" && $scope.userInfo.RoleId !== 6) {
                    $scope.tab.childTabs.splice(i, 1);
                }
            }
        }            
    }

    init();
});