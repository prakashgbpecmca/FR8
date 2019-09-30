angular.module('ngApp.baseRateCard').controller('BaseRateCardController', function ($scope, AppSpinner, $filter, $state, toaster, $translate, $uibModal, ZoneBaseRateCardService, $window) {

    //Set Multilingual for Modal Popup
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

    var getLogisticItems = function () {
        ZoneBaseRateCardService.GetLogisticItemList($scope.OperationZone.OperationZoneId).then(function (response) {
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

    //ViewBaseRateCard Modal popup

    $scope.ViewBaseRateCard = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            controller: 'ViewBaseRateCardController',
            templateUrl: 'zoneSetting/baseRateCard/baseRateCardHistory/baseRateCardHistory.tpl.html',
            backdrop: 'static',
            size: 'lg'

        });
    };

    $scope.ZoneRateByRateType = function (RateType) {
        $scope.RateType = RateType;
        $scope.IsChanged = true;
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
    $scope.BaseRateCardByZone = function () {

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
    $scope.BaseRateCardByLogisticType = function () {
        //   AppSpinner.showSpinnerTemplate("Template...", $scope.Template);
        setCourierCompanyAndRateTypeDropDown();

        if ($scope.LogisticType.Value === "UKShipment") {
            //$scope.LogisticService = {
            //    LogisticServiceTypeId: 1,
            //    LogisticServiceName: 'Uk Mail',
            //    LogisticServiceNameDisplay: 'UK Mail'
            //};
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
        $scope.showRateExportImport = false;

        if ($scope.CourierCompany !== null && $scope.CourierCompany.Value !== "UKMail") {
            $scope.showParcelServiceType = false;
            $scope.showPakageType = false;
        }
        else {
            $scope.showParcelServiceType = true;
            $scope.showPakageType = true;
        }
        //    getScreenInitialsOnLogisticType();
        //  getZoneBaseRateCardLimit();
    };
    $scope.setWeightLimit = function (RateLimit) {
        $scope.BaseRateLimitJson.push(RateLimit);
    };
    $scope.BaseRateCardByParcelServiceType = function (ParcelServiceType) {
        //AppSpinner.showSpinnerTemplate("Template...", $scope.Template);
        $scope.ParcelServiceType = ParcelServiceType;
        $scope.IsChanged = true;
        //getWeights();
        //getZoneBaseRateCardLimit();
    };
    $scope.BaseRateCardByPakageType = function (PakageType) {
        //  AppSpinner.showSpinnerTemplate("Template...", $scope.Template);
        $scope.PakageType = PakageType;
        $scope.IsChanged = true;
        //getWeights();
        //getZoneBaseRateCardLimit();
    };
    $scope.BaseRateCardByShipmentType = function (DocType) {
        //  AppSpinner.showSpinnerTemplate("Template...", $scope.Template);
        if (DocType !== undefined) {
            $scope.DocType = DocType;
        }

        $scope.IsChanged = true;
        //    getZoneBaseRateCard();
    };
    $scope.BaseRateCardByLogisticServiceType = function (LogisticServiceType) {
        AppSpinner.showSpinnerTemplate($scope.spinnerMessage, $scope.Template);
        if ($scope.LogisticService.LogisticServiceName === "Yodel") {
            $scope.showLogisticServiceType = true;
            $scope.showPakageType = false;
            $scope.showParcelServiceType = false;
            $scope.showRateExportImport = false;
            getScreenInitialsOnLogisticType();
            getZoneBaseRateCardLimit();
        }
        else if ($scope.LogisticService.LogisticServiceName === "Uk Mail") {
            $scope.showLogisticServiceType = true;
            $scope.showPakageType = true;
            $scope.showParcelServiceType = true;
            $scope.showRateExportImport = false;
            getScreenInitialsOnLogisticType();
            getZoneBaseRateCardLimit();
        }
        else if ($scope.LogisticService.LogisticServiceName === "Hermes") {
            $scope.showLogisticServiceType = true;
            $scope.showPakageType = false;
            $scope.showParcelServiceType = false;
            $scope.showRateExportImport = false;
            getScreenInitialsOnLogisticType();
            getZoneBaseRateCardLimit();

        }
    };
    $scope.BaseRateCardByPodType = function () {
        //AppSpinner.showSpinnerTemplate("Template...", $scope.Template);
        //getScreenInitialsOnLogisticType();
        //getZoneBaseRateCardLimit();
    };
    var getZoneBaseRateCardLimit = function () {
        $scope.BaseRateLimitJson = [];
        var pakageType = "";
        var parcelServiceType = "";
        var LogisticServiceType = "";
        if ($scope.LogisticService !== undefined && $scope.LogisticService.LogisticServiceName !== 'Hermes' && $scope.PakageType !== undefined) {
            pakageType = $scope.PakageType.PakageType;
        }
        if ($scope.ParcelServiceType !== undefined) {
            parcelServiceType = $scope.ParcelServiceType.ParcelServiceType;
        }
        if ($scope.LogisticService !== undefined) {
            LogisticServiceType = $scope.LogisticService.LogisticServiceName;
        }
        if ($scope.LogisticService !== undefined && $scope.LogisticService.LogisticServiceName === 'Hermes' && $scope.PodType !== undefined) {
            pakageType = $scope.PodType.PodTypeName;
        }
        ZoneBaseRateCardService.GetZoneBaseRateCardLimit($scope.OperationZone.OperationZoneId, $scope.LogisticType.LogisticName, pakageType, parcelServiceType, LogisticServiceType).then(function (response) {
            $scope.BaseRateCardLimit = response.data;
            getDimensionBaseRateCard();
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });
    };
    var getDimensionBaseRateCard = function () {
        var LogisticServiceType = "";
        var pakageType = "";
        if ($scope.LogisticService !== undefined) {
            LogisticServiceType = $scope.LogisticService.LogisticServiceName;
        }
        if ($scope.LogisticService !== undefined && $scope.LogisticService.LogisticServiceName === 'Hermes' && $scope.PodType !== undefined) {
            pakageType = $scope.PodType.PodTypeName;
        }
        ZoneBaseRateCardService.GetDimensionBaseRateCard($scope.OperationZone.OperationZoneId, $scope.LogisticType.LogisticName, LogisticServiceType, "", pakageType).then(function (response) {
            $scope.BaseRateCardDimensionLimit = response.data;
            limitJsonForHermes();
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });
    };

    $scope.baseRateCardRateHermesWeightDimensionLimit = function (rateLimit, ParcelType) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'zoneSetting/baseRateCard/baseRateCardWeightDimensionLimitHermes.tpl.html',
            controller: 'BaseRateCardWeightDimensionLimitHermesController',
            windowClass: '',
            size: 'md',
            backdrop: 'static',
            resolve: {
                OperationZone: function () {
                    return $scope.OperationZone;
                },
                BaseRateCardWeightLimit: function () {
                    return $scope.BaseRateCardLimit;
                },
                ParcelType: function () {
                    if (ParcelType !== undefined) {
                        return ParcelType;
                    }
                    else {
                        return '';
                    }
                },
                BaseRateCardDimensionLimit: function () {
                    return $scope.BaseRateCardDimensionLimit;
                },
                RateLimit: function () {
                    if (rateLimit === undefined) {
                        return null;
                    }
                    else {
                        return rateLimit;
                    }

                }
            }
        });
        modalInstance.result.then(function (Rate) {
            getZoneBaseRateCardLimit();
        }, function () {
        });
    };

    var limitJsonForHermes = function () {
        $scope.overLimitHermesJson = [];
        if ($scope.LogisticService !== undefined && $scope.LogisticService.LogisticServiceName === "Hermes") {
            var obj = {
                ParcelType: "",
                Row: [],
                OverWeightDimensionRate: null
            };
            if ($scope.BaseRateCardDimensionLimit !== null && $scope.BaseRateCardDimensionLimit !== undefined) {
                for (var i = 0; i < $scope.BaseRateCardDimensionLimit.length; i++) {
                    if ($scope.BaseRateCardDimensionLimit[i].Parceltype === "Parcel") {
                        obj.ParcelType = $scope.BaseRateCardDimensionLimit[i].Parceltype;
                        obj.Row.push($scope.BaseRateCardDimensionLimit[i]);
                        obj.OverWeightDimensionRate = $scope.BaseRateCardDimensionLimit[i];
                    }
                }
            }
            if ($scope.BaseRateCardLimit !== null && $scope.BaseRateCardLimit !== undefined) {
                for (var a = 0; a < $scope.BaseRateCardLimit.length; a++) {
                    if ($scope.BaseRateCardLimit[a].ParcelType === "Parcel") {
                        obj.ParcelType = $scope.BaseRateCardLimit[a].ParcelType;
                        obj.Row.push($scope.BaseRateCardLimit[a]);
                        obj.OverWeightDimensionRate = $scope.BaseRateCardLimit[a];
                    }
                }
            }

            if (obj.Row.length) {
                $scope.overLimitHermesJson.push(obj);
            }
            obj = {
                ParcelType: "",
                Row: [],
                OverWeightDimensionRate: null
            };
            if ($scope.BaseRateCardDimensionLimit !== null && $scope.BaseRateCardDimensionLimit !== undefined) {
                for (var j = 0; j < $scope.BaseRateCardDimensionLimit.length; j++) {
                    if ($scope.BaseRateCardDimensionLimit[j].Parceltype === "Packet") {
                        obj.ParcelType = $scope.BaseRateCardDimensionLimit[j].Parceltype;
                        obj.Row.push($scope.BaseRateCardDimensionLimit[j]);
                        obj.OverWeightDimensionRate = $scope.BaseRateCardDimensionLimit[j];
                    }
                }
            }
            if ($scope.BaseRateCardLimit !== null && $scope.BaseRateCardLimit !== undefined) {
                for (var b = 0; b < $scope.BaseRateCardLimit.length; b++) {
                    if ($scope.BaseRateCardLimit[b].ParcelType === "Packet") {
                        obj.ParcelType = $scope.BaseRateCardLimit[b].ParcelType;
                        obj.Row.push($scope.BaseRateCardLimit[b]);
                        obj.OverWeightDimensionRate = $scope.BaseRateCardLimit[b];
                    }
                }
            }
            if (obj.Row.length) {
                $scope.overLimitHermesJson.push(obj);
            }
        }
    };

    $scope.SearchBaseRateCard = function () {
        getBaseRateCard();
    };
    var getBaseRateCard = function () {
        AppSpinner.showSpinnerTemplate($scope.spinnerMessage, $scope.Template);
        getZones();
        courierAccounts();

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

        if ($scope.RateType !== undefined && $scope.RateType !== null && $scope.CourierCompany.Value !== 'Hermes' &&
            $scope.CourierCompany.Value !== 'Yodel' &&
            $scope.CourierCompany.Value !== 'UKMail') {
            RateType = $scope.RateType.Value;
        }



        ZoneBaseRateCardService.GetShipmentType(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType).then(function (response) {
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
    var getScreenInitials = function () {
        AppSpinner.showSpinnerTemplate($scope.spinnerMessage, $scope.Template);


        ZoneBaseRateCardService.GetOperationZone().then(function (response) {
            $scope.OperationZones = response.data;
            $scope.OperationZone = response.data[0];

            ZoneBaseRateCardService.GetLogisticItemList($scope.OperationZone.OperationZoneId).then(function (response) {
                $scope.LogisticItems = response.data;
                angular.copy($scope.LogisticItems, $scope.masterData);
                $scope.CourierCompanies = response.data.LogisticCompanies;
                $scope.RateTypes = response.data.LogisticRateTypes;
                $scope.LogisticTypes = response.data.LogisticTypes;
                $scope.LogisticType = $scope.LogisticTypes[1];
                $scope.CourierCompany = $scope.CourierCompanies[0];
                $scope.RateType = $scope.RateTypes[0];
                $scope.ShipmentDocTypes = response.data.DocTypes;
                $scope.DocType = response.data.DocTypes[0];
                //
                getZones();
                courierAccounts();

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

                ZoneBaseRateCardService.GetShipmentType(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType).then(function (response) {
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
        getZones();
        courierAccounts();
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
        getCurrencyCodes();
        getWeights();
    };
    var getWeights = function () {

        // var logisticServiceType = '';
        $scope.finalArrayList = [];
        var OperationZoneId = 0;
        var LogisticType = "";
        var CourierCompany = "";
        var RateType = "";
        var PackageType = "";
        var ParcelType = "";
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

        if ($scope.RateType !== undefined && $scope.RateType !== null && $scope.CourierCompany.Value !== 'Hermes' &&
            $scope.CourierCompany.Value !== 'Yodel' &&
            $scope.CourierCompany.Value !== 'UKMail') {
            RateType = $scope.RateType.Value;
        }

        if ($scope.CourierCompany !== undefined && $scope.CourierCompany.Value !== 'Hermes' && $scope.CourierCompany.Value === 'UKMail' && $scope.PakageType !== undefined) {
            PackageType = $scope.PakageType.PakageType;
        }

        if ($scope.CourierCompany !== undefined && $scope.CourierCompany.Value === 'Hermes' && $scope.PodType !== undefined) {
            PackageType = $scope.PodType.PodTypeName;
        }

        if ($scope.ParcelServiceType !== undefined && $scope.CourierCompany !== undefined && $scope.CourierCompany.Value === 'UKMail') {
            ParcelType = $scope.ParcelServiceType.ParcelServiceType;
        }

        //if ($scope.LogisticService !== undefined) {
        //    logisticServiceType = $scope.LogisticService.LogisticServiceName;
        //}

        ZoneBaseRateCardService.GetWeight(OperationZoneId, LogisticType, CourierCompany, RateType, PackageType, ParcelType, ModuleType).then(function (response) {
            if (response.data !== null && response.data.length > 0) {
                $scope.LogisticWeights = response.data;
                getZoneBaseRateCard();
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
    };
    $scope.refreshData = function () {
        $scope.CourierAccounts = $filter('filter')($scope.data, $scope.searchCourierAccount, undefined);
    };
    $scope.filterBaseRatecard = function (CourierAccount) {

        for (var ma = 0; ma < $scope.finalArrayList.length ; ma++) {

            $scope.finalArrayList[ma].IsRowShow = true;

            for (var na = 0 ; na < $scope.finalArrayList[ma].Rows.length; na++) {
                $scope.finalArrayList[ma].Rows[na].IsCellShow = true;
            }
        }

        for (var k = 0 ; k < $scope.CourierAccounts.length; k++) {
            if (CourierAccount.LogisticServiceCourierAccountId == $scope.CourierAccounts[k].LogisticServiceCourierAccountId) {
                if ($scope.CourierAccounts[k].IsCaSelected) {
                    $scope.CourierAccounts[k].IsCaSelected = false;
                }
                else {
                    $scope.CourierAccounts[k].IsCaSelected = true;
                }
            }
            else {
                $scope.CourierAccounts[k].IsCaSelected = false;
            }

        }

        //CourierAccount.IsCaSelected = true;

        if (CourierAccount !== undefined && CourierAccount !== null && CourierAccount.LogisticServiceCourierAccountId > 0 && CourierAccount.IsCaSelected === true) {

            for (var i = 0; i < $scope.finalArrayList.length ; i++) {
                var flag = false;
                for (var j = 0 ; j < $scope.finalArrayList[i].Rows.length; j++) {
                    if ($scope.finalArrayList[i].Rows[j].courierAccount.AccountNo === CourierAccount.AccountNo) {
                        flag = true;
                        break;
                    }
                }
                for (var ka = 0 ; ka < $scope.finalArrayList[i].Rows.length; ka++) {
                    if ($scope.finalArrayList[i].Rows[ka].courierAccount.AccountNo !== CourierAccount.AccountNo) {
                        $scope.finalArrayList[i].Rows[ka].IsCellShow = false;
                    }
                }

                if (flag) {
                    $scope.finalArrayList[i].IsRowShow = true;
                }
                else {
                    $scope.finalArrayList[i].IsRowShow = false;
                }
            }
        }
        else {
            for (var m = 0; m < $scope.finalArrayList.length ; m++) {

                $scope.finalArrayList[m].IsRowShow = true;

                for (var n = 0 ; n < $scope.finalArrayList[m].Rows.length; n++) {
                    $scope.finalArrayList[m].Rows[n].IsCellShow = true;
                }
            }
        }
    };
    $scope.ClearFilter = function () {
        for (var m = 0; m < $scope.finalArrayList.length ; m++) {

            $scope.finalArrayList[m].IsRowShow = true;

            for (var n = 0 ; n < $scope.finalArrayList[m].Rows.length; n++) {
                $scope.finalArrayList[m].Rows[n].IsCellShow = true;
            }
        }
        for (var k = 0 ; k < $scope.CourierAccounts.length; k++) {
            $scope.CourierAccounts[k].IsCaSelected = false;
        }
        $scope.searchCourierAccount = '';
    };
    var getZones = function () {
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

        if ($scope.RateType !== undefined && $scope.RateType !== null && $scope.CourierCompany.Value !== 'Hermes' &&
             $scope.CourierCompany.Value !== 'Yodel' &&
             $scope.CourierCompany.Value !== 'UKMail') {
            RateType = $scope.RateType.Value;
        }
        ZoneBaseRateCardService.GetZones(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType).then(function (response) {
            $scope.Zones = response.data;
        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });
        //if ($scope.LogisticService !== undefined) {
        //    LogisticServiceType = $scope.LogisticService.LogisticServiceName;
        //}
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
    var courierAccounts = function () {
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
        if ($scope.RateType !== undefined && $scope.RateType !== null && $scope.CourierCompany.Value !== 'Hermes' &&
           $scope.CourierCompany.Value !== 'Yodel' &&
           $scope.CourierCompany.Value !== 'UKMail') {
            RateType = $scope.RateType.Value;
        }
        ZoneBaseRateCardService.GetCourierAccount(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType).then(function (response) {
            $scope.CourierAccounts = response.data;
            $scope.data = response.data;
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
    var getZoneBaseRateCard = function () {
        //
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

        if ($scope.RateType !== undefined && $scope.RateType !== null && $scope.CourierCompany.CourierCompany !== 'Hermes' &&
             $scope.CourierCompany.Value !== 'Yodel' &&
             $scope.CourierCompany.Value !== 'UKMail') {
            RateType = $scope.RateType.Value;
        }

        if ($scope.RateType !== undefined && $scope.RateType !== null && $scope.CourierCompany.CourierCompany !== 'Hermes' &&
            $scope.CourierCompany.Value !== 'Yodel' &&
            $scope.CourierCompany.Value !== 'UKMail' && $scope.CourierCompany.Value === 'DHL') {
            RateType = $scope.RateType.Value;
        }

        if ($scope.CourierCompany !== undefined && $scope.CourierCompany.Value !== 'Hermes' && $scope.CourierCompany.Value === 'UKMail' && $scope.PakageType !== undefined) {
            PackageType = $scope.PakageType.PakageType;
        }

        if ($scope.CourierCompany !== undefined && $scope.CourierCompany.Value === 'Hermes' && $scope.PodType !== undefined) {
            PackageType = $scope.PodType.PodTypeName;
        }

        if ($scope.ParcelServiceType !== undefined && $scope.CourierCompany !== undefined && $scope.CourierCompany.Value === 'UKMail') {
            ParcelType = $scope.ParcelServiceType.ParcelServiceType;
        }
        if ($scope.DocType !== undefined && $scope.DocType !== null) {
            DocType = $scope.DocType.Value;
        }
        //

        //if ($scope.LogisticService !== undefined && $scope.LogisticService.LogisticServiceName !== 'Hermes' && $scope.PakageType !== undefined) {
        //    pakageType = $scope.PakageType.PakageType;
        //}
        //if ($scope.ParcelServiceType !== undefined) {
        //    parcelServiceType = $scope.ParcelServiceType.ParcelServiceType;
        //}

        //if ($scope.LogisticService !== undefined) {
        //    LogisticServiceType = $scope.LogisticService.LogisticServiceName;
        //}
        //if ($scope.LogisticService !== undefined && $scope.LogisticService.LogisticServiceName === 'Hermes' && $scope.PodType !== undefined) {
        //    pakageType = $scope.PodType.PodTypeName;
        //}

        ZoneBaseRateCardService.GetZoneBaseRateCard(OperationZoneId, LogisticType, CourierCompany, RateType, PackageType, ParcelType, DocType, ModuleType).then(function (response) {
            $scope.ZoneBaseRateCardList = response.data;
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
            else if ($scope.LogisticType.Value === "EUExport" || $scope.LogisticType.Value === "EUImport") {
                eUImportExportJson();
            }
            else if ($scope.LogisticType.Value === "UKShipment" && $scope.CourierCompany.Value === "DHL" && $scope.RateType.Value === "Express") {
                ukShipmentDHLJson();
            }
            else {
                importExportThirdPartyJson();
            }

            AppSpinner.hideSpinnerTemplate();
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

                for (var w = 0 ; w < $scope.LogisticWeights.length; w++) {
                    for (var k = 0 ; k < newArray.length ; k++) {
                        if ($scope.LogisticWeights[w].ShipmentType.LogisticDescription === newArray[k].shipmentType.LogisticDescription &&
                            $scope.LogisticWeights[w].WeightFrom === newArray[k].LogisticWeight.WeightFrom &&
                            $scope.LogisticWeights[w].WeightFrom !== newArray[k].LogisticWeight.WeightLimit
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
                for (var m = 0 ; m < $scope.LogisticWeights.length; m++) {
                    for (var l = 0 ; l < newArray.length ; l++) {
                        if ($scope.LogisticWeights[m].ShipmentType.LogisticDescription === newArray[l].shipmentType.LogisticDescription && $scope.LogisticWeights[m].WeightFrom === newArray[l].LogisticWeight.WeightFrom && $scope.LogisticWeights[m].WeightFrom === newArray[l].LogisticWeight.WeightLimit) {
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

                for (var w = 0 ; w < $scope.LogisticWeights.length; w++) {
                    for (var k = 0 ; k < newArray.length ; k++) {
                        if ($scope.LogisticWeights[w].ShipmentType.LogisticDescription === newArray[k].shipmentType.LogisticDescription && $scope.LogisticWeights[w].WeightFrom === newArray[k].LogisticWeight.WeightFrom) {
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
    var importExportThirdPartyJson = function () {
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

                for (var w = 0 ; w < $scope.LogisticWeights.length; w++) {
                    for (var k = 0 ; k < newArray.length ; k++) {
                        if ($scope.LogisticWeights[w].ShipmentType.LogisticDescription === newArray[k].shipmentType.LogisticDescription && $scope.LogisticWeights[w].WeightFrom === newArray[k].LogisticWeight.WeightFrom) {
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

        for (var w = 0 ; w < $scope.LogisticWeights.length; w++) {
            for (var k = 0 ; k < newArray.length ; k++) {
                if ($scope.LogisticWeights[w].WeightFrom === newArray[k].LogisticWeight.WeightFrom && $scope.LogisticWeights[w].WeightTo === newArray[k].LogisticWeight.WeightTo) {
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
            EXPSAT: [],
            EXPISLScHi: [],
            DNOSPChIs: [],
            ROIR48: [],
            ROIR72: []
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
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "EXPISLScHi") {
                array.EXPISLScHi.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "DNOSPChIs") {
                array.DNOSPChIs.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "ROIR48") {
                array.ROIR48.push($scope.ZoneBaseRateCardList[j]);
            }
            if ($scope.ZoneBaseRateCardList[j].shipmentType.LogisticDescription === "ROIR72") {
                array.ROIR72.push($scope.ZoneBaseRateCardList[j]);
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

                for (var w = 0 ; w < $scope.LogisticWeights.length; w++) {
                    for (var k = 0 ; k < newArray.length ; k++) {
                        if ($scope.LogisticWeights[w].ShipmentType.LogisticDescription === newArray[k].shipmentType.LogisticDescription && $scope.LogisticWeights[w].WeightFrom === newArray[k].LogisticWeight.WeightFrom) {
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

                for (var w = 0 ; w < $scope.LogisticWeights.length; w++) {
                    for (var k = 0 ; k < newArray.length ; k++) {
                        if ($scope.LogisticWeights[w].ParcelType === newArray[k].LogisticWeight.ParcelType && $scope.LogisticWeights[w].WeightFrom === newArray[k].LogisticWeight.WeightFrom) {
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

                for (var w = 0 ; w < $scope.LogisticWeights.length; w++) {
                    for (var k = 0 ; k < newArray.length ; k++) {
                        if ($scope.LogisticWeights[w].ShipmentType.LogisticDescription === newArray[k].shipmentType.LogisticDescription && $scope.LogisticWeights[w].WeightFrom === newArray[k].LogisticWeight.WeightFrom) {
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

    var getCurrencyCodes = function () {
        ZoneBaseRateCardService.GetOperationZoneCurrencyCode($scope.OperationZone.OperationZoneId, 'Buy').then(function (response) {
            if (response.data !== null) {
                $scope.CurrencyCodes = response.data;
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
    };
    $scope.baseRateCardRateYodelWeightDimensionLimit = function (rateLimit, LimitType) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'zoneSetting/baseRateCard/baseRateCardWeightDimensionLimit.tpl.html',
            controller: 'BaseRateCardWeightDimensionLimitController',
            windowClass: '',
            size: 'md',
            backdrop: 'static',
            resolve: {
                OperationZone: function () {
                    return $scope.OperationZone;
                },
                RateLimitType: function () {
                    if (LimitType !== undefined) {
                        return LimitType;
                    }
                    else {
                        return "";
                    }

                },
                RateLimit: function () {
                    if (rateLimit === undefined) {
                        return null;
                    }
                    else {
                        return rateLimit;
                    }

                }
            }
        });
        modalInstance.result.then(function (Rate) {
            getZoneBaseRateCardLimit();
            getDimensionBaseRateCard();
        }, function () {
        });
    };
    $scope.baseRateCardRateLimit = function (rateLimit) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'zoneSetting/baseRateCard/baseRateCardLimit.tpl.html',
            controller: 'BaseRateCardLimitController',
            windowClass: '',
            size: 'md',
            backdrop: 'static',
            resolve: {
                OperationZone: function () {
                    return $scope.OperationZone;
                },
                ParcelServiceType: function () {
                    return $scope.ParcelServiceType;
                },
                PakageType: function () {
                    return $scope.PakageType;
                },
                RateLimit: function () {
                    if (rateLimit === undefined) {
                        return null;
                    }
                    else {
                        return rateLimit;
                    }

                }
            }
        });
        modalInstance.result.then(function (Rate) {
            getZoneBaseRateCardLimit();
        }, function () {
        });
    };

    // Now Base Rate Card Saved through pop-up
    $scope.SaveBaseRateCard = function () {
        for (var i = 0; i < $scope.ZoneBaseRateCardList.length; i++) {
            if ($scope.ZoneBaseRateCardList[i].IsChanged) {
                $scope.ZoneBaseRateCardSaveJson.push($scope.ZoneBaseRateCardList[i]);
            }
        }
        if ($scope.BaseRateLimitJson !== undefined && $scope.BaseRateLimitJson !== null && $scope.BaseRateLimitJson.length > 0 && $scope.ZoneBaseRateCardSaveJson !== null && $scope.ZoneBaseRateCardSaveJson.length > 0) {
            ZoneBaseRateCardService.SaveZoneBaseRateCardLimit($scope.BaseRateLimitJson).then(function (response) {
                if (response.data.Status) {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'success',
                        title: $scope.Success,
                        body: $scope.RateCardSave_Validation,
                        showCloseButton: true
                    });
                }
                else {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorSavingRecord,
                        showCloseButton: true
                    });
                }

            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorSavingRecord,
                    showCloseButton: true
                });
            });

        }
        else if ($scope.BaseRateLimitJson !== undefined && $scope.BaseRateLimitJson !== null && $scope.BaseRateLimitJson.length > 0 && $scope.ZoneBaseRateCardSaveJson !== null && $scope.ZoneBaseRateCardSaveJson.length === 0) {
            ZoneBaseRateCardService.SaveZoneBaseRateCardLimit($scope.BaseRateLimitJson).then(function (response) {
                if (response.data.Status) {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'success',
                        title: $scope.Success,
                        body: $scope.RateCardSave_Validation,
                        showCloseButton: true
                    });
                    init();
                }
                else {
                    AppSpinner.hideSpinnerTemplate();
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorSavingRecord,
                        showCloseButton: true
                    });
                }

            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorSavingRecord,
                    showCloseButton: true
                });
            });
        }

        if ($scope.ZoneBaseRateCardSaveJson !== null && $scope.ZoneBaseRateCardSaveJson.length > 0) {
            ZoneBaseRateCardService.SaveZoneBaseRateCard($scope.ZoneBaseRateCardSaveJson).then(function (response) {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'success',
                    title: $scope.Success,
                    body: $scope.RateCardSave_Validation,
                    showCloseButton: true
                });
                init();
            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorSavingRecord,
                    showCloseButton: true
                });
            });
        }
        if ($scope.BaseRateLimitJson !== undefined && $scope.BaseRateLimitJson !== null && $scope.BaseRateLimitJson.length === 0 && $scope.ZoneBaseRateCardSaveJson !== null && $scope.ZoneBaseRateCardSaveJson.length === 0) {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarningValidation,
                body: $scope.UpdateRecordValidation,
                showCloseButton: true
            });
        }

    };

    $scope.SetCourierAccount = function (Rate) {
        if ($scope.CourierAccount !== null && $scope.CourierAccount !== undefined) {
            Rate.courierAccount = $scope.CourierAccount;
        }
        $scope.BaseRateCardData.push(Rate);

    };

    $scope.SetCourierAccountAndCurrency = function (BaseRate) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'zoneSetting/baseRateCard/baseRateCurrencyCourierAccountAddEdit.tpl.html',
            controller: 'BaseRateCurrencyCourierAccountAddEditController',
            windowClass: '',
            size: 'lg',
            backdrop: 'static',
            resolve: {
                BaseRateCell: function () {
                    if (BaseRate === undefined) {
                        return null;
                    }
                    else {
                        return BaseRate;
                    }
                },
                OperationZone: function () {
                    return $scope.OperationZone;
                },
                CourierAccounts: function () {
                    return $scope.CourierAccounts;
                }
            }
        });
        modalInstance.result.then(function (Rate) {
            getZoneBaseRateCard();
        }, function () {
        });
    };
    $scope.baseRateCardWithoutAccount = function (BaseRate) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'zoneSetting/baseRateCard/baseRateCardWithoutAccount.tpl.html',
            controller: 'baseRateCardWithoutAccountController',
            windowClass: '',
            size: 'md',
            backdrop: 'static',
            resolve: {
                BaseRateCell: function () {
                    if (BaseRate === undefined) {
                        return null;
                    }
                    else {
                        return BaseRate;
                    }
                },
                OperationZone: function () {
                    return $scope.OperationZone;
                },
                ParcelServiceType: function () {
                    return $scope.ParcelServiceType;
                },
                PakageType: function () {
                    return $scope.PakageType;
                }
            }
        });
        modalInstance.result.then(function (Rate) {
            getZoneBaseRateCard();
        }, function () {
        });
    };
    function init() {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.spinnerMessage = 'Loading Base Rate Card';
        $scope.masterData = {};
        $scope.IsChanged = true;
        $scope.UkShipmentParcel = false;
        $scope.UkShipmentBagit = false;
        $scope.showPakageType = false;
        $scope.showLogisticServiceType = false;
        $scope.showParcelServiceType = false;
        $scope.BaseRateCardData = [];
        $scope.showRateExportImport = false;
        $scope.showRateUKParcel = false;
        $scope.showRateUKBagit = false;
        $scope.showRateUKBagit = false;
        $scope.showRateEUExportImort = false;
        $scope.finalArrayList = [];
        $scope.ShipmentTypeWeights = [];
        $scope.BaseRateCardLimit = [];
        $scope.overLimitHermesJson = [];

        $scope.ZoneBaseRateCardSaveJson = [];
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

        $scope.ServiceTypes = [{
            Name: 'DIH',
            Display: 'DIH'
        }];
        $scope.ServiceType = $scope.ServiceTypes[0];

        $scope.OperationZone = { OperationZoneId: 1, OperationZoneName: 'HKG' };
        getScreenInitials();

        setModalOptions();

    }
    init();
});