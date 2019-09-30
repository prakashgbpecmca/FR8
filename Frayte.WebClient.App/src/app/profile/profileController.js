
angular.module('ngApp.profile').controller('ProfileController', function ($scope, ShipmentService, $rootScope, $state, $location, $translate, $filter, LogonService, SessionService, $uibModal, $uibModalInstance, $log, uiGridConstants, toaster, Upload, config, CustomerService) {

    $scope.checkPasswordValidation = function (str) {
        if (str) {
            if (str.search(/[A-Z]/) < 0) {
                $scope.message = $scope.Passwordcontainonecapitalletter;
                $scope.error = true;
            }
            else if (str.search(/[0-9]/) < 0) {
                $scope.message = $scope.Passwordcontainonedigit;
                $scope.error = true;
            }
            else if (str.length < 8) {
                $scope.message = $scope.Passwordmust8characters;
                $scope.error = true;
            }
            else {
                $scope.error = false;
            }
        }
    };
    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteSuccess', 'Frayte-Error', 'SuccessfullyChangedPassword', 'FrayteWarning', 'FrayteValidation', 'Please_Correct_Validation_Error_First', 'InternalServerErrorOccurred', 'UploadedSuccessfully', 'ErrorOccuredDuringUpload', 'PleaseSelectValidImagefile',
        'Passwordcontain_onecapitalletter', 'Passwordcontain_onedigit', 'Passwordmust8_characters']).then(function (translations) {

            $scope.TitleFrayteSuccess = translations.FrayteSuccess;
            $scope.TitleFrayteWarning = translations.FrayteWarning;
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TextPleaseSelectValidImagefile = translations.PleaseSelectValidImagefile;
            $scope.TextSuccessfullyChangedPassword = translations.SuccessfullyChangedPassword;
            $scope.TextValidation = translations.Please_Correct_Validation_Error_First;
            $scope.TextInternalServerErrorOccurred = translations.InternalServerErrorOccurred;
            $scope.TextUploadedSuccessfully = translations.UploadedSuccessfully;
            $scope.TextErrorOccuredDuringUpload = translations.ErrorOccuredDuringUpload;
            $scope.Passwordcontainonecapitalletter = translations.Passwordcontain_onecapitalletter;
            $scope.Passwordcontainonedigit = translations.Passwordcontain_onedigit;
            $scope.Passwordmust8characters = translations.Passwordmust8_characters;
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
            $scope.user.UserName = $scope.userInfo.UserName;
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
                    $uibModalInstance.close();

                } else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteWarning,
                        body: $scope.TextInternalServerErrorOccurred,
                        showCloseButton: true
                    });
                }
            }, function () {
                toaster.pop({
                    type: 'error',
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
            url: config.SERVICE_URL + '/Account/UploadProfilePhoto',
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
    $scope.SetPhoneCodeInfo = function (Country) {
        if (Country !== undefined && Country !== null) {
            for (var i = 0 ; i < $scope.countryPhoneCodes.length ; i++) {
                if ($scope.countryPhoneCodes[i].CountryCode === Country.Code) {
                    $scope.code = "(+" + $scope.countryPhoneCodes[i].PhoneCode + ")";
                    break;
                }
            }
        }
    };
    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();
        $scope.Show = false;
        $scope.userInfo = SessionService.getUser();
        ShipmentService.GetInitials().then(function (response) {
            $scope.countryPhoneCodes = response.data.CountryPhoneCodes;
            if ($scope.countryPhoneCodes !== undefined && $scope.countryPhoneCodes !== null &&  $scope.countryPhoneCodes.length > 0) {
                CustomerService.GetCustomerDetail($scope.userInfo.EmployeeId).then(function (response) {
                    $scope.CustomerData = response.data;
                    if ($scope.CustomerData) {
                        if ($scope.CustomerData.RoleId === 3 || $scope.CustomerData.RoleId === 17) {
                            $scope.CustomerData.AccountNumber = $scope.CustomerData.AccountNumber.substring(0, 3) + "-" + $scope.CustomerData.AccountNumber.substring(3, 6) + "-" + $scope.CustomerData.AccountNumber.substring(6, 9);
                        }
                        $scope.SetPhoneCodeInfo($scope.CustomerData.UserAddress.Country);
                    }
                });
            }
        });
    }

    init();

});