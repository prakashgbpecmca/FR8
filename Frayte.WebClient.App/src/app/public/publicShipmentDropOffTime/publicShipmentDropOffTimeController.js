/** 
 * Controller
 */
angular.module('ngApp.public').controller('ShipmentDropOffTimeController', function ($scope, $state, $translate, $stateParams, $location, config, $filter, SessionService, ShipmentService, Upload, $timeout, toaster) {

    var setModalOptions = function () {
        $translate(['FrayteSuccess', 'FrayteError', 'FrayteValidation', 'CouldNotUpdateInformation', 'ErrorGetting', 'PleaseCorrectValidationErrors', 'PleaseTryLater', 'DropOffInformationUpdatedSuccessfully', 'agent', 'records']).then(function (translations) {
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteSuccess = translations.FrayteSuccess;
            $scope.TitleFrayteValidation = translations.FrayteValidation;

            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextCouldNotUpdateInformation = translations.CouldNotUpdateInformation;
            $scope.TextPleaseTryLater = translations.PleaseTryLater;
            $scope.TextDropOffInformationUpdatedSuccessfully = translations.DropOffInformationUpdatedSuccessfully;
            $scope.TextErrorGettingAgentRecord = translations.ErrorGetting + " " + translations.agent + " " + translations.records;

        });
    };

    $scope.isLogin = false;
    var getTime = function () {
        var d = new Date();
        var h = d.getHours();
        var m = d.getMinutes();
        if (h.toString().length < 2) {
            h = '0' + h.toString();
        }
        if (m.toString().length < 2) {
            m = '0' + m.toString();
        }

        return h.toString() + m.toString();
    };
    $scope.shipmentDropOffTime = {
        ShipmentId: 0,
        DropOffDate: new Date(),
        DropOffTime: getTime()
    };

   
    $scope.UpdateDrofOffDetail = function (isValid) {

        if (isValid) {
            ShipmentService.UpdateDropOffDetail($scope.shipmentDropOffTime).then(function (response) {
                if (response.status == 200) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteSuccess,
                        body: $scope.TextDropOffInformationUpdatedSuccessfully,
                        showCloseButton: true
                    });

                    if ($stateParams.shipmentId === undefined || $stateParams.shipmentId === null) {
                        $scope.$dismiss('cancel');
                    }
                    else {
                        //Redirect to mail page after 4 second.
                        $timeout(function () {
                            $state.go('home.welcome');
                        }, 4000);
                    }
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextCouldNotUpdateInformation + " " + $scope.TextPleaseTryLater,
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

    $scope.status = {
        opened: false
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

        var userInfo = SessionService.getUser();
        if (userInfo === undefined || userInfo === null || userInfo.SessionId === undefined || userInfo.SessionId === '') {
            $scope.isLogin = false;
        }
        else {
            $scope.isLogin = true;
        }
        if ($stateParams.shipmentId === undefined || $stateParams.shipmentId === null) {
            $scope.shipmentDropOffTime.ShipmentId = $scope.params.shipmentId;
        }
        else {
            $scope.shipmentDropOffTime.ShipmentId = $stateParams.shipmentId;
            var id = $stateParams.shipmentId;
            if (!$scope.isLogin) {
                getShipmentDetail(id);
            }
        }
        // $scope.shipmentDropOffTime.DropOfTime = moment.utc().format();
    }
    init();

});