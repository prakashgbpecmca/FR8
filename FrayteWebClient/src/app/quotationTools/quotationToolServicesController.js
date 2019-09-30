angular.module('ngApp.quotationTools').controller('QuotationToolServicesController', function ($scope, config, toaster, AppSpinner, uiGridConstants, $translate, DirectBookingService, ModalService, $uibModalInstance, directBookingObj, $rootScope, CustomerId, SessionService, $http, $window) {


   

    var SetMultilingualOptions = function () {
        $translate(['ServiceUnavailable_Validation', 'ServiceNotAvailable_Validation', 'FrayteWarning', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation', 'ErrorSavingRecord',
            'FrayteError', 'FrayteSuccess', 'Could_Not_SelectService', 'Report_GeneratedDownloaded_Successfully', 'Error_DownloadingPDF',
        'Nosupplymentry_charges_Courier']).then(function (translations) {
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
            return Math.ceil(total);
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

        if ($scope.courierServices !== null && $scope.courierServices !== undefined && $scope.courierServices.length) {
            var obj = {};
            angular.forEach($scope.courierServices, function (eachObj) {
                if (eachObj.RowSelect) {
                         obj = eachObj;
                }
            });
            if(obj.RowSelect){
                $scope.setCustomerRateCard(obj);
            }
           
        }
        else {
            $uibModalInstance.dismiss();
        }
        $rootScope.GetRateValue = false;
    };

    $scope.setCustomerRateCard = function (RateCard) {
        if (RateCard !== undefined && RateCard !== null) {
            if ($scope.quotationDetail.QuotationPackages.length === 1 && ($scope.quotationDetail.QuotationPackages[0].CartoonValue === 1 || $scope.quotationDetail.QuotationPackages[0].CartoonValue === '1')) {
                RateCard.Length = $scope.quotationDetail.QuotationPackages[0].Length;
                RateCard.Width = $scope.quotationDetail.QuotationPackages[0].Width;
                RateCard.Height = $scope.quotationDetail.QuotationPackages[0].Height;
                RateCard.Weight = $scope.getChargeableWeight();
                RateCard.PakageCalculatonType = $scope.quotationDetail.PakageCalculatonType;
                DirectBookingService.GetAdditionalSurcharge(RateCard).then(function (response) {
                    if (response.data !== null) {
                        response.data.ImageURL = RateCard.ImageURL;
                        response.data.ShipmentType = RateCard.ShipmentType;
                        $uibModalInstance.close(response.data);
                    }
                }, function () {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.Frayte_Warning,
                        body: $scope.Could_NotSelectService,
                        showCloseButton: true
                    });
                });
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
            OtherServiceList: []

        };
        $scope.noUkShipmentService = [];
        if ($scope.courierServices !== null && $scope.courierServices.length > 0) {

                
            
            angular.forEach($scope.courierServices, function (eachObj) {
             
                eachObj.RowSelect = false;
            });
            $scope.courierServices.sort(function (a, b) { return a.Rate - b.Rate; });
            $scope.courierServices[0].RowSelect = true;

            for (var i = 0; i < $scope.courierServices.length; i++) {
                if ($scope.courierServices[i].LogisticType === 'UKShipment' && $scope.courierServices[i].CourierName === 'UKMail') {
                    $scope.ukShipmentService.IsUkShipment = true;

                    $scope.ukShipmentService.CourierType = $scope.courierServices[i].CourierName;
               //     $scope.courierServices[i].RowSelect = false;
                    $scope.courierServices[i].ShipmentType = $scope.courierServices[i].WeightType + " ( " + $scope.courierServices[i].ParcelServiceType + " ) ";
                    $scope.courierServices[i].ImageURL = config.BUILD_URL + "UKmail.png";
                    $scope.ukShipmentService.UkmailServiceList.push($scope.courierServices[i]);
                }
                else if ($scope.courierServices[i].LogisticType === 'UKShipment' && $scope.courierServices[i].CourierName === "Yodel") {
                    $scope.ukShipmentService.IsUkShipment = true;
                    $scope.ukShipmentService.CourierType = $scope.courierServices[i].CourierName;
               //     $scope.courierServices[i].RowSelect = false;
                    $scope.courierServices[i].ShipmentType = $scope.courierServices[i].WeightType;
                    $scope.courierServices[i].ImageURL = config.BUILD_URL + "yodel.png";
                    $scope.ukShipmentService.YodelServiceList.push($scope.courierServices[i]);
                }
                else if ($scope.courierServices[i].LogisticType === 'UKShipment' && $scope.courierServices[i].CourierName === "Hermes") {
                    $scope.ukShipmentService.IsUkShipment = true;
                    $scope.ukShipmentService.CourierType = $scope.courierServices[i].CourierName;
              //      $scope.courierServices[i].RowSelect = false;
                    $scope.courierServices[i].ShipmentType = $scope.courierServices[i].PakageType + " ( " + $scope.courierServices[i].ParcelServiceType + " ) ";
                    $scope.courierServices[i].ImageURL = config.BUILD_URL + "hermes.png";
                    $scope.ukShipmentService.HermesServiceList.push($scope.courierServices[i]);
                }
                else {
                    if ($scope.courierServices[i].CourierName === "DHL") {
                        $scope.courierServices[i].ImageURL = config.BUILD_URL + "DHL.png";
                    }
                    else if ($scope.courierServices[i].CourierName === "FedEx") {
                        $scope.courierServices[i].ImageURL = config.BUILD_URL + "Fedex.png";
                    }
                    else if ($scope.courierServices[i].CourierName === "TNT") {
                        $scope.courierServices[i].ImageURL = config.BUILD_URL + "TNT.png";
                    }
                    $scope.ukShipmentService.IsUkShipment = false;
                    $scope.ukShipmentService.CourierType = $scope.courierServices[i].CourierName;
                   // $scope.courierServices[i].RowSelect = false;
                    $scope.ukShipmentService.OtherServiceList.push($scope.courierServices[i]);
                }

            }

            if ($scope.ukShipmentService.UkmailServiceList.length) {
                $scope.ukShipmentService.UkmailServiceList.sort(function (b, a) { return b.TotalEstimatedCharge - a.TotalEstimatedCharge; });
                $scope.ukShipmentService.FirstUkmailService = $scope.ukShipmentService.UkmailServiceList[0];
                $scope.ukShipmentService.UkmailServiceList.splice(0, 1);
            }
            if ($scope.ukShipmentService.YodelServiceList.length) {
                $scope.ukShipmentService.YodelServiceList.sort(function (b, a) { return b.TotalEstimatedCharge - a.TotalEstimatedCharge; });
                $scope.ukShipmentService.FirstYodelService = $scope.ukShipmentService.YodelServiceList[0];
                $scope.ukShipmentService.YodelServiceList.splice(0, 1);
            }
            if ($scope.ukShipmentService.HermesServiceList.length) {
                $scope.ukShipmentService.HermesServiceList.sort(function (b, a) { return b.TotalEstimatedCharge - a.TotalEstimatedCharge; });
                $scope.ukShipmentService.FirstHermesService = $scope.ukShipmentService.HermesServiceList[0];
                $scope.ukShipmentService.HermesServiceList.splice(0, 1);
            }



        }
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
    $scope.DownloadSupplementryChargesGetServices = function (GetServiceDetail) {
        $rootScope.GetRateValue = null;
        if (GetServiceDetail) {
           
            AppSpinner.showSpinnerTemplate("Downloading Supplementary Charges PDF", $scope.Template);
            var Rate = {
                UserId: $scope.CustId,
                LogisticServiceId: GetServiceDetail.LogisticServiceId,
                LogisticCompany: GetServiceDetail.CourierName,
                LogisticType: GetServiceDetail.LogisticType,
                RateType: GetServiceDetail.RateType,
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
                    //



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
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.PleaseCorrect_ValidationErrors,
                showCloseButton: true
            });
        }
    };

    function init() {

        var userInfo = SessionService.getUser();
        $scope.RoleId = userInfo.RoleId;

        if (userInfo.OperationZoneId === 1)
        {
            $scope.FrayteWebsite = 'frayte.com';
        }
        else if (userInfo.OperationZoneId === 2) {
            $scope.FrayteWebsite = 'frayte.co.uk';
        }
        //AppSpinner.showSpinner();
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        AppSpinner.showSpinnerTemplate(" ", $scope.Template);
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
            Currency: ''
        };

        //Ste DirectBookingFindService details
        directBookingFindService.FromCountry = directBookingObj.QuotationFromAddress.Country;
        directBookingFindService.FromPostCode = directBookingObj.QuotationFromAddress.PostCode;
        directBookingFindService.ToCountry = directBookingObj.QuotationToAddress.Country;
        directBookingFindService.ToPostCode = directBookingObj.QuotationToAddress.PostCode;
        directBookingFindService.Weight = weight;
        directBookingFindService.OperationZoneId = directBookingObj.OpearionZoneId;

        if (totalPieces === 1) {
            directBookingFindService.PackageType = 'Single';
        }
        else if (totalPieces > 1) {
            directBookingFindService.PackageType = 'Multiple';
        }

        if (directBookingFindService.FromCountry.Code !==  directBookingFindService.ToCountry.Code ) {
            if (directBookingObj.ParcelType !== null && directBookingObj.ParcelType.ParcelDescription === "Parcel (Non Doc)") {
                directBookingFindService.DocType = 'Nondoc';
            }
            else if (directBookingObj.ParcelType !== null && directBookingObj.ParcelType.ParcelDescription === "Letter (Doc)") {
                directBookingFindService.DocType = 'Doc';
            }
        }
        else {
            directBookingFindService.DocType = '';
        }
       

        //$scope.TestCheck = directBookingFindService;

        DirectBookingService.GetServices(directBookingFindService).then(function (response) {
            //AppSpinner.hideSpinner();
            AppSpinner.hideSpinnerTemplate();
            if (response.status === 200) {
                for (i = 0; i < response.data.length; i++) {
                    //response.data[i].AdditionalSurcharge = response.data[i].AdditionalSurcharge.toFixed(2);
                    //response.data[i].FuelSurcharge = response.data[i].FuelSurcharge.toFixed(2);
                    //response.data[i].Rate = response.data[i].Rate.toFixed(2);
                    //response.data[i].TotalEstimatedCharge = response.data[i].TotalEstimatedCharge.toFixed(2);
                    response.data[i].AdditionalSurcharge = Number(parseFloat(response.data[i].AdditionalSurcharge).toFixed(2)).toLocaleString('en', { minimumFractionDigits: 2 });
                    response.data[i].FuelSurcharge = response.data[i].FuelSurcharge.toFixed(2);
                    response.data[i].Rate = Number(parseFloat(response.data[i].Rate).toFixed(2)).toLocaleString('en', { minimumFractionDigits: 2 });
                    response.data[i].TotalEstimatedCharge = Number(parseFloat(response.data[i].TotalEstimatedCharge).toFixed(2)).toLocaleString('en', { minimumFractionDigits: 2 });
                }
                $scope.courierServices = response.data;
                $scope.showHideUKmail = false;
                $scope.showHideYodel = false;
                $scope.showHideHermes = false;
                getServicesJson();
            }
            else {
                //  AppSpinner.hideSpinner();
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'warning',
                    title: $scope.Frayte_Warning,
                    body: $scope.ServiceNotAvailableValidation,
                    showCloseButton: true
                });
            }


        });

        new SetMultilingualOptions();
    }

    init();

});
