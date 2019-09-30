angular.module('ngApp.eCommerce').controller('uploadeCommerceWithServiceController', function ($uibModalStack, PostCodeService, $uibModalInstance, UtilityService, HSCodeService, $window, eCommerceBookingService, Upload, $rootScope, AppSpinner, $sce, TopCountryService, $location, $anchorScroll, TopCurrencyService, $scope, config, $filter, $state, ModalService, $translate, $uibModal, $timeout, toaster, $stateParams, DirectBookingService, SessionService, ShipmentId, UploadShipmentService, TrackingNo, CourierCompany) {

    // Post Code Search

    $scope.onSelectPostCode = function ($item, $model, $label, $event, PostCode, Type) {
        if (PostCode && Type) {
            if (Type === "ShipFrom") {
                $scope.eCommerceBooking.ShipFrom.PostCode = $item.PostCode;
                $scope.eCommerceBooking.ShipFrom.Address = $item.Address1;
                $scope.eCommerceBooking.ShipFrom.Address2 = $item.Address2;
                $scope.eCommerceBooking.ShipFrom.Area = $item.Area;
                $scope.eCommerceBooking.ShipFrom.City = $item.City;
                $scope.eCommerceBooking.ShipFrom.CompanyName = $item.CompanyName;
            }
            else if (Type === "ShipTo") {
                $scope.eCommerceBooking.ShipTo.PostCode = $item.PostCode;
                $scope.eCommerceBooking.ShipTo.Address = $item.Address1;
                $scope.eCommerceBooking.ShipTo.Address2 = $item.Address2;
                $scope.eCommerceBooking.ShipTo.Area = $item.Area;
                $scope.eCommerceBooking.ShipTo.City = $item.City;
                $scope.eCommerceBooking.ShipTo.CompanyName = $item.CompanyName;
            }
        }
    };
    $scope.GetPostCodeAddress = function (PostCode, CountryCode2) {
        if (PostCode && CountryCode2) {
            return PostCodeService.AllPostCode(PostCode, CountryCode2).then(function (response) {
                $scope.PostCodeAddressValue = false;
                if (response) {
                    $scope.fillPostlValues = response;
                }
                else {
                    $scope.PostCodeAddressValue = true;
                }
                return response;
            }, function () {
                $scope.PostCodeAddressValue = true;
            });
        }
    };


    $scope.openCalender = function ($event) {
        $scope.status.opened = true;
    };

    $scope.openCalender1 = function ($event) {
        $scope.status1.opened = true;
    };

    $scope.status1 = {
        opened: false
    };

    $scope.status = {
        opened: false
    };

    //$scope.changeKgToLb = function (pieceDetailOption) {
    //    if (pieceDetailOption === "kgToCms") {
    //        // for kg
    //        $translate('kGS').then(function (kGS) {
    //            $scope.Lb_Kgs = kGS;
    //        });
    //        $translate('KG').then(function (KG) {

    //            $scope.Lb_Kg = KG;
    //        });
    //        $translate('CMS').then(function (CMS) {
    //            $scope.Lb_Inch = CMS;
    //        });
    //    }
    //    if (pieceDetailOption === "lbToInchs") {
    //        // for LB
    //        $translate('LB').then(function (LB) {
    //            $scope.Lb_Kgs = LB;
    //        });
    //        $translate('LBs').then(function (LBs) {

    //            $scope.Lb_Kg = LBs;
    //        });
    //        $translate('INCHS').then(function (Inchs) {
    //            $scope.Lb_Inch = Inchs;
    //        });
    //    }
    //};

    $scope.AddPackage = function () {
        //if ($scope.directBooking.Packages.length === 1) {
        //    $scope.NewDirectBooking();
        //}
        $scope.HideContent = true;
        $scope.eCommerceBooking.Packages.push({
            DirectShipmentDetailDraftId: 0,
            Length: null,
            Width: null,
            Height: null,
            Weight: null,
            Value: null,
            Content: '',
            LabelName: '',
            TrackingNo: ''
        });
        var dbpac = $scope.eCommerceBooking.Packages.length - 1;
        for (i = 0; i < $scope.eCommerceBooking.Packages.length; i++) {

            if (i === dbpac) {
                $scope.eCommerceBooking.Packages[i].pacVal = true;
            }
            else {
                $scope.eCommerceBooking.Packages[i].pacVal = false;
            }
        }
    };
    $scope.RemovePackage = function (Package) {
        if (Package !== undefined && Package !== null) {
            var index = $scope.eCommerceBooking.Packages.indexOf(Package);
            if ($scope.eCommerceBooking.Packages.length === 2) {
                $scope.HideContent = false;
            }
            if (Package.DirectShipmentDetailId > 0) {
                DirectBookingService.DeleteDirectShipmentPackage(Package.DirectShipmentDetailId).then(function (response) {
                    if (response.data.Status) {
                        toaster.pop({
                            type: 'success',
                            title: $scope.TitleFrayteSuccess,
                            body: $scope.RemovePackageValidation,
                            showCloseButton: true
                        });
                        $scope.eCommerceBooking.Packages.splice(index, 1);
                        $scope.Packges = angular.copy($scope.directBooking.Packages);
                        $scope.eCommerceBooking.Packages = [];
                        $scope.eCommerceBooking.Packages = $scope.Packges;
                    }

                }, function () {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.RemovePackageValidation,
                        showCloseButton: true
                    });
                });
            }
            else {
                if (index === $scope.eCommerceBooking.Packages.length - 1) {
                    var dbpac = $scope.eCommerceBooking.Packages.length - 2;
                    for (i = 0; i < $scope.eCommerceBooking.Packages.length; i++) {

                        if (i === dbpac) {
                            $scope.eCommerceBooking.Packages[i].pacVal = true;
                        }
                        else {
                            $scope.eCommerceBooking.Packages[i].pacVal = false;
                        }
                    }
                }
                else {
                    var dbpac1 = $scope.eCommerceBooking.Packages.length - 1;
                    for (i = 0; i < $scope.eCommerceBooking.Packages.length; i++) {

                        if (i === dbpac1) {
                            $scope.eCommerceBooking.Packages[i].pacVal = true;
                        }
                        else {
                            $scope.eCommerceBooking.Packages[i].pacVal = false;
                        }
                    }
                }
                $scope.eCommerceBooking.Packages.splice(index, 1);
                $scope.Packges = angular.copy($scope.eCommerceBooking.Packages);
                $scope.eCommerceBooking.Packages = [];
                $scope.eCommerceBooking.Packages = $scope.Packges;
            }
        }
    };

    //
    $scope.Removecomma = function (Content) {
        var Description = [];
        var DESCString = '';
        if (Content !== null && Content !== undefined && Content !== "") {
            Description = Content.split('');
            for (i = 0; i < Description.length; i++) {
                if (Description[i] === ',') {
                    Description.splice(i, 1);
                }
                else {
                    //return Content;
                }
            }
            for (ii = 0; ii < Description.length; ii++) {
                DESCString = DESCString + Description[ii];
            }
            return DESCString;
        }
    };

    $scope.$watch('eCommerceBooking.ReferenceDetail.Reference1', function (newValue, oldValue) {
        if (newValue && newValue.length > 25) {
            if ($scope.eCommerceBooking !== undefined && $scope.eCommerceBooking.ReferenceDetail !== null) {
                $scope.eCommerceBooking.ReferenceDetail.Reference1 = oldValue;
            }
        }
    });

    function stopEnterKey(evt) {
        var evt1 = (evt) ? evt : ((event) ? event : null);
        var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
        if ((evt1.keyCode == 13) && (node.type == "text" || node.type == "checkbox" ||
                     node.type == "radio" || node.type == "email" || node.type == "number" || node.type == "textarea")) {
            return false;
        }
    }
    document.onkeypress = stopEnterKey;

    // Uploads and Download excel for Pieces grid
    $scope.WhileAddingExcel = function ($files, $file, $event) {
        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextPleaseSelectValidFile,
                showCloseButton: true
            });
            return;
        }
        // download excel
        $scope.downloadExcel = function () {
            ShipmentService.DownLoadSampleFile().then(function (data) {
            });
        };
        // Upload the excel file here.
        $scope.uploadExcel = Upload.upload({
            url: config.SERVICE_URL + '/DirectBooking/GetPiecesDetailFromExcel',
            file: $file

        });

        $scope.uploadExcel.progress($scope.progressExcel);

        $scope.uploadExcel.success($scope.successExcel);

        $scope.uploadExcel.error($scope.errorExcel);
    };

    $scope.progressExcel = function (evt) {
        //To Do:  show excel uploading progress message 

    };

    $scope.successExcel = function (data, status, headers, config) {
        if (status = 200) {
            if (data.Message == "Ok" || data.Message == "ok" || data.Message == "OK") {
                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteSuccess,
                    body: $scope.Package_Information_Addedd_Successfully,
                    showCloseButton: true
                });

                //  To Do: Logic to add row in Piecees grid.
                var result = data;

                if (result.FrayteShipmentDetail !== null && result.FrayteShipmentDetail.length) {
                    for (var i = 0 ; i < result.FrayteShipmentDetail.length; i++) {
                        $scope.eCommerceBooking.Packages.push(result.FrayteShipmentDetail[i]);
                    }

                }
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarningValidation,
                    body: data.Message,
                    showCloseButton: true
                });
            }


        }
        else {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError_Validation,
                body: $scpe.PackageShipment_Validation,
                showCloseButton: true
            });
        }

    };

    $scope.errorExcel = function (err) {
        toaster.pop({
            type: 'error',
            title: $scope.TitleFrayteError,
            body: $scpe.PackageShipment_Validation,
            showCloseButton: true
        });
    };

    // Uploads and Download excel for Pieces grid

    $scope.setFocusCustomTab = function (DirectBookingForm) {
        if (DirectBookingForm !== undefined) {
            if (customInfoTabValid(DirectBookingForm)) {
                $scope.blinkCustomInfo = false;
            }
            return $scope.blinkCustomInfo;
        }

    };
    $scope.totalPieces = function (directBooking) {
        if (directBooking !== undefined && directBooking !== null && directBooking.Packages !== null && directBooking.Packages.length) {
            var sum = 0;
            for (var i = 0; i < directBooking.Packages.length; i++) {
                if (directBooking.Packages[i].CartoonValue !== "" && directBooking.Packages[i].CartoonValue !== null && directBooking.Packages[i].CartoonValue !== undefined) {
                    sum += Math.abs(parseInt(directBooking.Packages[i].CartoonValue, 10));
                }
            }
            return sum;
        }
        else {
            return 0;
        }
    };
    $scope.GoToState = function () {
        $rootScope.State1 = false;
        $state.go('customer.direct-shipments');
    };
    var setScroll = function (id) {
        $location.hash(id);
        $anchorScroll();
    };
    var shipFromTabValid = function (DirectBookingForm) {
        if (DirectBookingForm !== undefined && $scope.eCommerceBooking !== undefined && $scope.eCommerceBooking.ShipFrom !== undefined && $scope.eCommerceBooking.ShipFrom.Country !== undefined) {
            var flag = false;
            if (DirectBookingForm.shipFromAddress1 !== undefined && DirectBookingForm.shipFromAddress1.$valid &&
                DirectBookingForm.shipFromCompanyName !== undefined && DirectBookingForm.shipFromCompanyName.$valid &&
                DirectBookingForm.shipFromFirstName !== undefined && DirectBookingForm.shipFromFirstName.$valid &&
                DirectBookingForm.shipFromLastName !== undefined && DirectBookingForm.shipFromLastName.$valid &&
                DirectBookingForm.shipFromCountry !== undefined && DirectBookingForm.shipFromCountry.$valid &&
                $scope.eCommerceBooking.ShipFrom.Country.CountryId > 0 &&
                DirectBookingForm.shipFromPostcode !== undefined && DirectBookingForm.shipFromPostcode.$valid &&
                DirectBookingForm.shipFromState !== undefined && DirectBookingForm.shipFromState.$valid &&
                DirectBookingForm.shipFromCityName !== undefined && DirectBookingForm.shipFromCityName.$valid &&
                DirectBookingForm.shipFromPhone !== undefined && DirectBookingForm.shipFromPhone.$valid
                ) {
                flag = true;
            }
            else {
                flag = false;
            }

            return flag;
        }
    };
    var shipToTabValid = function (DirectBookingForm) {
        if (DirectBookingForm !== undefined && $scope.eCommerceBooking !== undefined && $scope.eCommerceBooking.ShipTo !== undefined && $scope.eCommerceBooking.ShipTo.Country !== undefined) {
            var flag = false;
            if (DirectBookingForm.shipToAddress1 !== undefined && DirectBookingForm.shipToAddress1.$valid &&
        DirectBookingForm.shipToCompanyName !== undefined && DirectBookingForm.shipToCompanyName.$valid &&
        DirectBookingForm.shipToFirstName !== undefined && DirectBookingForm.shipToFirstName.$valid &&
        DirectBookingForm.shipToLastName !== undefined && DirectBookingForm.shipToLastName.$valid &&
        DirectBookingForm.shipToCountry !== undefined && DirectBookingForm.shipToCountry.$valid &&
        $scope.eCommerceBooking.ShipTo.Country.CountryId > 0 &&
        DirectBookingForm.shipToPostcode !== undefined && DirectBookingForm.shipToPostcode.$valid &&
        DirectBookingForm.shipToState !== undefined && DirectBookingForm.shipToState.$valid &&
        DirectBookingForm.shipToCityName !== undefined && DirectBookingForm.shipToCityName.$valid &&
        DirectBookingForm.shipToPhone !== undefined && DirectBookingForm.shipToPhone.$valid
        ) {
                flag = true;
            }
            else {
                flag = false;
            }


            return flag;
        }
    };
    var shipmentDetailTabValid = function (DirectBookingForm) {
        if (DirectBookingForm !== undefined && $scope.eCommerceBooking !== undefined) {
            var flag = false;
            if (DirectBookingForm.parcelType !== undefined && DirectBookingForm.parcelType.$valid && $scope.isCollectionTimeValid &&
                DirectBookingForm.shipmentPaymentAccount !== undefined && DirectBookingForm.shipmentPaymentAccount.$valid &&
                DirectBookingForm.paymentPartyCurrencyType !== undefined && DirectBookingForm.paymentPartyCurrencyType.$valid &&
                   DirectBookingForm.paymentPartyCurrencyType !== undefined && DirectBookingForm.paymentPartyCurrencyType.$valid &&
                   DirectBookingForm.reference1 !== undefined && DirectBookingForm.reference1.$valid &&
                //   DirectBookingForm.shipmentCollectionDate !== undefined && DirectBookingForm.shipmentCollectionDate.$valid &&
                // DirectBookingForm.collectiontime !== undefined && DirectBookingForm.collectiontime.$valid &&
                //DirectBookingForm.collectioTimeMinutes !== undefined && DirectBookingForm.collectioTimeMinutes.$valid &&
               DirectBookingForm.paymentPartyCurrencyType !== undefined && DirectBookingForm.paymentPartyCurrencyType.$valid &&
                DirectBookingForm.trackingNo !== undefined && DirectBookingForm.trackingNo.$valid &&
                DirectBookingForm.courierCompany !== undefined && DirectBookingForm.courierCompany.$valid
                ) {
                for (var i = 0 ; i < $scope.eCommerceBooking.Packages.length; i++) {
                    var packageForm = DirectBookingForm['tags' + i];
                    if (packageForm !== undefined && packageForm.$valid) {
                        flag = true;
                    }
                    else {
                        flag = false;
                        break;
                    }
                }
            }
            else {
                flag = false;
            }

            return flag;
        }
    };
    var customInfoTabValid = function (DirectBookingForm) {
        if (DirectBookingForm !== undefined && $scope.eCommerceBooking !== undefined && $scope.eCommerceBooking !== null) {
            var flag = false;

            if (DirectBookingForm.contentsType !== undefined && DirectBookingForm.contentsType.$valid &&
               DirectBookingForm.contentsExplanation !== undefined && DirectBookingForm.contentsExplanation.$valid &&
               DirectBookingForm.restrictionType !== undefined && DirectBookingForm.restrictionType.$valid &&
               DirectBookingForm.restrictionComments !== undefined && DirectBookingForm.restrictionComments.$valid &&
               DirectBookingForm.nonDeliveryOption !== undefined && DirectBookingForm.nonDeliveryOption.$valid &&
               DirectBookingForm.customsSigner !== undefined && DirectBookingForm.customsSigner.$valid &&
               $scope.eCommerceBooking.CustomInfo.CustomsCertify
               ) {
                flag = true;
            }
            else {
                flag = false;
            }
            return flag;
        }
    };
    $scope.shipFromTab = function (DirectBookingForm) {
        if (DirectBookingForm !== undefined) {
            return shipFromTabValid(DirectBookingForm);
        }
    };
    $scope.shipToTab = function (DirectBookingForm) {
        if (DirectBookingForm !== undefined) {
            return shipToTabValid(DirectBookingForm);
        }
    };
    $scope.currencyVisibility = function (directBooking) {
        if (directBooking !== undefined && directBooking !== null &&
          directBooking.ShipFrom !== undefined && directBooking.ShipFrom !== null &&
          directBooking.ShipTo !== undefined && directBooking.ShipTo !== null &&
          directBooking.ShipFrom.Country !== undefined && directBooking.ShipFrom.Country !== null &&
          directBooking.ShipFrom.Country !== undefined && directBooking.ShipTo.Country !== null &&
          directBooking.ShipFrom.Country.Code === 'GBR' && directBooking.ShipTo.Country.Code === 'GBR') {

            return false;
        }
        else {
            return true;
        }
    };
    $scope.setPackageParcelType = function (ParcelType) {
        if (ParcelType !== undefined && ParcelType !== null) {
            var parcel = ParcelType.ParcelDescription;
            if (ParcelType.ParcelDescription === 'Letter (Doc)') {
                if ($scope.eCommerceBooking.Packages !== null && $scope.eCommerceBooking.Packages.length > 1) {
                    var modalOptions = {
                        headerText: $scope.Confirmation,
                        bodyText: $scope.RemoveChangingParcelTypeDoc
                    };

                    ModalService.Confirm({}, modalOptions).then(function (result) {
                        var data = $scope.eCommerceBooking.Packages[0];
                        $scope.eCommerceBooking.Packages = [];
                        $scope.eCommerceBooking.Packages.push(data);
                        $scope.eCommerceBooking.Packages[0].Content = 'Document';
                        $scope.eCommerceBooking.Packages[0].CartoonValue = 1;
                        $scope.eCommerceBooking.Packages[0].Length = 5;
                        $scope.eCommerceBooking.Packages[0].Width = 5;
                        $scope.eCommerceBooking.Packages[0].Height = 5;
                        $scope.eCommerceBooking.Packages[0].Weight = 1;
                        $scope.eCommerceBooking.Packages[0].Value = 5;
                    }, function () {
                        var parcelDetail = $filter('filter')($scope.ParcelTypes, { ParcelDescription: "Parcel (Non Doc)" });
                        if (parcelDetail.length) {
                            $scope.eCommerceBooking.ParcelType = parcelDetail[0];
                        }

                    });

                }
                else {
                    $scope.eCommerceBooking.Packages[0].Content = 'Document';
                    $scope.eCommerceBooking.Packages[0].CartoonValue = 1;
                    $scope.eCommerceBooking.Packages[0].Length = 5;
                    $scope.eCommerceBooking.Packages[0].Width = 5;
                    $scope.eCommerceBooking.Packages[0].Height = 5;
                    $scope.eCommerceBooking.Packages[0].Weight = 1;
                    $scope.eCommerceBooking.Packages[0].Value = 5;
                }
            }
            else {
                if ($scope.eCommerceBooking.Packages !== null && $scope.eCommerceBooking.Packages.length > 0) {
                    $scope.eCommerceBooking.Packages[0].Content = '';
                }
            }
        }
    };
    $scope.shipmentDetailTab = function (DirectBookingForm) {
        if (DirectBookingForm !== undefined) {
            return shipmentDetailTabValid(DirectBookingForm);
        }
    };
    $scope.customInfoTab = function (DirectBookingForm) {
        if (DirectBookingForm !== undefined) {
            return customInfoTabValid(DirectBookingForm);
        }
    };
    $scope.checkDeclaredValue = function (DirectBookingForm, packageForm, value) {
        if (value !== undefined && value !== null && value !== '') {
            var total = parseFloat(value);
            if (total < 0.01) {
                if (packageForm !== undefined) {
                    packageForm.currencyValue.$invalid = true;
                }

                //DirectBookingForm.$valid = false;
            }
        }

    };
    $scope.shipFromOptionalField = function (action) {
        if (action !== undefined && action !== null && action !== '' && action === 'State') {
            if ($scope.eCommerceBooking !== undefined && $scope.eCommerceBooking !== null && $scope.eCommerceBooking.ShipFrom !== null && $scope.eCommerceBooking.ShipFrom.Country !== null) {
                if ($scope.eCommerceBooking.ShipFrom.Country.Code !== 'HKG' && $scope.eCommerceBooking.ShipFrom.Country.Code !== 'GBR') {
                    return true;
                }
                else {
                    $scope.eCommerceBooking.ShipFrom.State = '';
                    return false;
                }
            }
        }
        else if (action !== undefined && action !== null && action !== '' && action === 'PostCode') {
            if ($scope.eCommerceBooking !== undefined && $scope.eCommerceBooking !== null && $scope.eCommerceBooking.ShipFrom !== null && $scope.eCommerceBooking.ShipFrom.Country !== null) {
                if ($scope.eCommerceBooking.ShipFrom.Country.Code !== 'HKG') {
                    return true;
                }
                else {
                    $scope.eCommerceBooking.ShipFrom.PostCode = '';
                    return false;
                }
            }
        }

    };
    $scope.shipToOptionalField = function (action) {
        if (action !== undefined && action !== null && action !== '' && action === 'State') {
            if ($scope.eCommerceBooking !== undefined && $scope.eCommerceBooking !== null && $scope.eCommerceBooking.ShipTo !== null && $scope.eCommerceBooking.ShipTo.Country !== null) {
                if ($scope.eCommerceBooking.ShipTo.Country.Code !== 'HKG' && $scope.eCommerceBooking.ShipTo.Country.Code !== 'GBR') {

                    return true;
                }
                else {
                    $scope.eCommerceBooking.ShipTo.State = '';
                    return false;
                }
            }
        }
        else if (action !== undefined && action !== null && action !== '' && action === 'PostCode') {
            if ($scope.eCommerceBooking !== undefined && $scope.eCommerceBooking !== null && $scope.eCommerceBooking.ShipTo !== null && $scope.eCommerceBooking.ShipTo.Country !== null) {
                if ($scope.eCommerceBooking.ShipTo.Country.Code !== 'HKG') {

                    return true;
                }
                else {
                    $scope.eCommerceBooking.ShipTo.PostCode = '';
                    return false;
                }
            }
        }

    };
    $scope.getTotalKgs = function () {
        if ($scope.eCommerceBooking === undefined) {
            return;
        }

        else if ($scope.eCommerceBooking.Packages === undefined || $scope.eCommerceBooking.Packages === null) {
            return 0;
        }
        else if ($scope.eCommerceBooking.Packages.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.eCommerceBooking.Packages.length; i++) {
                var product = $scope.eCommerceBooking.Packages[i];
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
        if ($scope.eCommerceBooking === undefined) {
            return;
        }

        else if ($scope.eCommerceBooking.Packages === undefined || $scope.eCommerceBooking.Packages === null) {
            return 0;
        }

        if ($scope.eCommerceBooking.Packages.length >= 0) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.eCommerceBooking.Packages.length; i++) {
                var product = $scope.eCommerceBooking.Packages[i];
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
                    if ($scope.eCommerceBooking.PakageCalculatonType === "kgToCms") {
                        total += ((len * wid * height) / 6000) * qty;
                    }
                    else if ($scope.eCommerceBooking.PakageCalculatonType === "lbToInchs") {
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
    $scope.setCustomContentDescription = function (ContentDescription, CustomType) {
        if (CustomType !== undefined && CustomType === 'ContentExplanation') {
            if (ContentDescription !== undefined && ContentDescription !== null && ContentDescription !== '') {
                if ($scope.ShowCustomInfoSection($scope.eCommerceBooking)) {
                    $scope.eCommerceBooking.CustomInfo.ContentsExplanation = ContentDescription;
                }
            }
        }
        else if (CustomType !== undefined && CustomType === 'RestrictionExplanation') {
            if (ContentDescription !== undefined && ContentDescription !== null && ContentDescription !== '') {
                if ($scope.ShowCustomInfoSection($scope.eCommerceBooking)) {
                    $scope.eCommerceBooking.CustomInfo.RestrictionComments = ContentDescription;
                }
            }
        }

    };
    $scope.showGetServicePlaceBooking = function (DirectBookingForm) {

        if (DirectBookingForm !== undefined && $scope.eCommerceBooking !== undefined && $scope.eCommerceBooking.ShipFrom !== undefined && $scope.eCommerceBooking.ShipFrom.Country !== undefined &&
            $scope.eCommerceBooking.ShipTo !== undefined && $scope.eCommerceBooking.ShipTo.Country !== undefined) {

            var flag = false;
            if (DirectBookingForm.shipFromAddress1 !== undefined && DirectBookingForm.shipFromAddress1.$valid &&
                DirectBookingForm.shipFromCompanyName !== undefined && DirectBookingForm.shipFromCompanyName.$valid &&
                DirectBookingForm.shipFromFirstName !== undefined && DirectBookingForm.shipFromFirstName.$valid &&
                DirectBookingForm.shipFromLastName !== undefined && DirectBookingForm.shipFromLastName.$valid &&
                DirectBookingForm.shipFromCountry !== undefined && DirectBookingForm.shipFromCountry.$valid &&
                $scope.eCommerceBooking.ShipFrom.Country.CountryId > 0 &&
                DirectBookingForm.shipFromPostcode !== undefined && DirectBookingForm.shipFromPostcode.$valid &&
                DirectBookingForm.shipFromState !== undefined && DirectBookingForm.shipFromState.$valid &&
                DirectBookingForm.shipFromCityName !== undefined && DirectBookingForm.shipFromCityName.$valid &&
                DirectBookingForm.shipFromPhone !== undefined && DirectBookingForm.shipFromPhone.$valid
                ) {
                flag = true;
            }
            else {
                flag = false;
            }
            if (flag) {
                if (DirectBookingForm.shipToAddress1 !== undefined && DirectBookingForm.shipToAddress1.$valid &&
            DirectBookingForm.shipToCompanyName !== undefined && DirectBookingForm.shipToCompanyName.$valid &&
            DirectBookingForm.shipToFirstName !== undefined && DirectBookingForm.shipToFirstName.$valid &&
            DirectBookingForm.shipToLastName !== undefined && DirectBookingForm.shipToLastName.$valid &&
            DirectBookingForm.shipToCountry !== undefined && DirectBookingForm.shipToCountry.$valid &&
                     $scope.eCommerceBooking.ShipTo.Country.CountryId > 0 &&
            DirectBookingForm.shipToPostcode !== undefined && DirectBookingForm.shipToPostcode.$valid &&
            DirectBookingForm.shipToState !== undefined && DirectBookingForm.shipToState.$valid &&
            DirectBookingForm.shipToCityName !== undefined && DirectBookingForm.shipToCityName.$valid &&
            DirectBookingForm.shipToPhone !== undefined && DirectBookingForm.shipToPhone.$valid
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
    $scope.NewDirectBooking = function () {
        $scope.eCommerceBooking = {
            OpearionZoneId: 0,
            DirectShipmentDraftId: 0,
            CustomerId: null,
            ShipmentStatusId: 19,
            CreatedBy: $scope.CreatedBy,
            ShipFrom: {
                DirectShipmentAddressDraftId: 0,
                Country: null,
                PostCode: "",
                FirstName: "",
                LastName: "",
                CompanyName: "",
                Address: "",
                Address2: "",
                City: "",
                State: "",
                Area: "",
                Phone: "",
                Email: "",
                IsMailSend: false,
                ModuleType: 'eCommerce'
            },
            ShipTo: {
                DirectShipmentAddressDraftId: 0,
                Country: null,
                PostCode: "",
                FirstName: "",
                LastName: "",
                CompanyName: "",
                Address: "",
                Address2: "",
                City: "",
                State: "",
                Area: "",
                Phone: "",
                Email: "",
                IsMailSend: false,
                ModuleType: 'eCommerce'
            },
            ParcelType: null,
            PayTaxAndDuties: "Receiver",
            PaymentPartyAccountNumber: '',
            Currency: null,
            Packages: [{
                DirectShipmentDetailDraftId: 0,
                DirectShipmentDraftId: 0,
                Length: null,
                Width: null,
                Height: null,
                Weight: null,
                Value: null,
                Content: '',
                LabelName: '',
                TrackingNo: ''
            }],
            ReferenceDetail: {
                Reference1: "",
                Reference2: "",
                SpecialInstruction: "",
                ContentDescription: ""
            },
            CustomInfo: {
                ShipmentCustomDetailId: 0,
                ContentsType: null,
                ContentsExplanation: 'Goods',
                RestrictionType: $scope.RestrictionType[0].value,
                RestrictionComments: '',
                CustomsCertify: false,
                CustomsSigner: '',
                NonDeliveryOption: $scope.NonDeliveryOption[1].value,
                ModuleType: 'eCommerce'
            },
            CustomerRateCard: {
                ZoneRateCardId: 0,
                WeightType: '',
                CourierId: 0,
                LogisticServiceId: 0,
                CourierName: '',
                CourierAccountNo: '',
                CourierDescription: '',
                IntegrationAccountId: 0
            },
            PakageCalculatonType: '',
            TaxAndDutiesAcceptedBy: 'Receiver',
            BookingStatusType: 'Draft',
            ModuleType: 'eCommerce',
            BookingApp: "ECOMMERCE_ONL"
        };
    };
    var setDeliveryTime = function () {

        var h = "16";
        var m = "30";
        return h.toString() + m.toString();

    };
    var setArrivalTime = function () {

        var h = "16";
        var m = "30";
        return h.toString() + m.toString();

    };
    //$scope.checkTime = function (DirectBookingForm) {
    //    if (DirectBookingForm !== undefined && $scope.eCommerceBooking !== undefined && $scope.eCommerceBooking.ReferenceDetail !== undefined) {
    //        var d = new Date();
    //        var min = "";
    //        if (d.getMinutes().toString().length === 1) {
    //            min = "0" + d.getMinutes().toString();
    //        }
    //        else {
    //            min = d.getMinutes().toString();
    //        }
    //        var str = d.getHours().toString() + min;
    //        var colDate = $scope.eCommerceBooking.ReferenceDetail.CollectionDate;
    //        var dateString1 = colDate.getMonth().toString() + "-" + colDate.getDate().toString() + "-" + colDate.getFullYear().toString();
    //        var dateString2 = d.getMonth().toString() + "-" + d.getDate().toString() + "-" + d.getFullYear().toString();

    //        var CollectionDate = new Date(dateString1);
    //        var CurrentDate = new Date(dateString2);
    //        if (CollectionDate < CurrentDate) {
    //            DirectBookingForm.collectiontime.$dirty = true;
    //            $scope.isCollectionTimeValid = false;
    //            return true;
    //        }
    //        else if (+CollectionDate === +CurrentDate) {
    //            if (parseFloat($scope.eCommerceBooking.ReferenceDetail.CollectionTime) < parseFloat(str)) {
    //                DirectBookingForm.collectiontime.$dirty = true;
    //                $scope.isCollectionTimeValid = false;
    //                return true;
    //            }
    //            else {
    //                $scope.isCollectionTimeValid = true;
    //                return false;
    //            }
    //        }
    //        else {
    //            $scope.isCollectionTimeValid = true;
    //            return false;
    //        }
    //    }

    //};

    $scope.checkTime = function (DirectBookingForm) {
        $scope.isCollectionTimeValid = true;
        return true;
    };
    $scope.ValidateForSaving = function () {

        if ($scope.eCommerceBooking.Packages === undefined ||
            $scope.eCommerceBooking.Packages === null ||
            $scope.eCommerceBooking.Packages.length === 0) {

            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.PackageShipment_Validation,
                showCloseButton: true
            });

            return false;
        }
        else {
            return true;
        }
    };
    $scope.PlaceBooking = function (DirectBookingForm, isValid, eCommerceBooking) {

        if ($scope.BookingType !== undefined && $scope.BookingType !== null && $scope.BookingType !== '') {

            if ($scope.BookingType !== "Draft") {
                var isBusinessRulePassed = $scope.ValidateForSaving();
                if (!isBusinessRulePassed) {
                    return;
                }
                var flag = false;

                for (var i = 0 ; i < $scope.eCommerceBooking.Packages.length; i++) {
                    var packageForm = DirectBookingForm['tags' + i];
                    if (packageForm !== undefined && packageForm.$valid) {
                        flag = true;
                    }
                    else {
                        flag = false;
                        break;
                    }
                }
                if (!flag) {
                    $scope.BookingType = "";
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteValidation,
                        body: $scope.TextValidation,
                        showCloseButton: true
                    });
                    return;
                }
            }
            else {
                if (eCommerceBooking.ShipFrom.Country === undefined || eCommerceBooking.ShipFrom.Country === null || eCommerceBooking.ShipTo.Country === undefined || eCommerceBooking.ShipTo.Country === null || eCommerceBooking.CustomerId === undefined || eCommerceBooking.CustomerId === null || eCommerceBooking.CustomerId === 0 || eCommerceBooking.CustomerId === '0') {
                    $scope.BookingType = "";
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteValidation,
                        body: $scope.SaveDraft,
                        showCloseButton: true
                    });
                    return;
                }

            }

            if ($scope.BookingType === "Draft" || (isValid && $scope.isCollectionTimeValid)) {
                eCommerceBooking.DirectShipmentDraftId = ShipmentId;
                eCommerceBooking.ShipmentStatusId = 19;
                eCommerceBooking.Packages[0].TrackingNo = null;

                var flage = true;
                //if ($scope.eCommerceBooking.Packages !== null && $scope.eCommerceBooking.Packages.length > 0 && $scope.BookingType !== "Draft") {

                //    for (var dp = 0 ; dp < $scope.eCommerceBooking.Packages.length; dp++) {
                //        if (!$scope.eCommerceBooking.Packages[dp].IsHSCodeSet) {
                //            flage = false;
                //            break;
                //        }
                //    }

                //}

                if (!flage) {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteValidation,
                        body: $scope.Please_Set_Pieces_HsCode_Properly,
                        showCloseButton: true
                    });
                    return;
                }
                //$scope.spinnerMessage = 'Loading ...';
                if ($scope.BookingType === "Draft") {
                    $rootScope.GetServiceValue = '';
                    AppSpinner.showSpinnerTemplate($scope.SavingInfomation, $scope.Template);
                }
                else {
                    //$rootScope.GetServiceValue = false;
                    AppSpinner.showSpinnerTemplate($scope.UploadingShipments, $scope.Template);
                }

                if ($scope.BookingType === "Draft") {
                    eCommerceBooking.BookingStatusType = "Draft";
                }
                else {
                    eCommerceBooking.BookingStatusType = "Current";
                }
                UploadShipmentService.SaveeCommerceWithServiceUploadBooking(eCommerceBooking).then(function (response) {
                    if (response.status === 200) {
                        AppSpinner.hideSpinner();
                        toaster.pop({
                            type: 'success',
                            title: $scope.FrayteSuccess,
                            body: $scope.Shipment_Created_Successfully,
                            showCloseButton: true
                        });
                        $uibModalInstance.close({ reload: true });
                    }
                    else {
                        AppSpinner.hideSpinner();
                        //$scope.eCommerceBooking.DirectShipmentDraftId = response.data.DirectShipmentDraftId;
                        //$scope.eCommerceBooking.ShipFrom = response.data.ShipFrom;
                        //$scope.eCommerceBooking.ShipTo = response.data.ShipTo;
                        //$scope.eCommerceBooking.CustomInfo = response.data.CustomInfo;
                        //$scope.eCommerceBooking.Packages = response.data.Packages;
                        //angular.forEach($scope.eCommerceBooking.Packages, function (obj) {
                        //    obj.IsHSCodeSet = true;
                        //});
                        //$scope.ErrorTemplate(response.data.Error);
                        //toaster.pop({
                        //    type: 'error',
                        //    title: "Frayte Service Error",
                        //    body: "An error occurred on frayte service side. Please try later",
                        //    showCloseButton: true
                        //});
                        //$scope.BookingType = "";
                    }
                },
                    function () {
                        $scope.BookingType = "";
                        AppSpinner.hideSpinner();
                        toaster.pop({
                            type: 'error',
                            title: $scope.FrayteValidation,
                            body: $scope.ServiceSideError_Validation,
                            showCloseButton: true
                        });
                    });
            }
            else {
                $scope.BookingType = "";
                AppSpinner.hideSpinner();
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteValidation,
                    body: $scope.TextValidation,
                    showCloseButton: true
                });
            }
        }
    };
    $scope.DirectShipmentDetail = function (ShipmentId) {
        // ShipmentService.GetShipmentDetail(row.entity.ShipmentId).then(function (response) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBooking/directBookingDetail/directBookingDetail.tpl.html',
            controller: 'DirectBookingDetailController',
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            resolve: {
                shipmentId: function () {
                    return ShipmentId;
                }
            }
        });

    };
    //Set Multilingual for Modal Popup
    var SetMultilingualOtions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'FrayteSuccess', 'CustomInfoCheckTerms',
            'PleaseSelectionDeliveryOption', 'PleaseSelectTermAndConditions', 'PleaseCorrectValidationErrors',
            'ErrorSavingShipment', 'PleaseSelectValidFile', 'UploadedSuccessfully', 'ErrorOccuredDuringUpload',
            'ErrorGettingShipmentDetailServer', 'SaveDraft_Validation', 'PackageShipment_Validation',
            'SelectShipment_Validation', 'CustomInformation_Validation', 'BookingSave_Validation',
        'ServiceSideError_Validation', 'Success', 'RemovePackage_Validation', 'Validation_Error', 'GetService_Validation',
        'SelectCustomer_Validation', 'SelectCurrency_Validation', 'FrayteWarning_Validation', 'SelectCustomerAddressBook_Validation',
        'FrayteServiceError_Validation', 'ReceiveDetail_Validation', 'InitialData_Validation', 'Shipment_Created_Successfully', 'Please_Set_Pieces_HsCode_Properly',
        'PackageShipment_Validation', 'FrayteError_Validation', 'Package_Information_Addedd_Successfully', 'Confirmation', 'RemoveChangingParcelTypeDoc',
        'UploadingShipments', 'SavingInfomation']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TitleFrayteSuccess = translations.FrayteSuccess;
            $scope.FrayteError_Validation = translations.FrayteError_Validation;

            $scope.Please_Set_Pieces_HsCode_Properly = translations.Please_Set_Pieces_HsCode_Properly;
            $scope.PackageShipment_Validation = translations.PackageShipment_Validation;
            $scope.Package_Information_Addedd_Successfully = translations.Package_Information_Addedd_Successfully;

            $scope.Shipment_Created_Successfully = translations.Shipment_Created_Successfully;
            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextPleaseSelectionDeliveryOption = translations.PleaseSelectionDeliveryOption;

            $scope.TextErrorSavingShipment = translations.ErrorSavingShipment;
            $scope.TextCustomInfoCheckTerms = translations.CustomInfoCheckTerms;
            $scope.TextPleaseSelectTermAndConditions = translations.PleaseSelectTermAndConditions;

            $scope.TextPleaseSelectValidFile = translations.PleaseSelectValidFile;
            $scope.TextUploadedSuccessfully = translations.UploadedSuccessfully;
            $scope.TextErrorOccuredDuringUpload = translations.ErrorOccuredDuringUpload;
            $scope.TextErrorGettingShipmentDetailServer = translations.ErrorGettingShipmentDetailServer;

            $scope.SaveDraft = translations.SaveDraft_Validation;
            $scope.PackageShipment_Validation = translations.PackageShipment_Validation;
            $scope.PleaseSelectShipment = translations.SelectShipment_Validation;
            $scope.CustomInformationValidation = translations.CustomInformation_Validation;
            $scope.BookingSaveValidation = translations.BookingSave_Validation;
            $scope.ServiceSideError_Validation = translations.ServiceSideError_Validation;
            $scope.Success = translations.Success;
            $scope.RemovePackageValidation = translations.RemovePackage_Validation;
            $scope.ValidationError = translations.Validation_Error;
            $scope.GetServiceValidation = translations.GetService_Validation;
            $scope.SelectCustomerValidation = translations.SelectCustomer_Validation;
            $scope.SelectCurrencyValidation = translations.SelectCurrency_Validation;
            $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
            $scope.SelectCustomerAddressBookValidation = translations.SelectCustomerAddressBook_Validation;
            $scope.ReceiveDetailValidation = translations.FrayteServiceError_Validation;
            $scope.FrayteServiceErrorValidation = translations.ReceiveDetail_Validation;
            $scope.InitialDataValidation = translations.InitialData_Validation;
            $scope.Confirmation = translations.Confirmation;
            $scope.RemoveChangingParcelTypeDoc = translations.RemoveChangingParcelTypeDoc;
            $scope.UploadingShipments = translations.UploadingShipments;
            $scope.SavingInfomation = translations.SavingInfomation;
        });
    };
    //$scope.AddPackage = function () {
    //    $scope.eCommerceBooking.Packages.push({
    //        DirectShipmentDetailDraftId: 0,
    //        Length: null,
    //        Width: null,
    //        Height: null,
    //        Weight: null,
    //        Value: null,
    //        Content: '',
    //        LabelName: '',
    //        TrackingNo: ''
    //    });
    //};
    //$scope.RemovePackage = function (Package) {
    //    if (Package !== undefined && Package !== null) {
    //        var index = $scope.eCommerceBooking.Packages.indexOf(Package);
    //        if (Package.DirectShipmentDetailDraftId > 0) {
    //            eCommerceBookingService.DeleteeCommerceShipmentPackage(Package.DirectShipmentDetailDraftId).then(function (response) {
    //                if (response.data.Status) {
    //                    toaster.pop({
    //                        type: 'success',
    //                        title: $scope.Success,
    //                        body: $scope.RemovePackageValidation,
    //                        showCloseButton: true
    //                    });
    //                    $scope.eCommerceBooking.Packages.splice(index, 1);
    //                    $scope.Packges = angular.copy($scope.eCommerceBooking.Packages);
    //                    $scope.eCommerceBooking.Packages = [];
    //                    $scope.eCommerceBooking.Packages = $scope.Packges;
    //                }

    //            }, function () {
    //                toaster.pop({
    //                    type: 'error',
    //                    title: "Error",
    //                    body: $scope.RemovePackageValidation,
    //                    showCloseButton: true
    //                });
    //            });
    //        }
    //        else {
    //            $scope.eCommerceBooking.Packages.splice(index, 1);
    //            $scope.Packges = angular.copy($scope.eCommerceBooking.Packages);
    //            $scope.eCommerceBooking.Packages = [];
    //            $scope.eCommerceBooking.Packages = $scope.Packges;
    //        }
    //    }
    //};
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
    $scope.setParcelTypeDisable = function () {
        if ($scope.eCommerceBooking !== undefined && $scope.eCommerceBooking.Packages !== null && $scope.eCommerceBooking.Packages.length > 0) {
            var total = 0;
            for (var i = 0; i < $scope.eCommerceBooking.Packages.length; i++) {
                total += parseFloat($scope.eCommerceBooking.Packages[i].Weight);
            }

            if (total > 2.0) {
                $scope.eCommerceBooking.ParcelType = $scope.ParcelTypes[0];
                return true;
            }
            else {
                return false;
            }
        }
    };
    //$scope.OpenCalender = function ($event) {
    //    $scope.status.opened = true;
    //};
    $scope.SetCurrencyDisable = function (directBooking) {
        if (directBooking !== undefined && directBooking !== null &&
           directBooking.ShipFrom !== undefined && directBooking.ShipFrom !== null &&
           directBooking.ShipTo !== undefined && directBooking.ShipTo !== null &&
           directBooking.ShipFrom.Country !== undefined && directBooking.ShipFrom.Country !== null &&
           directBooking.ShipFrom.Country !== undefined && directBooking.ShipTo.Country !== null &&
           directBooking.ShipFrom.Country.Code === directBooking.ShipTo.Country.Code) {
            if (directBooking.ShipFrom.Country.Code === "GBR" && directBooking.ShipTo.Country.Code === "GBR") {
                for (var i = 0 ; i < $scope.CurrencyTypes.length; i++) {
                    if ($scope.CurrencyTypes[i].CurrencyCode == "GBP") {
                        directBooking.Currency = $scope.CurrencyTypes[i];
                        break;
                    }
                }
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    };
    // Disable input controls in Custom information panel
    $scope.disableSigner = function (value) {

        $scope.eCommerceBooking.CustomInfo.CustomsSigner = '';
        if (value) {
            $scope.disableCustomerSigner = false;
        }
        else {
            $scope.disableCustomerSigner = true;
        }
    };

    $scope.disableRestriction = function (RestrictionType) {
        $scope.eCommerceBooking.CustomInfo.RestrictionComments = '';
        if (RestrictionType === "other") {
            $scope.disableRestrictionComment = false;

        }
        else if (RestrictionType === "none") {
            $scope.eCommerceBooking.CustomInfo.RestrictionComments = 'N/A';
            $scope.disableRestrictionComment = false;
        }
        else {
            $scope.disableRestrictionComment = true;
        }
    };

    $scope.disableContent = function (ContentType) {
        $scope.eCommerceBooking.CustomInfo.ContentsExplanation = '';
        if (ContentType === "other") {
            $scope.disableContentExplanation = false;

        }
        else {
            $scope.disableContentExplanation = true;
        }

    };

    $scope.editGetServices = function (DirectBookingForm) {
        $scope.GetServices($scope.eCommerceBooking, DirectBookingForm);
    };
    $scope.GetServices = function (directBooking, DirectBookingForm) {
        if (directBooking !== undefined) {
            $rootScope.GetServiceValue = true;
        }
        if (directBooking !== undefined && DirectBookingForm !== undefined &&
           directBooking !== null &&
           directBooking.ShipFrom !== undefined &&
           directBooking.ShipFrom !== null &&
           directBooking.ShipTo !== undefined &&
           directBooking.ShipTo !== null &&
           directBooking.ShipFrom.Country !== null &&
           directBooking.ShipTo.Country !== null &&
           directBooking.Packages !== undefined &&
           directBooking.Packages !== null) {

            var weightTotal = $scope.PackagesTotal(directBooking.Packages, 'Weight');
            weightTotal = parseFloat(weightTotal);
            var flag = false;
            for (var i = 0 ; i < $scope.eCommerceBooking.Packages.length; i++) {
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
                toaster.pop({
                    type: 'warning',
                    title: $scope.ValidationError,
                    body: $scope.GetServiceValidation,
                    showCloseButton: true
                });

                return;
            }
            if (directBooking.CustomerId === null || directBooking.CustomerId === 0) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.ValidationError,
                    body: $scope.SelectCustomerValidation,
                    showCloseButton: true
                });

                return;
            }
            if (directBooking.Currency === null || directBooking.Currency === undefined) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.ValidationError,
                    body: $scope.SelectCurrencyValidation,
                    showCloseButton: true
                });

                return;
            }

            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'directBooking/directBookingServices/directBookingGetService.tpl.html',
                controller: 'DirectBookingServicesController',
                windowClass: 'DirectBookingService',
                size: 'lg',
                backdrop: 'static',
                resolve: {
                    directBookingObj: function () {
                        return directBooking;
                    },
                    IsRateShow: function () {
                        return $scope.IsRateShow;
                    }
                }
            });

            modalInstance.result.then(function (customerRateCard) {
                if (customerRateCard !== undefined && customerRateCard !== null) {
                    $scope.eCommerceBooking.CustomerRateCard = customerRateCard;
                    if ($scope.eCommerceBooking.CustomerRateCard.LogisticServiceId > 0) {
                        for (var aa = 0; aa < $scope.ShipmentMethods.length; aa++) {
                            //if ($scope.eCommerceBooking.CustomerRateCard.CourierId === $scope.ShipmentMethods[aa].ShipmentMethodId) {
                            //    $scope.eCommerceBooking.ShipmentMethod = $scope.ShipmentMethods[aa];
                            //    $scope.disableCourier = true;
                            //    break;
                            //}
                        }
                        if ($scope.ShowCustomInfoSection($scope.eCommerceBooking) && $scope.eCommerceBooking.CustomerRateCard.LogisticServiceId > 0) {
                            $scope.active = 2;
                        }
                    }

                    setScroll("top");
                }

            }, function () {

            });

        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.ValidationError,
                body: $scope.GetServiceValidation,
                showCloseButton: true
            });
        }


    };

    $scope.cancelServices = function () {
        $scope.active = 1;
        $scope.eCommerceBooking.CustomerRateCard = {
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


    $scope.SetCustomerAddress = function () {
        if ($scope.CustomerDetail !== null) {
            $scope.eCommerceBooking.ShipFrom.Country = $scope.CustomerDetail.Country;
            $scope.eCommerceBooking.ShipFrom.PostCode = $scope.CustomerDetail.PostCode;
            $scope.eCommerceBooking.ShipFrom.FirstName = $scope.CustomerDetail.FirstName;
            $scope.eCommerceBooking.ShipFrom.CompanyName = $scope.CustomerDetail.CompanyName;
            $scope.eCommerceBooking.ShipFrom.Address = $scope.CustomerDetail.Address;
            $scope.eCommerceBooking.ShipFrom.Address2 = $scope.CustomerDetail.Address2;
            $scope.eCommerceBooking.ShipFrom.City = $scope.CustomerDetail.City;
            $scope.eCommerceBooking.ShipFrom.Area = $scope.CustomerDetail.Suburb;
            $scope.eCommerceBooking.ShipFrom.State = $scope.CustomerDetail.State;
            $scope.eCommerceBooking.ShipFrom.Phone = $scope.CustomerDetail.Phone;
            $scope.eCommerceBooking.ShipFrom.Email = $scope.CustomerDetail.Email;
        }
    };

    $scope.ClearForm = function (form) {
        form.$setPristine();
        form.$setUntouched();
        $scope.NewDirectBooking();
        $scope.CustomerDetail = null;
        if ($scope.RoleId === 3) {
            $scope.eCommerceBooking.CustomerId = $scope.customerId;
        }
        if ($scope.ShipToPhoneCode !== undefined && $scope.ShipToPhoneCode !== null && $scope.ShipToPhoneCode !== '') {
            $scope.ShipToPhoneCode = null;
        }
    };

    $scope.ShowCustomInfoSection = function (directBooking) {
        if (directBooking !== undefined && directBooking !== null &&
            directBooking.ShipFrom !== undefined && directBooking.ShipFrom !== null &&
            directBooking.ShipTo !== undefined && directBooking.ShipTo !== null &&
            directBooking.ShipFrom.Country !== undefined && directBooking.ShipFrom.Country !== null &&
            directBooking.ShipFrom.Country !== undefined && directBooking.ShipTo.Country !== null &&
            directBooking.ShipFrom.Country.Code !== directBooking.ShipTo.Country.Code) {

            return true;
        }
        else {
            return false;
        }
    };

    $scope.SetCustomInfoSection = function (shipmentMethod) {

        if (shipmentMethod !== undefined && shipmentMethod !== null && shipmentMethod.ShipmentMethodName === 'UK/EU - Shipment') {
            return true;
        }
        else {
            return false;
        }
    };

    // Set Country Phone Code
    $scope.SetShipFrominfo = function (Country) {
        if (Country !== undefined && Country !== null) {
            for (var i = 0 ; i < $scope.CountryPhoneCodes.length ; i++) {
                if ($scope.CountryPhoneCodes[i].CountryCode === Country.Code) {
                    $scope.ShipFromPhoneCode = "(+" + $scope.CountryPhoneCodes[i].PhoneCode + ")";
                    break;
                }
            }

            setShipFromStatePostCodeForHKGUK(Country);

        }
    };

    var setShipFromStatePostCodeForHKGUK = function (Country) {
        if (Country.Code === 'HKG') {
            $scope.eCommerceBooking.ShipFrom.PostCode = null;
            $scope.eCommerceBooking.ShipFrom.State = null;
        }
        else if (Country.Code === 'GBR') {
            $scope.eCommerceBooking.ShipFrom.State = null;
        }
    };
    var setShipToStatePostCodeForHKGUK = function (Country) {
        if (Country.Code === 'HKG') {
            $scope.eCommerceBooking.ShipTo.PostCode = null;
            $scope.eCommerceBooking.ShipTo.State = null;
        }
        else if (Country.Code === 'GBR') {
            $scope.eCommerceBooking.ShipTo.State = null;
        }
    };
    // Set ShipTo info
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
    $scope.directBookingAddEdit = function (row) {
        if ($scope.eCommerceBooking !== null && ($scope.eCommerceBooking.CustomerId === undefined || $scope.eCommerceBooking.CustomerId === null || $scope.eCommerceBooking.CustomerId === 0 || $scope.eCommerceBooking.CustomerId === "0" || $scope.customerId === "")) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarningValidation,
                body: $scope.SelectCustomerAddressBookValidation,
                showCloseButton: true
            });
            return;
        }
        $scope.customerId = $scope.eCommerceBooking.CustomerId;
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBooking/directBookingAddEdit.tpl.html',
            controller: 'DirectBookingAddEditController',
            windowClass: 'DirectBooking-Modal',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                moduleType: function () {
                    return "eCommerce";
                },
                toCountryId: function () {
                    if ($scope.eCommerceBooking.ShipTo != null && $scope.eCommerceBooking.ShipTo.Country !== null && $scope.eCommerceBooking.ShipTo.Country.CountryId) {
                        return $scope.eCommerceBooking.ShipTo.Country.CountryId;
                    }
                    else {
                        return 0;
                    }
                },
                addressType: function () {
                    return "FromAddress";
                },
                customerId: function () {
                    return $scope.customerId;
                },
                Countries: function () {
                    return $scope.CountriesRepo;
                },
                CountryPhoneCodes: function () {
                    return $scope.CountryPhoneCodes;
                }
            }
        });
        modalInstance.result.then(function (addressBooks) {
            if (addressBooks !== undefined && addressBooks !== null) {
                $scope.eCommerceBooking.ShipFrom = addressBooks;
                $scope.eCommerceBooking.ShipFrom.Country = addressBooks.Country;
                $scope.eCommerceBooking.ShipFrom.AddressBookId = 0;


                $scope.SetShipFrominfo($scope.eCommerceBooking.ShipFrom.Country);
            }

        });
    };
    $scope.showTaxAndDutyCondition = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBooking/taxAndDuty.tpl.html',
            controller: 'DirectBookingTaxAndDutyController',
            windowClass: '',
            size: 'md',
            backdrop: 'static'
        });

        modalInstance.result.then(function (taxAndDutyName) {
            if (taxAndDutyName !== undefined) {
                $scope.eCommerceBooking.TaxAndDutiesAcceptedBy = taxAndDutyName;
            }

        }, function () {
            $scope.eCommerceBooking.PayTaxAndDuties = '';
        });
    };
    $scope.directBookingAddEditShipToAddress = function () {
        if ($scope.eCommerceBooking !== null && ($scope.eCommerceBooking.CustomerId === undefined || $scope.eCommerceBooking.CustomerId === null || $scope.eCommerceBooking.CustomerId === 0 || $scope.eCommerceBooking.CustomerId === "0" || $scope.customerId === "")) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarningValidation,
                body: $scope.SelectCustomerAddressBookValidation,
                showCloseButton: true
            });
            return;
        }
        $scope.customerId = $scope.eCommerceBooking.CustomerId;
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBooking/directBookingAddEdit.tpl.html',
            controller: 'DirectBookingAddEditController',
            windowClass: 'DirectBooking-Modal',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                moduleType: function () {
                    return "eCommerce";
                },
                toCountryId: function () {
                    if ($scope.eCommerceBooking.ShipTo != null && $scope.eCommerceBooking.ShipTo.Country !== null && $scope.eCommerceBooking.ShipTo.Country.CountryId) {
                        return $scope.eCommerceBooking.ShipTo.Country.CountryId;
                    }
                    else {
                        return 0;
                    }
                },
                addressType: function () {
                    return "ToAddress";
                },
                customerId: function () {
                    return $scope.customerId;
                },
                Countries: function () {
                    return $scope.CountriesRepo;
                },
                CountryPhoneCodes: function () {
                    return $scope.CountryPhoneCodes;
                }
            }
        });
        modalInstance.result.then(function (addressBooks) {
            if (addressBooks !== undefined && addressBooks !== null) {
                $scope.eCommerceBooking.ShipTo = addressBooks;
                $scope.SetShipToInfo($scope.eCommerceBooking.ShipTo.Country);
                $scope.eCommerceBooking.ShipTo.AddressBookId = 0;
            }

        });
    };

    $scope.ErrorTemplate = function (row) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBooking/errrorTemplate.tpl.html',
            controller: 'errorTemplateController',
            windowClass: 'ErrorTemplate-Modal',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                FrayteError: function () {
                    return row;
                }
            }

        });
    };
    $scope.directBookingGetService = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBooking/directBookingServices/directBookingGetService.tpl.html',
            controller: 'directBookingGetServiceController',
            windowClass: 'DirectBooking-Modal',
            size: 'lg',
            backdrop: 'static'
        });
    };

    $scope.getCustomerCurrencyDetail = function (CustomerDetail) {
        if (CustomerDetail !== undefined && CustomerDetail !== null) {
            $scope.eCommerceBooking.CustomerId = CustomerDetail.CustomerId;
            DirectBookingService.GetCustomerDetail(CustomerDetail.CustomerId).then(function (response) {
                if (response.data !== null) {
                    if (response.data.IsShipperTaxAndDuty) {
                        $scope.taxAndDutyDisabled = false;
                    }
                    else {
                        $scope.taxAndDutyDisabled = true;
                        $scope.eCommerceBooking.PayTaxAndDuties = '';
                        $scope.eCommerceBooking.TaxAndDutiesAcceptedBy = '';
                    }
                    //if (response.data !== null && response.data.IsRateShow) {
                    //    $scope.IsRateShow = true;
                    //}
                    //else {
                    //    $scope.IsRateShow = false;
                    //}
                    $scope.CurrencyCode = response.data.CurrencyCode;
                    var found = $filter('filter')($scope.CurrencyTypes, { CurrencyCode: $scope.CurrencyCode });
                    if (found.length) {
                        $scope.eCommerceBooking.Currency = found[0];
                    }
                }
            }, function () {

            });
        }

    };
    var getUserTab = function (tabs, tabKey) {
        if (tabs !== undefined && tabs !== null && tabs.length) {
            var tab = {};
            for (var i = 0; i < tabs.length; i++) {
                if (tabs[i].tabKey === tabKey) {
                    tab = tabs[i];
                    break;
                }
            }
            return tab;
        }
    };
    function init() {
        $scope.active = 1;
        $scope.BookingType = "";
        $scope.htmlPopover = $sce.trustAsHtml('<b style="color: red">I can</b> have <div class="label label-success">HTML</div> content');
        $scope.CountryPopOver = $sce.trustAsHtml('<span style : "width : 269px">China (CN1) covers Guangdong Province only. <br/>China (CN2) covers all other provinces.<span>');
        $scope.ConsineeNotification = $sce.trustAsHtml("If required, please enter the email address of the person you wish to receive this shipments details once booked.");
        $scope.ShipperNotification = $sce.trustAsHtml("Ticking this will send the newly created shipment detail to the provided shipper email.");
        $scope.AddressBook = $sce.trustAsHtml("The Address Book is dependent on the Payment Party (Customer) being filled in.");
        $scope.NextBillableWeight = $sce.trustAsHtml("This is the estimated volumetric weight based on the dimensions you have provided. For further information on volumetric weight and hows its calculated, please refer to your agreed Frayte Logistics Terms and Conditions.");
        $scope.ParcelTypePopup = $sce.trustAsHtml("Please specify whether the shipment(s) will be classed as documents or non-documents.");
        $scope.RestrictionExplanationPopup = $sce.trustAsHtml("If applicable, please explain why there maybe restrictions on the goods you are trying to send.");
        $rootScope.manageDirectBookingChange = true;
        $scope.CustomerDetail = null;
        $scope.submitted = true;
        $rootScope.directBookingChange = true;
        $scope.blinkCustomInfo = true;
        $anchorScroll.yOffset = 200;
        $scope.disableCourier = false;
        $scope.isCollectionTimeValid = true;
        $scope.parcelTypeDisable = false;
        //$scope.status = {
        //    opened: false
        //};

        UploadShipmentService.GetServices().then(function (response) {

            $scope.CourierService = response.data;

        },
     function () {
         toaster.pop({
             type: 'error',
             title: $scope.TitleFrayteError,
             body: $scope.TextErrorOccuredDuringUpload,
             showCloseButton: true
         });
     });

        $scope.toggleMin = function () {
            $scope.minDate = $scope.minDate ? null : new Date();
        };
        $scope.taxAndDutyDisabled = false;
        $scope.toggleMin();
        $scope.dateOptions = {
            formatYear: 'yy',
            startingDay: 1
        };
        $scope.CountriesRepo = [];
        $scope.photoUrl = config.BUILD_URL + "addressBook.png";
        $scope.ShipmentStatus = {
            Current: 17,
            Past: 18,
            Draft: 19
        };
        $scope.CurrencyTypes = [];
        new SetMultilingualOtions();

        var userInfo = SessionService.getUser();
        $scope.tabs = userInfo.tabs;
        $scope.tab = getUserTab($scope.tabs, "QuickDirect_Booking");
        $scope.RoleId = userInfo.RoleId;
        $scope.CreatedBy = userInfo.EmployeeId;
        if ($scope.RoleId === 1 || $scope.RoleId === 6) {
            $scope.customerId = 0;

            $scope.paymentAccount = true;
            DirectBookingService.GetDirectBookingCustomers(userInfo.EmployeeId).then(function (response) {
                $scope.directBookingCustomers = response.data;
                for (i = 0; i < $scope.directBookingCustomers.length; i++) {
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

            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteServiceErrorValidation,
                    body: $scope.ReceiveDetail_Validation,
                    showCloseButton: true
                });
            });
        }
        else {
            $scope.customerId = userInfo.EmployeeId;
            $scope.paymentAccount = false;
        }
        $scope.OperationZoneId = userInfo.OperationZoneId;

        $scope.ContentsType = [
             {
                 name: 'Documents',
                 value: 'documents'
             },
                 {
                     name: 'Gift',
                     value: 'gift'
                 },
             {
                 name: 'Merchandise',
                 value: 'merchandise'
             },
             {
                 name: 'Returned Goods',
                 value: 'returned_goods'
             },


             {
                 name: 'Sample',
                 value: 'sample'
             },
             {
                 name: 'Other',
                 value: 'other'
             }
        ];

        $scope.RestrictionType = [
            {
                name: 'None',
                value: 'none'
            },
            {
                name: 'Other',
                value: 'other'
            },
            {
                name: 'Quarantine',
                value: 'quarantine'
            },
            {
                name: 'Sanitary Phytosanitary Inspection',
                value: 'sanitary_phytosanitary_inspection'
            }
        ];

        $scope.NonDeliveryOption = [
           {
               name: 'Abandon',
               value: 'abandon'
           },
           {
               name: 'Return',
               value: 'return'
           }
        ];

        $scope.ItemCatogory = [
            {
                name: 'Sold',
                value: 'sold'
            },

            {
                name: 'Samples',
                value: 'samples'
            },
            {
                name: 'Gift',
                value: 'gift'
            },
            {
                name: 'Documents',
                value: 'documents'
            },
            {
                name: 'Commercial Sample',
                value: 'commercial_sample'
            },
            {
                name: 'Returned Goods',
                value: 'returned_goods'
            }
        ];



        //Step 1: Get initials and default address of logged in customer.
        DirectBookingService.GetInitials($scope.customerId).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            // Set Country type according to given order
            $scope.CountriesRepo = TopCountryService.TopCountryList(response.data.Countries);
            $scope.ToCountiesRepo = TopCountryService.TopCountryList(response.data.ToCountries);
            $scope.PiecesExcelDownloadPathXlsx = response.data.PiecesExcelDownloadPathXlsx;
            $scope.PiecesExcelDownloadPathXls = response.data.PiecesExcelDownloadPathXls;
            $scope.PiecesExcelDownloadPathCsv = response.data.PiecesExcelDownloadPathCsv;
            // Set Currency type according to given order
            $scope.CurrencyTypes = TopCurrencyService.TopCurrencyList(response.data.CurrencyTypes);

            $scope.ParcelTypes = response.data.ParcelTypes;
            $scope.ShipmentMethods = response.data.ShipmentMethods;
            $scope.CustomerDetail = response.data.CustomerDetail;

            if ($scope.RoleId === 3) {
                if ($scope.CustomerDetail !== null && $scope.CustomerDetail.IsShipperTaxAndDuty) {
                    $scope.taxAndDutyDisabled = false;
                }
                else if ($scope.CustomerDetail !== null && !$scope.CustomerDetail.IsShipperTaxAndDuty) {
                    $scope.taxAndDutyDisabled = true;
                }
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
            $scope.CountryPhoneCodes = response.data.CountryPhoneCodes;

            var eCommerceShipmentId = 0;
            if ($stateParams.shipmentId) {
                eCommerceShipmentId = parseInt($stateParams.shipmentId, 10);
            }
            if (ShipmentId) {
                eCommerceShipmentId = ShipmentId;
                CallingType = "ShipmentDraft";
            }

            if (eCommerceShipmentId > 0) {
                //    var CallingType = '';

                //    if ($scope.tab !== undefined && $scope.tab !== null && $scope.tab.childTabs !== null) {

                //        CallingType = UtilityService.GetCurrentShipmentType($scope.tab);

                //    }

                //if ($state.is('customer.booking-home.eCommerce-booking-return') ||
                //     $state.is('admin.booking-home.eCommerce-booking-return') ||
                //     $state.is('dbuser.booking-home.eCommerce-booking-return')) {
                //    CallingType = "ShipmentReturn";
                //}
                //else if ($state.is('customer.booking-home.eCommerce-booking-clone') ||
                //     $state.is('admin.booking-home.eCommerce-booking-clone') ||
                //     $state.is('dbuser.booking-home.eCommerce-booking-clone')) {
                //    CallingType = "ShipmentClone";
                //}
                //else if ($state.is('customer.booking-home.eCommerce-booking') ||
                //    $state.is('admin.booking-home.eCommerce-booking') ||
                //    $state.is('dbuser.booking-home.eCommerce-booking')) {
                //    CallingType = "ShipmentDraft";
                //}
                //else {
                //    CallingType = "";
                //}

                eCommerceBookingService.GeteCommerceWithServiceFormBookingDetailDraft(eCommerceShipmentId, CallingType).then(function (response) {
                    $scope.eCommerceBooking = response.data;
                    $scope.eCommerceBooking.TrackingNo = TrackingNo;
                    $scope.eCommerceBooking.LogisticCompany = CourierCompany;
                    //if ($scope.eCommerceBooking.PakageCalculatonType === null || $scope.eCommerceBooking.PakageCalculatonType === undefined || $scope.eCommerceBooking.PakageCalculatonType === "" ||
                    //    $scope.eCommerceBooking.PakageCalculatonType !== 'kgToCms' || $scope.eCommerceBooking.PakageCalculatonType !== 'lbToInchs') {
                    //    $scope.eCommerceBooking.PakageCalculatonType = 'kgToCms';
                    //}
                    $scope.HideContent = true;
                    var dbpac = $scope.eCommerceBooking.Packages.length - 1;
                    for (i = 0; i < $scope.eCommerceBooking.Packages.length; i++) {

                        if (i === dbpac) {
                            $scope.eCommerceBooking.Packages[i].pacVal = true;
                        }
                        else {
                            $scope.eCommerceBooking.Packages[i].pacVal = false;
                        }
                    }
                    angular.forEach($scope.eCommerceBooking.Packages, function (obj) {
                        obj.IsHSCodeSet = true;
                    });
                    $scope.eCommerceBooking.ModuleType = "eCommerce";
                    if ($scope.RoleId !== 3 && $scope.directBookingCustomers && $scope.directBookingCustomers.length) {
                        for (i = 0; i < $scope.directBookingCustomers.length; i++) {
                            if ($scope.eCommerceBooking.CustomerId === $scope.directBookingCustomers[i].CustomerId) {
                                $scope.CustomerDetail = $scope.directBookingCustomers[i];
                            }
                        }
                    }
                    var fromAddress = response.data.ShipFrom;
                    var toAddress = response.data.ShipTo;
                    if (CallingType === "ShipmentReturn") {
                        $scope.eCommerceBooking.ShipFrom = toAddress;
                        $scope.eCommerceBooking.ShipTo = fromAddress;
                    }
                    $scope.changeKgToLb($scope.eCommerceBooking.PakageCalculatonType);
                    // Set ShipFrom Phone Code
                    $scope.SetShipFrominfo($scope.eCommerceBooking.ShipFrom.Country);

                    // Set ShipTo PhoneCode
                    $scope.SetShipToInfo($scope.eCommerceBooking.ShipTo.Country);

                    var parcel = $filter('filter')($scope.ParcelTypes, { ParcelType: $scope.eCommerceBooking.ParcelType.ParcelType });
                    if (parcel.length) {
                        $scope.eCommerceBooking.ParcelType = parcel[0];
                    }
                    if ($scope.eCommerceBooking.ReferenceDetail.CollectionDate !== undefined &&
                        $scope.eCommerceBooking.ReferenceDetail.CollectionDate !== null) {
                        $scope.eCommerceBooking.ReferenceDetail.CollectionDate = moment.utc($scope.eCommerceBooking.ReferenceDetail.CollectionDate).toDate();
                    }



                    //If page is called from 'Clone' or 'Create Return' functionality then on save system will create new shpment.
                    if (CallingType === "ShipmentClone" || CallingType === "ShipmentClone") {
                        $scope.eCommerceBooking.ShipFrom.Email = '';
                        $scope.eCommerceBooking.ShipTo.Email = '';
                    }
                },
                function () {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteServiceErrorValidation,
                        body: $scope.ReceiveDetail_Validation,
                        showCloseButton: true
                    });
                });
            }
            else {
                $scope.NewDirectBooking();
                if ($scope.RoleId === 3) {
                    $scope.eCommerceBooking.CustomerId = $scope.customerId;
                }
                $scope.eCommerceBooking.OpearionZoneId = $scope.OperationZoneId;
                $scope.changeKgToLb($scope.eCommerceBooking.PakageCalculatonType);
                $scope.eCommerceBooking.DirectShipmentId = 0;
                //  $scope.eCommerceBooking.ShipmentMethod = $scope.ShipmentMethods[0];

                if ($scope.eCommerceBooking.ReferenceDetail.CollectionDate !== undefined &&
                        $scope.eCommerceBooking.ReferenceDetail.CollectionDate !== null) {
                    $scope.eCommerceBooking.ReferenceDetail.CollectionDate = moment.utc($scope.eCommerceBooking.ReferenceDetail.CollectionDate).toDate();
                }

                if ($scope.CustomerDetail !== undefined && $scope.CustomerDetail !== null && $scope.CustomerDetail.CurrencyCode !== null && $scope.CustomerDetail.CurrencyCode !== '') {
                    var found = $filter('filter')($scope.CurrencyTypes, { CurrencyCode: $scope.CustomerDetail.CurrencyCode });
                    if (found.length) {
                        $scope.eCommerceBooking.Currency = found[0];
                    }
                }


                for (var pa = 0 ; pa < $scope.ParcelTypes.length; pa++) {
                    if ($scope.ParcelTypes[pa].ParcelDescription === "Parcel (Non Doc)") {
                        $scope.eCommerceBooking.ParcelType = $scope.ParcelTypes[pa];
                        break;
                    }
                }
                //var found1 = $filter('filter')($scope.ParcelTypes, { ParcelDescription: "Parcel (Non Doc)" });
                //if (found1.length) {
                //    $scope.eCommerceBooking.ParcelType = found1[0];
                //}

                $scope.SetCustomerAddress();
                // Set ShipFrom Phone Code
                $scope.SetShipFrominfo($scope.eCommerceBooking.ShipFrom.Country);
            }

        },
       function () {
           AppSpinner.hideSpinnerTemplate();
           toaster.pop({
               type: 'error',
               title: $scope.FrayteServiceErrorValidation,
               body: $scope.InitialDataValidation,
               showCloseButton: true
           });
       });
        $rootScope.GetServiceValue = '';
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.ImagePath = config.BUILD_URL;
    }

    init();
});
