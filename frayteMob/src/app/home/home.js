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
.config(function config($stateProvider, $urlRouterProvider, $httpProvider, $locationProvider) {

    $stateProvider
         .state('login', {
             url: '/',
             controller: 'LoginController',
             templateUrl: 'login/login.tpl.html',
             data: { pageTitle: 'Login' }
         })
        .state('forgetPasword', {
            url: '/forgetPasword',
            controller: 'loginForgetPasswordController',
            templateUrl: 'login/loginForgetPassword/loginForgetPassword.tpl.html',
            data: { pageTitle: 'Forget Password' }
        })
             .state('home', {
                 url: '/home',
                 controller: 'HomeController',
                 templateUrl: 'home/home.tpl.html',
                 data: { pageTitle: 'Home' }
             })

        // in local when app starts the url generated = "/index.html" 
        .state('home.welcome', {
            url: '/welcome',
            templateUrl: 'home/welcome.tpl.html',
            data: { pageTitle: 'Home' }
        })

        //create bag code here
    .state('home.bag', {
        url: '/bag',
        templateUrl: 'home/homeCarton/homeCarton.tpl.html',
        controller: 'HomeCartonController',
        data: { pageTitle: 'Bag' }
    })


    //Barcode code here
     .state('home.receiving', {
         url: '/receiving',
         templateUrl: 'home/homeBarcode/homeBarcode.tpl.html',
         controller: 'HomeBarcodeController',
         data: { pageTitle: 'Receiving' }
     })

        //collection driver code here
     .state('home.collectionDriver', {
         url: '/collection-driver',
         templateUrl: 'home/homeCollectionDriver/homeCollectionDriver.tpl.html',
         controller: 'HomeCollectionDriverController',
         data: { pageTitle: 'Collection Driver' }
     })

        //collection driver handover code here
             .state('home.collectionDriverHandover', {
                 url: '/collection-driver-handover',
                 templateUrl: 'home/homeCollectionDriver/homeCollectionDriverHandover/homeCollectionDriverHandover.tpl.html',
                 controller: 'HomeCollectionDriverHandoverController',
                 data: { pageTitle: 'Collection Driver Handover' }
             })


    //dispatch code here
       .state('home.dispatch', {
           url: '/dispatch',
           templateUrl: 'home/homeCarton/homeDispatch/homeDispatch.tpl.html',
           controller: 'HomeDispatchController',
           data: { pageTitle: 'Dispatch' }
       })

    //pickup code here
          .state('home.pickup', {
              url: '/pickup',
              templateUrl: 'home/homeOrigin/homeOriginPickup.tpl.html',
              controller: 'HomeOriginPickupController',
              data: { pageTitle: 'Pickup' }
          })

    //airport dropoff code here
              .state('home.airport-dropoff', {
                  url: '/airport-dropoff',
                  templateUrl: 'home/homeAirportDropOff/homeAirportDropOff.tpl.html',
                  controller: 'HomeAirportDropOffController',
                  data: { pageTitle: 'Airport DropOff' }
              })

    //airport pickup code here
      .state('home.airport-pickup', {
          url: '/airport-pickup',
          templateUrl: 'home/homeAirportPickup/homeAirportPickup.tpl.html',
          controller: 'HomeAirportPickupController',
          data: { pageTitle: 'Airport Pick Up' }
      })

    //handover code here
          .state('home.handover', {
              url: '/handover',
              templateUrl: 'home/homeHandover/homeHandover.tpl.html',
              controller: 'HomeHandoverController',
              data: { pageTitle: 'Handover' }
          })


    ;

    $urlRouterProvider.otherwise('/')
    ;
});