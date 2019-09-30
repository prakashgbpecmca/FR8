angular.module('ngApp.receiver').controller('ReceiverController', function ($scope, $state, Upload,config, $location, $translate, $filter, ReceiverService, ShipperService, SessionService, $uibModal, uiGridConstants, toaster, CountryService, ModalService) {

    // Uplaod receiver via excel
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

            url: config.SERVICE_URL + '/Receiver/UploadReceivers',
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
        $translate(['Receiver', 'DeleteHeader', 'DeleteBody', 'detail', 'records', 'FrayteError', 'FrayteInformation',
            'FrayteValidation', 'SuccessfullyDeleteInformation', 'ErrorDeletingReocrd', 'ErrorGetting']).then(function (translations) {
            $scope.headerTextReceiverDetail = translations.Receiver + " " + translations.DeleteHeader;
            $scope.bodyTextReceiverDetail = translations.DeleteBody + " " + translations.Receiver + " " + translations.detail;

            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TextSuccessfullyDelete = translations.SuccessfullyDelete + " " + translations.receiver + " " + translations.information;       

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TextErrorDeletingReocrd = translations.ErrorDeletingReocrd;

            $scope.TextErrorErrorGettingRecord = translations.ErrorGetting + " " + translations.receiver + " " + translations.records;   
        });
    };

    var receiverEditState = '';

    $scope.AddEditReceiver = function (row) {
        if (row === undefined) {
            $state.go(receiverEditState, { "receiverId": 0 });
        }
        else {
            $state.go(receiverEditState, { "receiverId": row.entity.UserId });
        }
    };

    $scope.RemoveReceiver = function (row) {
        var modalOptions = {
            headerText: $scope.headerTextReceiverDetail,
            bodyText: $scope.bodyTextReceiverDetail
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            ReceiverService.DeleteReceiver(row.entity.UserId).then(function (response) {
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
                    body:  $scope.TextErrorDeletingReocrd,                   
                    showCloseButton: true
                });
            });
        });

    };

    $scope.LinkReceiver = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'receiver/receiverShipper/receiverLinkToShipper.tpl.html',
            controller: 'ReceiverShippersController',
            windowClass: 'ReceiverLinkDetail-Modal',
            size: 'md',
            backdrop: 'static',
            resolve: {
                receiverId: function () {
                    return row.entity.UserId;
                }
            }
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
        enableGridMenu: true,
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
          { name: 'Edit', displayName: "", enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "receiver/receiverEditButton.tpl.html", width: 65 }
        ]
    };

    $scope.LoadReceivers = function () {
        ReceiverService.GetReceiverList().then(function (response) {
            
            $scope.gridOptions.data = response.data;
            $scope.receivers = response.data;
            //For full List/Data in UI-GRID
            $scope.gridOptions.excessRows = $scope.gridOptions.data.length;
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,        
                body: $scope.TextErrorErrorGettingRecord,              
                showCloseButton: true
            });
        });
    };

    function init() {
        if ($state.is('shipper.receivers')) {
            $scope.quickBooingIcon.value = true;
        }
        // set Multilingual Modal Popup Options
        setModalOptions();

        $scope.gridheight = SessionService.getScreenHeight();
        $scope.LoadReceivers();
        if ($state.is('admin.receivers')) {
            receiverEditState = 'admin.receiver-detail';
        }
        else if ($state.is('shipper.receivers')) {
            receiverEditState = 'shipper.receiver-detail';
        }
        else if ($state.is('customer.receivers')) {
            receiverEditState = 'customer.receiver-detail';
        }
    }

    init();

});