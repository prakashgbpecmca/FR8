angular.module('ngApp.directBooking').factory('eCommerceBookingService', function ($http, config, SessionService) {

    var GetInitials = function (customerId) {
        return $http.get(config.SERVICE_URL + '/DirectBooking/GetInitials',
            {
                params: {
                    customerId: customerId
                }
            });
    };
    var SaveeCommerceBooking = function (eCommerceBookingDetail) {
        return $http.post(config.SERVICE_URL + '/eCommerce/SaveBooking', eCommerceBookingDetail);
    };
    var SetShipmentHSCode = function (eCommerceShipmentDetailid, HSCode) {
        return $http.get(config.SERVICE_URL + '/eCommerce/SetShipmentHSCode',
          {
              params: {
                  eCommerceShipmentDetailid: eCommerceShipmentDetailid,
                  HSCode: HSCode
              }
          });
    };
    var GeteCommerceBookingDetail = function (eCommerceShipmentId, callingType) {
        return $http.get(config.SERVICE_URL + '/eCommerce/GeteCommerceBookingDetail',
                {
                    params: {
                        eCommerceShipmentId: eCommerceShipmentId,
                        CallingType: callingType
                    }
                });
    };
    var DownLoadAsPdf = function (eCommerceShipmentId, CourierCompany) {
        return $http.get(config.SERVICE_URL + '/eCommerce/PrintLabelAsPDF',
    {
        params: {
            eCommerceShipmentId: eCommerceShipmentId,
            CourierCompany: CourierCompany
        }
    });
    };
    var SetPrintPackageStatus = function (package) {
        return $http.post(config.SERVICE_URL + '/eCommerce/SetPrintPackageStatus', package);
    };
    var GeteCommerceBookingDetailDraft = function (eCommerceShipmentId, callingType) {
        return $http.get(config.SERVICE_URL + '/eCommerce/GeteCommerceBookingDetailDraft',
            {
                params: {
                    DirectShipmentDraftId: eCommerceShipmentId,
                    CallingType: callingType
                }
            });
    };

    var DeleteeCommerceShipmentPackage = function (eCommerceShipmentDetailDraftId) {
        return $http.get(config.SERVICE_URL + '/eCommerce/DeleteeCommerceShipmentPackage',
           {
               params: {
                   eCommerceShipmentDetailDraftId: eCommerceShipmentDetailDraftId
               }
           });
    };

    var eCommerceBookingDetailWithLabel = function (ToMail, eCommerceShipmentId, CourierName) {
        return $http.get(config.SERVICE_URL + '/eCommerce/eCommerceDetailWithLabel', {
            params: {
                ToMail: ToMail,
                eCommerceShipmentId: eCommerceShipmentId,
                CourierName: CourierName
            }
        });
    };
    return {
        SetPrintPackageStatus: SetPrintPackageStatus,
        GetInitials: GetInitials,
        SaveeCommerceBooking: SaveeCommerceBooking,
        GeteCommerceBookingDetail: GeteCommerceBookingDetail,
        GeteCommerceBookingDetailDraft: GeteCommerceBookingDetailDraft,
        DownLoadAsPdf: DownLoadAsPdf,
        DeleteeCommerceShipmentPackage: DeleteeCommerceShipmentPackage,
        eCommerceBookingDetailWithLabel: eCommerceBookingDetailWithLabel,
        SetShipmentHSCode: SetShipmentHSCode

    };

});