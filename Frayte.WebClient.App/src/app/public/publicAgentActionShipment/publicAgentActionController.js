/** 
 * Controller
 */
angular.module('ngApp.public').controller('AgentActionController', function ($scope, $stateParams, config, $translate, ShipmentService, UserService, toaster, $timeout, $state) {

    var setModalOptions = function () {
        $translate(['FrayteSuccess', 'FrayteError', 'FrayteValidation', 'ThankyouConformedSuccessfully', 'PleaseTryLater', 'ErrorGetting', 'CouldNotConfirmAgent', 'PleaseCorrectValidationErrors', 'ThankyouForYourValuableFeedback', 'SomeErrorOccured', 'agent', 'records']).then(function (translations) {

            $scope.TitleFrayteSuccess = translations.FrayteSuccess;
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteValidation = translations.FrayteValidation;

            $scope.TextThankyouConformedSuccessfully = translations.ThankyouConformedSuccessfully;
            $scope.TextCouldNotConfirmAgent = translations.CouldNotConfirmAgent;

            $scope.TextPleaseTryLater = translations.PleaseTryLater;
            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextThankyouForYourValuableFeedback = translations.ThankyouForYourValuableFeedback;
            $scope.TextSomeErrorOccured = translations.SomeErrorOccured;

            $scope.TextErrorGettingAgentRecord = translations.ErrorGetting + " " + translations.agent + " " + translations.records;
        });
    };
    // make state and zip non required
    $scope.setStateAndZip = function (Code, stateZip) {
        if (Code !== null && Code !== '' && Code !== undefined) {
            if (Code === "HKG" && (stateZip === 'zip' || stateZip === 'state')) {
                return false;
            }
            else if ( Code === "GBR" && stateZip === 'state') {
                return false;
            }
            else {
                return true;
            }
        }
        else {
            return true;
        }
    };
    // Set State and Zip Disable
    $scope.SetZipStateValue = function (Code) {
        if (Code !== null && Code !== '' && Code !== undefined) {
            if (Code === "HKG") {
                $scope.setStateDisable = true;
                $scope.setZipDisable = true;
                $scope.originatingAgent.DeliveryAddress.Zip = null;
                $scope.originatingAgent.DeliveryAddress.State = null;
            }
            else if (Code === "GBR") {
                $scope.setStateDisable = true;
                $scope.setZipDisable = false;
                $scope.originatingAgent.DeliveryAddress.State = null;
            }
            else {
                $scope.setZipDisable = false;
                $scope.setStateDisable = false;
            }
        }
    };

    //Use Different Address
    $scope.setValues = function (value) {
        if (value) {
            $scope.originatingAgent.DeliveryAddress = null;
            // To enable the stae and zip text box.
            $scope.setZipDisable = false;
            $scope.setStateDisable = false;
        }
        else {
            $scope.originatingAgent.DeliveryAddress = $scope.newAddress;
        }
    };

    $scope.NewAgentConfirmReject = function () {

        $scope.originatingAgent = {
            ShipmentId: 0,
            OriginatingPlannedDepartureDate: new Date(),
            OriginatingPlannedDepartureTime: '',
            OriginatingPlannedArrivalDate: new Date(),
            OriginatingPlannedArrivalTime: '',
            DeliveryAddress: {
                UserAddressId: 0,//N
                UserId: 0,//N
                AddressTypeId: 0,//N
                Address: '',
                Address2: '',
                Address3: '',
                Suburb: '',
                City: '',
                State: '',
                Zip: '',
                Country: {
                    CountryId: 0,
                    Name: '',
                    Code: ''
                }
            }
        };
        //Agent Rejection code start
        $scope.agentRejection = {
            ShipmentId: 0,
            OriginatingAgentId: 0,
            RejectionReason: ''
        };

    };

    $scope.AgentConfirmed = function (isValid, dateTime) {
        if (isValid) {
            ShipmentService.UpdateOriginatingAgentDetail(dateTime).then(function (response) {
                if (response.status == 200) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteSuccess,
                        body: $scope.TextThankyouConformedSuccessfully,
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
                        body: $scope.TextCouldNotConfirmAgent + " " + $scope.TextPleaseTryLater,
                        showCloseButton: true
                    });
                }
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

    $scope.AgentRejected = function (isValid) {
        if (isValid) {
            ShipmentService.OriginatingAgentReject($scope.agentRejection).then(function (response) {
                if (response.status == 200) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteSuccess,
                        body: $scope.TextThankyouForYourValuableFeedback,
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
                        body: $scope.TextSomeErrorOccured + " " + $scope.TextPleaseTryLater,
                        showCloseButton: true
                    });
                }
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

    $scope.openCalender = function ($event) {
        $scope.status.opened = true;
    };
    $scope.openCalender1 = function ($event) {
        $scope.status.opened1 = true;
    };

    $scope.LoadInitials = function () {
        UserService.GetInitials().then(function (response) {
            $scope.countries = response.data.Countries;
        });
    };
    var SaveAgentPTDPTA = function (dateTime) {
        ShipmentService.SaveAgentPTDPTA(dateTime).then(function (response) {
            console.log("Saved!");
        });
    };

    var getDestinatingAgent = function (shipmentId) {
        ShipmentService.GetDestinatingAgent(shipmentId).then(function (response) {
            $scope.destinatingAgent = response.data;
        });
    };
    var getShipmentDetail = function (id) {
        ShipmentService.GetShipmentShipperReceiverDetail(id).then(function (response) {
            if (response.data.FrayteCargoWiseSo !== null && response.data.FrayteCargoWiseSo !== "") {
                $scope.FrayteCargoWiseSo = response.data.FrayteCargoWiseSo;
            }
            else
            {
                $scope.FrayteCargoWiseSo = id;
            }
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

    var getAgentDetail = function (id) {
        ShipmentService.GetOriginatingAgent(id).then(function (response) {
            $scope.originatingAgent = {
                ShipmentId: response.data.ShipmentId,
                OriginatingPlannedDepartureDate: new Date(),
                OriginatingPlannedDepartureTime: '',
                OriginatingPlannedArrivalDate: new Date(),
                OriginatingPlannedArrivalTime: '',
                DeliveryAddress: response.data.DeliveryAddress
            };
            //Set State and Zip for originating.Delivery 
            // Set State and Zip for "HKG" and "GBR"
            if ($scope.originatingAgent && $scope.originatingAgent.DeliveryAddress != null && $scope.originatingAgent.DeliveryAddress.Country.Code === "HKG") {
                $scope.setStateDisable = true;
                $scope.setZipDisable = true;
                $scope.originatingAgent.DeliveryAddress.Zip = null;
                $scope.originatingAgent.DeliveryAddress.State = null;
            }
            if ($scope.originatingAgent && $scope.originatingAgent.DeliveryAddress != null && $scope.originatingAgent.DeliveryAddress.Country.Code === "GBR") {
                $scope.setStateDisable = true;
                $scope.originatingAgent.DeliveryAddress.State = null;
            }
            // for restoring the OriginatiigAddress
            $scope.newAddress = response.data.DeliveryAddress;
        });

    };
    function init() {

        // set Multilingual Modal Popup Options
        setModalOptions();

        $scope.NewAgentConfirmReject();
        $scope.status = {
            opened: false
        };
        //Action will be either c or r : C = Confirm, R = Reject
        $scope.confirmationDetail = {
            ActionType: ''
        };

        $scope.confirmationDetail.ActionType = $stateParams.actionType;
        $scope.originatingAgent.ShipmentId = $stateParams.shipmentId;
        $scope.agentRejection.ShipmentId = $stateParams.shipmentId;

        $scope.LoadInitials();

        var id = $stateParams.shipmentId;

        getShipmentDetail(id);
        getAgentDetail(id);
        getDestinatingAgent(id);

        $scope.setStateDisable = false;
        $scope.setZipDisable = false;
    }

    init();

});