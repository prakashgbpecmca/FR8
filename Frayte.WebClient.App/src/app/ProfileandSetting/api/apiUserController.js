angular.module('ngApp.apiUser').controller('ApiController', function (AppSpinner, $scope, SessionService, toaster, ProfileandSettingService, $translate) {

    var setModalOptions = function () {
        $translate(['Frayte-Error', 'FrayteWarning', 'No_API_Available']).then(function (translations) {
            $scope.TitleFrayteWarning = translations.FrayteWarning;
            $scope.TitleFrayteError = translations.FrayteError;
            $scope.No_API_Available = translations.No_API_Available;
        });
    };

    var getInitial = function () {
        ProfileandSettingService.ApiDetail($scope.customerId).then(function (response) {
            if (response.data !== null) {
                $scope.CustomerApi = response.data;
            }
            else {
                toaster.pop({
                    type: 'warning',
                    title: $scope.TitleFrayteWarning,
                    body: $scope.TextSuccessfullyChangedPassword,
                    showCloseButton: true
                });
            }
        }, function () {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteError,
                body: $scope.No_API_Available,
                showCloseButton: true
            });
        });
    };

    function init() {       
        var userInfo = SessionService.getUser();
        $scope.RoleId = userInfo.RoleId;
        if ($scope.RoleId === 3) {
            $scope.customerId = userInfo.EmployeeId;
            getInitial();
        }
        setModalOptions();
    }

    init();
});