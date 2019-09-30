/** 
 * Controller
 */
angular.module('ngApp.public').controller('AgentAnticipatedShipmentController', function ($scope, $translate, $state, $stateParams, $location, config, $filter, SessionService, ShipmentService, Upload, $timeout, toaster) {
    var setModalOptions = function () {
        $translate(['FrayteSuccess', 'FrayteError', 'FrayteValidation', 'ErrorGetting', 'PleaseTryLater', 'PleaseCorrectValidationErrors', 'CouldNotUpdateAnticipatedDateTime', 'AnticipatedDateTimeUpdatedSuccessfully', 'agent', 'records']).then(function (translations) {

            $scope.TitleFrayteSuccess = translations.FrayteSuccess;
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteValidation = translations.FrayteValidation;

            $scope.TextCouldNotUpdateAnticipatedDateTime = translations.CouldNotUpdateAnticipatedDateTime;
            $scope.TextPleaseTryLater = translations.PleaseTryLater;
            $scope.TextAnticipatedDateTimeUpdatedSuccessfully = translations.AnticipatedDateTimeUpdatedSuccessfully;
            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextErrorGettingAgentRecord = translations.ErrorGetting + " " + translations.agent + " " + translations.records;

        });
    };

    $scope.updateAnticipatedDetail = {
        ShipmentId: 0,
        AnticipatedDeliveryDate: new Date(),
        AnticipatedDeliveryTime: '',
        OtherCustomIssues: '',
        IsUnexpectedDelay: false,
        UnexpectedClearance: null
    };


    $scope.unexpectedComment = function (Comment) {
        if (!Comment) {
            $scope.updateAnticipatedDetail.UnexpectedClearance = null;
        }
    };

    $scope.AgentAnticipating = function (isValid) {
        if (isValid) {
            if ($state.current.name === "public.shipper-anticipated") {
                ShipmentService.UpdateShipperAnticipatedDetail($scope.updateAnticipatedDetail).then(function (response) {
                    if (response.status == 200) {
                        toaster.pop({
                            type: 'success',
                            title: $scope.TitleFrayteSuccess,
                            body: $scope.TextAnticipatedDateTimeUpdatedSuccessfully,
                            showCloseButton: true
                        });

                        //Redirect to mail page after 4 second.
                        $timeout(function () {
                            $state.go('home.welcome');
                        }, 4000);
                    }
                    else {
                        toaster.pop({
                            type: 'error',
                            title: $scope.TitleFrayteError,
                            body: $scope.TextCouldNotUpdateAnticipatedDateTime + " " + $scope.TextPleaseTryLater,
                            showCloseButton: true
                        });
                    }
                });
            }
            else {
                ShipmentService.UpdateDestinatingAgentAnticipatedDetail($scope.updateAnticipatedDetail).then(function (response) {
                    if (response.status == 200) {
                        toaster.pop({
                            type: 'success',
                            title: $scope.TitleFrayteSuccess,
                            body: $scope.TextAnticipatedDateTimeUpdatedSuccessfully,
                            showCloseButton: true
                        });

                        //Redirect to mail page after 4 second.
                        $timeout(function () {
                            $state.go('home.welcome');
                        }, 4000);
                    }
                    else {
                        toaster.pop({
                            type: 'error',
                            title: $scope.TitleFrayteError,
                            body: $scope.TextCouldNotUpdateAnticipatedDateTime + " " + $scope.TextPleaseTryLater,
                            showCloseButton: true
                        });
                    }
                });
            }
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

    $scope.openCalender = function ($event) {
        $scope.status.opened = true;
    };
    $scope.status = {
        opened: false
    };
    $scope.confirmationCode = '';
    //$scope.shipmentId = '';
    var getShipmentDetail = function (id) {
        ShipmentService.GetShipmentShipperReceiverDetail(id).then(function (response) {
            $scope.FrayteCargoWiseSo = response.data.FrayteCargoWiseSo;
            $scope.ShipperInfo = response.data.Shipper;
            $scope.ShipperAddressInfo = response.data.ShipperAddress;
            $scope.ReceiverInfo = response.data.Receiver;
            $scope.ReceiverAddressInfo = response.data.ReceiverAddress;
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGettingAgentRecord,
                showCloseButton: true
            });
        });
    };
    function init() {
        
        // set Multilingual Modal Popup Options
        setModalOptions();

        $scope.updateAnticipatedDetail.ShipmentId = $stateParams.shipmentId;
        var id = $stateParams.shipmentId;
        getShipmentDetail(id);
    }
    init();

});