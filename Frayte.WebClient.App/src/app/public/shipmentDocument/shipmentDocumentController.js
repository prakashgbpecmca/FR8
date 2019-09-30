/** 
 * Controller
 */
angular.module('ngApp.public').controller('ShipmentDocumentController', function ($scope, $state, $translate, $stateParams, $location, config, $filter, SessionService, ShipmentService, Upload, $timeout, toaster) {

    var setModalOptions = function () {
        $translate(['FrayteSuccess', 'FrayteError', 'FrayteValidation', 'ErrorGetting', 'PleaseCorrectValidationErrors', 'PleaseSelectValidFile', 'ErrorOccuredDuringUpload', 'UploadedSuccessfully', 'PleaseAttachDocument', 'agent', 'records']).then(function (translations) {

            $scope.TitleFrayteSuccess = translations.FrayteSuccess;
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteValidation = translations.FrayteValidation;

            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextPleaseSelectValidFile = translations.PleaseSelectValidFile;
            $scope.TextPleaseAttachDocument = translations.PleaseAttachDocument;
            $scope.TextUploadedSuccessfully = translations.UploadedSuccessfully;
            $scope.TextErrorOccuredDuringUpload = translations.ErrorOccuredDuringUpload;
            $scope.TextErrorGettingAgentRecord = translations.ErrorGetting + " " + translations.agent + " " + translations.records;

        });
    };

    $scope.isLogin = false;

    $scope.uploadDocument = {
        ShipmentId: 0,
        CommercialInvoiceFileName: '',
        CommercialInvoice: '',
        PackingListFileName: '',
        PackingList: '',
        CustomDocFileName: '',
        CustomDoc: ''
    };

    $scope.whileAddingDoc = function ($files, $file, $event, type) {

        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextPleaseSelectValidFile,
                showCloseButton: true
            });
            return;
        }

        if (type === 1) {
            $scope.uploadDocument.CommercialInvoiceFileName = $file.name;
            $scope.uploadDocument.CommercialInvoice = $file.name;
        }
        else if (type === 2) {
            $scope.uploadDocument.PackingListFileName = $file.name;
            $scope.uploadDocument.PackingList = $file.name;
        }
        else if (type === 3) {
            $scope.uploadDocument.CustomDocFileName = $file.name;
            $scope.uploadDocument.CustomDoc = $file.name;
        }
    };

    $scope.upload = function (isValid, commercialInvoiceFile, packingListFile, customDocFile) {
        if ($scope.uploadDocument.CommercialInvoice === '' &&
            $scope.uploadDocument.PackingList === '' &&
            $scope.uploadDocument.CustomDoc === '') {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextPleaseAttachDocument,
                showCloseButton: true
            });
            return;
        }

        if (isValid) {
            var files = [];
            if (commercialInvoiceFile !== undefined && commercialInvoiceFile !== null) {
                files.push(commercialInvoiceFile);
            }
            if (packingListFile !== undefined && packingListFile !== null) {
                files.push(packingListFile);
            }
            if (customDocFile !== undefined && customDocFile !== null) {
                files.push(customDocFile);
            }
            $scope.upload = Upload.upload({
                url: config.SERVICE_URL + '/Shipment/UploadCommercialInoiceDocuments',
                fields: {
                    ShipmentId: $scope.uploadDocument.ShipmentId,
                    CommercialInvoiceFileName: $scope.uploadDocument.CommercialInvoiceFileName,
                    CommercialInvoice: $scope.uploadDocument.CommercialInvoice,
                    PackingListFileName: $scope.uploadDocument.PackingListFileName,
                    PackingList: $scope.uploadDocument.PackingList,
                    CustomDocFileName: $scope.uploadDocument.CustomDocFileName,
                    CustomDoc: $scope.uploadDocument.CustomDoc
                },
                file: files
            });

            $scope.upload.progress($scope.progress);

            $scope.upload.success($scope.success);

            $scope.upload.error($scope.error);
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextValidation,
                showCloseButton: true
            });
        }

    };

    $scope.progress = function (evt) {
        //console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
        //toaster.clear();
        //toaster.pop({
        //    type: 'success',
        //    title: 'uploading',
        //    body: 'percent: ' + parseInt(100.0 * evt.loaded / evt.total, 10),
        //    showCloseButton: true
        //});
    };

    $scope.success = function (data, status, headers, config) {
        if (status == 200) {
            toaster.pop({
                type: 'success',
                title: $scope.TitleFrayteSuccess,
                body: $scope.TextUploadedSuccessfully,
                showCloseButton: true
            });

            if ($stateParams.shipmentId === undefined || $stateParams.shipmentId === null) {
                $scope.$dismiss('cancel');
            }
            else {
                //Redirect to mail page after 4 second.
                $timeout(function () {
                    $state.go('home.welcome');
                }, 4000);
            }
        }
    };

    $scope.error = function (err) {
        toaster.pop({
            type: 'error',
            title: $scope.TitleFrayteError,
            body: $scope.TextErrorOccuredDuringUpload,
            showCloseButton: true
        });
    };

    // Shipment Deatil Section
    $scope.getTotalPieces = function () {
        if ($scope.shipment === undefined) {
            return 0;
        }
        else if ($scope.shipment.ShipmentDetails === undefined) {
            return 0;
        }
        else if ($scope.shipment.ShipmentDetails.length >= 1) {
            var total = parseInt(0, 10);
            for (var i = 0; i < $scope.shipment.ShipmentDetails.length; i++) {
                var product = $scope.shipment.ShipmentDetails[i];
                if (product.Pieces === null || product.Pieces === undefined) {
                    total += parseInt(0, 10);
                }
                else {
                    total = total + parseInt(product.Pieces, 10);
                }
            }
            return total;
        }
        else {
            return 0;
        }
    };

    $scope.getTotalKgs = function () {
        if ($scope.shipment === undefined) {
            return 0;
        }
        else if ($scope.shipment.ShipmentDetails === undefined) {
            return 0;
        }
        else if ($scope.shipment.ShipmentDetails.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.shipment.ShipmentDetails.length; i++) {
                var product = $scope.shipment.ShipmentDetails[i];
                if (product.WeightKg === null || product.WeightKg === undefined) {
                    total += parseFloat(0);
                }
                else {
                    total = total + parseFloat(product.WeightKg) * product.CartonQty;
                }
            }
            return Math.ceil(total);
        }
        else {
            return 0;
        }
    };

    $scope.getTotalCBM = function () {
        if ($scope.shipment === undefined) {
            return;
        }

        var shippingMethod = $scope.shipment.DeliveredBy;
        if ($scope.shipment === undefined) {
            return 0;
        }
        else if ($scope.shipment.ShipmentDetails === undefined) {
            return 0;
        }
        else if ($scope.shipment.ShipmentDetails.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.shipment.ShipmentDetails.length; i++) {
                var product = $scope.shipment.ShipmentDetails[i];
                var len = 0;
                var wid = 0;
                var height = 0;
                if (product.Lcms === null || product.Lcms === undefined) {
                    len = parseFloat(0);
                }
                else {
                    len = parseFloat(product.Lcms);
                }

                if (product.Wcms === null || product.Wcms === undefined) {
                    wid = parseFloat(0);
                }
                else {
                    wid = parseFloat(product.Wcms);
                }

                if (product.Hcms === null || product.Hcms === undefined) {
                    height = parseFloat(0);
                }
                else {
                    height = parseFloat(product.Hcms);
                }

                if (len > 0 && wid > 0 && height > 0) {
                    if (shippingMethod.CourierType === "Sea") {
                        if ($scope.shipment.PiecesCaculatonType === "kgToCms") {
                            total += ((len / 100) * (wid / 100) * (height / 100)) * product.CartonQty;
                        }
                        else if ($scope.shipment.PiecesCaculatonType === "lbToInchs") {
                            total += ((len / 39.37) * (wid / 39.37) * (height / 39.37)) * product.CartonQty;
                        }

                    }
                }
            }
            return total.toFixed(2);
        }
        else {
            return 0;
        }
    };

    $scope.getEstimatedChargeableCBM = function (totalCBM) {
        if (totalCBM === undefined && totalCBM === '' && totalCBM === null) {
            return 0;
        }
        if (totalCBM <= 2.5) {
            return 2.5;
        }
        else {
            var num = [];
            num = totalCBM.toString().split('.');
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
    };

    $scope.getTotalVolWeight = function () {
        if ($scope.shipment === undefined) {
            return;
        }

        var shippingMethod = $scope.shipment.DeliveredBy;
        if ($scope.shipment === undefined) {
            return 0;
        }
        else if ($scope.shipment.ShipmentDetails === undefined) {
            return 0;
        }
        else if ($scope.shipment.ShipmentDetails.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.shipment.ShipmentDetails.length; i++) {
                var product = $scope.shipment.ShipmentDetails[i];
                var len = 0;
                var wid = 0;
                var height = 0;
                if (product.Lcms === null || product.Lcms === undefined) {
                    len = parseFloat(0);
                }
                else {
                    len = parseFloat(product.Lcms);
                }

                if (product.Wcms === null || product.Wcms === undefined) {
                    wid = parseFloat(0);
                }
                else {
                    wid = parseFloat(product.Wcms);
                }

                if (product.Hcms === null || product.Hcms === undefined) {
                    height = parseFloat(0);
                }
                else {
                    height = parseFloat(product.Hcms);
                }

                if (len > 0 && wid > 0 && height > 0) {
                    if (shippingMethod.CourierType === "Air") {
                        total += ((len * wid * height) / 6000) * product.CartonQty;

                    }
                    else if (shippingMethod.CourierType === "Sea") {
                        total += ((len * wid * height) / 1000000) * product.CartonQty;

                    }
                    else if (shippingMethod.CourierType === "Courier") {
                        total += ((len * wid * height) / 5000) * product.CartonQty;

                    }

                }
            }
            return Math.ceil(total);
        }
        else {
            return 0;
        }
    };
    //$scope.pieceDetailOption 
    $scope.changeKgToLb = function (pieceDetailOption) {
        if (pieceDetailOption === "kgToCms") {
            // for kg
            $translate('kGS').then(function (kGS) {
                $scope.Lb_Kgs = kGS;
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
            $translate('Inchs').then(function (Inchs) {
                $scope.Lb_Inch = Inchs;
            });
        }
    };

    var getShipmentDetail = function (id) {

        ShipmentService.GetShipmentDetail(id).then(function (response) {
            if (response.data !== null) {
                $scope.shipment = response.data;
                $scope.changeKgToLb($scope.shipment.PiecesCaculatonType);
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGettingAgentRecord,
                showCloseButton: true
            });
        });

        ShipmentService.GetShipmentShipperReceiverDetail(id).then(function (response) {
            if (response.data.FrayteCargoWiseSo !== null &&response.data.FrayteCargoWiseSo !== ''){
                $scope.FrayteCargoWiseSo = response.data.FrayteCargoWiseSo;
            }
            else {
                $scope.FrayteCargoWiseSo = id;
            }
            $scope.PurchaseOrderNo = response.data.PurchaseOrderNo;
            $scope.ShipperInfo = response.data.Shipper;
            $scope.ShipperAddressInfo = response.data.ShipperAddress;
            $scope.ReceiverInfo = response.data.Receiver;
            $scope.ReceiverAddressInfo = response.data.ReceiverAddress;

        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGettingAgentRecord,
                showCloseButton: true
            });
        });

    };
    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();

        var userInfo = SessionService.getUser();
        if (userInfo === undefined || userInfo === null || userInfo.SessionId === undefined || userInfo.SessionId === '') {
            $scope.isLogin = false;
        }
        else {
            $scope.isLogin = true;
        }
        if ($stateParams.shipmentId === undefined || $stateParams.shipmentId === null) {
            $scope.uploadDocument.ShipmentId = $scope.params.shipmentId;
        }
        else {
            $scope.uploadDocument.ShipmentId = $stateParams.shipmentId;
            var id = $stateParams.shipmentId;
            if (!$scope.isLogin) {
                getShipmentDetail(id);
            }
        }
    }

    init();

});