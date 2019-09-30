angular.module('ngApp.directBooking').factory('DirectBookingService', function ($http, config, SessionService) {

    var GetCountryDefaultAddress = function (countryId, userId, addressType) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/GetCountryDefaultAddress', {
            params: {
                countryId: countryId,
                userId: userId,
                addressType: addressType
            }
        });
    };

    var CancelShipment = function (DirectShipmentId) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/DirectBookingCancel',
            {
                params: {
                    DirectShipmentId: DirectShipmentId
                }
            });
    };

    var DeleteShipment = function (DirectShipmentId) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/DirectBookingDelete',
            {
                params: {
                    DirectShipmentId: DirectShipmentId
                }
            });
    };

    var GetInitials = function (customerId) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/GetInitials',
            {
                params: {
                    customerId: customerId
                }
            });
    };

    var GetServices = function (serviceRequest) {
        return $http.post(config.SERVICE_URL + '/DirectBooking/GetServices', serviceRequest);
    };

    var GetDirectBookingDetail = function (directShipmentId, CallingType) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/GetDirectBookingDetail',
            {
                params: {
                    directShipmentId: directShipmentId,
                    CallingType: CallingType
                }
            });
    };

    var GetShipmentDraftDetail = function (DirectShipmentDraftId, CallingType) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/GetShipmentDraftDetail',
            {
                params: {
                    DirectShipmentDraftId: DirectShipmentDraftId,
                    CallingType: CallingType
                }
            });
    };

    var SaveDirectBooking = function (directBookingDetail) {
        return $http.post(config.SERVICE_URL + '/DirectBooking/SaveDirectBooking', directBookingDetail);
    };

    var GetAdditionalSurcharge = function (service) {
        return $http.post(config.SERVICE_URL + '/DirectBooking/GetAdditionalSurcharge', service);
    };

    var GetCustomerAddressBook = function (TrackSearchText) {
        return $http.post(config.SERVICE_URL + '/DirectBooking/CustomerAddressAdvanceSearch', TrackSearchText
       );
    };

    var CustomerDefaultAddress = function (addressBookId, countryId, userId, addressType) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/CustomerDefaultAddress', {
            params: {
                addressBookId: addressBookId,
                countryId: countryId,
                userId: userId,
                addressType: addressType
            }
        });
    };

    var CustomerDefaultAddressBook = function (addressBookId, countryId, userId, value, addressType) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/CustomerDefaultAddressBook', {
            params: {
                addressBookId: addressBookId,
                countryId: countryId,
                userId: userId,
                value: value,
                addressType: addressType

            }
        });
    };

    var DownLoadAsPdf = function (DirectShipmentId, CourierCompany, RateType) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/PrintLabelAsPDF', {
            params: {
                DirectShipmentId: DirectShipmentId,
                CourierCompany: CourierCompany,
                RateType: RateType
            }
        });
    };

    var DirectBookingConfirmReject = function (CustomerAction, DirectShipmentId) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/DirectBookingReject', {
            params: {
                CustomerAction: CustomerAction,
                DirectShipmentId: DirectShipmentId
            }
        });
    };

    var DirectBookingDetailWithLabel = function (ToMail, DirectShipmentId, CourierName, RoleId, LogInUserId) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/DirectBookingDetailWithLabel', {
            params: {
                ToMail: ToMail,
                DirectShipmentId: DirectShipmentId,
                CourierName: CourierName,
                RoleId: RoleId,
                LogInUserId: LogInUserId
            }
        });
    };

    var GetDirectBookingCustomers = function (userId, moduleType) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/GetDirectBookingCustomers', {
            params: {
                userId: userId,
                moduleType: moduleType
            }
        });
    };

    var GetCustomerDefaultDetail = function (CustomerId) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/UserDefaultAddress', {
            params: {
                CustomerId: CustomerId
            }
        });
    };

    var GetCustomerDetail = function (CustomerId) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/GetCustomerDetail', {
            params: {
                CustomerId: CustomerId
            }
        });
    };

    var SetPrintPackageStatus = function (Package) {
        return $http.post(config.SERVICE_URL + '/DirectBooking/SetPrintPackageStatus', Package);
    };

    var DeleteDirectShipmentPackage = function (DirectShipmentDetailId) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/DeletePcsDetail', {
            params: {
                DirectShipmentDetailId: DirectShipmentDetailId
            }
        });
    };

    var EraseAllCustomerAddress = function (CustomerId, AddressType) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/EraseAllCustomerAddress', {
            params: {
                CustomerId: CustomerId,
                AddressType: AddressType
            }
        });
    };

    var DeleteCustomerAddress = function (AddressId, TableType) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/DeleteCustomerAddress', {
            params: {
                AddressId: AddressId,
                TableType: TableType
            }
        });
    };

    var SaveAddressBook = function (FrayteAddressBook) {
        return $http.post(config.SERVICE_URL + '/DirectBooking/EditCustomerAddress', FrayteAddressBook);
    };

    var MarkAddressAsFavourite = function (AddressBookId, IsFavorites) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/SetAddressFavourite', {
            params: {
                AddressBookId: AddressBookId,
                IsFavorites: IsFavorites
            }
        });
    };

    var GetPostCodeAddress = function (PostCode, CountryCode) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/GetPostCodeAddress', {
            params: {
                PostCode: PostCode,
                CountryCode: CountryCode
            }
        });
    };

    var GetCustomerLogisticService = function (CustomerId) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/CustomerLogisticService', {
            params: {
                CustomerId: CustomerId
            }
        });
    };

    var GetCountryStateName = function (CountryId) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/GetCountryStateCode', {
            params: {
                CountryId: CountryId
            }
        });
    };

    var GetCountryCurrentDateTime = function (CountryId) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/CountryCurrentDateTime', {
            params: {
                CountryId: CountryId
            }
        });
    };

    return {
        GetCountryDefaultAddress: GetCountryDefaultAddress,
        GetInitials: GetInitials,
        GetServices: GetServices,
        GetDirectBookingDetail: GetDirectBookingDetail,
        SaveDirectBooking: SaveDirectBooking,
        GetCustomerAddressBook: GetCustomerAddressBook,
        CustomerDefaultAddress: CustomerDefaultAddress,
        CustomerDefaultAddressBook: CustomerDefaultAddressBook,
        DownLoadAsPdf: DownLoadAsPdf,
        CancelShipment: CancelShipment,
        DirectBookingConfirmReject: DirectBookingConfirmReject,
        DirectBookingDetailWithLabel: DirectBookingDetailWithLabel,
        SetPrintPackageStatus: SetPrintPackageStatus,
        GetDirectBookingCustomers: GetDirectBookingCustomers,
        GetCustomerDetail: GetCustomerDetail,
        GetCustomerDefaultDetail: GetCustomerDefaultDetail,
        DeleteDirectShipmentPackage: DeleteDirectShipmentPackage,
        SaveAddressBook: SaveAddressBook,
        EraseAllCustomerAddress: EraseAllCustomerAddress,
        DeleteCustomerAddress: DeleteCustomerAddress,
        GetShipmentDraftDetail: GetShipmentDraftDetail,
        GetAdditionalSurcharge: GetAdditionalSurcharge,
        GetPostCodeAddress: GetPostCodeAddress,
        GetCustomerLogisticService: GetCustomerLogisticService,
        GetCountryStateName: GetCountryStateName,
        MarkAddressAsFavourite: MarkAddressAsFavourite,
        DeleteShipment: DeleteShipment,
        GetCountryCurrentDateTime: GetCountryCurrentDateTime
    };

});