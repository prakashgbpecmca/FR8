angular.module('ngApp.customer')
.controller("CustomerManifestDetailController", function ($scope, uiGridConstants,manifestObj, manifestId, $http, config, $window, $uibModal, toaster, SessionService, TermAndConditionService, ZoneBaseRateCardService, CustomerService, $rootScope, AppSpinner, $translate) {
    var setModalOptions = function () {
        $translate(['Frayte_Error', 'Removed_shipment_error', 'Removed_shipment', 'Frayte_Success', 'Error_While_Getting_Record', 'Downloaded_Status', 'Report_Generated_Successfully', 'Report_Status', 'Could_Not_Download_TheReport', 'No_Report_Generated']).then(function (translations) {

          
            $scope.FrayteError = translations.Frayte_Error;
            $scope.FrayteSuccess = translations.Frayte_Success;
            $scope.ErrorWhileGettingRecord = translations.Error_While_Getting_Record;
            $scope.DownloadedStatus = translations.Downloaded_Status;
            $scope.ReportGeneratedSuccessfully = translations.Report_Generated_Successfully;
            $scope.ReportStatus = translations.Report_Status;
            $scope.CouldNotDownloadTheReport = translations.Could_Not_Download_TheReport;
            $scope.NoReportGenerated = translations.No_Report_Generated;
            $scope.Removedshipment = translations.Removed_shipment;
            $scope.Removedshipmenterror = translations.Removed_shipment_error;
     



        });
    };

    $scope.printToCart = function (divName) {
        $scope.removeItem = false;
        var printContents = document.getElementById(divName).innerHTML;
        var originalContents = document.body.innerHTML;
        var popupWin;
        if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {
            popupWin = window.open('', '_blank', 'width=900,height=600,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
            popupWin.window.focus();
            popupWin.document.write('<!DOCTYPE html><html><head>' +
                 '<link rel="stylesheet" type="text/css" href="vendor/bootstrap/bootstrap.min.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/font-awesome/font-awesome.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/loading-bar/loading-bar.min.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/angular-ui-grid/ui-grid-unstable.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/angular-toaster/toaster.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/df-tab-menu/df-tab-menu.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/textAngular/style.css" />' +
                '<link rel="stylesheet" type="text/css" href="assets/Frayte-1.0.0.css" />' +
                '</head><body onload="window.print()"><div>' + printContents + '</div></html>');
            popupWin.onbeforeunload = function (event) {
                popupWin.close();
                return '.\n';
            };
            popupWin.onabort = function (event) {
                popupWin.document.close();
                popupWin.close();
            };
        } else {
            popupWin = window.open('', '_blank', 'width=800,height=600');
            popupWin.document.open();
            popupWin.document.write('<html><head>' +
                '<link rel="stylesheet" type="text/css" href="vendor/bootstrap/bootstrap.min.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/font-awesome/font-awesome.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/loading-bar/loading-bar.min.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/angular-ui-grid/ui-grid-unstable.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/angular-toaster/toaster.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/df-tab-menu/df-tab-menu.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/textAngular/style.css" />' +
                '<link rel="stylesheet" type="text/css" href="assets/Frayte-1.0.0.css" />' +
            '</head><body onload="window.print()">' + printContents + '</html>');
            popupWin.document.close();
        }
        popupWin.document.close();

        return true;
    };

    $scope.removeShipmentFromManifest = function (shipment) {
        if (shipment !== undefined) {
            CustomerService.RemoveShipmentFromManifest(shipment.ShipmentId, $scope.ManifestDetail.ModuleType).then(function (response) {
                if (response.data != null && response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.Removedshipment,
                        showCloseButton: true
                    });
                    getScreeninitials();

                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteError,
                        body: $scope.Removedshipmenterror,
                        showCloseButton: true
                    });
                }
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.Removedshipmenterror,
                    showCloseButton: true
                });
            });
        }
      
    };

    var getScreeninitials = function () {
        AppSpinner.showSpinnerTemplate("Loading Manifest Detail", $scope.Template);
        CustomerService.ViewManifest($scope.manifestId, $scope.ManifestDetail.ModuleType).then(function (response) {
            if (response.data !== null) {
                $scope.ViewManifest = response.data;
                if ($scope.ViewManifest.ManifestedList !== null && $scope.ViewManifest.ManifestedList.length) {
                    for (var i = 0 ; i < $scope.ViewManifest.ManifestedList.length; i++) {

                        if ($scope.ViewManifest.ManifestedList[i].DirectShipments !== null && $scope.ViewManifest.ManifestedList[i].DirectShipments.length) {

                            for (var j = 0; j < $scope.ViewManifest.ManifestedList[i].DirectShipments.length; j++) {
                                if ($scope.ViewManifest.ManifestedList[i].DirectShipments[j].ShippingBy === "UKMail" || $scope.ViewManifest.ManifestedList[i].DirectShipments[j].ShippingBy === "Yodel" || $scope.ViewManifest.ManifestedList[i].DirectShipments[j].ShippingBy === "Hermes") {
                                    $scope.ViewManifest.ManifestedList[i].DirectShipments[j].TrackingType = "UKEUShipment";
                                }
                                else {
                                    if ($scope.ViewManifest.ManifestedList[i].DirectShipments[j].ShippingBy === "DHL") {
                                        $scope.ViewManifest.ManifestedList[i].DirectShipments[j].TrackingType = $scope.ViewManifest.ManifestedList[i].ShippingBy + $scope.ViewManifest.ManifestedList[i].RateType;
                                    }
                                    else {
                                        $scope.ViewManifest.ManifestedList[i].DirectShipments[j].TrackingType = $scope.ViewManifest.ManifestedList[i].DirectShipments[j].ShippingBy;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else {
                console.log("No record available");
                //toaster.pop({
                //    type: 'error',
                //    title: $scope.FrayteError,
                //    body: "Error while getting record.",
                //    showCloseButton: true
                //});
            }
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ErrorWhileGettingRecord,
                showCloseButton: true
            });
        });
    };
    $scope.DownloadmanifestReport = function () {
        if ($scope.manifestId !== undefined && $scope.manifestId !== 0) {
            AppSpinner.showSpinnerTemplate("Downloading Direct Booking Manifest PDF", $scope.Template);
            CustomerService.GenerateManifest($scope.manifestId, $scope.ManifestDetail.ModuleType, $scope.UserId).then(function (response) {
                AppSpinner.hideSpinner();
                var fileInfo = response.data;
                var fileName = {
                    FileName: response.data.FileName
                };
                if (response.data != null) {
                    $http({
                        method: 'POST',
                        url: config.SERVICE_URL + '/Manifest/DownloadReport',
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
                                    title: $scope.FrayteSuccess,
                                    body: $scope.ReportGeneratedSuccessfully,
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
                           title: $scope.FrayteError,
                           body: $scope.CouldNotDownloadTheReport,
                           showCloseButton: true
                       });
                   });

                }
                else {
                }
            }, function () {
                AppSpinner.hideSpinner();
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.NoReportGenerated,
                    showCloseButton: true
                });
            });
        }
    };
    var getCurrentOperationZone = function () {
        TermAndConditionService.GetCurrentOperationZone().then(function (response) {
            if (response.data !== null) {
                $scope.OperationZone = response.data;
                getLogisticItems();
            }
            else {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.TextErrorGettingTermAndCondition,
                    showCloseButton: true
                });
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.TextErrorGettingTermAndCondition,
                showCloseButton: true
            });
        });
    };
    function init() {
        $scope.manifestId = manifestId;
        $scope.ManifestDetail = manifestObj;
        $scope.ManifestedShipmentJson = [];
        getScreeninitials();

       // $scope.spinnerMessage = 'Downloading Manifest';
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        var userInfo = SessionService.getUser();
        $scope.UserId = userInfo.EmployeeId;
        setModalOptions();
    }

    init();
});