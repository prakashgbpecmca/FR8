
angular.module('ngApp.profile').controller('ProfileController', function ($scope, $rootScope, $state, $location, $translate, $filter, LogonService, SessionService, $uibModal, $log, uiGridConstants, toaster, Upload, config) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteSuccess', 'Frayte-Error', 'SuccessfullyChangedPassword', 'FrayteWarning', 'FrayteValidation', 'PleaseCorrectValidationErrors', 'InternalServerErrorOccurred', 'UploadedSuccessfully', 'ErrorOccuredDuringUpload', 'PleaseSelectValidImagefile']).then(function (translations) {

            $scope.TitleFrayteSuccess = translations.FrayteSuccess;
            $scope.TitleFrayteWarning = translations.FrayteWarning;
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteValidation = translations.FrayteValidation;

            $scope.TextPleaseSelectValidImagefile = translations.PleaseSelectValidImagefile;
            $scope.TextSuccessfullyChangedPassword = translations.SuccessfullyChangedPassword;
            $scope.TextValidation = translations.PleaseCorrectValidationErrors;

            $scope.TextInternalServerErrorOccurred = translations.InternalServerErrorOccurred;
            $scope.TextUploadedSuccessfully = translations.UploadedSuccessfully;
            $scope.TextErrorOccuredDuringUpload = translations.ErrorOccuredDuringUpload;
        });
    };

    $scope.user = {
        UserName: '',
        CurrentPassword: '',
        NewPassword: '',
        ConfirmPassword: ''
    };

    $scope.ChangePassword = function (isValid) {
        if (isValid) {
            $scope.user.UserName = $scope.userInfo.EmployeeMail;
            LogonService.changePassword($scope.user).then(function (response) {
                if (response.status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteSuccess,
                        body: $scope.TextSuccessfullyChangedPassword,
                        showCloseButton: true
                    });

                    $scope.user = {
                        UserName: $scope.userInfo.EmployeeMail,
                        CurrentPassword: '',
                        NewPassword: '',
                        ConfirmPassword: ''
                    };

                    $scope.Show = false;

                } else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteWarning,
                        body: $scope.TextInternalServerErrorOccurred,
                        showCloseButton: true
                    });
                }
            }, function () {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.TextInternalServerErrorOccurred,
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

    //This will hide the DIV by default.    
    $scope.ShowHide = function () {
        //If DIV is hidden it will be visible and vice versa.
        $scope.Show = !$scope.Show;
    };

    $scope.chooseImage = function ($files, $file, $event) {

        if ($file !== undefined && $file !== null) {
            if ($file.$error) {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteValidation,
                    body: $scope.PleaseSelectValidImagefile,
                    showCloseButton: true
                });
                return;
            }
            else {
                $scope.uploadProfileImage($file);
            }
        }
    };

    $scope.uploadProfileImage = function (profileImageFile) {
        var files = [];
        if (profileImageFile !== undefined && profileImageFile !== null) {
            files.push(profileImageFile);
        }

        var userInfo = SessionService.getUser();

        $scope.upload = Upload.upload({
            url: config.SERVICE_URL + '/FrayteUser/UploadProfilePhoto',
            fields: {
                UserId: userInfo.EmployeeId
            },
            file: files
        });

        $scope.upload.progress($scope.progress);

        $scope.upload.success($scope.success);

        $scope.upload.error($scope.error);
    };

    $scope.progress = function (evt) {
        //console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
        //toaster.clear();
        //toaster.pop({
        //    type: 'success',
        //    title: 'uploading',
        //    body: 'percent: ' + parseInt(100.0 * evt.loaded / evt.total, 10),
        //    showCloseButton: true
        //});
    };

    $scope.success = function (data, status, headers, config) {
        toaster.pop({
            type: 'success',
            title: $scope.TitleFrayteSuccess,
            body: $scope.TextUploadedSuccessfully,
            showCloseButton: true
        });

        //Need to show the new uploaded image
        $scope.userInfo.PhotoUrl = data;
        SessionService.setUser($scope.userInfo);
        $rootScope.$broadcast('updateUserInfo', $scope.userInfo);
        //$timeout(function () {
        //    $state.go('home.welcome');
        //}, 4000);
    };

    $scope.error = function (err) {
        toaster.pop({
            type: 'error',
            title: $scope.TitleFrayteError,
            body: $scope.TextErrorOccuredDuringUpload,
            showCloseButton: true
        });
    };


    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();

        $scope.Show = false;
        $scope.userInfo = SessionService.getUser();
    }

    init();

});