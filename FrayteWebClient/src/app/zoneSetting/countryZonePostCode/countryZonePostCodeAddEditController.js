angular.module('ngApp.CountryZonePostCode').controller('CountryZonePostCodeAddEditController', function ($scope, $uibModalInstance, mode, OperationZone, LogisticCompanies, RateType, LogisticType, CountryZonePostCode, TopCountryService, CountryZonePostCodeService, ThirdPartyMatrixService, toaster, AppSpinner, $translate) {
    

    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteSuccess', 'FrayteWarning', 'AssignPostcode_Warning', 'FrayteValidation', 'GetService_Validation', 'Record_Deleted_Successfully', 'ErrorDeletingRecord', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors', 'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteSuccess = translations.FrayteSuccess;
            $scope.TitleFrayteWarning = translations.FrayteWarning;


            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
            $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
            $scope.RecordGettingError = translations.ErrorGetting + " " + translations.records;
            $scope.Success = translations.Success;
            $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
            $scope.UpdateRecordValidation = translations.UpdateRecord_Validation;
            $scope.Record_DeletedSuccessfully = translations.Record_Deleted_Successfully;
            $scope.Error_DeletingRecord = translations.ErrorDeletingRecord;
            $scope.FillRequiredFields = translations.GetService_Validation;
            $scope.AssignPostcodeWarning = translations.AssignPostcode_Warning;
        });
    };
    $scope.GetScreenInitial = function () {
        CountryZonePostCodeService.GetCountryCodeList().then(function (response) {
            
            $scope.CountryData = response.data;
            if ($scope.CountryData) {
                $scope.Countries = TopCountryService.TopCountryList($scope.CountryData);
            }
            if ($scope.Countries) {
                for (i = 0; i < $scope.Countries.length; i++) {
                    if ($scope.CountryZonePostCode.CountryName === $scope.Countries[i].Name) {
                        //$scope.CountryZonePostCodeDetailnew();
                        $scope.CountryZonePostCodeDetail.Country = $scope.Countries[i];
                    }
                }
            }

        }, function () {
            AppSpinner.hideSpinnerTemplate();
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });
        
        CountryZonePostCodeService.GetZones($scope.OperationZoneId, $scope.LogisticType, $scope.LogisticCompany, $scope.RateType, $scope.ModuleType).then(function (response) {
            $scope.Zones = response.data;
        });
        


    };

    $scope.SaveCountryZonePostCode = function (Isvalid) {
        if (Isvalid) {
            if ($scope.CountryZonePostCodeDetail.ToPostCode === '')
            {
                $scope.CountryZonePostCodeDetail.ToPostCode = $scope.CountryZonePostCodeDetail.FromPostCode;
            }
            $scope.CountryZonePostCodeDetail.LogisticCompany = $scope.LogisticCompany;
            $scope.CountryZonePostCodeDetail.OperationZoneId = $scope.OperationZoneId;
            $scope.CountryZonePostCodeDetail.RateType = $scope.RateType;
            $scope.CountryZonePostCodeDetail.LogisticType = $scope.LogisticType;
            $scope.CountryZonePostCodeDetail.ModuleType = $scope.ModuleType;
            $scope.CountryZonePostCodeDetail.CountryName = $scope.CountryZonePostCodeDetail.Country.Name;
            CountryZonePostCodeService.SaveZoneCountryPostCode($scope.CountryZonePostCodeDetail).then(function (response) {
                if (response.data.Status === true) {
                    $uibModalInstance.close();
                    $scope.SaveDataDetail = response.data;
                    if (response.status === 200) {
                        toaster.pop({
                            type: 'success',
                            title: $scope.TitleFrayteSuccess,
                            body: $scope.TextSuccessfullySavedInformation,
                            showCloseButton: true
                        });
                    }
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteWarning,
                        body: $scope.AssignPostcodeWarning,
                        showCloseButton: true
                    });
                }
            }, function () {
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
                title: $scope.TitleFrayteWarning,
                body: $scope.FillRequiredFields,
                showCloseButton: true
            });
        }

    };

    $scope.CountryZonePostCodeDetailnew = function () {
        $scope.CountryZonePostCodeDetail = {};
    };

    function init() {
        $scope.mode = mode;
        $scope.LogisticCompany = LogisticCompanies.Value;
        $scope.OperationZoneId = OperationZone.OperationZoneId;
        $scope.RateType = RateType.Value;
        $scope.LogisticType = LogisticType.Value;
        $scope.CountryZonePostCode = CountryZonePostCode;
        $scope.CountryZonePostCodeDetail = CountryZonePostCode;
        $scope.ModuleType = "DirectBooking";
        $scope.GetScreenInitial();
        setModalOptions();
    }

    init();
});