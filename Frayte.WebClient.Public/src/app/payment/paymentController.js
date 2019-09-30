angular.module("ngApp.payment").controller("PaymentController", function ($window,config) {
    function init() {
        window.location.href = config.Payment_Link;
    }
    init();
});