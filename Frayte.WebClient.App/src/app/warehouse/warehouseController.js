angular.module('ngApp.warehouse').controller('WarehouseController', function ($scope, $state, $location, $filter, $translate, SessionService, CountryService, TimeZoneService, $uibModal, WarehouseService, toaster, uiGridConstants, ModalService) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['Warehouse', 'DeleteHeader', 'DeleteBody', 'detail', 'FrayteError', 'ErrorGetting', 'Warehouse', 'FrayteInformation', 'SuccessfullyDelete', 'The', 'information', 'ErrorDeletingRecord']).then(function (translations) {
            $scope.headerTextWarehouse = translations.Warehouse + " " + translations.DeleteHeader;
            $scope.bodyTextWarehouse = translations.DeleteBody + " " + translations.Warehouse + " " + translations.detail;

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            
            $scope.TextSuccessfullyDelete = translations.SuccessfullyDelete + " " + translations.The + " " + translations.Warehouse + " " + translations.information;
            $scope.TextErrorDeletingRecord = translations.ErrorDeletingRecord;
            $scope.TextErrorGettingWarehouseDetail = translations.ErrorGetting + " " + translations.Warehouse + " " + translations.detail;
        });
    };

    $scope.AddEditWarehouse = function (row) {
        if (row === undefined) {
            $state.go('admin.warehouse-detail', { "warehouseId": 0 });
        }
        else {
            $state.go('admin.warehouse-detail', { "warehouseId": row.entity.WarehouseId });
        }
    };

    $scope.RemoveWarehouse = function (row) {
        var modalOptions = {
            headerText: $scope.headerTextWarehouse,
            bodyText: $scope.bodyTextWarehouse + '?'
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            WarehouseService.DeleteWarehouse(row.entity.WarehouseId).then(function (response) {
                if (response.data.Status) {
                    var index = $scope.gridOptions.data.indexOf(row.entity);
                    $scope.gridOptions.data.splice(index, 1);
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteInformation,
                        body: $scope.TextSuccessfullyDelete,
                        showCloseButton: true
                    });
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteError,
                        body: response.data.Errors[0],
                        showCloseButton: true
                    });
                }
            }, function () {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorDeletingRecord,
                    showCloseButton: true
                });
            });
        });
    };

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
        enableVerticalScrollbar: uiGridConstants.scrollbars.NEVER,
        columnDefs: [
          { name: 'LocationName', displayName: 'LocationName', headerCellFilter: 'translate' },
          { name: 'Country.Name', displayName: 'Country', headerCellFilter: 'translate' },
          { name: 'State', headerCellFilter: 'translate' },
          { name: 'Zip', displayName: 'PostalCode', headerCellFilter: 'translate' },
          { name: 'Timezone.Name', displayName: 'TimeZone', headerCellFilter: 'translate' },
          { name: 'Manager.WorkingHours', displayName: 'WorkingHrs', headerCellFilter: 'translate' },
          { name: 'Email', headerCellFilter: 'translate' },
          { name: 'Manager.ContactName', displayName: 'ManagerName', headerCellFilter: 'translate' },
          { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "warehouse/warehouseEditButton.tpl.html", width: 65 }
        ]
    };

    $scope.LoadWarehouses = function () {
        WarehouseService.GetWarehouseList().then(function (response) {
            $scope.gridOptions.data = response.data;
            //For full List/Data in UI-GRID
            $scope.gridOptions.excessRows = $scope.gridOptions.data.length;
        }, function () {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGettingWarehouseDetail,
                showCloseButton: true
            });
        });
    };

    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();

        $scope.gridheight = SessionService.getScreenHeight();
        $scope.LoadWarehouses();
    }

    init();

});