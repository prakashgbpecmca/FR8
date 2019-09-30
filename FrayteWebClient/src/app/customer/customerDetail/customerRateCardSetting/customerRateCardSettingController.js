angular.module('ngApp.customer').controller('CustomerRateCardSettingController', function (AppSpinner,UtilityService, $scope, $state, $location, $translate, $stateParams, $filter, CustomerService, SessionService, $uibModal, uiGridConstants, toaster, ModalService, TradelaneService, CarrierService, CountryService, ShipmentService, UserService, TopCountryService, TopCurrencyService, $timeout, DateFormatChange) {


    var setModalOptions = function () {
        $translate(['FrayteWarning', 'FrayteSuccess', 'Services', 'Error_registering_service', 'ErrorGetting', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation', 'ErrorSavingRecord', 'GettingDataError_Validation']).then(function (translations) {
            $scope.Frayte_Warning = translations.FrayteWarning;
            $scope.PleaseCorrect_ValidationErrors = translations.PleaseCorrectValidationErrors;
            $scope.Successfully_SavedInformation = translations.SuccessfullySavedInformation;
            $scope.ErrorSaving_Record = translations.ErrorSavingRecord;
            $scope.GettingDataErrorValidation = translations.GettingDataError_Validation;
            $scope.Errorregisteringservice = translations.Error_registering_service;
            $scope.Frayte_Success = translations.FrayteSuccess;
            $scope.Error_Getting = translations.ErrorGetting + ' ' + translations.Services;
        });
    };
    $scope.newCustomerDetail = function () {
        $scope.customerDetail = {
            UserId: $scope.UserId,
            OperationZoneId: 0,
            RegistredServices: [],
            CustomerRateSetting: {
                CustomererSettingId: 0,
                ScheduleSetting: 'Scheduled',
                ScheduleType: 'Daily',
                ScheduleDay: '',
                ScheduleDate: new Date(),
                ScheduleTime: '',
                AdditionalMails: '',
                ScheduleSettingType: 'RateCard',
                IsExcel: false,
                IsPdf: true,
                CustomerSettingDetail: []
            }
        };
    };

    $scope.GoBack = function () {
        if ($scope.tabs !== undefined && $scope.tabs !== null) {
            var route = UtilityService.GetCurrentRoute($scope.tabs, "customers");
            $state.go(route);

        }

    };

    // SetCustomerDetailSetting
    $scope.SetCustomerSettingDetail = function (Courier) {
        if (Courier.IsSelected) {
            if ($scope.customerDetail.CustomerRateSetting.CustomerSettingDetail.length === 0) {
                $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail.push({ CustomerSettingDetailId: 0, CustomerSettingId: 0, CourierShipment: Courier });
            }
            else {
                var flag = false;
                for (var i = 0; i < $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail.length; i++) {
                    if ($scope.customerDetail.CustomerRateSetting.CustomerSettingDetail[i].CourierShipment !== null && $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail[i].CourierShipment.CourierId === Courier.CourierId) {
                        flag = true;
                        break;
                    }
                }
                if (!flag) {
                    $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail.push({ CustomerSettingDetailId: 0, CustomerSettingId: 0, CourierShipment: Courier });
                }
            }
        }
        else {
            if ($scope.customerDetail.CustomerRateSetting.CustomerSettingDetail !== null && $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail.length > 0) {
                for (var j = 0; j < $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail.length; j++) {
                    if ($scope.customerDetail.CustomerRateSetting.CustomerSettingDetail[j].CourierShipment !== null && $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail[j].CourierShipment.CourierId === Courier.CourierId) {
                        $scope.customerDetail.CustomerRateSetting.CustomerSettingDetail.splice(j, 1);
                        break;
                    }
                }
            }

        }
    };
    // Customer rate Card
    $scope.changeRateSheduleDaySetting = function (ScheduleDay) {
        if (ScheduleDay !== undefined && ScheduleDay !== null && ScheduleDay !== '') {
            if ($scope.customerDetail.CustomerRateSetting !== null && $scope.customerDetail.CustomerRateSetting.ScheduleSetting === "Scheduled" && $scope.customerDetail.CustomerRateSetting.ScheduleType === 'Weekly') {
                $scope.RateWeekDayVisibility = true;
            }
            else {
                $scope.RateWeekDayVisibility = false;
            }
        }
    };
    var customerRateSettingVisibility = function () {
        if ($scope.customerDetail.CustomerRateSetting !== null && $scope.customerDetail.CustomerRateSetting.ScheduleSetting === "Scheduled") {
            $scope.RateSettingVisibility = true;
        }
        else {
            $scope.RateSettingVisibility = false;
        }
    };

    var setCustomerLogisticService = function () {
        if ($scope.logisticServices !== null && $scope.logisticServices.length && $scope.customerDetail.RegistredServices !== null && $scope.customerDetail.RegistredServices.length) {
            for (var i = 0; i < $scope.logisticServices.length; i++) {
                for (var j = 0; j < $scope.customerDetail.RegistredServices.length ; j++) {
                    if ($scope.logisticServices[i].LogisticServiceId === $scope.customerDetail.RegistredServices[j]) {
                        $scope.logisticServices[i].IsSelected = true;

                    }
                    if ($scope.LogisticServiceDetail[i].CourierDetail[j].LogisticServiceId === $scope.customerDetail.RegistredServices[j]) {
                        $scope.LogisticServiceDetail[i].CourierDetail[j].IsSelected = true;
                    }

                }
            }
        }
    };

    var setCustomerLogisticServiceDetail = function () {
        if ($scope.LogisticServiceDetail !== null && $scope.LogisticServiceDetail.length && $scope.customerDetail.RegistredServices !== null && $scope.customerDetail.RegistredServices.length) {
            for (var i = 0; i < $scope.LogisticServiceDetail.length; i++) {
                for (var k = 0; k < $scope.LogisticServiceDetail[i].CourierDetail.length; k++) {
                    for (var j = 0; j < $scope.customerDetail.RegistredServices.length ; j++) {

                        if ($scope.LogisticServiceDetail[i].CourierDetail[k].LogisticServiceId === $scope.customerDetail.RegistredServices[j]) {
                            $scope.LogisticServiceDetail[i].CourierDetail[k].IsSelected = true;
                        }

                    }
                }
            }
        }
    };

    var scheduleDayRateVisibilty = function () {
        if ($scope.customerDetail.CustomerRateSetting !== null && $scope.customerDetail.CustomerRateSetting.ScheduleType === "Daily" || $scope.customerDetail.CustomerRateSetting.ScheduleType === "Monthly" || $scope.customerDetail.CustomerRateSetting.ScheduleType === "Yearly") {
            $scope.RateWeekDayVisibility = false;
        }
        else {
            $scope.RateWeekDayVisibility = true;
        }
    };
    var getcustomerRateCardDetail = function () {
        CustomerService.GetCustomerRateCardDetail($scope.UserId).then(function (response) {
            $scope.customerDetail = response.data;
            if (response.data) {
                if (response.data.CustomerRateSetting !== null && response.data.CustomerRateSetting.CustomerSettingId) {
                    customerRateSettingVisibility();
                    if (response.data.CustomerRateSetting.ScheduleDate !== null) {
                        var RateDate = moment.utc(response.data.CustomerRateSetting.ScheduleDate).toDate();
                        $scope.customerDetail.CustomerRateSetting.ScheduleDate = DateFormatChange.DateFormatChange(moment.utc(response.data.CustomerRateSetting.ScheduleDate).toDate());
                    }
                    if ($scope.customerDetail.CustomerRateSetting.ScheduleSetting === "Scheduled") {
                        scheduleDayRateVisibilty();
                        $scope.ShowCustomerSettingDetail = true;
                    }
                    else {
                        $scope.ShowCustomerSettingDetail = false;
                    }
                    $scope.changeRateSheduleDaySetting($scope.customerDetail.CustomerRateSetting.ScheduleType);
                    // set registered logistic services
                    //setCustomerLogisticService();
                    setCustomerLogisticServiceDetail();

                }
                else {
                    $scope.RateSettingVisibility = true;
                    $scope.customerDetail.CustomerRateSetting = {
                        CustomererSettingId: 0,
                        ScheduleSetting: 'Scheduled',
                        ScheduleType: 'Daily',
                        ScheduleDay: '',
                        ScheduleDate: new Date(),
                        ScheduleTime: '',
                        AdditionalMails: '',
                        ScheduleSettingType: 'RateCard',
                        IsExcel: false,
                        IsPdf: true,
                        CustomerSettingDetail: []
                    };
                }
            }

        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.Error_Getting,
                showCloseButton: true
            });
        });
    };

    $scope.ChangeOperationZone = function () {
        $scope.OperationZoneId = $scope.OperationZone.OperationZoneId;
        getScreenInitails();

    };

    var logisticServiceDetailNew = function () {

        $scope.LogisticServiceDetail = [];
    };

    var getScreenInitails = function () {
        CustomerService.GetLogisticServices($scope.OperationZoneId).then(function (response) {
            $scope.logisticServices = response.data;
            logisticServiceDetailNew();
            var Courier = {
                LogisticServiceId: 0,
                LogisticType: '',
                LogisticDisplayType: '',
                RateType: '',
                RateTypeDisplay: ''
            };
            var LogisticDetail1 = {
                LogisticServiceId: 0,
                OperationZoneId: 0,
                LogisticCompany: '',
                LogisticCompanyDisplay: '',

                CourierDetail: []
            };
            var LogisticDetail2 = {
                LogisticServiceId: 0,
                OperationZoneId: 0,
                LogisticCompany: '',
                LogisticCompanyDisplay: '',

                CourierDetail: []
            };
            for (i = 0; i < response.data.length; i++) {
                if (response.data[i].LogisticCompany === 'DHL' && response.data[i].LogisticType !== 'UKShipment') {
                    LogisticDetail1.LogisticCompany = response.data[i].LogisticCompany;
                    LogisticDetail1.LogisticCompanyDisplay = response.data[i].LogisticCompanyDisplay;
                    Courier.LogisticType = response.data[i].LogisticType;
                    Courier.LogisticDisplayType = response.data[i].LogisticTypeDisplay;
                    Courier.LogisticServiceId = response.data[i].LogisticServiceId;
                    Courier.RateType = response.data[i].RateType;
                    Courier.RateTypeDisplay = response.data[i].RateType;
                    LogisticDetail1.CourierDetail.push(Courier);
                }
                else if (response.data[i].LogisticType === 'UKShipment' || (response.data[i].LogisticType === 'UKShipment' && response.data[i].LogisticCompany === 'DHL')) {
                    LogisticDetail2.LogisticCompany = response.data[i].LogisticType;
                    LogisticDetail2.LogisticCompanyDisplay = response.data[i].LogisticTypeDisplay;
                    Courier.LogisticServiceId = response.data[i].LogisticServiceId;
                    Courier.LogisticType = response.data[i].LogisticCompany;
                    Courier.LogisticDisplayType = response.data[i].LogisticCompanyDisplay;
                    Courier.RateType = response.data[i].RateType;
                    Courier.RateTypeDisplay = response.data[i].RateType;
                    LogisticDetail2.CourierDetail.push(Courier);
                }

                //if ((i === 2 && response.data[i].LogisticType === 'UKShipment') || (i === 4 && response.data[i].LogisticType === 'DHL')) {
                //    $scope.LogisticServiceDetail.push(LogisticDetail1);
                //    LogisticDetail = {
                //        LogisticServiceId: 0,
                //        OperationZoneId: 0,
                //        LogisticCompany: '',
                //        LogisticCompanyDisplay: '',

                //        CourierDetail: []
                //    };
                //}
                if (i === 4) {
                    $scope.LogisticServiceDetail.push(LogisticDetail1);
                    $scope.LogisticServiceDetail.push(LogisticDetail2);
                    LogisticDetail = {
                        LogisticServiceId: 0,
                        OperationZoneId: 0,
                        LogisticCompany: '',
                        LogisticCompanyDisplay: '',

                        CourierDetail: []
                    };
                }
                else if (i === 2 && response.data[i].LogisticCompany === 'DHL') {
                    $scope.LogisticServiceDetail.push(LogisticDetail1);
                    LogisticDetail = {
                        LogisticServiceId: 0,
                        OperationZoneId: 0,
                        LogisticCompany: '',
                        LogisticCompanyDisplay: '',

                        CourierDetail: []
                    };
                }
                //else if (i === 5 && response.data[i].LogisticCompanyLogisticType === 'DHL' && response.data[i].LogisticType === 'UKShipment')
                //    {
                //        $scope.LogisticServiceDetail.push(LogisticDetail);
                //        LogisticDetail = {
                //            LogisticServiceId: 0,
                //            OperationZoneId: 0,
                //            LogisticCompany: '',
                //            LogisticCompanyDisplay: '',

                //            CourierDetail: []
                //        };
                //    }
                //else if (i === 4 && response.data[i].LogisticCompany === 'DHL') {
                //    $scope.LogisticServiceDetail.push(LogisticDetail);
                //    LogisticDetail = {
                //        LogisticServiceId: 0,
                //        OperationZoneId: 0,
                //        LogisticCompany: '',
                //        LogisticCompanyDisplay: '',

                //        CourierDetail: []
                //    };
                //}
                Courier = {
                    LogisticServiceId: 0,
                    LogisticType: '',
                    LogisticDisplayType: '',
                    RateType: '',
                    RateTypeDisplay: ''
                };
            }



            if ($scope.logisticServices !== null && $scope.logisticServices.length) {
                for (var i = 0; i < $scope.logisticServices.length; i++) {
                    $scope.logisticServices[i].IsSelected = false;
                }
                if ($scope.UserId > 0) {
                    getcustomerRateCardDetail();
                }
                else {
                    $scope.newCustomerDetail();
                }

            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.GettingDataErrorValidation,
                showCloseButton: true
            });
        });
    };

    $scope.setCustomerService = function (service) {
        if (service !== undefined && service !== null) {
            if (service.IsSelected) {
                $scope.customerDetail.RegistredServices.push(service.LogisticServiceId);
            }
            else {
                CustomerService.RemoveCustomerLogistic(service.LogisticServiceId, $scope.UserId).then(function (response) {
                    if (response.status === 200 && response.data !== null && response.data.Status) {
                        for (var j = 0; j < $scope.customerDetail.RegistredServices.length ; j++) {
                            if (service.LogisticServiceId === $scope.customerDetail.RegistredServices[j]) {
                                service.LogisticServiceId.IsSelected = false;
                                $scope.customerDetail.RegistredServices.splice(j, 1);
                                break;
                            }
                        }
                    }
                    else {

                    }
                }, function () {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.Errorregisteringservice,
                        showCloseButton: true
                    });
                });
            }

        }

    };

    $scope.saveCustomerRateCardDetail = function (isValid, customerDetail) {
        if (isValid) {
            AppSpinner.showSpinnerTemplate("Saving Customer Detail", $scope.Template);
            CustomerService.SaveCustomerRateCardDetail($scope.customerDetail).then(function (response) {
                toaster.pop({
                    type: 'success',
                    title: $scope.Frayte_Success,
                    body: $scope.Successfully_SavedInformation,
                    showCloseButton: true
                });
                AppSpinner.hideSpinnerTemplate();
                $timeout(function () {
                    $scope.GoBack();
                }, 900);
            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.ErrorSaving_Record,
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.PleaseCorrect_ValidationErrors,
                showCloseButton: true
            });
        }
    };
    $scope.openCalender1 = function ($event) {
        $scope.status1.opened = true;
    };
    $scope.status1 = {
        opened: false
    };
    var getUserTab = function (tabs, tabKey) {
        if (tabs !== undefined && tabs !== null && tabs.length) {
            var tab = {};
            for (var i = 0; i < tabs.length; i++) {
                if (tabs[i].tabKey === tabKey) {
                    tab = tabs[i];
                    break;
                }
            }
            return tab;
        }
    };
    function init() {
        var sessionInfo = SessionService.getUser();
        $scope.tabs = sessionInfo.tabs;
        // For Customer rate  Card Sretting
        $scope.ShowCustomerSettingDetail = true;
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.weekDaysList = [
    { Name: 'Sunday' },
    { Name: 'Monday' },
    { Name: 'Tuesday' },
    { Name: 'Wednesday' },
    { Name: 'Thursday' },
    { Name: 'Friday' },
    { Name: 'Starday' }
        ];
        $scope.ScheduleTypes = [
            { Name: 'Daily' },
            { Name: 'Weekly' },
            { Name: 'Monthly' },
            { Name: 'Yearly' }
        ];
        $scope.ScheduleSettings = [
           { Name: 'PerShipment', DisplayName: 'POD Per Shipment' },
           { Name: 'Scheduled', DisplayName: 'Consolidate POD Report' }
        ];
        if ($stateParams.customerId !== undefined && $stateParams.customerId !== null && $stateParams.customerId !== "") {
            $scope.UserId = $stateParams.customerId;
        }
        else {
            $scope.UserId = 7;
        }

        setModalOptions();
        CustomerService.GetOperationZones().then(function (response) {

            $scope.OperationZones = response.data;
            $scope.OperationZoneId = response.data[0].OperationZoneId;
            $scope.OperationZone = response.data[0];
            if (response.data) {
                getScreenInitails();
            }
        });

    }

    init();
});