/** 
 * Controller
// */
angular.module('ngApp.setting').controller('SettingController', function ($scope, $state, $translate,  SettingService, config, $filter,  SessionService, $uibModal, toaster) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['setting', 'ErrorGetting', 'detail', 'FrayteError']).then(function (translations) {
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TextErrorGettingSettingDetail = translations.ErrorGetting + " " + translations.setting + " " + translations.detail;
        });
    };

    $scope.exchangeRate = function () {
        var current = $state.current.name;
        if (current.substr(0, 6) === 'dbuser') {
            $state.go('dbuser.setting.exchange-rate');
        }
        else if (current.substr(0, 6) === 'admin.') {
            $state.go('admin.setting.exchange-rate');
        }

    };
    $scope.terms = function () {
        $state.go('admin.setting.terms-and-condition');
    };
    $scope.downloadExcels = function () {
        $state.go('admin.setting.download-excels');
    };
    $scope.reportSetting = function () {
        $state.go('admin.setting.report-setting');
    };
    $scope.specialDelivery = function () {
        $state.go('admin.setting.special-delivery-needed');
    };
    $scope.parcelHub = function () {
        var current = $state.current.name;
        if (current.substr(0, 6) === 'dbuser') {
            $state.go('dbuser.setting.parcel-hub');
        }
        else if (current.substr(0, 6) === 'admin.') {
            $state.go('admin.setting.parcel-hub');
        }

    };
    $scope.fuelSrucharge = function () {
        var current = $state.current.name;
        if (current.substr(0, 6) === 'dbuser') {
            $state.go('dbuser.setting.fuelSurCharge');
        }
        else if (current.substr(0, 6) === 'admin.') {
            $state.go('admin.setting.fuelSurCharge');
        }

    };
    $scope.disabled = false;

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

    $scope.activerfuelSurCharge = function () {
        if ($state.is('admin.setting.fuelSurCharge')) {
            return true;
        }
        //else if ($state.is('dbuser.setting.fuelSurCharge')) {
        //    return true;
        //}
        return false;
    };
    $scope.activerParcel = function () {
        if ($state.is('admin.setting.parcel-hub')) {
            return true;
        }
        else if ($state.is('dbuser.setting.parcel-hub')) {
            return true;
        }
        return false;
        // return $state.is('admin.setting.parcel-hub');
    };
    function init() {
        var obj = SessionService.getUser();
        $scope.tabs = obj.tabs;
        $scope.tab = getUserTab($scope.tabs, "Settings");

        // set Multilingual Modal Popup Options
        setModalOptions();
        var userInfo = SessionService.getUser();

        if (userInfo === undefined || userInfo === null || userInfo.SessionId === undefined || userInfo.SessionId === '') {
            $state.go('home.welcome');
        }
        else {
            $scope.userInfo = userInfo;

            if (userInfo.RoleId === 1) {//Admin
                $scope.parcelKeyAdmin = true;
            }
            else if (userInfo.RoleId === 6) {//Staff                
                $scope.parcelKeyAdmin = false;
            }
        }
        SettingService.GetPieceDetailsExcelPath().then(function (response) {
            $scope.picesExcelDownload = response.data[0];
            $scope.customerExcelDownload = response.data[1];
            $scope.shipperExcelDownload = response.data[2];
            $scope.receiverExcelDownload = response.data[3];
            $scope.agentExcelDownload = response.data[4];
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGettingSettingDetail,
                showCloseButton: true
            });
        });
    }

    init();

});