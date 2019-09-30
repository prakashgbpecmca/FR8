angular.module('ngApp.baseRateCard').controller('ViewBaseRateCardController', function ($scope, ZoneBaseRateCardHistoryService, $http, config, $window, AppSpinner, toaster, uiGridConstants, $translate) {

    var setMultilingualOptions = function () {
        $translate(['Success', 'Record_Saved', 'FrayteError_Validation', 'ErrorSavingRecord', 'GettingDetails_Error', 'CancelShipmentError_Validation',
                    'GeneratePdfError_Validation', 'GeneratePdfError_Validation', 'SuccessfullySendlLabel_Validation', 'SendingMailError_Validation',
                    'EnterValidEmailAdd', 'TrackShipmentNotTrack_Error', 'FrayteWarning', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation',
                    'ErrorSavingRecord', 'FrayteError', 'FrayteSuccess', 'RateCard_Generated_Successfully', 'Could_Not_Download_RateCard',
                    'Error_downloading_baserateexcel', 'No_Rate_Card_For_This_Courier', 'DownloadingBaseRateCardExcel', 'DownloadingReport']).then(function (translations) {
                    $scope.Frayte_Warning = translations.FrayteWarning;
                    $scope.ErrorSaving_Record = translations.ErrorSavingRecord;
                    $scope.Frayte_Error = translations.FrayteError;
                    $scope.Frayte_Success = translations.FrayteSuccess;
                    $scope.RateCard_GeneratedSuccessfully = translations.RateCard_Generated_Successfully;
                    $scope.Could_NotDownload_RateCard = translations.Could_Not_Download_RateCard;
                    $scope.Error_downloadingbaserateexcel = translations.Error_downloading_baserateexcel;
                    $scope.No_Rate_Card_For_This_Courier = translations.No_Rate_Card_For_This_Courier;
                    $scope.DownloadingBaseRateCardExcel = translations.DownloadingBaseRateCardExcel;
                    $scope.DownloadingReport = translations.DownloadingReport;
        });
    };

    $scope.ExportRateCardreport = function (row) {
        if (row !== undefined && row.entity !== null) {
            AppSpinner.showSpinnerTemplate($scope.DownloadingBaseRateCardExcel, $scope.Template);
            ZoneBaseRateCardHistoryService.GenerateReport(row.entity.LogisticServiceId, $scope.Year).then(function (response) {
                $scope.GenerateReportData = response.data;
                row.entity.FileName = response.data.FileName;
                row.entity.FilePath = response.data.FilePath;
                //
                if (row !== undefined && row.entity !== null) {
                    var fileName = {
                        FileName: row.entity.FileName
                    };
                    if (response.status == 200 && response.data !== null && response.data.FileName !== '' && response.data.FilePath !== '' && response.data.FileName !== null && response.data.FilePath !== null) {
                        AppSpinner.showSpinnerTemplate($scope.DownloadingReport, $scope.Template);
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
                    else if (response.data.FileStatus === false) {
                        AppSpinner.hideSpinnerTemplate();
                        toaster.pop({
                            type: 'warning',
                            title: $scope.Frayte_Warning,
                            body: $scope.No_Rate_Card_For_This_Courier,
                            showCloseButton: true
                        });
                    }
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

    $scope.setGirdOptions = function () {
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
                  { name: 'LogisticCompanyDisplay', displayName: 'Logistics_Company', width: '20%', headerCellFilter: 'translate' },
                  { name: 'LogisticTypeDisplay', displayName: 'Logistic_Type', headerCellFilter: 'translate', width: '20%' },
                  { name: 'RateTypeDisplay', displayName: 'Rate_Type', headerCellFilter: 'translate', width: '23%' },
                  { name: 'IssueDate', displayName: 'Issued_Date', headerCellFilter: 'translate', width: '13%' },
                  { name: 'ExpiryDate', displayName: 'Expiry_Date', headerCellFilter: 'translate', width: '13%' },
                  { name: 'Edit', displayName: '', enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "zoneSetting/baseRateCard/baseRateCardHistory/baseRateCardHistoryEditButton.tpl.html" }
            ]
        };
    };

    //Set Grid Data
    $scope.GetGridData = function () {
        ZoneBaseRateCardHistoryService.GetBaseRateCard($scope.Year).then(function (response) {
            if (response.data !== undefined && response.data !== null) {
                $scope.LogisticService = response.data;
                $scope.LogisticFinalServices = [];
                for (var i = 0; i < $scope.LogisticService.length; i++) {

                    if ($scope.LogisticService[i].IssueDate !== undefined && $scope.LogisticService[i].IssueDate !== null) {
                        var issdate = new Date($scope.LogisticService[i].IssueDate);
                        for (var j = 0; j < $scope.MonthName.length; j++) {
                            if ($scope.MonthName[j].Value === (issdate.getMonth() + 1)) {
                                $scope.NewIssueDate = ((issdate.getDate().toString().length > 1 ? issdate.getDate() : '0' + issdate.getDate().toString()) + '-' + $scope.MonthName[j].Display + '-' + issdate.getFullYear());
                            }
                        }
                    }

                    if ($scope.LogisticService[i].ExpiryDate !== undefined && $scope.LogisticService[i].ExpiryDate !== null) {
                        var exdate = new Date($scope.LogisticService[i].ExpiryDate);
                        for (var k = 0; k < $scope.MonthName.length; k++) {
                            if ($scope.MonthName[k].Value === (exdate.getMonth() + 1)) {
                                $scope.NewExpiryDate = ((exdate.getDate().toString().length > 1 ? exdate.getDate() : '0' + exdate.getDate().toString()) + '-' + $scope.MonthName[k].Display + '-' + exdate.getFullYear());
                            }
                        }
                    }

                    var serviceJson = {
                        LogisticServiceId: 0,
                        LogisticCompany: "",
                        LogisticCompanyDisplay: "",
                        LogisticType: "",
                        LogisticTypeDisplay: "",
                        RateType: "",
                        RateTypeDisplay: "",
                        IssueDate: "",
                        ExpiryDate: ""
                    };
                    if ($scope.LogisticService[i].LogisticCompany === "UKMail") {
                        serviceJson.LogisticServiceId = $scope.LogisticService[i].LogisticServiceId;
                        serviceJson.LogisticCompany = $scope.LogisticService[i].LogisticCompany;
                        serviceJson.LogisticCompanyDisplay = $scope.LogisticService[i].LogisticCompanyDisplay;
                        serviceJson.LogisticType = $scope.LogisticService[i].LogisticType;
                        serviceJson.LogisticTypeDisplay = $scope.LogisticService[i].LogisticTypeDisplay;
                        serviceJson.RateType = '';
                        serviceJson.RateTypeDisplay = "Singles - Multiples Services";
                        serviceJson.IssueDate = $scope.NewIssueDate;
                        serviceJson.ExpiryDate = $scope.NewExpiryDate;
                    }
                    else if ($scope.LogisticService[i].LogisticCompany === "Yodel") {
                        serviceJson.LogisticServiceId = $scope.LogisticService[i].LogisticServiceId;
                        serviceJson.LogisticCompany = $scope.LogisticService[i].LogisticCompany;
                        serviceJson.LogisticCompanyDisplay = $scope.LogisticService[i].LogisticCompanyDisplay;
                        serviceJson.LogisticType = $scope.LogisticService[i].LogisticType;
                        serviceJson.LogisticTypeDisplay = $scope.LogisticService[i].LogisticTypeDisplay;
                        serviceJson.RateType = '';
                        serviceJson.RateTypeDisplay = "B2B - B2C Services";
                        serviceJson.IssueDate = $scope.NewIssueDate;
                        serviceJson.ExpiryDate = $scope.NewExpiryDate;
                    }
                    else if ($scope.LogisticService[i].LogisticCompany === "Hermes") {
                        serviceJson.LogisticServiceId = $scope.LogisticService[i].LogisticServiceId;
                        serviceJson.LogisticCompany = $scope.LogisticService[i].LogisticCompany;
                        serviceJson.LogisticCompanyDisplay = $scope.LogisticService[i].LogisticCompanyDisplay;
                        serviceJson.LogisticType = $scope.LogisticService[i].LogisticType;
                        serviceJson.LogisticTypeDisplay = $scope.LogisticService[i].LogisticTypeDisplay;
                        serviceJson.RateType = '';
                        serviceJson.RateTypeDisplay = "POD - NONPOD Services";
                        serviceJson.IssueDate = $scope.NewIssueDate;
                        serviceJson.ExpiryDate = $scope.NewExpiryDate;
                    }
                    else {
                        serviceJson.LogisticServiceId = $scope.LogisticService[i].LogisticServiceId;
                        serviceJson.LogisticCompany = $scope.LogisticService[i].LogisticCompany;
                        serviceJson.LogisticCompanyDisplay = $scope.LogisticService[i].LogisticCompanyDisplay;
                        serviceJson.LogisticType = $scope.LogisticService[i].LogisticType;
                        serviceJson.LogisticTypeDisplay = $scope.LogisticService[i].LogisticTypeDisplay;
                        serviceJson.RateType = $scope.LogisticService[i].RateType;
                        serviceJson.RateTypeDisplay = $scope.LogisticService[i].RateTypeDisplay;
                        serviceJson.IssueDate = $scope.NewIssueDate;
                        serviceJson.ExpiryDate = $scope.NewExpiryDate;
                    }

                    $scope.LogisticFinalServices.push(serviceJson);
                }
            }
            $scope.gridOptions.data = $scope.LogisticFinalServices;
        });
    };

    function init() {

        $scope.MonthName = [
            {
                Value: 1,
                Display: 'Jan'
            },
            {
                Value: 2,
                Display: 'Feb'
            },
            {
                Value: 3,
                Display: 'Mar'
            },
            {
                Value: 4,
                Display: 'Apr'
            },
            {
                Value: 5,
                Display: 'May'
            },
            {
                Value: 6,
                Display: 'Jun'
            },
            {
                Value: 7,
                Display: 'Jul'
            },
            {
                Value: 8,
                Display: 'Aug'
            },
            {
                Value: 9,
                Display: 'Sep'
            },
            {
                Value: 10,
                Display: 'Oct'
            },
            {
                Value: 11,
                Display: 'Nov'
            },
            {
                Value: 12,
                Display: 'Dec'
            }
        ];

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