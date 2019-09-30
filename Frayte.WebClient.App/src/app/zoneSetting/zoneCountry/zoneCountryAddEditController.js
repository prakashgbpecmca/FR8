angular.module('ngApp.zoneCountry').controller('ZoneCountryAddEditController', function ($scope, CourierCompany, RateType, ModuleType, $state, LogisticType, $location, zoneCountry, ZoneCountryService, $filter, $translate, SessionService, $uibModal, $log, uiGridConstants, toaster, ModalService, $uibModalInstance, CustomerService) {

    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'ErrorSavingRecord', 'PleaseCorrectValidationErrors',
                    'SuccessfullySavedInformation', 'records', 'ErrorGetting', 'Success', 'FrayteWarning_Validation', 'UpdateRecord_Validation',
                    'PleaseCorrectValidationErrors', 'RateCardSave_Validation', 'Select_CourierAccount', 'THP_Matrix_Saved', 'Transit_Time']).then(function (translations) {
                        $scope.TitleFrayteError = translations.FrayteError;
                        $scope.TitleFrayteInformation = translations.FrayteInformation;
                        $scope.TitleFrayteValidation = translations.FrayteValidation;
                        $scope.TextErrorSavingRecord = translations.ErrorSavingRecord;
                        $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;
                        $scope.RecordGettingError = translations.ErrorGetting + " " + translations.records;
                        $scope.Success = translations.Success;
                        $scope.FrayteWarningValidation = translations.FrayteWarning_Validation;
                        $scope.UpdateRecordValidation = translations.UpdateRecord_Validation;
                        $scope.PleaseCorrectValidationErrors = translations.PleaseCorrectValidationErrors;
                        $scope.RateCardSaveValidation = translations.RateCardSave_Validation;
                        $scope.SelectCourierAccount = translations.Select_CourierAccount;
                        $scope.THPMatrixSaved = translations.THP_Matrix_Saved;
                        $scope.TransitTimeInfo = translations.Transit_Time;
                    });
    };

    $scope.AddCountryToZone = function (country) {
        if (country !== undefined && country !== null && $scope.ZoneCountry !== undefined && $scope.ZoneCountry !== null && $scope.ZoneCountry.Fraytezonezountry !== null) {

            if (country.TransitTime <= 1) {
                country.Day = 'Day';
            }
            else {
                country.Day = 'days';
            }
            $scope.ZoneCountry.Fraytezonezountry.push({
                CountryId: country.CountryId,
                CountryName: country.CountryName,
                TransitTime: country.TransitTime > 0 ? country.TransitTime : 0,
                IsSelected: true,
                Day: country.Day
            });

            if ($scope.Countries !== undefined && $scope.Countries !== null) {
                for (var i = 0; i < $scope.Countries.length; i++) {
                    if ($scope.Countries[i].CountryId === country.CountryId) {
                        $scope.Countries.splice(i, 1);
                        break;
                    }
                }
                for (var j = 0; j < $scope.dataRestore.length; j++) {
                    if ($scope.dataRestore[j].CountryId === country.CountryId) {
                        $scope.dataRestore.splice(j, 1);
                        break;
                    }
                }
                $scope.data = $scope.dataRestore;
            }
        }
    };

    $scope.refreshData = function () {
        $scope.Countries = $filter('filter')($scope.data, $scope.searchText, undefined);
    };

    $scope.submit = function (ZoneCountry) {

        if (ZoneCountry !== undefined && ZoneCountry !== null) {
            ZoneCountryService.SaveZoneCountry(ZoneCountry).then(function (response) {
                $uibModalInstance.close($scope.ZoneLogisticType);
            }, function () {
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.TextErrorSavingRecord,
                    showCloseButton: true
                });
            });
        }
    };

    $scope.removeCountryFromZoneCountry = function (country) {
        if (country !== undefined && country !== null && $scope.ZoneCountry !== undefined && $scope.ZoneCountry !== null && $scope.ZoneCountry.Fraytezonezountry !== null) {
            if ($scope.Countries !== undefined && $scope.Countries !== null) {
                for (var j = 0; j < $scope.Countries.length; j++) {
                    if ($scope.Countries[j].CountryId === country.CountryId) {
                        $scope.Countries.splice(j, 1);
                        $scope.Countries.push({
                            CountryId: country.CountryId,
                            CountryName: country.CountryName
                        });
                        $scope.data = $scope.Countries;
                    }
                }
            }

            for (var i = 0; i < $scope.ZoneCountry.Fraytezonezountry.length; i++) {
                if ($scope.ZoneCountry.Fraytezonezountry[i].CountryId === country.CountryId) {
                    $scope.ZoneCountry.Fraytezonezountry.splice(i, 1);
                    break;
                }
            }
        }
    };

    var getScreenInitials = function (OperationZoneId) {
        var LogisticType = "";
        var LogisticZoneId = $scope.ZoneCountry.ZoneId;
        var CourierCompany = "";
        var RateType = "";
        var ModuleType = "DirectBooking";

        if ($scope.OperationZone !== undefined) {
            OperationZoneId = $scope.OperationZone.OperationZoneId;
        }

        if ($scope.ZoneLogisticType !== undefined) {
            LogisticType = $scope.ZoneLogisticType.Value;
        }
        if ($scope.ZoneCourierCompany !== undefined) {
            CourierCompany = $scope.ZoneCourierCompany.Value;
        }

        if ($scope.ZoneRateType !== undefined) {
            RateType = $scope.ZoneRateType.Value;
        }

        ZoneCountryService.GetCountries(OperationZoneId, LogisticZoneId, LogisticType, CourierCompany, RateType, ModuleType).then(function (response) {
            $scope.Countries = response.data;
            $scope.data = response.data;
            $scope.dataRestore = response.data;
        }, function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.RecordGettingError,
                showCloseButton: true
            });
        });
    };

    $scope.DayChange = function (Country) {

        if (Country.TransitTime <= 1) {
            Country.Day = 'Day';
        }
        else {
            Country.Day = 'days';
        }
    };

    function init() {
        if (LogisticType !== undefined && LogisticType !== null) {
            $scope.ZoneLogisticType = LogisticType;
        }
        if (CourierCompany !== undefined && CourierCompany !== null) {
            $scope.ZoneCourierCompany = CourierCompany;
        }
        if (RateType !== undefined && RateType !== null) {
            $scope.ZoneRateType = RateType;
        }
        if (ModuleType !== undefined) {
            $scope.LogisticModuleType = ModuleType;
        }

        $scope.ZoneCountry = zoneCountry;
        for (i = 0; i < $scope.ZoneCountry.Fraytezonezountry.length ; i++) {

            if ($scope.ZoneCountry.Fraytezonezountry[i].TransitTime <= 1) {
                $scope.ZoneCountry.Fraytezonezountry[i].Day = 'Day';
            }
            else {
                $scope.ZoneCountry.Fraytezonezountry[i].Day = 'days';
            }
        }
        getScreenInitials($scope.ZoneCountry.OperationZoneId);
        setModalOptions();
    }

    init();

});