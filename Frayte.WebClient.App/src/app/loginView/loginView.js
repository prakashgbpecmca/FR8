angular.module('ngApp.loginview', [
  'ui.router',
  'ngApp.common',
  'ui.grid',
  'ui.grid.resizeColumns'
])

.config(function config($stateProvider, $urlRouterProvider, $locationProvider) {
    $stateProvider
        // -- Master-Admin State Start --
         .state('loginView', {
             abstract: true,
             url: '',
             controller: 'LoginViewController',
             templateUrl: 'loginView/loginView.tpl.html'
         })
        .state('loginView.services', {
            url: '/services',
            controller: 'ServicesController',
            templateUrl: 'loginView/service.tpl.html',
            data: { pageTitle: 'Available Services' }
        })
         .state('loginView.userTabs', {
             abstract: true,
             url: '',
             controller: 'UserTabController',
             templateUrl: 'loginView/userTab.tpl.html',
             data: { pageTitle: '' }
         })
        .state('loginView.userTabs.customers', {
            url: '/customers',
            templateUrl: 'customer/customer.tpl.html',
            controller: 'CustomerController',
            data: { pageTitle: 'Customers' }
        })

        .state('loginView.userTabs.profile-setting', {
            url: '/profile-setting',
            templateUrl: 'ProfileandSetting/profileandSetting.tpl.html',
            controller: 'ProfileandSettingController',
            data: { pageTitle: 'Profile and Setting Detail' }
        })
        .state('loginView.userTabs.profile-setting.service-code', {
            url: '/service-code',
            templateUrl: 'ProfileandSetting/serviceCode/serviceCode.tpl.html',
            controller: 'ServiceCodeController',
            data: { pageTitle: 'Service Code' }
        })
        .state('loginView.userTabs.profile-setting.change-password', {
            url: '/my-profile',
            templateUrl: 'ProfileandSetting/profile/profile.tpl.html',
            controller: 'UserProfileController',
            data: { pageTitle: 'Profile Detail' }
        })
        .state('loginView.userTabs.profile-setting.api-detail', {
            url: '/api-detail',
            templateUrl: 'ProfileandSetting/api/apiUser.tpl.html',
            controller: 'ApiController',
            data: { pageTitle: 'Api Detail' }
        })

        .state('loginView.userTabs.customer-detail', {
            abstract: true,
            url: '/customer-detail/:customerId',
            templateUrl: 'customer/customerDetail/customerDetail.tpl.html',
            controller: 'CustomerBasicController',
            data: { pageTitle: 'Customer Detail' }
        })
          .state('loginView.userTabs.customer-detail.basic-detail', {
              url: '/basic-detail',
              templateUrl: 'customer/customerDetail/customerBasicDetail/customerBasicDetail.tpl.html',
              controller: 'CustomerBasicDetailController',
              data: { pageTitle: 'Customer Basic Detail' }
          })
          .state('loginView.userTabs.customer-detail.normal-margin-cost', {
              url: '/normal-margin-cost',
              templateUrl: 'customer/customerDetail/maginCost/customerMarginCost.tpl.html',
              controller: 'CustomerMarginCostController',
              data: { pageTitle: 'Customer MarginCost' }
          })
         .state('loginView.userTabs.customer-detail.advance-margin-cost', {
             url: '/advance-margin-cost',
             templateUrl: 'customer/customerDetail/advanceRateCard/customerAdvanceRateCard.tpl.html',
             controller: 'CustomerAdvanceRateCardController',
             data: { pageTitle: 'Customer AdvanceRateCard' }
         })
            .state('loginView.userTabs.customer-detail.rate-card', {
                url: '/rate-card',
                templateUrl: 'customer/customerDetail/customerRateCardSetting/customerRateCardSetting.tpl.html',
                controller: 'CustomerRateCardSettingController',
                data: { pageTitle: 'Customer RateCard' }
            })
          .state('loginView.userTabs.customer-detail.tracking-configuration', {
              url: '/tracking-configuration',
              templateUrl: 'customer/customerDetail/trackingConfiguration/trackingConfiguration.tpl.html',
              controller: 'TrackingConfiguration',
              data: { pageTitle: 'Customer Tracking Configuration' }
          })
           .state('loginView.userTabs.courier-accounts', {
               url: '/courier-accounts',
               abstract: true,
               templateUrl: 'courierAccount/countryAccount.tpl.html',
               controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('loginView.userTabs.courier-accounts.easypost-account', {
               url: '/easypost-account',
               templateUrl: 'courierAccount/easyPostCourierAccounts.tpl.html',
               controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
           .state('loginView.userTabs.courier-accounts.parcel-hub-account', {
               url: '/parcel-hub-account',
               templateUrl: 'courierAccount/parcelHubCourierAccount.tpl.html',
               controller: 'CourierAccountController',
               data: { pageTitle: 'Couriers Account' }
           })
         .state('loginView.userTabs.zone-setting', {
             abstract: true,
             url: '/zone-settings',
             templateUrl: 'zoneSetting/zoneSetting.tpl.html',
             controller: 'ZoneSettingController',
             data: { pageTitle: 'Zone Setting' }
         })
         .state('loginView.userTabs.zone-setting.country-zone', {
             url: '/country-zone',
             templateUrl: 'zoneSetting/zoneCountry/zoneCountry.tpl.html',
             controller: 'ZoneCountryController',
             data: { pageTitle: 'Country Zone' }
         })
          .state('loginView.userTabs.zone-setting.third-party-matrix', {
              url: '/3rd-party-matrix',
              templateUrl: 'zoneSetting/thirdPartyMatrix/thirdPartyMatrix.tpl.html',
              controller: 'ThirdPartyMatrixController',
              data: { pageTitle: '3rd Party Matrix' }
          })
          .state('loginView.userTabs.zone-setting.base-rate-card', {
              url: '/base-rate-card',
              templateUrl: 'zoneSetting/baseRateCard/baseRateCard.tpl.html',
              controller: 'BaseRateCardController',
              data: { pageTitle: ' Base Rate Card' }
          })
          .state('loginView.userTabs.zone-setting.zone-post-code', {
              url: '/zone-post-code',
              templateUrl: 'zoneSetting/zonePostCode/zonePostCode.tpl.html',
              controller: 'zonePostCodeController',
              data: { pageTitle: ' Zone Post/Zip Code' }
          })
         .state('loginView.userTabs.zone-setting.country-zone-post-code', {
             url: '/country-zone-post-code',
             templateUrl: 'zoneSetting/countryZonePostCode/countryZonePostCode.tpl.html',
             controller: 'CountryZonePostCodeController',
             data: { pageTitle: ' Country Zone Post/Zip Code' }
         })
          .state('loginView.userTabs.user', {
              url: '/users',
              templateUrl: 'user/user.tpl.html',
              controller: 'UserController',
              data: { pageTitle: 'Users' }
          })

        .state('loginView.userTabs.user-detail', {
            url: '/user-detail/:UserId',
            templateUrl: 'user/userDetail.tpl.html',
            controller: 'UserDetailController',
            data: { pageTitle: 'User Detail' }
        })

  // Setting Routing
       .state('loginView.userTabs.setting', {
           abstract: true,
           url: '/settings',
           templateUrl: 'setting/setting.tpl.html',
           controller: 'SettingController',
           data: { pageTitle: 'Setting' }
       })
        .state('loginView.userTabs.setting.margin-options', {
            url: '/margin-options',
            templateUrl: 'setting/margin/margin.tpl.html',
            controller: 'MarginController',
            data: { pageTitle: 'Margin Options' }
        })
         .state('loginView.userTabs.setting.service-alerts', {
             url: '/service-alerts',
             templateUrl: 'setting/systemAlerts/systemAlert.tpl.html',
             controller: 'SystemAlertController',
             data: { pageTitle: 'Service Alerts' }
         })
        .state('loginView.userTabs.setting.terms-and-conditions', {
            url: '/terms-and-conditions',
            templateUrl: 'termAndCondition/termsAndCondition.tpl.html',
            controller: 'TermAndConditionController',
            data: { pageTitle: 'Terms and Conditions' }
        })
        .state('loginView.userTabs.setting.download-excels', {
            url: '/download-excels',
            templateUrl: 'downloadExcel/downloadExcel.tpl.html',
            controller: 'DownloadExcelController',
            data: { pageTitle: 'download Excels' }
        })
         .state('loginView.userTabs.setting.report-setting', {
             url: '/report-setting',
             templateUrl: 'setting/reportSetting/reportSetting.tpl.html',
             controller: 'ReportSettingController',
             data: { pageTitle: 'Report Setting' }
         })
          .state('loginView.userTabs.setting.special-delivery-needed', {
              url: '/special-delivery-needed',
              templateUrl: 'setting/specialDelivery/specialDelivery.tpl.html',
              controller: 'SpecialDeliveryController',
              data: { pageTitle: 'Special Delivery Needed' }
          })
        .state('loginView.userTabs.setting.parcel-hub-keys', {
            url: '/parcel-hub-keys',
            templateUrl: 'setting/parcellHub/parcelhub.tpl.html',
            controller: 'ParcelHubController',
            data: { pageTitle: 'Parcel Hub Keys' }
        })
         .state('loginView.userTabs.setting.exchange-rate', {
             url: '/exchange-rate',
             templateUrl: 'setting/exchangeRate/exchangeRate.tpl.html',
             controller: 'ExchangeRateController',
             data: { pageTitle: 'Exchange Rate' }
         })
        .state('loginView.userTabs.setting.fuel-surcharge', {
            url: '/fuel-surcharge',
            templateUrl: 'setting/fuelSurCharge/fuelSurChargeSetting.tpl.html',
            controller: 'FuelSurChargeController',
            data: { pageTitle: 'Fuel Surcharge' }
        })
        .state('loginView.userTabs.setting.admin-charges', {
            url: '/admin-charges',
            templateUrl: 'setting/adminCharge/adminCharge.tpl.html',
            controller: 'AdminChargesController',
            data: { pageTitle: 'Admin Surcharge' }
        })
           // Access-Level
         .state('loginView.userTabs.access-level', {
             url: '/access-level',
             templateUrl: 'accessLevel/accessLevel.tpl.html',
             controller: 'AccessLevelController',
             data: { pageTitle: 'Access Level' }
         })

        // HS-Code
        .state('loginView.userTabs.assigned-jobs', {
            url: '/assigned-jobs',
            templateUrl: 'hsCode/hsCodeOperation.tpl.html',
            controller: 'HSCodeController',
            data: { pageTitle: 'HSCode Operation' }
        })

         .state('loginView.userTabs.dashboard', {
             url: '/dashboard',
             templateUrl: 'jobs/jobsDashboard.tpl.html',
             controller: 'JobsDashboradController',
             data: { pageTitle: 'Jobs DasdhBoard' }
         })
        // Customer Direct Booking

        //.state('customer.setting', {
        //    url: '/setting',
        //    templateUrl: 'customer/customerSetting/customerSetting.tpl.html',
        //    controller: 'CustomerSettingController',
        //    data: { pageTitle: 'Customer Setting' }
        //})
        .state('loginView.userTabs.direct-shipments', {
            url: '/track-and-trace/:moduleType',
            templateUrl: 'directBooking/directShipments/directShipments.tpl.html',
            controller: 'DirectShipmentController',
            data: { pageTitle: 'Track & Trace' }
        })
         .state('loginView.userTabs.booking-home.address-book', {
             url: '/address-book/:userId',
             templateUrl: 'customer/customerAddressBook/customerAddressBook.tpl.html',
             controller: 'CustomerAddressBookController',
             data: { pageTitle: 'Address Book', IsFired: true, IsChanged: false }
         })
         //.state('customer.manifests', {
             //    url: '/manifests',
             //    templateUrl: 'customer/customerManifest/customerManifests.tpl.html',
             //    controller: 'CustomerManifestController',
             //    data: { pageTitle: 'Manifests' }
             //})

         .state('loginView.userTabs.manifest', {
             url: '/manifests',
             abstract: true,
             templateUrl: 'customer/customerManifest/manifest.tpl.html',
             controller: 'ManifestController',
             data: { pageTitle: 'Manifests' }
         })
        .state('loginView.userTabs.manifest.userManifest', {
            url: '/customer-manifest/:moduleType',
            templateUrl: 'customer/customerManifest/userManifest/customerManifests.tpl.html',
            controller: 'UserManifestController',
            data: { pageTitle: 'Manifests' }
        })
        .state('loginView.userTabs.manifest.customManifest', {
            url: '/custom-manifest',
            templateUrl: 'customer/customerManifest/customManifest/customerManifests.tpl.html',
            controller: 'CustomManifestController',
            data: { pageTitle: 'Manifests' }
        })

         .state('loginView.userTabs.booking-home', {
             url: '/booking-home',
             abstract: true,
             templateUrl: 'customer/bookingHome/bookingWelcome.tpl.html',
             //  controller: 'BookingHomeController',
             data: { pageTitle: 'Booking Home' }
         })
        .state('loginView.userTabs.direct-booking', {
            url: '/direct-booking/:directShipmentId/:callingtype',
            templateUrl: 'directBooking/directBooking.tpl.html',
            controller: 'DirectBookingController',
            data: { pageTitle: 'Direct Booking', IsFired: true, IsChanged: false }
        })
        .state('loginView.userTabs.quick-booking', {
            url: '/quick-booking',
            templateUrl: 'customer/bookingHome/bookingHome.tpl.html',
            controller: 'BookingHomeController',
            data: { pageTitle: 'Booking' }
        })
        .state('loginView.userTabs.direct-booking-clone', {
            url: '/direct-booking-clone/:directShipmentId',
            templateUrl: 'directBooking/directBooking.tpl.html',
            controller: 'DirectBookingController',
            data: { pageTitle: 'Direct Booking', IsFired: true, IsChanged: false }
        })
          .state('loginView.userTabs.direct-booking-return', {
              url: '/direct-booking-return/:directShipmentId',
              templateUrl: 'directBooking/directBooking.tpl.html',
              controller: 'DirectBookingController',
              data: { pageTitle: 'Direct Booking', IsFired: true, IsChanged: false }
          })
        .state('loginView.userTabs.quotation', {
            url: '/quotation',
            templateUrl: 'quotationTools/quotationTools.tpl.html',
            controller: 'QuotationToolController',
            data: { pageTitle: 'Quotation' }
        })

        // Customer eCommerce 
         .state('loginView.userTabs.eCommerce-booking', {
             url: '/eCommerce-booking/:shipmentId',
             templateUrl: 'eCommerceBooking/eCommerceBooking.tpl.html',
             controller: 'eCommerceBookingController',
             data: { pageTitle: 'eCommerce Booking', IsFired: true, IsChanged: false }
         })


        .state('loginView.userTabs.eCommerce-booking-clone', {
            url: '/eCommerce-booking-clone/:shipmentId',
            templateUrl: 'eCommerceBooking/eCommerceBooking.tpl.html',
            controller: 'eCommerceBookingController',
            data: { pageTitle: 'eCommerce Booking', IsFired: true, IsChanged: false }
        })
          .state('loginView.userTabs.eCommerce-booking-return', {
              url: '/eCommerce-booking-return/:shipmentId',
              templateUrl: 'eCommerceBooking/eCommerceBooking.tpl.html',
              controller: 'eCommerceBookingController',
              data: { pageTitle: 'eCommerce Booking', IsFired: true, IsChanged: false }
          })
        //
        .state('loginView.userTabs.current-shipment', {
            url: '/current-shipment',
            templateUrl: 'shipment/currentShipment/currentShipment.tpl.html',
            controller: 'CurrentShipmentController',
            data: { pageTitle: 'Current Shipment' }
        })

        .state('loginView.userTabs.past-shipment', {
            url: '/past-shipment',
            templateUrl: 'shipment/pastShipment/pastShipment.tpl.html',
            controller: 'PastShipmentController',
            data: { pageTitle: 'Past Shipment' }
        })

         .state('loginView.userTabs.receivers', {
             url: '/receivers/:receiverId',
             templateUrl: 'receiver/receiver.tpl.html',
             controller: 'ReceiverController',
             data: { pageTitle: 'Receivers' }
         })

         .state('loginView.userTabs.receiver-detail', {
             url: '/receiver-detail/:receiverId',
             templateUrl: 'shipper/shipperAddEdit.tpl.html',
             controller: 'ShipperAddEditController',
             data: { pageTitle: 'Receiver Detail' }
         })

        .state('loginView.userTabs.manage-detail', {
            url: '/manage-detail',
            templateUrl: 'customer/customerDetail/customerDetail.tpl.html',
            controller: 'CustomerDetailController',
            data: { pageTitle: 'Manage Detail' }
        })
         .state('loginView.userTabs.upload-shipments', {
             url: '/upload-shipments',
             templateUrl: 'uploadShipment/uploadShipment.tpl.html',
             controller: 'UploadShipmentController',
             data: { pageTitle: 'Upload Shipment' }
         })
         .state('loginView.userTabs.db-upload-shipments', {
             url: '/bulk-upload',
             templateUrl: 'directBookingUploadShipment/directBookingUploadShipment.tpl.html',
             controller: 'DirectBookingUploadShipment',
             data: { pageTitle: 'Bulk Upload' }
         })
         .state('loginView.userTabs.track-and-trace-dashboard', {
             url: '/track-and-trace-dashboard',
             templateUrl: 'trackAndTraceDashBoard/trackAndtraceDashboard.tpl.html',
             controller: 'TrackAndTraceDashBoardController',
             data: { pageTitle: 'Track & Trace DashBoard' }
         })
         .state('loginView.userTabs.main-dashboard', {
             url: '/main-dashboard',
             templateUrl: 'dashboard/dashboard.tpl.html',
             controller: 'DashBoardController',
             data: { pageTitle: 'DashBoard' }
         })
          .state('loginView.userTabs.ecomm-setting', {
              abstract: true,
              url: '/ecomm-setting',
              controller: 'EcommerceSettingController',
              templateUrl: 'eCommerceSetting/eCommerceSetting.tpl.html'
          })

        .state('loginView.userTabs.setting.hscode', {
            url: '/ecomm-setting/hscode',
            templateUrl: 'eCommerceSetting/hsCodeSetting/hsCodeSetting.tpl.html',
            controller: 'HsCodeSettingController',
            data: { pageTitle: 'HsCodeSetting' }
        })

        // Tradelane 

    .state('loginView.userTabs.tradelane-booking', {
        url: '/tradelane-booking/:shipmentId',
        templateUrl: 'tradelaneBooking/tradelaneBooking.tpl.html',
        controller: 'TradelaneBookingController',
        data: { pageTitle: 'Tradelane Booking' }
    })
     .state('loginView.userTabs.tradelane-booking-clone', {
         url: '/trdelane-booking-clone/:shipmentId',
         templateUrl: 'tradelaneBooking/tradelaneBooking.tpl.html',
         controller: 'TradelaneBookingController',
         data: { pageTitle: 'Tradelane Booking', IsFired: true, IsChanged: false }
     })
         .state('loginView.userTabs.tradelane-shipments', {
             url: '/trade-booking-shipment/',
             templateUrl: 'tradelaneShipments/tradelaneShipment.tpl.html',
             controller: 'TradelaneShipmentController',
             data: { pageTitle: 'Tradelane Booking', IsFired: true, IsChanged: false }
         })
    .state('loginView.userTabs.customer-detail.tracking-configuration-tl', {
        url: '/tradelane-tracking-configuration',
        templateUrl: 'customer/customerDetail/tradelaneTrackingConfiguration/tradelaneTrackingConfiguration.tpl.html',
        controller: 'TradelaneTrackingConfigurationController',
        data: { pageTitle: 'Customer Tracking Configuration' }
        })
        .state('loginView.userTabs.tracking-milestones', {
            url: '/tracking-milestones',
            templateUrl: 'tradelaneMileStone/tradelaneTrackingMileStone.tpl.html',
            controller: 'TradelaneTrackingMileStoneController',
            data: { pageTitle: 'Tradelane Tracking MileStone' }
        })
        .state('loginView.userTabs.staff-dashboard', {
            url: '/staff-dashboard/:ShipmentType',
            templateUrl: 'tradelaneStaffBoard/tradelaneStaffBoard.tpl.html',
            controller: 'TradelaneStaffBoardController',
            data: { pageTitle: 'Staff Dashboard' }
        })
    .state('loginView.userTabs.preAlert-dashboard', {
        url: '/preAlert-tracking-dashboard',
        templateUrl: 'tradelanePreAlertDashBoard/tradelanePreAlertDashBoard.tpl.html',
        controller: 'TradelanePreAlertDashBoardController',
        data: { pageTitle: 'PreAlert And Tracking Dashboard' }
    })
    .state('loginView.userTabs.customer-detail.configuration', {
        url: '/configuration',
        templateUrl: 'customer/customerDetail/customerConfiguration/customerConfiguration.tpl.html',
        controller: 'CustomerConfiguration',
        data: { pageTitle: 'Customer Configuration' }
    })

     .state('loginView.userTabs.break-bulk-booking', {
         url: '/break-bulk-booking',
         templateUrl: 'breakbulk/breakbulkBooking/breakbulkBooking.tpl.html',
         controller: 'breakbulkBookingController',
         data: { pageTitle: 'BBK-Booking' }
        })
        .state('loginView.userTabs.break-bulk-booking-clone', {
            url: '/break-bulk-booking-clone/:shipmentId',
            templateUrl: 'breakbulk/breakbulkBooking/breakbulkBooking.tpl.html',
            controller: 'breakbulkBookingController',
            data: { pageTitle: 'BBK-Booking', IsFired: true, IsChanged: false }
        })
    .state('loginView.userTabs.break-bulk-shipments', {
        url: '/break-bulk-shipment',
        templateUrl: 'breakbulk/breakbulkShipment/breakbulkShipment.tpl.html',
        controller: 'breakbulkShipmentController',
        data: { pageTitle: 'BBK-Shipment' }
    })

     .state('loginView.userTabs.mobile-configuration', {
         url: '/mobile-configuration',
         templateUrl: 'mobileTrackingConfiguration/mobileConfiguration.tpl.html',
         controller: 'MobileTrackingConfiguration',
         data: { pageTitle: 'Mobile Tracking Configuration' }
     })
        .state('loginView.userTabs.customer-staff', {
            url: '/customer-staff',
            templateUrl: 'customerStaff/customerStaff.tpl.html',
            controller: 'CustomerStaffController',
            data: { pageTitle: 'Customer-Staff' }
        })
     .state('loginView.userTabs.manifests-bb', {
         url: '/manifest-bk',
         templateUrl: 'breakbulk/breakBulkManifest/breakbulkmanifest.tpl.html',
         controller: 'BreakbulkManifest',
         data: { pageTitle: 'BBK-Manifest' }
     })

    .state('loginView.userTabs.public-tracking-configuration', {
        url: '/public-tracking-configuration',
        templateUrl: 'tracking/trackingConfiguration.tpl.html',
        controller: 'PublicTrackingConfiguration',
        data: { pageTitle: 'Public Tracking Configuration' }
    })

        //express solutions routing
    .state('loginView.userTabs.express-solution-create-shipment', {
        url: '/express-create-Shipment',
        templateUrl: 'express/expressAwbShipment/expressAwbShipment.tpl.html',
        controller: 'ExpressAWBShipmentController',
        data: { pageTitle: 'Express-CreateShipments' }
    })
     .state('loginView.userTabs.express-solution-booking', {
         url: '/express-solutions-Booking/:shipmentId',
         templateUrl: 'express/expressBooking/expressBooking.tpl.html',
         controller: 'ExpressBookingController',
         data: { pageTitle: 'Express-Booking' }
     })
    .state('loginView.userTabs.express-solution-booking-clone', {
        url: '/express-solutions-booking-clone/:shipmentId',
        templateUrl: 'express/expressBooking/expressBooking.tpl.html',
        controller: 'ExpressBookingController',
        data: { pageTitle: 'Express-Booking' }
        })
        .state('loginView.userTabs.express-solution-booking-return', {
            url: '/express-solutions-booking-return/:shipmentId',
            templateUrl: 'express/expressBooking/expressBooking.tpl.html',
            controller: 'ExpressBookingController',
            data: { pageTitle: 'Express-Booking' }
        })
    .state('loginView.userTabs.express-solution-shipments', {
        url: '/express-solutions-shipments',
        templateUrl: 'express/expressShipments/expressShipment.tpl.html',
        controller: 'ExpressShipmentController',
        data: { pageTitle: 'Express-Shipment' }
    })
    .state('loginView.userTabs.manifests-es', {
        url: '/manifest-es',
        templateUrl: 'express/expressManifest/expressManifest.tpl.html',
        controller: 'ExpressManifestController',
        data: { pageTitle: 'Express-Manifest' }
    })
    ;
});