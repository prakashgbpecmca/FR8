/** 
 * Controller
 */
angular.module('ngApp.public').controller('AgentFlightSeaShipmentDocumentController', function ($scope, $state, $stateParams, $translate, $location, config, $filter, SessionService, ShipmentService, Upload, $timeout, toaster) {

    var setModalOptions = function () {
        $translate(['FrayteSuccess', 'FrayteError', 'ErrorGetting', 'PleaseTryLater', 'CouldNotUpdateFlightDetail', 'FlightDetailConformedSuccessfully', 'agent', 'records']).then(function (translations) {

            $scope.TitleFrayteSuccess = translations.FrayteSuccess;
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TextFlightDetailConformedSuccessfully = translations.FlightDetailConformedSuccessfully;
            $scope.TextCouldNotUpdateFlightDetail = translations.CouldNotUpdateFlightDetail;
            $scope.TextPleaseTryLater = translations.PleaseTryLater;
            $scope.TextErrorGettingAgentRecord = translations.ErrorGetting + " " + translations.agent + " " + translations.records;

        });
    };

    $scope.AgentFlightSeaDetail = function (isValid) {
        if (isValid) {
            ShipmentService.UpdateFlightSeaDetail($scope.agentFlightSea).then(function (response) {
                if (response.status == 200) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteSuccess,
                        body: $scope.TextFlightDetailConformedSuccessfully,
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
                        body: $scope.TextCouldNotUpdateFlightDetail + " " + $scop.TextPleaseTryLater,
                        showCloseButton: true
                    });
                }
            });

        }
    };

    $scope.openCalender = function (calType, $event) {
        if (calType === 'Etd') {
            $scope.status.openedEtd = true;
            $scope.status.openedEta = false;
        }
        if (calType === 'Eta') {
            $scope.status.openedEtd = false;
            $scope.status.openedEta = true;
        }
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

    // Get Prefix
    var getPrefixViaTradelane = function (shipmentId) {
        ShipmentService.getPrefixViaTradelane(shipmentId).then(function (response) {
            $scope.PreFix = response.data;
        });
    };

    var getETDETADeatil = function (shipmentId) {
        ShipmentService.GetETAETDDetail(shipmentId).then(function (response) {
            if (response.data !== null) {
              var etaDate=  moment.utc(response.data.ETDDate).toDate();
               var etddate= moment.utc(response.data.ETADate).toDate();
                $scope.agentFlightSea = {
                    ShipmentId: shipmentId,
                    FlightVessel: '',
                    ETDDate: etaDate,
                    ETDTime: response.data.ETDTime,
                    ETADate: etddate,
                    ETATime: response.data.ETATime,
                    MABBL: ''
                };
            }

        }, function () {
        });
    };
       
    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();

        $scope.agentFlightSea = {
            ShipmentId: 0,
            FlightVessel: '',
            ETDDate: new Date(),
            ETDTime: '',
            ETADate: new Date(),
            ETATime: '',
            MABBL: ''
        };

        $scope.status = {
            openedEta: false,
            openedEtd: false
        };

        $scope.formCaption = {
            Caption1: '',
            Caption2: '',
            Caption3: ''
        };

        $scope.agentFlightSea.ShipmentId = $stateParams.shipmentId;
        $scope.shipmentType = $stateParams.shipmentType;

        if ($scope.shipmentType === 'a') {            
            $translate('Flight').then(function (flight) {                
                $scope.formCaption.Caption1 = flight;
                $scope.formCaption.Caption2 = flight;
            });
            
            $translate(['Master', 'AirWayBill']).then(function (flightDetails) {                
                $scope.Master = flightDetails.Master;
                $scope.AirWayBill = flightDetails.AirWayBill;
                $scope.formCaption.Caption3 = $scope.Master + " " + $scope.AirWayBill;
            });
            //$scope.formCaption.Caption1 = 'Flight';
            //$scope.formCaption.Caption2 = 'Flight';
           // $scope.formCaption.Caption3 = 'Mastair Airway Bill';
        }
        else if ($scope.shipmentType === 's') {
            $translate('Vessel').then(function (vessel) {
                $scope.formCaption.Caption1 = vessel;
            });
            $translate('Sailing').then(function (sailing) {
                $scope.formCaption.Caption2 = sailing;
            });
            $translate(['Bill', 'Of', 'Lading']).then(function (vesselDetails) {                
                $scope.Bill = vesselDetails.Bill;
                $scope.Of = vesselDetails.Of;
                $scope.Lading = vesselDetails.Lading;
                $scope.formCaption.Caption3 = $scope.Bill + " " + $scope.Of + " " + $scope.Lading;
            });
           // $scope.formCaption.Caption1 = 'Vessel';
           // $scope.formCaption.Caption2 = 'Sailing';
           // $scope.formCaption.Caption3 = 'Bill of Lading';
        }
        var id = $stateParams.shipmentId;
        getShipmentDetail(id);
        getPrefixViaTradelane(id);
        getETDETADeatil(id);
    }
    init();

});