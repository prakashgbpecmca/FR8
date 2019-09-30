angular.module('ngApp.express').controller("ExpressManifestController", function ($scope, $uibModal, ModalService, SessionService, config, $window, AppSpinner, ExpressManifestService, DirectBookingService, $rootScope, toaster, ExpressShipmentService, $http, $translate) {

    //multilingual translation key
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteWarning', 'FrayteSuccess', 'Export_Manifest_Downloaded_Successfully', 'Driver_Manifest_Downloaded_Successfully',
            'ReportCannotDownloadPleaseTryAgain', 'Could_Not_Download_TheReport', 'ErrorGettingRecord', 'To_Date', 'From_Date', 'To_Date_Validation', 'LoadingManifests',
            'DownLoading_Driver_Manifest_PDF', 'DownLoad_Export_Manifest_PDF', 'DownLoading_Custom_Manifest', 'Custom_Manifest_Downloaded_Successfully']).then(function (translations) {
            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.FrayteError = translations.FrayteError;
            $scope.Export_Manifest_Downloaded_Successfully = translations.Export_Manifest_Downloaded_Successfully;
            $scope.Driver_Manifest_Downloaded_Successfully = translations.Driver_Manifest_Downloaded_Successfully;
            $scope.ReportCannotDownloadPleaseTryAgain = translations.ReportCannotDownloadPleaseTryAgain;
            $scope.Could_Not_Download_TheReport = translations.Could_Not_Download_TheReport;
            $scope.ErrorGettingRecord = translations.ErrorGettingRecord;
            $scope.Loading_Manifests = translations.LoadingManifests;
            $scope.To_Date = translations.To_Date;
            $scope.From_Date = translations.From_Date;
            $scope.To_Date_Validation = translations.To_Date_Validation;
            $scope.DownLoading_Driver_Manifest_PDF = translations.DownLoading_Driver_Manifest_PDF;
            $scope.DownLoad_Export_Manifest_PDF = translations.DownLoad_Export_Manifest_PDF;
            $scope.DownLoading_Custom_Manifest = translations.DownLoading_Custom_Manifest;
            $scope.Custom_Manifest_Downloaded_Successfully = translations.Custom_Manifest_Downloaded_Successfully;
            $scope.getDetails();
        });

    };

    $scope.shipmentDetail = function (TradelaneShipmentId) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'tradelaneBooking/tradelaneBookingDetail/tradelaneBookingDetail.tpl.html',
            controller: 'TradelaneBookingDetailController',
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                ShipmentId: function () {
                    return TradelaneShipmentId;
                },
                ModuleType: function () {
                    return "ExpressBooking";
                }
            }
        });
        modalInstance.result.then(function (response) {
            //     $state.go("loginView.userTabs.tradelane-shipments");
        }, function () {
            //   $state.go("loginView.userTabs.tradelane-shipments");
        });
    };

    $scope.expressUpdateMAWB = function (TradelaneShipmentId) {
        var modalInstance = $uibModal.open({
            Animation: true,
            controller: 'ExpressUpdateManifestController',
            templateUrl: 'express/expressManifest/updateMAWB.tpl.html',
            keyboard: true,
            size: 'lg', 
            backdrop: 'static', 
            resolve: {
                ShipmentId: function () {
                    return TradelaneShipmentId;
                }
            }
        });
        modalInstance.result.then(function (response) {
        }, function () {

        });
    };

    $scope.ChangeFromdate = function (FromDate) {
        $scope.TrackManifest.FromDate = $scope.SetTimeinDateObj(FromDate);
        if ($scope.TrackManifest.ToDate === undefined || $scope.TrackManifest.ToDate === '' || $scope.TrackManifest.ToDate === null) {
            $scope.TrackManifest.ToDate = new Date();
        }
    };

    $scope.ChangeTodate = function (ToDate) {

        $scope.TrackManifest.ToDate = $scope.SetTimeinDateObj(ToDate);
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

    $scope.GetManifest = function (Searchtext) {        

        if (($scope.TrackManifest.ToDate === '' || $scope.TrackManifest.ToDate === null || $scope.TrackManifest.ToDate === undefined) && ($scope.TrackManifest.FromDate === '' || $scope.TrackManifest.FromDate === null || $scope.TrackManifest.FromDate === undefined)) {
            AppSpinner.hideSpinnerTemplate();
        }
        else if (($scope.TrackManifest.ToDate !== null || $scope.TrackManifest.ToDate !== '' || $scope.TrackManifest.ToDate !== undefined) && ($scope.TrackManifest.FromDate === undefined || $scope.TrackManifest.FromDate === '' || $scope.TrackManifest.FromDate === null)) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.From_Date,
                showCloseButton: true
            });
            AppSpinner.hideSpinnerTemplate();
            return;
        }
        else if (($scope.TrackManifest.ToDate === null || $scope.TrackManifest.ToDate === '' || $scope.TrackManifest.ToDate === undefined) && ($scope.TrackManifest.FromDate !== undefined || $scope.TrackManifest.FromDate !== '' || $scope.TrackManifest.FromDate !== null)) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.To_Date,
                showCloseButton: true
            });
            AppSpinner.hideSpinnerTemplate();
            return;
        }
        else {
            AppSpinner.hideSpinnerTemplate();
            if (($scope.TrackManifest.FromDate !== undefined || $scope.TrackManifest.FromDate !== null || $scope.TrackManifest.FromDate !== "") && ($scope.TrackManifest.ToDate !== undefined || $scope.TrackManifest.ToDate !== null || $scope.TrackManifest.ToDate !== "")) {
                var fromDate = new Date($scope.TrackManifest.FromDate);
                var toDate = new Date($scope.TrackManifest.ToDate);
                var from = (fromDate.getDate().toString().length === 1 ? "0" + fromDate.getDate() : fromDate.getDate()) + '/' + ((fromDate.getMonth() + 1).toString().length === 1 ? "0" + (fromDate.getMonth() + 1) : (fromDate.getMonth() + 1)) + '/' + fromDate.getFullYear();
                var to = (toDate.getDate().toString().length === 1 ? "0" + toDate.getDate() : toDate.getDate()) + '/' + ((toDate.getMonth() + 1).toString().length === 1 ? "0" + (toDate.getMonth() + 1) : (toDate.getMonth() + 1)) + '/' + toDate.getFullYear();
                var dateOne = new Date(from);
                var dateTwo = new Date(to);
                if (new Date(dateOne) > new Date(dateTwo)) {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteWarning,
                        body: $scope.To_Date_Validation,
                        showCloseButton: true
                    });
                    return;
                }
            }
        }

        AppSpinner.showSpinnerTemplate($scope.Loading_Manifests, $scope.Template);
        ExpressManifestService.GetManifestDetail($scope.TrackManifest).then(function (response) {
            $scope.ManifestData = response.data;
            if (response.data !== null && response.data.length > 0) {
                $scope.totalItemCount = response.data[0].TotalRows;
            }
            else {
                $scope.totalItemCount = 0;
            }
            if (response.data != null && response.data.length > 0) {
                for (i = 0; i < response.data.length; i++) {
                    var date = new Date(response.data[i].CreatedOn);
                    var days = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
                    var getdt = date.getDate();
                    var getmn1 = days[date.getMonth()];
                    var getyr = date.getFullYear();
                    response.data[i].CreatedOn = getdt + "-" + getmn1 + "-" + getyr + " " + response.data[i].CreatedOnTime;
                }
            }
            AppSpinner.hideSpinnerTemplate();
        }, function (response) {
            AppSpinner.hideSpinnerTemplate();
            if (response.status !== 401) {
                toaster.pop({
                    type: "error",
                    title: $scope.FrayteError,
                    body: $scope.ErrorGettingRecord,
                    showCloseButton: true
                });
            }
        });
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

    $scope.pageChanged = function (TrackManifest) {
        $scope.GetManifest();
    };

    $scope.getDetails = function () {
        $rootScope.rootModuleType = $scope.TrackManifest.ModuleType;
        $scope.GetManifest();
    };

    $scope.getCustomerDetail = function (CustomerDetail) {
        if (CustomerDetail !== undefined && CustomerDetail !== null && CustomerDetail !== '') {
            $scope.CustomerId = CustomerDetail.CustomerId;
            $scope.TrackManifest.UserId = CustomerDetail.CustomerId;
            $rootScope.CustomerManId = CustomerDetail.CustomerId;
            //$rootScope.isShow = true;
        }
    };

    $scope.toggleMin = function () {
        $scope.dateOptions.minDate = $scope.dateOptions.minDate ? null : new Date();
    };

    $scope.toggleMin1 = function () {
        $scope.dateOptions1.minDate = $scope.dateOptions1.minDate ? null : new Date();
    };

    $scope.$watch('TrackManifest.ToDate', function () {
        if ($scope.TrackManifest !== undefined && $scope.TrackManifest !== null && $scope.TrackManifest.ToDate !== undefined && $scope.TrackManifest.ToDate !== null && $scope.TrackManifest.ToDate !== "" && $scope.TrackManifest.ToDate.getFullYear() === 1970) {
            $scope.TrackManifest.ToDate = null;
        }
    });

    $scope.$watch('TrackManifest.FromDate', function () {
        if ($scope.TrackManifest !== undefined && $scope.TrackManifest !== null && $scope.TrackManifest.FromDate !== undefined && $scope.TrackManifest.FromDate !== null && $scope.TrackManifest.FromDate !== "" && $scope.TrackManifest.FromDate.getFullYear() === 1970) {
            $scope.TrackManifest.FromDate = null;
        }
    });

    //breakbulk purchase order detail code
    $scope.ExpressViewManifest = function (ManifestId) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'express/expressManifest/expressViewManifest.tpl.html',
            controller: 'ExpressViewManifestController',
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                ManifestId: function () {
                    return ManifestId;
                }
            }
        });
        modalInstance.result.then(function (response) {
        }, function () {
        });
    };

    //breakbulk purchase order detail code
    $scope.breakbulkAddManifest = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'ExpressUpdateManifestController',
            templateUrl: 'express/expressManifest/updateMAWB.tpl.html',
            keyboard: true,
            size: 'lg',
            backdrop: 'static'
        });
    };

    $scope.GetCustomers = function () {
        if ($scope.RoleId === 6 || $scope.RoleId === 1 || $scope.RoleId === 20) {
            ExpressShipmentService.GetExpressCustomers($scope.RoleId, $scope.createdBy).then(function (response) {
                if (response !== null && response.data.length > 0) {
                    $scope.ManifestCustomers = [];
                    for (var i = 0; i < response.data.length; i++) {
                        $scope.Customer = {
                            CustomerId: "",
                            CompanyName: ""
                        };
                        $scope.Customer.CustomerId = response.data[i].CustomerId;
                        $scope.Customer.CompanyName = response.data[i].CompanyName + ' - ' + accBreak(response.data[i].AccountNumber.length, response.data[i].AccountNumber);
                        $scope.ManifestCustomers.push($scope.Customer);
                    }
                    $scope.CustomerId = $scope.ManifestCustomers[0].CustomerId;
                    
                    if ($scope.RoleId === 1) {
                        $scope.ManifestCustomers.unshift({ CustomerId: 0, CompanyName: 'All' });
                        $scope.CustomerDetail = $scope.ManifestCustomers[0];
                        $scope.TrackManifest.UserId = $scope.ManifestCustomers[0].CustomerId;
                    }
                    else if ($scope.RoleId === 6) {
                        $scope.ManifestCustomers.unshift({ CustomerId: 0, CompanyName: 'All' });
                        $scope.CustomerDetail = $scope.ManifestCustomers[0];
                        $scope.TrackManifest.UserId = $scope.ManifestCustomers[0].CustomerId;
                    }
                    else if ($scope.RoleId === 20) {
                        $scope.ManifestCustomers.unshift({ CustomerId: $scope.createdBy, CompanyName: 'All' });
                        $scope.CustomerDetail = $scope.ManifestCustomers[0];
                        $scope.TrackManifest.UserId = $scope.ManifestCustomers[0].CustomerId;
                    }
                    $rootScope.isShow = true;
                    if ($rootScope.ChangeExpressManifest === true) {
                        $scope.ExpressCreateManifest();
                        $rootScope.ShowManifestButton = false;
                        $rootScope.ChangeExpressManifest = false;
                    }
                    else {
                        $rootScope.ShowManifestButton = false;
                    }
                }
            });
            $scope.TrackManifest.CreatedBy = 0;
        }

        var accBreak = function (fiterCountryLength, AccountNo) {
            var AccNo = AccountNo.split('');
            var AccNonew = [];
            AccNonew2 = "";
            if (fiterCountryLength <= 3) {
                for (j = 0; j < fiterCountryLength; j++) {
                    if (j === 0) {
                        AccNonew.push('a');
                    }
                    AccNonew.push(AccNo[j]);
                }
                AccNonew.splice(0, 1);
                for (jj = 0; jj < AccNonew.length; jj++) {
                    AccNonew2 = AccNonew2 + AccNonew[jj].toString();
                }
            }
            else if (fiterCountryLength >= 4 && fiterCountryLength <= 8) {
                for (j = 0; j < fiterCountryLength; j++) {
                    if (j === 0) {
                        AccNonew.push('a');
                    }
                    AccNonew.push(AccNo[j]);
                }
                AccNonew.splice(0, 1);
                for (jj = 0; jj < AccNonew.length; jj++) {
                    AccNonew2 = AccNonew2 + AccNonew[jj].toString();
                }
            }
            else if (fiterCountryLength > 8) {
                for (j = 0; j < fiterCountryLength; j++) {
                    if (j === 0) {
                        AccNonew.push('a');
                    }
                    AccNonew.push(AccNo[j]);
                }
                AccNonew.splice(0, 1);
                for (jj = 0; jj < AccNonew.length; jj++) {
                    if (jj === 3) {
                        AccNonew2 = AccNonew2 + '-' + AccNonew[jj].toString();
                    }
                    else if (jj === 6) {
                        AccNonew2 = AccNonew2 + '-' + AccNonew[jj].toString();
                    }
                    else {
                        AccNonew2 = AccNonew2 + AccNonew[jj].toString();
                    }
                }
            }
            return AccNonew2;
        };
    };

    $scope.ExpressCreateManifest = function () {
        var ModalInstance = $uibModal.open({
            animation: true,
            controller: 'expressManifestGeneratorController',
            templateUrl: 'express/expressManifest/expressManifestGenerator.tpl.html',
            resolve: {
                TrackObj: function () {
                    return $scope.TrackManifest;
                }
            },
            backdrop: true,
            size: 'lg',
            windowClass: 'CustomerAddress-Edit',
            keyboard: false
        });
    };

    $scope.DownLoadExportManifestPDF = function (ManifestDetail) {
        AppSpinner.showSpinnerTemplate($scope.DownLoad_Export_Manifest_PDF, $scope.Template);
        $scope.PdfDownloadModel.TradelaneShipmentId = ManifestDetail.TradelaneShipmentId;
        $scope.PdfDownloadModel.CustomerId = ManifestDetail.CustomerId;
        $scope.PdfDownloadModel.UserId = $scope.createdBy;

        ExpressManifestService.GetExportManifest($scope.PdfDownloadModel).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            if (response.data !== null) {
                var fileInfo = response.data;
                var File = {
                    FileName: response.data.FileName,
                    TradelaneShipmentId: ManifestDetail.TradelaneShipmentId,
                    FilePath: response.data.FilePath
                };

                $http({
                    method: 'POST',
                    url: config.SERVICE_URL + '/ExpressManifest/DownloadExpressReport',
                    data: File,
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
                                    title: $scope.FrayteSuccess,
                                    body: $scope.Export_Manifest_Downloaded_Successfully,
                                    showCloseButton: true
                                });
                            }

                        } catch (ex) {
                            $window.open(File.FilePath, "_blank");
                            console.log(ex);
                        }
                    }
                })
               .error(function (data) {
                   AppSpinner.hideSpinner();
                   console.log(data);
                   toaster.pop({
                       type: 'error',
                       title: $scope.FrayteError,
                       body: $scope.Could_Not_Download_TheReport,
                       showCloseButton: true
                   });
               });
            }
            else {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.ReportCannotDownloadPleaseTryAgain,
                    showCloseButton: true
                });
            }

        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ReportCannotDownloadPleaseTryAgain,
                showCloseButton: true
            });
        });
    };

    $scope.DownLoadDriverManifestPDF = function (ManifestDetail) {
        AppSpinner.showSpinnerTemplate($scope.DownLoading_Driver_Manifest_PDF, $scope.Template);
        $scope.PdfDownloadModel.TradelaneShipmentId = ManifestDetail.TradelaneShipmentId;
        $scope.PdfDownloadModel.CustomerId = ManifestDetail.CustomerId;
        $scope.PdfDownloadModel.UserId = $scope.createdBy;

        ExpressManifestService.GetDriverManifest($scope.PdfDownloadModel).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            if (response.data !== null) {
                var fileInfo = response.data;
                var File = {
                    FileName: response.data.FileName,
                    TradelaneShipmentId: ManifestDetail.TradelaneShipmentId,
                    FilePath: response.data.FilePath
                };

                $http({
                    method: 'POST',
                    url: config.SERVICE_URL + '/ExpressManifest/DownloadExpressReport',
                    data: File,
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
                                    title: $scope.FrayteSuccess,
                                    body: $scope.Driver_Manifest_Downloaded_Successfully,
                                    showCloseButton: true
                                });
                            }

                        } catch (ex) {
                            $window.open(File.FilePath, "_blank");
                            console.log(ex);
                        }
                    }
                })
               .error(function (data) {
                   AppSpinner.hideSpinner();
                   console.log(data);
                   toaster.pop({
                       type: 'error',
                       title: $scope.FrayteError,
                       body: $scope.Could_Not_Download_TheReport,
                       showCloseButton: true
                   });
               });
            }
            else {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.ReportCannotDownloadPleaseTryAgain,
                    showCloseButton: true
                });
            }

        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ReportCannotDownloadPleaseTryAgain,
                showCloseButton: true
            });
        });
    };

    $scope.DownLoadCustomManifest = function (ManifestDetail) {
        AppSpinner.showSpinnerTemplate($scope.DownLoading_Custom_Manifest, $scope.Template);
        ExpressManifestService.GetCustomManifest(ManifestDetail.ManifestId).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            if (response.data !== null && response.data.FileName !== null && response.data.FileName !== undefined && response.data.FileName !== '') {
                var fileInfo = response.data;
                var File = {
                    FileName: response.data.FileName,
                    FilePath: response.data.FilePath
                };

                $http({
                    method: 'POST',
                    url: config.SERVICE_URL + '/ExpressManifest/DownloadCustomManifest',
                    data: File,
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
                                    title: $scope.FrayteSuccess,
                                    body: $scope.Custom_Manifest_Downloaded_Successfully,
                                    showCloseButton: true
                                });
                            }

                        } catch (ex) {
                            $window.open(File.FilePath, "_blank");
                            console.log(ex);
                        }
                    }
                })
               .error(function (data) {
                   AppSpinner.hideSpinner();
                   console.log(data);
                   toaster.pop({
                       type: 'error',
                       title: $scope.FrayteError,
                       body: $scope.Could_Not_Download_TheReport,
                       showCloseButton: true
                   });
               });
            }
            else {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarning,
                    body: $scope.ReportCannotDownloadPleaseTryAgain,
                    showCloseButton: true
                });
            }

        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ReportCannotDownloadPleaseTryAgain,
                showCloseButton: true
            });
        });
    };

    function init() {
        $scope.ImagePath = config.BUILD_URL;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        
        $scope.dateOptions = {
            formatYear: 'yy',
            minDate: new Date(),
            maxDate: new Date(),
            dateDisabled: function (data) {
                var date = data.date,
                mode = data.mode;
                if ($scope.TrackManifest.ToDate !== undefined && $scope.TrackManifest.ToDate !== "" && $scope.TrackManifest.ToDate !== null && $scope.TrackManifest.ToDate.getDate() === date.getDate()) {
                    return mode === 'day' && false;
                }
                return mode === 'day' && (($scope.TrackManifest.ToDate !== undefined && $scope.TrackManifest.ToDate !== null && $scope.TrackManifest.ToDate !== "") && date > $scope.TrackManifest.ToDate);
            }
        };
        $scope.dateOptions1 = {
            formatYear: 'yy',
            minDate: new Date(),
            maxDate: new Date(),
            dateDisabled: function (data) {
                var date = data.date,
                mode = data.mode;
                if ($scope.TrackManifest.FromDate !== undefined && $scope.TrackManifest.FromDate !== "" && $scope.TrackManifest.FromDate !== null && $scope.TrackManifest.FromDate.getDate() === date.getDate()) {
                    return mode === 'day' && false;
                }
                return mode === 'day' && (date < $scope.TrackManifest.FromDate);
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

        $scope.PdfDownloadModel = {
            TradelaneShipmentId: null,
            CustomerId: null,
            UserId: null
        };
        // Pagination Logic 
        $scope.viewby = 10;
        $scope.currentPage = 1;
        $scope.itemsPerPage = $scope.viewby;
        $scope.totalItemCount = 1000;

        $scope.maxSize = 2; //Number of pager buttons to show

        $scope.TrackManifest = {
            UserId: null,
            FromDate: null,
            ToDate: null,
            CreatedBy: null,
            ModuleType: $scope.moduleType,
            subModuleType: "",
            CurrentPage: $scope.currentPage,
            TakeRows: $scope.itemsPerPage
        };

        var userInfo = SessionService.getUser();

        $scope.RoleId = userInfo.RoleId;
        $scope.createdBy = userInfo.EmployeeId;

        if ($scope.RoleId === 3) {
            $scope.CustomerId = userInfo.EmployeeId;
            $rootScope.CustomerManId = userInfo.EmployeeId;
            $scope.TrackManifest.UserId = userInfo.EmployeeId;
            $scope.TrackManifest.CreatedBy = userInfo.EmployeeId;
            if ($rootScope.ChangeExpressManifest === true) {
                $scope.ExpressCreateManifest();
                $rootScope.ShowManifestButton = false;
                $rootScope.ChangeExpressManifest = false;
            }
            else {
                $rootScope.ShowManifestButton = false;
            }
        }
        if ($scope.RoleId === 17) {
            $scope.CustomerId = userInfo.EmployeeId;
            $rootScope.CustomerManId = userInfo.EmployeeId;
            $scope.TrackManifest.UserId = userInfo.EmployeeCustomerId;
            $scope.TrackManifest.CreatedBy = userInfo.EmployeeId;
            if ($rootScope.ChangeExpressManifest === true) {
                $scope.ExpressCreateManifest();
                $rootScope.ShowManifestButton = false;
                $rootScope.ChangeExpressManifest = false;
            }
            else {
                $rootScope.ShowManifestButton = false;
            }
        }
        if ($scope.RoleId === 20) {
            $scope.CustomerId = userInfo.EmployeeId;
            $rootScope.CustomerManId = userInfo.EmployeeId;
            $scope.TrackManifest.UserId = userInfo.EmployeeId;
            $scope.TrackManifest.CreatedBy = 0;
            if ($rootScope.ChangeExpressManifest === true) {
                $scope.ExpressCreateManifest();
                $rootScope.ShowManifestButton = false;
                $rootScope.ChangeExpressManifest = false;
            }
            else {
                $rootScope.ShowManifestButton = false;
            }
        }
        $scope.GetCustomers();
        if (userInfo === undefined || userInfo === null || userInfo.SessionId === undefined || userInfo.SessionId === '') {
            $state.go('home.welcome');
        }
        else {
            $scope.userInfo = userInfo;
            $scope.tabs = $scope.userInfo.tabs;
        }
        setMultilingualOptions();
    }

    init();
});