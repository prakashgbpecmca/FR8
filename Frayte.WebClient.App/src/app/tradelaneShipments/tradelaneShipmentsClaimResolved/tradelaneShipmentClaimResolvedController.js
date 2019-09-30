angular.module('ngApp.tradelaneShipments').controller('TradelaneClaimResolvedController', function ($scope, $translate, $uibModal, toaster, ModalService, SessionService, TradelaneShipmentService, config, ShipmentInfo, TradelaneBookingService, $uibModalInstance, CustomerService, UtilityService, AppSpinner) {


    //translation code here
    var setMultilingualOptions = function () {
        $translate(['FrayteSuccess', 'FrayteWarning', 'FrayteError', 'ClaimResolved_EmailSend_Successfully', 'PleaseCorrectValidationErrors']).then(function (translations) {

            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.FrayteError = translations.FrayteError;
            $scope.Claim_Email_Send_Successfully_To_Agent = translations.ClaimResolved_EmailSend_Successfully;
            $scope.PleaseCorrectValidationErrors = translations.PleaseCorrectValidationErrors;
        });
    };

    //end

    $scope.SendClaimResolvedMailToAgent = function (isValid, ClaimModel) {
        if (isValid && !$scope.valid) {
            AppSpinner.showSpinnerTemplate($scope.SendClaimResolvedMessage, $scope.Template);
            TradelaneShipmentService.SendResolvedClaimShipment(ClaimModel).then(function (response) {
                if (response.data.Status) {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.Claim_Email_Send_Successfully_To_Agent,
                        showCloseButton: true
                    });
                    $uibModalInstance.close({ reload: true });
                }

            }, function (response) {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.PleaseCorrectValidationErrors,
                    showCloseButton: true
                });
            });
        }
        else {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteError,
                body: $scope.PleaseCorrectValidationErrors,
                showCloseButton: true
            });
        }

    };

    //var getCustomerDetail = function (tradelaneBookingDetail) {
    //    CustomerService.GetCustomerDetail($scope.tradelaneBookingDetail.CustomerId).then(function (response) {
    //        if (response.data) {
    //            $scope.CustomerDetail = response.data;
    //            $scope.ShipmentClaim.Subject = $scope.CustomerDetail.CompanyName + "_Tradelane_Shipment_Claim_<" + tradelaneBookingDetail.FrayteNumber;
    //        }
    //    }, function () {

    //    });
    //};

    var getTradelaneBookingDetail = function () {
        CallingType = "";
        TradelaneBookingService.GetBookingDetail($scope.ShipmentClaim.TradelaneShipmentId, '').then(function (response) {

            $scope.tradelaneBookingDetail = response.data;
            //getCustomerDetail($scope.tradelaneBookingDetail);
            //$scope.ShipmentClaim.Subject = "Tradelane Shipment Claim Resolved" + " - " + $scope.UserName + " - " + $scope.tradelaneBookingDetail.FrayteNumber;
            var Mawbvar = "";
            if ($scope.tradelaneBookingDetail.MAWB !== undefined && $scope.tradelaneBookingDetail.MAWB !== null && $scope.tradelaneBookingDetail.MAWB !== "") {
                Mawbvar = "MAWB# " + $scope.tradelaneBookingDetail.AirlinePreference.AilineCode + " " + $scope.tradelaneBookingDetail.MAWB.substring(0, 4) + " " + $scope.tradelaneBookingDetail.MAWB.substring(4, 8);
            }
            else {
                Mawbvar = $scope.tradelaneBookingDetail.FrayteNumber;
            }
            $scope.ShipmentClaim.Subject = Mawbvar + " (" + $scope.tradelaneBookingDetail.DepartureAirport.AirportCode + " To " + $scope.tradelaneBookingDetail.DestinationAirport.AirportCode + ")" + " - " + $scope.CustDetail.CompanyName + ": " + $scope.CustDetail.ContactName + " - Shipment Claim Resolved";
            $scope.totalweight = 0;
            $scope.totalCartons = 0;
            $scope.totalVolume = 0;
            for (i = 0; i < $scope.tradelaneBookingDetail.HAWBPackages.length; i++) {
                //$scope.tradelaneBookingDetail.HAWBPackages[i].TotalVolume = $scope.tradelaneBookingDetail.HAWBPackages[i].TotalVolume.toFixed(2);
                $scope.totalweight = $scope.totalweight + $scope.tradelaneBookingDetail.HAWBPackages[i].EstimatedWeight;
                $scope.totalVolume = $scope.totalVolume + $scope.tradelaneBookingDetail.HAWBPackages[i].TotalVolume;
                $scope.totalCartons = $scope.totalCartons + $scope.tradelaneBookingDetail.HAWBPackages[i].PackagesCount;
            }
            $scope.totalVolume = $scope.totalVolume.toFixed(2);
            if (!parseFloat($scope.tradelaneBookingDetail.DeclaredValue)) {
                $scope.tradelaneBookingDetail.DeclaredValue = null;
            }
            if ($scope.tradelaneBookingDetail.ShipmentStatusId !== 27) {
                //getTotalShipments();
            }
            else {
                //getTotalShipments();
            }
            if ($scope.tradelaneBookingDetail.PakageCalculatonType = "kgToCms") {

                $translate('kGS').then(function (kGS) {
                    $scope.Lb_Kgs = kGS;
                });
                $translate('KG').then(function (KG) {

                    $scope.Lb_Kg = KG;
                });

            }
            else if ($scope.tradelaneBookingDetail.PakageCalculatonType = "lbToInchs") {

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
                body: $scope.PleaseCorrectValidationErrors,
                showCloseButton: true
            });
        });
    };

    $scope.validateEmailList = function (raw) {
        if (raw !== undefined && raw !== null) {
            var emails = raw.split(',');

            $scope.valid = false;
            var regex = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

            for (var i = 0; i < emails.length; i++) {
                if (emails[i] === "" || !regex.test(emails[i].replace(/\s/g, ""))) {
                    $scope.valid = true;
                }
            }
            return $scope.valid;
        }

    };

    function init() {
        $scope.submitted = true;
        var userInfo = SessionService.getUser();
        $scope.UserName = userInfo.EmployeeName;
        $scope.CustomerId = userInfo.EmployeeId;
        if (userInfo.OperationZoneId === 1) {

            //$scope.Websitefrayte = UtilityService.getPublicSiteName();
            $scope.Websitefrayte = config.Public_Link;
            $scope.Website = "www.FRAYTE.com";
            $scope.Phone = "(+852) 2148 4880";
        }
        else {
            //$scope.Websitefrayte = UtilityService.getPublicSiteName();
            $scope.Websitefrayte = config.Public_Link;
            $scope.Website = "www.FRAYTE.co.uk";
            $scope.Phone = "(+44) 01792 277295";
        }

        setMultilingualOptions();
        $scope.emailFormat = /^[a-z0-9._]+@[a-z]+\.[a-z.]/;
        //$scope.emailFormat = /^(\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]{2,4}\s*?,?\s*?)+$/;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.SendClaimResolvedMessage = "Claim resolved is processing";
        //$scope.OperationZoneId = userInfo.OperationZoneId;
        $scope.ShipmentClaim = {
            TradelaneShipmentId: ShipmentInfo.TradelaneShipmentId,
            AgentId: 0,
            ToEmail: "",
            CC: "",
            BCC: "",
            Subject: "",
            Description: ""
        };
        CustomerService.GetCustomerDetail(ShipmentInfo.CustomerId).then(function (response) {
            $scope.CustDetail = response.data;
            $scope.ShipmentClaim.ToEmail = response.data.UserEmail != null ? response.data.UserEmail : "";
            $scope.customerDetail = response.data.OperationUser;
            //$scope.PhoneNo = response.data.OperationUser.TelephoneNo;
            //$scope.CompanyName = response.data.OperationUser.CompanyName;
            //$scope.UserEmail = response.data.OperationUser.Email;
            //$scope.UserName = response.data.OperationUser.ContactName;
            $scope.Position = response.data.OperationUser.Position;
            if ($scope.CustDetail !== undefined && $scope.CustDetail !== null) {
                getTradelaneBookingDetail();
            }
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGetting,
                //title: 'Frayte-Error',
                //body: 'Error while getting customer detail',
                showCloseButton: true
            });
        });

    

        TradelaneShipmentService.GetAgentMail(ShipmentInfo.TradelaneShipmentId).then(function (response) {
            $scope.ShipmentClaim.CC = response.data.staff != null ? response.data.staff : "";
            //$scope.AgentName = response.data.AgentName != null ? response.data.AgentName : "";
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGetting,
                //title: 'Frayte-Error',
                //body: 'Error while getting customer detail',
                showCloseButton: true
            });
        });


        
    }
    init();
});