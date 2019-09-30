angular.module('ngApp.directBooking').controller('eCommerceBookingDetailController', function (AppSpinner, $http, $window, UtilityService, eCommerceBookingService, $scope, $uibModalInstance, uiGridConstants, shipmentId, ShipmentStatus, DirectShipmentService, config, DirectBookingService, $rootScope, $translate, $location, Upload, $state, $filter, ShipmentService, CountryService, CourierService, HomeService, ShipperService, ReceiverService, SessionService, $uibModal, $log, toaster, $stateParams, CustomerService, UserService, ModalService, $uibModalStack, BookingApp) {
    $scope.IsPrinted = function (Pakage, Type) {
        if (Pakage !== undefined && Pakage !== null && Pakage.LabelName !== null && Pakage.LabelName !== '' && Type) {
            if (Type && Type === "Courier Label") {
                return Pakage.IsPrinted;
            }
            else if (Type && Type === "Frayte Label") {
                return Pakage.IsFrayteAWBPrinted;
            }

        }
        else {
            return;
        }
    };

    $scope.GetCorrectFormattedDatePanel = function (date) {
        if (date) {
            return UtilityService.GetForMattedDate(date);
        }
    };

    $scope.setPrintLabel = function (Pakage, Type) {
        if (Pakage !== undefined && Pakage !== null && Type) {
            setLabelPrintedStatus(Pakage, Type);
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
    var setLabelPrintedStatus = function (Pakage, Type) {
        if (Pakage.LabelName !== null && Pakage.LabelName !== '') {
            if (Type && Type === "Courier Label") {
                $scope.FrayteLabelName = "CourierLabel";
            }
            else if (Type && Type === "Frayte Label") {
                $scope.FrayteLabelName = "FrayteLabel";
            }

            eCommerceBookingService.SetPrintPackageStatus(Pakage.eCommercePackageTrackingDetailId, $scope.FrayteLabelName).then(function (response) {
                if (response.data !== null && response.data.Status) {
                    if (Type && Type === "Courier Label") {
                        Pakage.IsPrinted = true;
                    }
                    else if (Type && Type === "Frayte Label") {
                        Pakage.IsFrayteAWBPrinted = true;
                    }


                }
            }, function () {
                Pakage.IsPrinted = false;
                Pakage.IsFrayteAWBPrinted = false;

            });
        }
    };

    var setMultilingualOptions = function () {
        $translate(['FrayteSuccess', 'Record_Saved', 'FrayteError_Validation', 'FrayteWarning_Validation', 'ErrorSavingRecord', 'GettingDetails_Error', 'CancelShipmentError_Validation',
            'GeneratePdfError_Validation', 'GeneratePdfError_Validation', 'SuccessfullySendlLabel_Validation', 'SendingMailError_Validation',
        'EnterValidEmailAdd', 'TrackShipmentNotTrack_Error', 'Confirmation', 'ShipmentCancelConfirmText', 'ErrorDownloadingLabel', 'SuccessfullyDownloadedLabel', 'EnterTrackingDescriptionFirst', 'CouldNotUpdatedTrakcingLaterOn',
        'TrackingUpdatedSuccessfully', 'UpdatingTracking', 'Downloading', 'CancellingTheShipment', 'SendingEmail']).then(function (translations) {
            $scope.Frayte_Success = translations.FrayteSuccess;
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
            $scope.FrayteError_Validation = translations.FrayteError_Validation;
            $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
            $scope.Confirmation = translations.Confirmation;
            $scope.ShipmentCancelConfirmText = translations.ShipmentCancelConfirmText;
            $scope.ErrorDownloadingLabel = translations.ErrorDownloadingLabel;
            $scope.SuccessfullyDownloadedLabel = translations.SuccessfullyDownloadedLabel;
            $scope.EnterTrackingDescriptionFirst = translations.EnterTrackingDescriptionFirst;
            $scope.CouldNotUpdatedTrakcingLaterOn = translations.CouldNotUpdatedTrakcingLaterOn;
            $scope.TrackingUpdatedSuccessfully = translations.TrackingUpdatedSuccessfully;
            $scope.UpdatingTracking = translations.UpdatingTracking;
            $scope.Downloading = translations.Downloading;
            $scope.CancellingTheShipment = translations.CancellingTheShipment;
            $scope.SendingEmail = translations.SendingEmail;
        });
    };

    var getManualTracking = function () {
        eCommerceBookingService.GetManualTracking($scope.ShipmentId).then(function (response) {
            $scope.trackingDetails = response.data;
        }, function () {

        });
    };
    $scope.SaveManualTracking = function () {
        var obj = {
            eCommerceTrackingId: 0,
            eCommerceShipmentId: $scope.ShipmentId,
            TrackingNumber: $scope.eCommerceBookingDetail.Packages[0].TrackingNo,
            FrayteNumber: $scope.eCommerceBookingDetail.FrayteNumber,
            TrackingDescription: "",
            TrackingDescriptionCode: "Code",
            CreatedBy: $scope.CreatedBy,
            CreatedOn: new Date()
        };
        $scope.showEmail = true;

        $scope.trackingDetails.push(obj);

        isManualTrackingDisable();
    };

    var isManualTrackingDisable = function () {
        $scope.IsDisable = false;
        if ($scope.trackingDetails && $scope.trackingDetails.length) {
            for (var i = 0; i < $scope.trackingDetails.length; i++) {
                if (!$scope.trackingDetails[i].eCommerceTrackingId) {
                    $scope.IsDisable = true;
                    break;
                }
            }
        }
    };



    $scope.SentEmailToReceiver = false;
    $scope.UpdateManualBooking = function (obj1, SentEmailToReceiver) {
        if (obj1.TrackingDescription) {
            var obj = {
                SentEmailToReceiver: SentEmailToReceiver ? SentEmailToReceiver : false,
                Tracking: obj1
            };
            AppSpinner.showSpinnerTemplate($scope.UpdatingTracking, $scope.Template);
            eCommerceBookingService.SaveManualTracking(obj).then(function (response) {
                if (response.data && response.data.eCommerceTrackingId) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.Frayte_Success,
                        body: $scope.TrackingUpdatedSuccessfully,
                        showCloseButton: true
                    });
                    obj1.eCommerceTrackingId = response.data.eCommerceTrackingId;
                    AppSpinner.hideSpinnerTemplate();
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteErrorValidation,
                        body: $scope.CouldNotUpdatedTrakcingLaterOn,
                        showCloseButton: true
                    });
                }
                isManualTrackingDisable();
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteErrorValidation,
                    body: $scope.CouldNotUpdatedTrakcingLaterOn,
                    showCloseButton: true
                });
                isManualTrackingDisable();
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarningValidation,
                body: $SCOPE.EnterTrackingDescriptionFirst,
                showCloseButton: true
            });
        }

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

                frayteLabel();




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

                if ($scope.eCommerceBookingDetail.ShipmentStatusId !== 19) {
                    getManualTracking();
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

    var courierLabel = function () {
        eCommerceBookingService.GetLabelFileName($scope.eCommerceBookingDetail.eCommerceShipmentId, "CourierLabel").then(function (response) {
            $scope.CourierLabel = response.data;
        }, function () {
        });
    };
    var frayteLabel = function () {
        eCommerceBookingService.GetLabelFileName($scope.eCommerceBookingDetail.eCommerceShipmentId, "FrayteLabel").then(function (response) {
            $scope.FrayteLabel = response.data;
            if ($scope.eCommerceBookingDetail.BookingApp === "ECOMMERCE_WS") {
            }
            else {
                courierLabel();
            }

        }, function () {
        });
    };

    // Download Label 
    $scope.GenerateAWBPDF = function (Type) {

        $rootScope.GetServiceValue = null;
        if (Type && Type === "Courier Label") {
            $scope.FrayteLabelName = "CourierLabel";
        }
        else if (Type && Type === "Frayte Label") {
            $scope.FrayteLabelName = "FrayteLabel";
        }

        AppSpinner.showSpinnerTemplate($scope.Downloading + Type, $scope.Template);

        eCommerceBookingService.GetLabelFileName($scope.eCommerceBookingDetail.eCommerceShipmentId, $scope.FrayteLabelName).then(function (response) {
            var fileInfo = response.data;
            var fileName = {
                eCommerceShipmentId: $scope.eCommerceBookingDetail.eCommerceShipmentId,
                FileName: response.data.FileName
            };
            if (response.data != null) {
                $http({
                    method: 'POST',
                    url: config.SERVICE_URL + '/eCommerce/DownloadLabelReport',
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
                            AppSpinner.hideSpinnerTemplate();
                            toaster.pop({
                                type: 'success',
                                title: $scope.Frayte_Success,
                                body: $scope.SuccessfullyDownloadedLabel,
                                showCloseButton: true
                            });
                        } catch (ex) {
                            AppSpinner.hideSpinnerTemplate();
                            $window.open(fileInfo.FilePath, "_blank");
                            toaster.pop({
                                type: 'success',
                                title: $scope.Frayte_Success,
                                body: $scope.SuccessfullyDownloadedLabel,
                                showCloseButton: true
                            });
                        }

                    }
                })
               .error(function (data) {
                   AppSpinner.hideSpinnerTemplate();
                   toaster.pop({
                       type: 'error',
                       title: $scope.Frayte_Error,
                       body: $scope.ErrorDownloadingLabel,
                       showCloseButton: true
                   });
               });

            }
            else {
            }
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.Frayte_Error,
                body: $scope.ErrorDownloadingLabel,
                showCloseButton: true
            });
        });
    };


    // Download Label 
    $scope.GeneratePacakgelabel = function (Type, Package) {

        $rootScope.GetServiceValue = null;
        if (Type && Type === "Courier Label") {
            $scope.FrayteLabelName = "CourierLabel";
        }
        else if (Type && Type === "Frayte Label") {
            $scope.FrayteLabelName = "FrayteLabel";
        }

        AppSpinner.showSpinnerTemplate($scope.Downloading + Type, $scope.Template);

        eCommerceBookingService.GeneratePacakgelabel($scope.eCommerceBookingDetail.eCommerceShipmentId, Package.eCommercePackageTrackingDetailId, $scope.FrayteLabelName).then(function (response) {
            var fileInfo = response.data;
            var fileName = {
                eCommerceShipmentId: $scope.eCommerceBookingDetail.eCommerceShipmentId,
                FileName: response.data.FileName
            };
            if (response.data != null) {
                $http({
                    method: 'POST',
                    url: config.SERVICE_URL + '/eCommerce/DownloadLabelReport',
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
                            AppSpinner.hideSpinnerTemplate();
                            toaster.pop({
                                type: 'success',
                                title: $scope.Frayte_Success,
                                body: $scope.SuccessfullyDownloadedLabel,
                                showCloseButton: true
                            });
                        } catch (ex) {
                            AppSpinner.hideSpinnerTemplate();
                            $window.open(fileInfo.FilePath, "_blank");
                            toaster.pop({
                                type: 'success',
                                title: $scope.Frayte_Success,
                                body: $scope.SuccessfullyDownloadedLabel,
                                showCloseButton: true
                            });
                        }

                    }
                })
               .error(function (data) {
                   AppSpinner.hideSpinnerTemplate();
                   toaster.pop({
                       type: 'error',
                       title: $scope.Frayte_Error,
                       body: $scope.ErrorDownloadingLabel,
                       showCloseButton: true
                   });
               });

            }
            else {
            }
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.Frayte_Error,
                body: $scope.ErrorDownloadingLabel,
                showCloseButton: true
            });
        });
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
            headerText: $scope.Confirmation,
            bodyText: $scope.ShipmentCancelConfirmText
        };

        ModalService.Confirm({}, modalOptions).then(function (result) {
            //$scope.spinnerMessage = 'Canceling The Shipment...';

            AppSpinner.showSpinnerTemplate($scope.CancellingTheShipment, $scope.Template);
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
    $scope.sendLabelMail = function (ToMail, IsValid) {
        if (IsValid && ToMail) {
            $scope.ToMail = ToMail;
            AppSpinner.showSpinnerTemplate($scope.SendingEmail, $scope.Template);
            eCommerceBookingService.eCommerceBookingDetailWithLabel($scope.ToMail, $scope.ShipmentId, "DHLExpress").then(function (response) {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'success',
                    title: $scope.Frayte_Success,
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
                title: $scope.FrayteWarningValidation,
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

        $scope.eCommerceShipemntType = UtilityService.eCommerceShipmentType;

        $scope.BookingApp = BookingApp;
        $scope.photoUrl = config.BUILD_URL + "Red%20Label.png";

        var userInfo = SessionService.getUser();
        if (userInfo !== undefined) {
            $scope.RoleId = userInfo.RoleId;
            $scope.CreatedBy = userInfo.EmployeeId;
        }
        //eCommerceBookingDetailJson();
        //var data = $scope.eCommerceBookingDetail.Packages;
        $scope.ShipmentId = shipmentId;
        $scope.Status = ShipmentStatus;
        // $scope.generatePdf();

        geteCommerceBookingDetail($scope.ShipmentId);
       // new setMultilingualOptions();
        setMultilingualOptions();
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';

    }

    init();
});
