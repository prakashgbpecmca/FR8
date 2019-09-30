angular.module('ngApp.express').controller("ExpressShipmentController", function ($scope, $state, $uibModal, ModalService, config, SessionService, AppSpinner, $rootScope, toaster, ExpressShipmentService, DateFormatChange, $translate, $http, $window) {

    //express shipment detail code here
    $scope.expressShipmentDetail = function (row) {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'ExpressDetailController',
            templateUrl: 'express/expressDetail/expressShipmentDetail.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            resolve: {
                ShipmentId: function () {
                    return row.ExpressShipmentId;
                }
            }
        });
    };

    $scope.expressShipmentClone = function (row) {
        $state.go("loginView.userTabs.express-solution-booking-clone", { shipmentId: row.ExpressShipmentId });
    };

    //Return shipment code

    //$scope.CallingType = 'ShipmentReturn';

    $scope.expressReturnShipmentClone = function (row) {
        $state.go("loginView.userTabs.express-solution-booking-return", { shipmentId: row.ExpressShipmentId });
    };

    //end

    //Set Multilingual code here
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteWarning', 'FrayteSuccess', 'ReceiveDetailValidation', 'GettingDataErrorValidation', 'To_Date', 'ToDateValidation', 'From_Date', 'SuccessfullyDeletedDraftShipment',
            'ShipmentDraftDeleteConfirmation', 'SureDeleteShipmentDraft', 'From_Date_Required', 'To_Date_Required', 'Select_Date_To', 'Select_Date_From', 'LoadingTrackTrace', 'Loading_Shipments',
            'Tracking_Code', 'ReportCannotDownloadPleaseTryAgain','DownloadingTrackTraceExcel']).then(function (translations) {
            $scope.FrayteError = translations.FrayteError;
            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.ReceiveDetailValidation = translations.ReceiveDetailValidation;
            $scope.GettingDataErrorValidation = translations.GettingDataErrorValidation;
            $scope.To_Date = translations.To_Date;
            $scope.ToDateValidation = translations.ToDateValidation;
            $scope.From_Date = translations.From_Date;
            $scope.SuccessfullyDeletedDraftShipment = translations.SuccessfullyDeletedDraftShipment;
            $scope.ShipmentDraftDeleteConfirmation = translations.ShipmentDraftDeleteConfirmation;
            $scope.SureDeleteShipmentDraft = translations.SureDeleteShipmentDraft;
            $scope.Select_Date_To = translations.Select_Date_To;
            $scope.Select_Date_From = translations.Select_Date_From;
            $scope.LoadingTrackTrace = translations.LoadingTrackTrace;
            $scope.Loading_Shipments = translations.Loading_Shipments;
            $scope.From_Date_Required = translations.From_Date_Required;
            $scope.To_Date_Required = translations.To_Date_Required;
            $scope.Tracking_Code = translations.Tracking_Code;
            $scope.ReportCannotDownloadPleaseTryAgain = translations.ReportCannotDownloadPleaseTryAgain;
            $scope.DownloadingTrackTraceExcel = translations.DownloadingTrackTraceExcel;
        });
    };
    //end

    $scope.DownLoadExpressReport = function () {
        if ($scope.track.CustomerName === null || $scope.track.CustomerName === undefined || $scope.track.CustomerName === "") {
            $scope.track.CustomerName = "ALL";
        }
        if ($scope.CurrentStatus.DisplayStatusName === 'All') {
            $scope.track.ShipmentStatusId = 0;
        }
        else {
            $scope.track.ShipmentStatusId = $scope.CurrentStatus.ShipmentStatusId;
        }

        $scope.track.RoleId = $scope.$UserRoleId;
        AppSpinner.showSpinnerTemplate($scope.DownloadingTrackTraceExcel, $scope.Template);

        ExpressShipmentService.GenerateTrackAndTraceReport($scope.track).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            if (response.data !== null && response.data !== undefined && response.data !== '' && response.data.FileStatus === true) {
                var fileInfo = response.data;
                var fileName = {
                    FileName: response.data.FileName
                };

                $http({
                    method: 'POST',
                    url: config.SERVICE_URL + '/Express/DownloadExpressTrackAndTraceReport',
                    data: fileName,
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
                       title: $scope.Frayte_Error,
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
                title: $scope.Frayte_Error,
                body: $scope.ReportCannotDownloadPleaseTryAgain,
                showCloseButton: true
            });
        });
    };

    $scope.bookingStatus = function (Status) {
        $scope.CurrentStatus = Status;
        $scope.track.ShipmentStatusId = $scope.CurrentStatus.ShipmentStatusId;
        if ($scope.track.CurrentPage === 1) {

        }
        else {
            $scope.track.CurrentPage = 1;
        }
        $scope.DirectShipments();
    };

    $scope.ChangeSpecialSearch = function (SpecialSearch) {
        //$scope.CurrentStatus = Status;
        $scope.track.SpecialSearchId = SpecialSearch.SpecialSearchId;
        $scope.DirectShipments();
    };

    $scope.SeachDirectShipments = function () {
        $scope.DirectShipments();
    };

    $scope.LoadShipmentStatus = function () {
        //  getUserId();
        if ($scope.track.UserId != null && $scope.track.UserId !== undefined && $scope.track.UserId !== 0) {
            $scope.UserID = $scope.track.UserId;
        }
        else if ($scope.track.CustomerId !== null && $scope.track.CustomerId !== undefined && $scope.track.CustomerId !== 0) {
            $scope.UserID = $scope.track.CustomerId;
        }

        ExpressShipmentService.GetExpressStatusList("Express").then(function (response) {
            $scope.DirectShipmentStatus = response.data;
            ObjAll = {
                BookingType: "Express",
                DisplayStatusName: "All",
                ShipmentStatusId: 0,
                StatusName: "All"
            };
            $scope.DirectShipmentStatus.unshift(ObjAll);
            $scope.DirectShipmentStatus.sort(function (a, b) {
                if (a.StatusName < b.StatusName) { return -1; }
                if (a.StatusName > b.StatusName) { return 1; }
                return 0;
            });
            var DSS;
            for (var i = 0 ; i < $scope.DirectShipmentStatus.length; i++) {
                if ($scope.DirectShipmentStatus[i].StatusName === "Draft") {
                    $scope.DirectShipmentStatus[i].ImgURL = "draftImg.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "All") {
                    $scope.DirectShipmentStatus[i].ImgURL = "all.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "Current") {
                    $scope.DirectShipmentStatus[i].ImgURL = "currentImg.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "InTransit") {
                    $scope.DirectShipmentStatus[i].ImgURL = "T-intransit.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "CarrierDelivered") {
                    $scope.DirectShipmentStatus[i].ImgURL = "T-delivered.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "Delivered") {
                    $scope.DirectShipmentStatus[i].ImgURL = "T-delivered.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "Scanned") {
                    $scope.DirectShipmentStatus[i].ImgURL = "D-pending.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "Departed") {
                    $scope.DirectShipmentStatus[i].ImgURL = "D-departed.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "Rejected") {
                    $scope.DirectShipmentStatus[i].ImgURL = "D-rejected.png";
                }
                else if ($scope.DirectShipmentStatus[i].StatusName === "Arrived") {
                    $scope.DirectShipmentStatus[i].ImgURL = "D-arrived.png";
                } else if ($scope.DirectShipmentStatus[i].StatusName === "HubReceived") {
                    $scope.DirectShipmentStatus[i].ImgURL = "Hub.png";
                } else if ($scope.DirectShipmentStatus[i].StatusName === "ShipmentReturn") {
                    $scope.DirectShipmentStatus[i].ImgURL = "D-shipmentReturn.png";
                }
            }

            $scope.CurrentStatus = $scope.DirectShipmentStatus[0];
            $scope.track.ShipmentStatusId = $scope.CurrentStatus.ShipmentStatusId;
            $scope.DirectShipments();
        }, function (response) {
            if (response.status !== 401) {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.GettingDataErrorValidation,
                    showCloseButton: true
                });
            }
        });
    };

    $scope.DeleteDraft = function (ExpressShipmentId) {
        var modalOptions = {
            headerText: $scope.ShipmentDraftDeleteConfirmation,
            bodyText: $scope.SureDeleteShipmentDraft
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            ExpressShipmentService.DeleteExpressShipment(ExpressShipmentId).then(function (response) {
                if (response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.SuccessfullyDeletedDraftShipment,
                        showCloseButton: true
                    });
                    $scope.DirectShipments();
                }

            }, function (response) {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.GettingDataErrorValidation,
                    showCloseButton: true
                });
            });
        });

    };

    $scope.DirectShipments = function () {

        if (($scope.track.ToDate === '' || $scope.track.ToDate === null || $scope.track.ToDate === undefined) && ($scope.track.FromDate === '' || $scope.track.FromDate === null || $scope.track.FromDate === undefined)) {

        }
        else if (($scope.track.ToDate !== null || $scope.track.ToDate !== '' || $scope.track.ToDate !== undefined) && ($scope.track.FromDate === undefined || $scope.track.FromDate === '' || $scope.track.FromDate === null)) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.From_Date_Required,
                showCloseButton: true
            });
            return;
        }
        else if (($scope.track.ToDate === null || $scope.track.ToDate === '' || $scope.track.ToDate === undefined) && ($scope.track.FromDate !== undefined || $scope.track.FromDate !== '' || $scope.track.FromDate !== null)) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.To_Date_Required,
                showCloseButton: true
            });
            return;
        }
        else {
            if (($scope.track.FromDate !== undefined || $scope.track.FromDate !== null || $scope.track.FromDate !== "") && ($scope.track.ToDate !== undefined || $scope.track.ToDate !== null || $scope.track.ToDate !== "")) {
                var fromDate = new Date($scope.track.FromDate);
                var toDate = new Date($scope.track.ToDate);
                var from = (fromDate.getDate().toString().length === 1 ? "0" + fromDate.getDate() : fromDate.getDate()) + '/' + ((fromDate.getMonth() + 1).toString().length === 1 ? "0" + (fromDate.getMonth() + 1) : (fromDate.getMonth() + 1)) + '/' + fromDate.getFullYear();
                var to = (toDate.getDate().toString().length === 1 ? "0" + toDate.getDate() : toDate.getDate()) + '/' + ((toDate.getMonth() + 1).toString().length === 1 ? "0" + (toDate.getMonth() + 1) : (toDate.getMonth() + 1)) + '/' + toDate.getFullYear();
                var dateOne = new Date(from);
                var dateTwo = new Date(to);
                if (new Date(dateOne) > new Date(dateTwo)) {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteWarning,
                        body: $scope.ToDateValidation,
                        showCloseButton: true
                    });
                    return;
                }
            }
        }

        AppSpinner.showSpinnerTemplate($scope.Loading_Shipments, $scope.Template);
        ExpressShipmentService.GetExpressShipments($scope.track).then(function (response) {
            if (response.data !== null && response.data.length > 0) {
                $scope.totalItemCount = response.data[0].TotalRows;
            }
            else {
                $scope.totalItemCount = 0;
            }
            for (i = 0; i < response.data.length; i++) {
                response.data[i].CreatedOn = DateFormatChange.DateFormatChange(response.data[i].CreatedOn);
            }
            $scope.ExpressShipmentList = response.data;
            AppSpinner.hideSpinnerTemplate();
        }, function (error) {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.GettingDataErrorValidation,
                showCloseButton: true
            });
        });
    };

    $scope.currentImgURL = function (row) {
        if (row !== undefined) {
            var url = '';
            for (var i = 0; i < $scope.DirectShipmentStatus.length; i++) {
                if (row.Status == $scope.DirectShipmentStatus[i].StatusName) {
                    url = $scope.DirectShipmentStatus[i].ImgURL;
                    break;
                }
            }
            return url;
        }
    };

    $scope.GetCustomerName = function (Customer) {
        //$scope.Customer = Customer;
        if (Customer !== null && Customer !== undefined) {
            $scope.track.CustomerId = Customer.CustomerId;
            $scope.track.CustomerName = Customer.CustomerName;
        }
        else {
            $scope.track.CustomerId = 0;
            $scope.track.CustomerName = "ALL";
        }
    };

    var getCustomers = function () {
        //  getUserId();
        ExpressShipmentService.GetExpressCustomers($scope.$UserRoleId, $scope.UserId).then(function (response) {
            $scope.directBookingCustomers = response.data;
            for (i = 0; i < $scope.directBookingCustomers.length; i++) {
                var dbr = $scope.directBookingCustomers[i].AccountNumber.split("");
                var accno = "";
                for (var j = 0; j < dbr.length; j++) {
                    accno = accno + dbr[j];
                    if (j == 2 || j == 5) {
                        accno = accno + "-";
                    }
                }
                $scope.directBookingCustomers[i].AccountNumber = accno;
            }

        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ReceiveDetailValidation,
                showCloseButton: true
            });
        });
    };

    $scope.toggleMin = function () {
        $scope.dateOptions.minDate = $scope.dateOptions.minDate ? null : new Date();
    };

    $scope.toggleMin1 = function () {
        $scope.dateOptions1.minDate = $scope.dateOptions1.minDate ? null : new Date();
    };

    $scope.ChangeFromdate = function (FromDate) {
        $scope.track.FromDate = $scope.SetTimeinDateObj(FromDate);
        if ($scope.track.ToDate === undefined || $scope.track.ToDate === '' || $scope.track.ToDate === null) {
            $scope.track.ToDate = new Date();
            $scope.track.ToDate1 = new Date();
        }
    };

    $scope.ChangeTodate = function (ToDate) {
        $scope.track.ToDate = $scope.SetTimeinDateObj(ToDate);
    };

    $scope.pageChanged = function (track) {
        $scope.DirectShipments();
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

    $scope.openCalender = function ($event) {
        $scope.status.opened = true;
    };

    $scope.openCalender1 = function ($event) {
        $scope.status1.opened = true;
    };

    $scope.status1 = {
        opened: false
    };

    $scope.options = {
        maxDate: new Date(),
        showWeeks: true
    };

    $scope.status = {
        opened: false
    };

    $scope.$watch('track.ToDate', function () {
        if ($scope.track.ToDate !== undefined && $scope.track.ToDate !== null && $scope.track.ToDate !== "" && $scope.track.ToDate.getFullYear() === 1970) {
            $scope.track.ToDate = null;
        }

    });

    $scope.$watch('track.FromDate', function () {
        if ($scope.track.FromDate !== undefined && $scope.track.FromDate !== null && $scope.track.FromDate !== "" && $scope.track.FromDate.getFullYear() === 1970) {
            $scope.track.FromDate = null;
        }
    });

    //view confirm purchase order detail code
    $scope.viewConfirmPurchaseOrder = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            //controller: 'customerAddressBookAddEditController',
            templateUrl: 'breakbulk/details/viewConfirmPurchaseOrder.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end

    //breakbulkBookingCustomers code here
    $scope.breakbulkBookingCustomers = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'selectCustomersController',
            templateUrl: 'breakbulk/selectCustomers/selectCustomers.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end

    //breakbulk purchase order detail code
    $scope.purchaseOrderDetail = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            //controller: 'customerAddressBookAddEditController',
            templateUrl: 'breakbulk/details/breakbulkPurchaseOrderDetail.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end

    //breakbulk shipment detail code here
    $scope.breakbulkShipmentDetail = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'ExpressDetailController',
            templateUrl: 'express/expressDetail/expressShipmentDetail.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            resolve: {
                BagId: function () {
                    return BagId;
                }
            }
        });
    };
    //end

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

    //express shipment detail code here
    $scope.ExpressTracking = function (row) {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'expressBagTrackingController',
            templateUrl: 'express/expressTracking/expressBagTracking.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            resolve: {
                BagId: function () {
                    return row.BagId;
                }
            }
        });
    };

    //function for update tracking code
    $scope.updateTrackingPopup = function (shipment) {
        ModalInstance = $uibModal.open({
            Animation: true,
            templateUrl: 'tradelaneShipments/tradelaneUpdateTracking/tradelaneUpdateTracking.tpl.html',
            controller: 'TradelaneUpdateTrackingController',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                ShipmentInfo: function () {
                    return shipment;
                },
                PopupType: function () {
                    return "View";
                }
            }
        });
    };
    //end code

    function init() {
        setMultilingualOptions();
        $scope.ImagePath = config.BUILD_URL;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';

        $scope.dateOptions = {
            formatYear: 'yy',
            minDate: new Date(),
            maxDate: new Date(),
            dateDisabled: function (data) {
                var date = data.date,
                mode = data.mode;
                if ($scope.track.ToDate !== undefined && $scope.track.ToDate !== "" && $scope.track.ToDate !== null && $scope.track.ToDate.getDate() === date.getDate()) {
                    return mode === 'day' && false;
                }
                return mode === 'day' && (($scope.track.ToDate !== undefined && $scope.track.ToDate !== null && $scope.track.ToDate !== "") && date > $scope.track.ToDate);
            }
        };

        $scope.dateOptions1 = {
            formatYear: 'yy',
            minDate: new Date(),
            maxDate: new Date(),
            dateDisabled: function (data) {
                var date = data.date,
                mode = data.mode;
                if ($scope.track.FromDate !== undefined && $scope.track.FromDate !== "" && $scope.track.FromDate !== null && $scope.track.FromDate.getDate() === date.getDate()) {
                    return mode === 'day' && false;
                }
                return mode === 'day' && (date < $scope.track.FromDate);
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

        $scope.SpecialSearchList = [
            {
                SpecialSearchId: 0,
                SpecialSearch: 'All',
                SpecialSearchDisplay: 'All'
            },
            {
                SpecialSearchId: 1,
                SpecialSearch: 'NewAWBs',
                SpecialSearchDisplay: 'New CONs'
            },
            {
                SpecialSearchId: 2,
                SpecialSearch: 'ReadyforBag',
                SpecialSearchDisplay: 'Ready for Bag'
            },
            {
                SpecialSearchId: 3,
                SpecialSearch: 'ReadyforManifest',
                SpecialSearchDisplay: 'Ready for Manifest'
            }
        ];

        $scope.Speacialsearch = $scope.SpecialSearchList[0];
        var userInfo = SessionService.getUser();
        $scope.$UserRoleId = userInfo.RoleId;
        $scope.StaffRoleId = userInfo.RoleId;
        $scope.UserId = userInfo.EmployeeId;
        //Pagination Logic 
        $scope.viewby = 11;
        $scope.currentPage = 1;
        $scope.itemsPerPage = $scope.viewby;
        $scope.totalItemCount = 1000;

        $scope.maxSize = 2; //Number of pager buttons to show
        $scope.track = {
            UserId: userInfo.EmployeeId,
            DateTime: '',
            FromDate: '',
            ToDate: '',
            ShipmentStatusId: 0,
            CustomerId: 0,
            MAWB: '',
            FrayteNumber: '',
            CourierTrackingNo: '',
            CurrentPage: $scope.currentPage,
            TakeRows: $scope.itemsPerPage
        };

        if (userInfo) {
            //setMultilingualOptions();
            if ($scope.$UserRoleId === 3) {
                $scope.ShowServiceOptionPanel = true;
                $scope.IsDashboardShow = true;
                $scope.LoadShipmentStatus();
                //getInitialDetails();
            }
            else if ($scope.$UserRoleId === 1 || $scope.$UserRoleId === 6) {
                $scope.ShowServiceOptionPanel = true;
                $scope.IsDashboardShow = true;
                $scope.LoadShipmentStatus();
            }
        }
        else {
            $state.go("login");
        }

        //$scope.tabs = userInfo.tabs;
        //$scope.tab = getUserTab($scope.tabs, "QuickDirect_Booking");

        $scope.FromDate = null;
        $scope.ToDate = null;

        //setBookingState("DirectBooking");
        $scope.pageChangedObj = [{
            SpecialSearchId: 1,
            pageChangedValue: 20,
            pageChangedValueDisplay: 20
        },
         {
             SpecialSearchId: 2,
             pageChangedValue: 50,
             pageChangedValueDisplay: 50
         },
         {
             SpecialSearchId: 3,
             pageChangedValue: 100,
             pageChangedValueDisplay: 100
         },
         {
             SpecialSearchId: 4,
             pageChangedValue: 200,
             pageChangedValueDisplay: 200
         }];
        //$scope.track.TakeRows = 11;
        if ($scope.$UserRoleId !== 3) {
            getCustomers();
        }

        $scope.LoadShipmentStatus();
        $scope.gridheight = SessionService.getScreenHeight();
        $scope.buildURL = config.BUILD_URL;
        $rootScope.GetServiceValue = null;
    }

    init();
});