angular.module('ngApp.tradelaneShipments').controller('TradelaneShipmentsHawbLabelController', function ($scope, TradelaneBookingService, $http, $translate, toaster, config, TradelaneShipmentId, $window) {


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

    $scope.hawbLabel = "HAWB Label";

    $scope.getHawbLabelData = function (TradelaneShipmentId) {
        TradelaneBookingService.getHAWB(TradelaneShipmentId).then(function (res) {
            if (res.data.length > 0 && res.data !== null && res.data !== 'undefined') {
                $scope.getHawbData = res.data;
                console.log('data show successfully');
            } else {
                console.log('data is not load');
            }
        });
    };


    $scope.downloadHawbLabel = function (HAWB, index, TradelaneShipmentDetailId) {
        TradelaneBookingService.CreateHAWBLabel(HAWB, index, TradelaneShipmentDetailId).then(function (response) {
            if (response.data !== null) {
                var fileName = {
                    FileName: response.data.FileName,
                    FilePath: response.data.FilePath
                };

                $http({
                    method: 'POST',
                    url: config.SERVICE_URL + '/TradelaneBooking/DownloadHAWBLabel',
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


    function init() {
        $scope.getHawbLabelData(TradelaneShipmentId);
        setMultilingualOptions();
    }

    init();

});