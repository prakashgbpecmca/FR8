/**
 * Controller
 */
angular.module('ngApp.agent').controller('AddAssociatedController', function ($scope, $uibModal, mode, $translate, associatedUsers, associatedUser, $uibModalInstance, toaster) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteValidation', 'PleaseCorrectValidationErrors']).then(function (translations) {

            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TextValidation = translations.PleaseCorrectValidationErrors;
        });
    };

    $scope.submit = function (isValid, agentAssociateUserDetail) {
        if (isValid) {
            if (mode == 'Add') {
                $scope.AgentAssociateUserDetails.push(agentAssociateUserDetail);
                $uibModalInstance.close($scope.AgentAssociateUserDetails);
            }
            if (mode == 'Modify') {
                $scope.AgentAssociateUserDetail = associatedUser;
                $scope.UpdateAssociatedUsers(agentAssociateUserDetail);
                $uibModalInstance.close($scope.AgentAssociateUserDetails);
            }
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
    $scope.UpdateAssociatedUsers = function (agentAssociateUserDetail) {
        var objects = $scope.AgentAssociateUserDetails;
        for (var i = 0; i < objects.length; i++) {
            if (objects[i].AgentAssociatedUserId === agentAssociateUserDetail.AgentAssociatedUserId) {
                objects[i] = agentAssociateUserDetail;
                break;
            }
        }
    };
    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();

        //$scope.mode = mode;

        // Multilingual Mode
        if (mode === "Add") {
            $translate('Add').then(function (add) {
                $scope.mode = add;
            });
        }
        if (mode === "Modify") {
            $translate('Modify').then(function (modify) {
                $scope.mode = modify;
            });
        }

        $scope.AgentAssociateUserDetails = associatedUsers;

        $scope.AgentAssociateUserDetail = {
            AgentAssociatedUserId: associatedUser.AgentAssociatedUserId,
            AgentId: associatedUser.AgentId,
            UserType: associatedUser.UserType,
            Name: associatedUser.Name,
            Email: associatedUser.Email,
            TelephoneNo: associatedUser.TelephoneNo,
            WorkingStartTime: associatedUser.WorkingStartTime,
            WorkingEndTime: associatedUser.WorkingEndTime,
            WorkingWeekDays: associatedUser.WorkingWeekDays
        };
    }

    init();

});