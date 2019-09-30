/** 
 * Controller
 */
angular.module('ngApp.loginview').controller('UserTabController', function ($rootScope, localStorage, $scope, HomeService, DownloadExcelService, $state, $location, $translate, config, $filter, LogonService, SessionService, $uibModal, toaster, SystemAlertService, DateFormatChange, UtilityService, $timeout, $window) {


    //breakbulk view manifest code
    $scope.breakbulkViewManifest = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'breakbulkManifestGeneratorController',
            templateUrl: 'loginView/breakbulkManifestGenerator.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end      

    //express solution detail code
    $scope.expressViewManifest = function () {
        $state.go('loginView.userTabs.manifests-es', {}, { reload: true }, { location: false });
        $rootScope.ChangeExpressManifest = true;
    };
    //end   

    $scope.showHideTab = function (route) {
        if (route && $scope.userInfo && $scope.userInfo.UserType === 'SPECIAL' && route.route.search("userTabs.db-upload-shipments") > -1) {
            return true;
        }
        if (route && $scope.userInfo && $scope.userInfo.RoleId === 3 && $scope.userInfo.UserType === 'NORMAL' && (route.route.search("userTabs.customers") > -1)) {
            return true;
        }
        if (route && $scope.userInfo && $scope.userInfo.RoleId === 3 && $scope.userInfo.UserType === 'NORMAL' && (route.route.search("userTabs.setting.terms-and-conditions") > -1)) {
            return true;
        }
        if (route && $scope.userInfo && $scope.userInfo.RoleId === 3 && !$scope.userInfo.IsRateShow && (route.route.search("userTabs.quotation") > -1)) {
            return true;
        }
        return false;
    };

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['setting', 'ErrorGetting', 'detail', 'FrayteError']).then(function (translations) {
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TextErrorGettingSettingDetail = translations.ErrorGetting + " " + translations.setting + " " + translations.detail;
        });
        $translate(['access', 'FrayteError', 'information', 'ErrorGetting']).then(function (translations) {
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TextErrorGettingAccessInfo = translations.ErrorGetting + " " + translations.access + " " + translations.information;
        });
    };

    $scope.$on("updateUserInfo", function (event, userInfoDetail) {
        $scope.userInfo = userInfoDetail;
    });

    $scope.$on("UpdateShipmentTabCaption", function (event, tabDetail) {
        if ($scope.tabs !== undefined) {
            $scope.tabs.forEach(function (tab) {
                //if (tab.heading === 'New Shipment') {
                //    tab.heading = 'Modify Shipment';
                //}
            });
        }
    });

    //$scope.go = function (route) {

    //    $scope.Route = route;
    //    //if ($scope.isShow && route !== "customer.manifests") {
    //    if ($scope.isShow) {

    //        $rootScope.ShowManifestButton = true;
    //    }
    //    else {
    //        $rootScope.ShowManifestButton = false;
    //    }

    //    if (route === 'customer.upload-shipments') {
    //        //$state.go(route, { Id: 0, UserType: 5 });
    //        //$state.go(route);
    //        $rootScope.ServiceType = 'Empty';
    //    }

    //    if (route === 'shipper.shipment.addressdetail') {
    //        $state.go(route, { Id: 0, UserType: 5 });
    //    }
    //    if (route === "customer.booking-home.direct-booking") {
    //        $state.go(route, { directShipmentId: 0 });
    //    }
    //    else {
    //        $state.go(route);
    //    }
    //    $scope.menu2.navigationState = route;
    //};

    $scope.active = function (tab) {
        var flag = false;
        var route = $state.is(tab.route);
        if (!route) {
            if (tab.childTabs !== null && tab.childTabs) {
                var current1 = $state.current.name;
                for (var j = 0; j < tab.childTabs.length; j++) {
                    if (current1 === tab.childTabs[j].route) {
                        flag = true;
                        break;
                    }
                }

                return flag;
            }
            else {
                //
            }
        }
        else {
            return $state.is(tab.route);
        }


        //else {
        //    if (!route) {
        //        if (tab.childTabs !== null && tab.childTabs) {
        //            var current = $state.current.name;
        //            for (var i = 0; i < tab.childTabs.length; i++) {
        //                if (current === tab.childTabs[i].route) {
        //                    flag = true;
        //                    break;
        //                }
        //            }

        //            return flag;
        //        }
        //    }
        //    else {
        //        return $state.is(tab.route);
        //    }
        //}


    };

    //$scope.$on("$stateChangeSuccess", function () {
    //    tabActiveDiactive();
    //});

    var tabActiveDiactive = function () {
        if ($scope.tabs !== undefined) {
            $scope.tabs.forEach(function (tab) {
                tab.active = $scope.active(tab.route);
            });

            if ($state.is('admin.customer-detail')) {
                $scope.tabs[0].active = true;
            }

            if ($state.is('admin.customerbranchdetail')) {
                $scope.tabs[0].active = true;
            }

            if ($state.current.name.indexOf('receiver-detail') >= 0) {
                //Make receiver tab as active
                $scope.tabs.forEach(function (tab) {
                    if (tab.heading === 'Receivers') {
                        tab.active = true;
                    }
                });
            }

            if ($state.current.name.indexOf('shippers-detail') >= 0) {
                //Make shipper tab as active
                $scope.tabs.forEach(function (tab) {
                    if (tab.heading === 'Shippers') {
                        tab.active = true;
                    }
                });
            }

            if ($state.is('admin.agent-detail')) {
                $scope.tabs[5].active = true;
            }

            if ($state.is('admin.user-detail')) {
                $scope.tabs[2].active = true;
            }

            if ($state.is('admin.warehouse-detail')) {
                $scope.tabs[9].active = true;
            }

            if ($state.is('admin.country-detail')) {
                $scope.tabs[7].active = true;
            }

            $scope.currentScope = $state.current;
        }
    };
    //Create Manifest
    $scope.isShowManifestFirst = function () {
        if ($scope.isShow) {
            var current = $state.current.name;
            if (current.search("manifest.userManifest") > -1) {
                return false;
            }
            return true;
        }
        return false;
    };
    $scope.isShowManifestSecond = function () {
        if ($scope.isShow) {
            var current = $state.current.name;
            if (current.search("manifest.userManifest") > -1) {
                return true;
            }
            return false;
        }
        return false;
    };

    $scope.CreateManifestHome = function () {
        //$rootScope.ShowManifestButton = false;
        $state.go('loginView.userTabs.manifest.userManifest', {}, { reload: true });

        //if ($scope.ManifestDetail.Services.length > 1) {

        //}
        //    if ($state.current.name === 'msadmin.booking-home.direct-booking' ||
        //$state.current.name === 'admin.booking-home.direct-booking' ||
        //$state.current.name === 'customer.booking-home.direct-booking' ||
        //$state.current.name === 'dbuser.booking-home.direct-booking') {
        $rootScope.ChangeManifest = true;

        //}
        //else {
        //    $rootScope.ChangeManifest = false;
        //}

        //var ModalInstance = $uibModal.open({
        //    animation: true,
        //    controller: 'CreateManifestController',
        //    templateUrl: 'customer/customerManifest/createManifest/createManifest.tpl.html',
        //    backdrop: true,
        //    resolve: {
        //        TrackObj: function () {
        //            return {
        //                ModuleType: $scope.ManifestDetail.Services[0].ServiceName
        //            };
        //        }

        //    },
        //    size: 'lg',
        //    keyboard: false

        //});
    };


    var isMenifestVisible = function () {
        HomeService.IsManifestSupprt($scope.userInfo.EmployeeId).then(function (response) {
            if (response.data != null) {
                $scope.ManifestDetail = response.data;
                $rootScope.isShow = response.data.ManifestSupport;
            }
            else {
                $rootScope.isShow = false;
            }
            if ($rootScope.isShow) {
                $rootScope.ShowManifestButton = true;
            }
            else {
                $rootScope.ShowManifestButton = false;
            }

        }, function () {
            $rootScope.ShowManifestButton = false;
        });
    };

    function init() {
        $scope.menu2 =
           {
               navigationState: ''
           };

        $scope.ImagePath = config.BUILD_URL;
        var userInfo = SessionService.getUser();
        $scope.isShow = true;
        $scope.module = SessionService.getModuleType();
        setModalOptions();

        if (userInfo === undefined || userInfo === null || userInfo.SessionId === undefined || userInfo.SessionId === '') {
            window.location.href = config.Login_Link;
        }
        else {
            $scope.userInfo = userInfo;
            $rootScope.frayteTabs = $scope.userInfo.tabs;
            for (i = 0; i < $rootScope.frayteTabs.length; i++) {
                if ($scope.RoleId === 3 && $rootScope.frayteTabs[i].route === "loginView.userTabs.tradelane-shipments") {
                    $rootScope.frayteTabs[i].tabKey = "My_Shipments";
                }
            }

        }

        if (userInfo.OperationZoneId === 2 && userInfo.RoleId === 3) {
            $rootScope.ShowManifestButton = true;
        }
        else {
            $rootScope.ShowManifestButton = false;
        }

        //Set menu2.navigationState for More+ tab functionality
        $scope.menu2.navigationState = $state.current.name;

        isMenifestVisible();

    }

    init();

});