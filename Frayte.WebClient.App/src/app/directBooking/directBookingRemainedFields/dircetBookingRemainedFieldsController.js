angular.module('ngApp.directBooking').controller('DirectBookingRemainedFieldsController', function ($scope, toaster, $translate, Upload, $uibModalInstance, uiGridConstants, DownloadExcelService, config, $state, SessionService, UploadShipmentService, $uibModal, ShipmentData, DirectBookingService, TopCurrencyService, $uibModalStack, $rootScope, ErrorFormShow, CountryPhoneCodes) {


    $scope.GetErrorsForShipments = function (Shipment) {

        $scope.ErrorList = [];
        $scope.Shipment = [];
        $scope.ErrorFormShow = ErrorFormShow;
        $scope.Shipment.push(angular.copy(Shipment));
        $scope.ErrorList.push(Shipment);

    };

    // Set Country Phone Code
    $scope.SetShipFrominfo = function (Country, Action) {

        if (Country !== undefined && Country !== null && Action === "Country") {

            if (Country.Code === 'GBR') {
                $scope.MaximamLengthShipFrom = 9;
            }
            else {
                $scope.MaximamLengthShipFrom = null;
            }

            $scope.showPostCodeDropDown = false;
            for (var i = 0 ; i < $scope.CountryPhoneCodes.length ; i++) {
                if ($scope.CountryPhoneCodes[i].CountryCode === Country.Code) {
                    $scope.ShipFromPhoneCode = "(+" + $scope.CountryPhoneCodes[i].PhoneCode + ")";
                    $scope.PhoneCode.ShipFromPhoneCode = "(+" + $scope.CountryPhoneCodes[i].PhoneCode + ")";
                    break;
                }
            }

            //setShipFromStatePostCodeForHKGUK(Country);
            //if (Action !== undefined && Action !== null && Action !== '') {
            //    //$scope.ShipFromPhoneCode = '';
            //    $scope.directBooking.ShipFrom = {
            //        AddressBookId: 0,
            //        Country: Country,
            //        PostCode: "",
            //        FirstName: "",
            //        LastName: "",
            //        CompanyName: "",
            //        Address: "",
            //        Address2: "",
            //        City: "",
            //        State: "",
            //        Area: "",
            //        Phone: "",
            //        Email: "",
            //        IsMailSend: false
            //    };
            //}

        }

    };

    // Set ShipTo info
    $scope.SetShipToInfo = function (Country, Action) {
        if (Country !== undefined && Country !== null) {
            if (Country.Code === 'GBR') {
                $scope.MaximamLengthShipTo = 9;
            }
            else {
                $scope.MaximamLengthShipTo = null;
            }
            $scope.showPostCodeDropDown1 = false;
            for (var i = 0 ; i < $scope.CountryPhoneCodes.length ; i++) {
                if ($scope.CountryPhoneCodes[i].Name === Country.Name) {
                    $scope.ShipToPhoneCode = "(+" + $scope.CountryPhoneCodes[i].PhoneCode + ")";
                    $scope.PhoneCode.ShipToPhoneCode = "(+" + $scope.CountryPhoneCodes[i].PhoneCode + ")";
                    break;
                }
            }
            //setShipToStatePostCodeForHKGUK(Country);
        }
        //if (Action !== undefined && Action !== null && Action !== '') {
        //    //$scope.ShipToPhoneCode = '';
        //    $scope.directBooking.ShipTo = {
        //        AddressBookId: 0,
        //        Country: Country,
        //        PostCode: "",
        //        FirstName: "",
        //        LastName: "",
        //        CompanyName: "",
        //        Address: "",
        //        Address2: "",
        //        City: "",
        //        State: "",
        //        Area: "",
        //        Phone: "",
        //        Email: "",
        //        IsMailSend: false
        //    };

        //}
    };

    //var setShipFromStatePostCodeForHKGUK = function (Country) {
    //    if (Country.Code === 'HKG') {
    //        $scope.directBooking.ShipFrom.PostCode = null;
    //        $scope.directBooking.ShipFrom.State = null;
    //    }
    //    else if (Country.Code === 'GBR') {
    //        $scope.directBooking.ShipFrom.State = null;
    //    }
    //};

    //var setShipToStatePostCodeForHKGUK = function (Country) {
    //    if (Country.Code === 'HKG') {
    //        $scope.directBooking.ShipTo.PostCode = null;
    //        $scope.directBooking.ShipTo.State = null;
    //    }
    //    else if (Country.Code === 'GBR') {
    //        $scope.directBooking.ShipTo.State = null;
    //    }
    //};

    // set translation key

    var setMultilingualOptions = function () {
        $translate(['FrayteWarning', 'FrayteError', 'FrayteSuccess', 'PleaseFillMandatoryField', 'Shipment_Created_Successfully']).then(function (translations) {
            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.FrayteError = translations.FrayteError;
            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.PleaseFillMandatoryField = translations.PleaseFillMandatoryField;
            $scope.Shipment_Created_Successfully = translations.Shipment_Created_Successfully;
        });
    };



    $scope.Shipment1 = function (DirectBooking, directbooking) {
        if (DirectBooking.$valid) {
            $scope.vk = directbooking[0];
            UploadShipmentService.SaveShipmentFromErrorPopup($scope.vk).then(function (response) {
                var ab = response.data;
                toaster.pop({
                    type: 'success',
                    title: $scope.FrayteSuccess,
                    body: $scope.Shipment_Created_Successfully,
                    showCloseButton: true
                });
                $uibModalInstance.close('testParameter');

            }, function () {
                toaster.pop({
                    type: 'warning',
                    title: $scope.FrayteWarning,
                    body: $scope.PleaseFillMandatoryField,
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.PleaseFillMandatoryField,
                showCloseButton: true
            });
        }
    };

    $scope.Makeshipment = function (gridDirectBooking) {
        $uibModalInstance.close($scope.Shipment[0]);
    };

    //    var modalInstance = $uibModal.open({
    //        animation: true,
    //        templateUrl: 'uploadShipment/uploadShipmentEcommForm/uploadShipmentForm.tpl.html',
    //        controller: 'uploadeCommerceBookingController',
    //        windowClass: 'DirectBookingDetail',
    //        size: 'lg',
    //        resolve: {
    //            ShipmentId: function () {
    //                return gridDirectBooking.ShipmentId;
    //            },
    //            TrackingNo: function () {
    //                return gridDirectBooking.TrackingNo;
    //            },
    //            CourierCompany: function () {
    //                return gridDirectBooking.DisplayName;
    //            }
    //        }
    //    });
    //    modalInstance.result.then(function () {
    //        $uibModalInstance.close('testParameter');
    //    }, function () {
    //    });
    //};

    //$scope.DropDownBinding = function (er) {

    //    if (er.FileType === "DropDown" && er.FieldName === "ParcelType") {
    //        $scope.GeneralDropDownSource = $scope.ParcelTypes;
    //        $scope.Par = 'ParcelType.ParcelDescription';
    //        $scope.TrackBYOBJ = 'ParcelType.ParcelType';
    //        return true;
    //    }
    //    if (er.FileType === "DropDown" && er.FieldName === "Currency") {
    //        //$scope.GeneralDropDownSource = $scope.CurrencyTypes;
    //        //$scope.Par = 'ParcelType.CurrencyCode';
    //        //$scope.vm = 'ParcelType.CurrencyDescription';
    //        //$scope.str = "+ ' - ' +";
    //        //$scope.TrackBYOBJ = 'ParcelType.CurrencyCode';
    //        return true;
    //    }

    //};
    //$scope.SetCollectionTime = function (CollectionTime) {
    //    $scope.Shipment[0].ReferenceDetail.CollectionTime = CollectionTime;
    //};
    //$scope.$watch('CollectionTime', function (CollectionTime) {
    //    $scope.Shipment[0].ReferenceDetail.CollectionTime = CollectionTime;
    //});
    $scope.Makeshipment1 = function (gridDirectBooking) {
        //$scope.Shipment[0].ReferenceDetail.CollectionTime = CollectionTime;
        //$uibModalInstance.close($scope.PhoneCode, $scope.Shipment[0]);
        $uibModalInstance.close($scope.Shipment[0]);
    };

    $scope.OpenCalender = function ($event) {
        $scope.status.opened = true;
    };

    $scope.FormSave = function (DBObj, SaveType) {
        if (SaveType === "Save") {
            
        }
        else {
            $scope.Shipment[0] = $scope.ShipmentNewData;
        }
        $uibModalInstance.close($scope.Shipment[0]);
    };

    function init() {
        $scope.PhoneCode = {
            ShipFromPhoneCode: "",
            ShipToPhoneCode: ""
        };

        $scope.str = '';
        $scope.vm = '';
        $scope.Shipment = [{
            ShipFrom: {},
            ShipTo: {}
        }];
        $scope.status = {
            opened: false
        };
        $scope.dateOptions1 = {
            formatYear: 'yy',
            startingDay: 1,
            minDate: new Date()
        };
        $scope.CountryPhoneCodes = CountryPhoneCodes;
        $rootScope.FormName = "DBForm";
        $scope.vk = {};
        $scope.submitted = true;
        var userInfo = SessionService.getUser();
        $scope.customerId = userInfo.EmployeeId;
        $scope.ShipmentData = ShipmentData;
        $scope.ShipmentNewData = angular.copy(ShipmentData);
        $scope.GetErrorsForShipments($scope.ShipmentData);
        setMultilingualOptions();
    }

    init();

});