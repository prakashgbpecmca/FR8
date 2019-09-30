angular.module('ngApp.customer').controller('CustomerMarginCostController', function ($scope, config, $state, AppSpinner, $location, $translate, $stateParams, $filter, CustomerService, SessionService, $uibModal, uiGridConstants, toaster, ModalService, TradelaneService, CarrierService, CountryService, ShipmentService, UserService, $anchorScroll) {

    $scope.UKDomesticClass = function () {

        if ($scope.LogisticCompany !== undefined && $scope.LogisticCompany.LogisticCompany !== 'UKMail' &&
            $scope.LogisticCompany.LogisticCompany !== 'Yodel' && $scope.LogisticCompany.LogisticCompany !== 'Hermes') {
            return false;
        }
        else {
            return true;
        }
    };

    var setModalOptions = function () {
        $translate(['Address', 'Book', 'DeleteHeader', 'DeleteBody', 'Tradelane', 'SuccessfullySavedInformation', 'FrayteSuccess', 'FrayteValidation',
                    'PleaseCorrectValidationErrors', 'FrayteError', 'ErrorSavingRecord', 'ErrorGetting', 'customer', 'detail', 'Zone',
                    'Shipment_Type', 'Select_CourierAccount', 'SetRate_CArd', 'FrayteWarning', 'Customer_Margin_Saved_Successfully', 'Customer_Margin_Must_Higher', 'LoadingCustomerMarginCost',
        'UpdatingMarginCost']).then(function (translations) {
            $scope.headerTextOtherAddress = translations.Address + " " + translations.Book + " " + translations.DeleteHeader;
            $scope.bodyTextOtherAddress = translations.DeleteBody + " " + translations.Address;
            $scope.headerTextTradeLane = translations.Tradelane + " " + translations.DeleteHeader;
            $scope.bodyTextTradeLane = translations.DeleteBody + " " + translations.Tradelane + " " + translations.detail;
            $scope.TitleFrayteInformation = translations.FrayteSuccess;
            $scope.Customer_Margin_Saved_Successfully = translations.Customer_Margin_Saved_Successfully;
            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.SetRateCard = translations.SetRate_CArd;
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.Frayte_Warning = translations.FrayteWarning;
            $scope.TextSavingError = translations.ErrorSavingRecord;
            $scope.TextErrorGetting = translations.ErrorGetting + " " + translations.customer + " " + translations.detail;
            $scope.GettingZoneError = translations.ErrorGetting + " " + translations.Zone;
            $scope.TextErrorGettingShipment = translations.ErrorGetting + " " + translations.customer + " " + translations.Shipment_Type;
            $scope.SelectCourierAccount = translations.Select_CourierAccount;
            $scope.CustomerMarginMustHigher = translations.Customer_Margin_Must_Higher;
            $scope.LoadingCustomerMarginCost = translations.LoadingCustomerMarginCost;
            $scope.UpdatingMarginCost = translations.UpdatingMarginCost;
        });
    };

    $scope.CustomerMarginCost = {
        UserId: 0,
        OperationZoneId: 0,
        CustomerMarginOptionId: 0,
        CourierCompany: "",
        RateType: "",
        LogisticType: "",
        ModuleType: "DirectBooking",
        CustomerMargin:
        {
            PercentOfMargin: 0,
            Zone: {},
            ShipmentType: {}
        }
    };

    $scope.setNewMarginCostjson = function () {

        if ($scope.CustomerMarginCost !== null && $scope.CustomerMarginCost.CustomerMargin !== '') {
            $scope.FinalArray = [];
            $scope.FinalCustomerMargin = {
                LogisticType: "",
                LogisticTypeDisplay: "",
                CustomerMarginOptionId: 0,
                CourierCompany: "",
                RateType: "",
                ModuleType: "",
                Zones: [],
                Grid: []
            };
            var array = {
                DocType: '',
                DocDisplayType: '',
                Row: []

            };

            var newArray = $scope.CustomerMarginCost;
            for (var w = 0 ; w < newArray.length; w++) {
                for (var k = 0 ; k < $scope.ShipmentTypes.length ; k++) {

                    for (var m = 0 ; m < newArray[w].CustomerMargin.length; m++) {

                        if ($scope.ShipmentTypes[k] !== null && newArray[w].CustomerMargin[m].ShipmentType !== null && $scope.ShipmentTypes[k].LogisticDescription === newArray[w].CustomerMargin[m].ShipmentType.LogisticDescription &&
                            $scope.ShipmentTypes[k].RateType === newArray[w].CustomerMargin[m].ShipmentType.RateType &&
                            $scope.ShipmentTypes[k].LogisticType === newArray[w].CustomerMargin[m].ShipmentType.LogisticType) {
                            array.DocType = newArray[w].CustomerMargin[m].ShipmentType.LogisticDescription;
                            array.DocDisplayType = newArray[w].CustomerMargin[m].ShipmentType.LogisticDescriptionDisplay;
                            array.Row.push(newArray[w].CustomerMargin[m]);
                        }
                    }
                    if (array.Row.length > 0) {
                        $scope.FinalCustomerMargin.CellWidth = array.Row.length / 100;
                        $scope.FinalCustomerMargin.LogisticType = newArray[w].LogisticType;
                        $scope.FinalCustomerMargin.LogisticTypeDisplay = newArray[w].LogisticTypeDisplay;
                        $scope.FinalCustomerMargin.CourierCompany = newArray[w].CourierCompany;
                        $scope.FinalCustomerMargin.RateType = newArray[w].RateType;
                        $scope.FinalCustomerMargin.ModuleType = newArray[w].ModuleType;
                        $scope.FinalCustomerMargin.Grid.push(array);
                    }
                    array = {
                        DocType: '',
                        DocDisplayType: '',
                        Row: []

                    };
                }

                // set Zones here 
                for (var d = 0 ; d < $scope.Zones.length; d++) {
                    if ($scope.FinalCustomerMargin.LogisticType === $scope.Zones[d].LogisticType &&
                        $scope.FinalCustomerMargin.CourierCompany === $scope.Zones[d].CourierComapny &&
                        $scope.FinalCustomerMargin.RateType === $scope.Zones[d].RateType &&
                        $scope.FinalCustomerMargin.ModuleType === $scope.Zones[d].ModuleType) {
                        $scope.FinalCustomerMargin.Zones.push($scope.Zones[d]);
                    }
                }
                if ($scope.FinalCustomerMargin.Zones.length) {
                    $scope.CourierComp = $scope.FinalCustomerMargin.CourierCompany;
                    $scope.FinalArray.push($scope.FinalCustomerMargin);

                }

                $scope.FinalCustomerMargin = {
                    LogisticType: "",
                    LogisticDisplayType: "",
                    CustomerMarginOptionId: 0,
                    CourierCompany: "",
                    RateType: "",
                    ModuleType: "",
                    Zones: [],
                    Grid: []
                };
            }

            array = {
                DocType: '',
                DocDisplayType: '',
                Row: []

            };

            for (var uw = 0 ; uw < newArray.length; uw++) {
                for (var um = 0 ; um < newArray[uw].CustomerMargin.length; um++) {
                    if (newArray[uw].CustomerMargin[um].ShipmentType === null) {
                        array.Row.push(newArray[uw].CustomerMargin[um]);
                    }
                }
                if (array.Row.length > 0) {
                    $scope.FinalCustomerMargin.LogisticType = newArray[uw].LogisticType;
                    $scope.FinalCustomerMargin.CourierCompany = newArray[uw].CourierCompany;
                    $scope.FinalCustomerMargin.RateType = newArray[uw].RateType;
                    $scope.FinalCustomerMargin.ModuleType = newArray[uw].ModuleType;
                    $scope.FinalCustomerMargin.Grid.push(array);
                }
                array = {
                    DocType: '',
                    DocDisplayType: '',
                    Row: []

                };

                // set Zones here 
                for (var ud = 0 ; ud < $scope.Zones.length; ud++) {
                    if ($scope.FinalCustomerMargin.LogisticType === $scope.Zones[ud].LogisticType &&
                        $scope.FinalCustomerMargin.CourierCompany === $scope.Zones[ud].CourierComapny &&
                        $scope.FinalCustomerMargin.RateType === $scope.Zones[ud].RateType &&
                        $scope.FinalCustomerMargin.ModuleType === $scope.Zones[ud].ModuleType) {
                        $scope.FinalCustomerMargin.Zones.push($scope.Zones[ud]);
                    }
                }
                if ($scope.FinalCustomerMargin.Zones.length) {
                    $scope.FinalArray.push($scope.FinalCustomerMargin);
                }
                $scope.FinalCustomerMargin = {
                    LogisticType: "",
                    LogisticTypeDisplay: "",
                    CustomerMarginOptionId: 0,
                    CourierCompany: "",
                    RateType: "",
                    ModuleType: "",
                    Zones: [],
                    Grid: []
                };
            }

        }
    };

    $scope.SetCustomerMargin = function () {
        if ($scope.MarginPercent === '' || $scope.MarginPercent === 0 || $scope.MarginPercent === undefined || $scope.MarginPercent === null) {
            if ($scope.CustomerMarginCost !== null) {
                for (zz = 0; zz < $scope.CustomerMarginCost.length; zz++) {
                    for (var z = 0 ; z < $scope.CustomerMarginCost[zz].CustomerMargin.length; z++) {
                        $scope.CustomerMarginCost[zz].CustomerMargin[z].PercentOfMargin = 0;
                    }
                    $scope.CustomerMarginSaveJson.CustomerMarginList.push($scope.CustomerMarginCost[zz].CustomerMargin[z]);
                }
            }
            for (jjj = 0; jjj < $scope.CustomerMarginCost.length; jjj++) {
                for (var jj = 0 ; jj < $scope.FinalUKShipmentCustomerMargin.length; jj++) {
                    $scope.FinalUKShipmentCustomerMargin[jj].PercentOfMargin = 0;
                    $scope.CustomerMarginSaveJson.CustomerMarginList.push($scope.FinalUKShipmentCustomerMargin[jj]);
                }
            }
            for (kkk = 0; kkk < $scope.CustomerMarginCost.length; kkk++) {
                for (var kk = 0 ; kk < $scope.FinalEUImportCustomerMargin.length; kk++) {

                    $scope.FinalEUImportCustomerMargin[kk].PercentOfMargin = 0;
                    $scope.CustomerMarginSaveJson.CustomerMarginList.push($scope.FinalEUImportCustomerMargin[kk]);
                }
            }
            for (lll = 0; lll < $scope.CustomerMarginCost.length; lll++) {
                for (var ll = 0 ; ll < $scope.FinalEUExportCustomerMargin.length; ll++) {
                    $scope.FinalEUExportCustomerMargin[ll].PercentOfMargin = 0;
                    $scope.CustomerMarginSaveJson.CustomerMarginList.push($scope.FinalEUExportCustomerMargin[ll]);
                }
            }
        }

        if ($scope.MarginPercent !== undefined && $scope.MarginPercent !== '' && $scope.MarginPercent !== null) {
            if ($scope.CustomerMarginCost !== null) {
                for (var ii = 0 ; ii < $scope.CustomerMarginCost.length; ii++) {
                    for (var i = 0 ; i < $scope.CustomerMarginCost[ii].CustomerMargin.length; i++) {
                        $scope.CustomerMarginCost[ii].CustomerMargin[i].PercentOfMargin = $scope.MarginPercent;
                    }
                    $scope.CustomerMarginSaveJson.CustomerMarginList.push($scope.CustomerMarginCost[ii].CustomerMargin[i]);
                }
            }
            for (var e = 0 ; e < $scope.CustomerMarginCost.length; e++) {
                for (var j = 0 ; j < $scope.FinalUKShipmentCustomerMargin.length; j++) {
                    $scope.FinalUKShipmentCustomer[j].PercentOfMargin = $scope.MarginPercent;
                }
                $scope.CustomerMarginSaveJson.CustomerMarginList.push($scope.FinalUKShipmentCustomerMargin[j]);
            }

            for (var f = 0 ; f < $scope.CustomerMarginCost.length; f++) {
                for (var k = 0 ; k < $scope.FinalEUImportCustomerMargin.length; k++) {
                    $scope.FinalEUImportCustomerMargin[k].PercentOfMargin = $scope.MarginPercent;
                }
                $scope.CustomerMarginSaveJson.CustomerMarginList.push($scope.FinalEUImportCustomerMargin[k]);
            }

            for (var g = 0 ; g < $scope.CustomerMarginCost.length; g++) {
                for (var l = 0 ; l < $scope.FinalEUExportCustomerMargin.length; l++) {
                    $scope.FinalEUExportCustomerMargin[l].PercentOfMargin = $scope.MarginPercent;
                }
                $scope.CustomerMarginSaveJson.CustomerMarginList.push($scope.FinalEUExportCustomerMargin[l]);
            }
        }

        CustomerService.GetInitial($scope.OperationZone.OperationZoneId, $scope.LogisticCompany.LogisticCompany, $scope.customerId).then(function (response) {
            if (response.data) {
                $scope.LogisticCompanies = response.data.Logistic;
                $scope.Marginoptionobject = {
                    CustomerMarginOptionId: 0,
                    LogisticCompany: null,
                    LogisticCompanyDisplay: null,
                    MarginPercentage: 0,
                    OperationZoneId: null,
                    OptionName: "SetMarginPercentage",
                    OptionNameDisplay: "Set Margin Percentage",
                    ShipmentType: null,
                    ShipmentTypeDisplay: null
                };
                $scope.MarginOptions = response.data.MarginOption;
            }
        });
    };

    $scope.SetMarginCost = function () {
        if ($scope.MarginOption !== null) {
            $scope.MarginPer = $scope.MarginOption.OptionName;
            $scope.CustMargOptiId = $scope.MarginOption.CustomerMarginOptionId;
        }
        else {
            $scope.MarginPer = '';
            $scope.CustMargOptiId = 0;
        }

        CustomerService.CustomerMarginOption($scope.OperationZone.OperationZoneId, $scope.LogisticCompany.LogisticCompany, $scope.MarginPer).then(function (response) {
            $scope.Margin = response.data;
            if ($scope.Margin !== undefined && $scope.Margin !== '' && $scope.Margin !== null) {
                if ($scope.FinalArray !== null) {
                    for (var bb = 0 ; bb < $scope.Margin.length; bb++) {
                        for (var ii = 0 ; ii < $scope.FinalArray.length; ii++) {
                            for (var i = 0 ; i < $scope.FinalArray[ii].Grid.length; i++) {
                                if ($scope.Margin[bb].ShipmentType === $scope.FinalArray[ii].Grid[i].DocType) {
                                    for (aa = 0; aa < $scope.FinalArray[ii].Grid[i].Row.length; aa++) {
                                        $scope.FinalArray[ii].Grid[i].Row[aa].PercentOfMargin = $scope.Margin[bb].MarginPercentage;
                                        $scope.FinalArray[ii].CustomerMarginOptionId = $scope.CustMargOptiId;
                                    }
                                }
                            }
                        }
                    }
                }
                for (var e = 0 ; e < $scope.CustomerMarginCost.length; e++) {
                    for (var j = 0 ; j < $scope.FinalUKShipmentCustomerMargin.length; j++) {
                        $scope.FinalUKShipmentCustomer[j].PercentOfMargin = $scope.MarginPercent;
                    }
                    $scope.CustomerMarginSaveJson.CustomerMarginList.push($scope.FinalUKShipmentCustomerMargin[j]);
                }

                for (var f = 0 ; f < $scope.CustomerMarginCost.length; f++) {
                    for (var k = 0 ; k < $scope.FinalEUImportCustomerMargin.length; k++) {
                        $scope.FinalEUImportCustomerMargin[k].PercentOfMargin = $scope.MarginPercent;
                    }
                    $scope.CustomerMarginSaveJson.CustomerMarginList.push($scope.FinalEUImportCustomerMargin[k]);
                }

                for (var g = 0 ; g < $scope.CustomerMarginCost.length; g++) {
                    for (var l = 0 ; l < $scope.FinalEUExportCustomerMargin.length; l++) {
                        $scope.FinalEUExportCustomerMargin[l].PercentOfMargin = $scope.MarginPercent;
                    }
                    $scope.CustomerMarginSaveJson.CustomerMarginList.push($scope.FinalEUExportCustomerMargin[l]);
                }
            }
        });
    };

    $scope.MarginCostByOperationZone = function () {
        $scope.FinalCustomerMargin = [];
        $scope.FinalUKShipmentCustomerMargin = [];
        $scope.FinalEUExportCustomerMargin = [];
        $scope.FinalEUImportCustomerMargin = [];
    };

    var customerMarginCost = function () {
        AppSpinner.showSpinnerTemplate($scope.LoadingCustomerMarginCost, $scope.Template);
        if ($scope.LogisticCompany !== undefined && $scope.OperationZone !== undefined) {
            CustomerService.GetZoneList($scope.OperationZone.OperationZoneId, '', $scope.LogisticCompany.LogisticCompany, '', 'DirectBooking').then(function (response) {
                $scope.Zones = response.data;

                if ($scope.customerId > 0) {
                    CustomerService.GetCustomerMargin($scope.customerId, $scope.OperationZone.OperationZoneId, $scope.LogisticCompany.LogisticCompany, 'DirectBooking').then(function (response) {

                        if (response.data !== null && response.data.length > 0) {
                            $scope.flag = false;
                            $scope.CustomerMarginCost = response.data;

                            if ($scope.MarginOptions.length > 0) {
                                for (var m = 0; m < $scope.MarginOptions.length; m++) {
                                    if ($scope.CustomerMarginCost.length > 0) {
                                        for (var n = 0; n < $scope.CustomerMarginCost.length; n++) {
                                            if ($scope.MarginOptions[m].CustomerMarginOptionId === $scope.CustomerMarginCost[n].CustomerMarginOptionId) {
                                                $scope.MarginOption = $scope.MarginOptions[m];
                                            }
                                        }
                                    }
                                }
                            }

                            CustomerService.GetShipmentType($scope.OperationZone.OperationZoneId, $scope.LogisticCompany.LogisticCompany, 'DirectBooking').then(function (response) {
                                if (response.data !== null && response.data.length > 0) {
                                    $scope.ShipmentTypes = response.data;

                                    $scope.setNewMarginCostjson();
                                    AppSpinner.hideSpinnerTemplate();
                                }
                            }, function () {
                                AppSpinner.hideSpinnerTemplate();
                                toaster.pop({
                                    type: 'error',
                                    title: $scope.TitleFrayteError,
                                    body: $scope.TextErrorGettingShipment,
                                    showCloseButton: true
                                });
                            });
                        }
                        else {
                            $scope.flag = true;
                        }

                    }, function () {
                        AppSpinner.hideSpinnerTemplate();
                        toaster.pop({
                            type: 'error',
                            title: $scope.TitleFrayteError,
                            body: $scope.TextErrorGetting,
                            showCloseButton: true
                        });
                    });
                }
            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorGetting,
                    showCloseButton: true
                });
            });
        }
        else {
            AppSpinner.hideSpinnerTemplate();
        }

    };

    $scope.GetZoneData = function () {
        if ($scope.LogisticCompany !== undefined) {
            CustomerService.GetZoneList($scope.OperationZone.OperationZoneId, '', $scope.LogisticCompany.LogisticCompany, '', 'DirectBooking').then(function (response) {
                $scope.Zones = response.data;
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorGetting,
                    showCloseButton: true
                });
                AppSpinner.hideSpinnerTemplate();
            });

        }
        else {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.SetRateCard,
                showCloseButton: true
            });
        }


    };

    $scope.customerMarginChange = function () {
        AppSpinner.showSpinnerTemplate($scope.LoadingCustomerMarginCost, $scope.Template);
        CustomerService.GetInitial($scope.OperationZone.OperationZoneId, $scope.LogisticCompany.LogisticCompany, $scope.customerId).then(function (response) {
            if (response.data) {
                $scope.LogisticCompanies = response.data.Logistic;

                $scope.MarginOptions = response.data.MarginOption;
                customerMarginCost();
            }
        }, function () {
            AppSpinner.hideSpinnerTemplate();
        });
    };

    $scope.InitialMethodCall = function () {
        CustomerService.GetInitial($scope.OperationZone.OperationZoneId, '', $scope.customerId).then(function (response) {
            if (response.data) {
                $scope.LogisticCompanies = response.data.Logistic;
                $scope.LogisticCompany = $scope.LogisticCompanies[0];
                if ($scope.LogisticCompanies) {
                    $scope.GetZoneData();
                }
                $scope.MarginOptions = response.data.MarginOption;
                customerMarginCost();
            }
        }, function () {
            AppSpinner.hideSpinnerTemplate();
        });
    };

    $scope.GetCustomerDetails = function () {
        AppSpinner.showSpinnerTemplate($scope.spinnerMessage, $scope.Template);
        ShipmentService.GetInitials().then(function (response) {
            $scope.OperationZones = response.data.OperationZones;
            if ($scope.OperationZone !== undefined && $scope.OperationZone !== null && $scope.OperationZone.OperationZoneId > 0) {
                $scope.InitialMethodCall();
            }
        }, function () {
            AppSpinner.hideSpinnerTemplate();
        });
    };

    $scope.LogisticCompanyChange = function () {
        CustomerService.GetInitial($scope.OperationZone.OperationZoneId, $scope.LogisticCompany.LogisticCompany).then(function (response) {
            if (response.data) {
                $scope.LogisticCompanies = response.data.Logistic;
                $scope.MarginOptions = response.data.MarginOption;
            }
        });
        $scope.customerMarginChange();
    };

    $scope.CustomerMarginCost = {
        UserId: 0,
        OperationZoneId: 0,
        CustomerMarginOptionId: 0,
        CourierCompany: "",
        RateType: "",
        LogisticType: "",
        ModuleType: "DirectBooking",
        CustomerMargin:
        {
            PercentOfMargin: 0,
            Zone: {},
            ShipmentType: {}
        }
    };

    //ToDo
    $scope.SaveCustomerMargin = function (isValid) {
        if (isValid) {
            $scope.NewCustomerMarginCost = [];
            for (var i = 0; i < $scope.FinalUKShipmentCustomerMargin.length; i++) {
                $scope.CustomerMarginSaveJson.CustomerMarginList.push($scope.FinalUKShipmentCustomerMargin[i]);
            }
            for (var j = 0; j < $scope.FinalEUImportCustomerMargin.length; j++) {
                $scope.CustomerMarginSaveJson.CustomerMarginList.push($scope.FinalEUImportCustomerMargin[j]);
            }
            for (var k = 0; k < $scope.FinalEUExportCustomerMargin.length; k++) {
                $scope.CustomerMarginSaveJson.CustomerMarginList.push($scope.FinalEUExportCustomerMargin[k]);
            }

            if ($scope.CustomerMarginCost.length > 0) {
                for (var l = 0; l < $scope.CustomerMarginCost.length; l++) {
                    var n = 0;
                    $scope.CustomerMarginCost[l].CustomerMarginOptionId = $scope.CustMargOptiId;
                    for (var m = 0; m < $scope.CustomerMarginCost[l].CustomerMargin.length; m++) {
                        if ($scope.CustomerMarginCost[l].CustomerMargin[m].PercentOfMargin > 0) {
                            n++;
                            if (n === $scope.CustomerMarginCost[l].CustomerMargin.length) {
                                $scope.NewCustomerMarginCost.push($scope.CustomerMarginCost[l]);
                            }
                        }
                        else {
                            toaster.pop({
                                type: 'warning',
                                title: $scope.Frayte_Warning,
                                body: $scope.CustomerMarginMustHigher,
                                showCloseButton: true
                            });
                            break;
                        }
                    }
                }
            }

            if ($scope.NewCustomerMarginCost.length === $scope.CustomerMarginCost.length) {

                AppSpinner.showSpinnerTemplate($scope.UpdatingMarginCost, $scope.Template);
                CustomerService.SaveCustomerMargin($scope.CustomerMarginCost).then(function (response) {

                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteInformation,
                        body: $scope.Customer_Margin_Saved_Successfully,
                        showCloseButton: true
                    });
                    AppSpinner.hideSpinnerTemplate();

                }, function () {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextSavingError,
                        showCloseButton: true
                    });
                    AppSpinner.hideSpinnerTemplate();
                });
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.Frayte_Warning,
                    body: $scope.CustomerMarginMustHigher,
                    showCloseButton: true
                });
            }
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.TextValidation,
                showCloseButton: true
            });
        }
    };

    function init() {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.spinnerMessage = $scope.LoadingCustomerMarginCost;
        $scope.flag = false;
        $scope.submitted = true;
        $scope.FinalCustomerMargin = [];
        $scope.FinalUKShipmentCustomerMargin = [];
        $scope.FinalEUExportCustomerMargin = [];
        $scope.FinalEUImportCustomerMargin = [];

        //Set browser scroll in top
        $location.hash('top');
        $anchorScroll();

        if ($stateParams.customerId === undefined) {
            $scope.customerId = "0";
        }
        else {
            var userInfo1 = SessionService.getUser();
            $scope.OperationZoneName = userInfo1.OperationZoneName;
            $scope.customerId = $stateParams.customerId;
            CustomerService.GetCustomerDetail($scope.customerId).then(function (response) {
                if (response.data) {
                    $scope.OperationZone = { OperationZoneId: response.data.OperationZoneId };
                    $scope.InitialMethodCall();
                }
            }, function () {
                AppSpinner.hideSpinnerTemplate();
            });
        }
        $scope.CustomerMarginSaveJson = {
            UserId: $scope.customerId,
            CustomerMarginList: []
        };
        setModalOptions();
    }

    init();
});