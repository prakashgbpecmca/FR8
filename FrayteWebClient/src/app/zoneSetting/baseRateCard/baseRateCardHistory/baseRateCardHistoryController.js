angular.module('ngApp.baseRateCard').controller('ViewBaseRateCardController', function ($scope, ZoneBaseRateCardHistoryService, $http, config, $window, AppSpinner, toaster, uiGridConstants, $translate) {

    var setMultilingualOptions = function () {
        $translate(['Success', 'Record_Saved', 'FrayteError_Validation', 'ErrorSavingRecord', 'GettingDetails_Error', 'CancelShipmentError_Validation',
            'GeneratePdfError_Validation', 'GeneratePdfError_Validation', 'SuccessfullySendlLabel_Validation', 'SendingMailError_Validation',
        'EnterValidEmailAdd', 'TrackShipmentNotTrack_Error', 'FrayteWarning', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation',
        'ErrorSavingRecord', 'FrayteError', 'FrayteSuccess', 'RateCard_Generated_Successfully', 'Could_Not_Download_RateCard', 'Error_downloading_baserateexcel']).then(function (translations) {

            $scope.Frayte_Warning = translations.FrayteWarning;
            $scope.ErrorSaving_Record = translations.ErrorSavingRecord;
            $scope.Frayte_Error = translations.FrayteError;
            $scope.Frayte_Success = translations.FrayteSuccess;
            $scope.RateCard_GeneratedSuccessfully = translations.RateCard_Generated_Successfully;
            $scope.Could_NotDownload_RateCard = translations.Could_Not_Download_RateCard;
            $scope.Error_downloadingbaserateexcel = translations.Error_downloading_baserateexcel;


        });
    };
    $scope.ExportRateCardreport = function (row) {
        if (row !== undefined && row.entity !== null) {
            AppSpinner.showSpinnerTemplate("Downloading Base Rate Card Excel", $scope.Template);
            ZoneBaseRateCardHistoryService.GenerateReport(row.entity.LogisticServiceId, $scope.Year).then(function (response) {
                $scope.GenerateReportData = response.data;
                row.entity.FileName = response.data.FileName;
                row.entity.FilePath = response.data.FilePath;
                //
                if (row !== undefined && row.entity !== null) {
                    var fileName = {
                        FileName: row.entity.FileName
                    };
                    AppSpinner.showSpinnerTemplate("Downloading Report", $scope.Template);
                    $http({
                        method: 'POST',
                        url: config.SERVICE_URL + '/ViewBaseRateCard/DownLoadRateCardReport',
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
                                    body: $scope.RateCard_GeneratedSuccessfully,
                                    showCloseButton: true
                                });
                            } catch (ex) {
                                $window.open(row.entity.FilePath, "_blank");
                                AppSpinner.hideSpinnerTemplate();
                                toaster.pop({
                                    type: 'success',
                                    title: $scope.Frayte_Success,
                                    body: $scope.RateCard_GeneratedSuccessfully,
                                    showCloseButton: true
                                });
                            }

                        }
                    })
                   .error(function (data) {
                       AppSpinner.hideSpinnerTemplate();
                       toaster.pop({
                           type: 'error',
                           title: $scope.Frayte_Error,
                           body: $scope.Could_NotDownload_RateCard,
                           showCloseButton: true
                       });
                   });


                }
                //
                //toaster.pop({
                //    type: 'success',
                //    title: "Frayte Success",
                //    body: "Successfully downloaded the excel.",
                //    showCloseButton: true
                //});
                AppSpinner.hideSpinnerTemplate();
            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.Error_downloadingbaserateexcel,
                    showCloseButton: true
                });
            });
        }
    };

    // year change function call

    $scope.YearChange = function () {
        $scope.GetGridData();
    };

    //$scope.DownLaodRateCardreport = function (row) {
    //    if (row !== undefined && row.entity !== null) {
    //        var fileName = {
    //            FileName: row.entity.FileName
    //        };
    //        AppSpinner.showSpinnerTemplate("Downloading Report", $scope.Template);
    //        $http({
    //            method: 'POST',
    //            url: config.SERVICE_URL + '/ViewBaseRateCard/DownLoadRateCardReport',
    //            data: fileName,
    //            responseType: 'arraybuffer'
    //        }).success(function (data, status, headers) {
    //            if (status == 200 && data !== null) {
    //                headers = headers();
    //                var filename = headers['x-filename'];
    //                var contentType = headers['content-type'];

    //                var linkElement = document.createElement('a');
    //                try {
    //                    var blob = new Blob([data], { type: contentType });
    //                    var url = window.URL.createObjectURL(blob);

    //                    linkElement.setAttribute('href', url);
    //                    if (filename === undefined || filename === null) {
    //                        linkElement.setAttribute("download", "Generated_Report." + fileType);
    //                    }
    //                    else {
    //                        linkElement.setAttribute("download", filename);
    //                    }

    //                    var clickEvent = new MouseEvent("click", {
    //                        "view": window,
    //                        "bubbles": true,
    //                        "cancelable": false
    //                    });
    //                    linkElement.dispatchEvent(clickEvent);
    //                    AppSpinner.hideSpinnerTemplate();
    //                    toaster.pop({
    //                        type: 'success',
    //                        title: $scope.Frayte_Success,
    //                        body: 'Report downloaded successfully in your system. Please check browser download folder.',
    //                        showCloseButton: true
    //                    });
    //                } catch (ex) {
    //                    $window.open(row.entity.FilePath, "_blank");
    //                    AppSpinner.hideSpinnerTemplate();
    //                    console.log(ex);
    //                }

    //            }
    //        })
    //       .error(function (data) {
    //           AppSpinner.hideSpinnerTemplate();
    //           toaster.pop({
    //               type: 'error',
    //               title: $scope.Frayte_Error,
    //               body: 'Could not download the report. Please try again.',
    //               showCloseButton: true
    //           });
    //       });


    //    }

    //};

    //Grid

    $scope.setGirdOptions = function () {
        $scope.gridOptions = {
            multiSelect: false,
            enableRowSelection: true,
            enableSelectAll: false,
            enableRowHeaderSelection: false,
            selectionRowHeaderWidth: 35,
            noUnselect: true,
            enableGridMenu: true,
            enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
            enableVerticalScrollbar: true,
            columnDefs: [
                  { name: 'LogisticCompanyDisplay', displayName: 'Logistics_Company', width: '30%', headerCellFilter: 'translate' },
                      { name: 'LogisticTypeDisplay', displayName: 'Logistic_Type', headerCellFilter: 'translate', width: '28%' },
                      { name: 'RateTypeDisplay', displayName: 'Rate_Type', headerCellFilter: 'translate', width: '30%' },
                      { name: 'Edit', displayName: '', enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "zoneSetting/baseRateCard/baseRateCardHistory/baseRateCardHistoryEditButton.tpl.html" }

            ]
        };
    };

    //Set Grid Data

    $scope.GetGridData = function () {
        ZoneBaseRateCardHistoryService.GetBaseRateCard($scope.Year).then(function (response) {
            $scope.gridOptions.data = response.data;
        });
    };
    function init() {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.setGirdOptions();
        $scope.gridOptions.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
        };
        ZoneBaseRateCardHistoryService.GetYear().then(function (response) {
            $scope.Years = response.data;
            $scope.Year = $scope.Years[0];
            if ($scope.Years) {
                $scope.GetGridData();
            }
        });
        setMultilingualOptions();
    }

    init();

});