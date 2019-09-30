angular.module('ngApp.customer').controller('ManifestTrackingController', function ($scope, SessionService, CustomerService, manifestObj, toaster, $translate, $uibModalInstance) {

    var setModalOptions = function () {
        $translate(['Frayte_Error', 'Frayte_Success', 'ErrorGettingRecord', 'SuccessfullySavedInformation', 'ErrorSavingRecored']).then(function (translations) {

            $scope.FrayteError = translations.Frayte_Error;
            $scope.FrayteSuccess = translations.Frayte_Success;
            $scope.ErrorGettingRecord = translations.ErrorGettingRecord;
            $scope.Successfully_SavedInformation = translations.SuccessfullySavedInformation;
            $scope.ErrorSaving_Recored = translations.ErrorSavingRecored;

        });
    };

    $scope.SaveManifestTracking = function (Isvalid, Detail) {
        if(Isvalid){
        CustomerService.SaveManifestTracking($scope.ManifestTrackingDetail).then(function (response) {
            var result = response.data;
            toaster.pop({
            type: 'success',
            title: $scope.FrayteSuccess,
            body: $scope.Successfully_SavedInformation,
            showCloseButton: true
        });
            $uibModalInstance.close();
        }, function () {
            toaster.pop({
            type: 'error',
            title: $scope.FrayteError,
            body: $scope.ErrorSaving_Recored,
            showCloseButton: true
        });
        });
    }
    else{

    }
    };

    function init() {

        var userInfo = SessionService.getUser();
        $scope.CustomerId = userInfo.EmployeeId;

        $scope.ManifestTrackingDetail = {
            TrackingDescription: "",
            TrackingDescriptionCode: "Code",
            ManifestId: manifestObj.ManifestId,
            UserId: $scope.CustomerId,
            TrackingMode: "Manual",
            CreatedBy: $scope.CustomerId
        };
        setModalOptions();

    }

    init();
});