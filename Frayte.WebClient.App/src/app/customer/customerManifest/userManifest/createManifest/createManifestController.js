angular.module('ngApp.customer')
.controller('CreateManifestController', function ($scope, CustomerService, SessionService, $uibModal, TrackObj, uiGridConstants, $templateCache, $log, $timeout, toaster, $uibModalStack, AppSpinner, $rootScope, $translate, DirectBookingService, DateFormatChange) {
    $scope.title = 'CreateManifest';
    //trackingTemplate = '<div class="paddingTB5 word-wrap"><span ng-show="grid.appScope.CustomerId > 0" class="pointer"><a ng-click="grid.appScope.DirectShipmentDetail(row)">{{row.entity.FrayteNumber}}</a></span><span ng-show="grid.appScope.CustomerId === 0">{{row.entity.FrayteNumber}}</span></div>';
    //detailTemplate = '<div class="paddingTB5 word-wrap"><span class="pointer"><a ng-click="grid.appScope.DirectShipmentDetail(row)">{{row.entity.ShippingDate}}</a></span></div>';
    var setModalOptions = function () {
        $translate(['Frayte_Error', 'FrayteWarning', 'FrayteSuccess', 'SelectAtleast_OneManifest', 'SelectAtleast_OneCustomer', 'ErrorWhileCreating_Manifest', 'Error', 'SUCCESS', 'Manifest_Created_Successfully',
        'CreatingManifest', 'LoadingNonManifestShipments']).then(function (translations) {
            $scope.FrayteError = translations.Frayte_Error;
            $scope.Frayte_Warning = translations.FrayteWarning;
            $scope.Frayte_Success = translations.FrayteSuccess;
            $scope.SelectAtleastOneManifest = translations.SelectAtleast_OneManifest;
            $scope.SelectAtleastOneCustomer = translations.SelectAtleast_OneCustomer;
            $scope.ErrorWhileCreatingManifest = translations.ErrorWhileCreating_Manifest;
            $scope.Error = translations.Error;
            $scope.SUCCESS = translations.SUCCESS;
            $scope.ManifestCreatedSuccessfully = translations.Manifest_Created_Successfully;
            $scope.CreatingManifest = translations.CreatingManifest;
            $scope.LoadingNonManifestShipments = translations.LoadingNonManifestShipments;
            // GetInitial Call
            $scope.GetInitial();
        });

    };

    $scope.DirectShipmentDetail = function (row) {

        if ($scope.FrayteManifestShipment.ModuleType === "DirectBooking") {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'directBooking/directBookingDetail/directBookingDetail.tpl.html',
                controller: 'DirectBookingDetailController',
                windowClass: 'DirectBookingDetail',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    shipmentId: function () {
                        return row.entity.ShipmentId;
                    },
                    ShipmentStatus: function () {
                        return row.entity.Status;
                    }
                }
            });
        }
        else {
            var modalInstance1 = $uibModal.open({
                animation: true,
                templateUrl: 'eCommerceBooking/eCommerceBookingDetail/eCommerceBookingDetail.tpl.html',
                controller: 'eCommerceBookingDetailController',
                windowClass: 'DirectBookingDetail',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    shipmentId: function () {
                        return row.entity.ShipmentId;
                    },
                    IsTrackingShow: function () {
                        return row.entity.IsTrackingShow;
                    },
                    BookingApp: function () {
                        return row.entity.BookingApp;
                    },
                    ShipmentStatus: function () {
                        return row.entity.Status;
                    }
                }
            });
        }
    };

    $scope.TrackingPage = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'home/trackingnew.tpl.html',
            controller: 'HomeTrackingController',
            windowClass: '',
            size: 'lg',
            resolve: {
                ShipmentData: function () {
                    return;
                },
                ShipmentData1: function () {
                    return;
                },
                ShipmentData2: function () {
                    return row;
                }

            }
        });
        modalInstance.result.then(function () {

        }, function () {
        });
    };
    trackingTemplate = '<div class="paddingTB5 word-wrap"> <span class="pointer"><a ng-click="grid.appScope.TrackingPage(row.entity)">{{row.entity.TrackingNo}}</a></span></div>';
    var createManifestAfterETAETD = function () {
        if ($scope.FrayteManifestShipment.UserId && $scope.FrayteManifestShipment.CreatedBy) {
            CustomerService.GetManifestedShipments($scope.FrayteManifestShipment).then(function (response) {
                AppSpinner.hideSpinnerTemplate();
                $scope.result = response.data;
                if (response.status === 200) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.Frayte_Success,
                        body: $scope.ManifestCreatedSuccessfully,
                        showCloseButton: true
                    });
                    $uibModalStack.dismissAll('closing');
                    $rootScope.GetManifest();
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteError,
                        body: $scope.ErrorWhileCreatingManifest,
                        showCloseButton: true
                    });
                    AppSpinner.hideSpinnerTemplate();
                }

            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.ErrorWhileCreatingManifest,
                    showCloseButton: true
                });
                AppSpinner.hideSpinnerTemplate();
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.SelectAtleastOneCustomer,
                showCloseButton: true
            });
            AppSpinner.hideSpinnerTemplate();
        }
    };

    $scope.CreateManifest = function () {
        if ($scope.FrayteManifestShipment.DirectShipments.length !== 0) {
            if ($scope.FrayteManifestShipment.ModuleType === "eCommerce") {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: "customer/customerManifest/userManifest/createManifest/manifestETAETD.tpl.html",
                    controller: "CreateManifestETAETDController",
                    windowClass: "DirectBookingDetail",
                    size: "lg",
                    backdrop: "static",
                    resolve: {
                    }
                });

                modalInstance.result.then(function (data) {
                    data.CustomerId = $scope.UserId;
                    data.Shipments = $scope.FrayteManifestShipment.DirectShipments;
                    AppSpinner.showSpinnerTemplate($scope.CreatingManifest, $scope.Template);
                    console.log("", data.EstimatedDateofDelivery.toJSON());
                    CustomerService.SaveETAETD(data).then(function (response) {
                        if (response.data && response.data.Status) {
                            createManifestAfterETAETD();
                        }
                        else {
                            toaster.pop({
                                type: 'error',
                                title: $scope.FrayteError,
                                body: $scope.ErrorWhileCreatingManifest,
                                showCloseButton: true
                            });
                            AppSpinner.hideSpinnerTemplate();
                        }
                    }, function () {
                        toaster.pop({
                            type: 'error',
                            title: $scope.FrayteError,
                            body: $scope.ErrorWhileCreatingManifest,
                            showCloseButton: true
                        });
                        AppSpinner.hideSpinnerTemplate();
                    });

                }, function () {

                    // did not save ETA ETD
                });
            }
            else {
                createManifestAfterETAETD();
            }
        }
        else {
            AppSpinner.hideSpinner();
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.SelectAtleastOneManifest,
                showCloseButton: true
            });
        }
    };

    var statusTypeTemplate = '<div class="ui-grid-cell-contents">{{row.entity.DisplayName}} {{row.entity.RateTypeDisplay}}</div>';
    var customerTypeTemplate = '<div class="ui-grid-cell-contents">{{ "Customer" | translate}} <span class="redColor">*</span></div>';

    $scope.setGirdOptions = function () {
        $scope.gridOptions = {
            showFooter: true,
            enableSorting: true,
            multiSelect: true,
            enableFiltering: true,
            enableRowSelection: true,
            enableSelectAll: true,
            enableRowHeaderSelection: true,
            selectionRowHeaderWidth: 34,
            enableGridMenu: false,
            enableColumnMenus: false,
            enableVerticalScrollbar: true,
            enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
            columnDefs: [
                      { name: 'Customer', displayName: 'Customer', CellTemplate: customerTypeTemplate, headerCellFilter: 'translate', width: '15%' },
                      { name: 'DisplayName', displayName: 'Courier', cellTemplate: statusTypeTemplate, headerCellFilter: 'translate', width: '15%' },
                      { name: 'TrackingNo', displayName: 'Tracking_No', cellTemplate: trackingTemplate, headerCellFilter: 'translate', width: '15%' },
                      { name: 'ShippingDate', displayName: 'Created_On', headerCellFilter: 'translate', width: '11%' },
                      { name: 'TotalPieces', displayName: 'Total_Pieces', headerCellFilter: 'translate', width: '10%' },
                      { name: 'TotalWeight', displayName: 'Total_Weight_kgs', headerCellFilter: 'translate', width: '13%' },
                      { name: 'ShippedToCompany', displayName: 'To_Consignee', headerCellFilter: 'translate', width: '21%' }
            ]
        };
    };

    $scope.selectButtonClick = function (row, $event) {
        row.isSelected = !row.isSelected;
    };

    $scope.rowSelection = function () {
        $scope.gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            $scope.createval = true;
            if (row.isSelected === true) {
                $scope.FrayteManifestShipment.DirectShipments.push(row.entity);
            }
            else {

                for (i = 0; i < $scope.FrayteManifestShipment.DirectShipments.length; i++) {
                    if (row.entity.ShipmentId === $scope.FrayteManifestShipment.DirectShipments[i].ShipmentId) {
                        $scope.FrayteManifestShipment.DirectShipments.splice(i, 1);
                    }
                }
            }
        });

        $scope.createval = false;

        // Multiple row selections
        $scope.gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {
            $scope.createval = true;
            if (rows[0].isSelected === true) {
                for (i = 0; i < rows.length; i++) {
                    $scope.FrayteManifestShipment.DirectShipments.push(rows[i].entity);
                }
            }
            else {
                for (i = 0; i < $scope.FrayteManifestShipment.DirectShipments.length; i++) {
                    $scope.FrayteManifestShipment.DirectShipments = [];
                }
            }

        });
    };

    $scope.toggleRowSelection = function () {
        $scope.gridApi.selection.clearSelectedRows();
        $scope.gridOptions.enableRowSelection = !$scope.gridOptions.enableRowSelection;
        $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS);
    };

    $scope.seachManifestByModuleType = function () {
        if ($scope.FrayteManifestShipment.ModuleType === 'eCommerce') {
            $scope.FrayteManifestShipment.SubModuleType = "ECOMMERCE_ONL";
        }
        $scope.seachManifest();
    };

    $scope.seachManifest = function () {
        if ($scope.FrayteManifestShipment.ModuleType !== 'eCommerce') {
            $scope.FrayteManifestShipment.SubModuleType = "";
        }
        CustomerService.GetNonManifestedShipments($scope.OperationZoneId, $scope.UserId, $scope.TrackManifest.CreatedBy, $scope.FrayteManifestShipment.ModuleType, $scope.FrayteManifestShipment.SubModuleType).then(function (response) {

            for (j = 0; j < response.data.Shipments.length; j++) {
                if (response.data.Shipments !== null) {
                    response.data.Shipments[j].ShippingDate = DateFormatChange.DateFormatChange(response.data.Shipments[j].ShippingDate);
                }
            }
            $scope.gridOptions.data = response.data.Shipments;

            if ($scope.UserId === 0) {
                $scope.ManifestCustomers = [];
            }
            for (var i = 0; i < response.data.Customers.length; i++) {
                $scope.Customer = {
                    CustomerId: "",
                    CompanyName: ""
                };
                $scope.Customer.CustomerId = response.data.Customers[i].UserId;
                if (response.data.Customers[i].AccountNumber !== undefined && response.data.Customers[i].AccountNumber !== null && response.data.Customers[i].AccountNumber !== "")
                {
                    $scope.Customer.CompanyName = response.data.Customers[i].CompanyName + ' - ' + $scope.accBreak(response.data.Customers[i].AccountNumber.length, response.data.Customers[i].AccountNumber);
                }
                else {
                    $scope.Customer.CompanyName = response.data.Customers[i].CompanyName;
                }
                if ($scope.UserId === 0) {
                    $scope.ManifestCustomers.push($scope.Customer);
                }
            }
            if ($scope.UserId === 0) {
                $scope.ManifestCustomers = angular.copy($scope.ManifestCustomers);
                $scope.ManifestCustomers.unshift({ CustomerId: 0, CompanyName: 'All' });
            }
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
        });
    };

    $scope.getCustomerDetail = function (CustomerDetail) {
        if (CustomerDetail !== undefined && CustomerDetail !== null && CustomerDetail !== '') {
            $scope.CustomerId = CustomerDetail.CustomerId;
            $scope.TrackManifest.UserId = CustomerDetail.CustomerId;
            $scope.FrayteManifestShipment.UserId = CustomerDetail.CustomerId;
            $scope.UserId = CustomerDetail.CustomerId;
            $rootScope.CustomerManId = CustomerDetail.CustomerId;
            $scope.seachManifest();
        }
    };

    $scope.accBreak = function (fiterCountryLength, AccountNo) {
        var AccNo = AccountNo.split('');
        var AccNonew = [];
        AccNonew2 = "";
        if (fiterCountryLength <= 3) {
            for (j = 0; j < fiterCountryLength; j++) {
                if (j === 0) {
                    AccNonew.push('a');
                }
                AccNonew.push(AccNo[j]);
            }
            AccNonew.splice(0, 1);
            for (jj = 0; jj < AccNonew.length; jj++) {
                AccNonew2 = AccNonew2 + AccNonew[jj].toString();
            }
        }
        else if (fiterCountryLength >= 4 && fiterCountryLength <= 8) {

            for (j = 0; j < fiterCountryLength; j++) {

                if (j === 0) {
                    AccNonew.push('a');
                }

                AccNonew.push(AccNo[j]);
            }
            AccNonew.splice(0, 1);
            for (jj = 0; jj < AccNonew.length; jj++) {
                AccNonew2 = AccNonew2 + AccNonew[jj].toString();
            }
        }
        else if (fiterCountryLength > 8) {

            for (j = 0; j < fiterCountryLength; j++) {

                if (j === 0) {
                    AccNonew.push('a');
                }
                AccNonew.push(AccNo[j]);
            }
            AccNonew.splice(0, 1);
            for (jj = 0; jj < AccNonew.length; jj++) {
                if (jj === 3) {
                    AccNonew2 = AccNonew2 + '-' + AccNonew[jj].toString();
                }
                else if (jj === 6) {
                    AccNonew2 = AccNonew2 + '-' + AccNonew[jj].toString();
                }
                else {
                    AccNonew2 = AccNonew2 + AccNonew[jj].toString();
                }
            }
        }
        return AccNonew2;
    };

    $scope.GetInitial = function () {
        if ($scope.RoleId === 3 || $scope.RoleId === 17) {
            CustomerService.GetCustomerDetail($scope.UserId).then(function (response) {
                $scope.CustomerDetail = response.data;
                if ($scope.CustomerDetail) {
                    if ($scope.CustomerDetail.IsWithoutService === false) {
                        $scope.eCommerceShipmentTypes.splice(0, 1);
                    }
                    else if ($scope.CustomerDetail.IsServiceSelected === false) {
                        $scope.eCommerceShipmentTypes.splice(1, 1);
                    }
                    else {

                    }
                    $scope.seachManifest();
                }
            }, function () {
                AppSpinner.hideSpinnerTemplate();
            });
        }

        AppSpinner.showSpinnerTemplate($scope.LoadingNonManifestShipments, $scope.Template);
        if ($scope.TrackManifest.UserId === undefined || $scope.TrackManifest.UserId === '' || $scope.TrackManifest.UserId === null || $scope.TrackManifest.UserId === 0) {
            if ($scope.RoleId === 6 || $scope.RoleId === 1) {
                //CustomerService.GetManifestCustomerList().then(function (response) {
                DirectBookingService.GetDirectBookingCustomers($scope.EmployeeId, "DirectBooking").then(function (response) {
                    if (response !== null && response.data.length > 0) {
                        $scope.ManifestCustomers = [];
                        for (var i = 0; i < response.data.length; i++) {
                            $scope.Customer = {
                                CustomerId: "",
                                CompanyName: ""
                            };
                            $scope.Customer.CustomerId = response.data[i].CustomerId;
                            $scope.Customer.CompanyName = response.data[i].CompanyName + ' - ' + accBreak(response.data[i].AccountNumber.length, response.data[i].AccountNumber);
                            $scope.ManifestCustomers.push($scope.Customer);
                        }
                        $scope.ManifestCustomers.unshift({ CustomerId: 0, CompanyName: 'All' });
                        $scope.CustomerDetail = $scope.ManifestCustomers[0];
                        $scope.CustomerId = $scope.ManifestCustomers[0].CustomerId;
                        $scope.TrackManifest.UserId = $scope.ManifestCustomers[0].CustomerId;
                        $scope.UserId = $scope.TrackManifest.UserId;
                        $scope.seachManifest();
                        $rootScope.isShow = true;
                    }
                }, function () {
                    AppSpinner.hideSpinnerTemplate();
                });
            }
        }
        else {
            if ($scope.RoleId === 6 || $scope.RoleId === 1) {
                //CustomerService.GetManifestCustomerList().then(function (response) {
                DirectBookingService.GetDirectBookingCustomers($scope.EmployeeId, "DirectBooking").then(function (response) {
                    if (response !== null && response.data.length > 0) {
                        $scope.ManifestCustomers = [];
                        for (var i = 0; i < response.data.length; i++) {
                            $scope.Customer = {
                                CustomerId: "",
                                CompanyName: ""
                            };
                            $scope.Customer.CustomerId = response.data[i].CustomerId;
                            $scope.Customer.CompanyName = response.data[i].CompanyName + ' - ' + accBreak(response.data[i].AccountNumber.length, response.data[i].AccountNumber);
                            $scope.ManifestCustomers.push($scope.Customer);
                        }
                        $scope.ManifestCustomers.unshift({ CustomerId: 0, CompanyName: 'All' });

                        for (var j = 0; j < $scope.ManifestCustomers.length; j++) {
                            if ($scope.TrackManifest.UserId === $scope.ManifestCustomers[j].CustomerId) {
                                $scope.CustomerDetail = $scope.ManifestCustomers[j];
                                $scope.CustomerId = $scope.ManifestCustomers[j].CustomerId;
                                $scope.UserId = $scope.TrackManifest.UserId;
                                $scope.FrayteManifestShipment.UserId = $scope.TrackManifest.UserId;
                                $scope.seachManifest();
                                $rootScope.isShow = true;
                            }
                        }
                    }
                });
            }
        }

        var accBreak = function (fiterCountryLength, AccountNo) {
            var AccNo = AccountNo.split('');
            var AccNonew = [];
            AccNonew2 = "";
            if (fiterCountryLength <= 3) {
                for (j = 0; j < fiterCountryLength; j++) {
                    if (j === 0) {
                        AccNonew.push('a');
                    }
                    AccNonew.push(AccNo[j]);
                }
                AccNonew.splice(0, 1);
                for (jj = 0; jj < AccNonew.length; jj++) {
                    AccNonew2 = AccNonew2 + AccNonew[jj].toString();
                }
            }
            else if (fiterCountryLength >= 4 && fiterCountryLength <= 8) {

                for (j = 0; j < fiterCountryLength; j++) {

                    if (j === 0) {
                        AccNonew.push('a');
                    }

                    AccNonew.push(AccNo[j]);
                }
                AccNonew.splice(0, 1);
                for (jj = 0; jj < AccNonew.length; jj++) {
                    AccNonew2 = AccNonew2 + AccNonew[jj].toString();
                }
            }
            else if (fiterCountryLength > 8) {

                for (j = 0; j < fiterCountryLength; j++) {

                    if (j === 0) {
                        AccNonew.push('a');
                    }
                    AccNonew.push(AccNo[j]);
                }
                AccNonew.splice(0, 1);
                for (jj = 0; jj < AccNonew.length; jj++) {
                    if (jj === 3) {
                        AccNonew2 = AccNonew2 + '-' + AccNonew[jj].toString();
                    }
                    else if (jj === 6) {
                        AccNonew2 = AccNonew2 + '-' + AccNonew[jj].toString();
                    }
                    else {
                        AccNonew2 = AccNonew2 + AccNonew[jj].toString();
                    }
                }
            }
            return AccNonew2;
        };


        AppSpinner.showSpinnerTemplate($scope.LoadingNonManifestShipments, $scope.Template);

        if ($scope.UserId !== undefined && $scope.UserId !== null && $scope.UserId !== '' && $scope.UserId > 0) {
            CustomerService.GetBookingTypes($scope.UserId).then(function (response) {
                $scope.shipmentTypes = response.data;
                if (response.data && response.data.length === 1) {
                    $scope.IsDisable = true;
                }
                else {
                    $scope.IsDisable = false;
                }
            }, function () {
                AppSpinner.hideSpinnerTemplate();
            });
        }
        else {
            $scope.bookship = [];
            $scope.bookingshipment = {
                BookingType: "",
                BookingTypeDisplay: ""
            };

            $scope.bookingshipment.BookingType = 'DirectBooking';
            $scope.bookingshipment.BookingTypeDisplay = 'Direct Booking';
            $scope.bookship.push($scope.bookingshipment);
            $scope.shipmentTypes = $scope.bookship;
            $scope.ModuleType = $scope.shipmentTypes[0].BookingTypeDisplay;
            $scope.IsDisable = true;
        }
    };

    var init = function () {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.eCommerceShipmentTypes = [{
            value: '',
            display: 'All'
        }, {
            value: 'ECOMMERCE_WS',
            display: 'Without Service_CSV'
        }, {
            value: 'ECOMMERCE_SS',
            display: 'Service Select_CSV'
        },
        {
            value: 'ECOMMERCE_ONL',
            display: 'Manual Booking'
        }
        ];

        $scope.TrackManifest = TrackObj;
        $scope.subModuleType = $scope.TrackManifest.subModuleType;

        $scope.FrayteManifestShipment = {
            ModuleType: $scope.TrackManifest.ModuleType,
            SubModuleType: $scope.TrackManifest.subModuleType,
            UserId: null,
            CreatedBy: null,
            DirectShipments: []
        };

        var userInfo = SessionService.getUser();
        $scope.RoleId = userInfo.RoleId;
        $scope.EmployeeId = userInfo.EmployeeId;
        $scope.OperationZoneId = userInfo.OperationZoneId;
        if ($scope.RoleId === 3 || $scope.RoleId === 17) {
            $scope.UserId = $scope.TrackManifest.UserId;
            $scope.FrayteManifestShipment.UserId = $scope.TrackManifest.UserId;
            $scope.FrayteManifestShipment.CreatedBy = $scope.TrackManifest.CreatedBy;
        }
        else {
            $scope.FrayteManifestShipment.CreatedBy = $scope.TrackManifest.CreatedBy;
        }
        setModalOptions();

        $scope.setGirdOptions();

        $scope.gridOptions.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
            $scope.rowSelection();
        };
    };

    init();
});