
angular.module('ngApp.public').controller('AdminActionController', function ($scope, PublicService, $state, $stateParams, toaster, config) {

    //multilingual language code start
    var setModalOptions = function () {
        $translate(['FrayteError', 'ErrorProcessingRequest']).then(function (translations) {

            $scope.FrayteError = translations.FrayteError;
            $scope.ErrorProcessingRequest = translations.ErrorProcessingRequest;
        });
    };


    var customerConfirmReject = function () {
        PublicService.AdminConfirmReject($scope.actionDetail).then(function (response) {
            if (response.status === 200) {
                //console.log('');

            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.ErrorProcessingRequest,
                showCloseButton: true
            });
        });
    };

    function init() {
        $scope.actionType = '';
        $scope.customerId = 0;
        $scope.operationUserId = 0;


        if ($stateParams.actionType !== undefined && $stateParams.actionType !== null) {
            $scope.actionType = $stateParams.actionType;
        }
        if ($stateParams.customerId !== undefined && $stateParams.customerId !== null) {
            $scope.customerId = $stateParams.customerId;
        }
        if ($stateParams.operationUserId !== undefined && $stateParams.operationUserId !== null) {
            $scope.operationUserId = $stateParams.operationUserId;
        }
        if ($scope.actionType !== '' && $scope.actionType === 'c') {
            $scope.confirmSection = true;
        }
        if ($scope.actionType !== '' && $scope.actionType === 'r') {
            $scope.confirmSection = false;
        }
        $scope.actionDetail = {
            ActionType: $scope.actionType,
            CustomerId: $scope.customerId,
            OperationUserId: $scope.operationUserId
        };
        customerConfirmReject();
        setModalOptions();
    }

    init();

});