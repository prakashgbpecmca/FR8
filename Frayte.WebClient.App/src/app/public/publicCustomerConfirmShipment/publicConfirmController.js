/** 
 * Controller
 */
angular.module('ngApp.public').controller('PublicConfirmController', function ($scope, $stateParams, $translate, config, ShipmentService, toaster) {

    var setModalOptions = function () {
        $translate(['FrayteError', 'CouldNotFindShipmentConfirmationDetail']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TextCouldNotFindShipmentConfirmationDetail = translations.CouldNotFindShipmentConfirmationDetail;

        });
    };

    $scope.confirmationDetail = {
        ActionType: '',
        ConfirmationCode: ''
    };

    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();
        $scope.confirmationDetail.ConfirmationCode = $stateParams.confirmationCode;
        $scope.confirmationDetail.ActionType = $stateParams.actionType;

        if ($stateParams.confirmationCode !== undefined && $stateParams.confirmationCode !== null) {
            //To Dos: Send confirmation code to Web Api and confirm the new shipment         
            ShipmentService.CustomerAction($scope.confirmationDetail).then(function (response) {
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
                    body: $scope.TextCouldNotFindShipmentConfirmationDetail,
                    showCloseButton: true
                });
            });
        }
    }

    init();

});