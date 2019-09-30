angular.module('ngApp.courierAccount').controller('CourierAccountController', function ($scope, config, AppSpinner, UtilityService, $state, CourierAccountService, $translate, SessionService, toaster, uiGridConstants, $uibModal, ModalService, $rootScope) {

    $scope.CourierAccount = {
        LogisticServiceCourierAccountId: 0,
        IntegrationAccountId: '',
        AccountNo: '',
        AccountCountryCode: '',
        Description: '',
        ColorCode: '',
        IsActive: true,
        OperationZone: {
            OperationZoneId: null,
            OperationZoneName: ''
        },
        LogisticService: {
            LogisticServiceId: 0,
            OperationZoneId: null,
            LogisticCompany: '',
            LogisticCompanyDisplay: '',
            LogisticType: '',
            LogisticTypeDisplay: '',
            RateType: '',
            RateTypeDisplay: '',
            ModuleType: 'DirectBooking'
        }
    };

    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors',
            'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation',
            'PleaseCorrectValidationErrors', 'RateCardSave_Validation', 'Select_CourierAccount', 'THP_Matrix_Saved', 'DataSaved_Validation',
            'Record_Saved', 'APIkeyUserkey_Saved_Validation', 'ParcelHubKeysSavingErrorValidation', 'PleaseTryLater', 'ParcelHubKeyRemoved_Validation',
            'PacelHub', 'Keys', 'SelectLogisticType_Validation', 'Cancel', 'Confirmation', 'Cancel_Validation', 'DeleteBody', 'Courier_Account', 'Delete', 'Delete_Confirmation',
            'Delete_Courier_Account', 'Successfull', 'Courier_Account_Deleted_Successfully', 'LoadingCourierAccounts']).then(function (translations) {

                $scope.TitleFrayteError = translations.FrayteError;
                $scope.TitleFrayteInformation = translations.FrayteInformation;
                $scope.TitleFrayteValidation = translations.FrayteValidation;
                $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
                $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
                $scope.RecordGettingError = translations.ErrorGetting + " " + translations.records;
                $scope.Success = translations.Success;
                $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
                $scope.PleaseCorrectValidationErrors = translations.PleaseCorrectValidationErrors;
                $scope.RecordSaved = translations.Record_Saved;
                $scope.Header_Delete = translations.Delete_Confirmation;
                $scope.Deleted_Courier_Account = translations.Delete_Courier_Account;
                $scope.Successfully = translations.Successfull;
                $scope.CourierAccountDeletedSuccessfully = translations.Courier_Account_Deleted_Successfully;
                $scope.LoadingCourierAccounts = translations.LoadingCourierAccounts;
                $scope.ParcelHubKeyRemovedTryLaterValidation = translations.ParcelHubKeysSavingError_Validation + " " + translations.PleaseTryLater;
                $scope.ParcelHubKeyGettingValidation = translations.ErrorGetting + " " + translations.PacelHub + " " + translations.Keys;
                $scope.SelectLogisticTypeValidation = translations.SelectLogisticType_Validation;
                $scope.CancelConfirmation = translations.Cancel + " " + translations.Confirmation;
                $scope.DeleteConfirmation = translations.Delete + " " + translations.Confirmation;
                $scope.CancelValidation = translations.Cancel_Validation;
                $scope.DeleteCourierAcc = translations.DeleteBody + translations.Courier_Account + "?";
                $scope.CourierAccountRemovedValidation = translations.CourierAccountRemovedValidation;

            });
    };
    //$scope.ZoneCountryByOperationZone = function (OperationZone) {
    //    getScreenInitials(OperationZone);
    //};

    $scope.parcelHubCourierAccount = function () {
        var current = $state.current.name;
        if (current.substr(0, 6) === 'dbuser') {
            $state.go('dbuser.courier-accounts.parcel-hub-account', {}, { reload: true });
        }
        else if (current.substr(0, 6) === 'admin.') {
            $state.go('admin.courier-accounts.parcel-hub-account', {}, { reload: true });
        }

    };

    $scope.easyPostCourierAccount = function () {
        var current = $state.current.name;
        if (current.substr(0, 6) === 'dbuser') {
            $state.go('dbuser.courier-accounts.easypost-account');
        }
        else if (current.substr(0, 6) === 'admin.') {
            $state.go('admin.courier-accounts.easypost-account', {}, { reload: true });
        }
    };

    $scope.activeEasyPost = function () {
        if ($state.is('admin.courier-accounts.easypost-account')) {
            return true;
        }
        //else if ($state.is('dbuser.setting.fuelSurCharge')) {
        //    return true;
        //}
        return false;
    };

    $scope.activeParcelHub = function () {
        if ($state.is('admin.courier-accounts.parcel-hub-account')) {
            return true;
        }
        //else if ($state.is('dbuser.setting.parcel-hub-account')) {
        //    return true;
        //}
        return false;
        // return $state.is('admin.setting.parcel-hub-account');
    };

    $scope.removeCourierAccount = function (row) {
        var modalOptions = {
            headerText: $scope.Header_Delete,
            bodyText: $scope.Deleted_Courier_Account
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            CourierAccountService.DeleteCourierAccount(row.entity.LogisticServiceCourierAccountId).then(function (response) {
                if (response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteInformation,
                        body: $scope.CourierAccountDeletedSuccessfully,
                        showCloseButton: true
                    });
                    init();
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.CourierAccountRemovingError_Validation,
                        showCloseButton: true
                    });
                }
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.CourierAccountRemovingError_Validation,
                    showCloseButton: true
                });
            });
        });
    };

    $scope.AddEditCourierAccount = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'courierAccount/courierAccountAddEdit.tpl.html',
            controller: 'CourierAccountAddEditController',
            windowClass: '',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                mode: function () {
                    if (row === undefined) {
                        return 'Add';
                    }
                    else {
                        return 'Edit';
                    }
                },
                CourierAccount: function () {
                    if (row === undefined) {
                        return {

                            LogisticServiceCourierAccountId: 0,
                            IntegrationAccountId: '',
                            AccountNo: '',
                            AccountCountryCode: '',
                            Description: '',
                            ColorCode: '',
                            IsActive: true,
                            OperationZone: {
                                OperationZoneId: null,
                                OperationZoneName: ''
                            },
                            LogisticService: {
                                LogisticServiceId: 0,
                                OperationZoneId: null,
                                LogisticCompany: '',
                                LogisticCompanyDisplay: '',
                                LogisticType: '',
                                LogisticTypeDisplay: '',
                                RateType: '',
                                RateTypeDisplay: '',
                                ModuleType: 'DirectBooking'
                            }

                        };
                    }
                    else {
                        return row.entity;
                    }
                }
            }
        });
        modalInstance.result.then(function (countryAccount) {
            getCourierAccounts();
        }, function () {
        });
    };

    var logisticTypeTemplate = '<div class="ui-grid-cell-contents">{{grid.appScope.GetLogistictype(row)}}</div>';

    $scope.SetGridOptions = function () {
        $scope.gridOptions = {
            showFooter: true,
            enableSorting: true,
            multiSelect: false,
            enableFiltering: true,
            enableRowSelection: true,
            enableSelectAll: false,
            enableRowHeaderSelection: false,
            selectionRowHeaderWidth: 35,
            noUnselect: true,
            enableGridMenu: false,
            enableColumnMenus: false,
            enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
            enableVerticalScrollbar: true,
            columnDefs: [
              {
                  name: 'OperationZone.OperationZoneName', displayName: 'Business_Unit', headerCellFilter: 'translate', width: '15%', sort: {
                      direction: uiGridConstants.ASC,
                      priority: 0
                  }
              },
              { name: 'LogisticService.LogisticCompany', displayName: 'Logistics_Company', headerCellFilter: 'translate', width: '15%' },
              //{ name: 'IsImport', displayName: 'Logistic_Type', headerCellFilter: 'translate', cellTemplate: logisticTypeTemplate },
              { name: 'LogisticService.LogisticType', displayName: 'Logistic_Type', headerCellFilter: 'translate', width: '13%' },
              //{ name: 'IsExport', displayName: 'Export' },
              //{ name: 'IsThirdParty', displayName: 'Third Party' },
              //{ name: 'IsDomesticUK', displayName: 'UKShipment' },
              //{ name: 'IsEUImport', displayName: 'EUImport' },
              //{ name: 'IsEUExport', displayName: 'EUExport' },
              { name: 'IntegrationAccountId', displayName: 'Account_ID', headerCellFilter: 'translate', width: '14%' },
              { name: 'AccountNo', displayName: 'Account_No', headerCellFilter: 'translate', width: '10%' },
              { name: 'AccountCountryCode', displayName: 'Country', headerCellFilter: 'translate', width: '10%' },
              { name: 'Description', displayName: 'Description', headerCellFilter: 'translate', width: '12%' },
              { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "courierAccount/courierAccountEditButton.tpl.html" }
            ]
        };
    };

    $scope.toggleRowSelection = function () {
        $scope.gridApi.selection.clearSelectedRows();
        $scope.gridOptions.enableRowSelection = !$scope.gridOptions.enableRowSelection;
        $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS);
    };

    $scope.SearchZoneCountry = function () {
        AppSpinner.showSpinnerTemplate($scope.LoadingCourierAccounts, $scope.Template);
        var OperationZoneId = 0;
        var LogisticType = "";
        var CourierCompany = "";
        var RateType = "";
        var ModuleType = "DirectBooking";


        if ($scope.OperationZone) {
            OperationZoneId = $scope.OperationZone.OperationZoneId;
        }

        if ($scope.LogisticType) {
            LogisticType = $scope.LogisticType.Value;
        }
        if ($scope.CourierCompany) {
            CourierCompany = $scope.CourierCompany.Value;
        }

        if ($scope.RateType) {
            RateType = $scope.RateType.Value;
        }

        CourierAccountService.GetCourierAccounts(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType).then(function (response) {

            $scope.Accounts = response.data;
            $scope.EasyPostCourierAccounts = [];
            $scope.ParcelHubCourierAccounts = [];
            for (var i = 0; i < $scope.Accounts.length; i++) {
                if ($scope.Accounts[i].LogisticService.LogisticCompany === "UKMail" || $scope.Accounts[i].LogisticService.LogisticCompany === "Yodel" || $scope.Accounts[i].LogisticService.LogisticCompany === "Hermes" || $scope.Accounts[i].LogisticService.LogisticCompany === 'DHL') {
                    $scope.ParcelHubCourierAccounts.push($scope.Accounts[i]);
                }
                else {
                    $scope.EasyPostCourierAccounts.push($scope.Accounts[i]);

                }
            }
            var currentState = $state.current.name;

            var route1 = UtilityService.GetCurrentRoute($scope.tab.childTabs, "userTabs.courier-accounts.parcel-hub-account");
            var route2 = UtilityService.GetCurrentRoute($scope.tab.childTabs, "userTabs.courier-accounts.easypost-account");

            if (currentState === route1) {
                $scope.gridOptions.data = $scope.ParcelHubCourierAccounts;
                if ($scope.ParcelHubCourierAccounts.length === 0) {
                    $scope.ParcelHubflag = true;
                }
                else {
                    $scope.ParcelHubflag = false;
                }
            }
            if (currentState === route2) {
                $scope.gridOptions.data = $scope.EasyPostCourierAccounts;
                if ($scope.EasyPostCourierAccounts.length === 0) {
                    $scope.EasyPostflag = true;
                }
                else {
                    $scope.EasyPostflag = false;
                }
            }
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
            AppSpinner.hideSpinnerTemplate();
        });
    };

    $scope.ZoneCountryByCourierCompany = function () {
        $scope.RateTypes = UtilityService.getRateTypesByCourierCompany($scope.logisticServices, $scope.CourierCompany.Value, $scope.LogisticType.Value);

        if ($scope.RateTypes.length) {
            $scope.RateType = $scope.RateTypes[0];
        }
    };

    $scope.ZoneCountryByLogisticType = function () {
        $scope.CourierCompanies = UtilityService.getLogisticCompaniesByLogisticType($scope.logisticServices, $scope.LogisticType.Value);
        $scope.CourierCompany = $scope.CourierCompanies[0];

        $scope.RateTypes = UtilityService.getRateTypesByCourierCompany($scope.logisticServices, $scope.CourierCompany.Value, $scope.LogisticType.Value);

        if ($scope.RateTypes.length) {
            $scope.RateType = $scope.RateTypes[0];
        }
        
        //var LogisticCompany = [];

        //if ($scope.CourierAccount.LogisticService.LogisticType.Value === "UKShipment") {
        //    CourierAccountService.GetInfo($scope.CourierAccount.OperationZone.OperationZoneId).then(function (response) {


        //        $scope.LogisticCompanies = response.data.LogisticCompanies;
        //        for (i = 0; i < $scope.LogisticCompanies.length; i++) {

        //            if ($scope.LogisticCompanies[i].Value === "UKMail" || $scope.LogisticCompanies[i].Value === "Yodel" || $scope.LogisticCompanies[i].Value === "Hermes") {
        //                LogisticCompany.push($scope.LogisticCompanies[i]);

        //            }

        //        }
        //        $scope.LogisticCompanies = LogisticCompany;
        //        $scope.CourierAccount.LogisticService.LogisticCompany = LogisticCompany[0];
        //    });
        //}
        //else if ($scope.CourierAccount.LogisticService.LogisticType.Value !== "UKShipment") {
        //    CourierAccountService.GetInfo($scope.CourierAccount.OperationZone.OperationZoneId).then(function (response) {
        //        $scope.LogisticCompanies = response.data.LogisticCompanies;
        //        for (i = 0; i < $scope.LogisticCompanies.length; i++) {
        //            LogisticCompany.push($scope.LogisticCompanies[i]);
        //        }
        //        $scope.LogisticCompanies = LogisticCompany;
        //        $scope.CourierAccount.LogisticService.LogisticCompany = LogisticCompany[0];
        //    });
        //}
    };

    $scope.GetInfo = function () {
        UtilityService.getLogisticServices($scope.CourierAccount.OperationZone.OperationZoneId).then(function (response) {
            $scope.OperationZone = $scope.CourierAccount.OperationZone;

            $scope.logisticServices = response.data;

            $scope.LogisticTypes = UtilityService.getLogisticTypesByOperationZone($scope.logisticServices, $scope.OperationZone.OperationZoneId);
            $scope.LogisticType = $scope.LogisticTypes[0];

            $scope.CourierCompanies = UtilityService.getLogisticCompaniesByLogisticType($scope.logisticServices, $scope.LogisticType.Value);
            $scope.CourierCompany = $scope.CourierCompanies[0];

            $scope.RateTypes = UtilityService.getRateTypesByCourierCompany($scope.logisticServices, $scope.CourierCompany.Value, $scope.LogisticType.Value);
            if ($scope.RateTypes.length) {
                $scope.RateType = $scope.RateTypes[0];
            }


            //$scope.LogisticTypes = UtilityService.getLogisticTypesByOperationZone($scope.logisticServices, $scope.OperationZone.OperationZoneId);
            //if ($scope.LogisticTypes) {
            //    $scope.CourierAccount.LogisticService.LogisticType = $scope.LogisticTypes[0];
            //}
            //$scope.LogisticType = $scope.LogisticTypes[0];

            //$scope.CourierCompanies = UtilityService.getLogisticCompaniesByLogisticType($scope.logisticServices, $scope.LogisticType.Value);
            //if ($scope.LogisticCompanies.length) {
            //    $scope.CourierAccount.LogisticService.LogisticCompany = $scope.LogisticCompanies[0];
            //    $scope.CourierCompany = $scope.LogisticCompanies[0];
            //}
        
            //$scope.LogisticRateTypes = UtilityService.getRateTypesByCourierCompany($scope.logisticServices, $scope.CourierCompany.Value, $scope.LogisticType.Value);
            //if ($scope.LogisticRateTypes.length) {
            //    $scope.RateType = $scope.LogisticRateTypes[0];
            //    $scope.CourierAccount.LogisticService.LogisticRateType = $scope.LogisticRateTypes[0];
            //}
             
            $scope.ModuleType = "DirectBooking";

            getCourierAccounts();

        });
    };

    var getCourierAccounts = function () {
        AppSpinner.showSpinnerTemplate($scope.LoadingCourierAccounts, $scope.Template);

        var OperationZoneId = 0;
        var LogisticType = "";
        var CourierCompany = "";
        var RateType = "";
        var ModuleType = "DirectBooking";


        if ($scope.OperationZone) {
            OperationZoneId = $scope.OperationZone.OperationZoneId;
        }

        if ($scope.LogisticType) {
            LogisticType = $scope.LogisticType.Value;
        }
        if ($scope.CourierCompany) {
            CourierCompany = $scope.CourierCompany.Value;
        }

        if ($scope.RateType) {
            RateType = $scope.RateType.Value;
        }

        CourierAccountService.GetCourierAccounts(OperationZoneId, LogisticType, CourierCompany, RateType, $scope.ModuleType).then(function (response) {

            $scope.Accounts = response.data;
            $scope.EasyPostCourierAccountsList = [];
            $scope.ParcelHubCourierAccountsList = [];
            for (var i = 0; i < $scope.Accounts.length; i++) {
                if ($scope.Accounts[i].LogisticService.LogisticCompany === "UKMail" || $scope.Accounts[i].LogisticService.LogisticCompany === "Yodel" || $scope.Accounts[i].LogisticService.LogisticCompany === "Hermes") {
                    $scope.ParcelHubCourierAccountsList.push($scope.Accounts[i]);
                }
                else {
                    $scope.EasyPostCourierAccountsList.push($scope.Accounts[i]);
                }
            }

            var currentState = $state.current.name;

            var route1 = UtilityService.GetCurrentRoute($scope.tab.childTabs, "userTabs.courier-accounts.parcel-hub-account");
            var route2 = UtilityService.GetCurrentRoute($scope.tab.childTabs, "userTabs.courier-accounts.easypost-account");

            if (currentState === route1) {
                $scope.gridOptions.data = $scope.ParcelHubCourierAccountsList;
                if ($scope.ParcelHubCourierAccountsList.length === 0) {
                    $scope.ParcelHubflag = true;
                }
                else {

                    $scope.ParcelHubflag = false;
                }
            }
            if (currentState === route2) {
                $scope.gridOptions.data = $scope.EasyPostCourierAccountsList;
                if ($scope.EasyPostCourierAccountsList.length === 0) {
                    $scope.EasyPostflag = true;
                }
                else {
                    $scope.EasyPostflag = false;
                }
            }
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });

        });
    };

    $scope.ZoneCountryByOperationZone = function () {

        CourierAccountService.GetInfo($scope.CourierAccount.OperationZone.OperationZoneId).then(function (response) {

            $scope.LogisticCompanies = response.data.LogisticCompanies;
            if ($scope.LogisticCompanies) {
                $scope.CourierAccount.LogisticService.LogisticCompany = $scope.LogisticCompanies[0];

            }
            $scope.LogisticRateTypes = response.data.LogisticRateTypes;
            if ($scope.LogisticRateTypes) {
                $scope.CourierAccount.LogisticService.LogisticRateType = $scope.LogisticRateTypes[0];

            }
            $scope.LogisticTypes = response.data.LogisticTypes;
            if (response.data.LogisticTypes.length === 5 && $state.is('admin.courier-accounts.easypost-account')) {
                $scope.LogisticTypes.splice(4, 1);

            }
            if ($scope.LogisticTypes) {
                $scope.CourierAccount.LogisticService.LogisticType = $scope.LogisticTypes[0];


            }
            $scope.ModuleType = response.data.ModuleType;
        });
    };

    $scope.activeRate = function (tab) {

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
            return tab;
        }
    };

    function init() {

        var userInfo = SessionService.getUser();
        $rootScope.GetServiceValue = '';
        var obj = SessionService.getUser();
        $scope.tabs = obj.tabs;
        $scope.tab = getUserTab($scope.tabs, "Couriers_Account");
        $scope.userInfo = obj;
        if ($scope.userInfo === undefined || $scope.userInfo === null || $scope.userInfo.SessionId === undefined || $scope.userInfo.SessionId === '') {
            $state.go('home.welcome');
        }

        $scope.OperationZone = { OperationZoneId: userInfo.OperationZoneId, OperationZoneName: "" };

        $scope.EasyPostCourierAccountsList = [];
        $scope.ParcelHubCourierAccountsList = [];
        $scope.gridheight = SessionService.getScreenHeight();
        $scope.SetGridOptions();
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';

        $scope.CourierAccount.OperationZone = $scope.OperationZone;
    
        $scope.GetInfo();

        $scope.gridOptions.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
        };
        setModalOptions();
    }

    init();

});