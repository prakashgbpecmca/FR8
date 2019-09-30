angular.module('ngApp.quotationTools').controller('QuotationToolController', function ($uibModalStack, QuotationService, $http, $window, Upload, $rootScope, AppSpinner, $sce, TopCountryService, $location, $anchorScroll, TopCurrencyService, $scope, config, $filter, $state, ModalService, $translate, $uibModal, $timeout, toaster, $stateParams, DirectBookingService, SessionService, ZoneBaseRateCardHistoryService) {
    $scope.status = {
        isopen: false
    };
    var setModalOptions = function () {
        $translate(['FrayteWarning', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation', 'ErrorSavingRecord', 'FrayteError', 'FrayteSuccess',
        'Quotationcannotplaced_offervalidityexpired', 'Report_GeneratedDownloaded_Successfully',
        'Could_Not_Download_TheReport', 'Error_while_downloading', 'SendingMailError_Validation', 'SuccessfullySentMail',
        'PleaseCorrectValidationErrors_GetServices', 'Toaster_ErrorMessage', 'Please_SelectService', 'RateCard_GeneratedDownloaded_Successfully',
        'Error_DownloadingPDF', 'Error_DownloadingExcel']).then(function (translations) {
            $scope.Frayte_Warning = translations.FrayteWarning;
            $scope.PleaseCorrect_ValidationErrors = translations.PleaseCorrectValidationErrors;
            $scope.Successfully_SavedInformation = translations.SuccessfullySavedInformation;
            $scope.ErrorSaving_Record = translations.ErrorSavingRecord;
            $scope.Frayte_Error = translations.FrayteError;
            $scope.Frayte_Success = translations.FrayteSuccess;
            $scope.Quotationoffervalidityexpired = translations.Quotationcannotplaced_offervalidityexpired;
            $scope.Report_GeneratedDownloadedSuccessfully = translations.Report_GeneratedDownloaded_Successfully;
            $scope.CouldNot_Download_TheReport = translations.Could_Not_Download_TheReport;
            $scope.Errorwhile_downloading = translations.Error_while_downloading;
            $scope.SendingMailErrorValidation = translations.SendingMailError_Validation;
            $scope.Successfully_SentMail = translations.SuccessfullySentMail;
            $scope.PleaseCorrectValidationErrorsGetServices = translations.PleaseCorrectValidationErrors_GetServices;
            $scope.ToasterErrorMessage = translations.Toaster_ErrorMessage;
            $scope.PleaseSelectService = translations.Please_SelectService;
            $scope.RateCardGeneratedDownloaded_Successfully = translations.RateCard_GeneratedDownloaded_Successfully;
            $scope.ErrorDownloadingPDF = translations.Error_DownloadingPDF;
            $scope.ErrorDownloadingExcel = translations.Error_DownloadingExcel;
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
            if ($scope.RoleId === 1) {
                $scope.quotationDetail.CustomerId = $scope.CustId;
                $scope.quotationDetail.CustomerName = $scope.CustName;
            }
            else {
                $scope.quotationDetail.CustomerId = $scope.customerId;
                $scope.quotationDetail.CustomerName = $scope.UserName;
            }
            $rootScope.GetServiceValue = null;

            if ($scope.quotationDetail.QuotationRateCard !== null && $scope.quotationDetail.QuotationRateCard.LogisticServiceId > 0) {
                AppSpinner.showSpinnerTemplate("Downloading Quote PDF", $scope.Template);
                $scope.quotationDetail.TotalEstimatedWeight = $scope.getChargeableWeight();
                QuotationService.GenerateQuotationShipmentPdf($scope.quotationDetail).then(function (response) {
                    var fileInfo = response.data;
                    var fileName = {
                        FileName: response.data.FileName
                    };
                    if (response.data != null) {
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
                                    $scope.NewQuotation();
                                    toaster.pop({
                                        type: 'success',
                                        title: $scope.Frayte_Success,
                                        body: $scope.Report_GeneratedDownloadedSuccessfully,
                                        showCloseButton: true
                                    });
                                    AppSpinner.hideSpinnerTemplate();
                                } catch (ex) {
                                    $scope.NewQuotation();
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
    $scope.SaveQuotation = function (IsValid) {
        if ($scope.quotationDetail.QuotationShipmentId === 0) {
            if (IsValid) {
                if ($scope.RoleId === 1) {
                    $scope.quotationDetail.CustomerId = $scope.CustId;
                }
                else {
                    $scope.quotationDetail.CustomerId = $scope.customerId;
                }

                $rootScope.GetServiceValue = null;

                if ($scope.quotationDetail.QuotationRateCard !== null && $scope.quotationDetail.QuotationRateCard.LogisticServiceId > 0) {
                    if ($scope.Db === 'PlaceBooking') {
                        AppSpinner.showSpinnerTemplate("Placing Booking", $scope.Template);
                    }
                    else {
                        AppSpinner.showSpinnerTemplate("Saving Quotation", $scope.Template);
                    }
                    $scope.quotationDetail.TotalEstimatedWeight = $scope.getChargeableWeight();
                    QuotationService.SaveQuotation($scope.quotationDetail).then(function (response) {
                        if (response.status === 200) {
                            //$rootScope.QuoteCustomerratecard = response.data.QuotationDetail.QuotationRateCard;
                            toaster.pop({
                                type: 'success',
                                title: $scope.Frayte_Success,
                                body: $scope.Successfully_SavedInformation,
                                showCloseButton: true
                            });
                            $scope.NewQuotation();
                            if ($scope.Db === 'PlaceBooking') {
                                getPlaceBookinLink(response.data.QuotationDetail.QuotationShipmentId);
                            }
                            AppSpinner.hideSpinnerTemplate();
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
        else {
            getPlaceBookinLink($scope.quotationDetail.QuotationShipmentId);
        }
    };

    var getPlaceBookinLink = function (QuotationShipmentId) {
        //QuotationService.GetPlaceBookingLink(QuotationShipmentId, 'quotation-shipment').then(function (response) {
        //$scope.PlaceBookingLink = response.data;
        if ($scope.RoleId === 1) {
            $state.go('admin.booking-home.direct-booking', { directShipmentId: QuotationShipmentId, callingtype: 'quotation-shipment' }, { reload: true });
        }
        else if ($scope.RoleId === 6) {
            $state.go('dbuser.booking-home.direct-booking', { directShipmentId: QuotationShipmentId, callingtype: 'quotation-shipment' }, { reload: true });
        }
        else if ($scope.RoleId === 3) {
            $state.go('customer.booking-home.direct-booking', { directShipmentId: QuotationShipmentId, callingtype: 'quotation-shipment' }, { reload: true });
        }


        //});
    };

    // Change lb to kg Package Details
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
                    "Weight": null
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
            "EstimatedTotalCost": 0.0
        };
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
            return Math.ceil(total).toFixed(1);
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

    //$scope.shipFromOptionalField = function (action) {
    //    if (action !== undefined && action !== null && action !== '' && action === 'State') {
    //        if ($scope.quotationDetail !== undefined && $scope.quotationDetail !== null && $scope.quotationDetail.QuotationFromAddress !== null && $scope.quotationDetail.QuotationFromAddress.Country !== null) {
    //            if ($scope.quotationDetail.QuotationFromAddress.Country.Code !== 'HKG' && $scope.quotationDetail.QuotationFromAddress.Country.Code !== 'GBR') {
    //                return true;
    //            }
    //            else {
    //                $scope.quotationDetail.QuotationFromAddress.State = '';
    //                return false;
    //            }
    //        }
    //    }
    //    else if (action !== undefined && action !== null && action !== '' && action === 'PostCode') {
    //        if ($scope.quotationDetail !== undefined && $scope.quotationDetail !== null && $scope.quotationDetail.QuotationFromAddress !== null && $scope.quotationDetail.QuotationFromAddress.Country !== null) {
    //            if ($scope.quotationDetail.QuotationFromAddress.Country.Code !== 'HKG' && $scope.quotationDetail.OperationZoneId === 1) {
    //                return true;
    //            }
    //            else if ($scope.quotationDetail.QuotationFromAddress.Country.Code !== 'GBR' && $scope.quotationDetail.OperationZoneId === 2) {
    //                return true;
    //            }
    //            else {
    //                $scope.quotationDetail.QuotationFromAddress.PostCode = '';
    //                return false;
    //            }
    //        }
    //    }

    //};
    //$scope.shipToOptionalField = function (action) {
    //    if (action !== undefined && action !== null && action !== '' && action === 'State') {
    //        if ($scope.quotationDetail !== undefined && $scope.quotationDetail !== null && $scope.quotationDetail.QuotationToAddress !== null && $scope.quotationDetail.QuotationToAddress.Country !== null) {
    //            if ($scope.quotationDetail.QuotationToAddress.Country.Code !== 'HKG' && $scope.quotationDetail.QuotationToAddress.Country.Code !== 'GBR') {
    //                return true;
    //            }
    //            else {
    //                $scope.quotationDetail.QuotationToAddress.State = '';
    //                return false;
    //            }
    //        }
    //    }
    //    else if (action !== undefined && action !== null && action !== '' && action === 'PostCode') {
    //        if ($scope.quotationDetail !== undefined && $scope.quotationDetail !== null && $scope.quotationDetail.QuotationToAddress !== null && $scope.quotationDetail.QuotationToAddress.Country !== null) {
    //            if ($scope.quotationDetail.QuotationToAddress.Country.Code !== 'HKG') {
    //                return true;
    //            }
    //            else {
    //                $scope.quotationDetail.QuotationToAddress.PostCode = '';
    //                return false;
    //            }
    //        }
    //    }

    //};
    $scope.AddPackage = function () {
        $scope.quotationDetail.QuotationPackages.push({
            "QuotationShipmentDetailId": 0,
            "QuotationShipmentId": 0,
            "CartoonValue": null,
            "Length": null,
            "Width": null,
            "Height": null,
            "Weight": null
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
                // setParcelType(convertA + convertB);
                return convertA + convertB;
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
    // Set QuotationToAddress info
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

    // Change lb to kg Package Details
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
    $scope.GetServices = function (quotationDetail, DirectBookingForm) {

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
                    body: $scope.PleaseCorrectValidationErrorsGetServices,
                    showCloseButton: true
                });

                return;
            }
            if (quotationDetail.CustomerId === null || quotationDetail.CustomerId === 0) {
                AppSpinner.hideSpinner();
                toaster.pop({
                    type: 'warning',
                    title: $scope.Frayte_Warning,
                    body: $scope.PleaseCorrectValidationErrorsGetServices,
                    showCloseButton: true
                });

                return;
            }
            AppSpinner.hideSpinner();
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
                    }
                }
            });

            modalInstance.result.then(function (RateCard) {

                if (RateCard !== undefined && RateCard !== null) {
                    AppSpinner.hideSpinner();
                    $scope.quotationDetail.QuotationRateCard = RateCard;
                }
                $rootScope.GetRateValue = false;
            }, function () {
                AppSpinner.hideSpinner();
            });

        }
        else {
            AppSpinner.hideSpinner();
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.PleaseCorrectValidationErrorsGetServices,
                showCloseButton: true
            });
        }
        AppSpinner.hideSpinner();

    };
    $scope.sendQuotationMail = function (CustomerDetail, emailRate) {

        if ($scope.quotationDetail !== null && $scope.CustomerDetail !== null && $scope.CustomerDetail !== undefined && emailRate === 'EmailRateCard') {
            if ($scope.CustomerDetail.CustomerId !== undefined && $scope.CustomerDetail.CustomerId !== null) {
                $scope.CustoId = $scope.CustomerDetail.CustomerId;
            }
            else {
                $scope.CustoId = $scope.CustomerDetail.EmployeeId;
            }
            QuotationService.GetSalesRepresentiveEmail($scope.CustoId).then(function (response) {
                $scope.SalesRepresentativedata = {
                    Name: '',
                    Email: ''
                };
                $scope.SalesRepresentative = response.data;
                if (response.data === null) {
                    $scope.SalesRepresentativedata.Name = $scope.UserName;
                    $scope.SalesRepresentativedata.Email = $scope.UserEmail;
                }
                else {
                    $scope.SalesRepresentativedata.Name = $scope.SalesRepresentative.SalesRepresentiveName;
                    $scope.SalesRepresentativedata.Email = $scope.SalesRepresentative.SalesEmail;
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
                                return 'Thank you for your enquiry at FRAYTE Logistics Ltd. Please find your Quotation included in this email.';
                            },
                            SalesRepresentativeDetail: function () {
                                return $scope.SalesRepresentativedata;
                            }
                        }
                    });

                    modalInstanceMail.result.then(function () {
                        $scope.NewQuotation();
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
            if (CustomerDetail.CustomerId !== undefined && CustomerDetail.CustomerId !== null) {
                $scope.CustoId = CustomerDetail.CustomerId;
            }
            else {
                $scope.CustoId = CustomerDetail.EmployeeId;
            }

            //call to get sales representative
            QuotationService.GetSalesRepresentiveEmail($scope.CustoId).then(function (response) {
                $scope.SalesRepresentativedata = {
                    Name: '',
                    Email: ''
                };
                $scope.SalesRepresentative = response.data;
                if (response.data === null) {
                    $scope.SalesRepresentativedata.Name = $scope.UserName;
                    $scope.SalesRepresentativedata.Email = $scope.UserEmail;
                }
                else {
                    $scope.SalesRepresentativedata.Name = $scope.SalesRepresentative.SalesRepresentiveName;
                    $scope.SalesRepresentativedata.Email = $scope.SalesRepresentative.SalesEmail;
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
                                return 'Many thanks for requesting an Air Freight Quote for an ad hoc shipment at FRAYTE Logistics Ltd. It is our pleasure to present Your Quote as per the following attachment.Thank you very much and please feel free to let us know if you have any questions.';
                            },
                            SalesRepresentativeDetail: function () {
                                return $scope.SalesRepresentativedata;
                            }
                        }
                    });

                    modalInstanceEmailRateCard.result.then(function () {
                        $scope.NewQuotation();
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
    //Download Customer Rates

    $scope.DownloadCustomerRate = function (CompanyDetail) {

        if (CompanyDetail) {

            //var text = '';
            //if ($scope.filetype === 'pdf') {
            //    text = "pdf";
            //}
            //else if ($scope.filetype === 'exel') {
            //    text = "excel";
            //}
            //else {
            //    text = "summary";
            //}
            AppSpinner.showSpinnerTemplate("Downloading Rate Card PDF", $scope.Template);
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

                if (status == 200 && data !== null) {
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
                                        body: $scope.RateCardGeneratedDownloaded_Successfully,
                                        showCloseButton: true
                                    });
                                    AppSpinner.hideSpinnerTemplate();
                                } catch (ex) {
                                    toaster.pop({
                                        type: 'success',
                                        title: $scope.Frayte_Success,
                                        body: $scope.RateCardGeneratedDownloaded_Successfully,
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
                    //



                }
                else {
                    toaster.pop({
                        type: 'Error',
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

    //max value of weight will be 2 when parcel type is letter

    //$scope.$watch('Package.Height', function (newValue, oldValue, scope) {
    //    if (oldValue > 2) {
    //        var maxval = null;
    //        $scope.getmaxval = true;
    //    }

    //});
    $scope.ChangeWeight = function (weight) {
        $scope.Weight = weight;
        //var par = $scope.quotationDetail.ParcelType;
        if ($scope.quotationDetail.ParcelType === null) {
            return;
        }

        else if (weight > 2 && $scope.quotationDetail.ParcelType.ParcelType === 'Letter') {

            $scope.getmaxval = true;
        }
        else {
            $scope.getmaxval = false;
            return;

        }
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

    $scope.editGetServices = function (DirectBookingForm) {
        $scope.GetServices($scope.quotationDetail, DirectBookingForm);
    };
    $scope.cancelServices = function () {
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
    };

    $scope.getCustomerCurrencyDetail = function (CustomerDetail) {
        if (CustomerDetail !== undefined && CustomerDetail !== null) {
            $scope.CustName = CustomerDetail.CustomerName;
            $scope.CustId = CustomerDetail.CustomerId;
            $scope.CustEmail = CustomerDetail.EmailId;
            $scope.CustomerDetail = CustomerDetail;
            QuotationService.GetCustomerLogisticService(CustomerDetail.CustomerId).then(function (response) {
                $scope.CustomerServices = response.data;
                $scope.CustServLength = $scope.CustomerServices.length;

            });


        }
        else {
            $scope.CustServLength = 0;
            $scope.CustId = $scope.customerId;
        }
    };

    //$scope.EmailRateCard = function (CustomerDetail) {


    //    $scope.Customer = {
    //        UserId: $scope.CustId,
    //        LogisticServiceId: CustomerDetail.LogisticServiceId,
    //        LogisticCompany: CustomerDetail.LogisticCompany,
    //        LogisticType: CustomerDetail.LogisticType,
    //        RateType: CustomerDetail.RateType,
    //        FileType: '',
    //        CustomerName: $scope.CustName,
    //        SendingOption: 'EMAIL'
    //    };
    //    if ($scope.quotationDetail !== null && CustomerDetail !== null && CustomerDetail !== undefined) {
    //        QuotationService.SendCustomerRateCardAsEmail($scope.Customer).then(function (response) {
    //            if (response.status === 200 && response.data !== undefined && response.data !== null) {
    //                toaster.pop({
    //                    type: 'success',
    //                    title: "FRAYTE Success",
    //                    body: "Email sent successfully.",
    //                    showCloseButton: true
    //                });
    //                AppSpinner.hideSpinnerTemplate();
    //            }
    //            else {
    //                AppSpinner.hideSpinnerTemplate();
    //                toaster.pop({
    //                    type: 'error',
    //                    title: "FRAYTE Error",
    //                    body: "Error While sending the email.",
    //                    showCloseButton: true
    //                });
    //            }
    //        }, function () {
    //            AppSpinner.hideSpinnerTemplate();
    //            toaster.pop({
    //                type: 'error',
    //                title: "FRAYTE Error",
    //                body: "Error While sending the email.",
    //                showCloseButton: true
    //            });

    //        });
    //    }
    //    else {
    //        toaster.pop({
    //            type: 'warning',
    //            title: $scope.Frayte_Warning,
    //            body: "Please correct validation error first.",
    //            showCloseButton: true
    //        });
    //    }
    //};



    //Download Supplementry charges for customer rate card
    $scope.DownloadSupplementryCharges = function (CustomerDetail) {
        if (CustomerDetail) {
            AppSpinner.showSpinnerTemplate("Downloading Supplementary Charges PDF", $scope.Template);
            var Rate = {
                UserId: $scope.CustId,
                LogisticServiceId: CustomerDetail.LogisticServiceId,
                LogisticCompany: CustomerDetail.LogisticCompany,
                LogisticType: CustomerDetail.LogisticType,
                RateType: CustomerDetail.RateType,
                FileType: 'PDF',
                CustomerName: $scope.CustName
            };

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
                    //



                }
                else if (data.FileStatus === false) {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'warning',
                        title: $scope.Frayte_Warning,
                        body: 'No supplymentry charges for this Courier.',
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

    //Download Summary Call

    $scope.DownloadSummary = function (CustomerDetail) {
        if (CustomerDetail) {
            AppSpinner.showSpinnerTemplate("Downloading Rate Card Summary", $scope.Template);
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

                if (status == 200 && data !== null) {
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
                    //



                }
                else {
                    toaster.pop({
                        type: 'Error',
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
    // Newquote call

    $scope.newquote = function () {
        if ($scope.quotationDetail !== null && $scope.quotationDetail !== undefined) {
            $scope.NewQuotation();
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

    //Check Account Number Call


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
                    break;
                }
            }

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
        else if (newValue === undefined || newValue === "" ) {
            $scope.FilterCountry = newValue;
        }
    });

    //$scope.$watch('FilterCountry', function (newValue, oldValue) {
    //    var newValArr = [];

    //    if ($scope.directBookingCustomers !== undefined && $scope.directBookingCustomers !== null && $scope.directBookingCustomers.length ===1) {
    //        $scope.CustomerDetail = $scope.directBookingCustomers[0];
    //    }
    //});
    function init() {
        $rootScope.GetServiceValue = null;
        //$scope.regexnosp = '/^[!0]*[0-9-\)\(]+$/';
        //$scope.NewOuatationDetail();
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';

        $scope.submitted = true;
        $scope.sendMail = false;
        var userInfo = SessionService.getUser();
        $scope.RoleId = userInfo.RoleId;
        $scope.CreatedBy = userInfo.EmployeeId;
        $scope.customerId = userInfo.EmployeeId;
        $scope.UserName = userInfo.UserName;
        $scope.UserEmail = userInfo.EmployeeMail;
        $scope.OperationZoneId = userInfo.OperationZoneId;
        $scope.CustoDetail = userInfo;
        if ($scope.RoleId !== 1) {
            $scope.CustomerDetail = userInfo;
            $scope.CustName = userInfo.UserName;
            $scope.CustEmail = userInfo.EmployeeMail;
            $scope.CustId = userInfo.EmployeeId;

        }




        //Step 1: Get initials and default address of logged in customer.
        DirectBookingService.GetInitials($scope.customerId).then(function (response) {
            // Set Country type according to given order
            $scope.CountriesRepo = TopCountryService.TopCountryList(response.data.Countries);
            $scope.PiecesExcelDownloadPathXlsx = response.data.PiecesExcelDownloadPathXlsx;
            $scope.PiecesExcelDownloadPathXls = response.data.PiecesExcelDownloadPathXls;
            $scope.PiecesExcelDownloadPathCsv = response.data.PiecesExcelDownloadPathCsv;
            // Set Currency type according to given order
            $scope.CurrencyTypes = TopCurrencyService.TopCurrencyList(response.data.CurrencyTypes);
            $scope.NewQuotation();
            $scope.ParcelTypes = response.data.ParcelTypes;
            $scope.quotationDetail.ParcelType = response.data.ParcelTypes[0];

            $scope.CountryPhoneCodes = response.data.CountryPhoneCodes;


            $scope.changeKgToLb($scope.quotationDetail.PakageCalculatonType);
        },
       function () {
           toaster.pop({
               type: 'error',
               title: $scope.FrayteServiceErrorValidation,
               body: $scope.InitialDataValidation,
               showCloseButton: true
           });
       });
        var currentYear1 = new Date();
        var currentYear = currentYear1.getFullYear();
        if ($scope.RoleId !== 1) {
            $scope.CustName = userInfo.UserName;
            QuotationService.GetCustomerLogisticService($scope.customerId).then(function (response) {
                $scope.CustomerServices = response.data;
                $scope.CustServLength = $scope.CustomerServices.length;
            });
        }
        $scope.paymentAccount = true;
        DirectBookingService.GetDirectBookingCustomers($scope.customerId, "DirectBooking").then(function (response) {
            $scope.directBookingCustomers = response.data;
            var dbCustomers = [];

            for (i = 0; i < $scope.directBookingCustomers.length; i++) {

                if ($scope.directBookingCustomers[i].OperationZoneId === userInfo.OperationZoneId) {
                    dbCustomers.push($scope.directBookingCustomers[i]);
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
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteServiceErrorValidation,
                body: $scope.ReceiveDetail_Validation,
                showCloseButton: true
            });
        });
        setModalOptions();
    }

    init();
});