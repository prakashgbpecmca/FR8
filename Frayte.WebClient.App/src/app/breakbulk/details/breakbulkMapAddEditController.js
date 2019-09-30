angular.module('ngApp.breakBulk').controller("BreakbulkMapAddEditController", function ($scope, CustomerId, HubDetail, ExpressBookingService, SessionService, ModuleType, BreakBulkService, $uibModal, toaster, $translate, $uibModalInstance, AppSpinner) {
    //breakbulk product catalog code
    $scope.productCatalogs = function (ProductcatalogId) {
        if ($scope.CustomerDetail !== null && $scope.CustomerDetail !== undefined && $scope.CustomerDetail !== '' && $scope.CustomerDetail.CustomerId > 0) {
            ModalInstance = $uibModal.open({
                Animation: true,
                controller: 'ProductCatalogAddEditController',
                templateUrl: 'breakbulk/productCatalog/productCatalogAddEdit.tpl.html',
                keyboard: true,
                windowClass: 'CustomerAddress-Edit',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    CustomerId: function () {
                        return $scope.CustomerDetail.CustomerId;
                    },
                    HubDetail: function () {
                        return $scope.Hubs;
                    },
                    ProductcatalogId: function () {
                        return ProductcatalogId;
                    }
                }
            });
            ModalInstance.result.then(function (response) {
                if (response !== undefined && response !== null && response !== '') {
                    getHubs();
                    $scope.track.CustomerId = $scope.CustomerDetail.CustomerId;                    
                    $scope.getCatalogDetail($scope.track);
                }
            }, function () {
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.SelectCustomerValidation,
                showCloseButton: true
            });
        }
    };
    //end

    $scope.setProductDescription = function (breakbulkMap) {
        if (breakbulkMap !== undefined && breakbulkMap !== null && breakbulkMap !== '') {
            $uibModalInstance.close(breakbulkMap);
        }
    };

    //Set Multilingual for Modal Popup
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteWarning', 'FrayteSuccess', 'Confirmation', 'SomeErrorOccuredTryAgain', 'Product_Catalog_Not_Available',
                    'CorrectValidationErrorFirst', 'Please_Select_Hub', 'Product_Catalog_Save_Success',
                    'Enter_Product_Description', 'Loading_Product_Catalog']).then(function (translations) {
                        $scope.FrayteError = translations.FrayteError;
                        $scope.FrayteWarning = translations.FrayteWarning;
                        $scope.FrayteSuccess = translations.FrayteSuccess;
                        $scope.Confirmation = translations.Confirmation;
                        $scope.SomeErrorOccuredTryAgain = translations.SomeErrorOccuredTryAgain;
                        $scope.CorrectValidationErrorFirst = translations.CorrectValidationErrorFirst;
                        $scope.Product_Catalog_Not_Available = translations.Product_Catalog_Not_Available;
                        $scope.SelectHub = translations.Please_Select_Hub;
                        $scope.ProductCatalog_SaveSuccess = translations.Product_Catalog_Save_Success;
                        $scope.Enter_Product_Description = translations.Enter_Product_Description;
                        $scope.Loading_Product_Catalog = translations.Loading_Product_Catalog;
                    });
    };

    var getCustomers = function () {
        ExpressBookingService.GetCustomers($scope.userInfo.EmployeeId, "ExpressBooking").then(function (response) {
            $scope.customers = response.data;

            var dbCustomers = [];
            for (i = 0; i < $scope.customers.length; i++) {

                if ($scope.customers[i].CustomerId) {

                    $scope.customers[i].CustomerAccountNumberR = $scope.customers[i].AccountNumber;

                    var dbr = $scope.customers[i].AccountNumber.split("");
                    var accno = "";
                    for (var j = 0; j < dbr.length; j++) {
                        accno = accno + dbr[j];
                        if (j == 2 || j == 5) {
                            accno = accno + "-";
                        }
                    }
                    $scope.customers[i].AccountNumber = accno;
                }
            }

            for (k = 0; k < response.data.length; k++) {
                if (response.data[k].CustomerId === CustomerId) {
                    $scope.CustomerDetail = response.data[k];
                }
            }

            if ($scope.userInfo.RoleId === 5) {
            }

        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.SomeErrorOccuredTryAgain,
                showCloseButton: true
            });
        });
    };

    var getHubs = function () {
        BreakBulkService.Gethubs().then(function (response) {
            if (response.data !== undefined && response.data !== null && response.data !== '') {
                $scope.hubs = response.data;

                for (i = 0; i < $scope.hubs.length; i++) {
                    if ($scope.hubs[i].HubId === $scope.Hubs.HubId) {
                        $scope.Hub = $scope.hubs[i];
                        $scope.track.HubId = $scope.hubs[i].HubId;
                        $scope.getCatalogDetail($scope.track);
                    }
                }
            }
        });
    };

    $scope.setCustomerInfo = function (CustomerDetail) {
        if (CustomerDetail !== undefined && CustomerDetail !== null && CustomerDetail !== '' && CustomerDetail.CustomerId > 0) {
            $scope.track.CustomerId = CustomerDetail.CustomerId;
            $scope.getCatalogDetail($scope.track);
        }
    };

    $scope.getCatalogDetail = function (track) {
        AppSpinner.showSpinnerTemplate($scope.Loading_Product_Catalog, $scope.Template);
        if ($scope.Hubs !== null && $scope.Hubs !== undefined && $scope.Hubs !== '') {
            if ($scope.Hubs.HubCode === 'LHR') {
                BreakBulkService.getUKProductCatalog(track).then(function (response) {
                    if (response.data !== null && response.data !== undefined && response.data !== '' && response.data.length) {
                        $scope.breakbulkMaps = response.data;
                        AppSpinner.hideSpinnerTemplate();
                    }
                    else {
                        $scope.breakbulkMaps = [];                        
                        AppSpinner.hideSpinnerTemplate();
                    }
                });
            }
            if ($scope.Hubs.HubCode === 'ZRH') {
                BreakBulkService.getSwissProductCatalog(track).then(function (response) {
                    if (response.data !== null && response.data !== undefined && response.data !== '' && response.data.length) {
                        $scope.breakbulkMaps = response.data;
                        AppSpinner.hideSpinnerTemplate();
                    }
                    else {
                        $scope.breakbulkMaps = [];
                        AppSpinner.hideSpinnerTemplate();
                    }
                });
            }
            if ($scope.Hubs.HubCode === 'JFK' || $scope.Hubs.HubCode === 'ORD' || $scope.Hubs.HubCode === 'SFO') {
                BreakBulkService.getUSAProductCatalog(track).then(function (response) {
                    if (response.data !== null && response.data !== undefined && response.data !== '' && response.data.length) {
                        $scope.breakbulkMaps = response.data;
                        AppSpinner.hideSpinnerTemplate();
                    }
                    else {
                        $scope.breakbulkMaps = [];
                        AppSpinner.hideSpinnerTemplate();
                    }
                });
            }
            if ($scope.Hubs.HubCode === 'YVR' || $scope.Hubs.HubCode === 'YYZ') {
                BreakBulkService.getCanadaProductCatalog(track).then(function (response) {
                    if (response.data !== null && response.data !== undefined && response.data !== '' && response.data.length) {
                        $scope.breakbulkMaps = response.data;
                        AppSpinner.hideSpinnerTemplate();
                    }
                    else {
                        $scope.breakbulkMaps = [];
                        AppSpinner.hideSpinnerTemplate();
                    }
                });
            }
            if ($scope.Hubs.HubCode === 'NRT') {
                BreakBulkService.getJapanProductCatalog(track).then(function (response) {
                    if (response.data !== null && response.data !== undefined && response.data !== '' && response.data.length) {
                        $scope.breakbulkMaps = response.data;
                        AppSpinner.hideSpinnerTemplate();
                    }
                    else {
                        $scope.breakbulkMaps = [];
                        AppSpinner.hideSpinnerTemplate();
                    }
                });
            }
            if ($scope.Hubs.HubCode === 'OSL') {
                BreakBulkService.getNorwayProductCatalog(track).then(function (response) {
                    if (response.data !== null && response.data !== undefined && response.data !== '' && response.data.length) {
                        $scope.breakbulkMaps = response.data;
                        AppSpinner.hideSpinnerTemplate();
                    }
                    else {
                        $scope.breakbulkMaps = [];
                        AppSpinner.hideSpinnerTemplate();
                    }
                });
            }
            if ($scope.Hubs.HubCode === 'SIN') {
                BreakBulkService.getSingaporeProductCatalog(track).then(function (response) {
                    if (response.data !== null && response.data !== undefined && response.data !== '' && response.data.length) {
                        $scope.breakbulkMaps = response.data;
                        AppSpinner.hideSpinnerTemplate();
                    }
                    else {
                        $scope.breakbulkMaps = [];
                        AppSpinner.hideSpinnerTemplate();
                    }
                });
            }
        }
    };

    $scope.pageChanged = function (track) {
        $scope.getCatalogDetail(track);
    };

    function init() {

        $scope.module = ModuleType;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';

        // Pagination Logic 
        $scope.viewby = 50;
        $scope.maxSize = 2;
        $scope.currentPage = 1;
        $scope.itemsPerPage = $scope.viewby;

        $scope.track = {
            CustomerId: 0,
            HubId: 0,
            ProductDescription: '',
            TakeRows: $scope.itemsPerPage,
            CurrentPage: $scope.currentPage
        };

        $scope.pageChangedObj = [
            {
                SpecialSearchId: 1,
                pageChangedValue: 20,
                pageChangedValueDisplay: 20
            },
            {
                 SpecialSearchId: 2,
                 pageChangedValue: 50,
                 pageChangedValueDisplay: 50
            },
            {
                 SpecialSearchId: 3,
                 pageChangedValue: 100,
                 pageChangedValueDisplay: 100
            },
            {
                 SpecialSearchId: 4,
                 pageChangedValue: 200,
                 pageChangedValueDisplay: 200
            }];

        var userInfo = SessionService.getUser();
        $scope.userInfo = userInfo;
        if ($scope.userInfo.RoleId === 1 || $scope.userInfo.RoleId === 6) {
            $scope.RoleId = $scope.userInfo.RoleId;
            $scope.loggedInUserId = $scope.userInfo.EmployeeId;
            $scope.CustomerId = CustomerId;
            $scope.track.CustomerId = $scope.CustomerId;
            $scope.Hubs = HubDetail;
        }
        if ($scope.userInfo.RoleId === 3) {
            $scope.RoleId = $scope.userInfo.RoleId;
            $scope.loggedInUserId = $scope.userInfo.EmployeeId;
            $scope.CustomerId = $scope.userInfo.EmployeeId;
            $scope.track.CustomerId = $scope.CustomerId;
            $scope.Hubs = HubDetail;
        }

        setMultilingualOptions();
        getCustomers();
        getHubs();
    }

    init();

});