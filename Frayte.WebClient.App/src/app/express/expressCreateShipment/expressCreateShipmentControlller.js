angular.module('ngApp.express').controller("ExpressCreateShipmentController", function ($scope, $uibModal, UserService, SessionService, AppSpinner, Upload, config, $translate, PreAlertService, CustomerId, Hub, Bags, TradelaneBookingService, ExpressIntegrationShipmentService, toaster, TradelaneShipmentService, TopAirlineService, $uibModalInstance) {

    //multilingual translation key
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteWarning', 'FrayteInformation', 'FrayteSuccess', 'CannotRemoveDetailAtleastTwoFlight', 'DeletingProblem', 'DeletedSuccessfully', 'ConfirmationDeleteFlight',
            'SureDeleteGivenFlightDetail', 'MaxAddThreeFlight', 'FillMandataryFieldsStar', 'AgentSavedSuccessfully', 'Agent_Same', 'MAWBSavedSuccessfully',
            'AgentSame', 'DocumentUploadedSuccessfully', 'ErrorUploadingDocument', 'Errorwhil_uploading_the_excel', 'DocumentAleadyUploadedFor', 'Successfully_Deleted_Document',
            'Error_Deleting_Document', 'SuccessfullyUploadedForm', 'ErrorUploadingDocumentTryAgain', 'GettingDetails_Error', 'InitialDataValidation', 'PleaseSelectValidFile',
            'Loading_Create_Manifest', 'Loading_Allocating_Agent_Shipment', 'UploadingDocument', 'Document_Already_Uploaded', 'Manifest_Created_Successfully']).then(function (translations) {
                $scope.FrayteWarning = translations.FrayteWarning;
                $scope.FrayteSuccess = translations.FrayteSuccess;
                $scope.FrayteError = translations.FrayteError;
                $scope.CannotRemoveDetailAtleastTwoFlight = translations.CannotRemoveDetailAtleastTwoFlight;
                $scope.DeletingProblem = translations.DeletingProblem;
                $scope.AgentSame = translations.Agent_Same;
                $scope.DeletedSuccessfully = translations.DeletedSuccessfully;
                $scope.ConfirmationDeleteFlight = translations.ConfirmationDeleteFlight;
                $scope.SureDeleteGivenFlightDetail = translations.SureDeleteGivenFlightDetail;
                $scope.MaxAddThreeFlight = translations.MaxAddThreeFlight;
                $scope.FillMandataryFieldsStar = translations.FillMandataryFieldsStar;
                $scope.MAWBSavedSuccessfully = translations.AgentSavedSuccessfully;
                $scope.DocumentUploadedSuccessfully = translations.DocumentUploadedSuccessfully;
                $scope.ErrorUploadingDocument = translations.ErrorUploadingDocument;
                $scope.Errorwhil_uploading_the_excel = translations.Errorwhil_uploading_the_excel;
                $scope.DocumentAleadyUploadedFor = translations.DocumentAleadyUploadedFor;
                $scope.Successfully_Deleted_Document = translations.Successfully_Deleted_Document;
                $scope.Error_Deleting_Document = translations.Error_Deleting_Document;
                $scope.SuccessfullyUploadedForm = translations.SuccessfullyUploadedForm;
                $scope.ErrorUploadingDocumentTryAgain = translations.ErrorUploadingDocumentTryAgain;
                $scope.GettingDetails_Error = translations.GettingDetails_Error;
                $scope.InitialDataValidation = translations.InitialDataValidation;
                $scope.PleaseSelectValidFile = translations.PleaseSelectValidFile;
                $scope.UploadingDocument = translations.UploadingDocument;
                $scope.Loading_Create_Manifest = translations.Loading_Create_Manifest;
                $scope.Loading_Allocating_Agent_Shipment = translations.Loading_Allocating_Agent_Shipment;
                $scope.Document_Already_Uploaded = translations.Document_Already_Uploaded;
                $scope.Manifest_Created_Successfully = translations.Manifest_Created_Successfully;

                initials();

            });
    };

    //add edit user
    $scope.AddEditUser = function () {
        modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'user/userDetail.tpl.html',
            controller: 'UserDetailController',
            windowClass: 'DirectBookingDetail',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                RoleId: function () {
                    return $scope.UserRoleId;
                },
                UserId: function () {
                    return 0;
                },
                SystemRoles: function () {
                    return $scope.SystemRoles;
                }
            }
        });

        $scope.GetAgent();
    };
    //end

    $scope.bagWeightTotal = function () {

        if ($scope.tradelaneBookingIntegration && $scope.tradelaneBookingIntegration.Shipment && $scope.tradelaneBookingIntegration.Shipment.Packages.length) {
            var total = 0;
            for (var i = 0; i < $scope.tradelaneBookingIntegration.Shipment.Packages.length; i++) {
                total += parseFloat($scope.tradelaneBookingIntegration.Shipment.Packages[i].Weight);
            }
            return total.toFixed(2);
        }
        return;
    };

    $scope.GetShipments = function (BagId) {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'ExpressBagShipmentsController',
            templateUrl: 'express/expressManifest/expressManifestShipment.tpl.html',
            keyboard: true,
            windowClass: 'directBookingDetail',
            size: 'lg',
            resolve: {
                BagId: function () {
                    return BagId;
                }
            }
        });
    };

    $scope.CheckSameAgent = function (WithLegJson, LegType) {
        if (WithLegJson.FirstLeg.MawbMainObj.Agent !== null && WithLegJson.SecondLeg.MawbMainObj.Agent !== null && WithLegJson.FirstLeg.MawbMainObj.Agent.CustomerName == WithLegJson.SecondLeg.MawbMainObj.Agent.CustomerName && LegType === "Leg1") {
            $scope.LegJsonValue1 = true;
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.AgentSame,
                showCloseButton: true
            });
        }
        else if (WithLegJson.FirstLeg.MawbMainObj.Agent !== null && WithLegJson.SecondLeg.MawbMainObj.Agent !== null && WithLegJson.FirstLeg.MawbMainObj.Agent.CustomerName == WithLegJson.SecondLeg.MawbMainObj.Agent.CustomerName && LegType === "Leg2") {
            $scope.LegJsonValue2 = true;
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.AgentSame,
                showCloseButton: true
            });
        }
        else {
            $scope.LegJsonValue1 = false;
            $scope.LegJsonValue2 = false;
        }
    };

    $scope.ChangeAirlineCode = function (Ary, index) {
        if (index === 0) {
            for (i = 0; i < $scope.AirlineList.length; i++) {
                if (Ary.AirlineId === $scope.AirlineList[i].AirlineId) {
                    $scope.AirlineCode = $scope.AirlineList[i].AilineCode;
                    //$scope.AirlineCode2 = $scope.AirlineList[i].CarrierCode2;
                    Ary.AirlineCode2 = $scope.AirlineList[i].CarrierCode2;
                    break;
                }
            }
        }
    };

    $scope.ChangeAirlineCodeLeg = function (Ary, index, Leg) {
        if (index === 0 && Leg === 'Leg1') {
            for (i = 0; i < $scope.AirlineList.length; i++) {
                if (Ary.AirlineId === $scope.AirlineList[i].AirlineId) {
                    $scope.AirlineCodeLegOne = $scope.AirlineList[i].AilineCode;
                    //$scope.AirlineCode2LegOne = $scope.AirlineList[i].CarrierCode2;
                    Ary.AirlineCode2 = $scope.AirlineList[i].CarrierCode2;
                    break;
                }
            }
        }
        if (index === 0 && Leg === 'Leg2') {
            for (i = 0; i < $scope.AirlineList.length; i++) {
                if (Ary.AirlineId === $scope.AirlineList[i].AirlineId) {
                    $scope.AirlineCodeLegTwo = $scope.AirlineList[i].AilineCode;
                    //$scope.AirlineCode2LegTwo = $scope.AirlineList[i].CarrierCode2;
                    Ary.AirlineCode2 = $scope.AirlineList[i].CarrierCode2;
                    break;
                }
            }
        }
        if (index === 1 && Leg === 'Leg2') {
            for (i = 0; i < $scope.AirlineList.length; i++) {
                if (Ary.AirlineId === $scope.AirlineList[i].AirlineId) {

                    //$scope.AirlineCode2LegTwo = $scope.AirlineList[i].CarrierCode2;
                    Ary.AirlineCode2 = $scope.AirlineList[i].CarrierCode2;
                    break;
                }
            }
        }
        if (index === 2 && Leg === 'Leg2') {
            for (i = 0; i < $scope.AirlineList.length; i++) {
                if (Ary.AirlineId === $scope.AirlineList[i].AirlineId) {

                    //$scope.AirlineCode2LegTwo = $scope.AirlineList[i].CarrierCode2;
                    Ary.AirlineCode2 = $scope.AirlineList[i].CarrierCode2;
                    break;
                }
            }
        }
    };

    $scope.save = function (isValid) {
        if (isValid) {
            $scope.SaveMawbfun();
            $scope.FinalList = [];
            if ($scope.ShipmentHandlerMethodId !== 5) {
                for (i = 0; i < $scope.MawbMainObj.flightArray.length; i++) {
                    $scope.SaveMawbModel.TradelaneId = $scope.tradelaneBookingIntegration.Shipment.TradelaneShipmentId;
                    $scope.SaveMawbModel.MAWB = $scope.MawbMainObj.MAWB;
                    $scope.SaveMawbModel.MawbAllocationId = $scope.MawbMainObj.flightArray[i].MawbAllocationId;
                    $scope.SaveMawbModel.TimezoneId = $scope.MawbMainObj.flightArray[i].Timezone !== null ? $scope.MawbMainObj.flightArray[i].Timezone.TimezoneId : 0;
                    $scope.SaveMawbModel.AgentId = $scope.MawbMainObj.Agent.CustomerId;
                    $scope.SaveMawbModel.FlightNumber = $scope.MawbMainObj.flightArray[i].FlightNumber;
                    $scope.SaveMawbModel.AirlineId = $scope.MawbMainObj.flightArray[i].AirlineId;
                    $scope.SaveMawbModel.ETA = $scope.MawbMainObj.flightArray[i].ETA;
                    $scope.SaveMawbModel.ETD = $scope.MawbMainObj.flightArray[i].ETD;
                    $scope.SaveMawbModel.ETATime = $scope.MawbMainObj.flightArray[i].ETATime;
                    $scope.SaveMawbModel.ETDTime = $scope.MawbMainObj.flightArray[i].ETDTime;
                    $scope.SaveMawbModel.CreatedBy = $scope.CreatedBy;
                    $scope.FinalList.push($scope.SaveMawbModel);
                    $scope.SaveMawbfun();
                }
            }
            else {
                if ($scope.WithLegJson) {
                    if ($scope.WithLegJson.FirstLeg != null) {
                        for (i = 0; i < $scope.WithLegJson.FirstLeg.MawbMainObj.flightArray.length; i++) {
                            $scope.SaveMawbModel.TradelaneId = $scope.tradelaneBookingIntegration.Shipment.TradelaneShipmentId;
                            $scope.SaveMawbModel.MAWB = $scope.WithLegJson.FirstLeg.MawbMainObj.MAWB;
                            $scope.SaveMawbModel.LegNum = 'Leg1';
                            $scope.SaveMawbModel.MawbAllocationId = $scope.WithLegJson.FirstLeg.MawbMainObj.flightArray[i].MawbAllocationId;
                            $scope.SaveMawbModel.TimezoneId = $scope.WithLegJson.FirstLeg.MawbMainObj.flightArray[i].Timezone !== null ? $scope.WithLegJson.FirstLeg.MawbMainObj.flightArray[i].Timezone.TimezoneId : 0;
                            $scope.SaveMawbModel.AgentId = $scope.WithLegJson.FirstLeg.MawbMainObj.Agent.CustomerId;
                            $scope.SaveMawbModel.FlightNumber = $scope.WithLegJson.FirstLeg.MawbMainObj.flightArray[i].FlightNumber;
                            $scope.SaveMawbModel.AirlineId = $scope.WithLegJson.FirstLeg.MawbMainObj.flightArray[i].AirlineId;
                            $scope.SaveMawbModel.ETA = $scope.WithLegJson.FirstLeg.MawbMainObj.flightArray[i].ETA;
                            $scope.SaveMawbModel.ETD = $scope.WithLegJson.FirstLeg.MawbMainObj.flightArray[i].ETD;
                            $scope.SaveMawbModel.ETATime = $scope.WithLegJson.FirstLeg.MawbMainObj.flightArray[i].ETATime;
                            $scope.SaveMawbModel.ETDTime = $scope.WithLegJson.FirstLeg.MawbMainObj.flightArray[i].ETDTime;
                            $scope.SaveMawbModel.CreatedBy = $scope.CreatedBy;
                            $scope.FinalList.push($scope.SaveMawbModel);
                            $scope.SaveMawbfun();
                        }
                    }
                    if ($scope.WithLegJson.SecondLeg != null) {
                        for (j = 0; j < $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray.length; j++) {
                            $scope.SaveMawbModel.TradelaneId = $scope.tradelaneBookingIntegration.Shipment.TradelaneShipmentId;
                            $scope.SaveMawbModel.MAWB = $scope.WithLegJson.SecondLeg.MawbMainObj.MAWB;
                            $scope.SaveMawbModel.LegNum = 'Leg2';
                            $scope.SaveMawbModel.MawbAllocationId = $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[j].MawbAllocationId;
                            $scope.SaveMawbModel.TimezoneId = $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[j].Timezone !== null ? $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[j].Timezone.TimezoneId : 0;
                            $scope.SaveMawbModel.AgentId = $scope.WithLegJson.SecondLeg.MawbMainObj.Agent.CustomerId;
                            $scope.SaveMawbModel.FlightNumber = $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[j].FlightNumber;
                            $scope.SaveMawbModel.AirlineId = $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[j].AirlineId;
                            $scope.SaveMawbModel.ETA = $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[j].ETA;
                            $scope.SaveMawbModel.ETD = $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[j].ETD;
                            $scope.SaveMawbModel.ETATime = $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[j].ETATime;
                            $scope.SaveMawbModel.ETDTime = $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[j].ETDTime;
                            $scope.SaveMawbModel.CreatedBy = $scope.CreatedBy;
                            $scope.FinalList.push($scope.SaveMawbModel);
                            $scope.SaveMawbfun();
                        }
                    }
                }
            }
            if ($scope.FinalList.length > 0) {
                AppSpinner.showSpinnerTemplate($scope.Loading_Allocating_Agent_Shipment, $scope.Template);

                $scope.tradelaneBookingIntegration.MAWBList = $scope.FinalList;
                $scope.tradelaneBookingIntegration.Shipment.CreatedBy = $scope.CreatedBy;
                $scope.tradelaneBookingIntegration.Shipment.CustomerId = $scope.customerId;
                $scope.tradelaneBookingIntegration.Shipment.Hub.HubId = $scope.hub.HubId;
                $scope.tradelaneBookingIntegration.Shipment.Hub.Code = $scope.hub.Code;
                $scope.tradelaneBookingIntegration.Shipment.Hub.Name = $scope.hub.Name;
                $scope.tradelaneBookingIntegration.Shipment.MAWB = $scope.FinalList[0].MAWB;

                ExpressIntegrationShipmentService.SaveTradelaneIntegrationShipment($scope.tradelaneBookingIntegration).then(function (response) {
                    if (response.data) {
                        toaster.pop({
                            type: 'success',
                            title: $scope.FrayteSuccess,
                            body: $scope.Manifest_Created_Successfully,
                            showCloseButton: true
                        });
                        AppSpinner.hideSpinnerTemplate();
                        $uibModalInstance.close({ reload: true });
                    }
                }, function () {
                    AppSpinner.hideSpinnerTemplate();
                });
            }
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.FillMandataryFieldsStar,
                showCloseButton: true
            });
        }
    };

    $scope.AdditemWithoutLeg = function (flightDetail) {
        var flightObj = {
            FlightNumber: "",
            AirlineId: null,
            Timezone: null,
            ETA: null,
            ETD: null,
            ETAopened: false,
            ETDopened: false,
            IsRemove: false,
            IsAdd: false,
            dateOptions: {
                formatYear: 'yy',
                startingDay: 1,
                minDate: new Date()
            },
            dateOptions1: {
                formatYear: 'yy',
                startingDay: 1,
                minDate: new Date()

            },
            dateOptions2: {
                formatYear: 'yy',
                startingDay: 1,
                minDate: new Date()

            }
        };
        if ($scope.MawbMainObj.flightArray.length > 0 && $scope.MawbMainObj.flightArray.length < 3) {
            $scope.MawbMainObj.flightArray.push(flightObj);
            $scope.MawbMainObj.flightArray[2].IsRemove = true;
            $scope.MawbMainObj.flightArray[1].IsAdd = false;
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.MaxAddThreeFlight,
                showCloseButton: true
            });
        }

    };

    $scope.RemoveitemWithoutLeg = function (flightDetail) {
        if ($scope.MawbMainObj.flightArray.length > 2 && (flightDetail.MawbAllocationId === 0 || flightDetail.MawbAllocationId === undefined || flightDetail.MawbAllocationId === null || flightDetail.MawbAllocationId === "")) {
            $scope.MawbMainObj.flightArray.splice($scope.MawbMainObj.flightArray.length - 1, 1);
            $scope.MawbMainObj.flightArray[1].IsAdd = true;
        }
        else if ($scope.MawbMainObj.flightArray.length > 2 && flightDetail.MawbAllocationId > 0) {
            var modalOptions = {
                headerText: $scope.ConfirmationDeleteFlight,
                bodyText: $scope.SureDeleteGivenFlightDetail
            };

            ModalService.Confirm({}, modalOptions).then(function (result) {
                TradelaneShipmentService.DeleteMawbAllocation(flightDetail.MawbAllocationId).then(function (response) {

                    if (response.data.Status) {

                        toaster.pop({
                            type: 'success',
                            title: $scope.FrayteSuccess,
                            body: $scope.DeletedSuccessfully,
                            showCloseButton: true
                        });
                        for (i = 0; i < $scope.MawbMainObj.flightArray.length; i++) {
                            if ($scope.MawbMainObj.flightArray[i].MawbAllocationId === flightDetail.MawbAllocationId) {
                                $scope.MawbMainObj.flightArray.splice(i, 1);
                                //$scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[1].IsAdd = true;
                                $scope.MawbMainObj.flightArray[1].IsAdd = true;
                            }
                        }
                    }
                    else {
                        toaster.pop({
                            type: 'error',
                            title: $scope.FrayteError,
                            body: $scope.DeletingProblem,
                            showCloseButton: true
                        });
                    }
                });
            });

        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.CannotRemoveDetailAtleastTwoFlight,
                showCloseButton: true
            });
        }
    };

    $scope.Add = function (flightDetail) {
        var flightObj = {
            FlightNumber: "",
            AirlineId: null,
            Timezone: null,
            ETA: null,
            ETD: null,
            ETAopened: false,
            ETDopened: false,
            IsRemove: false,
            IsAdd: false,
            dateOptions: {
                formatYear: 'yy',
                startingDay: 1,
                minDate: new Date()

            },
            dateOptions1: {
                formatYear: 'yy',
                startingDay: 1,
                minDate: new Date()

            },
            dateOptions2: {
                formatYear: 'yy',
                startingDay: 1,
                minDate: new Date()

            }
        };
        if ($scope.WithLegJson.SecondLeg.MawbMainObj.flightArray.length > 0 && $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray.length < 3) {
            $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray.push(flightObj);
            $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[2].IsRemove = true;
            $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[1].IsAdd = false;
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.MaxAddThreeFlight,
                showCloseButton: true
            });
        }
    };

    $scope.Remove = function (flightDetail) {
        if ($scope.WithLegJson.SecondLeg.MawbMainObj.flightArray.length > 2 && (flightDetail.MawbAllocationId === 0 || flightDetail.MawbAllocationId === undefined || flightDetail.MawbAllocationId === null || flightDetail.MawbAllocationId === "")) {
            $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray.splice($scope.WithLegJson.SecondLeg.MawbMainObj.flightArray.length - 1, 1);
            $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[1].IsAdd = true;
        }
        else if ($scope.WithLegJson.SecondLeg.MawbMainObj.flightArray.length > 2 && flightDetail.MawbAllocationId > 0) {
            var modalOptions = {
                headerText: $scope.ConfirmationDeleteFlight,
                bodyText: $scope.SureDeleteGivenFlightDetail
            };

            ModalService.Confirm({}, modalOptions).then(function (result) {
                TradelaneShipmentService.DeleteMawbAllocation(flightDetail.MawbAllocationId).then(function (response) {

                    if (response.data.Status) {

                        toaster.pop({
                            type: 'success',
                            title: $scope.FrayteSuccess,
                            body: $scope.DeletedSuccessfully,
                            showCloseButton: true
                        });
                        for (i = 0; i < $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray.length; i++) {
                            if ($scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[i].MawbAllocationId === flightDetail.MawbAllocationId) {
                                $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray.splice(i, 1);
                                //$scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[1].IsAdd = true;

                            }
                        }
                        $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[1].IsAdd = true;
                    }
                    else {
                        toaster.pop({
                            type: 'error',
                            title: $scope.FrayteError,
                            body: $scope.DeletingProblem,
                            showCloseButton: true
                        });
                    }
                });
            });

        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.CannotRemoveDetailAtleastTwoFlight,
                showCloseButton: true
            });
        }
    };

    $scope.GetFlightNo = function (index, AllocationArray) {
        if (AllocationArray.length > 1) {
            if (index === 0) {
                return "First";
            }
            if (index === 1) {
                return "Second";
            }
            if (index === 2) {
                return "Third";
            }
            else if (AllocationArray.length === 1) {
                return "";
            }
        }

    };

    $scope.SaveMawbfun = function () {
        $scope.SaveMawbModel = {
            MawbAllocationId: 0,
            TradelaneId: 0,
            MAWB: '',
            AgentId: 0,
            FlightNumber: '',
            AirlineId: 0,
            ETA: null,
            ETD: null,
            CreatedBy: 0
        };
    };

    var setEditjsonWithoutLeg = function () {
        var flightObj = {
            FlightNumber: "",
            AirlineId: null,
            Timezone: null,
            ETA: null,
            ETD: null,
            ETAopened: false,
            ETDopened: false,
            IsAdd: false,
            dateOptions: {
                formatYear: 'yy',
                startingDay: 1,
                minDate: new Date()
            },
            dateOptions1: {
                formatYear: 'yy',
                startingDay: 1,
                minDate: new Date()
            }
        };

        $scope.MawbMainObj = {
            MAWB: "",
            TradelaneShipmentId: $scope.TradelaneShipmentId,
            Agent: null,
            flightArray: []

        };
        $scope.MawbMainObj.MAWB = $scope.MawbAllocation[0].MAWB;

        for (j = 0; j < $scope.Agents.length; j++) {
            if ($scope.MawbAllocation[0].AgentId === $scope.Agents[j].CustomerId) {
                $scope.MawbMainObj.Agent = $scope.Agents[j];
            }
        }

        for (i = 0; i < $scope.MawbAllocation.length; i++) {
            flightObj.FlightNumber = $scope.MawbAllocation[i].FlightNumber;
            flightObj.MawbAllocationId = $scope.MawbAllocation[i].MawbAllocationId;
            if ($scope.AirlineList != null && $scope.AirlineList.length > 0) {
                for (j = 0; j < $scope.AirlineList.length; j++) {
                    if ($scope.MawbAllocation[i].AirlineId === $scope.AirlineList[j].AirlineId) {
                        flightObj.AirlineId = $scope.AirlineList[j].AirlineId;
                        flightObj.AirlineCode2 = $scope.AirlineList[j].CarrierCode2;

                    }
                }

                for (j = 0; j < $scope.AirlineList.length; j++) {
                    if ($scope.MawbAllocation[i].AirlineId === $scope.AirlineList[j].AirlineId && i === 0) {
                        $scope.AirlineCode = $scope.AirlineList[j].AilineCode;
                        flightObj.AirlineCode2 = $scope.AirlineList[j].CarrierCode2;
                        break;
                    }
                }
            }
            if ($scope.timezones != null && $scope.timezones.length > 0) {
                for (jj = 0; jj < $scope.timezones.length; jj++) {
                    if ($scope.MawbAllocation[i].TimezoneId === $scope.timezones[jj].TimezoneId) {
                        flightObj.Timezone = $scope.timezones[jj];
                    }
                }
            }
            flightObj.ETA = $scope.MawbAllocation[i].ETA !== null ? new Date($scope.MawbAllocation[i].ETA) : null;
            flightObj.ETD = $scope.MawbAllocation[i].ETD !== null ? new Date($scope.MawbAllocation[i].ETD) : null;
            flightObj.ETATime = $scope.MawbAllocation[i].ETATime;
            flightObj.TimezoneId = $scope.MawbAllocation[i].TimezoneId;
            flightObj.ETDTime = $scope.MawbAllocation[i].ETDTime;
            $scope.MawbMainObj.flightArray.push(flightObj);
            flightObj = {
                FlightNumber: "",
                Timezone: null,
                AirlineId: null,
                ETA: null,
                ETD: null,
                ETAopened: false,
                ETDopened: false,
                IsAdd: false
            };
        }
        if ($scope.MawbMainObj.flightArray != null && $scope.MawbMainObj.flightArray.length == 2) {
            $scope.MawbMainObj.flightArray[1].IsAdd = true;
        }
        if ($scope.MawbMainObj.flightArray != null && $scope.MawbMainObj.flightArray.length == 3) {
            $scope.MawbMainObj.flightArray[2].IsRemove = true;
        }
    };

    var setjsonWithoutLeg = function () {
        var flightObj = {
            FlightNumber: "",
            AirlineId: null,
            Timezone: null,
            TimezoneId: null,
            ETA: null,
            ETATime: null,
            ETD: null,
            ETDTime: null,
            ETAopened: false,
            ETDopened: false,
            IsAdd: false,
            dateOptions: {
                formatYear: 'yy',
                startingDay: 1,
                minDate: new Date()

            },
            dateOptions1: {
                formatYear: 'yy',
                startingDay: 1,
                minDate: new Date()

            }
        };
        $scope.MawbMainObj = {
            MAWB: "",
            TradelaneShipmentId: $scope.TradelaneShipmentId,
            Agent: null,
            flightArray: []

        };
        if ($scope.tradelaneBookingIntegration.Shipment.ShipmentHandlerMethod.ShipmentHandlerMethodId === 1) {
            $scope.MawbMainObj.flightArray.push(flightObj);
        }
        else if ($scope.tradelaneBookingIntegration.Shipment.ShipmentHandlerMethod.ShipmentHandlerMethodId === 2 || $scope.tradelaneBookingIntegration.Shipment.ShipmentHandlerMethod.ShipmentHandlerMethodId === 4) {
            for (i = 0; i < 2; i++) {
                $scope.MawbMainObj.flightArray.push(flightObj);
                flightObj = {
                    FlightNumber: "",
                    AirlineId: null,
                    Timezone: null,
                    TimezoneId: null,
                    ETA: null,
                    ETATime: null,
                    ETD: null,
                    ETDTime: null,
                    IsAdd: false
                };
            }
            $scope.MawbMainObj.flightArray[1].IsAdd = true;
        }
    };

    var setEditjsonWithLeg = function () {
        var flightObj = {
            FlightNumber: "",
            AirlineId: null,
            Timezone: null,
            ETA: null,
            ETD: null,
            ETAopened: false,
            ETDopened: false,
            IsRemove: false,
            IsAdd: false,
            dateOptions: {
                formatYear: 'yy',
                startingDay: 1,
                minDate: new Date()
            },
            dateOptions1: {
                formatYear: 'yy',
                startingDay: 1,
                minDate: new Date()

            },
            dateOptions2: {
                formatYear: 'yy',
                startingDay: 1,
                minDate: new Date()

            }
        };
        $scope.MawbMainObj = {
            MAWB: "",
            TradelaneShipmentId: $scope.TradelaneShipmentId,
            Agent: null,
            flightArray: []

        };
        $scope.WithLegJson = {
            FirstLeg: {
                Leg1: "Leg 1",
                MawbMainObj: {
                    MAWB: null,
                    TradelaneShipmentId: $scope.TradelaneShipmentId,
                    Agent: null,
                    flightArray: []

                }
            },
            SecondLeg: {
                Leg2: "Leg 2",
                MawbMainObj: {
                    MAWB: null,
                    TradelaneShipmentId: $scope.TradelaneShipmentId,
                    Agent: null,
                    flightArray: []

                }
            }
        };

        $scope.WithLegJson.FirstLeg.MawbMainObj.MAWB = $scope.MawbAllocation[0].MAWB;
        for (j = 0; j < $scope.Agents.length; j++) {
            if ($scope.MawbAllocation[0].AgentId === $scope.Agents[j].CustomerId) {
                $scope.WithLegJson.FirstLeg.MawbMainObj.Agent = $scope.Agents[j];
            }
        }
        $scope.WithLegJson.SecondLeg.MawbMainObj.MAWB = $scope.MawbAllocation[1].MAWB;
        for (j = 0; j < $scope.Agents.length; j++) {
            if ($scope.MawbAllocation[1].AgentId === $scope.Agents[j].CustomerId) {
                $scope.WithLegJson.SecondLeg.MawbMainObj.Agent = $scope.Agents[j];
            }
        }

        for (i = 0; i < $scope.MawbAllocation.length; i++) {
            if ($scope.MawbAllocation[i].LegNum === 'Leg1') {
                flightObj.FlightNumber = $scope.MawbAllocation[i].FlightNumber;
                flightObj.MawbAllocationId = $scope.MawbAllocation[i].MawbAllocationId;
                //flightObj.TimezoneId = $scope.MawbAllocation[i].TimezoneId;
                for (j = 0; j < $scope.AirlineList.length; j++) {
                    if ($scope.MawbAllocation[i].AirlineId === $scope.AirlineList[j].AirlineId) {
                        flightObj.AirlineId = $scope.AirlineList[j].AirlineId;
                        $scope.AirlineCodeLegOne = $scope.AirlineList[j].AilineCode;
                        // $scope.AirlineCode2LegOne = $scope.AirlineList[j].CarrierCode2;
                        flightObj.AirlineCode2 = $scope.AirlineList[j].CarrierCode2;
                    }
                }
                for (jj = 0; jj < $scope.timezones.length; jj++) {
                    if ($scope.MawbAllocation[i].TimezoneId === $scope.timezones[jj].TimezoneId) {
                        flightObj.Timezone = $scope.timezones[jj];
                    }
                }
                flightObj.ETA = $scope.MawbAllocation[i].ETA !== null ? new Date($scope.MawbAllocation[i].ETA) : null;
                flightObj.ETD = $scope.MawbAllocation[i].ETD !== null ? new Date($scope.MawbAllocation[i].ETD) : null;
                flightObj.ETATime = $scope.MawbAllocation[i].ETATime;
                flightObj.ETDTime = $scope.MawbAllocation[i].ETDTime;
                $scope.WithLegJson.FirstLeg.MawbMainObj.flightArray.push(flightObj);
                flightObj = {
                    FlightNumber: "",
                    AirlineId: null,
                    Timezone: null,
                    ETA: null,
                    ETD: null,
                    ETAopened: false,
                    ETDopened: false,
                    IsRemove: false,
                    IsAdd: false
                };
            }
            if ($scope.MawbAllocation[i].LegNum === 'Leg2') {
                flightObj.FlightNumber = $scope.MawbAllocation[i].FlightNumber;
                flightObj.MawbAllocationId = $scope.MawbAllocation[i].MawbAllocationId;
                //flightObj.TimezoneId = $scope.MawbAllocation[i].TimezoneId;
                for (j = 0; j < $scope.AirlineList.length; j++) {
                    if ($scope.MawbAllocation[i].AirlineId === $scope.AirlineList[j].AirlineId) {
                        flightObj.AirlineId = $scope.AirlineList[j].AirlineId;
                        //$scope.AirlineCode2LegTwo = $scope.AirlineList[j].CarrierCode2;
                        flightObj.AirlineCode2 = $scope.AirlineList[j].CarrierCode2;
                    }
                }
                for (j = 0; j < $scope.AirlineList.length; j++) {
                    if ($scope.MawbAllocation[i].AirlineId === $scope.AirlineList[j].AirlineId && i === 1) {
                        $scope.AirlineCodeLegTwo = $scope.AirlineList[j].AilineCode;
                        //$scope.AirlineCode2LegTwo = $scope.AirlineList[i].CarrierCode2;
                        $scope.AirlineCode2 = $scope.AirlineList[i].CarrierCode2;
                        break;
                    }
                }
                for (jj = 0; jj < $scope.timezones.length; jj++) {
                    if ($scope.MawbAllocation[i].TimezoneId === $scope.timezones[jj].TimezoneId) {
                        flightObj.Timezone = $scope.timezones[jj];
                    }
                }
                flightObj.ETA = $scope.MawbAllocation[i].ETA !== null ? new Date($scope.MawbAllocation[i].ETA) : null;
                flightObj.ETD = $scope.MawbAllocation[i].ETD !== null ? new Date($scope.MawbAllocation[i].ETD) : null;
                flightObj.ETATime = $scope.MawbAllocation[i].ETATime;
                flightObj.ETDTime = $scope.MawbAllocation[i].ETDTime;
                $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray.push(flightObj);
                flightObj = {
                    FlightNumber: "",
                    AirlineId: null,
                    Timezone: null,
                    ETA: null,
                    ETD: null,
                    ETAopened: false,
                    ETDopened: false,
                    IsRemove: false,
                    IsAdd: false
                };
            }
        }

        if ($scope.WithLegJson.SecondLeg.MawbMainObj.flightArray.length == 2) {
            $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[1].IsAdd = true;
        }
        if ($scope.WithLegJson.SecondLeg.MawbMainObj.flightArray.length == 3) {
            $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[2].IsRemove = true;
        }

    };

    var setjsonWithLeg = function () {
        var flightObj = {
            FlightNumber: "",
            AirlineId: null,
            Timezone: null,
            TimezoneId: null,
            ETA: null,
            ETATime: null,
            ETD: null,
            ETDTime: null,
            ETAopened: false,
            ETDopened: false,
            IsRemove: false,
            IsAdd: false,
            dateOptions: {
                formatYear: 'yy',
                startingDay: 1,
                minDate: new Date()
            },
            dateOptions1: {
                formatYear: 'yy',
                startingDay: 1,
                minDate: new Date()
            },
            dateOptions2: {
                formatYear: 'yy',
                startingDay: 1,
                minDate: new Date()
            }
        };
        $scope.MawbMainObj = {
            MAWB: "",
            TradelaneShipmentId: $scope.TradelaneShipmentId,
            Agent: null,
            flightArray: []
        };
        $scope.WithLegJson = {
            FirstLeg: {
                Leg1: "Leg 1",
                MawbMainObj: {
                    MAWB: null,
                    TradelaneShipmentId: $scope.TradelaneShipmentId,
                    Agent: null,
                    flightArray: []
                }
            },
            SecondLeg: {
                Leg2: "Leg 2",
                MawbMainObj: {
                    MAWB: null,
                    TradelaneShipmentId: $scope.TradelaneShipmentId,
                    Agent: null,
                    flightArray: []
                }
            }
        };
        for (i = 0; i < 1; i++) {
            $scope.WithLegJson.FirstLeg.MawbMainObj.flightArray.push(flightObj);
            flightObj = {
                FlightNumber: null,
                AirlineId: null,
                Timezone: null,
                TimezoneId: null,
                ETA: null,
                ETATime: null,
                ETD: null,
                ETDTime: null,
                IsRemove: false,
                IsAdd: false
            };
        }
        for (i = 0; i < 2; i++) {
            $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray.push(flightObj);
            flightObj = {
                FlightNumber: null,
                AirlineId: null,
                Timezone: null,
                TimezoneId: null,
                ETA: null,
                ETATime: null,
                ETD: null,
                ETDTime: null,
                IsRemove: false,
                IsAdd: false
            };
        }
        $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[1].IsAdd = true;
    };

    var setMawbAddJson = function () {
        if ($scope.tradelaneBookingIntegration.Shipment.ShipmentHandlerMethod != null && $scope.tradelaneBookingIntegration.Shipment.ShipmentHandlerMethod.ShipmentHandlerMethodId > 0) {
            if ($scope.tradelaneBookingIntegration.Shipment.ShipmentHandlerMethod.ShipmentHandlerMethodId !== 5) {
                setjsonWithoutLeg();
            }
            else {
                setjsonWithLeg();
            }
        }
    };

    var setMawbEditJson = function () {
        TradelaneShipmentService.GetShipmentHandlerId($scope.TradelaneShipmentId).then(function (response) {
            $scope.ShipmentHandlerMethodId = response.data.ShipmentHandlerMethodId;
            $scope.FromAirport = response.data.RouteFrom;
            $scope.ToAirport = response.data.RouteTo;
            $scope.ShipmentHandlerCode = response.data.ShipemntHandlerMethodCode;
            if ($scope.ShipmentHandlerMethodId != null && $scope.ShipmentHandlerMethodId > 0) {
                if ($scope.ShipmentHandlerMethodId !== 5) {
                    setEditjsonWithoutLeg();
                }
                else {
                    setEditjsonWithLeg();
                }
            }
        });
    };

    $scope.ShipmentHandlerMethodChange = function () {
        setMawbAddJson();
    };

    $scope.GetInitial = function () {
        TradelaneShipmentService.GetAirlines().then(function (response) {
            $scope.AirlineList = TopAirlineService.TopAirlineList(response.data);
        });
        TradelaneShipmentService.GetTimeZoneList().then(function (response) {
            $scope.timezones = response.data;
        });
        $scope.GetAgent();
    };

    var getSystemRoles = function () {
        UserService.GetSystemRoles($scope.userId).then(function (response) {
            if (response.data) {
                $scope.SystemRoles = response.data;
            }
        });
    };

    $scope.OpenCa = function ($event, index, arr) {
        for (i = 0; i < $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray.length; i++) {
            if (index === i) {
                $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[i].ETAopened = true;
            }
        }
    };

    $scope.OpenCa1 = function ($event, index, arr) {
        for (i = 0; i < $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray.length; i++) {
            if (index === i) {
                $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[i].ETDopened = true;
            }
        }
    };

    $scope.OpenCal = function ($event, index, arr) {
        for (i = 0; i < $scope.WithLegJson.FirstLeg.MawbMainObj.flightArray.length; i++) {
            if (index === i) {
                $scope.WithLegJson.FirstLeg.MawbMainObj.flightArray[i].ETAopened = true;
            }
        }
    };

    $scope.OpenCal1 = function ($event, index, arr) {
        for (i = 0; i < $scope.WithLegJson.FirstLeg.MawbMainObj.flightArray.length; i++) {
            if (index === i) {
                $scope.WithLegJson.FirstLeg.MawbMainObj.flightArray[i].ETDopened = true;
            }
        }
    };

    $scope.OpenCalender = function ($event, index, arr) {
        for (i = 0; i < $scope.MawbMainObj.flightArray.length; i++) {
            if (index === i) {
                $scope.MawbMainObj.flightArray[i].ETAopened = true;
            }
        }
    };

    $scope.OpenCalender1 = function ($event, index, arr) {
        for (i = 0; i < $scope.MawbMainObj.flightArray.length; i++) {
            if (index === i) {
                $scope.MawbMainObj.flightArray[i].ETDopened = true;
            }
        }
    };

    var screenInitials = function () {
        TradelaneShipmentService.GetAirlines().then(function (response) {
            $scope.AirlineList = TopAirlineService.TopAirlineList(response.data);
            TradelaneShipmentService.GetTimeZoneList().then(function (response) {
                $scope.timezones = response.data;
                $scope.GetAgent();
                originCountryTimeZone($scope.bags[0].BagId);
            }, function () {
                AppSpinner.hideSpinnerTemplate();
            });

        }, function () {
            AppSpinner.hideSpinnerTemplate();
        });
    };

    $scope.GetAgent = function () {
        TradelaneShipmentService.GetTradelaneAgents().then(function (response) {
            $scope.Agents = response.data;
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
        });
    };

    var newBooking = function () {
        $scope.tradelaneBookingIntegration = {
            TradelaneShipmentId: 0,
            OpearionZoneId: 0,
            CustomerId: $scope.customerId,
            BatteryDeclarationType: 'None',
            CustomerAccountNumber: null,
            ShipmentStatusId: $scope.ShipmentStatus.Draft,
            CreatedBy: $scope.CreatedBy,
            CreatedOnUtc: new Date(),
            ShipFrom: {
                TradelaneShipmentAddressId: 0,
                Country: null,
                PostCode: "",
                FirstName: "",
                LastName: "",
                CompanyName: "",
                Address1: "",
                Address2: "",
                City: "",
                State: "",
                Area: "",
                Phone: "",
                Email: "",
                IsDefault: false,
                IsMailSend: false
            },
            ShipperAdditionalNote: '',
            ShipTo: {
                TradelaneShipmentAddressId: 0,
                Country: null,
                PostCode: "",
                FirstName: "",
                LastName: "",
                CompanyName: "",
                Address1: "",
                Address2: "",
                City: "",
                State: "",
                Area: "",
                Phone: "",
                Email: "",
                IsDefault: false,
                IsMailSend: false
            },
            ReceiverAdditionalNote: '',
            NotifyParty: {
                TradelaneShipmentAddressId: 0,
                Country: null,
                PostCode: "",
                FirstName: "",
                LastName: "",
                CompanyName: "",
                Address1: "",
                Address2: "",
                City: "",
                State: "",
                Area: "",
                Phone: "",
                Email: "",
                IsMailSend: false
            },
            NotifyPartyAdditionalNote: '',
            IsNotifyPartySameAsReceiver: true,
            PayTaxAndDuties: "Shipper",
            TaxAndDutyAccountNumber: '',
            CustomsSigner: '',
            TaxAndDutyAcceptedBy: '',
            Currency: null,
            Packages: [{
                TradelaneShipmentDetailId: 0,
                Length: null,
                Width: null,
                Height: null,
                Weight: null,
                Value: null,
                HAWB: null
            }],
            ShipmentHandlerMethod: null,
            UpdatedBy: 0,
            UpdatedOnUtc: new Date(),
            FrayteNumber: '',
            DepartureAirportCode: '',
            DestinationAirportCode: '',
            ShipmentReference: '',
            ShipmentDescription: '',
            DeclaredValue: null,
            DeclaredCurrency: null,
            AirlinePreference: null,
            TotalEstimatedWeight: null,
            MAWB: '',
            MAWBAgentId: '',
            ManifestName: '',
            FinalList: $scope.FinalList

        };

    };

    var setPackageJSon = function () {

        $scope.tradelaneBookingIntegration.Shipment.Packages = [];
        if ($scope.bags && $scope.bags.length) {
            for (var i = 0; i < $scope.bags.length; i++) {
                var obj = {
                    TradelaneShipmentDetailId: 0,
                    TradelaneShipmentId: 0,
                    Length: 50,
                    Width: 50,
                    Height: 50,
                    Value: 50,
                    BagId: $scope.bags[i].BagId,
                    CartonNumber: $scope.bags[i].BagNumber,
                    NoOfPcs: $scope.bags[i].TotalShipments,
                    Carrier: $scope.bags[i].Carrier,
                    Weight: $scope.bags[i].TotalWeight
                };
                $scope.tradelaneBookingIntegration.Shipment.Packages.push(obj);
            }
        }
    };

    var initials = function () {
        AppSpinner.showSpinnerTemplate($scope.Loading_Create_Manifest, $scope.Template);
        TradelaneBookingService.BookingInitials($scope.userInfo.EmployeeId).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            $scope.ShipmentMethods = response.data.ShipmentMethods;
            ExpressIntegrationShipmentService.TradelaneHubInitials($scope.customerId, $scope.hubId).then(function (response) {
                $scope.tradelaneBookingIntegration = response.data;
                $scope.tradelaneBookingIntegration.Shipment.ShipmentHandlerMethod = $scope.ShipmentMethods[0];
                $scope.ShipmentHandlerMethodChange();
                // MAWB initials
                screenInitials();
                setPackageJSon();
            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.InitialDataValidation,
                    showCloseButton: true
                });
            });
        },
        function (response) {
            AppSpinner.hideSpinnerTemplate();
            if (response.status !== 401) {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.InitialDataValidation,
                    showCloseButton: true
                });
            }
        });
    };

    var ShipmentInitials = function () {
    };

    // MAWB Document
    $scope.WhileAddingMAWB = function ($files, $file, $event) {
        if (!$file) {
            return;
        }
        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.PleaseSelectValidFile,
                showCloseButton: true
            });
            return;
        }

        AppSpinner.showSpinnerTemplate($scope.UploadingDocument, $scope.Template);


        $scope.MawbDoc = $file.name;

        // Upload the excel file here.
        $scope.uploadMAWB = Upload.upload({
            url: config.SERVICE_URL + '/ExpressManifest/UploadMAWBForms',
            file: $file,
            fields: {
                ShipmentId: $scope.tradelaneBookingIntegration.Shipment.TradelaneShipmentId,
                DocType: "MAWBDocument",
                UserId: $scope.userId,
                CustomerId: $scope.customerId,
                HubId: $scope.hubId
            }
        });

        $scope.uploadMAWB.progress($scope.progressMAWB);
        $scope.uploadMAWB.success($scope.successMAWB);
        $scope.uploadMAWB.error($scope.errorMAWB);
    };

    $scope.progressMAWB = function (evt) {
        //To Do:  show excel uploading progress message 
    };

    $scope.successMAWB = function (data, status, headers, config) {
        if (status = 200) {
            AppSpinner.hideSpinnerTemplate();
            if (data) {
                if (!isNaN(data) && (parseInt(data, 10) > 0)) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.DocumentUploadedSuccessfully,
                        showCloseButton: true
                    });

                    $scope.tradelaneBookingIntegration.Shipment.TradelaneShipmentId = data;
                }
                else if (data === "MAWB") {
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteError,
                        body: $scope.ErrorUploadingDocument,
                        showCloseButton: true
                    });
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteError,
                        body: $scope.ErrorUploadingDocument,
                        showCloseButton: true
                    });
                }
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarning,
                    body: data.Message,
                    showCloseButton: true
                });
            }
        }
        else {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ErrorUploadingDocument,
                showCloseButton: true
            });
        }
    };

    $scope.errorMAWB = function (err) {
        AppSpinner.hideSpinnerTemplate();
        toaster.pop({
            type: 'error',
            title: $scope.FrayteError,
            body: $scope.Errorwhil_uploading_the_excel,
            showCloseButton: true
        });
    };
    // end // MAWB Document

    // Upload Other Document Section
    $scope.WhileAddingOtherDoc = function ($files, $file, $event) {
        if (!$file) {
            return;
        }
        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.PleaseSelectValidFile,
                showCloseButton: true
            });
            return;
        }

        AppSpinner.showSpinnerTemplate($scope.UploadingDocument, $scope.Template);
        // Upload the excel file here.
        $scope.uploadExcel = Upload.upload({
            url: config.SERVICE_URL + '/ExpressManifest/UploadOtherDocument',
            file: $file,
            fields: {
                ShipmentId: $scope.tradelaneBookingIntegration.Shipment.TradelaneShipmentId,
                DocType: "OtherDocument",
                UserId: $scope.userId,
                CustomerId: $scope.customerId,
                HubId: $scope.hubId
            }
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
            AppSpinner.hideSpinnerTemplate();
            if (data) {
                if (!isNaN(data) && (parseInt(data, 10) > 0)) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.DocumentUploadedSuccessfully,
                        showCloseButton: true
                    });

                    $scope.tradelaneBookingIntegration.Shipment.TradelaneShipmentId = data;
                    getshipmentOtherDocuments();

                }
                else if (data === "Failed") {
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteError,
                        body: $scope.ErrorUploadingDocument,
                        showCloseButton: true
                    });

                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.FrayteWarning,
                        body: $scope.DocumentAleadyUploadedFor + data,
                        showCloseButton: true
                    });

                }
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarning,
                    body: data.Message,
                    showCloseButton: true
                });

            }
        }
        else {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ErrorUploadingDocument,
                showCloseButton: true
            });
        }
    };

    $scope.errorExcel = function (err) {
        AppSpinner.hideSpinnerTemplate();
        if (err === 'OtherDocument') {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.Document_Already_Uploaded, // document already uploaded 
                showCloseButton: true
            });
        }
        else {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ErrorUploadingDocument,
                showCloseButton: true
            });
        }

    };

    var getshipmentOtherDocuments = function () {
        PreAlertService.getTradelaneDocuments($scope.tradelaneBookingIntegration.Shipment.TradelaneShipmentId).then(function (response) {
            if (response.data && response.data.length) {
                $scope.otherDocuments = response.data[0];
                otherDocumentJson();
            }
            AppSpinner.hideSpinnerTemplate();
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.GettingDetails_Error,
                showCloseButton: true
            });
        });
    };

    var otherDocumentJson = function (type, UploadType) {
        $scope.attachments = [];
        if ($scope.otherDocuments.Documents.length) {
            angular.forEach($scope.otherDocuments.Documents, function (obj) {
                $scope.attachments.push(obj);
            });
        }
    };

    var removeDocument = function (doc, document) {
        if ($scope.attachments && $scope.attachments.length) {
            for (var j = 0; j < $scope.attachments.length ; j++) {
                if ($scope.attachments[j].TradelaneShipmentDocumentId === doc.TradelaneShipmentDocumentId) {
                    $scope.attachments.splice(j, 1);
                    break;
                }
            }
        }
        if (!$scope.attachments.length) {
            $scope.open = false;
        }
    };

    $scope.removeDoc = function (doc, document) {
        console.log(doc);
        if (doc) {
            AppSpinner.showSpinnerTemplate($scope.Loading_Create_Manifest, $scope.Template);
            TradelaneBookingService.removeDocument(doc.TradelaneShipmentDocumentId).then(function (response) {
                AppSpinner.hideSpinnerTemplate();
                if (response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.Successfully_Deleted_Document,
                        showCloseButton: true
                    });
                    removeDocument(doc, document);
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteError,
                        body: $scope.Error_Deleting_Document,
                        showCloseButton: true
                    });
                }
            }, function () {

                AppSpinner.hideSpinnerTemplate();

                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.Error_Deleting_Document,
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.Error_Deleting_Document,
                showCloseButton: true
            });
        }
    };

    //Upload BatteryFrom 
    $scope.WhileAddingMsBatteryForm = function ($files, $file, $event) {
        if (!$file) {
            return;
        }
        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.PleaseSelectValidFile,
                showCloseButton: true
            });
            return;
        }

        $scope.msdsBatteryFileName = $file.name;

        // Upload the excel file here.
        $scope.uploadMSDS = Upload.upload({
            url: config.SERVICE_URL + '/ExpressManifest/UploadBatteryForms',
            file: $file,
            fields: {
                ShipmentId: $scope.tradelaneBookingIntegration.Shipment.TradelaneShipmentId,
                DocType: "BatteryDeclaration",
                UserId: $scope.userId,
                CustomerId: $scope.customerId,
                HubId: $scope.hubId
            }
        });

        $scope.uploadMSDS.progress($scope.progressuploadMSDS);

        $scope.uploadMSDS.success($scope.successuploadMSDS);

        $scope.uploadMSDS.error($scope.erroruploadMSDS);
    };

    $scope.progressuploadMSDS = function (evt) {
        //To Do:  show excel uploading progress message 
    };

    $scope.successuploadMSDS = function (data, status, headers, config) {
        if (status = 200) {
            if (!isNaN(data) && (parseInt(data, 10) > 0)) {
                $scope.MSDS = $scope.msdsBatteryFileName;
                toaster.pop({
                    type: 'success',
                    title: $scope.FrayteSuccess,
                    body: $scope.SuccessfullyUploadedForm,
                    showCloseButton: true
                });
                $scope.tradelaneBookingIntegration.Shipment.TradelaneShipmentId = data;

            }
            else {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteWarning,
                    body: $scope.ErrorUploadingDocumentTryAgain,
                    showCloseButton: true
                });
            }
        }
        else {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteWarning,
                body: $scope.ErrorUploadingDocumentTryAgain,
                showCloseButton: true
            });
        }
    };

    $scope.erroruploadMSDS = function (err) {
        if (err && err.Message === "BatteryForm") {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.DocumentAleadyUploadedFor + err.Message,
                showCloseButton: true
            });
        }
        else {

            toaster.pop({
                type: 'error',
                title: $scope.FrayteWarning,
                body: $scope.ErrorUploadingDocumentTryAgain,
                showCloseButton: true
            });
        }
    };

    //Upload BatteryFrom  UN38
    $scope.WhileAddingUN38BatteryForm = function ($files, $file, $event) {
        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.PleaseSelectValidFile,
                showCloseButton: true
            });
            return;
        }
        $scope.un38BatteryFileName = $file.name;
        // Upload the excel file here.
        $scope.uploadUN38 = Upload.upload({
            url: config.SERVICE_URL + '/ExpressManifest/UploadBatteryForms',
            file: $file,
            fields: {
                ShipmentId: $scope.tradelaneBookingIntegration.Shipment.TradelaneShipmentId,
                DocType: "BatteryDeclaration",
                UserId: $scope.userId,
                CustomerId: $scope.customerId,
                HubId: $scope.hubId
            }
        });

        $scope.uploadUN38.progress($scope.progressuploadUN38);

        $scope.uploadUN38.success($scope.successuploadUN38);

        $scope.uploadUN38.error($scope.erroruploadUN38);
    };

    $scope.progressuploadUN38 = function (evt) {
        //To Do:  show excel uploading progress message 
    };

    $scope.successuploadUN38 = function (data, status, headers, config) {
        if (status = 200) {
            if (!isNaN(data) && (parseInt(data, 10) > 0)) {
                $scope.UN38 = $scope.un38BatteryFileName;
                toaster.pop({
                    type: 'success',
                    title: $scope.FrayteSuccess,
                    body: $scope.SuccessfullyUploadedForm,
                    showCloseButton: true
                });
                $scope.tradelaneBookingIntegration.Shipment.TradelaneShipmentId = data;

            }
            else {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteWarning,
                    body: $scope.ErrorUploadingDocumentTryAgain,
                    showCloseButton: true
                });
            }
        }
        else {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteWarning,
                body: $scope.ErrorUploadingDocumentTryAgain,
                showCloseButton: true
            });

        }
    };

    $scope.erroruploadUN38 = function (err) {
        if (err && err.Message === "BatteryForm") {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.DocumentAleadyUploadedFor + err.Message,
                showCloseButton: true
            });
        }
        else {

            toaster.pop({
                type: 'error',
                title: $scope.FrayteWarning,
                body: $scope.ErrorUploadingDocumentTryAgain,
                showCloseButton: true
            });
        }
    };

    //Upload BatteryFrom  UN38
    $scope.WhileAddingBatteryDeclarationForm = function ($files, $file, $event) {
        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.PleaseSelectValidFile,
                showCloseButton: true
            });
            return;
        }
        $scope.batteryDeclarationFormFileNBame = $file.name;
        // Upload the excel file here.
        $scope.uploadBatteryDeclarationForm = Upload.upload({
            url: config.SERVICE_URL + '/ExpressManifest/UploadBatteryForms',
            file: $file,
            fields: {
                ShipmentId: $scope.tradelaneBookingIntegration.Shipment.TradelaneShipmentId,
                DocType: "BatteryDeclaration",
                UserId: $scope.userId,
                CustomerId: $scope.customerId,
                HubId: $scope.hubId
            }
        });

        $scope.uploadUN38.progress($scope.progressuploadBatteryDeclarationForm);

        $scope.uploadUN38.success($scope.successuploadBatteryDeclarationForm);

        $scope.uploadUN38.error($scope.erroruploadBatteryDeclarationForm);
    };

    $scope.progressuploadBatteryDeclarationForm = function (evt) {
        //To Do:  show excel uploading progress message 
    };

    $scope.successuploadBatteryDeclarationForm = function (data, status, headers, config) {
        if (status = 200) {
            if (!isNaN(data) && (parseInt(data, 10) > 0)) {
                $scope.BatteryDeclaration = $scope.batteryDeclarationFormFileNBame;
                toaster.pop({
                    type: 'success',
                    title: $scope.FrayteSuccess,
                    body: $scope.SuccessfullyUploadedForm,
                    showCloseButton: true
                });
                $scope.tradelaneBookingIntegration.Shipment.TradelaneShipmentId = data;
            }
            else {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteWarning,
                    body: $scope.ErrorUploadingDocumentTryAgain,
                    showCloseButton: true
                });
            }
        }
        else {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteWarning,
                body: $scope.ErrorUploadingDocumentTryAgain,
                showCloseButton: true
            });
        }
    };

    $scope.erroruploadBatteryDeclarationForm = function (err) {
        if (err && err.Message === "BatteryForm") {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.DocumentAleadyUploadedFor + err.Message,
                showCloseButton: true
            });
        }
        else {

            toaster.pop({
                type: 'error',
                title: $scope.FrayteWarning,
                body: $scope.ErrorUploadingDocumentTryAgain,
                showCloseButton: true
            });
        }
    };
    //End Battery Form

    function originCountryTimeZone(BagId) {
        if (BagId !== undefined && BagId !== null && BagId !== '' && BagId > 0) {
            ExpressIntegrationShipmentService.GetTimeZoneName(BagId).then(function (response) {
                if (response.data !== undefined && response.data !== null && response.data !== '') {
                    angular.forEach($scope.timezones, function (item, key) {
                        if (response.data === item.Name) {
                            $scope.MawbMainObj.flightArray[0].Timezone = item;
                        }
                    });
                }
            });
        }
    }

    function init() {
        $scope.submitted = true;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.userInfo = SessionService.getUser();
        $scope.UserRoleId = $scope.userInfo.RoleId;
        $scope.userId = $scope.userInfo.EmployeeId;
        $scope.ShipmentStatus = {
            Draft: 27,
            ShipmentBooked: 28,
            Pending: 29,
            Delivered: 35,
            Rejeted: 34,
            Departed: 30,
            Intransit: 31,
            Arrived: 32

        };
        $scope.CreatedBy = $scope.userInfo.EmployeeId;
        $scope.hub = Hub;
        $scope.hubId = $scope.hub.HubId;
        $scope.customerId = CustomerId;
        $scope.bags = Bags;

        $scope.batteryDeclarations = [
            {
                key: 'None',
                value: 'None'
            },
            {
                key: 'PI966',
                value: 'PI966'
            },
            {
                key: 'PI967',
                value: 'PI967'
            }
        ];

        setMultilingualOptions();

        getSystemRoles();
    }

    init();
});