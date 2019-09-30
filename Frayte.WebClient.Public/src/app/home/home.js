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
  'ngApp.common'
])

/**
 * Each section or module of the site can also have its own routes. AngularJS
 * will handle ensuring they are all available at run-time, but splitting it
 * this way makes each module more "self-contained".
 */
.config(function config($stateProvider) {

    $stateProvider
        .state('payment', {
            url: '/payment',
            controller: 'PaymentController',
            //templateUrl: 'home/home.tpl.html',
            data: { pageTitle: 'Payment' }
        })
        .state('home', {
            abstract: true,
            url: '',
            controller: 'HomeController',
            templateUrl: 'home/home.tpl.html',
            data: { pageTitle: 'Home' }
        })

        // in local when app starts the url generated = "/index.html" 
        .state('home.welcome', {
            url: '',
            templateUrl: 'home/welcome.tpl.html',
            data: { pageTitle: 'Home' }
        })

        //// in live index.html we don't not show on url 
         .state('home.start', {
             url: '/',
             templateUrl: 'home/welcome.tpl.html',
             data: { pageTitle: 'Home' }
         })
          .state('home.login-start', {
              url: '/index.html',
              templateUrl: 'home/welcome.tpl.html',
              data: { pageTitle: 'Home' }
          })
         .state('page-not-found', {
             url: '/404-page-not-found',
             templateUrl: 'home/error.tpl.html',
             data: { pageTitle: '404 Page Not Found' }
         })
        //.state('home.welcome', {
        //    url: '/welcome',
        //    templateUrl: 'home/welcome.tpl.html',
        //    data: { pageTitle: 'Home' }
        //})

        .state('home.service', {
            url: '/services',
            templateUrl: 'home/services.tpl.html',
            data: { pageTitle: 'Services' }
        })

        .state('home.client', {
            url: '/client',
            templateUrl: 'home/client.tpl.html',
            data: { pageTitle: 'Client' }
        })

        .state('home.learnMore', {
            url: '/restricted-and-prohibited-items',
            templateUrl: 'home/learnMore.tpl.html',
            data: { pageTitle: 'Learn More' }
        })

        .state('home.contact', {
            url: '/contact',
            templateUrl: 'home/contact.tpl.html',
            data: { pageTitle: 'Contact' }
        })

        .state('home.fuelSurcharge', {
            url: '/fuel-surcharge',
            controller: 'HomeFuelSurChargeController',
            templateUrl: 'home/fuelSurcharge.tpl.html',
            data: { pageTitle: 'Fuel Surcharge' }
        })

        .state('home.download', {
            url: '/download-forms',
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
         //.state('home.tracking-hub', {
         //    url: '/tracking-hub/:carrierType/:trackingId/:RateType',
         //    controller: 'HomeTrackingController',
         //    templateUrl: 'home/trackingnew.tpl.html',
         //    data: { pageTitle: 'Tracking' }
         //})
         //.state('home.parcel-hub', {
         //    url: '/parcel-hub/:carrierType/:trackingId',
         //    controller: 'HomeTrackingController',
         //    templateUrl: 'home/parcelHubTracking.tpl.html',
         //    data: { pageTitle: 'Parcel Hub Tracking' }
         //})
          .state('home.tracking-hub', {
              url: '/tracking-detail/:carrierType/:trackingId/:RateType',
              controller: 'HomeTrackingController',
              templateUrl: 'home/trackingnew.tpl.html',
              data: { pageTitle: 'Tracking' }
          })
         .state('home.parcel-hub', {
             url: '/parcel-detail/:carrierType/:trackingId',
             controller: 'HomeTrackingController',
             templateUrl: 'home/parcelHubTracking.tpl.html',
             data: { pageTitle: 'Parcel Hub Tracking' }
         })

           .state('home.tradelanetracking-hub', {
               url: '/tracking-detail/:SearchNumber',
               controller: 'HomeTradelaneTrackingController',
               templateUrl: 'home/tradelaneTracking.tpl.html',
               data: { pageTitle: 'Tracking Detail' }
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
          .state('home.airlineULDSpecifications', {
              url: '/airline-uld-specification',
              controller: 'homeairlineULDController',
              templateUrl: 'home/homeairlineULDSpecifications.tpl.html',
              data: { pageTitle: 'Airline ULD Specifications' }
          })
          .state('home.aircraftPayload', {
              url: '/aircraft-payload',
              controller: 'homeAircraftPayloadController',
              templateUrl: 'home/homeAircraftPayload.tpl.html',
              data: { pageTitle: 'Aircraft Payload' }
          })
          .state('home.cargoFreighterSpecification', {
              url: '/cargo-freighter-specifications',
              controller: 'homeCargoFreighterSpecificationsController',
              templateUrl: 'home/homeCargoFreighterSpecifications.tpl.html',
              data: { pageTitle: 'Cargo Freighter Specifications' }
          })
          .state('home.shippingContainerSpecifications', {
              url: '/shipping-container-specifications',
              controller: 'homeShippingContainerSpecificationsController',
              templateUrl: 'home/homeShippingContainerSpecifications.tpl.html',
              data: { pageTitle: 'Shipping Container Specifications' }
          })
          .state('home.airCraftConfiguration', {
              url: '/air-craft-configuration',
              controller: 'homeAirCraftConfigurationController',
              templateUrl: 'home/homeAirCraftConfiguration.tpl.html',
              data: { pageTitle: 'Air Craft Configuration' }
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
            url: '/service',
            templateUrl: 'home/frayteForwarding.tpl.html',
            data: { pageTitle: 'Service' }
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
           url: '/paper-work',
           templateUrl: 'home/paperWork.tpl.html',
           data: { pageTitle: 'Paperwork' }
       })

        .state('home.transitTime', {
            url: '/transit-time',
            templateUrl: 'home/transitTime.tpl.html',
            data: { pageTitle: 'Transit Time' }
        })

       .state('home.bookingConsignment', {
           url: '/booking-consignment',
           templateUrl: 'home/bookingConsignment.tpl.html',
           data: { pageTitle: 'Booking Consignment' }
       })

           .state('home.trackYourConsignment', {
               url: '/tracking-your-consignment',
               templateUrl: 'home/trackingYourConsignment.tpl.html',
               data: { pageTitle: 'Tracking Your Consignment' }
           })

           .state('home.proofofdelivery', {
               url: '/proof-of-delivery',
               templateUrl: 'home/proofofdeleivery.tpl.html',
               data: { pageTitle: 'Proof of Delivery' }
           })

        .state('home.privacyPolicy', {
            url: '/privacy-policy',
            templateUrl: 'home/privacyPolicy.tpl.html',
            data: { pageTitle: 'Privacy Policy' }
        })

       .state('home.packaging', {
           url: '/packaging',
           templateUrl: 'home/packaging.tpl.html',
           data: { pageTitle: 'Packaging' }
       })

          .state('home.lithiumBatteries', {
              url: '/lithium-batteries',
              templateUrl: 'home/lithiumBatteries.tpl.html',
              data: { pageTitle: 'Lithium Batteries' }
          })

    .state('home.about-us', {
        url: '/about-us',
        templateUrl: 'home/aboutus.tpl.html',
        data: { pageTitle: 'About us' }
    })
      .state('home.direct-booking-detail', {
          url: '/direct-booking-detail',
          templateUrl: 'home/homeDirectBookingDetail.tpl.html',
          controller: 'homeDirectBookingDetailController',
          data: { pageTitle: 'Direct Booking Detail' }
      })
      .state('home.tradelane-detail', {
          url: '/tradelane-detail',
          templateUrl: 'home/homeTradelaneDetail.tpl.html',
          controller: 'homeTradelaneDetailController',
          data: { pageTitle: 'Tradelane Detail' }
      })
      .state('home.break-bulk', {
          url: '/break-bulk',
          templateUrl: 'home/homeBreakbulk.tpl.html',
          data: { pageTitle: 'Break Bulk' }
      })
      .state('home.ecommerce', {
          url: '/ecommerce',
          templateUrl: 'home/homeEcommerce.tpl.html',
          data: { pageTitle: 'Ecommerce' }
      })
      .state('home.warehouse-transport', {
          url: '/warehouse-transport',
          templateUrl: 'home/homeWarhouseTransport.tpl.html',
          data: { pageTitle: 'Warehouse & Transport' }
      })
      .state('home.express-solutions', {
          url: '/express-solutions',
          templateUrl: 'home/homeExpressSolutions.tpl.html',
          data: { pageTitle: 'Express Solutions' }
        })
        .state('home.thai-imp-exp', {
            url: '/thai-imp-exp',
            templateUrl: 'home/thailand.tpl.html',
            data: { pageTitle: 'Thailand Import & Export' }
        })

       .state('home.public-tracking', {
           url: '/public-tracking',
           templateUrl: 'home/publicTracking/publicTracking.tpl.html',
           controller: 'publicTrackingController',
           data: { pageTitle: 'Public Tracking' }
       });
});