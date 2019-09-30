angular.module('ngApp.directBooking').controller('DirectBookingController', function ($scope, $sce, $uibModalStack, UtilityService, Upload, $rootScope, AppSpinner, TopCountryService, $location, TopCurrencyService, config, $filter, $state, ModalService, $translate, $uibModal, $timeout, toaster, $stateParams, DirectBookingService, SessionService, TimeStringtoDateTime) {

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

    $scope.SetValueNotZero = function (Value) {
        if (Value !== undefined && Value !== null && Value === "0") {
            return;
        }
        return Value;
    };

    $scope.SetMaxixumWeight = function (Weight, CartoonValue, pieceDetailOption, directBooking) {
        if (pieceDetailOption === "kgToCms") {
            if (Weight) {
                $scope.isdisabled = true;
            }
            else {
                if (directBooking.Packages.length > 1) {
                    var isWegt = false;
                    angular.forEach(directBooking.Packages, function (value, key) {

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
            if (parseInt(Weight, 10) > 70) {
                //$scope.Package.Weight = '';
                var finalWeightkg = '';
                var weightkg = Weight.toString().split('');
                var weightkgnew = weightkg.splice(weightkg.length - 1);
                for (i = 0; i < weightkg.length; i++) {
                    finalWeightkg = finalWeightkg + weightkg[i];
                }
                Weight = finalWeightkg;
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.MaxWeight70kGS,
                    showCloseButton: true
                });
            }
            return Weight;
        }
        if (pieceDetailOption === "lbToInchs") {

            if (Weight) {
                $scope.isdisabled = true;

            }
            else {
                if (directBooking.Packages.length > 1) {
                    var isWegtlbs = false;
                    angular.forEach(directBooking.Packages, function (value, key) {

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
            if (parseInt(Weight, 10) > 154) {

                //$scope.Package.Weight = '';
                var finalWeightlb = '';
                var weightlbs = Weight.toString().split('');
                var weightlbsnew = weightlbs.splice(weightlbs.length - 1);
                for (i = 0; i < weightlbs.length; i++) {
                    finalWeightlb = finalWeightlb + weightlbs[i];
                }
                Weight = finalWeightlb;
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.MaxWeight154KGS,
                    showCloseButton: true
                });
            }
        }
        return Weight;
    };

    $scope.ContentCount = function (value) {
        if (value !== undefined && value !== null && value !== '') {
            var contentLength = [];
            for (i = 0; i < value.length; i++) {
                contentLength.push(value[i].Content);
            }
            $scope.Contentfinallength = contentLength.toString();
        }
    };

    function stopEnterKey(evt) {
        var evt1 = (evt) ? evt : ((event) ? event : null);
        var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
        if ((evt1.keyCode == 13) && (node.type == "text" || node.type == "checkbox" ||
                     node.type == "radio" || node.type == "email" || node.type == "number" || node.type == "textarea")) {
            return false;
        }
    }

    document.onkeypress = stopEnterKey;

    //Uploads and Download excel for Pieces grid
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

    $scope.progressExcel = function (evt) {
        //To Do:  show excel uploading progress message 
    };

    $scope.successExcel = function (data, status, headers, config) {
        if (status = 200) {
            if (data.Message == "Ok" || data.Message == "ok" || data.Message == "OK") {
                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteSuccess,
                    body: $scope.PackageAddeddSuccessfully,
                    showCloseButton: true
                });

                //  To Do: Logic to add row in Piecees grid.
                var result = data;

                if (result.FrayteShipmentDetail !== null && result.FrayteShipmentDetail.length && $scope.directBooking.Packages.length === 1 && ($scope.directBooking.Packages[0].CartoonValue > 0 ||
                    $scope.directBooking.Packages[0].Content !== "" || $scope.directBooking.Packages[0].Height > 0 || $scope.directBooking.Packages[0].Length > 0 ||
                    $scope.directBooking.Packages[0].Value > 0 || $scope.directBooking.Packages[0].Weight > 0 || $scope.directBooking.Packages[0].Width > 0)) {

                    for (var i = 0 ; i < result.FrayteShipmentDetail.length; i++) {
                        $scope.directBooking.Packages.push(result.FrayteShipmentDetail[i]);
                    }

                    var dbpac = $scope.directBooking.Packages.length - 1;
                    for (j = 0; j < $scope.directBooking.Packages.length; j++) {

                        if (j === dbpac) {
                            $scope.directBooking.Packages[j].pacVal = true;
                        }
                        else {
                            $scope.directBooking.Packages[j].pacVal = false;
                        }
                    }
                }
                else {
                    $scope.directBooking.Packages = [];
                    for (var ii = 0 ; ii < result.FrayteShipmentDetail.length; ii++) {
                        $scope.directBooking.Packages.push(result.FrayteShipmentDetail[ii]);
                    }
                    var dbpacval = $scope.directBooking.Packages.length - 1;
                    for (jj = 0; jj < $scope.directBooking.Packages.length; jj++) {

                        if (jj === dbpacval) {
                            $scope.directBooking.Packages[jj].pacVal = true;
                        }
                        else {
                            $scope.directBooking.Packages[jj].pacVal = false;
                        }
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
                body: $scope.Errorwhil_uploading_the_excel,
                showCloseButton: true
            });
        }
    };

    $scope.errorExcel = function (err) {
        toaster.pop({
            type: 'error',
            title: $scope.TitleFrayteError,
            body: $scope.Errorwhil_uploading_the_excel,
            showCloseButton: true
        });
    };

    //Uploads and Download excel for Pieces grid
    $scope.setFocusCustomTab = function (DirectBookingForm, ContentType) {
        if (DirectBookingForm !== undefined) {
            if (customInfoTabValid(DirectBookingForm, ContentType)) {
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

    var shipFromTabValid = function (DirectBookingForm) {
        if (DirectBookingForm && $scope.directBooking) {
            var flag = false;
            if ($scope.directBooking.ShipFrom.Country !== undefined && $scope.directBooking.ShipFrom.Country !== null &&
                ($scope.directBooking.ShipFrom.Country.Code === 'CAN' || $scope.directBooking.ShipFrom.Country.Code === 'USA')) {
                if (DirectBookingForm.shipFromAddress1 !== undefined && DirectBookingForm.shipFromAddress1.$valid &&
                    DirectBookingForm.shipFromCompanyName !== undefined && DirectBookingForm.shipFromCompanyName.$valid &&
                    DirectBookingForm.shipFromFirstName !== undefined && DirectBookingForm.shipFromFirstName.$valid &&
                    DirectBookingForm.shipFromLastName !== undefined && DirectBookingForm.shipFromLastName.$valid &&
                    DirectBookingForm.shipFromCountry !== undefined && DirectBookingForm.shipFromCountry.$valid &&
                    DirectBookingForm.shipFromPostcode !== undefined && DirectBookingForm.shipFromPostcode.$valid &&
                    DirectBookingForm.shipFromCountryState !== undefined && DirectBookingForm.shipFromCountryState.$valid &&
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
            else {
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
        }
    };

    var shipToTabValid = function (DirectBookingForm) {
        if (DirectBookingForm && $scope.directBooking) {
            var flag = false;
            if ($scope.directBooking.ShipTo.Country !== null && $scope.directBooking.ShipTo.Country !== undefined &&
                ($scope.directBooking.ShipTo.Country.Code === 'CAN' || $scope.directBooking.ShipTo.Country.Code === 'USA')) {
                if (DirectBookingForm.shipToAddress1 !== undefined && DirectBookingForm.shipToAddress1.$valid &&
                DirectBookingForm.shipToCompanyName !== undefined && DirectBookingForm.shipToCompanyName.$valid &&
                DirectBookingForm.shipToFirstName !== undefined && DirectBookingForm.shipToFirstName.$valid &&
                DirectBookingForm.shipToLastName !== undefined && DirectBookingForm.shipToLastName.$valid &&
                DirectBookingForm.shipToCountry !== undefined && DirectBookingForm.shipToCountry.$valid &&
                DirectBookingForm.shipToPostcode !== undefined && DirectBookingForm.shipToPostcode.$valid &&
                DirectBookingForm.shipToCountryState !== undefined && DirectBookingForm.shipToCountryState.$valid &&
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
            else {
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
        }
    };

    var shipmentDetailTabValid = function (DirectBookingForm) {
        if (DirectBookingForm && $scope.directBooking) {
            var flag = false;
            if ($scope.directBooking.CustomerRateCard !== undefined && $scope.directBooking.CustomerRateCard !== null &&
                DirectBookingForm.parcelType !== undefined && DirectBookingForm.parcelType.$valid && $scope.isCollectionTimeValid &&
                    DirectBookingForm.shipmentPaymentAccount !== undefined && DirectBookingForm.shipmentPaymentAccount.$valid &&
                    DirectBookingForm.paymentPartyCurrencyType !== undefined && DirectBookingForm.paymentPartyCurrencyType.$valid &&
                    DirectBookingForm.paymentPartyCurrencyType !== undefined && DirectBookingForm.paymentPartyCurrencyType.$valid &&
                    DirectBookingForm.reference1 !== undefined && DirectBookingForm.reference1.$valid &&
                    DirectBookingForm.shipmentCollectionDate !== undefined && DirectBookingForm.shipmentCollectionDate.$valid &&
                    DirectBookingForm.collectiontime !== undefined && DirectBookingForm.collectiontime.$valid &&
                    DirectBookingForm.collectioTimeMinutes !== undefined && DirectBookingForm.collectioTimeMinutes.$valid &&
                    DirectBookingForm.paymentPartyCurrencyType !== undefined && DirectBookingForm.paymentPartyCurrencyType.$valid
                ) {
                for (var j = 0 ; j < $scope.directBooking.Packages.length; j++) {
                    var packageForm1 = DirectBookingForm['tags' + j];
                    if (packageForm1 !== undefined && packageForm1.$valid) {
                        flag = true;
                    }
                    else {
                        flag = false;
                        break;
                    }
                }
            }
            else {
                if (DirectBookingForm.parcelType !== undefined && DirectBookingForm.parcelType.$valid && $scope.isCollectionTimeValid &&
                    DirectBookingForm.shipmentPaymentAccount !== undefined && DirectBookingForm.shipmentPaymentAccount.$valid &&
                    DirectBookingForm.paymentPartyCurrencyType !== undefined && DirectBookingForm.paymentPartyCurrencyType.$valid &&
                    DirectBookingForm.paymentPartyCurrencyType !== undefined && DirectBookingForm.paymentPartyCurrencyType.$valid &&
                    DirectBookingForm.reference1 !== undefined && DirectBookingForm.reference1.$valid &&
                    //DirectBookingForm.shipmentCollectionDate !== undefined && DirectBookingForm.shipmentCollectionDate.$valid &&
                    //DirectBookingForm.collectiontime !== undefined && DirectBookingForm.collectiontime.$valid &&
                    //DirectBookingForm.collectioTimeMinutes !== undefined && DirectBookingForm.collectioTimeMinutes.$valid &&
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
            }

            return flag;
        }
    };

    var customInfoTabValid = function (DirectBookingForm, ContentType) {
        if (DirectBookingForm && $scope.directBooking) {
            var flag = false;
            if (ContentType !== 'other') {
                if (DirectBookingForm.contentsType !== undefined && DirectBookingForm.contentsType.$valid &&
                    DirectBookingForm.restrictionType !== undefined && DirectBookingForm.restrictionType.$valid &&
                    DirectBookingForm.restrictionComments !== undefined && DirectBookingForm.restrictionComments.$valid &&
                    DirectBookingForm.nonDeliveryOption !== undefined && DirectBookingForm.nonDeliveryOption.$valid &&
                    DirectBookingForm.customsSigner !== undefined && DirectBookingForm.customsSigner.$valid &&
                    $scope.directBooking.CustomInfo.CustomsCertify) {
                    flag = true;
                }
                else {
                    flag = false;
                }
            }
            else {
                flag = false;
                if (DirectBookingForm.contentsType !== undefined && DirectBookingForm.contentsType.$valid &&
                    DirectBookingForm.contentsExplanation !== undefined && DirectBookingForm.contentsExplanation.$valid &&
                    DirectBookingForm.restrictionType !== undefined && DirectBookingForm.restrictionType.$valid &&
                    DirectBookingForm.restrictionComments !== undefined && DirectBookingForm.restrictionComments.$valid &&
                    DirectBookingForm.nonDeliveryOption !== undefined && DirectBookingForm.nonDeliveryOption.$valid &&
                    DirectBookingForm.customsSigner !== undefined && DirectBookingForm.customsSigner.$valid &&
                    $scope.directBooking.CustomInfo.CustomsCertify) {
                    flag = true;
                }
                else {
                    flag = false;
                }
            }
            return flag;
        }
    };

    $scope.shipFromTab = function (DirectBookingForm) {
        if (DirectBookingForm && $scope.directBooking) {
            return shipFromTabValid(DirectBookingForm);
        }
    };

    $scope.shipToTab = function (DirectBookingForm) {
        if (DirectBookingForm && $scope.directBooking) {
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
            if (ParcelType.ParcelDescription === 'Letter (Document)') {
                if ($scope.directBooking.Packages !== null && $scope.directBooking.Packages.length > 1) {
                    var modalOptions = {
                        headerText: $scope.Confirmation,
                        bodyText: $scope.RemoveChangingParcelTypeDoc
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
                    }, function () {
                        var found = $filter('filter')($scope.ParcelTypes, { ParcelDescription: 'Parcel (Non Doc)' });
                        if (found.length) {
                            $scope.directBooking.ParcelType = found[0];
                        }
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

    $scope.customInfoTab = function (DirectBookingForm, ContentType) {
        if (DirectBookingForm !== undefined) {
            return customInfoTabValid(DirectBookingForm, ContentType);
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

    $scope.UncheckFromCheckBox = function (ShipFromEmail) {
        if (ShipFromEmail !== undefined && ShipFromEmail !== null && ShipFromEmail !== '') {
            if ($scope.FromEmail !== undefined && $scope.FromEmail !== null && $scope.FromEmail !== '') {
                if ($scope.FromEmail !== ShipFromEmail) {
                    $scope.directBooking.ShipFrom.IsMailSend = false;
                }
                else {
                    $scope.directBooking.ShipFrom.IsMailSend = true;
                }
            }
            else {
                $scope.directBooking.ShipFrom.IsMailSend = false;
            }
        }
        else {
            $scope.directBooking.ShipFrom.IsMailSend = false;
        }
    };

    $scope.ShipFromCheckBox = function (ShipFromEmail) {
        if (ShipFromEmail !== undefined && ShipFromEmail !== null && ShipFromEmail !== '') {
            $scope.FromEmail = ShipFromEmail;
            $scope.directBooking.ShipFrom.IsMailSend = true;
        }
        else {
            $scope.directBooking.ShipFrom.IsMailSend = false;
        }
    };

    $scope.UncheckToCheckBox = function (ShipToEmail) {
        if (ShipToEmail !== undefined && ShipToEmail !== null && ShipToEmail !== '') {
            if ($scope.ToEmail !== undefined && $scope.ToEmail !== null && $scope.ToEmail !== '') {
                if ($scope.ToEmail !== ShipToEmail) {
                    $scope.directBooking.ShipTo.IsMailSend = false;
                }
                else {
                    $scope.directBooking.ShipTo.IsMailSend = true;
                }
            }
            else {
                $scope.directBooking.ShipTo.IsMailSend = false;
            }
        }
        else {
            $scope.directBooking.ShipTo.IsMailSend = false;
        }
    };

    $scope.ShipToCheckBox = function (ShipToEmail) {
        if (ShipToEmail !== undefined && ShipToEmail !== null && ShipToEmail !== '') {
            $scope.ToEmail = ShipToEmail;
            $scope.directBooking.ShipTo.IsMailSend = true;
        }
        else {
            $scope.directBooking.ShipTo.IsMailSend = false;
        }
    };

    $scope.shipFromOptionalField = function (action) {
        if (action !== undefined && action !== null && action !== '' && action === 'State') {
            if ($scope.directBooking !== undefined && $scope.directBooking !== null && $scope.directBooking.ShipFrom !== null && $scope.directBooking.ShipFrom.Country !== null) {
                if ($scope.directBooking.ShipFrom.Country.Code === 'HKG' || $scope.directBooking.ShipFrom.Country.Code === 'GBR') {
                    return false;
                }
                else if ($scope.directBooking.ShipFrom.Country.Code === 'CAN' || $scope.directBooking.ShipFrom.Country.Code === 'USA' || $scope.directBooking.ShipFrom.Country.Code === 'AUS') {
                    return false;
                }
                else {
                    return true;
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
                if ($scope.directBooking.ShipTo.Country.Code === 'HKG' || $scope.directBooking.ShipTo.Country.Code === 'GBR') {
                    return false;
                }
                else if ($scope.directBooking.ShipTo.Country.Code === 'CAN' || $scope.directBooking.ShipTo.Country.Code === 'USA' || $scope.directBooking.ShipTo.Country.Code === 'AUS') {
                    return false;
                }
                else {
                    return true;
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

    $scope.getTotalWeightKgs = function () {
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
            return parseFloat(total).toFixed(2);
        }
        else {
            return 0;
        }
    };

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

            sum = total.toFixed(2);
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
                        total += ((len * wid * height) / 5000) * qty;
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
                    if (as.length === 1) {
                        as = as + "0";
                        as = parseFloat(as);
                    }
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
        if ($scope.directBooking !== undefined && $scope.directBooking !== null) {
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
                    IsMailSend: false,
                    IsDefaultAddress: false
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
                    IsMailSend: false,
                    IsDefaultAddress: false
                },
                ParcelType: $scope.ParcelTypes[0],
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
                    CollectionTime: setColletionTime(),
                    IsCollection: false
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
                AddressType: 'B2B',
                BookingStatusType: 'Draft',
                IsPublic: false,
                RoleId: $scope.RoleId,
                LogInUseId: $scope.LogInUserId
            };
        }
        else {
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
                AddressType: 'B2B',
                BookingStatusType: 'Draft',
                IsPublic: false,
                RoleId: $scope.RoleId,
                LogInUseId: $scope.LogInUserId
            };
        }
    };

    var setColletionTime = function () {
        var h = "12";
        var m = "00";
        return h.toString() + m.toString();
    };

    $scope.checkTime = function (DirectBookingForm) {
        if (DirectBookingForm && $scope.directBooking && $scope.directBooking.ReferenceDetail && $scope.directBooking.ReferenceDetail) {
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
            var dd = new Date(colDate);
            var month = '';
            if (dd.getMonth().toString().length === 1) {
                month = "0" + dd.getMonth().toString();
            }
            else {
                month = dd.getMonth().toString();
            }

            var dateString1 = month + "-" + dd.getDate().toString() + "-" + dd.getFullYear().toString();
            var dateString2 = d.getMonth().toString() + "-" + d.getDate().toString() + "-" + d.getFullYear().toString();

            var CollectionDate = new Date(dateString1);
            var CurrentDate = new Date(dateString2);
            if (CollectionDate < CurrentDate) {
                DirectBookingForm.collectiontime.$dirty = true;
                $scope.isCollectionTimeValid = false;
                return true;
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
        else {
            return true;
        }
    };

    var remainFieldJson = function () {
        //Remained Fields Json
        $scope.RemainedFields = {
            FieldName: '',
            FieldLabel: '',
            FieldType: '',
            LabelName: '',
            ShowDiv: '',
            InputTypeName: '',
            RequiredMessage: '',
            GeneralObj: ''
        };
    };

    var remainedFieldsPopup = function (DirectBooking) {
        $scope.RemainField = [];
        var Shipment = DirectBooking;

        for (i = 0; i < Shipment.Packages.length; i++) {
            if (Shipment.ShipFrom.Country !== null && Shipment.ShipFrom.Country !== undefined && Shipment.ShipTo.Country !== null && Shipment.ShipTo.Country !== null && Shipment.ShipFrom.Country.Code === Shipment.ShipTo.Country.Code) {
                if ((Shipment.Packages[i].CartoonValue === 0 || Shipment.Packages[i].CartoonValue === null || Shipment.Packages[i].CartoonValue === undefined) ||
                (Shipment.Packages[i].Length === 0 || Shipment.Packages[i].Length === null || Shipment.Packages[i].Length === undefined) ||
                (Shipment.Packages[i].Width === 0 || Shipment.Packages[i].Width === null || Shipment.Packages[i].Width === undefined) ||
                (Shipment.Packages[i].Height === 0 || Shipment.Packages[i].Height === null || Shipment.Packages[i].Height === undefined) ||
                (Shipment.Packages[i].Weight === 0 || Shipment.Packages[i].Weight === null || Shipment.Packages[i].Weight === undefined) ||
                (Shipment.Packages[i].Content === "" || Shipment.Packages[i].Content === null || Shipment.Packages[i].Content === undefined)) {
                    $scope.ErrorFormShow = true;
                    break;
                }
                else {
                    $scope.ErrorFormShow = false;
                }
            }
            else {
                if ((Shipment.Packages[i].CartoonValue === 0 || Shipment.Packages[i].CartoonValue === null || Shipment.Packages[i].CartoonValue === undefined) ||
                (Shipment.Packages[i].Length === 0 || Shipment.Packages[i].Length === null || Shipment.Packages[i].Length === undefined) ||
                (Shipment.Packages[i].Width === 0 || Shipment.Packages[i].Width === null || Shipment.Packages[i].Width === undefined) ||
                (Shipment.Packages[i].Height === 0 || Shipment.Packages[i].Height === null || Shipment.Packages[i].Height === undefined) ||
                (Shipment.Packages[i].Weight === 0 || Shipment.Packages[i].Weight === null || Shipment.Packages[i].Weight === undefined) ||
                (Shipment.Packages[i].Value === 0 || Shipment.Packages[i].Value === null || Shipment.Packages[i].Value === undefined) ||
                (Shipment.Packages[i].Content === "" || Shipment.Packages[i].Content === null || Shipment.Packages[i].Content === undefined)) {
                    $scope.ErrorFormShow = true;
                    break;
                }
                else {
                    $scope.ErrorFormShow = false;
                }
            }
        }

        if ($scope.ButtonValue === "GetServices") {
            if (Shipment.ShipFrom.Country === "" || Shipment.ShipFrom.Country === null || Shipment.ShipFrom.Country === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldName = "Country";
                $scope.RemainedFields.FieldLabel = "From Country";
                $scope.RemainedFields.FileType = "DropDown";
                $scope.RemainedFields.ShowDiv = "FromShipper";
                $scope.RemainedFields.TrackBYOBJ = "ParcelType.CountryId";
                $scope.RemainedFields.IterationFor1 = "ParcelType.Name";
                $scope.RemainedFields.IterationForAs = "ParcelType";
                $scope.RemainedFields.IterationFor2 = "";
                $scope.RemainedFields.InputTypeName = "fromCountry";
                $scope.RemainedFields.RequiredMessage = "CountryValidationError";
                $scope.RemainedFields.GeneralObj = $scope.CountriesRepo;
                $scope.RemainField.push($scope.RemainedFields);
            }
            if ((Shipment.ShipFrom.PostCode === "" || Shipment.ShipFrom.PostCode === null || Shipment.ShipFrom.PostCode === undefined) && ((Shipment.ShipFrom.Country !== null && Shipment.ShipFrom.Country !== "" && Shipment.ShipFrom.Country !== undefined) && Shipment.ShipFrom.Country.Code !== "HKG" && Shipment.ShipFrom.Country.Code !== "BHS")) {
                remainFieldJson();
                $scope.RemainedFields.FieldName = "PostCode";
                $scope.RemainedFields.FieldLabel = "From FromPostCode";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "FromShipper";
                $scope.RemainedFields.LabelName = "fromPostCodeLabel";
                $scope.RemainedFields.InputTypeName = "fromPostCode";
                $scope.RemainedFields.RequiredMessage = "PostalCodeValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipFrom.FirstName === "" || Shipment.ShipFrom.FirstName === null || Shipment.ShipFrom.FirstName === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldName = "FirstName";
                $scope.RemainedFields.FieldLabel = "From Contact First Name";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "FromShipper";
                $scope.RemainedFields.LabelName = "fromFirstNameLabel";
                $scope.RemainedFields.InputTypeName = "fromFirstName";
                $scope.RemainedFields.RequiredMessage = "FirstName_Required";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipFrom.LastName === "" || Shipment.ShipFrom.LastName === null || Shipment.ShipFrom.LastName === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldName = "LastName";
                $scope.RemainedFields.FieldLabel = "From Contact Last Name";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "FromShipper";
                $scope.RemainedFields.LabelName = "fromLastNameLabel";
                $scope.RemainedFields.InputTypeName = "fromLastName";
                $scope.RemainedFields.RequiredMessage = "LastName_Required";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipFrom.Address === "" || Shipment.ShipFrom.Address === null || Shipment.ShipFrom.Address === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "From Address 1";
                $scope.RemainedFields.FieldName = "Address";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "FromShipper";
                $scope.RemainedFields.LabelName = "fromAddressNameLabel";
                $scope.RemainedFields.InputTypeName = "fromAddress";
                $scope.RemainedFields.RequiredMessage = "AddressValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipFrom.City === "" || Shipment.ShipFrom.City === null || Shipment.ShipFrom.City === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "From City";
                $scope.RemainedFields.FieldName = "City";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "FromShipper";
                $scope.RemainedFields.LabelName = "fromCityNameLabel";
                $scope.RemainedFields.InputTypeName = "fromCity";
                $scope.RemainedFields.RequiredMessage = "CityValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if ((Shipment.ShipFrom.State === "" || Shipment.ShipFrom.State === null || Shipment.ShipFrom.State === undefined) && (Shipment.ShipFrom.Country !== undefined && Shipment.ShipFrom.Country !== null && Shipment.ShipFrom.Country.Code !== 'HKG' && Shipment.ShipFrom.Country.Code !== 'GBR')) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "From State";
                $scope.RemainedFields.FieldName = "State";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "FromShipper";
                $scope.RemainedFields.LabelName = "fromStateNameLabel";
                $scope.RemainedFields.InputTypeName = "fromState";
                $scope.RemainedFields.RequiredMessage = "StateValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipFrom.Phone === "" || Shipment.ShipFrom.Phone === null || Shipment.ShipFrom.Phone === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "From TelephoneNo";
                $scope.RemainedFields.FieldName = "Phone";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "FromShipper";
                $scope.RemainedFields.LabelName = "fromPhoneLabel";
                $scope.RemainedFields.InputTypeName = "fromPhone";
                $scope.RemainedFields.RequiredMessage = "TelephoneValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipTo.Country === "" || Shipment.ShipTo.Country === null || Shipment.ShipTo.Country === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldName = "Country";
                $scope.RemainedFields.FieldLabel = "To Country";
                $scope.RemainedFields.FileType = "DropDown";
                $scope.RemainedFields.ShowDiv = "ToShipper";
                $scope.RemainedFields.TrackBYOBJ = "ParcelType.CountryId";
                $scope.RemainedFields.IterationFor1 = "ParcelType.Name";
                $scope.RemainedFields.IterationForAs = "ParcelType";
                $scope.RemainedFields.IterationFor2 = "";
                $scope.RemainedFields.InputTypeName = "toCountry";
                $scope.RemainedFields.RequiredMessage = "CountryValidationError";
                $scope.RemainedFields.GeneralObj = $scope.CountriesRepo;
                $scope.RemainField.push($scope.RemainedFields);
            }
            if ((Shipment.ShipTo.PostCode === "" || Shipment.ShipTo.PostCode === null || Shipment.ShipTo.PostCode === undefined) &&
                (Shipment.ShipTo.Country !== "" && Shipment.ShipTo.Country !== null && Shipment.ShipTo.Country !== undefined && Shipment.ShipTo.Country.Code !== "HKG" && Shipment.ShipTo.Country.Code !== "BHS")) {
                remainFieldJson();
                $scope.RemainedFields.FieldName = "PostCode";
                $scope.RemainedFields.FieldLabel = "To PostCode";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "ToShipper";
                $scope.RemainedFields.LabelName = "toPostCodeLabel";
                $scope.RemainedFields.InputTypeName = "toPostCode";
                $scope.RemainedFields.RequiredMessage = "PostalCodeValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipTo.FirstName === "" || Shipment.ShipTo.FirstName === null || Shipment.ShipTo.FirstName === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldName = "FirstName";
                $scope.RemainedFields.FieldLabel = "To Contact First Name";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "ToShipper";
                $scope.RemainedFields.LabelName = "toFirstNameLabel";
                $scope.RemainedFields.InputTypeName = "toFirstName";
                $scope.RemainedFields.RequiredMessage = "FirstName_Required";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipTo.LastName === "" || Shipment.ShipTo.LastName === null || Shipment.ShipTo.LastName === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldName = "LastName";
                $scope.RemainedFields.FieldLabel = "To Contact Last Name";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "ToShipper";
                $scope.RemainedFields.LabelName = "toLastNameLabel";
                $scope.RemainedFields.InputTypeName = "toLastName";
                $scope.RemainedFields.RequiredMessage = "LastName_Required";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipTo.Address === "" || Shipment.ShipTo.Address === null || Shipment.ShipTo.Address === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "To Address 1";
                $scope.RemainedFields.FieldName = "Address";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "ToShipper";
                $scope.RemainedFields.LabelName = "toAddressNameLabel";
                $scope.RemainedFields.InputTypeName = "toAddress";
                $scope.RemainedFields.RequiredMessage = "AddressValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipTo.City === "" || Shipment.ShipTo.City === null || Shipment.ShipTo.City === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "To City";
                $scope.RemainedFields.FieldName = "City";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "ToShipper";
                $scope.RemainedFields.LabelName = "toCityNameLabel";
                $scope.RemainedFields.InputTypeName = "toCity";
                $scope.RemainedFields.RequiredMessage = "CityValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if ((Shipment.ShipTo.State === "" || Shipment.ShipTo.State === null || Shipment.ShipTo.State === undefined) && (Shipment.ShipTo.Country !== undefined && Shipment.ShipTo.Country !== null && Shipment.ShipTo.Country.Code !== 'HKG' && Shipment.ShipTo.Country.Code !== 'GBR')) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "To State";
                $scope.RemainedFields.FieldName = "State";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "ToShipper";
                $scope.RemainedFields.LabelName = "toStateNameLabel";
                $scope.RemainedFields.InputTypeName = "toState";
                $scope.RemainedFields.RequiredMessage = "StateValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ShipTo.Phone === "" || Shipment.ShipTo.Phone === null || Shipment.ShipTo.Phone === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "To TelephoneNo";
                $scope.RemainedFields.FieldName = "Phone";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.ShowDiv = "ToShipper";
                $scope.RemainedFields.LabelName = "toPhoneLabel";
                $scope.RemainedFields.InputTypeName = "toPhone";
                $scope.RemainedFields.RequiredMessage = "TelephoneValidationError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.PayTaxAndDuties === "" || Shipment.PayTaxAndDuties === null || Shipment.PayTaxAndDuties === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "Pay Tax And Duties";
                $scope.RemainedFields.FieldName = "PayTaxAndDuties";
                $scope.RemainedFields.FileType = "radio";
                $scope.RemainedFields.ShowDiv = "MainObjProperty";
                $scope.RemainedFields.LabelName = "payTaxAndDutiesLabel";
                $scope.RemainedFields.InputTypeName = "payTaxAndDuties";
                $scope.RemainedFields.RequiredMessage = "PayTaxAndDuties is required.";
                $scope.RemainedFields.RadioButtonValues = ["Shipper", "Receiver", "ThirdParty"];
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.PakageCalculatonType === "" || Shipment.PakageCalculatonType === null || Shipment.PakageCalculatonType === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "Package Calculation Type";
                $scope.RemainedFields.FieldName = "PakageCalculatonType";
                $scope.RemainedFields.FileType = "radio";
                $scope.RemainedFields.InputTypeName = "pakageCalculatonType";
                $scope.RemainedFields.ShowDiv = "MainObjProperty";
                $scope.RemainedFields.LabelName = "pakageCalculatonTypeLabel";
                $scope.RemainedFields.RequiredMessage = "PackageCalculatonType is required.";
                $scope.RemainedFields.RadioButtonValues = ["kgToCms", "lbToInchs"];
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ParcelType === "" || Shipment.ParcelType === null || Shipment.ParcelType === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "Parcel Type";
                $scope.RemainedFields.FieldName = "ParcelType";
                $scope.RemainedFields.FileType = "DropDown";
                $scope.RemainedFields.DashString = "";
                $scope.RemainedFields.TrackBYOBJ = "ParcelType.ParcelType";
                $scope.RemainedFields.IterationFor1 = "ParcelType.ParcelDescription";
                $scope.RemainedFields.IterationFor2 = "";
                $scope.RemainedFields.InputTypeName = "parcelType";
                $scope.RemainedFields.LabelName = "parcelTypeLabel";
                $scope.RemainedFields.IterationForAs = "ParcelType.ParcelType";
                $scope.RemainedFields.RequiredMessage = "ParcelType_required";
                $scope.RemainedFields.ShowDiv = "MainObjProperty";
                $scope.RemainedFields.GeneralObj = $scope.ParcelTypes;

                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.Currency === "" || Shipment.Currency === null || Shipment.Currency === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "Currency";
                $scope.RemainedFields.FieldName = "Currency";
                $scope.RemainedFields.FileType = "DropDown";
                $scope.RemainedFields.DashString = "+ ' - ' +";
                $scope.RemainedFields.TrackBYOBJ = "ParcelType.CurrencyCode";
                $scope.RemainedFields.IterationFor1 = "ParcelType.CurrencyCode";
                $scope.RemainedFields.IterationFor2 = "ParcelType.CurrencyDescription";
                $scope.RemainedFields.InputTypeName = "currency";
                $scope.RemainedFields.LabelName = "currencyLabel";
                $scope.RemainedFields.ShowDiv = "MainObjProperty";
                $scope.RemainedFields.IterationForAs = "ParcelType.CurrencyCode";
                $scope.RemainedFields.RequiredMessage = "Currency_required";
                $scope.RemainedFields.GeneralObj = $scope.CurrencyTypes;
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.ReferenceDetail.Reference1 === "" || Shipment.ReferenceDetail.Reference1 === null || Shipment.ReferenceDetail.Reference1 === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "Shipment Reference";
                $scope.RemainedFields.FieldName = "Reference1";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.LabelName = "courierCompanyLable";
                $scope.RemainedFields.ShowDiv = "ReferenceDetail";
                $scope.RemainedFields.InputTypeName = "courierCompany";
                $scope.RemainedFields.RequiredMessage = "Reference_required";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if ((Shipment.AddressType === "" || Shipment.AddressType === null || Shipment.AddressType === undefined) &&
                $scope.LogisticCompany === 'Yodel' && Shipment.ShipFrom.Country.Code === 'GBR' && Shipment.ShipTo.Country.Code === 'GBR') {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "Address Type";
                $scope.RemainedFields.FieldName = "AddressType";
                $scope.RemainedFields.FileType = "DropDown";
                $scope.RemainedFields.LabelName = "addressTypeLable";
                $scope.RemainedFields.ShowDiv = "MainObjProperty";
                $scope.RemainedFields.InputTypeName = "addressType";
                $scope.RemainedFields.RequiredMessage = "Address_Type";
                $scope.RemainField.push($scope.RemainedFields);
            }
            //if ($scope.directBooking.CustomerRateCard != null && $scope.directBooking.CustomerRateCard.LogisticServiceId > 0 && Shipment.ReferenceDetail !== null && Shipment.ReferenceDetail !== undefined) {
            //    if (Shipment.ReferenceDetail.CollectionDate === undefined || Shipment.ReferenceDetail.CollectionDate === null || Shipment.ReferenceDetail.CollectionDate === "" ) {
            //        remainFieldJson();
            //        $scope.RemainedFields.FieldLabel = "Collection Date";
            //        $scope.RemainedFields.FieldName = "CollectionDate";
            //        $scope.RemainedFields.FileType = "Date";
            //        $scope.RemainedFields.LabelName = "collectionDateLable";
            //        $scope.RemainedFields.ShowDiv = "DirectBooking";
            //        $scope.RemainedFields.InputTypeName = "collectionDate";
            //        $scope.RemainedFields.RequiredMessage = "Collection Date is required.";
            //        $scope.RemainField.push($scope.RemainedFields);
            //    }
            //    if (Shipment.ReferenceDetail.CollectionTime === undefined || Shipment.ReferenceDetail.CollectionTime === null || Shipment.ReferenceDetail.CollectionTime ==="") {
            //        remainFieldJson();
            //        $scope.RemainedFields.FieldLabel = "Collection Time";
            //        $scope.RemainedFields.FieldName = "CollectionTime";
            //        $scope.RemainedFields.FileType = "DirectBooking";
            //        $scope.RemainedFields.LabelName = "collectionTimeLable";
            //        $scope.RemainedFields.ShowDiv = "ReferenceDetail";
            //        $scope.RemainedFields.InputTypeName = "collectionTime";
            //        $scope.RemainedFields.RequiredMessage = "Collection Time is required.";
            //        $scope.RemainField.push($scope.RemainedFields);
            //    }
            //}
        }

        if (Shipment.ShipFrom.Country != null && Shipment.ShipTo.Country != null && Shipment.ShipFrom.Country.Code != Shipment.ShipTo.Country.Code && $scope.ButtonValue === "PlaceBooking") {
            if (Shipment.CustomInfo.ContentsType === "" || Shipment.CustomInfo.ContentsType === null || Shipment.CustomInfo.ContentsType === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "Content_Type";
                $scope.RemainedFields.FieldName = "ContentsType";
                $scope.RemainedFields.FileType = "DropDown";
                $scope.RemainedFields.DashString = "";
                $scope.RemainedFields.TrackBYOBJ = "";
                $scope.RemainedFields.IterationFor1 = "ParcelType.name";
                $scope.RemainedFields.IterationFor2 = "";
                $scope.RemainedFields.InputTypeName = "contentsType";
                $scope.RemainedFields.LabelName = "contentsTypeLabel";
                $scope.RemainedFields.ShowDiv = "CustomInfo";
                $scope.RemainedFields.IterationForAs = "ParcelType.value";
                $scope.RemainedFields.RequiredMessage = "contentsTypeValidError";
                $scope.RemainedFields.GeneralObj = $scope.ContentsType;
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.CustomInfo.RestrictionType === "" || Shipment.CustomInfo.RestrictionType === null || Shipment.CustomInfo.RestrictionType === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "Restriction_Type";
                $scope.RemainedFields.FieldName = "RestrictionType";
                $scope.RemainedFields.FileType = "DropDown";
                $scope.RemainedFields.DashString = "";
                $scope.RemainedFields.TrackBYOBJ = "";
                $scope.RemainedFields.IterationFor1 = "ParcelType.name";
                $scope.RemainedFields.IterationFor2 = "";
                $scope.RemainedFields.InputTypeName = "restrictionType";
                $scope.RemainedFields.LabelName = "restrictionTypeLabel";
                $scope.RemainedFields.ShowDiv = "CustomInfo";
                $scope.RemainedFields.IterationForAs = "ParcelType.value";
                $scope.RemainedFields.RequiredMessage = "restrictionTypeValidError";
                $scope.RemainedFields.GeneralObj = $scope.RestrictionType;
                $scope.RemainField.push($scope.RemainedFields);
            }
            if ((Shipment.CustomInfo.RestrictionComments === "" || Shipment.CustomInfo.RestrictionComments === null || Shipment.CustomInfo.RestrictionComments === undefined) &&
                (Shipment.CustomInfo.RestrictionType !== 'quarantine' && Shipment.CustomInfo.RestrictionType !== 'sanitary_phytosanitary_inspection')) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "Restriction_Explanation";
                $scope.RemainedFields.FieldName = "RestrictionComments";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.LabelName = "restrictionCommentsLable";
                $scope.RemainedFields.ShowDiv = "CustomInfo";
                $scope.RemainedFields.InputTypeName = "restrictionComments";
                $scope.RemainedFields.RequiredMessage = "restrictionCommentsValidError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.CustomInfo.NonDeliveryOption === "" || Shipment.CustomInfo.NonDeliveryOption === null || Shipment.CustomInfo.NonDeliveryOption === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "nonDeliveryOption";
                $scope.RemainedFields.FieldName = "NonDeliveryOption";
                $scope.RemainedFields.FileType = "DropDown";
                $scope.RemainedFields.DashString = "";
                $scope.RemainedFields.TrackBYOBJ = "";
                $scope.RemainedFields.IterationFor1 = "ParcelType.name";
                $scope.RemainedFields.IterationFor2 = "";
                $scope.RemainedFields.InputTypeName = "nonDeliveryOption";
                $scope.RemainedFields.LabelName = "nonDeliveryOptionLabel";
                $scope.RemainedFields.ShowDiv = "CustomInfo";
                $scope.RemainedFields.IterationForAs = "ParcelType.value";
                $scope.RemainedFields.RequiredMessage = "nonDeliveryOptionValidError";
                $scope.RemainedFields.GeneralObj = $scope.NonDeliveryOption;
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.CustomInfo.CustomsSigner === "" || Shipment.CustomInfo.CustomsSigner === null || Shipment.CustomInfo.CustomsSigner === undefined) {
                remainFieldJson();
                $scope.RemainedFields.FieldLabel = "customsSigner";
                $scope.RemainedFields.FieldName = "CustomsSigner";
                $scope.RemainedFields.FileType = "text";
                $scope.RemainedFields.LabelName = "customsSignerLable";
                $scope.RemainedFields.ShowDiv = "CustomInfo";
                $scope.RemainedFields.InputTypeName = "customsSigner";
                $scope.RemainedFields.RequiredMessage = "customsSignerValidError";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.CustomInfo.CustomsCertify === "" || Shipment.CustomInfo.CustomsCertify === null || Shipment.CustomInfo.CustomsCertify === undefined) {
                remainFieldJson();
                if ($rootScope.SITECOMPANY === 'MAXLOGISTIC') {
                    $scope.RemainedFields.FieldLabel = "By_filling_validation_Mex";
                }
                else {
                    $scope.RemainedFields.FieldLabel = "By_filling_validation";
                }

                $scope.RemainedFields.FieldName = "CustomsCertify";
                $scope.RemainedFields.FileType = "checkbox";
                $scope.RemainedFields.LabelName = "customsCertifyLable";
                $scope.RemainedFields.ShowDiv = "CustomInfo";
                $scope.RemainedFields.InputTypeName = "customsCertify";
                $scope.RemainedFields.RequiredMessage = "";
                $scope.RemainField.push($scope.RemainedFields);
            }
            if (Shipment.CustomerRateCard != null && (Shipment.CustomerRateCard.CourierName === "UKMail" || Shipment.CustomerRateCard.CourierName === "Yodel" || Shipment.CustomerRateCard.CourierName === "Hermes")) {
                if (Shipment.CustomInfo.CatagoryOfItem === "" || Shipment.CustomInfo.CatagoryOfItem === null || Shipment.CustomInfo.CatagoryOfItem === undefined) {
                    remainFieldJson();
                    $scope.RemainedFields.FieldLabel = "Category_Item";
                    $scope.RemainedFields.FieldName = "CatagoryOfItem";
                    $scope.RemainedFields.FileType = "text";
                    $scope.RemainedFields.LabelName = "catagoryOfItemLable";
                    $scope.RemainedFields.ShowDiv = "CustomInfo";
                    $scope.RemainedFields.InputTypeName = "catagoryOfItem";
                    $scope.RemainedFields.RequiredMessage = "Category_Item_required";
                    $scope.RemainField.push($scope.RemainedFields);
                }
                if (Shipment.CustomInfo.CatagoryOfItemExplanation === "" || Shipment.CustomInfo.CatagoryOfItemExplanation === null || Shipment.CustomInfo.CatagoryOfItemExplanation === undefined) {
                    remainFieldJson();
                    $scope.RemainedFields.FieldLabel = "Category_Explanation";
                    $scope.RemainedFields.FieldName = "CatagoryOfItemExplanation";
                    $scope.RemainedFields.FileType = "text";
                    $scope.RemainedFields.LabelName = "catagoryOfItemExplanationLable";
                    $scope.RemainedFields.ShowDiv = "CustomInfo";
                    $scope.RemainedFields.InputTypeName = "catagoryOfItemExplanation";
                    $scope.RemainedFields.RequiredMessage = "Content_Explanation_Required";
                    $scope.RemainField.push($scope.RemainedFields);
                }
            }
        }

        Shipment.RemainedFields = $scope.RemainField;

        if ($scope.RemainField.length > 0 || $scope.ErrorFormShow === true) {

            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'uploadShipment/uploadShipmentWithoutService/uploadShipmentWithoutServiceErrors.tpl.html',
                controller: 'DirectBookingRemainedFieldsController',
                windowClass: '',
                size: 'md',
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    ShipmentData: function () {
                        return Shipment;
                    },
                    ErrorFormShow: function () {
                        return $scope.ErrorFormShow;
                    },
                    CountryPhoneCodes: function () {
                        return $scope.CountryPhoneCodes;
                    }
                }
            });
            modalInstance.result.then(function (Shipment) {
                $scope.directBooking = Shipment;
                //Shipment = data;
                //if (PhoneCode !== undefined && PhoneCode !== undefined && PhoneCode.ShipFromPhoneCode !== "") {
                //    $scope.ShipFromPhoneCode = PhoneCode.ShipFromPhoneCode;
                //}
                //if (PhoneCode !== undefined && PhoneCode !== undefined && PhoneCode.ShipToPhoneCode !== "") {
                //    $scope.ShipToPhoneCode = PhoneCode.ShipToPhoneCode;
                //}
                //if ($scope.directBooking.ShipFrom.Country !== undefined && $scope.directBooking.ShipFrom.Country !== null) {

                //}
                window.scrollTo(0, 1500);
            }, function () {
            });
        }
        return $scope.RemainField;
    };

    //Disable weekend selection
    $scope.disabled = function (date, mode) {
        return (mode === 'day' && (date.getDay() === 0 || date.getDay() === 6));
    };

    $scope.PlaceBooking = function (DirectBookingForm, isValid, directBooking) {

        if ($scope.BookingType === "Draft") {
            $scope.RemainFieldLength = [];
        }
        else {
            $scope.RemainFieldLength = remainedFieldsPopup(directBooking);
        }

        directBooking.CreatedBy = $scope.CreatedBy;

        if (DirectBookingForm !== undefined) {
            $rootScope.GetServiceValue = false;
        }
        if ($scope.LogisticCompany !== undefined && $scope.LogisticCompany !== null && $scope.LogisticCompany !== '') {
            if ($scope.LogisticCompany === 'Yodel') {

            }
        }
        else {
            directBooking.AddressType = '';
        }

        if ($scope.BookingType === "Draft") {
            if ($scope.RoleId === 6) {
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'directBooking/directBookingDraftShipmentConfirmation/directBookingDraftShipmentConfirmation.tpl.html',
                    controller: 'DraftPublicConfirmation',
                    windowClass: '',
                    size: 'md',
                    backdrop: 'static',
                    keyboard: false
                });

                modalInstance.result.then(function (response) {
                    if (response !== undefined && response !== null && response !== '') {
                        $scope.directBooking.IsPublic = response;
                        $scope.SaveDraftBooking(DirectBookingForm, isValid, directBooking);
                    }
                    else {

                    }
                });
            }
            else {
                $scope.SaveDraftBooking(DirectBookingForm, isValid, directBooking);
            }
        }
        else if ($scope.BookingType === "Current") {
            $scope.SaveCurrentBooking(DirectBookingForm, isValid, directBooking);
        }
    };

    $scope.SaveCurrentBooking = function (DirectBookingForm, isValid, directBooking) {

        if ($scope.RemainFieldLength.length === 0) {
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
            if (directBooking.ReferenceDetail.CollectionDate !== null && directBooking.ReferenceDetail.CollectionDate !== undefined) {
                directBooking.ReferenceDetail.CollectionDate = TimeStringtoDateTime.ConvertString(directBooking.ReferenceDetail.CollectionDate, directBooking.ReferenceDetail.CollectionTime);
            }

            if ($scope.BookingType === "Draft" && directBooking.ShipFrom.Country !== null && directBooking.ShipTo.Country !== null || (isValid && $scope.isCollectionTimeValid) || ($scope.BookingType === 'Current')) {

                directBooking.Packages[0].TrackingNo = null;
                if ($scope.directBooking.Packages !== null && $scope.directBooking.Packages.length > 0) {
                    var str = "";
                    for (var dp = 0 ; dp < $scope.directBooking.Packages.length; dp++) {
                        str += ", " + $scope.directBooking.Packages[dp].Content;
                    }
                    $scope.directBooking.ReferenceDetail.ContentDescription = str.slice(2);
                }

                if ($scope.BookingType === "Draft") {
                    directBooking.ShipmentStatusId = 14;
                    directBooking.BookingStatusType = "Draft";
                    AppSpinner.showSpinnerTemplate('Saving shipment', $scope.Template);
                }
                else {
                    directBooking.ShipmentStatusId = 12;
                    directBooking.BookingStatusType = "Current";
                    AppSpinner.showSpinnerTemplate('', $scope.Template);
                }

                // There is no need of Collection date and time for serives other than DHL
                if (directBooking.CustomerRateCard && directBooking.CustomerRateCard.LogisticServiceId) {
                    if (directBooking.CustomerRateCard.CourierName === "UKMail" || directBooking.CustomerRateCard.CourierName === "Yodel" || directBooking.CustomerRateCard.CourierName === "Hermes") {
                        directBooking.ReferenceDetail.CollectionDate = null;
                        directBooking.ReferenceDetail.CollectionTime = "";
                    }
                }

                // There is no need of State name for HK and UK
                if (directBooking.ShipFrom.Country.Code === 'HKG' || directBooking.ShipFrom.Country.Code === 'GBR') {
                    directBooking.ShipFrom.State = '';
                }
                if (directBooking.ShipTo.Country.Code === 'HKG' || directBooking.ShipTo.Country.Code === 'GBR') {
                    directBooking.ShipTo.State = '';
                }

                if (directBooking.CustomInfo.ContentsType === "merchandise") {
                    directBooking.CustomInfo.ContentsType = "Merchandise";
                }
                else if (directBooking.CustomInfo.ContentsType === "documents") {
                    directBooking.CustomInfo.ContentsType = "Documents";
                }
                else if (directBooking.CustomInfo.ContentsType === "gift") {
                    directBooking.CustomInfo.ContentsType = "Gift";
                }
                else if (directBooking.CustomInfo.ContentsType === "returned_goods") {
                    directBooking.CustomInfo.ContentsType = "Returned Goods";
                }
                else if (directBooking.CustomInfo.ContentsType === "sample") {
                    directBooking.CustomInfo.ContentsType = "Sample";
                }
                else if (directBooking.CustomInfo.ContentsType === "other") {
                    directBooking.CustomInfo.ContentsType = "Other";
                }

                DirectBookingService.SaveDirectBooking(directBooking).then(function (response) {
                    if (response.status === 200 && response.data.Error.Status) {
                        AppSpinner.hideSpinnerTemplate();
                        //Redirect to direct shipmnets page after 2 second.
                        $timeout(function () {
                            //$scope.DirectShipmentDetail(response.data.DirectShipmentId);
                            toaster.pop({
                                type: 'success',
                                title: $scope.TitleFrayteSuccess,
                                body: $scope.BookingSaveValidation,
                                showCloseButton: true
                            });

                            var route = UtilityService.GetCurrentRoute($scope.tabs, "userTabs.direct-shipments");

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
                                        $state.go(route, { moduleType: "db" }, { reload: true });
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
                                    $state.go(route, { moduleType: "db" }, { reload: true });
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
                    }
                },
                function () {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.ServiceSideErrorValidation,
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
            }
        }
    };

    $scope.SaveDraftBooking = function (DirectBookingForm, isValid, directBooking) {

        //if ($scope.RemainFieldLength.length === 0) {
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
        if (directBooking.ReferenceDetail.CollectionDate !== null && directBooking.ReferenceDetail.CollectionDate !== undefined) {
            directBooking.ReferenceDetail.CollectionDate = TimeStringtoDateTime.ConvertString(directBooking.ReferenceDetail.CollectionDate, directBooking.ReferenceDetail.CollectionTime);
        }

        if ($scope.BookingType === "Draft" && directBooking.ShipFrom.Country !== null && directBooking.ShipTo.Country !== null || (isValid && $scope.isCollectionTimeValid) || ($scope.BookingType === 'Current')) {

            directBooking.Packages[0].TrackingNo = null;
            if ($scope.directBooking.Packages !== null && $scope.directBooking.Packages.length > 0) {
                var str = "";
                for (var dp = 0 ; dp < $scope.directBooking.Packages.length; dp++) {
                    str += ", " + $scope.directBooking.Packages[dp].Content;
                }
                $scope.directBooking.ReferenceDetail.ContentDescription = str.slice(2);
            }

            if ($scope.BookingType === "Draft") {
                directBooking.ShipmentStatusId = 14;
                directBooking.BookingStatusType = "Draft";
                $rootScope.GetServiceValue = '';
                AppSpinner.showSpinnerTemplate('Saving Shipment', $scope.Template);
            }
            else {
                directBooking.ShipmentStatusId = 12;
                directBooking.BookingStatusType = "Current";
                AppSpinner.showSpinnerTemplate('', $scope.Template);
            }

            // There is no need of Collection date and time for serives other than DHL
            if (directBooking.CustomerRateCard && directBooking.CustomerRateCard.LogisticServiceId) {
                if (directBooking.CustomerRateCard.CourierName === "UKMail" || directBooking.CustomerRateCard.CourierName === "Yodel" || directBooking.CustomerRateCard.CourierName === "Hermes") {
                    directBooking.ReferenceDetail.CollectionDate = null;
                    directBooking.ReferenceDetail.CollectionTime = "";
                }
            }

            // There is no need of State name for HK and UK
            if (directBooking.ShipFrom.Country.Code === 'HKG' || directBooking.ShipFrom.Country.Code === 'GBR') {
                directBooking.ShipFrom.State = '';
            }
            if (directBooking.ShipTo.Country.Code === 'HKG' || directBooking.ShipTo.Country.Code === 'GBR') {
                directBooking.ShipTo.State = '';
            }

            DirectBookingService.SaveDirectBooking(directBooking).then(function (response) {
                if (response.status === 200 && response.data.Error.Status) {
                    //Redirect to direct shipmnets page after 2 second.
                    $timeout(function () {
                        AppSpinner.hideSpinnerTemplate();
                        //$scope.DirectShipmentDetail(response.data.DirectShipmentId);
                        toaster.pop({
                            type: 'success',
                            title: $scope.TitleFrayteSuccess,
                            body: $scope.BookingSaveValidation,
                            showCloseButton: true
                        });

                        var route = UtilityService.GetCurrentRoute($scope.tabs, "userTabs.direct-shipments");

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
                                    $state.go(route, { moduleType: "db" }, { reload: true });
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
                                $state.go(route, { moduleType: "db" }, { reload: true });
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
                }
            },
            function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.ServiceSideErrorValidation,
                    showCloseButton: true
                });
            });
        }
        else {
            AppSpinner.hideSpinnerTemplate();
            if ((directBooking.ShipFrom.Country === undefined || directBooking.ShipFrom.Country === null) || (directBooking.ShipTo.Country === undefined || directBooking.ShipTo.Country === null) || (directBooking.CustomerId === undefined || directBooking.CustomerId === null || directBooking.CustomerId === 0)) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.SaveDraft,
                    showCloseButton: true
                });
                return;
            }
        }
        //}
    };

    $scope.DirectShipmentDetail = function (ShipmentId) {
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
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteWarning', 'FrayteInformation', 'FrayteValidation', 'FrayteSuccess', 'CustomInfoCheckTerms',
                    'PleaseSelectionDeliveryOption', 'PleaseSelectTermAndConditions', 'PleaseCorrectValidationErrors',
                    'ErrorSavingShipment', 'PleaseSelectValidFile', 'UploadedSuccessfully', 'ErrorOccuredDuringUpload',
                    'ErrorGettingShipmentDetailServer', 'SaveDraft_Validation', 'PackageShipment_Validation',
                    'SelectShipment_Validation', 'CustomInformation_Validation', 'BookingSave_Validation',
                    'ServiceSideError_Validation', 'Success', 'RemovePackage_Validation', 'Validation_Error', 'GetService_Validation',
                    'SelectCustomer_Validation', 'SelectCurrency_Validation', 'FrayteWarning_Validation', 'SelectCustomerAddressBook_Validation',
                    'FrayteServiceError_Validation', 'ReceiveDetail_Validation', 'InitialData_Validation', 'DirectBooking_Error_Message', 'Confirmation',
                    'RemoveChangingParcelTypeDoc', 'Errorwhil_uploading_the_excel', 'PackageAddeddSuccessfully', 'MaxWeight154KGS', 'MaxWeight70kGS',
        'Sure_Clear_The_Form', 'Loading_Post_Code_Address']).then(function (translations) {
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteWarning = translations.FrayteWarning;
            $scope.TitleFrayteInformation = translations.FrayteSuccess;
            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TitleFrayteSuccess = translations.FrayteSuccess;
            $scope.TextValidation = translations.DirectBooking_Error_Message;
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
            $scope.Confirmation = translations.Confirmation;
            $scope.RemoveChangingParcelTypeDoc = translations.RemoveChangingParcelTypeDoc;
            $scope.Errorwhil_uploading_the_excel = translations.Errorwhil_uploading_the_excel;
            $scope.PackageAddeddSuccessfully = translations.PackageAddeddSuccessfully;
            $scope.MaxWeight154KGS = translations.MaxWeight154KGS;
            $scope.MaxWeight70kGS = translations.MaxWeight70kGS;
            $scope.Sure_Clear_The_Form = translations.Sure_Clear_The_Form;
            $scope.Loading_Post_Code_Address = translations.Loading_Post_Code_Address;
        });
    };

    $scope.AddPackage = function () {
        $scope.HideContent = true;
        $scope.directBooking.Packages.push({
            DirectShipmentDetailDraftId: 0,
            CartoonValue: null,
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
            if ($scope.directBooking.Packages.length === 2) {
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
                if (index === $scope.directBooking.Packages.length - 1) {
                    var dbpac = $scope.directBooking.Packages.length - 2;
                    for (i = 0; i < $scope.directBooking.Packages.length; i++) {

                        if (i === dbpac) {
                            $scope.directBooking.Packages[i].pacVal = true;
                        }
                        else {
                            $scope.directBooking.Packages[i].pacVal = false;
                        }
                    }
                }
                else {
                    var dbpac1 = $scope.directBooking.Packages.length - 1;
                    for (i = 0; i < $scope.directBooking.Packages.length; i++) {

                        if (i === dbpac1) {
                            $scope.directBooking.Packages[i].pacVal = true;
                        }
                        else {
                            $scope.directBooking.Packages[i].pacVal = false;
                        }
                    }
                }
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
                var f = convertc.toFixed(2);
                var swd = Number(parseFloat(convertc).toFixed(2)).toLocaleString('en', {
                    minimumFractionDigits: 2
                });

                return f;

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

    //Disable input controls in Custom information panel
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
            $scope.disableContentExplanation = true;
        }
        else {
            $scope.disableContentExplanation = false;
        }
    };

    $scope.editGetServices = function (DirectBookingForm) {
        $scope.GetServices($scope.directBooking, DirectBookingForm);
    };

    $scope.GetServices = function (directBooking, DirectBookingForm) {

        $scope.RemainFieldLength = remainedFieldsPopup(directBooking);

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
           directBooking.Packages !== null &&
           $scope.RemainFieldLength.length === 0) {

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
                //toaster.pop({
                //    type: 'warning',
                //    title: $scope.TitleFrayteWarning,
                //    body: $scope.GetServiceValidation,
                //    showCloseButton: true
                //});
                //$scope.RemainFieldLength = remainedFieldsPopup(directBooking);
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
                    },
                    AddressType: function () {
                        if ($scope.LogisticCompany !== undefined && $scope.LogisticCompany !== null && $scope.LogisticCompany !== '') {
                            if ($scope.LogisticCompany === 'Yodel') {
                                return $scope.AddressType;
                            }
                        }
                        else {
                            return '';
                        }
                    },
                    LogisticService: function () {
                        return $scope.LogisticCompany;
                    },
                    CallingFrom: function () {
                        return 'DirectBooking';
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

                    if ($scope.directBooking.CustomerRateCard.LogisticServiceId > 0) {
                        for (var aa = 0; aa < $scope.ShipmentMethods.length; aa++) {

                        }
                        if ($scope.ShowCustomInfoSection($scope.directBooking) && $scope.directBooking.CustomerRateCard.LogisticServiceId > 0) {
                            $scope.active = 2;
                        }
                    }
                    if ($scope.directBooking.CustomerRateCard.LogisticServiceId > 0 && ($scope.directBooking.CustomerRateCard.CourierName === "DHL" || $scope.directBooking.CustomerRateCard.CourierName === "TNT" ||
                        $scope.directBooking.CustomerRateCard.CourierName === "UPS" || $scope.directBooking.CustomerRateCard.CourierName === "EAM-DHL" || $scope.directBooking.CustomerRateCard.CourierName === "EAM-TNT" ||
                        $scope.directBooking.CustomerRateCard.CourierName === "EAM-FedEx")) {
                        if ($scope.directBooking.ReferenceDetail.CollectionDate === null || $scope.directBooking.ReferenceDetail.CollectionDate === undefined) {
                            //$scope.directBooking.ReferenceDetail.CollectionDate = new Date();
                        }
                        else {
                            $scope.directBooking.ReferenceDetail.CollectionDate = null;
                            $scope.directBooking.ReferenceDetail.CollectionTime = "";
                        }

                        var modalInstance = $uibModal.open({
                            animation: true,
                            templateUrl: 'directBooking/directBookingCollectionDateTime/directBookingCollectionDateTime.tpl.html',
                            controller: 'DirectBookingCollectionDateTimeController',
                            windowClass: 'DirectBookingService',
                            size: 'md',
                            backdrop: 'static',
                            resolve: {
                                PackageLength: function () {
                                    return $scope.directBooking.Packages.length;
                                },
                                ShipFromCountryId: function () {
                                    return $scope.directBooking.ShipFrom.Country.CountryId;
                                }
                            }
                        });
                        modalInstance.result.then(function (ReferenceDetail) {
                            if (ReferenceDetail) {
                                if (ReferenceDetail.IsCollection === true) {
                                    $scope.directBooking.ReferenceDetail.CollectionDate = ReferenceDetail.CollectionDate;
                                    $scope.directBooking.ReferenceDetail.CollectionTime = ReferenceDetail.CollectionTime;
                                    $scope.directBooking.ReferenceDetail.IsCollection = ReferenceDetail.IsCollection;
                                    $scope.IsCollection = ReferenceDetail.IsCollection;
                                    $scope.Message = ReferenceDetail.Message;
                                }
                                else {
                                    $scope.directBooking.ReferenceDetail.CollectionDate = null;
                                    $scope.directBooking.ReferenceDetail.CollectionTime = "";
                                }
                            }
                            $scope.BookingType = ReferenceDetail.BookingStatus;
                            if (ReferenceDetail.Type === "Confirm") {
                                $scope.ButtonValue = "PlaceBooking";
                                $scope.PlaceBooking($scope.DirectBookingForm, false, $scope.directBooking);
                            }
                        }, function (ReferenceDetail) {
                        });
                    }

                    //setScroll("pull-down");
                    //remainedFieldsPopup(directBooking);
                    window.scrollTo(0, 1500);

                }
            }, function () {

            });
        }
        else {
            //toaster.pop({
            //    type: 'warning',
            //    title: $scope.TitleFrayteWarning,
            //    body: $scope.GetServiceValidation,
            //    showCloseButton: true
            //});
        }
    };

    $scope.GetDateDay = function (CollectionDate) {
        var date = CollectionDate;
        if (date !== undefined && date !== null && date !== '') {
            if (date.getDay() === 6) {
                $scope.Message = true;
            }
            else {
                $scope.Message = false;
            }
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
        if ($scope.CustomerDetail !== null && $scope.CustomerDetail.ShipFrom !== null) {

            if ($scope.CustomerDetail.ShipFrom.IsDefaultAddress) {
                $scope.shipFromDefaultAddress = false;
            }
            else {
                $scope.shipFromDefaultAddress = true;
            }

            $scope.directBooking.ShipFrom.Country = $scope.CustomerDetail.ShipFrom.Country;
            $scope.directBooking.ShipFrom.PostCode = $scope.CustomerDetail.ShipFrom.PostCode;
            $scope.directBooking.ShipFrom.FirstName = $scope.CustomerDetail.ShipFrom.FirstName;
            $scope.directBooking.ShipFrom.LastName = $scope.CustomerDetail.ShipFrom.LastName;
            $scope.directBooking.ShipFrom.CompanyName = $scope.CustomerDetail.ShipFrom.CompanyName;
            $scope.directBooking.ShipFrom.Address = $scope.CustomerDetail.ShipFrom.Address;
            $scope.directBooking.ShipFrom.Address2 = $scope.CustomerDetail.ShipFrom.Address2;
            $scope.directBooking.ShipFrom.City = $scope.CustomerDetail.ShipFrom.City;
            $scope.directBooking.ShipFrom.Area = $scope.CustomerDetail.ShipFrom.Suburb;
            $scope.directBooking.ShipFrom.State = $scope.CustomerDetail.ShipFrom.State;
            $scope.directBooking.ShipFrom.Phone = $scope.CustomerDetail.ShipFrom.Phone;
            $scope.directBooking.ShipFrom.Email = "";
        }
        else {
            $scope.shipFromDefaultAddress = true;
        }
        if ($scope.CustomerDetail !== null && $scope.CustomerDetail.ShipTo !== null) {

            if ($scope.CustomerDetail.ShipTo.IsDefaultAddress) {
                $scope.shipToDefaultAddress = false;
            }
            else {
                $scope.shipToDefaultAddress = true;
            }

            $scope.directBooking.ShipTo.Country = $scope.CustomerDetail.ShipTo.Country;
            $scope.directBooking.ShipTo.PostCode = $scope.CustomerDetail.ShipTo.PostCode;
            $scope.directBooking.ShipTo.FirstName = $scope.CustomerDetail.ShipTo.FirstName;
            $scope.directBooking.ShipTo.LastName = $scope.CustomerDetail.ShipTo.LastName;
            $scope.directBooking.ShipTo.CompanyName = $scope.CustomerDetail.ShipTo.CompanyName;
            $scope.directBooking.ShipTo.Address = $scope.CustomerDetail.ShipTo.Address;
            $scope.directBooking.ShipTo.Address2 = $scope.CustomerDetail.ShipTo.Address2;
            $scope.directBooking.ShipTo.City = $scope.CustomerDetail.ShipTo.City;
            $scope.directBooking.ShipTo.Area = $scope.CustomerDetail.ShipTo.Suburb;
            $scope.directBooking.ShipTo.State = $scope.CustomerDetail.ShipTo.State;
            $scope.directBooking.ShipTo.Phone = $scope.CustomerDetail.ShipTo.Phone;
            $scope.directBooking.ShipTo.Email = "";
        }
        else {
            $scope.shipToDefaultAddress = true;
        }
    };

    var clearDirectBookingForm = function (form) {

        form.$setPristine();
        form.$setUntouched();
        $scope.NewDirectBooking();
        $scope.active = 1;

        $scope.CustomerDetail = null;

        if ($scope.RoleId === 3) {
            $scope.directBooking.CustomerId = $scope.customerId;
        }

        if ($scope.ShipToPhoneCode) {
            $scope.ShipToPhoneCode = null;
        }
        window.scrollTo(0, 0);

    };

    $scope.ClearForm = function (form) {

        if (form.$dirty) {
            var modalOptions = {
                headerText: $scope.Confirmation,
                bodyText: $scope.Sure_Clear_The_Form
            };

            ModalService.Confirm({}, modalOptions).then(function (result) {
                clearDirectBookingForm(form);
            }, function () {

            });
        }
        else {
            clearDirectBookingForm(form);
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

    //Set Country Phone Code
    $scope.SetShipFrominfo = function (Country, Action) {
        if (Country && Action) {
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

                $scope.directBooking.ShipFrom.Country = null;

                return;
            }

            $scope.customerId = $scope.RoleId === 17 ? $scope.userInfo.EmployeeId : $scope.directBooking.CustomerId;

            if (Action) {
                DirectBookingService.GetCountryDefaultAddress(Country.CountryId, $scope.directBooking.CustomerId, Action).then(function (response) {
                    if (response.data) {
                        if (Action === "ShipFrom") {
                            if (response.data.ShipFrom) {
                                $scope.directBooking.ShipFrom.Country = response.data.ShipFrom.Country;
                                $scope.directBooking.ShipFrom.PostCode = response.data.ShipFrom.PostCode;
                                $scope.directBooking.ShipFrom.FirstName = response.data.ShipFrom.FirstName;
                                $scope.directBooking.ShipFrom.LastName = response.data.ShipFrom.LastName;
                                $scope.directBooking.ShipFrom.CompanyName = response.data.ShipFrom.CompanyName;
                                $scope.directBooking.ShipFrom.Address = response.data.ShipFrom.Address;
                                $scope.directBooking.ShipFrom.Address2 = response.data.ShipFrom.Address2;
                                $scope.directBooking.ShipFrom.City = response.data.ShipFrom.City;
                                $scope.directBooking.ShipFrom.State = response.data.ShipFrom.State;
                                $scope.directBooking.ShipFrom.Area = response.data.ShipFrom.Area;
                                $scope.directBooking.ShipFrom.Phone = response.data.ShipFrom.Phone;
                                $scope.directBooking.ShipFrom.Email = "";
                                $scope.directBooking.ShipFrom.IsMailSend = false;

                            }
                            if (Action === "ShipTo") {
                                if (response.data.ShipTo) {
                                    $scope.directBooking.ShipTo.Country = response.data.ShipTo.Country;
                                    $scope.directBooking.ShipTo.PostCode = response.data.ShipTo.PostCode;
                                    $scope.directBooking.ShipTo.FirstName = response.data.ShipTo.FirstName;
                                    $scope.directBooking.ShipTo.LastName = response.data.ShipTo.LastName;
                                    $scope.directBooking.ShipTo.CompanyName = response.data.ShipTo.CompanyName;
                                    $scope.directBooking.ShipTo.Address = response.data.ShipTo.Address;
                                    $scope.directBooking.ShipTo.Address2 = response.data.ShipTo.Address2;
                                    $scope.directBooking.ShipTo.City = response.data.ShipTo.City;
                                    $scope.directBooking.ShipTo.State = response.data.ShipTo.State;
                                    $scope.directBooking.ShipTo.Area = response.data.ShipTo.Area;
                                    $scope.directBooking.ShipTo.Phone = response.data.ShipTo.Phone;
                                    $scope.directBooking.ShipTo.Email = response.data.ShipTo.Email;
                                }
                            }
                        }
                    }
                }, function () {
                });
            }
        }

        if (Country !== undefined && Country !== null) {
            if (Country.Code === 'GBR') {
                $scope.MaximamLengthShipFrom = 9;
            }
            else {
                $scope.MaximamLengthShipFrom = null;
            }

            $scope.showPostCodeDropDown = false;
            for (var i = 0 ; i < $scope.CountryPhoneCodes.length ; i++) {
                if ($scope.CountryPhoneCodes[i].CountryCode === Country.Code) {
                    $scope.ShipFromPhoneCode = "(+" + $scope.CountryPhoneCodes[i].PhoneCode + ")";
                    break;
                }
            }
            setShipFromStatePostCodeForHKGUK(Country);

            if (Action !== undefined && Action !== null && Action !== '') {
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

    //Set ShipTo info
    $scope.SetShipToInfo = function (Country, Action) {
        if (Country && Action) {
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
                $scope.directBooking.ShipTo.Country = null;
                return;
            }

            $scope.customerId = $scope.RoleId === 17 ? $scope.userInfo.EmployeeId : $scope.directBooking.CustomerId;



            if (Action) {
                DirectBookingService.GetCountryDefaultAddress(Country.CountryId, $scope.directBooking.CustomerId, Action).then(function (response) {

                    if (response.data) {
                        if (Action === "ShipFrom") {
                            if (response.data.ShipFrom) {
                                $scope.directBooking.ShipFrom.Country = response.data.ShipFrom.Country;
                                $scope.directBooking.ShipFrom.PostCode = response.data.ShipFrom.PostCode;
                                $scope.directBooking.ShipFrom.FirstName = response.data.ShipFrom.FirstName;
                                $scope.directBooking.ShipFrom.LastName = response.data.ShipFrom.LastName;
                                $scope.directBooking.ShipFrom.CompanyName = response.data.ShipFrom.CompanyName;
                                $scope.directBooking.ShipFrom.Address = response.data.ShipFrom.Address;
                                $scope.directBooking.ShipFrom.Address2 = response.data.ShipFrom.Address2;
                                $scope.directBooking.ShipFrom.City = response.data.ShipFrom.City;
                                $scope.directBooking.ShipFrom.State = response.data.ShipFrom.State;
                                $scope.directBooking.ShipFrom.Area = response.data.ShipFrom.Area;
                                $scope.directBooking.ShipFrom.Phone = response.data.ShipFrom.Phone;
                                $scope.directBooking.ShipFrom.Email = response.data.ShipFrom.Email;
                                $scope.directBooking.ShipFrom.IsMailSend = false;

                            }
                        }
                        if (Action === "ShipTo") {
                            if (response.data.ShipTo) {
                                $scope.directBooking.ShipTo.Country = response.data.ShipTo.Country;
                                $scope.directBooking.ShipTo.PostCode = response.data.ShipTo.PostCode;
                                $scope.directBooking.ShipTo.FirstName = response.data.ShipTo.FirstName;
                                $scope.directBooking.ShipTo.LastName = response.data.ShipTo.LastName;
                                $scope.directBooking.ShipTo.CompanyName = response.data.ShipTo.CompanyName;
                                $scope.directBooking.ShipTo.Address = response.data.ShipTo.Address;
                                $scope.directBooking.ShipTo.Address2 = response.data.ShipTo.Address2;
                                $scope.directBooking.ShipTo.City = response.data.ShipTo.City;
                                $scope.directBooking.ShipTo.State = response.data.ShipTo.State;
                                $scope.directBooking.ShipTo.Area = response.data.ShipTo.Area;
                                $scope.directBooking.ShipTo.Phone = response.data.ShipTo.Phone;
                                $scope.directBooking.ShipTo.Email = response.data.ShipTo.Email;
                            }
                        }
                    }
                }, function () {
                });
            }
        }


        if (Country !== undefined && Country !== null) {
            if (Country.Code === 'GBR') {
                $scope.MaximamLengthShipTo = 9;
            }
            else {
                $scope.MaximamLengthShipTo = null;
            }
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

    $scope.FromCountryStateCode = function (CountryId) {
        DirectBookingService.GetCountryStateName(CountryId).then(function (response) {
            if (response.data !== null && response.data !== undefined && response.data.length > 0) {
                $scope.FromCountryState = response.data;
            }
            else {
                $scope.FromCountryState = null;
            }
        });
    };

    $scope.ToCountryStateCode = function (CountryId) {
        DirectBookingService.GetCountryStateName(CountryId).then(function (response) {
            if (response.data !== null && response.data !== undefined && response.data.length > 0) {
                $scope.ToCountryState = response.data;
            }
            else {
                $scope.ToCountryState = null;
            }
        });
    };

    //Change lb to kg Package Details
    $scope.changeKgToLb = function (pieceDetailOption) {
        if (pieceDetailOption === "kgToCms") {
            // for kg
            $scope.isdisabled = false;

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
            $scope.isdisabled = false;
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

        $scope.customerId = $scope.RoleId === 17 ? $scope.userInfo.EmployeeId : $scope.directBooking.CustomerId;

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBooking/directBookingAddEdit.tpl.html',
            controller: 'DirectBookingAddEditController',
            windowClass: 'DirectBookingDetail',
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
            if (addressBooks !== undefined && addressBooks !== null) {
                if (addressBooks.IsDefault) {
                    $scope.shipFromDefaultAddress = false;
                }

                $scope.directBooking.ShipFrom = addressBooks;
                if (addressBooks.State !== undefined && addressBooks.State !== null && addressBooks.State !== '') {
                    $scope.directBooking.ShipFrom.State = addressBooks.State;
                    DirectBookingService.GetCountryStateName(addressBooks.Country.CountryId).then(function (response) {
                        if (response.data !== null && response.data !== undefined && response.data.length > 0) {
                            $scope.FromCountryState = response.data;
                            for (i = 0; i < $scope.FromCountryState.length; i++) {
                                if ($scope.directBooking.ShipFrom.State === $scope.FromCountryState[i].StateCode) {
                                    $scope.directBooking.ShipFrom.State = $scope.FromCountryState[i].StateCode;
                                }
                            }
                        }
                    });
                }
                $scope.directBooking.ShipFrom.Country = addressBooks.Country;
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

        $scope.customerId = $scope.RoleId === 17 ? $scope.userInfo.EmployeeId : $scope.directBooking.CustomerId;

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'directBooking/directBookingAddEdit.tpl.html',
            controller: 'DirectBookingAddEditController',
            windowClass: 'DirectBookingDetail',
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
            if (addressBooks !== undefined && addressBooks !== null) {
                if (addressBooks.IsDefault) {
                    $scope.shipToDefaultAddress = false;
                }

                $scope.directBooking.ShipTo = addressBooks;
                if (addressBooks.State !== undefined && addressBooks.State !== null && addressBooks.State !== '') {
                    $scope.directBooking.ShipTo.State = addressBooks.State;
                    DirectBookingService.GetCountryStateName(addressBooks.Country.CountryId).then(function (response) {
                        if (response.data !== null && response.data !== undefined && response.data.length > 0) {
                            $scope.ToCountryState = response.data;
                            for (i = 0; i < $scope.ToCountryState.length; i++) {
                                if ($scope.directBooking.ShipTo.State === $scope.ToCountryState[i].StateCode) {
                                    $scope.directBooking.ShipTo.State = $scope.ToCountryState[i].StateCode;
                                }
                            }
                        }
                    });
                }
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
            DirectBookingService.GetCustomerLogisticService(CustomerDetail.CustomerId).then(function (response) {
                if (response.data !== null) {
                    $scope.LogisticCompany = '';
                    for (var h = 0; h < response.data.length; h++) {
                        if (response.data[h].LogisticCompany === 'Yodel') {
                            $scope.LogisticCompany = response.data[h].LogisticCompany;
                        }
                    }
                }
            });

            //DirectBookingService.GetCustomerDefaultDetail(CustomerDetail.CustomerId).then(function (response) {
            //    if (response.data !== null) { 
            //        $scope.CustomerDetail = response.data;
            //        $scope.SetCustomerAddress();
            //        $scope.SetShipFrominfo(response.data.ShipFrom.Country);
            //        if (response.data.ShipTo && response.data.ShipTo.Country) {
            //            $scope.SetShipToInfo(response.data.ShipTo.Country);
            //        }
            //        if (response.data.ShipFrom.IsShipperTaxAndDuty) {
            //            $scope.taxAndDutyDisabled = false;
            //        }
            //        else {
            //            $scope.taxAndDutyDisabled = true;
            //            $scope.directBooking.PayTaxAndDuties = '';
            //            $scope.directBooking.TaxAndDutiesAcceptedBy = '';
            //        }
            //        $scope.CurrencyCode = response.data.ShipFrom.CurrencyCode;
            //        var found = $filter('filter')($scope.CurrencyTypes, { CurrencyCode: $scope.CurrencyCode });
            //        if (found.length) {
            //            $scope.directBooking.Currency = found[0];
            //        }
            //    }
            //}, function () { 
            //});
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

        if (PostCode && PostCode.length > 5 && PostCode.length > $scope.directBooking.ShipFrom.PostCode.length) {
            var pcode = PostCode.split('');

            for (i = 0; i < pcode.length; i++) {
                if (pcode[i] === " ") {
                    pcode.splice(i, 1);
                }
            }

            if (pcode.length >= 0 && $scope.directBooking.ShipFrom.Country !== null && $scope.directBooking.ShipFrom.Country !== undefined && $scope.directBooking.ShipFrom.Country.Code === 'GBR') {
                AppSpinner.showSpinnerTemplate($scope.Loading_Post_Code_Address, $scope.Template);
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
                                response.data[i].FillPostCodeInput = response.data[i].CompanyName + ',' + response.data[i].Address1 + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;
                            }
                            if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '') {
                                response.data[i].FillPostCodeInput = response.data[i].Address2 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;
                            }
                            if (response.data[i].Address1 !== '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].Area === '') {
                                response.data[i].FillPostCodeInput = response.data[i].CompanyName + ', ' + response.data[i].Address1 + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;
                            }
                            if (response.data[i].Address1 !== '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '') {
                                response.data[i].FillPostCodeInput = response.data[i].Address1 + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;
                            }
                            if (response.data[i].Address1 !== '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '' && response.data[i].Area === '') {
                                response.data[i].FillPostCodeInput = response.data[i].Address1 + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;
                            }
                            if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName !== '' && response.data[i].Area === '') {
                                response.data[i].FillPostCodeInput = response.data[i].CompanyName + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].PostCode;
                            }
                            if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '' && response.data[i].Area !== '') {
                                response.data[i].FillPostCodeInput = response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;
                            }
                            if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '' && response.data[i].Area === '') {
                                response.data[i].FillPostCodeInput = response.data[i].CompanyName + ',' + response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;
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
                        AppSpinner.hideSpinnerTemplate();
                        return response.data;
                    }
                    else {
                        AppSpinner.hideSpinnerTemplate();
                        $scope.PostCodeAddressValue = true;
                    }

                });
            }

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

    $scope.SetPostCodeAddressValue = function (Type) {
        if ($scope.fillpostval && $scope.fillpostval.length) {
            $scope.directBooking.ShipFrom.City = $scope.fillpostval[0].City;
        } else {
            $scope.PostCodeAddressValue = false;
        }
    };

    $scope.SetPostCodeNotfoundValidation = function (Type) {
        if ($scope.shipTopostcodelist && $scope.shipTopostcodelist.length) {
            $scope.directBooking.ShipTo.City = $scope.shipTopostcodelist[0].City;
        } else {
            $scope.ShipperPostCodeAddressValue = false;
        }

    };

    $scope.ShiperGetPostCodeAddress = function (ShipperPostCodeAdress) {

        if (ShipperPostCodeAdress && ShipperPostCodeAdress.length > 5 && ShipperPostCodeAdress.length > $scope.directBooking.ShipTo.PostCode.length) {
            var pcode = ShipperPostCodeAdress.split('');
            for (i = 0; i < pcode.length; i++) {
                if (pcode[i] === " ") {
                    pcode.splice(i, 1);
                }
            }
            if (pcode.length >= 0 && $scope.directBooking.ShipTo.Country !== null && $scope.directBooking.ShipTo.Country !== undefined && $scope.directBooking.ShipTo.Country.Code === 'GBR') {
                AppSpinner.showSpinnerTemplate("Loading post code addresses", $scope.Template);
                return DirectBookingService.GetPostCodeAddress(ShipperPostCodeAdress, $scope.directBooking.ShipTo.Country.Code2).then(function (response) {
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
                                response.data[i].FillPostCodeInput = response.data[i].CompanyName + ',' + response.data[i].Address1 + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;
                            }
                            if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '') {
                                response.data[i].FillPostCodeInput = response.data[i].Address2 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;
                            }
                            if (response.data[i].Address1 !== '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].Area === '') {
                                response.data[i].FillPostCodeInput = response.data[i].CompanyName + ', ' + response.data[i].Address1 + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;
                            }
                            if (response.data[i].Address1 !== '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '') {
                                response.data[i].FillPostCodeInput = response.data[i].Address1 + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;
                            }
                            if (response.data[i].Address1 !== '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '' && response.data[i].Area === '') {
                                response.data[i].FillPostCodeInput = response.data[i].Address1 + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;
                            }
                            if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName !== '' && response.data[i].Area === '') {
                                response.data[i].FillPostCodeInput = response.data[i].CompanyName + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].PostCode;
                            }
                            if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '' && response.data[i].Area !== '') {
                                response.data[i].FillPostCodeInput = response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;
                            }
                            if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '' && response.data[i].Area === '') {
                                response.data[i].FillPostCodeInput = response.data[i].CompanyName + ',' + response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;
                            }
                            if (response.data[i].Address1 !== '' && response.data[i].Address2 === '' && response.data[i].City !== '' && response.data[i].PostCode !== '') {
                                response.data[i].FillPostCodeInput = response.data[i].Address1 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;
                            }
                            if (response.data[i].Address1 === '' && response.data[i].Address2 === '' && response.data[i].City !== '' && response.data[i].PostCode !== '') {
                                response.data[i].FillPostCodeInput = response.data[i].City + ', ' + response.data[i].PostCode;
                            }
                        }
                        $scope.ShipperPostCodeAddressValue = false;
                        AppSpinner.hideSpinnerTemplate();
                        $scope.shipTopostcodelist = response.data;
                        return response.data;

                    }
                    else {
                        AppSpinner.hideSpinnerTemplate();
                        $scope.ShipperPostCodeAddressValue = true;
                    }
                });
            }
        }
    };

    $scope.SetService = function (data) {
        var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        if (data.CustomerRateCard.LogisticType === 'UKShipment' && data.CustomerRateCard.CourierName === 'UKMail') {
            $scope.directBooking.PakageCalculatonType = data.CustomerRateCard.PackageCalculationType;
            $scope.directBooking.ShipFrom.Country = data.ShipFrom.Country;
            $scope.directBooking.ShipTo.Country = data.ShipTo.Country;
            $scope.directBooking.CustomerRateCard.ImageURL = config.BUILD_URL + "UKmail.png";
            $scope.directBooking.CustomerRateCard.ShipmentType = data.CustomerRateCard.WeightType + " ( " + data.CustomerRateCard.ParcelServiceType + " ) ";
            $scope.directBooking.CustomerRateCard.FuelMonth = data.CustomerRateCard.FuelMonth;
            $scope.directBooking.CustomerRateCard.FuelSurcharge = data.CustomerRateCard.FuelSurchargePercent;
        }
        else if (data.CustomerRateCard.LogisticType === 'UKShipment' && data.CustomerRateCard.CourierName === 'Yodel') {
            $scope.directBooking.PakageCalculatonType = data.CustomerRateCard.PackageCalculationType;
            $scope.directBooking.ShipFrom.Country = data.ShipFrom.Country;
            $scope.directBooking.ShipTo.Country = data.ShipTo.Country;
            $scope.directBooking.CustomerRateCard.ImageURL = config.BUILD_URL + "yodel.png";
            $scope.directBooking.CustomerRateCard.ShipmentType = data.CustomerRateCard.WeightType + " ( " + data.CustomerRateCard.ParcelServiceType + " ) ";
            $scope.directBooking.CustomerRateCard.FuelMonth = data.CustomerRateCard.FuelMonth;
            $scope.directBooking.CustomerRateCard.FuelSurcharge = data.CustomerRateCard.FuelSurchargePercent;
        }
        else if (data.CustomerRateCard.LogisticType === 'UKShipment' && data.CustomerRateCard.CourierName === 'Hermes') {
            $scope.directBooking.PakageCalculatonType = data.CustomerRateCard.PackageCalculationType;
            $scope.directBooking.ShipFrom.Country = data.ShipFrom.Country;
            $scope.directBooking.ShipTo.Country = data.ShipTo.Country;
            $scope.directBooking.CustomerRateCard.ImageURL = config.BUILD_URL + "hermes.png";
            $scope.directBooking.CustomerRateCard.ShipmentType = data.CustomerRateCard.WeightType + " ( " + data.CustomerRateCard.ParcelServiceType + " ) ";
            $scope.directBooking.CustomerRateCard.FuelMonth = data.CustomerRateCard.FuelMonth;
            $scope.directBooking.CustomerRateCard.FuelSurcharge = data.CustomerRateCard.FuelSurchargePercent;
        }
        else if (data.CustomerRateCard.CourierName === 'DHL') {
            $scope.directBooking.PakageCalculatonType = data.CustomerRateCard.PackageCalculationType;
            $scope.directBooking.ShipFrom.Country = data.ShipFrom.Country;
            $scope.directBooking.ShipTo.Country = data.ShipTo.Country;
            $scope.directBooking.CustomerRateCard.ImageURL = config.BUILD_URL + "DHL.png";
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

    $scope.AddressTypeChange = function (AddressType) {
        $scope.AddressType = AddressType;
    };

    //scroll to top
    window.scrollTo(0, 0);

    function init() {
        $scope.ErrorFormShow = false;
        $scope.HideContent = false;
        $scope.emailFormat = /^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,10}$/;  //  
        $scope.RemainChar = 0;
        $scope.MaxLengthValue = 11;
        $scope.checkcourier = '';
        $scope.checkcustomerRateCard = null;
        $scope.ImagePath = config.BUILD_URL;
        AppSpinner.hideSpinnerTemplate();
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
        $scope.active = 1;
        $scope.BookingType = "";
        $scope.htmlPopover = $sce.trustAsHtml('<b style="color: red">I can</b> have <div class="label label-success">HTML</div> content');
        //$scope.CountryPopOver = $sce.trustAsHtml('<span style : "width : 269px">China (CN1) covers Guangdong Province only.China (CN2) covers all other provinces.<span>');
        //$scope.CountryPopOver = $sce.trustAsHtml("China covers Guangdong Province only.");
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
        //$anchorScroll.yOffset = 200;
        $scope.disableCourier = false;
        $scope.isCollectionTimeValid = true;
        $scope.parcelTypeDisable = false;

        $scope.status = {
            opened: false
        };

        $scope.taxAndDutyDisabled = false;
        //$scope.toggleMin();

        $scope.dateOptions = {
            formatYear: 'yy',
            startingDay: 1,
            minDate: new Date(),
            dateDisabled: function (data) {
                var date = data.date,
                mode = data.mode;
                return mode === 'day' && (date.getDay() === 0);
            }
        };

        //|| date.getDay() === 6

        var toggleMin = function () {
            $scope.dateOptions.minDate = $scope.dateOptions.minDate ? null : new Date();
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
        // new SetMultilingualOtions();
        setMultilingualOptions();
        $scope.AddressType = $scope.AddressTypes[0];

        var userInfo = SessionService.getUser();
        $scope.userInfo = userInfo;

        $scope.tabs = userInfo.tabs;
        $scope.tab = getUserTab($scope.tabs, "QuickDirect_Booking");
        $scope.RoleId = userInfo.RoleId;
        $scope.custid = userInfo.EmployeeId;
        $scope.CreatedBy = userInfo.EmployeeId;
        if ($scope.RoleId === 6) {
            $scope.LogInUserId = userInfo.EmployeeId;
        }
        else {
            $scope.LogInUserId = 0;
        }

        if (isNaN($stateParams.directShipmentId)) {
            $rootScope.isNotFound = true;
        }
        else {

            if ($scope.RoleId !== 3 && $scope.RoleId !== 17) { // Need to select customer from dropdown in case Login (other than customer and UserCustomer )
                $scope.customerId = 0;

                $scope.paymentAccount = true;

                DirectBookingService.GetDirectBookingCustomers(userInfo.EmployeeId, "DirectBooking").then(function (response) {
                    $scope.directBookingCustomers = response.data;

                    var dbCustomers = [];
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
                    if ($scope.directBookingCustomers) {
                        //Get Initial Call
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
                            $scope.ClearButtonEnable = false;

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

                            if (directShipmentId > 0) {
                                var CallingType = '';
                                if ($stateParams.callingtype === 'quotation-shipment') {
                                    CallingType = 'quotation-shipment';
                                    //$scope.OptionalServices = angular.copy($rootScope.OptionalServices);
                                    //$rootScope.OptionalServices = null;
                                }
                                else {

                                    if ($scope.tab !== undefined && $scope.tab !== null && $scope.tab.childTabs !== null) {
                                        CallingType = UtilityService.GetCurrentShipmentType($scope.tab);
                                    }
                                }

                                DirectBookingService.GetShipmentDraftDetail(directShipmentId, CallingType).then(function (response) {

                                    if (response.data.Packages.length === 1) {
                                        $scope.HideContent = false;
                                    }
                                    else {
                                        $scope.HideContent = true;
                                    }

                                    $scope.directBooking = response.data;

                                    //if ($stateParams.callingtype === 'quotation-shipment') {                                        
                                    //    $scope.directBooking.CustomerRateCard.OptionalServices = $scope.OptionalServices;
                                    //}

                                    if ($stateParams.callingtype === 'quotation-shipment' && $scope.directBooking.ParcelType.ParcelType === "Letter") {
                                        $scope.directBooking.Packages[0].Content = 'Document';
                                    }

                                    if (CallingType === "ShipmentClone" || CallingType === "ShipmentReturn") {
                                        if ($scope.directBooking !== null && $scope.directBooking !== undefined) {
                                            if ($scope.directBooking.ShipFrom.Address.length > 35) {
                                                $scope.directBooking.ShipFrom.Address = $scope.directBooking.ShipFrom.Address.substring(0, 34);
                                            }
                                            if ($scope.directBooking.ShipFrom.Address2.length > 35) {
                                                $scope.directBooking.ShipFrom.Address2 = $scope.directBooking.ShipFrom.Address2.substring(0, 34);
                                            }
                                            if ($scope.directBooking.ShipTo.Address.length > 35) {
                                                $scope.directBooking.ShipTo.Address = $scope.directBooking.ShipTo.Address.substring(0, 34);
                                            }
                                            if ($scope.directBooking.ShipTo.Address2.length > 35) {
                                                $scope.directBooking.ShipTo.Address2 = $scope.directBooking.ShipTo.Address2.substring(0, 34);
                                            }
                                            if ($scope.directBooking.ShipFrom.Phone.length > 16) {
                                                $scope.directBooking.ShipFrom.Phone = $scope.directBooking.ShipFrom.Phone.substring(0, 16);
                                            }
                                            if ($scope.directBooking.ShipTo.Phone.length > 16) {
                                                $scope.directBooking.ShipTo.Phone = $scope.directBooking.ShipTo.Phone.substring(0, 16);
                                            }
                                        }
                                        $scope.ErrorFormShow = false;

                                    }

                                    var parcel = $filter('filter')($scope.ParcelTypes, { ParcelType: $scope.directBooking.ParcelType.ParcelType });
                                    if (parcel.length) {
                                        for (i = 0; i < $scope.ParcelTypes.length; i++) {
                                            if ($scope.ParcelTypes[i].ParcelType === parcel[0].ParcelType) {
                                                $scope.directBooking.ParcelType = $scope.ParcelTypes[i];
                                            }
                                        }
                                    }
                                    $scope.checkcustomerRateCard = response.data.CustomerRateCard;
                                    if ($scope.directBooking.ShipmentStatusId === 14) {
                                        $scope.checkcustomerRateCard = response.data.CustomerRateCard;
                                        $scope.checkcourier = '';
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


                                    if (CallingType === 'quotation-shipment') {
                                        response.data.CustomerRateCard.RateValue = response.data.CustomerRateCard.Rate;
                                        response.data.CustomerRateCard.Rate = Number(parseFloat(response.data.CustomerRateCard.Rate).toFixed(2)).toLocaleString('en', { minimumFractionDigits: 2 });
                                        response.data.CustomerRateCard.FuelSurchargeValue = response.data.CustomerRateCard.FuelSurcharge;
                                        response.data.CustomerRateCard.FuelSurcharge = response.data.CustomerRateCard.FuelSurcharge.toFixed(2);
                                        response.data.CustomerRateCard.TotalEstimatedChargeValue = response.data.CustomerRateCard.TotalEstimatedCharge;
                                        response.data.CustomerRateCard.TotalEstimatedCharge = Number(parseFloat(response.data.CustomerRateCard.TotalEstimatedCharge).toFixed(2)).toLocaleString('en', { minimumFractionDigits: 2 });
                                        response.data.CustomerRateCard.AdditionalSurchargeValue = response.data.CustomerRateCard.AdditionalSurcharge;
                                        response.data.CustomerRateCard.AdditionalSurcharge = Number(parseFloat(response.data.CustomerRateCard.AdditionalSurcharge).toFixed(2)).toLocaleString('en', { minimumFractionDigits: 2 });
                                        response.data.CustomerRateCard.FuelSurchargePercent = response.data.CustomerRateCard.FuelSurchargePercent.toFixed(2);
                                        $scope.SetService(response.data);

                                        DirectBookingService.GetCustomerLogisticService($scope.directBooking.CustomerId).then(function (response) {
                                            if (response.data !== null) {
                                                $scope.LogisticCompany = '';
                                                for (var h = 0; h < response.data.length; h++) {
                                                    if (response.data[h].LogisticCompany === 'Yodel') {
                                                        if ($scope.directBooking.AddressType !== undefined && $scope.directBooking.AddressType !== null && $scope.directBooking.AddressType !== '') {
                                                            $scope.LogisticCompany = response.data[h].LogisticCompany;

                                                            for (i = 0; i < $scope.AddressTypes.length; i++) {
                                                                if ($scope.directBooking.AddressType === $scope.AddressTypes[i].Name) {
                                                                    $scope.AddressType = $scope.AddressTypes[i];
                                                                }
                                                            }
                                                        }
                                                        else {
                                                            $scope.LogisticCompany = response.data[h].LogisticCompany;
                                                            $scope.directBooking.AddressType = $scope.AddressTypes[0].Name;
                                                        }
                                                    }
                                                }
                                            }
                                        });
                                    }

                                    if (($scope.RoleId !== 3) && $scope.directBookingCustomers.length) {
                                        for (i = 0; i < $scope.directBookingCustomers.length; i++) {
                                            if ($scope.directBooking.CustomerId === $scope.directBookingCustomers[i].CustomerId) {
                                                $scope.CustomerDetailVal = $scope.directBookingCustomers[i];
                                            }
                                        }

                                        if ($scope.CustomerDetailVal.IsShipperTaxAndDuty) {
                                            $scope.taxAndDutyDisabled = false;
                                        }
                                        else {
                                            $scope.taxAndDutyDisabled = true;
                                        }

                                        DirectBookingService.GetCustomerLogisticService($scope.CustomerDetail.CustomerId).then(function (response) {
                                            if (response.data !== null) {
                                                $scope.LogisticCompany = '';
                                                for (var h = 0; h < response.data.length; h++) {
                                                    if (response.data[h].LogisticCompany === 'Yodel') {
                                                        if ($scope.directBooking.AddressType !== undefined && $scope.directBooking.AddressType !== null && $scope.directBooking.AddressType !== '') {
                                                            $scope.LogisticCompany = response.data[h].LogisticCompany;

                                                            for (i = 0; i < $scope.AddressTypes.length; i++) {
                                                                if ($scope.directBooking.AddressType === $scope.AddressTypes[i].Name) {
                                                                    $scope.AddressType = $scope.AddressTypes[i];
                                                                }
                                                            }
                                                        }
                                                        else {
                                                            $scope.LogisticCompany = response.data[h].LogisticCompany;
                                                            $scope.directBooking.AddressType = $scope.AddressTypes[0].Name;
                                                        }
                                                    }
                                                }
                                            }
                                        });
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

                                    parcel = $filter('filter')($scope.ParcelTypes, { ParcelType: $scope.directBooking.ParcelType.ParcelType });
                                    if (parcel.length) {
                                        $scope.directBooking.ParcelType = parcel[0];
                                    }
                                    if ($scope.directBooking.ReferenceDetail.CollectionDate) {
                                        $scope.directBooking.ReferenceDetail.CollectionDate = new Date(); //  moment.utc($scope.directBooking.ReferenceDetail.CollectionDate).toDate();
                                    }
                                    $scope.directBooking.ReferenceDetail.CollectionTime = "";

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

                                        //Ship From State Code
                                        DirectBookingService.GetCountryStateName($scope.directBooking.ShipFrom.Country.CountryId).then(function (response) {
                                            if (response.data !== null && response.data !== undefined && response.data.length > 0) {
                                                $scope.FromCountryState = response.data;
                                                for (i = 0; i < $scope.FromCountryState.length; i++) {
                                                    if ($scope.directBooking.ShipFrom.State === $scope.FromCountryState[i].StateCode) {
                                                        $scope.directBooking.ShipFrom.State = $scope.FromCountryState[i].StateCode;
                                                    }
                                                }
                                            }
                                        });

                                        //Ship To State Code
                                        DirectBookingService.GetCountryStateName($scope.directBooking.ShipTo.Country.CountryId).then(function (response) {
                                            if (response.data !== null && response.data !== undefined && response.data.length > 0) {
                                                $scope.ToCountryState = response.data;
                                                for (i = 0; i < $scope.ToCountryState.length; i++) {
                                                    if ($scope.directBooking.ShipTo.State === $scope.ToCountryState[i].StateCode) {
                                                        $scope.directBooking.ShipTo.State = $scope.ToCountryState[i].StateCode;
                                                    }
                                                }
                                            }
                                        });
                                    }

                                    $scope.directBooking.RoleId = $scope.RoleId;
                                    $scope.directBooking.LogInUseId = $scope.LogInUserId;
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


                                // $scope.SetCustomerAddress();

                                // Set ShipFrom Phone Code
                                //$scope.SetShipFrominfo($scope.directBooking.ShipFrom.Country);
                            }
                        },
                        function () {
                            AppSpinner.hideSpinnerTemplate();
                            if (response.status !== 401) {
                                toaster.pop({
                                    type: 'error',
                                    title: $scope.TitleFrayteError,
                                    body: $scope.InitialDataValidation,
                                    showCloseButton: true
                                });
                            }
                        });
                    }

                }, function (response) {
                    if (response.status !== 401) {
                        toaster.pop({
                            type: 'error',
                            title: $scope.TitleFrayteError,
                            body: $scope.ReceiveDetail_Validation,
                            showCloseButton: true
                        });
                    }
                });
            }
            if ($scope.RoleId === 17) {
                $scope.customerId = userInfo.EmployeeCustomerId;
                $scope.paymentAccount = false;
            }
            if ($scope.RoleId === 3 || $scope.RoleId === 17) {
                DirectBookingService.GetInitials($scope.RoleId === 17 ? userInfo.EmployeeId : $scope.customerId).then(function (response) {
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

                    $scope.ClearButtonEnable = false;

                    if ($scope.RoleId === 3 || $scope.RoleId === 17) {
                        if ($scope.CustomerDetail !== null && $scope.CustomerDetail.ShipFrom.IsShipperTaxAndDuty) {
                            $scope.taxAndDutyDisabled = false;
                        }
                        else if ($scope.CustomerDetail !== null && !$scope.CustomerDetail.ShipFrom.IsShipperTaxAndDuty) {
                            $scope.taxAndDutyDisabled = true;
                        }
                        if ($scope.CustomerDetail !== null && $scope.CustomerDetail.ShipFrom.IsRateShow) {
                            if ($scope.RoleId === 17) {
                                $scope.IsRateShow = false;
                            }
                            else {
                                $scope.IsRateShow = true;
                            }
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

                            if (response.data.Packages.length === 1) {
                                $scope.HideContent = false;
                            }
                            else {
                                $scope.HideContent = true;
                            }
                            $scope.directBooking = response.data;

                            if ($scope.CustomerDetail && $scope.CustomerDetail.ShipFrom && $scope.CustomerDetail.ShipFrom.IsDefaultAddress) {
                                $scope.shipFromDefaultAddress = false;
                            }
                            else {
                                $scope.shipFromDefaultAddress = true;
                            }
                            if ($scope.CustomerDetail && $scope.CustomerDetail.ShipTo && $scope.CustomerDetail.ShipTo.IsDefaultAddress) {
                                $scope.shipToDefaultAddress = false;
                            }
                            else {
                                $scope.shipToDefaultAddress = true;
                            }

                            if ($stateParams.callingtype === 'quotation-shipment' && $scope.directBooking.ParcelType.ParcelType === "Letter") {
                                $scope.directBooking.Packages[0].Content = 'Document';
                            }

                            var parcel = $filter('filter')($scope.ParcelTypes, { ParcelType: $scope.directBooking.ParcelType.ParcelType });
                            if (parcel.length) {
                                for (i = 0; i < $scope.ParcelTypes.length; i++) {
                                    if ($scope.ParcelTypes[i].ParcelType === parcel[0].ParcelType) {
                                        $scope.directBooking.ParcelType = $scope.ParcelTypes[i];
                                    }
                                }
                            }
                            $scope.checkcustomerRateCard = response.data.CustomerRateCard;
                            if ($scope.directBooking.ShipmentStatusId === 14) {
                                $scope.checkcustomerRateCard = response.data.CustomerRateCard;
                                $scope.checkcourier = '';
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
                                response.data.CustomerRateCard.RateValue = response.data.CustomerRateCard.Rate;
                                response.data.CustomerRateCard.Rate = Number(parseFloat(response.data.CustomerRateCard.Rate).toFixed(2)).toLocaleString('en', { minimumFractionDigits: 2 });
                                response.data.CustomerRateCard.FuelSurchargeValue = response.data.CustomerRateCard.FuelSurcharge;
                                response.data.CustomerRateCard.FuelSurcharge = response.data.CustomerRateCard.FuelSurcharge.toFixed(2);
                                response.data.CustomerRateCard.TotalEstimatedChargeValue = response.data.CustomerRateCard.TotalEstimatedCharge;
                                response.data.CustomerRateCard.TotalEstimatedCharge = Number(parseFloat(response.data.CustomerRateCard.TotalEstimatedCharge).toFixed(2)).toLocaleString('en', { minimumFractionDigits: 2 });
                                response.data.CustomerRateCard.AdditionalSurchargeValue = response.data.CustomerRateCard.AdditionalSurcharge;
                                response.data.CustomerRateCard.AdditionalSurcharge = Number(parseFloat(response.data.CustomerRateCard.AdditionalSurcharge).toFixed(2)).toLocaleString('en', { minimumFractionDigits: 2 });
                                response.data.CustomerRateCard.FuelSurchargePercent = response.data.CustomerRateCard.FuelSurchargePercent.toFixed(2);

                                $scope.SetService(response.data);

                                DirectBookingService.GetCustomerLogisticService($scope.RoleId === 17 ? $scope.directBooking.CreatedBy : $scope.directBooking.CustomerId).then(function (response) {
                                    if (response.data !== null) {
                                        $scope.LogisticCompany = '';
                                        for (var h = 0; h < response.data.length; h++) {
                                            if (response.data[h].LogisticCompany === 'Yodel') {
                                                if ($scope.directBooking.AddressType !== undefined && $scope.directBooking.AddressType !== null && $scope.directBooking.AddressType !== '') {
                                                    $scope.LogisticCompany = response.data[h].LogisticCompany;

                                                    for (i = 0; i < $scope.AddressTypes.length; i++) {
                                                        if ($scope.directBooking.AddressType === $scope.AddressTypes[i].Name) {
                                                            $scope.AddressType = $scope.AddressTypes[i];
                                                        }
                                                    }
                                                }
                                                else {
                                                    $scope.LogisticCompany = response.data[h].LogisticCompany;
                                                    $scope.directBooking.AddressType = $scope.AddressTypes[0].Name;
                                                }
                                            }
                                        }
                                    }
                                });
                            }

                            if (($scope.RoleId !== 3 && $scope.RoleId !== 17) && $scope.directBookingCustomers.length) {
                                for (i = 0; i < $scope.directBookingCustomers.length; i++) {
                                    if ($scope.directBooking.CustomerId === $scope.directBookingCustomers[i].CustomerId) {
                                        $scope.CustomerDetail = $scope.directBookingCustomers[i];
                                    }
                                }

                                //Get Customer Logistic Services
                                DirectBookingService.GetCustomerLogisticService($scope.directBooking.CustomerId).then(function (response) {
                                    if (response.data !== null) {
                                        $scope.LogisticCompany = '';
                                        for (var h = 0; h < response.data.length; h++) {
                                            if (response.data[h].LogisticCompany === 'Yodel') {
                                                if ($scope.directBooking.AddressType !== undefined && $scope.directBooking.AddressType !== null && $scope.directBooking.AddressType !== '') {
                                                    $scope.LogisticCompany = response.data[h].LogisticCompany;

                                                    for (i = 0; i < $scope.AddressTypes.length; i++) {
                                                        if ($scope.directBooking.AddressType === $scope.AddressTypes[i].Name) {
                                                            $scope.AddressType = $scope.AddressTypes[i];
                                                        }
                                                    }
                                                }
                                                else {
                                                    $scope.LogisticCompany = response.data[h].LogisticCompany;
                                                    $scope.directBooking.AddressType = $scope.AddressTypes[0].Name;
                                                }
                                            }
                                        }
                                    }
                                });
                            }
                            else {
                                //Get Customer Logistic Services
                                DirectBookingService.GetCustomerLogisticService($scope.RoleId === 17 ? $scope.directBooking.CreatedBy : $scope.directBooking.CustomerId).then(function (response) {
                                    if (response.data !== null) {
                                        $scope.LogisticCompany = '';
                                        for (var h = 0; h < response.data.length; h++) {
                                            if (response.data[h].LogisticCompany === 'Yodel') {
                                                if ($scope.directBooking.AddressType !== undefined && $scope.directBooking.AddressType !== null && $scope.directBooking.AddressType !== '') {
                                                    $scope.LogisticCompany = response.data[h].LogisticCompany;

                                                    for (i = 0; i < $scope.AddressTypes.length; i++) {
                                                        if ($scope.directBooking.AddressType === $scope.AddressTypes[i].Name) {
                                                            $scope.AddressType = $scope.AddressTypes[i];
                                                        }
                                                    }
                                                }
                                                else {
                                                    $scope.LogisticCompany = response.data[h].LogisticCompany;
                                                    $scope.directBooking.AddressType = $scope.AddressTypes[0].Name;
                                                }
                                            }
                                        }
                                    }
                                });
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

                            parcel = $filter('filter')($scope.ParcelTypes, { ParcelType: $scope.directBooking.ParcelType.ParcelType });
                            if (parcel.length) {
                                $scope.directBooking.ParcelType = parcel[0];
                            }
                            if ($scope.directBooking.ReferenceDetail.CollectionDate !== undefined &&
                                $scope.directBooking.ReferenceDetail.CollectionDate !== null) {
                                $scope.directBooking.ReferenceDetail.CollectionDate = moment.utc($scope.directBooking.ReferenceDetail.CollectionDate).toDate();
                            }

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

                                //Ship From State Code
                                DirectBookingService.GetCountryStateName($scope.directBooking.ShipFrom.Country.CountryId).then(function (response) {
                                    if (response.data !== null && response.data !== undefined && response.data.length > 0) {
                                        $scope.FromCountryState = response.data;
                                        for (i = 0; i < $scope.FromCountryState.length; i++) {
                                            if ($scope.directBooking.ShipFrom.State === $scope.FromCountryState[i].StateCode) {
                                                $scope.directBooking.ShipFrom.State = $scope.FromCountryState[i].StateCode;
                                            }
                                        }
                                    }
                                });

                                //Ship To State Code
                                DirectBookingService.GetCountryStateName($scope.directBooking.ShipTo.Country.CountryId).then(function (response) {
                                    if (response.data !== null && response.data !== undefined && response.data.length > 0) {
                                        $scope.ToCountryState = response.data;
                                        for (i = 0; i < $scope.ToCountryState.length; i++) {
                                            if ($scope.directBooking.ShipTo.State === $scope.ToCountryState[i].StateCode) {
                                                $scope.directBooking.ShipTo.State = $scope.ToCountryState[i].StateCode;
                                            }
                                        }
                                    }
                                });
                            }

                            $scope.directBooking.RoleId = $scope.RoleId;
                            $scope.directBooking.LogInUseId = $scope.LogInUserId;
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
                        if ($scope.RoleId === 3 || $scope.RoleId === 17) {
                            $scope.directBooking.CustomerId = $scope.customerId;

                            //Get Customer Logistic Services
                            DirectBookingService.GetCustomerLogisticService($scope.directBooking.CustomerId).then(function (response) {
                                if (response.data !== null) {
                                    $scope.LogisticCompany = '';
                                    for (var h = 0; h < response.data.length; h++) {
                                        if (response.data[h].LogisticCompany === 'Yodel') {
                                            $scope.LogisticCompany = response.data[h].LogisticCompany;
                                        }
                                    }
                                }
                            });
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

                        //   $scope.SetCustomerAddress();
                        // Set ShipFrom Phone Code
                        //   $scope.SetShipFrominfo($scope.directBooking.ShipFrom.Country);
                    }
                },
                  function (response) {
                      AppSpinner.hideSpinnerTemplate();
                      if (response.status !== 401) {
                          toaster.pop({
                              type: 'error',
                              title: $scope.TitleFrayteError,
                              body: $scope.InitialDataValidation,
                              showCloseButton: true
                          });
                      }
                  });
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
                   name: 'By Default Abandon',
                   value: 'abandon'
               },
               {
                   name: 'Return to Sender',
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

            $scope.disableContent();
            $scope.directBooking.CustomInfo.RestrictionType = $scope.RestrictionType[0];
            $scope.directBooking.CustomInfo.RestrictionComments = 'N/A';

            $rootScope.GetServiceValue = '';
            $scope.ClearButtonEnable = true;
            $scope.Template = 'directBooking/ajaxLoader.tpl.html';
            $rootScope.QuoteCustomerratecard = {};
            if ($rootScope.QuoteCustomerratecard !== null && $rootScope.QuoteCustomerratecard !== undefined) {
                $scope.directBooking.CustomerRateCard = $rootScope.QuoteCustomerratecard;
            }
            window.scrollTo(0, 0);

            //$scope.setScroll('top');
            //$anchorScroll.yOffset = 700;
        }
    }

    init();
});