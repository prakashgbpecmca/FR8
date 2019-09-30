angular.module('ngApp.home').controller('HomeLoginController', function (AppSpinner, localStorage, OauthService, CurrentUserService, $scope, $location, $translate, $state, $log, LogonService, SessionService, toaster) {
    var setModalOptions = function () {
        $translate(['FrayteValidation', 'ErrorWhileLogin', 'PleaseEnterValidUserPass', 'PleaseCorrectValidationErrors']).then(function (translations) {
            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TextPleaseEnterValidUserPass = translations.PleaseEnterValidUserPass;
            $scope.TextErrorWhileLogin = translations.ErrorWhileLogin;
            $scope.TextPleaseCorrectValidationErrors = translations.PleaseCorrectValidationErrors;
            $scope.UserAccessError = translations.UserAccessError;
            $scope.UserDoesNotExistPleaseVerify = translations.UserDoesNotExistPleaseVerify;
        });
    };


    $scope.submit = function (isValid, credentials) {
        if (isValid) {
            if (!CurrentUserService.profile.token) {
                OauthService.oauthToken(credentials.UserName, credentials.Password).then(
                function (response) {
                    // store token in local storage
                    if (response && response && response.data.access_token) {
                        CurrentUserService.setProfile($scope.credentials.UserName, response.data.access_token);


                        $scope.getuserTabs();
                    }
                    else {
                        toaster.pop({
                            type: 'warning',
                            title: $scope.UserAccessError,
                            body: $scope.UserDoesNotExistPleaseVerify,
                            showCloseButton: true
                        });
                    }

                }, function (error) {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteValidation,
                        body: $scope.TextErrorWhileLogin,
                        showCloseButton: true
                    });
                });
            } else {
                $scope.getuserTabs();
            }

        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextPleaseEnterValidUserPass,
                showCloseButton: true
            });
        }
    };



    $scope.getuserTabs = function (isValid) {
        LogonService.logon($scope.credentials).then(function (response) {
            if (response.data === null) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.UserAccessError,
                    body: $scope.UserDoesNotExistPleaseVerify,
                    showCloseButton: true
                });
            }
            else {
                if (response.data.SessionId) {
                    if (response.data.IsLastLogin) {
                        // if (!$scope.token || !$scope.token.token) {
                        SessionService.setUser({
                            UserName: response.data.UserName,
                            EmployeeName: response.data.EmployeeName,
                            EmployeeId: response.data.EmployeeId,
                            UserType: response.data.UserType,
                            EmployeeMail: response.data.EmployeeMail,
                            SessionId: response.data.SessionId,
                            IsRateShow : response.data.IsRateShow,
                            RoleId: response.data.EmployeeRoleId,
                            EmployeeCustomerId: response.data.EmployeeCustomerId,
                            EmployeeCustomerRoleId: response.data.EmployeeCustomerRoleId,
                            EmployeeCompanyName: response.data.EmployeeCompanyName,
                            EmployeeCompanyLogo: response.data.EmployeeCompanyLogo,
                            PhotoUrl: response.data.PhotoUrl,
                            OperationZoneId: response.data.OperationZoneId,
                            OperationZoneName: response.data.OperationZoneName,
                            ValidDays: response.data.ValidDays,
                            CustomerCurrency: response.data.CustomerCurrency,
                            tabs: response.data.frayteTabs,
                            modules: null,
                            selectedModule: ''
                        });

                        if (response.data.EmployeeCustomerDetail) {
                            SessionService.setUserCustomer({
                                EmployeeTrackingEmail: response.data.EmployeeTrackingEmail,
                                EmployeeCustomerId: response.data.EmployeeCustomerId,
                                EmployeeCustomerRoleId: response.data.EmployeeCustomerRoleId,
                                EmployeeCompanyName: response.data.EmployeeCompanyName,
                                EmployeeCompanyLogo: response.data.EmployeeCompanyLogo,
                                EmployeeCustomerService: response.data.EmployeeCustomerService
                            });
                        }
                         
                        if (response.data.frayteTabs && response.data.frayteTabs.length) {
                            //  $state.go(response.data.frayteTabs[0].route, {}, { reload: true });
                        }
                        $state.go('loginView.services');
                    }
                    else {
                        $state.go("newPassword", { userId: response.data.EmployeeId });
                    }

                } else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteValidation,
                        body: $scope.TextPleaseEnterValidUserPass,
                        showCloseButton: true
                    });
                }
            }
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextErrorWhileLogin,
                showCloseButton: true
            });
        });

    };

    function init() {
        setModalOptions();
        $scope.credentials = {
            UserId: '',
            UserName: '',
            Password: ''
        };
        var USERKEY = "utoken";
        var user = SessionService.getUser();
        if (user) {
            $scope.token = localStorage.get(USERKEY);
            if ($scope.token && $scope.token.token) {
                $scope.credentials.UserName = $scope.token.username;

                $scope.getuserTabs();
            }
        }
    }
    init();
});