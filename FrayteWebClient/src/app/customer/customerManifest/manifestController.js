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

    function init() {
        var userInfo = SessionService.getUser();
        //$scope.TrackManifest.UserId = userInfo.EmployeeId;


        if (userInfo === undefined || userInfo === null || userInfo.SessionId === undefined || userInfo.SessionId === '') {
            $state.go('home.welcome');
        }
        else {
            $scope.userInfo = userInfo;
            $scope.tabs = $scope.userInfo.tabs;


        }
        $scope.tab = getUserTab($scope.tabs, "Manifests");
    }

    init();
});