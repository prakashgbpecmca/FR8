
angular.module('ngApp.carrier').controller('CarrierController', function ($scope, $state, $location, $filter, $translate, CarrierService, SessionService, $uibModal, $log, uiGridConstants, toaster, ModalService) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['Carrier', 'DeleteHeader', 'The', 'DeleteBody', 'Detail', 'FrayteError', 'FrayteInformation', 'SuccessfullyDelete', 'information', 'ErrorDeletingRecord', 'ErrorGetting']).then(function (translations) {
            $scope.headerTextCarrier = translations.Carrier + " " + translations.DeleteHeader;
            $scope.bodyTextCarrier = translations.DeleteBody + " " + translations.The + " " + translations.Carrier;

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TextSuccessfullyDelete = translations.SuccessfullyDelete + " " + translations.The + " " + translations.Carrier + " " + translations.information;
            $scope.TextErrorDeletingRecord = translations.ErrorDeletingRecord;
            $scope.TextErrorGettingCarrierRecord = translations.ErrorGetting + " " + translations.Carrier + " " + translations.records;

        });
    };

    $scope.AddEditCarrier = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'carrier/carrierAddEdit.tpl.html',
            controller: 'CarrierAddEditController',
            windowClass: 'AddEditCarrier-Modal',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                mode: function () {
                    if (row === undefined) {
                        return 'Add';
                    }
                    else {
                        return 'Modify';
                    }

                },
                carriers: function () {
                    return $scope.carriers;
                },
                carrier: function () {
                    if (row === undefined) {
                        return {
                            CarrierId: 0,
                            CarrierName: '',
                            Code: '',
                            Prefix: '',
                            CarrierType: ''
                        };
                    }
                    else {
                        return row.entity;
                    }
                }
            }
        });

        modalInstance.result.then(function (carriers) {

            $scope.carriers = carriers;
        }, function () {
        });
    };

    $scope.RemoveCarrier = function (row) {
        var modalOptions = {
            headerText: $scope.headerTextCarrier,
            bodyText: $scope.bodyTextCarrier + " " + '(' + row.entity.CarrierName + ')' + '?'
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            CarrierService.DeleteCarrier(row.entity.CarrierId).then(function (response) {
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
            enableVerticalScrollbar: uiGridConstants.scrollbars.NEVER,
            columnDefs: [
              { name: 'CarrierName', displayName: 'CarrierName', headerCellFilter: 'translate' },
              { name: 'CarrierType', displayName: 'CarrierType', headerCellFilter: 'translate' },
              { name: 'Code', headerCellFilter: 'translate' },
              { name: 'Prefix', headerCellFilter: 'translate' },
              { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "carrier/carrierEditButton.tpl.html", width: 65 }
            ]
        };
    };

    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();

        $scope.gridheight = SessionService.getScreenHeight();
        $scope.SetGridOptions();
        CarrierService.GetCarrierList().then(function (response) {
            $scope.gridOptions.data = response.data;
            $scope.carriers = response.data;
            //For full List/Data in UI-GRID
            $scope.gridOptions.excessRows = $scope.gridOptions.data.length;
        }, function () {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGettingCarrierRecord,
                showCloseButton: true
            });
        });
    }

    init();

});