angular.module('ngApp.hsCode').controller('HSCodeDetailController', function (AppSpinner, HSCodeService, JobService, eCommerceBookingService, $scope, $uibModalInstance, uiGridConstants, shipmentId, ShipmentStatus, DirectShipmentService, config, DirectBookingService, $rootScope, $translate, $location, Upload, $state, $filter, ShipmentService, TimeZoneService, CountryService, CourierService, HomeService, ShipperService, ReceiverService, SessionService, $uibModal, $log, toaster, $stateParams, CustomerService, UserService, ModalService, $uibModalStack) {

    // HSCode
    $scope.detectHSCodeChange = function (Package, Type) {
        if (Package.IsHSCodeSet) {
            if (Type === "HSCode") {
                Package.Content = "";
            }
            else if (Type === "Content") {
                Package.HSCode = "";
            }
            JobService.SetShipmentHSCode(Package.eCommerceShipmentDetailId, "", Package.Content).then(function (response) {
                if (response.data.Status) {
                    Package.IsHSCodeSet = false;
                }
                else {
                    Package.IsHSCodeSet = true;
                    toaster.pop({
                        type: 'error',
                        title: "Frayte-Error",
                        body: "Could not set the HSCode. Try again.",
                        showCloseButton: true
                    });
                }
            }, function () {
                Package.IsPrinted = true;
                toaster.pop({
                    type: 'error',
                    title: "Frayte-Error",
                    body: "Could not set the HSCode. Try again.",
                    showCloseButton: true
                });
            });
        }
    };

    $scope.submit = function () {
        $uibModalInstance.close();
    };

    var setMultilingualOptions = function () {
        $translate(['Success', 'Record_Saved', 'FrayteError', 'FRAYTE_HSCode_Error', 'FrayteError_Validation', 'ErrorSavingRecord', 'FrayteWarning_Validation', 'PleaseCorrectValidationErrors']).then(function (translations) {
            $scope.Success = translations.Success;
            $scope.RecordSaved = translations.Record_Saved;
            $scope.FrayteErrorValidation = translations.FrayteError_Validation;
            $scope.ErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
            $scope.PleaseCorrectValidationErrors = translations.PleaseCorrectValidationErrors;
            $scope.FrayteError = translations.FrayteError;
            $scope.FRAYTE_HSCode = translations.FRAYTE_HSCode_Error;

        });
    };

    $scope.onSelect = function ($item, $model, $label, $event, Package) {
        JobService.SetShipmentHSCode(Package.eCommerceShipmentDetailId, $item.HSCode, $item.Description).then(function (response) {
            if (response.data.Status) {
                Package.HSCode = $item.HSCode;
                Package.Content = $item.Description;
                Package.IsHSCodeSet = true;
            }
            else {
                Package.IsHSCodeSet = false;
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.FRAYTE_HSCode,
                    showCloseButton: true
                });
            }
        }, function () {
            Package.IsHSCodeSet = false;
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.FRAYTE_HSCode,
                showCloseButton: true
            });
        });
    };

    $scope.getHSCodes = function (query, serachType) {

        return HSCodeService.GetHSCodes(query, $scope.eCommerceBookingDetail.ShipTo.Country.CountryId, serachType).then(function (response) {
            return response.data;
        });
    };

    $scope.alerMethod = function (Pakage) {
        $scope.PackageToPrint = Pakage;
        for (var a = 0 ; a < $scope.eCommerceBookingDetail.Packages.length; a++) {
            if ($scope.eCommerceBookingDetail.Packages[a].IsSelected) {
                $scope.eCommerceBookingDetail.Packages[a].IsSelected = false;
            }
        }
        Pakage.IsSelected = true;
        $scope.Printlabel = Pakage;
        if (Pakage.LabelName === null || Pakage.LabelName === '') {
            $scope.disablePrint = true;
        }
        else {
            $scope.disablePrint = false;
        }
    };

    var setHSCodeProperty = function () {
        for (var i = 0 ; i < $scope.eCommerceBookingDetail.Packages.length; i++) {
            $scope.eCommerceBookingDetail.Packages[i].IsSelected = false;
            if ($scope.eCommerceBookingDetail.Packages[i].HSCode !== null && $scope.eCommerceBookingDetail.Packages[i].HSCode !== "") {
                $scope.eCommerceBookingDetail.Packages[i].IsHSCodeSet = true;
            }
            else {
                $scope.eCommerceBookingDetail.Packages[i].IsHSCodeSet = false;
            }
            $scope.eCommerceBookingDetail.Packages[i].Weight = $scope.eCommerceBookingDetail.Packages[i].Weight.toFixed(1);
        }
    };
    var geteCommerceBookingDetail = function (shipmentId) {
        CallingType = "";
        eCommerceBookingService.GeteCommerceBookingDetail(shipmentId, '').then(function (response) {

            $scope.eCommerceBookingDetail = response.data;


            $scope.IsDisabled = false;
            $scope.TrackingType = "DHLExpress";

            setHSCodeProperty();

            if ($scope.eCommerceBookingDetail.PakageCalculatonType = "kgToCms") {
                $translate('KG').then(function (kg) {
                    $scope.KgLb = kg;
                });
                $translate('CM').then(function (cm) {
                    $scope.CMInchs = cm;
                });
            }
            else if ($scope.eCommerceBookingDetail.PakageCalculatonType = "lbToInchs") {
                $translate('LB').then(function (lb) {
                    $scope.KgLb = lb;
                });
                $translate('Inchs').then(function (inchs) {
                    $scope.CMInchs = inchs;
                });
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteErrorValidation,
                body: $scope.GettingDetailsError,
                showCloseButton: true
            });
        });

    };

    $scope.getTotalValue = function () {
        if ($scope.eCommerceBookingDetail === undefined) {
            return 0;
        }
        else if ($scope.eCommerceBookingDetail.Packages === undefined) {
            return 0;
        }
        else if ($scope.eCommerceBookingDetail.Packages.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.eCommerceBookingDetail.Packages.length; i++) {
                var product = $scope.eCommerceBookingDetail.Packages[i];
                if (product.Value === null || product.Value === undefined) {
                    total += parseFloat(0);
                }
                else {
                    total = total + parseFloat(product.Value);
                }
            }
            return Math.ceil(total);
        }
        else {
            return 0;
        }
    };
    $scope.getChargeableWeight = function (items, prop) {
        if ($scope.eCommerceBookingDetail === undefined) {
            return;
        }

        else if ($scope.eCommerceBookingDetail.Packages === undefined || $scope.eCommerceBookingDetail.Packages === null) {
            return 0;
        }

        if ($scope.eCommerceBookingDetail.Packages.length >= 0) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.eCommerceBookingDetail.Packages.length; i++) {
                var product = $scope.eCommerceBookingDetail.Packages[i];
                var len = 0;
                var wid = 0;
                var height = 0;
                var qty = 0;
                if (product.Length === null || product.Length === undefined) {
                    len = parseFloat(0);
                }
                else {
                    len = parseFloat(product.Length);
                }

                if (product.Width === null || product.Width === undefined) {
                    wid = parseFloat(0);
                }
                else {
                    wid = parseFloat(product.Width);
                }

                if (product.Height === null || product.Height === undefined) {
                    height = parseFloat(0);
                }
                else {
                    height = parseFloat(product.Height);
                }
                if (product.CartoonValue === null || product.CartoonValue === undefined) {
                    qty = parseFloat(0);
                }
                else {
                    qty = parseFloat(product.CartoonValue);
                }
                if (len > 0 && wid > 0 && height > 0) {
                    total += ((len * wid * height) / 5000) * qty;
                }
            }


            var sum = total.toFixed(2);
            if (sum === 0.00) {
                return 0;
            }
            else {
                var num = [];
                num = sum.toString().split('.');
                var kgs = parseFloat($scope.getTotalKgs());
                if (num.length > 1) {
                    var as = parseFloat(num[1]);
                    if (as === 0) {
                        if (kgs > parseFloat(num[0]).toFixed(1)) {
                            return $scope.getTotalKgs();
                        }
                        else {
                            return parseFloat(num[0]).toFixed(1);
                        }

                    }
                    else {
                        if (as > 49) {

                            var r = parseFloat(num[0]) + 1;
                            if (kgs > r.toFixed(1)) {
                                return $scope.getTotalKgs();
                            }
                            else {
                                return r.toFixed(1);
                            }

                        }
                        else {
                            var s = parseFloat(num[0]) + 0.50;
                            if (kgs > s.toFixed(1)) {
                                return $scope.getTotalKgs();
                            }
                            else {
                                return s.toFixed(1);
                            }
                        }
                    }

                }
                else {
                    if (kgs > parseFloat(num[0]).toFixed(1)) {
                        return $scope.getTotalKgs();
                    }
                    else {
                        return parseFloat(num[0]).toFixed(1);
                    }
                }
            }
        }

    };

    $scope.getTotalPieces = function () {
        if ($scope.eCommerceBookingDetail === undefined) {
            return;
        }
        if ($scope.eCommerceBookingDetail.Packages !== null && $scope.eCommerceBookingDetail.Packages.length > 0) {
            var pieces = 0;
            for (var i = 0; i < $scope.eCommerceBookingDetail.Packages.length; i++) {
                pieces += $scope.eCommerceBookingDetail.Packages[i].CartoonValue;
            }
            return pieces;
        }
    };

    $scope.TrackShipment = function (TrackingCode) {
        if (TrackingCode !== undefined && TrackingCode !== null && TrackingCode !== '' && $scope.eCommerceBookingDetail.CustomerRateCard !== null && $scope.eCommerceBookingDetail.CustomerRateCard.CourierName !== null && $scope.eCommerceBookingDetail.CustomerRateCard.CourierName !== '') {
            if ($scope.eCommerceBookingDetail.CustomerRateCard.CourierName === "UK/EU - Shipment") {
                $state.go('home.tracking-hub', { carrierType: "UKEUShipment", trackingId: TrackingCode });
            }
            else {
                $state.go('home.tracking-hub', { carrierType: $scope.eCommerceBookingDetail.CustomerRateCard.CourierName, trackingId: TrackingCode });
            }
            $state.go('', {}, { reload: true });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarningValidation,
                body: $scope.TrackShipmentNotTrackError,
                showCloseButton: true
            });
        }
    };
    function init() {
        $rootScope.GetServiceValue = null;
        $scope.TrackingType = '';
        $scope.IsDisabled = true;

        $scope.photoUrl = config.BUILD_URL + "Red%20Label.png";

        var userInfo = SessionService.getUser();
        if (userInfo !== undefined) {
            $scope.RoleId = userInfo.RoleId;
        }
        $scope.ShipmentId = shipmentId;
        $scope.Status = ShipmentStatus;

        geteCommerceBookingDetail($scope.ShipmentId);

        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        setMultilingualOptions();

    }

    init();
});
