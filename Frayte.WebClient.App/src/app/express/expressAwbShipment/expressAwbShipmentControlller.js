
angular.module('ngApp.express').controller("ExpressAWBShipmentController", function ($scope, config, $state, SessionService, ExpressShipmentService, ExpressScanAwbService, $http, $window, $translate, toaster, DateFormatChange, ModalService, AppSpinner) {

    //Set Multilingual for Modal Popup
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteWarning', 'FrayteSuccess', 'ShipmentScannedDeleteConfirmation', 'SuccessfullyDeletedScannedDraftShipment', 'SureDeleteShipmentScanned', 'Problem_Reading_Byte_Array', 'ReportGeneratedSuccessfully', 'CouldNotDownloadTheReport', 'NoReportGenerated', 'AWB_Image_Not_Available']).then(function (translations) {
            $scope.FrayteError = translations.FrayteError;
            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.Problem_Reading_Byte_Array = translations.Problem_Reading_Byte_Array;
            $scope.SuccessfullyDeletedScannedShipment = translations.SuccessfullyDeletedScannedDraftShipment;
            $scope.ReportGeneratedSuccessfully = translations.ReportGeneratedSuccessfully;
            $scope.CouldNotDownloadTheReport = translations.CouldNotDownloadTheReport;
            $scope.ShipmentScannedDeleteConfirmation = translations.ShipmentScannedDeleteConfirmation;
            $scope.SureDeleteShipmentScanned = translations.SureDeleteShipmentScanned;
            $scope.NoReportGenerated = translations.NoReportGenerated;
            $scope.AWB_Image_Not_Available = translations.AWB_Image_Not_Available;

        });
    };
    //end
    $scope.RefreshData = function () {
        $scope.Track.AWBNumber = "";
        $scope.SearchAWB();
    };
    

    $scope.SearchAWB = function () {
        $scope.GetAWBs();
    };

    $scope.CustomerChange = function () {
        $scope.GetAWBs();

    };

    $scope.pageChanged = function (Track) {
        $scope.GetAWBs();
    };

    $scope.GetAWBs = function () {
        ExpressScanAwbService.GetScannedAWB($scope.Track).then(function (response) {
            if (response.data !== undefined && response.data !== '' && response.data !== null && response.data.length > 0) {
                $scope.AWBList = response.data;
                for (i = 0; i < response.data.length; i++) {
                    response.data[i].ScannedOn = DateFormatChange.DateFormatChange(response.data[i].ScannedOn) + " " + response.data[i].ScannedOnTime;
                    response.data[i].IsSelected = false;
                }
                $scope.totalItemCount = response.data[0].TotalRows;
                $scope.AwbPageCount = response.data[0].CurrentPageAwb;
            }
            else {
                $scope.AWBList = response.data;
                $scope.totalItemCount = 0;
                $scope.AwbPageCount = 0;
            }
        }, function () {
            //toaster.pop({
            //    type: 'warning',
            //    title: $scope.TitleFrayteInformation,
            //    body: $scope.CustomerStaffRemoveSuccess,
            //    showCloseButton: true
            //});
        });
    };

    //checkbox check and uncheck code

    $scope.checkAll = function () {
        //$scope.AWBList = response.data;
        if ($scope.selectedAll) {
            $scope.selectedAll = true;
        } else {
            $scope.selectedAll = false;
        }
        angular.forEach($scope.AWBList, function (AWB) {
            AWB.IsSelected = $scope.selectedAll;
        });
    };

    //end

    $scope.AwbSelect = function (Awb) {
        if ($scope.AwbDeleteList === undefined || $scope.AwbDeleteList === null) {
            $scope.AwbDeleteList = [];
        }
        if (Awb.IsSelected) {
            $scope.AwbDeleteList.push(Awb.AWBId);
        }
        else {
            if ($scope.AwbDeleteList.length > 0) {
                for (i = 0; i < $scope.AwbDeleteList.length; i++) {
                    if ($scope.AwbDeleteList[i] === Awb.AWBId) {
                        $scope.AwbDeleteList.splice(i, 1);
                    }
                }
            }
        }
    };

    $scope.MultipleDelete = function () {

        if ($scope.AwbDeleteList !== undefined && $scope.AwbDeleteList !== null && $scope.AwbDeleteList.length > 0) {
            var modalOptions = {
                headerText: $scope.ShipmentScannedDeleteConfirmation,
                bodyText: $scope.SureDeleteShipmentScanned
            };
            ModalService.Confirm({}, modalOptions).then(function (result) {
                ExpressScanAwbService.MultipleScannedDelete($scope.AwbDeleteList).then(function (response) {
                    $scope.ExpressSolutioCustomers = response.data;
                    
                        toaster.pop({
                            type: 'success',
                            title: $scope.FrayteSuccess,
                            body: $scope.SuccessfullyDeletedScannedShipment,
                            showCloseButton: true
                        });
                        $scope.SearchAWB();
                    
                });
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: "Please select at least one AWB.",
                showCloseButton: true
            });
        }
    };

    var getCustomerList = function () {
        ExpressScanAwbService.GetCustomerList($scope.LogedInUser).then(function (response) {
            $scope.ExpressSolutioCustomers = response.data;
            for (i = 0; i < $scope.ExpressSolutioCustomers.length; i++) {
                var dbr = $scope.ExpressSolutioCustomers[i].AccountNumber.split("");
                var accno = "";
                for (var j = 0; j < dbr.length; j++) {
                    accno = accno + dbr[j];
                    if (j == 2 || j == 5) {
                        accno = accno + "-";
                    }
                }
                $scope.ExpressSolutioCustomers[i].Display = $scope.ExpressSolutioCustomers[i].CompanyName + " - " + accno;
            }

            ObjAll = {
                Display: "All",
                AccountNumber: "",
                CustomerId: 0
            };
            $scope.ExpressSolutioCustomers.unshift(ObjAll);


        });
    };

    $scope.DownloadAWBImage = function (row) {
        AppSpinner.showSpinnerTemplate($scope.Loading_Postcode_Addresses, $scope.Template);
        if (row !== undefined && row !== null) {
            ExpressScanAwbService.ImageToByte(row.AWBId).then(function (response) {
                //AppSpinner.hideSpinner();
                var fileInfo = response.data;
                var fileName = {
                    FileName: response.data.FileName
                };
                if (response.data != null && response.data.Status === true) {
                    $http({
                        method: 'POST',
                        url: config.SERVICE_URL + '/ExpressScannedAWB/DownloadAWBImage',
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
                                $window.open(fileInfo.FilePath);
                                console.log(ex);
                            }

                        }
                    })
                   .error(function (data) {
                       //AppSpinner.hideSpinner();
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
                    //AppSpinner.hideSpinner();
                    //console.log(data);
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteWarning,
                        body: $scope.AWB_Image_Not_Available,
                        showCloseButton: true
                    });
                }
            }, function () {

                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.NoReportGenerated,
                    showCloseButton: true
                });
            });
        }
        AppSpinner.hideSpinnerTemplate();
    };


    // Create Shipment  
    $scope.createShipment = function (row) {
        if (row) {
            $state.go("loginView.userTabs.express-solution-booking", { shipmentId: row.AWBId }, { reload: true });
        }
    };

    //end

    $scope.DeleteDraft = function (ExpressShipmentId) {
        var modalOptions = {
            headerText: $scope.ShipmentScannedDeleteConfirmation,
            bodyText: $scope.SureDeleteShipmentScanned
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            ExpressShipmentService.DeleteExpressShipment(ExpressShipmentId).then(function (response) {
                if (response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.SuccessfullyDeletedScannedShipment,
                        showCloseButton: true
                    });
                    $scope.SearchAWB();
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

    function init() {
        setMultilingualOptions();
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        var userInfo = SessionService.getUser();
        $scope.LogedInUser = userInfo.EmployeeId;
        $scope.CustomerId = 0;
        $scope.AWBNumber = "";
        $scope.RoleId = userInfo.RoleId;
        $scope.viewby = 11;
        $scope.currentPage = 1;
        $scope.itemsPerPage = $scope.viewby;
        $scope.maxSize = 2;
        $scope.Track = {
            CurrentPage: $scope.currentPage,
            TakeRows: $scope.itemsPerPage,
            CustomerId: 0,
            AWBNumber: ""
        };
        if ($scope.RoleId === 3)
        {
            
            $scope.Track.CustomerId = $scope.LogedInUser;
        }
        else {
           // $scope.Track.CustomerId = 0;
            getCustomerList();
        }
        
        $scope.GetAWBs();
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
        $scope.Track.TakeRows = $scope.pageChangedObj[1].pageChangedValue;
    }
    init();
});