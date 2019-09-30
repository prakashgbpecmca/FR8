angular.module('ngApp.margin').controller('MarginController', function ($scope, SessionService, MarginService, toaster, $translate) {


    var setMultilingualOptions = function () {
        $translate(['Success', 'Record_Saved', 'FrayteError_Validation', 'ErrorSavingRecord', 'GettingDetails_Error', 'CancelShipmentError_Validation',
            'GeneratePdfError_Validation', 'GeneratePdfError_Validation', 'SuccessfullySendlLabel_Validation', 'SendingMailError_Validation',
        'EnterValidEmailAdd', 'TrackShipmentNotTrack_Error', 'FrayteWarning', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation',
        'ErrorSavingRecord', 'FrayteError', 'FrayteSuccess', 'MarginRate_Save', 'ErrorGettingRecord']).then(function (translations) {

            $scope.Frayte_Warning = translations.FrayteWarning;
            $scope.PleaseCorrect_ValidationErrors = translations.PleaseCorrectValidationErrors;
            $scope.Successfully_SavedInformation = translations.SuccessfullySavedInformation;
            $scope.ErrorSaving_Record = translations.ErrorSavingRecord;
            $scope.Frayte_Error = translations.FrayteError;
            $scope.Frayte_Success = translations.FrayteSuccess;
            $scope.MarginRateSave = translations.MarginRate_Save;
            $scope.ErrorGetting_Record = translations.ErrorGettingRecord;


        });
    };
    $scope.CopyLogisticCompany = function (Type) {

        getScreenInitials(Type);

    };
    $scope.SaveOptions = function () {
        MarginService.AddOptions($scope.CopyData).then(function (response) {
            toaster.pop({
                type: 'success',
                title: $scope.Frayte_Success,
                body: $scope.MarginRateSave,
                showCloseButton: true
            },
            function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.Frayte_Error,
                    body: $scope.ErrorSaving_Record,
                    showCloseButton: true
                });
            });
        });
    };


    //need to resolve the close button problem

    $scope.removeOption = function (index) {

        
        if ($scope.CopyData.OptionId === undefined || $scope.CopyData.OptionId === null)
        {
            $scope.CopyData.OptionId = [];
        }
        
        
        for (i = 0; i < $scope.CopyData.Options[index].MarginRates.length; i++) {
            $scope.CopyData.OptionId.push($scope.CopyData.Options[index].MarginRates[i].CustomerMarginOptionId);
        }

        $scope.CopyData.Options.splice(index, 1);
        
    };


    // add margin option
    $scope.AddOption = function () {

        $scope.CopyData.Options.push({
            OptionName: '',
            OptionDisplayName: '',
            MarginRates: [{
                CustomerMarginOptionId: 0,
                ShipmentType: "Doc",
                ShipmentTypeDisplayText: "Doc",
                MarginPercentage: null
            },
            {
                CustomerMarginOptionId: 0,
                ShipmentType: "Nondoc",
                ShipmentTypeDisplayText: "Nondoc",
                MarginPercentage: null
            },
            {
                CustomerMarginOptionId: 0,
                ShipmentType: "Doc&Nondoc",
                ShipmentTypeDisplayText: "Doc & Nondoc",
                MarginPercentage: null
            },
            {
                CustomerMarginOptionId: 0,
                ShipmentType: "HeavyWeight",
                ShipmentTypeDisplayText: "Heavy Weight",
                MarginPercentage: null
            }]

        });

    };

    $scope.MarginOptionByCompany = function () {
        getScreenInitials();
    };


    var setMarginOption = function () {
        var data = $scope.CopyFromData;
        if (data.Options !== null && data.Options.length) {
            for (var i = 0 ; i < data.Options.length ; i++) {
                data.Options[i].OptionName = data.Options[i].OptionName + " New";
                data.Options[i].OptionDisplayName = data.Options[i].OptionDisplayName + " New";
                for (var b = 0 ; b < data.Options[i].MarginRates.length; b++) {
                    data.Options[i].MarginRates[b].CustomerMarginOptionId = 0;
                }
                $scope.CopyData.Options.push(data.Options[i]);
            }
        }


    };

    var getScreenInitials = function (Type) {

        var CourierCompany = "";
        if ($scope.CourierCompany !== undefined && (Type === undefined || Type === null || Type === '')) {
            CourierCompany = $scope.CourierCompany.LogisticCompany;
        }
        else if ($scope.CourierCompanyCopy !== undefined && Type === 'CopyFrom') {
            CourierCompany = $scope.CourierCompanyCopy.LogisticCompany;
        }

        MarginService.GetOptions($scope.Company.OperationZoneId, CourierCompany).then(function (response) {
            if (response.data !== null) {
                if (Type === undefined || Type === null) {
                    $scope.CopyData = response.data;
                }
                else {
                    //$scope.CopyFromData = response.data;
                    $scope.CopyFromDatanew = response.data;
                    $scope.CopyCompanyData();
                    //setMarginOption();
                }


            }
            else {
                $scope.CopyData = { Options: [] };
            }


        }, function () {
            $scope.CopyData = { Options: [] };
            toaster.pop({
                type: 'error',
                title: $scope.Frayte_Error,
                body: $scope.ErrorGetting_Record,
                showCloseButton: true
            });
        });
    };

    $scope.addNewOption = function () {
        var optionArray = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'];
        if ($scope.CopyData !== null || $scope.CopyData !== undefined) {

            var optLenght = 0;
            $scope.NewArray = { newArray: [] };

            for (var i = 0; i < $scope.CopyData.Options.length; i++) {
                $scope.NewArray.newArray.push($scope.CopyData.Options[i].OptionName);
            }

            for (var j = 0; j < optionArray.length; j++) {

                if ($scope.NewArray.newArray[j] !== optionArray[j]) {

                    if (optLenght === 0) {

                        $scope.NewOption = optionArray[j];
                        $scope.CopyData.Options.push({
                            OptionName: $scope.NewOption,
                            OptionDisplayName: 'Option ' + $scope.NewOption,
                            MarginRates: [{
                                CustomerMarginOptionId: 0,
                                ShipmentType: "Doc",
                                ShipmentTypeDisplayText: "Doc",
                                MarginPercentage: null
                            },
                            {
                                CustomerMarginOptionId: 0,
                                ShipmentType: "Nondoc",
                                ShipmentTypeDisplayText: "Nondoc",
                                MarginPercentage: null
                            },
                            {
                                CustomerMarginOptionId: 0,
                                ShipmentType: "Doc&Nondoc",
                                ShipmentTypeDisplayText: "Doc & Nondoc",
                                MarginPercentage: null
                            },
                            {
                                CustomerMarginOptionId: 0,
                                ShipmentType: "HeavyWeight",
                                ShipmentTypeDisplayText: "Heavy Weight",
                                MarginPercentage: null
                            }]

                        });
                        optLenght++;
                    }

                }

            }

        }
    };

    $scope.CopyCompanyData = function () {
        var CopyLen = 0;
        var checkCopyData = [];
        var checkCopyData1 = [];
        var CopyDataLen = $scope.CopyData.Options.length - 1;
        for (i = 0; i < $scope.CopyData.Options.length; i++) {
            CopyLen = CopyLen + $scope.CopyData.Options[i].MarginRates.length;
            for (j = 0; j < $scope.CopyData.Options[i].MarginRates.length; j++) {

                if ($scope.CopyData.Options[i].MarginRates[j].MarginPercentage === 0) {

                    if ($scope.CopyData.Options[i].MarginRates[j].MarginPercentage === 0) {

                        //$scope.DataVal = true;
                        checkCopyData.push(true);

                    }
                    else {
                        checkCopyData1.push(false);
                    }
                }
            }
        }
        //for (k = 0; k < checkCopyData.length; k++) {
        //    if (checkCopyData[k] !== false) {
        //        //$scope.DataVal = true;
        //        $scope.DataVal = false;
        //    }
        //    else {
        //        //$scope.DataVal = false;
        //    }
        //}

        if (CopyLen === checkCopyData.length) {
            $scope.DataVal = true;
        }
        else {
            $scope.DataVal = false;
        }
        if ($scope.DataVal) {
            for (i = 0; i < $scope.CopyData.Options.length; i++) {
                for (j = 0; j < $scope.CopyData.Options[i].MarginRates.length; j++) {
                    if (i < $scope.CopyFromDatanew.Options.length) {
                        if ($scope.CopyData.Options[i].OptionName === $scope.CopyFromDatanew.Options[i].OptionName &&
                            $scope.CopyData.Options[i].MarginRates[j].ShipmentType === $scope.CopyFromDatanew.Options[i].MarginRates[j].ShipmentType) {
                            $scope.CopyData.Options[i].MarginRates[j].MarginPercentage = $scope.CopyFromDatanew.Options[i].MarginRates[j].MarginPercentage;
                        }
                    }
                }
            }
        }
    };

    function init() {
        $scope.CopyData = { Options: [] };
        var userInfo = SessionService.getUser();
        if (userInfo !== undefined) {
            $scope.CustomerId = userInfo.EmployeeId;
            $scope.Company = { OperationZoneId: userInfo.OperationZoneId };
        }
        MarginService.GetCompany($scope.Company.OperationZoneId).then(function (response) {
            $scope.Companies = response.data;
            $scope.CourierCompany = $scope.Companies[0];

            getScreenInitials();
        });
        setMultilingualOptions();
    }
    init();

});