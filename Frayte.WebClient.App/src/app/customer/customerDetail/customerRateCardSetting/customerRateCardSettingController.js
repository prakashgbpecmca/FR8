angular.module('ngApp.customer').controller('CustomerRateCardSettingController', function (AppSpinner, UtilityService, $scope, config, $state, $location, $translate, $stateParams, $filter, CustomerService, SessionService, $uibModal, uiGridConstants, toaster, ModalService, TradelaneService, CarrierService, CountryService, ShipmentService, UserService, TopCountryService, TopCurrencyService, $timeout, DateFormatChange, $anchorScroll, TimeStringtoDateTime) {

    var setModalOptions = function () {
        $translate(['FrayteWarning', 'FrayteSuccess', 'Services', 'Error_registering_service', 'ErrorGetting', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation', 'ErrorSavingRecord',
            'GettingDataError_Validation', 'Customer_RateCard_Saved_Information_Successfully', 'Yodel_Address_Type', 'SavingCustomerDetail']).then(function (translations) {
                $scope.Frayte_Warning = translations.FrayteWarning;
                $scope.PleaseCorrect_ValidationErrors = translations.PleaseCorrectValidationErrors;
                $scope.Customer_RateCard_Saved_Information_Successfully = translations.Customer_RateCard_Saved_Information_Successfully;
                $scope.ErrorSaving_Record = translations.ErrorSavingRecord;
                $scope.GettingDataErrorValidation = translations.GettingDataError_Validation;
                $scope.Errorregisteringservice = translations.Error_registering_service;
                $scope.Frayte_Success = translations.FrayteSuccess;
                $scope.Error_Getting = translations.ErrorGetting + ' ' + translations.Services;
                $scope.YodelAddressService = translations.Yodel_Address_Type;
                $scope.SavingCustomerDetail = translations.SavingCustomerDetail;
            });
    };

    $scope.newCustomerDetail = function () {
        $scope.customerDetail = {
            UserId: $scope.UserId,
            OperationZoneId: 0,
            RegistredServices: [{
                LogisticServiceId: 0,
                LogisticServiceType: ''
            }],
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
            var route = UtilityService.GetCurrentRoute($scope.tabs, "userTabs.customers");
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
                    if ($scope.logisticServices[i].LogisticServiceId === $scope.customerDetail.RegistredServices[j].LogisticServiceId) {
                        $scope.logisticServices[i].IsSelected = true;
                    }
                    if ($scope.LogisticServiceDetail[i].CourierDetail[j].LogisticServiceId === $scope.customerDetail.RegistredServices[j].LogisticServiceId) {
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
                        if ($scope.LogisticServiceDetail[i].CourierDetail[k].LogisticServiceId === $scope.customerDetail.RegistredServices[j].LogisticServiceId) {
                            if ($scope.LogisticServiceDetail[i].CourierDetail[k].LogisticType === 'Yodel') {
                                $scope.LogisticServiceDetail[i].CourierDetail[k].LogisticServiceType = $scope.customerDetail.RegistredServices[j].LogisticServiceType;
                                $scope.LogisticServiceDetail[i].CourierDetail[k].IsSelected = true;
                            }
                            else {
                                $scope.LogisticServiceDetail[i].CourierDetail[k].IsSelected = true;
                            }
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
                for (i = 0; i < $scope.customerDetail.RegistredServices.length; i++) {
                    if ($scope.customerDetail.RegistredServices[i].LogisticServiceId === 13) {
                        //$scope.YodelService = $scope.customerDetail.RegistredServices[i];

                        if ($scope.customerDetail.RegistredServices[i].LogisticServiceType === 'DIH') {
                            $scope.customerDetail.RegistredServices[i].LogisticServiceType = 'DIH';
                            $scope.IsYodel = true;
                        }
                        else if ($scope.customerDetail.RegistredServices[i].LogisticServiceType === 'PODDISVC') {
                            $scope.customerDetail.RegistredServices[i].LogisticServiceType = 'PODDISVC';
                            $scope.IsYodel = true;
                        }
                        else if ($scope.customerDetail.RegistredServices[i].LogisticServiceType === 'SVCPOD') {
                            $scope.customerDetail.RegistredServices[i].LogisticServiceType = 'SVCPOD';
                            $scope.IsYodel = true;
                        }
                        else if ($scope.customerDetail.RegistredServices[i].LogisticServiceType === 'TRPOD') {
                            $scope.customerDetail.RegistredServices[i].LogisticServiceType = 'TRPOD';
                            $scope.IsYodel = true;
                        }
                    }
                }
                if ($scope.RoleId === 1 || $scope.RoleId === 6) {
                    if (response.data.CustomerRateSetting !== null && response.data.CustomerRateSetting.CustomerSettingId) {
                        customerRateSettingVisibility();
                        if (response.data.CustomerRateSetting.ScheduleDate !== null) {
                            var RateDate = moment.utc(response.data.CustomerRateSetting.ScheduleDate).toDate();
                            $scope.customerDetail.CustomerRateSetting.ScheduleDate = moment.utc(response.data.CustomerRateSetting.ScheduleDate).toDate();
                            //$scope.customerDetail.CustomerRateSetting.ScheduleDate = DateFormatChange.DateFormatChange(moment.utc(response.data.CustomerRateSetting.ScheduleDate).toDate());
                        }
                        if ($scope.customerDetail.CustomerRateSetting.ScheduleSetting === "Scheduled") {
                            scheduleDayRateVisibilty();
                            $scope.ShowCustomerSettingDetail = true;
                        }
                        else {
                            $scope.ShowCustomerSettingDetail = false;
                        }
                        $scope.changeRateSheduleDaySetting($scope.customerDetail.CustomerRateSetting.ScheduleType);
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
                else {
                    setCustomerLogisticServiceDetail();
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

    $scope.ChangeYodelService = function (LogisticServiceType) {
        if (LogisticServiceType) {
            $scope.YodelOptionService = LogisticServiceType;
        }
    };

    var logisticServiceDetailNew = function () {
        $scope.LogisticServiceDetail = [];
    };

    var getScreenInitails = function () {
        CustomerService.GetLogisticServices($scope.OperationZoneId, $scope.RoleId, $scope.CreatedBy).then(function (response) {
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
                OperationZoneId: $scope.OperationZoneId,
                LogisticCompany: '',
                LogisticCompanyDisplay: '',

                CourierDetail: []
            };
            var LogisticDetail2 = {
                LogisticServiceId: 0,
                OperationZoneId: $scope.OperationZoneId,
                LogisticCompany: '',
                LogisticCompanyDisplay: '',

                CourierDetail: []
            };
            var LogisticDetail3 = {
                LogisticServiceId: 0,
                OperationZoneId: $scope.OperationZoneId,
                LogisticCompany: '',
                LogisticCompanyDisplay: '',

                CourierDetail: []
            };
            var LogisticDetail4 = {
                LogisticServiceId: 0,
                OperationZoneId: $scope.OperationZoneId,
                LogisticCompany: '',
                LogisticCompanyDisplay: '',

                CourierDetail: []
            };
            var LogisticDetail5 = {
                LogisticServiceId: 0,
                OperationZoneId: $scope.OperationZoneId,
                LogisticCompany: '',
                LogisticCompanyDisplay: '',

                CourierDetail: []
            };
            var LogisticDetail6 = {
                LogisticServiceId: 0,
                OperationZoneId: $scope.OperationZoneId,
                LogisticCompany: '',
                LogisticCompanyDisplay: '',

                CourierDetail: []
            };
            var LogisticDetail7 = {
                LogisticServiceId: 0,
                OperationZoneId: $scope.OperationZoneId,
                LogisticCompany: '',
                LogisticCompanyDisplay: '',

                CourierDetail: []
            };
            var LogisticDetail8 = {
                LogisticServiceId: 0,
                OperationZoneId: $scope.OperationZoneId,
                LogisticCompany: '',
                LogisticCompanyDisplay: '',

                CourierDetail: []
            };

            var LogisticDetail9 = {
                LogisticServiceId: 0,
                OperationZoneId: $scope.OperationZoneId,
                LogisticCompany: '',
                LogisticCompanyDisplay: '',

                CourierDetail: []
            };
            var LogisticDetail10 = {
                LogisticServiceId: 0,
                OperationZoneId: $scope.OperationZoneId,
                LogisticCompany: '',
                LogisticCompanyDisplay: '',

                CourierDetail: []
            };
            var LogisticDetail11 = {
                LogisticServiceId: 0,
                OperationZoneId: $scope.OperationZoneId,
                LogisticCompany: '',
                LogisticCompanyDisplay: '',

                CourierDetail: []
            };
            var LogisticDetail12 = {
                LogisticServiceId: 0,
                OperationZoneId: $scope.OperationZoneId,
                LogisticCompany: '',
                LogisticCompanyDisplay: '',

                CourierDetail: []
            };
            var LogisticDetail13 = {
                LogisticServiceId: 0,
                OperationZoneId: $scope.OperationZoneId,
                LogisticCompany: '',
                LogisticCompanyDisplay: '',

                CourierDetail: []
            };
            var LogisticDetail14 = {
                LogisticServiceId: 0,
                OperationZoneId: $scope.OperationZoneId,
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
                else if (response.data[i].LogisticCompany === 'TNT' && response.data[i].LogisticType !== 'UKShipment' && $scope.OperationZoneId === 1) {
                    LogisticDetail3.LogisticCompany = response.data[i].LogisticCompany;
                    LogisticDetail3.LogisticCompanyDisplay = response.data[i].LogisticCompanyDisplay;
                    Courier.LogisticType = response.data[i].LogisticType;
                    Courier.LogisticDisplayType = response.data[i].LogisticTypeDisplay;
                    Courier.LogisticServiceId = response.data[i].LogisticServiceId;
                    Courier.RateType = response.data[i].RateType;
                    Courier.RateTypeDisplay = response.data[i].RateType;
                    LogisticDetail3.CourierDetail.push(Courier);
                }
                else if (response.data[i].LogisticCompany === 'UPS' && $scope.OperationZoneId === 1) {
                    LogisticDetail4.LogisticCompany = response.data[i].LogisticCompany;
                    LogisticDetail4.LogisticCompanyDisplay = response.data[i].LogisticCompanyDisplay;
                    Courier.LogisticType = response.data[i].LogisticType;
                    Courier.LogisticDisplayType = response.data[i].LogisticTypeDisplay;
                    Courier.LogisticServiceId = response.data[i].LogisticServiceId;
                    Courier.RateType = response.data[i].RateType;
                    Courier.RateTypeDisplay = response.data[i].RateTypeDisplay;
                    LogisticDetail4.CourierDetail.push(Courier);
                }
                else if (response.data[i].LogisticCompany === 'TNT' && $scope.OperationZoneId === 2) {
                    LogisticDetail3.LogisticCompany = response.data[i].LogisticCompany;
                    LogisticDetail3.LogisticCompanyDisplay = response.data[i].LogisticCompanyDisplay;
                    Courier.LogisticType = response.data[i].LogisticType;
                    Courier.LogisticDisplayType = response.data[i].LogisticTypeDisplay;
                    Courier.LogisticServiceId = response.data[i].LogisticServiceId;
                    Courier.RateType = response.data[i].RateType;
                    Courier.RateTypeDisplay = response.data[i].RateType;
                    LogisticDetail3.CourierDetail.push(Courier);
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
                else if (response.data[i].LogisticCompany === 'DPD' && $scope.OperationZoneId === 2) {
                    LogisticDetail5.LogisticCompany = response.data[i].LogisticCompany;
                    LogisticDetail5.LogisticCompanyDisplay = response.data[i].LogisticCompanyDisplay;
                    Courier.LogisticType = response.data[i].LogisticType;
                    Courier.LogisticDisplayType = response.data[i].LogisticTypeDisplay;
                    Courier.LogisticServiceId = response.data[i].LogisticServiceId;
                    Courier.RateType = response.data[i].RateType;
                    Courier.RateTypeDisplay = response.data[i].RateType;
                    LogisticDetail5.CourierDetail.push(Courier);
                }
                else if ((response.data[i].LogisticCompany === 'SKYPOSTAL' && $scope.OperationZoneId === 1) ||
                    (response.data[i].LogisticCompany === 'SKYPOSTAL' && $scope.OperationZoneId === 2)) {
                        LogisticDetail6.LogisticCompany = response.data[i].LogisticCompany;
                        LogisticDetail6.LogisticCompanyDisplay = response.data[i].LogisticCompanyDisplay;
                        Courier.LogisticType = response.data[i].LogisticType;
                        Courier.LogisticDisplayType = response.data[i].LogisticTypeDisplay;
                        Courier.LogisticServiceId = response.data[i].LogisticServiceId;
                        Courier.RateType = response.data[i].RateType;
                        Courier.RateTypeDisplay = response.data[i].RateType;
                        LogisticDetail6.CourierDetail.push(Courier);
                }
                else if ((response.data[i].LogisticCompany === 'AU' && response.data[i].LogisticType === 'AUShipment' && $scope.OperationZoneId === 1) ||
                    (response.data[i].LogisticCompany === 'AU' && response.data[i].LogisticType === 'AUShipment' && $scope.OperationZoneId === 2)) {
                        LogisticDetail7.LogisticCompany = response.data[i].LogisticCompany;
                        LogisticDetail7.LogisticCompanyDisplay = response.data[i].LogisticCompanyDisplay;
                        Courier.LogisticType = response.data[i].LogisticType;
                        Courier.LogisticDisplayType = response.data[i].LogisticTypeDisplay;
                        Courier.LogisticServiceId = response.data[i].LogisticServiceId;
                        Courier.RateType = response.data[i].RateType;
                        Courier.RateTypeDisplay = response.data[i].RateType;
                        LogisticDetail7.CourierDetail.push(Courier);
                }
                else if ((response.data[i].LogisticCompany === 'EAM' && $scope.OperationZoneId === 1) ||
                    (response.data[i].LogisticCompany === 'EAM' && $scope.OperationZoneId === 2)) {
                        LogisticDetail8.LogisticCompany = response.data[i].LogisticCompany;
                        LogisticDetail8.LogisticCompanyDisplay = response.data[i].LogisticCompany;
                        Courier.LogisticType = response.data[i].LogisticType;
                        Courier.LogisticDisplayType = response.data[i].LogisticTypeDisplay;
                        Courier.LogisticServiceId = response.data[i].LogisticServiceId;
                        Courier.RateType = response.data[i].RateType;
                        Courier.RateTypeDisplay = response.data[i].RateType;
                        LogisticDetail8.CourierDetail.push(Courier);
                }
                else if ((response.data[i].LogisticCompany === 'EAM-DHL' && $scope.OperationZoneId === 1) ||
                    (response.data[i].LogisticCompany === 'EAM-DHL' && $scope.OperationZoneId === 2)) {
                        LogisticDetail9.LogisticCompany = response.data[i].LogisticCompany;
                        LogisticDetail9.LogisticCompanyDisplay = response.data[i].LogisticCompanyDisplay;
                        Courier.LogisticType = response.data[i].LogisticType;
                        Courier.LogisticDisplayType = response.data[i].LogisticTypeDisplay;
                        Courier.LogisticServiceId = response.data[i].LogisticServiceId;
                        Courier.RateType = response.data[i].RateType;
                        Courier.RateTypeDisplay = response.data[i].RateType;
                        LogisticDetail9.CourierDetail.push(Courier);
                }
                else if ((response.data[i].LogisticCompany === 'EAM-TNT' && $scope.OperationZoneId === 1) ||
                    (response.data[i].LogisticCompany === 'EAM-TNT' && $scope.OperationZoneId === 2)) {
                        LogisticDetail10.LogisticCompany = response.data[i].LogisticCompany;
                        LogisticDetail10.LogisticCompanyDisplay = response.data[i].LogisticCompanyDisplay;
                        Courier.LogisticType = response.data[i].LogisticType;
                        Courier.LogisticDisplayType = response.data[i].LogisticTypeDisplay;
                        Courier.LogisticServiceId = response.data[i].LogisticServiceId;
                        Courier.RateType = response.data[i].RateType;
                        Courier.RateTypeDisplay = response.data[i].RateType;
                        LogisticDetail10.CourierDetail.push(Courier);
                }
                else if ((response.data[i].LogisticCompany === 'EAM-FedEx' && $scope.OperationZoneId === 1) ||
                    (response.data[i].LogisticCompany === 'EAM-FedEx' && $scope.OperationZoneId === 2)) {
                        LogisticDetail11.LogisticCompany = response.data[i].LogisticCompany;
                        LogisticDetail11.LogisticCompanyDisplay = response.data[i].LogisticCompanyDisplay;
                        Courier.LogisticType = response.data[i].LogisticType;
                        Courier.LogisticDisplayType = response.data[i].LogisticTypeDisplay;
                        Courier.LogisticServiceId = response.data[i].LogisticServiceId;
                        Courier.RateType = response.data[i].RateType;
                        Courier.RateTypeDisplay = response.data[i].RateType;
                        LogisticDetail11.CourierDetail.push(Courier);
                }
                else if ((response.data[i].LogisticCompany === 'BRING' && $scope.OperationZoneId === 1) ||
                    (response.data[i].LogisticCompany === 'BRING' && $scope.OperationZoneId === 2)) {
                        LogisticDetail12.LogisticCompany = response.data[i].LogisticCompany;
                        LogisticDetail12.LogisticCompanyDisplay = response.data[i].LogisticCompanyDisplay;
                        Courier.LogisticType = response.data[i].LogisticType;
                        Courier.LogisticDisplayType = response.data[i].LogisticTypeDisplay;
                        Courier.LogisticServiceId = response.data[i].LogisticServiceId;
                        Courier.RateType = response.data[i].RateType;
                        Courier.RateTypeDisplay = response.data[i].RateType;
                        LogisticDetail12.CourierDetail.push(Courier);
                }
                else if ((response.data[i].LogisticCompany === 'CANADAPOST' && $scope.OperationZoneId === 1) ||
                    (response.data[i].LogisticCompany === 'CANADAPOST' && $scope.OperationZoneId === 2)) {
                        LogisticDetail13.LogisticCompany = response.data[i].LogisticCompany;
                        LogisticDetail13.LogisticCompanyDisplay = response.data[i].LogisticCompanyDisplay;
                        Courier.LogisticType = response.data[i].LogisticType;
                        Courier.LogisticDisplayType = response.data[i].LogisticTypeDisplay;
                        Courier.LogisticServiceId = response.data[i].LogisticServiceId;
                        Courier.RateType = response.data[i].RateType;
                        Courier.RateTypeDisplay = response.data[i].RateType;
                        LogisticDetail13.CourierDetail.push(Courier);
                }
                else if ((response.data[i].LogisticCompany === 'DPDCH' && $scope.OperationZoneId === 1) ||
                    (response.data[i].LogisticCompany === 'DPDCH' && $scope.OperationZoneId === 2)) {
                        LogisticDetail14.LogisticCompany = response.data[i].LogisticCompany;
                        LogisticDetail14.LogisticCompanyDisplay = response.data[i].LogisticCompanyDisplay;
                        Courier.LogisticType = response.data[i].LogisticType;
                        Courier.LogisticDisplayType = response.data[i].LogisticTypeDisplay;
                        Courier.LogisticServiceId = response.data[i].LogisticServiceId;
                        Courier.RateType = response.data[i].RateType;
                        Courier.RateTypeDisplay = response.data[i].RateType;
                        LogisticDetail14.CourierDetail.push(Courier);
                }

                if (i === 4 && $scope.OperationZoneId === 2) {
                    $scope.LogisticServiceDetail.push(LogisticDetail1);
                    $scope.LogisticServiceDetail.push(LogisticDetail2);
                    $scope.LogisticServiceDetail.push(LogisticDetail3);
                    LogisticDetail = {
                        LogisticServiceId: 0,
                        OperationZoneId: 0,
                        LogisticCompany: '',
                        LogisticCompanyDisplay: '',

                        CourierDetail: []
                    };
                }
                if (i === 2 && response.data[i].LogisticCompany === 'DHL' && $scope.OperationZoneId === 1) {
                    $scope.LogisticServiceDetail.push(LogisticDetail1);
                    LogisticDetail = {
                        LogisticServiceId: 0,
                        OperationZoneId: 0,
                        LogisticCompany: '',
                        LogisticCompanyDisplay: '',

                        CourierDetail: []
                    };
                }
                else if (i === 4 && response.data[i].LogisticCompany === 'TNT' && $scope.OperationZoneId === 1) {
                    $scope.LogisticServiceDetail.push(LogisticDetail3);
                    LogisticDetail = {
                        LogisticServiceId: 0,
                        OperationZoneId: 0,
                        LogisticCompany: '',
                        LogisticCompanyDisplay: '',

                        CourierDetail: []
                    };
                }
                else if (i === 7 && response.data[i].LogisticCompany === 'UPS' && $scope.OperationZoneId === 1) {
                    $scope.LogisticServiceDetail.push(LogisticDetail4);
                    LogisticDetail = {
                        LogisticServiceId: 0,
                        OperationZoneId: 0,
                        LogisticCompany: '',
                        LogisticCompanyDisplay: '',

                        CourierDetail: []
                    };
                }
                else if ((i === 13 && response.data[i].LogisticCompany === 'DPD' && $scope.OperationZoneId === 2) ||
                    (i === 26 && response.data[i].LogisticCompany === 'DPD' && $scope.OperationZoneId === 1) ||
                    (i === 13 && response.data[i].LogisticCompany === 'AU' && $scope.OperationZoneId === 1)) {
                    $scope.LogisticServiceDetail.push(LogisticDetail5);
                    LogisticDetail = {
                        LogisticServiceId: 0,
                        OperationZoneId: 0,
                        LogisticCompany: '',
                        LogisticCompanyDisplay: '',

                        CourierDetail: []
                    };
                }
                else if ((i === 15 && response.data[i].LogisticCompany === 'AU' && $scope.OperationZoneId === 2) ||
                    (i === 17 && response.data[i].LogisticCompany === 'AU' && $scope.OperationZoneId === 1)) {
                    $scope.LogisticServiceDetail.push(LogisticDetail7);
                    LogisticDetail = {
                        LogisticServiceId: 0,
                        OperationZoneId: 0,
                        LogisticCompany: '',
                        LogisticCompanyDisplay: '',

                        CourierDetail: []
                    };
                }
                else if ((i === 16 && response.data[i].LogisticCompany === 'SKYPOSTAL' && $scope.OperationZoneId === 2) ||
                    (i === 18 && response.data[i].LogisticCompany === 'SKYPOSTAL' && $scope.OperationZoneId === 1) ||
                    ((i === 14 && response.data[i].LogisticCompany === 'SKYPOSTAL' && $scope.OperationZoneId === 1))) {
                    $scope.LogisticServiceDetail.push(LogisticDetail6);
                    LogisticDetail = {
                        LogisticServiceId: 0,
                        OperationZoneId: 0,
                        LogisticCompany: '',
                        LogisticCompanyDisplay: '',

                        CourierDetail: []
                    };
                }
                else if ((i === 19 && response.data[i].LogisticCompany === 'EAM-DHL' && $scope.OperationZoneId === 2) ||
                    (i === 21 && response.data[i].LogisticCompany === 'EAM-DHL' && $scope.OperationZoneId === 1) ||
                    ((i === 17 && response.data[i].LogisticCompany === 'EAM-DHL' && $scope.OperationZoneId === 1))) {
                    $scope.LogisticServiceDetail.push(LogisticDetail9);
                    LogisticDetail = {
                        LogisticServiceId: 0,
                        OperationZoneId: 0,
                        LogisticCompany: '',
                        LogisticCompanyDisplay: '',

                        CourierDetail: []
                    };
                }
                else if ((i === 20 && response.data[i].LogisticCompany === 'EAM-TNT' && $scope.OperationZoneId === 2) ||
                    (i === 22 && response.data[i].LogisticCompany === 'EAM-TNT' && $scope.OperationZoneId === 1) ||
                    ((i === 18 && response.data[i].LogisticCompany === 'EAM-TNT' && $scope.OperationZoneId === 1))) {
                    $scope.LogisticServiceDetail.push(LogisticDetail10);
                    LogisticDetail = {
                        LogisticServiceId: 0,
                        OperationZoneId: 0,
                        LogisticCompany: '',
                        LogisticCompanyDisplay: '',

                        CourierDetail: []
                    };
                }
                else if ((i === 21 && response.data[i].LogisticCompany === 'EAM-FedEx' && $scope.OperationZoneId === 2) ||
                   (i === 23 && response.data[i].LogisticCompany === 'EAM-FedEx' && $scope.OperationZoneId === 1) ||
                    ((i === 19 && response.data[i].LogisticCompany === 'EAM-FedEx' && $scope.OperationZoneId === 1))) {
                    $scope.LogisticServiceDetail.push(LogisticDetail11);
                    LogisticDetail = {
                        LogisticServiceId: 0,
                        OperationZoneId: 0,
                        LogisticCompany: '',
                        LogisticCompanyDisplay: '',

                        CourierDetail: []
                    };
                }
                else if ((i === 22 && response.data[i].LogisticCompany === 'EAM' && $scope.OperationZoneId === 2) ||
                   (i === 24 && response.data[i].LogisticCompany === 'EAM' && $scope.OperationZoneId === 1) ||
                    ((i === 20 && response.data[i].LogisticCompany === 'EAM' && $scope.OperationZoneId === 1))) {
                    $scope.LogisticServiceDetail.push(LogisticDetail8);
                    LogisticDetail = {
                        LogisticServiceId: 0,
                        OperationZoneId: 0,
                        LogisticCompany: '',
                        LogisticCompanyDisplay: '',

                        CourierDetail: []
                    };
                }
                else if ((i === 25 && response.data[i].LogisticCompany === 'BRING' && $scope.OperationZoneId === 1) ||
                    (i === 21 && response.data[i].LogisticCompany === 'BRING' && $scope.OperationZoneId === 1) ||
                    (i === 23 && response.data[i].LogisticCompany === 'BRING' && $scope.OperationZoneId === 2)) {
                    $scope.LogisticServiceDetail.push(LogisticDetail12);
                    LogisticDetail = {
                        LogisticServiceId: 0,
                        OperationZoneId: 0,
                        LogisticCompany: '',
                        LogisticCompanyDisplay: '',

                        CourierDetail: []
                    };
                }
                else if ((i === 27 && response.data[i].LogisticCompany === 'CANADAPOST' && $scope.OperationZoneId === 1) ||
                    (i === 23 && response.data[i].LogisticCompany === 'CANADAPOST' && $scope.OperationZoneId === 1) ||
                    (i === 25 && response.data[i].LogisticCompany === 'CANADAPOST' && $scope.OperationZoneId === 2)) {
                    $scope.LogisticServiceDetail.push(LogisticDetail13);
                    LogisticDetail = {
                        LogisticServiceId: 0,
                        OperationZoneId: 0,
                        LogisticCompany: '',
                        LogisticCompanyDisplay: '',

                        CourierDetail: []
                    };
                }
                else if ((i === 26 && response.data[i].LogisticCompany === 'DPDCH' && $scope.OperationZoneId === 1) ||
                    (i === 22 && response.data[i].LogisticCompany === 'DPDCH' && $scope.OperationZoneId === 1) ||
                    (i === 24 && response.data[i].LogisticCompany === 'DPDCH' && $scope.OperationZoneId === 2)) {
                    $scope.LogisticServiceDetail.push(LogisticDetail14);
                    LogisticDetail = {
                        LogisticServiceId: 0,
                        OperationZoneId: 0,
                        LogisticCompany: '',
                        LogisticCompanyDisplay: '',

                        CourierDetail: []
                    };
                }

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
                if (service.LogisticServiceId === 13 && service.LogisticType === 'Yodel' && service.IsSelected) {
                    $scope.IsYodel = true;
                }

                $scope.customerDetail.RegistredServices.push(service);
            }
            else {
                if (service.LogisticServiceId === 13 && service.LogisticType === 'Yodel' && service.IsSelected === false) {
                    $scope.IsYodel = false;
                }

                var registeredservices = $scope.customerDetail.RegistredServices;
                $scope.customerDetail.RegistredServices = [];
                for (i = 0; i < registeredservices.length; i++) {
                    if (registeredservices[i].LogisticServiceId != 13) {
                        $scope.customerDetail.RegistredServices.push(registeredservices[i]);
                    }
                }

                CustomerService.RemoveCustomerLogistic(service.LogisticServiceId, $scope.UserId).then(function (response) {
                    if (response.status === 200 && response.data !== null && response.data.Status) {
                        for (var j = 0; j < $scope.customerDetail.RegistredServices.length ; j++) {
                            if (service.LogisticServiceId === $scope.customerDetail.RegistredServices[j].LogisticServiceId) {
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
            $scope.LogisticType = '';
            $scope.LogisticServiceType = '';
            for (var i = 0; i < customerDetail.RegistredServices.length; i++) {
                if (customerDetail.RegistredServices[i].LogisticServiceId === 13) {
                    $scope.LogisticType = customerDetail.RegistredServices[i].LogisticType;
                    $scope.LogisticServiceType = customerDetail.RegistredServices[i].LogisticServiceType;
                    if ($scope.YodelOptionService !== undefined && $scope.YodelOptionService !== null) {
                        if ($scope.LogisticServiceType !== $scope.YodelOptionService) {
                            customerDetail.RegistredServices[i].LogisticServiceType = $scope.YodelOptionService;
                        }
                    }
                }
            }

            if ($scope.RoleId === 1 || $scope.RoleId === 6) {
                if (customerDetail.CustomerRateSetting.ScheduleDate !== null && customerDetail.CustomerRateSetting.ScheduleDate !== undefined) {
                    customerDetail.CustomerRateSetting.ScheduleDate = TimeStringtoDateTime.ConvertString(customerDetail.CustomerRateSetting.ScheduleDate, customerDetail.CustomerRateSetting.ScheduleTime);
                }
            }
            else {
                customerDetail.CustomerRateSetting = null;
            }

            if ($scope.LogisticType === 'Yodel') {
                if ($scope.LogisticServiceType === undefined || $scope.LogisticServiceType === '' || $scope.LogisticServiceType === null) {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.Frayte_Warning,
                        body: $scope.YodelAddressService,
                        showCloseButton: true
                    });
                }
                else {
                    AppSpinner.showSpinnerTemplate($scope.SavingCustomerDetail, $scope.Template);
                    CustomerService.SaveCustomerRateCardDetail($scope.customerDetail).then(function (response) {
                        toaster.pop({
                            type: 'success',
                            title: $scope.Frayte_Success,
                            body: $scope.Customer_RateCard_Saved_Information_Successfully,
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
            }
            else {
                AppSpinner.showSpinnerTemplate($scope.SavingCustomerDetail, $scope.Template);
                CustomerService.SaveCustomerRateCardDetail($scope.customerDetail).then(function (response) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.Frayte_Success,
                        body: $scope.Customer_RateCard_Saved_Information_Successfully,
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
        $scope.OperationZone = { OperationZoneId: 0, OperationZoneName: "" };
        $scope.RoleId = sessionInfo.RoleId;
        $scope.CreatedBy = sessionInfo.EmployeeId;
        //Set browser scroll in top
        $location.hash('top');
        $anchorScroll();

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

        CustomerService.GetCustomerDetail($scope.UserId).then(function (response) {
            if (response.data) {
                $scope.OperationZoneId = response.data.OperationZoneId;
                getScreenInitails();
            }
        });

        setModalOptions();
        //CustomerService.GetOperationZones().then(function (response) {

        //    $scope.OperationZones = response.data;

        //    if (config.SITE_COUNTRY === "COM") {
        //        $scope.OperationZone = $scope.OperationZones[0];
        //    }
        //    else {
        //        $scope.OperationZone = $scope.OperationZones[1];
        //    }
        //    $scope.ChangeOperationZone();
        //    if (response.data) {
        //        getScreenInitails();
        //    }
        //});

    }

    init();
});