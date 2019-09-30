angular.module('ngApp.profileandsetting').controller('ProfileandSettingController', function (AppSpinner, $scope, ProfileandSettingService, SessionService, toaster, $state) {
    $scope.showHideTab = function (route) {
        if (route && $scope.UserInfo && $scope.UserInfo.UserType === 'SPECIAL' && (route.route.search("userTabs.profile-setting.api-detail") > -1 || route.route.search("userTabs.profile-setting.service-code") > -1)) {
            return true;
        }
        return false;
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
        var obj = SessionService.getUser();
        $scope.UserInfo = obj;
        $scope.RoleId = obj.RoleId;
        var CustomerId = obj.CustomerId;
        $scope.tabs = obj.tabs;
        $scope.tab = getUserTab($scope.tabs, "Profile_Setting");

        //if ($scope.tab && $scope.tab.childTabs) {
        //    for (var i = 0 ; i < $scope.tab.childTabs.length ; i++) {
        //        if ($scope.UserInfo && $scope.UserInfo.UserType === 'SPECIAL') {
        //            if ($scope.tab.childTabs[i].route.search("userTabs.profile-setting.api-detail") > -1) {
        //                $scope.tab.childTabs.splice(i, 1);
        //            }
        //            if ($scope.tab.childTabs[i].route.search("userTabs.profile-setting.service-code") > -1)
        //            {
        //                $scope.tab.childTabs.splice(i, 1);
        //            }
        //        }
        //    }
        //}
    }
    init();
});