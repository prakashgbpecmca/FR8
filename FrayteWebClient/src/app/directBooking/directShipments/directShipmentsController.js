//angular.module('ngApp.directBooking').controller('DirectBookingController', function ($scope, $state, $translate, DirectBookingService, SessionService) {
angular.module('ngApp.directBooking').controller('DirectShipmentController', function (AppSpinner,UtilityService, $http, $window, $scope, ModalService, uiGridConstants, DirectShipmentService, config, DirectBookingService, $rootScope, $translate, $location, Upload, $state, $filter, ShipmentService, TimeZoneService, CountryService, CourierService, HomeService, ShipperService, ReceiverService, SessionService, $uibModal, $log, toaster, $stateParams, CustomerService, UserService) {

    $scope.DownLoadDirectBookingReport = function () {
        if ($scope.CurrentStatus.DisplayStatusName === 'All') {
            $scope.track.ShipmentStatusId = 12;
        }
        else {
            $scope.track.ShipmentStatusId = $scope.CurrentStatus.ShipmentStatusId;
        }
        AppSpinner.showSpinnerTemplate('Downloading Track & Trace Excel', $scope.Template);
        $scope.track.LogisticType = $scope.CurrentLogisticType.LogisticType;
        $scope.track.LogisticServiceType = $scope.CurrentLogisticServiceType.LogisticServiceName;
        DirectShipmentService.GenerateTrackAndTraceReport($scope.track).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            if (response.data !== null) {
                var fileInfo = response.data;
                var fileName = {
                    FileName: response.data.FileName
                };

                $http({
                    method: 'POST',
                    url: config.SERVICE_URL + '/DirectBooking/DownloadTrackAndTraceReport',
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

                            toaster.pop({
                                type: 'success',
                                title: $scope.Frayte_Success,
                                body: 'Report generated and downloaded successfully in your system. Please check browser download folder.',
                                showCloseButton: true
                            });
                        } catch (ex) {
                            $window.open(fileInfo.FilePath, "_blank");
                            console.log(ex);
                        }

                    }
                })
               .error(function (data) {
                   AppSpinner.hideSpinner();
                   console.log(data);
                   toaster.pop({
                       type: 'error',
                       title: $scope.Frayte_Error,
                       body: 'Could not download the report. Please try again.',
                       showCloseButton: true
                   });
               });

            }
            else {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.Frayte_Error,
                    body: "Report can not download at the moment. Please try again later.",
                    showCloseButton: true
                });
            }

        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.Frayte_Error,
                body: "Report can not download at the moment. Please try again later.",
                showCloseButton: true
            });
        });
    };

    $scope.DownLoadCommercialInvoice = function (row) {
        AppSpinner.showSpinnerTemplate('Downloading Commercial Invoice', $scope.Template);
        $scope.track.LogisticType = $scope.CurrentLogisticType.LogisticType;
        $scope.track.LogisticServiceType = $scope.CurrentLogisticServiceType.LogisticServiceName;
        DirectShipmentService.GenerateCommercialInvoice(row.entity.ShipmentId).then(function (response) {

            if (response.data !== null) {
                var fileInfo = response.data;
                var fileName = {
                    FileName: response.data.FileName,
                    FilePath: response.data.FilePath
                };

                $http({
                    method: 'POST',
                    url: config.SERVICE_URL + '/DirectBooking/DownloadrateDirectBookinCommercialInvoice',
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
                            AppSpinner.hideSpinnerTemplate();
                            toaster.pop({
                                type: 'success',
                                title: $scope.Frayte_Success,
                                body: 'Commercial Invoice downloaded successfully in your system. Please check browser download folder.',
                                showCloseButton: true
                            });
                        } catch (ex) {
                            AppSpinner.hideSpinnerTemplate();
                            $window.open(fileInfo.FilePath, "_blank");
                            console.log(ex);
                        }

                    }
                })
               .error(function (data) {
                   AppSpinner.hideSpinnerTemplate();
                   console.log(data);
                   toaster.pop({
                       type: 'warning',
                       title: $scope.Frayte_Warning,
                       body: 'Commercial Invoice is not avaliable.',
                       showCloseButton: true
                   });
               });

            }
            else {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'warning',
                    title: $scope.Frayte_Warning,
                    body: "Commercial Invoice is not avaliable.",
                    showCloseButton: true
                });
            }

        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: "Commercial Invoice is not avaliable.",
                showCloseButton: true
            });
        });
    };

    $scope.SeachDirectShipments = function () {
        $scope.DirectShipments();
    };
    var statusTypeTemplate = '<div class="ui-grid-cell-contents"><img ng-src="{{grid.appScope.buildURL}}{{grid.appScope.currentImgURL(row)}}" style="width:40px;margin: -5px 0px 0px;position:relative;">{{grid.appScope.GetStatus(row)}}</div>';
    $scope.currentImgURL = function (row) {
        if (row !== undefined) {
            var url = '';
            for (var i = 0; i < $scope.DirectShipmentStatus.length; i++) {
                if (row.entity.Status == $scope.DirectShipmentStatus[i].StatusName) {
                    url = $scope.DirectShipmentStatus[i].ImgURL;
                    break;
                }
            }
            return url;
        }
    };

    $scope.GetStatus = function (row) {
        if (row !== undefined) {
            return row.entity.DisplayStatus;
        }
    };
    $scope.rowFormatter = function (row) {
        return true;
    };

    var trackingTemplate = '<div class="paddingTB5 word-wrap"><a target="_blank" ui-sref="home.tracking-hub({carrierType:row.entity.CourierName,trackingId:row.entity.TrackingCode, RateType: row.entity.RateType})">{{row.entity.TrackingNo}}</a></div>';
    $scope.SetGridOptions = function () {
        if ($scope.$UserRoleId === 1) {
            $scope.gridOptions = {
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
                //enableVerticalScrollbar: uiGridConstants.scrollbars.NEVER,
                enableVerticalScrollbar: true,
                columnDefs: [
                      { name: 'DisplayStatus', displayName: 'Status', width: '10%', headerCellFilter: 'translate', enableFiltering: false, enableSorting: false, cellTemplate: statusTypeTemplate },
                      { name: 'Customer', headerCellFilter: 'translate', width: '15%' },
                      { name: 'DisplayName', displayName: 'Courier', headerCellFilter: 'translate', width: '15%' },
                      { name: 'TrackingNo', displayName: 'Tracking_No', headerCellFilter: 'translate', cellTemplate: trackingTemplate, width: '15%' },
                      //{ name: 'ShipmentId', displayName: 'Frayte_ShipmentNo', headerCellFilter: 'translate', width: '15%' },
                      { name: 'FrayteNumber', displayName: 'Frayte_ShipmentNo', headerCellFilter: 'translate', width: '17%' },
                      { name: 'ShippedToCompany', displayName: 'To_Consignee', headerCellFilter: 'translate', width: '18%' },
                      //{ name: 'ShippingDate', displayName: 'Date', cellFilter: 'dateFilter:this', width: '14%' },
                      { name: 'Edit', displayName: '', enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "directBooking/directShipments/directShipmentEditButton.tpl.html" }

                ]
            };
        }
        else {
            $scope.gridOptions = {
                //onRegisterApi: function (gridApi) {
                //    grid = gridApi;
                //},
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
                //enableVerticalScrollbar: uiGridConstants.scrollbars.NEVER,
                enableVerticalScrollbar: true,

                //     rowTemplate: '<div ng-class="{ \'demoClass\':grid.appScope.rowFormatter( row ) }">' + '  <div ng-repeat="(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ng-class="{ \'ui-grid-row-header-cell\': col.isRowHeader }"  ui-grid-cell></div>' + '</div>',
                columnDefs: [
                      { name: 'DisplayStatus', displayName: 'Status', width: '13%', enableFiltering: false, enableSorting: false, headerCellFilter: 'translate', cellTemplate: statusTypeTemplate },
                      { name: 'DisplayName', displayName: 'ShipmentMethod', width: '15%', headerCellFilter: 'translate' },
                      //{ name: 'Customer', displayName: 'Ship By', width: '15%' },
                      { name: 'TrackingNo', displayName: 'Tracking_No', cellTemplate: trackingTemplate, width: '12%', headerCellFilter: 'translate' },
                      { name: 'FrayteNumber', displayName: 'Frayte_ShipmentNo', width: '13%', headerCellFilter: 'translate' },
                      //{ name: 'ShipmentId', displayName: 'Frayte Shipment No #', width: '10%' },
                      { name: 'ShippedFromCompany', displayName: 'From_Shipper', width: '20%', headerCellFilter: 'translate' },
                      { name: 'ShippedToCompany', displayName: 'To_Consignee', headerCellFilter: 'translate', width: '17%' },
                      //{ name: 'ShippingDate', displayName: 'Date', cellFilter: 'dateFilter:this', width: '14%' },
                      { name: 'Edit', displayName: '', enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "directBooking/directShipments/directShipmentEditButton.tpl.html" }

                ]
            };
        }

    };
    var setMultilingualOptions = function () {
        $translate(['Success', 'Record_Saved', 'FrayteError_Validation', 'ErrorSavingRecord', 'GettingDetails_Error', 'CancelShipmentError_Validation',
       'ErrorDeletingRecord', 'DeletingShipmentError_Validation', 'GettingDataError_Validation', 'ReceiveDetail_Validation', 'ShipmentCancelConfirmText',
        'Confirmation', 'FrayteSuccess', 'FrayteWarning', 'FrayteError', 'Darft_Delete', 'SuccessfullyDelete']).then(function (translations) {
            $scope.Successfully_Delete = translations.Darft_Delete;
            $scope.Frayte_Success = translations.FrayteSuccess;
            $scope.Frayte_Warning = translations.FrayteWarning;
            $scope.RecordSaved = translations.Record_Saved;
            $scope.Frayte_Error = translations.FrayteError;
            $scope.ErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.GettingDetailsError = translations.GettingDetails_Error;
            $scope.CancelShipmentErrorValidation = translations.CancelShipmentError_Validation;
            $scope.ErrorDeletingRecord = translations.ErrorDeletingRecord;
            $scope.DeletingShipmentErrorValidation = translations.DeletingShipmentError_Validation;
            $scope.GettingDataErrorValidation = translations.GettingDataError_Validation;
            $scope.ReceiveDetailValidation = translations.ReceiveDetail_Validation;
            $scope.Confirmation_ = translations.Confirmation;
            $scope.ShipmentCancel_ConfirmText = translations.ShipmentCancelConfirmText;

        });
    };
    $scope.cancelShipment = function (row) {

        if (row !== undefined && row.entity !== null) {
            var modalOptions = {
                headerText: $scope.Confirmation_,
                bodyText: $scope.ShipmentCancel_ConfirmText
            };

            ModalService.Confirm({}, modalOptions).then(function (result) {
                //$scope.CancelMessage = 'Canceling the shipment...';
                $scope.Template = 'directBooking/ajaxLoader.tpl.html';
                AppSpinner.showSpinnerTemplate('Canceling the shipment', $scope.Template);
                DirectBookingService.CancelShipment(row.entity.ShipmentId).then(function (response) {
                    AppSpinner.hideSpinnerTemplate();
                    if (response.status === 200 && row.entity.ManifestId !== 0) {
                        var modalInstance = $uibModal.open({
                            animation: true,
                            templateUrl: 'directBooking/directBookingDetail/directBookingResponse.tpl.html',
                            windowClass: '',
                            size: 'md'

                        });
                        modalInstance.result.then(function () {
                            // $state.go('customer.direct-shipments', {}, { reload: true });
                        }, function () {
                        });
                        //toaster.pop({
                        //    type: 'success',
                        //    title: $scope.Frayte_Success,
                        //    body: "Request have been submitted and someone will be contacted in an hour.",
                        //    showCloseButton: true
                        //});

                    }
                }, function () {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.Frayte_Error,
                        body: $scope.CancelShipmentErrorValidation,
                        showCloseButton: true
                    });
                });
            });
        }



    };

    $scope.CreateShipment = function (row, CallingType) {
        if (row !== undefined && row !== null && row.entity !== null) {

            // ToDo: Need to check who is editing the shipment

            if ($scope.track.BookingMethod === "DirectBooking") {

                if ($scope.tab !== undefined && $scope.tab !== null && $scope.tab.childTabs !== null) {
                    var route ="";
                    if(CallingType === "ShipmentClone"){
                        route = UtilityService.GetCurrentRoute($scope.tab.childTabs, "booking-home.direct-booking-clone");
                    }
                    else if (CallingType === "ShipmentReturn") {
                        route = UtilityService.GetCurrentRoute($scope.tab.childTabs, "booking-home.direct-booking-return");
                    }
                    else if (CallingType === "ShipmentDraft") {
                        route = UtilityService.GetCurrentRoute($scope.tab.childTabs, "booking-home.direct-booking");
                    }
                    $state.go(route, { directShipmentId: row.entity.ShipmentId }, { reload: true });
                }
                 
            }
            else if ($scope.track.BookingMethod === "eCommerce") {
                if ($scope.$UserRoleId === 3) {
                    $state.go('customer.booking-home.eCommerce-booking-clone', { shipmentId: row.entity.ShipmentId }, { reload: true });
                }
            }

        }

    };


    $scope.DeleteDirectShipmentDocuments = function (row) {
        var modalOptions = {
            headerText: "Delete Draft Shipment",
            bodyText: "Are you sure want to remove the draft shipment detail?"
        };

        ModalService.Confirm({}, modalOptions).then(function () {
            if (row !== undefined && row !== null && row.entity !== null) {
                DirectShipmentService.DeleteDirectBooking(row.entity.ShipmentId).then(function (response) {
                    if (response.data !== null && response.data.Status) {
                        toaster.pop({
                            type: 'success',
                            title: $scope.Frayte_Success,
                            body: $scope.Successfully_Delete,
                            showCloseButton: true
                        });
                        $scope.DirectShipments();
                    }
                    else {
                        toaster.pop({
                            type: 'error',
                            title: $scope.Frayte_Error,
                            body: $scope.ErrorDeletingRecord,
                            showCloseButton: true
                        });
                    }

                }, function () {
                    toaster.pop({
                        type: 'error',
                        title: $scope.Frayte_Error,
                        body: $scope.DeletingShipmentErrorValidation,
                        showCloseButton: true
                    });
                });
            }
            else {
                toaster.pop({
                    type: 'error',
                    title: $scope.Frayte_Error,
                    body: $scope.DeletingShipmentErrorValidation,
                    showCloseButton: true
                });
            }


        }, function () {

        });
    };

    $scope.DirectShipmentDetail = function (row) {

        if ($scope.track.BookingMethod === "DirectBooking") {
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
                    ShipmentStatus: function () {
                        return row.entity.Status;
                    }

                }
            });
        }

    };

    $scope.ChangeFromdate = function (FromDate) {

        var newdate = [];

        newdate = new Date(FromDate);
        var gtDate = newdate.getDate();
        var gtDate1 = ++gtDate;
        var gtMonth = newdate.getMonth();
        var month1 = ++gtMonth;
        var gtYear = newdate.getFullYear();
        var nDate = month1 + "/" + gtDate1 + "/" + gtYear;

        $scope.track.FromDate = new Date(nDate);
    };

    $scope.ChangeTodate = function (ToDate) {

        var newdate = [];

        newdate = new Date(ToDate);
        var gtDate = newdate.getDate();
        var gtDate1 = ++gtDate;
        var gtMonth = newdate.getMonth();
        var month1 = ++gtMonth;
        var gtYear = newdate.getFullYear();
        var nDate = month1 + "/" + gtDate1 + "/" + gtYear;

        $scope.track.ToDate = new Date(nDate);
    };

    $scope.openCalender = function ($event) {
        $scope.status.opened = true;
    };

    $scope.openCalender1 = function ($event) {
        $scope.status1.opened = true;
    };

    $scope.status1 = {
        opened: false
    };

    $scope.status = {
        opened: false
    };
    $scope.bookingStatus = function (Status) {
        $scope.CurrentStatus = Status;
        $scope.track.ShipmentStatusId = $scope.CurrentStatus.ShipmentStatusId;
        $scope.DirectShipments();
    };
    function getUserId() {
        var userInfo = SessionService.getUser();
        $scope.track.UserId = userInfo.EmployeeId;
    }

    $scope.LoadShipmentStatus = function () {
        //  getUserId();
        DirectShipmentService.GetDirectShipmentStatus($scope.track.BookingMethod, $scope.track.UserId).then(function (response) {
            $scope.DirectShipmentStatus = response.data;
            ObjAll = {
                BookingType: "DirectBooking",
                DisplayStatusName: "All",
                ShipmentStatusId: 0,
                StatusName: "All"
            };
            $scope.DirectShipmentStatus.unshift(ObjAll);

            for (i = 0; i < response.data.length; i++) {

                if ($scope.DirectShipmentStatus[i].StatusName === "Cancel") {
                    $scope.DirectShipmentStatus.splice(i);
                }
            }

            for (var i = 0 ; i < $scope.DirectShipmentStatus.length; i++) {
                if ($scope.DirectShipmentStatus[i].StatusName === "Current") {
                    $scope.DirectShipmentStatus[i].ImgURL = "currentImg.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "Draft") {
                    $scope.DirectShipmentStatus[i].ImgURL = "draftImg.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "Past") {
                    $scope.DirectShipmentStatus[i].ImgURL = "pastImg.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "Delay") {
                    $scope.DirectShipmentStatus[i].ImgURL = "delayImg.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "Cancel") {
                    $scope.DirectShipmentStatus[i].ImgURL = "cancelImg.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "All") {
                    $scope.DirectShipmentStatus[i].ImgURL = "all.png";
                }
            }

            $scope.CurrentStatus = $scope.DirectShipmentStatus[0];
            $scope.track.ShipmentStatusId = $scope.CurrentStatus.ShipmentStatusId;
            $scope.DirectShipments();
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.Frayte_Error,
                body: $scope.GettingDataErrorValidation,
                showCloseButton: true
            });
        });

    };

    $scope.pageChanged = function (track) {
        $scope.DirectShipments();
    };
    $scope.getDirectShipments = function () {
        $scope.DirectShipments();
    };
    $scope.DirectShipments = function () {
        if ($scope.track.BookingMethod === 'DirectBooking') {
            AppSpinner.showSpinnerTemplate('Loading Track & Trace', $scope.Template);
        }
        else if ($scope.track.BookingMethod === 'eCommerce') {
            AppSpinner.showSpinnerTemplate('Loading Track & Trace', $scope.Template);
        }

        $scope.track.LogisticType = $scope.CurrentLogisticType.LogisticType;
        $scope.track.LogisticServiceType = $scope.CurrentLogisticServiceType.LogisticServiceName;
        DirectShipmentService.GetDirectShipments($scope.track).then(function (response) {
            if (response.data !== null && response.data.length > 0) {
                $scope.totalItemCount = response.data[0].TotalRows;
            }
            else {
                $scope.totalItemCount = 0;
            }
            for (var i = 0 ; i < response.data.length; i++) {

                if (response.data[i].ShippingBy === "UKMail" || response.data[i].ShippingBy === "Yodel" || response.data[i].ShippingBy === "Hermes") {
                    response.data[i].CourierName = "UKEUShipment";
                }
                else {
                    response.data[i].CourierName = response.data[i].ShippingBy;
                }
                if (response.data[i].TrackingNo === "" || response.data[i].TrackingNo === null) {
                    response.data[i].TrackingCode = "123";
                }
                else {
                    response.data[i].TrackingCode = response.data[i].TrackingNo;
                }

                if (response.data[i].RateType == null) {
                    response.data[i].DisplayName = response.data[i].DisplayName + "";
                }
                else {
                    response.data[i].DisplayName = response.data[i].DisplayName + " " + response.data[i].RateType;
                }

            }
            $scope.DirectShipmentList = response.data;
            $scope.gridOptions.data = response.data;
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.Frayte_Error,
                body: $scope.GettingDataErrorValidation,
                showCloseButton: true
            });
        });
    };
    $scope.buildURL = config.BUILD_URL;

    $scope.logisticStatus = function (LogisticType) {
        if (LogisticType !== undefined) {
            if (LogisticType.LogisticType !== "UKShipment") {
                $scope.CurrentLogisticServiceType = $scope.LogisticServiceTypes[0];
            }
            $scope.CurrentLogisticType = LogisticType;
        }
    };

    var getCustomers = function () {
        //  getUserId();
        DirectBookingService.GetDirectBookingCustomers($scope.track.UserId, $scope.track.BookingMethod).then(function (response) {
            $scope.directBookingCustomers = response.data;
            for (i = 0; i < $scope.directBookingCustomers.length; i++) {
                var dbr = $scope.directBookingCustomers[i].AccountNumber.split("");
                var accno = "";
                for (var j = 0; j < dbr.length; j++) {
                    accno = accno + dbr[j];
                    if (j == 2 || j == 5) {
                        accno = accno + "-";
                    }
                }
                $scope.directBookingCustomers[i].AccountNumber = accno;
            }

        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.Frayte_Error,
                body: $scope.ReceiveDetailValidation,
                showCloseButton: true
            });
        });
    };
    $scope.logisticServiceStatus = function (logisticService) {
        if (logisticService !== undefined) {
            $scope.CurrentLogisticServiceType = logisticService;
        }
    };
    $scope.toggleRowSelection = function () {
        $scope.gridApi.selection.clearSelectedRows();
        $scope.gridOptions.enableRowSelection = !$scope.gridOptions.enableRowSelection;
        $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.OPTIONS);
    };


    var setBookingState = function (BookingType) {

        if ($stateParams.moduleType !== undefined && $stateParams.moduleType !== null && $stateParams.moduleType !== '') {
            if ($stateParams.moduleType === "db") {
                $scope.moduleType = "DirectBooking";
            }
            else if ($stateParams.moduleType === "eCb") {
                $scope.moduleType = "eCommerce";
            }
        }
        else {
            if (BookingType && BookingType === 'DirectBooking') {
                $scope.moduleType = "DirectBooking";
            }
            else if (BookingType === 'eCommerce') {
                $scope.moduleType = "eCommerce";
            }
            else {
                $scope.moduleType = "DirectBooking";
            }
        }
    };
    $scope.getDetails = function () {
        $scope.LoadShipmentStatus();
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
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        //$scope.spinnerMessage = 'Loading Track and Trace...';
        setMultilingualOptions();
        $scope.LogisticTypes = [
           {
               LogisticTypeId: 0,
               LogisticType: '',
               LogisticTypeDisplay: 'All'
           },
              {
                  LogisticTypeId: 1,
                  LogisticType: 'Import',
                  LogisticTypeDisplay: 'Import'
              },
        {
            LogisticTypeId: 2,
            LogisticType: 'Export',
            LogisticTypeDisplay: 'Export'
        },
        {
            LogisticTypeId: 3,
            LogisticType: 'ThirdParty',
            LogisticTypeDisplay: '3rd Party'
        },
        {
            LogisticTypeId: 4,
            LogisticType: 'UKShipment',
            LogisticTypeDisplay: 'UK Domestic'
        }
            //,
        //{
        //    LogisticTypeId: 5,
        //    LogisticType: 'EUImport',
        //    LogisticTypeDisplay: 'EU Economy Import'
        //},
        // {
        //     LogisticTypeId: 6,
        //     LogisticType: 'EUExport',
        //     LogisticTypeDisplay: 'EU Economy Export'
        // }
        ];
        $scope.CurrentLogisticType = {
            LogisticTypeId: 0,
            LogisticType: '',
            LogisticTypeDisplay: 'All'
        };
        $scope.LogisticServiceTypes = [
              {
                  LogisticServiceTypeId: 1,
                  LogisticServiceName: '',
                  LogisticServiceNameDisplay: 'All'
              },
    {
        LogisticServiceTypeId: 1,
        LogisticServiceName: 'UKMail',
        LogisticServiceNameDisplay: 'UK Mail'
    },
    {
        LogisticServiceTypeId: 2,
        LogisticServiceName: 'Yodel',
        LogisticServiceNameDisplay: 'Yodel'
    },
    {
        LogisticServiceTypeId: 3,
        LogisticServiceName: 'Hermes',
        LogisticServiceNameDisplay: 'Hermes'
    }
        ];
        $scope.CurrentLogisticServiceType = $scope.LogisticServiceTypes[0];

        $scope.fryateShipmentTypes = [
            {
                value: 'DirectBooking',
                display: 'Direct Booking'
            },
            {
                value: 'eCommerce',
                display: 'eCommerce'
            }
        ];

        // Pagination Logic 
        $scope.viewby = 50;
        $scope.currentPage = 1;
        $scope.itemsPerPage = $scope.viewby;
        $scope.totalItemCount = 1000;

        $scope.maxSize = 2; //Number of pager buttons to show

        var userInfo = SessionService.getUser();
        $scope.tabs = userInfo.tabs;
        $scope.tab = getUserTab($scope.tabs, "QuickDirect_Booking");

        $scope.$UserRoleId = userInfo.RoleId;
        //$scope.customerId = userInfo.EmployeeId;
        $scope.SetGridOptions();
        $scope.gridOptions.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
        };

        $scope.FromDate = null;
        $scope.ToDate = null;
        $scope.DisableShipmentType = false;
        if ($scope.$UserRoleId === 3) {
            CustomerService.GetCustomerModules(userInfo.EmployeeId).then(function (response) {
                if (response.status === 200) {
                    $scope.CustomerModules = response.data;
                    if (!$scope.CustomerModules.IseCommerceBooking && !$scope.CustomerModules.IsTradeLaneBooking && $scope.CustomerModules.IsDirectBooking && !$scope.CustomerModules.IsBreakBulkBooking) {
                        $scope.DisableShipmentType = true;
                        setBookingState("DirectBooking");
                    }
                    else if ($scope.CustomerModules.IsTradeLaneBooking && !$scope.CustomerModules.IsDirectBooking && !$scope.CustomerModules.IsBreakBulkBooking) {
                        $scope.DisableShipmentType = false;
                        setBookingState("DirectBooking");
                    }
                    else if (!$scope.CustomerModules.IsTradeLaneBooking && !$scope.CustomerModules.IsDirectBooking && $scope.CustomerModules.IsBreakBulkBooking) {
                        $scope.DisableShipmentType = false;
                        setBookingState("DirectBooking");
                    }
                    else if ($scope.CustomerModules.IseCommerceBooking && !$scope.CustomerModules.IsTradeLaneBooking && !$scope.CustomerModules.IsDirectBooking && !$scope.CustomerModules.IsBreakBulkBooking) {
                        $scope.DisableShipmentType = true;
                        setBookingState("eCommerce");
                    }
                    else {
                        $scope.DisableShipmentType = false;
                        setBookingState("DirectBooking");
                    }
                    $scope.track = {
                        UserId: userInfo.EmployeeId,
                        DateTime: '',
                        ToDate: '',
                        ShipmentStatusId: 0,
                        CustomerId: 0,
                        FrayteNumber: '',
                        TrackingNo: '',
                        BookingMethod: $scope.moduleType,
                        LogisticType: '',
                        LogisticServiceType: '',
                        CurrentPage: $scope.currentPage,
                        TakeRows: $scope.itemsPerPage
                    };
                    $scope.LoadShipmentStatus();
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.Frayte_Error,
                        body: $scope.ReceiveDetailValidation,
                        showCloseButton: true
                    });
                }

            });
        }
        else {

            setBookingState("DirectBooking");
            $scope.track = {
                UserId: userInfo.EmployeeId,
                DateTime: '',
                ToDate: '',
                ShipmentStatusId: 0,
                CustomerId: 0,
                FrayteNumber: '',
                TrackingNo: '',
                BookingMethod: $scope.moduleType,
                LogisticType: '',
                LogisticServiceType: '',
                CurrentPage: $scope.currentPage,
                TakeRows: $scope.itemsPerPage
            };
            $scope.LoadShipmentStatus();
        }



        if ($scope.$UserRoleId !== 3) {
            getCustomers();
        }

        $scope.gridheight = SessionService.getScreenHeight();

        $rootScope.GetServiceValue = null;
    }

    init();
});