angular.module('ngApp.directBooking').controller('eCommerceBookingDetailController', function (AppSpinner, eCommerceBookingService, $scope, $uibModalInstance, uiGridConstants, shipmentId, ShipmentStatus, DirectShipmentService, config, DirectBookingService, $rootScope, $translate, $location, Upload, $state, $filter, ShipmentService, TimeZoneService, CountryService, CourierService, HomeService, ShipperService, ReceiverService, SessionService, $uibModal, $log, toaster, $stateParams, CustomerService, UserService, ModalService, $uibModalStack) {
    $scope.IsPrinted = function (Pakage) {
        if (Pakage !== undefined && Pakage !== null && Pakage.LabelName !== null && Pakage.LabelName !== '') {
            return Pakage.IsPrinted;
        }
        else {
            return;
        }
    };

    $scope.setPrintLabel = function (Pakage) {
        if (Pakage !== undefined && Pakage !== null) {
            setLabelPrintedStatus(Pakage);
        }
    };
    $scope.GetCorrectFormattedDate = function (date) {
        // Geting Correct Date Format
        if (date !== null && date !== '' && date !== undefined) {
            var newDate = new Date(date);
            Number.prototype.padLeft = function (base, chr) {
                var len = (String(base || 10).length - String(this).length) + 1;
                return len > 0 ? new Array(len).join(chr || '0') + this : this;
            };

            var d = newDate;
            var dformat = [d.getDate().padLeft(),
                         (d.getMonth() + 1).padLeft(),
                         d.getFullYear()].join('/');
            return dformat;
        }
        else {
            return;
        }
    };

    $scope.CloneDirectShipment = function () {
        $rootScope.manageDirectBookingChange = false;
        // ToDo: Need to check who is editing the shipment
        if ($scope.RoleId === 3) {
            $state.go('customer.booking-home.eCommerce-booking-clone', { shipmentId: $scope.ShipmentId }, { reload: true });
        }
        //else if ($scope.RoleId === 1) {
        //    $state.go('admin.booking-home.eCommerce-booking-clone', { directShipmentId: $scope.ShipmentId }, { reload: true });
        //}
        //else if ($scope.RoleId === 6) {
        //    $state.go('dbuser.booking-home.eCommerce-booking-clone', { directShipmentId: $scope.ShipmentId }, { reload: true });
        //}
        $uibModalStack.dismissAll();
    };

    $scope.gotoClonePage = function () {

    };
    var setLabelPrintedStatus = function (Pakage) {
        if (Pakage.LabelName !== null && Pakage.LabelName !== '') {
            eCommerceBookingService.SetPrintPackageStatus(Pakage).then(function (response) {
                if (response.data !== null && response.data.Status) {
                    Pakage.IsPrinted = true;
                }
            }, function () {
                Pakage.IsPrinted = false;
            });
        }
    };

    var SetMultilingualOptions = function () {
        $translate(['Success', 'Record_Saved', 'FrayteError_Validation', 'ErrorSavingRecord', 'GettingDetails_Error', 'CancelShipmentError_Validation',
            'GeneratePdfError_Validation', 'GeneratePdfError_Validation', 'SuccessfullySendlLabel_Validation', 'SendingMailError_Validation',
        'EnterValidEmailAdd', 'TrackShipmentNotTrack_Error']).then(function (translations) {
            $scope.Success = translations.Success;
            $scope.RecordSaved = translations.Record_Saved;
            $scope.FrayteErrorValidation = translations.FrayteError_Validation;
            $scope.ErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.GettingDetailsError = translations.GettingDetails_Error;
            $scope.CancelShipmentErrorValidation = translations.CancelShipmentError_Validation;
            $scope.GeneratePdfErrorValidation = translations.GeneratePdfError_Validation;
            $scope.SuccessfullySendlLabelValidation = translations.SuccessfullySendlLabel_Validation;
            $scope.SendingMailErrorValidation = translations.SendingMailError_Validation;
            $scope.EnterValidEmailAdd = translations.EnterValidEmailAdd;
            $scope.TrackShipmentNotTrackError = translations.TrackShipmentNotTrack_Error;


        });
    };

    var geteCommerceBookingDetail = function (shipmentId) {
        CallingType = "";
        if ($scope.Status === "Draft") {
            CallingType = "ShipmentDraft";
            eCommerceBookingService.GeteCommerceBookingDetailDraft(shipmentId, CallingType).then(function (response) {

                var monthName = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                var newMonth = new Date(response.data.FuelMonth);
                response.data.FuelMonth = monthName[newMonth.getMonth()];

                $scope.eCommerceBookingDetail = response.data;
                if ($scope.eCommerceBookingDetail !== null && $scope.eCommerceBookingDetail.CustomerRateCard !== null &&
                    $scope.eCommerceBookingDetail.CustomerRateCard.CourierName !== '' && $scope.eCommerceBookingDetail.CustomerRateCard.CourierName !== null) {
                    if ($scope.eCommerceBookingDetail.CustomerRateCard.CourierName === "UKMail" ||
                        $scope.eCommerceBookingDetail.CustomerRateCard.CourierName === "Yodel" ||
                        $scope.eCommerceBookingDetail.CustomerRateCard.CourierName === "Hermes") {
                        $scope.IsDisabled = false;
                        $scope.TrackingType = "UKEUShipment";
                    }
                    else {
                        if ($scope.eCommerceBookingDetail.CustomerRateCard.CourierName === "DHL") {
                            $scope.IsDisabled = false;
                            $scope.TrackingType = $scope.eCommerceBookingDetail.CustomerRateCard.CourierName + $scope.eCommerceBookingDetail.CustomerRateCard.RateType;
                        }
                        else {
                            $scope.IsDisabled = false;
                            $scope.TrackingType = $scope.eCommerceBookingDetail.CustomerRateCard.CourierName;
                        }

                    }
                }
                else {
                    $scope.IsDisabled = true;
                }

                for (var i = 0 ; i < $scope.eCommerceBookingDetail.Packages.length; i++) {
                    $scope.eCommerceBookingDetail.Packages[i].IsSelected = false;
                    $scope.eCommerceBookingDetail.Packages[i].Weight = $scope.eCommerceBookingDetail.Packages[i].Weight.toFixed(1);
                }

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
                    title: $scope.Frayte_Error,
                    body: $scope.GettingDetailsError,
                    showCloseButton: true
                });
            });
        }
        else {
            eCommerceBookingService.GeteCommerceBookingDetail(shipmentId, '').then(function (response) {
                var monthName = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                var newMonth = new Date(response.data.FuelMonth);
                response.data.FuelMonth = monthName[newMonth.getMonth()];

                $scope.eCommerceBookingDetail = response.data;

                //switch ($scope.eCommerceBookingDetail.CustomInfo.ContentsType) {
                //    case "documents":
                //        $scope.eCommerceBookingDetail.CustomInfo.ContentsType = "Documents";
                //        break;
                //    case "sample":
                //        $scope.eCommerceBookingDetail.CustomInfo.ContentsType = "Sample";
                //        break;
                //    case "gift":
                //        $scope.eCommerceBookingDetail.CustomInfo.ContentsType = "Gift";
                //        break;
                //    case "merchandise":
                //        $scope.eCommerceBookingDetail.CustomInfo.ContentsType = "Merchandise";
                //        break;
                //    case "returned_goods":
                //        $scope.eCommerceBookingDetail.CustomInfo.ContentsType = "Returned Goods";
                //        break;
                //    case "other":
                //        $scope.eCommerceBookingDetail.CustomInfo.ContentsType = "Other";
                //        break;
                //}

                //switch ($scope.eCommerceBookingDetail.CustomInfo.RestrictionType) {
                //    case "none":
                //        $scope.eCommerceBookingDetail.CustomInfo.RestrictionType = "None";
                //        break;
                //    case "other":
                //        $scope.eCommerceBookingDetail.CustomInfo.RestrictionType = "Other";
                //        break;
                //    case "quarantine":
                //        $scope.eCommerceBookingDetail.CustomInfo.RestrictionType = "Quarantine";
                //        break;
                //    case "sanitary_phytosanitary_inspection":
                //        $scope.eCommerceBookingDetail.CustomInfo.RestrictionType = "Sanitary Phytosanitary Inspection";
                //        break;

                //}

                //switch ($scope.eCommerceBookingDetail.CustomInfo.NonDeliveryOption) {
                //    case "abandon":
                //        $scope.eCommerceBookingDetail.CustomInfo.NonDeliveryOption = "Abandon";
                //        break;
                //    case "return":
                //        $scope.eCommerceBookingDetail.CustomInfo.NonDeliveryOption = "Return";
                //        break;
                //}

                //switch ($scope.eCommerceBookingDetail.CustomInfo.CatagoryOfItem) {
                //    case "sold":
                //        $scope.eCommerceBookingDetail.CustomInfo.CatagoryOfItem = "Sold";
                //        break;
                //    case "samples":
                //        $scope.eCommerceBookingDetail.CustomInfo.CatagoryOfItem = "Samples";
                //        break;
                //    case "gift":
                //        $scope.eCommerceBookingDetail.CustomInfo.CatagoryOfItem = "Gift";
                //        break;
                //    case "documents":
                //        $scope.eCommerceBookingDetail.CustomInfo.CatagoryOfItem = "Documents";
                //        break;
                //    case "commercial_sample":
                //        $scope.eCommerceBookingDetail.CustomInfo.CatagoryOfItem = "Commercial Sample";
                //        break;
                //    case "returned_goods":
                //        $scope.eCommerceBookingDetail.CustomInfo.CatagoryOfItem = "Returned Goods";
                //        break;
                //}


                if ($scope.eCommerceBookingDetail.ShipmentStatusId !== 14) {
                    $scope.generatePdf();
                }
                $scope.IsDisabled = false;
                $scope.TrackingType = "DHLExpress";



                for (var i = 0 ; i < $scope.eCommerceBookingDetail.Packages.length; i++) {
                    $scope.eCommerceBookingDetail.Packages[i].IsSelected = false;
                    $scope.eCommerceBookingDetail.Packages[i].Weight = $scope.eCommerceBookingDetail.Packages[i].Weight.toFixed(1);
                }

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
        }
    };

    $scope.submit = function () {
        $uibModalInstance.close();
    };
    $scope.closePage = function () {
        $uibModalInstance.close();
    };
     
    $scope.generatePdf = function () {
        eCommerceBookingService.DownLoadAsPdf($scope.eCommerceBookingDetail.eCommerceShipmentId, "DHLExpress").then(function (response) {
            if (response.data !== null) {
                $scope.pdfPath = response.data.PackagePath;
                $scope.IsDownloaded = response.data.IsDownloaded;
            }

        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteErrorValidation,
                body: $scope.GeneratePdfErrorValidation,
                showCloseButton: true
            });
        });
    };
    $scope.GetCorrectFormattedDate = function (date) {
        // Geting Correct Date Format
        if (date !== null && date !== '' && date !== undefined) {
            var newDate = new Date(date);
            Number.prototype.padLeft = function (base, chr) {
                var len = (String(base || 10).length - String(this).length) + 1;
                return len > 0 ? new Array(len).join(chr || '0') + this : this;
            };

            var d = newDate;
            var dformat = [d.getDate().padLeft(),
                         (d.getMonth() + 1).padLeft(),
                         d.getFullYear()].join('/');
            return dformat;
        }
        else {
            return;
        }
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
    $scope.getTotalKgs = function () {
        if ($scope.eCommerceBookingDetail === undefined) {
            return;
        }

        else if ($scope.eCommerceBookingDetail.Packages === undefined || $scope.eCommerceBookingDetail.Packages === null) {
            return 0;
        }
        else if ($scope.eCommerceBookingDetail.Packages.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.eCommerceBookingDetail.Packages.length; i++) {
                var product = $scope.eCommerceBookingDetail.Packages[i];
                if (product.Weight === null || product.Weight === undefined) {
                    total += parseFloat(0);
                }
                else {
                    if (product.CartoonValue === undefined || product.CartoonValue === null) {
                        var catroon = parseFloat(0);
                        total = total + parseFloat(product.Weight) * catroon;
                    }
                    else {
                        total = total + parseFloat(product.Weight) * product.CartoonValue;
                    }

                }
            }
            return Math.ceil(total).toFixed(1);
        }
        else {
            return 0;
        }
    };


    $scope.PrintToZebra = function (divName) {
        if ($scope.PackageToPrint !== undefined && $scope.PackageToPrint !== null && $scope.PackageToPrint.LabelName !== null && $scope.PackageToPrint.LabelName !== '') {
            $scope.setPrintLabel($scope.PackageToPrint);
        }
        $scope.removeItem = false;
        var printContents = "<div><img  src='" + $scope.Printlabel.LabelName + "'/></div>";
        var originalContents = document.body.innerHTML;
        var popupWin;
        if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {
            popupWin = window.open('', '_blank', 'width=800,height=600,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
            popupWin.window.focus();
            popupWin.document.write('<!DOCTYPE html><html><head>' +
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
            '</head><body onload="window.print()">' + printContents + '</html>');
            popupWin.document.close();
        }
        popupWin.document.close();

        return true;
    };

    $scope.PrintToA4 = function () {
        if ($scope.PackageToPrint !== undefined && $scope.PackageToPrint !== null && $scope.PackageToPrint.LabelName !== null && $scope.PackageToPrint.LabelName !== '') {
            $scope.setPrintLabel($scope.PackageToPrint);
        }
        var printContents = "<div><img  src='" + $scope.Printlabel.LabelName + "'/></div>";
        var originalContents = document.body.innerHTML;
        var popupWin;
        if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {
            popupWin = window.open('', '_blank', 'width=2480,height=3508,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
            popupWin.window.focus();
            popupWin.document.write('<!DOCTYPE html><html><head>' +
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
            popupWin = window.open('', '_blank', 'width=2480,height=3508');
            popupWin.document.open();
            popupWin.document.write('<html><head>' +
            '</head><body onload="window.print()">' + printContents + '</html>');
            popupWin.document.close();
        }
        popupWin.document.close();

        return true;
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
    $scope.cancelShipment = function () {

        var modalOptions = {
            headerText: "Confirmation",
            bodyText: "Are you sure want to cancel the shipment?"
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            //$scope.spinnerMessage = 'Canceling The Shipment...';

            AppSpinner.showSpinnerTemplate('Canceling The Shipment...', $scope.Template);
            DirectBookingService.CancelShipment($scope.ShipmentId).then(function (response) {
                AppSpinner.hideSpinnerTemplate();
                if (response.status === 200) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'directBooking/eCommerceBookingDetail/directBookingResponse.tpl.html',
                        windowClass: '',
                        size: 'md'

                    });
                    modalInstance.result.then(function () {
                        AppSpinner.hideSpinnerTemplate();
                        // $state.go('customer.direct-shipments', {}, { reload: true });
                    }, function () {
                        AppSpinner.hideSpinnerTemplate();
                    });
                    //toaster.pop({
                    //    type: 'success',
                    //    title: "Success",
                    //    body: "Request have been submitted and someone will be contacted in an hour.",
                    //    showCloseButton: true
                    //});

                }
            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError_Validation,
                    body: $scope.CancelShipmentError_Validation,
                    showCloseButton: true
                });
            });
        });
    };

    //$scope.eCommerceBookingDetailWithLabel = function () {
    //    DirectBookingService.eCommerceBookingDetailWithLabel($scope.ToMail, $scope.ShipmentId).then(function (response) {

    //    }, function () {

    //    });
    //};
    $scope.sendLabelMail = function (IsValid) {
        if (IsValid) {

            AppSpinner.showSpinnerTemplate('Sending Email', $scope.Template);
            eCommerceBookingService.eCommerceBookingDetailWithLabel($scope.ToMail, $scope.ShipmentId, "DHLExpress").then(function (response) {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'success',
                    title: $scope.Success,
                    body: $scope.SuccessfullySendlLabelValidation,
                    showCloseButton: true
                });
            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteErrorValidation,
                    body: $scope.SendingMailErrorValidation,
                    showCloseButton: true
                });
            });
        }
        else {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning_Validation,
                body: $scope.EnterValidEmailAdd,
                showCloseButton: true
            });
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
        $scope.PackageToPrint = null;
        $scope.TrackingType = '';
        $scope.IsDisabled = true;
        $scope.sendMail = false;
        $scope.submitted = false;
        $scope.Printlabel = {
            IsSelected: false
        };

        $scope.photoUrl = config.BUILD_URL + "Red%20Label.png";

        var userInfo = SessionService.getUser();
        if (userInfo !== undefined) {
            $scope.RoleId = userInfo.RoleId;
        }
        //eCommerceBookingDetailJson();
        //var data = $scope.eCommerceBookingDetail.Packages;
        $scope.ShipmentId = shipmentId;
        $scope.Status = ShipmentStatus;
        // $scope.generatePdf();

        geteCommerceBookingDetail($scope.ShipmentId);
        new SetMultilingualOptions();

        $scope.Template = 'directBooking/ajaxLoader.tpl.html';

    }

    init();
});
