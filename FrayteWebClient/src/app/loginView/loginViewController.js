/** 
 * Controller
 */
angular.module('ngApp.loginview').controller('LoginViewController', function ($rootScope, $scope, HomeService, DownloadExcelService, $state, $location, $translate, config, $filter, LogonService, SessionService, $uibModal, toaster, SystemAlertService, DateFormatChange) {

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
    $scope.ProfileModal = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'profile/profile.tpl.html',
            controller: 'ProfileController',
            windowClass: '',
            size: 'md'
        });
    };

    $scope.go = function (route) {
        $scope.Route = route;
        if ($scope.isShow && route !== "customer.manifests") {
            $scope.ShowManifestButton = true;
        }
        else {
            $scope.ShowManifestButton = false;
        }

        if (route === 'shipper.shipment.addressdetail') {
            $state.go(route, { Id: 0, UserType: 5 });
        }
        if (route === "customer.booking-home.direct-booking") {
            $state.go(route, { directShipmentId: 0 });
        }
        else {
            $state.go(route);
        }
        $scope.menu2.navigationState = route;
    };

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

    $scope.Logout = function () {
        SessionService.removeUser();
        $state.go('home.welcome');
    };

    $scope.rootState = function () {

        if ($scope.userInfo.RoleId === 1) {//Admin
            //$state.go('admin.direct-shipments', {}, { reload: true });
            $state.go('admin.booking-home.booking-welcome', {}, { reload: true });
        }
        else if ($scope.userInfo.RoleId === 3) {//Customer                                        
            //$state.go("customer.direct-shipments", {}, { reload: true });
            $state.go("customer.booking-home.booking-welcome", {}, { reload: true });
        }

        else if ($scope.userInfo.RoleId === 6) {//Staff                
            //$state.go('dbuser.direct-shipments', {}, { reload: true });
            $state.go('dbuser.booking-home.booking-welcome', {}, { reload: true });
        }

    };

    //Create Manifest

    $scope.CreateManifestHome = function () {
        $scope.ShowManifestButton = false;
        $state.go('customer.manifests', {}, { reload: true });
        
        if ($scope.ManifestDetail.Services.length > 1) {

        }
        var ModalInstance = $uibModal.open({
            animation: true,
            controller: 'CreateManifestController',
            templateUrl: 'customer/customerManifest/createManifest/createManifest.tpl.html',
            backdrop: true,
            resolve : {
                TrackObj: function () {
                    return {
                        ModuleType: $scope.ManifestDetail.Services[0].ServiceName 
                    };
                }

            },
            size: 'lg',
            keyboard: false

        });
    };

    //getsystemalert

    $rootScope.GetSystemAlert = function (OperationZoneId) {
        var CurrentDate = new Date();
        SystemAlertService.GetPublicSystemAlert(OperationZoneId, CurrentDate).then(function (response) {
            $scope.result = response.data;
            $scope.result.reverse();
            if ($scope.result.length > 0) {
                $scope.reslength = false;
                  $scope.val = false;
                $scope.Month = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                for (i = 0; i < $scope.result.length; i++) {
                    var Newdate = $scope.result[i].FromDate;

                    var newdate = [];
                    var Date = [];
                    var Time = [];
                    var gtMonth1 = [];
                    newdate = Newdate.split('T');
                    Date = newdate[0].split('-');
                    var gtDate = Date[2];
                    gtMonth1 = Date[1].split('');
                    var gtMonth2 = Date[1];
                    var gtMonth3 = gtMonth1[0] === "0" ? gtMonth1[1] : gtMonth2;
                    var gtMonth4 = gtMonth3--;
                    var gtMonth = $scope.Month[gtMonth3];
                    var gtYear = Date[0];
                    Time = $scope.result[i].FromTime.split(':');
                    var gtHour = Time[0];
                    var gtMin = Time[1];


                    $scope.result[i].Date = gtDate + "-" + gtMonth + "-" + gtYear + " - " + gtHour + ":" + gtMin;

                }
            }
            else {
                $rootScope.reslength = true;
            }
            if ($scope.result.length > 0) {
                $rootScope.reslength = false;
                $scope.Count = $scope.result.length;
                $scope.PublicPageDate = $scope.result[0].Date;
                $scope.GMT = $scope.result[0].TimeZoneDetail.OffsetShort;
            }
            else {
                $rootScope.reslength = true;
            }
        });
      

    };


    var isMenifestVisible = function () {
        HomeService.IsManifestSupprt($scope.userInfo.EmployeeId).then(function (response) {
            if (response.data != null) {
                $scope.ManifestDetail = response.data;
                $scope.isShow = response.data.ManifestSupport;
            }
            else {
                $scope.isShow = false;
            }
            if ($scope.isShow && !$state.is("customer.manifests")) {
                $scope.ShowManifestButton = true;
            }
            else {
                $scope.ShowManifestButton = false;
            }

        }, function () {
            $scope.ShowManifestButton = false;
        });

    };

    function init() {
        $scope.isCollapsed = false;
        // Load Download Excel Link
        DownloadExcelService.GetPieceDetailsExcelPath().then(function (response) {

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

        // set Multilingual Modal Popup Options
        setModalOptions();

        $scope.menu2 =
            {
                navigationState: ''
            };

        $scope.ImagePath = config.BUILD_URL;
        var userInfo = SessionService.getUser();
       
        if (userInfo === undefined || userInfo === null || userInfo.SessionId === undefined || userInfo.SessionId === '') {
            $state.go('home.welcome');
        }
        else {
            $scope.userInfo = userInfo;
            $scope.tabs = $scope.userInfo.tabs;
            //if (userInfo.RoleId === 1) {//Admin
            //    $scope.frayteRootState = 'admin.direct-shipments';
            //}
            //else if (userInfo.RoleId === 3) {//Customer                                        
            //    $scope.frayteRootState = 'customer.current-shipment';
            //}
            //else if (userInfo.RoleId === 5) {//Shipper                                        
            //    $scope.frayteRootState = 'shipper.current-shipment';
            //}
            //else if (userInfo.RoleId === 6) {//Staff                
            //    $scope.frayteRootState = 'dbuser.direct-shipments';
            //}
            //else if (userInfo.RoleId === 7) {//Warehouse
            //    $scope.frayteRootState = 'user.current-shipment';
            //}

            //LogonService.getUserTabs(userInfo.EmployeeId, $scope.userInfo.RoleId).then(function (response) {
            //    if (response.status == 200) {
            //        $scope.tabs = response.data;
            //        var obj = SessionService.getUser();
            //        obj.tabs = response.data;
            //        SessionService.setUser(obj);
            //    }
            //    else {
            //        toaster.pop({
            //            type: 'error',
            //            title: $scope.TitleFrayteError,
            //            body: $scope.TextErrorGettingAccessInfo,
            //            showCloseButton: true
            //        });
            //    }
            //});
        }

        if (userInfo.OperationZoneId === 2 && userInfo.RoleId === 3) {
            $scope.ShowManifestButton = true;
        }
        else {
            $scope.ShowManifestButton = false;
        }

        //Set menu2.navigationState for More+ tab functionality
        $scope.menu2.navigationState = $state.current.name;

        $scope.quickBooingIcon = {
            value: false
        };
        var OperationZoneId = $scope.userInfo.OperationZoneId;
        $rootScope.GetSystemAlert(OperationZoneId);
        if ($scope.userInfo !== undefined && $scope.userInfo !== null) {
            isMenifestVisible();
        }
        else {
            $scope.isShow = false;
            $scope.ShowManifestButton = false;
        }
        //$scope.MonthName = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        $scope.DayName = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
        $scope.CurrentDate = new Date();
        $scope.CurrentDay = $scope.DayName[$scope.CurrentDate.getDay()];
        //$scope.CurrentMonthName = $scope.MonthName[$scope.CurrentDate.getMonth()];
        
        //$scope.CurrentYear = $scope.CurrentDate.getFullYear();
        //$scope.CurrentDate1 = $scope.CurrentDate.getDate();
        //if ($scope.CurrentDate1 === 1 || $scope.CurrentDate1 === 21 || $scope.CurrentDate1 === 31) {
        //    $scope.CurrentDate1 = $scope.CurrentDate1 + "st";
        //}
        //else if ($scope.CurrentDate1 === 2 || $scope.CurrentDate1 === 22) {
        //    $scope.CurrentDate1 = $scope.CurrentDate1 + "nd";
        //}
        //else if ($scope.CurrentDate1 === 3 || $scope.CurrentDate1 === 23) {
        //    $scope.CurrentDate1 = $scope.CurrentDate1 + "rd";
        //}
        //else {
        //    $scope.CurrentDate1 = $scope.CurrentDate1 + "th";
        //}
        //$scope.CurrentDateTime = $scope.CurrentDay + ", " + $scope.CurrentDate1 + " " + $scope.CurrentMonthName + " " + $scope.CurrentYear;
        $scope.CurrentDateTime = $scope.CurrentDay + ", " + DateFormatChange.DateFormatChange(new Date());
    }

    init();

});