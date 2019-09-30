/**
 * Controller
 */
angular.module('ngApp.forgetPassword').controller('ForgetPasswordController', function ($timeout, $scope, $state, $location, $translate, ForgetPasswordService, SessionService, $log, toaster) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteSuccess', 'FrayteValidation', 'FrayteWarning', 'PleaseCorrectValidationErrors', 'ErrorWhileCommunicatingServer', 'EmailAddressNotFound', 'SuccessfullySentMailTo', 'PleaseCheckYourMail', 'User_Does_Not_Exist', 'Enter_AccountID_For_Reset_Password']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.Frayte_Success = translations.FrayteSuccess;
            $scope.Frayte_Warning = translations.FrayteWarning;


            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextErrorWhileCommunicatingServer = translations.ErrorWhileCommunicatingServer;
            $scope.TextEmailAddressNotFound = translations.EmailAddressNotFound;

            $scope.SuccessfullySentMailTo = translations.SuccessfullySentMailTo;
            $scope.PleaseCheckYourMail = translations.PleaseCheckYourMail;
            $scope.User_Does_Not_Exist = translations.User_Does_Not_Exist;
            $scope.Enter_AccountID_For_Reset_Password = translations.Enter_AccountID_For_Reset_Password;
        });
    };

    $scope.recover = {
        Email: ''
    };

    $scope.HomeState = function () {
        $state.go('login');
    };

    $scope.changeState = function () {
        $timeout(function () {
            $state.go('login');
        }, 1000);
    };

    $scope.submit = function (isValid, recover) {
        if (isValid) { 
            ForgetPasswordService.SendRecoveyMail(recover).then(function (response) {
                //$scope.UserEmail = response.data.Email;
                if (response.data && response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.Frayte_Success,
                        body: $scope.SuccessfullySentMailTo + ' ' + response.data.Email + '. ' + $scope.PleaseCheckYourMail,
                        showCloseButton: true
                    }); 
                  //  $scope.changeState();
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.Frayte_Warning,
                        body: $scope.TextEmailAddressNotFound,
                        showCloseButton: true
                    });
                }
            }, function (error) { 
                if (error.status === 400 && error.data) {
                    if (error.data.Message === "Enter_Account_Number") {
                        toaster.pop({
                            type: 'warning',
                            title: $scope.Frayte_Warning,
                            body: $scope.Enter_AccountID_For_Reset_Password,
                            showCloseButton: true
                        });
                    } 
                    if (error.data.Message === "Invalid_OperationZone") {
                        toaster.pop({
                            type: 'warning',
                            title: $scope.Frayte_Warning,
                            body: $scope.User_Does_Not_Exist,
                            showCloseButton: true
                        });
                    }
                    if (error.data.Message === "User_Not_Found") {
                        toaster.pop({
                            type: 'warning',
                            title: $scope.Frayte_Warning,
                            body: $scope.User_Does_Not_Exist,
                            showCloseButton: true
                        });
                    }
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextErrorWhileCommunicatingServer,
                        showCloseButton: true
                    });
                } 
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
        $state.go('login');
    };


    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();
    }

    init();

});