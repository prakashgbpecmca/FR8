angular.module('ngApp.baseRateCard').controller('BaseRateCardLogisticSericeDurationController', function ($scope, $uibModalInstance, AppSpinner, $http, $filter, $state, toaster, $translate, $uibModal, ZoneBaseRateCardService, $window, LogisticServiceDuration) {
    
    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteError', 'FrayteInformation', 'FrayteValidation', 'IssedDate_and_ExpiryDate_Updated_Successfully', 'ErrorWhile_Updating_IssedDate_and_ExpiryDate', 'Please_Correct_Validation_Error_First',
        'Updating']).then(function (translations) {

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.Updating = translations.Updating;
            $scope.IssedDate_and_ExpiryDate_Updated_Successfully = translations.IssedDate_and_ExpiryDate_Updated_Successfully;
            $scope.ErrorWhile_Updating_IssedDate_and_ExpiryDate = translations.ErrorWhile_Updating_IssedDate_and_ExpiryDate;
            $scope.Please_Correct_Validation_Error_First = translations.Please_Correct_Validation_Error_First;

        });
    };




    $scope.fn_DateCompare = function (DateA, DateB) {
        if (DateA && DateB) {
            var a = new Date(DateA);
            var b = new Date(DateB);

            var msDateA = Date.UTC(a.getFullYear(), a.getMonth() + 1, a.getDate());
            var msDateB = Date.UTC(b.getFullYear(), b.getMonth() + 1, b.getDate());

            if (parseFloat(msDateA) < parseFloat(msDateB)) {
                return -1;  // less than
            }

            else if (parseFloat(msDateA) == parseFloat(msDateB)) {
                return 0;  // equal
            }

            else if (parseFloat(msDateA) > parseFloat(msDateB)) {
                return 1;  // greater than
            }

            else {
                return null;  // error
            }

        }
        return 10;
    };
    //
    $scope.options = {
        formatYear: 'yy',
        startingDay: 1,
        minDate: new Date()
    };
   var toggleMin = function () {
        var Previous = new Date();
        Previous.setDate(tomorrow.getDate() - 1);

        $scope.options.minDate = $scope.options.minDate ? null : new Date();
    };


    $scope.dateOptions1 = {
        formatYear: 'yy',
        minDate: new Date(),
        startingDay: 1
    };
    $scope.status = {
        opened: false
    };
    $scope.status1 = {
        opened: false
    };
    $scope.OpenCalender = function ($event) {
        $scope.status.opened = true;
    };
    $scope.OpenCalender1 = function ($event) {
        $scope.status1.opened = true;
    };
    $scope.UpdateLogisticServiceDuration = function (IsValid) {
        if (IsValid) {

            AppSpinner.showSpinnerTemplate($scope.Updating, $scope.Template);
            ZoneBaseRateCardService.UpdateLogisticServiceDuration($scope.serviceDuration).then(function (response) {
                AppSpinner.hideSpinnerTemplate();
                if (response.data && response.data.Status) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteInformation,
                        body: $scope.IssedDate_and_ExpiryDate_Updated_Successfully,
                        showCloseButton: true
                    });
                }
                else {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.ErrorWhile_Updating_IssedDate_and_ExpiryDate,
                        showCloseButton: true
                    });
                }
                $uibModalInstance.close();
            }, function () {
                AppSpinner.hideSpinnerTemplate();
                toaster.pop({
                    type: 'error',
                    title: $scope.TitleFrayteError,
                    body: $scope.ErrorWhile_Updating_IssedDate_and_ExpiryDate,
                    showCloseButton: true
                });
                $uibModalInstance.close();
            });
        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.Please_Correct_Validation_Error_First,
                showCloseButton: true
            });
        }
    };

    function init() {
        $scope.Template = 'directBooking/ajaxLoader.tpl.html';
        $scope.submitted = true;
        if (LogisticServiceDuration) {
            $scope.serviceDuration = LogisticServiceDuration;
            if (LogisticServiceDuration.IssuedDate) {
                $scope.serviceDuration.IssuedDate = moment.utc(LogisticServiceDuration.IssuedDate).toDate();
            }
            if (LogisticServiceDuration.ExpiryDate) {
                $scope.serviceDuration.ExpiryDate = moment.utc(LogisticServiceDuration.ExpiryDate).toDate();
            }
        }
        setModalOptions();
    }

    init();
});