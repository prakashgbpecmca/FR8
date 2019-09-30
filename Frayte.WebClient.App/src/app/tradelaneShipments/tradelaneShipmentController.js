angular.module('ngApp.tradelaneShipments').controller('TradelaneShipmentController', function ($scope, $state, TradelaneMilestoneService, $translate, $uibModal, toaster, ModalService, SessionService, AppSpinner, DirectBookingService, DirectShipmentService, $rootScope, TradelaneShipmentService, TradelaneBookingService, uiGridConstants, config, DateFormatChange, $http, $window ) {

    var setMultilingualOptions = function () {
        $translate(['Success', 'Record_Saved', 'FrayteError_Validation', 'ErrorSavingRecord', 'GettingDetails_Error', 'CancelShipmentError_Validation',
                    'ErrorDeletingRecord', 'DeletingShipmentError_Validation', 'GettingDataError_Validation', 'ReceiveDetail_Validation', 'ShipmentCancelConfirmText',
                    'Confirmation', 'FrayteSuccess', 'FrayteWarning', 'FrayteError', 'Darft_Delete', 'SuccessfullyDelete', 'From_Date_Required',
                    'DeleteDraftShipment', 'SureRemoveDraftShipmentDetail', 'IssueDraftShipmentMakePublic', 'CommercialInvoiceNotAvailable', 'DownloadCommercialInvoiceSuccessfullyCheck',
                    'ReportCannotDownloadPleaseTryAgain', 'Could_Not_Download_TheReport', 'GenerateAndDownloadReportSuccessfullyCheck', 'LoadingTrackTrace', 'CancellingTheShipment',
                    'DownloadingCommercialInvoice', 'DownloadingTrackTraceExcel', 'FrayteInformation', 'FrayteValidation', 'ErrorGettingRecordPleaseTryAgain', 'LoadingTrackTraceDashboard',
                    'To_Date_Validation', 'Share_Draft_As_Public', 'SuccessfullyDeletedDraftShipment', 'SureDeleteShipmentDraft', 'ShipmentDraftDeleteConfirmation', 'To_Date_Required',
                    'Loading_Shipments']).then(function (translations) {
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
                    $scope.From_Date = translations.From_Date_Required;
                    $scope.To_Date = translations.To_Date_Required;
                    $scope.DeleteDraftShipment = translations.DeleteDraftShipment;
                    $scope.SureRemoveDraftShipmentDetail = translations.SureRemoveDraftShipmentDetail;
                    $scope.IssueDraftShipmentMakePublic = translations.IssueDraftShipmentMakePublic;
                    $scope.CommercialInvoiceNotAvailable = translations.CommercialInvoiceNotAvailable;
                    $scope.DownloadCommercialInvoiceSuccessfullyCheck = translations.DownloadCommercialInvoiceSuccessfullyCheck;
                    $scope.ReportCannotDownloadPleaseTryAgain = translations.ReportCannotDownloadPleaseTryAgain;
                    $scope.Could_Not_Download_TheReport = translations.Could_Not_Download_TheReport;
                    $scope.GenerateAndDownloadReportSuccessfullyCheck = translations.GenerateAndDownloadReportSuccessfullyCheck;
                    $scope.LoadingTrackTrace = translations.LoadingTrackTrace;
                    $scope.CancellingTheShipment = translations.CancellingTheShipment;
                    $scope.DownloadingCommercialInvoice = translations.DownloadingCommercialInvoice;
                    $scope.DownloadingTrackTraceExcel = translations.DownloadingTrackTraceExcel;
                    $scope.FrayteInformation = translations.FrayteInformation;
                    $scope.FrayteValidation = translations.FrayteValidation;
                    $scope.ErrorGettingRecordPleaseTryAgain = translations.ErrorGettingRecordPleaseTryAgain;
                    $scope.LoadingTrackTraceDashboard = translations.LoadingTrackTraceDashboard;
                    $scope.ToDateValidation = translations.To_Date_Validation;
                    $scope.ShareDraftAsPublic = translations.Share_Draft_As_Public;
                    $scope.ShipmentDraftDeleteConfirmation = translations.ShipmentDraftDeleteConfirmation;
                    $scope.SureDeleteShipmentDraft = translations.SureDeleteShipmentDraft;
                    $scope.SuccessfullyDeletedDraftShipment = translations.SuccessfullyDeletedDraftShipment;
                    $scope.Loading_Shipments = translations.Loading_Shipments;

                    // Initial Deatil call here --> Spinner Message should be multilingual
                    if ($scope.$UserRoleId === 3) {
                        $scope.ShowServiceOptionPanel = true;
                        $scope.IsDashboardShow = true;
                        $scope.LoadShipmentStatus();
                        //getInitialDetails();
                    }
                    else if ($scope.$UserRoleId === 1 || $scope.$UserRoleId === 6) {
                        $scope.ShowServiceOptionPanel = true;
                        $scope.IsDashboardShow = true;
                        $scope.LoadShipmentStatus();
                    }
                    if ($rootScope.ShipmentShowType != null && $rootScope.ShipmentShowType === "PreAlert") {

                        $scope.track.SpecialSearchId = $scope.SpecialSearchList[6].SpecialSearchId;
                        $scope.Speacialsearch = $scope.SpecialSearchList[6];
                        $rootScope.ShipmentShowType = null;
                        $scope.DirectShipments();
                    }
                    else if ($rootScope.ShipmentShowType != null && $rootScope.ShipmentShowType === "UnallocatedShipment") {
                        $scope.track.SpecialSearchId = $scope.SpecialSearchList[7].SpecialSearchId;
                        $scope.Speacialsearch = $scope.SpecialSearchList[7];
                        $rootScope.ShipmentShowType = null;
                        $scope.DirectShipments();
                    }
                    else if ($rootScope.ShipmentShowType != null && $rootScope.ShipmentShowType === "MilestoneNotCompleted") {
                        $scope.track.SpecialSearchId = $scope.SpecialSearchList[5].SpecialSearchId;
                        $scope.Speacialsearch = $scope.SpecialSearchList[5];
                        $rootScope.ShipmentShowType = null;
                        $scope.DirectShipments();
                    }
                });
    };

    //Special Search Description 
    $scope.specialSearchDescription = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'tradelaneShipments/specialSearchDescription.tpl.html',
            //controller: 'PreviewMAWBController',
            keyboard: true,
            windowClass: '',
            size: 'lg',
            backdrop: 'static'
        });
    };

    //hawb label code
    $scope.hawbLabel = function (TradelaneShipmentId) {
        ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'tradelaneShipments/tradelaneShipmentsHawbLabel/tradelaneShipmentsHawbLabel.tpl.html',
            controller: 'TradelaneShipmentsHawbLabelController',
            keyboard: true,
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                TradelaneShipmentId: function () {
                    return TradelaneShipmentId;
                }
            }
        });
    };
    //end

    //download destination manifest
    $scope.destinationManifest = function (TradelaneShipmentId) {
        TradelaneBookingService.CreateDestinationManifest(TradelaneShipmentId, $scope.loginUserId).then(function (response) {
                if (response.data !== null) {
                    var fileName = {
                        FileName: response.data.FileName,
                        FilePath: response.data.FilePath
                    };

                    $http({
                        method: 'POST',
                        url: config.SERVICE_URL + '/TradelaneBooking/DownloadDestinationManifest',
                        params: { 'fileName': fileName.FileName, 'TradelaneShipmentId': TradelaneShipmentId },
                        responseType: 'arraybuffer'
                    }).success(function (data, status, headers) {
                        if (status == 200 && data !== null) {
                            headers = headers();
                            var filename = headers['x-filename'];
                            var contentType = headers['content-type'];

                            var linkElement = document.createElement('a');
                            try {
                                if (navigator.userAgent.search("Safari") >= 0 && navigator.userAgent.search("Chrome") < 0) {
                                    alert("Browser is Safari");
                                }
                                else {
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
                                        body: $scope.GenerateAndDownloadReportSuccessfullyCheck,
                                        showCloseButton: true
                                    });
                                }

                            } catch (ex) {
                                $window.open(fileName.FilePath, "_blank");
                                console.log(ex);
                            }
                        }
                    })
                        .error(function (data) {
                            console.log(data);
                            toaster.pop({
                                type: 'error',
                                title: $scope.Frayte_Error,
                                body: $scope.Could_Not_Download_TheReport,
                                showCloseButton: true
                            });
                        });
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.Frayte_Error,
                        body: $scope.ReportCannotDownloadPleaseTryAgain,
                        showCloseButton: true
                    });
                }

            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.Frayte_Error,
                    body: $scope.ReportCannotDownloadPleaseTryAgain,
                    showCloseButton: true
                });
            });

    };
    //end

    //update customized mawb
    $scope.updateCustomizedMawb = function (shipment) {
        ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'tradelaneShipments/tradelaneShipmentsUpdateCustomizedMawb/tradelaneShipmentsUpdateCustomizedMawb.tpl.html',
            controller: 'TradelaneShipmentsUpdateCustomizedMawbController',
            keyboard: true,
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                ShipmentId: function () {
                    return shipment.TradelaneShipmentId;
                }
            }
        });
    };

    //Preview MAWB 
    $scope.previewMAWB = function (shipment) {
        ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'previewMAWB/mawb.tpl.html',
            controller: 'PreviewMAWBController',
            keyboard: true,
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                ShipmentId: function () {
                    return shipment.TradelaneShipmentId;
                }
            }
        });
    };
    //end code

    //function for update tracking code
    $scope.updateTrackingPopup = function (shipment, PopupType) {
        ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'tradelaneShipments/tradelaneUpdateTracking/tradelaneUpdateTracking.tpl.html',
            controller: 'TradelaneUpdateTrackingController',
            keyboard: true,
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                ShipmentInfo: function () {
                    return shipment;
                },
                PopupType: function () {
                    return PopupType;
                }
            }
        });
    };
    //end code

    $scope.CreateShipment = function (row) {
        if (row !== undefined && row !== null) {
            // ToDo: Need to check who is editing the shipment
            if (row.Status === "Draft") {
                //route = UtilityService.GetCurrentRoute($scope.tab.childTabs, "loginView.userTabs.tradelane-booking");
                $state.go("loginView.userTabs.tradelane-booking", { shipmentId: row.TradelaneShipmentId }, { reload: true });
            }
        }
    };

    $scope.tradelaneClaim = function (shipment) {
        ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'tradelaneShipments/tradelaneShipmentsClaim/tradelaneShipmentClaim.tpl.html',
            controller: 'TradelaneClaimController',
            keyboard: true,
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                ShipmentInfo: function () {
                    return shipment;
                }
            }
        });
    };

    $scope.tradelaneClaimResolved = function (shipment) {
        ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'tradelaneShipments/tradelaneShipmentsClaimResolved/tradelaneShipmentClaimResolved.tpl.html',
            controller: 'TradelaneClaimResolvedController',
            keyboard: true,
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                ShipmentInfo: function () {
                    return shipment;
                }
            }
        });
    };

    $scope.bookingStatus = function (Status) {
        $scope.CurrentStatus = Status;
        $scope.track.ShipmentStatusId = $scope.CurrentStatus.ShipmentStatusId;
        if ($scope.track.CurrentPage === 1) {

        }
        else {
            $scope.track.CurrentPage = 1;
        }
        $scope.DirectShipments();
    };

    $scope.ChangeSpecialSearch = function (SpecialSearch) {
        //$scope.CurrentStatus = Status;
        $scope.track.SpecialSearchId = SpecialSearch.SpecialSearchId;
        $scope.DirectShipments();
    };

    $scope.SeachDirectShipments = function () {
        $scope.DirectShipments();
    };

    $scope.LoadShipmentStatus = function () {
        //  getUserId();
        if ($scope.track.UserId != null && $scope.track.UserId !== undefined && $scope.track.UserId !== 0) {
            $scope.UserID = $scope.track.UserId;
        }
        else if ($scope.track.CustomerId !== null && $scope.track.CustomerId !== undefined && $scope.track.CustomerId !== 0) {
            $scope.UserID = $scope.track.CustomerId;
        }

        TradelaneShipmentService.GetShipmentStatus("Tradelane").then(function (response) {
            $scope.DirectShipmentStatus = response.data;
            ObjAll = {
                BookingType: "Tradelane",
                DisplayStatusName: "All",
                ShipmentStatusId: 0,
                StatusName: "All"
            };
            $scope.DirectShipmentStatus.unshift(ObjAll);
            $scope.DirectShipmentStatus.sort(function (a, b) {
                if (a.StatusName < b.StatusName) { return -1; }
                if (a.StatusName > b.StatusName) { return 1; }
                return 0;
            });
            var DSS;
            for (var i = 0 ; i < $scope.DirectShipmentStatus.length; i++) {
                if ($scope.DirectShipmentStatus[i].StatusName === "Draft") {
                    $scope.DirectShipmentStatus[i].ImgURL = "draftImg.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "All") {
                    $scope.DirectShipmentStatus[i].ImgURL = "all.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "ShipmentBooked") {
                    $scope.DirectShipmentStatus[i].ImgURL = "currentImg.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "InTransit") {
                    $scope.DirectShipmentStatus[i].ImgURL = "T-intransit.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "Delivered") {
                    $scope.DirectShipmentStatus[i].ImgURL = "T-delivered.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "Pending") {
                    $scope.DirectShipmentStatus[i].ImgURL = "D-pending.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "Departed") {
                    $scope.DirectShipmentStatus[i].ImgURL = "D-departed.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "Rejected") {
                    $scope.DirectShipmentStatus[i].ImgURL = "D-rejected.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "Arrived") {
                    $scope.DirectShipmentStatus[i].ImgURL = "D-arrived.png";
                }
            }

            $scope.CurrentStatus = $scope.DirectShipmentStatus[0];
            $scope.track.ShipmentStatusId = $scope.CurrentStatus.ShipmentStatusId;
            $scope.DirectShipments();
        }, function (response) {
            if (response.status !== 401) {
                toaster.pop({
                    type: 'error',
                    title: $scope.Frayte_Error,
                    body: $scope.GettingDataErrorValidation,
                    showCloseButton: true
                });
            }
        });
    };

    $scope.DeleteDraft = function (RowDetail) {
        var modalOptions = {
            headerText: $scope.ShipmentDraftDeleteConfirmation,
            bodyText: $scope.SureDeleteShipmentDraft
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            TradelaneShipmentService.DeleteTradelaneShipment(RowDetail.TradelaneShipmentId).then(function (response) {
                if (response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.Frayte_Success,
                        body: $scope.SuccessfullyDeletedDraftShipment,
                        showCloseButton: true
                    });
                    $scope.DirectShipments();
                }

            }, function (response) {
                toaster.pop({
                    type: 'error',
                    title: $scope.Frayte_Error,
                    body: $scope.GettingDataErrorValidation,
                    showCloseButton: true
                });
            });
        });
    };

    $scope.DirectShipments = function () {

        if (($scope.track.ToDate === '' || $scope.track.ToDate === null || $scope.track.ToDate === undefined) && ($scope.track.FromDate === '' || $scope.track.FromDate === null || $scope.track.FromDate === undefined)) {

        }
        else if (($scope.track.ToDate !== null || $scope.track.ToDate !== '' || $scope.track.ToDate !== undefined) && ($scope.track.FromDate === undefined || $scope.track.FromDate === '' || $scope.track.FromDate === null)) {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.From_Date,
                showCloseButton: true
            });
            return;
        }
        else if (($scope.track.ToDate === null || $scope.track.ToDate === '' || $scope.track.ToDate === undefined) && ($scope.track.FromDate !== undefined || $scope.track.FromDate !== '' || $scope.track.FromDate !== null)) {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.To_Date,
                showCloseButton: true
            });
            return;
        }
        else {
            if (($scope.track.FromDate !== undefined || $scope.track.FromDate !== null || $scope.track.FromDate !== "") && ($scope.track.ToDate !== undefined || $scope.track.ToDate !== null || $scope.track.ToDate !== "")) {
                var fromDate = new Date($scope.track.FromDate);
                var toDate = new Date($scope.track.ToDate);
                var from = (fromDate.getDate().toString().length === 1 ? "0" + fromDate.getDate() : fromDate.getDate()) + '/' + ((fromDate.getMonth() + 1).toString().length === 1 ? "0" + (fromDate.getMonth() + 1) : (fromDate.getMonth() + 1)) + '/' + fromDate.getFullYear();
                var to = (toDate.getDate().toString().length === 1 ? "0" + toDate.getDate() : toDate.getDate()) + '/' + ((toDate.getMonth() + 1).toString().length === 1 ? "0" + (toDate.getMonth() + 1) : (toDate.getMonth() + 1)) + '/' + toDate.getFullYear();
                var dateOne = new Date(from);
                var dateTwo = new Date(to);
                if (new Date(dateOne) > new Date(dateTwo)) {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.Frayte_Warning,
                        body: $scope.ToDateValidation,
                        showCloseButton: true
                    });
                    return;
                }
            }
        }

        AppSpinner.showSpinnerTemplate($scope.Loading_Shipments, $scope.Template);
        TradelaneShipmentService.GetTradelaneShipments($scope.track).then(function (response) {
            if (response.data !== null && response.data.length > 0) {
                $scope.totalItemCount = response.data[0].TotalRows;
            }
            else {
                $scope.totalItemCount = 0;
            }
            for (i = 0; i < response.data.length; i++) {
                response.data[i].CreatedOnNxtSevenDay = new Date(new Date(response.data[i].CreatedOn).setDate(new Date(response.data[i].CreatedOn).getDate() + 7));
                response.data[i].IsClaim = false;
                if (new Date() <= response.data[i].CreatedOnNxtSevenDay) {
                    response.data[i].IsClaim = true;
                }
                response.data[i].CreatedOn = DateFormatChange.DateFormatChange(response.data[i].CreatedOn);
            }
            $scope.DirectShipmentList = response.data;
            $scope.gridOptions = response.data;
            AppSpinner.hideSpinnerTemplate();
        }, function (error) {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.Frayte_Error,
                body: $scope.GettingDataErrorValidation,
                showCloseButton: true
            });
        });
    };

    $scope.CheckMawb = function (mawb) {
        if (mawb !== null && mawb !== "") {
            $scope.track.MAWB = mawb.replace(/[^0-9- ]/, '');
        }
    };

    var statusTypeTemplate = '<div class="ui-grid-cell-contents flex"><img ng-src="{{grid.appScope.buildURL}}{{grid.appScope.currentImgURL(row)}}" style="margin:-3px 3px 0px;position:relative;right:4px;">{{grid.appScope.GetStatus(row)}}</div>';
    $scope.currentImgURL = function (row) {
        if (row !== undefined) {
            var url = '';
            for (var i = 0; i < $scope.DirectShipmentStatus.length; i++) {
                if (row.Status == $scope.DirectShipmentStatus[i].StatusName) {
                    url = $scope.DirectShipmentStatus[i].ImgURL;
                    break;
                }
            }
            return url;
        }
    };

    $scope.GetStatus = function (row) {
        if (row !== undefined) {
            return row.entity.StatusDisplay;
        }
    };

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
                enableGridMenu: false,
                enableColumnMenus: false,
                enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
                enableVerticalScrollbar: true,
                columnDefs: [
                      { name: 'StatusDisplay', displayName: 'Status', width: '10%', headerCellFilter: 'translate', cellTemplate: statusTypeTemplate, enableFiltering: false, enableSorting: false },
                      { name: 'Customer', headerCellFilter: 'translate', width: '10%' },
                      { name: 'MAWB', displayName: 'MAWB', headerCellFilter: 'translate', width: '12%' },
                      { name: 'FrayteRefNo', displayName: 'Frayte_ShipmentNo#', headerCellFilter: 'translate', width: '16%' },
                      //{ name: 'ShipmentId', displayName: 'Frayte_ShipmentNo', headerCellFilter: 'translate', width: '15%' },
                      { name: 'ShipperCompanyName', displayName: 'From_Shipper', headerCellFilter: 'translate', width: '15%' },
                      { name: 'ConsigneeCompanyName', displayName: 'To_Consignee', headerCellFilter: 'translate', width: '15%' },
                       { name: 'CreatedOn', displayName: 'Created_On', cellFilter: 'date:"dd-MMM-yyyy"', width: '14%' },
                      { name: 'Edit', displayName: '', enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "tradelaneShipments/tradelaneShipmentEditButton.tpl.html" }

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
                enableGridMenu: false,
                enableColumnMenus: false,
                enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
                //enableVerticalScrollbar: uiGridConstants.scrollbars.NEVER,
                enableVerticalScrollbar: true,

                //     rowTemplate: '<div ng-class="{ \'demoClass\':grid.appScope.rowFormatter( row ) }">' + '  <div ng-repeat="(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ng-class="{ \'ui-grid-row-header-cell\': col.isRowHeader }"  ui-grid-cell></div>' + '</div>',
                columnDefs: [
                      { name: 'StatusDisplay', displayName: 'Status', width: '12%', enableFiltering: false, cellTemplate: statusTypeTemplate, enableSorting: false, headerCellFilter: 'translate' },

                      //{ name: 'Customer', displayName: 'Ship By', width: '15%' },
                      { name: 'MAWB', displayName: 'MAWB', width: '13%', headerCellFilter: 'translate' },
                      { name: 'FrayteRefNo', displayName: 'Frayte_ShipmentNo#', width: '13%', headerCellFilter: 'translate' },
                      //{ name: 'ShipmentId', displayName: 'Frayte Shipment No #', width: '10%' },
                      { name: 'ShipperCompanyName', displayName: 'From_Shipper', width: '19%', headerCellFilter: 'translate' },
                      { name: 'ConsigneeCompanyName', displayName: 'To_Consignee', headerCellFilter: 'translate', width: '17%' },
                      { name: 'CreatedOn', displayName: 'Created_On', cellFilter: 'date:"dd-MMM-yyyy"', width: '14%' },
                      { name: 'Edit', displayName: '', enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "tradelaneShipments/tradelaneShipmentEditButton.tpl.html" }
                ]
            };
        }
    };

    $scope.GetCustomerName = function (Customer) {
        //$scope.Customer = Customer;
        if (Customer !== null && Customer !== undefined) {
            $scope.track.CustomerId = Customer.CustomerId;
            $scope.track.CustomerName = Customer.CustomerName;
        }
        else {
            $scope.track.CustomerId = 0;
            $scope.track.CustomerName = "ALL";
        }
    };

    $scope.MAWBAllocationPopup = function (shipment) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'tradelaneShipments/tradelaneShipmentsMawbAllocation/tradelaneShipmentMawbAllocation.tpl.html',
            controller: 'TradelaneMAWBAllocationController',
            windowClass: 'directBookingDetail',
            size: 'lg',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                ShipmentId: function () {
                    return shipment.TradelaneShipmentId;
                }
            }
        });
        modalInstance.result.then(function (response) {
            $scope.DirectShipments();
        }, function () {

        });
    };

    $scope.MAWBCorrectionPopup = function (shipment) {
        ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'tradelaneShipments/tradelaneShipmentsMawbCorrection/tradelaneShipmentMawbCorrection.tpl.html',
            controller: 'TradelaneMAWBCorrectionController',
            keyboard: true,
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                ShipmentInfo: function () {
                    return shipment;
                }
            }
        });
    };

    $scope.previewHAWB = function (shipment) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'tradelaneBooking/tradelaneBookingDetail/tradelaneBookingDetailPackageDetail.tpl.html',
            controller: 'TradelaneBookingDetailPacakgeDetailController',
            windowClass: 'directBookingDetail',
            size: 'lg',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                ShipmentId: function () {
                    return shipment.TradelaneShipmentId;
                },
                PackageCalculatonType: function () {
                    return "";
                },
                FrayteNumber: function () {
                    return "";
                },
                HAWB: function () {
                    return "";
                },
                HAWBNumber: function () {
                    return "";
                },
                TotalUploaded: function () {
                    return 0;
                },
                SuccessUploaded: function () {
                    return 0;
                },
                ScreenType: function () {
                    return "PreviewHAWB";
                }
            }
        });
        modalInstance.result.then(function (response) {

        }, function () {

        });
    };

    var getCustomers = function () {
        //  getUserId();
        TradelaneShipmentService.GetTradelaneCustomers().then(function (response) {
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

    $scope.toggleMin = function () {
        $scope.dateOptions.minDate = $scope.dateOptions.minDate ? null : new Date();
    };

    $scope.toggleMin1 = function () {
        $scope.dateOptions1.minDate = $scope.dateOptions1.minDate ? null : new Date();
    };

    $scope.ChangeFromdate = function (FromDate) {
        $scope.track.FromDate = $scope.SetTimeinDateObj(FromDate);
        if ($scope.track.ToDate === undefined || $scope.track.ToDate === '' || $scope.track.ToDate === null) {
            $scope.track.ToDate = new Date();
            $scope.track.ToDate1 = new Date();
        }
    };

    $scope.ChangeTodate = function (ToDate) {
        $scope.track.ToDate = $scope.SetTimeinDateObj(ToDate);
    };

    $scope.pageChanged = function (track) {
        $scope.DirectShipments();
    };

    $scope.SetTimeinDateObj = function (DateValue) {
        var newdate1 = new Date();
        newdate = new Date(DateValue);
        var gtDate = newdate.getDate();
        var gtMonth = newdate.getMonth();
        var gtYear = newdate.getFullYear();
        var hour = newdate1.getHours();
        var min = newdate1.getMinutes();
        var Sec = newdate1.getSeconds();
        var MilSec = new Date().getMilliseconds();
        return new Date(gtYear, gtMonth, gtDate, hour, min, Sec, MilSec);
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

    $scope.options = {
        maxDate: new Date(),
        showWeeks: true
    };

    $scope.status = {
        opened: false
    };


    $scope.preAlertEmail = function (shipment) {

        var modalInstance1 = $uibModal.open({
            animation: true,
            templateUrl: 'preAlertEmail/preAlertEmail.tpl.html',
            controller: 'PreAlertEmailController',
            windowClass: 'directBookingDetail',
            size: 'lg',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                ShipmentId: function () {
                    return shipment.TradelaneShipmentId;
                }
            }
        });
        modalInstance1.result.then(function (response) {
        }, function () {
            
        });
    };

    // Grid Actions 
    $scope.cloneShipment = function (shipment) {
        if (shipment) {
            $state.go("loginView.userTabs.tradelane-booking-clone", { shipmentId: shipment.TradelaneShipmentId });
        }
    };

    $scope.shipmentDetail = function (shipment) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'tradelaneBooking/tradelaneBookingDetail/tradelaneBookingDetail.tpl.html',
            controller: 'TradelaneBookingDetailController',
            windowClass: 'directBookingDetail',
            size: 'lg',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                ShipmentId: function () {
                    return shipment.TradelaneShipmentId;
                },
                    ModuleType: function () {
                        return "TradelaneBooking";
                    }
            }
        });
        modalInstance.result.then(function (response) {
        }, function () {
        });
    };

    $scope.$watch('track.ToDate', function () {
        if ($scope.track.ToDate !== undefined && $scope.track.ToDate !== null && $scope.track.ToDate !== "" && $scope.track.ToDate.getFullYear() === 1970) {
            $scope.track.ToDate = null;
        }

    });
    $scope.$watch('track.FromDate', function () {
        if ($scope.track.FromDate !== undefined && $scope.track.FromDate !== null && $scope.track.FromDate !== "" && $scope.track.FromDate.getFullYear() === 1970) {
            $scope.track.FromDate = null;
        }

    });

    function init() {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.dateOptions = {
            formatYear: 'yy',
            minDate: new Date(),
            maxDate: new Date(),
            dateDisabled: function (data) {
                var date = data.date,
                mode = data.mode;
                if ($scope.track.ToDate !== undefined && $scope.track.ToDate !== "" && $scope.track.ToDate !== null && $scope.track.ToDate.getDate() === date.getDate()) {
                    return mode === 'day' && false;
                }
                return mode === 'day' && (($scope.track.ToDate !== undefined && $scope.track.ToDate !== null && $scope.track.ToDate !== "") && date > $scope.track.ToDate);
            }
        };

        $scope.dateOptions1 = {
            formatYear: 'yy',
            minDate: new Date(),
            maxDate: new Date(),
            dateDisabled: function (data) {
                var date = data.date,
                mode = data.mode;
                if ($scope.track.FromDate !== undefined && $scope.track.FromDate !== "" && $scope.track.FromDate !== null && $scope.track.FromDate.getDate() === date.getDate()) {
                    return mode === 'day' && false;
                }
                return mode === 'day' && (date < $scope.track.FromDate);
            }
        };

        var tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        var afterTomorrow = new Date(tomorrow);
        afterTomorrow.setDate(tomorrow.getDate() + 1);
        $scope.toggleMin();
        $scope.toggleMin1();
        $scope.events = [
          {
              date: tomorrow,
              status: 'full'
          },
          {
              date: afterTomorrow,
              status: 'partially'
          }
        ];

        $scope.SpecialSearchList = [
             {
                 SpecialSearchId: 0,
                 SpecialSearch: 'All',
                 SpecialSearchDisplay: 'All'
             },
            {
                SpecialSearchId: 4,
                SpecialSearch: 'AllocatedShipments',
                SpecialSearchDisplay: 'Allocated Shipments'
            },
             {
                 SpecialSearchId: 5,
                 SpecialSearch: 'ClaimShipments',
                 SpecialSearchDisplay: 'Claim Shipments'
             },
               {
                   SpecialSearchId: 7,
                   SpecialSearch: 'Mawb Pending Shipments',
                   SpecialSearchDisplay: 'Mawb Pending Shipments'
               },
            {
                SpecialSearchId: 6,
                SpecialSearch: 'MawbShipments',
                SpecialSearchDisplay: 'Mawb Shipments'
            },
            {
                SpecialSearchId: 3,
                SpecialSearch: 'MilestoneNotCompleted',
                SpecialSearchDisplay: 'Milestone Not Completed'
            },
            {
                SpecialSearchId: 2,
                SpecialSearch: 'ReadyforPreAlert',
                SpecialSearchDisplay: 'Ready for Pre Alert'
            },
            {
                SpecialSearchId: 1,
                SpecialSearch: 'UnallocatedShipments',
                SpecialSearchDisplay: 'Unallocated Shipments'
            }

        ];
        $scope.Speacialsearch = $scope.SpecialSearchList[0];
        // Pagination Logic 
        $scope.viewby = 50;
        $scope.currentPage = 1;
        $scope.itemsPerPage = $scope.viewby;
        $scope.totalItemCount = 1000;

        $scope.maxSize = 2; //Number of pager buttons to show

        var userInfo = SessionService.getUser();
        if (userInfo) {
            setMultilingualOptions();
        }
        else {
            $state.go("login");
        }

        $scope.$UserRoleId = userInfo.RoleId;
        $scope.StaffRoleId = userInfo.RoleId;

        $scope.loginUserId = userInfo.EmployeeId;

        $scope.SetGridOptions();

        $scope.gridOptions.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
        };

        $scope.FromDate = null;
        $scope.ToDate = null;

        //setBookingState("DirectBooking");

        $scope.track = {
            UserId: userInfo.EmployeeId,
            DateTime: '',
            FromDate: '',
            ToDate: '',
            ShipmentStatusId: 0,
            CustomerId: 0,
            MAWB: '',
            FrayteNumber: '',
            CurrentPage: $scope.currentPage,
            TakeRows: $scope.itemsPerPage
        };

        $scope.pageChangedObj = [{
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
        $scope.track.TakeRows = $scope.pageChangedObj[1].pageChangedValue;
        if ($scope.$UserRoleId !== 3) {
            getCustomers();
        }

        $scope.LoadShipmentStatus();
        $scope.gridheight = SessionService.getScreenHeight();
        $scope.buildURL = config.BUILD_URL;
        $rootScope.GetServiceValue = null;
        $scope.regexMawb = '^[a-zA-Z0-9-]+$';
    }

    init();
});