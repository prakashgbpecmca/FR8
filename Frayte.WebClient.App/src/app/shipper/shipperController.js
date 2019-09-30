angular.module('ngApp.shipper').controller('ShipperController', function ($scope, $state,Upload,config, $location, $translate, $filter, ShipperService, SessionService, $uibModal, uiGridConstants, toaster, CountryService, ModalService) {

    // Uplaod shipper via excel
    $scope.WhileAddingReceiverExcel = function ($files, $file, $event) {
        
        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextPleaseSelectValidFile,
                showCloseButton: true
            });
            return;
        }

        // Upload the excel file here.
        $scope.uploadExcel = Upload.upload({

            url: config.SERVICE_URL + '/Shipper/UploadShippers',
            file: $file

        });

        $scope.uploadExcel.progress($scope.progressExcel);

        $scope.uploadExcel.success($scope.successExcel);

        $scope.uploadExcel.error($scope.errorExcel);
    };

    $scope.progressExcel = function (evt) {
        //console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
        //toaster.clear();
        //toaster.pop({
        //    type: 'success',
        //    title: 'uploading',
        //    body: 'percent: ' + parseInt(100.0 * evt.loaded / evt.total, 10),
        //    showCloseButton: true
        //});
    };

    $scope.successExcel = function (data, status, headers, config) {
        if (status = 200) {

            toaster.pop({
                type: 'success',
                title: $scope.TitleFrayteSuccess,
                body: $scope.TextUploadedSuccessfully,
                showCloseButton: true
            });
            $state.reload();
        }

    };

    $scope.errorExcel = function (err) {
        toaster.pop({
            type: 'error',
            title: $scope.TitleFrayteError,
            body: $scope.TextErrorOccuredDuringUpload,
            showCloseButton: true
        });
    };

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['Shipper', 'DeleteHeader', 'DeleteBody', 'detail',
        'FrayteError', 'FrayteInformation', 'SuccessfullyDelete', 'shipper', 'information', 'ErrorDeletingRecord', 'ErrorGetting','records']).then(function (translations) {
            $scope.headerTextShipperDetail = translations.Shipper + " " + translations.DeleteHeader;
            $scope.bodyTextShipperDetail = translations.DeleteBody + " " + translations.Shipper + " " + translations.detail;

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;

            $scope.TextSuccessfullyDelete = translations.SuccessfullyDelete + " " + translations.Shipper + " " + translations.information;
            $scope.TextErrorDeletingRecord = translations.ErrorDeletingRecord;
            $scope.ErrorGetting = translations.ErrorGetting + " " + translations.Shipper + " " + translations.records;
        });
    };

    $scope.AddEditShipper = function (row) {
        if (row === undefined) {
            $state.go('admin.shippers-detail', { "shipperId": 0 });
        }
        else {
            $state.go('admin.shippers-detail', { "shipperId": row.entity.UserId });
        }
    };

    $scope.RemoveShipper = function (row) {
        var modalOptions = {
            headerText: $scope.headerTextShipperDetail,
            bodyText: $scope.bodyTextShipperDetail + '?'
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            ShipperService.DeleteShipper(row.entity.UserId).then(function (response) {
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
          { name: 'CompanyName', displayName: 'CompanyName', headerCellFilter: 'translate' },
          { name: 'ContactName', displayName: 'Contact', headerCellFilter: 'translate' },
          { name: 'UserAddress.Country.Name', displayName: 'Country', headerCellFilter: 'translate' },
          { name: 'UserAddress.State', displayName: 'State', headerCellFilter: 'translate' },
          { name: 'UserAddress.City', displayName: 'City', headerCellFilter: 'translate' },
          { name: 'UserAddress.Zip', displayName: 'PostalCode', headerCellFilter: 'translate' },
          { name: 'MobileNo', displayName: 'MobileNo', headerCellFilter: 'translate' },
          { name: 'Email', headerCellFilter: 'translate' },
          { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "shipper/shipperEditButton.tpl.html", width: 65 }
        ]
    };

    $scope.LoadShippers = function () {
        ShipperService.GetShipperList().then(function (response) {
            $scope.gridOptions.data = response.data;
            $scope.shippers = response.data;
            //For full List/Data in UI-GRID
            $scope.gridOptions.excessRows = $scope.gridOptions.data.length;
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.ErrorGetting,
                showCloseButton: true
            });
        });
    };

    function init() {

        // set Multilingual Modal Popup Options
        setModalOptions();

        $scope.gridheight = SessionService.getScreenHeight();
        $scope.LoadShippers();
    }

    init();

});