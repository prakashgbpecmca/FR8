/**
 * Controller
 */
angular.module('ngApp.receiver').controller('ReceiverAddEditController', function ($scope, $location, $filter, $translate, SessionService, ReceiverService, $uibModal, $uibModalInstance, toaster, receiver, countries) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['detail', 'records', 'FrayteError', 'FrayteInformation', 'FrayteValidation', 'SuccessfullyDeleteInformation', 'ErrorDeletingReocrd', 'ErrorGetting', 'PleaseCorrectValidationErrors', 'ErrorSavingRecord', 'SuccessfullySavedInformation']).then(function (translations) {
            
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;

            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextSavingError = translations.ErrorSavingRecord;
            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;

            $scope.TextSuccessfullyDelete = translations.SuccessfullyDelete + " " + translations.receiver + " " + translations.information;
           
            $scope.TextErrorDeletingReocrd = translations.ErrorDeletingReocrd;

            $scope.TextErrorErrorGettingReocrd = translations.ErrorGetting + " " + translations.receiver + " " + translations.records;
        });
    };

    $scope.receiverDetail = receiver;
    $scope.countries = countries;

    $scope.AddEditMode = function () {
        if ($scope.receiverDetail.UserId > 0) {
            return "Modify";
        }
        else {
            return "Add";
        }
    };

    $scope.submit = function (isValid, receiverDetail) {
        if (isValid) {
            ReceiverService.SaveReceiver(receiverDetail).then(function (response) {
                $uibModalInstance.close();
                toaster.pop({
                    type: 'success',
                    title: $scope.TitleFrayteInformation,
                    body: $scope.TextSuccessfullySavedInformation,
                    showCloseButton: true
                });
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextSavingError,
                    showCloseButton: true
                });
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

    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();       
    }

    init();
});