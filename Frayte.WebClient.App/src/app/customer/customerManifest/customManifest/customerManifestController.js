angular.module('ngApp.customer')
.controller("CustomManifestController", function ($scope, uiGridConstants, $uibModal, toaster, $window, $stateParams, SessionService, CustomerService, $rootScope, $http, config, AppSpinner, $translate) {

    $scope.setManifestGrid = function () {
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
                  { name: 'ManifestName', displayName: 'Manifest_Number', width: '30%', enableFiltering: true, enableSorting: true, headerCellFilter: 'translate' },
                  { name: 'ManifestDate', displayName: 'Manifest Date', width: '20%', headerCellFilter: 'translate' },
                  { name: 'NumberofShipments', displayName: 'No. of Shipments', width: '20%', headerCellFilter: 'translate' },
                  { name: 'TotalWeight', displayName: 'Total Weight (kgs)', width: '19%', headerCellFilter: 'translate' },
                  { name: 'Edit', displayName: '', enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "customer/customerManifest/customManifest/customerManifestEditButton.tpl.html" }
            ]
        };
    };

    var setModalOptions = function () {
        $translate(['Downloaded_Status', 'Frayte_Error', 'Frayte_Success', 'Report_Generated_Successfully', 'Report_Status', 'Could_Not_Download_TheReport', 'No_Report_Generated', 'ErrorGettingRecord',
        'DownloadingCustomManifestCSV', 'LoadingManifests']).then(function (translations) {
            $scope.DownloadedStatus = translations.Downloaded_Status;
            $scope.ReportGeneratedSuccessfully = translations.Report_Generated_Successfully;
            $scope.ReportStatus = translations.Report_Status;
            $scope.CouldNotDownloadTheReport = translations.Could_Not_Download_TheReport;
            $scope.NoReportGenerated = translations.No_Report_Generated;
            $scope.FrayteError = translations.Frayte_Error;
            $scope.FrayteSuccess = translations.Frayte_Success;
            $scope.ErrorGettingRecord = translations.ErrorGettingRecord;
            $scope.DownloadingCustomManifestCSV = translations.DownloadingCustomManifestCSV;
            $scope.LoadingManifests = translations.LoadingManifests;
        });
    };

    //view customer popup
    $scope.ViewManifest = function (row) {
        var ModalInstance = $uibModal.open({
            animation: true,
            controller: 'CustomManifestDetailController',
            resolve: {
                manifestId: function () {
                    return row.entity.ManifestId;
                },
                manifestObj: function () {
                    return row.entity;
                }
            },
            templateUrl: 'customer/customerManifest/customManifest/viewManifest.tpl.html',
            backdrop: true,
            size: 'lg',
            keyboard: false
        });
    };

    //Download Manifest 
    $scope.DownloadmanifestReport = function (row) {
        if (row !== undefined && row !== null && row.entity !== null) {
            AppSpinner.showSpinnerTemplate($scope.DownloadingCustomManifestCSV, $scope.Template);
            CustomerService.GenerateCustomManifest(row.ManifestId, row.ManifestName).then(function (response) {
                AppSpinner.hideSpinner();
                var fileInfo = response.data;
                var fileName = {
                    FileName: response.data.FileName,
                    FilePath: response.data.FilePath
                };
                if (response.data != null) {
                    $http({
                        method: 'POST',
                        url: config.SERVICE_URL + '/Manifest/DownloadCustomManifest',
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

    $rootScope.CustomerCreateManifest = function () {
        var ModalInstance = $uibModal.open({
            animation: true,
            controller: 'CreateCustomManifestController',
            templateUrl: 'customer/customerManifest/createManifest/createManifest.tpl.html',
            resolve: {
                TrackObj: function () {
                    return $scope.TrackManifest;
                }
            },
            backdrop: true,
            size: 'lg',
            windowClass: 'DirectBookingDetail',
            keyboard: false

        });
    };

    $scope.SearchManifest = function (Searchtext) {

        AppSpinner.showSpinnerTemplate($scope.LoadingManifests, $scope.Template);
        if (Searchtext !== 'Search') {
            $scope.TrackManifest.ModuleType = $scope.fryateShipmentTypes[0].BookingType;
        }

        CustomerService.GetCustomManifests($scope.TrackManifest).then(function (response) {

            for (i = 0; i < response.data.length; i++) {
                var newdate = [];
                var days = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
                newdate = new Date(response.data[i].ManifestDate);
                var gtDate = newdate.getDate();
                var gtDate1 = gtDate;
                var gtMonth = newdate.getMonth();
                var month1 = gtMonth;
                var getmn1 = days[month1];
                var gtYear = newdate.getFullYear();
                var nDate = gtDate1 + "-" + getmn1 + "-" + gtYear;
                response.data[i].ManifestDate = nDate;
            }

            $scope.gridOptions.data = response.data;
            if (response.data !== null && response.data.length > 0) {
                $scope.totalItemCount = response.data[0].TotalRows;
            }
            else {
                $scope.totalItemCount = 0;
            }
            AppSpinner.hideSpinnerTemplate();
        }, function () {
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

    $rootScope.GetManifest = function (Searchtext) {
        AppSpinner.showSpinnerTemplate($scope.LoadingManifests, $scope.Template);
        if (Searchtext !== 'Search') {
            $scope.TrackManifest.ModuleType = $scope.fryateShipmentTypes[0].BookingType;
        }

        CustomerService.GetCustomManifestDetail().then(function (response) {

            $scope.gridOptions.data = response.data;
            if (response.data !== null && response.data.length > 0) {
                $scope.totalItemCount = 101;
            }
            else {
                $scope.totalItemCount = 0;
            }
            for (i = 0; i < response.data.length; i++) {
                var date = new Date(response.data[i].CreateOn);
                var days = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
                var getdt = date.getDate();
                var getmn1 = days[date.getMonth()];
                var getyr = date.getFullYear();
                response.data[i].CreateOn = getdt + "-" + getmn1 + "-" + getyr;

            }
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
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
        $scope.SearchManifest();
    };

    function init() {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.setManifestGrid();
        if ($stateParams.moduleType !== undefined && $stateParams.moduleType !== null && $stateParams.moduleType !== '') {
            if ($stateParams.moduleType === "db") {
                $scope.moduleType = "DirectBooking";
            }
            else if ($stateParams.moduleType === "eCb") {
                $scope.moduleType = "eCommerce";
            }
        }
        else {
            $scope.moduleType = "DirectBooking";
        }

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
            ModuleType: '',
            CurrentPage: $scope.currentPage,
            TakeRows: $scope.itemsPerPage
        };

        $scope.TrackManifest.TakeRows = $scope.itemsPerPage;
        $scope.TrackManifest.CurrentPage = $scope.currentPage;
        var userInfo = SessionService.getUser();
        $scope.TrackManifest.UserId = userInfo.EmployeeId;

        $scope.fryateShipmentTypes = [{
            BookingType: 'eCommerce',
            BookingTypeDisplay: 'eCommerce'
        }];
           //$scope.GetManifest();
           $scope.SearchManifest();
           if ($scope.fryateShipmentTypes && $scope.fryateShipmentTypes.length === 1) {
               $scope.IsDisable = true;
           }
           else {
               $scope.IsDisable = false;
           }

        setModalOptions();
    }
    init();
});
