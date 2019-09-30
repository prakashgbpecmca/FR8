/**
 * Service
 */
angular.module('ngApp.receiver').factory('ReceiverService', function ($http, config, SessionService) {

    var GetReceiverList = function () {
        return $http.get(config.SERVICE_URL + '/Receiver/GetReceiverList');
    };

    var GetReceiverDetail = function (receiverId) {
        return $http.get(config.SERVICE_URL + '/Receiver/GetReceiverDetail',
            {
                params: {
                    receiverId: receiverId
                }
            });
    };

    var SaveReceiver = function (receiverDetail) {
        return $http.post(config.SERVICE_URL + '/Receiver/SaveReceiver', receiverDetail);
    };
    var SaveLoggedInReceiver = function (receiverDetail) {
        return $http.post(config.SERVICE_URL + '/Receiver/SaveLoggedInReceiver', receiverDetail);
    };

    var DeleteReceiver = function (receiverId) {
        return $http.get(config.SERVICE_URL + '/Receiver/DeleteReceiver',
            {
                params: {
                    receiverId: receiverId
                }
            });
    };

    var GetAssignedShippers = function (receiverId) {
        return $http.get(config.SERVICE_URL + '/Receiver/GetAssignedShippers',
            {
                params: {
                    receiverId: receiverId
                }
            });
    };

    var SaveReceiverShippers = function (receiverShipper) {
        return $http.post(config.SERVICE_URL + '/Receiver/SaveReceiverShippers', receiverShipper);
    };

    var RemoveReceiverShippers = function (receiverShipper) {
        return $http.post(config.SERVICE_URL + '/Receiver/RemoveReceiverShippers', receiverShipper);
    };

    var GetReceiverOtherAddresses = function (receiverId) {
        return $http.get(config.SERVICE_URL + '/Receiver/GetReceiverOtherAddresses',
            {
                params: {
                    receiverId: receiverId
                }
            });
    };

    var SaveReceiverOtherAddress = function (receiverOtherAddress) {
        return $http.post(config.SERVICE_URL + '/Receiver/SaveReceiverOtherAddress', receiverOtherAddress);
    };

    return {
        GetReceiverList: GetReceiverList,
        GetReceiverDetail: GetReceiverDetail,
        SaveReceiver: SaveReceiver,
        DeleteReceiver: DeleteReceiver,
        GetAssignedShippers: GetAssignedShippers,
        SaveReceiverShippers: SaveReceiverShippers,
        RemoveReceiverShippers: RemoveReceiverShippers,
        GetReceiverOtherAddresses: GetReceiverOtherAddresses,
        SaveReceiverOtherAddress: SaveReceiverOtherAddress,
        SaveLoggedInReceiver: SaveLoggedInReceiver
    };

});