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

        .state('home.welcome', {
            url: '/welcome',
            templateUrl: 'home/welcome.tpl.html',
            data: { pageTitle: 'Home' }
        })

        .state('home.service', {
            url: '/service',
            templateUrl: 'home/services.tpl.html',
            data: { pageTitle: 'Services' }
        })

        .state('home.client', {
            url: '/client',
            templateUrl: 'home/client.tpl.html',
            data: { pageTitle: 'Client' }
        })

        .state('home.learnMore', {
            url: '/learn-more',
            templateUrl: 'home/learnMore.tpl.html',
            data: { pageTitle: 'Learn More' }
        })

        .state('home.contact', {
            url: '/contact-us',
            templateUrl: 'home/contact.tpl.html',
            data: { pageTitle: 'Contact' }
        })

        .state('home.fuelSurcharge', {
            url: '/fuel-surcharge',
            controller :'HomeFuelSurChargeController',
            templateUrl: 'home/fuelSurcharge.tpl.html',
            data: { pageTitle: 'Fuel Surcharge' }
        })

        .state('home.download', {
            url: '/download',
            templateUrl: 'home/download.tpl.html',
            data: { pageTitle: 'Download' }
        })
         .state('home.impguide', {
             url: '/impguide',
             templateUrl: 'home/impguide.tpl.html',
             data: { pageTitle: 'ImpGuide' }
         })
         .state('home.tracking', {
             url: '/tracking/:carrierType/:trackingId',
             controller: 'HomeTrackingController',
             templateUrl: 'home/tracking.tpl.html',
             data: { pageTitle: 'Tracking' }
         })
         .state('home.tracking-hub', {
             url: '/tracking-hub/:carrierType/:trackingId/:RateType',
             controller: 'HomeTrackingController',
             templateUrl: 'home/trackingnew.tpl.html',
             data: { pageTitle: 'Tracking' }
         })
         .state('home.parcel-hub', {
             url: '/parcel-hub/:carrierType/:trackingId',
             controller: 'HomeTrackingController',
             templateUrl: 'home/parcelHubTracking.tpl.html',
             data: { pageTitle: 'Parcel Hub Tracking' }
         })

        .state('home.new-tracking', {
            url: '/new-tracking',
            controller: 'HomeNewTrackingController',
            templateUrl: 'home/newTracking.tpl.html',
            data: { pageTitle: 'New Tracking' }
        })
        .state('home.tracking-error', {
            url: '/tracking-error/:trackingId',
            templateUrl: 'home/tracking_error.tpl.html',
            data: { pageTitle: 'Tracking Error' }
        })
        // For Manish
        .state('home.bulk-tracking', {
            url: '/bulkTracking',
            controller: 'HomeBulkTrackingController',
            templateUrl: 'home/bulkTracking.tpl.html',
            data: { pageTitle: 'Bulk Tracking' }
        })
        .state('home.bulk-trackingDetail', {
            url: '/bulkTrackingDetail',
            controller: 'HomeTrackingController',
            templateUrl: 'home/bulkTrackingDetail.tpl.html',
            data: { pageTitle: 'Bulk Tracking' }
        })
        // End For Manish

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

    ;
});


