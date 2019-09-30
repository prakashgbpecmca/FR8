angular.module('ngApp.payment', [
  'ui.router',
  'ngApp.common'
])

/**
 * Each section or module of the site can also have its own routes. AngularJS
 * will handle ensuring they are all available at run-time, but splitting it
 * this way makes each module more "self-contained".
 */
.config(function config($stateProvider, $locationProvider) {
    //$locationProvider.html5Mode(true);
    $stateProvider
         .state('payment', {
             url: '/payment',
             controller: 'PaymentController',
             templateUrl: 'payment/payment.tpl.html',
             data: { pageTitle: 'Payment' }
         })
        .state('payment-success', {
            url: '/payment-success',
            //controller: 'PaymentController',
            templateUrl: 'payment/payment_success.tpl.html',
            data: { pageTitle: 'Payment-Success' }
        })
     .state('payment-error', {
         url: '/payment-error',
         //controller: 'PaymentController',
         templateUrl: 'payment/paymentError.tpl.html',
         data: { pageTitle: 'Payment-Error' }
     })

    ;
});