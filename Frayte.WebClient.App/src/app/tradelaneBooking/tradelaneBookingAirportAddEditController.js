angular.module('ngApp.tradelaneBooking').controller('TradelaneBookingAirportAddEditController', function ($scope, CountryId, toaster, $translate, TradelaneBookingService, $uibModalInstance, $timeout) {

    $scope.airport = 'Airport';

    var setMultilingualOtions = function () {
        $translate(['FrayteWarning', 'FrayteSuccess', 'FrayteWarning_Validation', 'Airport_Save',
                    'SomeErrorOccuredTryAgain', 'CorrectValidationErrorFirst']).then(function (translations) {
                    $scope.TitleFrayteWarning = translations.FrayteWarning;
                    $scope.Airport_Save = translations.Airport_Save;
                    $scope.TitleFrayteInformation = translations.FrayteSuccess;
                    $scope.CorrectValidationErrorFirst = translations.CorrectValidationErrorFirst;
                    $scope.SomeErrorOccuredTryAgain = translations.SomeErrorOccuredTryAgain;                   
                });
    };

    $scope.SaveAirport = function (IsValid, newAirport) {
        if (IsValid) {
            TradelaneBookingService.addAirport(CountryId, newAirport.AirportName, newAirport.AirportCode).then(function (response) {
                if (response.data !== undefined && response.data !== null && response.data !== '' && response.data === true) {
                    $timeout(function () {
                        toaster.pop({
                            type: 'success',
                            title: $scope.TitleFrayteInformation,
                            body: $scope.Airport_Save,
                            showCloseButton: true
                        });                        
                    }, 2000);
                    $uibModalInstance.close({ Status: response.data, Airport: newAirport });
                }
                else {
                    toaster.pop({
                        type: 'warning',
                        title: $scope.TitleFrayteWarning,
                        body: $scope.SomeErrorOccuredTryAgain,
                        showCloseButton: true
                    });
                }
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteWarning,
                body: $scope.CorrectValidationErrorFirst,
                showCloseButton: true
            });
        }
    };

    function init() {

        setMultilingualOtions();
        $scope.submitted = true;
    }

    init();

});