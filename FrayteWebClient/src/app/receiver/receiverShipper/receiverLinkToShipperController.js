angular.module('ngApp.receiver').controller('ReceiverShippersController', function ($scope, $uibModal, toaster, ReceiverService, ShipperService, receiverId) {

    $scope.SearchShippers = function (searchKey) {
        if (searchKey.length >= 3) {
            ShipperService.GetSearchShippers(searchKey).then(function (response) {
                $scope.searchShippers = response.data;
            });
        }
        else {
            $scope.searchShippers = [];
        }
    };

    $scope.AddShipper = function (shipper) {
        var receiverShipper = {
            ReceiverId: $scope.receiverId,
            ShipperId: shipper.UserId
        };

        ReceiverService.SaveReceiverShippers(receiverShipper).then(function (response) {
            if (response.status === 200) {
                $scope.assignedShippers.push(shipper);
            }
        });
    };

    $scope.RemoveShipper = function (shipper) {
        var receiverShipper = {
            ReceiverId: $scope.receiverId,
            ShipperId: shipper.UserId
        };

        ReceiverService.RemoveReceiverShippers(receiverShipper).then(function (response) {
            if (response.status === 200) {                
                var index = $scope.assignedShippers.indexOf(shipper);
                $scope.assignedShippers.splice(index, 1);
            }
        });
    };

    $scope.GetReceiverShippers = function (receiverId) {
        ReceiverService.GetAssignedShippers(receiverId).then(function (response) {
            $scope.assignedShippers  = response.data;
        });
    };

    function init() {
        $scope.receiverId = receiverId;        
        $scope.searchShippers = [];
        $scope.GetReceiverShippers($scope.receiverId);
    }

    init();

});