angular.module('ngApp.customer')
.controller("CustomerManifestController", function ($scope, uiGridConstants, $uibModal, toaster, $window, $stateParams, SessionService, CustomerService, $rootScope, $http, config, AppSpinner, $translate) {

    $scope.setManifestGrid = function () {
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
            enableGridMenu: true,
            enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
            enableVerticalScrollbar: true,

            //     rowTemplate: '<div ng-class="{ \'demoClass\':grid.appScope.rowFormatter( row ) }">' + '  <div ng-repeat="(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ng-class="{ \'ui-grid-row-header-cell\': col.isRowHeader }"  ui-grid-cell></div>' + '</div>',
            columnDefs: [
                  { name: 'ManifestName', displayName: 'Manifest_Number', width: '25%', enableFiltering: true, enableSorting: true, headerCellFilter: 'translate' },
                  { name: 'CourierDisplay', displayName: 'Courier', width: '20%', headerCellFilter: 'translate' },
                  { name: 'CreateOn', displayName: 'Manifest Date', width: '15%', headerCellFilter: 'translate' },
                  { name: 'NoOfShipments', displayName: 'No. of Shipments', width: '15%', headerCellFilter: 'translate' },
                  { name: 'TotalWeight', displayName: 'Total Weight (kgs)', width: '15%', headerCellFilter: 'translate' },
                  { name: 'Edit', displayName: '', enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "customer/customerManifest/customerManifestEditButton.tpl.html" }

            ]
        };
    };

    var setModalOptions = function () {
        $translate(['Downloaded_Status', 'Frayte_Error', 'Frayte_Success', 'Report_Generated_Successfully', 'Report_Status', 'Could_Not_Download_TheReport', 'No_Report_Generated']).then(function (translations) {

            $scope.DownloadedStatus = translations.Downloaded_Status;
            $scope.ReportGeneratedSuccessfully = translations.Report_Generated_Successfully;
            $scope.ReportStatus = translations.Report_Status;
            $scope.CouldNotDownloadTheReport = translations.Could_Not_Download_TheReport;
            $scope.NoReportGenerated = translations.No_Report_Generated;
            $scope.FrayteError = translations.Frayte_Error;
            $scope.FrayteSuccess = translations.Frayte_Success;



        });
    };


    //view customer popup
    $scope.ViewManifest = function (row) {
        var ModalInstance = $uibModal.open({
            animation: true,
            controller: 'CustomerManifestDetailController',
            resolve: {
                manifestId: function () {
                    return row.entity.ManifestId;
                },
                manifestObj: function () {
                    return row.entity;
                }
            },
            templateUrl: 'customer/customerManifest/viewManifest.tpl.html',
            backdrop: true,
            size: 'lg',
            keyboard: false

        });
    };

    //Download Manifest 

    $scope.DownloadmanifestReport = function (row) {
        if (row !== undefined && row !== null && row.entity !== null) {
            AppSpinner.showSpinnerTemplate("Downloading Direct Booking Manifest PDF", $scope.Template);
            CustomerService.GenerateManifest(row.ManifestId, $scope.TrackManifest.ModuleType, $scope.TrackManifest.UserId).then(function (response) {
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



    $rootScope.CustomerCreateManifest = function () {
        var ModalInstance = $uibModal.open({
            animation: true,
            controller: 'CreateManifestController',
            templateUrl: 'customer/customerManifest/createManifest/createManifest.tpl.html',
            resolve: {
                TrackObj: function () {
                    return $scope.TrackManifest;
                }
            },
            backdrop: true,
            size: 'lg',
            keyboard: false

        });
    };

    $rootScope.GetManifest = function (Searchtext) {
        AppSpinner.showSpinnerTemplate("Loading Manifests", $scope.Template);
        if (Searchtext !== 'Search')
        {
            $scope.TrackManifest.ModuleType = $scope.fryateShipmentTypes[0].BookingType;
        }
        
        CustomerService.GetManifestDetail($scope.TrackManifest).then(function (response) {

            $scope.gridOptions.data = response.data;
            if (response.data !== null && response.data.length > 0) {
                $scope.totalItemCount = response.data[0].TotalRows;
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
        $scope.GetManifest();
    };

    function init() {



        //  $scope.spinnerMessage = 'Downloading Manifest PDF';
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
        var userInfo = SessionService.getUser();
        $scope.TrackManifest.UserId = userInfo.EmployeeId;

        CustomerService.GetBookingTypes(userInfo.EmployeeId).then(function (response) {
            $scope.fryateShipmentTypes = response.data;
            $scope.GetManifest();
            if (response.data && response.data.length === 1) {
                $scope.IsDisable = true;
            }
            else {
                $scope.IsDisable = false;
            }
        }, function () {
            AppSpinner.hideSpinnerTemplate();
        });

        setModalOptions();
    }
    init();
});
