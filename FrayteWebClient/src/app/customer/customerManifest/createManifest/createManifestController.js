angular.module('ngApp.customer')
.controller('CreateManifestController', function ($scope, CustomerService, SessionService, TrackObj, uiGridConstants, $templateCache, $log, $timeout, toaster, $uibModalStack, AppSpinner, $rootScope, $translate) {
    $scope.title = 'CreateManifest';

    var setModalOptions = function () {
        $translate(['Frayte_Error', 'FrayteWarning', 'FrayteSuccess', 'SelectAtleast_OneManifest', 'ErrorWhileCreating_Manifest', 'Error', 'SUCCESS', 'Manifest_Created_Successfully']).then(function (translations) {


            $scope.FrayteError = translations.Frayte_Error;
            $scope.Frayte_Warning = translations.FrayteWarning;
            $scope.Frayte_Success = translations.FrayteSuccess;
            $scope.SelectAtleastOneManifest = translations.SelectAtleast_OneManifest;
            $scope.ErrorWhileCreatingManifest = translations.ErrorWhileCreating_Manifest;
            $scope.Error = translations.Error;
            $scope.SUCCESS = translations.SUCCESS;
            $scope.ManifestCreatedSuccessfully = translations.Manifest_Created_Successfully;
        });
    };

    //   $templateCache.put('ui-grid/selectionRowHeaderButtons',
    //  "<div class=\"ui-grid-selection-row-header-buttons \" ng-class=\"{'ui-grid-row-selected': row.isSelected}\" ><input style=\"margin: 0; vertical-align: middle\" type=\"checkbox\" ng-model=\"row.isSelected\" ng-click=\"row.isSelected=!row.isSelected;selectButtonClick(row, $event)\">&nbsp;</div>"
    //);


    //   $templateCache.put('ui-grid/selectionSelectAllButtons',
    //     "<div class=\"ui-grid-selection-row-header-buttons \" ng-class=\"{'ui-grid-all-selected': grid.selection.selectAll}\" ng-if=\"grid.options.enableSelectAll\"><input style=\"margin: 0; vertical-align: middle\" type=\"checkbox\" ng-model=\"grid.selection.selectAll\" ng-click=\"grid.selection.selectAll=!grid.selection.selectAll;headerButtonClick($event)\"></div>"
    //   );
    $scope.CreateManifest = function () {
        if ($scope.FrayteManifestShipment.DirectShipments.length !== 0) {
            AppSpinner.showSpinnerTemplate("Creating Manifest", $scope.Template);
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

    $scope.setGirdOptions = function () {
        $scope.gridOptions = {
            //    onRegisterApi: function (gridApi) {
            //        grid = gridApi;
            //    },
            showFooter: true,
            enableSorting: true,
            multiSelect: true,
            enableFiltering: true,
            enableRowSelection: true,
            enableSelectAll: true,
            enableRowHeaderSelection: true,
            selectionRowHeaderWidth: 35,
            enableGridMenu: true,
            enableVerticalScrollbar: true,
            enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
            //enableVerticalScrollbar: uiGridConstants.scrollbars.NEVER,

            //     rowTemplate: '<div ng-class="{ \'demoClass\':grid.appScope.rowFormatter( row ) }">' + '  <div ng-repeat="(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ng-class="{ \'ui-grid-row-header-cell\': col.isRowHeader }"  ui-grid-cell></div>' + '</div>',
            columnDefs: [
                //{ name: 'Items', width: '10%', enableFiltering: false, enableSelectAll: true, enableRowHeaderSelection: true, headerCellFilter: 'translate', cellTemplate: '<input type="checkbox" name="select_item" value="true" ng-model="row.entity.selected"/>' },
                  //{ name: 'Status', width: '12%', headerCellFilter: 'translate' },
                      { name: 'Customer', headerCellFilter: 'translate', width: '20%' },
                      { name: 'DisplayName', displayName: 'Courier', cellTemplate: statusTypeTemplate, headerCellFilter: 'translate', width: '20%' },
                      { name: 'TrackingNo', displayName: 'Tracking_No', headerCellFilter: 'translate', width: '20%' },
                      { name: 'FrayteNumber', displayName: 'Frayte_ShipmentNo', headerCellFilter: 'translate', width: '20%' },
                      { name: 'ShippedToCompany', displayName: 'To_Consignee', headerCellFilter: 'translate', width: '20%' }


                  //{ name: 'Edit', displayName: '', enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "customer/customerManifest/customerManifestEditButton.tpl.html", width:'8%' }

            ]
        };
    };
    $scope.selectButtonClick = function (row, $event) {
        row.isSelected = !row.isSelected;
    };
    $scope.rowSelection = function () {
        // $scope.rowArray = [];
        // Single row Selection
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

    $scope.seachManifest = function () {
        CustomerService.GetNonManifestedShipments($scope.OperationZoneId, $scope.UserId, $scope.TrackManifest.ModuleType).then(function (response) {
            $scope.gridOptions.data = response.data;

            AppSpinner.hideSpinnerTemplate();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
        });

    };
    var init = function () {
        //   $scope.spinnerMessage = 'Creating Manifest';
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';

        $scope.TrackManifest = TrackObj;

        $scope.FrayteManifestShipment = {
            ModuleType: $scope.TrackManifest.ModuleType,
            UserId: null,
            DirectShipments: []
        };

        $scope.setGirdOptions();


        $scope.gridOptions.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
            $scope.rowSelection();
        };
        var userInfo = SessionService.getUser();
        $scope.UserId = userInfo.EmployeeId;
        $scope.FrayteManifestShipment.UserId = userInfo.EmployeeId;
        $scope.OperationZoneId = userInfo.OperationZoneId;
        AppSpinner.showSpinnerTemplate("", $scope.Template);

        CustomerService.GetBookingTypes(userInfo.EmployeeId).then(function (response) {
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

        $scope.seachManifest();

        setModalOptions();

    };

    init();
});