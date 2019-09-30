angular.module('ngApp.termandcondition').controller('TermAndConditionDetailController', function ($scope, $state, $translate, TermAndConditionService, config, $filter, $uibModalInstance, LogonService, SessionService, $uibModal, toaster, termAndConditionId) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteError', 'ErrorGetting', 'TermsAndCondition']).then(function (translations) {
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TextErrorGettingTermAndCondition = translations.ErrorGetting + " " + translations.TermsAndCondition;
        });
    };

    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();

        if (termAndConditionId > 0) {
            TermAndConditionService.GetTermAndCondition(termAndConditionId).then(function (response) {
                $scope.termAndConditions = response.data;
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorGettingTermAndCondition,
                    showCloseButton: true
                });
            });
        }
        else {
            $scope.termAndConditions = {
                "TermAndConditionId": 0,
                "Detail": ""
            };
        }
    }

    init();

});