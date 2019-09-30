/**
 * Controller
 */
angular.module('ngApp.logon').controller('LogonController', function ($scope, $location, $state, $translate, LogonService, SessionService, $uibModal, $uibModalInstance, $log, toaster) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteValidation', 'FrayteWarning', 'ErrorWhileLogin', 'PleaseEnterValidUserPass', 'PleaseCorrectValidationErrors', 'UserAccessError', 'VerifyUserNamePasswordCorrectly']).then(function (translations) {
            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.TextPleaseEnterValidUserPass = translations.PleaseEnterValidUserPass;
            $scope.TextErrorWhileLogin = translations.ErrorWhileLogin;
            $scope.TextPleaseCorrectValidationErrors = translations.PleaseCorrectValidationErrors;
            $scope.UserAccessError = translations.UserAccessError;
            $scope.VerifyUserNamePasswordCorrectly = translations.VerifyUserNamePasswordCorrectly;
        });
    };

    $scope.credentials = {
        UserName: '',
        Password: ''
    };

    $scope.closeModal = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.submit = function (isValid, credentials) {
        setModalOptions();
        if (isValid) {
            LogonService.logon(credentials).then(function (response) {
                if (response.data === null) {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.UserAccessError,
                        body: $scope.VerifyUserNamePasswordCorrectly,
                        showCloseButton: true
                    });
                }
                else {
                    if (response.data.SessionId) {
                        SessionService.setUser({
                            UserName: response.data.EmployeeName,
                            EmployeeId: response.data.EmployeeId,
                            EmployeeMail: response.data.EmployeeMail,
                            SessionId: response.data.SessionId,
                            RoleId: response.data.EmployeeRoleId,
                            PhotoUrl: response.data.PhotoUrl,
                            OperationZoneId: response.data.OperationZoneId,
                            OperationZoneName: response.data.OperationZoneName,
                            tabs: response.data.frayteTabs
                        });

                      

                        if (response.data.frayteTabs && response.data.frayteTabs.length) {

                            $uibModalInstance.close('ok');

                        }

                        //if (response.data.EmployeeRoleId === 1) {//Admin
                        //    $state.go('admin.booking-home.booking-welcome', {}, {reload:true});
                        //}
                        //else if (response.data.EmployeeRoleId === 3) {//Customer                        
                        //    $state.go('customer.booking-home.booking-welcome');
                        //}
                        //else if (response.data.EmployeeRoleId === 5) {//Shipper                        
                        //    $state.go('shipper.current-shipment');
                        //}
                        //else if (response.data.EmployeeRoleId === 6) {//Staff
                        //    $state.go('dbuser.booking-home.booking-welcome');
                        //}
                        //else if (response.data.EmployeeRoleId === 7) {//Warehouse
                        //    $state.go('user.current-shipment');
                        //}
                        //else if (response.data.EmployeeRoleId === 8) {// HSCode Operator
                        //    $state.go('hsuser.hs-code-operation');
                        //}
                        //else if (response.data.EmployeeRoleId === 9) {//Mastaer - Admin
                        //    $state.go('msadmin.booking-home.booking-welcome', {}, { reload: true });
                        //}

                    } else {
                        //toaster.pop('warning', "Frayte-Validation", "Please enter valid user name/password");
                        toaster.pop({
                            type: 'warning',
                            title: $scope.FrayteWarning,
                            body: $scope.TextPleaseEnterValidUserPass,
                            showCloseButton: true
                        });
                    }
                }
            }, function () {
                //toaster.pop('error', "Frayte-Validation", "Error while login");
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteWarning,
                    body: $scope.TextErrorWhileLogin,
                    showCloseButton: true
                });
            });
        }
        else {
            //toaster.pop('warning', "Frayte-Validation", "Please correct the validation errors first.");            
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.TextPleaseCorrectValidationErrors,
                showCloseButton: true
            });
        }
    };

    $scope.forgetPassword = function () {
        $uibModalInstance.dismiss('cancel');

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'forgetPassword/forgetPassword.tpl.html',
            controller: 'ForgetPasswordController',
            windowClass: 'ForgetPass-Modal',
            size: 'sm'
        });

      
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    function init() {

        $scope.url = SessionService.GetSiteURL();

        // set Multilingual Modal Popup Options
        setModalOptions();
    }

    init();

});