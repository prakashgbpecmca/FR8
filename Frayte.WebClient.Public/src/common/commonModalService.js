angular.module('ngApp.common').factory('ModalService', ['$uibModal',
    function ($uibModal) {

        var modalDefaults = {
            backdrop: true,
            keyboard: true,
            modalFade: true,
            templateUrl: 'commonModal.tpl.html'
        };

        var modalOptions = {
            closeButtonVisible: true,
            closeButtonText: 'Cancel',
            actionButtonVisible: true,
            actionButtonText: 'Ok',
            headerText: 'Proceed?',
            bodyText: 'Perform this action?'
        };

        function Confirm(customModalDefaults, customModalOptions) {
            if (!customModalDefaults) {
                customModalDefaults = {};
            }
            customModalDefaults.backdrop = 'static';

            if (!customModalOptions) {
                customModalOptions = {
                };
            }

            customModalOptions.closeButtonVisible = true;
            customModalOptions.closeButtonText = "No";
            customModalOptions.actionButtonVisible = true;
            customModalOptions.actionButtonText = 'Yes';

            return this.show(customModalDefaults, customModalOptions);
        }

        function Alert(customModalDefaults, customModalOptions) {
            if (!customModalDefaults) {
                customModalDefaults = {};
            }
            customModalDefaults.backdrop = 'static';

            if (!customModalOptions) {
                customModalOptions = {
                };
            }
             
            customModalOptions.closeButtonVisible = false;            
            customModalOptions.actionButtonVisible = true;
            customModalOptions.actionButtonText = customModalOptions.okText;

            return this.show(customModalDefaults, customModalOptions);
        }

        function show(customModalDefaults, customModalOptions) {
            //Create temp objects to work with since we're in a singleton service
            var tempModalDefaults = {};
            var tempModalOptions = {};

            //Map angular-ui modal custom defaults to modal defaults defined in service
            angular.extend(tempModalDefaults, modalDefaults, customModalDefaults);

            //Map modal.html $scope custom properties to defaults defined in service
            angular.extend(tempModalOptions, modalOptions, customModalOptions);

            if (!tempModalDefaults.controller) {
                tempModalDefaults.controller = function ($scope, $uibModalInstance) {
                    $scope.modalOptions = tempModalOptions;
                    $scope.modalOptions.ok = function (result) {
                        $uibModalInstance.close(result);
                    };
                    $scope.modalOptions.close = function (result) {
                        $uibModalInstance.dismiss('cancel');
                    };
                };
            }

            return $uibModal.open(tempModalDefaults).result;
        }

        return {
            Confirm: Confirm,
            Alert: Alert,
            show: show
        };

    }]);
