/** 
 * Controller
// */
angular.module('ngApp.termandcondition').controller('TermAndConditionAddEditController', function ($scope, $state, $translate, TermAndConditionService, config, $filter, $uibModalInstance, LogonService, SessionService, $uibModal, toaster, termAndConditionId, isMaxTermAndCondition, ViewValue) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteSuccess', 'FrayteValidation', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation',
            'ErrorGetting', 'TermsAndCondition', 'records']).then(function (translations) {

                $scope.TitleFrayteError = translations.FrayteError;
                $scope.TitleFrayteWarning = translations.FrayteWarning;
            $scope.TitleFrayteInformation = translations.FrayteSuccess;
            $scope.TitleFrayteValidation = translations.FrayteValidation;

            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
            $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextErrorGettingTermAndCondition = translations.ErrorGetting + " " + translations.TermsAndCondition;
            $scope.RecordGettingError = translations.ErrorGetting + " " + translations.records;
        });
    };

    $scope.SaveTermAndCondition = function (isValid, termAndConditionDetail) {
        if (isValid) {
            var TermAndConditionId = termAndConditionDetail.TermAndConditionId;
          
            TermAndConditionService.SaveTermAndCondition(termAndConditionDetail).then(function (response) {
                if (response.status === 200) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteInformation,
                        body: $scope.TextSuccessfullySavedInformation,
                        showCloseButton: true
                    });
                    $uibModalInstance.close(termAndConditionDetail);
                }

            }, function () {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.TextErrorSavingRecord,
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.TextValidation,
                showCloseButton: true
            });
        }
    };


    var getOperationZone = function () {
        TermAndConditionService.GetOperationZoneList().then(function (response) {
            $scope.OperationZones = response.data;
            var found = $filter('filter')($scope.OperationZones, { OperationZoneId: $scope.termAndConditionDetail.OperationZoneId });

            if (found.length) {
                $scope.OperationZone = found[0];
            }

        }, function (reason) {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });

    };
    function init() {
        $scope.OperationZone = null;

        $scope.termConditionTypes = [
            {
                Id: 1,
                Condition: "Public",
                ConditionValue: "Public"
            },
            {
                Id: 2,
                Condition: "TradeLane",
                ConditionValue: "TradeLane"
            }
        ];

        $scope.TermAndConditionType = null;

        // set Multilingual Modal Popup Options
        setModalOptions();



        $scope.isMaxTermAndCondition = isMaxTermAndCondition;
        if (termAndConditionId > 0) {
            TermAndConditionService.GetTermAndCondition(termAndConditionId).then(function (response) {
                getOperationZone();
                $scope.termAndConditionDetail = response.data;
                var found = $filter('filter')($scope.termConditionTypes, { Condition: $scope.termAndConditionDetail.TermAndConditionType });
                if (found.length) {
                    $scope.TermAndConditionType = found[0];
                }

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
            $scope.termAndConditionDetail = {
                "TermAndConditionId": 0,
                "Detail": ""
            };
        }
        $scope.ViewValue = ViewValue;
    }

    init();

});