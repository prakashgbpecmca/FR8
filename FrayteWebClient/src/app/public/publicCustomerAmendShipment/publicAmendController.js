/** 
 * Controller
 */
angular.module('ngApp.public').controller('PublicAmendController', function ($scope, $state,$uibModal, $stateParams, $timeout, $translate, config, ShipmentService, SessionService, toaster) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'FrayteSuccess', 'CustomInfoCheckTerms', 'PleaseSelectionDeliveryOption', 'PleaseSelectTermAndConditions', 'PleaseCorrectValidationErrors', 'ErrorSavingShipment', 'PleaseSelectValidFile', 'UploadedSuccessfully', 'ErrorOccuredDuringUpload', 'ErrorGettingShipmentDetailServer']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TitleFrayteSuccess = translations.FrayteSuccess;

            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextPleaseSelectionDeliveryOption = translations.PleaseSelectionDeliveryOption;

            $scope.TextErrorSavingShipment = translations.ErrorSavingShipment;
            $scope.TextCustomInfoCheckTerms = translations.CustomInfoCheckTerms;
            $scope.TextPleaseSelectTermAndConditions = translations.PleaseSelectTermAndConditions;

            $scope.TextPleaseSelectValidFile = translations.PleaseSelectValidFile;
            $scope.TextUploadedSuccessfully = translations.UploadedSuccessfully;
            $scope.TextErrorOccuredDuringUpload = translations.ErrorOccuredDuringUpload;
            $scope.TextErrorGettingShipmentDetailServer = translations.ErrorGettingShipmentDetailServer;
        });
    };

    $scope.newAmendShipment = function () {
        $scope.frayteAmendShipment = {
            ShipmentId: 0,
            CustomerId: 0,
            CustomerAccountNumber: null,
            Receiver: null,
            ReceiverAddress: null,
            DeliveredBy: null,
            UserRoleId: null,
            IsLogin: null,
            FrayteShipmentPortOfArrival: null
        };
    };
    $scope.isLogin = false;

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
    $scope.SetCountryPhoneCode = function (userType, country) {
        if (country === null) {
            return;
        }
        else {
            $scope.frayteAmendShipment.FrayteShipmentPortOfArrival = null;
            if (country !== undefined && country !== null && country.CountryId > 0) {
                if (country.TimeZoneDetail != null) {
                    $scope.frayteAmendShipment.Receiver.Timezone = country.TimeZoneDetail;
                }

                if (country.Code !== null && country.Code !== '' && country.Code !== undefined) {

                    if (userType === 'Shipper') {
                        if (country.Code === "HKG") {
                            $scope.setShipperStateDisable = true;
                            $scope.setShipperZipDisable = true;
                            $scope.shipment.ShipperAddress.Zip = null;
                            $scope.shipment.ShipperAddress.State = null;
                        }
                        else if (country.Code === "GBR") {
                            $scope.setShipperStateDisable = true;
                            $scope.setShipperZipDisable = false;
                            $scope.shipment.ShipperAddress.State = null;
                        }
                        else {
                            $scope.setShipperZipDisable = false;
                            $scope.setShipperStateDisable = false;
                        }

                        // Set Shipper timeZone Based on Country
                        $scope.shipment.Shipper.Timezone = country.TimeZoneDetail;
                    }
                    else if (userType === 'Receiver') {
                        if (country.Code === "HKG") {
                            $scope.setReceiverStateDisable = true;
                            $scope.setReceiverZipDisable = true;
                            $scope.shipment.ReceiverAddress.Zip = null;
                            $scope.shipment.ReceiverAddress.State = null;
                        }
                        else if (country.Code === "GBR") {
                            $scope.setReceiverStateDisable = true;
                            $scope.setReceiverZipDisable = false;
                            $scope.shipment.ReceiverAddress.State = null;
                        }
                        else {
                            $scope.setReceiverZipDisable = false;
                            $scope.setReceiverStateDisable = false;
                        }

                        // Set Reciver timeZone Based on Country
                        $scope.shipment.Receiver.Timezone = country.TimeZoneDetail;
                    }
                    var objects = $scope.countryPhoneCodes;
                    for (var i = 0; i < objects.length; i++) {
                        if (objects[i].Name.indexOf($scope.frayteAmendShipment.ReceiverAddress.Country.Name) > -1) {
                            $scope.frayteAmendShipment.Receiver.TelephoneCode = objects[i];
                            $scope.frayteAmendShipment.Receiver.MobileCode = objects[i];
                            break;
                        }
                    }
                }
            }

        }
    };

    $scope.SaveAmmendShipment = function (IsVallid, frayteAmendShipment) {
        if (IsVallid && frayteAmendShipment !== undefined && frayteAmendShipment !== null) {
            // Set Receiver Telephonecode 
            if ($scope.frayteAmendShipment.Receiver.TelephoneCode !== undefined && $scope.frayteAmendShipment.Receiver.TelephoneCode !== null) {
                if (frayteAmendShipment.Receiver.TelephoneNo !== undefined && frayteAmendShipment.Receiver.TelephoneNo !== '') {
                    var str = frayteAmendShipment.Receiver.TelephoneNo;
                    var t = frayteAmendShipment.Receiver.TelephoneNo.substr(str.indexOf('('), str.indexOf(')') + 1);
                    var data = str.replace(t, "");
                    frayteAmendShipment.Receiver.TelephoneNo = '(+' + $scope.frayteAmendShipment.Receiver.TelephoneCode.PhoneCode + ') ' + data.trim();
                }
                if (frayteAmendShipment.Receiver.MobileNo !== undefined && frayteAmendShipment.Receiver.MobileNo !== '') {
                    var str1 = frayteAmendShipment.Receiver.MobileNo;
                    var t1 = frayteAmendShipment.Receiver.MobileNo.substr(str1.indexOf('('), str1.indexOf(')') + 1);
                    var data1 = str1.replace(t1, "");
                    frayteAmendShipment.Receiver.MobileNo = '(+' + $scope.frayteAmendShipment.Receiver.MobileCode.PhoneCode + ') ' + data1.trim();
                }
            }

            ShipmentService.SaveAmmendShipment(frayteAmendShipment).then(function (response) {
                if (response.status == 200) {

                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'shipment/shipmentCreatedSuccessfully.tpl.html',
                        controller: 'SuccessController',
                        windowClass: '',
                        size: 'lg',
                        resolve: {
                            shipmentChild: function () {
                                return $scope.shipment;
                            },
                            userType: function () {
                                return $scope.frayteAmendShipment.userRoleId;
                            },
                            pdfPath: function () {
                                return response.data;
                            }
                        }
                    });

                    modalInstance.result.then(function () {
                    },
                    function () {
                        if ($scope.isLogin) {
                            $state.go('shipper.current-shipment');
                            //$state.go('shipper.current-shipment', {}, { reload: true });
                        }
                        else {
                            $state.go('home.welcome');
                        }
                    });


                    ///
                    //toaster.pop({
                    //    type: 'success',
                    //    title: $scope.TitleFrayteSuccess,
                    //    body: 'Successfully Amended the Shipemnt',
                    //    showCloseButton: true
                    //});

                    //if ($stateParams.shipmentId === undefined || $stateParams.shipmentId === null) {
                    //    $scope.$dismiss('cancel');
                    //}
                    //else {
                    //    //Redirect to mail page after 4 second.
                    //    $timeout(function () {
                    //        $state.go('home.welcome');
                    //    }, 4000);
                    //}
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorSavingShipment,
                        showCloseButton: true
                    });
                }
            }, function () {

                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorSavingShipment,
                    showCloseButton: true
                });

            });
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

    $scope.SetShipperReceiverState = function (Code, stateZip) {
        if (Code !== null && Code !== '' && Code !== undefined) {
            if (Code === "HKG" && (stateZip === 'zip' || stateZip === 'state')) {
                return false;
            }
            else if (Code === "GBR" && stateZip === 'state') {
                return false;
            }
            else {
                return true;
            }
        }
        else {
            return true;
        }
    };
    var getShipmentInitials = function () {
        ShipmentService.GetInitials().then(function (response) {
            $scope.countriesStore = response.data.Countries;
            $scope.countryPhoneCodes = response.data.CountryPhoneCodes;
            $scope.ports = response.data.ShipmentPorts;
            $scope.countries = [];
            // Set Country Order as per Client Requirement
            var CountryData = $scope.countriesStore;
            for (var p = 0; p < CountryData.length; p++) {
                if (CountryData[p].Code === "CHN") {
                    $scope.countries.push({
                        CountryId: CountryData[p].CountryId,
                        Name: CountryData[p].Name,
                        Code: CountryData[p].Code,
                        Code2: CountryData[p].Code2,
                        TimeZoneDetail: CountryData[p].TimeZoneDetail,
                        OrderNumber: 1
                    });
                    CountryData.splice(p, 1);
                    break;
                }

            }
            for (var b = 0; b < CountryData.length; b++) {
                if (CountryData[b].Code === "HKG") {
                    $scope.countries.push({
                        CountryId: CountryData[b].CountryId,
                        Name: CountryData[b].Name,
                        Code: CountryData[b].Code,
                        Code2: CountryData[b].Code2,
                        TimeZoneDetail: CountryData[b].TimeZoneDetail,
                        OrderNumber: 2
                    });
                    CountryData.splice(b, 1);
                    break;
                }

            }
            for (var c = 0; c < CountryData.length; c++) {
                if (CountryData[c].Code === "IND") {
                    $scope.countries.push({
                        CountryId: CountryData[c].CountryId,
                        Name: CountryData[c].Name,
                        Code: CountryData[c].Code,
                        Code2: CountryData[c].Code2,
                        TimeZoneDetail: CountryData[c].TimeZoneDetail,
                        OrderNumber: 3
                    });
                    CountryData.splice(c, 1);
                    break;
                }

            }
            for (var d = 0; d < CountryData.length; d++) {
                if (CountryData[d].Code === "AUS") {
                    $scope.countries.push({
                        CountryId: CountryData[d].CountryId,
                        Name: CountryData[d].Name,
                        Code: CountryData[d].Code,
                        Code2: CountryData[d].Code2,
                        TimeZoneDetail: CountryData[d].TimeZoneDetail,
                        OrderNumber: 4
                    });
                    CountryData.splice(d, 1);
                    break;
                }

            }
            for (var e = 0; e < CountryData.length; e++) {
                if (CountryData[e].Code === "GBR") {
                    $scope.countries.push({
                        CountryId: CountryData[e].CountryId,
                        Name: CountryData[e].Name,
                        Code: CountryData[e].Code,
                        Code2: CountryData[e].Code2,
                        TimeZoneDetail: CountryData[e].TimeZoneDetail,
                        OrderNumber: 5
                    });
                    CountryData.splice(e, 1);
                    break;
                }

            }
            for (var f = 0; f < CountryData.length; f++) {
                if (CountryData[f].Code === "CAN") {
                    $scope.countries.push({
                        CountryId: CountryData[f].CountryId,
                        Name: CountryData[f].Name,
                        Code: CountryData[f].Code,
                        Code2: CountryData[f].Code2,
                        TimeZoneDetail: CountryData[f].TimeZoneDetail,
                        OrderNumber: 6
                    });
                    CountryData.splice(f, 1);
                    break;
                }

            }
            var z = 7;
            for (var h = 0; h < CountryData.length; h++) {
                $scope.countries.push({
                    CountryId: CountryData[h].CountryId,
                    Name: CountryData[h].Name,
                    Code: CountryData[h].Code,
                    Code2: CountryData[h].Code2,
                    TimeZoneDetail: CountryData[h].TimeZoneDetail,
                    OrderNumber: z
                });
                z++;
            }
            // Set Shipping Method 
            $scope.couriers = [];
            var shipingMethods = response.data.Couriers;
            for (var j = 0; j < shipingMethods.length; j++) {
                $scope.couriers.push({
                    CourierId: shipingMethods[j].CourierId,
                    CourierType: shipingMethods[j].CourierType,
                    LatestBookingTime: shipingMethods[j].LatestBookingTime,
                    Name: shipingMethods[j].Name,
                    TransitTime: shipingMethods[j].TransitTime,
                    Website: shipingMethods[j].Website,
                    orderNumber: j
                });
            }
        }, function () {

        });
    };
    var getShipmentDetail = function (id) {

        ShipmentService.GetShipmentDetail(id).then(function (response) {
            if (response.data !== null) {
                $scope.shipment = response.data;

                if(response.data.DeliveredBy !== null){

                    if (response.data.DeliveredBy.CourierType === "Air" || response.data.DeliveredBy.CourierType === "Sea") {


                    }
                    else if (response.data.DeliveredBy.CourierType === "Courier") {

                    }

                }

                $scope.frayteAmendShipment.ShipmentId = response.data.ShipmentId;
                $scope.frayteAmendShipment.CustomerId = response.data.CustomerId;
                $scope.frayteAmendShipment.CustomerAccountNumber = response.data.CustomerAccountNumber;
                $scope.frayteAmendShipment.Receiver = response.data.Receiver;
                $scope.frayteAmendShipment.ReceiverAddress = response.data.ReceiverAddress;
                $scope.frayteAmendShipment.DeliveredBy = response.data.DeliveredBy;
                $scope.frayteAmendShipment.IsLogin = response.data.IsLogin;
                $scope.frayteAmendShipment.FrayteShipmentPortOfArrival = response.data.FrayteShipmentPortOfArrival;

                if ($scope.frayteAmendShipment && $scope.frayteAmendShipment.ReceiverAddress != null && $scope.shipment.ReceiverAddress.Country.Code === "HKG") {
                    $scope.setReceiverStateDisable = true;
                    $scope.setReceiverZipDisable = true;
                    $scope.frayteAmendShipment.ReceiverAddress.Zip = null;
                    $scope.frayteAmendShipment.ReceiverAddress.State = null;
                }
                if ($scope.frayteAmendShipment && $scope.shipment.ReceiverAddress != null && $scope.shipment.ReceiverAddress.Country.Code === "GBR") {
                    $scope.setReceiverStateDisable = true;
                    $scope.frayteAmendShipment.ReceiverAddress.State = null;
                }
                $scope.frayteAmendShipment.FrayteShipmentPortOfArrival = response.data.FrayteShipmentPortOfArrival;
                $scope.frayteAmendShipment.DeliveredBy = response.data.DeliveredBy;
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
            if (response.data.FrayteCargoWiseSo !== null && response.data.FrayteCargoWiseSo !== '') {
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
        $scope.newAmendShipment();
        if ($stateParams.userRoleId !== undefined && $stateParams.userRoleId !== null && $stateParams.userRoleId !== '') {
            $scope.frayteAmendShipment.userRoleId = $stateParams.userRoleId;
        }
        // set Multilingual Modal Popup Options
        setModalOptions();

        getShipmentInitials();
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
            var id = $stateParams.shipmentId;

            if (!$scope.isLogin) {

                getShipmentDetail(id);
            }
        }
    }

    init();

});