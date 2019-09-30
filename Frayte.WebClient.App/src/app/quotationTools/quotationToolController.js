angular.module('ngApp.quotationTools').controller('QuotationToolController', function ($uibModalStack, UtilityService, QuotationService, $http, $window, Upload, $rootScope, AppSpinner, $sce, TopCountryService, $location, TopCurrencyService, $scope, config, $filter, $state, ModalService, $translate, $uibModal, $timeout, toaster, $stateParams, DirectBookingService, SessionService, ZoneBaseRateCardHistoryService) {

    $scope.status = {
        isopen: false
    };

    var setModalOptions = function () {
        $translate(['FrayteWarning', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation', 'ErrorSavingRecord', 'FrayteError', 'FrayteSuccess',
                    'Quotationcannotplaced_offervalidityexpired', 'Report_GeneratedDownloaded_Successfully', 'Quote_GeneratedDownloaded_Successfully', 'Supplementary_Downloaded_Successfully',
                    'Could_Not_Download_TheReport', 'Error_while_downloading', 'SendingMailError_Validation', 'SuccessfullySentMail', 'Quotation_Update',
                    'PleaseCorrectValidationErrors_GetServices', 'Toaster_ErrorMessage', 'Please_SelectService', 'RateCard_GeneratedDownloaded_Successfully',
                    'Error_DownloadingPDF', 'Error_DownloadingExcel', 'Quotation_Save', 'SuccessfullyDelete', 'RateCardDownlodedSuccessfully_Your_Browser_Download_Folder',
                    'PleaseCorrectValidationErrors', 'NoCustomerRateCardForThisCourier', 'MaxWeight70kGS', 'MaxWeight154KGS', 'Nosupplymentry_charges_Courier',
                    'DownloadingQuotePDF', 'PlacingBooking', 'SavingQuotation', 'DownloadingRateCardPDF', 'DownloadingSupplementaryPDF', 'DownloadingRateCardSummary',
                    'For_Additional_Domestic', 'Visit', 'Read', 'UPS_Info', 'TNT_Info']).then(function (translations) {
                        $scope.Frayte_Warning = translations.FrayteWarning;
                        $scope.PleaseCorrect_ValidationErrors = translations.PleaseCorrectValidationErrors;
                        $scope.Successfully_SavedInformation = translations.SuccessfullySavedInformation;
                        $scope.ErrorSaving_Record = translations.ErrorSavingRecord;
                        $scope.Frayte_Error = translations.FrayteError;
                        $scope.Frayte_Success = translations.FrayteSuccess;
                        $scope.Quotationoffervalidityexpired = translations.Quotationcannotplaced_offervalidityexpired;
                        $scope.Report_GeneratedDownloadedSuccessfully = translations.Report_GeneratedDownloaded_Successfully;
                        $scope.Quote_GeneratedDownloadedSuccessfully = translations.Quote_GeneratedDownloaded_Successfully;
                        $scope.Supplementary_DownloadedSuccessfully = translations.Supplementary_Downloaded_Successfully;
                        $scope.CouldNot_Download_TheReport = translations.Could_Not_Download_TheReport;
                        $scope.Errorwhile_downloading = translations.Error_while_downloading;
                        $scope.SendingMailErrorValidation = translations.SendingMailError_Validation;
                        $scope.Successfully_SentMail = translations.SuccessfullySentMail;
                        $scope.PleaseCorrectValidationErrors = translations.PleaseCorrectValidationErrors;
                        $scope.ToasterErrorMessage = translations.Toaster_ErrorMessage;
                        $scope.PleaseSelectService = translations.Please_SelectService;
                        $scope.RateCardDownlodedSuccessfully_Your_Browser_Download_Folder = translations.RateCardDownlodedSuccessfully_Your_Browser_Download_Folder;
                        $scope.ErrorDownloadingPDF = translations.Error_DownloadingPDF;
                        $scope.ErrorDownloadingExcel = translations.Error_DownloadingExcel;
                        $scope.QuotationSave = translations.Quotation_Save;
                        $scope.QuotationUpdate = translations.Quotation_Update;
                        $scope.NoCustomerRateCardForThisCourier = translations.NoCustomerRateCardForThisCourier;
                        $scope.MaxWeight70kGS = translations.MaxWeight70kGS;
                        $scope.MaxWeight154KGS = translations.MaxWeight154KGS;
                        $scope.Nosupplymentry_charges_Courier = translations.Nosupplymentry_charges_Courier;
                        $scope.DownloadingQuotePDF = translations.DownloadingQuotePDF;
                        $scope.PlacingBooking = translations.PlacingBooking;
                        $scope.SavingQuotation = translations.SavingQuotation;
                        $scope.DownloadingRateCardPDF = translations.DownloadingRateCardPDF;
                        $scope.DownloadingSupplementaryPDF = translations.DownloadingSupplementaryPDF;
                        $scope.DownloadingRateCardSummary = translations.DownloadingRateCardSummary;
                        $scope.ForAdditionalDomestic = translations.For_Additional_Domestic;
                        $scope.VisitInfo = translations.Visit;
                        $scope.ReadInfo = translations.Read;
                        $scope.UPSInfo = translations.UPS_Info;
                        $scope.TNTInfo = translations.TNT_Info;
                    });
    };

    $scope.toggleDropdown = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.status.isopen = !$scope.status.isopen;
    };

    $scope.showGetServicePlaceBooking = function (DirectBookingForm) {

        if (DirectBookingForm !== undefined) {

            var flag = false;
            if (
                DirectBookingForm.shipFromCountry !== undefined && DirectBookingForm.shipFromCountry.$valid &&
                DirectBookingForm.shipFromPostcode !== undefined && DirectBookingForm.shipFromPostcode.$valid

                ) {
                flag = true;
            }
            else {
                flag = false;
            }
            if (flag) {
                if (
            DirectBookingForm.shipToCountry !== undefined && DirectBookingForm.shipToCountry.$valid &&
            DirectBookingForm.shipToPostcode !== undefined && DirectBookingForm.shipToPostcode.$valid
            ) {
                    flag = true;
                }
                else {
                    flag = false;
                }
            }

            return flag;
        }
    };

    $scope.GenerateQuotationShipmentPdf = function (IsValid) {
        if (IsValid) {
            if ($scope.RoleId === 1 || $scope.RoleId === 6) {
                $scope.quotationDetail.CustomerId = $scope.CustId;
                $scope.quotationDetail.CustomerName = $scope.CustName;
            }
            else {
                $scope.quotationDetail.CustomerId = $scope.CustId;
                $scope.quotationDetail.CustomerName = $scope.CustName;
            }
            $rootScope.GetServiceValue = null;

            if ($scope.quotationDetail.QuotationRateCard !== null && $scope.quotationDetail.QuotationRateCard.LogisticServiceId > 0) {
                AppSpinner.showSpinnerTemplate($scope.DownloadingQuotePDF, $scope.Template);
                $scope.quotationDetail.TotalEstimatedWeight = $scope.getChargeableWeight();
                if ($scope.quotationDetail.AddressType !== undefined && $scope.quotationDetail.AddressType !== null && $scope.quotationDetail.AddressType !== '') {
                    $scope.quotationDetail.AddressType = $scope.quotationDetail.AddressType.Name;
                }
                QuotationService.GenerateQuotationShipmentPdf($scope.quotationDetail).then(function (response) {
                    var fileInfo = response.data;
                    var fileName = {
                        FileName: response.data.FileName
                    };
                    if (response.data != null) {
                        if ($scope.quotationDetail.AddressType !== undefined && $scope.quotationDetail.AddressType !== null && $scope.quotationDetail.AddressType !== '') {
                            for (i = 0; i < $scope.AddressTypes.length; i++) {
                                if ($scope.quotationDetail.AddressType === $scope.AddressTypes[i].Name) {
                                    $scope.quotationDetail.AddressType = $scope.AddressTypes[i];
                                }
                            }
                        }
                        $http({
                            method: 'POST',
                            url: config.SERVICE_URL + '/Quotation/DownloadQuotationReport',
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
                                        title: $scope.Frayte_Success,
                                        body: $scope.Quote_GeneratedDownloadedSuccessfully,
                                        showCloseButton: true
                                    });
                                    AppSpinner.hideSpinnerTemplate();
                                } catch (ex) {
                                    $window.open(fileInfo.FilePath, "_blank");
                                    AppSpinner.hideSpinnerTemplate();
                                }
                            }
                        })
                       .error(function (data) {
                           AppSpinner.hideSpinnerTemplate();
                           toaster.pop({
                               type: 'error',
                               title: $scope.Frayte_Error,
                               body: $scope.CouldNot_Download_TheReport,
                               showCloseButton: true
                           });
                       });
                    }
                    else {
                        if ($scope.quotationDetail.AddressType !== undefined && $scope.quotationDetail.AddressType !== null) {
                            for (i = 0; i < $scope.AddressTypes.length; i++) {
                                if ($scope.quotationDetail.AddressType === $scope.AddressTypes[i].Name) {
                                    $scope.quotationDetail.AddressType = $scope.AddressTypes[i];
                                }
                            }
                        }
                        AppSpinner.hideSpinnerTemplate();
                    }
                }, function () {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.Frayte_Error,
                        body: $scope.Errorwhile_downloading,
                        showCloseButton: true
                    });
                });
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.Frayte_Warning,
                    body: $scope.PleaseSelectService,
                    showCloseButton: true
                });
            }
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.PleaseCorrect_ValidationErrors,
                showCloseButton: true
            });
        }
    };

    $scope.PlaceBooking = function (IsValid) {
        if ($scope.QuotationShipmentId !== undefined && $scope.QuotationShipmentId > 0) {
            if (IsValid === true) {
                //$rootScope.OptionalServices = $scope.quotationDetail.QuotationRateCard.OptionalServices;
                getPlaceBookinLink($scope.QuotationShipmentId);
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.Frayte_Warning,
                    body: $scope.PleaseCorrect_ValidationErrors,
                    showCloseButton: true
                });
            }
        }
        else {
            toaster.pop({
                type: 'error',
                title: $scope.Frayte_Error,
                body: $scope.ToasterErrorMessage,
                showCloseButton: true
            });
        }
    };

    $scope.SaveQuotation = function (IsValid) {
        if ($scope.quotationDetail.QuotationShipmentId === 0) {
            if (IsValid) {
                if ($scope.RoleId === 1 || $scope.RoleId === 6) {
                    $scope.quotationDetail.CustomerId = $scope.CustId;
                }
                else {
                    $scope.quotationDetail.CustomerId = $scope.CustId;
                }

                if ($scope.quotationDetail.QuotationFromAddress.Country.Code === 'GBR' && $scope.quotationDetail.QuotationToAddress.Country.Code === 'GBR') {
                    $scope.quotationDetail.AddressType = $scope.quotationDetail.AddressType.Name;
                }
                else {
                    $scope.quotationDetail.AddressType = null;
                }

                $rootScope.GetServiceValue = null;

                if ($scope.quotationDetail.QuotationRateCard !== null && $scope.quotationDetail.QuotationRateCard.LogisticServiceId > 0) {
                    if ($scope.Db === 'PlaceBooking') {
                        AppSpinner.showSpinnerTemplate($scope.PlacingBooking, $scope.Template);
                    }
                    else {
                        AppSpinner.showSpinnerTemplate($scope.SavingQuotation, $scope.Template);
                    }
                    $scope.quotationDetail.TotalEstimatedWeight = $scope.getChargeableWeight();
                    QuotationService.SaveQuotation($scope.quotationDetail).then(function (response) {
                        if (response.status === 200) {
                            $scope.QuotationShipmentId = response.data.QuotationDetail.QuotationShipmentId;
                            $scope.quotationDetail = response.data.QuotationDetail;
                            $scope.quotationDetail.ValidDays = $scope.ValidDays;
                            for (i = 0; i < $scope.AddressTypes.length; i++) {
                                if ($scope.quotationDetail.AddressType === $scope.AddressTypes[i].Name) {
                                    $scope.quotationDetail.AddressType = $scope.AddressTypes[i];
                                }
                            }
                            AppSpinner.hideSpinnerTemplate();
                            toaster.pop({
                                type: 'success',
                                title: $scope.Frayte_Success,
                                body: $scope.QuotationSave,
                                showCloseButton: true
                            });
                        }
                        else {
                            toaster.pop({
                                type: 'error',
                                title: $scope.Frayte_Error,
                                body: $scope.ToasterErrorMessage,
                                showCloseButton: true
                            });
                            AppSpinner.hideSpinnerTemplate();
                        }
                    }, function () {
                        AppSpinner.hideSpinnerTemplate();
                        toaster.pop({
                            type: 'error',
                            title: $scope.Frayte_Error,
                            body: $scope.ToasterErrorMessage,
                            showCloseButton: true
                        });
                    });
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.Frayte_Warning,
                        body: $scope.PleaseSelectService,
                        showCloseButton: true
                    });
                }
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.Frayte_Warning,
                    body: $scope.PleaseCorrect_ValidationErrors,
                    showCloseButton: true
                });
            }
        }
    };

    var getPlaceBookinLink = function (QuotationShipmentId) {
        $state.go(UtilityService.GetCurrentRoute($scope.tabs, "userTabs.direct-booking"), { directShipmentId: QuotationShipmentId, callingtype: 'quotation-shipment' }, { reload: true });
    };

    //Change lb to kg Package Details
    $scope.changeKgToLb = function (pieceDetailOption) {
        if (pieceDetailOption === "kgToCms") {
            // for kg
            $translate('kGS').then(function (kGS) {
                $scope.Lb_Kgs = kGS;
            });
            $translate('KG').then(function (KG) {

                $scope.Lb_Kg = KG;
            });
            $translate('CMS').then(function (CMS) {
                $scope.Lb_Inch = CMS;
            });
        }
        if (pieceDetailOption === "lbToInchs") {
            // for LB
            $translate('LB').then(function (LB) {
                $scope.Lb_Kgs = LB;
            });
            $translate('LBs').then(function (LBs) {
                $scope.Lb_Kg = LBs;
            });
            $translate('INCHS').then(function (Inchs) {
                $scope.Lb_Inch = Inchs;
            });

        }
    };

    $scope.viewPastQuotes = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'quotationTools/viewPastQuotations.tpl.html',
            controller: 'QuotationToolViewController',
            windowClass: 'DirectBookingService',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                UserId: function () {
                    return $scope.CreatedBy;
                },
                OperationZoneId: function () {
                    return $scope.OperationZoneId;
                },
                CustomerId: function () {
                    if ($scope.CustId !== null && $scope.CustId !== undefined) {
                        return $scope.CustId;
                    }
                    else {
                        return 0;
                    }
                }
            }
        });

        modalInstance.result.then(function (RateCard) {
            if (RateCard !== undefined && RateCard !== null) {
                $scope.quotationDetail.QuotationRateCard = RateCard;
            }
        }, function () {

        });
    };

    $scope.NewQuotation = function () {
        if ($scope.quotationDetail !== undefined && $scope.quotationDetail !== null) {
            $scope.quotationDetail = {
                "QuotationShipmentId": 0,
                "OperationZoneId": $scope.OperationZoneId,
                "CustomerId": $scope.CustId,
                "QuotationFromAddress": {
                    "PostCode": null,
                    "Country": null
                },
                "QuotationToAddress": {
                    "PostCode": null,
                    "Country": null
                },
                "QuotationPackages": [
                    {
                        "QuotationShipmentDetailId": 0,
                        "QuotationShipmentId": 0,
                        "CartoonValue": null,
                        "Length": null,
                        "Width": null,
                        "Height": null,
                        "Weight": null,
                        "Value": null,
                        "Content": null
                    }
                ],
                "QuotationRateCard": null,
                "CreatedOn": new Date(),
                "CreatedBy": $scope.CreatedBy,
                "PakageCalculatonType": 'kgToCms',
                "LogisticCompany": null,
                "RateType": null,
                "ParcelType": $scope.quotationDetail.ParcelType,
                "ShipmentType": null,
                "LogisticType": null,
                "LogisticTypeDisplay": null,
                "ShipmentTypeDisplay": null,
                "ParcelServiceType": null,
                "RateTypeDisplay": null,
                "LogisticCompanyDisplay": null,
                "BaseRate": 0.0,
                "MarginCost": 0.0,
                "AdditionalSurcharge": 0.0,
                "FuelSurCharge": 0.0,
                "FuelPercent": 0.0,
                "FuelMonthYear": null,
                "EstimatedCost": 0.0,
                "EstimatedTotalCost": 0.0,
                "AddressType": $scope.AddressTypes[0],
                "ValidDays": 0
            };
        }
        else {
            $scope.quotationDetail = {
                "QuotationShipmentId": 0,
                "OperationZoneId": $scope.OperationZoneId,
                "CustomerId": $scope.CustId,
                "QuotationFromAddress": {
                    "PostCode": null,
                    "Country": null
                },
                "QuotationToAddress": {
                    "PostCode": null,
                    "Country": null
                },
                "QuotationPackages": [
                    {
                        "QuotationShipmentDetailId": 0,
                        "QuotationShipmentId": 0,
                        "CartoonValue": null,
                        "Length": null,
                        "Width": null,
                        "Height": null,
                        "Weight": null,
                        "Value": null,
                        "Content": null
                    }
                ],
                "QuotationRateCard": null,
                "CreatedOn": new Date(),
                "CreatedBy": $scope.CreatedBy,
                "PakageCalculatonType": 'kgToCms',
                "LogisticCompany": null,
                "RateType": null,
                "ParcelType": null,
                "ShipmentType": null,
                "LogisticType": null,
                "LogisticTypeDisplay": null,
                "ShipmentTypeDisplay": null,
                "ParcelServiceType": null,
                "RateTypeDisplay": null,
                "LogisticCompanyDisplay": null,
                "BaseRate": 0.0,
                "MarginCost": 0.0,
                "AdditionalSurcharge": 0.0,
                "FuelSurCharge": 0.0,
                "FuelPercent": 0.0,
                "FuelMonthYear": null,
                "EstimatedCost": 0.0,
                "EstimatedTotalCost": 0.0,
                "AddressType": $scope.AddressTypes[0],
                "ValidDays": 0
            };
        }
    };

    $scope.totalPieces = function (quotationDetail) {
        if (quotationDetail !== undefined && quotationDetail !== null && quotationDetail.QuotationPackages !== null && quotationDetail.QuotationPackages.length) {
            var sum = 0;
            for (var i = 0; i < quotationDetail.QuotationPackages.length; i++) {
                if (quotationDetail.QuotationPackages[i].CartoonValue !== "" && quotationDetail.QuotationPackages[i].CartoonValue !== null && quotationDetail.QuotationPackages[i].CartoonValue !== undefined) {
                    sum += Math.abs(parseInt(quotationDetail.QuotationPackages[i].CartoonValue, 10));
                }
            }
            return sum;
        }
        else {
            return 0;
        }
    };

    $scope.getTotalWeightKgs = function () {
        if ($scope.quotationDetail === undefined) {
            return;
        }

        else if ($scope.quotationDetail.QuotationPackages === undefined || $scope.quotationDetail.QuotationPackages === null) {
            return 0;
        }
        else if ($scope.quotationDetail.QuotationPackages.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.quotationDetail.QuotationPackages.length; i++) {
                var product = $scope.quotationDetail.QuotationPackages[i];
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
        if ($scope.quotationDetail === undefined) {
            return;
        }

        else if ($scope.quotationDetail.QuotationPackages === undefined || $scope.quotationDetail.QuotationPackages === null) {
            return 0;
        }
        else if ($scope.quotationDetail.QuotationPackages.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.quotationDetail.QuotationPackages.length; i++) {
                var product = $scope.quotationDetail.QuotationPackages[i];
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

    $scope.getChargeableWeight = function (items, prop) {
        if ($scope.quotationDetail === undefined) {
            return;
        }

        else if ($scope.quotationDetail.QuotationPackages === undefined || $scope.quotationDetail.QuotationPackages === null) {
            return 0;
        }

        if ($scope.quotationDetail.QuotationPackages.length >= 0) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.quotationDetail.QuotationPackages.length; i++) {
                var product = $scope.quotationDetail.QuotationPackages[i];
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
                    if ($scope.quotationDetail.PakageCalculatonType === 'kgToCms') {
                        total += ((len * wid * height) / 5000) * qty;
                    }
                    else if ($scope.quotationDetail.PakageCalculatonType === 'lbToInchs') {
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
                        if (kgs > parseFloat(num[0]).toFixed(2)) {
                            return $scope.getTotalKgs();
                        }
                        else {
                            return parseFloat(num[0]).toFixed(2);
                        }

                    }
                    else {
                        if (as > 49) {

                            var r = parseFloat(num[0]) + 1;
                            if (kgs > r.toFixed(2)) {
                                return $scope.getTotalKgs();
                            }
                            else {
                                return r.toFixed(2);
                            }

                        }
                        else {
                            var s = parseFloat(num[0]) + 0.50;
                            if (kgs > s.toFixed(2)) {
                                return $scope.getTotalKgs();
                            }
                            else {
                                return s.toFixed(2);
                            }
                        }
                    }

                }
                else {
                    if (kgs > parseFloat(num[0]).toFixed(2)) {
                        return $scope.getTotalKgs();
                    }
                    else {
                        return parseFloat(num[0]).toFixed(2);
                    }
                }
            }
        }

    };

    $scope.AddPackage = function () {
        $scope.HideContent = true;
        $scope.quotationDetail.QuotationPackages.push({
            "QuotationShipmentDetailId": 0,
            "QuotationShipmentId": 0,
            "CartoonValue": null,
            "Length": null,
            "Width": null,
            "Height": null,
            "Weight": null,
            "Value": null,
            "Content": null
        });
        var dbpac = $scope.quotationDetail.QuotationPackages.length - 1;
        for (i = 0; i < $scope.quotationDetail.QuotationPackages.length; i++) {

            if (i === dbpac) {
                $scope.quotationDetail.QuotationPackages[i].pacVal = true;
            }
            else {
                $scope.quotationDetail.QuotationPackages[i].pacVal = false;
            }
        }
    };

    $scope.RemovePackage = function (Package) {
        if (Package !== undefined && Package !== null) {
            var index = $scope.quotationDetail.QuotationPackages.indexOf(Package);
            if ($scope.quotationDetail.QuotationPackages.length === 2) {
                $scope.HideContent = false;
            }
            if (index === $scope.quotationDetail.QuotationPackages.length - 1) {
                var dbpac = $scope.quotationDetail.QuotationPackages.length - 2;
                for (i = 0; i < $scope.quotationDetail.QuotationPackages.length; i++) {

                    if (i === dbpac) {
                        $scope.quotationDetail.QuotationPackages[i].pacVal = true;
                    }
                    else {
                        $scope.quotationDetail.QuotationPackages[i].pacVal = false;
                    }
                }
            }
            else {
                var dbpac1 = $scope.quotationDetail.QuotationPackages.length - 1;
                for (i = 0; i < $scope.quotationDetail.QuotationPackages.length; i++) {

                    if (i === dbpac1) {
                        $scope.quotationDetail.QuotationPackages[i].pacVal = true;
                    }
                    else {
                        $scope.quotationDetail.QuotationPackages[i].pacVal = false;
                    }
                }
            }
            $scope.quotationDetail.QuotationPackages.splice(index, 1);
            $scope.Packges = angular.copy($scope.quotationDetail.QuotationPackages);
            $scope.quotationDetail.QuotationPackages = [];
            $scope.quotationDetail.QuotationPackages = $scope.Packges;
        }
    };

    $scope.PackagesTotal = function (items, prop) {
        if (items === null || items === undefined) {
            return 0;
        }
        else {
            return items.reduce(function (a, b) {
                var convertB = 0.0;
                if (b[prop] !== undefined && b[prop] !== null) {
                    convertB = parseFloat(b[prop]);
                }
                var convertA = 0.0;
                if (a !== undefined && a !== null) {
                    convertA = parseFloat(a);
                }

                convertc = convertA + convertB;
                var f = convertc.toFixed(2);
                var swd = Number(parseFloat(convertc).toFixed(2)).toLocaleString('en', {
                    minimumFractionDigits: 2
                });

                return f;

            }, 0);
        }
    };

    var setShipFromStatePostCodeForHKGUK = function (Country) {
        if (Country.Code === 'HKG') {
            $scope.quotationDetail.QuotationFromAddress.PostCode = null;
            $scope.quotationDetail.QuotationFromAddress.State = null;
        }
        else if (Country.Code === 'GBR') {
            $scope.quotationDetail.QuotationFromAddress.State = null;
        }
    };

    var setShipToStatePostCodeForHKGUK = function (Country) {
        if (Country.Code === 'HKG') {
            $scope.quotationDetail.QuotationToAddress.PostCode = null;
            $scope.quotationDetail.QuotationToAddress.State = null;
        }
        else if (Country.Code === 'GBR') {
            $scope.quotationDetail.QuotationToAddress.State = null;
        }
    };

    //Set QuotationToAddress info
    $scope.SetShipToInfo = function (Country) {
        if (Country !== undefined && Country !== null) {
            for (var i = 0 ; i < $scope.CountryPhoneCodes.length ; i++) {
                if ($scope.CountryPhoneCodes[i].Name === Country.Name) {
                    $scope.ShipToPhoneCode = "(+" + $scope.CountryPhoneCodes[i].PhoneCode + ")";
                    break;
                }
            }
            setShipToStatePostCodeForHKGUK(Country);
        }
    };

    //Change lb to kg Package Details
    $scope.changeKgToLb = function (pieceDetailOption) {
        if (pieceDetailOption === "kgToCms") {
            // for kg
            $translate('kGS').then(function (kGS) {
                $scope.Lb_Kgs = kGS;

            });
            $translate('KG').then(function (KG) {

                $scope.Lb_Kg = KG;
            });
            $translate('CMS').then(function (CMS) {
                $scope.Lb_Inch = CMS;
            });
        }
        if (pieceDetailOption === "lbToInchs") {
            // for LB
            $translate('LB').then(function (LB) {
                $scope.Lb_Kgs = LB;
            });
            $translate('LBs').then(function (LBs) {
                $scope.Lb_Kg = LBs;
            });
            $translate('INCHS').then(function (Inchs) {
                $scope.Lb_Inch = Inchs;
            });
        }
    };

    $scope.GetServices = function (quotationDetail, DirectBookingForm, IsValid) {
        if (IsValid) {
            if (quotationDetail !== undefined && DirectBookingForm !== undefined &&
               quotationDetail !== null &&
               quotationDetail.QuotationFromAddress !== undefined &&
               quotationDetail.QuotationFromAddress !== null &&
               quotationDetail.QuotationToAddress !== undefined &&
               quotationDetail.QuotationToAddress !== null &&
               quotationDetail.QuotationFromAddress.Country !== null &&
               quotationDetail.QuotationToAddress.Country !== null &&
               quotationDetail.QuotationPackages !== undefined &&
               quotationDetail.QuotationPackages !== null) {
                if (quotationDetail !== undefined) {
                    $rootScope.GetRateValue = true;
                }
                AppSpinner.showSpinnerTemplate('', $scope.Template);
                var weightTotal = $scope.PackagesTotal(quotationDetail.QuotationPackages, 'Weight');
                weightTotal = parseFloat(weightTotal);
                var flag = false;
                for (var i = 0 ; i < $scope.quotationDetail.QuotationPackages.length; i++) {
                    var packageForm = DirectBookingForm['tags' + i];
                    if (packageForm !== undefined && packageForm.$valid) {
                        flag = true;
                    }
                    else {
                        flag = false;
                        break;
                    }
                }

                if (weightTotal === 0 || !flag) {
                    AppSpinner.hideSpinner();
                    toaster.pop({
                        type: 'warning',
                        title: $scope.Frayte_Warning,
                        body: $scope.PleaseCorrectValidationErrors,
                        showCloseButton: true
                    });
                    return;
                }

                if ($scope.CustId === undefined || $scope.CustId === null || $scope.CustId === 0) {
                    AppSpinner.hideSpinner();
                    toaster.pop({
                        type: 'warning',
                        title: $scope.Frayte_Warning,
                        body: $scope.PleaseCorrectValidationErrors,
                        showCloseButton: true
                    });
                    return;
                }
                AppSpinner.hideSpinner();
                quotationDetail.CustomerId = $scope.CustId;
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'quotationTools/quotationToolService.tpl.html',
                    controller: 'QuotationToolServicesController',
                    windowClass: 'DirectBookingService',
                    size: 'lg',
                    backdrop: 'static',
                    resolve: {
                        directBookingObj: function () {
                            return quotationDetail;
                        },
                        CustomerId: function () {
                            return $scope.CustId;
                        },
                        IsRateShow: function () {
                            return $scope.IsRateShow;
                        },
                        LogisticService: function () {
                            return $scope.LogisticCompany;
                        },
                        CallingFrom: function () {
                            return 'Quotation';
                        }
                    }
                });

                modalInstance.result.then(function (RateCard) {

                    if (RateCard !== undefined && RateCard !== null) {
                        AppSpinner.hideSpinner();
                        if ($scope.quotationDetail.QuotationShipmentId > 0) {
                            $scope.quotationDetail.QuotationRateCard = RateCard;
                            for (i = 0; i < $scope.AddressTypes.length; i++) {
                                if ($scope.quotationDetail.AddressType === $scope.AddressTypes[i].Name) {
                                    $scope.quotationDetail.AddressType = $scope.AddressTypes[i];
                                }
                            }
                            $scope.quotationDetail.ValidDays = $scope.ValidDays;
                            $scope.editQuotation($scope.quotationDetail);
                        }
                        else {
                            $scope.quotationDetail.QuotationRateCard = RateCard;
                            for (i = 0; i < $scope.AddressTypes.length; i++) {
                                if ($scope.quotationDetail.AddressType === $scope.AddressTypes[i].Name) {
                                    $scope.quotationDetail.AddressType = $scope.AddressTypes[i];
                                }
                            }
                            $scope.quotationDetail.ValidDays = $scope.ValidDays;
                            $scope.SaveQuotation(DirectBookingForm);
                        }
                    }
                    $rootScope.GetRateValue = false;
                    //$scope.setScroll('pull-down');
                    window.scrollTo(0, 900);
                }, function () {
                    AppSpinner.hideSpinner();
                });
            }
            else {
                AppSpinner.hideSpinner();
                toaster.pop({
                    type: 'warning',
                    title: $scope.Frayte_Warning,
                    body: $scope.PleaseCorrectValidationErrors,
                    showCloseButton: true
                });
            }
            AppSpinner.hideSpinner();
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.PleaseCorrectValidationErrors,
                showCloseButton: true
            });
        }
    };

    $scope.editQuotation = function (quotationDetail) {
        if (quotationDetail !== undefined && quotationDetail !== null) {
            if (quotationDetail.AddressType !== undefined && quotationDetail.AddressType !== null) {
                quotationDetail.AddressType = quotationDetail.AddressType.Name;
            }
            else {
                quotationDetail.AddressType = '';
            }
            QuotationService.EditQuotation(quotationDetail).then(function (response) {
                if (response.status === 200) {
                    $scope.QuotationShipmentId = response.data.QuotationDetail.QuotationShipmentId;
                    $scope.quotationDetail = response.data.QuotationDetail;
                    for (i = 0; i < $scope.AddressTypes.length; i++) {
                        if ($scope.quotationDetail.AddressType === $scope.AddressTypes[i].Name) {
                            $scope.quotationDetail.AddressType = $scope.AddressTypes[i];
                        }
                    }
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'success',
                        title: $scope.Frayte_Success,
                        body: $scope.QuotationUpdate,
                        showCloseButton: true
                    });
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.Frayte_Error,
                        body: $scope.ToasterErrorMessage,
                        showCloseButton: true
                    });
                    AppSpinner.hideSpinnerTemplate();
                }
            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.Frayte_Error,
                    body: $scope.ToasterErrorMessage,
                    showCloseButton: true
                });
            });
        }
    };

    $scope.sendQuotationMail = function (CustomerDetail, emailRate) {
        if ($scope.quotationDetail !== null && $scope.CustomerDetail !== null && $scope.CustomerDetail !== undefined && emailRate === 'EmailRateCard') {
            if ($scope.CustomerDetail.CustomerId !== undefined && $scope.CustomerDetail.CustomerId !== null && $scope.RoleId === 1) {
                $scope.CustoId = $scope.CustomerDetail.CustomerId;
            }
            else if ($scope.RoleId === 6) {
                $scope.CustoId = $scope.customerId;
            }
            else if ($scope.RoleId === 3) {
                $scope.CustoId = $scope.CustId;
            }
            QuotationService.GetSalesRepresentiveEmail($scope.CustoId, $scope.RoleId).then(function (response) {
                $scope.SalesRepresentativedata = {
                    Name: '',
                    Email: '',
                    DeptName: ''
                };
                $scope.SalesRepresentative = response.data;
                if (response.data === null) {
                    $scope.SalesRepresentativedata.Name = $scope.UserName;
                    $scope.SalesRepresentativedata.Email = $scope.UserEmail;
                }
                else {
                    $scope.SalesRepresentativedata.Name = $scope.SalesRepresentative.SalesRepresentiveName;
                    $scope.SalesRepresentativedata.Email = $scope.SalesRepresentative.SalesEmail;
                    $scope.SalesRepresentativedata.DeptName = $scope.SalesRepresentative.DeptName;
                }
                if ($scope.SalesRepresentativedata !== null && $scope.SalesRepresentativedata !== undefined) {
                    var modalInstanceMail = $uibModal.open({
                        animation: true,
                        //templateUrl: 'quotationTools/quotationToolMail.tpl.html',
                        templateUrl: 'quotationTools/quotationToolEmailquote.tpl.html',
                        controller: 'QuotationToolMailController',
                        windowClass: '',
                        size: 'lg',
                        backdrop: 'static',
                        resolve: {
                            quotationDetail: function () {
                                return $scope.quotationDetail;
                            },
                            CustomerName: function () {
                                return CustomerDetail.CustomerName;
                            },
                            item: function () {
                                return $scope.item;
                            },
                            CustomerEmail: function () {
                                return CustomerDetail.EmailId;
                            },
                            CustomerDetail: function () {
                                return CustomerDetail;
                            },
                            CustId: function () {
                                return $scope.CustId;
                            },
                            CustName: function () {
                                return $scope.CustName;
                            },
                            CustEmail: function () {
                                return $scope.CustEmail;
                            },
                            Header: function () {
                                return 'Send Rate Card Email';
                            },
                            MailContentText: function () {
                                return '';
                            },
                            SalesRepresentativeDetail: function () {
                                return $scope.SalesRepresentativedata;
                            },
                            CompanyName: function () {
                                return $scope.CompanyName;
                            }
                        }
                    });

                    modalInstanceMail.result.then(function () {
                        //$scope.NewQuotation();
                        $uibModalStack.dismiss();
                        AppSpinner.hideSpinnerTemplate();
                    }, function () {
                        AppSpinner.hideSpinnerTemplate();
                    });
                }
            });
        }
        else if ($scope.quotationDetail !== null && CustomerDetail !== null && CustomerDetail !== undefined && emailRate === 'em' &&
                 $scope.quotationDetail.QuotationRateCard !== undefined && $scope.quotationDetail.QuotationRateCard !== null) {
            //if (CustomerDetail.CustomerId !== undefined && CustomerDetail.CustomerId !== null) {
            //    $scope.CustoId = CustomerDetail.CustomerId;
            //}
            //else {
            //    $scope.CustoId = CustomerDetail.EmployeeId;
            //}
            if (CustomerDetail.CustomerId !== undefined && CustomerDetail.CustomerId !== null && $scope.RoleId === 1) {
                $scope.CustoId = $scope.CustomerDetail.CustomerId;
            }
            else if ($scope.RoleId === 6) {
                $scope.CustoId = $scope.customerId;
            }
            else if ($scope.RoleId === 3) {
                $scope.CustoId = $scope.CustId;
            }

            //call to get sales representative
            QuotationService.GetSalesRepresentiveEmail($scope.CustoId, $scope.RoleId).then(function (response) {
                $scope.SalesRepresentativedata = {
                    Name: '',
                    Email: '',
                    DeptName: ''
                };
                $scope.SalesRepresentative = response.data;
                if (response.data === null) {
                    $scope.SalesRepresentativedata.Name = $scope.UserName;
                    $scope.SalesRepresentativedata.Email = $scope.UserEmail;
                }
                else {
                    $scope.SalesRepresentativedata.Name = $scope.SalesRepresentative.SalesRepresentiveName;
                    $scope.SalesRepresentativedata.Email = $scope.SalesRepresentative.SalesEmail;
                    $scope.SalesRepresentativedata.DeptName = $scope.SalesRepresentative.DeptName;
                }
                if ($scope.SalesRepresentativedata !== null && $scope.SalesRepresentativedata !== undefined) {
                    var modalInstanceEmailRateCard = $uibModal.open({
                        animation: true,
                        templateUrl: 'quotationTools/quotationToolEmailquote.tpl.html',
                        controller: 'QuotationToolMailController',
                        windowClass: '',
                        size: 'lg',
                        backdrop: 'static',
                        resolve: {
                            quotationDetail: function () {
                                return $scope.quotationDetail;
                            },
                            CustomerName: function () {
                                return CustomerDetail.CustomerName;
                            },
                            item: function () {
                                return $scope.item;
                            },
                            CustomerEmail: function () {
                                return CustomerDetail.EmailId;
                            },
                            CustomerDetail: function () {
                                return CustomerDetail;
                            },
                            CustId: function () {
                                return $scope.CustId;
                            },
                            CustName: function () {
                                return $scope.CustName;
                            },
                            CustEmail: function () {
                                return $scope.CustEmail;
                            },
                            Header: function () {
                                return 'Send Quotation Email';
                            },
                            MailContentText: function () {
                                return '';
                            },
                            SalesRepresentativeDetail: function () {
                                return $scope.SalesRepresentativedata;
                            },
                            CompanyName: function () {
                                return $scope.CompanyName;
                            }
                        }
                    });

                    modalInstanceEmailRateCard.result.then(function () {
                        //$scope.NewQuotation();
                        $uibModalStack.dismiss();
                        AppSpinner.hideSpinnerTemplate();
                    }, function () {
                        AppSpinner.hideSpinnerTemplate();
                    });
                }
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.PleaseCorrect_ValidationErrors,
                showCloseButton: true
            });
        }
    };

    $scope.sendRateMail = function (CustomerDetail, emailRate) {
        if ($scope.CustomerDetail.CustomerId !== undefined && $scope.CustomerDetail.CustomerId !== null && $scope.CustomerDetail.CustomerId > 0) {
            $scope.CustoId = $scope.CustomerDetail.CustomerId;
        }
        else {
            $scope.CustoId = $scope.CustId;
        }
        QuotationService.GetSalesRepresentiveEmail($scope.CustoId, $scope.RoleId).then(function (response) {
            $scope.SalesRepresentativedata = {
                Name: '',
                Email: '',
                DeptName: ''
            };
            $scope.SalesRepresentative = response.data;
            if (response.data === null) {
                $scope.SalesRepresentativedata.Name = $scope.UserName;
                $scope.SalesRepresentativedata.Email = $scope.UserEmail;
            }
            else {
                $scope.SalesRepresentativedata.Name = $scope.SalesRepresentative.SalesRepresentiveName;
                $scope.SalesRepresentativedata.Email = $scope.SalesRepresentative.SalesEmail;
                $scope.SalesRepresentativedata.DeptName = $scope.SalesRepresentative.DeptName;
            }
            if ($scope.SalesRepresentativedata !== null && $scope.SalesRepresentativedata !== undefined) {
                var modalInstanceMail = $uibModal.open({
                    animation: true,
                    templateUrl: 'quotationTools/quotationToolRate.tpl.html',
                    controller: 'QuotationToolRateController',
                    windowClass: '',
                    size: 'lg',
                    backdrop: 'static',
                    resolve: {
                        quotationDetail: function () {
                            return $scope.quotationDetail;
                        },
                        CustomerName: function () {
                            return CustomerDetail.CustomerName;
                        },
                        item: function () {
                            return $scope.item;
                        },
                        CustomerEmail: function () {
                            return CustomerDetail.EmailId;
                        },
                        CustomerDetail: function () {
                            return CustomerDetail;
                        },
                        CustId: function () {
                            return $scope.CustId;
                        },
                        CustName: function () {
                            return $scope.CustName;
                        },
                        CustEmail: function () {
                            return $scope.CustEmail;
                        },
                        Header: function () {
                            return 'Send Rate Card Email';
                        },
                        MailContentText: function () {
                            return '';
                        },
                        SalesRepresentativeDetail: function () {
                            return $scope.SalesRepresentativedata;
                        }
                    }
                });

                modalInstanceMail.result.then(function () {
                    //$scope.NewQuotation();
                    $uibModalStack.dismiss();
                    AppSpinner.hideSpinnerTemplate();
                }, function () {
                    AppSpinner.hideSpinnerTemplate();
                });
            }
        });
    };

    //Download Customer Rates
    $scope.DownloadCustomerRate = function (CompanyDetail) {

        if (CompanyDetail) {
            AppSpinner.showSpinnerTemplate($scope.DownloadingRateCardPDF, $scope.Template);
            var Rate = {
                UserId: $scope.CustId,
                LogisticServiceId: CompanyDetail.LogisticServiceId,
                LogisticCompany: CompanyDetail.LogisticCompany,
                LogisticType: CompanyDetail.LogisticType,
                RateType: CompanyDetail.RateType,
                FileType: 'PDF',
                CustomerName: $scope.CustName
            };

            $http({
                method: 'POST',
                url: config.SERVICE_URL + '/Customer/GenerateCustomerBaseRateCard',
                data: Rate
            }).success(function (data, status, headers) {

                if (status == 200 && data !== null && data.FileName !== '' && data.FilePath !== '' && data.FileName !== null && data.FilePath !== null) {
                    $scope.FileName = {
                        UserId: $scope.CustId,
                        FileName: data.FileName
                    };
                    var fileInfo = data;
                    //
                    if (data != null) {
                        $http({
                            method: 'POST',
                            url: config.SERVICE_URL + '/Customer/DownLoadRateCardReport',
                            data: $scope.FileName,
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
                                    // $scope.NewQuotation();
                                    toaster.pop({
                                        type: 'success',
                                        title: $scope.Frayte_Success,
                                        body: $scope.RateCardDownlodedSuccessfully_Your_Browser_Download_Folder,
                                        showCloseButton: true
                                    });
                                    AppSpinner.hideSpinnerTemplate();
                                } catch (ex) {
                                    toaster.pop({
                                        type: 'success',
                                        title: $scope.Frayte_Success,
                                        body: $scope.RateCardDownlodedSuccessfully_Your_Browser_Download_Folder,
                                        showCloseButton: true
                                    });
                                    //$scope.NewQuotation();
                                    $window.open(fileInfo.FilePath, "_blank");
                                    AppSpinner.hideSpinnerTemplate();
                                }

                            }
                        })
                       .error(function (data) {
                           AppSpinner.hideSpinnerTemplate();
                           toaster.pop({
                               type: 'error',
                               title: $scope.Frayte_Error,
                               body: $scope.CouldNot_Download_TheReport,
                               showCloseButton: true
                           });
                       });

                    }
                    else {
                        AppSpinner.hideSpinnerTemplate();
                    }
                }
                else if (data.FileStatus === false) {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'warning',
                        title: $scope.Frayte_Warning,
                        body: $scope.NoCustomerRateCardForThisCourier,
                        showCloseButton: true
                    });
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.Frayte_Error,
                        body: $scope.ErrorDownloadingPDF,
                        showCloseButton: true
                    });
                }
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.PleaseCorrect_ValidationErrors,
                showCloseButton: true
            });
        }

    };

    $scope.ChangeWeight = function (weight, CartoonValue, pieceDetailOption, quotationDetail) {
        if (pieceDetailOption === "kgToCms") {
            if (weight) {
                $scope.isdisabled = true;
            }
            else {
                if (quotationDetail.QuotationPackages.length > 1) {
                    var isWegt = false;
                    angular.forEach(quotationDetail.QuotationPackages, function (value, key) {

                        if (value.Weight) {
                            $scope.isdisabled = true;
                            isWegt = true;
                        }
                        else {
                            if (!isWegt) {
                                $scope.isdisabled = false;
                            }
                        }
                    });
                }
                else {
                    $scope.isdisabled = false;
                }
            }
            // for kg
            if (parseInt(weight, 10) > 70) {
                //$scope.Package.Weight = '';
                var finalWeightkg = '';
                var weightkg = weight.toString().split('');
                var weightkgnew = weightkg.splice(weightkg.length - 1);
                for (i = 0; i < weightkg.length; i++) {
                    finalWeightkg = finalWeightkg + weightkg[i];
                }
                weight = finalWeightkg;
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.MaxWeight70kGS,
                    showCloseButton: true
                });

                $scope.Weight = weight;
                return weight;
            }
        }

        if (pieceDetailOption === "lbToInchs") {

            if (weight) {
                $scope.isdisabled = true;

            }
            else {
                if (quotationDetail.QuotationPackages.length > 1) {
                    var isWegtlbs = false;
                    angular.forEach(quotationDetail.QuotationPackages, function (value, key) {

                        if (value.Weight) {
                            $scope.isdisabled = true;
                            isWegtlbs = true;
                        }
                        else {
                            if (!isWegtlbs) {
                                $scope.isdisabled = false;
                            }
                        }
                    });

                }
                else {
                    $scope.isdisabled = false;
                }
            }
            if (parseInt(weight, 10) > 154) {

                //$scope.Package.Weight = '';
                var finalWeightlb = '';
                var weightlbs = weight.toString().split('');
                var weightlbsnew = weightlbs.splice(weightlbs.length - 1);
                for (i = 0; i < weightlbs.length; i++) {
                    finalWeightlb = finalWeightlb + weightlbs[i];
                }
                weight = finalWeightlb;
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.MaxWeight154KGS,
                    showCloseButton: true
                });
            }
            $scope.Weight = weight;
        }
        if ($scope.quotationDetail.ParcelType === null) {

        }
        else if (weight > 2 && $scope.quotationDetail.ParcelType.ParcelType === 'Letter') {

            $scope.getmaxval = true;
        }
        else {
            $scope.getmaxval = false;

        }

        return weight;
    };

    $scope.setPackageParcelType = function () {
        if ($scope.quotationDetail.ParcelType === null) {
            return;
        }
        else if ($scope.Weight > 2 && $scope.quotationDetail.ParcelType.ParcelType === 'Letter') {

            $scope.getmaxval = true;
        }
        else {
            $scope.getmaxval = false;
            return;
        }
    };

    $scope.editGetServices = function (DirectBookingForm, IsValid) {
        $scope.GetServices($scope.quotationDetail, DirectBookingForm, IsValid);
    };

    $scope.cancelServices = function () {
        if ($scope.quotationDetail !== undefined && $scope.quotationDetail !== null && $scope.quotationDetail.QuotationShipmentId > 0) {
            QuotationService.RemoveQuotation($scope.quotationDetail.QuotationShipmentId).then(function (response) {
                if (response.data.Status === true) {
                    $scope.quotationDetail.QuotationShipmentId = 0;
                    $scope.quotationDetail.QuotationRateCard = {
                        ZoneRateCardId: 0,
                        LogisticServiceId: 0,
                        WeightType: '',
                        CourierId: 0,
                        CourierName: '',
                        CourierAccountNo: '',
                        CourierDescription: '',
                        IntegrationAccountId: 0
                    };
                    //toaster.pop({
                    //    type: 'success',
                    //    title: $scope.Frayte_Success,
                    //    body: ,
                    //    showCloseButton: true
                    //});

                }
                else {

                }
            });
        }
        else {

        }
    };

    $scope.checkDeclaredValue = function (DirectBookingForm, packageForm, value) {
        if (value !== undefined && value !== null && value !== '') {
            var total = parseFloat(value);
            if (total < 0.01) {
                if (packageForm !== undefined) {
                    packageForm.currencyValue.$invalid = true;
                }

                DirectBookingForm.$valid = false;
            }
        }

    };

    $scope.SetValueNotZero = function (Value) {
        if (Value !== undefined && Value !== null && Value === "0") {
            return;
        }
        return Value;
    };

    $scope.AddressTypeChange = function (AddressType) {
        $scope.AddressType = AddressType;
    };

    $scope.getCustomerCurrencyDetail = function (CustomerDetail) {
        if (CustomerDetail !== undefined && CustomerDetail !== null) {
            $scope.CustName = CustomerDetail.CustomerName;
            $scope.CustId = CustomerDetail.CustomerId;
            $scope.CustEmail = CustomerDetail.EmailId;
            $scope.CustomerDetail = CustomerDetail;
            $scope.CompanyName = CustomerDetail.CompanyName;
            $scope.IsEnable = false;
            //$scope.CustomerCurrency = CustomerDetail.CustomerCurrency;
            $scope.ValidDays = CustomerDetail.ValidDays;

            DirectBookingService.GetCustomerLogisticService($scope.CustomerDetail.CustomerId).then(function (response) {
                if (response.data !== null) {
                    $scope.LogisticCompany = '';
                    for (var h = 0; h < response.data.length; h++) {
                        if (response.data[h].LogisticCompany === 'Yodel') {
                            $scope.LogisticCompany = response.data[h].LogisticCompany;
                        }
                    }
                }
            });

            QuotationService.GetCustomerLogisticService(CustomerDetail.CustomerId).then(function (response) {
                $scope.CustomerServices = response.data;
                $scope.CustomerFinalServices = [];
                if ($scope.CustomerServices !== null && $scope.CustomerServices !== undefined) {
                    for (var i = 0; i < $scope.CustomerServices.length; i++) {

                        if ($scope.CustomerServices[i].IssueDate !== undefined && $scope.CustomerServices[i].IssueDate !== null) {
                            var issdate = new Date($scope.CustomerServices[i].IssueDate);
                            for (var j = 0; j < $scope.MonthName.length; j++) {
                                if ($scope.MonthName[j].Value === (issdate.getMonth() + 1)) {
                                    $scope.NewIssueDate = ((issdate.getDate().toString().length > 1 ? issdate.getDate() : '0' + issdate.getDate().toString()) + '-' + $scope.MonthName[j].Display + '-' + issdate.getFullYear());
                                }
                            }
                        }

                        if ($scope.CustomerServices[i].ExpiryDate !== undefined && $scope.CustomerServices[i].ExpiryDate !== null) {
                            var exdate = new Date($scope.CustomerServices[i].ExpiryDate);
                            for (var k = 0; k < $scope.MonthName.length; k++) {
                                if ($scope.MonthName[k].Value === (exdate.getMonth() + 1)) {
                                    $scope.NewExpiryDate = ((exdate.getDate().toString().length > 1 ? exdate.getDate() : '0' + exdate.getDate().toString()) + '-' + $scope.MonthName[k].Display + '-' + exdate.getFullYear());
                                }
                            }
                        }

                        var serviceJson = {
                            LogisticServiceId: 0,
                            LogisticCompany: "",
                            LogisticCompanyDisplay: "",
                            LogisticType: "",
                            LogisticTypeDisplay: "",
                            RateType: "",
                            RateTypeDisplay: "",
                            IssueDate: "",
                            ExpiryDate: ""
                        };
                        if ($scope.CustomerServices[i].LogisticCompany === "UKMail") {
                            serviceJson.LogisticServiceId = $scope.CustomerServices[i].LogisticServiceId;
                            serviceJson.LogisticCompany = $scope.CustomerServices[i].LogisticCompany;
                            serviceJson.LogisticCompanyDisplay = $scope.CustomerServices[i].LogisticCompanyDisplay;
                            serviceJson.LogisticType = $scope.CustomerServices[i].LogisticType;
                            serviceJson.LogisticTypeDisplay = $scope.CustomerServices[i].LogisticTypeDisplay;
                            serviceJson.RateType = '';
                            serviceJson.RateTypeDisplay = "Singles / Multiples Services";
                            serviceJson.IssueDate = $scope.NewIssueDate;
                            serviceJson.ExpiryDate = $scope.NewExpiryDate;
                            $scope.CustomerFinalServices.push(serviceJson);
                        }
                        else if ($scope.CustomerServices[i].LogisticCompany === "Yodel") {
                            serviceJson.LogisticServiceId = $scope.CustomerServices[i].LogisticServiceId;
                            serviceJson.LogisticCompany = $scope.CustomerServices[i].LogisticCompany;
                            serviceJson.LogisticCompanyDisplay = $scope.CustomerServices[i].LogisticCompanyDisplay;
                            serviceJson.LogisticType = $scope.CustomerServices[i].LogisticType;
                            serviceJson.LogisticTypeDisplay = $scope.CustomerServices[i].LogisticTypeDisplay;
                            serviceJson.RateType = '';
                            serviceJson.RateTypeDisplay = "B2B / B2C Services";
                            serviceJson.IssueDate = $scope.NewIssueDate;
                            serviceJson.ExpiryDate = $scope.NewExpiryDate;
                            $scope.CustomerFinalServices.push(serviceJson);
                        }
                        else if ($scope.CustomerServices[i].LogisticCompany === "Hermes") {
                            serviceJson.LogisticServiceId = $scope.CustomerServices[i].LogisticServiceId;
                            serviceJson.LogisticCompany = $scope.CustomerServices[i].LogisticCompany;
                            serviceJson.LogisticCompanyDisplay = $scope.CustomerServices[i].LogisticCompanyDisplay;
                            serviceJson.LogisticType = $scope.CustomerServices[i].LogisticType;
                            serviceJson.LogisticTypeDisplay = $scope.CustomerServices[i].LogisticTypeDisplay;
                            serviceJson.RateType = '';
                            serviceJson.RateTypeDisplay = "POD / NONPOD Services";
                            serviceJson.IssueDate = $scope.NewIssueDate;
                            serviceJson.ExpiryDate = $scope.NewExpiryDate;
                            $scope.CustomerFinalServices.push(serviceJson);
                        }
                        else if ($scope.CustomerServices[i].LogisticCompany === 'DHL' ||
                            $scope.CustomerServices[i].LogisticCompany === 'TNT' ||
                            $scope.CustomerServices[i].LogisticCompany === 'UPS') {
                            serviceJson.LogisticServiceId = $scope.CustomerServices[i].LogisticServiceId;
                            serviceJson.LogisticCompany = $scope.CustomerServices[i].LogisticCompany;
                            serviceJson.LogisticCompanyDisplay = $scope.CustomerServices[i].LogisticCompanyDisplay;
                            serviceJson.LogisticType = $scope.CustomerServices[i].LogisticType;
                            serviceJson.LogisticTypeDisplay = $scope.CustomerServices[i].LogisticTypeDisplay;
                            serviceJson.RateType = $scope.CustomerServices[i].RateType;
                            serviceJson.RateTypeDisplay = $scope.CustomerServices[i].RateTypeDisplay;
                            serviceJson.IssueDate = $scope.NewIssueDate;
                            serviceJson.ExpiryDate = $scope.NewExpiryDate;
                            $scope.CustomerFinalServices.push(serviceJson);
                        }                        
                    }
                    $scope.CustServLength = $scope.CustomerFinalServices.length;
                }

            });
        }
        else {
            $scope.CustServLength = 0;
            $scope.CustId = $scope.customerId;
        }
    };

    //Download Supplementry charges for customer rate card
    $scope.DownloadSupplementryCharges = function (CustomerDetail) {
        if (CustomerDetail) {
            AppSpinner.showSpinnerTemplate($scope.DownloadingSupplementaryPDF, $scope.Template);
            var Rate = {
                UserId: $scope.CustId,
                LogisticServiceId: CustomerDetail.LogisticServiceId,
                LogisticCompany: CustomerDetail.LogisticCompany,
                LogisticType: CustomerDetail.LogisticType,
                RateType: CustomerDetail.RateType,
                FileType: 'PDF',
                CustomerName: $scope.CustName
            };

            QuotationService.GetTNTSupplemetoryInfo(CustomerDetail.LogisticServiceId).then(function (response) {
                if (response.data !== undefined && response.data !== null) {
                    if ((response.data.LogisticCompany === 'TNT' && response.data.OperationZoneId === 2)) {
                        AppSpinner.hideSpinnerTemplate();
                        //|| (response.data.LogisticCompany === 'UPS' && response.data.OperationZoneId === 1))
                        //if (response.data.LogisticCompany === 'UPS') {
                        //    $scope.Message1 = $scope.ForAdditionalDomestic + ' ' + $scope.ReadInfo;
                        //    $scope.Message2 = $scope.UPSInfo;
                        //    $scope.LogisticCompany = response.data.LogisticCompany;
                        //}
                        //else
                        if (response.data.LogisticCompany === 'TNT') {
                            $scope.Message1 = $scope.ForAdditionalDomestic + ' ' + $scope.VisitInfo;
                            $scope.Message2 = $scope.TNTInfo;
                            $scope.LogisticCompany = response.data.LogisticCompany;
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
                                    return $scope.LogisticCompany;
                                }
                            }
                        });
                    }
                    else {
                        $http({
                            method: 'POST',
                            url: config.SERVICE_URL + '/Customer/GenerateSupplementoryCharges',
                            data: Rate
                        }).success(function (data, status, headers) {

                            if (status == 200 && data !== null && data.FileName !== '' && data.FilePath !== '' && data.FileName !== null && data.FilePath !== null) {
                                $scope.FileName = {
                                    UserId: $scope.CustId,
                                    FileName: data.FileName
                                };
                                var fileInfo = data;
                                //
                                if (data != null) {
                                    $http({
                                        method: 'POST',
                                        url: config.SERVICE_URL + '/Customer/DownloadSupplemantoryCharge',
                                        data: $scope.FileName,
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
                                                // $scope.NewQuotation();
                                                toaster.pop({
                                                    type: 'success',
                                                    title: $scope.Frayte_Success,
                                                    body: $scope.Supplementary_DownloadedSuccessfully,
                                                    showCloseButton: true
                                                });
                                                AppSpinner.hideSpinnerTemplate();
                                            } catch (ex) {
                                                //$scope.NewQuotation();
                                                $window.open(fileInfo.FilePath, "_blank");
                                                AppSpinner.hideSpinnerTemplate();
                                            }
                                        }
                                    })
                                   .error(function (data) {
                                       AppSpinner.hideSpinnerTemplate();
                                       toaster.pop({
                                           type: 'error',
                                           title: $scope.Frayte_Error,
                                           body: $scope.CouldNot_Download_TheReport,
                                           showCloseButton: true
                                       });
                                   });
                                }
                                else {
                                    AppSpinner.hideSpinnerTemplate();
                                }
                            }
                            else if (data.FileStatus === false) {
                                AppSpinner.hideSpinnerTemplate();
                                toaster.pop({
                                    type: 'warning',
                                    title: $scope.Frayte_Warning,
                                    body: $scope.Nosupplymentry_charges_Courier,
                                    showCloseButton: true
                                });
                            }
                            else {
                                toaster.pop({
                                    type: 'error',
                                    title: $scope.Frayte_Error,
                                    body: $scope.ErrorDownloadingPDF,
                                    showCloseButton: true
                                });
                            }
                        });
                    }
                }
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.PleaseCorrect_ValidationErrors,
                showCloseButton: true
            });
        }
    };

    //Download Summary Call
    $scope.DownloadSummary = function (CustomerDetail) {
        if (CustomerDetail) {
            AppSpinner.showSpinnerTemplate($scope.DownloadingRateCardSummary, $scope.Template);
            var Rate = {
                UserId: $scope.CustId,
                LogisticServiceId: CustomerDetail.LogisticServiceId,
                LogisticCompany: CustomerDetail.LogisticCompany,
                LogisticType: CustomerDetail.LogisticType,
                RateType: CustomerDetail.RateType,
                FileType: '',
                CustomerName: $scope.CustName,
                SendingOption: 'SUMMERY'
            };

            $http({
                method: 'POST',
                url: config.SERVICE_URL + '/Customer/GenerateCustomerBaseRateCard',
                data: Rate
            }).success(function (data, status, headers) {

                if (status == 200 && data !== null && data.FileName !== '' && data.FilePath !== '' && data.FileName !== null && data.FilePath !== null) {
                    $scope.FileName = {
                        UserId: $scope.CustId,
                        FileName: data.FileName
                    };
                    var fileInfo = data;
                    //
                    if (data != null) {
                        $http({
                            method: 'POST',
                            url: config.SERVICE_URL + '/Customer/DownLoadRateCardReport',
                            data: $scope.FileName,
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
                                    // $scope.NewQuotation();
                                    toaster.pop({
                                        type: 'success',
                                        title: $scope.Frayte_Success,
                                        body: $scope.Report_GeneratedDownloadedSuccessfully,
                                        showCloseButton: true
                                    });
                                    AppSpinner.hideSpinnerTemplate();
                                } catch (ex) {
                                    //$scope.NewQuotation();
                                    $window.open(fileInfo.FilePath, "_blank");
                                    AppSpinner.hideSpinnerTemplate();
                                }
                            }
                        })
                       .error(function (data) {
                           AppSpinner.hideSpinnerTemplate();
                           toaster.pop({
                               type: 'error',
                               title: $scope.Frayte_Error,
                               body: $scope.CouldNot_Download_TheReport,
                               showCloseButton: true
                           });
                       });
                    }
                    else {
                        AppSpinner.hideSpinnerTemplate();
                    }
                }
                else if (data.FileStatus === false) {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'warning',
                        title: $scope.Frayte_Warning,
                        body: $scope.NoCustomerRateCardForThisCourier,
                        showCloseButton: true
                    });
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.Frayte_Error,
                        body: $scope.ErrorDownloadingExcel,
                        showCloseButton: true
                    });
                }
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.PleaseCorrect_ValidationErrors,
                showCloseButton: true
            });
        }
    };

    //Newquote call
    $scope.newquote = function () {
        if ($scope.quotationDetail !== null && $scope.quotationDetail !== undefined) {
            var modalInstanceMail = $uibModal.open({
                animation: true,
                templateUrl: 'quotationTools/newQuotationConfirmation.tpl.html',
                controller: 'QuotationToolController',
                windowClass: '',
                size: 'md',
                backdrop: 'static'
            });

            modalInstanceMail.result.then(function () {
                $scope.NewQuotation();
                $uibModalStack.dismiss();
            }, function () {
                $uibModalStack.dismiss();
            });
        }
    };

    var accBreak = function (fiterCountryLength, AccountNo) {
        var AccNo = AccountNo.split('');
        var AccNonew = [];
        AccNonew2 = "";
        if (fiterCountryLength <= 3) {
            for (j = 0; j < fiterCountryLength; j++) {
                if (j === 0) {
                    AccNonew.push('a');
                }
                AccNonew.push(AccNo[j]);
            }
            AccNonew.splice(0, 1);
            for (jj = 0; jj < AccNonew.length; jj++) {
                AccNonew2 = AccNonew2 + AccNonew[jj].toString();
            }
        }
        else if (fiterCountryLength >= 4 && fiterCountryLength <= 8) {

            for (j = 0; j < fiterCountryLength; j++) {

                if (j === 0) {
                    AccNonew.push('a');
                }

                AccNonew.push(AccNo[j]);
            }

            AccNonew.splice(0, 1);
            for (jj = 0; jj < AccNonew.length; jj++) {
                AccNonew2 = AccNonew2 + AccNonew[jj].toString();
            }
        }
        else if (fiterCountryLength > 8) {

            for (j = 0; j < fiterCountryLength; j++) {

                if (j === 0) {
                    AccNonew.push('a');
                }
                AccNonew.push(AccNo[j]);
            }
            AccNonew.splice(0, 1);
            for (jj = 0; jj < AccNonew.length; jj++) {
                AccNonew2 = AccNonew2 + AccNonew[jj].toString();
            }
        }
        return AccNonew2;
    };

    $scope.CheckAccountNo = function (FilterCountry) {
        if (FilterCountry !== '' && FilterCountry !== null) {
            var filterCountry1 = [];
            filterCountry1 = FilterCountry.split('');
            var filterCountrylength = filterCountry1.length;
            for (ia = 0; ia < $scope.directBookingCustomers.length; ia++) {
                var AccNoBreak = accBreak(filterCountrylength, $scope.directBookingCustomers[ia].AccountNumber);
                $scope.CustomerDetail = null;
                if (FilterCountry == AccNoBreak) {
                    $scope.CustomerDetail = $scope.directBookingCustomers[ia];
                    $scope.getCustomerCurrencyDetail($scope.CustomerDetail);
                    $scope.IsEnable = false;
                    break;
                }
            }
        }
        else {
            $scope.CustomerDetail = null;
            $scope.IsEnable = true;
        }
    };

    $scope.$watch('FilterCountry', function (newValue, oldValue) {
        var newValArr = [];
        if (newValue !== undefined && newValue !== "") {
            newValArr = newValue.split('');
            newValArr = newValArr.length;
            if (newValue > oldValue) {
                if ($scope.directBookingCustomers !== undefined && $scope.directBookingCustomers !== null) {
                    for (ia = 0; ia < $scope.directBookingCustomers.length; ia++) {
                        if (newValArr === 3 || newValArr === 7) {
                            newValue = newValue + '-';
                            $scope.FilterCountry = newValue;
                            break;
                        }
                    }
                }
            }
            else if (newValue < oldValue) {
                var newvalarr = oldValue.split('');
                var newvalarrlen = newvalarr.length - 1;
                var newvalarr1 = newvalarr.splice(newvalarrlen);
                newvalstring = "";
                for (ib = 0; ib < newvalarr.length; ib++) {
                    newvalstring = newvalstring + newvalarr[ib].toString();
                }
                $scope.FilterCountry = newvalstring;
            }
        }
        else if (newValue === undefined || newValue === "") {
            $scope.FilterCountry = newValue;
        }
    });

    $scope.openCalender = function ($event) {
        $scope.status.opened = true;
    };

    function init() {

        $scope.AddressTypes = [
             {
                 Name: 'B2B',
                 Display: 'B2B'
             },
             {
                 Name: 'B2CNeighborhood',
                 Display: 'B2C Neighborhood'
             },
             {
                 Name: 'B2CHome',
                 Display: 'B2C Home'
             }
        ];

        $scope.MonthName = [
            {
                Value: 1,
                Display: 'Jan'
            },
            {
                Value: 2,
                Display: 'Feb'
            },
            {
                Value: 3,
                Display: 'Mar'
            },
            {
                Value: 4,
                Display: 'Apr'
            },
            {
                Value: 5,
                Display: 'May'
            },
            {
                Value: 6,
                Display: 'Jun'
            },
            {
                Value: 7,
                Display: 'Jul'
            },
            {
                Value: 8,
                Display: 'Aug'
            },
            {
                Value: 9,
                Display: 'Sep'
            },
            {
                Value: 10,
                Display: 'Oct'
            },
            {
                Value: 11,
                Display: 'Nov'
            },
            {
                Value: 12,
                Display: 'Dec'
            }
        ];

        $scope.NewQuotation();

        $rootScope.ChangeManifest = false;
        $rootScope.GetServiceValue = null;
        $scope.IsEnable = false;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';

        $scope.submitted = true;
        $scope.sendMail = false;
        var userInfo = SessionService.getUser();
        $scope.UserInfo = userInfo;
        $scope.tabs = userInfo.tabs;
        $scope.RoleId = userInfo.RoleId;
        $scope.CreatedBy = userInfo.EmployeeId;
        $scope.UserName = userInfo.UserName;
        $scope.UserEmail = userInfo.EmployeeMail;
        $scope.OperationZoneId = userInfo.OperationZoneId;
        $scope.CustoDetail = userInfo;
        if ($scope.RoleId !== 1 && $scope.RoleId !== 6) {
            $scope.CustomerDetail = userInfo;
            $scope.CustName = userInfo.EmployeeName;
            $scope.CustEmail = userInfo.EmployeeMail;
            $scope.CustId = userInfo.EmployeeId;
            $scope.ValidDays = userInfo.ValidDays;
        }

        if ($scope.RoleId === 1 || $scope.RoleId === 6) {
            $scope.customerId = userInfo.EmployeeId;
            $scope.IsEnable = true;
        }

        var SiteAddress = config.SITE_COUNTRY;
        if (SiteAddress !== undefined && SiteAddress !== null && SiteAddress !== '') {
            if (SiteAddress === 'CO.UK') {
                $scope.CustomerCurrency = 'GBP';
            }
            else if (SiteAddress === 'COM') {
                $scope.CustomerCurrency = 'HKD';
            }
        }

        //Step 1: Get initials and default address of logged in customer.
        DirectBookingService.GetInitials(($scope.customerId === undefined || $scope.customerId === null) ? $scope.CustId : $scope.customerId).then(function (response) {

            // Set Country type according to given order
            $scope.CountriesRepo = TopCountryService.TopCountryList(response.data.Countries);
            $scope.PiecesExcelDownloadPathXlsx = response.data.PiecesExcelDownloadPathXlsx;
            $scope.PiecesExcelDownloadPathXls = response.data.PiecesExcelDownloadPathXls;
            $scope.PiecesExcelDownloadPathCsv = response.data.PiecesExcelDownloadPathCsv;
            // Set Currency type according to given order
            $scope.CurrencyTypes = TopCurrencyService.TopCurrencyList(response.data.CurrencyTypes);
            $scope.ParcelTypes = response.data.ParcelTypes;
            $scope.quotationDetail.ParcelType = response.data.ParcelTypes[0];
            $scope.NewQuotation();
            $scope.CountryPhoneCodes = response.data.CountryPhoneCodes;
            $scope.CustomerDetail = response.data.FrayteCustomerDetail;
            if ($scope.RoleId === 3) {

                if ($scope.CustomerDetail !== null && $scope.CustomerDetail.IsRateShow) {
                    $scope.IsRateShow = true;
                }
                else {
                    $scope.IsRateShow = false;
                }
            }
            else {
                $scope.IsRateShow = true;
            }

            $scope.changeKgToLb($scope.quotationDetail.PakageCalculatonType);
        },
        function (response) {
            if (response.status !== 401) {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteServiceErrorValidation,
                    body: $scope.InitialDataValidation,
                    showCloseButton: true
                });
            }
        });

        var currentYear1 = new Date();
        var currentYear = currentYear1.getFullYear();
        if ($scope.RoleId !== 1) {
            $scope.CustName = userInfo.EmployeeName;
            QuotationService.GetCustomerLogisticService(($scope.customerId === undefined || $scope.customerId === null) ? $scope.CustId : $scope.customerId).then(function (response) {
                $scope.CustomerRateCardServices = response.data;
                $scope.CustomerFinalServices = [];
                if ($scope.CustomerRateCardServices !== null && $scope.CustomerRateCardServices !== undefined) {
                    for (var i = 0; i < $scope.CustomerRateCardServices.length; i++) {

                        if ($scope.CustomerRateCardServices[i].IssueDate !== undefined && $scope.CustomerRateCardServices[i].IssueDate !== null) {
                            var issdate = new Date($scope.CustomerRateCardServices[i].IssueDate);
                            for (var j = 0; j < $scope.MonthName.length; j++) {
                                if ($scope.MonthName[j].Value === (issdate.getMonth() + 1)) {
                                    $scope.NewIssueDate = ((issdate.getDate().toString().length > 1 ? issdate.getDate() : '0' + issdate.getDate().toString()) + '-' + $scope.MonthName[j].Display + '-' + issdate.getFullYear());
                                }
                            }
                        }

                        if ($scope.CustomerRateCardServices[i].ExpiryDate !== undefined && $scope.CustomerRateCardServices[i].ExpiryDate !== null) {
                            var exdate = new Date($scope.CustomerRateCardServices[i].ExpiryDate);
                            for (var k = 0; k < $scope.MonthName.length; k++) {
                                if ($scope.MonthName[k].Value === (exdate.getMonth() + 1)) {
                                    $scope.NewExpiryDate = ((exdate.getDate().toString().length > 1 ? exdate.getDate() : '0' + exdate.getDate().toString()) + '-' + $scope.MonthName[k].Display + '-' + exdate.getFullYear());
                                }
                            }
                        }

                        var serviceJson = {
                            LogisticServiceId: 0,
                            LogisticCompany: "",
                            LogisticCompanyDisplay: "",
                            LogisticType: "",
                            LogisticTypeDisplay: "",
                            RateType: "",
                            RateTypeDisplay: "",
                            IssueDate: "",
                            ExpiryDate: ""
                        };

                        if ($scope.CustomerRateCardServices[i].LogisticCompany === "UKMail") {
                            serviceJson.LogisticServiceId = $scope.CustomerRateCardServices[i].LogisticServiceId;
                            serviceJson.LogisticCompany = $scope.CustomerRateCardServices[i].LogisticCompany;
                            serviceJson.LogisticCompanyDisplay = $scope.CustomerRateCardServices[i].LogisticCompanyDisplay;
                            serviceJson.LogisticType = $scope.CustomerRateCardServices[i].LogisticType;
                            serviceJson.LogisticTypeDisplay = $scope.CustomerRateCardServices[i].LogisticTypeDisplay;
                            serviceJson.RateType = '';
                            serviceJson.RateTypeDisplay = "Singles / Multiples Services";
                            serviceJson.IssueDate = $scope.NewIssueDate;
                            serviceJson.ExpiryDate = $scope.NewExpiryDate;
                        }
                        else if ($scope.CustomerRateCardServices[i].LogisticCompany === "Yodel") {
                            serviceJson.LogisticServiceId = $scope.CustomerRateCardServices[i].LogisticServiceId;
                            serviceJson.LogisticCompany = $scope.CustomerRateCardServices[i].LogisticCompany;
                            serviceJson.LogisticCompanyDisplay = $scope.CustomerRateCardServices[i].LogisticCompanyDisplay;
                            serviceJson.LogisticType = $scope.CustomerRateCardServices[i].LogisticType;
                            serviceJson.LogisticTypeDisplay = $scope.CustomerRateCardServices[i].LogisticTypeDisplay;
                            serviceJson.RateType = '';
                            serviceJson.RateTypeDisplay = "B2B / B2C Services";
                            serviceJson.IssueDate = $scope.NewIssueDate;
                            serviceJson.ExpiryDate = $scope.NewExpiryDate;
                        }
                        else if ($scope.CustomerRateCardServices[i].LogisticCompany === "Hermes") {
                            serviceJson.LogisticServiceId = $scope.CustomerRateCardServices[i].LogisticServiceId;
                            serviceJson.LogisticCompany = $scope.CustomerRateCardServices[i].LogisticCompany;
                            serviceJson.LogisticCompanyDisplay = $scope.CustomerRateCardServices[i].LogisticCompanyDisplay;
                            serviceJson.LogisticType = $scope.CustomerRateCardServices[i].LogisticType;
                            serviceJson.LogisticTypeDisplay = $scope.CustomerRateCardServices[i].LogisticTypeDisplay;
                            serviceJson.RateType = '';
                            serviceJson.RateTypeDisplay = "POD / NONPOD Services";
                            serviceJson.IssueDate = $scope.NewIssueDate;
                            serviceJson.ExpiryDate = $scope.NewExpiryDate;
                        }
                        else {
                            serviceJson.LogisticServiceId = $scope.CustomerRateCardServices[i].LogisticServiceId;
                            serviceJson.LogisticCompany = $scope.CustomerRateCardServices[i].LogisticCompany;
                            serviceJson.LogisticCompanyDisplay = $scope.CustomerRateCardServices[i].LogisticCompanyDisplay;
                            serviceJson.LogisticType = $scope.CustomerRateCardServices[i].LogisticType;
                            serviceJson.LogisticTypeDisplay = $scope.CustomerRateCardServices[i].LogisticTypeDisplay;
                            serviceJson.RateType = $scope.CustomerRateCardServices[i].RateType;
                            serviceJson.RateTypeDisplay = $scope.CustomerRateCardServices[i].RateTypeDisplay;
                            serviceJson.IssueDate = $scope.NewIssueDate;
                            serviceJson.ExpiryDate = $scope.NewExpiryDate;
                        }

                        $scope.CustomerFinalServices.push(serviceJson);
                    }

                    $scope.CustServLength = $scope.CustomerFinalServices.length;
                }

            }, function (response) {
                if (response.status !== 401) {
                    toaster.pop({
                        type: "error",
                        title: $scope.FrayteError,
                        body: $scope.ReceiveDetail_Validation,
                        showCloseButton: true
                    });
                }
            });

            DirectBookingService.GetCustomerLogisticService(($scope.customerId === undefined || $scope.customerId === null) ? $scope.CustId : $scope.customerId).then(function (response) {
                if (response.data !== null) {
                    $scope.LogisticCompany = '';
                    for (var h = 0; h < response.data.length; h++) {
                        if (response.data[h].LogisticCompany === 'Yodel') {
                            $scope.LogisticCompany = response.data[h].LogisticCompany;
                        }
                    }
                }
            }, function () {

            });
        }
        $scope.paymentAccount = true;
        DirectBookingService.GetDirectBookingCustomers(($scope.customerId === undefined || $scope.customerId === null) ? $scope.CustId : $scope.customerId, "DirectBooking").then(function (response) {
            $scope.directBookingCustomers = response.data;
            var dbCustomers = [];

            for (i = 0; i < $scope.directBookingCustomers.length; i++) {

                if ($scope.RoleId === 1) {
                    dbCustomers.push($scope.directBookingCustomers[i]);
                }
                else {
                    if ($scope.directBookingCustomers[i].OperationZoneId === userInfo.OperationZoneId) {
                        dbCustomers.push($scope.directBookingCustomers[i]);
                    }
                }

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
            $scope.directBookingCustomers = dbCustomers;
        }, function (response) {
            if (response.status !== 401) {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteServiceErrorValidation,
                    body: $scope.ReceiveDetail_Validation,
                    showCloseButton: true
                });
            }
        });

        $scope.HideContent = false;
        window.scrollTo(0, 0);
        //$scope.setScroll('top');
        //$anchorScroll.yOffset = 200;
        setModalOptions();
    }

    init();
});