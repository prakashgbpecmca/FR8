angular.module('ngApp.courierAccount').controller('CourierAccountAddEditController', function ($scope, mode, CourierAccountService, CourierService, $state, $translate, SessionService, toaster, uiGridConstants, $uibModal, $uibModalInstance, CourierAccount, ModalService) {

    $scope.CourierAccount = {
        LogisticServiceCourierAccountId: 0,
        IntegrationAccountId: '',
        AccountNo: '',
        AccountCountryCode: '',
        Description: '',
        ColorCode: '',
        IsActive: false,
        OperationZone: {
            OperationZoneId: null,
            OperationZoneName: ''
        },
        LogisticService: {
            LogisticServiceId: 0,
            OperationZoneId: null,
            LogisticCompany: '',
            LogisticCompanyDisplay: '',
            LogisticType: '',
            LogisticTypeDisplay: '',
            RateType: '',
            RateTypeDisplay: '',
            ModuleType: 'DirectBooking'
        }
    };
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors',
            'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'FrayteSuccess', 'FrayteWarning_Validation', 'UpdateRecord_Validation',
            'PleaseCorrectValidationErrors', 'RateCardSave_Validation', 'Select_CourierAccount', 'THP_Matrix_Saved', 'DataSaved_Validation',
        'Record_Saved', 'APIkeyUserkey_Saved_Validation', 'ParcelHubKeysSavingErrorValidation', 'PleaseTryLater', 'ParcelHubKeyRemoved_Validation',
        'PacelHub', 'Keys', 'SelectLogisticType_Validation', 'Cancel', 'Confirmation', 'Cancel_Validation']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
            $scope.RecordGettingError = translations.ErrorGetting + " " + translations.records;
            $scope.Success = translations.FrayteSuccess;
            $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
            $scope.PleaseCorrectValidationErrors = translations.PleaseCorrectValidationErrors;
            $scope.RecordSaved = translations.Record_Saved;
            $scope.APIkeyUserkeySavedValidation = translations.APIkeyUserkey_Saved_Validation;
            $scope.ParcelHubKeysSavingErrorValidation = translations.ParcelHubKeysSavingError_Validation;
            $scope.ParcelHubKeyRemovedTryLaterValidation = translations.ParcelHubKeysSavingError_Validation + " " + translations.PleaseTryLater;
            $scope.ParcelHubKeyGettingValidation = translations.ErrorGetting + " " + translations.PacelHub + " " + translations.Keys;
            $scope.SelectLogisticTypeValidation = translations.SelectLogisticType_Validation;
            $scope.CancelConfirmation = translations.Cancel + " " + translations.Confirmation;
            $scope.CancelValidation = translations.Cancel_Validation;

        });
    };

    $scope.CourierAccountByOperationZone = function () {
        $scope.GetInfo();
    };


    $scope.changeLabelForparcelHub = function (value) {
        if (value !== undefined && value !== null && value.Name === "UK/EU - Shipment") {
            $scope.IsParcel = true;
        }
        else {
            $scope.IsParcel = false;
        }
    };

    $scope.submit = function (IsValid, CourierAccount) {


        if (IsValid) {
            debugger;
                $scope.CourierAccount.LogisticService.OperationZoneId = CourierAccount.OperationZone.OperationZoneId;
                $scope.CourierAccount.LogisticService.LogisticCompany = $scope.CourierCompany.Value;
                $scope.CourierAccount.LogisticService.LogisticType = $scope.LogisticType1.Value;
                if ($scope.RateType1 !== undefined) {
                    $scope.CourierAccount.LogisticService.RateType = $scope.RateType1.Value;
                }
                else {
                    $scope.CourierAccount.LogisticService.RateType = null;
                }
            CourierAccountService.SaveCourierAccount(CourierAccount).then(function () {


                $uibModalInstance.close(CourierAccount);
                toaster.pop({
                    type: 'success',
                    title: $scope.Success,
                    body: $scope.RecordSaved,
                    showCloseButton: true
                });
            }, function () {
                
                //toaster.pop('error', "Frayte-Validation", "Error while login");
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorSavingRecord,
                    showCloseButton: true
                });
            });
        }

        else {

            toaster.pop({
                type: 'warning',
                title: $scope.FrayteWarningValidation,
                body: $scope.PleaseCorrectValidationErrors,
                showCloseButton: true
            });
        }
    };

    $scope.CourierAccGoBack = function () {
        var modalOptions = {
            headerText: $scope.CancelConfirmation,
            bodyText: $scope.CancelValidation
        };

        ModalService.Confirm({}, modalOptions).then(function () {
            //if ($state.is('admin.courier-accounts.easypost')) {
            //    $state.go('admin.courier-accounts.easypost', null, { reload: true });
            //    $rootScope.getCourierAccounts();
            //}
            //else if ($state.is('admin.courier-accounts.parcel-hub')) {
            //    $state.go('admin.courier-accounts.parcel-hub', null);
            //    $rootScope.getCourierAccounts();
            //}
            $uibModalInstance.close(CourierAccount);
        }, function () {

        });
    };

  


    $scope.GetInfo = function () {
        CourierAccountService.GetInfo($scope.CourierAccount.OperationZone.OperationZoneId).then(function (response) {
            
            $scope.LogisticCompanies = response.data.LogisticCompanies;
            if (mode === "Modify") {
            for (var i = 0; i < $scope.LogisticCompanies.length; i++) {
                if ($scope.CourierAccount.LogisticService.LogisticCompany === $scope.LogisticCompanies[i].Value) {
                    $scope.CourierCompany = $scope.LogisticCompanies[i];
                    break;
                }
            }
            }
            else {
                $scope.CourierCompany = $scope.LogisticCompanies[0];
            }
            $scope.LogisticRateTypes = response.data.LogisticRateTypes;
            if (mode === "Edit") {
                for (var j = 0; j < $scope.LogisticRateTypes.length; j++) {
                    if ($scope.CourierAccount.LogisticService.RateType === $scope.LogisticRateTypes[j].Value) {
                        $scope.RateType1 = $scope.LogisticRateTypes[j];
                        break;
                    }
                }
            }
            else{
                $scope.RateType1 = $scope.LogisticRateTypes[0];
            }
            $scope.LogisticTypes = response.data.LogisticTypes;
            if (mode === "Modify") {
                for (var k = 0; k < $scope.LogisticTypes.length; k++) {
                    if ($scope.CourierAccount.LogisticService.LogisticType === $scope.LogisticTypes[k].Value) {
                        $scope.LogisticType1 = $scope.LogisticTypes[k];
                        break;
                    }
                }
            }
            else {
                $scope.LogisticType1 = $scope.LogisticTypes[0];
            }
           
            $scope.ModuleType = response.data.ModuleType;

           

        });
    };

    function init() {
        $scope.EasyPostCourierAccountsList = [];
        $scope.ParcelHubCourierAccountsList = [];
        $scope.LogisticTypeValue = "";
        $scope.IsLogisticTypeSelected = false;
      
        if (mode !== undefined) {
            $translate(mode).then(function (mode) {
                $scope.Mode = mode;
            });
        }
        CourierAccountService.GetOperationZones().then(function (response) {
            $scope.OperationZones = response.data;
            $scope.OperationZoneId = response.data.OperationZoneId;

            if ($scope.OperationZones) {
                if (mode === "Add") {
                    $scope.CourierAccount.OperationZone = $scope.OperationZones[0];
                    $scope.GetInfo();
                }
                else {
                    $scope.CourierAccount.OperationZone = CourierAccount.OperationZone;
                    $scope.GetInfo();
                }
               
            }


        }, function () {

        });

      
        $scope.CourierAccount = CourierAccount;

        $scope.CourierAccount.OperationZone = CourierAccount.OperationZone;
        
        
        if (mode === "Add")
        {

        }
        else {
            $scope.CourierAccount.OperationZone = CourierAccount.OperationZone;
        }
     
        setModalOptions();
    }

    init();

});