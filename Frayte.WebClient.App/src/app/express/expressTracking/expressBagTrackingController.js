angular.module('ngApp.express').controller("expressBagTrackingController", function ($scope, $uibModal, $translate, AppSpinner, $location, $anchorScroll, ModalService, SessionService, config, ExpressShipmentService, DateFormatChange, BagId) {

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
            controller: 'breakbulkTrackingDetailController',
            templateUrl: 'breakbulk/trackingDetails/breakbulkShipmentDetail.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg'
        });
    };


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

    $scope.GetExpressTracking = function () {
        ExpressShipmentService.GetExpressBagTracking($scope.BagId).then(function (response) {
            if (response.data !== undefined && response.data !== '' && response.data !== null) {
                $scope.AWBTracking = response.data;
                //for (i = 0; i < response.data.length; i++) {
                //    response.data[i].ScannedOn = DateFormatChange.DateFormatChange(response.data[i].ScannedOn);
                //}
                //$scope.totalItemCount = response.data[0].TotalRows;
                if (response.data != null && response.data.Tracking !== null) {
                    $scope.Iserrorshow = true;
                    AppSpinner.showSpinnerTemplate();
                    //    $scope.trackingInfos = response.data;
                    if (response.data.Tracking[0].IsHeaderShow) {
                        $translate('Hide').then(function (Hide) {
                            $scope.ShowHide = Hide;
                        });
                    }
                    else {
                        $translate('Show').then(function (Show) {
                            $scope.ShowHide = Show;
                        });
                    }
                    var json = response.data.Tracking;
                    if (json.length > 0) {
                        for (var i = 0 ; i < json.length; i++) {
                            json[i].TrackingDetails = $scope.TrackingDetailJson(json[i]);
                        }
                        $scope.trackingInfos = json;
                    }
                    //showDHLEstimatedWeight();
                    $scope.parcelHub = true;
                    AppSpinner.hideSpinnerTemplate();
                }
                else if (response.data === null || response.data.Tracking === null) {

                    //$state.go('home.tracking-error', { "trackingId": trackingNumber });
                    $scope.Iserrorshow = true;
                }
            }
            else {
                $scope.AWBTracking = response.data;
                $scope.totalItemCount = 0;
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

    function init() {
        $scope.ImagePath = config.BUILD_URL;
        var userInfo = SessionService.getUser();
        $scope.RoleId = userInfo.RoleId;
        $scope.createdBy = userInfo.EmployeeId;
        if (userInfo.OperationZoneId == 2) {
            $scope.FrayteWebsite = "frayte.co.uk";
        }
        else {
            $scope.FrayteWebsite = "frayte.com";
        }
        if (BagId != null) {
            $scope.BagId = BagId;
        }
        $scope.GetExpressTracking();
    }
    init();

});