angular.module('ngApp.eCommerce')
    .factory('eCommerceBookingService', function ($http, config, SessionService) {

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
        var SetPrintPackageStatus = function (eCommercePackageTrackingDetailId, Type) {
            return $http.get(config.SERVICE_URL + '/eCommerce/SetPrintPackageStatus', {
                params: {
                    eCommercePackageTrackingDetailId: eCommercePackageTrackingDetailId,
                    Type: Type
                }
            });
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

        var GeteCommerceWithServiceFormBookingDetailDraft = function (eCommerceShipmentId, callingType) {
            return $http.get(config.SERVICE_URL + '/eCommerce/GeteCommerceWithServiceFormBookingDetailDraft',
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

        var GetLabelFileName = function (eCommerceShipmentId, LabelType) {
            return $http.get(config.SERVICE_URL + '/eCommerce/GetLabelFile', {
                params: {
                    eCommerceShipmentId: eCommerceShipmentId,
                    LabelType: LabelType
                }
            });
        };

        var GeneratePacakgelabel = function (eCommerceShipmentId, Id, LabelType) {
            return $http.get(config.SERVICE_URL + '/eCommerce/GeneratePacakgelabel', {
                params: {
                    eCommerceShipmentId: eCommerceShipmentId,
                    Id: Id,
                    LabelType: LabelType
                }
            });
        };
        var CreateCommunication = function (invoiceCommunication) {
            return $http.post(config.SERVICE_URL + '/eCommerce/CreateCommunication', invoiceCommunication);
        };

        var GetCommunications = function (userId, shipmentId) {
            return $http.get(config.SERVICE_URL + '/eCommerce/GetCommunications', {
                params: {
                    userId: userId,
                    shipmentId: shipmentId
                }
            });
        };
        var SaveEmailCommunication = function (emailCommunication) {
            return $http.post(config.SERVICE_URL + '/eCommerce/SaveEmailCommunication', emailCommunication);
        };
        var GetEmailCommunication = function (userId, shipmentId) {
            return $http.get(config.SERVICE_URL + '/eCommerce/GetEmailCommunication', {
                params: {
                    userId: userId,
                    shipmentId: shipmentId
                }
            });
        };

        var InVoiceAndAcounting = function (userId, shipmentId) {
            return $http.get(config.SERVICE_URL + '/eCommerce/InVoiceAndAcounting', {
                params: {
                    userId: userId,
                    shipmentId: shipmentId
                }
            });
        };
        var AddUserCreditNote = function (creditNote) {
            return $http.post(config.SERVICE_URL + '/eCommerce/AddUserCreditNote', creditNote);
        };
        var GetCurrencies = function () {
            return $http.get(config.SERVICE_URL + '/eCommerce/GetCurrencies');
        };
        var GetManualTracking = function (shipmentId) {
            return $http.get(config.SERVICE_URL + '/eCommerce/GetManualTracking', {
                params: {
                    shipmentId: shipmentId
                }
            });
        };
        var SaveManualTracking = function (eCommerceTracking) {
            return $http.post(config.SERVICE_URL + '/eCommerce/SaveManualTracking', eCommerceTracking);
        };
        var GeteCommerceCustomers = function (userId) {
            return $http.get(config.SERVICE_URL + '/eCommerce/GeteCommerceCustomers', {
                params: {
                    userId: userId
                }
            });
        };
        return {
            SaveManualTracking: SaveManualTracking,
            GetManualTracking: GetManualTracking,
            GetCurrencies: GetCurrencies,
            AddUserCreditNote: AddUserCreditNote,
            InVoiceAndAcounting: InVoiceAndAcounting,
            GetEmailCommunication: GetEmailCommunication,
            SaveEmailCommunication: SaveEmailCommunication,
            GetCommunications: GetCommunications,
            CreateCommunication: CreateCommunication,
            SetPrintPackageStatus: SetPrintPackageStatus,
            GetInitials: GetInitials,
            SaveeCommerceBooking: SaveeCommerceBooking,
            GeteCommerceBookingDetail: GeteCommerceBookingDetail,
            GeteCommerceBookingDetailDraft: GeteCommerceBookingDetailDraft,
            DownLoadAsPdf: DownLoadAsPdf,
            DeleteeCommerceShipmentPackage: DeleteeCommerceShipmentPackage,
            eCommerceBookingDetailWithLabel: eCommerceBookingDetailWithLabel,
            SetShipmentHSCode: SetShipmentHSCode,
            GetLabelFileName: GetLabelFileName,
            GeneratePacakgelabel: GeneratePacakgelabel,
            GeteCommerceWithServiceFormBookingDetailDraft: GeteCommerceWithServiceFormBookingDetailDraft,
            GeteCommerceCustomers: GeteCommerceCustomers

        };

    })
.factory("PostCodeService", function ($http, config, DirectBookingService) {
    var AllPostCode = function (PostCode, CountryCode2) {

        //$scope.MaximamLength = 9;
        var pcode = PostCode.split('');

        for (i = 0; i < pcode.length; i++) {
            if (pcode[i] === " ") {
                pcode.splice(i, 1);
            }
        }

        if (pcode.length >= 0) {
            return DirectBookingService.GetPostCodeAddress(PostCode, CountryCode2).then(function (response) {
                if (response.data && response.data.length > 0) {
                    for (i = 0; i < response.data.length; i++) {
                        if (response.data[i].City === null || response.data[i].City === '') {
                            response.data[i].City = '';
                        }
                        if (response.data[i].Address1 === null || response.data[i].Address1 === '') {
                            response.data[i].Address1 = '';
                        }
                        if (response.data[i].Address2 === null || response.data[i].Address2 === '') {
                            response.data[i].Address2 = '';
                        }
                        if (response.data[i].CompanyName === null || response.data[i].CompanyName === '') {
                            response.data[i].CompanyName = '';
                        }
                        if (response.data[i].Area === null || response.data[i].Area === '') {
                            response.data[i].Area = '';
                        }
                        if (response.data[i].Address1 !== '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].Area !== '' && response.data[i].CompanyName !== '') {
                            response.data[i].FillPostCodeInput = response.data[i].CompanyName + ',' + response.data[i].Address1 + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '') {
                            response.data[i].FillPostCodeInput = (response.data[i].CompanyName ? response.data[i].CompanyName + ', ' : '') + response.data[i].Address2 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 !== '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].Area === '') {
                            response.data[i].FillPostCodeInput = response.data[i].CompanyName + ', ' + response.data[i].Address1 + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 !== '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '') {
                            response.data[i].FillPostCodeInput = (response.data[i].CompanyName ? response.data[i].CompanyName + ', ' : '') + response.data[i].Address1 + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 !== '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '' && response.data[i].Area === '') {
                            response.data[i].FillPostCodeInput = (response.data[i].CompanyName ? response.data[i].CompanyName + ', ' : '') + response.data[i].Address1 + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName !== '' && response.data[i].Area === '') {
                            response.data[i].FillPostCodeInput = response.data[i].CompanyName + ', ' + response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '' && response.data[i].Area !== '') {
                            response.data[i].FillPostCodeInput = (response.data[i].CompanyName ? response.data[i].CompanyName + ', ' : '') + response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 === '' && response.data[i].Address2 !== '' && response.data[i].City !== '' && response.data[i].PostCode !== '' && response.data[i].CompanyName === '' && response.data[i].Area === '') {
                            response.data[i].FillPostCodeInput = response.data[i].CompanyName + ',' + response.data[i].Address2 + ', ' + response.data[i].City + ',' + response.data[i].Area + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 !== '' && response.data[i].Address2 === '' && response.data[i].City !== '' && response.data[i].PostCode !== '') {
                            response.data[i].FillPostCodeInput = (response.data[i].CompanyName ? response.data[i].CompanyName + ', ' : '') + response.data[i].Address1 + ', ' + response.data[i].City + ', ' + response.data[i].PostCode;

                        }
                        if (response.data[i].Address1 === '' && response.data[i].Address2 === '' && response.data[i].City !== '' && response.data[i].PostCode !== '') {
                            response.data[i].FillPostCodeInput = (response.data[i].CompanyName ? response.data[i].CompanyName + ', ' : '') + response.data[i].City + ', ' + response.data[i].PostCode;
                        }
                    }
                    //$scope.PostCodeAddressValue = false;
                    //$scope.fillpostval = response.data;
                    return response.data;

                }
                else {
                    // $scope.PostCodeAddressValue = true;
                }
            });
        }




    };
    return {
        AllPostCode: AllPostCode
    };
});
