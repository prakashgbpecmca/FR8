angular.module('ngApp.shipment').controller('OtherAddressController', function ($scope, ShipperService, $translate, ReceiverService, ModalService, $uibModalInstance, userType, userId) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteError', 'PleaseSelectAddress', 'AddressSelection']).then(function (translations) {
            $scope.TextHeaderAddressSelection = translations.AddressSelection;
            $scope.TextBodyPleaseSelectAddress = translations.PleaseSelectAddress;
        });
    };

    $scope.SelectAddress = function (otherAddress) {

        if (otherAddress === undefined || otherAddress === null) {
            ModalService.Alert({}, {
                headerText: $scope.TextHeaderAddressSelection,
                bodyText: $scope.TextBodyPleaseSelectAddress
            });
        }
        else {
            $uibModalInstance.close(otherAddress);
        }
    };

    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();

        $scope.selectAddress = {
            Address: null
        };

        //Set injected values to local variable
        $scope.userType = userType;
        $scope.userId = userId;

        if ($scope.userType === 'SHIPPER') {
            ShipperService.GetShippeOtherAddresses($scope.userId).then(function (response) {
                $scope.addresses = response.data;
            });
        }
        else if ($scope.userType === 'RECEIVER') {
            ReceiverService.GetReceiverOtherAddresses($scope.userId).then(function (response) {
                $scope.addresses = response.data;
            });
        }
    }

    init();

});
