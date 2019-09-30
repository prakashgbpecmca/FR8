angular.module('ngApp.directBooking').controller('DirectBookingDetailController', function (AppSpinner, UtilityService, $scope, $uibModalInstance, uiGridConstants, shipmentId, ShipmentStatus, DirectShipmentService, config, DirectBookingService, $rootScope, $translate, $location, Upload, $state, $filter, ShipmentService, CountryService, CourierService, HomeService, ShipperService, ReceiverService, SessionService, $uibModal, $log, toaster, $stateParams, CustomerService, UserService, ModalService, $uibModalStack, $http, $window, QuotationService) {

    $scope.GetCorrectFormattedDatePanel = function (date) {
        if (date) {
            var df = UtilityService.GetForMattedDate(date) + ($scope.directBookingDetail.TimeZone ? " " + $scope.directBookingDetail.TimeZone.OffsetShort : "");
            return UtilityService.GetForMattedDate(date) ;
        }
    };

    $scope.IsPrinted = function (Pakage) {
        if (Pakage !== undefined && Pakage !== null && Pakage.LabelName !== null && Pakage.LabelName !== '') {
            return Pakage.IsPrinted;
        }
        else {
            return;
        }
    };

    $scope.TrackingPage = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'home/trackingnew.tpl.html',
            controller: 'HomeTrackingController',
            windowClass: '',
            size: 'lg',
            resolve: {
                ShipmentData: function () {
                    return;
                },
                ShipmentData1: function () {
                    return row;
                },
                ShipmentData2: function () {
                    return;
                }
            }
        });
        modalInstance.result.then(function () {

        }, function () {
        });
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
            var Mon = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
            var day = d.getDate();
            var Month = d.getMonth();
            var Month1 = Mon[Month];
            var Year = d.getFullYear();
            var dformat = day + "-" + Month1 + "-" + Year;
            return dformat;
        }
        else {
            return;
        }
    };

    $scope.DownloadSupplementaryfile = function () {
        DirectShipmentService.GenerateSupplementoryCharges($scope.ShipmentId).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            if (response.data !== null) {
                var fileInfo = response.data;
                if ((fileInfo.LogisticCompany === 'TNT' && fileInfo.OperationZoneId === 2)) {
                    AppSpinner.hideSpinnerTemplate();
                    //|| (fileInfo.LogisticCompany === 'UPS' && fileInfo.OperationZoneId === 1)
                    //if (fileInfo.LogisticCompany === 'UPS') {
                    //    $scope.Message1 = $scope.ForAdditionalDomestic + ' ' + $scope.ReadInfo;
                    //    $scope.Message2 = $scope.UPSInfo;
                    //    $scope.LogisticCompany = fileInfo.LogisticCompany;
                    //}
                    //else 
                    if (fileInfo.LogisticCompany === 'TNT') {
                        $scope.Message1 = $scope.ForAdditionalDomestic + ' ' + $scope.VisitInfo;
                        $scope.Message2 = $scope.TNTInfo;
                        $scope.LogisticCompany = fileInfo.LogisticCompany;
                    }
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'quotationTools/ukTNTInfoModal.tpl.html',
                        controller: 'ukTNTInfoController',
                        windowClass: '',
                        size: 'md',
                        backdrop: 'static',
                        resolve: {
                            TNTInfo: function () {
                                return $scope.Message1;
                            },
                            TNTInfo1: function () {
                                return $scope.Message2;
                            },
                            LogisticCompany: function () {
                                return $scope.directBookingDetail.CustomerRateCard.CourierName;
                            }
                        }
                    });
                }
                else {
                    var fileName = {
                        FileName: response.data.FileName
                    };

                    $http({
                        method: 'POST',
                        url: config.SERVICE_URL + '/DirectBooking/DownloadSupplemantoryPdf',
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
                                        body: $scope.SuccessfullyDownloadSupplementaryChargePDF,
                                        showCloseButton: true
                                    });
                                }
                            } catch (ex) {
                                $window.open(fileInfo.FilePath, "_blank");
                                console.log(ex);
                            }
                        }
                    }).error(function (data) {
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
            }
            else {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.Frayte_Error,
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

    $scope.CloneDirectShipment = function () {
        $rootScope.manageDirectBookingChange = false;
        // ToDo: Need to check who is editing the shipment

        $state.go('loginView.userTabs.direct-booking-clone', { directShipmentId: $scope.ShipmentId }, { reload: true });

        $uibModalStack.dismissAll();
    };

    $scope.gotoClonePage = function () {

    };

    var setLabelPrintedStatus = function (Pakage) {
        if (Pakage.LabelName !== null && Pakage.LabelName !== '') {
            DirectBookingService.SetPrintPackageStatus(Pakage).then(function (response) {
                if (response.data !== null && response.data.Status) {
                    Pakage.IsPrinted = true;
                }
            }, function () {
                Pakage.IsPrinted = false;
            });
        }
    };

    var setMultilingualOptions = function () {
        $translate(['Success', 'Record_Saved', 'FrayteError_Validation', 'ErrorSavingRecord', 'GettingDetails_Error', 'CancelShipmentError_Validation',
            'GeneratePdfError_Validation', 'GeneratePdfError_Validation', 'SuccessfullySendlLabel_Validation', 'SendingMailError_Validation',
            'EnterValidEmailAdd', 'TrackShipmentNotTrack_Error', 'FrayteWarning', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation', 'ErrorSavingRecord', 'FrayteError', 'FrayteSuccess',
            'Confirmation', 'ShipmentCancelConfirmText', 'ReportCannotDownloadPleaseTryAgain', 'Could_Not_Download_TheReport', 'SuccessfullyDownloadSupplementaryChargePDF',
            'SendingEmail', 'CancellingTheShipment', 'For_Additional_Domestic', 'Visit', 'Read', 'UPS_Info', 'TNT_Info']).then(function (translations) {
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
                $scope.Frayte_Warning = translations.FrayteWarning;
                $scope.PleaseCorrect_ValidationErrors = translations.PleaseCorrectValidationErrors;
                $scope.Successfully_SavedInformation = translations.SuccessfullySavedInformation;
                $scope.ErrorSaving_Record = translations.ErrorSavingRecord;
                $scope.Frayte_Error = translations.FrayteError;
                $scope.Frayte_Success = translations.FrayteSuccess;
                $scope.Confirmation = translations.Confirmation;
                $scope.ShipmentCancelConfirmText = translations.ShipmentCancelConfirmText;
                $scope.ReportCannotDownloadPleaseTryAgain = translations.ReportCannotDownloadPleaseTryAgain;
                $scope.Could_Not_Download_TheReport = translations.Could_Not_Download_TheReport;
                $scope.SuccessfullyDownloadSupplementaryChargePDF = translations.SuccessfullyDownloadSupplementaryChargePDF;
                $scope.SendingEmail = translations.SendingEmail;
                $scope.CancellingTheShipment = translations.CancellingTheShipment;
                $scope.ForAdditionalDomestic = translations.For_Additional_Domestic;
                $scope.VisitInfo = translations.Visit;
                $scope.ReadInfo = translations.Read;
                $scope.UPSInfo = translations.UPS_Info;
                $scope.TNTInfo = translations.TNT_Info;
            });
    };
     
    var getCustomerDetail = function () {

        CustomerService.GetCustomerDetail($scope.directBookingDetail.CreatedByRoleId === 17 ? $scope.directBookingDetail.CreatedByUserId : $scope.directBookingDetail.CustomerId).then(function (response) {

            $scope.customerDetail = response.data;
            var dbr = $scope.customerDetail.AccountNumber.split("");
            var accno = "";
            for (var j = 0; j < dbr.length; j++) {
                accno = accno + dbr[j];
                if (j == 2 || j == 5) {
                    accno = accno + "-";
                }
            }
            $scope.customerDetail.AccountNumber = accno;
        }, function () {
            console.log("Could not get customer detail.");
        });
    };

    var getDirectBookingDetail = function (shipmentId) {
        CallingType = "";
        if ($scope.Status === "Draft") {
            CallingType = "ShipmentDraft";

            DirectBookingService.GetShipmentDraftDetail(shipmentId, CallingType).then(function (response) {

                var monthName = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                var newMonth = new Date(response.data.FuelMonth);
                response.data.FuelMonth = monthName[newMonth.getMonth()] + "-" + newMonth.getFullYear().toString().substr(2, 2);

                $scope.directBookingDetail = response.data;

                if ($scope.RoleId && ($scope.RoleId === 1 || $scope.RoleId === 6 || $scope.RoleId === 3 || $scope.RoleId === 20)) {
                    getCustomerDetail($scope.directBookingDetail.CustomerId);
                }
                if ($scope.directBookingDetail.ParcelType.ParcelType === "Parcel") {
                    $scope.DocType = "(Non Doc)";
                }
                else if ($scope.directBookingDetail.ParcelType.ParcelType === "Letter") {
                    $scope.DocType = "(Doc)";
                }
                if ($scope.directBookingDetail !== null && $scope.directBookingDetail.CustomerRateCard !== null &&
                    $scope.directBookingDetail.CustomerRateCard.CourierName !== '' && $scope.directBookingDetail.CustomerRateCard.CourierName !== null) {
                    if ($scope.directBookingDetail.CustomerRateCard.CourierName === "UKMail" ||
                        $scope.directBookingDetail.CustomerRateCard.CourierName === "Yodel" ||
                        $scope.directBookingDetail.CustomerRateCard.CourierName === "Hermes") {
                        $scope.IsDisabled = false;
                        $scope.TrackingType = "UKEUShipment";
                    }
                    else {
                        if ($scope.directBookingDetail.CustomerRateCard.CourierName === "DHL") {
                            $scope.IsDisabled = false;
                            $scope.TrackingType = $scope.directBookingDetail.CustomerRateCard.CourierName + $scope.directBookingDetail.CustomerRateCard.RateType;
                        }
                        else {
                            $scope.IsDisabled = false;
                            $scope.TrackingType = $scope.directBookingDetail.CustomerRateCard.CourierName;
                        }
                    }
                }
                else {
                    $scope.IsDisabled = true;
                }

                for (var i = 0 ; i < $scope.directBookingDetail.Packages.length; i++) {
                    $scope.directBookingDetail.Packages[i].IsSelected = false;
                    $scope.directBookingDetail.Packages[i].Weight = $scope.directBookingDetail.Packages[i].Weight.toFixed(1);
                }

                if ($scope.directBookingDetail.PakageCalculatonType = "kgToCms") {
                    $translate('KG').then(function (kgs) {
                        $scope.KgLb = kgs;
                    });
                    $translate('kGS').then(function (kg) {
                        $scope.Kg_Lb = kg;
                    });
                    $translate('CM').then(function (cm) {
                        $scope.CMInchs = cm;
                    });
                }
                else if ($scope.directBookingDetail.PakageCalculatonType = "lbToInchs") {
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

            DirectShipmentService.GetDirectBookingDetail(shipmentId, '').then(function (response) {

                var monthName = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                var newMonth = new Date(response.data.FuelMonth);
                response.data.FuelMonth = monthName[newMonth.getMonth()] + "-" + newMonth.getFullYear().toString().substr(2, 2);


                $scope.directBookingDetail = response.data;
                if ($scope.RoleId && ($scope.RoleId === 1 || $scope.RoleId === 6 || $scope.RoleId === 3 || $scope.RoleId === 20)) {
                    getCustomerDetail($scope.directBookingDetail.CustomerId);
                }

                if ($scope.directBookingDetail.ParcelType.ParcelType === "Parcel") {
                    $scope.DocType = "(Non Doc)";
                }
                else if ($scope.directBookingDetail.ParcelType.ParcelType === "Letter") {
                    $scope.DocType = "(Doc)";
                }

                switch ($scope.directBookingDetail.CustomInfo.ContentsType) {
                    case "documents":
                        $scope.directBookingDetail.CustomInfo.ContentsType = "Documents";
                        break;
                    case "sample":
                        $scope.directBookingDetail.CustomInfo.ContentsType = "Sample";
                        break;
                    case "gift":
                        $scope.directBookingDetail.CustomInfo.ContentsType = "Gift";
                        break;
                    case "merchandise":
                        $scope.directBookingDetail.CustomInfo.ContentsType = "Merchandise";
                        break;
                    case "returned_goods":
                        $scope.directBookingDetail.CustomInfo.ContentsType = "Returned Goods";
                        break;
                    case "other":
                        $scope.directBookingDetail.CustomInfo.ContentsType = "Other";
                        break;
                }

                switch ($scope.directBookingDetail.CustomInfo.RestrictionType) {
                    case "none":
                        $scope.directBookingDetail.CustomInfo.RestrictionType = "None";
                        break;
                    case "other":
                        $scope.directBookingDetail.CustomInfo.RestrictionType = "Other";
                        break;
                    case "quarantine":
                        $scope.directBookingDetail.CustomInfo.RestrictionType = "Quarantine";
                        break;
                    case "sanitary_phytosanitary_inspection":
                        $scope.directBookingDetail.CustomInfo.RestrictionType = "Sanitary Phytosanitary Inspection";
                        break;
                }

                switch ($scope.directBookingDetail.CustomInfo.NonDeliveryOption) {
                    case "abandon":
                        $scope.directBookingDetail.CustomInfo.NonDeliveryOption = "Abandon";
                        break;
                    case "return":
                        $scope.directBookingDetail.CustomInfo.NonDeliveryOption = "Return";
                        break;
                }

                switch ($scope.directBookingDetail.CustomInfo.CatagoryOfItem) {
                    case "sold":
                        $scope.directBookingDetail.CustomInfo.CatagoryOfItem = "Sold";
                        break;
                    case "samples":
                        $scope.directBookingDetail.CustomInfo.CatagoryOfItem = "Samples";
                        break;
                    case "gift":
                        $scope.directBookingDetail.CustomInfo.CatagoryOfItem = "Gift";
                        break;
                    case "documents":
                        $scope.directBookingDetail.CustomInfo.CatagoryOfItem = "Documents";
                        break;
                    case "commercial_sample":
                        $scope.directBookingDetail.CustomInfo.CatagoryOfItem = "Commercial Sample";
                        break;
                    case "returned_goods":
                        $scope.directBookingDetail.CustomInfo.CatagoryOfItem = "Returned Goods";
                        break;
                }
                $scope.CourierName = response.data.CustomerRateCard.CourierName;
                $scope.RateType = response.data.CustomerRateCard.RateType;
                if ($scope.RateType === null) {
                    $scope.RateType = '';
                }

                if ($scope.directBookingDetail.ShipmentStatusId !== 14) {
                    $scope.generatePdf();
                }

                if ($scope.directBookingDetail !== null && $scope.directBookingDetail.CustomerRateCard !== null &&
                    $scope.directBookingDetail.CustomerRateCard.CourierName !== '' && $scope.directBookingDetail.CustomerRateCard.CourierName !== null) {
                    if ($scope.directBookingDetail.CustomerRateCard.CourierName === "UKMail" || $scope.directBookingDetail.CustomerRateCard.CourierName === "Yodel" || $scope.directBookingDetail.CustomerRateCard.CourierName === "Hermes") {
                        $scope.IsDisabled = false;
                        $scope.TrackingType = "UKEUShipment";
                    }
                    else {
                        $scope.IsDisabled = false;
                        $scope.TrackingType = $scope.directBookingDetail.CustomerRateCard.CourierName + $scope.directBookingDetail.CustomerRateCard.RateType;
                    }
                    $scope.trackingURL = $scope.url + "/tracking-detail/" + $scope.TrackingType + "/" + $scope.directBookingDetail.TrackingNo + "/";
                }
                else {
                    $scope.IsDisabled = true;
                }

                for (var i = 0 ; i < $scope.directBookingDetail.Packages.length; i++) {
                    $scope.directBookingDetail.Packages[i].IsSelected = false;
                    $scope.directBookingDetail.Packages[i].Weight = $scope.directBookingDetail.Packages[i].Weight.toFixed(1);
                }

                if ($scope.directBookingDetail.PakageCalculatonType = "kgToCms") {
                    $translate('KG').then(function (kgs) {
                        $scope.KgLb = kgs;
                    });
                    $translate('kGS').then(function (kg) {
                        $scope.Kg_Lb = kg;
                    });
                    $translate('CM').then(function (cm) {
                        $scope.CMInchs = cm;
                    });
                }
                else if ($scope.directBookingDetail.PakageCalculatonType = "lbToInchs") {
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

    var getcourierName = function () {

    };

    $scope.generatePdf = function () {
        getcourierName();
        DirectBookingService.DownLoadAsPdf($scope.directBookingDetail.DirectShipmentId, $scope.CourierName, $scope.RateType).then(function (response) {
            if (response.data !== null) {
                $scope.pdfPath = response.data.PackagePath;
                $scope.AllLabelPdfName = response.data.FileName;
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
            var Mon = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
            var day = d.getDate();
            var Month = d.getMonth();
            var Month1 = Mon[Month];
            var Year = d.getFullYear();
            var dformat = day + "-" + Month1 + "-" + Year;
            return dformat;
        }
        else {
            return;
        }
    };

    $scope.GetCorrectFormattedTime = function (Time) {
        var ForMatedTime = "";
        if (Time !== undefined && Time !== null) {

            var T = Time.split('');

            for (i = 0; i < T.length; i++) {
                ForMatedTime = ForMatedTime + T[i];
                if (i === 1) {
                    ForMatedTime = ForMatedTime + ":";
                }
            }
        }
        return ForMatedTime;
    };

    $scope.getTotalValue = function () {
        if ($scope.directBookingDetail === undefined) {
            return 0;
        }
        else if ($scope.directBookingDetail.Packages === undefined) {
            return 0;
        }
        else if ($scope.directBookingDetail.Packages.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.directBookingDetail.Packages.length; i++) {
                var product = $scope.directBookingDetail.Packages[i];
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
        if ($scope.directBookingDetail === undefined) {
            return;
        }

        else if ($scope.directBookingDetail.Packages === undefined || $scope.directBookingDetail.Packages === null) {
            return 0;
        }

        if ($scope.directBookingDetail.Packages.length >= 0) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.directBookingDetail.Packages.length; i++) {
                var product = $scope.directBookingDetail.Packages[i];
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
                    if ($scope.directBookingDetail.PakageCalculatonType === "kgToCms") {
                        total += ((len * wid * height) / 6000) * qty;
                    }
                    else if ($scope.directBookingDetail.PakageCalculatonType === "lbToInchs") {
                        total += ((len * wid * height) / 166) * qty;
                    }
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
    $scope.getTotalWeightKgs = function () {
        if ($scope.directBookingDetail === undefined) {
            return;
        }
        else if ($scope.directBookingDetail.Packages === undefined || $scope.directBookingDetail.Packages === null) {
            return 0;
        }
        else if ($scope.directBookingDetail.Packages.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.directBookingDetail.Packages.length; i++) {
                var product = $scope.directBookingDetail.Packages[i];
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
            return parseFloat(total).toFixed(2);
        }
        else {
            return 0;
        }
    };

    $scope.getTotalKgs = function () {
        if ($scope.directBookingDetail === undefined) {
            return;
        }
        else if ($scope.directBookingDetail.Packages === undefined || $scope.directBookingDetail.Packages === null) {
            return 0;
        }
        else if ($scope.directBookingDetail.Packages.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.directBookingDetail.Packages.length; i++) {
                var product = $scope.directBookingDetail.Packages[i];
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

            sum = total;
            var num = [];
            num = sum.toString().split('.');

            if (num.length > 1) {
                var as = parseFloat(num[1]);
                if (as.toString().length === 1) {
                    as = as.toString() + "0";
                    as = parseFloat(as);
                }
                if (as === 0) {
                    return total.toFixed(2);
                }
                else if (as === 50) {
                    return total.toFixed(2);
                }
                else {
                    if (as > 49) {
                        var r = parseFloat(num[0]) + 1;
                        return r.toFixed(2);

                    }
                    else {

                        var s = parseFloat(num[0]) + 0.50;
                        return s.toFixed(2);
                    }
                }
            }
            else {
                return total.toFixed(2);
            }

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
        var printContents = $scope.Printlabel.LabelName;
        var originalContents = document.body.innerHTML;
        var popupWin;
        if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {
            popupWin = window.open(printContents, '_blank', 'width=800,height=600,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
            popupWin.window.focus();
            popupWin.window.print(popupWin);
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
            popupWin.document.write(printContents);
            popupWin.window.print();
            popupWin.document.close();
        }
        popupWin.document.close();

        return true;
    };

    $scope.PrintToA4 = function () {
        if ($scope.PackageToPrint !== undefined && $scope.PackageToPrint !== null && $scope.PackageToPrint.LabelName !== null && $scope.PackageToPrint.LabelName !== '') {
            $scope.setPrintLabel($scope.PackageToPrint);
        }
        var printContents = $scope.Printlabel.LabelName;
        var originalContents = document.body.innerHTML;
        var popupWin;
        if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {
            popupWin = window.open(printContents, '_blank', 'width=2480,height=3508,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
            popupWin.print();
            popupWin.window.focus();
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
            popupWin.document.write(printContents);
            popupWin.window.print();
            popupWin.document.close();
        }
        popupWin.document.close();

        return true;
    };

    $scope.getTotalPieces = function () {
        if ($scope.directBookingDetail === undefined) {
            return;
        }
        if ($scope.directBookingDetail.Packages !== null && $scope.directBookingDetail.Packages.length > 0) {
            var pieces = 0;
            for (var i = 0; i < $scope.directBookingDetail.Packages.length; i++) {
                pieces += $scope.directBookingDetail.Packages[i].CartoonValue;
            }
            return pieces;
        }
    };

    $scope.alerMethod = function (Pakage) {
        $scope.PackageToPrint = Pakage;
        for (var a = 0 ; a < $scope.directBookingDetail.Packages.length; a++) {
            if ($scope.directBookingDetail.Packages[a].IsSelected) {
                $scope.directBookingDetail.Packages[a].IsSelected = false;
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

            AppSpinner.showSpinnerTemplate($scope.CancellingTheShipment, $scope.Template);
            DirectBookingService.CancelShipment($scope.ShipmentId).then(function (response) {
                AppSpinner.hideSpinnerTemplate();
                if (response.status === 200) {
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'directBooking/directBookingDetail/directBookingResponse.tpl.html',
                        windowClass: '',
                        size: 'md'

                    });
                    modalInstance.result.then(function () {
                        AppSpinner.hideSpinnerTemplate();
                    }, function () {
                        AppSpinner.hideSpinnerTemplate();
                    });
                }
            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteErrorValidation,
                    body: $scope.CancelShipmentErrorValidation,
                    showCloseButton: true
                });
            });
        });
    };

    $scope.sendLabelMail = function (IsValid, ToMail) {
        if (IsValid) {

            AppSpinner.showSpinnerTemplate($scope.SendingEmail, $scope.Template);
            DirectBookingService.DirectBookingDetailWithLabel(ToMail, $scope.ShipmentId, $scope.directBookingDetail.CustomerRateCard.CourierName, $scope.RoleId, $scope.LogInUserId).then(function (response) {
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
                title: $scope.Frayte_Warning,
                body: $scope.EnterValidEmailAdd,
                showCloseButton: true
            });
        }
    };

    $scope.TrackShipment = function (TrackingCode) {
        if (TrackingCode !== undefined && TrackingCode !== null && TrackingCode !== '' && $scope.directBookingDetail.CustomerRateCard !== null && $scope.directBookingDetail.CustomerRateCard.CourierName !== null && $scope.directBookingDetail.CustomerRateCard.CourierName !== '') {
            if ($scope.directBookingDetail.CustomerRateCard.CourierName === "UK/EU - Shipment") {
                $state.go('home.tracking-hub', { carrierType: "UKEUShipment", trackingId: TrackingCode });
            }
            else {
                $state.go('home.tracking-hub', { carrierType: $scope.directBookingDetail.CustomerRateCard.CourierName, trackingId: TrackingCode });
            }
            $state.go('', {}, { reload: true });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.TrackShipmentNotTrackError,
                showCloseButton: true
            });
        }
    };

    function init() {
        $scope.url = SessionService.GetSiteURL();
        $rootScope.GetServiceValue = null;
        $scope.PackageToPrint = null;
        $scope.TrackingType = '';
        $scope.IsDisabled = true;
        $scope.sendMail = false;
        $scope.submitted = false;
        $scope.Printlabel = {
            IsSelected: false
        };
        $scope.Websitefrayte = UtilityService.getPublicSiteName();
        $scope.photoUrl = config.BUILD_URL + "Red%20Label.png";

        var userInfo = SessionService.getUser();
        $scope.USERINFO = userInfo;
        if (userInfo !== undefined) {
            $scope.RoleId = userInfo.RoleId;
        }
        if ($scope.RoleId === 6) {
            $scope.LogInUserId = userInfo.EmployeeId;
        }
        else {
            $scope.LogInUserId = 0;
        }

        $scope.ShipmentId = shipmentId;
        $scope.Status = ShipmentStatus;

        getDirectBookingDetail($scope.ShipmentId);
        //new SetMultilingualOptions();
        setMultilingualOptions();

        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
    }

    init();
});
