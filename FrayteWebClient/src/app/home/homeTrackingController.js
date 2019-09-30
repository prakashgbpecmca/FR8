/** 
 * Controller
 */
angular.module('ngApp.home').controller('HomeTrackingController', function (AppSpinner, $scope, $translate, $location, $anchorScroll, $state, $stateParams, config, $filter, CountryService, CourierService, HomeService, SessionService, $uibModal, $log, toaster, DateFormatChange) {

    //This will hide Address Panel by default.    
    $scope.ShowHidePickupPanel = function (trackingInfo) {
        trackingInfo.IsPanelShow = !trackingInfo.IsPanelShow;
        if (trackingInfo !== undefined && trackingInfo !== null && trackingInfo.TrackingDetails !== undefined && trackingInfo.TrackingDetails !== null && trackingInfo.TrackingDetails.length > 0) {
            for (var i = 0; i < trackingInfo.TrackingDetails.length; i++) {
                trackingInfo.TrackingDetails[i].IsCollapsed = trackingInfo.IsPanelShow;
            }
        }
    };
    $scope.ShowHideBulkPickupPanel = function (trackingInfo) {
        $scope.ShowBulkPickupPanel = !$scope.ShowBulkPickupPanel;
        if (trackingInfo !== undefined && trackingInfo !== null && trackingInfo.TrackingDetails !== undefined && trackingInfo.TrackingDetails !== null && trackingInfo.TrackingDetails.length > 0) {
            trackingInfo.IsPanelShow = !trackingInfo.IsPanelShow;
            for (var i = 0; i < trackingInfo.TrackingDetails.length; i++) {
                trackingInfo.TrackingDetails[i].IsCollapsedBulk = $scope.ShowBulkPickupPanel;
            }
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
    //state to Tracking Detail Page
    $scope.GetTrackingDetail = function (carriertype, trackingNumber) {
        if (carriertype === "UKEUShipment") {
            
            AppSpinner.showSpinnerTemplate('Loading Tracking Detail', $scope.Template);
            HomeService.ParcelHubTracking(carriertype, trackingNumber.replace(/\s/g, '')).then(function (response) {
                AppSpinner.hideSpinnerTemplate();
                if (response.data != null && !response.data.Status) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'home/waitTrackingDetail.tpl.html',
                        windowClass: '',
                        size: 'md'
                    });
                    modalInstance.result.then(function () {
                        $state.go('home.welcome');
                    }, function () {
                        $state.go('home.welcome');
                    });
                }

                else if (response.data != null && response.data.Status && response.data.Tracking !== null) {
                    
                    
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
                    showDHLEstimatedWeight();
                    $scope.parcelHub = true;
                    AppSpinner.hideSpinnerTemplate();
                }
                else if (response.data === null || response.data.Tracking === null) {

                    $state.go('home.tracking-error', { "trackingId": trackingNumber });
                }

            }, function () {
                $state.go('home.tracking-error', { "trackingId": trackingNumber });
            });
        }
        else {
            
            AppSpinner.showSpinnerTemplate('Loading Tracking Detail', $scope.Template);
            HomeService.GettrackingDetail(carriertype, trackingNumber.replace(/\s/g, '')).then(function (response) {
                AppSpinner.hideSpinnerTemplate();
                if (response.data != null && !response.data.Status) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'home/waitTrackingDetail.tpl.html',
                        windowClass: '',
                        size: 'md'
                    });
                    modalInstance.result.then(function () {
                        $state.go('home.welcome');
                    }, function () {
                        $state.go('home.welcome');
                    });
                }
                else if (response.data !== null && response.data.Status && response.data.Tracking !== null && response.data.Tracking[0].TrackingDetails !== null && response.data.Tracking[0].TrackingDetails.length) {
                    $scope.trackingInfo = response.data.Tracking;
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

                        showDHLEstimatedWeight();

                    }
                    $scope.parcelHub = false;
                }
                else if (response.data === null || response.data.Tracking === null || response.data.Tracking[0].TrackingDetails === undefined || response.data.Tracking[0].TrackingDetails === null || response.data.Tracking[0].TrackingDetails.length === 0) {

                    $state.go('home.tracking-error', { "trackingId": trackingNumber });
                }

            }, function () {
                $state.go('home.tracking-error', { "trackingId": trackingNumber });
            });
        }
    };

    var showDHLEstimatedWeight = function () {
        for (var a = 0 ; a < $scope.trackingInfos.length; a++) {
            var str = $scope.trackingInfos[a].Carrier;
            var arr = str.split("");
            var text = 0;
            for (var b = 0; b < arr.length; b++) {
                if (arr[b] === "D") {
                    text++;
                    break;
                }
            }
            for (var c = 0; c < arr.length; c++) {
                if (arr[c] === "H") {
                    text++;
                    break;
                }
            }
            for (var d = 0; d < arr.length; d++) {
                if (arr[d] === "L") {
                    text++;
                    break;
                }
            }
            if (text === 3) {
                $scope.ShowDHLWeight = false;
            }
            else {
                $scope.ShowDHLWeight = true;
            }
        }
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

    $scope.GetEstimatedWeight = function (weight) {
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
    };
    var setScroll = function (printSectionId) {
        $location.hash(printSectionId);
        $anchorScroll();
    };
    var setBulkScroll = function (trackingInfo) {
        $location.hash(trackingInfo.TrackingNumber);
        $anchorScroll();
    };
    $scope.ShowBulkDetail = function (ShowHide) {
        if (ShowHide === undefined || ShowHide === null || ShowHide === '') {
            return;
        }
        if (ShowHide === "Hide") {
            return true;
        }
        else {
            return false;
        }
    };
    $scope.BulkShowHideDetail = function (trackingInfo) {
        if (trackingInfo !== undefined && trackingInfo !== null && trackingInfo.IsHeaderShow) {
            trackingInfo.IsShowHideDetail = !trackingInfo.IsShowHideDetail;
            if (trackingInfo.IsShowHideDetail) {
                setBulkScroll(trackingInfo);
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
        }


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
    $scope.TrySearch = function () {
        $state.go("home.new-tracking", {}, { reload: true });
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
    var setShowHide = function () {
        if ($scope.bulkTrackingDetail[i]) {
            $translate('Hide').then(function (Hide) {
                $scope.ShowHide = Hide;
            });
        }
        else {
            $translate('Show').then(function (Show) {
                $scope.ShowHide = Show;
            });
        }
    };

    $scope.TryBulkSearch = function () {
        $state.go("home.bulk-tracking");
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
                 '<link rel="stylesheet" type="text/css" href="vendor/bootstrap/bootstrap.min.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/font-awesome/font-awesome.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/loading-bar/loading-bar.min.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/angular-ui-grid/ui-grid-unstable.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/angular-toaster/toaster.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/df-tab-menu/df-tab-menu.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/textAngular/style.css" />' +
                '<link rel="stylesheet" type="text/css" href="assets/Frayte-1.0.0.css" />' +
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
                '<link rel="stylesheet" type="text/css" href="vendor/bootstrap/bootstrap.min.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/font-awesome/font-awesome.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/loading-bar/loading-bar.min.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/angular-ui-grid/ui-grid-unstable.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/angular-toaster/toaster.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/df-tab-menu/df-tab-menu.css" />' +
                '<link rel="stylesheet" type="text/css" href="vendor/textAngular/style.css" />' +
                '<link rel="stylesheet" type="text/css" href="assets/Frayte-1.0.0.css" />' +
            '</head><body onload="window.print()">' + printContents + '</html>');
            popupWin.document.close();
        }
        popupWin.document.close();

        return true;
    };
    function init() {
        $translate('Hide').then(function (Hide) {
            $scope.bulkHide = Hide;

        });
        $translate('Show').then(function (Show) {
            $scope.bulkShow = Show;
        });
        $scope.bulkTrackingDetail = HomeService.bulkTracking;
        if ($scope.bulkTrackingDetail.length > 0) {
            for (var i = 0; i < $scope.bulkTrackingDetail.length ; i++) {
                var str = $scope.bulkTrackingDetail[i].Carrier;

                if (str !== null && str !== '' && str.substr(0, 16) === "UK/EU - Shipment") {
                    $scope.bulkTrackingDetail[i].IsParcelHub = true;
                }
                else {
                    $scope.bulkTrackingDetail[i].IsParcelHub = false;
                }
                $scope.bulkTrackingDetail[i].TrackingDetails = $scope.TrackingDetailJson($scope.bulkTrackingDetail[i]);

            }

        }
        $scope.IsPiecesShow = true;
        $scope.collapsed = true;
        $scope.ShowDetail = true;
        $scope.removeItem = true;
        $scope.ShowPickupPanel = false;
        $scope.ShowBulkPickupPanel = true;
        $scope.id = $stateParams.trackingId;
        if ($stateParams.RateType !== "") {
            $scope.carriertype = $stateParams.carrierType + "" + $stateParams.RateType;
        }
        else {
            $scope.carriertype = $stateParams.carrierType;
        }
        if ($scope.id !== undefined && $scope.id !== null) {
            $scope.GetTrackingDetail($scope.carriertype, $scope.id);
        }
        setScroll('top');
        $anchorScroll.yOffset = 200;

        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        //$scope.spinnerMessage = 'Loading Tracking Detail...';

        HomeService.GetCurrentOperationZone().then(function (response) {
            if (response.data.OperationZoneId === 1) {
                $scope.FrayteWebsite = 'frayte.com';
            }
            else {
                $scope.FrayteWebsite = 'frayte.co.uk';
            }

        });
        

    }

    init();

});