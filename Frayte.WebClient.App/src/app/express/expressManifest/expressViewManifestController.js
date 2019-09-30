
angular.module('ngApp.express').controller("ExpressViewManifestController", function ($scope, $uibModal, $http, $window, ModalService, AppSpinner, config, toaster, ManifestId, ExpressManifestService, SessionService, $translate) {


    //multilingual translation key
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteWarning', 'FrayteSuccess', 'Downloading_Bag_Label', 'BagLabel_Downloaded_Successfully',
            'Download_Unsuccessfull_Try_Again', 'ErrorGettingRecord', 'Tracking_Code']).then(function (translations) {

            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.FrayteError = translations.FrayteError;
            $scope.Downloading_Bag_Label = translations.Downloading_Bag_Label;
            $scope.BagLabel_Downloaded_Successfully = translations.BagLabel_Downloaded_Successfully;
            $scope.Download_Unsuccessfull_Try_Again = translations.Download_Unsuccessfull_Try_Again;
            $scope.ErrorGettingRecord = translations.ErrorGettingRecord;
            $scope.Tracking_Code = translations.Tracking_Code;
        });
    };

    //breakbulk shipment detail code here
    $scope.ExpressTrackingDetail = function (ShipmentId) {
        AppSpinner.showSpinnerTemplate($scope.Tracking_Code, $scope.Template);
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'expressTrackingController',
            templateUrl: 'express/expressTracking/expressTracking.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            resolve: {
                ExpressShipmentId: function () {
                    return ShipmentId;
                }
            }
        });
    };
    AppSpinner.hideSpinnerTemplate();
    //end


    //breakbulk getExpressionTracking code here
    $scope.GetExpressTracking = function (row) {
        ExpressShipmentService.GetExpressTracking(row.MAWB).then(function (response) {
            $scope.Id = response.data;
            if (response.data) {
                if (row.MAWB.includes('BGL')) {
                    row.BagId = $scope.Id;
                    $scope.ExpressTracking(row);
                }
                else {
                    row.TradelaneShipmentId = $scope.Id;
                    $scope.updateTrackingPopup(row);
                }
            }
        });
    };
    //end


    $scope.GetShipments = function (BagId) {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'ExpressBagShipmentsController',
            templateUrl: 'express/expressManifest/expressManifestShipment.tpl.html',
            keyboard: true,
            windowClass: 'directBookingDetail',
            size: 'lg',
            resolve: {

                BagId: function () {
                    return BagId;
                }
            }
        });
    };

    $scope.DownloadBagLabel = function (BagId) {
        AppSpinner.showSpinnerTemplate($scope.Downloading_Bag_Label, $scope.Template);
       
        ExpressManifestService.GetBagLabel(BagId).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            if (response.data !== null) {
                var fileInfo = response.data;
                var File = {
                    FileName: response.data.FileName,
                    TradelaneShipmentId: response.data.TradelaneShipmentId,
                    FilePath: response.data.FilePath
                };

                $http({
                    method: 'POST',
                    url: config.SERVICE_URL + '/ExpressManifest/DownloadExpressBagLabel',
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
                                    body: $scope.BagLabel_Downloaded_Successfully,
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
                       body: $scope.Download_Unsuccessfull_Try_Again,
                       showCloseButton: true
                   });
               });
            }
            else {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.Download_Unsuccessfull_Try_Again,
                    showCloseButton: true
                });
            }

        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.Download_Unsuccessfull_Try_Again,
                showCloseButton: true
            });
        });
    };

    function init() {
        setMultilingualOptions();

        $scope.ImagePath = config.BUILD_URL;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.buildURL = config.BUILD_URL;
        var userInfo = SessionService.getUser();
        $scope.RoleId = userInfo.RoleId;
        $scope.createdBy = userInfo.EmployeeId;

        if (ManifestId !== undefined && ManifestId !== null && ManifestId !== 0) {
            ExpressManifestService.ViewManifestDetail(ManifestId).then(function (response) {
                $scope.ManifestData = response.data;
            }, function (response) {
                if (response.status !== 401) {
                    toaster.pop({
                        type: "error",
                        title: $scope.FrayteError,
                        body: $scope.ErrorGettingRecord,
                        showCloseButton: true
                    });
                }
            });
        }
    }
    init();
});
