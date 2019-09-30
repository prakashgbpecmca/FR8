angular.module('ngApp.quotationTools').factory('QuotationService', function ($http, config) {

    var SaveQuotation = function (quotationDetail) {
        return $http.post(config.SERVICE_URL + '/Quotation/SaveQuotation', quotationDetail);
    };

    var GetQuotationShipments = function (operationZoneId, UserId, CustomerId) {

        return $http.get(config.SERVICE_URL + '/Quotation/GetQuotationShipments', {
            params: {
                operationZoneId: operationZoneId,
                UserId: UserId,
                CustomerId: CustomerId
            }
        });
    };

    var GetTNTSupplemetoryInfo = function (LogisticServiceId) {
        return $http.get(config.SERVICE_URL + '/Quotation/GetTNTSupplemetoryInformation', {
            params: {
                LogisticServiceId: LogisticServiceId
            }
        });
    };

    var GenerateQuotationShipmentPdf = function (quotationDetail) {
        return $http.post(config.SERVICE_URL + '/Quotation/GenerateQuotationShipmentPdf', quotationDetail);
    };

    var SendQuotationMail = function (quotationEmailDetail) {
        return $http.post(config.SERVICE_URL + '/Quotation/SendQuotationMail', quotationEmailDetail);
    };

    var DownLoadRateCardReport = function (FileName) {
        return $http.post(config.SERVICE_URL + '/Customer/DownLoadRateCardReport', FileName);
    };

    var GetCustomerLogisticService = function (UserId) {
        return $http.get(config.SERVICE_URL + '/ViewBaseRateCard/GetCustomerLogisticService', {
            params: {
                UserId: UserId
            }
        });
    };

    var GetPlaceBookingLink = function (DirectShipmentDraftId, CallingType) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/GetShipmentDraftDetail', {
            params: {
                DirectShipmentDraftId: DirectShipmentDraftId,
                CallingType: CallingType
            }
        });
    };

    var SendViewPastQuotationMail = function (CustomerEmailDetail) {
        return $http.post(config.SERVICE_URL + '/Quotation/SendCustomerQuoteMail', CustomerEmailDetail);
    };

    var SendCustomerRateCardAsEmail = function (Customer) {
        return $http.post(config.SERVICE_URL + '/Customer/SendCustomerRateCardAsEmail', Customer);
    };

    var GetSalesRepresentiveEmail = function (UserId, RoleId) {
        return $http.get(config.SERVICE_URL + '/Quotation/GetSalesRepresentiveEmail', {
            params: {
                UserId: UserId,
                RoleId: RoleId
            }
        });
    };

    var GetQuotationValidity = function (QuotationShipmentId) {
        return $http.get(config.SERVICE_URL + '/Quotation/GetQuotationValidity', {
            params: {
                QuotationShipmentId: QuotationShipmentId
            }
        });
    };

    var CustomerAddressType = function (CustomerId) {
        return $http.get(config.SERVICE_URL + '/Quotation/GetCustomerAddressType', {
            params: {
                CustomerId: CustomerId
            }
        });
    };

    var EditQuotation = function (quotationDetail) {
        return $http.post(config.SERVICE_URL + '/Quotation/EditQuotation', quotationDetail);
    };

    var RemoveQuotation = function (QuotationShipmentId) {
        return $http.get(config.SERVICE_URL + '/Quotation/RemoveQuotation', {
            params: {
                QuotationShipmentId: QuotationShipmentId
            }
        });
    };

    var GetLogisticWeightLimit = function (directBookingFindService) {
        return $http.post(config.SERVICE_URL + '/Quotation/QuotationServices', directBookingFindService);
    };

    var GetCustomerLogisticServices = function (CustomerId) {
        return $http.get(config.SERVICE_URL + '/Quotation/GetCustomerLogisticServices', {
            params: {
                CustomerId: CustomerId
            }
        });
    };

    var GetSpecialCustomerCompany = function (UserId) {
        return $http.get(config.SERVICE_URL + '/Customer/CustomerCompanyDetail', {
            params: {
                UserId: UserId
            }
        });
    };

    return {
        SaveQuotation: SaveQuotation,
        GetQuotationShipments: GetQuotationShipments,
        GenerateQuotationShipmentPdf: GenerateQuotationShipmentPdf,
        SendQuotationMail: SendQuotationMail,
        DownLoadRateCardReport: DownLoadRateCardReport,
        GetCustomerLogisticService: GetCustomerLogisticService,
        GetPlaceBookingLink: GetPlaceBookingLink,
        SendViewPastQuotationMail: SendViewPastQuotationMail,
        SendCustomerRateCardAsEmail: SendCustomerRateCardAsEmail,
        GetSalesRepresentiveEmail: GetSalesRepresentiveEmail,
        GetQuotationValidity: GetQuotationValidity,
        CustomerAddressType: CustomerAddressType,
        GetTNTSupplemetoryInfo: GetTNTSupplemetoryInfo,
        EditQuotation: EditQuotation,
        RemoveQuotation: RemoveQuotation,
        GetLogisticWeightLimit: GetLogisticWeightLimit,
        GetCustomerLogisticServices: GetCustomerLogisticServices,
        GetSpecialCustomerCompany: GetSpecialCustomerCompany
    };
});