angular.module('ngApp.uploadShipment').controller('UploadShipmentController', function ($scope, $rootScope, toaster, $translate, Upload, uiGridConstants, DownloadExcelService, CustomerService, config, $state, SessionService, UploadShipmentService, $uibModal, $http, $window, AppSpinner, ModalService, $interval, $timeout) {

    $scope.status = {
        isCustomHeaderOpen: false,
        isFirstOpen: true,
        isFirstDisabled: false
    };

    $scope.GuideLinePopup = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBookingUploadShipment/directBookingGuideLine/directBookingGuideLine.tpl.html',
            controller: 'UploadShipmentGuideLineController',
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            resolve: {
                IsCollapedFillExcel: function () {
                    return $scope.IsCollapedValue;
                }
            }
        });
        modalInstance.result.then(function () {
        }, function () {
        });
    };

    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'FrayteSuccess', 'Frayte_Success', 'FrayteWarning',
             'PleaseCorrectValidationErrors', 'PleaseSelectValidFile', 'UploadedSuccessfully', 'ErrorOccuredDuringUpload',
            'ErrorGettingShipmentDetailServer', 'FrayteWarning_Validation', 'InitialData_Validation', 'DownloadSuccessfull', 'Created_Shipment',
        'select_atleast_one', 'ShipmentNot_Avaliable', 'Removed_Shipments', 'Download_Shipments', 'Reupload_Place_Booking', 'UploadingYourShipments',
        'UploadingShipments', 'RemovingShipments', 'WithoutServiceShipments', 'WithServiceShipments', 'CSVfile_notvalid']).then(function (translations) {

            $scope.Frayte_Warning = translations.FrayteWarning;
            $scope.Frayte_Success = translations.Frayte_Success;
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TitleFrayteSuccess = translations.FrayteSuccess;
            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextPleaseSelectValidFile = translations.PleaseSelectValidFile;
            $scope.TextUploadedSuccessfully = translations.UploadedSuccessfully;
            $scope.TextErrorOccuredDuringUpload = translations.ErrorOccuredDuringUpload;
            $scope.TextErrorGettingShipmentDetailServer = translations.ErrorGettingShipmentDetailServer;
            $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
            $scope.InitialDataValidation = translations.InitialData_Validation;
            $scope.DownloadSuccessfullCsv = translations.DownloadSuccessfull;
            $scope.CreatedShipment = translations.Created_Shipment;
            $scope.selectatleastone = translations.select_atleast_one;
            $scope.ShipmentNotAvaliable = translations.ShipmentNot_Avaliable;
            $scope.Removed_Shipments = translations.Removed_Shipments;
            $scope.Reupload_Place_Booking = translations.Reupload_Place_Booking;
            $scope.Download_Shipments = translations.Download_Shipments;
            $scope.UploadingYourShipments = translations.UploadingYourShipments;
            $scope.UploadingShipments = translations.UploadingShipments;
            $scope.RemovingShipments = translations.RemovingShipments;
            $scope.WithoutServiceShipments = translations.WithoutServiceShipments;
            $scope.WithServiceShipments = translations.WithServiceShipments;
            $scope.CSV_filenotvalid = translations.CSVfile_notvalid;
        });
    };

    // Uplaod agnets via excel
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

            url: config.SERVICE_URL + '/eCommerceUploadShipment/UploadShipments',

            params: {
                CustomerId: $scope.CustomerId,
                LogisticService: $scope.LogisticService,
                ServiceType: $rootScope.ServiceType
            },
            file: $file


        });
        $scope.uploadExcel.progress($scope.progressExcel);

        $scope.uploadExcel.success($scope.successExcel);

        $scope.uploadExcel.error($scope.errorExcel);
    };

    $scope.progressExcel = function (evt) {
        AppSpinner.showSpinnerTemplate($scope.UploadingYourShipments, $scope.Template);
        //AppSpinner.hideSpinnerTemplate();
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
            //$scope.gridOptions.data = data;
            AppSpinner.hideSpinnerTemplate();

            if ($rootScope.ServiceType === 'ECOMMERCE_WS') {

                $scope.WithOutServiceShipments();
                $scope.WithOutServiceShipmentModal(data);

            }
            else if ($rootScope.ServiceType === 'ECOMMERCE_SS') {
                $scope.WithServiceShipments();
                $scope.WithServiceShipmentModal(data);
            }
        }
        else {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorOccuredDuringUpload,
                showCloseButton: true
            });
        }

    };

    $scope.errorExcel = function (err) {
        AppSpinner.hideSpinnerTemplate();
        if (err.Message === "CSV file not valid, missing header name or may be wrong name") {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.CSV_filenotvalid,
                showCloseButton: true
            });
            $scope.IsCollapedValue = false;
            $scope.GuideLinePopup();
        }
        else {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                //body: err.Message,
                body: $scope.TextErrorOccuredDuringUpload,
                showCloseButton: true
            });
        }
    };

    $scope.WithOutServiceShipmentModal = function (shipments) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'uploadShipment/uploadShipmentWithServiceError/uploadShipmentWithServiceReturnErrors.tpl.html',
            controller: 'WithServiceReturnErrorsController',
            windowClass: '',
            size: 'lg',
            resolve: {
                ShipmentData: function () {
                    return shipments;
                },
                ServiceType: function () {
                    return $rootScope.ServiceType;
                }
            }
        });
        modalInstance.result.then(function () {

        }, function () {
        });
    };

    $scope.WithServiceShipmentModal = function (shipments) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'uploadShipment/uploadShipmentWithServiceMessage/uploadShipmentWithServiceMessage.tpl.html',
            controller: 'WithServiceMessageController',
            windowClass: '',
            size: 'lg',
            resolve: {
                ShipmentData: function () {
                    return shipments;
                },
                ServiceType: function () {
                    return $rootScope.ServiceType;
                }
            }
        });
        modalInstance.result.then(function () {

        }, function () {
        });
    };

    $scope.WithServiceFinalMessageShipmentModal = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'uploadShipment/uploadShipmentWithServiceFinalMessage/uploadShipmentWithServiceFinalMessage.tpl.html',
            controller: 'WithServiceFinalMessageController',
            windowClass: '',
            size: 'lg',
            resolve: {
                BatchProcessedShipments: function () {
                    return $scope.BatchProcessedShipments;
                },
                BatchUnprocessedShipments: function () {
                    return $scope.BatchUnprocessedShipments;
                },
                BatchProcess: function () {
                    return $scope.BatchProcess;
                }
            }
        });
        modalInstance.result.then(function () {

        }, function () {
        });
    };

    $scope.SaveUploadShipmentWithOutService = function (gridDirectBooking) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'uploadShipment/uploadShipmentEcommForm/uploadShipmentForm.tpl.html',
            controller: 'uploadeCommerceBookingController',
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            resolve: {
                ShipmentId: function () {
                    return gridDirectBooking.ShipmentId;
                },
                TrackingNo: function () {
                    return gridDirectBooking.TrackingNo;
                },
                CourierCompany: function () {
                    return gridDirectBooking.DisplayName;
                }
            }
        });
        modalInstance.result.then(function () {
            $scope.WithOutServiceShipments();
        }, function () {
        });
    };

    $scope.SaveUploadShipmentWithService = function (gridDirectBooking) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'uploadShipment/uploadShipmentWithServiceEcommForm/uploadShipmentWithServiceForm.tpl.html',
            controller: 'uploadeCommerceWithServiceController',
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            resolve: {
                ShipmentId: function () {
                    return gridDirectBooking.ShipmentId;
                },
                TrackingNo: function () {
                    return gridDirectBooking.TrackingNo;
                },
                CourierCompany: function () {
                    return gridDirectBooking.DisplayName;
                }
            }
        });
        modalInstance.result.then(function () {
            $scope.WithServiceShipments();
        }, function () {
        });
    };

    var errorShowWithServiceReturn = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'uploadShipment/uploadShipmentWithServiceError/uploadShipmentWithServiceReturnErrors.tpl.html',
            controller: 'WithServiceReturnErrorsController',
            windowClass: '',
            size: 'lg',
            resolve: {
                ShipmentData: function () {
                    return row;
                },
                ServiceType: function () {
                    return $rootScope.ServiceType;
                }
            }
        });
        modalInstance.result.then(function () {

        }, function () {
        });
    };
    var intervaldata = function () {
        var i = 1;
        $scope.stop = $interval(function () {

            UploadShipmentService.GetUpdatedBatchProcess($scope.CustomerId).then(function (response) {
                $scope.BatchProcess = response.data.TotalShipments;
                $scope.BatchProcessedShipments = response.data.ProcessedShipment;
                $scope.BatchUnprocessedShipments = response.data.UnprocessedShipment;
                if (i === 1) {
                    $scope.WithServiceShipments();
                }
                i++;
                var total = $scope.BatchUnprocessedShipments + $scope.BatchProcessedShipments;
                if ($scope.BatchProcess === total) {
                    $timeout(function () {
                        $scope.stopFight();
                        $scope.ShowInterval = false;
                        $scope.WithServiceFinalMessageShipmentModal();
                        $scope.WithServiceShipments();
                    }, 3000);
                }
            });
        }, 10000);
    };
    $scope.stopFight = function () {
        if (angular.isDefined($scope.stop)) {
            $interval.cancel($scope.stop);
            $scope.stop = undefined;
        }
    };

    $scope.ErrorShow = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'uploadShipment/uploadShipmentWithoutService/uploadShipmentWithoutServiceErrors.tpl.html',
            controller: 'WithoutServiceErrorsController',
            windowClass: '',
            size: 'md',
            resolve: {
                ShipmentData: function () {
                    return row;
                }
            }
        });
        modalInstance.result.then(function () {
            $scope.WithOutServiceShipments();
        }, function () {
        });
    };
    $scope.ErrorShowWithService = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'uploadShipment/uploadShipmentWithoutService/uploadShipmentWithoutServiceErrors.tpl.html',
            controller: 'WithServiceErrorsController',
            windowClass: '',
            size: 'md',
            resolve: {
                ShipmentData: function () {
                    return row;
                }
            }
        });
        modalInstance.result.then(function () {
            $scope.WithServiceShipments();
        }, function () {
        });
    };
    $scope.ErrorShowWithServiceSuccessfulShipment = function (row) {
        if (row.IsEasyPostError) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'uploadShipment/uploadShipmentWithServiceSuccessfulError/uploadShipmentWithServiceSuccessfulErrors.tpl.html',
                controller: 'WithServiceSuccessfulErrorsController',
                windowClass: '',
                size: 'lg',
                resolve: {
                    ShipmentData: function () {
                        return row;
                    }
                }
            });
            modalInstance.result.then(function () {

            }, function () {
            });
        }
    };

    $scope.GetMouseEnter = function (row) {
        if (row.IsEasyPostError) {
            row.ShowText = 'Double click to view error(s).';
            return 'mouseenter';
        }
        else {
            row.ShowText = '';
            return;
        }
    };

    $scope.SetGridOptions = function () {


        $scope.gridOptions = {
            //onRegisterApi: function (gridApi) {
            //    grid = gridApi;
            //},
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
            //enableVerticalScrollbar: uiGridConstants.scrollbars.NEVER,
            enableVerticalScrollbar: true,

            //     rowTemplate: '<div ng-class="{ \'demoClass\':grid.appScope.rowFormatter( row ) }">' + '  <div ng-repeat="(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ng-class="{ \'ui-grid-row-header-cell\': col.isRowHeader }"  ui-grid-cell></div>' + '</div>',
            columnDefs: [
                  { name: 'DisplayStatus', displayName: 'Status', width: '14%', enableFiltering: false, enableSorting: false, headerCellFilter: 'translate' },
                  { name: 'DisplayName', displayName: 'ShipmentMethod', width: '17%', headerCellFilter: 'translate' },
                  //{ name: 'Customer', displayName: 'Ship By', width: '15%' },
                  //{ name: 'TrackingNo', displayName: 'Tracking_No', width: '12%', headerCellFilter: 'translate' },
                  { name: 'FrayteNumber', displayName: 'Frayte_ShipmentNo', width: '16%', headerCellFilter: 'translate' },
                  //{ name: 'ShipmentId', displayName: 'Frayte Shipment No #', width: '10%' },
                  { name: 'ShippedFromCompany', displayName: 'From_Shipper', width: '22%', headerCellFilter: 'translate' },
                  { name: 'ShippedToCompany', displayName: 'To_Consignee', headerCellFilter: 'translate', width: '22%' },
                  //{ name: 'ShippingDate', displayName: 'Date', cellFilter: 'dateFilter:this', width: '14%' },
                  { name: 'Edit', displayName: '', enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "uploadShipment/uploadShipmentEditButton.tpl.html", width: '9%' }

            ]
        };


    };
    $scope.SetGridOptions1 = function () {
        //$scope.ShowSuccessfullDropDown = showdropdown;

        $scope.gridOptionsSuccessful = {
            //onRegisterApi: function (gridApi) {
            //    grid = gridApi;
            //},
            showFooter: true,
            enableSorting: true,
            multiSelect: true,
            enableFiltering: true,
            enableRowSelection: false,
            enableSelectAll: true,
            enableRowHeaderSelection: true,
            selectionRowHeaderWidth: 35,
            noUnselect: false,
            enableGridMenu: false,
            enableColumnMenus: false,
            enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
            //enableVerticalScrollbar: uiGridConstants.scrollbars.NEVER,
            enableVerticalScrollbar: true,

            //     rowTemplate: '<div ng-class="{ \'demoClass\':grid.appScope.rowFormatter( row ) }">' + '  <div ng-repeat="(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ng-class="{ \'ui-grid-row-header-cell\': col.isRowHeader }"  ui-grid-cell></div>' + '</div>',
            columnDefs: [
                  { name: 'DisplayStatus', displayName: 'Status', width: '14%', enableFiltering: false, enableSorting: false, headerCellFilter: 'translate' },
                  { name: 'DisplayName', displayName: 'ShipmentMethod', width: '17%', headerCellFilter: 'translate' },
                  //{ name: 'Customer', displayName: 'Ship By', width: '15%' },
                  //{ name: 'TrackingNo', displayName: 'Tracking_No', width: '12%', headerCellFilter: 'translate' },
                  { name: 'FrayteNumber', displayName: 'Frayte_ShipmentNo', width: '16%', headerCellFilter: 'translate' },
                  //{ name: 'ShipmentId', displayName: 'Frayte Shipment No #', width: '10%' },
                  { name: 'ShippedFromCompany', displayName: 'From_Shipper', width: '19%', headerCellFilter: 'translate' },
                  { name: 'ShippedToCompany', displayName: 'To_Consignee', headerCellFilter: 'translate', width: '19%' },
                  //{ name: 'ShippingDate', displayName: 'Date', cellFilter: 'dateFilter:this', width: '14%' },
                  { name: 'Edit', displayName: '', enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "uploadShipment/uploadShipmentSuccessfullEditButton.tpl.html", width: '11%' }

            ]
        };


    };
    $scope.SetGridOptions2 = function () {


        $scope.gridOptionsUnsuccessful = {
            //onRegisterApi: function (gridApi) {
            //    grid = gridApi;
            //},
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
            //enableVerticalScrollbar: uiGridConstants.scrollbars.NEVER,
            enableVerticalScrollbar: true,

            //     rowTemplate: '<div ng-class="{ \'demoClass\':grid.appScope.rowFormatter( row ) }">' + '  <div ng-repeat="(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ng-class="{ \'ui-grid-row-header-cell\': col.isRowHeader }"  ui-grid-cell></div>' + '</div>',
            columnDefs: [
                  { name: 'DisplayStatus', displayName: 'Status', width: '14%', enableFiltering: false, enableSorting: false, headerCellFilter: 'translate' },
                  { name: 'DisplayName', displayName: 'ShipmentMethod', width: '17%', headerCellFilter: 'translate' },
                  //{ name: 'Customer', displayName: 'Ship By', width: '15%' },
                  //{ name: 'TrackingNo', displayName: 'Tracking_No', width: '12%', headerCellFilter: 'translate' },
                  { name: 'FrayteNumber', displayName: 'Frayte_ShipmentNo', width: '16%', headerCellFilter: 'translate' },
                  //{ name: 'ShipmentId', displayName: 'Frayte Shipment No #', width: '10%' },
                  { name: 'ShippedFromCompany', displayName: 'From_Shipper', width: '22%', headerCellFilter: 'translate' },
                  { name: 'ShippedToCompany', displayName: 'To_Consignee', headerCellFilter: 'translate', width: '22%' },
                  //{ name: 'ShippingDate', displayName: 'Date', cellFilter: 'dateFilter:this', width: '14%' },
                  { name: 'Edit', displayName: '', enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "uploadShipment/uploadShipmentEditButton.tpl.html", width: '9%' }

            ]
        };


    };

    //$scope.GetUnsuccessfulShipmentWithoutService = function () {

    //    UploadShipmentService.GetUnSuccessfulShipments($scope.CustomerId).then(function (response) {

    //        $scope.gridOptions.data = response.data;

    //    },
    //    function () {
    //        toaster.pop({
    //            type: 'error',
    //            title: $scope.TitleFrayteError,
    //            body: $scope.TextErrorOccuredDuringUpload,
    //            showCloseButton: true
    //        });
    //    });

    //};
    //$scope.DownLoadUnsuccessfulshipmentsWithService = function () {
    //    //AppSpinner.showSpinnerTemplate('Downloading Commercial Invoice', $scope.Template);
    //    //$scope.track.LogisticType = $scope.CurrentLogisticType.LogisticType;
    //    //$scope.track.LogisticServiceType = $scope.CurrentLogisticServiceType.LogisticServiceName;
    //    UploadShipmentService.GenerateUnsuccessfulShipmentWithService($scope.CustomerId).then(function (response) {

    //        if (response.data !== null) {
    //            var fileInfo = response.data;
    //            var fileName = {
    //                FileName: response.data.FileName,
    //                FilePath: response.data.FilePath
    //            };

    //            $http({
    //                method: 'POST',
    //                url: config.SERVICE_URL + '/eCommerceUploadShipment/DownloadUnsucessfulShipmentsWithService',
    //                data: fileName,
    //                responseType: 'arraybuffer'
    //            }).success(function (data, status, headers) {
    //                if (status == 200 && data !== null) {
    //                    headers = headers();
    //                    var filename = headers['x-filename'];
    //                    var contentType = headers['content-type'];

    //                    var linkElement = document.createElement('a');
    //                    try {
    //                        var blob = new Blob([data], { type: contentType });
    //                        var url = window.URL.createObjectURL(blob);

    //                        linkElement.setAttribute('href', url);
    //                        if (filename === undefined || filename === null) {
    //                            linkElement.setAttribute("download", "Generated_Report." + fileType);
    //                        }
    //                        else {
    //                            linkElement.setAttribute("download", filename);
    //                        }

    //                        var clickEvent = new MouseEvent("click", {
    //                            "view": window,
    //                            "bubbles": true,
    //                            "cancelable": false
    //                        });
    //                        linkElement.dispatchEvent(clickEvent);
    //                        //AppSpinner.hideSpinnerTemplate();
    //                        toaster.pop({
    //                            type: 'success',
    //                            title: $scope.Frayte_Success,
    //                            body: $scope.DownloadSuccessfullCsv,
    //                            showCloseButton: true
    //                        });
    //                    } catch (ex) {
    //                        //AppSpinner.hideSpinnerTemplate();

    //                        $window.open(fileInfo.FilePath, "_blank");
    //                        console.log(ex);
    //                    }

    //                }
    //            })
    //           .error(function (data) {
    //               // AppSpinner.hideSpinnerTemplate();
    //               console.log(data);
    //               toaster.pop({
    //                   type: 'warning',
    //                   title: $scope.Frayte_Warning,
    //                   body: $scope.ShipmentNotAvaliable,
    //                   showCloseButton: true
    //               });
    //           });

    //        }
    //        else {
    //            //AppSpinner.hideSpinnerTemplate();
    //            toaster.pop({
    //                type: 'warning',
    //                title: $scope.Frayte_Warning,
    //                body: $scope.ShipmentNotAvaliable,
    //                showCloseButton: true
    //            });
    //        }

    //    }, function () {
    //        //AppSpinner.hideSpinnerTemplate();
    //        toaster.pop({
    //            type: 'warning',
    //            title: $scope.Frayte_Warning,
    //            body: $scope.ShipmentNotAvaliable,
    //            showCloseButton: true
    //        });
    //    });
    //};
    $scope.DownLoadUnsuccessfulshipmentswithservice = function () {
        //AppSpinner.showSpinnerTemplate('Downloading Commercial Invoice', $scope.Template);
        //$scope.track.LogisticType = $scope.CurrentLogisticType.LogisticType;
        //$scope.track.LogisticServiceType = $scope.CurrentLogisticServiceType.LogisticServiceName;
        if ($scope.ShipmentsWithServiceUnsuccessful.length > 0) {
            var modalOptions = {
                headerText: $scope.Download_Shipments,
                bodyText: $scope.Reupload_Place_Booking
            };

            ModalService.Confirm({}, modalOptions).then(function (result) {
                UploadShipmentService.GenerateUnsucessfulShipmentsWithServcie($scope.ShipmentsWithServiceUnsuccessful).then(function (response) {
                    $scope.RemoveShipmentCount2 = 0;
                    if (response.data !== null) {
                        var fileInfo = response.data;
                        var fileName = {
                            FileName: response.data.FileName,
                            FilePath: response.data.FilePath
                        };

                        $http({
                            method: 'POST',
                            url: config.SERVICE_URL + '/eCommerceUploadShipment/DownloadUnsucessfulShipments',
                            data: fileName,
                            responseType: 'arraybuffer'
                        }).success(function (data, status, headers) {
                            if (status == 200 && data !== null) {
                                headers = headers();
                                var filename = headers['x-filename'];
                                var contentType = headers['content-type'];

                                var linkElement = document.createElement('a');
                                try {
                                    var blob = new Blob([data], { type: contentType });
                                    var url = window.URL.createObjectURL(blob);

                                    linkElement.setAttribute('href', url);
                                    if (filename === undefined || filename === null) {
                                        linkElement.setAttribute("download", "Generated_Report." + fileType);
                                    }
                                    else {
                                        linkElement.setAttribute("download", filename);
                                    }

                                    var clickEvent = new MouseEvent("click", {
                                        "view": window,
                                        "bubbles": true,
                                        "cancelable": false
                                    });
                                    linkElement.dispatchEvent(clickEvent);
                                    //AppSpinner.hideSpinnerTemplate();
                                    toaster.pop({
                                        type: 'success',
                                        title: $scope.Frayte_Success,
                                        body: $scope.DownloadSuccessfullCsv,
                                        showCloseButton: true
                                    });
                                } catch (ex) {
                                    $scope.RemoveShipmentCount2 = 0;
                                    //AppSpinner.hideSpinnerTemplate();
                                    $scope.WithServiceShipments();
                                    $scope.ShipmentsWithServiceUnsuccessful = [];
                                    $window.open(fileInfo.FilePath, "_blank");
                                    console.log(ex);
                                }

                            }
                        })
                       .error(function (data) {
                           // AppSpinner.hideSpinnerTemplate();
                           console.log(data);
                           toaster.pop({
                               type: 'warning',
                               title: $scope.FrayteWarningValidation,
                               body: $scope.ShipmentNotAvaliable,
                               showCloseButton: true
                           });
                       });

                    }
                    else {
                        //AppSpinner.hideSpinnerTemplate();
                        toaster.pop({
                            type: 'warning',
                            title: $scope.FrayteWarningValidation,
                            body: $scope.ShipmentNotAvaliable,
                            showCloseButton: true
                        });
                    }

                }, function () {
                    //AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteWarningValidation,
                        body: $scope.ShipmentNotAvaliable,
                        showCloseButton: true
                    });
                });
            });
        }
    };

    $scope.DownLoadUnsuccessfulshipmentswithoutservice = function () {
        //AppSpinner.showSpinnerTemplate('Downloading Commercial Invoice', $scope.Template);
        //$scope.track.LogisticType = $scope.CurrentLogisticType.LogisticType;
        //$scope.track.LogisticServiceType = $scope.CurrentLogisticServiceType.LogisticServiceName;
        if ($scope.ShipmentsWithoutService.length > 0) {
            var modalOptions = {
                headerText: $scope.Download_Shipments,
                bodyText: $scope.Reupload_Place_Booking
            };

            ModalService.Confirm({}, modalOptions).then(function (result) {
                UploadShipmentService.GenerateUnsuccessfulShipmentWithoutService($scope.ShipmentsWithoutService).then(function (response) {
                    $scope.RemoveShipmentCount = 0;
                    if (response.data !== null) {
                        var fileInfo = response.data;
                        var fileName = {
                            FileName: response.data.FileName,
                            FilePath: response.data.FilePath
                        };

                        $http({
                            method: 'POST',
                            url: config.SERVICE_URL + '/eCommerceUploadShipment/DownloadUnsucessfulShipments',
                            data: fileName,
                            responseType: 'arraybuffer'
                        }).success(function (data, status, headers) {
                            if (status == 200 && data !== null) {
                                $scope.WithOutServiceShipments();

                                headers = headers();
                                var filename = headers['x-filename'];
                                var contentType = headers['content-type'];

                                var linkElement = document.createElement('a');
                                try {
                                    var blob = new Blob([data], { type: contentType });
                                    var url = window.URL.createObjectURL(blob);

                                    linkElement.setAttribute('href', url);
                                    if (filename === undefined || filename === null) {
                                        linkElement.setAttribute("download", "Generated_Report." + fileType);
                                    }
                                    else {
                                        linkElement.setAttribute("download", filename);
                                    }

                                    var clickEvent = new MouseEvent("click", {
                                        "view": window,
                                        "bubbles": true,
                                        "cancelable": false
                                    });
                                    linkElement.dispatchEvent(clickEvent);
                                    //AppSpinner.hideSpinnerTemplate();

                                } catch (ex) {
                                    //AppSpinner.hideSpinnerTemplate();
                                    toaster.pop({
                                        type: 'success',
                                        title: $scope.Frayte_Success,
                                        body: $scope.DownloadSuccessfullCsv,
                                        showCloseButton: true
                                    });
                                    $scope.RemoveShipmentCount = 0;
                                    $scope.WithOutServiceShipments();
                                    $scope.ShipmentsWithoutService = [];
                                    $window.open(fileInfo.FilePath, "_blank");
                                    console.log(ex);
                                }

                            }
                        })
                       .error(function (data) {
                           // AppSpinner.hideSpinnerTemplate();
                           console.log(data);
                           $scope.RemoveShipmentCount = 0;
                           toaster.pop({
                               type: 'warning',
                               title: $scope.FrayteWarningValidation,
                               body: $scope.ShipmentNotAvaliable,
                               showCloseButton: true
                           });
                       });

                    }
                    else {
                        //AppSpinner.hideSpinnerTemplate();
                        toaster.pop({
                            type: 'warning',
                            title: $scope.FrayteWarningValidation,
                            body: $scope.ShipmentNotAvaliable,
                            showCloseButton: true
                        });
                    }

                }, function () {
                    //AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteWarningValidation,
                        body: $scope.ShipmentNotAvaliable,
                        showCloseButton: true,
                        closeHtml: '<button>Close</button>'
                    });
                });
            });
        }

    };

    $scope.GetServices = function () {
        UploadShipmentService.GetServices().then(function (response) {
            if (response.data.length > 0) {
                $scope.ServcieDetail = response.data;
            }
            else {
                console.log(response);
            }
        },
     function () {
         if (response.status !== 401) {
             toaster.pop({
                 type: 'error',
                 title: $scope.TitleFrayteError,
                 body: $scope.TextErrorOccuredDuringUpload,
                 showCloseButton: true
             });
         }
     });
    };

    $scope.SaveShipmentWithService = function () {
        if ($scope.ServiceCount > 0) {
            AppSpinner.showSpinnerTemplate($scope.UploadingShipments, $scope.Template);
            UploadShipmentService.SaveShipmentWithService($scope.gridOptionsSuccessfuldata).then(function (response) {
                //if (response.status === 200 && response.data[0].Error !== null && response.data[0].Error.Status === true) {
                AppSpinner.hideSpinnerTemplate();
                if (response.status === 200) {
                    var res = response.data;
                    $scope.WithServiceShipments();
                    errorShowWithServiceReturn(res);
                    $scope.RemoveShipmentCount1 = 0;
                    //$scope.WithServiceShipments();
                    intervaldata();
                    $scope.ShowInterval = true;
                }
                else {
                    $scope.RemoveShipmentCount1 = 0;
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorOccuredDuringUpload,
                        showCloseButton: true
                    });
                }
            },
        function () {
            $scope.RemoveShipmentCount1 = 0;
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorOccuredDuringUpload,
                showCloseButton: true
            });
        });
        }
        else {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarningValidation,
                body: $scope.selectatleastone,
                showCloseButton: true
            });
        }
    };


    $scope.RemoveShipmentWithService = function () {
        if ($scope.RemoveShipmentCount1 > 0) {
            AppSpinner.showSpinnerTemplate($scope.RemovingShipments, $scope.Template);
            UploadShipmentService.RemoveShipmentWithService($scope.ShipmentsWithService).then(function (response) {
                //if (response.status === 200 && response.data[0].Error !== null && response.data[0].Error.Status === true) {
                AppSpinner.hideSpinnerTemplate();
                if (response.status === 200) {
                    var res = response.data;
                    $scope.IsSelected = false;
                    $scope.WithServiceShipments();
                    $scope.RemoveShipmentCount1 = 0;
                    //errorShowWithServiceReturn(res);
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteSuccess,
                        body: $scope.Removed_Shipments,
                        showCloseButton: true
                    });
                    //$scope.WithServiceShipments();


                }
                else {
                    $scope.RemoveShipmentCount1 = 0;
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorOccuredDuringUpload,
                        showCloseButton: true
                    });
                }
            },
        function () {
            $scope.RemoveShipmentCount = 0;
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorOccuredDuringUpload,
                showCloseButton: true
            });
        });
        }
        else {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarningValidation,
                body: $scope.selectatleastone,
                showCloseButton: true
            });
        }
    };

    $scope.RemoveShipmentWithServiceUnsuccessfullShipment = function () {
        if ($scope.RemoveShipmentCount2 > 0) {
            AppSpinner.showSpinnerTemplate($scope.RemovingShipments, $scope.Template);
            UploadShipmentService.RemoveShipmentWithService($scope.ShipmentsWithServiceUnsuccessful).then(function (response) {
                //if (response.status === 200 && response.data[0].Error !== null && response.data[0].Error.Status === true) {
                AppSpinner.hideSpinnerTemplate();
                if (response.status === 200) {
                    var res = response.data;
                    $scope.RemoveShipmentCount2 = 0;
                    $scope.IsSelected = false;
                    $scope.ShipmentsWithServiceUnsuccessful = [];
                    $scope.WithServiceShipments();
                    //errorShowWithServiceReturn(res);
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteSuccess,
                        body: $scope.Removed_Shipments,
                        showCloseButton: true
                    });
                    //$scope.WithServiceShipments();


                }
                else {
                    $scope.RemoveShipmentCount2 = 0;
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorOccuredDuringUpload,
                        showCloseButton: true
                    });
                }
            },
        function () {
            $scope.RemoveShipmentCount2 = 0;
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorOccuredDuringUpload,
                showCloseButton: true
            });
        });
        }
        else {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarningValidation,
                body: $scope.selectatleastone,
                showCloseButton: true
            });
        }
    };

    $scope.RemoveShipmentWithoutServiceUnsuccessfullShipment = function () {
        if ($scope.RemoveShipmentCount > 0) {
            AppSpinner.showSpinnerTemplate($scope.RemovingShipments, $scope.Template);
            UploadShipmentService.RemoveShipmentWithService($scope.ShipmentsWithoutService).then(function (response) {
                //if (response.status === 200 && response.data[0].Error !== null && response.data[0].Error.Status === true) {
                AppSpinner.hideSpinnerTemplate();
                if (response.status === 200) {
                    var res = response.data;
                    $scope.ShipmentsWithoutService = [];
                    $scope.IsSelected = false;
                    $scope.RemoveShipmentCount = 0;
                    $scope.WithOutServiceShipments();
                    //errorShowWithServiceReturn(res);
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteSuccess,
                        body: $scope.Removed_Shipments,
                        showCloseButton: true
                    });
                    //$scope.WithServiceShipments();


                }
                else {
                    $scope.RemoveShipmentCount = 0;
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorOccuredDuringUpload,
                        showCloseButton: true
                    });
                }
            },
        function () {
            $scope.RemoveShipmentCount = 0;
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorOccuredDuringUpload,
                showCloseButton: true
            });
        });
        }
        else {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarningValidation,
                body: $scope.selectatleastone,
                showCloseButton: true
            });
        }
    };

    $scope.SetServcieDetail = function (Service) {
        $scope.ServiceCount = 0;
        for (i = 0; i < $scope.gridOptionsSuccessfuldata.length; i++) {

            if ($scope.gridOptionsSuccessfuldata[i].ServiceValue !== null && $scope.gridOptionsSuccessfuldata[i].ServiceValue !== undefined && $scope.gridOptionsSuccessfuldata[i].ServiceValue === true) {
                $scope.ServiceCount = $scope.ServiceCount + 1;
                $scope.gridOptionsSuccessfuldata[i].Service = {};
                $scope.gridOptionsSuccessfuldata[i].Service = Service;

            }

            else {
                //toaster.pop({
                //    type: 'warning',
                //    title: 'FRAYTE - WARNING',
                //    body: 'Please select the shipment first',
                //    showCloseButton: true
                //});
                $scope.gridOptionsSuccessfuldata[i].ServiceValue = false;
                $scope.gridOptionsSuccessfuldata[i].Service = {};
            }
        }

    };

    $scope.WithOutServiceShipments = function () {
        AppSpinner.showSpinnerTemplate($scope.WithoutServiceShipments, $scope.Template);
        UploadShipmentService.GetUnSuccessfulShipments($scope.CustomerId).then(function (response) {

            $scope.gridOptions.data = response.data;
            for (i = 0; i < response.data.length; i++) {
                response.data[i].DisplayStatus = 'Failed';
            }
            //if (response.data.length === 0) {
            //    $scope.IsSelected = false;
            //}

            $scope.UnsuccessfulWithoutShipmentList = response.data;
            AppSpinner.hideSpinnerTemplate();
        },
     function () {
         AppSpinner.hideSpinnerTemplate();
         toaster.pop({
             type: 'error',
             title: $scope.TitleFrayteError,
             body: $scope.TextErrorOccuredDuringUpload,
             showCloseButton: true
         });
     });
    };
    $scope.WithServiceShipments = function () {
        AppSpinner.showSpinnerTemplate($scope.WithServiceShipments, $scope.Template);
        UploadShipmentService.GetShipmentsFromDraft($scope.CustomerId).then(function (response) {
            //$scope.gridOptionsSuccessful
            //$scope.Unsuccessful = response.data;
            $scope.gridOptionsSuccessful.data = response.data.SucessfulShipments;
            $scope.gridOptionsSuccessfuldata = response.data.SucessfulShipments;
            $scope.gridOptionsUnsuccessful.data = response.data.UnsucessfulShipments;
            $scope.gridOptionsUnsuccessfuldata = response.data.UnsucessfulShipments;
            AppSpinner.hideSpinnerTemplate();
        },
     function () {
         AppSpinner.hideSpinnerTemplate();
         toaster.pop({
             type: 'error',
             title: $scope.TitleFrayteError,
             body: $scope.TextErrorOccuredDuringUpload,
             showCloseButton: true
         });
     });
    };
    $scope.toggleRowSelection = function () {
        $scope.gridApi.selection.clearSelectedRows();
        $scope.gridOptions1.enableRowSelection = !$scope.gridOptions1.enableRowSelection;
        $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS);
    };


    $scope.rowSelectionCustomGrid = function (gridDirectBooking) {

        if (gridDirectBooking.IsSelected === true) {

            $scope.ShipmentsWithoutService.push(gridDirectBooking);
            $scope.RemoveShipmentCount++;
        }
        else {
            $scope.IsSelected = true;
            for (i = 0; i < $scope.ShipmentsWithoutService.length; i++) {

                if (gridDirectBooking.ShipmentId === $scope.ShipmentsWithoutService[i].ShipmentId) {
                    $scope.ShipmentsWithoutService.splice(i, 1);
                    $scope.RemoveShipmentCount--;
                }
            }
        }
    };

    $scope.allrowSelectionCustomGrid = function (IsSelected, gridDirectBooking) {

        $scope.ShipmentsWithoutService = [];
        if (IsSelected === true) {
            for (i = 0; i < gridDirectBooking.length; i++) {
                gridDirectBooking[i].IsSelected = true;

                $scope.RemoveShipmentCount++;
                $scope.ShipmentsWithoutService.push(gridDirectBooking[i]);
            }

        }
        else {
            $scope.ShipmentsWithoutService = [];
            for (i = 0; i < gridDirectBooking.length; i++) {
                gridDirectBooking[i].IsSelected = false;
                $scope.ShipmentsWithoutService.splice(gridDirectBooking[i], 1);
                $scope.RemoveShipmentCount--;
            }
        }
    };

    $scope.rowSelectionSelectServiceCustomGrid = function (gridDirectBooking) {

        if (gridDirectBooking.IsSelected === true) {
            $scope.RemoveShipmentCount1++;
            gridDirectBooking.ServiceValue = true;
            $scope.ShipmentsWithService.push(gridDirectBooking);

        }
        else {
            $scope.IsSelected1 = false;
            for (i = 0; i < $scope.ShipmentsWithService.length; i++) {
                if (gridDirectBooking.ShipmentId === $scope.ShipmentsWithService[i].ShipmentId) {
                    $scope.ShipmentsWithService.splice(i, 1);
                    gridDirectBooking.ServiceValue = false;
                    $scope.RemoveShipmentCount1--;
                    gridDirectBooking.Service = {};
                    $scope.ServiceCount--;
                }
            }
        }
    };

    $scope.allrowSelectionSelectServiceCustomGrid = function (IsSelected1, gridDirectBooking) {

        $scope.ShipmentsWithService = [];
        if (IsSelected1 === true) {
            for (i = 0; i < gridDirectBooking.length; i++) {
                $scope.RemoveShipmentCount1++;
                gridDirectBooking[i].IsSelected = true;
                gridDirectBooking[i].ServiceValue = true;
                $scope.ShipmentsWithService.push(gridDirectBooking[i]);
            }

        }
        else {
            $scope.ShipmentsWithService = [];
            for (i = 0; i < gridDirectBooking.length; i++) {
                $scope.RemoveShipmentCount1--;
                $scope.ShipmentsWithService.splice(gridDirectBooking[i], 1);
                gridDirectBooking[i].ServiceValue = false;
                gridDirectBooking[i].IsSelected = false;
            }
        }
    };

    $scope.rowSelectionUnsuccessfulCustomGrid = function (gridDirectBooking) {

        if (gridDirectBooking.IsSelected === true) {
            $scope.RemoveShipmentCount2++;
            gridDirectBooking.ServiceValue = true;
            $scope.ShipmentsWithServiceUnsuccessful.push(gridDirectBooking);

        }
        else {
            $scope.IsSelected2 = false;
            for (i = 0; i < $scope.ShipmentsWithServiceUnsuccessful.length; i++) {
                if (gridDirectBooking.ShipmentId === $scope.ShipmentsWithServiceUnsuccessful[i].ShipmentId) {
                    $scope.ShipmentsWithServiceUnsuccessful.splice(i, 1);
                    gridDirectBooking.ServiceValue = false;
                    $scope.RemoveShipmentCount2--;
                    gridDirectBooking.Service = {};
                    $scope.ServiceCount--;
                }
            }
        }
    };

    $scope.allrowSelectionUnsuccessfullCustomGrid = function (IsSelected2, gridDirectBooking) {

        $scope.ShipmentsWithServiceUnsuccessful = [];
        if (IsSelected2 === true) {
            for (i = 0; i < gridDirectBooking.length; i++) {
                $scope.RemoveShipmentCount2++;
                gridDirectBooking[i].IsSelected = true;
                gridDirectBooking[i].ServiceValue = true;
                $scope.ShipmentsWithServiceUnsuccessful.push(gridDirectBooking[i]);
            }

        }
        else {
            $scope.ShipmentsWithService = [];
            for (i = 0; i < gridDirectBooking.length; i++) {
                $scope.RemoveShipmentCount2--;
                $scope.ShipmentsWithServiceUnsuccessful.splice(gridDirectBooking[i], 1);
                gridDirectBooking[i].ServiceValue = false;
                gridDirectBooking[i].IsSelected = false;
            }
        }
    };

    //$scope.DeleteShipment = function (customer) {
    //    var modalOptions = {
    //        headerText: $scope.CustomerDeleteHeader,

    //        bodyText: $scope.CustomerDelete
    //    };

    //    ModalService.Confirm({}, modalOptions).then(function (result) {
    //        CustomerService.DeleteCustomer(customer.UserId).then(function () {
    //            toaster.pop({
    //                type: 'success',
    //                title: $scope.TitleFrayteInformation,

    //                body: $scope.CustomerDeleteValidation,
    //                showCloseButton: true,
    //                closeHtml: '<button></button>'
    //            });
    //            $scope.LoadCustomers();
    //        }, function () {
    //            toaster.pop({
    //                type: 'error',
    //                title: $scope.TitleFrayteError,
    //                body: $scope.CustomerDeleteErrorValidation,
    //                showCloseButton: true,
    //                closeHtml: '<button>Close</button>'
    //            });
    //        });
    //    });

    //};

    $scope.rowSelection = function () {

        $scope.gridApi.selection.on.rowSelectionChanged($scope, function (row) {

            if (row.isSelected === true) {

                row.entity.ServiceValue = true;
            }
            else {

                //for (i = 0; i < $scope.gridOptionsSuccessful.data.length; i++) {
                //    if (row.entity.ShipmentId === $scope.FrayteManifestShipment.DirectShipments[i].ShipmentId) {
                //        $scope.FrayteManifestShipment.DirectShipments.splice(i, 1);
                //    }
                //}

                row.entity.ServiceValue = false;
                row.entity.Service = {};
                $scope.ServiceCount--;
            }

        });



        // Multiple row selections
        $scope.gridApi.selection.on.rowSelectionChangedBatch($scope, function (rows) {

            for (i = 0; i < rows.length; i++) {

                if (rows[i].isSelected === true) {
                    rows[i].entity.ServiceValue = true;
                }
                else {
                    rows[i].entity.ServiceValue = true;
                }

            }


        });
    };

    //$scope.Csvfilepaths = function () {
    //    if ($scope.ServiceType === 'ECOMMERCE_WS') {
    //        $scope.SampleExcel = response.data[7];
    //    }
    //    else if ($scope.ServiceType === 'ECOMMERCE_SS') {
    //        $scope.SampleExcel = response.data[8];
    //    }
    //};
    $scope.SetServiceType = function (serviceType) {
        $rootScope.ServiceType = serviceType;
    };


    function init() {
        $scope.RemoveShipmentCount = 0;
        $scope.RemoveShipmentCount1 = 0;
        $scope.RemoveShipmentCount2 = 0;
        $scope.ShowInterval = false;
        $scope.ShipmentsWithServiceUnsuccessful = [];
        $scope.ShipmentsWithoutService = [];
        $scope.ShipmentsWithService = [];
        $scope.GetServices();
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.LogisticService = "DHLExpress";
        $rootScope.ServiceType = 'Empty';
        var userInfo = SessionService.getUser();
        $scope.RoleId = userInfo.RoleId;
        $scope.CustomerId = userInfo.EmployeeId;

        DownloadExcelService.GetPieceDetailsExcelPath().then(function (response) {

            $scope.picesExcelDownload = response.data[0];
            $scope.customerExcelDownload = response.data[1];
            $scope.shipperExcelDownload = response.data[2];
            $scope.receiverExcelDownload = response.data[3];
            $scope.agentExcelDownload = response.data[4];
            $scope.shipmentExcelDownloadWithoutService = response.data[5];
            $scope.shipmentExcelDownloadWithService = response.data[6];
            $scope.SampleExcelWithoutService = response.data[7];
            $scope.SampleExcelWithService = response.data[8];


        }, function () {
            if (response.status !== 401) {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorGettingSettingDetail,
                    showCloseButton: true
                });
            }
        });

        CustomerService.GetCustomerDetail($scope.CustomerId).then(function (response) {
            $scope.CustomerDetail = response.data;
            if ($scope.CustomerDetail.IsServiceSelected === true && $scope.CustomerDetail.IsWithoutService === false) {
                $scope.ServiceType = 'ECOMMERCE_SS';
                $scope.WithServiceShipments();
            }
            else if ($scope.CustomerDetail.IsServiceSelected === false && $scope.CustomerDetail.IsWithoutService === true) {
                $scope.ServiceType = 'ECOMMERCE_WS';
                $scope.WithOutServiceShipments();
            }
        }, function () {

        });


        setMultilingualOptions();
        $scope.SetGridOptions();
        $scope.SetGridOptions1();
        $scope.SetGridOptions2();
        $scope.gridOptionsSuccessful.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
            $scope.rowSelection();
        };
        $scope.IsCollapedValue = true;
        $scope.gridheight = SessionService.getScreenHeight();
    }

    init();

});