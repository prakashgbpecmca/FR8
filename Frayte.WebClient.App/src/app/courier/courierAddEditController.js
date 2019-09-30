/**
 * Controller
 */
angular.module('ngApp.courier').controller('CourierAddEditController', function ($scope, $location, $filter, $translate, LogonService, SessionService, CourierService, $uibModal, $uibModalInstance, couriers, courier, $log, toaster, mode) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;

            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
        });
    };


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
    $scope.CourierTypes = [
       { id: 1, name: 'Air' },
       { id: 2, name: 'Courier' },
       { id: 3, name: 'Expryes' },
       { id: 4, name: 'Sea' }
    ];

    $scope.hstep = 1;
    $scope.mstep = 15;

    $scope.couriers = couriers;

    $scope.courierDetail = {
        CourierId: courier.CourierId,
        Name: courier.Name,
        Website: courier.Website,
        CourierType: courier.CourierType,
        TransitTime: courier.TransitTime,
        LatestBookingTime: courier.LatestBookingTime
    };

    $scope.submit = function (isValid, courierDetail) {
        if (isValid) {
            var courierId = courierDetail.CourierId;
            //courierDetail.LatestBookingTime = moment(courierDetail.LatestBookingTime).format();
            CourierService.SaveCourier(courierDetail).then(function (response) {
                if (courierId === undefined || courierId === 0) {
                    //Here we need to add the data in $scope.couriers
                    courierDetail.CourierId = response.data.CourierId;
                    $scope.couriers.push(courierDetail);
                }
                else {
                    //Need to update the couriers collection and then return back to main grid
                    $scope.updateCourier(courierDetail);
                }
                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteInformation,
                    body: $scope.TextSuccessfullySavedInformation,
                    showCloseButton: true
                });
                $uibModalInstance.close($scope.couriers);

            }, function () {
                toaster.pop({
                    type: 'warning',
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

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.updateCourier = function (courierDetail) {
        var objects = $scope.couriers;
        for (var i = 0; i < objects.length; i++) {
            if (objects[i].CourierId === courierDetail.CourierId) {
                objects[i] = courierDetail;
                break;
            }
        }
    };


    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();       
    }

    init();
});