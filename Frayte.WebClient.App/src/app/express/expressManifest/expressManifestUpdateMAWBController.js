
angular.module('ngApp.express').controller("ExpressUpdateManifestController", function ($scope, SessionService, AppSpinner, Upload, config, $translate, PreAlertService, ShipmentId, TradelaneBookingService, ExpressIntegrationShipmentService, toaster, TradelaneShipmentService, TopAirlineService, $uibModalInstance) {

    //multilingual translation key
    var setMultilingualOptions = function () {
        $translate(['FrayteError','FrayteWarning', 'FrayteSuccess', 'CannotRemoveDetailAtleastTwoFlight', 'DeletingProblem', 'DeletedSuccessfully', 'ConfirmationDeleteFlight',
        'SureDeleteGivenFlightDetail', 'MaxAddThreeFlight', 'FillMandataryFieldsStar', 'MAWB_Agent_Detail_Saved', 'Agent_Same', 'Errorwhil_uploading_the_excel',
            'ErrorUploadingDocument', 'MAWB_DocumentUploadedSuccessfully', 'PleaseSelectValidFile', 'UploadingDocument', 'While_mawb_Document_Uploading',
            'Updating_MAWB_Detail','Update_MAWB_Detail']).then(function (translations) {

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
            $scope.MAWB_Agent_Detail_Saved = translations.MAWB_Agent_Detail_Saved;
            $scope.Updating_MAWB_Detail = translations.Updating_MAWB_Detail;
            $scope.Errorwhil_uploading_the_excel = translations.Errorwhil_uploading_the_excel;
            $scope.ErrorUploadingDocument = translations.ErrorUploadingDocument;
            $scope.MAWB_DocumentUploadedSuccessfully = translations.MAWB_DocumentUploadedSuccessfully;
            $scope.PleaseSelectValidFile = translations.PleaseSelectValidFile;
            $scope.UploadingDocument = translations.UploadingDocument;
            $scope.While_mawb_Document_Uploading = translations.While_mawb_Document_Uploading;
            $scope.Update_MAWB_Detail = translations.Update_MAWB_Detail;

       
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
                    $scope.SaveMawbModel.TradelaneId = $scope.TradelaneShipmentId;
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
                            $scope.SaveMawbModel.TradelaneId = $scope.TradelaneShipmentId;
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
                            $scope.SaveMawbModel.TradelaneId = $scope.TradelaneShipmentId;
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
                AppSpinner.showSpinnerTemplate($scope.While_mawb_Document_Uploading, $scope.Template);
                 
                TradelaneShipmentService.SaveMAWBAllocation($scope.FinalList).then(function (response) {
                    if (response.data.Status) {
                        toaster.pop({
                            type: 'success',
                            title: $scope.FrayteSuccess,
                            body: $scope.MAWB_Agent_Detail_Saved,
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
 

    $scope.GetInitial = function () {
        AppSpinner.showSpinnerTemplate($scope.Update_MAWB_Detail, $scope.Template);

        TradelaneShipmentService.GetAirlines().then(function (response) {
            $scope.AirlineList = TopAirlineService.TopAirlineList(response.data);
        });
        TradelaneShipmentService.GetTimeZoneList().then(function (response) {
            $scope.timezones = response.data;
        });
        $scope.GetAgent();
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

    $scope.GetTradelaneMawbAllocation = function () {
        TradelaneShipmentService.GetMawbAllocation($scope.TradelaneShipmentId).then(function (response) {
            $scope.MawbAllocation = response.data;
            if ($scope.MawbAllocation != null && $scope.MawbAllocation.length > 0) {
                setMawbEditJson();
            }
            else {
                setMawbAddJson();
            }
            AppSpinner.hideSpinnerTemplate();
        });
    };


    $scope.GetAgent = function () {
        TradelaneShipmentService.GetTradelaneAgents().then(function (response) {
            $scope.Agents = response.data;
            if ($scope.Agents != null && $scope.Agents.length > 0) {
                $scope.GetTradelaneMawbAllocation();
            }
        });
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

    // MAWB Document

    //$scope.WhileAddingMAWB = function ($files, $file, $event) {

    //    if (!$file) {
    //        return;
    //    }
    //    if ($file.$error) {
    //        toaster.pop({
    //            type: 'warning',
    //            title: $scope.FrayteWarning,
    //            body: $scope.PleaseSelectValidFile,
    //            showCloseButton: true
    //        });
    //        return;
    //    }

    //    AppSpinner.showSpinnerTemplate($scope.While_mawb_Document_Uploading, $scope.Template);


    //    $scope.MawbDoc = $file.name;

    //    // Upload the excel file here.
    //    $scope.uploadMAWB = Upload.upload({
    //        url: config.SERVICE_URL + '/TradelaneBooking/UploadMAWBDocument',
    //        file: $file,
    //        fields: {
    //            ShipmentId: $scope.TradelaneShipmentId,
    //            DocType: "MAWBDocument",
    //            UserId: $scope.userId   
    //        }
    //    });

    //    $scope.uploadMAWB.progress($scope.progressMAWB);
    //    $scope.uploadMAWB.success($scope.successMAWB);
    //    $scope.uploadMAWB.error($scope.errorMAWB);
    //};

    //$scope.progressMAWB = function (evt) {
    //    //To Do:  show excel uploading progress message 
    //};
    //$scope.successMAWB = function (data, status, headers, config) {
    //    if (status = 200) {
    //        AppSpinner.hideSpinnerTemplate();
    //        if (data) {
    //            if (data === "Ok") {
    //                toaster.pop({
    //                    type: 'success',
    //                    title: $scope.FrayteSuccess,
    //                    body: $scope.MAWB_DocumentUploadedSuccessfully,
    //                    showCloseButton: true
    //                });

                    
    //            }
    //            else if (data === "MAWB") {
    //                toaster.pop({
    //                    type: 'error',
    //                    title: $scope.FrayteError,
    //                    body: $scope.ErrorUploadingDocument,
    //                    showCloseButton: true
    //                });
    //            }
    //            else {
    //                toaster.pop({
    //                    type: 'error',
    //                    title: $scope.FrayteError,
    //                    body: $scope.ErrorUploadingDocument,
    //                    showCloseButton: true
    //                });
    //            }
    //        }
    //        else {
    //            toaster.pop({
    //                type: 'warning',
    //                title: $scope.FrayteWarning,
    //                body: data.Message,
    //                showCloseButton: true
    //            });
    //        }
    //    }
    //    else {
    //        AppSpinner.hideSpinnerTemplate();
    //        toaster.pop({
    //            type: 'error',
    //            title: $scope.FrayteError,
    //            body: $scope.ErrorUploadingDocument,
    //            showCloseButton: true
    //        });
    //    }
    //};

    //$scope.errorMAWB = function (err) {
    //    AppSpinner.hideSpinnerTemplate();
    //    toaster.pop({
    //        type: 'error',
    //        title: $scope.FrayteError,
    //        body: $scope.Errorwhil_uploading_the_excel,
    //        showCloseButton: true
    //    });
    //};


    // end // MAWB Document 
   

    function init() {
        $scope.submitted = true;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.userInfo = SessionService.getUser();
        $scope.UserRoleId = $scope.userInfo.RoleId;
        $scope.userId = $scope.userInfo.EmployeeId; 
        $scope.TradelaneShipmentId = ShipmentId;
        $scope.CreatedBy = $scope.userInfo.EmployeeId;
         
        $scope.GetInitial();

        setMultilingualOptions();

    }
    init();
});