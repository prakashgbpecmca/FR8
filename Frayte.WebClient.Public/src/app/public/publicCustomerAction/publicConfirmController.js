/** 
 * Controller
 */
angular.module('ngApp.public').controller('PublicCustomerConfirmController', function ($scope, $stateParams, AppSpinner, $translate, PublicService, config, toaster) {

    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteSuccess', 'CouldNotFindShipmentConfirmationDetail']).then(function (translations) {

            $scope.FrayteError = translations.FrayteError;
            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.TextCouldNotFindShipmentConfirmationDetail = translations.CouldNotFindShipmentConfirmationDetail;

        });
    };

    $scope.confirmationDetail = {
        ActionType: '',
        ConfirmationCode: ''
    };

    function init() {
        // set Multilingual Modal Popup Options
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        setModalOptions();
        $scope.confirmationDetail.ConfirmationCode = $stateParams.confirmationCode;
        $scope.confirmationDetail.ActionType = $stateParams.actionType;
        var successMessage = "";
        var errorMessage = "";
        if ($stateParams.actionType && ($stateParams.actionType === 'r' || $stateParams.actionType === 'c')) {
          
            var message = "";

            if ($stateParams.actionType === 'r') {

                message = "Rejecting the shipment";
                successMessage = "Your shipment has been rejected";
                errorMessage = "Error While rejecting the shipment";
            }
            if ($stateParams.actionType === 'c') {
                message = "Confirming the shipment";
                successMessage = "Your shipment has been confirmed";
                errorMessage = "Error While confirming the shipment";
            }
            if ($stateParams.confirmationCode && $stateParams.confirmationCode) {
                //To Dos: Send confirmation code to Web Api and confirm the new shipment         
                AppSpinner.showSpinnerTemplate(message, $scope.Template);
                PublicService.CustomerAction($scope.confirmationDetail).then(function (response) {
                    if (response.status != 200) {
                        toaster.pop({
                            type: 'error',
                            title: $scope.FrayteError,
                            body: errorMessage,
                            showCloseButton: true
                        });
                    }
                    else {
                        if (response.data && (response.data === 'AlreadyConfirmed' || response.data === 'AlreadyRejected')) {
                            console.log('already confirmed/rejected shipment');
                        }
                        else {
                            toaster.pop({
                                type: 'success',
                                title: $scope.FrayteSuccess,
                                body: successMessage,
                                showCloseButton: true
                            });
                        }
                        
                    }
                    AppSpinner.hideSpinnerTemplate();
                }, function () {
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteError,
                        body: errorMessage,
                        showCloseButton: true
                    });
                    AppSpinner.hideSpinnerTemplate();
                });
            }
            else {
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: errorMessage,
                    showCloseButton: true
                });
            }
        }
        else {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: errorMessage,
                showCloseButton: true
            });
        }

    }

    init();

});