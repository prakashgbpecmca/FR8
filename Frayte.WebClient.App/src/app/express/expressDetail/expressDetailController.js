angular.module('ngApp.express').controller("ExpressDetailController", function ($scope, $uibModal, AppSpinner, $translate, SessionService, toaster, $uibModalInstance, ModalService, config, ShipmentId, CustomerService, ExpressBookingService) {

    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteSuccess', 'FrayteWarning', 'EnterValidEmailAdd', 'GettingDetails_Error', 'SuccessfullySendlLabel_Validation',
            'SendingMailError_Validation', 'SendingEmail','SendingEmail_Label']).then(function (translations) {
            $scope.FrayteError = translations.FrayteError;
            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.EnterValidEmailAdd = translations.EnterValidEmailAdd;
            $scope.GettingDetailsError = translations.GettingDetails_Error;
            $scope.SuccessfullySendlLabelValidation = translations.SuccessfullySendlLabel_Validation;
            $scope.SendingMailErrorValidation = translations.SendingMailError_Validation;
            $scope.SendingEmail = translations.SendingEmail;
            $scope.SendingEmail_Label = translations.SendingEmail_Label;

            getexpressBookingDetail();
        });
    };


    // Pacakge Detail 
    $scope.alerMethod = function (Pakage) {

        for (var a = 0 ; a < $scope.expressBookingDetail.Packages.length; a++) {
            if ($scope.expressBookingDetail.Packages[a].IsSelected) {
                $scope.expressBookingDetail.Packages[a].IsSelected = false;
            }
        }
        Pakage.IsSelected = true;
    };
    $scope.IsPrinted = function (Pakage) {
        if (Pakage) {
            return Pakage.IsDownloaded;
        }
        return false;
    };

    $scope.setPrintLabel = function (Pakage, type) {
        ExpressBookingService.PrintLabel(Pakage.ExpressDetailPackageLabelId, type).then(function (response) {
            if (response.data && response.data.Status) {
                Pakage.IsDownloaded = true;
            }
        }, function () {

        });
    };
    
    $scope.getTotalWeightKgs = function () {
        if ($scope.expressBookingDetail === undefined) {
            return;
        }
        else if ($scope.expressBookingDetail.Packages === undefined || $scope.expressBookingDetail.Packages === null) {
            return 0;
        }
        else if ($scope.expressBookingDetail.Packages.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.expressBookingDetail.Packages.length; i++) {
                var product = $scope.expressBookingDetail.Packages[i];
                if (product.Weight === null || product.Weight === undefined) {
                    total += parseFloat(0);
                }
                else {
                    if (product.CartonValue === undefined || product.CartonValue === null) {
                        var catroon = parseFloat(0);
                        total = total + parseFloat(product.Weight) * catroon;
                    }
                    else {
                        total = total + parseFloat(product.Weight) * product.CartonValue;
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
        if ($scope.expressBookingDetail === undefined) {
            return;
        }
        else if ($scope.expressBookingDetail.Packages === undefined || $scope.expressBookingDetail.Packages === null) {
            return 0;
        }
        else if ($scope.expressBookingDetail.Packages.length >= 1) {
            var total = parseFloat(0);
            for (var i = 0; i < $scope.expressBookingDetail.Packages.length; i++) {
                var product = $scope.expressBookingDetail.Packages[i];
                if (product.Weight === null || product.Weight === undefined) {
                    total += parseFloat(0);
                }
                else {
                    if (product.CartonValue === undefined || product.CartonValue === null) {
                        var carton = parseFloat(0);
                        total = total + parseFloat(product.Weight) * carton;
                    }
                    else {
                        total = total + parseFloat(product.Weight) * product.CartonValue;
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

    $scope.totalPieces = function (directBooking) {
        if (directBooking !== undefined && directBooking !== null && directBooking.Packages !== null && directBooking.Packages.length) {
            var sum = 0;
            for (var i = 0; i < directBooking.Packages.length; i++) {
                if (directBooking.Packages[i].CartonValue !== "" && directBooking.Packages[i].CartonValue !== null && directBooking.Packages[i].CartonValue !== undefined) {
                    sum += Math.abs(parseInt(directBooking.Packages[i].CartonValue, 10));
                }
            }
            return sum;
        }
        else {
            return 0;
        }
    };

    // Customer Detail 
    var getCustomerDetail = function () {
        CustomerService.GetCustomerDetail($scope.expressBookingDetail.CustomerId).then(function (response) {
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

    // Shipment Detail 
    var getexpressBookingDetail = function () {
        CallingType = "";
        ExpressBookingService.GetBookingDetail($scope.shipmentId, '').then(function (response) {

            $scope.expressBookingDetail = response.data;
            for (var a = 0 ; a < $scope.expressBookingDetail.Packages.length; a++) {
                if ($scope.expressBookingDetail.Packages[a].IsSelected) {
                    $scope.expressBookingDetail.Packages[a].IsSelected = false;
                }
            }
            getCustomerDetail();
            if ($scope.expressBookingDetail.PakageCalculatonType = "kgToCms") {

                $translate('kGS').then(function (kGS) {
                    $scope.Lb_Kgs = kGS;
                });
                $translate('KG').then(function (KG) {

                    $scope.Lb_Kg = KG;
                });

            }
            else if ($scope.expressBookingDetail.PakageCalculatonType = "lbToInchs") {

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
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.GettingDetailsError,
                showCloseButton: true
            });
        });
    };

    $scope.submit = function () {
        $uibModalInstance.close();
    };

    $scope.closePage = function () {
        $uibModalInstance.close();
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
    //


    //breakbulk purchase order detail code
    $scope.purchaseOrderDetail = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            //controller: 'customerAddressBookAddEditController',
            templateUrl: 'breakbulk/details/breakbulkPurchaseOrderDetail.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end

    //view confirm purchase order detail code
    $scope.viewConfirmPurchaseOrder = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            //controller: 'customerAddressBookAddEditController',
            templateUrl: 'breakbulk/details/viewConfirmPurchaseOrder.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end

    $scope.sendLabelMail = function (IsValid, ToMail) {
        if (IsValid) {
            AppSpinner.showSpinnerTemplate($scope.SendingEmail_Label, $scope.Template);

            var obj = {
                ShipmentId: $scope.shipmentId,
                UserId: $scope.userInfo.EmployeeId,
                Email: ToMail
            };

            ExpressBookingService.SendLabelEmail(obj).then(function (response) {
                AppSpinner.hideSpinnerTemplate();
                if (response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.SuccessfullySendlLabelValidation,
                        showCloseButton: true
                    });
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteError,
                        body: $scope.SendingMailErrorValidation,
                        showCloseButton: true
                    });
                }
            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.SendingMailErrorValidation,
                    showCloseButton: true
                });
            });
        }
        else {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.EnterValidEmailAdd,
                showCloseButton: true
            });
        }
    };

    function init() {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.userInfo = SessionService.getUser();
        $scope.sendMail = false;
        $scope.ImagePath = config.BUILD_URL;
        $scope.shipmentId = ShipmentId;

        setMultilingualOptions();
    }
    init();

});
