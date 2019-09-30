/** 
 * Controller
 */
angular.module('ngApp.public').controller('OperationStaffConfirmShipment', function ($scope, $stateParams, toaster, ShipmentService, $translate) {

    var setModalOptions = function () {
        $translate(['FrayteError', 'CouldNotFindShipmentConfirmationDetail', 'CouldNotUpdateInformation']).then(function (translations) {
            debugger;
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TextCouldNotFindShipmentConfirmationDetail = translations.CouldNotFindShipmentConfirmationDetail;
            $scope.TextCouldNotUpdateInformation = translations.CouldNotUpdateInformation;
        });
    };

    var getShipmentDetail = function (id) {
        ShipmentService.GetShipmentShipperReceiverDetail(id).then(function (response) {
            $scope.FrayteCargoWiseSo = response.data.FrayteCargoWiseSo;
            $scope.PurchaseOrderNo = response.data.PurchaseOrderNo;
            $scope.ShipperInfo = response.data.Shipper;
            $scope.ShipperAddressInfo = response.data.ShipperAddress;
            $scope.ReceiverInfo = response.data.Receiver;
            $scope.ReceiverAddressInfo = response.data.ReceiverAddress;

        }, function () {
            console.log("Error!");
        });

    };

    function init() {
        debugger;
        // set Multilingual Modal Popup Options
        setModalOptions();

        var id = $stateParams.shipmentId;
        if (id !== null && id !== undefined) {
            getShipmentDetail(id);
        }
        ShipmentService.OperationStaffConfirmation(id).then(function (response) {
            if (response.status != 200) {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextCouldNotFindShipmentConfirmationDetail,
                    showCloseButton: true
                });
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextCouldNotUpdateInformation,
                showCloseButton: true
            });
        });
    }

    init();

});