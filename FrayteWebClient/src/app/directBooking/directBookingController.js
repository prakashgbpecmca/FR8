angular.module('ngApp.directBooking').controller('DirectBookingController', function ($uibModalStack, UtilityService, Upload, $rootScope, AppSpinner, $sce, TopCountryService, $location, $anchorScroll, TopCurrencyService, $scope, config, $filter, $state, ModalService, $translate, $uibModal, $timeout, toaster, $stateParams, DirectBookingService, SessionService) {

    $scope.$watch('directBooking.ReferenceDetail.Reference1', function (val) {
        if (val !== undefined && val !== null && val !== '') {
            var refLength = [];
            refLength = val.split('');
            if (refLength.length > 0) {
                $scope.RemainChar = refLength.length;
            }
            else {
                $scope.RemainChar = 0;
            }
        }
        if (val === undefined) {
            $scope.RemainChar = 0;
        }

    });
    //$scope.changefrayteUser = function (frayteUser) {
    //    $scope.FrayteUser = frayteUser;
    //};

    $scope.ContentCount = function (value) {
        if (value !== undefined && value !== null && value !== '') {
            var contentLength = [];
            for (i = 0; i < value.length; i++) {
                contentLength.push(value[i].Content);
            }
            $scope.Contentfinallength = contentLength.toString();
        }
    };
    //$scope.$watch('Package.Content', function (value) {


    //});

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
                title: $scope.TitleFrayteWarning,
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

    $scope.setScroll = function (printSectionId) {
        $location.hash(printSectionId);
        $anchorScroll();
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
                    body: "Package information addedd successfully.",
                    showCloseButton: true
                });

                //  To Do: Logic to add row in Piecees grid.
                var result = data;

                if (result.FrayteShipmentDetail !== null && result.FrayteShipmentDetail.length) {
                    for (var i = 0 ; i < result.FrayteShipmentDetail.length; i++) {
                        $scope.directBooking.Packages.push(result.FrayteShipmentDetail[i]);
                    }

                }
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: data.Message,
                    showCloseButton: true
                });
            }


        }
        else {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: "Error while uploading the excel. Please try again.",
                showCloseButton: true
            });
        }

    };

    $scope.errorExcel = function (err) {
        toaster.pop({
            type: 'error',
            title: $scope.TitleFrayteError,
            body: "Error while uploading the excel. Please try again.",
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
        if (DirectBookingForm !== undefined) {
            var flag = false;
            if (DirectBookingForm.shipFromAddress1 !== undefined && DirectBookingForm.shipFromAddress1.$valid &&
                DirectBookingForm.shipFromCompanyName !== undefined && DirectBookingForm.shipFromCompanyName.$valid &&
                DirectBookingForm.shipFromFirstName !== undefined && DirectBookingForm.shipFromFirstName.$valid &&
                DirectBookingForm.shipFromLastName !== undefined && DirectBookingForm.shipFromLastName.$valid &&
                DirectBookingForm.shipFromCountry !== undefined && DirectBookingForm.shipFromCountry.$valid &&
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
        if (DirectBookingForm !== undefined) {
            var flag = false;
            if (DirectBookingForm.shipToAddress1 !== undefined && DirectBookingForm.shipToAddress1.$valid &&
        DirectBookingForm.shipToCompanyName !== undefined && DirectBookingForm.shipToCompanyName.$valid &&
        DirectBookingForm.shipToFirstName !== undefined && DirectBookingForm.shipToFirstName.$valid &&
        DirectBookingForm.shipToLastName !== undefined && DirectBookingForm.shipToLastName.$valid &&
        DirectBookingForm.shipToCountry !== undefined && DirectBookingForm.shipToCountry.$valid &&
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
        if (DirectBookingForm !== undefined && $scope.directBooking !== undefined) {
            var flag = false;
            if (DirectBookingForm.parcelType !== undefined && DirectBookingForm.parcelType.$valid && $scope.isCollectionTimeValid &&
                DirectBookingForm.shipmentPaymentAccount !== undefined && DirectBookingForm.shipmentPaymentAccount.$valid &&
                DirectBookingForm.paymentPartyCurrencyType !== undefined && DirectBookingForm.paymentPartyCurrencyType.$valid &&
                   DirectBookingForm.paymentPartyCurrencyType !== undefined && DirectBookingForm.paymentPartyCurrencyType.$valid &&
                   DirectBookingForm.reference1 !== undefined && DirectBookingForm.reference1.$valid &&
                   DirectBookingForm.shipmentCollectionDate !== undefined && DirectBookingForm.shipmentCollectionDate.$valid &&
                 DirectBookingForm.collectiontime !== undefined && DirectBookingForm.collectiontime.$valid &&
                DirectBookingForm.collectioTimeMinutes !== undefined && DirectBookingForm.collectioTimeMinutes.$valid &&
               DirectBookingForm.paymentPartyCurrencyType !== undefined && DirectBookingForm.paymentPartyCurrencyType.$valid
                ) {
                for (var i = 0 ; i < $scope.directBooking.Packages.length; i++) {
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
        if (DirectBookingForm !== undefined && $scope.directBooking !== undefined && $scope.directBooking !== null) {
            var flag = false;
            //if ($scope.directBooking.ShipmentMethod !== null && $scope.directBooking.ShipmentMethod.ShipmentMethodName === 'UK/EU - Shipment') {
            //    if (DirectBookingForm.catagoryOfItem !== undefined && DirectBookingForm.catagoryOfItem.$valid &&
            // DirectBookingForm.catagoryOfItemExplanation !== undefined && DirectBookingForm.catagoryOfItemExplanation.$valid &&
            // DirectBookingForm.commodityCode !== undefined && DirectBookingForm.commodityCode.$valid &&
            //        $scope.directBooking.CustomInfo.CustomsCertify
            // ) {
            //        flag = true;
            //    }
            //    else {
            //        flag = false;
            //    }
            //}
            //else {
            if (DirectBookingForm.contentsType !== undefined && DirectBookingForm.contentsType.$valid &&
          DirectBookingForm.contentsExplanation !== undefined && DirectBookingForm.contentsExplanation.$valid &&
          DirectBookingForm.restrictionType !== undefined && DirectBookingForm.restrictionType.$valid &&
          DirectBookingForm.restrictionComments !== undefined && DirectBookingForm.restrictionComments.$valid &&
          DirectBookingForm.nonDeliveryOption !== undefined && DirectBookingForm.nonDeliveryOption.$valid &&
          DirectBookingForm.customsSigner !== undefined && DirectBookingForm.customsSigner.$valid &&
          $scope.directBooking.CustomInfo.CustomsCertify
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
          directBooking.ShipTo.Country !== undefined && directBooking.ShipTo.Country !== null &&
          directBooking.ShipFrom.Country.Code === 'GBR' && directBooking.ShipTo.Country.Code === 'GBR') {

            return false;
        }
        else {
            return true;
        }
    };
    $scope.setPackageParcelType = function (ParcelType) {
        if (ParcelType !== undefined && ParcelType !== null) {
            if (ParcelType.ParcelDescription === 'Letter (Doc)') {
                if ($scope.directBooking.Packages !== null && $scope.directBooking.Packages.length > 1) {
                    var modalOptions = {
                        headerText: "Confirmation",
                        bodyText: "Changing parcel type to DOC will remove the pieces and only consider first item as an shipment item, do you want to continue to change the parcel type?"
                    };

                    ModalService.Confirm({}, modalOptions).then(function (result) {
                        var data = $scope.directBooking.Packages[0];
                        $scope.directBooking.Packages = [];
                        $scope.directBooking.Packages.push(data);
                        $scope.directBooking.Packages[0].Content = 'Document';
                        $scope.directBooking.Packages[0].CartoonValue = 1;
                        $scope.directBooking.Packages[0].Length = 5;
                        $scope.directBooking.Packages[0].Width = 5;
                        $scope.directBooking.Packages[0].Height = 5;
                        $scope.directBooking.Packages[0].Weight = 1;
                        $scope.directBooking.Packages[0].Value = 5;
                    });

                }
                else {
                    $scope.directBooking.Packages[0].Content = 'Document';
                    $scope.directBooking.Packages[0].CartoonValue = 1;
                    $scope.directBooking.Packages[0].Length = 5;
                    $scope.directBooking.Packages[0].Width = 5;
                    $scope.directBooking.Packages[0].Height = 5;
                    $scope.directBooking.Packages[0].Weight = 1;
                    $scope.directBooking.Packages[0].Value = 5;
                }
            }
            else {
                if ($scope.directBooking.Packages !== null && $scope.directBooking.Packages.length > 0) {
                    $scope.directBooking.Packages[0].Content = '';
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

                DirectBookingForm.$valid = false;
            }
        }

    };
    //$scope.shipFromOptionalField = function (action) {
    //    if (action !== undefined && action !== null && action !== '' && action === 'State') {
    //        if ($scope.directBooking !== undefined && $scope.directBooking !== null && $scope.directBooking.ShipFrom !== null && $scope.directBooking.ShipFrom.Country !== null && $scope.directBooking.ShipFrom.Country !== undefined) {
    //            if ($scope.directBooking.ShipFrom.Country.Code !== 'HKG' && $scope.directBooking.ShipFrom.Country.Code !== 'GBR') {
    //                return true;
    //            }
    //            else {
    //                $scope.directBooking.ShipFrom.State = '';
    //                return false;
    //            }
    //        }
    //    }
    //    else if (action !== undefined && action !== null && action !== '' && action === 'PostCode') {
    //        if ($scope.directBooking !== undefined && $scope.directBooking !== null && $scope.directBooking.ShipFrom !== null) {
    //            if ($scope.directBooking.ShipFrom.PostCode === '' || $scope.directBooking.ShipFrom.PostCode === null) {
    //                return true;
    //            }
    //            else if ($scope.directBooking.ShipFrom !== undefined && $scope.directBooking.ShipFrom !== null && $scope.directBooking.ShipFrom.Country !== undefined && $scope.directBooking.ShipFrom.Country !== null && $scope.directBooking.ShipFrom.Country.Code !== 'HKG') {
    //                return true;
    //            }
    //            else {
    //                //$scope.directBooking.ShipFrom.PostCode = '';
    //                return false;
    //            }
    //        }
    //        else {
    //            return false;
    //        }
    //    }

    //};
    $scope.shipFromOptionalField = function (action) {
        if (action !== undefined && action !== null && action !== '' && action === 'State') {
            if ($scope.directBooking !== undefined && $scope.directBooking !== null && $scope.directBooking.ShipFrom !== null && $scope.directBooking.ShipFrom.Country !== null) {
                if ($scope.directBooking.ShipFrom.Country.Code !== 'HKG' && $scope.directBooking.ShipFrom.Country.Code !== 'GBR') {
                    return true;
                }
                else {
                    $scope.directBooking.ShipFrom.State = '';
                    return false;
                }
            }
        }
        else if (action !== undefined && action !== null && action !== '' && action === 'PostCode') {
            if ($scope.directBooking !== undefined && $scope.directBooking !== null && $scope.directBooking.ShipFrom !== null && $scope.directBooking.ShipFrom.Country !== null) {
                if ($scope.directBooking.ShipFrom.Country.Code !== 'HKG') {
                    return true;
                }
                else {
                    $scope.directBooking.ShipFrom.PostCode = '';
                    return false;
                }
            }
        }

    };
    $scope.shipToOptionalField = function (action) {
        if (action !== undefined && action !== null && action !== '' && action === 'State') {
            if ($scope.directBooking !== undefined && $scope.directBooking !== null && $scope.directBooking.ShipTo !== null && $scope.directBooking.ShipTo.Country !== null) {
                if ($scope.directBooking.ShipTo.Country.Code !== 'HKG' && $scope.directBooking.ShipTo.Country.Code !== 'GBR') {

                    return true;
                }
                else {
                    $scope.directBooking.ShipTo.State = '';
                    return false;
                }
            }
        }
        else if (action !== undefined && action !== null && action !== '' && action === 'PostCode') {
            if ($scope.directBooking !== undefined && $scope.directBooking !== null && $scope.directBooking.ShipTo !== null && $scope.directBooking.ShipTo.Country !== null) {
                if ($scope.directBooking.ShipTo.Country.Code !== 'HKG') {

                    return true;
                }
                else {
                    $scope.directBooking.ShipTo.PostCode = '';
                    return true;
                }
            }
        }

    };
    //$scope.shipToOptionalField = function (action) {
    //    if (action !== undefined && action !== null && action !== '' && action === 'State') {
    //        if ($scope.directBooking !== undefined && $scope.directBooking !== null && $scope.directBooking.ShipTo !== null && $scope.directBooking.ShipTo.Country !== null && $scope.directBooking.ShipTo.Country !== undefined) {
    //            if ($scope.directBooking.ShipTo.Country.Code !== 'HKG' && $scope.directBooking.ShipTo.Country.Code !== 'GBR') {

    //                return true;
    //            }
    //            else {
    //                $scope.directBooking.ShipTo.State = '';
    //                return false;
    //            }
    //        }
    //    }
    //    else if (action !== undefined && action !== null && action !== '' && action === 'PostCode') {
    //        if ($scope.directBooking !== undefined && $scope.directBooking !== null && $scope.directBooking.ShipTo !== null) {
    //            if ($scope.directBooking.ShipTo.PostCode === null || $scope.directBooking.ShipTo.PostCode === '') {
    //                return true;
    //            }

    //            else if ($scope.directBooking.ShipTo !== undefined && $scope.directBooking.ShipTo !== null && $scope.directBooking.ShipTo.Country !== null && $scope.directBooking.ShipTo.Country !== undefined && $scope.directBooking.ShipTo.Country.Code !== 'HKG') {

    //                return true;
    //            }
    //            else {
    //               // $scope.directBooking.ShipTo.PostCode = '';
    //                return false;
    //            }
    //        }
    //    }

    //};
    $scope.getTotalKgs = function () {
        if ($scope.directBooking === undefined) {
            return;
        }

        else if ($scope.directBooking.Packages === undefined || $scope.directBooking.Packages === null) {
            return 0;
        }
        else if ($scope.directBooking.Packages.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.directBooking.Packages.length; i++) {
                var product = $scope.directBooking.Packages[i];
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
        if ($scope.directBooking === undefined) {
            return;
        }

        else if ($scope.directBooking.Packages === undefined || $scope.directBooking.Packages === null) {
            return 0;
        }

        if ($scope.directBooking.Packages.length >= 0) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.directBooking.Packages.length; i++) {
                var product = $scope.directBooking.Packages[i];
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
                    if ($scope.directBooking.PakageCalculatonType === "kgToCms") {
                        total += ((len * wid * height) / 6000) * qty;
                    }
                    else if ($scope.directBooking.PakageCalculatonType === "lbToInchs") {
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
                if ($scope.ShowCustomInfoSection($scope.directBooking)) {
                    $scope.directBooking.CustomInfo.ContentsExplanation = ContentDescription;
                }
            }
        }
        else if (CustomType !== undefined && CustomType === 'RestrictionExplanation') {
            if (ContentDescription !== undefined && ContentDescription !== null && ContentDescription !== '') {
                if ($scope.ShowCustomInfoSection($scope.directBooking)) {
                    $scope.directBooking.CustomInfo.RestrictionComments = ContentDescription;
                }
            }
        }

    };
    $scope.showGetServicePlaceBooking = function (DirectBookingForm) {

        if (DirectBookingForm !== undefined) {

            var flag = false;
            if (DirectBookingForm.shipFromAddress1 !== undefined && DirectBookingForm.shipFromAddress1.$valid &&
                DirectBookingForm.shipFromCompanyName !== undefined && DirectBookingForm.shipFromCompanyName.$valid &&
                DirectBookingForm.shipFromFirstName !== undefined && DirectBookingForm.shipFromFirstName.$valid &&
                DirectBookingForm.shipFromLastName !== undefined && DirectBookingForm.shipFromLastName.$valid &&
                DirectBookingForm.shipFromCountry !== undefined && DirectBookingForm.shipFromCountry.$valid &&
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
        $scope.directBooking = {
            OpearionZoneId: 0,
            DirectShipmentDraftId: 0,
            CustomerId: null,
            ShipmentStatusId: 14,
            1: 0,
            CreatedBy: $scope.CreatedBy,
            ShipFrom: {
                AddressBookId: 0,
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
                IsMailSend: false
            },
            ShipTo: {
                AddressBookId: 0,
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
                IsMailSend: false
            },
            ParcelType: null,
            PayTaxAndDuties: "Receiver",
            PaymentPartyAccountNumber: '',
            Currency: null,
            Packages: [{
                DirectShipmentDetailDraftId: 0,
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
                ContentDescription: "Goods",
                CollectionDate: new Date(),
                CollectionTime: setColletionTime()
            },
            CustomInfo: {
                ShipmentCustomDetailId: 0,
                ContentsType: null,
                ContentsExplanation: 'Goods',
                RestrictionType: $scope.RestrictionType[0].value,
                RestrictionComments: 'N/A',
                CustomsCertify: false,
                CustomsSigner: '',
                NonDeliveryOption: $scope.NonDeliveryOption[1].value
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
            ShipmentMethod: {
                ShipmentMethodId: 0,
                ShipmentMethodName: ""
            },
            PakageCalculatonType: 'kgToCms',
            TaxAndDutiesAcceptedBy: '',
            BookingStatusType: 'Draft'

        };
    };
    var setColletionTime = function () {

        var h = "16";
        var m = "30";
        return h.toString() + m.toString();

    };
    $scope.checkTime = function (DirectBookingForm) {
        if (DirectBookingForm !== undefined && $scope.directBooking !== undefined && $scope.directBooking.ReferenceDetail !== undefined && $scope.directBooking.ReferenceDetail !== null) {
            var d = new Date();
            var min = "";
            if (d.getMinutes().toString().length === 1) {
                min = "0" + d.getMinutes().toString();
            }
            else {
                min = d.getMinutes().toString();
            }
            var str = d.getHours().toString() + min;
            var colDate = $scope.directBooking.ReferenceDetail.CollectionDate;
            var dateString1 = colDate.getMonth().toString() + "-" + colDate.getDate().toString() + "-" + colDate.getFullYear().toString();
            var dateString2 = d.getMonth().toString() + "-" + d.getDate().toString() + "-" + d.getFullYear().toString();

            var CollectionDate = new Date(dateString1);
            var CurrentDate = new Date(dateString2);
            if (CollectionDate < CurrentDate) {
                DirectBookingForm.collectiontime.$dirty = true;
                $scope.isCollectionTimeValid = false;
                return true;
            }
            else if (+CollectionDate === +CurrentDate) {
                if (parseFloat($scope.directBooking.ReferenceDetail.CollectionTime) < parseFloat(str)) {
                    if (DirectBookingForm.collectiontime !== undefined) {
                        DirectBookingForm.collectiontime.$dirty = true;
                    }
                    $scope.isCollectionTimeValid = false;
                    return true;
                }
                else {
                    $scope.isCollectionTimeValid = true;
                    return false;
                }
            }
            else {
                $scope.isCollectionTimeValid = true;
                return false;
            }
        }

    };
    $scope.ValidateForSaving = function () {

        if ($scope.directBooking.Packages === undefined ||
            $scope.directBooking.Packages === null ||
            $scope.directBooking.Packages.length === 0) {

            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.PackageShipment_Validation,
                showCloseButton: true
            });

            return false;
        }
        else if ($scope.directBooking.ShipmentStatusId === $scope.ShipmentStatus.Current &&
                ($scope.directBooking.CustomerRateCard === null ||
                 $scope.directBooking.CustomerRateCard.LogisticServiceId === 0)) {

            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.PleaseSelectShipment,
                showCloseButton: true
            });

            return false;
        }
            //else if ($scope.ShowCustomInfoSection($scope.directBooking) &&
            //         $scope.directBooking.CustomerRateCard !== null &&
            //         $scope.directBooking.CustomerRateCard.LogisticServiceId > 0 
            //         //$scope.directBooking.ShipmentMethod !== null &&
            //         //$scope.directBooking.ShipmentMethod !== undefined &&
            //         //$scope.directBooking.ShipmentMethod.ShipmentMethodId > 0 &&
            //    //$scope.directBooking.CustomerRateCard.CourierId !== $scope.directBooking.ShipmentMethod.ShipmentMethodId
            //    ) {
            //    toaster.pop({
            //        type: 'warning',
            //        title: $scope.TitleFrayteValidation,
            //        body: $scope.CustomInformationValidation,
            //        showCloseButton: true
            //    });
            //    return false;
            //}
        else {
            return true;
        }
    };
    $scope.PlaceBooking = function (DirectBookingForm, isValid, directBooking) {
        if (DirectBookingForm !== undefined) {
            $rootScope.GetServiceValue = false;
        }
        if ($scope.BookingType !== "Draft") {
            var isBusinessRulePassed = $scope.ValidateForSaving();
            if (!isBusinessRulePassed) {
                return;
            }
            var flag = false;
            for (var i = 0 ; i < $scope.directBooking.Packages.length; i++) {
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
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.TextValidation,
                    showCloseButton: true
                });
                return;
            }
        }
        //else {
        //    if (directBooking.ShipFrom.Country === undefined || directBooking.ShipFrom.Country === null || directBooking.ShipTo.Country === undefined || directBooking.ShipFrom.Country === null || directBooking.CustomerId === undefined || directBooking.CustomerId === null || directBooking.CustomerId === 0) {
        //        toaster.pop({
        //            type: 'warning',
        //            title: $scope.TitleFrayteWarning,
        //            body: $scope.SaveDraft,
        //            showCloseButton: true
        //        });
        //        return;
        //    }
        //    else {
        //        toaster.pop({
        //            type: 'warning',
        //            title: $scope.TitleFrayteWarning,
        //            body: $scope.TextValidation,
        //            showCloseButton: true
        //        });
        //        return;
        //    }
        //}

        if ($scope.BookingType === "Draft" && directBooking.ShipFrom.Country !== null && directBooking.ShipTo.Country !== null || (isValid && $scope.isCollectionTimeValid)) {
            //if ($scope.directBooking.CustomerRateCard !== null && $scope.directBooking.CustomerRateCard !== undefined && $scope.directBooking.CustomerRateCard.CourierId > 0) {
            //    $scope.directBooking.ShippingMethodId = $scope.directBooking.CustomerRateCard.CourierId;
            //}
            //else if ($scope.directBooking.ShipmentMethod !== null && $scope.directBooking.ShipmentMethod !== undefined && $scope.directBooking.ShipmentMethod.ShipmentMethodId > 0) {
            //    $scope.directBooking.ShippingMethodId = $scope.directBooking.ShipmentMethod.ShipmentMethodId;
            //}

            directBooking.Packages[0].TrackingNo = null;
            if ($scope.directBooking.Packages !== null && $scope.directBooking.Packages.length > 0) {
                var str = "";
                for (var dp = 0 ; dp < $scope.directBooking.Packages.length; dp++) {
                    str += ", " + $scope.directBooking.Packages[dp].Content;
                }
                $scope.directBooking.ReferenceDetail.ContentDescription = str.slice(2);
            }


            //AppSpinner.showSpinnerTemplate('', $scope.Template);

            if ($scope.BookingType === "Draft") {
                directBooking.ShipmentStatusId = 14;
                directBooking.BookingStatusType = "Draft";
            }
            else {
                directBooking.BookingStatusType = "Current";
                AppSpinner.showSpinnerTemplate('', $scope.Template);
            }
            DirectBookingService.SaveDirectBooking(directBooking).then(function (response) {
                if (response.status === 200 && response.data.Error.Status) {
                    AppSpinner.hideSpinnerTemplate();
                    //Redirect to direct shipmnets page after 2 second.
                    $timeout(function () {
                        //  $scope.DirectShipmentDetail(response.data.DirectShipmentId);
                        toaster.pop({
                            type: 'success',
                            title: $scope.TitleFrayteSuccess,
                            body: $scope.BookingSaveValidation,
                            showCloseButton: true
                        });

                        var route = UtilityService.GetCurrentRoute($scope.tabs, "direct-shipments");

                        if (response.data.ShipmentStatusId !== $scope.ShipmentStatus.Draft) {
                            var modalInstance = $uibModal.open({
                                animation: true,
                                templateUrl: 'directBooking/directBookingDetail/directBookingDetail.tpl.html',
                                controller: 'DirectBookingDetailController',
                                windowClass: 'DirectBookingDetail',
                                size: 'lg',
                                backdrop: 'static',
                                keyboard: false,
                                resolve: {
                                    shipmentId: function () {
                                        return response.data.DirectShipmentDraftId;
                                    },
                                    ShipmentStatus: function () {
                                        return "";
                                    }
                                }
                            });
                            modalInstance.result.then(function () {
                                $rootScope.manageDirectBookingChange = false;

                                if ($scope.RoleId === 3) {
                                    $state.go(route, { moduleType: "db" }, { reload: true });
                                }
                                else {
                                    $state.go(route, {}, { reload: true });
                                }

                            }, function () {
                            });
                        }
                        else {
                            $rootScope.manageDirectBookingChange = false;
                            if ($scope.RoleId === 3) {
                                $state.go(route, { moduleType: "db" }, { reload: true });
                            }
                            else {
                                $state.go(route, {}, { reload: true });
                            }
                        }

                    }, 2000);
                }
                else {
                    AppSpinner.hideSpinnerTemplate();
                    $scope.directBooking.DirectShipmentDraftId = response.data.DirectShipmentDraftId;
                    $scope.directBooking.ShipFrom = response.data.ShipFrom;
                    $scope.directBooking.ShipTo = response.data.ShipTo;
                    $scope.directBooking.CustomInfo = response.data.CustomInfo;
                    $scope.directBooking.Packages = response.data.Packages;
                    $scope.ErrorTemplate(response.data.Error);
                    //toaster.pop({
                    //    type: 'error',
                    //    title: "Frayte Service Error",
                    //    body: "An error occurred on frayte service side. Please try later",
                    //    showCloseButton: true
                    //});
                }
            },
                function () {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.ServiceSideError_Validation,
                        showCloseButton: true
                    });
                });
        }
        else {
            AppSpinner.hideSpinnerTemplate();
            if (directBooking.ShipFrom.Country === undefined || directBooking.ShipFrom.Country === null || directBooking.ShipTo.Country === undefined || directBooking.ShipFrom.Country === null || directBooking.CustomerId === undefined || directBooking.CustomerId === null || directBooking.CustomerId === 0) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.SaveDraft,
                    showCloseButton: true
                });
                return;
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.TextValidation,
                    showCloseButton: true
                });
                return;
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
        $translate(['FrayteError', 'FrayteWarning', 'FrayteInformation', 'FrayteValidation', 'FrayteSuccess', 'CustomInfoCheckTerms',
            'PleaseSelectionDeliveryOption', 'PleaseSelectTermAndConditions', 'PleaseCorrectValidationErrors',
            'ErrorSavingShipment', 'PleaseSelectValidFile', 'UploadedSuccessfully', 'ErrorOccuredDuringUpload',
            'ErrorGettingShipmentDetailServer', 'SaveDraft_Validation', 'PackageShipment_Validation',
            'SelectShipment_Validation', 'CustomInformation_Validation', 'BookingSave_Validation',
        'ServiceSideError_Validation', 'Success', 'RemovePackage_Validation', 'Validation_Error', 'GetService_Validation',
        'SelectCustomer_Validation', 'SelectCurrency_Validation', 'FrayteWarning_Validation', 'SelectCustomerAddressBook_Validation',
        'FrayteServiceError_Validation', 'ReceiveDetail_Validation', 'InitialData_Validation']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteWarning = translations.FrayteWarning;
            $scope.TitleFrayteInformation = translations.FrayteSuccess;
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

            $scope.SaveDraft = translations.SaveDraft_Validation;
            $scope.PackageShipment_Validation = translations.PackageShipment_Validation;
            $scope.PleaseSelectShipment = translations.SelectShipment_Validation;
            $scope.CustomInformationValidation = translations.CustomInformation_Validation;
            $scope.BookingSaveValidation = translations.BookingSave_Validation;
            $scope.ServiceSideErrorValidation = translations.ServiceSideError_Validation;
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
        });
    };
    $scope.AddPackage = function () {
        //if ($scope.directBooking.Packages.length === 1) {
        //    $scope.NewDirectBooking();
        //}
        $scope.directBooking.Packages.push({
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
        var dbpac = $scope.directBooking.Packages.length - 1;
        for (i = 0; i < $scope.directBooking.Packages.length; i++) {

            if (i === dbpac) {
                $scope.directBooking.Packages[i].pacVal = true;
            }
            else {
                $scope.directBooking.Packages[i].pacVal = false;
            }
        }
    };
    $scope.RemovePackage = function (Package) {
        if (Package !== undefined && Package !== null) {
            var index = $scope.directBooking.Packages.indexOf(Package);
            if (Package.DirectShipmentDetailId > 0) {
                DirectBookingService.DeleteDirectShipmentPackage(Package.DirectShipmentDetailId).then(function (response) {
                    if (response.data.Status) {
                        toaster.pop({
                            type: 'success',
                            title: $scope.TitleFrayteSuccess,
                            body: $scope.RemovePackageValidation,
                            showCloseButton: true
                        });
                        $scope.directBooking.Packages.splice(index, 1);
                        $scope.Packges = angular.copy($scope.directBooking.Packages);
                        $scope.directBooking.Packages = [];
                        $scope.directBooking.Packages = $scope.Packges;
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
                $scope.directBooking.Packages.splice(index, 1);
                $scope.Packges = angular.copy($scope.directBooking.Packages);
                $scope.directBooking.Packages = [];
                $scope.directBooking.Packages = $scope.Packges;
            }
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
                //return convertc.toFixed(2);
                return Number(parseFloat(convertc).toFixed(2)).toLocaleString('en', {
                    minimumFractionDigits: 2
                });



            }, 0);
        }
    };
    $scope.setParcelTypeDisable = function () {
        if ($scope.directBooking !== undefined && $scope.directBooking.Packages !== null && $scope.directBooking.Packages.length > 0) {
            var total = 0;
            for (var i = 0; i < $scope.directBooking.Packages.length; i++) {
                total += parseFloat($scope.directBooking.Packages[i].Weight);
            }

            if (total > 2.0) {
                $scope.directBooking.ParcelType = $scope.ParcelTypes[0];
                return true;
            }
            else {
                return false;
            }
        }
    };
    $scope.OpenCalender = function ($event) {
        $scope.status.opened = true;
    };
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

        $scope.directBooking.CustomInfo.CustomsSigner = '';
        if (value) {
            $scope.disableCustomerSigner = false;
        }
        else {
            $scope.disableCustomerSigner = true;
        }
    };

    $scope.disableRestriction = function (RestrictionType) {
        $scope.directBooking.CustomInfo.RestrictionComments = '';
        if (RestrictionType === "other") {
            $scope.disableRestrictionComment = false;

        }
        else if (RestrictionType === "none") {
            $scope.directBooking.CustomInfo.RestrictionComments = 'N/A';
            $scope.disableRestrictionComment = false;
        }
        else {
            $scope.disableRestrictionComment = true;
        }
    };

    $scope.disableContent = function (ContentType) {
        $scope.directBooking.CustomInfo.ContentsExplanation = '';
        if (ContentType === "other") {
            $scope.disableContentExplanation = false;

        }
        else {
            $scope.disableContentExplanation = true;
        }

    };

    $scope.editGetServices = function (DirectBookingForm) {
        $scope.GetServices($scope.directBooking, DirectBookingForm);
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
            for (var i = 0 ; i < $scope.directBooking.Packages.length; i++) {
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
                    title: $scope.TitleFrayteWarning,
                    body: $scope.GetServiceValidation,
                    showCloseButton: true
                });

                return;
            }
            if (directBooking.CustomerId === null || directBooking.CustomerId === 0) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.SelectCustomerValidation,
                    showCloseButton: true
                });

                return;
            }
            if (directBooking.Currency === null || directBooking.Currency === undefined) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
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
                    },
                    CustomerDetail: function () {
                        return $scope.CustomerDetail;
                    }
                }
            });

            modalInstance.result.then(function (customerRateCard) {
                if (customerRateCard !== undefined && customerRateCard !== null) {
                    $scope.directBooking.CustomerRateCard = customerRateCard;
                    $scope.ContentCount(directBooking.Packages);
                    for (i = 0; i < $scope.RestrictionType.length; i++) {
                        if ($scope.RestrictionType[0].value === $scope.RestrictionType[i].value) {
                            $scope.directBooking.CustomInfo.RestrictionType = $scope.RestrictionType[i].value;

                        }
                    }
                    $scope.directBooking.CustomInfo.RestrictionComments = 'N/A';
                    $scope.directBooking.CustomInfo.ContentsExplanation = $scope.Contentfinallength;
                    $scope.checkcustomerRateCard = customerRateCard;
                    if ($scope.directBooking.CustomerRateCard.CourierName === 'DHL' || $scope.directBooking.CustomerRateCard.CourierName === 'Yodel') {
                        $scope.MaxLengthValue = 26;
                    }
                    else if ($scope.directBooking.CustomerRateCard.CourierName === 'UKMail' || $scope.directBooking.CustomerRateCard.CourierName === 'Hermes') {
                        $scope.MaxLengthValue = 11;
                    }




                    if ($scope.directBooking.CustomerRateCard.LogisticServiceId > 0) {
                        for (var aa = 0; aa < $scope.ShipmentMethods.length; aa++) {
                            //if ($scope.directBooking.CustomerRateCard.CourierId === $scope.ShipmentMethods[aa].ShipmentMethodId) {
                            //    $scope.directBooking.ShipmentMethod = $scope.ShipmentMethods[aa];
                            //    $scope.disableCourier = true;
                            //    break;
                            //}
                        }
                        if ($scope.ShowCustomInfoSection($scope.directBooking) && $scope.directBooking.CustomerRateCard.LogisticServiceId > 0) {
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
                title: $scope.TitleFrayteWarning,
                body: $scope.GetServiceValidation,
                showCloseButton: true
            });
        }


    };

    $scope.cancelServices = function () {
        $scope.active = 1;
        $scope.directBooking.CustomerRateCard = {
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
            $scope.directBooking.ShipFrom.Country = $scope.CustomerDetail.Country;
            $scope.directBooking.ShipFrom.PostCode = $scope.CustomerDetail.PostCode;
            $scope.directBooking.ShipFrom.FirstName = $scope.CustomerDetail.FirstName;
            $scope.directBooking.ShipFrom.CompanyName = $scope.CustomerDetail.CompanyName;
            $scope.directBooking.ShipFrom.Address = $scope.CustomerDetail.Address;
            $scope.directBooking.ShipFrom.Address2 = $scope.CustomerDetail.Address2;
            $scope.directBooking.ShipFrom.City = $scope.CustomerDetail.City;
            $scope.directBooking.ShipFrom.Area = $scope.CustomerDetail.Suburb;
            $scope.directBooking.ShipFrom.State = $scope.CustomerDetail.State;
            $scope.directBooking.ShipFrom.Phone = $scope.CustomerDetail.Phone;
            $scope.directBooking.ShipFrom.Email = $scope.CustomerDetail.Email;
        }
    };

    $scope.ClearForm = function (form) {
        form.$setPristine();
        form.$setUntouched();
        $scope.NewDirectBooking();

        $scope.CustomerDetail = null;
        if ($scope.RoleId === 3) {
            $scope.directBooking.CustomerId = $scope.customerId;
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
            directBooking.ShipTo.Country !== undefined && directBooking.ShipTo.Country !== null &&
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
    $scope.SetShipFrominfo = function (Country, Action) {
        if (Country !== undefined && Country !== null) {
            $scope.showPostCodeDropDown = false;
            for (var i = 0 ; i < $scope.CountryPhoneCodes.length ; i++) {
                if ($scope.CountryPhoneCodes[i].CountryCode === Country.Code) {
                    $scope.ShipFromPhoneCode = "(+" + $scope.CountryPhoneCodes[i].PhoneCode + ")";
                    break;
                }
            }

            setShipFromStatePostCodeForHKGUK(Country);
            if (Action !== undefined && Action !== null && Action !== '') {
                //$scope.ShipFromPhoneCode = '';
                $scope.directBooking.ShipFrom = {
                    AddressBookId: 0,
                    Country: Country,
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
                    IsMailSend: false
                };
            }

        }

    };

    var setShipFromStatePostCodeForHKGUK = function (Country) {
        if (Country.Code === 'HKG') {
            $scope.directBooking.ShipFrom.PostCode = null;
            $scope.directBooking.ShipFrom.State = null;
        }
        else if (Country.Code === 'GBR') {
            $scope.directBooking.ShipFrom.State = null;
        }
    };
    var setShipToStatePostCodeForHKGUK = function (Country) {
        if (Country.Code === 'HKG') {
            $scope.directBooking.ShipTo.PostCode = null;
            $scope.directBooking.ShipTo.State = null;
        }
        else if (Country.Code === 'GBR') {
            $scope.directBooking.ShipTo.State = null;
        }
    };
    // Set ShipTo info
    $scope.SetShipToInfo = function (Country, Action) {
        if (Country !== undefined && Country !== null) {
            $scope.showPostCodeDropDown1 = false;
            for (var i = 0 ; i < $scope.CountryPhoneCodes.length ; i++) {
                if ($scope.CountryPhoneCodes[i].Name === Country.Name) {
                    $scope.ShipToPhoneCode = "(+" + $scope.CountryPhoneCodes[i].PhoneCode + ")";
                    break;
                }
            }
            setShipToStatePostCodeForHKGUK(Country);
        }
        if (Action !== undefined && Action !== null && Action !== '') {
            //$scope.ShipToPhoneCode = '';
            $scope.directBooking.ShipTo = {
                AddressBookId: 0,
                Country: Country,
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
                IsMailSend: false
            };

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
        if ($scope.CustDetail !== null && $scope.CustDetail !== undefined && $scope.CustDetail.CustomerId !== null) {
            $scope.directBooking.CustomerId = $scope.CustDetail.CustomerId;
        }


        if ($scope.directBooking !== null && ($scope.directBooking.CustomerId === undefined || $scope.directBooking.CustomerId === null || $scope.directBooking.CustomerId === 0 || $scope.directBooking.CustomerId === "0" || $scope.customerId === "")) {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.SelectCustomerAddressBookValidation,
                showCloseButton: true
            });
            return;
        }

        $scope.customerId = $scope.directBooking.CustomerId;
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBooking/directBookingAddEdit.tpl.html',
            controller: 'DirectBookingAddEditController',
            windowClass: 'DirectBooking-Modal',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                moduleType: function () {
                    return "DirectBooking";
                },
                toCountryId: function () {
                    if ($scope.directBooking.ShipTo != null && $scope.directBooking.ShipTo.Country !== null && $scope.directBooking.ShipTo.Country.CountryId) {
                        return $scope.directBooking.ShipTo.Country.CountryId;
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
            if (addressBooks !== undefined && addressBooks !== null && addressBooks.length > 0) {
                $scope.directBooking.ShipFrom = addressBooks[0].entity;
                $scope.directBooking.ShipFrom.Country = addressBooks[0].entity.Country;
                $scope.directBooking.ShipFrom.AddressBookId = 0;


                $scope.SetShipFrominfo($scope.directBooking.ShipFrom.Country);
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
                $scope.directBooking.TaxAndDutiesAcceptedBy = taxAndDutyName;
            }

        }, function () {
            $scope.directBooking.PayTaxAndDuties = '';
        });
    };
    $scope.directBookingAddEditShipToAddress = function () {
        if ($scope.directBooking !== null && ($scope.directBooking.CustomerId === undefined || $scope.directBooking.CustomerId === null || $scope.directBooking.CustomerId === 0 || $scope.directBooking.CustomerId === "0" || $scope.customerId === "")) {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.SelectCustomerAddressBookValidation,
                showCloseButton: true
            });
            return;
        }
        $scope.customerId = $scope.directBooking.CustomerId;
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBooking/directBookingAddEdit.tpl.html',
            controller: 'DirectBookingAddEditController',
            windowClass: 'DirectBooking-Modal',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                moduleType: function () {
                    return "DirectBooking";
                },
                toCountryId: function () {
                    if ($scope.directBooking.ShipTo != null && $scope.directBooking.ShipTo.Country !== null && $scope.directBooking.ShipTo.Country.CountryId) {
                        return $scope.directBooking.ShipTo.Country.CountryId;
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
            if (addressBooks !== undefined && addressBooks !== null && addressBooks.length > 0) {
                $scope.directBooking.ShipTo = addressBooks[0].entity;
                $scope.SetShipToInfo($scope.directBooking.ShipTo.Country);
                $scope.directBooking.ShipTo.AddressBookId = 0;
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
            $scope.NewDirectBooking();
            $scope.directBooking.CustomerId = CustomerDetail.CustomerId;
            $scope.CustDetail = CustomerDetail;
            $scope.CustomerDetail = CustomerDetail;
            DirectBookingService.GetCustomerDetail(CustomerDetail.CustomerId).then(function (response) {
                if (response.data !== null) {
                    if (response.data.IsShipperTaxAndDuty) {
                        $scope.taxAndDutyDisabled = false;
                    }
                    else {
                        $scope.taxAndDutyDisabled = true;
                        $scope.directBooking.PayTaxAndDuties = '';
                        $scope.directBooking.TaxAndDutiesAcceptedBy = '';
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
                        $scope.directBooking.Currency = found[0];
                    }
                }
            }, function () {

            });
          
        }

    };

    $scope.FillAddress = function (PostCode) {
        PostCode.Address1 = PostCode.Address1 === null ? $scope.directBooking.ShipFrom.Address = '' : $scope.directBooking.ShipFrom.Address = PostCode.Address1;
        PostCode.Address2 = PostCode.Address2 === null ? $scope.directBooking.ShipFrom.Address2 = '' : $scope.directBooking.ShipFrom.Address2 = PostCode.Address2;
        PostCode.City = PostCode.City === null ? $scope.directBooking.ShipFrom.City = '' : $scope.directBooking.ShipFrom.City = PostCode.City;
        PostCode.Company = PostCode.CompanyName === null ? $scope.directBooking.ShipFrom.CompanyName = '' : $scope.directBooking.ShipFrom.CompanyName = PostCode.CompanyName;
        PostCode.Area = PostCode.Area === null ? $scope.directBooking.ShipFrom.Area = '' : $scope.directBooking.ShipFrom.Area = PostCode.Area;
    };

    $scope.$watch('directBooking.ShipFrom.PostCode', function (PostCode) {
        if (PostCode !== null && PostCode !== undefined && PostCode !== '') {
            if (PostCode.PostCode === undefined || PostCode.PostCode === null) {
                $scope.directBooking.ShipFrom.PostCode = PostCode;
            }
            else {
                $scope.directBooking.ShipFrom.PostCode = PostCode.PostCode;
                PostCode.Address1 = PostCode.Address1 === null ? $scope.directBooking.ShipFrom.Address = '' : $scope.directBooking.ShipFrom.Address = PostCode.Address1;
                PostCode.Address2 = PostCode.Address2 === null ? $scope.directBooking.ShipFrom.Address2 = '' : $scope.directBooking.ShipFrom.Address2 = PostCode.Address2;
                PostCode.City = PostCode.City === null ? $scope.directBooking.ShipFrom.City = '' : $scope.directBooking.ShipFrom.City = PostCode.City;
                PostCode.Company = PostCode.CompanyName === null ? $scope.directBooking.ShipFrom.CompanyName = '' : $scope.directBooking.ShipFrom.CompanyName = PostCode.CompanyName;
                PostCode.Area = PostCode.Area === null ? $scope.directBooking.ShipFrom.Area = '' : $scope.directBooking.ShipFrom.Area = PostCode.Area;
            }

        }
    });

    // GetPostCodeAddress call
    $scope.GetPostCodeAddress = function (PostCode) {
        var pcode = PostCode.split('');

        for (i = 0; i < pcode.length; i++) {
            if (pcode[i] === " ") {
                pcode.splice(i, 1);
            }
        }
        if (pcode.length >= 0 && $scope.directBooking.ShipFrom.Country !== null && $scope.directBooking.ShipFrom.Country !== undefined && $scope.directBooking.ShipFrom.Country.Code === 'GBR') {
            return DirectBookingService.GetPostCodeAddress(PostCode, $scope.directBooking.ShipFrom.Country.Code2).then(function (response) {
                if (response.data.length > 0) {
                    for (i = 0; i < response.data.length; i++) {
                        if (response.data[i].City === null || response.data[i].City === '') {
                            response.data[i].City = '';
                        }
                        if (response.data[i].Address1 === null || response.data[i].Address1 === '') {
                            response.data[i].Address1 = '';
                        }
                        if (response.data[i].Address2 === null || response.data[i].Address2 === '') {
                            response.data[i].Address2 = '';
                        }
                        if (response.data[i].CompanyName === null || response.data[i].CompanyName === '') {
                            response.data[i].CompanyName = '';
                        }
                        if (response.data[i].Area === null || response.data[i].Area === '') {
                            response.data[i].Area = '';
                        }
                        if (response.data[i].Address1 !== '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].Area !== '' && response.data[i].CompanyName !== '') {
                            response.data[i].FillPostCodeInput = response.data[i].Address1 + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].CompanyName + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '') {
                            response.data[i].FillPostCodeInput = response.data[i].Address2 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 !== '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].Area === '') {
                            response.data[i].FillPostCodeInput = response.data[i].Address1 +', '+response.data[i].Address2 + ', ' + response.data[i].City + ', ' + response.data[i].CompanyName + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 !== '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '') {
                            response.data[i].FillPostCodeInput = response.data[i].Address1 +', '+response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 !== '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '' && response.data[i].Area === '') {
                            response.data[i].FillPostCodeInput = response.data[i].Address1 +', ' + response.data[i].Address2 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName !== '' && response.data[i].Area === '') {
                            response.data[i].FillPostCodeInput = response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].CompanyName + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '' && response.data[i].Area !== '') {
                            response.data[i].FillPostCodeInput = response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '' && response.data[i].Area === '') {
                            response.data[i].FillPostCodeInput = response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].CompanyName + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 !== '' && response.data[i].Address2 === '' && response.data[i].City !== '' && response.data[i].PostCode !== '') {
                            response.data[i].FillPostCodeInput = response.data[i].Address1 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 === '' && response.data[i].Address2 === '' && response.data[i].City !== '' && response.data[i].PostCode !== '') {
                            response.data[i].FillPostCodeInput = response.data[i].City + ', ' + response.data[i].PostCode;

                        }

                    }
                    $scope.PostCodeAddressValue = false;
                    $scope.fillpostval = response.data;
                    return response.data;
                    


                }
                else {
                    $scope.PostCodeAddressValue = true;
                }
            });
        }


    };


    $scope.ShiperFillAddress = function (ShipperPostCodeAdress) {
        ShipperPostCodeAdress.Address1 = ShipperPostCodeAdress.Address1 === null ? $scope.directBooking.ShipTo.Address = '' : $scope.directBooking.ShipTo.Address = ShipperPostCodeAdress.Address1;
        ShipperPostCodeAdress.Address2 = ShipperPostCodeAdress.Address2 === null ? $scope.directBooking.ShipTo.Address2 = '' : $scope.directBooking.ShipTo.Address2 = ShipperPostCodeAdress.Address2;
        ShipperPostCodeAdress.City = ShipperPostCodeAdress.City === null ? $scope.directBooking.ShipTo.City = '' : $scope.directBooking.ShipTo.City = ShipperPostCodeAdress.City;
    };

    $scope.$watch('directBooking.ShipTo.PostCode', function (ShipperPostCodeAdress) {
        if (ShipperPostCodeAdress !== null && ShipperPostCodeAdress !== undefined && ShipperPostCodeAdress !== '') {
            if (ShipperPostCodeAdress.PostCode === undefined || ShipperPostCodeAdress.PostCode === null) {
                $scope.directBooking.ShipTo.PostCode = ShipperPostCodeAdress;
            }
            else {
                $scope.directBooking.ShipTo.PostCode = ShipperPostCodeAdress.PostCode;
                ShipperPostCodeAdress.Address1 = ShipperPostCodeAdress.Address1 === null ? $scope.directBooking.ShipTo.Address = '' : $scope.directBooking.ShipTo.Address = ShipperPostCodeAdress.Address1;
                ShipperPostCodeAdress.Address2 = ShipperPostCodeAdress.Address2 === null ? $scope.directBooking.ShipTo.Address2 = '' : $scope.directBooking.ShipTo.Address2 = ShipperPostCodeAdress.Address2;
                ShipperPostCodeAdress.City = ShipperPostCodeAdress.City === null ? $scope.directBooking.ShipTo.City = '' : $scope.directBooking.ShipTo.City = ShipperPostCodeAdress.City;
                ShipperPostCodeAdress.Company = ShipperPostCodeAdress.CompanyName === null ? $scope.directBooking.ShipTo.CompanyName = '' : $scope.directBooking.ShipTo.CompanyName = ShipperPostCodeAdress.CompanyName;
                ShipperPostCodeAdress.Area = ShipperPostCodeAdress.Area === null ? $scope.directBooking.ShipTo.Area = '' : $scope.directBooking.ShipTo.Area = ShipperPostCodeAdress.Area;
            }

        }
    });

    $scope.ShiperGetPostCodeAddress = function (ShipperPostCodeAdress) {
        var pcode = ShipperPostCodeAdress.split('');

        for (i = 0; i < pcode.length; i++) {
            if (pcode[i] === " ") {
                pcode.splice(i, 1);
            }
        }
        if (pcode.length >= 0 && $scope.directBooking.ShipTo.Country !== null && $scope.directBooking.ShipTo.Country !== undefined && $scope.directBooking.ShipTo.Country.Code === 'GBR') {
            return DirectBookingService.GetPostCodeAddress(ShipperPostCodeAdress, $scope.directBooking.ShipTo.Country.Code2).then(function (response) {
                if (response.data.length > 0) {
                    //for (i = 0; i < response.data.length; i++) {
                    //    if (response.data[i].City === null || response.data[i].City === '') {
                    //        response.data[i].City = '';
                    //    }
                    //    if (response.data[i].Address1 === null || response.data[i].Address1 === '') {
                    //        response.data[i].Address1 = '';
                    //    }
                    //    if (response.data[i].Address2 === null || response.data[i].Address2 === '') {
                    //        response.data[i].Address2 = '';
                    //    }
                    //    if (response.data[i].Address1 !== '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '') {
                    //        response.data[i].FillPostCodeInput = response.data[i].Address1 + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;

                    //    }
                    //    if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '') {
                    //        response.data[i].FillPostCodeInput = response.data[i].Address2 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;

                    //    }
                    //    if (response.data[i].Address1 !== '' && response.data[i].Address2 === '' && response.data[i].City !== '' && response.data[i].PostCode !== '') {
                    //        response.data[i].FillPostCodeInput = response.data[i].Address1 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;

                    //    }
                    //    if (response.data[i].Address1 === '' && response.data[i].Address2 === '' && response.data[i].City !== '' && response.data[i].PostCode !== '') {
                    //        response.data[i].FillPostCodeInput = response.data[i].City + ', ' + response.data[i].PostCode;

                    //    }
                    //}
                    for (i = 0; i < response.data.length; i++) {
                        if (response.data[i].City === null || response.data[i].City === '') {
                            response.data[i].City = '';
                        }
                        if (response.data[i].Address1 === null || response.data[i].Address1 === '') {
                            response.data[i].Address1 = '';
                        }
                        if (response.data[i].Address2 === null || response.data[i].Address2 === '') {
                            response.data[i].Address2 = '';
                        }
                        if (response.data[i].CompanyName === null || response.data[i].CompanyName === '') {
                            response.data[i].CompanyName = '';
                        }
                        if (response.data[i].Area === null || response.data[i].Area === '') {
                            response.data[i].Area = '';
                        }
                        if (response.data[i].Address1 !== '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].Area !== '' && response.data[i].CompanyName !== '') {
                            response.data[i].FillPostCodeInput = response.data[i].Address1 + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].CompanyName + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '') {
                            response.data[i].FillPostCodeInput = response.data[i].Address2 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 !== '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].Area === '') {
                            response.data[i].FillPostCodeInput = response.data[i].Address1 + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ', ' + response.data[i].CompanyName + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 !== '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '') {
                            response.data[i].FillPostCodeInput = response.data[i].Address1 + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 !== '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '' && response.data[i].Area === '') {
                            response.data[i].FillPostCodeInput = response.data[i].Address1 + ', ' + response.data[i].Address2 + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName !== '' && response.data[i].Area === '') {
                            response.data[i].FillPostCodeInput = response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].CompanyName + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '' && response.data[i].Area !== '') {
                            response.data[i].FillPostCodeInput = response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '' && response.data[i].Area === '') {
                            response.data[i].FillPostCodeInput = response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].CompanyName + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 !== '' && response.data[i].Address2 === '' && response.data[i].City !== '' && response.data[i].PostCode !== '') {
                            response.data[i].FillPostCodeInput = response.data[i].Address1 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 === '' && response.data[i].Address2 === '' && response.data[i].City !== '' && response.data[i].PostCode !== '') {
                            response.data[i].FillPostCodeInput = response.data[i].City + ', ' + response.data[i].PostCode;

                        }

                    }
                    return response.data;

                }
                else {
                    $scope.ShipperPostCodeAddressValue = true;
                }
            });
        }
    };

    $scope.SetService = function (data) {
        var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        //$scope.directBooking.CustomerRateCard.ShipmentType = data.CustomerRateCard.WeightType;
        if (data.CustomerRateCard.LogisticType === 'UKShipment' && data.CustomerRateCard.CourierName === 'UKMail') {
            //$scope.ukShipmentService.IsUkShipment = true;
            $scope.directBooking.PakageCalculatonType = data.CustomerRateCard.PackageCalculationType;
            $scope.directBooking.ShipFrom.Country = data.ShipFrom.Country;
            $scope.directBooking.ShipTo.Country = data.ShipTo.Country;
            $scope.directBooking.CustomerRateCard.ImageURL = config.BUILD_URL + "UKmail.png";
            $scope.directBooking.CustomerRateCard.ShipmentType = data.CustomerRateCard.WeightType + " ( " + data.CustomerRateCard.ParcelServiceType + " ) ";
            //var Ukmaildate = new Date(data.CustomerRateCard.FuelMonth);
            //var Ukmaildate1 = Ukmaildate.getMonth();
            //var UkmailMonth = months[Ukmaildate1];
            $scope.directBooking.CustomerRateCard.FuelMonth = data.CustomerRateCard.FuelMonth;
            $scope.directBooking.CustomerRateCard.FuelSurcharge = data.CustomerRateCard.FuelSurchargePercent;
        }
        else if (data.CustomerRateCard.LogisticType === 'UKShipment' && data.CustomerRateCard.CourierName === 'Yodel') {
            $scope.directBooking.PakageCalculatonType = data.CustomerRateCard.PackageCalculationType;
            $scope.directBooking.ShipFrom.Country = data.ShipFrom.Country;
            $scope.directBooking.ShipTo.Country = data.ShipTo.Country;
            //$scope.ukShipmentService.IsUkShipment = true;
            $scope.directBooking.CustomerRateCard.ImageURL = config.BUILD_URL + "yodel.png";
            $scope.directBooking.CustomerRateCard.ShipmentType = data.CustomerRateCard.WeightType + " ( " + data.CustomerRateCard.ParcelServiceType + " ) ";
            //var Yodeldate = new Date(data.CustomerRateCard.FuelMonth);
            //var Yodeldate1 = Yodeldate.getMonth();
            //var YodelMonth = months[Yodeldate1];
            $scope.directBooking.CustomerRateCard.FuelMonth = data.CustomerRateCard.FuelMonth;
            $scope.directBooking.CustomerRateCard.FuelSurcharge = data.CustomerRateCard.FuelSurchargePercent;
        }
        else if (data.CustomerRateCard.LogisticType === 'UKShipment' && data.CustomerRateCard.CourierName === 'Hermes') {
            $scope.directBooking.PakageCalculatonType = data.CustomerRateCard.PackageCalculationType;
            $scope.directBooking.ShipFrom.Country = data.ShipFrom.Country;
            $scope.directBooking.ShipTo.Country = data.ShipTo.Country;
            //$scope.ukShipmentService.IsUkShipment = true;
            $scope.directBooking.CustomerRateCard.ImageURL = config.BUILD_URL + "hermes.png";
            $scope.directBooking.CustomerRateCard.ShipmentType = data.CustomerRateCard.WeightType + " ( " + data.CustomerRateCard.ParcelServiceType + " ) ";
            //var Hermesdate = new Date(data.CustomerRateCard.FuelMonth);
            //var Hermesdate1 = Hermesdate.getMonth();
            //var HermesMonth = months[Hermesdate1];
            $scope.directBooking.CustomerRateCard.FuelMonth = data.CustomerRateCard.FuelMonth;
            $scope.directBooking.CustomerRateCard.FuelSurcharge = data.CustomerRateCard.FuelSurchargePercent;
        }
        else if (data.CustomerRateCard.CourierName === 'DHL') {
            $scope.directBooking.PakageCalculatonType = data.CustomerRateCard.PackageCalculationType;
            $scope.directBooking.ShipFrom.Country = data.ShipFrom.Country;
            $scope.directBooking.ShipTo.Country = data.ShipTo.Country;
            $scope.directBooking.CustomerRateCard.ImageURL = config.BUILD_URL + "DHL.png";
            //var DHLdate = new Date(data.CustomerRateCard.FuelMonth);
            //var DHLdate1 = DHLdate.getMonth();
            //var DHLMonth = months[DHLdate1];
            $scope.directBooking.CustomerRateCard.FuelMonth = data.CustomerRateCard.FuelMonth;
            if ($scope.ShowCustomInfoSection($scope.directBooking) && $scope.directBooking.CustomerRateCard.LogisticServiceId > 0) {
                $scope.active = 2;
            }
            $scope.directBooking.CustomerRateCard.FuelSurcharge = data.CustomerRateCard.FuelSurchargePercent;
        }
        else if (data.CustomerRateCard.CourierName === 'FedEx') {
            $scope.directBooking.PakageCalculatonType = data.CustomerRateCard.PackageCalculationType;
            $scope.directBooking.ShipFrom.Country = data.ShipFrom.Country;
            $scope.directBooking.ShipTo.Country = data.ShipTo.Country;
            $scope.directBooking.CustomerRateCard.ImageURL = config.BUILD_URL + "FedEx.png";
            //var FedExdate = new Date(data.CustomerRateCard.FuelMonth);
            //var FedExdate1 = FedExdate.getMonth();
            //var FedExMonth = months[FedExdate1];
            $scope.directBooking.CustomerRateCard.FuelMonth = data.CustomerRateCard.FuelMonth;
            $scope.directBooking.CustomerRateCard.FuelSurcharge = data.CustomerRateCard.FuelSurchargePercent;
            if ($scope.ShowCustomInfoSection($scope.directBooking) && $scope.directBooking.CustomerRateCard.LogisticServiceId > 0) {
                $scope.active = 2;
            }
        }
        else if (data.CustomerRateCard.CourierName === 'TNT') {
            $scope.directBooking.PakageCalculatonType = data.CustomerRateCard.PackageCalculationType;
            $scope.directBooking.ShipFrom.Country = data.ShipFrom.Country;
            $scope.directBooking.ShipTo.Country = data.ShipTo.Country;
            $scope.directBooking.CustomerRateCard.ImageURL = config.BUILD_URL + "TNT.png";
            //var TNTdate = new Date(data.CustomerRateCard.FuelMonth);
            //var TNTdate1 = TNTdate.getMonth();
            //var TNTMonth = months[TNTdate1];
            $scope.directBooking.CustomerRateCard.FuelMonth = data.CustomerRateCard.FuelMonth;
            $scope.directBooking.CustomerRateCard.FuelSurcharge = data.CustomerRateCard.FuelSurchargePercent;
            if ($scope.ShowCustomInfoSection($scope.directBooking) && $scope.directBooking.CustomerRateCard.LogisticServiceId > 0) {
                $scope.active = 2;
            }
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
        $scope.checkcourier = '';
        $scope.checkcustomerRateCard = null;
        $scope.ImagePath = config.BUILD_URL;
        AppSpinner.hideSpinnerTemplate();
        $scope.active = 1;
        $scope.BookingType = "";
        $scope.htmlPopover = $sce.trustAsHtml('<b style="color: red">I can</b> have <div class="label label-success">HTML</div> content');
        $scope.CountryPopOver = $sce.trustAsHtml('<span style : "width : 269px">China (CN)1 covers the following<br/>Region: Guangdong Province<br/>China (CN)2 covers the following <br/>Region: Other Provinces <span>');
        $scope.ConsineeNotification = $sce.trustAsHtml("Ticking this will send the newly created shipment detail to the provided Consignee email.");
        $scope.ShipperNotification = $sce.trustAsHtml("Ticking this will send the newly created shipment detail to the provided shipper email.");
        $rootScope.manageDirectBookingChange = true;
        $scope.CustomerDetail = null;
        $scope.submitted = true;
        $rootScope.directBookingChange = true;
        $scope.blinkCustomInfo = true;
        $anchorScroll.yOffset = 200;
        $scope.disableCourier = false;
        $scope.isCollectionTimeValid = true;
        $scope.parcelTypeDisable = false;
        $scope.status = {
            opened: false
        };
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
        $scope.photoHazard = config.BUILD_URL + "Hazard_logo.png";
        $scope.ShipmentStatus = {
            Current: 12,
            Past: 13,
            Draft: 14
        };
        $scope.CurrencyTypes = [];
        new SetMultilingualOtions();

        var userInfo = SessionService.getUser();

        $scope.tabs = userInfo.tabs;
        $scope.tab = getUserTab($scope.tabs, "QuickDirect_Booking");
        $scope.RoleId = userInfo.RoleId;
        $scope.custid = userInfo.EmployeeId;
        $scope.CreatedBy = userInfo.EmployeeId;
        if ($scope.RoleId !== 3) { // Need to select customer from dropdown in case Login (other than customer)
            $scope.customerId = 0;

            $scope.paymentAccount = true;
            DirectBookingService.GetDirectBookingCustomers(userInfo.EmployeeId, "DirectBooking").then(function (response) {
                $scope.directBookingCustomers = response.data;
                var dbCustomers = [];
                for (i = 0; i < $scope.directBookingCustomers.length; i++) {

                    //if ($scope.directBookingCustomers[i].OperationZoneId === userInfo.OperationZoneId) {
                    //    dbCustomers.push($scope.directBookingCustomers[i]);
                    //}

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
                //$scope.directBookingCustomers = dbCustomers;

            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
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
        $scope.NewDirectBooking();
        $scope.directBooking.CustomInfo.RestrictionType = $scope.RestrictionType[0];
        $scope.directBooking.CustomInfo.RestrictionComments = 'N/A';

        //Step 1: Get initials and default address of logged in customer.
        DirectBookingService.GetInitials($scope.customerId).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            // Set Country type according to given order
            $scope.CountriesRepo = TopCountryService.TopCountryList(response.data.Countries);
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

            var directShipmentId = 0;
            if ($stateParams.directShipmentId) {
                directShipmentId = parseInt($stateParams.directShipmentId, 10);
            }

            //$scope.qs = $stateParams.callingtype;


            if (directShipmentId > 0) {
                var CallingType = '';
                if ($stateParams.callingtype === 'quotation-shipment') {
                    CallingType = 'quotation-shipment';
                }
                else {

                    if ($scope.tab !== undefined && $scope.tab !== null && $scope.tab.childTabs !== null) {

                        CallingType = UtilityService.GetCurrentShipmentType($scope.tab);

                    }
                }


                DirectBookingService.GetShipmentDraftDetail(directShipmentId, CallingType).then(function (response) {

                    $scope.directBooking = response.data;
                    $scope.checkcustomerRateCard = response.data.CustomerRateCard;
                    if ($scope.directBooking.ShipmentStatusId === 14) {
                        $scope.checkcustomerRateCard = response.data.CustomerRateCard;
                        $scope.checkcourier = '';

                        //$scope.directBooking.CustomerRateCard.LogisticServiceId = 0;
                    }
                    else {

                        $scope.checkcourier = response.data.CustomerRateCard.CourierName;
                    }
                    var dbpac = $scope.directBooking.Packages.length - 1;
                    for (i = 0; i < $scope.directBooking.Packages.length; i++) {

                        if (i === dbpac) {
                            $scope.directBooking.Packages[i].pacVal = true;
                        }
                        else {
                            $scope.directBooking.Packages[i].pacVal = false;
                        }
                    }

                    $scope.directBooking.PayTaxAndDuties = 'Receiver';
                    if (CallingType === 'quotation-shipment') {

                        response.data.CustomerRateCard.Rate = Number(parseFloat(response.data.CustomerRateCard.Rate).toFixed(2)).toLocaleString('en', { minimumFractionDigits: 2 });
                        response.data.CustomerRateCard.FuelSurcharge = response.data.CustomerRateCard.FuelSurcharge.toFixed(2);
                        response.data.CustomerRateCard.TotalEstimatedCharge = Number(parseFloat(response.data.CustomerRateCard.TotalEstimatedCharge).toFixed(2)).toLocaleString('en', { minimumFractionDigits: 2 });
                        response.data.CustomerRateCard.AdditionalSurcharge = Number(parseFloat(response.data.CustomerRateCard.AdditionalSurcharge).toFixed(2)).toLocaleString('en', { minimumFractionDigits: 2 });
                        response.data.CustomerRateCard.FuelSurchargePercent = response.data.CustomerRateCard.FuelSurchargePercent.toFixed(2);

                        $scope.SetService(response.data);
                    }

                    if ($scope.directBooking && $scope.directBooking.CustomerRateCard != null && ($scope.directBooking.CustomerRateCard.CourierName !== '' || $scope.directBooking.CustomerRateCard.CourierName !== undefined)) {
                        if ($scope.directBooking.CustomerRateCard.CourierName === 'DHL' || $scope.directBooking.CustomerRateCard.CourierName === 'Yodel') {
                            $scope.MaxLengthValue = 26;
                        }
                        else if ($scope.directBooking.CustomerRateCard.CourierName === 'UKMail' || $scope.directBooking.CustomerRateCard.CourierName === 'Hermes') {
                            $scope.MaxLengthValue = 11;
                        }
                    }
                    if (($scope.RoleId !==3) && $scope.directBookingCustomers.length) {
                        for (i = 0; i < $scope.directBookingCustomers.length; i++) {
                            if ($scope.directBooking.CustomerId === $scope.directBookingCustomers[i].CustomerId) {
                                $scope.CustomerDetail = $scope.directBookingCustomers[i];
                            }
                        }
                    }
                    var fromAddress = response.data.ShipFrom;
                    var toAddress = response.data.ShipTo;

                    if (CallingType === "SipmentReturn") {
                        $scope.directBooking.ShipFrom = toAddress;
                        $scope.directBooking.ShipTo = fromAddress;
                    }

                    $scope.changeKgToLb($scope.directBooking.PakageCalculatonType);
                    // Set ShipFrom Phone Code
                    $scope.SetShipFrominfo($scope.directBooking.ShipFrom.Country);

                    // Set ShipTo PhoneCode
                    $scope.SetShipToInfo($scope.directBooking.ShipTo.Country);

                    var parcel = $filter('filter')($scope.ParcelTypes, { ParcelType: $scope.directBooking.ParcelType.ParcelType });
                    if (parcel.length) {
                        $scope.directBooking.ParcelType = parcel[0];
                    }
                    if ($scope.directBooking.ReferenceDetail.CollectionDate !== undefined &&
                        $scope.directBooking.ReferenceDetail.CollectionDate !== null) {
                        $scope.directBooking.ReferenceDetail.CollectionDate = moment.utc($scope.directBooking.ReferenceDetail.CollectionDate).toDate();
                    }

                    //if ($scope.directBooking.CustomerRateCard !== null && $scope.directBooking.CustomerRateCard.CourierId > 0) {
                    //    var found = $filter('filter')($scope.CurrencyTypes, { ShipmentMethodId: $scope.directBooking.CustomerRateCard.CourierId });
                    //    if (found.length) {
                    //        for (var ac = 0; ac < $scope.ShipmentMethods.length; ac++) {
                    //            if ($scope.directBooking.CustomerRateCard.CourierId === $scope.ShipmentMethods[ac].ShipmentMethodId) {
                    //                $scope.directBooking.ShipmentMethod = $scope.ShipmentMethods[ac];
                    //                break;
                    //            }
                    //        }


                    //    }
                    //    else {
                    //        if ($scope.directBooking.ShippingMethodId > 0) {
                    //            if ($scope.ShipmentMethods.length > 0) {
                    //                for (var ab = 0; ab < $scope.ShipmentMethods.length; ab++) {
                    //                    if ($scope.directBooking.ShippingMethodId === $scope.ShipmentMethods[ab].ShipmentMethodId) {
                    //                        $scope.directBooking.ShipmentMethod = $scope.ShipmentMethods[ab];
                    //                        break;
                    //                    }
                    //                }
                    //            }

                    //        }
                    //    }
                    //}
                    //else {
                    //    if ($scope.directBooking.ShippingMethodId > 0) {
                    //        if ($scope.ShipmentMethods.length > 0) {
                    //            for (var aa = 0; aa < $scope.ShipmentMethods.length; aa++) {
                    //                if ($scope.directBooking.ShippingMethodId === $scope.ShipmentMethods[aa].ShipmentMethodId) {
                    //                    $scope.directBooking.ShipmentMethod = $scope.ShipmentMethods[aa];
                    //                    break;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    //If page is called from 'Clone' or 'Create Return' functionality then on save system will create new shpment.
                    if (CallingType === "ShipmentClone" || CallingType === "ShipmentReturn") {
                        $scope.directBooking.DirectShipmentId = 0;
                        $scope.directBooking.ShipFrom.DirectShipmentAddressId = 0;
                        $scope.directBooking.ShipTo.DirectShipmentAddressId = 0;
                        $scope.directBooking.CustomInfo.ShipmentCustomDetailId = 0;
                        $scope.directBooking.ShipFrom.Email = '';
                        $scope.directBooking.ShipTo.Email = '';
                        $scope.directBooking.CustomerRateCard = {
                            ZoneRateCardId: 0,
                            WeightType: '',
                            CourierId: 0,
                            LogisticServiceId: 0,
                            CourierName: '',
                            CourierAccountNo: '',
                            CourierDescription: '',
                            IntegrationAccountId: 0
                        };
                        if ($scope.directBooking.Packages !== undefined && $scope.directBooking.Packages !== null && $scope.directBooking.Packages.length > 0) {
                            for (var i = 0; i < $scope.directBooking.Packages.length; i++) {
                                $scope.directBooking.Packages[i].DirectShipmentDetailId = 0;
                            }
                        }
                    }
                },
                function () {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteWarning,
                        body: $scope.ReceiveDetail_Validation,
                        showCloseButton: true
                    });
                });
            }
            else {
                $scope.NewDirectBooking();
                if ($scope.RoleId === 3) {
                    $scope.directBooking.CustomerId = $scope.customerId;
                }
                $scope.directBooking.OpearionZoneId = $scope.OperationZoneId;
                $scope.changeKgToLb($scope.directBooking.PakageCalculatonType);
                $scope.directBooking.DirectShipmentId = 0;
                $scope.directBooking.ShipmentMethod = $scope.ShipmentMethods[0];

                if ($scope.directBooking.ReferenceDetail.CollectionDate !== undefined &&
                        $scope.directBooking.ReferenceDetail.CollectionDate !== null) {
                    $scope.directBooking.ReferenceDetail.CollectionDate = moment.utc($scope.directBooking.ReferenceDetail.CollectionDate).toDate();
                }

                if ($scope.CustomerDetail !== undefined && $scope.CustomerDetail !== null && $scope.CustomerDetail.CurrencyCode !== null && $scope.CustomerDetail.CurrencyCode !== '') {
                    var found = $filter('filter')($scope.CurrencyTypes, { CurrencyCode: $scope.CustomerDetail.CurrencyCode });
                    if (found.length) {
                        $scope.directBooking.Currency = found[0];
                    }
                }


                for (var pa = 0 ; pa < $scope.ParcelTypes.length; pa++) {
                    if ($scope.ParcelTypes[pa].ParcelDescription === "Parcel (Non Doc)") {
                        $scope.directBooking.ParcelType = $scope.ParcelTypes[pa];
                        break;
                    }
                }
                //var found1 = $filter('filter')($scope.ParcelTypes, { ParcelDescription: "Parcel (Non Doc)" });
                //if (found1.length) {
                //    $scope.directBooking.ParcelType = found1[0];
                //}

                $scope.SetCustomerAddress();
                // Set ShipFrom Phone Code
                $scope.SetShipFrominfo($scope.directBooking.ShipFrom.Country);
            }

        },
       function () {
           AppSpinner.hideSpinnerTemplate();
           toaster.pop({
               type: 'error',
               title: $scope.TitleFrayteError,
               body: $scope.InitialDataValidation,
               showCloseButton: true
           });
       });
        $rootScope.GetServiceValue = '';
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $rootScope.QuoteCustomerratecard = {};
        if ($rootScope.QuoteCustomerratecard !== null && $rootScope.QuoteCustomerratecard !== undefined) {
            $scope.directBooking.CustomerRateCard = $rootScope.QuoteCustomerratecard;
        }

        $scope.setScroll('top');
        $anchorScroll.yOffset = 700;
    }

    init();
});
