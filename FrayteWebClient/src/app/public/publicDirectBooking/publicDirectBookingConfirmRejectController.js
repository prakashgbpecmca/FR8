angular.module('ngApp.public').controller('DirectBookingConfirmRejectController', function ($scope, DirectBookingService, $state, $stateParams, toaster, config, $translate) {
    var setMultilingualOptions = function () {
        $translate(['Success', 'Record_Saved', 'FrayteError_Validation', 'ErrorSavingRecord', 'GettingDetails_Error', 'CancelShipmentError_Validation',
            'GeneratePdfError_Validation', 'GeneratePdfError_Validation', 'SuccessfullySendlLabel_Validation', 'SendingMailError_Validation',
        'EnterValidEmailAdd', 'TrackShipmentNotTrack_Error', 'FrayteWarning', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation', 'ErrorSavingRecord', 'FrayteError', 'FrayteSuccess']).then(function (translations) {
            $scope.Success = translations.Success;
            $scope.RecordSaved = translations.Record_Saved;
            $scope.FrayteErrorValidation = translations.FrayteError_Validation;
            $scope.ErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.GettingDetailsError = translations.GettingDetails_Error;
            $scope.CancelShipmentErrorValidation = translations.CancelShipmentError_Validation;
            $scope.GeneratePdfErrorValidation = translations.GeneratePdfError_Validation;
            $scope.SuccessfullySendlLabelValidation = translations.SuccessfullySendlLabel_Validation;
            $scope.SendingMailErrorValidation = translations.SendingMailError_Validation;
            $scope.EnterValidEmailAdd = translations.EnterValidEmailAdd;
            $scope.TrackShipmentNotTrackError = translations.TrackShipmentNotTrack_Error;
            $scope.Frayte_Warning = translations.FrayteWarning;
            $scope.PleaseCorrect_ValidationErrors = translations.PleaseCorrectValidationErrors;
            $scope.Successfully_SavedInformation = translations.SuccessfullySavedInformation;
            $scope.ErrorSaving_Record = translations.ErrorSavingRecord;
            $scope.Frayte_Error = translations.FrayteError;
            $scope.Frayte_Success = translations.FrayteSuccess;


        });
    };
    var directBookingConfirmReject = function () {
        DirectBookingService.DirectBookingConfirmReject($scope.actionType, $scope.directShipmentId).then(function (response) {
            if (response.status === 200) {
                //console.log('');
              
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.Frayte_Error,
                body: "Error while processing the request.",
                showCloseButton: true
            });
        });
    };

    function init() {
        $scope.actionType = '';
        $scope.directShipmentId = 0;
        if ($stateParams.actionType !== undefined && $stateParams.actionType !== null) {
            $scope.actionType = $stateParams.actionType;
        }
        if ($stateParams.directShipmentId !== undefined && $stateParams.directShipmentId !== null) {
            $scope.directShipmentId = $stateParams.directShipmentId;
        }
        if ($scope.actionType !== '' && $scope.actionType === 'c') {
            $scope.confirmSection = true;
        }
        if ($scope.actionType !== '' && $scope.actionType === 'r') {
            $scope.confirmSection = false;
        }
        directBookingConfirmReject();
        setMultilingualOptions();
    }

    init();

});