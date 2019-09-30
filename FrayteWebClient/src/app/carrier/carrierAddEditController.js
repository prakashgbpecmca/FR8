/**
 * Controller
 */
angular.module('ngApp.carrier').controller('CarrierAddEditController', function ($scope, $location,$translate, $filter, LogonService, SessionService, CarrierService, $uibModal, $uibModalInstance, carriers, mode, carrier, $log, toaster) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation','ErrorSavingRecord', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation']).then(function (translations) {
            
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;        

            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
            $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            });
    };

    $scope.SaveCarrier = function (isValid, carrierDetail) {
        if (isValid) {
            
            var carrierId = carrierDetail.CarrierId;
            CarrierService.SaveCarrier(carrierDetail).then(function (response) {                
                if (carrierId === undefined || carrierId === 0) {
                    //Here we need to add the data in $scope.carriers
                    carrierDetail.CarrierId = response.data.CarrierId;
                    $scope.carriers.push(carrierDetail);
                }
                else {                    
                    //Need to update the carriers collection and then return back to main grid
                    $scope.updateCarrier(carrierDetail);
                }
                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteInformation,
                    body: $scope.TextSuccessfullySavedInformation,
                    showCloseButton: true
                });
                $uibModalInstance.close($scope.carriers);

            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,	
                    body: $scope.TextErrorSavingRecord,
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextValidation,
                showCloseButton: true
            });
        }
    };
    
    $scope.updateCarrier = function (carrierDetail) {
        var objects = $scope.carriers;

        for (var i = 0; i < objects.length; i++) {
            if (objects[i].CarrierId === carrierDetail.CarrierId) {
                objects[i] = carrierDetail;
                break;
            }
        }
    };

    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();

        //Set initial value and other thins
        //$scope.mode = mode;
        
        if (mode === "Add") {
            $translate('Add').then(function (add) {
                $scope.mode = add;
            });
        }
        if (mode === "Modify") {
            $translate('Modify').then(function (modify) {
                $scope.mode = modify;
            });
        }

        //$scope.CarrierTypes = [
        //   { id: 1, name: 'Air' },
        //   { id: 2, name: 'Courier' },
        //   { id: 3, name: 'Expryes' },
        //   { id: 4, name: 'Sea' }
        //];

        $scope.CarrierTypes = [
           { name: 'Air' },
           { name: 'Courier' },
           { name: 'Expryes' },
           { name: 'Sea' }
        ];

        $scope.carriers = carriers;

        $scope.carrierDetail = {
            CarrierId: carrier.CarrierId,
            CarrierName: carrier.CarrierName,
            Code: carrier.Code,
            Prefix: carrier.Prefix,
            CarrierType: carrier.CarrierType 
        };
    }

    init();
});