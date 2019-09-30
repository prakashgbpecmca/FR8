angular.module('ngApp.customer')
.controller("UserManifestController", function ($scope, uiGridConstants, $uibModal, toaster, $window, $stateParams, SessionService, CustomerService, $rootScope, $http, config, AppSpinner, $translate, $state, DirectBookingService) {

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
                              { name: 'ManifestName', displayName: 'Manifest_Number', width: '20%', enableFiltering: true, enableSorting: true, headerCellFilter: 'translate' },
                              { name: 'Customer', displayName: 'Customer', width: '18%', headerCellFilter: 'translate' },
                              { name: 'CourierDisplay', displayName: 'Courier', width: '15%', headerCellFilter: 'translate' },
                              { name: 'CreateOn', displayName: 'Manifest_Date', width: '12%', headerCellFilter: 'translate' },
                              { name: 'NoOfShipments', displayName: 'No_of_Shipments', width: '12%', headerCellFilter: 'translate' },
                              { name: 'TotalWeight', displayName: 'Total_Weight_kgs', width: '13%', headerCellFilter: 'translate' },
                              { name: 'Edit', displayName: '', enableFiltering: false, enableSorting: false, enableGridMenuenableRowSelection: false, noUnselect: false, cellTemplate: "customer/customerManifest/userManifest/customerManifestEditButton.tpl.html" }
            ]
        };
    };

    var setModalOptions = function () {
        $translate(['Downloaded_Status', 'Frayte_Error', 'Frayte_Success', 'FrayteWarning', 'Report_Generated_Successfully', 'Report_Status', 'Could_Not_Download_TheReport', 'No_Report_Generated', 'ErrorGettingRecord',
                    'LoadingManifests', 'DownloadingDirectBookingManifestPDF', 'DownloadingDirectBookingManifestExcel', 'From_Date_Required', 'To_Date_Required']).then(function (translations) {
                        $scope.DownloadedStatus = translations.Downloaded_Status;
                        $scope.FrayteError = translations.Frayte_Error;
                        $scope.FrayteSuccess = translations.Frayte_Success;
                        $scope.Frayte_Warning = translations.FrayteWarning;
                        $scope.ReportGeneratedSuccessfully = translations.Report_Generated_Successfully;
                        $scope.ReportStatus = translations.Report_Status;
                        $scope.CouldNotDownloadTheReport = translations.Could_Not_Download_TheReport;
                        $scope.NoReportGenerated = translations.No_Report_Generated;
                        $scope.ErrorGettingRecord = translations.ErrorGettingRecord;
                        $scope.From_Date = translations.From_Date_Required;
                        $scope.To_Date = translations.To_Date_Required;
                        $scope.Loading_Manifests = translations.LoadingManifests;
                        $scope.DownloadingDirectBookingManifestPDF = translations.DownloadingDirectBookingManifestPDF;
                        $scope.DownloadingDirectBookingManifestExcel = translations.DownloadingDirectBookingManifestExcel;
                        $scope.bookingType();
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
            templateUrl: 'customer/customerManifest/userManifest/viewManifest.tpl.html',
            backdrop: true,
            size: 'lg',
            keyboard: false

        });
    };

    $scope.ManifestTrackingPopup = function (row) {
        var ModalInstance = $uibModal.open({
            animation: true,
            controller: 'ManifestTrackingController',
            resolve: {
                manifestObj: function () {
                    return row;
                }
            },
            templateUrl: 'customer/customerManifest/userManifest/manifestTracking/manifestTracking.tpl.html',
            backdrop: true,
            size: 'md',
            keyboard: false
        });
    };

    //Download Manifest 
    $scope.DownloadmanifestReport = function (row) {
        if (row !== undefined && row !== null && row.entity !== null) {
            AppSpinner.showSpinnerTemplate($scope.DownloadingDirectBookingManifestPDF, $scope.Template);
            CustomerService.GenerateManifest(row.ManifestId, $scope.TrackManifest.ModuleType, $scope.createdBy, $scope.RoleId).then(function (response) {
                AppSpinner.hideSpinner();
                var fileInfo = response.data;
                var fileName = {
                    FileName: response.data.FileName,
                    ModuleType: $scope.TrackManifest.ModuleType
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

    //Download Manifest 
    $scope.DownloadmanifestExcel = function (row) {
        if (row !== undefined && row !== null) {
            AppSpinner.showSpinnerTemplate($scope.DownloadingDirectBookingManifestExcel, $scope.Template);
            CustomerService.GenerateManifestExcel(row.ManifestId).then(function (response) {
                AppSpinner.hideSpinner();
                var fileInfo = response.data;
                var fileName = {
                    FileName: response.data.FileName,
                    ModuleType: $scope.TrackManifest.ModuleType
                };
                if (response.data != null) {
                    $http({
                        method: 'POST',
                        url: config.SERVICE_URL + '/Manifest/DownloadManifest',
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
            templateUrl: 'customer/customerManifest/userManifest/createManifest/createManifest.tpl.html',
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

    $rootScope.GetManifest = function (Searchtext) {
        AppSpinner.showSpinnerTemplate($scope.Loading_Manifests, $scope.Template);

        if (($scope.TrackManifest.ToDate === '' || $scope.TrackManifest.ToDate === null || $scope.TrackManifest.ToDate === undefined) && ($scope.TrackManifest.FromDate === '' || $scope.TrackManifest.FromDate === null || $scope.TrackManifest.FromDate === undefined)) {
            AppSpinner.hideSpinnerTemplate();
        }
        else if (($scope.TrackManifest.ToDate !== null || $scope.TrackManifest.ToDate !== '' || $scope.TrackManifest.ToDate !== undefined) && ($scope.TrackManifest.FromDate === undefined || $scope.TrackManifest.FromDate === '' || $scope.TrackManifest.FromDate === null)) {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.From_Date,
                showCloseButton: true
            });
            AppSpinner.hideSpinnerTemplate();
            return;
        }
        else if (($scope.TrackManifest.ToDate === null || $scope.TrackManifest.ToDate === '' || $scope.TrackManifest.ToDate === undefined) && ($scope.TrackManifest.FromDate !== undefined || $scope.TrackManifest.FromDate !== '' || $scope.TrackManifest.FromDate !== null)) {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
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
                        title: $scope.Frayte_Warning,
                        body: $scope.ToDateValidation,
                        showCloseButton: true
                    });
                    return;
                }
            }
        }

        CustomerService.GetManifestDetail($scope.TrackManifest).then(function (response) {

            $scope.gridOptions.data = response.data;
            if (response.data !== null && response.data.length > 0) {
                $scope.totalItemCount = response.data[0].TotalRows;
            }
            else {
                $scope.totalItemCount = 0;
            }
            if (response.data != null && response.data.length > 0) {
                for (i = 0; i < response.data.length; i++) {
                    var date = new Date(response.data[i].CreateOn);
                    var days = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
                    var getdt = date.getDate();
                    var getmn1 = days[date.getMonth()];
                    var getyr = date.getFullYear();
                    response.data[i].CreateOn = getdt + "-" + getmn1 + "-" + getyr;

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
            $scope.bookingType();
            $rootScope.isShow = true;
            $scope.GetCustomerDetailInfo();
        }
    };

    $scope.GetCustomerDetailInfo = function () {
        if ($scope.CustomerId !== undefined && $scope.CustomerId !== null && $scope.CustomerId !== '') {
            CustomerService.GetCustomerDetail($scope.CustomerId).then(function (response) {
                $scope.CustomerDetail = response.data;
                if ($scope.CustomerDetail) {
                    if ($scope.TrackManifest.ModuleType !== 'DirectBooking') {
                        if ($scope.CustomerDetail.IsWithoutService === false) {
                            $scope.eCommerceShipmentTypes.splice(0, 1);
                            $scope.TrackManifest.subModuleType = $scope.eCommerceShipmentTypes[0].value;
                        }
                        else if ($scope.CustomerDetail.IsServiceSelected === false) {
                            $scope.eCommerceShipmentTypes.splice(1, 1);
                            $scope.TrackManifest.subModuleType = $scope.eCommerceShipmentTypes[0].value;
                        }
                        else {
                            $scope.TrackManifest.subModuleType = $scope.eCommerceShipmentTypes[0].value;
                        }
                    }
                }
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
        }
        else {

        }
    };

    $scope.bookingType = function () {
        if ($scope.CustomerId !== undefined && $scope.CustomerId !== null && $scope.CustomerId !== '' && $scope.CustomerId > 0) {
            CustomerService.GetBookingTypes($scope.CustomerId).then(function (response) {
                $scope.fryateShipmentTypes = response.data;
                $scope.GetManifest();
                if (response.data && response.data.length === 1) {
                    $scope.IsDisable = true;
                }
                else {
                    $scope.IsDisable = false;
                }
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
        }
        else {
            $scope.bookship = [];
            $scope.bookingshipment = {
                BookingType: "",
                BookingTypeDisplay: ""
            };

            $scope.bookingshipment.BookingType = 'DirectBooking';
            $scope.bookingshipment.BookingTypeDisplay = 'Direct Booking';
            $scope.bookship.push($scope.bookingshipment);
            $scope.fryateShipmentTypes = $scope.bookship;
            $scope.moduleType = $scope.fryateShipmentTypes[0].BookingTypeDisplay;
            $scope.IsDisable = true;
            $scope.GetManifest();
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

    function init() {
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
        $scope.eCommerceShipmentTypes = [{
            value: '',
            display: 'All'
        }, {
            value: 'ECOMMERCE_WS',
            display: 'Without Service_CSV'
        },
        {
            value: 'ECOMMERCE_SS',
            display: 'Service Select_CSV'
        },
        {
            value: 'ECOMMERCE_ONL',
            display: 'Manual Booking'
        }];
        $scope.subModuleType = "ECOMMERCE_ONL";

        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.setManifestGrid();
        if ($stateParams.moduleType) {
            if ($stateParams.moduleType === "db") {
                $scope.moduleType = "DirectBooking";
                $rootScope.rootModuleType = "DirectBooking";
            }
            else if ($stateParams.moduleType === "eCb") {
                $scope.moduleType = "eCommerce";
                $rootScope.rootModuleType = "eCommerce";
            }
        }
        else {
            $scope.moduleType = SessionService.getModuleType();
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
            if ($rootScope.ChangeManifest === true) {
                $scope.CustomerCreateManifest();
                $rootScope.ShowManifestButton = false;
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
            if ($rootScope.ChangeManifest === true) {
                $scope.CustomerCreateManifest();
                $rootScope.ShowManifestButton = false;
            }
            else {
                $rootScope.ShowManifestButton = false;
            }
        }

        if ($scope.RoleId === 6 || $scope.RoleId === 1) {
            //CustomerService.GetManifestCustomerList().then(function (response) {
            DirectBookingService.GetDirectBookingCustomers(userInfo.EmployeeId, "DirectBooking").then(function (response) {
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
                    $scope.ManifestCustomers.unshift({ CustomerId: 0, CompanyName: 'All' });
                    $scope.CustomerDetail = $scope.ManifestCustomers[0];
                    $scope.CustomerId = $scope.ManifestCustomers[0].CustomerId;
                    $scope.TrackManifest.UserId = $scope.ManifestCustomers[0].CustomerId;
                    $scope.bookingType();
                    $rootScope.isShow = true;
                    if ($rootScope.ChangeManifest === true) {
                        $scope.CustomerCreateManifest();
                        $rootScope.ShowManifestButton = false;
                    }
                    else {
                        $rootScope.ShowManifestButton = false;
                    }
                }
            });
            $scope.TrackManifest.CreatedBy = userInfo.EmployeeId;
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

        if ($scope.CustomerId !== null && $scope.CustomerId !== undefined && $scope.CustomerId !== '') {
            $scope.GetCustomerDetailInfo();
        }
        else {

        }

        if (userInfo === undefined || userInfo === null || userInfo.SessionId === undefined || userInfo.SessionId === '') {
            $state.go('home.welcome');
        }
        else {
            $scope.userInfo = userInfo;
            $scope.tabs = $scope.userInfo.tabs;
        }

        setModalOptions();

        //userInfo.EmployeeId
        if ($scope.CustomerId !== undefined && $scope.CustomerId !== null && $scope.CustomerId !== '') {
            $scope.bookingType();
        }
        else {

        }
    }

    init();
});