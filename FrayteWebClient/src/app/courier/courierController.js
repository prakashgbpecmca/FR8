/** 
 * Controller
 */
angular.module('ngApp.courier').controller('CourierController', function ($scope, $state, $location, $filter, $translate, CourierService, SessionService, $uibModal, $log, uiGridConstants, toaster, ModalService) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['serviceoptn_ShippingMethod', 'DeleteHeader', 'This', 'DeleteBody', 'FrayteError', 'FrayteInformation', 'ErrorGetting', 'ErrorDeletingRecord', 'SuccessfullyDelete', 'information', 'courier', 'The']).then(function (translations) {
            $scope.headerTextShippingMethod = translations.serviceoptn_ShippingMethod + " " + translations.DeleteHeader;
            $scope.bodyTextShippingMethod = translations.DeleteBody + " " + translations.This + " " + translations.serviceoptn_ShippingMethod;

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;

            $scope.TextErrorGettingCourierRecord = translations.ErrorGetting + " " + translations.courier + " " + translations.records;
            $scope.TextErrorDeletingRecord = translations.ErrorDeletingRecord;
            $scope.TextSuccessfullyDeleteCourierInfo = translations.SuccessfullyDelete + " " + translations.The + " " + translations.courier + " " + translations.information;
        });
    };

    $scope.AddEditCourier = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'courier/courierAddEdit.tpl.html',
            controller: 'CourierAddEditController',
            windowClass: 'AddEditCourier-Modal',
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

                couriers: function () {
                    return $scope.couriers;
                },
                courier: function () {
                    if (row === undefined) {
                        return {
                            CourierId: 0,
                            Name: '',
                            Website: '',
                            CourierType: null,
                            LatestBookingTime: ''
                        };
                    }
                    else {
                        return row.entity;
                    }
                }
            }
        });

        modalInstance.result.then(function (couriers) {
            $scope.couriers = couriers;
        }, function () {
        });
    };

    $scope.DeleteCourier = function (row) {

        var modalOptions = {
            headerText: $scope.headerTextShippingMethod,
            bodyText: $scope.bodyTextShippingMethod + '?'
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            CourierService.DeleteCourier(row.entity.CourierId).then(function (response) {
                if (response.data.Status) {
                    var index = $scope.gridOptions.data.indexOf(row.entity);
                    $scope.gridOptions.data.splice(index, 1);
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteInformation,
                        body: $scope.TextSuccessfullyDeleteCourierInfo,
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
            enableGridMenu: true,
            enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
            enableVerticalScrollbar: true,
            columnDefs: [
              { name: 'CourierType', displayName: 'Type', headerCellFilter: 'translate', width:'23%' },
              { name: 'DisplayName', displayName: 'CourierName', headerCellFilter: 'translate', width: '22%' },
              { name: 'Website', headerCellFilter: 'translate', width: '23%' },
              { name: 'LatestBookingTime', displayName: 'LatestBookingTime', headerCellFilter: 'translate', enableFiltering: false, cellFilter: 'shortTimeFilter:this', width: '22%' },
              { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, cellTemplate: "courier/courierEditButton.tpl.html" }
            ]
        };
    };
    $scope.toggleRowSelection = function () {
        $scope.gridApi.selection.clearSelectedRows();
        $scope.gridOptions.enableRowSelection = !$scope.gridOptions.enableRowSelection;
        $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS);
    };
    function init() {

     

        // set Multilingual Modal Popup Options
        setModalOptions();
        $scope.gridheight = SessionService.getScreenHeight();
        $scope.SetGridOptions();

        CourierService.GetCourierList().then(function (response) {
            $scope.gridOptions.data = response.data;
            $scope.couriers = response.data;
            //For full List/Data in UI-GRID
            $scope.gridOptions.excessRows = $scope.gridOptions.data.length;
        }, function () {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGettingCourierRecord,
                showCloseButton: true
            });
        });
        $scope.gridOptions.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
        };
    }

    init();

})

;