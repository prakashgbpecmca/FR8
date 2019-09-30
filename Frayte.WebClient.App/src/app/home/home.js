/**
 * Each section of the site has its own module. It probably also has
 * submodules, though this boilerplate is too simple to demonstrate it. Within
 * `src/app/home`, however, could exist several additional folders representing
 * additional modules that would then be listed as dependencies of this one.
 * For example, a `note` section could have the submodules `note.create`,
 * `note.delete`, `note.edit`, etc.
 *
 * Regardless, so long as dependencies are managed correctly, the build process
 * will automatically take take of the rest.
 *
 * The dependencies block here is also where component dependencies should be
 * specified, as shown below.
 */
angular.module('ngApp.home', [
  'ui.router',
  'ngApp.common',
  'ui.grid',
  'ngSanitize',
  'ngAnimate'
])

/**
 * Each section or module of the site can also have its own routes. AngularJS
 * will handle ensuring they are all available at run-time, but splitting it
 * this way makes each module more "self-contained".
 */
.config(function config($stateProvider, $locationProvider) {
    //$locationProvider.html5Mode(true);
    $stateProvider
        .state('home', {
            abstract: true,
            url: '/home',
            controller: 'HomeController',
            templateUrl: 'home/home.tpl.html',
            data: { pageTitle: 'Home' }
        })
        .state('login-app', {
            url: '',
            templateUrl: 'home/homeLogin/homeLogin.tpl.html',
            controller: 'HomeLoginController',
            data: { pageTitle: 'Home' }
        })
         .state('login-chTrad-hk', {
             url: '/locale-chTrad-hk',
             templateUrl: 'home/homeLogin/homeLogin.tpl.html',
             controller: 'HomeLoginController',
             data: { pageTitle: 'Home' }
         })

        .state('login-chTrad-hkcm', {
            url: '/locale-chTrad-hkcm',
            templateUrl: 'home/homeLogin/homeLogin.tpl.html',
            controller: 'HomeLoginController',
            data: { pageTitle: 'Home' }
        })
         .state('login-th-th', {
             url: '/locale-th-th',
             templateUrl: 'home/homeLogin/homeLogin.tpl.html',
             controller: 'HomeLoginController',
             data: { pageTitle: 'Home' }
         })
        .state('login-th', {
            url: '/locale-th',
            templateUrl: 'home/homeLogin/homeLogin.tpl.html',
            controller: 'HomeLoginController',
            data: { pageTitle: 'Home' }
        })
          .state('login-en', {
              url: '/locale-en',
              templateUrl: 'home/homeLogin/homeLogin.tpl.html',
              controller: 'HomeLoginController',
              data: { pageTitle: 'Home' }
          })
          .state('login-chTrad', {
              url: '/locale-chTrad',
              templateUrl: 'home/homeLogin/homeLogin.tpl.html',
              controller: 'HomeLoginController',
              data: { pageTitle: 'Home' }
          })
          .state('login-chSim', {
              url: '/locale-chSim',
              templateUrl: 'home/homeLogin/homeLogin.tpl.html',
              controller: 'HomeLoginController',
              data: { pageTitle: 'Home' }
          })

        .state('login-start', {
             url: '/index.html',
             templateUrl: 'home/homeLogin/homeLogin.tpl.html',
             controller: 'HomeLoginController',
             data: { pageTitle: 'Home' }
         })
        .state('login', {
            url: '/login',
            templateUrl: 'home/homeLogin/homeLogin.tpl.html',
            controller: 'HomeLoginController',
            data: { pageTitle: 'Home' }
        })
          .state('temp', {
              url: '/temp',
              templateUrl: 'home/homeTemp/homeTemp.tpl.html',
              controller: 'HomeTempController',
              data: { pageTitle: 'Home' }
          })
           .state('helpSupport', {
               url: '/help-and-support',
               templateUrl: 'home/homeHelpSupport/helpSupport.tpl.html',
               controller: 'HelpSupportController',
               data: { pageTitle: 'Help & Support' }
           })
         .state('payment', {
             url: '/payment/:frayteNumber',
             templateUrl: 'payment/payment.tpl.html',
             controller: 'PaymentController',
             data: { pageTitle: 'Payment' }
         })
           .state('payment-success', {
               url: '/payment-success',
               templateUrl: 'payment/payment_success.tpl.html',
               controller: 'PaymentSucessController',
               data: { pageTitle: 'Payment-Success' }
           })

        .state('payment-error', {
            url: '/payment-error',
            templateUrl: 'payment/payment_error.tpl.html',
            controller: 'PaymenterrorController',
            data: { pageTitle: 'Payment-Error' }
        })
         .state('page-not-found', {
             url: '/404-page-not-found',
             templateUrl: 'home/error.tpl.html',
             controller:"HomeErrorController",
             data: { pageTitle: 'Not Round' }
         })
          .state('forgotPassword', {
              url: '/forgotPassword',
              templateUrl: 'home/homeForgetPassword/forgetPassword.tpl.html',
              controller: 'ForgetPasswordController',
              data: { pageTitle: 'Forget Password' }
          })
              .state('newPassword', {
                  url: '/newPassword/:userId',
                  templateUrl: 'home/homeNewPassword/homeNewPassword.tpl.html',
                  controller: 'NewPasswordController',
                  data: { pageTitle: 'New Password' }
              })
         .state('resetPassword', {
             url: '/reset-password/:userId/:token',
             templateUrl: 'home/homeForgetPassword/resetPassword.tpl.html',
             controller: 'ResetPasswordController',
             data: { pageTitle: 'Reset Password' }
         })
        .state('resetPasswordSuccesful', {
            url: '/reset-password-succesfull',
            templateUrl: 'home/homeResetPasswordSuccessfully/homeResetPasswordSuccessfully.tpl.html',
            controller: 'homeResetPasswordSuccessfullyController',
            data: { pageTitle: 'Reset Password Successful' }
        })
        .state('home.shipment', {
            abstract: true,
            url: '/shipment/:Id/:UserType',
            controller: 'ShipmentController',
            templateUrl: 'shipment/shipment.tpl.html'
        })

        .state('home.shipment.addressdetail', {
            url: '/addressdetail',
            templateUrl: 'shipment/addressdetail.tpl.html',
            data: { pageTitle: 'Shipment address detail' }
        })

        .state('home.shipment.shipmentdetail', {
            url: '/shipmentdetail',
            templateUrl: 'shipment/shipmentdetail.tpl.html',
            data: { pageTitle: 'Shipment detail' }
        })

        .state('home.shipment.serviceoption', {
            url: '/serviceoption',
            templateUrl: 'shipment/serviceoption.tpl.html',
            data: { pageTitle: 'Shipment service option' }
        })

        .state('home.shipment.confirmshipment', {
            url: '/confirmshipment',
            templateUrl: 'shipment/confirmshipment.tpl.html',
            data: { pageTitle: 'Confirm shipment' }
        })

        .state('home.frayteForwording', {
            url: '/frayteForwording',
            templateUrl: 'home/frayteForwarding.tpl.html',
            data: { pageTitle: 'FRAYTE Forwording' }
        })

      .state('home.e-commerce', {
          url: '/e-commerce',
          templateUrl: 'home/e-Commerce.tpl.html',
          data: { pageTitle: 'eCommerce' }
      })

     .state('home.courier', {
         url: '/courier',
         templateUrl: 'home/Courier.tpl.html',
         data: { pageTitle: 'Courier' }
     })

       .state('home.paperwork', {
           url: '/paperwork',
           templateUrl: 'home/paperWork.tpl.html',
           data: { pageTitle: 'Paperwork' }
       })

        .state('home.transitTime', {
            url: '/transitTime',
            templateUrl: 'home/transitTime.tpl.html',
            data: { pageTitle: 'Transit Time' }
        })

       .state('home.bookingConsignment', {
           url: '/bookingConsignment',
           templateUrl: 'home/bookingConsignment.tpl.html',
           data: { pageTitle: 'Booking Consignment' }
       })

           .state('home.trackingYourConsignment', {
               url: '/trackingYourConsignment',
               templateUrl: 'home/trackingYourConsignment.tpl.html',
               data: { pageTitle: 'Tracking Your Consignment' }
           })

           .state('home.proofofdelivery', {
               url: '/proofofdelivery',
               templateUrl: 'home/proofofdeleivery.tpl.html',
               data: { pageTitle: 'Proof of Delivery' }
           })
    //.state('home.helpSupport', {
    //    url: '/help-and-support',
    //    templateUrl: 'home/homeHelpSupport/helpSupport.tpl.html',
    //    controller: 'HelpSupportController',
    //    data: { pageTitle: 'Help & Support' }
    //})

    //.state('home.resetPasswordSuccessfully', {
    //    url: '/reset-password-successfully',
    //    templateUrl: 'home/homeResetPasswordSuccessfully/homeResetPasswordSuccessfully.tpl.html',
    //    controller: 'homeResetPasswordSuccessfullyController',
    //    data: { pageTitle: 'Reset Password Successfully' }
    //})
    
    ;
});


