angular.module('ngApp.directBooking').controller('DraftPublicConfirmation', function ($uibModalInstance, $uibModal, $scope) {

    $scope.Close = function (IsPublic) {
        $uibModalInstance.close(IsPublic);
    };

    $scope.Dismiss = function (IsPublic) {
        $uibModalInstance.close(IsPublic);
    };
});