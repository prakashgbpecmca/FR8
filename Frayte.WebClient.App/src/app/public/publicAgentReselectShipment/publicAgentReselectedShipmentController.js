/** 
 * Controller
 */
angular.module('ngApp.public').controller('AgentReselectedShipmentController', function ($scope, $translate, $state, $stateParams, $location, config, $filter, SessionService, ShipmentService, Upload, $timeout, toaster, AgentService) {

    var setModalOptions = function () {
        $translate(['FrayteSuccess', 'FrayteError', 'ErrorGetting', 'AgentSelectedSuccessfully', 'CouldNotSelectAgent', 'PleaseTryLater', 'agent', 'records']).then(function (translations) {

            $scope.TitleFrayteSuccess = translations.FrayteSuccess;
            $scope.TitleFrayteError = translations.FrayteError;

            $scope.TextAgentSelectedSuccessfully = translations.AgentSelectedSuccessfully;
            $scope.TextCouldNotSelectAgent = translations.CouldNotSelectAgent;
            $scope.TextPleaseTryLater = translations.PleaseTryLater;
            $scope.TextErrorGettingAgentRecord = translations.ErrorGetting + " " + translations.agent + " " + translations.records;

        });
    };

    $scope.agentReselect = {
        ShipmentId: 0,
        CustomerName: '',
        ShippingMethod: '',
        Country: null,
        Agent: {
            UserId: 0,
            Name: ''
        }
    };
    $scope.submit = function (isValid, agentReselect) {
        if (isValid) {
            var agentRelectData = {
                ShipmentId: 0,
                OriginatingAgent: null
            };
            //Set valuse
            agentRelectData.ShipmentId = agentReselect.ShipmentId;
            agentRelectData.OriginatingAgent = agentReselect.Agent;
            //agentRelectData.OriginatingAgent.UserId = agentReselect.Agent.UserId;
            //agentRelectData.OriginatingAgent.Name = agentReselect.Agent.Name;

            ShipmentService.ReselectAgent(agentRelectData).then(function (response) {
                if (response.status == 200) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteSuccess,
                        body: $scope.TextAgentSelectedSuccessfully,
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
                        body: $scope.TextCouldNotSelectAgent + " " + $scope.TextPleaseTryLater,
                        showCloseButton: true
                    });
                }
            });
        }
    };
    $scope.GetCountryAgents = function (country) {
        AgentService.GetCountryAgents(country.CountryId).then(function (response) {
            $scope.AgentList = response.data;
        });
    };
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

        ShipmentService.GetInitials().then(function (response) {
            $scope.countries = response.data.Countries;

            ShipmentService.GetReselectAgentShipmentDetail($scope.agentReselect.ShipmentId).then(function (response) {
                $scope.agentReselect = response.data;
                AgentService.GetCountryAgents($scope.agentReselect.Country.CountryId).then(function (response) {
                    if (response.data !== null) {
                        var list = response.data;
                        for (var i = 0; i < list.length; i++) {
                            if ($scope.agentReselect.Agent !== null && $scope.agentReselect.Agent.UserId > 0) {
                                if ( list[i].UserId === $scope.agentReselect.Agent.UserId) {
                                    list.splice(i, 1);
                                }
                            }
                        }
                        $scope.AgentList = list;
                    }
                });
            });
        });
        $scope.agentReselect.ShipmentId = $stateParams.shipmentId;
        var id = $stateParams.shipmentId;
        getShipmentDetail(id);
    }
    init();

});