angular.module('ngApp.tradelaneShipments').controller('TradelaneMAWBAllocationController', function ($scope, $state, TradelaneMilestoneService, $translate, $uibModal, toaster, ModalService, SessionService, AppSpinner, DirectBookingService, DirectShipmentService, $rootScope, TradelaneShipmentService, uiGridConstants, config, DateFormatChange, ShipmentId, $uibModalInstance, TopAirlineService, Upload) {

    //multilingual translation key
    var setMultilingualOptions = function () {
        $translate(['FrayteError', 'FrayteValidation', 'FrayteInformation', 'FrayteSuccess', 'CannotRemoveDetailAtleastTwoFlight', 'DeletingProblem', 'DeletedSuccessfully', 'ConfirmationDeleteFlight',
            'SureDeleteGivenFlightDetail', 'Successfully_Upload_MAWB', 'Error_Uploading_MAWB_Document', 'Document_Already_Upload_For', 'MaxAddThreeFlight', 'FillMandataryFieldsStar', 'AgentSavedSuccessfully', 'Agent_Same']).then(function (translations) {
                $scope.FrayteWarning = translations.FrayteValidation;
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
                $scope.Successfully_Upload_MAWB = translations.Successfully_Upload_MAWB;
                $scope.Document_Already_Upload_For = translations.Document_Already_Upload_For;
                $scope.Error_Uploading_MAWB_Document = translations.Error_Uploading_MAWB_Document;
            });
    };

    //Upload MAWB Doc
    $scope.WhileAddingMAWB = function ($files, $file, $event) {
        if (!$file) {
            return;
        }
        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.Warning,
                body: $scope.Select_Valid_File,
                showCloseButton: true
            });
            return;
        }

        $scope.MawbMainObj.MAWBFileName = $file.name;

        // Upload MAWB Doc
        $scope.uploadMAWBW = Upload.upload({
            url: config.SERVICE_URL + '/TradelaneBooking/UploadMAWBDocument',
            file: $file,
            fields: {
                DocType: "MAWB",
                ShipmentId: $scope.TradelaneShipmentId,
                UserId: $scope.CustomerId
            }
        });

        //$scope.MawbDoc = $file.name;
        if ($scope.ShipmentHandlerMethodId !== 5) {

            $scope.MawbMainObj.MawbDoc = $file.name;
        }
        else {

            $scope.WithLegJson.MawbDoc = $file.name;
        }
        $scope.uploadMAWBW.progress($scope.progressuploadMAWB);

        $scope.uploadMAWBW.success($scope.successuploadMAWB);

        $scope.uploadMAWBW.error($scope.erroruploadMAWB);
    };

    $scope.progressuploadMAWB = function (evt) {
        //To Do:  show excel uploading progress message 
    };

    $scope.successuploadMAWB = function (data, status, headers, config) {
        if (status = 200) {

            if (data === "Ok") {
                toaster.pop({
                    type: 'success',
                    title: $scope.Success,
                    body: $scope.Successfully_Upload_MAWB,
                    showCloseButton: true
                });
            }
            else if (data === "Failed") {
                toaster.pop({
                    type: 'error',
                    title: $scope.Error,
                    body: $scope.Error_Uploading_MAWB_Document,
                    showCloseButton: true
                });
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.Error,
                    body: $scope.Document_Already_Upload_For + data,
                    showCloseButton: true
                });
            }
        }
        else {
            toaster.pop({
                type: 'error',
                title: $scope.Error,
                body: $scope.ErrorUploadingDocumentTryAgain,
                showCloseButton: true
            });
        }
    };

    $scope.erroruploadMAWB = function (err) {
        toaster.pop({
            type: 'warning',
            title: $scope.Warning,
            body: $scope.ErrorUploadingDocumentTryAgain,
            showCloseButton: true
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

    $scope.SaveMawbAllocation = function (isValid) {
        if (isValid && !$scope.LegJsonValue1 && !$scope.LegJsonValue2) {

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
                    $scope.SaveMawbModel.CreatedBy = $scope.CustomerId;
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
                            $scope.SaveMawbModel.CreatedBy = $scope.CustomerId;
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
                            $scope.SaveMawbModel.CreatedBy = $scope.CustomerId;
                            $scope.FinalList.push($scope.SaveMawbModel);
                            $scope.SaveMawbfun();
                        }
                    }
                }
            }
            if ($scope.FinalList.length > 0) {
                AppSpinner.showSpinnerTemplate("Allocating agent for shipment.", $scope.Template);
                TradelaneShipmentService.SaveMAWBAllocation($scope.FinalList).then(function (response) {
                    if (response.data) {
                        toaster.pop({
                            type: 'success',
                            title: $scope.FrayteSuccess,
                            body: $scope.MAWBSavedSuccessfully,
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
        else if ($scope.LegJsonValue1 || $scope.LegJsonValue2) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.AgentSame,
                showCloseButton: true
            });
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
            MAWB: ($scope.MAWB !== undefined && $scope.MAWB !== null && $scope.MAWB !== '') ? $scope.MAWB : "",
            TradelaneShipmentId: $scope.TradelaneShipmentId,
            Agent: null,
            flightArray: []

        };
        if ($scope.ShipmentHandlerMethodId === 1) {
            $scope.MawbMainObj.flightArray.push(flightObj);
        }
        else if ($scope.ShipmentHandlerMethodId === 2 || $scope.ShipmentHandlerMethodId === 4) {
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
            MAWB: ($scope.MAWB !== undefined && $scope.MAWB !== null && $scope.MAWB !== '') ? $scope.MAWB : "",
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
            MAWB: ($scope.MAWB !== undefined && $scope.MAWB !== null && $scope.MAWB !== '') ? $scope.MAWB : "",
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
        TradelaneShipmentService.GetShipmentHandlerId($scope.TradelaneShipmentId).then(function (response) {
            $scope.ShipmentHandlerMethodId = response.data.ShipmentHandlerMethodId;
            $scope.FromAirport = response.data.RouteFrom;
            $scope.ToAirport = response.data.RouteTo;
            $scope.ShipmentHandlerCode = response.data.ShipemntHandlerMethodCode;
            $scope.MAWB = response.data.MAWB;
            if ($scope.ShipmentHandlerMethodId != null && $scope.ShipmentHandlerMethodId > 0) {
                if ($scope.ShipmentHandlerMethodId !== 5) {
                    setjsonWithoutLeg();
                    $scope.MawbMainObj.MawbDoc = $scope.MawbDocument;
                }
                else {
                    setjsonWithLeg();
                    $scope.WithLegJson.MawbDoc = $scope.MawbDocument;
                }
            }
        });
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
                    $scope.MawbMainObj.MawbDoc = $scope.MawbDocument;
                }
                else {
                    setEditjsonWithLeg();
                    $scope.WithLegJson.MawbDoc = $scope.MawbDocument;
                }
            }
        });
    };

    function originCountryTimeZone() {
        if ($scope.TradelaneShipmentId !== undefined && $scope.TradelaneShipmentId !== null && $scope.TradelaneShipmentId !== '' && $scope.TradelaneShipmentId > 0) {
            TradelaneShipmentService.GetTimeZoneName($scope.TradelaneShipmentId).then(function (response) {
                if (response.data !== undefined && response.data !== null && response.data !== '') {
                    angular.forEach($scope.timezones, function (item, key) {
                        if (response.data === item.Name) {
                            if ($scope.ShipmentHandlerMethodId !== undefined && $scope.ShipmentHandlerMethodId !== null && $scope.ShipmentHandlerMethodId !== '' && $scope.ShipmentHandlerMethodId !== 5) {
                                $scope.MawbMainObj.flightArray[0].Timezone = item;
                                $scope.MawbMainObj.flightArray[1].Timezone = item;
                            }
                            else {
                                $scope.WithLegJson.FirstLeg.MawbMainObj.flightArray[0].Timezone = item;
                                $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[0].Timezone = item;
                                $scope.WithLegJson.SecondLeg.MawbMainObj.flightArray[1].Timezone = item;
                            }
                        }
                    });
                }
            });
        }
    }

    $scope.GetTradelaneMawbAllocation = function () {
        TradelaneShipmentService.GetMawbAllocation($scope.TradelaneShipmentId).then(function (response) {
            $scope.MawbAllocation = response.data;
            if ($scope.MawbAllocation != null && $scope.MawbAllocation.length > 0) {
                setMawbEditJson();
            }
            else {
                setMawbAddJson();
                originCountryTimeZone();
            }
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
        TradelaneShipmentService.GetMawbDocumentName($scope.TradelaneShipmentId).then(function (response) {
            $scope.MawbDocument = response.data;
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

    function init() {
        $scope.submitted = true;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        var userInfo = SessionService.getUser();
        $scope.$UserRoleId = userInfo.RoleId;
        $scope.StaffRoleId = userInfo.RoleId;
        $scope.CustomerId = userInfo.EmployeeId;
        $scope.TradelaneShipmentId = ShipmentId;
        $scope.GetInitial();
        setMultilingualOptions();
    }

    init();

});