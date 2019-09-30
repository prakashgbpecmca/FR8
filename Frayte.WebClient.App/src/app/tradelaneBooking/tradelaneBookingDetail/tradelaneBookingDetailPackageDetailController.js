angular.module('ngApp.tradelaneBooking').controller('TradelaneBookingDetailPacakgeDetailController', function ($scope, UtilityService, ModalService, $rootScope, TradelaneBookingService, $http, FrayteNumber, $state, ShipmentId, PackageCalculatonType, HAWB, HAWBNumber, TotalUploaded, SuccessUploaded, ScreenType, $translate, $uibModalInstance, AppSpinner, config, $filter, SessionService, ShipmentService, Upload, $timeout, toaster) {

    $scope.closePage = function () {
        $uibModalInstance.close();
    };

    //multilingual translation key
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteSuccess', 'ThereIsNoUnassignedJob', 'Confirmation', 'NoAssignedHAWBSureCancel', 'SelectPackageFirstAssignHAWB',
        'ErrorAssigningHAWBTryagain', 'SuccessfullyAssignedHAWB', 'Loading_Shipments', 'ErrorGettingjobs', 'Downloading_Document', 'Successfully_Download_Document',
        'Couldnot_Download_Document', 'Loading_HAWB', 'Loading_Preview_HAWB']).then(function (translations) {
            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.FrayteError = translations.FrayteError;
            $scope.Confirmation = translations.Confirmation;
            $scope.NoAssignedHAWBSureCancel = translations.NoAssignedHAWBSureCancel;
            $scope.SelectPackageFirstAssignHAWB = translations.SelectPackageFirstAssignHAWB;
            $scope.ThereIsNoUnassignedJob = translations.ThereIsNoUnassignedJob;
            $scope.ErrorAssigningHAWBTryagain = translations.ErrorAssigningHAWBTryagain;
            $scope.SuccessfullyAssignedHAWB = translations.SuccessfullyAssignedHAWB;
            $scope.Loading_Shipments = translations.Loading_Shipments;
            $scope.Downloading_Document = translations.Downloading_Document;
            $scope.Successfully_Download_Document = translations.Successfully_Download_Document;
            $scope.Couldnot_Download_Document = translations.Couldnot_Download_Document;
            $scope.Loading_HAWB = translations.Loading_HAWB;
            $scope.DocumentUploadedSuccessfully = translations.DocumentUploadedSuccessfully;
            $scope.Loading_Preview_HAWB = translations.Loading_Preview_HAWB;
             
            if ($scope.screenType === 'ShipmentDetail') { 
                $scope.loadingMessage = $scope.Loading_HAWB; 
            }
            else {
                $scope.loadingMessage = $scope.Loading_Preview_HAWB;
            }

            if ($scope.screenType === 'PreviewHAWB') {
                $scope.track.Type = "Detail";
                getShipmentHAWB();
            }
            else {
                getScreenInitials();
            }
        });
    };

    $scope.closePopUp = function () {
        $uibModalInstance.close();
    };

    $scope.dowonLoadDoc = function (docType, docTypeName) {
        $rootScope.GetServiceValue = null;
        AppSpinner.showSpinnerTemplate($scope.Downloading_Document, $scope.Template);

        TradelaneBookingService.CreateDocument($scope.shipmentId, $scope.userInfo.EmployeeId, docType, docTypeName).then(function (response) {
            if (response.data !== null) {
                var fileInfo = response.data;
                var File = {
                    TradelaneShipmentId: $scope.shipmentId,
                    FileName: response.data.FileName
                };
                if (response.data != null) {
                    $http({
                        method: 'POST',
                        url: config.SERVICE_URL + '/TradelaneShipments/DownloadDocument',
                        data: File,
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

                            } catch (ex) {
                                AppSpinner.hideSpinnerTemplate();
                                $window.open(fileInfo.FilePath, "_blank");
                                toaster.pop({
                                    type: 'success',
                                    title: $scope.FrayteSuccess,
                                    body: $scope.Successfully_Download_Document,
                                    showCloseButton: true
                                });
                            }

                        }
                    })
                   .error(function (data) {
                       AppSpinner.hideSpinnerTemplate();
                       toaster.pop({
                           type: 'error',
                           title: $scope.FrayteError,
                           body: $scope.Couldnot_Download_Document,
                           showCloseButton: true
                       });
                   });

                }
                else {
                }
            }

            AppSpinner.hideSpinnerTemplate();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.Couldnot_Download_Document,
                showCloseButton: true
            });
        });
    };

    var getScreenInitials = function () {
        AppSpinner.showSpinnerTemplate($scope.loadingMessage, $scope.Template);
        TradelaneBookingService.getShipmentPackages($scope.track).then(function (response) {
            if (response.data) {
                if (response.data.length) {
                    $scope.packages = response.data;
                    $scope.totalItemCount = response.data[0].TotalRows;

                    TradelaneBookingService.GetHAWBAddress($scope.track.HAWB).then(function (response) {
                        if (response.data !== undefined && response.data !== null && response.data !== '') {
                            $scope.ShipTo = response.data.ShipTo;
                            $scope.ShipFrom = response.data.ShipFrom;                            
                            $scope.NotifyParty = response.data.NotifyParty;
                            $scope.IsNotifyPartySameAsReceiver = response.data.IsNotifyPartySameAsReceiver;
                        }
                    });
                } else {

                }
            }
            else {
            }
            AppSpinner.hideSpinnerTemplate();
        }, function (response) {

            AppSpinner.hideSpinnerTemplate();
            if (response.status !== 401) {

            }
        });
    };


    $scope.pageChanged = function () {
        $scope.selectAll = false;
        getScreenInitials("PageChanged");
    };
    $scope.changePagination = function () {
        $scope.selectAll = false;
        $scope.pageChanged();
    };

    $scope.selectAllPackage = function () {
        angular.forEach($scope.packages, function (obj) {
            obj.IsSelected = $scope.selectAll;
        });
    };


    var getShipmentHAWB = function () {
        $scope.hawbs = [];
        TradelaneBookingService.ShipmentHAWB($scope.shipmentId).then(function (response) {
            if (response.data && response.data.length) {
                $scope.hawbs = response.data;
                $scope.track.HAWB = response.data[0];
                getScreenInitials();
            }
            else {

            }
        }, function () {

        });
    };


    $scope.hawbChanged = function () {
        getScreenInitials("PageChanged");
    };

    function init() {

        $scope.userInfo = SessionService.getUser();
        $scope.shipmentId = ShipmentId;
        $scope.FrayteNumber = FrayteNumber;
        $scope.PackageCalculatonType = PackageCalculatonType;
        if (HAWB) {
            $scope.type = "Detail";

            if (HAWB === 'UnAllocated') {
                $scope.hawb = '';
            }
            else {
                $scope.hawb = HAWB;
            }
        }
        else {
            $scope.hawb = '';
            $scope.type = "Main";
        }

        if (HAWBNumber) {
            $scope.shipmentHAWBs = HAWBNumber;
            $scope.createHAWB(true);
        }
        else {
            $scope.shipmentHAWBs = 0;
        }

        if (ScreenType) {
            $scope.screenType = ScreenType;
        }
        else {
            $scope.screenType = "";
        }
         
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
         
        // Pagination Logic 
        $scope.viewby = 20;
        $scope.currentPage = 1;
        $scope.itemsPerPage = $scope.viewby;
        $scope.totalItemCount = 1000;
        $scope.maxSize = 2; //Number of pager buttons to show
        $scope.numbers = [$scope.viewby, 30, 50, 100];

        // Track obj
        $scope.track = {
            HAWB: $scope.hawb,
            Type: $scope.type,
            ShipmentId: $scope.shipmentId,
            CurrentPage: $scope.currentPage,
            TakeRows: $scope.itemsPerPage
        };
       
        setMultilingualOptions();
    }

    init();
});