angular.module('ngApp.tradelaneShipments').controller("TradelaneShipmentsUpdateCustomizedMawbController", function ($scope, toaster, $translate, AppSpinner, $uibModalInstance, ShipmentId, TradelaneShipmentService) {

    $scope.Mawb = 'Update Customized MAWB';
    $scope.MawbDetails = 'Mawb Details';

    //Set Multilingual for Modal Popup
    var setMultilingualOtions = function () {
        $translate(['FrayteError', 'FrayteWarning', 'FrayteValidation', 'FrayteSuccess',
                    'Customized_MAWB_Pdf_Save', 'Customized_MAWB_Pdf_Save_Problem', 'Customized_MAWB_Pdf_Validation']).then(function (translations) {
                    $scope.TitleFrayteError = translations.FrayteError;
                    $scope.TitleFrayteWarning = translations.FrayteWarning;
                    $scope.TitleFrayteInformation = translations.FrayteSuccess;
                    $scope.TitleFrayteValidation = translations.FrayteValidation;
                    $scope.Customized_MAWB_Pdf_Save = translations.Customized_MAWB_Pdf_Save;
                    $scope.Customized_MAWB_Pdf_Save_Problem = translations.Customized_MAWB_Pdf_Save_Problem;
                    $scope.Customized_MAWB_Pdf_Validation = translations.Customized_MAWB_Pdf_Validation;
            });
    };

    $scope.mawbCustomizefield = {
        MAWBCustomizedeFieldId: 0,
        TradelaneShipmentId: 0,
        IssuingCarriersAgentNameandCity: null,
        DeclaredValueForCarriage: null,
        DeclaredValueForCustoms: null,
        ValuationCharge: null,
        Tax: null,
        TotalOtherChargesDueAgent: null,
        TotalOtherChargesDueCarrier: null,
        OtherCharges: null,
        ChargesAtDestination: null,
        TotalCollectCharges: null,
        CurrencyConversionRates: null,
        TotalPrepaid: null,
        TotalCollect: null,
        HandlingInformation: null,
        AgentsIATACode: null,
        AccountNo: null
    };

    $scope.UpdateMAWBCustomizedpdf = function (mawbCustomizefield, IsValid) {
        if (IsValid) {
            $scope.mawbCustomizefield.TradelaneShipmentId = $scope.TradeLaneShipmentId;
            TradelaneShipmentService.AddMAWBCustomized(mawbCustomizefield).then(function (response) {
                if (response.data !== undefined && response.data !== null && response.data !== '' && response.data === true) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteInformation,
                        body: $scope.Customized_MAWB_Pdf_Save,
                        showCloseButton: true
                    });
                    $uibModalInstance.close();
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.Customized_MAWB_Pdf_Save_Problem,
                        showCloseButton: true
                    });
                }
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.Customized_MAWB_Pdf_Validation,
                showCloseButton: true
            });
        }
    };

    function getinitials() {
        TradelaneShipmentService.GetMawbCustomizePdf($scope.TradeLaneShipmentId).then(function (response) {
            if (response.data !== undefined && response.data !== null && response.data !== '') {
                $scope.mawbCustomizefield = response.data;
            }
        });
    }

    function init() {
        $scope.TradeLaneShipmentId = ShipmentId;
        setMultilingualOtions();
        getinitials();
    }

    init();

});