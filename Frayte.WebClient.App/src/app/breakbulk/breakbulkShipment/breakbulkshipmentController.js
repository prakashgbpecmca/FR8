
angular.module('ngApp.breakBulk').controller("breakbulkShipmentController", function ($scope, $uibModal, ModalService, toaster, AppSpinner, BreakBulkService, DirectBookingService, ExpressShipmentService, SessionService, $state, $translate, DateFormatChange, BreakbulkShipmentService, config) {


    $scope.breakbulkCollapse = function (id) {
        //$scope.isBreakbulk = !$scope.isBreakbulk;
        $scope.isBreakbulk = ($scope.isBreakbulk == id) ? -1 : id;
    };

    $scope.poviewClick = function () {
        $scope.poview = true;
        $scope.jobview = false;
    };

    $scope.jobviewClick = function () {
        $scope.poview = false;
        $scope.jobview = true;
    };

    //date code
    $scope.today = function () {
        $scope.dt = new Date();
    };
    $scope.today();

    $scope.clear = function () {
        $scope.dt = null;
    };

    $scope.inlineOptions = {
        customClass: getDayClass,
        minDate: new Date(),
        showWeeks: true
    };

    $scope.dateOptions = {
        dateDisabled: disabled,
        formatYear: 'yy',
        maxDate: new Date(2020, 5, 22),
        minDate: new Date(),
        startingDay: 1
    };

    // Disable weekend selection
    function disabled(data) {
        var date = data.date,
            mode = data.mode;
        return mode === 'day' && (date.getDay() === 0 || date.getDay() === 6);
    }

    $scope.toggleMin = function () {
        $scope.inlineOptions.minDate = $scope.inlineOptions.minDate ? null : new Date();
        $scope.dateOptions.minDate = $scope.inlineOptions.minDate;
    };

    $scope.toggleMin();

    $scope.open1 = function () {
        $scope.popup1.opened = true;
    };

    $scope.open2 = function () {
        $scope.popup2.opened = true;
    };

    $scope.setDate = function (year, month, day) {
        $scope.dt = new Date(year, month, day);
    };

    $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
    $scope.format = $scope.formats[0];
    $scope.altInputFormats = ['M!/d!/yyyy'];

    $scope.popup1 = {
        opened: false
    };

    $scope.popup2 = {
        opened: false
    };

    var tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    var afterTomorrow = new Date();
    afterTomorrow.setDate(tomorrow.getDate() + 1);
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

    function getDayClass(data) {
        var date = data.date,
            mode = data.mode;
        if (mode === 'day') {
            var dayToCheck = new Date(date).setHours(0, 0, 0, 0);

            for (var i = 0; i < $scope.events.length; i++) {
                var currentDay = new Date($scope.events[i].date).setHours(0, 0, 0, 0);

                if (dayToCheck === currentDay) {
                    return $scope.events[i].status;
                }
            }
        }

        return '';
    }
    //end

    //end

    //var getCustomers = function () {
    //    //  getUserId();
    //    BreakbulkShipmentService.GetExpressCustomers($scope.$UserRoleId, $scope.UserId).then(function (response) {
    //        $scope.directBookingCustomers = response.data;
            

    //    }, function () {
    //        toaster.pop({
    //            type: 'error',
    //            title: $scope.FrayteError,
    //            body: $scope.ReceiveDetailValidation,
    //            showCloseButton: true
    //        });
    //    });
    //};

    $scope.editpo = function (shipment) {
        if (shipment) {
            $state.go("loginView.userTabs.break-bulk-booking-clone", { shipmentId: shipment.PurchaseOrderId });
        }
    };

    $scope.editjob = function (shipment) {
        if (shipment) {
            $state.go("loginView.userTabs.break-bulk-booking-clone", { shipmentId: shipment.PurchaseOrderDetailId });
        }
    };
    

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
    $scope.breakbulkGenerateLabel = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'BreakbulkPurchaseOrderDetail',
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
            controller: 'breakbulkDetailController',
            templateUrl: 'breakbulk/details/breakbulkShipmentDetail.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg'
        });
    };
    //end

    //breakbulk shipment detail code here
    $scope.BreakBulkTrackingDetail = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'breakbulkTrackingController',
            templateUrl: 'breakbulk/breakBulkTracking/breakbulkTracking.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg'
        });
    };
    //end

    //breakbulk get service order code
    $scope.breakbulkGetService = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'BreakbulkGetserviceController',
            templateUrl: 'breakbulk/breakbulkGetservice/breakbulkGetservice.tpl.html',
            keyboard: true,
            windowClass: '',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end

    //breakbulkCreateCarton code here
    $scope.breakbulkCreateCarton = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'breakbulkCreateCartonController',
            templateUrl: 'breakbulk/details/breakbulkCreateCarton.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end

    //breakbulkPoDetail code here
    $scope.breakbulkPoDetail = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'breakbulkPoDetailController',
            templateUrl: 'breakbulk/details/breakbulkPoDetail.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end

    //end

    //Set Multilingual code here
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteWarning', 'FrayteSuccess', 'ReceiveDetailValidation', 'GettingDataErrorValidation', 'To_Date', 'ToDateValidation', 'From_Date', 'SuccessfullyDeletedDraftShipment',
            'ShipmentDraftDeleteConfirmation', 'SureDeleteShipmentDraft', 'From_Date_Required', 'To_Date_Required', 'Select_Date_To', 'Select_Date_From', 'LoadingTrackTrace', 'Loading_Shipments',
            'Tracking_Code', 'ReportCannotDownloadPleaseTryAgain']).then(function (translations) {
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
            });
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

    $scope.SeachDirectShipments = function () {
        $scope.DirectShipments();
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
        if ($scope.poview === true) {


            BreakbulkShipmentService.GetPOPurchaseOrderD($scope.track).then(function (response) {
                if (response.data !== null && response.data.length > 0) {
                    $scope.totalItemCount = response.data[0].TotalRows;
                }
                else {
                    $scope.totalItemCount = 0;
                }
                for (i = 0; i < response.data.length; i++) {
           
                    var utc = DateFormatChange.DateFormatChange(response.data[i].CreatedOnUtc);
                    response.data[i].CreatedOnUtc = utc.replace(/(^|\D)(\d)(?!\d)/g, '$10$2');

                    var factzeroadd = DateFormatChange.DateFormatChange(response.data[i].ExFactoryDate);
                    response.data[i].ExFactoryDate = factzeroadd.replace(/(^|\D)(\d)(?!\d)/g, '$10$2');

                }
                $scope.breakbulkShipments = response.data;
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

        }
        else {
          BreakbulkShipmentService.GetJobPurchaseOrderD($scope.track).then(function (response) {
                    if (response.data !== null && response.data.length > 0) {
                        $scope.totalItemCount = response.data[0].TotalRows;
                    }
                    else {
                        $scope.totalItemCount = 0;
                    }
                    for (i = 0; i < response.data.length; i++) {
                        response.data[i].CreatedOnUtc = DateFormatChange.DateFormatChange(response.data[i].CreatedOnUtc);
                    }
                $scope.breakbulkjobShipments = response.data;
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
        }
            //};
    };

    $scope.currentImgURL = function (row) {
        if (row !== undefined) {
            var url = '';
            for (var i = 0; i < $scope.DirectShipmentStatus.length; i++) {
                if (row.POStatus == $scope.DirectShipmentStatus[i].StatusName) {
                    url = $scope.DirectShipmentStatus[i].ImgURL;
                    break;
                }
            }
            return url;
        }
    };

    $scope.currentImgURLjob = function (row) {
        if (row !== undefined) {
            var url = '';
            for (var i = 0; i < $scope.DirectShipmentStatus.length; i++) {
                if (row.JobStatus == $scope.DirectShipmentStatus[i].StatusName) {
                    url = $scope.DirectShipmentStatus[i].ImgURL;
                    break;
                }
            }
            return url;
        }
    };


    $scope.jobviewClicktab = function () {
        BreakbulkShipmentService.GetJobPurchaseOrderD($scope.track).then(function (response) {
            if (response.data !== null && response.data.length > 0) {
                $scope.totalItemCount = response.data[0].TotalRows;
            }
            else {
                $scope.totalItemCount = 0;
            }
            for (i = 0; i < response.data.length; i++) {
                response.data[i].CreatedOnUtc = DateFormatChange.DateFormatChange(response.data[i].CreatedOnUtc);
            }
            $scope.breakbulkjobShipments = response.data;
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
  
    $scope.bookingStatus = function (ShipmentStatusId) {
        $scope.CurrentStatus = ShipmentStatusId;
        $scope.track.ShipmentStatusId = ShipmentStatusId;
        if ($scope.track.CurrentPage === 1) {

        }
        else {
            $scope.track.CurrentPage = 1;
        }
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

        BreakbulkShipmentService.GetBBKShipmentStatusList("Breakbulk").then(function (response) {
            $scope.DirectShipmentStatus = response.data;

            ObjAll = {
                BookingType: "Breakbulk",
                DisplayStatusName: "All",
                ShipmentStatusId: 0,
                StatusName: "All"
            };
            $scope.DirectShipmentStatus.unshift(ObjAll);
            
            var DSS;
            for (var i = 1; i < $scope.DirectShipmentStatus.length; i++) {
                if ($scope.DirectShipmentStatus[i].BBKStatus === "PO") {

                    //for (var j = 0; j < $scope.DirectShipmentStatus[i].Status.length; j++) {

                    //    if ($scope.DirectShipmentStatus[i].Status[j].StatusName === "Draft") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "draftImg.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[j].StatusName === "All") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "all.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[j].StatusName === "Current") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "currentImg.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[j].StatusName === "OrderPlaced") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "T-intransit.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[j].StatusName === "OrderCompleted") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "T-delivered.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[j].StatusName === "Delivered") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "T-delivered.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[j].StatusName === "JobPlaced") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "D-pending.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[j].StatusName === "JobCompleted") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "D-departed.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[j].StatusName === "LabelGenerated") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "D-rejected.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[j].StatusName === "Arrived") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "D-arrived.png";
                    //    } else if ($scope.DirectShipmentStatus[i].Status[j].StatusName === "HubReceived") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "Hub.png";
                    //    }
                    //}
                }
                else if ($scope.DirectShipmentStatus[i].BBKStatus === "JOB") {

                    //for (var k = 0; k < $scope.DirectShipmentStatus[i].Status.length; k++) {

                    //    if ($scope.DirectShipmentStatus[i].Status[k].StatusName === "Draft") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "draftImg.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[k].StatusName === "All") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "all.png";
                    //    }

                    //    else if ($scope.DirectShipmentStatus[i].Status[k].StatusName === "JobPlaced") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "D-pending.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[k].StatusName === "JobCompleted") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "D-departed.png";
                    //    }

                    //}

                }

                else if ($scope.DirectShipmentStatus[i].BBKStatus === "Shipment") {

                    //for (var m = 0; m < $scope.DirectShipmentStatus[i].Status.length; m++) {

                    //    if ($scope.DirectShipmentStatus[i].Status[m].StatusName === "Draft") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "draftImg.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[m].StatusName === "All") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "all.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[m].StatusName === "Current") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "currentImg.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[m].StatusName === "OrderPlaced") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "T-intransit.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[m].StatusName === "OrderCompleted") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "T-delivered.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[m].StatusName === "Delivered") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "T-delivered.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[m].StatusName === "JobPlaced") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "D-pending.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[m].StatusName === "JobCompleted") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "D-departed.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[m].StatusName === "LabelGenerated") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "D-rejected.png";
                    //    }
                    //    else if ($scope.DirectShipmentStatus[i].Status[m].StatusName === "Arrived") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "D-arrived.png";
                    //    } else if ($scope.DirectShipmentStatus[i].Status[m].StatusName === "HubReceived") {
                    //        $scope.DirectShipmentStatus[i].ImgURL = "Hub.png";
                    //    }
                    //}

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

    function init() {

        setMultilingualOptions();
        $scope.ImagePath = config.BUILD_URL;
        $scope.buildURL = config.BUILD_URL;

        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.poview = true;
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
        $scope.numbers = [20, 30, 50, 100];


        BreakBulkService.GetInitials($scope.UserId).then(function (response) {
            if (response.data !== undefined && response.data !== null && response.data !== '') {
                $scope.directBookingCustomers = response.data.PaymentParty;

                var dbCustomers = [];
                for (i = 0; i < $scope.directBookingCustomers.length; i++) {
                    if ($scope.directBookingCustomers[i].CustomerId) {
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
                }

         
            }
        });

        $scope.track = {
            UserId: $scope.userInfo.EmployeeId,
            DateTime: '',
            FromDate: '',
            ToDate: '',
            ShipmentStatusId: 0,
            OperationZoneId: 0,
            CustomerId: 0,
            MAWB: '',
            PONo: '',
            JobNo: '',
            Status: null,
            TrackingNo: '',
            StyleNo: '',
            CustomField: '',
            FrayteNumber: '',
            CourierTrackingNo: '',
            CurrentPage: $scope.currentPage,
            TakeRows: $scope.itemsPerPage
        };

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
        $scope.track.TakeRows = $scope.pageChangedObj[1].pageChangedValue;
        

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

        if ($scope.$UserRoleId !== 3) {
            //getCustomers();
        }

        $scope.LoadShipmentStatus();

    }
    init();

});