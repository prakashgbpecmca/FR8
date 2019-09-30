angular.module('ngApp.common').factory('AppSpinner', function($rootScope) {
    var hideSpinner = function () {
        toggleSpinner(false);
    };

    var showSpinner = function (message) {
        toggleSpinner(true, message);
    };

    var toggleSpinner = function (show, message) {
        $rootScope.$broadcast('spinner.toggle', { show: show, message: message });
    };
    var hideSpinnerTemplate = function () {
        toggleSpinnerTemplate(false);
    };

    var showSpinnerTemplate = function (message,template) {
        toggleSpinnerTemplate(true, message, template);
    };

    var toggleSpinnerTemplate = function (show, message,template) {
        $rootScope.$broadcast('spinner.toggleTemplate', { show: show, message: message, template: template });
    };

    return {
        hideSpinner: hideSpinner,
        showSpinner: showSpinner,
        hideSpinnerTemplate: hideSpinnerTemplate,
        showSpinnerTemplate:showSpinnerTemplate
    };
});

   