angular.module('ngApp.directBooking').controller('DirectShipmentController', function (AppSpinner, UtilityService, DateFormatChange, $http, $window, $scope, ModalService, uiGridConstants, DirectShipmentService, config, DirectBookingService, $rootScope, $translate, $location, Upload, $state, $filter, ShipmentService, CountryService, CourierService, HomeService, ShipperService, ReceiverService, SessionService, $uibModal, $log, toaster, $stateParams, CustomerService, UserService) {

    $scope.GetCustomerName = function (Customer) {
        //$scope.Customer = Customer;
        if (Customer) {

            $scope.Id = Customer.CustomerId;

            $scope.track.CustomerId = Customer.CustomerId;
            $scope.track.CustomerName = Customer.CustomerName;
        }
        else {
            $scope.track.CustomerId = 0;
            $scope.track.CustomerName = "ALL";
        }

        if ($scope.userInfo.RoleId === 3) {

            getInitialDetails();
        }
    };

    $scope.DownLoadDirectBookingReport = function () {
        if ($scope.track.CustomerName === null || $scope.track.CustomerName === undefined || $scope.track.CustomerName === "") {
            $scope.track.CustomerName = "ALL";
        }
        if ($scope.CurrentStatus.DisplayStatusName === 'All') {
            $scope.track.ShipmentStatusId = 0;
        }
        else {
            $scope.track.ShipmentStatusId = $scope.CurrentStatus.ShipmentStatusId;
        }
        AppSpinner.showSpinnerTemplate($scope.DownloadingTrackTraceExcel, $scope.Template);
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
                       body: $scope.Could_Not_Download_TheReport,
                       showCloseButton: true
                   });
               });
            }
            else {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.Frayte_Error,
                    body: $scope.ReportCannotDownloadPleaseTryAgain,
                    showCloseButton: true
                });
            }

        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.Frayte_Error,
                body: $scope.ReportCannotDownloadPleaseTryAgain,
                showCloseButton: true
            });
        });
    };

    $scope.DownLoadCommercialInvoice = function (row) {
        AppSpinner.showSpinnerTemplate($scope.DownloadingCommercialInvoice, $scope.Template);
        $scope.track.LogisticType = $scope.CurrentLogisticType.LogisticType;
        $scope.track.LogisticServiceType = $scope.CurrentLogisticServiceType.LogisticServiceName;
        DirectShipmentService.GenerateCommercialInvoice(row.entity.ShipmentId, row.entity.Customer).then(function (response) {

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
                                body: $scope.DownloadCommercialInvoiceSuccessfullyCheck,
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
                       body: $scope.CommercialInvoiceNotAvailable,
                       showCloseButton: true
                   });
               });

            }
            else {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'warning',
                    title: $scope.Frayte_Warning,
                    body: $scope.CommercialInvoiceNotAvailable,
                    showCloseButton: true
                });
            }

        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.CommercialInvoiceNotAvailable,
                showCloseButton: true
            });
        });
    };

    $scope.SeachDirectShipments = function () {
        $scope.DirectShipments();
    };

    var statusTypeTemplate = '<div class="ui-grid-cell-contents flex"><img ng-src="{{grid.appScope.buildURL}}{{grid.appScope.currentImgURL(row)}}" style="margin:-3px 3px 0px;position:relative;right:4px;">{{grid.appScope.GetStatus(row)}}</div>';

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

    var trackingTemplate = '';

    $scope.SetGridOptions = function () {
        if ($scope.$UserRoleId === 1) {
            $scope.gridOptions = {
                enableSorting: true,
                multiSelect: false,
                enableFiltering: false,
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
                      { name: 'DisplayStatus', displayName: 'Status', width: '16%', headerCellFilter: 'translate', enableFiltering: false, enableSorting: false, cellTemplate: statusTypeTemplate },
                      //{ name: 'FrayteNumber', displayName: 'Frayte_ShipmentNo', headerCellFilter: 'translate', cellTemplate: detailTemplate, width: '14%' },
                      { name: 'DisplayName', displayName: 'Courier', headerCellFilter: 'translate', width: '14%' },
                      { name: 'TrackingNo', displayName: 'Tracking_No', headerCellFilter: 'translate', cellTemplate: trackingTemplate, width: '14%' },
                      { name: 'Customer', headerCellFilter: 'translate', width: '16%' },
                      { name: 'ShippedToCompany', displayName: 'To_Consignee', headerCellFilter: 'translate', width: '16%' },
                      { name: 'ShippingDate', displayName: 'Created_On', headerCellFilter: 'translate', cellFilter: 'date : \'dd-MMM-yyyy\'', cellTemplate: detailTemplate, width: '14%' },
                      { name: 'Edit', displayName: '', enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "directBooking/directShipments/directShipmentEditButton.tpl.html" }
                ]
            };
        }
        else {
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
                enableVerticalScrollbar: true,
                columnDefs: [
                      { name: 'DisplayStatus', displayName: 'Status', width: '16%', enableFiltering: false, enableSorting: false, headerCellFilter: 'translate', cellTemplate: statusTypeTemplate },
                      //{ name: 'FrayteNumber', displayName: 'Frayte_ShipmentNo', width: '12%', cellTemplate: detailTemplate, headerCellFilter: 'translate' },
                      { name: 'DisplayName', displayName: 'Courier', width: '14%', headerCellFilter: 'translate' },
                      { name: 'TrackingNo', displayName: 'Tracking_No', cellTemplate: trackingTemplate, width: '14%', headerCellFilter: 'translate' },
                      { name: 'ShippedFromCompany', displayName: 'From_Shipper', width: '17%', headerCellFilter: 'translate' },
                      { name: 'ShippedToCompany', displayName: 'To_Consignee', headerCellFilter: 'translate', width: '16%' },
                      { name: 'ShippingDate', displayName: 'Created_On', headerCellFilter: 'translate', cellFilter: 'date : \'dd-MMM-yyyy\'', cellTemplate: detailTemplate, width: '14%' },
                      { name: 'Edit', displayName: '', enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "directBooking/directShipments/directShipmentEditButton.tpl.html" }
                ]
            };
        }
    };

    $scope.showCords = function (event) {
        $scope.clientY = event.clientY;
        $scope.clientX = event.clientX;
        console.log("X coords: " + $scope.clientX + ", Y coords: " + $scope.clientY);
    };

    var setMultilingualOptions = function () {
        $translate(['Success', 'Record_Saved', 'FrayteError_Validation', 'ErrorSavingRecord', 'GettingDetails_Error', 'CancelShipmentError_Validation',
                    'ErrorDeletingRecord', 'DeletingShipmentError_Validation', 'GettingDataError_Validation', 'ReceiveDetail_Validation', 'ShipmentCancelConfirmText',
                    'Confirmation', 'FrayteSuccess', 'FrayteWarning', 'FrayteError', 'Darft_Delete', 'SuccessfullyDelete', 'From_Date_Required', 'To_Date_Required',
                    'DeleteDraftShipment', 'SureRemoveDraftShipmentDetail', 'IssueDraftShipmentMakePublic', 'CommercialInvoiceNotAvailable', 'DownloadCommercialInvoiceSuccessfullyCheck',
                    'ReportCannotDownloadPleaseTryAgain', 'Could_Not_Download_TheReport', 'GenerateAndDownloadReportSuccessfullyCheck', 'LoadingTrackTrace', 'CancellingTheShipment',
                    'DownloadingCommercialInvoice', 'DownloadingTrackTraceExcel', 'FrayteInformation', 'FrayteValidation', 'ErrorGettingRecordPleaseTryAgain', 'LoadingTrackTraceDashboard',
                    'To_Date_Validation', 'Share_Draft_As_Public', 'ShipmentRemoveConfirmText', 'ShipmentRemoveSuccessfully', 'ShipmentRemoveError_Validation', 'DeletingTheShipment']).then(function (translations) {
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
                    $scope.ShipmentRemove_ConfirmText = translations.ShipmentRemoveConfirmText;
                    $scope.ShipmentRemoveSuccessfully = translations.ShipmentRemoveSuccessfully;
                    $scope.ShipmentRemoveErrorValidation = translations.ShipmentRemoveError_Validation;
                    $scope.DeletingTheShipment = translations.DeletingTheShipment;

                    // Initial Deatil call here --> Spinner Message should be multilingual
                    if ($scope.$UserRoleId === 3 || $scope.$UserRoleId === 17) {
                        $scope.ShowServiceOptionPanel = true;
                        $scope.IsDashboardShow = true;
                        getInitialDetails();
                    }
                    else if ($scope.$UserRoleId === 1 || $scope.$UserRoleId === 6 || $scope.$UserRoleId === 20) {
                        $scope.ShowServiceOptionPanel = true;
                        $scope.IsDashboardShow = true;
                        $scope.LoadShipmentStatus();
                    }
                });
    };

    $scope.removeshipment = function (row) {
        if (row !== undefined && row.entity !== null) {
            var modalOptions = {
                headerText: $scope.Confirmation_,
                bodyText: $scope.ShipmentRemove_ConfirmText
            };

            ModalService.Confirm({}, modalOptions).then(function (result) {
                $scope.Template = 'directBooking/ajaxLoader.tpl.html';
                AppSpinner.showSpinnerTemplate($scope.DeletingTheShipment, $scope.Template);
                DirectBookingService.DeleteShipment(row.entity.ShipmentId).then(function (response) {
                    AppSpinner.hideSpinnerTemplate();
                    if (response.data.Status === true) {
                        $scope.DirectShipments();
                        AppSpinner.hideSpinnerTemplate();                        
                        toaster.pop({
                            type: 'success',
                            title: $scope.Frayte_Success,
                            body: $scope.ShipmentRemoveSuccessfully,
                            showCloseButton: true
                        });
                    }
                    else {
                        AppSpinner.hideSpinnerTemplate();
                        toaster.pop({
                            type: 'error',
                            title: $scope.Frayte_Error,
                            body: $scope.ShipmentRemoveErrorValidation,
                            showCloseButton: true
                        });
                    }
                }, function () {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.Frayte_Error,
                        body: $scope.ShipmentRemoveErrorValidation,
                        showCloseButton: true
                    });
                });
            });
        }
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
                AppSpinner.showSpinnerTemplate($scope.CancellingTheShipment, $scope.Template);
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

    $scope.MakeDraftShipmentAsPublic = function (row, IsPublic) {
        if (row !== undefined && row !== null && row.entity !== null) {
            if ($scope.track.BookingMethod === "DirectBooking") {
                DirectShipmentService.MarkDirectShipmentDraftAsPublic(row.entity.ShipmentId, IsPublic).then(function (response) {
                    if (response.data === true) {
                        toaster.pop({
                            type: 'success',
                            title: $scope.Frayte_Success,
                            body: $scope.ShareDraftAsPublic,
                            showCloseButton: true
                        });
                    }
                    else {
                        toaster.pop({
                            type: 'warning',
                            title: $scope.Frayte_Warning,
                            body: $scope.IssueDraftShipmentMakePublic,
                            showCloseButton: true
                        });
                    }
                });
            }
        }
    };

    $scope.CreateShipment = function (row, CallingType) {
        if (row !== undefined && row !== null && row.entity !== null) {

            // ToDo: Need to check who is editing the shipment

            if ($scope.track.BookingMethod === "DirectBooking") {

                if ($scope.tab !== undefined && $scope.tab !== null && $scope.tab.childTabs !== null) {
                    var route = "";
                    if (CallingType === "ShipmentClone") {
                        route = UtilityService.GetCurrentRoute($scope.tab.childTabs, "userTabs.direct-booking-clone");
                    }
                    else if (CallingType === "ShipmentReturn") {
                        route = UtilityService.GetCurrentRoute($scope.tab.childTabs, "userTabs.direct-booking-return");
                    }
                    else if (CallingType === "ShipmentDraft") {
                        route = UtilityService.GetCurrentRoute($scope.tab.childTabs, "userTabs.direct-booking");
                    }
                    $state.go(route, { directShipmentId: row.entity.ShipmentId }, { reload: true });
                }

            }
            else if ($scope.track.BookingMethod === "eCommerce") {
                if ($scope.$UserRoleId === 3) {
                    $state.go('customer.booking-home.eCommerce-booking-clone', { shipmentId: row.entity.ShipmentId }, { reload: true });
                }
                else if ($scope.$UserRoleId === 6) {
                    $state.go('dbuser.booking-home.eCommerce-booking-clone', { shipmentId: row.entity.ShipmentId }, { reload: true });
                }
                else if ($scope.$UserRoleId === 1) {
                    $state.go('admin.booking-home.eCommerce-booking-clone', { shipmentId: row.entity.ShipmentId }, { reload: true });
                }
            }
        }
    };

    $scope.DeleteDirectShipmentDocuments = function (row) {
        var modalOptions = {
            headerText: $scope.DeleteDraftShipment,
            bodyText: $scope.SureRemoveDraftShipmentDetail
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

    $scope.ChangeFromdate = function (FromDate) {
        $scope.track.FromDate = $scope.SetTimeinDateObj(FromDate);
        if (($scope.track.ToDate === undefined || $scope.track.ToDate === '' || $scope.track.ToDate === null) && ($scope.track.FromDate.getFullYear() === 1970)) {
            $scope.track.ToDate = null;
        }
        else if ($scope.track.ToDate === undefined || $scope.track.ToDate === '' || $scope.track.ToDate === null) {
            $scope.track.ToDate = new Date();
        }
    };

    $scope.ChangeTodate = function (ToDate) {
        $scope.track.ToDate = $scope.SetTimeinDateObj(ToDate);
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

    function getUserId() {
        var userInfo = SessionService.getUser();
        $scope.track.UserId = userInfo.EmployeeId;
    }

    $scope.LoadShipmentStatus = function () {
        //  getUserId();
        if ($scope.track.UserId != null && $scope.track.UserId !== undefined && $scope.track.UserId !== 0) {
            $scope.UserID = $scope.track.UserId;
        }
        else if ($scope.track.CustomerId !== null && $scope.track.CustomerId !== undefined && $scope.track.CustomerId !== 0) {
            $scope.UserID = $scope.track.CustomerId;
        }

        DirectShipmentService.GetDirectShipmentStatus($scope.track.BookingMethod, $scope.UserID).then(function (response) {
            $scope.DirectShipmentStatus = response.data;
            ObjAll = {
                BookingType: "DirectBooking",
                DisplayStatusName: "All",
                ShipmentStatusId: 0,
                StatusName: "All"
            };
            $scope.DirectShipmentStatus.unshift(ObjAll);
            var DSS;
            for (i = 0; i < response.data.length; i++) {
                if ($scope.DirectShipmentStatus[i].StatusName === "Draft") {
                    DSS = $scope.DirectShipmentStatus[i];
                    $scope.DirectShipmentStatus.splice(i, 1);
                    $scope.DirectShipmentStatus.splice(1, 0, DSS);
                }
                if ($scope.DirectShipmentStatus[i].StatusName === "Cancel") {
                    $scope.DirectShipmentStatus.splice(i, 1);
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
                else if ($scope.DirectShipmentStatus[i].StatusName === "Delivered") {
                    $scope.DirectShipmentStatus[i].ImgURL = "T-delivered.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "InfoReceived") {
                    $scope.DirectShipmentStatus[i].ImgURL = "T-inforeceived.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "InTransit") {
                    $scope.DirectShipmentStatus[i].ImgURL = "T-intransit.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "OutForDelivery") {
                    $scope.DirectShipmentStatus[i].ImgURL = "T-outofdelivery.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "Exception") {
                    $scope.DirectShipmentStatus[i].ImgURL = "T-exception.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "FailedAttempt") {
                    $scope.DirectShipmentStatus[i].ImgURL = "T-failedattemt.png";
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

    $scope.pageChanged = function (track) {
        if (($scope.track.ToDate === '' || $scope.track.ToDate === null || $scope.track.ToDate === undefined) && ($scope.track.FromDate === '' || $scope.track.FromDate === null || $scope.track.FromDate === undefined)) {
            $scope.DirectShipments();
        }
        else if (($scope.track.ToDate !== null || $scope.track.ToDate !== '' || $scope.track.ToDate !== undefined) && ($scope.track.FromDate === undefined || $scope.track.FromDate === '' || $scope.track.FromDate === null)) {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.From_Date,
                showCloseButton: true
            });
            if (track.CurrentPage === 1) {

            }
            else {
                track.CurrentPage = 1;
            }
            return;
        }
        else if (($scope.track.ToDate === null || $scope.track.ToDate === '' || $scope.track.ToDate === undefined) && ($scope.track.FromDate !== undefined || $scope.track.FromDate !== '' || $scope.track.FromDate !== null)) {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.To_Date,
                showCloseButton: true
            });
            if (track.CurrentPage === 1) {

            }
            else {
                track.CurrentPage = 1;
            }
            return;
        }
        else {
            $scope.DirectShipments();
        }
    };

    $scope.getDirectShipments = function () {
        $scope.DirectShipments();
    };

    $scope.DirectShipments = function () { 
        if (($scope.track.ToDate === '' || $scope.track.ToDate === null || $scope.track.ToDate === undefined) && ($scope.track.FromDate === '' || $scope.track.FromDate === null || $scope.track.FromDate === undefined)) {
            $scope.track.ToDate = null;
            $scope.track.FromDate = null;
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
                var from = ((fromDate.getMonth() + 1).toString().length === 1 ? "0" + (fromDate.getMonth() + 1) : (fromDate.getMonth() + 1)) + '/' + (fromDate.getDate().toString().length === 1 ? "0" + fromDate.getDate() : fromDate.getDate()) + '/' + fromDate.getFullYear();
                var to = ((toDate.getMonth() + 1).toString().length === 1 ? "0" + (toDate.getMonth() + 1) : (toDate.getMonth() + 1)) + '/' + (toDate.getDate().toString().length === 1 ? "0" + toDate.getDate() : toDate.getDate()) + '/' + toDate.getFullYear();

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

        if ($scope.track.TrackingNo !== undefined && $scope.track.TrackingNo !== null && $scope.track.TrackingNo !== '') {
            $scope.track.CurrentPage = 1;
        }
        if ($scope.track.FrayteNumber !== undefined && $scope.track.FrayteNumber !== null && $scope.track.FrayteNumber !== '') {
            $scope.track.CurrentPage = 1;
        }

        if ($scope.track.BookingMethod === 'DirectBooking') {
            AppSpinner.showSpinnerTemplate($scope.LoadingTrackTrace, $scope.Template);
        }
        else if ($scope.track.BookingMethod === 'eCommerce') {
            AppSpinner.showSpinnerTemplate($scope.LoadingTrackTrace, $scope.Template);
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

                //if (response.data[i].BooingApp ===)

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
            for (i = 0; i < response.data.length; i++) {

                if (response.data[i].Status === 'Cancel') {
                    response.data.splice(i, 1);
                }
                response.data[i].ShippingDate = DateFormatChange.DateFormatChange(response.data[i].ShippingDate);
            }

            $scope.gridOptions.data = response.data;
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

    $scope.buildURL = config.BUILD_URL;

    $scope.logisticStatus = function (LogisticType) {
        if (LogisticType !== undefined) {
            if (LogisticType.LogisticType !== "UKShipment") {
                $scope.CurrentLogisticServiceType = $scope.LogisticServiceTypes[0];
            }
            $scope.CurrentLogisticType = LogisticType;
        }
    };

    var setOrderForCustomer = function () {
        for (var i = 0; i < $scope.directBookingCustomers.length; i++) {
            if ($scope.directBookingCustomers[i].CustomerId === $scope.userInfo.EmployeeId) {
                $scope.Customer = $scope.directBookingCustomers[i];
                break;
            }
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

                if ($scope.userInfo.RoleId === 3) {

                    setOrderForCustomer();
                }


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
            $scope.moduleType = SessionService.getModuleType();
        }
    };

    $scope.getDetails = function () {
        $scope.LoadShipmentStatus();
    };

    $scope.getShipmenmtStatus = function () {
        $scope.DirectShipmentStatus = $scope.DirectShipment;
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

    $scope.TrackingPage = function (row) {
        debugger;
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'home/trackingnew.tpl.html',
            controller: 'HomeTrackingController',
            windowClass: '',
            size: 'lg',
            resolve: {
                ShipmentData: function () {
                    return row;
                },
                ShipmentData1: function () {
                    return;
                },
                ShipmentData2: function () {
                    return;
                }
            }
        });
        modalInstance.result.then(function () {

        }, function () {
        });
    };

    $scope.toggleMin = function () {
        $scope.dateOptions.minDate = $scope.dateOptions.minDate ? null : new Date();
    };

    $scope.toggleMin1 = function () {
        $scope.dateOptions1.minDate = $scope.dateOptions1.minDate ? null : new Date();
    };

    var setUserDashBoard = function () {

        angular.forEach($scope.shipments, function (eachObj) {
            if (eachObj.StatusId === $scope.AftershipStatusTag.Pending) {
                $scope.dashBoardDetail.Pending.TotalShipment += 1;
                $scope.dashBoardDetail.Pending.Trackings.push(eachObj);
            }
            if (eachObj.StatusId === $scope.AftershipStatusTag.Expired) {
                $scope.dashBoardDetail.Expired.TotalShipment += 1;
                $scope.dashBoardDetail.Expired.Trackings.push(eachObj);
            }
            if (eachObj.StatusId === $scope.AftershipStatusTag.Exception) {
                $scope.dashBoardDetail.Exception.TotalShipment += 1;
                $scope.dashBoardDetail.Exception.Trackings.push(eachObj);
            }
            if (eachObj.StatusId === $scope.AftershipStatusTag.Delivered) {
                $scope.dashBoardDetail.Delivered.TotalShipment += 1;
                $scope.dashBoardDetail.Delivered.Trackings.push(eachObj);
            }
            if (eachObj.StatusId === $scope.AftershipStatusTag.AttemptFail) {
                $scope.dashBoardDetail.AttemptFail.TotalShipment += 1;
                $scope.dashBoardDetail.AttemptFail.Trackings.push(eachObj);
            }
            if (eachObj.StatusId === $scope.AftershipStatusTag.OutForDelivery) {
                $scope.dashBoardDetail.OutForDelivery.TotalShipment += 1;
                $scope.dashBoardDetail.OutForDelivery.Trackings.push(eachObj);
            }
            if (eachObj.StatusId === $scope.AftershipStatusTag.InTransit) {
                $scope.dashBoardDetail.InTransit.TotalShipment += 1;
                $scope.dashBoardDetail.InTransit.Trackings.push(eachObj);
            }
            if (eachObj.StatusId === $scope.AftershipStatusTag.InfoReceived) {
                $scope.dashBoardDetail.InfoReceived.TotalShipment += 1;
                $scope.dashBoardDetail.InfoReceived.Trackings.push(eachObj);
            }
        });

        calculatePercentage();
    };

    var calculatePercentage = function () {

        var totalShipments = $scope.shipments.length;
        $scope.dashBoardDetail.InfoReceived.Percentage = ($scope.dashBoardDetail.InfoReceived.TotalShipment / totalShipments) * 100;
        $scope.dashBoardDetail.Expired.Percentage = ($scope.dashBoardDetail.Expired.TotalShipment / totalShipments) * 100;
        $scope.dashBoardDetail.Exception.Percentage = ($scope.dashBoardDetail.Exception.TotalShipment / totalShipments) * 100;
        $scope.dashBoardDetail.Delivered.Percentage = ($scope.dashBoardDetail.Delivered.TotalShipment / totalShipments) * 100;
        $scope.dashBoardDetail.AttemptFail.Percentage = ($scope.dashBoardDetail.AttemptFail.TotalShipment / totalShipments) * 100;
        $scope.dashBoardDetail.OutForDelivery.Percentage = ($scope.dashBoardDetail.OutForDelivery.TotalShipment / totalShipments) * 100;
        $scope.dashBoardDetail.InTransit.Percentage = ($scope.dashBoardDetail.InTransit.TotalShipment / totalShipments) * 100;
        $scope.dashBoardDetail.Pending.Percentage = ($scope.dashBoardDetail.Pending.TotalShipment / totalShipments) * 100;

    };

    var getScreenInitials = function () { 
        AppSpinner.showSpinnerTemplate($scope.LoadingTrackTraceDashboard, $scope.Template);
        DirectShipmentService.TrackAndTraceDashboard($scope.Id).then(function (response) {
            if (response.status === 200 && response.data.Status) {
                $scope.shipments = response.data.Tracking;
                setUserDashBoard(); 
            }
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            toaster.pop({
                type: "error",
                status: $scope.FrayteError,
                body: $scope.ErrorGettingRecordPleaseTryAgain
            });
            AppSpinner.hideSpinnerTemplate();
        });
    };

    $scope.viewShipments = function (detail) {
        if (detail.TotalShipment) {
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'directBooking/directShipments/trackandtraceAftershipDetail/trackAndTraceDashBoardShipments.tpl.html',
                controller: 'DashBoardShipmentController',
                windowClass: 'DirectBookingDetail',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    StatusId: function () {
                        return detail.StatusId;
                    },
                    CustomerId: function () {
                        return $scope.customerId;
                    },
                    Status: function () {
                        return detail.Status;
                    }
                }
            });

            modalInstance.result.then(function () {

            }, function () {

            });
        }
    };

    var getInitialDetails = function () {
        $scope.customerId = $scope.userInfo.EmployeeId;
        
        AppSpinner.showSpinnerTemplate($scope.LoadingTrackTraceDashboard, $scope.Template);
        CustomerService.GetCustomerDetail($scope.customerId).then(function (response) {
            $scope.customerDetail = response.data;
            getScreenInitials();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ErrorGettingRecordPleaseTryAgain,
                showCloseButton: true
            });
        });

        $scope.AftershipStatusTag = {
            Pending: 0,
            InfoReceived: 1,
            InTransit: 2,
            OutForDelivery: 3,
            AttemptFail: 4,
            Delivered: 5,
            Exception: 6,
            Expired: 7
        };

        $scope.AftershipStatus =
        {
            Pending: "Pending",
            PendingDisplay: "Shipment Booked",
            InfoReceived: "InfoReceived",
            InfoReceivedDisplay: "Info Received",
            InTransit: "InTransit",
            InTransitDisplay: "In Transit",
            OutForDelivery: "OutForDelivery",
            OutForDeliveryDisplay: "Out For Delivery",
            AttemptFail: "AttemptFail",
            AttemptFailDisplay: "Failed Attempt",
            Delivered: "Delivered",
            DeliveredDisplay: "Delivered",
            Exception: "Exception",
            ExceptionDislay: "Exception",
            Expired: "Expired",
            ExpiredDisplay: "Expired"
        };

        $scope.dashBoardDetail = {
            Delivered: {
                StatusId: $scope.AftershipStatusTag.Delivered,
                Status: $scope.AftershipStatus.Delivered,
                StatusDisplay: $scope.AftershipStatus.DeliveredDisplay,
                TotalShipment: 0,
                Trackings: [],
                Percentage: 0,
                ImageName: 'Delivered.png'
            },
            InTransit: {
                StatusId: $scope.AftershipStatusTag.InTransit,
                Status: $scope.AftershipStatus.InTransit,
                StatusDisplay: $scope.AftershipStatus.InTransitDisplay,
                TotalShipment: 0,
                Trackings: [],
                Percentage: 0,
                ImageName: 'InTransit.png'
            },
            OutForDelivery: {
                StatusId: $scope.AftershipStatusTag.OutForDelivery,
                Status: $scope.AftershipStatus.OutForDelivery,
                StatusDisplay: $scope.AftershipStatus.OutForDeliveryDisplay,
                TotalShipment: 0,
                Trackings: [],
                Percentage: 0,
                ImageName: 'OutForDelivery.png'
            },
            Pending: {
                StatusId: $scope.AftershipStatusTag.Pending,
                Status: $scope.AftershipStatus.Pending,
                StatusDisplay: $scope.AftershipStatus.PendingDisplay,
                TotalShipment: 0,
                Trackings: [],
                Percentage: 0,
                ImageName: 'Pending.png'
            },
            AttemptFail: {
                StatusId: $scope.AftershipStatusTag.AttemptFail,
                Status: $scope.AftershipStatus.AttemptFail,
                StatusDisplay: $scope.AftershipStatus.AttemptFailDisplay,
                TotalShipment: 0,
                Trackings: [],
                Percentage: 0,
                ImageName: 'FailedAttemt.png'
            },
            InfoReceived: {
                StatusId: $scope.AftershipStatusTag.InfoReceived,
                Status: $scope.AftershipStatus.InfoReceived,
                StatusDisplay: $scope.AftershipStatus.InfoReceivedDisplay,
                TotalShipment: 0,
                Trackings: [],
                Percentage: 0,
                ImageName: 'InfoReceived.png'
            },
            Exception: {
                StatusId: $scope.AftershipStatusTag.Exception,
                Status: $scope.AftershipStatus.Exception,
                StatusDisplay: $scope.AftershipStatus.ExceptionDisplay,
                TotalShipment: 0,
                Trackings: [],
                Percentage: 0,
                ImageName: 'Exception.png'
            },
            Expired: {
                StatusId: $scope.AftershipStatusTag.Expired,
                Status: $scope.AftershipStatus.Expired,
                StatusDisplay: $scope.AftershipStatus.ExpiredDisplay,
                TotalShipment: 0,
                Trackings: [],
                Percentage: 0,
                ImageName: 'Expired.png'
            }
        };
    };

    //Shipment dashboard hide/show
    $scope.ShowHideServiceOptionPanel = function () {
        $scope.ShowServiceOptionPanel = !$scope.ShowServiceOptionPanel;
        if ($scope.ShowServiceOptionPanel === true) {
            $scope.ShowAdvanceOptionPanel = false;
            $scope.IsAdvanceShow = false;
            $scope.IsDashboardShow = true;
            getInitialDetails();
        }
        else if ($scope.ShowServiceOptionPanel === false) {
            $scope.ShowAdvanceOptionPanel = true;
            $scope.IsDashboardShow = false;
            $scope.IsAdvanceShow = true;
            $scope.LoadShipmentStatus();
        }
    };

    //Advance search option hide/show
    $scope.ShowHideAdvanceOptionPanel = function () {
        $scope.ShowAdvanceOptionPanel = !$scope.ShowAdvanceOptionPanel;
        if ($scope.ShowAdvanceOptionPanel === true) {
            $scope.ShowServiceOptionPanel = false;
            $scope.IsDashboardShow = false;
            $scope.IsAdvanceShow = true;
            $scope.LoadShipmentStatus();
        }
        else if ($scope.ShowAdvanceOptionPanel === false) {
            $scope.ShowServiceOptionPanel = true;
            $scope.IsDashboardShow = true;
            $scope.IsAdvanceShow = false;
            getInitialDetails();
        }
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

    $scope.pageChanged = function (track) {
        $scope.DirectShipments();
    };

    function init() {

        $rootScope.ChangeManifest = false;

        var url = SessionService.GetSiteURL();
        //trackingTemplate = '<div class="paddingTB5 word-wrap"><a target="_blank" href= "' + url + '/home/tracking-hub/{{row.entity.CourierName}}/{{row.entity.TrackingCode}}/{{row.entity.RateType}}" >{{row.entity.TrackingNo}}</a></div>';
        trackingTemplate = '<div class="paddingTB5 word-wrap"> <span class="pointer" ng-if="row.entity.IsTrackingShow"><a ng-click="grid.appScope.TrackingPage(row.entity)">{{row.entity.TrackingNo}}</a></span> <span class="pointer" ng-if="!row.entity.IsTrackingShow">{{row.entity.TrackingNo}}</span></div>';
        detailTemplate = '<div class="paddingTB5 word-wrap">{{row.entity.ShippingDate}}</div>';

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

        $scope.eCommerceShipmentTypes = [
            {
                value: 'All',
                display: 'All'
            },
            {
                value: 'ECOMMERCE_ONL',
                display: 'Online'
            },
            {
                value: 'ECOMMERCE_WS',
                display: 'Without Service'
            },
            {
                value: 'ECOMMERCE_SS',
                display: 'Service Selected'
            }
        ];

        // Pagination Logic 
        $scope.viewby = 50;
        $scope.currentPage = 1;
        $scope.itemsPerPage = $scope.viewby;
        $scope.totalItemCount = 1000;

        $scope.Id = $scope.customerId;

        $scope.maxSize = 2; //Number of pager buttons to show

        var userInfo = SessionService.getUser();
        $scope.USerInfo = userInfo;
        if (userInfo) {
            setMultilingualOptions();
        }
        else {
            $state.go("login");
        }

        $scope.tabs = userInfo.tabs;
        $scope.tab = getUserTab($scope.tabs, "QuickDirect_Booking");

        $scope.$UserRoleId = userInfo.RoleId;
        $scope.StaffRoleId = userInfo.RoleId;

        $scope.SetGridOptions();

        $scope.gridOptions.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
        };

        $scope.FromDate = null;
        $scope.ToDate = null;

        setBookingState("DirectBooking");

        $scope.calledFrom = "";

        if ($stateParams.moduleType && $stateParams.moduleType === "db") {

            $scope.calledFrom = $stateParams.moduleType;
        }
        
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

        $scope.track = {
            UserId: userInfo.EmployeeId,
            DateTime: '',
            FromDate: '',
            ToDate: '',
            ShipmentStatusId: 0,
            CustomerId: 0,
            FrayteNumber: '',
            TrackingNo: '',
            BookingMethod: SessionService.getModuleType(),
            CallingFrom: $scope.calledFrom,
            eCommerceShipmentType: "All",
            LogisticType: '',
            LogisticServiceType: '',
            CurrentPage: $scope.currentPage,
            TakeRows: $scope.itemsPerPage
        };

        if ($scope.$UserRoleId !== 17) {
            getCustomers();
        }

        $scope.gridheight = SessionService.getScreenHeight();

        $rootScope.GetServiceValue = null;
    }

    init();
});