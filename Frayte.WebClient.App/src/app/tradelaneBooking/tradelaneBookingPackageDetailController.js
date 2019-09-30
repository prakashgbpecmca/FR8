angular.module('ngApp.tradelaneBooking').controller('TradelanePacakgeDetailController', function ($scope, UtilityService, ModalService, CallingFrom, TradelaneBookingService, FrayteNumber, $state, ShipmentId, PackageCalculatonType, HAWB, HAWBNumber, TotalUploaded, SuccessUploaded, $translate, $uibModalInstance, AppSpinner, config, $filter, $uibModal, SessionService, ShipmentService, Upload, $timeout, toaster, CustomerId) {

    $scope.closePage = function () {
        $uibModalInstance.close();
    };

    $scope.pageChanged = function (track) {
        getScreenInitials("PageChanges");
    };

    $scope.$watch('hawbNumber', function (newVal, oldVal) {

        if (newVal == 1) {
            $scope.isNumberOne = true;
        } else {
            $scope.isNumberOne = false;
        }
        if (newVal > $scope.totalItemCount) {
            $scope.isMaxNumber = true;
        }
        else {
            $scope.isMaxNumber = false;
        }
        $scope.maxNumber = newVal;
    });

    //multilingual translation key
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteWarning', 'FrayteInformation', 'FrayteSuccess', 'ThereIsNoUnassignedJob', 'Confirmation', 'NoAssignedHAWBSureCancel', 'SelectPackageFirstAssignHAWB',
                'ErrorAssigningHAWBTryagain', 'SuccessfullyAssignedHAWB', 'CorrectValidationErrorFirst', 'Insert_Corresponds_No_With_Carton_Quality', 'Loading_Packages',
                'Ship_From_Ship_To_Already_Assign_For_Selected_HAWB_No']).then(function (translations) {
                $scope.FrayteWarning = translations.FrayteWarning;
                $scope.FrayteSuccess = translations.FrayteSuccess;
                $scope.FrayteError = translations.FrayteError;
                $scope.Confirmation = translations.Confirmation;
                $scope.NoAssignedHAWBSureCancel = translations.NoAssignedHAWBSureCancel;
                $scope.SelectPackageFirstAssignHAWB = translations.SelectPackageFirstAssignHAWB;
                $scope.ThereIsNoUnassignedJob = translations.ThereIsNoUnassignedJob;
                $scope.ErrorAssigningHAWBTryagain = translations.ErrorAssigningHAWBTryagain;
                $scope.SuccessfullyAssignedHAWB = translations.SuccessfullyAssignedHAWB;
                $scope.CorrectValidationErrorFirst = translations.CorrectValidationErrorFirst;
                $scope.Insert_Corresponds_No_With_Carton_Quality = translations.Insert_Corresponds_No_With_Carton_Quality;
                $scope.Loading_Packages = translations.Loading_Packages;
                $scope.Ship_From_Ship_To_Already_Assign_For_Selected_HAWB_No = translations.Ship_From_Ship_To_Already_Assign_For_Selected_HAWB_No;
            });
    };

    var getTradelanePackageWeight = function () {
        TradelaneBookingService.GetTradelanePackageWeight($scope.track.ShipmentId).then(function (response) {
            if (response.data) {
                $scope.TradelaneWeight = response.data;
            }
        });
    };

    var getScreenInitials = function () {

        AppSpinner.showSpinnerTemplate($scope.Loading_Packages, $scope.Template);
        TradelaneBookingService.getShipmentPackages($scope.track).then(function (response1) {
            if (response1.data) {
                getTradelanePackageWeight();
                $scope.shipmentStatus();
                if (response1.data.length) {

                    $scope.packages = response1.data;
                    $scope.totalItemCount = response1.data[0].TotalRows;
                    //setDefaultOption();
                }
                else {

                }
                if ($scope.type === 'Detail') {
                    if (HAWBNumber === 1) {
                        $scope.hawbNumbers = [];
                        $scope.hawbNumbers.push($scope.packages[0].HAWB);
                    }
                    $scope.getHAWBShipments();
                }
            }
            else {
                toaster.pop({
                    type: "error",
                    title: $scope.FrayteError,
                    body: $scope.ErrorGettingjobs,
                    showCloseButton: true
                });
            }
            AppSpinner.hideSpinnerTemplate();
        }, function (response) {
            AppSpinner.hideSpinnerTemplate();
            if (response.status !== 401) {
                toaster.pop({
                    type: "error",
                    title: $scope.FrayteError,
                    body: $scope.ErrorGettingjobs,
                    showCloseButton: true
                });
            }
        });
    };

    $scope.pageChanged = function () {
        $scope.selectAll = false;
        getScreenInitials("PageChanged");
        $scope.shipmentStatus();
    };

    $scope.changePagination = function () {
        $scope.selectAll = false;
        $scope.pageChanged();
        $scope.shipmentStatus();
    };

    $scope.selectAllPackage = function () {
        angular.forEach($scope.packages, function (obj) {
            obj.IsSelected = $scope.selectAll;
        });
    };

    $scope.assignHAWB = function (item) {
        if ($scope.hawbNumbers.length == 1) {
            var packs = [];
            if (item.IsSelected) {
                item.HAWB = $scope.hawbNumbers[0];
                angular.forEach($scope.packages, function (obj) {
                    if (obj.IsSelected && obj.HAWB) {
                        packs.push(obj);
                    }
                });
                if (packs.length) {
                    TradelaneBookingService.AssigneedHAWB(packs).then(function (response) {
                        if (response.data.Status) {
                            angular.forEach(packs, function (obj) {
                                obj.IsSelected = false;
                            });
                            $scope.selectAll = false;
                            toaster.pop({
                                type: 'success',
                                title: $scope.FrayteSuccess,
                                body: $scope.SuccessfullyAssignedHAWB,
                                showCloseButton: true
                            });
                            $scope.shipmentStatus();
                            $scope.getHAWBShipments();
                        }
                        else {
                            toaster.pop({
                                type: 'error',
                                title: $scope.FrayteError,
                                body: $scope.ErrorAssigningHAWBTryagain,
                                showCloseButton: true
                            });
                        }
                    }, function () {
                        $scope.shipmentStatus();
                        toaster.pop({
                            type: 'error',
                            title: $scope.FrayteError,
                            body: $scope.ErrorAssigningHAWBTryagain,
                            showCloseButton: true
                        });
                    });
                }
                else {
                    item.HAWB = "";
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteWarning,
                        body: $scope.SelectPackageFirstAssignHAWB,
                        showCloseButton: true
                    });
                }
            }
            else {
                item.HAWB = "";
            }
        }
    };

    //tradelane shipment hawb code
    $scope.tradelaneShipmentHawb = function (TradelaneShipmentDetailId) {
        if ($scope.HAWBNo !== undefined && $scope.HAWBNo !== null && $scope.HAWBNo !== '') {
            TradelaneBookingService.IsHAWBAddressExist($scope.HAWBNo).then(function (response) {
                if (response.data !== undefined && response.data !== null && response.data !== '') {
                    response.data.TradeLaneShipmentDetailId = TradelaneShipmentDetailId;
                    TradelaneBookingService.UpdateHAWBAddress(response.data).then(function (response) {
                        if (response.data !== undefined && response.data !== null && response.data !== '' && response.data === true) {
                            toaster.pop({
                                type: 'warning',
                                title: $scope.FrayteWarning,
                                body: $scope.Ship_From_Ship_To_Already_Assign_For_Selected_HAWB_No,
                                showCloseButton: true
                            });
                        }
                        else {

                        }
                    });
                }
                else {
                    ModalInstance = $uibModal.open({
                        Animation: true,
                        templateUrl: 'tradelaneShipments/tradelaneShipmentsHAWB/tradelaneShipmentsHAWB.tpl.html',
                        controller: 'TradelaneShipmentsHAWBontroller',
                        keyboard: true,
                        windowClass: 'DirectBookingDetail',
                        size: 'lg',
                        backdrop: 'static',
                        resolve: {
                            CustomerId: function () {
                                return $scope.customerId;
                            },
                            CallFrom: function () {
                                return 'PackageDetail';
                            },
                            TLSDetailId: function () {
                                return TradelaneShipmentDetailId;
                            }
                        }
                    });
                    ModalInstance.result.then(function (HAWBaddresses) {
                        if (HAWBaddresses !== undefined && HAWBaddresses !== null && HAWBaddresses !== '') {
                            for (var i = 0; i < $scope.packages.length; i++) {
                                if ($scope.packages[i].TradelaneShipmentDetailId === HAWBaddresses.ShipFrom.TradelaneShipmentDetailId) {
                                    $scope.packages[i].ShipTo = HAWBaddresses.ShipTo;
                                    $scope.packages[i].ShipFrom = HAWBaddresses.ShipFrom;
                                    $scope.packages[i].NotifyParty = HAWBaddresses.NotifyParty;
                                    $scope.packages[i].IsNotifyPartySameAsReceiver = HAWBaddresses.IsNotifyPartySameAsReceiver;
                                }
                            }
                        }
                    });
                }
            });
        }        
    };
    //end

    $scope.changeHAWB = function (item) {
        var packs = [];
        if (item && item.HAWB && item.IsSelected) {
            angular.forEach($scope.packages, function (obj) {
                if (obj.IsSelected) {
                    obj.HAWB = item.HAWB;
                    packs.push(obj);
                }
            });
            if (packs.length) {
                TradelaneBookingService.AssigneedHAWB(packs).then(function (response) {
                    if (response.data.Status) {
                        angular.forEach(packs, function (obj) {
                            obj.IsSelected = false;
                        });
                        $scope.selectAll = false;
                        $scope.shipmentStatus();
                        $scope.getHAWBShipments();
                        $scope.TLSDetailId = item.TradelaneShipmentDetailId;
                        $scope.HAWBNo = item.HAWB;
                        $scope.tradelaneShipmentHawb(item.TradelaneShipmentDetailId);
                    }
                    else {
                        toaster.pop({
                            type: 'error',
                            title: $scope.FrayteError,
                            body: $scope.ErrorAssigningHAWBTryagain,
                            showCloseButton: true
                        });
                    }
                }, function () {
                    $scope.shipmentStatus();
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteError,
                        body: $scope.ErrorAssigningHAWBTryagain,
                        showCloseButton: true
                    });
                });
            }
            else {
                item.HAWB = "";
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarning,
                    body: $scope.SelectPackageFirstAssignHAWB,
                    showCloseButton: true
                });
            }
        }
        else {
            $scope.selectAll = false;
            item.HAWB = "";
            angular.forEach($scope.packages, function (obj) {
                if (obj.IsSelected) {
                    packs.push(obj);
                }
            });
            if (!packs.length) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarning,
                    body: $scope.SelectPackageFirstAssignHAWB,
                    showCloseButton: true
                });
            }
        }
    };

    $scope.closePopUp = function () {
        if ($scope.BookingShipment !== undefined && $scope.BookingShipment !== null && $scope.BookingShipment !== '') {
            if ($scope.BookingShipment.UnAllocatedShipments) {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'tradelaneBooking/hawbConfirmation.tpl.html',
                    controller: 'HAWBConfirmationController',
                    windowClass: 'directBookingDetail',
                    size: 'md',
                    backdrop: 'static',
                    keyboard: false,
                    resolve: {
                        TotalShipments: function () {
                            return $scope.BookingShipment.TotalShipments;
                        },
                        AllocatedShipments: function () {
                            return $scope.BookingShipment.AllocatedShipments;
                        },
                        Type: function () {
                            return "HAWB";
                        }
                    }
                });
                modalInstance.result.then(function (response) {
                    $uibModalInstance.close($scope.packages);
                }, function () {
                });
            }
            else {
                $uibModalInstance.close($scope.packages);
            }
        }
        else {
            $uibModalInstance.close($scope.packages);
        }
    };

    var sortByAscending = function () {
        $scope.newUkShipmentService.Services.sort(compare);
    };

    var sortByDescending = function () {
        $scope.newUkShipmentService.Services.sort(deCompare);
    };

    $scope.$watch('value', function (value) {
        if (value !== undefined && value !== null && value !== '') {
            if (value === 'With HAWB') {
                $scope.IsHAWBShow = true;
                $scope.IsMAWBShow = false;
            }
            else if (value === 'Without HAWB') {
                $scope.IsMAWBShow = true;
                $scope.IsHAWBShow = false;
            }
        }
    });

    function compare(a, b) {
        if (!isNaN(parseInt(a, 10)) && !isNaN(parseInt(b, 10))) {
            if (parseInt(a, 10) < parseInt(b, 10)) { return -1; }
            if (parseInt(a, 10) > parseInt(b, 10)) { return 1; }
        }
        return 0;
    }

    var setHAWBDocumentOrder = function () {
        $scope.hawbShipments.sort(compare);
    };

    $scope.getHAWBShipments = function () {
        TradelaneBookingService.GetGroupedHAWBPieces($scope.track.ShipmentId).then(function (response) {
            $scope.hawbAssignedPackages = response.data;
            $scope.hawbShipments = [];
            if ($scope.hawbNumbers.length) {
                for (var i = 0; i < $scope.hawbNumbers.length; i++) {
                    var obj = {
                        HAWB: '',
                        TotalPackages: 0
                    };
                    var flag = false;
                    for (var j = 0; j < $scope.hawbAssignedPackages.length; j++) {
                        if ($scope.hawbNumbers[i] === $scope.hawbAssignedPackages[j].HAWB) {
                            flag = true;
                            obj.HAWB = $scope.hawbNumbers[i];
                            obj.TotalPackages = $scope.hawbAssignedPackages[j].PackagesCount;
                            break;
                        }
                    }
                    if (flag) {
                        $scope.hawbShipments.push(obj);
                    }
                    else {
                        obj.HAWB = $scope.hawbNumbers[i];
                        obj.TotalPackages = 0;
                        $scope.hawbShipments.push(obj);
                    }
                }
                setHAWBDocumentOrder();
            }
        }, function (response) {

        });
    };

    $scope.shipmentStatus = function () {
        TradelaneBookingService.IsAllHawbAssigned($scope.track.ShipmentId).then(function (response) {
            $scope.BookingShipment = response.data;
        }, function () {

        });
    };

    $scope.createHAWB = function (IsValid) {
        $scope.hawbShipments = [];
        $scope.hawbNumbers = [];
        if (IsValid) {

            if ($scope.isMaxNumber) {
                toaster.pop({
                    type: "warning",
                    title: $scope.FrayteWarning,
                    body: $scope.Insert_Corresponds_No_With_Carton_Quality,
                    showCloseButton: true
                });
                return;
            }
            if ($scope.isNumberOne) {
                toaster.pop({
                    type: "warning",
                    title: $scope.FrayteWarning,
                    body: "Please enter value greater than 1",
                    showCloseButton: true
                });
                return;
            }
            if ($scope.shipmentHAWBs) {
                $scope.hawbNumber = $scope.shipmentHAWBs;
            }
            else {
                TradelaneBookingService.UpdateHAWBNumber($scope.track.ShipmentId, $scope.hawbNumber).then(function (response) {
                    if (response.data.Status) {
                        console.log("updated hawb");
                    }
                    else {
                        console.log("could not update hawb number");
                    }
                }, function () {
                    console.log("could not update hawb number");
                });
            }
            for (var i = 0; i < $scope.hawbNumber; i++) {
                var r = i;
                if ((i + 1).toString().length === 1) {
                    r = "00" + (i + 1);
                }
                else if ((i + 1).toString().length === 2) {
                    r = "0" + (i + 1);
                }
                else if ((i + 1).toString().length === 3) {
                    r = i + 1;
                }
                var num = UtilityService.getTradelaneFrayteRef($scope.FrayteNumber) + r;
                $scope.hawbNumbers.push(num);

                var obj = {
                    HAWB: num,
                    TotalPackages: 0
                };
                $scope.hawbShipments.push(obj);
            }
            $scope.toggleGrid = true;
            setHAWBDocumentOrder();
        }
        else {
            toaster.pop({
                type: "warning",
                title: $scope.FrayteWarning,
                body: $scope.CorrectValidationErrorFirst,
                showCloseButton: true
            });
        }
    };

    $scope.createMAWB = function (IsValid) {
        if (IsValid) {
            if (($scope.mawbNumber.length > 8) || ($scope.mawbNumber.length < 8)) {
                toaster.pop({
                    type: "warning",
                    title: $scope.FrayteWarning,
                    body: $scope.CorrectValidationErrorFirst,
                    showCloseButton: true
                });
            }
            else {
                $scope.hawbNumbers = [];
                //update MAWB as a HAWB
                var obj = {
                    TradelaneShipmentId: $scope.track.ShipmentId,
                    MAWB: $scope.mawbNumber
                };
                $scope.hawbNumbers.push($scope.mawbNumber);
                TradelaneBookingService.upMAWBAsHAWB(obj).then(function (response) {
                    if (response.data) {
                        $scope.toggleGrid = true;
                        $scope.shipmentStatus();
                        $scope.getHAWBShipments();
                    }
                    else {
                        toaster.pop({
                            type: 'error',
                            title: $scope.FrayteError,
                            body: "There is some error ocurred. Please try again after some time",
                            showCloseButton: true
                        });
                    }
                }, function () {
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteError,
                        body: "There is some error ocurred. Please try again after some time",
                        showCloseButton: true
                    });
                });
            }
        } else {
            toaster.pop({
                type: "warning",
                title: $scope.FrayteWarning,
                body: $scope.CorrectValidationErrorFirst,
                showCloseButton: true
            });
        }
    };

    function init() {

        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.submitted = true;

        $scope.toggleMessage = false;
        $scope.shipmentId = ShipmentId;
        $scope.FrayteNumber = FrayteNumber;
        $scope.PackageCalculatonType = PackageCalculatonType;
        $scope.TotalUploaded = TotalUploaded;
        $scope.customerId = CustomerId;
        $scope.callingFrom = CallingFrom;

        if (callingFrom = 'Allocate') {
            TradelaneBookingService.AssignedHAWBDetail(ShipmentId).then(function (response) {
                if (response.data !== undefined && response.data !== null && response.data !== '') {
                    $scope.packages = response.data;
                }
            });
        }

        $scope.SuccessUploaded = SuccessUploaded;
        if ($scope.SuccessUploaded == 1) {
            $scope.value = "Without HAWB";
        } else {
            $scope.value = "With HAWB";
        }
        //$scope.packages = Packages;

        if (HAWB) {
            $scope.toggleGrid = true;
        }
        else {
            $scope.toggleGrid = false;
        }

        if (HAWB) {
            $scope.type = "Detail";

            if (HAWB === 'UnAllocated') {
                $scope.toggleMessage = true;
                $scope.hawb = '';
            }
            else {
                $scope.toggleMessage = false;
                $scope.hawb = HAWB;
            }
        }
        else {
            $scope.hawb = '';
            $scope.type = "Main";
        }

        if (HAWBNumber) {
            if (HAWBNumber > 1) {
                $scope.shipmentHAWBs = HAWBNumber;
                $scope.createHAWB(true);
            }
        }
        else {
            $scope.shipmentHAWBs = 0;
            $scope.toggleGrid = false;
        }

        // Pagination Logic 
        $scope.viewby = 50;
        $scope.currentPage = 1;
        $scope.itemsPerPage = $scope.viewby;
        $scope.totalItemCount = 1000;
        $scope.maxSize = 2; //Number of pager buttons to show
        $scope.numbers = [20, 30, 50, 100];

        // Track obj
        $scope.track = {
            HAWB: $scope.hawb,
            Type: $scope.type,
            ShipmentId: $scope.shipmentId,
            CurrentPage: $scope.currentPage,
            TakeRows: $scope.itemsPerPage
        };

        getScreenInitials();
        setMultilingualOptions();
    }

    init();
});