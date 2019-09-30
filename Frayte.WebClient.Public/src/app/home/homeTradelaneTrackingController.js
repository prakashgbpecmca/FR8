angular.module('ngApp.home').controller('HomeTradelaneTrackingController', function ($scope, $state, $translate, $uibModal, toaster, ModalService, SessionService, AppSpinner, $rootScope, config, HomeService, DateFormatChange, $anchorScroll, $location) {

    //translation code here
    var setMultilingualOptions = function () {
        $translate(['FrayteSuccess', 'FrayteWarning', 'FrayteError', 'DeletedSuccessfully', 'Sure_Delete_Flight_Tracking_Detail', 'Delete_Tracking_Confirmation',
                    'Sure_Delete_Tracking_Detail', 'Delete_Flight_Tracking_Confirmation']).then(function (translations) {
                        $scope.FrayteSuccess = translations.FrayteSuccess;
                        $scope.FrayteWarning = translations.FrayteWarning;
                        $scope.FrayteError = translations.FrayteError;
                        $scope.DeletedSuccessfully = translations.DeletedSuccessfully;
                        $scope.Sure_Delete_Flight_Tracking_Detail = translations.Sure_Delete_Flight_Tracking_Detail;
                        $scope.Delete_Tracking_Confirmation = translations.Delete_Tracking_Confirmation;
                        $scope.Sure_Delete_Tracking_Detail = translations.Sure_Delete_Tracking_Detail;
                        $scope.Delete_Flight_Tracking_Confirmation = translations.Delete_Flight_Tracking_Confirmation;

                    });
    };

    //end

    //collapsable panel code here

    $scope.oneCollapseClick = function () {
        $scope.oneCollapse = !$scope.oneCollapse;
    };

    $scope.twoCollapseClick = function () {
        $scope.twoCollapse = !$scope.twoCollapse;
    };

    $scope.threeCollapseClick = function () {
        $scope.threeCollapse = !$scope.threeCollapse;
    };

    //$scope.allCollapseClick = function () {
    //if ($scope.allCollapse === true) {
    //    $scope.oneCollapse = true;
    //    $scope.twoCollapse = true;
    //    $scope.threeCollapse = true;
    //}
    //if ($scope.allCollapse === false) { 
    //    $scope.oneCollapse = false;
    //    $scope.twoCollapse = false;
    //    $scope.threeCollapse = false;
    //}
    //};

    //end


    //update tracking collapsable code here
    $scope.updateTrackingCollapse = function () {
        $scope.collapse = !$scope.collapse;
    };
    //end

    $scope.AddTracking = function (row) {
        if (row === undefined || row === null) {
            ModalInstance = $uibModal.open({
                Animation: true,
                templateUrl: 'tradelaneShipments/tradelaneUpdateTracking/tradelaneAddTracking/tradelaneAddTracking.tpl.html',
                controller: 'TradelaneAddTrackingController',
                keyboard: true,
                windowClass: '',
                size: 'md',
                backdrop: 'static',
                resolve: {
                    ShipmentInfo: function () {
                        return $scope.Shipment;
                    },
                    TrackingInfo: function () {
                        return null;
                    }
                }
            });
            ModalInstance.result.then(function (response) {
                $scope.GetUpdateTracking();
            }, function () {

            });
        }
        else {
            ModalInstance = $uibModal.open({
                Animation: true,
                templateUrl: 'tradelaneShipments/tradelaneUpdateTracking/tradelaneAddTracking/tradelaneAddTracking.tpl.html',
                controller: 'TradelaneAddTrackingController',
                keyboard: true,
                windowClass: '',
                size: 'md',
                backdrop: 'static',
                resolve: {
                    ShipmentInfo: function () {
                        return $scope.Shipment;
                    },
                    TrackingInfo: function () {
                        return row;
                    }
                }
            });
            ModalInstance.result.then(function (response) {
                $scope.GetUpdateTracking();
            }, function () {

            });
        }
    };

    $scope.AddFlightDetail = function (row) {
        if (row === undefined || row === null) {
            ModalInstance = $uibModal.open({
                Animation: true,
                templateUrl: 'tradelaneShipments/tradelaneUpdateTracking/tradelaneAddFlightDetail/tradelaneAddFlightDetail.tpl.html',
                controller: 'TradelaneFlightDetailController',
                keyboard: true,
                windowClass: 'DirectBookingDetail',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    ShipmentInfo: function () {
                        return $scope.Shipment;
                    },
                    TrackingInfo: function () {
                        return null;
                    }
                }
            });
            ModalInstance.result.then(function (response) {
                $scope.GetUpdateTracking();
            }, function () {

            });
        }
        else {
            ModalInstance = $uibModal.open({
                Animation: true,
                templateUrl: 'tradelaneShipments/tradelaneUpdateTracking/tradelaneAddFlightDetail/tradelaneAddFlightDetail.tpl.html',
                controller: 'TradelaneFlightDetailController',
                keyboard: true,
                windowClass: 'DirectBookingDetail',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    ShipmentInfo: function () {
                        return $scope.Shipment;
                    },
                    TrackingInfo: function () {
                        return row;
                    }
                }
            });
            ModalInstance.result.then(function (response) {
                $scope.GetUpdateTracking();
            }, function () {

            });
        }
    };

    $scope.GetUpdateTracking = function () {
        HomeService.GetShipmentTradelaneDetail($scope.TradelaneShipmentId).then(function (response) {
            if (response.data) {
                $scope.TRadelaneShipmentDetail = response.data;
            }
        }, function () {

        });
        HomeService.GetUpdateTracking($scope.TradelaneShipmentId).then(function (response) {
            if (response.data) {
                $scope.TRadelaneTrackingList = response.data.TradelaneTrackingDetail;
                $scope.TRadelaneTrackingOperationalList = response.data.TradelaneOperationalDetail;
                for (i = 0; i < $scope.TRadelaneTrackingOperationalList.length; i++) {
                    $scope.TRadelaneTrackingOperationalList[i].CreatedOnUtc = DateFormatChange.DateFormatChange($scope.TRadelaneTrackingOperationalList[i].CreatedOnUtc);
                }
                for (j = 0; j < $scope.TRadelaneTrackingList.length; j++) {
                    $scope.TRadelaneTrackingList[j].DepartureDate = DateFormatChange.DateFormatChange($scope.TRadelaneTrackingList[j].DepartureDate);
                    $scope.TRadelaneTrackingList[j].ArrivalDate = DateFormatChange.DateFormatChange($scope.TRadelaneTrackingList[j].ArrivalDate);
                }
            }
        }, function () {

        });
        HomeService.GetTrackingStatus($scope.TradelaneShipmentId).then(function (response) {
            if (response.data) {
                $scope.TRadelaneShipmentStatus = response.data;
                for (i = 0; i < $scope.TRadelaneShipmentStatus.length; i++) {
                    var TS = [];
                    for (j = 0; j < $scope.TRadelaneShipmentStatus[i].TrackingStatus.length; j++) {
                        $scope.TRadelaneShipmentStatus[i].TrackingStatus[j].Date = DateFormatChange.DateFormatChange($scope.TRadelaneShipmentStatus[i].TrackingStatus[j].Date);
                    }
                }
            }
        }, function () {

        });
    };

    //Delete Operational Tracking
    $scope.DeleteTradelaneTracking = function (row) {
        var modalOptions = {
            headerText: $scope.Delete_Tracking_Confirmation,
            bodyText: $scope.Sure_Delete_Tracking_Detail
        };
        ModalService.Confirm({}, modalOptions).then(function (result) {
            TradelaneShipmentService.DeleteTradelaneOperationalTracking(row.TradelaneShipmentTrackingId).then(function (response) {
                if (response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.DeletedSuccessfully,
                        showCloseButton: true
                    });
                    $scope.GetUpdateTracking();
                }
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.SuccessfullyDeletedDraftShipment,
                    showCloseButton: true
                });
            });
        });
    };

    $scope.DeleteTradelaneFlightTracking = function (row) {
        var modalOptions = {
            headerText: $scope.Delete_Flight_Tracking_Confirmation,
            bodyText: $scope.Sure_Delete_Flight_Tracking_Detail
        };
        ModalService.Confirm({}, modalOptions).then(function (result) {
            TradelaneShipmentService.DeleteTradelaneTracking(row.TradelaneFlightId).then(function (response) {
                if (response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.DeletedSuccessfully,
                        showCloseButton: true
                    });
                    $scope.GetUpdateTracking();
                }
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.SuccessfullyDeletedDraftShipment,
                    showCloseButton: true
                });
            });
        });
    };

    $scope.setScroll = function (printSectionId) {
        $location.hash(printSectionId);
        $anchorScroll();
    };

    $scope.GetSearchUpdateTracking = function () {
        HomeService.GetShipmentTradelaneDetail($scope.TradelaneShipmentId).then(function (response) {
            if (response.data) {
                $scope.TRadelaneShipmentDetail = response.data;
                $scope.TrackingObj.DepartureAirport = $scope.TRadelaneShipmentDetail.DepartureAirport;
                $scope.TrackingObj.DestinationAirport = $scope.TRadelaneShipmentDetail.ArrivalAirport;
            }
        }, function () {

        });
        HomeService.GetUpdateTracking($scope.TradelaneShipmentId).then(function (response) {
            if (response.data) {
                $scope.TRadelaneTrackingList = response.data.TradelaneTrackingDetail;
                $scope.TRadelaneTrackingOperationalList = response.data.TradelaneOperationalDetail;
                for (i = 0; i < $scope.TRadelaneTrackingOperationalList.length; i++) {
                    $scope.TRadelaneTrackingOperationalList[i].CreatedOnUtc = DateFormatChange.DateFormatChange(new Date($scope.TRadelaneTrackingOperationalList[i].CreatedOnUtc));
                }
                for (j = 0; j < $scope.TRadelaneTrackingList.length; j++) {
                    if ($scope.TRadelaneTrackingList[j].DepartureDate !== null) {
                        $scope.TRadelaneTrackingList[j].DepartureDate = DateFormatChange.DateFormatChange($scope.TRadelaneTrackingList[j].DepartureDate);
                    }
                    else {
                        $scope.TRadelaneTrackingList[j].DepartureDate = "";
                    }
                    if ($scope.TRadelaneTrackingList[j].ArrivalDate !== null) {
                        $scope.TRadelaneTrackingList[j].ArrivalDate = DateFormatChange.DateFormatChange($scope.TRadelaneTrackingList[j].ArrivalDate);
                    }
                    else {
                        $scope.TRadelaneTrackingList[j].ArrivalDate = "";
                    }
                }
            }
            var flag = false;
            if ($scope.TrackingArray.length > 0) {
                for (i = 0; i < $scope.TrackingArray.length; i++) {
                    if ($scope.TrackingArray[i].Num === $scope.TrackingObj.Num) {
                        flag = true;
                        break;
                    }
                }
                if (!flag) {
                    $scope.TrackingArray.push($scope.TrackingObj);
                }
            }
            else {
                $scope.TrackingArray.push($scope.TrackingObj);
            }
            if ($scope.TrackingArray.length > 5) {
                $scope.TrackingArray.splice(0, 1);
            }
        }, function () {

        });
    };

    $scope.CheckMawb = function (mawb) {
        if ($scope.AirPickDetail.AirPick !== undefined && $scope.AirPickDetail.AirPick !== null && $scope.AirPickDetail.AirPick.Name === "MAWB" && mawb !== null && mawb !== "") {
            $scope.AirPickDetail.AirPicktype = mawb.replace(/[^0-9- ]/, '');
        }
    };

    $scope.SearchTradelaneTracking = function (isValid, AirpickDetail) {
        if (isValid) {
            $scope.TrackingObj = {
                Num: "",
                NumType: "",
                DepartureAirport: "",
                DestinationAirport: ""
            };
            $scope.TrackingObj.Num = AirpickDetail.AirPicktype;
            $scope.TrackingObj.NumType = AirpickDetail.AirPick.Name;
            HomeService.SearchTradelaneTracking(AirpickDetail.AirPicktype, AirpickDetail.AirPick.Name).then(function (response) {
                if (response.data) {
                    $scope.IsShowTrackingData = false;
                    $scope.TradelaneShipmentId = response.data;
                    $scope.GetSearchUpdateTracking();
                    HomeService.GetTrackingStatus($scope.TradelaneShipmentId).then(function (response) {
                        if (response.data) {
                            $scope.TRadelaneShipmentStatus = response.data;
                            for (i = 0; i < $scope.TRadelaneShipmentStatus.length; i++) {
                                for (j = 0; j < $scope.TRadelaneShipmentStatus[i].TrackingStatus.length; j++) {
                                    if ($scope.TRadelaneShipmentStatus[i].TrackingStatus[j].Date !== undefined && $scope.TRadelaneShipmentStatus[i].TrackingStatus[j].Date !== null && $scope.TRadelaneShipmentStatus[i].TrackingStatus[j].Date !== "") {
                                        $scope.TRadelaneShipmentStatus[i].TrackingStatus[j].Date = DateFormatChange.DateFormatChange(new Date($scope.TRadelaneShipmentStatus[i].TrackingStatus[j].Date));
                                    }
                                }
                            }
                        }
                    }, function () {

                    });
                }
                else {
                    $scope.IsShowTrackingData = true;
                }
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.SuccessfullyDeletedDraftShipment,
                    showCloseButton: true
                });
            });
        }
        else {
            //toaster.pop({
            //    type: 'warning',
            //    title: $scope.FrayteError,
            //    body: $scope.SuccessfullyDeletedDraftShipment,
            //    showCloseButton: true
            //});
        }
    };

    $scope.moduleClick = function () {
        $scope.module = !$scope.module;
    };

    $scope.firstClick = function (TL) {
        TL.IsCollapsed = !TL.IsCollapsed;
    };

    $scope.SearchTracking = function (Number) {
        $scope.trackingInfosList = [];
        $scope.TrackingListDetail = [];
        $scope.trackingBagInfosList = [];
        $scope.TrackingObj = {
            Num: "",
            NumType: "",
            DepartureAirport: "",
            DestinationAirport: ""
        };
        HomeService.SearchTracking(Number).then(function (response) {
            $scope.IsShowTrackingData = false;
            if (response.data != null && response.data[0].Status === true) {
                if (response.data[0].ModuleType === 'DirectBooking') {
                    $scope.trackingDBInfosList = [];
                    $scope.AWBBagTrackingList = response.data[0].BagTracking;
                    for (j = 0; j < $scope.AWBBagTrackingList.length; j++) {
                        if ($scope.AWBBagTrackingList != null && $scope.AWBBagTrackingList[j].Tracking !== null) {
                            $scope.Iserrorshow = true;
                            AppSpinner.showSpinnerTemplate();
                            var jsonDB = $scope.AWBBagTrackingList[j].Tracking;
                            if (jsonDB.length > 0) {
                                for (var jk = 0 ; jk < jsonDB.length; jk++) {
                                    jsonDB[jk].TrackingDetails = $scope.TrackingDetailJson(jsonDB[jk]);
                                }
                                $scope.trackingDBInfos = jsonDB;
                                if (j === 0) {
                                    $scope.trackingDBInfos[0].first = true;
                                }
                                else {
                                    $scope.trackingDBInfos[0].first = false;
                                }

                                $scope.trackingDBInfosList.push($scope.trackingDBInfos);
                                $scope.trackingDBInfos = [];
                            }
                            $scope.parcelHub = true;
                            AppSpinner.hideSpinnerTemplate();
                        }
                        if (response.data === null || response.data.Tracking === null) {
                            $scope.Iserrorshow = true;
                        }
                    }
                }
                if (response.data[0].ModuleType === 'Tradelane') {
                    $scope.TrackingDetail.IsCollapsed = false;
                    if (response.data[0].Trackingmodel.length === 1) {
                        $scope.TrackingDetail.IsCollapsed = true;
                    }
                    for (ii = 0; ii < response.data[0].Trackingmodel.length; ii++) {
                        $scope.TrackingDetail.TRadelaneTrackingList = response.data[0].Trackingmodel[ii].TradelaneTrackingDetail;
                        $scope.TrackingDetail.TRadelaneTrackingOperationalList = response.data[0].Trackingmodel[ii].TradelaneOperationalDetail;
                        $scope.TrackingDetail.ShipmentDetail = response.data[0].Trackingmodel[ii].ShipmentDetail;
                        $scope.TrackingDetail.TradelaneStatus = response.data[0].Trackingmodel[ii].TradelaneStatus;

                        for (i = 0; i < $scope.TrackingDetail.TradelaneStatus.length; i++) {
                            $scope.TrackingDetail.TradelaneStatus[i].TrackingStatus[0].Date = DateFormatChange.DateFormatChange(new Date($scope.TrackingDetail.TradelaneStatus[i].TrackingStatus[0].Date));
                        }
                        for (i = 0; i < $scope.TrackingDetail.TRadelaneTrackingOperationalList.length; i++) {
                            $scope.TrackingDetail.TRadelaneTrackingOperationalList[i].CreatedOnUtc = DateFormatChange.DateFormatChange(new Date($scope.TrackingDetail.TRadelaneTrackingOperationalList[i].CreatedOnUtc));
                        }
                        for (j = 0; j < $scope.TrackingDetail.TRadelaneTrackingList.length; j++) {
                            if ($scope.TrackingDetail.TRadelaneTrackingList[j].DepartureDate !== null) {
                                $scope.TrackingDetail.TRadelaneTrackingList[j].DepartureDate = DateFormatChange.DateFormatChange($scope.TrackingDetail.TRadelaneTrackingList[j].DepartureDate);
                            }
                            else {
                                $scope.TrackingDetail.TRadelaneTrackingList[j].DepartureDate = "";
                            }
                            if ($scope.TrackingDetail.TRadelaneTrackingList[j].ArrivalDate !== null) {
                                $scope.TrackingDetail.TRadelaneTrackingList[j].ArrivalDate = DateFormatChange.DateFormatChange($scope.TrackingDetail.TRadelaneTrackingList[j].ArrivalDate);
                            }
                            else {
                                $scope.TrackingDetail.TRadelaneTrackingList[j].ArrivalDate = "";
                            }
                        }
                        $scope.TrackingListDetail.push($scope.TrackingDetail);
                        $scope.TrackingDetail = {
                            TradelaneOperationalDetail: {},
                            TRadelaneTrackingList: {},
                            ShipmentDetail: {},
                            CurrentStauts: {},
                            TradelaneStatus: {}
                        };
                    }
                }
                if (response.data[0].ModuleType === 'Express AWB') {
                    $scope.trackingInfosList = [];
                    $scope.AWBTrackingList = response.data[0].ExpressTracking;
                    for (i = 0; i < $scope.AWBTrackingList.length; i++) {
                        if ($scope.AWBTrackingList != null && $scope.AWBTrackingList[i].Tracking !== null) {
                            $scope.Iserrorshow = true;
                            AppSpinner.showSpinnerTemplate();
                            var json = $scope.AWBTrackingList[i].Tracking;
                            if (json.length > 0) {
                                for (var ii = 0 ; ii < json.length; ii++) {
                                    json[ii].TrackingDetails = $scope.TrackingDetailJson(json[ii]);
                                }
                                $scope.trackingInfos = json;
                                $scope.trackingInfos.first = false;
                                $scope.trackingInfosList.push($scope.trackingInfos);
                                $scope.trackingInfos = [];
                            }
                            $scope.parcelHub = true;
                            AppSpinner.hideSpinnerTemplate();
                        }
                        else if (response.data === null || response.data.Tracking === null) {
                            $scope.Iserrorshow = true;
                        }
                    }
                }
                if (response.data[0].ModuleType === 'Express BAG') {
                    $scope.trackingBagInfosList = [];
                    $scope.AWBBagTrackingList = response.data[0].BagTracking;
                    for (j = 0; j < $scope.AWBBagTrackingList.length; j++) {
                        if ($scope.AWBBagTrackingList != null && $scope.AWBBagTrackingList[j].Tracking !== null) {
                            $scope.Iserrorshow = true;
                            AppSpinner.showSpinnerTemplate();
                            var jsonBag = $scope.AWBBagTrackingList[j].Tracking;
                            if (jsonBag.length > 0) {
                                for (var jj = 0 ; jj < jsonBag.length; jj++) {
                                    jsonBag[jj].TrackingDetails = $scope.TrackingDetailJson(jsonBag[jj]);
                                }
                                $scope.trackingBagInfos = jsonBag;
                                $scope.trackingBagInfos.first = false;
                                $scope.trackingBagInfosList.push($scope.trackingBagInfos);
                                $scope.trackingBagInfos = [];
                            }
                            $scope.parcelHub = true;
                            AppSpinner.hideSpinnerTemplate();
                        }
                        if (response.data === null || response.data.Tracking === null) {
                            $scope.Iserrorshow = true;
                        }
                    }
                }
                else {
                    $scope.module = true;
                    $scope.IsShowTrackingData = true;
                    AppSpinner.hideSpinnerTemplate();   
                }
            }
            else {
                $scope.TrackingListDetail = [];
                $scope.trackingInfosList = [];
                $scope.trackingBagInfosList = [];
                $scope.trackingDBInfosList = [];
                AppSpinner.hideSpinnerTemplate();
            }
        });
    };

    $scope.getstatus = function () {
        HomeService.GetTrackingStatus($scope.TrackingListDetail[i].TRadelaneTrackingOperationalList[0].TradelaneShipmentId).then(function (response) {
            if (response.data) {
                $scope.TrackingListDetail[i].TRadelaneShipmentStatus = response.data;
                for (i = 0; i < $scope.TrackingListDetail[i].TRadelaneShipmentStatus.length; i++) {
                    for (j = 0; j < $scope.TrackingListDetail[i].TRadelaneShipmentStatus[i].TrackingStatus.length; j++) {
                        if ($scope.TrackingListDetail[i].TRadelaneShipmentStatus[i].TrackingStatus[j].Date !== undefined && $scope.TrackingListDetail[i].TRadelaneShipmentStatus[i].TrackingStatus[j].Date !== null && $scope.TrackingListDetail[i].TRadelaneShipmentStatus[i].TrackingStatus[j].Date !== "") {
                            $scope.TrackingListDetail[i].TRadelaneShipmentStatus[i].TrackingStatus[j].Date = DateFormatChange.DateFormatChange(new Date($scope.TrackingListDetail[i].TRadelaneShipmentStatus[i].TrackingStatus[j].Date));
                        }
                    }
                }
            }
        }, function () {

        });

    };

    $scope.ClearForm = function () {
        $scope.AirPickDetail.AirPicktype = "";
    };

    $scope.ShowHideText = function (trackingInfo) {

        if (trackingInfo === undefined && trackingInfo === null && trackingInfo.ShowHideValue === '' && trackingInfo.ShowHideValue === null) {
            return "";
        }
        if (trackingInfo.ShowHideValue === "Hide") {
            trackingInfo.ShowHideValue = $scope.bulkHide;
            return trackingInfo.ShowHideValue;
        }
        else if (trackingInfo.ShowHideValue === "Show") {
            trackingInfo.ShowHideValue = $scope.bulkShow;
            return trackingInfo.ShowHideValue;
        }

    };

    $scope.trackingDetailNew = function (IsValid, easyPostDetail) {
        if (IsValid) {
            AppSpinner.showSpinnerTemplate($scope.LoadingBulkTracking, $scope.Template);
            $scope.TrackingListDetail = {};
            $state.go('home.tradelanetracking-hub', { SearchNumber: easyPostDetail.TrackingNumber });
            //$scope.SearchTracking(easyPostDetail.TrackingNumber);
            AppSpinner.hideSpinnerTemplate();
            //$scope.easyPostDetail.CarrierName = null;
        }
    };

    //Express Tracking
    $scope.GetCorrectFormattedDate = function (date, time) {
        // Geting Correct Date Format
        if (date !== null && date !== '' && date !== undefined) {
            var newDate = new Date(date);
            var newTime;
            if (time !== undefined && time !== null) {
                newTime = time;
            }

            var days = ["SUNDAY", "MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY"];
            //var monthNames = ["January", "February", "March", "April", "May", "June","July", "August", "September", "October", "November", "December" ];
            //var dformat = days[newDate.getDay()] + ', ' + monthNames[newDate.getMonth()] + ' ' + newDate.getDate() + ', ' + newDate.getFullYear();
            var dformat = days[newDate.getDay()] + ', ' + DateFormatChange.DateFormatChange(newDate);
            if (time !== undefined && time !== null) {
                //dformat = days[newDate.getDay()] + ', ' + monthNames[newDate.getMonth()] + ' ' + newDate.getDate() + ', ' + newDate.getFullYear() + ':' + newTime;
                dformat = days[newDate.getDay()] + ', ' + DateFormatChange.DateFormatChange(newDate) + ':' + newTime;
            }
            return dformat;
        }
        else {
            return;
        }
    };

    $scope.TrackingDetailJson = function (data1) {
        if (data1 !== undefined && data1 !== null) {
            var data = data1.TrackingDetails;
            data.sort(function (a, b) {
                // Turn your strings into dates, and then subtract them
                // to get a value that is either negative, positive, or zero.
                return new Date(b.Date) - new Date(a.Date);
            });
            // TO DO 
            var newData = {
                Date: null,
                IsCollapsedBulk: true,
                Detail: []
            };
            var trackingdetailNew = [];
            var flag = true;
            for (var i = 0 ; i < data.length; i++) {
                if (i + 1 < data.length) {
                    if (data[i].Date === data[i + 1].Date) {
                        if (flag) {
                            newData.Date = data[i].Date;
                            newData.IsCollapsedBulk = data[i].IsCollapsed;
                            flag = false;
                        }
                        newData.Detail.push(data[i]);
                    }
                    else {
                        if (flag) {
                            newData.Date = data[i].Date;
                            newData.IsCollapsedBulk = data[i].IsCollapsed;
                            flag = false;
                        }
                        newData.Detail.push(data[i]);
                        trackingdetailNew.push(newData);
                        newData = {
                            Date: null,
                            Detail: []
                        };
                        flag = true;
                    }
                }
                else {
                    if (flag) {
                        newData.Date = data[i].Date;
                        newData.IsCollapsedBulk = data[i].IsCollapsed;
                        flag = false;
                    }
                    newData.Detail.push(data[i]);
                    trackingdetailNew.push(newData);
                }
            }
            return trackingdetailNew;
        }

    };

    $scope.ShowHidePickupPanel = function (trackingInfo) {
        trackingInfo.IsPanelShow = !trackingInfo.IsPanelShow;
        if (trackingInfo !== undefined && trackingInfo !== null && trackingInfo.TrackingDetails !== undefined && trackingInfo.TrackingDetails !== null && trackingInfo.TrackingDetails.length > 0) {
            for (var i = 0; i < trackingInfo.TrackingDetails.length; i++) {
                trackingInfo.TrackingDetails[i].IsCollapsed = trackingInfo.IsPanelShow;
            }
        }
    };

    $scope.GetShipemtStatus = function (status) {
        if (status !== null && status !== '' && status !== undefined) {
            var str = status.substring(0, 1);
            var d = status.slice(1);

            return status.substring(0, 1).toUpperCase() + status.slice(1);
        }
        else {
            return;
        }
    };

    $scope.GetCorrectFormattedDatePanel = function (date) {
        // Geting Correct Date Format
        if (date !== null && date !== '' && date !== undefined) {
            var newDate = new Date(date);
            Number.prototype.padLeft = function (base, chr) {
                var len = (String(base || 10).length - String(this).length) + 1;
                return len > 0 ? new Array(len).join(chr || '0') + this : this;
            };

            var d = newDate;
            //var dformat = [d.getDate().padLeft(),
            //             (d.getMonth() + 1).padLeft(),
            //             d.getFullYear()].join('/');
            var dformat = DateFormatChange.DateFormatChange(d);
            return dformat;
        }
        else {
            return;
        }
    };

    $scope.getDayName = function (date) {

        //Day Name
        var days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
        if (date !== null && date !== '' && date !== undefined) {
            var newDate = new Date(date);
            //First of month
            return days[newDate.getDay()];
        }
        else {
            return;
        }
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
                 '<link rel="stylesheet" type="text/css" href="vendor/bootstrap/bootstrap.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/font-awesome/font-awesome.css" />' +
                '<link rel="stylesheet" type="text/css" href="assets/Frayte.App-1.0.0.css" />' +
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
                '<link rel="stylesheet" type="text/css" href="vendor/bootstrap/bootstrap.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/font-awesome/font-awesome.css" />' +
                '<link rel="stylesheet" type="text/css" href="assets/Frayte.App-1.0.0.css" />' +
            '</head><body onload="window.print()">' + printContents + '</html>');
            popupWin.document.close();
        }
        popupWin.document.close();

        return true;
    };

    $scope.ShowHideDetail = function (trackingInfo) {
        if (trackingInfo !== undefined && trackingInfo !== null && trackingInfo.IsHeaderShow) {
            trackingInfo.IsShowHideDetail = !trackingInfo.IsShowHideDetail;
            if (trackingInfo.IsShowHideDetail) {
                //$anchorScroll.yOffset = 40;
                setScroll(trackingInfo.TrackingNumber);
            }
        }
        if (trackingInfo.ShowHideValue === "Hide") {
            //$scope.ShowHide = "Show";
            $translate('Show').then(function (Show) {
                trackingInfo.ShowHideValue = Show;
            });
        }
        else if (trackingInfo.ShowHideValue === "Show") {
            //$scope.ShowHide = "Hide";
            $translate('Hide').then(function (Hide) {
                trackingInfo.ShowHideValue = Hide;
            });
        }
    };

    var setScroll = function (printSectionId) {
        $location.hash(printSectionId);
        $anchorScroll();
    };

    $scope.GetEstimatedWeight = function (weight, CarrierCompany) {
        if (CarrierCompany !== "UPS") {
            if (weight !== null && weight !== '' && weight !== undefined) {
                var weightInKg = parseFloat(weight) / 35.274;
                var estimatedWeight = weightInKg.toFixed(2);
                if (estimatedWeight >= 0.5 && estimatedWeight <= 30) {
                    var num = [];
                    num = estimatedWeight.toString().split('.');
                    if (num.length > 1) {
                        var as = parseFloat(num[1]);
                        if (as === 0) {
                            return parseFloat(num[0]).toFixed(2);
                        }
                        else {
                            if (as > 49) {
                                return parseFloat(num[0]) + 1;
                            }
                            else {
                                return parseFloat(num[0]) + 0.50;
                            }
                        }

                    }
                    else {
                        return parseFloat(num[0]).toFixed(2);
                    }
                }
                else if (estimatedWeight > 30) {
                    return Math.ceil(estimatedWeight);
                }
                else {
                    return weight;
                }
            }
            else {
                return;
            }
        }
        else {
            return weight;
        }

    };
    //End Express Tracking

    function init() {
        //panel collapse code
        //$scope.oneCollapse = false;
        //$scope.twoCollapse = false;
        //$scope.threeCollapse = false;
        //ene
        $scope.ShowMultiButton = true;
        $scope.IsShowTrackingData = false;
        $scope.TrackingArray = [];
        $scope.collapse = false;
        $scope.TradelaneAirPickType = [{
            Id: 1,
            Name: "MAWB"
        },
       {
           Id: 2,
           Name: "Shipment Reference No"
       }];
        $scope.AirpickDetail = {
            AirPick: {}
        };
        if (config.SITE_COUNTRY == "CO.UK") {
            $scope.FrayteWebsite = "frayte.co.uk";
        } else if (config.SITE_COUNTRY == "COM") {
            $scope.FrayteWebsite = "frayte.com";
        } else if (config.SITE_COUNTRY == "CO.TH") {
            $scope.FrayteWebsite = "frayte.co.th";
        }
        //if ($state.params.TrackingType === "mawb") {
        //    $scope.AirpickDetail.AirPick = $scope.TradelaneAirPickType[0];
        //}
        //else if ($state.params.TrackingType === "frn") {
        //    $scope.AirpickDetail.AirPick = $scope.TradelaneAirPickType[1];
        //}
        //else if ($state.params.TrackingType === "hawb") {
        //    $scope.AirpickDetail.AirPick.Id = 0;
        //    $scope.AirpickDetail.AirPick.Name = "HAWB";
        //}

        //$scope.AirpickDetail.AirPicktype = $state.params.SearchNumber;
        if ($state.params.SearchNumber !== undefined && $state.params.SearchNumber !== null && $state.params.SearchNumber !== "") {
            $scope.SearchTracking($state.params.SearchNumber);
        }
        $scope.TrackingDetail = {
            TradelaneOperationalDetail: {},
            TRadelaneTrackingList: {},
            ShipmentDetail: {},
            CurrentStauts: {},
            TRadelaneShipmentStatus: {}
        };
        $scope.TrackingListDetail = [];
        //$scope.TradelaneShipmentId = $state.params.TradelaneShipmentId;
        //$scope.SearchTradelaneTracking(true, $scope.AirpickDetail);
        //$scope.GetUpdateTracking();
        setMultilingualOptions();
        $scope.setScroll('top');
        $scope.allCollapse = false;
        $scope.module = true;
        $scope.first = false;
        //$scope.Detail = {
        //    TrackingNumber : ""
        //};

    }
    init();
});