angular.module('ngApp.home').controller('HomeTempController', function (AppSpinner, localStorage, OauthService, CurrentUserService, $scope, $location, $translate, $state, $log, LogonService, SessionService, toaster) {
   
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
                            title: 'User Access Error',
                            body: 'User does not exist. Please verify that the User Name and Password entered are correct.',
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
     
    function init() {
      
    }
    init();
});