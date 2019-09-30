angular.module('ngApp.uploadShipment').controller('WithServiceReturnErrorsController', function ($scope, $uibModal, ShipmentData, ServiceType, config, $translate) {
    
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors',
            'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation',
            'PleaseCorrectValidationErrors', 'RateCardSave_Validation', 'Select_CourierAccount', 'THP_Matrix_Saved', 'DataSaved_Validation',
        'LoadingZonePostZipCode', 'Shipment_Is', 'Shipment_Are', 'Shipment', 'shipments']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.ShipmentIs = translations.Shipment_Is;
            $scope.ShipmentAre = translations.Shipment_Are;
            $scope.Shipment_ = translations.Shipment;
            $scope.Shipments_ = translations.shipments;


        });
    };

    var filterUnsuccessfullShipment = function () {
        $scope.UnsuccessFullShipmentErrors = {
            Address: [],
            Package: [],
            Custom: [],
            Service: [],
            ServiceError: [],
            MiscErrors: [],
            Miscellaneous: []
        };


        $scope.UnsuccessFullShipments = [];
        $scope.SuccessFullShipments = [];
        $scope.UnsuccessFullWithoutShipments = [];

        angular.forEach($scope.ShipmentData, function (shipment, key) {
            //for(var shipment in  $scope.ShipmentData){
            if (shipment.Error !== null && shipment.Error.Status === false && $scope.ServiceType === 'ECOMMERCE_SS') {
                $scope.UnsuccessFullShipments.push(shipment);
                if (shipment.Error.Address != null && shipment.Error.Address.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.Address.push(shipment.Error.Address);
                }
                if (shipment.Error.Package != null && shipment.Error.Package.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.Package.push(shipment.Error.Package);
                }
                if (shipment.Error.Custom != null && shipment.Error.Custom.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.Custom.push(shipment.Error.Custom);
                }
                if (shipment.Error.Service != null && shipment.Error.Service.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.Service.push(shipment.Error.Service);
                }
                if (shipment.Error.ServiceError != null && shipment.Error.ServiceError.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.MiscErrors.push(shipment.Error.ServiceError);
                }
                if (shipment.Error.MiscErrors != null && shipment.Error.MiscErrors.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.push(shipment.Error.MiscErrors);
                }
                if (shipment.Error.Miscellaneous != null && shipment.Error.Miscellaneous.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.Miscellaneous.push(shipment.Error.Miscellaneous);
                }
            }
            if (shipment.Error !== null && shipment.Error.Status === false && $scope.ServiceType === 'DirectBooking_SS') {
                $scope.UnsuccessFullShipments.push(shipment);
                if (shipment.Error.Address != null && shipment.Error.Address.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.Address.push(shipment.Error.Address);
                }
                if (shipment.Error.Package != null && shipment.Error.Package.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.Package.push(shipment.Error.Package);
                }
                if (shipment.Error.Custom != null && shipment.Error.Custom.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.Custom.push(shipment.Error.Custom);
                }
                if (shipment.Error.Service != null && shipment.Error.Service.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.Service.push(shipment.Error.Service);
                }
                if (shipment.Error.ServiceError != null && shipment.Error.ServiceError.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.MiscErrors.push(shipment.Error.ServiceError);
                }
                if (shipment.Error.MiscErrors != null && shipment.Error.MiscErrors.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.push(shipment.Error.MiscErrors);
                }
                if (shipment.Error.Miscellaneous != null && shipment.Error.Miscellaneous.length > 0) {
                    $scope.UnsuccessFullShipmentErrors.Miscellaneous.push(shipment.Error.Miscellaneous);
                }
            }
            else if (shipment.Errors !== null && shipment.Errors.length === 0 && $scope.ServiceType === 'ECOMMERCE_WS') {
                shipment.BookingStatusType = 'Successfull';
                $scope.SuccessFullShipments.push(shipment);
            }
            else if (shipment.Errors !== null && shipment.Errors.length > 0 && $scope.ServiceType === 'ECOMMERCE_WS') {
                 $scope.UnsuccessFullShipments.push(shipment);
            }
        });
        $scope.ShipmentDataLength = $scope.ShipmentData.length - $scope.UnsuccessFullShipments.length;
        
        if ($scope.ShipmentDataLength > 1) {
            $scope.shipment = $scope.ShipmentAre;
        }
        else {
            $scope.shipment = $scope.ShipmentIs;
        }

        if ($scope.ShipmentData.length > 1) {
            $scope.ship = $scope.Shipments_;
        }
        else {
            $scope.ship = $scope.Shipment_;
        }
    };
  


    function init() {
        $scope.UnsuccessFullShipments = [];
        $scope.ShipmentData = ShipmentData;
        $scope.ErrorList = ShipmentData;
        $scope.ServiceType = ServiceType;
        filterUnsuccessfullShipment();
        $scope.ImagePath = config.BUILD_URL;
        setModalOptions();
    }

    init();

});