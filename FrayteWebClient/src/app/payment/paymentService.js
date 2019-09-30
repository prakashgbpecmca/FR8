angular.module('ngApp.payment').factory('PaymentService', function ($http, config) {

    var StripeInitiatePayment = function (item) {
        return $http.post(config.SERVICE_URL + '/Stripedemo/StripeInitiatePayment', item);
    };
    var PayWithPaypal = function (item) {
        return $http.post(config.SERVICE_URL + '/Stripedemo/PayPalInitialPaymet', item);
    };
    var SendPaypalMail = function (item) {
        return $http.post(config.SERVICE_URL + '/Stripedemo/SendPaypalMail', item);
    };
    return {
        StripeInitiatePayment: StripeInitiatePayment,
        PayWithPaypal: PayWithPaypal,
        SendPaypalMail: SendPaypalMail
    };

});