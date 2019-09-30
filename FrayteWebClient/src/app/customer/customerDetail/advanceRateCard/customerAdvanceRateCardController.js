angular.module('ngApp.customer').controller('CustomerAdvanceRateCardController', function ($scope, CustomerService, ZoneBaseRateCardService, $state, toaster, AppSpinner, $translate) {
   
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;


            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
            $scope.RecordGettingError = translations.ErrorGetting + " " + translations.records;
            $scope.Success = translations.Success;
            $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
            $scope.UpdateRecordValidation = translations.UpdateRecord_Validation;
        });
    };

    var getScreenInitials = function () {
        CustomerService.GetOperationZones().then(function (response) {
            $scope.OperationZones = response.data;
            $scope.OperationZone = response.data[0];
            if ($scope.OperationZone) {
                CustomerService.GetLogisticItems($scope.OperationZone.OperationZoneId).then(function (response) {

                    $scope.LogisticItems = response.data;
                    $scope.CourierCompanies = response.data.LogisticCompanies;
                    angular.copy($scope.LogisticItems, $scope.masterData);
                    $scope.RateTypes = response.data.LogisticRateTypes;
                    $scope.LogisticTypes = response.data.LogisticTypes;
                    $scope.LogisticType = $scope.LogisticTypes[1];
                    $scope.CourierCompany = $scope.CourierCompanies[0];
                    $scope.ShipmentDocTypes = response.data.DocTypes;
                    $scope.DocType = response.data.DocTypes[0];
                    $scope.RateType = $scope.RateTypes[0];
                    $scope.GetZones();

                    var OperationZoneId = 0;
                    var LogisticType = "";
                    var CourierCompany = "";
                    var RateType = "";
                    var ModuleType = "DirectBooking";

                    if ($scope.OperationZone !== undefined) {
                        OperationZoneId = $scope.OperationZone.OperationZoneId;
                    }

                    if ($scope.LogisticType !== undefined) {
                        LogisticType = $scope.LogisticType.Value;
                    }
                    if ($scope.CourierCompany !== undefined) {
                        CourierCompany = $scope.CourierCompany.Value;
                    }

                    if ($scope.RateType !== undefined && $scope.RateType !== null) {
                        RateType = $scope.RateType.Value;
                    }

                    CustomerService.GetShipmentTypes(OperationZoneId, CourierCompany, LogisticType, RateType, ModuleType).then(function (response) {
                        $scope.ShipmentTypes = response.data;
                        $scope.ShipmentType = $scope.ShipmentTypes[0];

                        getWeights();
                    }, function () {
                        toaster.pop({
                            type: 'error',
                            title: $scope.TitleFrayteError,
                            body: $scope.RecordGettingError,
                            showCloseButton: true
                        });
                    });

                }, function () {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.RecordGettingError,
                        showCloseButton: true
                    });

                });
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });

        });
    };

    var getScreenInitialsOnLogisticType = function () {
        var logisticServiceType = "";
        if ($scope.LogisticService !== undefined) {
            logisticServiceType = $scope.LogisticService.LogisticServiceName;

        }
        $scope.GetZones();

        ZoneBaseRateCardService.GetShipmentType($scope.LogisticType.LogisticName, logisticServiceType).then(function (response) {
            $scope.ShipmentTypes = response.data;
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });
        //getCurrencyCodes();
        getWeights();
    };

    $scope.GetZones = function () {
        var OperationZoneId = 0;
        var LogisticType = "";
        var CourierCompany = "";
        // var RateType = "";
        var ModuleType = "DirectBooking";
        if ($scope.OperationZone !== undefined) {
            OperationZoneId = $scope.OperationZone.OperationZoneId;
        }

        if ($scope.LogisticType !== undefined) {
            LogisticType = $scope.LogisticType.Value;
        }
        if ($scope.CourierCompany !== undefined) {
            CourierCompany = $scope.CourierCompany.Value;
        }

        //if ($scope.RateType !== undefined && $scope.RateType !== null) {
        //    RateType = $scope.RateType.Value;
        //}
        if ($scope.RateType !== undefined && $scope.RateType !== null && $scope.CourierCompany.Value !== 'Hermes' &&
            $scope.CourierCompany.Value !== 'Yodel' &&
            $scope.CourierCompany.Value !== 'UKMail') {
            RateType = $scope.RateType.Value;
        }
        if ($scope.LogisticType.Value === 'UKShipment' && ($scope.CourierCompany.Value === 'UKMail' || $scope.CourierCompany.Value === 'Hermes' || $scope.CourierCompany.Value === 'Yodel')) {
            RateType = "";
        }

        CustomerService.GetZoneList(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType).then(function (response) {
            $scope.Zones = response.data;
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });
        ZoneBaseRateCardService.GetZones(OperationZoneId, "UKShipment", CourierCompany, RateType, ModuleType).then(function (response) {
            $scope.UKShipmentZones = response.data;
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });

    };

    $scope.SearchAdvanceRateCard = function () {

        //$scope.PodType = $scope.PodType;
        //$scope.ParcelType = $scope.ParcelServiceType;
        //$scope.PackageType = $scope.PakageType;
        //if ($scope.PodType !== undefined && $scope.PodType !== null)
        //{
        //    $scope.PackageType = $scope.PodType;

        //}
        //else {
        //    $scope.PackageType = $scope.PakageType;
        //}

        $scope.GetAdvanceRateCard();
        //getWeights();
    };

    $scope.GetAdvanceRateCard = function () {
        $scope.GetZones();
        //CustomerService.GetOperationZones().then(function (response) {
        //    $scope.OperationZones = response.data;
        //    $scope.OperationZone = response.data[0];
        //    CustomerService.GetLogisticItems($scope.currentOperationZone.OperationZoneId).then(function (response) {

        //        $scope.LogisticItems = response.data;
        //        $scope.CourierCompanies = response.data.LogisticCompanies;
        //        $scope.RateTypes = response.data.LogisticRateTypes;
        //        $scope.LogisticTypes = response.data.LogisticTypes;
        //        $scope.LogisticType = $scope.LogisticTypes[1];
        //        $scope.CourierCompany = $scope.CourierCompanies[0];
        //        $scope.ShipmentDocTypes = response.data.DocTypes;
        //        //$scope.DocType = response.data.DocTypes[0];
        //        $scope.RateType = $scope.RateTypes[0];


        var OperationZoneId = 0;
        var LogisticType = "";
        var CourierCompany = "";
        var RateType = "";
        var ModuleType = "DirectBooking";

        if ($scope.OperationZone !== undefined) {
            OperationZoneId = $scope.OperationZone.OperationZoneId;
        }

        if ($scope.LogisticType !== undefined) {
            LogisticType = $scope.LogisticType.Value;
        }
        if ($scope.CourierCompany !== undefined) {
            CourierCompany = $scope.CourierCompany.Value;
        }

        if ($scope.RateType !== undefined && $scope.RateType !== null) {
            RateType = $scope.RateType.Value;
        }
        if ($scope.LogisticType.Value === 'UKShipment' && ($scope.CourierCompany.Value === 'UKMail' || $scope.CourierCompany.Value === 'Hermes' || $scope.CourierCompany.Value === 'Yodel')) {
            RateType = "";
        }

        CustomerService.GetShipmentTypes(OperationZoneId, CourierCompany, LogisticType, RateType, ModuleType).then(function (response) {
            $scope.ShipmentTypes = response.data;
            $scope.ShipmentType = $scope.ShipmentTypes[0];


        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });

       
        getWeights();
       
    };

    var getWeights = function () {
        var OperationZoneId = 0;
        var LogisticType = "";
        var CourierCompany = "";
        var RateType = "";
        var PackageType = "";
        var ParcelType = "";
        var DocType = "";
        var ModuleType = "DirectBooking";

        if ($scope.OperationZone !== undefined) {
            OperationZoneId = $scope.OperationZone.OperationZoneId;
        }

        if ($scope.LogisticType !== undefined) {
            LogisticType = $scope.LogisticType.Value;
        }
        if ($scope.CourierCompany !== undefined) {
            CourierCompany = $scope.CourierCompany.Value;
        }

        if ($scope.RateType !== undefined && $scope.RateType !== null) {
            RateType = $scope.RateType.Value;
        }
        if ($scope.PakageType !== undefined && $scope.PakageType !== null) {
            PackageType = $scope.PakageType.PakageType;
        }


        if ($scope.LogisticType.Value === 'UKShipment' && ($scope.CourierCompany.Value === 'UKMail' || $scope.CourierCompany.Value === 'Hermes' || $scope.CourierCompany.Value === 'Yodel')) {
            RateType = "";
        }
        if ($scope.ParcelServiceType !== undefined && $scope.ParcelServiceType !== null) {
            ParcelType = $scope.ParcelServiceType.ParcelServiceType;
        }
        if ($scope.CourierCompany.Value === 'Hermes' && $scope.PodType !== undefined) {
            PackageType = $scope.PodType.PodTypeName;
            ParcelType = "";
            //PackageType = null;
            //RateType = null;
        }
        if ($scope.DocType !== undefined && $scope.DocType !== null) {
            DocType = $scope.DocType.Value;
        }
        if ($scope.LogisticType.Value === 'UKShipment') {
            DocType = "";
        }

        if ($scope.LogisticType.Value !== 'UKShipment' && $scope.CourierCompany.Value !== 'UKMail') {
            PackageType = "";
            ParcelType = "";
        }
        if ($scope.LogisticType.Value === 'UKShipment' && $scope.CourierCompany.Value === 'DHL') {
            PackageType = "";
            ParcelType = "";
        }
        ZoneBaseRateCardService.GetWeight(OperationZoneId, LogisticType, CourierCompany, RateType, PackageType, ParcelType, ModuleType).then(function (response) {
            if (response.data !== null && response.data.length > 0) {
                $scope.LogisticWeight = response.data;
                //$scope.GetAdvanceRateCard();
            }
            else {
                AppSpinner.hideSpinnerTemplate();
            }

        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });

        CustomerService.GetCustomerAdvanceMarginCost(OperationZoneId, $scope.CustomerId, CourierCompany, LogisticType, RateType, PackageType, ParcelType, DocType, ModuleType).then(function (response) {
            $scope.ImportExprtThirdPartyRawData = response.data;
            $scope.ZoneBaseRateCardList = response.data;
            if ($scope.ImportExprtThirdPartyRawData) {
                if ($scope.LogisticType.Value === "UKShipment" && (PackageType === "Multiple" || PackageType === "Single") && ParcelType === "Parcel" && $scope.CourierCompany.Value === "UKMail") {
                    uKShipmentParcelJson();
                }
                else if ($scope.LogisticType.Value === "UKShipment" && (PackageType === "Multiple" || PackageType === "Single") && ParcelType === "BagItService" && $scope.CourierCompany.Value === "UKMail") {
                    uKShipmentBagitJson();
                }
                else if ($scope.LogisticType.Value === "UKShipment" && $scope.CourierCompany.Value === "Yodel") {
                    uKShipmentYodelJson();
                }
                else if ($scope.LogisticType.Value === "UKShipment" && $scope.CourierCompany.Value === "Hermes") {
                    uKShipmentHermesJson();
                }
                else if ($scope.LogisticType.Value === "UKShipment" && $scope.CourierCompany.Value === "DHL" && $scope.RateType.Value === "Express") {
                    ukShipmentDHLJson();
                }
                    //else if ($scope.LogisticType.Value === "EUExport" || $scope.LogisticType.Value === "EUImport") {
                    //    eUImportExportJson();
                    //}
                else {
                    importExportThirdPartyJson();
                }
            }
        });

    };
    $scope.AdvanceRateCardByZone = function () {

        getLogisticItems();

        if ($scope.OperationZone.OperationZoneId === 1) {
            $scope.showPakageType = false;
            $scope.showParcelServiceType = false;
            $scope.showLogisticServiceType = false;

        }
        else {
            //if ($scope.LogisticType.Value === "UKShipment") {
            //    $scope.showPakageType = true;
            //    $scope.showParcelServiceType = true;
            //    $scope.showLogisticServiceType = true;
            //    $scope.ParcelServiceType = $scope.ParcelServiceTypes[0];
            //    $scope.PakageType = $scope.PakageTypes[0];
            //}
        }

        $scope.showRateExportImport = false;
        //getScreenInitialsOnLogisticType();

    };

    $scope.AdvanceRateCardByLogisticType = function () {

        setCourierCompanyAndRateTypeDropDown();

        if ($scope.LogisticType.Value === "UKShipment") {

            $scope.showPakageType = true;
            $scope.showParcelServiceType = true;
            $scope.showLogisticServiceType = true;
            $scope.ParcelServiceType = $scope.ParcelServiceTypes[0];
            $scope.PakageType = $scope.PakageTypes[0];
        }
        else {
            $scope.showPakageType = false;
            $scope.showParcelServiceType = false;
            $scope.showLogisticServiceType = false;
        }
        $scope.IsShowImportExportThirdParty = false;

        if ($scope.CourierCompany !== null && $scope.CourierCompany.Value !== "UKMail") {
            $scope.showParcelServiceType = false;
            $scope.showPakageType = false;
        }
        else {
            $scope.showParcelServiceType = true;
            $scope.showPakageType = true;
        }
        //getScreenInitialsOnLogisticType();
    };

    var setCourierCompanyAndRateTypeDropDown = function () {
        $scope.LogisticData = angular.copy($scope.masterData);

        $scope.CourierCompanies = $scope.LogisticData.LogisticCompanies;

        if ($scope.LogisticType !== undefined && $scope.LogisticType !== null && $scope.LogisticType.Value === "UKShipment") {

            for (var j = 0; j < $scope.CourierCompanies.length; j++) {
                //if ($scope.CourierCompanies[j].Value === "DHL") {
                //    $scope.CourierCompanies.splice(j, 1);
                //}
                if ($scope.CourierCompanies[j].Value === "FedEx") {
                    $scope.CourierCompanies.splice(j, 1);
                }
                if ($scope.CourierCompanies[j].Value === "TNT") {
                    $scope.CourierCompanies.splice(j, 1);
                }
            }
        }
        else if ($scope.LogisticType !== undefined && $scope.LogisticType !== null &&
            ($scope.LogisticType.Value === "Import" || $scope.LogisticType.Value === "Export" ||
            $scope.LogisticType.Value === "ThirdParty" || $scope.LogisticType.Value === "EUExport")) {
            for (var i = 0; i < $scope.CourierCompanies.length; i++) {
                if ($scope.CourierCompanies[i].Value === "Yodel") {
                    $scope.CourierCompanies.splice(i, 1);
                }
                if ($scope.CourierCompanies[i].Value === "Hermes") {
                    $scope.CourierCompanies.splice(i, 1);
                }
                if ($scope.CourierCompanies[i].Value === "UKMail") {
                    $scope.CourierCompanies.splice(i, 1);
                }
            }
        }
        $scope.CourierCompany = $scope.CourierCompanies[0];
    };

    $scope.AdvanceRateCardByShipmentType = function (DocType) {
        //  AppSpinner.showSpinnerTemplate("Template...", $scope.Template);
        if (DocType !== undefined) {
            $scope.DocType = DocType;
        }

        //$scope.IsChanged = true;
        //    getZoneBaseRateCard();
    };

    $scope.ZoneCountryByCourierCompany = function () {

        if ($scope.CourierCompany.Value === "Yodel") {
            $scope.showYodelRate = true;
            $scope.showLogisticServiceType = true;
            $scope.showPakageType = false;
            $scope.showParcelServiceType = false;
            $scope.showRateExportImport = false;
            //getScreenInitialsOnLogisticType();
            //getZoneBaseRateCardLimit();
        }
        else if ($scope.CourierCompany.Value === "UKMail") {
            $scope.showYodelRate = false;
            $scope.showLogisticServiceType = true;
            $scope.showPakageType = true;
            $scope.showParcelServiceType = true;
            $scope.showRateExportImport = false;
            //getScreenInitialsOnLogisticType();
            //getZoneBaseRateCardLimit();
        }
        else if ($scope.CourierCompany.Value === "Hermes") {
            $scope.showYodelRate = false;
            $scope.showLogisticServiceType = true;
            $scope.showPakageType = false;
            $scope.showParcelServiceType = false;
            $scope.showRateExportImport = false;
            //getScreenInitialsOnLogisticType();
            //getZoneBaseRateCardLimit();

        }
        else if ($scope.CourierCompany.Value === "DHL") {
            $scope.showYodelRate = false;
            $scope.showLogisticServiceType = true;
            $scope.showPakageType = false;
            $scope.showParcelServiceType = false;
            $scope.showRateExportImport = false;
            //getScreenInitialsOnLogisticType();
            //getZoneBaseRateCardLimit();

        }
        else if ($scope.CourierCompany.Value === "FedEx") {
            $scope.showYodelRate = false;
            $scope.showLogisticServiceType = true;
            $scope.showPakageType = false;
            $scope.showParcelServiceType = false;
            $scope.showRateExportImport = false;
            //getScreenInitialsOnLogisticType();
            //getZoneBaseRateCardLimit();

        }
        else if ($scope.CourierCompany.Value === "TNT") {
            $scope.showYodelRate = false;
            $scope.showLogisticServiceType = true;
            $scope.showPakageType = false;
            $scope.showParcelServiceType = false;
            $scope.showRateExportImport = false;
            //getScreenInitialsOnLogisticType();
            //getZoneBaseRateCardLimit();

        }
    };

    $scope.ZoneRateByRateType = function (RateType) {
        $scope.RateType = RateType;
        $scope.IsChanged = true;
    };

    $scope.BaseRateCardByPakageType = function (PakageType) {
        //  AppSpinner.showSpinnerTemplate("Template...", $scope.Template);
        $scope.PakageType = PakageType;
        $scope.IsChanged = true;
        //getWeights();
        //getZoneBaseRateCardLimit();
    };
    $scope.BaseRateCardByParcelServiceType = function (ParcelServiceType) {
        //AppSpinner.showSpinnerTemplate("Template...", $scope.Template);
        $scope.ParcelServiceType = ParcelServiceType;
        $scope.IsChanged = true;
        //getWeights();
        //getZoneBaseRateCardLimit();
    };

    var getLogisticItems = function () {
        CustomerService.GetLogisticItems($scope.OperationZone.OperationZoneId).then(function (response) {
            $scope.LogisticItems = response.data;
            angular.copy($scope.LogisticItems, $scope.masterData);
            $scope.CourierCompanies = response.data.LogisticCompanies;
            $scope.RateTypes = response.data.LogisticRateTypes;
            $scope.LogisticTypes = response.data.LogisticTypes;
            $scope.LogisticType = $scope.LogisticTypes[1];
            $scope.CourierCompany = $scope.CourierCompanies[0];
            $scope.RateType = $scope.RateTypes[0];

        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });


    };

    var importExportThirdPartyJson = function () {

        $scope.finalArrayList = [];
        $scope.finalArrayListDoc = [];
        $scope.finalArrayListNonDoc = [];
        $scope.finalArrayListDocNonDoc = [];
        $scope.finalArrayListHeavyWeight = [];

        var DocTypeObject = {
            Doc: [],
            NonDoc: [],
            DocNonDoc: [],
            HeavyWeight: []
        };

        for (i = 0; i < $scope.ImportExprtThirdPartyRawData.length; i++) {
            if ($scope.ImportExprtThirdPartyRawData[i].shipmentType.LogisticDescription === 'Doc') {
                DocTypeObject.Doc.push($scope.ImportExprtThirdPartyRawData[i]);
            }
            if ($scope.ImportExprtThirdPartyRawData[i].shipmentType.LogisticDescription === 'Nondoc') {
                DocTypeObject.NonDoc.push($scope.ImportExprtThirdPartyRawData[i]);
            }
            if ($scope.ImportExprtThirdPartyRawData[i].shipmentType.LogisticDescription === 'Doc&Nondoc') {
                DocTypeObject.DocNonDoc.push($scope.ImportExprtThirdPartyRawData[i]);
            }
            if ($scope.ImportExprtThirdPartyRawData[i].shipmentType.LogisticDescription === 'HeavyWeight') {
                DocTypeObject.HeavyWeight.push($scope.ImportExprtThirdPartyRawData[i]);
            }
        }

        var FinalData = {
            DocType: '',
            WeightFrom: '',
            WeightTo: '',
            MarginCostRate: []
        };
        for (var type in DocTypeObject) {
            $scope.newArray = DocTypeObject[type];
            if ($scope.newArray.length > 0) {
                FinalData = {
                    DocType: '',
                    WeightFrom: '',
                    WeightTo: '',
                    MarginCostRate: []
                };



                for (i = 0; i < $scope.LogisticWeight.length; i++) {
                    for (j = 0; j < $scope.newArray.length; j++) {
                        if ($scope.LogisticWeight[i].ShipmentType.LogisticDescription === $scope.newArray[j].shipmentType.LogisticDescription &&
                            $scope.LogisticWeight[i].WeightFrom === $scope.newArray[j].LogisticWeight.WeightFrom) {
                            FinalData.DocType = $scope.newArray[j].shipmentType.LogisticDescriptionDisplay;
                            FinalData.DocTypeR = $scope.newArray[j].shipmentType.LogisticDescription;
                            FinalData.WeightFrom = $scope.newArray[j].LogisticWeight.WeightFrom;
                            FinalData.WeightTo = $scope.newArray[j].LogisticWeight.WeightTo;
                            FinalData.MarginCostRate.push($scope.newArray[j]);
                        }

                    }
                    if (FinalData.MarginCostRate.length > 0) {
                        $scope.finalArrayList.push(FinalData);
                    }
                    if (FinalData.DocTypeR === "Doc" && FinalData.MarginCostRate.length > 0) {
                        $scope.finalArrayListDoc.push(FinalData);
                    } if (FinalData.DocTypeR === "Nondoc" && FinalData.MarginCostRate.length > 0) {
                        $scope.finalArrayListNonDoc.push(FinalData);
                    } if (FinalData.DocTypeR === "Doc&Nondoc" && FinalData.MarginCostRate.length > 0) {
                        $scope.finalArrayListDocNonDoc.push(FinalData);
                    } if (FinalData.DocTypeR === "HeavyWeight" && FinalData.MarginCostRate.length > 0) {
                        $scope.finalArrayListHeavyWeight.push(FinalData);
                    }
                    FinalData = {
                        DocType: '',
                        WeightFrom: '',
                        WeightTo: '',
                        MarginCostRate: []
                    };
                }
            }
            $scope.IsShowImportExportThirdParty = true;
        }
    };
    var uKShipmentParcelJson = function () {
        $scope.showRateUKParcel = false;
        $scope.finalUkShipmentListWithCA = [];
        $scope.finalUkShipmentListWithOutCA = [];
        $scope.finalArrayList = [];
        var flag = false;
        // Craete Json
        // var finalArrayList = [];
        var array = {
            NWD: [],
            NWDN: [],
            NWD1030: [],
            NWD0900: [],
            SAT: [],
            SAT1030: [],
            SAT0900: []
        };

        for (var j = 0 ; j < $scope.ZoneBaseRateCardList.length ; j++) {
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "NWD") {
                array.NWD.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "NWDN") {
                array.NWDN.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "NWD1030") {
                array.NWD1030.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "NWD0900") {
                array.NWD0900.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "SAT") {
                array.SAT.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "SAT1030") {
                array.SAT1030.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "SAT0900") {
                array.SAT0900.push($scope.ZoneBaseRateCardList[j]);
            }
        }
        for (var type in array) {
            var newArray = array[type];
            if (newArray.length > 0) {
                var finalJson = {
                    DocType: "",
                    IsRowShow: true,
                    WeightFrom: 0,
                    WeightTo: 0,
                    Rows: []
                };

                for (var w = 0 ; w < $scope.LogisticWeight.length; w++) {
                    for (var k = 0 ; k < newArray.length ; k++) {
                        if ($scope.LogisticWeight[w].ShipmentType.LogisticDescription === newArray[k].shipmentType.LogisticDescription &&
                            $scope.LogisticWeight[w].WeightFrom === newArray[k].LogisticWeight.WeightFrom &&
                            $scope.LogisticWeight[w].WeightFrom !== newArray[k].LogisticWeight.WeightLimit
                            ) {

                            finalJson.DocType = newArray[k].shipmentType.LogisticDescriptionDisplay;
                            finalJson.WeightFrom = newArray[k].LogisticWeight.WeightFrom;
                            finalJson.WeightTo = newArray[k].LogisticWeight.WeightTo;
                            newArray[k].IsCellShow = true;
                            finalJson.Rows.push(newArray[k]);
                        }

                    }
                    if (finalJson.Rows.length > 0) {
                        $scope.finalArrayList.push(finalJson);
                    }

                    finalJson = {
                        DocType: "",
                        IsRowShow: true,
                        WeightFrom: 0,
                        WeightTo: 0,
                        WeightUnit: '',
                        Rows: []
                    };
                }
                for (var m = 0 ; m < $scope.LogisticWeight.length; m++) {
                    for (var l = 0 ; l < newArray.length ; l++) {
                        if ($scope.LogisticWeight[m].ShipmentType.LogisticDescription === newArray[l].shipmentType.LogisticDescription && $scope.LogisticWeight[m].WeightFrom === newArray[l].LogisticWeight.WeightFrom && $scope.LogisticWeight[m].WeightFrom === newArray[l].LogisticWeight.WeightLimit) {
                            finalJson.DocType = newArray[l].shipmentType.LogisticDescriptionDisplay;
                            finalJson.WeightFrom = newArray[l].LogisticWeight.WeightFrom;
                            finalJson.WeightTo = newArray[l].LogisticWeight.WeightTo;
                            finalJson.WeightUnit = newArray[l].LogisticWeight.WeightUnit;
                            finalJson.Rows.push(newArray[l]);
                        }

                    }
                    if (finalJson.Rows.length > 0) {
                        $scope.finalUkShipmentListWithOutCA.push(finalJson);
                    }

                    finalJson = {
                        DocType: "",
                        IsRowShow: true,
                        Weight: 0,
                        Rows: []
                    };
                }
            }

        }
        $scope.showRateUKParcel = true;
        $scope.UkShipmentParcel = true;
        $scope.UkShipmentBagit = false;

    };
    var uKShipmentBagitJson = function () {
        $scope.showRateUKBagit = false;
        $scope.finalbagitUkShipmentList = [];
        $scope.finalUkShipmentListSSPP = [];
        $scope.finalArrayList = [];
        var flag = false;
        // Craete Json
        // var finalArrayList = [];
        var array = {
            NWD: [],
            NWDN: [],
            NWD1030: [],
            NWD0900: [],
            SAT: [],
            SAT1030: [],
            SAT0900: [],
            SSPP: []
        };

        for (var j = 0 ; j < $scope.ZoneBaseRateCardList.length ; j++) {
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "NWD") {
                array.NWD.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "NWDN") {
                array.NWDN.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "NWD1030") {
                array.NWD1030.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "NWD0900") {
                array.NWD0900.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "SAT") {
                array.SAT.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "SAT1030") {
                array.SAT1030.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "SAT0900") {
                array.SAT0900.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "SSPP") {
                array.SSPP.push($scope.ZoneBaseRateCardList[j]);
            }
        }
        for (var type in array) {
            var newArray = array[type];
            if (newArray.length > 0) {
                var finalJson = {
                    DocType: "",
                    IsRowShow: true,
                    WeightFrom: 0,
                    WeightTo: 0,
                    WeightLimit: 0,
                    WeightUnit: '',
                    Rows: []
                };

                for (var w = 0 ; w < $scope.LogisticWeight.length; w++) {
                    for (var k = 0 ; k < newArray.length ; k++) {
                        if ($scope.LogisticWeight[w].ShipmentType.LogisticDescription === newArray[k].shipmentType.LogisticDescription && $scope.LogisticWeight[w].WeightFrom === newArray[k].LogisticWeight.WeightFrom) {
                            finalJson.DocType = newArray[k].shipmentType.LogisticDescriptionDisplay;
                            finalJson.DocTypeR = newArray[k].shipmentType.LogisticDescription;
                            finalJson.WeightFrom = newArray[k].LogisticWeight.WeightFrom;
                            finalJson.WeightTo = newArray[k].LogisticWeight.WeightTo;
                            finalJson.WeightLimit = newArray[k].LogisticWeight.WeightLimit;
                            finalJson.WeightUnit = newArray[k].LogisticWeight.WeightUnit;
                            newArray[k].IsCellShow = true;
                            finalJson.Rows.push(newArray[k]);
                        }

                    }
                    if (finalJson.DocTypeR !== "SSPP" && finalJson.Rows.length > 0) {
                        $scope.finalArrayList.push(finalJson);
                    }
                    //if (finalJson.DocTypeR === "SSPP" && finalJson.Rows.length > 0) {
                    //    $scope.finalUkShipmentListSSPP.push(finalJson);
                    //}

                    finalJson = {
                        DocType: "",
                        IsRowShow: true,
                        Weight: 0,
                        Rows: []
                    };
                }
            }

        }
        $scope.showRateUKBagit = true;
        $scope.UkShipmentParcel = false;
        $scope.UkShipmentBagit = true;


    };

    var eUImportExportJson = function () {
        $scope.showRateEUExportImort = false;
        $scope.finalArrayListWithoutCA = [];
        $scope.finalArrayList = [];
        var newArray = $scope.ZoneBaseRateCardList;

        var finalJson = {
            IsRowShow: true,
            WeightFrom: 0,
            WeightTo: 0,
            WeightUnit: '',
            UnitOfMeasurement: '',
            Rows: []
        };

        for (var w = 0 ; w < $scope.LogisticWeight.length; w++) {
            for (var k = 0 ; k < newArray.length ; k++) {
                if ($scope.LogisticWeight[w].WeightFrom === newArray[k].LogisticWeight.WeightFrom && $scope.LogisticWeight[w].WeightTo === newArray[k].LogisticWeight.WeightTo) {
                    finalJson.DocType = newArray[k].shipmentType.LogisticDescriptionDisplayType;
                    finalJson.WeightFrom = newArray[k].LogisticWeight.WeightFrom;
                    finalJson.WeightTo = newArray[k].LogisticWeight.WeightTo;
                    finalJson.WeightUnit = newArray[k].LogisticWeight.WeightUnit;
                    finalJson.UnitOfMeasurement = newArray[k].LogisticWeight.UnitOfMeasurement;
                    newArray[k].IsCellShow = true;
                    finalJson.Rows.push(newArray[k]);
                }

            }
            if (finalJson.Rows.length > 0 && finalJson.WeightTo !== 999) {
                $scope.finalArrayList.push(finalJson);
            }
            else if (finalJson.Rows.length > 0 && finalJson.WeightTo === 999) {
                $scope.finalArrayListWithoutCA.push(finalJson);
            }
            finalJson = {
                DocType: "",
                IsRowShow: true,
                Weight: 0,
                Rows: []
            };
        }
        $scope.showRateEUExportImort = true;
    };
    var uKShipmentYodelJson = function () {
        $scope.finalArrayList = [];
        var flag = false;
        var array = {
            EXP24: [],
            EXP48: [],
            EXPNI: [],
            HOM72: [],
            HOM72IN: [],
            EXP72: [],
            PRI12MF: [],
            SATPRI12: [],
            EXPSAT: []
        };

        for (var j = 0 ; j < $scope.ZoneBaseRateCardList.length ; j++) {
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "EXP24") {
                array.EXP24.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "EXP48") {
                array.EXP48.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "EXPNI") {
                array.EXPNI.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "HOM72") {
                array.HOM72.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "HOM72IN") {
                array.HOM72IN.push($scope.ZoneBaseRateCardList[j]);
            }

            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "PRI12MF") {
                array.PRI12MF.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "SATPRI12") {
                array.SATPRI12.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "EXPSAT") {
                array.EXP72.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "EXP72") {
                array.EXP72.push($scope.ZoneBaseRateCardList[j]);
            }
        }
        for (var type in array) {
            var newArray = array[type];
            if (newArray.length > 0) {
                var finalJson = {
                    DocType: "",
                    IsRowShow: true,
                    WeightFrom: 0,
                    WeightTo: 0,
                    WeightLimit: 0,
                    WeightUnit: '',
                    Rows: []
                };

                for (var w = 0 ; w < $scope.LogisticWeight.length; w++) {
                    for (var k = 0 ; k < newArray.length ; k++) {
                        if ($scope.LogisticWeight[w].ShipmentType.LogisticDescription === newArray[k].shipmentType.LogisticDescription && $scope.LogisticWeight[w].WeightFrom === newArray[k].LogisticWeight.WeightFrom) {
                            finalJson.DocType = newArray[k].shipmentType.LogisticDescriptionDisplay;
                            finalJson.WeightFrom = newArray[k].LogisticWeight.WeightFrom;
                            finalJson.WeightTo = newArray[k].LogisticWeight.WeightTo;
                            finalJson.WeightLimit = newArray[k].LogisticWeight.WeightLimit;
                            finalJson.WeightUnit = newArray[k].LogisticWeight.WeightUnit;
                            newArray[k].IsCellShow = true;
                            finalJson.Rows.push(newArray[k]);
                        }

                    }
                    if (finalJson.Rows.length > 0) {
                        $scope.finalArrayList.push(finalJson);
                    }
                    finalJson = {
                        DocType: "",
                        IsRowShow: true,
                        Weight: 0,
                        Rows: []
                    };
                }
            }

        }
    };
    var uKShipmentHermesJson = function () {
        $scope.finalArrayList = [];
        var flag = false;
        var array = {
            Packet: [],
            Parcel: []
        };

        for (var j = 0 ; j < $scope.ZoneBaseRateCardList.length ; j++) {
            if ($scope.ZoneBaseRateCardList[j].LogisticWeight.ParcelType === "Parcel") {
                array.Parcel.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].LogisticWeight.ParcelType === "Packet") {
                array.Packet.push($scope.ZoneBaseRateCardList[j]);
            }

        }
        for (var type in array) {
            var newArray = array[type];
            if (newArray.length > 0) {
                var finalJson = {
                    DocType: "",
                    IsRowShow: true,
                    WeightFrom: 0,
                    WeightTo: 0,
                    WeightLimit: 0,
                    WeightUnit: '',
                    Rows: []
                };

                for (var w = 0 ; w < $scope.LogisticWeight.length; w++) {
                    for (var k = 0 ; k < newArray.length ; k++) {
                        if ($scope.LogisticWeight[w].ParcelType === newArray[k].LogisticWeight.ParcelType && $scope.LogisticWeight[w].WeightFrom === newArray[k].LogisticWeight.WeightFrom) {
                            finalJson.DocType = newArray[k].LogisticWeight.ParcelType;
                            finalJson.WeightFrom = newArray[k].LogisticWeight.WeightFrom;
                            finalJson.WeightTo = newArray[k].LogisticWeight.WeightTo;
                            finalJson.WeightLimit = newArray[k].LogisticWeight.WeightLimit;
                            finalJson.WeightUnit = newArray[k].LogisticWeight.WeightUnit;
                            newArray[k].IsCellShow = true;
                            finalJson.Rows.push(newArray[k]);
                        }

                    }
                    if (finalJson.Rows.length > 0) {
                        $scope.finalArrayList.push(finalJson);
                    }
                    finalJson = {
                        DocType: "",
                        IsRowShow: true,
                        Weight: 0,
                        Rows: []
                    };
                }
            }

        }
    };
    $scope.CustomerMarginSaveTO = function (Rate) {
        var rate = Rate;
    };
    var ukShipmentDHLJson = function () {

        $scope.finalArrayList = [];
        $scope.finalArrayListDoc = [];
        $scope.finalArrayListNonDoc = [];
        $scope.finalArrayListDocNonDoc = [];
        $scope.finalArrayListHeavyWeight = [];
        var flag = false;
        // Craete Json
        // var finalArrayList = [];
        var array = {
            Doc: [],
            NonDoc: [],
            DocAndNonDoc: [],
            HeavyWeight: []
        };

        for (var j = 0 ; j < $scope.ZoneBaseRateCardList.length ; j++) {
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "Doc") {
                array.Doc.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "Nondoc") {
                array.NonDoc.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "Doc&Nondoc") {
                array.DocAndNonDoc.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "HeavyWeight") {
                array.HeavyWeight.push($scope.ZoneBaseRateCardList[j]);
            }
        }

        for (var type in array) {
            var newArray = array[type];
            if (newArray.length > 0) {
                var finalJson = {
                    DocType: "",
                    IsRowShow: true,
                    WeightFrom: 0,
                    WeightTo: 0,
                    Rows: []
                };

                for (var w = 0 ; w < $scope.LogisticWeight.length; w++) {
                    for (var k = 0 ; k < newArray.length ; k++) {
                        if ($scope.LogisticWeight[w].ShipmentType.LogisticDescription === newArray[k].shipmentType.LogisticDescription && $scope.LogisticWeight[w].WeightFrom === newArray[k].LogisticWeight.WeightFrom) {
                            finalJson.DocType = newArray[k].shipmentType.LogisticDescriptionDisplay;
                            finalJson.DocTypeR = newArray[k].shipmentType.LogisticDescription;
                            finalJson.WeightFrom = newArray[k].LogisticWeight.WeightFrom;
                            finalJson.WeightTo = newArray[k].LogisticWeight.WeightTo;
                            newArray[k].IsCellShow = true;
                            finalJson.Rows.push(newArray[k]);
                        }

                    }

                    if (finalJson.Rows.length > 0) {
                        $scope.finalArrayList.push(finalJson);
                    }

                    if (finalJson.DocTypeR === "Doc" && finalJson.Rows.length > 0) {
                        $scope.finalArrayListDoc.push(finalJson);
                        //$scope.finalArrayList.push(finalJson);
                    }
                    else if (finalJson.DocTypeR === "Nondoc" && finalJson.Rows.length > 0) {
                        $scope.finalArrayListNonDoc.push(finalJson);
                        //$scope.finalArrayList.push(finalJson);
                    }
                    else if (finalJson.DocTypeR === "Doc&Nondoc" && finalJson.Rows.length > 0) {
                        $scope.finalArrayListDocNonDoc.push(finalJson);
                        //$scope.finalArrayList.
                    }
                    else if (finalJson.DocTypeR === "HeavyWeight" && finalJson.Rows.length > 0) {
                        $scope.finalArrayListHeavyWeight.push(finalJson);
                        //$scope.finalArrayList.
                    }
                    finalJson = {
                        DocType: "",
                        IsRowShow: true,
                        Weight: 0,
                        Rows: []
                    };
                }

            }

        }
        $scope.showRateExportImport = true;

    };

    $scope.AdvanceRatechangeSave = function (Rate) {
        Rate.isValue = true;

    };

    $scope.SaveAdvanceRateCard = function () {

        var FinalAdvanceRateObj = [];
        var FinalObj = {
            CustomerAdvanceMarginCostId: 0,
            AdvancePercentage: 0
        };
        if ($scope.LogisticType !== undefined) {
            LogisticType = $scope.LogisticType.Value;
        }
        if ($scope.CourierCompany !== undefined) {
            CourierCompany = $scope.CourierCompany.Value;
        }
        if (($scope.LogisticType.Value === 'Import' || $scope.LogisticType.Value === 'Export' || $scope.LogisticType.Value === 'ThirdParty') && $scope.CourierCompany.Value === 'DHL') {
            for (i = 0; i < $scope.finalArrayList.length; i++) {
                for (j = 0; j < $scope.finalArrayList[i].MarginCostRate.length; j++) {
                    if ($scope.finalArrayList[i].MarginCostRate[j].isValue !== undefined && $scope.finalArrayList[i].MarginCostRate[j].isValue === true) {
                        FinalObj.CustomerAdvanceMarginCostId = $scope.finalArrayList[i].MarginCostRate[j].CustomerAdvanceMarginCostId;
                        FinalObj.AdvancePercentage = $scope.finalArrayList[i].MarginCostRate[j].Rate;
                    }

                    if (FinalObj.CustomerAdvanceMarginCostId > 0) {
                        FinalAdvanceRateObj.push(FinalObj);
                    }
                    FinalObj = {
                        CustomerAdvanceMarginCostId: 0,
                        AdvancePercentage: 0
                    };
                }
            }
        }
        else if ($scope.LogisticType.Value === 'UKShipment' && $scope.CourierCompany.Value === 'UKMail') {
            for (ii = 0; ii < $scope.finalArrayList.length; ii++) {
                for (jj = 0; jj < $scope.finalArrayList[ii].Rows.length; jj++) {
                    if ($scope.finalArrayList[ii].Rows[jj].isValue !== undefined && $scope.finalArrayList[ii].Rows[jj].isValue === true) {
                        FinalObj.CustomerAdvanceMarginCostId = $scope.finalArrayList[ii].Rows[jj].CustomerAdvanceMarginCostId;
                        FinalObj.AdvancePercentage = $scope.finalArrayList[ii].Rows[jj].Rate;
                    }

                    if (FinalObj.CustomerAdvanceMarginCostId > 0) {
                        FinalAdvanceRateObj.push(FinalObj);
                    }
                    FinalObj = {
                        CustomerAdvanceMarginCostId: 0,
                        AdvancePercentage: 0
                    };
                }
            }
            for (ii = 0; ii < $scope.finalUkShipmentListWithOutCA.length; ii++) {
                for (jj = 0; jj < $scope.finalUkShipmentListWithOutCA[ii].Rows.length; jj++) {
                    if ($scope.finalUkShipmentListWithOutCA[ii].Rows[jj].isValue !== undefined && $scope.finalUkShipmentListWithOutCA[ii].Rows[jj].isValue === true) {
                        FinalObj.CustomerAdvanceMarginCostId = $scope.finalUkShipmentListWithOutCA[ii].Rows[jj].CustomerAdvanceMarginCostId;
                        FinalObj.AdvancePercentage = $scope.finalUkShipmentListWithOutCA[ii].Rows[jj].Rate;
                    }

                    if (FinalObj.CustomerAdvanceMarginCostId > 0) {
                        FinalAdvanceRateObj.push(FinalObj);
                    }
                    FinalObj = {
                        CustomerAdvanceMarginCostId: 0,
                        AdvancePercentage: 0
                    };
                }
            }
        }
        else {
            for (ii = 0; ii < $scope.finalArrayList.length; ii++) {
                for (jj = 0; jj < $scope.finalArrayList[ii].Rows.length; jj++) {
                    if ($scope.finalArrayList[ii].Rows[jj].isValue !== undefined && $scope.finalArrayList[ii].Rows[jj].isValue === true) {
                        FinalObj.CustomerAdvanceMarginCostId = $scope.finalArrayList[ii].Rows[jj].CustomerAdvanceMarginCostId;
                        FinalObj.AdvancePercentage = $scope.finalArrayList[ii].Rows[jj].Rate;
                    }

                    if (FinalObj.CustomerAdvanceMarginCostId > 0) {
                        FinalAdvanceRateObj.push(FinalObj);
                    }
                    FinalObj = {
                        CustomerAdvanceMarginCostId: 0,
                        AdvancePercentage: 0
                    };
                }
            }
        }

        CustomerService.SaveCustomerAdvanceMarginCost(FinalAdvanceRateObj).then(function (response) {
            if (response.status === 200) {
                //getScreenInitials();
                $scope.SearchAdvanceRateCard();
                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteInformation,
                    body: $scope.TextSuccessfullySavedInformation,
                    showCloseButton: true
                });
            }
        }, function () {

            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });
        //$scope.rate = Rate;
    };

    function init() {
        $scope.masterData = {};
        CustomerService.GetCurrentOperationZones().then(function (response) {
            $scope.currentOperationZone = response.data;
            if ($scope.currentOperationZone) {
                getScreenInitials();
            }
        });
        $scope.LogisticTypes = [
        {
            LogisticTypeId: 1,
            LogisticName: 'Import',
            LogisticNameDisplay: 'Import'

        },
        {
            LogisticTypeId: 2,
            LogisticName: 'Export',
            LogisticNameDisplay: 'Export'
        },
        {
            LogisticTypeId: 3,
            LogisticName: 'ThirdParty',
            LogisticNameDisplay: 'ThirdParty'

        }
        ];
        $scope.LogisticType = {
            LogisticTypeId: 2,
            LogisticName: 'Export',
            LogisticNameDisplay: 'Export'
        };
        $scope.PakageTypes = [
           {
               PakageTypeId: 1,
               PakageType: 'Single'
           },
           {
               PakageTypeId: 2,
               PakageType: 'Multiple'
           }
        ];

        $scope.ParcelServiceTypes = [
       {
           ParcelServiceTypeId: 1,
           ParcelServiceType: 'Parcel'
       },
       {
           ParcelServiceTypeId: 2,
           ParcelServiceType: 'BagItService'
       }
        ];

        $scope.LogisticService = {
            LogisticServiceTypeId: 1,
            LogisticServiceName: 'Uk Mail',
            LogisticServiceNameDisplay: 'UK Mail'
        };

        $scope.LogisticServiceTypes = [
     {
         LogisticServiceTypeId: 1,
         LogisticServiceName: 'Uk Mail',
         LogisticServiceNameDisplay: 'UK Mail'
     },
     {
         LogisticServiceTypeId: 2,
         LogisticServiceName: 'Yodel',
         LogisticServiceNameDisplay: 'Yodel'
     },
     {
         LogisticServiceTypeId: 3,
         LogisticServiceName: 'Hermes',
         LogisticServiceNameDisplay: 'Hermes'
     }
        ];

        $scope.CourierCompanies = [
    {
        CourierCompanyId: 1,
        CourierCompany: 'DHL',
        CourierCompanyDisplay: 'DHL'
    },
    {
        CourierCompanyId: 2,
        CourierCompany: 'FedEx',
        CourierCompanyDisplay: 'FedEx'
    },
    {
        CourierCompanyId: 3,
        CourierCompany: 'TNT',
        CourierCompanyDisplay: 'TNT'
    }
        ];
        $scope.CourierCompany = {
            CourierCompanyId: 1,
            CourierCompany: 'DHL',
            CourierCompanyDisplay: 'DHL'
        };

        $scope.RateTypes = [
       {
           RateTypeId: 1,
           RateType: 'Express',
           RateTypeDisplay: 'Express'
       },
       {
           RateTypeId: 2,
           RateType: 'Economy',
           RateTypeDisplay: 'Economy'
       }
        ];
        $scope.RateType = {
            RateTypeId: 1,
            RateType: 'Express',
            RateTypeDisplay: 'Express'
        };

        $scope.PodType = {
            PodTypeId: 1,
            PodTypeName: "POD"
        };
        $scope.PodTypes = [{
            PodTypeId: 1,
            PodTypeName: "POD",
            PodTypeNameDisplay: "POD"
        }, {
            PodTypeId: 2,
            PodTypeName: "NONPOD",
            PodTypeNameDisplay: "NON POD"
        }];
        $scope.OperationZone = { OperationZoneId: 1, OperationZoneName: 'HKG' };
        $scope.CustomerId = $state.params.customerId;
        setModalOptions();
    }

    init();
});