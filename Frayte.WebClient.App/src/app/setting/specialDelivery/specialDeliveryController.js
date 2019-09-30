/** 
 * Controller
// */
angular.module('ngApp.specialDelivery').controller('SpecialDeliveryController', function ($scope, $state, $translate, uiGridConstants, SettingService, config, $filter, LogonService, SessionService, $uibModal, toaster) {

    //Set Multilingual for Modal Popup
    //var setModalOptions = function () {
    //    $translate(['setting', 'ErrorGetting', 'detail', 'FrayteError']).then(function (translations) {
    //        $scope.TitleFrayteError = translations.FrayteError;
    //        $scope.TextErrorGettingSettingDetail = translations.ErrorGetting + " " + translations.setting + " " + translations.detail;
    //    });
    //};



    function init() {
        // set Multilingual Modal Popup Options
        // setModalOptions();
    }

    init();

});