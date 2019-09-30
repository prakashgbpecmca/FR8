/**
 * Controller
 */
angular.module('ngApp.forgetPassword').controller('ForgetPasswordController', function ($scope, $location, $translate, ForgetPasswordService, SessionService, $uibModal, $uibModalInstance, $log, toaster) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteSuccess', 'FrayteValidation', 'PleaseCorrectValidationErrors', 'ErrorWhileCommunicatingServer', 'EmailAddressNotFound', 'SuccessfullySentMailTo', 'PleaseCheckYourMail']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.Frayte_Success = translations.FrayteSuccess;
            $scope.Frayte_Warning = translations.FrayteWarning;
            

            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextErrorWhileCommunicatingServer = translations.ErrorWhileCommunicatingServer;
            $scope.TextEmailAddressNotFound = translations.EmailAddressNotFound;

            $scope.SuccessfullySentMailTo = translations.SuccessfullySentMailTo;
            $scope.PleaseCheckYourMail = translations.PleaseCheckYourMail;
        });
    };

    $scope.recover = {
        Email: ''
        //,
        //EmployeeId: '',
        //SessionId: '',
        //Role: ''
    };

    $scope.submit = function (isValid, recover) {
        if (isValid) {
            //var userInfo = SessionService.getUser();
            //recover.EmployeeId = userInfo.EmployeeId;
            //recover.SessionId = userInfo.SessionId;
            //recover.Role = userInfo.Role;

            ForgetPasswordService.SendRecoveyMail(recover).then(function (response) {

                if (response.data.Email) {

                    toaster.pop({
                        type: 'success',
                        title: $scope.Frayte_Success,
                        body: $scope.SuccessfullySentMailTo +' '+ response.data.Email + '. ' + $scope.PleaseCheckYourMail,
                        showCloseButton: true
                    });

                    //$location.path("/home");

                    $uibModalInstance.close('ok');

                } else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.Frayte_Warning,
                        body: $scope.TextEmailAddressNotFound,
                        showCloseButton: true
                    });
                }
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorWhileCommunicatingServer,
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.Frayte_Warning,
                body: $scope.TextValidation,
                showCloseButton: true
            });
        }
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };


    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();
    }

    init();

});