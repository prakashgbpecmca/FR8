angular.module('ngApp.quotationTools').controller('QuotationToolServicesController', function ($scope, IsRateShow, config, UtilityService, toaster, AppSpinner, uiGridConstants, $translate, DirectBookingService, ModalService, $uibModalInstance, directBookingObj, $rootScope, CustomerId, SessionService, $http, $window, LogisticService, QuotationService, $uibModal, CallingFrom) {

    var setMultilingualOptions = function () {
        $translate(['ServiceUnavailable_Validation', 'ServiceNotAvailable_Validation', 'FrayteWarning', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation', 'ErrorSavingRecord',
                'FrayteError', 'FrayteSuccess', 'Could_Not_SelectService', 'Report_GeneratedDownloaded_Successfully', 'Error_DownloadingPDF',
                'Nosupplymentry_charges_Courier', 'DownloadingSupplementaryPDF', 'For_Additional_Domestic', 'Visit', 'Read', 'UPS_Info', 'TNT_Info', 'GetServicePopUpLoading']).then(function (translations) {
                    $scope.ServiceUnavailableValidation = translations.ServiceUnavailableValidation;
                    $scope.ServiceNotAvailableValidation = translations.ServiceNotAvailableValidation;
                    $scope.Frayte_Warning = translations.FrayteWarning;
                    $scope.PleaseCorrect_ValidationErrors = translations.PleaseCorrectValidationErrors;
                    $scope.Successfully_SavedInformation = translations.SuccessfullySavedInformation;
                    $scope.ErrorSaving_Record = translations.ErrorSavingRecord;
                    $scope.Frayte_Error = translations.FrayteError;
                    $scope.Frayte_Success = translations.FrayteSuccess;
                    $scope.Could_NotSelectService = translations.Could_Not_SelectService;
                    $scope.Report_GeneratedDownloadedSuccessfully = translations.Report_GeneratedDownloaded_Successfully;
                    $scope.ErrorDownloadingPDF = translations.Error_DownloadingPDF;
                    $scope.Nosupplymentrycharges_Courier = translations.Nosupplymentry_charges_Courier;
                    $scope.DownloadingSupplementaryPDF = translations.DownloadingSupplementaryPDF;
                    $scope.ForAdditionalDomestic = translations.For_Additional_Domestic;
                    $scope.VisitInfo = translations.Visit;
                    $scope.ReadInfo = translations.Read;
                    $scope.UPSInfo = translations.UPS_Info;
                    $scope.TNTInfo = translations.TNT_Info;
                    $scope.GetServicePopUpLoading = translations.GetServicePopUpLoading;

                    $scope.GetInitial();
                });
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
                        total += ((len * wid * height) / 6000) * qty;
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

    $scope.getTotalKgs = function (quotationDetail) {
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

    $scope.PackagesTotal = function (items, prop) {
        if (items === null || items === undefined) {
            return 0;
        }
        else {
            return items.reduce(function (a, b) {
                var convertB = 0;
                if (prop === "Weight" || prop === "Weight") {

                }
                if (b[prop] !== undefined && b[prop] !== null) {
                    convertB = parseFloat(b[prop]);
                }
                var convertA = 0;
                if (a !== undefined && a !== null) {
                    convertA = parseFloat(a);
                }

                return convertA + convertB;
            }, 0);
        }
    };

    $scope.cancelService = function () {
        $uibModalInstance.dismiss();
        $rootScope.GetRateValue = false;
    };

    $scope.setCustomerRateCard = function (RateCard) {
        if (RateCard !== undefined && RateCard !== null) {
            if ($scope.quotationDetail.QuotationPackages.length === 1 && ($scope.quotationDetail.QuotationPackages[0].CartoonValue === 1 || $scope.quotationDetail.QuotationPackages[0].CartoonValue === '1')) {
                RateCard.Length = $scope.quotationDetail.QuotationPackages[0].Length;
                RateCard.Width = $scope.quotationDetail.QuotationPackages[0].Width;
                RateCard.Height = $scope.quotationDetail.QuotationPackages[0].Height;
                //RateCard.Weight = $scope.getChargeableWeight();
                RateCard.PakageCalculatonType = $scope.quotationDetail.PakageCalculatonType;
                $uibModalInstance.close(RateCard);
            }
            else {
                $uibModalInstance.close(RateCard);
            }
        }
    };

    $scope.SelectServiceRow = function (service) {
        for (var i = 0 ; i < $scope.courierServices.length; i++) {
            if ($scope.courierServices[i].IsSelected) {
                $scope.courierServices[i].IsSelected = false;
            }
        }
        service.IsSelected = true;
        $scope.SelectedService.CourierService = service;
    };

    getServiceNewJson = function () {

        var obj = {

        };

        $scope.newUkShipmentService = {
            IsUkShipment: false,
            Services: []
        };

        if ($scope.courierServices !== null && $scope.courierServices.length > 0) {
            //setServiceOptions();

            if ($scope.ukShipmentService.UkmailServiceList.length) {

                $scope.ukShipmentService.UkmailServiceList.sort(function (b, a) { return b.TotalEstimatedCharge - a.TotalEstimatedCharge; });
                obj = {

                };
                obj.CourierName = $scope.ukShipmentService.UkmailServiceList[0].CourierName;
                obj.CourierDisplayName = $scope.ukShipmentService.UkmailServiceList[0].DisplayName;
                obj.Services = $scope.ukShipmentService.UkmailServiceList;

                $scope.newUkShipmentService.IsUkShipment = true;
                $scope.newUkShipmentService.Services.push(obj);
            }

            if ($scope.ukShipmentService.YodelServiceList.length) {

                obj = {

                };
                $scope.ukShipmentService.YodelServiceList.sort(function (b, a) { return b.TotalEstimatedCharge - a.TotalEstimatedCharge; });

                obj.CourierName = $scope.ukShipmentService.YodelServiceList[0].CourierName;
                obj.CourierDisplayName = $scope.ukShipmentService.YodelServiceList[0].DisplayName;
                obj.Services = $scope.ukShipmentService.YodelServiceList;
                $scope.newUkShipmentService.Services.push(obj);
            }

            if ($scope.ukShipmentService.HermesServiceList.length) {
                obj = {

                };
                obj.CourierName = $scope.ukShipmentService.HermesServiceList[0].CourierName;
                obj.CourierDisplayName = $scope.ukShipmentService.HermesServiceList[0].DisplayName;
                obj.Services = [];

                $scope.ukShipmentService.HermesServiceList.sort(function (b, a) { return b.TotalEstimatedCharge - a.TotalEstimatedCharge; });
                obj.Services = $scope.ukShipmentService.HermesServiceList;
                $scope.newUkShipmentService.Services.push(obj);

            }

            if ($scope.ukShipmentService.UKShipmentDHLServiceList.length) {
                obj = {

                };
                obj.CourierName = $scope.ukShipmentService.UKShipmentDHLServiceList[0].CourierName;
                obj.CourierDisplayName = $scope.ukShipmentService.UKShipmentDHLServiceList[0].DisplayName;
                obj.Services = [];
                $scope.ukShipmentService.UKShipmentDHLServiceList.sort(function (b, a) { return b.TotalEstimatedCharge - a.TotalEstimatedCharge; });
                obj.Services = $scope.ukShipmentService.UKShipmentDHLServiceList;
                $scope.newUkShipmentService.IsUkShipment = true;
                $scope.newUkShipmentService.Services.push(obj);
            }

            //if ($scope.ukShipmentService.DPDServiceList.length) {
            //    obj = {

            //    };
            //    obj.CourierName = $scope.ukShipmentService.DPDServiceList[0].CourierName;
            //    obj.CourierDisplayName = $scope.ukShipmentService.DPDServiceList[0].DisplayName;
            //    obj.Services = [];
            //    $scope.ukShipmentService.DPDServiceList.sort(function (b, a) { return b.TotalEstimatedCharge - a.TotalEstimatedCharge; });
            //    obj.Services = $scope.ukShipmentService.DPDServiceList;
            //    $scope.newUkShipmentService.Services.push(obj);
            //}

            /// For HK Operation zone we do not need to group services by courier company
            if ($scope.ukShipmentService.OtherServiceList.length) {
                $scope.newUkShipmentService.IsUkShipment = false;

                obj = {

                };

                $scope.ukShipmentService.OtherServiceList.sort(function (b, a) { return b.TotalEstimatedCharge - a.TotalEstimatedCharge; });
                $scope.newUkShipmentService.Services = $scope.ukShipmentService.OtherServiceList;

            }
        }
        $scope.setGridOrder($scope.SortBy);
    };

    getServicesJson = function () {

        $scope.ukShipmentService = {
            IsUkShipment: false,
            CourierType: '',
            FirstUkmailService: null,
            UkmailServiceList: [],
            FirstYodelService: null,
            YodelServiceList: [],
            FirstHermesService: null,
            HermesServiceList: [],
            FirstService: null,
            OtherServiceList: [],
            UKShipmentDHLServiceList: [],
            FirstUkmailDHLService: null
            //DPDServiceList: [],
            //FirstDPDService: null
        };

        $scope.noUkShipmentService = [];

        if ($scope.courierServices !== null && $scope.courierServices.length > 0) {
            for (var i = 0; i < $scope.courierServices.length; i++) {
                if ($scope.courierServices[i].LogisticType === 'UKShipment' &&
                    $scope.courierServices[i].CourierName === 'UKMail') {
                    $scope.ukShipmentService.IsUkShipment = true;
                    $scope.ukShipmentService.CourierType = $scope.courierServices[i].CourierName;
                    $scope.ukShipmentService.AdditionalSurchargeValue = $scope.courierServices[i].AdditionalSurchargeValue;
                    $scope.ukShipmentService.AdditionalSurcharge = $scope.courierServices[i].AdditionalSurcharge;
                    $scope.courierServices[i].RowSelect = false;
                    $scope.courierServices[i].ShipmentType = $scope.courierServices[i].WeightType + " ( " + $scope.courierServices[i].ParcelServiceType + " ) ";
                    $scope.courierServices[i].ImageURL = config.BUILD_URL + "UKmail.png";
                    $scope.ukShipmentService.UkmailServiceList.push($scope.courierServices[i]);
                }
                else if ($scope.courierServices[i].LogisticType === 'UKShipment' &&
                    $scope.courierServices[i].CourierName === "Yodel") {
                    $scope.ukShipmentService.IsUkShipment = true;
                    $scope.ukShipmentService.CourierType = $scope.courierServices[i].CourierName;
                    $scope.ukShipmentService.AdditionalSurchargeValue = $scope.courierServices[i].AdditionalSurchargeValue;
                    $scope.ukShipmentService.AdditionalSurcharge = $scope.courierServices[i].AdditionalSurcharge;
                    $scope.courierServices[i].RowSelect = false;
                    $scope.courierServices[i].ShipmentType = $scope.courierServices[i].WeightType;
                    $scope.courierServices[i].ImageURL = config.BUILD_URL + "yodel.png";
                    $scope.ukShipmentService.YodelServiceList.push($scope.courierServices[i]);
                }
                else if ($scope.courierServices[i].LogisticType === 'UKShipment' &&
                    $scope.courierServices[i].CourierName === "Hermes") {
                    $scope.ukShipmentService.IsUkShipment = true;
                    $scope.ukShipmentService.CourierType = $scope.courierServices[i].CourierName;
                    $scope.ukShipmentService.AdditionalSurchargeValue = $scope.courierServices[i].AdditionalSurchargeValue;
                    $scope.ukShipmentService.AdditionalSurcharge = $scope.courierServices[i].AdditionalSurcharge;
                    $scope.courierServices[i].RowSelect = false;
                    $scope.courierServices[i].ShipmentType = $scope.courierServices[i].PakageType + " ( " + $scope.courierServices[i].ParcelServiceType + " ) ";
                    $scope.courierServices[i].ImageURL = config.BUILD_URL + "hermes.png";
                    $scope.ukShipmentService.HermesServiceList.push($scope.courierServices[i]);
                }
                else if ($scope.courierServices[i].LogisticType === 'UKShipment' &&
                    $scope.courierServices[i].CourierName === "DHL") {
                    $scope.ukShipmentService.IsUkShipment = true;
                    $scope.ukShipmentService.CourierType = $scope.courierServices[i].CourierName;
                    $scope.ukShipmentService.AdditionalSurchargeValue = $scope.courierServices[i].AdditionalSurchargeValue;
                    $scope.ukShipmentService.AdditionalSurcharge = $scope.courierServices[i].AdditionalSurcharge;
                    $scope.courierServices[i].RowSelect = false;
                    $scope.courierServices[i].ShipmentType = $scope.courierServices[i].WeightType;
                    $scope.courierServices[i].ImageURL = config.BUILD_URL + "DHL.png";
                    $scope.ukShipmentService.UKShipmentDHLServiceList.push($scope.courierServices[i]);
                }                  
                else {
                    if ($scope.courierServices[i].CourierName === "DHL") {
                        $scope.courierServices[i].ImageURL = config.BUILD_URL + "DHL.png";
                    }
                    else if ($scope.courierServices[i].CourierName === "FedEx") {
                        $scope.courierServices[i].ImageURL = config.BUILD_URL + "FedEx.png";
                    }
                    else if ($scope.courierServices[i].CourierName === "TNT") {
                        $scope.courierServices[i].ImageURL = config.BUILD_URL + "TNT.png";
                    }
                    else if ($scope.courierServices[i].CourierName === "UPS") {
                        $scope.courierServices[i].ImageURL = config.BUILD_URL + "UPS.png";
                    }
                   
                    $scope.ukShipmentService.IsUkShipment = false;
                    $scope.ukShipmentService.CourierType = $scope.courierServices[i].CourierName;
                    $scope.courierServices[i].RowSelect = false;
                    $scope.ukShipmentService.OtherServiceList.push($scope.courierServices[i]);
                }
            }

            if ($scope.ukShipmentService.UkmailServiceList.length) {
                $scope.ukShipmentService.UkmailServiceList.sort(function (b, a) { return b.TotalEstimatedCharge - a.TotalEstimatedCharge; });
                $scope.ukShipmentService.FirstUkmailService = $scope.ukShipmentService.UkmailServiceList[0];
                //$scope.ukShipmentService.UkmailServiceList.splice(0, 1);
            }
            if ($scope.ukShipmentService.YodelServiceList.length) {
                $scope.ukShipmentService.YodelServiceList.sort(function (b, a) { return b.TotalEstimatedCharge - a.TotalEstimatedCharge; });
                $scope.ukShipmentService.FirstYodelService = $scope.ukShipmentService.YodelServiceList[0];
                //$scope.ukShipmentService.YodelServiceList.splice(0, 1);
            }
            if ($scope.ukShipmentService.HermesServiceList.length) {
                $scope.ukShipmentService.HermesServiceList.sort(function (b, a) { return b.TotalEstimatedCharge - a.TotalEstimatedCharge; });
                $scope.ukShipmentService.FirstHermesService = $scope.ukShipmentService.HermesServiceList[0];
                //$scope.ukShipmentService.HermesServiceList.splice(0, 1);
            }
            if ($scope.ukShipmentService.UKShipmentDHLServiceList.length) {
                $scope.ukShipmentService.UKShipmentDHLServiceList.sort(function (b, a) { return b.TotalEstimatedCharge - a.TotalEstimatedCharge; });
                $scope.ukShipmentService.FirstUkmailDHLService = $scope.ukShipmentService.UKShipmentDHLServiceList[0];
                //$scope.ukShipmentService.UKShipmentDHLServiceList.splice(0, 1);
            }
            //if ($scope.ukShipmentService.DPDServiceList.length) {
            //    $scope.ukShipmentService.DPDServiceList.sort(function (b, a) { return b.TotalEstimatedCharge - a.TotalEstimatedCharge; });
            //    $scope.ukShipmentService.FirstDPDService = $scope.ukShipmentService.DPDServiceList[0];
            //    //$scope.ukShipmentService.DPDServiceList.splice(0, 1);
            //}

            if ($scope.ukShipmentService.FirstUkmailService !== null && $scope.ukShipmentService.FirstYodelService !== null &&
                $scope.ukShipmentService.FirstHermesService !== null && $scope.ukShipmentService.FirstUkmailDHLService !== null &&
                parseFloat($scope.ukShipmentService.FirstUkmailService.TotalEstimatedCharge) < parseFloat($scope.ukShipmentService.FirstYodelService.TotalEstimatedCharge) &&
                parseFloat($scope.ukShipmentService.FirstUkmailService.TotalEstimatedCharge) < parseFloat($scope.ukShipmentService.FirstHermesService.TotalEstimatedCharge) &&
                parseFloat($scope.ukShipmentService.FirstUkmailService.TotalEstimatedCharge) < parseFloat($scope.ukShipmentService.FirstUkmailDHLService.TotalEstimatedCharge)
                ) {
                $scope.IsUkMailCheapest = true;
            }
            else if ($scope.ukShipmentService.FirstUkmailService !== null && $scope.ukShipmentService.FirstYodelService !== null &&
                     $scope.ukShipmentService.FirstHermesService !== null && $scope.ukShipmentService.FirstUkmailDHLService !== null &&
                     parseFloat($scope.ukShipmentService.FirstYodelService.TotalEstimatedCharge) < parseFloat($scope.ukShipmentService.FirstHermesService.TotalEstimatedCharge) &&
                     parseFloat($scope.ukShipmentService.FirstYodelService.TotalEstimatedCharge) < parseFloat($scope.ukShipmentService.FirstUkmailService.TotalEstimatedCharge) &&
                     parseFloat($scope.ukShipmentService.FirstYodelService.TotalEstimatedCharge) < parseFloat($scope.ukShipmentService.FirstUkmailDHLService.TotalEstimatedCharge)
                    ) {
                $scope.IsYodelCheapest = true;
            }
            else if ($scope.ukShipmentService.FirstUkmailService !== null && $scope.ukShipmentService.FirstYodelService !== null &&
                     $scope.ukShipmentService.FirstHermesService !== null && $scope.ukShipmentService.FirstUkmailDHLService !== null &&
                     parseFloat($scope.ukShipmentService.FirstHermesService.TotalEstimatedCharge) < parseFloat($scope.ukShipmentService.FirstUkmailDHLService.TotalEstimatedCharge) &&
                     parseFloat($scope.ukShipmentService.FirstHermesService.TotalEstimatedCharge) < parseFloat($scope.ukShipmentService.FirstYodelService.TotalEstimatedCharge) &&
                     parseFloat($scope.ukShipmentService.FirstHermesService.TotalEstimatedCharge) < parseFloat($scope.ukShipmentService.FirstUkmailService.TotalEstimatedCharge)
                    ) {
                $scope.IsHermesCheapest = true;
            }
            else if ($scope.ukShipmentService.FirstUkmailService !== null && $scope.ukShipmentService.FirstYodelService !== null &&
                     $scope.ukShipmentService.FirstHermesService !== null && $scope.ukShipmentService.FirstUkmailDHLService !== null &&
                     parseFloat($scope.ukShipmentService.FirstUkmailDHLService.TotalEstimatedCharge) < parseFloat($scope.ukShipmentService.FirstUkmailService.TotalEstimatedCharge) &&
                     parseFloat($scope.ukShipmentService.FirstUkmailDHLService.TotalEstimatedCharge) < parseFloat($scope.ukShipmentService.FirstYodelService.TotalEstimatedCharge) &&
                     parseFloat($scope.ukShipmentService.FirstUkmailDHLService.TotalEstimatedCharge) < parseFloat($scope.ukShipmentService.FirstHermesService.TotalEstimatedCharge)
                   ) {
                $scope.IsDHLCheapest = true;
            }
        }
        getServiceNewJson();
    };

    $scope.selectCustomerRateCard = function (RateCard) {
        if ($scope.ukShipmentService !== undefined && $scope.courierServices !== null && $scope.courierServices.length > 0) {
            for (var i = 0; i < $scope.courierServices.length; i++) {
                $scope.courierServices[i].RowSelect = false;
            }
        }
        if (RateCard !== undefined && RateCard !== null) {
            RateCard.RowSelect = true;
        }
    };

    //Get services Supplementry charges
    $scope.DownloadGetServiceSupplymentryCharges = function (GetServiceDetail) {
        $rootScope.GetRateValue = null;
        if (GetServiceDetail) {

            AppSpinner.showSpinnerTemplate($scope.DownloadingSupplementaryPDF, $scope.Template);
            var Rate = {
                UserId: $scope.CustId,
                LogisticServiceId: GetServiceDetail.LogisticServiceId,
                LogisticCompany: GetServiceDetail.CourierName,
                LogisticType: GetServiceDetail.LogisticType,
                RateType: GetServiceDetail.RateType,
                FileType: 'PDF',
                CustomerName: $scope.CustName
            };

            QuotationService.GetTNTSupplemetoryInfo(GetServiceDetail.LogisticServiceId).then(function (response) {
                if (response.data !== undefined && response.data !== null) {
                    if ((response.data.LogisticCompany === 'TNT' && response.data.OperationZoneId === 2)) {
                        AppSpinner.hideSpinnerTemplate();
                        //|| (response.data.LogisticCompany === 'UPS' && response.data.OperationZoneId === 1)
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
                                       toaster.pop({
                                           type: 'warning',
                                           title: $scope.Frayte_Warning,
                                           body: $scope.Nosupplymentrycharges_Courier,
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
                                    body: $scope.Nosupplymentrycharges_Courier,
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

    var sortByAscending = function () {
        $scope.newUkShipmentService.Services.sort(compare);
    };

    var sortByDescending = function () {
        $scope.newUkShipmentService.Services.sort(deCompare);
    };

    function compare(a, b) {
        if (a.CourierName < b.CourierName)
        { return -1; }
        if (a.CourierName > b.CourierName)
        { return 1; }
        return 0;
    }

    function deCompare(a, b) {
        if (a.CourierName < b.CourierName) {
            return 1;
        }
        if (a.CourierName > b.CourierName) {
            return -1;
        }
        return 0;
    }

    var setAlternateRow = function () {
        if ($scope.newUkShipmentService.Services && $scope.newUkShipmentService.Services.length) {
            var n = 0;
            for (var i = 0 ; i < $scope.newUkShipmentService.Services.length ; i++) {
                if ($scope.newUkShipmentService.Services[i].Services && $scope.newUkShipmentService.Services[i].Services.length) {
                    for (var j = 0 ; j < $scope.newUkShipmentService.Services[i].Services.length ; j++) {
                        n++;
                        if (n % 2 === 0) {
                            $scope.newUkShipmentService.Services[i].Services[j].backGroundColor = '#fdfdfd';
                        }
                        else {
                            $scope.newUkShipmentService.Services[i].Services[j].backGroundColor = '#d4d4d4';
                        }
                    }
                }
            }
        }
    };

    $scope.setGridOrder = function (Type, ButtonType) {

        if (Type === 'CourierName' || Type === '-CourierName') {
            $scope.IsPriceGrid = false;
            if (Type === "CourierName") {
                $scope.sortByOrder = 'Ascending';
                sortByAscending();
            }
            else if (Type === "-CourierName") {
                $scope.sortByOrder = 'Descending';
                sortByDescending();
            }
            setAlternateRow();
        }
        else if (Type === 'Rate' || Type === '-Rate' || Type === 'TotalEstimatedCharge' || Type === '-TotalEstimatedCharge') {
            $scope.IsPriceGrid = true;
            if (Type === "Rate") {
                $scope.sortByOrder = 'Ascending';
                $scope.courierServices.sort(function (a, b) { return a.Rate - b.Rate; });
            }
            else if (Type === "-Rate") {
                $scope.sortByOrder = 'Descending';
                $scope.courierServices.sort(function (a, b) { return b.Rate - a.Rate; });
            }
            else if (Type === "TotalEstimatedCharge") {
                $scope.sortByOrder = 'Ascending';
                $scope.courierServices.sort(function (a, b) { return a.TotalEstimatedCharge - b.TotalEstimatedCharge; });
            }
            else if (Type === "-TotalEstimatedCharge") {
                $scope.sortByOrder = 'Descending';
                $scope.courierServices.sort(function (a, b) { return b.TotalEstimatedCharge - a.TotalEstimatedCharge; });
            }
        }
    };

    $scope.gridbyIcon = function (Type, ButtonType) {
        for (i = 0; i < $scope.options.length; i++) {
            if (ButtonType === 'buttonAscending' && Type === 'CourierName') {
                Type = $scope.options[0].value;
                $scope.setGridOrder(Type, ButtonType);
                $scope.sortByOrder = 'Ascending';
                return Type;
            }
            else if (ButtonType === 'buttonDescending' && Type === '-TotalEstimatedCharge') {
                Type = $scope.options[$scope.options.length - 1].value;
                $scope.setGridOrder(Type, ButtonType);
                $scope.sortByOrder = 'Descending';
                return Type;
            }
            else if (ButtonType === 'buttonAscending' && Type === $scope.options[i].value) {
                Type = $scope.options[i - 1].value;
                $scope.setGridOrder(Type, ButtonType);
                $scope.sortByOrder = 'Ascending';
                return Type;
            }
            else if (ButtonType === 'buttonDescending' && Type === $scope.options[i].value) {
                Type = $scope.options[i + 1].value;
                $scope.setGridOrder(Type, ButtonType);
                $scope.sortByOrder = 'Descending';
                return Type;
            }

        }
    };

    $scope.GetInitial = function () {

        // Initially sortby = 'CourierName'
        $scope.IsPriceGrid = false;
        // For sorting
        $scope.sortByOrder = 'Ascending';
        $scope.options = [
              {
                  value: "CourierName",
                  display: "Courier (A to Z)"
              },
               {
                   value: "-CourierName",
                   display: "Courier (Z to A)"
               },
               {
                   value: "Rate",
                   display: "Price (Low to High)"
               },
             {
                 value: "-Rate",
                 display: "Price (High to Low)"
             },
             {
                 value: "TotalEstimatedCharge",
                 display: "Total Estimated Cost (Low to High)"
             },
            {
                value: "-TotalEstimatedCharge",
                display: "Total Estimated Cost (High to Low)"
            }
        ];
        $scope.SortBy = "CourierName";
        if ($scope.RoleId === 1) {
            $scope.CustomerId = CustomerId;

        }
        else {
            $scope.CustomerId = directBookingObj.CustomerId;
        }

        $scope.SelectedService = {
            CourierService: null
        };
        $scope.showHideUKmail = false;


        //Set injected values to local variable
        $scope.quotationDetail = directBookingObj;
        if ($scope.quotationDetail.AddressType !== undefined && $scope.quotationDetail.AddressType !== null) {
            $scope.Address = $scope.quotationDetail.AddressType.Name;
        }
        else {
            $scope.Address = '';
        }

        var weight = $scope.getChargeableWeight();
        var totalPieces = $scope.totalPieces($scope.quotationDetail);
        var directBookingFindService = {
            CustomerId: $scope.CustomerId,
            FromCountry: null,
            FromPostCode: "",
            ToCountry: null,
            ToPostCode: "",
            Weight: 0.0,
            OperationZoneId: 0,
            Date: new Date(),
            Currency: '',
            AddressType: $scope.Address,
            CallingFrom: "",
            PackageCalculationType: "",
            Packages: []
        };

        //Ste DirectBookingFindService details
        directBookingFindService.FromCountry = directBookingObj.QuotationFromAddress.Country;
        directBookingFindService.FromPostCode = directBookingObj.QuotationFromAddress.PostCode;
        directBookingFindService.ToCountry = directBookingObj.QuotationToAddress.Country;
        directBookingFindService.ToPostCode = directBookingObj.QuotationToAddress.PostCode;
        directBookingFindService.Weight = weight;
        directBookingFindService.OperationZoneId = directBookingObj.OpearionZoneId;
        directBookingFindService.CallingFrom = CallingFrom;
        directBookingFindService.PackageCalculationType = directBookingObj.PakageCalculatonType;

        for (i = 0; i < directBookingObj.QuotationPackages.length; i++) {
            directBookingFindService.Packages.push(directBookingObj.QuotationPackages[i]);
        }

        if (totalPieces === 1) {
            directBookingFindService.PackageType = 'Single';
        }
        else if (totalPieces > 1) {
            directBookingFindService.PackageType = 'Multiple';
        }

        if (directBookingFindService.FromCountry.Code !== directBookingFindService.ToCountry.Code) {
            if (directBookingObj.ParcelType !== null && directBookingObj.ParcelType.ParcelDescription === "Parcel (Non Doc)") {
                directBookingFindService.DocType = 'Nondoc';
            }
            else if (directBookingObj.ParcelType !== null && directBookingObj.ParcelType.ParcelDescription === "Letter (Document)") {
                directBookingFindService.DocType = 'Doc';
            }
        }
        else {
            directBookingFindService.DocType = '';
        }

        if (LogisticService !== undefined && LogisticService !== null && LogisticService !== '') {
            if (LogisticService === 'Yodel' && directBookingObj.AddressType !== undefined && directBookingObj.AddressType !== null) {
                directBookingFindService.AddressType = directBookingObj.AddressType.Name;
            }
            else {
                directBookingFindService.AddressType = '';
            }
        }
        else {
            directBookingFindService.AddressType = '';
        }

        AppSpinner.showSpinnerTemplate($scope.GetServicePopUpLoading, $scope.Template);
        QuotationService.GetCustomerLogisticServices(directBookingFindService.CustomerId).then(function (response) {
            if (response.data === true) {
                DirectBookingService.GetServices(directBookingFindService).then(function (response) {
                    if (response.data.length > 0) {
                        var k = response.data.length - 1;
                        var j = angular.copy(response.data.length);
                        for (i = 0; i < j; i++) {
                            if (response.data[k].CourierName === 'AU' ||
                                response.data[k].CourierName === 'DPD' ||
                                response.data[k].CourierName === 'EAM' ||
                                response.data[k].CourierName === 'EAM-DHL' ||
                                response.data[k].CourierName === 'EAM-TNT' ||
                                response.data[k].CourierName === 'EAM-FedEx' ||
                                response.data[k].CourierName === 'BRING' ||
                                response.data[k].CourierName === 'DPDCH' ||
                                response.data[k].CourierName === 'SKYPOSTAL' ||
                                response.data[k].CourierName === 'CANADAPOST') {
                                response.data.splice(k, 1);
                                k = k - 1;
                            }
                        }

                        for (l = 0; l < response.data.length; l++) {
                            response.data[l].AdditionalSurchargeValue = response.data[l].AdditionalSurcharge; //For Checking price greater in UI
                            response.data[l].AdditionalSurcharge = Number(parseFloat(response.data[l].AdditionalSurcharge).toFixed(2)).toLocaleString('en', { minimumFractionDigits: 2 });
                            response.data[l].FuelSurchargeValue = response.data[l].FuelSurcharge; //For Checking price greater in UI
                            response.data[l].FuelSurcharge = response.data[l].FuelSurcharge.toFixed(2);
                            response.data[l].RateValue = response.data[l].Rate; //For Checking price greater in UI
                            response.data[l].Rate = Number(parseFloat(response.data[l].Rate).toFixed(2)).toLocaleString('en', { minimumFractionDigits: 2 });
                            response.data[l].TotalEstimatedChargeValue = response.data[l].TotalEstimatedCharge; //For Checking price greater in UI
                            response.data[l].TotalEstimatedCharge = Number(parseFloat(response.data[l].TotalEstimatedCharge).toFixed(2)).toLocaleString('en', { minimumFractionDigits: 2 });
                        }


                        $scope.courierServices = response.data;
                        $scope.showHideUKmail = false;
                        $scope.showHideYodel = false;
                        $scope.showHideHermes = false;
                        getServicesJson();
                        AppSpinner.hideSpinnerTemplate();
                        if ($scope.courierServices.length === 0) {
                            $scope.NoServicesAssign = true;
                        }
                    }
                    else {
                        AppSpinner.hideSpinnerTemplate();
                        $scope.courierServices = '';
                        $scope.CourierServiceWeightLimit = [];

                        QuotationService.GetLogisticWeightLimit(directBookingFindService).then(function (response) {
                            if (response.data.length > 0) {
                                for (i = 0; i < response.data.length; i++) {

                                    $scope.Services = {
                                        CourierName: "",
                                        LogisticType: "",
                                        ServiceType: "",
                                        IsWeightShow: false,
                                        Weight: "",
                                        ImageURL: ""
                                    };

                                    if (response.data[i].CourierName === 'DHL' &&
                                        response.data[i].IsWeightShow === true) {
                                        $scope.Services.CourierName = response.data[i].CourierName;
                                        $scope.Services.LogisticType = response.data[i].LogisticType;
                                        $scope.Services.ServiceType = response.data[i].RateTypeDisplay;
                                        $scope.Services.IsWeightShow = response.data[i].IsWeightShow;
                                        $scope.Services.Weight = response.data[i].Weight;
                                        $scope.Services.ImageURL = config.BUILD_URL + "DHL.png";
                                        $scope.CourierServiceWeightLimit.push($scope.Services);
                                    }
                                    else if (response.data[i].CourierName === 'TNT' &&
                                        response.data[i].IsWeightShow === true) {
                                        $scope.Services.CourierName = response.data[i].CourierName;
                                        $scope.Services.LogisticType = response.data[i].LogisticType;
                                        $scope.Services.ServiceType = response.data[i].RateTypeDisplay;
                                        $scope.Services.IsWeightShow = response.data[i].IsWeightShow;
                                        $scope.Services.Weight = response.data[i].Weight;
                                        $scope.Services.ImageURL = config.BUILD_URL + "TNT.png";
                                        $scope.CourierServiceWeightLimit.push($scope.Services);
                                    }
                                    else if (response.data[i].CourierName === 'UKMail' &&
                                        response.data[i].IsWeightShow === true) {
                                        $scope.Services.CourierName = response.data[i].CourierName;
                                        $scope.Services.LogisticType = response.data[i].LogisticType;
                                        $scope.Services.ServiceType = response.data[i].RateTypeDisplay;
                                        $scope.Services.IsWeightShow = response.data[i].IsWeightShow;
                                        $scope.Services.Weight = response.data[i].Weight;
                                        $scope.Services.ImageURL = config.BUILD_URL + "UKmail.png";
                                        $scope.CourierServiceWeightLimit.push($scope.Services);
                                    }
                                    else if (response.data[i].CourierName === 'Yodel' &&
                                        response.data[i].IsWeightShow === true) {
                                        $scope.Services.CourierName = response.data[i].CourierName;
                                        $scope.Services.LogisticType = response.data[i].LogisticType;
                                        $scope.Services.ServiceType = response.data[i].RateTypeDisplay;
                                        $scope.Services.IsWeightShow = response.data[i].IsWeightShow;
                                        $scope.Services.Weight = response.data[i].Weight;
                                        $scope.Services.ImageURL = config.BUILD_URL + "yodel.png";
                                        $scope.CourierServiceWeightLimit.push($scope.Services);
                                    }
                                    else if (response.data[i].CourierName === 'Hermes' &&
                                        response.data[i].IsWeightShow === true) {
                                        $scope.Services.CourierName = response.data[i].CourierName;
                                        $scope.Services.LogisticType = response.data[i].LogisticType;
                                        $scope.Services.ServiceType = response.data[i].RateTypeDisplay;
                                        $scope.Services.IsWeightShow = response.data[i].IsWeightShow;
                                        $scope.Services.Weight = response.data[i].Weight;
                                        $scope.Services.ImageURL = config.BUILD_URL + "hermes.png";
                                        $scope.CourierServiceWeightLimit.push($scope.Services);
                                    }
                                    else if (response.data[i].CourierName === 'UPS' &&
                                        response.data[i].IsWeightShow === true) {
                                        $scope.Services.CourierName = response.data[i].CourierName;
                                        $scope.Services.LogisticType = response.data[i].LogisticType;
                                        $scope.Services.ServiceType = response.data[i].RateTypeDisplay;
                                        $scope.Services.IsWeightShow = response.data[i].IsWeightShow;
                                        $scope.Services.Weight = response.data[i].Weight;
                                        $scope.Services.ImageURL = config.BUILD_URL + "UPS.png";
                                        $scope.CourierServiceWeightLimit.push($scope.Services);
                                    }
                                }
                            }
                            else {
                                AppSpinner.hideSpinnerTemplate();
                                $scope.NoServicesAssign = true;
                            }
                        });
                    }
                });
            }
            else {
                AppSpinner.hideSpinnerTemplate();
                $scope.NoServicesAssign = true;
            }
        });
    };

    function init() {
        if (IsRateShow !== undefined) {
            $scope.IsRateShow = IsRateShow;
        }
        else {
            $scope.IsRateShow = true;
        }
        var userInfo = SessionService.getUser();
        $scope.RoleId = userInfo.RoleId;

        $rootScope.GetRateValue = null;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        //new setMultilingualOptions();
        setMultilingualOptions();

        if (userInfo.OperationZoneId === 1) {
            if ($rootScope.SITECOMPANY === 'MAXLOGISTIC') {
                $scope.FrayteWebsite = UtilityService.getPublicSiteName();
                $scope.SalesFrayteWeb = 'sales@mexlogistcs.com';
            }
            else {
                $scope.FrayteWebsite = UtilityService.getPublicSiteName();
                $scope.SalesFrayteWeb = 'sales@frayte.com';
            }

        }
        else if (userInfo.OperationZoneId === 2) {
            if ($rootScope.SITECOMPANY === 'MAXLOGISTIC') {
                $scope.FrayteWebsite = UtilityService.getPublicSiteName();
                $scope.SalesFrayteWeb = 'sales@mexlogistcs.co.uk';
            }
            else {
                $scope.FrayteWebsite = UtilityService.getPublicSiteName();
                $scope.SalesFrayteWeb = 'sales@frayte.co.uk';
            }
        }
    }

    init();
});