angular.module('ngApp.customer').controller('CustomerConfiguration', function ($scope, config, $stateParams, ExpressBookingService, $state, CustomerService, AppSpinner, toaster, Upload, $translate) {


    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteSuccess', 'FrayteWarning', 'FrayteError', 'Successfully_Saved_Configuration', 'Error_Saving_Configuration',
        'GettingDataError_Validation', 'CorrectValidationErrorFirst', 'Successfully_Uploaded_Logo', 'Error_Uploading_Logo',
        'Loading_Customer_Configuration', 'Saving_Configuration']).then(function (translations) {
            $scope.FrayteSuccess = translations.FrayteSuccess;
            $scope.FrayteWarning = translations.FrayteWarning;
            $scope.FrayteError = translations.FrayteError;
            $scope.InitialDataValidation = translations.InitialDataValidation;
            $scope.Successfully_Saved_Configuration = translations.Successfully_Saved_Configuration;
            $scope.Error_Saving_Configuration = translations.Error_Saving_Configuration;
            $scope.GettingDataError_Validation = translations.GettingDataError_Validation;
            $scope.Successfully_Uploaded_Logo = translations.Successfully_Uploaded_Logo;
            $scope.Error_Uploading_Logo = translations.Error_Uploading_Logo;
            $scope.Loading_Customer_Configuration = translations.Loading_Customer_Configuration;
            $scope.Saving_Configuration = translations.Saving_Configuration;

            getScreenInitials();

        });
    };


    $scope.save = function (IsValid) {
        if (IsValid) {
            AppSpinner.showSpinnerTemplate($scope.Saving_Configuration, $scope.Template);
            CustomerService.SaveCustomerSetUp($scope.customerConfiguration).then(function (response) {
                AppSpinner.hideSpinnerTemplate();

                if (response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.FrayteSuccess,
                        body: $scope.Successfully_Saved_Configuration,
                        showCloseButton: true
                    });

                    $state.go("loginView.userTabs.customers");

                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.FrayteError,
                        body: $scope.Error_Saving_Configuration,
                        showCloseButton: true
                    });
                }

            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.GettingDataError_Validation,
                    showCloseButton: true
                });
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.CorrectValidationErrorFirst,
                showCloseButton: true
            });
        }
    };

    //Upload Logo 
    $scope.WhileAddingLogo = function ($files, $file, $event) {
        if (!$file) {
            return;
        }
        if ($file.$error) {
            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarning,
                body: $scope.TextPleaseSelectValidFile,
                showCloseButton: true
            });
            return;
        }

        $scope.msdsBatteryFileName = $file.name;

        // Upload the excel file here.
        $scope.uploadLogo = Upload.upload({
            url: config.SERVICE_URL + '/Customer/UploadCustomerLogo',
            file: $file,
            fields: {
                UserId: $scope.customerId
            }
        });

        $scope.uploadLogo.progress($scope.progressuploadLogo);

        $scope.uploadLogo.success($scope.successuploadLogo);

        $scope.uploadLogo.error($scope.erroruploadLogo);
    };
    $scope.progressuploadLogo = function (evt) {
        //To Do:  show excel uploading progress message 
    };
    $scope.successuploadLogo = function (data, status, headers, config) {
        if (status = 200) {
            $scope.customerConfiguration.LogoFileName = data.LogoFileName;
            $scope.customerConfiguration.LogoFilePath = data.LogoFilePath;
            toaster.pop({
                type: 'success',
                title: $scope.FrayteSuccess,
                body: $scope.Successfully_Uploaded_Logo,
                showCloseButton: true
            });
        }
        else {
            toaster.pop({
                type: 'error',
                title: $scope.FrayteError,
                body: $scope.Error_Uploading_Logo,
                showCloseButton: true
            });
        }
    };
    $scope.erroruploadLogo = function (err) {
        toaster.pop({
            type: 'error',
            title: $scope.FrayteError,
            body: $scope.Error_Uploading_Logo,
            showCloseButton: true
        });

    };
    // 

    var getScreenInitials = function () {

        AppSpinner.showSpinnerTemplate($scope.Loading_Customer_Configuration, $scope.Template);
        ExpressBookingService.BookingInitials($scope.userInfo.EmployeeId).then(function (response) {

            $scope.CountryPhoneCodes = response.data.CountryPhoneCodes;

            CustomerService.GetCustomerSetUp($scope.customerId).then(function (response) {

                $scope.customerConfiguration = response.data;

                AppSpinner.hideSpinnerTemplate();

            }, function () {

                AppSpinner.hideSpinnerTemplate();

                toaster.pop({
                    type: 'error',
                    title: $scope.FrayteError,
                    body: $scope.GettingDataError_Validation,
                    showCloseButton: true
                });
            });
        },
   function (response) {
       AppSpinner.hideSpinnerTemplate();
       if (response.status !== 401) {
           toaster.pop({
               type: 'error',
               title: $scope.FrayteError,
               body: $scope.InitialDataValidation,
               showCloseButton: true
           });
       }
   });



    };

    function init() {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        setModalOptions();
        $scope.ImagePath = config.BUILD_URL;
        $scope.title = "Demo";
        $scope.submitted = true;
        $scope.emailFormat = /^[a-z0-9._]+@[a-z]+\.[a-z.]/;
        $scope.customerId = $stateParams.customerId;

    }

    init();

});