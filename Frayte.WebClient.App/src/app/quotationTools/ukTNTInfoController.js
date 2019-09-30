angular.module('ngApp.quotationTools').controller('ukTNTInfoController', function ($uibModal, $scope, TNTInfo, TNTInfo1, LogisticCompany) {
    $scope.UKTNTInfo = TNTInfo;
    $scope.UKTNTInfo1 = TNTInfo1;
    $scope.LogisticCompany = LogisticCompany;
});