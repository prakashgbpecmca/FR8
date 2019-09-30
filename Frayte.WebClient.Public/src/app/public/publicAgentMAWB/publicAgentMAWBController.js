/** 
 * Controller
 */
angular.module('ngApp.public').controller('AgentMAWBController', function ($scope, $state, $stateParams, AppSpinner, $translate, $location, config, $filter, PublicService, $timeout, toaster, Upload, UtilityService, TopAirlineService) {

    //translate code 
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteWarning', 'FrayteSuccess', 'CouldNotFindShipmentConfirmationDetail', 'Some_Problem_Try_Again', 'Updating_MAWB_details', 'MAWB_Detail_Saved', 'Fill_Mandatory_Fill', 'Select_Valid_File',
        'ErrorUploadingDocumentTryAgain', 'Successfully_Upload_MAWB', 'Error_Uploading_MAWB_Document', 'Document_Already_Upload_For']).then(function (translations) {
            $scope.Error = translations.FrayteError;
            $scope.Warning = translations.FrayteWarning;
            $scope.Success = translations.FrayteSuccess;
            $scope.TextCouldNotFindShipmentConfirmationDetail = translations.CouldNotFindShipmentConfirmationDetail;
            $scope.Error_Uploading_MAWB_Document = translations.Error_Uploading_MAWB_Document;
            $scope.Some_Problem_Try_Again = translations.Some_Problem_Try_Again;
            $scope.Updating_MAWB_details = translations.Updating_MAWB_details;
            $scope.MAWB_Detail_Saved = translations.MAWB_Detail_Saved;
            $scope.Fill_Mandatory_Fill = translations.Fill_Mandatory_Fill;
            $scope.Select_Valid_File = translations.Select_Valid_File;
            $scope.ErrorUploadingDocumentTryAgain = translations.ErrorUploadingDocumentTryAgain;
            $scope.Successfully_Upload_MAWB = translations.Successfully_Upload_MAWB;
            $scope.Document_Already_Upload_For = translations.Document_Already_Upload_For;

            $scope.GetInitial();
        });
    };
    //end 
    $scope.ChangeAirlineCode = function (Ary, index) {
        if (index === 0) {
            for (i = 0; i < $scope.AirlineList.length; i++) {
                if (Ary.AirlineId === $scope.AirlineList[i].AirlineId) {
                    $scope.AirlineCode = $scope.AirlineList[i].AilineCode;
                    Ary.AirlineCode2 = $scope.AirlineList[i].CarrierCode2;
                    break;
                }
            }
        }
        if (index === 1) {
            for (i = 0; i < $scope.AirlineList.length; i++) {
                if (Ary.AirlineId === $scope.AirlineList[i].AirlineId) {
                    $scope.AirlineCode = $scope.AirlineList[i].AilineCode;
                    Ary.AirlineCode2 = $scope.AirlineList[i].CarrierCode2;
                    break;
                }
            }
        }
        if (index === 2) {
            for (i = 0; i < $scope.AirlineList.length; i++) {
                if (Ary.AirlineId === $scope.AirlineList[i].AirlineId) {
                    $scope.AirlineCode = $scope.AirlineList[i].AilineCode;
                    Ary.AirlineCode2 = $scope.AirlineList[i].CarrierCode2;
                    break;
                }
            }
        }
    };
    var setEditjsonWithoutLeg = function () {
        var flightObj = {
            AirlineId: null,
            Timezone: null,
            ETA: null,
            ETD: null,
            ETAopened: false,
            ETDopened: false,
            dateOptions: {
                formatYear: 'yy',
                startingDay: 1,
                minDate: new Date()
                //dateDisabled: function (data) {
                //    var date = data.date,
                //    mode = data.mode;
                //    return mode === 'day' && (date.getDay() === 0 || date.getDay() === 6);
                //}
            },
            dateOptions1: {
                formatYear: 'yy',
                startingDay: 1,
                minDate: new Date()
                //dateDisabled: function (data) {
                //    var date = data.date,
                //    mode = data.mode;
                //    return mode === 'day' && (date.getDay() === 0 || date.getDay() === 6);
                //}
            }
        };


        $scope.MawbMainObj.MAWB = $scope.MawbAllocation[0].MAWB;

        for (i = 0; i < $scope.MawbAllocation.length; i++) {

            flightObj.FlightNumber = $scope.MawbAllocation[i].FlightNumber;
            flightObj.MawbAllocationId = $scope.MawbAllocation[i].MawbAllocationId;

            for (j = 0; j < $scope.AirlineList.length; j++) {
                if ($scope.MawbAllocation[i].AirlineId === $scope.AirlineList[j].AirlineId) {
                    flightObj.AirlineId = $scope.AirlineList[j].AirlineId;
                    flightObj.AirlineCode2 = $scope.AirlineList[j].CarrierCode2;
                }
            }

            for (j = 0; j < $scope.AirlineList.length; j++) {
                if ($scope.MawbAllocation[i].AirlineId === $scope.AirlineList[j].AirlineId && i === 0) {
                    flightObj.AirlineId = $scope.AirlineList[j].AirlineId;
                    $scope.AirlineCode = $scope.AirlineList[j].AilineCode;
                    flightObj.AirlineCode2 = $scope.AirlineList[j].CarrierCode2;
                }
            }
            for (j = 0; j < $scope.AirlineList.length; j++) {
                if ($scope.MawbAllocation[i].AirlineId === $scope.AirlineList[j].AirlineId && i === 1) {
                    flightObj.AirlineId = $scope.AirlineList[j].AirlineId;
                    $scope.AirlineCode = $scope.AirlineList[j].AilineCode;
                    flightObj.AirlineCode2 = $scope.AirlineList[j].CarrierCode2;
                }
            }

            for (j = 0; j < $scope.AirlineList.length; j++) {
                if ($scope.MawbAllocation[i].AirlineId === $scope.AirlineList[j].AirlineId && i === 2) {
                    flightObj.AirlineId = $scope.AirlineList[j].AirlineId;
                    $scope.AirlineCode = $scope.AirlineList[j].AilineCode;
                    flightObj.AirlineCode2 = $scope.AirlineList[j].CarrierCode2;
                }
            }

            for (jj = 0; jj < $scope.timezones.length; jj++) {
                if ($scope.MawbAllocation[i].TimezoneId === $scope.timezones[jj].TimezoneId) {
                    flightObj.Timezone = $scope.timezones[jj];
                }
            }
            flightObj.ETA = $scope.MawbAllocation[i].ETA ? new Date($scope.MawbAllocation[i].ETA) : new Date();
            flightObj.ETD = $scope.MawbAllocation[i].ETD ? new Date($scope.MawbAllocation[i].ETD) : new Date();
            flightObj.ETATime = $scope.MawbAllocation[i].ETATime;
            flightObj.TimezoneId = $scope.MawbAllocation[i].TimezoneId;
            flightObj.ETDTime = $scope.MawbAllocation[i].ETDTime;
            $scope.MawbMainObj.List.push(flightObj);

            flightObj = {
                FlightNumber: "",
                AirlineId: null,
                ETA: null,
                ETD: null,
                ETAopened: false,
                ETDopened: false
            };
        }

        angular.forEach($scope.MawbMainObj.List, function (obj, key) {
            $scope.ChangeAirlineCode(obj.AirlineId, key);
        });
    };

    var setMawbEditJson = function () {
        setEditjsonWithoutLeg();
    };

    function compare(a, b) {
        if (!isNaN(parseInt(a.HAWB, 10)) && !isNaN(parseInt(b.HAWB, 10))) {
            if (parseInt(a.HAWB, 10) < parseInt(b.HAWB, 10))
            { return -1; }
            if (parseInt(a.HAWB, 10) > parseInt(b.HAWB, 10))
            { return 1; }
        }
        return 0;
    }

    var setHAWBDocumentOrder = function () {
        $scope.MawbMainObj.HAWBPackages.sort(compare);
    };

    $scope.GetTradelaneMawbAllocation = function () {

        PublicService.GetMawbAllocation($scope.action).then(function (response) {
            AppSpinner.hideSpinnerTemplate();
            if (response.status === 200) {
                $scope.sessionExpired = false;
                if (response.data.List.length) {

                    if (response.data.IsMAWBAllocated) {
                        $scope.mawbUplaoded = true;
                    }
                    else {
                        $scope.mawbUplaoded = false;
                    }
                    $scope.MawbDetail = response.data;

                    $scope.MawbMainObj = {
                        MAWB: "",
                        FrayteNumber: $scope.MawbDetail.FrayteNumber,
                        HAWBPackages: $scope.MawbDetail.HAWBpackages,
                        MAWBFileName: '',
                        TradelaneShipmentId: $scope.MawbDetail.TradelaneShipmentId,
                        AgentId: $scope.MawbDetail.AgentId,
                        List: []
                    };

                    $scope.MawbAllocation = $scope.MawbDetail.List;
                    if ($scope.MawbAllocation.length > 0) {
                        setMawbEditJson();
                    }
                    setHAWBDocumentOrder();
                }

                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.Error,
                        body: $scope.Some_Problem_Try_Again,
                        showCloseButton: true
                    });
                }

            }
            else {
                toaster.pop({
                    type: 'error',
                    title: $scope.Error,
                    body: $scope.Some_Problem_Try_Again,
                    showCloseButton: true
                });
            }
        }, function (error) {
            AppSpinner.hideSpinnerTemplate();
            if (error.status === 400 && error.data && error.data.Message === 'SessionExpired') {
                $scope.sessionExpired = true;
            }
            else {
                toaster.pop({
                    type: 'error',
                    title: $scope.Error,
                    body: $scope.Some_Problem_Try_Again,
                    showCloseButton: true
                });
            }
        });
    };


    $scope.SaveMawbAllocation = function (IsValid) {

        if (IsValid) {

            $scope.MawbDetail.MAWB = $scope.MawbMainObj.MAWB;
            $scope.MawbDetail.MAWBFileName = $scope.MawbMainObj.MAWBFileName;

            angular.forEach($scope.MawbMainObj.List, function (obj) {
                if (obj.Timezone && obj.Timezone.TimezoneId) {
                    obj.TimezoneId = obj.Timezone.TimezoneId;
                }
            });

            AppSpinner.showSpinnerTemplate($scope.Updating_MAWB_details, $scope.Template);

            PublicService.SaveMawbAllocation($scope.MawbMainObj).then(function (response) {
                if (response.data.Status) {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'success',
                        title: $scope.Success,
                        body: $scope.MAWB_Detail_Saved,
                        showCloseButton: true
                    });
                    $timeout(function () {
                        $state.go('home.welcome');
                    }, 4000);
                }
                else {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.Error,
                        body: $scope.Some_Problem_Try_Again,
                        showCloseButton: true
                    });
                }
            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.Error,
                    body: $scope.Some_Problem_Try_Again,
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.Warning,
                body: $scope.Fill_Mandatory_Fill,
                showCloseButton: true
            });
        }
    };
    $scope.GetInitial = function () {
        AppSpinner.showSpinnerTemplate("", $scope.Template);
        PublicService.GetAirlines().then(function (response) {
            $scope.AirlineList = TopAirlineService.TopAirlineList(response.data);
            PublicService.GetTimeZoneList().then(function (response) {
                $scope.timezones = response.data;
                $scope.GetTradelaneMawbAllocation();
            });
        }, function () {
            AppSpinner.hideSpinnerTemplate();
        });


    };
    $scope.OpenCa = function ($event, index, arr) {
        for (i = 0; i < $scope.WithLegJson.SecondLeg.MawbMainObj.List.length; i++) {
            if (index === i) {
                $scope.WithLegJson.SecondLeg.MawbMainObj.List[i].ETAopened = true;
            }
        }
    };
    $scope.OpenCa1 = function ($event, index, arr) {
        for (i = 0; i < $scope.WithLegJson.SecondLeg.MawbMainObj.List.length; i++) {
            if (index === i) {
                $scope.WithLegJson.SecondLeg.MawbMainObj.List[i].ETDopened = true;
            }
        }
    };
    $scope.OpenCal = function ($event, index, arr) {
        for (i = 0; i < $scope.WithLegJson.FirstLeg.MawbMainObj.List.length; i++) {
            if (index === i) {
                $scope.WithLegJson.FirstLeg.MawbMainObj.List[i].ETAopened = true;
            }
        }
    };
    $scope.OpenCal1 = function ($event, index, arr) {
        for (i = 0; i < $scope.WithLegJson.FirstLeg.MawbMainObj.List.length; i++) {
            if (index === i) {
                $scope.WithLegJson.FirstLeg.MawbMainObj.List[i].ETDopened = true;
            }
        }
    };
    $scope.OpenCalender = function ($event, index, arr) {
        for (i = 0; i < $scope.MawbMainObj.List.length; i++) {
            if (index === i) {
                $scope.MawbMainObj.List[i].ETAopened = true;
            }
        }
    };
    $scope.OpenCalender1 = function ($event, index, arr) {
        for (i = 0; i < $scope.MawbMainObj.List.length; i++) {
            if (index === i) {
                $scope.MawbMainObj.List[i].ETDopened = true;
            }
        }
    };


    //Upload MAWB Doc
    $scope.WhileAddingMAWBDoc = function ($files, $file, $event) {
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
                ShipmentId: $scope.MawbMainObj.TradelaneShipmentId,
                UserId: $scope.MawbMainObj.AgentId
            }
        });

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

    //

    function init() {

        $scope.url = UtilityService.getPublicSiteName();
        $scope.action = $stateParams.action;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        setModalOptions();
    }

    init();

});